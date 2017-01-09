using System;
using System.Data;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Reflection;
using Resources;
using System.Collections.Generic;
using System.Xml;
using Infragistics.WebUI.UltraWebNavigator;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Infragistics.WebUI.UltraWebListbar;
using Csla;
using Csla.Validation;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEPickListPickerService;
using PerkinElmer.COE.Registration.Server.Forms.Master;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEGenericObjectStorageService;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.Common.Validation;
using PerkinElmer.COE.Registration.Server.Code;
using CambridgeSoft.COE.Registration;

namespace PerkinElmer.COE.Registration.Server.Forms.ReviewRegister.ContentArea
{
    public partial class ReviewRegisterMixture : GUIShellPage
    {
        #region GUIShell Variables
        RegistrationMaster _masterPage = null;
        private string _controlToListen = String.Empty;
        private string _groupToListen = String.Empty;
        RegistryTypes _currentRegistryType;   

        /// <summary>
        /// These are the controls which I can be interested in the Control(MyCustomizedControl)
        /// inside the Accordion Group.
        /// </summary>
        private enum ControlsToTakeCare
        {
            ShowButtonsHidden,
            MessageLabel,
            UltraWebTreeControl
        }

        /// <summary>
        /// Those are the events from the control inside the accordion that I'm interested.
        /// </summary>
        private enum EventsToListen
        {
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

        #region Page Variables
        private enum PageState
        {
            None,
            ViewRecord,
            EditRecord,
            ViewComponent,
            EditComponent,
            EndComponent,
            ViewBatch,
            EditBatch,
            End,
            ViewSingleComponent,
            AddComponent,
        }

        private enum SubForms
        {
            MixtureResumeForm = 0,
            ComponentForm = 1,
            SubmittedComponentForm = 2,
            BatchComponentForm = 3,
            BatchForm = 4,
            BatchComponentFragmentsForm = 5,
            BatchComponentFragmentsFormEdit = 6,
            //BatchComponentListFragmentsForm = 6,
            RegistryCustomProperties = 1000,
            CompoundCustomProperties = 1001,
            BatchCustomProperties = 1002,
            BatchComponentCustomProperties = 1003

        }
        COEFormGroup _mixtureCOEFormGroup;

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

        public bool AllowUnregisteredComponents
        {
            get
            {
                return RegUtilities.GetAllowUnregisteredComponents();
            }

        }
        public bool IsPreRegistration
        {
            get
            {

                return RegUtilities.GetEnableSubmissionDuplicateCheck();
            }

        }



        RegistryTypes CurrentRegistryType
        {
            get
            {

                if (this.RegistryRecord.ComponentList.Count > 1)
                {
                    if (AllowUnregisteredComponents)
                    {
                        _currentRegistryType = RegistryTypes.Both;
                    }
                    else
                    {
                        _currentRegistryType = RegistryTypes.Mixture;
                    }

                }
                else
                {
                    _currentRegistryType = RegistryTypes.Component;
                }

                return _currentRegistryType;

            }

        }

        public string SubmittedObjectId
        {
            get
            {
                return ViewState["SubmittedObjectId"] == null ? null : ViewState["SubmittedObjectId"].ToString();
            }
            set
            {
                ViewState["SubmittedObjectId"] = value;
            }
        }

        public int CurrentComponentIndex
        {
            get
            {
                return ViewState["CurrentComponentIndex"] == null ? -1 : int.Parse(ViewState["CurrentComponentIndex"].ToString());
            }
            set
            {
                ViewState["CurrentComponentIndex"] = value;
                this.ComponentIndexHiddenField.Value = value.ToString();
            }
        }

        /// <summary>
        /// If the Registry has a RegNumber it means is already Registered.
        /// </summary>
        private bool AlreadyRegistered
        {
            get
            {
                if (RegistryRecord == null)
                    return false;

                return !string.IsNullOrEmpty(RegistryRecord.RegNumber.RegNum);
            }
        }

        /// <summary>
        /// Based if the Session var exists and also if has a true value.
        /// </summary>
        private bool IsEditModeEnable
        {
            get
            {
                bool retVal = false;
                if (Session["EditRegistryModeEnable"] != null)
                    bool.TryParse(Session["EditRegistryModeEnable"].ToString(), out retVal);

                return retVal;
            }

            set
            {
                Session["EditRegistryModeEnable"] = value;
            }
        }
        //Fix for CSBR-149346 
        //In this bug the User Navigates to Search temp form after updates a Registered record insted Search Parmanent page.
        //Apart from this Bug, It rectified all flows from starting page to starting page in  Registration. 

        private bool IsEditStructureModeEnable
        {
            get
            {
                bool retVal = false;
                if (Session["EditRegistryStructureModeEnable"] != null)
                    bool.TryParse(Session["EditRegistryStructureModeEnable"].ToString(), out retVal);

                return retVal;
            }
            set
            {
                Session["EditRegistryStructureModeEnable"] = value;
            }
        }
        //Fix for CSBR-149346 
        //In this bug the User Navigates to Search temp form after updates a Registered record insted Search Parmanent page.
        //Apart from this Bug, It rectified all flows from starting page to starting page in  Registration. 
        private bool CheckIsSubmit
        {
            get
            {
                bool retVal = false;
                if (Session["isSubmit"] != null)
                {
                    retVal = Convert.ToBoolean(Session["isSubmit"].ToString());
                }
                return retVal;
            }
            set
            {
                Session["isSubmit"] = value;
            }
        }

        private RegistryRecord RegistryRecord
        {
            get
            {
                string requestSubmittedObjectID = Request[PerkinElmer.COE.Registration.Server.Constants.SubmittedObjectId_UrlParameter];
                if (Session[Constants.MultiCompoundObject_Session] == null || SubmittedObjectId != requestSubmittedObjectID)
                {
                    if (!string.IsNullOrEmpty(requestSubmittedObjectID))
                    {
                        if (RegUtilities.IsAValidTempId(requestSubmittedObjectID))
                        {
                            SubmittedObjectId = requestSubmittedObjectID;
                            Session[Constants.MultiCompoundObject_Session] = RegistryRecord.GetRegistryRecord(int.Parse(requestSubmittedObjectID));
                            //IsEditModeEnable = false;
                        }
                    }

                    if (Session[PerkinElmer.COE.Registration.Server.Constants.MultiCompoundObject_Session] == null)
                        _masterPage.DisplayErrorMessage(Resource.NotRecordFound_MasterPage, true);
                }

                return (RegistryRecord)Session[PerkinElmer.COE.Registration.Server.Constants.MultiCompoundObject_Session];
            }
            set
            {
                if (Session[Constants.MultiCompoundObject_Session] != null && !RegistryRecord.ReferenceEquals(value, Session[Constants.MultiCompoundObject_Session]))
                    ((IDisposable)Session[Constants.MultiCompoundObject_Session]).Dispose();

                Session[Constants.MultiCompoundObject_Session] = value;
            }
        }

        int PageFormGroupID
        {
            get
            {
                return RegUtilities.GetCOEFormID("ReviewRegisterRegistryFormGroupId");
            }
        }
        #endregion

        #region Page Properties

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

        #region Events Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                string currentMessage = string.Empty;
                switch (CurrentRegistryType)
                {
                    case RegistryTypes.Both:
                        currentMessage = Resource.ConfirmDeleteTempRegistry_Alert_Text;
                        break;
                    case RegistryTypes.Mixture:
                        currentMessage = Resource.ConfirmDeleteTempMixture_Alert_Text;
                        break;
                    case RegistryTypes.Component:
                        currentMessage = Resource.ConfirmDeleteTempComponent_Alert_Text;
                        break;
                }
                DeleteRecordImageMenuButton.OnClientClick = "return window.confirm('" + currentMessage + "');";
                this.MessagesAreaUserControl.Visible = false;
                this.CheckSearchParams();
                //Fix for CSBR-149346 
                //In this bug the User Navigates to Search temp form after updates a Registered record insted Search Parmanent page.
                //Apart from this Bug, It rectified all flows from starting page to starting page in  Registration. 
                this.CheckIsSubmitvalue();

                if (!Page.IsPostBack)
                {
                    this.IsEditModeEnable = false;
                    this.LoadControlInAccordionPanel();
                    this.SetControlsAttributtes();
                    if (Request[PerkinElmer.COE.Registration.Server.Constants.CurrentPageState_UrlParameter] != null)
                    {
                        string mode = Request[PerkinElmer.COE.Registration.Server.Constants.CurrentPageState_UrlParameter] as string;
                        if (Request[PerkinElmer.COE.Registration.Server.Constants.SubmittedObjectId_UrlParameter] != null)
                        {
                            if (mode.ToLower().Contains("edit"))
                                this.IsEditModeEnable = true;

                            if (((PageState)Enum.Parse(typeof(PageState), mode, true) == PageState.ViewBatch || (PageState)Enum.Parse(typeof(PageState), mode, true) == PageState.EditBatch) && this.RegistryRecord.ComponentList.Count < 2)
                            {
                                this.IsEditModeEnable = false;
                                this.SetPageMode(PageState.ViewRecord, Resource.ViewEditBatch_Mode_Message);
                            }
                            else
                            {
                                try
                                {
                                    this.SetPageMode((PageState)Enum.Parse(typeof(PageState), mode, true), string.Empty);
                                }
                                catch
                                {
                                    _masterPage.DisplayErrorMessage(Resource.CurrentPageState_ErrorMessage, false);
                                }
                            }
                        }
                        else
                            _masterPage.DisplayErrorMessage(Resource.SavedObjectId_Error_Message, false);
                    }
                    else
                    {
                        CurrentComponentIndex = 0;
                        this.DisplayRecord();
                    }
                    if (Request[PerkinElmer.COE.Registration.Server.Constants.RegisteredCompoundId_UrlParameter] != null)
                    {
                        this.ReplaceExistingComponent(Request[PerkinElmer.COE.Registration.Server.Constants.RegisteredCompoundId_UrlParameter], Request[PerkinElmer.COE.Registration.Server.Constants.RegisteredRegId_UrlParameter]);
                        IsEditModeEnable = true;
                        SetPageMode(PageState.ViewComponent, string.Empty);
                    }//Fix for CSBR 166611- Duplicate Resolution does not display exact Duplicates.
                    if ((Session[Constants.DuplicateIdsList_Session] != null) || (Session[Constants.DuplicateMultiCompounds_Session] != null))
                    {
                        Session.Remove(Constants.DuplicateIdsList_Session);
                        Session.Remove(Constants.DuplicateMultiCompounds_Session);
                    }
                    this.DisplayCompoundIntoTree(RegistryRecord);
                    RegUtilities.UserNavigationPaths = Resource.ReviewRegister_URL;
                }
                else if (this.RegistryRecord == null)
                    _masterPage.DisplayErrorMessage(Resource.RecordNotFound_Label_Text, false);

