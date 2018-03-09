using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.UnitTests.Helpers
{
    internal class Authentication
    {
        /// <summary>
        /// Provides authentication for database-enabled unit tests.
        /// </summary>
        /// <returns></returns>
        public static bool Logon()
        {
            return CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.Login(ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"]);
        }

        /// <summary>
        /// Clearing current login session.
        /// </summary>
        public static void Logoff()
        {
            CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.Logout();
        }

    }

    public class LoginBase
    {
        public LoginBase()
        {
            CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.Logout();
            System.Security.Principal.IPrincipal user = Csla.ApplicationContext.User;
            bool result = CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.Login(ConfigurationManager.AppSettings["LogonUserName"], ConfigurationManager.AppSettings["LogonPassword"]);
        }
    }
}
