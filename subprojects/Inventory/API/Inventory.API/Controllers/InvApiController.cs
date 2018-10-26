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
