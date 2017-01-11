using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    public class AuthController : ApiController
    {
        [HttpPost]
        [Route("api/auth/login")]
        public JObject Login()
        {
            var userData = Request.Content.ReadAsAsync<JObject>();
            return new JObject(
                new JProperty("data", new JObject(new JProperty("msg", "LOGIN SUCCESSFUL"))),
                new JProperty("meta",
                    new JObject(
                        new JProperty("id", 1),
                        new JProperty("token", "abcd1234"),
                        new JProperty("expires", "2020-01-01"),
                        new JProperty("profile", new JObject(new JProperty("fullName", "Admin User")))
                    )
                )
            );
        }
    }
}
