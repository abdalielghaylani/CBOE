using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;

using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEGenericObjectStorageService;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Registration.Services.Types;

using Csla.Validation;
using Csla;
using Infragistics.WebUI.UltraWebListbar;
using Infragistics.WebUI.UltraWebNavigator;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using RegistrationWebApp.Forms.Master;
using Resources;
using CambridgeSoft.COE.Framework.Common.Validation;
using RegistrationWebApp.Code;
using CambridgeSoft.COE.Registration;

namespace RegistrationWebApp.Forms.SubmitRecord.ContentArea
{
    public partial class SubmitMixturePage : GUIShellPage
    {
        #region GUIShell Variables

        COENavigationPane COENavigationPaneObject = null;
        RegistrationMaster _masterPage = null;
        private string _controlToListen = String.Empty;
        private string _groupToListen = String.Empty;
        private RegistryTypes _currentRegistryType;

        private const int minComponents = 1;        
        #endregion

        #region Page Variables

        private enum SubForms
        {
            MixtureResumeForm = 0,
            ComponentForm = 1,
            SubmittedComponentForm = 2,
            BatchComponentForm = 3,
            BatchForm = 4,
            BatchComponentFragmentsForm = 5,
            BatchComponentListFragmentsForm = 6,
            RegistryCustomProperties = 1000,
            CompoundCustomProperties = 1001,
            BatchCustomProperties = 1002,
            BatchComponentCustomProperties = 1003,
            StructureCustomProperties = 1004
        }

        private enum PageState
        {
            None,
            EditRecord,
            AddSingleCompound,
            EditSingleCompound,
            AddComponent,
            EditComponent,
            ViewComponent,
            AddBatch,
            EditBatch,
            End,
            EndComponent,
            ViewBatch,
            DisplayUserPreference
        }

        private enum MessageBoxMode
        {
            MixtureDuplicateMessageBox,
            ComponentDuplicateMessageBox,
            MessageBox,
        }

        protected enum RegistryTypes
        {
            Mixture,
            Component,
            Both,
        }

        private enum MixtureRule
        {
            projectList = 1, ComponentList = 2, batchList = 1
        }

        COEFormGroup _mixtureCOEFormGroup;
        private bool _lockRegistryRecordRequest = false;

        PageState CurrentPageState
        {
            get
            {
                return ViewState["CurrentPageState"] == null ? PageState.None : (PageState)ViewState["CurrentPageState"];
            }
            set
            {
                ViewState["CurrentPageState"] = value;
            }
        }

        public bool IsPreRegistration
        {
            get
            {
                return RegUtilities.GetEnableSubmissionDuplicateCheck();
            }

        }

        public bool AllowUnregisteredComponents
        {
            get
            {
                return RegUtilities.GetAllowUnregisteredComponents();
            }

        }

        private bool LockRegistryRecordRequest
        {
            get
            {
                return _lockRegistryRecordRequest;
            }
            set
            {
                _lockRegistryRecordRequest = value;
            }
        }



        RegistryTypes CurrentRegistryType
        {
            get
            {
                if ((Session["CurrentRegistryType"] == null) || (String.IsNullOrEmpty(Session["CurrentRegistryType"].ToString())))
                {
                    if (Request.QueryString["RegistryType"] != null)
                    {
                        switch (Request.QueryString["RegistryType"].ToString().ToLower())
                        {
                            case "component":
                                Session["CurrentRegistryType"] = RegistryTypes.Component;
                                break;
                            //Fix For CSBR 153190
                            //we don't have switch case:both(RegistryTypes.Both)
                            // here added the Switch case code

                            case "both":
                                if (AllowUnregisteredComponents)
                                {
                                    Session["CurrentRegistryType"] = RegistryTypes.Both;
                                }
                                break;

                            case "mixture":
                                if (AllowUnregisteredComponents)
                                {
                                    Session["CurrentRegistryType"] = RegistryTypes.Both;
                                }
                                else
                                {
                                    Session["CurrentRegistryType"] = RegistryTypes.Mixture;
                                }
                                IsMixture = false;
                                break;
                        }
                    }
                    else
                    {
                        Session["CurrentRegistryType"] = RegistryTypes.Component;
                    }
                }


                return (RegistryTypes)Session["CurrentRegistryType"];

            }
            set
            {

                Session["CurrentRegistryType"] = value;
            }
        }



        RegistryRecord RegistryRecord
        {
            get
            {
                if (Session[Constants.MultiCompoundObject_Session] == null && !LockRegistryRecordRequest)
                {
                    Session[Constants.MultiCompoundObject_Session] = RegUtilities.GetNewRegistryRecord();

                    // Stop requests for registry record once the return value is null , This ensures that Configuration is dirty , or not valid.
                    if (Session[Constants.MultiCompoundObject_Session] == null)
                        LockRegistryRecordRequest = true;
                    else
                        LockRegistryRecordRequest = false;
                }
                return (RegistryRecord)Session[Constants.MultiCompoundObject_Session];
            }
            set
            {
                if (Session[Constants.MultiCompoundObject_Session] != null && !RegistryRecord.ReferenceEquals(value, Session[Constants.MultiCompoundObject_Session]))
                    ((IDisposable)Session[Constants.MultiCompoundObject_Session]).Dispose();

                Session[Constants.MultiCompoundObject_Session] = value;
                LockRegistryRecordRequest = false;
            }
        }
        private bool _restoreSavedMixture = false;
        #endregion

        #region Page Properties
        private int LastNodeEdited
        {
            get
            {
                if (ViewState[Constants.LastNodeEditted_ViewState] != null)
                    return (int)ViewState[Constants.LastNodeEditted_ViewState];
                else
                    return this.RegistryRecord.ComponentList.Count - 1;
            }
            set
            {
                ViewState[Constants.LastNodeEditted_ViewState] = value;
            }
        }

        /// <summary>
        /// Check if the current registry is a multicompound (Mixture)
        /// </summary>
        private bool IsAMixture
        {
            get
            {
                return true;
            }
        }


        /// <summary>
        /// Check if the current user completes creating the mixture in GUI following mixture rules [ENUM : MixtureRule]
        /// </summary>
        private bool IsMixture
        {
            get
            {
                if (Session["ISMIXTURE"] == null)
                    Session["ISMIXTURE"] = false;
                return Convert.ToBoolean(Session["ISMIXTURE"]);
            }
            set
            {
                Session["ISMIXTURE"] = value;
            }
        }

        /// <summary>
        /// Check if the current registry is a already submitted registry.
        /// </summary>
        private bool AlreadySubmitted
        {
            get { return ViewState["AlreadySubmited"] != null ? (bool)ViewState["AlreadySubmited"] : false; }
            set { ViewState["AlreadySubmited"] = value; }
        }

        /// <summary>
        /// Represent the centralized display culture configuration
        /// </summary>
        private string DisplayCulture
        {
            get
            {
                if (Session["DisplayCulture"] == null)
                    Session["DisplayCulture"] = ConfigurationUtilities.GetApplicationData(COEAppName.Get().ToString()).DisplayCulture;
                return (string)Session["DisplayCulture"];
            }
        }
        
        #endregion

        #region GUIShell Properties
        /// <summary>
        /// These are the controls which I can be interested in the Control(MyCustomizedControl)
        /// inside the Accordion Group.
        /// </summary>
        private enum ControlsToTakeCare
        {
            CompoundIDHidden,
            BatchIDHidden,
            ShowButtonsHidden,
            MessageLabel,
            DoneButton,
            UltraWebTreeControl
        }



        /// <summary>
        /// Those are the events from the control inside the accordion that I'm interested.
        /// </summary>
        private enum EventsToListen
        {
            Done,
            NodeClicked
        }

        /// <summary>
        /// This is the controlID that you are interested in listen its public events
        /// </summary>
        /// <remarks>This control is a ICOENavigationControl type</remarks>
        protected string AccordionControlToListen
        {
            get { return this._controlToListen; }
            set { this._controlToListen = value; }
        }

        /// <summary>
        /// This is the groupID in the accordion that I'm interested.
        /// </summary>
        /// <remarks>The returned string is the Property Group Key</remarks>
        protected string AccordionGroupToListen
        {
            get { return this._groupToListen; }
            set { this._groupToListen = value; }
        }

        #endregion

