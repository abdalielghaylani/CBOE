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
using CambridgeSoft.COE.Framework.Controls;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COELoggingService;

public partial class Forms_Public_UserControls_Login : System.Web.UI.UserControl
{

    protected void Page_Load(object sender, EventArgs e)
    {
        Control ctl = Login1.FindControl("LoginButton");
        Login1.Attributes.Add("onkeypress", String.Format("javascript:return WebForm_FireDefaultButton(event, '{0}')", ctl.ClientID));
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributes();
            this.InactivityMessage.Visible = !string.IsNullOrEmpty(Page.Request.QueryString[Constants.InactivityURLParam]) && bool.Parse(Page.Request.QueryString[Constants.InactivityURLParam]);
        }
    }

    private void SetControlsAttributes()
    {
        this.Login1.TitleText = Resources.Resource.Login_Title_Text;
        this.Login1.LoginButtonText = Resources.Resource.Login_Button_Text;
        ((Forms_Master_MasterPage)this.Page.Master).SetDefaultFocus(this.Login1.ClientID);
        this.InactivityMessage.Text = Resources.Resource.InactivityMessage_Error_Text;
        this.Login1.Focus();
    }

    /// <summary>
    /// add client script, add style sheets etc
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreRender(EventArgs e)
    {
        //Login has to be outside of the theme folder because it's styles are not compatible 
        HtmlLink loginStyle = new HtmlLink();
        loginStyle.Attributes.Add("rel", "stylesheet");
        loginStyle.Href = Utilities.CssRelativeFolder("Login" + this.Page.StyleSheetTheme) + "/loginStyle.css";
        loginStyle.Attributes.Add("type", "text/css");
        this.Page.Header.Controls.Add(loginStyle);
        base.OnPreRender(e);
    }

    protected void OnLoggedInEvent(object sender, EventArgs e)
    {
        HttpCookie authCookie = FormsAuthentication.GetAuthCookie(Login1.UserName, false);
        FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(HttpContext.Current.Session["SSOTicket"].ToString());
        //Coverity Fixes: CBOE-313
        if (ticket != null)
        {
            FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, ticket.UserData);
            authCookie.Value = FormsAuthentication.Encrypt(newTicket);
        } 

        Response.Cookies.Add(authCookie);
        if ((Request.QueryString["ReturnURL"] != null) && (Convert.ToString(Request.QueryString["ReturnURL"]).ToUpper().Contains(".ASPX")))
        {
            //CBOE-708 : Object reference error is displayed ... when session timeout occurred. ASV 260413.
			//sending return url to DestinationPageUrl as parameter, so that return url will
			//open in new window and DestinationPageUrl will continue execution.
            string ReturnURL = Request.QueryString.ToString();
            ReturnURL = ReturnURL.Contains("logoff.aspx") ? "" : "?"+ReturnURL;
            Response.Redirect(Login1.DestinationPageUrl + ReturnURL);
        }
        else
        {
            Response.Redirect(Login1.DestinationPageUrl);
        }
    }


    protected void DisplayError(object sender, EventArgs e)
    {
        COELog _coeLog = COELog.GetSingleton("COESecurity");
        _coeLog.LogStart("LoginError", 0, System.Diagnostics.SourceLevels.Error);
        try
        {

            if (HttpContext.Current.Session["LOGIN_FAILURE_MODE"].ToString() != null && HttpContext.Current.Session["LOGIN_FAILURE_MODE"].ToString() != string.Empty)
            {
                if (HttpContext.Current.Session["LOGIN_FAILURE_MODE"].ToString() == "ErrorServerName")
                    this.Login1.FailureText = "An Error occurred due to wrong Server Name.";
                else
                    this.Login1.FailureText = "Login Failed.";
                    ((WebControl)((Literal)Login1.FindControl("FailureText")).Parent).ToolTip = HttpContext.Current.Session["LOGIN_FAILURE_MODE"].ToString();
                    _coeLog.Log(HttpContext.Current.Session["LOGIN_FAILURE_MODE"].ToString(), 0, System.Diagnostics.SourceLevels.Error);
            }
            else
            {
                this.Login1.FailureText = "Login Failed. Username and/or password is incorrect";
                ((WebControl)((Literal)Login1.FindControl("FailureText")).Parent).ToolTip = "Error Login Failed:\n* Make sure you typed your username and password correctly\n* Passwords may be case sensitve\n* Make sure you are using a valid user account";
                _coeLog.Log(this.Login1.FailureText, 0, System.Diagnostics.SourceLevels.Error);
            }
        }
        catch (System.Exception ex)
        {
            this.Login1.FailureText = "Login Failed.";
            if (HttpContext.Current.Trace.IsEnabled)
                this.Login1.FailureText += "<br>" + ex.InnerException.Message;
            ((WebControl)((Literal)Login1.FindControl("FailureText")).Parent).ToolTip = "Error Login Failed:\n* Make sure you typed your username and password correctly\n* Passwords may be case sensitve\n* Make sure you are using a valid user account";
            _coeLog.Log(this.Login1.FailureText, 0, System.Diagnostics.SourceLevels.Error);

        }
        finally
        {
            _coeLog.LogEnd("LoginError", 0, System.Diagnostics.SourceLevels.Error);
        }
    }
}
