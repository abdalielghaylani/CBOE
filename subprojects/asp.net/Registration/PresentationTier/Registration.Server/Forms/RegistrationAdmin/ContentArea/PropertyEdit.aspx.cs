using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Resources;

using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.GUIShell;

using CambridgeSoft.COE.Registration.Services.Types;

using CambridgeSoft.COE.RegistrationAdmin;
using CambridgeSoft.COE.RegistrationAdmin.Services;
using CambridgeSoft.COE.RegistrationAdmin.Services.Common;

namespace PerkinElmer.COE.Registration.Server.Forms.RegistrationAdmin.ContentArea
{
    public partial class PropertyEdit : System.Web.UI.Page
    {
        #region Variables
        RegistrationMaster _masterPage = null;
        #endregion

        #region Properties

        private ConfigurationRegistryRecord ConfigurationBO
        {
            get
            {
                if (Session["ConfigurationRegistryRecord"] == null)
                {
                    Session["ConfigurationRegistryRecord"] = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                    return (ConfigurationRegistryRecord)Session["ConfigurationRegistryRecord"];
                }
                else
                    return (ConfigurationRegistryRecord)Session["ConfigurationRegistryRecord"];
            }
            set
            {
                Session["ConfigurationRegistryRecord"] = value;
            }
        }

        private CambridgeSoft.COE.RegistrationAdmin.Services.ConfigurationProperty SelectedProperty
        {
            get
            {
                return (CambridgeSoft.COE.RegistrationAdmin.Services.ConfigurationProperty)Session["SelectedProperty"];
            }
            set
            {
                Session["SelectedProperty"] = value;
            }

        }

        #endregion

        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    SetControlsAttributes();
                    //FillPropertyTypes();
                    FillPropertyData();
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            #region Page Settings

            // To make easier to read the source code.
            if (this.Master is RegistrationMaster)
            {
                _masterPage = (RegistrationMaster)this.Master;
                _masterPage.ShowLeftPanel = false;
            }

            #endregion
            base.OnInit(e);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region Event Handler

        protected void ButtonCancelEdit_Click(object sender, EventArgs e)
        {
            try
            {
                this.CancelEdit();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        protected void ButtonSaveProperty_Click(object sender, EventArgs e)
        {  
            try
            {
                if (this.Page.IsValid)
                {
                    Session["EditDone"] = SelectedProperty.Name;
                    this.SaveEdit();
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        #endregion

        #region Methods

        private void UpdateValidationRules() 
        {
            if (this.SelectedProperty.PrecisionIsUpdate) 
            {
                ValidationRuleList valRulesToDelete = ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].ValRuleList.Clone();
                foreach (ValidationRule valRule in valRulesToDelete)
                    ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].ValRuleList.RemoveValidationRule(valRule.ID);
                SelectedProperty.AddDefaultRule();
            }
        }

        private void SaveEdit()
        {
            ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].BeginEdit();            
            if (ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].Precision  != this.TextBoxPrecision.Text)
            {
                switch (SelectedProperty.Type)
                {
                    case "NUMBER":
                        if (this.TextBoxPrecision.Text.Contains(".") || this.TextBoxPrecision.Text.Contains(","))
                            ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].Precision = this.TextBoxPrecision.Text;
                        else
                            ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].Precision = this.TextBoxPrecision.Text + ".0";
                        break;
                    case "TEXT":
                        ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].Precision = this.TextBoxPrecision.Text;
                        break;
                }
            }            
            this.UpdateValidationRules();
            if(SelectedProperty.Type == "NUMBER")
                ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].Precision = RegAdminUtils.ConvertPrecision(ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].Precision,true);
            ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].ValRuleList = SelectedProperty.ValRuleList;
            ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].ApplyEdit();
            if (ConfigurationBO.SelectedPropertyList == ConfigurationRegistryRecord.PropertyListType.PropertyList)
                Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/RootPropertyList.aspx");
            else
                Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/" + ConfigurationBO.SelectedPropertyList.ToString() + ".aspx");
        }

        private void CancelEdit()
        {
            SelectedProperty.CancelEdit();
            if (ConfigurationBO.SelectedPropertyList == ConfigurationRegistryRecord.PropertyListType.PropertyList)
                Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/RootPropertyList.aspx");
            else
                Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/" + ConfigurationBO.SelectedPropertyList.ToString() + ".aspx");
        }

        internal void SetControlsAttributes()
        {       
            this.LabelName.Text = Resource.Name_Label_Text;
            this.LabelType.Text = Resource.Type_Label_Text;            
            this.TextBoxName.Enabled = false;
            this.CheckPrecisionChangeValidator.ErrorMessage = "You cannot reduce the precision or scale of a property. Please delete the property or create a new property";
            this.RegularExpressionValidatorPrecision.ControlToValidate = this.TextBoxPrecision.ID;
            if (SelectedProperty.Type == "NUMBER")
            {
                this.RegularExpressionValidatorPrecision.ValidationExpression = "[-+]?[0-9]*\\.?[0-9]*";
                this.RegularExpressionValidatorPrecision.ErrorMessage = "This field can be only a number";
                this.LabelPrecision.Text = "Precision:";
            }
            else
            {
                this.RegularExpressionValidatorPrecision.ValidationExpression = "^\\d+$";
                this.RegularExpressionValidatorPrecision.ErrorMessage = "This field can be only an integer number";
                this.LabelPrecision.Text = "Length:";
            }
            this.ButtonCancelEdit.Text = Resource.Cancel_Button_Text;
            this.ButtonSaveProperty.Text = Resource.Save_Button_Text;
            this.Page.Title = Resource.Brand + " - " + Resource.RegAdminHome_Page_Title + " - " + Resource.EditProperty_PageTitle;
        }

        private void FillPropertyData()
        {
            this.TextBoxName.Text = ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].FriendlyName;
            if (SelectedProperty.Type == "NUMBER")
                this.TextBoxPrecision.Text = RegAdminUtils.ConvertPrecision(ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].Precision, false);
            else
                this.TextBoxPrecision.Text = ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].Precision;
            this.TextBoxType.Text = ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].Type.ToUpper();
        }
        #endregion


        public void CheckPrecisionChange(Object sender, ServerValidateEventArgs args){
            string currentPrecision = string.Empty;
            //first find out the current precision
            if (SelectedProperty.Type == "NUMBER")
            {
                 currentPrecision = RegAdminUtils.ConvertPrecision(ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].Precision, false);
            }
            else
            {
                 currentPrecision = ConfigurationBO.GetSelectedPropertyList[SelectedProperty.Name].Precision;
            }
             
           string newPrecision = this.TextBoxPrecision.Text;
           if (RegAdminUtils.CanEditPrecision(currentPrecision, newPrecision))
            {
                //edit is allowed
                args.IsValid = true;
            }
            else
            {
                //edit not allowed
                args.IsValid = false;
                

            }

        }
    }
}
