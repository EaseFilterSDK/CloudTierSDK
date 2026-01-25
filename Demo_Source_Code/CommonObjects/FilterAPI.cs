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
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Cryptography;

namespace EaseFilter.CommonObjects
{

    static public class FilterAPI
    {
        public delegate Boolean FilterDelegate(IntPtr sendData, IntPtr replyData);
        public delegate void DisconnectDelegate();
        static GCHandle gchFilter;
        static GCHandle gchDisconnect;
        public static bool isFilterStarted = false;
        public const int MAX_FILE_NAME_LENGTH = 1024;
        public const int MAX_SID_LENGTH = 256;
        public const int MAX_MESSAGE_LENGTH = 65536;
        public const int MAX_PATH = 260;
        public const int MAX_ERROR_MESSAGE_SIZE = 1024;

        public const uint MESSAGE_SEND_VERIFICATION_NUMBER = 0xFF000001;

        //the key represent the tag data is using the following structure
        public const uint REPARSETAG_KEY = 0xbba65d6f;

        public static string licenseKey = string.Empty;

        public struct REPARSETAG_DATA 
        {
	        public uint		ReparseTagKey;
	        public uint		Flags;
	        public uint		FileNameLength;
	      //  public string	fileName;
        }

        /// <summary>
        /// the message type of the filter driver send request 
        /// </summary>
        public enum MessageType : uint
        {
            /// <summary>
            /// This message type indicates you can restore the full content of the file, 
            /// or restore the request block of data from the offset and length
            /// </summary>
            MESSAGE_TYPE_RESTORE_BLOCK_OR_FILE = 0x00000001,

            /// <summary>
            /// This message type indicates you have to restore the full content of the file.
            /// </summary>
            MESSAGE_TYPE_RESTORE_FILE = 0x00000002,

            /// <summary>
            /// require to download the whole file to the cache folder,and return cache file name to filter
            /// </summary>
            MESSAGE_TYPE_RESTORE_FILE_TO_CACHE = 0x00000008,

            /// <summary>
            /// This is the notification event of the file, it doesn't need to reply the request.
            /// </summary>
            MESSAGE_TYPE_SEND_EVENT_NOTIFICATION = 0x00000010,
        }

        public enum NTSTATUS : uint
        {
            STATUS_SUCCESS = 0,
            STATUS_UNSUCCESSFUL = 0xc0000001,
        }

        public enum EVENTTYPE : uint
        {
            CREATEED = 0x00000020,
            CHANGED = 0x00000040,
            RENAMED = 0x00000080,
            DELETED = 0x00000100,
        }

        public enum BooleanConfig : uint
        {
            ENABLE_NO_RECALL_FLAG = 0x00000001, //for easetag, if it was true, after the reparsepoint file was opened, it won't restore data back for read and write.
            DISABLE_FILTER_UNLOAD_FLAG = 0x00000002, //if it is true, the filter driver can't be unloaded.
            ENABLE_REOPEN_FILE_ON_REHYDRATION = 0x00000400, //if it is enabled, it will reopen the file when rehydration of the stub file.

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MessageSendData
        {
            public uint MessageId;          //this is the request sequential number.
            public IntPtr FileObject;       //the address of FileObject,it is equivalent to file handle,it is unique per file stream open.
            public IntPtr FsContext;        //the address of FsContext,it is unique per file.
            public uint MessageType;        //the I/O request type.
            public uint ProcessId;          //the process ID for the process associated with the thread that originally requested the I/O operation.
            public uint ThreadId;           //the thread ID which requested the I/O operation.
            public long Offset;             //the read/write offset.
            public uint Length;             //the read/write length.
            public long FileSize;           //the size of the file for the I/O operation.
            public long TransactionTime;    //the transaction time in UTC of this request.
            public long CreationTime;       //the creation time in UTC of the file.
            public long LastAccessTime;     //the last access time in UTC of the file.
            public long LastWriteTime;      //the last write time in UTC of the file.
            public uint FileAttributes;     //the file attributes.
            public uint DesiredAccess;      //the DesiredAccess for file open, please reference CreateFile windows API.
            public uint Disposition;        //the Disposition for file open, please reference CreateFile windows API.
            public uint SharedAccess;       //the SharedAccess for file open, please reference CreateFile windows API.
            public uint CreateOptions;      //the CreateOptions for file open, please reference CreateFile windows API.
            public uint CreateStatus;       //the CreateStatus after file was openned, please reference CreateFile windows API.
            public uint InfoClass;          //the information class or security information
            public uint Status;             //the I/O status which returned from file system.
            public uint FileNameLength;     //the file name length in byte.
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FILE_NAME_LENGTH)]
            public string FileName;         //the file name of the I/O operation.
            public uint SidLength;          //the length of the security identifier.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_SID_LENGTH)]
            public byte[] Sid;              //the security identifier data.
            public uint DataBufferLength;   //the data buffer length.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_MESSAGE_LENGTH)]
            public byte[] DataBuffer;       //the data buffer which contains read/write/query information/set information data.
            public uint VerificationNumber; //the verification number which verifiys the data structure integerity.
        }

