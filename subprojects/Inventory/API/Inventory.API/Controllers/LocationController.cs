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
using PerkinElmer.COE.Inventory.Model;
using PerkinElmer.COE.Inventory.DAL;
using Swashbuckle.Swagger.Annotations;

namespace PerkinElmer.COE.Inventory.API.Controllers
{
    /// <summary>
    /// The location controller
    /// </summary>
    [ApiVersion(Consts.apiVersion)]
    public class LocationController : InvApiController
    {
        private LocationDAL _locationDAL;

        /// <summary>
        /// DAL for locations
        /// </summary>
        protected LocationDAL locationDAL
        {
            get
            {
                if (_locationDAL == null) {
                    _locationDAL = new LocationDAL();
                }
                return _locationDAL;
            }
        }

        /// <summary>
        /// Get all locations
        /// </summary>
        /// <returns>List of locations</returns>
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

                var locations = new List<LocationData>();
                foreach (var location in locationDAL.GetLocations())
                {
                    locations.Add(location);
                }
                responseMessage = Request.CreateResponse(statusCode, locations);
            }
            catch (Exception ex)
            {
                responseMessage = CreateErrorResponse(ex);
            }

            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }

        /// <summary>
        /// Get location by id
        /// </summary>
        /// <param name="id">Location id</param>
        /// <returns>Location</returns>
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

                LocationData location = locationDAL.GetLocation(id);
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
