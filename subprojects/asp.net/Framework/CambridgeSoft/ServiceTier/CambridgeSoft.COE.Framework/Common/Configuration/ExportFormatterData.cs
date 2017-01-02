using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Configuration data defining formatters for the COEExport service
    /// </summary>    	
    public class ExportFormatterData : COENamedConfigurationElement
    {
        private const string _nameProperty = "name";
        private const string _formatterAssemblyNameProperty = "assembly";
        private const string _formatterTypeNameProperty = "type";

       
        /// <summary>
		/// Initialize a new instance of the <see cref="ApplicationData"/> class.
		/// </summary>
        public ExportFormatterData()
		{
		}
        /// <summary>
        /// Name of service
        /// </summary>
        [ConfigurationProperty(_nameProperty, IsRequired = true)]
        public string Name
        {
            get { return (string)base[_nameProperty]; }
            set { base[_nameProperty] = value; }
        }

        /// <summary>
        /// Full name of assebmly, including public token for the formater
        /// </summary>
        [ConfigurationProperty(_formatterAssemblyNameProperty, IsRequired = true)]
        public string FormatterAssemblyName
        {
            get { return (string)base[_formatterAssemblyNameProperty]; }
            set { base[_formatterAssemblyNameProperty] = value; }
        }

        /// <summary>
        /// Name type for formatter
        /// </summary>
        [ConfigurationProperty(_formatterTypeNameProperty, IsRequired = true)]
        public string FormatterTypeName
        {
            get { return (string)base[_formatterTypeNameProperty]; }
            set { base[_formatterTypeNameProperty] = value; }
        }

      

       
    }

}
