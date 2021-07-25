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

using EaseFilter.CommonObjects;

namespace CloudTierDemo
{
    public partial class CloudTierDemoForm : Form
    {
        FilterMessage filterMessage = null;

        //Purchase a license key with the link: http://www.easefilter.com/Order.htm
        //Email us to request a trial key: info@easefilter.com //free email is not accepted.
        string registerKey = GlobalConfig.registerKey;

        Boolean isMessageDisplayed = false;

        public CloudTierDemoForm()
        {
            InitializeComponent();

            Utils.CopyOSPlatformDependentFiles();

            StartPosition = FormStartPosition.CenterScreen;
            filterMessage = new FilterMessage(listView_Info);

            GlobalConfig.EventLevel = EventLevel.Verbose;

            DisplayVersion();

        }

        ~CloudTierDemoForm()
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
                    RequestHandler.ProcessRequest(messageSend,ref messageReply);

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
            TestStubFileForms testStubFileForm = new TestStubFileForms();
            testStubFileForm.ShowDialog();
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
     
        private void CloudTierDemoForm_Shown(object sender, EventArgs e)
        {
            if (!isMessageDisplayed)
            {
                isMessageDisplayed = true;
                TestStubFileForms.CreateTestFiles();
                MessageBox.Show("Some test stub files were created in folder " + TestStubFileForms.stubFilesFolder + ". You can test those stub files in test folder, if you want to create more stub files, you can go to 'Tools->Create test stub file' to create your own stub files.");
            }
        }



    }
}
