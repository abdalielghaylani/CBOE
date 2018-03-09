using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using Csla;
using Csla.Data;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using System.Web;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COESecurityService
{
    [Serializable()]
    public class COEPrincipal : Csla.Security.BusinessPrincipalBase
    {
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESecurity");
        static COEIdentity identity = null;
        static COEConnection _coeConnection = new COEConnection();
        COEConnectionInfo _coeConnectionInfo = null;
        #region Constructor

        private COEPrincipal(IIdentity identity)
            : base(identity)
        {
        }

        #endregion

        public static bool IsAuthenticated
        {
            get 
            {
                return identity != null && identity.IsAuthenticated;
            }
        }


        public static string Token
        {
            get
            {
                if (identity != null)
                    return identity.IdentityToken;

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the coe connection information via wrapper class
        /// </summary>
        /// <exception cref="System.ArgumentNullException">identity is null</exception>
        public static COEConnectionInfo COEConnectionInfo
        {
            get
            {
                if (identity != null)
                {
                    return COEConnectionInfo.Instance(identity);
                }
                throw new ArgumentNullException("identity", "identity is null");
            }
        }

        public static bool Login(string userName, string password)
        {
            Logout(); // Important, so a BusinessPrincipalBase-based object gets created no matter what !!!
            _coeConnection = CheckCompatibility();

            try
            {

                identity = COEIdentity.GetIdentity(userName, password);
                identity.COEConnection = _coeConnection;

                if (identity.IsAuthenticated)
                {
                    identity.COEConnection.StartSession(COEUser.ID);
                    COEPrincipal principal = new COEPrincipal(identity);
                    Csla.ApplicationContext.User = principal;
                }
            }
            catch (Exception ex)
            {
                _coeLog.LogStart("LoginError", 0, System.Diagnostics.SourceLevels.Error);
                _coeLog.Log("Login Failed for user " + userName + " and password '" + password + "'. Attempt made from IP " + COEUser.CurrentIP + ".\n\n**** Exception ****\n\n" + ex.Message, 0, System.Diagnostics.SourceLevels.Error);
                _coeLog.LogEnd("LoginError", 0, System.Diagnostics.SourceLevels.Error);
                throw ex;
            }
            return identity.IsAuthenticated;
        }


        public static bool Login(string userName, bool isTicket)
        {

            try
            {
                Logout(); // Important, so a BusinessPrincipalBase-based object gets created no matter what !!!
                _coeConnection = CheckCompatibility();
                identity = COEIdentity.GetIdentity(userName, isTicket);
                identity.COEConnection = _coeConnection;
                if (identity.IsAuthenticated)
                {
                    //need to store the password with the identity
                    identity.COEConnection.StartSession(COEUser.ID);
                    COEPrincipal principal = new COEPrincipal(identity);
                    Csla.ApplicationContext.User = principal;
                }
            }
            catch (Exception ex)
            {
                _coeLog.LogStart("LoginError", 0, System.Diagnostics.SourceLevels.Error);
                _coeLog.Log("Login Failed for user " + userName + " and " + (!isTicket ? "not " : " ") + "using ticket. Attempt made from IP " + COEUser.CurrentIP + ".\n\n**** Exception ****\n\n" + ex.Message, 0, System.Diagnostics.SourceLevels.Error);
                _coeLog.LogEnd("LoginError", 0, System.Diagnostics.SourceLevels.Error);
            }
            return identity.IsAuthenticated;
        }

        public static bool Login(string authenticationTicket)
        {

            try
            {
                Logout(); // Important, so a BusinessPrincipalBase-based object gets created no matter what !!!
                identity = COEIdentity.GetIdentity(authenticationTicket);
                _coeConnection = CheckCompatibility();
                identity = COEIdentity.GetIdentity(authenticationTicket);
                identity.COEConnection = _coeConnection;
                if (identity.IsAuthenticated)
                {
                    identity.COEConnection.StartSession(COEUser.ID);
                    COEPrincipal principal = new COEPrincipal(identity);
                    Csla.ApplicationContext.User = principal;
                }
            }
            catch (Exception ex)
            {
                _coeLog.LogStart("LoginError", 0, System.Diagnostics.SourceLevels.Error);
                _coeLog.Log("Login Failed for ticket " + authenticationTicket + ". Attempt made from IP " + COEUser.CurrentIP + ".\n\n**** Exception ****\n\n" + ex.Message, 0, System.Diagnostics.SourceLevels.Error);
                _coeLog.LogEnd("LoginError", 0, System.Diagnostics.SourceLevels.Error);
                throw;
            }
            return identity.IsAuthenticated;
        }


        public static bool Login_Standalone(string userName, string password, string standAloneID)
        {

            try
            {
                // Logout();
                _coeConnection = CheckCompatibility();
                identity = COEIdentity.GetIdentity(userName, password, standAloneID);
                identity.COEConnection = _coeConnection;
                if (identity.IsAuthenticated)
                {
                    COEPrincipal principal = new COEPrincipal(identity);

                    Csla.ApplicationContext.User = principal;
                }

            }
            catch (Exception ex)
            {
                _coeLog.LogStart("LoginError", 0, System.Diagnostics.SourceLevels.Error);
                _coeLog.Log("Login Failed for user " + userName + " and password '" + password + "', with standAloneID '" + standAloneID + "'. Attempt made from IP " + COEUser.CurrentIP + ".\n\n**** Exception ****\n\n" + ex.Message, 0, System.Diagnostics.SourceLevels.Error);
                _coeLog.LogEnd("LoginError", 0, System.Diagnostics.SourceLevels.Error);
                throw;
            }
            return identity.IsAuthenticated;

        }

        public static void Logout(bool endSession = true)
        {
            try
            {
                identity = COEIdentity.UnauthenticatedIdentity();
                COEPrincipal principal = new COEPrincipal(identity);
                Csla.ApplicationContext.User = principal;
                if (endSession)
                {
                    UpdateSessionEndtime();
                }
            }
            catch (Exception ex)
            {
                _coeLog.LogStart("LogoutError", 0, System.Diagnostics.SourceLevels.Error);
                _coeLog.Log("Logout Failed for user " + COEUser.Name + ".\n\n**** Exception ****\n\n" + ex.Message, 0, System.Diagnostics.SourceLevels.Error);
                _coeLog.LogEnd("LogoutError", 0, System.Diagnostics.SourceLevels.Error);
                throw;
            }
        }
        /// <summary>
        /// This method will update the COEDB.COESESSION.ENDTIME column
        /// when user gets logoff
        /// This method is added to fix CSBR:133832
        /// </summary>
        private static void UpdateSessionEndtime()
        {
            try
            {

                if (_coeConnection!=null && _coeConnection.SessionID > 0)
                {
                    identity.COEConnection.EndSession(_coeConnection.SessionID);
                }
            }
            catch (Exception ex)
            {
                _coeLog.LogStart("LogoutError", 0, System.Diagnostics.SourceLevels.Error);
                _coeLog.Log("Logout Failed for user " + COEUser.Name + ".\n\n**** Exception ****\n\n" + ex.Message, 0, System.Diagnostics.SourceLevels.Error);
                _coeLog.LogEnd("LogoutError", 0, System.Diagnostics.SourceLevels.Error);
                throw;
            }
        }
        public override bool IsInRole(string rolePrivilege)
        {
            try
            {
                identity = (COEIdentity)this.Identity;
            }
            catch (Exception ex)
            {

                throw;
            }
            return identity.IsInRole(rolePrivilege);
        }

        /// <summary>
        /// Method to check if the current user has the privileges set in the rolePrivs param. 
        /// </summary>
        /// <param name="rolePrivileges">The list of roles to check</param>
        /// <param name="logOperator">The Logical operator to apply</param>
        /// <returns></returns>
        public static bool HasPrivilege(string rolePrivileges, string logOperator)
        {
            bool retVal = true;
            //List generally separated by a | char.
            string[] privsArray = rolePrivileges.Split(GUIShellTypes.COEPageSettingsNameSeparator.ToCharArray());
            //If the XML has just one priv, we don't care about the operator tag. In this context, is useless.
            if (string.IsNullOrEmpty(logOperator) || privsArray.Length == 1)
                logOperator = GUIShellTypes.Operators.AND.ToString();
            //If the logOperator is an OR, we should start with a false value to have a correct result.
            if (logOperator == GUIShellTypes.Operators.OR.ToString())
                retVal = false;

            foreach (string currentPrivilege in privsArray)
            {
                //Check if the current user has the current priv.
                bool userInRole = HttpContext.Current.User.IsInRole(currentPrivilege);
                //Apply Operator.
                if (logOperator.ToUpper() == GUIShellTypes.Operators.OR.ToString())
                    retVal |= userInRole;
                else if (logOperator.ToUpper() == GUIShellTypes.Operators.AND.ToString())
                    retVal &= userInRole;
            }
            return retVal;
        }

        /// <summary>
        /// Method to check if the current user has the privileges set in the rolePrivs param. 
        /// </summary>
        /// <param name="rolePrivileges">The list of roles to check</param>
        /// <param name="logOperator">The Logical operator to apply</param>
        /// <returns></returns>
        public static bool HasPrivilege(COEPageControlSettingsService.PrivilegeList rolePrivileges, COEPageControlSettingsService.PrivilegeList.Operators logOperator)
        {
            bool retVal = logOperator == CambridgeSoft.COE.Framework.COEPageControlSettingsService.PrivilegeList.Operators.OR ? false : true;
            foreach (COEPageControlSettingsService.Privilege privilege in rolePrivileges)
            {
                //Check if the current user has the current priv.
                if (HttpContext.Current.User != null)
                {
                    bool userInRole = HttpContext.Current.User.IsInRole(privilege.ID);
                    switch (logOperator)
                    {
                        case CambridgeSoft.COE.Framework.COEPageControlSettingsService.PrivilegeList.Operators.AND:
                            retVal &= userInRole; break;
                        case CambridgeSoft.COE.Framework.COEPageControlSettingsService.PrivilegeList.Operators.OR:
                            retVal |= userInRole; break;
                        default:
                            retVal |= userInRole; break;
                    }
                }
            }
            return retVal;
        }

        private static COEConnection CheckCompatibility()
        {
            //checks connection to oracle database
            //checks compatiblity between oracle schema and frameowrk
            //checks compatiblity of frameowrk version on client and server
            _coeConnection.ClientFrameworkVersion = _coeConnection.GetClientFrameworkVersion();
            try
            {
                _coeConnection.CheckFrameworkCompatibility();

                if (_coeConnection.FrameworkVersionsAreCompatible == false)
                {
                    throw new IncompatibleVersions("FRAMEWORK", _coeConnection.ClientFrameworkVersion, _coeConnection.VersionInfo.MinRequiredClientFrameworkVersion);
                }
                if (_coeConnection.OracleSchemasAreCompatible == false)
                {
                    throw new IncompatibleVersions("SCHEMA", _coeConnection.VersionInfo.ServerOracleSchemaVersion, _coeConnection.VersionInfo.MinOracleSchemaVersion);
                }
            }
            catch (Csla.DataPortalException ex)
            {
                throw;
            }
            catch (System.Exception e)
            {
                throw;
            }

            return _coeConnection;
        }

        public static string RenewToken()
        {
            try
            {
                identity = COEIdentity.RenewTicket(Token);
            }
            catch (Exception ex)
            {
                _coeLog.LogStart("LoginError", 0, System.Diagnostics.SourceLevels.Error);
                _coeLog.Log("RenewTicket with token " + Token + " failed. Attempt made from IP " + COEUser.CurrentIP + ".\n\n**** Exception ****\n\n" + ex.Message, 0, System.Diagnostics.SourceLevels.Error);
                _coeLog.LogEnd("LoginError", 0, System.Diagnostics.SourceLevels.Error);
                throw ex;
            }

            return Token;
        }

        public static bool IsValidToken()
        {
            return COEIdentity.IsValidTicket(Token);
        }
    }
}
