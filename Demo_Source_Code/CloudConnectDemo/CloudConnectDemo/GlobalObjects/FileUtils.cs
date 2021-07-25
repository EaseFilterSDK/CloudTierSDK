
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
using System.IO;
using System.Text;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Reflection;
using System.Text.RegularExpressions;


namespace EaseFilter.GlobalObjects
{
    public class FileUtils
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool ConvertSidToStringSid(
            [In] IntPtr sid,
            [Out] out IntPtr sidString);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalFree(IntPtr hMem);

        [DllImport("kernel32", SetLastError = true)]
        public static extern uint GetCurrentProcessId();

        [DllImport("FilterAPI.dll", SetLastError = true)]
        private static extern bool CreateFileAPI(
             [MarshalAs(UnmanagedType.LPWStr)]string fileName,
              uint dwDesiredAccess,
              uint dwShareMode,
              uint dwCreationDisposition,
              uint dwFlagsAndAttributes,
              ref IntPtr fileHandle);

        public static bool IsBase64String(string s)
        {
            s = s.Trim();

            if (s.Length > 0 && (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool IsRightPlatformAssembly(string binPath)
        {
            bool ret = false;

            Assembly assembly = Assembly.ReflectionOnlyLoadFrom(binPath);
            PortableExecutableKinds kinds;
            ImageFileMachine imgFileMachine;

            assembly.ManifestModule.GetPEKind(out kinds, out imgFileMachine);
            bool is64BitOperatingSystem = Environment.Is64BitOperatingSystem;

             return ret;
        }

        /// <summary>
        /// for win32 and x64 platform, copy the right dlls to the folder.
        /// </summary>
        public static void CopyOSPlatformDependentFiles()
        {
            Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
            string localPath = Path.GetDirectoryName(assembly.Location);
            string targetName = Path.Combine(localPath, "FilterAPI.dll");

            bool is64BitOperatingSystem = Environment.Is64BitOperatingSystem;
            string sourceFolder = localPath;

            try
            {

                if (is64BitOperatingSystem)
                {
                    sourceFolder = Path.Combine(localPath, "x64");
                }
                else
                {
                    sourceFolder = Path.Combine(localPath, "win32");
                }

                string sourceFile = Path.Combine(sourceFolder, "FilterAPI.dll");

                //only copy files for x86 platform, by default for x64, the files were there already.

                if (!is64BitOperatingSystem)
                {
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


                    sourceFile = Path.Combine(sourceFolder, "FilterAPI.sys");
                    targetName = Path.Combine(localPath, "FilterAPI.sys");


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
            }
            catch (Exception ex)
            {
                string lastError = "Copy platform dependent files 'FilterAPI.dll' and 'EaseClouds.sys' to folder " + localPath + " got exception:" + ex.Message;
                EventManager.WriteMessage(80, "CopyOSPlatformDependentFiles", EventLevel.Error, lastError);
            }
        }

        private static bool  DeleteFiles(string folder)
        {
            bool retVal = true;

            try
            {
                string[] subDirs = Directory.GetDirectories(folder);

                foreach (string dir in subDirs)
                {
                    DeleteFiles(dir);
                }

                string[] files = Directory.GetFiles(folder);

                foreach (string file in files)
                {
                    try
                    {
                        File.Delete(file);
                        EventManager.WriteMessage(150, "DeleteFiles", EventLevel.Verbose, "Delete file " + file + " in folder " + folder + " succeeded.");
                    }
                    catch (Exception ex)
                    {
                        retVal = false;
                        EventManager.WriteMessage(149, "DeleteFiles", EventLevel.Verbose, "Delete file " + file + " in folder " + folder + " failed with error " + ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                retVal = false;
                EventManager.WriteMessage(135, "DeleteFiles", EventLevel.Verbose, "DeleteFiles in folder " + folder + " failed with error " + ex.Message);
            }

            return retVal;
        }

        public static bool ClearCachedFiles()
        {
            bool retVal = true;

            try
            {
                string cacheFolder = GlobalConfig.CloudCacheFolder;
                retVal = DeleteFiles(cacheFolder);

            }
            catch (Exception ex)
            {
                retVal = false;
                EventManager.WriteMessage(136, "ClearCacheFiles", EventLevel.Error, "Clear cache file failed with error " + ex.Message );
            }

            return retVal;
        }

        private static void DeleteExpiredCachedFiles(string folder)
        {
            try
            {
                string[] subDirs = Directory.GetDirectories(folder);

                foreach (string dir in subDirs)
                {
                    DeleteExpiredCachedFiles(dir);
                }

                string[] files = Directory.GetFiles(folder);

                int expireCachedDirListingSeconds = GlobalConfig.ExpireCachedDirectoryListingAfterSeconds;
                int deleteCachedFileSeconds = GlobalConfig.DeleteCachedFilesAfterSeconds;
                string dirListingName = GlobalConfig.DirInfoListName;

                bool deleteFolderNeeded = true;

                foreach (string file in files)
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        TimeSpan timeSpan = DateTime.Now - fileInfo.LastWriteTime;

                        if (string.Compare(dirListingName, Path.GetFileName(file)) == 0)
                        {
                            //this is the directory listing file
                            if (timeSpan.TotalSeconds > expireCachedDirListingSeconds)
                            {
                                File.Delete(file);
                            }
                            else
                            {
                                deleteFolderNeeded = false;
                            }
                        }
                        else
                        {
                            if (timeSpan.TotalSeconds > deleteCachedFileSeconds)
                            {
                                File.Delete(file);
                            }
                            else
                            {
                                deleteFolderNeeded = false;
                            }
                        }

                        
                    }
                    catch (Exception ex)
                    {
                        EventManager.WriteMessage(243, "DeleteExpiredCachedFiles", EventLevel.Verbose, "Delete file " + file + " in folder " + folder + " failed with error " + ex.Message);
                    }
                }

                if (deleteFolderNeeded && subDirs.Length == 0 )
                {
                    Directory.Delete(folder);
                }

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(255, "DeleteExpiredCachedFiles", EventLevel.Verbose, "DeleteExpiredCachedFiles in folder " + folder + " failed with error " + ex.Message);
            }
        }

        public static void DeleteExpiredCachedFiles(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                string cacheFolder = GlobalConfig.CloudCacheFolder;
                DeleteExpiredCachedFiles(cacheFolder);

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(136, "ClearCacheFiles", EventLevel.Error, "Clear cache file failed with error " + ex.Message);
            }
        }

     
    }

}
