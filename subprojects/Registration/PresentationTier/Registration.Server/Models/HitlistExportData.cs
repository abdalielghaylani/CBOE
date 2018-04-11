using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    public class HitlistExportData
    {
        [JsonConstructor]
        public HitlistExportData(List<ResultsCriteriaTableData> resultsCriteriaTableData)
        {
            ResultsCriteriaTables = resultsCriteriaTableData;
        }        
    
        [JsonProperty(PropertyName = "resultsCriteriaTables")]
        public List<ResultsCriteriaTableData> ResultsCriteriaTables { get; set; }
    }
}