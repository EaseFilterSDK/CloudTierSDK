///////////////////////////////////////////////////////////////////////////////
//
//    (C) Copyright 2011 EaseFilter
//    All Rights Reserved
//
//    This software is part of a licensed software product and may
//    only be used or copied in accordance with the terms of that license.
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Reflection;

namespace CloudTier.CommonObjects
{


    public class GlobalConfig
    {

        static Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
        public static string AssemblyPath = Path.GetDirectoryName(assembly.Location);
        public static string AssemblyName = assembly.Location;

        //the message output level. It will output the messages which less than this level.
        static EventLevel eventLevel = EventLevel.Information;
        static bool[] selectedDisplayEvents = new bool[] { false, true, true, true, false, false };
        static EventOutputType eventOutputType = EventOutputType.EventView;
        //The log file name if outputType is ToFile.
        static string eventLogFileName = "EventLog.txt";
        static int maxEventLogFileSize = 4 * 1024 * 1024; //4MB
        static string eventSource = "EaseFilter";
        static string eventLogName = "EaseFilter";

        static uint filterConnectionThreads = 5;
        static int connectionTimeOut = 30; //seconds
        static List<uint> includePidList = new List<uint>();
        static List<uint> excludePidList = new List<uint>();

        static int maximumFilterMessages = 5000;

        static string configFileName = ConfigSetting.GetFilePath();       

        /// <summary>
        /// rehydrate the stub file on the first read if it is true.
        /// </summary>
        static bool rehydrateFileOnFirstRead = false;

        //if this flag is true, the filter driver will reopen the file when the stub file was rehydrated.
        static bool reOpenFileOneReHydration = false;
        /// <summary>
        /// download the whole file to the cache folder, and return the cache file name to the filter driver.
        /// </summary>
        static bool returnCacheFileName = false;
        /// <summary>
        /// return the block data which the application requested if it is true.
        /// </summary>
        static bool returnBlockData = true;

        /// <summary>
        /// the folder to store the cache files
        /// </summary>
        static string cacheFolder = AssemblyPath + "\\CacheFolder";

        /// <summary>
        /// the file name of the virtual folder's file list meta data's file.
        /// </summary>
        static string virtualFileListName = "virtualFileList.data";

        /// <summary>
        /// The cache directory file list life time
        /// </summary>
        static private int expireCachedDirectoryListingAfterSeconds = 60;

        /// <summary>
        /// delete the cached files after x seconds
        /// </summary>
        static private int deleteCachedFilesAfterSeconds = 60 * 60;

        public static bool isRunning = true;
        public static ManualResetEvent stopEvent = new ManualResetEvent(false);

        static string accountName = "";
        static string licenseKey = "";
        static long expireTime = 0;


        public static System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

        static GlobalConfig()
        {
            stopWatch.Start();

            uint currentPid = (uint)System.Diagnostics.Process.GetCurrentProcess().Id;
            excludePidList.Add(currentPid);

            try
            {
                eventLevel = (EventLevel)ConfigSetting.Get("eventLevel", (int)eventLevel);
                virtualFileListName = ConfigSetting.Get("virtualFileListName", virtualFileListName);
                cacheFolder = ConfigSetting.Get("cacheFolder", cacheFolder);
                filterConnectionThreads = ConfigSetting.Get("filterConnectionThreads", filterConnectionThreads);
                connectionTimeOut = ConfigSetting.Get("connectionTimeOut", connectionTimeOut);
                maximumFilterMessages = ConfigSetting.Get("maximumFilterMessages", maximumFilterMessages);
                rehydrateFileOnFirstRead = ConfigSetting.Get("rehydrateFileOnFirstRead", rehydrateFileOnFirstRead);
                returnCacheFileName = ConfigSetting.Get("returnCacheFileName", returnCacheFileName);
                returnBlockData = ConfigSetting.Get("returnBlockData", returnBlockData);
                reOpenFileOneReHydration = ConfigSetting.Get("reOpenFileOneReHydration", reOpenFileOneReHydration);

                accountName = ConfigSetting.Get("accountName", accountName);
                licenseKey = ConfigSetting.Get("licenseKey", licenseKey);
                expireTime = ConfigSetting.Get("expireTime", expireTime);

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(176, "LoadConfigSetting", CommonObjects.EventLevel.Error, "Load config file " + configFileName + " failed with error:" + ex.Message);
            }
        }

