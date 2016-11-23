using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common {
    /// <summary>
    /// Hit List related information
    /// </summary>
    [XmlRoot(ElementName = "HitListInfo", IsNullable = true, Namespace = "COE.HitListInfo"), Serializable]
    public class HitListInfo {
        
        #region Variables
        private int _hitListID;
        private int _recordCount;
        private const string _xmlNS = "COE.HitListInfo";
        private string _database;
        private bool _isExactRowCount;
        private int _currentRecordCount;
        private HitListType _hitListType;
        #endregion

        #region Properties

        /// <summary>
        /// Type of hitlist
        /// </summary>
        [XmlElement("HitListType", typeof(HitListType))]
        public HitListType HitListType
        {
            get { return this._hitListType; }
            set { this._hitListType = value; }
        }

        /// <summary>
        /// Unique identifier of a hitlist.
        /// </summary>
        [XmlElement(ElementName = "HitListID", DataType = "int")]
        public int HitListID {
            get { return this._hitListID; }
            set { this._hitListID = value; }
        }

        /// <summary>
        /// Quantity of records in the hitlist.
        /// </summary>
        [XmlElement(ElementName = "RecordCount", DataType = "int")]
        public int RecordCount {
            get { return this._recordCount; }
            set { this._recordCount = value; }
        }


        /// <summary>
        /// datbase that hitlist refers to
        /// </summary>
        [XmlElement(ElementName = "Database", DataType = "string")]
        public string Database
        {
            get { return this._database; }
            set { this._database = value; }
        }

        /// <summary>
        /// boolean indicated if record count is exact or approximate
        /// </summary>
        [XmlElement(ElementName = "IsExactRecordCount", DataType = "boolean")]
        public bool IsExactRecordCount
        {
            get { return this._isExactRowCount; }
            set { this._isExactRowCount = value; }
        }

        /// <summary>
        /// boolean indicated if record count is exact or approximate
        /// </summary>
        [XmlElement(ElementName = "CurrentRecordCount", DataType = "int")]
        public int CurrentRecordCount {
            get { return this._currentRecordCount; }
            set { this._currentRecordCount = value; }
        }
        #endregion

        #region Contructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public HitListInfo() {
            this._hitListID = this._currentRecordCount = this._recordCount = 0;
            this._database = string.Empty;
            this._isExactRowCount = true;
        }

        /// <summary>
        /// Initializes the messaging type from its xml representation.
        /// </summary>
        /// <param name="hitlistXml">Its xml representation.</param>
        public HitListInfo(XmlDocument hitlistXml) {
            this.GetFromXml(hitlistXml);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Loads itself from an xml representation of the object.
        /// </summary>
        /// <param name="hitlistXml">The xml representation.</param>
        public void GetFromXml(XmlDocument hitlistXml) {
            foreach(XmlNode node in hitlistXml.DocumentElement.ChildNodes) {
                if(node.NodeType == XmlNodeType.Element) {
                    switch(node.Name.ToLower()) {
                        case "hitlistid":
                            int.TryParse(node.InnerText, out this._hitListID);
                            break;
                        case "recordcount":
                            int.TryParse(node.InnerText, out this._recordCount);
                            break;
                        case "database":
                            this._database = node.InnerText;
                            break;
                        case "isexactrecordcount":
                            if(node.InnerText.ToUpper() == "TRUE") {
                                this._isExactRowCount = true;
                            } else {
                                this._isExactRowCount = false;
                            }
                            break;
                        case "currentrecordcount":
                            int.TryParse(node.InnerText, out this._currentRecordCount);
                            break;

                    }
                }
            }
        }

        /// <summary>
        /// Fills the HitListInfo object from it's xml representation contained in a string.
        /// </summary>
        /// <param name="hitlistXml">The string containing the HitListInfo xml representation</param>
        public void GetFromXml(string hitlistXml) {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(hitlistXml);
            this.GetFromXml(doc);
        }

        /// <summary>
        /// Builds its xml representation as string .
        /// </summary>
        /// <returns>The xml representation as string.</returns>
        public override string ToString() {
            StringBuilder builder = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            builder.Append("<HitListInfo  xmlns=\"" + _xmlNS + "\">");
            builder.Append("<HitListID>");
            builder.Append(this._hitListID);
            builder.Append("</HitListID>");
            builder.Append("<RecordCount>");
            builder.Append(this._recordCount);
            builder.Append("</RecordCount>");
            builder.Append("<Database>");
            builder.Append(this._database);
            builder.Append("</Database>");
            builder.Append("<IsExactRecordCount>");
            builder.Append(this._isExactRowCount);
            builder.Append("</IsExactRecordCount>");
            builder.Append("<CurrentRecordCount>");
            builder.Append(this._currentRecordCount);
            builder.Append("</CurrentRecordCount>");
            builder.Append("</HitListInfo>");

            return builder.ToString();
        }
        #endregion
    }
}
