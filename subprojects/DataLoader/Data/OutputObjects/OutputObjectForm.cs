using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Xml;
using CambridgeSoft.COE.Framework.COEDisplayDataBrokerService;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.COEPickListPickerService;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.DataLoader.Common;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Registration;

namespace CambridgeSoft.COE.DataLoader.Data.OutputObjects
{
    /// <summary>
    /// <see cref="OutputObject"/> for adding substances to registration
    /// </summary>
    class OutputObjectForm : OutputObject
    {
        public List<int> LoadableRecordIndices = new List<int>();

        private Dictionary<string, Dictionary<string, string>> _dictBindingExpressions;
        private Dictionary<string, Dictionary<string, string>> _dictBindingConstants;
        private Dictionary<string, Dictionary<string, string>> _dictBindingVariables;
        private List<string> _listFieldList;
        private string _serverDateFormat = "yyyy-MM-dd hh:mm:ss"; // "yyyy-MM-dd hh:mm:ss tt" does NOT work
        private CultureInfo _serverCultureInfo = new CultureInfo("en-US", false);

        //TODO: these variables need to be refactored
        private string _vstrLabel;
        private string _vstrFieldInfo;
        private string _vstrBindingExpression;
        private string _strDropDownItemsSelect;
        private string _strDisplayData;
        private int _DDLPickListDomainID;
        public static int updateformid;
        protected RegistryRecord _registryRecord;
        public static string activaRLSvalue;
        protected static string strConfiguration;
        protected static string batchOutputDataRow;
        protected static string batchId = null;
        #region > Fragment look-ups <

        private FragmentList _fragmentCollection = null;
        /// <summary>
        /// A lazy-load implementation of the master fragment list.
        /// </summary>
        public FragmentList FragmentCollection
        {
            get
            {
                if (_fragmentCollection == null)
                    _fragmentCollection = FragmentList.GetFragmentList();
                return _fragmentCollection;
            }
        }

        private SortedDictionary<string, int> _fragmentIdByDescription;
        /// <summary>
        /// A lazy-load lookup for fragment IDs based on fragment descriptions.
        /// </summary>
        private SortedDictionary<string, int> FragmentIdByDescription
        {
            get
            {
                if (_fragmentIdByDescription == null)
                {
                    _fragmentIdByDescription = new SortedDictionary<string, int>();
                    string key = null;
                    foreach (Fragment f in FragmentCollection)
                    {
                        key = f.Description.ToUpper().Trim();
                        if (!_fragmentIdByDescription.ContainsKey(key))
                            _fragmentIdByDescription.Add(key, f.FragmentID);
                    }
                }
                return _fragmentIdByDescription;
            }
        }

        private SortedDictionary<string, int> _fragmentIdByFormula;
        /// <summary>
        /// A lazy-load lookup for fragment IDs based on fragment formulae.
        /// </summary>
        private SortedDictionary<string, int> FragmentIdByFormula
        {
            get
            {
                if (_fragmentIdByFormula == null)
                {
                    _fragmentIdByFormula = new SortedDictionary<string, int>();
                    string key = null;
                    foreach (Fragment f in FragmentCollection)
                    {
                        key = f.Formula.ToUpper().Trim();
                        if (!_fragmentIdByFormula.ContainsKey(key))
                            _fragmentIdByFormula.Add(key, f.FragmentID);
                    }
                }
                return _fragmentIdByFormula;
            }
        }

        private SortedDictionary<string, int> _fragmentIdByCode;
        /// <summary>
        /// A lazy-load lookup for fragment IDs based on fragment codes.
        /// </summary>
        private SortedDictionary<string, int> FragmentIdByCode
        {
            get
            {
                if (_fragmentIdByCode == null)
                {
                    _fragmentIdByCode = new SortedDictionary<string, int>();
                    string key = null;
                    foreach (Fragment f in FragmentCollection)
                    {
                        key = f.Code.ToUpper().Trim();
                        if (!_fragmentIdByCode.ContainsKey(key))
                            _fragmentIdByCode.Add(key, f.FragmentID);
                    }
                }
                return _fragmentIdByCode;
            }
        }

        #endregion

        protected void ConfigurationSet(
            string value
            , int vnID
            , FormGroup.DisplayMode vnMode
            , string vstrLabel
            , string vstrFieldInfo
            , string vstrBindingExpression
        )
        {
            base.Configuration = value;
            _vstrLabel = vstrLabel;
            _vstrFieldInfo = vstrFieldInfo;
            _vstrBindingExpression = vstrBindingExpression;

            _dictBindingExpressions = new Dictionary<string, Dictionary<string, string>>();
            _listFieldList = new List<string>();

            COEFormBO oCOEFormBO = COEFormBO.Get(vnID);
            if (oCOEFormBO != null)
            {
                updateformid = oCOEFormBO.FormGroupId;
                strConfiguration = value.ToString();
                foreach (FormGroup.Display oDisplay in oCOEFormBO.COEFormGroup.DetailsForms.Displays)
                {
                    SetDisplay(oDisplay, vnMode);
                }
            }
        }

        /// <summary>
        /// Determines which 'form' elements from a COEForm can be bound to incoming data.
        /// </summary>
        /// <param name="oDisplay"></param>
        /// <param name="vnMode"></param>
        private void SetDisplay(FormGroup.Display oDisplay, FormGroup.DisplayMode vnMode)
        {
            List<int> bindableIds = new List<int>();

            //only extract form information if it is considered 'bindable'
            foreach (FormGroup.Form oForm in oDisplay.Forms)
            {
                //_strForm = string.Empty;
                //if (!string.IsNullOrEmpty(oForm.Title))
                //    _strForm = oForm.Title.Trim();

                //JED: use the DataSourceId instead of the form title as a Binding indicator
                string formKey = oForm.DataSourceId;
                int formId = oForm.Id;
                bool bindable = IsBindable(formId);

                if (bindable)
                    bindableIds.Add(formId);

                if (oForm.FormDisplay.Visible == true && bindable == true)
                    SetForm(oForm, vnMode, formKey.ToLower());
            }

            SetXmlFieldSpec();

        }

        protected bool IsBindable(int formId)
        {
            bool isBindable = false;
            switch (formId)
            {
                case 0:
                case 1:
                case 2:
                case 4:
                case 5:
                case 1000:
                case 1001:
                case 1002:
                case 1003:
                    {
                        isBindable = true; break;
                    }
                default:
                    break;
            }
            return isBindable;
        }

