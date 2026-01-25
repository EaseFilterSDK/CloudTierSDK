using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;

using CloudTier.CommonObjects;
using CloudTier.FilterControl;
using CloudTier.AmazonS3Sup;

namespace CloudTierS3Demo
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
        static private Dictionary<string, AsyncTask> workingTasks = new Dictionary<string, AsyncTask>();

        public static bool StartService(StartType _startType, ListView listView_Message, out string lastError)
        {
            bool ret = true;
            lastError = string.Empty;

            startType = _startType;

            try
            {
                //Purchase a license key with the link: http://www.easefilter.com/Order.htm
                //Email us to request a trial key: info@easefilter.com //free email is not accepted.
                string licenseKey = GlobalConfig.LicenseKey;

                if(!filterControl.StartFilter((int)GlobalConfig.FilterConnectionThreads, GlobalConfig.ConnectionTimeOut, licenseKey, ref lastError))
                {
                    return false;
                }

                filterControl.ByPassWriteEventOnReHydration = GlobalConfig.ByPassWriteEventOnReHydration;
                filterControl.ExcludeProcessIdList = GlobalConfig.ExcludePidList;

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
            filterControl.StopFilter();

            return true;
        }

        public static bool DownloadAmazonS3File(FilterRequestEventArgs e)
        {
            bool ret = false;
            bool isCacheFileDownloaded = false;

            try
            {
                //this is the tag data from the stub file.
                string tagDataString = Encoding.Unicode.GetString(e.TagData);
                tagDataString = tagDataString.Substring(0, e.TagDataLength / 2);

                string siteName = tagDataString.Substring(0, tagDataString.IndexOf(";"));
                string remotePath = tagDataString.Substring(siteName.Length + 1);

                siteName = siteName.Replace("sitename:", "");
                
                AmazonS3SiteInfo siteInfo = CloudUtil.GetSiteInfoBySiteName(siteName);

                if( null == siteInfo )
                {
                    EventManager.WriteMessage(90, "Download S3 File", EventLevel.Error, "Download file " + e.FileName + " from S3 failed, the site name:"
                        + siteName + " can't be found from the configuration setting.");
                    e.ReturnStatus = FilterAPI.NTSTATUS.STATUS_UNSUCCESSFUL;

                    return false;
                }

                string returnCacheFileName = Path.Combine(GlobalConfig.CacheFolder, siteName);
                returnCacheFileName = Path.Combine(returnCacheFileName, remotePath).Replace("/", "\\");

                string cacheFolder = Path.GetDirectoryName(returnCacheFileName);
                if (!Directory.Exists(cacheFolder))
                {
                    Directory.CreateDirectory(cacheFolder);
                }

                if (File.Exists(returnCacheFileName))
                {
                    isCacheFileDownloaded = true;
                }
                else
                {

                    AsyncTask asyncTask = null;

                    lock (workingTasks)
                    {
                        if (workingTasks.ContainsKey(remotePath.ToLower()))
                        {
                            asyncTask = workingTasks[remotePath.ToLower()];
                            asyncTask.CompletedEvent.WaitOne();
                        }
                    }

                    if (null == asyncTask)
                    {

                        asyncTask = new AsyncTask(TaskType.DownloadFile, siteInfo, returnCacheFileName, remotePath,
                            e.FileSize, DateTime.FromFileTime(e.CreationTime), (FileAttributes)e.Attributes
                            , 0, "", "");

                        lock (workingTasks)
                        {
                            if (!workingTasks.ContainsKey(remotePath.ToLower()))
                            {
                                workingTasks[remotePath.ToLower()] = asyncTask;
                            }
                        }

                        AmazonS3 s3 = new AmazonS3(siteInfo, null, asyncTask);
                        Task downloadTask = s3.DownloadAsync();
                        downloadTask.Wait();
                        asyncTask.CompleteTask("");

                        lock (workingTasks)
                        {
                            if (workingTasks.ContainsKey(remotePath.ToLower()))
                            {
                                workingTasks.Remove(remotePath.ToLower());
                            }
                        }
                    }

                    if(asyncTask.IsTaskCompleted && asyncTask.State == TaskState.completed && File.Exists(returnCacheFileName))
                    {
                        isCacheFileDownloaded = true;
                    }
                    else
                    {
                        //the file didn't download successfully
                        File.Delete(returnCacheFileName);
                    }
                }

                if (isCacheFileDownloaded)
                {
                    e.ReturnCacheFileName = returnCacheFileName;

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

                    ret = true;
                }
                else
                {
                    EventManager.WriteMessage(130, "Download S3 File", EventLevel.Error, "Download file " + e.FileName + " from S3 failed. Cache file:" + returnCacheFileName + " doesn't exist.");
                    e.ReturnStatus = FilterAPI.NTSTATUS.STATUS_UNSUCCESSFUL;
                }

            }
            catch( Exception ex)
            {
                EventManager.WriteMessage(190, "Download S3 File", EventLevel.Error, "Download file " + e.FileName + " from S3 got exception:" + ex.Message);
                e.ReturnStatus = FilterAPI.NTSTATUS.STATUS_UNSUCCESSFUL;
            }

            return ret;

        }

        static void OnFilterRequestHandler(object sender, FilterRequestEventArgs e)
        {
            Boolean ret = true;

            try
            {
                //this is the tag data from the stub file.
                string tagDataString = Encoding.Unicode.GetString(e.TagData);

                if (tagDataString.StartsWith("sitename:"))
                {
                    //this is the stub file associated to the Amazon S3 storage.
                    DownloadAmazonS3File(e);
                }
                else
                {
                    //this is the local test stub file.
                    string cacheFileName = tagDataString.Substring(0, e.TagDataLength / 2);

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
                         + ",ReturnLength:" + e.ReturnBufferLength + " fileName:" + e.FileName + ",cacheFileName:" + tagDataString);

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(181, "ProcessRequest", EventLevel.Error, "Process request exception:" + ex.Message);
            }

        }
    }
}
