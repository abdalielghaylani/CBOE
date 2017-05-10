using System.Web.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Reflection;
using PerkinElmer.COE.Registration.Server.Code;
using Microsoft.Web.Http;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class DataConversionController : RegControllerBase
    {
        [HttpPost]
        [Route(Consts.apiPrefix + "DataConversion/ToCdxml")]
        public JObject ToCdxml()
        {
            var data = Request.Content.ReadAsAsync<JObject>().Result["data"].ToString();
            return new JObject(new JProperty("data", ChemistryHelper.ConvertToCdxml(data)));
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "DataConversion/FromCdxml")]
        public JObject FromCdxml()
        {
            var data = Request.Content.ReadAsAsync<JObject>().Result["data"].ToString();
            return new JObject(new JProperty("data", ChemistryHelper.ConvertToCdx(data)));
        }
    }
}
