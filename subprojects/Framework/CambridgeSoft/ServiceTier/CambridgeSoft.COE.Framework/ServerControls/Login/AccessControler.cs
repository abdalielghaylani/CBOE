using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Drawing;


namespace CambridgeSoft.COE.Framework.ServerControls.Login
{
    public class AccessController
    {
        #region Singleton
        private static AccessController _instance;
        public static AccessController Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AccessController();

                return _instance;
            }
        }
        #endregion

        #region Variables
        string _userName = string.Empty;
        string _server = string.Empty;
        bool _ssl = false;

        string _loginDialogCaption;
        int _loginDialogMaxRetries;
        Image _loginDialogLogo;
        #endregion

        #region Events
        public event EventHandler<LoginAttemptEventArgs> AfterLoginAttempt;

        public void OnAfterLoginAttempt(bool succeeded, string errorMessage)
        {
            if (this.AfterLoginAttempt != null)
                this.AfterLoginAttempt(this, new LoginAttemptEventArgs(succeeded, errorMessage));
        }

        public event EventHandler BeforeLoginAttempt;

        public void OnBeforeLoginAttempt()
        {
            if (this.BeforeLoginAttempt != null)
                this.BeforeLoginAttempt(this, new EventArgs());
        }
        #endregion

        #region Properties
        public int LoginMaxRetries
        {
            get 
            {
                return _loginDialogMaxRetries;
            }
            set 
            {
                _loginDialogMaxRetries = value;
            }
        }
        public string LoginDialogCaption
        {
            get 
            {
                return _loginDialogCaption;
            }
            set 
            {
                _loginDialogCaption = value;
            }
        }

        public Image LoginDialogLogo
        {
            get 
            {
                return _loginDialogLogo;
            }
            set 
            {
                _loginDialogLogo = value;
            }
        }
        public string LoginDialogLogoFile
        {
            set
            {
                _loginDialogLogo = Bitmap.FromFile(value);
            }
        }
        public string UserName
        {
            get
            {
                return _userName.Trim().ToUpper();
            }
        }

        public string Server
        {
            get
            {
                return _server;
            }
        }
        public bool SSL
        {
            get
            {
                return _ssl;
            }
        }

        public string ServerMode
        {
            get 
            {
                if (Is2TierServer(this.Server))
                    return StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_2T);

                return StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_3T);
            }
        }
        public bool IsLogged
        {
            get
            {
                return COEPrincipal.IsAuthenticated;
            }
        }

        private string LastServer
        {
            get
            {
                string key = string.Format("{0}_LastServer", COEAppName.Get());

                return AppDomain.CurrentDomain.GetData(key) == null ? string.Empty : AppDomain.CurrentDomain.GetData(key).ToString();
            }
            set
            {
                string key = string.Format("{0}_LastServer", COEAppName.Get());

                AppDomain.CurrentDomain.SetData(key, value);
            }
        }

        // 11.0.3 - Property for saving the last username
        private string LastUser
        {
            get
            {
                string key = string.Format("{0}_LastUser", COEAppName.Get());

                return AppDomain.CurrentDomain.GetData(key) == null ? string.Empty : AppDomain.CurrentDomain.GetData(key).ToString();
            }
            set
            {
                string key = string.Format("{0}_LastUser", COEAppName.Get());

                AppDomain.CurrentDomain.SetData(key, value);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the AccessControler object.
        /// </summary>
        /// <param name="stringParameters">the parameters sent from the web</param>
        private AccessController()
        {
            CleanUpCredentials();
        }
        #endregion

        #region Methods
        //to login when autentication ticket exist.
        public bool Login(string autenticationTicket)
        {
            try
            {
                if (!IsLogged)
                {
                    OnBeforeLoginAttempt();
                    bool suceeded = COEPrincipal.Login(autenticationTicket);
                    OnAfterLoginAttempt(IsLogged, string.Empty);
                    return suceeded;
                }

                return true;
            }
            catch (Exception exception)
            {
                OnAfterLoginAttempt(IsLogged, exception.Message);
                throw exception;
            }
        }

        //to login using username and password.
        public bool Login(string userName, string password)
        {
            try
            {
                if (!IsLogged)
                {
                    OnBeforeLoginAttempt();
                    bool response = COEPrincipal.Login(userName, password);

                    if(response)
                        _userName = userName;

                    OnAfterLoginAttempt(IsLogged, string.Empty);

                    return response;
                }
                return true;
            }
            catch (Exception exception)
            {
                OnAfterLoginAttempt(IsLogged, exception.Message);
                throw exception;
            }
        }

        private bool Login(string userName, string password, string serverName, bool ssl)
        {
                if (!this.IsLogged || serverName != LastServer)
                {
                    if (!string.IsNullOrEmpty(serverName))
                        SetServer(serverName, ssl);

                    return this.Login(userName, password);
                }

                return true;
        }

        public bool Login(string autenticationTicket, string serverName, bool ssl)
        {
            if (!this.IsLogged || serverName != LastServer)
            {
                if (!string.IsNullOrEmpty(serverName))
                    SetServer(serverName, ssl);

                return this.Login(autenticationTicket);
            }

            return true;
        }

        public bool Login()
        {
            try
            {
                if (!IsLogged)
                {
                    // 11.0.3
                    //string server = LastServer;
                    string server_user = string.IsNullOrEmpty(LastServer) ? "" : string.Format("{0} [{1}]", LastServer, LastUser);

                    // 11.0.3
                    //using (LoginDialog loginDialog = new LoginDialog(server))
                    using (LoginDialog loginDialog = new LoginDialog(server_user))
                    {
                        loginDialog.Text = this.LoginDialogCaption;
                        loginDialog.MaxRetries = this.LoginMaxRetries;
                        
                        // 11.0.3 - CSBR 135544
                        if (this.LoginDialogLogo != null)
                        {
                            loginDialog.Logo = this.LoginDialogLogo;
                        }

                        loginDialog.ValidateUser += new EventHandler<LoginEventArgs>(loginDialog_ValidateUser);

                        return loginDialog.ShowDialog() == DialogResult.OK;
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Advance Export - Overwrite the login method to auto fill the user and server details
        public bool LoginAdvanceExport(string userName, string serverName, string tier, bool ssl)
        {
            try
            {
                if (!this.IsLogged)
                {
                    if (tier.Equals(StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_2T), StringComparison.OrdinalIgnoreCase))
                    {
                        serverName = string.Format(StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_2T) + " / {0}", serverName);
                    }
                    else if (tier.Equals(StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_3T), StringComparison.OrdinalIgnoreCase) && ssl)
                    {
                        serverName = string.Format(StringValueAttribute.GetstringValue(AccessControllerConstants.SSL) + " / {0}", serverName);
                    }
                    //if (!string.IsNullOrEmpty(serverName))
                    //{
                    //    this.SetServer(serverName, ssl);
                    //}
                    using (LoginDialog loginDialog = new LoginDialog(string.Format("{0} [{1}]", serverName, userName), userName, ssl))
                    {
                        loginDialog.Text = this.LoginDialogCaption;
                        loginDialog.MaxRetries = this.LoginMaxRetries;
                        if (this.LoginDialogLogo != null)
                        {
                            loginDialog.Logo = this.LoginDialogLogo;
                        }
                        loginDialog.ValidateUser += new EventHandler<LoginEventArgs>(this.loginDialog_ValidateUser);
                        return (loginDialog.ShowDialog() == DialogResult.OK);
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }
        }


        public void LogOut()
        {
            COEPrincipal.Logout();
            CleanUpCredentials();
        }

        private void CleanUpCredentials()
        {
            _userName = _server = string.Empty;
            _ssl = false;
        }


        private void loginDialog_ValidateUser(object sender, LoginEventArgs e)
        {
            try
            {
                e.Succeeded = this.Login(e.UserName, e.Password, e.Server, e.SSL);

                if (e.Succeeded)
                {
                    LastServer = e.Server;
                    // 11.0.3
                    LastUser = e.UserName;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(Properties.Resources.excepUserPass) || ex.Message.Contains(Properties.Resources.nullPassword))
                    ((LoginDialog)sender).DisplayError(Properties.Resources.msgInvalidUserPass);
                else if (ex.Message.Contains(Properties.Resources.excepInvalidService))
                    ((LoginDialog)sender).DisplayError(Properties.Resources.msgInvalidService);
                else if (ex.Message.Contains(Properties.Resources.excepConfigNotFound))
                    ((LoginDialog)sender).DisplayError(Properties.Resources.msgConfigNotFound);
                else
                    ((LoginDialog)sender).DisplayError("Login Failed: " + ex.Message);
            }
        }


        private bool Is2TierServer(string serverName)
        {
            return ServerNameUtils.StartsWith(serverName, StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_2T));
        }

        public void SetServer(string serverName, bool ssl)
        {
            try
            {
                if (Is2TierServer(serverName))
                {
                    AppSettingsManager.Write(StringValueAttribute.GetstringValue(AccessControllerConstants.CSLADATAPORTALPROXY), null);
                    AppSettingsManager.Write(StringValueAttribute.GetstringValue(AccessControllerConstants.CSLADATAPORTALURL), "");


                    string sInputName = ServerNameUtils.BeforeDelimiter(ServerNameUtils.AfterDelimiter(serverName, '/'), '[').Trim();
                    if (!string.IsNullOrEmpty(sInputName))
                    {
                        DBMSTypeData dbmsd = ConfigurationUtilities.GetDBMSTypeData(DBMSType.ORACLE);
                        dbmsd.DataSource = sInputName;
                    }
                }
                else
                {

                    string sServerStr = string.Empty;
                    if (ssl == true)
                        sServerStr = MRUEntry.MakeSURL(serverName);
                    else
                        sServerStr = MRUEntry.MakeURL(serverName);

                    AppSettingsManager.Write(StringValueAttribute.GetstringValue(AccessControllerConstants.CSLADATAPORTALPROXY), typeof(Csla.DataPortalClient.WebServicesProxy).AssemblyQualifiedName);
                    AppSettingsManager.Write(StringValueAttribute.GetstringValue(AccessControllerConstants.CSLADATAPORTALURL), sServerStr);
                }

                _server = serverName;
                _ssl = ssl;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }

    public class LoginAttemptEventArgs : EventArgs
    {
        #region Variables
        private bool _succeeded;
        private string _errorMessage;
        #endregion

        #region Properties
        public bool Succeeded
        {
            get
            {
                return _succeeded;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
        }
        #endregion

        #region Constructors
        public LoginAttemptEventArgs(bool succeeded)
            : this(succeeded, string.Empty)
        {
        }

        public LoginAttemptEventArgs(bool succeeded, string errorMessage)
        {
            _succeeded = succeeded;
            _errorMessage = errorMessage;
        }
        #endregion
    }

    public enum AccessControllerConstants
    {
        [StringValue("2-Tier")]
        MRU_2T,
        [StringValue("3-Tier")]
        MRU_3T,
        [StringValue("SSL")]
        SSL,
        [StringValue("CslaDataPortalProxy")]
        CSLADATAPORTALPROXY,
        [StringValue("CslaDataPortalUrl")]
        CSLADATAPORTALURL,
    }

    /// <summary>
    /// Simple attribute class for storing string Values
    /// </summary>
    public class StringValueAttribute : Attribute
    {
        #region Attributes
        private string _value;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value></value>
        public string Value
        {
            get { return _value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="stringValueAttribute"/> instance.
        /// </summary>
        /// <param name="value">Value.</param>
        public StringValueAttribute(string value)
        {
            _value = value;
        }
        #endregion

        #region Methods
        public static string GetstringValue(Enum value)
        {
            object[] attributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(StringValueAttribute), false);
            if (attributes.Length > 0)
            {
                return ((StringValueAttribute)attributes[0]).Value;
            }
            return null;
        }
        #endregion
    }

}
