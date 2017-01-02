using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COEConfigurationService;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{
	/// <summary> 
	/// <para>
	/// This class allows the user to perform a CsCartridge.MolWeight SQL Statement. Which is implemented only in Oracle by now.
	/// </para>
	/// <para>
	/// <b>Input</b>
	/// </para>
	/// <para>
	/// The SelectClauseMolWeight class requires the following members to be initialized to the desired value:
	/// </para>
	/// <list type="bullet">
	/// <item><b>Field DataField:</b> The field where is located the molecule structure.</item>
	/// <item><b>string CartridgeSchema:</b> The name of the cartridge schema.</item>
	/// </list>
	/// <para>
	/// <b>Notes:</b>
	/// </para>
	/// <para>
	/// This class supports only Oracle cartridge implementation.
	/// SelectClauses are implemented in two differentiable parts: The SelectClause itself, in charge of generating the string and a Parser whose responsibility is to extract the information needed for building the clause from an xmlnode.
	/// All SelectClause are mapped in mappings.xml, wich indicates given a SelectClause name, wich class to use for parsing and obtaining the generated string. There is a set of predefined SelectClauseItems, but the user can expand with his own. By implementing SelectClauseItem and ISelectClauseParser and adding the corresponding entry in this file.
	/// Along with SelectClause component of Query class, this class follows the command pattern, having the GetDependantString(DataBaseType) method as the “Execute Method”
	/// </para>
	/// <para>
	/// <b>Example:</b>
	/// </para>
	/// <b>Programatically:</b>
	/// <code lang="C#">
	/// SelectClauseMolWeight molweight = new SelectClauseMolWeight();
	/// molweight.DataField.FieldId = 20;
	/// molweight.DataField.FieldName = "base64_cdx";
	/// molweight.DataField.FieldType = System.Data.SqlDbType.VarBinary;
	/// molweight.DataField.Table.TableName = "inv_compounds";
	/// molweight.CartridgeSchema = "CSCARTRIDGE";
	/// query.AddSelectItem(molweight);
	/// </code>
	/// 
	/// <b>With XML:</b>
	/// <code lang="XML">
	/// &lt;CsCartridgeMolWeight fieldId="20" CartridgeSchema="CSCARTRIDGE" /&gt;
	/// </code>
	/// <para>
	/// <b>Output</b>
	/// </para>
	/// <para>
	/// This code will produce the following select clause statement:
	/// </para>
	/// <para>
	/// CSCARTRIDGE.MolWeight(inv_compounds.base64_cdx,'')
	/// </para>
	/// </summary>
	public class SelectClauseMolWeight : SelectClauseItem, ISelectClauseParser
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
                    base.Alias = "MolWeight";
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

		public override string Name {
			get {
                if(this.Alias != null && this.Alias.Trim() != string.Empty)
                    return this.Alias;

                return this.DataField.GetNameString();

			}
		}
		#endregion

		#region Variables
		private string cartridgeSchema;
		#endregion

		#region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
		public SelectClauseMolWeight() {
            this.DataField = new Field();
		}
		#endregion

		#region Methods
        /// <summary>
        /// This method does the actual job.
        /// In this case returns the cscartridge formula sintaxys.
        /// </summary>
        /// <param name="dataBaseType">The database type.</param>
        /// <returns>A string containing the select part corresponding to this clause (i.e. the CsCartridge.MolWeight(fieldName,''))</returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> values)
        {
			if (dataBaseType != DBMSType.ORACLE)
			{
			    throw new Exception("This select clause is only working in oracle implementations.");
			}

            cartridgeSchema = ConfigurationUtilities.GetChemEngineSchema(dataBaseType);
            var builder = new StringBuilder(cartridgeSchema);
            builder.Append(".MolWeight(");
			builder.Append(this.DataField.GetFullyQualifiedNameString());
			builder.Append(")");

			return builder.ToString();
		}
		#endregion

		#region ISelectClauseParser Members
        /// <summary>
        /// Implementation of ISelectClauseParser. Returns an Instance of SelectClauseMolWeight according to the desired xml snippet.
        /// </summary>
        /// <param name="fieldNode">The CsCartridge MolWeight node of the search results xml definition.</param>
        /// <param name="dvnLookup">The INamesLookup interface from wich the parser can obtain the names corresponding to ids in dataview.xml</param>
        /// <returns>An instance of SelectClauseMolWeight.</returns>
		public SelectClauseItem CreateInstance(System.Xml.XmlNode resultsXmlNode, CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.INamesLookup dvnLookup)
        {
			var item = new SelectClauseMolWeight();
			
			if (resultsXmlNode.Attributes["alias"] != null)
			{
			    item.Alias = resultsXmlNode.Attributes["alias"].Value;
			}

            if (resultsXmlNode.Attributes["visible"] != null
                && resultsXmlNode.Attributes["visible"].Value != string.Empty)
            {
                this.Visible = bool.Parse(resultsXmlNode.Attributes["visible"].Value);
            }

            item.cartridgeSchema = resultsXmlNode.Attributes["CartridgeSchema"] != null ? resultsXmlNode.Attributes["CartridgeSchema"].Value : "CSCARTRIDGE";

			var fldId = int.Parse(resultsXmlNode.Attributes["fieldId"].Value.Trim());
            item.dataField = dvnLookup.GetColumn(fldId);

			return item;
		}

		#endregion
	}
}
