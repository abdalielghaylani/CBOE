using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.AggregateItems;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries
{
    /// <summary>
    /// A wrapper for an order by clause. Expose methods to modify the order by clause.
    /// </summary>
	public class OrderByClause
	{
        
        #region Variables
        private List<OrderByClauseItem> items;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the order by items.
        /// </summary>
        public List<OrderByClauseItem> Items {
            get { return items; }
            set { items = value; }
        }
        #endregion

		#region Constructors
        /// <summary>
        /// Initializes its values to its default values.
        /// </summary>
		public OrderByClause() {
			this.items = new List<OrderByClauseItem>();
		}
		#endregion

		#region Methods
        /// <summary>
        /// Gets the string representation for the default DBMS (Oracle).
        /// </summary>
        /// <returns></returns>
		public string ToString(List<Value> values)
		{
			return this.ToString(DBMSType.ORACLE, values);
		}

        /// <summary>
        /// Gets the string representation for the given database.
        /// </summary>
        /// <param name="dataBaseType">The database to generate the string to.</param>
        /// <returns>The SQL string.</returns>
        public string ToString(DBMSType dataBaseType, List<Value> values)
        {
			StringBuilder builder = new StringBuilder(string.Empty);

			if(this.items.Count > 0) {
				for(int index = 0; index < this.items.Count; index++) { 
					builder.Append(this.items[index].GetDependantString(dataBaseType, values));

					if(index != this.items.Count - 1)
						builder.Append(", ");
				}
			}
			return builder.ToString();
		}

        /// <summary>
        /// Adds an order by clause item.
        /// </summary>
        /// <param name="item">The item to add.</param>
		public  void AddItem(OrderByClauseItem item) {
			this.items.Add(item);
		}

        /// <summary>
        /// Removes an item.
        /// </summary>
        /// <param name="item">The item to remove.</param>
		public void RemoveItem(OrderByClauseItem item) {
			this.items.Remove(item);
		}

        /// <summary>
        /// Removes an item.
        /// </summary>
        /// <param name="position">The position to remove at.</param>
		public void RemoveItem(int position) {
			this.items.RemoveAt(position);
		}

        public void Sort() {
            this.items.Sort();
        }

		#endregion

	}
}
