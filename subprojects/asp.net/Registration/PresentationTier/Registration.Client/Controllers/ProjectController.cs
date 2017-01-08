using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;

namespace PerkinElmer.CBOE.Registration.Client.Controllers
{
    public class ProjectController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Index()
        {
            return new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                ReasonPhrase = "OK"
            };
        }
    }
}