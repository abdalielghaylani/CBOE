using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using PerkinElmer.COE.Registration.Server.Forms.Public.UserControls;

using Resources;

using Csla.Validation;
using Infragistics.WebUI.UltraWebNavigator;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using CambridgeSoft.COE.Framework.COEFormService;
using CS = CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.Common.Exceptions;
using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Registration.Services.Types;
using PerkinElmer.COE.Registration.Server.Code;
using CambridgeSoft.COE.Registration;
using System.Linq;
using PerkinElmer.COE.Registration.Server.Controls;

namespace PerkinElmer.COE.Registration.Server.Forms.RegisteredRecord.ContentArea
{
    public partial class ViewMixture : GUIShellPage
    {
        #region GUIShell Variables

        RegistrationMaster _masterPage = null;
        private string _controlToListen = String.Empty;
        private string _groupToListen = String.Empty;
        private const string ACTIONSTRING = "Action";
        RegistryTypes _currentRegistryType;        

        /// <summary>
        /// These are the controls which I can be interested in the Control(MyCustomizedControl)
        /// inside the Accordion Group.
        /// </summary>
        private enum ControlsToTakeCare
        {
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

        /// <summary>
        /// Indicates if the BatchComponentFragmentList of the BatchComponent related to current Component is potentially updated.
        /// </summary>
        private bool BatchComponentFragmentListIsModified
        {
            get
            {
                if (this.RegistryRecord != null)
                    return this.RegistryRecord.BatchList[CurrentBatchIndex].BatchComponentList[CurrentComponentIndex].BatchComponentFragmentList.IsDirty;
                else
                    return false;
            }
        }

        #endregion

        #region Page Variables

        private bool AddingBatch
        {
            get
            {
                return ViewState["AddingBatch"] == null ? false : (bool)ViewState["AddingBatch"];
            }
            set
            {
                ViewState["AddingBatch"] = value;
            }
        }

        private enum PageState
        {
            None,
            ViewRecord,
            EditRecord,
            AddComponent,
            ViewComponent,
            ViewSingleCompound,
            EditComponent,
            ViewBatch,
            AddBatch,
            EditBatch,
            End
        }

        private RegistryRecord RegistryRecord
        {
            get { return (RegistryRecord)Session[Constants.MultiCompoundObject_Session]; }
            set { SetRegistryRecord(value); }
        }

        RegistryRecord OriginalRegistryRecord
        {
            get
            {

                return (RegistryRecord)Session["OriginalRegistryRecord"];
            }
            set
            {

                Session["OriginalRegistryRecord"] = value;
            }
        }

        private RegistryRecord GetRegistryRecord()
        {
            string regNum = Request[Constants.RegNum_UrlParameter];
            string fullRegNum = Request[Constants.FullRegNum_UrlParameter];
            string objectId = Request[Constants.RegisteredObjectId_UrlParameter];

            //Use the current session-scoped RegistryRecord if one is defined
            RegistryRecord rec = this.RegistryRecord;
            if (IsCurrentRegistry(rec, regNum, fullRegNum, objectId))
                return rec;

            RegistryRecord fetchedRecord = null;
            try
            {
                if (!string.IsNullOrEmpty(regNum))
                {
                    fetchedRecord = RegistryRecord.GetRegistryRecord(regNum);
                    if (fetchedRecord == null)
                        throw new BusinessObjectNotFoundException(regNum, typeof(RegistryRecord));
                }
                else if (!string.IsNullOrEmpty(fullRegNum))
                {
                    fetchedRecord = RegistryRecord.GetRegistryRecordByBatch(fullRegNum);
                    if (fetchedRecord == null)
                        throw new BusinessObjectNotFoundException(fullRegNum, typeof(Batch));
                }
                else if (!string.IsNullOrEmpty(objectId))
                {
                    //Try using it as a RegNum, then as a FullRegNum (for more friendly URL from BioAssay)
                    try
                    {
                        fetchedRecord = RegistryRecord.GetRegistryRecord(objectId);
                    }
                    catch
                    {
                        fetchedRecord = RegistryRecord.GetRegistryRecordByBatch(objectId);
                    }
                    finally
                    {
                        if (fetchedRecord == null)
                            throw new BusinessObjectNotFoundException(objectId, typeof(RegistryRecord));
                    }
                }

            }
            catch (BusinessObjectNotFoundException bex)
            {
                Response.Redirect("~/Forms/Public/ContentArea/Messages.aspx?MessageCode="
                    + GUIShellTypes.MessagesCode.RecordNotFound.ToString());
            }
            catch (Exception ex)
            {
                Response.Redirect("~/Forms/Public/ContentArea/Messages.aspx?MessageCode="
                    + GUIShellTypes.MessagesCode.Error.ToString());
            }

            Session["DocManagerDocs"] = null;
            Session["IsRegistryLocked"] = null;
            this.RegistryRecord = fetchedRecord;
            UpdateLocked();

            //redirect to the error page if no RegistryRecord session object was set
            if (this.RegistryRecord == null)
                _masterPage.DisplayErrorMessage(Resource.NotRecordFound_MasterPage, true);

            return this.RegistryRecord;
        }

        private bool IsCurrentRegistry(RegistryRecord currentRecord, string regNumber, string batchRegNumber, string objectId)
        {
            if (regNumber != null && currentRecord != null && currentRecord.RegNum == regNumber)
                return true;
            if (objectId != null && currentRecord != null && currentRecord.RegNum == objectId)
                return true;

            if (!string.IsNullOrEmpty(batchRegNumber) && currentRecord != null)
            {
                BatchList batches = currentRecord.BatchList;
                if (batches != null)
                {
                    foreach (Batch b in batches)
                    {
                        if (b.FullRegNumber == batchRegNumber)
                            return true;
                    }
                }
            }

            return false;
        }

        private void SetRegistryRecord(RegistryRecord value)
        {
            if (Session[Constants.MultiCompoundObject_Session] != null && !RegistryRecord.ReferenceEquals(value, Session[Constants.MultiCompoundObject_Session]))
                ((IDisposable)Session[Constants.MultiCompoundObject_Session]).Dispose();

            Session[PerkinElmer.COE.Registration.Server.Constants.MultiCompoundObject_Session] = value;
        }

        private enum SubForms
        {
            MixtureDrawingForm = 0,
            ComponentForm = 1,
            BatchComponentForm = 3,
            BatchForm = 4,
            BatchComponentFragmentsForm = 5,
            BatchComponentFragmentsFormEdit = 6,
            BatchFragmentList = 7,
            DocManagerForm = 8,
            InvContainerForm = 9,
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

        int CurrentBatchIndex
        {
            get
            {
                return ViewState["CurrentBatchIndex"] == null ? 0 : (int)ViewState["CurrentBatchIndex"];
            }
            set
            {
                ViewState["CurrentBatchIndex"] = value;
            }
        }

        int CurrentComponentIndex
        {
            get
            {
                return ViewState["CurrentComponentIndex"] == null ? -1 : (int)ViewState["CurrentComponentIndex"];
            }
            set
            {
                ViewState["CurrentComponentIndex"] = value;
            }
        }

        private bool IsEditModeEnable
        {
            get
            {
                bool retVal = false;
                if (ViewState["EditRegistryModeEnable"] != null)
                    bool.TryParse(ViewState["EditRegistryModeEnable"].ToString(), out retVal);

                return retVal;
            }

            set
            {
                ViewState["EditRegistryModeEnable"] = value;
            }
        }

        int PageFormGroupID
        {
            get
            {
                return RegUtilities.GetCOEFormID("ViewRegistryFormGroupId");
            }
        }

        #endregion

        #region Page Properties

        private string _pageMode;
        private string _compoundIdToAdd;
        private bool _structureChanged = false;
        private bool _saveAddbatch = true;