        public static void Stop()
        {
            isRunning = false;
            stopEvent.Set();
        }

        public static string AccountName
        {
            get { return accountName; }
            set
            {
                accountName = value;
                ConfigSetting.Set("accountName", value.ToString());
            }
        }

        public static string LicenseKey
        {
            get
            {
                //Purchase a license key with the link: http://www.easefilter.com/Order.htm
                //Email us to request a trial key: info@easefilter.com //free email is not accepted.

                //for demo code.
                if (string.IsNullOrEmpty(licenseKey))
                {
                    System.Windows.Forms.MessageBox.Show("You don't have a valid license key, Please contact support@easefilter.com to get a trial key.", "LicenseKey",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
                
                return licenseKey;
                
            }

            set
            {
                licenseKey = value;
                ConfigSetting.Set("licenseKey", value.ToString());
            }
        }

        public static long ExpireTime
        {
            get { return expireTime; }
            set
            {
                expireTime = value;
                ConfigSetting.Set("expireTime", value.ToString());
            }
        }
     
        public static bool SaveConfigSetting()
        {
            bool ret = true;

            try
            {
                ConfigSetting.Save();
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(235, "SaveConfigSetting", CommonObjects.EventLevel.Error, "Save config file " + configFileName + " failed with error:" + ex.Message);
                ret = false;
            }

            return ret;
        }

        static public bool IsRunning
        {
            get { return isRunning; }
        }

        static public ManualResetEvent StopEvent
        {
            get { return stopEvent; }
        }

        static public bool[] SelectedDisplayEvents
        {
            get
            {
                return selectedDisplayEvents;
            }
            set
            {
                selectedDisplayEvents = value;
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
                ConfigSetting.Set("eventLevel", ((int)value).ToString());
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
            }
        }

        static public string EventLogFileName
        {
            get
            {
                return eventLogFileName;
            }
            set
            {
                eventLogFileName = value;
            }
        }

        static public int MaxEventLogFileSize
        {
            get
            {
                return maxEventLogFileSize;
            }
            set
            {
                maxEventLogFileSize = value;
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
            }
        }


        public static uint FilterConnectionThreads
        {
            get { return filterConnectionThreads; }
            set
            { 
                filterConnectionThreads = value;
                ConfigSetting.Set("filterConnectionThreads", value.ToString());
            }
        }

     
        public static int MaximumFilterMessages
        {
            get { return maximumFilterMessages; }
            set
            { 
                maximumFilterMessages = value;
                ConfigSetting.Set("maximumFilterMessages", value.ToString());
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

        public static int ConnectionTimeOut
        {
            get { return connectionTimeOut; }
            set 
            {
                connectionTimeOut = value;
                ConfigSetting.Set("connectionTimeOut", value.ToString());
            }
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
                ConfigSetting.Set("rehydrateFileOnFirstRead", value.ToString());
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
                ConfigSetting.Set("returnCacheFileName", value.ToString());
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
                ConfigSetting.Set("returnBlockData", value.ToString());
            }
        }

        /// <summary>
        /// if this flag is true, the filter driver will reopen the file when the stub file was rehydrated to bypass the write event for monitor filter driver.
        /// </summary>
        public static bool ByPassWriteEventOnReHydration
        {
            get { return reOpenFileOneReHydration; }
            set
            {
                reOpenFileOneReHydration = value;
                ConfigSetting.Set("reOpenFileOneReHydration", value.ToString());
            }
        }

        /// <summary>
        /// The cache folder to store the cache files which download from the remote server.
        /// </summary>
        public static string CacheFolder
        {
            get { return cacheFolder; }
            set
            {
                cacheFolder = value;
                ConfigSetting.Set("cacheFolder", value.ToString());
            }
        }

        /// <summary>
        /// The file name of the virtual folder file list, includes all files' name, size, attribute, tag data and sub directories name.
        /// </summary>
        public static string VirtualFileListName
        {
            get { return virtualFileListName; }
            set
            {
                virtualFileListName = value;
                ConfigSetting.Set("virtualFileListName", value.ToString());
            }
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
                ConfigSetting.Set("expireCachedDirectoryListingAfterSeconds", expireCachedDirectoryListingAfterSeconds.ToString());
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
                ConfigSetting.Set("deleteCachedFilesAfterSeconds", deleteCachedFilesAfterSeconds.ToString());
            }
        }



    }
}
