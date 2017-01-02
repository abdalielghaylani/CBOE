using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Data;

namespace CambridgeSoft.COE.Framework.COEPickListPickerService
{
    /// <summary>
    /// Class for building sql statements in PickListDomain tables.
    /// </summary>
    public class QueryBuilder : BusinessBase<QueryBuilder>, ISqlBuilder
    {

        #region Variables PRIVATE
        private int _id;
        private ISelectClause _selectClause;
        private IWhereClause _whereClause;
        private IOrderByGrouping _orderByGrouping;
        private string _constructedQuery = string.Empty;
        #endregion
        
        #region Properties PUBLIC
        public ISelectClause SelectClause
        {
            get
            {

                return _selectClause;
            }
            set
            {
                _selectClause = value;
            }
        }

        public IWhereClause WhereClause
        {
            get
            {

                return _whereClause;
            }
            set
            {
                _whereClause = value;
            }
        }

        public IOrderByGrouping OrderByGrouping
        {
            get
            {

                return _orderByGrouping;
            }
            set
            {
                _orderByGrouping = value;
            }
        }

        /// <summary>
        /// Implements ISqlBuilder
        /// </summary>
        public string GetQuery
        {
            get
            {
                this.BuildQuery();
                return _constructedQuery;
            }
        }
        #endregion

        #region Methods PRIVATE
        protected override object GetIdValue()
        {
            return _id;
        }

        /// <summary>
        /// Build sql query from selectclause, whereclause.
        /// </summary>
        private void BuildQuery()
        {
            _constructedQuery = this.SelectClause.GetQuery
                 + this.WhereClause.GetQuery;
        }
        #endregion

        #region Constructors PRIVATE
        private QueryBuilder(ISelectClause selectClause, IConditionClause conditionClause)
        {
            _selectClause = new SelectClause(selectClause);

            _whereClause = new WhereClause(conditionClause);

            _orderByGrouping = new OrderByGrouping(conditionClause);

            // ORDER BY Conditions to be included in select clause columns
            foreach (string column in _orderByGrouping.OrderClauseList)
            {
               if(!_selectClause.Columns.Contains(column.Trim(), StringComparer.OrdinalIgnoreCase))
                    _selectClause.Columns.Add(column.Trim());
            }
        }
        #endregion

        #region Constructors PUBLIC
        public QueryBuilder(IPicklistService picklistService)
            : this((ISelectClause)picklistService, (IConditionClause)picklistService)
        {
        }
        #endregion

    }
}
