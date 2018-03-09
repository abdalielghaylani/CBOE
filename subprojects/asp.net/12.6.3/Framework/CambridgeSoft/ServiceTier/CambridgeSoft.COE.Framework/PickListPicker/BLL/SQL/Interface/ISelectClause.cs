using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CambridgeSoft.COE.Framework.COEPickListPickerService
{
    /// <summary>
    /// Interface that defines table and its columns used in building select statement. This interface is implemented by SelectClause which use below properties to return constructed select sql statement.
    /// </summary>
  public interface ISelectClause : ISqlBuilder
    {

        /// <summary>
        /// Identifier of the column
        /// </summary>
      List<string> Columns
      {
          get;
          set;
      }

        /// <summary>
        /// Source owner of the column
        /// </summary>
        string Table
        {
            get;
            set;
        }
        
    }
}
