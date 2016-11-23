using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// This class defines (validation rule's) parameter configuration data that will be used by the validation rule. 
    /// Properties should be added as required by the validation rule. Each propery should be added to element add in element parameter under element validationRule. 
    /// <code lang="Xml">
    ///                            	<parameter>
    ///                            		<add name="min" value="1" />
    ///                            		<add name="max" value="10" />
    ///                            	<parameter>
    /// </code>
    /// </summary>
    public class Parameter : COENamedConfigurationElement
    {
        private const string nameProperty = "name";
        private const string valueProperty = "value";

        /// <summary>
        /// Initialize a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        public Parameter()
        {
        }

        /// <summary>
        /// Name of the ValidationRule Parameter
        /// </summary>
        [ConfigurationProperty(nameProperty, IsRequired = false)]
        public string Name
        {
            get { return (string)base[nameProperty]; }
            set { base[nameProperty] = value; }
        }

        /// <summary>
        /// Value of the ValidationRule Parameter
        /// </summary>
        [ConfigurationProperty(valueProperty, IsRequired = false)]
        public string Value
        {
            get { return (string)base[valueProperty]; }
            set { base[valueProperty] = value; }
        }
    }
}
