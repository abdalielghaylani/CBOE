using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.AggregateItems;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries {
    
    /// <summary>
    /// 
    /// </summary>
    public class GroupByClause {
        
        #region Variables
        private List<GroupByClauseItem> items;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the group by items.
        /// </summary>
        public List<GroupByClauseItem> Items {
            get { return items; }
            set { items = value; }
        }
        #endregion

		#region Constructors
        /// <summary>
        /// Initializes its values to its default values.
        /// </summary>
        public GroupByClause() {
            this.items = new List<GroupByClauseItem>();
		}
		#endregion
        
        #region Methods
        /// <summary>
        /// Gets the string representation for the default DBMS (Oracle).
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return this.ToString(DBMSType.ORACLE);
        }

        /// <summary>
        /// Gets the string representation for the given database.
        /// </summary>
        /// <param name="dataBaseType">The database to generate the string to.</param>
        /// <returns>The SQL string.</returns>
        public string ToString(DBMSType dataBaseType) {
            StringBuilder builder = new StringBuilder(string.Empty);

            if(this.items.Count > 0) {
                for(int index = 0; index < this.items.Count; index++) {
                    builder.Append(this.items[index].GetDependantString(dataBaseType));

                    if(index != this.items.Count - 1)
                        builder.Append(", ");
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Adds an group by clause item.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddItem(GroupByClauseItem item) {
            this.items.Add(item);
        }

        #endregion
    }
}
