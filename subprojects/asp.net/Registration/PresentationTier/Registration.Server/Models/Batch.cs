/* 
 * Registration 17 API
 */

using System;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Services.Types;

namespace PerkinElmer.COE.Registration.Server.Models
{
    /// <summary>
    /// Batch
    /// </summary>
    [DataContract]
    public partial class Batch : IEquatable<Batch>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Batch" /> class.
        /// </summary>
        /// <param name="id">ID.</param>
        /// <param name="tempBatchID">TempBatchID.</param>
        /// <param name="batchNumber">BatchNumber.</param>
        /// <param name="fullRegNumber">FullRegNumber.</param>
        /// <param name="dateCreated">DateCreated.</param>
        /// <param name="personCreated">PersonCreated.</param>
        /// <param name="personRegistered">PersonRegistered.</param>
        /// <param name="personApproved">PersonApproved.</param>
        /// <param name="dateLastModified">DateLastModified.</param>
        /// <param name="status">Status.</param>
        /// <param name="projectList">ProjectList.</param>
        /// <param name="propertyList">PropertyList.</param>
        /// <param name="identifierList">IdentifierList.</param>
        /// <param name="batchComponentList">BatchComponentList.</param>
        public Batch(int? id = default(int?), int? tempBatchID = default(int?), int? batchNumber = default(int?), string fullRegNumber = default(string), DateTime? dateCreated = default(DateTime?), int? personCreated = default(int?), int? personRegistered = default(int?), int? personApproved = default(int?), DateTime? dateLastModified = default(DateTime?), RegistryStatus status = default(RegistryStatus), ProjectList projectList = default(ProjectList), PropertyList propertyList = default(PropertyList), IdentifierList identifierList = default(IdentifierList), BatchComponentList batchComponentList = default(BatchComponentList))
        {
            this.ID = id;
            this.TempBatchID = tempBatchID;
            this.BatchNumber = batchNumber;
            this.FullRegNumber = fullRegNumber;
            this.DateCreated = dateCreated;
            this.PersonCreated = personCreated;
            this.PersonRegistered = personRegistered;
            this.PersonApproved = personApproved;
            this.DateLastModified = dateLastModified;
            this.Status = status;
            this.ProjectList = projectList;
            this.PropertyList = propertyList;
            this.IdentifierList = identifierList;
            this.BatchComponentList = batchComponentList;
        }

        /// <summary>
        /// Gets or sets ID
        /// </summary>
        [DataMember(Name = "ID", EmitDefaultValue = false)]
        public int? ID { get; set; }

        /// <summary>
        /// Gets or sets TempBatchID
        /// </summary>
        [DataMember(Name = "TempBatchID", EmitDefaultValue = false)]
        public int? TempBatchID { get; set; }

        /// <summary>
        /// Gets or sets BatchNumber
        /// </summary>
        [DataMember(Name = "BatchNumber", EmitDefaultValue = false)]
        public int? BatchNumber { get; set; }

        /// <summary>
        /// Gets or sets FullRegNumber
        /// </summary>
        [DataMember(Name = "FullRegNumber", EmitDefaultValue = false)]
        public string FullRegNumber { get; set; }

        /// <summary>
        /// Gets or sets DateCreated
        /// </summary>
        [DataMember(Name = "DateCreated", EmitDefaultValue = false)]
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Gets or sets PersonCreated
        /// </summary>
        [DataMember(Name = "PersonCreated", EmitDefaultValue = false)]
        public int? PersonCreated { get; set; }

        /// <summary>
        /// Gets or sets PersonRegistered
        /// </summary>
        [DataMember(Name = "PersonRegistered", EmitDefaultValue = false)]
        public int? PersonRegistered { get; set; }

        /// <summary>
        /// Gets or sets PersonApproved
        /// </summary>
        [DataMember(Name = "PersonApproved", EmitDefaultValue = false)]
        public int? PersonApproved { get; set; }

        /// <summary>
        /// Gets or sets DateLastModified
        /// </summary>
        [DataMember(Name = "DateLastModified", EmitDefaultValue = false)]
        public DateTime? DateLastModified { get; set; }

        /// <summary>
        /// Gets or sets Status
        /// </summary>
        [DataMember(Name = "Status", EmitDefaultValue = false)]
        public RegistryStatus Status { get; set; }

        /// <summary>
        /// Gets or sets ProjectList
        /// </summary>
        [DataMember(Name = "ProjectList", EmitDefaultValue = false)]
        public ProjectList ProjectList { get; set; }

        /// <summary>
        /// Gets or sets PropertyList
        /// </summary>
        [DataMember(Name = "PropertyList", EmitDefaultValue = false)]
        public PropertyList PropertyList { get; set; }

        /// <summary>
        /// Gets or sets IdentifierList
        /// </summary>
        [DataMember(Name = "IdentifierList", EmitDefaultValue = false)]
        public IdentifierList IdentifierList { get; set; }

