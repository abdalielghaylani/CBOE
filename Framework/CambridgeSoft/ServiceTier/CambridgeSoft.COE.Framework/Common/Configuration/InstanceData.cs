using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Configuration data defining instance data. 
    /// </summary>
    public class InstanceData : COENamedConfigurationElement
    {
        private const string idProperty = "id";
        private const string instanceNameProperty = "name";
        private const string dbmsTypeProperty = "dbmsType";
        private const string dataSourceProperty = "dataSource";
        private const string databaseGlobalUserProperty = "databaseGlobalUser";
        private const string isCBOEInstanceProperty = "isCBOEInstance";
        private const string useProxyProperty = "useProxy";
        private const string hostNameProperty = "hostName";
        private const string portProperty = "port";
        private const string sidProperty = "sid";
        private const string sqlGeneratorDataProperty = "sqlGeneratorData";
        private const string driverTypeProperty = "driverType";

        /// <summary>
        /// Initialize a new instance of the <see cref="InstanceData"/> class.
        /// </summary>
        public InstanceData()
        {
        }

        /// <summary>
        /// The instance unique id.
        /// </summary>
        [ConfigurationProperty(idProperty, IsRequired = true)]
        public Guid Id
        {
            get { return (Guid)base[idProperty]; }
            set { base[idProperty] = value; }
        }

        /// <summary>
        /// Instance name of application
        /// </summary>
        [ConfigurationProperty(instanceNameProperty, IsRequired = true)]
        public string InstanceName
        {
            get { return (string)base[instanceNameProperty]; }
            set { base[instanceNameProperty] = value; }
        }

        /// <summary>
        /// dbms type for application
        /// </summary>
        [ConfigurationProperty(dbmsTypeProperty, IsRequired = true)]
        public DBMSType DBMSType
        {
            get { return (DBMSType)base[dbmsTypeProperty]; }
            set { base[dbmsTypeProperty] = value; }
        }

        /// <summary>
        /// dataSource for application
        /// </summary>
        [ConfigurationProperty(dataSourceProperty, IsRequired = false)]
        public string DataSource
        {
            get { return (string)base[dataSourceProperty]; }
            set { base[dataSourceProperty] = value; }
        }

        /// <summary>
        /// databaseGlobalUser for application
        /// </summary>
        [ConfigurationProperty(databaseGlobalUserProperty, IsRequired = true)]
        public string DatabaseGlobalUser
        {
            get { return (string)base[databaseGlobalUserProperty]; }
            set { base[databaseGlobalUserProperty] = value; }
        }

        /// <summary>
        /// isCBOEInstance for application
        /// </summary>
        [ConfigurationProperty(isCBOEInstanceProperty, IsRequired = false)]
        public bool IsCBOEInstance
        {
            get { return (bool)base[isCBOEInstanceProperty]; }
            set { base[isCBOEInstanceProperty] = value; }
        }

        /// <summary>
        /// useProxy for application
        /// </summary>
        [ConfigurationProperty(useProxyProperty, IsRequired = false)]
        public bool UseProxy
        {
            get { return (bool)base[useProxyProperty]; }
            set { base[useProxyProperty] = value; }
        }

        /// <summary>
        /// HostName for application
        /// </summary>
        [ConfigurationProperty(hostNameProperty, IsRequired = false)]
        public string HostName
        {
            get { return (string)base[hostNameProperty]; }
            set { base[hostNameProperty] = value; }
        }

        /// <summary>
        /// Port for application
        /// </summary>
        [ConfigurationProperty(portProperty, IsRequired = false)]
        public int Port
        {
            get { return (int)base[portProperty]; }
            set { base[portProperty] = value; }
        }

        /// <summary>
        /// SID for application
        /// </summary>
        [ConfigurationProperty(sidProperty, IsRequired = false)]
        public string SID
        {
            get { return (string)base[sidProperty]; }
            set { base[sidProperty] = value; }
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
            set { base[sqlGeneratorDataProperty] = value; }
        }

        /// <summary>
        /// driver type for application
        /// </summary>
        [ConfigurationProperty(driverTypeProperty, IsRequired = false)]
        public DriverType DriverType
        {
            get { return (DriverType)base[driverTypeProperty]; }
            set { base[driverTypeProperty] = value; }
        }
    }
}
