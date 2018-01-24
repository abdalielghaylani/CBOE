using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.Common
{
    [Serializable]
    //this class is for convenience since we are not clear where the user is coming from yet.
    public static class COEDatabaseName
    {
        public static string Get()
        {
            if (Csla.ApplicationContext.GlobalContext["DatabaseName"] != null)
            {
                return Csla.ApplicationContext.GlobalContext["DatabaseName"].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public static void Set(string databaseName)
        {
            Csla.ApplicationContext.GlobalContext["DatabaseName"] = databaseName;
      

        }
    }
}
