using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COEPageControlSettingsService;
using System.IO;
using System.IO.Compression;

public class Global : System.Web.HttpApplication
{
    #region Class Events 

    void Application_Start(object sender, EventArgs e)
    {
        Application_Start_Code();
    }

    void Application_End(object sender, EventArgs e)
    {
        Application_End_Code();
    }

    void Application_Error(object sender, EventArgs e)
    {
        Application_Error_Code();
    }

    protected void Application_AcquireRequestState(object sender, EventArgs e)
    {
        Application_AcquireRequestState_Code();
    }

    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {
        Application_AuthenticateRequest_Code(this.Request, this.Response, this.Context);
    }

    protected void Session_Start(object sender, EventArgs e)
    {
        Session_Start_Code();
    }

    protected void Session_End(object sender, EventArgs e)
    {
        Session_End_Code();
    }

    void Application_PreRequestHandlerExecute(object sender, EventArgs e)
    {
        HttpApplication app = sender as HttpApplication;

        if (app != null) //Coverity Fix 
        {
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
        // Once ChemBioViz has a COEConfiguration uncomment the lines below to enable/disable COEPageControl from configuration
        //if (!string.IsNullOrEmpty(CambridgeSoft.COE.Framework.Common.FrameworkUtils.GetAppConfigSetting(GUIShellUtilities.GetApplicationName(), "MISC", "PageControlsManager")))
        //    if (CambridgeSoft.COE.Framework.Common.FrameworkUtils.GetAppConfigSetting("CHEMBIOVIZ", "MISC", "PageControlsManager").ToUpper() == "ENABLE")
                Utilities.SetCOEPageSettings(false); 
    }
    #endregion

    #region Method to be called from outside apps.

    private void Application_Start_Code()
    {
        // Code that runs on application startup
        HttpContext.Current.Application.Lock();
        HttpContext.Current.Application[Constants.AppName] = "CHEMBIOVIZ";
        //The following settings will (someday) moved to an App configuration file or will be provided by the config services.
        //This can also be moved to be a session var, so we can set this by user, not for the entire app.
        HttpContext.Current.Application[Constants.AppPagesTitle] = Resources.Resource.ChemBioViz_Page_Title;
        //HttpContext.Current.Application[Constants.PageControlsSettingsPath] = "~/Forms/ContentArea/Settings/PageControl";
        HttpContext.Current.Application[Constants.ImagesFolder] = "~/App_Themes/{0}/Images/";
        HttpContext.Current.Application[Constants.ImagesLibName] = "IconLibrary";
        HttpContext.Current.Application[Constants.ImagesSubType] = "Aqua";
        HttpContext.Current.Application[Constants.ImagesFormat] = "PNG";
        HttpContext.Current.Application[Constants.ImagesSize] = "16";
        HttpContext.Current.Application[GUIShellTypes.Themes] = "BLUE|GREEN|"; //Separed by |
        HttpContext.Current.Application[GUIShellTypes.Embed] = false; //Separed by |
        //Setting to popUp a new clean window or not according the IE (*) version. (*) Not tested in non IE browsers.
        HttpContext.Current.Application[GUIShellTypes.EnablePopUpForBrowserVersions] = "-1"; //Separed by |. -1 means don't open popup for any IE version.
        HttpContext.Current.Application[Constants.StructuresCount] = "567,498";
        HttpContext.Current.Application[GUIShellTypes.Embed] = "COEREGISTRATION|";
        //End  Settings
        HttpContext.Current.Application.UnLock();
    }

    private void Application_End_Code()
    {
        if(HttpContext.Current != null)
        {
            HttpContext.Current.Application.Lock();
            HttpContext.Current.Application.UnLock();
        }
    }

    private void Application_Error_Code()
    {
        if (HttpContext.Current != null)
        {
            HttpContext.Current.Application.Lock();
            Exception exception = null;

            try
            {
                exception = this.Server.GetLastError().GetBaseException();

                //Unhandled exeptions must display a generic message and be logged.
                Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.ExceptionPolicy.HandleException(
                        exception,
                        GUIShellTypes.PoliciesNames.UnknownExceptionPolicy.ToString());
            }
            finally
            {
                if (exception is HttpRequestValidationException)
                    Server.Transfer(Constants.GetPublicContentAreaFolder() + "Messages.aspx?MessageCode=" +
                                        GUIShellTypes.MessagesCode.InvalidHttpRequest +
                                        "&" + GUIShellTypes.RequestVars.MessagesButtonType.ToString() +
                                        "=" + GUIShellTypes.MessagesButtonType.Back.ToString());
                else
                    Server.Transfer(Constants.GetPublicContentAreaFolder() + "Messages.aspx?MessageCode=" +
                                        GUIShellTypes.MessagesCode.Unknown.ToString() +
                                        "&" + GUIShellTypes.RequestVars.MessagesButtonType.ToString() +
                                        "=" + GUIShellTypes.MessagesButtonType.Back.ToString());
            }
            HttpContext.Current.Application.UnLock();
        }
    }

    private void Application_AcquireRequestState_Code()
    {
        HttpContext.Current.Application.Lock();
        if (HttpContext.Current.Request.Url.AbsoluteUri.Contains(".axd") || HttpContext.Current.Request.Url.AbsoluteUri.Contains(".asmx"))
        {
            return;
        }
        CambridgeSoft.COE.Framework.Common.WebUtils.SetCslaPrincipal();
        Csla.ApplicationContext.GlobalContext["USER_PERSONID"] = HttpContext.Current.Session["USER_PERSONID"];
        HttpContext.Current.Application.UnLock();
    }

    private void Application_AuthenticateRequest_Code(HttpRequest request, HttpResponse response, HttpContext context)
    {
        HttpContext.Current.Application.Lock();
        if(!Request.IsAuthenticated)
        {
            FormsAuthenticationTicket authTicket = null;

            try
            {
                if(Request["ticket"] != null)
                {
                    authTicket = FormsAuthentication.Decrypt(Request["ticket"]);
                }
                else if(Request.Cookies["COESSO"] != null && !string.IsNullOrEmpty(Request.Cookies["COESSO"].Value))
                {
                    authTicket = FormsAuthentication.Decrypt(Request.Cookies["COESSO"].Value);
                }
            }
            catch
            {
                // Log exception details (omitted for simplicity)
                return;
            }
            if(null == authTicket)
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
        HttpContext.Current.Application.UnLock();
    }

    private void Session_Start_Code()
    {
        HttpContext.Current.Application.Lock();
        //Starts DoLogin CODE
        DoLogin();
        //Ends DoLogin Code
        if (System.Configuration.ConfigurationManager.AppSettings["PageControlsManager"] != null)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["PageControlsManager"].ToUpper() == "ENABLE")
                LoadControlPageSettingsObj();
        }
        Utilities.CleanCache();
        if (Request["ticket"] == null && Request.Headers["Cookie"] != null && Request.Headers["Cookie"].IndexOf("ASP.NET_SessionId") >= 0 && (Request.Cookies["DisableInactivity"] == null || string.IsNullOrEmpty(Request.Cookies["DisableInactivity"].Value)) && !Request.Url.Query.Contains("Inactivity=true"))
        {
            DoLogout();
            Response.Redirect("/COEManager/Forms/Public/ContentArea/Login.aspx?Inactivity=true&ReturnURL=" + HttpContext.Current.Request.Url.ToString());
        }
        HttpContext.Current.Application.UnLock();
    }

