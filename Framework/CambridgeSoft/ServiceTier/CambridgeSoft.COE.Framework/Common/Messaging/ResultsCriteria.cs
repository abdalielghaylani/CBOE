using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using CambridgeSoft.COE.Framework.Common.Utility;

namespace CambridgeSoft.COE.Framework.Common
{
    using System.Linq;

    using CambridgeSoft.COE.Framework.Common.Messaging;

    /// ResultsCriteria
    /// <summary>
    /// Class that provides messaging capabilities to communicate some result criteria.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "resultsCriteria", Namespace = "COE.ResultsCriteria", DataType = "resultsCriteria")]
    [XmlType(TypeName = "resultsCriteria", IncludeInSchema = true, Namespace = "COE.ResultsCriteria")]
    [TypeConverterAttribute(typeof(ResultsCriteriaTypeConverter))]
    public class ResultsCriteria
    {
        #region Properties
        /// <summary>
        /// The list of tables to be retrieved.
        /// </summary>
        [XmlArray("tables", Namespace = "COE.ResultsCriteria")]
        [XmlArrayItem("table", Type = typeof(ResultsCriteriaTable))]
        public List<ResultsCriteriaTable> Tables
        {
            get { return this.tables; }
            set { this.tables = value; }
        }

        /// <summary>
        /// The xml namespace.
        /// </summary>
        [XmlIgnore]
        public string XmlNS
        {
            get { return xmlNS; }
            set { xmlNS = value; }
        }


        [XmlAttribute("sortByHitList")]
        public bool SortByHitList
        {
            get { return _sortByHitList; }
            set { _sortByHitList = value; }
        }

        /// <summary>
        /// Gets a resultcriteria table based on its Table ID
        /// </summary>
        /// <param name="resultCriteriaTableId">Table ID</param>
        /// <returns></returns>
        public ResultsCriteriaTable this[int resultCriteriaTableId]
        {
            get
            {
                foreach (ResultsCriteriaTable currentTable in this.tables)
                    if (currentTable.Id == resultCriteriaTableId)
                        return currentTable;

                return null;
            }
        }
        #endregion

        #region Variables
        private List<ResultsCriteriaTable> tables;
        private string xmlNS;
        private string xmlNamespace;
        private bool _sortByHitList;

        [NonSerialized]
        private ResultsCriteria fullResultsCriteria = null;

        [NonSerialized]
        private XmlNamespaceManager manager;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public ResultsCriteria()
        {
            this.tables = new List<ResultsCriteriaTable>();
            this.xmlNS = "COE.ResultsCriteria";
            xmlNamespace = "COE";
            manager = new XmlNamespaceManager(new NameTable());
            manager.AddNamespace(xmlNamespace, this.xmlNS);
        }

        /// <summary>
        /// Initialies its members from its xml representation.
        /// </summary>
        /// <param name="doc">The xml representation.</param>
        public ResultsCriteria(XmlDocument doc)
        {
            this.xmlNS = "COE.ResultsCriteria";
            xmlNamespace = "COE";
            manager = new XmlNamespaceManager(doc.NameTable);
            manager.AddNamespace(xmlNamespace, this.xmlNS);
            this.GetFromXML(doc);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Load its members from its xml representation.
        /// </summary>
        /// <param name="xmlResultsCriteria">The xml representation.</param>
        public void GetFromXML(XmlDocument xmlResultsCriteria)
        {
            XmlNode tablesNodeList = xmlResultsCriteria.SelectSingleNode("//" + xmlNamespace + ":tables", this.manager);

            this.tables = new List<ResultsCriteriaTable>();
            // Coverity Fix CID - 13128
            if (tablesNodeList != null)
            {
                foreach (XmlNode tableNode in tablesNodeList.ChildNodes)
                {
                    if (tableNode.NodeType == XmlNodeType.Element)
                    {
                        ResultsCriteriaTable tbl = new ResultsCriteriaTable(tableNode);
                        this.tables.Add(tbl);
                    }
                }
            }

            XmlNode resultsCriteriaNode = xmlResultsCriteria.SelectSingleNode("//" + xmlNamespace + ":resultsCriteria", this.manager);
            if (resultsCriteriaNode != null)  //Coverity Fix CID 13128 ASV
            {
                if (resultsCriteriaNode.Attributes["xmlns"] != null && resultsCriteriaNode.Attributes["xmlns"].Value != string.Empty)
                    xmlNS = resultsCriteriaNode.Attributes["xmlns"].Value;
                if (resultsCriteriaNode.Attributes["sortByHitList"] != null && resultsCriteriaNode.Attributes["sortByHitList"].Value != string.Empty)
                    _sortByHitList = bool.Parse(resultsCriteriaNode.Attributes["sortByHitList"].Value);
            }
        }

        /// <summary>
        /// Fills the result criteria object from it's xml representation contained in a string.
        /// </summary>
        /// <param name="xmlResultsCriteria">The string containing the result criteria xml representation</param>
        public void GetFromXML(string xmlResultsCriteria)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlResultsCriteria);
            this.GetFromXML(doc);
        }

        public ResultsCriteria Clone()
        {
            return ResultsCriteria.GetResultsCriteria(this.ToString());
        }

        /// <summary>
        /// Builds its xml string representation.
        /// </summary>
        /// <returns>The xml string representation.</returns>
        public override string ToString()
        {
            return Utilities.XmlSerialize(this);
        }

        /// <summary>
        /// Deserialize the ResultsCriteria from xml string.
        /// </summary>
        /// <param name="xml">ResultsCriteria object xml string.</param>
        /// <returns>
        /// ResultsCriteria object.
        /// </returns>
        public static ResultsCriteria GetResultsCriteria(string xml)
        {
            return Utilities.XmlDeserialize<ResultsCriteria>(xml);
        }

        /// <summary>
        /// Gets the MolWeightType type by index type.
        /// </summary>
        /// <param name="indexType">The COEDataView index type.</param>
        /// <returns>
        /// Returns the MolWeight result criteria type. 
        /// </returns>
        public static Type GetResultMolWeightTypeByIndexType(COEDataView.IndexTypes indexType)
        {
            Type molWeightType;

            switch (indexType)
            {
                case COEDataView.IndexTypes.CS_CARTRIDGE:
                    molWeightType = typeof(ResultsCriteria.MolWeight);
                    break;
                case COEDataView.IndexTypes.DIRECT_CARTRIDGE:
                    molWeightType = typeof(ResultsCriteria.DirectMolWeight);
                    break;
                case COEDataView.IndexTypes.JCHEM_CARTRIDGE:
                    molWeightType = typeof(ResultsCriteria.JChemMolWeight);
                    break;
                default:
                    throw new NotSupportedException(string.Format("Does not support IndexType of '{0}'", indexType));
            }

            return molWeightType;
        }

        /// <summary>
        /// Gets the formula criteria type by index type.
        /// </summary>
        /// <param name="indexType">The COEDataView index type enum.</param>
        /// <returns>
        /// Returns the formula type name.
        /// </returns>
        public static Type GetResultFormulaTypeByIndexType(COEDataView.IndexTypes indexType)
        {
            Type formulaType;

            switch (indexType)
            {
                case COEDataView.IndexTypes.CS_CARTRIDGE:
                    formulaType = typeof(ResultsCriteria.Formula);
                    break;
                case COEDataView.IndexTypes.DIRECT_CARTRIDGE:
                    formulaType = typeof(ResultsCriteria.DirectFormula);
                    break;
                case COEDataView.IndexTypes.JCHEM_CARTRIDGE:
                    formulaType = typeof(ResultsCriteria.JChemFormula);
                    break;
                default:
                    throw new NotSupportedException(string.Format("Does not support IndexType of '{0}'", indexType));
            }

            return formulaType;
        }

