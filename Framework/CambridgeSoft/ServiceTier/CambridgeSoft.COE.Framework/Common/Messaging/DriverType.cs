using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Enum for supported driver type. Allowed values are:
    /// <list type="bullet">
    ///   <item>Oracle</item>
    ///   <item>OracleDataDirect</item>
    /// </list>
    /// </summary>
    public enum DriverType
    {
        ///<summary>represents an Oracle driver</summary>
        Oracle,
        ///<summary>represents an Oracle datadirect driver</summary>
        OracleDataDirect
    }
}
