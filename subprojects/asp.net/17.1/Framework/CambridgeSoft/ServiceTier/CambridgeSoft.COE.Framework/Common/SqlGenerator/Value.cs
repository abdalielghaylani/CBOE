using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator
{
	/// <summary>
    /// This class stores values of fields.
    /// The app values can have a different type than the field they are related to.
	/// </summary>
	public class Value
	{

		#region properties
		/// <summary>
		/// Value of the field.
		/// Can be ignored if the field is used for storing a column name only.
		/// </summary>
		public string Val {
			get {
				return this.value;
			}
			set {
				this.value = value;
			}
		}

		/// <summary>
		/// Type of the value. Used for producing the string representation of the value.
		/// </summary>
		public DbType Type {
			get {
				return this.type;
			}
			set {
				this.type = value;
			}
		}
		#endregion

		#region Variables
		/// <summary>
		/// Value of the field.
		/// Can be ignored if the field is used for storing a column name only.
		/// </summary>
		private string value;

		/// <summary>
		/// Type of the field. Used for producing the string representation of the value.
		/// </summary>
		private DbType type;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes its members to its default values.
		/// </summary>
		public Value() {
			this.value = string.Empty;
            this.type = DbType.String;
		}

		/// <summary>
		/// Initializes its members to the given parameter values.
		/// </summary>
		public Value(string value, DbType type) {
			this.value = value;
			this.type = type;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Two values are equals if all of its members has the same values.
		/// </summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>True if they are equals false if not.</returns>
		public override bool Equals(object obj) {
			if(ReferenceEquals(this, obj)) {
				return true;
			}

			if(obj.GetType() == typeof(Value)) {
				Value otherValue = (Value) obj;
				if(this.Type == otherValue.Type &&
					this.Val == otherValue.Val) {
					return true;
				} else {
					return false;
				}
			} else {
				return false;
			}
		}

		/// <summary>
		/// Returns a hash code.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode() {
            // Coverity Fix CID - 11704
			string uniqueString = this.Val + ":" + this.Type.ToString();
			return uniqueString.GetHashCode();
		}

		/// <summary>
		/// Determines if two values are equal or not.
		/// </summary>
		/// <param name="left">Left member of the operation.</param>
		/// <param name="right">Right member of the operation.</param>
		/// <returns>True if equals, false otherwise.</returns>
		public static bool operator ==(Value left, Value right) {
			if(ReferenceEquals(left, right))
				return true;
			else if(ReferenceEquals(right, null))
				return false;
			return left.Equals(right);
		}

		/// <summary>
		/// Determines if two values are different or not.
		/// </summary>
		/// <param name="left">Left member of the operation.</param>
		/// <param name="right">Right member of the operation.</param>
		/// <returns>True if defferents, false otherwise.</returns>
		public static bool operator !=(Value left, Value right) {
			if(ReferenceEquals(left, right))
				return false;
			else if(ReferenceEquals(right, null))
				return true;
			return !left.Equals(right);
		}

        /// <summary>
        /// Returns its string representation. Which is its the value itself, but if needed, the value is simple quotted.
        /// </summary>
        /// <returns>The string value.</returns>
		public override string ToString() {
			switch(TypesConversor.GetAbstractType(this.Type)) {
                case COEDataView.AbstractTypes.Text:
                case COEDataView.AbstractTypes.Date:
					return "'" + this.Val + "'";
				default:
					return this.Val;
			}
		}
		#endregion
	}
}
