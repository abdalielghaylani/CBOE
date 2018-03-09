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
    public class CustomItems : COENamedConfigurationElement
    {
        private const string _name = "name";
        private const string _privilegeRequired = "privilege";
        private const string _displayText = "display";
        private const string _assembly = "assembly";
        private const string _className = "className";
        private const string _configuration = "itemConfig";
        

       

        /// <summary>
        /// Initialize a new instance of the <see cref="TableEditorData"/> class.
        /// </summary>
        public CustomItems()
        {
        }

        /// <summary>
        /// Name that uniquely identifies the configuration entry)
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("name that uniquely identifies the configuration entry")]
        [Category("Dash")]
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
        [Description("privilege required to show a dashboard item")]
        [Category("Dash")]
        [DisplayName("Privilege")]
        [ConfigurationProperty(_privilegeRequired, IsRequired = false)]
        public string PrivilegeRequired
        {
            get { return (string)base[_privilegeRequired]; }
            set { base[_privilegeRequired] = value; }
        }

      

        /// <summary>
        /// text to display for link)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("text to display if empty then implementation class should return information")]
        [Category("Dash")]
        [DisplayName("display text")]
        [ConfigurationProperty(_displayText, IsRequired = true)]
        public string DisplayText
        {
            get { return (string)base[_displayText]; }
            set { base[_displayText] = value; }
        }

        


      

        /// assbmely name for custom link output
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("assembly name for custom link output")]
        [Category("Dash")]
        [DisplayName("assembly name")]
        [ConfigurationProperty(_assembly, IsRequired = false)]
        public string AssemblyName
        {
            get { return (string)base[_assembly]; }
            set { base[_assembly] = value; }
        }

        /// class name for custom link output
        /// </summary>
        /// 
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("class name for custom link output")]
        [Category("Dash")]
        [DisplayName("class name")]
        [ConfigurationProperty(_className, IsRequired = false)]
        public string ClassName
        {
            get { return (string)base[_className]; }
            set { base[_className] = value; }
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
        [Description("configuration for a custom item")]
        [Category("Items")]
        [DisplayName("Dashboard")]
        [ConfigurationProperty(_configuration, IsRequired = false)]
        public COENamedElementCollection<CustomItemConfiguration> Configuration
        {
            get { return (COENamedElementCollection<CustomItemConfiguration>)base[_configuration]; }
        }
}
}
