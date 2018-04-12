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
using CambridgeSoft.COE.Framework.GUIShell;
using Resources;
using System.Reflection;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using System.Collections.Generic;
using Csla.Validation;
using CambridgeSoft.COE.DocumentManager.Services.Types;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using System.Xml;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using CambridgeSoft.COE.Framework.COEFormService;
using System.IO;
using CambridgeSoft.COE.Framework.COEGenericObjectStorageService;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;

namespace DocManagerWeb.Forms.Public.ContentArea
{
    public partial class ViewDocument : GUIShellPage
    {

        #region GUIShell Variables

        COENavigationPane COENavigationPaneObject = null;
        DocManagerMaster _masterPage = null;
        private string _controlToListen = String.Empty;
        private string _groupToListen = String.Empty;

        private const int minComponents = 1;

        #endregion

        #region Page Variables

        private enum PageState
        {
            None,
            EditDocument,
            AddDocument,
            ViewDocument,
            End
        }


        COEFormGroup _documentCOEFormGroup;
        int _docId;

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

        Document DocumentRecord
        {
            get
            {
                if (Session[Constants.DocumentObject_Session] == null)
                    Session[Constants.DocumentObject_Session] = Document.GetNewDocument();


                return (Document)Session[Constants.DocumentObject_Session];
            }
            set
            {
                if (Session[Constants.DocumentObject_Session] != null && !Document.ReferenceEquals(value, Session[Constants.DocumentObject_Session]))
                    ((IDisposable)Session[Constants.DocumentObject_Session]).Dispose();

                Session[Constants.DocumentObject_Session] = value;
            }
        }
        #endregion

        #region Page Properties


        /// <summary>
        /// Check if the current document is a already submitted.
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
            ShowButtonsHidden,
            MessageLabel,
            DoneButton,
            UltraWebTreeControl
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
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            try
            {

                if (!Page.IsPostBack)
                {
                    this.SetControlsAttributtes();
                    SetJScriptReference();

                    Int32.TryParse(Request[Constants.DocId_UrlParameter], out _docId);
                    if (_docId != 0)
                    {
                        this.DocumentRecord = Document.GetDocumentByID(_docId);
                        this.SetPageMode(PageState.ViewDocument, String.Empty);
                    }
                    else if (!this.RestoreSavedObject() && !this.RestoreSessionObject())
                    {
                        this.DocumentRecord = null;
                        this.SetPageMode(PageState.AddDocument, String.Empty);
                    }

                }
                else
                {
                    if (this.DocumentRecord == null)
                        _masterPage.DisplayErrorMessage(Resource.RecordNotFound_Label_Text, false);

                    this.HideConfirmationMessage();
                }
                this.SetPageTitle();
                _masterPage.MakeCtrlShowProgressModal(this.SubmitButton.ClientID, "Submitting...", string.Empty);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception.Message, false);
            }

            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected override void OnPreInit(EventArgs e)
        {
            ((DocManagerMaster)Master).FormGroupIdKey = "SubmitNewDocFormGroupId";

            #region Page Settings
            // To make easier to read the code.
            _masterPage = (DocManagerMaster)this.Master;
            //Set the accordion key where is the customized control inside.
            this.AccordionGroupToListen = "SubmitNewDocument";
            //Set the cotrolID inside accordion group.
            this.AccordionControlToListen = "SubmitDocumentNavigation";
            #endregion

            this.LoadFormGenerators();

            base.OnPreInit(e);

        }

        protected override void OnInit(EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                base.OnInit(e);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception.Message, false);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod,
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

