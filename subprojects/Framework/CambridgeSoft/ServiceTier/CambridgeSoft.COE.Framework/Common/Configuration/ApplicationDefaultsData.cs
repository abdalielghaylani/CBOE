using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// This class defines the default values for all applications. This values can be overriden for each application
    /// <code lang="Xml">
    /// &lt;applicationDefaults&gt;   
    ///   &lt;searchService&gt;
    ///     /*some configurations for search service*/
    ///   &lt;/searchService&gt;
    ///   &lt;chemDrawEmbed&gt;
    ///     /*some configurations for chem draw embed*/
    ///   &lt;/chemDrawEmbed&gt;
    /// &lt;/applicationDefaults&gt;
    /// </code>
    /// </summary>
    public class ApplicationDefaultsData : COEConfigurationElement
    {

        private const string searchServiceProperty = "searchService";
        private const string chemDrawEmbedProperty = "chemDrawEmbed";
        private const string cachingProperty = "caching";
        private const string displayCulture = "displayCulture";
        private const string fipsEnabled = "fipsEnabled";
        private const string dateFormat = "dateFormat";
        private const string safeExportSize = "safeExportSize";

        public ApplicationDefaultsData()
        {
        }

        [ConfigurationProperty(searchServiceProperty, IsRequired = true)]
        public SearchServiceData SearchServiceData
        {
            get { return (SearchServiceData)base[searchServiceProperty]; }
        }

        [ConfigurationProperty(chemDrawEmbedProperty, IsRequired = true)]
        public ChemDrawEmbedData ChemDrawEmbedData
        {
            get { return (ChemDrawEmbedData)base[chemDrawEmbedProperty]; }
        }

        /// <summary>
        /// Caching configurations for the various items that are cachable
        /// </summary>
        [ConfigurationProperty(cachingProperty, IsRequired = false)]
        public CachingData CachingData
        {
            get { return (CachingData)base[cachingProperty]; }
        }

        /// <summary>
        /// Default display culture for all applications
        /// </summary>
        [ConfigurationProperty(displayCulture, IsRequired = false, DefaultValue = "")]
        public string DisplayCulture
        {
            get { return (string)base[displayCulture]; }
            set { base[displayCulture] = value; }
        }

        /// <summary>
        /// Default fipsEnabled for all applications
        /// </summary>
        [ConfigurationProperty(fipsEnabled, IsRequired = true, DefaultValue = false)]
        public bool FipsEnabled
        {
            get { return (bool)base[fipsEnabled]; }
            set { base[fipsEnabled] = value; }
        }

        /// <summary>
        /// Default date format is given by the DisplayCulture settings, but if that format is not good, date format may be overriden by
        /// setting this property.
        /// For further information on the string syntax, see: http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx
        /// </summary>
        [ConfigurationProperty(dateFormat, IsRequired = false, DefaultValue = "")]
        public string DateFormat
        {
            get { return (string)base[dateFormat]; }
            set { base[dateFormat] = value; }
        }

        /// <summary>
        /// Default hitlist size to export safely
        /// </summary>
        [ConfigurationProperty(safeExportSize, IsRequired = false, DefaultValue = 100)]
        public int SafeExportSize
        {
            get { return (int)base[safeExportSize]; }
            set { base[safeExportSize] = value; }
        }
    }
}
