using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.COESecurityService;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Utilitarian class for web applications.
    /// </summary>
    public static class WebUtils
    {
        /// <summary>
        /// Sets the CSLA principal based on the Http Current Context
        /// </summary>
        public static void SetCslaPrincipal()
        {
            // this ensures that the Csla.ApplicationContext.User is properly filled before each page request

            if (Csla.ApplicationContext.AuthenticationType == "Windows")
                return;

            System.Security.Principal.IPrincipal principal;
            try
            {
                principal = HttpContext.Current.Session["CslaPrincipal"] as System.Security.Principal.IPrincipal;
            }
            catch
            {
                principal = null;
            }

            if (principal == null)
            {
                CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.Logout();
               
                
            }
            else
                Csla.ApplicationContext.User = principal;


            if (Csla.ApplicationContext.GlobalContext["USER_PERSONID"] != null)
            {
                Csla.ApplicationContext.GlobalContext["USER_PERSONID"] = HttpContext.Current.Application["USER_PERSONID"];
            }

            if (Csla.ApplicationContext.GlobalContext["COEConfigPathOverride"] != null)
            {
                Csla.ApplicationContext.GlobalContext["COEConfigPathOverride"] = HttpContext.Current.Application["COEConfigPathOverride"].ToString();
            }
                   

        }

        /// <summary>
        /// Thes recontitues csla application state variables when using a web appliaton or webservice that 
        /// where you only won't to log on once at application startup.
        /// </summary>
        public static void SetCslaPrincipal_StaticLogin()
        {
            // this ensures that the Csla.ApplicationContext.User is properly filled before each page request

            if (Csla.ApplicationContext.AuthenticationType == "Windows")
                return;

            System.Security.Principal.IPrincipal principal;
            try
            {
                principal = HttpContext.Current.Application["CslaPrincipal"] as System.Security.Principal.IPrincipal;

            }
            catch
            {
                principal = null;
            }

            if (principal == null)
                CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.Logout();
            else
                Csla.ApplicationContext.User = principal;

            if (Csla.ApplicationContext.GlobalContext["USER_PERSONID"] != null)
            {
                Csla.ApplicationContext.GlobalContext["USER_PERSONID"] = HttpContext.Current.Application["USER_PERSONID"];
            }

            

        }

        /// <summary>
        /// Encodes the given token  using HttpUtility.HtmlEncode
        /// </summary>
        public static string Encode(string token)
        {
            return HttpUtility.HtmlEncode(token).ToString();
        }

        /// <summary>
        /// Decodes the given token  using HttpUtility.HtmlDecode
        /// </summary>
        public static string Decode(string token)
        {
            return HttpUtility.HtmlDecode(token).ToString();
        }

        public static void SetCslaAlternateConfigLocation()
        {
            if (Csla.ApplicationContext.GlobalContext["COEConfigPathOverride"] != null)
            {
                Csla.ApplicationContext.GlobalContext["COEConfigPathOverride"] = HttpContext.Current.Application["COEConfigPathOverride"].ToString();
            }
            else
            {
                if (ConfigurationManager.AppSettings["COEConfigPathOverride"].ToLower() == "true")
                {
                    HttpContext.Current.Application["COEConfigPathOverride"] = HttpContext.Current.Server.MapPath(@"\" + ConfigurationManager.AppSettings["AppName"]);
                    Csla.ApplicationContext.GlobalContext["COEConfigPathOverride"] = HttpContext.Current.Application["COEConfigPathOverride"].ToString();
                }
            }
        }


      
    }
}
