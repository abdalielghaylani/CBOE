using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// This class defines the configuration data for the TableEditor's each Element that has been added inside each application
    /// properties can be added as needed. Each propery will be added to the add element in the TableEditor parent element
    /// <code lang="Xml">
   
    ///     &lt;LinkPermission&gt;
    ///         &lt;add name="Registration"&gt;
    ///             &lt;Links&gt;
    ///                     &lt;add name"ID" privilege="" URL="xx.com" display="linktext"&gt linkIcon="icon_32.png"&gt linkIconCSS="icon_32"&gt linkIconBckCSS="blue_32"&gt;
    ///             &lt;/Links&gt;
    ///         &lt;/add&gt;
    ///     &lt;/LinkPermission&gt;
    /// </code>
    /// </summary>
    /// 
     [Serializable]
    public class LinkData : COENamedConfigurationElement
    {
        private const string _name = "name";
        private const string _privilegeRequired = "privilege";
        private const string _url = "url";
        private const string _displayText = "display";
        private const string _toolTip = "tip";
        private const string _linkIconSize = "linkIconSize";
        private const string _linkIconBasePath = "linkIconBasePath";
        private const string _linkIconFileName = "linkIconFileName";
        private const string _newWindow = "newWindow";
     
        /// <summary>
        /// Initialize a new instance of the <see cref="TableEditorData"/> class.
        /// </summary>
        public LinkData()
        {
        }

        /// <summary>
        /// Name that uniquely identifies the configuration entry)
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("name that uniquely identifies the configuration entry")]
        [Category("Links")]
        [DisplayName("Name")]
        [ConfigurationProperty(_name, IsRequired = true)]
        public string Name
        {
            get { return (string)base[_name]; }
            set { base[_name] = value; }
        }

        /// <summary>
        /// privilege required to show a link
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("privilege required to show a link")]
        [Category("Links")]
        [DisplayName("Privilege")]
        [ConfigurationProperty(_privilegeRequired, IsRequired = false)]
        public string PrivilegeRequired
        {
            get { return (string)base[_privilegeRequired]; }
            set { base[_privilegeRequired] = value; }
        }

        /// <summary>
        /// URL for the link)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("url for the link")]
        [Category("Links")]
        [DisplayName("url")]
        [ConfigurationProperty(_url, IsRequired = false)]
        public string URL
        {
            get { return (string)base[_url]; }
            set { base[_url] = value; }
        }

        /// <summary>
        /// text to display for link)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("text to display for link")]
        [Category("Links")]
        [DisplayName("display text")]
        [ConfigurationProperty(_displayText, IsRequired = true)]
        public string DisplayText
        {
            get { return (string)base[_displayText]; }
            set { base[_displayText] = value; }
        }

        /// <summary>
        /// Tool tip for the link
        /// </summary>
        
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Tool tip for the link")]
        [Category("Links")]
        [DisplayName("tool tip")]
        [ConfigurationProperty(_toolTip, IsRequired = false)]
        public string ToolTip
        {
            get { return (string)base[_toolTip]; }
            set { base[_toolTip] = value; }
        }


        /// icon for link
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("icon size")]
        [Category("Links")]
        [DisplayName("Icon Size to use")]
        [ConfigurationProperty(_linkIconSize, IsRequired = false)]
        public Group.IconSize LinkIconSize
        {
            get { return (Group.IconSize)base[_linkIconSize]; }
            set { base[_linkIconSize] = value; }
        }

        /// base path to icon file for group
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("base file path to icon file")]
        [Category("Links")]
        [DisplayName("base file path to icon file")]
        [ConfigurationProperty(_linkIconBasePath, IsRequired = false)]
        public string LinkIconBasePath
        {
            get { return (string)base[_linkIconBasePath]; }
            set { base[_linkIconBasePath] = value; }
        }


        /// base path to icon file for group
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("icon file name")]
        [Category("Links")]
        [DisplayName("icon file name")]
        [ConfigurationProperty(_linkIconFileName, IsRequired = false)]
        public string LinkIconFileName
        {
            get { return (string)base[_linkIconFileName]; }
            set { base[_linkIconFileName] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("specify whether to launch group links in a new window (true|false|javascript window options)")]
        [Category("Links")]
        [DisplayName("New Window")]
        [ConfigurationProperty(_newWindow, IsRequired = false)]
        public string NewWindow
        {
            get { return (string)base[_newWindow]; }
            set { base[_newWindow] = value; }
        }


}
}
