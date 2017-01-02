using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    [Serializable]
    //this class is for convenience since we are not clear where the user is coming from yet.
    public static class COEAppName
    {
        public static string Get(){
            if (Csla.ApplicationContext.GlobalContext["AppName"]==null)
            {
                try{
                    if (ConfigurationManager.AppSettings.Get("AppName")!=null){
                        Csla.ApplicationContext.GlobalContext["AppName"] = ConfigurationManager.AppSettings.Get("AppName");
                    }else{
                        Csla.ApplicationContext.GlobalContext["AppName"] = string.Empty;

                    }
                }catch(System.Exception e){
                    Csla.ApplicationContext.GlobalContext["AppName"] = string.Empty;
                }

            }
            return Csla.ApplicationContext.GlobalContext["AppName"].ToString();
        }

      
    }
}
