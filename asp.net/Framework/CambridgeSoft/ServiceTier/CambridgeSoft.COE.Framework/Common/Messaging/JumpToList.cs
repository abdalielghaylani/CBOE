using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Framework.Common
{
    /// JumpToList
    /// <summary>
    /// Write something here.
    /// </summary>

    [Serializable]
    public class JumpToList : List<JumpTo>
    {
        #region Variables

        #endregion

        #region Constructor

        public JumpToList()
        {

        }

        public JumpToList(XmlDocument jumpToListXML)
        {
            this.GetFromXml(jumpToListXML);
        }
        #endregion

        #region Methods



        /// <summary>
        /// Fills the DataView object from it's xml representation contained in an XmlDocument.
        /// </summary>
        /// <param name="xmlDataView">xml representation of this DataView object</param>
        private void GetFromXml(XmlDocument jumpToListXML)
        {            
            foreach (XmlNode node in jumpToListXML.DocumentElement.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    if (node.Name.ToLower() == "jumpto")
                    {
                        JumpTo jumpTo = new JumpTo();
                        foreach (XmlAttribute atr in node.Attributes)
                        {
                            switch (atr.Name.ToLower())
                            {
                                case "sourcedataviewid":
                                    jumpTo.SourceDataviewId = int.Parse(atr.Value);
                                    break;
                                case "sourcedataviewname":
                                    jumpTo.SourceDataviewName = atr.Value;
                                    break;
                                case "sourcefieldname":
                                    jumpTo.SourceFieldAlias = atr.Value;
                                    break;
                                case "sourcefieldid":
                                    jumpTo.SourceFieldId = int.Parse(atr.Value);
                                    break;
                                case "sourcetablename":
                                    jumpTo.SourceTablealias = atr.Value;
                                    break;
                                case "sourcetableid":
                                    jumpTo.SourceTableId = int.Parse(atr.Value);
                                    break;
                                case "targetdataviewid":
                                    jumpTo.TargetDataviewId = int.Parse(atr.Value);
                                    break;
                                case "targetdataviewname":
                                    jumpTo.TargetDataviewName = atr.Value;
                                    break;
                                case "targetfieldname":
                                    jumpTo.TargetFieldAlias = atr.Value;
                                    break;
                                case "targetfieldid":
                                    jumpTo.TargetFieldId = int.Parse(atr.Value);
                                    break;
                                case "targettablename":
                                    jumpTo.TargetTableAlias = atr.Value;
                                    break;
                                case "targettableid":
                                    jumpTo.TargetTableId = int.Parse(atr.Value);
                                    break;
                                case "targetformid":
                                    int.TryParse(atr.Value, out jumpTo.FormId);
                                    break;
                                case "targetformname":
                                    jumpTo.FormName = atr.Value;
                                    break;
                                case "targetformtype":
                                    switch (atr.Value.ToLower())
                                    {
                                        case "webform":
                                            jumpTo.FormType = JumpTo.FormTypes.Web;
                                            break;
                                        case "richform":
                                            jumpTo.FormType = JumpTo.FormTypes.Rich;
                                            break;
                                        default:
                                            jumpTo.FormType = JumpTo.FormTypes.NONE;
                                            break;
                                    }
                                    
                                    break;
                            }
                        }
                        this.Add(jumpTo);
                    }
                }
            }
        }

        /// <summary>
        /// Fills the DataView object from it's xml representation contained in a string.
        /// </summary>
        /// <param name="xmlDataView">the string containing the dataview xml representation</param>
        public void GetFromXML(string xmlDataView)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlDataView);
            this.GetFromXml(doc);
        }

        /// <summary>
        /// Converts the current JumpToList object to it's xml representation and returns it into a string
        /// </summary>
        /// <returns>the resulting string containing the xml representation of the JumpToList</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            builder.Append("<JumpToList>");
            foreach (JumpTo jumpTo in this)
            {
                builder.Append("!--" + jumpTo.FormType + "--");
                builder.Append("<JumpTo  sourceDataviewId=\"" + jumpTo.SourceDataviewId + "\" sourceDataviewName=\"" + jumpTo.SourceDataviewName + "\" sourceTableId=\"" + jumpTo.SourceTableId + "\" sourceTableName=\"" + jumpTo.SourceTablealias + "\" sourceFieldId=\"" + jumpTo.SourceFieldId + "\" sourceFieldName=\"" + jumpTo.SourceFieldAlias + "\" tagetDataviewId=\"" + jumpTo.TargetDataviewId + "\" tagetDataviewName=\"" + jumpTo.TargetDataviewName + "\" targetTableId=\"" + jumpTo.TargetTableId + "\" targetTableName=\"" + jumpTo.TargetTableAlias + "\" targetFieldId=\"" + jumpTo.TargetFieldId + "\" targetFieldName=\"" + jumpTo.TargetFieldAlias + "\">");

                builder.Append("</JumpTo>");
            }
            builder.Append("</JumpToList>");
            return builder.ToString();
        }

        /// <summary>
        /// returns the source dataview name
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        public string GetSourceDataviewNameById(int dvId)
        {
            string dvName = String.Empty;

            foreach (JumpTo j in this)
            {
                if (j.SourceDataviewId == dvId)
                {
                    dvName = j.SourceDataviewName;
                    break;
                }
            }

            return dvName;
        }

        /// <summary>
        /// returns the taget dataview name
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        public string GetTargetDataviewNameById(int dvId)
        {
            string dvName = String.Empty;

            foreach (JumpTo j in this)
            {
                if (j.TargetDataviewId == dvId)
                {
                    dvName = j.TargetDataviewName;
                    break;
                }
            }

            return dvName;
        }

        /// <summary>
        /// returns the source tableName from a tableID
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        public string GetSourceTableNameById(int tableID)
        {
            string tableName = String.Empty;

            foreach (JumpTo j in this)
            {
                if (j.SourceTableId == tableID)
                {
                    tableName = j.SourceTablealias;
                    break;
                }
            }

            return tableName;
        }

        /// <summary>
        /// returns the target tableName from a tableID
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        public string GetTargetTableNameById(int tableID)
        {
            string tableName = String.Empty;

            foreach (JumpTo j in this)
            {
                if (j.TargetTableId == tableID)
                {
                    tableName = j.TargetTableAlias;
                    break;
                }
            }

            return tableName;
        }

        /// <summary>
        /// returns the source field name from a field id
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        public string GetSourceFieldNameById(int fieldId)
        {
            string fieldName = String.Empty;

            foreach (JumpTo j in this)
            {
                if (j.SourceFieldId == fieldId)
                {
                    fieldName = j.SourceFieldAlias;
                    break;
                }
            }

            return fieldName;
        }

        /// <summary>
        /// returns the target field name by field id
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        public string GetTargetFieldNameById(int fieldId)
        {
            string fieldName = String.Empty;

            foreach (JumpTo j in this)
            {
                if (j.TargetFieldId == fieldId)
                {
                    fieldName = j.TargetFieldAlias;
                    break;
                }
            }

            return fieldName;
        }

        /// <summary>
        /// returns the form name ny id
        /// </summary>
        /// <param name="formID"></param>
        /// <returns></returns>
        public string GetFormNameById(int formId)
        {
            string formname = String.Empty;

            foreach (JumpTo j in this)
            {
                if (j.FormId == formId)
                {
                    formname = j.FormName;
                    break;
                }
            }

            return formname;
        }

        /// <summary>
        /// returns the JumpToList of type WebFrom
        /// </summary>
        /// <returns>A JumpToList or null</returns>
        public JumpToList GetJumpToWebForms()
        {
            JumpToList jl = new JumpToList();;
            foreach (JumpTo j in this)
            {
                if (j.FormType == JumpTo.FormTypes.Web)
                {
                    jl.Add(j);
                }
            }
            return jl;
        }

        /// <summary>
        /// returns the JumpToList of type RichForm
        /// </summary>
        /// <returns>A JumpToList or null</returns>
        public JumpToList GetJumpToRichForms()
        {
            JumpToList jl = new JumpToList();;
            foreach (JumpTo j in this)
            {
                if (j.FormType == JumpTo.FormTypes.Rich)
                {
                    jl.Add(j);
                }
            }
            return jl;
        }

        /// <summary>
        /// returns the JumpToList by source dataview id
        /// </summary>
        /// <returns>A JumpToList or null</returns>
        public JumpToList GetJumpToListBySourceDataviewId(int dataviewId)
        {
            JumpToList jl = new JumpToList();
            foreach (JumpTo j in this)
            {
                if (j.SourceDataviewId == dataviewId)
                {
                    jl.Add(j);
                }
            }
            return jl;
        }

        /// <summary>
        /// returns the JumpToList by target dataview id
        /// </summary>
        /// <returns>A JumpToList or null</returns>
        public JumpToList GetJumpToListByTargetDataviewId(int dataviewId)
        {
            JumpToList jl = new JumpToList();
            foreach (JumpTo j in this)
            {
                if (j.TargetDataviewId == dataviewId)
                {
                    jl.Add(j);
                }
            }
            return jl;
        }

        #endregion
    }
}
