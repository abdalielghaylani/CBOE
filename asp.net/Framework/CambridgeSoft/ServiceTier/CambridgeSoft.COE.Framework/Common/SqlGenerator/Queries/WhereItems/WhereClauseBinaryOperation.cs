using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems
{
    /// <summary>
    /// A representation of a binary operation to be applied in the where clause of a select.
    ///     I.E.: An equal operation is a binary operation: ... Field.name = Field.value
    /// </summary>
    public abstract class WhereClauseBinaryOperation : WhereClauseItem
    {
        #region Properties
        /// <summary>
        /// The database field name and its value. We recommend setting the tablename to use
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
		/// The value to compare to.
		/// </summary>
		public Value Val {
			get {
				return this.val;
			}
			set {
				this.val = value;
			}
		}
        #endregion

        #region Variables
        /// <summary>
        /// The database field name and its value. We recommend setting the tablename to use
        /// fully qualified names to avoid any posibility of ambiguity.
        /// </summary>
        protected Field dataField;

		/// <summary>
		/// The value to compare to.
		/// </summary>
		protected Value val;
        #endregion
    }
}