        /// <summary>
        /// Represent the centralized display culture configuration
        /// </summary>
        private string DisplayCulture
        {
            get
            {
                if (Session["DisplayCulture"] == null)
                    Session["DisplayCulture"] = CS.ConfigurationUtilities.GetApplicationData(COEAppName.Get().ToString()).DisplayCulture;
                return (string)Session["DisplayCulture"];
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


        #endregion

        #region Events Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.CheckSearchParams();

                this.ConfirmationMessageBox.Visible = false;
                this.DuplicatesMessageBox.Visible = false;
                this.StoppingMessageBox.Visible = false;
                this.CopiesMessageBox.Visible = false;
                this.DeleteRecordImageMenuButton.Enabled = true;
                this.MessagesAreaUserControl.Visible = false;
                if (!Page.IsPostBack)
                {
                    _compoundIdToAdd = Request[Constants.RegisteredCompoundId_UrlParameter];
                    _pageMode = Request[Constants.CurrentPageState_UrlParameter];


                    if (_compoundIdToAdd != null)
                    {
                        this.LoadControlInAccordionPanel();
                        this.SetControlsAttributtes();
                        this.ReplaceExistingComponent(_compoundIdToAdd);
                        IsEditModeEnable = true;
                        SetPageMode(PageState.ViewComponent, string.Empty);
                    }
                    else
                    {
                        this.RegistryRecord = GetRegistryRecord();
                        this.LoadControlInAccordionPanel();
                        this.SetControlsAttributtes();

                        if (_pageMode != null)
                        {
                            string message = string.Empty;

                            if (this.ContainsRequestParameterEnd())
                                switch (CurrentRegistryType)
                                {
                                    case RegistryTypes.Both:
                                        message = string.Format(Resource.EditedRecord_Label_Text, RegistryRecord.RegNumber.RegNum);
                                        break;
                                    case RegistryTypes.Mixture:
                                        message = string.Format(Resource.EditedMixture_Label_Text, RegistryRecord.RegNumber.RegNum);
                                        break;
                                    case RegistryTypes.Component:
                                        message = string.Format(Resource.EditedComponent_Label_Text, RegistryRecord.RegNumber.RegNum);
                                        break;
                                }

                            if (_pageMode.ToLower().Contains("edit"))
                                this.IsEditModeEnable = true;

                            if (((PageState)Enum.Parse(typeof(PageState), _pageMode, true) == PageState.EditBatch) && this.RegistryRecord.ComponentList.Count < 2)
                            {
                                this.IsEditModeEnable = false;
                                this.SetPageMode(PageState.ViewRecord, Resource.ViewEditBatch_Mode_Message);
                            }
                            else
                            {
                                try
                                {
                                    this.SetPageMode((PageState)Enum.Parse(typeof(PageState), _pageMode, true), message);
                                }
                                catch
                                {
                                    _masterPage.DisplayErrorMessage(Resource.CurrentPageState_ErrorMessage, false);
                                }
                            }
                            if (((PageState)Enum.Parse(typeof(PageState), _pageMode, true) == PageState.ViewBatch))
                            {
                                if (!string.IsNullOrEmpty(Request[Constants.CurrentBatchIndex_UrlParameter]))
                                {
                                    int currentBatchIndex = int.Parse(Request[Constants.CurrentBatchIndex_UrlParameter]);
                                    this.DisplayBatch(currentBatchIndex);
                                }
                            }
                        }
                        else
                        {
                            this.SetPageMode(PageState.ViewRecord, string.Empty);
                        }
                    }
                    if (this.CurrentPageState == PageState.ViewBatch)
                        this.DisplayCompoundIntoTree(this.RegistryRecord, "1|" + (this.CurrentBatchIndex + 1).ToString());
                    else
                        this.DisplayCompoundIntoTree(RegistryRecord);
                }
                else if (this.RegistryRecord == null)
                    _masterPage.DisplayErrorMessage(Resource.RecordNotFound_Label_Text, false);

                this.SetPageTitle();
                this.SubscribeToEventsInAccordionControl();
                _masterPage.MakeCtrlShowProgressModal(this.SubmitButton.ClientID, "Updating...", string.Empty);
                this.DefineMixtureImageMenuButton.MenuItemList[2].OnClientClick = @"if(RemoveComponent('" + this.ActionToDoHiddenField.ClientID + "') == false) return false;";
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        public bool ContainsRequestParameterEnd()
        {
            try
            {
                if (Request[Constants.CurrentPageState_UrlParameter] != null)
                {
                    string mode = Request[PerkinElmer.COE.Registration.Server.Constants.CurrentPageState_UrlParameter] as string;
                    switch (mode.ToLower())
                    {
                        case "end":
                            return true;
                    }
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, true);
            }
            return false;
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

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
                    ClearOutputValue('0');    
                    document.getElementById('" + this.ActionToDoHiddenField.ClientID + @"').value = 'null';
                  }");
                }

                //according rls status is going to add the css classes to make projects grids look like required.
                this.Header.Controls.Add(RegUtilities.GetRLSCSSClass(this.RegistryRecord.RLSStatus));

                string closeModalTitle = "CloseModal";
                string url = Page.ResolveUrl(Resource.ViewMixture_URL) + "?" + Constants.RegisteredObjectId_UrlParameter + "=" + this.RegistryRecord.RegNum + "&" + Constants.CurrentPageState_UrlParameter + "=" + PageState.ViewRecord.ToString() + "&" + Constants.CurrentBatchIndex_UrlParameter + "=" + this.CurrentBatchIndex;
                string closeModalJS = @"
    function CloseModal(refresh)
    {
        if(typeof(CloseCOEModalIframe) == 'function')
            CloseCOEModalIframe();

        if(refresh)
            self.location.href = '" + url + @"';
    }

    function HideObject(obj) {
        var element = document.getElementById(obj);
        element.style.display = 'none';
    }

    function ShowModalFrame(url, headerText)
    {
        if(typeof(InitModalIframeControl_" + this.ModalFrame.ClientID + @") == 'function')
            InitModalIframeControl_" + this.ModalFrame.ClientID + @"(url, headerText, true);
    }";
                if (!Page.ClientScript.IsClientScriptBlockRegistered(closeModalTitle))
                    Page.ClientScript.RegisterClientScriptBlock(typeof(ViewMixture), closeModalTitle, closeModalJS, true);
                this.RenderJSArrayWithCliendIds();
                SetFormToReadOnly();
                base.OnPreRenderComplete(e);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void SetNewContainerURL()
        {
            #region SendToInventory OnClientClick -- OnInit is too early as it needs RegistyRecord and Current Batch filled in --
            //If no batch has been selected, it is preferred to use the latest added. The default property prefers the fist instead...
            if (RegUtilities.GetInventoryIntegration())
            {
                int currentBatchIndexOverride = ViewState["CurrentBatchIndex"] == null ? RegistryRecord.BatchList.Count - 1 : CurrentBatchIndex;

                // Under some scenarios the batch index stored in viewstate is out of bounds. For instance when cancelling a batch addition.
                if (currentBatchIndexOverride >= RegistryRecord.BatchList.Count)
                    currentBatchIndexOverride = RegistryRecord.BatchList.Count - 1;

                string url = string.Empty;
                if (RegUtilities.GetUseFullContainerForm())
                {
                    url = string.Format("{0}{1}{2}={3}&RefreshOpenerLocation=true",
                                        RegUtilities.GetNewInventoryContainerURL(),
                                        RegUtilities.GetNewInventoryContainerURL().Contains("?") ? "&" : "?",
                                        "vRegBatchID",
                                        RegistryRecord.BatchList[currentBatchIndexOverride].ID);
                }
                else
                {
                    url = string.Format("{0}{1}{2}={3}&OpenAsModalFrame=true",
                                        RegUtilities.GetSendToInventoryURL(),
                                        RegUtilities.GetSendToInventoryURL().Contains("?") ? "&" : "?",
                                        "RegIDList",
                                        RegistryRecord.RegNumber.ID);
                }
                string HideObj = "ChemBioPluginContainer";
                this.SendToInventoryImageMenuButton.OnClientClick = "HideObject('" + HideObj + "');ShowModalFrame('" + url + "', '" + Resource.NewContainer_Header_Text + "'); return false;";               
            }
            #endregion
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
        /// Set the reference to the JScript Page.
        /// </summary>
        private void SetJScriptReference()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string jScriptKey = this.ID.ToString() + "CommonScriptsPage";

            /*CSBR # - 117333
             * Changed by - Soorya Anwar
             * Date - 25-Jun-2010
             * Purpose - To register the commonUtilities.js file,
             *           from which the OpenNewWindow is to be invoked for popping up Inventory page as new window.
             */
            string jScriptUtilitiesKey = this.ID.ToString() + "CommonUtilitiesPage";
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(jScriptUtilitiesKey))
                Page.ClientScript.RegisterClientScriptInclude(jScriptUtilitiesKey, Page.ResolveUrl("~/Forms/Public/JScripts/CommonUtilities.js"));
            //End of Change for CSBR-117333
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(jScriptKey))
                Page.ClientScript.RegisterClientScriptInclude(jScriptKey, Page.ResolveUrl("~/Forms/Public/JScripts/CommonScripts.js"));

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void DuplicatesMessageBox_OkClicked(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                MessagesBox messageBox = (MessagesBox)sender;
                if (messageBox.Attributes["DuplicateRecords"] == bool.TrueString)
                {
                    IsEditModeEnable = true;
                    SubmitEdition(false, false);
                }
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

        protected void DuplicatesMessageBox_CancelClicked(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                //this.GetRegistryRecordFromDB();
                CancelRecordEdit();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void ConfirmationMessageBox_NoClicked(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                //this.GetRegistryRecordFromDB();
                CancelRecordEdit();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void ConfirmationMessageBox_YesClicked(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                SubmitEdition(false, true);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        }

        protected void CopiesMessageBox_CancelClicked(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            try
            {
                CancelRecordEdit();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void CopiesMessageBox_ContinueClicked(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                SubmitEdition(false, true);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void CopiesMessageBox_CopyClicked(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            CopiesAction action = (CopiesAction)Enum.Parse(typeof(CopiesAction), ((MessagesBox)sender).Attributes[ACTIONSTRING]);

            try
            {
                CreateCopies(action);
                if (action == CopiesAction.CopyComponentWithDupCheck || action == CopiesAction.CopyStructureWithDupCheck)
                {
                    SubmitEdition(false, true);
                }
                else
                {
                    SubmitEdition(false, false);
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void DeleteRegButton_Click(object sender, EventArgs e)
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

        void DefineMixtureImageMenuButton_Command(object sender, CommandEventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
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
                        this.DisplayBatch(0);
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

        void DeleteRecordImageMenuButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                    MethodBase.GetCurrentMethod().Name);
            try
            {
                this.DeleteRegistry();
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
        /// Transfers to the page where the delete happens
        /// </summary>
        private void DeleteRegistry()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            Server.Transfer("MoveDelete.aspx?Action=REG_DELETE&RegID=" + this.RegistryRecord.RegNumber.RegNum, false);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void CreateCopies(CopiesAction action)
        {

            if (action == CopiesAction.CopyComponent || action == CopiesAction.CopyComponentWithDupCheck)
                RegistryRecord.CopyEditedComponents();
            else if (action == CopiesAction.CopyStructure || action == CopiesAction.CopyStructureWithDupCheck)
                RegistryRecord.CopyEditedStructures();
        }

        protected void StoppingMessageBox_OkClicked(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                CancelRecordEdit();
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
            if (this.Master is RegistrationMaster)
            {
                _masterPage = (RegistrationMaster)this.Master;
                //Set the accordion key where is the customized control inside.
                this.AccordionGroupToListen = "SearchRegistryRecord";
                //Set the cotrolID inside accordion group.
                this.AccordionControlToListen = "RegistryRecordNavigation";
            }
            #endregion
            try
            {
                this.LoadFormGenerators();
                base.OnPreInit(e);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected override void OnInit(EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                base.OnInit(e);
                this.DefineMixtureImageMenuButton.Text = Resource.DefineMixtureImageMenuButton_text;
                this.DefineMixtureImageMenuButton.MenuItemList[0].Text = Resource.AddComponent_Button_Text;
                this.DefineMixtureImageMenuButton.MenuItemList[1].Text = Resource.Select_Button_Text;
                this.DefineMixtureImageMenuButton.MenuItemList[2].Text = Resource.DeleteComponent_Button_Text;
                this.DefineMixtureImageMenuButton.ValidateAddComponent = true;
                this.SendToInventoryImageMenuButton.Text = Resource.SendToInventoryMenuButton_Text;
                this.SetJScriptReference();
                this.DefineMixtureImageMenuButton.Command += new CommandEventHandler(DefineMixtureImageMenuButton_Command);
                this.DeleteRecordImageMenuButton.Click += new EventHandler(DeleteRecordImageMenuButton_Click);
                UpdateInvSettings();
                UpdateLocked();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected override void OnPreRender(EventArgs e)
        {
            SetNewContainerURL();
            if (CurrentPageState == PageState.ViewBatch || CurrentPageState == PageState.AddBatch || CurrentPageState == PageState.EditBatch)
            {
                if (RegistryRecord.BatchList.Count > CurrentBatchIndex && !RegistryRecord.BatchList[CurrentBatchIndex].IsBatchEditable)
                    this.DisableEditDeleteButtons();
            }
            else if (this.RegistryRecord != null && !this.RegistryRecord.IsEditable)
            {
                this.DisableEditDeleteButtons();
            }
            else if (this.RegistryRecord != null && !this.RegistryRecord.IsRegistryDeleteable && CurrentBatchIndex == 0) // show only for registry, component & 1st batch; since the delete link is shown only there
            {   // to decide whether the registry delete link should be displayed or not
                // user can delete only if he has access to all the batches
                this.DisableRegistryDeleteLink();
            }
            base.OnPreRender(e);
            UpdateLocked();


        }

        private void UpdateLocked()
        {
            if (this.RegistryRecord != null)
            {
                string previousValue = (string)Session["IsRegistryLocked"];

                if (this.RegistryRecord.Status == RegistryStatus.Locked && RegUtilities.GetLockingEnabled())
                    Session["IsRegistryLocked"] = bool.TrueString;
                else
                    Session["IsRegistryLocked"] = bool.FalseString;

                if (previousValue == null || !previousValue.Equals(Session["IsRegistryLocked"]))
                    RegUtilities.SetCOEPageSettings(true);
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
                this.CancelRecordEdit();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
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

        protected void AddBatchButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.AddBatch();
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

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                _mixtureCOEFormGroup.Update();
                if (AddingBatch)
                {
                    // since all we are doing is adding a batch there is no need to duplicate check
                    _saveAddbatch = true;
                    this.SubmitEdition(false, false);
                    AddingBatch = !(_saveAddbatch);
                }
                else
                {
                    this.SubmitEdition(RegistryRecord.ComponentList.IsDirty, true);
                    AddingBatch = false;
                }

            }
            catch (ValidationException validationException)
            {
                DisplayBrokenRules(validationException, RegistryRecord.GetBrokenRulesDescription());
            }
            catch (Exception exception)
            {
                //if (this.CurrentPageState == PageState.AddBatch)
                //    this.SetPageMode(PageState.EditBatch, String.Empty);

                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void MoveBatchButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.MoveDeleteBatch(true);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }

        }

        protected void DeleteBatchButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.MoveDeleteBatch(false);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
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

        #endregion

        #region GUIShell Methods

        protected override void SetControlsAttributtes()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            switch (CurrentRegistryType)
            {
                case RegistryTypes.Both:
                    this.PageTitleLabel.Text = Resource.ViewRecord_Page_Title;
                    break;
                case RegistryTypes.Mixture:
                    this.PageTitleLabel.Text = Resource.ViewMixture_Page_Title;
                    break;
                case RegistryTypes.Component:
                    this.PageTitleLabel.Text = Resource.ViewComponent_Page_Title;
                    break;
            }

            this.EditButton.Text = Resource.Edit_Button_Text;
            this.CancelButton.Text = Resource.Cancel_Button_Text;
            this.AddBatchButton.Text = Resource.AddBatch_Button_Text;
            this.SubmitButton.Text = Resource.Save_Button_Text;
            this.SubmitButton.ToolTip = string.Format(Resource.Save_Button_Tooltip_Perm, RegistryRecord.RegNum);

            this.GoHomeButton.Text = this.GetLeaveButtonLabel();
            if (this.GoHomeButton.Text == Resource.Back_Button_Text)
                this.GoHomeButton.ToolTip = this.GetLeaveButtonTooltip();

            this.MoveBatchButton.Text = Resource.MoveBatch_Button;
            this.DeleteBatchButton.Text = Resource.DeleteBatch_Button;
            this.DeleteRecordImageMenuButton.Text = Resource.DeleteRegistryButton_Text;
            this.ModalFrame.HeaderText = Resource.NewContainer_Header_Text;
            //Accordion control attributes
            Control showButtonsIDHidden = _masterPage.GetControlInsideAnAccordionGroup(this.AccordionGroupToListen, ControlsToTakeCare.ShowButtonsHidden.ToString());
            if (showButtonsIDHidden != null)
                ((HiddenField)showButtonsIDHidden).Value = bool.FalseString;

            switch (CurrentRegistryType)
            {
                case RegistryTypes.Both:
                    this.DisplayStatusMessage(Resource.ViewRecord_Label_Text);
                    break;
                case RegistryTypes.Mixture:
                    this.DisplayStatusMessage(Resource.ViewMixture_Label_Text);
                    break;
                case RegistryTypes.Component:
                    this.DisplayStatusMessage(Resource.ViewComponent_Label_Text);
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
                    if (IsEditModeEnable)
                    {
                        this.Page.Title = Resource.ViewRecord_Page_Title;
                        this.Header.Title = Resource.ViewRecord_Page_Title;
                    }
                    else
                    {
                        this.Page.Title = Resource.EditRecord_Label_Text;
                        this.Header.Title = Resource.EditRecord_Label_Text;

                    }
                    break;
                case RegistryTypes.Mixture:
                    if (IsEditModeEnable)
                    {
                        this.Page.Title = Resource.ViewMixture_Page_Title;
                        this.Header.Title = Resource.ViewMixture_Page_Title;
                    }
                    else
                    {
                        this.Page.Title = Resource.EditMixture_Label_Text;
                        this.Header.Title = Resource.EditMixture_Label_Text;

                    }
                    break;
                case RegistryTypes.Component:
                    if (IsEditModeEnable)
                    {
                        this.Page.Title = Resource.ViewComponent_Page_Title;
                        this.Header.Title = Resource.ViewComponent_Page_Title;
                    }
                    else
                    {
                        this.Page.Title = Resource.EditComponent_Label_Text;
                        this.Header.Title = Resource.EditComponent_Label_Text;

                    }
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
                if (CurrentPageState != PageState.End)
                    _mixtureCOEFormGroup.Update();

                int index = int.Parse(clickedNode.DataKey.ToString().Substring(1));
                switch (clickedNode.DataKey.ToString()[0])
                {
                    case 'C':
                        this.DisplayCompound(index);
                        break;
                    case 'B':
                        this.DisplayBatch(index);
                        break;
                    default:
                        this.DisplayRecord();
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
            registryRecordNavigationControlRow.ParentID = this.AccordionGroupToListen;
            registryRecordNavigationControlRow.ControlType = "UserControl";
            if (registryRecordNavigationControlRow.ControlType == GUIShellTypes.SupportedControlTypes.UserControl.ToString())
                registryRecordNavigationControlRow.ControlSource = GUIShellTypes.UCPath + "RegistryRecordNavigation.ascx";
            registryRecordNavigationControlRow.ID = this.AccordionControlToListen;
            COENavigationPaneObject.ControlItem.AddControlItemRow(registryRecordNavigationControlRow);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }


        private void DisplayBrokenRules(List<BrokenRuleDescription> brokenRules)
        {
            this.DisplayBrokenRules(null, brokenRules);
        }

        private void DisplayBrokenRules(Exception exception, List<BrokenRuleDescription> brokenRules)
        {
            string errorMessage = exception == null ? string.Empty : exception.Message + "<BR/>";

            foreach (BrokenRuleDescription currentBrokenRule in brokenRules)
            {
                foreach (string currentError in currentBrokenRule.BrokenRulesMessages)
                    errorMessage += string.Format("{0}<br>", currentError);
            }
            _masterPage.DisplayErrorMessage(errorMessage, false);
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
            if (tree != null) //Covrity fix - CID 11864 
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
            if (tree != null) //Covrity fix - CID 11865
            {
                tree.NewRegistryType = this.CurrentRegistryType.ToString();
                tree.DataBind(registryRecord, nodeToShowSelected);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The method will check if it's the first time you visit the page, or you come after saving a compound.
        /// </summary>
        /// <remarks>In this method you have to make visible the rows that you pretend</remarks>
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
                //FormGroup coeFormHolder = BuildCoeFormHolder("../FormGeneratorXml/ViewMixture.xml");
                FormGroup coeFormHolder = COEFormBO.Get(this.PageFormGroupID).COEFormGroup;
                _mixtureCOEFormGroup = new COEFormGroup();
                _mixtureCOEFormGroup.ID = "ViewMixtureFormGroup";

                mixtureInformationHolder.Controls.Clear();
                mixtureInformationHolder.Controls.Add(_mixtureCOEFormGroup);

                _mixtureCOEFormGroup.FormGroupDescription = coeFormHolder;
                _mixtureCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;

                _mixtureCOEFormGroup.DisplayCulture = this.DisplayCulture;
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
            PageState forcedMode = PageState.None;
            EnableDisableMixtureTree(true);

            switch (mode)
            {
                case PageState.ViewRecord:

                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;

                    SetFormGeneratorVisibility(SubForms.MixtureDrawingForm, true);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.DocManagerForm, IsAuthorizedToEditRecord());
                    SetFormGeneratorVisibility(SubForms.InvContainerForm, true);
                    SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentFragmentsForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentFragmentsFormEdit, false);

                    this.DeleteRecordImageMenuButton.Visible = true;
                    /*CSBR-154565
                     *Modified By DIVYA
                     Added to enable or disable SendToInventory link depending upon configuration settings.
                     */
                    this.SendToInventoryImageMenuButton.Visible = RegUtilities.IsSendToInventoryEnabled();
                    // End of CSBR-154565

                    this.MoveBatchButton.Visible = this.DeleteBatchButton.Visible = false;



                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DisplayStatusMessage(Resource.ViewRecord_Label_Text);
                            break;
                        case RegistryTypes.Mixture:
                            this.DisplayStatusMessage(Resource.ViewMixture_Label_Text);
                            break;
                        case RegistryTypes.Component:
                            this.DisplayStatusMessage(Resource.ViewComponent_Label_Text);
                            break;
                    }

                    break;
                case PageState.ViewComponent:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;

                    SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);

                    SetFormGeneratorVisibility(SubForms.MixtureDrawingForm, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.DocManagerForm, false);
                    SetFormGeneratorVisibility(SubForms.InvContainerForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);

                    this.DeleteRecordImageMenuButton.Visible = false;
                    this.MoveBatchButton.Visible = this.DeleteBatchButton.Visible = false;

                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DisplayStatusMessage(Resource.ViewRecord_Label_Text);
                            break;
                        case RegistryTypes.Mixture:
                            this.DisplayStatusMessage(Resource.ViewMixture_Label_Text);
                            break;
                        case RegistryTypes.Component:
                            this.DisplayStatusMessage(Resource.ViewComponent_Label_Text);
                            break;
                    }
                    break;
                case PageState.ViewSingleCompound:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;

                    SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.BatchForm, true);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.InvContainerForm, true);

                    SetFormGeneratorVisibility(SubForms.DocManagerForm, false);
                    SetFormGeneratorVisibility(SubForms.MixtureDrawingForm, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);

                    this.DeleteRecordImageMenuButton.Visible = false;
                    this.MoveBatchButton.Visible = this.DeleteBatchButton.Visible = false;
                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DisplayStatusMessage(Resource.ViewRecord_Label_Text);
                            break;
                        case RegistryTypes.Mixture:
                            this.DisplayStatusMessage(Resource.ViewMixture_Label_Text);
                            break;
                        case RegistryTypes.Component:
                            this.DisplayStatusMessage(Resource.ViewComponent_Label_Text);
                            break;
                    }
                    break;
                case PageState.EditComponent:
                    // Component can be editable if the user can edit Registry (Non-Mixture mode)
                    if ((IsAuthorizedToEditCompound(this.CurrentComponentIndex) || IsAuthorizedToEditRegistry()) && IsAuthorizedToEditRecord() && CurrentRegistryType != RegistryTypes.Mixture)
                    {
                        _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Edit;
                    }
                    else
                    {
                        forcedMode = PageState.ViewComponent;
                        _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;
                    }

                    SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);

                    SetFormGeneratorVisibility(SubForms.MixtureDrawingForm, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.DocManagerForm, false);
                    SetFormGeneratorVisibility(SubForms.InvContainerForm, false);

                    SetFormGeneratorVisibility(SubForms.BatchComponentFragmentsForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentFragmentsFormEdit, true);

                    this.DeleteRecordImageMenuButton.Visible = false;
                    this.MoveBatchButton.Visible = this.DeleteBatchButton.Visible = false;
                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DisplayStatusMessage(Resource.EditRecord_Label_Text);

                            this.DefineMixtureImageMenuButton.MenuItemList[0].Visible = true;
                            break;
                        case RegistryTypes.Mixture:

                            this.DisplayStatusMessage(Resource.EditMixture_Label_Text);


                            this.DefineMixtureImageMenuButton.MenuItemList[0].Visible = false;
                            break;
                        case RegistryTypes.Component:
                            this.DisplayStatusMessage(Resource.EditComponent_Label_Text);
                            if (RegUtilities.GetAllowUnregisteredComponents() == true)
                            {
                                this.DefineMixtureImageMenuButton.MenuItemList[0].Visible = true;
                            }
                            else
                            {
                                this.DefineMixtureImageMenuButton.MenuItemList[0].Visible = false;
                            }
                            break;
                    }

                    break;

                case PageState.AddComponent:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Add;

                    SetFormGeneratorVisibility(SubForms.ComponentForm, true);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, true);

                    SetFormGeneratorVisibility(SubForms.MixtureDrawingForm, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.DocManagerForm, false);
                    SetFormGeneratorVisibility(SubForms.InvContainerForm, false);

                    this.DeleteRecordImageMenuButton.Visible = false;
                    this.MoveBatchButton.Visible = this.DeleteBatchButton.Visible = false;
                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DisplayStatusMessage(Resource.EditRecord_Label_Text);
                            break;
                        case RegistryTypes.Mixture:
                            SetPageMode(PageState.ViewComponent, String.Empty);
                            this.DisplayStatusMessage(Resource.EditMixture_Label_Text);
                            break;
                        case RegistryTypes.Component:
                            this.DisplayStatusMessage(Resource.EditComponent_Label_Text);
                            break;
                    }

                    break;

                case PageState.EditRecord:
                    if (IsAuthorizedToEditRecord() && IsAuthorizedToEditRegistry())
                    {
                        _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Edit;
                    }
                    else
                    {
                        forcedMode = PageState.ViewRecord;
                        _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;
                    }

                    SetFormGeneratorVisibility(SubForms.MixtureDrawingForm, true);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);

                    SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.DocManagerForm, false);
                    SetFormGeneratorVisibility(SubForms.InvContainerForm, false);
                    this.DeleteRecordImageMenuButton.Visible = false;
                    this.MoveBatchButton.Visible = this.DeleteBatchButton.Visible = false;

                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DisplayStatusMessage(Resource.EditRecord_Label_Text);
                            break;
                        case RegistryTypes.Mixture:
                            SetFormGeneratorVisibility(SubForms.MixtureDrawingForm, true);
                            this.DisplayStatusMessage(Resource.EditMixture_Label_Text);
                            break;
                        case RegistryTypes.Component:
                            this.DisplayStatusMessage(Resource.EditComponent_Label_Text);
                            break;
                    }
                    break;
                case PageState.AddBatch:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Add;

                    SetFormGeneratorVisibility(SubForms.BatchForm, true);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, true);

                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties,
                            this.RegistryRecord.ComponentList.Count > 1 ? true : false);

