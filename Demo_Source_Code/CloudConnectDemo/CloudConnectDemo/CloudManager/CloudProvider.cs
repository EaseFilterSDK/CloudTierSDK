
///////////////////////////////////////////////////////////////////////////////
//
//    (C) Copyright 2011 EaseFilter Technologies
//    All Rights Reserved
//
//    This software is part of a licensed software product and may
//    only be used or copied in accordance with the terms of that license.
//
//    NOTE:  THIS MODULE IS UNSUPPORTED SAMPLE CODE
//
//    This module contains sample code provided for convenience and
//    demonstration purposes only,this software is provided on an 
//    "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
//     either express or implied.  
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

using EaseFilter.GlobalObjects;


namespace EaseFilter.CloudManager
{

    public class CloudProvider : IDisposable 
    {
        private static readonly Task _completedTask = Task.FromResult(false);
        /// <summary>
        /// Download cloud storage file list 
        /// </summary>
        /// <param name="directoryName">the directory name</param>
        /// <param name="refresh">if it is true, it will always to get data from remote storage, or it check local cache first,
        /// if there are data in local, then load it or get it from remote storage.</param>
        /// <returns></returns>
        public virtual async Task<DirectoryList> DownloadFileListAsync(string directoryName, bool refresh) { await _completedTask; return new DirectoryList(); }
        /// <summary>
        /// Delete the cloud storage file.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> DeleteFileAsync() { await _completedTask; return false; }
        /// <summary>
        /// Download the cloud storage file.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> DownloadAsync() { await _completedTask; return false; }
        /// <summary>
        /// Upload the cloud storage file.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> UploadAsync() { await _completedTask; return false; }
        /// <summary>
        /// Create the cloud storage directory.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> MakeDirAsync() { await _completedTask; return false; }
        /// <summary>
        /// Rename or move the cloud storage file name.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> RenameFileAsync() { await _completedTask; return false; }
        /// <summary>
        /// Check if the cloud storage directory exist.
        /// </summary>
        /// <param name="directoryName"></param>
        /// <returns></returns>
        public virtual async Task<bool> IsDirectoryExistAsync(string directoryName) { await _completedTask; return false; }
        /// <summary>
        /// Test the cloud storage connection.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public virtual bool TestConnection(ref string errorMessage) { return false; }

        protected SiteInfo siteInfo = null;
      
        protected bool isConnected = false;
        protected AsyncTask asyncTask = null;

        public delegate void UpdataStatusDlgt(string message, bool isErrorMessage);
        protected UpdataStatusDlgt updataStatusDlgt = null;

        public virtual void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these  
            // operations, as well as in your methods that use the resource. 
            //if (!_disposed)
            //{
            //    if (disposing)
            //    {
            //        if (_resource != null)
            //            _resource.Dispose();
            //        Console.WriteLine("Object disposed.");
            //    }

            //    // Indicate that the instance has been disposed.
            //    _resource = null;
            ////    _disposed = true;
            //}
        }


        public async Task ProcessTaskAsync(AsyncTask asyncTask)
        {
            string lastError = string.Empty;

            try
            {
                this.asyncTask = asyncTask;

                switch (asyncTask.TaskType)
                {
                    case TaskType.DownloadBlock:
                    case TaskType.DownloadFile:
                        {
                            await DownloadAsync();
                            break;
                        }
                    case TaskType.UploadFile:
                        {
                            await UploadAsync();
                            break;
                        }
                    case TaskType.DeleteFile:
                        {
                            await DeleteFileAsync();
                            break;
                        }
                    case TaskType.Rename:
                        {
                            await RenameFileAsync();
                            break;
                        }

                    case TaskType.CreateDirectory:
                        {
                            await MakeDirAsync();
                            break;
                        }
                
                }

            }
            catch (Exception ex)
            {
                lastError = "Process async task type:" + asyncTask.TaskType.ToString() + " fileName " + asyncTask.LocalFileName + " failed with error:" + ex.Message;
                EventManager.WriteMessage(167, "ProcessAsyncTask", EventLevel.Error, lastError);
            }

           
            return ;

        }


    }

    public class CloudBroker
    {

        public static CloudProvider GetProvider(SiteInfo currentSiteInfo, CloudProvider.UpdataStatusDlgt updataStatusDlgt)
        {
            CloudProvider cloudProvider = null;

            try
            {
                switch (currentSiteInfo.CloudProvider)
                {
                    case CloudProviderType.AzureStorage: cloudProvider = new AzureStorage(currentSiteInfo, updataStatusDlgt); break;
                    case CloudProviderType.Amazon_S3: cloudProvider = new AmazonS3Provider(currentSiteInfo, updataStatusDlgt); break;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("connect to " + currentSiteInfo.SiteName + "' failed with error:" + ex.Message);
            }

            return cloudProvider;
        }

       
    }

  
}
