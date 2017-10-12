using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    public class ResultsCriteriaData
    {
        public ResultsCriteriaData()
        {
        }

        [JsonConstructor]
        public ResultsCriteriaData(List<CriteriaTableData> criterias)
        {
            Criterias = criterias;
        }

        /// <summary>
        /// Gets or sets the criterias
        /// </summary>
        [JsonProperty(PropertyName = "criterias", NullValueHandling = NullValueHandling.Ignore)]
        public List<CriteriaTableData> Criterias { get; set; }
       
    }
}