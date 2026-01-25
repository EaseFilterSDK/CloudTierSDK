using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

using CloudTier.CommonObjects;
using CloudTier.FilterControl;

namespace CloudTierS3Demo
{
    public partial class CloudTierS3Form : Form
    {
        Boolean isMessageDisplayed = false;

        public CloudTierS3Form()
        {
            InitializeComponent();

            string lastError = string.Empty;
            Utils.CopyOSPlatformDependentFiles(ref lastError);

            StartPosition = FormStartPosition.CenterScreen;

            DisplayVersion();

        }

        ~CloudTierS3Form()
        {
            FilterWorker.StopService();
        }

        private void DisplayVersion()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            try
            {
                string filterDllPath = Path.Combine(GlobalConfig.AssemblyPath, "FilterAPI.Dll");
                version = FileVersionInfo.GetVersionInfo(filterDllPath).ProductVersion;
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(43, "LoadFilterAPI Dll", EventLevel.Error, "FilterAPI.dll can't be found." + ex.Message);
            }

            this.Text += "    Version:  " + version;
        }



        private void toolStripButton_StartFilter_Click(object sender, EventArgs e)
        {
            string lastError = string.Empty;

            if (!FilterWorker.StartService(FilterWorker.StartType.GuiApp, listView_Info, out lastError))
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Start filter failed." + lastError, "StartFilter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            toolStripButton_StartFilter.Enabled = false;
            toolStripButton_Stop.Enabled = true;

            EventManager.WriteMessage(102, "StartFilter", EventLevel.Information, "Start filter service succeeded.");

        }

        private void toolStripButton_Stop_Click(object sender, EventArgs e)
        {
            FilterWorker.StopService();

            toolStripButton_StartFilter.Enabled = true;
            toolStripButton_Stop.Enabled = false;
        }

        private void toolStripButton_ClearMessage_Click(object sender, EventArgs e)
        {
            listView_Info.Clear();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingForm = new SettingsForm();
            settingForm.StartPosition = FormStartPosition.CenterParent;
            settingForm.ShowDialog();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            EventForm.DisplayEventForm();
        }

        private void createTestStubFileWithToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TestStubFileForms testStubFileForm = new TestStubFileForms();
            testStubFileForm.ShowDialog();
        }


        private void uninstallDriverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterWorker.StopService();
            FilterAPI.UnInstallDriver();
        }

        private void installDriverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterAPI.InstallDriver();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterWorker.StopService();
            Application.Exit();
        }

        private void demoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FilterWorker.StopService();
        }

        private void CloudTierDemoForm_Shown(object sender, EventArgs e)
        {
            if (!isMessageDisplayed)
            {
                isMessageDisplayed = true;
                TestStubFileForms.CreateTestFiles();
                MessageBox.Show("1. Local Stub Files Test\r\n\r\nThe local test stub files were created in the folder " + TestStubFileForms.stubFilesFolder + ",and their corresponding source files are located in " + TestStubFileForms.cacheFolder
                    + ". You can use these stub files for testing.\r\n\r\n2. S3 Stub Files Test\r\n\r\nTo test the stub file with Amazon S3, you first need to configure the S3 site information. Then, open the S3 Explorer, upload the files to S3, and create the stub file in S3 Explorer."
                    , "Test stub file", MessageBoxButtons.OK);
            }
        }

        private void toolStripButton_Help_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.easefilter.com/cloud/cloudtier-S3-intelligent-tiering-demo.htm");
        }


        private void toolStripButton_S3Explorer_Click(object sender, EventArgs e)
        {
            string s3explorerPath = GlobalConfig.AssemblyPath + "\\CloudTier.S3Explorer.exe";
            System.Diagnostics.Process.Start(s3explorerPath);
        }
    }
}
