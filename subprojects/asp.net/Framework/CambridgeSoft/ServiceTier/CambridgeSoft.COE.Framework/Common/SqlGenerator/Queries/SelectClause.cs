using System;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries
{
    /// <summary>
    /// A wrapper for a select clause. Expose methods to modify the select clause.
    /// </summary>
    public class SelectClause
    {
        
        #region Variables
        /// <summary>
        /// A list of SelectClauseItems.
        /// </summary>
        protected IList<SelectClauseItem> items;

        /// <summary>
        /// By default all where clauses are separated by commas...
        /// </summary>
        protected string defaultConcatenationLiteral = ",";
        #endregion

		#region Properties
        /// <summary>
        /// Gets or sets the list of Select Clause Items.
        /// </summary>
		public IList<SelectClauseItem> Items {
			get { return items; }
            set { this.items = value; }
		}
		#endregion

		#region Constructors
		/// <summary>
        /// Initializes the WhereClause to its default values.
        /// </summary>
        public SelectClause() {
            this.items = new List<SelectClauseItem>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds an item to the end of the select clause.
        /// </summary>
        public void AddItem(SelectClauseItem item) {
            if(!this.items.Contains(item))
                this.items.Add(item);
        }
        public void AddItem(SelectClauseItem item, int index)
        {
            this.items.Insert(index, item);
        }

        /// <summary>
        /// Removes an item of the select clause.
        /// </summary>
        /// <param name="position">The item position where is located the item to remove.</param>
        public void RemoveItem(int position) {
            this.items.RemoveAt(position);
        }

        /// <summary>
        /// Removes an item of the select clause.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void RemoveItem(SelectClauseItem item) {
            this.items.Remove(item);
        }

        /// <summary>
        /// Returns a string with all the items concatenated for the database Oracle 10g
        /// </summary>
        /// <returns>The select clause as string.</returns>
        public string ToString(List<Value> values) {
            return this.ToString(DBMSType.ORACLE, values);
        }

        /// <summary>
        /// Returns a string with all the items concatenated for the specified database.
        /// </summary>
		/// <param name="dataBaseType">the underlying database.</param>
        /// <returns>The select clause as string.</returns>
        public string ToString(DBMSType dataBaseType, List<Value> values)
        {
            StringBuilder builder = new StringBuilder();
            string doubleQuote = @"""";
            for(int i = 0; i < items.Count; i++) {
                
                 if (items[i] != null && items[i].Visible)
                 {
                     builder.Append(items[i].Execute(dataBaseType, values));

                     if (items[i].Alias != null && items[i].Alias.Trim() != "")
                     {
                         builder.AppendFormat(" AS {0}", doubleQuote + items[i].Alias + doubleQuote);
                     }
                     // Always add a comma and then remove the last one
                     // This is less expensive than previous approach of 
                     // checking to not add the comma to the last item
                     builder.Append(", ");    
                 }
            }
            // Remove the last comma before return
            return builder.ToString(0, builder.Length-2);
        }
        #endregion
    }
}
