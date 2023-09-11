using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

using EaseFilter.CommonObjects;

namespace CloudTierDemo
{
    public partial class TestStubFileForms : Form
    {
        static Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
        static string AssemblyPath = Path.GetDirectoryName(assembly.Location);

        public static string cacheFolder = AssemblyPath +"\\TestSourceFolder";
        public static string stubFilesFolder = AssemblyPath + "\\TestStubFolder";
        static int totalStubFile = 0;

        const uint FILE_ATTRIBUTE_RECALL_ON_DATA_ACCESS = 0x00400000; 

        public TestStubFileForms()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            cacheFolder = textBox_SourceFolder.Text;
            stubFilesFolder = textBox_StubFolder.Text;

            CreateTestStubFiles(cacheFolder);

            MessageBox.Show(totalStubFile + " stub files were created, Please start the filter service and test the stub file in folder " + stubFilesFolder, "StubFile", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        static public void CreateTestFiles()
        {
            if (!Directory.Exists(cacheFolder))
            {
                Directory.CreateDirectory(cacheFolder);
            }        

            CreateTestSourceFiles();
            CreateTestStubFiles(cacheFolder);
        }

        /// <summary>
        /// Here will create some test source file with file content, it is only for demo purpose.
        /// for your own application, you need to get the file content from the remote host.
        /// </summary>
        static public void CreateTestSourceFiles()
        {
            //create 5 test source file here.                    
            for (int i = 1; i < 6; i++)
            {
                string testStr = string.Empty;
                for (int j = 0; j < i*10240; j++)
                {
                    int rem = 0;
                    Math.DivRem(j, 26, out rem);
                    testStr += (char)('a' + rem);
                    if (rem == 25)
                    {
                        testStr += Environment.NewLine;
                    }
                }

                string testFileName = Path.Combine(cacheFolder, "testFile." + i.ToString() + ".txt");
                File.AppendAllText(testFileName, testStr);
            }
        }

        /// <summary>
        /// Here create the test stub file based on the test source folder for demo purpose.
        /// It will traverse the test source folder with recursion, it will create a stub file in the test stub folder for every source file.
        /// the stub file won't take the physical storage space, it has the same file size of the source file, the soure file's file name
        /// was embedded into the stub file's reparse point tag data.
        /// For your own application, you can put your own custom data to the reparse point tag.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
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
                    string stubFileName = stubFilesFolder + file.Substring(cacheFolder.Length);

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

                    //Here we put the source file's name to the reparse point tag of the stub file.
                    //you will get this reparse point tag data in the filter callback function when the stub file was accessed.

                    byte[] tagData = ASCIIEncoding.Unicode.GetBytes(file);
                    GCHandle gcHandle = GCHandle.Alloc(tagData, GCHandleType.Pinned);
                    IntPtr fileHandle = IntPtr.Zero;

                    try
                    {
                        //FILE_ATTRIBUTE_RECALL_ON_DATA_ACCESS is the attribute to tell the antivirus software to skip the scanning.//it requires the driver was loaded.

                        uint fileAttribute = (uint)FileAttributes.Offline; //FILE_ATTRIBUTE_RECALL_ON_DATA_ACCESS;
                        
                        ret = FilterAPI.CreateStubFileEx(stubFileName, fileInfo.Length, fileAttribute,
                            (uint)tagData.Length, Marshal.UnsafeAddrOfPinnedArrayElement(tagData, 0), 0, 0, 0, true, ref fileHandle);
                        if (!ret)
                        {
                           EventManager.WriteMessage(100, "createTestStubFile", EventLevel.Error, "Create stub file:" + stubFileName + " failed.\n" + FilterAPI.GetLastErrorMessage());
                           continue;
                        }

                        totalStubFile++;
                    }
                    catch (Exception ex)
                    {
                        EventManager.WriteMessage(150, "createTestStubFile", EventLevel.Error, "Create stub file:" + stubFileName + " failed.\n" + ex.Message);
                    }
                    finally
                    {
                        gcHandle.Free();

                        if (fileHandle != IntPtr.Zero)
                        {
                            FilterAPI.CloseHandle(fileHandle);
                        }
                    }                  

                }

                return ret;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Create test stub file got exception:" + ex.Message, "StubFile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        /// <summary>
        /// Here will create a stub file with reparseFileName embedded, when the stub file is accessed,
        /// the filter driver will automatically reparse the file open to the "reparseFileName", it won't call the callback function.
        /// the reparseFileName must be accessed by local computer.
        /// </summary>
        /// <param name="stubFileName"></param>
        /// <param name="reparseFileName"></param>
        private void CreateReparseStubFile(string stubFileName, string reparseFileName)
        {
            FileInfo fileInfo = new FileInfo(reparseFileName);
            long fileSize = fileInfo.Length;
            uint fileAttributes = (uint)fileInfo.Attributes;

            byte[] fileNameBuffer = ASCIIEncoding.Unicode.GetBytes(reparseFileName);

            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(FilterAPI.REPARSETAG_KEY);
            bw.Write((uint)0);
            bw.Write((uint)fileNameBuffer.Length);
            bw.Write(fileNameBuffer);

            byte[] tagData = ms.ToArray();

            IntPtr fileHandle = IntPtr.Zero;
            GCHandle gcHandle = GCHandle.Alloc(tagData, GCHandleType.Pinned);

            try
            {
                bool ret = FilterAPI.CreateStubFile(stubFileName, fileSize, fileAttributes, (uint)tagData.Length, Marshal.UnsafeAddrOfPinnedArrayElement(tagData, 0), true, ref fileHandle);
                if (!ret)
                {
                    MessageBox.Show("Create stub file:" + stubFileName + " failed.\n" + FilterAPI.GetLastErrorMessage(), "CreateStubFile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
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

            MessageBox.Show("Create stub file:" + stubFileName + " succeeded.\n", "CreateStubFile", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
