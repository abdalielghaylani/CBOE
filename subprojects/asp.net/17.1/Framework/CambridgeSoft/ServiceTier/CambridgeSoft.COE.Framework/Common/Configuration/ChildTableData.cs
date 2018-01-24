using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// This class defines the (child table's) column configuration data that will be used by the child table. 
    /// Properties can be added with different values of name. Each propery should be added to element add in element childTableData under element childTable. 
    /// <code lang="Xml">
    ///                &lt;childTableData&gt;
    ///                    &lt;add name="PROJECTID" dataType="number" /&gt; 
    ///                    &lt;add name="name" dataType="string" /&gt;
    ///                &lt;/childTableData&gt;
    /// </code>
    /// </summary>
    public class COEChildTableData : COENamedConfigurationElement
    {
        private const string columnNameProperty = "name";
        private const string dataTypeProperty = "dataType";

        /// <summary>
        /// Initialize a new instance of the <see cref="ChildTableData"/> class.
        /// </summary>
        public COEChildTableData()
        {
        }

        /// <summary>
        /// Name of the column(<see cref="ChildTableData"/>) used by Table (<<see cref="ChildTable"/>>)
        /// </summary>
        [ConfigurationProperty(columnNameProperty, IsRequired = true)]
        public string Name
        {
            get { return (string)base[columnNameProperty]; }
            set { base[columnNameProperty] = value; }
        }

        /// <summary>
        /// Data type of the Column (<see cref="ChildTableData"/>)
        /// </summary>
        [ConfigurationProperty(dataTypeProperty, IsRequired = true)]
        public string DataType
        {
            get { return (string)base[dataTypeProperty]; }
            set { base[dataTypeProperty] = value; }
        }
    }
}
