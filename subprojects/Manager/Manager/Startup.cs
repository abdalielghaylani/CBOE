using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

[assembly: OwinStartup(typeof(Manager.Startup))]

namespace Manager
{
    /// <summary>
    /// Startup
    /// </summary>
    public partial class Startup
    {
        // The Client ID is used by the application to uniquely identify itself to Azure AD.
        string clientId = ConfigurationManager.AppSettings["ClientId"];
        // RedirectUri is the URL where the user will be redirected to after they sign in.
        string redirectUri = ConfigurationManager.AppSettings["RedirectUri"];
        // Tenant is the tenant ID (e.g. contoso.onmicrosoft.com, or 'common' for multi-tenant)
        static string tenant = ConfigurationManager.AppSettings["Tenant"];
        // Authority is the URL for authority, composed by Azure Active Directory v2 endpoint 
        // and the tenant name (e.g. https://login.microsoftonline.com/contoso.onmicrosoft.com/v2.0)
        string authority = string.Empty;
        // Scopes are the specific permissions we are requesting for the application.
        string scopes = ConfigurationManager.AppSettings["Scopes"];
        // ClientSecret is a password associated with the application in the authority. 
        // It is used to obtain an access token for the user on server-side apps.
        string clientSecret = ConfigurationManager.AppSettings["ClientSecret"];

        string resourceID = ConfigurationManager.AppSettings["ResourceId"];

        string postLogoutRedirectUri = ConfigurationManager.AppSettings["PostLogoutRedirectUri"];

        /// <summary>
        /// Configuration
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            if (!String.IsNullOrEmpty(tenant))
            {
                authority = String.Format(System.Globalization.CultureInfo.InvariantCulture,
                    ConfigurationManager.AppSettings["Authority"], tenant);

                app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

                app.UseCookieAuthentication(new CookieAuthenticationOptions());
                app.UseOpenIdConnectAuthentication(
                    new OpenIdConnectAuthenticationOptions
                    {
                        // Sets the ClientId, authority, RedirectUr,ClientSecret as obtained from web.config
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        Authority = authority,
                        RedirectUri = redirectUri,
                        PostLogoutRedirectUri = postLogoutRedirectUri,
                        Scope = OpenIdConnectScope.OpenIdProfile,
                        ResponseType = OpenIdConnectResponseType.CodeIdToken,
                        UseTokenLifetime = true,
                        Caption = null,
                        TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateIssuer = false // This is a simplification
                        },
                        // OpenIdConnectAuthenticationNotifications configures OWIN to send notification of failed authentications to OnAuthenticationFailed method
                        Notifications = new OpenIdConnectAuthenticationNotifications
                        {
                            AuthenticationFailed = OnAuthenticationFailed,
                            AuthorizationCodeReceived = OnAuthorizationCodeReceived
                        }
                    }
                );
            }
        }

        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            return Task.FromResult(0);
        }

        /// <summary>
        /// OnAuthorizationCodeReceived
        /// </summary>
        /// <param name="notification"></param>
        /// <returns>Task</returns>
        private async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedNotification notification)
        {
            var idClient = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithRedirectUri(redirectUri)
                .WithClientSecret(clientSecret)
                .Build();

            string[] scopes = this.scopes.Split(new char[] { ' ' });

            AuthenticationResult result = await idClient.AcquireTokenByAuthorizationCode(
                scopes, notification.Code).ExecuteAsync();
            Utilities.token = result.AccessToken;
            Utilities.user = result.Account.Username.Split('@')[0];
        }
    }
}
