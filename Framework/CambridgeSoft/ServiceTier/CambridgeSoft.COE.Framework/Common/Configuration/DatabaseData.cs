using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Configuration data defining an Applicaton using a core service. 
    /// </summary>    	
    public class DatabaseData : COENamedConfigurationElement
    {
        private const string nameProperty = "name";
        private const string dbmsTypeProperty = "dbmsType";
        private const string ownerProperty = "owner";
        private const string passwordProperty = "password";
        private const string providerNameProperty = "providerName";
        private const string providerOptionsProperty = "providerOptions";
        private const string tracingProperty = "tracing";
        private const string oracleTracingProperty = "oracleTracing";
        private const string dataSourceProperty = "dataSource";  
        private const string sqlGeneratorDataProperty = "sqlGeneratorData";
        private const string instanceIdProperty = "instanceId";

        private string originalOwner = "originalOwner";

        /// <summary>
        /// Initialize a new instance of the <see cref="DatabaseData"/> class.
		/// </summary>
		public DatabaseData()
		{
		}

        private InstanceData _instanceData;
        public InstanceData InstanceData
        {
            get
            {
                return this._instanceData;
            }
            set
            {
                this._instanceData = value;
                if (value != null)
                {
                    this.InstanceId = value.Id;
                }
            }
        }

        /// <summary>
        /// Name of application
        /// </summary>
        [ConfigurationProperty(nameProperty, IsRequired = true)]
        public string Name
        {
            get { return (string)base[nameProperty]; }
            set { base[nameProperty] = value; }
        }

        /// <summary>
        /// Instance name for connecting
        /// </summary>
        [ConfigurationProperty(instanceIdProperty, IsRequired = false)]
        public Guid InstanceId
        {
            get { return (Guid)base[instanceIdProperty]; }
            set { base[instanceIdProperty] = value; }
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
        /// data source override
        /// </summary>
        [ConfigurationProperty(dataSourceProperty, IsRequired = false)]
        public string DataSource
        {
            get { return (string)base[dataSourceProperty]; }
            set { base[dataSourceProperty] = value; }
        }

        /// <summary>
        /// owner name of schema or catalog 
        /// </summary>
        [ConfigurationProperty(ownerProperty, IsRequired = false)]
        public string Owner
        {
            get { return (string)base[ownerProperty]; }
            set { base[ownerProperty] = value; }
        }

        /// <summary>
        /// the original owner from the call before it is changed by a globaldatabase attrigute name of schema or catalog not a configuration setting 
        /// </summary>
        public string OriginalOwner
        {
            get { return originalOwner.Substring(originalOwner.IndexOf(".")+1); }
            set { originalOwner = value; }
        }

        /// <summary>
        /// password for building connecton string for dbms
        /// </summary>
        [ConfigurationProperty(passwordProperty, IsRequired = true)]
        public string Password
        {
            get { return (string)base[passwordProperty]; }
            set { base[passwordProperty] = value; }
        }

        /// <summary>
        /// provider name used for connecting
        /// </summary>
        [ConfigurationProperty(providerNameProperty, IsRequired = true)]
        public string ProviderName
        {
            get { return (string)base[providerNameProperty]; }
            set { base[providerNameProperty] = value; }
        }

        /// <summary>
        /// provider options used for connecting
        /// </summary>
        [ConfigurationProperty(providerOptionsProperty, IsRequired = false)]
        public string ProviderOptions
        {
            get { return (string)base[providerOptionsProperty]; }
            set { base[providerOptionsProperty] = value; }
        }

        /// <summary>
        /// dbms type for application 
        /// </summary>
        [ConfigurationProperty(oracleTracingProperty, IsRequired = false)]
        public bool OracleTracing
        {
            get { return (bool)base[oracleTracingProperty]; }
            set { base[oracleTracingProperty] = value; }
        }

        /// <summary>
        /// dbms type for application 
        /// </summary>
        [ConfigurationProperty(tracingProperty, IsRequired = false)]
        public bool Tracing
        {
            get { return (bool)base[tracingProperty]; }
            set { base[tracingProperty] = value; }
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
