using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Configuration;
using System.ComponentModel;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// This class defines the configuration data for the TableEditor for each application
    /// properties can be added as needed. Each propery will be added to the add element in the TableEditor parent element
    /// <code lang="Xml">
    ///     &lt;Group&gt;
    ///         &lt;add name="Registration" displayname="Registration"&gt displayname="Registration"&gt height="100"&gt  width="100"&gt scrollbars="false"&gt icon="32_icon.png"&gt iconCSS="icon_32"&gt iconBckCSS="blue_34"&gt;
    ///         &lt;Links&gt;
    ///         &lt;/Links&gt;
    ///     &lt;/Group&gt;
    /// </code>
    /// </summary>
    /// 
    [Serializable]
    public class Group : COENamedConfigurationElement
    {
        private const string _name = "name";
        private const string _coeIdentifier = "coeIdentifier";
        private const string _displayName = "display";
        private const string _height = "height";
        private const string _scrollbars = "scrollbars";
        private const string _linksData = "links";
        private const string _groupIconSize = "groupIconSize";
        private const string _newWindow = "newWindow";
        private const string _color = "color";
        private const string _helpText = "helpText";
        private const string _pageSectionTarget = "pageSectionTarget";
        private const string _customItems = "customItems";
        private const string _enabled = "enabled";

        /// <summary>
        /// Initialize a new instance of the <see cref="LinkPermissions"/> class.
        /// </summary>
        public Group()
        {
        }

        /// <summary>
        /// Target Area of page to display group links. Current there are two, panel and dashboard. Only panel is supported on the main home page
        /// Application home pages have both
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("page area to display links")]
        [Category("Items")]
        [DisplayName("PageSectionTarget")]
        [ConfigurationProperty(_pageSectionTarget, IsRequired = false)]
        public string PageSectionTarget
        {
            get { return (string)base[_pageSectionTarget]; }
            set { base[_pageSectionTarget] = value; }
        }



        /// <summary>
        /// Name of the Table used by application
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("application identifier")]
        [Category("Items")]
        [DisplayName("COEIdentifier")]
        [ConfigurationProperty(_coeIdentifier, IsRequired = true)]
        public string COEIdentifier
        {
            get { return (string)base[_coeIdentifier]; }
            set { base[_coeIdentifier] = value; }
        }

        /// <summary>
        /// unique GroupName
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("application unique groupName")]
        [Category("Items")]
        [DisplayName("Name")]
        [ConfigurationProperty(_name, IsRequired = true)]
        public string Name
        {
            get { return (string)base[_name]; }
            set { base[_name] = value; }
        }

        /// <summary>
        /// Name to display
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("name to display in header")]
        [Category("Items")]
        [DisplayName("Display Name")]
        [ConfigurationProperty(_displayName, IsRequired = true)]
        public string DisplayName
        {
            get { return (string)base[_displayName]; }
            set { base[_displayName] = value; }
        }

        /// <summary>
        /// Color
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("color of header and help")]
        [Category("Items")]
        [DisplayName("Color")]
        [ConfigurationProperty(_color, IsRequired = true)]
        public string Color
        {
            get { return (string)base[_color]; }
            set { base[_color] = value; }
        }

        /// <summary>
        /// Color
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("help text")]
        [Category("Items")]
        [DisplayName("helpText")]
        [ConfigurationProperty(_helpText, IsRequired = true)]
        public string HelpText
        {
            get { return (string)base[_helpText]; }
            set { base[_helpText] = value; }
        }
       

        /// <summary>
        /// icon for group
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("size of icon")]
        [Category("Groups")]
        [DisplayName("Group Icon Size")]
        [ConfigurationProperty(_groupIconSize, IsRequired = false)]
        public  IconSize GroupIconSize
        {
            get { return (IconSize)base[_groupIconSize]; }
            set { base[_groupIconSize] = value; }
        }


        


        

        /// <summary>
        /// is group enabled
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("specify whether a group is enabled")]
        [Category("Groups")]
        [DisplayName("Enabled")]
        [ConfigurationProperty(_enabled, IsRequired = false)]
        public string Enabled
        {
            get { return (string)base[_enabled]; }
            set { base[_enabled] = value; }
        }

        /// <summary>
        /// user scrollbars if height is fixed
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("specify whether to launch group links in a new window (true|false|javascript window options)")]
        [Category("Groups")]
        [DisplayName("New Window")]
        [ConfigurationProperty(_newWindow, IsRequired = false)]
        public string NewWindow
        {
            get { return (string)base[_newWindow]; }
            set { base[_newWindow] = value; }
        }
   
        /// <summary>
        /// Gets the collection of defined <see cref="LinksData"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="LinksData"/> objects.
        /// </value>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("links to show in box")]
        [Category("Items")]
        [DisplayName("Links")]
        [ConfigurationProperty(_linksData, IsRequired = false)]
        public COENamedElementCollection<LinkData> LinksData
        {
            get { return (COENamedElementCollection<LinkData>)base[_linksData]; }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="LinksData"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="LinksData"/> objects.
        /// </value>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("dashboard items to show in dashboard")]
        [Category("Items")]
        [DisplayName("Dashboard")]
        [ConfigurationProperty(_customItems, IsRequired = false)]
        public COENamedElementCollection<CustomItems> CustomItems
        {
            get { return (COENamedElementCollection<CustomItems>)base[_customItems]; }
        }
    

    public enum IconSize{
        none,
        xsmall,
        small,
        medium,
        large,
        xlarge,
        xxlarge
    }


   

}
}

