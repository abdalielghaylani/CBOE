using System;
using System.Xml;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;
using CambridgeSoft.COE.Framework.Properties;
using System.Collections.Generic;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{
	
    public class SelectClauseSortByHitList : SelectClauseItem, ISelectClauseParser
    {

        #region Properties
		/// <summary>
		/// Gets or sets the id of the DataBase Field.
		/// </summary>
		public int FieldId {
			get {
				return this.fieldId;
			}
			set {
				this.fieldId = value;
			}
		}

		public override string Name {
			get {
                return this.DataField.GetFullyQualifiedNameString();
			}
		}

		
        #endregion

        #region Variables
		/// <summary>
		/// Id of the DataBase Field.
		/// </summary>
		protected int fieldId;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its values to its default values.
        /// </summary>
        public SelectClauseSortByHitList() {
            this.DataField = new Field();
        }
        public SelectClauseSortByHitList(IColumn column)
        {
            this.DataField = column;
        }
        #endregion

        #region Methods
        /// <summary>
        /// This method does the actual job.
		/// In this case just return the name of the field.
        /// </summary>
        /// <returns>A string containing the select part corresponding to this field (i.e. the field name)</returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> values)
        {
            return this.DataField.GetFullyQualifiedNameString();
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
		public SelectClauseItem CreateInstance(XmlNode fieldNode, INamesLookup dvnLookup) {
            SelectClauseSortByHitList item = new SelectClauseSortByHitList();
            if(fieldNode.Attributes["alias"] != null && fieldNode.Attributes["alias"].Value != string.Empty) {
                item.Alias = fieldNode.Attributes["alias"].Value.ToString();
            } else {
                item.Alias = dvnLookup.GetColumnAlias(int.Parse(fieldNode.Attributes["fieldId"].Value.Trim()));
            }

            if (fieldNode.Attributes["visible"] != null && fieldNode.Attributes["visible"].Value != string.Empty)
                item.Visible = bool.Parse(fieldNode.Attributes["visible"].Value);

            item.DataField = dvnLookup.GetColumn(int.Parse(fieldNode.Attributes["fieldId"].Value.Trim()));

			return item;
		}
		#endregion
	}
}
