using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Patch to update new structure column to show structures with NO STRUCTURE etc.
    /// </summary>
    class CSBR161739 : BugFixBaseCommand
    {

        #region Variables
        XmlNode _parentNode;
        XmlNode _newNode;
        XmlNode _insertBefore;
        XmlNode _insertAfter;
        string _newElementPath = string.Empty;
        string _newElementAttributes = string.Empty;
        string _nameSpaceURI = string.Empty;
        const string PREFIX = "COE:";
        bool _errorsInPatch = false;
        XmlDocument _formDoc = new XmlDocument();
        XmlNamespaceManager _manager;
        string _innerText = string.Empty;
        string _innerXml = string.Empty;
        Dictionary<string, string> _listAttributes = new Dictionary<string, string>();
        #endregion


        #region Property
        private string InnerText
        {
            get
            {
                return _innerText;
            }
            set
            {
                _innerText = value;
            }
        }

        private string NewElementAttributes
        {
            get
            {
                return _newElementAttributes;
            }
            set
            {
                _newElementAttributes = value;
            }
        }

        private string InnerXml
        {
            get
            {
                return _innerXml;
            }
            set
            {
                _innerXml = value;
            }
        }

        private Dictionary<string, string> ListAttributes
        {
            get
            {
                return _listAttributes;
            }
        }

        #endregion



        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            string coeFormPath = string.Empty;
            string status = string.Empty;
            string fieldIdToReplace = string.Empty;
            string fieldIdToReplaceBy = string.Empty;
            string newFieldId = string.Empty;
            string oldFieldId = string.Empty;
            string searchCriteriumId = string.Empty;
            string tableId = "1";
            XmlNodeList xnodelist = null;


            #region Dataview Updates

            #region GetFormDoc & Namespace
            for (int i = 0; i < dataviews.Count; i++) // Loop through List with for
            {
                _formDoc = (XmlDocument)(dataviews[i]);
                string id = _formDoc.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : _formDoc.DocumentElement.Attributes["dataviewid"].Value;
                if (id == "4016")
                    break;
            }
            _manager = new XmlNamespaceManager(_formDoc.NameTable);
            _manager.AddNamespace("COE", "COE.COEDataView");
            _nameSpaceURI = "COE.COEDataView";
            #endregion

            #region fields @ name=NORMALIZEDSTRUCTURE

            // Get Last FieldId
            ArrayList arrFieldTableId = new ArrayList();
            tableId = _formDoc.SelectSingleNode("//COE:tables/COE:table[@name='VW_COMPOUND_STRUCTURE']", _manager).Attributes["id"].Value;
            xnodelist = _formDoc.SelectNodes("//COE:tables/COE:table[@name='VW_COMPOUND_STRUCTURE']/COE:fields", _manager);
            foreach (XmlNode xformNode in xnodelist)
            {
                if (xformNode != null && xformNode.Attributes["id"] != null)
                {
                    arrFieldTableId.Add(Convert.ToInt32(xformNode.Attributes["id"].Value));
                }
            }
            newFieldId = tableId + "0";
            arrFieldTableId.Sort();
            newFieldId = arrFieldTableId[arrFieldTableId.Count - 1].ToString();
            newFieldId = (Convert.ToInt32(newFieldId) + 1).ToString();

            oldFieldId = _formDoc.SelectSingleNode("//COE:tables/COE:table[@name='VW_COMPOUND_STRUCTURE']/COE:fields [@name='STRUCTURE'][@alias='STRUCTURE']", _manager).Attributes["id"].Value;

            fieldIdToReplace = "fieldid=\"" + oldFieldId + "\"";
            fieldIdToReplaceBy = "fieldid=\"" + newFieldId + "\"";


            // Add Field
            coeFormPath = "//COE:tables/COE:table[@id='" + tableId + "']";
            NewElementAttributes = "[@name='NORMALIZEDSTRUCTURE']";
            _listAttributes.Add("id", newFieldId);
            _listAttributes.Add("name", "NORMALIZEDSTRUCTURE");
            _listAttributes.Add("dataType", "TEXT");
            _listAttributes.Add("alias", "STRUCTURE");
            _listAttributes.Add("indexType", "CS_CARTRIDGE");
            _listAttributes.Add("mimeType", "NONE");
            _listAttributes.Add("sortOrder", Convert.ToString(arrFieldTableId.Count + 1));
            status = ManipulateNode(coeFormPath, "COE:fields", "", "");
            messages.Add(status);
            Reset();

            #endregion


            #endregion

            #region FromBO Updates

            #region GetFormDoc & Namespace
            for (int i = 0; i < forms.Count; i++) // Loop through List with for
            {
                _formDoc = (XmlDocument)(forms[i]);
                string id = _formDoc.DocumentElement.Attributes["id"] == null ? string.Empty : _formDoc.DocumentElement.Attributes["id"].Value;
                if (id == "4016")
                    break;
            }
            _manager = new XmlNamespaceManager(_formDoc.NameTable);
            _manager.AddNamespace("COE", "COE.FormGroup");
            _nameSpaceURI = "COE.FormGroup";
            #endregion

            string xml = _formDoc.InnerXml;
            xml = xml.Replace(fieldIdToReplace, fieldIdToReplaceBy);
            fieldIdToReplace = fieldIdToReplace.Replace("fieldid", "fieldId");
            fieldIdToReplaceBy = fieldIdToReplaceBy.Replace("fieldid", "fieldId");
            xml = xml.Replace(fieldIdToReplace, fieldIdToReplaceBy);
            _formDoc.InnerXml = xml;

            #endregion


            if (!_errorsInPatch)
                messages.Add("CSBR161739 Workflow was successfully fixed.");
            else
                messages.Add("CSBR161739 Workflow  was fixed with partial update.");
            return messages;
        }

        #region Private Function & Method
        private void createNewAttribute(string attributeName, string attributeValue, ref XmlNode node)
        {
            XmlAttribute attributes = node.OwnerDocument.CreateAttribute(attributeName);
            node.Attributes.Append(attributes);
            node.Attributes[attributeName].Value = attributeValue;
        }
        private string ManipulateNode(string coeFormPath, string newElementPath, string beforeNode, string afterNode)
        {
            try
            {
                _newElementPath = newElementPath;
                _parentNode = _formDoc.SelectSingleNode(coeFormPath, _manager);
                _newNode = GetNode(_newElementPath, _parentNode, _manager);
                NewElementAttributes = "";
                _insertBefore = GetNode(beforeNode, _parentNode, _manager);
                _insertAfter = GetNode(afterNode, _parentNode, _manager);
                if (_newNode == null)
                {
                    _newNode = _parentNode.OwnerDocument.CreateNode(XmlNodeType.Element, _newElementPath, _nameSpaceURI);
                    if (InnerText != string.Empty)
                        _newNode.InnerText = InnerText;
                    else if (InnerXml != string.Empty)
                        _newNode.InnerXml = InnerXml;
                    if (ListAttributes.Count > 0)
                        foreach (KeyValuePair<string, string> entry in ListAttributes)
                            createNewAttribute(entry.Key, entry.Value, ref _newNode);
                    if (InsertNode(_parentNode, _newNode, _insertBefore, _insertAfter))
                        return "Form[4016]: " + _newElementPath + " was added succesfully.";
                    else
                        return "Form[4016]: " + _newElementPath + " was not added due to errors.";
                }
                else
                {
                    if (InnerText != string.Empty)
                        _newNode.InnerText = InnerText;
                    else if (InnerXml != string.Empty)
                        _newNode.InnerXml = InnerXml;
                    return "Form[4016]: " + _newElementPath + " was updated.";
                }
            }
            catch (Exception ex)
            { _errorsInPatch = true; return ex.Message; }
        }

        private XmlNode GetNode(string path, XmlNode rootNode, XmlNamespaceManager manager)
        {
            XmlNode childNode;
            path = (path == String.Empty) ? "NoEmptyName" : path;
            childNode = rootNode.SelectSingleNode(PREFIX + path.Replace(PREFIX, "") + NewElementAttributes, manager);
            if (childNode == null)
            {
                childNode = rootNode.SelectSingleNode(path.Replace(PREFIX, "") + NewElementAttributes, manager);
                if (childNode == null)
                    childNode = rootNode.SelectSingleNode(path.Replace(PREFIX, "") + NewElementAttributes);
            }
            return childNode;
        }

        private Boolean InsertNode(XmlNode parentNode, XmlNode childNode, XmlNode insertBeforeNode, XmlNode insertAfterNode)
        {
            try
            {
                if (insertBeforeNode != null)
                    parentNode.InsertBefore(childNode, insertBeforeNode);
                else if (insertAfterNode != null)
                    parentNode.InsertAfter(childNode, insertAfterNode);
                else
                    parentNode.AppendChild(childNode);
                return true;
            }
            catch (Exception ex)
            { _errorsInPatch = true; return false; }
        }
        private void InsertAttributes(string name, string value)
        {
            try
            {
                _listAttributes.Add(name, value);
            }
            catch (Exception ex)
            { }
        }

        private void Reset()
        {
            _parentNode = null;
            _newNode = null;
            _insertBefore = null;
            _insertAfter = null;
            _newElementPath = string.Empty;
            NewElementAttributes = string.Empty;
            InnerText = string.Empty;
            InnerXml = string.Empty;
            _listAttributes.Clear();
        }
        #endregion
    }


}
