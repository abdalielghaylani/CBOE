using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using CambridgeSoft.COE.Framework.GUIShell;
using System.IO;
using CambridgeSoft.COE.Framework.COEPageControlSettingsService;
using System.IO.Compression;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

public class Global : System.Web.HttpApplication
{
    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
        HttpContext.Current.Application.Lock();
        HttpContext.Current.Application[Constants.AppName] = "MANAGER";
        //The following settings will (someday) moved to an App configuration file or will be provided by the config services.
        //This can also be moved to be a session var, so we can set this by user, not for the entire app.
        HttpContext.Current.Application[Constants.AppPagesTitle] = Resources.Resource.DataViewManager_Page_Title;
        //HttpContext.Current.Application[Constants.PageControlsSettingsPath] = "~/Forms/ContentArea/Settings/PageControl";
        HttpContext.Current.Application[Constants.ImagesFolder] = "/App_Themes/{0}/Images/";
        HttpContext.Current.Application[Constants.ImagesLibName] = "IconLibrary";
        HttpContext.Current.Application[Constants.ImagesSubType] = "Aqua";
        HttpContext.Current.Application[Constants.ImagesFormat] = "PNG";
        HttpContext.Current.Application[Constants.ImagesSize] = "24";
        HttpContext.Current.Application[GUIShellTypes.Themes] = "BLUE|GREEN"; //Separed by |
        HttpContext.Current.Application[GUIShellTypes.Embed] = "DataViewManager|SecurityManager";
        ////Header and Footer dynamic load settings.
        //HttpContext.Current.Application[GUIShellTypes.UseDefaultHeader] = GUIShellTypes.DefaultHeader.No;
        //HttpContext.Current.Application[GUIShellTypes.CustomHeaderName] = "SimpleCustomControlsLib.dll";
        //HttpContext.Current.Application[GUIShellTypes.CustomHeaderType] = GUIShellTypes.HeaderControlSupportedTypes.Composite;
        //HttpContext.Current.Application[GUIShellTypes.CustomHeadersPerPage] = "ASP.FORMS_PUBLIC_CONTENTAREA_HOME_ASPX,CambridgeSoft.COE.Framework.GUIShell.CustomControls.Header|ASP.FORMS_PUBLIC_CONTENTAREA_LOGIN_ASPX,CambridgeSoft.COE.Framework.GUIShell.CustomControls.Header|";

        ////Composite/WebControl footer. Still needed one more parameter to identify the assembly name
        //HttpContext.Current.Application[GUIShellTypes.UseDefaultFooter] = GUIShellTypes.DefaultFooter.No;
        //HttpContext.Current.Application[GUIShellTypes.CustomFooterName] = "SimpleCustomControlsLib.dll";
        //HttpContext.Current.Application[GUIShellTypes.CustomFooterType] = GUIShellTypes.FooterControlSupportedTypes.Composite; //For now all footer must be the same type.
        //HttpContext.Current.Application[GUIShellTypes.CustomFootersPerPage] = "ASP.FORMS_PUBLIC_CONTENTAREA_HOME_ASPX,CambridgeSoft.COE.Framework.GUIShell.CustomControls.Footer|ASP.FORMS_PUBLIC_CONTENTAREA_HELP_ASPX,CambridgeSoft.COE.Framework.GUIShell.CustomControls.Footer_Print|";

