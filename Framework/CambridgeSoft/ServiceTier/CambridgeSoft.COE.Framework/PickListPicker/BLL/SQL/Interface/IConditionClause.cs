using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CambridgeSoft.COE.Framework.COEPickListPickerService
{
    /// <summary>
    /// Interface that defines where clause and grouping filters used in building sql query. This interface is implemented by ConditionClause which use below properties to construct whereclause, order by grouping in sql statement.
    /// </summary>
    public interface IConditionClause : ISqlBuilder, IWhereClause, IOrderByGrouping
    {

    }
}
