using System;
using System.Text;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;

using CloudTier.CommonObjects;
using CloudTier.FilterControl;

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

        static FilterControl filterControl = new FilterControl();

        static StartType startType = StartType.GuiApp;
        static FilterMessage filterMessage = null;

        public static bool StartService(StartType _startType, ListView listView_Message, out string lastError)
        {
            bool ret = true;
            lastError = string.Empty;

            startType = _startType;

            try
            {
                //Purchase a license key with the link: http://www.easefilter.com/Order.htm
                //Email us to request a trial key: info@easefilter.com //free email is not accepted.
                string licenseKey =  GlobalConfig.LicenseKey;

                if(!filterControl.StartFilter((int)GlobalConfig.FilterConnectionThreads, GlobalConfig.ConnectionTimeOut, licenseKey, ref lastError))
                {
                    return false;
                }

                filterControl.ByPassWriteEventOnReHydration = GlobalConfig.ByPassWriteEventOnReHydration;

                if (!filterControl.SendConfigSettingsToFilter(ref lastError))
                {
                    return false;
                }

                filterControl.OnFilterRequest += OnFilterRequestHandler;

                filterMessage = new FilterMessage(listView_Message, startType == StartType.ConsoleApp);

            }
            catch (Exception ex)
            {
                lastError = "Start filter service failed with error " + ex.Message;
                EventManager.WriteMessage(104, "StartFilter", EventLevel.Error, lastError);
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

        static void OnFilterRequestHandler(object sender, FilterRequestEventArgs e)
        {
            Boolean ret = true;

            try
            {

                //here the data buffer is the reparse point tag data, in our test, we assume the reparse point tag data is the cache file name of the stub file.
                string cacheFileName = Encoding.Unicode.GetString(e.TagData);
                cacheFileName = cacheFileName.Substring(0, e.TagDataLength / 2);

                if (e.MessageType == FilterAPI.MessageType.MESSAGE_TYPE_RESTORE_FILE_TO_CACHE)
                {
                    //for the write request, the filter driver needs to restore the whole file first,
                    //here we need to download the whole cache file and return the cache file name to the filter driver,
                    //the filter driver will replace the stub file data with the cache file data.

                    //for memory mapped file open( for example open file with notepad in local computer )
                    //it also needs to download the whole cache file and return the cache file name to the filter driver,
                    //the filter driver will read the cache file data, but it won't restore the stub file.

                    e.ReturnCacheFileName = cacheFileName;

                    //if you want to rehydrate the stub file, please return with REHYDRATE_FILE_VIA_CACHE_FILE
                    if (GlobalConfig.RehydrateFileOnFirstRead)
                    {
                        e.FilterStatus = FilterAPI.FilterStatus.REHYDRATE_FILE_VIA_CACHE_FILE;
                    }
                    else
                    {
                        e.FilterStatus = FilterAPI.FilterStatus.CACHE_FILE_WAS_RETURNED;
                    }

                    e.ReturnStatus = (uint)FilterAPI.NTSTATUS.STATUS_SUCCESS;
                }
                else if (e.MessageType == FilterAPI.MessageType.MESSAGE_TYPE_RESTORE_BLOCK_OR_FILE)
                {

                    e.ReturnCacheFileName = cacheFileName;

                    //for this request, the user is trying to read block of data, you can either return the whole cache file
                    //or you can just restore the block of data as the request need, you also can rehydrate the file at this point.

                    //if you want to rehydrate the stub file, please return with REHYDRATE_FILE_VIA_CACHE_FILE
                    if (GlobalConfig.RehydrateFileOnFirstRead)
                    {
                        e.FilterStatus = FilterAPI.FilterStatus.REHYDRATE_FILE_VIA_CACHE_FILE;
                    }
                    else if (GlobalConfig.ReturnCacheFileName)
                    {
                        e.FilterStatus = FilterAPI.FilterStatus.CACHE_FILE_WAS_RETURNED;
                    }
                    else
                    {
                        //we return the block the data back to the filter driver.
                        FileStream fs = new FileStream(cacheFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        fs.Position = e.ReadOffset;

                        int returnReadLength = fs.Read(e.ReturnBuffer, 0, (int)e.ReadLength);
                        e.ReturnBufferLength = (uint)returnReadLength;                        

                        e.FilterStatus = FilterAPI.FilterStatus.BLOCK_DATA_WAS_RETURNED;                       

                        fs.Close();

                    }

                    e.ReturnStatus = FilterAPI.NTSTATUS.STATUS_SUCCESS;
                }
                else
                {
                    EventManager.WriteMessage(158, "ProcessRequest", EventLevel.Error, "File " + e.FileName + " messageType:" + e.MessageType + " unknow.");

                    e.ReturnStatus = FilterAPI.NTSTATUS.STATUS_UNSUCCESSFUL;

                    ret = false;
                }

                if (startType != StartType.WindowsService)
                {
                    filterMessage.DisplayMessage(e);
                }

                EventLevel eventLevel = EventLevel.Information;
                if (!ret)
                {
                    eventLevel = EventLevel.Error;
                }

                EventManager.WriteMessage(169, "ProcessRequest", eventLevel, "Return MessageId#" + e.MessageId
                         + " ReturnStatus:" + ((FilterAPI.NTSTATUS)(e.ReturnStatus)).ToString() + ",FilterStatus:" + e.FilterStatus
                         + ",ReturnLength:" + e.ReturnBufferLength + " fileName:" + e.FileName + ",cacheFileName:" + cacheFileName);

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(181, "ProcessRequest", EventLevel.Error, "Process request exception:" + ex.Message);
            }

        }
    }
}
