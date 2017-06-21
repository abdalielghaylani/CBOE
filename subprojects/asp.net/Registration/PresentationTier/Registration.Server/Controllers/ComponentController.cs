using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using Microsoft.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using CambridgeSoft.COE.Registration.Services;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration.Access;
using PerkinElmer.COE.Registration.Server.Code;
using PerkinElmer.COE.Registration.Server.Models;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class ComponentController : RegControllerBase
    {
        protected static RecordColumn[] ComponentRecordColumns
        {
            get
            {
                return new RecordColumn[]
                {
                    new RecordColumn { Definitions = "STRUCTUREID", Sortable = true }, 
                    new RecordColumn { Definitions = "COMPONENTID", Sortable = true }, 
                    new RecordColumn { Definitions = "STRUCT_NAME", Sortable = false }, 
                    new RecordColumn { Definitions = "STRUCT_COMMENTS", Sortable = false }, 
                    new RecordColumn { Definitions = "CMP_COMMENTS", Sortable = false }, 
                    new RecordColumn { Definitions = "MOLECULARFORMULA", Sortable = false }, 
                    new RecordColumn { Definitions = "FORMULAWEIGHT", Sortable = false }, 
                    new RecordColumn { Definitions = "NORMALIZEDSTRUCTURE", Sortable = false }
                };
            }
        }

        /// <summary>
        /// Returns the list of all components
        /// </summary>
        /// <remarks>Returns the list of all components corresponding to the specified parameters
        /// </remarks>
        /// <response code="200">Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        /// <param name="recordId">The record ID</param>
        /// <param name="skip">The number of components to skip</param>
        /// <param name="count">The maximum count of components to return</param>
        /// <param name="sort">The sort information</param>
        /// <returns>The list of components</returns>
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
        [Route(Consts.apiPrefix + "reg-components/{regNum}")]
        [SwaggerOperation("GetComponentsByRegNum")]
        [SwaggerResponse(200, type: typeof(JArray))]
        [SwaggerResponse(404, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetComponentsByRegNum(string regNum)
        {
            return await CallMethod(() =>
            {
                var args = new Dictionary<string, object>();
                args.Add(":regNum", regNum);
                var data = ExtractData("SELECT STRUCTUREID, COMPONENTID, STRUCT_NAME, STRUCT_COMMENTS, CMP_COMMENTS, MOLECULARFORMULA, FORMULAWEIGHT, NORMALIZEDSTRUCTURE FROM REGDB.VW_MIXTURE_STRUCTURE WHERE REGNUMBER=:regNum", args);
                if (data.Count() == 0)
                    throw new IndexOutOfRangeException(string.Format("Cannot find the registration number, {0}", regNum));
                return data;
            });
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "components/{id}")]
        [SwaggerOperation("GetComponent")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(404, type: typeof(JObject))]
        public async Task<IHttpActionResult> GetComponent(string id)
        {
            return await CallMethod(() =>
            {
                var args = new Dictionary<string, object>();
                args.Add(":id", id);
                var data = ExtractData("SELECT STRUCTUREID, COMPONENTID, STRUCT_NAME, STRUCT_COMMENTS, CMP_COMMENTS, MOLECULARFORMULA, FORMULAWEIGHT, NORMALIZEDSTRUCTURE FROM REGDB.VW_MIXTURE_STRUCTURE WHERE COMPONENTID=:id");
                if (data.Count() == 0)
                    throw new IndexOutOfRangeException(string.Format("Cannot find the component ID, {0}", id));
                return data[0];
            });
        }

        [HttpPost]
        [Route(Consts.apiPrefix + "components")]
        [SwaggerOperation("CreateComponent")]
        [SwaggerResponse(201, type: typeof(JObject))]
        public async Task<IHttpActionResult> CreateComponent(JObject component)
        {
            return await CallMethod(() =>
            {
                var id = -1;
                return Created<JObject>(string.Format("{0}components/{1}", Consts.apiPrefix, id), component);
            });
        }

        [HttpPut]
        [Route(Consts.apiPrefix + "components/{id}")]
        [SwaggerOperation("UpdateComponent")]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        public async Task<IHttpActionResult> UpdateComponent(JObject componentData)
        {
            return await CallMethod(() =>
            {
                XmlDocument datatoXml = Newtonsoft.Json.JsonConvert.DeserializeXmlNode(componentData.ToString(Newtonsoft.Json.Formatting.None));
                string message;
                RegDal.UpdateRegistryRecord(datatoXml.InnerXml, CambridgeSoft.COE.Registration.DuplicateCheck.CompoundCheck, out message);
                return new ResponseData(null, null, message, null);
            });
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
