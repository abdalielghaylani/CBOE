using System;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries
{
    /// <summary>
    /// A wrapper for a where clause. Expose methods to modify the where clause.
    /// </summary>
    public class WhereClause
    {
        #region Variables
        /// <summary>
        /// A list of WhereClauseBase.
        /// </summary>
        protected IList<WhereClauseBase> items;

        /// <summary>
        /// By default all where clauses are separated by ands...
        /// </summary>
        protected string defaultConcatenationOperator;

        private string parameterHolder;
        private bool useParametersByName;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the WhereClause to its default values.
        /// </summary>
        public WhereClause()
        {
            this.items = new List<WhereClauseBase>();
            this.parameterHolder = ":";
            this.useParametersByName = true;
            this.defaultConcatenationOperator = "AND";
        }
        #endregion

        #region Properties
        /// <summary>
        /// The character to use in the resulting prepared statement for indicating a parameter position (i.e. the parameter holder).
        /// </summary>
        public string ParameterHolder
        {
            get
            {
                return this.parameterHolder;
            }
            set
            {
                this.parameterHolder = value;

                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i] != null && items[i].GetType() == typeof(WhereClauseItem))
                    {
                        ((WhereClauseItem)items[i]).ParameterHolder = this.parameterHolder;
                    }
                }
            }
        }

        /// <summary>
        /// Indicates the way parameters are specified: 
        ///		if true parameters are specified by name (and an ordinal is appended to te character)
        ///		otherwise parameters are specified by position (and nothing is appended)
        ///  THIS PROPERTY IS MEANT TO BE SET BY THE QUERY THAT CONTAINS THE WHERECLAUSE. THE USER SHOULD USE THAT PROPERTY.
        /// </summary>
        public bool UseParametersByName
        {
            get
            {
                return this.useParametersByName;
            }
            set
            {
                this.useParametersByName = value;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i] != null && items[i].GetType() == typeof(WhereClauseItem))
                    {
                        ((WhereClauseItem)items[i]).UseParametersByName = this.useParametersByName;
                    }
                }
            }
        }

        public IList<WhereClauseBase> Items
        {
            get { return this.items; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds an item to the end of the where clause.
        /// </summary>
        /// <param name="item">A where clause item.</param>
        public void AddItem(WhereClauseBase item)
        {
            this.items.Add(item);
        }

        /// <summary>
        /// Removes an item of the where clause.
        /// </summary>
        /// <param name="position">The item position where is located the item to remove.</param>
        public void RemoveItem(int position)
        {
            this.items.RemoveAt(position);
        }

        /// <summary>
        /// Removes an item of the where clause.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void RemoveItem(WhereClauseBase item)
        {
            this.items.Remove(item);
        }

        public List<string> GetHints()
        {
            List<string> result = new List<string>();
            foreach (WhereClauseBase item in this.items)
            {
                if (item != null && !string.IsNullOrEmpty(item.Hint) && !result.Contains(item.Hint))
                    result.Add(item.Hint);
            }
            return result;
        }

        /// <summary>
        /// Returns a string with all the items concatenated for the specified database
        /// </summary>
        /// <returns>The where clause as string</returns>
        public string ToString(DBMSType dataBaseType, List<Value> values)
        {
            StringBuilder builder = new StringBuilder(string.Empty);

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && string.IsNullOrEmpty(items[i].AggregateFunctionName))
                {
                    builder.Append(items[i].Execute(dataBaseType, values));

                    // CSBR-156203: CBV crashes when we searching by two fields, 
                    // one of them being an aggregate field.
                    // Since aggregate functions are handled in the HAVING clause
                    // It is hard to keep track of how many ANDs will be added here.
                    // It is now easier to always add the AND operator 
                    // and the remove the last AND before returning.
                    builder.Append(" " + this.defaultConcatenationOperator + " ");
                }

            }

            // Remove the last AND operator before returning
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - this.defaultConcatenationOperator.Length - 1, this.defaultConcatenationOperator.Length + 1);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Returns a string with all the items concatenated for the specified database, to be used in the HAVING clause
        /// </summary>
        /// <returns>The where clause as string</returns>
        public string GetHaving(DBMSType dataBaseType, List<Value> values)
        {
            StringBuilder builder = new StringBuilder(string.Empty);

            List<WhereClauseItem> havingItems = this.GetAggregatedWhereClauseItems();
            for (int i = 0; i < havingItems.Count; i++)
            {
                builder.Append(havingItems[i].Execute(dataBaseType, values));
                if (i + 1 < havingItems.Count && havingItems[i + 1] != null)
                {
                    builder.Append(" " + this.defaultConcatenationOperator + " ");
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Gets the aggregated where clauses from the underlying xml definition.
        /// </summary>
        /// <returns>The Where Clause Item list</returns>
        public List<WhereClauseItem> GetAggregatedWhereClauseItems()
        {
            List<WhereClauseItem> result = new List<WhereClauseItem>();

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && !string.IsNullOrEmpty(items[i].AggregateFunctionName) && items[i] is WhereClauseItem)
                {
                    result.Add(items[i] as WhereClauseItem);
                }
            }

            return result;
        }

        public WhereClause Clone()
        {
            WhereClause clause = new WhereClause();
            clause.defaultConcatenationOperator = this.defaultConcatenationOperator;
            clause.parameterHolder = this.parameterHolder;
            clause.useParametersByName = this.useParametersByName;
            ((List<WhereClauseBase>)clause.items).AddRange(this.items);
            return clause;
        }
        #endregion

    }
}
