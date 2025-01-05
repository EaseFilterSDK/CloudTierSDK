using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;

namespace CloudTier.CommonObjects
{

    public class ConfigSetting
    {
        static private string configPath = string.Empty;
        static private System.Configuration.Configuration config = null;

        static ConfigSetting()
        {
            try
            {
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;

            }
            catch
            {
            }
        }

        public static void Save()
        {
            config.Save(ConfigurationSaveMode.Full);
        }


        public static string GetFilePath()
        {
            return configPath;
        }

        public static bool Get(string name, bool value)
        {
            try
            {
                return bool.Parse(config.AppSettings.Settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public static byte Get(string name, byte value)
        {
            try
            {
                return byte.Parse(config.AppSettings.Settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public static sbyte Get(string name, sbyte value)
        {
            try
            {
                return sbyte.Parse(config.AppSettings.Settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public static char Get(string name, char value)
        {
            try
            {
                return char.Parse(config.AppSettings.Settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public static decimal Get(string name, decimal value)
        {
            try
            {
                return decimal.Parse(config.AppSettings.Settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public static double Get(string name, double value)
        {
            try
            {
                return double.Parse(config.AppSettings.Settings[name].Value);
            }
            catch
            {
                return value;
            }
        }

        public static float Get(string name, float value)
        {
            try
            {
                return float.Parse(config.AppSettings.Settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public static int Get(string name, int value)
        {
            try
            {
                return int.Parse(config.AppSettings.Settings[name].Value);
            }
            catch
            {
                return value;
            }
        }

        public static uint Get(string name, uint value)
        {
            try
            {
                return uint.Parse(config.AppSettings.Settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public static long Get(string name, long value)
        {
            try
            {
                return long.Parse(config.AppSettings.Settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public static ulong Get(string name, ulong value)
        {
            try
            {
                return ulong.Parse(config.AppSettings.Settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public static short Get(string name, short value)
        {
            try
            {
                return short.Parse(config.AppSettings.Settings[name].Value);
            }
            catch
            {
                return value;
            }
        }


        public static ushort Get(string name, ushort value)
        {
            try
            {
                return ushort.Parse(config.AppSettings.Settings[name].Value);
            }
            catch
            {
                return value;
            }
        }

        public static string Get(string name, string value)
        {
            string str = string.Empty;
            try
            {
                str = config.AppSettings.Settings[name].Value;
            }
            catch
            {
                return value;
            }

            if (str == null)
                str = value;

            return str;
        }

        public static void Set(string name, string value)
        {
            try
            {
                config.AppSettings.Settings.Remove(name);
                config.AppSettings.Settings.Add(name, value);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

            }
            catch
            {
            }
        }

    }
}
