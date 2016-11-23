using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Enumerate allowed connectrion string types. Allowed values are:
    /// <list type="bullet">
    ///   <item>Proxy: Schema owner that uses logged in user for permissions</item>
    ///   <item>Owner: Schema owner</item>
    /// </list>
    /// </summary>
    public enum ConnStringType
    {
        ///<summary>Schema owner that uses logged in user for permissions</summary>
        PROXY,
        ///<summary>schema owner</summary>
        OWNER,
        ///<summary>schema owner using proxy</summary>
        OWNERPROXY
    }
}
