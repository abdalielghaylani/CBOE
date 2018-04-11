using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    public class AddinEvent
    {
        public AddinEvent()
        {
        }

        [JsonConstructor]
        public AddinEvent(string eventName, string eventHandler)
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