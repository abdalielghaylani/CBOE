using System.Net.Http;
using System.Web.Http;
using Microsoft.Web.Http;
using Newtonsoft.Json.Linq;
using PerkinElmer.COE.Registration.Server.Code;

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
