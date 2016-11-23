using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CambridgeSoft.COE.Framework.COEPickListPickerService
{
    /// <summary>
    /// Interface that defines  grouping filters used in building sql query. This interface is implemented by OrderByGrouping which use below properties to construct order by grouping in sql statement.
    /// </summary>
    public interface IOrderByGrouping : ISqlBuilder
    {

        List<string> OrderClauseList
        {
            get;
            set;
        }

    }
}
