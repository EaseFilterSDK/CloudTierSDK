///////////////////////////////////////////////////////////////////////////////
//
//    (C) Copyright 2012 EaseFilter
//    All Rights Reserved
//
//    This software is part of a licensed software product and may
//    only be used or copied in accordance with the terms of that license.
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace CloudTier.FilterControl
{
    public class Utils
    {

        static Dictionary<string, string> userNameTable = new Dictionary<string, string>();
        static Dictionary<uint, string> processNameTable = new Dictionary<uint, string>();

        [DllImport("FilterAPI.dll", SetLastError = true)]
        public static extern bool AESEncryptDecryptBuffer(
                IntPtr inputBuffer,
                IntPtr outputBuffer,
                uint bufferLength,
                long offset,
                byte[] encryptionKey,
                uint keyLength,
                byte[] iv,
                uint ivLength);

        public static uint WinMajorVersion()
        {
            dynamic major;
            // The 'CurrentMajorVersionNumber' string value in the CurrentVersion key is new for Windows 10, 
            // and will most likely (hopefully) be there for some time before MS decides to change this - again...
            if (TryGeRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMajorVersionNumber", out major))
            {
                return (uint)major;
            }

            // When the 'CurrentMajorVersionNumber' value is not present we fallback to reading the previous key used for this: 'CurrentVersion'
            dynamic version;
            if (!TryGeRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", out version))
                return 0;

            var versionParts = ((string)version).Split('.');
            if (versionParts.Length != 2) return 0;

            uint majorAsUInt;
            return uint.TryParse(versionParts[0], out majorAsUInt) ? majorAsUInt : 0;
        }

        /// <summary>
        ///     Returns the Windows minor version number for this computer.
        /// </summary>
        public static uint WinMinorVersion()
        {
            dynamic minor;
            // The 'CurrentMinorVersionNumber' string value in the CurrentVersion key is new for Windows 10, 
            // and will most likely (hopefully) be there for some time before MS decides to change this - again...
            if (TryGeRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMinorVersionNumber",
                out minor))
            {
                return (uint)minor;
            }

            // When the 'CurrentMinorVersionNumber' value is not present we fallback to reading the previous key used for this: 'CurrentVersion'
            dynamic version;
            if (!TryGeRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", out version))
                return 0;

            var versionParts = ((string)version).Split('.');
            if (versionParts.Length != 2) return 0;
            uint minorAsUInt;
            return uint.TryParse(versionParts[1], out minorAsUInt) ? minorAsUInt : 0;
        }


        private static bool TryGeRegistryKey(string path, string key, out dynamic value)
        {
            value = null;
            try
            {
                var rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(path);
                if (rk == null) return false;
                value = rk.GetValue(key);
                return value != null;
            }
            catch
            {
                return false;
            }
        }

        public static bool DecodeUserName(byte[] sid, out string userName)
        {
            bool ret = true;

            IntPtr sidStringPtr = IntPtr.Zero;
            string sidString = string.Empty;

            userName = string.Empty;

            try
            {

                IntPtr sidBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(sid, 0);

                if (FilterAPI.ConvertSidToStringSid(sidBuffer, out sidStringPtr))
                {

                    sidString = Marshal.PtrToStringAuto(sidStringPtr);

                    lock (userNameTable)
                    {
                        //check the user name cache table
                        if (userNameTable.ContainsKey(sidString))
                        {
                            userName = userNameTable[sidString];
                            return ret;
                        }
                    }

                    try
                    {
                        SecurityIdentifier secIdentifier = new SecurityIdentifier(sidString);
                        IdentityReference reference = secIdentifier.Translate(typeof(NTAccount));
                        userName = reference.Value;
                    }
                    catch
                    {
                    }

                    lock (userNameTable)
                    {
                        //check the user name cache table
                        if (!userNameTable.ContainsKey(sidString))
                        {
                            userNameTable.Add(sidString, userName);
                        }
                    }
                }
                else
                {
                    string errorMessage = "Convert sid to sid string failed with error " + Marshal.GetLastWin32Error();
                    Console.WriteLine(errorMessage);
                }
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
                    FilterAPI.LocalFree(sidStringPtr);
                }
            }

            return ret;
        }

        public static bool DecodeProcessName(uint processId, out string processName)
        {
            bool ret = true;
            processName = string.Empty;


            //this is the optimization of the process to get the process name from the process Id
            //it is not reliable for this process, since the process Id is reuasble when the process was ternmiated.
            lock (processNameTable)
            {
                if (processNameTable.ContainsKey(processId))
                {
                    processName = processNameTable[processId];
                    return true;
                }
            }

            try
            {
                System.Diagnostics.Process requestProcess = System.Diagnostics.Process.GetProcessById((int)processId);
                processName = requestProcess.ProcessName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Convert pid to process name got exception:{0}", ex.Message));
                ret = false;
            }

            lock (processNameTable)
            {
                if (!processNameTable.ContainsKey(processId))
                {
                    processNameTable.Add(processId, processName);
                }
            }

            return ret;
        }


        public static bool IsDriverChanged()
        {
            bool ret = false;

            try
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
                string localPath = Path.GetDirectoryName(assembly.Location);
                string driverName = Path.Combine(localPath, "CloudTier.sys");

                if (File.Exists(driverName))
                {
                    string driverInstalledPath = Path.Combine(Environment.SystemDirectory, "drivers\\cloudtier.sys");

                    if (File.Exists(driverInstalledPath))
                    {
                        FileInfo fsInstalled = new FileInfo(driverInstalledPath);
                        FileInfo fsToInstall = new FileInfo(driverName);

                        if (fsInstalled.LastWriteTime != fsToInstall.LastWriteTime)
                        {
                            return true;
                        }
                    }

                }

            }
            catch
            {
                ret = false;
            }

            return ret;
        }

        public static void CopyOSPlatformDependentFiles(ref string lastError)
        {
            Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
            string localPath = Path.GetDirectoryName(assembly.Location);
            string targetName = Path.Combine(localPath, "FilterAPI.DLL");

            bool is64BitOperatingSystem = Environment.Is64BitOperatingSystem;
            uint winMajorVersion = WinMajorVersion();
            lastError = string.Empty;

            string sourceFolder = localPath;

            try
            {

                if (is64BitOperatingSystem)
                {
                    sourceFolder = Path.Combine(localPath, "Bin\\x64");
                }
                else
                {
                    sourceFolder = Path.Combine(localPath, "Bin\\win32");
                }

                string sourceFile = Path.Combine(sourceFolder, "FilterAPI.DLL");

                //only copy files for x86 platform, by default for x64, the files were there already.

                bool skipCopy = false;
                if (File.Exists(targetName))
                {
                    FileInfo sourceFileInfo = new FileInfo(sourceFile);
                    FileInfo targetFileInfo = new FileInfo(targetName);

                    if (sourceFileInfo.LastWriteTime.ToFileTime() == targetFileInfo.LastWriteTime.ToFileTime())
                    {
                        skipCopy = true;
                    }
                }

                if (!skipCopy)
                {
                    File.Copy(sourceFile, targetName, true);
                }


                sourceFile = Path.Combine(sourceFolder, "CloudTier.sys");
                targetName = Path.Combine(localPath, "CloudTier.sys");


                skipCopy = false;
                if (File.Exists(targetName))
                {
                    FileInfo sourceFileInfo = new FileInfo(sourceFile);
                    FileInfo targetFileInfo = new FileInfo(targetName);

                    if (sourceFileInfo.LastWriteTime.ToFileTime() == targetFileInfo.LastWriteTime.ToFileTime())
                    {
                        skipCopy = true;
                    }
                }

                if (!skipCopy)
                {
                    File.Copy(sourceFile, targetName, true);
                }

            }
            catch (Exception ex)
            {
                lastError = "Copy platform dependent files 'FilterAPI.DLL' and 'CloudTier.sys' to folder " + localPath + " got exception:" + ex.Message;
            }
        }

        public static string FormatTime(long milliseconds)
        {
            string ret = string.Empty;
            long sec = milliseconds / 1000;

            long day = sec / 60 / 60 / 24;

            if (day > 0)
            {
                ret = day.ToString() + " d ";
            }

            long hour = 0;
            Math.DivRem(sec / (60 * 60), 24, out hour);

            if (hour > 0)
            {
                ret = hour.ToString() + " h ";
            }

            long min = 0;
            Math.DivRem(sec / 60, 60, out min);

            if (min > 0)
            {
                ret += min + " m ";
            }

            Math.DivRem(sec, 60, out sec);

            if (sec > 0)
            {
                ret += sec + " s";
            }

            return ret;
        }

        public static string ByteArrayToHexStr(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString().ToUpper();
        }

        public static byte[] ConvertHexStrToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format("The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] HexAsBytes = new byte[hexString.Length / 2];
            for (int index = 0; index < HexAsBytes.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                HexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return HexAsBytes;
        }

        /// <summary>
        /// Generate 32 bytes key array by pass phrase string
        /// </summary>
        /// <param name="pwStr"></param>
        /// <returns></returns>
        public static byte[] GetKeyByPassPhrase(string pwStr)
        {
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] passwordBytes = Encoding.UTF8.GetBytes(pwStr);

            var rfckey = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
            byte[] key = rfckey.GetBytes(32);

            return key;


        }

        public static byte[] GetRandomKey()
        {
            AesManaged aesManaged = new AesManaged();
            aesManaged.KeySize = 256;
            byte[] key = aesManaged.Key;

            return key;
        }

        public static byte[] GetRandomIV()
        {
            AesManaged aesManaged = new AesManaged();
            byte[] IV = aesManaged.IV;

            return IV;
        }

        public static bool IsBase64String(string s)
        {
            s = s.Trim();

            if ((s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static void ToDebugger(string message)
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(false);
            string caller = st.GetFrame(1).GetMethod().Name;
            System.Diagnostics.Debug.WriteLine(caller + " Time:" + DateTime.Now.ToLongTimeString() + ": " + message);
        }

        public enum EncryptType
        {
            Decryption = 0,
            Encryption,
        }

        public static string AESEncryptDecryptStr(string inStr, EncryptType encryptType)
        {

            if (string.IsNullOrEmpty(inStr))
            {
                return string.Empty;
            }

            byte[] inbuffer = null;

            if (encryptType == EncryptType.Encryption)
            {
                inbuffer = ASCIIEncoding.UTF8.GetBytes(inStr);
            }
            else if (encryptType == EncryptType.Decryption)
            {
                inbuffer = Convert.FromBase64String(inStr);
            }
            else
            {
                throw new Exception("Failed to encrypt decrypt string, the encryptType " + encryptType.ToString() + " doesn't know.");
            }

            byte[] outBuffer = new byte[inbuffer.Length];

            GCHandle gcHandleIn = GCHandle.Alloc(inbuffer, GCHandleType.Pinned);
            GCHandle gcHandleOut = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

            IntPtr inBufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(inbuffer, 0);
            IntPtr outBufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(outBuffer, 0);

            try
            {
                bool retVal = AESEncryptDecryptBuffer(inBufferPtr, outBufferPtr, (uint)inbuffer.Length, 0, null, 0, null, 0);

                if (encryptType == EncryptType.Encryption)
                {
                    return Convert.ToBase64String(outBuffer);
                }
                else //if (encryptType == EncryptType.Decryption)
                {
                    return ASCIIEncoding.UTF8.GetString(outBuffer);
                }
            }
            finally
            {
                gcHandleIn.Free();
                gcHandleOut.Free();
            }

        }


        public static void AESEncryptDecryptBuffer(byte[] inbuffer, long offset, byte[] key, byte[] IV)
        {
            if (null == inbuffer || inbuffer.Length == 0)
            {
                throw new Exception("Failed to encrypt decrypt buffer, the input buffer can't be null");
            }

            GCHandle gcHandle = GCHandle.Alloc(inbuffer, GCHandleType.Pinned);

            try
            {
                IntPtr inBufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(inbuffer, 0);

                uint keyLength = 0;
                uint IVLength = 0;

                if (key != null)
                {
                    keyLength = (uint)key.Length;
                }

                if (IV != null)
                {
                    IVLength = (uint)IV.Length;
                }


                bool retVal = AESEncryptDecryptBuffer(inBufferPtr, inBufferPtr, (uint)inbuffer.Length, offset, key, keyLength, IV, IVLength);

                if (!retVal)
                {
                    throw new Exception("Failed to encrypt buffer, return error:" + FilterAPI.GetLastErrorMessage());
                }
            }
            finally
            {
                gcHandle.Free();
            }

            return;
        }

    }
}
