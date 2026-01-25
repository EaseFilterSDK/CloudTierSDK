using System;
using System.Collections.Generic;
using System.Windows.Forms;

using CloudTier.CommonObjects;
using CloudTier.AmazonS3Sup;

namespace CloudTierS3Demo
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            InitOptionForm();
        }

        private void InitOptionForm()
        {
            try
            {
                InitListView();

                textBox_CacheFolder.Text = GlobalConfig.CacheFolder;
                textBox_Threads.Text = GlobalConfig.FilterConnectionThreads.ToString();
                textBox_Timeout.Text = GlobalConfig.ConnectionTimeOut.ToString();
                radioButton_Rehydrate.Checked = GlobalConfig.RehydrateFileOnFirstRead;
                radioButton_CacheFile.Checked = GlobalConfig.ReturnCacheFileName;
                radioButton_Block.Checked = GlobalConfig.ReturnBlockData;

                foreach (EventLevel item in Enum.GetValues(typeof(EventLevel)))
                {
                    comboBox_EventLevel.Items.Add(item.ToString());

                    if (item.ToString().Equals(GlobalConfig.EventLevel.ToString()))
                    {
                        comboBox_EventLevel.SelectedItem = item.ToString();
                    }
                }

                foreach (uint pid in GlobalConfig.ExcludePidList)
                {
                    if (textBox_ExcludePID.Text.Length > 0)
                    {
                        textBox_ExcludePID.Text += ";";
                    }

                    textBox_ExcludePID.Text += pid.ToString();
                }

            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Initialize the option form failed with error " + ex.Message, "Init options.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void InitListView()
        {
            //init ListView control
            listView_S3SiteInfos.Clear();		//clear control
            //create column header for ListView
            listView_S3SiteInfos.Columns.Add("#", 20, System.Windows.Forms.HorizontalAlignment.Left);
            listView_S3SiteInfos.Columns.Add("SiteName", 150, System.Windows.Forms.HorizontalAlignment.Left);
            listView_S3SiteInfos.Columns.Add("LocalPath", 200, System.Windows.Forms.HorizontalAlignment.Left);
            listView_S3SiteInfos.Columns.Add("RemotePath", 100, System.Windows.Forms.HorizontalAlignment.Left);

            foreach (AmazonS3SiteInfo s3SiteInfo in S3Config.AmazonS3SiteInfos.Values)
            {
                AddItem(s3SiteInfo);
            }

        }

        private void AddItem(AmazonS3SiteInfo s3SiteInfo)
        {
            string[] itemStr = new string[listView_S3SiteInfos.Columns.Count];
            itemStr[0] = listView_S3SiteInfos.Items.Count.ToString();
            itemStr[1] = s3SiteInfo.SiteName;
            itemStr[2] = s3SiteInfo.LocalPath;
            itemStr[3] = s3SiteInfo.RemotePath;
            ListViewItem item = new ListViewItem(itemStr, 0);
            item.Tag = s3SiteInfo;
            listView_S3SiteInfos.Items.Add(item);
        }
       

        private void button_ApplyOptions_Click(object sender, EventArgs e)
        {
            try
            {

                GlobalConfig.ConnectionTimeOut = int.Parse(textBox_Timeout.Text);
                GlobalConfig.FilterConnectionThreads = uint.Parse(textBox_Threads.Text);
                GlobalConfig.EventLevel = (EventLevel)comboBox_EventLevel.SelectedIndex;
                GlobalConfig.RehydrateFileOnFirstRead = radioButton_Rehydrate.Checked;
                GlobalConfig.ReturnCacheFileName = radioButton_CacheFile.Checked;
                GlobalConfig.ReturnBlockData = radioButton_Block.Checked;

                List<uint> exPids = new List<uint>();
                if (textBox_ExcludePID.Text.Length > 0)
                {
                    if (textBox_ExcludePID.Text.EndsWith(";"))
                    {
                        textBox_ExcludePID.Text = textBox_ExcludePID.Text.Remove(textBox_ExcludePID.Text.Length - 1);
                    }

                    string[] pids = textBox_ExcludePID.Text.Split(new char[] { ';' });
                    for (int i = 0; i < pids.Length; i++)
                    {
                        exPids.Add(uint.Parse(pids[i].Trim()));
                    }
                }

                GlobalConfig.ExcludePidList = exPids;

                GlobalConfig.SaveConfigSetting();

                this.Close();

            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Save options failed with error " + ex.Message, "Save options.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button_SelectExcludePID_Click(object sender, EventArgs e)
        {

            OptionForm optionForm = new OptionForm(OptionForm.OptionType.ProccessId, textBox_ExcludePID.Text);

            if (optionForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_ExcludePID.Text = optionForm.ProcessId;
            }
        }

        private void button_BrowseCacheFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_CacheFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

      

        private void button_AddS3Site_Click_1(object sender, EventArgs e)
        {
            S3SettingsForm s3SettingForm = new S3SettingsForm();
            s3SettingForm.StartPosition = FormStartPosition.CenterParent;
            s3SettingForm.ShowDialog();

            InitListView();
        }

        private void button_EditS3Site_Click_1(object sender, EventArgs e)
        {
            if (listView_S3SiteInfos.SelectedItems.Count != 1)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Please select one site to edit.", "Edit site", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            System.Windows.Forms.ListViewItem item = listView_S3SiteInfos.SelectedItems[0];
            AmazonS3SiteInfo s3SiteInfo = ((AmazonS3SiteInfo)item.Tag);

            S3SettingsForm s3SettingForm = new S3SettingsForm(s3SiteInfo);
            s3SettingForm.StartPosition = FormStartPosition.CenterParent;
            s3SettingForm.ShowDialog();

            InitListView();
        }

        private void button_DeleteS3Site_Click(object sender, EventArgs e)
        {
            if (listView_S3SiteInfos.SelectedItems.Count == 0)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("There are no site selected.", "Delete site", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (System.Windows.Forms.ListViewItem item in listView_S3SiteInfos.SelectedItems)
            {
                AmazonS3SiteInfo s3SiteInfo = (AmazonS3SiteInfo)item.Tag;
                S3Config.RemoveAmazonS3SiteInfo(s3SiteInfo);
            }

            InitListView();
        }
    }
}
