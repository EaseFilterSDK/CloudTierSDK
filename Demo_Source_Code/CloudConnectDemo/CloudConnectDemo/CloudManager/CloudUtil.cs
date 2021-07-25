
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
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using EaseFilter.GlobalObjects;

namespace EaseFilter.CloudManager
{
    static public class CloudUtil
    {

        public static bool IsMappedFolder(string folderName)
        {
            foreach (SiteInfo siteInfo in GlobalConfig.SiteInfos.Values)
            {
                if (folderName.ToLower().StartsWith(siteInfo.LocalPath.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        public static SiteInfo GetSiteInfoByLocalPath(string localFileName)
        {

            foreach (SiteInfo siteInfo in GlobalConfig.SiteInfos.Values)
            {
                if (localFileName.ToLower().StartsWith(siteInfo.LocalPath.ToLower()))
                {
                    return siteInfo;
                }
            }

            return null;
        }

        public static SiteInfo GetSiteInfoBySiteName(string siteName)
        {

            foreach (SiteInfo siteInfo in GlobalConfig.SiteInfos.Values)
            {
                if (string.Compare(siteInfo.SiteName,siteName) == 0)
                {
                    return siteInfo;
                }
            }

            return null;
        }


        public static bool IsLocalPathAlreadyMapped(string siteName,string localFolder)
        {
            string lowerLocalPath = localFolder.ToLower();

            foreach (SiteInfo siteInfo in GlobalConfig.SiteInfos.Values)
            {
                string siteInfolocalPath = siteInfo.LocalPath.ToLower();

                if (string.Compare(siteInfo.SiteName, siteName) == 0)
                {
                    continue;
                }

                if (lowerLocalPath.StartsWith(siteInfolocalPath))
                {
                    string restFolder = localFolder.Substring(siteInfo.LocalPath.Length);
                    if (restFolder.Length == 0 || restFolder.StartsWith("\\"))
                    {
                        return false;
                    }
                }

                if (  siteInfolocalPath.StartsWith(lowerLocalPath))
                {
                    string restFolder = siteInfolocalPath.Substring(localFolder.Length);

                    if (restFolder.Length == 0 || restFolder.StartsWith("\\"))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static string GetRemotePathByLocalPath(string localPath, SiteInfo siteInfo)
        {
            string remotePath = localPath;

            if (remotePath.StartsWith(siteInfo.LocalPath, true, System.Globalization.CultureInfo.CurrentCulture))
            {
                remotePath = remotePath.Substring(siteInfo.LocalPath.Length);
            }
            else
            {
                remotePath = localPath;
            }

            if (remotePath.StartsWith("\\") || remotePath.StartsWith("/"))
            {
                remotePath = remotePath.Substring(1);
            }

            remotePath = Path.Combine(siteInfo.RemotePath, remotePath);
            remotePath = remotePath.Replace("\\", "/");

            return remotePath;
        }

        public static void GetMappingInfo(SiteInfo siteInfo, string fileName, ref string localCacheFileName, ref string remoteFileName)
        {
            string cacheFolder = GlobalConfig.CloudCacheFolder;
            string mappingFolder = siteInfo.LocalPath;

            string cacheFileName = Path.Combine(cacheFolder, siteInfo.CloudProvider.ToString());
            cacheFileName = Path.Combine(cacheFileName, siteInfo.SiteName);
            
            remoteFileName = GetRemotePathByLocalPath(fileName, siteInfo);

            string remoteName = siteInfo.RemotePath;

            localCacheFileName = cacheFileName;

            if (remoteName.Length > 0)
            {
                //the site info has the remote path setting
                if (remoteFileName.Length > remoteName.Length)
                {
                    localCacheFileName = Path.Combine(cacheFileName, remoteFileName.Substring(remoteName.Length + 1));
                }
            }
            else
            {
                localCacheFileName = Path.Combine(cacheFileName, remoteFileName);
            }

            localCacheFileName = localCacheFileName.Replace("/", "\\");

            string directory = Path.GetDirectoryName(localCacheFileName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

        }

        public static bool TestRemotePath(SiteInfo siteInfo,string remotePath,out string lastError)
        {
            lastError = string.Empty;
            bool ret = false;

            try
            {
                CloudProvider cloudProvider = CloudBroker.GetProvider(siteInfo,null);

                if (null == cloudProvider)
                {
                    lastError = "There are no cloud provider for site" + siteInfo.SiteName;
                    return ret;
                }

                ret = cloudProvider.TestConnection(ref lastError);

                return ret;
            }
            catch (Exception ex)
            {
                lastError = "TestConnection failed:" + ex.Message;
            }

            return ret;

        }


        public static string GetCacheFileNameByFolderName(string directoryName)
        {
            SiteInfo siteInfo = GetSiteInfoByLocalPath(directoryName);

            if( null == siteInfo )
            {
                EventManager.WriteMessage(173, "GetCacheFileNameByFolderName", EventLevel.Error, "Can't get site info by folder name :" + directoryName);
                return string.Empty;
            }

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
          

            if (!Directory.Exists(cacheDirName))
            {
                Directory.CreateDirectory(cacheDirName);
            }

            cacheDirName = Path.Combine(cacheDirName, GlobalConfig.DirInfoListName);

            return cacheDirName;
        }
    }
}
