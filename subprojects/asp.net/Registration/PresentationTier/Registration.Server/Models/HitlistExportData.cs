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
        public HitlistExportData(List<ResultsCriteriaTableData> resultsCriteriaTableData, List<string> records)
        {
            ResultsCriteriaTables = resultsCriteriaTableData;
            Records = records;
        }        
    
        [JsonProperty(PropertyName = "resultsCriteriaTables")]
        public List<ResultsCriteriaTableData> ResultsCriteriaTables { get; set; }
    
        [JsonProperty(PropertyName = "records")]
        public List<string> Records { get; set; }
    }
}