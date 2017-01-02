using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// This class defines the validation rule configuration data that will be used by the column. 
    /// Properties can be added with different values of name. Each propery should be added to element add in element validationRule under element tableEditorData. 
    /// <code lang="Xml">
    ///                        &lt;validationRule&gt;
    ///                            &lt;add name="requiredField" errorMessage="This field is required." /&gt; 
    ///                            &lt;add name="chemicallyValid" errorMessage="This field must be a chemical structure." /&gt; 
    ///                        &lt;/validationRule&gt;
    /// 
    /// </code>
    /// </summary>
    public class ValidationRule : COENamedConfigurationElement
    {
        private const string nameProperty = "name";
        private const string errorMessageProperty = "errorMessage";
        private const string parameterProperty = "parameter";
       
        /// <summary>
        /// Initialize a new instance of the <see cref="ValidationRule"/> class.
        /// </summary>
        public ValidationRule()
        {
        }

        /// <summary>
        /// validation rule name 
        /// </summary>
        [ConfigurationProperty(nameProperty, IsRequired = false)]
        public string Name
        {
            get { return (string)base[nameProperty]; }
            set { base[nameProperty] = value; }
        }
        
        /// <summary>
        /// validation rule error message
        /// </summary>
        [ConfigurationProperty(errorMessageProperty, IsRequired = false)]
        public string ErrorMessage
        {
            get { return (string)base[errorMessageProperty]; }
            set { base[errorMessageProperty] = value; }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="Parameter"/> objects.
        /// </summary>
        [ConfigurationProperty(parameterProperty, IsRequired = false)]
        public COENamedElementCollection<Parameter> Parameter
        {
            get { return (COENamedElementCollection<Parameter>)base[parameterProperty]; }
        }
    }
}
