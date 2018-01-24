using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// This class defines the table configuration data that will be used by TableEditor. 
    /// Properties can be added as needed. Each propery should be added to element add in element tableEditor under element applications. 
    /// <code lang="Xml">
    ///        &lt;childTable&gt;
    ///            &lt;add name="VW_project"  primaryKeyField="projectid" parentPK="VW_PeopleProject.personid" childPK="VW_PeopleProject.projectid"&gt;
    ///                &lt;childTableData&gt;
    ///                &lt;/childTableData&gt;
    ///            &lt;/add&gt;
    ///        &lt;/childTable&gt;
    /// </code>
    /// </summary>
    public class COEChildTable : COENamedConfigurationElement
    {
        private const string tableNameProperty = "name";
        private const string primaryKeyProperty = "primaryKeyField";
        private const string parentPKProperty = "parentPK";
        private const string childPKProperty = "childPK";
        private const string childTableDataProperty = "childTableData";
        private const string displayNameProperty = "displayName";
        private const string sqlFilterProperty = "sqlFilter";
        private const string sqlSortOrderProperty = "sqlSortOrder";

        /// <summary>
        /// Initialize a new instance of the <see cref="TableEditor"/> class.
        /// </summary>
        public COEChildTable()
        {
        }

        /// <summary>
        /// Name of the Table used by application
        /// </summary>
        [ConfigurationProperty(tableNameProperty, IsRequired = true)]
        public string Name
        {
            get { return (string)base[tableNameProperty]; }
            set { base[tableNameProperty] = value; }
        }

        /// <summary>
        /// Primary key of the Table
        /// </summary>
        [ConfigurationProperty(primaryKeyProperty, IsRequired = true)]
        public string PrimaryKey
        {
            get { return (string)base[primaryKeyProperty]; }
            set { base[primaryKeyProperty] = value; }
        }

        /// <summary>
        /// parent table primary key
        /// </summary>
        [ConfigurationProperty(parentPKProperty, IsRequired = false)]
        public string ParentPK
        {
            get { return (string)base[parentPKProperty]; }
            set
            {
                if (value != null)
                    base[parentPKProperty] = value;
            }
        }
        
        /// <summary>
        /// child table primary key
        /// </summary>
        [ConfigurationProperty(childPKProperty, IsRequired = false)]
        public string ChildPK
        {
            get { return (string)base[childPKProperty]; }
            set
            {
                if (value != null)
                    base[childPKProperty] = value;
            }
        }

        /// <summary>
        /// child table title
        /// </summary>
        [ConfigurationProperty(displayNameProperty, IsRequired = false)]
        public string DisplayName
        {
            get { return (string)base[displayNameProperty]; }
            set
            {
                if (value != null)
                    base[displayNameProperty] = value;
            }
        }

        /// <summary>
        /// child table where clause
        /// </summary>
        [ConfigurationProperty(sqlFilterProperty, IsRequired = false)]
        public string SqlFilter
        {
            get { return (string)base[sqlFilterProperty]; }
            set 
            {
                if (value != null)
                    base[sqlFilterProperty] = value;
            }
        }

        /// <summary>
        /// child table sort order
        /// </summary>
        [ConfigurationProperty(sqlSortOrderProperty, IsRequired = false)]
        public string SqlSortOrder
        {
            get { return (string)base[sqlSortOrderProperty]; }
            set
            {
                if (value != null)
                    base[sqlSortOrderProperty] = value;
            }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="ChildTableData"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="ChildTableData"/> objects.
        /// </value>
        [ConfigurationProperty(childTableDataProperty, IsRequired = false)]
        public COENamedElementCollection<COEChildTableData> ChildTableData
        {
            get { return (COENamedElementCollection<COEChildTableData>)base[childTableDataProperty]; }
        }
    }
}

