using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;

using Resources;

using CambridgeSoft.COE.Framework.COEPickListPickerService;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.GUIShell;

using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.Caching;
using CambridgeSoft.COE.RegistrationAdmin;
using CambridgeSoft.COE.RegistrationAdmin.Services;
using CambridgeSoft.COE.RegistrationAdmin.Services.Common;

namespace CambridgeSoft.COE.RegistrationAdminWebApp.Forms.RegistrationAdmin.UserControls
{
    public partial class PropertyListUC : System.Web.UI.UserControl
    {
        #region Variables

        private string _prefix;
        private CambridgeSoft.COE.Registration.Services.Types.PropertyList _selectedPropertyList;
        private CourrentPageEnum _courrentPage;
        RegistrationMaster _masterPage = null;
        private const string VALIDATIONGROUP = "PropertyList";

        private const string MODALPROGRESS_SAVE = "Saving...";
        private const string MODALPROGRESS_ADD = "Adding...";
        private const string MODALPROGRESS_DELETE = "Deleting...";
        private const string MODALPROGRESS_SORTORDER = "Updating...";

        #endregion

        #region Properties

        public string DataSourceID
        {
            get
            {
                if (ViewState["DataSourceID"] == null)
                    ViewState["DataSourceID"] = string.Empty;

                return (string)ViewState["DataSourceID"];
            }
            set
            {
                ViewState["DataSourceID"] = value;
            }
        }

        public CourrentPageEnum CurrentPage
        {
            get
            {
                return _courrentPage;
            }
            set
            {
                _courrentPage = value;
            }
        }

        public ConfigurationRegistryRecord ConfigurationBO
        {
            get
            {
                if (Session["ConfigurationRegistryRecord"] == null)
                    Session["ConfigurationRegistryRecord"] = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();

                return (ConfigurationRegistryRecord)Session["ConfigurationRegistryRecord"];
            }
            set
            {
                Session["ConfigurationRegistryRecord"] = value;
            }
        }

