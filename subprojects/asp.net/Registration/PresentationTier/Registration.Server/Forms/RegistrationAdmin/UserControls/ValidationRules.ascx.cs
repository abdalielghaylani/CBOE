using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.RegistrationAdmin.Services;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.RegistrationAdmin;
using CambridgeSoft.COE.Registration.Services;
using CambridgeSoft.COE.Framework.Common.Messaging;
using System.Collections.Generic;
using System.Text;
using Csla;
using CambridgeSoft.COE.Registration.Services.BLL;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Xml.Serialization;
using System.Reflection;
using Resources;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.ExceptionHandling;


namespace PerkinElmer.COE.Registration.Server.Forms.RegistrationAdmin.UserControls
{
    public partial class ValidationRules : System.Web.UI.UserControl
    {

        #region Enums
        private enum EventMode
        {
            Add, 
            Save,
            Delete
        }
               
        #endregion

        #region Variables
        RegistrationMaster _masterPage = null;
        #endregion

        #region Constants
        private const string MODALPROGRESS_SAVE = "Saving...";
        private const string MODALPROGRESS_ADD = "Adding...";
        private const string MODALPROGRESS_DELETE = "Deleting...";
        #endregion

        #region Properties

        private Property SelectedProperty
        {
            get
            {
                return (Property)Session["SelectedProperty"];
            }
            set
            {
                Session["SelectedProperty"] = value;
            }

        }

        private ParameterList Parameters
        {
            get
            {
                return (ParameterList)ViewState["Parameters"];
            }
            set
            {
                ViewState["Parameters"] = value;
            }

        }

        public ConfigurationRegistryRecord ConfigurationBO
        {
            get
            {
                try
                {
                    if (Session["ConfigurationRegistryRecord"] == null)
                        Session["ConfigurationRegistryRecord"] = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();

                    return (ConfigurationRegistryRecord)Session["ConfigurationRegistryRecord"];
                }
                catch (Exception exception)
                {
                    COEExceptionDispatcher.HandleUIException(exception);
                    _masterPage.DisplayErrorMessage(exception, false);
                    return null;
                }	
            }
            set
            {
                Session["ConfigurationRegistryRecord"] = value;
            }
        }

        #endregion

        #region Page Load