        //End  Settings
        HttpContext.Current.Application.UnLock();
    }

    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown
    }

    void Application_Error(object sender, EventArgs e)
    {
        Response.Filter = null;

        //Coverity Fixes: CBOE-313
        //Unhandled exeptions must display a generic message and be logged.
        Exception lastError = this.Server.GetLastError();
        if (lastError != null)
            Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.ExceptionPolicy.HandleException(lastError.GetBaseException(), GUIShellTypes.PoliciesNames.UnknownExceptionPolicy.ToString());
        
        Server.Transfer(Constants.PublicContentAreaFolder + "Messages.aspx?MessageCode=" +
                            GUIShellTypes.MessagesCode.Unknown.ToString() +
                            "&" + GUIShellTypes.RequestVars.MessagesButtonType.ToString() +
                            "=" + GUIShellTypes.MessagesButtonType.Back.ToString());
    }

    protected void Application_AcquireRequestState(object sender, EventArgs e)
    {
        if (HttpContext.Current.Request.Url.AbsoluteUri.Contains(".axd") || HttpContext.Current.Request.Url.AbsoluteUri.Contains(".asmx"))
        {
            return;
        }
        
        if (HttpContext.Current.Session == null)
        {
            return;
        }
        
        CambridgeSoft.COE.Framework.Common.WebUtils.SetCslaPrincipal();
        Csla.ApplicationContext.GlobalContext["USER_PERSONID"] = HttpContext.Current.Session["USER_PERSONID"];
        if (HttpContext.Current.User.Identity.IsAuthenticated)
        {
            if (!string.IsNullOrEmpty(CambridgeSoft.COE.Framework.Common.FrameworkUtils.GetAppConfigSetting("MANAGER", "MISC", "PageControlsManager")))
                if (CambridgeSoft.COE.Framework.Common.FrameworkUtils.GetAppConfigSetting("MANAGER", "MISC", "PageControlsManager").ToUpper() == "ENABLE")
                    Utilities.SetCOEPageSettings(false);
        }
    }

    protected void Application_AuthenticateRequest(Object sender, EventArgs e)
    {
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
    }

    void Application_PreRequestHandlerExecute(object sender, EventArgs e)
    {
        HttpApplication app = sender as HttpApplication;
        if (app == null)
            return;

        if(!HttpContext.Current.Request.Url.AbsoluteUri.Contains(".aspx") || app.Request["HTTP_X_MICROSOFTAJAX"] != null)
            return;
        
        string acceptEncoding = app.Request.Headers["Accept-Encoding"];
        Stream prevUncompressedStream = app.Response.Filter;

        if(acceptEncoding == null || acceptEncoding.Length == 0)
            return;

        acceptEncoding = acceptEncoding.ToLower();

        if(acceptEncoding.Contains("deflate") || acceptEncoding == "*")
        {
            // defalte
            app.Response.Filter = new DeflateStream(prevUncompressedStream,
                CompressionMode.Compress);
            app.Response.AppendHeader("Content-Encoding", "deflate");
        }
        else if(acceptEncoding.Contains("gzip"))
        {
            // gzip
            app.Response.Filter = new GZipStream(prevUncompressedStream,
                CompressionMode.Compress);
            app.Response.AppendHeader("Content-Encoding", "gzip");
        }
    }

    void Session_Start(object sender, EventArgs e)
    {
        string redirectUri = ConfigurationManager.AppSettings["redirectUri"];
        if (!string.IsNullOrEmpty(redirectUri))
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.Current.GetOwinContext().Authentication.Challenge(
                   new AuthenticationProperties { RedirectUri = redirectUri },
                   OpenIdConnectAuthenticationDefaults.AuthenticationType);
                HttpCookie azureCookie = new HttpCookie("CS_SEC_Azure");
                azureCookie.Value = "Azure";
                Response.Cookies.Add(azureCookie);
            }
            DoLogin();
        }
        else
        {
            DoLogin();
            if (Request["ticket"] == null && Request.Headers["Cookie"] != null && Request.Headers["Cookie"].IndexOf("ASP.NET_SessionId") >= 0 && (Request.Cookies["DisableInactivity"] == null || string.IsNullOrEmpty(Request.Cookies["DisableInactivity"].Value)) && !Request.Url.Query.Contains("Inactivity=true"))
            {
                DoLogout();
                Response.Redirect("/COEManager/Forms/Public/ContentArea/Login.aspx?Inactivity=true&ReturnURL=" + RemoveLoginPageFromReturnUrl(HttpContext.Current.Request.Url.ToString()));
            }
        }
    }

    private string RemoveLoginPageFromReturnUrl(string currentURL)
    {
        if (currentURL.IndexOf("Login.aspx") > 0 && Request.QueryString["ReturnURL"]!=null && !string.IsNullOrEmpty(Request.QueryString["ReturnURL"]))
        {
            // Check if ReturnURL is already encoded
            if (Server.UrlEncode(Request.QueryString["ReturnURL"]) == Request.QueryString["ReturnURL"])
            {
                return Request.QueryString["ReturnURL"];
            }
            else
            {
                return Server.UrlEncode(Request.QueryString["ReturnURL"]);
            }
        }
        else
        {
            return currentURL;
        }
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


        if(Request.Cookies["COESSO"] != null && !string.IsNullOrEmpty(Request.Cookies["COESSO"].Value))
        {
            userIdentifier = Request.Cookies["COESSO"].Value;
            isTicket = true;
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

    private void DoLogout()
    {
        CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider memProvider = new CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider();
        memProvider.LogOut();
    }

    void Session_End(object sender, EventArgs e)
    {
        /*Server.Transfer(Constants.PublicContentAreaFolder + "Messages.aspx" +
                            "?" + GUIShellTypes.RequestVars.MessageCode.ToString() + 
                            "=" + Constants.MessagesCode.SessionTimeOut.ToString() +
                            "&" + GUIShellTypes.RequestVars.MessagesButtonType.ToString() +
                            "=" + GUIShellTypes.MessagesButtonType.Login.ToString());*/
    }   
}
