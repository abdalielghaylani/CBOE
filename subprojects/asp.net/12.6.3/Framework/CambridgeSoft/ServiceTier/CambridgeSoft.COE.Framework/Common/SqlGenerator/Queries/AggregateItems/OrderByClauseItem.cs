using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.AggregateItems
{
	/// <summary>
	/// OrderByClauseItem class wraps a selectClauseItem and uses it for generating an ordering based upong it.
	/// </summary>
	public class OrderByClauseItem : IComparable, ICloneable
	{
		#region Constructors
		/// <summary>
		/// Default Constructor
		/// </summary>
		public OrderByClauseItem() { 
		}

		/// <summary>
		/// Receives a selectClauseItem on wich the order by will be performed
		/// </summary>
		/// <param name="item">The SelectClause item to be used</param>
		public OrderByClauseItem(SelectClauseItem item) {
			this.item = item;
		}
		#endregion

		#region Variables
		private SelectClauseItem item;
        private ResultsCriteria.SortDirection sortDirection;
        private int _orderById;
		#endregion

		#region Properties
		/// <summary>
		/// Order By is performed upon a SelectClause Item, wich is specified in this property.
		/// </summary>
		public SelectClauseItem Item {
			get { return item; }
			set { item = value; }
		}

        public ResultsCriteria.SortDirection Direction
        {
            get { return sortDirection; }
            set { sortDirection = value; }
        }

        public int OrderByID {
            get { return _orderById; }
            set { _orderById = value; }
        }
		#endregion

		#region Methods
		/// <summary>
		/// returns the string representation of this single order by item, so that it's concatenated by the order by clause and inserted into the whole query
		/// </summary>
		/// <param name="dataBaseType">Supported databases: ORACLE, SQLServer, ACCESS</param>
		/// <returns>string representation of this single order by item</returns>
        public string GetDependantString(DBMSType dataBaseType, List<Value> values)
        {
			/*if(this.item.Alias != null && this.item.Alias != "")
				return this.item.Alias;
			else
				return this.item.Execute(dataBaseType);
			*/
			return this.item.Execute(dataBaseType, values) + " " + this.Direction.ToString();
		}
		#endregion

        #region IComparable Members

        public int CompareTo(object objToCompare) {
            if(objToCompare is OrderByClauseItem) {
                OrderByClauseItem item = objToCompare as OrderByClauseItem;
                return _orderById.CompareTo(item.OrderByID);
            } else
                throw new CambridgeSoft.COE.Framework.Types.Exceptions.UnsupportedDataTypeException("Cannot compare " + objToCompare.GetType() + " against " + this.GetType());
        }

        #endregion

        #region ICloneable Members
        /// <summary>
        /// Creates a copy of the OrderByClauseItem object.
        /// </summary>
        /// <returns>The cloned OrderByClauseItem object.</returns>
        public object Clone()
        {
            OrderByClauseItem other = MemberwiseClone() as OrderByClauseItem;
            // Coverity Fix CID - 10290 (from local server)
            if (other != null)
                other.item = item.Clone() as SelectClauseItem;
            return other;
        }
        #endregion
    
    }
}
