using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using CambridgeSoft.COE.Registration.Services;
using PerkinElmer.COE.Registration.Server.Code;
using Microsoft.Web.Http;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class BatchController : RegControllerBase
    {
        /// <summary>
        /// Returns the list of all batches
        /// </summary>
        /// <remarks>Returns the list of all batches corresponding to the specified parameters
        /// </remarks>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid request</response>
        /// <response code="0">Unexpected error</response>
        [HttpGet]
        [Route(Consts.apiPrefix + "batches")]
        [SwaggerOperation("GetBatchs")]
        [SwaggerResponse(200, type: typeof(List<JObject>))]
        public async Task<IHttpActionResult> GetBatchs(int? recordId = null, int? skip = null, int? count = null, string sort = null)
        {
            CheckAuthentication();
            // Get the list of batches asynchronously
            // And return the list
            List<JObject> data = null;
            return Ok(data);
        }

        // TODO: Add comments for summary, remarks, and response codes.
        // Response code should include
        // - 404 for id not found error
        // - 400 for bad request
        // etc.
        // TODO: Return type should be properly defined in Models directory.
        // Refer to Hitlist model as an example.
        // Swagger code-gen can be used to generate model objects.
        // The model does not have to be complete up-front.
        // Add just enough members such as ID to get started.
        [HttpGet]
        [Route(Consts.apiPrefix + "batches/{id}")]
        [SwaggerOperation("GetBatch")]
        [SwaggerResponse(200, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetBatch(int id)
        {
            CheckAuthentication();
            // TODO: This has to be implemented.
            // The method to return batch data from DAL should be an asynchronous method.
            if (id < 0) return NotFound();
            return Ok(new JObject());
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "batches")]
        [SwaggerOperation("CreateBatch")]
        [SwaggerResponse(201, type: typeof(JObject))]
        public async Task<IHttpActionResult> CreateBatch(JObject batch)
        {
            CheckAuthentication();
            var id = -1;
            return Created<JObject>(string.Format("{0}batches/{1}", Consts.apiPrefix, id), batch);
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "batches/{id}")]
        [SwaggerOperation("UpdateBatch")]
        [SwaggerResponse(200, type: typeof(string))]
        public async Task<IHttpActionResult> UpdateBatch(int id)
        {
            CheckAuthentication();
            return Ok(string.Format("The batch #{0} was updated successfully!", id));
        }

        [HttpDelete]
        [Route(Consts.apiPrefix + "batches/{id}")]
        [SwaggerOperation("DeleteBatch")]
        [SwaggerResponse(200, type: typeof(string))]
        public async Task<IHttpActionResult> DeleteBatch(int id)
        {
            CheckAuthentication();
            return Ok(string.Format("The batch #{0} was deleted successfully!", id));
        }
    }
}
