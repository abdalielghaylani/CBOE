using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.Common.Messaging;
using System.Xml;
using System.Reflection;
using Infragistics.WebUI.UltraWebNavigator;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Resources;
using Csla.Validation;
using CambridgeSoft.COE.Framework.COEPickListPickerService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using PerkinElmer.COE.Registration.Server.Code;
using CambridgeSoft.COE.Registration;
using PerkinElmer.COE.Registration.Server.Controls;


namespace PerkinElmer.COE.Registration.Server.Forms.ComponentDuplicates.ContentArea
{
    public partial class ComponentDuplicates : GUIShellPage
    {
        #region GUIShell Variables

        RegistrationMaster _masterPage = null;
        private string _controlToListen = String.Empty;
        private string _groupToListen = String.Empty;
        COENavigationPane COENavigationPaneObject = null;
        private string _registryID = String.Empty;
        private string _registryBatchID = String.Empty;

        private uint _compoundIndex = uint.MinValue;
        private uint _numberOfCompounds = uint.MinValue;
        private bool _allCompoundsHaveBeenSeen = false;
        private bool _isOriginatingFromTemp = true;
        private bool _showSaltBatchSuffixDialog = false;
        private Panel pnlSaltandBatchNumPanel = new Panel();
        private const string PROGRESS_MODEL_DUPLICATE = "Duplicating...";

        /// <summary>
        /// These are the controls which I can be interested in the Control(MyCustomizedControl)
        /// inside the Accordion Group.
        /// </summary>
        private enum ControlsToTakeCare
        {
            MessageLabel,
            DoneButton,
            ShowButtonsHidden,
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

        private enum PageState
        {
            Start,
            Display,
            End
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

        private bool AllCompoundsHaveBeenSeen
        {
            get { return this._allCompoundsHaveBeenSeen; }
            set { this._allCompoundsHaveBeenSeen = value; }
        }

        private uint CompoundIndex
        {
            get { return this._compoundIndex; }
            set { this._compoundIndex = value; }
        }

        private uint NumberOfCompounds
        {
            get { return this._numberOfCompounds; }
            set { this._numberOfCompounds = value; }
        }

        public bool BatchPrefixesEnabled
        {
            get
            {
                return RegUtilities.GetBatchPrefixesEnabled();
            }

        }

      

        #endregion

        #region Page Variables
        COEFormGroup _compoundsCOEFormManager;

        private enum SubForms
        {
            ComponentRepeaterSubForm,
            ComponentSubForm,
        }

       
        PageState CurrentPageState
        {
            get
            {
                /*if (Session[Constants.CurrentPageState_ViewState] == null)
                    return PageState.Start;
                else*/
                return (PageState)ViewState[Constants.CurrentPageState_ViewState];
            }
            set
            {
                ViewState[Constants.CurrentPageState_ViewState] = value;
            }
        }

        RegistryRecord RegistryRecord
        {
            get
            {
                return (RegistryRecord)Session[Constants.MultiCompoundObject_Session];
            }
            set
            {
                if (Session[Constants.MultiCompoundObject_Session] != null && !RegistryRecord.ReferenceEquals(value, Session[Constants.MultiCompoundObject_Session]))
                    ((IDisposable)Session[Constants.MultiCompoundObject_Session]).Dispose();

                Session[Constants.MultiCompoundObject_Session] = value;
            }
        }

