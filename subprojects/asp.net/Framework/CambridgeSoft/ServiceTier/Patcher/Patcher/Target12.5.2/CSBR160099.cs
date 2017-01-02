using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Patch to insert Struct id form element .
    /// </summary>
    class CSBR160099 : BugFixBaseCommand
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
                if (id == "4011")
                    break;
            }
            _manager = new XmlNamespaceManager(_formDoc.NameTable);
            _manager.AddNamespace("COE", "COE.FormGroup");
            _nameSpaceURI = "COE.FormGroup";
            #endregion

            #region listform AMOUNT_UNITS
            coeFormPath = "//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:viewMode";
            NewElementAttributes = "[@name='Structure ID']";
            InsertAttributes("name", "Structure ID");
            InnerXml = XmlCode.FormStructureId.Trim();
            status = ManipulateNode(coeFormPath, "formElement", "COE:formElement[@name='Identifiers']", "COE:formElement[@name='MW']");
            messages.Add(status);
            Reset();
            #endregion

            #endregion


            if (!_errorsInPatch)
                messages.Add("CSBR160099 Workflow was successfully fixed.");
            else
                messages.Add("CSBR160099 Workflow  was fixed with partial update.");
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
                        return "Form[4011]: " + _newElementPath + " was added succesfully.";
                    else
                        return "Form[4011]: " + _newElementPath + " was not added due to errors.";
                }
                else
                {
                    if (InnerText != string.Empty)
                        _newNode.InnerText = InnerText;
                    else if (InnerXml != string.Empty)
                        _newNode.InnerXml = InnerXml;
                    return "Form[4011]: " + _newElementPath + " was updated.";
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

        private class XmlCode
        {

            #region Readonly Structure Id
            public const string FormStructureId = @"
              <label xmlns=""COE.FormGroup"">Structure ID</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <isFileUpload xmlns=""COE.FormGroup"">false</isFileUpload>
              <pageComunicationProvider xmlns=""COE.FormGroup"" />
              <fileUploadBindingExpression xmlns=""COE.FormGroup""/>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup""/>
              <bindingExpression xmlns=""COE.FormGroup"">Compound.BaseFragment.Structure.ID</bindingExpression>
              <Id xmlns=""COE.FormGroup"">CSID</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <cssClass xmlns=""COE.FormGroup"">Std25x40</cssClass>
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly</type>
                <visible xmlns=""COE.FormGroup"">true</visible>
              </displayInfo>
              <serverEvents xmlns=""COE.FormGroup""/>
              <clientEvents xmlns=""COE.FormGroup""/>
              <COE:configInfo xmlns:COE=""COE.FormGroup"">
	            <COE:fieldConfig>
		            <COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
		            <COE:CSSClass>FETextBoxViewMode</COE:CSSClass>
	            </COE:fieldConfig>
		      </COE:configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <requiredStyle xmlns=""COE.FormGroup""/>
              <displayData xmlns=""COE.FormGroup""/>
";
            #endregion

        }
    }


}
