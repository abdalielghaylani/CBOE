using System;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COEDataViewService;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using System.Collections.Generic;

public partial class Forms_DataViewManager_ContentArea_EnterNameDescription : GUIShellPage
{
    #region Properties

    private int SelectedDataViewID
    {
        get
        {
            int retVal = -1;
            if (ViewState[Constants.SelectedDataViewID] != null)
                int.TryParse(ViewState[Constants.SelectedDataViewID].ToString(), out retVal);
            if (retVal == int.MinValue)
                retVal = -1;
            return retVal;
        }
        set
        {
            ViewState[Constants.SelectedDataViewID] = value;
        }
    }

    private COEDataViewBO SelectedDataView
    {
        get
        {
            COEDataViewBO currentDV = null;
            if (Session[Constants.COEDataViewBO] != null)
                currentDV = ((COEDataViewBO)Session[Constants.COEDataViewBO]);
            return currentDV;
        }
    }

    private bool IsMasterDataView
    {
        get
        {
            //Coverity Fixes: CBOE-313 : CID-11768
            COEDataViewBO coeDataViewBO = this.SelectedDataView;
            if (coeDataViewBO != null && coeDataViewBO.ID == Constants.MasterSchemaDataViewID)
                return true;
            return false;
        }
    }

    private Constants.PageStates Action
    {
        get
        {
            return ViewState[Constants.Action] == null ? Constants.PageStates.Undefined : (Constants.PageStates)ViewState[Constants.Action];
        }
        set
        {
            ViewState[Constants.Action] = value;
        }
    }
    #endregion

