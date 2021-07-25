
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
using System.Threading.Tasks;

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

using EaseFilter.GlobalObjects;

namespace EaseFilter.CloudManager
{

    public class AzureBlob 
    {
        CloudBlobClient cloudBlobClient = null;
        SiteInfo siteInfo = null;      

        public AzureBlob(CloudBlobClient cloudBlobClient, SiteInfo siteInfo)
        {
            this.cloudBlobClient = cloudBlobClient;
            this.siteInfo = siteInfo;
        }

        public async Task<IEnumerable<CloudBlobContainer>> ListContainersAsync(CloudBlobClient cloudBlobClient)
        {
            BlobContinuationToken continuationToken = null;
            var containers = new List<CloudBlobContainer>();

            do
            {
                var response = await cloudBlobClient.ListContainersSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                containers.AddRange(response.Results);

            } while (continuationToken != null);


            return containers;
        }


        public async Task<DirectoryList> FileListAsync(string directory, DirectoryList dirFileList)
        {

            String path = directory;

            if (path.Length != 0 && !path.EndsWith("/"))
            {
                path += "/";
            }


            if (path == string.Empty)
            {
                try
                {
                    bool containerFound = false;
                    var containers = await ListContainersAsync(cloudBlobClient);

                    foreach (var item in containers)
                    {
                        dirFileList.AddFileEntry(item.Name,
                            ASCIIEncoding.ASCII.GetBytes(item.Properties.ETag),
                            item.Properties.LastModified.Value.DateTime.ToFileTimeUtc(),
                            item.Properties.LastModified.Value.DateTime.ToFileTimeUtc(),
                            item.Properties.LastModified.Value.DateTime.ToFileTimeUtc(),
                            0,
                            (uint)FileAttributes.Directory);

                        containerFound = true;

                        EventManager.WriteMessage(63, "FileList", EventLevel.Verbose, "Azure get container:" + item.Name);
                    }

                    if (!containerFound)
                    {
                        EventManager.WriteMessage(72, "FileList", EventLevel.Information, "Azure can't get any container for account:" + siteInfo.AzureAccountName);
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Azure get directory list failed with error:" + ex.Message);
                }
            }
            else
            {
                try
                {
                    string containerName = path.Substring(0, path.IndexOf('/') );
                    string prefix = path.Substring(path.IndexOf('/') +1 );

                    CloudBlobContainer container = cloudBlobClient.GetContainerReference(containerName);
                    BlobContinuationToken continuationToken = null;
                    do
                    {
                        BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(prefix,
                                false, BlobListingDetails.Metadata, null, continuationToken, null, null);
                        foreach (var blobItem in resultSegment.Results)
                        {
                            if (blobItem is CloudBlobDirectory)
                            {
                                var itemDirectory = blobItem as CloudBlobDirectory;
                                var dateStamp = DateTime.UtcNow;
                                var directoryName = itemDirectory.Uri.PathAndQuery.Substring(itemDirectory.Parent.Uri.PathAndQuery.Length);

                                if (directoryName.StartsWith(@"/"))
                                {
                                    directoryName = directoryName.Substring(1, directoryName.Length - 1);
                                }

                                if (directoryName.EndsWith(@"/"))
                                {
                                    directoryName = directoryName.Substring(0, directoryName.Length - 1);
                                }


                                //ASCIIEncoding.ASCII.GetBytes(itemDirectory.Uri.PathAndQuery)

                                dirFileList.AddFileEntry(directoryName,
                                                            null,
                                                            dateStamp.ToFileTimeUtc(),
                                                            dateStamp.ToFileTimeUtc(),
                                                            dateStamp.ToFileTimeUtc(),
                                                            0,
                                                            (uint)FileAttributes.Directory);

                                EventManager.WriteMessage(100, "FileList", EventLevel.Verbose, "Azure get directoryName:" + directoryName);
                            }
                            else
                            {
                                // Write out the name of the blob.
                                var itemBlob = blobItem as CloudBlockBlob;
                                var fileName = Path.GetFileName(itemBlob.Name);

                                if (fileName.Length > 0)
                                {
                                    long lastModifiedTime = DateTime.Now.ToLocalTime().ToFileTime();

                                    if (null != itemBlob.Properties.LastModified)
                                    {
                                        lastModifiedTime = itemBlob.Properties.LastModified.Value.DateTime.ToLocalTime().ToFileTime();
                                    }

                                    //ASCIIEncoding.ASCII.GetBytes(itemBlob.Properties.ETag)
                                    dirFileList.AddFileEntry(fileName,
                                                                null,
                                                                lastModifiedTime,
                                                                lastModifiedTime,
                                                                lastModifiedTime,
                                                                itemBlob.Properties.Length,
                                                                (uint)FileAttributes.Offline);
                                    //  EventManager.WriteMessage(116, "FileList", EventLevel.Verbose, "Azure get fileName:" + fileName);
                                }

                            }
                        }

                        continuationToken = resultSegment.ContinuationToken;

                    } while (continuationToken != null);

       
                }
                catch (Exception ex)
                {
                    throw new Exception("Azure get directory " + directory + " file list failed with error:" + ex.Message);
                }
            }

            return dirFileList;
        }


    }
}

