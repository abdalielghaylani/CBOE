using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.Common.Messaging;
using System.Data;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.COEDisplayDataBrokerService;
using CambridgeSoft.COE.Framework.COEPickListPickerService;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Common;
using CambridgeSoft.COE.Registration.Services.Common;

namespace CambridgeSoft.NCDS_DataLoader.Common
{
    public class OutputObjectForm
    {
        public const string MIXTURE = "mixturecsladatasource";
        public const string COMPONENT = "componentlistcsladatasource";
        public const string BATCH = "batchlistcsladatasource";
        public const string FRAGMENT = "fragmentscsladatasource";
        private string _vstrLabel;
        private string _vstrFieldInfo;
        private string _vstrBindingExpression;
        private string _strDropDownItemsSelect;
        private string _strDisplayData;
        private bool _haveStructure;
        private Dictionary<string, Dictionary<string, string>> _dictBindingExpressions;
        private List<string> _listFieldList;
        private string _OutputFieldSpec;
        private bool _noStructureDupCheck = false;
        private string[] _dupCheckMechanism;

        public Dictionary<string, Dictionary<string, string>> BindingExpressions
        {
            get { return _dictBindingExpressions; }
        }

        public bool NoStructureDupCheck
        {
            get { return _noStructureDupCheck; }
        }


        public string ConfigurationSet(
            string value
            , FormGroup.DisplayMode vnMode
            , string vstrLabel
            , string vstrFieldInfo
            , string vstrBindingExpression
            , bool haveStructure
        )
        {
            _OutputFieldSpec = string.Empty;
            _vstrLabel = vstrLabel;
            _vstrFieldInfo = vstrFieldInfo;
            _vstrBindingExpression = vstrBindingExpression;
            _haveStructure = haveStructure;
            _dictBindingExpressions = new Dictionary<string, Dictionary<string, string>>();
            _listFieldList = new List<string>();


            NCDSDataLoaderService.NCDSDataLoaderService service = new CambridgeSoft.NCDS_DataLoader.NCDSDataLoaderService.NCDSDataLoaderService();
            service.Url = System.Configuration.ConfigurationManager.AppSettings["NCDSDataLoaderUrl"].ToString();
            _dupCheckMechanism = service.GetNonStructuralDuplicateCheckSettings();

            COEFormBO oCOEFormBO = COEFormBO.Get(4015);
            if (oCOEFormBO != null)
            {
                foreach (FormGroup.Display oDisplay in oCOEFormBO.COEFormGroup.DetailsForms.Displays)
                {
                    SetDisplay(oDisplay, vnMode);
                }
            }
            return _OutputFieldSpec;
        }

