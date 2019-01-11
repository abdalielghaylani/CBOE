using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PerkinElmer.COE.Inventory.API.Controllers
{
    /// <summary>
    /// The inventory api controller
    /// </summary>
    public class InvApiController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected HttpResponseMessage CreateErrorResponse(Exception ex)
        {
            var message = ex.Message;
            var statusCode = ex is IndexOutOfRangeException ?
                HttpStatusCode.NotFound :
                HttpStatusCode.InternalServerError;
            return string.IsNullOrEmpty(message) ? Request.CreateErrorResponse(statusCode, ex) : Request.CreateErrorResponse(statusCode, message, ex);
        }
    }
}