        /// <summary>
        /// To check if the type is formual type or not.
        /// </summary>
        /// <param name="type">The type name.</param>
        /// <returns>
        /// Return true if it is cs, direct or jchem formula type.
        /// </returns>
        public static bool IsResultFormulaCriteriaType(Type type)
        {
            return type == typeof(ResultsCriteria.Formula)
                || type == typeof(ResultsCriteria.DirectFormula)
                || type == typeof(ResultsCriteria.JChemFormula);
        }

        /// <summary>
        /// To check if type is molweight type or not.
        /// </summary>
        /// <param name="type">The type name.</param>
        /// <returns>
        /// Return true if it is cs, direct or jchem molweight type.
        /// </returns>
        public static bool IsResultMolWeightCriteriaType(Type type)
        {
            return type == typeof(ResultsCriteria.MolWeight)
                || type == typeof(ResultsCriteria.DirectMolWeight)
                || type == typeof(ResultsCriteria.JChemMolWeight);
        }

        /// <summary>
        /// Gets the result criteria type base on the search criteria type.
        /// </summary>
        /// <param name="searchCriteriaType">The type of search criteria.</param>
        /// <returns>
        /// Returns the type of result criteria type.
        /// </returns>
        public static Type MapResultCriteriaType(string searchCriteriaType)
        {
            var criteriaTypes = new Dictionary<string, Type>
            {
                {typeof(SearchCriteria.CSMolWeightCriteria).ToString(),typeof(ResultsCriteria.MolWeight)},
                {typeof(SearchCriteria.CSFormulaCriteria).ToString(),typeof(ResultsCriteria.Formula)},
                {typeof(SearchCriteria.DirectFormulaCriteria).ToString(),typeof(ResultsCriteria.DirectFormula)},
                {typeof(SearchCriteria.DirectMolWeightCriteria).ToString(),typeof(ResultsCriteria.DirectMolWeight)},
                {typeof(SearchCriteria.JChemFormulaCriteria).ToString(),typeof(ResultsCriteria.JChemFormula)},
                {typeof(SearchCriteria.JChemMolWeightCriteria).ToString(),typeof(ResultsCriteria.JChemMolWeight)}
            };

            if (criteriaTypes.ContainsKey(searchCriteriaType))
            {
                return criteriaTypes[searchCriteriaType];
            }

            return null;
        }

        /// <summary>
        /// Adds a Table to the collection.
        /// </summary>
        /// <param name="table">The table to add.</param>
        public void Add(ResultsCriteriaTable table)
        {
            this.tables.Add(table);
        }

        /// <summary>
        /// Gets all the table ids involved in the results criteria
        /// </summary>
        /// <returns>The table ids involved in the results criteria</returns>
        public List<int> GetTableIds()
        {
            List<int> result = new List<int>();
            foreach (ResultsCriteriaTable table in this.tables)
            {
                result.Add(table.Id);
            }
            return result;
        }

        public ResultsCriteria RemoveGrandChild(COEDataView orgCOEDataView, ResultsCriteria resultCriteria)
        {
            //CSBR-127380: Implementing the removal of grandchildren from resultant criteria
            int intBaseTableid = Convert.ToInt32(orgCOEDataView.Basetable);
            ResultsCriteria resCriteria = new ResultsCriteria();
            List<ResultsCriteria.ResultsCriteriaTable> resout = new List<ResultsCriteria.ResultsCriteriaTable>();
            //Loaded the resultant criteria
            resCriteria = resultCriteria;
            for (int i = 0; i < orgCOEDataView.Relationships.Count; i++)
            {
                //If parent id is not basetable then remove the child id from resultant criteria
                if (orgCOEDataView.Relationships[i].Parent != intBaseTableid)
                {
                    //removing tables from resultant criteria for which parent table is not base table.
                    resCriteria.Tables.RemoveAll(delegate(ResultsCriteria.ResultsCriteriaTable var) { return var.Id == orgCOEDataView.Relationships[i].Child; });
                }
            }
            return resCriteria;
        }

        public ResultsCriteriaTable GetResultTable(int id)
        {
            return this.tables.FirstOrDefault(table => table.Id == id);
        }

        /// <summary>
        /// Get full result criteria which contains all related data table
        /// </summary>
        /// <param name="dataView">The data view of current result criteria</param>
        /// <returns>Full result criteria</returns>
        public ResultsCriteria GetFullResultsCriteria(COEDataView dataView)
        {
            if (fullResultsCriteria == null)
            {
                fullResultsCriteria = ResultsCriteriaHelper.GenerateFullResultsCriteria(dataView, this);
            }

            return fullResultsCriteria;
        }

        #endregion

        #region Additional Classes
        /// Table
        /// <summary>
        /// A table to be get in a search.
        /// </summary>
        [Serializable]
        [XmlType("resultsCriteria.Table")]
        public class ResultsCriteriaTable
        {

            #region Properties
            /// <summary>
            /// An unique identifier of the table.
            /// </summary>
            [XmlAttribute("id")]
            public int Id
            {
                get { return id; }
                set { id = value; }
            }

            /// <summary>
            /// The list of returning operators for this table.
            /// </summary>
            [XmlElement(typeof(ResultsCriteria.Literal))]
            [XmlElement(typeof(ResultsCriteria.Field))]
            [XmlElement(typeof(ResultsCriteria.Switch))]
            [XmlElement(typeof(ResultsCriteria.Concatenation))]
            [XmlElement(typeof(ResultsCriteria.Condition))]
            [XmlElement(typeof(ResultsCriteria.MolWeight))]
            [XmlElement(typeof(ResultsCriteria.DirectMolWeight))]
            [XmlElement(typeof(ResultsCriteria.JChemMolWeight))]
            [XmlElement(typeof(ResultsCriteria.Formula))]
            [XmlElement(typeof(ResultsCriteria.DirectFormula))]
            [XmlElement(typeof(ResultsCriteria.JChemFormula))]
            [XmlElement(typeof(ResultsCriteria.Similarity))]
            [XmlElement(typeof(ResultsCriteria.HighlightedStructure))]
            [XmlElement(typeof(ResultsCriteria.Screen))]
            [XmlElement(typeof(ResultsCriteria.SQLFunction))]
            [XmlElement(typeof(ResultsCriteria.LowerCase))]
            [XmlElement(typeof(ResultsCriteria.ToDate))]
            [XmlElement(typeof(ResultsCriteria.CustomCriteria))]
            [XmlElement(typeof(ResultsCriteria.CDXToMolFile))]
            [XmlElement(typeof(ResultsCriteria.Marked))]
            [XmlElement(typeof(ResultsCriteria.Avg))]
            [XmlElement(typeof(ResultsCriteria.Max))]
            [XmlElement(typeof(ResultsCriteria.Min))]
            [XmlElement(typeof(ResultsCriteria.AggregateFunction))]
            [XmlElement(typeof(ResultsCriteria.DirectStructure))]
            [XmlElement(typeof(ResultsCriteria.RowId))]
            public List<IResultsCriteriaBase> Criterias
            {
                get { return criterias; }
                set { criterias = value; }
            }
            #endregion

