using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using CambridgeSoft.COE.Framework.Common;
using Csla;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COESecurityService
{
    public class COEMembershipProvider : MembershipProvider
    {
        [NonSerialized]
         static COELog _coeLog =  COELog.GetSingleton("COESecurity");


        /// <summary>
        /// Validate user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override bool ValidateUser(string userName, string password)
        {   bool result = false;
            string appName = string.Empty;
                //this variable needs to be set when an application starts up. It is done in the global.aspx. If it is not populated throw an error

            if (HttpContext.Current.Application["AppName"]!= null)
            {
                appName = HttpContext.Current.Application["AppName"].ToString();
                Csla.ApplicationContext.GlobalContext["AppName"] = appName;
            }else{
                appName=ConfigurationManager.AppSettings["AppName"].ToString();
                Csla.ApplicationContext.GlobalContext["AppName"] = appName;
                HttpContext.Current.Application["AppName"] = appName;
            }
            
                if (appName == string.Empty){
                     throw new Exception(Resources.AppNameNotSetInCSLA);
                }else
                {
                    try
                    {
                        result = COEPrincipal.Login(userName, password);
                        HttpContext.Current.Session["CslaPrincipal"] = Csla.ApplicationContext.User;
                        COEIdentity identity = (COEIdentity)Csla.ApplicationContext.User.Identity;
  
                        //JHS 2/10/2012 - Add a new session variable that has the ticket received from SSO
                        //This ticket will get used in Login.ascx.cs so that it can be written to the cookie.
                        //Writing it below seems to be basically useless.
                        HttpContext.Current.Session["SSOTicket"] = identity.IdentityToken;

                        //JHS 2/10/2012 - I am leaving this here despite that it may not be too useful.
                        //This is a *just in case* since I don't want new issues to appear.  Login will
                        //overwrite this cookie anyway.
                        HttpContext.Current.Response.Cookies["COESSO"].Value = identity.IdentityToken;
                        HttpContext.Current.Response.Cookies["COESSO"].Path = "/";

                        HttpContext.Current.Response.Cookies["DisableInactivity"].Value = "true";
                        HttpContext.Current.Response.Cookies["DisableInactivity"].Path = "/";
                        HttpContext.Current.Response.Cookies["DisableInactivity"].Expires = DateTime.Now.AddMinutes(HttpContext.Current.Session.Timeout - 1);


                        //JHS 1/8/2009 - write some info to classic cs_security cookie so that classic can
                        //pick up username and password
                        //this can be removed when classic apps are gone
                        //JHS 9/1/2009 - ONly the encoded cookie names are needed
                        //The password is now encrypted in an algorythm matching the classic code
                        //The values must be encoded so that classic asp can read them properly
                        HttpContext.Current.Response.Cookies["CS%5FSEC%5FUserName"].Path = "/";
                        HttpContext.Current.Response.Cookies["CS%5FSEC%5FUserName"].Value = HttpUtility.UrlEncode(userName);

                        HttpContext.Current.Response.Cookies["CS%5FSEC%5FUserID"].Path = "/";
                        HttpContext.Current.Response.Cookies["CS%5FSEC%5FUserID"].Value = HttpUtility.UrlEncode(CryptVBS(password, userName));

                        //JHS 6/10/2009 - Set the SSO provider so it can be used in classic
                        //Do it using both methods...see below..until it is determined which one is actually working
                        //don't want to call SSO, so for now stick with the isLdap flag
                        string ssop = "CSSecurity";
                        if (identity.IsLDAP)
                        {
                            ssop = "COELDAP";
                        }

                        HttpContext.Current.Response.Cookies["SSOPROVIDER"].Path = "/";
                        HttpContext.Current.Response.Cookies["SSOPROVIDER"].Value = ssop;


                        //HttpCookie nameCookie = new HttpCookie(HttpUtility.UrlEncode("CS_SEC_UserName"), HttpUtility.UrlEncode(userName));
                        //nameCookie.Path = "/";
                        //HttpContext.Current.Response.Cookies.Add(nameCookie);
                        //HttpCookie passwordCookie = new HttpCookie(HttpUtility.UrlEncode("CS_SEC_UserID"), HttpUtility.UrlEncode(password));
                        //passwordCookie.Path = "/";
                        //HttpContext.Current.Response.Cookies.Add(passwordCookie);


                        //JHS 6/10/2009 - Set the SSO provider so it can be used in classic
                        //Do it using both methods...see above..until it is determined which one is actually working
                        HttpCookie ssoTypeCookie = new HttpCookie(HttpUtility.UrlEncode("SSOPROVIDER"), HttpUtility.UrlEncode(ssop));
                        ssoTypeCookie.Path = "/";
                        HttpContext.Current.Response.Cookies.Add(ssoTypeCookie);


                        //Add aspsessionid to list
                        AddToCookies();
                    }
                    catch (Csla.DataPortalException dpex)
                    {
                        if (Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"] == null || Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"] == string.Empty)
                        {
                            Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"] = dpex.BusinessException.Message;
                        }

                        HttpContext.Current.Session["LOGIN_FAILURE_MODE"] = Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"];

                        return false;
                    }
                    catch (System.Exception ex)
                    {
                        if (Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"] == null || Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"] == string.Empty)
                        {
                            Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"] = ex.Message;
                        }

                        HttpContext.Current.Session["LOGIN_FAILURE_MODE"] = Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"];

                        return false;
                    }
                }
                HttpContext.Current.Session["USER_PERSONID"] = Csla.ApplicationContext.GlobalContext["USER_PERSONID"];
                return result;
        }


        public  bool ValidateUser_StaticLogin(string userName, string password)
        {
            bool result = false;
            string appName = string.Empty;
            //this variable needs to be set when an application starts up. It is done in the global.aspx. If it is not populated throw an error

            if (HttpContext.Current.Application["AppName"] != null)
            {
                appName = HttpContext.Current.Application["AppName"].ToString();
                Csla.ApplicationContext.GlobalContext["AppName"] = appName;
            }
            else
            {
                appName = ConfigurationManager.AppSettings["AppName"].ToString();
                Csla.ApplicationContext.GlobalContext["AppName"] = appName;
                HttpContext.Current.Application["AppName"] = appName;
            }

            if (appName == string.Empty)
            {
                throw new Exception(Resources.AppNameNotSetInCSLA);
            }
            else
            {
                try
                {
                    result = COEPrincipal.Login(userName, password);
                    HttpContext.Current.Application["CslaPrincipal"] = Csla.ApplicationContext.User;
                    HttpContext.Current.Application["USER_PERSONID"] = Csla.ApplicationContext.GlobalContext["USER_PERSONID"];


                }
                catch (Exception e)
                {
                    HttpContext.Current.Session["LOGIN_FAILURE_MODE"] = Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"];
                    throw;
                }
            }
            return result;
        }
        public bool Login(string userName, bool isTicket)
        {
            bool result = false;
            string appName = string.Empty;
            //this variable needs to be set when an application starts up. It is done in the global.aspx. If it is not populated throw an error

            if (HttpContext.Current.Application["AppName="] != null)
            {
                appName = HttpContext.Current.Application["AppName="].ToString();
                Csla.ApplicationContext.GlobalContext["AppName"] = appName;
            }
            else
            {
                appName = ConfigurationManager.AppSettings["AppName"].ToString();
                Csla.ApplicationContext.GlobalContext["AppName"] = appName;
                HttpContext.Current.Application["AppName="] = appName;
            }

            if (appName == string.Empty)
            {
                throw new Exception(Resources.AppNameNotSetInCSLA);
            }
            else
            {
                result = COEPrincipal.Login(userName, isTicket);
                HttpContext.Current.Session["CslaPrincipal"] = Csla.ApplicationContext.User;
            }

            HttpContext.Current.Session["USER_PERSONID"] = Csla.ApplicationContext.GlobalContext["USER_PERSONID"];
            return result;
        }

        /// <summary>
        /// Validate user  in standalone application mode
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public  bool ValidateUser_StandAlone(string userName, string password)
        {
            bool result = false;
            string appName = string.Empty;
            //need to see if the framework config file location is being changed for standalnone
            if (ConfigurationManager.AppSettings["COEConfigPathOverride"].ToLower() == "true")
            {
                HttpContext.Current.Application["COEConfigPathOverride"] = HttpContext.Current.Server.MapPath(@"\" + ConfigurationManager.AppSettings["AppName"]);
                Csla.ApplicationContext.GlobalContext["COEConfigPathOverride"] = HttpContext.Current.Application["COEConfigPathOverride"].ToString();
            }

            if (HttpContext.Current.Application["AppName"] != null)
            {
                appName = HttpContext.Current.Application["AppName"].ToString();
                Csla.ApplicationContext.GlobalContext["AppName"] = appName;
            }
            else
            {
                appName = ConfigurationManager.AppSettings["AppName"].ToString();
                Csla.ApplicationContext.GlobalContext["AppName"] = appName;
                HttpContext.Current.Application["AppName"] = appName;
            }

          

            if (appName == string.Empty)
            {
                throw new Exception(Resources.AppNameNotSetInCSLA);
            }
            else
            {
                try
                {
                    
                    string standAloneID = string.Empty;
                    if (HttpContext.Current.Request.Cookies[appName + "STANDALONE_USERID"] != null)
                    {
                        standAloneID = HttpContext.Current.Request.Cookies[appName + "STANDALONE_USERID"].Value;
                    }
                    if (standAloneID == null) { standAloneID = string.Empty; }
                    result = COEPrincipal.Login_Standalone(userName, password, standAloneID);

                    HttpContext.Current.Session["CslaPrincipal"] = Csla.ApplicationContext.User;
                    COEIdentity identity = (COEIdentity)Csla.ApplicationContext.User.Identity;
                    HttpContext.Current.Session["TEMPUSERNAME"] = Csla.ApplicationContext.GlobalContext["TEMPUSERNAME"].ToString();

                    HttpContext.Current.Response.Cookies[appName + "STANDALONE_USERID"].Value = identity.Name;
                    HttpContext.Current.Response.Cookies[appName + "STANDALONE_USERID"].Expires = DateTime.Now.AddDays(365);


                    HttpContext.Current.Response.Cookies[appName + "STANDALONE_USERID"].Path = "/";



                    HttpContext.Current.Session["CslaPrincipal"] = Csla.ApplicationContext.User;

                    
                }
                catch (System.Exception ex)
                {
                    HttpContext.Current.Session["LOGIN_FAILURE_MODE"] = Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"];
                    return false;
                }
            }
            HttpContext.Current.Session["USER_PERSONID"] = Csla.ApplicationContext.GlobalContext["USER_PERSONID"];
            return result;
        }

        public bool ValidateUser_StaticLogin_StandAlone(string userName, string password)
        {
            bool result = false;
            string appName = string.Empty;
            //this variable needs to be set when an application starts up. It is done in the global.aspx. If it is not populated throw an error
            if (ConfigurationManager.AppSettings["COEConfigPathOverride"].ToLower() == "true")
            {
                HttpContext.Current.Application["COEConfigPathOverride"] = HttpContext.Current.Server.MapPath(@"\" + ConfigurationManager.AppSettings["AppName"]);
                Csla.ApplicationContext.GlobalContext["COEConfigPathOverride"] = HttpContext.Current.Application["COEConfigPathOverride"].ToString();
            }
            if (HttpContext.Current.Application["AppName"] != null)
            {
                appName = HttpContext.Current.Application["AppName"].ToString();
                Csla.ApplicationContext.GlobalContext["AppName"] = appName;
            }
            else
            {
                appName = ConfigurationManager.AppSettings["AppName"].ToString();
                Csla.ApplicationContext.GlobalContext["AppName"] = appName;
                HttpContext.Current.Application["AppName"] = appName;
            }

            if (appName == string.Empty)
            {
                throw new Exception(Resources.AppNameNotSetInCSLA);
            }
            else
            {
                try
                {

                    string standAloneID = string.Empty;
                    if (HttpContext.Current.Request.Cookies[appName + "STANDALONE_USERID"] != null)
                    {
                        standAloneID = HttpContext.Current.Request.Cookies[appName + "STANDALONE_USERID"].Value;
                    }
                    if (standAloneID == null) { standAloneID = string.Empty; }
                    result = COEPrincipal.Login_Standalone(userName, password, standAloneID);
                    HttpContext.Current.Application["CslaPrincipal"] = Csla.ApplicationContext.User;
                    HttpContext.Current.Application["USER_PERSONID"] = Csla.ApplicationContext.GlobalContext["USER_PERSONID"];
                    COEIdentity identity = (COEIdentity)Csla.ApplicationContext.User.Identity;
                    HttpContext.Current.Application["TEMPUSERNAME"] = Csla.ApplicationContext.GlobalContext["TEMPUSERNAME"].ToString();
                    HttpContext.Current.Response.Cookies[appName + "STANDALONE_USERID"].Value = identity.Name;
                    HttpContext.Current.Response.Cookies[appName + "STANDALONE_USERID"].Expires = DateTime.Now.AddDays(365);
                    HttpContext.Current.Response.Cookies[appName + "STANDALONE_USERID"].Path = "/";



                }
                catch (Exception e)
                {
                    HttpContext.Current.Session["LOGIN_FAILURE_MODE"] = Csla.ApplicationContext.GlobalContext["LOGIN_FAILURE_MODE"];
                    throw;
                }
            }
            return result;
        }

        public void LogOut()
        {
            KillCookies();
            KillSessions(); 
            COEPrincipal.Logout();
            if (HttpContext.Current != null && HttpContext.Current.Response !=null) 
                HttpContext.Current.Response.Redirect("/COEManager"); //CBOE-1622 Windows Security Window is launched on logging out of CBOE Manager. : We are redirecting to home page to resolve this.
        }

        public void AzureLogOut()
        {
            KillCookies();
            KillSessions();
            COEPrincipal.Logout();
        }

        private void KillCookies()
        {
            if(HttpContext.Current != null)
            {
                //%5F is an underscore -classic asp requires the encoding
                if(HttpContext.Current.Request.Cookies["CS%5FSEC%5FUserName"] != null)
                {
                    HttpContext.Current.Response.Cookies["CS%5FSEC%5FUserName"].Path = "/";
                    HttpContext.Current.Response.Cookies["CS%5FSEC%5FUserName"].Value = string.Empty;
                    HttpContext.Current.Response.Cookies["CS%5FSEC%5FUserName"].Expires = DateTime.Now.AddDays(-1);
                }

                if(HttpContext.Current.Request.Cookies["CS%5FSEC%5FUserID"] != null)
                {
                    HttpContext.Current.Response.Cookies["CS%5FSEC%5FUserID"].Path = "/";
                    HttpContext.Current.Response.Cookies["CS%5FSEC%5FUserID"].Value = string.Empty;
                    HttpContext.Current.Response.Cookies["CS%5FSEC%5FUserID"].Expires = DateTime.Now.AddDays(-1);
                }

                if(HttpContext.Current.Request.Cookies["COESSO"] != null)
                {
                    HttpContext.Current.Response.Cookies["COESSO"].Path = "/";
                    HttpContext.Current.Response.Cookies["COESSO"].Value = string.Empty;
                    HttpContext.Current.Response.Cookies["COESSO"].Expires = DateTime.Now.AddDays(-1);
                }

                if(HttpContext.Current.Request.Cookies["SSOPROVIDER"] != null)
                {
                    HttpContext.Current.Response.Cookies["SSOPROVIDER"].Path = "/";
                    HttpContext.Current.Response.Cookies["SSOPROVIDER"].Value = string.Empty;
                    HttpContext.Current.Response.Cookies["SSOPROVIDER"].Expires = DateTime.Now.AddDays(-1);
                }

                if(HttpContext.Current.Request.Cookies["COWSASPIDS"] != null)
                {
                    HttpContext.Current.Response.Cookies["COWSASPIDS"].Value = string.Empty;
                    HttpContext.Current.Response.Cookies["COWSASPIDS"].Path = "/";
                    HttpContext.Current.Response.Cookies["COWSASPIDS"].Expires = DateTime.Now.AddDays(-1);
                }
            }
        }

        private void KillSessions()
        {
            if(HttpContext.Current != null)
            {
                string aspCookieName = string.Empty;
                string cookieString = string.Empty;
                /*CSBR# 136122
                 *Date of Change - 22-02-2011 by Soorya
                 * Purpose - To decode the escaped  "%2c" back to "," before the cookie string is split
                 */ 
                if(HttpContext.Current.Request.Cookies["COWSASPIDS"] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Cookies["COWSASPIDS"].Value))
                    cookieString = HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies["COWSASPIDS"].Value);
                //End of Change

                // Coverity Fix CID - 19050
                if (!string.IsNullOrEmpty(cookieString) && cookieString.Trim().Length != 0) //Coverity Fix CID 19050
                {
                    string[] cookieArray = cookieString.Split(',');
                    for (int i = 0; i < cookieArray.Length; i++)
                    {
                        //asp
                        aspCookieName = "ASPSESSIONID" + cookieArray[i];
                        if (HttpContext.Current.Response.Cookies[aspCookieName] != null)
                        {
                            HttpContext.Current.Response.Cookies[aspCookieName].Value = string.Empty;
                            HttpContext.Current.Response.Cookies[aspCookieName].Expires = DateTime.Now;
                            HttpContext.Current.Response.Cookies[aspCookieName].Path = "/";
                        }
                    }
                }

                //asp.net
                if(HttpContext.Current.Response.Cookies["ASP.NET_SessionId"] != null)
                {
                    HttpContext.Current.Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                    HttpContext.Current.Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now;
                    HttpContext.Current.Response.Cookies["ASP.NET_SessionId"].Path = "/";
                }
                
            }
        }

        private string CryptVBS(string txt, string key)
        {
            txt = shiftChr(txt, -1);
            int keyLength = key.Length;
            int keyPtr = 0;
            string hold = string.Empty;
            string cryptKey = string.Empty;

            for (int i = 0; i < txt.Length; i++)
            {

                keyPtr = (keyPtr + 1) % keyLength;

                cryptKey = ((char)((int)txt[i] ^ (int)key[keyPtr])).ToString();
                hold += cryptKey;
            }

            return shiftChr(hold, 1);

        }
        private string shiftChr(string txt, int offset)
        {
            string hold = string.Empty;
            for (int i = 0; i < txt.Length; i++)
            {
                hold += ((char)((int)txt[i] + offset)).ToString();
            }

            return hold;
        }



        private void AddToCookies()
        {
            string cookieString = string.Empty;
            string ASPcookieID = string.Empty;
            try
            {
                if (HttpContext.Current.Request.Cookies["COWSASPIDS"] != null && HttpContext.Current.Request.Cookies["COWSASPIDS"].Value != null)
                {
                    cookieString = HttpContext.Current.Request.Cookies["COWSASPIDS"].Value;
                }
                //string aspSessionID = ;
                //if (HttpContext.Current.Request.Cookies["ASP.NET_SessionId"].Value != null)
                //{
                //     ASPcookieID = HttpContext.Current.Request.Cookies["ASP.NET_SessionId"].Value;
                //}
                if (cookieString.Length > 0)
                {
                    cookieString = cookieString + "," + HttpContext.Current.Session.SessionID;
                }
                else
                {
                    cookieString = HttpContext.Current.Session.SessionID;
                }

            }
            catch(System.Exception e)
            { 
            
            }
            HttpContext.Current.Response.Cookies["COWSASPIDS"].Value = cookieString;
            HttpContext.Current.Response.Cookies["COWSASPIDS"].Path = "/";
        }





        #region Members not implemented

        /// <summary>
        /// Application Name property
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                return string.Empty;
            }
            set
            {
                ;
            }
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {   //here we need a scheme to check that the newPassword follows a protocol

            bool success = false;
            try
            {
                success = COEUserBO.ChangePassword(username, oldPassword, newPassword);

                return success;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Change password only after answering questions
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="newPasswordQuestion"></param>
        /// <param name="newPasswordAnswer"></param>
        /// <returns></returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            return false;
        }


        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="passwordQuestion"></param>
        /// <param name="passwordAnswer"></param>
        /// <param name="isApproved"></param>
        /// <param name="providerUserKey"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            status = MembershipCreateStatus.UserRejected;
            return null;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            return false;
        }

        public override bool EnablePasswordReset
        {
            get { return false; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = -1;
            return null;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = -1;
            return null;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = -1;
            return null;
        }

        public override int GetNumberOfUsersOnline()
        {
            return -1;
        }

        public override string GetPassword(string username, string answer)
        {
            return string.Empty;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            
            MembershipUser myUser = new MembershipUser("COEMembershipProvider", username, null, string.Empty, string.Empty, string.Empty, true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
            return myUser;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            return null;
        }

        public override string GetUserNameByEmail(string email)
        {
            return string.Empty;
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return 0; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 0; }
        }

        public override int PasswordAttemptWindow
        {
            get { return -1; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Clear; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return string.Empty; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return false; }
        }

        public override string ResetPassword(string username, string answer)
        {
            return string.Empty;
        }

        public override bool UnlockUser(string userName)
        {
            return false;
        }

        public override void UpdateUser(MembershipUser user)
        {
            return;
        }

        #endregion

    }
}
