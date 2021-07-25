
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
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;


namespace EaseFilter.GlobalObjects
{

    public enum TaskType
    {
        UploadFile = 0,
        DownloadBlock,
        DownloadFile,
        DeleteFile,
        Rename,
        CreateDirectory,
        DownloadDirectoryList,
    }

    public enum TaskState
    {
        running = 0,
        cancel,
        completed,
        error,
    }

    public struct WriteInfo
    {
        public long offset;
        public byte[] buffer;
        public int bufferLength;
    }

    public enum MetaData
    {
        md5Base64String = 0,
        encryptionIV,
    }

    /// <summary>
    /// Async task includes file upload,file download
    /// File block download:    1.  if startOffset is 0, by default the task will continue to download.If this is sequential read,the filter driver will update the last access time,
    ///                             if the last access time already over more than 3(configurable) seconds,then stop download.
    /// 
    /// 
    /// </summary>
    public class AsyncTask:IDisposable
    {        
     
        private bool disposed = false;
        private SiteInfo siteInfo = null;
        private string taskId = Guid.NewGuid().ToString("N");
        private TaskType taskType = TaskType.DownloadFile;

        private TaskState taskState = TaskState.running;
        private DateTime startTime = DateTime.Now;
        private DateTime endTime = DateTime.Now;
        private DateTime creationTime = DateTime.Now;
        private FileAttributes fileAttributes = FileAttributes.Normal;
        
        private DateTime lastAccessTime = DateTime.Now;

        //it only one thread can get the notification.
        private AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        private DateTime taskDisplayTime = DateTime.Now;
       
        private long fileSize = -1;
        //the number of bytes was trasfferred from server
        private long transferredSize = 0;
        //the number of bytes of the cached file was downloaded
        private long downloadedSize = 0;

        private long startOffset = 0;
        private long blockSize = GlobalConfig.BlockSize;
        private long nextBlockOffset = 0;        
         
        private string remoteFileName = string.Empty;
        private string remoteDestFileName = string.Empty;

        private string localFileName = string.Empty;
        private FileStream localFileStream = null;
        private byte[] downloadedBitmap = null;
        private string downloadedBitmapFileName = string.Empty;
        private FileStream downloadedBitmapFileStream = null;

        /// <summary>
        /// the queue of waiting download block request,for multiple threads reading the same file, it will queue the download request blocks to queue.
        /// </summary>
        private Queue<long> downloadRequestQueue = new Queue<long>();
        
        private string lastError = string.Empty;
        private bool supportResume = true;

        private string userName = string.Empty;
        private string processName = string.Empty;

        //this indicates if the task object can be dispose.
        private bool isTaskCompleted = false;

        //the flag shows if the content was encrypted 
        private bool enableEncryption = false;
        private string encryptionPassPhrase = string.Empty;
        private string encryptionIVBase64Str = string.Empty;
        private string md5HashBase64String = string.Empty;

        /// <summary>
        /// the directory list
        /// </summary>
        public DirectoryList directoryList = null;

        #region Public Handler
        public event EventHandler<TaskStatusChangedEventArgs> TaskChangedEvent = null;
        #endregion
        
        public AsyncTask()
        {
        }

        public AsyncTask(   TaskType _taskType,
                            SiteInfo _siteInfo, 
                            string _localFileName, 
                            string _remoteFileName, 
                            long _fileSize, 
                            DateTime _creationTime, 
                            FileAttributes _fileAttributes,
                            long _startOffset,
                            string _userName,
                            string _processName)
        {
            this.taskType = _taskType;
            this.siteInfo = _siteInfo;
            this.localFileName = _localFileName;
            this.remoteFileName = _remoteFileName;
            this.fileSize = _fileSize;
            this.creationTime = _creationTime;
            this.fileAttributes = _fileAttributes;
            this.startOffset = _startOffset;
            this.userName = _userName;
            this.processName = _processName;
            this.nextBlockOffset = _startOffset;

            if (IsDownloadRequest)
            {
                LoadDownloadedBitmap();
            }

        }

        ~AsyncTask()
        {
          Dispose(false);
        }

         public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

         private void Dispose(bool disposing)
         {
             try
             {
                 if (!this.disposed)
                 {
                 }

                 if (null != localFileStream)
                 {
                     localFileStream.Close();
                     localFileStream = null;
                 }

                 if (downloadedBitmapFileStream != null)
                 {
                     downloadedBitmapFileStream.Close();
                     downloadedBitmapFileStream = null;
                 }

                 if (State == TaskState.error
                     && (taskType == TaskType.DownloadBlock || taskType == TaskType.DownloadFile)
                     && fileSize < blockSize)
                 {
                     File.Delete(localFileName);
                 }
             }
             catch 
             {
             }

             disposed = true;
         }


