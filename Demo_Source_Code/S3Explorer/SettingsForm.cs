using System;
using System.Collections.Generic;
using System.Windows.Forms;

using CloudTier.CommonObjects;
using CloudTier.AmazonS3Sup;

namespace CloudTier.S3Explorer
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
                InitGetGeneralSetting();
                InitListView();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Initialize the option form failed with error " + ex.Message, "Init options.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitGetGeneralSetting()
        {
            comboBox_EventLevel.Items.Clear();

            //General infomation
            foreach (EventLevel item in Enum.GetValues(typeof(EventLevel)))
            {
                comboBox_EventLevel.Items.Add(item.ToString());

                if (item.ToString().Equals(GlobalConfig.EventLevel.ToString()))
                {
                    comboBox_EventLevel.SelectedItem = item.ToString();
                }
            }


            foreach (EventOutputType item in Enum.GetValues(typeof(EventOutputType)))
            {
                comboBox_EventOutputType.Items.Add(item.ToString());

                if (item.ToString().Equals(GlobalConfig.EventOutputType.ToString()))
                {
                    comboBox_EventOutputType.SelectedItem = item.ToString();
                }
            }

            textBox_CacheFolder.Text = GlobalConfig.CacheFolder;
            textBox_ExpireCacheFileTTL.Text = GlobalConfig.ExpireCachedDirectoryListingAfterSeconds.ToString();
            textBox_DeleteCachedFilesTTL.Text = GlobalConfig.DeleteCachedFilesAfterSeconds.ToString();


        }

        /// <summary>
        /// Validate all the input fields
        /// </summary>
        /// <param name="lastError">if there are some invalid field,it will return false and out the error message</param>
        /// <returns></returns>
        private bool ValidateGeneralSetting(out string lastError)
        {
            lastError = string.Empty;

            Boolean retVal = false;
            foreach (EventLevel item in Enum.GetValues(typeof(EventLevel)))
            {
                if (item.ToString().Equals(comboBox_EventLevel.Text))
                {
                    GlobalConfig.EventLevel = item;
                    retVal = true;
                    break;
                }
            }

            if (!retVal)
            {
                lastError = "Can't save the general setting,event level is not valid.";
                return false;
            }

            retVal = false;
            foreach (EventOutputType item in Enum.GetValues(typeof(EventOutputType)))
            {
                if (item.ToString().Equals(comboBox_EventOutputType.Text))
                {
                    GlobalConfig.EventOutputType = item;
                    retVal = true;
                    break;
                }
            }

            if (!retVal)
            {
                lastError = "Can't save the general setting,event output type is not valid.";
                return false;
            }

            if (string.IsNullOrEmpty(textBox_DeleteCachedFilesTTL.Text))
            {
                lastError = "delete cached files time can't be empty";
                return false;
            }

            if (string.IsNullOrEmpty(textBox_CacheFolder.Text))
            {
                lastError = "cache folder can't be empty";
                return false;
            }


            if (string.IsNullOrEmpty(textBox_ExpireCacheFileTTL.Text))
            {
                lastError = "cache TTL can't be empty";
                return false;
            }

            return true;

        }


        /// <summary>
        /// set all the site info value with the input fields.
        /// </summary>
        /// <param name="siteInfo">the site info class which will accept the value</param>
        /// <returns></returns>
        private bool SetGeneralSetting()
        {
            string lastError = string.Empty;

            //General infomation
            try
            {

                if (!ValidateGeneralSetting(out lastError))
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("Can't save the general setting." + lastError, "Update general setting", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                GlobalConfig.CacheFolder = textBox_CacheFolder.Text;
                GlobalConfig.ExpireCachedDirectoryListingAfterSeconds = int.Parse(textBox_ExpireCacheFileTTL.Text);
                GlobalConfig.DeleteCachedFilesAfterSeconds = int.Parse(textBox_DeleteCachedFilesTTL.Text);

            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("Can't save the general setting,some fields are invalid:" + ex.Message,
                                     "Update general setting", MessageBoxButtons.OK,
                                     MessageBoxIcon.Error);
                return false;

            }

            return true;

        }


        private void button_ApplyOptions_Click(object sender, EventArgs e)
        {
            try
            {

                SetGeneralSetting();

                GlobalConfig.SaveConfigSetting();

                this.Close();

            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Save options failed with error " + ex.Message, "Save options.", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void button_AddS3Site_Click(object sender, EventArgs e)
        {
            S3SettingsForm s3SettingForm = new S3SettingsForm();
            s3SettingForm.StartPosition = FormStartPosition.CenterParent;
            s3SettingForm.ShowDialog();

            InitListView();
        }

        private void button_EditS3Site_Click(object sender, EventArgs e)
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

        private void button_BrowseCacheFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_CacheFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
