using System;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COEDataViewService;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using System.Web;
using System.Configuration;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

public partial class Forms_Public_ContentArea_Login : GUIShellPage
{
    #region Events Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        string redirectUri = ConfigurationManager.AppSettings["redirectUri"];
        if (!string.IsNullOrEmpty(redirectUri))
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.Current.GetOwinContext().Authentication.Challenge(
                   new AuthenticationProperties { RedirectUri = redirectUri },
                   OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
            else
            {

            }
        }
        else
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            //if (!Page.IsPostBack)
            //{
            this.SetControlsAttributtes();
            //}
            this.SubscribeToEventsInLeftPanel();
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
    }

    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        base.OnInit(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region GUIShell Methods

    /// <summary>
    /// This method sets all the controls attributtes as Text, etc...
    /// </summary>
    protected override void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Master.SetPageTitle(Resources.Resource.Login_Page_Tiltle);
        this.Master.DisableLoggedOnMenus();
        //don't show the header and footer for the login page
        this.DisableFooter();
        this.DisableHeader();
        this.Page.Title = Resources.Resource.COEManager_Page_Title;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// This method is to subscribe to a public method in the customized control 
    /// to handle it here in a defined method.
    /// </summary>
    private void SubscribeToEventsInLeftPanel()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion
}
