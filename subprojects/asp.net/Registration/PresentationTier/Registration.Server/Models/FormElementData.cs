using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using CambridgeSoft.COE.Framework.Common.Messaging;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// The class for the FormElement data object
    /// </summary>
    public partial class FormElementData
    {
        [JsonConstructor]
        public FormElementData(string formGroupName, string name, string controlType, string label, string cssClass, bool visible)
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
        /// Gets or Sets Name
        /// </summary>
        [JsonProperty(PropertyName = "FormGroupName")]
        public string FormGroupName { get; set; }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets ControlType
        /// </summary>
        [JsonProperty(PropertyName = "ControlType")]
        public string ControlType { get; set; }

        /// <summary>
        /// Gets or Sets Label
        /// </summary>
        [JsonProperty(PropertyName = "Label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or Sets CssClass
        /// </summary>
        [JsonProperty(PropertyName = "CssClass")]
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or Sets Visible
        /// </summary>
        [JsonProperty(PropertyName = "Visible")]
        public bool Visible { get; set; }
    }
}
