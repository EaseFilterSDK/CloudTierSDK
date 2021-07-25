
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
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace EaseFilter.GlobalObjects
{

    public class ConfigSetting
    {
        static Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
        public static string AssemblyPath = Path.GetDirectoryName(assembly.Location);
        public static string configFileName = assembly.Location + ".config";//  Path.Combine(AssemblyPath, "ConfigInfo.xml");

        //static private string configPath = string.Empty;
        static private System.Configuration.Configuration config = null;
        static private SiteInfoSection siteInfoSection = new SiteInfoSection();
        static private KeyValueSettings appSettings = new KeyValueSettings();
       
        static ConfigSetting()
        {
            LoadConfigSetting();
        }

        public static void LoadConfigSetting()
        {
            try
            {

                ConfigurationManager.RefreshSection("appSettings");
                ConfigurationManager.RefreshSection("FilterRuleSection");
                ConfigurationManager.RefreshSection("SiteInfoSection");

                ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
                configFileMap.ExeConfigFilename = configFileName;
                config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

                KeyValueConfigurationCollection settings = config.AppSettings.Settings;
                appSettings = new KeyValueSettings(settings);

                siteInfoSection = (SiteInfoSection)config.Sections["SiteInfoSection"];

                if (siteInfoSection == null)
                {
                    siteInfoSection = new SiteInfoSection();
                    config.Sections.Add("SiteInfoSection", siteInfoSection);

                }
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(48, "ConfigSetting", EventLevel.Error, "Load config file " + configFileName + " failed with error:" + ex.Message);
            }
        }

        public static KeyValueSettings AppSettings
        {
            get { return appSettings; }
        }

        public static void SetConfigPath(string newConfigPath)
        {

            try
            {
                if (!File.Exists(newConfigPath))
                {
                    EventManager.WriteMessage(45, "SetConfigPath", EventLevel.Error, newConfigPath + " doesn't exist.");
                    return;
                }

                config = ConfigurationManager.OpenExeConfiguration(newConfigPath);
            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(53, "SetConfigPath", EventLevel.Error, "Set new config path:" +  newConfigPath + " failed with error:" + ex.Message);
            }
        }


        public static void Save()
        {
            try
            {
                config.Save(ConfigurationSaveMode.Full);
            }
            catch( Exception ex)
            {
                EventManager.WriteMessage(48, "ConfigSetting", EventLevel.Error, "Save config file " + configFileName + " failed with error:" + ex.Message);
            }

        }


        public static void ClearSiteInfoSection()
        {
            siteInfoSection.SiteInfos.Clear();
        }

        public static Dictionary<string, SiteInfo> GetSiteInfos()
        {

            Dictionary<string, SiteInfo> siteInfos = new Dictionary<string, SiteInfo>();

            try
            {

                foreach (SiteInfoElement siteInfoElement in siteInfoSection.SiteInfos)
                {
                    KeyValueSettings siteInfoSettings = new KeyValueSettings(siteInfoElement.SiteProperties);
                    SiteInfo siteInfo = new SiteInfo(siteInfoSettings);

                    siteInfos.Add(siteInfoElement.SiteName, siteInfo);
                }
            }            
            catch (Exception ex)
            {
                EventManager.WriteMessage(214, "GetSiteInfos", EventLevel.Error, "GetSiteInfos failed with error:" + ex.Message);
            }

            return siteInfos;
        }

        public static void AddSiteInfo(SiteInfo siteInfo)
        {
            try
            {
                SiteInfoElement siteInfoElement = new SiteInfoElement();
                siteInfoElement.SiteName = siteInfo.SiteName;
                siteInfoElement.SiteProperties = siteInfo.siteInfoSettings.settings;

                siteInfoSection.SiteInfos.Remove(siteInfo.SiteName);
                siteInfoSection.SiteInfos.Add(siteInfoElement);

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(234, "AddSiteInfo", EventLevel.Error, "AddSiteInfo failed with error:" + ex.Message);
            }

            return;
        }

        public static void RemoveSiteInfo(string siteName)
        {
            siteInfoSection.SiteInfos.Remove(siteName);

            return;
        }


    
     
    }
}
