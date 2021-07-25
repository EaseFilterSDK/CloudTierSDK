namespace CloudConnect
{
    partial class CreateTestStubFileForms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateTestStubFileForms));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_SourceFolder = new System.Windows.Forms.TextBox();
            this.textBox_StubFolder = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_BrowseStubFolder = new System.Windows.Forms.Button();
            this.button_BrowseSoruceFolder = new System.Windows.Forms.Button();
            this.button_Start = new System.Windows.Forms.Button();
            this.richTextBox_Info = new System.Windows.Forms.RichTextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "The source folder";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "The test stub file folder";
            // 
            // textBox_SourceFolder
            // 
            this.textBox_SourceFolder.Location = new System.Drawing.Point(145, 30);
            this.textBox_SourceFolder.Name = "textBox_SourceFolder";
            this.textBox_SourceFolder.Size = new System.Drawing.Size(357, 20);
            this.textBox_SourceFolder.TabIndex = 2;
            this.textBox_SourceFolder.Text = "c:\\cloudtier\\TestSourceFolder";
            // 
            // textBox_StubFolder
            // 
            this.textBox_StubFolder.Location = new System.Drawing.Point(145, 58);
            this.textBox_StubFolder.Name = "textBox_StubFolder";
            this.textBox_StubFolder.Size = new System.Drawing.Size(357, 20);
            this.textBox_StubFolder.TabIndex = 3;
            this.textBox_StubFolder.Text = "c:\\cloudtier\\TestStubFolder";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_BrowseStubFolder);
            this.groupBox1.Controls.Add(this.button_BrowseSoruceFolder);
            this.groupBox1.Controls.Add(this.textBox_StubFolder);
            this.groupBox1.Controls.Add(this.textBox_SourceFolder);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(35, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(597, 99);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // button_BrowseStubFolder
            // 
            this.button_BrowseStubFolder.Location = new System.Drawing.Point(523, 58);
            this.button_BrowseStubFolder.Name = "button_BrowseStubFolder";
            this.button_BrowseStubFolder.Size = new System.Drawing.Size(49, 23);
            this.button_BrowseStubFolder.TabIndex = 5;
            this.button_BrowseStubFolder.Text = "browse";
            this.button_BrowseStubFolder.UseVisualStyleBackColor = true;
            this.button_BrowseStubFolder.Click += new System.EventHandler(this.button_BrowseStubFolder_Click);
            // 
            // button_BrowseSoruceFolder
            // 
            this.button_BrowseSoruceFolder.Location = new System.Drawing.Point(523, 27);
            this.button_BrowseSoruceFolder.Name = "button_BrowseSoruceFolder";
            this.button_BrowseSoruceFolder.Size = new System.Drawing.Size(49, 23);
            this.button_BrowseSoruceFolder.TabIndex = 4;
            this.button_BrowseSoruceFolder.Text = "browse";
            this.button_BrowseSoruceFolder.UseVisualStyleBackColor = true;
            this.button_BrowseSoruceFolder.Click += new System.EventHandler(this.button_BrowseSoruceFolder_Click);
            // 
            // button_Start
            // 
            this.button_Start.Location = new System.Drawing.Point(557, 406);
            this.button_Start.Name = "button_Start";
            this.button_Start.Size = new System.Drawing.Size(75, 23);
            this.button_Start.TabIndex = 2;
            this.button_Start.Text = "Start";
            this.button_Start.UseVisualStyleBackColor = true;
            this.button_Start.Click += new System.EventHandler(this.button_Start_Click);
            // 
            // richTextBox_Info
            // 
            this.richTextBox_Info.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_Info.Location = new System.Drawing.Point(35, 129);
            this.richTextBox_Info.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBox_Info.Name = "richTextBox_Info";
            this.richTextBox_Info.Size = new System.Drawing.Size(599, 266);
            this.richTextBox_Info.TabIndex = 3;
            this.richTextBox_Info.Text = resources.GetString("richTextBox_Info.Text");
            // 
            // CreateTestStubFileForms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 466);
            this.Controls.Add(this.richTextBox_Info);
            this.Controls.Add(this.button_Start);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CreateTestStubFileForms";
            this.Text = "Create Test Stub Files";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_SourceFolder;
        private System.Windows.Forms.TextBox textBox_StubFolder;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_Start;
        private System.Windows.Forms.Button button_BrowseStubFolder;
        private System.Windows.Forms.Button button_BrowseSoruceFolder;
        private System.Windows.Forms.RichTextBox richTextBox_Info;
    }
}