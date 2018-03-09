using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Patch to update Struct_Comments form element .
    /// </summary>
    class CSBR156118 : BugFixBaseCommand
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
            string fieldId = string.Empty;
            string searchCriteriumId = string.Empty;
            string tableId = "1";
            XmlNodeList xnodelist = null;
            #region Dataview Updates

            #region GetFormDoc & Namespace
            for (int i = 0; i < dataviews.Count; i++) // Loop through List with for
            {
                _formDoc = (XmlDocument)(dataviews[i]);
                string id = _formDoc.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : _formDoc.DocumentElement.Attributes["dataviewid"].Value;
                if (id == "4002")
                    break;
            }
            _manager = new XmlNamespaceManager(_formDoc.NameTable);
            _manager.AddNamespace("COE", "COE.COEDataView");
            _nameSpaceURI = "COE.COEDataView";
            #endregion

            #region fields @ name=AMOUNT_UNITS

            ArrayList arrDataViewtableId = new ArrayList();
            string dataViewtableId = string.Empty;
            if (_formDoc.SelectSingleNode("//COE:tables/COE:table[@name='VW_UNIT']", _manager) == null)
            {
                xnodelist = _formDoc.SelectNodes("//COE:tables/COE:table", _manager);
                foreach (XmlNode xformNode in xnodelist)
                {
                    if (xformNode != null && xformNode.Attributes["id"] != null)
                    {
                        arrDataViewtableId.Add(Convert.ToInt32(xformNode.Attributes["id"].Value));
                    }
                }
                arrDataViewtableId.Sort();
                dataViewtableId = arrDataViewtableId[arrDataViewtableId.Count - 1].ToString();
                dataViewtableId = (Convert.ToInt32(dataViewtableId) + 1).ToString();
            }
            else
                dataViewtableId = _formDoc.SelectSingleNode("//COE:tables/COE:table[@name='VW_UNIT']", _manager).Attributes["id"].Value;



            coeFormPath = "//COE:tables/COE:table[@id='" + tableId + "']";
            NewElementAttributes = "[@name='AMOUNT_UNITS']";
            status = ManipulateNode(coeFormPath, "COE:fields", "", "");
            if (_newNode != null && _newNode.Attributes["id"] != null)
            {
                //<fields id="155" name="AMOUNT_UNITS" dataType="INTEGER" lookupFieldId="800" lookupDisplayFieldId="801" indexType="NONE" mimeType="NONE" visible="1" lookupSortOrder="ASCENDING" alias="AMOUNT_UNITS"/>

                if (_newNode.Attributes["lookupFieldId"].Value != dataViewtableId + "00")
                {
                    _newNode.Attributes["lookupFieldId"].Value = dataViewtableId + "00";
                    _newNode.Attributes["lookupDisplayFieldId"].Value = dataViewtableId + "01";
                }
                fieldId = _newNode.Attributes["id"].Value;
            }
            messages.Add(status);
            Reset();

            #region Add Table UNITS
            coeFormPath = "//COE:tables";
            NewElementAttributes = "[@name='VW_UNIT']";
            InsertAttributes("id", dataViewtableId);
            InsertAttributes("name", "VW_UNIT");
            InsertAttributes("alias", "VW_UNIT");
            InsertAttributes("database", "REGDB");
            InsertAttributes("primaryKey", dataViewtableId + "00");
            InnerXml = XmlCode.DataviewTable_VW_Units.Trim();
            InnerXml = string.Format(InnerXml, dataViewtableId + "00", dataViewtableId + "01");
            status = ManipulateNode(coeFormPath, "table", "", "COE:table[@name='VW_TEMPORARYREGNUMBERSPROJECT']");
            messages.Add(status);
            Reset();
            #endregion

            #endregion


            #endregion

            #region FromBO Updates

            #region GetFormDoc & Namespace
            for (int i = 0; i < forms.Count; i++) // Loop through List with for
            {
                _formDoc = (XmlDocument)(forms[i]);
                string id = _formDoc.DocumentElement.Attributes["id"] == null ? string.Empty : _formDoc.DocumentElement.Attributes["id"].Value;
                if (id == "4002")
                    break;
            }
            _manager = new XmlNamespaceManager(_formDoc.NameTable);
            _manager.AddNamespace("COE", "COE.FormGroup");
            _nameSpaceURI = "COE.FormGroup";
            #endregion

            xnodelist = _formDoc.SelectNodes("//COE:queryForms[@defaultForm='0']/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement", _manager);

            ArrayList searchCriterium = new ArrayList();
            foreach (XmlNode xformNode in xnodelist)
            {
                XmlNode nodeSearchcriterum = xformNode.SelectSingleNode("COE:searchCriteriaItem", _manager);
                if (nodeSearchcriterum != null && nodeSearchcriterum.Attributes["id"] != null)
                {
                    searchCriterium.Add(Convert.ToInt32(nodeSearchcriterum.Attributes["id"].Value));
                }
            }
            searchCriterium.Sort();
            searchCriteriumId = searchCriterium[searchCriterium.Count - 1].ToString();
            searchCriteriumId = (Convert.ToInt32(searchCriteriumId) + 1).ToString();

            if (_formDoc.SelectSingleNode("//COE:queryForms[@defaultForm='0']/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='AMOUNT_UNITS']", _manager) != null)
            {
                searchCriteriumId = _formDoc.SelectSingleNode("//COE:queryForms[@defaultForm='0']/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='AMOUNT_UNITS']/COE:searchCriteriaItem", _manager).Attributes["id"].Value;

            }

            #region Queryform AMOUNT_UNITS
            coeFormPath = "//COE:queryForms[@defaultForm='0']/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo";
            NewElementAttributes = "[@name='AMOUNT_UNITS']";
            InsertAttributes("name", "AMOUNT_UNITS");
            InnerXml = XmlCode.queryFormAmount_Units.Trim();
            InnerXml = string.Format(InnerXml, searchCriteriumId, fieldId, tableId);
            status = ManipulateNode(coeFormPath, "formElement", "COE:formElement[@name='APPEARANCE']", "COE:formElement[@name='AMOUNT']");
            messages.Add(status);
            Reset();
            #endregion

            #region listform AMOUNT_UNITS
            coeFormPath = "//COE:listForms[@defaultForm='0']/COE:listForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='']/COE:configInfo/COE:fieldConfig/COE:tables/COE:table[@name='Table_1']/COE:Columns";
            NewElementAttributes = "[@name='UNITS']";
            InsertAttributes("name", "UNITS");
            InsertAttributes("hidden", "false");
            InnerXml = XmlCode.listFormAmount_Units.Trim();
            status = ManipulateNode(coeFormPath, "COE:Column", "COE:Column[@name='APPEARANCE']", "COE:Column[@name='AMOUNT']");
            messages.Add(status);
            Reset();
            #endregion

            #endregion


            if (!_errorsInPatch)
                messages.Add("CSBR159135 Workflow was successfully fixed.");
            else
                messages.Add("CSBR159135 Workflow  was fixed with partial update.");
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
                        return "Form[4002]: " + _newElementPath + " was added succesfully.";
                    else
                        return "Form[4002]: " + _newElementPath + " was not added due to errors.";
                }
                else
                {
                    if (InnerText != string.Empty)
                        _newNode.InnerText = InnerText;
                    else if (InnerXml != string.Empty)
                        _newNode.InnerXml = InnerXml;
                    return "Form[4002]: " + _newElementPath + " was updated.";
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

            #region ReadonlyStructureComments
            public const string queryFormAmount_Units = @"
              <label xmlns=""COE.FormGroup"">Units</label>
              <showHelp xmlns=""COE.FormGroup"">false</showHelp>
              <helpText xmlns=""COE.FormGroup""/>
              <defaultValue xmlns=""COE.FormGroup""/>
              <bindingExpression xmlns=""COE.FormGroup"">SearchCriteria[{0}].Criterium.Value</bindingExpression>
              <Id xmlns=""COE.FormGroup"">AMOUNT_UNITSProperty</Id>
              <displayInfo xmlns=""COE.FormGroup"">
                <cssClass xmlns=""COE.FormGroup"">Std25x40</cssClass>
                <type xmlns=""COE.FormGroup"">CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList</type>
                <visible xmlns=""COE.FormGroup"">true</visible>
              </displayInfo>
              <validationRuleList xmlns=""COE.FormGroup""/>
              <serverEvents xmlns=""COE.FormGroup""/>
              <clientEvents xmlns=""COE.FormGroup""/>
              <COE:configInfo xmlns:COE=""COE.FormGroup"">
	            <COE:fieldConfig>
		            <COE:dropDownItemsSelect>SELECT ID as key,UNIT as value FROM REGDB.VW_Unit ORDER BY LOWER(UNIT) ASC</COE:dropDownItemsSelect>
		            <COE:CSSClass>FEDropDownList</COE:CSSClass>
		            <COE:CSSLabelClass>FELabel</COE:CSSLabelClass>
		            <COE:Enable>True</COE:Enable>
		            <COE:ID>AMOUNT_UNITSProperty</COE:ID>
		            <COE:AutoPostBack>False</COE:AutoPostBack>
	            </COE:fieldConfig>
		      </COE:configInfo>
              <dataSource xmlns=""COE.FormGroup""/>
              <dataSourceId xmlns=""COE.FormGroup""/>
              <searchCriteriaItem fieldid=""{1}"" id=""{0}"" tableid=""{2}"" xmlns=""COE.FormGroup"">
                   <numericalCriteria negate=""NO"" trim=""NONE"" operator=""IN"" xmlns=""COE.FormGroup""/>        
              </searchCriteriaItem>
              <displayData xmlns=""COE.FormGroup""/>
";

            public const string listFormAmount_Units = @"
                <headerText>Units</headerText>
                <width>50px</width>
                <formElement xmlns=""COE.FormGroup""  name=""AMOUNT_UNITS"">
                    <label xmlns=""COE.FormGroup""></label>
                    <defaultValue xmlns=""COE.FormGroup""></defaultValue>
                    <bindingExpression xmlns=""COE.FormGroup"">AMOUNT_UNITS</bindingExpression>
                    <Id xmlns=""COE.FormGroup"">AMOUNT_UNITSProperty</Id>
                    <validationRuleList xmlns=""COE.FormGroup""/>
              <COE:configInfo xmlns:COE=""COE.FormGroup"">
	            <COE:fieldConfig>
		            <COE:CSSClass>FELabel</COE:CSSClass>
		            <Width>50px</Width>
	            </COE:fieldConfig>
		      </COE:configInfo>
                    <dataSource xmlns=""COE.FormGroup""/>
                    <dataSourceId xmlns=""COE.FormGroup""/>
                </formElement>
";
            public const string DataviewTable_VW_Units = @"
      <fields id='{0}' name='ID' dataType='INTEGER' alias='ID' indexType='NONE' mimeType='NONE' visible='1'   xmlns=""COE.COEDataView"" />
      <fields id='{1}' name='UNIT' dataType='TEXT' alias='UNIT_VALUE' indexType='NONE' mimeType='NONE' visible='1' xmlns=""COE.COEDataView""  />
";
            #endregion

        }
    }


}
