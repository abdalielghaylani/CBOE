using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CambridgeSoft.COE.Framework.COEPickListPickerService
{
    /// <summary>
    /// Interface that defines sql query properties. This class is implemented by QueryBuilder, SelectClause, ConditionClause which use below property to return constructed sql.
    /// </summary>
   public interface ISqlBuilder
    {
        /// <summary>
        /// Constrcted sql
        /// </summary>
       string GetQuery { get; }

    }
}
