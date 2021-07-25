
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
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Security.Cryptography;

using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.RetryPolicies;

using EaseFilter.GlobalObjects;


namespace EaseFilter.CloudManager
{
    class AzureStorage: CloudProvider
    {

        const long KB = 1024;
        const long MB = 1024 * KB;
        const long GB = 1024 * MB;
        const long MAXBLOCKS = 50000;
        const long MAXBLOBSIZE = 200 * GB;
        const long MAXBLOCKSIZE = 4 * MB;

        AutoResetEvent m_reset = new AutoResetEvent(false);
        CloudBlobClient cloudBlobClient = null;

        public AzureStorage(SiteInfo siteInfo, UpdataStatusDlgt updataStatusDlgt)
        {

            try
            {
                this.siteInfo = siteInfo;
                this.updataStatusDlgt = updataStatusDlgt;

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(siteInfo.AzureConnectionString);

                // Create service client for credentialed access to the Blob service.
                cloudBlobClient = storageAccount.CreateCloudBlobClient();

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(87, "Azure Connection", EventLevel.Error, "Connecting to " + siteInfo.SiteName + " failed with error:" + ex.Message);

                string errorMessage = "Failed to connect Azure with connection string " + siteInfo.AzureConnectionString;
                errorMessage += ",please check if your connection string is correct, if you haven't signed up for Azure, please visit https://portal.azure.com/";

                throw new Exception(errorMessage);
            }

        }


        ~AzureStorage()
        {
        }

        public override bool TestConnection(ref string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                AzureBlob azureBlob = new AzureBlob(cloudBlobClient, siteInfo);
                var containerList = azureBlob.ListContainersAsync(cloudBlobClient);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return false;
        }

        private string GetContainerName(string remotePath)
        {
            string[] paths = remotePath.Split(new char[] { '/' });

            if (paths.Length < 1)
            {
                EventManager.WriteMessage(62, "Parse remote path", EventLevel.Error, "container name can't be empty");
                return "";
            }

            return paths[0];
        }

