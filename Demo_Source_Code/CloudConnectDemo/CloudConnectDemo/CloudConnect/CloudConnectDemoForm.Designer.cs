namespace CloudConnect
{
    partial class CloudConnectDemoForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CloudConnectDemoForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createTestStubFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createStubFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uninstallDriverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.installDriverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportAProblemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sdkManualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutCloudTierDemoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutReparsePointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutSparseFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cloudTierSDKSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_StartFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Stop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_CloudSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_CloudExplorer = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_ClearMessage = new System.Windows.Forms.ToolStripButton();
            this.listView_Info = new System.Windows.Forms.ListView();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(974, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Settings";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createTestStubFileToolStripMenuItem,
            this.createStubFileToolStripMenuItem,
            this.uninstallDriverToolStripMenuItem,
            this.installDriverToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // createTestStubFileToolStripMenuItem
            // 
            this.createTestStubFileToolStripMenuItem.Name = "createTestStubFileToolStripMenuItem";
            this.createTestStubFileToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.createTestStubFileToolStripMenuItem.Text = "Create test stub files";
            this.createTestStubFileToolStripMenuItem.Click += new System.EventHandler(this.createTestStubFileWithToolStripMenuItem_Click);
            // 
            // createStubFileToolStripMenuItem
            // 
            this.createStubFileToolStripMenuItem.Name = "createStubFileToolStripMenuItem";
            this.createStubFileToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.createStubFileToolStripMenuItem.Text = "Create reparse stub file";
            this.createStubFileToolStripMenuItem.Click += new System.EventHandler(this.createStubFileToolStripMenuItem_Click);
            // 
            // uninstallDriverToolStripMenuItem
            // 
            this.uninstallDriverToolStripMenuItem.Name = "uninstallDriverToolStripMenuItem";
            this.uninstallDriverToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.uninstallDriverToolStripMenuItem.Text = "Uninstall driver";
            this.uninstallDriverToolStripMenuItem.Click += new System.EventHandler(this.uninstallDriverToolStripMenuItem_Click);
            // 
            // installDriverToolStripMenuItem
            // 
            this.installDriverToolStripMenuItem.Name = "installDriverToolStripMenuItem";
            this.installDriverToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.installDriverToolStripMenuItem.Text = "Install driver";
            this.installDriverToolStripMenuItem.Click += new System.EventHandler(this.installDriverToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reportAProblemToolStripMenuItem,
            this.sdkManualToolStripMenuItem,
            this.aboutCloudTierDemoToolStripMenuItem,
            this.aboutReparsePointToolStripMenuItem,
            this.aboutSparseFileToolStripMenuItem,
            this.cloudTierSDKSolutionToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // reportAProblemToolStripMenuItem
            // 
            this.reportAProblemToolStripMenuItem.Name = "reportAProblemToolStripMenuItem";
            this.reportAProblemToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.reportAProblemToolStripMenuItem.Text = "Report a problem or suggestion";
            this.reportAProblemToolStripMenuItem.Click += new System.EventHandler(this.reportAProblemToolStripMenuItem_Click);
            // 
            // sdkManualToolStripMenuItem
            // 
            this.sdkManualToolStripMenuItem.Name = "sdkManualToolStripMenuItem";
            this.sdkManualToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.sdkManualToolStripMenuItem.Text = "SDK online manual";
            this.sdkManualToolStripMenuItem.Click += new System.EventHandler(this.helpTopicsToolStripMenuItem_Click);
            // 
            // aboutCloudTierDemoToolStripMenuItem
            // 
            this.aboutCloudTierDemoToolStripMenuItem.Name = "aboutCloudTierDemoToolStripMenuItem";
            this.aboutCloudTierDemoToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.aboutCloudTierDemoToolStripMenuItem.Text = "CloudTier Demo Help";
            this.aboutCloudTierDemoToolStripMenuItem.Click += new System.EventHandler(this.aboutCloudTierDemoToolStripMenuItem_Click);
            // 
            // aboutReparsePointToolStripMenuItem
            // 
            this.aboutReparsePointToolStripMenuItem.Name = "aboutReparsePointToolStripMenuItem";
            this.aboutReparsePointToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.aboutReparsePointToolStripMenuItem.Text = "About reparse point";
            this.aboutReparsePointToolStripMenuItem.Click += new System.EventHandler(this.aboutReparsePointToolStripMenuItem_Click);
            // 
            // aboutSparseFileToolStripMenuItem
            // 
            this.aboutSparseFileToolStripMenuItem.Name = "aboutSparseFileToolStripMenuItem";
            this.aboutSparseFileToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.aboutSparseFileToolStripMenuItem.Text = "About sparse file";
            this.aboutSparseFileToolStripMenuItem.Click += new System.EventHandler(this.aboutSparseFileToolStripMenuItem_Click);
            // 
            // cloudTierSDKSolutionToolStripMenuItem
            // 
            this.cloudTierSDKSolutionToolStripMenuItem.Name = "cloudTierSDKSolutionToolStripMenuItem";
            this.cloudTierSDKSolutionToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.cloudTierSDKSolutionToolStripMenuItem.Text = "CloudTier SDK solution";
            this.cloudTierSDKSolutionToolStripMenuItem.Click += new System.EventHandler(this.cloudTierSDKSolutionToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_StartFilter,
            this.toolStripSeparator1,
            this.toolStripButton_Stop,
            this.toolStripSeparator2,
            this.toolStripButton_CloudSettings,
            this.toolStripSeparator5,
            this.toolStripButton_CloudExplorer,
            this.toolStripSeparator4,
            this.toolStripButton1,
            this.toolStripSeparator3,
            this.toolStripButton_ClearMessage});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(974, 27);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton_StartFilter
            // 
            this.toolStripButton_StartFilter.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_StartFilter.Image")));
            this.toolStripButton_StartFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_StartFilter.Name = "toolStripButton_StartFilter";
            this.toolStripButton_StartFilter.Size = new System.Drawing.Size(121, 24);
            this.toolStripButton_StartFilter.Text = "Start filter service";
            this.toolStripButton_StartFilter.Click += new System.EventHandler(this.toolStripButton_StartFilter_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButton_Stop
            // 
            this.toolStripButton_Stop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Stop.Image")));
            this.toolStripButton_Stop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Stop.Name = "toolStripButton_Stop";
            this.toolStripButton_Stop.Size = new System.Drawing.Size(121, 24);
            this.toolStripButton_Stop.Text = "Stop filter service";
            this.toolStripButton_Stop.Click += new System.EventHandler(this.toolStripButton_Stop_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButton_CloudSettings
            // 
            this.toolStripButton_CloudSettings.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_CloudSettings.Image")));
            this.toolStripButton_CloudSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_CloudSettings.Name = "toolStripButton_CloudSettings";
            this.toolStripButton_CloudSettings.Size = new System.Drawing.Size(108, 24);
            this.toolStripButton_CloudSettings.Text = "Cloud Settings";
            this.toolStripButton_CloudSettings.Click += new System.EventHandler(this.toolStripButton_CloudSettings_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButton_CloudExplorer
            // 
            this.toolStripButton_CloudExplorer.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_CloudExplorer.Image")));
            this.toolStripButton_CloudExplorer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_CloudExplorer.Name = "toolStripButton_CloudExplorer";
            this.toolStripButton_CloudExplorer.Size = new System.Drawing.Size(108, 24);
            this.toolStripButton_CloudExplorer.Text = "Cloud Explorer";
            this.toolStripButton_CloudExplorer.Click += new System.EventHandler(this.toolStripButton_CloudExplorer_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(97, 24);
            this.toolStripButton1.Text = "Event viewer";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButton_ClearMessage
            // 
            this.toolStripButton_ClearMessage.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_ClearMessage.Image")));
            this.toolStripButton_ClearMessage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_ClearMessage.Name = "toolStripButton_ClearMessage";
            this.toolStripButton_ClearMessage.Size = new System.Drawing.Size(112, 24);
            this.toolStripButton_ClearMessage.Text = "Clear messages";
            this.toolStripButton_ClearMessage.Click += new System.EventHandler(this.toolStripButton_ClearMessage_Click);
            // 
            // listView_Info
            // 
            this.listView_Info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_Info.FullRowSelect = true;
            this.listView_Info.HoverSelection = true;
            this.listView_Info.LabelEdit = true;
            this.listView_Info.Location = new System.Drawing.Point(0, 51);
            this.listView_Info.Name = "listView_Info";
            this.listView_Info.ShowItemToolTips = true;
            this.listView_Info.Size = new System.Drawing.Size(974, 455);
            this.listView_Info.TabIndex = 2;
            this.listView_Info.UseCompatibleStateImageBehavior = false;
            this.listView_Info.View = System.Windows.Forms.View.Details;
            // 
            // CloudConnectDemoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(974, 506);
            this.Controls.Add(this.listView_Info);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "CloudConnectDemoForm";
            this.Text = "CloudTier Cloud Connect";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.demoForm_FormClosed);
            this.Shown += new System.EventHandler(this.CloudTierDemoForm_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton_StartFilter;
        private System.Windows.Forms.ToolStripButton toolStripButton_Stop;
        private System.Windows.Forms.ToolStripButton toolStripButton_ClearMessage;
        private System.Windows.Forms.ListView listView_Info;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportAProblemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sdkManualToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createTestStubFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createStubFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uninstallDriverToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem installDriverToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutCloudTierDemoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutReparsePointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutSparseFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cloudTierSDKSolutionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButton_CloudExplorer;
        private System.Windows.Forms.ToolStripButton toolStripButton_CloudSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
    }
}

