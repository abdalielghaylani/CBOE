using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// This class defines the configuration data for the TableEditor for each application
    /// properties can be added as needed. Each propery will be added to the add element in the TableEditor parent element
    /// <application name="SAMPLE">   
    ///     <TableEditor>
    ///         <add name="Project" PrimaryKeyField="ID" FieldToShow="Name" >
    ///     </TableEditor>
    /// </application>
    /// </summary>
    public class WebGrid : COENamedConfigurationElement
    {
        private const string keyNameProperty = "name";
        private const string valueProperty = "value";


        /// <summary>
        /// Initialize a new instance of the <see cref="TableEditor"/> class.
        /// </summary>
        public WebGrid()
        {
        }
        /// <summary>
        /// Name of the Table used by application
        /// </summary>
        [ConfigurationProperty(keyNameProperty, IsRequired = true)]
        public string Name
        {
            get { return (string)base[keyNameProperty]; }
            set { base[keyNameProperty] = value; }
        }

        /// <summary>
        /// Primary key of the Table
        /// </summary>
        [ConfigurationProperty(valueProperty, IsRequired = true)]
        public string Value
        {
            get { return (string)base[valueProperty]; }
            set { base[valueProperty] = value; }
        }
      


    }
}

