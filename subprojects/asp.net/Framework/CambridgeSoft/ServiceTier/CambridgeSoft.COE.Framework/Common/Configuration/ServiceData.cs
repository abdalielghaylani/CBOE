using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Configuration data defining an CBOE Service
    /// </summary>    	
    public class ServiceData : COENamedConfigurationElement
    {
        private const string nameProperty = "name";
        private const string dbmsTypeProperty = "dbmsType";
        private const string dalProviderAssemblyNameShort = "dalProviderAssemblyNameShort";
        private const string dalProviderAssemblyNameFull = "dalProviderAssemblyNameFull";
        private const string dalProviderDataProperty = "dalProviders";
        private const string searchServiceDataProperty = "searchServiceData";
        private const string partialHitlistCommitSize = "partialHitlistCommitSize";
        private const string maxRecordCount = "maxRecordCount";


        /// <summary>
        /// Initialize a new instance of the <see cref="ServiceData"/> class.
		/// </summary>
		public ServiceData()
		{
		}

        /// <summary>
        /// Name of service
        /// </summary>
        [ConfigurationProperty(nameProperty, IsRequired = true)]
        public string Name
        {
            get { return (string)base[nameProperty]; }
            set { base[nameProperty] = value; }
        }

        /// <summary>
        /// Name of service assembly to use short form
        /// </summary>
        [ConfigurationProperty(dalProviderAssemblyNameShort, IsRequired = true)]
        public string DALProviderAssemblyNameShort
        {
            get { return (string)base[dalProviderAssemblyNameShort]; }
            set { base[dalProviderAssemblyNameShort] = value; }
        }

        /// <summary>
        /// Name of service assbembly to use full form
        /// </summary>
        [ConfigurationProperty(dalProviderAssemblyNameFull, IsRequired = true)]
        public string DALProviderAssemblyNameFull
        {
            get { return (string)base[dalProviderAssemblyNameFull]; }
            set { base[dalProviderAssemblyNameFull] = value; }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="DALProviderData"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="DALProviderData"/> objects.
        /// </value>
        [ConfigurationProperty(dalProviderDataProperty, IsRequired = false)]
        public COENamedElementCollection<DALProviderData> DALProviders
        {
            get { return (COENamedElementCollection<DALProviderData>)base[dalProviderDataProperty]; }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="SearchServiceData"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="SearchServiceData"/> objects.
        /// </value>
        [ConfigurationProperty(searchServiceDataProperty, IsRequired = false)]
        public SearchServiceData SearchServiceData
        {
            get { return (SearchServiceData)base[searchServiceDataProperty]; }
        }

        /// <summary>
        /// If partial hits is used, this is the default commit size.
        /// </summary>
        [ConfigurationProperty(partialHitlistCommitSize, IsRequired = false)]
        [DefaultValue(150)]
        public int PartialHitlistCommitSize {
            get { return (int) base[partialHitlistCommitSize]; }
            set { base[partialHitlistCommitSize] = value; }
        }

        /// <summary>
        /// If set, limits the number of records retrieved.
        /// </summary>
        [ConfigurationProperty(maxRecordCount, IsRequired = false)]
        public int MaxRecordCount
        {
            get { return (int)base[maxRecordCount]; }
            set { base[maxRecordCount] = value; }
        }
    }
}
