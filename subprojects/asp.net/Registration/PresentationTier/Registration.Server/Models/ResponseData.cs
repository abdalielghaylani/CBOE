using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using Csla;
using Newtonsoft.Json.Linq;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the response data object
    /// </summary>
    [DataContract()]
    public partial class ResponseData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseData" /> class.
        /// </summary>
        /// <param name="id">ID of the processed record</param>
        /// <param name="regNumber">Registry number associated with the processed record</param>
        /// <param name="message">Message to return</param>
        /// <param name="data">Additional data to return</param>
        public ResponseData(int? id = null, string regNumber = null, string message = null, JObject data = null)
        {
            Id = id;
            RegNumber = regNumber;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// Gets or sets ID
        /// </summary>
        [DataMember(Name = "id")]
        public int? Id { get; set; }

        /// <summary>
        /// Gets or sets RegNumber
        /// </summary>
        [DataMember(Name = "regNumber")]
        public string RegNumber { get; set; }

        /// <summary>
        /// Gets or sets Message
        /// </summary>
        [DataMember(Name = "message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets Data
        /// </summary>
        [DataMember(Name = "data")]
        public JObject Data { get; set; }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }

    }
}
