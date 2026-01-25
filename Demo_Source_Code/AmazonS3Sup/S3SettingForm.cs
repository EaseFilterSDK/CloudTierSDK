using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CloudTier.CommonObjects;

namespace CloudTier.AmazonS3Sup
{
    public partial class S3SettingsForm : Form
    {
            AmazonS3SiteInfo s3SiteInfo = new AmazonS3SiteInfo();

            public S3SettingsForm()
            {
                //initialize the new s3 site info
                s3SiteInfo.SiteName = "MyAmazonS3Site1";
                s3SiteInfo.LocalPath = "c:\\" + s3SiteInfo.SiteName;
                s3SiteInfo.RemotePath = "";
                s3SiteInfo.ParallelTasks = 1;
                s3SiteInfo.BufferSize = 65536;
                s3SiteInfo.PartSize = 5 * 1024 * 1024;
                s3SiteInfo.RegionName = Amazon.RegionEndpoint.USEast1.DisplayName;

                InitializeComponent();
                InitOptionForm();
            }

            public S3SettingsForm(AmazonS3SiteInfo _s3SiteInfo)
            {
                s3SiteInfo = _s3SiteInfo;

                InitializeComponent();
                InitOptionForm();
            }

            private void InitOptionForm()
            {
                try
                {
                    comboBox_S3Region.Items.Clear();
                    foreach (Amazon.RegionEndpoint endpoint in Amazon.RegionEndpoint.EnumerableAllRegions)
                    {
                        comboBox_S3Region.Items.Add(endpoint.DisplayName);
                    }

                    comboBox_S3Region.Text = s3SiteInfo.RegionName;
                    textBox_LocalPath.Text = s3SiteInfo.LocalPath;
                    textBox_RemotePath.Text = s3SiteInfo.RemotePath;
                    textBox_S3AccessKeyId.Text = s3SiteInfo.AccessKeyId;
                    textBox_S3SecretKey.Text = s3SiteInfo.SecretAccessKey;
                    textBox_S3BufferSize.Text = s3SiteInfo.BufferSize.ToString();
                    textBox_S3ParallelTasks.Text = s3SiteInfo.ParallelTasks.ToString();
                    textBox_S3PartSize.Text = s3SiteInfo.PartSize.ToString();
                    textBox_S3SiteName.Text = s3SiteInfo.SiteName;
                    checkBox_S3EnableMultiPartsUpload.Checked = s3SiteInfo.EnabledMultiBlocksUpload;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Initialize the option form failed with error " + ex.Message, "Init options.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            /// <summary>
            /// Validate all the input fields
            /// </summary>
            /// <param name="lastError">if there are some invalid field,it will return false and out the error message</param>
            /// <returns></returns>
            private bool ValidateS3Setting(out string lastError)
            {
                lastError = string.Empty;

                if (textBox_S3SiteName.ReadOnly == false)
                {
                    //only not readonly we need to validate the following inputs;


                    if (string.IsNullOrEmpty(textBox_S3SiteName.Text))
                    {
                        lastError = "site name can't be empty";
                        return false;
                    }

                    if (string.IsNullOrEmpty(textBox_S3AccessKeyId.Text))
                    {
                        lastError = "access key id can't be empty";
                        return false;
                    }

                    if (string.IsNullOrEmpty(textBox_S3SecretKey.Text))
                    {
                        lastError = "secrect key can't be empty";
                        return false;
                    }

                    if (string.IsNullOrEmpty(comboBox_S3Region.Text))
                    {
                        lastError = "region name can't be empty";
                        return false;
                    }
                    else
                    {
                        bool findRegion = false;

                        foreach (Amazon.RegionEndpoint endpoint in Amazon.RegionEndpoint.EnumerableAllRegions)
                        {
                            if (comboBox_S3Region.Text.Equals(endpoint.DisplayName))
                            {
                                findRegion = true;
                                break;
                            }
                        }

                        if (!findRegion)
                        {
                            lastError = "Region " + comboBox_S3Region.Text + " is invalid.";
                            return findRegion;
                        }


                    }

                    if (string.IsNullOrEmpty(textBox_S3BufferSize.Text))
                    {
                        lastError = "buffer size can't be empty";
                        return false;
                    }

                    if (string.IsNullOrEmpty(textBox_S3PartSize.Text))
                    {
                        lastError = "part size can't be empty";
                        return false;
                    }

                }

                return true;

            }

        private bool GetS3SiteInfo()
        {
            try
            {
                string lastError = string.Empty;

                if (!ValidateS3Setting(out lastError))
                {
                    MessageBox.Show("Save S3 setting failed with error " + lastError, "Save S3 setting.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                s3SiteInfo.SiteName = textBox_S3SiteName.Text;
                s3SiteInfo.LocalPath = textBox_LocalPath.Text;
                s3SiteInfo.RemotePath = textBox_RemotePath.Text;
                s3SiteInfo.AccessKeyId = textBox_S3AccessKeyId.Text;
                s3SiteInfo.SecretAccessKey = textBox_S3SecretKey.Text;
                s3SiteInfo.RegionName = comboBox_S3Region.Text;

                s3SiteInfo.BufferSize = (uint.Parse(textBox_S3BufferSize.Text));
                if (s3SiteInfo.BufferSize > 640 * 1024 * 1024 || s3SiteInfo.BufferSize < 512)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("the single buffer size is 512 - 671088640",
                                   "Update site info", MessageBoxButtons.OK,
                                   MessageBoxIcon.Error);
                    return false;
                }


                s3SiteInfo.PartSize = (uint.Parse(textBox_S3PartSize.Text));
                if (s3SiteInfo.PartSize < 5 * 1024 * 1024 || s3SiteInfo.PartSize > 1024 * 1024 * 1024)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("the single upload part size is 5M - 1GB",
                                   "Update site info", MessageBoxButtons.OK,
                                   MessageBoxIcon.Error);
                    return false;
                }

                s3SiteInfo.EnabledMultiBlocksUpload = checkBox_S3EnableMultiPartsUpload.Checked;

                s3SiteInfo.ParallelTasks = uint.Parse(textBox_S3ParallelTasks.Text);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Configure S3 setting failed with error " + ex.Message, "Configure S3 setting.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        private void button_ApplySettings_Click(object sender, EventArgs e)
        {
            if (GetS3SiteInfo())
            {
                S3Config.AddAmazonS3SiteInfo(s3SiteInfo);
                S3Config.SaveConfigSetting();

                this.Close();
            }
        }

        private void button_TestConnection_Click(object sender, EventArgs e)
        {
            string lastError = string.Empty;

            if (GetS3SiteInfo())
            {
                if (!CloudUtil.TestRemotePath(s3SiteInfo, s3SiteInfo.RemotePath, out lastError))
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                    MessageBox.Show(lastError, "Test Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("Connection passed.", "Test Connection", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void button_RestoreStubFiles_Click(object sender, EventArgs e)
        {

        }
    }
}
