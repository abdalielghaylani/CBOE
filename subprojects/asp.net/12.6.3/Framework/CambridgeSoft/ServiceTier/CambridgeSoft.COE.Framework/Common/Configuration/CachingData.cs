using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// This class defines the various items that have caching configurations
    /// </summary>
    public class CachingData : COEConfigurationElement
    {
        private const string dataviewProperty = "dataview";
        private const string searchCriteriaProperty = "searchCriteria";
        private const string formProperty = "form";

        /// <summary>
        /// Caching configurations for dataview objects
        /// </summary>
        [ConfigurationProperty(dataviewProperty, IsRequired = false)]
        public CacheItemData Dataview
        {
            get { return (CacheItemData)base[dataviewProperty]; }
            set { base[dataviewProperty] = value; }
        }

        /// <summary>
        /// Caching configurations for search criteria objects
        /// </summary>
        [ConfigurationProperty(searchCriteriaProperty, IsRequired = false)]
        public CacheItemData SearchCriteria
        {
            get { return (CacheItemData)base[searchCriteriaProperty]; }
            set { base[searchCriteriaProperty] = value; }
        }

        /// <summary>
        /// Caching configurations for form objects
        /// </summary>
        [ConfigurationProperty(formProperty, IsRequired = false)]
        public CacheItemData Form
        {
            get { return (CacheItemData)base[formProperty]; }
            set { base[formProperty] = value; }
        }
    }
}
