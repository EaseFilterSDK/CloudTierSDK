
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
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace EaseFilter.GlobalObjects
{
    public partial class EventForm : Form
    {
        static DateTime currentLogDateModified = DateTime.MinValue;
        static string logFileName = Path.Combine(GlobalConfig.AssemblyPath, GlobalConfig.LogFileName);
        static EventLevel selectedDisplayEvents = GlobalConfig.SelectedDisplayEvents;

        static EventForm eventForm = new EventForm();
        public delegate void ShowMessageFormDlgt();

        public static void DisplayEventForm()
        {
            Thread messageThread = new Thread(new ThreadStart(ShowEventForm));
            messageThread.Name = "ShowEventFormThread";
            messageThread.Start();
        }

        private static void ShowEventForm()
        {
           // var handle = eventForm.Handle;

            if (eventForm.InvokeRequired)
            {
                eventForm.Invoke(new ShowMessageFormDlgt(ShowEventForm));
            }
            else
            {
                if (!eventForm.Visible)
                {
                    eventForm.ShowDialog();
                }
                else
                {
                    eventForm.Activate();
                }
            }
        }

        public EventForm()
        {
            InitializeComponent();
            ResetEventView();
        }

        public void ResetEventView()
        {
            listView_EventView.Clear();		//clear control
            //create column header for ListView
            //Level Date and Time Source Event ID Message
            listView_EventView.Columns.Add("Id",50, System.Windows.Forms.HorizontalAlignment.Left);
            listView_EventView.Columns.Add("Level", 70, System.Windows.Forms.HorizontalAlignment.Left);
            listView_EventView.Columns.Add("Date and Time", 170, System.Windows.Forms.HorizontalAlignment.Left);
            listView_EventView.Columns.Add("Source", 150, System.Windows.Forms.HorizontalAlignment.Left);
            listView_EventView.Columns.Add("EventId",40, System.Windows.Forms.HorizontalAlignment.Left);
            listView_EventView.Columns.Add("Message", 900, System.Windows.Forms.HorizontalAlignment.Left);

            errorToolStripMenuItem.Checked = (GlobalConfig.SelectedDisplayEvents&EventLevel.Error) > 0 ;
            warningToolStripMenuItem.Checked = (GlobalConfig.SelectedDisplayEvents&EventLevel.Warning) > 0 ;
            informationToolStripMenuItem.Checked = (GlobalConfig.SelectedDisplayEvents&EventLevel.Information) > 0;
            verboseToolStripMenuItem.Checked = (GlobalConfig.SelectedDisplayEvents&EventLevel.Verbose) > 0;
            traceToolStripMenuItem.Checked = (GlobalConfig.SelectedDisplayEvents & EventLevel.Trace) > 0;
            
        }

        public void LoadEventLog()
        {

            try
            {

                FileInfo fileInfo = new FileInfo(logFileName);

                //if ( fileInfo.LastWriteTime == currentLogDateModified)
                //{
                //    //the log file didn't change, we don't need to reload it.
                //    return;
                //}

                ResetEventView();

                if (!File.Exists(logFileName))
                {
                    EventManager.WriteMessage(42, "LoadEventLog", GlobalObjects.EventLevel.Information, "Selected log file name:" + logFileName + " doesn't exist.");
                    return;
                }

                FileStream fs = new FileStream(logFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs);

                List<ListViewItem> items = new List<ListViewItem>();

                string logEntry = string.Empty;
                int i = 0;

                while ((logEntry = sr.ReadLine()) != null && logEntry.Length > 0)
                {

                    try
                    {

                        MessageEventArgs arg = EventManager.ConvertSringToEventArg(logEntry);

                        if (arg == null)
                        {
                            continue;
                        }

                        string[] itemStr = new string[listView_EventView.Columns.Count];

                        int itemNum = 0;

                        itemStr[itemNum++] = i++.ToString();

                        switch (arg.Type)
                        {
                            case EventLevel.Error: if (!errorToolStripMenuItem.Checked) continue; break;
                            case EventLevel.Warning: if (!warningToolStripMenuItem.Checked) continue; break;
                            case EventLevel.Information: if (!informationToolStripMenuItem.Checked) continue; break;
                            case EventLevel.Verbose: if (!verboseToolStripMenuItem.Checked) continue; break;
                            case EventLevel.Trace: if (!traceToolStripMenuItem.Checked) continue; break;
                        }

                        itemStr[itemNum++] = arg.Type.ToString();
                        itemStr[itemNum++] = EventManager.FormatDateTime(arg.Time);
                        itemStr[itemNum++] = arg.CallerName;
                        itemStr[itemNum++] = arg.EventID.ToString();
                        itemStr[itemNum++] = arg.Message;

                        ListViewItem listItem = new ListViewItem(itemStr, 0);

                        if (arg.Type == EventLevel.Error)
                        {
                            // item.BackColor = Color.Red;
                            listItem.ForeColor = Color.Red;
                            // item.Font = new Font(item.Font,FontStyle.Bold);
                        }
                        else if (arg.Type == EventLevel.Warning)
                        {
                            listItem.BackColor = Color.LightGray;
                            listItem.ForeColor = Color.Yellow;
                            // item.Font = new Font(item.Font, FontStyle.Bold);
                        }

                        items.Add(listItem);
                    }
                    catch (Exception ex)
                    {
                        Debugger.Log(0, "EaseCloud", "ConvertSringToEventArg failed with error:" + ex.Message );
                    }
                }

                currentLogDateModified = fileInfo.LastWriteTime;

                if (items.Count > 0)
                {
                    var listItems = new ListViewItem[items.Count];
                    for (int j = 0; j < items.Count; j++)
                    {
                        listItems[j] = (ListViewItem)items[j];
                    }

                    listView_EventView.Items.AddRange(listItems);
                    listView_EventView.EnsureVisible(listView_EventView.Items.Count - 1);
                }

                fs.Close();

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(124, "LoadEventLog", EventLevel.Error, "LoadEventLog failed with error:" + ex.Message);
            }

        }

        private void clearTasksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadEventLog();
        }   

        private void errorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (errorToolStripMenuItem.Checked)
            {
                errorToolStripMenuItem.Checked = false;
                selectedDisplayEvents = selectedDisplayEvents&(~EventLevel.Error);
            }
            else
            {
                errorToolStripMenuItem.Checked = true;
                selectedDisplayEvents = selectedDisplayEvents |EventLevel.Error;
            }

            GlobalConfig.SelectedDisplayEvents = selectedDisplayEvents;

            GlobalConfig.SaveConfigInfo();

            LoadEventLog();
        }

        private void warningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (warningToolStripMenuItem.Checked)
            {
                warningToolStripMenuItem.Checked = false;
                selectedDisplayEvents = selectedDisplayEvents & (~EventLevel.Warning);
            }
            else
            {
                warningToolStripMenuItem.Checked = true;
                selectedDisplayEvents = selectedDisplayEvents | EventLevel.Warning;
            }

            GlobalConfig.SelectedDisplayEvents = selectedDisplayEvents;

            GlobalConfig.SaveConfigInfo();
            LoadEventLog();
        }

        private void informationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (informationToolStripMenuItem.Checked)
            {
                informationToolStripMenuItem.Checked = false;
                selectedDisplayEvents = selectedDisplayEvents & (~EventLevel.Information);
            }
            else
            {
                informationToolStripMenuItem.Checked = true;
                selectedDisplayEvents = selectedDisplayEvents | EventLevel.Information;
            }

            GlobalConfig.SelectedDisplayEvents = selectedDisplayEvents;
            GlobalConfig.SaveConfigInfo();
            LoadEventLog();
        }

        private void verboseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (verboseToolStripMenuItem.Checked)
            {
                verboseToolStripMenuItem.Checked = false;
                selectedDisplayEvents = selectedDisplayEvents & (~EventLevel.Verbose);
            }
            else
            {
                verboseToolStripMenuItem.Checked = true;
                selectedDisplayEvents = selectedDisplayEvents | EventLevel.Verbose;
            }

            GlobalConfig.SelectedDisplayEvents = selectedDisplayEvents;
            GlobalConfig.SaveConfigInfo();
            LoadEventLog();
        }

        private void traceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (traceToolStripMenuItem.Checked)
            {
                traceToolStripMenuItem.Checked = false;
                selectedDisplayEvents = selectedDisplayEvents & (~EventLevel.Trace);
            }
            else
            {
                traceToolStripMenuItem.Checked = true;
                selectedDisplayEvents = selectedDisplayEvents | EventLevel.Trace;
            }

            GlobalConfig.SelectedDisplayEvents = selectedDisplayEvents;
            GlobalConfig.SaveConfigInfo();
            LoadEventLog();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);

            if (MessageBox.Show("Do you really want to delete all the event messages?", "Delete Message", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    File.Delete(logFileName);
                }
                catch { };

                ResetEventView();

            }
        }

        private void EventForm_Activated(object sender, EventArgs e)
        {
           // LoadEventLog();
        }

        private void listView_EventView_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem item = listView_EventView.SelectedItems[0];
            string message = (string)item.SubItems[listView_EventView.Columns.Count -1 ].Text;

            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);MessageBox.Show(message, "Event Message");
        }

        private void EventForm_Load(object sender, EventArgs e)
        {
            LoadEventLog();
        }
      
    }
}
