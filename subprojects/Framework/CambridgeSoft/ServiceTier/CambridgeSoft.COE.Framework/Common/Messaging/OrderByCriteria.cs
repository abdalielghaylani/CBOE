using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Class that provides messaging capabilities to communicate some order by criteria.
    /// </summary>
    [Serializable]
    [XmlRoot("orderByCriteria", Namespace = "COE.OrderByCriteria")]
    [XmlType(TypeName = "orderByCriteria", IncludeInSchema = true, Namespace = "COE.OrderByCriteria")]
    public class OrderByCriteria
    {
        #region Variables
        private int _id;
        private List<OrderByCriteriaItem> _orderByCriteriaItem;
        private string _xmlNS;
        [NonSerialized]
        private XmlNamespaceManager _manager;
        private string _xmlNamespace;
        #endregion

        #region Properties
        /// <summary>
        /// Unique identifier of the "Order by criteria".
        /// </summary>
        [XmlElement("orderByCriteriaID")]
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// The list of ordering criterium.
        /// </summary>
        [XmlElement("orderByCriteriaItem")]
        public List<OrderByCriteriaItem> Items
        {
            get { return _orderByCriteriaItem; }
            set { _orderByCriteriaItem = value; }
        }

        /// <summary>
        /// Xml Namespace.
        /// </summary>
        [XmlIgnore]
        public string XmlNS
        {
            get { return _xmlNS; }
            set { _xmlNS = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public OrderByCriteria()
        {
            _orderByCriteriaItem = new List<OrderByCriteriaItem>();
            _xmlNS = "COE.OrderByCriteria";
            _xmlNamespace = "COE";
            _manager = new XmlNamespaceManager(new NameTable());
            _manager.AddNamespace(_xmlNamespace, _xmlNS);
        }

        /// <summary>
        /// Initializes the messaging type from its xml representation.
        /// </summary>
        /// <param name="doc">Its xml representation.</param>
        public OrderByCriteria(XmlDocument doc)
        {
            _xmlNS = "COE.OrderByCriteria";
            _xmlNamespace = "COE";
            _manager = new XmlNamespaceManager(doc.NameTable);
            _manager.AddNamespace(_xmlNamespace, _xmlNS);
            this.GetFromXML(doc);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Load its members from its xml representation.
        /// </summary>
        /// <param name="xmlResultsCriteria">The xml representation.</param>
        public void GetFromXML(XmlDocument xmlOrderByCriteria)
        {
            XmlNodeList orderByCriteriaItemNodeList = xmlOrderByCriteria.SelectNodes("//" + _xmlNamespace + ":orderByCriteriaItem", _manager);

            if (orderByCriteriaItemNodeList != null)
            {
                _orderByCriteriaItem = new List<OrderByCriteriaItem>();
                foreach (XmlNode orderByItem in orderByCriteriaItemNodeList)
                {
                    if (orderByItem.NodeType == XmlNodeType.Element)
                    {
                        OrderByCriteriaItem newOrderByItem = new OrderByCriteriaItem(orderByItem);
                        _orderByCriteriaItem.Add(newOrderByItem);
                    }
                }

            }
            XmlNode orderByCriteriaNode = xmlOrderByCriteria.SelectSingleNode("//" + _xmlNamespace + ":orderByCriteria", _manager);
            //Coverity Fix CID 19195
            if (orderByCriteriaNode != null)
            {
                XmlAttribute xmlAtt = orderByCriteriaNode.Attributes["xmlns"];
                if (xmlAtt != null)
                {
                    string xmlnsValue = xmlAtt.Value;
                    if (!string.IsNullOrEmpty(xmlnsValue))
                    {
                        _xmlNS = xmlnsValue;
                    }
                }

                xmlAtt = orderByCriteriaNode.Attributes["orderByCriteriaID"];
                if (xmlAtt != null)
                {
                    string orderById = xmlAtt.Value;
                    if (!string.IsNullOrEmpty(orderById))
                    {
                        _id = int.Parse(orderById);
                    }
                }
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

        /// <summary>
        /// Builds its xml string representation.
        /// </summary>
        /// <returns>The xml string representation.</returns>
        public override string ToString()
        {
            return Utilities.XmlSerialize(this);
        }
        #endregion

        #region Additional Classes
        /// <summary>
        /// </summary>
        [Serializable]
        public class OrderByCriteriaItem
        {
            #region Variables
            private int _id;
            private OrderByDirection _direction;
            private int _orderIndex;
            private ResultsCriteria.IResultsCriteriaBase _resultCriteriaItem;
            private int _tableID;
            #endregion

            #region Properties
            [XmlAttribute("id")]
            public int ID
            {
                get { return _id; }
                set { _id = value; }
            }

            [XmlAttribute("direction")]
            public OrderByDirection Direction
            {
                get { return _direction; }
                set { _direction = value; }
            }

            [XmlAttribute("orderIndex")]
            public int OrderIndex
            {
                get { return _orderIndex; }
                set { _orderIndex = value; }
            }

            [XmlElement(typeof(ResultsCriteria.AggregateFunction))]
            [XmlElement(typeof(ResultsCriteria.Avg))]
            [XmlElement(typeof(ResultsCriteria.CDXToMolFile))]
            [XmlElement(typeof(ResultsCriteria.Concatenation))]
            [XmlElement(typeof(ResultsCriteria.Condition))]
            [XmlElement(typeof(ResultsCriteria.Field))]
            [XmlElement(typeof(ResultsCriteria.Formula))]
            [XmlElement(typeof(ResultsCriteria.DirectFormula))]
            [XmlElement(typeof(ResultsCriteria.JChemFormula))]
            [XmlElement(typeof(ResultsCriteria.HighlightedStructure))]
            [XmlElement(typeof(ResultsCriteria.Literal))]
            [XmlElement(typeof(ResultsCriteria.LowerCase))]
            [XmlElement(typeof(ResultsCriteria.Marked))]
            [XmlElement(typeof(ResultsCriteria.Max))]
            [XmlElement(typeof(ResultsCriteria.Min))]
            [XmlElement(typeof(ResultsCriteria.MolWeight))]
            [XmlElement(typeof(ResultsCriteria.DirectMolWeight))]
            [XmlElement(typeof(ResultsCriteria.JChemMolWeight))]
            [XmlElement(typeof(ResultsCriteria.Screen))]
            [XmlElement(typeof(ResultsCriteria.Similarity))]
            [XmlElement(typeof(ResultsCriteria.SQLFunction))]
            [XmlElement(typeof(ResultsCriteria.Switch))]
            [XmlElement(typeof(ResultsCriteria.ToDate))]
            [XmlElement(typeof(ResultsCriteria.DirectStructure))]
            [XmlElement(typeof(ResultsCriteria.CustomCriteria))]
            public ResultsCriteria.IResultsCriteriaBase ResultCriteriaItem
            {
                get { return _resultCriteriaItem; }
                set { _resultCriteriaItem = value; }
            }

            [XmlAttribute("tableId")]
            public int TableID
            {
                get { return _tableID; }
                set { _tableID = value; }
            }
            #endregion

            #region Constructors
            public OrderByCriteriaItem()
            {
                _id = 0;
                _orderIndex = 0;
                _resultCriteriaItem = new ResultsCriteria.Literal();
                _direction = OrderByDirection.ASC;
                _tableID = 0;
            }
            public OrderByCriteriaItem(XmlNode orderByItemXml)
            {
                if (orderByItemXml.Attributes["id"] != null && orderByItemXml.Attributes["id"].Value != string.Empty)
                    _id = int.Parse(orderByItemXml.Attributes["id"].Value);

                if (orderByItemXml.Attributes["direction"] != null && orderByItemXml.Attributes["direction"].Value != string.Empty)
                    _direction = (OrderByDirection)Enum.Parse(typeof(OrderByDirection), orderByItemXml.Attributes["direction"].Value);
                else
                    _direction = OrderByDirection.ASC;

                if (orderByItemXml.Attributes["orderIndex"] != null && orderByItemXml.Attributes["orderIndex"].Value != string.Empty)
                    _orderIndex = int.Parse(orderByItemXml.Attributes["orderIndex"].Value);

                foreach (XmlNode node in orderByItemXml.ChildNodes)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        _resultCriteriaItem = ResultsCriteria.ResultsCriteriaBuilder.BuildCriteria(node);
                        break;
                    }
                }

                if (orderByItemXml.Attributes["tableId"] != null && orderByItemXml.Attributes["tableId"].Value != string.Empty)
                    _tableID = int.Parse(orderByItemXml.Attributes["tableId"].Value);
            }
            #endregion
        }
        #endregion

        #region Enumerations
        /// <summary>
        /// List of supported order by direction. Allowable values are ASC and DESC
        /// </summary>
        public enum OrderByDirection
        {
            /// <summary>
            /// Ascending ordering.
            /// </summary>
            [XmlEnum("ASC")]
            ASC = 1,
            /// <summary>
            /// Descending ordering.
            /// </summary>
            [XmlEnum("DESC")]
            DESC = 2
        }
        #endregion
    }
}
