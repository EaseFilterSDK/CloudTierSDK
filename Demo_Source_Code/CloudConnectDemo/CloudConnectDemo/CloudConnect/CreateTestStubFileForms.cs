using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

using EaseFilter.GlobalObjects;

namespace CloudConnect
{
    public partial class CreateTestStubFileForms : Form
    {
        public static string testFileSourceFolder = GlobalConfig.testSourceFolder;
        public static string testStubFilesFolder = GlobalConfig.testStubFilesFolder;
        static int totalStubFile = 0;

        public CreateTestStubFileForms()
        {
            InitializeComponent();
            textBox_SourceFolder.Text = testFileSourceFolder;
            textBox_StubFolder.Text = testStubFilesFolder;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            testFileSourceFolder = textBox_SourceFolder.Text;
            testStubFilesFolder = textBox_StubFolder.Text;

            CreateTestStubFiles(testFileSourceFolder);

            MessageBox.Show(totalStubFile + " stub files were created, Please start the filter service and test the stub file in folder " + testStubFilesFolder, "StubFile", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        static public void CreateTestSourceFiles()
        {
            if (!Directory.Exists(testFileSourceFolder))
            {
                Directory.CreateDirectory(testFileSourceFolder);
            }

            //create the test file here.                    
            for (int i = 1; i < 5; i++)
            {
                string testStr = string.Empty;
                byte[] buffer = new byte[i * 10240];
                for (int j = 0; j < buffer.Length; j++)
                {
                    int rem = 0;
                    Math.DivRem(j, 26, out rem);
                    testStr += (char)('a' + rem);
                    if (rem == 25)
                    {
                        testStr += Environment.NewLine;
                    }
                }

                string testFileName = Path.Combine(testFileSourceFolder, "testFile." + i.ToString() + ".txt");
                File.AppendAllText(testFileName, testStr);
            }


            CreateTestStubFiles(testFileSourceFolder);
        }

        static public bool CreateTestStubFiles(string folder)
        {

            try
            {
                string[] dirs = Directory.GetDirectories(folder);

                bool ret = false;

                foreach (string dir in dirs)
                {
                    CreateTestStubFiles(dir);
                }

                string[] files = Directory.GetFiles(folder);

                foreach (string file in files)
                {
                    string stubFileName = testStubFilesFolder + file.Substring(testFileSourceFolder.Length);

                    string stubFolder = Path.GetDirectoryName(stubFileName);
                    if (!Directory.Exists(stubFolder))
                    {
                        Directory.CreateDirectory(stubFolder);
                    }

                    if (File.Exists(stubFileName))
                    {
                        File.Delete(stubFileName);
                    }

                    FileInfo fileInfo = new FileInfo(file);

                    byte[] tagData = ASCIIEncoding.Unicode.GetBytes(file);
                    GCHandle gcHandle = GCHandle.Alloc(tagData, GCHandleType.Pinned);
                    IntPtr fileHandle = IntPtr.Zero;

                    try
                    {
                        ret = FilterAPI.CreateStubFile(stubFileName, fileInfo.Length, (uint)FileAttributes.Offline, (uint)tagData.Length, Marshal.UnsafeAddrOfPinnedArrayElement(tagData, 0), true, ref fileHandle);
                        if (!ret)
                        {
                            Console.WriteLine("Create stub file:" + stubFileName + " failed.\n" + FilterAPI.GetLastErrorMessage());
                            return ret;
                        }
                    }
                    finally
                    {
                        gcHandle.Free();

                        if (fileHandle != IntPtr.Zero)
                        {
                            FilterAPI.CloseHandle(fileHandle);
                        }
                    }

                    totalStubFile++;

                }

                return ret;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Create test stub file got exception:" + ex.Message, "StubFile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        private void button_BrowseSoruceFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog diagLog = new FolderBrowserDialog();
            if (diagLog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_SourceFolder.Text = diagLog.SelectedPath;
            }
        }

        private void button_BrowseStubFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog diagLog = new FolderBrowserDialog();
            if (diagLog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_StubFolder.Text = diagLog.SelectedPath;
            }
        }

    }
}
