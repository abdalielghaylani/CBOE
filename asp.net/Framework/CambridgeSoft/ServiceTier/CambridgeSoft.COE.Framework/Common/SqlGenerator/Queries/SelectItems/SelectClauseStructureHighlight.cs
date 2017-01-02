using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Xml;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{

    /// <summary>
    /// Retrieve a higlighted structure based on your search
    /// If no structure was part of the search then you don't get any highlighting
    /// </summary>
	public class SelectClauseStructureHighlight : SelectClauseItem, ISelectClauseParser
	{
		#region Properties
        /// <summary>
        /// The field that contains the alias for the molweight field.
        /// </summary>
        public override string Alias
        {
            get
            {
                if ((base.Alias == null) || (base.Alias == string.Empty))
                {
                    base.Alias = "HighlightedBase64";
                }
                return base.Alias;
            }
            set
            {
                base.Alias = value;
            }
        }

		/// <summary>
		/// The cartridge name.
		/// </summary>
		public string CartridgeSchema {
			get {
				return this.cartridgeSchema;
			}
			set {
				this.cartridgeSchema = value;
			}
		}

        public bool Highlight
        {
            get
            {
                return this.highlight;
            }
            set
            {
                this.highlight = value;
            }
        }

        public string MoleculeContainsClause
        {
            get
            {
                return this.moleculeContainsClause;
            }
            set
            {
                this.moleculeContainsClause = value;
            }
        }

        public string MoleculeContainsOptions
        {
            get
            {
                return this.moleculeContainsOptions;
            }
            set
            {
                this.moleculeContainsOptions = value;
            }
        }

		public override string Name {
			get {
                if(this.Alias != null && this.Alias.Trim() != string.Empty)
                    return this.Alias;

                return this.dataField.GetNameString();

			}
		}
		#endregion

		#region Variables
		private string cartridgeSchema;
        private bool highlight;
        private string moleculeContainsClause;
        private string moleculeContainsOptions;
		#endregion

		#region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public SelectClauseStructureHighlight()
        {
			this.dataField = new Field();
		}
		#endregion

		#region Methods
        /// <summary>
        /// This method does the actual job.
        /// In this case returns the cscartridge formula sintaxys.
        /// </summary>
        /// <param name="dataBaseType">The database type.</param>
        /// <returns>A string containing the select part corresponding to this clause (i.e. the CsCartridge.Molweight(fieldName))</returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> values)
        {
			if (dataBaseType != DBMSType.ORACLE)
				throw new Exception("This select clause is only working in oracle implementations.");
 
            if ((highlight) && (moleculeContainsClause != null))
            {
                //we want to construct this thing
                //decode(CsCartridge.MoleculeContains(cdx, 'Select clause','','highlight=yes',42), 1, CsCartridge.Highlight(42), cdx)
                cartridgeSchema = ConfigurationUtilities.GetChemEngineSchema(dataBaseType);
                StringBuilder builder = new StringBuilder();

                //generate a random number to reference for highlight this might be overkill
                //I think there is something used in memory and I don't want conflicts
                Random random = new Random(DateTime.Now.Millisecond);
                int randomNumber = random.Next(0, 100);


                //create a decod statement
                builder.Append("DECODE(");

                builder.Append(cartridgeSchema);
                builder.Append(".MoleculeContains(");
                string lookupField = string.Empty;
                if(dataField is Lookup)
                {
                    lookupField = ((Lookup) this.dataField).LookupDisplayField.GetNameString();
                    builder.Append(lookupField);
                }
                else
                    builder.Append(this.dataField.GetFullyQualifiedNameString());
                builder.Append(", '");
                builder.Append(moleculeContainsClause);
                builder.Append("','','");
                builder.Append(moleculeContainsOptions);
                builder.Append(",highlight=yes', ");
                builder.Append(randomNumber.ToString());
                builder.Append("), ");
                builder.Append(" 1, ");
                builder.Append(cartridgeSchema);
                builder.Append(".Highlight(");
                builder.Append(randomNumber.ToString());
                builder.Append("), ");
                builder.Append(this.dataField.GetFullyQualifiedNameString());
                builder.Append(")");
                // CSBR-158139 take into account that lookupField may now be quoted in GetNameString
                string fldName = this.dataField.GetNameString();
                string dquote = "\"";
                if (this.dataField is Lookup)
                {
                    // if fldName cotains quoted lookupField
                    if (fldName.Contains(dquote + lookupField + dquote))
                    {
                        // Include quotes in replaced string
                        lookupField = dquote + lookupField + dquote;
                    }
                    return fldName.Replace(lookupField, builder.ToString());
                }
                else
                {
                    return builder.ToString();
                }
            }
            else
            {
                SelectClauseField scf = new SelectClauseField();
                scf.Alias = this.Alias;
                scf.DataField = this.dataField;
                scf.FieldId = this.dataField.FieldId;             
                this.Visible = this.Visible;
                return scf.Execute(dataBaseType, values);
            }			
		}
		#endregion

		#region ISelectClauseParser Members
        /// <summary>
        /// Implementation of ISelectClauseParser. Returns an Instance of SelectClauseMolWeight according to the desired xml snippet.
        /// </summary>
        /// <param name="fieldNode">The CsCartridge MolWeight node of the search results xml definition.</param>
        /// <param name="dvnLookup">The INamesLookup interface from wich the parser can obtain the names corresponding to ids in dataview.xml</param>
        /// <returns>An instance of SelectClauseMolWeight.</returns>
		public SelectClauseItem CreateInstance(System.Xml.XmlNode resultsXmlNode, CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.INamesLookup dvnLookup) {
            SelectClauseStructureHighlight item = new SelectClauseStructureHighlight();
			
			if(resultsXmlNode.Attributes["alias"] != null)
				item.Alias = resultsXmlNode.Attributes["alias"].Value.ToString();

            if (resultsXmlNode.Attributes["visible"] != null && resultsXmlNode.Attributes["visible"].Value != string.Empty)
                this.Visible = bool.Parse(resultsXmlNode.Attributes["visible"].Value);

            if (resultsXmlNode.Attributes["highlight"] != null && resultsXmlNode.Attributes["highlight"].Value != string.Empty)
                item.highlight = bool.Parse(resultsXmlNode.Attributes["highlight"].Value);


            if (resultsXmlNode.Attributes["moleculeContainsClause"] != null && resultsXmlNode.Attributes["moleculeContainsClause"].Value != string.Empty)
                item.moleculeContainsClause = resultsXmlNode.Attributes["moleculeContainsClause"].Value;

            if (resultsXmlNode.Attributes["moleculeContainsOptions"] != null && resultsXmlNode.Attributes["moleculeContainsOptions"].Value != string.Empty)
                item.moleculeContainsOptions = resultsXmlNode.Attributes["moleculeContainsOptions"].Value;


			item.cartridgeSchema = resultsXmlNode.Attributes["CartridgeSchema"] != null ? resultsXmlNode.Attributes["CartridgeSchema"].Value : "CSCARTRIDGE";
			int fieldId = int.Parse(resultsXmlNode.Attributes["fieldId"].Value.Trim());
            item.dataField = dvnLookup.GetColumn(fieldId);

			return item;
		}

		#endregion
	}
}
