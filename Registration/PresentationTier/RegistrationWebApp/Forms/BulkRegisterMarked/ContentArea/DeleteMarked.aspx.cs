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
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Services.Types;
using Resources;
using System.Reflection;
using CambridgeSoft.COE.Registration.Services;

namespace RegistrationWebApp.Forms.BulkRegisterMarked.ContentArea
{
    public partial class DeleteMarked : GUIShellPage
    {
        #region GUIShell Variables

        GUIShellMaster _masterPage = null;
        #endregion

        #region Properties

        private RegistryRecordList Registries
        {
            get
            {
                if (ViewState["Registries"] == null)
                {

                    ViewState["Registries"] = RegistryRecordList.GetRegistryRecordList(SavedHitlistID, this.IsTemporal);


                }

                return (RegistryRecordList)ViewState["Registries"];
            }
            set
            {
                ViewState["Registries"] = value;
            }
        }

        private int LogId
        {
            get {
                if(ViewState["LogId"] == null)
                    ViewState["LogId"] = 0;

                return (int) ViewState["LogId"];
            }
            set {
                ViewState["LogId"] = value;
            }
        }

        private int SavedHitlistID
        {
            get
            {
                try
                {
                    if (ViewState["saveHitlistID"] == null)
                        ViewState["saveHitlistID"] = Request.QueryString["COESavedHitListID"];

                    return int.Parse(ViewState["saveHitlistID"].ToString());
                }
                catch (Exception)
                {
                    throw new Csla.Validation.ValidationException(Resource.InvalidOrNoSavedHitListID_Exception);
                }
            }
            set
            {
                ViewState["saveHitlistID"] = value;
            }
        }

        private bool IsTemporal
        {
            get
            {
                if (ViewState["IsTemporal"] == null)
                    ViewState["IsTemporal"] = Request.QueryString["IsTemporal"] == null ? true : bool.Parse(Request.QueryString["IsTemporal"]);

                return (bool)ViewState["IsTemporal"];
            }
            set
            {
                ViewState["IsTemporal"] = value;
            }
        }
        #endregion

        #region Methods
        protected override void SetControlsAttributtes()
        {
            this.LogDescriptionLabel.Text = Resource.AddDescription_Label_Text;
            this.LogLinkButton.Text = Resource.ViewLog_Button_Text;
            this.AcceptButton.Text = Resource.Delete_Button_Text;
            this.ExitButton.Text = Resource.Cancel_Button_Text;
            this.ExitButton.ToolTip = Resource.ReturnToSearchList_Button_ToolTip;
            this.PageTitleLabel.Text = Resource.DeleteMarked_Page_Title;
            this.GoHomeButton.Text = Resource.Home_Button_Text;
        }

        protected enum PageState
        {
            Start,
            End,
            Log,
            Error,
        }
        protected void SetPageMode(PageState state)
        {
            switch (state)
            {
                case PageState.Start:
                    this.LogDescriptionTextBox.Visible = this.LogDescriptionLabel.Visible = false;
                    this.StatusLabel.Visible = false;
                    this.ChemBioVizFrame.Visible = false;
                    this.LogLinkButton.Visible = false;
                    break;
                case PageState.End:
                    this.LogDescriptionTextBox.Visible = this.LogDescriptionLabel.Visible = false;
                    this.StatusLabel.Visible = true;
                    this.ChemBioVizFrame.Visible = false;
                    this.ExitButton.Text = Resource.Done_Button_Text;
                    this.RegistriesCOEGridView.Visible = false;
                    this.AcceptButton.Visible = false;
                    this.LogLinkButton.Visible = false;
                    this.GoHomeButton.Visible = false;
                    break;
                case PageState.Log:
                    this.LogDescriptionTextBox.Visible = this.LogDescriptionLabel.Visible = false;
                    this.StatusLabel.Visible = false;
                    this.ChemBioVizFrame.Visible = true;
                    this.ExitButton.Text = Resource.Done_Button_Text;
                    this.RegistriesCOEGridView.Visible = false;
                    this.AcceptButton.Visible = false;
                    this.LogLinkButton.Visible = false;
                    break;
                case PageState.Error:
                    this.LogDescriptionTextBox.Visible = this.LogDescriptionLabel.Visible = false;
                    this.StatusLabel.Visible = false;
                    this.ChemBioVizFrame.Visible = false;
                    this.ExitButton.Text = Resource.Done_Button_Text;
                    this.RegistriesCOEGridView.Visible = false;
                    this.AcceptButton.Visible = false;
                    this.LogLinkButton.Visible = false;
                    break;
            }
        }

