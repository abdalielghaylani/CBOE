using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace CambridgeSoft.COE.Framework.Common.Messaging
{
    [Serializable]
    [XmlRoot("formGroup", Namespace = "COE.FormGroup")]
    public class FormGroup
    {
        #region Variables
        Int32 _id;
        Int32 _dataviewId;

        DisplayCollection _queryForms;
        DisplayCollection _detailsForms;
        DisplayCollection _listForms;

        string _style;
        string _stylesSheet;
        private ScriptInfo _script;

        private bool _enableValidationSummary;
        private string _validationSummaryCssClass;

        #endregion

        #region Properties
        [XmlAttribute("id")]
        public Int32 Id
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }

        [XmlAttribute("dataViewId")]
        public Int32 DataViewId
        {
            get
            {
                return this._dataviewId;
            }
            set
            {
                this._dataviewId = value;
            }
        }

        [XmlElement("queryForms")]
        public DisplayCollection QueryForms
        {
            get
            {
                if(this._queryForms == null)
                    this._queryForms = new DisplayCollection();

                return this._queryForms;
            }
            set
            {
                this._queryForms = value;
            }
        }

        [XmlElement("detailsForms")]
        public DisplayCollection DetailsForms
        {
            get
            {
                if(_detailsForms == null)
                    this._detailsForms = new DisplayCollection();

                return this._detailsForms;
            }
            set
            {
                this._detailsForms = value;
            }
        }


        [XmlElement("style")]
        public string Style
        {
            get
            {
                return this._style;
            }
            set
            {
                this._style = value;
            }
        }
        [XmlElement("styleSheet")]
        public string StyleSheet
        {
            get
            {
                return this._stylesSheet;
            }
            set
            {
                this._stylesSheet = value;
            }
        }

        [XmlElement("script")]
        public ScriptInfo Script
        {
            get { return _script; }
            set { _script = value; }
        }
	

        [XmlElement("listForms")]
        public DisplayCollection ListForms
        {
            get
            {
                if(this._listForms == null)
                    this._listForms = new DisplayCollection();

                return this._listForms;
            }
            set
            {
                this._listForms = value;
            }
        }

        [XmlElement("validationSummaryCssClass")]
        public string ValidationSummaryCssClass
        {
            get { return _validationSummaryCssClass; }
            set { _validationSummaryCssClass = value; }
        }

        [XmlAttribute("enableValidationSummary")]
        public bool EnableValidationSummary
        {
            get { return _enableValidationSummary; }
            set { _enableValidationSummary = value; }
        }

        #endregion

        #region Methods
        #endregion

        #region Factory Methods
        public static FormGroup GetFormGroup(string xml)
        {
            return Utilities.XmlDeserialize<FormGroup>(xml);
        }

        public override string ToString()
        {
            return Utilities.XmlSerialize(this);
        }
        #endregion

        #region Additional Classes

        [Serializable]
        public class DisplayCollection
        {
            #region Variables
            private int _defaultForm;
            List<Display> _displays;
            #endregion

            #region Properties
            [XmlAttribute("defaultForm")]
            public int DefaultForm
            {
                get { return _defaultForm; }
                set { _defaultForm = value; }
            }

            [XmlElement("queryForm", typeof(QueryDisplay))]
            [XmlElement("listForm", typeof(ListDisplay))]
            [XmlElement("detailsForm", typeof(DetailsDisplay))]
            public List<Display> Displays
            {
                get
                {
                    if(this._displays == null)
                        this._displays = new List<Display>();

                    return this._displays;
                }
                set
                {
                    this._displays = value;
                }
            }
            #endregion

            #region Methods

            public Display this[int index]
            {
                get
                {
                    return this.Displays[index];
                }
                set
                {
                    this.Displays[index] = value;
                }
            }

            public void Add(Display item)
            {
                this.Displays.Add(item);
            }

            public void Remove(Display item)
            {
                this.Displays.Remove(item);
            }

            public void RemoveAt(int index)
            {
                this.Displays.RemoveAt(index);
            }
            #endregion
        }

        [Serializable]
        public class Display
        {
            #region Variables
            int id;
            string name;
            string dataSourceId;
            string displayCulture;
            List<Form> _forms;
            DisplayMode _defaultDisplayMode;
            #endregion

            #region Properties
            [XmlAttribute("id")]
            public int Id
            {
                get
                {
                    return this.id;
                }
                set
                {
                    this.id = value;
                }
            }

            [XmlAttribute("name")]
            public string Name
            {
                get
                {
                    return this.name;
                }
                set
                {
                    this.name = value;
                }
            }

            [XmlAttribute("dataSourceId")]
            public string DataSourceId
            {
                get
                {
                    return this.dataSourceId;
                }
                set
                {
                    this.dataSourceId = value;
                }
            }

            [XmlAttribute("displayCulture")]
            public string DisplayCulture
            {
                get { return displayCulture; }
                set { displayCulture = value; }
            }

            [XmlAttribute("defaultDisplayMode")]
            public DisplayMode DefaultDisplayMode
            {
                get
                {
                    return _defaultDisplayMode;
                }
                set
                {
                    _defaultDisplayMode = value;
                }
            }

            [XmlArray("coeForms")]
            [XmlArrayItem("coeForm")]
            public List<Form> Forms
            {
                get
                {
                    if(this._forms == null)
                        this._forms = new List<Form>();

                    return this._forms;
                }
                set
                {
                    this._forms = value;
                }
            }
            public Form this[string name]
            {
                get
                {
                    foreach(Form currentForm in this._forms)
                        if(currentForm.Name == name)
                            return currentForm;

                    return null;
                }
            }
            #endregion
        }

        [Serializable()]
        public class QueryDisplay : Display
        {
        }

        [Serializable()]
        public class DetailsDisplay : Display
        {
            #region Variables
            ResultsCriteria _resultCriteria;
            #endregion

            #region Properties
            [XmlElement("resultsCriteria", Namespace = "COE.ResultsCriteria")]
            public ResultsCriteria ResultsCriteria
            {
                get
                {
                    return this._resultCriteria;
                }
                set
                {
                    this._resultCriteria = value;
                }
            }
            #endregion
        }

        [Serializable()]
        public class ListDisplay : Display
        {
            #region Variables
            ResultsCriteria _resultCriteria;
            #endregion

            #region Properties
            [XmlElement("resultsCriteria", Namespace = "COE.ResultsCriteria")]
            public ResultsCriteria ResultsCriteria
            {
                get
                {
                    return this._resultCriteria;
                }
                set
                {
                    this._resultCriteria = value;
                }
            }
            #endregion
        }

        [Serializable()]
        [XmlRoot("formElement", Namespace = "COE.FormGroup")]
        public class FormElement : ISerializable
        {
            public FormElement()
            {
                _bindingExpression = null;
                _clientEvents = null;
                _configInfo = null;
                _dataSource = string.Empty;
                _dataSourceID = string.Empty;
                _defaultValue = string.Empty;
                _displayInfo = null;
                _id = string.Empty;
                _label = null;
                _name = string.Empty;
                _searchCriteriaItem = null;
                _serverEvents = null;
                _validationRuleList = null;
                _showHelp = false;
                _helpText = string.Empty;
                _isFileUpload = false;
                _pageComunicationProvider = string.Empty;
                _fileUploadBindingExpression = string.Empty;
                _requiredStyle = string.Empty;
            }

            #region variables
            string _label;
            string _defaultValue;
            string _bindingExpression;
            string _id;
            string _helpText;
            bool _showHelp;
            DisplayInfo _displayInfo;
            DisplayData _displayData;
            List<ValidationRuleInfo> _validationRuleList;
            SearchCriteria.SearchCriteriaItem _searchCriteriaItem;
            XmlNode _configInfo;
            string _dataSource;
            string _dataSourceID;
            string _name;
            List<COEEventInfo> _clientEvents;
            List<COEEventInfo> _serverEvents;
            bool _isFileUpload;
            string _pageComunicationProvider;
            string _fileUploadBindingExpression;
            string _requiredStyle;
            string _requiredLabelStyle;

            #endregion

            #region properties
            [XmlAttribute("name")]
            public string Name
            {
                get
                {
                    return this._name;
                }
                set
                {
                    this._name = value;
                }
            }

            [XmlElement("label")]
            public string Label
            {
                get
                {
                    return this._label;
                }
                set
                {
                    this._label = value;
                }
            }

            [XmlElement("showHelp")]
            public bool ShowHelp
            {
                get
                {
                    return this._showHelp;
                }
                set
                {
                    this._showHelp = value;
                }
            }

            [XmlElement("isFileUpload")]
            public bool IsFileUpload
            {
                get
                {
                    return this._isFileUpload;
                }
                set
                {
                    this._isFileUpload = value;
                }
            }

            [XmlElement("pageComunicationProvider")]
            public string PageComunicationProvider
            {
                get
                {
                    return this._pageComunicationProvider;
                }
                set
                {
                    this._pageComunicationProvider = value;
                }
            }

            [XmlElement("fileUploadBindingExpression")]
            public string FileUploadBindingExpression
            {
                get
                {
                    return this._fileUploadBindingExpression;
                }
                set
                {
                    this._fileUploadBindingExpression = value;
                }
            }

            [XmlElement("helpText")]
            public string HelpText
            {
                get
                {
                    return this._helpText;
                }
                set
                {
                    this._helpText = value;
                }
            }

            [XmlElement("defaultValue")]
            public string DefaultValue
            {
                get
                {
                    return this._defaultValue;
                }
                set
                {
                    this._defaultValue = value;
                }
            }

            [XmlElement("bindingExpression")]
            public string BindingExpression
            {
                get
                {
                    return this._bindingExpression;
                }
                set
                {
                    this._bindingExpression = value;
                }
            }

            [XmlElement("Id")]
            public string Id
            {
                get
                {
                    return this._id;
                }
                set
                {
                    this._id = value;
                }
            }

            [XmlElement("displayInfo")]
            public DisplayInfo DisplayInfo
            {
                get
                {
                    if(this._displayInfo == null)
                        this._displayInfo = new DisplayInfo();

                    return this._displayInfo;
                }
                set
                {
                    this._displayInfo = value;
                }
            }

            /// <summary>
            /// List of client validations to be performed.
            /// </summary>
            [XmlArray("validationRuleList")]
            [XmlArrayItem("validationRule")]
            public List<ValidationRuleInfo> ValidationRuleList
            {
                get
                {
                    if(this._validationRuleList == null)
                        _validationRuleList = new List<ValidationRuleInfo>();

                    return _validationRuleList;
                }
                set { _validationRuleList = value; }
            }

            /// <summary>
            /// Server events' subscriptions.
            /// </summary>
            [XmlArray("serverEvents")]
            [XmlArrayItem("event")]
            public List<COEEventInfo> ServerEvents
            {
                get { return _serverEvents; }
                set { _serverEvents = value; }
            }

            /// <summary>
            /// Client events' subscriptions.
            /// </summary>
            [XmlArray("clientEvents")]
            [XmlArrayItem("event")]
            public List<COEEventInfo> ClientEvents
            {
                get { return _clientEvents; }
                set { _clientEvents = value; }
            }

            [XmlAnyElement("configInfo")]
            public XmlNode ConfigInfo
            {
                get
                {
                    return this._configInfo;
                }
                set
                {
                    this._configInfo = value;
                }
            }

            [XmlElement("dataSource")]
            public string DataSource
            {
                get { return _dataSource; }
                set { _dataSource = value; }
            }

            [XmlElement("dataSourceId")]
            public string DataSourceId
            {
                get { return _dataSourceID; }
                set { _dataSourceID = value; }
            }


            /// <summary>
            /// Holds the CSS style for control's label that implement ICOELabelable
            /// The style string accepts a comma separated list of css attibutes that allow the customization of 
            /// the Labelable control's appearence
            /// Examples of LabelStyle value:
            /// Set the label's color to red and add a red border to it 
            /// color: red; border: 1px solid red; 
            /// </summary>
            [XmlElement("requiredLabelStyle")]
            public string RequiredLabelStyle
            {
                get { return _requiredLabelStyle; }
                set { _requiredLabelStyle = value; }
            }

            /// <summary>
            /// Holds the CSS style for control's that implement ICOERequireable
            /// The style string accepts an extended syntax for css attibutes that allow the customization of 
            /// the Requireable control's appearence
            /// Examples of RequiredStyle value:
            /// Set the control's color to red and add a red border to it 
            /// color: red; border: 1px solid red; 
            /// </summary>
            [XmlElement("requiredStyle")]
            public string RequiredStyle
            {
                get { return _requiredStyle; }
                set { _requiredStyle = value; }
            }


            [XmlElement("searchCriteriaItem")]
            public SearchCriteria.SearchCriteriaItem SearchCriteriaItem
            {
                get
                {
                    return this._searchCriteriaItem;
                }
                set
                {
                    this._searchCriteriaItem = value;
                }
            }

            [XmlElement("displayData")]
            public DisplayData DisplayData
            {
                get
                {
                    if(this._displayData == null)
                        this._displayData = new DisplayData();

                    return this._displayData;
                }
                set
                {
                    this._displayData = value;
                }
            }
            #endregion

            #region methods
            public static FormElement GetFormElement(string xml)
            {
                return Utilities.XmlDeserialize<FormElement>(xml);
            }

            public override string ToString()
            {
                return Utilities.XmlSerialize(this);
            }
            #endregion

            #region ISerializable Members
            protected FormElement(SerializationInfo info, StreamingContext ctx)
            {
                _bindingExpression = info.GetString("bindingExpression");
                _clientEvents = (List<COEEventInfo>) info.GetValue("clientEvents", typeof(List<COEEventInfo>));
                _serverEvents = (List<COEEventInfo>) info.GetValue("serverEvents", typeof(List<COEEventInfo>));
                string xmlOuterXml = info.GetString("configInfo");
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlOuterXml);
                _configInfo = doc.FirstChild;
                _dataSource = info.GetString("dataSource");
                _dataSourceID = info.GetString("dataSourceId");
                _defaultValue = info.GetString("defaultValue");
                _displayInfo = (DisplayInfo) info.GetValue("displayInfo", typeof(DisplayInfo));
                _displayData = (DisplayData) info.GetValue("displayData", typeof(DisplayData));
                _id = info.GetString("Id");
                _label = info.GetString("label");
                _name = info.GetString("name");
                _helpText = info.GetString("helpText");
                _showHelp = info.GetBoolean("showHelp");
                _searchCriteriaItem = (SearchCriteria.SearchCriteriaItem) info.GetValue("searchCriteriaItem", typeof(SearchCriteria.SearchCriteriaItem));
                _validationRuleList = (List<ValidationRuleInfo>) info.GetValue("validationRuleList", typeof(List<ValidationRuleInfo>));
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("bindingExpression", this.BindingExpression);
                info.AddValue("clientEvents", this.ClientEvents, typeof(List<COEEventInfo>));
                info.AddValue("serverEvents", this.ServerEvents, typeof(List<COEEventInfo>));
                info.AddValue("configInfo", this.ConfigInfo == null ? "<configInfo />" : this.ConfigInfo.OuterXml);
                info.AddValue("dataSource", this.DataSource);
                info.AddValue("dataSourceId", this.DataSourceId);
                info.AddValue("defaultValue", this.DefaultValue);
                info.AddValue("displayInfo", this.DisplayInfo, typeof(DisplayInfo));
                info.AddValue("displayData", this.DisplayData, typeof(DisplayData));
                info.AddValue("Id", this.Id);
                info.AddValue("label", this.Label);
                info.AddValue("name", this.Name);
                info.AddValue("helpText", this.HelpText);
                info.AddValue("showHelp", this.ShowHelp);
                info.AddValue("isFileUpload", this.IsFileUpload);
                info.AddValue("pageComunicationProvider", this.PageComunicationProvider);
                info.AddValue("fileUploadBindingExpression", this.FileUploadBindingExpression);
                info.AddValue("searchCriteriaItem", this.SearchCriteriaItem, typeof(SearchCriteria.SearchCriteriaItem));
                info.AddValue("validationRuleList", this.ValidationRuleList, typeof(List<ValidationRuleInfo>));
            }

            #endregion
        }

        [Serializable]
        [XmlRoot("coeForm", Namespace = "COE.FormGroup")]
        public class Form
        {
            #region Variables
            int _id;
            string _name;
            string _dataMember;
            string _displayCulture;
            string _titleCssClass;
            List<FormElement> _layoutInfo;
            List<FormElement> _addMode;
            List<FormElement> _editMode;
            List<FormElement> _viewMode;
            DisplayInfo _formDisplay;
            int _currentIndex;
            List<COEEventInfo> _clientScripts;
            List<ValidationRuleInfo> _validationRuleList;
            string _dataSourceId;
            string _title;
            #endregion

            #region Properties
            [XmlIgnore()]
            public int CurrentIndex
            {
                get
                {
                    return this._currentIndex;
                }
                set
                {
                    this._currentIndex = value;
                }
            }

            [XmlArray("validationRuleList")]
            [XmlArrayItem("validationRule")]
            public List<ValidationRuleInfo> ValidationRuleList
            {
                get
                {
                    if(this._validationRuleList == null)
                        _validationRuleList = new List<ValidationRuleInfo>();

                    return _validationRuleList;
                }
                set { _validationRuleList = value; }
            }

            [XmlAttribute("id")]
            public int Id
            {
                get
                {
                    return this._id;
                }
                set
                {
                    this._id = value;
                }
            }

            [XmlAttribute("name")]
            public string Name
            {
                get
                {
                    return this._name;
                }
                set
                {
                    this._name = value;
                }
            }

            [XmlElement("title")]
            public string Title
            {
                get
                {
                    return _title;
                }
                set
                {
                    _title = value;
                }
            }

            [XmlElement("titleCssClass")]
            public string TitleCssClass
            {
                get { return _titleCssClass; }
                set { _titleCssClass = value; }
            }

            [XmlAttribute("dataMember")]
            public string DataMember
            {
                get
                {
                    return this._dataMember;
                }
                set
                {
                    this._dataMember = value;
                }
            }

            [XmlAttribute("displayCulture")]
            public string DisplayCulture
            {
                get { return _displayCulture; }
                set { _displayCulture = value; }
            }

            [XmlArray("layoutInfo")]
            [XmlArrayItem("formElement")]
            public List<FormElement> LayoutInfo
            {
                get
                {
                    if(_layoutInfo == null)
                        _layoutInfo = new List<FormElement>();

                    return _layoutInfo;
                }
                set
                {
                    _layoutInfo = value;
                }
            }

            [XmlElement("formDisplay")]
            public DisplayInfo FormDisplay
            {
                get
                {
                    if(_formDisplay == null)
                        _formDisplay = new DisplayInfo();

                    return _formDisplay;
                }
                set { _formDisplay = value; }
            }

            [XmlArray("addMode")]
            [XmlArrayItem("formElement")]
            public List<FormElement> AddMode
            {
                get
                {
                    if(_addMode == null)
                        _addMode = new List<FormElement>();

                    return _addMode;
                }
                set
                {
                    _addMode = value;
                }
            }

            [XmlArray("editMode")]
            [XmlArrayItem("formElement")]
            public List<FormElement> EditMode
            {
                get
                {
                    if(_editMode == null)
                        _editMode = new List<FormElement>();

                    return _editMode;
                }
                set
                {
                    _editMode = value;
                }
            }

            [XmlArray("viewMode")]
            [XmlArrayItem("formElement")]
            public List<FormElement> ViewMode
            {
                get
                {
                    if(_viewMode == null)
                        _viewMode = new List<FormElement>();

                    return _viewMode;
                }
                set
                {
                    _viewMode = value;
                }
            }

            /// <summary>
            /// Client events' subscriptions.
            /// </summary>
            [XmlArray("clientScripts")]
            [XmlArrayItem("script")]
            public List<COEEventInfo> ClientScripts
            {
                get { return _clientScripts; }
                set { _clientScripts = value; }
            }

            [XmlAttribute("dataSourceId")]
            public string DataSourceId
            {
                get
                {
                    return this._dataSourceId;
                }
                set
                {
                    this._dataSourceId = value;
                }
            }

            #endregion

            #region Methods
            public List<FormElement> GetFormElements(DisplayMode displayMode)
            {


                List<FormElement> replacementList = null;

                switch(displayMode)
                {
                    case DisplayMode.Add:
                        replacementList = this.AddMode;
                        break;
                    case DisplayMode.Edit:
                        replacementList = this.EditMode;
                        break;
                    case DisplayMode.View:
                        replacementList = this.ViewMode;
                        break;
                }


                return this.OverwriteFormElements(this.LayoutInfo, replacementList);
            }

            private List<FormElement> OverwriteFormElements(List<FormElement> baseList, List<FormElement> aditionalList)
            {
                List<FormElement> resultList = new List<FormElement>();

                //resultList = baseList;
                foreach(FormElement currentFormElement in baseList)
                {
                    string serializedFormElementXml = currentFormElement.ToString();

                    FormElement clonedFormElement = FormElement.GetFormElement(serializedFormElementXml);

                    resultList.Add(clonedFormElement);

                }

                if(aditionalList != null)
                {
                    foreach(FormElement currentAdditionalFormElement in aditionalList)
                    {
                        FormElement currentOriginalFormElement = GetFormElement(resultList, currentAdditionalFormElement.Id);
                        if(currentOriginalFormElement != null)
                            OverwriteFormElement(currentOriginalFormElement, currentAdditionalFormElement);
                        else
                            resultList.Add(FormElement.GetFormElement(currentAdditionalFormElement.ToString()));
                    }
                }

                return resultList;
            }

            private void OverwriteFormElement(Object originalFormElement, Object additionalFormElement)
            {
                if(additionalFormElement != null)
                {
                    if(!originalFormElement.GetType().Equals(additionalFormElement.GetType()))
                        throw new ArgumentException("Incompatible data types");

                    foreach(System.Reflection.PropertyInfo currentProperty in originalFormElement.GetType().GetProperties())
                    {
                        if(currentProperty.CanRead && currentProperty.CanWrite)
                        {
                            if(currentProperty.PropertyType.IsPrimitive ||
                                currentProperty.PropertyType.FullName == "System.String" ||
                                currentProperty.PropertyType.FullName.Contains("List") ||
                                currentProperty == null)
                            {
                                object additionalFormElementValue = currentProperty.GetValue(additionalFormElement, new object[] { });
                                if(additionalFormElementValue != null)
                                    currentProperty.SetValue(originalFormElement, additionalFormElementValue, new object[] { });
                            }
                            else if(currentProperty.PropertyType == typeof(XmlNode))
                            {
                                OverwriteXmlNode((XmlNode) currentProperty.GetValue(originalFormElement, new object[] { }),
                                                 (XmlNode) currentProperty.GetValue(additionalFormElement, new object[] { })
                                    );
                            }
                            else
                            {
                                object originalProperty = currentProperty.GetValue(originalFormElement, new object[] { });

                                if(originalProperty == null)
                                    currentProperty.SetValue(originalFormElement, currentProperty.GetValue(additionalFormElement, new object[] { }), new object[] { });
                                else
                                    OverwriteFormElement(originalProperty, currentProperty.GetValue(additionalFormElement, new object[] { }));
                            }
                        }
                    }
                }
            }

            public static void OverwriteXmlNode(XmlNode originalXmlNode, XmlNode additionalXmlNode)
            {
                if(additionalXmlNode != null)
                {
                    if(additionalXmlNode.Attributes != null && additionalXmlNode.Attributes.Count > 0)
                    {
                        foreach(XmlAttribute currentAttribute in additionalXmlNode.Attributes)
                        {
                            if(originalXmlNode.Attributes[currentAttribute.Name] != null)
                                originalXmlNode.Attributes[currentAttribute.Name].Value = currentAttribute.Value;
                            else
                                originalXmlNode.Attributes.Append((XmlAttribute) originalXmlNode.OwnerDocument.ImportNode((XmlNode) currentAttribute, true));
                        }
                    }

                    if(!string.IsNullOrEmpty(additionalXmlNode.Value))
                    {
                        originalXmlNode.Value = additionalXmlNode.Value;
                    }


                    foreach(XmlNode currentAdditionalNode in additionalXmlNode.ChildNodes)
                    {
                        bool containsNode = false;

                        foreach(XmlNode currentOriginalNode in originalXmlNode.ChildNodes)
                            if(currentOriginalNode.Name == currentAdditionalNode.Name)
                            {
                                OverwriteXmlNode(currentOriginalNode, currentAdditionalNode);
                                containsNode = true;
                            }

                        if(!containsNode)
                        {
                            originalXmlNode.AppendChild(originalXmlNode.OwnerDocument.CreateNode(currentAdditionalNode.NodeType, currentAdditionalNode.Name, currentAdditionalNode.NamespaceURI));
                            OverwriteXmlNode(originalXmlNode.LastChild, currentAdditionalNode);
                        }
                    }
                }
            }

            private FormElement GetFormElement(List<FormElement> container, string id)
            {
                if(!string.IsNullOrEmpty(id))
                {
                    foreach(FormElement currentElement in container)
                        if(!string.IsNullOrEmpty(currentElement.Id) && currentElement.Id.Trim() == id.Trim())
                            return currentElement;
                }
                return null;
            }
            #endregion

            #region Factory Methods
            public static Form GetForm(string xml)
            {
                return Utilities.XmlDeserialize<Form>(xml);
            }

            public override string ToString()
            {
                return Utilities.XmlSerialize(this);
            }
            #endregion
        }

        /// <summary>
        /// Class used for setting layout information at several levels
        /// </summary>
        [Serializable]
        public class DisplayInfo
        {

            #region Variables
            private string _height;
            private string _width;
            private string _style;
            private string _cssClass;
            private string _top;
            private string _left;
            private string _position;
            private string _assembly;
            private string _type;
            private bool _fitToContents;
            private LayoutStyle _layoutStyle;
            private bool _visible = true;
            #endregion

            #region Properties

            /// <summary>
            /// Sets or gets the height of the containing type (A form or an element).
            /// </summary>
            [XmlElement("height")]
            public string Height
            {
                get { return _height; }
                set { _height = value; }
            }

            /// <summary>
            /// Sets or gets the width of the containing type (A form or an element).
            /// </summary>
            [XmlElement("width")]
            public string Width
            {
                get { return _width; }
                set { _width = value; }
            }

            /// <summary>
            /// Sets or gets the style of the containing type (A form or an element).
            /// </summary>
            [XmlElement("style")]
            public string Style
            {
                get { return _style; }
                set { _style = value; }
            }

            [XmlElement("cssClass")]
            public string CSSClass
            {
                get { return _cssClass; }
                set { _cssClass = value; }
            }

            /// <summary>
            /// Sets or gets the top location of the containing type (A form or an element).
            /// </summary>
            [XmlElement("top")]
            public string Top
            {
                get { return _top; }
                set { _top = value; }
            }

            /// <summary>
            /// Sets or gets the left location of the containing type (A form or an element).
            /// </summary>
            [XmlElement("left")]
            public string Left
            {
                get { return _left; }
                set { _left = value; }
            }

            /// <summary>
            /// Sets or gets the assemble where the containing type is located (A form or an element).
            /// </summary>
            [XmlElement("assembly")]
            public string Assembly
            {
                get { return _assembly; }
                set { _assembly = value; }
            }

            /// <summary>
            /// Sets or gets the class name that implements the containing type (A form or an element).
            /// </summary>
            [XmlElement("type")]
            public string Type
            {
                get { return _type; }
                set { _type = value; }
            }

            /// <summary>
            /// Sets or gets the position (absolute or relative) of the containing type (A form or an element).
            /// </summary>
            [XmlElement("position")]
            public string Position
            {
                get { return _position; }
                set { _position = value; }
            }


            /// <summary>
            /// Sets or gets the position (absolute or relative) of the containing type (A form or an element).
            /// </summary>
            [XmlElement("fitToContents")]
            [DefaultValue(false)]
            public bool FitToContents
            {
                get { return _fitToContents; }
                set { _fitToContents = value; }
            }

            [XmlElement("layoutStyle")]
            [DefaultValue(LayoutStyle.AbsoluteLayout)]
            public LayoutStyle LayoutStyle
            {
                get { return _layoutStyle; }
                set { _layoutStyle = value; }
            }

            [XmlElement("visible")]
            //[DefaultValue(true)]
            public bool Visible
            {
                get { return _visible; }
                set { _visible = value; }
            }
            #endregion

            #region Constructors

            public DisplayInfo()
            {
                this._visible = true;
            }

            #endregion
        }

        /// <summary>
        /// List of client validation to be performed over an element.
        /// <b>Example:</b>
        /// <code lang="xml">
        ///   &lt;validationRule validationRuleName="textLength" errorMessage="The length must be between 0 and 120"&gt;
        ///     &lt;params&gt;
        ///       &lt;param name="min" value="0"/&gt;
        ///       &lt;param name="max" value="120"/&gt;
        ///     &lt;/params&gt;
        ///   &lt;/validationRule&gt;
        /// </code>
        /// </summary>
        [Serializable]
        public class ValidationRuleInfo
        {
            #region Variables
            private ValidationRuleEnum _validationRuleName;
            private string _errorMessage;
            private List<Parameter> _parameters;
            private System.Web.UI.WebControls.ValidatorDisplay _display;
            private DisplayPosition _displayPosition;
            private string _validationGroup;
            #endregion

            #region Constructor
            public ValidationRuleInfo()
            {
                this.Display = System.Web.UI.WebControls.ValidatorDisplay.Dynamic;
            }
            #endregion

            #region Properties

            /// <summary>
            /// The rule Name. For a detailed list of allowed values see <see cref="COEValidationRuleEnum"/>.
            /// </summary>
            [XmlAttribute("validationRuleName")]
            public ValidationRuleEnum ValidationRuleName
            {
                get { return _validationRuleName; }
                set { _validationRuleName = value; }
            }

            /// <summary>
            /// The the validation fail, what would be the error massage for the user?
            /// </summary>
            [XmlAttribute("errorMessage")]
            public string ErrorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

            /// <summary>
            /// The the validation fail, what would be the error massage for the user?
            /// </summary>
            [XmlAttribute("validationGroup")]
            public string ValidationGroup
            {
                get { return _validationGroup; }
                set { _validationGroup = value; }
            }

            [XmlAttribute("display")]
            [DefaultValue(System.Web.UI.WebControls.ValidatorDisplay.Dynamic)]
            public System.Web.UI.WebControls.ValidatorDisplay Display
            {
                get { return _display; }
                set { _display = value; }
            }

            [XmlAttribute("displayPosition")]
            public DisplayPosition DisplayPosition
            {
                get { return _displayPosition; }
                set { _displayPosition = value; }
            }



            /// <summary>
            /// A list of params for the validation rule. A common example is:
            /// <code lang="xml">
            ///     &lt;params&gt;
            ///       &lt;param name="min" value="0"/&gt;
            ///       &lt;param name="max" value="120"/&gt;
            ///     &lt;/params&gt;
            /// </code>
            /// </summary>
            [XmlArray("params")]
            [XmlArrayItem("param")]
            public List<Parameter> Params
            {
                get
                {
                    if(_parameters == null)
                        _parameters = new List<Parameter>();

                    return _parameters;
                }
                set { _parameters = value; }
            }
            #endregion
        }

        /// <summary>
        /// Messaging type used for specifying parameters for validation rules.
        /// </summary>
        [Serializable]
        public class Parameter
        {
            #region Variables
            private string _name;
            private string _value;
            #endregion

            #region Properties

            /// <summary>
            /// The param name.
            /// </summary>
            [XmlAttribute("name")]
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            /// <summary>
            /// The param value.
            /// </summary>
            [XmlAttribute("value")]
            public string Value
            {
                get { return _value; }
                set { _value = value; }
            }
            #endregion
        }

        /// <summary>
        /// Messaging type for passing the information needed to subscribe to controls events.
        /// </summary>
        [Serializable]
        public class COEEventInfo
        {
            private string _eventName;
            private string _eventHandlerName;
            private string _eventHandlerScript;
            private string _value;

            /// <summary>
            /// The name of the method that will handle the event.
            /// </summary>
            [XmlAttribute("handlerName")]
            public string EventHandlerName
            {
                get { return _eventHandlerName; }
                set { _eventHandlerName = value; }
            }

            /// <summary>
            /// The event to subscribe to.
            /// </summary>
            [XmlAttribute("eventName")]
            public string EventName
            {
                get { return _eventName; }
                set { _eventName = value; }
            }

            /// <summary>
            /// If a script will handle the event, it may be passed through this property.
            /// </summary>
            [XmlAttribute("handlerScript")]
            public string EventHandlerScript
            {
                get { return _eventHandlerScript; }
                set { _eventHandlerScript = value; }
            }

            [XmlText()]
            public string Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                }
            }
        }

        [Serializable]
        public class ScriptInfo
        {
            #region Variables
            private string _src;
            private string _value;
            #endregion

            #region Properties
            [XmlAttribute("src")]
            public string Src
            {
                get
                {
                    return _src;
                }
                set
                {
                    _src = value;
                }
            }
            [XmlText()]
            public string Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                }
            }
            #endregion
        }

        /// <summary>
        /// Class used for setting data display information (Read only binding for display)
        /// </summary>
        [Serializable]
        [XmlRoot("displayData", Namespace = "COE.FormGroup")]
        public class DisplayData
        {
            //<displayData>
            //  <assembly name="CambridgeSoft.COE.Registration.Services" />
            //  <class name="ProjectList" />
            //  <method name="GetActiveProjectListByPersonID">
            //    <paramList>
            //      <param value="4" type="System.Int32" />
            //    </paramList>
            //  </method>
            //</displayData>

            #region Variables
            private Assembly _assembly;
            private Class _class;
            private Method _method;
            #endregion

            #region Properties
            /// <summary>
            /// Assembly where the class is held.
            /// </summary>
            [XmlElement("assembly")]
            public Assembly Assembly
            {
                get { return _assembly; }
                set { _assembly = value; }
            }

            /// <summary>
            /// Class that holds the method to call.
            /// </summary>
            [XmlElement("class")]
            public Class Class
            {
                get { return _class; }
                set { _class = value; }
            }

            /// <summary>
            /// The method to execute.
            /// </summary>
            [XmlElement("method")]
            public Method Method
            {
                get { return _method; }
                set { _method = value; }
            }
            #endregion

            #region Constructors
            #endregion
        }

        [Serializable()]
        public class Assembly
        {
            #region Variables
            private string _name;
            #endregion

            #region Properties
            /// <summary>
            /// Assembly's name.
            /// </summary>
            [XmlAttribute("name")]
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }
            #endregion

            #region Constructors
            #endregion
        }

        [Serializable()]
        public class Class
        {
            #region Variables
            private string _name;
            #endregion

            #region Properties
            /// <summary>
            /// Class' name.
            /// </summary>
            [XmlAttribute("name")]
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }
            #endregion

            #region Constructors
            #endregion
        }

        [Serializable()]
        public class Method
        {
            #region Variables
            private MethodParameter[] _methodParameters;
            private string _name;
            #endregion

            #region Properties
            /// <summary>
            /// Method's name.
            /// </summary>
            [XmlAttribute("name")]
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            /// <summary>
            /// Method's parameters
            /// </summary>
            [XmlArray("paramList")]
            [XmlArrayItem("param")]
            public MethodParameter[] ParamList
            {
                get { return _methodParameters; }
                set { _methodParameters = value; }
            }
            #endregion
        }

        [Serializable()]
        public class MethodParameter
        {
            #region Variables
            private string _value;
            private string _type;
            #endregion

            #region Properties
            /// <summary>
            /// Parameter's type.
            /// </summary>
            [XmlAttribute("type")]
            public string Type
            {
                get { return _type; }
                set { _type = value; }
            }

            /// <summary>
            /// Parameter's value.
            /// </summary>
            [XmlAttribute("value")]
            public string Value
            {
                get { return _value; }
                set { _value = value; }
            }
            #endregion
        }
        #endregion

        #region Enumerations
        /// <summary>
        /// Validation rules enumeration. Specify allowed values for validation rules' names:
        /// <list type="bullet">
        ///   <item>requiredField</item>
        ///   <item>numericRange</item>
        ///   <item>positiveInteger</item>
        ///   <item>integer</item>
        ///   <item>double</item>
        ///   <item>date</item>
        ///   <item>textLength</item>
        ///   <item>onlyChemicalContentAllowed</item>
        ///   <item>wordListEnumeration</item>
        /// </list>
        /// </summary>
        [Serializable]
        public enum ValidationRuleEnum
        {
            [XmlEnum("requiredField")]
            RequiredField,
            [XmlEnum("numericRange")]
            NumericRange,
            [XmlEnum("positiveInteger")]
            PositiveInteger,
            [XmlEnum("integer")]
            Integer,
            [XmlEnum("double")]
            Double,
            [XmlEnum("date")]
            Date,
            [XmlEnum("float")]
            Float,
            [XmlEnum("textLength")]
            TextLength,
            //[XmlEnum("onlyChemicalContentAllowed")]
            //OnlyChemicalContentAllowed,
            [XmlEnum("wordListEnumeration")]
            WordListEnumeration,
            [XmlEnum("custom")]
            Custom,
            [XmlEnum("notEmptyStructure")]
            NotEmptyStructure,
            [XmlEnum("notEmptyQuery")]
            NotEmptyQuery,
            [XmlEnum("notEmptyStructureAndNoText")]
            NotEmptyStructureAndNoText,
            [XmlEnum("casValidation")]
            CasValidation,
        }

        [Serializable]
        public enum DisplayMode
        {
            [XmlEnum("Add")]
            Add,
            [XmlEnum("Edit")]
            Edit,
            [XmlEnum("View")]
            View,
            [XmlEnum("All")]
            All
        }

        [Serializable]
        public enum DisplayPosition
        {
            [XmlEnum("Top_Left")]
            Top_Left,
            [XmlEnum("Top_Right")]
            Top_Right,
            [XmlEnum("Bottom_Left")]
            Bottom_Left,
            [XmlEnum("Bottom_Right")]
            Bottom_Right,
        }

        [Serializable]
        public enum CurrentFormEnum
        {
            QueryForm = 1,
            DetailForm = 2,
            ListForm = 3
        }

        /// <summary>
        /// Detemines the Layout style to be used when rendering a Form
        /// </summary>
        [Serializable]
        public enum LayoutStyle
        {
            /// <summary>
            /// Absoute layout style, which allows using top, and left attributes for positioning.
            /// </summary>
            [XmlEnum("absoluteLayout")]
            AbsoluteLayout,
            /// <summary>
            /// Flow layout style, which means that the elements would be positioned from left to right and top to bottom.
            /// </summary>
            [XmlEnum("flowLayout")]
            FlowLayout
        }
        #endregion
    }

}
