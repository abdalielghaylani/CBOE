using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// This class defines the any configuration data for the sql generator for each application
    /// properties can be added as needed. Each propery will be added to the add element in the sqlGeneratorData parent element
    /// <code lang="Xml">
    /// &lt;application name="SAMPLE"&gt;   
    ///     &lt;sqlGeneratorData&gt;
    ///         &lt;add cartridgeSchema="CSCARTRIDGE"/&gt;
    ///   &lt;/sqlGeneratorData&gt;
    /// &lt;/application&gt;
    /// </code>
    /// </summary>
    public class SQLGeneratorData : COENamedConfigurationElement
    {
        private const string nameProperty = "name";
        private const string SchemaProperty = "schema";
        private const string tempQueries = "tempQueriesTableName";
        private const string chemMajorVersion = "chemMajorVersion";
        private const string chemMinorVersion = "chemMinorVersion";
        private const string molFileFormat = "molFileFormat";
        private const string innerQueryAliasHint = "innerQueryAliasHint";
        private const string currentPageHint = "currentPageHint";

        /// <summary>
        /// Initialize a new instance of the <see cref="SQLGeneratorData"/> class.
		/// </summary>
        public SQLGeneratorData()
		{
		}

        /// <summary>
        /// Name of the chemistry cartridge used by application
        /// </summary>
        [ConfigurationProperty(nameProperty, IsRequired = true)]
        public string Name
        {
            get { return (string)base[nameProperty]; }
            set { base[nameProperty] = value; }
        }

        /// <summary>
        /// what is the schema/database name used by the cartridge
        /// </summary>
        [ConfigurationProperty(SchemaProperty, IsRequired = true)]
        public string Schema
        {
            get { return (string)base[SchemaProperty]; }
            set { base[SchemaProperty] = value; }
        }

        /// <summary>
        /// Name of the temporal table where the queries are stored
        /// </summary>
        [ConfigurationProperty(tempQueries, IsRequired = false)]
        public string TempQueries {
            get { return (string) base[tempQueries]; }
            set { base[tempQueries] = value; }
        }


        /// <summary>
        /// Primarily Cartridge - Major Version
        /// </summary>
        [ConfigurationProperty(chemMajorVersion, IsRequired = false)]
        public string ChemMajorVersion
        {
            get { return (string)base[chemMajorVersion]; }
            set { base[chemMajorVersion] = value; }
        }

        /// <summary>
        /// Primarily Cartridge - Minor Version
        /// </summary>
        [ConfigurationProperty(chemMinorVersion, IsRequired = false)]
        public string ChemMinorVersion
        {
            get { return (string)base[chemMinorVersion]; }
            set { base[chemMinorVersion] = value; }
        }
        /// <summary>
        /// Primarily Cartridge - Export MolFileFormat Type
        /// </summary>
        [ConfigurationProperty(molFileFormat, IsRequired = false)]
        public string MolFileFormat
        {
            get { return (string)base[molFileFormat]; }
            set { base[molFileFormat] = value; }
        }

        /// <summary>
        /// SQLGenerator  - Inject a hint into the ParentQuery outer select clause
        /// usage in COEFrameworkConfig.xml: add  innerQueryAliasHint="NO_MERGE" attribute
        /// to sqlGeneratorData tag
        /// </summary>
        [ConfigurationProperty(innerQueryAliasHint, IsRequired = false)]
        public string InnerQueryAliasHint
        {
            get { return (string)base[innerQueryAliasHint]; }
            set { base[innerQueryAliasHint] = value; }
        }

        /// <summary>
        /// SQLGenerator  - Inject a hint into Queries involving currentPage.
        /// Defaults in the code to OPT_ESTIMATE(table, \"currentPage\", rows=100)
        /// Can be overridden in COEFrameworkConfig.xml: add  currentPageHint="YOUR HINT" attribute
        /// to sqlGeneratorData tag. It can be the same hint provided with a different estimate.  
        /// Will effect all queries.
        /// </summary>
        [ConfigurationProperty(currentPageHint, IsRequired = false)]
        public string CurrentPageHint
        {
            get { return (string)base[currentPageHint]; }
            set { base[currentPageHint] = value; }
        }
        
    }
}

