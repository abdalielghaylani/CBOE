using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COEConfigurationService;


namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{
    /// <summary>
    /// <para>
    /// This class allows the user to perform a CsCartridge.CDXToMolFile SQL Statement. Which is implemented only in Oracle by now.
    /// </para>
    /// <para>
    /// <b>Input</b>
    /// </para>
    /// <para>
    /// The SelectClauseCDXToMolFile class requires the following members to be initialized to the desired value:
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
    /// CDXToMolFile molfile = new CDXToMolFile();
    /// molfile.DataField.FieldId = 20;
    /// molfile.DataField.FieldName = "base64_cdx";
    /// molfile.DataField.FieldType = System.Data.SqlDbType.VarBinary;
    /// molfile.DataField.Table.TableName = "inv_compounds";
    /// molfile.CartridgeSchema = "CSCARTRIDGE";
    /// query.AddSelectItem(molfile);
    /// </code>
    /// 
    /// <b>With XML:</b>
    /// <code lang="XML">
    /// &lt;CsCartridgeCDXToMolFile fieldId="20" CartridgeSchema="CSCARTRIDGE" /&gt;
    /// </code>
    /// <para>
    /// <b>Output</b>
    /// </para>
    /// <para>
    /// This code will produce the following select clause statement:
    /// </para>
    /// <para>
    /// CSCARTRIDGE.CDXToMolFile(inv_compounds.base64_cdx, '')
    /// </para>
    /// </summary>
    public class SelectClauseCDXToMolFile : SelectClauseItem, ISelectClauseParser
    {
        #region Properties
        /// <summary>
        /// The cartridge name.
        /// </summary>
        public string CartridgeSchema
        {
            get
            {
                return this.cartridgeSchema;
            }
            set
            {
                this.cartridgeSchema = value;
            }
        }

        public override string Name
        {
            get
            {
                if (this.Alias != null && this.Alias.Trim() != string.Empty)
                    return this.Alias;

                return this.dataField.GetNameString();
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
        public SelectClauseCDXToMolFile()
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
            //LJB 2/2/2009 fixe hardcoding of cartridge schema name
            cartridgeSchema = ConfigurationUtilities.GetChemEngineSchema(dataBaseType);
            SQLGeneratorData sqlGenerateData = ConfigurationUtilities.GetMolFileFormat(dataBaseType);

            StringBuilder builder = new StringBuilder();

            if (this.dataField is Lookup)
            {
                string dblQuote = "\"";
                string cnvCdx = cartridgeSchema + ".ConvertCDX.CDXToMolFile({0}{1})";
                string lookUpFieldName = this.dataField.GetNameString();
                string lookUpDisplayField = dblQuote + ((Lookup)this.dataField).LookupDisplayField.GetNameString() + dblQuote;
                string newField = string.Format(cnvCdx, lookUpDisplayField, sqlGenerateData.MolFileFormat == "V3000" ? ",'V3000'" : "");
                builder.Append(lookUpFieldName.Replace(lookUpDisplayField, newField));
            }
            else
            {
                builder.Append(cartridgeSchema);
                builder.Append(".ConvertCDX.CDXToMolFile(");
                builder.Append(this.dataField.GetFullyQualifiedNameString());

                if (sqlGenerateData.MolFileFormat == "V3000")
                {
                    builder.Append(",'V3000'");
                }

                builder.Append(")");
            }
            return builder.ToString();
        }
        #endregion

        #region ISelectClauseParser Members
        /// <summary>
        /// Implementation of ISelectClauseParser. Returns an Instance of SelectClauseCDXToMolFile according to the desired xml snippet.
        /// </summary>
        /// <param name="fieldNode">The CsCartridge MolWeight node of the search results xml definition.</param>
        /// <param name="dvnLookup">The INamesLookup interface from wich the parser can obtain the names corresponding to ids in dataview.xml</param>
        /// <returns>An instance of SelectClauseCDXToMolFile.</returns>
        public SelectClauseItem CreateInstance(System.Xml.XmlNode resultsXmlNode, CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.INamesLookup dvnLookup)
        {
            SelectClauseCDXToMolFile item = new SelectClauseCDXToMolFile();

            if (resultsXmlNode.Attributes["alias"] != null)
                item.Alias = resultsXmlNode.Attributes["alias"].Value.ToString();

            if (resultsXmlNode.Attributes["visible"] != null && resultsXmlNode.Attributes["visible"].Value != string.Empty)
                item.Visible = bool.Parse(resultsXmlNode.Attributes["visible"].Value);
            item.cartridgeSchema = resultsXmlNode.Attributes["CartridgeSchema"] != null ? resultsXmlNode.Attributes["CartridgeSchema"].Value : "CSCARTRIDGE";

            item.DataField = dvnLookup.GetColumn(int.Parse(resultsXmlNode.Attributes["fieldId"].Value.Trim()));

            return item;
        }

        #endregion
    }
}
