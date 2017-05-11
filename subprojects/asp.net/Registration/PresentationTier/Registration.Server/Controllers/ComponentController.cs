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
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration.Access;
using System.Net;
using System.Xml;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class ComponentController : RegControllerBase
    {
        private RegistrationOracleDAL regDal = null;

        private RegistrationOracleDAL RegDal
        {
            get
            {
                if (regDal == null)
                    DalUtils.GetRegistrationDAL(ref regDal, CambridgeSoft.COE.Registration.Constants.SERVICENAME);
                return regDal;
            }
        }
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
        public async Task<IHttpActionResult> GetComponents(string recordId, int? skip = null, int? count = null, string sort = null)
        {
            CheckAuthentication();
            var tableName = "REGDB.VW_MIXTURE_STRUCTURE";
            var whereClause = " WHERE REGNUMBER=:regNumber";
            var query = GetQuery(tableName + whereClause, ComponentRecordColumns, sort, "COMPONENTID", "STRUCTUREID");
            var args = new Dictionary<string, object>();
            args.Add(":regNumber", recordId);
            var responseMessage = Request.CreateResponse(HttpStatusCode.OK, new JObject(
                new JProperty("VW_MIXTURE_STRUCTURE", false),
                new JProperty("regNumber", recordId),
                new JProperty("totalCount", Convert.ToInt32(ExtractValue("SELECT cast(count(1) as int) c FROM " + tableName + whereClause, args))),
                new JProperty("startIndex", skip == null ? 0 : Math.Max(skip.Value, 0)),
                new JProperty("rows", ExtractData(query, args, skip, count))
            ));
            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "components/{id}")]
        [SwaggerOperation("GetComponent")]
        [SwaggerResponse(200, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetComponent(string id, string regnumber)
        {            
            CheckAuthentication();
            JArray data = new JArray();           
            data = ExtractData(" SELECT STRUCTUREID, COMPONENTID, STRUCT_NAME, STRUCT_COMMENTS, CMP_COMMENTS, MOLECULARFORMULA, FORMULAWEIGHT, NORMALIZEDSTRUCTURE FROM REGDB.VW_MIXTURE_STRUCTURE WHERE REGNUMBER = '" + regnumber + "' AND COMPONENTID = '" + id + "'");
            var responseMessage = Request.CreateResponse(HttpStatusCode.OK, data);
            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
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
        public async Task<IHttpActionResult> UpdateComponent(JObject data)
        {
            string _dalResponseXml = "";
            CheckAuthentication();
            var jData = Request.Content.ReadAsAsync<JObject>().Result;
            RegistrationOracleDAL registrationoracleDAL = new RegistrationOracleDAL();
            XmlDocument datatoXml = Newtonsoft.Json.JsonConvert.DeserializeXmlNode(data.ToString());
            RegDal.UpdateRegistryRecord(datatoXml.InnerXml, CambridgeSoft.COE.Registration.DuplicateCheck.CompoundCheck, out _dalResponseXml);
            return Ok(string.Format("The component #{0} was updated successfully!", _dalResponseXml));
        }

        [HttpDelete]
        [Route(Consts.apiPrefix + "components/{id}")]
        [SwaggerOperation("DeleteComponent")]
        [SwaggerResponse(200, type: typeof(string))]
        public async Task<IHttpActionResult> DeleteComponent(int id)
        {            
            CheckAuthentication();
            RegistryRecord registryRecord = null;
            registryRecord.DeleteComponent(id);
            var responseMessage = Request.CreateResponse(HttpStatusCode.OK, string.Format("The component #{0} was deleted successfully!", id));
            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));           
        }
        
    }
}
