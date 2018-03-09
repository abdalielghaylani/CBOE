using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator
{
	/// <summary>
	/// This class stores a table information.
    /// The logic for obtaining its string representation is implemented here.
	/// </summary>
	public class Table : ISource
	{
		#region Properties
		/// <summary>
		/// Name of the table in the xml schema.
		/// </summary>
		public string TableName {
			get {
				return this.tableName;
			}
			set {
				this.tableName = value;
			}
		}

		/// <summary>
		/// The id of the table in the xml schema.
		/// </summary>
		public int TableId {
			get {
				return this.tableId;
			}
			set {
				this.tableId = value;
			}
		}

		/// <summary>
		/// The table alias to be used.
		/// </summary>
		public string Alias {
            get
            {
                if (!string.IsNullOrEmpty(alias) && alias.Length > 30)
                {
                    string hashStr = GetHashCode().ToString();
                    return alias.Substring(0, 29 - hashStr.Length) + "|" + hashStr;
                }
                else
                    return this.alias;
            }
			set {
				this.alias = value;
			}
		}

		/// <summary>
		/// Name of the database or schema.
		/// </summary>
		public string Database {
			get {
				return this.database;
			}
			set {
				this.database = value;
			}
		}

		public List<Value> ParamValues {
			get {
				return null;
			}
			set {
                ;
			}
		}
		#endregion

		#region Variables
		/// <summary>
		/// Name of the database or schema.
		/// </summary>
		private string database;
		/// <summary>
		/// Name of the table in the xml schema.
		/// </summary>
		private string tableName;

		/// <summary>
		/// The id of the table in the xml schema.
		/// </summary>
		private int tableId;

		/// <summary>
		/// The table alias to be used.
		/// </summary>
		private string alias;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes its members to its default values.
		/// </summary>
		public Table() {
			this.database = string.Empty;
			this.tableName = string.Empty;
			this.Alias = string.Empty;
			this.tableId = (int) Decimal.MinusOne;
		}

		/// <summary>
		/// Initializes its members to the desired values.
		/// </summary>
		public Table(int id, string name, string alias, string database) {
			this.database = database;
			this.tableId = id;
			this.tableName = name;
			this.alias = alias;
		}

		/// <summary>
		/// Initializes its members to its default values except of its name.
		/// </summary>
		public Table(string name) : this() 
		{
			this.tableName = name;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Two tables are equals if all of its members has the same values.
		/// </summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>True if they are equals false if not.</returns>
		public override bool Equals(object obj) {
			if(ReferenceEquals(this, obj)) {
				return true;
			}

			if(obj == null)
				return false;

			if(obj.GetType() == typeof(Table)) {
				Table otherTable = (Table) obj;
				if(this.TableName == otherTable.TableName &&
					this.Alias == otherTable.Alias &&
					this.TableId == otherTable.TableId &&
					this.database == otherTable.database) {
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
			string uniqueString = this.database + "." + this.TableName + "." + this.TableId + ":" + this.alias;
			return uniqueString.GetHashCode();
		}

		/// <summary>
		/// Determines if two tables are equal or not.
		/// </summary>
		/// <param name="left">Left member of the operation.</param>
		/// <param name="right">Right member of the operation.</param>
		/// <returns>True if equals, false otherwise.</returns>
		public static bool operator ==(Table left, Table right) {
			if(ReferenceEquals(left, right))
				return true;
			else if(ReferenceEquals(right, null))
				return false;
			else
				return left.Equals(right);
		}

		/// <summary>
		/// Determines if two tables are different or not.
		/// </summary>
		/// <param name="left">Left member of the operation.</param>
		/// <param name="right">Right member of the operation.</param>
		/// <returns>True if defferents, false otherwise.</returns>
		public static bool operator !=(Table left, Table right) {
			 if(ReferenceEquals(left, right))
				return false;
			else if(ReferenceEquals(right, null))
				return true;
			else
				return !left.Equals(right);

		}
		#endregion

        #region ISource Members
        /// <summary>
        /// Returns the alias of the table if defined, otherwise returns string.Empty. If longer than 30 chars the alias is trimmed
        /// </summary>
        /// <returns>The alias or string.Empty.</returns>
        public string GetAlias() {
            return (string.IsNullOrEmpty(this.Alias) ? string.Empty : "\"" + this.Alias + "\"");
        }

        /// <summary>
        /// Gets the string that represents the fully qualified name of this table.
        /// </summary>
        /// <returns>The fully qualified name.</returns>
        public override string ToString() {
            StringBuilder builder = new StringBuilder(string.Empty);
            if (this.tableName.Contains("."))
            {
                int idx = this.tableName.LastIndexOf(".");
                string DB = this.tableName.Substring(0, idx);
                string Tablename = this.tableName.Substring(idx + 1);
                this.database = DB;
                this.tableName = Tablename;
            }
            if (this.database != null && this.database != string.Empty)
                builder.Append("\"" + this.database + "\".");

            if (this.tableName != null && this.tableName != string.Empty)
                builder.Append("\"" + this.tableName + "\"");
            return builder.ToString();
        }

        public string ToString(List<Value> paramValues)
        {
            return this.ToString();
        }
        #endregion
    }
}
