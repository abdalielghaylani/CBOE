using System;
using System.Configuration;
using System.Collections;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.GUIShell;
using Resources;

public class Reg2_Global : System.Web.HttpApplication
{

    protected void Application_Start(object sender, EventArgs e)
    {
        HttpContext.Current.Application.Lock();
        AppDomain.CurrentDomain.DomainUnload += new EventHandler(AppDomainUnloading);
        HttpContext.Current.Application[PerkinElmer.CBOE.Registration.Client.Constants.AppName] = GUIShellUtilities.GetApplicationName();
        HttpContext.Current.Application[PerkinElmer.CBOE.Registration.Client.Constants.AppPagesTitle] = GUIShellUtilities.GetDefaultPagesTitle();
        HttpContext.Current.Application[GUIShellTypes.EnablePopUpForBrowserVersions] = GUIShellUtilities.GetPopUpBrowserVersions();
        HttpContext.Current.Application[GUIShellTypes.Themes] = GUIShellUtilities.GetThemes();
        HttpContext.Current.Application.UnLock();
    }

    public void AppDomainUnloading(object s, EventArgs e)
    {
        System.Environment.Exit(0);
    }

    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown

    }

    void Application_Error(object sender, EventArgs e)
    {
        //What was the error we encountered?
        Exception serverEx = this.Server.GetLastError().GetBaseException();
        //Coverity fix - CID 11866 - no need to check for null value of server exception, as the event will trigger only when any error occurs.
        if (serverEx != null)
        {
            string policyName = PerkinElmer.CBOE.Registration.Client.Constants.REG_OTHER_POLICY;

            //Default error 'code'; not very user-friendly in reality
            string error = CambridgeSoft.COE.Framework.GUIShell.GUIShellTypes.MessagesCode.Unknown.ToString();

            //Unhandled exeptions must display a generic message and be logged.
            Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.ExceptionPolicy.HandleException(
                serverEx, policyName
            );

            //Log all unhandled exceptions!

            error = serverEx.Message;
            try
            {
                System.Diagnostics.EventLog.WriteEntry(
                    "COERegistrationWeb"
                    , serverEx.ToString()
                    , System.Diagnostics.EventLogEntryType.Error
                );
            }
            catch
            {
                COELog _coeLog = COELog.GetSingleton("RegistartionWebApp");
                _coeLog.Log(serverEx.ToString());
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
        CambridgeSoft.COE.Framework.Common.WebUtils.SetCslaPrincipal();
        if (HttpContext.Current.Session != null)
            Csla.ApplicationContext.GlobalContext["USER_PERSONID"] = HttpContext.Current.Session["USER_PERSONID"];
    }

    protected void Application_AuthenticateRequest(Object sender, EventArgs e)
    {
        if (!Request.IsAuthenticated)
        {
            FormsAuthenticationTicket authTicket = null;

            try
            {
                if (Request["ticket"] != null)
                {
                    authTicket = FormsAuthentication.Decrypt(Request["ticket"]);
                }
                else if (Request.Cookies["COESSO"] != null && !string.IsNullOrEmpty(Request.Cookies["COESSO"].Value))
                {
                    authTicket = FormsAuthentication.Decrypt(Response.Cookies["COESSO"].Value);
                }
            }
            catch
            {
                // Log exception details (omitted for simplicity)
                return;
            }
            if (null == authTicket)
            {
                // Cookie failed to decrypt.
                return;
            }
            else
            {
                authTicket = FormsAuthentication.RenewTicketIfOld(authTicket);
                // Create an Identity object
                FormsIdentity id = new FormsIdentity(authTicket);
                // This principal will flow throughout the request.
                System.Security.Principal.GenericPrincipal principal = new System.Security.Principal.GenericPrincipal(id, null);
                // Attach the new principal object to the current HttpContext object
                Context.User = principal;
                Response.Cookies["COESSO"].Value = Request["ticket"].ToString();
                Response.Cookies["COESSO"].Path = "/";
                Response.Cookies["DisableInactivity"].Value = "true";
                Response.Cookies["DisableInactivity"].Path = "/";
                Response.Cookies["DisableInactivity"].Expires = DateTime.Now.AddMinutes(25);
            }
        }
    }

    // Code that runs when a new session is started
    void Session_Start(object sender, EventArgs e)
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

    void Session_End(object sender, EventArgs e)
    {
        //CambridgeSoft.COE.Framework.GUIShell.GUIShellUtilities.DoLogout();
        //this.DoLogout();
    }

    void Application_PreRequestHandlerExecute(object sender, EventArgs e)
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

    public void DoLogin()
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
            if (Request.Cookies["COESSO"] != null && !string.IsNullOrEmpty(Request.Cookies["COESSO"].Value))
            {
                userIdentifier = Request.Cookies["COESSO"].Value;
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
