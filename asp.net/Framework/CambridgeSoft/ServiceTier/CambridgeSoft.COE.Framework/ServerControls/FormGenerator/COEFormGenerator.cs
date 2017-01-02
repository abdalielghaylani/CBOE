using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Reflection;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using System.Xml;
using CambridgeSoft.COE.Framework.Common;
using System.Globalization;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common.Messaging;
using System.Text.RegularExpressions;
using CambridgeSoft.COE.Framework.ServerControls.FormGenerator;
using CambridgeSoft.COE.Framework.Controls.ChemDraw;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COELoggingService;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// COEFormGenerator is the core class of this namespace. It is a control that is capable of rendering dinamically a Form definition.
    /// That means that if that definition is changed in runtime, the resulting rendered form will automatically adopt the new layout and
    /// control's change.
    /// That Form Definition is specified by the class <see cref="COEForm"/> and defines which controls will be rendered in which modes and also where in the screen. Additionally binding expressions
    /// may be defined for each control to control the databinding process.
    /// This last means that the form generator may be bound to a Datasource (or datasourceid), and is capable of reading and writing values
    /// from and to the datasource.
    /// </para>
    /// </summary>
    public class COEFormGenerator : CompositeDataBoundControl, ICOEContainer
    {
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEFormGenerator");

        #region Variables
        private CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.Form _formData;
        private List<ICOEGenerableControl> _userControls = new List<ICOEGenerableControl>();
        private bool _dataBinding;
        private Panel _container;
        private HtmlGenericControl _formGenDebuggingInfo = new HtmlGenericControl("div");
        private object _dataItem;
        private int _pageIndex;
        private int _pagesCount;
        private CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.DisplayMode _displayMode;
        private FormGroup.CurrentFormEnum _currentFormMode = FormGroup.CurrentFormEnum.QueryForm;
        private IDictionary _oldValues;
        private static readonly object _itemUpdatedEvent = new object();
        private static readonly object _itemUpdatingEvent = new object();
        #endregion

        #region Properties
        public int PagesCount
        {
            get
            {
                return _pagesCount;
            }
        }
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
                    RequiresDataBinding = true;
                }
            }
        }

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
            get { return _currentFormMode; }
            set
            {
                if (_currentFormMode != value)
                {
                    _currentFormMode = value;
                }
            }
        }

        /// <summary>
        /// <para>
        /// The index of the current page to be displayed. 
        /// </para>
        /// </summary>
        public int PageIndex
        {
            get { return _pageIndex; }
            set
            {
                EnsureDataBound();

                value = Math.Max(0, Math.Min(value, _pagesCount - 1));

                _pageIndex = value;
                RequiresDataBinding = true;
            }
        }

        /// <summary>
        /// <para>
        /// The messaging type description of the form. This is the main property which will determine how the form will rendered at runtime.
        /// </para>
        /// </summary>
        public CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.Form FormDescription
        {
            get
            {
                return _formData;
            }
            set
            {
                _formData = value;
            }
        }

        public string DisplayCulture
        {
            set
            {
                if (_formData.DisplayCulture == string.Empty)
                    _formData.DisplayCulture = value;
            }
        }
        public override Unit Height
        {
            get
            {
                if (this.FormDescription.FormDisplay.FitToContents)
                    return Unit.Empty;

                Unit maxHeight = new Unit(0);

                if (!string.IsNullOrEmpty(this.FormDescription.FormDisplay.Height))
                {
                    maxHeight = new Unit(this.FormDescription.FormDisplay.Height);
                }
                else
                {
                    maxHeight = Unit.Empty;
                }
                return maxHeight;
            }
            set
            {
                this.FormDescription.FormDisplay.Height = value.ToString();
            }
        }

        public override Unit Width
        {
            get
            {
                if (this.FormDescription.FormDisplay.FitToContents)
                    return Unit.Empty;

                Unit maxWidth = new Unit(0);

                if (!string.IsNullOrEmpty(this.FormDescription.FormDisplay.Width))
                {
                    maxWidth = new Unit(this.FormDescription.FormDisplay.Width);
                }
                else
                {
                    maxWidth = Unit.Empty;
                }

                return maxWidth;
            }
            set
            {
                this.FormDescription.FormDisplay.Width = value.ToString();
            }
        }

        private void ToggleEnable(WebControl currentControl, bool value)
        {
            currentControl.Enabled = value;

            if (currentControl is COEChemDrawEmbed)
                ((COEChemDrawEmbed)currentControl).ViewOnly = !value;
            else if (currentControl is TextBox)
                ((TextBox)currentControl).ReadOnly = !value;
            else if (currentControl is COEWebGridUltra)
                ((COEWebGridUltra)currentControl).ReadOnly = true;
            else if (currentControl is COEFragments)
                ((COEFragments)currentControl).ReadOnly = true;

            foreach (Control control in currentControl.Controls)
                if (control is WebControl)
                    ToggleEnable((WebControl)control, value);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// <para>When the form is not bound to a datasourceid, but a datasource ItemUpdated event is fired when the Update process ends.</para>
        /// </summary>
        public event COEFGEventHandler ItemUpdated
        {
            add { Events.AddHandler(_itemUpdatedEvent, value); }
            remove { Events.RemoveHandler(_itemUpdatedEvent, value); }
        }

        /// <summary>
        /// <para>When the form is not bound to a datasourceid, but a datasource ItemUpdating event is fired when the Update process is in progress.</para>
        /// </summary>
        public event COEFGEventHandler ItemUpdating
        {
            add { Events.AddHandler(_itemUpdatingEvent, value); }
            remove { Events.RemoveHandler(_itemUpdatingEvent, value); }
        }

        public event MarkingHitHandler MarkingHit;
        #endregion

        #region Events
        void COEFormGenerator_MarkingHit(object sender, MarkHitEventArgs eventArgs)
        {
            if (MarkingHit != null)
                MarkingHit(sender, eventArgs);
        }
        #endregion

        #region control Events
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.RegisterRequiresControlState(this);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (_container != null && HasControlsToRender())
            {
                _container.RenderControl(writer);
                if (this.FormDescription.FormDisplay.FitToContents)
                    writer.Write("<script language='javascript'>fitToContents(new getObj('" + _container.ClientID + "').obj);</script>");
                _formGenDebuggingInfo.RenderControl(writer);
            }
        }

        private bool HasControlsToRender()
        {
            bool hasControlsToRender = false;
            foreach (Control ctrl in _container.Controls)
            {
                if (!(ctrl is IValidator))
                {
                    if (string.IsNullOrEmpty(ctrl.ID))
                        return true;
                    else
                        if (!ctrl.ID.Contains("FormGeneratorTitle"))
                            return true;

                }
            }
            return hasControlsToRender;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.ClientScript.IsStartupScriptRegistered(Constants.JSFunction_ShowErrorDivName))
            {
                Page.ClientScript.RegisterStartupScript(typeof(string), Constants.JSFunction_ShowErrorDivName,
                    Constants.JSFunction_ShowErrorDiv, true);
            }
            if (!Page.ClientScript.IsStartupScriptRegistered(Constants.JSFunction_HideErrorDivName))
            {
                Page.ClientScript.RegisterStartupScript(typeof(string), Constants.JSFunction_HideErrorDivName,
                    Constants.JSFunction_HideErrorDiv, true);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!this.Enabled)
            {
                this.ToggleEnable(this, false);

                if (this._container != null)
                    this._container.Enabled = false;
            }

            //Moved form scripts to prerender due to possible pageCount and pageIndex changes that could generate wrong control names. 
            //Here we're granted that neither pageCount nor pageIndex will change after generating the scripts.
            if (_formData != null)
                this.RegisterToFormClientEvents(_formData.ClientScripts);

        }

        protected override void LoadControlState(object savedState)
        {
            object[] savedStates = (object[])savedState;

            int currentIndex = 0;
            base.LoadControlState(savedStates[currentIndex++]);
            //this._oldValues = (IDictionary)savedStates[currentIndex++];
            _pageIndex = (int)savedStates[currentIndex++];
            _displayMode = (CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.DisplayMode)savedStates[currentIndex++];
            _pagesCount = (int)savedStates[currentIndex++];
        }

        protected override object SaveControlState()
        {
            object[] savedState = new object[5];

            int currentIndex = 0;
            savedState[currentIndex++] = base.SaveControlState();
            //savedState[currentIndex++] = _oldValues;
            savedState[currentIndex++] = _pageIndex;
            savedState[currentIndex++] = _displayMode;
            savedState[currentIndex++] = _pagesCount;

            return savedState;
        }

        protected override int CreateChildControls(IEnumerable data, bool dataBinding)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            _dataBinding = dataBinding;
            _userControls = new List<ICOEGenerableControl>();
            int currentPage = 0;
            this.Controls.Clear();
            //a whole form can be disabled
            if (_formData != null && _formData.FormDisplay.Visible)
            {
                if (data != null)
                {
                    if (_formData.FormDisplay.FitToContents)
                    {
                        if (!Page.ClientScript.IsClientScriptIncludeRegistered("positioningScripts"))
                            Page.ClientScript.RegisterClientScriptInclude("positioningScripts", Page.ClientScript.GetWebResourceUrl(typeof(COEFormGenerator), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.JSAndCSS.positioning.js"));
                    }

                    IEnumerator e = data.GetEnumerator();

                    if (!string.IsNullOrEmpty(_formData.DataMember))
                    {
                        if (dataBinding)
                        {
                            if (e.MoveNext())
                            {
                                COEDataBinder dataBinder = new COEDataBinder(e.Current);

                                data = (IEnumerable)dataBinder.RetrieveProperty(_formData.DataMember);
                            }
                        }
                    }

                    if (dataBinding)
                    {
                        _pagesCount = 0;

                        e = data.GetEnumerator();
                        while (e.MoveNext())
                            _pagesCount++;
                    }


                    e = data.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (_pageIndex == currentPage++)
                        {
                            BuildFormGenerator(e.Current);
                            break;
                        }
                    }
                }
            }

            if (FrameworkUtils.GetAppConfigSetting(GUIShellUtilities.GetApplicationName(), "MISC", "ENABLE_COEFORM_DEBUG_INFO").ToLower() == bool.TrueString.ToLower())
                this.AddDebuggingInfo();

            _coeLog.LogEnd(methodSignature);
            return _pagesCount;
        }

        private void AddDebuggingInfo()
        {
            _formGenDebuggingInfo.ID = "FormGeneratorDebuggingInfo";

            Label adminInformation = new Label();

            adminInformation.Style.Add(HtmlTextWriterStyle.Position, "relative");
            adminInformation.Style.Add("float", "right");
            adminInformation.Style.Add(HtmlTextWriterStyle.FontFamily, "verdana");
            adminInformation.Style.Add(HtmlTextWriterStyle.FontSize, "8px");
            adminInformation.Style.Add(HtmlTextWriterStyle.Color, "gray");
            adminInformation.Text = string.Format(Resources.ID + ": {0}, {1}" + Resources.Index + ": {2}",
                                                    this._formData.Id,
                                                    string.IsNullOrEmpty(this._formData.Name) ? string.Empty : Resources.Name + ": " + _formData.Name + ", ",
                                                    this._pageIndex);
            _formGenDebuggingInfo.Controls.Add(adminInformation);
            _formGenDebuggingInfo.Controls.Add(new HtmlGenericControl("br"));
            this.Controls.Add(_formGenDebuggingInfo);
        }

        /// <summary>
        /// Returns the full id (with the text added for pagination)
        /// </summary>
        /// <param name="name">Control ID</param>
        /// <returns></returns>
        public string NormalizedFormElementName(string name)
        {
            return name + _pageIndex.ToString() + "OF" + _pagesCount.ToString();
        }

        private void BuildFormGenerator(object currentItem)
        {
            List<FormGroup.FormElement> formElements = _formData.GetFormElements(this.DisplayMode);

            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            this.CreateContainer(_formData.FormDisplay);
            if (currentItem != null)
            {
                _dataItem = currentItem;


                COEDataBinder dataBinder = new COEDataBinder(_dataItem);

                _oldValues.Clear();

                foreach (FormGroup.FormElement formElement in formElements)
                {
                    if (formElement.DisplayInfo.Visible)
                    {
                        //if (this.DisplayMode != CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.DisplayMode.Add)
                        //{
                        if (!string.IsNullOrEmpty(formElement.BindingExpression))

                            if (!_oldValues.Contains(formElement.BindingExpression))
                            {
                                object value = null;
                                try
                                {
                                    value = dataBinder.RetrieveProperty(formElement.BindingExpression);
                                    if (value == null)
                                        value = string.Empty;
                                }
                                catch (Exception exception)
                                {
                                    value = string.Format("Invalid Property '{0}'", formElement.BindingExpression);
                                }

                                _oldValues.Add(formElement.BindingExpression, value);
                            }
                        //}
                        if (!string.IsNullOrEmpty(formElement.DataSource))
                            if (!_oldValues.Contains(formElement.DataSource))
                                _oldValues.Add(formElement.DataSource,
                                                    dataBinder.RetrieveProperty(formElement.DataSource));
                    }
                }
            }

            List<FormGroup.FormElement> visibleFormElements = new List<FormGroup.FormElement>();
            foreach (CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.FormElement formElement in formElements)
            {
                if (formElement.DisplayInfo.Visible)
                {
                    visibleFormElements.Add(formElement);
                    InitializeChild(formElement, _oldValues, _formData.FormDisplay.LayoutStyle, currentItem);
                }
            }

            //Registers Scripts and validations
            this.RegisterScripts(visibleFormElements);
            visibleFormElements.Clear();
            this.RegisterFormValidations(_formData.ValidationRuleList);
            _coeLog.LogEnd(methodSignature);
        }

        private void RegisterFormValidations(List<FormGroup.ValidationRuleInfo> list)
        {
            foreach (CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.ValidationRuleInfo coeValidationRule in list)
            {
                // Below condition commented for feature request COE:Search in CSBR-158089 and Validation Message in CSBR-158084.
                //if (coeValidationRule.ValidationRuleName != FormGroup.ValidationRuleEnum.NotEmptyQuery || !Page.ClientScript.IsClientScriptBlockRegistered(typeof(COEFormGenerator), coeValidationRule.ValidationRuleName.ToString()))
                //{  Implemented for CSBR-108034,CSBR-108322
                    BaseValidator validator = COEValidatorFactory.GetValidator(coeValidationRule, this, this);
                    //Coverity Bug Fix CID 11654 
                    if (validator != null)
                    {
                        validator.ControlToValidate = string.Empty;
                        _container.Controls.Add(validator);
                    }
                //}
            }
        }

        /// <summary>
        /// Registers validations, client side & server side events of controls. This is performed after all controls have been created ( this is a must).
        /// </summary>
        /// <param name="formElements"></param>
        private void RegisterScripts(List<CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.FormElement> formElements)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);

            for (int index = 0; index < formElements.Count; index++)
            {
                FormGroup.FormElement currentFormElement = formElements[index];
                if (currentFormElement.DisplayInfo.Visible)
                {
                    if (_userControls[index] != null && currentFormElement.ClientEvents.Count > 0)
                    {
                        this.RegisterToControlClientEvents(_userControls[index], currentFormElement.ClientEvents);
                    }

                    if (_userControls[index] != null && currentFormElement.ServerEvents.Count > 0)
                    {
                        this.RegisterToControlServerEvents(_userControls[index], currentFormElement.ServerEvents);
                    }

                    if (_userControls[index] != null && currentFormElement.ValidationRuleList.Count > 0)
                    {
                        this.AddValidatorToControl((Panel)((WebControl)_userControls[index]).Parent, currentFormElement.ValidationRuleList, (WebControl)_userControls[index]);
                    }
                }
            }
            _coeLog.LogEnd(methodSignature);
        }

        public static ICOEGenerableControl GetCOEGenerableControl(string formElementXml, out string errorMessage)
        {
            try
            {
                string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
                _coeLog.LogStart(methodSignature + "formElementXml", 1, System.Diagnostics.SourceLevels.All);
                ICOEGenerableControl control = GetCOEGenerableControl(FormGroup.FormElement.GetFormElement(formElementXml), out errorMessage);
                _coeLog.LogEnd(methodSignature + "formElementXml");
                return control;
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;

                return null;
            }
        }

        public static int currentUnnamed = 0;
        public static ICOEGenerableControl GetCOEGenerableControl(FormGroup.FormElement formElement, out string errorMessage)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature + "formElement.Id=" + formElement.Id + ", formElement.Name=" + formElement.Name, 1, System.Diagnostics.SourceLevels.All);
            ICOEGenerableControl control = InstanciateControl(formElement.DisplayInfo, out errorMessage);

            if ((control != null))
            {
                if (!string.IsNullOrEmpty(formElement.Id))
                    ((Control)control).ID = formElement.Id.Trim();
                else if (!string.IsNullOrEmpty(formElement.Name))
                    ((Control)control).ID = formElement.Name + control.GetType().Name;


                if (string.IsNullOrEmpty(((Control)control).ID))
                {
                    errorMessage = "FormElement must have whether Id or Name:\n " + formElement.ToString();
                    return null;
                }

                if (!string.IsNullOrEmpty(formElement.DataSourceId))
                    ((DataBoundControl)control).DataSourceID = formElement.DataSourceId;

                if (formElement.ConfigInfo != null)
                {
                    /*Regex regex = new Regex(" xmlns=\".*?\"");
                    control.LoadFromXml(regex.Replace(formElement.ConfigInfo.OuterXml, ""));*/
                    control.LoadFromXml(formElement.ConfigInfo.OuterXml);
                }

                //if control is ILabeleable we have to set its label that is supposed to be in the xml.
                if (control is ICOELabelable)
                {
                    if (!string.IsNullOrEmpty(formElement.Label))
                    {
                        ((ICOELabelable)control).Label = formElement.Label;
                    }
                }

                if (control is ICOEHelpContainer)
                {
                    if (!string.IsNullOrEmpty(formElement.HelpText))
                    {
                        ((ICOEHelpContainer)control).HelpText = formElement.HelpText;
                        ((ICOEHelpContainer)control).ShowHelp = formElement.ShowHelp;
                    }
                }
                //Controls that will display an upload control when clicked.
                if (control is ICOEFileUpload)
                {
                    if (formElement.IsFileUpload)
                    {
                        ((ICOEFileUpload)control).IsFileUpload = true;
                        ((ICOEFileUpload)control).PageComunicationProvider = formElement.PageComunicationProvider;
                        ((ICOEFileUpload)control).FileUploadBindingExpression = formElement.FileUploadBindingExpression;
                    }
                }
            }

            _coeLog.LogEnd(methodSignature + "formElement.Id=" + formElement.Id + ", formElement.Name=" + formElement.Name);
            return control;
        }

        int currentErrorLabel = 0;
        private void InitializeChild(CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.FormElement formElement, IDictionary dataBoundValues, FormGroup.LayoutStyle layoutStyle, object fullDataSource)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            if (formElement != null)
            {
                //Control Container
                 Panel pan = new Panel();
                _container.Controls.Add(pan);

                if (!string.IsNullOrEmpty(formElement.DisplayInfo.Height))
                    pan.Height = new Unit(formElement.DisplayInfo.Height); //pan.Style.Add("height", formElement.DisplayInfo.Height);

                if (!string.IsNullOrEmpty(formElement.DisplayInfo.Width))
                    pan.Width = new Unit(formElement.DisplayInfo.Width);
              
              
                if (!string.IsNullOrEmpty(formElement.DisplayInfo.Left))
                    pan.Style.Add("left", formElement.DisplayInfo.Left);

                if (!string.IsNullOrEmpty(formElement.DisplayInfo.Style))
                    pan.Style.Value += formElement.DisplayInfo.Style;

                if (!string.IsNullOrEmpty(formElement.DisplayInfo.CSSClass))
                    pan.CssClass = formElement.DisplayInfo.CSSClass;


                if (formElement.DisplayInfo.Position != null && formElement.DisplayInfo.Position != string.Empty)
                    pan.Style.Add("position", formElement.DisplayInfo.Position);
                else
                {
                    switch (layoutStyle)
                    {
                        case FormGroup.LayoutStyle.AbsoluteLayout:
                            pan.Style.Add("position", "absolute");
                            break;
                        case FormGroup.LayoutStyle.FlowLayout:
                            pan.Style.Add("position", "static");
                            pan.Style.Add("float", "left");
                            pan.Style.Add(HtmlTextWriterStyle.Padding, "2px");
                            break;
                    }
                }

                //Control Creation through reflexion
                string controlToAddErrorMessage;
                ICOEGenerableControl controlToAdd = GetCOEGenerableControl(formElement, out controlToAddErrorMessage);
                //Control Initialization
                if (controlToAdd != null)
                {
                    ((Control)controlToAdd).Page = this.Page;
                    ((Control)controlToAdd).ID = this.NormalizedFormElementName(((Control)controlToAdd).ID);
                    _coeLog.LogStart("COEFormGenerator->INITIALIZECHILD: controlToAdd=" + ((Control)controlToAdd).ID, 1, System.Diagnostics.SourceLevels.All);
                    
                    pan.Controls.Add((Control)controlToAdd);
                    
                    //If control is databound, find it's corresponding datasource. If both datasource and datasourceId are set, an _exception will be thrown
                    if (controlToAdd is DataBoundControl)
                    {
                        if (!string.IsNullOrEmpty(formElement.DataSource))
                        {
                            if (dataBoundValues != null)
                            {
                                ((DataBoundControl)controlToAdd).DataSource = dataBoundValues[formElement.DataSource];
                                ((DataBoundControl)controlToAdd).DataBind();
                            }
                        }
                        if (!string.IsNullOrEmpty(formElement.DataSourceId))
                        {
                            ((DataBoundControl)controlToAdd).DataSourceID = formElement.DataSourceId;
                        }
                    }

                    if (controlToAdd is ICOECultureable)
                    {
                        if (!string.IsNullOrEmpty(((CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.Form)_formData).DisplayCulture))
                            ((ICOECultureable)controlToAdd).DisplayCulture = CultureInfo.GetCultureInfoByIetfLanguageTag(_formData.DisplayCulture);
                        else if (this.Page.Session["DisplayCulture"] != null && this.Page.Session["DisplayCulture"].ToString() != string.Empty)
                            ((ICOECultureable)controlToAdd).DisplayCulture = CultureInfo.GetCultureInfoByIetfLanguageTag(this.Page.Session["DisplayCulture"].ToString());
                    }

                    if (controlToAdd is ICOEHitMarker)
                    {
                        ((ICOEHitMarker)controlToAdd).MarkingHit += new MarkingHitHandler(COEFormGenerator_MarkingHit);
                        if (!string.IsNullOrEmpty(((ICOEHitMarker)controlToAdd).ColumnIDBindingExpression) && _dataItem != null)
                        {
                            COEDataBinder databinder = new COEDataBinder(_dataItem);
                            ((ICOEHitMarker)controlToAdd).ColumnIDValue = databinder.RetrieveProperty(((ICOEHitMarker)controlToAdd).ColumnIDBindingExpression).ToString();
                        }
                    }

                    if (controlToAdd is ICOEFullDatasource)
                        ((ICOEFullDatasource)controlToAdd).FullDatasource = fullDataSource;

                    if (controlToAdd is ICOERequireable)
                    {
                        if (!string.IsNullOrEmpty(formElement.RequiredStyle))
                        {
                            ((ICOERequireable)controlToAdd).RequiredStyle = formElement.RequiredStyle;
                        }
                        if (!string.IsNullOrEmpty(formElement.RequiredLabelStyle) && controlToAdd is ICOELabelable)
                        {
                            ((ICOERequireable)controlToAdd).RequiredLabelStyle = formElement.RequiredLabelStyle;
                        }
                        FormGroup.ValidationRuleInfo validationRule = new FormGroup.ValidationRuleInfo();
                        validationRule.ValidationRuleName = FormGroup.ValidationRuleEnum.RequiredField;
                        if (string.IsNullOrEmpty(formElement.RequiredStyle) && string.IsNullOrEmpty(formElement.RequiredLabelStyle) && formElement.ValidationRuleList.Find(delegate(FormGroup.ValidationRuleInfo f) { return f.ValidationRuleName == FormGroup.ValidationRuleEnum.RequiredField; }) != null)
                        {
                            if (controlToAdd is ICOELabelable)
                                ((ICOERequireable)controlToAdd).RequiredLabelStyle = Resources.DefaultRequiredLabelStyle;
                            else
                                ((ICOERequireable)controlToAdd).RequiredStyle = Resources.DefaultRequiredStyle;
                        }
                    }

                    if (controlToAdd is ICOEDisplayMode)
                        ((ICOEDisplayMode)controlToAdd).DisplayMode = this._displayMode;

                    if (controlToAdd is ICOECurrentFormMode)
                        ((ICOECurrentFormMode)controlToAdd).CurrentFormMode = this.CurrentFormMode;

                    if (!string.IsNullOrEmpty(formElement.DefaultValue))
                        controlToAdd.PutData(formElement.DefaultValue);

                    if (!string.IsNullOrEmpty(formElement.BindingExpression))
                    {
                        if (this.DisplayMode != CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.DisplayMode.Add)
                        {
                            // Coverity Fix CID - 11653
                            if (dataBoundValues != null)
                            {
                                object value = dataBoundValues[formElement.BindingExpression];
                                if (value != null)
                                {
                                    _coeLog.LogStart("COEFormGenerator->INITIALIZECHILD: controlToAdd.PutData", 1, System.Diagnostics.SourceLevels.All);
                                    controlToAdd.PutData(value);
                                    _coeLog.LogEnd("COEFormGenerator->INITIALIZECHILD: controlToAdd.PutData", 1, System.Diagnostics.SourceLevels.All);
                                }
                            }
                        }

                        ((WebControl)controlToAdd).Attributes["BindingExpression"] = formElement.BindingExpression;
                    }
                    _coeLog.LogEnd("COEFormGenerator->INITIALIZECHILD: controlToAdd=" + ((Control)controlToAdd).ID, 1, System.Diagnostics.SourceLevels.All);
                }
                else
                {
                    _coeLog.LogStart("COEFormGenerator->INITIALIZECHILD: controlToAdd=NULL", 1, System.Diagnostics.SourceLevels.All);
                    Label controlNotFoundLabel = new Label();
                    pan.Controls.Add(controlNotFoundLabel);

                    controlNotFoundLabel.ID = "ControlNotFound_" + (currentErrorLabel++).ToString();
                    controlNotFoundLabel.Text = Resources.UnableToLoadControl;
                    controlNotFoundLabel.ToolTip = controlToAddErrorMessage;
                    controlNotFoundLabel.Font.Size = FontUnit.XXSmall;

                    pan.Style.Add("border-color", Constants.ErrorMsg_BorderColor);
                    pan.Style.Add("border-width", "1px");
                    pan.Style.Add("border-style", Constants.ErrorMsg_BorderStyle);
                    pan.Style.Add("color", Constants.ErrorMsg_TextColor);
                    pan.Style.Add("background-color", Constants.ErrorMsg_BackGroundColor);
                    pan.Attributes.Add("tabindex", Constants.ErrorMsg_TabIndex);
                    pan.Style.Add("z-index", Constants.ErrorMsg_ZIndex);
                    _coeLog.LogEnd("COEFormGenerator->INITIALIZECHILD: controlToAdd=NULL", 1, System.Diagnostics.SourceLevels.All);
                }
                _userControls.Add(controlToAdd);
            }
            _coeLog.LogEnd(methodSignature);
        }
                
        private static ICOEGenerableControl InstanciateControl(FormGroup.DisplayInfo displayInfo, out string controlToAddErrorMessage)
        {
            ICOEGenerableControl controlToAdd = null;

            string type = string.IsNullOrEmpty(displayInfo.Type) ? "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELabel" :
                                                                    displayInfo.Type.Trim();

            try
            {
                controlToAddErrorMessage = string.Empty;
                if (!string.IsNullOrEmpty(displayInfo.Assembly))
                {
                    System.Reflection.Assembly assembly = null;
                    if (displayInfo.Assembly.Trim().IndexOf((char)47) != -1)
                    {
                        assembly = System.Reflection.Assembly.LoadFrom(System.Web.HttpContext.Current.Server.MapPath(displayInfo.Assembly.Trim()));
                    }
                    else
                    {
                        assembly = System.Reflection.Assembly.Load(displayInfo.Assembly.Trim());
                    }
                    controlToAdd = (ICOEGenerableControl)assembly.CreateInstance(type);

                    if (controlToAdd == null)
                        controlToAddErrorMessage = string.Format("{0}: {1}", Resources.ControlNotFound, type);
                }
                else
                {
                    Type controlToAddClass = Type.GetType(type);
                    if (controlToAddClass == null)
                    {
                        controlToAddErrorMessage = string.Format("{0}: {1}", Resources.ClassNotFound, type);
                    }
                    else
                    {

                        ConstructorInfo controlToAddDefaultConstructor = controlToAddClass.GetConstructor(System.Type.EmptyTypes);
                        if (controlToAddDefaultConstructor == null)
                        {
                            controlToAddErrorMessage = string.Format("{0}: {1}", Resources.MissingDefaultConstructor, type);
                        }
                        else
                        {
                            controlToAdd = (ICOEGenerableControl)controlToAddDefaultConstructor.Invoke(null);

                            if (controlToAdd == null)
                            {
                                controlToAddErrorMessage = string.Format("{0}: {1}", Resources.ControlNotFound, type);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                controlToAddErrorMessage = exception.Message;
            }
            return controlToAdd;

        }

        private void AddValidatorToControl(Panel pan, List<CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.ValidationRuleInfo> list, WebControl controlToAdd)
        {
            Label errorDiv = new Label();
            errorDiv.ID = Constants.ErrorMsg_PrefixId + controlToAdd.ID;

            pan.Controls.Add(errorDiv);

            errorDiv.CssClass = "COEErrorDiv"; //Default CSSClass - Todo: Set this dynamically.
            if (_formData.FormDisplay.LayoutStyle == FormGroup.LayoutStyle.AbsoluteLayout)
            {
                errorDiv.Style.Add("top", "0px");
                errorDiv.Style.Add("left", "0px");
            }
            errorDiv.Attributes.Add("tabindex", Constants.ErrorMsg_TabIndex);
            errorDiv.Attributes.Add("onblur", "hideErrorDiv(this.id);");
            errorDiv.Attributes.Add("onmouseout", "hideErrorDiv(this.id);");

            foreach (CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.ValidationRuleInfo coeValidationRule in list)
            {
                CreateValidatorFromRule(coeValidationRule, pan, controlToAdd, errorDiv);
                controlToAdd.Attributes.Add("onblur", "showErrorDiv('" + errorDiv.ClientID + "');");
            }

            controlToAdd.Style.Add(HtmlTextWriterStyle.Display, "inline");
        }

        private void CreateValidatorFromRule(FormGroup.ValidationRuleInfo coeValidationRule, Panel pan, WebControl controlToAdd, WebControl errorDiv)
        {
            BaseValidator validator = COEValidatorFactory.GetValidator(coeValidationRule, controlToAdd, this);
            if (validator != null)
            {
                pan.Controls.Add((Control)validator);


                errorDiv.Attributes.Add(
                    Constants.ErrorMsg_ValidatorsAttribute,
                    errorDiv.Attributes[Constants.ErrorMsg_ValidatorsAttribute] == null ? ((Control)validator).ClientID :
                    errorDiv.Attributes[Constants.ErrorMsg_ValidatorsAttribute] + "," + ((Control)validator).ClientID);

                validator.Text = string.Format("<img src='{0}'/>", Page.ClientScript.GetWebResourceUrl(typeof(COEFormGenerator), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.error.png"));
                validator.Style.Add("cursor", Constants.Validator_Cursor);
                validator.Style.Add("display", Constants.Validator_Display);

                if (this.FormDescription.FormDisplay.LayoutStyle == FormGroup.LayoutStyle.AbsoluteLayout)
                    validator.Style.Add("position", Constants.Validator_Position);
                else
                {
                    validator.Style.Add("position", "inherit");
                    controlToAdd.Style.Add("float", "left");
                }

                validator.Style.Add("top", GetErrorDivTop(coeValidationRule.DisplayPosition, controlToAdd.Height.Value) + "px");
                validator.Style.Add("left", GetErrorDivLeft(coeValidationRule.DisplayPosition, controlToAdd.Width.Value) + "px");

                validator.Style.Add("height", Constants.Validator_Height);
                validator.Style.Add("width", Constants.Validator_Width);
                //CSBR-126868
                validator.Style.Add("left", "-70px");
                validator.Style.Add("float", "right");
                validator.Style.Add("margin-top", "-40px");
                validator.Attributes.Add("onmouseover", "showErrorDiv('" + errorDiv.ClientID + "');");
            }

        }

        /// <summary>
        /// Set the top position based in the given displayPosition
        /// </summary>
        /// <param name="displayPosition">what corner to take as origin</param>
        /// <param name="height">Height of the control</param>
        /// <returns>Top position to see the div</returns>
        private string GetErrorDivTop(FormGroup.DisplayPosition displayPosition, double height)
        {

            string top = Constants.Validator_Top_Margin.ToString();

            if (displayPosition == FormGroup.DisplayPosition.Bottom_Left || displayPosition == FormGroup.DisplayPosition.Bottom_Right)
            {
                if (this.FormDescription.FormDisplay.LayoutStyle == FormGroup.LayoutStyle.AbsoluteLayout)
                    top = Convert.ToString(((int)height + Constants.Validator_Top_Margin));
                else
                    top = "0";
            }
            else if (displayPosition == FormGroup.DisplayPosition.Top_Left || displayPosition == FormGroup.DisplayPosition.Top_Right)
                if (this.FormDescription.FormDisplay.LayoutStyle == FormGroup.LayoutStyle.AbsoluteLayout)
                    top = "0";
                else
                    top = Convert.ToString(-((int)height + Constants.Validator_Top_Margin));

            return top;

        }

        /// <summary>
        /// GetKeyInCSSCollection(pan.Style,"top")
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        private int GetKeyInCSSCollection(CssStyleCollection collection, string paramName)
        {
            int paramStartIndex = collection.Value.IndexOf(paramName);
            int paramEndIndex = collection.Value.IndexOf(";", paramStartIndex) - 2;
            string tempResult = collection.Value.Substring(paramStartIndex + paramName.Length, paramEndIndex - paramStartIndex);
            tempResult = Regex.Replace(tempResult, @"[a-zA-Z;:]", String.Empty);
            return Convert.ToInt32(tempResult);
        }

        /// <summary>
        /// Set the left position based in the given displayPosition
        /// </summary>
        /// <param name="displayPosition">what corner to take as origin</param>
        /// <param name="height">Width of the control</param>
        /// <returns>Left position to see the div</returns>
        private string GetErrorDivLeft(FormGroup.DisplayPosition displayPosition, double width)
        {
            if (displayPosition == FormGroup.DisplayPosition.Bottom_Right || displayPosition == FormGroup.DisplayPosition.Top_Right)
                return Convert.ToString(((int)width + Constants.Validator_Left_Margin));
            else if (displayPosition == FormGroup.DisplayPosition.Top_Left || displayPosition == FormGroup.DisplayPosition.Bottom_Left)
                return "0";
            else
                return Constants.Validator_Left_Margin.ToString();
        }

        char[] separator = { '\"', '\'', '@', ' ', ',', /*'.',*/ '|', '!', '\\', '%', '&', '/', '(', ')', '=', '?', '*', '-', '+', ';', ':', '<', '>', ';' };

        private void RegisterToControlClientEvents(ICOEGenerableControl controlToAdd, List<FormGroup.COEEventInfo> list)
        {
            foreach (FormGroup.COEEventInfo currentEvent in list)
            {
                string eventHandlerScript = !string.IsNullOrEmpty(currentEvent.EventHandlerScript) ? currentEvent.EventHandlerScript : currentEvent.Value;

                if (!string.IsNullOrEmpty(currentEvent.EventHandlerName))
                {
                    ((WebControl)controlToAdd).Attributes.Add(currentEvent.EventName,
                                                                                string.Format("return {0}(this, event)", currentEvent.EventHandlerName));
                    //maybe we should allow the user to specify the method handler code here too, and register it with the page.
                }
                else if (!string.IsNullOrEmpty(eventHandlerScript))
                {
                    string scriptCode = ReplaceControlNames(eventHandlerScript);

                    ((WebControl)controlToAdd).Attributes.Add(currentEvent.EventName,
                                                                               scriptCode);
                }
            }
        }

        private void RegisterToFormClientEvents(List<FormGroup.COEEventInfo> list)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            foreach (FormGroup.COEEventInfo currentEvent in list)
            {
                string eventHandlerScript = !string.IsNullOrEmpty(currentEvent.EventHandlerScript) ? currentEvent.EventHandlerScript : currentEvent.Value;

                if (!string.IsNullOrEmpty(eventHandlerScript) && _container != null)
                {
                    string script = ReplaceContextVariables(ReplaceControlNames(string.Format("if(document.getElementById('{0}') != null) {{ {1} }}", _container.ClientID, eventHandlerScript)));
                    if (!string.IsNullOrEmpty(script))
                    {
                        switch (currentEvent.EventName.Trim().ToLower())
                        {
                            case "onsubmit":
                                Page.ClientScript.RegisterOnSubmitStatement(typeof(string), this.ID + "_" + currentEvent.EventName.Trim(), script);
                                break;
                            case "startup":
                                if (!Page.ClientScript.IsStartupScriptRegistered(this.ID + "_" + currentEvent.EventName.Trim()))
                                    Page.ClientScript.RegisterStartupScript(typeof(string), this.ID + "_" + currentEvent.EventName.Trim(), script, true);
                                break;
                            default:
                                if (!string.IsNullOrEmpty(currentEvent.EventName))
                                    if (currentEvent.EventName.Contains("("))
                                        script = string.Format("function {0} {{{1}}}", currentEvent.EventName, script);
                                    else
                                        script = string.Format("function {0}() {{{1}}}", currentEvent.EventName, script);

                                Page.ClientScript.RegisterStartupScript(typeof(string), this.ID + "_" + currentEvent.EventName.Trim(), script, true);
                                break;
                        }
                    }
                }
            }
            _coeLog.LogEnd(methodSignature);
        }



        public string ReplaceControlNames(string script)
        {
            return ReplaceReservedWords(ReplaceReservedWords(script, "@", null), "<%=", "%>");
        }

        public string ReplaceContextVariables(string script)
        {
            return ReplaceReservedWords(script, "<%=", "%>");
        }

        public string ReplaceReservedWords(string script, string openToken, string closingToken)
        {
            string scriptCode = script;

            int startIndex = 0;
            while (startIndex < scriptCode.Length && scriptCode.IndexOf(openToken, startIndex) > 0)
            {
                startIndex = scriptCode.IndexOf(openToken) + openToken.Length;

                int endIndex = closingToken == null ? scriptCode.IndexOfAny(separator, startIndex + openToken.Length) : scriptCode.IndexOf(closingToken, startIndex + openToken.Length);

                if (endIndex < 0)
                    endIndex = scriptCode.Length - openToken.Length;

                int length = endIndex - startIndex;

                if (length > 0)
                {
                    string reservedWord = scriptCode.Substring(startIndex, endIndex - startIndex);
                    string newValue = string.Empty;

                    if (reservedWord.Contains("Context"))
                    {
                        COEDataBinder dataBinder = new COEDataBinder(HttpContext.Current);

                        if (dataBinder.RetrieveProperty(reservedWord.Replace("Context", "")) != null)
                            newValue = dataBinder.RetrieveProperty(reservedWord.Replace("Context", "")).ToString();
                        else
                            newValue = string.Format("COEFormGenerator Client script error: {0} NOT FOUND!", reservedWord);
                    }
                    else
                    {
                        Control referencedControl = this.FindControl(reservedWord);
                        if (referencedControl != null)
                            newValue = referencedControl.ClientID;
                        else
                            newValue = string.Format("COEFormGenerator Client script error: {0} NOT FOUND!", reservedWord);
                    }

                    scriptCode = scriptCode.Substring(0, startIndex - openToken.Length) + newValue + scriptCode.Substring(endIndex + (closingToken == null ? 0 : closingToken.Length));
                }
            }

            return scriptCode;
        }

        public override Control FindControl(string name)
        {
            Control returnControl = base.FindControl(name);
            if (returnControl == null)
            {
                foreach (Control currentControl in this._userControls)
                    if (currentControl != null && currentControl.ID == this.NormalizedFormElementName(name))
                        return currentControl;
            }
            return returnControl;

        }

        private void CreateContainer(FormGroup.DisplayInfo displayInfo)
        {
            _container = new Panel();
            this.Controls.Add(_container);

            _container.Height = this.Height;
            _container.Width = this.Width;

            if (!string.IsNullOrEmpty(displayInfo.Top))
                _container.Style.Add("Top", new Unit(displayInfo.Top).ToString());

            if (!string.IsNullOrEmpty(displayInfo.Left))
                _container.Style.Add("Left", new Unit(displayInfo.Left).ToString());

            _container.ID = this.ID + "Container";

             AddStyles(_container.Style, displayInfo.Style);

            _container.CssClass = displayInfo.CSSClass;

            switch (displayInfo.LayoutStyle)
            {
                case FormGroup.LayoutStyle.AbsoluteLayout:
                    _container.Style.Add("position", "relative");
                    break;
                case FormGroup.LayoutStyle.FlowLayout:
                    _container.Style.Add("position", "static");
                    break;
            }
            if (!string.IsNullOrEmpty(_formData.Title))
            {
                Panel titleContainer = new Panel();
                _container.Controls.Add(titleContainer);
                titleContainer.ID = "FormGeneratorTitle_" + this.ID;
                if (!string.IsNullOrEmpty(_formData.TitleCssClass))
                    titleContainer.CssClass = _formData.TitleCssClass;
                else
                {
                    titleContainer.Style.Add("height", "19px");
                    titleContainer.Style.Add("border-bottom", "1px solid #000099");
                    titleContainer.Style.Add("top", "0px");
                    titleContainer.Style.Add("width", "730px");
                    titleContainer.Style.Add("font-size", "11px");
                    titleContainer.Style.Add("font-family", "Verdana");
                    titleContainer.Style.Add("color", "#000066");
                    titleContainer.Style.Add("font-weight", "bold");
                }

                Label title = new Label();
                titleContainer.Controls.Add(title);
                title.Text = _formData.Title;
                title.Style.Add(HtmlTextWriterStyle.Position, "relative");
                title.Style.Add(HtmlTextWriterStyle.Top, "-2px");
                _container.Style.Add(HtmlTextWriterStyle.MarginTop, !string.IsNullOrEmpty(_container.Style[HtmlTextWriterStyle.MarginTop]) ? (new Unit(_container.Style[HtmlTextWriterStyle.MarginTop]).Value + 10).ToString() + "px" : "10px");
            }

            if (string.IsNullOrEmpty(_container.Style["overflow"]))
                _container.Style.Add("overflow", "auto");
        }

        public static void AddStyles(CssStyleCollection styleContainer, string styleToAdd)
        {
            if (!string.IsNullOrEmpty(styleToAdd))
            {
                string[] styles = styleToAdd.Split(new char[1] { ';' });
                for (int i = 0; i < styles.Length; i++)
                {
                    if (styles[i].Length > 0)
                    {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        string styleId = styleDef[0].Trim();
                        string styleValue = styleDef[1].Trim();
                        styleContainer.Add(styleId, styleValue);
                    }
                }
            }
        }

        private void RegisterToControlServerEvents(ICOEGenerableControl controlToAdd, List<FormGroup.COEEventInfo> list)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            foreach (FormGroup.COEEventInfo currentEvent in list)
            {
                EventInfo eventDelegate = controlToAdd.GetType().GetEvent(currentEvent.EventName);
                if (eventDelegate != null)
                {
                    Type eventHandlerType = eventDelegate.EventHandlerType;

                    MethodInfo handlerMethod = this.Page.GetType().GetMethod(currentEvent.EventHandlerName);
                    if (handlerMethod != null)
                    {

                        Delegate d = Delegate.CreateDelegate(eventHandlerType, this.Page, handlerMethod);
                        eventDelegate.AddEventHandler(controlToAdd, d);
                    }
                    else
                        throw new Exception(string.Format("Couldn't find Event Handler '{0}' - It HAS to be public", currentEvent.EventHandlerName));
                }
                else
                    throw new Exception(string.Format("Invalid Event '{0}' for object of type {1}", currentEvent.EventName, controlToAdd.GetType().ToString()));
            }

            PropertyInfo autoPostBack = controlToAdd.GetType().GetProperty("AutoPostBack");

            if (autoPostBack != null)
                autoPostBack.SetValue(controlToAdd, true, null);
            _coeLog.LogEnd(methodSignature);
        }
        #endregion

        #region CRUD methods

        DataSourceSelectArguments selectArguments;
        protected override DataSourceSelectArguments CreateDataSourceSelectArguments()
        {
            selectArguments = new DataSourceSelectArguments();
            DataSourceView dataSourceView = this.GetData();

            if (dataSourceView.CanPage)
            {
                selectArguments.AddSupportedCapabilities(DataSourceCapabilities.Page);

                selectArguments.StartRowIndex = _pageIndex;
                selectArguments.MaximumRows = 1;
            }

            if (dataSourceView.CanRetrieveTotalRowCount)
            {
                selectArguments.AddSupportedCapabilities(DataSourceCapabilities.RetrieveTotalRowCount);
                selectArguments.RetrieveTotalRowCount = true;
            }

            return selectArguments;
        }

        /// <summary>
        /// <para>This methods will trigger the update process through the binding datasource. If datasourceid is being used, then
        /// its dataview.Update methods is called, otherwise ItemUpdating and ItemUpdated events are fired.</para>
        /// </summary>
        public void Update()
        {
            EnsureChildControls();

            DataSourceView dataView = GetData();
            Hashtable values = new Hashtable();
            Hashtable keys = new Hashtable();
            keys.Add("PageIndex", this.PageIndex);
            keys.Add("DataMember", this.FormDescription.DataMember);

            foreach (ICOEGenerableControl control in _userControls)
            {
                if (control != null)
                {
                    if (!string.IsNullOrEmpty(((WebControl)control).Attributes["BindingExpression"]))
                    {
                        values.Add(((WebControl)control).Attributes["BindingExpression"] + Constants.ControlIdsSplitter + ((WebControl)control).ID + Constants.ControlIdsSplitter, control.GetData());
                    }
                }
            }
            if (dataView.CanUpdate)
            {
                dataView.Update(keys, values, _oldValues, new DataSourceViewOperationCallback(UpdateCallBack));
            }
            else
            {
                COEFGEventArgs dargs = new COEFGEventArgs(0, null, (IDictionary)keys, (IDictionary)_oldValues, (IDictionary)values);
                COEFGEventHandler handler = Events[_itemUpdatingEvent] as COEFGEventHandler;
                if (handler != null)
                {
                    handler(this, dargs);
                }
            }

            RequiresDataBinding = true;
        }

        /// <summary>
        /// <para>Helper method that returns the value of a control inside the form using the bindingExpression to locate the control.</para>
        /// </summary>
        /// <param name="bindingExpression">The binding expression of the control.</param>
        /// <returns>The control's value.</returns>
        public object GetControlValue(string bindingExpression)
        {
            foreach (ICOEGenerableControl control in _userControls)
            {
                if (control != null)
                {
                    if (!string.IsNullOrEmpty(((WebControl)control).Attributes["BindingExpression"]) && ((WebControl)control).Attributes["BindingExpression"] == bindingExpression)
                    {
                        return control.GetData();
                    }
                }
            }

            return null;
        }

        bool UpdateCallBack(int recordsAffected, Exception exception)
        {
            COEFGEventArgs dargs = new COEFGEventArgs(recordsAffected, exception, null, null, null);
            return dargs.ExceptionHandled;
        }

        #endregion

        #region Contructors
        /// <summary>
        /// <para>Initializes a new instance of the <see cref="COEFormGenerator"/> class in Add mode.</para>
        /// </summary>
        /// 
        public COEFormGenerator()
        {
            _oldValues = new Dictionary<string, object>();
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="COEFormGenerator"/> class with the specified FormDescription.</para>
        /// </summary>
        /// <param name="formData">The desired form description.</param>
        public COEFormGenerator(CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.Form formData)
            : this()
        {
            _formData = formData;

            /*if(_formData.DisplayMode != null)
                this._displayMode = formData.DisplayMode;
            */
            this.ID = formData.Id.ToString();
        }
        #endregion

        #region Methods
        /// <summary>
        /// <para>Returns the name of the class followed by a slash and by its ID.</para>
        /// </summary>
        /// <returns>A string of the form "COEFormGenerator - ID".</returns>
        public override string ToString()
        {
            return this.GetType().Name + " - " + this.ID;
        }
        #endregion

    }
    #region Additional Classes
    internal class Constants
    {
        public const string ErrorMsg_InitialDisplay = "none";
        public const string ErrorMsg_Position = "absolute";
        public const string ErrorMsg_BackGroundColor = "lightblue";
        public const string ErrorMsg_BorderColor = "blue";
        public const string ErrorMsg_BorderStyle = "solid";
        public const string ErrorMsg_BorderWidth = "2px";
        public const string ErrorMsg_TabIndex = "-1";
        public const string ErrorMsg_ZIndex = "100";
        public const string ErrorMsg_TextColor = "red";
        public const float ErrorMsg_Opacity = 0.5f;
        public const string ErrorMsg_PrefixId = "errorMessageFor";
        public const string ErrorMsg_ValidatorsAttribute = "validators";

        public const string Browser_IE = "ie";
        public const string Browser_Explorer = "explorer";

        public const string Validator_Min = "min";
        public const string Validator_Max = "max";
        public const string Validator_BackgroundImage = "url(error.png)";
        public const string Validator_BackgroundRepeat = "url(no-repeat)";
        public const string Validator_Cursor = "pointer";
        public const string Validator_Display = "inline-block";
        public const string Validator_Position = "absolute";
        public const string Validator_Top = "2px";
        public const string Validator_Height = "16px";
        public const string Validator_Width = "16px";
        public const string Validatior_ValidWord = "validword";
        public const int Validator_Top_Margin = 2;
        public const int Validator_Left_Margin = 10;
        public const string Validator_IntegerPart = "integerpart";
        public const string Validator_DecimalPart = "decimalpart";

        public const string JSFunction_ShowErrorDivName = "ShowErrorDiv";
        public const string JSFunction_HideErrorDivName = "HideErrorDiv";
        public const string JSFunction_ShowErrorDiv =
                    @"function showErrorDiv(divId) {
                        validatorList = WebForm_GetElementById(divId).getAttribute('" + ErrorMsg_ValidatorsAttribute + @"').split(',');
                        WebForm_GetElementById(divId).innerHTML = '';
                        for(i = 0; i < validatorList.length; i++){
                            ValidatorValidate(WebForm_GetElementById(validatorList[i]), '', window.event)
                            if(!WebForm_GetElementById(validatorList[i]).isvalid) {
                                if(WebForm_GetElementById(divId).innerHTML != '')
                                    WebForm_GetElementById(divId).innerHTML += '<br/>';

                                WebForm_GetElementById(divId).innerHTML += WebForm_GetElementById(validatorList[i]).getAttribute('text');
                            }
                        }
                        if(WebForm_GetElementById(divId).innerHTML.length > 0) {
                            WebForm_GetElementById(divId).style.display = 'inline';
                            WebForm_GetElementById(divId).focus();
                        }
                    }";
        public const string JSFunction_HideErrorDiv =
                    @"function hideErrorDiv(divId) {
                        WebForm_GetElementById(divId).style.display = 'none';
                    }";
        public static string DatesFormat = Resources.DefaultDateFormat;
        public const string ControlIdsSplitter = "&";

        public const string LabelStyleControlSelector = "control-";
        public const string LabelStyleLabelSelector = "label-";

        #region ViewState Constants
        public const string Label_VS = "Label";
        public const string CSSClass_VS = "CSSClass";
        public const string LabelStyles_VS = "LabelStyles";
        public const string RequiredStyle_VS = "RequiredStyle";
        public const string RequiredLabelStyle_VS = "RequiredLabelStyle";
        #endregion

    }
    /// <summary>
    /// <para>General event arguments class for describing old values and new values of the control's values.</para>
    /// </summary>
    public class COEFGEventArgs : EventArgs
    {
        private int _recordsAffected;
        private Exception _exception;
        private IDictionary _keys;
        private IDictionary _oldValues;
        private IDictionary _newValues;
        /// <summary>
        /// <para>Tells if an exception was handled.</para>
        /// </summary>
        public bool ExceptionHandled
        {
            get { return (_exception == null); }
        }

        public IDictionary Keys
        {
            get { return _keys; }
        }

        /// <summary>
        /// <para>Dictionary containing the values previous to the last databinding operation.</para>
        /// </summary>
        public IDictionary OldValues
        {
            get { return _oldValues; }
        }

        /// <summary>
        /// <para>Dictionary containing the values of the current databinding operation.</para>
        /// </summary>
        public IDictionary NewValues
        {
            get { return _newValues; }
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="COEFGEventArgs"/>.</para>
        /// </summary>
        public COEFGEventArgs(int recordsAffected, Exception exception, IDictionary keys, IDictionary oldvalues, IDictionary newValues)
            : this(keys, oldvalues, newValues)
        {
            this._recordsAffected = recordsAffected;
            this._exception = exception;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="COEFGEventArgs"/>.</para>
        /// </summary>
        public COEFGEventArgs(IDictionary keys, IDictionary oldValues, IDictionary newValues)
        {
            this._keys = keys;
            this._oldValues = oldValues;
            this._newValues = newValues;
        }
    }

    /// <summary>
    /// <para>Form Generator Event Handler that contains a <see cref="COEFGEventArgs"/> as parameter, which has the old and the new values of the 
    /// binding process</para>
    /// </summary>
    /// <param name="sender">The sender control.</param>
    /// <param name="e">The Event Arguments, that contains control's old and new values.</param>
    public delegate void COEFGEventHandler(Control sender, COEFGEventArgs e);
    #endregion
}
