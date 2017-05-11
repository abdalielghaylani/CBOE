using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Web.Http;
using Swashbuckle.Swagger.Annotations;
using CambridgeSoft.COE.Framework.COETableEditorService;
using PerkinElmer.COE.Registration.Server.Code;
using PerkinElmer.COE.Registration.Server.Models;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class CustomTablesController : RegControllerBase
    {
        [HttpGet]
        [Route(Consts.apiPrefix + "custom-tables")]
        [SwaggerOperation("GetCustomTables")]
        [SwaggerResponse(200, type: typeof(JArray))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetCustomTables()
        {
            return await CallMethod(() =>
            {
                var tableList = new JArray();
                var tables = COETableEditorUtilities.getTables();
                foreach (var key in tables.Keys)
                {
                    var table = new JObject(
                        new JProperty("tableName", key),
                        new JProperty("label", tables[key])
                    );
                    tableList.Add(table);
                }
                return tableList;
            });
        }

        private static JObject GetTableConfig(string tableName)
        {
            // Returns the field configuration
            // COETableEditorUtilities.getColumnList returns the column lists
            // COETableEditorUtilities.GetAlias returns the column aliases (labels)
            // COETableEditorUtilities.getLookupLocation returns the look-up information
            // This should also return user authorization
            // Refer to routines like COETableEditorUtilities.HasDeletePrivileges and COETableEditorUtilities.HasEditPrivileges

            return new JObject();
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "custom-tables/{tableName}")]
        [SwaggerOperation("GetCustomTableRows")]
        [SwaggerResponse(200, type: typeof(JArray))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetCustomTableRows(string tableName)
        {
            return await CallMethod(() =>
            {
                var config = GetTableConfig(tableName);
                var rows = new JArray();
                COETableEditorBOList.NewList().TableName = tableName;
                var dt = COETableEditorBOList.getTableEditorDataTable(tableName);
                foreach (DataRow dr in dt.Rows)
                {
                    var p = new JObject();
                    foreach (DataColumn dc in dt.Columns)
                        p.Add(new JProperty(dc.ColumnName, dc.ColumnName.Equals("structure", StringComparison.OrdinalIgnoreCase) ? "fragment/" + dr[0].ToString() : dr[dc]));
                    rows.Add(p);
                }
                return new JObject(
                    new JProperty("config", config),
                    new JProperty("rows", rows)
                );
            });
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "custom-tables/{tableName}/{id}")]
        [SwaggerOperation("GetCustomTableRow")]
        [SwaggerResponse(200, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetCustomTableRow(string tableName, int id)
        {
            return await CallMethod(() =>
            {
                var row = new JObject();
                return row;
            });
        }

        [HttpPost]
        [SwaggerResponse(201, type: typeof(JObject))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [Route(Consts.apiPrefix + "custom-tables/{tableName}")]
        public async Task<IHttpActionResult> Post(string tableName, JObject data)
        {
            return await CallMethod(() =>
            {
                var id = -1;
                return new ResponseData(id, null, null, null);
            });
        }

        [HttpPut]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [Route(Consts.apiPrefix + "custom-tables/{tableName}/{id}")]
        public async Task<IHttpActionResult> Put(string tablename, int id, JObject data)
        {
            return await CallMethod(() =>
            {
                return new ResponseData(id, null, null, null);
            });
        }

        [HttpDelete]
        [SwaggerResponse(200, type: typeof(ResponseData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(404, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        [Route(Consts.apiPrefix + "custom-tables/{tableName}/{id}")]
        public async Task<IHttpActionResult> Delete(string tableName, int id)
        {
            return await CallMethod(() =>
            {
                if (!COETableEditorUtilities.HasDeletePrivileges(tableName))
                    throw new UnauthorizedAccessException();
                var tableEditorListBO = COETableEditorBOList.NewList();
                tableEditorListBO.TableName = tableName;
                // TODO: Should check if id is present.
                // If not, throw error 404.
                COETableEditorBO.Delete(id);
                return new ResponseData(id, null, null, null);
            });
        }
    }
}
