using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace CambridgeSoft.COE.Framework.COEPickListPickerService
{/// <summary>
    /// Class for building selectclause used in PickListDomain tables.
    /// </summary>
    public class SelectClause : ISelectClause
    {
       
        #region CONSTANTS PRIVATE
        private const string SELECT = " SELECT ";
        private const string FROM = " FROM ";
        private const string ALIAS = " AS ";
        #endregion

        #region Variables PRIVATE
        private List<string> _columns;
        private string _table = string.Empty;
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
                this.BuildQuery();
                return _constructedQuery;
            }
        }

        /// <summary>
        /// Identifier of the database column
        /// </summary>
        public List<string> Columns
        {
            get
            {
                if (_columns == null)
                    _columns = new List<string>();
                return _columns;
            }
            set
            {
                _columns = value;
            }
        }

        /// <summary>
        /// Source owner of the column
        /// </summary>
        public string Table
        {
            get { return _table; }
            set { _table = value; }
        }
        #endregion

        #region Methods PRIVATE
        /// <summary>
        /// Build select statement query  
        /// </summary>
        private void BuildQuery()
        {
            List<string> query = new List<string>();
            foreach (string column in _columns)
            {
                query.Add(column);
            }
            _constructedQuery = SELECT + string.Join(",", query.ToArray()) + FROM + Table;
        }
        #endregion
        
        #region Constructors PRIVATE
        private SelectClause(string tableName, List<string> columns)
        {
            this.Table = tableName;
            this.Columns = columns;
        }
        #endregion

        #region Constructors PUBLIC
        public SelectClause(ISelectClause selectClause)
            : this(selectClause.Table, selectClause.Columns)
        {

        }
        #endregion

    }
}
