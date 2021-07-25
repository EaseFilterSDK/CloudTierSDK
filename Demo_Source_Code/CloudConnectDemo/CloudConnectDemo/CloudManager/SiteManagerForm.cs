
///////////////////////////////////////////////////////////////////////////////
//
//    (C) Copyright 2011 EaseFilter Technologies
//    All Rights Reserved
//
//    This software is part of a licensed software product and may
//    only be used or copied in accordance with the terms of that license.
//
//    NOTE:  THIS MODULE IS UNSUPPORTED SAMPLE CODE
//
//    This module contains sample code provided for convenience and
//    demonstration purposes only,this software is provided on an 
//    "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
//     either express or implied.  
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using EaseFilter.GlobalObjects;

namespace EaseFilter.CloudManager
{
    public partial class SiteManagerForm : Form
    {
        internal static Dictionary<string, SiteInfo> siteInfos = null;

        internal SiteInfo selectedSiteInfo = null;
        internal CloudProviderType selectedProvider = CloudProviderType.AzureStorage;

        public SiteManagerForm()
        {
            InitializeComponent();

            try
            {
                siteInfos = GlobalConfig.SiteInfos;
                GetGeneralSetting();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("Load config file failed with error:" + ex.Message);
                siteInfos = new Dictionary<string, SiteInfo>();
            }

            UpdateTreeNode();

            dynamic version;
            if (!Utils.TryGeRegistryKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full", "Release", out version))
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("To connect the cloud storage, it requires the .NET Framework 4.5 or later version.","Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning) ;
            }

        }



        /// <summary>
        /// if no select site or group, it will disable interface,else output the select info to all the fields
        /// </summary>
        private void UpdateInterface()
        {
            try
            {
                if (selectedSiteInfo == null)
                {
                    tabControl_SiteSetting.SelectedTab = tabPage_GeneralSetting;
                }
                else
                {
                    switch (selectedSiteInfo.CloudProvider)
                    {

                        case CloudProviderType.AzureStorage:
                            {
                                tabControl_SiteSetting.SelectedTab = tabPage_Azure;
                                EnableAzureSetting();
                                GetAzureSetting();

                                break;
                            }
                        case CloudProviderType.Amazon_S3:
                            {
                                tabControl_SiteSetting.SelectedTab = tabPage_AmazonS3;

                                EnableS3Setting();
                                GetS3Setting();

                                break;
                            }

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Update interface failed:" + ex.Message, "UpdateInterface", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// recreate the tree node with site list info, and update the interface base on the site list info
        /// </summary>
        private void UpdateTreeNode()
        {
            try
            {
                treeView_SiteList.Nodes.Clear();
                TreeNode selectedNode = null;


                //add cloud provider nodes.
                foreach (CloudProviderType cloudProvider in Enum.GetValues(typeof(CloudProviderType)))
                {

                    TreeNode node = treeView_SiteList.Nodes.Add(cloudProvider.ToString(), cloudProvider.ToString(), 0, 1);

                }


                foreach (SiteInfo siteInfo in siteInfos.Values)
                {
                    //look for the clouder provider name
                    TreeNode[] nodes = treeView_SiteList.Nodes.Find(siteInfo.CloudProvider.ToString(), false);

                    if (nodes.Length != 1)
                    {
                        MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("Can't find the cloud provider name:" + siteInfo.CloudProvider,
                                "Update tree node", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        return;
                    }

                    TreeNode node = nodes[0].Nodes.Add(siteInfo.SiteName, siteInfo.SiteName, 2, 3);

                    if (siteInfo.Selected)
                    {
                        selectedNode = node;

                    }
                }


                treeView_SiteList.ExpandAll();

                if (selectedNode != null)
                {
                    treeView_SiteList.SelectedNode = selectedNode;

                }

                UpdateInterface();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Update tree node failed:" + ex.Message, "UpdateTreeNode", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// update the site list with the select info.
        /// </summary>
        private void UpdateSelectedSiteInfo()
        {

            if (selectedSiteInfo == null)
            {
                return;
            }


            foreach (SiteInfo siteInfo in siteInfos.Values)
            {
                if (string.Compare(siteInfo.SiteName, selectedSiteInfo.SiteName) == 0)
                {
                    siteInfo.Selected = true;
                }
                else
                {
                    siteInfo.Selected = false;
                }
            }
        }



        private void GetGeneralSetting()
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

            textBox_CacheFolder.Text = GlobalConfig.CloudCacheFolder;
            textBox_ExpireCacheFileTTL.Text = GlobalConfig.ExpireCachedDirectoryListingAfterSeconds.ToString();

            textBox_DeleteCachedFilesTTL.Text = GlobalConfig.DeleteCachedFilesAfterSeconds.ToString();

            checkBox_ClearCacheOnConnect.Checked = GlobalConfig.DeleteCachedFilesOnConnect;


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

                GlobalConfig.CloudCacheFolder = textBox_CacheFolder.Text;
                GlobalConfig.ExpireCachedDirectoryListingAfterSeconds = int.Parse(textBox_ExpireCacheFileTTL.Text);

                GlobalConfig.DeleteCachedFilesAfterSeconds = int.Parse(textBox_DeleteCachedFilesTTL.Text);
                GlobalConfig.DeleteCachedFilesOnConnect = checkBox_ClearCacheOnConnect.Checked;

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


        /// <summary>
        /// Make all interface input to readonly,and empty all the input fields
        /// </summary>
        /// <param name="disableGroupName">if true,it will make group name input readonly,else not</param>
        private void DisableAzureSetting()
        {
            groupBox_Azure.Enabled = false;

        }

        /// <summary>
        /// Make all the input fields readonly false
        /// </summary>
        /// <param name="disableGroupName">if true,it will make group name input readonly,else not</param>
        private void EnableAzureSetting()
        {
            //General infomation      
            groupBox_Azure.Enabled = true;

        }


        /// <summary>
        /// set the input value from selected site list
        /// </summary>
        private void GetAzureSetting()
        {
            //General infomation          
            textBox_AzureSiteName.Text = selectedSiteInfo.SiteName;
            textBox_AzureConnectionString.Text = selectedSiteInfo.AzureConnectionString;

            textBox_AzureBufferSize.Text = selectedSiteInfo.BufferSize.ToString();
            textBox_AzureBlobSize.Text = selectedSiteInfo.AzureMaxBlockSize.ToString();

            checkBox_AzureEnableMultiBlocksUpload.Checked = selectedSiteInfo.EnabledMultiBlocksUpload;
            checkBox_AzureEnableParallelDownload.Checked = selectedSiteInfo.EnabledMultiBlocksDownload;
            textBox_AzureParallelTasks.Text = selectedSiteInfo.ParallelTasks.ToString();

          
        }



        /// <summary>
        /// Validate all the input fields
        /// </summary>
        /// <param name="lastError">if there are some invalid field,it will return false and out the error message</param>
        /// <returns></returns>
        private bool ValidateAzureSetting(out string lastError)
        {
            lastError = string.Empty;

            if (textBox_AzureSiteName.ReadOnly == false)
            {
                //only not readonly we need to validate the following inputs;


                if (string.IsNullOrEmpty(textBox_AzureSiteName.Text))
                {
                    lastError = "site name can't be empty";
                    return false;
                }
                           

                if (string.IsNullOrEmpty(textBox_AzureConnectionString.Text))
                {
                    lastError = "connection string can't be empty";
                    return false;
                }             

                if (string.IsNullOrEmpty(textBox_AzureBufferSize.Text))
                {
                    lastError = "buffer size can't be empty";
                    return false;
                }

                if (string.IsNullOrEmpty(textBox_AzureBlobSize.Text))
                {
                    lastError = "single blob size can't be empty";
                    return false;
                }

             
            }

            return true;

        }


        /// <summary>
        /// set all the site info value with the input fields.
        /// </summary>
        /// <param name="siteInfo">the site info class which will accept the value</param>
        /// <returns></returns>
        private bool SetAzureSiteInfo(ref SiteInfo siteInfo)
        {
            //General infomation
            string lastError = string.Empty;
            try
            {
                if (!ValidateAzureSetting(out lastError))
                {
                    throw new Exception(lastError);
                }

                siteInfo.CloudProvider = CloudProviderType.AzureStorage;
                siteInfo.Selected = true;
                siteInfo.SiteName = textBox_AzureSiteName.Text;
                siteInfo.AzureConnectionString = textBox_AzureConnectionString.Text;

                siteInfo.BufferSize = (int)(uint.Parse(textBox_AzureBufferSize.Text));
                if (siteInfo.BufferSize > 640 * 1024 * 1024 || siteInfo.BufferSize < 512)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("the single buffer size is 512 - 671088640",
                                   "Update site info", MessageBoxButtons.OK,
                                   MessageBoxIcon.Error);
                    return false;
                }


                siteInfo.AzureMaxBlockSize = (int)(uint.Parse(textBox_AzureBlobSize.Text));
                if (siteInfo.AzureMaxBlockSize > 4 * 1024 * 1024 || siteInfo.AzureMaxBlockSize < 65536)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("the single block size is 65536 - 4194304",
                                   "Update site info", MessageBoxButtons.OK,
                                   MessageBoxIcon.Error);
                    return false;
                }

                siteInfo.EnabledMultiBlocksUpload = checkBox_AzureEnableMultiBlocksUpload.Checked;
                siteInfo.EnabledMultiBlocksDownload = checkBox_AzureEnableParallelDownload.Checked;
                siteInfo.ParallelTasks = int.Parse(textBox_AzureParallelTasks.Text);

            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("Can't save the site info,some fields are invalid:" + ex.Message,
                                     "Update site info", MessageBoxButtons.OK,
                                     MessageBoxIcon.Error);
                return false;

            }

            return true;

        }

        /// <summary>
        /// Make all interface input to readonly,and empty all the input fields
        /// </summary>
        /// <param name="disableGroupName">if true,it will make group name input readonly,else not</param>
        private void DisableS3Setting()
        {
            groupBox_S3.Enabled = false;

        }

        /// <summary>
        /// Make all the input fields readonly false
        /// </summary>
        /// <param name="disableGroupName">if true,it will make group name input readonly,else not</param>
        private void EnableS3Setting()
        {
            //General infomation      
            groupBox_S3.Enabled = true;

            comboBox_S3Region.Items.Clear();
            foreach (Amazon.RegionEndpoint endpoint in Amazon.RegionEndpoint.EnumerableAllRegions)
            {
                comboBox_S3Region.Items.Add(endpoint.DisplayName);
            }
         
        }

        /// <summary>
        /// set the input value from selected site list
        /// </summary>
        private void GetS3Setting()
        {
            //General infomation          
            textBox_S3SiteName.Text = selectedSiteInfo.SiteName;
            textBox_S3AccessKeyId.Text = selectedSiteInfo.S3AccessKeyId;
            textBox_S3SecretKey.Text = selectedSiteInfo.S3SecretKey;
            comboBox_S3Region.Text = selectedSiteInfo.S3Region;

            textBox_S3BufferSize.Text = selectedSiteInfo.BufferSize.ToString();
            textBox_S3PartSize.Text = selectedSiteInfo.S3MaxPartSize.ToString();

            checkBox_S3EnableMultiPartsUpload.Checked = selectedSiteInfo.EnabledMultiBlocksUpload;
            checkBox_S3EnableParallelDownload.Checked = selectedSiteInfo.EnabledMultiBlocksDownload;
            textBox_S3ParallelTasks.Text = selectedSiteInfo.ParallelTasks.ToString();

            if (checkBox_S3EnableMultiPartsUpload.Checked)
            {
                textBox_S3PartSize.ReadOnly = false;
            }
            else
            {
                textBox_S3PartSize.ReadOnly = true;
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

        /// <summary>
        /// set all the site info value with the input fields.
        /// </summary>
        /// <param name="siteInfo">the site info class which will accept the value</param>
        /// <returns></returns>
        private bool SetS3SiteInfo(ref SiteInfo siteInfo)
        {
            string lastError = string.Empty;

            try
            {
                if (!ValidateS3Setting(out lastError))
                {
                    throw new Exception(lastError);
                }

                siteInfo.CloudProvider = CloudProviderType.Amazon_S3;
                siteInfo.Selected = true;
                siteInfo.SiteName = textBox_S3SiteName.Text;
                siteInfo.S3AccessKeyId = textBox_S3AccessKeyId.Text;
                siteInfo.S3SecretKey = textBox_S3SecretKey.Text;
                siteInfo.S3Region = comboBox_S3Region.Text;

                siteInfo.BufferSize = (int)(uint.Parse(textBox_S3BufferSize.Text));
                if (siteInfo.BufferSize > 640 * 1024 * 1024 || siteInfo.BufferSize < 512)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("the single buffer size is 512 - 671088640",
                                   "Update site info", MessageBoxButtons.OK,
                                   MessageBoxIcon.Error);
                    return false;
                }


                siteInfo.S3MaxPartSize = (int)(uint.Parse(textBox_S3PartSize.Text));
                if (siteInfo.S3MaxPartSize < 5 * 1024 * 1024 || siteInfo.S3MaxPartSize > 1024 * 1024 * 1024)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("the single upload part size is 5M - 1GB",
                                   "Update site info", MessageBoxButtons.OK,
                                   MessageBoxIcon.Error);
                    return false;
                }

                if (siteInfo.ConnectionTimeout > 600 || siteInfo.ConnectionTimeout < 1)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("the ConnectionTimeout range is 1- 600",
                                   "Update site info", MessageBoxButtons.OK,
                                   MessageBoxIcon.Error);
                    return false;
                }

                siteInfo.EnabledMultiBlocksUpload = checkBox_S3EnableMultiPartsUpload.Checked;
                siteInfo.EnabledMultiBlocksDownload = checkBox_S3EnableParallelDownload.Checked;
                siteInfo.ParallelTasks = int.Parse(textBox_S3ParallelTasks.Text);

            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("Can't save the site info,some fields are invalid:" + ex.Message,
                                     "Update site info", MessageBoxButtons.OK,
                                     MessageBoxIcon.Error);
                return false;

            }

            return true;

        }


        private void button_BrowseCacheFolder_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == folderBrowserDialog_LocalPath.ShowDialog())
            {
                textBox_CacheFolder.Text = folderBrowserDialog_LocalPath.SelectedPath;
            }
        }


        private void button_AddSite_Click(object sender, EventArgs e)
        {
            try
            {
                if (null == treeView_SiteList.SelectedNode)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                    MessageBox.Show("Please select a cloud provider", "AddSite", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                SiteInfo siteInfo = new SiteInfo();
                siteInfo.CloudProvider = selectedProvider;

                int i = 0;
                int maxDefaultName = 1000;

                for (i = 0; i < maxDefaultName; i++)
                {
                    siteInfo.SiteName = "New Site Name " + i;
                    siteInfo.Selected = true;

                    if (!siteInfos.ContainsKey(siteInfo.SiteName))
                    {
                        break;
                    }

                }

                if (i > maxDefaultName - 1)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("The maximum default site name is: " + maxDefaultName, "Add site", MessageBoxButtons.OK,
                                     MessageBoxIcon.Error);

                    return;
                }

                siteInfo.Selected = true;
                selectedSiteInfo = siteInfo;
                UpdateSelectedSiteInfo();
                UpdateTreeNode();

                button_CopyItem.Enabled = false;
                button_TestConnection.Enabled = false;
                button_AddSite.Enabled = false;
                button_Apply.Enabled = true;
                button_Delete.Enabled = false;
                button_Cancel.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Add new site failed:" + ex.Message, "AddSite", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_CopyItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedSiteInfo == null)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("Please select a site name!");
                    return;
                }


                SiteInfo siteInfo = new SiteInfo(selectedSiteInfo.siteInfoSettings);

                int i = 0;
                int maxDefaultName = 1000;

                for (i = 0; i < maxDefaultName; i++)
                {
                    siteInfo.SiteName = "Copy of " + selectedSiteInfo.SiteName + i;
                    if (!siteInfos.ContainsKey(siteInfo.SiteName))
                    {
                        break;
                    }
                }

                if (i > maxDefaultName - 1)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("The maximum default site name is: " + maxDefaultName, "Add site", MessageBoxButtons.OK,
                                     MessageBoxIcon.Error);

                    return;
                }

                siteInfo.Selected = true;
                selectedSiteInfo = siteInfo;

                UpdateSelectedSiteInfo();
                UpdateTreeNode();

                button_TestConnection.Enabled = false;
                button_AddSite.Enabled = false;
                button_Apply.Enabled = true;
                button_Delete.Enabled = false;
                button_Cancel.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Copy site failed:" + ex.Message, "CopySite", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                string message = string.Empty;
                string caption = "Confirm to delete group";
                DialogResult result = DialogResult.Cancel;

                if (selectedSiteInfo == null)
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("Please select a site name!");
                    return;
                }

                message = "Are you sure you wan to delete this site \" " + selectedSiteInfo.SiteName + "\"";

                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                result = MessageBox.Show(this, message, caption, MessageBoxButtons.YesNoCancel,
                                                          MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
                                                          MessageBoxOptions.RightAlign);

                if (DialogResult.Yes == result)
                {
                    if (siteInfos.ContainsKey(selectedSiteInfo.SiteName))
                    {
                        siteInfos.Remove(selectedSiteInfo.SiteName);
                    }

                    selectedSiteInfo = null;
                    foreach (SiteInfo siteInfo in siteInfos.Values)
                    {
                        siteInfo.Selected = false;
                        selectedSiteInfo = siteInfo;
                    }

                    //if there are alvalable siteinfo, set the last one as the selected one.
                    if (selectedSiteInfo != null)
                    {
                        selectedSiteInfo.Selected = true;
                    }
                }

                UpdateSelectedSiteInfo();
                UpdateTreeNode();

                GlobalConfig.SaveConfigInfo();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Delete site failed:" + ex.Message, "DeleteSite", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button_TestConnection_Click(object sender, EventArgs e)
        {
            string lastError = string.Empty;

            if (!CloudUtil.TestRemotePath(selectedSiteInfo, selectedSiteInfo.RemotePath, out lastError))
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show(lastError, "Test Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this); MessageBox.Show("Connection passed.", "Test Connection", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void button_Apply_Click(object sender, EventArgs e)
        {
            try
            {
                string message = "Are you sure you wan to save the configuration?";
                string caption = " Save configuration";

                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);

                if (DialogResult.Yes != MessageBox.Show(this, message, caption, MessageBoxButtons.YesNoCancel,
                                                          MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
                                                          MessageBoxOptions.RightAlign))
                {
                    return;
                }


                if (tabControl_SiteSetting.SelectedTab == tabPage_GeneralSetting)
                {
                    SetGeneralSetting();
                    GlobalConfig.SaveConfigInfo();
                    return;
                }
                else if (selectedSiteInfo != null)
                {
                    SiteInfo siteInfo = selectedSiteInfo;

                    string selectedSiteName = siteInfo.SiteName;

                    bool retVal = false;
                    switch (selectedSiteInfo.CloudProvider)
                    {

                        case CloudProviderType.AzureStorage:
                            {
                                retVal = SetAzureSiteInfo(ref siteInfo);
                                break;
                            }
                        case CloudProviderType.Amazon_S3:
                            {
                                retVal = SetS3SiteInfo(ref siteInfo);
                                break;
                            }
                    }


                    if (retVal)
                    {
                        selectedSiteInfo = siteInfo;

                        if (siteInfos.ContainsKey(siteInfo.SiteName))
                        {
                            siteInfo = (SiteInfo)siteInfos[selectedSiteInfo.SiteName];
                            siteInfos.Remove(siteInfo.SiteName);
                        }

                        siteInfos.Add(siteInfo.SiteName, siteInfo);


                        GlobalConfig.SaveConfigInfo();

                        UpdateSelectedSiteInfo();
                        UpdateTreeNode();

                        button_CopyItem.Enabled = true;
                        button_TestConnection.Enabled = true;
                        button_AddSite.Enabled = true;
                        button_Apply.Enabled = true;
                        button_Delete.Enabled = true;
                        button_Cancel.Enabled = true;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Apply site setting failed:" + ex.Message, "ApplySetting", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            try
            {
                siteInfos = GlobalConfig.SiteInfos;
                UpdateTreeNode();

                button_TestConnection.Enabled = true;
                button_AddSite.Enabled = true;
                button_Apply.Enabled = true;
                button_Delete.Enabled = true;
                button_Cancel.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Cancel failed:" + ex.Message, "Cancel", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void treeView_SiteList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                TreeNode node = treeView_SiteList.SelectedNode;

                if (node.Level > 0)
                {
                    if (siteInfos.ContainsKey(node.Text))
                    {
                        selectedSiteInfo = siteInfos[node.Text];
                        selectedProvider = selectedSiteInfo.CloudProvider;
                    }
                }
                else
                {
                    selectedProvider = (CloudProviderType)Enum.Parse(typeof(CloudProviderType), node.Text);
                }

                UpdateSelectedSiteInfo();
                UpdateInterface();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Seleted site failed:" + ex.Message, "SiteSeleted", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
