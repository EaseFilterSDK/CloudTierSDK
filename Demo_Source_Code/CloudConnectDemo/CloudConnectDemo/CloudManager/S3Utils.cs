
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
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

using EaseFilter.GlobalObjects;


namespace EaseFilter.CloudManager
{
    public class S3Utils
    {

        public static async Task<DirectoryList> FileListAsync(IAmazonS3 client, string directory, SiteInfo siteInfo, DirectoryList dirFileList)
        {
            string path = directory;

            if (path.Length != 0 && !path.EndsWith("/"))
            {
                path += "/";
            }

            try
            {
                if (path == string.Empty)
                {
                    ListBucketsResponse response = await client.ListBucketsAsync();
                    foreach (S3Bucket bucket in response.Buckets)
                    {
                        dirFileList.AddFileEntry(bucket.BucketName,
                            null,
                            bucket.CreationDate.ToFileTime(),
                             bucket.CreationDate.ToFileTime(),
                              bucket.CreationDate.ToFileTime(),
                            0,
                            (uint)FileAttributes.Directory);

                        EventManager.WriteMessage(57, "FileList", EventLevel.Verbose, "Get bucket name:" + bucket.BucketName);
                    }

                }
                else
                {
                    ListObjectsRequest request = new ListObjectsRequest();
                    string bucketName = path.Substring(0, path.IndexOf('/'));
                    path = path.Substring(bucketName.Length + 1);
                    request.BucketName = bucketName;
                    request.Prefix = path;
                    request.Delimiter = "/";

                    bool listObjects = true;

                    while (listObjects)
                    {

                        ListObjectsResponse response = await client.ListObjectsAsync(request);
                        foreach (string dir in response.CommonPrefixes)
                        {

                            var dateStamp = DateTime.Now;
                            var directoryName = dir.Substring(path.Length);

                            if (directoryName.EndsWith(@"/"))
                            {
                                directoryName = directoryName.Substring(0, directoryName.Length - 1);
                            }

                            dirFileList.AddFileEntry(directoryName,
                                                        null,
                                                        dateStamp.ToFileTime(),
                                                        dateStamp.ToFileTime(),
                                                        dateStamp.ToFileTime(),
                                                        0,
                                                        (uint)FileAttributes.Directory);

                            EventManager.WriteMessage(97, "FileList", EventLevel.Trace, "Get directory name:" + directoryName);

                        }

                        foreach (S3Object entry in response.S3Objects)
                        {
                            var fileName = entry.Key.Substring(path.Length);


                            if (fileName.Length > 0)
                            {
                                //   ASCIIEncoding.ASCII.GetBytes(entry.ETag)

                                DateTime dateStamp = entry.LastModified;
                                dirFileList.AddFileEntry(fileName,
                                                        null,
                                                         dateStamp.ToFileTime(),
                                                          dateStamp.ToFileTime(),
                                                          dateStamp.ToFileTime(),
                                                          entry.Size,
                                                         (uint)FileAttributes.Offline);

                            }


                        }

                        if (response.IsTruncated)
                        {
                            request.Marker = response.NextMarker;
                        }
                        else
                        {
                            listObjects = false;
                            break;
                        }

                    }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Please check the provided AWS Credentials.");
                }
                else
                {
                   throw new Exception(string.Format("S3 get file list failed. Error code:{0} message:{1} for directory:{2}", amazonS3Exception.ErrorCode, amazonS3Exception.Message,directory));
                }
            }
            catch (Exception ex)
            {
               throw new Exception( "S3 get file list failed with error:" + ex.Message + " for directory:" + directory);
            }


            return dirFileList;
        }


        public static void ListingBuckets(IAmazonS3 client)
        {
            try
            {
                ListBucketsResponse response = client.ListBuckets();
                {
                    foreach (S3Bucket bucket in response.Buckets)
                    {
                        EventManager.WriteMessage(146, "S3 FileList", EventLevel.Verbose,
                        string.Format("You own Bucket with name: {0}", bucket.BucketName));
                    }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    string errorMessage = "Please check the provided AWS Credentials.";
                    errorMessage +="If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3";

                    throw new Exception(errorMessage);
                }
                else
                {
                    throw new Exception(string.Format("An Error, number {0}, occurred when listing buckets with the message '{1}", amazonS3Exception.ErrorCode, amazonS3Exception.Message));
                }
            }
        }

        static void CreateABucket(IAmazonS3 client,string bucketName)
        {
            try
            {
                PutBucketRequest request = new PutBucketRequest();
                request.BucketName = bucketName;
                client.PutBucket(request);

                EventManager.WriteMessage(199, "FileList", EventLevel.Verbose,
                                        string.Format("Create bucket {0} succeeded.",bucketName));
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    EventManager.WriteMessage(199, "FileList", EventLevel.Error,
                                       string.Format("Create bucket {0} failed.Please check the provided AWS Credentials.", bucketName));
                }
                else
                {
                    EventManager.WriteMessage(199, "FileList", EventLevel.Error,
                                     string.Format("Create bucket {0} failed.{1}", bucketName, amazonS3Exception.Message));
                }
            }
        }

