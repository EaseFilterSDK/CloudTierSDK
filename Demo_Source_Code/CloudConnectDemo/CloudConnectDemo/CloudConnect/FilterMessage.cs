///////////////////////////////////////////////////////////////////////////////
//
//    (C) Copyright 2011 EaseFilter Technologies Inc.
//    All Rights Reserved
//
//    This software is part of a licensed software product and may
//    only be used or copied in accordance with the terms of that license.
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.IO;
using System.Threading;
using System.Reflection;

using EaseFilter.GlobalObjects;

namespace CloudConnect
{


    public class FilterMessage : IDisposable
    {
      
        ListView listView_Message = null;
        Thread messageThread = null;
        Queue<string[]> messageQueue = new Queue<string[]>();
        AutoResetEvent autoEvent = new AutoResetEvent(false);
        bool disposed = false;


        public FilterMessage(ListView lvMessage)
        {
            this.listView_Message = lvMessage;
            InitListView();
            messageThread = new Thread(new ThreadStart(ProcessMessage));
            messageThread.Start();
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

            autoEvent.Set();
            messageThread.Abort();
            disposed = true;
        }

        ~FilterMessage()
        {
            Dispose(false);
        }

        public void InitListView()
        {
            messageQueue.Clear();
            //init ListView control
            listView_Message.Clear();		//clear control
            //create column header for ListView
            listView_Message.Columns.Add("#", 40, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Message.Columns.Add("Time", 120, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Message.Columns.Add("UserName", 150, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Message.Columns.Add("ProcessName(PID)", 100, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Message.Columns.Add("ThreadId", 60, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Message.Columns.Add("MessageType", 160, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Message.Columns.Add("FileName", 350, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Message.Columns.Add("FileSize", 70, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Message.Columns.Add("FileAttributes", 70, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Message.Columns.Add("LastWriteTime", 120, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Message.Columns.Add("Offset", 50, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Message.Columns.Add("Length", 50, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Message.Columns.Add("ReturnStatus", 50, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Message.Columns.Add("ReturnFilterStatus", 50, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Message.Columns.Add("ReturnBufferLength", 50, System.Windows.Forms.HorizontalAlignment.Left);
            listView_Message.Columns.Add("Description", 200, System.Windows.Forms.HorizontalAlignment.Left);
        }

        public void AddMessage(FilterAPI.MessageSendData messageSend, FilterAPI.MessageReplyData messageReply)
        {

            string[] message = DecodeFilterMessage(messageSend, messageReply);
          
            lock (messageQueue)
            {
                if (message != null)
                {
                    messageQueue.Enqueue(message);
                }
            }

            autoEvent.Set();

        }


        void ProcessMessage()
        {
            WaitHandle[] waitHandles = new WaitHandle[] { autoEvent, GlobalConfig.StopEvent };

            while (GlobalConfig.IsRunning)
            {
                try
                {
                    if (messageQueue.Count == 0)
                    {
                        int result = WaitHandle.WaitAny(waitHandles);
                        if (!GlobalConfig.IsRunning)
                        {
                            return;
                        }
                    }

                    lock (messageQueue)
                    {
                        while (messageQueue.Count > 0)
                        {
                            string[] message = (string[])messageQueue.Dequeue();

                            ListViewItem lvItem = new ListViewItem(message, 0);

                            listView_Message.Items.Add(lvItem);

                            if (listView_Message.Items.Count > 0 && listView_Message.Items.Count > GlobalConfig.MaximumFilterMessages)
                            {
                                //the message records in the list view reached to the maximum value, remove the first one till the record less than the maximum value.
                                listView_Message.Items.RemoveAt(0);
                            }

                            listView_Message.EnsureVisible(listView_Message.Items.Count - 1);

                        }
                    }
                }
                catch (Exception ex)
                {
                    EventManager.WriteMessage(165, "ProcessMessage", EventLevel.Error, "ProcessMessage failed with error " + ex.Message);
                }


            }

        }



        string FormatDescription(FilterAPI.MessageSendData messageSend)
        {

            string message = string.Empty;
            try
            {
                if (messageSend.MessageType == (uint)FilterAPI.MessageType.MESSAGE_TYPE_SEND_EVENT_NOTIFICATION)
                {
                    FilterAPI.EVENTTYPE eventType = (FilterAPI.EVENTTYPE)messageSend.InfoClass;
                    string fileName = messageSend.FileName;

                    switch (eventType)
                    {
                        case FilterAPI.EVENTTYPE.CREATEED:
                            {
                                message = "File Created Event: new file " + fileName + " created.";
                                break;
                            }
                        case FilterAPI.EVENTTYPE.CHANGED:
                            {
                                message = "File Modified Event: file " + fileName + " was modified.";
                                break;
                            }
                        case FilterAPI.EVENTTYPE.DELETED:
                            {
                                message = "File Deleted Event: file " + fileName + " was deleted.";
                                break;
                            }
                        case FilterAPI.EVENTTYPE.RENAMED:
                            {
                                string newFileName = string.Empty;

                                if (messageSend.DataBufferLength > 0)
                                {
                                    byte[] buffer = new byte[messageSend.DataBufferLength];
                                    Array.Copy(messageSend.DataBuffer, buffer, buffer.Length);
                                    newFileName = Encoding.Unicode.GetString(buffer);
                                }

                                message = "File Rename Event: file " + fileName + " was renamed to " + newFileName;
                                break;
                            }
                        default: message = "File Event:" + messageSend.InfoClass + " not found, file " + fileName;
                            break;
                    }

                }
                else
                {
                    message += " DesiredAccess:" + FormatDesiredAccess(messageSend.DesiredAccess);
                    message += " Disposition:" + ((WinData.Disposition)messageSend.Disposition).ToString();
                    message += " ShareAccess:" + ((WinData.ShareAccess)messageSend.SharedAccess).ToString();
                    message += " CreateOptions:" + FormatCreateOptions(messageSend.CreateOptions);
                }

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(318, "FormatDescription", EventLevel.Error, "Format description failed with error " + ex.Message);
            }


            return message;
        }

        string FormatNTStatus(uint status)
        {
            string ret = string.Empty;

            foreach (NtStatus.Status ntStatus in Enum.GetValues(typeof(NtStatus.Status)))
            {
                if (status == (uint)ntStatus)
                {
                    ret = ntStatus.ToString() + "(0x" + status.ToString("X") + ")";
                }
            }

            if (string.IsNullOrEmpty(ret))
            {
                ret = "(0x" + status.ToString("X") + ")";
            }

            return ret;
        }

        string FormatFilterStatus(uint status)
        {
            string ret = string.Empty;

            foreach (FilterAPI.FilterStatus filterStatus in Enum.GetValues(typeof(FilterAPI.FilterStatus)))
            {
                if (status == (uint)filterStatus)
                {
                    ret = filterStatus.ToString() + "(0x" + status.ToString("X") + ")";
                }
            }

            if (string.IsNullOrEmpty(ret))
            {
                ret = "(0x" + status.ToString("X") + ")";
            }

            return ret;
        }


        string[] DecodeFilterMessage(FilterAPI.MessageSendData messageSend, FilterAPI.MessageReplyData messageReply)
        {
            try
            {

                string userName = string.Empty;
                string processName = string.Empty;

                FilterAPI.DecodeUserInfo(messageSend, out userName, out processName);

                string[] listData = new string[listView_Message.Columns.Count];
                int col = 0;
                listData[col++] = messageSend.MessageId.ToString();
                listData[col++] = FormatDateTime(messageSend.TransactionTime);
                listData[col++] = userName;
                listData[col++] = processName + "  (" + messageSend.ProcessId + ")";
                listData[col++] = messageSend.ThreadId.ToString();
                listData[col++] = ((FilterAPI.MessageType)messageSend.MessageType).ToString();
                listData[col++] = messageSend.FileName;
                listData[col++] = messageSend.FileSize.ToString();
                listData[col++] = ((FileAttributes)messageSend.FileAttributes).ToString();
                listData[col++] = FormatDateTime(messageSend.LastWriteTime);
                listData[col++] = messageSend.Offset.ToString();
                listData[col++] = messageSend.Length.ToString();
                listData[col++] = FormatNTStatus(messageReply.ReturnStatus);
                listData[col++] = FormatFilterStatus(messageReply.FilterStatus);
                listData[col++] = messageReply.DataBufferLength.ToString();
                listData[col++] = FormatDescription(messageSend);

                return listData;

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(303, "DecodeFilterMessage", EventLevel.Error, "DecodeFilterMessage failed." + ex.Message);
            }

            return null;
        }



        string FormatDesiredAccess(uint desiredAccess)
        {
            string ret = string.Empty;

            foreach (WinData.DisiredAccess access in Enum.GetValues(typeof(WinData.DisiredAccess)))
            {
                if (access == (WinData.DisiredAccess)((uint)access & desiredAccess))
                {
                    ret += access.ToString() + "; ";
                }
            }

            return ret;
        }

        string FormatCreateOptions(uint createOptions)
        {
            string ret = string.Empty;

            foreach (WinData.CreateOptions option in Enum.GetValues(typeof(WinData.CreateOptions)))
            {
                if (option == (WinData.CreateOptions)((uint)option & createOptions))
                {
                    ret += option.ToString() + "; ";
                }
            }

            if (string.IsNullOrEmpty(ret))
            {
                ret = "(0x)" + createOptions.ToString("X");
            }

            return ret;
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