                    SetFormGeneratorVisibility(SubForms.MixtureDrawingForm, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.DocManagerForm, false);
                    SetFormGeneratorVisibility(SubForms.InvContainerForm, false);

                    this.DeleteRecordImageMenuButton.Visible = false;
                    this.MoveBatchButton.Visible = this.DeleteBatchButton.Visible = false;
                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DisplayStatusMessage(Resource.EditRecord_Label_Text);
                            break;
                        case RegistryTypes.Mixture:
                            this.DisplayStatusMessage(Resource.EditMixture_Label_Text);
                            break;
                        case RegistryTypes.Component:
                            this.DisplayStatusMessage(Resource.EditComponent_Label_Text);
                            break;
                    }
                    break;
                case PageState.ViewBatch:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;

                    SetFormGeneratorVisibility(SubForms.BatchForm, true);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, true);
                    SetFormGeneratorVisibility(SubForms.InvContainerForm, true);

                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties,
                            this.RegistryRecord.ComponentList.Count > 1 ? true : false);

                    SetFormGeneratorVisibility(SubForms.MixtureDrawingForm, false);
                    SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.DocManagerForm, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);

                    this.DeleteRecordImageMenuButton.Visible = false;
                    //if the registry has only one batch, we cannot delete neither move it.
                    this.MoveBatchButton.Visible = this.DeleteBatchButton.Visible =
                                                this.RegistryRecord.BatchList.Count > 1;

                    if (this.RegistryRecord.BatchList.Count == 1)
                    {
                        this.MoveBatchButton.Visible = true;
                        this.MoveBatchButton.Enabled = false;
                        this.MoveBatchButton.ToolTip = Resource.MoveBatchDisabled_ToolTip;
                    }
                    else
                    {
                        this.MoveBatchButton.Enabled = this.DeleteBatchButton.Enabled = true;
                    }
                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DisplayStatusMessage(Resource.ViewRecord_Label_Text);
                            break;
                        case RegistryTypes.Mixture:
                            this.DisplayStatusMessage(Resource.ViewMixture_Label_Text);
                            break;
                        case RegistryTypes.Component:
                            this.DisplayStatusMessage(Resource.ViewComponent_Label_Text);
                            break;
                    }
                    break;
                case PageState.EditBatch:
                    if (IsAuthorizedToEditBatch(CurrentBatchIndex))
                    {
                        _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Edit;
                    }
                    else
                    {
                        forcedMode = PageState.ViewBatch;
                        _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;
                    }

                    SetFormGeneratorVisibility(SubForms.BatchForm, true);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, true);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, true);

                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties,
                            this.RegistryRecord.ComponentList.Count > 1 ? true : false);

                    SetFormGeneratorVisibility(SubForms.MixtureDrawingForm, false);
                    SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.DocManagerForm, false);
                    SetFormGeneratorVisibility(SubForms.InvContainerForm, false);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, false);

                    this.DeleteRecordImageMenuButton.Visible = false;
                    this.MoveBatchButton.Visible = this.DeleteBatchButton.Visible = false;
                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DisplayStatusMessage(Resource.EditRecord_Label_Text);
                            break;
                        case RegistryTypes.Mixture:
                            this.DisplayStatusMessage(Resource.EditMixture_Label_Text);
                            break;
                        case RegistryTypes.Component:
                            this.DisplayStatusMessage(Resource.EditComponent_Label_Text);
                            break;
                    }
                    break;
                case PageState.End:
                    _mixtureCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;

                    SetFormGeneratorVisibility(SubForms.MixtureDrawingForm, true);
                    SetFormGeneratorVisibility(SubForms.RegistryCustomProperties, true);

                    SetFormGeneratorVisibility(SubForms.ComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.BatchForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchCustomProperties, false);
                    SetFormGeneratorVisibility(SubForms.DocManagerForm, false);
                    SetFormGeneratorVisibility(SubForms.InvContainerForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomProperties, false);
                    this.DeleteRecordImageMenuButton.Visible = false;
                    this.MoveBatchButton.Visible = this.DeleteBatchButton.Visible = false;
                    switch (CurrentRegistryType)
                    {
                        case RegistryTypes.Both:
                            this.DisplayStatusMessage(Resource.ViewRecord_Label_Text);
                            break;
                        case RegistryTypes.Mixture:
                            this.DisplayStatusMessage(Resource.ViewMixture_Label_Text);
                            break;
                        case RegistryTypes.Component:
                            this.DisplayStatusMessage(Resource.ViewComponent_Label_Text);
                            break;
                    }
                    RegUtilities.CleanSession(Page.Session);
                    break;
            } // End Switch


            CurrentPageState = mode;
            this.SetFragmentsFormsVisibility(((RegUtilities.PageState)Enum.Parse(
                typeof(RegUtilities.PageState), forcedMode == PageState.None ? mode.ToString() : forcedMode.ToString())));

            this.DefineMixtureImageMenuButton.Visible = false;
            if (this.IsEditModeEnable)
            {
                this.EditButton.Visible = this.AddBatchButton.Visible = this.GoHomeButton.Visible = false;
                this.CancelButton.Visible = this.SubmitButton.Visible = true;
                this.SendToInventoryImageMenuButton.Visible = false;
                switch (CurrentRegistryType)
                {
                    case RegistryTypes.Both:
                        this.DefineMixtureImageMenuButton.Visible = true;


                        if (CurrentComponentIndex >= 0) // and showing a component
                        {
                            this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true;   //AddComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[1].Enabled = true;  //SearchComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled =
                                 this.RegistryRecord.ComponentList.Count > 1;  //DeleteComponent
                        }
                        else
                        { // and not showing a component
                            if (mode == PageState.EditBatch)
                                this.DefineMixtureImageMenuButton.Visible = false; // Don't show when batch is selected
                            this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true;  //AddComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[1].Enabled = true;  //SearchComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false;  //DeleteComponent
                        }
                        break;
                    case RegistryTypes.Mixture:

                        //in the simplied mixture mode you can only edit registry level information and batches
                        this.DefineMixtureImageMenuButton.Visible = false;
                        this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = false;  //AddComponent
                        this.DefineMixtureImageMenuButton.MenuItemList[1].Enabled = false;  //SearchComponent
                        this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false;  //DeleteComponent

                        break;
                    case RegistryTypes.Component:
                        if (RegUtilities.GetAllowUnregisteredComponents() == true)
                        {
                            if (mode == PageState.EditBatch)
                                this.DefineMixtureImageMenuButton.Visible = false; // Don't show when batch is selected and in edit mode
                            else
                                this.DefineMixtureImageMenuButton.Visible = true;
                            this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = true;   //AddComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[1].Enabled = true;  //SearchComponent
                            this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false;

                        }
                        else
                        {
                            this.DefineMixtureImageMenuButton.Visible = false;
                        }


                        break;
                }


            }
            else
            {
                //This is for the registry record and for 1st batch; Note: 1st batch owner is the owner of the record
                if (IsAuthorizedToEditRegistry() && IsAuthorizedToEditBatch(CurrentBatchIndex) && CurrentBatchIndex == 0)
                {
                    this.EditButton.Visible = true;
                    this.DeleteRecordImageMenuButton.Visible = true;

                }
                else if (IsAuthorizedToEditBatch(CurrentBatchIndex) && CurrentBatchIndex > 0) // Other than 1st batch
                {
                    this.EditButton.Visible = true;
                }
                else
                {
                    this.EditButton.Visible = false;
                    this.DeleteRecordImageMenuButton.Visible = false;
                    this.MoveBatchButton.Visible = this.DeleteBatchButton.Visible = false;//if not editable then cannot delete/move too
                }
                //check for the editability of component section (this is inclusive of structure editability)
                if (mode == PageState.ViewComponent && !IsAuthorizedToEditCompound(this.CurrentComponentIndex) && !IsAuthorizedToEditRegistry())
                {
                    this.EditButton.Visible = false;
                    this.DeleteRecordImageMenuButton.Visible = false;
                }
                this.AddBatchButton.Visible = this.GoHomeButton.Visible = true;
                this.CancelButton.Visible = this.SubmitButton.Visible = false;
                /*CSBR-154565
                  Modified By DIVYA
                  Added to enable or disable SendToInventory link depending upon configuration settings.
                */
                this.SendToInventoryImageMenuButton.Visible = RegUtilities.IsSendToInventoryEnabled();
                //End of CSBR-154565
            }


            this.ShowConfirmationMessage(messageToDisplay);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod,
                                        MethodBase.GetCurrentMethod().Name);
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
                this.Session["PermSearchedInfo"] = SearchedInfo.Create(hitlistid, currentPage, CambridgeSoft.COE.Framework.HitListType.SAVED);
        }

        /// <summary>
        /// Method to display confirmation messages in the top of the page (MessagesAreaUC)
        /// </summary>
        /// <param name="messageToDisplay">The text to display</param>
        private void ShowConfirmationMessage(string messageToDisplay)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (messageToDisplay != String.Empty)
            {
                this.MessagesAreaUserControl.AreaText = messageToDisplay;
                this.MessagesAreaUserControl.Visible = true;
            }
            else
            {
                this.MessagesAreaUserControl.Visible = false;
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }


        //Fix for 142348
        //Home button is available in place of Back button , After creating a registered record.
        //It fixes when we click on "Registration" button directly with Duplicate resolution,
        //It replaces Home button with Back button. 
        //When click on Back button it Redirects to "Submit new component" flow page. 

        private string GetLeaveButtonLabel()
        {
            if (_masterPage.PreviousPage.AbsolutePath.Contains(Resource.SubmitMixture_URL.Substring(Resource.SubmitMixture_URL.LastIndexOf("/"))) ||
                _masterPage.PreviousPage.AbsolutePath.Contains(Resource.ViewMixtureSearch_URL.Substring(Resource.ViewMixtureSearch_URL.LastIndexOf("/"))) || _masterPage.PreviousPage.AbsolutePath.Contains("RegisterMarked.aspx") || _masterPage.PreviousPage.AbsolutePath.Contains(Resource.MoveDelete_PageName) || _masterPage.PreviousPage.AbsolutePath.Contains(Resource.ComponentDuplicatesSearchPage_URL) || _masterPage.PreviousPage.AbsolutePath.Contains("ComponentDuplicates.aspx") || _masterPage.PreviousPage.AbsolutePath.Contains("ReviewRegisterMixture.aspx"))
                //Fix for 142348, It replaces Home button with Back button, After click  AddBatch button in Duplicate resolution
                return Resource.Back_Button_Text;
            else
                return Resource.Home_Button_Text;
        }


        //Fix for 142348
        //Home button is available in place of Back button , After creating a registered record.
        //It fixes when we click on "Registration" button directly with Duplicate resolution,
        //It replaces Home button with Back button.
        //When click on Back button it Redirects to "Submit new component" flow page. 

        private string GetLeaveButtonTooltip()
        {
            if (_masterPage.PreviousPage.AbsolutePath.Contains(Resource.SubmitMixture_URL.Substring(Resource.SubmitMixture_URL.LastIndexOf("/"))))
                return Resource.ReturnToSubmitMixture_Button_Tooltip;
            else if (_masterPage.PreviousPage.AbsolutePath.Contains(Resource.ComponentDuplicatesSearchPage_URL))
                return Resource.ReturnToDuplicatesComponents_Button_Text;
            else if (_masterPage.PreviousPage.AbsolutePath.Contains("ComponentDuplicates.aspx"))
                return Resource.ReturnToNewComponent_Button_ToolTip;
            //Fix for 142348,I It replaces Home button with Back button, After click AddBatch button in Duplicate resolution
            else if (_masterPage.PreviousPage.AbsolutePath.Contains("ReviewRegisterMixture.aspx"))
                return Resource.ReturnToNewComponent_Button_ToolTip;
            else
                return Resource.ReturnToSearchList_Button_Text;

        }

        private void LeavePage()
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

                if (_masterPage.PreviousPage.AbsolutePath.Contains(Resource.SubmitMixture_URL.Substring(Resource.SubmitMixture_URL.LastIndexOf("/"))))
                    Server.Transfer(_masterPage.PreviousPage.PathAndQuery, false);
                else if (_masterPage.PreviousPage.AbsolutePath.Contains(Resource.ViewMixtureSearch_URL.Substring(Resource.ViewMixtureSearch_URL.LastIndexOf("/"))) || _masterPage.PreviousPage.AbsolutePath.Contains(Resource.MoveDelete_PageName))
                    Server.Transfer(Resource.ViewMixtureSearch_URL + "?Caller=VM");
                else if (_masterPage.PreviousPage.AbsolutePath.Contains("RegisterMarked.aspx"))
                    Server.Transfer(_masterPage.PreviousPage.PathAndQuery, false);
                else if (_masterPage.PreviousPage.AbsolutePath.Contains(Resource.ComponentDuplicatesSearchPage_URL))
                    Server.Transfer(Resource.ComponentDuplicatesSearchPage_URL + "?Caller=VM");

                //Fix for 142348
                //Home button is available in place of Back button , After creating a registered record.
                //It fixes when we click on "Registration" button directly with Duplicate resolution,
                //It replaces Home button with Back button. 
                //When click on Back button it Redirects to "Submit new component" flow page. 

                else if (_masterPage.PreviousPage.AbsolutePath.Contains("ComponentDuplicates.aspx"))
                    Server.Transfer(Resource.SubmitMixture_URL + "?Caller=LASTSEARCH");
                //Fix for 142348, It replaces Home button with Back button, After click  AddBatch button in Duplicate resolution
                else if (_masterPage.PreviousPage.AbsolutePath.Contains("ReviewRegisterMixture.aspx"))
                    Server.Transfer(Resource.SubmitMixture_URL + "?Caller=LASTSEARCH");
                else
                    Server.Transfer(Resource.Home_URL);
            }
            finally
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void DisplayRecord()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            this.CurrentComponentIndex = -1;
            CurrentBatchIndex = 0; // reset the current batch index to 0

            if (CurrentPageState == PageState.AddBatch || CurrentPageState == PageState.EditBatch || CurrentPageState == PageState.EditComponent)
                SetPageMode(PageState.EditRecord, string.Empty);
            else if (CurrentPageState == PageState.ViewComponent || CurrentPageState == PageState.ViewBatch)
                SetPageMode(PageState.ViewRecord, string.Empty);
            this.DisplayCompoundIntoTree(RegistryRecord, null);
            _mixtureCOEFormGroup.DataBind();
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void DisplayCompound(int index)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            //RegistryRecord registryRecord = this.GetRegistryFromSession();

            this.CurrentComponentIndex = index;
            CurrentBatchIndex = 0; // reset the current batch index to 0

            if (this.IsEditModeEnable)
            {
                if (RegistryRecord.ComponentList[index].IsNew && !RegistryRecord.ComponentList[index].IsDirty)
                    this.SetPageMode(PageState.AddComponent, string.Empty);
                else
                    this.SetPageMode(PageState.EditComponent, string.Empty);

                if (!IsAuthorizedToEditCompound(index) && !IsAuthorizedToEditRegistry())
                    ((COEFormGenerator)_mixtureCOEFormGroup.FindControl(GetFormIndex(SubForms.ComponentForm))).Enabled = false;
            }
            else
                this.SetPageMode(PageState.ViewComponent, string.Empty);

            this.DisplayCompoundIntoTree(RegistryRecord);
            SetFormGeneratorPageIndex(SubForms.ComponentForm, index);
            SetFormGeneratorPageIndex(SubForms.CompoundCustomProperties, index);
            SetFormGeneratorPageIndex(SubForms.BatchComponentCustomProperties, index);
            SetFormGeneratorPageIndex(SubForms.BatchComponentFragmentsForm, index);
            SetFormGeneratorPageIndex(SubForms.BatchComponentFragmentsFormEdit, index);
            // While editing Componet/Mixture, selected structure toolbar icon value is stored for 'Restore Original structure' link click 
            if (RegistryRecord != null)
                this.structureToolbarOption.Value = ((int)RegistryRecord.ComponentList[index].Compound.BaseFragment.Structure.DrawingType).ToString();

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected override void DisableCOEForm(CambridgeSoft.COE.Framework.COEPageControlSettingsService.Control control)
        {
            if (control.ID == this.GetFormIndex(SubForms.ComponentForm).ToString())
            {
                if (CurrentComponentIndex >= 0)
                {
                    if (!this.IsAuthorizedToEditCompound(CurrentComponentIndex))
                        ((COEFormGenerator)_mixtureCOEFormGroup.FindControl(GetFormIndex(SubForms.ComponentForm))).Enabled = false;
                }
                else
                {
                    ((COEFormGenerator)_mixtureCOEFormGroup.FindControl(GetFormIndex(SubForms.ComponentForm))).Enabled = false;

                    if (RegistryRecord.SameBatchesIdentity)
                        ((COEFormGenerator)_mixtureCOEFormGroup.FindControl(GetFormIndex(SubForms.BatchComponentFragmentsFormEdit))).Enabled = false;
                }
            }
            else if (control.ID == this.GetFormIndex(SubForms.BatchForm).ToString() && CurrentBatchIndex >= 0)
            {
                if (!this.IsAuthorizedToEditBatch(CurrentBatchIndex))
                    ((COEFormGenerator)_mixtureCOEFormGroup.FindControl(GetFormIndex(SubForms.BatchForm))).Enabled = false;
            }
            else
            {
                //this._mixtureCOEFormGroup.GetSubForm(int.Parse(control.ID)).Enabled = false;
                base.DisableCOEForm(control);
            }
        }

        private bool IsAuthorizedToEditBatch(int index)
        {
            //Since RLS editability overrides Supervisor/Supervisee editability
            return ((RegistryRecord.CanEditBatch(index) || RegistryRecord.BatchList[index].IsNew) && RegistryRecord.BatchList[index].IsBatchEditable);
        }

        private bool IsAuthorizedToEditCompound(int index)
        {

            return RegistryRecord.CanEditComponent(index) && IsAuthorizedToEditRecord();
        }

        private bool IsAuthorizedToEditRegistry()
        {

            return RegistryRecord.CanEditRegistry();
        }

        private bool IsAuthorizedToEditRecord()
        {
            return RegistryRecord.IsEditable;
        }

        private void DisplayBatch(int index)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            CurrentBatchIndex = index;
            CurrentComponentIndex = 0;

            if (this.IsEditModeEnable)
            {
                if (RegistryRecord.BatchList[index].IsNew && !RegistryRecord.BatchList[index].IsDirty)
                    this.SetPageMode(PageState.AddBatch, String.Empty);
                else
                    this.SetPageMode(PageState.EditBatch, String.Empty);
            }
            else
                this.SetPageMode(PageState.ViewBatch, string.Empty);

            SetFormGeneratorPageIndex(SubForms.BatchForm, index);
            SetFormGeneratorPageIndex(SubForms.BatchCustomProperties, index);
            SetFormGeneratorPageIndex(SubForms.BatchComponentForm, index);
            SetFormGeneratorPageIndex(SubForms.BatchComponentCustomProperties, index);
            SetFormGeneratorPageIndex(SubForms.BatchComponentFragmentsForm, index);
            SetFormGeneratorPageIndex(SubForms.BatchComponentFragmentsFormEdit, index);
            SetFormGeneratorPageIndex(SubForms.InvContainerForm, index);

            this.DisplayCompoundIntoTree(RegistryRecord);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void SetFormGeneratorPageIndex(SubForms subForm, int pageIndex)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (_mixtureCOEFormGroup != null)
                _mixtureCOEFormGroup.SetFormGeneratorPageIndex(GetFormIndex(subForm), pageIndex);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void SetFormGeneratorVisibility(SubForms subForm, bool visible)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            if (_mixtureCOEFormGroup != null)
                _mixtureCOEFormGroup.SetFormGeneratorVisibility(GetFormIndex(subForm), visible);

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private int GetFormIndex(SubForms subform)
        {
            return (int)subform;
        }

        /// <summary>
        /// Method to display the correct page mode to display according the submitted action
        /// </summary>
        /// <param name="confirmationMessage">The message to display</param>
        private void SetCurrentPageState(string confirmationMessage)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (CurrentPageState == PageState.EditBatch || CurrentPageState == PageState.AddBatch)
            {
                if (AddingBatch)
                    IsEditModeEnable = AddingBatch = false;

                this.SetPageMode(PageState.ViewBatch, confirmationMessage);
                //this.DisplayBatch(this.CurrentBatchIndex);                
            }
            else if (CurrentPageState == PageState.EditComponent)
                this.SetPageMode(PageState.ViewComponent, confirmationMessage);
            else if (CurrentPageState == PageState.EditRecord || CurrentPageState == PageState.AddComponent)
                this.SetPageMode(PageState.ViewRecord, confirmationMessage);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Method to set the correct Confirmation message according the submitted action
        /// </summary>
        /// <param name="regNum">The Registered MixtureID</param>
        /// <param name="batchID">The BatchID added</param>
        private string SetConfirmationMessage(string regNum, int batchID)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string confirmationMessage = String.Empty;
            if (this.CurrentPageState == PageState.AddBatch)
                confirmationMessage = string.Format(Resource.AddedBatch_Label_Text, batchID.ToString("00"));
            else if (this.CurrentPageState == PageState.EditBatch)
                confirmationMessage = string.Format(Resource.EditedBatch_Label_Text, batchID.ToString("00"));
            else if (this.CurrentPageState == PageState.EditRecord || this.CurrentPageState == PageState.EditComponent)
                switch (CurrentRegistryType)
                {
                    case RegistryTypes.Both:
                        confirmationMessage = string.Format(Resource.EditedRecord_Label_Text, regNum);
                        break;
                    case RegistryTypes.Mixture:
                        confirmationMessage = string.Format(Resource.EditedMixture_Label_Text, regNum);
                        break;
                    case RegistryTypes.Component:
                        confirmationMessage = string.Format(Resource.EditedComponent_Label_Text, regNum);
                        break;
                }

            if (AddingBatch)
                confirmationMessage = string.Format(Resource.AddedBatch_Label_Text, batchID.ToString("00"));
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            return confirmationMessage;
        }

        private void SubmitEdition(bool checkOtherMixtures, bool checkDuplicates)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string message = string.Empty;
            RegistryRecord.CheckOtherMixtures = checkOtherMixtures;
            RegistryRecord backupRegistryRecord = this.RegistryRecord.Clone();
            try
            {
                if (this.IsEditModeEnable)
                {
                    RegistryRecord.ApplyEdit();                    

                    RegistryRecord.ModuleName = ChemDrawWarningChecker.ModuleName.REGISTRATION;

                    if (!RegistryRecord.IsValid)
                    {
                        //restore backup
                        DisplayBrokenRules(this.RegistryRecord.GetBrokenRulesDescription());
                        this.RegistryRecord = backupRegistryRecord.Clone();
                        return;
                    }

                    //Start Fix CBOE-1292: 400 batches or more could not be added to a registration record
                    if (RegistryRecord.BatchList.Count > 0)
                    {
                        if (this.AddingBatch || this.CurrentPageState == PageState.EditBatch)
                        {
                            Batch firstBatch = RegistryRecord.BatchList[0];
                            List<Batch> modifiedBatchList = (from bl in RegistryRecord.BatchList
                                                             where bl.IsDirty == true
                                                             select bl).ToList();

                            RegistryRecord.BatchList.Clear();
                            if (modifiedBatchList != null && modifiedBatchList.Count > 0)
                            {
                                foreach (Batch batchItem in modifiedBatchList)
                                {
                                    RegistryRecord.BatchList.Add(batchItem);
                                }
                            }
                            else //keep at least one batch when there is no modification in batch to avoid exception
                                RegistryRecord.BatchList.Add(firstBatch);

                        }
                    }
                    //End Fix CBOE-1292: 400 batches or more could not be added to a registration record

                    if (CurrentRegistryType == RegistryTypes.Mixture)
                    {
                        RegistryRecord.CheckOtherMixtures = false;
                        RegistryRecord = RegistryRecord.Save(DuplicateCheck.None);

                    }
                    else
                    {
                        RegistryRecord = RegistryRecord.Save(checkDuplicates ? DuplicateCheck.CompoundCheck : DuplicateCheck.None);

                    }

                    if (!RegistryRecord.IsValid)
                    {                        
                        DisplayBrokenRules(this.RegistryRecord.GetBrokenRulesDescription());
                        this.RegistryRecord = backupRegistryRecord.Clone();
                        RegistryRecord.ClearRedBoxWarningSetting();
                        return;
                    }
                    CurrentBatchIndex = 0; // reset the current batch index to 0
                    if (!string.IsNullOrEmpty(this.RegistryRecord.FoundDuplicates))
                    {                           
                        this.HandleDuplicates(this.RegistryRecord.FoundDuplicates);
                        this.IsEditModeEnable = false;
                        this.DisplayCompoundIntoTree(RegistryRecord);
                    }
                    else
                    {
                        this.IsEditModeEnable = false;
                        UpdateLocked();
                        SetPageMode(PageState.ViewRecord, string.Empty);
                        DisplayRecord();
                    }
                }

            }
            catch (Exception ex)
            {
                //Start Fix CBOE-1292: 400 batches or more could not be added to a registration record
                if (RegistryRecord.BatchList.Count  != backupRegistryRecord.BatchList.Count)
                {
                    //Reloading batches if RegistryRecord.Save fails
                    this.RegistryRecord = backupRegistryRecord.Clone();
                }
                //End Fix CBOE-1292: 400 batches or more could not be added to a registration record

                EditAffectsOtherMixturesException editAffectsOthersException = null;
                _saveAddbatch = false;
                if (ex is EditAffectsOtherMixturesException)
                    editAffectsOthersException = (EditAffectsOtherMixturesException)ex;
                if (ex.GetBaseException() is EditAffectsOtherMixturesException)
                    editAffectsOthersException = (EditAffectsOtherMixturesException)ex.GetBaseException();

                if (editAffectsOthersException != null)
                {
                    CopiesMessageBox.Visible = true;


                    if (editAffectsOthersException.Message == Resource.UpdateAffectOtherRecords)
                    {
                        string affectedRegistries = ShowAffectsOtherRecordsDialog();
                        if (!String.IsNullOrEmpty(affectedRegistries))
                        {
                            if (_structureChanged)
                            {
                                CopiesMessageBox.Attributes.Add(ACTIONSTRING, CopiesAction.CopyComponentWithDupCheck.ToString());
                            }
                            else
                            {
                                CopiesMessageBox.Attributes.Add(ACTIONSTRING, CopiesAction.CopyComponent.ToString());
                            }

                            if (CopiesMessageBox.FindControl("btn1") != null)
                                _masterPage.MakeCtrlShowProgressModal(CopiesMessageBox.FindControl("btn1").ClientID, "Updating...", string.Empty);
                            if (CopiesMessageBox.FindControl("btn2") != null)
                                _masterPage.MakeCtrlShowProgressModal(CopiesMessageBox.FindControl("btn2").ClientID, "Updating...", string.Empty);
                            string allowContinue = !this.BatchComponentFragmentListIsModified && this.RegistryRecord.CanPropogateComponentEdits() && IsAuthorizedToEditCompound(CurrentComponentIndex) && RegistryRecord.ComponentList[CurrentComponentIndex].Compound.CanPropogateComponentEdits ? Resource.Continue_Button_Text : null;
                            //override allowContinue if user does not have privilege to propogate edits
                            string checkLockedRecords = (RegUtilities.GetLockingEnabled() ? FindRegNumbersLocked(editAffectsOthersException) : "");
                            if (!string.IsNullOrEmpty(checkLockedRecords))
                            {
                                ((Button)CopiesMessageBox.FindControl("btn1")).Enabled = false;
                                ((Button)CopiesMessageBox.FindControl("btn1")).ToolTip = Resource.Locked_Registration + checkLockedRecords;
                            }
                            CopiesMessageBox.Show(Resource.EditingComponentsEffectsCaution_MessagesBox,
                                                   Resource.ReccordsAffected_MessageBox_Title,
                                                   allowContinue,
                                                   Resource.CreateCopies_Button_Text,
                                                   Resource.Cancel_Button_Text);
                        }
                        else
                        {
                            CopiesMessageBox.Visible = false;
                            SubmitEdition(false, false);
                        }
                    }
                    else
                    {
                        string affectedRegistries = ShowAffectsOtherRecordsDialog();
                        if (!String.IsNullOrEmpty(affectedRegistries))
                        {
                            bool allowCreateCopy = false;
                            bool.TryParse(RegUtilities.GetConfigSetting("REGADMIN", "AllowCreateCopySharedStructure"), out allowCreateCopy);
                            string allowCreateCopyButton = allowCreateCopy ? Resource.CreateCopies_Button_Text : null;
                            string allowContinue = this.RegistryRecord.CanPropogateStructureEdits() && RegistryRecord.ComponentList[CurrentComponentIndex].Compound.BaseFragment.Structure.CanPropogateStructureEdits ? Resource.Continue_Button_Text : null;
                            if (_structureChanged)
                            {
                                CopiesMessageBox.Attributes.Add(ACTIONSTRING, CopiesAction.CopyStructureWithDupCheck.ToString());
                            }
                            else
                            {
                                CopiesMessageBox.Attributes.Add(ACTIONSTRING, CopiesAction.CopyStructure.ToString());
                            }
                            string checkLockedRecords = (RegUtilities.GetLockingEnabled() ? FindRegNumbersLocked(editAffectsOthersException) : "");
                            if (!string.IsNullOrEmpty(checkLockedRecords))
                            {
                                ((Button)CopiesMessageBox.FindControl("btn1")).Enabled = false;
                                ((Button)CopiesMessageBox.FindControl("btn1")).ToolTip = Resource.Locked_Registration + checkLockedRecords;
                            }
                            CopiesMessageBox.Show(Resource.EditingStructureEffectsCaution_MessagesBox,
                                                   Resource.ComponentsAffected_MessageBox_Title,
                                                   allowContinue,
                                                   allowCreateCopyButton,
                                                   Resource.Cancel_Button_Text);
                        }
                        else
                        {
                            CopiesMessageBox.Visible = false;
                            SubmitEdition(false, false);
                        }
                    }
                }


                else
                    _masterPage.DisplayErrorMessage(ex.Message, false);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private string FindRegNumbersLocked(EditAffectsOtherMixturesException editAffextExp)
        {
            string regNumbers = string.Empty;
            string listofRegnumbers = string.Empty;
            foreach (string[] valueSet in editAffextExp.SharedComponentConflicts.Values)
            {
                foreach (string sRegnum in valueSet)
                {
                    listofRegnumbers = (string.IsNullOrEmpty(listofRegnumbers) ? "'" + sRegnum + "'" : listofRegnumbers + ",'" + sRegnum + "'");
                }
            }
            regNumbers = RegistryRecord.GetLockedRegistryRecords(listofRegnumbers);
            return regNumbers;
        }

        private void HandleDuplicates(string duplicates)
        {
            switch (RegUtilities.HandleDuplicates(RegistryRecord, duplicates, false))
            {
                case RegUtilities.DuplicateType.Compound:
                    DuplicatesMessageBox.Attributes.Remove("DuplicateMixture");
                    DuplicatesMessageBox.Attributes.Add("DuplicateMixture", bool.FalseString);
                    DuplicatesMessageBox.Visible = true;
                    DuplicatesMessageBox.Show(Resource.CheckDuplicates_Label_Text,
                                              Resource.DuplicateBeingCreated_Msg_Title,
                                              Resource.OK_Button_Text,
                                              Resource.Cancel_Button_Text,
                                              null);

                    this.SetCurrentPageState(string.Empty);
                    break;
                case RegUtilities.DuplicateType.Mixture:
                    DuplicatesMessageBox.Attributes.Remove("DuplicateRecords");
                    DuplicatesMessageBox.Attributes.Add("DuplicateRecords", bool.TrueString);
                    DuplicatesMessageBox.Visible = true;
                    DuplicatesMessageBox.Show(Resource.MixtureDuplicatesExistingDirect_Msg_Text,
                                              Resource.MixtureDuplicatesExisting_Msg_Title,
                                              Resource.OK_Button_Text,
                                              Resource.Cancel_Button_Text,
                                              null);
                    this.SetCurrentPageState(string.Empty);
                    break;
                case RegUtilities.DuplicateType.None:
                    this.SetCurrentPageState(this.SetConfirmationMessage(RegistryRecord.RegNumber.RegNum, RegistryRecord.BatchList[this.CurrentBatchIndex].BatchNumber));
                    break;
                case RegUtilities.DuplicateType.DuplicatedOrCopied:
                    RegistryRecord.UpdateFragments();
                    RegistryRecord.UpdateXml();
                    RegistryRecord = RegistryRecord.Save(DuplicateCheck.None);
                    this.IsEditModeEnable = false;
                    this.SetCurrentPageState(this.SetConfirmationMessage(RegistryRecord.RegNumber.RegNum, RegistryRecord.BatchList[this.CurrentBatchIndex].BatchNumber));
                    break;

            }
        }


        private void HandleMixtureDuplicates()
        {


            DuplicatesMessageBox.Attributes.Remove("DuplicateRecords");
            DuplicatesMessageBox.Attributes.Add("DuplicateRecords", bool.TrueString);
            DuplicatesMessageBox.Visible = true;
            DuplicatesMessageBox.Show(Resource.MixtureDuplicatesExistingDirect_Msg_Text,
                                      Resource.MixtureDuplicatesExisting_Msg_Title,
                                      Resource.OK_Button_Text,
                                      Resource.Cancel_Button_Text,
                                      null);
        }

        private void DeleteComponent()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            //RegistryRecord registry = this.GetRegistryFromSession();
            if (RegistryRecord.ComponentList.Count > 1)
            {
                EditMixture();
                int componentIndex = _mixtureCOEFormGroup.GetFormGeneratorPageIndex(GetFormIndex(SubForms.ComponentForm));

                Component deletedComponent = RegistryRecord.ComponentList[componentIndex];

                RegistryRecord.DeleteComponent(componentIndex);

                _mixtureCOEFormGroup.DataBind();

                //Original (and theoretically correct) code
                this.DisplayCompound(Math.Min(componentIndex, RegistryRecord.ComponentList.Count - 1));

                this.DisplayCompoundIntoTree(RegistryRecord, ((decimal)Math.Min(componentIndex, RegistryRecord.ComponentList.Count - 1)).ToString());
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void SearchCompound()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            _mixtureCOEFormGroup.Update();

            if (AllowUnregisteredComponents)
            {
                this.RegistryRecord.AddComponent(Component.NewComponent());
                DisplayCompound(this.RegistryRecord.ComponentList.Count - 1);
            }
            // Pick an existing Component will not be replaced by selected component in treeview
            Session["currentCompoundIndex"] = _mixtureCOEFormGroup.GetFormGeneratorPageIndex(GetFormIndex(SubForms.ComponentForm));
            Server.Transfer(Resource.SearchCompound_Page_URL + "?caller=VM", false);

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
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
            DisplayCompound(RegistryRecord.ComponentList.Count - 1);
            this.DisplayCompoundIntoTree(RegistryRecord, NodeKeywords.LastComponent.ToString());
            this.SetPageMode(PageState.AddComponent, string.Empty);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void AddBatch()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            AddingBatch = true;
            this.IsEditModeEnable = true;
            RegistryRecord.BeginEdit();
            RegistryRecord.AddBatch();
            this._mixtureCOEFormGroup.DataBind();
            this.CurrentBatchIndex = RegistryRecord.BatchList.Count - 1;
            this.DisplayBatch(RegistryRecord.BatchList.Count - 1);
            this.DisplayCompoundIntoTree(RegistryRecord, NodeKeywords.LastBatch.ToString());
            EnableDisableMixtureTree(false);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        //Fix for CSBR-149346 
        //In this bug the User Navigates to Search temp form after updates a Registered record insted Search Parmanent page.
        //Apart from this Bug, It rectified all flows from starting page to starting page in  Registration. 
        private void CancelRecordEdit()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            AddingBatch = false;
            string regNum = RegistryRecord.RegNumber.RegNum;
            RegistryRecord = RegistryRecord.GetRegistryRecord(regNum);
            _mixtureCOEFormGroup.DataBind();
            this.DisplayCompoundIntoTree(RegistryRecord);

            this.IsEditModeEnable = false;
            CurrentBatchIndex = 0; // reset the current batch index to 0
            if (CurrentPageState == PageState.EditComponent)
            {
                this.SetPageMode(PageState.ViewComponent, string.Empty);
                this.DisplayCompound(CurrentComponentIndex);
            }
            else if (CurrentPageState == PageState.EditBatch)
                this.SetPageMode(PageState.ViewBatch, string.Empty);
            else
            {
                this.SetPageMode(PageState.ViewRecord, string.Empty);
                this.DisplayCompoundIntoTree(RegistryRecord, null);
            }
            Session["EditRegistryStructureModeEnable"] = null;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        //Fix for CSBR-149346 
        //In this bug the User Navigates to Search temp form after updates a Registered record insted Search Parmanent page.
        //Apart from this Bug, It rectified all flows from starting page to starting page in  Registration. 
        private void EditMixture()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (!this.IsEditModeEnable)
            {
                //make a copy of the regitry record so a determination
                OriginalRegistryRecord = RegistryRecord.Clone();
                RegistryRecord.BeginEdit();

                this.IsEditModeEnable = true;

                if (CurrentPageState == PageState.ViewBatch)
                {
                    this.DisplayBatch(CurrentBatchIndex);
                }
                else if (CurrentPageState == PageState.ViewComponent)
                {
                    this.DisplayCompound(CurrentComponentIndex);
                }
                else
                {
                    this.SetPageMode(PageState.EditRecord, string.Empty);
                    this.IsEditStructureModeEnable = true;
                }
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Transfer to the page where the move/delete happens. 
        /// </summary>
        /// <param name="action">Action to perform</param>
        private void MoveDeleteBatch(bool action)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            if (action)
                Server.Transfer("MoveDelete.aspx?Action=MOVE&BatchID=" + this.RegistryRecord.BatchList[this.CurrentBatchIndex].ID +
                    "&RegID=" + this.RegistryRecord.RegNumber.RegNum, false);
            else
                Server.Transfer("MoveDelete.aspx?Action=DELETE&BatchID=" + this.RegistryRecord.BatchList[this.CurrentBatchIndex].ID +
                    "&RegID=" + this.RegistryRecord.RegNumber.RegNum, false);

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void ReplaceExistingComponent(string compoundIdToAdd)
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod,
                                           MethodBase.GetCurrentMethod().Name);
                if (Request[PerkinElmer.COE.Registration.Server.Constants.RegisteredCompoundId_UrlParameter] != null)
                {
                    int index = Session["currentCompoundIndex"] != null ? (int)Session["currentCompoundIndex"] : 0;
                    Session["currentCompoundIndex"] = null;
                    Compound compound = null;

                    if (!string.IsNullOrEmpty(compoundIdToAdd))
                        compound = this.GetCompound(compoundIdToAdd);

                    this.RegistryRecord.ReplaceCompound(index, compound);

                    DisplayCompound(index);
                }
                else if (Session["currentCompoundIndex"] != null)
                {
                    int index = Session["currentCompoundIndex"] != null ? (int)Session["currentCompoundIndex"] : 0;
                    Session["currentCompoundIndex"] = null;

                    DisplayCompound(index);
                }
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

        private void DisableEditDeleteButtons()
        {
            this.DeleteBatchButton.Visible = this.EditButton.Visible = this.MoveBatchButton.Visible = this.ComponentToolBar.Visible = false;
            this.ShowConfirmationMessage(Resource.IsNotEditable_Message);
        }

        private void DisableRegistryDeleteLink()
        {
            this.DeleteRecordImageMenuButton.Visible = false;
            this.ShowConfirmationMessage(Resource.IsNotRegistryDeleteable_Message);
        }

        private void UpdateInvSettings()
        {   //don't do this is inventory is not enable

            if (RegUtilities.GetInventoryIntegration())
            {
                if (!(Convert.ToString(HttpContext.Current.Application[PerkinElmer.COE.Registration.Server.Constants.InvHasRegIdInGroupingField]).ToLower() == "true" &&
                    Convert.ToString(HttpContext.Current.Application[PerkinElmer.COE.Registration.Server.Constants.InvHasRegBatchIdInGroupingField]).ToLower() == "true"))
                {
                    COESearch simpleSearch = new COESearch(RegUtilities.GetInvGroupingFieldsDataViewID());
                    ResultPageInfo rpi = new ResultPageInfo(0, 100, 1, 101);
                    DataResult dr = simpleSearch.GetDataPage(rpi, new string[] { "GROUPINGFIELDS.FIELDNAME" });
                    if (dr != null && dr.ResultSet != null)
                    {
                        HttpContext.Current.Application[PerkinElmer.COE.Registration.Server.Constants.InvHasRegIdInGroupingField] = dr.ResultSet.Contains("REG_ID_FK").ToString();
                        HttpContext.Current.Application[PerkinElmer.COE.Registration.Server.Constants.InvHasRegBatchIdInGroupingField] = dr.ResultSet.Contains("BATCH_NUMBER_FK").ToString();
                        if (dr.ResultSet.Contains("REG_ID_FK") || dr.ResultSet.Contains("BATCH_NUMBER_FK"))
                            RegUtilities.SetCOEPageSettings(true);
                    }
                }
            }
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
            cs.RegisterStartupScript(typeof(ViewMixture), "EnableDisaable", sStartupRunScripts.ToString(), true);

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

        private void EnableDisableMixtureTree(bool status)
        {
            Control treeControl = _masterPage.AccordionControl.FindControl("UltraWebTreeControl");
            if (treeControl != null && treeControl is UltraWebTree)
            {
                ((UltraWebTree)treeControl).Enabled = status;
            }

        }

        public void SetFormToReadOnly()
        {
            if (RegUtilities.GetLockingEnabled())
            {
                if (RegistryRecord.Status == RegistryStatus.Locked)
                {
                    this.DefineMixtureImageMenuButton.Enabled = false;
                    this.MoveBatchButton.Enabled = false;
                    this.DeleteBatchButton.Enabled = false;
                    this.DefineMixtureImageMenuButton.MenuItemList[0].Enabled = false;  //AddComponent
                    this.DefineMixtureImageMenuButton.MenuItemList[1].Enabled = false;  //SearchComponent
                    this.DefineMixtureImageMenuButton.MenuItemList[2].Enabled = false;
                    this.DeleteRecordImageMenuButton.DisableControl();
                    if ((!AddingBatch) && (this.CurrentPageState == PageState.EditRecord || this.CurrentPageState == PageState.EditBatch || this.CurrentPageState == PageState.EditComponent))
                        SetControlsToReadOnly(this.mixtureInformationHolder);
                }
                else if (this.CurrentPageState == PageState.EditComponent && GetCurrentComopnentLocked())
                {
                    SetControlsToReadOnly(this.mixtureInformationHolder);
                }

                else
                {
                    this.MoveBatchButton.Enabled = this.RegistryRecord.BatchList.Count > 1;
                    this.DeleteBatchButton.Enabled = true;
                    this.DefineMixtureImageMenuButton.Enabled = true;
                }
            }
        }

        public void SetControlsToReadOnly(Control control)
        {

            if (control is ICOELabelable && ((ICOELabelable)control).Label.ToLower().Contains("identifiers"))
            {
                ((COEWebGridUltra)control).SetCellsNonEditable();
                Control deletButton = control.FindControl("DeleteLinkButton");//Disabling the delete button for locked record edit mode
                if (deletButton != null)
                {
                    ((LinkButton)deletButton).Enabled = false;
                    ((HtmlTableCell)deletButton.Parent).Disabled = true;

                }
                return;
            }
            else if (control is COEWebGridUltra && control is ICOELabelable && ((ICOELabelable)control).Label.ToLower().Contains("projects"))
            {
                ((COEWebGridUltra)control).SetCellsNonEditable();
                Control deletButton = control.FindControl("DeleteLinkButton");//Disabling the delete button for locked record edit mode
                if (deletButton != null)
                {
                    ((LinkButton)deletButton).Enabled = false;
                    ((HtmlTableCell)deletButton.Parent).Disabled = true;
                }
                return;
            }
            else if (control is ICOEReadOnly)
            {
                ((ICOEReadOnly)control).COEReadOnly = COEEditControl.ReadOnly;
                return;
            }
            foreach (Control subcontrol in control.Controls)
                if (subcontrol is WebControl)
                    SetControlsToReadOnly((WebControl)subcontrol);
        }

        public bool GetCurrentComopnentLocked()
        {
            bool compoundLockstatus = false;
            if (!this.RegistryRecord.IsSingleCompound)
            {
                if (RegistryRecord.ComponentList[CurrentComponentIndex] != null && RegistryRecord.ComponentList[CurrentComponentIndex].Compound != null)
                {
                    if (RegistryRecord.GetCompoundLockedStatus(RegistryRecord.ComponentList[CurrentComponentIndex].Compound.ID) == (int)RegistryStatus.Locked)
                        compoundLockstatus = true;
                }
            }
            return compoundLockstatus;
        }
        #endregion

        #region CSLA Datasources Events

        protected void MixtureCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            try
            {
                e.BusinessObject = RegistryRecord;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void DocMgrDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (RegUtilities.GetDocMgrEnabled())
                {
                    if (Session["DocManagerDocs"] == null)
                    {
                        DataSet retVal = this.GetDocMgrDocs(this.RegistryRecord != null ? this.RegistryRecord.RegNumber.RegNum : string.Empty);
                        if (retVal != null)
                        {
                            List<DocumentList> dsDocList = new List<DocumentList>();
                            if (retVal.Tables.Count > 0)
                            {
                                //Below some workaround because some formgenerator limitations (regarding pageIndex and detailsview)
                                dsDocList.Add(DocumentList.NewDocumentList(retVal.Tables[0], this.RegistryRecord.RegNumber.RegNum));
                            }
                            else
                                dsDocList.Add(DocumentList.NewDocumentList(this.RegistryRecord.RegNumber.RegNum));

                            Session["DocManagerDocs"] = dsDocList;
                        }
                    }
                    e.BusinessObject = (List<DocumentList>)Session["DocManagerDocs"];
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private string ShowAffectsOtherRecordsDialog()
        {
            //here we will look through the components that match  - ignoring newly added components
            //we need to see if any of the propertylists are dirty.  If they are not, but the structure
            //is dirty, we need to assure that the edit to the structure changed the atom count
            //only in that instance will we alert the user that they could affect other registries
            string affectedRegistries = string.Empty;
            foreach (Component component in this.RegistryRecord.ComponentList)
            {
                foreach (Component componentOriginal in OriginalRegistryRecord.ComponentList)
                {
                    if (componentOriginal.Compound.RegNumber.RegNum == component.Compound.RegNumber.RegNum)
                    {
                        if (component.IsDirty && (!component.Compound.FragmentList.IsDirty && !component.Compound.IdentifierList.IsDirty && !component.Compound.PropertyList.IsDirty
                            && !component.Compound.BaseFragment.Structure.IdentifierList.IsDirty && !component.Compound.BaseFragment.Structure.PropertyList.IsDirty))
                        {
                            //we need to know if the structure has changed to know whether or not to duplicate check when the editted record is submitted
                            if (!RegUtilities.CheckIfMWAndFormulaMatch(component.Compound.BaseFragment.Structure.Value.ToString(), componentOriginal.Compound.BaseFragment.Structure.Value.ToString()))
                            {
                                _structureChanged = true;
                                if (string.IsNullOrEmpty(affectedRegistries))
                                {
                                    affectedRegistries = component.Compound.RegNumber.RegNum;
                                }
                                else
                                {
                                    affectedRegistries = affectedRegistries + ", " + component.Compound.RegNumber.RegNum;
                                }
                            }
                        }

                        if (component.IsDirty && (component.Compound.FragmentList.IsDirty || component.Compound.IdentifierList.IsDirty || component.Compound.PropertyList.IsDirty
                             || component.Compound.BaseFragment.Structure.IdentifierList.IsDirty || component.Compound.BaseFragment.Structure.PropertyList.IsDirty))
                        {
                            //we need to know if the structure has changed to know whether or not to duplicate check when the editted record is submitted
                            if (component.IsDirty)
                            {
                                if (!RegUtilities.CheckIfMWAndFormulaMatch(component.Compound.BaseFragment.Structure.Value.ToString(), componentOriginal.Compound.BaseFragment.Structure.Value.ToString()))
                                {
                                    _structureChanged = true;
                                }
                            }
                            if (string.IsNullOrEmpty(affectedRegistries))
                            {
                                affectedRegistries = component.Compound.RegNumber.RegNum;
                            }
                            else
                            {
                                affectedRegistries = affectedRegistries + ", " + component.Compound.RegNumber.RegNum;
                            }

                        }
                        //Check when changes made to equivalents in fragments.
                        //Note : when SBI = TRUE.(Mandatory).
                        if (string.IsNullOrEmpty(affectedRegistries) && OriginalRegistryRecord.SameBatchesIdentity)
                        {
                            foreach (Batch currentBatch in RegistryRecord.BatchList)
                            {
                                if (currentBatch.IsDirty)
                                {
                                    foreach (BatchComponent batchComponent in currentBatch.BatchComponentList)
                                    {
                                        BatchComponentFragmentList fl = batchComponent.BatchComponentFragmentList;
                                        if (fl.IsDirty)
                                        {
                                            if (batchComponent.ComponentIndex == componentOriginal.ComponentIndex)
                                            {
                                                if (string.IsNullOrEmpty(affectedRegistries))
                                                    affectedRegistries = componentOriginal.Compound.RegNumber.RegNum;
                                                else
                                                    affectedRegistries = affectedRegistries + ", " + componentOriginal.Compound.RegNumber.RegNum;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return affectedRegistries;

        }
        /// <summary>
        /// Search and get all the documents associated with the given RegNum
        /// </summary>
        /// <param name="regNum">RegNum to find docs</param>
        /// <returns>Found list of docs</returns>
        private DataSet GetDocMgrDocs(string regNum)
        {
            COESearch search = new COESearch(RegUtilities.GetDocMgrDataViewID());
            SearchInput searchInput = new SearchInput();
            ResultPageInfo resultPageInfo = new ResultPageInfo(0, 100000, 1, 100001);
            searchInput.ReturnPartialResults = false;
            string searchCriteria = RegUtilities.GetDocMgrSearchField() + " equals " + regNum;
            string[] searchFields = new string[] { searchCriteria };
            searchInput.FieldCriteria = searchFields;
            DataResult dataResult = search.DoSearch(searchInput, RegUtilities.GetDocMgrFields(), resultPageInfo);
            DataSet ds = new DataSet("Documents");
            if (!dataResult.Status.Contains("FAILURE"))
                this.ConvertToDataTable(dataResult, ref ds);
            return ds;
        }

        /// <summary>
        /// Gets the containers associated to the record or the batch.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InvContainersDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (RegUtilities.GetInventoryIntegration())
            {
                int containersCount = 0;
                try
                {
                    if (this.RegistryRecord != null)
                    {
                        DataSet retVal = this.GetInvContainers(this.RegistryRecord.RegNumber.ID, this.CurrentPageState == PageState.ViewBatch ? this.RegistryRecord.BatchList[CurrentBatchIndex].BatchNumber : -1);
                        if (retVal != null)
                        {
                            List<InvContainerList> invContainerList = new List<InvContainerList>();
                            if (retVal.Tables.Count > 0)
                            {
                                //Below some workaround because some formgenerator limitations (regarding pageIndex and detailsview)
                                invContainerList.Add(InvContainerList.NewInvContainerList(retVal.Tables[0], this.RegistryRecord.RegNumber.RegNum));
                            }
                            else
                                invContainerList.Add(InvContainerList.NewInvContainerList(this.RegistryRecord.RegNumber.RegNum));

                            e.BusinessObject = invContainerList;
                            containersCount = invContainerList[0].Count;
                        }
                    }
                }
                catch (Exception exception)
                {
                    COEExceptionDispatcher.HandleUIException(exception);
                }
                finally
                {
                    if (containersCount == 0)
                        this.SetFormGeneratorVisibility(SubForms.InvContainerForm, false);
                }
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
        }


        /// <summary>
        /// Seasrch and get all the documents associated with the given record/batch
        /// </summary>
        /// <param name="regid">Registry id</param>
        /// <param name="batchid">Current batch id</param>
        /// <returns>Found list of docs</returns>
        private DataSet GetInvContainers(int regid, int batchid)
        {
            COESearch search = new COESearch();
            ResultsCriteria rc = RegUtilities.GetInvContainersRC(regid, batchid);
            SearchCriteria sc = RegUtilities.GetInvContainersSC(regid, batchid);
            PagingInfo pi = new PagingInfo();
            pi.Start = 1;
            pi.End = 100001;
            pi.RecordCount = 100000;

            SearchResponse result = search.DoSearch(sc, rc, pi, RegUtilities.GetInvContainersDataViewID());
            return result.ResultsDataSet;
        }

        /// <summary>
        /// Coverts from DataResult to Dataset (easier binding)
        /// </summary>
        /// <param name="result"></param>
        /// <param name="ds"></param>
        private void ConvertToDataTable(DataResult result, ref DataSet ds)
        {
            using (System.IO.StringReader stringReader = new System.IO.StringReader(result.ResultSet))
                ds.ReadXml(stringReader);
        }

        protected void MixtureCslaDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            try
            {
                if (this.IsEditModeEnable)
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
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
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
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void BatchListCslaDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                string listName = RegUtilities.FindKey("ProjectList", e.Values.Keys);
                Batch currentBatch = RegistryRecord.BatchList[Math.Min(RegistryRecord.BatchList.Count - 1, this._mixtureCOEFormGroup.GetSubForm(this.GetFormIndex(SubForms.BatchForm)).PageIndex)];
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
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void BatchComponentListCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = this.RegistryRecord.BatchList[Math.Min(RegistryRecord.BatchList.Count - 1, this._mixtureCOEFormGroup.GetSubForm(this.GetFormIndex(SubForms.BatchForm)).PageIndex)].BatchComponentList;
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
                BatchComponent currentBatchComponent = this.RegistryRecord.BatchList[Math.Min(RegistryRecord.BatchList.Count - 1, this._mixtureCOEFormGroup.GetSubForm(this.GetFormIndex(SubForms.BatchForm)).PageIndex)].BatchComponentList[(int)e.Keys["PageIndex"]];
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
                if (this.IsEditModeEnable)
                {
                    Component currentComponent = RegistryRecord.ComponentList[(int)e.Keys["PageIndex"]];
                    string listName = RegUtilities.FindKey("Compound.IdentifierList", e.Values.Keys);
                    if (e.Values[listName] is DataTable)
                        RegUtilities.SincronizeListDataTable((DataTable)e.Values[listName], "IdentifierID", currentComponent.Compound.IdentifierList, Identifier.NewIdentifier);

                    listName = RegUtilities.FindKey("Compound.BaseFragment.Structure.IdentifierList", e.Values.Keys);
                    if (e.Values[listName] is DataTable)
                        RegUtilities.SincronizeListDataTable((DataTable)e.Values[listName], "IdentifierID", currentComponent.Compound.BaseFragment.Structure.IdentifierList, Identifier.NewIdentifier);

                    COEDataMapper.Map(e.Values, currentComponent);
                    e.RowsAffected = 1;
                }
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
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = PrefixNameValueList.GetPrefixNameValueList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

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
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = ChemistNameValueList.GetChemistNameValueList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
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
            try
            {
                e.BusinessObject = FragmentList.GetFragmentList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void FragmentsCslaDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            int batchIndex = Math.Min(RegistryRecord.BatchList.Count - 1, this._mixtureCOEFormGroup.GetSubForm(this.GetFormIndex(SubForms.BatchForm)).PageIndex);
            int batchComponentIndex = (int)e.Keys["PageIndex"];
            try
            {
                string key = RegUtilities.FindKey("BatchComponentFragmentList", e.Values.Keys);
                if (e.Values[key] is DataTable)
                    RegUtilities.SincronizeListDataTable((DataTable)e.Values[key], "FragmentID", RegistryRecord.BatchList[batchIndex].BatchComponentList[batchComponentIndex].BatchComponentFragmentList, BatchComponentFragment.NewBatchComponentFragment);
                COEDataMapper.Map(e.Values, RegistryRecord.BatchList[batchIndex].BatchComponentList[batchComponentIndex]);
                if (this.RegistryRecord.SameBatchesIdentity)
                {
                    for (int iBatch = 1; iBatch < RegistryRecord.BatchList.Count; iBatch++)
                    {
                        RegUtilities.SincronizeListDataTable((DataTable)e.Values[key], "FragmentID", RegistryRecord.BatchList[iBatch].BatchComponentList[batchComponentIndex].BatchComponentFragmentList, BatchComponentFragment.NewBatchComponentFragment);
                        COEDataMapper.Map(e.Values, RegistryRecord.BatchList[iBatch].BatchComponentList[batchComponentIndex]);
                    }
                }
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
                int batchIndex = Math.Min(RegistryRecord.BatchList.Count - 1, this._mixtureCOEFormGroup.GetSubForm(this.GetFormIndex(SubForms.BatchForm)).PageIndex);
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
                int batchIndex = Math.Min(RegistryRecord.BatchList.Count - 1, this._mixtureCOEFormGroup.GetSubForm(this.GetFormIndex(SubForms.BatchForm)).PageIndex);
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

        #region Enums

        private enum CopiesAction
        {
            None,
            CopyComponent,
            CopyStructure,
            CopyComponentWithDupCheck,
            CopyStructureWithDupCheck
        }

        #endregion

    }
}