    #region Page Life Cycle Events
    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.NextImageButton.ButtonClicked += new EventHandler<EventArgs>(NextImageButton_ButtonClicked);
        this.CancelImageButton.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
        this.AddModalValidationSummary(Resources.Resource.ValidationSummaryHeader_Label_Text, Resources.Resource.ValidationSummaryFooter_Label_Text);
        base.OnInit(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {
            //Jira ID: CBOE-1161 : Veritcal scroll bar 
            Session["PrimaryKeyWarning"] = false;
            this.WarningControl.Text = Resources.Resource.ErrorSelectBaseTable;
            this.WarningControl.Visible = false;

            this.CheckURLParams();

            if (this.Action == Constants.PageStates.Clone_DV)
            {
                this.Master.ClearAllCurrentDVSessionVars();
                this.SelectBaseTableUserControl.Visible = true;
            }
            else
            {
                this.SelectBaseTableUserControl.Visible = false;
            }
            this.DisplayForm();
            this.SetControlsAttributtes();
        }

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region Events Handlers
    void NextImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        bool primaryKeyValidation = false;

        //Jira ID: CBOE-1161 : Veritcal scroll bar 
        if (this.Action == Constants.PageStates.Create_DV || this.Action == Constants.PageStates.Clone_DV)
        {
            bool isValid = this.SelectBaseTableUserControl.IsValidPrimaryKey();

            if (isValid && Session["PrimaryKeyWarning"] != null && Convert.ToBoolean(Session["PrimaryKeyWarning"]) == false)
            {
                Session["PrimaryKeyWarning"] = true;
                //this.WarningControl.Visible = true;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "MyKey", "alert('" + Resources.Resource.ErrorSelectBaseTable + "');", true);
                primaryKeyValidation = true;
            }
            else
            {
                Session["PrimaryKeyWarning"] = false;    //Jira ID: CBOE-1161 : Veritcal scroll bar         
                this.WarningControl.Visible = false;
            }
        }
        if (Page.IsValid && !primaryKeyValidation)
        {
            Session["PrimaryKeyWarning"] = false;    //Jira ID: CBOE-1161 : Veritcal scroll bar 
            this.WarningControl.Visible = false;
            this.NextDataViewStep();
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void CancelImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Cancel();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region Private Methods

    private void Cancel()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (this.Action != Constants.PageStates.Clone_DV)
        {
            Server.Transfer("~/Forms/DataViewManager/ContentArea/DataviewBoard.aspx", false);
        }
        else
        {
            this.Master.ClearAllCurrentDVSessionVars();
            Server.Transfer(Constants.PublicContentAreaFolder + "Home.aspx", false);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to bind information to the form to display
    /// </summary>
    private void DisplayForm()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.BindSelectedCOEDataViewBO();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void CheckURLParams()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        int dataViewID = int.MinValue;
        if (Request[Constants.Action] != null)
        {
            if (Enum.IsDefined(typeof(Constants.PageStates), Request[Constants.Action].ToString()))
                this.Action = (Constants.PageStates)Enum.Parse(typeof(Constants.PageStates), Request[Constants.Action].ToString());
        }
        if (Request[Constants.SelectedDataViewID] != null)
        {
            if (int.TryParse(Request[Constants.SelectedDataViewID].ToString(), out dataViewID))
                this.SelectedDataViewID = dataViewID;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to get the selected DVBO from the DB given the DV id.
    /// </summary>
    private void BindSelectedCOEDataViewBO()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        COEDataViewBO dataViewBO = null;
        if (this.SelectedDataViewID > int.MinValue)
            dataViewBO = this.Master.GetDataViewBO() == null ? COEDataViewBO.Get(this.SelectedDataViewID, true) : this.Master.GetDataViewBO();

        //Coverity Fixes: CBOE-313
        if (dataViewBO != null && dataViewBO.COEDataView != null && dataViewBO.DataViewManager.Tables.Count > 0)
        {
            this.NameDescriptionUserControl.Action = this.Action;
            this.NameDescriptionUserControl.DataBind(dataViewBO);
            if (this.SelectBaseTableUserControl.Visible)
                this.SelectBaseTableUserControl.DataBind();

            this.Master.SetDataViewBO(dataViewBO);
        }
        else
        {
            if (this.SelectedDataViewID == Constants.MasterSchemaDataViewID)
                this.Master.DisplayMessagesPage(Constants.MessagesCode.NoMasterDataView, GUIShellTypes.MessagesButtonType.Home);
            else
                this.Master.DisplayMessagesPage(Constants.MessagesCode.InvalidDataView, GUIShellTypes.MessagesButtonType.Home);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Check to change the Is_Public status of the form.
    /// </summary>
    /// <param name="dataViewBO">Dataview to modify</param>
    /// <remarks>In a further step the user can select users/roles</remarks>
    private void CheckIsPublicStatus(COEDataViewBO dataViewBO)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (this.Action == Constants.PageStates.Clone_DV && dataViewBO.ID == -1)
        {
            dataViewBO.IsPublic = true;
            dataViewBO.COEAccessRights = null;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Move to the following DV creating/edit step.
    /// </summary>
    private void NextDataViewStep()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (this.NameDescriptionUserControl != null)
        {
            try
            {
                COEDataViewBO dataViewBO = this.NameDescriptionUserControl.UnBind();
                TableBO baseTable = null;
                if (this.SelectBaseTableUserControl != null && this.SelectBaseTableUserControl.Visible)
                    baseTable = this.SelectBaseTableUserControl.Unbind();
                if (baseTable != null)
                {
                    if (!dataViewBO.DataViewManager.Tables.Contains(baseTable))
                        dataViewBO.DataViewManager.Tables.Add(baseTable);

                    dataViewBO.DataViewManager.SetBaseTable(baseTable.ID);
                }
                this.Master.SetDataViewBO(dataViewBO);
                //Session["BackButton"] = false;
                //Session["PrimaryKeyWarning"] = false;    //Jira ID: CBOE-1161 : Veritcal scroll bar 
                //this.WarningControl.Visible = false;
                Server.Transfer("DataviewBoard.aspx");
            }
            catch (InvalidInputText ex)
            {
                this.Master.DisplayErrorMessage(ex.Message);
            }
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
        this.Master.SetPageTitle(Resources.Resource.EnterNameDescription_Page_Title);
        this.NextImageButton.ButtonText = this.NextImageButton.ButtonToolTip = Resources.Resource.OK_Button_Text;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion
}