                this.SetPageTitle();
                this.SubscribeToEventsInAccordionControl();
                _masterPage.MakeCtrlShowProgressModal(this.SubmitButton.ClientID, "Submitting...", string.Empty);
                _masterPage.MakeCtrlShowProgressModal(this.RegisterButton.ClientID, "Registering...", string.Empty);
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
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            #region Page Settings
            // To make more easy to read the code.
            _masterPage = (RegistrationMaster)this.Master;
            //Set the accordion key where is the customized control inside.
            this.AccordionGroupToListen = "ReviewRegister";
            //Set the cotrolID inside accordion group.
            this.AccordionControlToListen = "RegistryRecordNavigation";
            #endregion

            this.LoadFormGenerators();

            base.OnPreInit(e);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        protected override void OnInit(EventArgs e)
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                this.SetJScriptReference();
                base.OnInit(e);
                this.DefineMixtureImageMenuButton.Text = Resource.DefineMixtureImageMenuButton_text;
                this.DefineMixtureImageMenuButton.MenuItemList[0].Text = Resource.AddComponent_Button_Text;
                this.DefineMixtureImageMenuButton.MenuItemList[1].Text = Resource.Select_Button_Text;
                this.DefineMixtureImageMenuButton.MenuItemList[2].Text = Resource.DeleteComponent_Button_Text;
                this.DefineMixtureImageMenuButton.ValidateAddComponent = true;
                this.DefineMixtureImageMenuButton.Command += new CommandEventHandler(DefineMixtureImageMenuButton_Command);

