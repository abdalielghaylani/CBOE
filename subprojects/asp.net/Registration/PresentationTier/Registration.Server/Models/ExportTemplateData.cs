using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    public class ExportTemplateData
    {
        [JsonConstructor]
        public ExportTemplateData(string templateName, string templateDescription, bool isPublic, List<ResultsCriteriaTableData> resultsCriteriaTableData)
        {
            TemplateName = templateName;
            TemplateDescription = templateDescription;
            IsPublic = isPublic;
            ResultsCriteriaTableData = resultsCriteriaTableData;
        }

        /// <summary>
        /// Gets or sets TemplateName
        /// </summary>
        [JsonProperty(PropertyName = "templateName")]
        public string TemplateName { get; set; }

        /// <summary>
        /// Gets or sets TemplateDescription
        /// </summary>
        [JsonProperty(PropertyName = "templateDescription")]
        public string TemplateDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it is Public
        /// </summary>
        [JsonProperty(PropertyName = "isPublic")]
        public bool IsPublic { get; set; }

        /// <summary>
        /// Gets or sets ResultsCriteriaTableData
        /// </summary>
        [JsonProperty(PropertyName = "resultsCriteriaTableData")]
        public List<ResultsCriteriaTableData> ResultsCriteriaTableData { get; set; }
    }
}