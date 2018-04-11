using System;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COEDataViewService;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using System.Collections.Generic;

public partial class Forms_DataViewManager_ContentArea_NewDataView : GUIShellPage
{
    #region Properties

    private Constants.PageStates Action
    {
        get
        {
            return ViewState[Constants.Action] == null ? Constants.PageStates.Undefined : (Constants.PageStates)Enum.Parse(typeof(Constants.PageStates), ViewState[Constants.Action].ToString());
        }
        set
        {
            ViewState[Constants.Action] = value;
        }
    }

    #endregion

    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {
            this.CheckURLParams();
            this.SetControlsAttributtes();
            this.ShowExistingDataViews();
			//CBOE-708 : Object reference error is displayed ... when session timeout occurred. ASV 260413.
			//Added condition to check if querystring contains the SelectedDataViewID attribute.
			//This will be the case if the session ends while editing dataview.
			//if yes then redirect to next page
            if (this.Page.ClientQueryString.Contains(Constants.SelectedDataViewID))
            	GoToNextPage();
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        #region Buttons Events
        this.CancelImageButton.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
        this.NextImageButton.ButtonClicked += new EventHandler<EventArgs>(NextImageButton_ButtonClicked);
        #endregion
        base.OnInit(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void NextImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.GoToNextPage();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.GoHome();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void CancelImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.GoHome();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnPreRender(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //this.CheckToDisplayNextButton();
        base.OnPreRender(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region Methods

    private void CheckURLParams()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (Request[Constants.Action] != null)
        {
            if (Enum.IsDefined(typeof(Constants.PageStates), Request[Constants.Action].ToString()))
                this.Action = (Constants.PageStates)Enum.Parse(typeof(Constants.PageStates), Request[Constants.Action].ToString());
        }
        else
        {
            this.Master.DisplayMessagesPage(Constants.MessagesCode.InvalidDataView, GUIShellTypes.MessagesButtonType.None);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Go to the next page in the flow.
    /// </summary>
    private void GoToNextPage()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (this.DataViewSummaryUserControl!= null)
        {
            if(this.DataViewSummaryUserControl.SelectedNodeID > 0) //Hide Master schema, that's why > 0
            {
                //Clean unnecesary session vars in following pages.
                this.CleanSessionVars();
                int selectedDV = this.GetUCSelectedNode();
    
                // temp, ForceLoad is true so that dataview information will retrieved from database, not from cache while opening Edit Existing dataview
                COEDataViewBO.ForceLoad = true;
                this.Master.SetDataViewBO(COEDataViewBO.Get(selectedDV, true));
                COEDataViewBO.ForceLoad = false;

                //CBOE-708 : Object reference error is displayed ... when session timeout occurred. ASV 260413.
				//Add selected dataview, action and/or paramcaller to every redirect url so that it will be
				//used in return url if session ends in between
				string url = (this.Action == Constants.PageStates.Edit_DV) ? "DataviewBoard.aspx?" : "EnterNameDescription.aspx?";
                url += Constants.SelectedDataViewID + "=" + selectedDV + "&" + Constants.Action + "=" + this.Action.ToString();
                if (this.Action == Constants.PageStates.Edit_DV && this.Page.ClientQueryString.Contains(Constants.ParamCaller))
                    url += "&" + Constants.ParamCaller + "=" + Page.Request.QueryString.Get(Constants.ParamCaller);
                Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
                Server.Transfer(url, false);
            }
            else
            {
                this.ErrorAreaUserControl.Text = Resources.Resource.NoDataViewSelected_Label_Text;
                this.ErrorAreaUserControl.Visible = true;
            }
        }
    }
    /// <summary>
    /// Method to clean heavy session vars unnecesaries in following pages.
    /// </summary>
    private void CleanSessionVars()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Master.ClearAllCurrentDVSessionVars();
        if (Session[Constants.DataViewFrom] != null) 
            Session.Remove(Constants.DataViewFrom);
        if (Session[Constants.COEDataViewBOList] != null) 
            Session.Remove(Constants.COEDataViewBOList);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Redirect to Home Page.
    /// </summary>
    private void GoHome()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Master.ClearAllCurrentDVSessionVars();
        Server.Transfer(Constants.PublicContentAreaFolder + "Home.aspx", false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to get the selected Node in the UC tree.
    /// </summary>
    /// <returns>The ID of the selected node</returns>
    private int GetUCSelectedNode()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        int retVal = int.MinValue;
        if (this.DataViewSummaryUserControl != null)
        {
            retVal = this.DataViewSummaryUserControl.SelectedNodeID;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
    }

    /// <summary>
    /// Method to check to display the next button (just in case a DV from an existing DV)
    /// </summary>
    private void CheckToDisplayNextButton()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (this.DataViewSummaryUserControl != null)
            this.NextImageButton.Enabled = this.DataViewSummaryUserControl.SelectedNodeID > 0 ? true : false;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to show the exising DVs.
    /// </summary>
    private void ShowExistingDataViews()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        COEDataViewBOList dataViews = COEDataViewBOList.GetDataViewListAndNoMaster();
        if (dataViews.Count > 0)
        {
            Session[Constants.COEDataViewBOList] = dataViews;
            if (this.DataViewSummaryUserControl != null)
                this.DataViewSummaryUserControl.DataBind(dataViews);
        }
        else
        {
            //No Dataview present in DB
            Constants.MessagesCode messageCode = Constants.MessagesCode.None;
            messageCode = Constants.MessagesCode.NoDataViews;
            //switch(this.Action)
            //{
            //    case Constants.PageStates.Clone_DV:
            //        messageCode = Constants.MessagesCode.NoDataViews;
            //        break;
            //    case Constants.PageStates.Edit_DV:
            //        messageCode = Constants.MessagesCode.NoMasterDataView;
            //        break;
            //}
            
            this.Master.DisplayMessagesPage(messageCode, GUIShellTypes.MessagesButtonType.Close);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region GUIShell Methods

    /// <summary>
    /// This method sets all the controls attributtes as Text, etc...
    /// </summary>
    protected override void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Master.SetPageTitle(Resources.Resource.NewDataView_Page_Title);
        this.NextImageButton.ButtonText = Resources.Resource.OK_Button_Text;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion
}