         public class TaskStatusChangedEventArgs : EventArgs
         {
             #region Private Fields
             private AsyncTask asyncTask = null;
             private string message = string.Empty;
             #endregion

             #region Public Constructor
             public TaskStatusChangedEventArgs(AsyncTask asyncTask)
             {
                 this.asyncTask = asyncTask;
             }

             public TaskStatusChangedEventArgs(AsyncTask asyncTask,string message)
             {
                 this.asyncTask = asyncTask;
                 this.message = message;
             }

             #endregion

             #region Public properties

             public AsyncTask Task
             {
                 get
                 {
                     return this.asyncTask;
                 }
             }

             public string Message
             {
                 get
                 {
                     return this.message;
                 }
             }

             #endregion
         }


         protected virtual void OnTaskChanged(TaskStatusChangedEventArgs e)
         {
             try
             {
                 if (TaskChangedEvent != null)
                     TaskChangedEvent(this, e);
             }
             catch (Exception ex)
             {
                 EventManager.WriteMessage(254,"OnTaskStatusChanged", EventLevel.Error,"OnTaskStatusChanged exception:" + ex.Message);
             }
         }

         public void UpdateTaskStatusMessage(string message)
         {
             if (TaskChangedEvent != null)
             {
                 TaskStatusChangedEventArgs e = new TaskStatusChangedEventArgs(this, message);
                 TaskChangedEvent(this, e);
             }
         }

      
        public FileAttributes FileAttributes
        {
            get { return fileAttributes; }
            set { fileAttributes = value; }
        }

        /// <summary>
        /// the file creation time
        /// </summary>
        public DateTime CreationTime
        {
            get { return creationTime; }
            set { creationTime = value; }
        }

        /// <summary>
        /// For block download request, it will set this last access time when the new request coming.
        /// if there are no access( no new download request) since the 
        /// </summary>
        public DateTime LastAccessTime
        {
            get { return lastAccessTime; }
            set { lastAccessTime = value; }
        }

        /// <summary>
        /// the file size of the file
        /// </summary>
        public long FileSize
        {
            get
            {
                if (-1 == fileSize)
                {
                    fileSize = new FileInfo(localFileName).Length;
                }

                return fileSize;
            }
            set
            {
                fileSize = value;
            }
        }

        public string LocalFileName
        {
            get { return localFileName; }
            set { localFileName = value;}
        }

        public string RemoteFileName
        {
            get { return remoteFileName; }
            set { remoteFileName = value; }
        }

        /// <summary>
        /// For rename, this is the new file name in remote server
        /// </summary>
        public string RemoteDestFileName
        {
            get { return remoteDestFileName; }
            set { remoteDestFileName = value; }
        }

        /// <summary>
        /// the cache file stream of the local file
        /// </summary>
        public FileStream LocalFileStream
        {
            get 
            {
                if (null == localFileStream)
                {
                    OpenLocalFile();
                }

                return localFileStream; 
            }
        }


        public void CloseLocalStream()
        {
            lock (this)
            {
                if (null != localFileStream)
                {
                    localFileStream.Close();
                    localFileStream = null;
                }
            }

        }

        /// <summary>
        /// the bitmap of the downloaded blocks, it shows which block of the file was downloaded to the local cache file
        /// </summary>
        private void LoadDownloadedBitmap()
        {
            try
            {
                if (null == localFileStream)
                {
                    OpenLocalFile();
                }

                //if the local file exist, it will initialize the download bitmap
                if (null == downloadedBitmap)
                {
                    int bytes = (int)Math.Ceiling(Math.Ceiling((double)(fileSize) / BlockSize) / 8);
                    downloadedBitmap = new byte[bytes];
                }
            }
            catch (Exception ex)
            {
                lastError = "LoadDownloadedBitmap failed with error:" + ex.Message;
                State = TaskState.error;
                EventManager.WriteMessage(294, "LoadDownloadedBitmap", EventLevel.Error, lastError);
               
                    
            }

        }

        /// <summary>
        /// the bitmap indicates the local file download blocks
        /// </summary>
        public byte[] DownloadedBitmap
        {
            get
            {
                if (null == downloadedBitmap)
                {
                    LoadDownloadedBitmap();
                }

                return downloadedBitmap; 
            }
           // set { downloadedBitmap = value; }
        }

        /// <summary>
        /// if support resume download or upload
        /// </summary>
        public bool SupportResume
        {
            get { return supportResume; }
            set { supportResume = value; }
        }

        public SiteInfo SiteInfo
        {
            get { return siteInfo; }
            set { siteInfo = value; }
        }

