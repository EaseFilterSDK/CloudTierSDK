
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
using System.IO;
using System.Text;
using System.Xml;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace EaseFilter.GlobalObjects
{
    public enum EventLevel
    {
        Off = 1,
        Error = 2,
        Warning = 4,
        Information = 8,
        Verbose = 0x10,
        Trace = 0x20,
    }

    public enum EventOutputType
    {
        EventViewer = 0,
        File,
        Console,
        CallbackDelegate,
        NamedPipe,
        DbgViewer,
    }    
  
    
    public class GlobalConfig
    {
        //Purchase a license key with the link: http://www.easefilter.com/Order.htm
        //Email us to request a trial key: info@easefilter.com //free email is not accepted.
        public static string registerKey = "****************************************************";

        public static byte[] defaultIVKey = {0xa0,0xa1,0xa2,0xa3,0xa4,0xa5,0xa6,0xa7,0xa8,0xa9,0xaa,0xab,0xac,0xad,0xae,0xaf};// Initialization vector

        static Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
        public static string AssemblyPath = Path.GetDirectoryName(assembly.Location);
        public static string UsrAppFolder = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

        //the message output level. It will output the messages which less than this level.
        static EventLevel eventLevel = EventLevel.Information;
        static EventLevel selectedDisplayEvents = EventLevel.Information | EventLevel.Error | EventLevel.Warning;

        //The reciever which the message will output to.
        static EventOutputType eventOutputType = EventOutputType.EventViewer;

        //The log file name if outputType is ToFile.
        static string logFileName = "EventLog.log";
        static int maxLogFileSize = 4 * 1024 * 1024; //4MB
        static string eventSource = "CloudTier";
        static string eventLogName = "EaseFilter Cloud Connect";

        //maximum threads used for communication between service and driver.
        static int fileSystemThreads = 10;

        static int numberOfParallelTasks = 5;

        //the cache folder for the client,used for temporary data
        static string cloudCacheFolder = "c:\\CloudTier\\CloudCacheFolder";
        public static string cloudStubFileFolder = "c:\\CloudTier\\CloudStubFileFolder";
        public static string testSourceFolder = @"c:\CloudTier\TestSourceFolder";
        public static string testStubFilesFolder = @"c:\CloudTier\TestStubFolder";

        //the default name to store the directory file list in cache folder.
        static string dirInfoListName = "CloudDirlist.dat";

        /// <summary>
        /// The cloud provider site information list
        /// </summary>
        static private Dictionary<string, SiteInfo> siteInfos = new Dictionary<string, SiteInfo>();

        /// <summary>
        /// The cache directory file list life time
        /// </summary>
        static private int expireCachedDirectoryListingAfterSeconds = 60;

        /// <summary>
        /// delete the cached files after x seconds
        /// </summary>
        static private int deleteCachedFilesAfterSeconds = 60 * 60;

        /// <summary>
        /// this is the time out of the virtual file system wait for the reply of the request.
        /// </summary>
        static private int fileSystemWaitTimeoutInSeconds = 30;

        static int maximumFilterMessages = 5000;

        /// <summary>
        /// this is the time out of the cloud service response.
        /// </summary>
        static private int serviceResponseTimeoutInSeconds = 60;

        static private bool deleteCachedFilesOnConnect = true;

        static private int blockSize = 65536;

        //if this flag is true, the stub file will be rehydrated on first read.
        static bool rehydrateFileOnFirstRead = false;

        //if this flag is true, the filter driver will get data from the cache file.
        static bool returnCacheFileName = true;

        static bool returnBlockData = false;

        static List<uint> includePidList = new List<uint>();
        static List<uint> excludePidList = new List<uint>();

        /// <summary>
        /// The flag to show if the programme is running
        /// </summary>
        static bool isRunning = true;
        static ManualResetEvent stopEvent = new ManualResetEvent(false);

        static private KeyValueSettings appSettings = new KeyValueSettings();

        public static System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

        static GlobalConfig()
        {
            LoadConfigInfo();
        }

        public static void LoadConfigInfo()
        {
            try
            {
                stopWatch.Start();

                ConfigSetting.LoadConfigSetting();

                appSettings = ConfigSetting.AppSettings;

                siteInfos = ConfigSetting.GetSiteInfos();

                uint currentPid = (uint)System.Diagnostics.Process.GetCurrentProcess().Id;
                excludePidList.Add(currentPid);

                rehydrateFileOnFirstRead = appSettings.Get("rehydrateFileOnFirstRead", rehydrateFileOnFirstRead);
                returnCacheFileName = appSettings.Get("returnCacheFileName", returnCacheFileName);
                returnBlockData = appSettings.Get("returnBlockData", returnBlockData);

                eventLevel = (EventLevel)(Enum.Parse(typeof(EventLevel), appSettings.Get("eventLevel", eventLevel.ToString())));
                selectedDisplayEvents = (EventLevel)appSettings.Get("selectedDisplayEvents", (int)selectedDisplayEvents);
                eventOutputType = (EventOutputType)(Enum.Parse(typeof(EventOutputType), appSettings.Get("eventOutputType", eventOutputType.ToString())));
                logFileName = appSettings.Get("logFileName", logFileName);
                maxLogFileSize = appSettings.Get("maxLogFileSize", maxLogFileSize);
                eventSource = appSettings.Get("eventSource", eventSource);
                eventLogName = appSettings.Get("eventLogName", eventLogName);
                maximumFilterMessages = appSettings.Get("maximumFilterMessages", maximumFilterMessages);

                fileSystemThreads = appSettings.Get("fileSystemThreads", fileSystemThreads);
                numberOfParallelTasks = appSettings.Get("numberOfParallelTasks", numberOfParallelTasks);
                cloudCacheFolder = appSettings.Get("cloudCacheFolder", cloudCacheFolder);
                dirInfoListName = appSettings.Get("dirInfoListName", dirInfoListName);
                deleteCachedFilesOnConnect = appSettings.Get("deleteCachedFilesOnConnect", deleteCachedFilesOnConnect);
                expireCachedDirectoryListingAfterSeconds = appSettings.Get("expireCachedDirectoryListingAfterSeconds", expireCachedDirectoryListingAfterSeconds);
                deleteCachedFilesAfterSeconds = appSettings.Get("deleteCachedFilesAfterSeconds", deleteCachedFilesAfterSeconds);
                fileSystemWaitTimeoutInSeconds = appSettings.Get("fileSystemWaitTimeoutInSeconds", fileSystemWaitTimeoutInSeconds);
                serviceResponseTimeoutInSeconds = appSettings.Get("serviceResponseTimeoutInSeconds", serviceResponseTimeoutInSeconds);

                if (deleteCachedFilesOnConnect)
                {
                    FileUtils.ClearCachedFiles();
                }

                int deleteFilesInterval = deleteCachedFilesAfterSeconds < expireCachedDirectoryListingAfterSeconds ? deleteCachedFilesAfterSeconds : expireCachedDirectoryListingAfterSeconds;

                System.Timers.Timer clearCachedFilesTimer = new System.Timers.Timer();
                clearCachedFilesTimer.Interval = deleteFilesInterval * 1000; //millisecond
                clearCachedFilesTimer.Start();
                clearCachedFilesTimer.Enabled = true;
                clearCachedFilesTimer.Elapsed += new System.Timers.ElapsedEventHandler(FileUtils.DeleteExpiredCachedFiles);

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(256, "LoadConfigInfo", GlobalObjects.EventLevel.Error, "Load config file " + ConfigSetting.configFileName + " failed with error:" + ex.Message);
            }
        }

        static public bool IsRunning
        {
            get { return isRunning; }
        }

        static public ManualResetEvent StopEvent
        {
            get { return stopEvent; }
        }

        static public void Start()
        {
            isRunning = true;
            stopEvent.Reset();
        }

        static public void Stop()
        {
            isRunning = false;
            stopEvent.Set();

        }
        public static bool SaveConfigInfo()
        {
            bool ret = true;

            try
            {
                ConfigSetting.ClearSiteInfoSection();

                foreach (SiteInfo siteInfo in siteInfos.Values)
                {
                    ConfigSetting.AddSiteInfo(siteInfo);
                }

                ConfigSetting.Save();

               
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(214, "SaveConfigSetting", GlobalObjects.EventLevel.Error, "Save config file " + ConfigSetting.configFileName + " failed with error:" + ex.Message);
                ret = false;
            }

            return ret;
        }

        public static bool SendConfigSettingsToFilter()
        {
            try
            {
                FilterAPI.ResetConfigData();
                FilterAPI.SetConnectionTimeout((uint)fileSystemWaitTimeoutInSeconds);

                return true;
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(502, "SendConfigSettingsToFilter", EventLevel.Error, "Send config settings to filter failed with error " + ex.Message);
            }

            return false;
        }


        /// <summary>
        /// if this flag is true, the stub file will be rehydrated on first read.
        /// </summary>
        public static bool RehydrateFileOnFirstRead
        {
            get { return rehydrateFileOnFirstRead; }
            set
            {
                rehydrateFileOnFirstRead = value;
                appSettings.Set("rehydrateFileOnFirstRead", value.ToString());
            }
        }

        /// <summary>
        /// if this flag is true, the whole cache file name will be returned.
        /// </summary>
        public static bool ReturnCacheFileName
        {
            get { return returnCacheFileName; }
            set
            {
                returnCacheFileName = value;
                appSettings.Set("returnCacheFileName", value.ToString());
            }
        }

        /// <summary>
        /// if this flag is true, the block data will return to driver
        /// </summary>
        public static bool ReturnBlockData
        {
            get { return returnBlockData; }
            set
            {
                returnBlockData = value;
                appSettings.Set("returnBlockData", value.ToString());
            }
        }

        /// <summary>
        ///   this is the time out of the virtual file system wait for the reply of the request.
        /// </summary>
        static public int FileSystemWaitTimeoutInSeconds
        {
            get
            {
                return fileSystemWaitTimeoutInSeconds;
            }
            set
            {
                fileSystemWaitTimeoutInSeconds = value;
                appSettings.Set("fileSystemWaitTimeoutInSeconds", fileSystemWaitTimeoutInSeconds.ToString());
            }
        }

        
        /// <summary>
        /// this is the time out of the cloud service response,if the cloud provider didn't response in x seconds,the cloud service 
        /// will timeout and return the failed request to the virtual file system.
        /// </summary>
        static public int ServiceResponseTimeoutInSeconds
        {
            get
            {
                return serviceResponseTimeoutInSeconds;
            }
            set
            {
                serviceResponseTimeoutInSeconds = value;
                appSettings.Set("serviceResponseTimeoutInSeconds", serviceResponseTimeoutInSeconds.ToString());
            }
        }

        public static int MaximumFilterMessages
        {
            get { return maximumFilterMessages; }
            set
            {
                maximumFilterMessages = value;
                appSettings.Set("maximumFilterMessages", value.ToString());
            }
        }

        /// <summary>
        ///  the block size for download / upload 
        /// </summary>
        static public int BlockSize
        {
            get
            {
                return blockSize;
            }
            set
            {
                int result = 0;
                System.Math.DivRem(value, 65536, out result);

                if (value < 65536 || result != 0)
                {
                    throw new Exception("The block size " + value + " is invalid.It must be 65536 or multiple times of 65536.");
                }

                appSettings.Set("blockSize", blockSize.ToString());

                blockSize = value;
            }
        }

        public static List<uint> IncludePidList
        {
            get { return includePidList; }
            set { includePidList = value; }
        }

        public static List<uint> ExcludePidList
        {
            get { return excludePidList; }
            set { excludePidList = value; }
        }


        /// <summary>
        /// The cache dir file info list and cache file life time, if the cache directory file list or
        /// cache file laste write time greater than the value, it needs to re-download from the server, 
        /// or it will use the local data.
        /// </summary>
        static public int ExpireCachedDirectoryListingAfterSeconds
        {
            get
            {
                return expireCachedDirectoryListingAfterSeconds;
            }
            set
            {
                expireCachedDirectoryListingAfterSeconds = value;
                appSettings.Set("expireCachedDirectoryListingAfterSeconds", expireCachedDirectoryListingAfterSeconds.ToString());
            }
        }


        /// <summary>
        /// delete the cached files after it was created in seconds x
        /// </summary>
        static public int DeleteCachedFilesAfterSeconds
        {
            get
            {
                return deleteCachedFilesAfterSeconds;
            }
            set
            {
                deleteCachedFilesAfterSeconds = value;
                appSettings.Set("deleteCachedFilesAfterSeconds", deleteCachedFilesAfterSeconds.ToString());
            }
        }



        static public string DirInfoListName
        {
            get { return dirInfoListName; }
            set
            {
                dirInfoListName = value;
                appSettings.Set("dirInfoListName", dirInfoListName);
            }
        }



        static public Dictionary<string, SiteInfo> SiteInfos
        {
            get
            {
                return siteInfos;
            }
            set
            {
                siteInfos = value;
            }
        }

     
        static public EventLevel SelectedDisplayEvents
        {
            get
            {
                return selectedDisplayEvents;
            }
            set
            {
                selectedDisplayEvents = value;
                appSettings.Set("selectedDisplayEvents", ((int)selectedDisplayEvents).ToString());
            }
        }

        static public EventLevel EventLevel
        {
            get
            {
                return eventLevel;
            }
            set
            {
                eventLevel = value;
                appSettings.Set("eventLevel", eventLevel.ToString());
            }
        }

        static public EventOutputType EventOutputType
        {
            get
            {
                return eventOutputType;
            }
            set
            {
                eventOutputType = value;
                appSettings.Set("eventOutputType", eventOutputType.ToString());
            }
        }

        static public string LogFileName
        {
            get
            {
                return logFileName;
            }
            set
            {
                logFileName = value;
                appSettings.Set("logFileName", logFileName.ToString());
            }
        }

        static public int MaxLogFileSize
        {
            get
            {
                return maxLogFileSize;
            }
            set
            {
                maxLogFileSize = value;
                appSettings.Set("maxLogFileSize", maxLogFileSize.ToString());
            }
        }

        static public string EventSource
        {
            get
            {
                return eventSource;
            }
            set
            {
                eventSource = value;
                appSettings.Set("eventSource", eventSource.ToString());
            }
        }


        static public string EventLogName
        {
            get
            {
                return eventLogName;
            }
            set
            {
                eventLogName = value;
                appSettings.Set("eventLogName", eventLogName.ToString());
            }
        }

        /// <summary>
        /// The number of threads for the file system service
        /// </summary>
        static public int FilterConnectionThreads
        {
            get
            {
                return fileSystemThreads;
            }
            set
            {
                fileSystemThreads = value;
                appSettings.Set("fileSystemThreads", fileSystemThreads.ToString());
            }
        }

        static public int NumberOfParallelTasks
        {
            get
            {
                return numberOfParallelTasks;
            }
            set
            {
                numberOfParallelTasks = value;
                appSettings.Set("numberOfParallelTasks", numberOfParallelTasks.ToString());
            }
        }

        static public string CloudCacheFolder
        {
            get
            {               
                return cloudCacheFolder;
            }
            set
            {
                cloudCacheFolder = value;

                //verify the folder exist
                if (!Directory.Exists(cloudCacheFolder))
                {
                    Directory.CreateDirectory(cloudCacheFolder);
                }

                appSettings.Set("cloudCacheFolder", cloudCacheFolder.ToString());
            }
        }

        static public bool DeleteCachedFilesOnConnect
        {
            get
            {
                return deleteCachedFilesOnConnect;
            }
            set
            {
                deleteCachedFilesOnConnect = value;

                appSettings.Set("deleteCachedFilesOnConnect", deleteCachedFilesOnConnect.ToString());

            }
        }

   
    }

}

