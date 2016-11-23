using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.AggregateItems
{
    public class GroupByClauseItem : IComparable, ICloneable
	{
		#region Constructors
		/// <summary>
		/// Default Constructor
		/// </summary>
		public GroupByClauseItem() { 
		}

		/// <summary>
		/// Receives a selectClauseItem on wich the group by will be performed
		/// </summary>
		/// <param name="item">The SelectClause item to be used</param>
        public GroupByClauseItem(SelectClauseItem item) {
			this.item = item;
		}
		#endregion

		#region Variables
		private SelectClauseItem item;
        private int _groupById;
		#endregion

		#region Properties
		/// <summary>
		/// Group By is performed upon a SelectClause Item, wich is specified in this property.
		/// </summary>
		public SelectClauseItem Item {
			get { return item; }
			set { item = value; }
		}

        /// <summary>
        /// 
        /// </summary>
        public int GroupById {
            get { return _groupById; }
            set { _groupById = value; }
        }
		#endregion

		#region Methods
		/// <summary>
		/// returns the string representation of this single group by item, so that it's concatenated by the Group by clause and inserted into the whole query
		/// </summary>
		/// <param name="dataBaseType">Supported databases: ORACLE, SQLServer, ACCESS</param>
		/// <returns>string representation of this single group by item</returns>
		public string GetDependantString(DBMSType dataBaseType) {
			return @"" + this.item.Execute(dataBaseType, new List<Value>()) + @"";
		}
		#endregion

        #region IComparable Members
        public int CompareTo(object objToCompare) {
            if(objToCompare is GroupByClauseItem) {
                GroupByClauseItem item = objToCompare as GroupByClauseItem;
                return _groupById.CompareTo(item.GroupById);
            } else
                throw new CambridgeSoft.COE.Framework.Types.Exceptions.UnsupportedDataTypeException("Cannot compare " + objToCompare.GetType() + " against " + this.GetType());
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Creates a copy of the GroupByClauseItem object.
        /// </summary>
        /// <returns>The cloned GroupByClauseItem object.</returns>
        public object Clone()
        {
            GroupByClauseItem other = MemberwiseClone() as GroupByClauseItem;
            // Coverity Fix CID - 10289 (from local server)
            if(other != null)
                other.item = item.Clone() as SelectClauseItem;
            return other;
        }
        #endregion
    
    }
}
