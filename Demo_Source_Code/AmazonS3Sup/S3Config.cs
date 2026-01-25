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
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Collections.Generic;


namespace CloudTier.AmazonS3Sup
{
    public class S3Config
    {
        static Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
        static string assemblyPath = Path.GetDirectoryName(assembly.Location);

        static private string configPath = string.Empty;
        static private System.Configuration.Configuration config = null;

        static private AmazonS3Section amazonS3Section = new AmazonS3Section();

        //the registry filter rule collection
        static private Dictionary<string, AmazonS3SiteInfo> amazonS3SiteInfos = new Dictionary<string, AmazonS3SiteInfo>();

        static S3Config()
        {
            try
            {
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;

                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = Path.Combine(assemblyPath, "Amazon.S3.Config");
                config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

                amazonS3Section = (AmazonS3Section)config.Sections["AmazonS3Section"];

                if (amazonS3Section == null)
                {
                    amazonS3Section = new AmazonS3Section();
                    config.Sections.Add("AmazonS3Section", amazonS3Section);
                }

                amazonS3SiteInfos = GetAmazonS3SiteInfo();
            }
            catch (Exception ex)
            {
                throw new Exception("Load Amazon s3 site info got exception." + ex.Message);
            }
        }

        public static bool SaveConfigSetting()
        {
            bool ret = true;

            try
            {
                config.Save(ConfigurationSaveMode.Full);
            }
            catch (Exception ex)
            {
                throw new Exception("Save config file got exception." + ex.Message);
            }

            return ret;
        }

        public static string GetFilePath()
        {
            return configPath;
        }

        public static Dictionary<string, AmazonS3SiteInfo> GetAmazonS3SiteInfo()
        {
            Dictionary<string, AmazonS3SiteInfo> s3SiteInfos = new Dictionary<string, AmazonS3SiteInfo>();

            foreach (AmazonS3SiteInfo s3SiteInfo in amazonS3Section.Instances)
            {
                s3SiteInfos.Add(s3SiteInfo.SiteName, s3SiteInfo);
            }

            return s3SiteInfos;
        }

        public static void AddS3SiteInfo(AmazonS3SiteInfo s3SiteInfo)
        {
            amazonS3Section.Instances.Add(s3SiteInfo);
            return;
        }

        public static void RemoveS3SiteInfo(AmazonS3SiteInfo s3SiteInfo)
        {
            amazonS3Section.Instances.Remove(s3SiteInfo.SiteName);

            return;
        }

        public static bool AddAmazonS3SiteInfo(AmazonS3SiteInfo amazonS3SiteInfo)
        {
            if (amazonS3SiteInfos.ContainsKey(amazonS3SiteInfo.SiteName))
            {
                amazonS3SiteInfos.Remove(amazonS3SiteInfo.SiteName);
                RemoveS3SiteInfo(amazonS3SiteInfo);
            }

            amazonS3SiteInfos.Add(amazonS3SiteInfo.SiteName, amazonS3SiteInfo);
            AddS3SiteInfo(amazonS3SiteInfo);

            return true;
        }

        public static void RemoveAmazonS3SiteInfo(AmazonS3SiteInfo amazonS3SiteInfo)
        {
            if (amazonS3SiteInfos.ContainsKey(amazonS3SiteInfo.SiteName))
            {
                amazonS3SiteInfos.Remove(amazonS3SiteInfo.SiteName);
                RemoveS3SiteInfo(amazonS3SiteInfo);
            }
        }

        public static AmazonS3SiteInfo GetAmazonS3SiteInfo(string siteName)
        {
            if (amazonS3SiteInfos.ContainsKey(siteName))
            {
                return amazonS3SiteInfos[siteName];
            }

            return null;
        }

        public static Dictionary<string, AmazonS3SiteInfo> AmazonS3SiteInfos
        {
            get { return amazonS3SiteInfos; }
        }

    }
}
