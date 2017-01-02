using System;
using System.Data;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator
{
    /// <summary>
    /// This class stores a field information.
    /// The logic for obtaining its string representation is implemented here.
    /// </summary>
    public class Field : IColumn
    {
        #region Properties
        /// <summary>
        /// Name of the field. 
        /// Can be ignored if the field is used for storing a parameter for a sql function.
        /// </summary>
        public string FieldName {
            get {
                return this.name;
            }
            set {
                name = value;
            }
        }

        /// <summary>
        /// DBType of the field.
        /// </summary>

        public string OrderByIdName
        {
            get
            {
                return this.orderbyname;
            }
            set
            {
                orderbyname = value;
            }
        }

        /// <summary>
        /// DBType of the field.
        /// </summary>
        
        public DbType FieldType {
            get {
                return this.type;
            }
            set {
                this.type = value;
            }
        }

		/// <summary>
		/// The table related to the field.
		/// </summary>
        public ISource Table {
			get {
				return this.table;
			}
			set {
				this.table = value;
			}
		}

		/// <summary>
		/// Gets or sets the identifier of the field.
		/// </summary>
		public int FieldId {
			get {
				return this.id;
			}
			set {
				this.id = value;
			}
		}

        /// <summary>
        /// Gets or sets the mime type of the field.
        /// </summary>
        public COEDataView.MimeTypes MimeType {
            get {
                return this.mimeType;
            }
            set {
                this.mimeType = value;
            }
        }

        #endregion

        #region Variables
        /// <summary>
        /// Name of the field. 
        /// Can be ignored if the field is used for storing a parameter for a sql function.
        /// </summary>
        private string name;

        /// <summary>
        /// DBType of the field. Used for producing the string representation of the field.
        /// </summary>
        private string orderbyname;

        private DbType type;

		/// <summary>
		/// The table related to the field.
		/// </summary>
		private ISource table;

		/// <summary>
		/// Its identifier.
		/// </summary>
		private int id;

        /// <summary>
        /// Its mime type.
        /// </summary>
        private COEDataView.MimeTypes mimeType = COEDataView.MimeTypes.NONE;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public Field() {
            this.name = string.Empty;
            this.type = DbType.String;
			this.table = new Table();
			this.id = (int) Decimal.MinusOne;
            this.mimeType = COEDataView.MimeTypes.NONE;
        }

        /// <summary>
        /// Initializes its members to the desired values.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The field name</param>
        /// <param name="type">Its DbType</param>
		public Field(int id, string name, DbType type)
			: this() {
			this.id = id;
			this.name = name;
			this.type = type;
		}
		
        /// <summary>
        /// Initializes its members to the desired values.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <param name="type">Its DbType.</param>
        public Field(string name, DbType type)
			: this() {
			this.name = name;
			this.type = type;
		}
		
        /// <summary>
        /// Initializes its members to the desired values.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <param name="type">Its DbType.</param>
        /// <param name="parentTable">Its Parent Table.</param>
        public Field(string name, DbType type, ISource parentTable)
			: this(name, type) {
			this.table = parentTable;
		}
        #endregion

        #region Methods
        /// <summary>
        /// Gets the string representation of the field fully qualified name.
        /// </summary>
        /// <returns>A string cointaining the string representation of the name of the Field. 
        /// i.e. SPONSORS.SPONSORID</returns>
        public string GetFullyQualifiedNameString() {
            string doubleQuote = @"""";
			StringBuilder builder = new StringBuilder(string.Empty);

			if(this.table.GetAlias() != string.Empty) {
                builder.Append(this.table.GetAlias());
                builder.Append(".");
			} else if(this.table.ToString().Length > 0){
                builder.Append(this.table.ToString());
                builder.Append(".");
			}
            builder.Append(doubleQuote);
            builder.Append(this.name);
            builder.Append(doubleQuote);

			return builder.ToString();
        }

		public string GetNameString() {
            
			return this.name;
		}

		/// <summary>
		/// Two fields are equals if all of its members has the same values.
		/// </summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>True if they are equals false if not.</returns>
		public override bool Equals(object obj) {
			if(ReferenceEquals(this, obj)){
				return true;
			}

			if(obj.GetType() == typeof(Field)) {
				Field otherField = (Field) obj;
				if(this.FieldName == otherField.FieldName &&
					this.Table == otherField.Table &&
					this.FieldType == otherField.FieldType &&
					this.FieldId == otherField.FieldId) {
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
            // Coverity Fix CID - 11702
            string uniqueString = this.Table.ToString() + "." + this.FieldName + ":" + this.FieldType + "=" + this.FieldId;			
			return uniqueString.GetHashCode();
		}
		
		/// <summary>
		/// Determines if two field are equal or not.
		/// </summary>
		/// <param name="left">Left member of the operation.</param>
		/// <param name="right">Right member of the operation.</param>
		/// <returns>True if equals, false otherwise.</returns>
		public static bool operator ==(Field left, Field right){
			if(ReferenceEquals(left, right))
				return true;
			else if(ReferenceEquals(right, null))
				return false;
			return left.Equals(right);
		}

		/// <summary>
		/// Determines if two field are different or not.
		/// </summary>
		/// <param name="left">Left member of the operation.</param>
		/// <param name="right">Right member of the operation.</param>
		/// <returns>True if defferents, false otherwise.</returns>
		public static bool operator !=(Field left, Field right) {
			if(ReferenceEquals(left, right))
				return false;
			else if(ReferenceEquals(right, null))
				return true;
			return !left.Equals(right);
		}
        #endregion
    }
}
