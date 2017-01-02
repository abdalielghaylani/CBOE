using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using System.Xml.Serialization;
using System.IO;


namespace CambridgeSoft.COE.Framework.COESecurityService
{
    public static class COESecurityUtilities
    {
        public static void BuildCOEGlobalsTableName(string owner, ref string globalsTableName)
        {
            if (owner.Length > 0)
            {
                globalsTableName = owner + "." + globalsTableName;
            }
            else
            {
                globalsTableName = globalsTableName;
            }
        }

        public static void BuildCOESessionTableName(string owner, ref string sessionTableName)
        {
            if (owner.Length > 0)
            {
                sessionTableName = owner + "." + sessionTableName;
            }
            else
            {
                sessionTableName = sessionTableName;
            }
        }

       

    }
}