        /// <summary>
        /// Based on a data-access mode (Add/Edit), determines which list of 'form'
        /// field elements are available for binding.
        /// </summary>
        /// <remarks>
        /// There are different sets of data-bindings for Add and Edit modes for this
        /// particular COEForm.
        /// </remarks>
        /// <param name="oForm"></param>
        /// <param name="vnMode"></param>
        /// <param name="fieldListKey"></param>
        private void SetForm(FormGroup.Form oForm, FormGroup.DisplayMode vnMode, string fieldListKey)
        {//Fix for CSBR 160708 - "BATCH ID does not belong to table OutputTable" error while updating batches
            if (updateformid == 4011 && OutputType.ToString() == "Update batch data in registration")
            {
                if (fieldListKey.ToLower() == "batchlistcsladatasource")
                {
                    //distinguish between 'add' and 'edit' mode for form-field specifications
                    List<FormGroup.FormElement> oFormElementList = null;
                    if (vnMode == FormGroup.DisplayMode.Add)
                        oFormElementList = oForm.AddMode;
                    else if (vnMode == FormGroup.DisplayMode.Edit)
                        oFormElementList = oForm.EditMode;
                    if (oFormElementList == null)
                        throw new System.NullReferenceException();
                    //only extract form-field information if it is considered 'bindable'
                    foreach (FormGroup.FormElement oFormElement in oFormElementList)
                    {
                        //conditionally gather information from the FormElement
                        string bindingExpression = oFormElement.BindingExpression;
                        string formFieldType = oFormElement.DisplayInfo.Type;
                        string formFieldName = oFormElement.Name;

                        //skip read-only and non-binding fields
                        if (!string.IsNullOrEmpty(bindingExpression)
                            && !string.IsNullOrEmpty(formFieldName)
                            && !formFieldType.EndsWith(".COETextBoxReadOnly")
                            && oFormElement.DisplayInfo.Visible == true
                        )
                            SetFormElement(oFormElement, fieldListKey);
                    }
                }
            }
            else
            {
                //distinguish between 'add' and 'edit' mode for form-field specifications
                List<FormGroup.FormElement> oFormElementList = null;
                if (vnMode == FormGroup.DisplayMode.Add)
                    oFormElementList = oForm.AddMode;
                else if (vnMode == FormGroup.DisplayMode.Edit)
                    oFormElementList = oForm.EditMode;
                if (oFormElementList == null)
                    throw new System.NullReferenceException();
                //only extract form-field information if it is considered 'bindable'
                foreach (FormGroup.FormElement oFormElement in oFormElementList)
                {
                    //conditionally gather information from the FormElement
                    string bindingExpression = oFormElement.BindingExpression;
                    string formFieldType = oFormElement.DisplayInfo.Type;
                    string formFieldName = oFormElement.Name;

                    //skip read-only and non-binding fields
                    if (!string.IsNullOrEmpty(bindingExpression)
                        && !string.IsNullOrEmpty(formFieldName)
                        && !formFieldType.EndsWith(".COETextBoxReadOnly")
                        && oFormElement.DisplayInfo.Visible == true
                    )
                        SetFormElement(oFormElement, fieldListKey);
                }

            }
        }
        //Fix for CSBR: 164615-Projects were available at Registry information when RLS = Batch
        public string GetActivaRLSvalue()
        {
            _registryRecord = RegistryRecord.NewRegistryRecord();
            activaRLSvalue = _registryRecord.RLSStatus.ToString();
            return activaRLSvalue;
        }
        /// <summary>
        /// Determines if this COEForm 'form' field can be bound to incoming data.
        /// Also, directs the extraction of binding configuration information.
        /// </summary>
        /// <remarks>
        /// Most 'form' element exist because they either display or allow input of data
        /// from user interaction. Data-loading simulates user interaction on a record-by-record basis,
        /// so the objective is to allow for an auto-discovery objects/properties to bind the incoming 
        /// </remarks>data to.
        /// <param name="oFormElement"></param>
        /// <param name="fieldListKey"></param>
        private void SetFormElement(FormGroup.FormElement oFormElement, string fieldListKey)
        {
            _strDropDownItemsSelect = string.Empty;
            _strDisplayData = string.Empty;
            string bindingExpression = oFormElement.BindingExpression;
            string fieldLabel = string.Empty;
            string fieldId = oFormElement.Id;

            if (oFormElement.ConfigInfo != null)
                SetFieldConfigInfo(oFormElement, fieldListKey);

            if (!string.IsNullOrEmpty(oFormElement.Label))
                fieldLabel = oFormElement.Label;
            else if (!string.IsNullOrEmpty(oFormElement.DefaultValue))
                fieldLabel = oFormElement.DefaultValue;

            // Examine validation to infer type and default
            List<FormGroup.ValidationRuleInfo> oListValidationRuleList = oFormElement.ValidationRuleList;
            string fieldType = string.Empty;
            string fieldTypeSpecification = string.Empty;
            string fieldDefault = "default";
            string fieldIsRequired = "false";

            foreach (FormGroup.ValidationRuleInfo oValidationRuleInfo in oListValidationRuleList)
            {
                switch (oValidationRuleInfo.ValidationRuleName)
                {
                    case FormGroup.ValidationRuleEnum.Date:
                        fieldType = "Date";
                        break;
                    case FormGroup.ValidationRuleEnum.Double:
                    case FormGroup.ValidationRuleEnum.Float:
                        fieldType = "Decimal";
                        break;
                    case FormGroup.ValidationRuleEnum.Integer:
                    case FormGroup.ValidationRuleEnum.PositiveInteger:
                        fieldType = "Integer";
                        break;
                    case FormGroup.ValidationRuleEnum.RequiredField:
                    case FormGroup.ValidationRuleEnum.NotEmptyStructure:
                    case FormGroup.ValidationRuleEnum.NotEmptyStructureAndNoText:
                        fieldDefault = string.Empty;
                        fieldIsRequired = "true";
                        break;
                    case FormGroup.ValidationRuleEnum.TextLength:
                    {
                        // Implies String if type is not known
                        if (string.IsNullOrEmpty(fieldType)) fieldType = "String";
                        foreach (FormGroup.Parameter oParameter in oValidationRuleInfo.Params)
                        {
                            switch (oParameter.Name)
                            {
                                case "max": fieldTypeSpecification = oParameter.Value; break;
                                case "min": break;
                                default: break;
                            }
                        }
                        break;
                    }
                    default: break;
                }
            }

            // If we still don't know the type, consult displayInfo.Type
            if (string.IsNullOrEmpty(fieldType))
            {
                string strDisplayInfoType = oFormElement.DisplayInfo.Type;
                if (strDisplayInfoType.EndsWith("ChemDraw"))
                {
                    fieldType = "Binary";
                    fieldDefault = "map";
                    fieldLabel = "Structure";
                }
                else if (strDisplayInfoType.EndsWith("Fragments"))
                {
                    fieldType = "Integer";
                }
                else if (strDisplayInfoType.EndsWith("CheckBox"))
                {
                    fieldType = "Boolean";
                }
                else if (strDisplayInfoType.EndsWith("DatePicker"))
                {
                    fieldType = "Date";
                }
                else
                {
                    switch (oFormElement.DisplayInfo.Type)
                    {
                        default: break;
                    }
                }
            }

            // Set default if picklist
           // Fix for 164620- Prefix was not  avaialble at Registry information while mapping
            if ( (!string.IsNullOrEmpty(_strDropDownItemsSelect)) || (!string.IsNullOrEmpty(_strDisplayData)) || (_DDLPickListDomainID > 0 ))
            { 
                if (string.IsNullOrEmpty(fieldType)) fieldType = "String";
                fieldDefault = "picklist";
            }
            if (string.IsNullOrEmpty(fieldType))
            {
                fieldType = "String";  // Hmmm . . . still unknown
                fieldLabel = string.Empty;
                return;   // Unsupported, for now
            }

            // possible sources
            string strSources;  // EXCEPTION no such concept
            {
                if (fieldDefault == "default" || (fieldDefault == "picklist" && fieldIsRequired == "false"))
                    strSources = "default";
                else
                    strSources = string.Empty;

                strSources += ((strSources.Length > 0) ? "|" : string.Empty) + "map";
                if (fieldType == "Boolean")
                {
                    strSources += ((strSources.Length > 0) ? "|" : string.Empty) + "checkbox";
                }
                else if (fieldType == "Date")
                {
                    strSources += ((strSources.Length > 0) ? "|" : string.Empty) + "date";
                }
                else if (fieldDefault == "picklist")
                {
                    if (fieldIsRequired == "false")
                        fieldDefault = "default";

                    strSources += ((strSources.Length > 0) ? "|" : string.Empty) + "picklist" + "&";
                    if (_strDropDownItemsSelect != string.Empty)
                    {
                        strSources += _strDropDownItemsSelect;
                        if (strSources.IndexOf("ORDER BY", 0, StringComparison.CurrentCultureIgnoreCase) <= 0)
                        {
                            strSources += " ORDER BY value";
                        }
                    } // Fix for 164620- Prefix was not  avaialble at Registry information while mapping
                    else if (_DDLPickListDomainID != 0)
                    {
                        strSources += _DDLPickListDomainID.ToString();
                    }
                    else if (_strDisplayData != string.Empty)
                    {
                        strSources += _strDisplayData;
                    }
                }
                else if (fieldType != "Binary")
                {
                    strSources += ((strSources.Length > 0) ? "|" : string.Empty) + "constant";
                }
              if (fieldType != "Binary") strSources += ((strSources.Length > 0) ? "|" : string.Empty) + "calculation";
            }

            // Hack for legacy
            //Fix for CSBR 166102- Unable to load Legacy data
            if (_dictBindingExpressions.Count == 0)
            {
                if (_vstrLabel != string.Empty && _vstrLabel == "RegNum" && fieldListKey.ToLower() == "mixturecsladatasource")
                {
                    if (_vstrFieldInfo != string.Empty)
                    {
                        _listFieldList.Add(fieldListKey + ";" + _vstrLabel + ";" + _vstrFieldInfo);
                    }
                    if (_vstrBindingExpression != string.Empty)
                    {
                        Dictionary<string, string> listBindingExpressions = new Dictionary<string, string>();
                        _dictBindingExpressions.Add(fieldListKey + "List", listBindingExpressions);
                        listBindingExpressions.Add(_vstrBindingExpression, fieldListKey + ":" + _vstrLabel);
                    }
                }
            }

            // Hack for Batch ID
            //Fix for CSBR 160708 - "BATCH ID does not belong to table OutputTable" error while updating batches
            if (updateformid == 4011 && OutputType.ToString()=="Update batch data in registration") 
            {
                int fieldsCount = 0;
                foreach (string strtemp in _listFieldList)
                {
                    if (strtemp.Contains("Batch ID"))
                    {
                        fieldsCount++;

                    }
                }
                 if (fieldsCount == 0)
                {
                    if (_vstrLabel != string.Empty)
                        {
                            if (_vstrFieldInfo != string.Empty)
                            {
                                _listFieldList.Add(fieldListKey + ";" + _vstrLabel + ";" + _vstrFieldInfo);
                            }
                            if (_vstrBindingExpression != string.Empty)
                            {
                                Dictionary<string, string> listBindingExpressions = new Dictionary<string, string>();
                                _dictBindingExpressions.Add(fieldListKey + "List", listBindingExpressions);
                                listBindingExpressions.Add(_vstrBindingExpression, fieldListKey + ":" + _vstrLabel);
                            }
                        }
                   
                }
            }

            //JED: Hack for Fragments
            if (bindingExpression.ToLower() == "BatchComponentFragmentList".ToLower())
            {
                //create the UI elements
                _listFieldList.Add("fragmentscsladatasource;Fragment 1;String;default;default|map;Fragment1");
                _listFieldList.Add("fragmentscsladatasource;Fragment 1 Equivalents;Decimal;default;default|map;Fragment1Eq");
                _listFieldList.Add("fragmentscsladatasource;Fragment 2;String;default;default|map;Fragment2");
                _listFieldList.Add("fragmentscsladatasource;Fragment 2 Equivalents;Decimal;default;default|map;Fragment2Eq");

                //create the binding expressions
                string strKey = fieldListKey;// +"List";
                Dictionary<string, string> listBingingExpressions = null;
                if (_dictBindingExpressions.ContainsKey(strKey))
                {
                    listBingingExpressions = _dictBindingExpressions[strKey];
                }
                else
                {
                    listBingingExpressions = new Dictionary<string, string>();
                    _dictBindingExpressions.Add(strKey, listBingingExpressions);
                }
                listBingingExpressions.Add("FragmentsList@Fragment1Id", strKey + ":" + "Fragment 1");
                listBingingExpressions.Add("FragmentsList@Fragment1Equivalents", strKey + ":" + "Fragment 1 Equivalents");
                listBingingExpressions.Add("FragmentsList@Fragment2Id", strKey + ":" + "Fragment 2");
                listBingingExpressions.Add("FragmentsList@Fragment2Equivalents", strKey + ":" + "Fragment 2 Equivalents");
            }
            //Hack for identifiers
            else if ((bindingExpression == "IdentifierList") || (bindingExpression.EndsWith(".IdentifierList")))
            {
                IKeyValueListHolder oKeyValueListHolder = null;

                if (!string.IsNullOrEmpty(_strDisplayData))
                    oKeyValueListHolder = (IKeyValueListHolder)COEDisplayDataBroker.GetDisplayData(_strDisplayData);

                if (!string.IsNullOrEmpty(_strDropDownItemsSelect))
                {
                    PickListNameValueList oPickListNameValueList =
                        PickListNameValueList.GetPickListNameValueList(_strDropDownItemsSelect);
                    oKeyValueListHolder = (IKeyValueListHolder)oPickListNameValueList;
                }
                if (oKeyValueListHolder != null)        // Coverity Fix- CBOE-1941
                {
                    foreach (System.Collections.DictionaryEntry de in oKeyValueListHolder.KeyValueList)
                    {
                        string strFieldInfo;

                        strFieldInfo = fieldListKey; // the field group
                        strFieldInfo += ";" + de.Value.ToString();  // fieldLabel
                        strFieldInfo += ";" + "String";  // fieldType
                        strFieldInfo += ";" + "default";  // fieldDefault
                        strFieldInfo += ";" + "default|map|constant|calculation"; // strSources
                        strFieldInfo += ";" + de.Key; // fieldId;
                        //Fix for CSBR -163563- Unhandled exception while loading data when anyone of teh Identifer TYPE is set to ALL
                        int fieldDuplicateCount = 0;
                        foreach (string strtemp in _listFieldList)
                        {
                            if (strtemp.Contains(fieldListKey) && strtemp.Equals(strFieldInfo))
                            {
                                fieldDuplicateCount++;

                            }
                        }
                        if (fieldDuplicateCount == 0)
                        {
                            _listFieldList.Add(strFieldInfo);

                            string strKey = fieldListKey + "List";
                            Dictionary<string, string> listBingingExpressions = null;
                            if (_dictBindingExpressions.ContainsKey(strKey))
                            {
                                listBingingExpressions = _dictBindingExpressions[strKey];
                            }
                            else
                            {
                                listBingingExpressions = new Dictionary<string, string>();
                                _dictBindingExpressions.Add(strKey, listBingingExpressions);
                            }
                            // Add suffix to the binding expression
                            listBingingExpressions.Add(bindingExpression + "@" + de.Key.ToString(), fieldListKey + ":" + de.Value.ToString());  // fieldLabel
                        }
                    }
                }
            }
            else
            {
                // Emit

                string strFieldInfo;
                strFieldInfo = fieldListKey;
                if ((fieldLabel == null) || (fieldLabel == string.Empty))
                {
                    fieldLabel = oFormElement.Name;
                }
                fieldLabel = fieldLabel.Replace('º', 'o'); // WJC TODO need a better fix
                strFieldInfo += ";" + fieldLabel;
                strFieldInfo += ";" + fieldType;
                if (fieldTypeSpecification != string.Empty) strFieldInfo += "(" + fieldTypeSpecification + ")";
                strFieldInfo += ";" + fieldDefault;
                strFieldInfo += ";" + strSources;
                strFieldInfo += ";" + fieldId;
                strFieldInfo += ";" + fieldIsRequired;
                //Fix for CSBR: 164615-Projects were available at Registry information when RLS = Batch
                int fieldCount = 0;
                int _RKRvalue = 0;
                //Fix for CSBR 166046-Prefix should not be available while loading Legacy data.
                string strConfigurationxml = strConfiguration.ToString();
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(strConfigurationxml);
                XmlNode memberNode = xdoc.SelectSingleNode("OutputConfiguration/GroupBox[@member='RegistrySaveAction']");
                if (memberNode != null)
                {
                    string strvalue = memberNode.Attributes["value"].Value;

                    Int32.TryParse(strvalue, out _RKRvalue);
                }
                switch (GetActivaRLSvalue())
                {
                    case "Off":
                        if (_RKRvalue == 2 && strFieldInfo.Contains("mixturecsladatasource") && strFieldInfo.Contains("Prefix"))
                            fieldCount++;
                        break;
                    case "BatchLevelProjects":
                        if (strFieldInfo.Contains("mixturecsladatasource") && strFieldInfo.Contains("Projects"))
                            fieldCount++;
                        if (_RKRvalue == 2 && strFieldInfo.Contains("mixturecsladatasource") && strFieldInfo.Contains("Prefix"))
                            fieldCount++;
                        break;
                    case "RegistryLevelProjects":
                        if (strFieldInfo.Contains("batchlistcsladatasource") && strFieldInfo.Contains("Projects"))
                            fieldCount++;
                        if (_RKRvalue == 2 && strFieldInfo.Contains("mixturecsladatasource") && strFieldInfo.Contains("Prefix"))
                            fieldCount++;
                        break;
                }

                if (fieldCount == 0)
                {
                    _listFieldList.Add(strFieldInfo);

                    // Store binding expression
                    {
                        string strKey = fieldListKey + "List";
                        Dictionary<string, string> listBingingExpressions = null;
                        if (_dictBindingExpressions.ContainsKey(strKey))
                        {
                            listBingingExpressions = _dictBindingExpressions[strKey];
                        }
                        else
                        {
                            listBingingExpressions = new Dictionary<string, string>();
                            _dictBindingExpressions.Add(strKey, listBingingExpressions);
                        }
                        listBingingExpressions.Add(bindingExpression, fieldListKey + ":" + fieldLabel);
                    }
                }
            }
        }

