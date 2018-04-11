using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CambridgeSoft.COE.Framework.Common {
    /// <summary>
    /// <para>
    /// Class that tell the framework how to page the results.
    /// </para>
    /// <para>
    /// Start cannot be less than 1, if so it would be overridden with that value. RecordCount and End values are redundant, you may choose  any one of those, but RecordCount is honored if used.
    /// </para>
    /// <para>
    /// RecordCount must be greater than 1 if used, and End must be greater than start. If none is provided a RecordCount of 1 is used.
    /// </para>
    /// <para>
    /// Said in other words, you will always specify to get at least 1 record when using this object and thus the only way of getting 0 results is because there is no matching data in the database.
    /// </para>
    /// </summary>
    [XmlRoot(ElementName = "PagingInfo", IsNullable = false, Namespace = "COE.PagingInfo"), Serializable]
    public class PagingInfo {

        #region Variables
        private int pagingInfoID;
        private int hitListID;
        private HitListType _hitListType;
        private HitListQueryType _hitListQueryType;
        private KeepAliveModes keepAlive;
        private int recordCount;
        private int start;
        private int end;
        private string transactionId;
        private bool _filterChildData;
        private bool _highlightSubStructures;
        private const string xmlNS = "COE.PagingInfo";
        #endregion

        #region Properties
        /// <summary>
        /// Unique identifier of the PagingInfo.
        /// </summary>
        [XmlElement(ElementName = "PagingInfoID", DataType = "int")]
        public int PagingInfoID {
            get { return this.pagingInfoID; }
            set { this.pagingInfoID = value; }
        }

        /// <summary>
        /// Gets or sets the hitlist that is to be paged.
        /// </summary>
        [XmlElement(ElementName = "HitListID", DataType = "int")]
        public int HitListID {
            get { return this.hitListID; }
            set { this.hitListID = value; }
        }

        /// <summary>
        /// Type of hitlist
        /// </summary>
        [XmlElement("HitListType", typeof(HitListType))]
        public HitListType HitListType {
            get { return _hitListType; }
            set { _hitListType = value; }
        }

        /// <summary>
        /// Type of query hitlist
        /// </summary>
        [XmlElement("HitListQueryType", typeof(HitListQueryType))]
        public HitListQueryType HitListQueryType
        {
            get { return _hitListQueryType; }
            set { _hitListQueryType = value; }
        }

        /// <summary>
        /// Determines if results must be kept alive and how. Allowed values are:
        /// <para>NONE: The results won't be maintained alive at all.</para>
        /// <para>TRANSIENT: The results will be maintained as long as the same search manager is maintained.</para>
        /// <para>PERSISTENT: The results will be maintained no matter what.</para>
        /// </summary>
        [XmlElement(ElementName = "KeepAlive")]
        public KeepAliveModes KeepAlive {
            get { return this.keepAlive; }
            set { this.keepAlive = value; }
        }

        /// <summary>
        /// Gets or sets the amount of records expected per page. The record count takes precedence over End property.
        /// </summary>
        [XmlElement(ElementName = "RecordCount", DataType = "int")]
        public int RecordCount {
            get { return this.recordCount; }
            set { this.recordCount = value; }
        }

        /// <summary>
        /// Gets or sets the starting position of the page. If this is not set -or set to be less than 1-, it is supposed to be 1.
        /// </summary>
        [XmlElement(ElementName = "Start", DataType = "int")]
        public int Start {
            get { return this.start; }
            set { this.start = value; }
        }

        /// <summary>
        /// Gets or sets the position of the last record in page. RecordCount takes precedence over End. If none is set, then the recordcount
        /// is set to 1.
        /// </summary>
        [XmlElement(ElementName = "End", DataType = "int")]
        public int End {
            get { return this.end; }
            set { this.end = value; }
        }

        /// <summary>
        /// Unique identifier of a transaction.
        /// </summary>
        [XmlElement(ElementName = "TransactionId", DataType = "string")]
        public string TransactionId {
            get { return this.transactionId; }
            set { this.transactionId = value; }
        }

        /// <summary>
        /// Indicates if Child Data must be filtered with search criteria.
        /// </summary>
        [XmlElement(ElementName = "FilterChildData", DataType = "boolean")]
        public bool FilterChildData
        {
            get { return _filterChildData; }
            set { _filterChildData = value; }
        }

        /// <summary>
        /// Indicates if sub structures should be highlighted. This is a best effort approach, under some circumstances it would still not be possible. (IE. cartrige version less than 13)
        /// </summary>
        [XmlElement(ElementName = "HighlightSubStructures", DataType = "boolean")]
        public bool HighlightSubStructures
        {
            get { return _highlightSubStructures; }
            set { _highlightSubStructures = value; }
        }

        public bool AddToRecent { get; set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public PagingInfo() {
            this.end = 0;
            this.hitListID = 0;
            _hitListType = HitListType.TEMP;
            this.keepAlive = KeepAliveModes.NONE;
            this.pagingInfoID = 0;
            this.recordCount = 1;
            this.start = 1;
            this.transactionId = string.Empty;
            _filterChildData = false;
            _highlightSubStructures = false;
        }

        /// <summary>
        /// Initializes its members from its xml representation.
        /// </summary>
        /// <param name="doc">The xml representation.</param>
        public PagingInfo(XmlDocument doc) {
            this.GetFromXML(doc);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes its members from its xml representation.
        /// </summary>
        /// <param name="xmlRequest">The xml representation.</param>
        public void GetFromXML(XmlDocument xmlRequest) {
            foreach(XmlNode currentNode in xmlRequest.DocumentElement.ChildNodes) {
                if(currentNode.NodeType == XmlNodeType.Element) {
                    switch(currentNode.Name.Trim().ToLower()) {
                        case "end":
                            if(currentNode.InnerText.ToString().Trim() != string.Empty)
                                this.end = int.Parse(currentNode.InnerText);
                            break;
                        case "start":
                            if(currentNode.InnerText.ToString().Trim() != string.Empty)
                                this.start = int.Parse(currentNode.InnerText);
                            break;
                        case "recordcount":
                            if(currentNode.InnerText.ToString().Trim() != string.Empty)
                                this.recordCount = int.Parse(currentNode.InnerText);
                            break;
                        case "keepalive":
                            if(currentNode.InnerText.ToString().Trim() != string.Empty)
                                this.keepAlive = GetKeepAliveModeFromString(currentNode.InnerText);
                            break;
                        case "hitlistid":
                            if(currentNode.InnerText.ToString().Trim() != string.Empty)
                                this.hitListID = int.Parse(currentNode.InnerText);
                            break;
                        case "hitlisttype":
                            if(currentNode.InnerText.ToString().Trim() != string.Empty)
                                this.HitListType = (HitListType)Enum.Parse(typeof(HitListType), currentNode.InnerText);
                            break;
                        case "paginginfoid":
                            if(currentNode.InnerText.ToString().Trim() != string.Empty)
                                this.pagingInfoID = int.Parse(currentNode.InnerText);
                            break;
                        case "transactionid":
                            if(currentNode.InnerText.ToString().Trim() != string.Empty)
                                this.transactionId = currentNode.InnerText;
                            break;
                        case "filterchilddata":
                            if (currentNode.InnerText.ToString().Trim() != string.Empty)
                                _filterChildData = bool.Parse(currentNode.InnerText);
                            break;
                        case "highlightsubstructures":
                            if (currentNode.InnerText.ToString().Trim() != string.Empty)
                                _highlightSubStructures = bool.Parse(currentNode.InnerText);
                            break;
                    }
                }
            }

        }

        /// <summary>
        /// Initializes its members from its xml representation.
        /// </summary>
        /// <param name="xmlRequest">The xml representation as string.</param>
        public void GetFromXML(string xmlRequest) {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlRequest);
            this.GetFromXML(doc);
        }

        /// <summary>
        /// Builds its xml representation as string .
        /// </summary>
        /// <returns>The xml representation as string.</returns>
        public override string ToString() {
            StringBuilder builder = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            builder.Append("<PagingInfo  xmlns=\"" + xmlNS + "\">");
            builder.Append("<PagingInfoID>");
            builder.Append(this.pagingInfoID);
            builder.Append("</PagingInfoID>");
            builder.Append("<HitListID>");
            builder.Append(this.hitListID);
            builder.Append("</HitListID>");
            builder.Append("<HitListType>");
            builder.Append(_hitListType.ToString());
            builder.Append("</HitListType>");
            builder.Append("<KeepAlive>");
            builder.Append(this.keepAlive.ToString());
            builder.Append("</KeepAlive>");
            builder.Append("<RecordCount>");
            builder.Append(this.recordCount);
            builder.Append("</RecordCount>");
            builder.Append("<Start>");
            builder.Append(this.start);
            builder.Append("</Start>");
            builder.Append("<End>");
            builder.Append(this.end);
            builder.Append("</End>");
            if(this.transactionId != string.Empty) {
                builder.Append("<TransactionId>");
                builder.Append(this.transactionId);
                builder.Append("</TransactionId>");
            }
            builder.Append("<FilterChildData>");
            builder.Append((_filterChildData ? "true" : "false")); //xml's boolean is represented by lower case strings as opposite to Pascal case used by C#
            builder.Append("</FilterChildData>");
            builder.Append("<HighlightSubStructures>");
            builder.Append((_highlightSubStructures ? "true" : "false")); //xml's boolean is represented by lower case strings as opposite to Pascal case used by C#
            builder.Append("</HighlightSubStructures>");
            builder.Append("</PagingInfo>");

            return builder.ToString();
        }
        #endregion

        #region Private Methods
        private KeepAliveModes GetKeepAliveModeFromString(string keepAliveString) {
            switch(keepAliveString) {
                case "TRANSIENT":
                    return KeepAliveModes.TRANSIENT;
                case "PERSISTENT":
                    return KeepAliveModes.PERSISTENT;
                default:
                    return KeepAliveModes.NONE;
            }
        }
        #endregion
    }

    #region Enums
    /// <summary>
    /// Keep alive modes determines how results shall be maintained in server.
    /// </summary>
    public enum KeepAliveModes {
        /// <summary>
        /// No keep alive.
        /// </summary>
        [XmlEnum("NONE")]
        NONE,
        /// <summary>
        /// Keep alive per search manager.
        /// </summary>
        [XmlEnum("TRANSIENT")]
        TRANSIENT,
        /// <summary>
        /// Force keep alive.
        /// </summary>
        [XmlEnum("PERSISTENT")]
        PERSISTENT
    }
    #endregion
}