    public void Session_End_Code()
    {
        if(HttpContext.Current != null)
        {
            HttpContext.Current.Application.Lock();
            HttpContext.Current.Application.UnLock();
        }
    }

    #endregion

    #region Misc Methods
    private void DoLogout()
    {
        CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider memProvider = new CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider();
        memProvider.LogOut();
    }

    public void DoLogin2()
    {
        string userName = "CSSADMIN";
        string password = "CSSADMIN";
        string appName = "ADMINISTRATION";
        Csla.ApplicationContext.GlobalContext["AppName"] = appName;
        HttpContext.Current.Application["AppName"] = appName;
        CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider memProvider = new CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider();
        memProvider.ValidateUser(userName, password);
    }

    /// <summary>
    /// Method to load the PageSettings object that details which control must be disable for the current logged user. 
    /// </summary>
    private void LoadControlPageSettingsObj()
    {
        Utilities.SetCOEPageSettings(false);
    }

    public void DoLogin()
    {
        string appName = string.Empty;
        bool isTicket = true;
        string userIdentifier = string.Empty;
        //check to see if there is an entry for ticket in the url. if ther is then you need to go through a different root.

        if(HttpContext.Current.Application["AppName"] != null)
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


        if(!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["LogInSimulationMode"]))
        {
            userIdentifier = System.Configuration.ConfigurationManager.AppSettings["LogInSimulationMode"];
            isTicket = false;
        }
        else
        {
            if(Request.Cookies["COESSO"] != null && !string.IsNullOrEmpty(Request.Cookies["COESSO"].Value))
            {
                userIdentifier = Request.Cookies["COESSO"].Value;
                isTicket = true;
            }
            else
            {
                throw new Exception("no user name provided");
            }
        }

        if(!string.IsNullOrEmpty(userIdentifier))
        {
            CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider memProvider = new CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider();
            if(!memProvider.Login(userIdentifier, isTicket))
            {
                throw new Exception("Unable to Login");
            }
        }
    }
    #endregion
}
