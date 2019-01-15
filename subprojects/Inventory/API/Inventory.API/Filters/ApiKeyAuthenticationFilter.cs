using Microsoft.Ajax.Utilities;
using PerkinElmer.COE.Inventory.API.Code;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using WebApi.AuthenticationFilter;

namespace PerkinElmer.COE.Inventory.API.Filters
{
    /// <summary>
    /// ApiKeyAuthenticationFilter
    /// </summary>
    public class ApiKeyAuthenticationFilter : AuthenticationFilterAttribute
    {
        /// <summary>
        /// OnAuthentication
        /// </summary>
        /// <param name="context"></param>
        public override void OnAuthentication(HttpAuthenticationContext context)
        {
            if (!Authenticate(context))
            {
                context.ErrorResult = new StatusCodeResult(HttpStatusCode.Unauthorized,
                    context.Request);
            }
        }

        private bool Authenticate(HttpAuthenticationContext context)
        {
            // Get the value for the "api-key" header key
            var apikey = context.Request?
                .Headers?
                .SingleOrDefault(x => x.Key == "api-key")
                .Value?
                .FirstOrDefault();

            if (!apikey.IsNullOrWhiteSpace())
            {
                return SecurityHelper.IsValidToken(ConfigurationManager.AppSettings["AuthenticationSecret"], apikey); // Is authenticated
            }

            return false;
        }
    }
}