        /// <summary>
        /// Configures control information either for subsequent processing steps
        /// or for serialization as part of a JOB object which can be saved and re-loaded.
        /// </summary>
        /// <param name="oFormElement"></param>
        /// <param name="fieldListKey"></param>
        private void SetFieldConfigInfo(FormGroup.FormElement oFormElement, string fieldListKey)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            XmlDocument oXmlDocument = new XmlDocument();
            oXmlDocument.LoadXml(oFormElement.ConfigInfo.OuterXml);
            XmlNamespaceManager oXmlNamespaceManager = new XmlNamespaceManager(oXmlDocument.NameTable);
            oXmlNamespaceManager.AddNamespace("COE", "COE.FormGroup");
            XmlNode oXmlNodeSelect = oXmlDocument.SelectSingleNode("//COE:dropDownItemsSelect", oXmlNamespaceManager);
            XmlNode oXmlNodePickListDomain = oXmlDocument.SelectSingleNode("//COE:PickListDomain", oXmlNamespaceManager);
            XmlNode oXmlNodeDisplay = oXmlDocument.SelectSingleNode("//COE:displayData", oXmlNamespaceManager);
            _DDLPickListDomainID = 0;
            if ((oXmlNodeSelect != null) && (oXmlNodeSelect.InnerText.ToUpper().StartsWith("SELECT ")))
            {
                _strDropDownItemsSelect = oXmlNodeSelect.InnerText;
            } // Fix for 164620- Prefix was not  avaialble at Registry information while mapping
            else if ((oXmlNodePickListDomain != null))
            {
                Int32.TryParse(oXmlNodePickListDomain.InnerText, out _DDLPickListDomainID);
            }
            else if ((oXmlNodeDisplay != null) && (oXmlNodeDisplay.ChildNodes.Count != 0))
            {
                sb.Append(oXmlNodeDisplay.OuterXml);
            }
            else
            {   // This entire else clause is a workaround for missing displayData sections
                string dataSourceID = oFormElement.DataSourceId;
                if (string.IsNullOrEmpty(dataSourceID))
                {
                    XmlNode dataSourceIdNode = oXmlDocument.SelectSingleNode("//COE:DataSourceID", oXmlNamespaceManager);
                    if (dataSourceIdNode != null)
                        dataSourceID = dataSourceIdNode.InnerText;

                    //Only show the element if it's visible in the submit form
                    if (oFormElement.DisplayInfo.Visible)
                    {
                        if (dataSourceID == "RegistryProjectsCslaDataSource" || dataSourceID == "ProjectsCslaDataSource")
                            sb.Append(GetProjectXmlSpec(ProjectList.ProjectTypeEnum.R));

                        if (dataSourceID == "BatchProjectsCslaDataSource")
                            sb.Append(GetProjectXmlSpec(ProjectList.ProjectTypeEnum.B));
                    }
                }

                string bindingExpression = oFormElement.BindingExpression;
                switch (bindingExpression)
                {
                    case "IdentifierList":
                        {
                            XmlNode dataSourceNode = oXmlDocument.SelectSingleNode("//COE:DataSource", oXmlNamespaceManager);
                            if (dataSourceNode != null)
                                _strDropDownItemsSelect = dataSourceNode.InnerText;
                            else
                                if( fieldListKey == "BatchListCslaDataSource".ToLower())
                                    sb.Append(GetIdentifierXmlSpec(IdentifierTypeEnum.B));
                                else
                                    sb.Append(GetIdentifierXmlSpec(IdentifierTypeEnum.R));
                            break;
                        }
                    case "Compound.IdentifierList":
                        {
                            XmlNode dataSourceNode = oXmlDocument.SelectSingleNode("//COE:DataSource", oXmlNamespaceManager);
                            if (dataSourceNode != null)
                                _strDropDownItemsSelect = dataSourceNode.InnerText;
                            else
                                sb.Append(GetIdentifierXmlSpec(IdentifierTypeEnum.C));
                            break;
                        }
                    case "Compound.BaseFragment.Structure.IdentifierList":
                        {
                            XmlNode dataSourceNode = oXmlDocument.SelectSingleNode("//COE:DataSource", oXmlNamespaceManager);
                            if (dataSourceNode != null)
                                _strDropDownItemsSelect = dataSourceNode.InnerText;
                            else
                                sb.Append(GetIdentifierXmlSpec(IdentifierTypeEnum.S));
                            break;
                        }
                    case "Structure.IdentifierList":
                        {
                            XmlNode dataSourceNode = oXmlDocument.SelectSingleNode("//COE:DataSource", oXmlNamespaceManager);
                            if (dataSourceNode != null)
                                _strDropDownItemsSelect = dataSourceNode.InnerText;
                            else
                                sb.Append(GetIdentifierXmlSpec(IdentifierTypeEnum.S));
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                string sbBuffer = sb.ToString();
                _strDisplayData += sbBuffer;
            }
        }

        private string GetIdentifierXmlSpec(IdentifierTypeEnum type)
        {
            string buf = 
            "<displayData xmlns=\"COE.FormGroup\">" + "\r\n"
            + "  <assembly name=\"" + "CambridgeSoft.COE.Registration.Services" + "\"/>" + "\r\n"
            + "  <class name=\"" + "CambridgeSoft.COE.Registration.Services.Types.IdentifierList" + "\"/>" + "\r\n"
            + "  <method name=\"" + "GetIdentifierListByType" + "\">" + "\r\n"
            + "    <paramList>" + "\r\n"
            + string.Format("      <param type=\"{0}\" value=\"{1}\"/>", "System.String", type.ToString()) + "\r\n"
            + string.Format("      <param type=\"{0}\" value=\"{1}\"/>", "System.Boolean", "True") + "\r\n"
            + string.Format("      <param type=\"{0}\" value=\"{1}\"/>", "System.Boolean", "True") + "\r\n"
            + "    </paramList>" + "\r\n"
            + "  </method>" + "\r\n"
            + "</displayData>" + "\r\n";
            return buf;
        }

        private string GetProjectXmlSpec(ProjectList.ProjectTypeEnum projectType)
        {
            int id = COEUser.ID;
            string buf =
            "<displayData xmlns=\"COE.FormGroup\">" + "\r\n"
            + "  <assembly name=\"" + "CambridgeSoft.COE.Registration.Services" + "\"/>" + "\r\n"
            + "  <class name=\"" + "CambridgeSoft.COE.Registration.Services.Types.ProjectList" + "\"/>" + "\r\n"
            + "  <method name=\"" + "GetActiveProjectListByPersonIDAndType" + "\">" + "\r\n"
            + "    <paramList>" + "\r\n"
            + string.Format("      <param type=\"{0}\" value=\"{1}\"/>", "System.Int32", id.ToString()) + "\r\n"
            + string.Format("      <param type=\"{0}\" value=\"{1}\"/>", "System.String", projectType.ToString()) + "\r\n"
            + string.Format("      <param type=\"{0}\" value=\"{1}\"/>", "System.Boolean", "True") + "\r\n"
            + "    </paramList>" + "\r\n"
            + "  </method>" + "\r\n"
            + "</displayData>" + "\r\n";
            return buf;
        }

        /// <summary>
        /// Converts the collection of bindable fields (and related control configuration data)
        /// into an xml format for later translation into an actual GUI.
        /// </summary>
        private void SetXmlFieldSpec()
        {
            // Convert _listFieldList to xmlFieldSpec
            COEXmlTextWriter oCOEXmlTextWriter = oCOEXmlTextWriter = new COEXmlTextWriter();

            // open fieldlist
            oCOEXmlTextWriter.WriteStartElement("fieldlists");
            string strCurrentSection = string.Empty;
            foreach (string strField in _listFieldList)
            {
                //parse this 'atypically-formatted' string into a more usable format
                string[] strFieldInfo = strField.Split(';');
                string[] strGenericTypeInfo = strFieldInfo[2].Split(new char[] { '(', ')' }, 3);

                string strUiSection = strFieldInfo[0];
                string strUiLabel = strFieldInfo[1];
                string strGenericType = strGenericTypeInfo[0];
                string strGenericTypeSpecification = (strGenericTypeInfo.Length > 1) ? strGenericTypeInfo[1] : string.Empty;
                string strDefaultSource = strFieldInfo[3];
                string strSourceList = strFieldInfo[4];
                string strId = string.Empty;
                string strRequired = "false";

                if (strFieldInfo.Length == 6)
                    strId = strFieldInfo[5];

                if (strFieldInfo.Length == 7)
                    strRequired = strFieldInfo[6];

                if (strCurrentSection != strUiSection)
                {
                    if (strCurrentSection.Length > 0)
                    {
                        // close (previous) fieldlist
                        oCOEXmlTextWriter.WriteEndElement();
                    }
                    strCurrentSection = strUiSection;
                    // open (new fieldlist)
                    oCOEXmlTextWriter.WriteStartElement("fieldlist");
                    {
                        string fieldGroupCaption = strUiSection.ToLower();
                        // (section) name
                        switch (fieldGroupCaption)
                        {
                            case MIXTURE:
                                fieldGroupCaption = "Registration Information"; break;
                            case COMPONENT:
                                fieldGroupCaption = "Component Information"; break;
                            case BATCH:
                                fieldGroupCaption = "Batch Information"; break;
                            case FRAGMENT:
                                fieldGroupCaption = "Fragment Information"; break;
                        }
                        oCOEXmlTextWriter.WriteStartAttribute("name");
                        oCOEXmlTextWriter.WriteString(strUiSection.ToLower());
                        oCOEXmlTextWriter.WriteEndAttribute();
                        oCOEXmlTextWriter.WriteStartAttribute("caption");
                        oCOEXmlTextWriter.WriteString(fieldGroupCaption);
                        oCOEXmlTextWriter.WriteEndAttribute();
                    }
                } //

                // open field
                oCOEXmlTextWriter.WriteStartElement("field");
                // (field) name
                oCOEXmlTextWriter.WriteStartAttribute("name");
                oCOEXmlTextWriter.WriteString(strUiLabel);
                oCOEXmlTextWriter.WriteEndAttribute();
                //(field) key
                oCOEXmlTextWriter.WriteStartAttribute("key");
                oCOEXmlTextWriter.WriteString(strId);
                oCOEXmlTextWriter.WriteEndAttribute();
                // format
                oCOEXmlTextWriter.WriteStartAttribute("format");
                oCOEXmlTextWriter.WriteString(strGenericType);
                oCOEXmlTextWriter.WriteEndAttribute();
                // format specifier
                oCOEXmlTextWriter.WriteStartAttribute("specification");
                oCOEXmlTextWriter.WriteString(strGenericTypeSpecification);
                oCOEXmlTextWriter.WriteEndAttribute();

                // open sources
                oCOEXmlTextWriter.WriteStartElement("sources");

                // default
                oCOEXmlTextWriter.WriteStartAttribute("default");
                oCOEXmlTextWriter.WriteString(strDefaultSource);
                oCOEXmlTextWriter.WriteEndAttribute();

                // required?
                oCOEXmlTextWriter.WriteStartAttribute("required");
                oCOEXmlTextWriter.WriteString(strRequired);
                oCOEXmlTextWriter.WriteEndAttribute();

                string[] strSources = strSourceList.Split('|');
                foreach (string strSource in strSources)
                {
                    // open 'source'
                    string[] strSourceInfo = strSource.Split('&');
                    oCOEXmlTextWriter.WriteStartElement(strSourceInfo[0]);
                    switch (strSourceInfo[0])
                    {
                        case "picklist":    // EXCEPTION
                        {
                            int pickListDomainId;
                            oCOEXmlTextWriter.WriteStartElement("items");
                            string strPicklistInfo = strSourceInfo[1];
                            if (strPicklistInfo.ToUpper().StartsWith("SELECT "))
                            {
                                PickListNameValueList oPickListNameValueList = 
                                    PickListNameValueList.GetPickListNameValueList(strPicklistInfo);

                                foreach (Csla.NameValueListBase<string, string>.NameValuePair nvp in oPickListNameValueList)
                                {
                                    oCOEXmlTextWriter.WriteStartElement("item");
                                    oCOEXmlTextWriter.WriteStartAttribute("value");
                                    oCOEXmlTextWriter.WriteString(nvp.Key.ToString());
                                    oCOEXmlTextWriter.WriteEndAttribute();
                                    oCOEXmlTextWriter.WriteString(nvp.Value);
                                    oCOEXmlTextWriter.WriteEndElement();
                                }
                            }
                            else
                            { // Fix for 164620- Prefix was not  avaialble at Registry information while mapping
                                int.TryParse(strPicklistInfo, out pickListDomainId);
                                if (pickListDomainId > 0)
                                {
                                    strPicklistInfo = PickListNameValueList.GetDropdownSql(pickListDomainId, PickListStatus.Active, null);

                                    PickListNameValueList oPickListNameValueList =
                                        PickListNameValueList.GetPickListNameValueList(strPicklistInfo);

                                    foreach (Csla.NameValueListBase<string, string>.NameValuePair nvp in oPickListNameValueList)
                                    {
                                        oCOEXmlTextWriter.WriteStartElement("item");
                                        oCOEXmlTextWriter.WriteStartAttribute("value");
                                        oCOEXmlTextWriter.WriteString(nvp.Key.ToString());
                                        oCOEXmlTextWriter.WriteEndAttribute();
                                        oCOEXmlTextWriter.WriteString(nvp.Value);
                                        oCOEXmlTextWriter.WriteEndElement();
                                    }
                                }
                                else
                                {
                                    IKeyValueListHolder oKeyValueListHolder = (IKeyValueListHolder)COEDisplayDataBroker.GetDisplayData(strPicklistInfo);
                                    foreach (System.Collections.DictionaryEntry de in oKeyValueListHolder.KeyValueList)
                                    {
                                        oCOEXmlTextWriter.WriteStartElement("item");
                                        oCOEXmlTextWriter.WriteStartAttribute("value");
                                        oCOEXmlTextWriter.WriteString(de.Key.ToString());
                                        oCOEXmlTextWriter.WriteEndAttribute();
                                        oCOEXmlTextWriter.WriteString(de.Value.ToString());
                                        oCOEXmlTextWriter.WriteEndElement();
                                    }
                                }
                            }
                            oCOEXmlTextWriter.WriteEndElement();
                            break;
                        }
                        default:
                            break;  // WJC populate InnerText iif not empty
                    }

                    // close 'source'
                    oCOEXmlTextWriter.WriteEndElement();
                }

                // close sources
                oCOEXmlTextWriter.WriteEndElement();

                // close field
                oCOEXmlTextWriter.WriteEndElement();
            }

            // close (final) fieldlist
            if (strCurrentSection.Length > 0)
                // close (previous) section
                oCOEXmlTextWriter.WriteEndElement();

            // close fieldlists
            oCOEXmlTextWriter.WriteEndElement();

            OutputFieldSpec = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
            oCOEXmlTextWriter.Close();
        }

        #region > Binding methods <

        protected virtual bool BindStart(int vnTransaction, DataRow voOutputDataRow, int totalRows) { return true; }
        protected virtual bool BindAble(string vKey) { return false; }
        protected virtual Object BindObject(string vKey) { return null; }
        protected virtual Object BindObjectByPath(object vParent, string vPath) { return null; }
        // TOTAL KLUGE FOR DAVE LEVY DEMO SLICE
        protected virtual void BindKluge(string vKey, Dictionary<string, string> vBindings) { return; }
        protected virtual void BindWrite(int vnTransaction) { return; }
        protected virtual void BindCommit(int vnTransaction) { return; }
        protected override bool DataSetWrite(int vnRecord)
        {
            DataTable oOutputDataTable = OutputDataSet.Tables[0];
            int cRows = oOutputDataTable.Rows.Count;
            int nTransaction = vnRecord;
            bool returnBindStart;

            foreach (DataRow oOutputDataRow in oOutputDataTable.Rows)
            {
                try
                {
                    nTransaction++;

                    // Creates the RegistryRecord instance as cloned from the template copy
                    returnBindStart = BindStart(nTransaction, oOutputDataRow, cRows);
                    if (returnBindStart == false)
                    {
                        // Map the data provided by the binding constants
                        foreach (KeyValuePair<string, Dictionary<string, string>> kvp in _dictBindingConstants)
                        {
                            Dictionary<string, string> listBindingConstants = new Dictionary<string, string>();
                            Dictionary<string, string> listIdentifierList = new Dictionary<string, string>();
                            Dictionary<string, string> listProjectList = new Dictionary<string, string>();
                            Dictionary<string, string> listFragmentsList = new Dictionary<string, string>();
                            foreach (KeyValuePair<string, string> kvpInner in kvp.Value)
                            {
                                if (kvpInner.Key.Contains("IdentifierList@"))
                                    listIdentifierList.Add(kvpInner.Key, kvpInner.Value);
                                else if (kvpInner.Key == "ProjectList")
                                    listProjectList.Add(kvpInner.Key, kvpInner.Value);
                                else if (kvpInner.Key.Contains("FragmentsList@"))
                                {
                                    //do nothing!
                                }
                                else
                                    listBindingConstants.Add(kvpInner.Key, kvpInner.Value);
                            }

                            if (listBindingConstants.Count > 0)
                            {
                                Object obj = BindObject(kvp.Key);
                                COEDataMapper.Map(listBindingConstants, obj);
                                BindKluge(kvp.Key, listBindingConstants);
                            }

                            if (listIdentifierList.Count > 0)
                            {
                                Object obj = BindObject(kvp.Key);
                                foreach (KeyValuePair<string, string> pair in listIdentifierList)
                                {
                                    if (!string.IsNullOrEmpty(pair.Value) && !string.IsNullOrEmpty(pair.Value.Trim()))
                                    {
                                        string[] bindingDetails = pair.Key.Split('@');
                                        string path = bindingDetails[0];
                                        string identifierId = bindingDetails[1];
                                        Object identifierList = BindObjectByPath(obj, path);
                                        IdentifierList oIdentifierList = (IdentifierList)identifierList;
                                        oIdentifierList.Add(Identifier.NewIdentifier());
                                        oIdentifierList[oIdentifierList.Count - 1].IdentifierID = Convert.ToInt32(identifierId);
                                        oIdentifierList[oIdentifierList.Count - 1].InputText = pair.Value.Trim();
                                    }
                                }
                            }

                            if (listProjectList.Count > 0)
                            {
                                string extension = "ProjectList";
                                Object obj = BindObject(kvp.Key + "@" + extension);
                                ProjectList projects = (ProjectList)obj;

                                projects.Add(Project.NewProject());
                                projects[projects.Count - 1].ProjectID = Convert.ToInt32(kvp.Value[extension]);
                            }
                        }

                        // Map the data provided by the binding variables
                        foreach (KeyValuePair<string, Dictionary<string, string>> kvp in _dictBindingVariables)
                        {
                            Dictionary<string, string> listBindingValues = new Dictionary<string, string>();
                            Dictionary<string, string> listIdentifierList = new Dictionary<string, string>();
                            Dictionary<string, string> listProjectList = new Dictionary<string, string>();
                            foreach (KeyValuePair<string, string> kvpInner in kvp.Value)
                            {
                                // Source of the value
                                Object oValue = oOutputDataRow[kvpInner.Value];
                                // The value as a string
                                string strValue = string.Empty;
                                string valType = oValue.GetType().FullName;

                                if (valType == "System.Byte[]")
                                {
                                    byte[] byteStructure = (byte[])oValue;
                                    strValue = Convert.ToBase64String(byteStructure);
                                }
                                else if (valType == "System.Boolean")
                                {
                                    switch (oValue.ToString().ToLower())
                                    {
                                        case "y":
                                        case "yes":
                                        case "t":
                                        case "true":
                                        case "1":
                                            {
                                                strValue = "true";
                                                break;
                                            }
                                        case "n":
                                        case "no":
                                        case "f":
                                        case "false":
                                        case "0":
                                            {
                                                strValue = "false";
                                                break;
                                            }
                                        default: strValue = "false"; break;
                                    }
                                    bool result;
                                    if (bool.TryParse(oValue.ToString(), out result))
                                        strValue = result.ToString();
                                }
                                else if (valType == "System.DateTime")
                                {
                                    strValue = oValue.ToString();
                                    if (!string.IsNullOrEmpty(strValue))
                                        strValue = Convert.ToDateTime(strValue).ToString(_serverDateFormat, _serverCultureInfo);
                                }
                                else if (valType == "System.Decimal")
                                {
                                    if (oValue != null)
                                        strValue = Convert.ToString(oValue, CultureInfo.CreateSpecificCulture(_serverCultureInfo.Name));
                                }
                                else if (valType == "System.Double")
                                {
                                    if (oValue != null)
                                        strValue = Convert.ToString(oValue, CultureInfo.CreateSpecificCulture(_serverCultureInfo.Name));
                                }
                                else
                                    strValue = oValue.ToString();

                                if (kvpInner.Key.Contains("IdentifierList@"))
                                    listIdentifierList.Add(kvpInner.Key, strValue);
                                else if (kvpInner.Key == "ProjectList")
                                    listProjectList.Add(kvpInner.Key, strValue);
                                else
                                    listBindingValues.Add(kvpInner.Key, strValue);
                            }
                            if (listBindingValues.Count > 0)
                            {
                                Object obj = BindObject(kvp.Key);
                                if (kvp.Key == "fragmentscsladatasource")
                                {
                                    BatchComponentFragmentList bcfList = (BatchComponentFragmentList)obj;
                                    AddBatchComponentFragments(ref bcfList, listBindingValues, oOutputDataRow);
                                }
                                else
                                {
                                    CambridgeSoft.COE.Framework.Controls.COEDataMapper.COEDataMapper.Map(listBindingValues, obj);
                                    BindKluge(kvp.Key, listBindingValues);
                                }
                            }

                            if (listIdentifierList.Count > 0)
                            {
                                Object obj = BindObject(kvp.Key);
                                foreach (KeyValuePair<string, string> pair in listIdentifierList)
                                {
                                    if (!string.IsNullOrEmpty(pair.Value) && !string.IsNullOrEmpty(pair.Value.Trim()))
                                    {
                                        string[] bindingDetails = pair.Key.Split('@');
                                        string path = bindingDetails[0];
                                        string identifierId = bindingDetails[1];
                                        Object identifierList = BindObjectByPath(obj, path);
                                        IdentifierList oIdentifierList = (IdentifierList)identifierList;
                                        oIdentifierList.Add(Identifier.NewIdentifier());
                                        oIdentifierList[oIdentifierList.Count - 1].IdentifierID = Convert.ToInt32(identifierId);
                                        oIdentifierList[oIdentifierList.Count - 1].InputText = pair.Value.Trim();
                                    }
                                }
                            }

                            if (listProjectList.Count > 0)
                            {
                                string extension = "ProjectList";
                                Object obj = BindObject(kvp.Key + "@" + extension);
                                ProjectList projects = (ProjectList)obj;
                                projects.Add(Project.NewProject());
                                if (kvp.Key == "batchlistcsladatasourceList")
                                    projects[projects.Count - 1].ProjectID = Convert.ToInt32(oOutputDataRow["BatchListCslaDataSource:Projects"]);
                                else
                                    projects[projects.Count - 1].ProjectID = Convert.ToInt32(oOutputDataRow["mixturecsladatasource:Projects"]);
                            }
                        }

                        // If there are and errors reported, exclude these rows from processing by
                        // throwing an exception...Otherwise, add the RegistryRecord to the queue
                        // via the BindWrite() method.
                        if (oOutputDataRow.HasErrors)
                        {
                            string err = string.Empty;
                            foreach (DataColumn dc in oOutputDataRow.GetColumnsInError())
                            {
                                if (string.IsNullOrEmpty(err))
                                    err = oOutputDataRow.GetColumnError(dc).Trim();
                                else
                                    err += (" | " + oOutputDataRow.GetColumnError(dc).Trim());
                            }
                            throw new Exception(err);
                        }
                        else
                        {
                            LoadableRecordIndices.Add(nTransaction);
                            BindWrite(nTransaction);
                        }

                    }
                    else
                    {
                        if (batchId == "NoBatchID")
                        {
                            AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, nTransaction, _fmtNoSuchBatchId, new string[] { batchOutputDataRow.ToString() });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Logging all errors here allows us to trap the index of the errant record
                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output
                        , nTransaction, _fmtGenericError, new string[] { ex.Message }
                    );
                }
            }

            // Finally, send the queued records to the repository via the RegistrationLoadList
            // Csla business object (the collection of RegistryRecord objects)
            BindCommit(nTransaction);


            return HasMessages;
        }

        protected override bool StartWrite()
        {
            ClearMessages();
            do
            {
                _dictBindingConstants = new Dictionary<string, Dictionary<string, string>>();
                _dictBindingVariables = new Dictionary<string, Dictionary<string, string>>();
                // #1 Map constant binds and save calculations and mappings
                foreach (KeyValuePair<string, Dictionary<string, string>> kvp in _dictBindingExpressions)
                {
                    Dictionary<string, string> listBindingConstants = new Dictionary<string, string>();
                    Dictionary<string, string> listBindingVariables = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> kvpInner in kvp.Value)
                    {
                        if (kvpInner.Value.Length == 0) continue;   // Shouldn't really happen
                        if (IsMapped(kvpInner.Value))
                        {
                            listBindingVariables.Add(kvpInner.Key, kvpInner.Value); // mapped
                        }
                        else if (IsMapped("=" + kvpInner.Value))
                        {
                            listBindingVariables.Add(kvpInner.Key, kvpInner.Value); // calculation
                        }
                        else if (IsConstant(kvpInner.Value))
                        {
                            string strConstant = ConstantValue(kvpInner.Value);
                            listBindingConstants.Add(kvpInner.Key, strConstant);    // constant
                        }
                    } // foreach (KeyValuePair<string,string> kvpInner in kvp.Value)
                    if (listBindingVariables.Count > 0)
                    {
                        _dictBindingVariables.Add(kvp.Key, listBindingVariables);
                    }
                    if (listBindingConstants.Count > 0)
                    {
                        _dictBindingConstants.Add(kvp.Key, listBindingConstants);
                    }
                } // foreach (KeyValuePair<string, Dictionary<string,string>> kvp in _dictBindingExpressions)
            } while (false);
            return HasMessages;
        }

