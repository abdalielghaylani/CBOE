using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CambridgeSoft.COE.Framework.COEPickListPickerService
{
    /// <summary>
    /// Interface that defines where clause filters used in building sql query. This interface is implemented by WhereClause which use below properties to construct whereclause in sql statement.
    /// </summary>
    public interface IWhereClause : ISqlBuilder
    {
        List<string> WhereClauseList
        {
            get;
            set;
        }

    }
}
