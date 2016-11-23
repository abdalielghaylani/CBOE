using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Overall configuration settings for ChemOffice Enterprise Services
    /// </summary>    
    public class COEClientConfigurationSettings : COESerializableConfigurationSection 
    {
    
		/// <summary>
		/// Configuration key for ChemOffice Enterprise applications and services.
		/// </summary>
		public const string SectionName = "coeClientConfiguration";
        //private const string servicesBaseTypeName = "servicesBaseTypeName";
        private const string file = "file";
        
        /// <summary>
        /// Defines the use of a file to load the configuration
        /// </summary>
        [ConfigurationProperty(file, IsRequired = false)]
        public string File
        {
            get { return (string)base[file]; }
            set { base[file] = value; }
        }

       

       



 

       
    
    }
}
