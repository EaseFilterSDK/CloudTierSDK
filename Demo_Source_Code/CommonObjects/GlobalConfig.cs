///////////////////////////////////////////////////////////////////////////////
//
//    (C) Copyright 2011 EaseFilter Technologies Inc.
//    All Rights Reserved
//
//    This software is part of a licensed software product and may
//    only be used or copied in accordance with the terms of that license.
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Xml;
using System.Text.RegularExpressions;
using System.Threading;
using System.Reflection;

namespace EaseFilter.CommonObjects
{


    public class GlobalConfig
    {
        //Purchase a license key with the link: http://www.easefilter.com/Order.htm
        //Email us to request a trial key: info@easefilter.com //free email is not accepted.
        public static string registerKey = "****************************************************";

        static Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
        public static string AssemblyPath = Path.GetDirectoryName(assembly.Location);

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
        static uint connectionTimeOut = 30; //seconds
        static List<uint> includePidList = new List<uint>();
        static List<uint> excludePidList = new List<uint>();

        static int maximumFilterMessages = 5000;

        static string configFileName = ConfigSetting.GetFilePath();

        //if it was true, when the reparsepoint file was opened with "FILE_OPEN_NO_RECALL", it won't restore data back for read and write.
        static bool enableNoRecallFlag = false;

        //if this flag is true, the stub file will be rehydrated on first read.
        static bool rehydrateFileOnFirstRead = false;

        //if this flag is true, the filter driver will reopen the file when the stub file was rehydrated.
        static bool reOpenFileOneReHydration = false;

        static bool returnCacheFileName = false;

        static bool returnBlockData = true;

        public static bool isRunning = true;
        public static ManualResetEvent stopEvent = new ManualResetEvent(false);


        public static System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

        static GlobalConfig()
        {
            stopWatch.Start();

            uint currentPid = (uint)System.Diagnostics.Process.GetCurrentProcess().Id;
            excludePidList.Add(currentPid);

            try
            {
                filterConnectionThreads = ConfigSetting.Get("filterConnectionThreads", filterConnectionThreads);
                connectionTimeOut = ConfigSetting.Get("connectionTimeOut", connectionTimeOut);
                maximumFilterMessages = ConfigSetting.Get("maximumFilterMessages", maximumFilterMessages);
                enableNoRecallFlag = ConfigSetting.Get("enableNoRecallFlag", enableNoRecallFlag);
                rehydrateFileOnFirstRead = ConfigSetting.Get("rehydrateFileOnFirstRead", rehydrateFileOnFirstRead);
                returnCacheFileName = ConfigSetting.Get("returnCacheFileName", returnCacheFileName);
                returnBlockData = ConfigSetting.Get("returnBlockData", returnBlockData);
                reOpenFileOneReHydration = ConfigSetting.Get("reOpenFileOneReHydration", reOpenFileOneReHydration);

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
     
        public static bool SaveConfigSetting()
        {
            bool ret = true;

            try
            {
                ConfigSetting.Save();
                SendConfigSettingsToFilter();
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

        public static uint ConnectionTimeOut
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
        /// if this flag is true, the filter driver will reopen the file when the stub file was rehydrated.
        /// </summary>
        public static bool ReOpenFileOneReHydration
        {
            get { return reOpenFileOneReHydration; }
            set
            {
                reOpenFileOneReHydration = value;
                ConfigSetting.Set("reOpenFileOneReHydration", value.ToString());
            }
        }

        public static void SendConfigSettingsToFilter()
        {
            try
            {
                FilterAPI.ResetConfigData();

                FilterAPI.SetConnectionTimeout(connectionTimeOut);

                uint boolConfig = 0;
                if (reOpenFileOneReHydration)
                {
                    boolConfig = (uint)FilterAPI.BooleanConfig.ENABLE_REOPEN_FILE_ON_REHYDRATION;                    
                }

                FilterAPI.SetBooleanConfig(boolConfig);

                foreach (uint includedPid in includePidList)
                {
                    FilterAPI.AddIncludedProcessId(includedPid);
                }

                foreach (uint excludedPid in excludePidList)
                {
                    FilterAPI.AddExcludedProcessId(excludedPid);
                }

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(502, "SendConfigSettingsToFilter", CommonObjects.EventLevel.Error, "Send config settings to filter failed with error " + ex.Message);
            }
        }
    }
}
