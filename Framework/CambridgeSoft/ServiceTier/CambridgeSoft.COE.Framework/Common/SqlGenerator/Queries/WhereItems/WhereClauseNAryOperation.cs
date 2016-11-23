using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems
{
    /// <summary>
    /// A representation of a N-Ary operation to be used in a where clause.
    ///     I.E.: IN operation is an N-ary operation ... Field.name = IN(field[0].value, ..., field[n].value)...
    /// </summary>
    public abstract class WhereClauseNAryOperation : WhereClauseItem
    {
        #region Properties
        /// <summary>
        /// The database field name. We recommend setting the tablename to use
        /// fully qualified names to avoid any posibility of ambiguity.
        /// </summary>
        public Field DataField {
            get {
                return this.dataField;
            }
            set {
                this.dataField = value;
            }
        }

		/// <summary>
		/// The values to compare to.
		/// </summary>
		public Value[] Values {
			get {
				return this.values;
			}
			set {
				this.values = value;
			}
		}
        #endregion

        #region Variables
        /// <summary>
        /// The database field name. We recommend setting the tablename to use
        /// fully qualified names to avoid any posibility of ambiguity.
        /// </summary>
        protected Field dataField;

		/// <summary>
		/// The values to compare to.
		/// </summary>
		protected Value[] values;
        #endregion
    }
}
