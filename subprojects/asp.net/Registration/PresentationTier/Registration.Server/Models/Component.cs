/* 
 * Registration 17 API
 */

using System;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;

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
        /// <param name="structureId">StructureID</param>
        /// <param name="struct_Name">Struct_Name</param>
        /// <param name="struct_Comments">Stuct_Comments</param>
        /// <param name="cmp_Comments">Cmp_Comments</param>
        /// <param name="molecularFormula">MolecularFormula</param>
        /// <param name="formulaWeight">FormulaWeight</param>
        /// <param name="normalizedStructure">NormalizedStructure</param>
        public Component(int? id = default(int?), CambridgeSoft.COE.Registration.Services.Types.Compound compound = default(CambridgeSoft.COE.Registration.Services.Types.Compound), int? componentIndex = default(int?), double? percentage = default(double?), int? structureId = default(int?), string struct_Name = default(string), string struct_Comments = default(string), string cmp_Comments = default(string), double? molecularFormula = default(double?), double? formulaWeight = default(double?), string normalizedStructure = default(string))
        {
            this.ID = id;
            this.Compound = compound;
            this.ComponentIndex = componentIndex;
            this.Percentage = percentage;
            this.StructureId = structureId;
            this.Struct_Name = struct_Name;
            this.Stuct_Comments = struct_Comments;
            this.Cmp_Comments = cmp_Comments;
            this.MolecularFormula = molecularFormula;
            this.FormulaWeight = formulaWeight;
            this.NormalizedStructure = normalizedStructure;
        }

        /// <summary>
        /// Gets or sets ID
        /// </summary>
        [DataMember(Name = "ID", EmitDefaultValue = false)]
        public int? ID { get; set; }

        /// <summary>
        /// Gets or sets Compound
        /// </summary>
        [DataMember(Name = "Compound", EmitDefaultValue = false)]
        public CambridgeSoft.COE.Registration.Services.Types.Compound Compound { get; set; }

        /// <summary>
        /// Gets or sets ComponentIndex
        /// </summary>
        [DataMember(Name = "ComponentIndex", EmitDefaultValue = false)]
        public int? ComponentIndex { get; set; }

        /// <summary>
        /// Gets or sets Percentage
        /// </summary>
        [DataMember(Name = "Percentage", EmitDefaultValue = false)]
        public double? Percentage { get; set; }

        /// <summary>
        /// Gets or sets StructureId
        /// </summary>
        [DataMember(Name = "StructureId", EmitDefaultValue = false)]
        public int? StructureId { get; set; }

        /// <summary>
        /// Gets or sets Struct_Name
        /// </summary>
        [DataMember(Name = "Struct_Name", EmitDefaultValue = false)]
        public string Struct_Name { get; set; }

        /// <summary>
        /// Gets or sets Stuct_Comments
        /// </summary>
        [DataMember(Name = "Stuct_Comments", EmitDefaultValue = false)]
        public string Stuct_Comments { get; set; }

        /// <summary>
        /// Gets or sets Cmp_Comments
        /// </summary>
        [DataMember(Name = "Cmp_Comments", EmitDefaultValue = false)]
        public string Cmp_Comments { get; set; }

        /// <summary>
        /// Gets or sets MolecularFormula
        /// </summary>
        [DataMember(Name = "MolecularFormula", EmitDefaultValue = false)]
        public double? MolecularFormula { get; set; }

        /// <summary>
        /// Gets or sets FormulaWeight
        /// </summary>
        [DataMember(Name = "FormulaWeight", EmitDefaultValue = false)]
        public double? FormulaWeight { get; set; }

        /// <summary>
        /// Gets or sets NormalizedStructure
        /// </summary>
        [DataMember(Name = "NormalizedStructure", EmitDefaultValue = false)]
        public string NormalizedStructure { get; set; }

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
            sb.Append("  StructureId: ").Append(StructureId).Append("\n");
            sb.Append("  Struct_Name: ").Append(Struct_Name).Append("\n");
            sb.Append("  Stuct_Comments: ").Append(Stuct_Comments).Append("\n");
            sb.Append("  Cmp_Comments: ").Append(Cmp_Comments).Append("\n");
            sb.Append("  MolecularFormula: ").Append(MolecularFormula).Append("\n");
            sb.Append("  FormulaWeight: ").Append(FormulaWeight).Append("\n");
            sb.Append("  NormalizedStructure: ").Append(NormalizedStructure).Append("\n");
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
                ) &&
                (
                    this.StructureId == other.StructureId || this.StructureId != null && this.StructureId.Equals(other.StructureId)
                ) &&
                (
                    this.Struct_Name == other.Struct_Name || this.Struct_Name != null && this.Struct_Name.Equals(other.Struct_Name)
                ) &&
                (
                    this.Stuct_Comments == other.Stuct_Comments || this.Stuct_Comments != null && this.Stuct_Comments.Equals(other.Stuct_Comments)
                ) &&
                (
                    this.Cmp_Comments == other.Cmp_Comments || this.StructureId != null && this.StructureId.Equals(other.StructureId)
                ) &&
                (
                    this.MolecularFormula == other.MolecularFormula || this.MolecularFormula != null && this.MolecularFormula.Equals(other.MolecularFormula)
                ) &&
                (
                    this.FormulaWeight == other.FormulaWeight || this.FormulaWeight != null && this.FormulaWeight.Equals(other.FormulaWeight)
                ) &&
                (
                    this.NormalizedStructure == other.NormalizedStructure || this.NormalizedStructure != null && this.NormalizedStructure.Equals(other.NormalizedStructure)
                );
        }

        /// <summary> 
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            // Overflow is fine, just wrap
            unchecked 
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
                if (this.StructureId != null)
                    hash = hash * 59 + this.StructureId.GetHashCode();
                if (this.Struct_Name != null)
                    hash = hash * 59 + this.Struct_Name.GetHashCode();
                if (this.Stuct_Comments != null)
                    hash = hash * 59 + this.Stuct_Comments.GetHashCode();
                if (this.Cmp_Comments != null)
                    hash = hash * 59 + this.Cmp_Comments.GetHashCode();
                if (this.MolecularFormula != null)
                    hash = hash * 59 + this.MolecularFormula.GetHashCode();
                if (this.FormulaWeight != null)
                    hash = hash * 59 + this.FormulaWeight.GetHashCode();
                if (this.NormalizedStructure != null)
                    hash = hash * 59 + this.NormalizedStructure.GetHashCode();
                return hash;
            }
        }
    }
}