            #region Variables
            private int id;
            private List<IResultsCriteriaBase> criterias;
            #endregion

            #region Constructors
            public ResultsCriteriaTable(int tableId)
                : this()
            {
                this.id = tableId;
            }

            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public ResultsCriteriaTable()
            {
                this.id = -1;
                this.criterias = new List<IResultsCriteriaBase>();
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation of the table.</param>
            public ResultsCriteriaTable(XmlNode node)
            {
                if (node.Attributes["id"] != null && node.Attributes["id"].Value != string.Empty)
                {
                    id = int.Parse(node.Attributes["id"].Value);
                }
                criterias = new List<IResultsCriteriaBase>();
                foreach (XmlNode criteriaNode in node.ChildNodes)
                {
                    if (criteriaNode.NodeType != null && criteriaNode.NodeType == XmlNodeType.Element)
                    {
                        IResultsCriteriaBase criteria = ResultsCriteriaBuilder.BuildCriteria(criteriaNode);
                        criterias.Add(criteria);
                    }
                }
            }
            #endregion

            #region Indexers
            public ResultsCriteria.IResultsCriteriaBase this[string alias]
            {
                get
                {
                    foreach (ResultsCriteria.IResultsCriteriaBase currentCriteria in this.Criterias)
                        if (currentCriteria.Alias.Equals(alias))
                            return currentCriteria;

                    return null;
                }
                set
                {
                    for (int index = 0; index < Criterias.Count; index++)
                        if (this.Criterias[index].Alias.Equals(alias))
                            this.Criterias[index] = value;
                }
            }
            #endregion

            #region Methods
            /// <summary>
            /// Append result criteria field
            /// </summary>
            /// <param name="fieldId">the field id</param>
            /// <param name="fieldAlias">the field alias</param>
            public void AppendFieldCriteria(int fieldId, string fieldAlias)
            {
                if (!this.Criterias.Exists(c => ((c is Field) && ((Field)c).Id == fieldId)))
                {
                    Field field = new Field { Id = fieldId, Alias = fieldAlias };
                    this.Criterias.Add(field);
                }
            }
            #endregion
        }

        /// IResultsCriteriaBase
        /// <summary>
        /// Base abstract class for each result criterium.
        /// </summary>
        [Serializable]
        public abstract class IResultsCriteriaBase
        {
            #region Methods
            public IResultsCriteriaBase()
            { }

            public IResultsCriteriaBase(XmlNode node)
            {
                if (node.Attributes["visible"] != null && !string.IsNullOrEmpty(node.Attributes["visible"].Value))
                    this.visible = bool.Parse(node.Attributes["visible"].Value);

                if (node.Attributes["alias"] != null && node.Attributes["alias"].Value != string.Empty)
                    alias = node.Attributes["alias"].Value;

                if (node.Attributes["orderById"] != null && node.Attributes["orderById"].Value != string.Empty)
                    orderById = int.Parse(node.Attributes["orderById"].Value);

                if (node.Attributes["direction"] != null && node.Attributes["direction"].Value != string.Empty)
                    direction = COEConvert.ToSortDirection(node.Attributes["direction"].Value);
            }

            public override string ToString()
            {
                return Utilities.XmlSerialize(this);
            }
            #endregion

            #region Variables
            protected bool visible = true;
            protected string alias;
            protected int orderById;
            protected SortDirection direction;
            #endregion

            #region Properties
            [XmlAttribute("visible")]
            public bool Visible
            {
                get { return this.visible; }
                set { this.visible = value; }
            }

            /// <summary>
            /// The desired alias of the returning field.
            /// </summary>
            [XmlAttribute("alias")]
            public string Alias
            {
                get { return alias; }
                set { alias = value; }
            }

            /// <summary>
            /// If ordering by this field is wanted, here it must me set which is its importance between other ordered criteria.
            /// </summary>
            [XmlAttribute("orderById")]
            public int OrderById
            {
                get { return orderById; }
                set { orderById = value; }
            }

            /// <summary>
            /// If ordering by this field is wanted, you may set if is ascending or descending.
            /// </summary>
            [XmlAttribute("direction")]
            public SortDirection Direction
            {
                get { return direction; }
                set { direction = value; }
            }
            #endregion
        }

        /// Field
        /// <summary>
        /// Represents a single field in database.
        /// </summary>
        [Serializable]
        [XmlType("field")]
        public class Field : IResultsCriteriaBase
        {
            #region Properties
            /// <summary>
            /// The field id in dataview to be get.
            /// </summary>
            [XmlAttribute("fieldId")]
            public int Id
            {
                get { return id; }
                set { id = value; }
            }
            #endregion

            #region Variables
            private int id;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public Field()
            {
                this.id = -1;
            }

            /// <summary>
            /// Accepts the fieldId as parameter
            /// </summary>
            /// <param name="id">A fieldId that points to a field in a dataview</param>
            public Field(int fieldId)
            {
                this.id = fieldId;
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public Field(XmlNode node)
                : base(node)
            {
                Debug.Assert(node.Attributes != null, "node.Attributes != null");
                if (node.Attributes["fieldId"] != null && node.Attributes["fieldId"].Value != string.Empty)
                    id = int.Parse(node.Attributes["fieldId"].Value);
            }
            #endregion
        }

        /// Formula
        /// <summary>
        /// Represents a ROWID field
        /// </summary>
        [Serializable]
        [XmlType("rowid")]
        public class RowId : Field
        {
            #region Constructors

            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public RowId(): base()
            {
                this.Id = 0;
                this.Alias = COEDataView.RowIdField.ReservedFieldAliasRowId;
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public RowId(XmlNode node)
                : base(node)
            {
            }

            #endregion
        }

        /// Literal
        /// <summary>
        /// Represents a returning literal.
        /// </summary>
        [Serializable]
        [XmlType("literal")]
        public class Literal : IResultsCriteriaBase
        {
            #region Properties
            /// <summary>
            /// The literal to be displayed.
            /// </summary>
            [XmlText()]
            public string LiteralValue
            {
                get { return literalValue; }
                set { literalValue = value; }
            }
            #endregion

            #region Variables
            private string literalValue;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public Literal()
            {
                this.literalValue = string.Empty;
            }

            public Literal(string literal)
            {
                this.literalValue = literal;
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public Literal(XmlNode node)
                : base(node)
            {
                this.literalValue = node.InnerXml;
            }
            #endregion
        }

        /// Concatenation
        /// <summary>
        /// Represents a concatenation of several ResultCriteriaItems.
        /// </summary>
        [Serializable]
        [XmlType("concatenation")]
        public class Concatenation : IResultsCriteriaBase
        {

