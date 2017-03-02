using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using CambridgeSoft.COE.Framework.Common.Messaging;
using System.Xml;
using Resources;
using Infragistics.WebUI.UltraWebNavigator;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using CS = CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEFormService;
using PerkinElmer.COE.Registration.Server.Forms.ComponentDuplicates;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using PerkinElmer.COE.Registration.Server.Code;
using CambridgeSoft.COE.Registration;
using System.Text;
using CambridgeSoft.COE.Framework.Common.Validation;
using System.Collections.Generic;
using PerkinElmer.COE.Registration.Server.Controls;

namespace PerkinElmer.COE.Registration.Server.Forms.RegistryDuplicates
{
    public partial class RegistryDuplicates : GUIShellPage
    {
        #region GUIShell Variables

        RegistrationMaster _masterPage = null;
        private string _controlToListen = String.Empty;
        private string _groupToListen = String.Empty;
        private bool _showSaltBatchSuffixDialog = false;
        private Panel pnlSaltandBatchNumPanel = new Panel();
        /// <summary>
        /// These are the controls which I can be interested in the Control(MyCustomizedControl)
        /// inside the Accordion Group.
        /// </summary>
        private enum ControlsToTakeCare
        {
            MessageLabel,
            DoneButton,
            ShowButtonsHidden,
            UltraWebTreeControl,
        }

        /// <summary>
        /// Those are the events from the control inside the accordion that I'm interested.
        /// </summary>
        private enum EventsToListen
        {
            NodeClicked,
            Done,
        }
        public bool IsPreRegistration
        {
            get
            {

                if (Request.QueryString["DupCheckOnSubmit"] != null)
                {
                    return System.Convert.ToBoolean(Request.QueryString["DupCheckOnSubmit"].ToString());
                }

                else
                {
                    return false;
                }
            }

        }

        public bool AllowUnregisteredComponents
        {
            get
            {
                return RegUtilities.GetAllowUnregisteredComponents();
            }

        }

        public bool BatchPrefixesEnabled
        {
            get
            {
                return RegUtilities.GetBatchPrefixesEnabled();
            }

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
        int PageFormGroupID
        {
            get
            {
                return RegUtilities.GetCOEFormID("RegistryDuplicatesFormGroupId");
            }
        }

        public RegistryRecord RegistryRecord
        {
            get
            {
                return (RegistryRecord)Session[PerkinElmer.COE.Registration.Server.Constants.MultiCompoundObject_Session];
            }
        }

        bool IsAMixture
        {
            get
            {
                return true;
            }
        }

        public DuplicatesList Duplicates
        {
            get
            {
                if (Session[Constants.DuplicateMultiCompounds_Session] == null)
                {
                    string duplicatesIdList = Session[PerkinElmer.COE.Registration.Server.Constants.DuplicateIdsList_Session].ToString();
                    if (!string.IsNullOrEmpty(duplicatesIdList))
                    {
                        Session[PerkinElmer.COE.Registration.Server.Constants.DuplicateMultiCompounds_Session] = new DuplicatesList(duplicatesIdList, RegistryRecord.ComponentList.Count > 1);
                        Session.Remove(PerkinElmer.COE.Registration.Server.Constants.DuplicateIdsList_Session);
                    }
                }
                return (DuplicatesList)Session[PerkinElmer.COE.Registration.Server.Constants.DuplicateMultiCompounds_Session];
            }
        }

        private enum SubForms
        {
            MixtureForm,
            MixtureCustomForm,
            CompoundForm,
            CompoundCustomForm,
            BatchForm,
            BatchCustomForm,
            BatchComponentForm,
            BatchComponentCustomForm
        }

        private enum PageState
        {
            None,
            ViewRecord,
            ViewComponent,
            ViewBatch,
            End,
        }

        AllowedActions Actions
        {
            get
            {
                if (Request.Params[PerkinElmer.COE.Registration.Server.Constants.AllowedActionsParameter] != null && Session[PerkinElmer.COE.Registration.Server.Constants.AllowedActionsParameter] == null)
                {
                    return (AllowedActions)Enum.Parse(typeof(AllowedActions), Request.Params[PerkinElmer.COE.Registration.Server.Constants.AllowedActionsParameter]);
                }
                return Session[Constants.AllowedActionsParameter] != null ? (AllowedActions)Session[PerkinElmer.COE.Registration.Server.Constants.AllowedActionsParameter] : AllowedActions.All;
            }
            set
            {
                Session[PerkinElmer.COE.Registration.Server.Constants.AllowedActionsParameter] = value;
            }
        }

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

        public int CurrentBatchIndex
        {
            get
            {
                return (Session["CurrentBatchIndex"] == null ? 0 : (int)Session["CurrentBatchIndex"]);
            }
            set
            {
                Session["CurrentBatchIndex"] = value;
            }
        }

        COEFormGroup duplicatesCOEFormManager;
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
                    Session["DisplayCulture"] = CS.ConfigurationUtilities.GetApplicationData(COEAppName.Get().ToString()).DisplayCulture;
                return (string)Session["DisplayCulture"];
            }
        }

