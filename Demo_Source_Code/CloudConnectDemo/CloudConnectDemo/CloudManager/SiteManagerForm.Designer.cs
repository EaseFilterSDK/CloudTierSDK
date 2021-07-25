namespace EaseFilter.CloudManager
{
    partial class SiteManagerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Node0");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Cloud Sites", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SiteManagerForm));
            this.treeView_SiteList = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tabControl_SiteSetting = new System.Windows.Forms.TabControl();
            this.tabPage_GeneralSetting = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_DeleteCachedFiles = new System.Windows.Forms.Button();
            this.textBox_DeleteCachedFilesTTL = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.comboBox_EventOutputType = new System.Windows.Forms.ComboBox();
            this.checkBox_ClearCacheOnConnect = new System.Windows.Forms.CheckBox();
            this.button_BrowseCacheFolder = new System.Windows.Forms.Button();
            this.textBox_ExpireCacheFileTTL = new System.Windows.Forms.TextBox();
            this.textBox_CacheFolder = new System.Windows.Forms.TextBox();
            this.comboBox_EventLevel = new System.Windows.Forms.ComboBox();
            this.label42 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.tabPage_AmazonS3 = new System.Windows.Forms.TabPage();
            this.groupBox_S3 = new System.Windows.Forms.GroupBox();
            this.checkBox_S3EnableParallelDownload = new System.Windows.Forms.CheckBox();
            this.textBox_S3ParallelTasks = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.textBox_S3BufferSize = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.checkBox_S3EnableMultiPartsUpload = new System.Windows.Forms.CheckBox();
            this.textBox_S3PartSize = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.comboBox_S3Region = new System.Windows.Forms.ComboBox();
            this.label21 = new System.Windows.Forms.Label();
            this.textBox_S3SecretKey = new System.Windows.Forms.TextBox();
            this.textBox_S3AccessKeyId = new System.Windows.Forms.TextBox();
            this.textBox_S3SiteName = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.tabPage_Azure = new System.Windows.Forms.TabPage();
            this.groupBox_Azure = new System.Windows.Forms.GroupBox();
            this.checkBox_AzureEnableParallelDownload = new System.Windows.Forms.CheckBox();
            this.textBox_AzureParallelTasks = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox_AzureMd5Verification = new System.Windows.Forms.CheckBox();
            this.textBox_AzureBufferSize = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox_AzureEnableMultiBlocksUpload = new System.Windows.Forms.CheckBox();
            this.textBox_AzureBlobSize = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_AzureConnectionString = new System.Windows.Forms.TextBox();
            this.textBox_AzureSiteName = new System.Windows.Forms.TextBox();
            this.label39 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.folderBrowserDialog_LocalPath = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button_Delete = new System.Windows.Forms.Button();
            this.button_CopyItem = new System.Windows.Forms.Button();
            this.button_AddSite = new System.Windows.Forms.Button();
            this.button_TestConnection = new System.Windows.Forms.Button();
            this.button_Apply = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.tabControl_SiteSetting.SuspendLayout();
            this.tabPage_GeneralSetting.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage_AmazonS3.SuspendLayout();
            this.groupBox_S3.SuspendLayout();
            this.tabPage_Azure.SuspendLayout();
            this.groupBox_Azure.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView_SiteList
            // 
            this.treeView_SiteList.ImageIndex = 1;
            this.treeView_SiteList.ImageList = this.imageList1;
            this.treeView_SiteList.Location = new System.Drawing.Point(31, 22);
            this.treeView_SiteList.Name = "treeView_SiteList";
            treeNode1.ImageIndex = 0;
            treeNode1.Name = "Node0";
            treeNode1.Text = "Node0";
            treeNode2.Name = "Cloud Sites";
            treeNode2.Text = "Cloud Sites";
            this.treeView_SiteList.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
            this.treeView_SiteList.SelectedImageIndex = 0;
            this.treeView_SiteList.Size = new System.Drawing.Size(252, 346);
            this.treeView_SiteList.TabIndex = 1;
            this.treeView_SiteList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_SiteList_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folders.png");
            this.imageList1.Images.SetKeyName(1, "folder_ok.png");
            this.imageList1.Images.SetKeyName(2, "environment_network.png");
            this.imageList1.Images.SetKeyName(3, "environment_ok.png");
            this.imageList1.Images.SetKeyName(4, "monitor.png");
            this.imageList1.Images.SetKeyName(5, "monitor_preferences.png");
            this.imageList1.Images.SetKeyName(6, "workstation_network.png");
            this.imageList1.Images.SetKeyName(7, "clients.png");
            this.imageList1.Images.SetKeyName(8, "folder_network.png");
            // 
            // tabControl_SiteSetting
            // 
            this.tabControl_SiteSetting.Controls.Add(this.tabPage_GeneralSetting);
            this.tabControl_SiteSetting.Controls.Add(this.tabPage_AmazonS3);
            this.tabControl_SiteSetting.Controls.Add(this.tabPage_Azure);
            this.tabControl_SiteSetting.Location = new System.Drawing.Point(308, 12);
            this.tabControl_SiteSetting.Name = "tabControl_SiteSetting";
            this.tabControl_SiteSetting.SelectedIndex = 0;
            this.tabControl_SiteSetting.Size = new System.Drawing.Size(549, 356);
            this.tabControl_SiteSetting.TabIndex = 3;
            // 
            // tabPage_GeneralSetting
            // 
            this.tabPage_GeneralSetting.Controls.Add(this.groupBox2);
            this.tabPage_GeneralSetting.Location = new System.Drawing.Point(4, 22);
            this.tabPage_GeneralSetting.Name = "tabPage_GeneralSetting";
            this.tabPage_GeneralSetting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_GeneralSetting.Size = new System.Drawing.Size(541, 330);
            this.tabPage_GeneralSetting.TabIndex = 2;
            this.tabPage_GeneralSetting.Text = "General";
            this.tabPage_GeneralSetting.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_DeleteCachedFiles);
            this.groupBox2.Controls.Add(this.textBox_DeleteCachedFilesTTL);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Controls.Add(this.comboBox_EventOutputType);
            this.groupBox2.Controls.Add(this.checkBox_ClearCacheOnConnect);
            this.groupBox2.Controls.Add(this.button_BrowseCacheFolder);
            this.groupBox2.Controls.Add(this.textBox_ExpireCacheFileTTL);
            this.groupBox2.Controls.Add(this.textBox_CacheFolder);
            this.groupBox2.Controls.Add(this.comboBox_EventLevel);
            this.groupBox2.Controls.Add(this.label42);
            this.groupBox2.Controls.Add(this.label38);
            this.groupBox2.Controls.Add(this.label37);
            this.groupBox2.Controls.Add(this.label31);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(535, 324);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // button_DeleteCachedFiles
            // 
            this.button_DeleteCachedFiles.Location = new System.Drawing.Point(216, 244);
            this.button_DeleteCachedFiles.Name = "button_DeleteCachedFiles";
            this.button_DeleteCachedFiles.Size = new System.Drawing.Size(130, 23);
            this.button_DeleteCachedFiles.TabIndex = 40;
            this.button_DeleteCachedFiles.Text = "Delete Cached Data";
            this.button_DeleteCachedFiles.UseVisualStyleBackColor = true;
            // 
            // textBox_DeleteCachedFilesTTL
            // 
            this.textBox_DeleteCachedFilesTTL.Location = new System.Drawing.Point(217, 134);
            this.textBox_DeleteCachedFilesTTL.Name = "textBox_DeleteCachedFilesTTL";
            this.textBox_DeleteCachedFilesTTL.Size = new System.Drawing.Size(281, 20);
            this.textBox_DeleteCachedFilesTTL.TabIndex = 38;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(19, 134);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(201, 15);
            this.label22.TabIndex = 39;
            this.label22.Text = "Delete cached files after(x) seconds";
            // 
            // comboBox_EventOutputType
            // 
            this.comboBox_EventOutputType.FormattingEnabled = true;
            this.comboBox_EventOutputType.Location = new System.Drawing.Point(217, 40);
            this.comboBox_EventOutputType.Name = "comboBox_EventOutputType";
            this.comboBox_EventOutputType.Size = new System.Drawing.Size(281, 21);
            this.comboBox_EventOutputType.TabIndex = 37;
            // 
            // checkBox_ClearCacheOnConnect
            // 
            this.checkBox_ClearCacheOnConnect.AutoSize = true;
            this.checkBox_ClearCacheOnConnect.Location = new System.Drawing.Point(217, 212);
            this.checkBox_ClearCacheOnConnect.Name = "checkBox_ClearCacheOnConnect";
            this.checkBox_ClearCacheOnConnect.Size = new System.Drawing.Size(191, 19);
            this.checkBox_ClearCacheOnConnect.TabIndex = 33;
            this.checkBox_ClearCacheOnConnect.Text = "Clear cached data on connect";
            this.checkBox_ClearCacheOnConnect.UseVisualStyleBackColor = true;
            // 
            // button_BrowseCacheFolder
            // 
            this.button_BrowseCacheFolder.Location = new System.Drawing.Point(504, 70);
            this.button_BrowseCacheFolder.Name = "button_BrowseCacheFolder";
            this.button_BrowseCacheFolder.Size = new System.Drawing.Size(25, 21);
            this.button_BrowseCacheFolder.TabIndex = 22;
            this.button_BrowseCacheFolder.Text = "..";
            this.button_BrowseCacheFolder.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button_BrowseCacheFolder.UseVisualStyleBackColor = true;
            // 
            // textBox_ExpireCacheFileTTL
            // 
            this.textBox_ExpireCacheFileTTL.Location = new System.Drawing.Point(217, 104);
            this.textBox_ExpireCacheFileTTL.Name = "textBox_ExpireCacheFileTTL";
            this.textBox_ExpireCacheFileTTL.Size = new System.Drawing.Size(281, 20);
            this.textBox_ExpireCacheFileTTL.TabIndex = 5;
            // 
            // textBox_CacheFolder
            // 
            this.textBox_CacheFolder.Location = new System.Drawing.Point(217, 70);
            this.textBox_CacheFolder.Name = "textBox_CacheFolder";
            this.textBox_CacheFolder.Size = new System.Drawing.Size(281, 20);
            this.textBox_CacheFolder.TabIndex = 3;
            // 
            // comboBox_EventLevel
            // 
            this.comboBox_EventLevel.FormattingEnabled = true;
            this.comboBox_EventLevel.Location = new System.Drawing.Point(217, 10);
            this.comboBox_EventLevel.Name = "comboBox_EventLevel";
            this.comboBox_EventLevel.Size = new System.Drawing.Size(281, 21);
            this.comboBox_EventLevel.TabIndex = 1;
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(19, 104);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(210, 15);
            this.label42.TabIndex = 11;
            this.label42.Text = "Expire cached listing after(x) seconds";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(19, 40);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(99, 15);
            this.label38.TabIndex = 7;
            this.label38.Text = "Event output type";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(19, 70);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(103, 15);
            this.label37.TabIndex = 6;
            this.label37.Text = "Cache folder path";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(19, 10);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(65, 15);
            this.label31.TabIndex = 0;
            this.label31.Text = "Event level";
            // 
            // tabPage_AmazonS3
            // 
            this.tabPage_AmazonS3.Controls.Add(this.groupBox_S3);
            this.tabPage_AmazonS3.Location = new System.Drawing.Point(4, 22);
            this.tabPage_AmazonS3.Name = "tabPage_AmazonS3";
            this.tabPage_AmazonS3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_AmazonS3.Size = new System.Drawing.Size(541, 330);
            this.tabPage_AmazonS3.TabIndex = 4;
            this.tabPage_AmazonS3.Text = "AmazonS3";
            this.tabPage_AmazonS3.UseVisualStyleBackColor = true;
            // 
            // groupBox_S3
            // 
            this.groupBox_S3.Controls.Add(this.checkBox_S3EnableParallelDownload);
            this.groupBox_S3.Controls.Add(this.textBox_S3ParallelTasks);
            this.groupBox_S3.Controls.Add(this.label34);
            this.groupBox_S3.Controls.Add(this.textBox_S3BufferSize);
            this.groupBox_S3.Controls.Add(this.label27);
            this.groupBox_S3.Controls.Add(this.checkBox_S3EnableMultiPartsUpload);
            this.groupBox_S3.Controls.Add(this.textBox_S3PartSize);
            this.groupBox_S3.Controls.Add(this.label26);
            this.groupBox_S3.Controls.Add(this.comboBox_S3Region);
            this.groupBox_S3.Controls.Add(this.label21);
            this.groupBox_S3.Controls.Add(this.textBox_S3SecretKey);
            this.groupBox_S3.Controls.Add(this.textBox_S3AccessKeyId);
            this.groupBox_S3.Controls.Add(this.textBox_S3SiteName);
            this.groupBox_S3.Controls.Add(this.label18);
            this.groupBox_S3.Controls.Add(this.label19);
            this.groupBox_S3.Controls.Add(this.label20);
            this.groupBox_S3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_S3.Location = new System.Drawing.Point(3, 3);
            this.groupBox_S3.Name = "groupBox_S3";
            this.groupBox_S3.Size = new System.Drawing.Size(535, 324);
            this.groupBox_S3.TabIndex = 21;
            this.groupBox_S3.TabStop = false;
            // 
            // checkBox_S3EnableParallelDownload
            // 
            this.checkBox_S3EnableParallelDownload.AutoSize = true;
            this.checkBox_S3EnableParallelDownload.Location = new System.Drawing.Point(317, 217);
            this.checkBox_S3EnableParallelDownload.Name = "checkBox_S3EnableParallelDownload";
            this.checkBox_S3EnableParallelDownload.Size = new System.Drawing.Size(169, 19);
            this.checkBox_S3EnableParallelDownload.TabIndex = 35;
            this.checkBox_S3EnableParallelDownload.Text = "Enable parallel download";
            this.checkBox_S3EnableParallelDownload.UseVisualStyleBackColor = true;
            // 
            // textBox_S3ParallelTasks
            // 
            this.textBox_S3ParallelTasks.Location = new System.Drawing.Point(120, 216);
            this.textBox_S3ParallelTasks.Name = "textBox_S3ParallelTasks";
            this.textBox_S3ParallelTasks.Size = new System.Drawing.Size(191, 20);
            this.textBox_S3ParallelTasks.TabIndex = 34;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(10, 216);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(79, 15);
            this.label34.TabIndex = 33;
            this.label34.Text = "parallel tasks";
            // 
            // textBox_S3BufferSize
            // 
            this.textBox_S3BufferSize.Location = new System.Drawing.Point(120, 146);
            this.textBox_S3BufferSize.Name = "textBox_S3BufferSize";
            this.textBox_S3BufferSize.Size = new System.Drawing.Size(358, 20);
            this.textBox_S3BufferSize.TabIndex = 32;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(10, 146);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(100, 15);
            this.label27.TabIndex = 31;
            this.label27.Text = "Buffer size(bytes)";
            // 
            // checkBox_S3EnableMultiPartsUpload
            // 
            this.checkBox_S3EnableMultiPartsUpload.AutoSize = true;
            this.checkBox_S3EnableMultiPartsUpload.Location = new System.Drawing.Point(317, 178);
            this.checkBox_S3EnableMultiPartsUpload.Name = "checkBox_S3EnableMultiPartsUpload";
            this.checkBox_S3EnableMultiPartsUpload.Size = new System.Drawing.Size(186, 19);
            this.checkBox_S3EnableMultiPartsUpload.TabIndex = 30;
            this.checkBox_S3EnableMultiPartsUpload.Text = "Enable upload multiple parts";
            this.checkBox_S3EnableMultiPartsUpload.UseVisualStyleBackColor = true;
            // 
            // textBox_S3PartSize
            // 
            this.textBox_S3PartSize.Location = new System.Drawing.Point(120, 178);
            this.textBox_S3PartSize.Name = "textBox_S3PartSize";
            this.textBox_S3PartSize.Size = new System.Drawing.Size(191, 20);
            this.textBox_S3PartSize.TabIndex = 25;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(10, 176);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(90, 15);
            this.label26.TabIndex = 24;
            this.label26.Text = "Part size(bytes)";
            // 
            // comboBox_S3Region
            // 
            this.comboBox_S3Region.FormattingEnabled = true;
            this.comboBox_S3Region.Location = new System.Drawing.Point(120, 111);
            this.comboBox_S3Region.Name = "comboBox_S3Region";
            this.comboBox_S3Region.Size = new System.Drawing.Size(358, 21);
            this.comboBox_S3Region.TabIndex = 19;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(10, 111);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(82, 15);
            this.label21.TabIndex = 18;
            this.label21.Text = "Region name";
            // 
            // textBox_S3SecretKey
            // 
            this.textBox_S3SecretKey.Location = new System.Drawing.Point(120, 81);
            this.textBox_S3SecretKey.Name = "textBox_S3SecretKey";
            this.textBox_S3SecretKey.Size = new System.Drawing.Size(358, 20);
            this.textBox_S3SecretKey.TabIndex = 13;
            this.textBox_S3SecretKey.UseSystemPasswordChar = true;
            // 
            // textBox_S3AccessKeyId
            // 
            this.textBox_S3AccessKeyId.Location = new System.Drawing.Point(120, 51);
            this.textBox_S3AccessKeyId.Name = "textBox_S3AccessKeyId";
            this.textBox_S3AccessKeyId.Size = new System.Drawing.Size(358, 20);
            this.textBox_S3AccessKeyId.TabIndex = 12;
            // 
            // textBox_S3SiteName
            // 
            this.textBox_S3SiteName.Location = new System.Drawing.Point(120, 21);
            this.textBox_S3SiteName.Name = "textBox_S3SiteName";
            this.textBox_S3SiteName.Size = new System.Drawing.Size(358, 20);
            this.textBox_S3SiteName.TabIndex = 8;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(10, 81);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(104, 15);
            this.label18.TabIndex = 4;
            this.label18.Text = "Secret access key";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(10, 51);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(79, 15);
            this.label19.TabIndex = 3;
            this.label19.Text = "Access key id";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(10, 21);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(63, 15);
            this.label20.TabIndex = 0;
            this.label20.Text = "Site name";
            // 
            // tabPage_Azure
            // 
            this.tabPage_Azure.Controls.Add(this.groupBox_Azure);
            this.tabPage_Azure.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Azure.Name = "tabPage_Azure";
            this.tabPage_Azure.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Azure.Size = new System.Drawing.Size(541, 330);
            this.tabPage_Azure.TabIndex = 5;
            this.tabPage_Azure.Text = "Azure";
            this.tabPage_Azure.UseVisualStyleBackColor = true;
            // 
            // groupBox_Azure
            // 
            this.groupBox_Azure.Controls.Add(this.checkBox_AzureEnableParallelDownload);
            this.groupBox_Azure.Controls.Add(this.textBox_AzureParallelTasks);
            this.groupBox_Azure.Controls.Add(this.label1);
            this.groupBox_Azure.Controls.Add(this.checkBox_AzureMd5Verification);
            this.groupBox_Azure.Controls.Add(this.textBox_AzureBufferSize);
            this.groupBox_Azure.Controls.Add(this.label3);
            this.groupBox_Azure.Controls.Add(this.checkBox_AzureEnableMultiBlocksUpload);
            this.groupBox_Azure.Controls.Add(this.textBox_AzureBlobSize);
            this.groupBox_Azure.Controls.Add(this.label6);
            this.groupBox_Azure.Controls.Add(this.textBox_AzureConnectionString);
            this.groupBox_Azure.Controls.Add(this.textBox_AzureSiteName);
            this.groupBox_Azure.Controls.Add(this.label39);
            this.groupBox_Azure.Controls.Add(this.label40);
            this.groupBox_Azure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_Azure.Location = new System.Drawing.Point(3, 3);
            this.groupBox_Azure.Name = "groupBox_Azure";
            this.groupBox_Azure.Size = new System.Drawing.Size(535, 324);
            this.groupBox_Azure.TabIndex = 22;
            this.groupBox_Azure.TabStop = false;
            // 
            // checkBox_AzureEnableParallelDownload
            // 
            this.checkBox_AzureEnableParallelDownload.AutoSize = true;
            this.checkBox_AzureEnableParallelDownload.Location = new System.Drawing.Point(329, 170);
            this.checkBox_AzureEnableParallelDownload.Name = "checkBox_AzureEnableParallelDownload";
            this.checkBox_AzureEnableParallelDownload.Size = new System.Drawing.Size(169, 19);
            this.checkBox_AzureEnableParallelDownload.TabIndex = 54;
            this.checkBox_AzureEnableParallelDownload.Text = "Enable parallel download";
            this.checkBox_AzureEnableParallelDownload.UseVisualStyleBackColor = true;
            // 
            // textBox_AzureParallelTasks
            // 
            this.textBox_AzureParallelTasks.Location = new System.Drawing.Point(149, 169);
            this.textBox_AzureParallelTasks.Name = "textBox_AzureParallelTasks";
            this.textBox_AzureParallelTasks.Size = new System.Drawing.Size(174, 20);
            this.textBox_AzureParallelTasks.TabIndex = 53;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 169);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 15);
            this.label1.TabIndex = 52;
            this.label1.Text = "parallel tasks";
            // 
            // checkBox_AzureMd5Verification
            // 
            this.checkBox_AzureMd5Verification.AutoSize = true;
            this.checkBox_AzureMd5Verification.Location = new System.Drawing.Point(149, 419);
            this.checkBox_AzureMd5Verification.Name = "checkBox_AzureMd5Verification";
            this.checkBox_AzureMd5Verification.Size = new System.Drawing.Size(216, 19);
            this.checkBox_AzureMd5Verification.TabIndex = 49;
            this.checkBox_AzureMd5Verification.Text = "Enable download MD5 verification";
            this.checkBox_AzureMd5Verification.UseVisualStyleBackColor = true;
            // 
            // textBox_AzureBufferSize
            // 
            this.textBox_AzureBufferSize.Location = new System.Drawing.Point(149, 93);
            this.textBox_AzureBufferSize.Name = "textBox_AzureBufferSize";
            this.textBox_AzureBufferSize.Size = new System.Drawing.Size(329, 20);
            this.textBox_AzureBufferSize.TabIndex = 32;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 15);
            this.label3.TabIndex = 31;
            this.label3.Text = "Buffer size(bytes)";
            // 
            // checkBox_AzureEnableMultiBlocksUpload
            // 
            this.checkBox_AzureEnableMultiBlocksUpload.AutoSize = true;
            this.checkBox_AzureEnableMultiBlocksUpload.Location = new System.Drawing.Point(329, 131);
            this.checkBox_AzureEnableMultiBlocksUpload.Name = "checkBox_AzureEnableMultiBlocksUpload";
            this.checkBox_AzureEnableMultiBlocksUpload.Size = new System.Drawing.Size(189, 19);
            this.checkBox_AzureEnableMultiBlocksUpload.TabIndex = 30;
            this.checkBox_AzureEnableMultiBlocksUpload.Text = "Enable upload multiple blobs";
            this.checkBox_AzureEnableMultiBlocksUpload.UseVisualStyleBackColor = true;
            // 
            // textBox_AzureBlobSize
            // 
            this.textBox_AzureBlobSize.Location = new System.Drawing.Point(149, 131);
            this.textBox_AzureBlobSize.Name = "textBox_AzureBlobSize";
            this.textBox_AzureBlobSize.Size = new System.Drawing.Size(174, 20);
            this.textBox_AzureBlobSize.TabIndex = 25;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 131);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 15);
            this.label6.TabIndex = 24;
            this.label6.Text = "Blob size(bytes)";
            // 
            // textBox_AzureConnectionString
            // 
            this.textBox_AzureConnectionString.Location = new System.Drawing.Point(149, 59);
            this.textBox_AzureConnectionString.Name = "textBox_AzureConnectionString";
            this.textBox_AzureConnectionString.Size = new System.Drawing.Size(327, 20);
            this.textBox_AzureConnectionString.TabIndex = 12;
            this.textBox_AzureConnectionString.UseSystemPasswordChar = true;
            // 
            // textBox_AzureSiteName
            // 
            this.textBox_AzureSiteName.Location = new System.Drawing.Point(149, 29);
            this.textBox_AzureSiteName.Name = "textBox_AzureSiteName";
            this.textBox_AzureSiteName.Size = new System.Drawing.Size(329, 20);
            this.textBox_AzureSiteName.TabIndex = 8;
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(8, 59);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(102, 15);
            this.label39.TabIndex = 3;
            this.label39.Text = "Connection string";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(8, 29);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(63, 15);
            this.label40.TabIndex = 0;
            this.label40.Text = "Site name";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button_Delete);
            this.groupBox3.Controls.Add(this.button_CopyItem);
            this.groupBox3.Controls.Add(this.button_AddSite);
            this.groupBox3.Controls.Add(this.button_TestConnection);
            this.groupBox3.Controls.Add(this.button_Apply);
            this.groupBox3.Controls.Add(this.button_Cancel);
            this.groupBox3.Location = new System.Drawing.Point(31, 382);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(827, 68);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            // 
            // button_Delete
            // 
            this.button_Delete.Image = global::EaseFilter.CloudManager.Properties.Resources.delete;
            this.button_Delete.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.button_Delete.Location = new System.Drawing.Point(290, 20);
            this.button_Delete.Name = "button_Delete";
            this.button_Delete.Size = new System.Drawing.Size(120, 25);
            this.button_Delete.TabIndex = 3;
            this.button_Delete.Text = "   Delete";
            this.button_Delete.UseVisualStyleBackColor = true;
            this.button_Delete.Click += new System.EventHandler(this.button_Delete_Click);
            // 
            // button_CopyItem
            // 
            this.button_CopyItem.Image = ((System.Drawing.Image)(resources.GetObject("button_CopyItem.Image")));
            this.button_CopyItem.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.button_CopyItem.Location = new System.Drawing.Point(150, 20);
            this.button_CopyItem.Name = "button_CopyItem";
            this.button_CopyItem.Size = new System.Drawing.Size(120, 25);
            this.button_CopyItem.TabIndex = 7;
            this.button_CopyItem.Text = "Copy";
            this.button_CopyItem.UseVisualStyleBackColor = true;
            this.button_CopyItem.Click += new System.EventHandler(this.button_CopyItem_Click);
            // 
            // button_AddSite
            // 
            this.button_AddSite.Image = global::EaseFilter.CloudManager.Properties.Resources.add;
            this.button_AddSite.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.button_AddSite.Location = new System.Drawing.Point(10, 20);
            this.button_AddSite.Name = "button_AddSite";
            this.button_AddSite.Size = new System.Drawing.Size(120, 25);
            this.button_AddSite.TabIndex = 2;
            this.button_AddSite.Text = "     Add Site";
            this.button_AddSite.UseVisualStyleBackColor = true;
            this.button_AddSite.Click += new System.EventHandler(this.button_AddSite_Click);
            // 
            // button_TestConnection
            // 
            this.button_TestConnection.Image = global::EaseFilter.CloudManager.Properties.Resources.folder_network;
            this.button_TestConnection.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.button_TestConnection.Location = new System.Drawing.Point(430, 20);
            this.button_TestConnection.Name = "button_TestConnection";
            this.button_TestConnection.Size = new System.Drawing.Size(120, 25);
            this.button_TestConnection.TabIndex = 6;
            this.button_TestConnection.Text = "Test Connection";
            this.button_TestConnection.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button_TestConnection.UseVisualStyleBackColor = true;
            this.button_TestConnection.Click += new System.EventHandler(this.button_TestConnection_Click);
            // 
            // button_Apply
            // 
            this.button_Apply.Image = global::EaseFilter.CloudManager.Properties.Resources.check;
            this.button_Apply.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.button_Apply.Location = new System.Drawing.Point(570, 20);
            this.button_Apply.Name = "button_Apply";
            this.button_Apply.Size = new System.Drawing.Size(120, 25);
            this.button_Apply.TabIndex = 4;
            this.button_Apply.Text = "Apply";
            this.button_Apply.UseVisualStyleBackColor = true;
            this.button_Apply.Click += new System.EventHandler(this.button_Apply_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.Image = global::EaseFilter.CloudManager.Properties.Resources.undo;
            this.button_Cancel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.button_Cancel.Location = new System.Drawing.Point(710, 20);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(100, 25);
            this.button_Cancel.TabIndex = 5;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // SiteManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 474);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.tabControl_SiteSetting);
            this.Controls.Add(this.treeView_SiteList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SiteManagerForm";
            this.Text = "Cloud Connection Settings";
            this.tabControl_SiteSetting.ResumeLayout(false);
            this.tabPage_GeneralSetting.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage_AmazonS3.ResumeLayout(false);
            this.groupBox_S3.ResumeLayout(false);
            this.groupBox_S3.PerformLayout();
            this.tabPage_Azure.ResumeLayout(false);
            this.groupBox_Azure.ResumeLayout(false);
            this.groupBox_Azure.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView_SiteList;
        private System.Windows.Forms.TabControl tabControl_SiteSetting;
        private System.Windows.Forms.TabPage tabPage_GeneralSetting;
        private System.Windows.Forms.TabPage tabPage_AmazonS3;
        private System.Windows.Forms.GroupBox groupBox_S3;
        private System.Windows.Forms.TextBox textBox_S3BufferSize;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.CheckBox checkBox_S3EnableMultiPartsUpload;
        private System.Windows.Forms.TextBox textBox_S3PartSize;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.ComboBox comboBox_S3Region;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox textBox_S3SecretKey;
        private System.Windows.Forms.TextBox textBox_S3AccessKeyId;
        private System.Windows.Forms.TextBox textBox_S3SiteName;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TabPage tabPage_Azure;
        private System.Windows.Forms.GroupBox groupBox_Azure;
        private System.Windows.Forms.CheckBox checkBox_AzureMd5Verification;
        private System.Windows.Forms.TextBox textBox_AzureBufferSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox_AzureEnableMultiBlocksUpload;
        private System.Windows.Forms.TextBox textBox_AzureBlobSize;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_AzureConnectionString;
        private System.Windows.Forms.TextBox textBox_AzureSiteName;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog_LocalPath;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button_Delete;
        private System.Windows.Forms.Button button_CopyItem;
        private System.Windows.Forms.Button button_AddSite;
        private System.Windows.Forms.Button button_TestConnection;
        private System.Windows.Forms.Button button_Apply;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_DeleteCachedFiles;
        private System.Windows.Forms.TextBox textBox_DeleteCachedFilesTTL;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.ComboBox comboBox_EventOutputType;
        private System.Windows.Forms.CheckBox checkBox_ClearCacheOnConnect;
        private System.Windows.Forms.Button button_BrowseCacheFolder;
        private System.Windows.Forms.TextBox textBox_ExpireCacheFileTTL;
        private System.Windows.Forms.TextBox textBox_CacheFolder;
        private System.Windows.Forms.ComboBox comboBox_EventLevel;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.CheckBox checkBox_S3EnableParallelDownload;
        private System.Windows.Forms.TextBox textBox_S3ParallelTasks;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.CheckBox checkBox_AzureEnableParallelDownload;
        private System.Windows.Forms.TextBox textBox_AzureParallelTasks;
        private System.Windows.Forms.Label label1;
    }
}