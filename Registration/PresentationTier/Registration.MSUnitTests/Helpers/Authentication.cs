using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace CambridgeSoft.COE.Registration.MSUnitTests.Helpers
{
    /// <summary>
    /// Helper class for Authentication
    /// </summary>
    internal class Authentication
    {
        #region Methdos

        /// <summary>
        /// Provides authentication for database-enabled unit tests.
        /// </summary>
        /// <returns></returns>
        public static bool Logon()
        {
            return CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.Login(ConfigurationManager.AppSettings["TestingUserName"], ConfigurationManager.AppSettings["TestingPassword"]);
        }

        /// <summary>
        /// Clearing current login session.
        /// </summary>
        public static void Logoff()
        {
            CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.Logout();
        }

        #endregion
    }
}