            #region Properties
            /// <summary>
            /// The result criteria to concatenate.
            /// </summary>
            [XmlElement(typeof(ResultsCriteria.Literal))]
            [XmlElement(typeof(ResultsCriteria.Field))]
            [XmlElement(typeof(ResultsCriteria.Switch))]
            [XmlElement(typeof(ResultsCriteria.Concatenation))]
            [XmlElement(typeof(ResultsCriteria.Condition))]
            [XmlElement(typeof(ResultsCriteria.MolWeight))]
            [XmlElement(typeof(ResultsCriteria.DirectMolWeight))]
            [XmlElement(typeof(ResultsCriteria.JChemMolWeight))]
            [XmlElement(typeof(ResultsCriteria.Formula))]
            [XmlElement(typeof(ResultsCriteria.DirectFormula))]
            [XmlElement(typeof(ResultsCriteria.JChemFormula))]
            [XmlElement(typeof(ResultsCriteria.Similarity))]
            [XmlElement(typeof(ResultsCriteria.Screen))]
            [XmlElement(typeof(ResultsCriteria.CustomCriteria))]
            [XmlElement(typeof(ResultsCriteria.CDXToMolFile))]
            [XmlElement(typeof(ResultsCriteria.HighlightedStructure))]
            [XmlElement(typeof(ResultsCriteria.SQLFunction))]
            [XmlElement(typeof(ResultsCriteria.DirectStructure))]
            [XmlElement(typeof(ResultsCriteria.RowId))]
            public List<IResultsCriteriaBase> Operands
            {
                get { return operands; }
                set { operands = value; }
            }

            #endregion

            #region Variables
            private List<IResultsCriteriaBase> operands;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public Concatenation()
            {
                this.operands = new List<IResultsCriteriaBase>();
            }

            /// <summary>
            /// Constructor that instanciates a new concatenation criteria based upon it's xml representation.
            /// </summary>
            /// <param name="node">The xml representation of the concatenation.</param>
            public Concatenation(XmlNode node)
                : base(node)
            {
                this.operands = new List<IResultsCriteriaBase>();

                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (node.NodeType != null && node.NodeType == XmlNodeType.Element)
                        operands.Add(ResultsCriteriaBuilder.BuildCriteria(childNode));
                }
            }
            #endregion
        }

        /// Condition
        /// <summary>
        /// Represents the condition for a switch criterium.
        /// </summary>
        [Serializable]
        [XmlType("condition")]
        public class Condition : IResultsCriteriaBase
        {

            #region Properties
            /// <summary>
            /// The value to compare to, that determines if this case applies. If the clause equals this value, then the corresponding conditional is used.
            /// </summary>
            [XmlAttribute("value")]
            public string Value
            {
                get { return this.value; }
                set { this.value = value; }
            }

            /// <summary>
            /// The Result criteria used if the corresponding value matches.
            /// </summary>
            [XmlElement(typeof(ResultsCriteria.Literal))]
            [XmlElement(typeof(ResultsCriteria.Field))]
            [XmlElement(typeof(ResultsCriteria.Switch))]
            [XmlElement(typeof(ResultsCriteria.Concatenation))]
            [XmlElement(typeof(ResultsCriteria.Condition))]
            [XmlElement(typeof(ResultsCriteria.MolWeight))]
            [XmlElement(typeof(ResultsCriteria.DirectMolWeight))]
            [XmlElement(typeof(ResultsCriteria.JChemMolWeight))]
            [XmlElement(typeof(ResultsCriteria.Formula))]
            [XmlElement(typeof(ResultsCriteria.DirectFormula))]
            [XmlElement(typeof(ResultsCriteria.JChemFormula))]
            [XmlElement(typeof(ResultsCriteria.Similarity))]
            [XmlElement(typeof(ResultsCriteria.Screen))]
            [XmlElement(typeof(ResultsCriteria.CustomCriteria))]
            [XmlElement(typeof(ResultsCriteria.CDXToMolFile))]
            [XmlElement(typeof(ResultsCriteria.HighlightedStructure))]
            [XmlElement(typeof(ResultsCriteria.SQLFunction))]
            [XmlElement(typeof(ResultsCriteria.DirectStructure))]
            [XmlElement(typeof(ResultsCriteria.RowId))]
            public IResultsCriteriaBase Conditional
            {
                get { return condition; }
                set { condition = value; }
            }

            /// <summary>
            /// Determines if this is the default case.
            /// </summary>
            [XmlAttribute("default")]
            public bool IsDefault
            {
                get { return this.isDefault; }
                set { this.isDefault = value; }
            }
            #endregion

            #region Variables
            private string value;
            private IResultsCriteriaBase condition;
            private bool isDefault;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public Condition()
            {
                this.value = string.Empty;
                this.isDefault = false;
                this.condition = new Literal();
            }

            public Condition(string value, IResultsCriteriaBase condition)
                : this()
            {
                this.value = value;
                this.Conditional = condition;
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public Condition(XmlNode node)
                : base(node)
            {
                if (node.Attributes["value"] != null && node.Attributes["value"].Value.Trim() != string.Empty)
                    this.value = node.Attributes["value"].Value;

                if (node.Attributes["default"] != null && node.Attributes["default"].Value.Trim() != string.Empty)
                    isDefault = bool.Parse(node.Attributes["default"].Value);

                foreach (XmlNode conditionNode in node.ChildNodes)
                {
                    if (conditionNode.NodeType == XmlNodeType.Element)
                    {
                        this.condition = ResultsCriteriaBuilder.BuildCriteria(conditionNode);
                        break;
                    }
                }
            }
            #endregion
        }

        /// Switch
        /// <summary>
        /// Represents a switch operator.
        /// </summary>
        [Serializable]
        [XmlType("switch")]
        public class Switch : IResultsCriteriaBase
        {

            #region Properties
            /// <summary>
            /// The clause used to determine which condition applies. Analog to switch(clause) in C# or C++.
            /// </summary>
            [XmlElement(typeof(ResultsCriteria.Literal))]
            [XmlElement(typeof(ResultsCriteria.Field))]
            [XmlElement(typeof(ResultsCriteria.Switch))]
            [XmlElement(typeof(ResultsCriteria.Concatenation))]
            [XmlElement(typeof(ResultsCriteria.Condition))]
            [XmlElement(typeof(ResultsCriteria.MolWeight))]
            [XmlElement(typeof(ResultsCriteria.DirectMolWeight))]
            [XmlElement(typeof(ResultsCriteria.JChemMolWeight))]
            [XmlElement(typeof(ResultsCriteria.Formula))]
            [XmlElement(typeof(ResultsCriteria.DirectFormula))]
            [XmlElement(typeof(ResultsCriteria.JChemFormula))]
            [XmlElement(typeof(ResultsCriteria.Similarity))]
            [XmlElement(typeof(ResultsCriteria.Screen))]
            [XmlElement(typeof(ResultsCriteria.HighlightedStructure))]
            [XmlElement(typeof(ResultsCriteria.CustomCriteria))]
            [XmlElement(typeof(ResultsCriteria.CDXToMolFile))]
            [XmlElement(typeof(ResultsCriteria.SQLFunction))]
            [XmlElement(typeof(ResultsCriteria.DirectStructure))]
            [XmlElement(typeof(ResultsCriteria.RowId))]
            public IResultsCriteriaBase Clause
            {
                get { return clause; }
                set { clause = value; }
            }

            /// <summary>
            /// The list of ResultsCriteria.Condition used for representing "case" (as in C# or C++) statements.
            /// </summary>
            [XmlArray("conditions")]
            public List<Condition> Conditions
            {
                get { return conditions; }
                set { conditions = value; }
            }

            /// <summary>
            /// The input type.
            /// </summary>
            [XmlAttribute("inputType")]
            public string InputType
            {
                get { return inputType; }
                set { inputType = value; }
            }
            #endregion

