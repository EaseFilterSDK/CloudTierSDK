using System;
using System.Text;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;

using EaseFilter.CommonObjects;

namespace CloudTierDemo
{
    public class FilterWorker
    {
        public enum StartType
        {
            WindowsService = 0,
            GuiApp,
            ConsoleApp
        }

        static StartType startType = StartType.GuiApp;
        static FilterMessage filterMessage = null;

        public static bool StartService(StartType _startType, ListView listView_Message, out string lastError)
        {
            bool ret = false;
            lastError = string.Empty;

            startType = _startType;

            try
            {
                //Purchase a license key with the link: http://www.easefilter.com/Order.htm
                //Email us to request a trial key: info@easefilter.com //free email is not accepted.
                string licenseKey =  GlobalConfig.LicenseKey;                           

                ret = FilterAPI.StartFilter(licenseKey
                                            , (int)GlobalConfig.FilterConnectionThreads
                                            , new FilterAPI.FilterDelegate(FilterCallback)
                                            , new FilterAPI.DisconnectDelegate(DisconnectCallback)
                                            , ref lastError);
                if (!ret)
                {
                    return ret;
                }

                GlobalConfig.SendConfigSettingsToFilter();

                filterMessage = new FilterMessage(listView_Message, startType == StartType.ConsoleApp);

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(104, "StartFilter", EventLevel.Error, "Start filter service failed with error " + ex.Message);
                ret = false;
            }

            return ret;
        }

        public static bool StopService()
        {
            GlobalConfig.Stop();
            FilterAPI.StopFilter();

            return true;
        }

        static Boolean FilterCallback(IntPtr sendDataPtr, IntPtr replyDataPtr)
        {
            Boolean ret = true;

            try
            {
                FilterAPI.MessageSendData messageSend = new FilterAPI.MessageSendData();
                messageSend = (FilterAPI.MessageSendData)Marshal.PtrToStructure(sendDataPtr, typeof(FilterAPI.MessageSendData));

                if (FilterAPI.MESSAGE_SEND_VERIFICATION_NUMBER != messageSend.VerificationNumber)
                {
                    EventManager.WriteMessage(139, "FilterCallback", EventLevel.Error, "Received message corrupted.Please check if the MessageSendData structure is correct.");
                    return false;
                }
              
                //here we store our cache file name in stub file tag data, you can customize your own tag data here.
                if (messageSend.DataBufferLength == 0)
                {
                    Console.WriteLine("There are no tag data for stub file " + messageSend.FileName + ", return false here.");
                    return false;
                }

                FilterAPI.MessageReplyData messageReply = new FilterAPI.MessageReplyData();
              
                if (replyDataPtr.ToInt64() != 0)
                {
                    messageReply = (FilterAPI.MessageReplyData)Marshal.PtrToStructure(replyDataPtr, typeof(FilterAPI.MessageReplyData));

                    messageReply.MessageId = messageSend.MessageId;
                    messageReply.MessageType = messageSend.MessageType;

                    //here you can control the IO behaviour and modify the data.
                    FilterWorker.ProcessRequest(messageSend,ref messageReply);

                    Marshal.StructureToPtr(messageReply, replyDataPtr, true);
                }

                if (startType != StartType.WindowsService)
                {
                    filterMessage.AddMessage(messageSend, messageReply);
                }

                return ret;
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(134, "FilterCallback", EventLevel.Error, "filter callback exception." + ex.Message);
                return false;
            }

        }

        static void DisconnectCallback()
        {
            Console.WriteLine("Filter Disconnected.");
        }

        /// <summary>
        /// This function just demo how to create cache file from test folder, in your application you can get it from remote site with TCP/IP.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        static bool DownloadCacheFile(FilterAPI.MessageSendData messageSend, string cacheFileName,ref FilterAPI.MessageReplyData messageReply)
        {
            bool ret = false;

            try
            {
                string returnCacheFileName = cacheFileName;

                //for the test,we already create the cache file, you need to customize your own cache file.
                //download the data to the cache file and return the cache file name to the filter driver.

                if (!File.Exists(cacheFileName))
                {
                    EventManager.WriteMessage(34,"DownloadCacheFile", EventLevel.Error,"Can't find the cache file " + cacheFileName);
                    ret = false;
                }
                else
                {
                    if ((messageSend.CreateOptions & (uint)WinData.CreateOptions.FO_REMOTE_ORIGIN) > 0)
                    {
                        //this is the request comes from remote server
                        if (cacheFileName.StartsWith("\\")) //the cache file name is UNC path,
                        {
                            //we can't return the UNC path back for the remote access.
                            //we need to create local cache file here, you need to setup a cache folder, here we just hardcode the folder.
                            string cacheFolder = "c:\\filterTest\\cacheFolder";

                            if (!Directory.Exists(cacheFolder))
                            {
                                Directory.CreateDirectory(cacheFolder);
                            }

                            returnCacheFileName = Path.Combine(cacheFolder,Guid.NewGuid() + "." + Path.GetFileName(cacheFileName));

                            File.Copy(cacheFileName, returnCacheFileName);

                        }
                    }

                    if (messageReply.DataBuffer != null && messageReply.DataBuffer.Length >= returnCacheFileName.Length * 2)
                    {
                        messageReply.DataBufferLength = (uint)returnCacheFileName.Length * 2;
                        Array.Copy(Encoding.Unicode.GetBytes(returnCacheFileName), messageReply.DataBuffer, messageReply.DataBufferLength);

                        //if you want to rehydrate the stub file, please return with REHYDRATE_FILE_VIA_CACHE_FILE
                        if (GlobalConfig.RehydrateFileOnFirstRead)
                        {
                            messageReply.FilterStatus = (uint)FilterAPI.FilterStatus.REHYDRATE_FILE_VIA_CACHE_FILE;
                        }
                        else
                        {
                            messageReply.FilterStatus = (uint)FilterAPI.FilterStatus.CACHE_FILE_WAS_RETURNED;
                        }
                        
                        messageReply.ReturnStatus = (uint)FilterAPI.NTSTATUS.STATUS_SUCCESS;

                        EventManager.WriteMessage(47, "DownloadCacheFile", EventLevel.Verbose, "Return cache return filterStatus:" + messageReply.FilterStatus + " returnStatus:" + messageReply.ReturnStatus + " fileName:" + returnCacheFileName);

                        ret = true;
                    }
                }

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(56, "DownloadCacheFile", EventLevel.Error, "DownloadCacheFile " + cacheFileName + " exception:" + ex.Message);
                ret = false;
            }


            return ret;
        }

