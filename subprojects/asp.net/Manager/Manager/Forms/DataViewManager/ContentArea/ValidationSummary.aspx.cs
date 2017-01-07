using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Web.UI;

using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.GUIShell;

using Manager.Code;

public partial class Forms_DataViewManger_ContentArea_ValidationSummary : GUIShellPage
{
    #region Properties

    private string PreviousPageURL
    {
        get
        {
            return ViewState["PreviousPageURL"] != null ? (string)ViewState["PreviousPageURL"] : string.Empty;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
                ViewState["PreviousPageURL"] = value;
        }
    }
    #endregion

    #region Events Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {
            this.DisplayValidationSummary();
            this.SetControlsAttributtes();
        }
        if (Page.PreviousPage != null)
        {
            this.PreviousPageURL = Page.PreviousPage.AppRelativeVirtualPath;
            if (!string.IsNullOrEmpty(this.Request.QueryString[Constants.ParamCaller]))
                this.PreviousPageURL += "?" + Constants.ParamCaller + "=" + this.Request.QueryString[Constants.ParamCaller];
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        #region Button Event Handlers
        this.CancelImageButton.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
        this.SubmitImageButton.ButtonClicked += new EventHandler<EventArgs>(SubmitImageButton_ButtonClicked);
        this.CancelImageButton1.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
        this.SubmitImageButton1.ButtonClicked += new EventHandler<EventArgs>(SubmitImageButton_ButtonClicked);
        #endregion
        base.OnInit(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void SubmitImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (Page.IsValid)
            this.SubmitDataView();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void BackImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Server.Transfer("Security.aspx", true);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void CancelImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string transferUrl = this.PreviousPageURL;
        if (string.IsNullOrEmpty(transferUrl))
            transferUrl = "DataviewBoard.aspx";
        Server.Transfer(transferUrl, false);
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
        this.Master.SetPageTitle(Resources.Resource.ValidationSummary_Page_Title);
        //Read the isValid property in the UC to enable or not the Submit button.
        if (this.ValidationSummaryUserControl != null)
            this.SubmitImageButton.Enabled = this.SubmitImageButton1.Enabled = this.ValidationSummaryUserControl.IsValid;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region Methods

    private bool UnPublishSchema(string databaseName)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        bool retVal = false;
        if (!string.IsNullOrEmpty(databaseName))
        {
            //Get the password to send as an argument to publish.
            //string password = this.PasswordTextBox.Text;
            InstanceData mainInstance = ConfigurationUtilities.GetMainInstance();
            var instanceName = databaseName.Contains(".") ? databaseName.Split(new char[] { '.' })[0] : mainInstance.Name;
            COEDatabaseBO database = COEDatabaseBOList.GetList(true, instanceName).GetDatabase(databaseName);
            COEDatabaseBO unPublishedDataBase;
            if (database != null)
            {
                try
                {
                    unPublishedDataBase = database.UnPublish();
                    if (unPublishedDataBase != null) //If it fails, keep it on the list with no changes.
                    {
                        retVal = true;
                    }
                }
                catch { }
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
    }

    private void PublishAndUnpublishSchemas()
    {
        // Publish the schemas.
        var schemasOnPublishing = Session[Constants.COESchemasOnPublishing] as Dictionary<string, COEDatabaseBO>;

        if (schemasOnPublishing != null)
        {
            foreach (var kv in schemasOnPublishing)
            {
                kv.Value.PublishOnlyWithoutValidation(kv.Key);
            }
        }

        // Unpublish the schemas.
        var schemasOnRemoving = Session[Constants.COESchemasOnRemoving] as Collection<string>;

        if (schemasOnRemoving != null)
        {
            foreach (var schema in schemasOnRemoving)
            {
                UnPublishSchema(schema);
            }
        }
    }

    private void SubmitDataView()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        COEDataViewBO dataViewBO = this.Master.GetDataViewBO();

        if (dataViewBO != null)
        {
            bool errors = false;
            bool dataviewSaved = false;

            // Record the id value before saving.
            int oldDataviewId = dataViewBO.ID;
            COEDataViewBO oldDataviewBO = null;

            try
            {
                // when save the master dataview, the publishing schemas on session also need to be saved into database.
                if (dataViewBO.ID == Constants.MasterSchemaDataViewID)
                {
                    PublishAndUnpublishSchemas();
                }

                dataViewBO.SaveFromDataViewManager();
                dataviewSaved = true;

                if (dataViewBO.ID != Constants.MasterSchemaDataViewID)
                {
                    //Delete existing dataview
                    var publishingMsg = SpotfireServiceClient.DeleteDataView(dataViewBO);
                    if (!string.IsNullOrEmpty(publishingMsg))
                    {
                        throw new Exception(publishingMsg);
                    }

                    // publishing dataview to spotfire.
                    publishingMsg = SpotfireServiceClient.PublishDataView(dataViewBO);
                    if (!string.IsNullOrEmpty(publishingMsg))
                    {
                        throw new Exception(publishingMsg);
                    }
                }

                // Cache dataviewBO
                Session["TempDataViewBO"] = dataViewBO;
                // Clean Session variables.
                Session.Remove(Constants.COEDataViewBO);
                Session.Remove(Constants.COESchemasOnPublishing);
                Session.Remove(Constants.COESchemasOnRemoving);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                errors = true;
                if (ex is Csla.DataPortalException)
                    message = ((Csla.DataPortalException)ex).BusinessException.Message;

                // Rollback the changes.
                try
                {
                    if (dataviewSaved)
                    {
                        // Resave the old value for dataview if this is modification.
                        if (oldDataviewId >= 0)
                        {
                            // If dataview id is -1, this is new, otherwise this is updating.
                            if (oldDataviewId != -1)
                            {
                                COEDataViewBO.ForceLoad = true;
                                oldDataviewBO = COEDataViewBO.Get(oldDataviewId, true);
                            }

                            oldDataviewBO.SaveFromDataViewManager();
                        }
                        else
                        {
                            // Delete the dataview.
                            COEDataViewBO.Delete(dataViewBO.ID);
                            // Revert the id generated from database sequence. actually it should be -1
                            dataViewBO.ID = oldDataviewId;
                        }
                    }
                    else
                    {
                        dataViewBO.ID = oldDataviewId;
                    }

                    message += " Dataview changes rollback.";
                }
                catch
                {
                    message += " Dataview changes rollback failed";
                }

                this.DisplayErrorMessage(message);
            }

            if (!errors)
            {
                Session["MasterSchema"] = null;
                this.Master.DisplayMessagesPage(Constants.MessagesCode.SubmittedDataView, GUIShellTypes.MessagesButtonType.Close);
            }
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Go to home page
    /// </summary>
    private void GoHome()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Master.ClearAllCurrentDVSessionVars();
        Server.Transfer(Constants.PublicContentAreaFolder + "Home.aspx", false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Bind dataview datasource to the UC in charge to display the information.
    /// </summary>
    private void DisplayValidationSummary()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        COEDataViewBO dataViewBO = this.Master.GetDataViewBO();
        if (this.ValidationSummaryUserControl != null && dataViewBO != null)
        {
            this.ValidationSummaryUserControl.DataBind(dataViewBO);
        }
        else
        {
            //handle a scenario which refresh page during submiting
            if (this.ValidationSummaryUserControl != null && Session["TempDataViewBO"] != null)
            {
                this.ValidationSummaryUserControl.DataBind((COEDataViewBO)Session["TempDataViewBO"]);
            }
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void DisplayErrorMessage(string message)
    {
        UpdatePanel errorAreaUpatePanel = (UpdatePanel)((Forms_DataViewManger_ContentArea_ValidationSummary)this.Page).Master.FindControlInPage("ErrorAreaUpdatePanel");
        if (string.IsNullOrEmpty(message))
            ((Forms_Public_UserControls_ErrorArea)errorAreaUpatePanel.FindControl("ErrorAreaUserControl")).Visible = false;
        else
        {
            ((Forms_Public_UserControls_ErrorArea)errorAreaUpatePanel.FindControl("ErrorAreaUserControl")).Text = message;
            ((Forms_Public_UserControls_ErrorArea)errorAreaUpatePanel.FindControl("ErrorAreaUserControl")).Visible = true;
        }
        errorAreaUpatePanel.Update();
    }
    #endregion
}
