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
    ///             &lt;linkParameter&gt;
    ///                     &lt;add name"ID" privilege="" URL="xx.com" display="linktext"&gt linkIcon="icon_32.png"&gt linkIconCSS="icon_32"&gt linkIconBckCSS="blue_32"&gt;
    ///             &lt;/linkParameter&gt;
    ///         &lt;/add&gt;
    ///     &lt;/LinkPermission&gt;
    /// </code>
    /// </summary>
    /// 
     [Serializable]
    public class InnerXml : COENamedConfigurationElement
    {
        private const string innerXmlNameProperty = "name";
        private const string innerXmlDataProperty = "innerXmlData";

        /// <summary>
        /// Initialize a new instance of the <see cref="LinkParameterData"/> class.
        /// </summary>
        public InnerXml()
        {
        }

        /// <summary>
        /// Name that uniquely identifies the configuration entry)
        /// </summary>
        /// 
        //[Browsable(true)]
        //[ReadOnly(false)]
        //[Description("name that uniquely identifies the configuration entry")]
        //[Category("innerXml")]
        //[DisplayName("Name")]
        [ConfigurationProperty(innerXmlNameProperty, IsRequired = true)]
        public string Name
        {
            get { return (string)base[innerXmlNameProperty]; }
            set { base[innerXmlNameProperty] = value; }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="LinkParameters"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="LinkParameters"/> objects.
        /// </value>
        [ConfigurationProperty(innerXmlDataProperty, IsRequired = false)]
        public COENamedElementCollection<InnerXmlData> InnerXmlData
        {
            get { return (COENamedElementCollection<InnerXmlData>)base[innerXmlDataProperty]; }
        }

}
}
