
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
using System.Configuration;
using System.Text;

namespace EaseFilter.GlobalObjects
{
    public class KeyValueSettings
    {
        public  KeyValueConfigurationCollection settings = new KeyValueConfigurationCollection();

        public KeyValueSettings()
        {
        }

        public  KeyValueSettings(KeyValueConfigurationCollection _settings)
        {
            settings = _settings;
        }


        public  bool Get(string name, bool value)
        {
            try
            {
                return bool.Parse(settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public  byte Get(string name, byte value)
        {
            try
            {
                return byte.Parse(settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public  sbyte Get(string name, sbyte value)
        {
            try
            {
                return sbyte.Parse(settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public  char Get(string name, char value)
        {
            try
            {
                return char.Parse(settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public  decimal Get(string name, decimal value)
        {
            try
            {
                return decimal.Parse(settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public  double Get(string name, double value)
        {
            try
            {
                return double.Parse(settings[name].Value);
            }
            catch
            {
                return value;
            }
        }

        public  float Get(string name, float value)
        {
            try
            {
                return float.Parse(settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public  int Get(string name, int value)
        {
            try
            {
                return int.Parse(settings[name].Value);
            }
            catch
            {
                return value;
            }
        }

        public  uint Get(string name, uint value)
        {
            try
            {
                return uint.Parse(settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public  long Get(string name, long value)
        {
            try
            {
                return long.Parse(settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public  ulong Get(string name, ulong value)
        {
            try
            {
                return ulong.Parse(settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public  short Get(string name, short value)
        {
            try
            {
                return short.Parse(settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public  ushort Get(string name, ushort value)
        {
            try
            {
                return ushort.Parse(settings[name].Value);
            }
            catch
            {
                return value;
            }
        }

        public  string Get(string name, string value)
        {
            string str = string.Empty;
            try
            {
                str = settings[name].Value;
            }
            catch
            {
                return value;
            }

            if (str == null)
                str = value;

            return str;
        }

        public void Set(string name, string value)
        {
            try
            {
                settings.Remove(name);
                settings.Add(name, value);
            
            }
            catch
            {
            }
        }
    }
}
