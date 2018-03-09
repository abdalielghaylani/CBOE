using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Patch to update missing xml configuration in 4006.xml from upgrade [1210_1250].
    /// </summary>
    class Upgrade1250_4006 : BugFixBaseCommand
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

           

            #region FromBO Updates

            #region GetFormDoc & Namespace
            for (int i = 0; i < forms.Count; i++) // Loop through List with for
            {
                _formDoc = (XmlDocument)(forms[i]);
                string id = _formDoc.DocumentElement.Attributes["id"] == null ? string.Empty : _formDoc.DocumentElement.Attributes["id"].Value;
                if (id == "4006")
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
                messages.Add("Upgrade1250_4006 Workflow was successfully fixed.");
            else
                messages.Add("Upgrade1250_4006 Workflow  was fixed with partial update.");
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
                        return "Form[4006]: " + _newElementPath + " was added succesfully.";
                    else
                        return "Form[4006]: " + _newElementPath + " was not added due to errors.";
                }
                else
                {
                    _errorsInPatch = true;
                    return "Form[4006]: " + _newElementPath + " was already available.";
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
