using System;
using System.Reflection;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Security.Principal;
using Csla;
using Csla.Data;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Properties;
using System.Web.Services.Protocols;


namespace CambridgeSoft.COE.Framework.COESecurityService
{
    [Serializable()]
    public class COEIdentity : ReadOnlyBase<COEIdentity>, IIdentity
    {
        #region Constructors

        private COEIdentity() {

        }

      

      
        
        #endregion

        #region Member variables
        private  COEConnection _coeConnection = new COEConnection();
        private bool _isAuthenticated = false;
        private bool _isValidTicket = false;
        private string _userName = string.Empty;
        private int _id = 0;
        protected string _password = string.Empty;
        protected bool _isLDAP = false;
        protected bool _isExempt = true;
        protected string _identityToken = string.Empty;
        private Dictionary<string, List<string>> _appRolePrivileges = new Dictionary<string, List<string>>();
        [NonSerialized()]
        private DAL _securityServiceDAL = null;
        [NonSerialized()]
        private DALFactory _dalFactory = new DALFactory();
        private string _databaseName = string.Empty;
        private string _serviceName = "COESecurity";
        [NonSerialized]
         static COELog _coeLog = COELog.GetSingleton("COESecurity");


        #endregion

        #region IIdentity Members

        public string AuthenticationType
        {
            get
            {
                return "Csla";
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return _isAuthenticated;
            }
        }

