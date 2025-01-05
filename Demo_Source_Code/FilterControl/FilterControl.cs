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
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace CloudTier.FilterControl
{

    /// <summary>
    /// This is the general information for every file IO which was sent by filter driver if you register the callback IO.
    /// </summary>
    public class FilterRequestEventArgs : EventArgs
    {
        public FilterRequestEventArgs(IntPtr sendDataPtr, IntPtr replyDataPtr)
        {
            messageSend = (FilterAPI.MessageSendData)Marshal.PtrToStructure(sendDataPtr, typeof(FilterAPI.MessageSendData));

            if (FilterAPI.MESSAGE_SEND_VERIFICATION_NUMBER != messageSend.VerificationNumber)
            {
                throw new Exception("Received message corrupted.Please check if the MessageSendData structure is correct.");
            }

            replyData = replyDataPtr;
            messageReply = (FilterAPI.MessageReplyData)Marshal.PtrToStructure(replyDataPtr, typeof(FilterAPI.MessageReplyData));

            string userName = string.Empty;
            string processName = string.Empty;

            Utils.DecodeUserName(messageSend.Sid, out userName);
            Utils.DecodeProcessName(messageSend.ProcessId, out processName);

            UserName = userName;
            ProcessName = processName;
            TagDataLength = (int)messageSend.DataBufferLength;
            TagData = messageSend.DataBuffer;
            MessageId = messageSend.MessageId;
            MessageType = (FilterAPI.MessageType)messageSend.MessageType;
            CreationTime = messageSend.CreationTime;
            LastWriteTime = messageSend.LastWriteTime;
            ProcessId = messageSend.ProcessId;
            ThreadId = messageSend.ThreadId;
            FileName = messageSend.FileName;
            FileSize = messageSend.FileSize;
            Attributes =(FileAttributes)messageSend.FileAttributes;

            ReadOffset = messageSend.Offset;
            ReadLength = messageSend.Length;
         
            messageReply.MessageId = MessageId;
            messageReply.MessageType = (uint)MessageType;            

        }

        /// <summary>
        /// The Message Id.
        /// </summary>
        public uint MessageId { get; set; }
        /// <summary>
        /// The Message Id.
        /// </summary>
        public FilterAPI.MessageType MessageType { get; set; }
        /// <summary>
        /// The length of the reparse point tag data in the stub file..
        /// </summary>
        public int TagDataLength { get; set; }
        /// <summary>
        /// The reparse point tag data in byte array format.
        /// </summary>
        public byte[] TagData { get; set; }
        /// <summary>
        /// The process Id who initiates the IO.
        /// </summary>
        public uint ProcessId { get; set; }
        /// <summary>
        /// The process name who initiates the IO.
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// The thread Id who initiates the IO.
        /// </summary>
        public uint ThreadId { get; set; }
        /// <summary>
        /// The user name who initiates the IO.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// The file name of the file IO.
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// The file size of the file IO.
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// The creation time in UTC of the file.
        /// </summary>
        public long CreationTime { get; set; }
        /// <summary>
        /// The last write time in UTC of the file.
        /// </summary>
        public long LastWriteTime { get; set; }
        /// <summary>
        /// The file attributes of the file IO.
        /// </summary>
        public FileAttributes Attributes { get; set; }
        /// <summary>
        /// The read offset
        /// </summary>
        public long ReadOffset { get; set; }
        /// <summary>
        /// The length of the Read
        /// </summary>
        public uint ReadLength { get; set; }

        FilterAPI.MessageSendData messageSend = new FilterAPI.MessageSendData();
        FilterAPI.MessageReplyData messageReply = new FilterAPI.MessageReplyData();
        IntPtr replyData = IntPtr.Zero;

        uint returnBufferLength = 0;
        string returnCacheFileName = string.Empty;
        FilterAPI.FilterStatus filterStatus = FilterAPI.FilterStatus.BLOCK_DATA_WAS_RETURNED;
        FilterAPI.NTSTATUS returnStatus = FilterAPI.NTSTATUS.STATUS_SUCCESS;

        /// <summary>
        /// The length of the return buffer.
        /// </summary>
        public uint ReturnBufferLength
        {
            get { return returnBufferLength; }
            set
            {
                returnBufferLength = value;

                messageReply.DataBufferLength = value;
                Marshal.StructureToPtr(messageReply, replyData, true);
            }
        }
        /// <summary>
        /// The return buffer.
        /// </summary>
        public byte[] ReturnBuffer { get { return messageReply.DataBuffer; } }

        /// <summary>
        /// The return cache file name.
        /// </summary>
        public string ReturnCacheFileName
        {
            get { return returnCacheFileName; }
            set
            {
                returnCacheFileName = value;
                returnBufferLength = (uint)value.Length * 2;                
                messageReply.DataBufferLength = (uint)value.Length * 2;
                Array.Copy(Encoding.Unicode.GetBytes(value), messageReply.DataBuffer, messageReply.DataBufferLength);

                Marshal.StructureToPtr(messageReply, replyData, true);
            }
        }

        /// <summary>
        /// the return filter status to tell the filter driver how to handle the return buffer.
        /// </summary>
        public FilterAPI.FilterStatus FilterStatus
        {
            get { return filterStatus; }
            set
            {
                filterStatus = value;
                messageReply.FilterStatus = (uint)value;

                Marshal.StructureToPtr(messageReply, replyData, true);
            }
        }

        /// <summary>
        /// the return status of the I/O request.
        /// </summary>
        public FilterAPI.NTSTATUS ReturnStatus
        {
            get { return returnStatus; }
            set
            {
                returnStatus = value;
                messageReply.ReturnStatus = (uint)value;

                Marshal.StructureToPtr(messageReply, replyData, true);

            }
        }
    }


    public class FilterControl 
    {
        //The FilterControl is the component to communicate with the filter driver.
        //With this control, you can start or stop the filter service, setup the global configuration setting, and handle the filter callback request.

        delegate Boolean FilterDelegate(IntPtr sendData, IntPtr replyData);
        delegate void DisconnectDelegate();
        GCHandle gchFilter;
        GCHandle gchDisconnect;

        public static bool IsStarted = false;
        static bool isFilterStarted = false;

        int filterConnectionThreads = 5;
        int connectionTimeout = 30;
        uint booleanConfig = 0;

        List<uint> includeProcessIdList = new List<uint>();
        List<uint> excludeProcessIdList = new List<uint>();

        public static string licenseKey = string.Empty;

        /// <summary>
        /// The global boolean config setting
        /// </summary>
        public uint BooleanConfig
        {
            get { return booleanConfig; }
            set { booleanConfig = value; }
        }

        /// <summary>
        /// The global include process Id list, only IO from the process in the list will be managed by the filter driver.
        /// </summary>
        public List<uint> IncludeProcessIdList
        {
            get { return includeProcessIdList; }
            set { includeProcessIdList = value; }
        }
        /// <summary>
        /// The global exclude process Id list, skip all the IOs from the exclude process id list.
        /// </summary>
        public List<uint> ExcludeProcessIdList
        {
            get { return excludeProcessIdList; }
            set { excludeProcessIdList = value; }
        }

        /// <summary>
        /// if this flag is true, the filter driver will reopen the file when the stub file was rehydrated to bypass the write event for the file monitor driver.
        /// </summary>
        public bool ByPassWriteEventOnReHydration
        {
            get { return ( booleanConfig & (uint)FilterAPI.BooleanConfig.ENABLE_REOPEN_FILE_ON_REHYDRATION) > 0 ; }
            set
            {
                if (value)
                {
                    booleanConfig |= (uint)FilterAPI.BooleanConfig.ENABLE_REOPEN_FILE_ON_REHYDRATION;
                }
                else
                {
                    booleanConfig &= ~(uint)FilterAPI.BooleanConfig.ENABLE_REOPEN_FILE_ON_REHYDRATION;
                }
            }
        }



        /// <summary>
        /// Fires this event when the filter send the request.
        /// </summary>
        public event EventHandler<FilterRequestEventArgs> OnFilterRequest;


        /// <summary>
        /// Start the filter driver service.
        /// </summary>
        /// <param name="filterConnectionThreads"></param>
        /// <param name="connectionTimeout"></param>
        /// <param name="licenseKey"></param>
        /// <param name="lastError"></param>
        /// <returns></returns>
        public bool StartFilter(int _filterConnectionThreads, int _connectionTimeout, string _licenseKey, ref string lastError)
        {

            bool ret = true;
            FilterDelegate filterCallback = new FilterDelegate(FilterRequestHandler);
            DisconnectDelegate disconnectCallback = new DisconnectDelegate(DisconnectCallback);           

            try
            {
                filterConnectionThreads = _filterConnectionThreads;
                connectionTimeout = _connectionTimeout;
                licenseKey = _licenseKey;

                if (Utils.IsDriverChanged())
                {
                    //uninstall or install driver needs the Admin permission.
                    FilterAPI.UnInstallDriver();

                    //wait for 3 seconds for the uninstallation completed.
                    System.Threading.Thread.Sleep(3000);
                }

                if (!FilterAPI.IsDriverServiceRunning())
                {
                    ret = FilterAPI.InstallDriver();
                    if (!ret)
                    {
                        lastError = "Installed driver failed with error:" + FilterAPI.GetLastErrorMessage();
                        return false;
                    }
                }


                if (!isFilterStarted)
                {

                    if (!FilterAPI.SetRegistrationKey(licenseKey))
                    {
                        lastError = "Set license key failed with error:" + FilterAPI.GetLastErrorMessage();
                        return false;
                    }

                    gchFilter = GCHandle.Alloc(filterCallback);
                    IntPtr filterCallbackPtr = Marshal.GetFunctionPointerForDelegate(filterCallback);

                    gchDisconnect = GCHandle.Alloc(disconnectCallback);
                    IntPtr disconnectCallbackPtr = Marshal.GetFunctionPointerForDelegate(disconnectCallback);

                    isFilterStarted = FilterAPI.RegisterMessageCallback(filterConnectionThreads, filterCallbackPtr, disconnectCallbackPtr);
                    if (!isFilterStarted)
                    {
                        lastError = "Connect to the filter driver failed with error:" + FilterAPI.GetLastErrorMessage();
                        return false;
                    }

                    ret = true;

                    IsStarted = true;

                }
            }
            catch (Exception ex)
            {
                ret = false;
                lastError = "Start filter failed with error " + ex.Message;
            }
            finally
            {
                if (!ret)
                {
                    lastError = lastError + " Make sure you run this application as administrator.";
                }
              
            }

            return ret;
        }

        /// <summary>
        ///The filter driver service is started if it is true.
        /// </summary>
        public bool IsFilterStarted
        {
            get { return isFilterStarted; }
        }

        /// <summary>
        /// Stop the filter driver service.
        /// </summary>
        public void StopFilter()
        {
            if (isFilterStarted)
            {
                FilterAPI.Disconnect();
                gchFilter.Free();
                gchDisconnect.Free();
                isFilterStarted = false;
            }

            return;
        }


        /// <summary>
        /// Install the filter driver service.
        /// </summary>
        /// <param name="lastError"></param>
        /// <returns></returns>
        public bool InstallDriver(ref string lastError)
        {
            if (!FilterAPI.InstallDriver())
            {
                lastError = "Installed driver failed with error:" + FilterAPI.GetLastErrorMessage();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Uninstall the filter driver service.
        /// </summary>
        /// <param name="lastError"></param>
        /// <returns></returns>
        public bool UnInstallDriver(ref string lastError)
        {
            if (!FilterAPI.UnInstallDriver())
            {
                lastError = "UnInstallDriver failed with error:" + FilterAPI.GetLastErrorMessage();
                return false;
            }

            return true;
        }

        public bool SendConfigSettingsToFilter(ref string lastError)
        {
            try
            {
                if (!isFilterStarted)
                {
                    lastError = "the filter driver is not started.";
                    return true;
                }

                if (!FilterAPI.ResetConfigData())
                {
                    lastError = "ResetConfigData failed:" + FilterAPI.GetLastErrorMessage();
                    return false;
                }

                if (!FilterAPI.SetConnectionTimeout((uint)connectionTimeout))
                {
                    lastError = "SetConnectionTimeout failed:" + FilterAPI.GetLastErrorMessage();
                    return false;
                }

                if (BooleanConfig > 0 && !FilterAPI.SetBooleanConfig(BooleanConfig))
                {
                    lastError = "SetBooleanConfig " + BooleanConfig + " failed:" + FilterAPI.GetLastErrorMessage();
                    return false;
                }

                foreach (uint includedPid in IncludeProcessIdList)
                {
                    if (!FilterAPI.AddIncludedProcessId(includedPid))
                    {
                        lastError = "AddIncludedProcessId " + includedPid + " failed:" + FilterAPI.GetLastErrorMessage();
                        return false;
                    }
                }

                foreach (uint excludedPid in ExcludeProcessIdList)
                {
                    if (!FilterAPI.AddExcludedProcessId(excludedPid))
                    {
                        lastError = "AddExcludedProcessId " + excludedPid + " failed:" + FilterAPI.GetLastErrorMessage();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                lastError = "Send config settings to filter failed with error " + ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handle the requests from the filter driver, process the notification if no return needed, 
        /// or reply the message back to the filter which the filter driver is wating for.
        /// </summary>
        /// <param name="sendDataPtr"></param>
        /// <param name="replyDataPtr"></param>
        /// <returns></returns>
        Boolean FilterRequestHandler(IntPtr sendDataPtr, IntPtr replyDataPtr)
        {
            try
            {
                FilterRequestEventArgs filterRequestEventArgs = new FilterRequestEventArgs(sendDataPtr, replyDataPtr);
                OnFilterRequest(null, filterRequestEventArgs);
            }
            catch (Exception ex)
            {
                throw new Exception("FilterRequestHandler exception." + ex.Message);
            }

            return true;
        }

        void DisconnectCallback()
        {
            return;
        }
    }
}
