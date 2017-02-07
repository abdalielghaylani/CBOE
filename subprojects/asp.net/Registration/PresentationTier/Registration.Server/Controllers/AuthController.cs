using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Xml;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    public class AuthController : RegControllerBase
    {
        [HttpPost]
        [Route("api/auth/login")]
        public HttpResponseMessage Login()
        {
            var errorMessage = new StringBuilder();
            try
            {
                var userData = Request.Content.ReadAsAsync<JObject>().Result;
                errorMessage.AppendLine(string.Format("Logging in user, {0}...", userData["username"]));
                var webClient = new WebClient();
                var loginUrl = string.Format("/COESingleSignOn/SingleSignOn.asmx/GetAuthenticationTicket?userName={0}&password=", userData["username"]);
                loginUrl = GetAbsoluteUrl(loginUrl, true);
                string token;
                errorMessage.AppendLine(string.Format("Opening {0}...", loginUrl));
                using (var loginResponse = new StreamReader(webClient.OpenRead(loginUrl + userData["password"])))
                    token = loginResponse.ReadToEnd();
                var xml = new XmlDocument();
                xml.LoadXml(token);
                token = xml.DocumentElement.FirstChild.InnerText.Trim();
                errorMessage.AppendLine(string.Format("Token: {0}", token));
                var response = Request.CreateResponse(HttpStatusCode.OK, new JObject(
                    new JProperty("data", new JObject(new JProperty("msg", "LOGIN SUCCESSFUL"))),
                    new JProperty("meta",
                        new JObject(
                            new JProperty("token", token),
                            new JProperty("profile", new JObject(new JProperty("fullName", userData["username"])))
                        )
                    )
                ));
                var cookie = new CookieHeaderValue("COESSO", token);
                cookie.Expires = DateTime.Now.AddMinutes(60);
                cookie.Domain = Request.RequestUri.Host;
                cookie.Path = "/";
                response.Headers.AddCookies(new CookieHeaderValue[] { cookie });
                return response;
            }
            catch (Exception ex)
            {
                errorMessage.AppendLine("Login failed!");
                throw new InvalidOperationException(errorMessage.ToString(), ex);
            }
        }

        [Route("api/auth/validate/{token}")]
        public HttpResponseMessage Validate(string token)
        {
            var errorMessage = new StringBuilder();
            try
            {
                errorMessage.AppendLine(string.Format("Validating token, {0}...", token));
                var webClient = new WebClient();
                var validateUrl = "/COESingleSignOn/SingleSignOn.asmx/ValidateTicket?encryptedTicket=";
                validateUrl = GetAbsoluteUrl(validateUrl, true);
                string response;
                errorMessage.AppendLine(string.Format("Opening {0}...", validateUrl));
                using (var validateResponse = new StreamReader(webClient.OpenRead(validateUrl + token)))
                    response = validateResponse.ReadToEnd();
                var xml = new XmlDocument();
                xml.LoadXml(response);
                response = xml.DocumentElement.FirstChild.InnerText.Trim();
                errorMessage.AppendLine(string.Format("Valid: {0}", response));
                var responseJobject = Request.CreateResponse(HttpStatusCode.OK, new JObject(
                    new JProperty("data", Boolean.Parse(response))
                ));
                return responseJobject;
            }
            catch (Exception ex)
            {
                errorMessage.AppendLine("Validation failed!");
                throw new InvalidOperationException(errorMessage.ToString(), ex);
            }
        }

        [Route("api/auth/renew/{token}")]
        public HttpResponseMessage Renew(string token)
        {
            var errorMessage = new StringBuilder();
            try
            {
                var userData = Request.Content.ReadAsAsync<JObject>().Result;
                errorMessage.AppendLine(string.Format("Logging in user, {0}...", userData["username"]));
                var webClient = new WebClient();
                var loginUrl = string.Format("/COESingleSignOn/SingleSignOn.asmx/GetAuthenticationTicket?userName={0}&password=", userData["username"]);
                loginUrl = GetAbsoluteUrl(loginUrl, true);
                string newToken;
                errorMessage.AppendLine(string.Format("Opening {0}...", loginUrl));
                using (var loginResponse = new StreamReader(webClient.OpenRead(loginUrl + userData["password"])))
                    newToken = loginResponse.ReadToEnd();
                var xml = new XmlDocument();
                xml.LoadXml(newToken);
                newToken = xml.DocumentElement.FirstChild.InnerText.Trim();
                errorMessage.AppendLine(string.Format("Token: {0}", newToken));
                var response = Request.CreateResponse(HttpStatusCode.OK, new JObject(
                    new JProperty("data", new JObject(new JProperty("msg", "LOGIN SUCCESSFUL"))),
                    new JProperty("meta",
                        new JObject(
                            new JProperty("token", newToken),
                            new JProperty("profile", new JObject(new JProperty("fullName", userData["username"])))
                        )
                    )
                ));
                var cookie = new CookieHeaderValue("COESSO", newToken);
                cookie.Expires = DateTime.Now.AddMinutes(60);
                cookie.Domain = Request.RequestUri.Host;
                cookie.Path = "/";
                response.Headers.AddCookies(new CookieHeaderValue[] { cookie });
                return response;
            }
            catch (Exception ex)
            {
                errorMessage.AppendLine("Login failed!");
                throw new InvalidOperationException(errorMessage.ToString(), ex);
            }
        }
    }
}