        protected override void OnPreRenderComplete(EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            try
            {
                base.OnPreRenderComplete(e);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception.Message, false);
            }

            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (Page.IsValid)
                {
                    this.SubmitDocument();
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception.Message, false);
            }
            finally
            {
                Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {

            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.LeavePage();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception.Message, false);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void ConfirmationMessageBox_BtnNoClicked(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                Server.Transfer(string.Format("{0}?{1}={2}", Resource.SaveDocumentForm_Page_URL, Constants.FormGroup_UrlParameter, 1), false);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception.Message, false);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void ConfirmationMessageBox_BtnYesClicked(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (((Button)sender).CommandName.Equals("GoSubmitNewDocument"))
                    Server.Transfer(Resource.SubmitNewDocument_URL);
                else
                    Server.Transfer(Resource.Home_URL);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception.Message, false);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void DoneButton_Click(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                this.LeavePage();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception.Message, false);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void EditButton_Click(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (AlreadySubmitted)//Display in ViewMode
                    this.SetPageMode(PageState.EditDocument, string.Empty);
                else
                {

                }
                _documentCOEFormGroup.DataBind();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception.Message, false);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        public void TitleLinkButton_Click(object sender, EventArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                Document documentRecord = (Document)Session[Constants.NewDocumentObject_Session];
                if (documentRecord != null)
                {
                    //DocManager/Forms/Public/ContentArea/BinaryWriter.aspx?DocId={0}&amp;disposition=1
                    Server.Transfer(string.Format("{0}?{1}={2}&{3}={4}", Resource.BinaryWriter_Page_URL, Constants.DocId_UrlParameter, documentRecord.ID, Constants.BinaryWriter_UrlParameter, 1), false);

                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception.Message, false);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region GUIShell Methods

        /// <summary>
        /// This method sets all the controls attributtes as Text, etc...
        /// </summary>
        /// <remarks>You have to expand the group of your interest in the accordion</remarks>
        protected override void SetControlsAttributtes()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            this.SubmitButton.Text = Resource.Submit_Button_Text;
            this.CancelButton.Text = Resource.Cancel_Button_Text;
            this.BackButton.Text = Resource.Back_Button_Text;
            this.DoneButton.Text = Resource.Done_Button_Text;
            this.EditButton.Text = Resource.Edit_Button_Text;
            this.ClearFormButton.Text = Resource.Clear_Button_Text;

            Control messageLabel = _masterPage.GetControlInsideAnAccordionGroup(this.AccordionGroupToListen, ControlsToTakeCare.MessageLabel.ToString());
            _masterPage.SetDefaultButton(this.SubmitButton.UniqueID);

            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// This method sets the Page Title <Title></Title>
        /// </summary>
        protected void SetPageTitle()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            this.PageTitleLabel.Text = Resource.SubmitNewDocument_Page_Title;
            Header.Title = Resource.SubmitNewDocument_Page_Title;

            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);

        }

        /// <summary>
        /// Set the reference to the JScript Page.
        /// </summary>
        private void SetJScriptReference()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            string jScriptKey = this.ID.ToString() + "CommonScriptsPage";

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(jScriptKey))
                Page.ClientScript.RegisterClientScriptInclude(jScriptKey, Page.ResolveUrl(Constants.GetPublicCommonJSsPath()));

            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void DisplayStatusMessage(string text)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            Control messageLabel = _masterPage.GetControlInsideAnAccordionGroup(this.AccordionGroupToListen, ControlsToTakeCare.MessageLabel.ToString());
            if (messageLabel != null)
            {
                ((Label)messageLabel).Text = text;
                ((Label)messageLabel).Visible = true;
            }

            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region Page Methods

        private FormGroup BuildCoeFormHolder(string xmlUrl)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(Page.MapPath(Page.TemplateSourceDirectory) + "/" + xmlUrl);
                return FormGroup.GetFormGroup(xmlDocument.OuterXml);
            }
            catch (Exception exception)
            {
                if (ExceptionPolicy.HandleException(exception, Constants.DOC_GUI_POLICY))
                    throw;
                else
                    _masterPage.DisplayErrorMessage(exception.Message, false);

                return null;
            }
            finally
            {
                Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void LoadFormGenerators()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            try
            {
                FormGroup coeFormCompoundHolder = COEFormBO.Get(this._masterPage.PageFormGroupID).COEFormGroup;

                _documentCOEFormGroup = new COEFormGroup();

                _documentCOEFormGroup.ID = "SubmitDocument";

                viewDocInformationHolder.Controls.Clear();
                viewDocInformationHolder.Controls.Add(_documentCOEFormGroup);

                _documentCOEFormGroup.FormGroupDescription = coeFormCompoundHolder;
                _documentCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;
                _documentCOEFormGroup.DisplayCulture = this.DisplayCulture;

                viewDocInformationHolder.Visible = true;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception.Message, false);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        private void SetPageMode(PageState mode, string messageToDisplay)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            PageState previousState = CurrentPageState;
            CurrentPageState = mode;

            Document documentRecord = (Document)Session[Constants.DocumentObject_Session];


            switch (mode)
            {
                case PageState.AddDocument:
                    _documentCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Add;
                    _documentCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;
                    this.ClearFormButton.Visible = true;
                    this.BackButton.Visible = false;
                    break;
                case PageState.EditDocument:
                    _documentCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.Edit;
                    _documentCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;


                    this.SubmitButton.Visible = true;
                    this.ClearFormButton.Visible = true;
                    this.CancelButton.Visible = true;
                    this.DoneButton.Visible = false;
                    this.BackButton.Visible = false;
                    this.EditButton.Visible = false;


                    break;
                case PageState.ViewDocument:
                    _documentCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;
                    _documentCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;


                    this.SubmitButton.Visible = false;
                    this.CancelButton.Visible = false;
                    this.BackButton.Visible = true;
                    this.ClearFormButton.Visible = false;
                    this.DoneButton.Visible = false;
                    this.EditButton.Visible = true;

                    this.DisplayStatusMessage(String.Empty);
                    this.DisplayStatusMessage(Resource.ViewDocumentDetails_Label_Text);
                    AlreadySubmitted = true;
                    break;
                case PageState.End:
                    _documentCOEFormGroup.CurrentDisplayMode = FormGroup.DisplayMode.View;
                    _documentCOEFormGroup.CurrentDisplayGroup = FormGroup.CurrentFormEnum.DetailForm;


                    this.SubmitButton.Visible = false;
                    this.CancelButton.Visible = false;
                    this.BackButton.Visible = false;
                    this.ClearFormButton.Visible = false;
                    this.DoneButton.Visible = true;
                    this.EditButton.Visible = true;

                    this.DisplayStatusMessage(String.Empty);
                    if (DocumentRecord.ID > 0)
                        this.ShowConfirmationMessage(string.Format(Resource.SubmittedDocument_Label_Text,
                                documentRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                                                                     GUIShellTypes.IdsPaddingCharacter)));
                    this.DisplayStatusMessage(Resource.ViewDocumentDetails_Label_Text);
                    break;


            } // End Switch


            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        private Boolean isEditable(Document docObject)
        {
            return docObject.IsEditable;
        }


        /// <summary>
        /// This method handles all the logic after a Submit Button was clicked
        /// </summary>
        private void SubmitDocument()
        {
            //tracing
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            _documentCOEFormGroup.Update();

            if (this.DocumentRecord.IsValid)
            {
                if (!AlreadySubmitted)// for new document
                {
                    FileStream fls;
                    fls = new FileStream(this.DocumentRecord.Location, FileMode.Open, FileAccess.Read);
                    byte[] blob = new byte[fls.Length];
                    fls.Read(blob, 0, System.Convert.ToInt32(fls.Length));
                    fls.Close();
                    this.DocumentRecord.BinaryContent = blob;
                    this.DocumentRecord.Name = this.DocumentRecord.Location.Substring(this.DocumentRecord.Location.LastIndexOf('\\') + 1);
                    this.DocumentRecord = Document.InsertNewDocument(this.DocumentRecord);
                }
                else// update exisitng document
                {
                    this.DocumentRecord = Document.UpdateDocument(this.DocumentRecord);
                }
                this.AlreadySubmitted = true;
                DisplayDocument();


                this.SetPageMode(PageState.End, string.Format(Resource.SubmittedDocument_Label_Text,
                                 this.DocumentRecord.ID.ToString().PadLeft(GUIShellTypes.CharactersOfIdNumbers,
                    GUIShellTypes.IdsPaddingCharacter)));
            }
            //tracing
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
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
        /// Method to display confirmation messages in the top of the page (MessagesAreaUC)
        /// </summary>
        /// <param name="messageToDisplay">The text to display</param>
        private void ShowConfirmationMessage(string messageToDisplay)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            if (!string.IsNullOrEmpty(messageToDisplay))
            {
                this.MessagesAreaUserControl.AreaText = messageToDisplay;
                this.MessagesAreaUserControl.Visible = true;
                this.MessagesAreaRow.Visible = true;
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        private void HideConfirmationMessage()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            if (MessagesAreaRow.Visible)
            {
                this.MessagesAreaUserControl.AreaText = string.Empty;
                this.MessagesAreaUserControl.Visible = false;
                this.MessagesAreaRow.Visible = false;
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        private bool RestoreSavedObject()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod,
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

                        Document record = Document.GetNewDocument();
                        record.InitializeFromXml(genericStorageBO.COEGenericObject, true, false);

                        this.DocumentRecord = record;

                        if (Request[Constants.CurrentPageState_UrlParameter] != null)
                        {
                            if (Enum.IsDefined(typeof(PageState), Request[Constants.CurrentPageState_UrlParameter].ToString()))
                            {
                                this.SetPageMode((PageState)Enum.Parse(typeof(PageState), Request[Constants.CurrentPageState_UrlParameter].ToString(), true), string.Empty);
                            }
                        }
                        else
                        {

                        }


                        if (Page.PreviousPage != null)
                        {
                            if (Page.PreviousPage.ToString().ToUpper().Equals(GUIShellTypes.SavedCompoundPage))
                            {
                                string descriptionSummary = String.Empty;
                                if (((COEGenericObjectStorageBO)genericStorageBO).Name.Length > GUIShellTypes.DescriptionFieldSummaryLenght)
                                    descriptionSummary = ((COEGenericObjectStorageBO)genericStorageBO).Name.Substring(0, GUIShellTypes.DescriptionFieldSummaryLenght).ToString() + "...";
                                else
                                    descriptionSummary = ((COEGenericObjectStorageBO)genericStorageBO).Name.ToString();

                                this.ShowConfirmationMessage(string.Format(Resource.SavedDocument_Label_Text, descriptionSummary));
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

                    return true;
                }

                return false;

            }
            catch (Exception exception)
            {
                if (ExceptionPolicy.HandleException(exception, Constants.DOC_GUI_POLICY))
                    throw;
                else
                    _masterPage.DisplayErrorMessage(exception.Message, false);

                return false;
            }
            finally
            {

                Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod,
                                           MethodBase.GetCurrentMethod().Name);
            }
        }

        private bool RestoreSessionObject()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod,
                                    MethodBase.GetCurrentMethod().Name);

            try
            {
                if (Request[Resource.RestoreSessionObject_URLParameter] != null && Request[Resource.RestoreSessionObject_URLParameter] == bool.TrueString)
                {
                    return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                if (ExceptionPolicy.HandleException(exception, Constants.DOC_GUI_POLICY))
                    throw;
                else
                    _masterPage.DisplayErrorMessage(exception.Message, false);

                return false;
            }
            finally
            {
                Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod,
                                           MethodBase.GetCurrentMethod().Name);
            }

        }

        private void LeavePage()
        {
            try
            {
                Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

                if (_masterPage.PreviousPage.AbsolutePath.Contains(Resource.SubmitNewDocument_URL.Substring(Resource.SubmitNewDocument_URL.LastIndexOf("/"))))
                    Server.Transfer(_masterPage.PreviousPage.PathAndQuery, false);
                else if (_masterPage.PreviousPage.AbsolutePath.Contains(Resource.SearchDocuments_URL.Substring(Resource.SearchDocuments_URL.LastIndexOf("/"))))
                    Server.Transfer(Resource.SearchDocuments_URL + "?Caller=VM");
                else
                    Server.Transfer(Resource.Home_URL);
            }
            finally
            {
               Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void DisplayDocument()
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            if (AlreadySubmitted)//Display in ViewMode
                this.SetPageMode(PageState.End, string.Empty);
            else
            {

            }
            _documentCOEFormGroup.DataBind();
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Method to check if this particular Component is Editable
        /// </summary>
        /// <param name="currentComponent">Component to check</param>
        /// <returns>Boolean indicating if it is editable</returns>
        /// <remarks>Note already submitted documents cannot be edited</remarks>
        private bool IsEditable(Document currentDoc)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);
            bool retVal = true;

            retVal = currentDoc.IsEditable;
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
            return retVal;

        }

        #endregion

        #region CSLA Datasources Events


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


        protected void documentCslaDataSource_SelectObject(object sender, Csla.Web.SelectObjectArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

            try
            {
                e.BusinessObject = DocumentRecord;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception.Message, false);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        protected void documentCslaDataSource_UpdateObject(object sender, Csla.Web.UpdateObjectArgs e)
        {
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod,
                                       MethodBase.GetCurrentMethod().Name);

            try
            {
                //First we have to map those control that support to create "virtual" objects client side, so then we can bind the values easily.

                COEDataMapper.Map(e.Values, this.DocumentRecord);
                e.RowsAffected = 1;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception.Message, true);
            }
            Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod,
                                       MethodBase.GetCurrentMethod().Name);
        }

        #endregion
    }
}
