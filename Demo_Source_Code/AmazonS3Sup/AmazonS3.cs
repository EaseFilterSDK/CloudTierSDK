
///////////////////////////////////////////////////////////////////////////////
//
//    (C) Copyright 2011 EaseFilter
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
using System.Diagnostics;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

using CloudTier.CommonObjects;



//life cycle management
//http://docs.aws.amazon.com/IAmazonS3/latest/dev/manage-lifecycle-using-dot-net.html

//bjectMetadata metadata = amazonS3Client.getObjectMetadata(bucketName, fileKey);
//ObjectMetadata metadataCopy = new ObjectMetadata();
//metadataCopy.addUserMetadata("yourKey", "updateValue");
//metadataCopy.addUserMetadata("otherKey", "newValue");
//metadataCopy.addUserMetadata("existingKey", metadata.getUserMetaDataOf("existingValue"));

//CopyObjectRequest request = new CopyObjectRequest(bucketName, fileKey, bucketName, fileKey)
//      .withSourceBucketName(bucketName)
//      .withSourceKey(fileKey)
//      .withNewObjectMetadata(metadataCopy);

//amazonS3Client.copyObject(request);


namespace CloudTier.AmazonS3Sup
{
    public class AmazonS3
    {

        IAmazonS3 client = null;
        int UPLOAD_TIMEOUT = 60000;

        AmazonS3SiteInfo siteInfo = new AmazonS3SiteInfo();

        public AsyncTask asyncTask = new AsyncTask();

        public delegate void UpdataStatusDlgt(string message, bool isErrorMessage);
        protected UpdataStatusDlgt updataStatusDlgt = null;

        /// <summary>
        /// store the transferred size of every part (the thread).
        /// </summary>
        private Dictionary<int, long> transferredSizeOfThePart = new Dictionary<int, long>();

        public AmazonS3(AmazonS3SiteInfo siteInfo)
        {
            InitAmazonS3(siteInfo);
        }

        public AmazonS3(AmazonS3SiteInfo siteInfo, UpdataStatusDlgt _updataStatusDlgt)
        {
            updataStatusDlgt = _updataStatusDlgt;
            InitAmazonS3(siteInfo);
        }

        public AmazonS3(AmazonS3SiteInfo siteInfo, UpdataStatusDlgt _updataStatusDlgt, AsyncTask _asyncTask)
        {
            asyncTask = _asyncTask;
            updataStatusDlgt = _updataStatusDlgt;
            InitAmazonS3(siteInfo);
        }

        private void InitAmazonS3(AmazonS3SiteInfo siteInfo)
        {
            try
            {
                this.siteInfo = siteInfo;

                ServicePointManager.DefaultConnectionLimit = 64;

                string accessKey = siteInfo.AccessKeyId;
                string secretKey = siteInfo.SecretAccessKey;

                Amazon.RegionEndpoint region = Amazon.RegionEndpoint.APNortheast1;

                foreach (Amazon.RegionEndpoint siteRegion in Amazon.RegionEndpoint.EnumerableAllRegions)
                {
                    if (siteInfo.RegionName.Equals(siteRegion.DisplayName))
                    {
                        region = siteRegion;
                        break;
                    }
                }

                Amazon.S3.AmazonS3Config amazonS3Config = new Amazon.S3.AmazonS3Config();

                client = new AmazonS3Client(accessKey, secretKey, region);

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(84, "S3Connection", EventLevel.Error, "Connecting to " + siteInfo.SiteName + " failed with error:" + ex.Message);
                throw ex;
            }

        }


        ~AmazonS3()
        {
        }

        public bool TestConnection(ref string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                S3Utils.ListingBuckets(client);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return false;
        }

        private string GetBucketName(string remotePath)
        {
            string[] paths = remotePath.Split(new char[] { '/' });

            if (paths.Length < 1)
            {
                EventManager.WriteMessage(62, "Parse remote path", EventLevel.Error, "Bucket name can't be empty");
                return "";
            }

            return paths[0];
        }


