using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Common.Messaging;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Drawing;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.GUIShell;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// 
    /// </summary>
    public class COEFormGroup : CompositeControl
    {
        #region Variables
        private COEFormCollection _queryFormCollection;
        private COEFormCollection _detailFormCollection;
        private COEFormCollection _listFormCollection;
        private FormGroup.CurrentFormEnum _currentDisplayGroup = FormGroup.CurrentFormEnum.QueryForm;
        private FormGroup _coeFormGroupDescription;
        private List<COEFormGenerator> _coeFormGenerators;
        Panel _formGroupValidationSummaryPanel;
        ValidationSummary _formGroupvalidationSummary;
        private string _displayCulture;
        private bool _enableValidationSummary;

        private static readonly object _itemUpdatedEvent = new object();
        private static readonly object _itemUpdatingEvent = new object();

        private string _dataSourceId;
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
        public event MarkAllHitsHandler MarkAllHits;
        public event CommandEventHandler OrderCommand;
        #endregion

        #region Properties
        public string ValidationSummaryCssClass
        {
            get { return _formGroupvalidationSummary.CssClass; }
            set { _formGroupvalidationSummary.CssClass = value; }
            }


        public bool EnableValidationSummary
        {
            get { return _enableValidationSummary; }
            set { _enableValidationSummary = value; }
            }

        public string DataSourceId
        {
            get
            {
                return _dataSourceId;
            }
            set
            {
                _dataSourceId = value;
            }
        }
        public CambridgeSoft.COE.Framework.Common.Messaging.FormGroup FormGroupDescription
        {
            get
            {
                return _coeFormGroupDescription;
            }
            set
            {
                _coeFormGroupDescription = value;

                this.QueryFormCollection = new COEFormCollection(_coeFormGroupDescription.QueryForms);
                this.DetailFormCollection = new COEFormCollection(_coeFormGroupDescription.DetailsForms);
                this.ListFormCollection = new COEFormCollection(_coeFormGroupDescription.ListForms);

                this.EnableValidationSummary = _coeFormGroupDescription.EnableValidationSummary;
                this.ValidationSummaryCssClass = _coeFormGroupDescription.ValidationSummaryCssClass;
            }
        }
        public COEFormCollection QueryFormCollection
        {
            get
            {
                if(_queryFormCollection == null)
                    _queryFormCollection = new COEFormCollection(null);

                return _queryFormCollection;
            }

            set { _queryFormCollection = value; }
            }

        public COEFormCollection DetailFormCollection
        {
            get
            {
                if(_detailFormCollection == null)
                    _detailFormCollection = new COEFormCollection(null);

                return _detailFormCollection;
            }

            set { _detailFormCollection = value; }
            }

        public COEFormCollection ListFormCollection
        {
            get
            {
                if(_listFormCollection == null)
                    _listFormCollection = new COEFormCollection(null);

                return _listFormCollection;
            }
            set { _listFormCollection = value; }
        }

        public FormGroup.CurrentFormEnum CurrentDisplayGroup
        {
            get { return _currentDisplayGroup; }
            set
            {
                if(_currentDisplayGroup != value)
                {
                    _currentDisplayGroup = value;
                    ChildControlsCreated = false;
                }
            }
        }

        public FormGroup.DisplayMode CurrentDisplayMode
        {
            get
            {
                EnsureChildControls();
                return CurrentCollection.CurrentDisplayMode;
            }
            set
            {
                if(CurrentCollection.CurrentDisplayMode != value)
                {
                    CurrentCollection.CurrentDisplayMode = value;
                    foreach(COEFormGenerator currentGenerator in this.COEFormGenerators)
                    {
                        currentGenerator.DisplayMode = value;
                    }
                    ChildControlsCreated = false;
                }
            }
        }

        private List<COEFormGenerator> COEFormGenerators
        {
            get
            {
                //EnsureChildControls();
                if(_coeFormGenerators == null)
                    _coeFormGenerators = new List<COEFormGenerator>();

                return _coeFormGenerators;
            }
        }

        public COEFormCollection CurrentCollection
        {
            get
            {
                switch(_currentDisplayGroup)
                {
                    case FormGroup.CurrentFormEnum.QueryForm:
                        return this.QueryFormCollection;
                    case FormGroup.CurrentFormEnum.DetailForm:
                        return this.DetailFormCollection;
                    default:
                        return this.ListFormCollection;
                }
            }
        }

        public int CurrentFormIndex
        {
            get
            {
                return CurrentCollection.CurrentFormIndex;
            }
            set
            {
                if(value != CurrentCollection.CurrentFormIndex)
                {
                    CurrentCollection.CurrentFormIndex = value;
                    ChildControlsCreated = false;
                }
            }
        }

        public string DisplayCulture
        {
            set
            {
                _displayCulture = value;
            }
        }
        #endregion


        #region Overriden Properties
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }
        #endregion

        #region Methods
        public COEFormGroup()
        {
            _formGroupvalidationSummary = new ValidationSummary();
            _formGroupvalidationSummary.ID = "ValidationSummary";
            _formGroupvalidationSummary.DisplayMode = ValidationSummaryDisplayMode.BulletList;

            _formGroupValidationSummaryPanel = new Panel();
            _formGroupValidationSummaryPanel.ID = "formGroupValidationSummaryPanel";

            //_formGroupvalidationSummary.CssClass = "id1COEErrorDiv";
        }

        public void Update()
        {
            EnsureChildControls();

            foreach(COEFormGenerator currentControl in this.COEFormGenerators)
            {
                if(currentControl.Visible && currentControl.Enabled)
                    ((COEFormGenerator)currentControl).Update();
            }
        }

        public int SetFormGeneratorPageIndex(int formGeneratorIndex, int pageIndex)
        {

            COEFormGenerator formGenerator = (COEFormGenerator)this.FindControl(this.GetFormName(formGeneratorIndex));
            if(formGenerator != null)
            {
                formGenerator.PageIndex = pageIndex;
                return formGenerator.PageIndex;
            }
            return -1;
        }
        public void SetFormGeneratorsPageIndex(int pageIndex)
        {
            EnsureChildControls();
            foreach(COEFormGenerator currentFormGenerator in this.COEFormGenerators)
            {
                currentFormGenerator.PageIndex = pageIndex;
            }
        }

        public int GetFormGeneratorPageIndex(int formGeneratorIndex)
        {
            COEFormGenerator formGenerator = (COEFormGenerator)this.FindControl(this.GetFormName(formGeneratorIndex));
            if(formGenerator != null)
            {
                return formGenerator.PageIndex;
            }

            throw new Exception("FormGenerator not found");
        }
        public void SetFormGeneratorVisibility(int formGeneratorIndex, bool visibility)
        {
            Control formGenerator = this.FindControl(this.GetFormName(formGeneratorIndex));

            if(formGenerator != null)
                formGenerator.Visible = visibility;
        }

        public void SetFormGeneratorEnable(int formGeneratorIndex, bool enable)
        {
             COEFormGenerator form = GetSubForm(formGeneratorIndex);
             if (form != null)
                 form.Enabled = enable;
        }

        public void SetFormGeneratorViewState(int formGeneratorIndex, bool enable)
        {
            COEFormGenerator form = GetSubForm(formGeneratorIndex);
            if (form != null)
                form.EnableViewState = enable;
        }

        public COEFormGenerator GetSubForm(int formGeneratorIndex)
        {
            return (COEFormGenerator)this.FindControl(this.GetFormName(formGeneratorIndex));
        }

        public Control FindControl(int id)
        {
            string controlName = GetFormName(id);
            return base.FindControl(controlName);
        }

        #endregion

        #region LifeCycle Events
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.RegisterRequiresControlState(this);
        }

        protected override void LoadControlState(object savedState)
        {
            object[] savedStateArray = (object[])savedState;

            int currentIndex = 0;

            base.LoadControlState(savedStateArray[currentIndex++]);
            _currentDisplayGroup = (FormGroup.CurrentFormEnum)(savedStateArray[currentIndex++]);

            this.QueryFormCollection.LoadControlState(savedStateArray[currentIndex++]);
            this.DetailFormCollection.LoadControlState(savedStateArray[currentIndex++]);
            this.ListFormCollection.LoadControlState(savedStateArray[currentIndex++]);
            DataSourceId = (string)savedStateArray[currentIndex++];

        }

        protected override object SaveControlState()
        {
            object[] savedStateArray = new object[] { 
                base.SaveControlState(), 
                
                this.CurrentDisplayGroup, 

                this.QueryFormCollection.SaveControlState(),
                this.DetailFormCollection.SaveControlState(),
                this.ListFormCollection.SaveControlState(),
                DataSourceId
                };

            return savedStateArray;
        }

        private string GetFormName(int id)
        {
            return string.Format("{0}{1}", Enum.GetName(typeof(FormGroup.CurrentFormEnum), CurrentDisplayGroup), id.ToString());
        }

        protected override void CreateChildControls()
        {
            this.Controls.Clear();

            COEFormGenerators.Clear();


            FormGroup.Display currentDisplay = this.CurrentCollection.GetCurrent();

            if(currentDisplay != null)
            {
                foreach(FormGroup.Form currentForm in currentDisplay.Forms)
                {
                    COEFormGenerator currentGenerator = new COEFormGenerator();
                    currentGenerator.ID = GetFormName(currentForm.Id);


                    COEFormGenerators.Add(currentGenerator);
                    this.Controls.Add(currentGenerator);
                    currentGenerator.EnableViewState = true;
                    currentGenerator.FormDescription = currentForm;
                    currentGenerator.DisplayCulture = _displayCulture;
                    currentGenerator.DisplayMode = CurrentDisplayMode;
                    currentGenerator.CurrentFormMode = this.CurrentDisplayGroup;
                    currentGenerator.ItemUpdating += new COEFGEventHandler(currentGenerator_ItemUpdating);
                    currentGenerator.MarkingHit += new MarkingHitHandler(currentGenerator_MarkingHit);

                    if(!string.IsNullOrEmpty(this.DataSourceId))
                        currentGenerator.DataSourceID = this.DataSourceId;
                    else if(!string.IsNullOrEmpty(currentForm.DataSourceId))
                        currentGenerator.DataSourceID = currentForm.DataSourceId;

                    if(!string.IsNullOrEmpty(currentForm.DataMember))
                        currentGenerator.DataMember = currentForm.DataMember;
                }
                if (FrameworkUtils.GetAppConfigSetting(GUIShellUtilities.GetApplicationName(),"MISC","ENABLE_COEFORM_DEBUG_INFO").ToLower() == bool.TrueString.ToLower())
                {
                    HtmlGenericControl formGroupDebuggingInfo = new HtmlGenericControl("div");
                    formGroupDebuggingInfo.ID = "FormGroupDebuggingInfo";
                    this.Controls.Add(formGroupDebuggingInfo);
                    Label adminInformation = new Label();
                    adminInformation.Style.Add(HtmlTextWriterStyle.Position, "relative");
                    adminInformation.Style.Add("float", "right");
                    adminInformation.Style.Add(HtmlTextWriterStyle.FontFamily, "verdana");
                    adminInformation.Style.Add(HtmlTextWriterStyle.FontSize, "8px");
                    adminInformation.Style.Add(HtmlTextWriterStyle.Color, "gray");
                    adminInformation.Text = string.Format(Resources.FormGroupID + ": {0}, " + Resources.Mode + ": {1}, " + Resources.Index + ": {2}, " + Resources.SubMode + ": {3}",
                                                            this.FormGroupDescription.Id,
                                                            this.CurrentDisplayGroup.ToString(),
                                                            this.CurrentFormIndex,
                                                            this.CurrentDisplayMode);
                    formGroupDebuggingInfo.Controls.Add(adminInformation);
                }
            }
        }

        private void InsertValidationSummary(ControlCollection controlCollection)
        {/*
            <div id="panel1">
            <div class="hd">
                Panel #1 from Markup &mdash; This Panel is Draggable</div>
            <div class="bd">
                <asp:ValidationSummary ID="lalala" runat="server" HeaderText="Validation Summary"
                    ValidationGroup="*" />
            </div>
            <div class="ft">
                End of Panel #1</div>
        </div>*/
            if(_enableValidationSummary)
            {
                _formGroupValidationSummaryPanel.Controls.Clear();
                controlCollection.Add(_formGroupValidationSummaryPanel);

                Panel header = new Panel();
                _formGroupValidationSummaryPanel.Style.Add("visibility", "Hidden");
                _formGroupValidationSummaryPanel.Controls.Add(header);

                Literal literal = new Literal();
                literal.Text = Resources.ValidationSummary_Header;
                header.Controls.Add(literal);
                header.CssClass = "hd";
                header.Style.Add(HtmlTextWriterStyle.FontSize, "100%");

                Panel body = new Panel();
                _formGroupValidationSummaryPanel.Controls.Add(body);
                body.CssClass = "bd";

                Panel footer = new Panel();
                _formGroupValidationSummaryPanel.Controls.Add(footer);

                literal = new Literal();
                literal.Text = Resources.ValidationSummary_Footer;
                footer.Controls.Add(literal);
                footer.CssClass = "ft";
                footer.Style.Add(HtmlTextWriterStyle.FontSize, "100%");

                body.Controls.Add(_formGroupvalidationSummary);

                if(!Page.ClientScript.IsStartupScriptRegistered("OverwriteValidations"))
                {
                    string overrideValidatorJS = @"
        YAHOO.namespace('FormGroup.ValidationSummary');

		function initFormGroupValidationSummary() {
			// Instantiate a Panel from markup
			YAHOO.FormGroup.ValidationSummary.panel1 = new YAHOO.widget.Panel('" + _formGroupValidationSummaryPanel.ClientID + @"', { width:'500px', visible:false, constraintoviewport:true, modal:true, fixedcenter:true } );
			YAHOO.FormGroup.ValidationSummary.panel1.render();
            YAHOO.FormGroup.ValidationSummary.panel1.beforeShowEvent.subscribe(HideChemDraws);
            YAHOO.FormGroup.ValidationSummary.panel1.beforeHideEvent.subscribe(HideValidationSummary);

            //Show validation summary dialog box if client-side validation is not enabled.
            if( document.getElementById('" + this._formGroupvalidationSummary.ClientID + @"') != null &&
                document.getElementById('" + this._formGroupvalidationSummary.ClientID + @"').style.display != 'none')
                YAHOO.FormGroup.ValidationSummary.panel1.show();

		}
        
        //Mozilla does not hides the validator automatically.
        function HideValidationSummary()
        { 
            ShowChemDraws();
            document.getElementById('" + this._formGroupvalidationSummary.ClientID + @"').style.display = 'none';
        }

        function ValidationSummaryOnSubmitEx(validationGroup)
        { 
            original_ValidationSummaryOnSubmit(validationGroup);

            if(document.getElementById('" + this._formGroupvalidationSummary.ClientID + @"').style.display != 'none')
                YAHOO.FormGroup.ValidationSummary.panel1.show();
        }

        YAHOO.util.Event.addListener(window, 'load', initFormGroupValidationSummary);

        if(typeof(ValidationSummaryOnSubmit) != 'undefined')
        {
            /* Replace ValidationSumaryOnSubmit from asp.net native validation script */
            var original_ValidationSummaryOnSubmit = ValidationSummaryOnSubmit;
            ValidationSummaryOnSubmit = ValidationSummaryOnSubmitEx;
        }
";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "OverwriteValidations", overrideValidatorJS, true);
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            FormGroup.Display currentDisplay = this.CurrentCollection.GetCurrent();
            if(currentDisplay != null)
            {
                InsertValidationSummary(this.Controls);

                this.AddStyleSheet();
                this.AddStyle();
                this.AddScripts();
            }

            base.OnPreRender(e);
        }

        private void AddScripts()
        {
            if(this._coeFormGroupDescription.Script != null)
            {
                if(!string.IsNullOrEmpty(this._coeFormGroupDescription.Script.Src + this._coeFormGroupDescription.Script.Value))
                {
                    HtmlGenericControl link = new HtmlGenericControl("script");
                    link.Attributes.Add("type", "text/javascript");
                    this.Page.Header.Controls.Add(link);

                    if(!string.IsNullOrEmpty(this._coeFormGroupDescription.Script.Src))
                    {
                        link.Attributes.Add("src", Page.ResolveUrl(this._coeFormGroupDescription.Script.Src));
                    }
                    else if(!string.IsNullOrEmpty(this._coeFormGroupDescription.Script.Value))
                    {
                        link.InnerHtml = this._coeFormGroupDescription.Script.Value;
                    }
                }
            }
        }

        private void AddStyleSheet()
        {
            if(!string.IsNullOrEmpty(this._coeFormGroupDescription.StyleSheet))
            {
                HtmlLink link = (HtmlLink)Page.Header.FindControl("styleSheetLink");
                if(link == null)
                {
                    link = new HtmlLink();
                    link.ID = "styleSheetLink";
                    Page.Header.Controls.Add(link);
                }

                link.Attributes.Add("href", this.FormGroupDescription.StyleSheet);
                link.Attributes.Add("rel", "stylesheet");
                link.Attributes.Add("type", "text/css");
            }
        }

        private void AddStyle()
        {
            if(!string.IsNullOrEmpty(this._coeFormGroupDescription.Style))
            {
                Literal literal = (Literal)Page.Header.FindControl("styleLiteral");
                if(literal == null)
                {
                    literal = new Literal();
                    literal.ID = "styleLiteral";
                    Page.Header.Controls.Add(literal);
                }

                literal.Text = "<style type=\"text/css\">" + this._coeFormGroupDescription.Style + "</style>";
            }
        }

        void currentGenerator_ItemUpdating(System.Web.UI.Control sender, COEFGEventArgs e)
        {
            COEFGEventHandler handler = Events[_itemUpdatingEvent] as COEFGEventHandler;
            if(handler != null)
            {
                handler(sender, e);
            }
        }

        void currentGenerator_MarkingHit(object sender, MarkHitEventArgs eventArgs)
        {
            if(MarkingHit != null)
                MarkingHit(sender, eventArgs);
        }

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            bool handled = false;

            if(args is CommandEventArgs)
            {
                CommandEventArgs cmdArgs = args as CommandEventArgs;
                if(cmdArgs.CommandName == "OrderCommand" && this.OrderCommand != null)
                {
                    OrderCommand(source, cmdArgs);
                    handled = true;
                }
                else if(cmdArgs.CommandName == "ApplyMarkAction" && this.MarkingHit != null)
                {
                    MarkingHit(source, cmdArgs.CommandArgument as MarkHitEventArgs);
                    handled = true;
                }
            }
            else if(args is MarkAllHitsEventArgs)
            {
                MarkAllHitsEventArgs markAllEventArgs = args as MarkAllHitsEventArgs;
                if(this.MarkAllHits != null)
                {
                    MarkAllHits(source, markAllEventArgs);
                    handled = true;
                }
            }
            return handled;
        }
        #endregion
    }
}
