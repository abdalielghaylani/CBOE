using Newtonsoft.Json;

namespace PerkinElmer.COE.Registration.Server.Models
{
    public class ControlState
    {
        public ControlState()
        {
        }

        [JsonConstructor]
        public ControlState(string name, bool? enable, bool? visible, string tooltip)
        {           
            Name = name;
            Enable = enable;
            Visible = visible;
            Tooltip = tooltip;
        }        

        /// <summary>
        /// Gets or sets the name of the control
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the control is enabled or not
        /// </summary>
        [JsonProperty(PropertyName = "enable")]
        public bool? Enable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the control is visible or not
        /// </summary>
        [JsonProperty(PropertyName = "visible")]
        public bool? Visible { get; set; }

        /// <summary>
        /// Gets or sets the tooltip displayed on the control
        /// </summary>
        [JsonProperty(PropertyName = "tooltip")]
        public string Tooltip { get; set; }
    }
}