        static void WritingAnObject(IAmazonS3 client,string bucketName,string keyName)
        {
            try
            {
                //// simple object put
                //PutObjectRequest request = new PutObjectRequest();
                //request.WithContentBody("this is a test")
                //    .WithBucketName(bucketName)
                //    .WithKey(keyName);

                //S3Response response = client.PutObject(request);
                //response.Dispose();

                //// put a more complex object with some metadata and http headers.
                //PutObjectRequest titledRequest = new PutObjectRequest();
                //titledRequest.WithMetaData("title", "the title")
                //    .WithContentBody("this object has a title")
                //    .WithBucketName(bucketName)
                //    .WithKey(keyName);

                //using (S3Response responseWithMetadata = client.PutObject(titledRequest))
                //{
                //    WebHeaderCollection headers = response.Headers;
                //    foreach (string key in headers.Keys)
                //    {
                //        EventManager.WriteMessage(242, "S3 WritingAnObject", EventLevel.Verbose,
                //        string.Format("Response Header: {0}, Value: {1}", key, headers.Get(key)));
                //    }
                //}
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    EventManager.WriteMessage(253, "S3 WritingAnObject", EventLevel.Error,
                        string.Format("S3 writing an object, bucket name: {0}, keyName: {1} failed.,Please check the provided AWS Credentials.", bucketName, keyName));
                }
                else
                {
                    EventManager.WriteMessage(258, "S3 WritingAnObject", EventLevel.Error,
                       string.Format("S3 writing an object, bucket name: {0}, keyName: {1} failed.{2}.", bucketName, keyName, amazonS3Exception.Message));

                }
            }
        }

        static void ReadingAnObject(IAmazonS3 client, string bucketName, string keyName)
        {
            try
            {
                //GetObjectRequest request = new GetObjectRequest().WithBucketName(bucketName).WithKey(keyName);

                //using (GetObjectResponse response = client.GetObject(request))
                //{
                //    string title = response.Metadata["x-amz-meta-title"];
                //    Console.WriteLine("The object's title is {0}", title);
                //    string dest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), keyName);
                //    if (!File.Exists(dest))
                //    {
                //        response.WriteResponseStreamToFile(dest);
                //    }
                //}
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Please check the provided AWS Credentials.");
                    Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine("An error occurred with the message '{0}' when reading an object", amazonS3Exception.Message);
                }
            }
        }

        static void DeletingAnObject(IAmazonS3 client, string bucketName, string keyName)
        {
            try
            {
                //DeleteObjectRequest request = new DeleteObjectRequest();
                //request.WithBucketName(bucketName)
                //    .WithKey(keyName);
                //using (DeleteObjectResponse response = client.DeleteObject(request))
                //{
                //    WebHeaderCollection headers = response.Headers;
                //    foreach (string key in headers.Keys)
                //    {
                //        Console.WriteLine("Response Header: {0}, Value: {1}", key, headers.Get(key));
                //    }
                //}
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Please check the provided AWS Credentials.");
                    Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine("An error occurred with the message '{0}' when deleting an object", amazonS3Exception.Message);
                }
            }
        }

        static void ListingObjects(IAmazonS3 client, string bucketName)
        {
            try
            {
            //    ListObjectsRequest request = new ListObjectsRequest();
            //    request.BucketName = bucketName;
            //    using (ListObjectsResponse response = client.ListObjects(request))
            //    {
            //        foreach (S3Object entry in response.S3Objects)
            //        {
            //            Console.WriteLine("key = {0} size = {1}", entry.Key, entry.Size);
            //        }
            //    }

            //    // list only things starting with "foo"
            //    request.WithPrefix("foo");
            //    using (ListObjectsResponse response = client.ListObjects(request))
            //    {
            //        foreach (S3Object entry in response.S3Objects)
            //        {
            //            Console.WriteLine("key = {0} size = {1}", entry.Key, entry.Size);
            //        }
            //    }

            //    // list only things that come after "bar" alphabetically
            //    request.WithPrefix(null)
            //        .WithMarker("bar");
            //    using (ListObjectsResponse response = client.ListObjects(request))
            //    {
            //        foreach (S3Object entry in response.S3Objects)
            //        {
            //            Console.WriteLine("key = {0} size = {1}", entry.Key, entry.Size);
            //        }
            //    }

            //    // only list 3 things
            //    request.WithPrefix(null)
            //        .WithMarker(null)
            //        .WithMaxKeys(3);
            //    using (ListObjectsResponse response = client.ListObjects(request))
            //    {
            //        foreach (S3Object entry in response.S3Objects)
            //        {
            //            Console.WriteLine("key = {0} size = {1}", entry.Key, entry.Size);
            //        }
            //    }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Please check the provided AWS Credentials.");
                    Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine("An error occurred with the message '{0}' when listing objects", amazonS3Exception.Message);
                }
            }
        }
    }
}