            #region Variables
            private IResultsCriteriaBase clause;
            private List<Condition> conditions;
            private string inputType;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public Switch()
            {
                this.clause = new Literal();
                this.conditions = new List<Condition>();
                this.inputType = "text";
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public Switch(XmlNode node)
                : base(node)
            {
                this.clause = new Literal();
                this.conditions = new List<Condition>();
                this.inputType = "text";

                if (node.Attributes["inputType"] != null && node.Attributes["inputType"].Value != string.Empty)
                    inputType = node.Attributes["inputType"].Value;

                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.NodeType == XmlNodeType.Element)
                    {
                        if (childNode.Name.ToLower() == "conditions")
                        {
                            foreach (XmlNode currentCondition in childNode.ChildNodes)
                                this.conditions.Add(new Condition(childNode));
                        }
                        else
                        {
                            //foreach (XmlNode currentCondition in childNode.ChildNodes)
                            //{
                            //    if (currentCondition.NodeType != XmlNodeType.Comment && this.clause != null)
                            //        this.conditions.Add(new Condition(childNode));
                            //}
                            this.clause = ResultsCriteria.ResultsCriteriaBuilder.BuildCriteria(childNode);
                        }



                    }
                }
            }
            #endregion
        }

        /// MolWeight
        /// <summary>
        /// Represents a mol weight returning operator of CSCartridge.
        /// </summary>
        [Serializable]
        [XmlType("MolWeight")]
        public class MolWeight : Field
        {
            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public MolWeight()
                : base()
            {
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public MolWeight(XmlNode node)
                : base(node)
            {
            }
            #endregion
        }

        /// DirectMolWeight
        /// <summary>
        /// Represents a mol weight returning operator of Direct Cartridge.
        /// </summary>
        [Serializable]
        [XmlType("DirectMolWeight")]
        public class DirectMolWeight : Field
        {
            #region Constructors

            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public DirectMolWeight()
                : base()
            {
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public DirectMolWeight(XmlNode node)
                : base(node)
            {
            }

            #endregion
        }

        /// JChemMolWeight
        /// <summary>
        /// Represents a mol weight returning operator of JChem Cartridge.
        /// </summary>
        [Serializable]
        [XmlType("JChemMolWeight")]
        public class JChemMolWeight : Field
        {
            #region Constructors

            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public JChemMolWeight()
                : base()
            {
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public JChemMolWeight(XmlNode node)
                : base(node)
            {
            }

            #endregion
        }

        /// Formula
        /// <summary>
        /// Represents a formula returning operator of CSCartridge.
        /// </summary>
        [Serializable]
        [XmlType("Formula")]
        public class Formula : Field
        {
            #region Properties

            /// <summary>
            /// If ordering by this formula is wanted, you may set if is ascending or descending.
            /// </summary>
            [XmlAttribute("sortable")]
            public bool Sortable
            {
                get { return sortable; }
                set { sortable = value; }
            }

            /// <summary>
            /// If ordering by this formula is wanted, you may set if is ascending or descending.
            /// </summary>
            [XmlAttribute("htmlFormatted")]
            public bool HTMLFormatted
            {
                get { return htmlFormatted; }
                set { htmlFormatted = value; }
            }
            #endregion

            #region Variables
            private bool sortable;
            private bool htmlFormatted;
            #endregion

            #region Constructors

            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public Formula()
                : base()
            {
                this.sortable = false;
                this.htmlFormatted = false;
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public Formula(XmlNode node)
                : base(node)
            {
                this.sortable = false;
                this.htmlFormatted = false;

                Debug.Assert(node.Attributes != null, "node.Attributes != null");
                if (node.Attributes["sortable"] != null && node.Attributes["sortable"].Value != string.Empty)
                    this.sortable = bool.Parse(node.Attributes["sortable"].Value);

                if (node.Attributes["htmlFormatted"] != null && node.Attributes["htmlFormatted"].Value != string.Empty)
                    this.htmlFormatted = bool.Parse(node.Attributes["htmlFormatted"].Value);
            }
            #endregion
        }

        /// DirectFormula
        /// <summary>
        /// Represents a formula returning operator of Direct Cartridge.
        /// </summary>
        [Serializable]
        [XmlType("DirectFormula")]
        public class DirectFormula : Field
        {
            #region Properties

            /// <summary>
            /// If ordering by this formula is wanted, you may set if is ascending or descending.
            /// </summary>
            [XmlAttribute("sortable")]
            public bool Sortable { get; set; }

            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public DirectFormula()
            {
                Id = -1;
                Sortable = false;
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public DirectFormula(XmlNode node)
                : base(node)
            {
                Sortable = false;

                Debug.Assert(node.Attributes != null, "node.Attributes != null");
                if (node.Attributes["sortable"] != null && node.Attributes["sortable"].Value != string.Empty)
                    Sortable = bool.Parse(node.Attributes["sortable"].Value);
            }

            #endregion
        }

        /// JChemFormula
        /// <summary>
        /// Represents a formula returning operator of JChem Cartridge.
        /// </summary>
        [Serializable]
        [XmlType("JChemFormula")]
        public class JChemFormula : Field
        {
            #region Properties

            /// <summary>
            /// If ordering by this formula is wanted, you may set if is ascending or descending.
            /// </summary>
            [XmlAttribute("sortable")]
            public bool Sortable { get; set; }

            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public JChemFormula()
                : base()
            {
                Sortable = false;
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public JChemFormula(XmlNode node)
                : base(node)
            {
                Sortable = false;

                Debug.Assert(node.Attributes != null, "node.Attributes != null");
                if (node.Attributes["sortable"] != null && node.Attributes["sortable"].Value != string.Empty)
                    Sortable = bool.Parse(node.Attributes["sortable"].Value);
            }

            #endregion
        }

        /// Screen
        /// <summary>
        /// Represents a Screen returning operator.
        /// </summary>
        [Serializable]
        [XmlType("Screen")]
        public class Screen : IResultsCriteriaBase
        {

            #region Properties
            /// <summary>
            /// The field id in dataview that contains the molecule structure.
            /// </summary>
            [XmlAttribute("fieldId")]
            public int Id
            {
                get { return id; }
                set { id = value; }
            }

            /// <summary>
            /// If fieldId is not provided this is the data to get the screen from.
            /// </summary>
            [XmlAttribute("structure")]
            public string Structure
            {
                get { return structure; }
                set { structure = value; }
            }

            /// <summary>
            /// The source. Allowable values are NORMAL, SIMILAR, FULLEXACT and SKELETAL. Default value is NORMAL.
            /// </summary>
            [XmlAttribute("source")]
            public string Source
            {
                get { return source; }
                set { source = value; }
            }

            /// <summary>
            /// The output form. Allowable values are LIST, BITSRING. Default value is BITSRING.
            /// </summary>
            [XmlAttribute("outputForm")]
            public string OutputForm
            {
                get { return outputForm; }
                set { outputForm = value; }
            }

            #endregion

            #region Variables
            private int id;
            private string structure;
            private string source;
            private string outputForm;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public Screen()
            {
                this.id = -1;
                this.structure = string.Empty;
                this.outputForm = "BITSRING";
                this.source = "NORMAL";
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public Screen(XmlNode node)
                : base(node)
            {
                this.id = -1;

                if (node.Attributes["fieldId"] != null && node.Attributes["fieldId"].Value != string.Empty)
                    id = int.Parse(node.Attributes["fieldId"].Value);

                if (node.Attributes["structure"] != null && node.Attributes["structure"].Value != string.Empty)
                    this.structure = node.Attributes["structure"].Value;

                if (node.Attributes["source"] != null && node.Attributes["source"].Value != string.Empty)
                    this.structure = node.Attributes["source"].Value;

                if (node.Attributes["outputForm"] != null && node.Attributes["outputForm"].Value != string.Empty)
                    this.structure = node.Attributes["outputForm"].Value;
            }
            #endregion
        }