        public override async Task<bool> IsDirectoryExistAsync(string remoteDirName)
        {
            bool ret = true;
            try
            {
                string containerName = GetContainerName(remoteDirName);

                if (containerName.Length < 1)
                {
                    return false;
                }

                CloudBlobContainer blobContainer = cloudBlobClient.GetContainerReference(containerName);
                await blobContainer.FetchAttributesAsync();

                if (remoteDirName.Length > containerName.Length)
                {
                    remoteDirName = remoteDirName.Substring(containerName.Length + 1);
                    var blob = blobContainer.GetBlockBlobReference(string.Format("{0}/", remoteDirName));

                    await blob.FetchAttributesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(110, "Azure isDirectoryExist", EventLevel.Error, "Check directory " + remoteDirName + " exist failed with error:" + ex.Message);
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// create a new container if it doesn't exist
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public override async Task<bool> MakeDirAsync()
        {
            bool ret = true;
            string lassError = string.Empty;
            string remoteDirName = asyncTask.RemoteFileName;

            try
            {
                string containerName = GetContainerName(remoteDirName);

                if (containerName.Length < 1)
                {
                    return false;
                }

                CloudBlobContainer blobContainer = cloudBlobClient.GetContainerReference(containerName);
                await blobContainer.CreateIfNotExistsAsync();

                if (remoteDirName.Length > containerName.Length)
                {
                    remoteDirName = remoteDirName.Substring(containerName.Length + 1);
                    var blob = blobContainer.GetBlockBlobReference(string.Format("{0}/", remoteDirName));
                    blob.Properties.ContentType = "application/octet-stream";
                    await blob.UploadTextAsync("");

                    EventManager.WriteMessage(205, "MakeDir", EventLevel.Verbose, "Azure create new directory:" + remoteDirName + " succeeded.");

                }
            }
            catch (Exception ex)
            {
                lassError = " Azure create new directory:" + remoteDirName + " failed with error " + ex.Message;
                EventManager.WriteMessage(194, "MakeDir", EventLevel.Error, lassError);
            }

            return ret;
        }

   
        public override async Task<bool> DeleteFileAsync()
        {
            bool ret = true;
            string lastError = string.Empty;
            string remoteFileName = asyncTask.RemoteFileName;

            try
            {
                string containerName = GetContainerName(remoteFileName);

                if (containerName.Length < 1)
                {
                    return false;
                }

                if (remoteFileName.Equals(containerName))
                {
                    CloudBlobContainer blobContainer = cloudBlobClient.GetContainerReference(containerName);
                    await blobContainer.DeleteAsync();
                }
                else
                {
                    CloudBlobContainer blobContainer = cloudBlobClient.GetContainerReference(containerName);
                    if (remoteFileName.Length > containerName.Length)
                    {
                        remoteFileName = remoteFileName.Substring(containerName.Length + 1);
                        var blob = blobContainer.GetBlockBlobReference(string.Format("{0}", remoteFileName));
                        await blob.DeleteIfExistsAsync();
                    }

                }

                EventManager.WriteMessage(205, "DeleteFile", EventLevel.Verbose, "Delete file " + remoteFileName + " succeeded.");

            }
            catch (Exception ex)
            {
                ret = false;
                lastError = "Delete file " + remoteFileName + " failed with error:" + ex.Message;
                EventManager.WriteMessage(218, "DeleteFile", EventLevel.Error, lastError);

            }

            return ret;
        }

        public override async Task<bool> RenameFileAsync()
        {
            bool ret = true;
            string lastError = string.Empty;
            string remoteSourceFileName = asyncTask.RemoteFileName;
            string remoteDestFileName = asyncTask.RemoteDestFileName;

            try
            {
                string sourceContainerName = GetContainerName(remoteSourceFileName);
                string destContainerName = GetContainerName(remoteDestFileName);

                if (sourceContainerName.Length < 1 || destContainerName.Length < 1)
                {
                    EventManager.WriteMessage(241, "RenameFile", EventLevel.Error, string.Format("Invalid name.sourceContainerName {0},destContainerName {1}", sourceContainerName, destContainerName));
                    return false;
                }


                if (remoteSourceFileName.StartsWith("/"))
                {
                    remoteSourceFileName = remoteSourceFileName.Substring(1);
                }

                remoteSourceFileName = remoteSourceFileName.Substring(sourceContainerName.Length + 1);

                if (remoteDestFileName.StartsWith("/"))
                {
                    remoteDestFileName = remoteDestFileName.Substring(1);
                }

                remoteDestFileName = remoteDestFileName.Substring(destContainerName.Length + 1);

                CloudBlobContainer sourceContainer = cloudBlobClient.GetContainerReference(sourceContainerName);
                CloudBlobContainer destontainer = cloudBlobClient.GetContainerReference(destContainerName);

                CloudBlockBlob existBlob = sourceContainer.GetBlockBlobReference(remoteSourceFileName);
                CloudBlockBlob newBlob = destontainer.GetBlockBlobReference(remoteDestFileName);

                await newBlob.StartCopyAsync(existBlob.Uri);

                if (newBlob.CopyState.Status == CopyStatus.Success)
                {
                    await existBlob.DeleteAsync();
                }
                else
                {
                    lastError = "Rename " + remoteSourceFileName + " to " + remoteDestFileName + " failed.";
                }

                string message2 = "Rename " + remoteSourceFileName + " to " + remoteDestFileName + " completed.";
                EventManager.WriteMessage(288, "Rename", EventLevel.Verbose, message2);

            }
            catch (Exception ex)
            {
                ret = false;
                lastError = "Rename file " + remoteSourceFileName + " failed with error:" + ex.Message;
                EventManager.WriteMessage(304, "RenameFile", EventLevel.Error, lastError);

            }

            return ret;
        }


        async Task ParallelUploadFileAsync(ProgressStream localStream, CloudBlockBlob blob)
        {
            byte[] buff = null;
            int readLength = 0;
            long offset = 0;
            string lastError = string.Empty;

            try
            {

                while (asyncTask.State == TaskState.running && asyncTask.NextBlockOffset < asyncTask.FileSize)
                {
                   
                    if (!asyncTask.GetAndSetNextBlockOffset(out offset,out lastError))
                    {
                        EventManager.WriteMessage(341, "AzureParallelUploadFile", EventLevel.Verbose,"Get next block offset for file " + asyncTask.LocalFileName + " return false." + lastError);
                        break;
                    }

                    if (!GlobalConfig.IsRunning)
                    {
                        asyncTask.State = TaskState.cancel;
                        break;
                    }


                    lock (localStream)
                    {
                        if (asyncTask.FileSize - offset >= siteInfo.AzureMaxBlockSize)
                        {
                            buff = new byte[siteInfo.AzureMaxBlockSize];
                        }
                        else
                        {
                            buff = new byte[asyncTask.FileSize - offset];
                        }

                        localStream.Seek(offset, SeekOrigin.Begin);
                        readLength = localStream.Read(buff, 0, buff.Length);
                    }

                    if (readLength == 0)
                    {
                        break;
                    }

                    string blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes((offset/siteInfo.AzureMaxBlockSize).ToString("00000000")));
                    await blob.PutBlockAsync(blockId, new MemoryStream(buff), null);

                    string message = string.Empty;

                    if (asyncTask.FileSize - asyncTask.TransferredSize > 0)
                    {
                        message = "Azure uploading file " + asyncTask.RemoteFileName + "(uploaded:" + asyncTask.TransferredSize 
                            + "/remain:" + (asyncTask.FileSize - asyncTask.TransferredSize) + ",speed:" + asyncTask.TransferRate + "kb/s)";
                    }
                    else
                    {
                        message = "Azure uploaded file " + asyncTask.RemoteFileName + " completed.(FileSize:" + asyncTask.TransferredSize + ",speed:" + asyncTask.TransferRate + "kb/s)";
                    }

                    if (null != updataStatusDlgt)
                    {
                        updataStatusDlgt(message, false);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("AzureParrelUpload " + asyncTask.LocalFileName + " failed:" + ex.Message);
            }
             
       }

        private void pstream_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lock (asyncTask)
            {
                asyncTask.TransferredSize += e.BytesChanged;
                asyncTask.EndTime = DateTime.Now;
            }

            string message = string.Empty;
            if (asyncTask.TaskType == TaskType.UploadFile && !asyncTask.SiteInfo.EnabledMultiBlocksUpload)
            {
                if (asyncTask.FileSize - asyncTask.TransferredSize > 0)
                {
                    message = "Azure uploading file " + asyncTask.RemoteFileName + "(uploaded:" + asyncTask.TransferredSize
                        + "/remain:" + (asyncTask.FileSize - asyncTask.TransferredSize) + ",speed:" + asyncTask.TransferRate + "kb/s)";
                }
                else
                {
                    message = "Azure uploaded file " + asyncTask.RemoteFileName + " completed.(FileSize:" + asyncTask.TransferredSize + ",speed:" + asyncTask.TransferRate + "kb/s)";
                }
            }
            else if (asyncTask.TaskType == TaskType.DownloadFile && !asyncTask.SiteInfo.EnabledMultiBlocksDownload)
            {
                if (asyncTask.FileSize - asyncTask.TransferredSize > 0)
                {
                    message = "Azure downloading file " + asyncTask.RemoteFileName + "(downloaded:" + asyncTask.TransferredSize
                        + "/remain:" + (asyncTask.FileSize - asyncTask.TransferredSize) + ",speed:" + asyncTask.TransferRate + "kb/s)";
                }
                else
                {
                    message = "Azure downloaded file " + asyncTask.RemoteFileName + " completed.(FileSize:" + asyncTask.TransferredSize + ",speed:" + asyncTask.TransferRate + "kb/s)";
                }
            }

            if (message.Length> 0 && null != updataStatusDlgt)
            {
                updataStatusDlgt(message, false);
            }
        }

        public override async Task<bool> UploadAsync()
        {
            bool ret = true;
            FileStream localStream = asyncTask.LocalFileStream;
            string lastError = string.Empty;

            try
            {

                string destFileName = asyncTask.RemoteFileName;
                string containerName = GetContainerName(destFileName);

                if (containerName.Length < 1)
                {
                    ret = false;
                    lastError = "Remote file name " + destFileName + " is invalid.";
                    return false;
                }

                CloudBlobContainer blobContainer = cloudBlobClient.GetContainerReference(containerName);
                blobContainer.CreateIfNotExists();

                destFileName = destFileName.Substring(containerName.Length + 1);
                var blob = blobContainer.GetBlockBlobReference(destFileName);
                blob.Properties.ContentType = MimeType.ConvertExtensionToMimeType(Path.GetExtension(asyncTask.LocalFileName));

                FileInfo fileInfo = new FileInfo(asyncTask.LocalFileName);

                ProgressStream progressStream = new ProgressStream(localStream);
                progressStream.ProgressChanged += pstream_ProgressChanged;

                if (!siteInfo.EnabledMultiBlocksUpload)
                {    
                    await blob.UploadFromStreamAsync(progressStream);
                    EventManager.WriteMessage(532, "UploadFile", EventLevel.Verbose, "Azure UploadFile " + asyncTask.RemoteFileName + " succeeded.");
                }
                else
                {

                    int numberOfTasks = siteInfo.ParallelTasks;
                    Task[] tasks = new Task[numberOfTasks];

                    for (int i = 0; i < numberOfTasks; i++)
                    {
                        tasks[i] = Task.Run(() => ParallelUploadFileAsync(progressStream, blob));
                    }

                    await Task.WhenAll(tasks);

                    var blockIds = new List<string>();
                    long fileSize = fileInfo.Length;

                    long index = 0;
                    while (fileSize > 0)
                    {
                        string blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(index.ToString("00000000"))); //index.ToString("00000000");
                        blockIds.Add(blockId);

                        fileSize -= siteInfo.AzureMaxBlockSize;

                        index++;

                    }

                    // commit the file
                    await blob.PutBlockListAsync(blockIds);

                    EventManager.WriteMessage(485, "UploadFile", EventLevel.Verbose, "Azure parallel upload file " + asyncTask.RemoteFileName + " succeeded.");
                }
              
                //blob.SetMetadata();

            }
            catch (Exception ex)
            {
                ret = false;
                lastError = "Upload file " + asyncTask.LocalFileName + " to " + asyncTask.RemoteFileName + " failed with error:" + ex.Message;
                EventManager.WriteMessage(524, "AZureUpload", EventLevel.Error, lastError);
            }

            return ret;
        }

        private async Task ParallelDownloadAsync(ProgressStream progressStream,CloudBlob m_Blob)
        {
            string lastError = string.Empty;

            try
            {
                long offset = 0;

                while (asyncTask.NextBlockOffset < asyncTask.FileSize && asyncTask.State == TaskState.running)
                {
                    if (!asyncTask.GetAndSetNextBlockOffset(out offset, out lastError))
                    {
                        EventManager.WriteMessage(563, "AzureParallelDownload", EventLevel.Verbose, "Get next block offset for file " + asyncTask.LocalFileName + " return false." + lastError);
                        break;
                    }

                    if (!GlobalConfig.IsRunning)
                    {
                        asyncTask.State = TaskState.cancel;
                        break;
                    }

                    long readLength = asyncTask.BlockSize;
                    if (asyncTask.FileSize - offset < readLength)
                    {
                        readLength = (int)(asyncTask.FileSize - offset);
                    }

                    string message = string.Empty;
                    MemoryStream ms = new MemoryStream();
                    await m_Blob.DownloadRangeToStreamAsync(ms, offset, readLength);

                    byte[] buffer = ms.ToArray();

                    if (!asyncTask.WriteToLocalStream(progressStream, offset, buffer, buffer.Length, ref message))
                    {
                        EventManager.WriteMessage(584, "paralleldownload", EventLevel.Error, "Downloading at blcok offset: " + offset / 65536 + " length " + readLength
                            + " total size:" + asyncTask.TransferredSize + ",remain:" + (asyncTask.FileSize - asyncTask.TransferredSize) + " failed with error:" + asyncTask.LastError);
                    }
                    else
                    {
                        EventManager.WriteMessage(574, "Azure paralleldownload", EventLevel.Trace, "Azure downloading at offset: " + offset + " length " + readLength
                           + " DownloadedSize:" + asyncTask.DownloadedSize + " TransferredSize:" + asyncTask.TransferredSize + ",remain:" + (asyncTask.FileSize - asyncTask.TransferredSize));
                    }


                    if (asyncTask.FileSize - asyncTask.TransferredSize > 0)
                    {
                        message = "Azure downloading file " + asyncTask.RemoteFileName + "(downloaded:" + asyncTask.TransferredSize + "/remain:" 
                            + (asyncTask.FileSize - asyncTask.TransferredSize) + ",speed:" + asyncTask.TransferRate+ "kb/s)";
                    }
                    else
                    {
                        message = "Azure download file " + asyncTask.RemoteFileName + " completed.(FileSize:" + asyncTask.TransferredSize + ",speed:" + asyncTask.TransferRate + "kb/s)";
                    }


                    if (null != updataStatusDlgt)
                    {
                        updataStatusDlgt(message, false);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("AzureParrelDownload "+ asyncTask.LocalFileName + " failed:" + ex.Message);
            }

  
        }

        private async Task DownloadSingleFileAsync(ProgressStream progressStream, CloudBlockBlob cloudBlockBlob)
        {
            try
            {
                await cloudBlockBlob.DownloadToStreamAsync(progressStream);
            }
            catch( Exception ex)
            {
                throw new Exception("AzureDownload " + asyncTask.LocalFileName + " failed:" + ex.Message);
            }
        }


        public override async Task<bool> DownloadAsync()
        {
            bool ret = true;
            string lastError = string.Empty;

            try
            {
                string remoteFileName = asyncTask.RemoteFileName;
                string localFileName = asyncTask.LocalFileName;
                string containerName = GetContainerName(remoteFileName);

                if (containerName.Length < 1)
                {
                    ret = false;
                    lastError = "Remote file name " + remoteFileName + " is invalid.";
                    return false;
                }
             
                CloudBlobContainer blobContainer = cloudBlobClient.GetContainerReference(containerName);

                if (remoteFileName.Length <= containerName.Length)
                {
                    lastError = "Remote file name " + remoteFileName + " is invalid.";
                    ret = false;
                    return false;
                }

                remoteFileName = remoteFileName.Substring(containerName.Length + 1);
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(remoteFileName);

                await blob.FetchAttributesAsync();

                asyncTask.CreationTime = blob.Properties.LastModified.Value.DateTime.ToLocalTime();
                long fileSize = asyncTask.FileSize = blob.Properties.Length;

                string md5Base64String = string.Empty;
                string encryptionIV = string.Empty;

                ProgressStream progressStream = new ProgressStream(asyncTask.LocalFileStream);
                progressStream.ProgressChanged += pstream_ProgressChanged;


                if (fileSize == 0)
                {
                    return true;
                }

                if ( !siteInfo.EnabledMultiBlocksDownload)
                {
                    await DownloadSingleFileAsync(progressStream, blob);
                }
                else
                {

                    int numberOfTasks = siteInfo.ParallelTasks;
                    Task[] tasks = new Task[numberOfTasks];

                    for (int i = 0; i < numberOfTasks; i++)
                    {
                        tasks[i] = Task.Run(() => ParallelDownloadAsync(progressStream, blob));
                    }

                    await Task.WhenAll(tasks);
                }
            }
            catch (Exception ex)
            {
                lastError = "Azure Download file " + asyncTask.LocalFileName + " failed with error:" + ex.Message;
                EventManager.WriteMessage(677, "Azure Download", EventLevel.Error, lastError);
                ret = false;
            }

            return ret;
        }

        private string GetDestDirName(SiteInfo siteInfo,string directoryName)
        {
            string destDirName = siteInfo.RemotePath + directoryName.Substring(siteInfo.LocalPath.Length);
            destDirName = destDirName.Replace('\\', '/');

            return destDirName;
        }

        public override async Task<DirectoryList> DownloadFileListAsync(string directoryName, bool refresh)
        {
            DirectoryList dirFileList = new DirectoryList(directoryName, siteInfo, refresh);

            try
            {

                if (refresh || !dirFileList.LoadFileList())
                {
                    AzureBlob azureBlob = new AzureBlob(cloudBlobClient, siteInfo);
                    string remotePath = CloudUtil.GetRemotePathByLocalPath(directoryName, siteInfo);
                    dirFileList = await azureBlob.FileListAsync(remotePath, dirFileList);
                }

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(101, "DirFileList", EventLevel.Error, "Azure get " + directoryName + " directory list failed with error:" + ex.Message);
            }

            return dirFileList;

        }
   
    }
}
