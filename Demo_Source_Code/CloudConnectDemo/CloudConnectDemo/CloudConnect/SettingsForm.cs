using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EaseFilter.GlobalObjects;
using EaseFilter.CloudManager;

namespace CloudConnect
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
                textBox_Threads.Text = GlobalConfig.FilterConnectionThreads.ToString();
                textBox_Timeout.Text = GlobalConfig.FileSystemWaitTimeoutInSeconds.ToString();
                textBox_MaximumFilterMessage.Text = GlobalConfig.MaximumFilterMessages.ToString();
                radioButton_Rehydrate.Checked = GlobalConfig.RehydrateFileOnFirstRead;
                radioButton_CacheFile.Checked = GlobalConfig.ReturnCacheFileName;
                radioButton_ReturnBlock.Checked = GlobalConfig.ReturnBlockData;

                foreach (uint pid in GlobalConfig.IncludePidList)
                {
                    if (textBox_IncludePID.Text.Length > 0)
                    {
                        textBox_IncludePID.Text += ";";
                    }

                    textBox_IncludePID.Text += pid.ToString();
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

        private void button_ApplyOptions_Click(object sender, EventArgs e)
        {
            try
            {

                GlobalConfig.FileSystemWaitTimeoutInSeconds = int.Parse(textBox_Timeout.Text);
                GlobalConfig.FilterConnectionThreads = int.Parse(textBox_Threads.Text);
                GlobalConfig.MaximumFilterMessages = int.Parse(textBox_MaximumFilterMessage.Text);
                GlobalConfig.RehydrateFileOnFirstRead = radioButton_Rehydrate.Checked;
                GlobalConfig.ReturnCacheFileName = radioButton_CacheFile.Checked;
                GlobalConfig.ReturnBlockData = radioButton_ReturnBlock.Checked;

                List<uint> inPids = new List<uint>();
                if (textBox_IncludePID.Text.Length > 0)
                {
                    if (textBox_IncludePID.Text.EndsWith(";"))
                    {
                        textBox_IncludePID.Text = textBox_IncludePID.Text.Remove(textBox_IncludePID.Text.Length - 1);
                    }

                    string[] pids = textBox_IncludePID.Text.Split(new char[] { ';' });
                    for (int i = 0; i < pids.Length; i++)
                    {
                        inPids.Add(uint.Parse(pids[i].Trim()));
                    }
                }
                 
                GlobalConfig.IncludePidList = inPids;

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

                GlobalConfig.SaveConfigInfo();

                this.Close();

            }
            catch (Exception ex)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Save options failed with error " + ex.Message, "Save options.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_SelectIncludePID_Click(object sender, EventArgs e)
        {

            OptionForm optionForm = new OptionForm(OptionForm.OptionType.ProccessId, textBox_IncludePID.Text);

            if (optionForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_IncludePID.Text = optionForm.ProcessId;
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

        private void button_ConfigCloudProvider_Click(object sender, EventArgs e)
        {
            SiteManagerForm siteManagerForm = new SiteManagerForm();
            siteManagerForm.StartPosition = FormStartPosition.CenterParent;
            siteManagerForm.ShowDialog();
        }
    }
}