        #endregion

        #region Events Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                if (!Page.IsPostBack)
                {
                    this.LoadControlInAccordionPanel();
                    this.SetControlsAttributtes();
                    this.SetPageMode(PageState.ViewRecord, String.Empty);
                }

                if (IsPreRegistration && this.RegistryRecord.RegNum == string.Empty)
                {
                    this.SubmitterComments.Visible = true;
                    this.SubmitterCommentsLabel.Visible = true;
                    this.SubmitterCommentsLabel.Text = "Submission Comments";
                }
                else
                {
                    if (String.IsNullOrEmpty(this.RegistryRecord.SubmissionComments))
                    {
                        this.SubmitterComments.Visible = false;
                        this.SubmitterCommentsLabel.Visible = false;
                    }
                    else
                    {
                        this.SubmitterComments.Visible = true;
                        this.SubmitterComments.ReadOnly = true;
                        this.SubmitterComments.Text = this.RegistryRecord.SubmissionComments;
                        this.SubmitterCommentsLabel.Visible = true;
                        this.SubmitterCommentsLabel.Text = "Submission Comments";
                    }

                    if (BatchPrefixesEnabled)
                    {
                        //LJB this sets a variable that indicates whether the salt and batch suffix dialog is shown
                        //when paging this needs to be reset so that it is looking at the correct record
                        BuildSaltandBatchNumPanel();
                        SetSaltBatchSuffixDialog();

                    }

                }
                this.MessagesAreaUserControl.Visible = true;
                if (RegUtilities.GetAllowUnregisteredComponents())
                {
                    this.MessagesAreaUserControl.AreaText = Resource.RegistryDuplicate_Message;
                }
                else
                {
                    if (IsPreRegistration && this.RegistryRecord.RegNum == string.Empty)
                    {
                        this.MessagesAreaUserControl.AreaText = Resource.MixtureDuplicatePreReg_Message;
                    }
                    else
                    {
                        this.MessagesAreaUserControl.AreaText = Resource.MixtureDuplicate_Message;
                    }
                }
                this.SetPageTitle();
                this.SubscribeToEventsInAccordionControl();
                this.DisplayRegistryIntoTree(Duplicates.Current);
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
            base.OnPreRender(e);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.SetPagingButtonsStatus();
                this.SetActionButtonsStatus();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }


        protected override void OnPreRenderComplete(EventArgs e)
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

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

        public void RenderJSArrayWithCliendIds()
        {
            StringBuilder arrClientIDValue = new StringBuilder();
            StringBuilder arrServerIDValue = new StringBuilder();
            StringBuilder sStartupRunScripts = new StringBuilder();
            ClientScriptManager cs = Page.ClientScript;

            getSubcontrols(this.duplicateFormHolder, ref arrClientIDValue, ref arrServerIDValue, ref sStartupRunScripts);

            // Register the array of client and server id to the client
            cs.RegisterArrayDeclaration("ClientIDList", arrClientIDValue.ToString().Remove(arrClientIDValue.ToString().Length - 1, 1));
            cs.RegisterArrayDeclaration("ServerIDList", arrServerIDValue.ToString().Remove(arrServerIDValue.ToString().Length - 1, 1));
            cs.RegisterStartupScript(typeof(RegistryDuplicates), "EnableDisaable", sStartupRunScripts.ToString(), true);

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

        protected override void OnInit(EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            #region Page Settings

            // To make more easy to read the code.
            _masterPage = (RegistrationMaster)this.Master;
            //Set the accordion key where is the customized control inside.
            this.AccordionGroupToListen = RegistryRecord.IsTemporal ? "MixtureDuplicateResolution" : "SearchRegistryRecord";
            //Set the cotrolID inside accordion group.
            this.AccordionControlToListen = "RegistryRecordNavigation";

            #endregion
            base.OnInit(e);
            try
            {
                this.LoadFormGenerators();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void NextLinkButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.PageToNext();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        protected void PreviousLinkButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.PageToPrevious();
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
                CopySaltandBacthSuffixValue();
                this.AddBatch();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void DuplicateButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.DuplicateMixture();
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
                this.CancelProcess();
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

        #endregion

        #region GUIShell Methods

        /// <summary>
        /// This method sets all the controls attributtes as Text, etc...
        /// </summary>
        /// <remarks>You have to expand the group of your interest in the accordion</remarks>
        protected override void SetControlsAttributtes()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.MessagesAreaUserControl.Visible = false;
            switch (Actions)
            {
                case AllowedActions.All:
                    RegistryRecord registry = (RegistryRecord)Session[Constants.MultiCompoundObject_Session];

                    if (registry.IsTemporal)
                    {

                        if (this.IsPreRegistration && registry.RegNum == string.Empty)
                        {
                            this.AddBatchButton.Text = Resource.AddBatch_Button_Text;
                            this.AddBatchButton.ToolTip = Resource.AddBatchPreReg_Button_ToolTip;
                            this.AddBatchButton.Visible = false;
                        }
                        else
                        {
                            this.AddBatchButton.Text = Resource.AddBatch_Button_Text;
                            this.AddBatchButton.ToolTip = Resource.AddBatch_Button_ToolTip;
                        }

                    }
                    else
                    {

                        this.AddBatchButton.Text = Resource.AddBatches_Button_Text;
                        this.AddBatchButton.ToolTip = Resource.MoveBatch_Button_ToolTip;
                        this.AddBatchButton.Visible = false;


                    }

                    break;
                case AllowedActions.AddBatch:
                    if (this.IsPreRegistration && this.RegistryRecord.RegNum == string.Empty)
                    {
                        this.AddBatchButton.Text = Resource.AddBatch_Button_Text;
                        this.AddBatchButton.ToolTip = Resource.AddBatchPreReg_Button_ToolTip;

                    }
                    else
                    {
                        this.AddBatchButton.Text = Resource.AddBatch_Button_Text;
                        this.AddBatchButton.ToolTip = Resource.AddBatch_Button_ToolTip;

                    }
                    this.DuplicateButton.Visible = false;
                    break;
            }

            this.DuplicateButton.Text = Resource.DuplicatedCompound_Button_Text;

            if (this.IsPreRegistration && this.RegistryRecord.RegNum == String.Empty)
            {
                this.DuplicateButton.ToolTip = Resource.DuplicateRegistryPreReg_Button_ToolTip;

            }
            else
            {
                this.DuplicateButton.ToolTip = Resource.DuplicateRegistry_Button_ToolTip;
            }
            this.SkipButton.Text = Resource.Skip_Button_Text;
            this.CancelButton.Text = Resource.Cancel_Button_Text;
            this.DoneButton.Text = Resource.Done_Button_Text;

            this.PreviousLinkButton.Text = Resource.Previous_Button_Text;
            this.NextLinkButton.Text = Resource.Next_Button_Text;
            this.PreviousLinkButton.ToolTip = Resource.DuplicateRegistries_PreviousLinkButton_ToolTip;
            this.NextLinkButton.ToolTip = Resource.DuplicateRegistries_NextLinkButton_ToolTip;


            if (RegUtilities.GetAllowUnregisteredComponents())
            {

                this.DisplayStatusMessage(string.Format(Resource.DuplicatedRegistry_Status_Text, this.Duplicates.Count));
            }
            else
            {
                if (IsPreRegistration && this.RegistryRecord.RegNum == String.Empty)
                {
                    this.DisplayStatusMessage(string.Format(Resource.DuplicatedMixtures_StatusPreReg_Text, this.Duplicates.Count));
                }
                else
                {
                    this.DisplayStatusMessage(string.Format(Resource.DuplicatedMixtures_Status_Text, this.Duplicates.Count));
                }
            }

            Control showButtonsIDHidden = _masterPage.GetControlInsideAnAccordionGroup(this.AccordionGroupToListen, ControlsToTakeCare.ShowButtonsHidden.ToString());
            if (showButtonsIDHidden != null)
                ((HiddenField)showButtonsIDHidden).Value = bool.FalseString;

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// This method sets the Page Title <Title></Title>
        /// </summary>
        protected void SetPageTitle()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.Page.Title = this.Header.Title = Resource.DuplicatedRecords_Page_Title;
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
                    case 'B':
                        this.DisplayBatch(index);
                        break;
                    case 'C':
                        this.DisplayComponent(index);
                        break;
                    default:
                        this.DisplayRecord();
                        break;
                }
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }



        private void DisplayRecord()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            duplicatesCOEFormManager.DataBind();


            this.PageTitleLabel.Text = string.Format(Resource.DulplicateRegistryNumber_Control_Text, Duplicates.Current.RegNumber.RegNum);

            this.DisplayStatusMessage(string.Format(Resource.DuplicatePager_Label_Text,
                                        Duplicates.CurrentIndex + 1,
                                        Duplicates.Count));

            this.DisplayRegistryIntoTree(Duplicates.Current);
            this.SetPageMode(PageState.ViewRecord, String.Empty);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void DisplayComponent(int index)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            SetFormGeneratorPageIndex(SubForms.CompoundForm, index);
            SetFormGeneratorPageIndex(SubForms.CompoundCustomForm, index);

            this.SetPageMode(PageState.ViewComponent, String.Empty);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void DisplayBatch(int index)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            SetFormGeneratorPageIndex(SubForms.BatchForm, index);
            SetFormGeneratorPageIndex(SubForms.BatchCustomForm, index);
            SetFormGeneratorPageIndex(SubForms.BatchComponentForm, index);
            SetFormGeneratorPageIndex(SubForms.BatchComponentCustomForm, index);

            this.SetPageMode(PageState.ViewBatch, String.Empty);
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

        private void DisplayRegistryIntoTree(RegistryRecord record)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            ICOENavigationPanelControl tree = _masterPage.GetICOENavigationPanelControlInsideAnAccordionGroup(this.AccordionGroupToListen);

            if (tree != null) //Coverity fix - CID 11859
            {
                if (this.AllowUnregisteredComponents)
                {
                    tree.NewRegistryType = RegistryTypes.Both.ToString().ToLower();
                }
                else
                {
                    tree.NewRegistryType = RegistryTypes.Mixture.ToString().ToLower();
                }


                if (RegUtilities.GetAllowUnregisteredComponents())
                {
                    tree.NewRegistryType = "both";
                    tree.DataBind(record, true);
                    tree.SetTitle(Resource.DuplicateRecord_TreeTitle);
                }
                else
                {
                    tree.NewRegistryType = "mixture";
                    tree.DataBind(record, true);
                    tree.SetTitle(Resource.DuplicateMixture_TreeTitle);
                }

            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        #endregion

        #region PageMethods
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
                FormGroup coeFormHolder = COEFormBO.Get(this.PageFormGroupID).COEFormGroup;

                duplicateFormHolder.Controls.Clear();
                duplicatesCOEFormManager = new COEFormGroup();
                duplicatesCOEFormManager.ID = "DuplicatesCOEFormManager";
                duplicateFormHolder.Controls.Add(duplicatesCOEFormManager);

                duplicatesCOEFormManager.FormGroupDescription = coeFormHolder;
                duplicatesCOEFormManager.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;
                duplicatesCOEFormManager.CurrentDisplayMode = FormGroup.DisplayMode.View;
                duplicatesCOEFormManager.DisplayCulture = this.DisplayCulture;
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
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void SetFormGeneratorPageIndex(SubForms subForm, int pageIndex)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            if (duplicatesCOEFormManager != null)
                duplicatesCOEFormManager.SetFormGeneratorPageIndex(GetFormIndex(subForm), pageIndex);

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        }

        private int GetFormIndex(SubForms subForm)
        {
            switch (subForm)
            {
                case SubForms.MixtureForm:
                    return 0;
                    break;
                case SubForms.MixtureCustomForm:
                    return 1000;
                    break;
                case SubForms.CompoundForm:
                    return 1;
                    break;
                case SubForms.CompoundCustomForm:
                    return 1001;
                    break;
                case SubForms.BatchForm:
                    return 2;
                    break;
                case SubForms.BatchCustomForm:
                    return 1002;
                    break;
                case SubForms.BatchComponentForm:
                    return 3;
                    break;
                case SubForms.BatchComponentCustomForm:
                    return 1003;
                    break;
                default:
                    return 0;
                    break;
            }
        }

        private void SetFormGeneratorVisibility(SubForms subForm, bool visible)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            if (duplicatesCOEFormManager != null)
                duplicatesCOEFormManager.SetFormGeneratorVisibility(GetFormIndex(subForm), visible);

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void PageToPrevious()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                Duplicates.CurrentIndex--;
                this.DisplayRecord();
                if (BatchPrefixesEnabled)
                {
                    SetSaltBatchSuffixDialog();
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

        private void PageToNext()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                Duplicates.CurrentIndex++;
                this.DisplayRecord();
                if (BatchPrefixesEnabled)
                {
                    SetSaltBatchSuffixDialog();
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

        private void SetActionButtonsStatus()
        {

            if (this.RegistryRecord.CanCreateDuplicateRegistry())
            {
                this.AddBatchButton.Visible = this.DuplicateButton.Visible = true;
            }
            else
            {
                this.AddBatchButton.Visible = true;
                this.DuplicateButton.Visible = false;

            }
        }
        //CSBR 149045 fixlet
        //In this added four lines of code for change the disable/enable buttons images.
        private void SetPagingButtonsStatus()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (Duplicates != null)
                {
                    this.NextLinkButton.Enabled = this.PreviousLinkButton.Enabled = true;
                    this.NextLinkButton.CssClass = "NextDupResolution";
                    this.PreviousLinkButton.CssClass = "BackDupResolution";
                    if (Duplicates.CurrentIndex >= Duplicates.Count - 1)
                    {
                        this.NextLinkButton.Enabled = false;
                        this.NextLinkButton.CssClass = "NextDupResolutionD";
                    }
                    if (Duplicates.CurrentIndex <= 0)
                    {
                        this.PreviousLinkButton.Enabled = false;
                        this.PreviousLinkButton.CssClass = "BackDupResolutionD";
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

        private void LeavePage()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            Session.Remove(Constants.DuplicateIdsList_Session);
            Session.Remove(Constants.DuplicateMultiCompounds_Session);
            string url = string.Empty;
            if (this._masterPage.AccordionControl.Groups.FromKey(this.AccordionGroupToListen) != null)
                url = this._masterPage.AccordionControl.Groups.FromKey(this.AccordionGroupToListen).TargetUrl;
            else
                url = Resource.SubmitMixture_URL;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            Server.Transfer(url, false);
        }

        private void DuplicateMixture()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string message = string.Empty;
            this.RegistryRecord.CheckOtherMixtures = false;
            string vRegNum = this.RegistryRecord.RegNumber.RegNum;
            Session.Remove(Constants.DuplicateIdsList_Session);
            Session.Remove(Constants.DuplicateMultiCompounds_Session);
            if (IsPreRegistration && this.RegistryRecord.RegNum == String.Empty)
            {
                this.RegistryRecord.SubmissionComments = String.Format(Resource.Submitter_SuggestedAction_DuplicateComponent, Csla.ApplicationContext.User.Identity.Name, this.SubmitterComments.Text);
                this.RegistryRecord.Save();
                Server.Transfer(string.Format("{0}?IsSubmit=true&SubmittedObjectId={1}&{2}={3}", this.ResolveUrl(Resource.ReviewRegister_URL), this.RegistryRecord.ID, Constants.CurrentPageState_UrlParameter, "end"));

            }
            else
            {
                if (this.RegistryRecord.IsDirectReg || this.RegistryRecord.IsTemporal)
                {
                    this.RegistryRecord.Register(DuplicateCheck.None);
                    Server.Transfer(string.Format("{0}?RegisteredObjectId={1}", this.ResolveUrl(Resource.ReviewRegister_URL), this.RegistryRecord.RegNumber.RegNum));

                }
                else
                {

                    this.RegistryRecord.Save(DuplicateCheck.None);

                    Server.Transfer(string.Format("{0}?RegisteredObjectId={1}", Resource.ViewMixture_URL, this.RegistryRecord.RegNumber.RegNum), false);

                }


            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void AddBatch()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string message = string.Empty;

            //CSBR 155767: validate that the batch being added has not broken any rules
            //create a clone rather than using directly from the list.  If there is a validation error
            //you need to start over with the same dupliate
            //without the clone, you are directly editing the record in the duplicatelist.

            RegistryRecord selectedDuplicate = Duplicates.Current.Clone();

            try
            {
                if (IsPreRegistration && this.RegistryRecord.RegNum == String.Empty)
                {
                    this.RegistryRecord.SubmissionComments = String.Format(Resource.Submitter_SuggestedAction_AddBatchMixture, Csla.ApplicationContext.User.Identity.Name, Duplicates.Current.RegNum, this.SubmitterComments.Text);
                    RegistryRecord.Save();
                    Session.Remove(Constants.DuplicateIdsList_Session);
                    Session.Remove(Constants.DuplicateMultiCompounds_Session);
                    Server.Transfer(string.Format("{0}?IsSubmit=true&SubmittedObjectId={1}&{2}={3}", this.ResolveUrl(Resource.ReviewRegister_URL), this.RegistryRecord.ID, Constants.CurrentPageState_UrlParameter, "end"));

                }
                else
                {
                    this.RegistryRecord.CheckOtherMixtures = false;

                    bool isTemporal = this.RegistryRecord.IsTemporal;

                    if (isTemporal)
                    {
                        selectedDuplicate.AddBatch(this.RegistryRecord.BatchList[0]);
                    }
                    else
                    {
                        selectedDuplicate.MoveBatches(this.RegistryRecord);
                    }

                    selectedDuplicate.CheckOtherMixtures = false;
                    //CSBR 155767: validate that the batch being added has not broken any rules
                    if (!selectedDuplicate.IsValid)
                    {
                        DisplayBrokenRules(selectedDuplicate.GetBrokenRulesDescription());

                    }
                    else
                    {
                        selectedDuplicate.Save(DuplicateCheck.None);

                        if (RegistryRecord.IsTemporal && RegistryRecord.Status == RegistryStatus.Approved)
                        {
                            //Next two lines are to make the record deletable
                            this.RegistryRecord.Status = RegistryStatus.Submitted;
                            this.RegistryRecord.SetApprovalStatus();
                        }
                        this.RegistryRecord.Delete();
                        this.RegistryRecord.Save();
                        this.DisplayRegistryIntoTree(selectedDuplicate);
                        this.Actions = AllowedActions.All;
                        //Transfer to appropriate page for review.  Put the duplicate chosen into the session object
                        selectedDuplicate.IsDirectReg = this.RegistryRecord.IsDirectReg;

                        Session[Constants.MultiCompoundObject_Session] = (RegistryRecord)selectedDuplicate;
                        string vRegNum = selectedDuplicate.RegNumber.RegNum;
                        Session.Remove(Constants.DuplicateIdsList_Session);
                        Session.Remove(Constants.DuplicateMultiCompounds_Session);
                        if (!this.RegistryRecord.IsDirectReg)
                        {
                            Server.Transfer(string.Format("{0}?RegisteredObjectId={1}", Resource.ReviewRegister_URL, vRegNum), false);
                        }
                        else
                        {
                            Server.Transfer(string.Format("{0}?RegisteredObjectId={1}", Resource.ViewMixture_URL, vRegNum), false);

                        }
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

        private void CancelProcess()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.LeavePage();
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
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

        private void SetPageMode(PageState mode, string confirmationMessage)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.duplicatesCOEFormManager.CurrentDisplayMode = FormGroup.DisplayMode.View;
            switch (mode)
            {
                case PageState.ViewRecord:
                    SetFormGeneratorVisibility(SubForms.MixtureForm, true);
                    SetFormGeneratorVisibility(SubForms.MixtureCustomForm, true);
                    SetFormGeneratorVisibility(SubForms.CompoundForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchCustomForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomForm, false);
                    break;
                case PageState.ViewComponent:
                    SetFormGeneratorVisibility(SubForms.MixtureForm, false);
                    SetFormGeneratorVisibility(SubForms.MixtureCustomForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundForm, true);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomForm, true);
                    SetFormGeneratorVisibility(SubForms.BatchForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchCustomForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomForm, false);
                    break;
                case PageState.ViewBatch:
                    SetFormGeneratorVisibility(SubForms.MixtureForm, false);
                    SetFormGeneratorVisibility(SubForms.MixtureCustomForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundForm, false);
                    SetFormGeneratorVisibility(SubForms.CompoundCustomForm, false);
                    SetFormGeneratorVisibility(SubForms.BatchForm, true);
                    SetFormGeneratorVisibility(SubForms.BatchCustomForm, true);
                    SetFormGeneratorVisibility(SubForms.BatchComponentForm, true);
                    SetFormGeneratorVisibility(SubForms.BatchComponentCustomForm, true);
                    break;

                case PageState.End:
                    if (IsPreRegistration && RegistryRecord.RegNum == String.Empty)
                    {
                        Server.Transfer(string.Format("{0}?SubmittedObjectId={1}", this.ResolveUrl(Resource.ReviewRegister_URL), this.RegistryRecord.ID));
                    }
                    else
                    {
                        Server.Transfer(string.Format("{0}?RegisteredObjectId={1}", Resource.ViewMixture_URL, RegistryRecord.RegNumber.RegNum), false);
                    }
                    break;
            }



            CurrentPageState = mode;
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
            throw new InvalidOperationException(errorMessage);
        }
        #endregion

        #region CSLA Datasources Events
        protected void RootCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = this.Page;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void ComponentListCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = this.RegistryRecord.ComponentList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        protected void mixtureCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.DisplayRegistryIntoTree(Duplicates.Current);

                if (this.RegistryRecord.RegNum == String.Empty && !this.AllowUnregisteredComponents)
                {
                    this.PageTitleLabel.Text = string.Format(Resource.DulplicateMixtureNumber_Control_Text, Duplicates.Current.RegNumber.RegNum);

                }
                else
                {
                    this.PageTitleLabel.Text = string.Format(Resource.DulplicateRegistryNumber_Control_Text, Duplicates.Current.RegNumber.RegNum);
                }

                e.BusinessObject = Duplicates.Current;
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
                e.BusinessObject = Duplicates.Current.BatchList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void BatchCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                int index = duplicatesCOEFormManager.GetFormGeneratorPageIndex(GetFormIndex(SubForms.BatchForm));

                if (index < Duplicates.Current.BatchList.Count)
                    e.BusinessObject = Duplicates.Current.BatchList[index];
                else
                    e.BusinessObject = null;
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

        protected void SequenceListCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            //Get the list of the sequences to be aplied to a Registry.
            try
            {
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

        #endregion

        #region batch prefix
        private void SetSaltBatchSuffixDialog()
        {
            try
            {
                int _legacydisplaylimit = (Session["LegacyFieldDisplayLimit"] == null ? 0 : (int)Session["LegacyFieldDisplayLimit"]);
                string sregnum = (this.Duplicates.Current != null ? this.Duplicates.Current.RegNum : "");
                if (this.Duplicates.Current.RegNumber.Sequence.Prefix != "")
                    sregnum = sregnum.Replace(this.Duplicates.Current.RegNumber.Sequence.Prefix, "");
                int curRegnum = (string.IsNullOrEmpty(sregnum) ? 0 : int.Parse(sregnum));
                //fix this to allow regnumber of 0
                if (_legacydisplaylimit > 0 && curRegnum >= 0 && curRegnum < _legacydisplaylimit)
                {
                    _showSaltBatchSuffixDialog = true;
                    EnableLegacyBatchPanel();
                }
                else
                {
                    _showSaltBatchSuffixDialog = false;
                    DisableLegacyBatchPanel();
                }

            }
            catch (Exception ex)
            {
                _showSaltBatchSuffixDialog = false;

            }

        }

        private void BuildSaltandBatchNumPanel()
        {


            Panel pnlheader = new Panel();
            Panel pnlBody = new Panel();
            Panel pnlfooter = new Panel();
            TextBox txtSaltandBatchNum = new TextBox();
            Button btnAddBatch = new Button();
            Button submitForm = new Button();
            Label headerLabel = new Label();
            Label required = new Label();
            Label lblSaltandbatchSuffix = new Label();
            /*Style changes for controls*/
            pnlheader.CssClass = "hd";
            pnlBody.CssClass = "bd";
            pnlfooter.CssClass = "hd";
            headerLabel.CssClass = "PickListDivTitle";
            btnAddBatch.CssClass = "ButtonBig";
            txtSaltandBatchNum.CssClass = "FETextBox";
            txtSaltandBatchNum.Width = new Unit("200px");
            lblSaltandbatchSuffix.CssClass = "FELabel";
            required.Style.Add("color", "red");
            submitForm.Style.Add("visibility", "hidden");
            pnlSaltandBatchNumPanel.Style.Add("visibility", "hidden");
            btnAddBatch.Enabled = false;
            /*Text for controls*/
            required.Text = " * ";
            headerLabel.Text = "Legacy Compound";
            lblSaltandbatchSuffix.Text = "Salt and Batch Suffix ";
            btnAddBatch.Text = "Add Batch";
            submitForm.ID = "submitForm";
            txtSaltandBatchNum.ID = "txtSaltandBatchNum";
            pnlSaltandBatchNumPanel.ID = "SaltandBatchNumPanel";
            submitForm.Click += AddBatchButton_Click;
            this.Form.Controls.Add(submitForm);
            btnAddBatch.Attributes.Add("onclick", "YAHOO.COERegistration.MessagePopupPanel.hide();document.getElementById('" + submitForm.ClientID + "').click()");  //fix for CSBR-166530 script issue.
            pnlheader.Controls.Add(headerLabel);
            pnlBody.Controls.Add(lblSaltandbatchSuffix);
            pnlBody.Controls.Add(txtSaltandBatchNum);
            pnlBody.Controls.Add(required);
            pnlBody.Controls.Add(btnAddBatch);
            pnlSaltandBatchNumPanel.Controls.Add(pnlheader);
            pnlSaltandBatchNumPanel.Controls.Add(pnlBody);
            pnlSaltandBatchNumPanel.Controls.Add(pnlfooter);
            this.Form.Controls.Add(pnlSaltandBatchNumPanel);
            txtSaltandBatchNum.Attributes.Add("onchange", "document.getElementById('" + this.SaltandbatchSuffixhid.ClientID + "').value=this.value;if(this.value==''){document.getElementById('" + btnAddBatch.ClientID + "').disabled =true;}else{document.getElementById('" + btnAddBatch.ClientID + "').disabled =false;}");
            txtSaltandBatchNum.Attributes.Add("onkeyup", "if(this.value==''){document.getElementById('" + btnAddBatch.ClientID + "').disabled =true;}else{document.getElementById('" + btnAddBatch.ClientID + "').disabled =false;}");

        }

        private void EnableLegacyBatchPanel()
        {
            this.AddBatchButton.OnClientClick = "ShowMessagePopUp('" + pnlSaltandBatchNumPanel.ClientID + "');return false;";

        }
        private void DisableLegacyBatchPanel()
        {
            this.AddBatchButton.OnClientClick = "";

        }

        private void CopySaltandBacthSuffixValue()
        {
            if (!string.IsNullOrEmpty(this.SaltandbatchSuffixhid.Value) && _showSaltBatchSuffixDialog)
            {
                if (this.RegistryRecord.BatchList[0] != null && this.RegistryRecord.BatchList[0].PropertyList["SALTANDBATCHSUFFIX"] != null)
                    this.RegistryRecord.BatchList[0].PropertyList["SALTANDBATCHSUFFIX"].Value = this.SaltandbatchSuffixhid.Value;
            }
        }
        #endregion
    }
}
