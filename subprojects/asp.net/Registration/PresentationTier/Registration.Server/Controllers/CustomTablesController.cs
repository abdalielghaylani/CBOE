using CambridgeSoft.COE.Framework.COETableEditorService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using PerkinElmer.COE.Registration.Server.Code;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    public class CustomTablesController : ApiController
    {
        public IEnumerable<IDictionary<string, string>> Get()
        {
            var options = Request.GetQueryNameValuePairs()
                .ToDictionary(x => x.Key, x => JsonConvert.DeserializeObject(x.Value));
            var tableList = new List<IDictionary<string, string>>();
            var tables = COETableEditorUtilities.getTables();
            foreach (var key in tables.Keys)
            {
                var table = new Dictionary<string, string>();
                table[key] = tables[key];
                tableList.Add(table);
            }
            return tableList.AsEnumerable().FilterByOptions(options).SortByOptions(options).PageByOptions(options);
        }

        [Route("api/CustomTables/{tableName}")]
        public IEnumerable<dynamic> Get(string tableName)
        {
            var options = Request.GetQueryNameValuePairs()
                .ToDictionary(x => x.Key, x => JsonConvert.DeserializeObject(x.Value));
            var projects = new List<dynamic>();
            COETableEditorBOList.NewList().TableName = tableName;
            var dt = COETableEditorBOList.getTableEditorDataTable(tableName);
            foreach (DataRow dr in dt.Rows)
            {
                dynamic expando = new ExpandoObject();
                var p = expando as IDictionary<String, object>;
                foreach (DataColumn dc in dt.Columns)
                    p[dc.ColumnName] = dr[dc];
                projects.Add(p);
            }
            var query = projects.AsEnumerable().FilterByOptions(options).SortByOptions(options).PageByOptions(options);
            return query;
        }

        [Route("api/CustomTables/{tableName}/{id}")]
        public dynamic Get(string tableName, int id)
        {
            return null;
        }

        [Route("api/CustomTables/{tableName}")]
        public int Post(dynamic data)
        {
            return 0;
        }

        [Route("api/CustomTables/{tableName}/{id}")]
        public int Put(dynamic data)
        {
            return 0;
        }

        [Route("api/CustomTables/{tableName}/{id}")]
        public int Delete(int id)
        {
            return 0;
        }
    }
}