        protected void GoHomeButton_Click(object sender, EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            Server.Transfer(Resource.RegisterMarked_LeavePage_Url_Text, false);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }


        private void ShowConfirmationMessage(string messageToDisplay)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            if (!string.IsNullOrEmpty(messageToDisplay))
            {
                this.MessagesAreaUserControl.AreaText = messageToDisplay;
                this.MessagesAreaUserControl.Visible = true;
                this.MessagesAreaRow.Visible = true;
            }
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// This method sets the Page Title <Title></Title>
        /// </summary>
        protected void SetPageTitle()
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.Page.Title = Resource.DeleteMarked_Page_Title;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    SetControlsAttributtes();
                    this.SetPageTitle();
                    this.DeleteMarkedRegistries();
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
            try
            {
                RegistriesCOEGridView.LoadFromXml("<configInfo xmlns=\"COE.FormGroup\"><fieldConfig><tables><table><CSSClass>myTableClass</CSSClass><headerStyle>color: #000; background-color: rgb(255, 255, 255); font-weight: bold; font-family: Verdana; font-size: 10px;</headerStyle><Columns><Column name=\"Id\"><width>30px</width><formElement><Id>Id</Id><bindingExpression>ID</bindingExpression></formElement></Column><Column name=\"AggregatedStructure\"><formElement><Id>StructureColumn</Id><bindingExpression>StructureAggregation</bindingExpression><configInfo><fieldConfig><Height>135px</Height><!--<CSSClass>COETextBox</CSSClass>--></fieldConfig></configInfo><displayInfo><type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEChemDrawEmbedReadOnly</type></displayInfo></formElement></Column></Columns></table></tables><ReadOnly>true</ReadOnly><CSSClass>COEGridView</CSSClass><Style>margin-left:5px;margin-top:5px;margin-bottom:5px;</Style></fieldConfig></configInfo>");
                
                if (this.Master is GUIShellMaster)
                {
                    _masterPage = (GUIShellMaster)this.Master;
                    _masterPage.ShowLeftPanel = false;
                }
                base.OnInit(e);
            }
            catch (Exception exception)
            {
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void AcceptButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.DeleteMarkedRegistries();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        private void DeleteMarkedRegistries()
        {
            int logId;
            try
            {
                BulkDelete command = BulkDelete.Execute(SavedHitlistID, this.IsTemporal, this.LogDescriptionTextBox.Text);
                if (command.Result)
                {
                    if (command.FailedRecords.Length > 0)
                    {

                        this.ShowConfirmationMessage(string.Format(Resource.FollowingRecordsNotDeleted_Label_Text, string.Join(", ", command.FailedRecords)));
                    }
                    else
                        this.ShowConfirmationMessage(Resource.MarkedDeleted_Label_Text);
                    SetPageMode(PageState.End);
                }
                else
                {
                    this.ShowConfirmationMessage(Resource.NoRecordWasDeleted_Label_Text);
                    SetPageMode(PageState.End);
                }
                LogId = command.LogId;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void LogLinkButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.SetPageMode(PageState.Log);
                this.EmbedCBVForm();
            }
            catch(Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void ExitButton_Click(object sender, EventArgs e)
        {
            try
            {
                if(IsTemporal)
                    Server.Transfer(Resources.Resource.ReviewRegisterSearch_URL + "?Caller=LASTSEARCHTODELETE", false);
                else
                    Server.Transfer(Resources.Resource.ViewMixtureSearch_URL + "?Caller=LASTSEARCHTODELETE", false);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        private void EmbedCBVForm()
        {
            this.ChemBioVizFrame.Attributes["src"] = RegUtilities.GetDeleteMarkedURL() + string.Format("&SearchCriteriaId={0}&SearchCriteriaValue={1}", 1, this.LogId);
        }
        #endregion
    }
}
