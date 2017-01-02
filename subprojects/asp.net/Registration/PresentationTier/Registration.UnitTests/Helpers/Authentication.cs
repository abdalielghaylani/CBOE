using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Registration.UnitTests.Helpers
{
    internal class Authentication
    {
        /// <summary>
        /// Provides authentication for database-enabled unit tests.
        /// </summary>
        /// <param name="login">the user's login</param>
        /// <param name="password">the user's password</param>
        /// <returns></returns>
        public static bool Logon(string login, string password)
        {
            return CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.Login(login, password);
        }

        public static void Logoff()
        {
            CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.Logout();
        }
    }
}
