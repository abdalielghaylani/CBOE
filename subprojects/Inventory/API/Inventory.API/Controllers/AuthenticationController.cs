using Microsoft.Web.Http;
using PerkinElmer.COE.Inventory.API.Code;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace PerkinElmer.COE.Inventory.API.Controllers
{
    /// <summary>
    /// The authentication controller
    /// </summary>
    [ApiVersion(Consts.apiVersion)]
    public class AuthenticationController : InvApiController
    {
        /// <summary>
        /// Get an API Key based on the secret prhase
        /// </summary>
        /// <param name="secret">Known secret phrase</param>
        /// <returns>Encrypted token</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "authentication/apikey")]
        [SwaggerOperation("Authentication")]
        [SwaggerResponse(200, type: typeof(string))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GenerateApiKey(string secret)
        {
            HttpResponseMessage responseMessage;
            try
            {
                // TODO: Should check authentication and authorization
                var statusCode = HttpStatusCode.OK;

                var apikey = SecurityHelper.GenerateKey(secret);

                responseMessage = Request.CreateResponse(statusCode, apikey);
            }
            catch (Exception ex)
            {
                responseMessage = CreateErrorResponse(ex);
            }

            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }
    }
}
