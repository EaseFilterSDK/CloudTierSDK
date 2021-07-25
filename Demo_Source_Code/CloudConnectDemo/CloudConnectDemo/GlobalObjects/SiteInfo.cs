
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
using System.Configuration;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace EaseFilter.GlobalObjects
{
    public enum CloudProviderType
    {
        //SMBServer = 0,
        //FtpServer,       
        AzureStorage,
        Amazon_S3,        
        //  SkyDrive,
        //  GoogleDrive,
 
    }


    public class SiteInfo
    {
        /// <summary>
        /// the cloudProvider name
        /// </summary>
        private CloudProviderType cloudProvider = CloudProviderType.Amazon_S3;

        /// <summary>
        /// Indicate if the selected working site
        /// </summary>
        private bool selected = false;
        /// <summary>
        /// The name for this site,it is unique name for the siteinfo list
        /// </summary>
        private string siteName = string.Empty;

        /// <summary>
        /// The local path which will map the remote site directory to this folder
        /// </summary>
        private string localPath = string.Empty;
        /// <summary>
        /// The remote path which will be the first entry
        /// </summary>
        private string remotePath = string.Empty;
        /// <summary>
        /// The comments for this site
        /// </summary>
        private string notes = string.Empty;

        /// <summary>
        /// The buffer size of the network stream
        /// </summary>
        private int bufferSize = 8192;

        private string filterRule = string.Empty;


        private int connectionTimeoutInSeconds = 30;

        /// <summary>
        /// the number of tasks to upload/download the single file
        /// </summary>
        private int parallelTasks = 5;

        /// <summary>
        /// enalbe multi blocks/parts upload for single file
        /// </summary>
        private bool enabledMultiBlocksUpload = true;

        /// <summary>
        /// enable multi blocks download for a single file
        /// </summary>
        private bool enabledMultiBlocksDownload = true;

        /// <summary>
        /// enable the content encryption
        /// </summary>
        private bool enableEncryption = false;

        /// <summary>
        /// if it is true, all encryption will use the same IV key.
        /// </summary>
        private bool useDefaultIVKey = false;

        /// <summary>
        /// if the encryption enable, this is the encryption pass phrase to generate the 256bits encrytpion key to encrypt the files.
        /// </summary>
        private string encryptionPassPhrase = string.Empty;

        /// <summary>
        /// caculate the md5 hash for the file content when upload the file.
        /// </summary>
        private bool enableMd5hash = true;

        /// <summary>
        /// compare the md5 hash for the file content when download the file.
        /// </summary>
        private bool enableDownloadMd5Verification = true;


        //ftp size information-------------------------------------------
        /// <summary>
        /// The ftp entryption type
        /// </summary>
        private string ftpType = "None";
        /// <summary>
        /// The remote server hostname or IP address
        /// </summary>
        private string serverName = string.Empty;
        /// <summary>
        /// The connected port number
        /// </summary>
        private int portNumber = 21;
        /// <summary>
        /// The user name to connect the remote server
        /// </summary>
        private string userName = string.Empty;
        /// <summary>
        /// The password for the user name
        /// </summary>
        private string password = string.Empty;
        /// <summary>
        /// Indicate if use anonymous as user name
        /// </summary>
        private bool anonymous = false;

        //Azure server information-------------------------------------------
        private string azureConnectionString = "";
        private string azureAccountName = "";
        private string azureSecretKey = "";
        private long azureMaxBlockSize = 65536;
        private long azureSingleBlobUploadThresholdInBytes = 1048576;
        private bool useDevelopmentStorage = false;
        private string azureEndpointSuffix = "core.windows.net";
        private bool useSSL = false;
        private bool useAzureChina = false;


        /// <summary>
        /// Amazon S3 setting----------------------------------------------
        /// </summary>
        private string s3AccessKeyId = string.Empty;
        private string s3SecretKey = string.Empty;
        private string s3Region = "US East (Virginia)";
        private long s3MaxPartSize = 5 * 1024 * 1024;

        public KeyValueSettings siteInfoSettings = new KeyValueSettings();

        public SiteInfo()
        {
        }

        public SiteInfo(KeyValueSettings _settings)
        {
            try
            {
                siteInfoSettings = _settings;

                cloudProvider = (CloudProviderType)Enum.Parse(typeof(CloudProviderType), siteInfoSettings.Get("cloudProvider", cloudProvider.ToString()));
                selected = siteInfoSettings.Get("selected", selected);
                siteName = siteInfoSettings.Get("siteName", siteName);
                localPath = siteInfoSettings.Get("localPath", localPath);
                remotePath = siteInfoSettings.Get("remotePath", remotePath);
                notes = siteInfoSettings.Get("notes", notes);

                bufferSize = siteInfoSettings.Get("bufferSize", bufferSize);
                filterRule = siteInfoSettings.Get("filterRule", filterRule);
                connectionTimeoutInSeconds = siteInfoSettings.Get("connectionTimeoutInSeconds", connectionTimeoutInSeconds);
                parallelTasks = siteInfoSettings.Get("parallelTasks", parallelTasks);
                enabledMultiBlocksUpload = siteInfoSettings.Get("enabledMultiBlocksUpload", enabledMultiBlocksUpload);
                enabledMultiBlocksDownload = siteInfoSettings.Get("enabledMultiBlocksDownload", enabledMultiBlocksDownload);
                enableEncryption = siteInfoSettings.Get("enableEncryption", enableEncryption);
                encryptionPassPhrase = siteInfoSettings.Get("encryptionPassPhrase", encryptionPassPhrase);
                useDefaultIVKey = siteInfoSettings.Get("useDefaultIVKey", useDefaultIVKey);
                enableMd5hash = siteInfoSettings.Get("enableMd5hash", enableMd5hash);
                enableDownloadMd5Verification = siteInfoSettings.Get("enableDownloadMd5Verification", enableDownloadMd5Verification);

                ftpType = siteInfoSettings.Get("ftpType", ftpType);
                serverName = siteInfoSettings.Get("serverName", serverName);
                portNumber = siteInfoSettings.Get("portNumber", portNumber);
                userName = siteInfoSettings.Get("userName", userName);
                password = siteInfoSettings.Get("password", password);
                anonymous = siteInfoSettings.Get("anonymous", anonymous);

                azureConnectionString = siteInfoSettings.Get("azureConnectionString", azureConnectionString);
                azureAccountName = siteInfoSettings.Get("azureAccountName", azureAccountName);
                azureSecretKey = siteInfoSettings.Get("azureSecretKey", azureSecretKey);
                azureMaxBlockSize = siteInfoSettings.Get("azureMaxBlockSize", azureMaxBlockSize);
                azureSingleBlobUploadThresholdInBytes = siteInfoSettings.Get("azureSingleBlobUploadThresholdInBytes", azureSingleBlobUploadThresholdInBytes);
                useDevelopmentStorage = siteInfoSettings.Get("useDevelopmentStorage", useDevelopmentStorage);
                azureEndpointSuffix = siteInfoSettings.Get("azureEndpointSuffix", azureEndpointSuffix);
                useSSL = siteInfoSettings.Get("useSSL", useSSL);
                useAzureChina = siteInfoSettings.Get("useAzureChina", useAzureChina);

                s3AccessKeyId = siteInfoSettings.Get("s3AccessKeyId", s3AccessKeyId);
                s3SecretKey = siteInfoSettings.Get("s3SecretKey", s3SecretKey);
                s3Region = siteInfoSettings.Get("s3Region", s3Region);
                s3MaxPartSize = siteInfoSettings.Get("s3MaxPartSize", s3MaxPartSize);



            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(88, "SiteInfo", EventLevel.Error, "Initialize site info exception:" + ex.Message);
            }
        }

        public int BufferSize
        {
            get
            {
                return this.bufferSize;
            }
            set
            {
                this.bufferSize = value;
                siteInfoSettings.Set("bufferSize", bufferSize.ToString());
            }
        }


        /// <summary>
        /// the ftp encryption mode
        /// </summary>
        public string FtpType
        {
            get
            {
                return this.ftpType;
            }
            set
            {
                this.ftpType = value;
                siteInfoSettings.Set("ftpType", ftpType);
            }
        }

        public CloudProviderType CloudProvider
        {
            get
            {
                return this.cloudProvider;
            }
            set
            {
                this.cloudProvider = value;
                siteInfoSettings.Set("cloudProvider", cloudProvider.ToString());
            }
        }


        public bool Selected
        {
            get
            {
                return this.selected;
            }
            set
            {
                this.selected = value;
                siteInfoSettings.Set("selected", selected.ToString());
            }
        }

        public string SiteName
        {
            get
            {
                return this.siteName;
            }
            set
            {
                this.siteName = value;
                siteInfoSettings.Set("siteName", siteName.ToString());
            }
        }

        /// <summary>
        /// the server name or ip address
        /// </summary>
        public string ServerName
        {
            get
            {
                return this.serverName;
            }
            set
            {
                this.serverName = value;
                siteInfoSettings.Set("serverName", serverName.ToString());
            }
        }

        public int PortNumber
        {
            get
            {
                return this.portNumber;
            }
            set
            {
                this.portNumber = value;
                siteInfoSettings.Set("portNumber", portNumber.ToString());
            }
        }

        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                this.userName = value;
                siteInfoSettings.Set("userName", userName.ToString());
            }
        }

        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password = value;
                siteInfoSettings.Set("password", password.ToString());
            }
        }

        public bool Anonymous
        {
            get
            {
                return this.anonymous;
            }
            set
            {
                this.anonymous = value;
                siteInfoSettings.Set("anonymous", anonymous.ToString());
            }
        }

        public string LocalPath
        {
            get
            {
                return this.localPath;
            }
            set
            {
                this.localPath = value;
                siteInfoSettings.Set("localPath", localPath.ToString());
            }
        }

        public string RemotePath
        {
            get
            {
                return this.remotePath;
            }
            set
            {
                this.remotePath = value;
                siteInfoSettings.Set("remotePath", remotePath.ToString());
            }
        }

        public string Notes
        {
            get
            {
                return this.notes;
            }
            set
            {
                this.notes = value;
                siteInfoSettings.Set("notes", notes.ToString());
            }
        }

        /// <summary>
        /// the time out setting for cloud connection in seconds
        /// </summary>
        public int ConnectionTimeout
        {
            get
            {
                return this.connectionTimeoutInSeconds;
            }
            set
            {
                this.connectionTimeoutInSeconds = value;
                siteInfoSettings.Set("connectionTimeoutInSeconds", connectionTimeoutInSeconds.ToString());
            }
        }

        /// <summary>
        /// the number of the threads for azure/s3 async task upload/download the same file
        /// </summary>
        public int ParallelTasks
        {
            get
            {
                return this.parallelTasks;
            }
            set
            {
                this.parallelTasks = value;

                if(parallelTasks <= 0)
                {
                    parallelTasks = 1;
                }

                siteInfoSettings.Set("parallelTasks", parallelTasks.ToString());
            }
        }

        /// <summary>
        /// enable/disable the multiple blocks upload for single file
        /// </summary>
        public bool EnabledMultiBlocksUpload
        {
            get
            {
                return this.enabledMultiBlocksUpload;
            }
            set
            {
                this.enabledMultiBlocksUpload = value;
                siteInfoSettings.Set("enabledMultiBlocksUpload", enabledMultiBlocksUpload.ToString());
            }
        }

        /// <summary>
        /// enable/disable the multiple blocks download for single file
        /// </summary>
        public bool EnabledMultiBlocksDownload
        {
            get
            {
                return this.enabledMultiBlocksDownload;
            }
            set
            {
                this.enabledMultiBlocksDownload = value;
                siteInfoSettings.Set("enabledMultiBlocksDownload", enabledMultiBlocksDownload.ToString());
            }
        }

        //Azure server setting----------------------------------

        /// <summary>
        /// When multil blocks upload is disabled,and the upload file size is greater than this value,it will sperate the upload task with the
        /// parallel threads(the theads number will gets from the ParallelThreads) from the system threads pool
        /// </summary>
        public long AzureSingleBlobUploadThresholdInBytes
        {
            get
            {
                return this.azureSingleBlobUploadThresholdInBytes;
            }
            set
            {
                this.azureSingleBlobUploadThresholdInBytes = value;
                siteInfoSettings.Set("azureSingleBlobUploadThresholdInBytes", azureSingleBlobUploadThresholdInBytes.ToString());
            }
        }

        /// <summary>
        /// Azure account connection string
        /// </summary>
        public string AzureConnectionString
        {
            get
            {
                return this.azureConnectionString;
            }
            set
            {
                this.azureConnectionString = value;
                siteInfoSettings.Set("azureConnectionString", azureConnectionString.ToString());
            }
        }

        /// <summary>
        /// Azure server account name
        /// </summary>
        public string AzureAccountName
        {
            get
            {
                return this.azureAccountName;
            }
            set
            {
                this.azureAccountName = value;
                siteInfoSettings.Set("azureAccountName", azureAccountName.ToString());
            }
        }

        /// <summary>
        /// Azure server endpoint suffix
        /// </summary>
        public string AzureEndpointSuffix
        {
            get
            {
                return this.azureEndpointSuffix;
            }
            set
            {
                this.azureEndpointSuffix = value;
                siteInfoSettings.Set("azureEndpointSuffix", azureEndpointSuffix.ToString());
            }
        }

        /// <summary>
        /// Azure server secret key
        /// </summary>
        public string AzureSecretKey
        {
            get
            {
                string decryptedKey = azureSecretKey;
                if (FileUtils.IsBase64String(decryptedKey))
                {
                    decryptedKey = FilterAPI.AESEncryptDecryptStr(decryptedKey, FilterAPI.EncryptType.Decryption);
                }

                return decryptedKey;
            }
            set
            {
                this.azureSecretKey = value;

                if (azureSecretKey.Length > 0)
                {
                    azureSecretKey = FilterAPI.AESEncryptDecryptStr(azureSecretKey, FilterAPI.EncryptType.Encryption);
                }

                siteInfoSettings.Set("azureSecretKey", azureSecretKey.ToString());
            }
        }

        public bool UseDevelopmentStorage
        {
            get
            {
                return this.useDevelopmentStorage;
            }
            set
            {
                this.useDevelopmentStorage = value;
                siteInfoSettings.Set("useDevelopmentStorage", useDevelopmentStorage.ToString());
            }
        }

        public bool UseSSL
        {
            get
            {
                return this.useSSL;
            }
            set
            {
                this.useSSL = value;
                siteInfoSettings.Set("useSSL", useSSL.ToString());
            }
        }

        public bool UseAzureChina
        {
            get
            {
                return this.useAzureChina;
            }
            set
            {
                this.useAzureChina = value;
                siteInfoSettings.Set("useAzureChina", useAzureChina.ToString());
            }
        }


        /// <summary>
        /// the max block size of the blob 
        /// </summary>
        public long AzureMaxBlockSize
        {
            get
            {
                return this.azureMaxBlockSize;
            }
            set
            {
                //the max block size of the block is 4 MB
                if (value > 4 * 1024 * 1024)
                {
                    this.azureMaxBlockSize = 4 * 1024 * 1024;
                }
                else
                {
                    this.azureMaxBlockSize = value;
                }

                siteInfoSettings.Set("azureMaxBlockSize", azureMaxBlockSize.ToString());
            }
        }

        /// <summary>
        /// get s3 access key id
        /// </summary>
        public string S3AccessKeyId
        {
            get
            {
                return this.s3AccessKeyId;
            }
            set
            {
                this.s3AccessKeyId = value;
                siteInfoSettings.Set("s3AccessKeyId", s3AccessKeyId.ToString());
            }
        }

        /// <summary>
        /// get amazon s3 secret key
        /// </summary>
        public string S3SecretKey
        {
            get
            {
                string decryptedKey = s3SecretKey;
                if (FileUtils.IsBase64String(decryptedKey))
                {
                    decryptedKey = FilterAPI.AESEncryptDecryptStr(decryptedKey, FilterAPI.EncryptType.Decryption);
                }

                return decryptedKey;
            }
            set
            {
                this.s3SecretKey = value;

                if (s3SecretKey.Length > 0)
                {
                    s3SecretKey = FilterAPI.AESEncryptDecryptStr(s3SecretKey, FilterAPI.EncryptType.Encryption);
                }

                siteInfoSettings.Set("s3SecretKey", s3SecretKey.ToString());
            }
        }

        /// <summary>
        /// get s3 region
        /// </summary>
        public string S3Region
        {
            get
            {
                return this.s3Region;
            }
            set
            {
                this.s3Region = value;
                siteInfoSettings.Set("s3Region", s3Region.ToString());
            }
        }

        /// <summary>
        /// the s3 maximum part size,range from 5M to 200GB
        /// </summary>
        public long S3MaxPartSize
        {
            get
            {
                return this.s3MaxPartSize;
            }
            set
            {
                if (value < 5 * 1024 * 1024)
                {
                    this.s3MaxPartSize = 5 * 1024 * 1024;
                }
                else if (value < (long)200 * 1024 * 1024 * 1024)
                {
                    this.s3MaxPartSize = value;
                }
                else
                {
                    this.s3MaxPartSize = (long)200 * 1024 * 1024 * 1024;
                }

                siteInfoSettings.Set("s3MaxPartSize", s3MaxPartSize.ToString());
            }
        }

        /// <summary>
        /// the rule to filter the files
        /// </summary>
        public string FilterRule
        {
            get
            {
                return this.filterRule;
            }
            set
            {
                this.filterRule = value;

                siteInfoSettings.Set("filterRule", filterRule.ToString());
            }
        }

        /// <summary>
        /// enable the encryption for the file content
        /// </summary>
        public bool EnableEncryption
        {
            get
            {
                return this.enableEncryption;
            }
            set
            {
                this.enableEncryption = value;

                siteInfoSettings.Set("enableEncryption", enableEncryption.ToString());
            }
        }

        /// <summary>
        /// if it is true, all encryption will use the same IV key.
        /// </summary>
        public bool UseDefaultIVKey
        {
            get
            {
                return this.useDefaultIVKey;
            }
            set
            {
                this.useDefaultIVKey = value;
                siteInfoSettings.Set("useDefaultIVKey", useDefaultIVKey.ToString());
            }
        }


        /// <summary>
        /// the encryption passphrase
        /// </summary>
        public string EncryptionPassPhrase
        {
            get
            {
                string decryptedKey = encryptionPassPhrase;
                if (FileUtils.IsBase64String(decryptedKey))
                {
                    decryptedKey = FilterAPI.AESEncryptDecryptStr(decryptedKey, FilterAPI.EncryptType.Decryption);
                }

                return decryptedKey;
            }
            set
            {
                this.encryptionPassPhrase = value;

                if (encryptionPassPhrase.Length > 0)
                {
                    encryptionPassPhrase = FilterAPI.AESEncryptDecryptStr(encryptionPassPhrase, FilterAPI.EncryptType.Encryption);
                }

                siteInfoSettings.Set("encryptionPassPhrase", encryptionPassPhrase.ToString());
            }
        }

        /// <summary>
        ///  caculate the md5 hash for the file content when upload the file.
        /// </summary>
        public bool EnableMd5hash
        {
            get
            {
                return this.enableMd5hash;
            }
            set
            {
                this.enableMd5hash = value;
                siteInfoSettings.Set("enableMd5hash", enableMd5hash.ToString());
            }
        }

        /// <summary>
        /// compare the md5 hash when download the file
        /// </summary>
        public bool EnableDownloadMd5Verification
        {
            get
            {
                return this.enableDownloadMd5Verification;
            }
            set
            {
                enableDownloadMd5Verification = value;
                siteInfoSettings.Set("enableDownloadMd5Verification", enableDownloadMd5Verification.ToString());
            }
        }
    }
}
