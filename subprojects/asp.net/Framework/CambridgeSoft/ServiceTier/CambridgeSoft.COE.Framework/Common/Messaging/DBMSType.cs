using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Enum for supported databases. Allowed values are:
    /// <list type="bullet">
    ///   <item>Oracle</item>
    ///   <item>SQL Server</item>
    ///   <item>Ms Access</item>
    /// </list>
    /// </summary>
    public enum DBMSType
    {
         ///<summary>represents an Oracle database</summary>
            ORACLE,
            ///<summary>represents an SQLService database</summary>
            SQLSERVER,
            ///<summary>represents an Microsoft Access database</summary>
            MSACCESS
    }
}
