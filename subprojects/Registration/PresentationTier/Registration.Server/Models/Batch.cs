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
        /// <param name="id">ID</param>
        /// <param name="tempBatchID">Temporary batch ID</param>
        /// <param name="batchNumber">Batch number.</param>
        /// <param name="fullRegNumber">Full registration  number</param>
        /// <param name="dateCreated">Date created</param>
        /// <param name="personCreated">Person created</param>
        /// <param name="personRegistered">Person registered</param>
        /// <param name="personApproved">Person approved</param>
        /// <param name="dateLastModified">Date last modified</param>
        /// <param name="status">Status</param>
        /// <param name="projectList">Project list</param>
        /// <param name="propertyList">Property list</param>
        /// <param name="identifierList">Identifier list</param>
        /// <param name="batchComponentList">Batch component list</param>
        public Batch(int? id = default(int?), int? tempBatchID = default(int?), int? batchNumber = default(int?), string fullRegNumber = default(string), DateTime? dateCreated = default(DateTime?), int? personCreated = default(int?), int? personRegistered = default(int?), int? personApproved = default(int?), DateTime? dateLastModified = default(DateTime?), RegistryStatus status = default(RegistryStatus), ProjectList projectList = default(ProjectList), PropertyList propertyList = default(PropertyList), IdentifierList identifierList = default(IdentifierList), BatchComponentList batchComponentList = default(BatchComponentList))
        {
            ID = id;
            TempBatchID = tempBatchID;
            BatchNumber = batchNumber;
            FullRegNumber = fullRegNumber;
            DateCreated = dateCreated;
            PersonCreated = personCreated;
            PersonRegistered = personRegistered;
            PersonApproved = personApproved;
            DateLastModified = dateLastModified;
            Status = status;
            ProjectList = projectList;
            PropertyList = propertyList;
            IdentifierList = identifierList;
            BatchComponentList = batchComponentList;
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
        /// Gets or sets the batch number
        /// </summary>
        [DataMember(Name = "BatchNumber", EmitDefaultValue = false)]
        public int? BatchNumber { get; set; }

        /// <summary>
        /// Gets or sets the full registration number
        /// </summary>
        [DataMember(Name = "FullRegNumber", EmitDefaultValue = false)]
        public string FullRegNumber { get; set; }

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        [DataMember(Name = "DateCreated", EmitDefaultValue = false)]
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the creator
        /// </summary>
        [DataMember(Name = "PersonCreated", EmitDefaultValue = false)]
        public int? PersonCreated { get; set; }

        /// <summary>
        /// Gets or sets the person who registered
        /// </summary>
        [DataMember(Name = "PersonRegistered", EmitDefaultValue = false)]
        public int? PersonRegistered { get; set; }

        /// <summary>
        /// Gets or sets the person who approved
        /// </summary>
        [DataMember(Name = "PersonApproved", EmitDefaultValue = false)]
        public int? PersonApproved { get; set; }

        /// <summary>
        /// Gets or sets the last modification date
        /// </summary>
        [DataMember(Name = "DateLastModified", EmitDefaultValue = false)]
        public DateTime? DateLastModified { get; set; }

        /// <summary>
        /// Gets or sets the status
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

            return (
                ID == other.ID || (ID != null && ID.Equals(other.ID))
            ) && (
                TempBatchID == other.TempBatchID || (TempBatchID != null && TempBatchID.Equals(other.TempBatchID))
            ) && (
                BatchNumber == other.BatchNumber || (BatchNumber != null && BatchNumber.Equals(other.BatchNumber))
            ) && (
                FullRegNumber == other.FullRegNumber || (FullRegNumber != null && FullRegNumber.Equals(other.FullRegNumber))
            ) && (
                DateCreated == other.DateCreated || (DateCreated != null && DateCreated.Equals(other.DateCreated))
            ) && (
                PersonCreated == other.PersonCreated || (PersonCreated != null && PersonCreated.Equals(other.PersonCreated))
            ) && (
                PersonRegistered == other.PersonRegistered || (PersonRegistered != null && PersonRegistered.Equals(other.PersonRegistered))
            ) && (
                PersonApproved == other.PersonApproved || (PersonApproved != null && PersonApproved.Equals(other.PersonApproved))
            ) && (
                DateLastModified == other.DateLastModified || (DateLastModified != null && DateLastModified.Equals(other.DateLastModified))
            ) && (
                Status == other.Status || Status.Equals(other.Status)
            ) && (
                ProjectList == other.ProjectList || (ProjectList != null && ProjectList.Equals(other.ProjectList))
            ) && (
                PropertyList == other.PropertyList || (PropertyList != null && PropertyList.Equals(other.PropertyList))
            ) && (
                IdentifierList == other.IdentifierList || (IdentifierList != null && IdentifierList.Equals(other.IdentifierList))
            ) && (
                BatchComponentList == other.BatchComponentList || (BatchComponentList != null && BatchComponentList.Equals(other.BatchComponentList))
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
                if (TempBatchID != null)
                    hash = (hash * 59) + TempBatchID.GetHashCode();
                if (BatchNumber != null)
                    hash = (hash * 59) + BatchNumber.GetHashCode();
                if (FullRegNumber != null)
                    hash = (hash * 59) + FullRegNumber.GetHashCode();
                if (DateCreated != null)
                    hash = (hash * 59) + DateCreated.GetHashCode();
                if (PersonCreated != null)
                    hash = (hash * 59) + PersonCreated.GetHashCode();
                if (PersonRegistered != null)
                    hash = (hash * 59) + PersonRegistered.GetHashCode();
                if (PersonApproved != null)
                    hash = (hash * 59) + PersonApproved.GetHashCode();
                if (DateLastModified != null)
                    hash = (hash * 59) + DateLastModified.GetHashCode();
                hash = (hash * 59) + Status.GetHashCode();
                if (ProjectList != null)
                    hash = (hash * 59) + ProjectList.GetHashCode();
                if (PropertyList != null)
                    hash = (hash * 59) + PropertyList.GetHashCode();
                if (IdentifierList != null)
                    hash = (hash * 59) + IdentifierList.GetHashCode();
                if (BatchComponentList != null)
                    hash = (hash * 59) + BatchComponentList.GetHashCode();
                return hash;
            }
        }
    }

}
