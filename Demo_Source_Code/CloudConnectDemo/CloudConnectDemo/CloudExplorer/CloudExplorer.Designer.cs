using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Management;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;

namespace EaseFilter.CloudExplorer
{
     partial class CloudExplorer 
    {
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.TreeView tvFolders;
        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.ImageList m_imageListTreeView;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem downloadFilesToolStripMenuItem;
        private ToolStripMenuItem restoreStubFilesToolStripMenuItem;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButton_Refresh;
        private ToolStripButton toolStripButton_Upload;
        private ToolStripButton toolStripButton_Download;
        private StatusStrip statusStrip1;
        private System.ComponentModel.IContainer components;

        private string currentFile = string.Empty;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton toolStripButton_Delete;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem renameFileToolStripMenuItem;
        private ToolStripMenuItem deleteFileToolStripMenuItem;
        private ToolStripStatusLabel explorerStatus;
        private TreeNode currentNode = null;
        private ToolStripButton toolStripButton_Rename;
        private ToolStripButton toolStripButton_New;

        private SystemImageList imageListDocuments;

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CloudExplorer));
            this.tvFolders = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.downloadFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreStubFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createStubFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_imageListTreeView = new System.Windows.Forms.ImageList(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.lvFiles = new System.Windows.Forms.ListView();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_Refresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_New = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_Upload = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Download = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Rename = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_Delete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_StubFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Settings = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Help = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.explorerStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvFolders
            // 
            this.tvFolders.ContextMenuStrip = this.contextMenuStrip1;
            this.tvFolders.Dock = System.Windows.Forms.DockStyle.Left;
            this.tvFolders.ImageIndex = 0;
            this.tvFolders.ImageList = this.m_imageListTreeView;
            this.tvFolders.Location = new System.Drawing.Point(0, 27);
            this.tvFolders.Name = "tvFolders";
            this.tvFolders.SelectedImageIndex = 0;
            this.tvFolders.Size = new System.Drawing.Size(264, 561);
            this.tvFolders.TabIndex = 2;
            this.tvFolders.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFolders_AfterSelect);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadFilesToolStripMenuItem,
            this.restoreStubFilesToolStripMenuItem,
            this.renameFileToolStripMenuItem,
            this.deleteFileToolStripMenuItem,
            this.createStubFileToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(226, 152);
            // 
            // downloadFilesToolStripMenuItem
            // 
            this.downloadFilesToolStripMenuItem.Name = "downloadFilesToolStripMenuItem";
            this.downloadFilesToolStripMenuItem.Size = new System.Drawing.Size(225, 24);
            this.downloadFilesToolStripMenuItem.Text = "Download file";
            this.downloadFilesToolStripMenuItem.Click += new System.EventHandler(this.downloadFilesToolStripMenuItem_Click);
            // 
            // restoreStubFilesToolStripMenuItem
            // 
            this.restoreStubFilesToolStripMenuItem.Name = "restoreStubFilesToolStripMenuItem";
            this.restoreStubFilesToolStripMenuItem.Size = new System.Drawing.Size(225, 24);
            this.restoreStubFilesToolStripMenuItem.Text = "Upload files";
            this.restoreStubFilesToolStripMenuItem.Click += new System.EventHandler(this.restoreStubFilesToolStripMenuItem_Click);
            // 
            // renameFileToolStripMenuItem
            // 
            this.renameFileToolStripMenuItem.Name = "renameFileToolStripMenuItem";
            this.renameFileToolStripMenuItem.Size = new System.Drawing.Size(225, 24);
            this.renameFileToolStripMenuItem.Text = "Rename file";
            this.renameFileToolStripMenuItem.Click += new System.EventHandler(this.renameFileToolStripMenuItem_Click);
            // 
            // deleteFileToolStripMenuItem
            // 
            this.deleteFileToolStripMenuItem.Name = "deleteFileToolStripMenuItem";
            this.deleteFileToolStripMenuItem.Size = new System.Drawing.Size(225, 24);
            this.deleteFileToolStripMenuItem.Text = "Delete file";
            this.deleteFileToolStripMenuItem.Click += new System.EventHandler(this.deleteFileToolStripMenuItem_Click);
            // 
            // createStubFileToolStripMenuItem
            // 
            this.createStubFileToolStripMenuItem.Name = "createStubFileToolStripMenuItem";
            this.createStubFileToolStripMenuItem.Size = new System.Drawing.Size(225, 24);
            this.createStubFileToolStripMenuItem.Text = "Create Cloud Stub File";
            this.createStubFileToolStripMenuItem.Click += new System.EventHandler(this.createStubFileToolStripMenuItem_Click);
            // 
            // m_imageListTreeView
            // 
            this.m_imageListTreeView.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_imageListTreeView.ImageStream")));
            this.m_imageListTreeView.TransparentColor = System.Drawing.Color.Transparent;
            this.m_imageListTreeView.Images.SetKeyName(0, "folders.png");
            this.m_imageListTreeView.Images.SetKeyName(1, "folder_preferences.png");
            this.m_imageListTreeView.Images.SetKeyName(2, "");
            this.m_imageListTreeView.Images.SetKeyName(3, "");
            this.m_imageListTreeView.Images.SetKeyName(4, "");
            this.m_imageListTreeView.Images.SetKeyName(5, "");
            this.m_imageListTreeView.Images.SetKeyName(6, "");
            this.m_imageListTreeView.Images.SetKeyName(7, "");
            this.m_imageListTreeView.Images.SetKeyName(8, "");
            this.m_imageListTreeView.Images.SetKeyName(9, "environment_network.png");
            this.m_imageListTreeView.Images.SetKeyName(10, "environment_ok.png");
            this.m_imageListTreeView.Images.SetKeyName(11, "folder_ok.png");
            this.m_imageListTreeView.Images.SetKeyName(12, "");
            this.m_imageListTreeView.Images.SetKeyName(13, "");
            this.m_imageListTreeView.Images.SetKeyName(14, "cloudservice.ico");
            this.m_imageListTreeView.Images.SetKeyName(15, "text.png");
            this.m_imageListTreeView.Images.SetKeyName(16, "text_ok.png");
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(928, 27);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 561);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // lvFiles
            // 
            this.lvFiles.AllowColumnReorder = true;
            this.lvFiles.ContextMenuStrip = this.contextMenuStrip1;
            this.lvFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFiles.Location = new System.Drawing.Point(264, 27);
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(664, 561);
            this.lvFiles.TabIndex = 4;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            this.lvFiles.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvFiles_ColumnClick);
            this.lvFiles.ItemActivate += new System.EventHandler(this.lvFiles_ItemActivate);
            this.lvFiles.SelectedIndexChanged += new System.EventHandler(this.lvFiles_SelectedIndexChanged);
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem3});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem2});
            this.menuItem1.Text = "&File";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 0;
            this.menuItem2.Text = "&Close";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 1;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem4});
            this.menuItem3.Text = "&Help";
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 0;
            this.menuItem4.Text = "&About";
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_Refresh,
            this.toolStripSeparator1,
            this.toolStripButton_New,
            this.toolStripButton_Upload,
            this.toolStripSeparator2,
            this.toolStripButton_Download,
            this.toolStripSeparator3,
            this.toolStripButton_Rename,
            this.toolStripButton_Delete,
            this.toolStripSeparator4,
            this.toolStripButton_StubFile,
            this.toolStripSeparator5,
            this.toolStripButton_Settings,
            this.toolStripSeparator6,
            this.toolStripButton_Help});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(931, 27);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton_Refresh
            // 
            this.toolStripButton_Refresh.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Refresh.Image")));
            this.toolStripButton_Refresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Refresh.Name = "toolStripButton_Refresh";
            this.toolStripButton_Refresh.Size = new System.Drawing.Size(82, 24);
            this.toolStripButton_Refresh.Text = "Refresh";
            this.toolStripButton_Refresh.Click += new System.EventHandler(this.toolStripButton_Refresh_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButton_New
            // 
            this.toolStripButton_New.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_New.Image")));
            this.toolStripButton_New.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_New.Name = "toolStripButton_New";
            this.toolStripButton_New.Size = new System.Drawing.Size(63, 24);
            this.toolStripButton_New.Text = "New";
            this.toolStripButton_New.Click += new System.EventHandler(this.toolStripButton_New_Click);
            // 
            // toolStripButton_Upload
            // 
            this.toolStripButton_Upload.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Upload.Image")));
            this.toolStripButton_Upload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Upload.Name = "toolStripButton_Upload";
            this.toolStripButton_Upload.Size = new System.Drawing.Size(82, 24);
            this.toolStripButton_Upload.Text = "Upload";
            this.toolStripButton_Upload.Click += new System.EventHandler(this.toolStripButton_UploadFiles_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButton_Download
            // 
            this.toolStripButton_Download.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Download.Image")));
            this.toolStripButton_Download.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Download.Name = "toolStripButton_Download";
            this.toolStripButton_Download.Size = new System.Drawing.Size(102, 24);
            this.toolStripButton_Download.Text = "Download";
            this.toolStripButton_Download.Click += new System.EventHandler(this.toolStripButton_Download_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButton_Rename
            // 
            this.toolStripButton_Rename.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Rename.Image")));
            this.toolStripButton_Rename.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Rename.Name = "toolStripButton_Rename";
            this.toolStripButton_Rename.Size = new System.Drawing.Size(87, 24);
            this.toolStripButton_Rename.Text = "Rename";
            this.toolStripButton_Rename.Click += new System.EventHandler(this.toolStripButton_Rename_Click);
            // 
            // toolStripButton_Delete
            // 
            this.toolStripButton_Delete.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Delete.Image")));
            this.toolStripButton_Delete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Delete.Name = "toolStripButton_Delete";
            this.toolStripButton_Delete.Size = new System.Drawing.Size(77, 24);
            this.toolStripButton_Delete.Text = "Delete";
            this.toolStripButton_Delete.Click += new System.EventHandler(this.toolStripButton_Delete_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButton_StubFile
            // 
            this.toolStripButton_StubFile.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_StubFile.Image")));
            this.toolStripButton_StubFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_StubFile.Name = "toolStripButton_StubFile";
            this.toolStripButton_StubFile.Size = new System.Drawing.Size(137, 24);
            this.toolStripButton_StubFile.Text = "Create Stub File";
            this.toolStripButton_StubFile.Click += new System.EventHandler(this.toolStripButton_CreateStubFile_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButton_Settings
            // 
            this.toolStripButton_Settings.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Settings.Image")));
            this.toolStripButton_Settings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Settings.Name = "toolStripButton_Settings";
            this.toolStripButton_Settings.Size = new System.Drawing.Size(86, 24);
            this.toolStripButton_Settings.Text = "Settings";
            this.toolStripButton_Settings.Click += new System.EventHandler(this.toolStripButton_Settings_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButton_Help
            // 
            this.toolStripButton_Help.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Help.Image")));
            this.toolStripButton_Help.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Help.Name = "toolStripButton_Help";
            this.toolStripButton_Help.Size = new System.Drawing.Size(65, 24);
            this.toolStripButton_Help.Text = "Help";
            this.toolStripButton_Help.Click += new System.EventHandler(this.toolStripButton_Help_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.explorerStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 588);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(931, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // explorerStatus
            // 
            this.explorerStatus.Name = "explorerStatus";
            this.explorerStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // CloudExplorer
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(931, 610);
            this.Controls.Add(this.lvFiles);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.tvFolders);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CloudExplorer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CloudTier Cloud File Explorer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CloudExplorer_FormClosed);
            this.contextMenuStrip1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private ToolStripButton toolStripButton_Settings;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripButton toolStripButton_Help;
        private ToolStripButton toolStripButton_StubFile;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem createStubFileToolStripMenuItem;
    }
}
