using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using EaseFilter.GlobalObjects;
using EaseFilter.CloudManager;

namespace EaseFilter.CloudExplorer
{
    public class CloudWorker
    {

        protected CloudProvider.UpdataStatusDlgt updataStatusDlgt = null;

        public CloudWorker(CloudProvider.UpdataStatusDlgt _updataStatusDlgt)
        {
            updataStatusDlgt = _updataStatusDlgt;
        }


        public bool GetLocalFullPathByTreeNodeFullPath(string folderName,out string localFullPath, out SiteInfo currentSiteInfo)
        {
            currentSiteInfo = null;
            localFullPath = string.Empty;

            string[] str = folderName.Split(new string[] { "\\" }, StringSplitOptions.None);

            if (str.Length < 2)
            {
                string errorMessage = "Can't extract file name " + folderName;
                EventManager.WriteMessage(51, "GetLocalFullPathByTreeNodeFullPath", EventLevel.Error, errorMessage);

                MessageBox.Show(errorMessage, "GetLocalFullPathByTreeNodeFullPath", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            string provider = str[0];
            string siteName = str[1];

            currentSiteInfo = CloudUtil.GetSiteInfoBySiteName(siteName);

            string localFolder = folderName.Replace(provider + "\\" + siteName, "");

            if (localFolder.StartsWith("\\")) 
            {
                localFolder = localFolder.Substring(1);
            }

            localFullPath = Path.Combine(currentSiteInfo.LocalPath, localFolder);

            return true;
        }

        public async Task<DirectoryList> ExtractTreeNodes(string folderName, bool refresh)
        {
            bool ret = false;
            string localFullPath = string.Empty;
            SiteInfo currentSiteInfo = null;

            try
            {
                ret = GetLocalFullPathByTreeNodeFullPath(folderName, out localFullPath, out currentSiteInfo);

                CloudProvider cloudProvider = CloudBroker.GetProvider(currentSiteInfo, updataStatusDlgt);
                if (null == cloudProvider)
                {
                    EventManager.WriteMessage(82, "ExtractTreeNodes", EventLevel.Error, "Can't get cloud provider for site info:" + currentSiteInfo.SiteName);
                    return null;
                }

                string remotePath = CloudUtil.GetRemotePathByLocalPath(localFullPath, currentSiteInfo);

                DirectoryList downloadFileList = await cloudProvider.DownloadFileListAsync(localFullPath, refresh);

                return downloadFileList;

            }
            catch (Exception ex)
            {
                string errorMessage = "Can't extract file name " + folderName + ",error:" + ex.Message;
                EventManager.WriteMessage(1102, "ExtractTreeNodes", EventLevel.Error, errorMessage);

                MessageBox.Show(errorMessage, "ExtractTreeNodes", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
            }


        }
      

        public bool GetSiteInfo(string treenodeFolderFullPath, ref SiteInfo currentSiteInfo)
        {
            string[] str = treenodeFolderFullPath.Split(new string[] { "\\" }, StringSplitOptions.None);

            if (str.Length < 2)
            {
                EventManager.WriteMessage(4847, "ExtractTreeNodes", EventLevel.Error, "Can't extract file name " + treenodeFolderFullPath);
                return false;
            }

            string provider = str[0];
            string siteName = str[1];
           
            if( GlobalConfig.SiteInfos.ContainsKey(siteName))
            {
                currentSiteInfo = GlobalConfig.SiteInfos[siteName];
            }

            return true;
        }


        public async Task<bool> DownloadFilesAsync(string treeNodeFullPath, List<string> fileNames, string destinationFolder)
        {
            bool ret = false;
     
            try
            {
                SiteInfo currentSiteInfo = null;

                string localFullPath = string.Empty;
                ret = GetLocalFullPathByTreeNodeFullPath(treeNodeFullPath, out localFullPath, out currentSiteInfo);

                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                CloudProvider cloudProvider = CloudBroker.GetProvider(currentSiteInfo, updataStatusDlgt);

                if (null == cloudProvider)
                {
                    return false;
                }

                DirectoryList downloadFileList = await cloudProvider.DownloadFileListAsync(localFullPath, false);

                List<FileEntry> dirInfos = downloadFileList.FolderList;
                List<FileEntry> fileInfos = downloadFileList.FileList;
                Dictionary<string, FileEntry> allFileList = downloadFileList.FolderFileList;

                foreach (string fileName in fileNames)
                {
                    long fileSize = 0;
                    DateTime creationTime = DateTime.Now;
                    FileAttributes fileAttributes = FileAttributes.Normal;

                    if( allFileList.ContainsKey(fileName.ToLower()))
                    {
                        FileEntry entry = allFileList[fileName.ToLower()];
                        {
                            fileSize = entry.FileSize;
                            creationTime = DateTime.FromFileTime(entry.LastWriteTime);
                            fileAttributes = (FileAttributes)entry.FileAttributes;
                        }
                    }

                    string remotePath = treeNodeFullPath.Replace(currentSiteInfo.CloudProvider.ToString() + "\\" + currentSiteInfo.SiteName, "");
                    if (remotePath.StartsWith("\\") || remotePath.StartsWith("/"))
                    {
                        remotePath = remotePath.Substring(1);
                    }

                    remotePath = Path.Combine(currentSiteInfo.RemotePath, remotePath);
                    remotePath = Path.Combine(remotePath, fileName);
                    remotePath = remotePath.Replace("\\", "/");
                    string destFileName = Path.Combine(destinationFolder, Path.GetFileName(fileName));

                    AsyncTask asyncTask = new AsyncTask(TaskType.DownloadFile,currentSiteInfo,destFileName,remotePath,fileSize,creationTime,fileAttributes,0,Environment.UserName,"CloudExplorer");

                    await cloudProvider.ProcessTaskAsync(asyncTask);

                    ret = true;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "DownloadFiles failed with error:" + ex.Message;
                EventManager.WriteMessage(217, "DownloadFiles", EventLevel.Error, errorMessage);

                MessageBox.Show(errorMessage, "DownloadFiles", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return ret;

        }


        public async Task<bool> UploadFilesAsync(List<string> fileNames, string treeNodeFullPath)
        {
            bool ret = false;

            try
            {
                SiteInfo currentSiteInfo = null;
                if (!GetSiteInfo(treeNodeFullPath, ref currentSiteInfo))
                {
                    return ret;
                }

                foreach (string fileName in fileNames)
                {
                    try
                    {
                        string remotePath = treeNodeFullPath.Replace(currentSiteInfo.CloudProvider.ToString() + "\\" + currentSiteInfo.SiteName, "");
                        if (remotePath.StartsWith("\\") || remotePath.StartsWith("/"))
                        {
                            remotePath = remotePath.Substring(1);
                        }
                        remotePath = Path.Combine(currentSiteInfo.RemotePath, remotePath);
                        remotePath = remotePath.Replace("\\", "/");
                        remotePath = remotePath + "/" + Path.GetFileName(fileName);
                        FileInfo fileInfo = new FileInfo(fileName);

                        AsyncTask asyncTask = new AsyncTask(TaskType.UploadFile, currentSiteInfo, fileName, remotePath, fileInfo.Length, fileInfo.CreationTime, fileInfo.Attributes, 0, Environment.UserName, "CloudExplorer");

                        CloudProvider cloudProvider = CloudBroker.GetProvider(currentSiteInfo,updataStatusDlgt);
                        if (null == cloudProvider)
                        {
                            return false;
                        }

                        await cloudProvider.ProcessTaskAsync(asyncTask);
                    }
                    catch (Exception ex)
                    {
                        EventManager.WriteMessage(267, "UploadFiles", EventLevel.Error, "Add upload file name:" + fileName + " failed with error:" + ex.Message);
                    }

                    ret = true;
                }         
            }
            catch (Exception ex)
            {
                string errorMessage = "UploadFiles failed with error:" + ex.Message;
                EventManager.WriteMessage(267, "UploadFiles", EventLevel.Error, errorMessage);

                MessageBox.Show(errorMessage, "UploadFiles", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            return ret;

        }

        public async Task<bool> MakeDirAsync(string treeNodeFullPath, string dirName)
        {
            bool ret = false;

            try
            {
                SiteInfo currentSiteInfo = null;

                if (!GetSiteInfo(treeNodeFullPath, ref currentSiteInfo))
                {
                    return ret;
                }

                string remotePath = treeNodeFullPath.Replace(currentSiteInfo.CloudProvider.ToString() + "\\" + currentSiteInfo.SiteName, "");
                if (remotePath.StartsWith("\\") || remotePath.StartsWith("/"))
                {
                    remotePath = remotePath.Substring(1);
                }

                remotePath = Path.Combine(currentSiteInfo.RemotePath, remotePath);
                remotePath = Path.Combine(remotePath, Path.GetFileName(dirName));
                remotePath = remotePath.Replace("\\", "/");

                AsyncTask asyncTask = new AsyncTask(TaskType.CreateDirectory, currentSiteInfo, remotePath, remotePath, 0, DateTime.Now, FileAttributes.Normal, 0, Environment.UserName, "CloudExplorer");                

                CloudProvider cloudProvider = CloudBroker.GetProvider(currentSiteInfo, updataStatusDlgt);
                if (cloudProvider != null)
                {
                    await cloudProvider.ProcessTaskAsync(asyncTask);
                }
                else
                {
                    EventManager.WriteMessage(300, "MakeDir", EventLevel.Error, "Can't cloud provider for site info:" + currentSiteInfo.ServerName);
                    return false;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "Create new directory " + treeNodeFullPath + " failed with error:" + ex.Message;
                EventManager.WriteMessage(319, "MakeDir", EventLevel.Error, errorMessage);

                MessageBox.Show(errorMessage, "MakeDir", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            return ret;

        }

        public async Task<bool> RenameFileAsync(SiteInfo currentSiteInfo, string sourceName, string destFileName)
        {
            bool ret = false;

            try
            {
                AsyncTask asyncTask = new AsyncTask(TaskType.Rename, currentSiteInfo, sourceName + " to " + destFileName, sourceName, 0, DateTime.Now, FileAttributes.Normal, 0, Environment.UserName, "CloudExplorer");
                asyncTask.RemoteDestFileName = destFileName;

                CloudProvider cloudProvider = CloudBroker.GetProvider(currentSiteInfo, updataStatusDlgt);
                if (cloudProvider != null)
                {
                    await cloudProvider.ProcessTaskAsync(asyncTask);
                }
                else
                {
                    EventManager.WriteMessage(300, "RenameFileAsync", EventLevel.Error, "Can't cloud provider for site info:" + currentSiteInfo.ServerName);
                    return false;
                }

                ret = true;
            }
            catch (Exception ex)
            {
                string errorMessage = "Rename " + sourceName + " to " + destFileName + " failed with error:" + ex.Message;
                EventManager.WriteMessage(346, "Rename", EventLevel.Error, errorMessage);

                MessageBox.Show(errorMessage, "Rename", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            return ret;

        }

        public async Task<bool> DeleteFileAsync(string treeNodeFullPath, string fileName)
        {
            bool ret = false;

            try
            {
                SiteInfo currentSiteInfo = null;
                if (!GetSiteInfo(treeNodeFullPath, ref currentSiteInfo))
                {
                    return ret;
                }

                string remotePath = treeNodeFullPath.Replace(currentSiteInfo.CloudProvider.ToString() + "\\" + currentSiteInfo.SiteName, "");
                remotePath = remotePath.Replace("\\", "/");
                if (remotePath.StartsWith("/"))
                {
                    remotePath = remotePath.Substring(1);
                }

                remotePath = Path.Combine(currentSiteInfo.RemotePath, remotePath);
                remotePath = Path.Combine(remotePath, fileName);
                remotePath = remotePath.Replace("\\", "/");

                //this is a directory and it is not a bucket(s3) or container name(azure)
                if (fileName.Length == 0 && remotePath.IndexOf("/") >= 0)
                {
                    remotePath += "/";
                }

                AsyncTask asyncTask = new AsyncTask(TaskType.DeleteFile, currentSiteInfo, remotePath, remotePath, 0, DateTime.Now, FileAttributes.Normal, 0, Environment.UserName, "CloudExplorer");

                CloudProvider cloudProvider = CloudBroker.GetProvider(currentSiteInfo, updataStatusDlgt);
                if (cloudProvider != null)
                {
                    await cloudProvider.ProcessTaskAsync(asyncTask);
                }
                else
                {
                    EventManager.WriteMessage(300, "DeleteFileAsync", EventLevel.Error, "Can't cloud provider for site info:" + currentSiteInfo.ServerName);
                    return false;
                }

                ret = true;

            }
            catch (Exception ex)
            {
                string errorMessage = "Delete file " + treeNodeFullPath + " failed with error:" + ex.Message;
                EventManager.WriteMessage(426, "Delete", EventLevel.Error, errorMessage);

                MessageBox.Show(errorMessage, "Delete", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            return ret;

        }
    }

}