        /// <summary>
        /// Here we assume the data was stored in cache file name, and the cache file name was stored in stub file tag data.
        /// You can customize your own tag data, and return the block data which the user requested.
        /// </summary>
        /// <param name="cacheFileName"></param>
        /// <param name="offset"></param>
        /// <param name="readLength"></param>
        /// <param name="messageReply"></param>
        /// <returns></returns>
        static bool GetRequestedBlockData(string cacheFileName, long offset, uint readLength, ref FilterAPI.MessageReplyData messageReply)
        {
            bool ret = false;

            try
            {
                messageReply.ReturnStatus = (uint)FilterAPI.NTSTATUS.STATUS_UNSUCCESSFUL;

                if (!File.Exists(cacheFileName))
                {
                    EventManager.WriteMessage(86, "GetRequestedBlockData", EventLevel.Error, "Can't find the cache file " + cacheFileName);
                    return false;
                }

                FileStream fs = new FileStream(cacheFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fs.Position = offset;

                int returnReadLength = fs.Read(messageReply.DataBuffer, 0, (int)readLength);
                messageReply.DataBufferLength = (uint)returnReadLength;

                messageReply.ReturnStatus = (uint)FilterAPI.NTSTATUS.STATUS_SUCCESS;
                messageReply.FilterStatus = (uint)FilterAPI.FilterStatus.BLOCK_DATA_WAS_RETURNED;

                fs.Close();
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(104, "GetRequestedBlockData", EventLevel.Error, "RestoreBlockData cache file " + cacheFileName + " offset " + offset + " readLength " + readLength + " exception:" + ex.Message);
                return false;
            }

            return ret;
        }


        public static Boolean ProcessRequest(FilterAPI.MessageSendData messageSend, ref FilterAPI.MessageReplyData messageReply)
        {
            Boolean ret = false;

            try
            {

                //here the data buffer is the reparse point tag data, in our test, we assume the reparse point tag data is the cache file name of the stub file.
                string cacheFileName = Encoding.Unicode.GetString(messageSend.DataBuffer);
                cacheFileName = cacheFileName.Substring(0, (int)messageSend.DataBufferLength / 2);

                if (messageSend.MessageType == (uint)FilterAPI.MessageType.MESSAGE_TYPE_RESTORE_FILE_TO_CACHE)
                {
                    //for the first write request, the filter driver needs to restore the whole file first,
                    //here we need to download the whole cache file and return the cache file name to the filter driver,
                    //the filter driver will replace the stub file data with the cache file data.

                    //for memory mapping file open( for example open file with notepad in local computer,
                    //it also needs to download the whole cache file and return the cache file name to the filter driver,
                    //the filter driver will read the cache file data, but it won't restore the stub file.

                    ret = DownloadCacheFile(messageSend,cacheFileName, ref messageReply);
                }
                else if (messageSend.MessageType == (uint)FilterAPI.MessageType.MESSAGE_TYPE_RESTORE_BLOCK_OR_FILE)
                {

                    //for this request, the user is trying to read block of data, you can either return the whole cache file
                    //or you can just restore the block of data as the request need, you also can rehydrate the file at this point.

                    //if the whole cache file was restored, you better to return the cache file instead of block data.
                    if (GlobalConfig.RehydrateFileOnFirstRead || GlobalConfig.ReturnCacheFileName)
                    {
                        ret = DownloadCacheFile(messageSend, cacheFileName, ref messageReply);
                    }
                    else
                    {
                        ret = GetRequestedBlockData(cacheFileName, messageSend.Offset, messageSend.Length, ref messageReply);
                    }
                }
                else
                {
                    EventManager.WriteMessage(158, "ProcessRequest", EventLevel.Error, "File " + messageSend.FileName + " messageType:" + messageSend.MessageType + " unknow.");

                    messageReply.ReturnStatus = (uint)FilterAPI.NTSTATUS.STATUS_UNSUCCESSFUL;
                }

                messageReply.MessageId = messageSend.MessageId;
                messageReply.MessageType = messageSend.MessageType;

                EventLevel eventLevel = EventLevel.Information;
                if (messageReply.ReturnStatus != (uint)FilterAPI.NTSTATUS.STATUS_SUCCESS)
                {
                    eventLevel = EventLevel.Error;
                }

                EventManager.WriteMessage(169, "ProcessRequest", eventLevel, "Return MessageId#" + messageSend.MessageId
                         + " ReturnStatus:" + ((FilterAPI.NTSTATUS)(messageReply.ReturnStatus)).ToString() + ",FilterStatus:" + messageReply.FilterStatus
                         + ",ReturnLength:" + messageReply.DataBufferLength + " fileName:" + messageSend.FileName + ",cacheFileName:" + cacheFileName);

                ret = true;

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(181, "ProcessRequest", EventLevel.Error, "Process request exception:" + ex.Message);
                return false;
            }


            return ret;

        }
    }
}
