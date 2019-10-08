using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Reflection;

public partial class ChangePasswordUC : System.Web.UI.UserControl
{
    COEIdentity myIdentity;
    #region General Event handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
        }
        else
        {
        }
    }

    protected void CancelClicked(object sender, EventArgs e)
    {
        Server.Transfer(this.Page.ResolveUrl(Constants.PublicContentAreaFolder) + Resources.Resource.COE_HOME, false);
    }

    protected void ContinueClicked(object sender, EventArgs e)
    {
        Server.Transfer(this.Page.ResolveUrl(Constants.PublicContentAreaFolder) + Resources.Resource.COE_HOME, false);
    }

    protected override void OnInit(EventArgs e)
    {
        COEPrincipal principal = (COEPrincipal)Csla.ApplicationContext.User;
        myIdentity = (COEIdentity)principal.Identity;
        if (myIdentity.IsLDAP == true && myIdentity.IsExempt == false)
        {//replace password control with text control
            ChangePassword1.Visible = false;
            LDAPMessagePanel.Visible = true;
            LDAPEnabledMessage.Visible = true;
            LDAPMessageDoneButton.Visible = true;

        }
        else if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["redirectUri"]))
        {
            ChangePassword1.Visible = false;
            LDAPMessagePanel.Visible = true;
            LDAPEnabledMessage.Visible = true;
            LDAPMessageDoneButton.Visible = true;
            LDAPEnabledMessage.Text = "Password cannot be changed here because Azure authentication is enabled on the server.<br /> " +
                    "Please contact the system Administrator for instructions on how to change the Azure password.";
        }
        else
        {
            ChangePassword1.Visible = true;
            LDAPMessagePanel.Visible = false;
            LDAPEnabledMessage.Visible = false;
            LDAPMessageDoneButton.Visible = false;
        }

        this.LDAPMessageDoneButton.ButtonClicked += new EventHandler<EventArgs>(DoneImageButton_ButtonClicked);

        base.OnInit(e);
    }


    void DoneImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Server.Transfer(this.Page.ResolveUrl(Constants.PublicContentAreaFolder) + Resources.Resource.COE_HOME, false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void cusCustom_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        if (e.Value.Equals(myIdentity.Password))
            e.IsValid = true;
        else
            e.IsValid = false;
    }
   
    #endregion
}