        /// Screen
        /// <summary>
        /// Represents a Screen returning operator.
        /// </summary>
        [Serializable]
        [XmlType("Similarity")]
        public class Similarity : IResultsCriteriaBase
        {

            #region Properties
            /// <summary>
            /// The field id in dataview that contains the molecule structure.
            /// </summary>
            [XmlAttribute("fieldId")]
            public int Id
            {
                get { return id; }
                set { id = value; }
            }

            /// <summary>
            /// If fieldId is not provided this is the data to get the screen from.
            /// </summary>
            [XmlAttribute("screen")]
            public string Screen
            {
                get { return screen; }
                set { screen = value; }
            }

            /// <summary>
            /// The source. Allowable values are NORMAL, SIMILAR, FULLEXACT and SKELETAL. Default value is NORMAL.
            /// </summary>
            [XmlElement(Type = typeof(Screen))]
            public Screen ScreenResultCriteria
            {
                get { return screenResultCriteria; }
                set { screenResultCriteria = value; }
            }

            #endregion

            #region Variables
            private int id;
            private string screen;
            private Screen screenResultCriteria;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public Similarity()
            {
                this.id = -1;
                this.screen = string.Empty;
                this.screenResultCriteria = null;
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public Similarity(XmlNode node)
                : base(node)
            {
                this.id = -1;

                if (node.Attributes["fieldId"] != null && node.Attributes["fieldId"].Value != string.Empty)
                    id = int.Parse(node.Attributes["fieldId"].Value);

                if (node.FirstChild != null)
                    this.screenResultCriteria = new Screen(node.FirstChild);
                else
                {
                    if (node.Attributes["screen"] != null && node.Attributes["screen"].Value != string.Empty)
                        this.screen = node.Attributes["screen"].Value;
                }
            }
            #endregion
        }

        [Serializable]
        [XmlType("HighlightedStructure")]
        public class HighlightedStructure : Field
        {
            #region Properties

            [XmlAttribute("highlight")]
            public bool Highlight
            {
                get { return highlight; }
                set { highlight = value; }
            }

            [XmlAttribute("moleculeContainsClause")]
            public string MoleculeContainsClause
            {
                get { return moleculeContainsClause; }
                set { moleculeContainsClause = value; }
            }

            [XmlAttribute("moleculeContainsOptions")]
            public string MoleculeContainsOptions
            {
                get { return moleculeContainsOptions; }
                set { moleculeContainsOptions = value; }
            }

            #endregion

            #region Variables
            private bool highlight;
            private string moleculeContainsClause;
            private string moleculeContainsOptions;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public HighlightedStructure()
                : base()
            {
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            ///  <HighlightedStructure visible="true" alias="StructureH" orderByID="0" direction="asc" fieldId="205" />
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public HighlightedStructure(XmlNode node)
                : base(node)
            {
                Debug.Assert(node.Attributes != null, "node.Attributes != null");
                if (node.Attributes["highlight"] != null && node.Attributes["highlight"].Value != string.Empty)
                    highlight = bool.Parse(node.Attributes["highlight"].Value);

                if (node.Attributes["moleculeContainsClause"] != null && node.Attributes["moleculeContainsClause"].Value != string.Empty)
                    moleculeContainsClause = node.Attributes["moleculeContainsClause"].Value;

                if (node.Attributes["moleculeContainsOptions"] != null && node.Attributes["moleculeContainsOptions"].Value != string.Empty)
                    moleculeContainsOptions = node.Attributes["moleculeContainsOptions"].Value;
            }
            #endregion
        }

        /// DirectMolWeight
        /// <summary>
        /// Represents a molefile returning operator of Direct Cartridge.
        /// </summary>
        [Serializable]
        [XmlType("DirectStructure")]
        public class DirectStructure : Field
        {
            #region Properties

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public DirectStructure()
            {
                Id = -1;
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public DirectStructure(XmlNode node)
                : base(node)
            {
            }

            #endregion
        }

        /// Marked
        /// <summary>
        /// Represents a formula returning operator.
        /// </summary>
        [Serializable]
        [XmlType("marked")]
        public class Marked : IResultsCriteriaBase
        {

            #region Properties
            /// <summary>
            /// The field id in dataview that contains the molecule structure.
            /// </summary>
            [XmlAttribute("dataViewID")]
            public int Id
            {
                get { return dataviewID; }
                set { dataviewID = value; }
            }
            #endregion

            #region Variables
            private int dataviewID;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public Marked()
            {
                this.dataviewID = -1;
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public Marked(XmlNode node)
                : base(node)
            {
                this.dataviewID = -1;

                if (node.Attributes["dataviewID"] != null && node.Attributes["dataviewID"].Value != string.Empty)
                    dataviewID = int.Parse(node.Attributes["dataviewID"].Value);
            }
            #endregion
        }

        /// cdxToMolFile
        /// <summary>
        /// Represents a cdxToMolFile returning operator.
        /// </summary>
        [Serializable]
        [XmlType("cdxToMolFile")]
        public class CDXToMolFile : IResultsCriteriaBase
        {

            #region Properties
            /// <summary>
            /// The field id in dataview that contains the molecule structure.
            /// </summary>
            [XmlAttribute("fieldId")]
            public int Id
            {
                get { return id; }
                set { id = value; }
            }
            #endregion

            #region Variables
            private int id;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public CDXToMolFile()
            {
                this.id = -1;
            }

            public CDXToMolFile(int fieldID)
            {
                this.id = fieldID;
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public CDXToMolFile(XmlNode node)
                : base(node)
            {
                if (node.Attributes["fieldId"] != null && node.Attributes["fieldId"].Value != string.Empty)
                    id = int.Parse(node.Attributes["fieldId"].Value);
            }
            #endregion
        }

        ///Custom Criteria: 
        /// <summary>
        /// Represents a custom criteria
        /// </summary>
        [Serializable]
        [XmlType("Custom")]
        public class CustomCriteria : IResultsCriteriaBase, ISerializable
        {

            #region Properties
            /// <summary>
            /// Whichever xml that a third party add in recongnizes.
            /// </summary>
            [XmlAnyElement()]
            public XmlNode Criteria
            {
                get { return criteria; }
                set { criteria = value; }
            }
            #endregion

            #region Variables
            private XmlNode criteria;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public CustomCriteria()
            {
                criteria = null;
            }

            /// <summary>
            /// Initializes the Criteria with the outer xml of the node.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public CustomCriteria(XmlNode node)
                : base(node)
            {
                criteria = node.FirstChild;
            }
            #endregion

            #region ISerializable Members

            protected CustomCriteria(SerializationInfo info, StreamingContext ctx)
            {
                this.alias = info.GetString("alias");
                string xmlOuterXml = info.GetString("criteria");
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlOuterXml);
                this.criteria = doc.FirstChild;
                this.direction = (SortDirection)info.GetValue("direction", typeof(SortDirection));
                this.orderById = info.GetInt32("orderById");
                this.visible = info.GetBoolean("visible");
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("alias", this.Alias);
                info.AddValue("criteria", this.Criteria.OuterXml);
                info.AddValue("direction", this.Direction, typeof(SortDirection));
                info.AddValue("orderById", this.OrderById);
                info.AddValue("visible", this.Visible);
            }

