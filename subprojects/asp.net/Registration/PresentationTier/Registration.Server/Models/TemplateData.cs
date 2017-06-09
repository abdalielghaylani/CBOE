using System;
using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    public class TemplateData
    {
        public TemplateData()
        {
        }

        [JsonConstructor]
        public TemplateData(int id, string name, DateTime dateCreated, string description, bool isPublic, string username, string data)
        {
            Id = id;
            Name = name;
            DateCreated = dateCreated;
            Description = description;
            IsPublic = isPublic;
            Username = username;
            Data = data;
        }

        /// <summary>
        /// Gets or sets the id of Template
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of Template
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the DateCreated
        /// </summary>
        [JsonProperty(PropertyName = "dateCreated")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the Description
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the template is public or not
        /// </summary>
        [JsonProperty(PropertyName = "isPublic")]
        public bool IsPublic { get; set; }

        /// <summary>
        /// Gets or sets the Username
        /// </summary>
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the Data
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }
    }
}