        protected override void OnInit(EventArgs e)
        {
            if (this.Page.Master is RegistrationMaster)
            {
                _masterPage = (RegistrationMaster)this.Page.Master;
            }
            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (!this.Page.ClientScript.IsStartupScriptRegistered(typeof(ValidationRules), "SetParameterScripts"))
            {
                string script = @"
                    function Disable(obj) {
                        if (!(obj == 'undefined' || obj == null))
                            obj.disabled = true;
                        return obj;
                    }

                    function Enable(obj) {
                        if (!(obj == 'undefined' || obj == null))
                            obj.disabled = false;
                        return obj;
                    }

                    function EnableValidator(obj) {
                        if (!(obj == 'undefined' || obj == null))
                            ValidatorEnable(obj, true);
                        return obj;
                    }

                    function DisableValidator(obj) {
                        if (!(obj == 'undefined' || obj == null))
                            ValidatorEnable(obj, false);
                        return obj;
                    }

                    function ValidateValidators() {
                        if (typeof (Page_ClientValidate) == 'function')
                            Page_ClientValidate();
                        if (typeof (Page_IsValid) == 'undefined' || Page_IsValid) {
                            return true;
                        }
                        else
                           return false;
                    }

                    function ValidateDefalutValue() {
                        btnAddRule = document.getElementById('" + this.BtnAddRule.ClientID + @"');
                        HFDVToolTip = document.getElementById('" + this.HFDVToolTip.ClientID + @"');

                        if (!(ValidateValidators())) {
                            Disable(btnAddRule);
                            btnAddRule.title = HFDVToolTip.value;
                        }
                        else {
                            Enable(btnAddRule);
                            btnAddRule.title = '';
                        }
                    }

                    function SetParameterDiv(typeId)
                    { 
                        
                      try {

                            validationSummary = document.getElementById('" + this.ValidationSummary.ClientID + @"');
                            btnAddRule = document.getElementById('" + this.BtnAddRule.ClientID + @"');
                       
                            validator = document.getElementById('" + this.TextBoxValueValidator.ClientID + @"');

                            validatorDVReq = document.getElementById('" + this.ReqDefaultValue.ClientID + @"');
                            validatorDVReg = document.getElementById('" + this.RegDefaultValue.ClientID + @"');

                            div = document.getElementById('" + this.ParametersDiv.ClientID + @"');
                            divDV = document.getElementById('" + this.divDefaultValue.ClientID + @"');

                            type = document.getElementById(typeId);
                            PropType = document.getElementById('" + this.TxtPropType.ClientID + @"');
                            parameterName = document.getElementById('" + this.LabelName.ClientID + @"');
                            parameterValue = document.getElementById('" + this.TxtValue.ClientID + @"'); 
                            btnAddRule = document.getElementById('" + this.BtnAddRule.ClientID + @"');

                            clientDiv =document.getElementById('" + this.ClientScriptDiv.ClientID + @"');
                            clientDiv.style.display ='none';

                            div.style.display ='none';
                            parameterName.innerHTML = '';
                            parameterValue.value = '';  
                            btnAddRule.disabled = false;
                            btnAddRule.title = ''; 

                            if(div != 'undefined' && div != null)
                            {
                                div.style.display ='none';
                                parameterName.innerHTML ='';
                                ValidatorEnable(validator, false);
                            }
                            if(divDV != 'undefined' && divDV != null)
                            {
                                divDV.style.display ='none';
                                DisableValidator(validatorDVReq);
                                DisableValidator(validatorDVReg);
                            }         
                            validationSummary.innerText = '';
                            switch (type.options[type.selectedIndex].value)
                            {
                                case 'wordListEnumeration':                
                                    div.style.display ='';
                                    parameterName.innerHTML ='validWord';                              
                                break;
                                case 'requiredField':
                                    if(divDV != 'undefined' && divDV != null)
                                    {
                                        divDV.style.display ='';
                                        EnableValidator(validatorDVReq);
                                        EnableValidator(validatorDVReg);
                                        Disable(btnAddRule);
                                    }
                                break;
                                case 'numericRange':
                                case 'textLength':
                                    var paramList = document.getElementById('" + this.Rbl_Event.ClientID + @"');

                                    if(paramList != 'undefined' && paramList != null && paramList.rows[0].innerText.toString().indexOf('min') != -1) 
                                       parameterName.innerHTML = 'max';
                                    else
                                       parameterName.innerHTML = 'min';  

                                    div.style.display ='';                                       
                                    ValidatorEnable(validator, true);                                
                                break;
                                case 'custom':
                                    div.style.display='none';
                                    clientDiv.style.display ='';
                                    parameterName.innerHTML = '';
                                    parameterValue.value='';
                                default:
                                    div.style.display='none';
                                    parameterName.innerHTML = '';
                                    parameterValue.value='';     
                                break;
                            }
                        }
                      catch (ex) {
                        }  
                    };SetParameterDiv('" + DdlType.ClientID + @"');";

                this.Page.ClientScript.RegisterStartupScript(typeof(ValidationRules), "SetParameterScripts", script, true);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (!(Page.ClientScript.IsStartupScriptRegistered("CallConfirmBox")))
                    {
                        string ConfirmBox = @"
                        function CallConfirmBox()
                        {
                           if(confirm('" + Resource.ConfirmDeleteValidationRule_Alert_Text + @"'))
                                return true;
                           else
                            {
                                YAHOO.ChemOfficeEnterprise.ProgressModal.MasterProgressModal.hide();
                                return false;
                            }
                        }";

                        Page.ClientScript.RegisterStartupScript(this.GetType(), "CallConfirmBox", ConfirmBox, true);
                    }
                    SetControlsAttributes();
                    FillValidationRulesList();
                    FillValidationRuleTypeDdl();
                    this.SetSelectedPropertyName = this.SelectedProperty.FriendlyName;
                    CheckDBToProceedForValidations(this.ConfigurationBO.SelectedPropertyList, (Property)SelectedProperty);
                }
                 SetProgressModal();
                _masterPage.SetDefaultAction(this.BtnSave.UniqueID);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        #endregion

        #region Event Handlers

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.SaveConfigurationBO();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        protected void BtnAddParameter_Click(object sender, EventArgs e)
        {
            try
            {
                CreateParameter();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        protected void BtnCancelParameter_Click(object sender, EventArgs e)
        {
            try
            {
                CancelParameter();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        protected void BtnAddRule_Click(object sender, EventArgs e)
        {
            try
            {
                AddValidationRule();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        protected void BtnDeleteRule_Click(object sender, EventArgs e)
        {
            try
            {                
                DeleteValidationRule();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        protected void BtnDeleteParameter_Click(object sender, EventArgs e)
        {
            try
            {
                DeleteParameter();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        protected void Btn_Modify_Parameters_Click(object sender, EventArgs e)
        {
            try
            {
                this.EditParameters();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void LinkButtonBack_Click(object sender, EventArgs e)
        {
            try
            {
                if (ConfigurationBO.SelectedPropertyList == ConfigurationRegistryRecord.PropertyListType.PropertyList)
                    Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/RootPropertyList.aspx");
                else
                    Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/" + ConfigurationBO.SelectedPropertyList.ToString() + ".aspx");
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }

        #endregion
        
        #region Business Property

        private string SetSelectedPropertyName
        {
            set
            {
                this.ConfigurationBO.SelectedPropertyName = value;
            }
        }

        private bool ProceedToAddValidation
        {
            get
            {
                return this.ConfigurationBO.IsPropValidToAddValidator;
            }
        }

        #endregion
        
        #region Methods

        private void SetProgressModal()
        {

            _masterPage.MakeCtrlShowProgressModal(this.BtnSave.ClientID, MODALPROGRESS_SAVE, string.Empty, false);
            _masterPage.MakeCtrlShowProgressModal(this.BtnAddRule.ClientID, MODALPROGRESS_ADD, string.Empty, false);
            _masterPage.MakeCtrlShowProgressModal(this.BtnDeleteRule.ClientID, MODALPROGRESS_DELETE, string.Empty, false);
        }

        private void SetControlsAttributes()
        {
            Page.Title = Resource.Brand + " - " + Resource.RegAdminHome_Page_Title + " - " + Resource.ValidationRules_Page_Title;
            this.BtnSave.Text = Resource.Save_Button_Text;
            LblError.Text = Resource.Error_Label_Text;
            LblName.Text = Resource.Name_Label_Text;
            LblType.Text = Resource.Type_Label_Text;
            LblValue.Text = Resource.Value_Label_Text;
            LlbPropName.Text = Resource.PropertyName_Label_Text;
            LblPropType.Text = Resource.PropertyType_Label_Text;
            LblClientScript.Text = Resource.Client_Script_Label;
                                    
            BtnAddParameter.Text = Resource.Add_Button_Text;
            BtnAddRule.Text = Resource.Add_Rule_Button_Text;
            BtnDeleteParameter.Text = Resource.Delete_Button_Text;
            BtnDeleteRule.Text = Resource.Delete_Rule_Button_Text;
            BtnCancelParameter.Text = Resource.Cancel_Button_Text;

            this.TxtPropName.Text = SelectedProperty.FriendlyName;
            this.TxtPropType.Text = SelectedProperty.Type;

            this.ParametersDiv.Style.Add("display", "none");
            this.ClientScriptDiv.Style.Add("display", "none");

            this.BtnDeleteRule.Attributes.Add("onclick", "javascript:return CallConfirmBox();");
            this.BtnCancelParameter.Attributes.Add("onclick", "return confirm('" + Resource.ConfirmCancelValidationRule_Alert_Text + "');");
            this.BtnDeleteParameter.Attributes.Add("onclick", "return confirm('"+ Resource.ConfirmDeleteParameter_Alert_Text +"');");
            this.TextBoxValueValidator.ErrorMessage = Resource.PositiveInteger_ErrorMessage;
            this.TextBoxValueValidator.Text = "*";
            this.TextBoxValueValidator.ValidationExpression = "[0-9]{0,8}";
            divDefaultValue.Visible = false;
            this.TxtDefaultValue.Visible = false;
            this.DdlDefaultValue.Visible = false;
            this.dateDefaultValue.Visible = false;
            this.HFDVToolTip.Value = "";
        }
        
        private void CheckDBToProceedForValidations(ConfigurationRegistryRecord.PropertyListType PropertyType, Property selectedProperty )
        {
            try
            {

                if (!ProceedToAddValidation)
                {
                    divDefaultValue.Visible = true;
                    if (selectedProperty.Type.ToUpper() == ConfigurationRegistryRecord.PropertyTypeEnum.PickListDomain.ToString().ToUpper())
                    {

                        RegDefaultValue.Visible = !(this.DdlDefaultValue.Visible = true);
                        DataSet dsTable = this.ConfigurationBO.GetPickListValues(selectedProperty.PickListDomainId);
                        this.DdlDefaultValue.DataSource = dsTable.Tables[0];
                        this.DdlDefaultValue.DataTextField = "Value";
                        this.DdlDefaultValue.DataValueField = "Key";
                        this.DdlDefaultValue.DataBind();
                        this.DdlDefaultValue.Items.Insert(0, new ListItem("Select " + selectedProperty.PickListDisplayValue, "-1"));
                        this.DdlDefaultValue.Attributes.Add("onchange", "javascript:return ValidateDefalutValue(this)");
                        lblDefault.Text = "Choose a default value";
                    }
                    else if (selectedProperty.Type.ToUpper() == ConfigurationRegistryRecord.PropertyTypeEnum.Text.ToString().ToUpper())
                    {
                        lblDefault.Text = string.Format(Resource.Required_default_admin_textbox_label_text, selectedProperty.Type);
                        this.TxtDefaultValue.Visible = true;
                        // this.TxtDefaultValue.Attributes.Add("onkeyup", "javascript:return ValidateDefalutValue(this)");
                        this.TxtDefaultValue.Attributes.Add("onblur", "javascript:return ValidateDefalutValue(this)");
                        FillRegularExpression(ConfigurationRegistryRecord.PropertyTypeEnum.Text, selectedProperty.Precision);
                    }
                    else if (selectedProperty.Type.ToUpper() == ConfigurationRegistryRecord.PropertyTypeEnum.Number.ToString().ToUpper())
                    {
                        lblDefault.Text = string.Format(Resource.Required_default_admin_textbox_label_text, selectedProperty.Type);
                        this.TxtDefaultValue.Visible = true;
                        this.TxtDefaultValue.Attributes.Add("onkeyup", "javascript:return ValidateDefalutValue(this)");
                        this.TxtDefaultValue.Attributes.Add("onblur", "javascript:return ValidateDefalutValue(this)");
                        this.FillRegularExpression(ConfigurationRegistryRecord.PropertyTypeEnum.Number, selectedProperty.Precision);
                    }
                    else if (selectedProperty.Type.ToUpper() == ConfigurationRegistryRecord.PropertyTypeEnum.Date.ToString().ToUpper())
                    {
                        lblDefault.Text = string.Format(Resource.Required_default_admin_textbox_label_text, selectedProperty.Type);
                        this.dateDefaultValue.Visible = true;
                        string prevMonthImageUrl = "/COECommonResources/infragistics/20111CLR20/Images/ig_cal_grayP0.gif";
                        this.dateDefaultValue.CalendarLayout.PrevMonthImageUrl = prevMonthImageUrl;
                        string nextMonthImageUrl = "/COECommonResources/infragistics/20111CLR20/Images/ig_cal_grayN0.gif";
                        this.dateDefaultValue.CalendarLayout.NextMonthImageUrl = nextMonthImageUrl;
                        this.dateDefaultValue.Editable = false;
                        this.dateDefaultValue.Value = DateTime.Now;
                        this.FillRegularExpression(ConfigurationRegistryRecord.PropertyTypeEnum.Date, selectedProperty.Precision);
                   }

                    this.FillRequiredValidator(selectedProperty.Type);
                    HFDVToolTip.Value = Resource.Required_default_admin_Tooltip;
                }
            }
            catch (Exception ex)
            { }
        }
        
        private void FillRequiredValidator(string propertyType)
        {
            ReqDefaultValue.ErrorMessage = string.Format(Resource.Required_default_admin_datatype , propertyType.ToLower());
            ReqDefaultValue.Text = "*";
            RegDefaultValue.ControlToValidate = ReqDefaultValue.ControlToValidate = (TxtDefaultValue.Visible) ? TxtDefaultValue.ID : DdlDefaultValue.ID;
        }

        private void FillRegularExpression(ConfigurationRegistryRecord.PropertyTypeEnum propEnum, string precision)
        {
            string regExpression = string.Empty;
            int startRange = 0;

            switch (propEnum)
            {
                case ConfigurationRegistryRecord.PropertyTypeEnum.Number :
                   
                    int intPrecison = 0;
                    int decPrecison = 0;

                    string intPartRegExp = string.Empty;
                    string DecPartRegExp = string.Empty;


                    intPrecison = Convert.ToInt32(precision.Split('.')[0]);
                    decPrecison = Convert.ToInt32(precision.Split('.')[1]);

                    intPrecison = (intPrecison > decPrecison) ? intPrecison - decPrecison : decPrecison - intPrecison;


                    intPartRegExp = string.Format("[0-9]{{{0},{1}}}",startRange,intPrecison);

                    DecPartRegExp = (decPrecison == 0) ? "" : string.Format(@"([.]\d{{{0},{1}}})?", startRange, decPrecison);

                    regExpression = string.Format(@"{0}{1}", intPartRegExp, DecPartRegExp);

                    RegDefaultValue.ErrorMessage = string.Format(Resource.Required_default_admin_numeric_RegExpression , intPrecison, decPrecison);
                    RegDefaultValue.Text = "*";
                    RegDefaultValue.ValidationExpression = regExpression;

                    break;
                case ConfigurationRegistryRecord.PropertyTypeEnum.Text :

                    int lengthPrecison = 0;
                    string charsToInculde = "[^$%<>]";

                    lengthPrecison = Convert.ToInt32(precision);
                    regExpression = string.Format("{0}{{{1},{2}}}", charsToInculde, startRange, lengthPrecison);

                    RegDefaultValue.ErrorMessage = string.Format(Resource.Required_default_admin_text_RegExpression, precision);          
                    RegDefaultValue.Text = "*";
                    RegDefaultValue.ValidationExpression = regExpression;

                    break;
                case ConfigurationRegistryRecord.PropertyTypeEnum.Date:
                    break;
                case ConfigurationRegistryRecord.PropertyTypeEnum.PickListDomain:
                    break;
                case ConfigurationRegistryRecord.PropertyTypeEnum.Boolean:
                    break;
            }
        }
        
        private void FillValidationRulesList()
        {
            this.UltraWebTree_Rules.ClearAll();

            string nodeRule;

            for (int i = 0; i < SelectedProperty.ValRuleList.Count; i++)
            {
                nodeRule = "validationRule: " + SelectedProperty.ValRuleList[i].Name;
                this.UltraWebTree_Rules.Nodes.Add(nodeRule);
                if (SelectedProperty.ValRuleList[i].Parameters != null)
                {
                    if (SelectedProperty.ValRuleList[i].Parameters.Count > 0)
                    {
                        foreach (CambridgeSoft.COE.Registration.Services.BLL.Parameter param in SelectedProperty.ValRuleList[i].Parameters)
                            this.UltraWebTree_Rules.Nodes[i].Nodes.Add("parameter: " + param.Name + " = " + param.Value);
                    }
                    else
                    {
                        // CBOE-1251, validation rules info having no parameters should be displayed
                        if (!string.IsNullOrEmpty(SelectedProperty.ValRuleList[i].DefaultValue))
                            this.UltraWebTree_Rules.Nodes[i].Nodes.Add("Default: = " + SelectedProperty.ValRuleList[i].DefaultValue);
                        if (!string.IsNullOrEmpty(SelectedProperty.ValRuleList[i].Error))
                            this.UltraWebTree_Rules.Nodes[i].Nodes.Add("Error: = " + SelectedProperty.ValRuleList[i].Error);
                    }
                }
            }
        }

        private void FillValidationRuleTypeDdl()
        {
            this.DdlType.Items.Clear();

            Array types = Enum.GetValues(typeof(FormGroup.ValidationRuleEnum));
            if (SelectedProperty.Type == CambridgeSoft.COE.RegistrationAdmin.Services.ConfigurationRegistryRecord.PropertyTypeEnum.Number.ToString().ToUpper())
            {
                EnabledControls(true);
                foreach (FormGroup.ValidationRuleEnum type in types)
                    switch (type)
                    {
                        case FormGroup.ValidationRuleEnum.Custom:
                            DdlType.Items.Add("custom");
                            break;
                        case FormGroup.ValidationRuleEnum.Double:
                            DdlType.Items.Add("double");
                            break;
                        case FormGroup.ValidationRuleEnum.Integer:
                            DdlType.Items.Add("integer");
                            break;
                        case FormGroup.ValidationRuleEnum.NumericRange:
                            DdlType.Items.Add("numericRange");
                            break;
                        case FormGroup.ValidationRuleEnum.PositiveInteger:
                            DdlType.Items.Add("positiveInteger");
                            break;
                        case FormGroup.ValidationRuleEnum.RequiredField:
                            DdlType.Items.Add("requiredField");
                            break;
                        case FormGroup.ValidationRuleEnum.TextLength:
                            DdlType.Items.Add("textLength");
                            break;
                    }
            }
            else if (SelectedProperty.Type == CambridgeSoft.COE.RegistrationAdmin.Services.ConfigurationRegistryRecord.PropertyTypeEnum.Text.ToString().ToUpper())
            {
                EnabledControls(true);
                foreach (FormGroup.ValidationRuleEnum type in types)
                    switch (type)
                    {
                        case FormGroup.ValidationRuleEnum.Custom:
                            DdlType.Items.Add("custom");
                            break;
                        //case FormGroup.ValidationRuleEnum.OnlyChemicalContentAllowed:
                        //    DdlType.Items.Add("onlyChemicalContentAllowed");
                        //    break;
                        case FormGroup.ValidationRuleEnum.RequiredField:
                            DdlType.Items.Add("requiredField");
                            break;
                        case FormGroup.ValidationRuleEnum.TextLength:
                            DdlType.Items.Add("textLength");
                            break;
                        case FormGroup.ValidationRuleEnum.WordListEnumeration:
                            DdlType.Items.Add("wordListEnumeration");
                            break;
                        case FormGroup.ValidationRuleEnum.NotEmptyStructure:
                            DdlType.Items.Add("notEmptyStructure");
                            break;
                        case FormGroup.ValidationRuleEnum.NotEmptyStructureAndNoText:
                            DdlType.Items.Add("notEmptyStructureAndNoText");
                            break;
                    }
            }
            else if (SelectedProperty.Type == CambridgeSoft.COE.RegistrationAdmin.Services.ConfigurationRegistryRecord.PropertyTypeEnum.Boolean.ToString().ToUpper())
            {
                EnabledControls(false);
            }
            else
            {
                EnabledControls(true);
                foreach (FormGroup.ValidationRuleEnum type in types)
                {
                    switch (type)
                    {
                        case FormGroup.ValidationRuleEnum.Custom:
                            DdlType.Items.Add("custom");
                            break;
                        case FormGroup.ValidationRuleEnum.RequiredField:
                            DdlType.Items.Add("requiredField");
                            break;
                    }
                }
            }
        }

        private void EnabledControls(bool b)
        {
            this.BtnDeleteRule.Enabled = b;
            this.BtnSave.Enabled = b;
            this.BtnAddRule.Enabled = b;
            this.DdlType.Enabled = b;
            this.TxtError.Enabled = b;
        }

        private void ShowAddedParameters()
        {
            this.Rbl_Event.Items.Clear();

            if (Parameters != null)
            {
                foreach (CambridgeSoft.COE.Registration.Services.BLL.Parameter param in Parameters)
                    this.Rbl_Event.Items.Add(param.Name + " = " + param.Value);
            }
        }
        
        private bool SavePropDefaultValue(EventMode mode)
        {
            bool retVal = true;
            switch (mode)
            {
                case EventMode.Save:
                    switch (this.DdlType.Text)
                    {
                        case "requiredField":
                            if (!ProceedToAddValidation)
                            {
                                if (this.SelectedProperty.Type.ToUpper() == ConfigurationRegistryRecord.PropertyTypeEnum.PickListDomain.ToString().ToUpper())
                                {
                                    if (this.ConfigurationBO.DefalutValue == "-1")
                                    {
                                        _masterPage.DisplayErrorMessage("Please select default value", true);
                                        retVal = false;
                                    }
                                }
                                else if (this.SelectedProperty.Type.ToUpper() == ConfigurationRegistryRecord.PropertyTypeEnum.Date.ToString().ToUpper())
                                {
                                    if (dateDefaultValue.Text == "")
                                    {
                                        this.ConfigurationBO.DefalutValue = string.Empty;
                                        _masterPage.DisplayErrorMessage("Please select default value", true);
                                        retVal = false;
                                    }
                                 }
                                else
                                {
                                    if (this.ConfigurationBO.DefalutValue.Trim().Length == 0)
                                    {
                                        _masterPage.DisplayErrorMessage("Please enter default value", true);
                                        retVal = false;
                                    }
                                }

                            }
                            break;
                    }
                    break;
                case EventMode.Add:
                    switch (this.DdlType.Text)
                    {
                        case "requiredField":
                            if (!ProceedToAddValidation)
                            {
                                if (this.SelectedProperty.Type.ToUpper() == ConfigurationRegistryRecord.PropertyTypeEnum.PickListDomain.ToString().ToUpper())
                                {
                                    if (DdlDefaultValue.SelectedValue == "-1")
                                    {
                                        this.ConfigurationBO.DefalutValue = string.Empty;
                                        _masterPage.DisplayErrorMessage("Please select default value", true);
                                        retVal = false;
                                    }
                                    else
                                        this.ConfigurationBO.DefalutValue = DdlDefaultValue.SelectedItem.Value;
                                }
                                else if (this.SelectedProperty.Type.ToUpper() == ConfigurationRegistryRecord.PropertyTypeEnum.Date.ToString().ToUpper())
                                {
                                    if (dateDefaultValue.Text == "")
                                    {
                                        this.ConfigurationBO.DefalutValue = string.Empty;
                                        _masterPage.DisplayErrorMessage("Please select default date", true);
                                        retVal = false;
                                    }
                                    else
                                        this.ConfigurationBO.DefalutValue = dateDefaultValue.Text;
                                }
                                else
                                {
                                    if (TxtDefaultValue.Text.Trim().Length == 0)
                                    {
                                        this.ConfigurationBO.DefalutValue = string.Empty;
                                        _masterPage.DisplayErrorMessage("Please enter default value", true);
                                        retVal = false;
                                    }
                                    else
                                        this.ConfigurationBO.DefalutValue = TxtDefaultValue.Text;
                                }

                            }
                            break;
                    }
                    break;
                case EventMode.Delete:
                    this.ConfigurationBO.DefalutValue = "";
                    break;
            }
            return retVal;
        }

        private void CreateParameter()
        {
            switch (this.DdlType.Text)
            {
                case "numericRange":
                case "textLength":
                    if (Parameters == null)
                        this.LabelName.Text = "min";
                    if (!string.IsNullOrEmpty(this.TxtValue.Text))
                    {
                        if (Parameters == null || Parameters.Count == 0)
                        {
                            Parameters = ParameterList.NewParameterList();
                            AddParameter(this.LabelName.Text, this.TxtValue.Text);                            
                            this.DdlType.Enabled = false;
                            this.TxtValue.Text = string.Empty;
                        }
                        else 
                        {
                            AddParameter(this.LabelName.Text, this.TxtValue.Text);                            
                            TxtValue.Text = LabelName.Text = string.Empty;
                            TxtValue.Enabled = BtnAddParameter.Enabled = false;
                        }                        
                    }
                    else
                        _masterPage.DisplayErrorMessage(Resource.NonEmptyParmeterValue_Message, true);
                    break;

                case "wordListEnumeration":
                    if (Parameters == null)
                        this.LabelName.Text = "validWord";
                    if (!string.IsNullOrEmpty(this.TxtValue.Text))
                    {
                        if (Parameters == null)
                            Parameters = ParameterList.NewParameterList();

                        AddParameter(this.LabelName.Text, this.TxtValue.Text);                        
                        this.DdlType.Enabled = false;
                        this.TxtValue.Text = string.Empty;                        
                    }
                    else
                        _masterPage.DisplayErrorMessage(Resource.NonEmptyParmeterValue_Message, true);
                    break;
                default:
                    if (Parameters == null)
                    {
                        Parameters = ParameterList.NewParameterList();
                        AddParameter(string.Empty, string.Empty);                        
                    }
                    break;
            }

            this.BtnCancelParameter.Enabled = true;

            ShowAddedParameters();

            SetParameterDivVisibility();
        }

        private void CancelParameter()
        {
            ClearControl();
            this.BtnCancelParameter.Enabled = true;
            switch (this.DdlType.SelectedValue)
            {
                case "textLength":
                case "numericRange":
                    this.LabelName.Text = "min";
                    break;
                case "wordListEnumeration":
                    this.LabelName.Text = "validWord";
                    break;
            }
            // Select Defalut Rule.
            this.DdlType.ClearSelection();
            this.DdlType.Items.FindByValue(DdlType.Items[0].Value).Selected = true;
        }

        private void AddParameter(string name, string value)
        {
            Parameters.Add(CambridgeSoft.COE.Registration.Services.BLL.Parameter.NewParameter(name, value, true));
        }

        private void AddValidationRule()
        {
            if (!SelectedProperty.ValRuleList.CheckDuplicated(DdlType.SelectedItem.Text))
            {
                this.Parameters = this.Parameters;
                string error = string.Empty;
                bool proceedToAddParam = true;

                switch (DdlType.SelectedValue)
                {
                    case "numericRange":
                    case "textLength":
                        if (Parameters != null)
                        {
                            if (Parameters.Count != 2)
                                _masterPage.DisplayErrorMessage(Resource.ValidationRuleMinAndMax_Message, true);
                            else if (int.Parse(Parameters[0].Value) > int.Parse(Parameters[1].Value))
                            {
                                Parameters.Clear();
                                TxtValue.Text = string.Empty;
                                TxtValue.Enabled = BtnAddParameter.Enabled = true;
                                ShowAddedParameters();
                                _masterPage.DisplayErrorMessage(Resource.IncorectParameterRange_Message, true);
                            }
                            else
                            {
                                error = this.TxtError.Text;
                                if (error == string.Empty && DdlType.SelectedValue == "textLength")
                                    error = string.Format(Resource.TextLengthErrorMessage, Parameters[0].Value, Parameters[1].Value);
                                else if (error == string.Empty)
                                    string.Format(Resource.NumericRangeErrorMessage, Parameters[0].Value, Parameters[1].Value);
                                ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].ValRuleList.Add(ValidationRule.NewValidationRule(this.DdlType.Text, error, this.Parameters, false));
                                BtnCancelParameter.Enabled = true;
                                FillValidationRulesList();
                                ClearControl();
                            }
                        }
                        else
                        {
                            _masterPage.DisplayErrorMessage(Resource.ValidationRuleMinAndMax_Message, true);
                        }
                        break;
                    case "wordListEnumeration":
                        if (Parameters == null || Parameters.Count < 1)
                            _masterPage.DisplayErrorMessage(Resource.OneParameterAtLeaseMessage, true);
                        else
                        {
                            error = this.TxtError.Text;
                            if (error == string.Empty)
                                error = Resource.ValidWordDefaultErrorMessage;
                            ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].ValRuleList.Add(ValidationRule.NewValidationRule(this.DdlType.Text, error, this.Parameters, false));
                            FillValidationRulesList();
                            ClearControl();
                        }
                        break;
                    case "requiredField":
                        proceedToAddParam = SavePropDefaultValue(EventMode.Add);                        
                        if (proceedToAddParam) 
                        {
                            error = this.TxtError.Text;
                            if (error == string.Empty)
                                error = Resource.ReguiredFiledDefaultErrorMessage;
                            Parameters = ParameterList.NewParameterList();
                            ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].ValRuleList.Add(ValidationRule.NewValidationRule(this.DdlType.Text, error, this.Parameters, false));                                                                                   
                            // RAG : CBOE-1423,  Saves default value for type date, text
                            if (SelectedProperty.Type.ToUpperInvariant() == "DATE")
                                ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].DefaultValue = this.dateDefaultValue.Text;
                            else
                                ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].DefaultValue = this.TxtDefaultValue.Text;

                            // RAG : CBOE-1251
                            int intCount = ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].ValRuleList.Count - 1;
                            ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].ValRuleList[intCount].DefaultValue = ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].DefaultValue;

                            FillValidationRulesList();
                            ClearControl();
                        }                       
                        break;
                    case "custom":
                        error = this.TxtError.Text;
                        if (error == string.Empty)
                            error = Resource.ValidationRuleCustomDefault_Message;
                        if (string.IsNullOrEmpty(this.TextAreaClientScript.Text.Trim()))
                        {
                            _masterPage.DisplayErrorMessage(Resource.ValidationRuleCustom_Message, true);
                        }
                        else
                        {
                            Parameters = ParameterList.NewParameterList();
                            this.AddParameter("clientscript", this.TextAreaClientScript.Text);
                            ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].ValRuleList.Add(ValidationRule.NewValidationRule(this.DdlType.Text, error, this.Parameters, false));
                            FillValidationRulesList();
                            ClearControl();
                        }
                        break;
                    default:
                        Parameters = ParameterList.NewParameterList();
                        ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].ValRuleList.Add(ValidationRule.NewValidationRule(this.DdlType.Text, this.TxtError.Text, this.Parameters, false));
                        FillValidationRulesList();
                        ClearControl();
                        break;
                }
            }
            else
                _masterPage.DisplayErrorMessage(Resource.DuplicatedValidationRule_Message, true);

            SetParameterDivVisibility();
        }

        private void ClearControl()
        {
            Parameters = null;
            LabelName.Text = TxtValue.Text = string.Empty;
            TxtValue.Enabled = DdlType.Enabled = BtnAddParameter.Enabled = true;
            TextAreaClientScript.Text = string.Empty;
            TxtError.Text = string.Empty;
            TxtDefaultValue.Text = string.Empty;
            ShowAddedParameters();
            SetParameterDivVisibility();
        }

        private void DeleteValidationRule()
        {
            try
            {
                SelectedProperty.ValRuleList.RemoveValidationRule(this.UltraWebTree_Rules.SelectedNode.Index);
                SavePropDefaultValue(EventMode.Delete);              
                FillValidationRulesList();
            }
            catch
            {
                _masterPage.DisplayErrorMessage(Resource.NoValidationRulesToDelete_Message, true);
            }
            SetParameterDivVisibility();
        }

        private void DeleteParameter()
        {
            if (Rbl_Event.Items != null && Rbl_Event.SelectedIndex != -1)
            {
                if (Parameters[Rbl_Event.SelectedIndex] != null)
                {
                    Parameters.RemoveAt(Rbl_Event.SelectedIndex);

                    SetParameterDivVisibility();

                    ShowAddedParameters();
                }
            }
            else
                _masterPage.DisplayErrorMessage(Resource.NonSeletedParameter_Message, true);

            ShowAddedParameters();
        }

        private void EditParameters()
        {
            Session["SelValRuleID"] = this.UltraWebTree_Rules.SelectedNode.Index;
            Server.Transfer(Resource.EditParameters_URL);
        }

        private void SetParameterDivVisibility()
        {
           
            this.ParametersDiv.Style.Add("display", "none");
            this.ClientScriptDiv.Style.Add("display", "none");

            if (!string.IsNullOrEmpty(this.DdlType.SelectedValue) && (
                this.DdlType.SelectedValue == "wordListEnumeration" ||
                this.DdlType.SelectedValue == "textLength" ||
                this.DdlType.SelectedValue == "numericRange"))
             {
                this.ParametersDiv.Style.Add("display", "");
                if (this.DdlType.SelectedValue == "textLength" ||
                this.DdlType.SelectedValue == "numericRange")
                {
                    this.LabelName.Text = "min";
                    this.BtnAddParameter.Enabled = this.TxtValue.Enabled = true;
                    if (Parameters != null)
                    {
                        if (Parameters.Count == 1 && Parameters[0].Name == "min")
                        {
                            this.LabelName.Text = "max";
                            this.BtnAddParameter.Enabled = this.TxtValue.Enabled = true;
                        }
                        else if (Parameters.Count == 2)
                        {
                            this.LabelName.Text = string.Empty;
                            this.BtnAddParameter.Enabled = this.TxtValue.Enabled = false;
                            this.LabelName.Text = "Parameter list completed";
                        }
                    }
                }
                else
                    this.LabelName.Text = "validWord";
             }
             else if (!string.IsNullOrEmpty(this.DdlType.SelectedValue) && (
                this.DdlType.SelectedValue == "custom"))
             {
                 this.ClientScriptDiv.Style.Add("display", "");
             }

            if (!string.IsNullOrEmpty(this.DdlType.SelectedValue) && (this.DdlType.SelectedValue == "textLength" || this.DdlType.SelectedValue == "numericRange"))             
                this.TextBoxValueValidator.Enabled = true;
            else
                this.TextBoxValueValidator.Enabled = false;


        }

        private void SaveConfigurationBO()
        {
            this.ConfigurationBO = this.ConfigurationBO.Save();
            if (ConfigurationBO.SelectedPropertyList == ConfigurationRegistryRecord.PropertyListType.PropertyList)
                Server.Transfer(Resource.RootPorpertyList_URL);
            else
                Server.Transfer(Resource.RegAdmin_ContentArea_URL + ConfigurationBO.SelectedPropertyList.ToString() + ".aspx");
            
        }

        #endregion
    }
}
