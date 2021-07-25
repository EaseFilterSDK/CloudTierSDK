
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
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;

namespace EaseFilter.GlobalObjects
{

    public struct FileEntry
    {
        public uint EntryLength;
        public uint Flags;        
        public uint FileAttributes;
        public long FileId;
        public long FileSize;
        public long CreationTime;
        public long LastAccessTime;
        public long LastWriteTime;
        public uint TagDataLength;
        public uint FileNameLength;
        public string FileName;
        public byte[] TagData;
    }

    public class DirectoryList :IDisposable
    {
        public const int MAX_PATH = 260;

        private Dictionary<string, FileEntry> dirFileList = null;
        private string directoryName = string.Empty;
        private string cacheDirName = string.Empty;
        private string cacheFolder = GlobalConfig.CloudCacheFolder;
        private string mappingFolder = string.Empty;
        private long currentFileId = 0;

        /// <summary>
        /// if this is true, it will discard the cached diectory listing, and download it again.
        /// </summary>
        private bool forceDownload = false;

        private uint FILE_ENTRY_STRUCT_SIZE = 4/*entryLength*/ + 4/*flags*/ + 4/*FileAttributes*/ + 8/*Index*/+ 8/*FileSize*/ + 8/*creationTime*/
                                              + 8/*LastAccessTime*/ + 8/*LastWriteTime*/  + 4/*TagDataLength*/ + 4/*FileNameLength*/ ;

        public DirectoryList()
        {
        }
        public DirectoryList(string dirName, SiteInfo siteInfo, bool downloadNeeded)
        {
            directoryName = dirName;
            forceDownload = downloadNeeded;

            cacheDirName = GetCacheFileNameByFolderName(dirName, siteInfo);
            this.dirFileList = new Dictionary<string, FileEntry>();

        }

