﻿using CambridgeSoft.COE.Framework.COETableEditorService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using PerkinElmer.COE.Registration.Server.Code;
using Newtonsoft.Json.Linq;

namespace PerkinElmer.COE.Registration.Server.Controllers
{
    public class CustomTablesController : RegControllerBase
    {
        [Route("api/CustomTables")]
        public JArray Get()
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
        }

        [Route("api/CustomTables/{tableName}")]
        public IEnumerable<JToken> Get(string tableName)
        {
            var options = Request.GetQueryNameValuePairs()
                .ToDictionary(x => x.Key, x => JsonConvert.DeserializeObject(x.Value));
            var projects = new JArray();
            COETableEditorBOList.NewList().TableName = tableName;
            var dt = COETableEditorBOList.getTableEditorDataTable(tableName);
            foreach (DataRow dr in dt.Rows)
            {
                var p = new JObject();
                foreach (DataColumn dc in dt.Columns)
                    p.Add(new JProperty(dc.ColumnName, dr[dc]));
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

        [HttpPost]
        [Route("api/CustomTables/{tableName}")]
        public int Post(dynamic data)
        {
            return 0;
        }

        [HttpPut]
        [Route("api/CustomTables/{tableName}/{id}")]
        public int Put(dynamic data)
        {
            return 0;
        }

        [HttpDelete]
        [Route("api/CustomTables/{tableName}/{id}")]
        public int Delete(int id)
        {
            return 0;
        }
    }
}
