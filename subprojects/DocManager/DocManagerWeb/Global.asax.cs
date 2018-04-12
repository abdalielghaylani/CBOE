using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using CambridgeSoft.COE.Framework.GUIShell;

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

    #endregion

    #region Method to be called from outside apps.

    private void Application_Start_Code()
    {
        HttpContext.Current.Application.Lock();
        HttpContext.Current.Application[Constants.AppName] = "DOCMANAGER";
        //The following settings will (someday) moved to an App configuration file or will be provided by the config services.
        //This can also be moved to be a session var, so we can set this by user, not for the entire app.
        HttpContext.Current.Application[Constants.AppPagesTitle] = "DOCMANAGER";
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
        HttpContext.Current.Application[GUIShellTypes.Embed] = "COEDOCMANAGER|";
        //End  Settings
        HttpContext.Current.Application.UnLock();
    }

    private void Application_End_Code()
    {
        HttpContext.Current.Application.Lock();

        HttpContext.Current.Application.UnLock();
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
        if (request["ticket"] != null)
        {
            FormsAuthenticationTicket authTicket = null;
            try
            {
                authTicket = FormsAuthentication.Decrypt(request["ticket"].ToString());
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

            // Create an Identity object
            FormsIdentity id = new FormsIdentity(authTicket);
            // This principal will flow throughout the request.
            System.Security.Principal.GenericPrincipal principal = new System.Security.Principal.GenericPrincipal(id, null);
            // Attach the new principal object to the current HttpContext object
            context.User = principal;
            response.Cookies["COESSO"].Value = request["ticket"].ToString();
            response.Cookies["COESSO"].Path = "/";
        }
        HttpContext.Current.Application.UnLock();
    }

    private void Session_Start_Code()
    {
        HttpContext.Current.Application.Lock();
        //Starts DoLogin CODE
        string userIdentifier = string.Empty;
        bool isTicket = false;
        string password = string.Empty;
        string appName = string.Empty;
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
        if (System.Configuration.ConfigurationManager.AppSettings["LogInSimulationMode"] != null && System.Configuration.ConfigurationManager.AppSettings["LogInSimulationMode"].ToString() != String.Empty)
        {
            userIdentifier = System.Configuration.ConfigurationManager.AppSettings["LogInSimulationMode"].ToString();
        }
        else
        {

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                userIdentifier = HttpContext.Current.User.Identity.Name;
                isTicket = false;
            }
            else
            {
                throw new Exception("no user name provided");
            }
        }
        CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider memProvider = new CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider();
        memProvider.Login(userIdentifier, isTicket);
        //Ends DoLogin Code
        if (System.Configuration.ConfigurationManager.AppSettings["PageControlsManager"] != null)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["PageControlsManager"].ToUpper() == "ENABLE")
                LoadControlPageSettingsObj();
        }
        HttpContext.Current.Application.UnLock();
    }

    public void Session_End_Code()
    {
        HttpContext.Current.Application.Lock();
        HttpContext.Current.Application.UnLock();
    }

    #endregion

    #region Misc Methods

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

    public void DoLogin()
    {
        string userIdentifier = string.Empty;
        bool isTicket = false;
        string password = string.Empty;
        string appName = string.Empty;
        // check to see if there is an entry for ticket in the url. if ther is then you need to go through a different root.
        if (HttpContext.Current.Application["AppName"] != null)
        {
            appName = HttpContext.Current.Application["AppName"].ToString();
            Csla.ApplicationContext.GlobalContext["AppName"] = appName;
        }
        else
        {
            if (Request.QueryString["AppName"] != null)
            {
                appName = Request.QueryString["AppName"];
                if (appName == string.Empty)
                {
                    appName = "Manager";
                }
                else
                {
                    appName = Request.QueryString["AppName"];
                }
            }
            else
            {
                appName = "DOCMANAGER";
            }
        }

        Csla.ApplicationContext.GlobalContext["AppName"] = appName;
        HttpContext.Current.Application["AppName"] = appName;


        if (HttpContext.Current.User.Identity.IsAuthenticated)
        {
            userIdentifier = HttpContext.Current.User.Identity.Name;
            isTicket = false;
        }
        else
        {
            throw new Exception("no user name provided");
        }
        CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider memProvider = new CambridgeSoft.COE.Framework.COESecurityService.COEMembershipProvider();
        memProvider.Login(userIdentifier, isTicket);
    }

    /// <summary>
    /// Method to load the PageSettings object that details wich control must be disable for the current logged user. 
    /// </summary>
    private void LoadControlPageSettingsObj()
    {
        //TODO: Utilities.SetCOEPageSettings();
    }

    #endregion
}