                this.DeleteRecordImageMenuButton.Click += new EventHandler(DeleteRecordImageMenuButton_Click);
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {


            if (RegUtilities.GetApprovalsEnabled())
            {
                if (this.RegistryRecord.Status == RegistryStatus.Approved)
                {
                    this.RegisterButton.Visible = true;
                    this.DeleteRecordImageMenuButton.Visible = this.EditButton.Visible = false;
                }
                else if (this.RegistryRecord.Status == RegistryStatus.Submitted)
                {
                    //edit button should not be visible when in edit mode
                    this.EditButton.Visible = !IsEditModeEnable;
                    this.DeleteRecordImageMenuButton.Visible = !IsEditModeEnable;
                    this.RegisterButton.Visible = false;
                }
                //User authorization need to override other approval status checking.
                if (!this.IsAuthorizedToEditRegistry())
                {
                    this.DeleteRecordImageMenuButton.Visible = this.EditButton.Visible = this.RegisterButton.Visible = false;
                }
            }

            if (this.RegistryRecord != null && !this.RegistryRecord.IsEditable)
            {
                this.DisableEditDeleteButtons();
            }
            base.OnPreRender(e);
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            try
            {

                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

                string _changeInLinesWhenRemoving = "ChangeInLinesWhenRemoving";
                //Script to clear ('null') the output Hidden var of the ChemDraw control in case its the first component in a mixture (0)
                if (!Page.ClientScript.IsOnSubmitStatementRegistered(_changeInLinesWhenRemoving))
                {
                    Page.ClientScript.RegisterOnSubmitStatement(typeof(string), _changeInLinesWhenRemoving,
                        @"if (document.getElementById('" + this.ActionToDoHiddenField.ClientID + @"') != null &&
                            document.getElementById('" + this.ActionToDoHiddenField.ClientID + @"').value == 'REMOVE')
                      {
                        ClearOutputValue('0');    
                        document.getElementById('" + this.ActionToDoHiddenField.ClientID + @"').value = 'null';
                      }");
                }


                //according rls status is going to add the css classes to make projects grids look like required.
                this.Header.Controls.Add(RegUtilities.GetRLSCSSClass(this.RegistryRecord.RLSStatus));

                this.RenderJSArrayWithCliendIds();



                base.OnPreRenderComplete(e);

                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        public void SetCssClassForStatus_DataBound(object sender, EventArgs e)
        {
            try
            {
                if (((DropDownList)sender).SelectedItem.Text == "Rejected")
                {
                    ((DropDownList)sender).CssClass = "StatusRejectedView";
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
            }
        }

        protected void EditButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.EditMixture();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.CancelEdit();
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


        protected void GoHomeButton_Click(object sender, EventArgs e)
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

        protected void RegisterButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.RegisterMixture();
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
                if (Request[PerkinElmer.COE.Registration.Server.Constants.CurrentPageState_UrlParameter] != null && Request[PerkinElmer.COE.Registration.Server.Constants.CurrentPageState_UrlParameter].ToLower().Trim() == "endnewregistry")
                    Server.Transfer(Resource.SubmitMixture_URL);
                else
                    //Fix for 148546
                    //By Clicking the 'Done' button after Registering the approved Batch.
                    //The page should be returned to the list view(ChemBioVizSearch.aspx)
                    Server.Transfer(string.Format("{0}?Caller=LASTSEARCH", Resources.Resource.ReviewRegisterSearch_URL));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        //Fix for CSBR:152510
        //To resolve error like "Object reference is not set  to Instance of the object"
        //when we click on 'Search' button after submitting temparary record
        protected void SearchTemp_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                Server.Transfer(Resource.ReviewRegisterSearch_URL + "?Caller=ST", false);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleUIException(ex);
                _masterPage.DisplayErrorMessage(ex.Message, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.SubmitEdition();
                this._mixtureCOEFormGroup.DataBind();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        void DeleteRecordImageMenuButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.DeleteRegistry();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void CompoundDuplicatesMessageBox_AcceptButtonClicked(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (CompoundDuplicatesMessageBox.Attributes["DuplicateMixture"] != null && CompoundDuplicatesMessageBox.Attributes["DuplicateMixture"].Equals(bool.TrueString))
                    Server.Transfer(Resource.RegistryDuplicates_Page_URL, false);
                else
                    Server.Transfer(Resource.ComponentDuplicates_URL, false);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
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
                _masterPage.DisplayErrorMessage(exception, false);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

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
                _masterPage.DisplayErrorMessage(exception, false);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        }

        void DefineMixtureImageMenuButton_Command(object sender, CommandEventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                switch (e.CommandName)
                {
                    case "AddComponent":
                        this.AddComponent();
                        break;
                    case "SearchComponent":
                        this.SearchCompound();
                        break;
                    case "DeleteComponent":
                        this.DeleteComponent();
                        break;
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region GUIShell Methods

        /// <summary>
        /// Set the reference to the JScript Page.
        /// </summary>
        private void SetJScriptReference()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string jScriptKey = this.ID.ToString() + "CommonScriptsPage";

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(jScriptKey))
                Page.ClientScript.RegisterClientScriptInclude(jScriptKey, Page.ResolveUrl("~/Forms/Public/JScripts/CommonScripts.js"));

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        /// <summary>
        /// This method sets all the controls attributtes as Text, etc...
        /// </summary>
        /// <remarks>You have to expand the group of your interest in the accordion</remarks>
        protected override void SetControlsAttributtes()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string currentMessage = string.Empty;
            switch (CurrentRegistryType)
            {
                case RegistryTypes.Both:
                    currentMessage = Resource.ReviewRegisterRecord_Page_Title;
                    break;
                case RegistryTypes.Mixture:
                    currentMessage = Resource.ReviewRegisterMixture_Page_Title;
                    break;
                case RegistryTypes.Component:
                    currentMessage = Resource.ReviewRegisterComponent_Page_Title;
                    break;
            }
            this.PageTitleLabel.Text = currentMessage;
            this.EditButton.Text = Resource.Edit_Button_Text;
            this.CancelButton.Text = Resource.Cancel_Button_Text;
            this.SearchTempButton.Text = Resource.SearchTemp_Button_Text;
            this.RegisterButton.Text = Resource.Register_Button_Text;
            this.SubmitButton.Text = Resource.Save_Button_Text;
            this.SubmitButton.ToolTip = string.Format(Resource.Save_Button_Tooltip_Temp,
                RegistryRecord.ID);
            this.DoneButton.Text = Resource.Done_Button_Text;
            this.BackButton.Value = Resource.Back_Button_Text;
            this.GoHomeButton.Text = Resource.Back_Button_Text;
            if ((!string.IsNullOrEmpty(Request["IsSubmit"]) && Request["IsSubmit"] == "true"))
            {
                switch (CurrentRegistryType)
                {
                    case RegistryTypes.Mixture:
                        this.GoHomeButton.ToolTip = Resource.ReturnToNewMixture_Button_ToolTip;
                        break;
                    case RegistryTypes.Component:
                        this.GoHomeButton.ToolTip = Resource.ReturnToNewComponent_Button_ToolTip;
                        break;
                    case RegistryTypes.Both:
                        this.GoHomeButton.ToolTip = Resource.ReturnToNewRecord_Button_ToolTip;
                        break;
                }
            }
            else
            {
                //Fix for CSBR 157678- Clicking Done button from a registered record page leading to
                //search registry form instead of Submit Component form.
                this.GoHomeButton.ToolTip = Resource.ReturnToNewRecord_Button_ToolTip;
            }
            this.SearchTempButton.ToolTip = Resource.Search_Button_ToolTip;

            switch (CurrentRegistryType)
            {
                case RegistryTypes.Mixture:
                    this.EditButton.ToolTip = Resource.EditMixture_Button_ToolTip;
                    break;
                case RegistryTypes.Component:
                    this.EditButton.ToolTip = Resource.EditComponent_Button_ToolTip;
                    break;
                case RegistryTypes.Both:
                    this.EditButton.ToolTip = Resource.EditRecord_Button_ToolTip;
                    break;
            }
            this.DeleteRecordImageMenuButton.Text = Resource.DeleteRegistryButton_Text;


            //DGB TODO This OnClientClick event needs to be implemented on the MenuItem
            //this.DeleteComponentButton.OnClientClick = @"return RemoveComponent('" + this.ActionToDoHiddenField.ClientID + "');";

            //Accordion control attributes
            Control showButtonsIDHidden = _masterPage.GetControlInsideAnAccordionGroup(this.AccordionGroupToListen, ControlsToTakeCare.ShowButtonsHidden.ToString());
            if (showButtonsIDHidden != null)
                ((HiddenField)showButtonsIDHidden).Value = bool.FalseString;



            switch (CurrentRegistryType)
            {
                case RegistryTypes.Both:
                    this.DisplayStatusMessage(Resource.RegisterRecord_Label_Text);
                    this.DeleteRecordImageMenuButton.Text = Resource.DeleteTempRegistryButton_Text;
                    break;
                case RegistryTypes.Mixture:
                    this.DisplayStatusMessage(Resource.RegisterMixture_Label_Text);
                    this.DeleteRecordImageMenuButton.Text = Resource.DeleteTempMixtureButton_Text;
                    break;
                case RegistryTypes.Component:
                    this.DisplayStatusMessage(Resource.RegisterComponent_Label_Text);
                    this.DeleteRecordImageMenuButton.Text = Resource.DeleteTempComponentButton_Text;
                    break;
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// This method sets the Page Title <Title></Title>
        /// </summary>
        protected void SetPageTitle()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            switch (CurrentRegistryType)
            {
                case RegistryTypes.Both:
                    this.Page.Title = Resource.ReviewRegisterRecord_Page_Title;
                    this.Header.Title = Resource.ReviewRegisterRecord_Page_Title;
                    break;
                case RegistryTypes.Mixture:
                    this.Page.Title = Resource.ReviewRegisterMixture_Page_Title;
                    this.Header.Title = Resource.ReviewRegisterMixture_Page_Title;
                    break;
                case RegistryTypes.Component:
                    this.Page.Title = Resource.ReviewRegisterComponent_Page_Title;
                    this.Header.Title = Resource.ReviewRegisterComponent_Page_Title;
                    break;
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
                        ((ICOENavigationPanelControl)currentGroup.UserControl).CommandRaised += new EventHandler<COENavigationPanelControlEventArgs>(AccordionControl_CommandRaised);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        }

        void AccordionControl_CommandRaised(object sender, COENavigationPanelControlEventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);


            if (e.EventType == EventsToListen.NodeClicked.ToString())
            {
                UltraWebTree mixtureTree = (UltraWebTree)((Control)sender).FindControl(ControlsToTakeCare.UltraWebTreeControl.ToString());

                Node clickedNode = mixtureTree.SelectedNode;



                int index = int.Parse(clickedNode.DataKey.ToString().Substring(1));
                switch (clickedNode.DataKey.ToString()[0])
                {
                    case 'C':
                        if (CurrentRegistryType == RegistryTypes.Mixture && clickedNode.Text.IndexOf("Pick") > 0)
                        {
                            if (CurrentPageState != PageState.End)
                                _mixtureCOEFormGroup.Update();
                            SearchCompound(index);
                        }
                        else
                        {
                            if (CurrentPageState != PageState.End)
                                _mixtureCOEFormGroup.Update();
                            this.DisplayComponent(index);
                            if (RegistryRecord.SameBatchesIdentity && RegUtilities.GetFragmentsEnabled() && CurrentRegistryType == RegistryTypes.Component)
                            {
                                _mixtureCOEFormGroup.Update();
                            }


                        }
                        break;
                    case 'B':
                        if (CurrentPageState != PageState.End)
                            _mixtureCOEFormGroup.Update();
                        this.DisplayBatch(index);
                        break;
                    default:
                        if (CurrentPageState != PageState.End)
                            _mixtureCOEFormGroup.Update();
                        this.DisplayRecord(); //Show as Editable the first Component.

                        break;
                }

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

            COENavigationPane COENavigationPaneObject = null;

            this.GetControlInfo(ref COENavigationPaneObject);

            for (int i = 0; i < COENavigationPaneObject.ControlItem.Count; i++)
            {
                _masterPage.AddControlToAccordionPanel(COENavigationPaneObject.ControlItem[i], true);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        }

        private void GetControlInfo(ref COENavigationPane COENavigationPaneObject)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            COENavigationPaneObject = new COENavigationPane();

            COENavigationPane.ControlItemRow registryRecordNavigationControlRow = COENavigationPaneObject.ControlItem.NewControlItemRow();
            registryRecordNavigationControlRow.ParentID = "ReviewRegister";
            registryRecordNavigationControlRow.ControlType = "UserControl";
            if (registryRecordNavigationControlRow.ControlType == GUIShellTypes.SupportedControlTypes.UserControl.ToString())
                registryRecordNavigationControlRow.ControlSource = GUIShellTypes.UCPath + "RegistryRecordNavigation.ascx";
            registryRecordNavigationControlRow.ID = "RegistryRecordNavigation";
            COENavigationPaneObject.ControlItem.AddControlItemRow(registryRecordNavigationControlRow);

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

        private void DisplayCompoundIntoTree(RegistryRecord record)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            ICOENavigationPanelControl tree = _masterPage.GetICOENavigationPanelControlInsideAnAccordionGroup(this.AccordionGroupToListen);
            if (tree != null) //Coverity fix - CID 11860 
            {
                tree.NewRegistryType = this.CurrentRegistryType.ToString();
                tree.DataBind(record);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void DisplayCompoundIntoTree(RegistryRecord registryRecord, string nodeToShowSelected)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            ICOENavigationPanelControl tree = _masterPage.GetICOENavigationPanelControlInsideAnAccordionGroup(this.AccordionGroupToListen);
            if (tree != null) //Coverity fix - CID 11861 
            {
                tree.NewRegistryType = this.CurrentRegistryType.ToString();
                tree.DataBind(registryRecord, nodeToShowSelected);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region Page Methods
        private void ReplaceExistingComponent(string compoundIdToAdd, string regIdToSearch)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (RegistryRecord != null)
            {
                int index = (int)Session["currentCompoundIndex"];
                Session["currentCompoundIndex"] = null;
                if (!string.IsNullOrEmpty(compoundIdToAdd)/* && !string.IsNullOrEmpty(regIdToSearch)*/)
                {
                    Compound compound = this.GetCompound(compoundIdToAdd);

                    RegistryRecord.ReplaceCompound(index, compound);

                    DisplayComponent(index);
                    this.DisplayCompoundIntoTree(RegistryRecord);
                }
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private Compound GetCompound(string regNumToSearch)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string filter = "<REGISTRYLIST><REGNUMBER>";
            filter += regNumToSearch;
            filter += "</REGNUMBER></REGISTRYLIST>";

            CompoundList compounds = CompoundList.GetCompoundList(filter);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            if (compounds.Count > 0)
            {
                return compounds[0];
            }
            else
                return null;
        }

        private bool ComponentExists(RegistryRecord currentRegistry, string componentRegID)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            bool retVal = false;
            foreach (Component currentComponent in currentRegistry.ComponentList)
            {
                if (!string.IsNullOrEmpty(currentComponent.Compound.RegNumber.RegNum))
                {
                    if (currentComponent.Compound.RegNumber.RegNum == componentRegID)
                    {
                        retVal = true;
                        break;
                    }
                }
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            return retVal;
        }

        /// <summary>
        /// The method will check if it's the first time you visit the page, or you come after saving a compound.
        /// </summary>
        /// <remarks>In this method you have to make visible the rows that you pretend</remarks>
        private FormGroup BuildCoeFormHolder(string xmlUrl)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

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
                //FormGroup formGroup = BuildCoeFormHolder("../FormGeneratorXml/ReviewRegisterMixture.xml");
                FormGroup formGroup = COEFormBO.Get(this.PageFormGroupID).COEFormGroup;
                _mixtureCOEFormGroup = new COEFormGroup();
                _mixtureCOEFormGroup.ID = "ReviewRegisterMixtureFormGroup";

                mixtureInformationHolder.Controls.Clear();
                mixtureInformationHolder.Controls.Add(_mixtureCOEFormGroup);

                _mixtureCOEFormGroup.FormGroupDescription = formGroup;
                _mixtureCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;
                _mixtureCOEFormGroup.DisplayCulture = this.DisplayCulture;

                mixtureInformationHolder.Visible = true;
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

        private void CheckSearchParams()
        {
            int hitlistid = -1;
            int currentPage = -1;
            if (Request.QueryString["HitListID"] != null)
                int.TryParse(Request.QueryString["HitListID"].ToString(), out hitlistid);
            if (Request.QueryString["CurrentPage"] != null)
                int.TryParse(Request.QueryString["CurrentPage"].ToString(), out currentPage);
            //if(hitlistid > -1 && currentPage > -1)
            if (hitlistid > -1)
                this.Session["TempSearchedInfo"] = SearchedInfo.Create(hitlistid, currentPage, CambridgeSoft.COE.Framework.HitListType.TEMP);
        }

        private void SetFragmentsFormsVisibility(RegUtilities.PageState mode)
        {
            RegUtilities.FragmentForm showFragmentsRO = RegUtilities.ShowFragmentsInRO(this.RegistryRecord.SameBatchesIdentity, mode, this.RegistryRecord.ComponentList.Count == 1, CurrentRegistryType.ToString());
            switch (showFragmentsRO)
            {
                case RegUtilities.FragmentForm.Edit:
                    SetFormGeneratorVisibility(SubForms.BatchComponentFragmentsForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentFragmentsFormEdit, true);
                    break;
                case RegUtilities.FragmentForm.Hidden:
                    SetFormGeneratorVisibility(SubForms.BatchComponentFragmentsForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentFragmentsFormEdit, false);
                    break;
                case RegUtilities.FragmentForm.ReadOnly:
                    SetFormGeneratorVisibility(SubForms.BatchComponentFragmentsForm, true);
                    SetFormGeneratorVisibility(SubForms.BatchComponentFragmentsFormEdit, false);
                    break;
            }
        }

        private void SetPageMode(PageState mode, string messageToDisplay)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);


            switch (mode)
            {
                case PageState.AddComponent:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Add;

                    SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);

                    SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);




                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            if (AlreadyRegistered)
                                messageToDisplay = string.Format(Resource.RegisteredRecord_Label_Text,
                                                                    RegistryRecord.RegNumber.RegNum);
                            this.DisplayStatusMessage(Resource.RegisterRecord_Label_Text);
                            break;
                        case RegistryTypes.Mixture:
                            if (IsEditModeEnable)
                            {
                                SetPageMode(PageState.EndComponent, String.Empty);
                            }
                            if (AlreadyRegistered)
                                messageToDisplay = string.Format(Resource.RegisteredMixture_Label_Text,
                                                                    RegistryRecord.RegNumber.RegNum);
                            this.DisplayStatusMessage(Resource.RegisterMixture_Label_Text);
                            break;
                        case RegistryTypes.Component:
                            if (AlreadyRegistered)
                                messageToDisplay = string.Format(Resource.RegisteredComponent_Label_Text,
                                                                    RegistryRecord.RegNumber.RegNum);
                            this.DisplayStatusMessage(Resource.RegisterComponent_Label_Text);
                            break;
                    }
                    break;

                case PageState.ViewRecord:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;

                    SetFormGeneratorVisibility(SubForms.MixtureResumeForm, true);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);

                    SetFormGeneratorVisibility(SubForms.BatchForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);



                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            if (AlreadyRegistered)

                                messageToDisplay = string.Format(Resource.RegisteredRecord_Label_Text,
                                                                    RegistryRecord.RegNumber.RegNum);
                            this.DisplayStatusMessage(Resource.RegisterRecord_Label_Text);
                            break;
                        case RegistryTypes.Mixture:
                            if (AlreadyRegistered)

                                messageToDisplay = string.Format(Resource.RegisteredMixture_Label_Text,
                                                                    RegistryRecord.RegNumber.RegNum);
                            this.DisplayStatusMessage(Resource.RegisterMixture_Label_Text);
                            if (IsEditModeEnable)
                            {
                                SetPageMode(PageState.EditRecord, string.Empty);
                            }
                            break;
                        case RegistryTypes.Component:
                            if (AlreadyRegistered)
                            {

                                messageToDisplay = string.Format(Resource.RegisteredComponent_Label_Text,
                                                                   RegistryRecord.RegNumber.RegNum);
                            }
                            this.DisplayStatusMessage(Resource.RegisterComponent_Label_Text);
                            break;
                    }
                    break;
                case PageState.ViewSingleComponent:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;

                    SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.BatchForm, true);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);



                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:

                            this.DisplayStatusMessage(Resource.RegisterRecord_Label_Text);
                            SetFormGeneratorVisibility(SubForms.BatchComponentForm, true);
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, true);
                            break;
                        case RegistryTypes.Mixture:

                            this.DisplayStatusMessage(Resource.RegisterMixture_Label_Text);
                            SetFormGeneratorVisibility(SubForms.BatchComponentForm, true);
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, true);
                            break;
                        case RegistryTypes.Component:

                            this.DisplayStatusMessage(Resource.RegisterComponent_Label_Text);
                            SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                            break;
                    }
                    break;
                case PageState.ViewComponent:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;

                    SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);

                    SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);

                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:

                            this.DisplayStatusMessage(Resource.RegisterRecord_Label_Text);
                            break;
                        case RegistryTypes.Mixture:
                            if (IsEditModeEnable)
                            {
                                SetPageMode(PageState.EndComponent, string.Empty);
                            }
                            this.DisplayStatusMessage(Resource.RegisterMixture_Label_Text);
                            break;
                        case RegistryTypes.Component:

                            this.DisplayStatusMessage(Resource.RegisterComponent_Label_Text);
                            break;
                    }
                    break;
                case PageState.ViewBatch:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;

                    SetFormGeneratorVisibility(SubForms.BatchForm, true);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, true);


                    SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                    SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);

                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, true);

                            if (AlreadyRegistered)
                                messageToDisplay = string.Format(Resource.RegisteredRecord_Label_Text,
                                                                    RegistryRecord.RegNumber.RegNum);
                            this.DisplayStatusMessage(Resource.RegisterRecord_Label_Text);
                            break;
                        case RegistryTypes.Mixture:
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, true);

                            if (AlreadyRegistered)
                                messageToDisplay = string.Format(Resource.RegisteredMixture_Label_Text,
                                                                    RegistryRecord.RegNumber.RegNum);
                            this.DisplayStatusMessage(Resource.RegisterMixture_Label_Text);
                            break;
                        case RegistryTypes.Component:
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);

                            if (AlreadyRegistered)
                                messageToDisplay = string.Format(Resource.RegisteredComponent_Label_Text,
                                                                    RegistryRecord.RegNumber.RegNum);
                            this.DisplayStatusMessage(Resource.RegisterComponent_Label_Text);
                            break;
                    }
                    break;
                case PageState.EditRecord:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Edit;

                    SetFormGeneratorVisibility(SubForms.MixtureResumeForm, true);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);

                    SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);

                    this.DisplayStatusMessage(Resource.EditMixture_Label_Text);

                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DisplayStatusMessage(Resource.EditRecord_Label_Text);
                            this.PageTitleLabel.Text = Resource.ReviewRegisterEditRecord_Page_Title;
                            break;
                        case RegistryTypes.Mixture:

                            this.DisplayStatusMessage(Resource.EditMixture_Label_Text);
                            this.PageTitleLabel.Text = Resource.ReviewRegisterEditMixture_Page_Title;
                            break;
                        case RegistryTypes.Component:
                            this.DisplayStatusMessage(Resource.EditComponent_Label_Text);
                            this.PageTitleLabel.Text = Resource.ReviewRegisterEditComponent_Page_Title;
                            break;
                    }


                    break;
                case PageState.EditComponent:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Edit;

                    SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);

                    SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);

                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DisplayStatusMessage(Resource.EditRecord_Label_Text);
                            this.PageTitleLabel.Text = Resource.ReviewRegisterRecord_Page_Title;
                            break;
                        case RegistryTypes.Mixture:
                            SetPageMode(PageState.EndComponent, String.Empty);
                            this.DisplayStatusMessage(Resource.EditMixture_Label_Text);
                            this.PageTitleLabel.Text = Resource.ReviewRegisterMixture_Page_Title;
                            break;
                        case RegistryTypes.Component:
                            this.DisplayStatusMessage(Resource.EditComponent_Label_Text);
                            this.PageTitleLabel.Text = Resource.ReviewRegisterComponent_Page_Title;
                            break;
                    }
                    break;
                case PageState.EditBatch:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Edit;

                    SetFormGeneratorVisibility(SubForms.BatchForm, true);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, true);
                    SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                    SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);

                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DisplayStatusMessage(Resource.EditRecord_Label_Text);
                            this.PageTitleLabel.Text = Resource.ReviewRegisterRecord_Page_Title;
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, true);
                            break;
                        case RegistryTypes.Mixture:
                            this.DisplayStatusMessage(Resource.EditMixture_Label_Text);
                            this.PageTitleLabel.Text = Resource.ReviewRegisterMixture_Page_Title;
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, true);
                            break;
                        case RegistryTypes.Component:
                            this.DisplayStatusMessage(Resource.EditComponent_Label_Text);
                            this.PageTitleLabel.Text = Resource.ReviewRegisterComponent_Page_Title;
                            SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                            break;
                    }
                    break;
                case PageState.End:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;
                    SetFormGeneratorVisibility(SubForms.MixtureResumeForm, true);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                    if (!string.IsNullOrEmpty(Request[Constants.CurrentPageState_UrlParameter]))
                    {
                        if ((Request[Constants.CurrentPageState_UrlParameter].ToLower() == "end"
                            || Request[Constants.CurrentPageState_UrlParameter].ToLower() == "endnewregistry")
                            && messageToDisplay == String.Empty)
                            switch (CurrentRegistryType)
                            {
                                case RegistryTypes.Both:
                                    if (this.RegistryRecord.RegNum != string.Empty)
                                    {
                                        messageToDisplay = string.Format(Resource.RegisteredRecord_Label_Text,
                                                                     GetRegistryRegNumberFromSession());
                                    }
                                    else
                                    {
                                        messageToDisplay = string.Format(Resource.SubmittedRecord_Label_Text,
                                        this.RegistryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                                                             GUIShellTypes.IdsPaddingCharacter));
                                    }
                                    break;
                                case RegistryTypes.Mixture:
                                    if (this.RegistryRecord.RegNum != string.Empty)
                                    {
                                        messageToDisplay = string.Format(Resource.RegisteredMixture_Label_Text,
                                                                    GetRegistryRegNumberFromSession());
                                    }
                                    else
                                    {
                                        messageToDisplay = string.Format(Resource.SubmittedMixture_Label_Text,
                                         this.RegistryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                                                              GUIShellTypes.IdsPaddingCharacter));


                                    }
                                    break;
                                case RegistryTypes.Component:
                                    if (this.RegistryRecord.RegNum != string.Empty)
                                    {
                                        messageToDisplay = string.Format(Resource.RegisteredComponent_Label_Text,
                                                                     GetRegistryRegNumberFromSession());
                                    }
                                    else
                                    {
                                        messageToDisplay = string.Format(Resource.SubmittedComponent_Label_Text,
                                        this.RegistryRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                                                             GUIShellTypes.IdsPaddingCharacter));
                                    }
                                    break;
                            }

                    }

                    break;
                case PageState.EndComponent:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;

                    SetFormGeneratorVisibility(SubForms.SubmittedComponentForm, true);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);

                    SetFormGeneratorVisibility(SubForms.MixtureResumeForm, false);
                    SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);


                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            if (AlreadyRegistered)
                                messageToDisplay = string.Format(Resource.RegisteredRecord_Label_Text,
                                                             RegistryRecord.RegNumber.RegNum);

                            break;
                        case RegistryTypes.Mixture:
                            if (AlreadyRegistered)
                                messageToDisplay = string.Format(Resource.RegisteredMixture_Label_Text,
                                                           RegistryRecord.RegNumber.RegNum);

                            break;
                        case RegistryTypes.Component:
                            if (AlreadyRegistered)
                                messageToDisplay = string.Format(Resource.RegisteredComponent_Label_Text,
                                                             RegistryRecord.RegNumber.RegNum);

                            break;
                    }
                    break;
            } //End Switch


            CurrentPageState = mode;
            this.SetFragmentsFormsVisibility(((RegUtilities.PageState)Enum.Parse(
                                    typeof(RegUtilities.PageState), mode.ToString())));

            this.DefineMixtureImageMenuButton.Visible = false;
            if (this.AlreadyRegistered)
            {
                this.EditButton.Visible = this.RegisterButton.Visible = false;
                this.CancelButton.Visible = this.SubmitButton.Visible = false;
                this.BackButton.Visible = this.SearchTempButton.Visible = false;
                this.DoneButton.Visible = false;
                this.GoHomeButton.Visible = true;
                this.GoHomeButton.Text = Resource.Done_Button_Text;
                this.DeleteRecordImageMenuButton.Visible = false;

                switch (CurrentRegistryType)
                {
                    case RegistryTypes.Both:
                        if (string.IsNullOrEmpty(messageToDisplay))
                            messageToDisplay = string.Format(Resource.RegisteredRecord_Label_Text,
                                                         RegistryRecord.RegNumber.RegNum);

                        break;
                    case RegistryTypes.Mixture:
                        if (string.IsNullOrEmpty(messageToDisplay))
                            messageToDisplay = string.Format(Resource.RegisteredMixture_Label_Text,
                                                       RegistryRecord.RegNumber.RegNum);

                        break;
                    case RegistryTypes.Component:
                        if (string.IsNullOrEmpty(messageToDisplay))
                            messageToDisplay = string.Format(Resource.RegisteredComponent_Label_Text,
                                                         RegistryRecord.RegNumber.RegNum);

                        break;
                }



            }
            else
            {
                if (this.IsEditModeEnable) // if we are editing
                {
                    this.EditButton.Visible = this.RegisterButton.Visible = this.GoHomeButton.Visible = false;
                    this.SearchTempButton.Visible = false;
                    this.CancelButton.Visible = this.SubmitButton.Visible = true;
                    this.DeleteRecordImageMenuButton.Visible = false;

                    if (CurrentComponentIndex >= 0) // and showing a component
                    {
                        switch (this.CurrentRegistryType)
                        {
                            case RegistryTypes.Both:
                                this.DefineMixtureImageMenuButton.Visible = true;
                                this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true;   //AddComponent
                                this.DefineMixtureImageMenuButton.MenuItemList[1].Enabled = true;  //SearchComponent
                                this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled =
                                     this.RegistryRecord.ComponentList.Count > 1;  //DeleteComponent
                                break;
                            case RegistryTypes.Mixture:
                                //Fix for CSBR: 154064, Title- Mixture creation has too many options in drop-down menu and 
                                //confusing mouse over.
                                // This code removes ‘pick an existing component’ menu item from Drop down menu, Which is  
                                //not required in Mixture flow.
                                this.DefineMixtureImageMenuButton.Visible = true;
                                this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true;   //AddComponent
                                if (mode == PageState.EndComponent)
                                {
                                    this.DefineMixtureImageMenuButton.MenuItemList[1].Visible = false;  //SearchComponent
                                    this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = this.RegistryRecord.ComponentList.Count > 1;  //DeleteComponent

                                }
                                else
                                {
                                    if (mode == PageState.ViewRecord)
                                    {
                                        this.DefineMixtureImageMenuButton.MenuItemList[1].Visible = false;  //SearchComponent
                                        this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false;  //DeleteComponent
                                    }
                                    else
                                    {
                                        //Fix for CSBR: 154064, Title- Mixture creation has too many options in drop-down menu and 
                                        //confusing mouse over.
                                        // This code removes ‘pick an existing component’ menu item from Drop down menu, Which is  
                                        //not required in Mixture flow.
                                        this.DefineMixtureImageMenuButton.MenuItemList[1].Visible = false; //SearchComponent
                                        this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false;  //DeleteComponent
                                    }
                                }



                                break;
                            case RegistryTypes.Component:

                                if (RegUtilities.GetMixturesEnabled() == true)
                                {

                                    //mixtures should be shown in the single component case just incase it was determine
                                    //that the component is really a mixture
                                    this.DefineMixtureImageMenuButton.Visible = true;
                                    this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true;   //AddComponent
                                    this.DefineMixtureImageMenuButton.MenuItemList[1].Enabled = true;  //SearchComponent
                                    this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled =
                                    this.RegistryRecord.ComponentList.Count > 1;  //DeleteComponent

                                }
                                else
                                {
                                    this.DefineMixtureImageMenuButton.Visible = false;
                                }
                                break;
                        }

                    }
                    else
                    { // and not showing a component
                        if (mode == PageState.EditBatch)
                        {
                            this.DefineMixtureImageMenuButton.Visible = false; // Don't show when batch is selected
                        }

                        switch (this.CurrentRegistryType)
                        {
                            case RegistryTypes.Both:
                                this.DefineMixtureImageMenuButton.Visible = true;
                                this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true;   //AddComponent
                                this.DefineMixtureImageMenuButton.MenuItemList[1].Enabled = false;  //SearchComponent [Not showing component]
                                this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false;

                                break;
                            //Fix for CSBR: 154064, Title- Mixture creation has too many options in drop-down menu and 
                            //confusing mouse over.
                            // This code removes ‘pick an existing component’ menu item from Drop down menu, Which is  
                            //not required in Mixture flow.
                            case RegistryTypes.Mixture:
                                this.DefineMixtureImageMenuButton.Visible = true;
                                this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true;   //AddComponent
                                this.DefineMixtureImageMenuButton.MenuItemList[1].Visible = false; //SearchComponent
                                this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false;
                                break;
                            case RegistryTypes.Component:
                                if (RegUtilities.GetAllowUnregisteredComponents() == true)
                                {
                                    //mixtures should be shown in the single component case just incase it was determine
                                    //that the component is really a mixture
                                    this.DefineMixtureImageMenuButton.Visible = true;
                                    this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true;   //AddComponent
                                    this.DefineMixtureImageMenuButton.MenuItemList[1].Enabled = true;  //SearchComponent
                                    this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled =
                                         this.RegistryRecord.ComponentList.Count > 1;  //DeleteComponent
                                }
                                else
                                {
                                    this.DefineMixtureImageMenuButton.Visible = false;
                                }
                                break;
                        }
                    }
                    //according rls status is going to add the css classes to make projects grids look like required.
                    this.Header.Controls.Add(RegUtilities.GetRLSCSSClass(this.RegistryRecord.RLSStatus));
                }
                else
                { // if we are not editing

                    //No matter how many components stored in a RegistryRecord object, they were all created by the same user.
                    //Check the first component can determine whether the user have eidt privilege for current record.
                    if (IsAuthorizedToEditRegistry())
                    {
                        this.EditButton.Visible = true;
                        this.DeleteRecordImageMenuButton.Visible = true;

                    }
                    else
                    {
                        this.EditButton.Visible = false;
                        this.DeleteRecordImageMenuButton.Visible = false;
                    }
                    this.GoHomeButton.Visible = true;
                    this.CancelButton.Visible = this.SubmitButton.Visible = false;
                    //this.DeleteRecordImageMenuButton.Visible = true;

                    if (CurrentComponentIndex >= 0) // but showing a component
                    {
                        this.RegisterButton.Visible = IsAuthorizedToEditRegistry();
                        this.SearchTempButton.Visible = true;
                    }
                    else
                    { // nor showing a component
                        this.RegisterButton.Visible = IsAuthorizedToEditRegistry();
                        this.SearchTempButton.Visible = true;
                    }
                }
            }

            this.ShowConfirmationMessage(messageToDisplay);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                        MethodBase.GetCurrentMethod().Name);
        }

        private void HideTemRegistryNumber()
        {
            if (AlreadyRegistered)
            {
                if (((COEFormGenerator)_mixtureCOEFormGroup.FindControl(GetFormIndex(SubForms.MixtureResumeForm))).FindControl("MixtureResumeTitle") != null)
                {
                    ((COEFormGenerator)_mixtureCOEFormGroup.FindControl(GetFormIndex(SubForms.MixtureResumeForm))).FindControl("MixtureResumeTitle").Visible = false;

                }
                if (((COEFormGenerator)_mixtureCOEFormGroup.FindControl(GetFormIndex(SubForms.MixtureResumeForm))).FindControl("MixtureResumeID") != null)
                {
                    ((COEFormGenerator)_mixtureCOEFormGroup.FindControl(GetFormIndex(SubForms.MixtureResumeForm))).FindControl("MixtureResumeID").Visible = false;

                }
            }
        }

        /// <summary>
        /// Method to display confirmation messages in the top of the page (MessagesAreaUC)
        /// </summary>
        /// <param name="messageToDisplay">The text to display</param>
        private void ShowConfirmationMessage(string messageToDisplay)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                            MethodBase.GetCurrentMethod().Name);
            if (messageToDisplay != String.Empty)
            {
                this.MessagesAreaUserControl.AreaText = messageToDisplay;
                this.MessagesAreaUserControl.Visible = true;
                this.MessagesAreaRow.Visible = true;
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                            MethodBase.GetCurrentMethod().Name);
        }
        //Fix for CSBR-149346 
        //In this bug the User Navigates to Search temp form after updates a Registered record insted Search Parmanent page.
        //Apart from this Bug, It rectified all flows from starting page to starting page in  Registration. 
        private void CheckIsSubmitvalue()
        {
            string tempval = String.Empty;
            if (Request.QueryString["isSubmit"] != null)
            {
                tempval = Request["isSubmit"].ToString();
                this.CheckIsSubmit = Convert.ToBoolean(tempval);
            }
        }
        //Fix for CSBR-149346 
        //In this bug the User Navigates to Search temp form after updates a Registered record insted Search Parmanent page.
        //Apart from this Bug, It rectified all flows from starting page to starting page in  Registration. 
        private void LeavePage()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                            MethodBase.GetCurrentMethod().Name);
            Session.Remove("EditRegistryModeEnable");

            List<string> lstPaths = (List<string>)RegUtilities.UserNavigationPaths;
            if (lstPaths != null && lstPaths.Count > 0)
            {
                string userFirstPage = lstPaths[0].ToString();
                if (userFirstPage == Resource.SubmitComponent_URL.ToString())
                {
                    Server.Transfer(string.Format("{0}?{1}={2}", Resource.SubmitMixture_URL, Constants.RegistryTypeParameter, this.CurrentRegistryType));
                }
                if (userFirstPage == Resource.ViewMixtureSearch_URL.ToString())
                {
                    string url = Resource.ViewMixtureSearch_URL;
                    url += "?Caller=LASTSEARCH"; // url += "?Caller=SP";
                    Session["isSubmit"] = null;
                    Server.Transfer(url, false);
                }
                if (userFirstPage == Resource.ReviewRegisterSearch_URL.ToString())
                {
                    string url = Resource.ReviewRegisterSearch_URL;
                    url += "?Caller=LASTSEARCH"; // url += "?Caller=SP";
                    Session["isSubmit"] = null;
                    Server.Transfer(url, false);
                }
                if (userFirstPage == Resource.ComponentDuplicates_URL.ToString())
                {
                    string url = Resource.ComponentDuplicates_URL;
                    url += "?Caller=LASTSEARCH"; // url += "?Caller=SP";
                    Session["isSubmit"] = null;
                    Server.Transfer(url, false);
                }
            }

            //Fix for CSBR 157678- Clicking Done button from a registered record page leading to 
            //search registry form instead of Submit Component form.
            if ((!string.IsNullOrEmpty(Request["IsSubmit"]) && Request["IsSubmit"] == "true") || (CheckIsSubmit && !string.IsNullOrEmpty(Request["RegisteredObjectId"])) || (_masterPage.PreviousPage.AbsolutePath.Contains("ComponentDuplicates.aspx")))
            {
                Session["isSubmit"] = null;
                Server.Transfer(string.Format("{0}?{1}={2}", Resource.SubmitMixture_URL, Constants.RegistryTypeParameter, this.CurrentRegistryType));
            }
            else
            {
                string url = Resource.ReviewRegisterSearch_URL;
                if (_masterPage.PreviousPage.AbsolutePath.Contains(Resource.ReviewRegisterSearch_URL.Substring(Resource.ReviewRegisterSearch_URL.LastIndexOf("/"))))
                {
                    url += "?Caller=LASTSEARCH";
                }
                else if (IsEditStructureModeEnable)
                {
                    url += "?Caller=SP";
                }
                else
                    url += "?Caller=LASTSEARCH";

                IsEditStructureModeEnable = false;
                Session["isSubmit"] = null;
                Server.Transfer(url, false);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                        MethodBase.GetCurrentMethod().Name);
        }

        private void DisplayRecord()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                        MethodBase.GetCurrentMethod().Name);
            CurrentComponentIndex = -1;

            if (IsEditModeEnable)
                this.SetPageMode(PageState.EditRecord, string.Empty);
            else
                this.SetPageMode(PageState.ViewRecord, string.Empty);

            this.DisplayCompoundIntoTree(RegistryRecord);
            _mixtureCOEFormGroup.DataBind(); // Fix for CSBR-155652.
            HideTemRegistryNumber();
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void DisplayComponent(int index)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            CurrentComponentIndex = index;

            if (AlreadyRegistered)
            {
                SetPageMode(PageState.EndComponent, String.Empty);
            }
            else
            {
                Component currentComponent = RegistryRecord.ComponentList[index];

                if (IsEditModeEnable)
                {
                    if (this.IsEditable(currentComponent))
                    {
                        if (currentComponent.IsNew && !currentComponent.IsDirty)
                            SetPageMode(PageState.AddComponent, string.Empty);
                        else
                            SetPageMode(PageState.EditComponent, string.Empty);
                    }
                    else
                    {
                        SetPageMode(PageState.EndComponent, string.Empty);
                    }
                }
                else
                {
                    if (!this.IsEditable(currentComponent))
                        SetPageMode(PageState.EndComponent, String.Empty);
                    else
                        SetPageMode(PageState.ViewComponent, String.Empty);
                }
            }

            SetFormGeneratorPageIndex(SubForms.ComponentForm, index);
            SetFormGeneratorPageIndex(SubForms.CompoundCustomProperties, index);
            SetFormGeneratorPageIndex(SubForms.SubmittedComponentForm, index);
            SetFormGeneratorPageIndex(SubForms.BatchComponentFragmentsForm, index);
            SetFormGeneratorPageIndex(SubForms.BatchComponentCustomProperties, index);

            this.DisplayCompoundIntoTree(RegistryRecord);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        }

        private void DisplayBatch(int index)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            CurrentComponentIndex = -1;
            if (IsEditModeEnable)
                this.SetPageMode(PageState.EditBatch, string.Empty);
            else if (CurrentPageState == PageState.ViewRecord || CurrentPageState == PageState.ViewComponent)
                SetPageMode(PageState.ViewBatch, string.Empty);
            else
                SetPageMode(PageState.ViewBatch, string.Empty);

            SetFormGeneratorPageIndex(SubForms.BatchForm, index);
            SetFormGeneratorPageIndex(SubForms.BatchCustomProperties, index);
            SetFormGeneratorPageIndex(SubForms.BatchComponentForm, index);
            SetFormGeneratorPageIndex(SubForms.BatchComponentCustomProperties, index);

            SetFormGeneratorPageIndex(SubForms.BatchComponentFragmentsFormEdit, index);
            SetFormGeneratorPageIndex(SubForms.BatchComponentFragmentsForm, index);

            this.DisplayCompoundIntoTree(RegistryRecord);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void SetFormGeneratorPageIndex(SubForms subform, int pageIndex)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            if (_mixtureCOEFormGroup != null)
                _mixtureCOEFormGroup.SetFormGeneratorPageIndex(GetFormIndex(subform), pageIndex);

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }



        private void SetFormGeneratorVisibility(SubForms subform, bool visible)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            if (_mixtureCOEFormGroup != null)
                _mixtureCOEFormGroup.SetFormGeneratorVisibility(GetFormIndex(subform), visible);

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private int GetFormIndex(SubForms subform)
        {
            return (int)subform;
        }

        private void SubmitEdition()
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                _mixtureCOEFormGroup.Update();
                if (IsPreRegistration && (CurrentPageState == PageState.EditComponent || CurrentPageState == PageState.EndComponent))
                {
                    RegistryRecord.ApplyEdit();
                    RegistryRecord.ModuleName = ChemDrawWarningChecker.ModuleName.REGISTRATION;

                    RegistryRecord.Save(DuplicateCheck.PreReg);                    
                    if (!RegistryRecord.IsValid)
                    {                        
                        DisplayBrokenRules(this.RegistryRecord.GetBrokenRulesDescription());
                        RegistryRecord.ClearRedBoxWarningSetting();
                    }
                    else
                    {
                        this.HandleDuplicates(this.RegistryRecord.FoundDuplicates, false);
                        Session["EditRegistryModeEnable"] = false;
                    }
                }
                else
                {
                    RegistryRecord.ApplyEdit();
                    RegistryRecord.ModuleName = ChemDrawWarningChecker.ModuleName.REGISTRATION;
                    RegistryRecord.Save();
                   
                    if(!RegistryRecord.IsValid)
                    {                        
                        DisplayBrokenRules(this.RegistryRecord.GetBrokenRulesDescription());
                        RegistryRecord.ClearRedBoxWarningSetting();
                    }
                    else
                    {                        
                        Session["EditRegistryModeEnable"] = false;
                    }
                }


                if (CurrentPageState == PageState.EditBatch)
                {

                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.SetPageMode(PageState.ViewBatch, string.Format(Resource.EditedTemporaryRecord_Label_Text, RegistryRecord.ID.ToString()));
                            break;
                        case RegistryTypes.Mixture:
                            this.SetPageMode(PageState.ViewBatch, string.Format(Resource.EditedTemporaryMixture_Label_Text, RegistryRecord.ID.ToString()));
                            break;
                        case RegistryTypes.Component:
                            this.SetPageMode(PageState.ViewBatch, string.Format(Resource.EditedTemporaryComponent_Label_Text, RegistryRecord.ID.ToString()));
                            break;
                    }

                    this.DisplayCompoundIntoTree(RegistryRecord);
                }
                else if (CurrentPageState == PageState.EditComponent || CurrentPageState == PageState.EndComponent)
                {
                    this.DisplayComponent(this.CurrentComponentIndex);
                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DisplayStatusMessage(string.Format(Resource.EditedTemporaryRecord_Label_Text, RegistryRecord.ID.ToString()));
                            break;
                        case RegistryTypes.Mixture:
                            this.DisplayStatusMessage(string.Format(Resource.EditedTemporaryMixture_Label_Text, RegistryRecord.ID.ToString()));
                            break;
                        case RegistryTypes.Component:
                            this.DisplayStatusMessage(string.Format(Resource.EditedTemporaryComponent_Label_Text, RegistryRecord.ID.ToString()));
                            break;
                    }
                    this.DisplayCompoundIntoTree(RegistryRecord);
                }
                else
                {
                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.SetPageMode(PageState.ViewRecord, string.Format(Resource.EditedTemporaryRecord_Label_Text, RegistryRecord.ID.ToString()));
                            break;
                        case RegistryTypes.Mixture:
                            this.SetPageMode(PageState.ViewRecord, string.Format(Resource.EditedTemporaryMixture_Label_Text, RegistryRecord.ID.ToString()));
                            break;
                        case RegistryTypes.Component:
                            this.SetPageMode(PageState.ViewRecord, string.Format(Resource.EditedTemporaryComponent_Label_Text, RegistryRecord.ID.ToString()));
                            break;
                    }
                    this.DisplayCompoundIntoTree(RegistryRecord, null);
                }
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
            catch (ValidationException validationException)
            {
                this.DisplayBrokenRules(validationException, RegistryRecord.GetBrokenRulesDescription());
            }
            catch (Exception exception)
            {
                if (exception.Message.Contains("Attempted to read or write protected memory. This is often an indication that other memory is corrupt"))
                    _masterPage.DisplayErrorMessage(exception, false);
                else
                    _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        private void DisplayBrokenRules(List<BrokenRuleDescription> brokenRules)
        {
            DisplayBrokenRules(null, brokenRules);
        }

        private void DisplayBrokenRules(ValidationException validationException, List<BrokenRuleDescription> brokenRules)
        {
            string errorMessage = (validationException == null ? string.Empty : validationException.Message + "<BR/>");

            foreach (BrokenRuleDescription currentBrokenRule in brokenRules)
            {
                foreach (string currentError in currentBrokenRule.BrokenRulesMessages)
                    errorMessage += string.Format("{0}<br>", currentError);

            }
            _masterPage.DisplayErrorMessage(errorMessage, false);
        }

        /// <summary>
        /// Check if the current Component is editable according if has RegNum
        /// </summary>
        /// <param name="currentComponent">The component to check</param>
        /// <returns>Boolean according check</returns>
        private bool IsEditable(Component currentComponent)
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

                if (!string.IsNullOrEmpty(currentComponent.Compound.RegNumber.RegNum))
                    return false;
                return this.IsAuthorizedToEditCompound(CurrentComponentIndex);
            }
            finally
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
        }
        private bool IsAuthorizedToEditCompound(int currentComponentIndex)
        {
            return RegistryRecord.CanEditComponent(currentComponentIndex);
        }

        private bool IsAuthorizedToEditRegistry()
        {
            return RegistryRecord.CanEditRegistry();
        }

        protected override void DisableCOEForm(CambridgeSoft.COE.Framework.COEPageControlSettingsService.Control control)
        {
            if (control.ID == this.GetFormIndex(SubForms.ComponentForm).ToString() && CurrentComponentIndex >= 0)
            {
                if (!this.IsAuthorizedToEditCompound(CurrentComponentIndex))
                    ((COEFormGenerator)_mixtureCOEFormGroup.FindControl(GetFormIndex(SubForms.ComponentForm))).Enabled = false;
            }
            else
            {
                base.DisableCOEForm(control);
            }
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
                    if (IsPreRegistration && !isRegistration)
                    {
                        Server.Transfer(string.Format("{0}?SubmittedObjectId={1}", this.ResolveUrl(Resource.ReviewRegister_URL), this.RegistryRecord.ID));
                    }
                    else
                    {
                        if ((!string.IsNullOrEmpty(Request["IsSubmit"]) && Request["IsSubmit"] == "true"))
                        {

                            Server.Transfer(string.Format("{0}?IsSubmit=true&RegisteredObjectId={1}", this.ResolveUrl(Resource.ReviewRegister_URL), RegistryRecord.RegNumber.RegNum), false);
                        }
                        else
                        {
                            Server.Transfer(string.Format("{0}?RegisteredObjectId={1}", this.ResolveUrl(Resource.ReviewRegister_URL), RegistryRecord.RegNumber.RegNum), false);

                        }
                    }
                    break;
            }
        }

        private void DeleteRegistry()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            RegistryRecord.DeleteRegistryRecord(RegistryRecord.ID);
            //RegistryRecord.Delete();
            //RegistryRecord.Save();

            Server.Transfer(Resource.MoveDelete_URL + "?Action=REG_DELETE_TEMP");
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }


        private void RegisterMixture()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string message = string.Empty;

            try
            {
                _mixtureCOEFormGroup.Update();
                this.RegistryRecord.ModuleName = ChemDrawWarningChecker.ModuleName.REGISTRATION;
                this.RegistryRecord = this.RegistryRecord.Register(DuplicateCheck.CompoundCheck);
                
                if(!this.RegistryRecord.IsValid)
                {                    
                    DisplayBrokenRules(this.RegistryRecord.GetBrokenRulesDescription());
                }
                else
                {
                    this.HandleDuplicates(this.RegistryRecord.FoundDuplicates, true);
                }

            }
            catch (Csla.DataPortalException dataPortalException)
            {
                if (dataPortalException.Message.Contains("Attempted to read or write protected memory. This is often an indication that other memory is corrupt"))
                    throw new Exception(Resource.ChemscriptBusy_Exception, dataPortalException);
                else
                    throw dataPortalException;
            }
            catch (ValidationException ve)
            {
                DisplayBrokenRules(this.RegistryRecord.GetBrokenRulesDescription());
                if (RegistryRecord.GetBrokenRulesDescription().Count == 0)
                    _masterPage.DisplayErrorMessage(ve.Message, true);
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
        private void CancelEdit()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            RegistryRecord.CancelEdit();
            Session["EditRegistryModeEnable"] = false;
            if (CurrentPageState == PageState.EditBatch)
                this.SetPageMode(PageState.ViewBatch, string.Empty);
            else if (CurrentPageState == PageState.EditComponent)
                this.SetPageMode(PageState.ViewComponent, string.Empty);
            else
                this.SetPageMode(PageState.ViewRecord, string.Empty);

            _mixtureCOEFormGroup.DataBind();

            this.DisplayCompoundIntoTree(RegistryRecord);

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        private void EditMixture()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (!IsEditModeEnable)
            {
                IsEditModeEnable = true;
                RegistryRecord.BeginEdit();

                if (CurrentPageState == PageState.ViewBatch)
                    this.SetPageMode(PageState.EditBatch, string.Empty);
                else if (CurrentPageState == PageState.ViewComponent)
                    DisplayComponent(CurrentComponentIndex);
                else if (CurrentPageState == PageState.ViewRecord || CurrentPageState == PageState.End)
                    this.SetPageMode(PageState.EditRecord, string.Empty);
                else
                    DisplayComponent(CurrentComponentIndex);

                this.DisplayCompoundIntoTree(RegistryRecord);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void DeleteComponent()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            if (RegistryRecord.ComponentList.Count > 1)
            {
                EditMixture();
                int componentIndex = _mixtureCOEFormGroup.GetSubForm(GetFormIndex(SubForms.ComponentForm)).PageIndex;

                RegistryRecord.DeleteComponent(componentIndex);

                _mixtureCOEFormGroup.DataBind();

                //Original (and theoretically correct) code
                this.DisplayComponent(Math.Min(componentIndex, RegistryRecord.ComponentList.Count - 1));

                //Patch to avoid the issue of structure not refreshing.
                this.DisplayCompoundIntoTree(RegistryRecord, ((decimal)Math.Min(componentIndex, RegistryRecord.ComponentList.Count - 1)).ToString());
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        private void SearchCompound()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            if (this.IsEditModeEnable)
                _mixtureCOEFormGroup.Update();
            else
            {
                this.IsEditModeEnable = true;
                RegistryRecord.BeginEdit();
            }
            // Pick an existing Component will not be replaced by selected component in treeview
            if (AllowUnregisteredComponents)
            {
                this.RegistryRecord.AddComponent(Component.NewComponent());
                DisplayComponent(this.RegistryRecord.ComponentList.Count - 1);
            }
            Session["currentCompoundIndex"] = _mixtureCOEFormGroup.GetFormGeneratorPageIndex(GetFormIndex(SubForms.ComponentForm));
            Server.Transfer(Resource.SearchCompound_Page_URL + "?caller=RR", false);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void SearchCompound(int index)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                        MethodBase.GetCurrentMethod().Name);
            //_mixtureCOEFormGroup.Update();
            Session["currentCompoundIndex"] = index;

            Server.Transfer(Resource.SearchCompound_Page_URL + "?caller=RR", false);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        private void AddComponent()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            if (this.IsEditModeEnable)
            {
                _mixtureCOEFormGroup.Update();
            }
            else
            {
                this.IsEditModeEnable = true;
                RegistryRecord.BeginEdit();
            }

            RegistryRecord.AddComponent(Component.NewComponent());

            this._mixtureCOEFormGroup.DataBind();
            if (CurrentRegistryType == RegistryTypes.Mixture)
            { //here we go directly to add a component
                SearchCompound(this.RegistryRecord.ComponentList.Count - 1);
            }
            else
            {
                DisplayComponent(RegistryRecord.ComponentList.Count - 1);
                this.DisplayCompoundIntoTree(RegistryRecord, NodeKeywords.LastComponent.ToString());
                this.SetPageMode(PageState.AddComponent, string.Empty);
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
        }

        /// <summary>
        /// Method to find a RegNum of a mixture
        /// </summary>
        /// <returns>The RegNumber found</returns>
        private string GetRegistryRegNumberFromSession()
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

                return this.RegistryRecord.RegNumber.RegNum.ToString();
            }
            finally
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
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

                        RegistryRecord record = RegistryRecord.NewRegistryRecord();
                        record.InitializeFromXml(genericStorageBO.COEGenericObject, true, false);

                        this.RegistryRecord = record;
                    }
                    return true;
                }
                return false;

            }
            catch
            {
                return false;
            }
            finally
            {

                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                           MethodBase.GetCurrentMethod().Name);
            }
        }

        private void DisableEditDeleteButtons()
        {
            this.RegisterButton.Visible = this.EditButton.Visible = this.ComponentToolBar.Visible = false;
            this.ShowConfirmationMessage(Resource.IsNotEditable_Message);
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
            cs.RegisterStartupScript(typeof(ReviewRegisterMixture), "EnableDisaable", sStartupRunScripts.ToString(), true);

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
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            try
            {
                e.BusinessObject = RegistryRecord;
            }
            catch (DataPortalException) //No rows returned.
            {
                Server.Transfer("~/Forms/ContentArea/Messages.aspx?MessageCode=" + GUIShellTypes.MessagesCode.Error.ToString());
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        }

        protected void mixtureCslaDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            try
            {
                //First we have to map those control that support to create "virtual" objects client side, so then we can bind the values easily.
                //Create as many project as created by the control. 
                string listName = RegUtilities.FindKey("ProjectList", e.Values.Keys);
                if (e.Values[listName] is DataTable && this.RegistryRecord.ProjectList != null)
                    RegUtilities.SincronizeListDataTable((DataTable)e.Values[listName], "ProjectID", RegistryRecord.ProjectList, Project.NewProject);

                //Create as many identifiers as created by the control in client side. 
                listName = RegUtilities.FindKey("IdentifierList", e.Values.Keys);
                if (e.Values[listName] is DataTable && this.RegistryRecord.IdentifierList != null)
                    RegUtilities.SincronizeListDataTable((DataTable)e.Values[listName], "IdentifierID", RegistryRecord.IdentifierList, Identifier.NewIdentifier);

                COEDataMapper.Map(e.Values, this.RegistryRecord);
                e.RowsAffected = 1;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void BatchListCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = RegistryRecord.BatchList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void BatchListCslaDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                string listName = RegUtilities.FindKey("ProjectList", e.Values.Keys);
                Batch currentBatch = RegistryRecord.BatchList[(int)e.Keys["PageIndex"]];

                if (e.Values[listName] is DataTable)
                    RegUtilities.SincronizeListDataTable((DataTable)e.Values[listName], "ProjectID", currentBatch.ProjectList, Project.NewProject);

                listName = RegUtilities.FindKey("IdentifierList", e.Values.Keys);
                if (e.Values[listName] is DataTable)
                    RegUtilities.SincronizeListDataTable((DataTable)e.Values[listName], "IdentifierID", currentBatch.IdentifierList, Identifier.NewIdentifier);

                COEDataMapper.Map(e.Values, currentBatch);
                e.RowsAffected = 1;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void BatchComponentListCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = this.RegistryRecord.BatchList[this._mixtureCOEFormGroup.GetSubForm(this.GetFormIndex(SubForms.BatchForm)).PageIndex].BatchComponentList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void BatchComponentListCslaDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
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
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void ComponentListCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = RegistryRecord.ComponentList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        protected void ComponentListCslaDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                Component currentComponent = this.RegistryRecord.ComponentList[(int)e.Keys["PageIndex"]];
                if (this.CurrentPageState != PageState.ViewComponent && this.CurrentPageState != PageState.EndComponent)
                {
                    string listName = RegUtilities.FindKey("Compound.IdentifierList", e.Values.Keys);
                    if (e.Values[listName] is DataTable)
                        RegUtilities.SincronizeListDataTable((DataTable)e.Values[listName], "IdentifierID", currentComponent.Compound.IdentifierList, Identifier.NewIdentifier);

                    listName = RegUtilities.FindKey("Compound.BaseFragment.Structure.IdentifierList", e.Values.Keys);
                    if (e.Values[listName] is DataTable)
                        RegUtilities.SincronizeListDataTable((DataTable)e.Values[listName], "IdentifierID", currentComponent.Compound.BaseFragment.Structure.IdentifierList, Identifier.NewIdentifier);
                }
                COEDataMapper.Map(e.Values, currentComponent);
                e.RowsAffected = 1;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void prefixDropDownCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                e.BusinessObject = PrefixNameValueList.GetPrefixNameValueList();
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        /// <summary>
        /// CSBR 141834: Purpose to retrieve the list of all projects - active, inactive, Batch, Registration etc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AllProjectsCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = ProjectList.GetProjectList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void ProjectsCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = ProjectList.GetActiveProjectListByPersonID(COEUser.ID);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
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

        protected void ChemistsCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                e.BusinessObject = ChemistNameValueList.GetChemistNameValueList();
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void IdentifiersCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = IdentifierList.GetIdentifierList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void RegistryIdentifiersCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                switch (this.CurrentPageState)
                {
                    case PageState.ViewComponent:
                    case PageState.ViewBatch:
                    case PageState.ViewRecord:
                        e.BusinessObject = IdentifierList.GetAllIdentifierListByType(IdentifierTypeEnum.R);
                        break;
                    case PageState.EditComponent:
                    case PageState.EditBatch:
                    case PageState.EditRecord:
                        e.BusinessObject = RegUtilities.AddInActiveIdentifiers(IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.R), this.RegistryRecord, IdentifierTypeEnum.R);
                        break;
                    default:
                        e.BusinessObject = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.R);
                        break;
                }
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
                switch (this.CurrentPageState)
                {
                    case PageState.ViewComponent:
                    case PageState.ViewBatch:
                    case PageState.ViewRecord:
                        e.BusinessObject = IdentifierList.GetAllIdentifierListByType(IdentifierTypeEnum.C);
                        break;
                    case PageState.EditComponent:
                    case PageState.EditBatch:
                    case PageState.EditRecord:
                        e.BusinessObject = RegUtilities.AddInActiveIdentifiers(IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C), this.RegistryRecord, IdentifierTypeEnum.C);
                        break;
                    default:
                        e.BusinessObject = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.C);
                        break;
                }
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
                switch (this.CurrentPageState)
                {
                    case PageState.ViewComponent:
                    case PageState.ViewBatch:
                    case PageState.ViewRecord:
                        e.BusinessObject = IdentifierList.GetAllIdentifierListByType(IdentifierTypeEnum.B);
                        break;
                    case PageState.EditComponent:
                    case PageState.EditBatch:
                    case PageState.EditRecord:
                        e.BusinessObject = RegUtilities.AddInActiveIdentifiers(IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.B), this.RegistryRecord, IdentifierTypeEnum.B);
                        break;
                    default:
                        e.BusinessObject = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.B);
                        break;
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        protected void StatusCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            try
            {
                string databaseName = COEConfiguration.GetDatabaseNameFromAppName(CambridgeSoft.COE.Framework.Common.COEAppName.Get().ToString());
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                e.BusinessObject = PickListNameValueList.GetPickListNameValueList(databaseName, "Status");
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void FragmentTypesCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            try
            {
                e.BusinessObject = FragmentNameValueList.GetFragmentTypesNameValueList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void FragmentNamesCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            //If we have to keep the same fragments along the components, no need to bind to the entire list of available fragmetns.
            try
            {
                e.BusinessObject = FragmentList.GetFragmentList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void FragmentsCslaDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

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
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void FragmentsCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                int batchIndex = Math.Min(RegistryRecord.BatchList.Count - 1, this._mixtureCOEFormGroup.GetSubForm(this.GetFormIndex(SubForms.BatchForm)).PageIndex);
                e.BusinessObject = RegistryRecord.BatchList[batchIndex].BatchComponentList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void BatchComponentFragmentsCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
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
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void BatchComponentFragmentsCslaDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                int batchIndex = this._mixtureCOEFormGroup.GetSubForm(this.GetFormIndex(SubForms.BatchForm)).PageIndex;
                COEDataMapper.Map(e.Values, RegistryRecord.BatchList[batchIndex]);
                //This call checks that all the fragments that are inside the BatchComponents also are part at Component level.
                this.RegistryRecord.UpdateFragments();
                e.RowsAffected = 1;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void SequenceListCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
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
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void StructureIdentifiersCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                switch (this.CurrentPageState)
                {
                    case PageState.ViewComponent:
                    case PageState.ViewBatch:
                    case PageState.ViewRecord:
                        e.BusinessObject = IdentifierList.GetAllIdentifierListByType(IdentifierTypeEnum.S);
                        break;
                    case PageState.EditComponent:
                    case PageState.EditBatch:
                    case PageState.EditRecord:
                        e.BusinessObject = RegUtilities.AddInActiveIdentifiers(IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.S), this.RegistryRecord, IdentifierTypeEnum.S);
                        break;
                    default:
                        e.BusinessObject = IdentifierList.GetIdentifierListByType(IdentifierTypeEnum.S);
                        break;
                }
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
