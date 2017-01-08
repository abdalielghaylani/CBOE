using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.RegistrationAdmin.Services;
using PerkinElmer.CBOE.Registration.Client.Forms.RegistrationAdmin.UserControls;
using System.Text;
using System.IO;
using System.Xml;
using System.Net;
using Resources;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Services.Types;

namespace PerkinElmer.CBOE.Registration.Client.Forms.RegistrationAdmin.ContentArea
{
    public partial class Default : GUIShellPage
    {
        #region Variables

        RegistrationMaster _masterPage = null;
        ConfigurationRegistryRecord _cRR;


        #endregion

        #region Properties

        private ConfigurationRegistryRecord ConfigurationRegistryRecord
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

        #endregion

        #region PageLoad

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.InitSessionVars();

                if (!Page.IsPostBack)
                {
                    _cRR = ConfigurationRegistryRecord;
                    SetControlsAttributtes();
                    ShowHideLinks();
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

        protected override void OnPreRender(EventArgs e)
        {
            if (!this.Page.ClientScript.IsStartupScriptRegistered(typeof(Default), "HelpIconTextVisibility"))
            {
                string script = @"
                    function HelpIconTextVisibility(active)
                    {                       
                        divHelpLink = document.getElementById('" + this.DivHelpLink.ClientID + @"');
                        if(active == true)
                            divHelpLink.style.display = 'block';
                        else
                            divHelpLink.style.display = 'none';                                            
                        
                }";

                this.Page.ClientScript.RegisterStartupScript(typeof(Default), "HelpIconTextVisibility", script, true);
            }
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            base.OnPreRenderComplete(e);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            if (RegUtilities.OverrideDisplayErrorMessage)
                _masterPage.DisplayErrorMessage(RegUtilities.ErrorMessage, false);
            RegUtilities.ErrorMessage = string.Empty;
            RegUtilities.OverrideDisplayErrorMessage = false;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        }

        #endregion

        #region Methods

        protected override void  SetControlsAttributtes()
        {
            this._masterPage.SetDefaultAction(this.LinkButtonSetUpPropList.UniqueID);
            this.Page.Title =Resource.Brand + " - " + Resource.RegAdminHome_Page_Title;
            this.LinkButtonSetUpPropList.Text = Resource.SetUpPropList_LinkButton_Text;
            this.CustomizeFormsLinkButton.Text = Resource.CustomizeForms_LinkButton_Text;
            this.LinkButtonFormElEdit.Text = Resource.FomrElementsEdit_LinkButton_Text;
            this.LinkButtonAddInsConf.Text = Resource.MangeAddIn_LinkButton_Text;
            this.LabelRegAdminDescription.Text = Resource.RegAdminDescription_Label_Text;
            this.LabelRegAdminTitle.Text = Resource.RegAdminTitle_Label_Text;
            this.LinkButtonImpExpPropList.Text = Resource.ImpExpPropList_LinkButton_Text;
            this.TableEditorLink.Text = Resource.TableEditor_Link_Text;
            this.LabelPropListAddInTitle.Text = Resource.PropListAddInsConfTitle_Label_Text;         
            this.LabelHideInfo.Text = Resource.RegHomeHideInfo_Label_Text;
            this.ImageCSoftLogo.ImageUrl = RegUtilities.ThemesCommonImagesPath + "ClientLogo.jpg";
            this.ImageImpExpIcon.ImageUrl = RegUtilities.ThemesCommonImagesPath + "Export.gif";
            this.ImagePropertyList.ImageUrl = RegUtilities.ThemesCommonImagesPath + "SetUpProperties.gif";
            this.TableEditorImage.ImageUrl = RegUtilities.ThemesCommonImagesPath + "TableEditor.gif";
            this.ConfigSettingsLinkButton.Text = Resource.ConfigSettings_LinkButton_Text;
            this._cRR.SelectedPropertyList = ConfigurationRegistryRecord.PropertyListType.None;
        }

        private void InitSessionVars()
        {
            ConfigurationRegistryRecord = null;
            // This is to ensure that the user is not forced to go to import a config. This assures there is a safe customization imported. Do not remove.
            if (Session[PerkinElmer.CBOE.Registration.Client.Constants.MultiCompoundObject_Session] == null)
                Session[PerkinElmer.CBOE.Registration.Client.Constants.MultiCompoundObject_Session] = Session["NewRegistryRecord"] != null ? Session["NewRegistryRecord"] : RegUtilities.GetNewRegistryRecord();
            // Set NewRegistryRecord session to null [User may change the configuration ] to load new settings.
            Session["NewRegistryRecord"] = null;
        }

        /// <summary>
        /// Show or Hide Reg Admin task links based on user privilege
        /// </summary>
        protected  void ShowHideLinks()
        {
            //Show and hide links based on presence of user privilege
            bool hasOneManageSysPriv = false;
            Panel importConfigLink = (Panel)this.Master.FindControl(GUIShellTypes.MainFormID).FindControl("ContentPlaceHolder").FindControl("ImportExportItem");
            if (RegAdminPrivilegeValidator.HasPrivilege(RegAdminPrivilegeValidator.RegAdminTasks.IMPORT_CONFIG))
            {
                importConfigLink.Visible = true;

            }
            else
            {
                importConfigLink.Visible = false;
            }

            Panel tableEditorControl = (Panel)this.Master.FindControl(GUIShellTypes.MainFormID).FindControl("ContentPlaceHolder").FindControl("TableEditorItem");
            if (RegAdminPrivilegeValidator.HasPrivilege(RegAdminPrivilegeValidator.RegAdminTasks.MANAGE_TABLES))
            {
                tableEditorControl.Visible = true;

            }
            else
            {
                tableEditorControl.Visible = false;

            }

            LinkButton propertyListControl = (LinkButton)this.Master.FindControl(GUIShellTypes.MainFormID).FindControl("ContentPlaceHolder").FindControl("LinkButtonSetUpPropList");
            if (RegAdminPrivilegeValidator.HasPrivilege(RegAdminPrivilegeValidator.RegAdminTasks.MANAGE_PROPERTIES))
            {
                propertyListControl.Visible = true;
                hasOneManageSysPriv = true;

            }
            else
            {
                propertyListControl.Visible = false;
            }

            LinkButton customizeFormsControl = (LinkButton)this.Master.FindControl(GUIShellTypes.MainFormID).FindControl("ContentPlaceHolder").FindControl("CustomizeFormsLinkButton");
            if (RegAdminPrivilegeValidator.HasPrivilege(RegAdminPrivilegeValidator.RegAdminTasks.CUSTOMIZE_FORMS))
            {
                customizeFormsControl.Visible = true;
                hasOneManageSysPriv = true;

            }
            else
            {
                customizeFormsControl.Visible = false;
            }

            LinkButton manageAddInsControl = (LinkButton)this.Master.FindControl(GUIShellTypes.MainFormID).FindControl("ContentPlaceHolder").FindControl("LinkButtonAddInsConf");
            if (RegAdminPrivilegeValidator.HasPrivilege(RegAdminPrivilegeValidator.RegAdminTasks.MANAGE_ADDINS))
            {
                manageAddInsControl.Visible = true;
                hasOneManageSysPriv = true;

            }
            else
            {
                manageAddInsControl.Visible = false;
            }

            LinkButton editFormsControl = (LinkButton)this.Master.FindControl(GUIShellTypes.MainFormID).FindControl("ContentPlaceHolder").FindControl("LinkButtonFormElEdit");
            if (RegAdminPrivilegeValidator.HasPrivilege(RegAdminPrivilegeValidator.RegAdminTasks.EDIT_FORM_XML))
            {
                editFormsControl.Visible = true;
                hasOneManageSysPriv = true;

            }
            else
            {
                editFormsControl.Visible = false;
            }


            LinkButton manageSystemSettingsControl = (LinkButton)this.Master.FindControl(GUIShellTypes.MainFormID).FindControl("ContentPlaceHolder").FindControl("ConfigSettingsLinkButton");
            if (RegAdminPrivilegeValidator.HasPrivilege(RegAdminPrivilegeValidator.RegAdminTasks.MANAGE_SYSTEM_SETTINGS))
            {
                manageSystemSettingsControl.Visible = true;
                hasOneManageSysPriv = true;

            }
            else
            {
                manageSystemSettingsControl.Visible = false;
            }

            //hide manage system behaviors graphic and desciption since user doesn't have any privileges
            Panel manageSystemBehaviorsControl = (Panel)this.Master.FindControl(GUIShellTypes.MainFormID).FindControl("ContentPlaceHolder").FindControl("ManageSystemBehaviors");
            if (hasOneManageSysPriv == false)
            {
                manageSystemBehaviorsControl.Visible = false;
            }
            else
            {
                manageSystemBehaviorsControl.Visible = true;
            }
          

        }

        private void RevertConfigurationBO()
        {
            if (ConfigurationRegistryRecord.IsDirty)
                this.ConfigurationRegistryRecord = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
        }

        private void SaveConfigurationBO()
        {
            //this.ConfigurationRegistryRecord.CreateCoeFormTemplates();
            ConfigurationRegistryRecord = ConfigurationRegistryRecord.Save();
        }

        //private void ImportConfigurations()
        //{
        //    try
        //    {
        //        XmlDocument xmlDoc = new XmlDocument();
        //        xmlDoc.LoadXml(System.Text.ASCIIEncoding.ASCII.GetString(FileUploadImport.FileBytes));
        //        XmlNode customForms = xmlDoc.DocumentElement.RemoveChild(xmlDoc.DocumentElement.ChildNodes[xmlDoc.DocumentElement.ChildNodes.Count - 1]);
        //        ImportCustomProperties(xmlDoc.OuterXml);
        //        foreach (XmlNode node in customForms.ChildNodes)
        //        {
        //            this.ImportForm(node);
        //        }
        //    }
        //    catch
        //    {
        //        this.Lbl_Error.Text = "The imported xml file is not correct";
        //    }
        //}

        private void ImportForm(XmlNode formGroup) {
            this.ConfigurationRegistryRecord.ImportForm(formGroup.OuterXml);
        }


        private string ExportCustomProperties()
        {
            return this.ConfigurationRegistryRecord.ExportCustomizedProperties();
        }    

        private void SetUpPropertyList()
        {
            ConfigurationRegistryRecord.SelectedPropertyList = ConfigurationRegistryRecord.PropertyListType.PropertyList;
            Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/RootPropertyList.aspx");
        }

        private void SetUpAddIns() 
        {
            ConfigurationRegistryRecord.SelectedPropertyList = ConfigurationRegistryRecord.PropertyListType.AddIns;

            Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/AddIns.aspx");
        }

        private void CustomizeFormElements() 
        {
            Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/EditFormsXml.aspx");
        }

        #endregion

        #region Event Handlers

        
        protected void Btn_Save_Click(object sender, EventArgs e)
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

        protected void Btn_Revert_Click(object sender, EventArgs e)
        {
            try
            {
                this.RevertConfigurationBO();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }

        protected void LinkButtonSetUpPropList_Click(object sender, EventArgs e) 
        {
            try
            {
                SetUpPropertyList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }

        protected void LinkButtonAddInsConf_Click(object sender, EventArgs e) 
        {
            try
            {
                SetUpAddIns();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }

        protected void LinkButtonFormElEdit_Click(object sender, EventArgs e) 
        {
            try
            {
                CustomizeFormElements();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }

        protected void LinkButtonImpExpPropList_Click(object sender, EventArgs e) 
        {
            try
            {
                Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/ImportExportCustom.aspx");
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        protected void ConfigSettingsLinkButton_Click(object sender, EventArgs e)
        {
            try
            {
                Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/ConfigSettings.aspx", false);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
	
        }

        protected void CustomizeFormsLinkButton_Click(object sender, EventArgs e)
        {
            try
            {
                Server.Transfer("~/Forms/RegistrationAdmin/ContentArea/CustomizeForms.aspx", false);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void TableEditorLink_Click(object sender, EventArgs e)
        {
            try
            {
                Server.Transfer("~/Forms/TableEditor/ContentArea/TableEditor.aspx", false);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

       #endregion

    }
}
