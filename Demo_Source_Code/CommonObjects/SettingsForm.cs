using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CloudTier.CommonObjects
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
                textBox_Timeout.Text = GlobalConfig.ConnectionTimeOut.ToString();
                textBox_MaximumFilterMessage.Text = GlobalConfig.MaximumFilterMessages.ToString();
                radioButton_Rehydrate.Checked = GlobalConfig.RehydrateFileOnFirstRead;
                radioButton_CacheFile.Checked = GlobalConfig.ReturnCacheFileName;
                radioButton_Block.Checked = GlobalConfig.ReturnBlockData;
                checkBox_ReOpenFileOnReHydration.Checked = GlobalConfig.ByPassWriteEventOnReHydration;

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

                GlobalConfig.ConnectionTimeOut = int.Parse(textBox_Timeout.Text);
                GlobalConfig.FilterConnectionThreads = uint.Parse(textBox_Threads.Text);
                GlobalConfig.MaximumFilterMessages = int.Parse(textBox_MaximumFilterMessage.Text);
                GlobalConfig.RehydrateFileOnFirstRead = radioButton_Rehydrate.Checked;
                GlobalConfig.ReturnCacheFileName = radioButton_CacheFile.Checked;
                GlobalConfig.ReturnBlockData = radioButton_Block.Checked;
                GlobalConfig.ByPassWriteEventOnReHydration = checkBox_ReOpenFileOnReHydration.Checked;

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

       
    
     
    }
}