        #region Events Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            try
            {
                //this.ClearErrorMessage();
                if (!Page.IsPostBack)
                {

                    this.LoadControlInAccordionPanel();
                    this.SetControlsAttributtes();
                    this.ShowTreeButtons(false);

                    if (!this.RestoreSavedObject() && !this.ReplaceExistingComponent(Request[RegistrationWebApp.Constants.RegisteredCompoundId_UrlParameter], Request[RegistrationWebApp.Constants.RegisteredRegId_UrlParameter]) && !this.RestoreSessionObject())
                    {
                        //you will not get into this sectio of the code if the above if statment finds that there was a search for components
                        this.RegistryRecord = null;
                        Session["CurrentRegistryType"] = null;
                        this.SetPageMode(PageState.AddSingleCompound, String.Empty);

                    }
                    if (CurrentRegistryType == RegistryTypes.Mixture && (this.RegistryRecord.ComponentList.Count == 1 && this.RegistryRecord.ComponentList[0].Compound.RegNumber.RegNum == string.Empty))
                    {
                        this.DisplayCompoundIntoTree(this.RegistryRecord, NodeKeywords.Root.ToString());
                    }
                    else if (_restoreSavedMixture)
                    {
                        this.DisplayCompoundIntoTree(this.RegistryRecord, null);
                        this.DisplayRecord();
                        this.SetPageMode(PageState.EditRecord, string.Empty);
                    }
                    else
                    {
                        this.DisplayCompoundIntoTree(this.RegistryRecord);
                    }//Fix for CSBR 166611- Duplicate Resolution does not display exact Duplicates.
                    if ((Session[Constants.DuplicateIdsList_Session] != null) || (Session[Constants.DuplicateMultiCompounds_Session] != null))
                    {
                        Session.Remove(Constants.DuplicateIdsList_Session);
                        Session.Remove(Constants.DuplicateMultiCompounds_Session);
                    }
                    if (Request[Constants.ShowUserPreference_UrlParameter] !=null)
                    {
                        if (Request[Constants.ShowUserPreference_UrlParameter].ToString().ToUpper()== "YES")
                            RestoreUserPreference();
                    }
                    RegUtilities.ClearNavigationPaths();
                    RegUtilities.UserNavigationPaths = Resource.SubmitComponent_URL;
                }
                else
                {
                    this.HideConfirmationMessage();
                }

                this.SubscribeToEventsInAccordionControl();
                this.SetPageTitle(this.CurrentRegistryType);
                this.SetAccordianTitle(this.CurrentRegistryType);
                _masterPage.MakeCtrlShowProgressModal(this.SubmitButton.ClientID, "Submitting...", string.Empty);
                _masterPage.MakeCtrlShowProgressModal(this.RegisterButton.ClientID, "Registering...", string.Empty);
                _masterPage.MakeCtrlShowProgressModal(this.SavePreferenceButton.ClientID, "Saving...", string.Empty, false);
                this.DefineMixtureImageMenuButton.MenuItemList[2].OnClientClick = @"if(RemoveComponent('" + this.ActionToDoHiddenField.ClientID + "') == false) return false;";
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected override void OnPreInit(EventArgs e)
        {
            ((RegistrationMaster)Master).FormGroupIdKey = "SubmitRegistryFormGroupId";

            #region Page Settings
            // To make easier to read the code.
            _masterPage = (RegistrationMaster)this.Master;
            //Set the accordion key where is the customized control inside.

            this.AccordionGroupToListen = "SubmitMixture";
            //Set the cotrolID inside accordion group.
            this.AccordionControlToListen = "RegistryRecordNavigation";
            #endregion

            this.LoadFormGenerators();

            base.OnPreInit(e);

        }

        protected override void OnInit(EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                base.OnInit(e);
                this.DefineMixtureImageMenuButton.Text = Resource.DefineMixtureImageMenuButton_text;
                this.DefineMixtureImageMenuButton.MenuItemList[0].Text = Resource.AddComponent_Button_Text;
                this.DefineMixtureImageMenuButton.MenuItemList[1].Text = Resource.Select_Button_Text;
                this.DefineMixtureImageMenuButton.MenuItemList[2].Text = Resource.DeleteComponent_Button_Text;
                this.DefineMixtureImageMenuButton.MenuItemList[3].Text = Resource.Continue_Label_Text;
                this.DefineMixtureImageMenuButton.ValidateAddComponent = true;

                this.SubmissionTemplateImageMenuButton.MenuItemList[0].Text = Resource.SaveTemplate_Button_Text;
                this.SubmissionTemplateImageMenuButton.MenuItemList[1].Text = Resource.Load_Button_Text;

                this.SubmissionTemplateImageMenuButton.Command += new CommandEventHandler(SubmissionTemplate_Command);
                this.DefineMixtureImageMenuButton.Command += new CommandEventHandler(DefineMixtureImageMenuButton_Command);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Disables the Entire header User Control.
        /// </summary>
        public override void DisableHeader()
        {
            Control headerContainer = this.Master.FindControl("HeaderUserControl");
            if (headerContainer != null)
                headerContainer.Visible = false;
        }
        /// <summary>
        /// Method to customize thechanges required for User Preference page
        /// </summary>
        protected void CustomizeUserPreference()
        {
            
            if (this.Master != null)
            {
                RegistrationMaster regmaster = (RegistrationMaster)this.Master;
                regmaster.CustomizeUserPreference();
            }
            HideCustomProperties((int)SubForms.ComponentForm, "Structure|StructureID");
            string hideBatchProperties = RegUtilities.GetConfigSetting(RegUtilities.Groups.RegAdmin, "UserPreferenceHideBatchProperties");
            HideCustomProperties((int)SubForms.BatchCustomProperties, hideBatchProperties);
            
        }

        /// <summary>
        /// This method is added to temporarly hide batch level properites for User preference page,
        /// this mechanism need to revist while building the Admin Config screen for User perference
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="propertyList"></param>
        protected void HideCustomProperties(int formId, string propertyList)
        {
            Dictionary<string, string> dictPropertList = new Dictionary<string, string>();
            string[] propertyArray = propertyList.Split("|".ToCharArray());
            foreach (string sProperty in propertyArray)
            {
                if (!dictPropertList.ContainsKey(sProperty))
                    dictPropertList.Add(sProperty, sProperty);
            }
            //Coverity Fix :CIE 20050
            FormGroup.Display theDisplayObject = _mixtureCOEFormGroup.DetailFormCollection[0];
            if (theDisplayObject != null)
            {
                foreach (FormGroup.Form coeSubform in theDisplayObject.Forms)
                {
                    if ( coeSubform.Id == formId && coeSubform.EditMode != null)
                    {
                        COEFormGenerator formgeneratorControl = ((COEFormGenerator)_mixtureCOEFormGroup.FindControl(formId));
                        foreach (FormGroup.FormElement subFormElement in coeSubform.EditMode)
                        {
                            if (dictPropertList.ContainsKey(subFormElement.Name) && formgeneratorControl != null)
                            {
                                Control subFormElementControl = formgeneratorControl.FindControl(subFormElement.Id);//Hiding the control rather than formelement because the coeform will be cached.
                                if (subFormElementControl != null)
                                {
                                    subFormElementControl.Visible = false;
                                    subFormElementControl.Parent.Visible = false; //Hiding the container div too.
                                }
                            }
                        }
                        break;
                    }

                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (RegUtilities.GetApprovalsEnabled())
            {
                this.RegisterButton.Visible = false;
            }
            base.OnPreRender(e);
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                string _changeInLinesWhenRemoving = "ChangeInLinesWhenRemoving";
                //Script to clear ('null') the output Hidden var of the ChemDraw control in case its the first component in a mixture (0)
                if (!Page.ClientScript.IsOnSubmitStatementRegistered(_changeInLinesWhenRemoving))
                {
                    Page.ClientScript.RegisterOnSubmitStatement(typeof(string), _changeInLinesWhenRemoving,
                        @"
                        if (document.getElementById('" + this.ActionToDoHiddenField.ClientID + @"') != null &&
                            document.getElementById('" + this.ActionToDoHiddenField.ClientID + @"').value == 'REMOVE')
                      {
                        ClearOutputValue(document.getElementById('" + this.ComponentIndexHiddenField.ClientID + @"').value);    
                        document.getElementById('" + this.ActionToDoHiddenField.ClientID + @"').value = 'null';
                      }");
                }
                //Fix for 147996
                //In this fix we Clear the data Whaen clear button click in Submit mixture flow.
                //loading the the relevent Js file to clear the corrosponding controls data like chemdraw structure/grid dada.

                /* CSBR-160682 : Hitting clear in mixture new Batch page is not clearing the data added and showing an error 
                 * Fix is to check whether the current registry type  is both */

                if (((CurrentRegistryType == RegistryTypes.Mixture) || (CurrentRegistryType == RegistryTypes.Both)) && this.ClearFormButton.Visible)
                {

                    ClearFormButton.OnClientClick = ClearFormButton.OnClientClick.Replace("coe_clearForm()", "clearForm(document.forms[0].id)");

                    if (!Page.ClientScript.IsClientScriptIncludeRegistered("chemdraw"))
                    {

                        Page.ClientScript.RegisterClientScriptInclude("chemdraw", "/COECommonResources/ChemDraw/chemdraw.js");
                    }

                    if (!Page.ClientScript.IsClientScriptIncludeRegistered("ChemOfficeEnterprise"))
                    {

                        Page.ClientScript.RegisterClientScriptInclude("ChemOfficeEnterprise", "/COECommonResources/ChemDraw/ChemOfficeEnterprise.js");
                    }

                }


                //according rls status is going to add the css classes to make projects grids look like required.
                this.Header.Controls.Add(RegUtilities.GetRLSCSSClass(this.RegistryRecord.RLSStatus));
                if (this.ControlsToDisable != null)
                {
                    CambridgeSoft.COE.Framework.COEPageControlSettingsService.ControlList disableControlList = this.ControlsToDisable.GetByPageID(this.Page.ToString());
                    foreach (CambridgeSoft.COE.Framework.COEPageControlSettingsService.Control control in disableControlList)
                    {
                        foreach (object curValidator in Page.Validators)
                        {
                            if (curValidator is RequiredFieldValidator && ((RequiredFieldValidator)curValidator).ControlToValidate.IndexOf(control.ID, 0) == 0)
                            {
                                ((RequiredFieldValidator)curValidator).Enabled = false;
                                ((RequiredFieldValidator)curValidator).EnableClientScript = false;
                                ((RequiredFieldValidator)curValidator).IsValid = true;
                                break;
                            }
                        }
                    }
                }
                if (this.CurrentPageState == PageState.DisplayUserPreference)
                    CustomizeUserPreference();
                this.RenderJSArrayWithCliendIds();
                base.OnPreRenderComplete(e);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            finally
            {
                if (RegUtilities.OverrideDisplayErrorMessage)
                    _masterPage.DisplayErrorMessage(RegUtilities.ErrorMessage, false);
                RegUtilities.ErrorMessage = string.Empty;
                RegUtilities.OverrideDisplayErrorMessage = false;
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);            
            try
            {
                if (Page.IsValid)
                {
                    this.SubmitMixture();
                }
            }
            catch (Exception exception)
            {
                if (exception.GetBaseException() is ValidationException)
                {
                    List<BrokenRuleDescription> brokenRuleDescriptionList = RegistryRecord.GetBrokenRulesDescription();

                    if (brokenRuleDescriptionList.Count > 0)
                        DisplayBrokenRules(((ValidationException)exception.GetBaseException()), brokenRuleDescriptionList);
                    else
                    {
                        SetPageMode(PageState.EditSingleCompound, null);
                        _masterPage.DisplayErrorMessage(exception, true);
                    }
                }
                else
                {
                    COEExceptionDispatcher.HandleUIException(exception);
                    this.SetPageMode(PageState.EditBatch, string.Empty);
                    //_masterPage.DisplayErrorMessage(exception, true);
                    _masterPage.DisplayErrorMessage(exception, true);
                }
            }
            finally
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
        }

        protected void RegisterButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.RegisterMixture();
            }
            catch (Exception exception)
            {
                if (exception.GetBaseException() is ValidationException)
                {
                    List<BrokenRuleDescription> brokenRuleDescriptionList = RegistryRecord.GetBrokenRulesDescription();

                    if (brokenRuleDescriptionList.Count > 0)
                        DisplayBrokenRules(brokenRuleDescriptionList);
                    else
                    {
                        SetPageMode(PageState.EditSingleCompound, null);
                        _masterPage.DisplayErrorMessage(exception, true);
                    }
                }
                else
                {
                    COEExceptionDispatcher.HandleUIException(exception);
                    this.SetPageMode(PageState.EditBatch, string.Empty);
                    //_masterPage.DisplayErrorMessage(exception, true);
                    _masterPage.DisplayErrorMessage(exception, true);
                }
            }
            finally
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (CurrentPageState != PageState.End && CurrentPageState != PageState.EndComponent)
                    _mixtureCOEFormGroup.Update();
                this.DisplayComponent(LastNodeEdited);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        void SubmissionTemplate_Command(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "SaveTemplate":
                    RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                    this.SaveMixture();
                    RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
                    break;
                case "LoadTemplate":
                    RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                    Server.Transfer(string.Format("{0}?{1}={2}&{3}={4}", Resource.LoadRegistryForm_Page_URL, RegistrationWebApp.Constants.FormGroup_UrlParameter, (int)FormGroups.MultiCompound, RegistrationWebApp.Constants.RegistryTypeParameter, CurrentRegistryType), false);
                    RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
                    break;
            }
        }


        void DefineMixtureImageMenuButton_Command(object sender, CommandEventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            switch (e.CommandName)
            {
                case "AddComponent":
                    try
                    {
                        this.AddComponent();
                    }
                    catch (Exception exception)
                    {
                        _masterPage.DisplayErrorMessage(exception, true);
                    }
                    break;
                case "SearchComponent":
                    try
                    {
                        this.SearchCompound();
                    }
                    catch (Exception exception)
                    {
                        _masterPage.DisplayErrorMessage(exception, true);
                    }
                    break;
                case "DeleteComponent":
                    try
                    {
                        this.DeleteComponent();
                    }
                    catch (Exception exception)
                    {
                        _masterPage.DisplayErrorMessage(exception, true);
                    }
                    break;
                case "ContinueToBatch":
                    _mixtureCOEFormGroup.Update();
                    this.DisplayRecord();
                    this.DisplayBatch(0);
                    break;
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }



        protected void SaveButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.SaveMixture();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                _mixtureCOEFormGroup.Update();
                this.DisplayBatch(0);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void LoadButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                Server.Transfer(string.Format("{0}?{1}={2}&{3}={4}", Resource.LoadRegistryForm_Page_URL, RegistrationWebApp.Constants.FormGroup_UrlParameter, (int)FormGroups.MultiCompound, RegistrationWebApp.Constants.RegistryTypeParameter, CurrentRegistryType), false);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void ConfirmationMessageBox_BtnNoClicked(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                Server.Transfer(string.Format("{0}?{1}={2}&{3}={4}", Resource.SaveRegistryForm_Page_URL, RegistrationWebApp.Constants.FormGroup_UrlParameter, (int)FormGroups.MultiCompound, RegistrationWebApp.Constants.RegistryTypeParameter, CurrentRegistryType), false);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void ConfirmationMessageBox_BtnYesClicked(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                switch ((MessageBoxMode)Enum.Parse(typeof(MessageBoxMode), ConfirmationMessageBox.Attributes["MessageBoxMode"]))
                {
                    case MessageBoxMode.MessageBox:

                        string _databaseName = COEConfiguration.GetDatabaseNameFromAppName(CambridgeSoft.COE.Framework.Common.COEAppName.Get().ToString());
                        COEGenericObjectStorageBO storageBO = COEGenericObjectStorageBO.Get(int.Parse(Request[Constants.SavedObjectId_UrlParameter]));
                        this.RegistryRecord.UpdateDrawingType();
                        this.RegistryRecord.ApplyAddIns();
                        this.RegistryRecord.OnSaving(this.RegistryRecord, new EventArgs());
                        storageBO.COEGenericObject = this.RegistryRecord.XmlWithAddIns;
                        storageBO.Save();


                        if (CurrentPageState == PageState.AddComponent)
                            SetPageMode(PageState.EditComponent, string.Empty);

                        DisplayComponent(0);
                        break;
                    case MessageBoxMode.ComponentDuplicateMessageBox:
                        Server.Transfer(string.Format("{0}?{1}={2}", Resource.ComponentDuplicates_URL, Constants.CurrentPageState_UrlParameter, "endNewRegistry"), false);
                        break;
                    case MessageBoxMode.MixtureDuplicateMessageBox:
                        Server.Transfer(string.Format("{0}?{1}={2}", Resource.RegistryDuplicates_Page_URL, Constants.CurrentPageState_UrlParameter, "endNewRegistry"), false);
                        break;
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void SearchComponentButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.SearchCompound();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void AddComponentButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {

                this.AddComponent();

            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (((Button)sender).CommandName.Equals("GoSubmitMixture"))
                    Server.Transfer(Resource.SubmitMixture_URL);
                else
                    Server.Transfer(Resource.Home_URL);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void DoneButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.LeavePage();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void SavePreferenceButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                COEGenericObjectStorageBO storageBO = COEGenericObjectStorageBO.Get(COEUser.Name, (int)FormGroups.UserPreference);
                _mixtureCOEFormGroup.Update();

                if (RegistryRecord.IsDirty)
                {
                    if (storageBO == null || storageBO.ID == 0)
                    {
                        storageBO = RegistryRecord.SaveTemplate("USER PREFERENCE", "USER PREFERENCE", false, 3);
                    }
                    else
                    {
                        storageBO.COEGenericObject = this.RegistryRecord.XmlWithAddIns;
                        storageBO.Save();
                    }
                    ShowConfirmationMessage(Resource.User_Preference_Save_Message + "<br>" + Resource.User_Preference_Note_Text);
                }
                else
                {
                    ShowConfirmationMessage(Resource.NoDataToSave_MessagesArea);
                }
            }
            catch (Exception exception)
            {
                if (ExceptionPolicy.HandleException(exception, Constants.REG_GUI_POLICY))
                    throw;
                else
                    _masterPage.DisplayErrorMessage(exception, false);
            }
            finally
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                           MethodBase.GetCurrentMethod().Name);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void DeleteComponentButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.DeleteComponent();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        void SubmitMixture_CommandRaised(object sender, COENavigationPanelControlEventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                //after any click we need to store the registry record

                if (e.EventType == EventsToListen.Done.ToString())
                {
                    this.LeavePage();
                }
                else if (e.EventType == EventsToListen.NodeClicked.ToString())
                {
                    UltraWebTree mixtureTree = (UltraWebTree)((Control)sender).FindControl(ControlsToTakeCare.UltraWebTreeControl.ToString());

                    Node clickedNode = mixtureTree.SelectedNode;

                    int index = int.Parse(clickedNode.DataKey.ToString().Substring(1));

                    if (index.ToString() != this.ComponentIndexHiddenField.Value)
                        this.ComponentIndexHiddenField.Value = index.ToString();

                    if (CurrentPageState != PageState.End && CurrentPageState != PageState.EndComponent)
                        _mixtureCOEFormGroup.Update();

                    switch (clickedNode.DataKey.ToString()[0])
                    {
                        case 'C':
                            LastNodeEdited = index;
                            if (CurrentRegistryType == RegistryTypes.Mixture && clickedNode.Text.IndexOf("Pick") > 0)
                            {

                                SearchCompound(index);
                            }
                            else
                            {
                                if (CurrentRegistryType == RegistryTypes.Mixture)
                                {
                                    _mixtureCOEFormGroup.DataBind();
                                }
                                this.DisplayComponent(index);

                            }
                            break;
                        case 'B':
                            if (CurrentRegistryType == RegistryTypes.Mixture)
                            {   //with out this workaround the fragments are missed after adding a component
                                //I see no downside.
                                this.DisplayRecord();

                            }

                            this.DisplayBatch(index);
                            if (CurrentPageState != PageState.EditBatch)
                            {
                                _mixtureCOEFormGroup.Update();
                            }
                            break;
                        default:
                            if (AlreadySubmitted)
                                this.DisplayRecord();
                            else
                            {
                                this.DisplayRecord();
                                this.SetPageMode(PageState.EditRecord, string.Empty);
                            }
                            break;
                    }
                }
            }
            catch (Exception exception)
            {

                if (ExceptionPolicy.HandleException(exception, Constants.REG_GUI_POLICY))
                    throw;
                else
                    _masterPage.DisplayErrorMessage(exception, false);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region GUIShell Methods

        /// <summary>
        /// This method sets all the controls attributtes as Text, etc...
        /// </summary>
        /// <remarks>You have to expand the group of your interest in the accordion</remarks>
        protected override void SetControlsAttributtes()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            this.SubmitButton.Text = Resource.Submit_Button_Text;
            this.CancelButton.Text = Resource.Cancel_Button_Text;
            this.CancelButton.ToolTip = Resource.Cancel_Button_ToolTip;
            //this button makes no sense in this form
            this.BackButton.Text = Resource.Back_Button_Text;
            //this button makes no sense in this form
            this.DoneButton.Text = Resource.Done_Button_Text;


            this.RegisterButton.Text = Resource.Register_Button_Text;
            this.ClearFormButton.Text = Resource.Clear_Button_Text;
            this.ClearFormButton.ToolTip = Resource.Clear_Button_ToolTip;



            Control messageLabel = _masterPage.GetControlInsideAnAccordionGroup(this.AccordionGroupToListen, ControlsToTakeCare.MessageLabel.ToString());

            switch (this.CurrentRegistryType)
            {
                case RegistryTypes.Both:
                    if (messageLabel != null)
                        ((Label)messageLabel).Text = Resource.AddRecord_Label_Text; break;
                case RegistryTypes.Mixture:
                    if (messageLabel != null)
                        ((Label)messageLabel).Text = Resource.AddMixture_Label_Text; break;
                case RegistryTypes.Component:
                    if (messageLabel != null)
                        ((Label)messageLabel).Text = Resource.AddComponent_Label_Text; break;
            }


            //DGB TODO This OnClientClick event needs to be implemented on the MenuItem
            //this.DeleteComponentButton.OnClientClick = @"return RemoveComponent('" + this.ActionToDoHiddenField.ClientID + "');";
            _masterPage.SetDefaultButton(this.SubmitButton.UniqueID);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// This method sets the Page Title <Title></Title>
        /// </summary>
        protected void SetPageTitle(RegistryTypes registryType)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            switch (registryType)
            {
                case RegistryTypes.Both:
                    this.PageTitleLabel.Text = Resource.SubmitRecord_Page_Title;
                    Header.Title = Resource.SubmitRecord_Page_Title;
                    break;
                case RegistryTypes.Component:
                    this.PageTitleLabel.Text = Resource.SubmitComponent_Page_Title;
                    Header.Title = Resource.SubmitComponent_Page_Title;
                    break;
                case RegistryTypes.Mixture:
                    this.PageTitleLabel.Text = Resource.SubmitMixture_Page_Title;
                    Header.Title = Resource.SubmitMixture_Page_Title;
                    break;


            }
            if (CurrentPageState == PageState.DisplayUserPreference)
            {
                this.PageTitleLabel.Text = Resource.User_Preference_Title;
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        }



        /// <summary>
        /// This method sets the Page Title <Title></Title>
        /// </summary>
        protected void SetAccordianTitle(RegistryTypes registryType)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);


            foreach (Infragistics.WebUI.UltraWebListbar.Group currentGroup in _masterPage.AccordionControl.Groups)
            {
                if ((ICOENavigationPanelControl)currentGroup.UserControl != null && ((ICOENavigationPanelControl)currentGroup.UserControl).ID == this.AccordionControlToListen)
                {
                    switch (registryType)
                    {
                        case RegistryTypes.Both:
                            currentGroup.Text = "Submit Mixture";
                            currentGroup.ToolTip = "Create a new mixture from new or registered components";
                            currentGroup.TargetUrl = this.Page.ResolveUrl(Resource.SubmitRecord_URL) + (!string.IsNullOrEmpty(this._masterPage.FormGroupIdKey) ? string.Format("?{0}={1}", this._masterPage.FormGroupIdKey, this._masterPage.PageFormGroupID) : string.Empty) + "&RegistryType=Mixture";

                            break;
                        case RegistryTypes.Component:
                            currentGroup.Text = "Submit Component";
                            currentGroup.ToolTip = "Create a new component";
                            currentGroup.TargetUrl = this.Page.ResolveUrl(Resource.SubmitComponent_URL) + (!string.IsNullOrEmpty(this._masterPage.FormGroupIdKey) ? string.Format("?{0}={1}", this._masterPage.FormGroupIdKey, this._masterPage.PageFormGroupID) : string.Empty) + "&RegistryType=Component";

                            break;
                        case RegistryTypes.Mixture:
                            currentGroup.Text = "Submit Mixture";
                            currentGroup.ToolTip = "Create a mixture from registered components";
                            currentGroup.TargetUrl = this.Page.ResolveUrl(Resource.SubmitMixture_URL) + (!string.IsNullOrEmpty(this._masterPage.FormGroupIdKey) ? string.Format("?{0}={1}", this._masterPage.FormGroupIdKey, this._masterPage.PageFormGroupID) : string.Empty) + "&RegistryType=Mixture";
                            break;

                    }

                }
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        }

        /// <summary>
        /// This method subscribe to a public method in the customized control 
        /// to handle it here in a defined method.
        /// </summary>
        protected void SubscribeToEventsInAccordionControl()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            foreach (Infragistics.WebUI.UltraWebListbar.Group currentGroup in _masterPage.AccordionControl.Groups)
            {
                if ((ICOENavigationPanelControl)currentGroup.UserControl != null)
                    if (((ICOENavigationPanelControl)currentGroup.UserControl).ID == this.AccordionControlToListen)
                        ((ICOENavigationPanelControl)currentGroup.UserControl).CommandRaised += new EventHandler<COENavigationPanelControlEventArgs>(SubmitMixture_CommandRaised);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        }

        /// <summary>
        /// Load the control/s in the right accordion's panel
        /// </summary>
        /// <remarks>You should show as expanded the group that you are interested</remarks> 
        protected void LoadControlInAccordionPanel()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            /* Get Control Data from source. */
            this.GetControlInfo();
            for (int i = 0; i < COENavigationPaneObject.ControlItem.Count; i++)
            {
                _masterPage.AddControlToAccordionPanel(COENavigationPaneObject.ControlItem[i], true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Method which load the Control Info in a DataSet (jsut for testing purpose)
        /// This Data Object will come from the Configuration Services
        /// </summary>
        private void GetControlInfo()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {

                COENavigationPaneObject = new COENavigationPane();
                COENavigationPane.ControlItemRow registryRecordNavigationControlRow = COENavigationPaneObject.ControlItem.NewControlItemRow();
                registryRecordNavigationControlRow.ParentID = "SubmitMixture";
                registryRecordNavigationControlRow.ControlType = GUIShellTypes.SupportedControlTypes.UserControl.ToString();
                if (registryRecordNavigationControlRow.ControlType == GUIShellTypes.SupportedControlTypes.UserControl.ToString())
                    registryRecordNavigationControlRow.ControlSource = GUIShellTypes.UCPath + "RegistryRecordNavigation.ascx";
                registryRecordNavigationControlRow.ID = "RegistryRecordNavigation";
                COENavigationPaneObject.ControlItem.AddControlItemRow(registryRecordNavigationControlRow);
            }
            catch (Exception exception)
            {
                if (ExceptionPolicy.HandleException(exception, Constants.REG_GUI_POLICY))
                    throw;
                else
                    _masterPage.DisplayErrorMessage(exception, false);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void DisplayStatusMessage(string text)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            Control messageLabel = _masterPage.GetControlInsideAnAccordionGroup(this.AccordionGroupToListen, ControlsToTakeCare.MessageLabel.ToString());
            if (messageLabel != null)
            {
                ((Label)messageLabel).Text = text;
                ((Label)messageLabel).Visible = true;
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }



        private void DisplayCompoundIntoTree(RegistryRecord registryRecord)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            ICOENavigationPanelControl tree = _masterPage.GetICOENavigationPanelControlInsideAnAccordionGroup(this.AccordionGroupToListen);
            if (tree != null) //Coverity fix - CID 11862 
            {
                //LJB: Set registry type so the tree will display specific information relevant to mixture workflow
                tree.NewRegistryType = this.CurrentRegistryType.ToString();
                tree.DataBind(registryRecord);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void DisplayCompoundIntoTree(RegistryRecord registryRecord, string nodeToShowSelected)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            ICOENavigationPanelControl tree = _masterPage.GetICOENavigationPanelControlInsideAnAccordionGroup(this.AccordionGroupToListen);
            if (tree != null) //Coverity fix - CID 11863 
            {
                //LJB: Set registry type so the tree will display specific information relevant to mixture workflow
                tree.NewRegistryType = this.CurrentRegistryType.ToString();
                tree.DataBind(registryRecord, nodeToShowSelected);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void ShowTreeButtons(bool show)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            Control showButtonsIDHidden = _masterPage.GetControlInsideAnAccordionGroup(this.AccordionGroupToListen, ControlsToTakeCare.ShowButtonsHidden.ToString());
            if (showButtonsIDHidden != null)
                ((HiddenField)showButtonsIDHidden).Value = show.ToString();

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }


        /// <summary>
        /// Overrides the template value for specified property 
        /// Note : Works only for batch level properties.
        /// </summary>
        /// <param name="propertyName">Property to override</param>
        /// <param name="defaultValue">Value to set in property</param>
        /// <param name="coeGenericObjectXml">Template ObjectConfig XML</param>
        private void RestoreDefaultBatchPropertyValue(string propertyName, string defaultValue, ref string coeGenericObjectXml)
        {
            try
            {
                string propertyPath = "MultiCompoundRegistryRecord/BatchList/Batch/PropertyList/Property[@name='" + propertyName + "']";
                XmlDocument xDocument = new XmlDocument();
                xDocument.LoadXml(coeGenericObjectXml);
                XmlNode xNode = xDocument.SelectSingleNode(propertyPath);
                if (xNode != null)
                {
                    if (xNode.InnerText == "")
                    {
                    xNode.InnerText = defaultValue;
                    coeGenericObjectXml = xDocument.OuterXml;
                    }
                }
            }
            catch (Exception exception)
            {
                if (ExceptionPolicy.HandleException(exception, Constants.REG_GUI_POLICY))
                    throw;
                else
                    _masterPage.DisplayErrorMessage(exception, false);
            }
            finally
            {

                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                           MethodBase.GetCurrentMethod().Name);
            }
        }
        #endregion

        #region Page Methods

        private FormGroup BuildCoeFormHolder(string xmlUrl)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(Page.MapPath(Page.TemplateSourceDirectory) + "/" + xmlUrl);
                return FormGroup.GetFormGroup(xmlDocument.OuterXml);
            }
            catch (Exception exception)
            {
                if (ExceptionPolicy.HandleException(exception, Constants.REG_GUI_POLICY))
                    throw;
                else
                    _masterPage.DisplayErrorMessage(exception, false);

                return null;
            }
            finally
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void LoadFormGenerators()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            try
            {
                FormGroup coeFormCompoundHolder = COEFormBO.Get(this._masterPage.PageFormGroupID).COEFormGroup;

                _mixtureCOEFormGroup = new COEFormGroup();

                _mixtureCOEFormGroup.ID = "SubmitMixture";

                mixtureInformationHolder.Controls.Clear();
                mixtureInformationHolder.Controls.Add(_mixtureCOEFormGroup);

                _mixtureCOEFormGroup.FormGroupDescription = coeFormCompoundHolder;
                _mixtureCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;
                _mixtureCOEFormGroup.DisplayCulture = this.DisplayCulture;

                mixtureInformationHolder.Visible = true;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void SetButtonStates()
        {
            if (CurrentRegistryType == RegistryTypes.Mixture || CurrentRegistryType == RegistryTypes.Both)
            {
                if (CurrentPageState != PageState.End)
                {
                    // Set to default.
                    bool isValidComponentList = true;
                    bool isValidBatchList = true;
                    this.SubmitButton.Visible = true;
                    this.SubmitButton.Enabled = false;
                    this.SubmitButton.ToolTip = Resource.MixtureError_Creation_Batch_ToolTip; ;
                    this.RegisterButton.Visible = true;
                    this.RegisterButton.Enabled = false;
                    this.RegisterButton.ToolTip = Resource.MixtureRegError_Creation_Batch_ToolTip;


                    if (CurrentPageState != PageState.End)
                    {
                        if (this.RegistryRecord.ComponentList.Count < Convert.ToInt32(MixtureRule.ComponentList))
                        {
                            this.SubmitButton.ToolTip = Resource.MixtureError_Creation_ToolTip;
                            this.RegisterButton.ToolTip = Resource.MixtureRegError_Creation_ToolTip;
                            isValidComponentList = false;
                        }
                        else if (RegistryRecord.BatchList[0].BatchComponentList[0].PropertyList["PERCENTAGE"].Value == string.Empty && !CurrentPageState.ToString().Contains("Batch") && !IsMixture)
                        {
                            this.SubmitButton.ToolTip = Resource.MixtureError_Creation_Batch_ToolTip;
                            this.RegisterButton.ToolTip = Resource.MixtureRegError_Creation_Batch_ToolTip;
                            isValidBatchList = false;
                        }
                        else if (this.RegistryRecord.ComponentList.Count >= Convert.ToInt32(MixtureRule.ComponentList) && CurrentPageState.ToString().Contains("Batch"))
                        {
                            isValidComponentList = true;
                            isValidBatchList = true;
                            IsMixture = true;
                        }

                        if (IsMixture && isValidComponentList && isValidBatchList)
                        {
                            this.SubmitButton.Enabled = true;
                            this.SubmitButton.ToolTip = Resource.MixtureOK_Creation_ToolTip;
                            this.RegisterButton.Visible = true;
                            this.RegisterButton.Enabled = true;
                            this.RegisterButton.ToolTip = Resource.MixtureRegOK_Creation_ToolTip;
                        }
                        else
                        {
                            this.SubmitButton.Enabled = false;
                            this.RegisterButton.Enabled = false;
                        }
                    }
                }

                if (CurrentPageState != PageState.ViewComponent && CurrentPageState != PageState.End)
                {
                    this.ClearFormButton.Enabled = true;
                }
                else
                {
                    this.ClearFormButton.Enabled = false;
                }
                this.ClearFormButton.Visible = true;
                this.BackButton.Visible = false;
                this.CancelButton.Visible = true;
                this.CancelButton.Enabled = true;
            }
            else
            {

                if (CurrentRegistryType == RegistryTypes.Component)
                {
                    this.SubmitButton.Enabled = true;
                    this.SubmitButton.ToolTip = Resource.ComponentOK_Creation_ToolTip;
                    this.RegisterButton.Enabled = true;
                    this.RegisterButton.ToolTip = Resource.ComponentRegOK_Creation_ToolTip;
                    this.BackButton.Visible = false;
                    this.CancelButton.Visible = true;
                    this.ClearFormButton.Visible = true;
                    this.CancelButton.Enabled = true;
                    this.ClearFormButton.Enabled = true;
                }
                if (CurrentRegistryType == RegistryTypes.Both)
                {
                    this.SubmitButton.Enabled = true;
                    this.SubmitButton.ToolTip = Resource.RegistryOK_Creation_ToolTip;
                    this.RegisterButton.Enabled = true;
                    this.RegisterButton.ToolTip = Resource.RegistryRegOK_Creation_ToolTip;
                    this.BackButton.Visible = false;
                    this.CancelButton.Visible = true;
                    this.ClearFormButton.Visible = true;
                    this.CancelButton.Enabled = true;
                    this.ClearFormButton.Enabled = true;
                }
                if (CurrentPageState == PageState.DisplayUserPreference)
                {
                    this.DoneButton.Visible = false;
                    this.CancelButton.Visible = false;
                    this.ClearFormButton.Visible = false;
                }

            }
        }

        private void SetPageMode(PageState mode, string messageToDisplay)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            PageState previousState = CurrentPageState;
            CurrentPageState = mode;

            RegistryRecord registryRecord = (RegistryRecord)Session[RegistrationWebApp.Constants.MultiCompoundObject_Session];             

            this.DefineMixtureImageMenuButton.Visible = true;
            switch (mode)
            {
                case PageState.AddSingleCompound:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Add;
                    _mixtureCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;
                    //need logic to look at system seeting to show or hide
                    this.DefineMixtureImageMenuButton.Visible = false;
                    this.ClearFormButton.Visible = true;
                    this.BackButton.Visible = false;
                    //handle the three RegistryTypes so controls and messages are appropriate
                    switch (this.CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.MixtureResumeForm, true);
                            SetFormGeneratorVisibility(SubForms.StructureCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                            this.DefineMixtureImageMenuButton.Visible = true;
                            this.DisplayStatusMessage(Resource.AddRecord_Label_Text);
                            this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true;  //AddComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[1].Enabled = false;  //SearchComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false; //DeleteComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[3].Enabled = false;  //Continue
                            break;
                        //Fix for CSBR: 154064, Title- Mixture creation has too many options in drop-down menu and 
                        //confusing mouse over.
                        // This code removes ‘pick an existing component’ menu item from Drop down menu, Which is  
                        //not required in Mixture flow.
                        case RegistryTypes.Mixture:
                            SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.MixtureResumeForm, true);
                            SetFormGeneratorVisibility(SubForms.StructureCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                            this.DisplayStatusMessage(Resource.AddMixture_Label_Text);
                            this.DefineMixtureImageMenuButton.Visible = true;
                            //Fix for CSBR 161699- Issue with Pick Component link on Mixtures form
                            if (registryRecord != null)
                            {
                                if (registryRecord.ComponentList.Count > 0)
                                    this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true;  //AddComponent
                                else
                                    this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = false;  //AddComponent
                            }
                            else
                                this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = false; //AddComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[1].Visible = false;//SearchComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false; //DeleteComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[3].Enabled = false;  //Continue


                            break;
                        //Fix for CSBR: 154064, Title- Mixture creation has too many options in drop-down menu and 
                        //confusing mouse over.
                        // This code removes ‘pick an existing component’ menu item from Drop down menu, Which is  
                        //not required in Mixture flow.
                        case RegistryTypes.Component:
                            SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                            SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.BatchForm, true);
                            SetFormGeneratorVisibility(SubForms.BatchCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.MixtureResumeForm, true);
                            SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                            this.DefineMixtureImageMenuButton.Visible = false;
                            this.DisplayStatusMessage(Resource.AddComponent_Label_Text);
                            this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = false;  //AddComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[1].Visible = false;  //SearchComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false; //DeleteComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[3].Enabled = false;  //Continue
                            break;
                    }
                    break;

                case PageState.EditSingleCompound:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Edit;
                    _mixtureCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;
                    this.BackButton.Visible = false;
                    this.RegisterButton.Visible = this.SubmitButton.Visible = true;
                    this.DisplayStatusMessage(Resource.EditCompound_Label_Text);
                    switch (this.CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                            SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.BatchForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                            SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                            this.DefineMixtureImageMenuButton.Visible = true;
                            this.DisplayStatusMessage(Resource.EditCompound_Label_Text);
                            this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true;  //AddComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[1].Enabled = true;  //SearchComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false; //DeleteComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[3].Enabled = false;  //Continue
                            break;
                        //Fix for CSBR: 154064, Title- Mixture creation has too many options in drop-down menu and 
                        //confusing mouse over.
                        // This code removes ‘pick an existing component’ menu item from Drop down menu, Which is  
                        //not required in Mixture flow.
                        case RegistryTypes.Mixture:
                            SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.MixtureResumeForm, true);
                            SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.StructureCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                            this.DefineMixtureImageMenuButton.Visible = true;
                            this.DisplayStatusMessage(Resource.EditCompound_Label_Text);
                            this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true; //AddComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[1].Visible = false; //SearchComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false; //DeleteComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[3].Enabled = false;  //Continue
                            break;
                        //Fix for CSBR: 154064, Title- Mixture creation has too many options in drop-down menu and 
                        //confusing mouse over.
                        // This code removes ‘pick an existing component’ menu item from Drop down menu, Which is  
                        //not required in Mixture flow.
                        case RegistryTypes.Component:
                            SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                            SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.BatchForm, true);
                            SetFormGeneratorVisibility(SubForms.BatchCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.MixtureResumeForm, true);
                            SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                            this.DefineMixtureImageMenuButton.Visible = false;
                            this.DisplayStatusMessage(Resource.EditCompound_Label_Text);
                            this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = false;  //AddComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[1].Visible = false;  //SearchComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false; //DeleteComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[3].Enabled = false;  //Continue
                            break;
                    }
                    break;
                case PageState.AddComponent:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Add;
                    _mixtureCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;

                    this.RegisterButton.Visible = this.SubmitButton.Visible = false;
                    switch (this.CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DefineMixtureImageMenuButton.Visible = true;
                            this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true;  //AddComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[1].Enabled = true;  //SearchComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled =
                                registryRecord.ComponentList.Count > minComponents; //DeleteComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[3].Enabled = true;  //Continue
                            this.DisplayStatusMessage(Resource.AddRecord_Label_Text);
                            SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                            SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);
                            break;
                        //Fix for CSBR: 154064, Title- Mixture creation has too many options in drop-down menu and 
                        //confusing mouse over.
                        // This code removes ‘pick an existing component’ menu item from Drop down menu, Which is  
                        //not required in Mixture flow.
                        case RegistryTypes.Mixture:

                            this.DisplayStatusMessage(Resource.AddMixture_Label_Text);
                            SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.MixtureResumeForm, true);
                            SetFormGeneratorVisibility(SubForms.StructureCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                            this.DisplayStatusMessage(Resource.AddMixture_Label_Text);
                            this.DefineMixtureImageMenuButton.Visible = true;
                            this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true;  //AddComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[1].Visible = false;  //SearchComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = registryRecord.ComponentList.Count > minComponents; //DeleteComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[3].Enabled = true;  //Continue
                            break;
                        //Fix for CSBR: 154064, Title- Mixture creation has too many options in drop-down menu and 
                        //confusing mouse over.
                        // This code removes ‘pick an existing component’ menu item from Drop down menu, Which is  
                        //not required in Mixture flow.
                        case RegistryTypes.Component:
                            this.DefineMixtureImageMenuButton.Visible = false;
                            this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = false;  //AddComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[1].Visible = false; //SearchComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false; //DeleteComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[3].Enabled = false;  //Continue
                            this.DisplayStatusMessage(Resource.AddComponent_Label_Text);
                            SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                            SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.BatchForm, true);
                            SetFormGeneratorVisibility(SubForms.BatchCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.BatchComponentFragmentsForm, true);
                            SetFormGeneratorVisibility(SubForms.MixtureResumeForm, true);
                            SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentListFragmentsForm, false);
                            break;
                    }
                    break;


                case PageState.AddBatch:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Add;
                    _mixtureCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;
                    SetFormGeneratorVisibility(SubForms.BatchForm, true);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, true);
                    SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties,
                                             this.RegistryRecord.ComponentList.Count > 1 ? true : false);
                    SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                    SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);
                    this.RegisterButton.Visible = true;
                    this.SubmitButton.Visible = true;
                    this.BackButton.Visible = true;
                    this.CancelButton.CommandName = "GoSubmitMixture";
                    this.DefineMixtureImageMenuButton.Visible = false;
                    this.SubmissionTemplateImageMenuButton.MenuItemList[0].Enabled = true; //SaveTemplate
                    this.SubmissionTemplateImageMenuButton.MenuItemList[1].Enabled = false; //LoadTemplate

                    switch (this.CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DisplayStatusMessage(Resource.CompleteMixtureBatch_Label_Text);
                            break;
                        case RegistryTypes.Mixture:
                            this.DisplayStatusMessage(Resource.CompleteMixtureBatch_Label_Text);
                            break;
                        case RegistryTypes.Component:
                            this.DisplayStatusMessage(Resource.CompleteMixtureBatch_Label_Text);
                            break;
                    }
                    break;
                case PageState.EditComponent:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Edit;
                    _mixtureCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;
                    this.RegisterButton.Visible = false;
                    this.SubmitButton.Visible = false;
                    switch (this.CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DisplayStatusMessage(Resource.AddRecord_Label_Text);
                            SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                            SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.BatchComponentFragmentsForm, true);
                            SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentListFragmentsForm, false);
                            this.DefineMixtureImageMenuButton.Visible = true;
                            // Fix for CBOE-1479
                            this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true;  //AddComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[1].Enabled = true;  //SearchComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled =
                                registryRecord.ComponentList.Count > minComponents; //DeleteComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[3].Enabled = true;  //Continue
                            break;
                        //Fix for CSBR: 154064, Title- Mixture creation has too many options in drop-down menu and 
                        //confusing mouse over.
                        // This code removes ‘pick an existing component’ menu item from Drop down menu, Which is  
                        //not required in Mixture flow.
                        case RegistryTypes.Mixture:
                            this.DisplayStatusMessage(Resource.AddMixture_Label_Text);
                            SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                            // SetFormGeneratorVisibility(SubForms.BatchComponentFragmentsForm, false);
                            SetFormGeneratorVisibility(SubForms.StructureCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.MixtureResumeForm, true);
                            SetFormGeneratorVisibility(SubForms.BatchForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);
                            //SetFormGeneratorVisibility(SubForms.BatchComponentListFragmentsForm, false);
                            this.DefineMixtureImageMenuButton.Visible = true;
                            this.DefineMixtureImageMenuButton.MenuItemList[0].Visible = false;  //AddComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[1].Visible = false;  //SearchComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled =
                                registryRecord.ComponentList.Count > minComponents; //DeleteComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[3].Enabled = true;  //Continue
                            break;
                        //Fix for CSBR: 154064, Title- Mixture creation has too many options in drop-down menu and 
                        //confusing mouse over.
                        // This code removes ‘pick an existing component’ menu item from Drop down menu, Which is  
                        //not required in Mixture flow.
                        case RegistryTypes.Component:
                            SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                            SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.BatchComponentFragmentsForm, true);
                            SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);

                            SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentListFragmentsForm, false);
                            this.DisplayStatusMessage(Resource.AddComponent_Label_Text);
                            this.DefineMixtureImageMenuButton.Visible = false;
                            this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = false;  //AddComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[1].Visible = false;  //SearchComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false; //DeleteComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[3].Enabled = false;  //Continue
                            break;
                    }
                    break;
                case PageState.ViewComponent:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Edit;
                    _mixtureCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;

                    int componentIndex = this._mixtureCOEFormGroup.GetSubForm(
                            this.GetFormIndex(SubForms.ComponentForm)).PageIndex;
                    _mixtureCOEFormGroup.GetSubForm(GetFormIndex(
                SubForms.ComponentForm)).DisplayMode = FormGroup.DisplayMode.View;
                    _mixtureCOEFormGroup.GetSubForm(GetFormIndex(
                    SubForms.CompoundCustomProperties)).DisplayMode = FormGroup.DisplayMode.View;

                    // Disable viewstate for the components in viewmode.
                    SetFormGeneratorViewState(SubForms.CompoundCustomProperties, false);
                    SetFormGeneratorViewState(SubForms.ComponentForm, false);
                    SetFormGeneratorViewState(SubForms.RegistryCustomProperties, false);
                    //---------------------------------------------------------------
                    if (AlreadySubmitted)
                    {
                        this.RegisterButton.Visible = this.SubmitButton.Visible = false;
                        this.ComponentToolBar.Visible = false;
                        this.ConfirmationMessageBox.Visible = false;
                        switch (this.CurrentRegistryType)
                        {
                            case RegistryTypes.Both:

                                SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);
                                SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                                SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                                SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                                SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                                SetFormGeneratorVisibility(SubForms.BatchForm, false);
                                SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                                SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                                SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                                SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);

                                this.DefineMixtureImageMenuButton.Visible = true;
                                this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = false; //AddComponent
                                this.ShowConfirmationMessage(string.Format(Resource.SubmittedMixture_Label_Text,
                                    registryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                                                         GUIShellTypes.IdsPaddingCharacter)));
                                break;
                            case RegistryTypes.Mixture:
                                SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);
                                SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                                SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                                SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                                SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                                SetFormGeneratorVisibility(SubForms.BatchForm, false);
                                SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                                SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                                SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                                SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);
                                this.DefineMixtureImageMenuButton.Visible = true;
                                this.DefineMixtureImageMenuButton.MenuItemList[0].Visible = false; //AddComponent
                                this.ShowConfirmationMessage(string.Format(Resource.SubmittedMixture_Label_Text,
                                    registryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                                                         GUIShellTypes.IdsPaddingCharacter)));
                                if (this.RegistryRecord.ComponentList.Count < 2)
                                {
                                    this.SubmitButton.Enabled = false;
                                    this.SubmitButton.ToolTip = "Mixtures must have more than 1 component to submit";
                                    this.RegisterButton.Enabled = false;
                                    this.RegisterButton.ToolTip = "Mixtures must have more than 1 component to register";


                                }
                                if (this.RegistryRecord.BatchList[0].BatchComponentList.IsDirty == false)
                                {
                                    this.SubmitButton.Enabled = false;
                                    this.SubmitButton.ToolTip = "Mixtures must have batch information to submit";
                                    this.RegisterButton.Enabled = false;
                                    this.RegisterButton.ToolTip = "Mixtures must have batch information to register";

                                }
                                break;
                            case RegistryTypes.Component:
                                SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);
                                SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                                SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                                SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                                SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                                SetFormGeneratorVisibility(SubForms.BatchForm, false);
                                SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                                SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                                SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                                SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);
                                this.DefineMixtureImageMenuButton.Visible = false;
                                this.ShowConfirmationMessage(string.Format(Resource.SubmittedMixture_Label_Text,
                                    registryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                                                         GUIShellTypes.IdsPaddingCharacter)));
                                break;
                        }
                    }
                    else
                    {
                        this.RegisterButton.Visible = this.SubmitButton.Visible = false;

                        switch (this.CurrentRegistryType)
                        {
                            case RegistryTypes.Both:
                                SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);
                                SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                                SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                                SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                                SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                                SetFormGeneratorVisibility(SubForms.BatchForm, false);
                                SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                                SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                                SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                                SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);
                                this.DefineMixtureImageMenuButton.Visible = true;
                                this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true; //AddComponent
                                this.DefineMixtureImageMenuButton.MenuItemList[1].Enabled = true; //SearchComponent
                                break;
                            //Fix for CSBR: 154064, Title- Mixture creation has too many options in drop-down menu and 
                            //confusing mouse over.
                            // This code removes ‘pick an existing component’ menu item from Drop down menu, Which is  
                            //not required in Mixture flow.
                            case RegistryTypes.Mixture:
                                this.RegisterButton.Visible = this.SubmitButton.Visible = true;
                                SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);
                                SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                                SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                                SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                                SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                                SetFormGeneratorVisibility(SubForms.BatchForm, false);
                                SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                                SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                                SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                                SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);
                                this.DefineMixtureImageMenuButton.Visible = true;
                                this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true; //AddComponent
                                this.DefineMixtureImageMenuButton.MenuItemList[1].Visible = false; //SearchComponent
                                if (this.RegistryRecord.ComponentList.Count > 1)
                                {
                                    this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = true; //deleteComponent
                                }
                                break;
                            case RegistryTypes.Component:
                                SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);
                                SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                                SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                                SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                                SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                                SetFormGeneratorVisibility(SubForms.BatchForm, false);
                                SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                                SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                                SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                                SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);
                                this.DefineMixtureImageMenuButton.Visible = false;
                                break;
                        }
                    }





                    break;
                case PageState.ViewBatch:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;
                    _mixtureCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;

                    SetFormGeneratorVisibility(SubForms.BatchForm, true);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, true);
                    SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties,
                                                this.RegistryRecord.ComponentList.Count > 1 ? true : false);
                    SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                    SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);
                    //SetFormGeneratorVisibility(SubForms.BatchComponentListFragmentsForm, false);

                    this.RegisterButton.Visible = this.SubmitButton.Visible = false;

                    this.ComponentToolBar.Visible = false;

                    this.DefineMixtureImageMenuButton.Visible = false;

                    this.SubmissionTemplateImageMenuButton.MenuItemList[0].Enabled = true; //SaveTemplate
                    this.SubmissionTemplateImageMenuButton.MenuItemList[1].Enabled = false; //LoadTemplate


                    switch (this.CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            if (AlreadySubmitted)
                                this.ShowConfirmationMessage(string.Format(Resource.SubmittedMixture_Label_Text,
                                registryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                        GUIShellTypes.IdsPaddingCharacter)));

                            this.DisplayStatusMessage(Resource.ViewBatchDetails_Label_Text);

                            break;
                        case RegistryTypes.Mixture:
                            if (AlreadySubmitted)
                                this.ShowConfirmationMessage(string.Format(Resource.SubmittedMixture_Label_Text,
                                registryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                        GUIShellTypes.IdsPaddingCharacter)));

                            this.DisplayStatusMessage(Resource.ViewBatchDetails_Label_Text);

                            break;
                        case RegistryTypes.Component:
                            if (AlreadySubmitted)
                                this.ShowConfirmationMessage(string.Format(Resource.SubmittedMixture_Label_Text,
                                registryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                        GUIShellTypes.IdsPaddingCharacter)));

                            this.DisplayStatusMessage(Resource.ViewBatchDetails_Label_Text);

                            break;
                    }
                    break;
                case PageState.EditBatch:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Edit;
                    _mixtureCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;

                    SetFormGeneratorVisibility(SubForms.BatchForm, true);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, true);
                    SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties,
                                                this.RegistryRecord.ComponentList.Count > 1 ? true : false);
                    SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                    SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);

                    this.RegisterButton.Visible = this.SubmitButton.Visible = this.BackButton.Visible = true;

                    this.DefineMixtureImageMenuButton.Visible = false;


                    switch (this.CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DisplayStatusMessage(Resource.CompleteMixtureBatch_Label_Text);
                            break;
                        //Fix for CSBR: 154064, Title- Mixture creation has too many options in drop-down menu and 
                        //confusing mouse over.
                        // This code removes ‘pick an existing component’ menu item from Drop down menu, Which is  
                        //not required in Mixture flow.
                        case RegistryTypes.Mixture:
                            this.DefineMixtureImageMenuButton.Visible = true;
                            this.DefineMixtureImageMenuButton.MenuItemList[0].Visible = true;  //AddComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[1].Visible = false;  //SearchComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false;
                            this.DisplayStatusMessage(Resource.CompleteMixtureBatch_Label_Text);
                            if (this.RegistryRecord.ComponentList.Count < 2)
                            {
                                this.SubmitButton.Enabled = false;
                                this.SubmitButton.ToolTip = "Mixtures must have more than 1 component to submit";
                                this.RegisterButton.Enabled = false;
                                this.RegisterButton.ToolTip = "Mixtures must have more than 1 component to register";


                            }
                            break;
                        case RegistryTypes.Component:
                            this.DisplayStatusMessage(Resource.CompleteMixtureBatch_Label_Text);
                            break;
                    }
                    break;
                case PageState.EditRecord:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Edit;
                    _mixtureCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;

                    SetFormGeneratorVisibility(SubForms.MixtureResumeForm, true);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);

                    this.RegisterButton.Visible = true;
                    this.SubmitButton.Visible = true;
                    this.BackButton.Visible = true;

                    switch (this.CurrentRegistryType)
                    {
                        case RegistryTypes.Both:

                            SetFormGeneratorVisibility(SubForms.MixtureResumeForm, true);
                            SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                            this.DefineMixtureImageMenuButton.Visible = true;
                            this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true;  //AddComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[1].Enabled = false;  //SearchComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false; //DeleteComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[3].Enabled = false;  //Continue
                            this.DisplayStatusMessage(Resource.AddRecord_Label_Text);
                            break;
                        //Fix for CSBR: 154064, Title- Mixture creation has too many options in drop-down menu and 
                        //confusing mouse over.
                        // This code removes ‘pick an existing component’ menu item from Drop down menu, Which is  
                        //not required in Mixture flow.
                        case RegistryTypes.Mixture:
                            SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.MixtureResumeForm, true);
                            SetFormGeneratorVisibility(SubForms.StructureCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                            this.DefineMixtureImageMenuButton.Visible = true;
                            this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true;  //AddComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[1].Visible = false; //SearchComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false; //DeleteComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[3].Enabled = true;  //Continue
                            this.DisplayStatusMessage(Resource.AddMixture_Label_Text);
                            break;
                        case RegistryTypes.Component:
                            SetFormGeneratorVisibility(SubForms.MixtureResumeForm, true);
                            SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                            SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);

                            this.DefineMixtureImageMenuButton.Visible = false;
                            this.DisplayStatusMessage(Resource.AddComponent_Label_Text);
                            break;
                    }
                    break;
                case PageState.End:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;
                    _mixtureCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;

                    SetFormGeneratorVisibility(SubForms.MixtureResumeForm, true);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);

                    this.RegisterButton.Visible = false;
                    this.SubmitButton.Visible = false;
                    this.CancelButton.Visible = false;

                    this.BackButton.Visible = false;
                    this.DoneButton.Visible = true;

                    this.DisplayStatusMessage(String.Empty);

                    this.SubmissionTemplateImageMenuButton.Visible = false;
                    this.DefineMixtureImageMenuButton.Visible = false;


                    this.SubmissionTemplateImageMenuButton.MenuItemList[0].Enabled = false; //SaveTemplate
                    this.SubmissionTemplateImageMenuButton.MenuItemList[1].Enabled = false; //LoadTemplate


                    if (RegistryRecord.RegNum == string.Empty)
                    {


                        switch (this.CurrentRegistryType)
                        {
                            case RegistryTypes.Both:
                                //if (RegistryRecord.ID > 0)
                                //    this.ShowConfirmationMessage(string.Format(Resource.SubmittedMixture_Label_Text,
                                //            registryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                //                                                 GUIShellTypes.IdsPaddingCharacter)));
                                //this.DisplayStatusMessage(Resource.ViewRecordDetails_Label_Text);
                                Server.Transfer(string.Format("{0}?isSubmit=true&SubmittedObjectId={1}&{2}={3}", this.ResolveUrl(Resource.ReviewRegister_URL), this.RegistryRecord.ID, Constants.CurrentPageState_UrlParameter, "end"));

                                break;
                            case RegistryTypes.Mixture:
                                //if (RegistryRecord.ID > 0)
                                //    this.ShowConfirmationMessage(string.Format(Resource.SubmittedMixture_Label_Text,
                                //            registryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                //                                                 GUIShellTypes.IdsPaddingCharacter)));
                                //this.DisplayStatusMessage(Resource.ViewMixtureDetails_Label_Text);
                                Server.Transfer(string.Format("{0}?isSubmit=true&SubmittedObjectId={1}&{2}={3}", this.ResolveUrl(Resource.ReviewRegister_URL), this.RegistryRecord.ID, Constants.CurrentPageState_UrlParameter, "end"));

                                break;
                            case RegistryTypes.Component:
                                //if (RegistryRecord.ID > 0)
                                //    this.ShowConfirmationMessage(string.Format(Resource.SubmittedComponent_Label_Text,
                                //            registryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                //                                                 GUIShellTypes.IdsPaddingCharacter)));
                                //this.DisplayStatusMessage(Resource.ViewComponentDetails_Label_Text);
                                Server.Transfer(string.Format("{0}?isSubmit=true&SubmittedObjectId={1}&{2}={3}", this.ResolveUrl(Resource.ReviewRegister_URL), this.RegistryRecord.ID, Constants.CurrentPageState_UrlParameter, "end"));

                                break;
                        }
                    }
                    else
                    {

                        //When the registry was succesfully inserted, we redirect the user to the ViewMixture Page.
                        string url = string.Format("{0}?RegisteredObjectId={1}", Resource.ViewMixture_URL, RegistryRecord.RegNumber.RegNum);
                        string extraParams = RegUtilities.GetViewMixURLParams();
                        if (!string.IsNullOrEmpty(extraParams))
                            url += "&" + extraParams;
                        Server.Transfer(url, false);
                        break;
                    }
                    break;

                case PageState.EndComponent:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;
                    _mixtureCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;

                    if (!isEditable(registryRecord.ComponentList[LastNodeEdited])
                        || registryRecord.ComponentList[LastNodeEdited].Compound.BaseFragment.Structure.IsWildcard)
                    {
                        SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                        SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                    }
                    else
                    {
                        SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, true);
                        SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                    }

                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.StructureCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);

                    this.RegisterButton.Visible = false;
                    this.SubmitButton.Visible = false;
                    this.CancelButton.Visible = false;

                    this.BackButton.Visible = false;
                    this.DoneButton.Visible = true;

                    this.DefineMixtureImageMenuButton.Visible = false;

                    this.SubmissionTemplateImageMenuButton.MenuItemList[0].Enabled = false; //SaveTemplate
                    this.SubmissionTemplateImageMenuButton.MenuItemList[1].Enabled = false; //LoadTemplate

                    switch (this.CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DisplayStatusMessage(String.Empty);

                            if (RegistryRecord.ID > 0)
                                this.ShowConfirmationMessage(string.Format(Resource.SubmittedRecord_Label_Text,
                                    registryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                                                         GUIShellTypes.IdsPaddingCharacter)));
                            break;
                        case RegistryTypes.Mixture:
                            this.DisplayStatusMessage(String.Empty);

                            if (RegistryRecord.ID > 0)
                                this.ShowConfirmationMessage(string.Format(Resource.SubmittedMixture_Label_Text,
                                    registryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                                                         GUIShellTypes.IdsPaddingCharacter)));
                            break;
                        case RegistryTypes.Component:
                            this.DisplayStatusMessage(String.Empty);

                            if (RegistryRecord.ID > 0)
                                this.ShowConfirmationMessage(string.Format(Resource.SubmittedComponent_Label_Text,
                                    registryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                                                         GUIShellTypes.IdsPaddingCharacter)));
                            break;
                    }
                    break;
                case PageState.DisplayUserPreference: // User Preference pagestate changes
                     _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Add;
                    _mixtureCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;
                    this.DefineMixtureImageMenuButton.Visible = false;
                    this.SavePreferenceButton.Visible = true;
                    this.SavePreferenceButton.Text = Resource.Save_Button_Text;//User preference modification
                    this.ShowConfirmationMessage(Resource.User_Preference_Note_Text);
                    this.BackButton.Visible = false;
                    this.RegisterButton.Visible = false;
                    this.SubmitButton.Visible = false;
                    this.CancelButton.Visible = false;
                    this.BackButton.Visible = false;
                    this.DoneButton.Visible = false;
                    this.DefineMixtureImageMenuButton.Visible = false;
                    this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = false;  //AddComponent
                    this.DefineMixtureImageMenuButton.MenuItemList[1].Visible = false;  //SearchComponent
                    this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false; //DeleteComponent
                    this.DefineMixtureImageMenuButton.MenuItemList[3].Enabled = false;  //Continue
                    this.SubmissionTemplateImageMenuButton.Visible = false;
                    this.SubmissionTemplateImageMenuButton.MenuItemList[0].Enabled = false;
                    this.SubmissionTemplateImageMenuButton.MenuItemList[1].Enabled = false;
                    SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchForm, true);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.MixtureResumeForm, true);
                    SetFormGeneratorVisibility(SubForms.StructureCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                    CustomizeUserPreference();
                    break;
            } // End Switch

            if (CurrentPageState != PageState.AddSingleCompound)
                this.ClearFormButton.Visible = false;
            if (CurrentPageState == PageState.EditSingleCompound)
                this.ClearFormButton.Visible = true;


            SetButtonStates();


            this.SetFragmentsFormsVisibility(((RegUtilities.PageState)Enum.Parse(
                                    typeof(RegUtilities.PageState), mode.ToString())));

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        private void SetFragmentsFormsVisibility(RegUtilities.PageState mode)
        {
            RegUtilities.FragmentForm showFragmentsRO = RegUtilities.ShowFragmentsInRO(this.RegistryRecord.SameBatchesIdentity, mode, this.RegistryRecord.ComponentList.Count == 1, CurrentRegistryType.ToString());
            switch (showFragmentsRO)
            {
                case RegUtilities.FragmentForm.Edit:
                    SetFormGeneratorVisibility(SubForms.BatchComponentFragmentsForm, true);
                    SetFormGeneratorVisibility(SubForms.BatchComponentListFragmentsForm, false);
                    break;
                case RegUtilities.FragmentForm.Hidden:
                    SetFormGeneratorVisibility(SubForms.BatchComponentFragmentsForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentListFragmentsForm, false);
                    break;
                case RegUtilities.FragmentForm.ReadOnly:
                    _mixtureCOEFormGroup.GetSubForm(GetFormIndex(
                        SubForms.BatchComponentFragmentsForm)).DisplayMode = FormGroup.DisplayMode.View;
                    SetFormGeneratorVisibility(SubForms.BatchComponentFragmentsForm, true);
                    SetFormGeneratorVisibility(SubForms.BatchComponentListFragmentsForm, true);

                    break;
            }
        }

        private Boolean isEditable(Component component)
        {
            return string.IsNullOrEmpty(component.Compound.RegNumber.RegNum);
        }

        private void DeleteComponent()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            if (this.RegistryRecord.ComponentList.Count > minComponents)
            {
                int componentIndex = _mixtureCOEFormGroup.GetFormGeneratorPageIndex(GetFormIndex(SubForms.ComponentForm));
                this.RegistryRecord.DeleteComponent(componentIndex);

                componentIndex = Math.Min(componentIndex, this.RegistryRecord.ComponentList.Count - 1);
                this.DisplayComponent(componentIndex);
                this.DisplayCompoundIntoTree(this.RegistryRecord, "1|" + componentIndex.ToString());
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        private void AddComponent()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            _mixtureCOEFormGroup.Update();

            this.RegistryRecord.AddComponent(Component.NewComponent());

            DisplayComponent(this.RegistryRecord.ComponentList.Count - 1);
            if (CurrentRegistryType == RegistryTypes.Mixture)
            { //here we go directly to add a component
                SearchCompound(this.RegistryRecord.ComponentList.Count - 1);
            }
            else
            {
                this.DisplayCompoundIntoTree(this.RegistryRecord, NodeKeywords.LastComponent.ToString());


                /*if (this.RegistryRecord.ComponentList.Count >= minComponents)
                    this.DoneAddingButton.Enabled = true;*/

                this.SetPageMode(PageState.AddComponent, string.Empty);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        private bool ReplaceExistingComponent(string compoundIdToAdd, string regIdToSearch)
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                           MethodBase.GetCurrentMethod().Name);
                if (Request[RegistrationWebApp.Constants.RegisteredCompoundId_UrlParameter] != null && Request[RegistrationWebApp.Constants.RegisteredRegId_UrlParameter] != null)
                {
                    int index = Session["currentCompoundIndex"] != null ? (int)Session["currentCompoundIndex"] : 0;
                    Session["currentCompoundIndex"] = null;
                    Compound compound = null;
                    if (!string.IsNullOrEmpty(compoundIdToAdd) && !string.IsNullOrEmpty(regIdToSearch))
                    {
                        compound = this.GetCompound(regIdToSearch);
                    }

                    this.RegistryRecord.ReplaceCompound(index, compound);
                    DisplayComponent(index);
                    return true;
                }
                else if (Session["currentCompoundIndex"] != null)
                {
                    int index = Session["currentCompoundIndex"] != null ? (int)Session["currentCompoundIndex"] : 0;
                    Session["currentCompoundIndex"] = null;

                    DisplayComponent(index);
                    return true;
                }

                SetButtonStates();


                return false;
            }
            finally
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                           MethodBase.GetCurrentMethod().Name);
            }
        }

        private Compound GetCompound(string regNumToSearch)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            string filter = "<REGISTRYLIST><REGNUMBER>";
            filter += regNumToSearch;
            filter += "</REGNUMBER></REGISTRYLIST>";

            CompoundList compounds = CompoundList.GetCompoundList(filter);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
            if (compounds.Count > 0)
            {
                return compounds[0];
            }
            else
                return null;
        }

        private void SearchCompound()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                        MethodBase.GetCurrentMethod().Name);
            _mixtureCOEFormGroup.Update();
            // Pick an existing Component will not be replaced by selected component in treeview
            if (AllowUnregisteredComponents)
            {
                this.RegistryRecord.AddComponent(Component.NewComponent());
                DisplayComponent(this.RegistryRecord.ComponentList.Count - 1);
            }
            Session["currentCompoundIndex"] = _mixtureCOEFormGroup.GetFormGeneratorPageIndex(GetFormIndex(SubForms.ComponentForm));

            Server.Transfer(Resource.SearchCompound_Page_URL + "?caller=SR", false);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        private void SearchCompound(int index)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                        MethodBase.GetCurrentMethod().Name);
            //_mixtureCOEFormGroup.Update();
            Session["currentCompoundIndex"] = index;

            Server.Transfer(Resource.SearchCompound_Page_URL + "?caller=SR", false);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }




        /// <summary>
        /// This method handles all the logic after a Submit Button was clicked
        /// </summary>
        private void SubmitMixture()
        {
            //tracing
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            _mixtureCOEFormGroup.Update();
            
            if (this.RegistryRecord.IsValid)
            {
                this.RegistryRecord.ModuleName = ChemDrawWarningChecker.ModuleName.REGISTRATION;
                if (IsPreRegistration)
                {

                    this.RegistryRecord = RegistryRecord.Save(DuplicateCheck.PreReg);
                    if (!this.RegistryRecord.IsValid)
                    {
                        DisplayBrokenRules(this.RegistryRecord.GetBrokenRulesDescription());
                        this.RegistryRecord.ClearRedBoxWarningSetting();
                        SetPageMode(PageState.EditSingleCompound, null);
                    }
                    else
                    {
                        this.HandleDuplicates(this.RegistryRecord.FoundDuplicates, false);
                        this.AlreadySubmitted = true;
                        DisplayCompoundIntoTree(this.RegistryRecord, null);

                        switch (this.CurrentRegistryType)
                        {
                            case RegistryTypes.Both:
                                this.SetPageMode(PageState.End, string.Format(Resource.SubmittedRecord_Label_Text,
                                           this.RegistryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                              GUIShellTypes.IdsPaddingCharacter)));
                                break;
                            case RegistryTypes.Mixture:
                                this.SetPageMode(PageState.End, string.Format(Resource.SubmittedMixture_Label_Text,
                                          this.RegistryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                             GUIShellTypes.IdsPaddingCharacter)));
                                break;
                            case RegistryTypes.Component:
                                this.SetPageMode(PageState.End, string.Format(Resource.SubmittedComponent_Label_Text,
                                           this.RegistryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                              GUIShellTypes.IdsPaddingCharacter)));
                                break;
                        }
                    }
                }
                else
                {                   
                    this.RegistryRecord = this.RegistryRecord.Save();                    
                    if (!this.RegistryRecord.IsValid)                    
                    {                       
                        DisplayBrokenRules(this.RegistryRecord.GetBrokenRulesDescription());
                        this.RegistryRecord.ClearRedBoxWarningSetting();                      
                        SetPageMode(PageState.EditSingleCompound, null);
                        
                    }
                    else
                    {
                        this.AlreadySubmitted = true;
                        DisplayCompoundIntoTree(this.RegistryRecord, null);

                        switch (this.CurrentRegistryType)
                        {
                            case RegistryTypes.Both:
                                this.SetPageMode(PageState.End, string.Format(Resource.SubmittedRecord_Label_Text,
                                           this.RegistryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                        GUIShellTypes.IdsPaddingCharacter)));
                                break;
                            case RegistryTypes.Mixture:
                                this.SetPageMode(PageState.End, string.Format(Resource.SubmittedMixture_Label_Text,
                                    this.RegistryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                    GUIShellTypes.IdsPaddingCharacter)));
                                break;
                            case RegistryTypes.Component:
                                this.SetPageMode(PageState.End, string.Format(Resource.SubmittedComponent_Label_Text,
                                           this.RegistryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                         GUIShellTypes.IdsPaddingCharacter)));
                                break;
                        }
                    }
                }
            }
            else
            {
                DisplayBrokenRules(this.RegistryRecord.GetBrokenRulesDescription());
                SetPageMode(PageState.EditSingleCompound, null);
            }


            //tracing
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
       

        /// <summary>
        /// Assembles the messages from all broken rules into a single string for display.
        /// </summary>
        /// <param name="brokenRules">the collection of broken rules to assess</param>
        /// <returns>a formatted string of one or more validation messages</returns>
        private string ExtractBrokenRules(List<BrokenRuleDescription> brokenRules)
        {
            string errorMessage = string.Empty;
            foreach (BrokenRuleDescription currentBrokenRule in brokenRules)
            {
                foreach (string currentError in currentBrokenRule.BrokenRulesMessages)
                    errorMessage += string.Format("{0}<br>", currentError);
            }
            return errorMessage;
        }

        private void DisplayBrokenRules(List<BrokenRuleDescription> brokenRules)
        {
            this.DisplayBrokenRules(null, brokenRules);
        }

        private void DisplayBrokenRules(ValidationException exception, List<BrokenRuleDescription> brokenRules)
        {
            string brokenRulesMessage = (exception == null ? string.Empty : exception.Message + "<br/>") + ExtractBrokenRules(brokenRules);
            _masterPage.DisplayErrorMessage(brokenRulesMessage, false);
        }

        /// <summary>
        /// This method handles all the logic after a Save Button was clicked
        /// </summary>
        private void SaveMixture()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);

            try
            {
                _mixtureCOEFormGroup.Update();

                if (RegistryRecord.IsDirty)
                {
                    if (Request.Params[Constants.SavedObjectId_UrlParameter] != null && int.Parse(Request.Params[Constants.SavedObjectId_UrlParameter]) > 0)
                    {
                        string genericObjectID = Request[Constants.SavedObjectId_UrlParameter];
                        if (CheckTemplateCreator(genericObjectID))
                        {
                            ConfirmationMessageBox.Attributes["MessageBoxMode"] = MessageBoxMode.MessageBox.ToString();
                            ConfirmationMessageBox.Show(Resource.OverwriteForm_Msg_Text, Resource.OverwriteForm_Msg_Title, Resource.Yes_Button_Text, Resource.No_Button_Text, Resource.Cancel_Button_Text);
                        }
                        else
                        {
                            _masterPage.DisplayErrorMessage(Resource.InvalidUser_To_SaveTemplate, true);
                        }
                    }
                    else
                    {
                        Server.Transfer(string.Format("{0}?{1}={2}&{3}={4}&{5}={6}", Resource.SaveRegistryForm_Page_URL, RegistrationWebApp.Constants.FormGroup_UrlParameter, (int)FormGroups.MultiCompound, RegistrationWebApp.Constants.CurrentPageState_UrlParameter, CurrentPageState, RegistrationWebApp.Constants.RegistryTypeParameter, CurrentRegistryType), false);
                    }
                }
                else
                {
                    ShowConfirmationMessage(Resource.NoDataToSave_MessagesArea);
                }
            }
            catch (Exception exception)
            {
                if (ExceptionPolicy.HandleException(exception, Constants.REG_GUI_POLICY))
                    throw;
                else
                    _masterPage.DisplayErrorMessage(exception, false);
            }
            finally
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                           MethodBase.GetCurrentMethod().Name);
            }
        }
        /// <summary>
        /// To check the creator of Template
        /// </summary>
        private bool CheckTemplateCreator(string genericObjectId)
        {
            COEGenericObjectStorageBO template = null;
            template = COEGenericObjectStorageBO.Get(int.Parse(genericObjectId));
            string user = (COEUser.Get() == null || COEUser.Get() == string.Empty) ?
                    "unknown" : COEUser.Get();
            if (template.UserName.Trim().ToUpper() != user.Trim().ToUpper())
                return false;
            return true;
        }


        /// <summary>
        /// Handle the returned message by the DB
        /// </summary>
        /// <param name="duplicates">Message from the Db regarding duplicates</param>
        private void HandleDuplicates(string duplicates, bool isRegistration)
        {

            switch (RegUtilities.HandleDuplicates(RegistryRecord, duplicates, IsPreRegistration && !isRegistration))
            {
                case RegUtilities.DuplicateType.Compound:
                    if (IsPreRegistration && !isRegistration)
                    {
                        Server.Transfer(string.Format("{0}?{1}={2}&DupCheckOnSubmit=true", Resource.ComponentDuplicates_URL, Constants.CurrentPageState_UrlParameter, "endNewRegistry"), false);
                    }
                    else
                    {
                        Server.Transfer(string.Format("{0}?{1}={2}", Resource.ComponentDuplicates_URL, Constants.CurrentPageState_UrlParameter, "endNewRegistry"), false);
                    }
                    break;
                case RegUtilities.DuplicateType.Mixture:
                    if (IsPreRegistration && !isRegistration)
                    {
                        Server.Transfer(string.Format("{0}?{1}={2}&DupCheckOnSubmit=true", Resource.RegistryDuplicates_Page_URL, Constants.CurrentPageState_UrlParameter, "endNewRegistry"), false);
                    }
                    else
                    {
                        Server.Transfer(string.Format("{0}?{1}={2}", Resource.RegistryDuplicates_Page_URL, Constants.CurrentPageState_UrlParameter, "endNewRegistry"), false);

                    }
                    break;
                case RegUtilities.DuplicateType.None:

                    this.SetPageMode(PageState.End, string.Empty);

                    break;
            }
        }



        /// <summary>
        /// Submit a registration to the permanent registry.
        /// </summary>
        private void RegisterMixture()
        {
            //tracing
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            _mixtureCOEFormGroup.Update();

            if (this.RegistryRecord.IsValid)
            {
                this.RegistryRecord.ModuleName = ChemDrawWarningChecker.ModuleName.REGISTRATION;

                this.RegistryRecord.IsDirectReg = true;
                this.RegistryRecord = this.RegistryRecord.Register();
             
                if(!this.RegistryRecord.IsValid)
                {
                    this.RegistryRecord.IsDirectReg = false;
                    
                    DisplayBrokenRules(this.RegistryRecord.GetBrokenRulesDescription());
                    this.RegistryRecord.ClearRedBoxWarningSetting();
                    SetPageMode(PageState.EditSingleCompound, null);                  
                }
                else
                {
                    this.HandleDuplicates(this.RegistryRecord.FoundDuplicates, true);
                }


            }
            else
            {
                DisplayBrokenRules(this.RegistryRecord.GetBrokenRulesDescription());
                SetPageMode(PageState.EditSingleCompound, null);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }


        private void HideButtons()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);

            this.SubmissionTemplateImageMenuButton.MenuItemList[0].Enabled = false;
            this.SubmissionTemplateImageMenuButton.MenuItemList[1].Enabled = false;

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Method to display confirmation messages in the top of the page (MessagesAreaUC)
        /// </summary>
        /// <param name="messageToDisplay">The text to display</param>
        private void ShowConfirmationMessage(string messageToDisplay)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            if (!string.IsNullOrEmpty(messageToDisplay))
            {
                this.MessagesAreaUserControl.AreaText = messageToDisplay;
                this.MessagesAreaUserControl.Visible = true;
                this.MessagesAreaRow.Visible = true;
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        private void HideConfirmationMessage()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            if (MessagesAreaRow.Visible)
            {
                this.MessagesAreaUserControl.AreaText = string.Empty;
                this.MessagesAreaUserControl.Visible = false;
                this.MessagesAreaRow.Visible = false;
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        private bool RestoreSavedObject()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);

            try
            {
                string genericObjectID = Request[Constants.SavedObjectId_UrlParameter];
                if (!string.IsNullOrEmpty(genericObjectID))
                {
                    if (int.Parse(genericObjectID) > 0)
                    {
                        string _databaseName = COEConfiguration.GetDatabaseNameFromAppName(CambridgeSoft.COE.Framework.Common.COEAppName.Get().ToString());
                        COEGenericObjectStorageBO genericStorageBO = COEGenericObjectStorageBO.Get(int.Parse(genericObjectID));
                        //if this is coming from a stored template - reset the default scientistID
                        string COEGenericObjectXml = genericStorageBO.COEGenericObject;
                        RestoreDefaultBatchPropertyValue("SCIENTIST_ID", COEUser.ID.ToString(), ref COEGenericObjectXml);

                        RegistryRecord record = RegistryRecord.NewRegistryRecord();
                        record.InitializeFromXml(COEGenericObjectXml, true, false);

                        if (record.ComponentList.Count > 1)
                        {
                            if (AllowUnregisteredComponents == false)
                                if (record.ComponentList[0].Compound.RegNumber.RegNum != String.Empty)
                                    CurrentRegistryType = RegistryTypes.Mixture;
                                else
                                {
                                    record = this.RegistryRecord; // Set to previous state.
                                    this._masterPage.DisplayErrorMessage(Resource.UnregisteredComponents_Message, false);
                                }
                            else

                                CurrentRegistryType = RegistryTypes.Both;
                        }
                        else
                            CurrentRegistryType = RegistryTypes.Component;

                        this.RegistryRecord = record; // If flow is good accept the new one or update with the previous flow.
                        this.RegistryRecord.UpdateDrawingType();  // Added updatedrawing type to set the structure drawing type of temp compounds.
                        if (Request[RegistrationWebApp.Constants.CurrentPageState_UrlParameter] != null)
                        {
                            if (Enum.IsDefined(typeof(PageState), Request[RegistrationWebApp.Constants.CurrentPageState_UrlParameter].ToString()))
                            {

                                this.SetPageMode((PageState)Enum.Parse(typeof(PageState), Request[RegistrationWebApp.Constants.CurrentPageState_UrlParameter].ToString(), true), string.Empty);
                            }
                        }
                        else
                        {
                            this.DisplayComponent(0);
                            this.DisplayCompoundIntoTree(record);
                        }

                        UpdateDrawingTypeControl();

                        if (Page.PreviousPage != null)
                        {
                            if (Page.PreviousPage.ToString().ToUpper().Equals(GUIShellTypes.SavedCompoundPage))
                            {
                                string descriptionSummary = String.Empty;
                                if (((COEGenericObjectStorageBO)genericStorageBO).Name.Length > GUIShellTypes.DescriptionFieldSummaryLenght)
                                    descriptionSummary = ((COEGenericObjectStorageBO)genericStorageBO).Name.Substring(0, GUIShellTypes.DescriptionFieldSummaryLenght).ToString() + "...";
                                else
                                    descriptionSummary = ((COEGenericObjectStorageBO)genericStorageBO).Name.ToString();

                                switch (this.CurrentRegistryType)
                                {
                                    case RegistryTypes.Both:
                                        this.ShowConfirmationMessage(string.Format(Resource.SavedRecord_Label_Text, descriptionSummary));
                                        break;
                                    case RegistryTypes.Mixture:
                                        this.ShowConfirmationMessage(string.Format(Resource.SavedMixture_Label_Text, descriptionSummary));
                                        break;
                                    case RegistryTypes.Component:
                                        this.ShowConfirmationMessage(string.Format(Resource.SavedComponent_Label_Text, descriptionSummary));
                                        break;
                                }

                            }
                            else if (Page.PreviousPage.ToString().ToUpper().Equals(GUIShellTypes.LoadCompoundPage))
                            {
                                string descriptionSummary = String.Empty;
                                if (((COEGenericObjectStorageBO)genericStorageBO).Name.Length > GUIShellTypes.DescriptionFieldSummaryLenght)
                                    descriptionSummary = ((COEGenericObjectStorageBO)genericStorageBO).Name.Substring(0, GUIShellTypes.DescriptionFieldSummaryLenght).ToString() + "...";
                                else
                                    descriptionSummary = ((COEGenericObjectStorageBO)genericStorageBO).Name.ToString();

                            }
                        }
                    }
                    if (CurrentRegistryType != RegistryTypes.Component)
                        _restoreSavedMixture = true;
                    return true;
                }

                return false;

            }
            catch (Exception exception)
            {
                if (ExceptionPolicy.HandleException(exception, Constants.REG_GUI_POLICY))
                    throw;
                else
                    _masterPage.DisplayErrorMessage(exception, false);

                return false;
            }
            finally
            {

                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                           MethodBase.GetCurrentMethod().Name);
            }
        }

        /// <summary>
        /// Method to pull the user preference if its already saved.
        /// </summary>
        private void  RestoreUserPreference()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                COEGenericObjectStorageBO genericStorageBO = COEGenericObjectStorageBO.Get(COEUser.Name, (int)FormGroups.UserPreference);
                if (genericStorageBO != null && genericStorageBO.ID > 0)
                {
                    string COEGenericObjectXml = genericStorageBO.COEGenericObject;
                    RegistryRecord record = RegistryRecord.NewRegistryRecord();
                    record.InitializeFromXml(COEGenericObjectXml, true, false);
                    CurrentRegistryType = RegistryTypes.Component;
                    this.RegistryRecord = record; // If flow is good accept the new one or update with the previous flow.
                }
                SetPageMode(PageState.DisplayUserPreference, string.Empty);
            }
            catch (Exception exception)
            {
                if (ExceptionPolicy.HandleException(exception, Constants.REG_GUI_POLICY))
                    throw;
                else
                    _masterPage.DisplayErrorMessage(exception, false);

                return;
            }
            finally
            {

                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                           MethodBase.GetCurrentMethod().Name);
            }
        }

        /// <summary>
        /// Method to Assign DrawingTypeValue to hiddenfield [This is were COEDataMapper uses to identify the drawing type selected by the user].
        /// <remarks>Ensure Databind is completed for formgenerator [Because you can find controls after (Update(), DataBind())] only.</remarks>
        /// </summary>
        private void UpdateDrawingTypeControl()
        {
            try
            {

                if (CurrentRegistryType == RegistryTypes.Component && this.RegistryRecord.ComponentList.Count == 1)
                {
                    string drawingTypeValue = string.Empty;
                    Structure structure = this.RegistryRecord.ComponentList[0].Compound.BaseFragment.Structure;
                    switch (structure.DrawingType)
                    {
                        case DrawingType.Chemical:
                            drawingTypeValue = DrawingType.Chemical.ToString();
                            break;
                        case DrawingType.NonChemicalContent:
                            drawingTypeValue = "-" + ((int)DrawingType.NonChemicalContent).ToString();
                            break;
                        case DrawingType.NoStructure:
                            drawingTypeValue = "-" + ((int)DrawingType.NoStructure).ToString();
                            break;
                        case DrawingType.Unknown:
                            drawingTypeValue = "-" + ((int)DrawingType.Unknown).ToString();
                            break;
                    }
                    ControlCollection webcontrolCollection = ((COEFormGenerator)_mixtureCOEFormGroup.FindControl(this.GetFormIndex(SubForms.ComponentForm))).FindControl("ListStructureIDCheckBox").Controls;
                    for (int j = 0; j <= webcontrolCollection.Count - 1; j++)
                    {
                        if (webcontrolCollection[j] != null)
                        {
                            if (webcontrolCollection[j] is HtmlInputHidden)
                            {
                                HtmlInputHidden htmlInputHidden = (HtmlInputHidden)webcontrolCollection[j];
                                htmlInputHidden.Value = drawingTypeValue;
                                break;
                            }
                        }
                    }
                }

            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);

            }
            finally
            {

                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                           MethodBase.GetCurrentMethod().Name);
            }
        }

        private bool RestoreSessionObject()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                    MethodBase.GetCurrentMethod().Name);

            try
            {
                if (Request[Resource.RestoreSessionObject_URLParameter] != null && Request[Resource.RestoreSessionObject_URLParameter] == bool.TrueString)
                {
                    this.RegistryRecord.CleanFoundDuplicates();
                    this.DisplayComponent(0);
                    this.DisplayCompoundIntoTree(this.RegistryRecord);
                    UpdateDrawingTypeControl();
                    return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                if (ExceptionPolicy.HandleException(exception, Constants.REG_GUI_POLICY))
                    throw;
                else
                    _masterPage.DisplayErrorMessage(exception, false);

                return false;
            }
            finally
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                           MethodBase.GetCurrentMethod().Name);
            }

        }

        private void LeavePage()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            Session.Remove(RegistrationWebApp.Constants.MultiCompoundObject_Session);

            string url = this._masterPage.AccordionControl.Groups.FromKey(this.AccordionGroupToListen).TargetUrl;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
            Server.Transfer(url, false);
        }

        private void DisplayRecord()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            if (AlreadySubmitted)//Display in ViewMode
                this.SetPageMode(PageState.End, string.Empty);
            else
            {
                if (!IsAMixture)
                    this.SetPageMode(PageState.AddSingleCompound, string.Empty);
                else
                    if (CurrentRegistryType == RegistryTypes.Mixture)
                    {
                        this.SetPageMode(PageState.AddSingleCompound, string.Empty);
                    }
                    else
                    {
                        this.SetPageMode(PageState.AddComponent, string.Empty);
                    }
            }
            _mixtureCOEFormGroup.DataBind();
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Method to check if this particular Component is Editable
        /// </summary>
        /// <param name="currentComponent">Component to check</param>
        /// <returns>Boolean indicating if it is editable</returns>
        /// <remarks>Note already registered Registries cannot be edited</remarks>
        private bool IsEditable(Component currentComponent)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            bool retVal = true;
            retVal = string.IsNullOrEmpty(currentComponent.Compound.RegNumber.RegNum) ? true : currentComponent.Compound.RegNumber.RegNum.ToUpper() == "N/A";
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
            return retVal;

        }

        private void DisplayComponent(int index)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            Component currentComponent = this.RegistryRecord.ComponentList[index];

            if (AlreadySubmitted)
            {
                SetPageMode(PageState.EndComponent, String.Empty);
            }
            else
            {
                if (!this.IsEditable(currentComponent))
                {
                    SetPageMode(PageState.ViewComponent, String.Empty);
                    this.ComponentToolBar.Visible = true;
                    //this.ContinueButton.Visible = true;
                    this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = (RegistryRecord.ComponentList.Count > 1 ? true : false);
                    // Fix for CSBR 161699- Issue with Pick Component link on Mixtures form
                    // Fix for CBOE-1427, Componet count condtion should be checked for all configuration setting in Mixture. i.e AllowUnregisteredComponents = true/false                    
                     this.DefineMixtureImageMenuButton.MenuItemList[3].Enabled = (RegistryRecord.ComponentList.Count > 1 ? true : false); //Continue
                    
                    SetButtonStates();
                }
                else
                {
                    if (this.RegistryRecord.ComponentList.Count == 1)
                    {

                        if (CurrentRegistryType == RegistryTypes.Mixture)
                        {
                            if (currentComponent.IsNew && !currentComponent.IsDirty)
                                SetPageMode(PageState.AddSingleCompound, string.Empty);
                            else
                            {
                                SetPageMode(PageState.EditSingleCompound, String.Empty);
                            }
                        }
                        else
                        {

                            SetPageMode(PageState.EditSingleCompound, string.Empty);
                        }
                    }
                    else
                    {
                        if (CurrentRegistryType == RegistryTypes.Mixture)
                        {
                            SetPageMode(PageState.AddSingleCompound, string.Empty);
                        }
                        else
                        {
                            if (currentComponent.IsNew && !currentComponent.IsDirty)
                                SetPageMode(PageState.AddComponent, string.Empty);

                            else
                                SetPageMode(PageState.EditComponent, String.Empty);
                        }
                    }
                }
            }

            SetFormGeneratorPageIndex(SubForms.ComponentForm, index);
            SetFormGeneratorPageIndex(SubForms.CompoundCustomProperties, index);
            SetFormGeneratorPageIndex(SubForms.SubmittedComponentForm, index);
            SetFormGeneratorPageIndex(SubForms.BatchComponentFragmentsForm, index);
            _mixtureCOEFormGroup.DataBind();
            this.DisplayCompoundIntoTree(this.RegistryRecord, "1|" + index.ToString());
            //this.DisplayCompoundIntoTree(this.RegistryRecord, NodeKeywords.LastComponent.ToString());
            // CBOE-1924, Drawing Type should be preserved for all components while submitting Mixture
            if (currentComponent != null)
            {
                this.structureToolbarOption.Value = ((int)currentComponent.Compound.BaseFragment.Structure.DrawingType).ToString();
                this.RegistryRecord.UpdateDrawingType();
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        private void DisplayBatch(int index)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            Batch currentBatch = this.RegistryRecord.BatchList[index];

            if (CurrentPageState != PageState.EndComponent)
            {
                if (currentBatch.IsNew && !currentBatch.IsDirty)
                    SetPageMode(PageState.AddBatch, String.Empty);
                else if (!currentBatch.IsNew)
                    SetPageMode(PageState.ViewBatch, String.Empty);
                else
                    SetPageMode(PageState.EditBatch, String.Empty);
            }
            else
            {
                this.SetPageMode(PageState.ViewBatch, string.Format(Resource.MixtureSuccessfullySubmited_Label_Text,
                                                        this.RegistryRecord.BatchList[this.RegistryRecord.BatchList.Count - 1].ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers, GUIShellTypes.IdsPaddingCharacter)));
            }

            SetFormGeneratorPageIndex(SubForms.BatchForm, index);
            SetFormGeneratorPageIndex(SubForms.BatchCustomProperties, index);
            SetFormGeneratorPageIndex(SubForms.BatchComponentForm, index);
            SetFormGeneratorPageIndex(SubForms.BatchComponentCustomProperties, index);

            this.DisplayCompoundIntoTree(this.RegistryRecord, NodeKeywords.FirstBatch.ToString());
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void SetFormGeneratorPageIndex(SubForms subform, int pageIndex)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);

            if (_mixtureCOEFormGroup != null)
                _mixtureCOEFormGroup.SetFormGeneratorPageIndex(GetFormIndex(subform), pageIndex);

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        private void SetFormGeneratorViewState(SubForms subform, bool enable)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);

            if (_mixtureCOEFormGroup != null)
                _mixtureCOEFormGroup.SetFormGeneratorViewState(GetFormIndex(subform), enable);

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        private void SetFormGeneratorVisibility(SubForms subform, bool visible)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);

            if (_mixtureCOEFormGroup != null)
                _mixtureCOEFormGroup.SetFormGeneratorVisibility(GetFormIndex(subform), visible);

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        private int GetFormIndex(SubForms subform)
        {
            return (int)subform;
        }

        public void RenderJSArrayWithCliendIds()
        {
            StringBuilder arrClientIDValue = new StringBuilder();
            StringBuilder arrServerIDValue = new StringBuilder();
            StringBuilder sStartupRunScripts = new StringBuilder();
            ClientScriptManager cs = Page.ClientScript;

            getSubcontrols(this.mixtureInformationHolder, ref arrClientIDValue, ref arrServerIDValue, ref sStartupRunScripts);

            // Register the array of client and server id to the client
            cs.RegisterArrayDeclaration("ClientIDList", arrClientIDValue.ToString().Remove(arrClientIDValue.ToString().Length - 1, 1));
            cs.RegisterArrayDeclaration("ServerIDList", arrServerIDValue.ToString().Remove(arrServerIDValue.ToString().Length - 1, 1));
            cs.RegisterStartupScript(typeof(SubmitMixturePage), "EnableDisaable", sStartupRunScripts.ToString(), true);

        }
        public void getSubcontrols(Control curControl, ref StringBuilder arClientID, ref StringBuilder arServerId, ref StringBuilder StartupRunScripts)
        {
            foreach (Control cntrl in curControl.Controls)
            {

                if (cntrl is COEDatePicker)
                {
                    arClientID.Append("\"" + cntrl.ClientID + "_input" + "\",");
                    arServerId.Append("\"" + cntrl.ID + "\",");
                }
                else if (cntrl.Controls.Count > 0)
                    getSubcontrols(cntrl, ref arClientID, ref arServerId, ref StartupRunScripts);
                else if (cntrl is ICOEGenerableControl)
                {
                    arClientID.Append("\"" + cntrl.ClientID + "\",");
                    arServerId.Append("\"" + cntrl.ID + "\",");
                    if (cntrl is WebControl)
                    {
                        WebControl webCurrent = (WebControl)cntrl;
                        if (webCurrent.Attributes["onclick"] != null && webCurrent.Attributes["onclick"].StartsWith("CheckBox_EnableFields"))
                            StartupRunScripts.Append(webCurrent.Attributes["onclick"].Replace("this.id", "'" + webCurrent.ClientID + "'"));
                    }
                }

            }
        }
        #endregion

        #region CSLA Datasources Events
        protected void mixtureCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = this.RegistryRecord;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);

        }

        protected void mixtureCslaDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);

            try
            {
                //First we have to map those control that support to create "virtual" objects client side, so then we can bind the values easily.
                //Create as many project as created by the control. 
                string listName = RegUtilities.FindKey("ProjectList", e.Values.Keys);

                if (e.Values[listName] is DataTable)
                    RegUtilities.SincronizeListDataTable((DataTable)e.Values[listName], "ProjectID", this.RegistryRecord.ProjectList, Project.NewProject);


                //Create as many identifiers as created by the control in client side. 
                listName = RegUtilities.FindKey("IdentifierList", e.Values.Keys);
                ;
                if (e.Values[listName] is DataTable)
                    RegUtilities.SincronizeListDataTable((DataTable)e.Values[listName], "IdentifierID", this.RegistryRecord.IdentifierList, Identifier.NewIdentifier);

                COEDataMapper.Map(e.Values, this.RegistryRecord);
                e.RowsAffected = 1;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void BatchListCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = this.RegistryRecord.BatchList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void BatchListCslaDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                Batch currentBatch = this.RegistryRecord.BatchList[(int)e.Keys["PageIndex"]];

                string listName = RegUtilities.FindKey("ProjectList", e.Values.Keys);
                if (e.Values[listName] is DataTable)
                    RegUtilities.SincronizeListDataTable(((DataTable)e.Values[listName]), "ProjectID", currentBatch.ProjectList, Project.NewProject);

                listName = RegUtilities.FindKey("IdentifierList", e.Values.Keys);
                ;
                if (e.Values[listName] is DataTable)
                    RegUtilities.SincronizeListDataTable(((DataTable)e.Values[listName]), "IdentifierID", currentBatch.IdentifierList, Identifier.NewIdentifier);


                COEDataMapper.Map(e.Values, currentBatch);
                e.RowsAffected = 1;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void BatchComponentListCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = this.RegistryRecord.BatchList[this._mixtureCOEFormGroup.GetSubForm(this.GetFormIndex(SubForms.BatchForm)).PageIndex].BatchComponentList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void BatchComponentListCslaDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                BatchComponent currentBatchComponent = this.RegistryRecord.BatchList[this._mixtureCOEFormGroup.GetSubForm(this.GetFormIndex(SubForms.BatchForm)).PageIndex].BatchComponentList[(int)e.Keys["PageIndex"]];

                COEDataMapper.Map(e.Values, currentBatchComponent);

                e.RowsAffected = 1;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void ComponentListCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = this.RegistryRecord.ComponentList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void ComponentListCslaDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                Component currentComponent = this.RegistryRecord.ComponentList[(int)e.Keys["PageIndex"]];

                string listName = RegUtilities.FindKey("Compound.IdentifierList", e.Values.Keys);
                //if (e.Values[listName] is DataTable)
                RegUtilities.SincronizeListDataTable(((DataTable)e.Values[listName]), "IdentifierID", currentComponent.Compound.IdentifierList, Identifier.NewIdentifier);

                listName = RegUtilities.FindKey("Compound.BaseFragment.Structure.IdentifierList", e.Values.Keys);
                RegUtilities.SincronizeListDataTable(((DataTable)e.Values[listName]), "IdentifierID", currentComponent.Compound.BaseFragment.Structure.IdentifierList, Identifier.NewIdentifier);

                COEDataMapper.Map(e.Values, currentComponent);
                e.RowsAffected = 1;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }


        delegate Object CreateNewListElement();

        Csla.Core.IBusinessObject GetListElement(int id, string idPrimaryKey, IList list)
        {
            IEnumerator e = list.GetEnumerator();

            COEDataBinder databinder = new COEDataBinder(null);

            while (e.MoveNext())
            {
                databinder.RootObject = e.Current;
                int key = (int)databinder.RetrieveProperty(idPrimaryKey);
                if (key == id)
                    return (Csla.Core.IBusinessObject)e.Current;
            }

            return null;

        }
        protected void prefixDropDownCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = PrefixNameValueList.GetPrefixNameValueList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Returns the list of active projects filter by userID and registry type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ProjectsCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = ProjectList.GetActiveProjectListByPersonID(COEUser.ID);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Returns the list of active projects filter by userID and registry type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RegistryProjectsCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = ProjectList.GetActiveProjectListByPersonIDAndType(COEUser.ID, ProjectList.ProjectTypeEnum.R);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Returns the list of active projects filter by userID and batch type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BatchProjectsCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = ProjectList.GetActiveProjectListByPersonIDAndType(COEUser.ID, ProjectList.ProjectTypeEnum.B);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void IdentifiersCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = IdentifierList.GetIdentifierList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void RegistryIdentifiersCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.R);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void CompoundIdentifiersCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void StructureIdentifiersCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.S);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void BatchIdentifiersCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.B);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void ChemistsCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = ChemistNameValueList.GetActiveChemistNameValueList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void FragmentTypesCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = FragmentNameValueList.GetFragmentTypesNameValueList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void FragmentNamesCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = FragmentList.GetFragmentList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void FragmentsCslaDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            int batchIndex = this._mixtureCOEFormGroup.GetSubForm(this.GetFormIndex(SubForms.BatchForm)).PageIndex;
            int batchComponentIndex = (int)e.Keys["PageIndex"];
            try
            {
                string key = RegUtilities.FindKey("BatchComponentFragmentList", e.Values.Keys);
                if (e.Values[key] is DataTable)
                    RegUtilities.SincronizeListDataTable((DataTable)e.Values[key], "FragmentID", RegistryRecord.BatchList[batchIndex].BatchComponentList[batchComponentIndex].BatchComponentFragmentList, BatchComponentFragment.NewBatchComponentFragment);

                COEDataMapper.Map(e.Values, RegistryRecord.BatchList[batchIndex].BatchComponentList[batchComponentIndex]);
                //This call checks that all the fragments that are inside the BatchComponents also are part at Component level.
                this.RegistryRecord.UpdateFragments();
                e.RowsAffected = 1;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void FragmentsCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = RegistryRecord.BatchList[0].BatchComponentList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void BatchComponentFragmentsCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                int batchIndex = this._mixtureCOEFormGroup.GetSubForm(this.GetFormIndex(SubForms.BatchForm)).PageIndex;
                e.BusinessObject = RegistryRecord.BatchList[batchIndex];
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void SequenceListCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                //Get the list of the sequences to be aplied to a Registry.
                e.BusinessObject = SequenceList.GetSequenceList(1);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        #endregion

    }
}
