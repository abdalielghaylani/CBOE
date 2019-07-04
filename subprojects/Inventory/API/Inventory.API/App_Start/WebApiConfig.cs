using Microsoft.Web.Http.Routing;
using PerkinElmer.COE.Inventory.API.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Routing;

namespace PerkinElmer.COE.Inventory.API
{
    /// <summary>
    /// Web API configuration class.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Web API registration method
        /// </summary>
        /// <param name="config">Http configuration</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            var constraintResolver = new DefaultInlineConstraintResolver();
            constraintResolver.ConstraintMap.Add("apiVersion", typeof(ApiVersionRouteConstraint));
            config.MapHttpAttributeRoutes(constraintResolver);
            config.AddApiVersioning();

            config.Routes.MapHttpRoute(
            name: "Swagger UI",
            routeTemplate: "",
            defaults: null,
            constraints: null,
            handler: new Swashbuckle.Application.RedirectHandler(Swashbuckle.Application.SwaggerDocsConfig.DefaultRootUrlResolver, "swagger/ui/index"));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v{version:apiVersion}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // config.Filters.Add(new ApiKeyAuthenticationFilter());
        }
    }
}
