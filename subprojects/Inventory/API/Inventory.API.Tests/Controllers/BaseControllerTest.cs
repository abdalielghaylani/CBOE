using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace PerkinElmer.COE.Inventory.API.Tests.Controllers
{
    public class BaseControllerTest
    {
        protected TestInventoryDBContext Context = new TestInventoryDBContext();

        protected static void SetupControllerForTests(ApiController controller, string controllerName, HttpMethod httpMethod)
        {
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(httpMethod, "http://localhost/api/" + controllerName);
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", controllerName } });

            controller.ControllerContext = new HttpControllerContext(config, routeData, request);
            controller.Request = request;
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
        }
    }
}
