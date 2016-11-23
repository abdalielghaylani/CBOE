

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{
    using System.Xml;
    using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;
    using System.Collections.Generic;

    using System.Text;
    
    public class SelectClauseRowId : SelectClauseItem, ISelectClauseParser
    {
        #region Properties

        /// <summary>
        /// Table Alias
        /// </summary>
        public string TableAlias { get; set; }

        /// <summary>
		/// Gets or sets the id of the DataBase Field.
		/// </summary>
		public int FieldId {
			get
            {
				return this.fieldId;
			}
			set
            {
				this.fieldId = value;
			}
		}

		public override string Name {
			get
            {
				return this.DataField.GetNameString();
			}
		}
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes its values to its default values.
        /// </summary>
        public SelectClauseRowId()
        {
            this.Alias = COEDataView.RowIdField.ReservedFieldAliasRowId;
            this.dataField = new Field();
        }

        #endregion

        #region Methods

        /// <summary>
        /// This method does the actual job.
		/// In this case just return the name of the field.
        /// </summary>
        /// <returns> TableAlias.ROWID </returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> values)
        {
            const string DoubleQuote = @"""";
            StringBuilder builder = new StringBuilder(string.Empty);

            // table alias name
            if (!string.IsNullOrEmpty(TableAlias))
            {
                builder.Append("\""+TableAlias+"\"");
                builder.Append(".");
            }

            // field name
            builder.Append(DoubleQuote);
            builder.Append("ROWID");
            builder.Append(DoubleQuote);

            return builder.ToString();
        }

        #endregion

		#region ISelectClauseParser Members

		/// <summary>
		/// Implementation of ISelectClauseParser. Returns an Instance of SelectClauseField, which at this point
		/// has only set its id.
		/// </summary>
		/// <param name="fieldNode">The field node of the search results xml definition.</param>
		/// <param name="dvnLookup">The INamesLookup interface from wich the parser can obtain the names corresponding to ids in dataview.xml</param>
		/// <returns>An instance of SelectClauseField.</returns>
		public SelectClauseItem CreateInstance(XmlNode fieldNode, INamesLookup dvnLookup)
        {
            return new SelectClauseRowId();
		}

		#endregion
	}
}
