using System;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.GUIShell;
using PerkinElmer.COE.Registration.Server.Code;
using Resources;

namespace PerkinElmer.COE.Registration.Server
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Add(new JsonMediaTypeFormatter());
            HttpContext.Current.Application.Lock();
            AppDomain.CurrentDomain.DomainUnload += AppDomainUnloading;
            HttpContext.Current.Application[RegistrationWebApp.Constants.AppName] = GUIShellUtilities.GetApplicationName();
            HttpContext.Current.Application[RegistrationWebApp.Constants.AppPagesTitle] = GUIShellUtilities.GetDefaultPagesTitle();
            HttpContext.Current.Application.UnLock();
        }

        private void AppDomainUnloading(object s, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        protected void Application_End(object sender, EventArgs e)
        {
            // Code that runs on application shutdown
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //What was the error we encountered?
            Exception serverEx = this.Server.GetLastError().GetBaseException();
            //Coverity fix - CID 11866 - no need to check for null value of server exception, as the event will trigger only when any error occurs.
            if (serverEx != null)
            {
                string policyName = RegistrationWebApp.Constants.REG_OTHER_POLICY;

                //Default error 'code'; not very user-friendly in reality
                string error = CambridgeSoft.COE.Framework.GUIShell.GUIShellTypes.MessagesCode.Unknown.ToString();

                //Unhandled exeptions must display a generic message and be logged.
                Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.ExceptionPolicy.HandleException(
                    serverEx, policyName);

                //Log all unhandled exceptions!

                error = serverEx.Message;
                try
                {
                    System.Diagnostics.EventLog.WriteEntry("COERegistrationWeb", serverEx.ToString(),
                        System.Diagnostics.EventLogEntryType.Error);
                }
                catch
                {
                    COELog coeLog = COELog.GetSingleton("RegistartionWebApp");
                    coeLog.Log(serverEx.ToString());
                }

                //Redirect ends the response. Server transfer doesn't so it might not show some errors and Causes a Session being null on some pages.
                Response.Redirect("~/Forms/Public/ContentArea/Messages.aspx?MessageCode=" + error);
            }
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.Url.AbsoluteUri.Contains(".axd") || HttpContext.Current.Request.Url.AbsoluteUri.Contains(".asmx"))
            {
                return;
            }
            if (Request["ticket"] != null)
            {
                CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider member = new CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider();
                member.Login(Request["ticket"].ToString(), true);
            }
            if (HttpContext.Current.Session != null)
            {
                CambridgeSoft.COE.Framework.Common.WebUtils.SetCslaPrincipal();
                Csla.ApplicationContext.GlobalContext["USER_PERSONID"] = HttpContext.Current.Session["USER_PERSONID"];
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated) return;

            var ssoCookie = Request.Cookies[Consts.ssoCookieName];
            var ssoCookieValue = ssoCookie == null ? null : ssoCookie.Value;
            FormsAuthenticationTicket authTicket = null;
            try
            {
                if (!string.IsNullOrEmpty(ssoCookieValue))
                    authTicket = FormsAuthentication.Decrypt(ssoCookieValue);
            }
            catch
            {
                // Log exception details (omitted for simplicity)
                return;
            }
            if (null == authTicket) return;

            authTicket = FormsAuthentication.RenewTicketIfOld(authTicket);
            // Create an Identity object
            FormsIdentity id = new FormsIdentity(authTicket);
            // This principal will flow throughout the request.
            System.Security.Principal.GenericPrincipal principal = new System.Security.Principal.GenericPrincipal(id, null);
            // Attach the new principal object to the current HttpContext object
            Context.User = principal;
            Response.Cookies[Consts.ssoCookieName].Value = ssoCookieValue;
            Response.Cookies[Consts.ssoCookieName].Path = "/";
            Response.Cookies["DisableInactivity"].Value = "true";
            Response.Cookies["DisableInactivity"].Path = "/";
            Response.Cookies["DisableInactivity"].Expires = DateTime.Now.AddMinutes(25);
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;

            if (!HttpContext.Current.Request.Url.AbsoluteUri.Contains(".aspx") || app.Request["HTTP_X_MICROSOFTAJAX"] != null)
                return;

            string acceptEncoding = app.Request.Headers["Accept-Encoding"];
            Stream prevUncompressedStream = app.Response.Filter;

            if (acceptEncoding == null || acceptEncoding.Length == 0)
                return;

            acceptEncoding = acceptEncoding.ToLower();

            if (acceptEncoding.Contains("deflate") || acceptEncoding == "*")
            {
                // defalte
                app.Response.Filter = new DeflateStream(prevUncompressedStream,
                    CompressionMode.Compress);
                app.Response.AppendHeader("Content-Encoding", "deflate");
            }
            else if (acceptEncoding.Contains("gzip"))
            {
                // gzip
                app.Response.Filter = new GZipStream(prevUncompressedStream,
                    CompressionMode.Compress);
                app.Response.AppendHeader("Content-Encoding", "gzip");
            }
        }

        // Code that runs when a new session is started
        protected void Session_Start(object sender, EventArgs e)
        {
            DoLogin();
            // Get RegistryRecord prototype to avoid multiple calls.
            if (HttpContext.Current.Session["NewRegistryRecord"] == null)
            {
                RegUtilities.GetNewRegistryRecord(); // Dont change.
                // Note : Some pages will call registryrecord object at prerendercomplete[Submitmixutre.aspx], it will not force user to import configuration if is defect, works independent of any page events.
            }
            // COEPageControlSettings.
            if (!string.IsNullOrEmpty(RegUtilities.GetConfigSetting("MISC", "PageControlsManager")))
                if (RegUtilities.GetConfigSetting("MISC", "PageControlsManager").ToUpper() == "ENABLE")
                    RegUtilities.SetCOEPageSettings(false);

            if (!string.IsNullOrEmpty(RegUtilities.GetConfigSetting("MISC", "EnableLogging")))
                CambridgeSoft.COE.Framework.COELoggingService.COELog.GetSingleton("COERegistration").Enabled = bool.Parse(RegUtilities.GetConfigSetting("MISC", "EnableLogging"));

            if (Request["ticket"] == null && Request.Headers["Cookie"] != null && Request.Headers["Cookie"].IndexOf("ASP.NET_SessionId") >= 0 && (Request.Cookies["DisableInactivity"] == null || string.IsNullOrEmpty(Request.Cookies["DisableInactivity"].Value)) && !Request.Url.Query.Contains("Inactivity=true"))
            {
                DoLogout();
                Response.Redirect("/COEManager/Forms/Public/ContentArea/Login.aspx?Inactivity=true&ReturnURL=" + HttpContext.Current.Request.Url.ToString());
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {
            //CambridgeSoft.COE.Framework.GUIShell.GUIShellUtilities.DoLogout();
            //this.DoLogout();
        }

        private void DoLogin()
        {
            string appName = string.Empty;
            bool isTicket = true;
            string userIdentifier = string.Empty;
            //check to see if there is an entry for ticket in the url. if ther is then you need to go through a different root.

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


            if (RegUtilities.GetConfigSetting("MISC", "LogInSimulationMode") != String.Empty)
            {
                userIdentifier = RegUtilities.GetConfigSetting("MISC", "LogInSimulationMode");
                isTicket = false;
            }
            else
            {
                if (Request.Cookies[Consts.ssoCookieName] != null && !string.IsNullOrEmpty(Request.Cookies[Consts.ssoCookieName].Value))
                {
                    userIdentifier = Request.Cookies[Consts.ssoCookieName].Value;
                    isTicket = true;
                }
                else
                {
                    throw new Exception(Resource.UserNameRequired_Exception);
                }
            }

            if (!string.IsNullOrEmpty(userIdentifier))
            {
                CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider memProvider = new CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider();
                if (!memProvider.Login(userIdentifier, isTicket))
                {
                    throw new Exception(Resource.UnableToLogin_Exception);
                }
            }
        }

        private void DoLogin(string ticket)
        {
            if (!string.IsNullOrEmpty(ticket))
            {
                CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider memProvider = new CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider();
                if (!memProvider.Login(ticket, true))
                {
                    throw new Exception(Resource.UnableToLogin_Exception);
                }
            }
        }

        private void DoLogout()
        {
            CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider memProvider = new CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider();
            memProvider.LogOut();
        }
    }
}