        /// <summary>
        /// Gets or sets BatchComponentList
        /// </summary>
        [DataMember(Name = "BatchComponentList", EmitDefaultValue = false)]
        public BatchComponentList BatchComponentList { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Batch {\n");
            sb.Append("  ID: ").Append(ID).Append("\n");
            sb.Append("  TempBatchID: ").Append(TempBatchID).Append("\n");
            sb.Append("  BatchNumber: ").Append(BatchNumber).Append("\n");
            sb.Append("  FullRegNumber: ").Append(FullRegNumber).Append("\n");
            sb.Append("  DateCreated: ").Append(DateCreated).Append("\n");
            sb.Append("  PersonCreated: ").Append(PersonCreated).Append("\n");
            sb.Append("  PersonRegistered: ").Append(PersonRegistered).Append("\n");
            sb.Append("  PersonApproved: ").Append(PersonApproved).Append("\n");
            sb.Append("  DateLastModified: ").Append(DateLastModified).Append("\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
            sb.Append("  ProjectList: ").Append(ProjectList).Append("\n");
            sb.Append("  PropertyList: ").Append(PropertyList).Append("\n");
            sb.Append("  IdentifierList: ").Append(IdentifierList).Append("\n");
            sb.Append("  BatchComponentList: ").Append(BatchComponentList).Append("\n");
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
            return this.Equals(obj as Batch);
        }

        /// <summary>
        /// Returns true if Batch instances are equal
        /// </summary>
        /// <param name="other">Instance of Batch to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Batch other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return
                (
                    this.ID == other.ID ||
                    this.ID != null &&
                    this.ID.Equals(other.ID)
                ) &&
                (
                    this.TempBatchID == other.TempBatchID ||
                    this.TempBatchID != null &&
                    this.TempBatchID.Equals(other.TempBatchID)
                ) &&
                (
                    this.BatchNumber == other.BatchNumber ||
                    this.BatchNumber != null &&
                    this.BatchNumber.Equals(other.BatchNumber)
                ) &&
                (
                    this.FullRegNumber == other.FullRegNumber ||
                    this.FullRegNumber != null &&
                    this.FullRegNumber.Equals(other.FullRegNumber)
                ) &&
                (
                    this.DateCreated == other.DateCreated ||
                    this.DateCreated != null &&
                    this.DateCreated.Equals(other.DateCreated)
                ) &&
                (
                    this.PersonCreated == other.PersonCreated ||
                    this.PersonCreated != null &&
                    this.PersonCreated.Equals(other.PersonCreated)
                ) &&
                (
                    this.PersonRegistered == other.PersonRegistered ||
                    this.PersonRegistered != null &&
                    this.PersonRegistered.Equals(other.PersonRegistered)
                ) &&
                (
                    this.PersonApproved == other.PersonApproved ||
                    this.PersonApproved != null &&
                    this.PersonApproved.Equals(other.PersonApproved)
                ) &&
                (
                    this.DateLastModified == other.DateLastModified ||
                    this.DateLastModified != null &&
                    this.DateLastModified.Equals(other.DateLastModified)
                ) &&
                (
                    this.Status == other.Status ||
                    this.Status != null &&
                    this.Status.Equals(other.Status)
                ) &&
                (
                    this.ProjectList == other.ProjectList ||
                    this.ProjectList != null &&
                    this.ProjectList.Equals(other.ProjectList)
                ) &&
                (
                    this.PropertyList == other.PropertyList ||
                    this.PropertyList != null &&
                    this.PropertyList.Equals(other.PropertyList)
                ) &&
                (
                    this.IdentifierList == other.IdentifierList ||
                    this.IdentifierList != null &&
                    this.IdentifierList.Equals(other.IdentifierList)
                ) &&
                (
                    this.BatchComponentList == other.BatchComponentList ||
                    this.BatchComponentList != null &&
                    this.BatchComponentList.Equals(other.BatchComponentList)
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
                if (this.TempBatchID != null)
                    hash = hash * 59 + this.TempBatchID.GetHashCode();
                if (this.BatchNumber != null)
                    hash = hash * 59 + this.BatchNumber.GetHashCode();
                if (this.FullRegNumber != null)
                    hash = hash * 59 + this.FullRegNumber.GetHashCode();
                if (this.DateCreated != null)
                    hash = hash * 59 + this.DateCreated.GetHashCode();
                if (this.PersonCreated != null)
                    hash = hash * 59 + this.PersonCreated.GetHashCode();
                if (this.PersonRegistered != null)
                    hash = hash * 59 + this.PersonRegistered.GetHashCode();
                if (this.PersonApproved != null)
                    hash = hash * 59 + this.PersonApproved.GetHashCode();
                if (this.DateLastModified != null)
                    hash = hash * 59 + this.DateLastModified.GetHashCode();
                if (this.Status != null)
                    hash = hash * 59 + this.Status.GetHashCode();
                if (this.ProjectList != null)
                    hash = hash * 59 + this.ProjectList.GetHashCode();
                if (this.PropertyList != null)
                    hash = hash * 59 + this.PropertyList.GetHashCode();
                if (this.IdentifierList != null)
                    hash = hash * 59 + this.IdentifierList.GetHashCode();
                if (this.BatchComponentList != null)
                    hash = hash * 59 + this.BatchComponentList.GetHashCode();
                return hash;
            }
        }
    }

}
