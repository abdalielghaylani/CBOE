using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Linq;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Security.Claims;
using CambridgeSoft.COE.Framework.COESecurityService;

public partial class _Default : System.Web.UI.Page
{
    /// <summary>
    /// Page load
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">event arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        string redirectUri = ConfigurationManager.AppSettings["redirectUri"];
        if (!string.IsNullOrEmpty(redirectUri))
        {
            if (string.IsNullOrEmpty(Utilities.token))
            {
                HttpContext.Current.GetOwinContext().Authentication.Challenge(
                   new AuthenticationProperties { RedirectUri = redirectUri },
                   OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
            else
            {
                COEMembershipProvider sso = new COEMembershipProvider();
                if (sso.ValidateUser(Utilities.user, Utilities.token))
                {
                    HttpCookie authCookie = FormsAuthentication.GetAuthCookie(Utilities.user, false);
                    if (!String.IsNullOrEmpty(HttpContext.Current.Session["SSOTicket"].ToString()))
                    {
                        FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(HttpContext.Current.Session["SSOTicket"].ToString());
                        if (ticket != null)
                        {
                            FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, ticket.UserData);
                            authCookie.Value = FormsAuthentication.Encrypt(newTicket);
                        }

                        Response.Cookies.Add(authCookie);
                        Response.Cookies["COESSO"].Value = ticket.ToString();
                    }

                    Response.Redirect(this.Page.ResolveUrl("~/Forms/Public/ContentArea/Home.aspx"));
                }
                else
                {
                    Utilities.token = string.Empty;
                    Response.Redirect(this.Page.ResolveUrl("~/AccessDenied.html"));
                }
            }
        }
        else
        {
            Response.Redirect(this.Page.ResolveUrl("~/Forms/Public/ContentArea/Home.aspx"));
        }
    }
}
