using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CloudConnect
{
    public partial class AboutUsForm : Form
    {
        public AboutUsForm()
        {
            InitializeComponent();

            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            label_Version.Text = "Version " + version;

            richTextBox_Info.Text = "To test the CloudTier demo features, you can do following steps:" + Environment.NewLine + Environment.NewLine;
            richTextBox_Info.Text += "1. Go to 'Tools->Create test stub files', creat the stub files for your test, you can have as many files as you want to your test purpose." + Environment.NewLine + Environment.NewLine;
            richTextBox_Info.Text += "2. Start the filter service, then the filter driver will intercept all the I/O to the stub files, and retrive data from the source files if you read the stub files." + Environment.NewLine;


        }
    }
}
