
namespace CloudTier.AmazonS3Sup
{
    partial class S3SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(S3SettingsForm));
            this.groupBox_S3 = new System.Windows.Forms.GroupBox();
            this.textBox_RemotePath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_LocalPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
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
            this.button_ApplySettings = new System.Windows.Forms.Button();
            this.button_TestConnection = new System.Windows.Forms.Button();
            this.groupBox_S3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_S3
            // 
            this.groupBox_S3.Controls.Add(this.textBox_RemotePath);
            this.groupBox_S3.Controls.Add(this.label2);
            this.groupBox_S3.Controls.Add(this.textBox_LocalPath);
            this.groupBox_S3.Controls.Add(this.label1);
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
            this.groupBox_S3.Location = new System.Drawing.Point(3, 12);
            this.groupBox_S3.Name = "groupBox_S3";
            this.groupBox_S3.Size = new System.Drawing.Size(535, 324);
            this.groupBox_S3.TabIndex = 23;
            this.groupBox_S3.TabStop = false;
            // 
            // textBox_RemotePath
            // 
            this.textBox_RemotePath.Location = new System.Drawing.Point(120, 87);
            this.textBox_RemotePath.Name = "textBox_RemotePath";
            this.textBox_RemotePath.Size = new System.Drawing.Size(358, 20);
            this.textBox_RemotePath.TabIndex = 39;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 38;
            this.label2.Text = "Remote path name";
            // 
            // textBox_LocalPath
            // 
            this.textBox_LocalPath.Location = new System.Drawing.Point(120, 53);
            this.textBox_LocalPath.Name = "textBox_LocalPath";
            this.textBox_LocalPath.Size = new System.Drawing.Size(358, 20);
            this.textBox_LocalPath.TabIndex = 37;
            this.textBox_LocalPath.Text = "c:\\AmazonS3Site1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 36;
            this.label1.Text = "Local folder name";
            // 
            // textBox_S3ParallelTasks
            // 
            this.textBox_S3ParallelTasks.Location = new System.Drawing.Point(120, 286);
            this.textBox_S3ParallelTasks.Name = "textBox_S3ParallelTasks";
            this.textBox_S3ParallelTasks.Size = new System.Drawing.Size(191, 20);
            this.textBox_S3ParallelTasks.TabIndex = 34;
            this.textBox_S3ParallelTasks.Text = "1";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(10, 286);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(69, 13);
            this.label34.TabIndex = 33;
            this.label34.Text = "Parallel tasks";
            // 
            // textBox_S3BufferSize
            // 
            this.textBox_S3BufferSize.Location = new System.Drawing.Point(120, 216);
            this.textBox_S3BufferSize.Name = "textBox_S3BufferSize";
            this.textBox_S3BufferSize.Size = new System.Drawing.Size(358, 20);
            this.textBox_S3BufferSize.TabIndex = 32;
            this.textBox_S3BufferSize.Text = "65536";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(10, 216);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(87, 13);
            this.label27.TabIndex = 31;
            this.label27.Text = "Buffer size(bytes)";
            // 
            // checkBox_S3EnableMultiPartsUpload
            // 
            this.checkBox_S3EnableMultiPartsUpload.AutoSize = true;
            this.checkBox_S3EnableMultiPartsUpload.Location = new System.Drawing.Point(317, 248);
            this.checkBox_S3EnableMultiPartsUpload.Name = "checkBox_S3EnableMultiPartsUpload";
            this.checkBox_S3EnableMultiPartsUpload.Size = new System.Drawing.Size(158, 17);
            this.checkBox_S3EnableMultiPartsUpload.TabIndex = 30;
            this.checkBox_S3EnableMultiPartsUpload.Text = "Enable upload multiple parts";
            this.checkBox_S3EnableMultiPartsUpload.UseVisualStyleBackColor = true;
            // 
            // textBox_S3PartSize
            // 
            this.textBox_S3PartSize.Location = new System.Drawing.Point(120, 248);
            this.textBox_S3PartSize.Name = "textBox_S3PartSize";
            this.textBox_S3PartSize.Size = new System.Drawing.Size(191, 20);
            this.textBox_S3PartSize.TabIndex = 25;
            this.textBox_S3PartSize.Text = "5242880";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(10, 246);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(78, 13);
            this.label26.TabIndex = 24;
            this.label26.Text = "Part size(bytes)";
            // 
            // comboBox_S3Region
            // 
            this.comboBox_S3Region.FormattingEnabled = true;
            this.comboBox_S3Region.Location = new System.Drawing.Point(120, 181);
            this.comboBox_S3Region.Name = "comboBox_S3Region";
            this.comboBox_S3Region.Size = new System.Drawing.Size(358, 21);
            this.comboBox_S3Region.TabIndex = 19;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(10, 181);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(70, 13);
            this.label21.TabIndex = 18;
            this.label21.Text = "Region name";
            // 
            // textBox_S3SecretKey
            // 
            this.textBox_S3SecretKey.Location = new System.Drawing.Point(120, 151);
            this.textBox_S3SecretKey.Name = "textBox_S3SecretKey";
            this.textBox_S3SecretKey.Size = new System.Drawing.Size(358, 20);
            this.textBox_S3SecretKey.TabIndex = 13;
            this.textBox_S3SecretKey.UseSystemPasswordChar = true;
            // 
            // textBox_S3AccessKeyId
            // 
            this.textBox_S3AccessKeyId.Location = new System.Drawing.Point(120, 121);
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
            this.textBox_S3SiteName.Text = "AmazonS3Site1";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(10, 151);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(95, 13);
            this.label18.TabIndex = 4;
            this.label18.Text = "Secret access key";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(10, 121);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(73, 13);
            this.label19.TabIndex = 3;
            this.label19.Text = "Access key id";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(10, 21);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(54, 13);
            this.label20.TabIndex = 0;
            this.label20.Text = "Site name";
            // 
            // button_ApplySettings
            // 
            this.button_ApplySettings.Location = new System.Drawing.Point(375, 357);
            this.button_ApplySettings.Name = "button_ApplySettings";
            this.button_ApplySettings.Size = new System.Drawing.Size(106, 23);
            this.button_ApplySettings.TabIndex = 24;
            this.button_ApplySettings.Text = "Apply Settings";
            this.button_ApplySettings.UseVisualStyleBackColor = true;
            this.button_ApplySettings.Click += new System.EventHandler(this.button_ApplySettings_Click);
            // 
            // button_TestConnection
            // 
            this.button_TestConnection.Location = new System.Drawing.Point(16, 357);
            this.button_TestConnection.Name = "button_TestConnection";
            this.button_TestConnection.Size = new System.Drawing.Size(106, 23);
            this.button_TestConnection.TabIndex = 25;
            this.button_TestConnection.Text = "Test Connection";
            this.button_TestConnection.UseVisualStyleBackColor = true;
            this.button_TestConnection.Click += new System.EventHandler(this.button_TestConnection_Click);
            // 
            // S3SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 410);
            this.Controls.Add(this.button_TestConnection);
            this.Controls.Add(this.button_ApplySettings);
            this.Controls.Add(this.groupBox_S3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "S3SettingsForm";
            this.Text = "Amazon S3 Site Settings";
            this.groupBox_S3.ResumeLayout(false);
            this.groupBox_S3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_S3;
        private System.Windows.Forms.TextBox textBox_RemotePath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_LocalPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_S3ParallelTasks;
        private System.Windows.Forms.Label label34;
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
        private System.Windows.Forms.Button button_ApplySettings;
        private System.Windows.Forms.Button button_TestConnection;
    }
}