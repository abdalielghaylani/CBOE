using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Configuration data defining a data access provider.
    /// </summary>
    public class DALProviderData : COENamedConfigurationElement
    {
        private const string nameProperty = "name";
        private const string dalClassProperty = "dalClass";
        
       
        /// <summary>
        /// Initialize a new instance of the <see cref="DALProviderData"/> class.
		/// </summary>
        public DALProviderData()
		{
		}
        /// <summary>
        /// Name of the provider
        /// </summary>
        [ConfigurationProperty(nameProperty, IsRequired = true)]
        public string Name
        {
            get { return (string)base[nameProperty]; }
            set { base[nameProperty] = value; }
        }

        /// <summary>
        /// Name of application
        /// </summary>
        [ConfigurationProperty(dalClassProperty, IsRequired = true)]
        public string DALClass
        {
            get { return (string)base[dalClassProperty]; }
            set { base[dalClassProperty] = value; }
        }
    }
}

