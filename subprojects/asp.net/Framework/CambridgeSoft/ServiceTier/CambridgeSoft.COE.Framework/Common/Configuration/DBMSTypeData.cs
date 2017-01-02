using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Configuration data defining a DBMS type. 
    /// </summary>    	
    public class DBMSTypeData : COENamedConfigurationElement
    {
        private const string nameProperty = "name";
        private const string useProxyProperty = "useProxy";
        private const string datbaseGlobalUserProperty = "databaseGlobalUser";
        private const string dataSourceProperty = "dataSource";
        private const string sqlGeneratorDataProperty = "sqlGeneratorData";

        /// <summary>
        /// Initialize a new instance of the <see cref="DBMSTypeData"/> class.
        /// </summary>
        public DBMSTypeData()
        {
        }

        /// <summary>
        /// Name of Data Provider
        /// </summary>
        [ConfigurationProperty(nameProperty, IsRequired = true)]
        public string Name
        {
            get { return (string)base[nameProperty]; }
            set { base[nameProperty] = value; }
        }

        /// <summary>
        /// source for data  
        /// </summary>
        [ConfigurationProperty(dataSourceProperty, IsRequired = true)]
        public string DataSource
        {
            get { return (string)base[dataSourceProperty]; }
            set { base[dataSourceProperty] = value; }
        }

        /// <summary>
        /// Name of Data Provider
        /// </summary>
        [ConfigurationProperty(useProxyProperty, IsRequired = false)]
        public string UseProxy
        {
            get { return (string)base[useProxyProperty]; }
            set { base[useProxyProperty] = value; }
        }

        /// <summary>
        /// Name of Data Provider
        /// </summary>
        [ConfigurationProperty(datbaseGlobalUserProperty, IsRequired = false)]
        public string DatabaseGlobalUser
        {
            get { return (string)base[datbaseGlobalUserProperty]; }
            set { base[datbaseGlobalUserProperty] = value; }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="SQLGeneratorData"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="SQLGeneratorData"/> objects.
        /// </value>
        [ConfigurationProperty(sqlGeneratorDataProperty, IsRequired = false)]
        public COENamedElementCollection<SQLGeneratorData> SQLGeneratorData
        {
            get { return (COENamedElementCollection<SQLGeneratorData>)base[sqlGeneratorDataProperty]; }
        }

    }
}