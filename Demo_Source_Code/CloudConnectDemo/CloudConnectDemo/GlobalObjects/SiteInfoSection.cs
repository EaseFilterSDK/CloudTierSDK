
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
using System.Linq;
using System.Text;
using System.Configuration;

namespace EaseFilter.GlobalObjects
{
    public class SiteInfoSection : ConfigurationSection
    {
        [ConfigurationProperty("SiteInfos", IsRequired = true)]
        public SiteInfoCollection SiteInfos
        {
            get { return (SiteInfoCollection)this["SiteInfos"]; }
            set { this["SiteInfos"] = value; }
        }
    }

    public class SiteInfoCollection : ConfigurationElementCollection
    {
        public SiteInfoElement this[int index]
        {
            get { return (SiteInfoElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }


        public void Add(SiteInfoElement siteInfoElement)
        {
            BaseAdd(siteInfoElement);
        }

        public void Clear()
        {
            BaseClear();
        }

        public void Remove(SiteInfoElement siteInfoElement)
        {
            BaseRemove(siteInfoElement.SiteName);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SiteInfoElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            //set to whatever Element Property you want to use for a key
            return ((SiteInfoElement)element).SiteName;
        }
    }

    public class SiteInfoElement : ConfigurationElement
    {
        //Make sure to set IsKey=true for property exposed as the GetElementKey above
        [ConfigurationProperty("siteName", IsKey = true, IsRequired = true)]
        public string SiteName
        {
            get { return (string)base["siteName"]; }
            set { base["siteName"] = value; }
        }


        [ConfigurationProperty("properties", IsRequired = true)]
        public KeyValueConfigurationCollection SiteProperties
        {
            get { return (KeyValueConfigurationCollection)this["properties"]; }
            set { this["properties"] = value; }
        }
    }


}


