using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;
using CambridgeSoft.COE.Framework.Common.Utility;
using CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// <para>
    /// Class that provides messaging capabilities to communicate some search criteria.
    /// There are a number of criterim suppoorted:
    /// </>
    /// <list type="bullet">
    ///     <item>
    ///         <strong>TextCriteria:</strong> For communicating filters using text operations.
    ///         <seealso cref="SearchCriteria.TextCriteria"/>
    ///     </item>
    ///     <item>
    ///         <strong>NumericalCriteria</strong> For communicating filters using numerical operations.
    ///         <seealso cref="SearchCriteria.NumericalCriteria"/>
    ///     </item>
    ///     <item>
    ///         <strong>DateCriteria</strong> For communicating date's restrictions.
    ///         <seealso cref="SearchCriteria.DateCriteria"/>
    ///     </item>
    ///     <item>
    ///         <strong>FormulaCriteria</strong> For using formulas as filters.
    ///         <seealso cref="SearchCriteria.FormulaCriteria"/>
    ///     </item>
    ///     <item>
    ///         <strong>StructureCriteria</strong> For communicating molecular weight filters.
    ///         <seealso cref="SearchCriteria.StructureCriteria"/>
    ///     </item>
    ///     <item>
    ///         <strong>StructureCriteria</strong> For communicating single structure filters.
    ///         <seealso cref="SearchCriteria.StructureCriteria"/>
    ///     </item>
    ///     <item>
    ///         <strong>StructureListCriteria</strong> For communication a list of structure criterias used to filter.
    ///         <seealso cref="SearchCriteria.StructureListCriteria"/>
    ///     </item>
    ///     <item>
    ///         <strong>VerbatimCriteria</strong> For adding sql directly into the filter.
    ///         <seealso cref="SearchCriteria.VerbatimCriteria"/>
    ///     </item>
    ///     <item>
    ///         <strong>CustomCriteria</strong> For third party filters. Documentation should be provided by itself.
    ///         <seealso cref="SearchCriteria.CustomCriteria"/>
    ///     </item>
    ///     <item>
    ///         <strong>FullTextCriteria</strong> For using full text operations.
    ///         <seealso cref="SearchCriteria.FullTextCriteria"/>
    ///     </item>
    /// </list>
    /// <para>
    /// Each type of criterium has its own support for operations and its own syntax. See each one documentation for further details.
    /// </para>
    /// </summary>
    [Serializable]
    [XmlRoot("searchCriteria", Namespace = "COE.SearchCriteria")]
    [XmlType(TypeName = "searchCriteria", IncludeInSchema = true, Namespace = "COE.SearchCriteria")]
    public class SearchCriteria : ICloneable
    {
        #region Properties
        /// <summary>
        /// The list of criterium. Can be <see cref="SearchCriteria.LogicalCriteria"/> or <see cref="SearchCriteria.SearcCriteriaItem"/>
        /// </summary>
        [XmlElement(typeof(SearchCriteria.LogicalCriteria))]
        [XmlElement(typeof(SearchCriteria.SearchCriteriaItem))]
        public List<SearchExpression> Items
        {
            get { return items; }
            set { items = value; }
        }

        /// <summary>
        /// Xml Namespace.
        /// </summary>
        [XmlIgnore]
        public string XmlNS
        {
            get { return xmlNS; }
            set { xmlNS = value; }
        }

        /// <summary>
        /// Unique identifier of the search criteria.
        /// </summary>
        [XmlElement("searchCriteriaID", ElementName = "searchCriteriaID")]
        public int SearchCriteriaID
        {
            get { return searchCriteriaID; }
            set { searchCriteriaID = value; }
        }

        /// <summary>
        /// Gets or sets a given criterium by its id.
        /// </summary>
        /// <param name="id">Criterium's id</param>
        /// <returns>The criterium or null</returns>
        public SearchCriteriaItem this[int id]
        {
            get
            {
                foreach (SearchExpression currentExpression in this.items)
                    if (currentExpression.GetSearchCriteriaItem(id) != null)
                        return currentExpression.GetSearchCriteriaItem(id);

                return null;
            }
            set
            {
                for (int i = 0; 0 < this.items.Count; i++)
                {
                    if (this.items[i].GetSearchCriteriaItem(id) != null)
                    {
                        this.items[i] = value;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a new criteria that holds all the criterias for a given table id.
        /// </summary>
        /// <param name="tableId">Table's id</param>
        /// <returns>The criteria that matched</returns>
        public SearchCriteria GetSubset(int tableId)
        {
            SearchCriteria crit = new SearchCriteria();

            foreach (SearchExpression currentExpression in this.items)
                if (currentExpression is SearchCriteriaItem && tableId == ((SearchCriteriaItem)currentExpression).TableId)
                    crit.Items.Add(currentExpression);

            return crit;
        }
        #endregion

        #region Variables
        private List<SearchExpression> items;
        private string xmlNS;
        [NonSerialized]
        private XmlNamespaceManager manager;
        private string xmlNamespace;
        private int searchCriteriaID;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public SearchCriteria()
        {
            this.items = new List<SearchExpression>();
            this.xmlNS = "COE.SearchCriteria";
            xmlNamespace = "COE";
            manager = new XmlNamespaceManager(new NameTable());
            manager.AddNamespace(xmlNamespace, this.xmlNS);
        }

        /// <summary>
        /// Initializes the messaging type from its xml representation.
        /// </summary>
        /// <param name="doc">Its xml representation.</param>
        public SearchCriteria(XmlDocument doc)
        {
            this.xmlNS = "COE.SearchCriteria";
            xmlNamespace = "COE";
            manager = new XmlNamespaceManager(doc.NameTable);
            manager.AddNamespace(xmlNamespace, this.xmlNS);
            this.GetFromXML(doc);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Loads itself from an xml representation of the object.
        /// </summary>
        /// <param name="xmlSearchCriteria">The xml representation.</param>
        public void GetFromXML(XmlDocument xmlSearchCriteria)
        {
            //Get Rid of XmlComments for once and for all
            XmlNodeList commentsList = xmlSearchCriteria.SelectNodes("//comment()", this.manager);
            foreach (XmlNode currentComment in commentsList)
            {
                currentComment.ParentNode.RemoveChild(currentComment);
            }

            XmlNode searchCriteriaNode = xmlSearchCriteria.SelectSingleNode("//" + xmlNamespace + ":searchCriteria", this.manager);

            if (searchCriteriaNode != null)
            {
                XmlNodeList expressionsList = searchCriteriaNode.ChildNodes;
                this.items = new List<SearchExpression>();

                foreach (XmlNode expressionNode in expressionsList)
                {
                    if (expressionNode.NodeType == XmlNodeType.Element)
                    {
                        switch (expressionNode.Name.ToLower())
                        {
                            case "groupcriteria":
                            case "logicalcriteria":
                                this.items.Add(new LogicalCriteria(expressionNode));
                                break;
                            case "searchcriteriaitem":
                                this.items.Add(new SearchCriteriaItem(expressionNode));
                                break;
                            case "searchcriteriaid":
                                break;
                            default:
                                throw new Exception("Unsupported Expression: " + expressionNode.Name);
                                break;
                        }
                    }
                }

                if (searchCriteriaNode.Attributes["xmlns"] != null && searchCriteriaNode.Attributes["xmlns"].Value != string.Empty)
                    this.xmlNS = searchCriteriaNode.Attributes["xmlns"].Value;
            }
            else
                throw new ArgumentException("searchCriteria xmlns=\"COE.SearchCriteria\" parent node  required");
        }

        /// <summary>
        /// Fills the search criteria object from it's xml representation contained in a string.
        /// </summary>
        /// <param name="xmlSearchCriteria">The string containing the search criteria xml representation</param>
        public void GetFromXML(string xmlSearchCriteria)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlSearchCriteria);
            this.GetFromXML(doc);
        }

        /// <summary>
        /// Find the first structure type search criteria
        /// </summary>
        /// <returns>structure criteria</returns>
        public ISearchCriteriaBase FindStructCriteria()
        {
            foreach (SearchExpression se in this.Items)
            {
                SearchCriteriaItem sci = se as SearchCriteriaItem;

                if (sci != null && sci.Criterium != null)
                {
                    if (sci.Criterium is StructureCriteria)
                    {
                        return sci.Criterium as StructureCriteria;
                    }

                    if (sci.Criterium is JChemStructureCriteria)
                    {
                        return sci.Criterium as JChemStructureCriteria;
                    }

                    if (sci.Criterium is DirectFlexmatchCriteria)
                    {
                        return sci.Criterium as DirectFlexmatchCriteria;
                    }

                    if (sci.Criterium is DirectSssCriteria)
                    {
                        return sci.Criterium as DirectSssCriteria;
                    }

                    if (sci.Criterium is DirectSimilarCriteria)
                    {
                        return sci.Criterium as DirectSimilarCriteria;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the formula search criteria type by index type.
        /// </summary>
        /// <param name="indexType">The COEDataView index type.</param>
        /// <returns>
        /// Return the type name of the formula search criteria.
        /// </returns>
        public static Type GetFormulaCriteriaByIndexType(COEDataView.IndexTypes indexType)
        {
            Type formulaCriteriaType;

            switch (indexType)
            {
                case COEDataView.IndexTypes.CS_CARTRIDGE:
                    formulaCriteriaType = typeof(SearchCriteria.CSFormulaCriteria);
                    break;
                case COEDataView.IndexTypes.DIRECT_CARTRIDGE:
                    formulaCriteriaType = typeof(SearchCriteria.DirectFormulaCriteria);
                    break;
                case COEDataView.IndexTypes.JCHEM_CARTRIDGE:
                    formulaCriteriaType = typeof(SearchCriteria.JChemFormulaCriteria);
                    break;
                default:
                    throw new NotSupportedException(string.Format("Does not support IndexType of '{0}'", indexType));
            }

            return formulaCriteriaType;
        }

        /// <summary>
        /// To Check if the type is search formula criteria type or not.
        /// </summary>
        /// <param name="type">The criteria type</param>
        /// <returns>
        /// Return true if it is cs, direct, jchem cartridge formula type.
        /// </returns>
        public static bool IsSearchFormulaCriteriaType(Type type)
        {
            return type == typeof(SearchCriteria.CSFormulaCriteria)
                || type == typeof(SearchCriteria.DirectFormulaCriteria)
                || type == typeof(SearchCriteria.JChemFormulaCriteria);
        }

        /// <summary>
        /// To check if the type is search molweight type or not.
        /// </summary>
        /// <param name="type">The criteria type.</param>
        /// <returns>
        /// Return true if it is cs, direct, jchem cartridge molweight type.
        /// </returns>
        public static bool IsSearchMolWeightCriteriaType(Type type)
        {
            return type == typeof(SearchCriteria.CSMolWeightCriteria)
                || type == typeof(SearchCriteria.DirectMolWeightCriteria)
                || type == typeof(SearchCriteria.JChemMolWeightCriteria);
        }

        /// <summary>
        /// Gets the molweight search criteria type by index type.
        /// </summary>
        /// <param name="indexType">The COEDataView index type.</param>
        /// <returns>
        /// Returns the molweight search criteria type.
        /// </returns>
        public static Type GetMolWeightCriteriaByIndexType(COEDataView.IndexTypes indexType)
        {
            Type molWeightCriteriaType;

            switch (indexType)
            {
                case COEDataView.IndexTypes.CS_CARTRIDGE:
                    molWeightCriteriaType = typeof(SearchCriteria.CSMolWeightCriteria);
                    break;
                case COEDataView.IndexTypes.DIRECT_CARTRIDGE:
                    molWeightCriteriaType = typeof(SearchCriteria.DirectMolWeightCriteria);
                    break;
                case COEDataView.IndexTypes.JCHEM_CARTRIDGE:
                    molWeightCriteriaType = typeof(SearchCriteria.JChemMolWeightCriteria);
                    break;
                default:
                    throw new NotSupportedException(string.Format("Does not support IndexType of '{0}'", indexType));
            }

            return molWeightCriteriaType;
        }

        /// <summary>
        /// Gets the search criteria base on the result criteria type name.
        /// </summary>
        /// <param name="resultCriteriaType">The result criteria type name.</param>
        /// <returns>
        /// Returns the search criteria type.
        /// </returns>
        public static Type MapSearchCriteria(string resultCriteriaType)
        {
            var criteriaTypes = new Dictionary<string, Type>
            {
                {typeof(ResultsCriteria.MolWeight).ToString(),typeof(SearchCriteria.CSMolWeightCriteria)},
                {typeof(ResultsCriteria.Formula).ToString(),typeof(SearchCriteria.CSFormulaCriteria)},
                {typeof(ResultsCriteria.DirectFormula).ToString(),typeof(SearchCriteria.DirectFormulaCriteria)},
                {typeof(ResultsCriteria.DirectMolWeight).ToString(),typeof(SearchCriteria.DirectMolWeightCriteria)},
                {typeof(ResultsCriteria.JChemFormula).ToString(),typeof(SearchCriteria.JChemFormulaCriteria)},
                {typeof(ResultsCriteria.JChemMolWeight).ToString(),typeof(SearchCriteria.JChemMolWeightCriteria)}
            };

            if (criteriaTypes.ContainsKey(resultCriteriaType))
            {
                return criteriaTypes[resultCriteriaType];
            }

            return null;
        }

        /// <summary>
        /// Builds its xml string representation.
        /// </summary>
        /// <returns>Its xml string representation.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            builder.Append("<searchCriteria  xmlns=\"" + xmlNS + "\">");
            for (int i = 0; i < items.Count; i++)
            {
                builder.Append(items[i].ToString());
            }
            builder.Append("</searchCriteria>");

            return builder.ToString();
        }
        #endregion

        #region Additional Classes
        /// <summary>
        /// SearchExpression is the base class for any item to be used as item in the SearchCriteria.Items list.
        /// Members that inherit from this one are SearchCriteriaItem and LogicalCriteria.
        /// The first one represent single criterium that would be concatenated with an AND clause with the rest of items.
        /// The second one allows to group several criterium and allows the joining operator to be overriden.
        /// </summary>
        [Serializable]
        public class SearchExpression
        {
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public SearchExpression()
            {
            }

            /// <summary>
            /// Gets a SearchCriteriaItem if possible. Must be overriden, it will return null otherwise.
            /// </summary>
            /// <param name="id">The search criteria item id.</param>
            /// <returns>A SearchCriteriaItem.</returns>
            public virtual SearchCriteriaItem GetSearchCriteriaItem(int id)
            {
                return null;
            }
        }

        /// <summary>
        /// A single search criterium.
        /// </summary>
        [Serializable]
        [XmlType("searchCriteriaItem")]
        public class SearchCriteriaItem : SearchExpression
        {
            #region Properties
            /// <summary>
            /// The field id that is going to be used in this criterium. This is related to the COEDataView type.
            /// </summary>
            [XmlAttribute("fieldid")]
            public int FieldId
            {
                get { return fieldId; }
                set { fieldId = value; }
            }

            /// <summary>
            /// Unique identifier of the criterium.
            /// </summary>
            [XmlAttribute("id")]
            public int ID
            {
                get { return Id; }
                set { Id = value; }
            }

            /// <summary>
            /// Modifier is currently unused.
            /// </summary>
            [XmlAttribute("modifier")]
            public string Modifier
            {
                get { return modifier; }
                set { modifier = value; }
            }

            /// <summary>
            /// The table id that is going to be used in this criterium. This is related to the COEDataView type.
            /// </summary>
            [XmlAttribute("tableid")]
            public int TableId
            {
                get { return tableId; }
                set { tableId = value; }
            }

            /// <summary>
            /// Indicates if lookup fields are going to be searched using the main table id or by value.
            /// </summary>
            [XmlAttribute("searchLookupByID")]
            public bool SearchLookupByID
            {
                get { return searchLookupByID; }
                set { searchLookupByID = value; }
            }

            /// <summary>
            /// The actual criterium to be applied to the choosen field. Allowed members are:
            /// <list type="bullet">
            /// <item><see cref="SearchCriteria.TextCriteria"/></item>
            /// <item><see cref="SearchCriteria.NumericalCriteria"/></item>
            /// <item><see cref="SearchCriteria.DateCriteria"/></item>
            /// <item><see cref="SearchCriteria.CSFormulaCriteria"/></item>
            /// <item><see cref="SearchCriteria.CSMolWeightCriteria"/></item>
            /// <item><see cref="SearchCriteria.StructureCriteria"/></item>
            /// <item><see cref="SearchCriteria.StructureListCriteria"/></item>
            /// <item><see cref="SearchCriteria.VerbatimCriteria"/></item>
            /// <item><see cref="SearchCriteria.CustomCriteria"/></item>
            /// <item><see cref="SearchCriteria.FullTextCriteria"/></item>
            /// </list>
            /// </summary>
            [XmlElement(typeof(SearchCriteria.TextCriteria))]
            [XmlElement(typeof(SearchCriteria.NumericalCriteria))]
            [XmlElement(typeof(SearchCriteria.DateCriteria))]
            [XmlElement(typeof(SearchCriteria.CSFormulaCriteria))]
            [XmlElement(typeof(SearchCriteria.DirectFormulaCriteria))]
            [XmlElement(typeof(SearchCriteria.JChemFormulaCriteria))]
            [XmlElement(typeof(SearchCriteria.CSMolWeightCriteria))]
            [XmlElement(typeof(SearchCriteria.DirectMolWeightCriteria))]
            [XmlElement(typeof(SearchCriteria.JChemMolWeightCriteria))]
            [XmlElement(typeof(SearchCriteria.StructureCriteria))]
            [XmlElement(typeof(SearchCriteria.DirectFlexmatchCriteria))]
            [XmlElement(typeof(SearchCriteria.DirectSssCriteria))]
            [XmlElement(typeof(SearchCriteria.DirectSimilarCriteria))]
            [XmlElement(typeof(SearchCriteria.JChemStructureCriteria))]
            [XmlElement(typeof(SearchCriteria.StructureListCriteria))]
            [XmlElement(typeof(SearchCriteria.VerbatimCriteria))]
            [XmlElement(typeof(SearchCriteria.CustomCriteria))]
            [XmlElement(typeof(SearchCriteria.FullTextCriteria))]
            [XmlElement(typeof(SearchCriteria.HitlistCriteria))]
            public ISearchCriteriaBase Criterium
            {
                get { return criterium; }
                set { criterium = value; }
            }

            [XmlAttribute("aggregateFunctionName")]
            public string AggregateFunctionName
            {
                get { return _aggregateFunctionName; }
                set { _aggregateFunctionName = value; }
            }
            #endregion

            #region Variables
            private int fieldId;
            private int Id;
            private string modifier;
            private int tableId;
            private ISearchCriteriaBase criterium;
            private bool searchLookupByID;
            private string _aggregateFunctionName;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members form its xml representation.
            /// </summary>
            public SearchCriteriaItem(XmlNode criteriaNode)
            {
                foreach (XmlNode criteriaChild in criteriaNode.ChildNodes)
                {
                    if (criteriaChild.NodeType == XmlNodeType.Element)
                    {
                        switch (criteriaChild.Name.ToLower())
                        {
                            case "textcriteria":
                                this.Criterium = new TextCriteria(criteriaChild);
                                break;
                            case "numericalcriteria":
                                this.Criterium = new NumericalCriteria(criteriaChild);
                                break;
                            case "datecriteria":
                                this.Criterium = new DateCriteria(criteriaChild);
                                break;
                            case "molweightcriteria":
                                this.Criterium = new CSMolWeightCriteria(criteriaChild);
                                break;
                            case "directmolweightcriteria":
                                this.Criterium = new DirectMolWeightCriteria(criteriaChild);
                                break;
                            case "jchemmolweightcriteria":
                                this.Criterium = new JChemMolWeightCriteria(criteriaChild);
                                break;
                            case "structurecriteria":
                                this.Criterium = new StructureCriteria(criteriaChild);
                                break;
                            case "directflexmatchcriteria":
                                this.Criterium = new DirectFlexmatchCriteria(criteriaChild);
                                break;
                            case "directssscriteria":
                                this.Criterium = new DirectSssCriteria(criteriaChild);
                                break;
                            case "directsimilarcriteria":
                                this.Criterium = new DirectSimilarCriteria(criteriaChild);
                                break;
                            case "jchemstructurecriteria":
                                this.Criterium = new JChemStructureCriteria(criteriaChild);
                                break;
                            case "structurelistcriteria":
                                this.Criterium = new StructureListCriteria(criteriaChild);
                                break;
                            case "formulacriteria":
                                this.Criterium = new CSFormulaCriteria(criteriaChild);
                                break;
                            case "directformulacriteria":
                                this.Criterium = new DirectFormulaCriteria(criteriaChild);
                                break;
                            case "jchemformulacriteria":
                                this.Criterium = new JChemFormulaCriteria(criteriaChild);
                                break;
                            case "verbatimcriteria":
                                this.Criterium = new VerbatimCriteria(criteriaChild);
                                break;
                            case "domaincriteria":
                                this.Criterium = new DomainCriteria(criteriaChild);
                                break;
                            case "customcriteria":
                                this.Criterium = new CustomCriteria(criteriaChild);
                                break;
                            case "fulltextcriteria":
                                this.Criterium = new FullTextCriteria(criteriaChild);
                                break;
                            case "hitlistcriteria":
                                this.Criterium = new HitlistCriteria(criteriaChild);
                                break;
                        }
                    }
                }

                // take the 4 attributes
                if (criteriaNode.Attributes["id"] != null && criteriaNode.Attributes["id"].Value != string.Empty)
                    this.ID = int.Parse(criteriaNode.Attributes["id"].Value);
                if (criteriaNode.Attributes["fieldid"] != null && criteriaNode.Attributes["fieldid"].Value != string.Empty)
                    this.FieldId = int.Parse(criteriaNode.Attributes["fieldid"].Value);
                if (criteriaNode.Attributes["tableid"] != null && criteriaNode.Attributes["tableid"].Value != string.Empty)
                    this.TableId = int.Parse(criteriaNode.Attributes["tableid"].Value);
                if (criteriaNode.Attributes["modifier"] != null && criteriaNode.Attributes["modifier"].Value != string.Empty)
                    this.Modifier = criteriaNode.Attributes["modifier"].Value;
                if (criteriaNode.Attributes["searchLookupByID"] != null && criteriaNode.Attributes["searchLookupByID"].Value != string.Empty)
                    this.SearchLookupByID = criteriaNode.Attributes["searchLookupByID"].Value.ToLower() != "false" && criteriaNode.Attributes["searchLookupByID"].Value.ToLower() != "0";
                else
                    this.searchLookupByID = true;
                if (criteriaNode.Attributes["aggregateFunctionName"] != null && criteriaNode.Attributes["aggregateFunctionName"].Value != string.Empty)
                    this.AggregateFunctionName = criteriaNode.Attributes["aggregateFunctionName"].Value;
            }

            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public SearchCriteriaItem()
            {
                this.searchLookupByID = true;
                _aggregateFunctionName = string.Empty;
            }
            #endregion

            #region Methods
            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder("<searchCriteriaItem");
                builder.Append(" id=\"");
                builder.Append(Id);
                builder.Append("\" fieldid=\"");
                builder.Append(fieldId);
                builder.Append("\" tableid=\"");
                builder.Append(tableId);
                builder.Append("\" modifier=\"");
                builder.Append(modifier);
                builder.Append("\" aggregateFunctionName=\"");
                builder.Append(_aggregateFunctionName);
                builder.Append("\" searchLookupByID=\"");
                builder.Append(searchLookupByID ? "1" : "0");
                builder.Append("\">");
                builder.Append(criterium.GenerateXmlSnippet());
                builder.Append("</searchCriteriaItem>");

                return builder.ToString();
            }

            /// <summary>
            /// If the id matches returns this object.
            /// </summary>
            /// <param name="id">Search criteria id</param>
            /// <returns>The SearchCriteriaItem</returns>
            public override SearchCriteriaItem GetSearchCriteriaItem(int id)
            {
                if (id == this.Id)
                    return this;

                return null;
            }

            /// <summary>
            /// Builds a search criteria type from an xml representation (as string).
            /// </summary>
            /// <param name="xml">The xml as string.</param>
            /// <returns>A SearchCriteriaItem.</returns>
            public static SearchCriteriaItem GetSearchCriteriaItem(string xml)
            {
                return Utilities.XmlDeserialize<SearchCriteriaItem>(xml);
            }

            /// <summary>
            /// Performs a member wise clone.
            /// </summary>
            /// <returns>An object's clone.</returns>
            public object Clone()
            {
                return this.MemberwiseClone();
            }

            #endregion
        }

        /// <summary>
        /// Allows the grouping of search expressions and overriding the grouping operator.
        /// </summary>
        [Serializable]
        [XmlType("logicalCriteria")]
        public class LogicalCriteria : SearchExpression
        {
            #region Properties
            /// <summary>
            /// The conditional operator. <see cref="COELogicalOperators"/>
            /// </summary>
            [XmlAttribute("operator")]
            public COELogicalOperators LogicalOperator
            {
                get { return logicalOperator; }
                set { logicalOperator = value; }
            }

            /// <summary>
            /// The list of criteria.
            /// </summary>
            [XmlElement(typeof(SearchCriteria.SearchCriteriaItem))]
            [XmlElement(typeof(SearchCriteria.LogicalCriteria))]
            public List<SearchExpression> Items
            {
                get { return expressions; }
                set { expressions = value; }
            }
            #endregion

            #region Variables
            private COELogicalOperators logicalOperator;
            private List<SearchExpression> expressions;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its values with the given information.
            /// </summary>
            /// <param name="expressionNode">The xml representation.</param>
            public LogicalCriteria(XmlNode expressionNode)
                : this()
            {
                foreach (XmlNode expressionChild in expressionNode.ChildNodes)
                {
                    if (expressionChild.NodeType == XmlNodeType.Element)
                    {
                        SearchExpression exp = new SearchExpression();
                        switch (expressionChild.Name.ToLower())
                        {
                            case "logicalcriteria":
                            case "groupcriteria":
                                exp = new LogicalCriteria(expressionChild);
                                break;
                            case "searchcriteriaitem":
                                exp = new SearchCriteriaItem(expressionChild);
                                break;
                            default:
                                throw new Exception("Unsupported Expression: " + expressionChild.Name);
                                break;
                        }
                        this.expressions.Add(exp);
                    }
                }

                if (expressionNode.Attributes["operator"] != null && expressionNode.Attributes["operator"].Value != string.Empty)
                    logicalOperator = COEConvert.ToCOELogicalOperators(expressionNode.Attributes["operator"].Value);
                else
                    logicalOperator = COELogicalOperators.And;
            }

            /// <summary>
            /// Initializes its values to its default values.
            /// Default logical operator is AND.
            /// </summary>
            public LogicalCriteria()
            {
                logicalOperator = COELogicalOperators.And;
                this.expressions = new List<SearchExpression>();
            }
            #endregion

            #region Methods
            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder("<logicalCriteria ");
                builder.Append(@"operator=""");
                builder.Append(logicalOperator.ToString());
                builder.Append(@""" >");
                foreach (SearchExpression exp in expressions)
                {
                    builder.Append(exp.ToString());
                }
                builder.Append("</logicalCriteria>");
                return builder.ToString();
            }

            /// <summary>
            /// Finds the given search criteria item in its members.
            /// </summary>
            /// <param name="id">The search criteria item id.</param>
            /// <returns>The search criteria item if found, null otherwise.</returns>
            public override SearchCriteriaItem GetSearchCriteriaItem(int id)
            {
                foreach (SearchExpression exp in this.expressions)
                {
                    if (exp.GetSearchCriteriaItem(id) != null)
                        return exp.GetSearchCriteriaItem(id);
                }

                return null;
            }
            #endregion
        }

        /// <summary>
        /// Base type for an specific criterium.
        /// </summary>
        [Serializable]
        public abstract class ISearchCriteriaBase
        {
            #region Properties
            /// <summary>
            /// The Not operator.
            /// </summary>
            [XmlAttribute("negate")]
            public COEBoolean Negate
            {
                get { return negate; }
                set { negate = value; }
            }

            [XmlIgnore()]
            public abstract string Value
            {
                get;
                set;
            }
            #endregion

            #region Variables
            private COEBoolean negate;
            #endregion

            /// <summary>
            /// Method intended to build its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation</returns>
            public abstract string GenerateXmlSnippet();
        }

        /// <summary>
        /// Class for querying in text mode.
        /// </summary>
        [Serializable]
        [XmlType("textCriteria")]
        public class TextCriteria : ISearchCriteriaBase
        {

            #region Properties
            /// <summary>
            /// Gets or sets if the query must normalize a chemical name.
            /// </summary>
            [XmlAttribute("normalizedChemicalName")]
            public COEBoolean NormalizedChemicalName
            {
                get { return normalizedChemicalName; }
                set { normalizedChemicalName = value; }
            }

            /// <summary>
            /// Gets or sets if this search is about a hill formula.
            /// </summary>
            [XmlAttribute("hillFormula")]
            public COEBoolean HillFormula
            {
                get { return hillFormula; }
                set { hillFormula = value; }
            }

            /// <summary>
            /// Gets or sets if only full word search matches must be returned.
            /// </summary>
            [XmlAttribute("fullWordSearch")]
            public COEBoolean FullWordSearch
            {
                get { return fullWordSearch; }
                set { fullWordSearch = value; }
            }

            /// <summary>
            /// Gets or sets if the query must be case sensitive.
            /// </summary>
            [XmlAttribute("caseSensitive")]
            public COEBoolean CaseSensitive
            {
                get { return caseSensitive; }
                set { caseSensitive = value; }
            }

            /// <summary>
            /// Determines where trim must be applied.
            /// </summary>
            [XmlAttribute("trim")]
            public SearchCriteria.Positions Trim
            {
                get { return trimPosition; }
                set { trimPosition = value; }
            }

            /// <summary>
            /// Determines which is the operator of the search.
            /// </summary>
            [XmlAttribute("operator")]
            public COEOperators Operator
            {
                get { return operation; }
                set { operation = value; }
            }

            /// <summary>
            /// Determines which is the operator of the search.
            /// </summary>
            [XmlAttribute("hint")]
            public string Hint
            {
                get { return _hint; }
                set { _hint = value; }
            }

            /// <summary>
            /// The value to compare to. 
            /// </summary>
            [XmlText()]
            public string InnerText
            {
                get { return System.Web.HttpUtility.HtmlDecode(innerText); }
                set { innerText = System.Web.HttpUtility.HtmlEncode(value); }
            }

            /// <summary>
            /// Gets or sets the <see cref="InnerText"/>. Used for standarization between criteria.
            /// </summary>
            [XmlIgnore()]
            public override string Value
            {
                get
                {
                    return InnerText;
                }
                set
                {
                    InnerText = value;
                }
            }

            /// <summary>
            /// Determines which is the operator of the search.
            /// </summary>
            [XmlAttribute("defaultWildCardPosition")]
            public Positions DefaultWildCardPosition
            {
                get { return defaultWildCardPosition; }
                set { defaultWildCardPosition = value; }
            }
            #endregion

            #region Variables
            private string innerText;
            private COEOperators operation;
            private Positions trimPosition;
            private Positions defaultWildCardPosition;
            private COEBoolean caseSensitive;
            private COEBoolean fullWordSearch;
            private COEBoolean hillFormula;
            private COEBoolean normalizedChemicalName;
            private string _hint;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes the text criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public TextCriteria(XmlNode node)
            {
                if (node.Attributes["operator"] != null && node.Attributes["operator"].Value != string.Empty)
                    operation = COEConvert.ToCOEOperator(node.Attributes["operator"].Value);
                if (node.Attributes["trim"] != null && node.Attributes["trim"].Value != string.Empty)
                    trimPosition = COEConvert.ToPositions(node.Attributes["trim"].Value);
                if (node.Attributes["caseSensitive"] != null && node.Attributes["caseSensitive"].Value != string.Empty)
                    caseSensitive = COEConvert.ToCOEBoolean(node.Attributes["caseSensitive"].Value);
                if (node.Attributes["fullWordSearch"] != null && node.Attributes["fullWordSearch"].Value != string.Empty)
                    fullWordSearch = COEConvert.ToCOEBoolean(node.Attributes["fullWordSearch"].Value);
                if (node.Attributes["hillFormula"] != null && node.Attributes["hillFormula"].Value != string.Empty)
                    hillFormula = COEConvert.ToCOEBoolean(node.Attributes["hillFormula"].Value);
                if (node.Attributes["normalizedChemicalName"] != null && node.Attributes["normalizedChemicalName"].Value != string.Empty)
                    normalizedChemicalName = COEConvert.ToCOEBoolean(node.Attributes["normalizedChemicalName"].Value);
                if (node.Attributes["negate"] != null && node.Attributes["negate"].Value != string.Empty)
                    Negate = COEConvert.ToCOEBoolean(node.Attributes["negate"].Value);
                if (node.Attributes["defaultWildCardPosition"] != null && node.Attributes["defaultWildCardPosition"].Value != string.Empty)
                    defaultWildCardPosition = COEConvert.ToPositions(node.Attributes["defaultWildCardPosition"].Value);
                if (node.Attributes["hint"] != null && node.Attributes["hint"].Value != string.Empty)
                    _hint = node.Attributes["hint"].Value;

                innerText = node.InnerXml;
            }

            /// <summary>
            /// Initializes its members to its default values.
            /// <para>Default values are:</para>
            /// <para>operation = Greater Than Equal</para>
            /// <para>trim = None</para>
            /// <para>caseSensitive = Yes</para>
            /// <para>fullWordSearch = No</para>
            /// <para>hillFormula = No</para>
            /// <para>normalizedChemicalName = No</para>
            /// <para>innerText = Empty string</para>
            /// </summary>
            public TextCriteria()
            {
                this.operation = COEOperators.EQUAL;
                this.trimPosition = Positions.None;
                this.caseSensitive = COEBoolean.Yes;
                this.fullWordSearch = COEBoolean.No;
                this.hillFormula = COEBoolean.No;
                this.normalizedChemicalName = COEBoolean.No;
                this.innerText = string.Empty;
                this.Negate = COEBoolean.No;
                this.defaultWildCardPosition = Positions.None;
                _hint = string.Empty;
            }
            #endregion

            #region ISearchCriteriaBase Members
            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                StringBuilder builder = new StringBuilder("<textCriteria");
                builder.Append(" negate=\"");
                builder.Append(Negate.ToString().ToUpper());
                builder.Append("\" operator=\"");
                builder.Append(operation);
                builder.Append("\" trim=\"");
                builder.Append(trimPosition.ToString().ToUpper());
                builder.Append("\" caseSensitive=\"");
                builder.Append(CaseSensitive.ToString().ToUpper());
                builder.Append("\" fullWordSearch=\"");
                builder.Append(fullWordSearch.ToString().ToUpper());
                builder.Append("\" hillFormula=\"");
                builder.Append(hillFormula.ToString().ToUpper());
                builder.Append("\" normalizedChemicalName=\"");
                builder.Append(normalizedChemicalName.ToString().ToUpper());
                builder.Append("\" defaultWildCardPosition=\"");
                builder.Append(defaultWildCardPosition.ToString().ToUpper());
                builder.Append("\" hint=\"");
                builder.Append(_hint);
                builder.Append("\">");
                builder.Append(innerText);
                builder.Append("</textCriteria>");
                return builder.ToString();
            }
            #endregion
        }

        /// <summary>
        /// Class for querying in text mode.
        /// </summary>
        [Serializable]
        [XmlType("fullTextCriteria")]
        public class FullTextCriteria : ISearchCriteriaBase
        {
            #region Properties

            /// <summary>
            /// The value to compare to.
            /// </summary>
            [XmlText()]
            public string InnerText
            {
                get { return System.Web.HttpUtility.HtmlDecode(innerText); }
                set { innerText = System.Web.HttpUtility.HtmlEncode(value); }
            }
            [XmlIgnore()]
            public override string Value
            {
                get
                {
                    return InnerText;
                }
                set
                {
                    InnerText = value;
                }
            }


            #endregion

            #region Variables
            private string innerText;

            #endregion

            #region Constructors
            /// <summary>
            /// Initializes the text criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public FullTextCriteria(XmlNode node)
            {
                innerText = node.InnerXml;
            }

            /// <summary>
            /// Initializes its members to its default values.
            /// <para>Default values are:</para>

            /// <para>innerText = Empty string</para>
            /// </summary>
            public FullTextCriteria()
            {
                this.innerText = string.Empty;
            }
            #endregion

            #region ISearchCriteriaBase Members
            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                StringBuilder builder = new StringBuilder("<fullTextCriteria");
                builder.Append(">");
                builder.Append(innerText);
                builder.Append("</fullTextCriteria>");
                return builder.ToString();
            }
            #endregion
        }
        /// <summary>
        /// Class for querying in numerical mode.
        /// </summary>
        [Serializable]
        [XmlType("numericalCriteria")]
        public class NumericalCriteria : ISearchCriteriaBase
        {
            #region Properties
            /// <summary>
            /// Determines where trim must be applied.
            /// </summary>
            [XmlAttribute("trim")]
            public SearchCriteria.Positions Trim
            {
                get { return trimPosition; }
                set { trimPosition = value; }
            }

            /// <summary>
            /// Determines which is the operator of the search.
            /// </summary>
            [XmlAttribute("operator")]
            public COEOperators Operator
            {
                get { return operation; }
                set { operation = value; }
            }

            /// <summary>
            /// The value to compare to.
            /// </summary>
            [XmlText()]
            public string InnerText
            {
                get { return System.Web.HttpUtility.HtmlDecode(innerText); }
                set { innerText = System.Web.HttpUtility.HtmlEncode(value); }
            }

            [XmlIgnore()]
            public override string Value
            {
                get
                {
                    return InnerText;
                }
                set
                {
                    InnerText = value;
                }
            }
            #endregion

            #region Variables
            private string innerText;
            private COEOperators operation;
            private SearchCriteria.Positions trimPosition;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes the numerical criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public NumericalCriteria(XmlNode node)
            {
                if (node.Attributes["operator"] != null && node.Attributes["operator"].Value != string.Empty)
                    operation = COEConvert.ToCOEOperator(node.Attributes["operator"].Value);
                if (node.Attributes["trim"] != null && node.Attributes["trim"].Value != string.Empty)
                    trimPosition = COEConvert.ToPositions(node.Attributes["trim"].Value);
                if (node.Attributes["negate"] != null && node.Attributes["negate"].Value != string.Empty)
                    Negate = COEConvert.ToCOEBoolean(node.Attributes["negate"].Value);

                innerText = node.InnerXml;
            }

            /// <summary>
            /// Initializes its members to its default values.
            /// <para>Default values are:</para>
            /// <para>operation = Greater Than Equal</para>
            /// <para>trim = None</para>
            /// <para>innerText = 0</para>
            /// </summary>
            public NumericalCriteria()
            {
                this.operation = COEOperators.GTE;
                this.trimPosition = SearchCriteria.Positions.None;
                this.innerText = string.Empty;
                this.Negate = COEBoolean.No;
            }
            #endregion

            #region ISearchCriteriaBase Members
            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                StringBuilder builder = new StringBuilder("<numericalCriteria");
                builder.Append(" negate=\"");
                builder.Append(Negate.ToString().ToUpper());
                builder.Append("\" operator=\"");
                builder.Append(operation.ToString().ToUpper());
                builder.Append("\" trim=\"");
                builder.Append(trimPosition.ToString().ToUpper());
                builder.Append("\">");
                builder.Append(innerText);
                builder.Append("</numericalCriteria>");
                return builder.ToString();
            }
            #endregion
        }

        /// <summary>
        /// Class for querying in date mode.
        /// </summary>
        [Serializable]
        [XmlType("dateCriteria")]
        public class DateCriteria : ISearchCriteriaBase
        {
            #region Properties
            /// <summary>
            /// Determines which is the operator of the search.
            /// </summary>
            [XmlAttribute("operator")]
            public COEOperators Operator
            {
                get { return operation; }
                set { operation = value; }
            }

            /// <summary>
            /// Determines which is the operator of the search.
            /// </summary>
            [XmlAttribute("culture")]
            public string Culture
            {
                get { return culture; }
                set { culture = value; }
            }

            /// <summary>
            /// The value to compare to.
            /// </summary>
            [XmlText()]
            public string InnerText
            {
                get { return System.Web.HttpUtility.HtmlDecode(innerText); }
                set { innerText = System.Web.HttpUtility.HtmlEncode(value); }
            }
            [XmlIgnore()]
            public override string Value
            {
                get
                {
                    return InnerText;
                }
                set
                {
                    InnerText = value;
                }
            }
            #endregion

            #region Variables
            private string innerText;
            private string culture;
            private COEOperators operation;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes the date criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public DateCriteria(XmlNode node)
            {
                if (node.Attributes["operator"] != null && node.Attributes["operator"].Value != string.Empty)
                    operation = COEConvert.ToCOEOperator(node.Attributes["operator"].Value);
                if (node.Attributes["negate"] != null && node.Attributes["negate"].Value != string.Empty)
                    Negate = COEConvert.ToCOEBoolean(node.Attributes["negate"].Value);
                if (node.Attributes["culture"] != null && node.Attributes["culture"].Value != string.Empty)
                    culture = node.Attributes["culture"].Value;
                innerText = node.InnerXml;
            }

            /// <summary>
            /// Initializes its members to its default values.
            /// <para>Default values are:</para>
            /// <para>operation = Greater Than Equal</para>
            /// <para>innerText = 01/01/2006</para>
            /// </summary>
            public DateCriteria()
            {
                this.operation = COEOperators.GTE;
                this.culture = "en-US";
                this.Negate = COEBoolean.No;
            }
            #endregion

            #region ISearchCriteriaBase Members
            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                StringBuilder builder = new StringBuilder("<dateCriteria");
                builder.Append(" negate=\"");
                builder.Append(Negate.ToString().ToUpper());
                builder.Append("\" culture=\"");
                builder.Append(culture);
                builder.Append("\" operator=\"");
                builder.Append(operation.ToString().ToUpper());
                builder.Append("\">");
                builder.Append(innerText);
                builder.Append("</dateCriteria>");
                return builder.ToString();
            }
            #endregion
        }

        /// <summary>
        /// Class for querying chemical mol weights by CSCartridge.
        /// </summary>
        [Serializable]
        [XmlType("molweightCriteria")]
        public class CSMolWeightCriteria : ISearchCriteriaBase
        {
            #region Variables
            private string _value = string.Empty;
            private string innerText = string.Empty;
            #endregion

            #region Properties
            /// <summary>
            /// Allowed values are: CsCartridge and MolServer.
            /// </summary>
            [XmlAttribute("implementation")]
            public string Implementation
            {
                get { return implementation; }
                set { implementation = value; }
            }

            /// <summary>
            /// Upper limit.
            /// </summary>
            [XmlAttribute("max")]
            public double Max
            {
                get { return max; }
                set { max = value; }
            }

            /// <summary>
            /// Lower limit.
            /// </summary>
            [XmlAttribute("min")]
            public double Min
            {
                get { return min; }
                set { min = value; }
            }

            /// <summary>
            /// Compare methoed.
            /// </summary>
            [XmlAttribute("operator")]
            public COEOperators Operator { get; set; }

            /// <summary>
            /// The value to compare to.
            /// </summary>
            [XmlText()]
            public string InnerText
            {
                get { return System.Web.HttpUtility.HtmlDecode(innerText); }
                set { innerText = System.Web.HttpUtility.HtmlEncode(value); }
            }

            public override string Value
            {
                get
                {
                    //return _value;
                    return InnerText;
                }
                set
                {
                    _value = value;
                    implementation = "CsCartridge";

                    InnerText = _value;

                    if (System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator != System.Globalization.NumberFormatInfo.InvariantInfo.NumberDecimalSeparator)
                    {
                        _value = _value.Replace(System.Globalization.NumberFormatInfo.InvariantInfo.NumberDecimalSeparator, System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
                    }

                    if (value.Trim() != string.Empty)
                    {
                        if (_value.StartsWith("'") || _value.StartsWith("\""))
                            _value = _value.Remove(0, 1);
                        if (_value.EndsWith("'") || _value.EndsWith("\""))
                            _value = _value.Remove(_value.Length - 1, 1);

                        if (_value.ToUpper().Contains("AND"))  //Fixed CSBR-159089
                        {
                            string[] _valueRange = _value.ToUpper().Replace("AND", ";").Split(new char[] { ';' });
                            if (_valueRange[0].Trim().StartsWith(">"))
                            {
                                this.min = double.Parse(_valueRange[0].Trim().Substring(_valueRange[0].Trim().IndexOf('>') + 1));
                                this.max = double.Parse(_valueRange[1].Trim().Substring(_valueRange[1].Trim().IndexOf('<') + 1));
                            }
                            else if (_valueRange[0].Trim().StartsWith("<"))
                            {
                                this.max = double.Parse(_valueRange[0].Trim().Substring(_valueRange[0].Trim().IndexOf('<') + 1));
                                this.min = double.Parse(_valueRange[1].Trim().Substring(_valueRange[1].Trim().IndexOf('>') + 1));
                            }
                        }
                        else if (_value.Trim()[0] == '<')
                        {
                            this.min = 0.0;
                            this.max = double.Parse(_value.Substring(_value.IndexOf('<') + 1));
                        }
                        else if (_value.Trim()[0] == '>')
                        {
                            this.min = double.Parse(_value.Substring(_value.IndexOf('>') + 1));
                            this.max = float.MaxValue;

                        }
                        else
                        {
                            string[] values = NormalizationUtils.ParseRange(_value.Trim());

                            if (values.Length > 1)
                            {
                                this.max = double.Parse(values[1]);
                                this.min = double.Parse(values[0]);
                            }
                            else if (values.Length > 0)
                            {
                                double doubleValue;
                                if (double.TryParse(values[0], out doubleValue))
                                {
                                    NormalizationUtils.GetSearchRange(values[0],
                                                              ref this.min,
                                                              ref this.max);
                                }
                                else
                                {
                                    throw new Exception(Resources.InvalidDataTypeForMolWeightCriteria);
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Variables
            private double min;
            private double max;
            private string implementation;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes the mol weight criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public CSMolWeightCriteria(XmlNode node)
            {
                implementation = "CsCartridge";
                if (node.FirstChild != null && node.FirstChild.Name == "CSCartridgeMolWeightCriteria")
                {
                    if (node.FirstChild.Attributes["min"] != null && node.FirstChild.Attributes["min"].Value != string.Empty)
                    {
                        min = double.Parse(node.FirstChild.Attributes["min"].Value);
                    }
                    if (node.FirstChild.Attributes["max"] != null && node.FirstChild.Attributes["max"].Value != string.Empty)
                    {
                        max = double.Parse(node.FirstChild.Attributes["max"].Value);
                    }
                    innerText = node.FirstChild.InnerXml;
                }
                else
                {
                    if (node.Attributes["min"] != null && node.Attributes["min"].Value != string.Empty)
                    {
                        min = double.Parse(node.Attributes["min"].Value);
                    }
                    if (node.Attributes["max"] != null && node.Attributes["max"].Value != string.Empty)
                    {
                        max = double.Parse(node.Attributes["max"].Value);
                    }
                }
                if (node.Attributes["negate"] != null && node.Attributes["negate"].Value != string.Empty)
                    Negate = COEConvert.ToCOEBoolean(node.Attributes["negate"].Value);

                if (node.FirstChild != null &&
                    node.FirstChild.Attributes != null &&
                    node.FirstChild.Attributes["operator"] != null &&
                    !string.IsNullOrEmpty(node.FirstChild.Attributes["operator"].Value))
                {
                    Operator = COEConvert.ToCOEOperator(node.FirstChild.Attributes["operator"].Value);
                }
            }

            /// <summary>
            /// Initializes its members to its default values.
            /// <para>Default values are:</para>
            /// <para>min = 0</para>
            /// <para>max = 100</para>
            /// </summary>
            public CSMolWeightCriteria()
            {
                this.min = 0;
                this.max = 0;
                this.Negate = COEBoolean.No;
                this.innerText = string.Empty;
                implementation = "CsCartridge";
            }
            #endregion

            #region ISearchCriteriaBase Members
            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                if (implementation.ToLower() != "cscartridge")
                    throw new Exception("Unsupported Implementation. Try using CsCartridge");
                StringBuilder builder = new StringBuilder("<MolWeightCriteria>");
                builder.Append("<CSCartridgeMolWeightCriteria");
                builder.Append(" negate=\"");
                builder.Append(Negate.ToString().ToUpper());
                builder.Append("\" operator=\"");
                builder.Append(Operator);
                builder.Append("\" min=\"");
                builder.Append(min.ToString());
                builder.Append("\" max=\"");
                builder.Append(max.ToString());
                builder.Append("\">");
                builder.Append(innerText);
                builder.Append("</CSCartridgeMolWeightCriteria>");
                builder.Append("</MolWeightCriteria>");
                return builder.ToString();
            }
            #endregion
        }

        /// <summary>
        /// Abstract molweight creteria class for all cartridges except CSCartridge.
        /// </summary>
        [Serializable]
        public abstract class BaseMolWeightCriteria : ISearchCriteriaBase
        {
            #region Variables
            private string _value = string.Empty;
            #endregion

            #region Properties

            /// <summary>
            /// The XML section name
            /// </summary>
            protected abstract string ElementName { get; }

            /// <summary>
            /// Compare methoed.
            /// </summary>
            [XmlAttribute("operator")]
            public COEOperators Operator { get; set; }

            /// <summary>
            /// Upper limit.
            /// </summary>
            [XmlAttribute("parameter2")]
            public double Parameter2 { get; set; }

            /// <summary>
            /// Lower limit.
            /// </summary>
            [XmlAttribute("parameter1")]
            public double Parameter1 { get; set; }

            public override string Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;

                    if (System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator
                        != System.Globalization.NumberFormatInfo.InvariantInfo.NumberDecimalSeparator)
                    {
                        _value =
                            _value.Replace(
                                System.Globalization.NumberFormatInfo.InvariantInfo.NumberDecimalSeparator,
                                System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
                    }

                    if (value.Trim() != string.Empty)
                    {
                        try
                        {
                            if (_value.StartsWith("'") || _value.StartsWith("\""))
                            {
                                _value = _value.Remove(0, 1);
                            }

                            if (_value.EndsWith("'") || _value.EndsWith("\""))
                            {
                                _value = _value.Remove(_value.Length - 1, 1);
                            }

                            if (_value.ToUpper().Contains("AND"))
                            {
                                Operator = COEOperators.EQUAL;
                                string[] _valueRange = _value.ToUpper().Replace("AND", ";").Split(new char[] { ';' });
                                if (_valueRange[0].Trim().StartsWith(">"))
                                {
                                    Parameter1 =
                                        double.Parse(
                                            _valueRange[0].Trim().Substring(_valueRange[0].Trim().IndexOf('>') + 1));
                                    Parameter2 =
                                        double.Parse(
                                            _valueRange[1].Trim().Substring(_valueRange[1].Trim().IndexOf('<') + 1));
                                }
                                else if (_valueRange[0].Trim().StartsWith("<"))
                                {
                                    Parameter2 =
                                        double.Parse(
                                            _valueRange[0].Trim().Substring(_valueRange[0].Trim().IndexOf('<') + 1));
                                    Parameter1 =
                                        double.Parse(
                                            _valueRange[1].Trim().Substring(_valueRange[1].Trim().IndexOf('>') + 1));
                                }
                            }
                            else if (_value.Trim()[0] == '<')
                            {
                                Operator = COEOperators.LT;
                                Parameter1 = double.Parse(_value.Substring(_value.IndexOf('<') + 1));
                            }
                            else if (_value.Trim()[0] == '>')
                            {
                                Operator = COEOperators.GT;
                                Parameter1 = double.Parse(_value.Substring(_value.IndexOf('>') + 1));
                            }
                            else
                            {
                                string[] values = NormalizationUtils.ParseRange(_value.Trim());

                                if (values.Length > 1)
                                {
                                    Operator = COEOperators.EQUAL;
                                    Parameter1 = double.Parse(values[0]);
                                    Parameter2 = double.Parse(values[1]);
                                }
                                else if (values.Length > 0)
                                {
                                    double min = 0.0;
                                    double max = 0.0;
                                    NormalizationUtils.GetSearchRange(values[0], ref min, ref max);
                                    Operator = COEOperators.EQUAL;
                                    Parameter1 = min;
                                    Parameter2 = max;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            throw new Exception(Resources.InvalidDataTypeForMolWeightCriteria);
                        }
                    }
                }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes the mol weight criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            protected BaseMolWeightCriteria(XmlNode node)
            {
                if (node.Attributes["negate"] != null && node.Attributes["negate"].Value != string.Empty)
                {
                    Negate = COEConvert.ToCOEBoolean(node.Attributes["negate"].Value);
                }
                if (node.Attributes["operator"] != null && node.Attributes["operator"].Value != string.Empty)
                {
                    Operator = COEConvert.ToCOEOperator(node.Attributes["operator"].Value);
                }
                if (node.Attributes["parameter2"] != null && node.Attributes["parameter2"].Value != string.Empty)
                {
                    Parameter2 = double.Parse(node.Attributes["parameter2"].Value);
                }
                // Webplayer passing empty string to server if NONE operator selected, but client passing '0'.
                // The best way is use tryParse, but for consistent, keep the old way.
                if (node.InnerXml != null && !string.IsNullOrWhiteSpace(node.InnerXml))
                {
                    Parameter1 = double.Parse(node.InnerXml);
                }
            }

            /// <summary>
            /// Initializes its members to its default values.
            /// <para>Default values are:</para>
            /// <para>min = 0</para>
            /// <para>max = 100</para>
            /// </summary>
            protected BaseMolWeightCriteria()
            {
                Parameter1 = 0;
                Parameter2 = 0;
                Negate = COEBoolean.No;
                //innerText = string.Empty;
            }
            #endregion

            #region ISearchCriteriaBase Members
            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                var builder = new StringBuilder("<");
                builder.Append(ElementName);
                builder.Append(" negate=\"");
                builder.Append(Negate.ToString().ToUpper());
                builder.Append("\" operator=\"");
                builder.Append(Operator);
                builder.Append("\" parameter2=\"");
                builder.Append(Parameter2);
                builder.Append("\">");
                builder.Append(Parameter1);
                builder.Append("</");
                builder.Append(ElementName);
                builder.Append(">");
                return builder.ToString();
            }
            #endregion
        }

        /// <summary>
        /// Class for querying chemical mol weights by Direct Cartridge.
        /// </summary>
        [Serializable]
        [XmlType("directMolweightCriteria")]
        public class DirectMolWeightCriteria : BaseMolWeightCriteria
        {
            #region Constructors

            public DirectMolWeightCriteria()
                : base()
            {
            }

            /// <summary>
            /// Initializes the mol weight criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public DirectMolWeightCriteria(XmlNode node)
                : base(node)
            {
            }
            #endregion

            #region Properties
            /// <summary>
            /// The XML section name
            /// </summary>
            protected override string ElementName
            {
                get
                {
                    return "DirectMolWeightCriteria";
                }
            }
            #endregion
        }


        /// <summary>
        /// Class for querying chemical mol weights by JChem Cartridge.
        /// </summary>
        [Serializable]
        [XmlType("jchemMolweightCriteria")]
        public class JChemMolWeightCriteria : BaseMolWeightCriteria
        {
            #region Constructors

            public JChemMolWeightCriteria()
                : base()
            {
            }
            /// <summary>
            /// Initializes the mol weight criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public JChemMolWeightCriteria(XmlNode node)
                : base(node)
            {
            }
            #endregion

            #region Properties
            /// <summary>
            /// The XML section name
            /// </summary>
            protected override string ElementName
            {
                get
                {
                    return "JChemMolWeightCriteria";
                }
            }
            #endregion
        }

        /// <summary>
        /// Abstract formula criteria class for all Cartridge.
        /// </summary>
        [Serializable]
        public abstract class BaseFormulaCriteria : ISearchCriteriaBase
        {
            #region Variables
            private string implementation;
            #endregion

            #region Properties
            /// <summary>
            /// The value to compare to.
            /// </summary>
            [XmlText()]
            public string Formula { get; set; }

            /// <summary>
            /// Determines which is the operator of the search.
            /// </summary>
            [XmlAttribute("operator")]
            public COEOperators Operator { get; set; }

            [XmlIgnore()]
            public override string Value
            {
                get
                {
                    return Formula;
                }
                set
                {
                    Formula = value;
                }
            }

            /// <summary>
            /// Allowed values are: CsCartridge and MolServer.
            /// </summary>
            [XmlAttribute("implementation")]
            public string Implementation
            {
                get { return implementation; }
                set { implementation = value; }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes its members to its default values.
            /// <para>Default values are:</para>
            /// <para>implementation = Empty string</para>
            /// <para>formula = Empty string</para>
            /// </summary>
            public BaseFormulaCriteria()
            {
                this.implementation = string.Empty;
                this.Formula = string.Empty;
                Operator = COEOperators.CONTAINS;
                this.Negate = COEBoolean.No;
            }

            /// <summary>
            /// Initializes its members with the desired values.
            /// </summary>
            /// <param name="formula">The value to compare to.</param>
            /// <param name="implementation">The implementation.</param>
            public BaseFormulaCriteria(string formula, string implementation)
                : this()
            {
                this.Formula = formula;
                this.implementation = implementation;
                Operator = COEOperators.CONTAINS;
                this.Negate = COEBoolean.No;
            }

            /// <summary>
            /// Initializes the formula criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public BaseFormulaCriteria(XmlNode node)
            {
                if (node.Attributes["negate"] != null && node.Attributes["negate"].Value != string.Empty)
                {
                    Negate = COEConvert.ToCOEBoolean(node.Attributes["negate"].Value);
                }

                Operator = COEOperators.CONTAINS;

                if (node.Attributes != null &&
                    node.Attributes["operator"] != null &&
                    !string.IsNullOrEmpty(node.Attributes["operator"].Value))
                {
                    Operator = COEConvert.ToCOEOperator(node.Attributes["operator"].Value);
                }
            }
            #endregion
        }

        /// <summary>
        /// Class for querying Formulas by CSCartridge.
        /// </summary>
        [Serializable]
        [XmlType("formulaCriteria")]
        public class CSFormulaCriteria : BaseFormulaCriteria
        {
            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// <para>Default values are:</para>
            /// <para>implementation = Empty string</para>
            /// <para>formula = Empty string</para>
            /// </summary>
            public CSFormulaCriteria()
                : base()
            {
            }

            /// <summary>
            /// Initializes its members with the desired values.
            /// </summary>
            /// <param name="formula">The value to compare to.</param>
            /// <param name="implementation">The implementation.</param>
            public CSFormulaCriteria(string formula, string implementation)
                : base(formula, implementation)
            {
            }

            /// <summary>
            /// Initializes the formula criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public CSFormulaCriteria(XmlNode node)
                : base(node)
            {
                if (node.FirstChild != null && node.FirstChild.Name == "CSCartridgeFormulaCriteria")
                {
                    this.Formula = node.FirstChild.InnerText;
                }
                else
                {
                    this.Formula = node.InnerText;
                }

                if (node.FirstChild != null && node.FirstChild.Attributes != null &&
                    node.FirstChild.Attributes["operator"] != null &&
                    !string.IsNullOrEmpty(node.FirstChild.Attributes["operator"].Value))
                {
                    Operator = COEConvert.ToCOEOperator(node.FirstChild.Attributes["operator"].Value);
                }
            }

            #endregion

            #region ISearchCriteriaBase Members

            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                StringBuilder builder = new StringBuilder("<FormulaCriteria>");
                builder.Append("<CSCartridgeFormulaCriteria");
                builder.Append(" negate=\"");
                builder.Append(Negate.ToString().ToUpper());
                builder.Append("\" operator=\"");
                builder.Append(Operator);
                builder.Append("\">");
                builder.Append(this.Formula);
                builder.Append("</CSCartridgeFormulaCriteria>");
                builder.Append("</FormulaCriteria>");
                return builder.ToString();
            }

            #endregion
        }

        /// <summary>
        /// Class for querying Formulas by Direct Cartridge.
        /// </summary>
        [Serializable]
        [XmlType("directFormulaCriteria")]
        public class DirectFormulaCriteria : BaseFormulaCriteria
        {
            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// <para>Default values are:</para>
            /// <para>implementation = Empty string</para>
            /// <para>formula = Empty string</para>
            /// </summary>
            public DirectFormulaCriteria()
                : base()
            {
            }

            /// <summary>
            /// Initializes the formula criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public DirectFormulaCriteria(XmlNode node)
                : base(node)
            {
                if (node.Attributes != null && node.Attributes["full"].Value.ToLower() == "true")
                {
                    Operator = COEOperators.EQUAL;
                }

                Formula = node.InnerText;
            }

            #endregion

            #region ISearchCriteriaBase Members

            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                var builder = new StringBuilder("<DirectFormulaCriteria");
                builder.Append(" negate=\"");
                builder.Append(Negate.ToString().ToUpper());
                builder.Append("\" full=\"");
                if (Operator == COEOperators.EQUAL)
                {
                    builder.Append("true");
                }
                else
                {
                    builder.Append("false");
                }
                builder.Append("\" operator=\"");
                builder.Append(Operator);
                builder.Append("\">");
                builder.Append(Formula);
                builder.Append("</DirectFormulaCriteria>");
                return builder.ToString();
            }

            #endregion
        }

        /// <summary>
        /// Class for querying Formulas by Direct Cartridge.
        /// </summary>
        [Serializable]
        [XmlType("jchemFormulaCriteria")]
        public class JChemFormulaCriteria : BaseFormulaCriteria
        {
            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// <para>Default values are:</para>
            /// <para>implementation = Empty string</para>
            /// <para>formula = Empty string</para>
            /// </summary>
            public JChemFormulaCriteria()
                : base()
            {
            }

            /// <summary>
            /// Initializes the formula criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public JChemFormulaCriteria(XmlNode node)
                : base(node)
            {
                Formula = node.InnerText;
            }

            #endregion

            #region ISearchCriteriaBase Members

            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                var builder = new StringBuilder("<JChemFormulaCriteria");
                builder.Append(" negate=\"");
                builder.Append(Negate.ToString().ToUpper());
                builder.Append("\" full=\"");
                if (Operator == COEOperators.EQUAL)
                {
                    builder.Append("true");
                }
                else
                {
                    builder.Append("false");
                }
                builder.Append("\" operator=\"");
                builder.Append(Operator);
                builder.Append("\">");
                builder.Append(Formula);
                builder.Append("</JChemFormulaCriteria>");
                return builder.ToString();
            }

            #endregion
        }

        /// <summary>
        /// Class for querying molecular structures.
        /// </summary>
        [Serializable]
        [XmlType("structureCriteria")]
        public class StructureCriteria : ISearchCriteriaBase
        {
            #region Variables
            private string implementation;
            private string cartridgeSchema;
            private COEBoolean absoluteHitsRel;
            private COEBoolean relativeTetStereo;
            private TetrahedralStereoMatching tetrahedralStereo;
            private int simThreshold;
            private COEBoolean reactionCenter;
            private COEBoolean fullSearch;
            private COEBoolean tautometer;
            private COEBoolean fragmentsOverlap;
            private COEBoolean permitExtraneousFragmentsIfRXN;
            private COEBoolean permitExtraneousFragments;
            private COEBoolean doubleBondStereo;
            private COEBoolean hitAnyChargeHetero;
            private COEBoolean identity;
            private COEBoolean hitAnyChargeCarbon;
            private COEBoolean similar;
            private COEBoolean ignoreImplicitHydrogens;
            private COEBoolean highlight;
            private string format; // Is this option necessary?
            private string structure;
            private string query8000;
            private string cartridgeParams;
            #endregion

            #region Properties
            /// <summary>
            /// Cartridge schema is set based on configuration.
            /// </summary>
            [XmlIgnore()]
            public string CartridgeSchema
            {
                get { return cartridgeSchema; }
                set { cartridgeSchema = value; }
            }

            /// <summary>
            /// Allowed values are: CsCartridge and MolServer.
            /// </summary>
            [XmlAttribute("implementation")]
            public string Implementation
            {
                get { return implementation; }
                set { implementation = value; }
            }

            /// <summary>
            /// Describes in which format comes the structure.
            /// </summary>
            [XmlAttribute("format")]
            public string Format
            {
                get { return format; }
                set { format = value; }
            }

            /// <summary>
            /// Gets or sets if absolute hits or not. Default is NO.
            /// </summary>
            [XmlAttribute("absoluteHitsRel")]
            public COEBoolean AbsoluteHitsRel
            {
                get { return absoluteHitsRel; }
                set { absoluteHitsRel = value; }
            }

            /// <summary>
            /// Gets or sets if relative tet stereo. Default is NO.
            /// </summary>
            [XmlAttribute("relativeTetStereo")]
            public COEBoolean RelativeTetStereo
            {
                get { return relativeTetStereo; }
                set { relativeTetStereo = value; }
            }

            /// <summary>
            /// Gets or sets if reaction search can match only reaction centers. Default is YES.
            /// </summary>
            [XmlAttribute("reactionCenter")]
            public COEBoolean ReactionCenter
            {
                get { return reactionCenter; }
                set { reactionCenter = value; }
            }

            /// <summary>
            /// Gets or sets if it will be a full structure search. Default is substructure search.
            /// </summary>
            [XmlAttribute("fullSearch")]
            public COEBoolean FullSearch
            {
                get { return fullSearch; }
                set { fullSearch = value; }
            }


            /// <summary>
            /// Gets or sets if tautometer. Default is NO.
            /// </summary>
            [XmlAttribute("tautometer")]
            public COEBoolean Tautometer
            {
                get { return tautometer; }
                set { tautometer = value; }
            }

            /// <summary>
            /// Gets or sets if fragments can overlap. Default is NO.
            /// </summary>
            [XmlAttribute("fragmentsOverlap")]
            public COEBoolean FragmentsOverlap
            {
                get { return fragmentsOverlap; }
                set { fragmentsOverlap = value; }
            }

            /// <summary>
            /// Gets or sets if permit extraneous fragments. Default is NO.
            /// </summary>
            [XmlAttribute("permitExtraneousFragmentsIfRXN")]
            public COEBoolean PermitExtraneousFragmentsIfRXN
            {
                get { return permitExtraneousFragmentsIfRXN; }
                set { permitExtraneousFragmentsIfRXN = value; }
            }

            /// <summary>
            /// Gets or sets if permit extraneous fragments. Default is NO.
            /// </summary>
            [XmlAttribute("permitExtraneousFragments")]
            public COEBoolean PermitExtraneousFragments
            {
                get { return permitExtraneousFragments; }
                set { permitExtraneousFragments = value; }
            }

            /// <summary>
            /// Gets or sets if double bond stereo matching. Default is YES.
            /// </summary>
            [XmlAttribute("doubleBondStereo")]
            public COEBoolean DoubleBondStereo
            {
                get { return doubleBondStereo; }
                set { doubleBondStereo = value; }
            }

            /// <summary>
            /// Gets or sets if hit any charged hetero atom. Default is YES.
            /// </summary>
            [XmlAttribute("hitAnyChargeHetero")]
            public COEBoolean HitAnyChargeHetero
            {
                get { return hitAnyChargeHetero; }
                set { hitAnyChargeHetero = value; }
            }

            /// <summary>
            /// Gets or sets if identity matching. Default is NO.
            /// </summary>
            [XmlAttribute("identity")]
            public COEBoolean Identity
            {
                get { return identity; }
                set { identity = value; }
            }

            /// <summary>
            /// Gets or sets if hit any charged carbon. Default is YES.
            /// </summary>
            [XmlAttribute("hitAnyChargeCarbon")]
            public COEBoolean HitAnyChargeCarbon
            {
                get { return hitAnyChargeCarbon; }
                set { hitAnyChargeCarbon = value; }
            }

            /// <summary>
            /// Gets or sets if similarity search. Default is NO.
            /// </summary>
            [XmlAttribute("similar")]
            public COEBoolean Similar
            {
                get { return similar; }
                set { similar = value; }
            }

            /// <summary>
            /// IgnoreImplicitHydrogens is deprecated and should not be used.
            /// </summary>
            /// <remarks>This property is deprecated and should not be used</remarks>
            [XmlAttribute("ignoreImplicitHydrogens")]
            public COEBoolean IgnoreImplicitHydrogens
            {
                get { return ignoreImplicitHydrogens; }
                set { ignoreImplicitHydrogens = value; }
            }

            /// <summary>
            /// Gets or sets the similarity percentage. It is taken into account when SIMILAR=YES is set as well, 
            /// otherwise disregarded. Default value (used if it is omitted) is 90 percent.
            /// </summary>
            [XmlAttribute("simThreshold")]
            public int SimThreshold
            {
                get { return simThreshold; }
                set { simThreshold = value; }
            }

            /// <summary>
            /// Gets or sets the tetrahedral stereo matching. Default is YES.
            /// </summary>
            [XmlAttribute("tetrahedralStereo")]
            public TetrahedralStereoMatching TetrahedralStereo
            {
                get { return tetrahedralStereo; }
                set { tetrahedralStereo = value; }
            }


            /// <summary>
            /// Gets or sets the tetrahedral stereo matching. Default is YES.
            /// </summary>
            [XmlAttribute("highlight")]
            public COEBoolean Highlight
            {
                get { return highlight; }
                set { highlight = value; }
            }

            /// <summary>
            /// Query8000 is the first 4000 characters of the query string. The query string can be an encoded CDX document, a SMILES string, or a MolFile. For more information about these formats, please see DDL Considerations. The Oracle server limits the size of string literals to 4000 characters. 
            /// </summary>
            [XmlAttribute("query8000")]
            public string Query8000
            {
                get { return this.query8000; }
                set { this.query8000 = value; }
            }

            /// <summary>
            /// CartridgeParams allows you to pass a list of structure query parameters
            /// that cartridge would accept directly as a comma delimited list
            /// If you use this then all other properties set are ignored
            /// </summary>
            [XmlAttribute("cartridgeParams")]
            public string CartridgeParams
            {
                get { return this.cartridgeParams; }
                set { this.cartridgeParams = value; }
            }

            /// <summary>
            /// The structure to compare to.
            /// </summary>
            [XmlText()]
            public string Structure
            {
                get { return structure; }
                set { structure = value; }
            }

            /// <summary>
            /// Query4000 is the first 4000 characters of the query string. The query string can be an encoded CDX document, a SMILES string, or a MolFile. For more information about these formats, please see DDL Considerations. The Oracle server limits the size of string literals to 4000 characters. 
            /// </summary>
            [XmlIgnore()]
            public string Query4000
            {
                get { return Structure; }
                set { Structure = value; }
            }

            [XmlIgnore()]
            public override string Value
            {
                get
                {
                    return Structure;
                }
                set
                {
                    Structure = value;
                }
            }
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// Default values are:
            /// <para>implementation: Empty string.</para>
            /// <para>cartridgeSchema: Empty string.</para>
            /// <para>absoluteHitsRel: No.</para>
            /// <para>relativeTetStereo: No.</para>
            /// <para>tetrahedralStereo: Yes.</para>
            /// <para>simThreshold: 100.</para>
            /// <para>reactionCenter: Yes.</para>
            /// <para>fullSearch: No.</para>
            /// <para>tautometer: No.</para>
            /// <para>fragmentsOverlap: No.</para>
            /// <para>permitExtraneousFragmentsIfRXN: No.</para>
            /// <para>permitExtraneousFragments: No.</para>
            /// <para>doubleBondStereo: Yes.</para>
            /// <para>hitAnyChargeHetero: Yes.</para>
            /// <para>identity: No.</para>
            /// <para>hitAnyChargeCarbon: Yes.</para>
            /// <para>similar: No.</para>
            /// <para>format: base64cdx.</para>
            /// <para>structure: Empty string.</para>
            /// <para>query8000: Empty string.</para>
            /// <para>cartridgeParams: Empty string.</para>
            /// </summary>
            public StructureCriteria()
            {
                this.implementation = this.cartridgeSchema = string.Empty;
                this.absoluteHitsRel = COEBoolean.No;
                this.relativeTetStereo = COEBoolean.No;
                this.tetrahedralStereo = TetrahedralStereoMatching.Yes;
                this.simThreshold = 100;
                this.reactionCenter = COEBoolean.Yes;
                this.fullSearch = COEBoolean.No;
                this.tautometer = COEBoolean.No;
                this.fragmentsOverlap = COEBoolean.No;
                this.permitExtraneousFragmentsIfRXN = COEBoolean.No;
                this.permitExtraneousFragments = COEBoolean.No;
                this.doubleBondStereo = COEBoolean.Yes;
                this.hitAnyChargeHetero = COEBoolean.Yes;
                this.identity = COEBoolean.No;
                this.hitAnyChargeCarbon = COEBoolean.Yes;
                this.similar = COEBoolean.No;
                this.ignoreImplicitHydrogens = COEBoolean.No;
                this.highlight = COEBoolean.No;
                this.format = "base64cdx"; // Is this option necessary?
                this.structure = string.Empty;
                this.query8000 = string.Empty;
                this.Negate = COEBoolean.No;
                this.CartridgeParams = string.Empty;
            }

            /// <summary>
            /// Initializes the structure criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public StructureCriteria(XmlNode node)
                : this()
            {
                implementation = "CsCartridge";
                XmlNode structureNode = node;
                if (node.FirstChild != null && node.FirstChild.Name == "CSCartridgeStructureCriteria")
                {
                    structureNode = node.FirstChild;
                }

                if (structureNode.Attributes["absoluteHitsRel"] != null)
                    this.absoluteHitsRel = COEConvert.ToCOEBoolean(structureNode.Attributes["absoluteHitsRel"].Value);
                if (structureNode.Attributes["relativeTetStereo"] != null)
                    this.relativeTetStereo = COEConvert.ToCOEBoolean(structureNode.Attributes["relativeTetStereo"].Value);
                if (structureNode.Attributes["tetrahedralStereo"] != null)
                    this.tetrahedralStereo = COEConvert.ToTetrahedralStereoMatching(structureNode.Attributes["tetrahedralStereo"].Value);
                if (structureNode.Attributes["simThreshold"] != null)
                    this.simThreshold = int.Parse(structureNode.Attributes["simThreshold"].Value);
                if (structureNode.Attributes["reactionCenter"] != null)
                    this.reactionCenter = COEConvert.ToCOEBoolean(structureNode.Attributes["reactionCenter"].Value);
                if (structureNode.Attributes["fullSearch"] != null)
                    this.fullSearch = COEConvert.ToCOEBoolean(structureNode.Attributes["fullSearch"].Value);
                if (structureNode.Attributes["tautometer"] != null)
                    this.tautometer = COEConvert.ToCOEBoolean(structureNode.Attributes["tautometer"].Value);
                if (structureNode.Attributes["fragmentsOverlap"] != null)
                    this.fragmentsOverlap = COEConvert.ToCOEBoolean(structureNode.Attributes["fragmentsOverlap"].Value);
                if (structureNode.Attributes["permitExtraneousFragmentsIfRXN"] != null)
                    this.permitExtraneousFragmentsIfRXN = COEConvert.ToCOEBoolean(structureNode.Attributes["permitExtraneousFragmentsIfRXN"].Value);
                if (structureNode.Attributes["permitExtraneousFragments"] != null)
                    this.permitExtraneousFragments = COEConvert.ToCOEBoolean(structureNode.Attributes["permitExtraneousFragments"].Value);
                if (structureNode.Attributes["doubleBondStereo"] != null)
                    this.doubleBondStereo = COEConvert.ToCOEBoolean(structureNode.Attributes["doubleBondStereo"].Value);
                if (structureNode.Attributes["hitAnyChargeHetero"] != null)
                    this.hitAnyChargeHetero = COEConvert.ToCOEBoolean(structureNode.Attributes["hitAnyChargeHetero"].Value);
                if (structureNode.Attributes["identity"] != null)
                    this.identity = COEConvert.ToCOEBoolean(structureNode.Attributes["identity"].Value);
                if (structureNode.Attributes["hitAnyChargeCarbon"] != null)
                    this.hitAnyChargeCarbon = COEConvert.ToCOEBoolean(structureNode.Attributes["hitAnyChargeCarbon"].Value);
                if (structureNode.Attributes["similar"] != null)
                    this.similar = COEConvert.ToCOEBoolean(structureNode.Attributes["similar"].Value);
                if (structureNode.Attributes["ignoreImplicitHydrogens"] != null)
                    this.ignoreImplicitHydrogens = COEConvert.ToCOEBoolean(structureNode.Attributes["ignoreImplicitHydrogens"].Value);
                if (structureNode.Attributes["highlight"] != null)
                    this.highlight = COEConvert.ToCOEBoolean(structureNode.Attributes["highlight"].Value);
                if (structureNode.Attributes["format"] != null)
                    this.format = structureNode.Attributes["format"].Value;
                if (structureNode.Attributes["identity"] != null)
                    this.identity = COEConvert.ToCOEBoolean(structureNode.Attributes["identity"].Value);
                if (structureNode.Attributes["query8000"] != null)
                    this.Query8000 = structureNode.Attributes["query8000"].Value;
                if (structureNode.Attributes["cartridgeParams"] != null)
                    this.CartridgeParams = structureNode.Attributes["cartridgeParams"].Value;
                this.Structure = structureNode.InnerText;

                if (node.Attributes["negate"] != null && node.Attributes["negate"].Value != string.Empty)
                    Negate = COEConvert.ToCOEBoolean(node.Attributes["negate"].Value);
            }
            #endregion

            #region ISearchCriteriaBase Members
            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                if (implementation.ToLower() == "cscartridge")
                {
                    StringBuilder builder = new StringBuilder("<StructureCriteria");
                    builder.Append(" negate=\"");
                    builder.Append(Negate.ToString().ToUpper());
                    builder.Append("\">");
                    builder.Append("<CSCartridgeStructureCriteria");

                    if (CartridgeSchema.Trim() != "")
                        builder.AppendFormat(" cartridgeSchema=\"" + CartridgeSchema + "\"");

                    builder.AppendFormat(" absoluteHitsRel=\"" + AbsoluteHitsRel.ToString().ToUpper() + "\"");
                    builder.AppendFormat(" relativeTetStereo=\"" + RelativeTetStereo.ToString().ToUpper() + "\"");

                    builder.Append(" tetrahedralStereo=\"");
                    builder.Append(COEConvert.ToString(this.tetrahedralStereo));
                    builder.Append("\"");

                    builder.AppendFormat(" simThreshold=\"{0}\"", SimThreshold.ToString());
                    builder.AppendFormat(" reactionCenter=\"{0}\"", ReactionCenter.ToString().ToUpper());
                    builder.AppendFormat(" fullSearch=\"{0}\"", FullSearch.ToString().ToUpper());
                    builder.AppendFormat(" tautometer=\"{0}\"", Tautometer.ToString().ToUpper());
                    builder.AppendFormat(" fragmentsOverlap=\"{0}\"", FragmentsOverlap.ToString().ToUpper());
                    builder.AppendFormat(" permitExtraneousFragmentsIfRXN=\"{0}\"", PermitExtraneousFragmentsIfRXN.ToString().ToUpper());
                    builder.AppendFormat(" permitExtraneousFragments=\"{0}\"", PermitExtraneousFragments.ToString().ToUpper());
                    builder.AppendFormat(" doubleBondStereo=\"{0}\"", DoubleBondStereo.ToString().ToUpper());
                    builder.AppendFormat(" hitAnyChargeHetero=\"{0}\"", HitAnyChargeHetero.ToString().ToUpper());
                    builder.AppendFormat(" identity=\"{0}\"", Identity.ToString().ToUpper());
                    builder.AppendFormat(" hitAnyChargeCarbon=\"{0}\"", HitAnyChargeCarbon.ToString().ToUpper());
                    builder.AppendFormat(" similar=\"{0}\"", Similar.ToString().ToUpper());
                    builder.AppendFormat(" ignoreImplicitHydrogens=\"{0}\"", IgnoreImplicitHydrogens.ToString().ToUpper());
                    builder.AppendFormat(" highlight=\"{0}\"", Highlight.ToString().ToUpper());
                    builder.AppendFormat(" cartridgeParams=\"{0}\"", CartridgeParams.ToString().ToUpper());

                    if (this.Query8000.Trim() != "")
                        builder.AppendFormat(" query8000=\"{0}\"", this.Query8000.Trim());

                    if (Format.Trim() != "")
                        builder.AppendFormat(" format=\"{0}\"", Format.Trim());

                    builder.Append(">");
                    builder.Append(this.structure);
                    builder.Append("</CSCartridgeStructureCriteria>");
                    builder.Append("</StructureCriteria>");
                    return builder.ToString();
                }
                else
                    throw new Exception("Unsupported Implementation. Try using CsCartridge");
            }
            #endregion
        }

        /// <summary>
        /// Class for full structure search by Direct Cartridge.
        /// </summary>
        [Serializable]
        [XmlType("directFlexmatchCriteria")]
        public class DirectFlexmatchCriteria : ISearchCriteriaBase
        {
            #region Properties

            /// <summary>
            /// Allowed values are: DirectCartridge.
            /// </summary>
            [XmlAttribute("implementation")]
            public string Implementation { get; set; }

            /// <summary>
            /// For a 2D flexmatch search, it is a string containing the flexmatch switches. 
            /// </summary>
            [XmlAttribute("flexmatchparameters")]
            public string FlexmatchParameters { get; set; }

            /// <summary>
            /// A number that is equal to the flexmatch-number parameter used with the flexmatchtimeout 
            /// operator. This parameter only applies if you use flexmatchtimeout.
            /// </summary>
            [XmlAttribute("flexmatchnumber")]
            public string FlexmatchNumber { get; set; }

            /// <summary>
            /// A molecule that uses one of the following formats: 
            /// Molfile string, Chime string, molecule object, SMILES string, Accelrys Line Notation string
            /// </summary>
            [XmlText()]
            public string Query { get; set; }

            [XmlIgnore()]
            public override string Value
            {
                get
                {
                    return Query;
                }
                set
                {
                    Query = value;
                }
            }

            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public DirectFlexmatchCriteria()
            {
                Implementation = "DirectCartridge";
                Query = String.Empty;
                FlexmatchParameters = String.Empty;
                FlexmatchNumber = String.Empty;
            }

            /// <summary>
            /// Initializes the structure criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public DirectFlexmatchCriteria(XmlNode node)
                : this()
            {
                Implementation = "DirectCartridge";

                if (node.Attributes["negate"] != null && node.Attributes["negate"].Value != string.Empty)
                {
                    Negate = COEConvert.ToCOEBoolean(node.Attributes["negate"].Value);
                }
                if (node.Attributes["flexmatchparameters"] != null)
                {
                    FlexmatchParameters = node.Attributes["flexmatchparameters"].Value;
                }
                if (node.Attributes["flexmatchnumber"] != null)
                {
                    FlexmatchNumber = node.Attributes["flexmatchnumber"].Value;
                }

                Query = node.InnerText;
            }

            #endregion

            #region ISearchCriteriaBase Members
            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                var builder = new StringBuilder("<DirectFlexmatchCriteria");
                builder.Append(" negate=\"");
                builder.Append(Negate.ToString().ToUpper());
                builder.AppendFormat("\" flexmatchparameters=\"" + FlexmatchParameters + "\"");
                if (!string.IsNullOrEmpty(FlexmatchNumber))
                {
                    builder.AppendFormat(" flexmatchnumber=\"" + FlexmatchNumber + "\"");
                }

                builder.Append(">");
                builder.Append(Query);
                builder.Append("</DirectFlexmatchCriteria>");
                return builder.ToString();
            }
            #endregion
        }

        /// <summary>
        /// Class for substructure search by Direct Cartridge.
        /// </summary>
        [Serializable]
        [XmlType("directSssCriteria")]
        public class DirectSssCriteria : ISearchCriteriaBase
        {
            #region Properties

            /// <summary>
            /// Allowed values are: CsCartridge and MolServer.
            /// </summary>
            [XmlAttribute("implementation")]
            public string Implementation { get; set; }

            /// <summary>
            /// Any or a combination of the following values: 
            /// NOFS, ORIEN, GENERICS
            /// </summary>
            [XmlAttribute("option")]
            public string Option { get; set; }

            /// <summary>
            /// A number that is equal to the sss-number parameter used with the ssshighlight, ssstimeout, 
            /// ssscount, sss_highlight_molfile, and sss_highlight_chime operators. 
            /// This parameter only applies if you use these other operators.
            /// </summary>
            [XmlAttribute("sssnumber")]
            public string SssNumber { get; set; }

            /// <summary>
            /// A molecule that uses one of the following formats: 
            /// Molfile string, Chime string, molecule object, SMILES string, Accelrys Line Notation string
            /// </summary>
            [XmlText()]
            public string Query { get; set; }

            [XmlIgnore()]
            public override string Value
            {
                get
                {
                    return Query;
                }
                set
                {
                    Query = value;
                }
            }

            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public DirectSssCriteria()
            {
                Implementation = "DirectCartridge";
                Query = String.Empty;
                Option = String.Empty;
                SssNumber = String.Empty;
            }

            /// <summary>
            /// Initializes the structure criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public DirectSssCriteria(XmlNode node)
                : this()
            {
                if (node.Attributes["negate"] != null && node.Attributes["negate"].Value != string.Empty)
                {
                    Negate = COEConvert.ToCOEBoolean(node.Attributes["negate"].Value);
                }
                if (node.Attributes["option"] != null)
                {
                    Option = node.Attributes["option"].Value;
                }
                if (node.Attributes["sssnumber"] != null)
                {
                    SssNumber = node.Attributes["sssnumber"].Value;
                }

                Query = node.InnerText;
            }

            #endregion

            #region ISearchCriteriaBase Members
            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                var builder = new StringBuilder("<DirectSssCriteria");
                builder.Append(" negate=\"");
                builder.Append(Negate.ToString().ToUpper());
                builder.AppendFormat("\" sssnumber=\"" + SssNumber + "\"");
                if (!string.IsNullOrEmpty(Option))
                {
                    builder.AppendFormat(" option=\"" + Option + "\"");
                }

                builder.Append(">");
                builder.Append(Query);
                builder.Append("</DirectSssCriteria>");
                return builder.ToString();
            }
            #endregion
        }

        /// <summary>
        /// Class for similar structure search by Direct Cartridge.
        /// </summary>
        [Serializable]
        [XmlType("directSimilarCriteria")]
        public class DirectSimilarCriteria : ISearchCriteriaBase
        {
            #region Variables

            private int simThreshold;

            #endregion

            #region Properties

            /// <summary>
            /// Allowed values are: CsCartridge and MolServer.
            /// </summary>
            [XmlAttribute("implementation")]
            public string Implementation { get; set; }

            /// <summary>
            /// A VARCHAR2 containing the similarity threshhold values (as percentages) 
            /// and an optional 'SUB' or 'SUPER' flag
            /// </summary>
            [XmlAttribute("similarityvalues")]
            private string SimilarityValues { get; set; }

            /// <summary>
            /// An integer from 0 to 100 that represents the degree of similarity expressed as apercentage.
            /// This property has two purpose:
            ///   One is to hold the number the user set on Structure Filter panel
            ///   The other is to generate 'SimilarityValues' argument for similar function in Direct Cartridge
            /// </summary>
            [XmlAttribute("threshold")]
            public int SimThreshold
            {
                get
                {
                    return simThreshold;
                }
                set
                {
                    simThreshold = value;

                    // The similar function in Direct Cartridge support 3 kinds of similarity calculation:
                    //   - normal similarity calculation
                    //   - 'SUB' similarity calculation
                    //   - 'SUPER' similarity calculation
                    //
                    // here we used normal similarity calculation
                    SimilarityValues = simThreshold.ToString();
                }
            }

            /// <summary>
            /// A number that is equal to the similar-number parameter used with the similarity 
            /// operator. This parameter only applies if you use the similarity operator.
            /// </summary>
            [XmlAttribute("similarnumber")]
            public string SimilarNumber { get; set; }

            /// <summary>
            /// A molecule that uses one of the following formats: 
            /// Molfile string, Chime string, molecule object, SMILES string, Accelrys Line Notation string
            /// </summary>
            [XmlText()]
            public string Query { get; set; }

            [XmlIgnore()]
            public override string Value
            {
                get
                {
                    return Query;
                }
                set
                {
                    Query = value;
                }
            }

            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public DirectSimilarCriteria()
            {
                Implementation = "DirectCartridge";
                Query = String.Empty;
                SimilarNumber = String.Empty;
                SimThreshold = 0;
            }

            /// <summary>
            /// Initializes the structure criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public DirectSimilarCriteria(XmlNode node)
                : this()
            {
                if (node.Attributes["negate"] != null && node.Attributes["negate"].Value != string.Empty)
                {
                    Negate = COEConvert.ToCOEBoolean(node.Attributes["negate"].Value);
                }
                if (node.Attributes["similarityvalues"] != null)
                {
                    SimilarityValues = node.Attributes["similarityvalues"].Value;
                }
                if (node.Attributes["threshold"] != null)
                {
                    SimThreshold = Convert.ToInt32(node.Attributes["threshold"].Value);
                }
                if (node.Attributes["similarnumber"] != null)
                {
                    SimilarNumber = node.Attributes["similarnumber"].Value;
                }

                Query = node.InnerText;
            }

            #endregion

            #region ISearchCriteriaBase Members
            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                var builder = new StringBuilder("<DirectSimilarCriteria");
                builder.Append(" negate=\"");
                builder.Append(Negate.ToString().ToUpper());
                builder.AppendFormat("\" similarityvalues=\"" + SimilarityValues + "\"");
                builder.AppendFormat(" threshold=\"" + SimThreshold + "\"");
                if (!string.IsNullOrEmpty(SimilarNumber))
                {
                    builder.AppendFormat(" similarnumber=\"" + SimilarNumber + "\"");
                }

                builder.Append(">");
                builder.Append(Query);
                builder.Append("</DirectSimilarCriteria>");
                return builder.ToString();
            }
            #endregion
        }

        /// <summary>
        /// Class for structure search by JChem Cartridge.
        /// </summary>
        [Serializable]
        [XmlType("jchemStructureCriteria")]
        public class JChemStructureCriteria : ISearchCriteriaBase
        {
            #region Properties

            /// <summary>
            /// Allowed values are: JChemCartridge.
            /// </summary>
            [XmlAttribute("implementation")]
            public string Implementation { get; set; }

            /// <summary>
            /// Gets or sets the structure search type, Full:f, substructure:s, similarity:t
            /// </summary>
            [XmlAttribute("searchType")]
            public JChemStructureSearchType SearchType { get; set; }

            /// <summary>
            /// Gets or sets the similarity percentage. It is taken into account when SIMILAR=YES is set as well, 
            /// otherwise disregarded. Default value (used if it is omitted) is 90 percent.
            /// </summary>
            [XmlAttribute("simThreshold")]
            public int SimThreshold { get; set; }

            /// <summary>
            /// A molecule that uses one of the following formats: 
            /// Molfile string, Chime string, molecule object, SMILES string, Accelrys Line Notation string
            /// </summary>
            [XmlText()]
            public string Query { get; set; }

            [XmlIgnore()]
            public override string Value
            {
                get
                {
                    return Query;
                }
                set
                {
                    Query = value;
                }
            }

            #endregion

            #region Constructors
            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public JChemStructureCriteria()
            {
                Implementation = "JChemCartridge";
                Query = String.Empty;

                // Default search type is full search.
                SearchType = JChemStructureSearchType.Full;
                SimThreshold = -1;
            }

            /// <summary>
            /// Initializes the structure criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public JChemStructureCriteria(XmlNode node)
                : this()
            {
                Implementation = "JChemCartridge";

                if (node.Attributes["negate"] != null && node.Attributes["negate"].Value != string.Empty)
                {
                    this.Negate = COEConvert.ToCOEBoolean(node.Attributes["negate"].Value);
                }
                if (node.Attributes["searchType"] != null)
                {
                    this.SearchType = COEConvert.ToStructureSearchType(node.Attributes["searchType"].Value.Trim());
                }
                if (node.Attributes["simThreshold"] != null)
                {
                    this.SimThreshold = int.Parse(node.Attributes["simThreshold"].Value);
                }

                Query = node.InnerText;
            }

            #endregion

            #region ISearchCriteriaBase Members
            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                var builder = new StringBuilder("<JChemStructureCriteria");

                builder.AppendFormat(" negate=\"{0}\"", Negate.ToString().ToUpper());
                builder.AppendFormat(" searchType=\"{0}\"", COEConvert.ToString(SearchType));
                if (SimThreshold >= 0)
                {
                    builder.AppendFormat(" simThreshold=\"{0}\"", SimThreshold.ToString());
                }
                builder.Append(">");
                builder.Append(Query);
                builder.Append("</JChemStructureCriteria>");

                return builder.ToString();
            }
            #endregion
        }

        /// <summary>
        /// Class for querying molecular structures based on a list of structures
        /// </summary>
        [Serializable]
        [XmlType("structureListCriteria")]
        public class StructureListCriteria : ISearchCriteriaBase
        {
            #region Variables
            private string _structureList;
            #endregion

            #region Properties
            /// <summary>
            /// The structure to compare to.
            /// </summary>
            [XmlText()]
            public string StructureList
            {
                get { return System.Web.HttpUtility.HtmlDecode(_structureList); }
                set { _structureList = System.Web.HttpUtility.HtmlEncode(value); }
            }

            [XmlIgnore()]
            public override string Value
            {
                get
                {
                    return StructureList;
                }
                set
                {
                    StructureList = value;
                }
            }
            #endregion

            #region Constructors
            public StructureListCriteria()
            {
                this.StructureList = string.Empty;
                this.Negate = COEBoolean.No;
            }

            public StructureListCriteria(XmlNode node)
            {
                this.StructureList = node.InnerText;

                if (node.Attributes["negate"] != null && node.Attributes["negate"].Value != string.Empty)
                    this.Negate = COEConvert.ToCOEBoolean(node.Attributes["negate"].Value);
            }
            #endregion

            #region ISearchCriteriaBase Members
            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                StringBuilder builder = new StringBuilder("<structureListCriteria");
                builder.Append(" negate=\"");
                builder.Append(Negate.ToString().ToUpper());
                builder.Append("\">");
                builder.Append(_structureList);
                builder.Append("</structureListCriteria>");
                return builder.ToString();
            }
            #endregion
        }

        /// <summary>
        /// Class for querying over an existing hitlist mode.
        /// </summary>
        [Serializable]
        [XmlType("domainCriteria")]
        public class DomainCriteria : ISearchCriteriaBase
        {
            #region Properties

            /// <summary>
            /// The value to compare to.
            /// </summary>
            [XmlText()]
            public string InnerText
            {
                get { return innerText; }
                set { innerText = value; }
            }

            public override string Value
            {
                get
                {
                    return InnerText;
                }
                set
                {
                    InnerText = value;
                }
            }
            #endregion

            #region Variables
            private string innerText;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes the domain criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public DomainCriteria(XmlNode node)
            {

                innerText = node.InnerXml;
            }

            /// <summary>
            /// Initializes its members to its default values.
            /// <para>Default values are:</para>
            /// <para>innerText = string.empty</para>
            /// </summary>
            public DomainCriteria()
            {

                this.innerText = string.Empty;
            }
            #endregion

            #region ISearchCriteriaBase Members
            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                StringBuilder builder = new StringBuilder("<domainCriteria");
                builder.Append(">");
                builder.Append(innerText);
                builder.Append("</domainCriteria>");
                return builder.ToString();
            }
            #endregion
        }

        /// <summary>
        /// Class for querying using literal mode. Very raw way of building searches, and should only be used when no other criteria fits your needs.
        /// </summary>
        [Serializable]
        [XmlType("verbatimCriteria")]
        public class VerbatimCriteria : ISearchCriteriaBase
        {
            #region Properties
            /// <summary>
            /// The sentence to execute.
            /// </summary>
            [XmlElement("verbatim")]
            public string Verbatim
            {
                get { return System.Web.HttpUtility.HtmlDecode(verbatim); }
                set { verbatim = System.Web.HttpUtility.HtmlEncode(value); }
            }

            /// <summary>
            /// The list of parameters.
            /// </summary>
            [XmlElement("parameter", ElementName = "parameter")]
            public List<string> Parameter
            {
                get { return parameters; }
                set { parameters = value; }
            }

            public override string Value
            {
                get
                {
                    return this.verbatim;
                }
                set
                {
                    this.verbatim = value;
                }
            }
            #endregion

            #region Variables
            private string verbatim;
            private string _value;
            private List<string> parameters;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes the verbatim criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public VerbatimCriteria(XmlNode node)
                : this()
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    switch (childNode.Name.ToLower())
                    {
                        case "verbatim":
                            verbatim = childNode.InnerText;
                            break;
                        case "parameter":
                            parameters.Add(childNode.InnerText);
                            break;
                    }
                }
            }

            /// <summary>
            /// Initializes its members to its default values.
            /// <para>Default value is:</para>
            /// <para>verbatim = Empty string.</para>
            /// <para>parameters = Empty List string.</para>
            /// </summary>
            public VerbatimCriteria()
            {
                this.verbatim = string.Empty;
                this.parameters = new List<string>();
            }
            #endregion

            #region ISearchCriteriaBase Members
            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                StringBuilder builder = new StringBuilder("<verbatimCriteria>");
                builder.Append("<verbatim>");
                builder.Append(verbatim.ToString());
                builder.Append("</verbatim>");
                foreach (string param in parameters)
                {
                    builder.Append("<parameter>");
                    builder.Append(param.ToString());
                    builder.Append("</parameter>");
                }
                builder.Append("</verbatimCriteria>");
                return builder.ToString();
            }
            #endregion
        }

        /// <summary>
        /// Class for querying in custom manner.
        /// </summary>
        [Serializable]
        [XmlType("customCriteria")]
        public class CustomCriteria : ISearchCriteriaBase, ISerializable
        {
            #region Properties
            /// <summary>
            /// The --third party-- custom criteria.
            /// </summary>
            [XmlAnyElement(Namespace = "COE.SearchCriteria")]
            public XmlNode Custom
            {
                get { return _custom; }
                set { _custom = value; }
            }

            public override string Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                }
            }
            #endregion

            #region Variables
            private XmlNode _custom;
            private string _value;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes the custom criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public CustomCriteria(XmlNode node)
                : this()
            {
                this.Value = node.Attributes["Value"] == null ? string.Empty : node.Attributes["Value"].Value;
                _custom = node.FirstChild;
            }

            /// <summary>
            /// Initializes its members to its default values.
            /// <para>Default value is a null xmlNode:</para>
            /// </summary>
            public CustomCriteria()
            {
                XmlDocument doc = new XmlDocument();
                _custom = doc.CreateNode(XmlNodeType.Element, "customCriteria", "COE.SearchCriteria");
            }
            #endregion

            #region ISearchCriteriaBase Members
            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                StringBuilder builder = new StringBuilder("<customCriteria");
                if (!string.IsNullOrEmpty(Value))
                    builder.Append(" Value=\"" + Value + "\"");
                builder.Append(">" + _custom.OuterXml);
                builder.Append("</customCriteria>");

                return builder.ToString();
            }
            #endregion

            #region ISerializable Members
            protected CustomCriteria(SerializationInfo info, StreamingContext ctx)
            {
                _value = info.GetString("value");
                string xmlOuterXml = info.GetString("custom");
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlOuterXml);
                _custom = doc.FirstChild;
                this.Negate = (COEBoolean)info.GetValue("negate", typeof(COEBoolean));
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("custom", this.Custom.OuterXml);
                info.AddValue("negate", this.Negate, typeof(COEBoolean));
                info.AddValue("value", this.Value);
            }

            #endregion
        }

        /// <summary>
        /// Class for joining two dataviews based on a hitlist made in the original dv.
        /// </summary>
        [Serializable]
        [XmlType("hitlistCriteria")]
        public class HitlistCriteria : ISearchCriteriaBase
        {
            #region Properties
            /// <summary>
            /// The field id in the original dataview.
            /// </summary>
            [XmlAttribute("sourceFieldId")]
            public int SourceFieldId
            {
                get { return _sourceFieldId; }
                set { _sourceFieldId = value; }
            }

            /// <summary>
            /// The hitlist type of the original search.
            /// </summary>
            [XmlAttribute("hitlistType")]
            public HitListType HitlistType
            {
                get { return _hitlistType; }
                set { _hitlistType = value; }
            }

            /// <summary>
            /// String representation of the sourcefieldid, is filled in by the processor.
            /// </summary>
            [XmlAttribute("sourceJoiningFieldStr")]
            public string SourceJoiningFieldStr
            {
                get { return System.Web.HttpUtility.HtmlDecode(_sourceJoiningFieldStr); }
                set { _sourceJoiningFieldStr = System.Web.HttpUtility.HtmlEncode(value); }
            }

            /// <summary>
            /// HitlistID to use in the search. This must've been generated using a search on the source dataview.
            /// </summary>
            [XmlText()]
            public override string Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                }
            }
            #endregion

            #region Variables
            private int _sourceFieldId;
            private string _value;
            private HitListType _hitlistType;
            private string _sourceJoiningFieldStr;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes the HitlistCriteria criteria from its xml representation.
            /// </summary>
            /// <param name="node">Its xml representation.</param>
            public HitlistCriteria(XmlNode node)
                : this()
            {
                if (node.Attributes["sourceFieldId"] != null && !string.IsNullOrEmpty(node.Attributes["sourceFieldId"].Value))
                {
                    _sourceFieldId = int.Parse(node.Attributes["sourceFieldId"].Value);
                }
                if (node.Attributes["sourceJoiningFieldStr"] != null && !string.IsNullOrEmpty(node.Attributes["sourceJoiningFieldStr"].Value))
                {
                    this.SourceJoiningFieldStr = node.Attributes["sourceJoiningFieldStr"].Value;
                }
                if (node.Attributes["hitlistType"] != null && !string.IsNullOrEmpty(node.Attributes["hitlistType"].Value))
                {
                    _hitlistType = (HitListType)Enum.Parse(typeof(HitListType), node.Attributes["hitlistType"].Value);
                }
                _value = node.InnerText;
            }

            /// <summary>
            /// Initializes its members to its default values.
            /// </summary>
            public HitlistCriteria()
            {
                _sourceFieldId = -1;
                _value = string.Empty;
            }
            #endregion

            #region ISearchCriteriaBase Members
            /// <summary>
            /// Builds its xml string representation.
            /// </summary>
            /// <returns>Its xml string representation.</returns>
            public override string GenerateXmlSnippet()
            {
                StringBuilder builder = new StringBuilder("<hitlistCriteria ");
                builder.Append("sourceFieldId=\"");
                builder.Append(_sourceFieldId);
                builder.Append("\" ");

                builder.Append("sourceJoiningFieldStr=\"");
                builder.Append(_sourceJoiningFieldStr);
                builder.Append("\" ");

                builder.Append("hitlistType=\"");
                builder.Append(_hitlistType.ToString());
                builder.Append("\">");
                builder.Append(_value);
                builder.Append("</hitlistCriteria>");
                return builder.ToString();
            }
            #endregion
        }
        #endregion

        #region Enums
        /// <summary>
        ///  List of Supported Comparison Operators for using with Where Clause Items.
        /// </summary>
        public enum COEOperators
        {
            /// <summary>
            /// Greater than.
            /// </summary>
            [XmlEnum("GT")]
            GT,
            /// <summary>
            /// Greater or equal than.
            /// </summary>
            [XmlEnum("GTE")]
            GTE,
            /// <summary>
            /// Lower than.
            /// </summary>
            [XmlEnum("LT")]
            LT,
            /// <summary>
            /// Lower or equal than.
            /// </summary>
            [XmlEnum("LTE")]
            LTE,
            /// <summary>
            /// Equal than.
            /// </summary>
            [XmlEnum("EQUAL")]
            EQUAL,
            /// <summary>
            /// Not equal than.
            /// </summary>
            [XmlEnum("NOTEQUAL")]
            NOTEQUAL,
            /// <summary>
            /// Like.
            /// </summary>
            [XmlEnum("LIKE")]
            LIKE,
            /// <summary>
            /// Not Like.
            /// </summary>
            [XmlEnum("NOTLIKE")]
            NOTLIKE,
            /// <summary>
            /// In.
            /// </summary>
            [XmlEnum("IN")]
            IN,
            /// <summary>
            /// In.
            /// </summary>
            [XmlEnum("NOTIN")]
            NOTIN,

            /// <summary>
            /// Contains.
            /// </summary>
            [XmlEnum("CONTAINS")]
            CONTAINS,
            /// <summary>
            /// Not Contains.
            /// </summary>
            [XmlEnum("NOTCONTAINS")]
            NOTCONTAINS,
            /// <summary>
            /// StartsWith.
            /// </summary>
            [XmlEnum("STARTSWITH")]
            STARTSWITH,
            /// <summary>
            /// EndWith.
            /// </summary>
            [XmlEnum("ENDWITH")]
            ENDWITH,
            /// <summary>
            /// NONE.
            /// </summary>
            [XmlEnum("NONE")]
            NONE
        }

        /// <summary>
        /// COE Boolean class using yes-no instead of true-false.
        /// </summary>
        public enum COEBoolean
        {
            /// <summary>
            /// Yes.
            /// </summary>
            [XmlEnum("YES")]
            Yes = 1,
            /// <summary>
            /// No.
            /// </summary>
            [XmlEnum("NO")]
            No = 0
        }

        /// <summary>
        /// Allowed values for tetrahedral stereo matching when searching:
        /// <list type="bullet">
        ///   <item>Yes</item>
        ///   <item>No</item>
        ///   <item>Same</item>
        ///   <item>Either</item>
        ///   <item>Any</item>
        /// </list>
        /// </summary>
        public enum TetrahedralStereoMatching
        {
            /// <summary>
            /// Yes.
            /// </summary>
            [XmlEnum("YES")]
            Yes,
            /// <summary>
            /// No.
            /// </summary>
            [XmlEnum("NO")]
            No,
            /// <summary>
            /// Same.
            /// </summary>
            [XmlEnum("SAME")]
            Same,
            /// <summary>
            /// Either.
            /// </summary>
            [XmlEnum("EITHER")]
            Either,
            /// <summary>
            /// Any.
            /// </summary>
            [XmlEnum("ANY")]
            Any
        }

        /// <summary>
        /// Used in commands like WhereClauseLike to determine where to apply the wildcards. 
        /// Also specifies trim side.
        /// </summary>
        public enum Positions
        {
            /// <summary>
            /// Wildcard will be applied at the left side.
            /// </summary>
            [XmlEnum("LEFT")]
            Left = 0,
            /// <summary>
            /// Wildcard will be applied at the right side.
            /// </summary>
            [XmlEnum("RIGHT")]
            Right = 1,
            /// <summary>
            /// Wildcard will be applied both sides (left AND right).
            /// </summary>
            [XmlEnum("BOTH")]
            Both = 2,
            /// <summary>
            /// There will be no wildcard.
            /// </summary>
            [XmlEnum("NONE")]
            None = 3
        }

        /// <summary>
        /// </summary>
        public enum COELogicalOperators
        {
            /// <summary>
            /// </summary>
            [XmlEnum("AND")]
            And = 0,
            /// <summary>
            /// </summary>
            [XmlEnum("OR")]
            Or = 1
        }

        /// <summary>
        /// Structure search type.
        /// </summary>
        public enum JChemStructureSearchType
        {
            /// <summary>
            /// Full structure search.
            /// </summary>
            [XmlEnum("f")]
            Full,
            /// <summary>
            /// Sub structure search.
            /// </summary>
            [XmlEnum("s")]
            SubStructure,
            /// <summary>
            /// Similar search.
            /// </summary>
            [XmlEnum("t")]
            Similar
        }

        #endregion

        /// <summary>
        /// Clone a new object.
        /// </summary>
        /// <returns>return an object</returns>
        public object Clone()
        {
            SearchCriteria criteria = new SearchCriteria();
            criteria.GetFromXML(this.ToString());

            return criteria;
        }
    }
}