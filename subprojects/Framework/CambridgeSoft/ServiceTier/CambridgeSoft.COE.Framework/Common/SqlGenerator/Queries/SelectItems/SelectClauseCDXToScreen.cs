using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COEConfigurationService;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems {
    /// <summary>
    /// <para>
    /// This class allows the user to perform a CsCartridge.CONVERTCDX.CDXTOSCREEN SQL Statement. Which is implemented only in Oracle by now.
    /// </para>
    /// <para>
    /// <b>Input</b>
    /// </para>
    /// <para>
    /// The SelectClauseCDXToScreen class requires the following members to be initialized to the desired value:
    /// </para>
    /// <list type="bullet">
    /// <item><b>Field DataField:</b> The field where is located the molecule structure. If this is not provided the Structure field would be used as input.</item>
    /// <item><b>string Structure:</b> The structure representation to get the screem from if a DataField was not provided.</item>
    /// <item><b>string CartridgeSchema:</b> The name of the cartridge schema.</item>
    /// <item><b>string Source: Allowed values are NORMAL, SIMILAR, FULLEXACT, SKELETAL.</b> .</item>
    /// <item><b>string OutputForm: Allowed values are LIST, BITSRING.</b> .</item>
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
    /// SelectClauseCDXToScreen screen = new SelectClauseCDXToScreen();
    /// screen.Structure = "c1ccccc1";
    /// screen.SimilarityType = "normal";
    /// screen.CartridgeSchema = "CSCARTRIDGE";
    /// query.AddSelectItem(screen);
    /// </code>
    /// 
    /// <b>With XML:</b>
    /// <code lang="XML">
    /// &lt;screen CartridgeSchema="CSCARTRIDGE" structure="c1ccccc1" source="NORMAL" outputForm="BITSRING" /&gt;
    /// </code>
    /// <para>
    /// <b>Output</b>
    /// </para>
    /// <para>
    /// This code will produce the following select clause statement:
    /// </para>
    /// <para>
    /// (CSCARTRIDGE.CONVERTCDX.CDXTOSCREEN('c1ccccc1', 'normal', '') from dual)
    /// </para>
    /// </summary>
    public class SelectClauseCDXToScreen : SelectClauseItem, ISelectClauseParser {
        // (select cscartridge.convertcdx.cdxtoscreen('c1ccccc1', 'normal', '') from dual)) 
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
                    base.Alias = "screen";
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
        /// Parameter for selecting the source, allowable values are NORMAL, SIMILAR, FULLEXACT and SKELETAL. Default value is NORMAL
        /// </summary>
        public string Source {
            get { return _source; }
            set { 
                value = value.ToUpper();
                if(value != "NORMAL" && value != "SIMILAR" && value != "FULLEXACT" && value != "SKELETAL") {
                    value = "NORMAL";
                }
                _source = value; 
            }
        }

        /// <summary>
        /// Parameter for selecting the output, allowable values are LIST, BITSRING. Default value is BITSRING
        /// </summary>
        public string OutputForm {
            get { return _outputForm; }
            set {
                value = value.ToUpper();
                if(value != "LIST" && value != "BITSRING") {
                    value = "BITSRING";
                }
                _outputForm = value; 
            }
        }

        /// <summary>
        /// If no field is provided then this is used for getting the screen.
        /// </summary>
        public string Structure {
            get { return _structure; }
            set { _structure = value; }
        }
        #endregion

        #region Variables
        private string _cartridgeSchema;
        private string _structure;
        private string _source;
        private string _outputForm;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its values to its default values.
        /// </summary>
        public SelectClauseCDXToScreen()
        {
            this.DataField = new Field();
            this.DataField.FieldId = -1;
            _source = "NORMAL";
            _outputForm = "BITSRING";
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
            //(select cscartridge.convertcdx.cdxtoscreen('c1ccccc1', 'normal', '') from dual))
            _cartridgeSchema = ConfigurationUtilities.GetChemEngineSchema(dataBaseType);
            StringBuilder builder = new StringBuilder();
            if(this.DataField.FieldId < 0) {
                builder.Append("(SELECT ");
            }
            builder.Append(_cartridgeSchema);
            builder.Append(".CONVERTCDX.CDXTOSCREEN(");
            if (this.DataField.FieldId < 0)
            {
                builder.AppendFormat("'{0}'", _structure);
            } else {
                builder.Append(this.DataField.GetFullyQualifiedNameString());
            }
            builder.AppendFormat(", '{0}'", _source);
            builder.AppendFormat(", '{0}')", _outputForm);

            if (this.DataField.FieldId < 0)
            {
                builder.AppendFormat(" from dual)");
            }

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
        public SelectClauseItem CreateInstance(System.Xml.XmlNode resultsXmlNode, CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.INamesLookup dvnLookup)
        {
            SelectClauseCDXToScreen item = new SelectClauseCDXToScreen();
            if (resultsXmlNode.Attributes["alias"] != null)
                item.Alias = resultsXmlNode.Attributes["alias"].Value;

            if(resultsXmlNode.Attributes["structure"] != null) {
                item.Structure = resultsXmlNode.Attributes["structure"].Value;
            }

            if(resultsXmlNode.Attributes["source"] != null) {
                item.Source = resultsXmlNode.Attributes["source"].Value;
            }

            if(resultsXmlNode.Attributes["outputForm"] != null) {
                item.OutputForm = resultsXmlNode.Attributes["outputForm"].Value;
            }

            //item.DataField = dvnLookup.GetColumn(int.Parse(fieldNode.Attributes["fieldId"].Value.Trim()));
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
        