        public PropertyList SelectedPropertyList
        {
            get
            {
                if (_selectedPropertyList == null)
                    return (PropertyList)Session["SelectedPropertyList"];
                else
                {
                    return _selectedPropertyList;
                }
            }
            set
            {
                if (value == null)
                {
                    _selectedPropertyList = value;
                    Session["SelectedPropertyList"] = value;
                }

            }
        }
               

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //catch changeis session varialbe so you can display the save method for configuration. otherwise it is often missed
                    if (Session["EditDone"] != null && Session["EditDone"] != string.Empty)
                    {
                        ShowConfirmationMessage(string.Format(Resource.PropertySuccessfullyEdited_MessagesArea, Session["EditDone"]));
                        Session["EditDone"] = "";
                    }
                    this.ConfigurationBO.CleanUpConfiguration(); // Must rule to follow . add more to clear values on start up.
                    SetControlsAttributes();
                    TakePropertyList();
                    FillPropertyList(-1, true);
                }
                else
                {
                    TakePropertyList();
                }
                SetPropertyNamePrefix();
                SetPrecisionDivVisibility();
                SetProgressModal();
                _masterPage.SetDefaultAction(this.BtnSave.UniqueID);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }

        protected override void OnInit(EventArgs e)
        {
            if (this.Page.Master is RegistrationMaster)
                _masterPage = (RegistrationMaster)this.Page.Master;
            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                base.OnPreRender(e);
                this.RegisterJSFunctions();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
            
        }

        #endregion

        #region Methods

        private void RegisterJSFunctions() 
        {
            foreach (ListItem control in this.Rbl_Properties.Items)
            {
                control.Attributes.Add("onclick", "UpDownArrowsVisibility(this)");
            }
            if (!this.Page.ClientScript.IsStartupScriptRegistered(typeof(PropertyListUC), "UpDownArrowsVisibilityScript"))
            {
                string script = @"
                        function ConfirmDelete()
                        {
                           if(confirm('" + Resource.DeleteProperty_Message  + @"'))
                                return true;
                           else
                            {
                                YAHOO.ChemOfficeEnterprise.ProgressModal.MasterProgressModal.hide();
                                return false;
                            }
                        } 
                        function UpDownArrowsVisibility(radioClicked)
                        {
                            upButton = document.getElementById('" + this.UpPropertyButton.ClientID + @"');
                            downButton = document.getElementById('" + this.DownPropertyButton.ClientID + @"');
                            upButton.style.display='';
                            downButton.style.display='';
                            firstElementKey = '';
                            lastElementKey = '';";
                if (this.Rbl_Properties.Items.Count > 0)
                {

                    script += @"
                            firstElementKey = '" + this.Rbl_Properties.Items[0].Value + @"';
                            lastElementKey = '" + this.Rbl_Properties.Items[this.Rbl_Properties.Items.Count - 1].Value + @"';";
                }
                script += @"

                            if(firstElementKey == '' || radioClicked.value == firstElementKey) { 
                                upButton.style.display='none';                                                              
                            }else{
                                upButton.style.display='';
                            }
                                                    
                            if(lastElementKey == '' || radioClicked.value == lastElementKey) {  
                                downButton.style.display='none';                               
                            }else{
                                downButton.style.display='block';
                            }
                          
                        }";

                this.Page.ClientScript.RegisterStartupScript(typeof(PropertyListUC), "UpDownArrowsVisibility", script, true);
            }

            if (!this.Page.ClientScript.IsStartupScriptRegistered(typeof(PropertyListUC), "ShowHidePrecisionTextBox"))
            {
                string script = @"
                        function ShowHidePrecisionTextBox(selectedType)
                        {
                            SetRegExpValidator(selectedType);
                            decimalPrecision = document.getElementById('" + this.TextBoxDecimalPrecision.ClientID + @"');
                            precision = document.getElementById('" + this.PrecisionDiv.ClientID + @"');                        
                            decimalDot = document.getElementById('" + this.LabelDecimalDot.ClientID + @"');
                            integerPrecision = document.getElementById('" + this.TextBoxIntegerPrecision.ClientID + @"');
                            dropDown = document.getElementById('" + this.DdlType.ClientID + @"');
                            precisionLbl = document.getElementById('" + this.LblPrecision.ClientID + @"');
                            selectedText = dropDown.options[dropDown.selectedIndex].text;
                            pickListDomainDiv = document.getElementById('" + this.PickListDomainsDiv.ClientID + @"');                       
                            pickListDomainDiv.style.display = 'none';                        
                            switch(selectedType)
                            {
                                case 'TEXT':
                                     precision.style.display = '';
                                     decimalPrecision.style.display = 'none';
                                     decimalDot.style.display = 'none';
                                     integerPrecision.className = 'ControlElements';
                                     integerPrecision.style.textAlign = 'left';
                                     integerPrecision.value = '200';
                                     precisionLbl.innerHTML  = 'Length:';                                
                                     break;
                                case 'NUMBER':
                                     precision.style.display = '';
                                     decimalPrecision.style.display = '';                        
                                     decimalDot.style.display = '';                                
                                     integerPrecision.className = 'NumericInput';
                                     integerPrecision.style.textAlign = 'right';
                                     integerPrecision.style.display='inline';
                                     decimalPrecision.style.display='inline';
                                     decimalDot.style.display='inline';
                                     precisionLbl.style.display='inline';
                                     integerPrecision.value = '8';
                                     decimalPrecision.value = '0';
                                     precisionLbl.innerHTML  = 'Precision:';   
                                     break;
                                case 'DATE':
                                case 'BOOLEAN':
                                      precision.style.display = 'none';                                 
                                      break;
                                case 'URL':
                                     precision.style.display='none';
                                     integerPrecision.className = 'ControlElements';
                                     integerPrecision.value = '200';                                 
                                     break;
                                case 'INTEGER':
                                     decimalPrecision.style.display = '';                        
                                     decimalDot.style.display = '';                                
                                     integerPrecision.className = 'NumericInput';
                                     integerPrecision.style.textAlign = 'right';
                                     integerPrecision.style.display='inline';
                                     decimalPrecision.style.display='inline';
                                     decimalDot.style.display='inline';
                                     precisionLbl.style.display='inline';  
                                     integerPrecision.value='9';
                                     decimalPrecision.value='0';
                                     precision.style.display='none';   
                                     break;
                                 case 'FLOAT':
                                     decimalPrecision.style.display = '';                        
                                     decimalDot.style.display = '';                                
                                     integerPrecision.className = 'NumericInput';
                                     integerPrecision.style.textAlign = 'right';
                                     integerPrecision.style.display='inline';
                                     decimalPrecision.style.display='inline';
                                     decimalDot.style.display='inline';
                                     precisionLbl.style.display='inline';  
                                     integerPrecision.value='8';
                                     decimalPrecision.value='6';
                                     precision.style.display='none';   
                                     break;
                                 case 'PICKLISTDOMAIN':
                                     precision.style.display = 'none';
                                     pickListDomainDiv.style.display = '';                                 
                                     break;
                            }
                        }";

                this.Page.ClientScript.RegisterStartupScript(typeof(PropertyListUC), "ShowHidePrecisionTextBox", script, true);
            }

            if (!this.Page.ClientScript.IsStartupScriptRegistered(typeof(PropertyListUC), "SetRegExpValidator"))
            {
                string script = @"
                        function SetRegExpValidator(selectedType)
                        {
                            decimalPrecision = document.getElementById('" + this.RegularExpressionValidatorDecimal.ClientID + @"');
                            integerPrecision = document.getElementById('" + this.RegularExpressionValidatorInteger.ClientID + @"'); 
                            requiredIntegerPrecision = document.getElementById('" + this.RequiredIntegerPrecision.ClientID + @"'); 
                            requiredDecimalPrecision = document.getElementById('" + this.RequiredDecimalPrecision.ClientID + @"'); 
                            switch(selectedType)
                            {
                                case 'TEXT':                                                             
                                ValidatorEnable(decimalPrecision, false);
                                ValidatorEnable(integerPrecision, true);
                                ValidatorEnable(requiredIntegerPrecision, true);
                                ValidatorEnable(requiredDecimalPrecision, false);
                                break;
                                case 'NUMBER':                                 
                                ValidatorEnable(decimalPrecision, true);
                                ValidatorEnable(integerPrecision, true);
                                ValidatorEnable(requiredIntegerPrecision, true);
                                ValidatorEnable(requiredDecimalPrecision, true);
                                break;
                                case 'DATE':
                                case 'BOOLEAN':
                                ValidatorEnable(decimalPrecision, false);
                                ValidatorEnable(integerPrecision, false);
                                ValidatorEnable(requiredIntegerPrecision, false);
                                ValidatorEnable(requiredDecimalPrecision, false);
                                break; 
                                case 'URL':                                                             
                                    ValidatorEnable(decimalPrecision, false);
                                    ValidatorEnable(integerPrecision, true);
                                    ValidatorEnable(requiredIntegerPrecision, false);
                                    ValidatorEnable(requiredDecimalPrecision, false);                                
                                    break;
                                case 'FLOAT':                                                             
                                    ValidatorEnable(decimalPrecision, true);
                                    ValidatorEnable(integerPrecision, true);
                                    ValidatorEnable(requiredIntegerPrecision, false);
                                    ValidatorEnable(requiredDecimalPrecision, false);                                
                                    break; 
                                 case 'INTEGER':                                                             
                                    ValidatorEnable(decimalPrecision, true);
                                    ValidatorEnable(integerPrecision, true);
                                    ValidatorEnable(requiredIntegerPrecision, false);
                                    ValidatorEnable(requiredDecimalPrecision, false);                                
                                    break; 
                            }
                        }";

                this.Page.ClientScript.RegisterStartupScript(typeof(PropertyListUC), "SetRegExpValidator", script, true);
            }

            if (!this.Page.ClientScript.IsStartupScriptRegistered(typeof(PropertyListUC), "CleanFields"))
            {
                string script = @"
                        function CleanFields()
                        {
                            propertyName = document.getElementById('" + this.TxtName.ClientID + @"');
                            decimalPrecision = document.getElementById('" + this.TextBoxDecimalPrecision.ClientID + @"');
                            integerPrecision = document.getElementById('" + this.TextBoxIntegerPrecision.ClientID + @"'); 
                            
                            propertyName.value = '';
                            decimalPrecision.value = '';
                            integerPrecision.value = '';
                            
              
                        }";

                this.Page.ClientScript.RegisterStartupScript(typeof(PropertyListUC), "CleanFields", script, true);
            }          
        }

        private void EditProperty()
        {
            if (Rbl_Properties.SelectedIndex == -1)
            {

                _masterPage.DisplayErrorMessage(Resource.SelectProperty_Label_Text, false);
            }
            else if (ConfigurationBO.GetSelectedPropertyList[_selectedPropertyList.GetPropertyIndex(Rbl_Properties.SelectedValue)].Type == "NUMBER" ||
                ConfigurationBO.GetSelectedPropertyList[_selectedPropertyList.GetPropertyIndex(Rbl_Properties.SelectedValue)].Type == "TEXT")
            {
                int selectedIndex = _selectedPropertyList.GetPropertyIndex(Rbl_Properties.SelectedValue);
                if (selectedIndex > -1)
                { 

                    Session["SelectedProperty"] = ConfigurationBO.GetSelectedPropertyList[selectedIndex];
                    Server.Transfer("PropertyEdit.aspx");
                }
                else
                {
                    _masterPage.DisplayErrorMessage( Resource.SelectProperty_Label_Text , false);
                }
            }
            else
            {
                ShowConfirmationMessage(Resource.NoEditableProperty_Message_Text);
            }
        }

        private void SetPropertyNamePrefix()
        {

            switch (ConfigurationBO.SelectedPropertyList)
            {
                case ConfigurationRegistryRecord.PropertyListType.PropertyList:
                    _prefix = RegAdminUtils.GetRegistryPrefix();
                    break;
                case ConfigurationRegistryRecord.PropertyListType.Batch:
                    _prefix = RegAdminUtils.GetBatchPrefix();
                    break;
                case ConfigurationRegistryRecord.PropertyListType.Compound:
                    _prefix = RegAdminUtils.GetComponentPrefix();
                    break;
                case ConfigurationRegistryRecord.PropertyListType.BatchComponent:
                    _prefix = RegAdminUtils.GetBatchComponentsPrefix();
                    break;
                case ConfigurationRegistryRecord.PropertyListType.Structure:
                    _prefix = RegAdminUtils.GetStructurePrefix();
                    break;
            }
        }

        private void SaveChanges()
        {
            ConfigurationBO.UpdateSelf();
        }

        private void SetDefaultSettings()
        {
            //if (this.DdlType.Items.FindByText("TEXT") != null)
            //    this.DdlType.SelectedValue = this.DdlType.Items.FindByText("TEXT").Value;
            //Coverity fix - CID 11789 - Used local variable to resolve Dereference null return value.
            if (this.DdlType.Items != null && this.DdlType.Items.Count > 0)
            {
                ListItem lstTemp = this.DdlType.Items.FindByText("TEXT");
                if (lstTemp != null)
                    this.DdlType.SelectedValue = lstTemp.Value;
            }
            this.TextBoxIntegerPrecision.Text = "200";
            this.TxtName.Text = string.Empty;
            this.SetPrecisionDivVisibility();
        }

        private void SetProgressModal()
        {

            _masterPage.MakeCtrlShowProgressModal(this.BtnSave.ClientID, MODALPROGRESS_SAVE, string.Empty, false);
            _masterPage.MakeCtrlShowProgressModal(this.BtnAddProperty.ClientID, MODALPROGRESS_ADD, string.Empty, true);
            _masterPage.MakeCtrlShowProgressModal(this.BtnDeleteProp.ClientID, MODALPROGRESS_DELETE, string.Empty, false);
            _masterPage.MakeCtrlShowProgressModal(this.UpPropertyButton.ClientID, MODALPROGRESS_SORTORDER, string.Empty, false);
            _masterPage.MakeCtrlShowProgressModal(this.DownPropertyButton.ClientID, MODALPROGRESS_SORTORDER, string.Empty, false);

        }

        internal void SetControlsAttributes()
        {
            try
            {
                this.SetValidators();
                this.LblName.Text = Resource.Name_Label_Text;
                this.LblType.Text = Resource.Type_Label_Text;
                this.LblPrecision.Text = Resource.Precision_Label_Text;
                this.BtnAddProperty.Text = Resource.Add_Property_Button_Text;
                this.BtnDeleteProp.Text = Resource.Delete_Property_Button_Text;
                this.Button_ValidationRules.Text = Resource.ValidationRules_LinkButton_Text;
                this.LabelDecimalDot.Text = ".";
                this.LabelLabel.Text = Resource.Label_Label_Tex;
                this.TextBoxIntegerPrecision.Text = "200";
                this.BtnDeleteProp.Attributes.Add("onclick", "javascript:return ConfirmDelete('" + Resource.DeleteProperty_Message + "');");
                this.BtnSave.Text = Resource.Save_Button_Text;
                FillPropertyTypes();
                FillPickListDomains();
                this.UpPropertyButton.ToolTip = Resource.UpProperty_Button_ToolTip;
                this.DownPropertyButton.ToolTip = Resource.DownProperty_Button_ToolTip;
                this.ButtonEditProperty.Text = Resource.Edit_Button_Text;
                this.LabelPickListDomains.Text = Resource.PickListDomainLabel_Text;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleUIException(ex);
                _masterPage.DisplayErrorMessage(ex.Message, false);
            }
        }

        private void SetValidators()
        {
            this.ValidationSummary.EnableClientScript = true;
            this.RegularExpressionValidatorName.ValidationExpression = "^[a-zA-Z_@#][a-zA-Z0-9_\\$@#]{0,28}[a-zA-Z0-9_\\$@#]$";
            this.RegularExpressionValidatorName.Text = "*";
            this.RegularExpressionValidatorName.ErrorMessage = "Invalid property name: use only alpha-numeric and underscore characters (no spaces, 30 characters max)";
            this.RegularExpressionValidatorName.EnableClientScript = true;
            this.RequiredTxtName.ErrorMessage = "Property name field is required";
            this.RequiredTxtName.Text = "*";
            this.RegularExpressionValidatorLabel.ValidationExpression = @"^[a-zA-Z0-9.\-_,;:\?!\[\]\{\}\(\)][a-zA-Z0-9\s.\-_,;:\?!\[\]\{\}\(\)]{0,28}[a-zA-Z0-9.\-_,;:\?!\[\]\{\}\(\)]$";
            this.RegularExpressionValidatorLabel.Text = "*";
            this.RegularExpressionValidatorLabel.ErrorMessage = "Invalid label text: some punctuation characters not allowed (30 characters max)";
            this.RegularExpressionValidatorLabel.EnableClientScript = true;
            this.RequiredTxtLabel.ErrorMessage = "Property label field is required";
            this.RequiredTxtLabel.Text = "*";
            this.RegularExpressionValidatorInteger.ValidationExpression = "[0-9]{0,8}";
            this.RegularExpressionValidatorInteger.Text = "*";
            this.RegularExpressionValidatorInteger.ErrorMessage = "Invalid integer number precision";
            this.RegularExpressionValidatorInteger.EnableClientScript = true;
            this.RequiredIntegerPrecision.ErrorMessage = "Precision field is required";
            this.RequiredIntegerPrecision.Text = "*";
            this.RegularExpressionValidatorDecimal.ValidationExpression = "[0-9]{0,8}";
            this.RegularExpressionValidatorDecimal.Text = "*";
            this.RegularExpressionValidatorDecimal.ErrorMessage = "Invalid decimal number precision";
            this.RegularExpressionValidatorDecimal.EnableClientScript = true;
            this.RequiredDecimalPrecision.ErrorMessage = "Decimal part of precision field is required";
            this.RequiredDecimalPrecision.Text = "*";
        }

        private void TakePropertyList()
        {
            this._selectedPropertyList = ConfigurationBO.GetSelectedPropertyList;
            Session["SelectedPropertyList"] = this._selectedPropertyList;

        }

        private bool  AddProperty()
        {
            bool retVal = false;

            if (this.TxtName.Text != string.Empty)
            {
                if ((TxtName.Text.ToUpper() != "DATE" && TxtName.Text.ToUpper() != "NUMBER" &&
                    TxtName.Text.ToUpper() != "TEXT" && TxtName.Text.ToUpper() != "BOOLEAN" && TxtName.Text.ToUpper() != "PICKLISTDOMAIN") && !ConfigurationBO.DatabaseReservedWords.Contains(TxtName.Text.ToUpper()) )
                {
                    this.ConfigurationBO.SelectedPropertyName = this.TxtName.Text;
                    this.MessagesAreaUserControl.Visible = false;
                    if (this.RegularExpressionValidatorName.IsValid)
                    {
                        string precision = string.Empty;
                        string selectedType = this.GetSelectedValueFromDDL();
                        if (selectedType == "NUMBER" && this.TextBoxDecimalPrecision.Text != string.Empty)
                            precision = this.TextBoxIntegerPrecision.Text + "," + this.TextBoxDecimalPrecision.Text;
                        else if (selectedType == "TEXT")
                            precision = this.TextBoxIntegerPrecision.Text;
                        else
                            precision = "1";

                        string duplicatedMessage = string.Empty;

                        // Check for property names exit in DB. 
                        // Fix for CSBR-159151,CSBR-159364.Revision no in perforce : #3 1250_Developement.

                        if (ConfigurationBO.PropertyList.CheckExistingNames(this.TxtName.Text.ToUpper(), true) || ConfigurationBO.PropertyColumnList.Contains(this.TxtName.Text.ToUpper()))
                            duplicatedMessage = Resource.RegistryPropertyExists_MasterPage;
                        else if (ConfigurationBO.BatchPropertyList.CheckExistingNames(this.TxtName.Text.ToUpper(), true) || ConfigurationBO.BatchPropertyColumnList.Contains(this.TxtName.Text.ToUpper()))
                                    duplicatedMessage = Resource.BatchPropertyExists_MasterPage;
                        else if (ConfigurationBO.BatchComponentList.CheckExistingNames(this.TxtName.Text.ToUpper(), true) ||ConfigurationBO.BatchComponentColumnList.Contains(this.TxtName.Text.ToUpper()))
                                    duplicatedMessage = Resource.BatchComponentPropertyExists_MasterPage;
                        else if (ConfigurationBO.CompoundPropertyList.CheckExistingNames(this.TxtName.Text.ToUpper(), true) || ConfigurationBO.CompoundPropertyColumnList.Contains(this.TxtName.Text.ToUpper()))
                                    duplicatedMessage = Resource.CompoundPropertyExists_MasterPage;
                        else if (ConfigurationBO.StructurePropertyList.CheckExistingNames(this.TxtName.Text.ToUpper(), true) || ConfigurationBO.StructurePropertyColumnList.Contains(this.TxtName.Text.ToUpper()))
                                    duplicatedMessage = Resource.StructurePropertyExists_MasterPage;
                          
                        if (duplicatedMessage == string.Empty)
                        {
                            try
                            {
                                if (precision != string.Empty)
                                {
                                    string picklistDomainId = string.Empty;
                                    if (selectedType == "PICKLISTDOMAIN")
                                        picklistDomainId = this.DropDownListPickListDomains.SelectedItem.Value;

                                    string subType = (this.DdlType.SelectedValue=="URL"?"URL":string.Empty);
                                    ConfigurationProperty confProperty = ConfigurationProperty.NewConfigurationProperty(_prefix + this.TxtName.Text.ToUpper(),
                                    this.TxtName.Text.ToUpper(), selectedType, precision, true, subType, picklistDomainId);

                                    if (!this._selectedPropertyList.CheckExistingNames(confProperty.Name,true))
                                    {
                                        this._selectedPropertyList.AddProperty(confProperty);

                                        switch (CurrentPage)
                                        {
                                            case CourrentPageEnum.PropertyList:
                                                if (TextBoxLabel.Text == string.Empty)
                                                    ConfigurationBO.PropertiesLabels[0].Add(_prefix + this.TxtName.Text.ToUpper(), this.TxtName.Text);
                                                else
                                                    ConfigurationBO.PropertiesLabels[0].Add(_prefix + this.TxtName.Text.ToUpper(), this.TextBoxLabel.Text);
                                                break;
                                            case CourrentPageEnum.Compound:
                                                if (TextBoxLabel.Text == string.Empty)
                                                    ConfigurationBO.PropertiesLabels[1].Add(_prefix + this.TxtName.Text.ToUpper(), this.TxtName.Text);
                                                else
                                                    ConfigurationBO.PropertiesLabels[1].Add(_prefix + this.TxtName.Text.ToUpper(), this.TextBoxLabel.Text);
                                                break;
                                            case CourrentPageEnum.Batch:
                                                if (TextBoxLabel.Text == string.Empty)
                                                    ConfigurationBO.PropertiesLabels[2].Add(_prefix + this.TxtName.Text.ToUpper(), this.TxtName.Text);
                                                else
                                                    ConfigurationBO.PropertiesLabels[2].Add(_prefix + this.TxtName.Text.ToUpper(), this.TextBoxLabel.Text);
                                                break;
                                            case CourrentPageEnum.BatchComponent:
                                                if (TextBoxLabel.Text == string.Empty)
                                                    ConfigurationBO.PropertiesLabels[3].Add(_prefix + this.TxtName.Text.ToUpper(), this.TxtName.Text);
                                                else
                                                    ConfigurationBO.PropertiesLabels[3].Add(_prefix + this.TxtName.Text.ToUpper(), this.TextBoxLabel.Text);
                                                break;
                                            case CourrentPageEnum.Structure:
                                                if (TextBoxLabel.Text == string.Empty)
                                                    ConfigurationBO.PropertiesLabels[4].Add(_prefix + this.TxtName.Text.ToUpper(), this.TxtName.Text);
                                                else
                                                    ConfigurationBO.PropertiesLabels[4].Add(_prefix + this.TxtName.Text.ToUpper(), this.TextBoxLabel.Text);
                                                break;
                                        }

                                        SelectedPropertyList = this._selectedPropertyList;

                                        ShowConfirmationMessage( string.Format(Resource.PropertySuccessfullyAdded_MessagesArea, confProperty.FriendlyName) );
                                        retVal = true;
                                    }
                                    else
                                    {
                                        _masterPage.DisplayErrorMessage(string.Format(Resource.PropertyAlreadyExists_MasterPage, confProperty.FriendlyName), false);
                                    }
                                }
                                else
                                    _masterPage.DisplayErrorMessage(Resource.InvalidPrecisionValue_MasterPage , false);
                            }
                            catch (Exception e)
                            {
                                _masterPage.DisplayErrorMessage(e.Message, false);
                            }
                        }
                        else
                        {
                            _masterPage.DisplayErrorMessage(duplicatedMessage, false);
                        }
                    }
                    else
                    {
                        _masterPage.DisplayErrorMessage( Resource.InvalidPropertyNameFormat_MasterPage , false);
                    }
                }
                else
                {
                    _masterPage.DisplayErrorMessage( Resource.PropertyNameSameAsPropertyType_MasterPage , false);
                }
            }
            else if (TextBoxIntegerPrecision.Text == string.Empty)
            {
                _masterPage.DisplayErrorMessage( Resource.PropertyNameAndPrecisionValueRequired_MasterPage , false);
            }
            else
            {
                _masterPage.DisplayErrorMessage( Resource.PropertyNameRequired_MasterPage , false);
            }
            return retVal;
        }

        private void DeleteProperty()
        {
            try
            {
                if (this.Rbl_Properties.SelectedIndex != -1)
                {
                    string propertyName = Rbl_Properties.SelectedItem.Value;
                    string friendlyName = Rbl_Properties.SelectedItem.Text;

                    int index = _selectedPropertyList.GetPropertyIndex(propertyName);

                    if (_selectedPropertyList[propertyName].IsNew)
                    {
                        switch (CurrentPage)
                        {
                            case CourrentPageEnum.PropertyList:
                                ConfigurationBO.PropertiesLabels[0].Remove(propertyName);
                                break;
                            case CourrentPageEnum.Compound:
                                ConfigurationBO.PropertiesLabels[1].Remove(propertyName);
                                break;
                            case CourrentPageEnum.Batch:
                                ConfigurationBO.PropertiesLabels[2].Remove(propertyName);
                                break;
                            case CourrentPageEnum.BatchComponent:
                                ConfigurationBO.PropertiesLabels[3].Remove(propertyName);
                                break;
                            case CourrentPageEnum.Structure:
                                ConfigurationBO.PropertiesLabels[4].Remove(propertyName);
                                break;
                        }
                    }

                    _selectedPropertyList.RemoveAt(index);
                    SelectedPropertyList = _selectedPropertyList;
                    ShowConfirmationMessage(string.Format(Resource.PropertyDeletedSuccessfully_MessagesArea,friendlyName));
                }
                else
                {
                    _masterPage.DisplayErrorMessage(Resource.SelectProperty_Label_Text, false);
                }
            }
            catch (Exception e)
            {
                _masterPage.DisplayErrorMessage(e.Message, true);
            }
        }

        private void ClearProperty()
        {
            this.TxtName.Text = string.Empty;
            this.TextBoxLabel.Text = string.Empty;
            this.TextBoxIntegerPrecision.Text = string.Empty;
            this.TextBoxDecimalPrecision.Text = string.Empty;
        }

        private void FillPropertyList(int selectedItem, bool isEnable)
        {
            this.Rbl_Properties.Items.Clear();

            for (int i = 0; i < SelectedPropertyList.Count; i++)
            {
                Rbl_Properties.Items.Add("");
            }

            foreach (Property prop in _selectedPropertyList)
            {
                this.Rbl_Properties.Items[prop.SortOrder].Text = prop.FriendlyName;
                this.Rbl_Properties.Items[prop.SortOrder].Value = prop.Name;
            }
            
            if (selectedItem != -1)
            {
                Rbl_Properties.SelectedIndex = selectedItem;
            }
           
            if (selectedItem == 0)
            {
                this.UpPropertyButton.Style.Add(HtmlTextWriterStyle.Display, "none");
            }
            else
            {
                this.UpPropertyButton.Style.Add(HtmlTextWriterStyle.Display, "block");
            }

            if (selectedItem == (_selectedPropertyList.Count - 1))
            {
                this.DownPropertyButton.Style.Add(HtmlTextWriterStyle.Display, "none");
            }
            else
            {
                this.DownPropertyButton.Style.Add(HtmlTextWriterStyle.Display, "block");
            }

            if (selectedItem == -1)
            {
                this.UpPropertyButton.Style.Add(HtmlTextWriterStyle.Display, "none");
                this.DownPropertyButton.Style.Add(HtmlTextWriterStyle.Display, "none");
            }

            this.UpPropertyButton.Enabled = isEnable;
            this.DownPropertyButton.Enabled = isEnable;
            Button_ValidationRules.Enabled = isEnable;
            ButtonEditProperty.Enabled = isEnable;
            if (!isEnable)
            {
                this.UpPropertyButton.ToolTip = Resource.ManageProperties_Disable_ToolTip;
                this.DownPropertyButton.ToolTip = Resource.ManageProperties_Disable_ToolTip;
                Button_ValidationRules.ToolTip = Resource.ManageProperties_Disable_ToolTip;
                ButtonEditProperty.ToolTip = Resource.ManageProperties_Disable_ToolTip;

            }
        }

        private void SaveSelectedProperty()
        {
            int selectedIndex = _selectedPropertyList.GetPropertyIndex(Rbl_Properties.SelectedValue); // Returns -1 if property is not available 

            if (selectedIndex == -1)
            {
                _masterPage.DisplayErrorMessage(Resource.SelectProperty_Label_Text, false);
            }
            else if (ConfigurationBO.GetSelectedPropertyList[selectedIndex] != null && ConfigurationBO.GetSelectedPropertyList[selectedIndex].Type.ToUpper() == CambridgeSoft.COE.RegistrationAdmin.Services.ConfigurationRegistryRecord.PropertyTypeEnum.Boolean.ToString().ToUpper())
            {

                _masterPage.DisplayErrorMessage("Boolean type property is not allowed to add validation", false);
            }
            else
            {
                if (selectedIndex > -1)
                {
                    Session["SelectedProperty"] = ConfigurationBO.GetSelectedPropertyList[selectedIndex];
                    Server.Transfer("ValidationRules.aspx");
                }
                else
                {
                    _masterPage.DisplayErrorMessage( Resource.SelectProperty_Label_Text , false);
                }
            }
        }

        private void FillPropertyTypes()
        {
            //ljb: csbr-13010 to fix this, we will do hard coding this list, at this was already started.  Here I have rearranged the order
            //so there is some sorting.  I am turning off getting ths at the configuratoin record level since there was already hard coding (url, float, url).  
            //foreach (ConfigurationRegistryRecord.PropertyTypeEnum type in Enum.GetValues(typeof(ConfigurationRegistryRecord.PropertyTypeEnum)))
             //DdlType.Items.Add(new ListItem(type.ToString().ToUpper(), type.ToString().ToUpper()));
            //Fake ones.
            //DdlType.Items.Add(new ListItem("INTEGER", ConfigurationRegistryRecord.PropertyTypeEnum.Number.ToString().ToUpper()));
            //DdlType.Items.Add(new ListItem("FLOAT", ConfigurationRegistryRecord.PropertyTypeEnum.Number.ToString().ToUpper()));
            //DdlType.Items.Add(new ListItem("URL", ConfigurationRegistryRecord.PropertyTypeEnum.Text.ToString().ToUpper()));
            DdlType.Items.Add(new ListItem("BOOLEAN", "BOOLEAN"));
            DdlType.Items.Add(new ListItem("DATE", "DATE"));
            DdlType.Items.Add(new ListItem("FLOAT", "FLOAT"));
            DdlType.Items.Add(new ListItem("INTEGER", "INTEGER"));
            DdlType.Items.Add(new ListItem("NUMBER", "NUMBER"));
            DdlType.Items.Add(new ListItem("PICKLISTDOMAIN", "PICKLISTDOMAIN")); 
            DdlType.Items.Add(new ListItem("TEXT", "TEXT"));
            DdlType.Items.Add(new ListItem("URL", "URL"));//TODO:Remove -NotReady text.
        }

        private string GetSelectedValueFromDDL()
        {
            string retVal = string.Empty;
            if (!string.IsNullOrEmpty(this.DdlType.SelectedValue))
            {
                switch (this.DdlType.SelectedValue)
                {
                    case "INTEGER":
                        retVal = ConfigurationRegistryRecord.PropertyTypeEnum.Number.ToString().ToUpper();
                        break;
                    case "FLOAT":
                        retVal = ConfigurationRegistryRecord.PropertyTypeEnum.Number.ToString().ToUpper();
                        break;
                    case "URL":
                        retVal = ConfigurationRegistryRecord.PropertyTypeEnum.Text.ToString().ToUpper();
                        break;
                    default:
                        retVal = this.DdlType.SelectedValue;
                        break;
                }
            }
            return retVal;
        }

        private void ChangePropertyOrder(ChangeOrder changeOrder)
        {
            int selectPropertyOrder = this.Rbl_Properties.SelectedIndex;
            string selectPropertyName = this.Rbl_Properties.SelectedValue.ToString();

            try
            {
                if (changeOrder == ChangeOrder.Up)
                {
                    string affectedPropertyName = _selectedPropertyList.ChangeOrder(selectPropertyOrder - 1, true, _selectedPropertyList[selectPropertyName].Name);
                    Dictionary<string, string> propertyLabels = GetPropertyLabelsByContainedPropertyName(_selectedPropertyList[selectPropertyName].Name);
                    if (propertyLabels != null)
                    {
                        propertyLabels.Add(_selectedPropertyList[affectedPropertyName].Name,
                            _selectedPropertyList[affectedPropertyName].FriendlyName);
                    }
                }
                else
                {
                    string affectedPropertyName = _selectedPropertyList.ChangeOrder(selectPropertyOrder + 1, false, _selectedPropertyList[selectPropertyName].Name);
                    Dictionary<string, string> propertyLabels = GetPropertyLabelsByContainedPropertyName(_selectedPropertyList[selectPropertyName].Name);
                    if (propertyLabels != null)
                    {
                        propertyLabels.Add(_selectedPropertyList[affectedPropertyName].Name,
                            _selectedPropertyList[affectedPropertyName].FriendlyName);
                    }                    
                }
                FillPropertyList(_selectedPropertyList[selectPropertyName].SortOrder, true);
            }
            catch
            {
                FillPropertyList(_selectedPropertyList[selectPropertyName].SortOrder, true);
            }
        }

        private Dictionary<string, string> GetPropertyLabelsByContainedPropertyName(string propertyName)
        {
            foreach (Dictionary<string, string> propertyLabels in ConfigurationBO.PropertiesLabels)
            {
                if (propertyLabels.ContainsKey(propertyName))
                {
                    return propertyLabels;
                }
            }

            return null;
        }

        private void SetPrecisionDivVisibility()
        {
            this.PickListDomainsDiv.Style.Add("display", "none");
            switch (this.GetSelectedValueFromDDL())
            {
                case "TEXT":
                    this.PrecisionDiv.Style.Add("display", "block");
                    this.TextBoxDecimalPrecision.Style.Add("display", "none");
                    this.LabelDecimalDot.Style.Add("display", "none");
                    this.TextBoxIntegerPrecision.CssClass = "ControlElements";
                    this.TextBoxIntegerPrecision.Style.Add(HtmlTextWriterStyle.TextAlign, "left");
                    this.RegularExpressionValidatorInteger.Enabled = true;
                    this.RegularExpressionValidatorDecimal.Enabled = false;
                    break;
                case "NUMBER":
                    this.TextBoxDecimalPrecision.Style.Add("display", "block");
                    this.LabelDecimalDot.Style.Add("display", "block");
                    this.TextBoxIntegerPrecision.CssClass = "NumericInput";
                    this.TextBoxIntegerPrecision.Style.Add(HtmlTextWriterStyle.TextAlign, "right");
                    this.RegularExpressionValidatorInteger.Enabled = true;
                    this.RegularExpressionValidatorDecimal.Enabled = true;
                    break;
                case "FLOAT":
                    this.TextBoxDecimalPrecision.Style.Add("display", "block");
                    this.LabelDecimalDot.Style.Add("display", "block");
                    this.TextBoxIntegerPrecision.CssClass = "NumericInput";
                    this.TextBoxIntegerPrecision.Style.Add(HtmlTextWriterStyle.TextAlign, "right");
                    this.RegularExpressionValidatorInteger.Enabled = true;
                    this.RegularExpressionValidatorDecimal.Enabled = true;
                    break;
                case "INTEGER":
                    this.TextBoxDecimalPrecision.Style.Add("display", "block");
                    this.LabelDecimalDot.Style.Add("display", "block");
                    this.TextBoxIntegerPrecision.CssClass = "NumericInput";
                    this.TextBoxIntegerPrecision.Style.Add(HtmlTextWriterStyle.TextAlign, "right");
                    this.RegularExpressionValidatorInteger.Enabled = true;
                    this.RegularExpressionValidatorDecimal.Enabled = true;
                    break;
                case "DATE":
                case "BOOLEAN":
                    this.PrecisionDiv.Style.Add("display", "none");
                    this.RegularExpressionValidatorInteger.Enabled = false;
                    this.RegularExpressionValidatorDecimal.Enabled = false;
                    break;
                case "URL":
                    this.TextBoxDecimalPrecision.Style.Add("display", "none");
                    this.LabelDecimalDot.Style.Add("display", "none");
                    this.TextBoxIntegerPrecision.CssClass = "ControlElements";
                    this.TextBoxIntegerPrecision.Style.Add(HtmlTextWriterStyle.TextAlign, "left");
                    this.RegularExpressionValidatorInteger.Enabled = false;
                    this.RegularExpressionValidatorDecimal.Enabled = false;
                    break;
                case "PICKLISTDOMAIN":
                    this.PrecisionDiv.Style.Add("display", "none");
                    this.PickListDomainsDiv.Style.Add("display", "block");
                    this.RegularExpressionValidatorInteger.Enabled = false;
                    this.RegularExpressionValidatorDecimal.Enabled = false;
                    break;
            }
        }

        private void SaveConfigurationBO()
        {
            this.ConfigurationBO = this.ConfigurationBO.Save();
            string error = ConfigurationBO.GetSaveErrorMessage;
            if (error == string.Empty)

                ShowConfirmationMessage(Resource.ConfigurationSaved_MessagesArea);

            else
                ShowConfirmationMessage(string.Format(Resource.ConfigurationSavedWithWarnings_MessagesArea, error));
            //Below cached object is used in search engines when user updates the configuration.
             LocalCache.Add(RegistrationWebApp.Constants.ClearCache, typeof(RegistrationWebApp.Constants), RegistrationWebApp.Constants.ClearCache, LocalCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60), COECacheItemPriority.Normal);

            //this.ErrorMessagesRow.Visible = false;

            this.FillPropertyList(-1, true); 
        }

        private void FillPickListDomains()
        {
            PickListNameValueList list = PickListNameValueList.GetAllPickListDomains("REGDB");
            this.DropDownListPickListDomains.DataSource = list.KeyValueList;
            this.DropDownListPickListDomains.DataTextField = "Value";
            this.DropDownListPickListDomains.DataValueField = "Key";
            this.DropDownListPickListDomains.DataBind();
        }

        /// <summary>
        /// Method to display confirmation messages in the top of the page (MessagesAreaUC)
        /// </summary>
        /// <param name="messageToDisplay">The text to display</param>
        private void ShowConfirmationMessage(string messageToDisplay)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.MessagesAreaUserControl.AreaText = messageToDisplay;
            this.MessagesAreaUserControl.Visible = true;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region Event Handlers

        protected void ButtonEditProperty_Click(object sender, EventArgs e)
        {
            try
            {
                this.EditProperty();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void BtnAddProperty_Click(object sender, EventArgs e)
        {
            try
            {
                bool isEnable = this.AddProperty();
                this.FillPropertyList(-1, !isEnable);
                this.ClearProperty();
                this.SetDefaultSettings();
               
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ClearProperty();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void BtnDeleteProp_Click(object sender, EventArgs e)
        {
            try
            {
                this.DeleteProperty();
                this.FillPropertyList(-1, false);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void Button_ValidationRules_Click(object sender, EventArgs e)
        {
            try
            {
                SaveSelectedProperty();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void LinkButtonCustomForms_Click(object sender, EventArgs e)
        {
            try
            {
                this.ConfigurationBO = this.ConfigurationBO.Save();
                Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/CustomizeForms.aspx");
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

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

        protected void UpPropertyButton_Click(object sender, EventArgs e)
        {
            try
            {
                ChangePropertyOrder(ChangeOrder.Up);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        protected void DownPropertyButton_Click(object sender, EventArgs e)
        {
            try
            {
                ChangePropertyOrder(ChangeOrder.Down);
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
                //this.ConfigurationBO.CreateCoeFormTemplates();
                //this.ConfigurationBO = this.ConfigurationBO.Save();  Stop saving configuration on backlink click.
                Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/Default.aspx");
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }       

    }
        #endregion

    #region Enums

    public enum CourrentPageEnum
    {
        Batch = 0,
        BatchComponent = 1,
        Compound = 2,
        Structure = 3,
        PropertyList = 5
    }

    public enum ChangeOrder
    {
        Up = 0,
        Down = 1
    }

    #endregion

}
