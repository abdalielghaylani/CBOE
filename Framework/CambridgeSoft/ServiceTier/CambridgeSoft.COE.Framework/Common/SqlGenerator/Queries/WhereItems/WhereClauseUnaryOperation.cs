using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems
{
    /// <summary>
    /// A representation of an unary operation to be applied in the where clause of a select.
    /// </summary>
    public abstract class WhereClauseUnaryOperation : WhereClauseItem
    {
        #region Properties
        /// <summary>
        /// The value to apply the operation. If a field is represented we recommend using 
        /// fully qualified names to avoid any posibility of ambiguity.
        /// </summary>
        public Value DataValue {
            get {
                return this.dataValue;
            }
            set {
                this.dataValue = value;
            }
        }
        #endregion

        #region Variables
        /// <summary>
        /// The value to apply the operation. If a field is represented we recommend using 
        /// fully qualified names to avoid any posibility of ambiguity.
        /// </summary>
        protected Value dataValue;
        #endregion
    }
}