            #endregion
        }

        ///Function: 
        /// <summary>
        /// Represents a function criteria
        /// </summary>
        [Serializable]
        [XmlType("SQLFunction")]
        public class SQLFunction : IResultsCriteriaBase
        {

            #region Properties
            /// <summary>
            /// The result criteria to concatenate.
            /// </summary>
            [XmlElement(typeof(ResultsCriteria.Literal))]
            [XmlElement(typeof(ResultsCriteria.Field))]
            [XmlElement(typeof(ResultsCriteria.Switch))]
            [XmlElement(typeof(ResultsCriteria.Concatenation))]
            [XmlElement(typeof(ResultsCriteria.Condition))]
            [XmlElement(typeof(ResultsCriteria.MolWeight))]
            [XmlElement(typeof(ResultsCriteria.DirectMolWeight))]
            [XmlElement(typeof(ResultsCriteria.JChemMolWeight))]
            [XmlElement(typeof(ResultsCriteria.Formula))]
            [XmlElement(typeof(ResultsCriteria.DirectFormula))]
            [XmlElement(typeof(ResultsCriteria.JChemFormula))]
            [XmlElement(typeof(ResultsCriteria.Similarity))]
            [XmlElement(typeof(ResultsCriteria.Screen))]
            [XmlElement(typeof(ResultsCriteria.CustomCriteria))]
            [XmlElement(typeof(ResultsCriteria.CDXToMolFile))]
            [XmlElement(typeof(ResultsCriteria.HighlightedStructure))]
            [XmlElement(typeof(ResultsCriteria.AggregateFunction))]
            [XmlElement(typeof(ResultsCriteria.SQLFunction))]
            [XmlElement(typeof(ResultsCriteria.DirectStructure))]
            [XmlElement(typeof(ResultsCriteria.RowId))]
            public List<IResultsCriteriaBase> Parameters
            {
                get { return parameters; }
                set { parameters = value; }
            }

            /// <summary>
            /// The name of the function.
            /// </summary>
            [XmlAttribute("functionName")]
            public string FunctionName
            {
                get { return functionName; }
                set { functionName = value; }
            }
            #endregion

            #region Variables
            private List<IResultsCriteriaBase> parameters;
            private string functionName;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public SQLFunction()
            {
                this.parameters = new List<IResultsCriteriaBase>();
            }

            /// <summary>
            /// Constructor that instanciates a new function criteria based upon it's xml representation.
            /// </summary>
            /// <param name="node">The xml representation of the function.</param>
            public SQLFunction(XmlNode node)
                : base(node)
            {
                this.parameters = new List<IResultsCriteriaBase>();

                if (node.Attributes["functionName"] != null && node.Attributes["functionName"].Value != string.Empty)
                    functionName = node.Attributes["functionName"].Value;

                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.NodeType == XmlNodeType.Element)
                    {
                        this.parameters.Add(ResultsCriteriaBuilder.BuildCriteria(childNode));
                    }
                }

            }
            #endregion
        }

        /// LowerCase function
        /// <summary>
        /// Represents a LowerCase function on a single field in database.
        /// </summary>
        [Serializable]
        [XmlType("lowerCase")]
        public class LowerCase : IResultsCriteriaBase
        {

            #region Properties
            /// <summary>
            /// The field id in dataview to be get.
            /// </summary>
            [XmlAttribute("fieldId")]
            public int Id
            {
                get { return id; }
                set { id = value; }
            }
            #endregion

            #region Variables
            private int id;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public LowerCase()
            {
                this.id = -1;
            }

            /// <summary>
            /// Accepts the fieldId as parameter
            /// </summary>
            /// <param name="id">A fieldId that points to a field in a dataview</param>
            public LowerCase(int fieldId)
            {
                this.id = fieldId;
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public LowerCase(XmlNode node)
                : base(node)
            {
                if (node.Attributes["fieldId"] != null && node.Attributes["fieldId"].Value != string.Empty)
                    id = int.Parse(node.Attributes["fieldId"].Value);
            }
            #endregion
        }

        /// LowerCase function
        /// <summary>
        /// Represents a LowerCase function on a single field in database.
        /// </summary>
        [Serializable]
        [XmlType("toDate")]
        public class ToDate : IResultsCriteriaBase
        {

            #region Properties
            /// <summary>
            /// The field id in dataview to be get.
            /// </summary>
            [XmlAttribute("fieldId")]
            public int Id
            {
                get { return id; }
                set { id = value; }
            }
            #endregion

            #region Variables
            private int id;
            private string dateMask;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public ToDate()
            {
                this.id = -1;
            }

            /// <summary>
            /// Accepts the fieldId as parameter
            /// </summary>
            /// <param name="id">A fieldId that points to a field in a dataview</param>
            public ToDate(int fieldId, string dateMask)
            {
                this.id = fieldId;
                this.dateMask = dateMask;
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public ToDate(XmlNode node)
                : base(node)
            {
                if (node.Attributes["fieldId"] != null && node.Attributes["fieldId"].Value != string.Empty)
                    id = int.Parse(node.Attributes["fieldId"].Value);

                if (node.Attributes["dateMask"] != null && node.Attributes["dateMask"].Value != string.Empty)
                    dateMask = node.Attributes["dateMask"].Value.Trim();
            }
            #endregion
        }


        ///AggregateFunction: 
        /// <summary>
        /// Represents a AggregateFunction criteria
        /// </summary>
        [Serializable]
        [XmlType("aggregateFunction")]
        public class AggregateFunction : IResultsCriteriaBase
        {

            #region Properties
            /// <summary>
            /// The result criteria that are parameters for the aggregate function.
            /// </summary>
            [XmlElement(typeof(ResultsCriteria.Literal))]
            [XmlElement(typeof(ResultsCriteria.Field))]
            [XmlElement(typeof(ResultsCriteria.Switch))]
            [XmlElement(typeof(ResultsCriteria.Concatenation))]
            [XmlElement(typeof(ResultsCriteria.Condition))]
            [XmlElement(typeof(ResultsCriteria.MolWeight))]
            [XmlElement(typeof(ResultsCriteria.DirectMolWeight))]
            [XmlElement(typeof(ResultsCriteria.JChemMolWeight))]
            [XmlElement(typeof(ResultsCriteria.Formula))]
            [XmlElement(typeof(ResultsCriteria.DirectFormula))]
            [XmlElement(typeof(ResultsCriteria.JChemFormula))]
            [XmlElement(typeof(ResultsCriteria.Similarity))]
            [XmlElement(typeof(ResultsCriteria.Screen))]
            [XmlElement(typeof(ResultsCriteria.CustomCriteria))]
            [XmlElement(typeof(ResultsCriteria.CDXToMolFile))]
            [XmlElement(typeof(ResultsCriteria.HighlightedStructure))]
            [XmlElement(typeof(ResultsCriteria.DirectStructure))]
            [XmlElement(typeof(ResultsCriteria.SQLFunction))]
            [XmlElement(typeof(ResultsCriteria.RowId))]
            public List<IResultsCriteriaBase> Parameters
            {
                get { return parameters; }
                set { parameters = value; }
            }

            /// <summary>
            /// The name of the function.
            /// </summary>
            [XmlAttribute("functionName")]
            public string FunctionName
            {
                get { return functionName; }
                set { functionName = value; }
            }
            #endregion