        public string Name
        {
            get
            {
                return _userName;
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
        }

        /// <summary>
        /// Gets the ID of the current logged user.
        /// </summary>
        /// <value>The unique ID.</value>
        public int ID
        {
            get
            {
                return this._id;
            }
        }

        public bool IsLDAP
        {
            get
            {
                return _isLDAP;
            }
            set
            {
                _isLDAP = value;
            }
        }

        public bool IsExempt
        {
            get
            {
                return _isExempt;
            }
            set
            {
                _isExempt = value;
            }
        }

        public COEConnection COEConnection
        {
            get
            {
                return _coeConnection;
            }
            set
            {
                _coeConnection = value;
            }
        }

        public string IdentityToken
        {
            get
            {
                return _identityToken;
            }
            set
            {
                _identityToken = value;
            }
        }
        
        #endregion

        #region Business base requirements

        protected override object GetIdValue()
        {
            return _userName;
        }

        #endregion

        #region Misc methods

        internal bool IsInRole(string rolePrivilege)
        {
            foreach(KeyValuePair<string, List<string>> appPrivilege in _appRolePrivileges)
            {
                if(appPrivilege.Value.Contains(rolePrivilege))
                    return true;
            }
            return false;
        }

        public bool HasAppPrivilege(string coeIdentifier, string privilegeName)
        {
            return _appRolePrivileges.ContainsKey(coeIdentifier) && _appRolePrivileges[coeIdentifier].Contains(privilegeName);
        }


        #endregion

        #region Factory Methods

        internal static COEIdentity UnauthenticatedIdentity()
        {
            return new COEIdentity();
        }

        internal static COEIdentity GetIdentity(string userName, string password)
        {
            
                return DataPortal.Fetch<COEIdentity>(new Criteria(userName, password));
            
        }

    

        internal static COEIdentity GetIdentity(string value, bool isTicket)
        {
            if (isTicket)
            {
                return DataPortal.Fetch<COEIdentity>(new TicketCriteria(value));
            }
            else
            {
                return DataPortal.Fetch<COEIdentity>(new PrivilegeCriteria(value));

            }

        }

        internal static COEIdentity GetIdentity(string authenticationTicket) {

            return DataPortal.Fetch<COEIdentity>(new TicketCriteria(authenticationTicket));
        }

        internal static COEIdentity GetIdentity(string userName, string password, string standaloneID)
        {
            return DataPortal.Fetch<COEIdentity>(new StandAloneCriteria(userName, password, standaloneID));
        }

        internal static COEIdentity RenewTicket(string authenticationTicket)
        {
            return DataPortal.Fetch<COEIdentity>(new RenewTicketCriteria(authenticationTicket));
        }

        internal static bool IsValidTicket(string authenticationTicket)
        {
            return DataPortal.Fetch<COEIdentity>(new ValidateTicketCriteria(authenticationTicket))._isValidTicket;
        }
        #endregion

        #region Criteria classes

        [Serializable()]
        private class Criteria
        {

            public Criteria(string userName, string password)
            {
                this.userName = userName;
                this.password = password;
                
            }
            
            private string userName;
            private string password;

           

            public string Name
            {
                get
                {
                    return this.userName;
                }
            }

            public string Password
            {
                get
                {
                    return this.password;
                }
                set
                {
                    this.password = value;
                }
            }
        }

        [Serializable()]
        private class TicketCriteria {

            public TicketCriteria(string authenticationTicket) {
                this.authenticationTicket = authenticationTicket;

            }

            private string authenticationTicket;


            public string AuthenticationTicket {
                get {
                    return this.authenticationTicket;
                }
                set {
                    this.authenticationTicket = value;
                }
            }
        }

        [Serializable()]
        private class ValidateTicketCriteria
        {
            public ValidateTicketCriteria(string authenticationTicket)
            {
                this.authenticationTicket = authenticationTicket;
            }

            private string authenticationTicket;
            public string AuthenticationTicket
            {
                get
                {
                    return this.authenticationTicket;
                }
                set
                {
                    this.authenticationTicket = value;
                }
            }
        }


        [Serializable()]
        private class RenewTicketCriteria
        {
            public RenewTicketCriteria(string authenticationTicket)
            {
                this.authenticationTicket = authenticationTicket;
            }

            private string authenticationTicket;
            public string AuthenticationTicket
            {
                get
                {
                    return this.authenticationTicket;
                }
                set
                {
                    this.authenticationTicket = value;
                }
            }
        }

        [Serializable()]
        private class PrivilegeCriteria
        {

            public PrivilegeCriteria(string userName)
            {
                this.userName = userName;

            }

            private string userName;

            

            public string UserName
            {
                get
                {
                    return this.userName;
                }
                set
                {
                    this.userName = value;
                }
            }
        }

        
        [Serializable()]
        internal class StandAloneCriteria
        {

            public StandAloneCriteria(string userName, string passWord, string standAloneUserID)
            {
                this._userName = userName;
                this._passWord = passWord;
                this._standAloneID = standAloneUserID;

            }

            
            internal string _userName;
            internal string _passWord;
            internal string _standAloneID;



            
        }
        #endregion

        #region Data access


        private void DataPortal_Fetch(Criteria criteria)
        {

            _userName = criteria.Name;
            _password = criteria.Password;
            string authTicket = string.Empty;
            string authTicketResult = string.Empty;
            try
            {
                coesinglesignon.SingleSignOn singleSignOn = new coesinglesignon.SingleSignOn();
                string url = ConfigurationUtilities.GetSingleSignOnURL();

                if (url != string.Empty)
                {
                    singleSignOn.Url = url;
                    authTicket = singleSignOn.GetAuthenticationTicket(_userName, _password).ToString();

                    if (authTicket == "INVALID_USER")
                    {
                        authTicketResult = Resources.InvalidCredentialsSingleSignOn;
                    }
                    else
                    {
                        //get information about LDAP and exempt users status
                        string authType = singleSignOn.GetDefaultAuthenticationProvider();
                        if (authType.ToUpper() != "CSSECURITY")
                        {
                            _isLDAP = true;
                            _isExempt = singleSignOn.IsExemptUser(_userName);
                        }
                        _identityToken = authTicket;

                        this.LoadDAL();
                        this._id = _securityServiceDAL.GetPersonID(_userName);
                    }
                }
                else
                {
                    authTicket = "INVALID_USER";
                    authTicketResult = Resources.InvalidCredentialsSingleSignOn;
                }

            }
            catch (SoapException sex)
            {
                Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"] = sex.Detail.InnerText;
                throw new InvalidCredentials(sex.Detail.InnerText);
            }
            catch (System.Net.WebException webex)
            {
                if (webex.Status == System.Net.WebExceptionStatus.NameResolutionFailure)
                {
                    Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"] = Resources.ErrorServerName;
                    throw new InvalidCredentials(Resources.ErrorServerName);
                }
            }
            catch (Exception ex)
            {
                Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"] = Resources.ErrorRetrievingAuthTicket;
                throw new InvalidCredentials(Resources.ErrorRetrievingAuthTicket);
            }
            if (authTicketResult != string.Empty)
            {
                Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"] = authTicketResult;
                throw new InvalidCredentials(authTicketResult);
            }
            if (authTicket != "INVALID_USER"){
                _isAuthenticated = true;
                Csla.ApplicationContext.GlobalContext["TEMPUSERNAME"] = _userName;
                //we now require that cs_security is always used
                PopulateRolePrivileges(_userName);
                    
            }
            else
            {

                _identityToken = string.Empty;
                _userName = string.Empty;
                _isAuthenticated = false;
                _appRolePrivileges.Clear();
                Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"] = "Login failed getting privileges";
                throw new Exception("Login failed getting privileges");

            }
           
        }


       //this is only called when a user is already authenticated and only needs privileges
        private void DataPortal_Fetch(PrivilegeCriteria criteria)
        {
            
            _userName=criteria.UserName;

            if (_userName != "INVALID_USER")
            {
                _isAuthenticated = true;
                Csla.ApplicationContext.GlobalContext["TEMPUSERNAME"] = _userName;
                PopulateRolePrivileges(_userName);
                //Ulises:Since the caller needs the ID and if there is a username an ID must exist, we force to retrieve the ID.
                //Note that when the DtaPtl_Fetch(PrivilegeCriteria) is called, there is no personID retrieval.
                if (this._id <= 0)
                    this._id = this.GetPersonID();
            }
            else
            {
                _identityToken = string.Empty;
                _userName = string.Empty;
                _isAuthenticated = false;
                _appRolePrivileges.Clear();
                Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"] = "Login failed getting privileges";
                throw new Exception("Login failed getting privileges");

            }

        }

        private void DataPortal_Fetch(TicketCriteria criteria) {
           
          
            //User SingleSingleOn to authenticateUser.
            coesinglesignon.SingleSignOn singleSignOn = new coesinglesignon.SingleSignOn();
            singleSignOn.Url = ConfigurationUtilities.GetSingleSignOnURL();
            string authTicket = string.Empty;
            try
            {
                _userName = singleSignOn.GetUserFromTicket(criteria.AuthenticationTicket).ToString();

                string authType = singleSignOn.GetDefaultAuthenticationProvider();
                if (authType.ToUpper() != "CSSECURITY")
                {
                    _isLDAP = true;
                    _isExempt = singleSignOn.IsExemptUser(_userName);
                }
                
                this.LoadDAL();
                this._id = _securityServiceDAL.GetPersonID(_userName);
                this._identityToken = criteria.AuthenticationTicket;
            }
            catch (Exception ex)
            {
                Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"] = ex.Message;
                throw;
            }

            if (_userName != string.Empty)
            {
                _isAuthenticated = true;
                Csla.ApplicationContext.GlobalContext["TEMPUSERNAME"] = _userName;
                PopulateRolePrivileges(_userName);
                //get information about LDAP and exempt users status
               
            }
            else
            {
                _identityToken = string.Empty;
                _userName = string.Empty;
                _isAuthenticated = false;
                _appRolePrivileges.Clear();
                Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"] = "Login failed getting privileges";
                throw new Exception("Login failed getting privileges");

            }
        
        }

        private void DataPortal_Fetch(RenewTicketCriteria criteria)
        {
            //User SingleSingleOn to authenticateUser.
            coesinglesignon.SingleSignOn singleSignOn = new coesinglesignon.SingleSignOn();
            singleSignOn.Url = ConfigurationUtilities.GetSingleSignOnURL();
            try
            {
                _identityToken = singleSignOn.RenewTicketIfOld(criteria.AuthenticationTicket).ToString();
            }
            catch (Exception ex)
            {
                _identityToken = criteria.AuthenticationTicket;
            }
        }

        private void DataPortal_Fetch(ValidateTicketCriteria criteria)
        {
            coesinglesignon.SingleSignOn singleSignOn = new coesinglesignon.SingleSignOn();
            singleSignOn.Url = ConfigurationUtilities.GetSingleSignOnURL();
            try
            {
                _isValidTicket = singleSignOn.ValidateTicket(criteria.AuthenticationTicket);
            }
            catch (Exception ex)
            {
                _isValidTicket = false;
            }
        }

        //this method bypasses singlesignon and uses a passed in cookie to set the identity.Name after initial authentication
        //using the passed in credentials.
        //this is used by applications that are simple databases that do not have oracle user accounts, but 
        //require a unique id inorder to user hitlist management
        private void DataPortal_Fetch(StandAloneCriteria criteria)
        {
            try
            {
                _identityToken= string.Empty;
                _isAuthenticated = true;
                Csla.ApplicationContext.GlobalContext["TEMPUSERNAME"] = criteria._userName;
                //we now require that cs_security is always used
                PopulateRolePrivileges(criteria._userName);
                string newUserName;
                if (criteria._standAloneID.Trim() == string.Empty)
                {
                    this._coeConnection.StartSession(COEUser.ID);
                    newUserName = Csla.ApplicationContext.GlobalContext["AppName"] + "_" + this._coeConnection.SessionID;
                }
                else
                {
                    newUserName = criteria._standAloneID;
                }
                Csla.ApplicationContext.GlobalContext["TEMPUSERNAME"] = newUserName;
                _userName = newUserName;

            }
            catch(System.Exception)
            {

                _identityToken = string.Empty;
                _userName = string.Empty;
                _isAuthenticated = false;
                _appRolePrivileges.Clear();
                Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"] = "Login failed getting privileges";
                throw new Exception("Login failed getting privileges");
            }
        }
        #endregion
       
        #region privileges method

        private void PopulateRolePrivileges(string userName)
        {
            LoadDAL();

            List<string> myUser = new List<string>();
            myUser.Add(userName);
            _appRolePrivileges = _securityServiceDAL.GetAllApplicationRoles(userName);
        }

        private void LoadDAL()
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _securityServiceDAL, _serviceName, Resources.SecurityDatabaseName, true);
        }

        /// <summary>
        /// Gets the current person ID.
        /// </summary>
        /// <returns>The unique ID of the current user</returns>
        private int GetPersonID()
        {
            int retVal = -1;
            this.LoadDAL();
            if (!string.IsNullOrEmpty(_userName))
                retVal = _securityServiceDAL.GetPersonID(_userName);
            return retVal;
        }

        #endregion
    }
 }

