using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace CambridgeSoft.COE.Framework.Common {
    /// <summary>
    /// Class that represents the response of a search.
    /// </summary>
    [XmlRoot(ElementName = "SearchResponse", IsNullable = false, Namespace = "COE.SearchResponse"), Serializable]
    public class SearchResponse {

        #region Variables
        private PagingInfo pagingInfo;
        private DataSet resultsDataSet;
        private HitListInfo hitListInfo;
        #endregion

        #region Properties
        /// <summary>
        /// Info about Hit List, like its id and its record count.
        /// </summary>
        [XmlElement(Type = typeof(HitListInfo), ElementName = "HitListInfo", IsNullable = true, Namespace = "COE.HitListInfo")]
        public HitListInfo HitListInfo {
            get { return this.hitListInfo; }
            set { this.hitListInfo = value; }
        }

        /// <summary>
        /// The returning dataset which contains the datatables as defined by ResultsCriteria.
        /// </summary>
        [XmlElement(Type = typeof(DataSet), ElementName = "ResultsDataSet", IsNullable = true)]
        public DataSet ResultsDataSet {
            get { return this.resultsDataSet; }
            set { this.resultsDataSet = value; }
        }

        /// <summary>
        /// Information about paging, including records per page, start and end and keep alive mode.
        /// </summary>
        [XmlElement(Type = typeof(PagingInfo), ElementName = "PagingInfo", IsNullable = true, Namespace = "COE.PagingInfo")]
        public PagingInfo PagingInfo {
            get { return this.pagingInfo; }
            set { this.pagingInfo = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its members from its xml representation.
        /// </summary>
        /// <param name="doc">The xml representation.</param>
        public SearchResponse(XmlDocument doc) {
            this.GetFromXml(doc);
        }

        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public SearchResponse() {
            this.hitListInfo = new HitListInfo();
            this.pagingInfo = new PagingInfo();
            this.resultsDataSet = new DataSet();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Loads itself from its xml representation.
        /// </summary>
        /// <param name="doc">The xml representation.</param>
        public void GetFromXml(XmlDocument doc) {
            foreach(XmlNode node in doc.DocumentElement.ChildNodes) {
                if(node.NodeType == XmlNodeType.Element) {
                    XmlDocument nodeAsDocument = new XmlDocument();
                    nodeAsDocument.AppendChild(nodeAsDocument.ImportNode(node, true));
                    switch(node.Name.ToLower()) {
                        case "hitlistinfo":
                            this.hitListInfo = new HitListInfo(nodeAsDocument);
                            break;
                        case "paginginfo":
                            this.pagingInfo = new PagingInfo(nodeAsDocument);
                            break;
                        default:
                            this.resultsDataSet = new DataSet();
                            this.resultsDataSet.ReadXml(new StringReader(nodeAsDocument.OuterXml));
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Loads itself from its xml representation as a string.
        /// </summary>
        /// <param name="xml">The xml representation as a string.</param>
        public void GetFromXml(string xml) {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            this.GetFromXml(doc);
        }
        #endregion

    }
}