        /// <summary>
        /// the time of the task was sent to display the status.
        /// </summary>
        public DateTime DisplayTime
        {
            get { return taskDisplayTime; }
            set { taskDisplayTime = value; }
        }      

        /// <summary>
        /// async task start time
        /// </summary>
        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        /// <summary>
        /// async task completed time
        /// </summary>
        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

          public bool IsBlockDownType
        {
            get { return taskType == TaskType.DownloadBlock; }
        }

        public TaskState State
        {
            get { return taskState; }
            set 
            {
                taskState = value;
            }
        }

        //the number of bytes of the cached file was downloaded
        public long DownloadedSize
        {
            get { return downloadedSize; }
            set
            {
                downloadedSize = value;
                if (downloadedSize > fileSize)
                {
                    downloadedSize = fileSize;
                }           

            }
        }

        public long TransferredSize
        {
            get { return transferredSize; }
            set
            {
                transferredSize = value;
                if (transferredSize > fileSize)
                {
                    transferredSize = fileSize;
                }

                endTime = DateTime.Now;

            }
        }

        public AutoResetEvent SyncEvent
        {
            get { return autoResetEvent; }
        }


        public long StartOffset
        {
            get { return startOffset; }
            set 
            {
                startOffset = value;
                nextBlockOffset = startOffset;
            }
        }

        /// <summary>
        /// it shows it needs to encrypt/decrypt the content.
        /// </summary>
        public bool EnableEncryption
        {
            get { return enableEncryption; }
            set { enableEncryption = value; }
        }

        public string EncryptionPassPhrase
        {
            get { return encryptionPassPhrase; }
            set
            {
                encryptionPassPhrase = value;
            }
        }

        public string EncryptionIVBase64Str
        {
            get { return encryptionIVBase64Str; }
            set
            {
                encryptionIVBase64Str = value;
            }
        }

        public string Md5HashBase64String
        {
            get { return md5HashBase64String; }
            set { md5HashBase64String = value; }
        }


        /// <summary>
        /// for download, all block size will be the same, this is the block size per thread can download
        /// for upload, for azure this is the block size of the siteinfo, 
        /// for amazon s3, this is the part size.
        /// </summary>
        public long BlockSize
        {
            get
            {
                if (taskType == TaskType.UploadFile)
                {
                    if (siteInfo.CloudProvider == CloudProviderType.AzureStorage)
                    {
                        blockSize = siteInfo.AzureMaxBlockSize;
                    }
                    else if (siteInfo.CloudProvider == CloudProviderType.Amazon_S3)
                    {
                        blockSize = siteInfo.S3MaxPartSize;
                    }

                }

                return blockSize;
            }
          
        }

        public long NextBlockOffset
        {
            get { return nextBlockOffset; }
            set { nextBlockOffset = value; }
        }


