using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Patch to update missing xml configuration in 4003.xml from upgrade [1210_1250].
    /// </summary>
    class Upgrade1250_4003 : BugFixBaseCommand
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




            #region Dataview Updates

            #region GetFormDoc & Namespace
            for (int i = 0; i < dataviews.Count; i++) // Loop through List with for
            {
                _formDoc = (XmlDocument)(dataviews[i]);
                string id = _formDoc.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : _formDoc.DocumentElement.Attributes["dataviewid"].Value;
                if (id == "4003")
                    break;
            }
            _manager = new XmlNamespaceManager(_formDoc.NameTable);
            _manager.AddNamespace("COE", "COE.COEDataView");
            _nameSpaceURI = "COE.COEDataView";
            #endregion

            #region table id="16"
            coeFormPath = "//COE:tables";
            InnerXml = "<fields id=\"1600\" name=\"ID\" dataType=\"INTEGER\" alias=\"UNIT_ID\" indexType=\"NONE\" mimeType=\"NONE\" visible=\"1\" xmlns=\"COE.COEDataView\" /><fields id=\"1601\" name=\"UNIT\" dataType=\"TEXT\" alias=\"UNIT_VALUE\" indexType=\"NONE\" mimeType=\"NONE\" visible=\"1\" xmlns=\"COE.COEDataView\" />";
            NewElementAttributes = "[@id='16']";
            InsertAttributes("id", "16");
            InsertAttributes("name", "VW_UNIT");
            InsertAttributes("alias", "VW_UNIT");
            InsertAttributes("database", "REGDB");
            InsertAttributes("primaryKey", "1600");
            status = ManipulateNode(coeFormPath, "table", "clientEvents", "validationRuleList");
            messages.Add(status);
            Reset();
            #endregion

            #region table id="17"
            coeFormPath = "//COE:tables";
            InnerXml = "<fields id=\"1700\" name=\"REGID\" dataType=\"INTEGER\" alias=\"REGID\" indexType=\"NONE\" mimeType=\"NONE\" visible=\"1\" xmlns=\"COE.COEDataView\" /><fields id=\"1701\" name=\"STRUCTUREAGGREGATION\" dataType=\"TEXT\" alias=\"STRUCTUREAGGREGATION\" indexType=\"CS_CARTRIDGE\" mimeType=\"NONE\" visible=\"1\" xmlns=\"COE.COEDataView\" />";
            NewElementAttributes = "[@id='17']";
            InsertAttributes("id", "17");
            InsertAttributes("name", "MIXTURES");
            InsertAttributes("alias", "MIXTURES");
            InsertAttributes("database", "REGDB");
            InsertAttributes("primaryKey", "1700");
            status = ManipulateNode(coeFormPath, "table", "clientEvents", "validationRuleList");
            messages.Add(status);
            Reset();
            #endregion

            #region Relation for table id="17"
            coeFormPath = "//COE:relationships";
            NewElementAttributes = "[@child='17']";
            InsertAttributes("child", "17");
            InsertAttributes("parent", "1");
            InsertAttributes("childkey", "1700");
            InsertAttributes("parentkey", "101");
            InsertAttributes("jointype", "INNER");
            status = ManipulateNode(coeFormPath, "relationship", "clientEvents", "validationRuleList");
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
                if (id == "4003")
                    break;
            }
            _manager = new XmlNamespaceManager(_formDoc.NameTable);
            _manager.AddNamespace("COE", "COE.FormGroup");
            _nameSpaceURI = "COE.FormGroup";
            #endregion

            #region Server Events
            coeFormPath = "//COE:listForms[@defaultForm='0']/COE:listForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='']";
            status = ManipulateNode(coeFormPath, "serverEvents", "clientEvents", "validationRuleList");
            messages.Add(status);
            Reset();
            #endregion

            #region Client Events
            coeFormPath = "//COE:listForms[@defaultForm='0']/COE:listForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='']";
            status = ManipulateNode(coeFormPath, "clientEvents", "", "serverEvents");
            messages.Add(status);
            Reset();
            #endregion

            #endregion


            if (!_errorsInPatch)
                messages.Add("Upgrade1250_4003 Workflow was successfully fixed.");
            else
                messages.Add("Upgrade1250_4003 Workflow  was fixed with partial update.");
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
                        return "Form[4003]: " + _newElementPath + " was added succesfully.";
                    else
                        return "Form[4003]: " + _newElementPath + " was not added due to errors.";
                }
                else
                {
                    _errorsInPatch = true;
                    return "Form[4003]: " + _newElementPath + " was already available.";
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
