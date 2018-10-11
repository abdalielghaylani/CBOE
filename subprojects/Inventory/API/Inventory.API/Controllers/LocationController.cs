using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Web.Http;
using Oracle.DataAccess.Client;
using PerkinElmer.COE.Inventory.API.Code;
using PerkinElmer.COE.Inventory.API.Models;
using Swashbuckle.Swagger.Annotations;

namespace PerkinElmer.COE.Inventory.API.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class LocationController : InvApiController
    {
        private LocationData BuildLocationData(OracleDataReader reader)
        {
            return new LocationData
            {
                Id = (int)reader["location_id"],
                Name = (string)reader["location_name"]
            };
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "locations")]
        [SwaggerOperation("Locations")]
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
                        locations.Add(BuildLocationData(reader));
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

        [HttpGet]
        [Route(Consts.apiPrefix + "locations/{id}")]
        [SwaggerOperation("Locations")]
        [SwaggerResponse(200, type: typeof(LocationData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetLocationById(int id)
        {
            HttpResponseMessage responseMessage;
            try
            {
                // TODO: Should check authentication and authorization
                var statusCode = HttpStatusCode.OK;
                LocationData location = null;
                using (var command = new OracleCommand("select * from inv_locations where location_id=:id", Connection))
                {
                    command.Parameters.Add(new OracleParameter("id", OracleDbType.Int32, id, ParameterDirection.Input));
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            location = BuildLocationData(reader);
                        }
                    }
                }
                if (location == null)
                    throw new IndexOutOfRangeException(string.Format("Cannot find the location, {0}", id));
                responseMessage = Request.CreateResponse(statusCode, location);
            }
            catch (Exception ex)
            {
                responseMessage = CreateErrorResponse(ex);
            }

            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }
    }
}
