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
    ///        &lt;tableEditor&gt;
    ///            &lt;add name="FRAGMENTS" description="fragment table" primaryKeyField="FRAGMENT_ID" sequenceName="SEQ_FRAGMENTS"&gt;
    ///                &lt;tableEditorData&gt;
    ///                &lt;/tableEditorData&gt;
    ///            &lt;/add&gt;
    ///        &lt;/tableEditor&gt;
    /// </code>
    /// </summary>
    public class COETableEditor : COENamedConfigurationElement
    {
        private const string tableNameProperty = "name";
        private const string primaryKeyProperty = "primaryKeyField";
        private const string descriptionProperty = "description";
        private const string displayNameProperty = "displayName";
        private const string maxPageSizeProperty = "maxPageSize";
        private const string sequenceNameProperty = "sequenceName";
        private const string tableEditorDataProperty = "tableEditorData";
        private const string childTableProperty = "childTable";
        private const string addPrivProperty = "addPriv";
        private const string editPrivProperty = "editPriv";
        private const string deletePrivProperty = "deletePriv";
        private const string disableChildTablesProperty = "disableChildTables";
        private const string disableTableProperty = "disableTable";

        /// <summary>
        /// Initialize a new instance of the <see cref="TableEditor"/> class.
        /// </summary>
        public COETableEditor()
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
        /// The table's description. will show on the current page
        /// </summary>
        [ConfigurationProperty(descriptionProperty, IsRequired = true)]
        public string Description
        {
            get { return (string)base[descriptionProperty]; }
            set
            {
                if (value != null)
                    base[descriptionProperty] = value;
            }
        }

        /// <summary>
        /// The table's display name. Used as friendly name only.
        /// </summary>
        [ConfigurationProperty(displayNameProperty, IsRequired = false)]
        public string DisplayName
        {
            get { return (string) base[displayNameProperty]; }
            set
            {
                if(value != null)
                    base[displayNameProperty] = value;
            }
        }

        [ConfigurationProperty(maxPageSizeProperty, IsRequired = false)]
        public string MaxPageSize
        {
            get { return (string) base[maxPageSizeProperty]; }
            set { if(value != null) base[maxPageSizeProperty] = value; }
        }

        /// <summary>
        /// Used to generate primary key.
        /// Leave out SequenceName or set SequenceName=”” when you have a trigger taking care of PK; 
        /// set SequenceName=”sequecename?when you don’t have the trigger, Table Editor then takes care of PK and if the sequence does not exist, Table Editor will create it. 
        /// </summary>
        [ConfigurationProperty(sequenceNameProperty, IsRequired = false)]
        public string SequenceName
        {
            get { return (string)base[sequenceNameProperty]; }
            set
            {
                if (value != null)
                    base[sequenceNameProperty] = value;
            }
        }
        /// <summary>
        /// Gets the collection of defined <see cref="TableEditorData"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="TableEditorData"/> objects.
        /// </value>
        [ConfigurationProperty(tableEditorDataProperty, IsRequired = false)]
        public COENamedElementCollection<COETableEditorData> TableEditorData
        {
            get { return (COENamedElementCollection<COETableEditorData>)base[tableEditorDataProperty]; }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="ChildTable"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="ChildTable"/> objects.
        /// </value>
        [ConfigurationProperty(childTableProperty, IsRequired = false)]
        public COENamedElementCollection<COEChildTable> ChildTable
        {
            get { return (COENamedElementCollection<COEChildTable>)base[childTableProperty]; }
        }

        /// <summary>
        /// Privileges needed to add rows to a table.
        /// </summary>
        [ConfigurationProperty(addPrivProperty, IsRequired = false)]
        public string AddPrivileges
        {
            get { return (string) base[addPrivProperty]; }
            set
            {
                if(value != null)
                    base[addPrivProperty] = value;
            }
        }

        /// <summary>
        /// Privileges needed to edit rows of the current table.
        /// </summary>
        [ConfigurationProperty(editPrivProperty, IsRequired = false)]
        public string EditPrivileges
        {
            get { return (string) base[editPrivProperty]; }
            set
            {
                if(value != null)
                    base[editPrivProperty] = value;
            }
        }

        /// <summary>
        /// Privileges needed to delete rows from a table.
        /// </summary>
        [ConfigurationProperty(deletePrivProperty, IsRequired = false)]
        public string DeletePrivileges
        {
            get { return (string) base[deletePrivProperty]; }
            set
            {
                if(value != null)
                    base[deletePrivProperty] = value;
            }
        }

        /// <summary>
        /// Enables/Disables child tables 
        /// </summary>
        [ConfigurationProperty(disableChildTablesProperty, IsRequired = false)]
        public bool DisableChildTables
        {
            get { return (bool)base[disableChildTablesProperty]; }
            set
            {
                if (value != null)
                    base[disableChildTablesProperty] = value;
            }
        }

        /// <summary>
        /// Enables/Disables Table from GUI
        /// </summary>
        [ConfigurationProperty(disableTableProperty, IsRequired = false)]
        public bool DisableTable
        {
            get { return (bool)base[disableTableProperty]; }
            set
            {
                if (value != null)
                    base[disableTableProperty] = value;
            }
        }

    }
}

