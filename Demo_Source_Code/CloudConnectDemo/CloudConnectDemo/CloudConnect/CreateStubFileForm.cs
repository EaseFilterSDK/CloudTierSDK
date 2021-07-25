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
    public partial class CreateStubFileForm : Form
    {
        public CreateStubFileForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void button_CreateStubFile_Click(object sender, EventArgs e)
        {
            string fileName = textBox_FileName.Text;
            string reparseToNewFileName = textBox_TagData.Text;          

            FileInfo fileInfo = new FileInfo(reparseToNewFileName);
            long fileSize = fileInfo.Length;
            uint fileAttributes = (uint)fileInfo.Attributes;

            byte[] fileNameBuffer =  ASCIIEncoding.Unicode.GetBytes(reparseToNewFileName);

            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(FilterAPI.EASETAG_KEY);
            bw.Write((uint)0);
            bw.Write((uint)fileNameBuffer.Length);
            bw.Write(fileNameBuffer);

            byte[] tagData = ms.ToArray();

            IntPtr fileHandle = IntPtr.Zero;
            GCHandle gcHandle = GCHandle.Alloc(tagData, GCHandleType.Pinned);

            try
            {
                bool ret = FilterAPI.CreateStubFile(fileName, fileSize, fileAttributes, (uint)tagData.Length, Marshal.UnsafeAddrOfPinnedArrayElement(tagData, 0), true, ref fileHandle);
                if (!ret)
                {
                    MessageBox.Show("Create stub file:" + fileName + " failed.\n" + FilterAPI.GetLastErrorMessage(), "CreateStubFile", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            MessageBox.Show("Create stub file:" + fileName + " succeeded.\n", "CreateStubFile", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}