        public  async Task<bool> IsDirectoryExistAsync(string remoteDirName)
        {
            bool ret = true;
            try
            {
                string bucketName = GetBucketName(remoteDirName);

                if (bucketName.Length < 1)
                {
                    return false;
                }

                ListObjectsRequest requestObject = new ListObjectsRequest();
                requestObject.BucketName = bucketName;

                await client.ListObjectsAsync(requestObject);

                if (remoteDirName.Length > bucketName.Length)
                {
                    remoteDirName = remoteDirName.Substring(bucketName.Length + 1);

                    GetObjectMetadataRequest request = new GetObjectMetadataRequest();
                    request.BucketName = bucketName;
                    request.Key = remoteDirName;

                    GetObjectMetadataResponse response = client.GetObjectMetadata(request);
                    return true;
                }
                else
                {
                    return true;
                }

                
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(126, "S3 isDirectoryExist", EventLevel.Error, "Check directory " + remoteDirName + " exist failed with error:" + ex.Message);
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// create a new container if it doesn't exist
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public  async Task<bool> MakeDirAsync()
        {
            bool ret = true;
            string lastError = string.Empty;

            string remoteDirName = asyncTask.RemoteFileName;

            try
            {

                string bucketName = GetBucketName(remoteDirName);

                if (bucketName.Length < 1)
                {
                    return false;
                }

                if (remoteDirName.Equals(bucketName))
                {
                    PutBucketRequest request = new PutBucketRequest();
                    request.BucketName = bucketName;
                    client.PutBucket(request);

                    EventManager.WriteMessage(166, "S3 MakeDir", EventLevel.Information, "Create bucket:" + bucketName + " succeeded.");
                }
                else
                {                  

                    remoteDirName = remoteDirName.Substring(bucketName.Length + 1) + "/";
                    PutObjectResponse response = await client.PutObjectAsync(new PutObjectRequest()
                    {
                        BucketName = bucketName,
                        ContentBody = "",
                        ContentType = "application/octet-stream",
                        Key = remoteDirName
                    });

                    EventManager.WriteMessage(180, "S3 MakeDir", EventLevel.Information, "Create new directory:" + remoteDirName + " succeeded.");
                }
            }
            catch (Exception ex)
            {
                lastError = "Create " + remoteDirName + " failed with error:" + ex.Message;
                EventManager.WriteMessage(202, "MakeDir", EventLevel.Error, lastError);                

                if(null != updataStatusDlgt)
                {
                    updataStatusDlgt(lastError, true);
                }

                ret = false;
            }

            return ret;
        }


        public  async Task<bool> DeleteFileAsync()
        {
            bool ret = true;
            string lastError = string.Empty;
            string remoteFileName = asyncTask.RemoteFileName;

            try
            {
                string bucketName = GetBucketName(remoteFileName);

                if (bucketName.Length < 1)
                {
                    return false;
                }

                if (remoteFileName.Equals(bucketName))
                {
                    client.DeleteBucket(new Amazon.S3.Model.DeleteBucketRequest()
                    {
                        BucketName = bucketName,
                    });
                }
                else
                {
                    if (remoteFileName.StartsWith("/"))
                    {
                        remoteFileName = remoteFileName.Substring(1);
                    }

                    remoteFileName = remoteFileName.Substring(bucketName.Length + 1);

                    await client.DeleteObjectAsync(new Amazon.S3.Model.DeleteObjectRequest()
                    {
                        BucketName = bucketName,
                        Key = remoteFileName
                    });
                }

            }
            catch (Exception ex)
            {
                ret = false;
                lastError = "Delete file " + remoteFileName + " failed with error:" + ex.Message;
                EventManager.WriteMessage(278, "DeleteFile", EventLevel.Error, lastError);

                if (null != updataStatusDlgt)
                {
                    updataStatusDlgt(lastError, true);
                }

            }

            return ret;
        }

        public async Task<bool> RenameFileAsync()
        {
            bool ret = true;
            string lastError = string.Empty;
            string remoteSourceFileName = asyncTask.RemoteFileName;
            string remoteDestFileName = asyncTask.RemoteDestFileName;

            try
            {
                string sourceBucketName = GetBucketName(remoteSourceFileName);
                string destBucketName = GetBucketName(remoteDestFileName);

                if (sourceBucketName.Length < 1 || destBucketName.Length < 1)
                {
                    EventManager.WriteMessage(293, "RenameFile", EventLevel.Error, string.Format("Invalid name.sourceBucketName {0},destBucketName {1}", sourceBucketName, destBucketName));
                    return false;
                }


                if (remoteSourceFileName.StartsWith("/"))
                {
                    remoteSourceFileName = remoteSourceFileName.Substring(1);
                }

                remoteSourceFileName = remoteSourceFileName.Substring(sourceBucketName.Length + 1);

                if (remoteDestFileName.StartsWith("/"))
                {
                    remoteDestFileName = remoteDestFileName.Substring(1);
                }

                remoteDestFileName = remoteDestFileName.Substring(destBucketName.Length + 1);


                CopyObjectRequest copyRequest = new CopyObjectRequest();
                copyRequest.SourceBucket = sourceBucketName;
                copyRequest.SourceKey = remoteSourceFileName;
                copyRequest.DestinationBucket = destBucketName;
                copyRequest.DestinationKey = remoteDestFileName;
                //copyRequest.CannedACL = S3CannedACL.PublicRead;

                await client.CopyObjectAsync(copyRequest);

                //Delete the original
                DeleteObjectRequest deleteRequest = new DeleteObjectRequest();
                deleteRequest.BucketName = sourceBucketName;
                deleteRequest.Key = remoteSourceFileName;

                await client.DeleteObjectAsync(deleteRequest);

                string message2 = "Rename " + remoteSourceFileName + " to " + remoteDestFileName + " completed.";
                EventManager.WriteMessage(349, "Rename", EventLevel.Information, message2);
            }
            catch (Exception ex)
            {
                ret = false;
                lastError = "Rename file " + remoteSourceFileName + " failed with error:" + ex.Message;
                EventManager.WriteMessage(387, "S3RenameFile", EventLevel.Error, lastError);

                if (null != updataStatusDlgt)
                {
                    updataStatusDlgt(lastError, true);
                }
            }

            return ret;
        }

        protected async Task UploadSingleFileAsync(string bucketName,string keyName,ProgressStream progressStream)
        {
            try
            {
                PutObjectRequest req = new PutObjectRequest();
                req.BucketName = bucketName;
                req.Key = keyName;
                req.Timeout = TimeSpan.FromMilliseconds(UPLOAD_TIMEOUT);
                req.AutoCloseStream = false;

                progressStream.Seek(0, SeekOrigin.Begin);
                req.InputStream = progressStream;

                PutObjectResponse response = await client.PutObjectAsync(req);

                string message = string.Empty;              

            }
            catch (Exception ex)
            {
                string lastError = "UploadSmallFile " + asyncTask.LocalFileName + " failed:" + ex.Message;

                if (null != updataStatusDlgt)
                {
                    updataStatusDlgt(lastError, true);
                }

                throw new Exception(lastError);
            }

            EventManager.WriteMessage(418, "S3 UploadSmallFile", EventLevel.Verbose, "UploadSmallFile:" + asyncTask.RemoteFileName + " succeeded.");
        }

        public static async Task<List<MultipartUpload>> ListMultipartUploadsAsync(string BucketName, IAmazonS3 Client, string Prefix = "")
        {
            ListMultipartUploadsRequest listMultipartRequest = new ListMultipartUploadsRequest();
            listMultipartRequest.BucketName = BucketName;
            listMultipartRequest.Prefix = Prefix;

            ListMultipartUploadsResponse response = await Client.ListMultipartUploadsAsync(listMultipartRequest);

            return response.MultipartUploads;
        }

        public static void AbortMultipartUpload(string BucketName, IAmazonS3 Client, string Key, string UploadId)
        {
            AbortMultipartUploadRequest req = new AbortMultipartUploadRequest();

            req.BucketName = BucketName;
            req.Key = Key;
            req.UploadId = UploadId;

            Client.AbortMultipartUpload(req);
        }

        private void pstream_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lock (asyncTask)
            {
                asyncTask.TransferredSize += e.BytesChanged;
                asyncTask.EndTime = DateTime.Now;
            }

            string message = string.Empty;
            if (asyncTask.TaskType == TaskType.UploadFile)
            {
                if (asyncTask.FileSize - asyncTask.TransferredSize > 0)
                {
                    message = "S3 uploading file " + asyncTask.RemoteFileName + "(uploaded:" + asyncTask.TransferredSize
                        + "/remain:" + (asyncTask.FileSize - asyncTask.TransferredSize) + ",speed:" + asyncTask.TransferRate + "kb/s)";
                }
                else
                {
                    message = "S3 uploaded file " + asyncTask.RemoteFileName + " completed.(FileSize:" + asyncTask.TransferredSize + ",speed:" + asyncTask.TransferRate + "kb/s)";
                }
            }
            else if (asyncTask.TaskType == TaskType.DownloadFile )
            {
                if (asyncTask.FileSize - asyncTask.TransferredSize > 0)
                {
                    message = "S3 downloading file " + asyncTask.RemoteFileName + "(downloaded:" + asyncTask.TransferredSize
                        + "/remain:" + (asyncTask.FileSize - asyncTask.TransferredSize) + ",speed:" + asyncTask.TransferRate + "kb/s)";
                }
                else
                {
                    message = "S3 downloaded file " + asyncTask.RemoteFileName + " completed.(FileSize:" + asyncTask.TransferredSize + ",speed:" + asyncTask.TransferRate + "kb/s)";
                }
            }

            if (message.Length > 0 && null != updataStatusDlgt)
            {
                updataStatusDlgt(message, false);
            }

        }

        async Task ParallelUploadFileAsync(InitiateMultipartUploadResponse initResponse, string bucketName, string keyName, List<PartETag> partETags)
        {
            string lastError = string.Empty;
            FileStream localStream = null;

            try
            {

                long partSize = asyncTask.S3UploadBlockSize;
                long offset = 0;

                localStream = new FileStream(asyncTask.LocalFileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                ProgressStream progressStream = new ProgressStream(localStream);
                progressStream.ProgressChanged += pstream_ProgressChanged;

                while (asyncTask.NextBlockOffset < asyncTask.FileSize && asyncTask.State == TaskState.running)
                {
                    if (!asyncTask.GetAndSetNextBlockOffset(out offset, out lastError))
                    {
                        EventManager.WriteMessage(341, "S3ParallelUploadFile", EventLevel.Verbose, "Get next block offset for file " + asyncTask.LocalFileName + " return false." + lastError);
                        return;
                    }

                    if (!GlobalConfig.IsRunning)
                    {
                        asyncTask.State = TaskState.cancel;
                        break;
                    }

                    int partNumber = (int)(offset / partSize) + 1;

                    // Create request to upload a part.
                    UploadPartRequest uploadRequest = new UploadPartRequest();

                    if (localStream.Length - offset < partSize)
                    {
                        partSize = localStream.Length - offset;
                    }


                    uploadRequest.BucketName = bucketName;
                    uploadRequest.Key = keyName;
                    uploadRequest.UploadId = initResponse.UploadId;
                    uploadRequest.PartNumber = partNumber;
                    uploadRequest.PartSize = partSize;
                    uploadRequest.FilePosition = offset;
                    progressStream.Position = offset;
                    uploadRequest.InputStream = progressStream;

                    // uploadRequest.StreamTransferProgress = new EventHandler<StreamTransferProgressArgs>(this.UploadPartRequestProgressCallback);

                    EventManager.WriteMessage(515, "S3ParallelUploadFile", EventLevel.Trace, "uploading block offset:" + offset + ",partNumber:" + partNumber + ",partSize:" + partSize + " for file " + asyncTask.LocalFileName);

                    // Upload part and add response to our list.
                     UploadPartResponse response = await client.UploadPartAsync(uploadRequest);

                    string message = string.Empty;

                    if (asyncTask.FileSize - asyncTask.TransferredSize > 0)
                    {
                        message = "S3 uploading file " + asyncTask.RemoteFileName + "(uploaded:" + asyncTask.TransferredSize + "/remain:"
                            + (asyncTask.FileSize - asyncTask.TransferredSize) + ",speed:" + asyncTask.TransferRate + "kb/s)";
                    }
                    else
                    {
                        message = "S3 uploaded file " + asyncTask.RemoteFileName + " completed.(FileSize:" + asyncTask.TransferredSize+ ",speed:" + asyncTask.TransferRate + "kb/s)";
                    }

                    if (null != updataStatusDlgt)
                    {
                        updataStatusDlgt(message, false);
                    }

                    partETags.Add(new PartETag(partNumber, response.ETag));


                }
            }
            catch (Exception ex)
            {
                throw new Exception("S3ParrelUpload failed:" + ex.Message);
            }
            finally
            {
                if (null != localStream)
                {
                    localStream.Close();
                }
            }

        }

        public async Task<bool> UploadAsync()
        {
            bool ret = true;
            string lastError = string.Empty;
            FileStream localStream = new FileStream(asyncTask.LocalFileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            try
            {
                string destFileName = asyncTask.RemoteFileName;
                string bucketName = GetBucketName(destFileName);

                if (bucketName.Length < 1)
                {
                    EventManager.WriteMessage(178, "UploadFiles", EventLevel.Error, "Remote file name " + destFileName + " is invalid.");
                    return false;
                }

                string keyName = destFileName.Substring(bucketName.Length + 1);

                long partSize = siteInfo.PartSize;
                uint numberOfTasks = siteInfo.ParallelTasks;

                List<Thread> threads = new List<Thread>();

                if (!siteInfo.EnabledMultiBlocksUpload)
                {
                    ProgressStream progressStream = new ProgressStream(localStream);
                    progressStream.ProgressChanged += pstream_ProgressChanged;

                    await UploadSingleFileAsync(bucketName, keyName, progressStream);
                }
                else
                {

                    // List to store upload part responses.
                    List<PartETag> partETags = new List<PartETag>();

                    // 1. Initialize.
                    InitiateMultipartUploadRequest initiateRequest = new InitiateMultipartUploadRequest();
                    initiateRequest.BucketName = bucketName;
                    initiateRequest.Key = keyName;

                    InitiateMultipartUploadResponse initResponse = await client.InitiateMultipartUploadAsync(initiateRequest);

                    Task[] tasks = new Task[numberOfTasks];

                    for (int i = 0; i < numberOfTasks; i++)
                    {
                        tasks[i] =  Task.Run(()=> ParallelUploadFileAsync(initResponse, bucketName, keyName, partETags) );
                    }

                    await Task.WhenAll(tasks);

                    // Step 3: complete.
                    CompleteMultipartUploadRequest completeRequest = new CompleteMultipartUploadRequest();
                    completeRequest.BucketName = bucketName;
                    completeRequest.Key = keyName;
                    completeRequest.UploadId = initResponse.UploadId;
                    completeRequest.PartETags = partETags;


                    CompleteMultipartUploadResponse completeUploadResponse = await client.CompleteMultipartUploadAsync(completeRequest);


                }

                string message = "S3 Upload " + asyncTask.LocalFileName + " to " + asyncTask.RemoteFileName;
                EventManager.WriteMessage(557, "S3 Upload", EventLevel.Verbose, message);


            }
            catch (Exception ex)
            {
                lastError = "S3 Upload file " + asyncTask.LocalFileName + " to " + asyncTask.RemoteFileName + " failed with error:" + ex.Message;
                EventManager.WriteMessage(600, "S3upload", EventLevel.Error, lastError);
                ret = false;
            }

            return ret;
        }


        private async Task ParallelDownloadAsync(Stream progressStream,string bucketName, string keyName)
        {
            string lastError = string.Empty;

            try
            {
                byte[] buffer = new byte[asyncTask.DownloadBlockSize];
                int bytesToRead = buffer.Length;
                long offset = 0;

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                while (asyncTask.NextBlockOffset < asyncTask.FileSize )
                {

                    bool ret = asyncTask.GetAndSetNextBlockOffset(out offset, out lastError);
                    if (!ret)
                    {
                        EventManager.WriteMessage(662, "S3ParallelDownload", EventLevel.Verbose, "S3 thread " + Thread.CurrentThread.Name
                            + ",get next block offset for file " + asyncTask.LocalFileName + " return false." + lastError);

                        break;
                    }

                    if (!GlobalConfig.IsRunning)
                    {
                        asyncTask.State = TaskState.cancel;
                        break;
                    }

                    bytesToRead = buffer.Length;
                    if (asyncTask.FileSize - offset < bytesToRead)
                    {
                        bytesToRead = (int)(asyncTask.FileSize - offset);
                    }

                    GetObjectRequest request = new GetObjectRequest()
                    {
                        BucketName = bucketName,
                        ByteRange = new ByteRange(offset, offset + bytesToRead),
                        Key = keyName

                    };

                    using (GetObjectResponse response = await client.GetObjectAsync(request))
                    {
                        using (Stream stream = response.ResponseStream)
                        {
                            int bytesRead = 0;
                            while (bytesToRead > 0)
                            {
                                int read = await stream.ReadAsync(buffer, bytesRead, bytesToRead);
                                bytesRead += read;
                                bytesToRead -= read;

                                if (read == 0)
                                {
                                    EventManager.WriteMessage(580, "S3 paralleldownload", EventLevel.Warning, "S3 download file:" + asyncTask.LocalFileName + " offset: " + offset + " readlength " + buffer.Length
                                   + " returnLength:" + bytesRead + " fileSize:" + asyncTask.FileSize + " totalDownload size:" + asyncTask.TransferredSize);

                                    break;
                                }

                            }

                            string message = string.Empty;

                            if (!asyncTask.WriteToLocalStream(progressStream, offset, buffer, bytesRead, ref message))
                            {
                                EventManager.WriteMessage(584, "S3 paralleldownload", EventLevel.Error, "S3 downloading at offset: " + offset + " length " + bytesRead + ",lastAccessTime:" + asyncTask.LastAccessTime.ToLongTimeString()
                                    + " TransferredSize:" + asyncTask.TransferredSize + ",remain:" + (asyncTask.FileSize - asyncTask.TransferredSize) + " failed with error:" + asyncTask.LastError);
                                break;
                            }

                            if (asyncTask.FileSize - asyncTask.TransferredSize > 0)
                            {
                                message = "S3 downloading file " + asyncTask.RemoteFileName + "(downloaded:" + asyncTask.TransferredSize + "/remain:"
                                     + (asyncTask.FileSize - asyncTask.TransferredSize) + ",speed:" + asyncTask.TransferRate + "kb/s)";
                            }
                            else
                            {
                                message = "S3 download file " + asyncTask.RemoteFileName + " completed.(FileSize:" + asyncTask.TransferredSize + ",speed:" + asyncTask.TransferRate + "kb/s)";
                            }

                            if (null != updataStatusDlgt)
                            {
                                updataStatusDlgt(message, false);
                            }
                        }
                    }

                }

            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception("S3 ParrelDownload AmazonS3Exception error code:" + ex.ErrorCode + ", error:" + ex.Message);
            }
            catch (Exception ex)
            {
               throw new Exception("S3 ParrelDownload failed:" + ex.Message);
            }             
        }

        private async Task DownloadSingleFileAsync(Stream progressStream, string bucketName, string keyName)
        {
            string message = string.Empty;

            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };
                using (GetObjectResponse response = await client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                {
                    byte[] buffer = new byte[asyncTask.DownloadBlockSize];
                    int bytesToRead = buffer.Length;
                    int bytesRead = 0;
                    long offset = 0;

                    do
                    {
                        bytesRead = await responseStream.ReadAsync(buffer, 0, bytesToRead);
                        if (bytesRead > 0)
                        {
                            if (!asyncTask.WriteToLocalStream(progressStream, offset, buffer, bytesRead, ref message))
                            {
                                EventManager.WriteMessage(584, "S3 DownloadSignleFileAsync", EventLevel.Error, "S3 downloading at offset: " + offset + " length " + bytesRead + ",lastAccessTime:" + asyncTask.LastAccessTime.ToLongTimeString()
                                    + " TransferredSize:" + asyncTask.TransferredSize + ",remain:" + (asyncTask.FileSize - asyncTask.TransferredSize) + " failed with error:" + asyncTask.LastError);
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    while (bytesRead > 0);
                }
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception("S3 Download AmazonS3Exception error code:" + ex.ErrorCode + ", error:" + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("S3 Download failed:" + ex.Message);
            }
        }

        public async Task<bool> DownloadAsync()
        {
            bool ret = true;
            string lastError = string.Empty;

            try
            {
                string remoteFileName = asyncTask.RemoteFileName;
                string localFileName = asyncTask.LocalFileName;
                string bucketName = GetBucketName(remoteFileName);

                if (bucketName.Length < 1)
                {
                    ret = false;
                    lastError = "Remote file name " + remoteFileName + " is invalid.";
                    return false;
                }

                if (remoteFileName.Length <= bucketName.Length)
                {
                    ret = false;
                    lastError = "Remote file name " + remoteFileName + " is invalid.";
                    return false;
                }
             
                long fileSize = asyncTask.FileSize;
                remoteFileName = remoteFileName.Substring(bucketName.Length + 1);

                ProgressStream progressStream = new ProgressStream(asyncTask.LocalFileStream);
                progressStream.ProgressChanged += pstream_ProgressChanged;

                if (fileSize == 0)
                {
                    return true;
                }           

                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();

                if (siteInfo.ParallelTasks == 1)
                {
                    await DownloadSingleFileAsync(progressStream, bucketName, remoteFileName);
                }
                else
                {
                    uint numberOfTasks = siteInfo.ParallelTasks;
                    Task[] tasks = new Task[numberOfTasks];

                    for (int i = 0; i < numberOfTasks; i++)
                    {
                        tasks[i] = Task.Run(() => ParallelDownloadAsync(progressStream, bucketName, remoteFileName));
                    }

                    await Task.WhenAll(tasks);

                }

                stopWatch.Stop();

                string ms = "Download completed,Name:" + asyncTask.RemoteFileName + " FileSize:" + fileSize + ",SpentTime(s):" + stopWatch.Elapsed.TotalSeconds + " startOffset:" + asyncTask.StartOffset
                    + " downloaded size:" + asyncTask.DownloadedSize + " transfered size:" + asyncTask.TransferredSize + ",at " + DateTime.Now.ToString("G");
                EventManager.WriteMessage(871, "S3 Download", EventLevel.Trace, ms);

            }
            catch (AmazonS3Exception ex)
            {
                lastError = "S3 Download file " + asyncTask.LocalFileName + " failed with error:" + ex.Message + ",error code:" + ex.ErrorCode;
                EventManager.WriteMessage(910, "S3 Download", EventLevel.Error, lastError);

                if (null != updataStatusDlgt)
                {
                    updataStatusDlgt(lastError, true);
                }

                ret = false;
            }
            catch (Exception ex)
            {
                lastError = "S3 Download file " + asyncTask.LocalFileName + " failed with error:" + ex.Message;
                EventManager.WriteMessage(396, "S3 Download", EventLevel.Error, lastError);

                if (null != updataStatusDlgt)
                {
                    updataStatusDlgt(lastError, true);
                }

                ret = false;
            }

            return ret;
        }



        private string GetDestDirName(AmazonS3SiteInfo siteInfo,string directoryName)
        {
            string destDirName = siteInfo.RemotePath + directoryName.Substring(siteInfo.LocalPath.Length);
            destDirName = destDirName.Replace('\\', '/');

            return destDirName;
        }

        public async Task<DirectoryList> DownloadFileListAsync(string directoryName, bool refresh)
        {
            DirectoryList dirFileList = new DirectoryList(directoryName, siteInfo, refresh);

            try
            {

                EventManager.WriteMessage(101, "DirFileList", EventLevel.Verbose, "Initialize dirFileList for dirName " + directoryName + ",refresh:" + refresh);

                dirFileList = await S3Utils.FileListAsync(client, siteInfo, dirFileList);

            }
            catch (Exception ex)
            {
                string lastError = "S3 get " + directoryName + " directory list failed with error:" + ex.Message;

                if (null != updataStatusDlgt)
                {
                    updataStatusDlgt(lastError, true);
                }

                EventManager.WriteMessage(101, "DirFileList", EventLevel.Error, lastError);
            }

            return dirFileList;
        }

    }
}
