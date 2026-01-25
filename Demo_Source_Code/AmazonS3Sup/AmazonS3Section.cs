///////////////////////////////////////////////////////////////////////////////
//
//    (C) Copyright 2011 EaseFilter Inc.
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

using System.Configuration;

namespace CloudTier.AmazonS3Sup
{
    public class AmazonS3Section : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public AmazonS3Collection Instances
        {
            get { return (AmazonS3Collection)this[""]; }
            set { this[""] = value; }
        }
    }

    public class AmazonS3Collection : ConfigurationElementCollection
    {
        public AmazonS3SiteInfo this[int index]
        {
            get { return (AmazonS3SiteInfo)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(AmazonS3SiteInfo s3SiteInfo)
        {
            BaseAdd(s3SiteInfo);
        }

        public void Clear()
        {
            BaseClear();
        }

        public void Remove(AmazonS3SiteInfo s3SiteInfo)
        {
            BaseRemove(s3SiteInfo.SiteName);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AmazonS3SiteInfo();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AmazonS3SiteInfo)element).SiteName;
        }
    }

    public class AmazonS3SiteInfo : ConfigurationElement
    {
        /// <summary>
        /// Amazon S3 cloud provider. 
        /// </summary>
        public static string ProviderName
        {
            get { return "AmazonS3"; }
        }
        /// <summary>
        /// the name of the s3 site. 
        /// </summary>
        [ConfigurationProperty("siteName", IsRequired = false)]
        public string SiteName
        {
            get { return (string)base["siteName"]; }
            set { base["siteName"] = value; }
        }

        /// <summary>
        /// the local mapping folder path. 
        /// </summary>
        [ConfigurationProperty("localPath", IsKey = false, IsRequired = false)]
        public string LocalPath
        {
            get { return (string)base["localPath"]; }
            set { base["localPath"] = value; }
        }

        /// <summary>
        /// the s3 remote path. 
        /// </summary>
        [ConfigurationProperty("remotePath", IsKey = false, IsRequired = false)]
        public string RemotePath
        {
            get { return (string)base["remotePath"]; }
            set { base["remotePath"] = value; }
        }

        /// <summary>
        /// the access key Id
        /// </summary>
        [ConfigurationProperty("accessKeyId", IsRequired = false)]
        public string AccessKeyId
        {
            get { return (string)base["accessKeyId"]; }
            set { base["accessKeyId"] = value; }
        }

        /// <summary>
        /// the s3 secret access key
        /// </summary>
        [ConfigurationProperty("secretAccessKey", IsRequired = false)]
        public string SecretAccessKey
        {
            get { return (string)base["secretAccessKey"]; }
            set { base["secretAccessKey"] = value; }
        }

        /// <summary>
        /// the s3 region name
        /// </summary>
        [ConfigurationProperty("regionName", IsRequired = false)]
        public string RegionName
        {
            get { return (string)base["regionName"]; }
            set { base["regionName"] = value; }
        }

        /// <summary>
        /// the buffer size of the connection
        /// </summary>
        [ConfigurationProperty("bufferSize", IsRequired = false)]
        public uint BufferSize
        {
            get { return (uint)base["bufferSize"]; }
            set { base["bufferSize"] = value; }
        }

        /// <summary>
        /// the s3 maximum part size,range from 5M to 200GB
        /// </summary>
        [ConfigurationProperty("partSize", IsRequired = false)]
        public uint PartSize
        {
            get { return (uint)base["partSize"]; }
            set { base["partSize"] = value; }
        }

        /// <summary>
        /// indicates if enabled multiple part upload
        /// </summary>
        [ConfigurationProperty("enabledMultiBlocksUpload", IsRequired = false)]
        public bool EnabledMultiBlocksUpload
        {
            get { return (bool)base["enabledMultiBlocksUpload"]; }
            set { base["enabledMultiBlocksUpload"] = value; }
        }

        /// <summary>
        ///  the number of the threads for s3 async task upload/download the same file
        /// </summary>
        [ConfigurationProperty("parallelTasks", IsRequired = true)]
        public uint ParallelTasks
        {
            get { return (uint)base["parallelTasks"]; }
            set { base["parallelTasks"] = value; }
        }
      
    }


}
