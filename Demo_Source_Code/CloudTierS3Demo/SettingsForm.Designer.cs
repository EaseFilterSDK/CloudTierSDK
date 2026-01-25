namespace CloudTierS3Demo
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_BrowseCacheFolder = new System.Windows.Forms.Button();
            this.textBox_CacheFolder = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton_CacheFile = new System.Windows.Forms.RadioButton();
            this.radioButton_Rehydrate = new System.Windows.Forms.RadioButton();
            this.radioButton_Block = new System.Windows.Forms.RadioButton();
            this.textBox_Threads = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_Timeout = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button_SelectExcludePID = new System.Windows.Forms.Button();
            this.textBox_ExcludePID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button_ApplyOptions = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button_EditS3Site = new System.Windows.Forms.Button();
            this.button_DeleteS3Site = new System.Windows.Forms.Button();
            this.listView_S3SiteInfos = new System.Windows.Forms.ListView();
            this.button_AddS3Site = new System.Windows.Forms.Button();
            this.comboBox_EventLevel = new System.Windows.Forms.ComboBox();
            this.label31 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox_EventLevel);
            this.groupBox1.Controls.Add(this.label31);
            this.groupBox1.Controls.Add(this.button_BrowseCacheFolder);
            this.groupBox1.Controls.Add(this.textBox_CacheFolder);
            this.groupBox1.Controls.Add(this.label37);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.textBox_Threads);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBox_Timeout);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.button_SelectExcludePID);
            this.groupBox1.Controls.Add(this.textBox_ExcludePID);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(588, 293);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // button_BrowseCacheFolder
            // 
            this.button_BrowseCacheFolder.Location = new System.Drawing.Point(553, 203);
            this.button_BrowseCacheFolder.Name = "button_BrowseCacheFolder";
            this.button_BrowseCacheFolder.Size = new System.Drawing.Size(25, 21);
            this.button_BrowseCacheFolder.TabIndex = 63;
            this.button_BrowseCacheFolder.Text = "..";
            this.button_BrowseCacheFolder.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button_BrowseCacheFolder.UseVisualStyleBackColor = true;
            this.button_BrowseCacheFolder.Click += new System.EventHandler(this.button_BrowseCacheFolder_Click);
            // 
            // textBox_CacheFolder
            // 
            this.textBox_CacheFolder.Location = new System.Drawing.Point(180, 203);
            this.textBox_CacheFolder.Name = "textBox_CacheFolder";
            this.textBox_CacheFolder.Size = new System.Drawing.Size(361, 20);
            this.textBox_CacheFolder.TabIndex = 61;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(10, 210);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(91, 13);
            this.label37.TabIndex = 62;
            this.label37.Text = "Cache folder path";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton_CacheFile);
            this.groupBox2.Controls.Add(this.radioButton_Rehydrate);
            this.groupBox2.Controls.Add(this.radioButton_Block);
            this.groupBox2.Location = new System.Drawing.Point(6, 237);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(536, 50);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            // 
            // radioButton_CacheFile
            // 
            this.radioButton_CacheFile.AutoSize = true;
            this.radioButton_CacheFile.Location = new System.Drawing.Point(177, 20);
            this.radioButton_CacheFile.Name = "radioButton_CacheFile";
            this.radioButton_CacheFile.Size = new System.Drawing.Size(145, 17);
            this.radioButton_CacheFile.TabIndex = 65;
            this.radioButton_CacheFile.Text = "Return cache file on read";
            this.radioButton_CacheFile.UseVisualStyleBackColor = true;
            // 
            // radioButton_Rehydrate
            // 
            this.radioButton_Rehydrate.AutoSize = true;
            this.radioButton_Rehydrate.Location = new System.Drawing.Point(368, 20);
            this.radioButton_Rehydrate.Name = "radioButton_Rehydrate";
            this.radioButton_Rehydrate.Size = new System.Drawing.Size(148, 17);
            this.radioButton_Rehydrate.TabIndex = 63;
            this.radioButton_Rehydrate.Text = "Rehydrate file on first read";
            this.radioButton_Rehydrate.UseVisualStyleBackColor = true;
            // 
            // radioButton_Block
            // 
            this.radioButton_Block.AutoSize = true;
            this.radioButton_Block.Checked = true;
            this.radioButton_Block.Location = new System.Drawing.Point(10, 20);
            this.radioButton_Block.Name = "radioButton_Block";
            this.radioButton_Block.Size = new System.Drawing.Size(149, 17);
            this.radioButton_Block.TabIndex = 64;
            this.radioButton_Block.TabStop = true;
            this.radioButton_Block.Text = "Return block data on read";
            this.radioButton_Block.UseVisualStyleBackColor = true;
            // 
            // textBox_Threads
            // 
            this.textBox_Threads.Location = new System.Drawing.Point(180, 73);
            this.textBox_Threads.Name = "textBox_Threads";
            this.textBox_Threads.Size = new System.Drawing.Size(362, 20);
            this.textBox_Threads.TabIndex = 60;
            this.textBox_Threads.Text = "5";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(129, 13);
            this.label4.TabIndex = 59;
            this.label4.Text = "Filter connection threads  ";
            // 
            // textBox_Timeout
            // 
            this.textBox_Timeout.Location = new System.Drawing.Point(179, 115);
            this.textBox_Timeout.Name = "textBox_Timeout";
            this.textBox_Timeout.Size = new System.Drawing.Size(362, 20);
            this.textBox_Timeout.TabIndex = 58;
            this.textBox_Timeout.Text = "30";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 13);
            this.label3.TabIndex = 57;
            this.label3.Text = "Connection timeout(Seconds)  ";
            // 
            // button_SelectExcludePID
            // 
            this.button_SelectExcludePID.Location = new System.Drawing.Point(548, 31);
            this.button_SelectExcludePID.Name = "button_SelectExcludePID";
            this.button_SelectExcludePID.Size = new System.Drawing.Size(30, 20);
            this.button_SelectExcludePID.TabIndex = 38;
            this.button_SelectExcludePID.Text = "...";
            this.button_SelectExcludePID.UseVisualStyleBackColor = true;
            this.button_SelectExcludePID.Click += new System.EventHandler(this.button_SelectExcludePID_Click);
            // 
            // textBox_ExcludePID
            // 
            this.textBox_ExcludePID.Location = new System.Drawing.Point(180, 31);
            this.textBox_ExcludePID.Name = "textBox_ExcludePID";
            this.textBox_ExcludePID.ReadOnly = true;
            this.textBox_ExcludePID.Size = new System.Drawing.Size(361, 20);
            this.textBox_ExcludePID.TabIndex = 37;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 13);
            this.label5.TabIndex = 36;
            this.label5.Text = "Excluded process IDs";
            // 
            // button_ApplyOptions
            // 
            this.button_ApplyOptions.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_ApplyOptions.Location = new System.Drawing.Point(494, 449);
            this.button_ApplyOptions.Name = "button_ApplyOptions";
            this.button_ApplyOptions.Size = new System.Drawing.Size(106, 23);
            this.button_ApplyOptions.TabIndex = 1;
            this.button_ApplyOptions.Text = "Apply Settings";
            this.button_ApplyOptions.UseVisualStyleBackColor = true;
            this.button_ApplyOptions.Click += new System.EventHandler(this.button_ApplyOptions_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button_EditS3Site);
            this.groupBox3.Controls.Add(this.button_DeleteS3Site);
            this.groupBox3.Controls.Add(this.listView_S3SiteInfos);
            this.groupBox3.Controls.Add(this.button_AddS3Site);
            this.groupBox3.Location = new System.Drawing.Point(12, 305);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(588, 129);
            this.groupBox3.TabIndex = 52;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Amazon S3 Site Settings Collection";
            // 
            // button_EditS3Site
            // 
            this.button_EditS3Site.Location = new System.Drawing.Point(236, 92);
            this.button_EditS3Site.Name = "button_EditS3Site";
            this.button_EditS3Site.Size = new System.Drawing.Size(92, 23);
            this.button_EditS3Site.TabIndex = 56;
            this.button_EditS3Site.Text = "Edit S3 Site";
            this.button_EditS3Site.UseVisualStyleBackColor = true;
            this.button_EditS3Site.Click += new System.EventHandler(this.button_EditS3Site_Click_1);
            // 
            // button_DeleteS3Site
            // 
            this.button_DeleteS3Site.Location = new System.Drawing.Point(427, 92);
            this.button_DeleteS3Site.Name = "button_DeleteS3Site";
            this.button_DeleteS3Site.Size = new System.Drawing.Size(114, 23);
            this.button_DeleteS3Site.TabIndex = 55;
            this.button_DeleteS3Site.Text = "Delete S3 Site";
            this.button_DeleteS3Site.UseVisualStyleBackColor = true;
            this.button_DeleteS3Site.Click += new System.EventHandler(this.button_DeleteS3Site_Click);
            // 
            // listView_S3SiteInfos
            // 
            this.listView_S3SiteInfos.FullRowSelect = true;
            this.listView_S3SiteInfos.HideSelection = false;
            this.listView_S3SiteInfos.Location = new System.Drawing.Point(3, 16);
            this.listView_S3SiteInfos.Name = "listView_S3SiteInfos";
            this.listView_S3SiteInfos.Size = new System.Drawing.Size(538, 70);
            this.listView_S3SiteInfos.TabIndex = 1;
            this.listView_S3SiteInfos.UseCompatibleStateImageBehavior = false;
            this.listView_S3SiteInfos.View = System.Windows.Forms.View.Details;
            // 
            // button_AddS3Site
            // 
            this.button_AddS3Site.Location = new System.Drawing.Point(0, 92);
            this.button_AddS3Site.Name = "button_AddS3Site";
            this.button_AddS3Site.Size = new System.Drawing.Size(127, 23);
            this.button_AddS3Site.TabIndex = 54;
            this.button_AddS3Site.Text = "Add New S3 Site";
            this.button_AddS3Site.UseVisualStyleBackColor = true;
            this.button_AddS3Site.Click += new System.EventHandler(this.button_AddS3Site_Click_1);
            // 
            // comboBox_EventLevel
            // 
            this.comboBox_EventLevel.FormattingEnabled = true;
            this.comboBox_EventLevel.Location = new System.Drawing.Point(180, 152);
            this.comboBox_EventLevel.Name = "comboBox_EventLevel";
            this.comboBox_EventLevel.Size = new System.Drawing.Size(362, 21);
            this.comboBox_EventLevel.TabIndex = 65;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(10, 160);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(60, 13);
            this.label31.TabIndex = 64;
            this.label31.Text = "Event level";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 489);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button_ApplyOptions);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.Text = "CloudTier Filter Driver Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_SelectExcludePID;
        private System.Windows.Forms.TextBox textBox_ExcludePID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_ApplyOptions;
        private System.Windows.Forms.TextBox textBox_Threads;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_Timeout;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButton_CacheFile;
        private System.Windows.Forms.RadioButton radioButton_Block;
        private System.Windows.Forms.RadioButton radioButton_Rehydrate;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button button_BrowseCacheFolder;
        private System.Windows.Forms.TextBox textBox_CacheFolder;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button_EditS3Site;
        private System.Windows.Forms.Button button_DeleteS3Site;
        private System.Windows.Forms.ListView listView_S3SiteInfos;
        private System.Windows.Forms.Button button_AddS3Site;
        private System.Windows.Forms.ComboBox comboBox_EventLevel;
        private System.Windows.Forms.Label label31;
    }
}