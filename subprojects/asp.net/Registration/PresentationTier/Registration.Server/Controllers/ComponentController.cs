using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using CambridgeSoft.COE.Registration.Services;
using Swashbuckle.Swagger.Annotations;
using PerkinElmer.COE.Registration.Server.Code;
using Microsoft.Web.Http;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class ComponentController : RegControllerBase
    {
        /// <summary>
        /// Returns the list of all components
        /// </summary>
        /// <remarks>Returns the list of all components corresponding to the specified parameters
        /// </remarks>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid request</response>
        /// <response code="0">Unexpected error</response>
        [HttpGet]
        [Route(Consts.apiPrefix + "components")]
        [SwaggerOperation("GetComponents")]
        [SwaggerResponse(200, type: typeof(List<JObject>))]
        public async Task<IHttpActionResult> GetComponents(int? recordId = null, int? skip = null, int? count = null, string sort = null)
        {
            CheckAuthentication();
            // Get the list of components asynchronously
            // And return the list
            List<JObject> data = null;
            return Ok(data);
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "components/{id}")]
        [SwaggerOperation("GetComponent")]
        [SwaggerResponse(200, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetComponent(int id)
        {
            CheckAuthentication();
            if (id < 0) return NotFound();
            return Ok(new JObject());
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "components")]
        [SwaggerOperation("CreateComponent")]
        [SwaggerResponse(201, type: typeof(JObject))]
        public async Task<IHttpActionResult> CreateComponent(JObject component)
        {
            CheckAuthentication();
            var id = -1;
            return Created<JObject>(string.Format("{0}components/{1}", Consts.apiPrefix, id), component);
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "components/{id}")]
        [SwaggerOperation("UpdateComponent")]
        [SwaggerResponse(200, type: typeof(string))]
        public async Task<IHttpActionResult> UpdateComponent(int id)
        {
            CheckAuthentication();
            return Ok(string.Format("The component #{0} was updated successfully!", id));
        }

        [HttpDelete]
        [Route(Consts.apiPrefix + "components/{id}")]
        [SwaggerOperation("DeleteComponent")]
        [SwaggerResponse(200, type: typeof(string))]
        public async Task<IHttpActionResult> DeleteComponent(int id)
        {
            CheckAuthentication();
            return Ok(string.Format("The component #{0} was deleted successfully!", id));
        }
    }
}