        private void Dispose(Boolean freeManagedObjectsAlso)
        {
            if (freeManagedObjectsAlso)
            {
                dirFileList.Clear();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~DirectoryList()
        {
            Dispose(false);
        }

        public string GetCacheFileNameByFolderName(string directoryName, SiteInfo siteInfo)
        {
            
            string cacheFolder = GlobalConfig.CloudCacheFolder;
            string mappingFolder = siteInfo.LocalPath.ToLower();

            string cacheDirName = Path.Combine(cacheFolder, siteInfo.CloudProvider.ToString());
            cacheDirName = Path.Combine(cacheDirName, siteInfo.SiteName);

            if (mappingFolder.Length > 0)
            {
                if (!directoryName.ToLower().StartsWith(mappingFolder, StringComparison.CurrentCultureIgnoreCase))
                {
                    EventManager.WriteMessage(185, "GetCacheFileNameByFolderName", EventLevel.Error, "Folder name " + directoryName + " doesn't match the mapping folder " + mappingFolder);
                    return string.Empty;
                }
                else
                {

                    string folder = directoryName.Substring(mappingFolder.Length);
                    if (folder.StartsWith("\\"))
                    {
                        folder = folder.Substring(1);
                    }

                    cacheDirName = Path.Combine(cacheDirName, folder);
                }
            }
            else
            {
                string folder = directoryName;
                if (folder.StartsWith("\\"))
                {
                    folder = folder.Substring(1);
                }

                cacheDirName = Path.Combine(cacheDirName, folder);
            }


            if (!Directory.Exists(cacheDirName))
            {
                Directory.CreateDirectory(cacheDirName);
            }

            cacheDirName = Path.Combine(cacheDirName, GlobalConfig.DirInfoListName);

            return cacheDirName;
        }

      
      
        public void AddFileEntry(string fileName,byte[] tagData, long creationTime, long lastAccessTime, long lastWriteTime, long fileSize, uint fileAttributes)
        {
            fileName = fileName.Replace("%20", " ");

            //check if the file entry already exist.
            if( dirFileList.ContainsKey(fileName.ToLower()))
            {
                dirFileList.Remove(fileName.ToLower());
            }

             FileEntry fileEntry = new FileEntry();

             if (null != tagData)
             {
                 fileEntry.TagDataLength = (uint)tagData.Length;
             }
             else
             {
                 fileEntry.TagDataLength = 0;
             }

             uint EntryLength = FILE_ENTRY_STRUCT_SIZE + (uint)fileName.Length * 2 + (uint)fileEntry.TagDataLength;

            fileEntry.EntryLength = EntryLength;
            fileEntry.Flags = 0;
            fileEntry.FileId = ++currentFileId;
            fileEntry.FileAttributes = fileAttributes;
            fileEntry.FileName = fileName;
            fileEntry.FileNameLength = (uint)fileName.Length * 2;
            fileEntry.FileSize = fileSize;
            fileEntry.CreationTime = creationTime;
            fileEntry.LastAccessTime = lastAccessTime;
            fileEntry.LastWriteTime = lastWriteTime;

          
            fileEntry.TagData = tagData;
            dirFileList.Add(fileName.ToLower(),fileEntry);

            return;
        }

        /// <summary>
        /// the cache file name for the directory file list
        /// </summary>
        public string CacheDirName
        {
            get { return cacheDirName; }
        }

        public string DirectoryName
        {
            get { return directoryName; }
        }

        /// <summary>
        /// The file list includes all sub diectories and files in current folder.
        /// </summary>
        public Dictionary<string, FileEntry> FolderFileList
        {
            get { return dirFileList; }
        }

        public List<FileEntry> FolderList
        {
            get 
            {
                List<FileEntry> folderList = new List<FileEntry>();
                foreach (KeyValuePair<string,FileEntry> entry in dirFileList)
                {
                    FileEntry fileEntry = entry.Value;
                    if ((fileEntry.FileAttributes & (uint)FileAttributes.Directory) ==(uint)FileAttributes.Directory)
                    {
                        folderList.Add(fileEntry);
                    }
                }
                return folderList; 
            }
        }

        public List<FileEntry> FileList
        {
            get
            {
                List<FileEntry> fileList = new List<FileEntry>();
                foreach (KeyValuePair<string, FileEntry> entry in dirFileList)
                {
                    FileEntry fileEntry = entry.Value;
                    if ((fileEntry.FileAttributes & (uint)FileAttributes.Directory) != (uint)FileAttributes.Directory)
                    {
                        fileList.Add(fileEntry);
                    }
                }
                return fileList;
            }
        }



        private int CompareFile(FileEntry x, FileEntry y)
        {
            return string.Compare(x.FileName, y.FileName, true);
        }

        /// <summary>
        /// Load file list from the directory cache file.
        /// </summary>
        public bool LoadFileList()
        {
            bool ret = false;
            FileStream fs = null;

            if (forceDownload)
            {
                //we need to force download the directory listing again.
                return false;
            }

            if (!File.Exists(cacheDirName))
            {
                string lastError = cacheDirName + " doesn't exist.";
                EventManager.WriteMessage(229, "LoadFileList", EventLevel.Verbose, lastError);
                return false;
            }
            else
            {
                FileInfo fileInfo = new FileInfo(cacheDirName);
                DateTime createdTime = fileInfo.LastWriteTime;

                TimeSpan ttl = DateTime.Now - createdTime;

                if (ttl.TotalSeconds > GlobalConfig.ExpireCachedDirectoryListingAfterSeconds)
                {

                    EventManager.WriteMessage(240, "LoadFileList", EventLevel.Verbose
                        , "Cache dirName " + cacheDirName + " is expired. Last write time is " + fileInfo.LastWriteTime.ToString("yyyy-MM-ddTHH:mm:ss") + ",cache timeout in seconds:" + GlobalConfig.ExpireCachedDirectoryListingAfterSeconds);

                    try
                    {
                        File.Delete(cacheDirName);
                    }
                    catch
                    {
                    }

                    return false;

                }

            }

            try
            {
                dirFileList.Clear();

                fs = new FileStream(cacheDirName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);
                int totalReadSize = 0;

                while (totalReadSize < fs.Length && fs.Position < fs.Length)
                {
                    FileEntry fileEntry = new FileEntry();

                    //private uint FILE_ENTRY_STRUCT_SIZE = 4/*entryLength*/ + 4/*flags*/ +  4/*FileAttributes*/ + 8/*Index*/+ 8/*FileSize*/ + 8/*creationTime*/
                    //                                      + 8/*LastAccessTime*/ + 8/*LastWriteTime*/  + 4/*TagDataLength*/ + 4/*FileNameLength*/ ;

                    fileEntry.EntryLength = br.ReadUInt32();
                    fileEntry.Flags = br.ReadUInt32();                                                            
                    fileEntry.FileAttributes = br.ReadUInt32();
                    fileEntry.FileId = br.ReadInt64();
                    fileEntry.FileSize = br.ReadInt64();
                    fileEntry.CreationTime = br.ReadInt64();
                    fileEntry.LastAccessTime = br.ReadInt64();
                    fileEntry.LastWriteTime = br.ReadInt64();
                    fileEntry.TagDataLength = br.ReadUInt32();
                    fileEntry.FileNameLength = br.ReadUInt32();
                    if (fileEntry.FileNameLength > 0 && fileEntry.FileNameLength < fileEntry.EntryLength)
                    {
                        fileEntry.FileName = UnicodeEncoding.Unicode.GetString(br.ReadBytes((int)fileEntry.FileNameLength));
                    }
                    if (fileEntry.TagDataLength > 0 && fileEntry.TagDataLength < fileEntry.EntryLength)
                    {
                        fileEntry.TagData = br.ReadBytes((int)fileEntry.TagDataLength);
                    }

                    totalReadSize = totalReadSize + (int)fileEntry.EntryLength;

                    dirFileList.Add(fileEntry.FileName.ToLower(), fileEntry);
                }

                EventManager.WriteMessage(265, "GetFileList", EventLevel.Verbose, "Cache file " + cacheDirName + " exist,load it total entries:" + dirFileList.Count + " size:" + totalReadSize);


                //foreach (FileEntry fileEntry in dirFileList.Values)
                //{
                //    string message = "Get fileEntry Length:" + fileEntry.EntryLength + " FileId:" + fileEntry.FileId + " Name:" + fileEntry.FileName
                //        + " size:" + fileEntry.FileSize + " NameLength:" + fileEntry.FileNameLength + " Attributes:" + (FileAttributes)fileEntry.FileAttributes
                //        + " DateModified:" + DateTime.FromFileTime(fileEntry.LastWriteTime).ToLongTimeString();

                //    EventManager.WriteMessage(295, "GetFileList", EventLevel.Trace,message);
                //}

                ret = true;
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(1001, "LoadFileList", EventLevel.Error, "Load cache file " + cacheDirName + " error with " + ex.Message);
                ret = false;
            }

            if (fs != null)
            {
                fs.Close();
            }


            return ret;

        }

        /// <summary>
        /// Write file list to the directory cache file.
        /// </summary>
        /// <returns></returns>
        public bool SaveFileList()
        {
            bool retVal = true;
            FileStream fs = null;

            try
            {
                fs = new FileStream(cacheDirName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                if (dirFileList.Values.Count == 0)
                {
                    return true;
                }

                int fileListSize = 0;

                foreach (FileEntry fileEntry in dirFileList.Values)
                {
                    fileListSize += (int)fileEntry.EntryLength;
                }

                int fileSize = 0;

                foreach (FileEntry fileEntry in dirFileList.Values)
                {
                    //private uint FILE_ENTRY_STRUCT_SIZE =  4/*entryLength*/ + 4/*flags*/ +  4/*FileAttributes*/ + 8/*Index*/+ 8/*FileSize*/ + 8/*creationTime*/
                    //                                      + 8/*LastAccessTime*/ + 8/*LastWriteTime*/  + 4/*TagDataLength*/ + 4/*FileNameLength*/ ;

                    byte[] buffer = new byte[fileEntry.EntryLength];
                    MemoryStream ms = new MemoryStream(buffer);
                    BinaryWriter bw = new BinaryWriter(ms);

                    bw.Write(fileEntry.EntryLength);
                    bw.Write(fileEntry.Flags);
                    bw.Write(fileEntry.FileAttributes);
                    bw.Write(fileEntry.FileId);
                    bw.Write(fileEntry.FileSize);
                    bw.Write(fileEntry.CreationTime);
                    bw.Write(fileEntry.LastAccessTime);
                    bw.Write(fileEntry.LastWriteTime);
                    bw.Write(fileEntry.TagDataLength);
                    bw.Write(fileEntry.FileNameLength);
                    if (fileEntry.FileNameLength > 0)
                    {
                        byte[] fileNameArray = UnicodeEncoding.Unicode.GetBytes(fileEntry.FileName);
                        bw.Write(fileNameArray);
                    }
                    if (fileEntry.TagDataLength > 0)
                    {
                        bw.Write(fileEntry.TagData);
                    }
                    fileSize = fileSize + (int)fileEntry.EntryLength;

                    bw.Flush();
                    fs.Write(buffer, 0, buffer.Length);
                }

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(1002, "WriteFileList", EventLevel.Error, "Write cache file " + cacheDirName + " error with " + ex.Message);
                retVal = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Flush();
                    fs.Close();
                }
            }

            return retVal;
        }

        //public bool GetFileList()
        //{
        //    bool ret = true;
        //    DirectoryInfo dirInfo = new DirectoryInfo(directoryName);
        //    FileInfo[] fileInfos = dirInfo.GetFiles();
        //    foreach (FileInfo fileInfo in fileInfos)
        //    {
        //        AddFileEntry(fileInfo.Name,
        //                    ASCIIEncoding.ASCII.GetBytes(fileInfo.Name),
        //                    fileInfo.CreationTime.ToFileTime(),
        //                    fileInfo.LastAccessTime.ToFileTime(),
        //                    fileInfo.LastWriteTime.ToFileTime(),
        //                    fileInfo.Length,
        //                    (uint)fileInfo.Attributes);
        //    }

        //    if (SaveFileList())
        //    {
        //        //LoadFileList();

        //        //foreach (FileEntry fileEntry in fileList.Values)
        //        //{
        //        //    string message = "Length:" + fileEntry.EntryLength + " Id:" + fileEntry.FileId + " Name:" + fileEntry.FileName
        //        //        + " size:" + fileEntry.FileSize + " NameLength:" + fileEntry.FileNameLength + " Attributes:" + (FileAttributes)fileEntry.FileAttributes
        //        //        + " DateModified:" + DateTime.FromFileTime(fileEntry.LastWriteTime).ToLongTimeString();

        //        //    EventManager.WriteMessage(1, "GetFileList", EventManager.EventType.Information, message);
        //        //}
        //    }

        //    return ret;
        //}
    }

}