        public bool IsDownloadRequest
        {
            get
            {
                if (taskType == TaskType.DownloadBlock || taskType == TaskType.DownloadFile)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void AddRequestDownloadBlock(long offset)
        {
            lock (downloadRequestQueue)
            {
                downloadRequestQueue.Enqueue(offset);
            }
        }

        public Boolean GetAndSetNextBlockOffset(out long offset,out string getOffsetError)
        {
            offset = 0;
            getOffsetError = string.Empty;

            try
            {
                if (State != TaskState.running)
                {
                    getOffsetError = "Current task state " + State + " is not running." + this.LastError;
                    return false;
                }

            
                lock (this)
                {
                    offset = nextBlockOffset;
                    nextBlockOffset += BlockSize;
                }

                if (nextBlockOffset > FileSize)
                {
                    nextBlockOffset = FileSize;
                }

                if (    offset >= FileSize )                    
                {
                    getOffsetError = "No next offset,offset: " + offset + ",DownloadedSize size:" + DownloadedSize + ",TransferredSize:" + transferredSize + ",file size:" + fileSize + ",fileName "  + localFileName ;

                    EventManager.WriteMessage(580, "GetAndSetNextBlockOffset", EventLevel.Trace, getOffsetError);

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                getOffsetError = "GetAndSetNextBlockOffset failed with error:" + ex.Message;

                lastError = getOffsetError;
                State = TaskState.error;

                EventManager.WriteMessage(615, "GetAndSetNextBlockOffset", EventLevel.Error, getOffsetError);

                return false;
            }

        }

     
        public string TaskId
        {
            get { return taskId; }
            set { taskId = value; }
        }

        public TaskType TaskType
        {
            get { return taskType; }
            set { taskType = value; }
        }      
        /// <summary>
        /// upload/download transfer speed in kb/s.
        /// </summary>
        public double TransferRate
        {
            get
            {
                if (endTime > startTime)
                {
                    return Math.Truncate((transferredSize / endTime.Subtract(startTime).TotalSeconds / 1024)*100)/100;
                }
                else
                {
                    if (DateTime.Now.Subtract(startTime).TotalSeconds > 0)
                    {
                        return Math.Truncate((transferredSize / DateTime.Now.Subtract(startTime).TotalSeconds/1024)*100)/100;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        public int Percent
        {
            get
            {
                if (fileSize > 0 && State != TaskState.completed)
                {
                    return (int)((double)TransferredSize / (double)(fileSize - startOffset) * 100);
                }
                else
                {
                    return 100;
                }
            }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public string ProcessName
        {
            get { return processName; }
            set { processName = value; }
        }

        public string LastError
        {
            get { return lastError; }
            set { lastError = value; }
        }

        public string Info
        {
            get
            {
                string message = "";

                return message;
            }
        }

        /// <summary>
        /// For download request, write data to the local file,make sure write with block site or it is the last block of the file.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool WriteToLocalStream(Stream stream, long offset, byte[] buffer, int length, ref string message )
        {
            bool retVal = true;
            
            try
            {
                if (length == 0)
                {
                    return true;
                }


                if (null == stream)
                {
                    throw new Exception("the local file stream is null.");
                }

                lock (this)
                {
                    stream.Position = offset;
                    stream.Write(buffer, 0, length);

                    DownloadedSize += length;

                }

            }
            catch (Exception ex)
            {
                lastError = string.Format("Write offset {0} length {1} to local file {2} failed.{3}", offset, length, localFileName, ex.Message);
                retVal = false;
                State = TaskState.error;
            }

            SyncEvent.Set();

            return retVal;
        }

        public bool IsTaskCompleted
        {
            get { return isTaskCompleted; }
        }

        /// <summary>
        /// set the async task completed state, all the caller to use task object, it has to call this 
        /// </summary>
        /// <param name="lastError">if the lastError is not empty,async completed with error</param>
        public void CompleteTask(string lastErrorMessage)
        {

            lock (this)
            {
                try
                {
                    if (isTaskCompleted)
                    {
                        //task already completed.
                        return;
                    }

                    if (localFileStream != null)
                    {
                        localFileStream.Flush();
                        localFileStream.Close();
                        localFileStream = null;
                    }

                    if (State != TaskState.error)
                    {
                        if (lastErrorMessage.Trim().Length > 0)
                        {
                            lastError = lastErrorMessage;
                            State = TaskState.error;

                        }
                        else if (State == TaskState.running)
                        {
                            State = TaskState.completed;
                        }
                    }

                    if (IsDownloadRequest && State == TaskState.error)
                    {
                        try
                        {
                            File.Delete(localFileName);
                        }
                        catch (Exception ex)
                        {
                            lastError += ",delete cache file " + localFileName + " failed:" + ex.Message;
                        }

                        EventManager.WriteMessage(1027, "CompleteTask", EventLevel.Error, "Download " + remoteFileName + " failed with error:" + lastError + ", delete the cache file:" + localFileName);

                    }

                    isTaskCompleted = true;
                    endTime = DateTime.Now;

                    EventManager.WriteMessage(1015, "CompleteTask", EventLevel.Verbose, "Task " + taskId + ",state:" + State + ",local file:" + localFileName + ",remote file:" + remoteFileName);
                }
                catch (Exception ex)
                {
                    lastError = "Complete task failed with error:" + ex.Message;
                    State = TaskState.error;
                    EventManager.WriteMessage(853, "CompleteTask", EventLevel.Error, lastError);

                }
            }
        }

        private void OpenLocalFile()
        {

            try
            {
                lock (this)
                {
                    if (localFileStream != null)
                    {
                        //the local file was opened.
                        return;
                    }

                    if (!IsDownloadRequest)
                    {
                        if (File.Exists(localFileName))
                        {
                            localFileStream = File.Open(localFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                        }
                        else
                        {
                            throw new Exception("Can't upload file " + localFileName + ",it doesn't exist.");
                        }

                    }
                    else   //this is download request
                    {
                        if (File.Exists(localFileName))
                        {
                            localFileStream = File.Open(localFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                            return;
                        }

                        localFileStream = FilterAPI.CreateSparseFile(localFileName, FileSize, creationTime, (uint)fileAttributes, true);
                    }

                }
            }
            catch (Exception ex)
            {
                lastError = "Open local file " + localFileName + " failed with error:" + ex.Message;
                State = TaskState.error;
                throw new Exception(lastError);
            }
        }
        
    }
}

