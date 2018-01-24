using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// This class serve to define any configuration data needed by the search service. 
    /// Properties can be added as needed. Each peopery will be added to the add element in the searchserviceData parent element
    /// <code lang="Xml">
    ///   &lt;searchService&gt;
    ///     &lt;add add saveQueryHistory="NO" returnPartialHitlist="YES" partialHitlistCommitSize="1500" partialHitlistFirstCommitSize="100" /&gt;
    ///   &lt;/searchService&gt;
    /// </code>
    /// </summary>
    //&lt;add name="aName" returnPartialHitlistProperty="example"  partialHitlistCommitSizeProperty=""  partialHitlistFirstCommitSizeProperty=""/&gt;
    public class SearchServiceData : COENamedElementCollection<NameValueConfigurationElement>
    {
        private const string saveQueryHistoryProperty = "saveQueryHistory";
        private const string returnPartialHitlistProperty = "returnPartialHitlist";
        private const string partialHitlistCommitSizeProperty = "partialHitlistCommitSize";
        private const string partialHitlistFirstCommitSizeProperty = "partialHitlistFirstCommitSize";
        private const string maxRecordCountProperty = "maxRecordCount";
        private const string useRealTableNames = "useRealTableNames";

        /// <summary>
        /// Initialize a new instance of the <see cref="SearchServiceData"/> class.
        /// </summary>
        public SearchServiceData()
        {
        }

        /// <summary>
        /// Indicates if the query history should be stored in database.
        /// </summary>
        public string SaveQueryHistory
        {
            get
            {
                if (this.Get(saveQueryHistoryProperty) != null)
                    return this.Get(saveQueryHistoryProperty).Value;
                else
                    return string.Empty;
            }
            set { this.Get(saveQueryHistoryProperty).Value = value; }
        }

        /// <summary>
        /// Indicates if fast, partial searches are going to be used
        /// </summary>
        public string ReturnPartialHitlist
        {
            get
            {
                if (this.Get(returnPartialHitlistProperty) != null)
                    return this.Get(returnPartialHitlistProperty).Value;
                else
                    return string.Empty;
            }
            set { this.Get(returnPartialHitlistProperty).Value = value; }
        }

        /// <summary>
        /// Indicates which would be the commit size when using partial hits.
        /// </summary>
        public int PartialHitlistCommitSize
        {
            get
            {
                if (this.Get(partialHitlistCommitSizeProperty) != null && !string.IsNullOrEmpty(this.Get(partialHitlistCommitSizeProperty).Value))
                    return int.Parse(this.Get(partialHitlistCommitSizeProperty).Value);
                else
                    return -1;
            }
            set { this.Get(partialHitlistCommitSizeProperty).Value = value.ToString(); }
        }

        /// <summary>
        /// Indicates which would be the commit size for the FIRST bunch of hits when using partial hits.
        /// </summary>
        public int PartialHitlistFirstCommitSize
        {
            get
            {
                if (this.Get(partialHitlistFirstCommitSizeProperty) != null && !string.IsNullOrEmpty(this.Get(partialHitlistFirstCommitSizeProperty).Value))
                    return int.Parse(this.Get(partialHitlistFirstCommitSizeProperty).Value);
                else
                    return -1;
            }

            set { this.Get(partialHitlistFirstCommitSizeProperty).Value = value.ToString(); }
        }

        /// <summary>
        /// Indicates which would be the maximmum hits size to be searched.
        /// </summary>
        public int MaxRecordCount
        {
            get
            {
                if (this.Get(maxRecordCountProperty) != null && !string.IsNullOrEmpty(this.Get(maxRecordCountProperty).Value))
                    return int.Parse(this.Get(maxRecordCountProperty).Value);
                else
                    return -1;
            }
            set { this.Get(maxRecordCountProperty).Value = value.ToString(); }
        }

        /// <summary>
        /// Indicates if real table names would be returned in DataTables. By default it is false, so returning table names in the form "Table_Tableid"
        /// </summary>
        /// <value>Indicates if real table names would be returned in DataTables. Default value is false.</value>
        [System.ComponentModel.DefaultValue("")]
        public string UseRealTableNames
        {
            get
            {
                if (this.Get(useRealTableNames) != null)
                    return this.Get(useRealTableNames).Value;
                else
                    return string.Empty;
            }
            set { this.Get(useRealTableNames).Value = value; }
        }
    }
}

