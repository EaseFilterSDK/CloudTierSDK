using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Management;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

using EaseFilter.GlobalObjects;
using EaseFilter.CloudManager;

namespace EaseFilter.CloudExplorer
{   

    public partial class CloudExplorer : System.Windows.Forms.Form
    {

        public enum TreeNodeType
        {
            File = 0,
            Directory
        }

        public CloudExplorer()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            EventManager.MessageEventHandler = new EventManager.MessageEventDlgt(MessageEventHandler);

          //  this.WindowState = FormWindowState.Maximized;

            // Populate TreeView with cloud provider list
            PopulateCloudProviderList(false);

        }

        ~CloudExplorer()
        {
            Cleanup();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
          
            Cleanup();

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
            Application.Exit();
        }

        private void menuItem2_Click(object sender, System.EventArgs e)
        {
            //Cleanup();
            //quit application
            this.Close();
        }

        private void Cleanup()
        {
            try
            {
                GlobalConfig.Stop();
                EventManager.Stop();
           
            
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(910,"Cleanup", EventLevel.Verbose,ex.Message );
            }
        }

    

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new CloudExplorer());           
        }

        //This procedure populate the TreeView with the Drive list
        private void PopulateCloudProviderList(bool overWrite)
        {
            try
            {
                TreeNode nodeTreeNode;

                this.Cursor = Cursors.WaitCursor;
                //clear TreeView
                tvFolders.Nodes.Clear();
                Dictionary<string, SiteInfo> siteInfos = GlobalConfig.SiteInfos;

                foreach (CloudProviderType provider in Enum.GetValues(typeof(CloudProviderType)))
                {

                    nodeTreeNode = new TreeNode(provider.ToString(), 0, 1);
                    tvFolders.Nodes.Add(nodeTreeNode);

                    //populate the cloud provider site list
                    foreach (SiteInfo siteInfo in siteInfos.Values)
                    {
                        if (siteInfo.CloudProvider == provider)
                        {
                            //create node for directories
                            TreeNode nodeDir = new TreeNode(siteInfo.SiteName, 9, 10);
                            nodeTreeNode.Nodes.Add(nodeDir);
                        }
                    }
                }

                //Init files ListView
                InitListView();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(903, "PopulateDriveList", EventLevel.Error, ex.Message);
            }

        }

        private void tvFolders_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            try
            {
                //Populate folders and files when a folder is selected
                this.Cursor = Cursors.WaitCursor;

                //get current selected drive or folder
                currentNode = e.Node;
                Refresh(false);
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(904, "tvFolders_AfterSelect", EventLevel.Error, ex.Message);
            }
        }

        protected void InitListView()
        {
            //init ListView control
            lvFiles.Clear();		//clear control
            //create column header for ListView
            lvFiles.Columns.Add("Name", 250, System.Windows.Forms.HorizontalAlignment.Center);
            lvFiles.Columns.Add("Size", 75, System.Windows.Forms.HorizontalAlignment.Right);
            lvFiles.Columns.Add("FileSyncTime", 140, System.Windows.Forms.HorizontalAlignment.Left);
        
        }

        protected async void PopulateDirectory(TreeNode nodeCurrent, TreeNodeCollection nodeCurrentCollection,bool refresh)
        {

            TreeNode nodeDir;
            int imageIndex = 2;		//unselected image index
            int selectIndex = 3;	//selected image index

            //populate treeview with folders
            try
            {
                CloudProvider.UpdataStatusDlgt updateStatusDlgt = new CloudProvider.UpdataStatusDlgt(UpdateStatus);
                CloudWorker cloudWorker = new CloudWorker(updateStatusDlgt);

                DirectoryList directoryList = await cloudWorker.ExtractTreeNodes(nodeCurrent.FullPath,refresh);

                if (null == directoryList)
                {
                    return;
                }

                List<FileEntry> dirInfos = directoryList.FolderList;
                List<FileEntry> fileInfos = directoryList.FileList;

                //populate files
                PopulateFiles(fileInfos);

                if (null == dirInfos || dirInfos.Count == 0)
                {
                    return;
                }

                //loop throught all directories
                foreach (FileEntry dirInfo in dirInfos)
                {
                    //create node for directories
                    nodeDir = new TreeNode(dirInfo.FileName, imageIndex, selectIndex);
                    nodeCurrentCollection.Add(nodeDir);
                }


            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(905, "PopulateDirectory", EventLevel.Error, ex.Message);
            }
        }

        protected string GetPathName(string stringPath)
        {
            //Get Name of folder
            string[] stringSplit = stringPath.Split('\\');
            int _maxIndex = stringSplit.Length;
            return stringSplit[_maxIndex - 1];
        }

        protected void PopulateFiles(List<FileEntry> fileInfos)
        {
            try
            {
                //Populate listview with files
                string[] lvData = new string[3];

                //clear list
                InitListView();


                if (null == fileInfos || 0 == fileInfos.Count)
                {
                    return;
                }

                if (null == imageListDocuments)
                {
                    imageListDocuments = new SystemImageList(SystemImageListSize.SmallIcons);
                }

                SystemImageListHelper.SetListViewImageList(lvFiles, imageListDocuments, false);

                if (null == currentNode
                    || currentNode.SelectedImageIndex != 0
                    || null == fileInfos)
                {


                    string stringFileName = "";
                    DateTime dtModifyDate;
                    long lFileSize = 0;

                    //loop throught all files
                    foreach (FileEntry fileInfo in fileInfos)
                    {
                        stringFileName = fileInfo.FileName;
                        lFileSize = fileInfo.FileSize;

                        dtModifyDate = DateTime.FromFileTime(fileInfo.LastWriteTime);

                        //create listview data
                        lvData[0] = GetPathName(stringFileName);
                        lvData[1] = formatSize(lFileSize);
                        lvData[2] = formatDate(dtModifyDate);

                        //Create actual list item
                        ListViewItem lvItem = new ListViewItem(lvData, 0);
                        lvItem.Tag = fileInfo;
                        lvItem.ImageIndex = imageListDocuments.IconIndex(lvData[0]);
                        lvFiles.Items.Add(lvItem);

                    }

                }
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(906, "PopulateFiles", EventLevel.Error, ex.Message);
            }
        }


        protected string formatDate(DateTime dtDate)
        {
            //Get date and time in short format
            string stringDate = "";

            stringDate = dtDate.ToShortDateString().ToString() + " " + dtDate.ToShortTimeString().ToString();

            return stringDate;
        }

        protected string formatSize(Int64 lSize)
        {
            //Format number to KB
            string stringSize = "";
            NumberFormatInfo myNfi = new NumberFormatInfo();

            Int64 lKBSize = 0;

            try
            {
                if (lSize < 1024)
                {
                    if (lSize == 0)
                    {
                        //zero byte
                        stringSize = "0";
                    }
                    else
                    {
                        //less than 1K but not zero byte
                        stringSize = "1";
                    }
                }
                else
                {
                    //convert to KB
                    lKBSize = lSize / 1024;
                    //format number with default format
                    stringSize = lKBSize.ToString("n", myNfi);
                    //remove decimal
                    stringSize = stringSize.Replace(".00", "");
                }
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(907, "formatSize", EventLevel.Error, ex.Message);
            }

            return stringSize + " KB";

        }

        private async Task DownloadDirectoryFilesAsync(string treeNodeFullPath, string destinationFolder)
        {
            try
            {
                CloudProvider.UpdataStatusDlgt updateStatusDlgt = new CloudProvider.UpdataStatusDlgt(UpdateStatus);
                CloudWorker cloudWorker = new CloudWorker(updateStatusDlgt);

                string localFullPath = string.Empty;
                SiteInfo currentSiteInfo = null;

                if (!cloudWorker.GetLocalFullPathByTreeNodeFullPath(treeNodeFullPath, out localFullPath, out currentSiteInfo))
                {
                    return;
                }

                CloudProvider cloudProvider = CloudBroker.GetProvider(currentSiteInfo, updateStatusDlgt);

                if (cloudProvider != null)
                {
                    DirectoryList downloadFileList = await cloudProvider.DownloadFileListAsync(localFullPath, false);

                    List<FileEntry> dirInfos = downloadFileList.FolderList;
                    List<FileEntry> fileInfos = downloadFileList.FileList;

                    if (fileInfos.Count > 0)
                    {
                        string lastError = string.Empty;
                        List<string> selectedFiles = new List<string>();
                        foreach (FileEntry fileEntry in fileInfos)
                        {
                            selectedFiles.Add(fileEntry.FileName);
                        }

                        await cloudWorker.DownloadFilesAsync(treeNodeFullPath, selectedFiles, destinationFolder);
                    }

                    foreach (FileEntry fileEntry in dirInfos)
                    {
                        string subFolder = treeNodeFullPath + "\\" + fileEntry.FileName;
                        await DownloadDirectoryFilesAsync(subFolder, destinationFolder + "\\" + fileEntry.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(773, "DownloadDirectoryFilesAsync", EventLevel.Error, "DownloadDirectoryFilesAsync " + treeNodeFullPath + " failed with error:" + ex.Message);
            }
        }

        private async Task DownloadFilesAsync()
        {
            try
            {
                TreeNode tn = this.tvFolders.SelectedNode;
                TreeNodeType fileType = TreeNodeType.Directory;
                string treeNodePath = currentNode.FullPath;
                List<string> selectedFiles = new List<string>();
                string destinationFolder = GlobalConfig.CloudCacheFolder;

               if (lvFiles.SelectedItems.Count > 0)
                {
                    fileType = TreeNodeType.File;
                }
               else if (tn != null)
               {
                   fileType = TreeNodeType.Directory;
               }
               else
               {
                   MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);MessageBox.Show(" Please select a folder.", "Select folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                   return;
               }
             
                destinationFolder += "\\" + treeNodePath;

                InputForm inputForm = new InputForm("Download Path", "Destination path:", destinationFolder);
                if (inputForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    destinationFolder = inputForm.InputText;
                }
                else
                {
                    return;
                }                

                foreach (ListViewItem lvItem in lvFiles.SelectedItems)
                {
                    FileEntry fileEntry = (FileEntry)lvItem.Tag;
                    selectedFiles.Add(fileEntry.FileName);
                }

                if (selectedFiles.Count > 0)
                {
                    CloudProvider.UpdataStatusDlgt updateStatusDlgt = new CloudProvider.UpdataStatusDlgt(UpdateStatus);
                    CloudWorker cloudWorker = new CloudWorker(updateStatusDlgt);

                    await cloudWorker.DownloadFilesAsync(treeNodePath, selectedFiles,destinationFolder);
                }
                else if(fileType == TreeNodeType.Directory)
                {
                    string[] str = treeNodePath.Split(new string[] { "\\" }, StringSplitOptions.None);

                    if (str.Length <= 2)
                    {
                        MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);MessageBox.Show(treeNodePath + " is invalid. Please select a folder.", "Select folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        await DownloadDirectoryFilesAsync(treeNodePath, destinationFolder);
                    }
                }

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(908, "RestoreFiles", EventLevel.Error, ex.Message);
            }

        }


        private void lvFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lvFiles.SelectedItems.Count > 0)
                {
                    currentFile = lvFiles.SelectedItems[0].Text;
                    TreeNode tn = tvFolders.SelectedNode;
                    currentFile = tn.FullPath;
                    currentFile = Path.Combine(currentFile, lvFiles.SelectedItems[0].Text);

                    UpdateStatus(lvFiles.SelectedItems.Count + " files were selected.",false);
                }

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(901, "lvFiles_SelectedIndexChanged", EventLevel.Error, "Error in listViewFiles_SelectedIndexChanged, System reports: " + ex.Message);
            }
        }


        private async void downloadFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await DownloadFilesAsync();         
        }

        private async void restoreStubFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await DownloadFilesAsync();     
        }

        private void lvFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {

            ListView lsView = null;

            try
            {
                lsView = (ListView)sender;

                switch (lsView.Sorting)
                {
                    case SortOrder.Ascending:
                        lsView.Sorting = SortOrder.Descending;
                        break;

                    case SortOrder.Descending:
                    case SortOrder.None:
                        lsView.Sorting = SortOrder.Ascending;
                        break;
                }

                lsView.ListViewItemSorter = new ListViewComparer((ListViewComparer.Column)e.Column, lsView.Sorting);
                SystemImageListHelper.SetListViewImageList(lvFiles, imageListDocuments, false);                 

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(902, "lvFiles_ColumnClick", EventLevel.Error, ex.Message);
            }
            finally
            {
                lsView = null;
            }

        }

        private void lvFiles_ItemActivate(object sender, EventArgs e)
        {
            string lastError = string.Empty;

            try
            {
                if (lvFiles.SelectedItems.Count > 0)
                {
                    ListViewItem lvi = lvFiles.SelectedItems[0];
                    if (lvi != null)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(909, "lvFiles_ItemActivate", EventLevel.Error, ex.Message);
            }
        }

        private void Refresh(bool forcedownload)
        {
            try
            {
                //Populate folders and files when a folder is selected
                this.Cursor = Cursors.WaitCursor;

                //clear all sub-folders
                currentNode.Nodes.Clear();

                if (currentNode.Parent == null)
                {
                    //Selected My Computer - repopulate drive list
                    PopulateCloudProviderList(false);
                }
                else
                {
                    //populate sub-folders and folder files
                    PopulateDirectory(currentNode, currentNode.Nodes, forcedownload);
                    currentNode.Expand();
                }
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(911, "toolStripButton_Refresh_Click", EventLevel.Error, ex.Message);
            }
        }

        private void toolStripButton_Refresh_Click(object sender, EventArgs e)
        {
            Refresh(true);
        }

        private async Task UploadDirectoryAsync(string localDirectoryFullPath, string treeNodePath,string directoryName)
        {
            try
            {
                CloudProvider.UpdataStatusDlgt updateStatusDlgt = new CloudProvider.UpdataStatusDlgt(UpdateStatus);
                CloudWorker cloudWorker = new CloudWorker(updateStatusDlgt);

                string lastError = string.Empty;

                DirectoryInfo dirInfo = new DirectoryInfo(localDirectoryFullPath);
                FileInfo[] fileInfos = dirInfo.GetFiles();
           
                List<string> selectedFiles = new List<string>();
                foreach (FileInfo fileInfo in fileInfos)
                {
                    selectedFiles.Add(fileInfo.FullName);
                }

                treeNodePath += "\\" + directoryName;
                await cloudWorker.UploadFilesAsync(selectedFiles, treeNodePath);

                foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                {
                    await UploadDirectoryAsync(dir.FullName, treeNodePath, dir.Name);
                }

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(1012, "UploadDirectory", EventLevel.Error, "Upload " + localDirectoryFullPath + " failed with error:" + ex.Message);
            }
        }

        private async Task UploadFilesAsync()
        {
            TreeNode tn = this.tvFolders.SelectedNode;

            if (tn == null)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show(" Please select a folder.", "Select folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string treeNodePath = currentNode.FullPath;

                string[] str = treeNodePath.Split(new string[] { "\\" }, StringSplitOptions.None);

                if (str.Length <= 2)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show(treeNodePath + " is invalid. Please select a folder.", "Select folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<string> selectedFiles = new List<string>();
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = "All files|*.*";
                fileDialog.Multiselect = true;
                if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    foreach (string fileName in fileDialog.FileNames)
                    {
                        selectedFiles.Add(fileName);
                    }

                    CloudProvider.UpdataStatusDlgt updateStatusDlgt = new CloudProvider.UpdataStatusDlgt(UpdateStatus);
                    CloudWorker cloudWorker = new CloudWorker(updateStatusDlgt);

                    string lastError = string.Empty;
                    await cloudWorker.UploadFilesAsync(selectedFiles, treeNodePath);

                }

            }
        }

        private async void toolStripButton_UploadFiles_Click(object sender, EventArgs e)
        {
           await UploadFilesAsync();
            Refresh(true);
        }

        private async void toolStripButton_Download_Click(object sender, EventArgs e)
        {
           await DownloadFilesAsync();
        }

        private async Task DeleteDirectoryAsync(string treeNodeFullPath)
        {
            try
            {

                CloudProvider.UpdataStatusDlgt updateStatusDlgt = new CloudProvider.UpdataStatusDlgt(UpdateStatus);
                CloudWorker cloudWorker = new CloudWorker(updateStatusDlgt);

                string localFullPath = string.Empty;
                SiteInfo currentSiteInfo = null;

                if (!cloudWorker.GetLocalFullPathByTreeNodeFullPath(treeNodeFullPath, out localFullPath, out currentSiteInfo))
                {
                    return;
                }

                CloudProvider cloudProvider = CloudBroker.GetProvider(currentSiteInfo,updateStatusDlgt);

                if (cloudProvider != null)
                {                  
                    Task<DirectoryList> downloadFileListTask = cloudProvider.DownloadFileListAsync(localFullPath, false);

                    Dictionary<string, FileEntry> allFileList = new Dictionary<string, FileEntry>();
                    List<FileEntry> dirInfos = downloadFileListTask.Result.FolderList;
                    List<FileEntry> fileInfos = downloadFileListTask.Result.FileList;

                    foreach (FileEntry fileEntry in dirInfos)
                    {
                        string subFolder = treeNodeFullPath + "\\" + fileEntry.FileName;
                        await DeleteDirectoryAsync(subFolder);
                        
                    }

                    if (fileInfos.Count > 0)
                    {
                        foreach (FileEntry fileEntry in fileInfos)
                        {
                            await cloudWorker.DeleteFileAsync(treeNodeFullPath, fileEntry.FileName);                            
                        }
                    }

                    await cloudWorker.DeleteFileAsync(treeNodeFullPath,"");
                }
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(773, "DeleteDirectoryAsync", EventLevel.Error, "DeleteDirectoryAsync " + treeNodeFullPath + " failed with error:" + ex.Message);
            }
        }

        private async Task DeleteFilesAsync()
        {
            TreeNode tn = this.tvFolders.SelectedNode;            

            if (tn == null)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);MessageBox.Show(" Please select a folder or files.", "Select folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string treeNodePath = currentNode.FullPath;
            string[] str = treeNodePath.Split(new string[] { "\\" }, StringSplitOptions.None);

            if (str.Length <= 2)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);MessageBox.Show(treeNodePath + " is invalid. Please select a folder or files.", "Select folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string warningMessage = "Are you sure you want to delete selected ";

            if (lvFiles.SelectedItems.Count > 0)
            {
                warningMessage += "files?";
            }
            else
            {
                warningMessage += "folder:" + treeNodePath + "?";
            }

            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            if (MessageBox.Show(warningMessage, "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
            {
                return;
            }

            if (lvFiles.SelectedItems.Count > 0)
            {
                CloudProvider.UpdataStatusDlgt updateStatusDlgt = new CloudProvider.UpdataStatusDlgt(UpdateStatus);
                CloudWorker cloudWorker = new CloudWorker(updateStatusDlgt);

                foreach (ListViewItem lvItem in lvFiles.SelectedItems)
                {
                    FileEntry fileEntry = (FileEntry)lvItem.Tag;
                    await cloudWorker.DeleteFileAsync(treeNodePath, fileEntry.FileName);                    
                }
            }
            else
            {
                await DeleteDirectoryAsync(treeNodePath);
                currentNode = currentNode.Parent;
               
            }
        }

        private async void toolStripButton_Delete_Click(object sender, EventArgs e)
        {
            await DeleteFilesAsync();
            Refresh(true);
        }

        private void toolStripButton_CreateStubFile_Click(object sender, EventArgs e)
        {
            CreateStubFile();
        }


        private void toolStripButton_Status_Click(object sender, EventArgs e)
        {
        }

        private async Task RenameDirectoryAsync(string treeNodeFullPath,string sourcePath,string destPath)
        {
            try
            {
                CloudProvider.UpdataStatusDlgt updateStatusDlgt = new CloudProvider.UpdataStatusDlgt(UpdateStatus);
                CloudWorker cloudWorker = new CloudWorker(updateStatusDlgt);

                string localFullPath = string.Empty;
                SiteInfo currentSiteInfo = null;

                if (!cloudWorker.GetLocalFullPathByTreeNodeFullPath(treeNodeFullPath, out localFullPath, out currentSiteInfo))
                {
                    return;
                }


                CloudProvider cloudProvider = CloudBroker.GetProvider(currentSiteInfo,updateStatusDlgt);
                if (cloudProvider != null)
                {

                    Task<DirectoryList> taskDownloadFileList = cloudProvider.DownloadFileListAsync(localFullPath, false);

                    List<FileEntry> dirInfos = taskDownloadFileList.Result.FolderList;
                    List<FileEntry> fileInfos = taskDownloadFileList.Result.FileList;

                    foreach (FileEntry fileEntry in dirInfos)
                    {
                        string subFolder = treeNodeFullPath + "\\" + fileEntry.FileName;
                        string subSourcePath = sourcePath + "/" + fileEntry.FileName;
                        string subDestPath = destPath + "/" + fileEntry.FileName;

                        await RenameDirectoryAsync(subFolder, subSourcePath, subDestPath);

                    }

                    foreach (FileEntry fileEntry in fileInfos)
                    {
                        string sourceName = sourcePath + "/" + fileEntry.FileName;
                        string destName = destPath + "/" + fileEntry.FileName;
                        await cloudWorker.RenameFileAsync(currentSiteInfo, sourceName, destName);
                    }

                    if (fileInfos.Count == 0 && dirInfos.Count == 0)
                    {
                        //this is empty folder, create a new empty one 
                        await cloudWorker.MakeDirAsync(treeNodeFullPath,destPath);
                    }

                    //need to test if the folder was created as a file at the beginning, then we need to delete it here.
                    await cloudWorker.DeleteFileAsync(treeNodeFullPath,"");

                }
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(773, "RenameDirectoryAsync", EventLevel.Error, "RenameDirectoryAsync " + treeNodeFullPath + " failed with error:" + ex.Message);
            }
        }

        private async Task  RenameFileAsync()
        {
            try
            {
                string treeNodeFullPath = currentNode.FullPath;
                string destFileName = "newName";

                ListViewItem lvItem = lvFiles.SelectedItems[0];
                FileEntry fileEntry = (FileEntry)lvItem.Tag;
                string sourceName = fileEntry.FileName;

                SiteInfo currentSiteInfo = null;

                CloudProvider.UpdataStatusDlgt updateStatusDlgt = new CloudProvider.UpdataStatusDlgt(UpdateStatus);
                CloudWorker cloudWorker = new CloudWorker(updateStatusDlgt);

                if (!cloudWorker.GetSiteInfo(treeNodeFullPath, ref currentSiteInfo))
                {
                    return;
                }

                string remotePath = treeNodeFullPath.Replace(currentSiteInfo.CloudProvider.ToString() + "\\" + currentSiteInfo.SiteName, "");
                if (remotePath.StartsWith("\\") || remotePath.StartsWith("/"))
                {
                    remotePath = remotePath.Substring(1);
                }

                remotePath = Path.Combine(currentSiteInfo.RemotePath, remotePath);
                sourceName = Path.Combine(remotePath, sourceName).Replace("\\", "/");
                destFileName = Path.Combine(remotePath, destFileName).Replace("\\", "/");

                InputForm form = new InputForm("Enter New Path", "New name:", destFileName);
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    destFileName = form.InputText;
                }
                else
                {
                    return;
                }

                await cloudWorker.RenameFileAsync(currentSiteInfo, sourceName, destFileName);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);MessageBox.Show("Rename error:" + ex.Message, "Rename", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void toolStripButton_Rename_Click(object sender, EventArgs e)
        {
            TreeNode tn = this.tvFolders.SelectedNode;

            if (tn == null || lvFiles.SelectedItems.Count != 1)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show(" Please select a folder or file.", "Select folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string treeNodePath = currentNode.FullPath;

            string warningMessage = "Are you sure you want to rename the selected ";

            if (lvFiles.SelectedItems.Count > 0)
            {
                warningMessage += "file?";
            }
            else
            {
                warningMessage += "folder " + treeNodePath + "?";
            }

            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            if (MessageBox.Show(warningMessage, "Rename", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
            {
                return;
            }


            if (lvFiles.SelectedItems.Count > 0)
            {               
                Task renameTask = RenameFileAsync();
            }
            else
            {
                string sourcePath = string.Empty;
                string destPath = "newName";
                SiteInfo currentSiteInfo = null;

                CloudProvider.UpdataStatusDlgt updateStatusDlgt = new CloudProvider.UpdataStatusDlgt(UpdateStatus);
                CloudWorker cloudWorker = new CloudWorker(updateStatusDlgt);

                if (!cloudWorker.GetSiteInfo(treeNodePath, ref currentSiteInfo))
                {
                    return;
                }

                string remotePath = treeNodePath.Replace(currentSiteInfo.CloudProvider.ToString() + "\\" + currentSiteInfo.SiteName, "");
                if (remotePath.StartsWith("\\") || remotePath.StartsWith("/"))
                {
                    remotePath = remotePath.Substring(1);
                }

                remotePath = Path.Combine(currentSiteInfo.RemotePath, remotePath);
                sourcePath = remotePath.Replace("\\", "/");
                destPath = Path.Combine(Path.GetDirectoryName(remotePath), destPath).Replace("\\", "/");

                InputForm form = new InputForm("Enter New Path", "New path:", destPath);
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    destPath = form.InputText;
                }
                else
                {
                    return;
                }

                await RenameDirectoryAsync(treeNodePath, sourcePath, destPath );
                currentNode = currentNode.Parent;

                Refresh(true);
            }
        }

        private async void toolStripButton_New_Click(object sender, EventArgs e)
        {
            TreeNode tn = this.tvFolders.SelectedNode;

            try
            {

                if (!tvFolders.Focused || tn == null)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);MessageBox.Show("Please select the site name or folder name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {

                    string fullpath = tn.FullPath;
                    string[] paths = fullpath.Split(new char[] { '\\' });

                    if (paths.Length < 2)
                    {
                        MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);MessageBox.Show("Please select the site node.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    CloudProviderType cloudType = (CloudProviderType)Enum.Parse(typeof(CloudProviderType), paths[0]);


                    string promptStr = "";

                    if (paths.Length == 2)
                    {
                        if (cloudType == CloudProviderType.Amazon_S3)
                        {
                            promptStr = "Enter the bucket name";
                        }
                        else if (cloudType == CloudProviderType.AzureStorage)
                        {
                            promptStr = "Enter the container name";
                        }
                    }
                    else
                    {
                        promptStr = "Enter the new directory name";
                    }

                    InputForm form = new InputForm(promptStr, promptStr, "");

                    string dirName = string.Empty;
                    if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        dirName = form.InputText;

                        if (dirName.Length > 0)
                        {

                            CloudProvider.UpdataStatusDlgt updateStatusDlgt = new CloudProvider.UpdataStatusDlgt(UpdateStatus);
                            CloudWorker cloudWorker = new CloudWorker(updateStatusDlgt);

                            await cloudWorker.MakeDirAsync(fullpath, dirName);

                        }
                        else
                        {
                            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);MessageBox.Show("new directory name can't be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                Refresh(true);

            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);MessageBox.Show("Create new name error:" + ex.Message, "Create", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void CloudExplorer_FormClosed(object sender, FormClosedEventArgs e)
        {
            Cleanup();           
        }

        private void MessageEventHandler(MessageEventArgs message)
        {
            if(message.Type <= EventLevel.Warning )
            {
                UpdateStatus(message.Message, true);
            }
        }
        private void UpdateStatus(string message,bool isErrorMessage)
        {
            if (statusStrip1.InvokeRequired)
            {
                this.statusStrip1.Invoke(new CloudProvider.UpdataStatusDlgt(UpdateStatus), new Object[] { message, isErrorMessage });
            }
            else
            {
                if (isErrorMessage)
                {
                    explorerStatus.ForeColor = Color.Red;
                }
                else
                {
                    explorerStatus.ForeColor = Color.Black;
                }

                explorerStatus.Text = message;
             
            }
        }

        private async void deleteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
           await DeleteFilesAsync();
            Refresh(true);
        }

        private async void renameFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await RenameFileAsync();
            Refresh(true);
        }

        private void toolStripButton_Settings_Click(object sender, EventArgs e)
        {
            SiteManagerForm siteManager = new SiteManagerForm();
            siteManager.StartPosition = FormStartPosition.CenterParent;
            siteManager.ShowDialog();
        }

      
        private void CreateStubFile()
        {
            try
            {
                TreeNode tn = this.tvFolders.SelectedNode;

                if (null == tn || lvFiles.SelectedItems.Count == 0)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show(" Please select the files.", "Select files", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string treeNodePath = currentNode.FullPath;
                List<string> selectedFiles = new List<string>();
                string destinationFolder = GlobalConfig.cloudStubFileFolder;

                destinationFolder += "\\" + treeNodePath;

                InputForm inputForm = new InputForm("StubFile Path", "Stub file path:", destinationFolder);
                if (inputForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    destinationFolder = inputForm.InputText;

                    if(!Directory.Exists(destinationFolder))
                    {
                        Directory.CreateDirectory(destinationFolder);
                    }
                }
                else
                {
                    return;
                }

                SiteInfo currentSiteInfo = null;
                CloudWorker cloudWorker = new CloudWorker(null);
                if (!cloudWorker.GetSiteInfo(treeNodePath, ref currentSiteInfo))
                {
                    return;
                }

                foreach (ListViewItem lvItem in lvFiles.SelectedItems)
                {
                    FileEntry fileEntry = (FileEntry)lvItem.Tag;
                    long fileSize = fileEntry.FileSize;
                    string fileName = fileEntry.FileName;
                    uint fileAttributes = fileEntry.FileAttributes;

                    string remotePath = treeNodePath.Replace(currentSiteInfo.CloudProvider.ToString() + "\\" + currentSiteInfo.SiteName, "");
                    if (remotePath.StartsWith("\\") || remotePath.StartsWith("/"))
                    {
                        remotePath = remotePath.Substring(1);
                    }

                    remotePath = Path.Combine(remotePath, fileName);
                    remotePath = remotePath.Replace("\\", "/");
                    string stubFileName = Path.Combine(destinationFolder, Path.GetFileName(fileName));

                    string fileNameInTagData = "sitename:" + currentSiteInfo.SiteName + ";" + remotePath;
                    byte[] tagData = System.Text.ASCIIEncoding.Unicode.GetBytes(fileNameInTagData);

                    IntPtr fileHandle = IntPtr.Zero;
                    GCHandle gcHandle = GCHandle.Alloc(tagData, GCHandleType.Pinned);

                    try
                    {
                        bool ret = FilterAPI.CreateStubFile(stubFileName, fileSize, fileAttributes, (uint)tagData.Length, Marshal.UnsafeAddrOfPinnedArrayElement(tagData, 0), true, ref fileHandle);
                        if (!ret)
                        {
                            MessageBox.Show("Create cloud stub file:" + fileName + " failed.\n" + FilterAPI.GetLastErrorMessage()
                                , "CreateStubFile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        UpdateStatus("Created stub file " + stubFileName + " succeeded.", false);
                    }
                    finally
                    {
                        gcHandle.Free();

                        if (fileHandle != IntPtr.Zero)
                        {
                            FilterAPI.CloseHandle(fileHandle);
                        }
                    }
                }

                MessageBox.Show("Total " + lvFiles.SelectedItems.Count + " cloud stub file was created.");
            }
            catch(Exception ex)
            {
                EventManager.WriteMessage(1200, "CreateStubFile", EventLevel.Error, "Create cloud stub file failed with error:" + ex.Message);
            }

        }

        private void createStubFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateStubFile();
        }

        private void toolStripButton_Help_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.easefilter.com/Forums_Files/CloudExplorer.htm");
        }

    }
}
