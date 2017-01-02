using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CambridgeSoft.COE.Framework.COEPickListPickerService
{
    /// <summary>
    /// Interface that defines all properties used to build sql query which contain select clause where clause, order by grouping used in picklist service.
    /// </summary>
    public interface IPicklistService : ISelectClause , IConditionClause
    {

        #region Properties public
        
        /// <summary>
        /// The column that will contain the value members for the picklist.
        /// </summary>
        string IdColumn { get; set; }

        /// <summary>
        /// The column that will contain the display members for the picklist.
        /// </summary>
        string DisplayColumn { get; set; }

        /// <summary>
        /// Sql query where clause filter
        /// </summary>
        string WhereFilter { set; }

        /// <summary>
        /// Sql query order by filter
        /// </summary>
        string OrderByFilter { set; }
        #endregion


    }
}