        /// <summary>
        /// Determines which 'form' elements from a COEForm can be bound to incoming data.
        /// </summary>
        /// <param name="oDisplay"></param>
        /// <param name="vnMode"></param>
        private void SetDisplay(FormGroup.Display oDisplay, FormGroup.DisplayMode vnMode)
        {
            //only extract form information if it is considered 'bindable'
            foreach (FormGroup.Form oForm in oDisplay.Forms)
            {
                //_strForm = string.Empty;
                //if (!string.IsNullOrEmpty(oForm.Title))
                //    _strForm = oForm.Title.Trim();

                //JED: use the DataSourceId instead of the form title as a Binding indicator
                string formKey = oForm.DataSourceId;

                if (oForm.FormDisplay.Visible == true && BindAble(formKey) == true)
                    SetForm(oForm, vnMode, formKey.ToLower());
            }

            SetXmlFieldSpec();

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
        {
            //distinguish between 'add' and 'edit' mode for form-field specifications
            List<FormGroup.FormElement> oFormElementList = null;
            if (vnMode == FormGroup.DisplayMode.Add)
                oFormElementList = oForm.AddMode;
            else if (vnMode == FormGroup.DisplayMode.Edit)
                oFormElementList = oForm.EditMode;

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
                if (strDisplayInfoType.EndsWith(".COEChemDraw"))
                {
                    fieldType = "Binary";
                    fieldDefault = "map";
                    fieldLabel = "Structure";
                }
                else if (strDisplayInfoType.EndsWith(".COEFragments"))
                {
                    fieldType = "Integer";
                }
                else if (strDisplayInfoType.EndsWith(".COECheckBox"))
                {
                    fieldType = "Boolean";
                }
                else if (strDisplayInfoType.EndsWith(".COEDatePicker"))
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
            if ((!string.IsNullOrEmpty(_strDropDownItemsSelect)) || (!string.IsNullOrEmpty(_strDisplayData)))
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
                //if (fieldType != "Binary") strSources += ((strSources.Length > 0) ? "|" : string.Empty) + "calculation";
            }

            // Hack for legacy
            if (_dictBindingExpressions.Count == 0)
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
                {
                    oKeyValueListHolder = (IKeyValueListHolder)COEDisplayDataBroker.
                        GetDisplayData(_strDisplayData);
                }

                if (!string.IsNullOrEmpty(_strDropDownItemsSelect))
                {
                    PickListNameValueList oPickListNameValueList =
                        PickListNameValueList.GetPickListNameValueList(_strDropDownItemsSelect);
                    oKeyValueListHolder = (IKeyValueListHolder)oPickListNameValueList;
                }

                foreach (System.Collections.DictionaryEntry de in oKeyValueListHolder.KeyValueList)
                {
                    string strFieldInfo;

                    strFieldInfo = fieldListKey; // the field group
                    strFieldInfo += ";" + de.Value.ToString();  // fieldLabel
                    strFieldInfo += ";" + "String";  // fieldType
                    // No structure duplicate check
                    string nameValue = string.Empty;
                    string[] name = de.Value.ToString().Split('\'');
                    if (name.Length == 3)
                    {
                        nameValue = name[1];
                    }
                    if ((_dupCheckMechanism.Length == 2 &&
                        (de.Value.ToString().ToLower() == _dupCheckMechanism[1].ToLower() ||
                        nameValue.ToLower() == _dupCheckMechanism[1].ToLower())) &&
                        _haveStructure == false)
                    {
                        strFieldInfo += ";" + "map";  // fieldDefault
                        strFieldInfo += ";" + "map"; // strSources
                        strFieldInfo += ";" + "true"; // strSources
                        _noStructureDupCheck = true;
                    }
                    else
                    {
                        strFieldInfo += ";" + "default";  // fieldDefault
                        //strFieldInfo += ";" + "default|map|constant|calculation"; // strSources
                        strFieldInfo += ";" + "default|map|constant"; // strSources
                        strFieldInfo += ";" + de.Key; // fieldId;
                    }
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
            else
            {
                // Emit
                {
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

                    string nameValue = string.Empty;
                    string[] name = bindingExpression.Split('\'');
                    if (name.Length == 3)
                    {
                        nameValue = name[1];
                    }
                    if (fieldLabel.ToLower() == "Structure".ToLower() && _haveStructure == false)
                    {
                        strFieldInfo += ";" + "default";
                        //strFieldInfo += ";" + "default|map|constant|calculation";
                        strFieldInfo += ";" + "default|map|constant";
                    }
                    // No structure duplicate check
                    else if ((_dupCheckMechanism.Length == 2 &&
                        (bindingExpression.ToLower() == _dupCheckMechanism[1].ToLower() ||
                        nameValue.ToLower() == _dupCheckMechanism[1].ToLower())) &&
                        _haveStructure == false)
                    {
                        strFieldInfo += ";" + "map";  // fieldDefault
                        strFieldInfo += ";" + "map"; // strSources
                        strFieldInfo += ";" + "true"; // strSources
                        _noStructureDupCheck = true;
                    }
                    else
                    {
                        strFieldInfo += ";" + fieldDefault;
                        strFieldInfo += ";" + strSources;
                    }
                    strFieldInfo += ";" + fieldId;
                    strFieldInfo += ";" + fieldIsRequired;
                    _listFieldList.Add(strFieldInfo);
                }

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

        /// <summary>
        /// Configures control information either for subsequent processing steps
        /// or for serialization as part of a JOB object which can be saved and re-loaded.
        /// </summary>
        /// <param name="oFormElement"></param>
        /// <param name="fieldListKey"></param>
        private void SetFieldConfigInfo(FormGroup.FormElement oFormElement, string fieldListKey)
        {
            XmlDocument oXmlDocument = new XmlDocument();
            oXmlDocument.LoadXml(oFormElement.ConfigInfo.OuterXml);
            XmlNamespaceManager oXmlNamespaceManager = new XmlNamespaceManager(oXmlDocument.NameTable);
            oXmlNamespaceManager.AddNamespace("COE", "COE.FormGroup");
            XmlNode oXmlNodeSelect = oXmlDocument.SelectSingleNode("//COE:dropDownItemsSelect", oXmlNamespaceManager);
            XmlNode oXmlNodeDisplay = oXmlDocument.SelectSingleNode("//COE:displayData", oXmlNamespaceManager);

            if ((oXmlNodeSelect != null) && (oXmlNodeSelect.InnerText.ToUpper().StartsWith("SELECT ")))
            {
                _strDropDownItemsSelect = oXmlNodeSelect.InnerText;
            }
            else if ((oXmlNodeDisplay != null) && (oXmlNodeDisplay.ChildNodes.Count != 0))
            {
                _strDisplayData = oXmlNodeDisplay.OuterXml;
            }
            else
            {   // This entire else clause is a workaround for missing displayData sections
                string dataSourceID = oFormElement.DataSourceId;
                if (string.IsNullOrEmpty(dataSourceID))
                {
                    XmlNode dataSourceIdNode = oXmlDocument.SelectSingleNode("//COE:DataSourceID", oXmlNamespaceManager);
                    if (dataSourceIdNode != null)
                        dataSourceID = dataSourceIdNode.InnerText;

                    if (dataSourceID == "ProjectsCslaDataSource")
                    {
                        _strDisplayData += "<displayData xmlns=\"COE.FormGroup\">" + "\r\n";
                        _strDisplayData += "  <assembly name=\"" + "CambridgeSoft.COE.Registration.Services" + "\"/>" + "\r\n";
                        _strDisplayData += "  <class name=\"" + "CambridgeSoft.COE.Registration.Services.Types.ProjectList" + "\"/>" + "\r\n";
                        _strDisplayData += "  <method name=\"" + "GetProjectList" + "\">" + "\r\n";
                        _strDisplayData += "    <paramList/>" + "\r\n";
                        _strDisplayData += "  </method>" + "\r\n";
                        _strDisplayData += "</displayData>" + "\r\n";
                    }
                }

                string bindingExpression = oFormElement.BindingExpression;
                if (bindingExpression == "Compound.IdentifierList")
                {
                    XmlNode dataSourceNode = oXmlDocument.SelectSingleNode("//COE:DataSource", oXmlNamespaceManager);
                    if (dataSourceNode != null)
                        _strDropDownItemsSelect = dataSourceNode.InnerText;
                    else
                    {
                        _strDisplayData += "<displayData xmlns=\"COE.FormGroup\">" + "\r\n";
                        _strDisplayData += "  <assembly name=\"" + "CambridgeSoft.COE.Registration.Services" + "\"/>" + "\r\n";
                        _strDisplayData += "  <class name=\"" + "CambridgeSoft.COE.Registration.Services.Types.IdentifierList" + "\"/>" + "\r\n";
                        _strDisplayData += "  <method name=\"" + "GetIdentifierList" + "\">" + "\r\n";
                        _strDisplayData += "    <paramList/>" + "\r\n";
                        _strDisplayData += "  </method>" + "\r\n";
                        _strDisplayData += "</displayData>" + "\r\n";
                    }
                }
                else if (bindingExpression == "IdentifierList"
                    && fieldListKey != "BatchListCslaDataSource".ToLower()
                    )
                {
                    XmlNode dataSourceNode = oXmlDocument.SelectSingleNode("//COE:DataSource", oXmlNamespaceManager);
                    if (dataSourceNode != null)
                        _strDropDownItemsSelect = dataSourceNode.InnerText;
                    else
                    {
                        //JED: Batch Identifiers are not yet supported via the form
                        _strDisplayData += "<displayData xmlns=\"COE.FormGroup\">" + "\r\n";
                        _strDisplayData += "  <assembly name=\"" + "CambridgeSoft.COE.Registration.Services" + "\"/>" + "\r\n";
                        _strDisplayData += "  <class name=\"" + "CambridgeSoft.COE.Registration.Services.Types.IdentifierList" + "\"/>" + "\r\n";
                        _strDisplayData += "  <method name=\"" + "GetIdentifierList" + "\">" + "\r\n";
                        _strDisplayData += "    <paramList/>" + "\r\n";
                        _strDisplayData += "  </method>" + "\r\n";
                        _strDisplayData += "</displayData>" + "\r\n";
                    }
                }
            }
        }


        protected bool BindAble(string vKey)
        {
            bool isBindable = false;
            switch (vKey.ToLower())
            {
                case MIXTURE:
                    isBindable = true;
                    break;
                case COMPONENT:
                    isBindable = true;
                    break;
                case BATCH:
                    isBindable = true;
                    break;
                case FRAGMENT:
                    isBindable = true;
                    break;
                default:
                    break;
            }
            return isBindable;
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

            _OutputFieldSpec = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
            oCOEXmlTextWriter.Close();
        }



    }
}