        public enum FilterStatus : uint
        {
            BLOCK_DATA_WAS_RETURNED = 0x00000008, //Set this flag if return read block databuffer to filter.
            CACHE_FILE_WAS_RETURNED = 0x00000010, //Set this flag if the whole cache file was downloaded.
            REHYDRATE_FILE_VIA_CACHE_FILE = 0x00000020, //Set this flag if the whole cache file was downloaded and you want to rehydrate the file from the cache file.
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MessageReplyData
        {
            public uint MessageId;
            public uint MessageType;
            public uint ReturnStatus;
            public uint FilterStatus;
            public uint DataBufferLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 65536)]
            public byte[] DataBuffer;
        }

        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool AddFilterRule(
         uint accessFlag,
        [MarshalAs(UnmanagedType.LPWStr)]string filterMask,
        [MarshalAs(UnmanagedType.LPWStr)] string reparseMask);

        /// <summary>
        /// Monitor the create/delete/rename event of the files under the register folder
        /// </summary>
        public static bool RegisterEvent(
         uint eventType,
         string folder)
        {
            if (!folder.EndsWith("\\*"))
            {
                folder += "\\*";
            }

            return AddFilterRule(eventType, folder, "");
        }

        /// <summary>
        /// set the filter driver boolean config setting based on the enum booleanConfig
        /// </summary>
        /// <param name="booleanConfig"></param>
        /// <returns></returns>
        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool SetBooleanConfig(uint booleanConfig);

        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool InstallDriver();

        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool UnInstallDriver();

        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool IsDriverServiceRunning();


        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool SetRegistrationKey([MarshalAs(UnmanagedType.LPStr)]string key);

        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool Disconnect();

        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool GetLastErrorMessage(
            [MarshalAs(UnmanagedType.LPWStr)] 
            string errorMessage,
            ref int messageLength);

        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool RegisterMessageCallback(
            int threadCount,
            IntPtr filterCallback,
            IntPtr disconnectCallback);

        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool ResetConfigData();

        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool SetConnectionTimeout(uint timeOutInSeconds);

        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool SetMaxWaitingRequestCount(uint maxWaitingRequestCount);

        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool AddIncludedProcessId(uint processId);

        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool AddExcludedProcessId(uint processId);

        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool RemoveExcludeProcessId(uint processId);

  
        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool GetFileHandleInFilter(
             [MarshalAs(UnmanagedType.LPWStr)]string fileName,
             FileAccess fileAccess,
             ref IntPtr fileHandle);

        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool OpenStubFile(
             [MarshalAs(UnmanagedType.LPWStr)]string fileName,
              FileAccess access,
              FileShare share,
              ref IntPtr fileHandle);

        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool CreateStubFile(
             [MarshalAs(UnmanagedType.LPWStr)]string fileName,
             long fileSize,  //if it is 0 and the file exist,it will use the current file size.
              uint fileAttributes, //if it is 0 and the file exist, it will use the current file attributes.
              uint tagDataLength,
              IntPtr tagData,
              bool overwriteIfExist,
              ref IntPtr fileHandle);

        /// <summary>
        /// Create stub file.
        /// </summary>
        /// <param name="fileName">the stub file name to be created</param>
        /// <param name="fileSize">if it is 0 and the file exist,it will use the current file size.</param>
        /// <param name="fileAttributes">if it is 0 and the file exist, it will use the current file attributes.</param>
        /// <param name="tagDataLength">the length of the reparse point tag data</param>
        /// <param name="tagData">the reparse point tag data</param>
        /// <param name="creationTime">set the creation time of the stub file if it is not 0</param>
        /// <param name="lastWriteTime">set the last write time of the stub file if it is not 0</param>
        /// <param name="lastAccessTime">set the last access time of the stub file if it is not 0</param>
        /// <param name="overwriteIfExist">overwrite the existing file if it is true and the file exist. </param>
        /// <param name="fileHandle">the return file handle of the stub file</param>
        /// <returns>return true if the stub file was created successfully.</returns>
        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool CreateStubFileEx(
             [MarshalAs(UnmanagedType.LPWStr)]string fileName,
             long fileSize,
              uint fileAttributes,
              uint tagDataLength,
              IntPtr tagData,
              long creationTime,
              long lastWriteTime,
              long lastAccessTime,
              bool overwriteIfExist,
              ref IntPtr fileHandle);


        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool GetTagData(
              IntPtr fileHandle,
              ref int tagDataLength,
              IntPtr tagData);

        public static bool GetTagData(IntPtr fileHandle, out byte[] tagData, out string lastError)
        {
            bool ret = false;
            IntPtr tagPtr = IntPtr.Zero;
            int tagDataLength = 0;

            lastError = string.Empty;
            tagData = null;

            ret = GetTagData(fileHandle, ref tagDataLength, tagPtr);

            if (!ret)
            {
                if (tagDataLength > 0)
                {
                    tagPtr = Marshal.AllocHGlobal((int)tagDataLength);
                    ret = GetTagData(fileHandle, ref tagDataLength, tagPtr);

                }
            }

            if (!ret)
            {
                lastError = GetLastErrorMessage();
            }
            else
            {
                tagData = new byte[tagDataLength];
                Marshal.Copy(tagPtr, tagData, 0, tagDataLength);
            }

            if (tagPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(tagPtr);
            }

            return ret;
        }

        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool RemoveTagData(
              IntPtr fileHandle);


        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool AddTagData(
              IntPtr fileHandle,
              int tagDataLength,
              IntPtr tagData);




        public static bool AddTagData(
              IntPtr fileHandle,
              byte[] tagData)
           {
               GCHandle pinnedArray = GCHandle.Alloc(tagData, GCHandleType.Pinned);
                IntPtr pointer = pinnedArray.AddrOfPinnedObject();

                bool ret = AddTagData(fileHandle, tagData.Length, pointer);

                pinnedArray.Free();


               return ret;

           }
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool ConvertSidToStringSid(
            [In] IntPtr sid,
            [Out] out IntPtr sidString);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalFree(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        public static string GetLastErrorMessage()
        {
            int len = MAX_ERROR_MESSAGE_SIZE;
            string errorMessage = new string((char)0, len);

            if (!GetLastErrorMessage(errorMessage, ref len))
            {
                errorMessage = new string((char)0, len);
                if (!GetLastErrorMessage(errorMessage, ref len))
                {
                    return "failed to get last error message.";
                }
            }

            return errorMessage;
        }



        public static bool DecodeUserInfo(MessageSendData messageSend, out string userName, out string processName)
        {
            bool ret = true;

            IntPtr sidStringPtr = IntPtr.Zero;
            string sidString = string.Empty;

            userName = string.Empty;
            processName = string.Empty;

            try
            {
                IntPtr sidBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(messageSend.Sid, 0);

                if (ConvertSidToStringSid(sidBuffer, out sidStringPtr))
                {
                    sidString = Marshal.PtrToStringAuto(sidStringPtr);
                    SecurityIdentifier secIdentifier = new SecurityIdentifier(sidString);
                    IdentityReference reference = secIdentifier.Translate(typeof(NTAccount));
                    userName = reference.Value;
                }
                else
                {
                    string errorMessage = "Convert sid to sid string failed with error " + Marshal.GetLastWin32Error();
                    Console.WriteLine(errorMessage);
                }

                System.Diagnostics.Process requestProcess = System.Diagnostics.Process.GetProcessById((int)messageSend.ProcessId);
                processName = requestProcess.ProcessName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Convert sid to user name got exception:{0}", ex.Message));
                ret = false;

            }
            finally
            {
                if (sidStringPtr != null && sidStringPtr != IntPtr.Zero)
                {
                    LocalFree(sidStringPtr);
                }
            }

            return ret;
        }


        static public bool StartFilter(string registerKey, int threadCount, FilterDelegate filterCallback, DisconnectDelegate disconnectCallback, ref string lastError)
        {
            bool ret = true;

            try
            {
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
                    if (!SetRegistrationKey(registerKey))
                    {
                        lastError = "Set registration key failed with error:" + GetLastErrorMessage();
                        return false;
                    }

                    licenseKey = registerKey;

                    gchFilter = GCHandle.Alloc(filterCallback);
                    IntPtr filterCallbackPtr = Marshal.GetFunctionPointerForDelegate(filterCallback);

                    gchDisconnect = GCHandle.Alloc(disconnectCallback);
                    IntPtr disconnectCallbackPtr = Marshal.GetFunctionPointerForDelegate(disconnectCallback);

                    isFilterStarted = RegisterMessageCallback(threadCount, filterCallbackPtr, disconnectCallbackPtr);
                    if (!isFilterStarted)
                    {
                        lastError = "SRegisterMessageCallback failed with error:" + GetLastErrorMessage();
                        return false;
                    }

                    ret = true;
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


        static public void StopFilter()
        {
            if (isFilterStarted)
            {
                Disconnect();
                gchFilter.Free();
                gchDisconnect.Free();
                isFilterStarted = false;
            }

            return;
        }

        static public bool IsFilterStarted
        {
            get { return isFilterStarted; }
        }

    }
}
