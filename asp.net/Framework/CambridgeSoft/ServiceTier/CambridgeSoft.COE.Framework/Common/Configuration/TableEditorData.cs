using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// This class defines the (table's) column configuration data that will be used by the table. 
    /// Properties can be added with different values of name. Each propery should be added to element add in element tableEditorData under element tableEditor. 
    /// <code lang="Xml">
    ///                &lt;tableEditorData&gt;
    ///                    &lt;add name="FRAGMENT_ID" dataType="number" /> 
    ///                    &lt;add name="BASE64_CDX" dataType="string" isStructure="true" structureType="BASE64_CDX"&gt;
    ///                        &lt;validationRule&gt;
    ///                        &lt;/validationRule&gt;
    ///                    &lt;/add&gt;
    ///                &lt;/tableEditorData&gt;
    /// </code>
    /// </summary>
    public class COETableEditorData : COENamedConfigurationElement
    {
        private const string columnNameProperty = "name";
        private const string dataTypeProperty = "dataType";
        private const string lookupFieldProperty = "lookupField";
        private const string lookupIDProperty = "lookupID";
        private const string isStructureProperty = "isStructure";
        private const string structureTypeProperty = "structureType";
        private const string validationRuleProperty = "validationRule";
        private const string isStructureLookupFieldProperty = "isStructureLookupField";
        private const string lookupLocationProperty = "lookupLocation";
        private const string aliasProperty = "alias";
        private const string hidden = "hidden";
        private const string isUnique = "isUnique";
        private const string isUsedCheck = "isUsedCheck";
        private const string defaultValue = "defaultValue";
        private const string lookupFilter = "lookupFilter";

        /// <summary>
        /// Initialize a new instance of the <see cref="TableEditorData"/> class.
        /// </summary>
        public COETableEditorData()
        {
        }

        /// <summary>
        /// Name of the column(<see cref="TableEditorData"/>) used by Table (<<see cref="TableEditor"/>>)
        /// </summary>
        [ConfigurationProperty(columnNameProperty, IsRequired = true)]
        public string Name
        {
            get { return (string)base[columnNameProperty]; }
            set { base[columnNameProperty] = value; }
        }

        /// <summary>
        /// LookupField in another table for the column(<see cref="TableEditorData"/>) used by Table (<<see cref="TableEditor"/>>)
        /// </summary>
        [ConfigurationProperty(lookupFieldProperty, IsRequired = false)]
        public string LookupField
        {
            get { return (string)base[lookupFieldProperty]; }
            set { base[lookupFieldProperty] = value; }
        }

        /// <summary>
        /// LookupID in another table for the column(<see cref="TableEditorData"/>) used by Table (<<see cref="TableEditor"/>>)
        /// </summary>
        [ConfigurationProperty(lookupIDProperty, IsRequired = false)]
        public string LookupID
        {
            get { return (string)base[lookupIDProperty]; }
            set { base[lookupIDProperty] = value; }
        }

        /// <summary>
        /// Data type of the Column (<see cref="TableEditorData"/>)
        /// </summary>
        [ConfigurationProperty(dataTypeProperty, IsRequired = true)]
        public string DataType
        {
            get { return (string)base[dataTypeProperty]; }
            set { base[dataTypeProperty] = value; }
        }
        /// <summary>
        /// If a field is structure
        /// </summary>
        [ConfigurationProperty(isStructureProperty, IsRequired = false)]
        public string IsStructure
        {
            get { return (string)base[isStructureProperty]; }
            set { base[isStructureProperty] = value; }
        }
       
        /// <summary>
        /// Structure type: B64CDX, CDXML, etc.
        /// </summary>
        [ConfigurationProperty(structureTypeProperty, IsRequired = false)]
        public string StructureType
        {
            get { return (string)base[structureTypeProperty]; }
            set { base[structureTypeProperty] = value; }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="ValidationRule"/> ValidationRule.
        /// </summary>
        [ConfigurationProperty(validationRuleProperty, IsRequired = false)]
        public COENamedElementCollection<ValidationRule> ValidationRule
        {
            get { return (COENamedElementCollection<ValidationRule>)base[validationRuleProperty]; }
        }

        /// <summary>
        /// If LookupField is structure
        /// </summary>
        [ConfigurationProperty(isStructureLookupFieldProperty, IsRequired = false)]
        public string IsStructureLookupField
        {
            get { return (string)base[isStructureLookupFieldProperty]; }
            set
            {
                if (value != null)
                    base[isStructureLookupFieldProperty] = value;
            }
        }

        /// <summary>
        /// LookupLocation determines the location to find lookup values.
        /// When lookup values are from table in database, LookupLocation should be set to “database? 
        /// when lookup values are from hard coded xml file, LookupLocation should be set to the xml file name preceding with its exact path info. 
        /// Refer to the hard coded xml file.
        /// </summary>
        [ConfigurationProperty(lookupLocationProperty, IsRequired = false)]
        public string LookupLocation
        {
            get { return (string)base[lookupLocationProperty]; }
            set
            {
                if (value != null)
                    base[lookupLocationProperty] = value;
            }
        }

        /// <summary>
        /// Alias of the column(<see cref="TableEditorData"/>) used by Table (<<see cref="TableEditor"/>>)
        /// </summary>
        [ConfigurationProperty(aliasProperty, IsRequired = false)]
        public string Alias
        {
            get { return (string)base[aliasProperty]; }
            set { base[aliasProperty] = value; }
        }

        /// <summary>
        /// Hide/Show column from GUI(<see cref="TableEditorData"/>) used by Table (<<see cref="TableEditor"/>>)
        /// Allowed values true/false
        /// </summary>
        [ConfigurationProperty(hidden, IsRequired = false)]        
        public bool Hidden
        {
            get { return (bool)base[hidden]; }
            set { base[hidden] = value; }
        }

        /// <summary>
        /// Property to check the uniqueness of the field based on the value in the database
        /// </summary>
        [ConfigurationProperty(isUnique, IsRequired = false)]
        public bool IsUnique
        {
            get { return (bool)base[isUnique]; }
            set { base[isUnique] = value; }
        }

        /// <summary>
        ///Property to do custom checking whether the field is used for any records( Validate 
        /// </summary>
        [ConfigurationProperty(isUsedCheck, IsRequired = false)]
        public string IsUsedCheck
        {
            get { return (string)base[isUsedCheck]; }
            set { base[isUnique] = value; }
        }

        /// <summary>
        ///Property to get the default value assigned to the column, (Updates DB in case user selects null) 
        /// </summary>
        [ConfigurationProperty(defaultValue, IsRequired = false)]
        public string DefaultValue
        {
            get { return (string)base[defaultValue]; }
            set { base[defaultValue] = value; }
        }

        /// <summary>
        ///Property to get the filter value assigned to the lookup fields
        /// </summary>
        [ConfigurationProperty(lookupFilter, IsRequired = false)]
        public string LookupFilter
        {
            get { return (string)base[lookupFilter]; }
            set { base[lookupFilter] = value; }
        }
    }
}