        public override bool EndWrite()
        {
            ClearMessages();
            // WJC cleanup
            return HasMessages;
        }

        /// <summary>
        /// Generates BatchComponentFragmentList elements for the zeroeth Batch's first BatchComponent.
        /// </summary>
        /// <param name="bcfList">the BatchComponentFragmentList corresponding to the 'current' RegistryRecord</param>
        /// <param name="listBindingValues">the list of all possible fields to be mapped</param>
        /// <param name="oOutputDataRow">the row of data from which the 'current' RegistryRecord is derived</param>
        private void AddBatchComponentFragments(
            ref BatchComponentFragmentList bcfList
            , Dictionary<string, string> listBindingValues
            , DataRow oOutputDataRow
            )
        {
            string idKeyFormat = "FragmentsList@Fragment{0}Id";
            string eqKeyFormat = "FragmentsList@Fragment{0}Equivalents";
            string idColumnFormat = "fragmentscsladatasource:Fragment {0}";
            string eqColumnFormat = "fragmentscsladatasource:Fragment {0} Equivalents";
            int[] fragmentIndex = new int[] { 1, 2 };
            //decimal[] fragmentEqs = new decimal[] { -1, -1 };
            foreach (int index in fragmentIndex)
            {
                string idKey = string.Format(idKeyFormat, index);
                string eqKey = string.Format(eqKeyFormat, index);
                string idColumn = string.Format(idColumnFormat, index);
                string eqColumn = string.Format(eqColumnFormat, index);

                if (listBindingValues.ContainsKey(idKey))
                {
                    object fragmentIdentifier = oOutputDataRow[idColumn];
                    // * A 'null' value from a DataTable is System.DBNull!
                    if (!IsNullValue(fragmentIdentifier))
                    {
                        Fragment matchedFrag = FindMatchingFragment(fragmentIdentifier);
                        if (matchedFrag != null)
                        {
                            if (listBindingValues.ContainsKey(eqKey))
                            {
                                object fragmentEquivalents = oOutputDataRow[eqColumn];
                                float equivalents = 0;
                                if (fragmentEquivalents != null)
                                    if (!float.TryParse(fragmentEquivalents.ToString(), out equivalents))
                                        fragmentEquivalents = 0;
                                    else
                                        fragmentEquivalents = equivalents;

                                //generate the fragment data; make the business object to validate the equivalents value
                                BatchComponentFragment item =
                                    BatchComponentFragment.NewBatchComponentFragment(matchedFrag, equivalents);
                                bcfList.Add(item);
                            }
                        }
                        else
                        {
                            //Don't throw an exception, simply add a column error to the DataRow;
                            //  downstream processing will handle this properly.
                            oOutputDataRow.SetColumnError(
                                idColumn, string.Format("Unable to locate fragment '{0}'", fragmentIdentifier)
                            );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Given either a numeric identifier or a string descriptor, gets a matching Fragment from the
        /// cached FragmentList 'dictionary'.
        /// </summary>
        /// <remarks>
        /// The order of search operations is: 'code', 'description', 'formula' and finally 'id'
        /// </remarks>
        /// <param name="fragmentIndicator">an object corresponding to a Fragment key descriptor</param>
        /// <returns>a Fragment object</returns>
        private Fragment FindMatchingFragment(object fragmentIndicator)
        {
            Fragment frag = null;
            int fragmentId;
            string propertyMatch = fragmentIndicator.ToString().ToUpper().Trim();

            if (FragmentIdByCode.TryGetValue(propertyMatch, out fragmentId))
                frag = FragmentCollection.GetByID(fragmentId);
            else if (FragmentIdByDescription.TryGetValue(propertyMatch, out fragmentId))
                frag = FragmentCollection.GetByID(fragmentId);
            else if (FragmentIdByFormula.TryGetValue(propertyMatch, out fragmentId))
                frag = FragmentCollection.GetByID(fragmentId);

            if (frag == null)
                if (int.TryParse(fragmentIndicator.ToString(), out fragmentId))
                    frag = FragmentCollection.GetByID(fragmentId);

            return frag;
        }

        #endregion

    }
}