        public DuplicatesResolver DuplicatesResolver
        {
            get
            {
                try
                {
                    if (Session["DuplicatesResolver"] == null)
                    {
                        Session["DuplicatesResolver"] = new DuplicatesResolver(RegistryRecord, Session[Constants.DuplicateCompoundIdsList_Session].ToString(), IsPreRegistration);

                    }

                    return (DuplicatesResolver)Session["DuplicatesResolver"];
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
                Session["DuplicatesResolver"] = value;
            }
        }


        bool IsAMixture
        {
            get
            {
                return true;
            }
        }

        bool IsTemporaryRegistry
        {
            get
            {
                if (ViewState["IsTemporaryRegistry"] == null)
                    ViewState["IsTemporaryRegistry"] = true;

                return (bool)ViewState["IsTemporaryRegistry"];
            }
            set
            {
                ViewState["IsTemporaryRegistry"] = value;
            }
        }

        int PageFormGroupID
        {
            get
            {
                return RegUtilities.GetCOEFormID("ComponentDuplicatesFormGroupId");
            }
        }

        public int CurrentBatchIndex
        {
            get
            {
                if (ViewState["CurrentBatchIndex"] == null)
                    ViewState["CurrentBatchIndex"] = 0;

                return (int)ViewState["CurrentBatchIndex"];                            
            }
            set
            {
                ViewState["CurrentBatchIndex"] = value;
            }
        }

        public bool IsPreRegistration
        {
            get
            {

                if (Request.QueryString["DupCheckOnSubmit"] != null)
                {
                    return System.Convert.ToBoolean(Request.QueryString["DupCheckOnSubmit"].ToString());
                }

               else{
                   return false;
               }
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
                if (!Page.IsPostBack)
                {
                    
                    this.IsTemporaryRegistry = RegistryRecord.IsTemporal;
                    DuplicatesResolver = null;

                    if (!DuplicatesResolver.HasUnsolvedComponents)
                    {
                        if (!string.IsNullOrEmpty(RegistryRecord.FoundDuplicates))
                            this.HandleMixtureDuplicates(RegistryRecord.FoundDuplicates);

                        SetPageMode(PageState.End);
                    }
                    else
                    {
                        if (RegistryRecord.IsSingleCompound)
                        {
                            this.SelectComponentToSolve(DuplicatesResolver.CompoundsToResolve[0].ID);
                            this.SetPageMode(PageState.Display);
                        }
                        else
                            this.SetPageMode(PageState.Start);
                    }
                    this.SetControlsAttributtes();
                    this.LoadControlInAccordionPanel();
                    this.SetAccordionControlsAttributes();
                }

                if (IsPreRegistration && RegistryRecord.RegNum == String.Empty)
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
                    if (BatchPrefixesEnabled && !IsPreRegistration)
                    {
                        //LJB this sets a variable that indicates whether the salt and batch suffix dialog is shown
                        //when paging this needs to be reset so that it is looking at the correct record
                        BuildSaltandBatchNumPanel();
                        SetSaltBatchSuffixDialog();

                    }
                }
                if (!RegUtilities.GetAllowUnregisteredComponents())
                {
                     this.MessagesAreaUserControl.Visible = true;
                }

                if (IsPreRegistration && RegistryRecord.RegNum == String.Empty)
                {
                    this.MessagesAreaUserControl.AreaText = Resource.ComponentDuplicatePreReg_Message;
                }
                else
                {
                    
                        this.MessagesAreaUserControl.AreaText = Resource.ComponentDuplicate_Message;
                    
                }
                
                this.SetPageTitle();
                this.ShowRegistryInTree();
                this.SubscribeToEventsInAccordionControl();
                _masterPage.MakeCtrlShowProgressModal(this.DuplicateRegistryButton.ClientID, PROGRESS_MODEL_DUPLICATE, string.Empty);
                
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
            try
            {
                this.SetActionButtonsStatus();
                this.SetPagingButtonsStatus();
                base.OnPreRender(e);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            #region Page Settings
            // To make more easy to read the code.
            _masterPage = (RegistrationMaster)this.Master;
            //Set the accordion key where is the customized control inside.
            this.AccordionGroupToListen = RegistryRecord.IsTemporal ? "ComponentDuplicateResolution" : "SearchRegistryRecord";
            //Set the cotrolID inside accordion group.
            this.AccordionControlToListen = "RegistryRecordNavigation";
            #endregion

            try
            {
                this.LoadFormGeneratorManager();

                base.OnPreInit(e);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        private void DuplicatedRegistryRecords_CommandRaised(object sender, COENavigationPanelControlEventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (e.EventType == EventsToListen.NodeClicked.ToString())
                {
                    Node clickedNode = ((UltraWebTree)((Control)sender).FindControl(ControlsToTakeCare.UltraWebTreeControl.ToString())).SelectedNode;
                    int tempId;
                    if (clickedNode.DataKey.ToString().StartsWith("B"))
                    {
                        this.CurrentBatchIndex = int.Parse(clickedNode.DataKey.ToString().Substring(1));
                        _compoundsCOEFormManager.DataBind();
                    }
                    else if (int.TryParse(clickedNode.DataKey.ToString(), out tempId))
                    {
                        this.SelectComponentToSolve(tempId);
                    }
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        public void ResolveLinkButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                int tempOrderIndex = 0;
                if (sender is LinkButton)
                    int.TryParse(((LinkButton)sender).CommandArgument, out tempOrderIndex);

                this.SelectComponentToSolve(DuplicatesResolver.CompoundsToResolve[tempOrderIndex].ID);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        private void SelectComponentToSolve(int tempId)
        {
            CurrentBatchIndex = 0;
            DuplicatesResolver.SelectCompoundToSolveById(tempId);

            _compoundsCOEFormManager.DataBind();

            SetPageMode(PageState.Display);
            string text = string.Format(Resource.ComponentId_Duplicates_Text, Math.Abs(tempId));
            this.PageTitleLabel.Text = text;
            

            this.PageToStart();
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.RegistryRecord.UpdateDrawingType();
                LeavePage();
                //LeavePage();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void NewCompoundButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.CreateNewCompoundForm();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void CreateCompoundFormButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                CreateNewCompoundForm();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void NextButton_Click(object sender, EventArgs e)
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

        protected void BackButton_Click(object sender, EventArgs e)
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

        protected void DuplicateRegistryButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.CreateDuplicate();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void UseStructureButton_Click(object sender, EventArgs e) 
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.UseStructure();
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


        public void OverrideBatchButtonVisibility()
        {
            if (!this.IsTemporaryRegistry)
            {
                if (RegUtilities.GetAllowMoveBatch() == false){
                    this.AddBatchButton.Visible = false;
                }
            }
        }

        /// <summary>
        /// This method sets all the controls attributtes as Text, etc...
        /// </summary>
        /// <remarks>You have to expand the group of your interest in the accordion</remarks>
        protected override void SetControlsAttributtes()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            if (this.IsTemporaryRegistry)
            {
                if (IsPreRegistration)
                {
                    this.AddBatchButton.Text = Resource.AddBatch_Button_Text;
                    this.AddBatchButton.ToolTip = Resource.AddBatchPreReg_Button_ToolTip;
                }
                else
                {

                    this.AddBatchButton.Text = Resource.AddBatch_Button_Text;
                    this.AddBatchButton.ToolTip = Resource.AddBatch_Button_ToolTip;
                }
               
                
            }
            else
            {

                
                    this.AddBatchButton.Text = Resource.MoveBatches_Button_Text;
                    this.AddBatchButton.ToolTip = Resource.MoveBatch_Button_ToolTip;
                
                 
                
                    
                
            }

            this.DuplicateRegistryButton.Text = Resource.DuplicatedCompound_Button_Text;

            if (this.IsTemporaryRegistry)
            {
                if (IsPreRegistration)
                {
                    this.DuplicateRegistryButton.ToolTip = Resource.DuplicateCompoundPreReg_Button_ToolTip;
                }
                else
                {

                    this.DuplicateRegistryButton.ToolTip = Resource.DuplicateCompound_Button_ToolTip;
                }
                
            }
            else
            {
               
                    this.DuplicateRegistryButton.ToolTip = Resource.OverwriteCompound_Button_ToolTip;
                
            }

            this.CreateCompoundFormButton.Text = Resource.UseCompound_Button_Text;

            if (this.IsTemporaryRegistry)
            {
                if (IsPreRegistration)
                {
                    this.CreateCompoundFormButton.ToolTip = Resource.UseCompoundPreReg_Button_ToolTip;
                }
                else
                {

                    this.CreateCompoundFormButton.ToolTip = Resource.UseCompound_Button_ToolTip;
                }
             
            }
            else
            { 
              
                this.CreateCompoundFormButton.ToolTip = Resource.UseCompound_Button_ToolTip;

                
            }

            this.UseStructureButton.Text = Resource.UseStructure_Button_Text;
            if (this.IsTemporaryRegistry)
            {

                if (IsPreRegistration)
                {
                    this.UseStructureButton.ToolTip = Resource.UseStructureOnRegisteringPreReg_Button_ToolTip;
                }
                else
                {

                    this.UseStructureButton.ToolTip = Resource.UseStructureOnRegistering_Button_ToolTip;
                }
               
            }
            else
            {
               
                    this.UseStructureButton.ToolTip = Resource.UseStructureOnEdit_Button_ToolTip;
                
            }

            this.NextLinkButton.Text = this.NextButtonTop.Text = Resource.Next_Button_Text;
            this.BackLinkButton.Text = this.BackButtonTop.Text = Resource.Previous_Button_Text;
            this.BackLinkButton.Enabled = this.BackButtonTop.Enabled = false;
            this.MessagesAreaUserControl.Visible = false;

            this.CancelButton.Text = Resource.Cancel_Button_Text;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// This method sets the Page Title <Title></Title>
        /// </summary>
        protected void SetPageTitle()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
          
             this.Header.Title = this.Page.Title = Resource.DuplicatedRecords_Page_Title;
            

            if (string.IsNullOrEmpty(this.PageTitleLabel.Text))
            {
                this.ExplanationContainerRow.Visible = this.ExplanationMessageLabel.Visible = true;
                this.ExplanationMessageLabel.Text = Resource.CheckDuplicates_Label_Text;
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
                        ((ICOENavigationPanelControl)currentGroup.UserControl).CommandRaised += new EventHandler<COENavigationPanelControlEventArgs>(DuplicatedRegistryRecords_CommandRaised);
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
            this.GetControlInfo();
            for (int i = 0; i < COENavigationPaneObject.ControlItem.Count; i++)
            {
                _masterPage.AddControlToAccordionPanel(COENavigationPaneObject.ControlItem[i], true);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void SetAccordionControlsAttributes()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (IsPreRegistration && RegistryRecord.RegNum == String.Empty)
            {
                
                this.DisplayStatusMessage(string.Format(Resource.DuplicatedCompounds_StatusPreReg_Text, DuplicatesResolver.Duplicates.Count));
            }
            else
            {

                this.DisplayStatusMessage(string.Format(Resource.DuplicatedCompounds_Status_Text, DuplicatesResolver.Duplicates.Count));

            }
            Control showButtonsIDHidden = _masterPage.GetControlInsideAnAccordionGroup(this.AccordionGroupToListen, ControlsToTakeCare.ShowButtonsHidden.ToString());
            if (showButtonsIDHidden != null)
                ((HiddenField)showButtonsIDHidden).Value = bool.FalseString;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void ShowRegistryInTree()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            ICOENavigationPanelControl tree = _masterPage.GetICOENavigationPanelControlInsideAnAccordionGroup(this.AccordionGroupToListen);

            if (tree != null)
            {
                this.SetActionButtonsStatus();

                if (AddBatchButton.Visible)
                {
                    tree.DataBind(DuplicatesResolver.Duplicates.Current.BatchList);
                    tree.SetTitle(Resource.DuplicatedRegistryComponent_Tree_Title);
                    /*string text = string.Format(Resource.ComponentId_Duplicates_Text, ViewState[Constants.SelectedNode_ViewState]);
                    if(PageTitleLabel.Text != Resource.CheckDuplicates_Label_Text)
                        this.PageTitleLabel.Text = text;*/
                }
                else
                {
                    tree.DataBind(DuplicatesResolver.CompoundsToResolve);
                    tree.SetTitle("Components to Resolve");
                    /*string text = string.Format(Resource.ComponentId_Duplicates_Text, ViewState[Constants.SelectedNode_ViewState]);
                    if(PageTitleLabel.Text != Resource.CheckDuplicates_Label_Text)
                        this.PageTitleLabel.Text = text;*/
                }
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region Methods

        /// <summary> 
        /// Method which load the Control Info in a DataSet (jsut for testing purpose)
        /// This Data Object will come from the Configuration Services
        /// </summary>
        private void GetControlInfo()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            COENavigationPaneObject = new COENavigationPane();

            COENavigationPane.ControlItemRow checkDuplicatedRegistryRecordsControlRow = COENavigationPaneObject.ControlItem.NewControlItemRow();
            checkDuplicatedRegistryRecordsControlRow.ParentID = this.AccordionGroupToListen;
            checkDuplicatedRegistryRecordsControlRow.ControlType = GUIShellTypes.SupportedControlTypes.UserControl.ToString();
            if (checkDuplicatedRegistryRecordsControlRow.ControlType == GUIShellTypes.SupportedControlTypes.UserControl.ToString())
                checkDuplicatedRegistryRecordsControlRow.ControlSource = GUIShellTypes.UCPath + "RegistryRecordNavigation.ascx";
            checkDuplicatedRegistryRecordsControlRow.ID = this.AccordionControlToListen;
            COENavigationPaneObject.ControlItem.AddControlItemRow(checkDuplicatedRegistryRecordsControlRow);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void LoadFormGeneratorManager()
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                duplicateRepeaterFormHolder.Controls.Clear();
                _compoundsCOEFormManager = new COEFormGroup();
                _compoundsCOEFormManager.ID = "DuplicatesRepeaterCOEFormManager";
                duplicateRepeaterFormHolder.Controls.Add(_compoundsCOEFormManager);
                //_compoundsCOEFormManager.FormGroupDescription = BuildCoeFormHolder("../FormGeneratorXml/ComponentDuplicates.xml");
                _compoundsCOEFormManager.FormGroupDescription = COEFormBO.Get(this.PageFormGroupID).COEFormGroup;
                _compoundsCOEFormManager.DisplayCulture = this.DisplayCulture;
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
        private void PageToPrevious()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                DuplicatesResolver.Duplicates.CurrentIndex--;
                DuplicatesResolver.DetermineAvailableActions();
                _compoundsCOEFormManager.SetFormGeneratorsPageIndex(DuplicatesResolver.Duplicates.CurrentIndex);

                this.DisplayStatusMessage(string.Format(Resource.DuplicatePager_Label_Text,
                                                            DuplicatesResolver.Duplicates.CurrentIndex + 1,
                                                            DuplicatesResolver.Duplicates.Count));
                CurrentBatchIndex = 0;
                this.SetPagingButtonsStatus();
                this.ShowRegistryInTree();
                if (BatchPrefixesEnabled && !IsPreRegistration)
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

        private int GetFormIndex(SubForms subForms)
        {
            return 0;
        }

        private void PageToNext()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                DuplicatesResolver.Duplicates.CurrentIndex++;
                DuplicatesResolver.DetermineAvailableActions();
                _compoundsCOEFormManager.SetFormGeneratorsPageIndex(DuplicatesResolver.Duplicates.CurrentIndex);

                this.DisplayStatusMessage(string.Format(Resource.DuplicatePager_Label_Text,
                                            DuplicatesResolver.Duplicates.CurrentIndex + 1,
                                            this.DuplicatesResolver.Duplicates.Count));
                CurrentBatchIndex = 0;
                this.SetPagingButtonsStatus();
                this.ShowRegistryInTree();
                if (BatchPrefixesEnabled && !IsPreRegistration)
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

        private void PageToStart()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                DuplicatesResolver.Duplicates.CurrentIndex = 0;
                _compoundsCOEFormManager.SetFormGeneratorsPageIndex(DuplicatesResolver.Duplicates.CurrentIndex);

                this.DisplayStatusMessage(string.Format(Resource.DuplicatePager_Label_Text,
                                            DuplicatesResolver.Duplicates.CurrentIndex + 1,
                                            this.DuplicatesResolver.Duplicates.Count));

                CurrentBatchIndex = 0;
                this.SetPagingButtonsStatus();
                this.ShowRegistryInTree();
                if (BatchPrefixesEnabled && !IsPreRegistration)
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
            this.AddBatchButton.Visible = this.DuplicateRegistryButton.Visible = this.CreateCompoundFormButton.Visible = false;

            if (CurrentPageState == PageState.Display)
            {
                this.AddBatchButton.Visible = this.DuplicatesResolver.CanAddBatch;
                this.CreateCompoundFormButton.Visible = this.DuplicatesResolver.CanCreateCompoundForm;
                this.DuplicateRegistryButton.Visible = this.DuplicatesResolver.CanDuplicate;
                this.UseStructureButton.Visible = this.DuplicatesResolver.CanUseStructure;
            }
            OverrideBatchButtonVisibility();
        }

        private void SetPagingButtonsStatus()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {

                if (CurrentPageState == PageState.Display)
                {
                    if (this.DuplicatesResolver.Duplicates.Count > 0)
                    {
                        this.NextLinkButton.Enabled = this.BackLinkButton.Enabled = this.NextButtonTop.Enabled = this.BackButtonTop.Enabled = true;
                        this.NextLinkButton.Visible = this.BackLinkButton.Visible = this.NextButtonTop.Visible = this.BackButtonTop.Visible = true;
                        this.NextLinkButton.CssClass = this.NextButtonTop.CssClass = "NextDupResolution";
                        this.BackLinkButton.CssClass = this.BackButtonTop.CssClass = "BackDupResolution";
                        int currentPageIndex = DuplicatesResolver.Duplicates.CurrentIndex;

                        if (currentPageIndex >= this.DuplicatesResolver.Duplicates.Count - 1)
                        {
                            this.NextLinkButton.Enabled = this.NextButtonTop.Enabled = false;
                            this.NextLinkButton.CssClass = this.NextButtonTop.CssClass = "NextDupResolutionD";
                        }

                        if (currentPageIndex <= 0)
                        {
                            this.BackLinkButton.Enabled = this.BackButtonTop.Enabled = false;
                            this.BackLinkButton.CssClass = this.BackButtonTop.CssClass = "BackDupResolutionD";
                        }
                    }
                }
                else if (CurrentPageState == PageState.Start)
                {
                    BackLinkButton.Visible = BackButtonTop.Visible = NextButtonTop.Visible = NextLinkButton.Visible = false;
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
            Session.Remove(Constants.DuplicateCompoundIdsList_Session);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

            if (this.DuplicatesResolver.RegistryRecord.IsTemporal)
            {
                if (_masterPage.PreviousPage.AbsolutePath.Contains(Resource.ReviewRegister_URL.Substring(Resource.ReviewRegister_URL.LastIndexOf("/"))))
                    Server.Transfer(string.Format("{0}?SubmittedObjectId={1}", this.ResolveUrl(Resource.ReviewRegister_URL), this.DuplicatesResolver.RegistryRecord.ID));
                else
                    Server.Transfer(string.Format("{0}?" + Resource.RestoreSessionObject_URLParameter + "={1}", this.ResolveUrl(Resource.SubmitMixture_URL),bool.TrueString));
            }
            else
                Server.Transfer(string.Format("{0}?RegisteredObjectId={1}", this.ResolveUrl(Resource.ViewMixture_URL), this.DuplicatesResolver.RegistryRecord.RegNumber.RegNum));
        }

        private void SetPageMode(PageState state)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                CurrentPageState = state;
                switch (state)
                {
                    case PageState.End:
                        if (IsPreRegistration)
                        {
                            Server.Transfer(string.Format("{0}?IsSubmit=true&SubmittedObjectId={1}&{2}={3}", this.ResolveUrl(Resource.ReviewRegister_URL), this.RegistryRecord.ID, Constants.CurrentPageState_UrlParameter, "end"));

                        }
                        else
                        {
                            if (!this.RegistryRecord.IsDirectReg)
                            {
                                Server.Transfer(string.Format("{0}?RegisteredObjectId={1}", this.ResolveUrl(Resource.ReviewRegister_URL), RegistryRecord.RegNumber.RegNum), false);


                            }
                            else
                            {
                                Server.Transfer(string.Format("{0}?RegisteredObjectId={1}", Resource.ViewMixture_URL, RegistryRecord.RegNumber.RegNum), false);
                            }
                        } 
                        break;
                    case PageState.Start:
                        _compoundsCOEFormManager.CurrentDisplayGroup = FormGroup.CurrentFormEnum.ListForm;
                        _compoundsCOEFormManager.CurrentDisplayMode = FormGroup.DisplayMode.View;
                        OverrideBatchButtonVisibility();
                        break;
                    case PageState.Display:
                        _compoundsCOEFormManager.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;
                        _compoundsCOEFormManager.CurrentDisplayMode = FormGroup.DisplayMode.View;
                        OverrideBatchButtonVisibility();
                        break;
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

        private void SetPagerButtonsVisibility(bool visible)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.NextLinkButton.Visible = this.BackLinkButton.Visible = this.NextButtonTop.Visible = this.BackButtonTop.Visible = visible;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void CreateDuplicate()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (IsPreRegistration && RegistryRecord.RegNum == String.Empty)
            {
                RegistryRecord = DuplicatesResolver.CreateDuplicate(true, this.SubmitterComments.Text);
            }
            else
            {
               
                RegistryRecord = DuplicatesResolver.CreateDuplicate();
            }
            if (DuplicatesResolver.HasUnsolvedComponents)
                this.SelectComponentToSolve(DuplicatesResolver.CurrentSolvingCompound.ID);
            else
                SetPageMode(PageState.End);

            this.ShowRegistryInTree();

            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void CreateNewCompoundForm()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (IsPreRegistration && RegistryRecord.RegNum == String.Empty)
                {
                    RegistryRecord = DuplicatesResolver.CreateCompoundForm(true, this.SubmitterComments.Text);
                }
                else
                {
                    
                    RegistryRecord = this.DuplicatesResolver.CreateCompoundForm();
                }

                if (DuplicatesResolver.HasUnsolvedComponents)
                    this.SelectComponentToSolve(DuplicatesResolver.CurrentSolvingCompound.ID);
                else
                {
                    if (!string.IsNullOrEmpty(RegistryRecord.FoundDuplicates))
                        this.HandleMixtureDuplicates(RegistryRecord.FoundDuplicates);

                    SetPageMode(PageState.End);
                }

                this.ShowRegistryInTree();
            }
            catch (ValidationException validationException)
            {
                this.ShowConfirmationMessage(validationException.Message);
            }
            catch (Exception exception)
            {
                _masterPage.DisplayErrorMessage(exception, false);
            }
            finally
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void HandleMixtureDuplicates(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                switch (RegUtilities.HandleDuplicates(RegistryRecord, message, IsPreRegistration))
                {
                    case RegUtilities.DuplicateType.Mixture:
                        Server.Transfer(
                            string.Format(
                                "{0}?{1}={2}",
                                Resource.RegistryDuplicates_Page_URL,
                                Constants.AllowedActionsParameter,
                                this.IsAMixture ? AllowedActions.AddBatch : AllowedActions.All
                            )
                        );
                        break;
                }
            }
        }

        private void AddBatch()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (IsPreRegistration && RegistryRecord.RegNum == String.Empty)
                {
                    RegistryRecord = DuplicatesResolver.AddBatch(true, this.SubmitterComments.Text);
                }
                else
                {
                    
                    RegistryRecord = this.DuplicatesResolver.AddBatch();
                    
                    
                }
                if (DuplicatesResolver.HasUnsolvedComponents)
                {
                    PageToStart();
                    _compoundsCOEFormManager.DataBind();
                }
                else
                {
                    SetPageMode(PageState.End);
                }
            }
            catch (ValidationException validationException)
            {
                this.ShowConfirmationMessage(validationException.Message);
            }
            catch (Exception exception)
            {
                _masterPage.DisplayErrorMessage(exception, false);
                
            }
            finally
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
        }

        /// <summary>
        /// Creates a new registry record linking to the structure of this existing record
        /// </summary>
        private void UseStructure() 
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (IsPreRegistration && RegistryRecord.RegNum == String.Empty)
                {
                    RegistryRecord = DuplicatesResolver.UseStructure(true, this.SubmitterComments.Text);
                }
                else
                {
                    
                    RegistryRecord = this.DuplicatesResolver.UseStructure();
                }
                if (DuplicatesResolver.HasUnsolvedComponents)
                    this.SelectComponentToSolve(DuplicatesResolver.CurrentSolvingCompound.ID);
                else
                {
                    if (!string.IsNullOrEmpty(RegistryRecord.FoundDuplicates))
                        this.HandleMixtureDuplicates(RegistryRecord.FoundDuplicates);

                    SetPageMode(PageState.End);
                }

                this.ShowRegistryInTree();
            }
            catch (Exception exception)
            {
                _masterPage.DisplayErrorMessage(exception, false);
            }
            finally 
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
        }

       
       

        #endregion

        #region DataSources Events
        protected void CompoundListCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = this.DuplicatesResolver.CompoundsToResolve;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void DuplicateBatchesCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = DuplicatesResolver.Duplicates.Current.BatchList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

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

        protected void DuplicatesCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = DuplicatesResolver;
            }
            catch (Exception exception)
            {
                //if (ExceptionPolicy.HandleException(exception, GUIShellTypes.PoliciesNames.UIExceptionPolicy.ToString()))
                //    throw;
                //else
                //    _masterPage.DisplayErrorMessage(exception, false);
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void CurrentDuplicateCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = DuplicatesResolver.Duplicates.Current;
            }
            catch (Exception exception)
            {
                //if (ExceptionPolicy.HandleException(exception, GUIShellTypes.PoliciesNames.UIExceptionPolicy.ToString()))
                //    throw;
                //else
                //    _masterPage.DisplayErrorMessage(exception, false);
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void DuplicateCompoundsCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                foreach (Component currentComponent in DuplicatesResolver.Duplicates.Current.ComponentList)
                    if (currentComponent.Compound.ID == DuplicatesResolver.Duplicates.CurrentCompoundId)
                    {
                        e.BusinessObject = DuplicatesResolver.Duplicates.Current.ComponentList[DuplicatesResolver.Duplicates.CurrentCompoundIndex];
                        break;
                    }
            }
            catch (Exception exception)
            {
                //if (ExceptionPolicy.HandleException(exception, Constants.REG_GUI_POLICY))
                //    throw;
                //else
                //    _masterPage.DisplayErrorMessage(exception, false);
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
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

        protected void FragmentTypesCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                e.BusinessObject = FragmentNameValueList.GetFragmentTypesNameValueList();
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }	
        }

        protected void FragmentNamesCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                e.BusinessObject = FragmentList.GetFragmentList();
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

        protected void SequenceListCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                //Get the list of the sequences to be aplied to a Registry.
                e.BusinessObject = SequenceList.GetSequenceList(1);
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
            try
            {
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
                e.BusinessObject = IdentifierList.GetIdentifierList();
                RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
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
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void BatchListCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = DuplicatesResolver.Duplicates.Current.BatchList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void CurrentBatchCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                e.BusinessObject = DuplicatesResolver.Duplicates.Current.BatchList[CurrentBatchIndex];
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region batch prefix
        private void SetSaltBatchSuffixDialog()
        {
            try
            {
                int _legacydisplaylimit = (Session["LegacyFieldDisplayLimit"] == null ? 0 : (int)Session["LegacyFieldDisplayLimit"]);
                string sregnum = (this.DuplicatesResolver.Duplicates.Current != null ? this.DuplicatesResolver.Duplicates.Current.RegNum : "");
                if (this.DuplicatesResolver.Duplicates.Current.RegNumber.Sequence.Prefix != "")
                    sregnum = sregnum.Replace(this.DuplicatesResolver.Duplicates.Current.RegNumber.Sequence.Prefix, "");
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
            btnAddBatch.Attributes.Add("onclick", "YAHOO.COERegistration.MessagePopupPanel.hide();document.getElementById('" + submitForm.ClientID + "').click()"); //fix for CSBR-166530 script issue.
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
