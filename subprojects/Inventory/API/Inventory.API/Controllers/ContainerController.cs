using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Web.Http;
using PerkinElmer.COE.Inventory.API.Code;
using PerkinElmer.COE.Inventory.Model;
using PerkinElmer.COE.Inventory.DAL;
using Swashbuckle.Swagger.Annotations;
using PerkinElmer.COE.Inventory.API.Filters;

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
        [ApiKeyAuthenticationFilter]
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
        [ApiKeyAuthenticationFilter]
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

        /// <summary>
        /// Create a container
        /// </summary>
        /// <param name="container">The container data</param>
        /// <returns>Container</returns>
        [HttpPost]
        [ApiKeyAuthenticationFilter]
        [Route(Consts.apiPrefix + "containers")]
        [SwaggerOperation("Containers")]
        [SwaggerResponse(200, type: typeof(ContainerData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> CreateContainer(ContainerData container)
        {
            HttpResponseMessage responseMessage;
            try
            {
                var statusCode = HttpStatusCode.OK;

                var newContainerId = containerDAL.CreateContainer(container);
                var newContainer = containerDAL.GetContainerById(newContainerId);

                responseMessage = Request.CreateResponse(statusCode, newContainer);
            }
            catch (Exception ex)
            {
                responseMessage = CreateErrorResponse(ex);
            }

            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }

        /// <summary>
        /// Update a container
        /// </summary>
        /// <param name="containerId"></param>
        /// <param name="containerUpdatedData"></param>
        /// <returns>Container</returns>
        [HttpPut]
        [ApiKeyAuthenticationFilter]
        [Route(Consts.apiPrefix + "containers")]
        [SwaggerOperation("Containers")]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> UpdateContainer(int containerId, List<ContainerUpdatedData> containerUpdatedData)
        {
            HttpResponseMessage responseMessage;
            try
            {
                var statusCode = HttpStatusCode.OK;

                containerDAL.UpdateContainer(containerId, containerUpdatedData);

                responseMessage = Request.CreateResponse(statusCode);
            }
            catch (Exception ex)
            {
                responseMessage = CreateErrorResponse(ex);
            }

            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }

        /// <summary>
        /// Update container remaining quantity
        /// </summary>
        /// <param name="containerId">The container id to be updated</param>
        /// <param name="remainingQuantity">The remaining quantity to update</param>
        /// <returns>Container</returns>
        [HttpPut]
        [ApiKeyAuthenticationFilter]
        [Route(Consts.apiPrefix + "containers/remainingQuantity")]
        [SwaggerOperation("UpdateContainerRemainingQuantity")]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> UpdateContainerRemainingQuantity(int containerId, decimal remainingQuantity)
        {
            HttpResponseMessage responseMessage;
            try
            {
                var statusCode = HttpStatusCode.OK;

                containerDAL.UpdateContainerRemainingQuantity(containerId, remainingQuantity);

                responseMessage = Request.CreateResponse(statusCode);
            }
            catch (Exception ex)
            {
                responseMessage = CreateErrorResponse(ex);
            }

            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }

        /// <summary>
        /// Update container status
        /// </summary>
        /// <param name="containerId">The container id to be updated</param>
        /// <param name="containerStatusId">The container status id to update</param>
        /// <returns>Container</returns>
        [HttpPut]
        [ApiKeyAuthenticationFilter]
        [Route(Consts.apiPrefix + "containers/status")]
        [SwaggerOperation("UpdateContainerStatus")]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> UpdateContainerStatus(int containerId, int containerStatusId)
        {
            HttpResponseMessage responseMessage;
            try
            {
                var statusCode = HttpStatusCode.OK;

                containerDAL.UpdateContainerStatus(containerId, containerStatusId);

                responseMessage = Request.CreateResponse(statusCode);
            }
            catch (Exception ex)
            {
                responseMessage = CreateErrorResponse(ex);
            }

            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }

        /// <summary>
        /// Get container types
        /// </summary>
        /// <returns>List of container types</returns>
        [HttpGet]
        [ApiKeyAuthenticationFilter]
        [Route(Consts.apiPrefix + "containers/types")]
        [SwaggerOperation("GetContainerTypes")]
        [SwaggerResponse(200, type: typeof(List<ContainerTypeData>))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetContainerTypes()
        {
            HttpResponseMessage responseMessage;
            try
            {
                var statusCode = HttpStatusCode.OK;

                List<ContainerTypeData> containerTypes = containerDAL.GetContainerTypes();

                responseMessage = Request.CreateResponse(statusCode, containerTypes);
            }
            catch (Exception ex)
            {
                responseMessage = CreateErrorResponse(ex);
            }

            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }

        /// <summary>
        /// Get container status
        /// </summary>
        /// <returns>List of container status</returns>
        [HttpGet]
        [ApiKeyAuthenticationFilter]
        [Route(Consts.apiPrefix + "containers/status")]
        [SwaggerOperation("GetContainerStatus")]
        [SwaggerResponse(200, type: typeof(List<ContainerStatusData>))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetContainerStatus()
        {
            HttpResponseMessage responseMessage;
            try
            {
                var statusCode = HttpStatusCode.OK;

                List<ContainerStatusData> containerStatus = containerDAL.GetContainerStatus();

                responseMessage = Request.CreateResponse(statusCode, containerStatus);
            }
            catch (Exception ex)
            {
                responseMessage = CreateErrorResponse(ex);
            }

            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }

        /// <summary>
        /// Get container units
        /// </summary>
        /// <returns>List of container units</returns>
        [HttpGet]
        [ApiKeyAuthenticationFilter]
        [Route(Consts.apiPrefix + "containers/units")]
        [SwaggerOperation("GetContainerUnits")]
        [SwaggerResponse(200, type: typeof(List<UnitData>))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetContainerUnits()
        {
            HttpResponseMessage responseMessage;
            try
            {
                var statusCode = HttpStatusCode.OK;

                List<UnitData> containerUnits = containerDAL.GetContainerUnits();

                responseMessage = Request.CreateResponse(statusCode, containerUnits);
            }
            catch (Exception ex)
            {
                responseMessage = CreateErrorResponse(ex);
            }

            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }
    }
}
