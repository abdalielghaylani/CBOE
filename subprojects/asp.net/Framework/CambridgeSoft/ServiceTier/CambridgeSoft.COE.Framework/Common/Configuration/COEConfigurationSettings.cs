using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;


namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Overall configuration settings for ChemOffice Enterprise Services
    /// </summary>
    [Serializable]
    public class COEConfigurationSettings : COESerializableConfigurationSection, ISerializable
    {
		/// <summary>
		/// Configuration key for ChemOffice Enterprise applications and services.
		/// </summary>
		public const string SectionName = "coeConfiguration";
        //private const string servicesBaseTypeName = "servicesBaseTypeName";
        private const string singleSignOnURL = "singleSignOnURL";
        private const string applicationDataProperty = "applications";
        private const string databaseDataProperty = "databases";
        private const string dbmsTypeDataProperty = "dbmsTypes";
        private const string serviceDataProperty = "services";
        private const string exportFormatterData = "exportFormatters";
        private const string applicationDefaultsDataProperty = "applicationDefaults";
        private const string manageConfigurationSettings = "manageConfigurationSettings";
        

        /// <summary>
        /// Defines the base type used for loading services
        /// </summary>
        [ConfigurationProperty(singleSignOnURL, IsRequired = false)]
        public string SingleSignOnURL
        {
            get { return (string)base[singleSignOnURL]; }
            set { base[singleSignOnURL] = value; }
        }

        /// <summary>
        /// Defines if edition on Admin Settings is enabled
        /// </summary>
        [ConfigurationProperty(manageConfigurationSettings, IsRequired = false)]
        public string ManageConfigurationSettings
        {
            get { return (string)base[manageConfigurationSettings]; }
            set { base[manageConfigurationSettings] = value; }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="ApplicationDefaultsData"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="ApplicationDefaultsData"/> objects.
        /// </value>
        [ConfigurationProperty(applicationDefaultsDataProperty, IsRequired = false)]
        public ApplicationDefaultsData ApplicationDefaults {
            get { return (ApplicationDefaultsData) base[applicationDefaultsDataProperty]; }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="ApplicationData"/> objects.
        /// </summary>
		/// <value>
        /// The collection of defined <see cref="ApplicationData"/> objects.
		/// </value>
        [ConfigurationProperty(applicationDataProperty, IsRequired = true)]
        public COENamedElementCollection<ApplicationData> Applications
		{
            get { return (COENamedElementCollection<ApplicationData>)base[applicationDataProperty]; }
		}

        /// <summary>
        /// Gets the collection of defined <see cref="ServiceData"/> objects.
        /// </summary>
		/// <value>
        /// The collection of defined <see cref="ServiceData"/> objects.
		/// </value>
        [ConfigurationProperty(serviceDataProperty, IsRequired = false)]
        public COENamedElementCollection<ServiceData> Services
		{
            get { return (COENamedElementCollection<ServiceData>)base[serviceDataProperty]; }
		}

        /// <summary>
        /// Gets the collection of defined <see cref="DatabaseData"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="DatabaseData"/> objects.
        /// </value>
        [ConfigurationProperty(databaseDataProperty, IsRequired = true)]
        public COENamedElementCollection<DatabaseData> Databases
        {
            get { return (COENamedElementCollection<DatabaseData>)base[databaseDataProperty]; }
        }
        /// <summary>
        /// Gets the collection of defined <see cref="DBMSTypeData"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="DBMSTypeData"/> objects.
        /// </value>
        /// 

        [ConfigurationProperty(dbmsTypeDataProperty, IsRequired = true)]
        public COENamedElementCollection<DBMSTypeData> DBMSTypes
        {
            get { return (COENamedElementCollection<DBMSTypeData>)base[dbmsTypeDataProperty]; }
        }

          /// <summary>
        /// Gets the collection of defined <see cref="ExportFormatterData"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="ExportFormatterData"/> objects.
        /// </value>
        /// 
        [ConfigurationProperty(exportFormatterData, IsRequired = true)]
        public COENamedElementCollection<ExportFormatterData> ExportFormatters
        {
            get { return (COENamedElementCollection<ExportFormatterData>)base[exportFormatterData]; }
        }

        public COEConfigurationSettings()
        {
        }       

        #region ISerializable Members
        public COEConfigurationSettings(SerializationInfo info, StreamingContext context)
        {
            StringReader strReader = new StringReader(info.GetString("XMLValue"));
            XmlReader xmlReader = XmlReader.Create(strReader);
            this.ReadXml(xmlReader);
            xmlReader.Close();
            strReader.Close();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(xmlString);
            this.WriteXml(writer);
            writer.Close();
            info.AddValue("XMLValue", xmlString.ToString());
        }
        #endregion
    }
}
