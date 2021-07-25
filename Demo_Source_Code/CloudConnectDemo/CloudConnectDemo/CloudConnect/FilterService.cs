using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security.Principal;

using EaseFilter.GlobalObjects;
using EaseFilter.CloudManager;

namespace CloudConnect
{
    public class FilterService
    {


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
                            //we need to create local cache file here, you need to setup a cache folder, 
                            string cacheFolder = GlobalConfig.testSourceFolder;//"c:\\filterTest\\cacheFolder";

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

                        //if you want to restore the stub file, please return with RESTORE_STUB_FILE_WITH_CACHE_FILE
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

        private static bool DownloadFileFromCloudStorage(FilterAPI.MessageSendData messageSend,string testSourceFileName, ref FilterAPI.MessageReplyData messageReply)
        {
            Boolean ret = false;
            try
            {
                string siteName = testSourceFileName.Substring(0, testSourceFileName.IndexOf(";"));
                string remotePath = testSourceFileName.Substring(siteName.Length + 1);
                siteName = siteName.Replace("sitename:","");
                SiteInfo siteInfo = CloudUtil.GetSiteInfoBySiteName(siteName);
                string returnCacheFileName = Path.Combine(GlobalConfig.CloudCacheFolder, siteInfo.CloudProvider.ToString()+"\\" + siteName);
                returnCacheFileName = Path.Combine(returnCacheFileName, remotePath);
                returnCacheFileName = returnCacheFileName.Replace("/", "\\");
                string cacheFolder = Path.GetDirectoryName(returnCacheFileName);

                if(!Directory.Exists(cacheFolder))
                {
                    Directory.CreateDirectory(cacheFolder);
                }

                CloudProvider cloudProvider = CloudBroker.GetProvider(siteInfo, null);
                if(null == cloudProvider)
                {
                    EventManager.WriteMessage(150, "downloadFileFromCloud", EventLevel.Error, "download cloud file " + testSourceFileName + "failed, can't find the cloud provider.");

                    return false;
                }

                AsyncTask asyncTask = new AsyncTask(TaskType.DownloadFile, siteInfo, returnCacheFileName, remotePath, 
                    messageSend.FileSize,DateTime.FromFileTime(messageSend.CreationTime), (FileAttributes)messageSend.FileAttributes
                    , 0, "", "");

                Task downlaodTask = cloudProvider.ProcessTaskAsync(asyncTask);
                downlaodTask.Wait();

                if (downlaodTask.IsCompleted && messageReply.DataBuffer != null && messageReply.DataBuffer.Length >= returnCacheFileName.Length * 2)
                {
                    asyncTask.CompleteTask("");

                    messageReply.DataBufferLength = (uint)returnCacheFileName.Length * 2;
                    Array.Copy(Encoding.Unicode.GetBytes(returnCacheFileName), messageReply.DataBuffer, messageReply.DataBufferLength);

                    //if you want to restore the stub file, please return with RESTORE_STUB_FILE_WITH_CACHE_FILE
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
            catch(Exception ex)
            {
                ret = false;
                EventManager.WriteMessage(140, "DownloadFileFromCloudStorage", EventLevel.Error, "DownloadFileFromCloudStorage " + messageSend.FileName 
                    + " failed:" + ex.Message);

            }

            return ret;
        }
        public static Boolean ProcessRequest(FilterAPI.MessageSendData messageSend, ref FilterAPI.MessageReplyData messageReply)
        {
            Boolean ret = false;

            try
            {

                //here the data buffer is the reparse point tag data, in our test, we assume the reparse point tag data is the cache file name of the stub file.
                string testSourceFileName = Encoding.Unicode.GetString(messageSend.DataBuffer);
                testSourceFileName = testSourceFileName.Substring(0, (int)messageSend.DataBufferLength / 2);

                if(testSourceFileName.StartsWith("sitename:"))
                {
                    //this is test cloud stub file, it needs to download the file from the cloud storage
                    ret = DownloadFileFromCloudStorage(messageSend, testSourceFileName, ref messageReply);

                }
                else if (messageSend.MessageType == (uint)FilterAPI.MessageType.MESSAGE_TYPE_RESTORE_FILE_TO_CACHE)
                {
                    //for the first write request, the filter driver needs to restore the whole file first,
                    //here we need to download the whole cache file and return the cache file name to the filter driver,
                    //the filter driver will replace the stub file data with the cache file data.

                    //for memory mapping file open( for example open file with notepad in local computer,
                    //it also needs to download the whole cache file and return the cache file name to the filter driver,
                    //the filter driver will read the cache file data, but it won't restore the stub file.

                    ret = DownloadCacheFile(messageSend,testSourceFileName, ref messageReply);
                }
                else if (messageSend.MessageType == (uint)FilterAPI.MessageType.MESSAGE_TYPE_RESTORE_BLOCK_OR_FILE)
                {

                    //for this request, the user is trying to read block of data, you can either return the whole cache file
                    //or you can just restore the block of data as the request need, you also can rehydrate the file at this point.

                    //if the whole cache file was restored, you better to return the cache file instead of block data.
                    if (GlobalConfig.RehydrateFileOnFirstRead || GlobalConfig.ReturnCacheFileName)
                    {
                        ret = DownloadCacheFile(messageSend, testSourceFileName, ref messageReply);
                    }
                    else
                    {
                        ret = GetRequestedBlockData(testSourceFileName, messageSend.Offset, messageSend.Length, ref messageReply);
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
                         + ",ReturnLength:" + messageReply.DataBufferLength + " fileName:" + messageSend.FileName + ",cacheFileName:" + testSourceFileName);

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
