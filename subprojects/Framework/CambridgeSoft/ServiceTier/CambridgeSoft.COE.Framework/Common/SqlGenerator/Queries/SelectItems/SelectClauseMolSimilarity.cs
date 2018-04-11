using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Xml;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems {
    /// <summary>
    /// <para>
    /// This class allows the user to perform a CsCartridge.SIMILARITY SQL Statement. Which is implemented only in Oracle by now.
    /// </para>
    /// <para>
    /// <b>Input</b>
    /// </para>
    /// <para>
    /// The SelectClauseCDXToScreen class requires the following members to be initialized to the desired value:
    /// </para>
    /// <list type="bullet">
    /// <item><b>Field DataField:</b> The field where is located the molecule structure.</item>
    /// <item><b>string Structure:</b> The structure representation to get the screem from if a DataField was not provided.</item>
    /// <item><b>string CartridgeSchema:</b> The name of the cartridge schema.</item>
    /// <item><b>string ScreenResultCriteria: The resultcriteria to get an screen from a query structure.</b> .</item>
    /// <item><b>string Screen: If the screen is provided (and not a screenresultcriteri) then it is used to get the similarity.</b> .</item>
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
    /// SelectClauseMolSimilarity similarity = new SelectClauseMolSimilarity();
    /// similarity.DataField.FieldId = 20;
    /// SelectClauseCDXToScreen screen = new SelectClauseCDXToScreen();
    /// screen.Structure = "c1ccccc1";
    /// screen.SimilarityType = "normal";
    /// screen.CartridgeSchema = "CSCARTRIDGE";
    /// similarity.ScreenResultCriteria = screen;
    /// similarity.SimilarityType = "normal";
    /// similarity.CartridgeSchema = "CSCARTRIDGE";
    /// query.AddSelectItem(similarity);
    /// </code>
    /// 
    /// <b>With XML:</b>
    /// <code lang="XML">
    /// &lt;similarity CartridgeSchema="CSCARTRIDGE" fieldId="20"&gt;
    ///    &lt;screen CartridgeSchema="CSCARTRIDGE" structure="c1ccccc1" source="NORMAL" outputForm="BITSRING" /&gt;
    /// &lt;similarity/&gt;
    /// </code>
    /// <para>
    /// <b>Output</b>
    /// </para>
    /// <para>
    /// This code will produce the following select clause statement:
    /// </para>
    /// <para>
    /// CsCartridge.Similarity(base64_cdx, (select cscartridge.convertcdx.cdxtoscreen('c1ccccc1', 'normal', '') from dual)) "similarity"
    /// </para>
    /// </summary>
    public class SelectClauseMolSimilarity : SelectClauseItem, ISelectClauseParser {

        #region Properties
        /// <summary>
        /// The field that contains the _alias for the formula field.
        /// </summary>
        public override string Alias
        {
            get
            {
                if ((base.Alias == null) || (base.Alias == string.Empty))
                {
                    base.Alias = "similarity";
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
                return this._cartridgeSchema;
            }
            set
            {
                this._cartridgeSchema = value;
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

        /// <summary>
        /// The screen bitvector for the query structure.
        /// </summary>
        public string Screen {
            get { return _screenString; }
            set { _screenString = value; }
        }

        /// <summary>
        /// If a select for getting the screen is needed, then use this instead of Screen parameter.
        /// </summary>
        public SelectClauseCDXToScreen ScreenResultCriteria {
            get { return _screenResultCriteria; }
            set { _screenResultCriteria = value; }
        }
        #endregion

        #region Variables
        private string _cartridgeSchema;
        private string _screenString;
        private SelectClauseCDXToScreen _screenResultCriteria;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its values to its default values.
        /// </summary>
        public SelectClauseMolSimilarity()
        {
            this.DataField = new Field();
            this.DataField.FieldId = -1;
            _screenString = string.Empty;
            _screenResultCriteria = null;
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
                throw new Exception("This select clause is only working in oracle implementations.");

            //CsCartridge.Similarity(mol, (select cscartridge.convertcdx.cdxtoscreen('c1ccccc1', 'normal', '') from dual)) "similarity"
            _cartridgeSchema = ConfigurationUtilities.GetChemEngineSchema(dataBaseType);
            StringBuilder builder = new StringBuilder(_cartridgeSchema);
            builder.Append(".SIMILARITY(");
            builder.Append(this.DataField.GetFullyQualifiedNameString());
            if(_screenResultCriteria != null) {
                builder.AppendFormat(", '{0}')", _screenResultCriteria.Execute(dataBaseType, values).Replace("'", "''"));
            }
            else
                builder.AppendFormat(", '{0}')", _screenString);
            
            return builder.ToString();
        }
        #endregion

        #region ISelectClauseParser Members
        /// <summary>
        /// Implementation of ISelectClauseParser. Returns an Instance of SelectClauseCDXToScreen according to the desired xml snippet.
        /// </summary>
        /// <param name="fieldNode">The CsCartridge CDXToScreen node of the search results xml definition.</param>
        /// <param name="dvnLookup">The INamesLookup interface from wich the parser can obtain the names corresponding to ids in dataview.xml</param>
        /// <returns>An instance of SelectClauseCDXToScreen.</returns>
        public SelectClauseItem CreateInstance(XmlNode resultsXmlNode, CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.INamesLookup dvnLookup)
        {
            SelectClauseMolSimilarity item = new SelectClauseMolSimilarity();
            if (resultsXmlNode.Attributes["alias"] != null)
                item.Alias = resultsXmlNode.Attributes["alias"].Value;

            if(resultsXmlNode.FirstChild != null) {
                item.ScreenResultCriteria = (SelectClauseCDXToScreen) new SelectClauseCDXToScreen().CreateInstance(resultsXmlNode.FirstChild, dvnLookup);
            }
            else if(resultsXmlNode.Attributes["screen"] != null) {
                item.Screen = resultsXmlNode.Attributes["screen"].Value;
            }

            if (resultsXmlNode.Attributes["visible"] != null && resultsXmlNode.Attributes["visible"].Value != string.Empty)
                item.Visible = bool.Parse(resultsXmlNode.Attributes["visible"].Value);

            item.CartridgeSchema = resultsXmlNode.Attributes["CartridgeSchema"] != null ? resultsXmlNode.Attributes["CartridgeSchema"].Value : "CSCARTRIDGE";

            if(resultsXmlNode.Attributes["fieldId"] != null && !string.IsNullOrEmpty(resultsXmlNode.Attributes["fieldId"].Value)) {
                int fieldId = int.Parse(resultsXmlNode.Attributes["fieldId"].Value.Trim());
                if(fieldId > 0)
                    item.DataField = dvnLookup.GetColumn(fieldId);
            }

            return item;
        }
        #endregion
    }
}
