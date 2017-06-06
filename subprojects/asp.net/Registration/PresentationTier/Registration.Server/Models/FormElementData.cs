using System.Collections.Generic;
using Newtonsoft.Json;
using CambridgeSoft.COE.Framework.Common.Messaging;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the FormElement data object
    /// </summary>
    public partial class FormElementData
    {
        [JsonConstructor]
        public FormElementData(string group, string name, string type, string controlType, bool controlEnabled, List<KeyValuePair<string, string>> controlTypeOptions, string label, string cssClass, bool? visible)
        {
            Group = group;
            Name = name;
            Type = type;
            Label = label;
            ControlType = controlType;
            ControlEnabled = controlEnabled;
            ControlTypeOptions = controlTypeOptions;
            CssClass = cssClass;
            Visible = visible;
        }

        public FormElementData(string group, FormGroup.FormElement formElement)
        {
            Group = group;
            Name = formElement.Id.Replace("Property", string.Empty);
            ControlType = formElement.DisplayInfo.Type;
            Label = formElement.Label;
            CssClass = formElement.DisplayInfo.CSSClass == null ? string.Empty : formElement.DisplayInfo.CSSClass;
            Visible = formElement.DisplayInfo.Visible;
        }

        /// <summary>
        /// Gets or sets the group name
        /// </summary>
        [JsonProperty(PropertyName = "group")]
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the label
        /// </summary>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the Type
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the control type
        /// </summary>
        [JsonProperty(PropertyName = "controlType")]
        public string ControlType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ControlEnabled is enabled
        /// </summary>
        [JsonProperty(PropertyName = "controlEnabled")]
        public bool? ControlEnabled { get; set; }

        /// <summary>
        /// Gets or sets the list of ControlTypeOptions 
        /// </summary>
        [JsonProperty(PropertyName = "controlTypeOptions", NullValueHandling = NullValueHandling.Ignore)]
        public List<KeyValuePair<string, string>> ControlTypeOptions { get; set; }

        /// <summary>
        /// Gets or sets the CSS class name
        /// </summary>
        [JsonProperty(PropertyName = "cssClass")]
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item is visible
        /// </summary>
        [JsonProperty(PropertyName = "visible")]
        public bool? Visible { get; set; }
    }
}
