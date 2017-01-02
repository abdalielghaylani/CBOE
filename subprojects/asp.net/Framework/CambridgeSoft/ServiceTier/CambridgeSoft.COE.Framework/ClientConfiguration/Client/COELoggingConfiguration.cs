using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Logging settings
    /// </summary>    
    public class COELoggingConfiguration : COESerializableConfigurationSection
    {
    
		/// <summary>
		/// Configuration key for ChemOffice Enterprise applications and services.
		/// </summary>
        ///
        public const string SectionName = "coeLoggingConfiguration";
        private const string _categories = "categories";
        private const string _priority = "priority";
        private const string  _severity = "severity";
        private const string _enabled = "enabled";
        private const string _logEntryIdentifier = "logEntryIdentifier";

        /// <summary>
        /// specifies if logging is turned on or off
        /// </summary>
        [ConfigurationProperty(_enabled, IsRequired = true)]
        public string Enabled
        {
            get { return (string)base[_enabled]; }
            set { base[_enabled] = value; }

        }
        /// <summary>
        /// name used to identify calling app in logging config output
        /// </summary>
        [ConfigurationProperty(_logEntryIdentifier, IsRequired = true)]
        public string LogEntryIdentifier
        {
            get { return (string)base[_logEntryIdentifier]; }
            set { base[_logEntryIdentifier] = value; }

        }

       



        /// <summary>
        /// Defines the categories for logging 
        /// </summary>
        [ConfigurationProperty(_categories, IsRequired = false)]
        public CatagoryTypes Categories
        {
            get { return (CatagoryTypes)base[_categories]; }
            set { base[_categories] = value; }
        }

        /// <summary>
        /// Defines the severities for logging 
        /// </summary>
        [ConfigurationProperty(_severity, IsRequired = false)]
        public System.Diagnostics.SourceLevels Severity
        {
            get { return (System.Diagnostics.SourceLevels)base[_severity]; }
            set { base[_severity] = value; }
        }

        /// <summary>
        /// Defines the priority of logging 
        /// </summary>
        [ConfigurationProperty(_priority, IsRequired = false)]
        public string Priority
        {
            get { return (string)base[_priority]; }
            set { base[_priority] = value; }
        }



        #region ICloneable Members

        public COELoggingConfiguration Clone()
        {
            COELoggingConfiguration clone = new COELoggingConfiguration();
            clone.Categories = this.Categories;
            clone.Enabled = this.Enabled;
            clone.Priority = this.Priority;
            clone.LogEntryIdentifier = this.LogEntryIdentifier;
            clone.Severity = this.Severity;
            return (COELoggingConfiguration)clone;
        }

        #endregion
    }
    public enum CatagoryTypes
    {
        All, COESearch, COEDataView, COEConfiguration, COESearchCriteria, COEHitList, COESecurity, COEForm, COEGenericObjectStorage, COEDatabase

    }
}
