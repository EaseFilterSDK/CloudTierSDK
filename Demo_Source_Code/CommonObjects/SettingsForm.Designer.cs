namespace CloudTier.CommonObjects
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
            this.checkBox_ReOpenFileOnReHydration = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton_CacheFile = new System.Windows.Forms.RadioButton();
            this.radioButton_Rehydrate = new System.Windows.Forms.RadioButton();
            this.radioButton_Block = new System.Windows.Forms.RadioButton();
            this.textBox_Threads = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_Timeout = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_MaximumFilterMessage = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button_SelectExcludePID = new System.Windows.Forms.Button();
            this.textBox_ExcludePID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button_ApplyOptions = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_ReOpenFileOnReHydration);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.textBox_Threads);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBox_Timeout);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBox_MaximumFilterMessage);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.button_SelectExcludePID);
            this.groupBox1.Controls.Add(this.textBox_ExcludePID);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(602, 356);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // checkBox_ReOpenFileOnReHydration
            // 
            this.checkBox_ReOpenFileOnReHydration.AutoSize = true;
            this.checkBox_ReOpenFileOnReHydration.Location = new System.Drawing.Point(180, 208);
            this.checkBox_ReOpenFileOnReHydration.Name = "checkBox_ReOpenFileOnReHydration";
            this.checkBox_ReOpenFileOnReHydration.Size = new System.Drawing.Size(284, 17);
            this.checkBox_ReOpenFileOnReHydration.TabIndex = 61;
            this.checkBox_ReOpenFileOnReHydration.Text = "Bypass the write event on rehydration for monitor driver";
            this.checkBox_ReOpenFileOnReHydration.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton_CacheFile);
            this.groupBox2.Controls.Add(this.radioButton_Rehydrate);
            this.groupBox2.Controls.Add(this.radioButton_Block);
            this.groupBox2.Location = new System.Drawing.Point(3, 263);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(564, 50);
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
            // textBox_MaximumFilterMessage
            // 
            this.textBox_MaximumFilterMessage.Location = new System.Drawing.Point(180, 161);
            this.textBox_MaximumFilterMessage.Name = "textBox_MaximumFilterMessage";
            this.textBox_MaximumFilterMessage.Size = new System.Drawing.Size(362, 20);
            this.textBox_MaximumFilterMessage.TabIndex = 41;
            this.textBox_MaximumFilterMessage.Text = "5000";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(162, 13);
            this.label6.TabIndex = 40;
            this.label6.Text = "Maximum filter message records  ";
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
            this.button_ApplyOptions.Location = new System.Drawing.Point(508, 388);
            this.button_ApplyOptions.Name = "button_ApplyOptions";
            this.button_ApplyOptions.Size = new System.Drawing.Size(106, 23);
            this.button_ApplyOptions.TabIndex = 1;
            this.button_ApplyOptions.Text = "Apply Settings";
            this.button_ApplyOptions.UseVisualStyleBackColor = true;
            this.button_ApplyOptions.Click += new System.EventHandler(this.button_ApplyOptions_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 439);
            this.Controls.Add(this.button_ApplyOptions);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.Text = "Filter Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_SelectExcludePID;
        private System.Windows.Forms.TextBox textBox_ExcludePID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_ApplyOptions;
        private System.Windows.Forms.TextBox textBox_MaximumFilterMessage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_Threads;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_Timeout;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButton_CacheFile;
        private System.Windows.Forms.RadioButton radioButton_Block;
        private System.Windows.Forms.RadioButton radioButton_Rehydrate;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBox_ReOpenFileOnReHydration;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}