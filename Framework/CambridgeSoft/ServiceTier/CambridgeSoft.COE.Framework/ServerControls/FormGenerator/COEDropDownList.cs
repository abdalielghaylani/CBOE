using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web.UI;
using System.Collections;
using CambridgeSoft.COE.Framework.Common;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.COEPickListPickerService;
using CambridgeSoft.COE.Framework.COEDisplayDataBrokerService;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This class implements a DropDownList control that may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COEDropDownList class accepts every DropDownList property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: What value would be selected by default?</item>
    ///         <item>bindingExpression: What is the binding Attribute, relative to the datasource, of the formgenerator.</item>
    ///         <item>label: What is its label?</item>
    ///         <item>configInfo: Additional attributes like CSSClass, Style, Width, Height, ID...</item>
    ///     </list>
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// </para>
    /// <b>With XML:</b>
    /// <code lang="XML">
    ///     &lt;formElement&gt;
    ///       &lt;label&gt;Chemist&lt;/label&gt;
    ///       &lt;bindingExpression&gt;PropertyList[@Name='CHEMIST' | Value]&lt;/bindingExpression&gt;
    ///       &lt;validationRuleList&gt;
    ///         &lt;validationRule validationRuleName="positiveInteger" errorMessage="The value must be a positive integer"/&gt;
    ///         &lt;validationRule validationRuleName="requiredField" errorMessage="You must select a chemist" /&gt;
    ///       &lt;/validationRuleList&gt;
    ///       &lt;dataSourceID&gt;ChemistsCslaDataSource&lt;/dataSourceID&gt;
    ///       &lt;configInfo&gt;
    ///         &lt;fieldConfig&gt;
    ///         &lt;CSSClass&gt;COEDropDownListNew&lt;/CSSClass&gt;
    ///         &lt;CSSLabelClass&gt;COERequiredField&lt;/CSSLabelClass&gt;
    ///         &lt;ID&gt;ChemistDropDownList&lt;/ID&gt;
    ///         &lt;dataTextField&gt;Value&lt;/dataTextField&gt;
    ///         &lt;dataValueField&gt;Key&lt;/dataValueField&gt;
    ///         &lt;/fieldConfig&gt;
    ///       &lt;/configInfo&gt;
    ///       &lt;displayInfo&gt;
    ///         &lt;height&gt;15px&lt;/height&gt;
    ///         &lt;width&gt;170px&lt;/width&gt;
    ///         &lt;top&gt;5px&lt;/top&gt;
    ///         &lt;left&gt;110px&lt;/left&gt;
    ///         &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList&lt;/type&gt;
    ///       &lt;/displayInfo&gt;
    ///     &lt;/formElement&gt;
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// In this implementation "Default Value", GetData and PutData Methods reffer to the SelectedItem's value.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COEDropDownList runat=server></{0}:COEDropDownList>")]
    public class COEDropDownList : DropDownList, ICOEGenerableControl, ICOERequireable, ICOELabelable, ICOEDesignable, ICOEHelpContainer, ICOEDisplayMode, ICOEReadOnly, ICOECurrentFormMode
    {
        #region Constants
        private const string YAHOODOMEVENTS = "yahoo-dom-event";
        private const string DRAGDROPMIN = "dragdrop-min";
        private const string CONTAINERMIN = "container-min";
        private const string DEFAULT_CSSCLASS = "FEDropDownList";
        private const string DEFAULT_LABEL_CSSCLASS = "FELabel";
        #endregion
        
        #region Variables
        private string _defaultValue;
        private Label _lit = new Label();
        int _picklistDomain = -1;
        string _sqlSelect = string.Empty;
        string _dllname = string.Empty;
        string _className = string.Empty;
        string _methodName = string.Empty;
        IDictionary keyValuePair;
        private Image _helpImage = null;

        private CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL = null;
        private CambridgeSoft.COE.Framework.Common.DALFactory dalFactory = new DALFactory();
        private string ServiceName = "COESearch";
        private string _databaseName = Resources.SecurityDatabaseName;
        private CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.DisplayMode _displayMode;
        private COEEditControl _editControl = COEEditControl.NotSet;
        private CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.CurrentFormEnum _currentFormMode = FormGroup.CurrentFormEnum.DetailForm;

        #endregion

        public string SQLSelect
        {
            get
            {
                return _sqlSelect;
            }
            set
            {
                _sqlSelect = value;
            }
        }

        public string[] AllowableTokens
        {
            get
            {
                return new string[] { "&&loggedInUser", "&&currentDate", "&&useSortOrderTop" };
            }
        }

        #region ICOEGenerableControl Members
        /// <summary>
        /// Gets the selected value as string.
        /// </summary>
        /// <returns>A string with the selected value.</returns>
        public new object GetData()
        {
            EnsureDataBound();

            if (this.SelectedItem == null)
                return string.Empty;

            return this.SelectedItem.Value;
        }

        /// <summary>
        /// Sets the selected value.
        /// </summary>
        /// <param name="data">A string with the desired selected value.</param>
        public void PutData(object data)
        {
            EnsureDataBound();
            if (data!=null && !string.IsNullOrEmpty(Convert.ToString(data))) //Coverity Fix CID 19637 ASV
            {
                //Coverity Fixes: CBOE-194 : CID-11746
                bool bValidToken = IsAValidToken(data.ToString());
                ListItem listItem = this.Items.FindByText(this.ConvertToken(data.ToString()));
                if (bValidToken && listItem != null)
                    this.SelectedValue = listItem.Value;
                else
                    this.SelectedValue = Convert.ToString(data);

                if (this.DisplayMode != FormGroup.DisplayMode.Add && _picklistDomain != -1 && !bValidToken && !IsItemAvailable(data.ToString()))
                {
                    keyValuePair = ((IKeyValueListHolder)PickListNameValueList.GetPickListNameValueList(_picklistDomain, true, data)).KeyValueList;
                    this.DataSource = keyValuePair;
                    this.DataBind();
                    this.SelectedValue = data.ToString();
                }
            }
            else if (CurrentFormMode == FormGroup.CurrentFormEnum.QueryForm && _picklistDomain != -1)
            {
                keyValuePair = ((IKeyValueListHolder)PickListNameValueList.GetPickListNameValueList(_picklistDomain, PickListStatus.All, data)).KeyValueList;
                this.DataSource = keyValuePair;
                this.DataBind();
            }
        }

        /// <summary>Loads its specific configuration from an xml in the form:
        /// <code lang="Xml">
        ///   &lt;fieldConfig&gt;
        ///   &lt;CSSClass&gt;COEDropDownListNew&lt;/CSSClass&gt;
        ///   &lt;CSSLabelClass&gt;COERequiredField&lt;/CSSLabelClass&gt;
        ///   &lt;ID&gt;ChemistDropDownList&lt;/ID&gt;
        ///   &lt;dataTextField&gt;Value&lt;/dataTextField&gt;
        ///   &lt;dataValueField&gt;Key&lt;/dataValueField&gt;
        ///   &lt;/fieldConfig&gt;
        /// </code>
        /// </summary>
        /// <param name="xmlDataAsString">The configInfo xml snippet</param>
        public void LoadFromXml(string xmlDataAsString)
        {
            /*
              * <fieldConfig>
              *  <Style>border-color:blue;</Style>
              *  <DropDownItems>
              *    <Item key="1" value="llala"/>
              *    <Item key="2" value="lele"/>
              *    <Item key="3" value="lili"/>
              *  </DropDownItems>
              *  <Height></Height>
              *  <Width></Width>
              *  <ID>32</ID>
              * </fieldConfig>
              */
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);

            XmlNamespaceManager manager = new XmlNamespaceManager(xmlData.NameTable);
            manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);

            //Try to avoid the use of Style, Width and Height; instead of it, define all in a CSSClass.
            XmlNode style = xmlData.SelectSingleNode("//COE:Style", manager);
            if (style != null && style.InnerText.Length > 0)
            {
                string[] styles = style.InnerText.Split(new char[1] { ';' });
                for (int i = 0; i < styles.Length; i++)
                {
                    if (styles[i].Length > 0)
                    {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        string styleId = styleDef[0].Trim();
                        string styleValue = styleDef[1].Trim();
                        this.Style.Add(styleId, styleValue);
                    }
                }
            }

            XmlNode dropdownItems = xmlData.SelectSingleNode("//COE:dropDownItems", manager);
            if (dropdownItems != null)
            {
                foreach (XmlNode currentItem in dropdownItems.ChildNodes)
                {
                    string key = currentItem.Attributes["key"].Value.ToString();
                    string value = currentItem.Attributes["value"] == null ? string.Empty : currentItem.Attributes["value"].Value.ToString();
                    this.Items.Add(new ListItem(value, key));
                }
            }

            //create a new node for method invocation
            //string dllName, string className, string methodName
            //<dropdownItemsByMethod>
            //  <dllName>CambridgeSoft.COE.Framework</dllName>
            //  <className>CambridgeSoft.COE.Framework.PickListPicker.GenericPicklists</className>
            //  <methodName>GetPeople</methodName>
            //</dropdownItemsByMethod>
            XmlNode dropdownItemsByMethod = xmlData.SelectSingleNode("//COE:dropdownItemsByMethod", manager);
            if (dropdownItemsByMethod != null)
            {
                //coverity fix
                XmlNode dllNameNode = dropdownItemsByMethod.SelectSingleNode("//COE:dllName", manager);
                if (dllNameNode != null)
                    _dllname = dllNameNode.InnerText;
                XmlNode classNameNode = dropdownItemsByMethod.SelectSingleNode("//COE:className", manager);
                if (classNameNode != null)
                    _className = classNameNode.InnerText;
                XmlNode methodNameNode =dropdownItemsByMethod.SelectSingleNode("//COE:methodName", manager);
                if(methodNameNode != null)
                    _methodName = methodNameNode.InnerText;

                this.AppendDataBoundItems = true;
                this.DataSource = ((IKeyValueListHolder)COEDisplayDataBroker.GetDisplayData(_dllname, _className, _methodName)).KeyValueList;
                this.DataBind();
            }

            if (dropdownItemsByMethod == null)
            {
                //JHS 9/23/2008 - Adding this new node to allow the drop down to be built from a select statement
                XmlNode dropdownItemsSelect = xmlData.SelectSingleNode("//COE:dropDownItemsSelect", manager);
                XmlNode pickListDomain = xmlData.SelectSingleNode("//COE:PickListDomain", manager);
                bool usePicklistList = false;

                if (pickListDomain != null && int.TryParse(pickListDomain.InnerText, out _picklistDomain))
                    usePicklistList = true;
                switch (usePicklistList)
                {
                    case true:
                        //Sujen 09/28/2012 - Adding this new node to allow the drop down to be built from a picklist service
                        keyValuePair = ((IKeyValueListHolder)PickListNameValueList.GetPickListNameValueList(_picklistDomain, true, null)).KeyValueList;
                        this.DataSource = keyValuePair;
                        this.DataBind();
                        break;
                    case false:
                        if (dropdownItemsSelect != null)
                        {
                            _picklistDomain = -1;
                            _sqlSelect = dropdownItemsSelect.InnerText;
                            this.DataSource = _sqlSelect;
                            this.DataBind();
                        }
                        break;
                }

            }

            #region Comment
            //we should switch to this in Degas...It is missing sorting
            //but uses the picklist service which seems like a good thing
            //this will build the picklist by passing to the PickListNameValueList 
            //sql statement must return 2 items (1 named key and the other named value)
            /*
            if (dropdownItemsByMethod == null)
            {

                XmlNode dropdownItemsSelect = xmlData.SelectSingleNode("//COE:dropDownItemsSelect", manager);
                if (dropdownItemsSelect != null)
                {

                    _sqlSelect = dropdownItemsSelect.InnerText;

                    //conditionally add a 'sort order' if none was specified in the original query
                    if ((_sqlSelect.IndexOf("order by", 0, StringComparison.InvariantCultureIgnoreCase) == -1) && (_sqlSelect.IndexOf("value",0,StringComparison.InvariantCultureIgnoreCase) > -1))
                       _sqlSelect += " order by value asc";


                    //Make sure it is a select statement as a security measure
                    if (_sqlSelect.Trim().Substring(0, 6).ToUpper() != "SELECT")
                    {
                        throw new Exception("An invalid select statement was specified for a drop down box");
                    }

                    this.Items.Add(new ListItem("Select One", ""));
                    this.AppendDataBoundItems = true;
                    this.DataTextField = "Value";
                    this.DataValueField = "Key";
                    this.DataSource = PickListNameValueList.GetPickListNameValueList(_sqlSelect).KeyValueList;
                   
                    this.DataBind();
                   
                }
            }
            */
            #endregion

            XmlNode dataTextFieldNode = xmlData.SelectSingleNode("//COE:dataTextField", manager);
            if (dataTextFieldNode != null && !string.IsNullOrEmpty(dataTextFieldNode.InnerText))
                this.DataTextField = dataTextFieldNode.InnerText;

            XmlNode dataValueFieldNode = xmlData.SelectSingleNode("//COE:dataValueField", manager);
            if (dataValueFieldNode != null && !string.IsNullOrEmpty(dataValueFieldNode.InnerText))
                this.DataValueField = dataValueFieldNode.InnerText;
            //this.DataBind();
            //}

            XmlNode width = xmlData.SelectSingleNode("//COE:Width", manager);
            if (width != null && width.InnerText.Length > 0)
            {
                this.Width = new Unit(width.InnerText);
            }

            XmlNode height = xmlData.SelectSingleNode("//COE:Height", manager);
            if (height != null && height.InnerText.Length > 0)
            {
                this.Height = new Unit(height.InnerText);
            }

            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if (cssClass != null && cssClass.InnerText.Length > 0)
                this.CssClass = cssClass.InnerText;

            XmlNode cssLabelClass = xmlData.SelectSingleNode("//COE:CSSLabelClass", manager);
            if (cssLabelClass != null && cssLabelClass.InnerText.Length > 0)
                _lit.CssClass = cssLabelClass.InnerText;

            XmlNode labelStyle = xmlData.SelectSingleNode("//COE:LabelStyle", manager);
            if (labelStyle != null && labelStyle.InnerText.Length > 0)
            {
                string[] styles = labelStyle.InnerText.Split(new char[1] { ';' });
                for (int i = 0; i < styles.Length; i++)
                {
                    if (styles[i].Length > 0)
                    {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        string styleId = styleDef[0].Trim();
                        string styleValue = styleDef[1].Trim();
                        _lit.Style.Add(styleId, styleValue);
                    }
                }
            }

            XmlNode enable = xmlData.SelectSingleNode("//COE:Enable", manager);
            if (enable != null && enable.InnerText.Length > 0)
                this.Enabled = bool.Parse(enable.InnerText);

            /*XmlNode id = xmlData.SelectSingleNode("//COE:ID", manager);
            if (id != null && id.InnerText.Length > 0)
                this.ID = id.InnerText;
             */

            XmlNode autoPB = xmlData.SelectSingleNode("//COE:AutoPostBack", manager);
            if (autoPB != null && autoPB.InnerText.Length > 0)
                this.AutoPostBack = bool.Parse(autoPB.InnerText);

        }

        /// <summary>
        /// Allows to set the default selected value.
        /// </summary>
        public string DefaultValue
        {
            get
            {
                return _defaultValue;
            }
            set
            {

                _defaultValue = value;
            }
        }

        #endregion

        #region ICOEDesignable Members
        public XmlNode GetConfigInfo()
        {
            XmlDocument xmlData = new XmlDocument();
            string xmlns = "COE.FormGroup";
            string xmlprefix = "COE";

            xmlData.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "fieldConfig", xmlns));

            //Try to avoid the use of Style, Width and Height; instead of it, define all in a CSSClass.
            if (this.Style.Count > 0)
            {
                XmlNode style = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Style", xmlns);
                style.InnerText = this.Style.Value;
                xmlData.FirstChild.AppendChild(style);
            }

            if (!string.IsNullOrEmpty(_sqlSelect))
            {
                XmlNode dropdownItemsSelect = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "dropDownItemsSelect", xmlns);
                dropdownItemsSelect.InnerText = _sqlSelect;
                xmlData.FirstChild.AppendChild(dropdownItemsSelect);
            }
            else if (this.Items.Count > 0)
            {
                XmlNode dropdownItems = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "dropDownItems", xmlns);
                //    *  <dropDownItems>
                //    *    <item key="1" value="llala"/>
                //    *    <item key="2" value="lele"/>
                //    *    <item key="3" value="lili"/>
                //    *  </dropDownItem
                foreach (ListItem currentItem in this.Items)
                {
                    XmlNode item = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "item", xmlns);

                    item.Attributes.Append(xmlData.CreateAttribute(xmlprefix, "key", xmlns));
                    item.Attributes[0].Value = currentItem.Value;

                    item.Attributes.Append(xmlData.CreateAttribute(xmlprefix, "value", xmlns));
                    item.Attributes[1].Value = currentItem.Text;

                    dropdownItems.AppendChild(item);
                }
                xmlData.FirstChild.AppendChild(dropdownItems);
            }


            if (!string.IsNullOrEmpty(this.DataTextField))
            {
                XmlNode dataTextFieldNode = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "dataTextField", xmlns);
                dataTextFieldNode.InnerText = this.DataTextField;
                xmlData.FirstChild.AppendChild(dataTextFieldNode);
            }

            if (!string.IsNullOrEmpty(this.DataValueField))
            {
                XmlNode dataValueFieldNode = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "dataValueField", xmlns);
                dataValueFieldNode.InnerText = this.DataValueField;
                xmlData.FirstChild.AppendChild(dataValueFieldNode);
            }

            if (this.Width != Unit.Empty)
            {
                XmlNode width = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Width", xmlns);
                width.InnerText = this.Width.ToString();
                xmlData.FirstChild.AppendChild(width);
            }

            if (this.Height != Unit.Empty)
            {
                XmlNode height = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Height", xmlns);
                height.InnerText = this.Height.ToString();
                xmlData.FirstChild.AppendChild(height);
            }

            //Ensure there is a default CSSClass value for this form-field and its label
            if (string.IsNullOrEmpty(this.CssClass))
                this.CssClass = DEFAULT_CSSCLASS;
            XmlNode cssClass = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "CSSClass", xmlns);
            cssClass.InnerText = this.CssClass;
            xmlData.FirstChild.AppendChild(cssClass);

            if (string.IsNullOrEmpty(_lit.CssClass))
                _lit.CssClass = DEFAULT_LABEL_CSSCLASS;
            XmlNode cssLabelClass = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "CSSLabelClass", xmlns);
            cssLabelClass.InnerText = _lit.CssClass;
            xmlData.FirstChild.AppendChild(cssLabelClass);

            if (!string.IsNullOrEmpty(_lit.Style.Value))
            {
                XmlNode labelStyle = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "LabelStyle", xmlns);
                labelStyle.InnerText = _lit.Style.Value;
                xmlData.FirstChild.AppendChild(labelStyle);
            }


            XmlNode enable = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Enable", xmlns);
            enable.InnerText = this.Enabled.ToString();
            xmlData.FirstChild.AppendChild(enable);

            if (!string.IsNullOrEmpty(this.ID))
            {
                XmlNode id = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "ID", xmlns);
                id.InnerText = this.ID;
                xmlData.FirstChild.AppendChild(id);
            }

            XmlNode autoPB = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "AutoPostBack", xmlns);
            autoPB.InnerText = this.AutoPostBack.ToString();
            xmlData.FirstChild.AppendChild(autoPB);

            return xmlData.FirstChild;
        }
        #endregion

        #region ICOELabelable Members
        /// <summary>
        /// Gets or sets the Control's label.
        /// </summary>
        public string Label
        {
            get
            {
                if (ViewState[Constants.Label_VS] != null)
                    return (string)ViewState[Constants.Label_VS];
                else
                    return string.Empty;
            }
            set
            {
                ViewState[Constants.Label_VS] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Control's label CSS styles attributes.
        /// </summary>
        public Dictionary<string, string> LabelStyles
        {
            get
            {
                if (ViewState[Constants.LabelStyles_VS] != null)
                    return (Dictionary<string, string>)ViewState[Constants.LabelStyles_VS];
                else
                    return null;
            }
            set
            {
                ViewState[Constants.LabelStyles_VS] = value;
            }
        }


        #endregion

        #region ICOERequireable Members
        /// <summary>
        /// Gets or sets if the control is required to have a value.
        /// </summary>
        public bool Required
        {
            get
            {
                if (ViewState["Required"] != null)
                    return (bool)ViewState["Required"];
                else
                    return false;
            }
            set
            {
                ViewState["Required"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the ICOERequireable css style
        /// </summary>
        public string RequiredStyle
        {
            get
            {
                if (ViewState[Constants.RequiredStyle_VS] != null)
                    return (string)ViewState[Constants.RequiredStyle_VS];
                else
                    return String.Empty;
            }
            set
            {
                ViewState[Constants.RequiredStyle_VS] = value;
            }
        }

        /// <summary>
        /// Gets or sets the ICOERequireable label's css style
        /// </summary>
        public string RequiredLabelStyle
        {
            get
            {
                if (ViewState[Constants.RequiredLabelStyle_VS] != null)
                    return (string)ViewState[Constants.RequiredLabelStyle_VS];
                else
                    return String.Empty;
            }
            set
            {
                ViewState[Constants.RequiredLabelStyle_VS] = value;
            }
        }

        #endregion

        #region ICOEDisplayMode


        /// <summary>
        /// <para>Sets or gets the current display mode of the form. Four types are supported:</para>
        /// <list type="bullet">
        ///     <item>Add Mode</item>
        ///     <item>Edit Mode</item>
        ///     <item>View Mode</item>
        /// </list>
        /// </summary>
        public CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.DisplayMode DisplayMode
        {
            get
            {
                return _displayMode;
            }
            set
            {
                if (_displayMode != value)
                {
                    _displayMode = value;
                }
            }
        }

        #endregion

        #region ICOECurrentFormMode
        
        /// <summary>
        /// <para>Sets or gets the current form mode of the form. Three types are supported:</para>
        /// <list type="bullet">
        ///     <item>Query Form</item>
        ///     <item>Detail Form</item>
        ///     <item>List Form</item>
        /// </list>
        /// </summary>
        public FormGroup.CurrentFormEnum CurrentFormMode
        {
            get 
            { 
                return _currentFormMode;
            }
            set
            {
                if (_currentFormMode != value)
                {
                    _currentFormMode = value;
                    this.PutData(null);
                }
            }
        }

        #endregion

        #region Life Cycle Events
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
           if (this.COEReadOnly == COEEditControl.ReadOnly)
            {
                this.Enabled = false;
            }
		    if (!string.IsNullOrEmpty(this.Label))
            {
                CambridgeSoft.COE.Framework.ServerControls.Utilities.LabelStylesParser.ConfigureLabelStyles(this.RequiredLabelStyle, this, _lit);
                _lit.Style.Add(HtmlTextWriterStyle.Display, "block");
                //this.Style.Add(HtmlTextWriterStyle.Display, "block");
                _lit.Text = this.Label;
                _lit.RenderControl(writer);
            }
            if (!string.IsNullOrEmpty(this.RequiredStyle))
            {
                CambridgeSoft.COE.Framework.ServerControls.Utilities.LabelStylesParser.SetRequiredControlStyles(this.RequiredStyle, this);
            }
            base.Render(writer);
            if (this.ShowHelp)
            {
                _helpImage = new Image();
                _helpImage.ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(ICOEGenerableControl), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.help.png");
                _helpImage.ID = this.ClientID + "HelpImage";
                _helpImage.Style.Add(HtmlTextWriterStyle.MarginLeft, "2px");
                _helpImage.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
                _helpImage.Attributes.Add("onclick", "showHide(YAHOO.COEFormGenerator." + _helpImage.ClientID + "Tooltip, " + _helpImage.ClientID + ");");
                _helpImage.RenderControl(writer);
                string yahooScript = "<script language='javascript' type='text/javascript'>YAHOO.namespace('COEFormGenerator');" +
                                     "YAHOO.COEFormGenerator." + _helpImage.ClientID + "Tooltip = new YAHOO.widget.Tooltip(\"" + _helpImage.ClientID + "Tooltip\", " +
                                     "{ context:\"" + _helpImage.ClientID + "\", text:\"" + FormatText(this.HelpText) + "\"," +
                                     "showdelay: 20000," +
                                     "hidedelay: 20000," +
                                     "visible:false});" +
                                     "YAHOO.COEFormGenerator." + _helpImage.ClientID + "Tooltip.isToBeShown = true;" +
                                     "</script>";
                writer.Write(yahooScript);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (this.ShowHelp)
                this.AddYUIScripts();
        }
        #endregion

        #region ICOEHelpContainer Members
        public bool ShowHelp
        {
            get
            {
                if (ViewState["ShowHelp"] != null)
                    return (bool)ViewState["ShowHelp"];
                else
                    return false;
            }
            set
            {
                ViewState["ShowHelp"] = value;
            }
        }

        public string HelpText
        {
            get
            {
                if (ViewState["HelpText"] != null)
                    return (string)ViewState["HelpText"];
                else
                    return string.Empty;
            }
            set
            {
                ViewState["HelpText"] = value;
            }
        }
        #endregion

        #region ICOEReadOnly Members
        /// <summary>
        /// EditControl Property implementation.
        /// </summary>
        public COEEditControl COEReadOnly
        {
            get
            {
                return _editControl;
            }
            set
            {
                _editControl = value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines whether [is A valid token] [the specified token].
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        /// 	<c>true</c> if [is A valid token] [the specified token]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsAValidToken(string token)
        {
            bool retVal = false;
            if (token.Equals("&&loggedInUser"))
                retVal = true;
            if (token.Equals("&&currentDate"))
                retVal = true;
            if (token.Equals("&&useSortOrderTop"))
                retVal = true;
            else if (token.Trim().Length >= 6)
            {
                if (token.Trim().Substring(0, 6).ToUpper() == "SELECT")
                    retVal = true;
            }
            return retVal;
        }

        /// <summary>
        /// Determines whether [is token available] [the specified token].
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        /// 	<c>true</c> if [is A valid token] [the specified token]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsItemAvailable(string token)
        {
            return this.Items.FindByValue(token) != null;
        }

        /// <summary>
        /// Converts the given token to a known converted value.
        /// </summary>
        /// <param name="token">The token to convert.</param>
        /// <returns>The real needed value</returns>
        private string ConvertToken(string token)
        {
            string retVal = string.Empty;
            if (token == "&&loggedInUser")
                retVal = COEUser.Name.ToUpper();
            else if (token == "&&useSortOrderTop")
            {
                if (this.Items.Count > 1)
                {
                    retVal = (this.Items[0].Text.ToString().ToUpper().Contains("SELECT")) ? this.Items[1].Text.ToString() : this.Items[0].Text.ToString();
                }
            }
            else if (token.Trim().Length >= 6)
            {
                if (token.Trim().Substring(0, 6).ToUpper() == "SELECT")
                {
                    if (searchDAL == null) { LoadDAL(ConnStringType.PROXY); }
                    System.Data.DataSet ds = searchDAL.ExecuteDataSet(this.ReplaceSpecialTokens(token));
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                            retVal = ds.Tables[0].Rows[0][0].ToString();
                    }
                }
            }
            return retVal;
        }



        private void AddYUIScripts()
        {
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.YAHOODOMEVENTS);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.DRAGDROPMIN);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.CONTAINERMIN);
            FrameworkUtils.AddYUICSSReference(this.Page, FrameworkConstants.YUI_CSS.CONTAINER);

            string showHideJS = @"
                function showHide(tooltip, contextid)
                {
                    var originalshowdelay = tooltip.cfg.getProperty('showdelay');
                    if(tooltip.isToBeShown)
                    {
                        tooltip.isToBeShown = false;
                        tooltip.cfg.setProperty('showdelay', 0);
                        tooltip.doShow(null, document.getElementById(contextid));
                    }
                    else
                    {
                        tooltip.isToBeShown = true;
                        tooltip.hide();
                    }

                    tooltip.cfg.setProperty('showdelay', originalshowdelay);
                }";
            if (!Page.ClientScript.IsStartupScriptRegistered(typeof(ICOEGenerableControl), "showHideJS"))
            {
                Page.ClientScript.RegisterStartupScript(typeof(ICOEGenerableControl), "showHideJS", showHideJS, true);
            }
        }

        private string FormatText(string txt)
        {
            return "<span style='font-size:9px;'>" + Page.Server.HtmlEncode(txt) + "</span>";
        }

        private string ReplaceSpecialTokens(string _sqlSelect)
        {
            if (_sqlSelect.Contains("&&loggedInUser"))
            {
                _sqlSelect = _sqlSelect.Replace("&&loggedInUser", "'" + COEUser.Name.ToUpper() + "'");
            }
            if (_sqlSelect.Contains("&&currentDate"))
            {
                _sqlSelect = _sqlSelect.Replace("&&currentDate", "'" + DateTime.Today.ToShortDateString() + "'");
            }

            return _sqlSelect;
        }

        #region Override Base class 
        public override object DataSource
        {
            get
            {
                return base.DataSource;
            }
            set
            {
                if (value.GetType() == typeof(System.String))
                {
                    _sqlSelect = (string)value;
                    if (_sqlSelect.Trim().Substring(0, 6).ToUpper() != "SELECT")
                    {
                        throw new Exception("An invalid select statement was specified for a drop down box");
                    }
                    //jhs right to the db ...
                    if (searchDAL == null) { LoadDAL(ConnStringType.PROXY); }
                    System.Data.DataSet ds = searchDAL.ExecuteDataSet(this.ReplaceSpecialTokens(_sqlSelect));
                    base.DataSource = ConvertToListPair(ds.Tables[0].Copy());
                }
                else if (value.GetType() == typeof(System.Data.DataSet))
                {
                    if (((System.Data.DataSet)value).Tables.Count == 0)
                    {
                        throw new Exception("An invalid datasource was specified for a drop down box.table not found");
                    }
                    base.DataSource = ConvertToListPair(((System.Data.DataSet)value).Tables[0].Copy());
                }
                else if (value.GetType() == typeof(System.Collections.Specialized.OrderedDictionary))
                {
                    base.DataSource = (IDictionary)value;
                }
                else
                    base.DataSource = value;
            }
        }

        public override void DataBind()
        {
            if (base.DataSource.GetType() == typeof(System.Collections.Specialized.OrderedDictionary))
            {
                this.Items.Clear();
                this.Items.Add(new ListItem("Select One", ""));
                foreach (DictionaryEntry keyValuePair in (System.Collections.Specialized.OrderedDictionary)base.DataSource)
                {
                    this.Items.Add(new ListItem(keyValuePair.Value.ToString(), keyValuePair.Key.ToString()));
                }
            }
            else
                base.DataBind();
        }
        #endregion

        /// <summary>
        /// Converts datatable to OrderedDictionary, used as datasoure for dropdown
        /// <returns>System.Collections.Specialized.OrderedDictionary</returns>
        /// </summary>
        private new System.Collections.Specialized.OrderedDictionary ConvertToListPair(System.Data.DataTable dt)
        {
            System.Collections.Specialized.OrderedDictionary retDictionary = new System.Collections.Specialized.OrderedDictionary(dt.Rows.Count);
            System.Data.DataView dv = dt.DefaultView;
            string keyColumn = dt.Columns.Contains("key") ? "key" : dt.Columns[0].ColumnName;
            string valueColumn = dt.Columns.Contains("value") ? "value" : dt.Columns[1].ColumnName;
            //conditionally apply a 'sort order' if none was specified in the original query
            if (_sqlSelect.IndexOf("order by", 0, StringComparison.InvariantCultureIgnoreCase) == -1)
                dv.Sort = valueColumn + " asc";
            //apply the values from the DataView 
            foreach (System.Data.DataRowView drv in dv)
            {
                string key = drv[keyColumn].ToString();
                string val = drv[valueColumn] == null ? string.Empty : System.Web.HttpUtility.HtmlDecode(drv[valueColumn].ToString());
                retDictionary.Add(key, val);
            }
            return retDictionary;
        }

        /// <summary>
        /// Load the DAL
        /// </summary>
        private void LoadDAL(ConnStringType connStringType)
        {
            if (dalFactory == null) { dalFactory = new DALFactory(); }
            //switch (connStringType)
            //{
            //    case ConnStringType.PROXY:
            //logged in user must have credential 
            this.dalFactory.GetDAL<CambridgeSoft.COE.Framework.COESearchService.DAL>(ref this.searchDAL, this.ServiceName, this._databaseName, true);

            //break;

            //    case ConnStringType.OWNERPROXY:
            //        //coedb is used for search
            //        //this.dalFactory.GetDAL<CambridgeSoft.COE.Framework.COESearchService.DAL>(ref this.searchDAL, this.ServiceName, Resources.CentralizedStorageDB, true);
            //        //break;

            //}

        }
        #endregion
    }

}
