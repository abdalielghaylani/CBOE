using System;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;
using System.Text;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.AggregateItems;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Types.Exceptions;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries
{
    /// <summary>
    /// The main class to represent a portable select statement.
    /// </summary>
    public class Query : ISource, ICloneable
    {
        
        #region Properties
        /// <summary>
        /// To generate specific sql statements for each database, this property should be used.
        /// The list of supported database can be found in:
        ///     <see cref="CambridgeSoft.COE.Framework.Common.DBMSType"/>
        /// </summary>
        public DBMSType DataBaseType {
            get {
                return this.dataBaseType;
            }
            set {
                this.dataBaseType = value;
            }
        }

        /// <summary>
        /// the TableID for the main table in the query
        /// </summary>
        public int MainTableID
        {
            get
            {
                return this.mainTableID;
            }
            set
            {
                this.mainTableID = value;
            }
        }

		/// <summary>
		/// Gets or sets the parameter values to use in prepared statements.
		/// </summary>
		public  List<Value> ParamValues {
			get {
				return this._paramValues;
			}
			set {
                this._paramValues = value;
			}
		}

		/// <summary>
		/// The character to use in the resulting prepared statement for indicating a parameter position (i.e. the parameter holder).
		/// </summary>
		public string ParameterHolder {
			get {
				return this.parameterHolder;
			}
			set {
				this.parameterHolder = value;
				this.where.UseParametersByName = this.useParametersByName;
			}
		}

		/// <summary>
		/// Indicates the way parameters are specified: 
		///		if true parameters are specified by name (and an ordinal is appended to te character. ie ':1')
		///		otherwise parameters are specified by position (and nothing is appended. ie ':')
		/// </summary>
		public bool UseParametersByName {
			get {
				return this.useParametersByName;
			}
			set {
				this.useParametersByName = value;
				this.where.UseParametersByName = this.useParametersByName;
			}
		}

        /// <summary>
        /// Determines how many records you want to retrive. If set to a value less equal than zero no restriction is applied.
        /// Its Default value is -1.
        /// </summary>
        public int Top {
            get {
                return this.top;
            }
            set {
                this.top = value;
            }
        }

        /// <summary>
        /// Nested queries may have an alias, which is set here.
        /// </summary>
        public string Alias {
            get {
                return this._alias;
            }
            set {
                this._alias = value;
            }
        }

		/// <summary>
		/// Determines whether enclose the resulting query in parenthesis or not. Default = true.
		/// </summary>
		public bool EncloseInParenthesis {
			get { return encloseInParenthesis; }
			set { encloseInParenthesis = value; }
		}

		/// <summary>
		/// Determines whether produce a prepared statment (with parameter holders and actual parameters in the paramValues List), or a plain query.
		/// </summary>
		public bool UseParameters {
			get { return useParameters; }
			set { useParameters = value; }
		}

        public List<string> Hints {
            get {
                return _hints;
            }
            set {
                _hints = value;
            }
        }
        #endregion

        #region Variables
		/// <summary>
        /// The underlying database.
        /// </summary>
		private DBMSType dataBaseType;

        /// <summary>
        /// The where part of a select statement.
        /// </summary>
        private WhereClause where;

        /// <summary>
        /// The returning fields/functions of a select statement.
        /// </summary>
        private SelectClause select;

        /// <summary>
        /// The join needed plus the from part of a select statement.
        /// </summary>
        private JoinClause join;

		private OrderByClause orderby;

        /// <summary>
        /// 
        /// </summary>
        private GroupByClause groupby;

		private string parameterHolder;
		private bool useParametersByName;
		private bool encloseInParenthesis;
		private bool useParameters;
        private int top;
        private int mainTableID;
        private string _alias;
        private List<Value> _paramValues;

        private List<string> _hints;

        #endregion

        #region Constructors
		/// <summary>
		/// Initializes its members to its default values.
		/// </summary>
		public Query() {
			this.where = new WhereClause();
			this.select = new SelectClause();
			this.join = new JoinClause();
			this.orderby = new OrderByClause();
			this.dataBaseType = DBMSType.ORACLE;
            this.top = -1;
            this._alias = string.Empty;
			this.encloseInParenthesis = true;
			this.useParameters = true;
			this.ParameterHolder = ":";
			this.UseParametersByName = true;
			this._paramValues = null;
            _hints = new List<string>();
		}
        #endregion

        #region Methods
        /// <summary>
        /// Adds a where item to the query.
        /// </summary>
        /// <param name="item">The WhereClauseItem to add.</param>
        public void AddWhereItem(WhereClauseItem item) {
			item.UseParametersByName = this.useParametersByName;
			item.ParameterHolder = this.parameterHolder;

            this.where.AddItem(item);
        }
        
        /// <summary>
        /// Removes a where item from the query.
        /// </summary>
        /// <param name="item">The WhereClauseItem to remove.</param>
        public void RemoveWhereItem(WhereClauseItem item) {
            this.where.RemoveItem(item);
        }
        
        /// <summary>
        /// Removes a where item from the query.
        /// </summary>
        /// <param name="position">The position of the WhereClauseItem to remove.</param>
        public void RemoveWhereItem(int position) {
            this.where.RemoveItem(position);
        }

        /// <summary>
        /// Remove the where clause
        /// </summary>
        public void RemoveWhereClause() {
            this.where = new WhereClause();
        }

        /// <summary>
        /// Adds a select item to the query.
        /// </summary>
        /// <param name="item">The SelectClauseItem to add.</param>
        public void AddSelectItem(SelectClauseItem item) {
            this.select.AddItem(item);
        }

        public void AddSelectItem(SelectClauseItem item, int index)
        {
            this.select.AddItem(item, index);
        }
        
        /// <summary>
        /// Removes a select item from the query.
        /// </summary>
        /// <param name="item">The SelectClauseItem to remove.</param>
        public void RemoveSelectItem(SelectClauseItem item) {
            this.select.RemoveItem(item);
        }
        
        /// <summary>
        /// Removes a select item from the query.
        /// </summary>
        /// <param name="position">The position of the SelectClauseItem to remove.</param>
        public void RemoveSelectItem(int position) {
            this.select.RemoveItem(position);
        }

        /// <summary>
        /// Adds a join relation to the query.
        /// </summary>
        /// <param name="item">The Relation to add.</param>
        public void AddJoinRelation(Relation item) {
            this.join.AddRelation(item);
        }

        /// <summary>
        /// Removes a join relation from the query.
        /// </summary>
        /// <param name="item">The Relation to remove.</param>
        public void RemoveJoinRelation(Relation item) {
            this.join.RemoveRelation(item);
        }

        /// <summary>
        /// Removes a join relation from the query.
        /// </summary>
        /// <param name="position">The position of the Relation to remove.</param>
        public void RemoveJoinRelation(int position) {
            this.join.RemoveRelation(position);
        }

        /// <summary>
        /// Sets the order by clause.
        /// </summary>
        /// <param name="orderByClause">The order by clause.</param>
		public void SetOrderByClause(OrderByClause orderByClause) {
			this.orderby = orderByClause;
		}

        /// <summary>
        /// Gets the list of order by clauses.
        /// </summary>
        /// <returns>The list of order by.</returns>
		public List<OrderByClauseItem> GetOrderByClause() {
			return this.orderby.Items;
		}

        /// <summary>
        /// Adds an order by clause item to the query.
        /// </summary>
        /// <param name="item">The order by clause item.</param>
		public void AddOrderByItem(OrderByClauseItem item) {
			this.orderby.AddItem(item);
		}

		/// <summary>
		/// Removes a select item from the query.
		/// </summary>
		/// <param name="item">The SelectClauseItem to remove.</param>
		public void RemoveOrderByItem(OrderByClauseItem item) {
			this.orderby.RemoveItem(item);
		}

		/// <summary>
		/// Removes a select item from the query.
		/// </summary>
		/// <param name="position">The position of the SelectClauseItem to remove.</param>
		public void RemoveOrderByItem(int position) {
			this.orderby.RemoveItem(position);
		}

        /// <summary>
        /// Adds the main table to the query. This is the one that follows the FROM.
        /// </summary>
        /// <param name="mainSource">The name of the main table.</param>
        public void SetMainTable(ISource mainSource) {
            join.MainSource = mainSource;
        }

        /// <summary>
        /// Gets the main table of the query.
        /// </summary>
        /// <returns>The main table.</returns>
		public ISource GetMainTable() {
			return join.MainSource;
		}

		/// <summary>
		/// Sets the select clause.
		/// </summary>
		/// <param name="clause">The SelectClause.</param>
		public void SetSelectClause(SelectClause clause) {
			this.select = clause;
		}

        /// <summary>
        /// Gets the list of select clauses.
        /// </summary>
        /// <returns>The list of select clauses.</returns>
        public List<SelectClauseItem> GetSelectClauseItems() {
            return (List<SelectClauseItem>) this.select.Items;
        }

		/// <summary>
		/// Sets the where clause.
		/// </summary>
		/// <param name="clause">The WhereClause</param>
		public void SetWhereClause(WhereClause clause) {
			this.where = clause;
            Hints = this.where.GetHints();
		}

        /// <summary>
        /// Gets the where clauses that forms the Having clause
        /// </summary>
        /// <returns></returns>
        public List<WhereClauseItem> GetAggregatedWhereClauseItems()
        {
            List<WhereClauseItem> result = new List<WhereClauseItem>();
            foreach (WhereClauseBase itemBase in this.where.Items)
            {
                WhereClauseItem item = itemBase as WhereClauseItem;

                if (item != null && !string.IsNullOrEmpty(item.AggregateFunctionName))
                {
                    result.Add(item);
                }
            }
            return result;
        }

		/// <summary>
		/// Sets the join clause.
		/// </summary>
		/// <param name="clause">The JoinClause</param>
		public void SetJoinClause(JoinClause clause) {
			this.join = clause;
		}

        /// <summary>
        /// Gets the Join Clause
        /// </summary>
        /// <returns></returns>
        public JoinClause GetJoinClause()
        {
            return this.join;
        }

        public void SetGroupByClause(GroupByClause groupByClause) {
            this.groupby = groupByClause;
        }

        private string ProcessQuery()
        {
            List<Value> paramValues = new List<Value>();

            return ProcessQuery(paramValues);
        }

        private string ProcessQuery(List<Value> paramValues)
        {
            StringBuilder builder = new StringBuilder(string.Empty);

            if(this.encloseInParenthesis)
                builder.Append("(");
            builder.Append("SELECT ");

            if(this.dataBaseType == DBMSType.ORACLE && _hints.Count > 0)
            {
                builder.Append("/*+ ");
                foreach(string hint in _hints)
                    builder.Append(hint + " ");
                builder.Append("*/ ");
            }

            if(top > 0 && this.dataBaseType != DBMSType.ORACLE)
            {
                builder.Append("TOP ");
                builder.Append(top);
                builder.Append(" ");
            }

            string selectString = string.Empty;

            if((selectString = select.ToString(this.dataBaseType, paramValues)).Trim().Length > 0)
                builder.Append(selectString);
            else
                builder.Append("*");

            if(!join.IsEmpty(this.dataBaseType))
            {
                builder.Append(" FROM ");
                builder.Append(join.FromToString(this.dataBaseType, paramValues));
            }

            string whereString;
            string rownumTop = string.Empty;

            if((whereString = join.WhereToString(this.dataBaseType)).Length > 0)
            {
                builder.Append(" WHERE ");
                builder.Append(whereString);
                if((whereString = where.ToString(this.dataBaseType, paramValues)).Length > 0)
                {
                    builder.Append(" AND ");
                    builder.Append(whereString);
                }
            }
            else if((whereString = where.ToString(this.dataBaseType, paramValues)).Length > 0)
            {
                builder.Append(" WHERE ");
                builder.Append(whereString);
            }

            if(top > 0 && this.dataBaseType == DBMSType.ORACLE)
            {
                if(whereString.Length > 0)
                {
                    builder.Append(" AND rownum <= ");
                }
                else
                {
                    builder.Append("rownum <= ");
                }
                builder.Append(":");
                builder.Append(paramValues.Count);
                paramValues.Add(new Value(top.ToString(), System.Data.DbType.Int32));
            }

            if(this.groupby != null)
            {
                string groupbyString = this.groupby.ToString(this.dataBaseType);
                if(groupbyString.Length > 0)
                {
                    builder.Append(" GROUP BY ");
                    builder.Append(groupbyString);
                }
            }

            string havingString;
            if ((havingString = where.GetHaving(this.dataBaseType, paramValues)).Length > 0)
            {
                builder.Append(" HAVING ");
                builder.Append(havingString);
            }

            string orderbyString = this.orderby.ToString(this.dataBaseType, paramValues);
            if(orderbyString.Length > 0)
            {
                builder.Append(" ORDER BY ");
                builder.Append(orderbyString);
            }
            if(this.encloseInParenthesis)
                builder.Append(")");

            ParamValues = paramValues;

            if(!this.useParameters)
                return this.DeparametrizedQuery(builder.ToString());
            else
                return builder.ToString();
        }

        /// <summary>
        /// It creates a deep copy of the Query object.
        /// </summary>
        /// <returns>A cloned Query object</returns>
        public object Clone()
        {
            Query clone = (Query)this.MemberwiseClone();
            clone._paramValues = new List<Value>();
            if (this._paramValues != null)
            {
                foreach (Value val in _paramValues)
                {
                    clone._paramValues.Add(new Value(val.Val, val.Type));
                }
            }

            clone.where = new WhereClause();
            if (this.where != null && this.where.Items != null)
            {
                foreach (WhereClauseBase item in this.where.Items)
                {
                    clone.where.AddItem(item.Clone() as WhereClauseBase);
                }
            }

            clone.select = new SelectClause();
            if (this.select != null && this.select.Items != null)
            {
                foreach (SelectClauseItem item in this.select.Items)
                {
                    clone.select.AddItem(item.Clone() as SelectClauseItem);
                }
            }

            clone.join = new JoinClause();
            if (this.join != null)
            {
                if (this.join.MainSource != null)
                {
                    if (this.join.MainSource is Query)
                        clone.join.MainSource = ((Query)this.join.MainSource).Clone() as Query;
                    else
                    {
                        Table oldTbl = (Table)join.MainSource;
                        clone.join.MainSource = new Table(oldTbl.TableId, oldTbl.TableName, oldTbl.Alias, oldTbl.Database);
                    }
                }
                if (this.join.Relations != null)
                {
                    foreach (Relation rel in this.join.Relations)
                    {
                        clone.join.AddRelation(rel);
                    }
                }
            }
            clone.orderby = new OrderByClause();
            if (this.orderby != null && this.orderby.Items != null)
            {
                foreach (OrderByClauseItem item in this.orderby.Items)
                {
                    clone.orderby.AddItem(item.Clone() as OrderByClauseItem);
                }
            }

            clone.groupby = new GroupByClause();
            if (this.groupby != null && this.groupby.Items != null)
            {
                foreach (GroupByClauseItem item in this.groupby.Items)
                {
                    clone.groupby.AddItem(item.Clone() as GroupByClauseItem);
                }
            }

            clone._hints = new List<string>();
            if (_hints != null)
            {
                foreach (string str in _hints)
                {
                    clone._hints.Add(str.Clone() as string);
                }
            }

            return clone;
        }
        #endregion

        #region ISource Members
        /// <summary>
        /// Gets the string of the query for the underlying database.
        /// </summary>
        /// <returns>The query string.</returns>
        public override string ToString() {
            return this.ProcessQuery();
        }

        /// <summary>
        /// Gets the string of the query for the underlying database.
        /// </summary>
        /// <returns>The query string.</returns>
        public string ToString(List<Value> paramValues)
        {
            return this.ProcessQuery(paramValues);
        }

		private string DeparametrizedQuery(string query) {
			string replacedQuery = query;

			int position = 0;
			int currentParameter = 0;
			foreach(Value currentValue in this._paramValues) {
                this.ReplaceNextParamterHolder(ref replacedQuery, currentParameter, currentValue);

				/*if((position = this.ReplaceNextParamterHolder(ref replacedQuery, position, currentParameter, currentValue)) == -1)
					throw new SQLGeneratorException(Resources.CannotReplaceParamInLiteral.Replace("&currentVal", currentValue.Val));*/
				currentParameter++;
			}

			return replacedQuery;
		}

        private void ReplaceNextParamterHolder(ref string replacedQuery, int currentParameter, Value currentValue)
        {
            string parameterHolderString = this.parameterHolder + (this.UseParametersByName ? currentParameter.ToString() : "");

            while(replacedQuery.IndexOf(parameterHolderString) > 0)
            {
                int index = replacedQuery.IndexOf(parameterHolderString);

                //if(this.UseParametersByName) {
					replacedQuery = replacedQuery.Substring(0, index) + currentValue.ToString() +
                                            (index + parameterHolderString.Length - 1 < replacedQuery.Length ?
                                            replacedQuery.Substring(index + parameterHolderString.Length, replacedQuery.Length - index - parameterHolderString.Length) :
											string.Empty);
				/*} else
                    replacedQuery = replacedQuery.Substring(0, index) + currentValue.ToString() +
                                            (index < replacedQuery.Length ?
                                            replacedQuery.Substring(index + 1, replacedQuery.Length - index - 1) :
											string.Empty);
                 */
            }
        }

		private int ReplaceNextParamterHolder(ref string verbatim, int currentPosition, int nextParamOrdinal, Value nextParamValue) {
			string parameterHolderString = this.parameterHolder + (this.UseParametersByName ? nextParamOrdinal.ToString() : "");

			int nextHolderPosition = verbatim.IndexOf(parameterHolderString, currentPosition);

			if(nextHolderPosition >= 0) {
				string originalVerbatim = verbatim;

				verbatim = originalVerbatim.Substring(0, nextHolderPosition);
				verbatim = verbatim + nextParamValue.ToString();
				if(this.UseParametersByName) {
					verbatim += (nextHolderPosition + 1 < originalVerbatim.Length ?
											originalVerbatim.Substring(nextHolderPosition + 2, originalVerbatim.Length - nextHolderPosition - 2) :
											string.Empty);
				} else
					verbatim += (nextHolderPosition < originalVerbatim.Length ?
											originalVerbatim.Substring(nextHolderPosition + 1, originalVerbatim.Length - nextHolderPosition - 1) :
											string.Empty);
			}
			return nextHolderPosition;
		}

		/// <summary>
		/// Gets the alias of the query.
		/// </summary>
		/// <returns>The alias or string.Empty.</returns>
		public string GetAlias() {
            return (this._alias == null) ? string.Empty : this._alias;
        }
        #endregion
    }
}
