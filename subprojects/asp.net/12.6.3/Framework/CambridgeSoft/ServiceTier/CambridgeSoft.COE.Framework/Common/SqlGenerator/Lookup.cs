using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator {
    /// <summary>
    /// A lookup is a column that belongs to a table, but it's information is extracted from another table. Those table are linked by a one to one relationship
    /// </summary>
	public class Lookup : IColumn {
        
        #region Variables
        private ISource _lookupTable;
        private ISource _table;
        private int _fieldId;
        private string _fieldName;
        private COEDataView.MimeTypes _mimeType;

        IColumn _lookupField;
        IColumn _lookupDisplayField;
        /*private int lookupFieldId;
        private string lookupFieldName;
        private int _lookupDisplayFieldId;
        private string _lookupDisplayFieldName;*/
        #endregion

        #region Properties
        /// <summary>
        /// The table related to the lookup field.
        /// </summary>
        public ISource LookupTable {
            get { return this._lookupTable; }
            set { this._lookupTable = value; }
        }

        /// <summary>
        /// The table related to the source field.
        /// </summary>
        public ISource Table {
            get { return this._table; }
            set { this._table = value; }
        }


        public IColumn LookupField
        {
            get {
                return this._lookupField;
            }
            set {
                this._lookupField = value;
            }
        }

        public IColumn LookupDisplayField
        {
            get {
                return this._lookupDisplayField;
            }
            set {
                this._lookupDisplayField = value;
            }
        }

        /// <summary>
        /// The lookup field id.
        /// </summary>
        public int LookupFieldId {
            get { return _lookupField.FieldId; }
            set { /*lookupFieldId = value; */}
        }

        /// <summary>
        /// The lookup field name.
        /// </summary>
        public string LookupFieldName {
            get { return _lookupField.GetNameString(); }
            set { /*lookupFieldName = value; */}
        }

        /// <summary>
        /// The lookup field mime type.
        /// </summary>
        public COEDataView.MimeTypes LookupFieldMimeType
        {
            get { return _lookupField.MimeType; }
            set { /*lookupFieldMimeType = value; */}
        }

        /// <summary>
        /// Id of the lookup field to display.
        /// </summary>
        /*public int LookupDisplayFieldId {
            get { return _lookupDisplayFieldId; }
            set { _lookupDisplayFieldId = value; }
        }

        /// <summary>
        /// Name of the lookup field to display.
        /// </summary>
        public string LookupDisplayFieldName {
            get { return _lookupDisplayFieldName; }
            set { _lookupDisplayFieldName = value; }
        }*/

        /// <summary>
        /// Id of the source field.
        /// </summary>
        public int FieldId {
            get { return this._fieldId; }
            set { this._fieldId = value; }
        }

        /// <summary>
        /// Name of the source field.
        /// </summary>
        public string FieldName {
            get { return this._fieldName; }
            set { this._fieldName = value; }
        }

        /// <summary>
        /// Mime type of the column
        /// </summary>
        public COEDataView.MimeTypes MimeType
        {
            get { return this._mimeType; }
            set { this._mimeType = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the string representation of the lookup, which is in the form: 
        ///     (SELECT DISPLAYFIELDNAME 
        ///         FROM LOOKUPTABLE
        ///         WHERE SOURCEFIELDNAME = LOOKUPFIELDNAME)
        /// 
        /// Where sourcefieldname and lookupfieldname are fully qualified.
        /// </summary>
        /// <returns>A string of the SQL lookup.</returns>
        public string GetFullyQualifiedNameString() {
            string doubleQuote = @"""";
            StringBuilder builder = new StringBuilder("(select ");
            // Conditionally add quote see: CSBR-157577 and CSBR-158139
            builder.Append(MaybeQuote(LookupDisplayField.GetNameString()));
            builder.Append(" from ");
            builder.Append(LookupTable.ToString());
           // builder.Append(LookupTable.GetAlias() != string.Empty ? LookupTable.GetAlias() : LookupTable.ToString());
            builder.Append(" where ");
            builder.Append(LookupField.GetNameString());
            builder.Append("=");
            builder.Append(Table.GetAlias() != string.Empty ? Table.GetAlias() : Table.ToString());
            builder.Append(".");
            builder.Append(doubleQuote);
            builder.Append(FieldName);
            builder.Append(doubleQuote);
            builder.Append(" AND ROWNUM<2 )");
            return builder.ToString();
        }
		public string GetNameString() {
			return this.GetFullyQualifiedNameString();
		}

        // CSBR-157577 and CSBR-158139
        // The string being passed into this function is the display field of the lookup.
        // It may be a columnname or a more complex expression such an Oracle DECODE or
        // other function.  We want to quote and uppercase it if it is a simple columnname. 
        // This will protect us against reserved Oracle keywords used in columnnames.
        // However, if it is a longer expression or a name that already contains doublequotes
        // then we don't wont to quote it.
        private string MaybeQuote(string s)
        {
            string doubleQuote = @"""";
            
            // Less that 30 characters without any doublequotes likely
            // means that it is a simple columnname that can be quoted.
            if (s.Length < 30 && !s.Contains(doubleQuote))
            {
                return doubleQuote + s + doubleQuote;
            }
            else
            {
                return s;    
            }
        }


        #endregion
    }
}