            #region Variables
            private List<IResultsCriteriaBase> parameters;
            private string functionName;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public AggregateFunction()
            {
                this.parameters = new List<IResultsCriteriaBase>();
            }

            /// <summary>
            /// Constructor that instanciates a new function criteria based upon it's xml representation.
            /// </summary>
            /// <param name="node">The xml representation of the function.</param>
            public AggregateFunction(XmlNode node)
                : base(node)
            {
                this.parameters = new List<IResultsCriteriaBase>();

                if (node.Attributes["name"] != null && node.Attributes["name"].Value != string.Empty)
                    alias = node.Attributes["name"].Value;

                if (node.Attributes["functionName"] != null && node.Attributes["functionName"].Value != string.Empty)
                    functionName = node.Attributes["functionName"].Value;

                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.NodeType == XmlNodeType.Element)
                    {
                        this.parameters.Add(ResultsCriteriaBuilder.BuildCriteria(childNode));
                    }
                }

            }
            #endregion
        }

        /// Max aggregate function
        /// <summary>
        /// Represents a Max aggregate function on a single field in database.
        /// </summary>
        [Serializable]
        [XmlType("max")]
        public class Max : IResultsCriteriaBase
        {

            #region Properties
            /// <summary>
            /// The field id in dataview to be get.
            /// </summary>
            [XmlAttribute("fieldId")]
            public int Id
            {
                get { return id; }
                set { id = value; }
            }
            #endregion

            #region Variables
            private int id;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public Max()
            {
                this.id = -1;
            }

            /// <summary>
            /// Accepts the fieldId as parameter
            /// </summary>
            /// <param name="id">A fieldId that points to a field in a dataview</param>
            public Max(int fieldId)
            {
                this.id = fieldId;
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public Max(XmlNode node)
                : base(node)
            {
                if (node.Attributes["fieldId"] != null && node.Attributes["fieldId"].Value != string.Empty)
                    id = int.Parse(node.Attributes["fieldId"].Value);
            }
            #endregion
        }


        /// Min aggregate function
        /// <summary>
        /// Represents a Min aggregate function on a single field in database.
        /// </summary>
        [Serializable]
        [XmlType("min")]
        public class Min : IResultsCriteriaBase
        {

            #region Properties
            /// <summary>
            /// The field id in dataview to be get.
            /// </summary>
            [XmlAttribute("fieldId")]
            public int Id
            {
                get { return id; }
                set { id = value; }
            }
            #endregion

            #region Variables
            private int id;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public Min()
            {
                this.id = -1;
            }

            /// <summary>
            /// Accepts the fieldId as parameter
            /// </summary>
            /// <param name="id">A fieldId that points to a field in a dataview</param>
            public Min(int fieldId)
            {
                this.id = fieldId;
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public Min(XmlNode node)
                : base(node)
            {
                if (node.Attributes["fieldId"] != null && node.Attributes["fieldId"].Value != string.Empty)
                    id = int.Parse(node.Attributes["fieldId"].Value);
            }
            #endregion
        }


        /// Avg aggregate function
        /// <summary>
        /// Represents a Avg aggregate function on a single field in database.
        /// </summary>
        [Serializable]
        [XmlType("avg")]
        public class Avg : IResultsCriteriaBase
        {

            #region Properties
            /// <summary>
            /// The field id in dataview to be get.
            /// </summary>
            [XmlAttribute("fieldId")]
            public int Id
            {
                get { return id; }
                set { id = value; }
            }
            #endregion

            #region Variables
            private int id;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public Avg()
            {
                this.id = -1;
            }

            /// <summary>
            /// Accepts the fieldId as parameter
            /// </summary>
            /// <param name="id">A fieldId that points to a field in a dataview</param>
            public Avg(int fieldId)
            {
                this.id = fieldId;
            }

            /// <summary>
            /// Initializes its members from its xml representation.
            /// </summary>
            /// <param name="node">The xml representation.</param>
            public Avg(XmlNode node)
                : base(node)
            {
                if (node.Attributes["fieldId"] != null && node.Attributes["fieldId"].Value != string.Empty)
                    id = int.Parse(node.Attributes["fieldId"].Value);
            }
            #endregion

        }

        #endregion

        #region Builder
        /// <summary>
        /// Builds a proper ResultCriteria item from an xml representation.
        /// </summary>
        internal class ResultsCriteriaBuilder
        {
            /// <summary>
            /// Builds a proper ResultCriteria item from an xml representation.
            /// </summary>
            /// <param name="criteriaNode">The xml node.</param>
            /// <returns>An IResultsCriteriaBase built with the proper subclass.</returns>
            public static IResultsCriteriaBase BuildCriteria(XmlNode criteriaNode)
            {
                IResultsCriteriaBase criteria = null;
                if (criteriaNode.NodeType == XmlNodeType.Element)
                {
                    string type = criteriaNode.Name.Trim();
                    switch (type.ToLower())
                    {
                        case "literal":
                            criteria = new Literal(criteriaNode);
                            break;
                        case "field":
                            criteria = new Field(criteriaNode);
                            break;
                        case "concatenation":
                            criteria = new Concatenation(criteriaNode);
                            break;
                        case "switch":
                            criteria = new Switch(criteriaNode);
                            break;
                        case "condition":
                            break;
                        case "molweight":
                            criteria = new MolWeight(criteriaNode);
                            break;
                        case "directmolweight":
                            criteria = new DirectMolWeight(criteriaNode);
                            break;
                        case "jchemmolweight":
                            criteria = new JChemMolWeight(criteriaNode);
                            break;
                        case "formula":
                            criteria = new Formula(criteriaNode);
                            break;
                        case "directformula":
                            criteria = new DirectFormula(criteriaNode);
                            break;
                        case "jchemformula":
                            criteria = new JChemFormula(criteriaNode);
                            break;
                        case "cdxtomolfile":
                            criteria = new CDXToMolFile(criteriaNode);
                            break;
                        case "sqlfunction":
                            criteria = new SQLFunction(criteriaNode);
                            break;
                        case "lowercase":
                            criteria = new LowerCase(criteriaNode);
                            break;
                        case "aggregatefunction":
                            criteria = new AggregateFunction(criteriaNode);
                            break;
                        case "max":
                            criteria = new Max(criteriaNode);
                            break;
                        case "min":
                            criteria = new Min(criteriaNode);
                            break;
                        case "avg":
                            criteria = new Avg(criteriaNode);
                            break;
                        case "highlightedstructure":
                            criteria = new HighlightedStructure(criteriaNode);
                            break;
                        case "directstructure":
                            criteria = new DirectStructure(criteriaNode);
                            break;
                        default:
                            criteria = new CustomCriteria(criteriaNode);
                            break;
                    }
                }
                return criteria;
            }
        }
        #endregion

        #region Enums
        /// <summary>
        /// Represents the ways of sorting.
        /// </summary>
        public enum SortDirection
        {
            /// <summary>
            /// Ascending sorting.
            /// </summary>
            [XmlEnum("asc")]
            ASC,
            /// <summary>
            /// Descending sorting.
            /// </summary>
            [XmlEnum("desc")]
            DESC
        }
        #endregion

    }

    public class ResultsCriteriaTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType.Equals(typeof(string)))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
                return ResultsCriteria.GetResultsCriteria((string)value);

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType.Equals(typeof(string)))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType.Equals(typeof(string)))
                return ((ResultsCriteria)value).ToString();

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}