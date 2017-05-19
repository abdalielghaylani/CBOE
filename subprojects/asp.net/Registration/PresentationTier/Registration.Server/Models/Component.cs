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
            ID = id;
            Compound = compound;
            ComponentIndex = componentIndex;
            Percentage = percentage;
            StructureId = structureId;
            Struct_Name = struct_Name;
            Stuct_Comments = struct_Comments;
            Cmp_Comments = cmp_Comments;
            MolecularFormula = molecularFormula;
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

            return (
                ID == other.ID || (ID != null && ID.Equals(other.ID))
            ) && (
                Compound == other.Compound || (Compound != null && Compound.Equals(other.Compound))
            ) && (
                ComponentIndex == other.ComponentIndex || (ComponentIndex != null && ComponentIndex.Equals(other.ComponentIndex))
            ) && (
                Percentage == other.Percentage || (Percentage != null && Percentage.Equals(other.Percentage))
            ) && (
                StructureId == other.StructureId || (StructureId != null && StructureId.Equals(other.StructureId))
            ) && (
                Struct_Name == other.Struct_Name || (Struct_Name != null && Struct_Name.Equals(other.Struct_Name))
            ) && (
                Stuct_Comments == other.Stuct_Comments || (Stuct_Comments != null && Stuct_Comments.Equals(other.Stuct_Comments))
            ) && (
                Cmp_Comments == other.Cmp_Comments || (StructureId != null && StructureId.Equals(other.StructureId))
            ) && (
                MolecularFormula == other.MolecularFormula || (MolecularFormula != null && MolecularFormula.Equals(other.MolecularFormula))
            ) && (
                FormulaWeight == other.FormulaWeight || (FormulaWeight != null && FormulaWeight.Equals(other.FormulaWeight))
            ) && (
                NormalizedStructure == other.NormalizedStructure || (NormalizedStructure != null && NormalizedStructure.Equals(other.NormalizedStructure))
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
                if (ID != null)
                    hash = (hash * 59) + ID.GetHashCode();
                if (Compound != null)
                    hash = (hash * 59) + Compound.GetHashCode();
                if (ComponentIndex != null)
                    hash = (hash * 59) + ComponentIndex.GetHashCode();
                if (Percentage != null)
                    hash = (hash * 59) + Percentage.GetHashCode();
                if (StructureId != null)
                    hash = (hash * 59) + StructureId.GetHashCode();
                if (Struct_Name != null)
                    hash = (hash * 59) + Struct_Name.GetHashCode();
                if (Stuct_Comments != null)
                    hash = (hash * 59) + Stuct_Comments.GetHashCode();
                if (Cmp_Comments != null)
                    hash = (hash * 59) + Cmp_Comments.GetHashCode();
                if (MolecularFormula != null)
                    hash = (hash * 59) + MolecularFormula.GetHashCode();
                if (FormulaWeight != null)
                    hash = (hash * 59) + FormulaWeight.GetHashCode();
                if (NormalizedStructure != null)
                    hash = (hash * 59) + NormalizedStructure.GetHashCode();
                return hash;
            }
        }
    }
}
