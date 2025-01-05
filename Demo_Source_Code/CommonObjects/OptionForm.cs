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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.IO;

using CloudTier.FilterControl;

namespace CloudTier.CommonObjects
{
    public partial class OptionForm : Form
    {
        OptionType optionType = OptionType.EventNotification;
        string value = string.Empty;

        uint fileAttributes = 0;
        uint eventNotification = 0;
        string processId = "0";

        public enum OptionType
        {
            EventNotification = 0,
            FileAttribute,
            ProccessId,
        }

        public OptionForm(OptionType formType,string defaultValue)
        {
            this.optionType = formType;
            this.value = defaultValue;

            InitializeComponent();
            InitForm();
        }

        public uint EventNotification
        {
            get { return eventNotification; }
        }

        public uint FileAttributes
        {
            get { return fileAttributes; }
        }

        public string ProcessId
        {
            get { return processId; }
        }

        

        void InitForm()
        {
            this.Text = optionType.ToString();

            switch (optionType)
            {
                case OptionType.FileAttribute:
                    {
                        listView1.Clear();		//clear control
                        //create column header for ListView
                        listView1.Columns.Add("Select FileAttributes", 400, System.Windows.Forms.HorizontalAlignment.Left);

                        fileAttributes = uint.Parse(value);

                        foreach (FileAttributes attribute in Enum.GetValues(typeof(FileAttributes)))
                        {
                            string item = attribute.ToString();
                            ListViewItem lvItem = new ListViewItem(item, 0);
                            lvItem.Tag = (uint)attribute;

                            if (((uint)attribute & fileAttributes) > 0)
                            {
                                lvItem.Checked = true;
                            }

                            listView1.Items.Add(lvItem);
                        }

                        break;
                    }

                case OptionType.ProccessId:
                    {
                        Process[] processlist = Process.GetProcesses();

                        listView1.Clear();		//clear control
                        //create column header for ListView
                        listView1.Columns.Add("Process Id", 100, System.Windows.Forms.HorizontalAlignment.Left);
                        listView1.Columns.Add("Process Name", 300, System.Windows.Forms.HorizontalAlignment.Left);

                        List<uint> pidList = new List<uint>();

                        string[] pids = value.Split(';');
                        foreach (string pid in pids)
                        {
                            if (!string.IsNullOrEmpty(pid))
                            {
                                pidList.Add(uint.Parse(pid));
                            }
                        }


                        for (int i = 0; i < processlist.Length; i++)
                        {
                            string[] item = new string[2];
                            item[0] = processlist[i].Id.ToString();
                            item[1] = processlist[i].ProcessName;

                            if (processlist[i].Id == 0)
                            {
                                //this is idle process, skip it.
                                continue;
                            }

                            ListViewItem lvItem = new ListViewItem(item, 0);

                            lvItem.Tag = processlist[i].Id;

                            if (pidList.Contains((uint)(processlist[i].Id)))
                            {
                                lvItem.Checked = true;
                            }

                            if (i > 0)
                            {
                                for (int k = 0; k < i; k++)
                                {
                                    if ((int)listView1.Items[k].Tag > processlist[i].Id)
                                    {
                                        listView1.Items.Insert(k, lvItem);
                                        break;
                                    }
                                }

                                if (listView1.Items.Count == i)
                                {
                                    listView1.Items.Insert(i, lvItem);
                                }

                            }
                            else
                            {
                                listView1.Items.Insert(i, lvItem);
                            }

                        }

                        break;
                    }


            }
        }

        private void button_Ok_Click(object sender, EventArgs e)
        {
            eventNotification = 0;
            fileAttributes = 0;

            foreach (ListViewItem item in listView1.CheckedItems)
            {
                switch (optionType)
                {
                    case OptionType.EventNotification:
                        eventNotification |= (uint)item.Tag;
                        break;

                    case OptionType.FileAttribute:
                        fileAttributes |= (uint)item.Tag;
                        break;

                    case OptionType.ProccessId:
                        int pid = (int)item.Tag;
                        processId += pid.ToString() + ";";
                        break;

                }
            }

        }

        private void button_SelectAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                item.Checked = true;
            }
        }

        private void button_ClearAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                item.Checked = false;
            }
        }

        
    }
}
