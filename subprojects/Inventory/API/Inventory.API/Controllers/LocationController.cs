using Microsoft.Web.Http;
using PerkinElmer.COE.Inventory.API.Code;
using PerkinElmer.COE.Inventory.API.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Oracle.DataAccess.Client;

namespace PerkinElmer.COE.Inventory.API.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class LocationController : ApiController
    {
        private OracleConnection connection;

        protected OracleConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    connection = new OracleConnection(ConfigurationManager.ConnectionStrings["InvDB"].ConnectionString);
                    connection.Open();
                }
                return connection;
            }
        }

        protected HttpResponseMessage CreateErrorResponse(Exception ex)
        {
            var message = ex.Message;
            var statusCode = ex is IndexOutOfRangeException ?
                HttpStatusCode.NotFound :
                HttpStatusCode.InternalServerError;
            return string.IsNullOrEmpty(message) ? Request.CreateErrorResponse(statusCode, ex) : Request.CreateErrorResponse(statusCode, message, ex);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (connection != null)
                {
                    connection.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "locations")]
        [SwaggerOperation("GetLocations")]
        [SwaggerResponse(200, type: typeof(List<LocationData>))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetLocations()
        {
            HttpResponseMessage responseMessage;
            try
            {
                // TODO: Should check authentication and authorization
                var statusCode = HttpStatusCode.OK;
                // TODO: This should be moved to service and DAL
                var connectionString = ConfigurationManager.ConnectionStrings["InvDB"];
                var locations = new List<LocationData>();
                using (var command = new OracleCommand("select * from inv_locations", Connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        locations.Add(new LocationData
                        {
                            Id = (int)reader["LOCATION_ID"],
                            Name = (string)reader["LOCATION_NAME"]
                        });
                    }
                }
                responseMessage = Request.CreateResponse(statusCode, locations);
            }
            catch (Exception ex)
            {
                responseMessage = CreateErrorResponse(ex);
            }

            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }
    }
}
