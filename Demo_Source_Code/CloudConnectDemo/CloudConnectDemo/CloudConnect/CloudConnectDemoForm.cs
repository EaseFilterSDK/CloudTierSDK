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

using EaseFilter.GlobalObjects;
using EaseFilter.CloudExplorer;
using EaseFilter.CloudManager;

namespace CloudConnect
{
    public partial class CloudConnectDemoForm : Form
    {
        FilterMessage filterMessage = null;

        //Purchase a license key with the link: http://www.easefilter.com/Order.htm
        //Email us to request a trial key: info@easefilter.com //free email is not accepted.
        string registerKey = GlobalConfig.registerKey;

        Boolean isMessageDisplayed = false;

        public CloudConnectDemoForm()
        {
            InitializeComponent();

            Utils.CopyOSPlatformDependentFiles();

            StartPosition = FormStartPosition.CenterScreen;
            filterMessage = new FilterMessage(listView_Info);

            GlobalConfig.EventLevel = EventLevel.Verbose;

            DisplayVersion();

        }

        ~CloudConnectDemoForm()
        {
            GlobalConfig.Stop();
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
            try
            {
                string lastError = string.Empty;                

                bool ret = FilterAPI.StartFilter(registerKey
                                            , (int)GlobalConfig.FilterConnectionThreads
                                            , new FilterAPI.FilterDelegate(FilterCallback)
                                            , new FilterAPI.DisconnectDelegate(DisconnectCallback)
                                            , ref lastError);
                if (!ret)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                    MessageBox.Show("Start filter failed." + lastError);
                    return;
                }

                toolStripButton_StartFilter.Enabled = false;
                toolStripButton_Stop.Enabled = true;

                GlobalConfig.SendConfigSettingsToFilter();

                EventManager.WriteMessage(102, "StartFilter", EventLevel.Information, "Start filter service succeeded.");
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(104, "StartFilter", EventLevel.Error, "Start filter service failed with error " + ex.Message);
            }

        }

        private void toolStripButton_Stop_Click(object sender, EventArgs e)
        {
            FilterAPI.StopFilter();

            toolStripButton_StartFilter.Enabled = true;
            toolStripButton_Stop.Enabled = false;
        }

        private void toolStripButton_ClearMessage_Click(object sender, EventArgs e)
        {
            filterMessage.InitListView();
        }

        Boolean FilterCallback(IntPtr sendDataPtr, IntPtr replyDataPtr)
        {
            Boolean ret = true;

            try
            {
                FilterAPI.MessageSendData messageSend = new FilterAPI.MessageSendData();
                messageSend = (FilterAPI.MessageSendData)Marshal.PtrToStructure(sendDataPtr, typeof(FilterAPI.MessageSendData));

                if (FilterAPI.MESSAGE_SEND_VERIFICATION_NUMBER != messageSend.VerificationNumber)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                    MessageBox.Show("Received message corrupted.Please check if the MessageSendData structure is correct.");

                    EventManager.WriteMessage(139, "FilterCallback", EventLevel.Error, "Received message corrupted.Please check if the MessageSendData structure is correct.");
                    return false;
                }
              
                //here we store our cache file name in stub file tag data, you can customize your own tag data here.
                if (messageSend.DataBufferLength == 0)
                {
                    Console.WriteLine("There are no tag data for stub file " + messageSend.FileName + ", return false here.");
                    return false;
                }

                FilterAPI.MessageReplyData messageReply = new FilterAPI.MessageReplyData();
              
                if (replyDataPtr.ToInt64() != 0)
                {
                    messageReply = (FilterAPI.MessageReplyData)Marshal.PtrToStructure(replyDataPtr, typeof(FilterAPI.MessageReplyData));

                    messageReply.MessageId = messageSend.MessageId;
                    messageReply.MessageType = messageSend.MessageType;

                    //here you can control the IO behaviour and modify the data.
                    FilterService.ProcessRequest(messageSend,ref messageReply);

                    Marshal.StructureToPtr(messageReply, replyDataPtr, true);
                }


                filterMessage.AddMessage(messageSend, messageReply);

                return ret;
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(134, "FilterCallback", EventLevel.Error, "filter callback exception." + ex.Message);
                return false;
            }

        }

        void DisconnectCallback()
        {
            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            MessageBox.Show("Filter Disconnected." + FilterAPI.GetLastErrorMessage(), "Filter Disconnected.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            CreateTestStubFileForms testStubFileForm = new CreateTestStubFileForms();
            testStubFileForm.ShowDialog();
        }

  
        private void createStubFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateStubFileForm createStubFileForm = new CreateStubFileForm();
            createStubFileForm.ShowDialog();
        }
      
  
        private void uninstallDriverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterAPI.StopFilter();
            FilterAPI.UnInstallDriver();
        }

        private void installDriverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterAPI.InstallDriver();
        }



        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterAPI.StopFilter();
            GlobalConfig.Stop();
            Application.Exit();
        }



        private void demoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FilterAPI.StopFilter();
            GlobalConfig.Stop();
        }

        private void aboutCloudTierDemoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutUsForm aboutUs = new AboutUsForm();
            aboutUs.ShowDialog();

        }
     

        private void CloudTierDemoForm_Shown(object sender, EventArgs e)
        {
            if (!isMessageDisplayed)
            {
                isMessageDisplayed = true;
                CreateTestStubFileForms.CreateTestSourceFiles();
                MessageBox.Show("You can test the local stub files which were created in local test folder " 
                    + CreateTestStubFileForms.testStubFilesFolder 
                    + ", to create more stub files, Please go to Tools->Create test stub file.\r\n\r\n" 
                    + "To test the stub files which link to the cloud storage, you need to configure the cloud connection settings first, then go to the cloud explorer,"
                    + "select the files and click 'create stub file' there.\r\n\r\n"  
                    + "To open the stub file, you need to start the filter service first.");
            }
        }

        private void toolStripButton_CloudExplorer_Click(object sender, EventArgs e)
        {
            CloudExplorer cloudExplorer = new CloudExplorer();
            cloudExplorer.StartPosition = FormStartPosition.CenterParent;
            cloudExplorer.ShowDialog();
        }

        private void toolStripButton_CloudSettings_Click(object sender, EventArgs e)
        {
            SiteManagerForm siteManagerForm = new SiteManagerForm();
            siteManagerForm.StartPosition = FormStartPosition.CenterParent;
            siteManagerForm.ShowDialog();
        }

        private void aboutReparsePointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://easefilter.com/forums_files/Reparse%20Points.htm");
        }

        private void aboutSparseFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://easefilter.com/forums_files/sparsefile.htm");
        }

        private void cloudTierSDKSolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.easefilter.com/Forums_Files/CloudStorageMigration.htm");
        }


        private void reportAProblemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.easefilter.com/ReportIssue.htm");
        }

        private void helpTopicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://easefilter.com/info/cloudtier_manual.pdf");
        }

      
    }
}
