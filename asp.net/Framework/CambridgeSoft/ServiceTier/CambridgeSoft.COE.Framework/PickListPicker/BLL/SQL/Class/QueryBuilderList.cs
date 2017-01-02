using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Data;

namespace CambridgeSoft.COE.Framework.COEPickListPickerService
{
    /// <summary>
    /// Class to collect multiple sql statements for PickListDomain tables.
    /// </summary>
    public class QueryBuilderList : BusinessListBase<QueryBuilderList, QueryBuilder>, ISqlBuilder
    {

        #region CONSTANTS PRIVATE
        private const string UNION = " UNION ";
        #endregion

        #region Variables PRIVATE
        private string _constructedQuery = string.Empty;
        #endregion

        #region Properties PUBLIC
        /// <summary>
        /// Implements ISqlBuilder
        /// </summary>
        public string GetQuery
        {
            get
            {
                BuildQuery();
                return _constructedQuery;
            }
        }
        #endregion

        #region Methods PRIVATE
        /// <summary>
        /// Build sql query looping querybuilder added to list.
        /// </summary>
        private void BuildQuery()
        {
            string sql = string.Empty;

            foreach (ISqlBuilder queryBuilder in this) // Get select and where statement
            {
                sql = (!string.IsNullOrEmpty(sql) ? (sql + UNION) : "") + queryBuilder.GetQuery;
            }

            foreach (QueryBuilder queryBuilder in this)// Get order by
            {
                if (queryBuilder.OrderByGrouping.GetQuery != string.Empty)
                {
                    sql = sql + queryBuilder.OrderByGrouping.GetQuery;
                    break;
                }
            }
             _constructedQuery = sql;
        }
        #endregion

        #region Methods PUBLIC
        internal static QueryBuilderList NewQueryBuilderList()
        {
            return new QueryBuilderList();
        }

        public void AddNewQueryBuilder(IPicklistService picklistService)
        {
            this.Add(new QueryBuilder(picklistService));
        }
        #endregion

    }
}
