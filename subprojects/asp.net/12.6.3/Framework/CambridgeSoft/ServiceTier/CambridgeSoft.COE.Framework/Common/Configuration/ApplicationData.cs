using System;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Configuration data defining an Applicaton using a core service. 
    /// </summary> 
    [Serializable]
    public class ApplicationData : COENamedConfigurationElement
    {
        private const string nameProperty = "name";
        private const string databaseProperty = "database";
        private const string tableEditorProperty = "tableEditor";
        private const string reportingProperty = "reporting";
        private const string linkParametersProperty = "linkParameters";

        //private const string linkABCsProperty = "linkABCs";
        private const string innerXmlProperty = "innerXml";

        private const string gridConfigSettingsProperty = "GridConfigSettings";
        private const string saveQueryHistory = "saveQueryHistory";
        private const string returnPartialHitlist = "returnPartialHitlist";
        private const string partialHitlistCommitSize = "partialHitlistCommitSize";
        private const string partialHitlistFirstCommitSize = "partialHitlistFirstCommitSize";
        private const string useRealTableNames = "useRealTableNames";
        private const string maxRecordCount = "maxRecordCount";
        private const string showPluginDownloadProperty = "showPluginDownload";
        private const string pluginDownloadURLProperty = "pluginDownloadURL";
        private const string downloadChemDrawImageSrcProperty = "downloadChemDrawImageSrc";
        private const string displayCulture = "displayCulture";
        private const string dateFormat = "dateFormat";
        private const string safeExportSize = "safeExportSize";
        private const string applicationHome = "applicationHome";
        private const string cachingProperty = "caching";
        private const string parentApplicationProperty = "parentApplication";
        private const string appSettingsProperty = "applicationSettings";

        /// <summary>
        /// Initialize a new instance of the <see cref="ApplicationData"/> class.
        /// </summary>
        public ApplicationData()
        {
        }

        /// <summary>
        /// Name of application
        /// </summary>
        [ConfigurationProperty(nameProperty, IsRequired = true)]
        public string Name
        {
            get { return (string) base[nameProperty]; }
            set { base[nameProperty] = value; }
        }

        /// <summary>
        /// Name of database
        /// </summary>
        [ConfigurationProperty(databaseProperty, IsRequired = true)]
        public string Database
        {
            get { return (string) base[databaseProperty]; }
            set { base[databaseProperty] = value; }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="COETableEditor"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="COETableEditor"/> objects.
        /// </value>
        [ConfigurationProperty(tableEditorProperty, IsRequired = false)]
        public COENamedElementCollection<COETableEditor> TableEditor
        {
            get { return (COENamedElementCollection<COETableEditor>) base[tableEditorProperty]; }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="COETableEditor"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="COETableEditor"/> objects.
        /// </value>
        [ConfigurationProperty(reportingProperty, IsRequired = false)]
        [ConfigurationCollection(typeof(COEReportingConfiguration), AddItemName = "reportBuilder")]
        public COENamedElementCollection<COEReportingConfiguration> Reporting
        {
            get { return (COENamedElementCollection<COEReportingConfiguration>) base[reportingProperty]; }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="ParentApplicationData"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="ParentApplicationData"/> objects.
        /// </value>
        [ConfigurationProperty(parentApplicationProperty, IsRequired = false)]
        public COENamedElementCollection<ParentApplicationData> ParentApplication
        {
            get { return (COENamedElementCollection<ParentApplicationData>) base[parentApplicationProperty]; }
        }

        private const string formBehaviour = "formBehaviour";
        /// <summary>
        /// Configures forms for chembioviz.
        /// </summary>
        [ConfigurationProperty(formBehaviour, IsRequired = false)]
        [ConfigurationCollection(typeof(Behaviour), AddItemName = "form")]
        public BehaviourCollection FormBehaviour
        {
            get { return (BehaviourCollection) base[formBehaviour]; }
            set { base[formBehaviour] = value; }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="LinkParameters"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="LinkParameters"/> objects.
        /// </value>
        [ConfigurationProperty(linkParametersProperty, IsRequired = false)]
        public COENamedElementCollection<linkParameters> LinkParameters
        {
            get { return (COENamedElementCollection<linkParameters>) base[linkParametersProperty]; }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="InnerXml"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="InnerXml"/> objects.
        /// </value>
        [ConfigurationProperty(innerXmlProperty, IsRequired = false)]
        public COENamedElementCollection<InnerXml> InnerXml
        {
            get { return (COENamedElementCollection<InnerXml>)base[innerXmlProperty]; }
        }

        [ConfigurationProperty(gridConfigSettingsProperty, IsRequired = false)]
        public COENamedElementCollection<WebGrid> WebGridSettings
        {
            get { return (COENamedElementCollection<WebGrid>)base[gridConfigSettingsProperty]; }
        }

        /// <summary>
        /// Will the criterias be stored into db?
        /// </summary>
        [ConfigurationProperty(saveQueryHistory, IsRequired = false)]
        public string SaveQueryHistory
        {
            get { return (string) base[saveQueryHistory]; }
            set { base[saveQueryHistory] = value; }
        }

        /// <summary>
        /// Sets if quick partial searches are going to be performed.
        /// </summary>
        [ConfigurationProperty(returnPartialHitlist, IsRequired = false)]
        public string ReturnPartialHitlist
        {
            get { return (string) base[returnPartialHitlist]; }
            set { base[returnPartialHitlist] = value; }
        }

        /// <summary>
        /// If partial hits is used, this will override the default commit size.
        /// </summary>
        [ConfigurationProperty(partialHitlistCommitSize, IsRequired = false)]
        public int PartialHitlistCommitSize
        {
            get { return (int) base[partialHitlistCommitSize]; }
            set { base[partialHitlistCommitSize] = value; }
        }

        /// <summary>
        /// If partial hits is used, this will override the commit size for the fist insertion only, so will allow a fist bunch
        /// of pages to be retrieved as fast as possible.
        /// </summary>
        [ConfigurationProperty(partialHitlistFirstCommitSize, IsRequired = false)]
        public int PartialHitlistFirstCommitSize
        {
            get { return (int) base[partialHitlistFirstCommitSize]; }
            set { base[partialHitlistFirstCommitSize] = value; }
        }

        [ConfigurationProperty(maxRecordCount, IsRequired = false, DefaultValue = -1)]
        public int MaxRecordCount
        {
            get { return (int) base[maxRecordCount]; }
            set { base[maxRecordCount] = value; }
        }

        /// <summary>
        /// Max hitlist size to export safely.
        /// </summary>
        [ConfigurationProperty(safeExportSize, IsRequired = false, DefaultValue = -1)]
        public int SafeExportSize
        {
            get { return (int) base[safeExportSize]; }
            set { base[safeExportSize] = value; }
        }

        [ConfigurationProperty(showPluginDownloadProperty, IsRequired = false)]
        public string ShowPluginDownload
        {
            get { return (string) base[showPluginDownloadProperty]; }
            set { base[showPluginDownloadProperty] = value; }
        }

        [ConfigurationProperty(pluginDownloadURLProperty, IsRequired = false)]
        public string PluginDownloadURL
        {
            get { return (string) base[pluginDownloadURLProperty]; }
            set { base[pluginDownloadURLProperty] = value; }
        }

        [ConfigurationProperty(downloadChemDrawImageSrcProperty, IsRequired = false)]
        public string DownloadChemDrawImageSrc
        {
            get { return (string) base[downloadChemDrawImageSrcProperty]; }
            set { base[downloadChemDrawImageSrcProperty] = value; }
        }

        [ConfigurationProperty(displayCulture, IsRequired = false)]
        public string DisplayCulture
        {
            get { return (string) base[displayCulture]; }
            set { base[displayCulture] = value; }
        }

        /// <summary>
        /// Default date format is given by the DisplayCulture settings, but if that format is not good, date format may be overriden by
        /// setting this property.
        /// For further information on the string syntax, see: http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx
        /// </summary>
        [ConfigurationProperty(dateFormat, IsRequired = false)]
        public string DateFormat
        {
            get { return (string)base[dateFormat]; }
            set { base[dateFormat] = value; }
        }
        
        /// <summary>
        /// Gets the collection of defined <see cref="ApplicationHome"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="ApplicationHome"/> objects.
        /// </value>
        [ConfigurationProperty(applicationHome, IsRequired = false)]
        public ApplicationHome ApplicationHome
        {
            get { return (ApplicationHome) base[applicationHome]; }
            set { base[applicationHome] = value; }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="AppSettingsData"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="AppSettingsData"/> objects.
        /// </value>
        [ConfigurationProperty(appSettingsProperty, IsRequired = false)]
        public AppSettingsData AppSettings
        {
            get { return (AppSettingsData) base[appSettingsProperty]; }
            set { base[appSettingsProperty] = value; }
        }

        /// <summary>
        /// Indicates if real table names would be returned in DataTables. By default it is false, so returning table names in the form "Table_Tableid"
        /// </summary>
        /// <value>Indicates if real table names would be returned in DataTables. Default value is false.</value>
        [ConfigurationProperty(useRealTableNames, IsRequired = false, DefaultValue = "")]
        public string UseRealTableNames
        {
            get { return (string) base[useRealTableNames]; }
            set { base[useRealTableNames] = value; }
        }

        /// <summary>
        /// Caching configurations for the various items that are cachable
        /// </summary>
        [ConfigurationProperty(cachingProperty, IsRequired = false)]
        public CachingData CachingData
        {
            get { return (CachingData)base[cachingProperty]; }
            set { base[cachingProperty] = value; }
        }

    }
}
