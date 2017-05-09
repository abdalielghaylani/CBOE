/* 
 * Registration 17 API
 */

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

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// Component
    /// </summary>
    [DataContract]
    public partial class Component : IEquatable<Component>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Component" /> class.
        /// </summary>
        /// <param name="id">ID.</param>
        /// <param name="compound">Compound.</param>
        /// <param name="componentIndex">ComponentIndex.</param>
        /// <param name="percentage">Percentage.</param>
        public Component(int? id = default(int?), CambridgeSoft.COE.Registration.Services.Types.Compound compound = default(CambridgeSoft.COE.Registration.Services.Types.Compound), int? componentIndex = default(int?), double? percentage = default(double?))
        {
            this.ID = id;
            this.Compound = compound;
            this.ComponentIndex = componentIndex;
            this.Percentage = percentage;
        }

        /// <summary>
        /// Gets or Sets ID
        /// </summary>
        [DataMember(Name = "ID", EmitDefaultValue = false)]
        public int? ID { get; set; }

        /// <summary>
        /// Gets or Sets Compound
        /// </summary>
        [DataMember(Name = "Compound", EmitDefaultValue = false)]
        public CambridgeSoft.COE.Registration.Services.Types.Compound Compound { get; set; }

        /// <summary>
        /// Gets or Sets ComponentIndex
        /// </summary>
        [DataMember(Name = "ComponentIndex", EmitDefaultValue = false)]
        public int? ComponentIndex { get; set; }

        /// <summary>
        /// Gets or Sets Percentage
        /// </summary>
        [DataMember(Name = "Percentage", EmitDefaultValue = false)]
        public double? Percentage { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Component {\n");
            sb.Append("  ID: ").Append(ID).Append("\n");
            sb.Append("  Compound: ").Append(Compound).Append("\n");
            sb.Append("  ComponentIndex: ").Append(ComponentIndex).Append("\n");
            sb.Append("  Percentage: ").Append(Percentage).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            return this.Equals(obj as Component);
        }

        /// <summary>
        /// Returns true if Component instances are equal
        /// </summary>
        /// <param name="other">Instance of Component to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Component other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return
                (
                    this.ID == other.ID || this.ID != null && this.ID.Equals(other.ID)
                ) &&
                (
                    this.Compound == other.Compound || this.Compound != null && this.Compound.Equals(other.Compound)
                ) &&
                (
                    this.ComponentIndex == other.ComponentIndex || this.ComponentIndex != null && this.ComponentIndex.Equals(other.ComponentIndex)
                ) &&
                (
                    this.Percentage == other.Percentage || this.Percentage != null && this.Percentage.Equals(other.Percentage)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = 41;
                // Suitable nullity checks etc, of course :)
                if (this.ID != null)
                    hash = hash * 59 + this.ID.GetHashCode();
                if (this.Compound != null)
                    hash = hash * 59 + this.Compound.GetHashCode();
                if (this.ComponentIndex != null)
                    hash = hash * 59 + this.ComponentIndex.GetHashCode();
                if (this.Percentage != null)
                    hash = hash * 59 + this.Percentage.GetHashCode();
                return hash;
            }
        }

    }

}
