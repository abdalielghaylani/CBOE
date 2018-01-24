using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COEConfigurationService;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems
{
    /// <summary>
    /// <para>
    /// This class allows the user to perform a CsCartridge.Formula SQL Statement. Which is implemented only in Oracle by now.
    /// </para>
    /// <para>
    /// <b>Input</b>
    /// </para>
    /// <para> 
    /// The SelectClauseFormula class requires the following members to be initialized to the desired value:
    /// </para>
    /// <list type="bullet">
    /// <item><b>Field DataField:</b> The field where is located the molecule structure.</item>
    /// <item><b>string CartridgeSchema:</b> The name of the cartridge schema.</item>
    /// <item><b>string Sortable: for the Formula function to generate formula strings where the elements are two characters wide, and the numbers are three characters wide with leading zeros.</b> .</item>
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
    /// SelectClauseFormula formula = new SelectClauseFormula();
    /// formula.DataField.FieldId = 20;
    /// formula.DataField.FieldName = "base64_cdx";
    /// formula.DataField.FieldType = System.Data.SqlDbType.VarBinary;
    /// formula.DataField.Table.TableName = "inv_compounds";
    /// formula.CartridgeSchema = "CSCARTRIDGE";
    /// query.AddSelectItem(itemConcatenation);query.AddSelectItem(formula);
    /// </code>
    /// 
    /// <b>With XML:</b>
    /// <code lang="XML">
    /// &lt;CsCartridgeFormula fieldId="20" CartridgeSchema="CSCARTRIDGE" /&gt;
    /// </code>
    /// <para>
    /// <b>Output</b>
    /// </para>
    /// <para>
    /// This code will produce the following select clause statement:
    /// </para>
    /// <para>
    /// CSCARTRIDGE.Formula(inv_compounds.base64_cdx, '')
    /// </para>
    /// </summary>
    public class SelectClauseFormula : SelectClauseItem, ISelectClauseParser
    {
        #region Properties

        /// <summary>
        /// The field that contains the alias for the formula field.
        /// </summary>
        public override string Alias
        {
            get
            {
                if ((base.Alias == null) || (base.Alias == string.Empty))
                {
                    base.Alias = "Formula";
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

                return this.DataField.GetNameString();

            }

        }

        public bool Sortable
        {
            get
            {
                return this.sortable;
            }
            set
            {
                this.sortable = value;
            }
        }

        /// <summary>
        /// If sortable is Yes, this property is discarded
        /// </summary>
        public bool HTMLFormatted
        {
            get
            {
                return this.htmlFormatted;
            }
            set
            {
                this.htmlFormatted = value;
            }
        }

        #endregion

        #region Variables
        private string cartridgeSchema;
        private bool sortable;
        private bool htmlFormatted;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its values to its default values.
        /// </summary>
        public SelectClauseFormula()
        {
            this.DataField = new Field();
            this.sortable = false;
            this.htmlFormatted = false;
        }


        #endregion

        #region Methods
        /// <summary>
        /// This method does the actual job.
        /// In this case returns the cscartridge formula sintaxys.
        /// </summary>
        /// <param name="dataBaseType">The database type.</param>
        /// <returns>A string containing the select part corresponding to this clause (i.e. the CsCartridge.Formula(fieldName,''))</returns>
        protected override string GetDependantString(DBMSType dataBaseType, List<Value> values)
        {
            if (dataBaseType != DBMSType.ORACLE)
            {
                throw new Exception("This select clause is only working in oracle implementations.");
            }

            cartridgeSchema = ConfigurationUtilities.GetChemEngineSchema(dataBaseType);
            var builder = new StringBuilder(cartridgeSchema);
            builder.Append(".Formula(");
            builder.Append(this.DataField.GetFullyQualifiedNameString());
            builder.Append(", ");

            builder.Append("'");
            builder.Append(this.sortable ? "SORTABLE=YES" : "");
            builder.Append(this.sortable && this.htmlFormatted ? "," : "");
            builder.Append(this.htmlFormatted ? "HTML=YES" : "");
            builder.Append("'");

            builder.Append(")");

            return builder.ToString();
        }
        #endregion

        #region ISelectClauseParser Members
        /// <summary>
        /// Implementation of ISelectClauseParser. Returns an Instance of SelectClauseFormula according to the desired xml snippet.
        /// </summary>
        /// <param name="fieldNode">The CsCartridge Formula node of the search results xml definition.</param>
        /// <param name="dvnLookup">The INamesLookup interface from wich the parser can obtain the names corresponding to ids in dataview.xml</param>
        /// <returns>An instance of SelectClauseFormula.</returns>
        public SelectClauseItem CreateInstance(System.Xml.XmlNode resultsXmlNode, CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.INamesLookup dvnLookup)
        {
            var item = new SelectClauseFormula();

            if (resultsXmlNode.Attributes["alias"] != null)
            {
                item.Alias = resultsXmlNode.Attributes["alias"].Value.ToString();
            }

            if (resultsXmlNode.Attributes["sortable"] != null)
            {
                item.sortable = bool.Parse(resultsXmlNode.Attributes["sortable"].Value);
            }

            if (resultsXmlNode.Attributes["htmlFormatted"] != null)
            {
                item.htmlFormatted = bool.Parse(resultsXmlNode.Attributes["htmlFormatted"].Value);
            }

            //item.DataField = dvnLookup.GetColumn(int.Parse(fieldNode.Attributes["fieldId"].Value.Trim()));
            if (resultsXmlNode.Attributes["visible"] != null
                && resultsXmlNode.Attributes["visible"].Value != string.Empty)
            {
                item.Visible = bool.Parse(resultsXmlNode.Attributes["visible"].Value);
            }

            item.cartridgeSchema = resultsXmlNode.Attributes["CartridgeSchema"] != null ? resultsXmlNode.Attributes["CartridgeSchema"].Value : "CSCARTRIDGE";
            
            var fldId = int.Parse(resultsXmlNode.Attributes["fieldId"].Value.Trim());

            item.DataField = dvnLookup.GetColumn(fldId);

            return item;
        }
        #endregion
    }
}
