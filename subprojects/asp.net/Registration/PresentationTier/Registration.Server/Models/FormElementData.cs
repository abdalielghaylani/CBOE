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
        public FormElementData(string formGroupName, string name, string controlType, string label, string cssClass, bool? visible)
        {
            FormGroupName = formGroupName;
            Name = name;
            ControlType = controlType;
            Label = label;
            CssClass = cssClass;
            Visible = visible;
        }

        public FormElementData(FormGroup.FormElement formElement)
        {
            Name = formElement.Name;
            ControlType = formElement.DisplayInfo.Type;
            Label = formElement.Label;
            CssClass = formElement.DisplayInfo.CSSClass;
            Visible = formElement.DisplayInfo.Visible;
        }

        /// <summary>
        /// Gets or sets the form-group name
        /// </summary>
        [JsonProperty(PropertyName = "formGroupName")]
        public string FormGroupName { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the control type
        /// </summary>
        [JsonProperty(PropertyName = "controlType")]
        public string ControlType { get; set; }

        /// <summary>
        /// Gets or sets the label
        /// </summary>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the CSS class name
        /// </summary>
        [JsonProperty(PropertyName = "cssClass")]
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets the visible flag
        /// </summary>
        [JsonProperty(PropertyName = "visible")]
        public bool? Visible { get; set; }
    }
}
