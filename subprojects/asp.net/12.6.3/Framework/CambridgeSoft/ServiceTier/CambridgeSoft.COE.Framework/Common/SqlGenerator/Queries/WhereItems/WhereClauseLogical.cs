using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems
{
    /// <summary>
    /// </summary>
    public class WhereClauseLogical : WhereClauseBase
    {
        #region Constructors
        /// <summary>
        /// </summary>
        public WhereClauseLogical()
        {
            logicalOperator = SearchCriteria.COELogicalOperators.And;
            items = new List<WhereClauseBase>();
        }
        #endregion

        #region Variables
        /// <summary>
        /// </summary>
        private SearchCriteria.COELogicalOperators logicalOperator;

        /// <summary>
        /// </summary>
        private List<WhereClauseBase> items;
        #endregion

        #region Properties
        /// <summary>
        /// </summary>
        public List<WhereClauseBase> Items
        {
            get { return items; }
            set { items = value; }
        }

        /// <summary>
        /// </summary>
        public SearchCriteria.COELogicalOperators LogicalOperator
        {
            get { return logicalOperator; }
            set { logicalOperator = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// </summary>
        /// <param name="dataBaseType"></param>
        /// <param name="queryValues"></param>
        /// <returns></returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> queryValues)
        {
            StringBuilder builder = new System.Text.StringBuilder(string.Empty);
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null)
                {
                    builder.Append(items[i].Execute(dataBaseType, queryValues));
                    if (i + 1 < items.Count && items[i + 1] != null)
                    {
                        builder.Append(" " + this.logicalOperator + " ");
                    }
                }
            }
            return "(" + builder.ToString() + ")";
        }
        #endregion
    }
}
