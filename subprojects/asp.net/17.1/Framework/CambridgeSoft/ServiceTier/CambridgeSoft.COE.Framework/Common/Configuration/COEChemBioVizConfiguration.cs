using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Configuration settings for Chem Bio Viz search module.
    /// </summary>    
    public class COEChemBioVizConfiguration : COESerializableConfigurationSection {
        /// <summary>
        /// Configuration key for ChemBioViz Search Module.
        /// </summary>
        ///
        public const string SectionName = "coeChemBioVizConfiguration";
        private const string _coeFormGroupID = "coeFormGroupID";
        
        /// <summary>
        /// name used to identify calling app in logging config output
        /// </summary>
        [ConfigurationProperty(_coeFormGroupID, IsRequired = true)]
        public int COEFormGroupID {
            get { return (int) base[_coeFormGroupID]; }
        }
    }
}
