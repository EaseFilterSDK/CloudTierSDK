namespace CloudTier.S3Explorer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox_DeleteCachedFilesTTL = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.comboBox_EventOutputType = new System.Windows.Forms.ComboBox();
            this.button_BrowseCacheFolder = new System.Windows.Forms.Button();
            this.textBox_ExpireCacheFileTTL = new System.Windows.Forms.TextBox();
            this.textBox_CacheFolder = new System.Windows.Forms.TextBox();
            this.comboBox_EventLevel = new System.Windows.Forms.ComboBox();
            this.label42 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.button_ApplyOptions = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button_EditS3Site = new System.Windows.Forms.Button();
            this.button_DeleteS3Site = new System.Windows.Forms.Button();
            this.listView_S3SiteInfos = new System.Windows.Forms.ListView();
            this.button_AddS3Site = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(543, 205);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox_DeleteCachedFilesTTL);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Controls.Add(this.comboBox_EventOutputType);
            this.groupBox2.Controls.Add(this.button_BrowseCacheFolder);
            this.groupBox2.Controls.Add(this.textBox_ExpireCacheFileTTL);
            this.groupBox2.Controls.Add(this.textBox_CacheFolder);
            this.groupBox2.Controls.Add(this.comboBox_EventLevel);
            this.groupBox2.Controls.Add(this.label42);
            this.groupBox2.Controls.Add(this.label38);
            this.groupBox2.Controls.Add(this.label37);
            this.groupBox2.Controls.Add(this.label31);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 16);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(537, 186);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
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
            this.label22.Size = new System.Drawing.Size(176, 13);
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
            // button_BrowseCacheFolder
            // 
            this.button_BrowseCacheFolder.Location = new System.Drawing.Point(504, 70);
            this.button_BrowseCacheFolder.Name = "button_BrowseCacheFolder";
            this.button_BrowseCacheFolder.Size = new System.Drawing.Size(25, 21);
            this.button_BrowseCacheFolder.TabIndex = 22;
            this.button_BrowseCacheFolder.Text = "..";
            this.button_BrowseCacheFolder.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button_BrowseCacheFolder.UseVisualStyleBackColor = true;
            this.button_BrowseCacheFolder.Click += new System.EventHandler(this.button_BrowseCacheFolder_Click);
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
            this.label42.Size = new System.Drawing.Size(182, 13);
            this.label42.TabIndex = 11;
            this.label42.Text = "Expire cached listing after(x) seconds";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(19, 40);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(91, 13);
            this.label38.TabIndex = 7;
            this.label38.Text = "Event output type";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(19, 70);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(91, 13);
            this.label37.TabIndex = 6;
            this.label37.Text = "Cache folder path";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(19, 10);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(60, 13);
            this.label31.TabIndex = 0;
            this.label31.Text = "Event level";
            // 
            // button_ApplyOptions
            // 
            this.button_ApplyOptions.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_ApplyOptions.Location = new System.Drawing.Point(407, 418);
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
            this.groupBox3.Location = new System.Drawing.Point(10, 223);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(545, 175);
            this.groupBox3.TabIndex = 51;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Amazon S3 Site Settings Collection";
            // 
            // button_EditS3Site
            // 
            this.button_EditS3Site.Location = new System.Drawing.Point(210, 133);
            this.button_EditS3Site.Name = "button_EditS3Site";
            this.button_EditS3Site.Size = new System.Drawing.Size(92, 23);
            this.button_EditS3Site.TabIndex = 56;
            this.button_EditS3Site.Text = "Edit S3 Site";
            this.button_EditS3Site.UseVisualStyleBackColor = true;
            this.button_EditS3Site.Click += new System.EventHandler(this.button_EditS3Site_Click);
            // 
            // button_DeleteS3Site
            // 
            this.button_DeleteS3Site.Location = new System.Drawing.Point(389, 133);
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
            this.listView_S3SiteInfos.Size = new System.Drawing.Size(500, 111);
            this.listView_S3SiteInfos.TabIndex = 1;
            this.listView_S3SiteInfos.UseCompatibleStateImageBehavior = false;
            this.listView_S3SiteInfos.View = System.Windows.Forms.View.Details;
            // 
            // button_AddS3Site
            // 
            this.button_AddS3Site.Location = new System.Drawing.Point(3, 133);
            this.button_AddS3Site.Name = "button_AddS3Site";
            this.button_AddS3Site.Size = new System.Drawing.Size(127, 23);
            this.button_AddS3Site.TabIndex = 54;
            this.button_AddS3Site.Text = "Add New S3 Site";
            this.button_AddS3Site.UseVisualStyleBackColor = true;
            this.button_AddS3Site.Click += new System.EventHandler(this.button_AddS3Site_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(573, 469);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button_ApplyOptions);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.Text = "Amazon S3 Explorer Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_ApplyOptions;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView listView_S3SiteInfos;
        private System.Windows.Forms.Button button_EditS3Site;
        private System.Windows.Forms.Button button_DeleteS3Site;
        private System.Windows.Forms.Button button_AddS3Site;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox_DeleteCachedFilesTTL;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.ComboBox comboBox_EventOutputType;
        private System.Windows.Forms.Button button_BrowseCacheFolder;
        private System.Windows.Forms.TextBox textBox_ExpireCacheFileTTL;
        private System.Windows.Forms.TextBox textBox_CacheFolder;
        private System.Windows.Forms.ComboBox comboBox_EventLevel;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}