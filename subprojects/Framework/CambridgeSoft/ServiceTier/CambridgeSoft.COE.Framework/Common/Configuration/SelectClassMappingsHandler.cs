using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Configuration information for mapping select clauses with its implementing class
    /// </summary>
    public class SelectClassMappingsHandler : COEConfigurationSection
    {
        /// <summary>
        /// The xml namespace if applies.
        /// </summary>
        [ConfigurationProperty("xmlns")]
        public string XmlNamespace
        {
            get { return (string)this["xmlns"]; }
            set { this["xmlns"] = value; }
        }

        /// <summary>
        /// Collection of Select clause to map.
        /// </summary>
        [ConfigurationProperty("selectClauses", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(SelectClassMappingsHandler.SelectClauseMapping), AddItemName = "selectClause")]
        public SelectClauseMappingCollection Mappings
        {
            get { return (SelectClauseMappingCollection)this["selectClauses"]; }
            set { this["selectClauses"] = value; }
        }

        /// <summary>
        /// Collection of Where clause to map.
        /// </summary>
        [ConfigurationProperty("whereClauses", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(SelectClassMappingsHandler.WhereClauseMapping), AddItemName = "whereClause")]
        public WhereClauseMappingCollection WhereClauseMappings {
            get { return (WhereClauseMappingCollection) this["whereClauses"]; }
            set { this["whereClauses"] = value; }
        }

        /// <summary>
        /// Class that represents a collection of select clause mapping
        /// </summary>
        public class SelectClauseMappingCollection : COEConfigurationElementCollection
        {
            /// <summary>
            /// Creates a new xml data element.
            /// </summary>
            /// <returns>The configuration element.</returns>
            protected override ConfigurationElement CreateNewElement()
            {
                return new SelectClauseMapping();
            }

            /// <summary>
            /// Gets an element by its key.
            /// </summary>
            /// <param name="element">The element with the key.</param>
            /// <returns>A string with the element's name</returns>
            protected override object GetElementKey(ConfigurationElement element)
            {
                return ((SelectClauseMapping)element).Name;
            }

            /*public override ConfigurationElementCollectionType CollectionType
            {
                get
                {
                    return ConfigurationElementCollectionType.AddRemoveClearMap;
                }
            }*/

            /// <summary>
            /// Indexer for select clause mapping by its index
            /// </summary>
            /// <param name="index">The index</param>
            /// <returns>The <see cref="SelectClauseMapping"/></returns>
            public SelectClauseMapping this[int index]
            {
                get { return (SelectClauseMapping)base.BaseGet(index); }
                set
                {
                    if (base.BaseGet(index) != null)
                    {
                        base.BaseRemoveAt(index);
                    }
                    base.BaseAdd(index, value);
                }
            }

            /// <summary>
            /// Indexer for select clause mapping by its name
            /// </summary>
            /// <param name="index">The name</param>
            /// <returns>The <see cref="SelectClauseMapping"/></returns>
            public SelectClauseMapping this[string name]
            {
                get { return (SelectClauseMapping)base.BaseGet(name); }
            }
        }

        /// <summary>
        /// Class that maps a Select clause with its implementing class
        /// </summary>
        public class SelectClauseMapping : COEConfigurationElement
        {
            /// <summary>
            /// Select clause's name
            /// </summary>
            [ConfigurationProperty("name")]
            public string Name {
                get { return (string)this["name"]; }
                set { this["name"] = value; }
            }

            /// <summary>
            /// Parser class' name for the select clause.
            /// </summary>
            [ConfigurationProperty("parserClassName")]
            public string ParserClassName
            {
                get { return (string)this["parserClassName"]; }
                set { this["parserClassName"] = value; }
            }

            /// <summary>
            /// Assembly name for the select clause. Used only for third party implementations
            /// </summary>
            [ConfigurationProperty("assemblyName", IsRequired = false)]
            public string AssemblyName
            {
                get { return (string) this["assemblyName"]; }
                set { this["assemblyName"] = value; }
            }
        }

        /// <summary>
        /// Class that represents a collection of where clause mapping
        /// </summary>
        public class WhereClauseMappingCollection : COEConfigurationElementCollection
        {
            /// <summary>
            /// Creates a new xml data element.
            /// </summary>
            /// <returns>The configuration element.</returns>
            protected override ConfigurationElement CreateNewElement() {
                return new WhereClauseMapping();
            }

            /// <summary>
            /// Gets an element by its key.
            /// </summary>
            /// <param name="element">The element with the key.</param>
            /// <returns>A string with the element's name</returns>
            protected override object GetElementKey(ConfigurationElement element) {
                return ((WhereClauseMapping) element).Name;
            }

            /// <summary>
            /// Indexer for where clause mapping by its index
            /// </summary>
            /// <param name="index">The index</param>
            /// <returns>The <see cref="WhereClauseMapping"/></returns>
            public WhereClauseMapping this[int index] {
                get { return (WhereClauseMapping) base.BaseGet(index); }
                set {
                    if(base.BaseGet(index) != null) {
                        base.BaseRemoveAt(index);
                    }
                    base.BaseAdd(index, value);
                }
            }

            /// <summary>
            /// Indexer for where clause mapping by its name
            /// </summary>
            /// <param name="index">The name</param>
            /// <returns>The <see cref="WhereClauseMapping"/></returns>
            public WhereClauseMapping this[string name] {
                get { return (WhereClauseMapping) base.BaseGet(name); }
            }
        }

        /// <summary>
        /// Class that maps a Where clause with its implementing class
        /// </summary>
        public class WhereClauseMapping : COEConfigurationElement {
            /// <summary>
            /// Where clause's name
            /// </summary>
            [ConfigurationProperty("name")]
            public string Name {
                get { return (string) this["name"]; }
                set { this["name"] = value; }
            }

            /// <summary>
            /// Parser class' name for the where clause.
            /// </summary>
            [ConfigurationProperty("parserClassName")]
            public string ParserClassName {
                get { return (string) this["parserClassName"]; }
                set { this["parserClassName"] = value; }
            }

            /// <summary>
            /// Assembly name for the where clause. Used only for third party implementations
            /// </summary>
            [ConfigurationProperty("assemblyName", IsRequired = false)]
            public string AssemblyName {
                get { return (string) this["assemblyName"]; }
                set { this["assemblyName"] = value; }
            }

            /// <summary>
            /// Search processor class name for the where clause.
            /// </summary>
            [ConfigurationProperty("searchProcessorClassName")]
            public string SearchProcessorClassName {
                get { return (string) this["searchProcessorClassName"]; }
                set { this["searchProcessorClassName"] = value; }
            }
        }
    }
}
