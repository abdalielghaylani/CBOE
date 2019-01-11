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
    /// The inventory container controller
    /// </summary>
    [ApiVersion(Consts.apiVersion)]
    public class ContainerController : InvApiController
    {
        /// <summary>
        /// 
        /// </summary>
        protected ContainerDAL internalContainerDAL;

        /// <summary>
        /// DAL for containers
        /// </summary>
        protected ContainerDAL containerDAL
        {
            get
            {
                if (internalContainerDAL == null)
                {
                    internalContainerDAL = new ContainerDAL();
                }
                return internalContainerDAL;
            }
        }

        /// <summary>
        /// Constructor for ContainerController
        /// </summary>
        public ContainerController()
        {
        }

        /// <summary>
        /// Constructor for ContainerController
        /// </summary>
        public ContainerController(IInventoryDBContext context)
        {
            internalContainerDAL = new ContainerDAL(context);
        }

        /// <summary>
        /// Get container by internal id
        /// </summary>
        /// <param name="id">Internal id</param>
        /// <returns>Container</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "containers/{id:int}")]
        [SwaggerOperation("Containers")]
        [SwaggerResponse(200, type: typeof(ContainerData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetContainerById(int id)
        {
            HttpResponseMessage responseMessage;
            try
            {
                // TODO: Should check authentication and authorization
                var statusCode = HttpStatusCode.OK;

                ContainerData container = containerDAL.GetContainerById(id);
                if (container == null)
                    throw new IndexOutOfRangeException(string.Format("Cannot find the container, {0}", id));
                responseMessage = Request.CreateResponse(statusCode, container);
            }
            catch (Exception ex)
            {
                responseMessage = CreateErrorResponse(ex);
            }

            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }

        /// <summary>
        /// Get container by barcode
        /// </summary>
        /// <param name="barcode">Barcode</param>
        /// <returns>Container</returns>
        [HttpGet]
        [Route(Consts.apiPrefix + "containers/barcode/{barcode}")]
        [SwaggerOperation("Containers")]
        [SwaggerResponse(200, type: typeof(ContainerData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetContainerByBarcode(string barcode)
        {
            HttpResponseMessage responseMessage;
            try
            {
                // TODO: Should check authentication and authorization
                var statusCode = HttpStatusCode.OK;

                ContainerData container = containerDAL.GetContainerByBarcode(barcode);
                if (container == null)
                    throw new IndexOutOfRangeException(string.Format("Cannot find the container, {0}", barcode));
                responseMessage = Request.CreateResponse(statusCode, container);
            }
            catch (Exception ex)
            {
                responseMessage = CreateErrorResponse(ex);
            }

            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }
    }
}
