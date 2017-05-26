using Newtonsoft.Json;
using System.Collections.Generic;

namespace PerkinElmer.COE.Registration.Server.Models
{
    public class AddinData
    {
        public AddinData()
        {
        }

        [JsonConstructor]
        public AddinData(string name, string addIn, string assembly, string className, string classNameSpace, bool enable, bool required, string configuration, List<EventData> events)
        {
            Name = name;
            AddIn = addIn;
            Assembly = assembly;
            ClassName = className;
            ClassNamespace = classNameSpace;
            Configuration = configuration;
            Enable = enable;
            Required = required;
            Configuration = configuration;
            Events = events;
        }

        /// <summary>
        /// Gets or sets the friendly name of addin
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the addin class name
        /// addinName = Assembly = addin.IsNew ? addin.ClassNameSpace + "." + addin.ClassName : addin.ClassName;
        /// </summary>
        [JsonProperty(PropertyName = "addinName")]
        public string AddIn { get; set; }

        /// <summary>
        /// Gets or sets the Assembly name
        /// </summary>
        [JsonProperty(PropertyName = "assembly")]
        public string Assembly { get; set; }

        /// <summary>
        /// Gets or sets the class name
        /// </summary>
        [JsonProperty(PropertyName = "className")]
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets the ClassNamespace 
        /// </summary>
        [JsonProperty(PropertyName = "classNamespace")]
        public string ClassNamespace { get; set; }

        /// <summary>
        /// Gets or sets the Enable property
        /// </summary>
        [JsonProperty(PropertyName = "enable")]
        public bool Enable { get; set; }

        /// <summary>
        /// Gets or sets the Required property
        /// </summary>
        [JsonProperty(PropertyName = "required")]
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the configuration 
        /// </summary>
        [JsonProperty(PropertyName = "Configuration")]
        public string Configuration { get; set; }

        /// <summary>
        /// Gets or sets the Events 
        /// </summary>
        [JsonProperty(PropertyName = "events")]
        public List<EventData> Events { get; set; }
    }

    public class EventData
    {
        public EventData()
        {
        }

        [JsonConstructor]
        public EventData(string eventName, string eventHandler)
        {
            EventName = eventName;
            EventHandler = eventHandler;
        }

        /// <summary>
        /// Gets or sets the EventName
        /// </summary>
        [JsonProperty(PropertyName = "eventName")]
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets the EventHandler
        /// </summary>
        [JsonProperty(PropertyName = "eventHandler")]
        public string EventHandler { get; set; }
    }
}