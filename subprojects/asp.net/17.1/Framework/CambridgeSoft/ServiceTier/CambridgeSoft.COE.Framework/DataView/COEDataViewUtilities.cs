using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections;

namespace CambridgeSoft.COE.Framework.COEDataViewService
{
    public static class COEDataViewUtilities
    {
        public static void BuildDataViewTableName(string owner, ref string tableName)
        {
            if (owner.Length > 0)
            {
                tableName = owner + "." + Resources.COEDataViewTableName;
            }
            else
            {
                tableName = Resources.COEDataViewTableName;
            }
        }



        public static COEDataView DeserializeCOEDataView(string coeDataViewString)
        {
            //to deserialize
            XmlSerializer deSerializer = new XmlSerializer(typeof(COEDataView), "COE.COEDataView");
            StringReader stringReader = new StringReader(coeDataViewString);
            COEDataView myCOEDataView = (COEDataView)deSerializer.Deserialize(stringReader);
            SetFieldParents(myCOEDataView);
            return myCOEDataView;
        }

        public static string SerializeCOEDataView(COEDataView coeDataView)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(COEDataView), "COE.COEDataView");
            StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, coeDataView);
            string serializedCOEDataView = stringWriter.ToString();
            return serializedCOEDataView;
        }

        /// <summary>
        /// Sets the parentTableId property for all fields in the dataview
        /// </summary>
        /// <param name="coeDataView"></param>
        /// TODO: could not figure out how to do this during deserialization
        /// so it is being done after the dataview has already been built
        /// it would be better to do it as each field is added to the dataview.
        private static void SetFieldParents(COEDataView coeDataView)
        {
            foreach (COEDataView.DataViewTable tbl in coeDataView.Tables)
            {
                foreach (COEDataView.Field fld in tbl.Fields)
                {
                    fld.ParentTableId = tbl.Id;
                }
            }
        }

        /// <summary>
        /// Sort CoeDataview tables and fields
        /// </summary>
        /// <param name="coeDataview"></param>
        public static string SortCoeDataview(string coeDataView)
        {
            try
            {
                XmlDocument dataviewXMLDoc = new XmlDocument();
                XmlDocument dataviewXmlDocSorted = new XmlDocument();
                dataviewXMLDoc.LoadXml(coeDataView);

                XmlElement root = dataviewXMLDoc.DocumentElement;
                dataviewXmlDocSorted.LoadXml(root.CloneNode(false).OuterXml);

                XmlNamespaceManager manager = new XmlNamespaceManager(dataviewXMLDoc.NameTable);
                manager.AddNamespace("coedv", "COE.COEDataView");

                //get the tables node
                XmlNode tablesNodeOld = root.SelectSingleNode("coedv:tables", manager);
                XmlNode tablesNodeNew = dataviewXmlDocSorted.ImportNode(tablesNodeOld, false);//copy tables node with out child nodes.
                dataviewXmlDocSorted.DocumentElement.AppendChild(tablesNodeNew);
                
                XPathNavigator nav = dataviewXMLDoc.CreateNavigator();
                XPathExpression exp = nav.Compile("//coedv:COEDataView/coedv:tables/coedv:table");
                exp.AddSort("@alias", XmlSortOrder.Ascending, XmlCaseOrder.None, "", XmlDataType.Text);
                exp.SetContext(manager);
                //sort the tables by table alias
                XPathNodeIterator nodeIterator = nav.Select(exp);
                Dictionary<string, string> tableDic = new Dictionary<string, string>();
                while (nodeIterator.MoveNext())
                {
                    if (nodeIterator.Current is IHasXmlNode)
                    {
                        XmlNode tableNode = ((IHasXmlNode)nodeIterator.Current).GetNode();
                        tableDic.Add(tableNode.Attributes["id"].Value, tableNode.Attributes["alias"].Value);
                        //sort current table fields
                        //necessary for crossing XmlDocument contexts
                        XmlNode tableNodeSorted = dataviewXmlDocSorted.ImportNode(SortFields(dataviewXMLDoc, tableNode), true);
                        tablesNodeNew.AppendChild(tableNodeSorted);
                    }
                }
                
                //get the relationships node
                XmlNode relationshipsNodeOld = root.SelectSingleNode("coedv:relationships", manager);
                SortedList<string, XmlNode> sortedRelationships = new SortedList<string, XmlNode>();
                // preparing a sorted list of relationship according to table alias name
                if (relationshipsNodeOld.HasChildNodes)
                {
                    int i = 0;
                    foreach (XmlNode relationship in relationshipsNodeOld.ChildNodes)
                    {                        
                        i++;
                        string childKey = relationship.Attributes["child"].Value;
                        if (tableDic.ContainsKey(childKey))
                        {
                            sortedRelationships.Add(tableDic[childKey]+ i.ToString(), relationship);
                        }
                    }
                }                
                
                // loop to add sorted relationships to Relationships node
                XmlNode relationshipsNodeNew = dataviewXmlDocSorted.ImportNode(relationshipsNodeOld, false);
                foreach (KeyValuePair<string, XmlNode> pair in sortedRelationships)
                {                    
                    XmlNode node = dataviewXmlDocSorted.ImportNode((XmlNode)pair.Value, true);
                    relationshipsNodeNew.AppendChild(node);
                }
                
                dataviewXmlDocSorted.DocumentElement.AppendChild(relationshipsNodeNew);                

                return dataviewXmlDocSorted.OuterXml;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Sort Fields of the current tableNode by field alias
        /// </summary>
        /// <param name="xmlDataviewResponse"></param>
        /// <param name="tableNode"></param>
        /// <returns></returns>
        private static XmlNode SortFields(XmlDocument xmlDataviewResponse, XmlNode tableNode)
        {
            XmlNamespaceManager manager = new XmlNamespaceManager(xmlDataviewResponse.NameTable);
            manager.AddNamespace("coedv", "COE.COEDataView");

            XPathNavigator nav = xmlDataviewResponse.CreateNavigator();
            XPathExpression isDefaultTrueExp = nav.Compile("//coedv:COEDataView/coedv:tables/coedv:table[@id='" + tableNode.Attributes["id"].Value + "']/coedv:fields[@isDefault='1' or @isDefault='true']");  //CBOE-1720 @isDefault='true'
            isDefaultTrueExp.AddSort("@alias", XmlSortOrder.Ascending, XmlCaseOrder.None, "", XmlDataType.Text);
            isDefaultTrueExp.SetContext(manager);
            //Sort the current table default fields by field alias
            XPathNodeIterator isDefaultTrueNodeIterator = nav.Select(isDefaultTrueExp);

            XPathExpression isDefaultFalseExp = nav.Compile("//coedv:COEDataView/coedv:tables/coedv:table[@id='" + tableNode.Attributes["id"].Value + "']/coedv:fields[@isDefault='0' or not(@isDefault) or @isDefault='false']"); //CBOE-1720 added @isDefault='false'
            isDefaultFalseExp.AddSort("@alias", XmlSortOrder.Ascending, XmlCaseOrder.None, "", XmlDataType.Text);
            isDefaultFalseExp.SetContext(manager);
            //Sort the current table non-default fields by field alias
            XPathNodeIterator isDefaultFalseNodeIterator = nav.Select(isDefaultFalseExp);

            //Get the tag XML
            XmlNode tagsNodeOld = tableNode.SelectSingleNode("coedv:tags", manager);
            XmlNode tagsNodeNew = xmlDataviewResponse.CreateElement("tags", "COE.COEDataView");
            if(tagsNodeOld!=null)
                tagsNodeNew = tagsNodeOld.CloneNode(true);
            //copy the table node with out child nodes
            XmlNode tblNodeSorted = tableNode.CloneNode(false);
            tblNodeSorted.AppendChild(tagsNodeNew);

            //Get default fields in sort order
            while (isDefaultTrueNodeIterator.MoveNext())
            {
                XmlNode fieldsNode = ((IHasXmlNode)isDefaultTrueNodeIterator.Current).GetNode();
                tblNodeSorted.AppendChild(fieldsNode);
            }

            //Get non-default fields in sort order
            while (isDefaultFalseNodeIterator.MoveNext())
            {
                XmlNode fieldsNode = ((IHasXmlNode)isDefaultFalseNodeIterator.Current).GetNode();
                tblNodeSorted.AppendChild(fieldsNode);
            }

            return tblNodeSorted;
        }
        /// <summary>
        /// Relace Special Characters to string specified.
        /// </summary>
        /// <param name="stringValue">String to replace</param>
        /// <returns></returns>
        public static string ReplaceStringForSpecialCharToSave(string stringValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(stringValue))
                {
                    stringValue = stringValue.Contains(">") ? stringValue.Replace(">", "&gt;") : stringValue;
                    stringValue = stringValue.Contains("<") ? stringValue.Replace("<", "&lt;") : stringValue;
                    stringValue = stringValue.Contains("&") ? stringValue.Replace("&", "&amp;") : stringValue;
                    stringValue = stringValue.Contains("%") ? stringValue.Replace("%", "&#37;") : stringValue;
                }
            }
            catch
            {
                throw;
            }
            return stringValue;
        }
        /// <summary>
        /// Relace Strings by special Characters 
        /// </summary>
        /// <param name="stringValue">String to replace</param>
        /// <returns></returns>
        public static string ReplaceStringForSpecialCharToLoad(string stringValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(stringValue))
                {
                    stringValue = stringValue.Contains("&gt;") ? stringValue.Replace("&gt;", ">") : stringValue;
                    stringValue = stringValue.Contains("&lt;") ? stringValue.Replace("&lt;", "<") : stringValue;
                    stringValue = stringValue.Contains("&amp;") ? stringValue.Replace("&amp;", "&") : stringValue;
                    stringValue = stringValue.Contains("&#37;") ? stringValue.Replace("&#37;", "%") : stringValue;
                }
            }
            catch
            {
                throw;
            }
            return stringValue;
        }
    }
}
