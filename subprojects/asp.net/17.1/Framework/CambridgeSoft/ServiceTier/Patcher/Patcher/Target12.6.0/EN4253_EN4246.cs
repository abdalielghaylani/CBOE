using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Patch to update missing xml configuration in 4006.xml.
    /// </summary>
    class EN4253_EN4246 : BugFixBaseCommand
    {

        #region Variables
        XmlNode _parentNode;
        XmlNode _newNode;
        string _nameSpaceURI = string.Empty;
        const string PREFIX = "COE:";
        bool _errorsInPatch = false;
        XmlDocument _formDoc = new XmlDocument();
        XmlNamespaceManager _manager;
        string _innerXml = string.Empty;
        #endregion


        #region Property
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

        #endregion


        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            string coeFormPath = string.Empty;
            XmlNode tempNode = null;

            #region GetFormDoc & Namespace
            for (int i = 0; i < forms.Count; i++) // Loop through List with for
            {
                _formDoc = (XmlDocument)(forms[i]);
                string id = _formDoc.DocumentElement.Attributes["id"] == null ? string.Empty : _formDoc.DocumentElement.Attributes["id"].Value;
                if (id == "4006")
                    break;
            }
            #endregion
                    
            #region manager
            _manager = new XmlNamespaceManager(_formDoc.NameTable);
            _manager.AddNamespace("COE", "COE.FormGroup");
            _nameSpaceURI = "COE.FormGroup";
            #endregion

            #region Project Registry_Level
            coeFormPath = "//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='PROJECT']";
            _parentNode = _formDoc.SelectSingleNode(coeFormPath, _manager);
            if (_parentNode != null)
            {
                tempNode = _parentNode.SelectSingleNode("COE:label", _manager);
                if (tempNode != null)
                {
                    tempNode.InnerText = "Registry Project Name";
                    messages.Add("Form[4006]: Label node was updated successfully.");
                }
                else
                {
                    _errorsInPatch = true;
                    messages.Add("Form[4006]: Label node was not updated due to errors.");
                }
                tempNode = _parentNode.SelectSingleNode("COE:Id", _manager);
                if (tempNode != null)
                {
                    tempNode.InnerText = "REGISTRY_PROJECTDropDownListPerm";
                    messages.Add("Form[4006]: Id node was updated successfully.");
                }
                else
                {
                    _errorsInPatch = true;
                    messages.Add("Form[4006]: Id node was not updated due to errors.");
                }
                tempNode = _parentNode.SelectSingleNode("COE:configInfo/COE:fieldConfig/COE:dropDownItemsSelect", _manager);
                if (tempNode != null)
                {
                    tempNode.InnerText = "SELECT PROJECTID as key, NAME as value FROM REGDB.VW_PROJECT WHERE (TYPE ='R' OR TYPE='A') AND (ACTIVE = 'T' OR ACTIVE = 'F')";
                    messages.Add("Form[4006]: dropDownItemsSelect node was updated successfully.");
                }
                else
                {
                    _errorsInPatch = true;
                    messages.Add("Form[4006]: dropDownItemsSelect node was not updated due to errors.");
                }
                tempNode = _parentNode.SelectSingleNode("COE:configInfo/COE:fieldConfig/COE:ID", _manager);
                if (tempNode != null)
                {
                    tempNode.InnerText = "REGISTRY_PROJECTDropDownListPerm";
                    messages.Add("Form[4006]: ID node was updated successfully.");
                }
                else
                {
                    _errorsInPatch = true;
                    messages.Add("Form[4006]: ID node was not updated due to errors.");
                }
                if (_parentNode.Attributes["name"] != null)
                {
                    _parentNode.Attributes["name"].Value = "REGISTRY_PROJECT";
                    messages.Add("Form[4006]: Name attribute was added to REGISTRY_PROJECT Form Element successfully.");
                    messages.Add("Form[4006]: PROJECT Form Element was updated successfully.");
                }
                else
                {
                    _errorsInPatch = true;
                    messages.Add("Form[4006]: Not able to update PROJECT Element attribute to REGISTRY_PROJECT due to errors.");
                }
            }
            else
            {
                coeFormPath = "//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='REGISTRY_PROJECT']";
                _parentNode = _formDoc.SelectSingleNode(coeFormPath, _manager);
                if (_parentNode != null)
                    messages.Add("Form[4006]: REGISTRY_PROJECT element is already updated.");
                else
                {
                    _errorsInPatch = true;
                    messages.Add("Form[4006]: Not able to update Project form element as its not available.");
                }
            }
            #endregion

            #region Project Batch_Level
            InnerXml = XmlCode.BatchProjectXML.Replace("\r\n", "").Trim();
            coeFormPath = "//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:layoutInfo/COE:formElement[@name='BATCH_PROJECT']";
            _parentNode = _formDoc.SelectSingleNode(coeFormPath, _manager);
            if (_parentNode != null)
                messages.Add("Form[4006]: BATCH_PROJECT element is already updated.");
            else
            {
                coeFormPath = "//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:layoutInfo";
                _parentNode = _formDoc.SelectSingleNode(coeFormPath, _manager);
                if (_parentNode != null)
                {
                    _newNode = _parentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "formElement", _nameSpaceURI);
                    if (_newNode != null)
                    {
                        try
                        {
                            _newNode.InnerXml = InnerXml;
                            XmlAttribute attributes = _newNode.OwnerDocument.CreateAttribute("name");
                            _newNode.Attributes.Append(attributes);
                            _newNode.Attributes["name"].Value = "BATCH_PROJECT";
                            _parentNode.AppendChild(_newNode);
                            messages.Add("Form[4006]: BATCH_PROJECT element is added successfully.");
                        }
                        catch (Exception ex)
                        {
                            _errorsInPatch = true;
                            messages.Add("Form[4006]: Not able to add BATCH_PROJECT form element due to error: " + ex.Message);
                        }
                    }
                    else 
                    {
                        _errorsInPatch = true;
                        messages.Add("Form[4006]: Not able to create BATCH_PROJECT form element.");                    
                    }
                }
                else
                {
                    _errorsInPatch = true;
                    messages.Add("Form[4006]: Not able to locate the parent node to add BATCH_PROJECT form element.");
                }
            }

            #endregion

            if (!_errorsInPatch)
                messages.Add("EN4253_EN4246 ISSUE was fixed successfully .");
            else
                messages.Add("EN4253_EN4246 ISSUE was not fixed due to errors.");
            return messages;
        }


        private class XmlCode
        {
            #region Project Batch_Level_XML
            public const string BatchProjectXML = @"
              <label xmlns=""COE.FormGroup"">Batch Project Name</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup""/>
              <bindingExpression xmlns=""COE.FormGroup"">SearchCriteria[37].Criterium.Value</bindingExpression>
              <Id xmlns=""COE.FormGroup"">BATCH_PROJECTDropDownListPerm</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <cssClass xmlns=""COE.FormGroup"">Std25x40</cssClass>
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList</type>
                <visible xmlns=""COE.FormGroup"">true</visible>
              </displayInfo>
              <serverEvents xmlns=""COE.FormGroup""/>
              <clientEvents xmlns=""COE.FormGroup""/>
              <configInfo xmlns=""COE.FormGroup"">
                <fieldConfig>
                  <dropDownItemsSelect>SELECT PROJECTID as key, NAME as value FROM REGDB.VW_PROJECT WHERE (TYPE ='B' OR TYPE='A') AND (ACTIVE = 'T' OR ACTIVE = 'F')</dropDownItemsSelect>
                  <PickListDomain />
                  <CSSLabelClass>FELabel</CSSLabelClass>
                  <CSSClass>FEDropDownList</CSSClass>
                  <Enable>True</Enable> 
                  <ID>Inner_BATCH_PROJECTDropDownListPerm</ID> 
                  <AutoPostBack>False</AutoPostBack> 
                </fieldConfig>
              </configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <searchCriteriaItem xmlns=""COE.FormGroup"" fieldid=""1302"" id=""37"" tableid=""13"">
                <numericalCriteria negate=""NO"" trim=""NONE"" operator=""EQUAL"" /> 
              </searchCriteriaItem>
              <displayData xmlns=""COE.FormGroup""/>
";
            #endregion
        }
    }
}
