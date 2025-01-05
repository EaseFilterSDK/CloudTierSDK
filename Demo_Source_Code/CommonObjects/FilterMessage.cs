///////////////////////////////////////////////////////////////////////////////
//
//    (C) Copyright 2011 EaseFilter
//    All Rights Reserved
//
//    This software is part of a licensed software product and may
//    only be used or copied in accordance with the terms of that license.
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;

using CloudTier.FilterControl;

namespace CloudTier.CommonObjects
{


    public class FilterMessage : IDisposable
    {

        ListView listView_Message = null;
        bool disposed = false;
        bool isConsoleApp = false;
        
        public FilterMessage(ListView lvMessage, bool _isConsoleApp)
        {
            this.isConsoleApp = _isConsoleApp;

            this.listView_Message = lvMessage;
            InitListView();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
            }

            disposed = true;
        }

        ~FilterMessage()
        {
            Dispose(false);
        }

        public void InitListView()
        {
            if (null != listView_Message)
            {
                //init ListView control
                listView_Message.Clear();		//clear control
                //create column header for ListView
                listView_Message.Columns.Add("#", 40, System.Windows.Forms.HorizontalAlignment.Left);
                listView_Message.Columns.Add("Time", 50, System.Windows.Forms.HorizontalAlignment.Left);
                listView_Message.Columns.Add("UserName", 150, System.Windows.Forms.HorizontalAlignment.Left);
                listView_Message.Columns.Add("ProcessName(PID)", 120, System.Windows.Forms.HorizontalAlignment.Left);
                listView_Message.Columns.Add("ThreadId", 70, System.Windows.Forms.HorizontalAlignment.Left);
                listView_Message.Columns.Add("MessageType", 160, System.Windows.Forms.HorizontalAlignment.Left);
                listView_Message.Columns.Add("FileName", 200, System.Windows.Forms.HorizontalAlignment.Left);
                listView_Message.Columns.Add("FileSize", 50, System.Windows.Forms.HorizontalAlignment.Left);
                listView_Message.Columns.Add("FileAttributes", 100, System.Windows.Forms.HorizontalAlignment.Left);
                listView_Message.Columns.Add("LastWriteTime", 60, System.Windows.Forms.HorizontalAlignment.Left);
                listView_Message.Columns.Add("Offset", 50, System.Windows.Forms.HorizontalAlignment.Left);
                listView_Message.Columns.Add("Length", 50, System.Windows.Forms.HorizontalAlignment.Left);
                listView_Message.Columns.Add("ReturnStatus", 100, System.Windows.Forms.HorizontalAlignment.Left);
                listView_Message.Columns.Add("ReturnFilterStatus", 150, System.Windows.Forms.HorizontalAlignment.Left);
                listView_Message.Columns.Add("ReturnBufferLength", 100, System.Windows.Forms.HorizontalAlignment.Left);
            }
        }

        public void DisplayMessage(FilterRequestEventArgs e)
        {

            if (isConsoleApp)
            {
                Console.WriteLine(Environment.NewLine + "Id#" + e.MessageId);
                Console.WriteLine("UserName:" + e.UserName);
                Console.WriteLine("ProcessId:" + e.ProcessId);
                Console.WriteLine("ProcessName(PID):" + e.ProcessName);
                Console.WriteLine("MessageType:" + e.MessageType);
                Console.WriteLine("FileName:" + e.FileName);
                Console.WriteLine("FileSize:" + e.FileSize);
                Console.WriteLine("FileAttributes:" + e.Attributes.ToString());
                Console.WriteLine("Offset:" + e.ReadOffset);
                Console.WriteLine("Length:" + e.ReadLength);
                Console.WriteLine("ReturnStatus:" + e.ReturnStatus.ToString());
                Console.WriteLine("ReturnFilterStatus:" + e.FilterStatus.ToString());
                Console.WriteLine("ReturnBufferLength:" + e.ReturnBufferLength.ToString());

            }
            else
            {
                string[] item = new string[listView_Message.Columns.Count];
                item[0] = e.MessageId.ToString();
                item[1] = FormatDateTime(DateTime.Now.ToFileTime());
                item[2] = e.UserName;
                item[3] = e.ProcessName + "(" + e.ProcessId.ToString() + ")";
                item[4] = e.ThreadId.ToString();
                item[5] = e.MessageType.ToString();
                item[6] = e.FileName;
                item[7] = e.FileSize.ToString();
                item[8] = e.Attributes.ToString();
                item[9] = FormatDateTime(e.LastWriteTime);
                item[10] = e.ReadOffset.ToString();
                item[11] = e.ReadLength.ToString();
                item[12] = e.ReturnStatus.ToString();
                item[13] = e.FilterStatus.ToString();
                item[14] = e.ReturnBufferLength.ToString();

                ListViewItem lvItem = new ListViewItem(item, 0);

                listView_Message.Items.Add(lvItem);

                if (listView_Message.Items.Count > 0 && listView_Message.Items.Count > GlobalConfig.MaximumFilterMessages)
                {
                    //the message records in the list view reached to the maximum value, remove the first one till the record less than the maximum value.
                    listView_Message.Items.RemoveAt(0);
                }

                listView_Message.EnsureVisible(listView_Message.Items.Count - 1);
            }

        }


        string FormatDateTime(long lDateTime)
        {
            try
            {
                if (0 == lDateTime)
                {
                    return "0";
                }

                DateTime dateTime = DateTime.FromFileTime(lDateTime);
                string ret = dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString();
                return ret;
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(502, "FormatDateTime", EventLevel.Error, "FormatDateTime :" + lDateTime.ToString() + " failed." + ex.Message);
                return ex.Message;
            }
        }




    }
}