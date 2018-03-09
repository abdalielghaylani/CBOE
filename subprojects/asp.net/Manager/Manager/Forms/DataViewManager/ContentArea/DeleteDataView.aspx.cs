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
using CambridgeSoft.COE.Framework.COEDataViewService;
using System.Reflection;
using Resources;

public partial class DeleteDataView : GUIShellPage
{
    #region Page Life Cycle Events
    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        base.OnInit(e);
        this.CancelImageButton.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
        this.OkImageButton.ButtonClicked += new EventHandler<EventArgs>(DeleteImageButton_ButtonClicked);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributtes();
            Csla.SortedBindingList<COEDataViewBO> sortedList = new Csla.SortedBindingList<COEDataViewBO>(COEDataViewBOList.GetDataViewListAndNoMaster());
            sortedList.ApplySort("Name", System.ComponentModel.ListSortDirection.Ascending);
            this.DataViewListDropDown.DataSource = sortedList;
            this.DataViewListDropDown.DataTextField = "Name";
            this.DataViewListDropDown.DataValueField = "ID";
            this.DataViewListDropDown.DataBind();
            this.Master.SetPageTitle(Resources.Resource.DeleteADataview_Page_Title);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region Event Handlers
    void DeleteImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        try
        {
            this.Delete();
            this.GoHome();
        }
        catch (Exception ex)
        {
            this.ErrorAreaUserControl.Text = ex.Message;
            this.ErrorAreaUserControl.Visible = true;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void CancelImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Server.Transfer(Constants.PublicContentAreaFolder + "Home.aspx", false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region Private Methods
    protected override void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.DataViewListDropDown.Items.Add(new ListItem("Select a dataview to delete", "-1"));
        this.WarningMessage.InnerHtml = Resource.DeleteDataViewWarning;
        this.HeaderDiv.InnerText = Resource.DeleteDataViewHeader;
        this.OkImageButton.ButtonText = Resource.OK_Button_Text;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Redirect to Home Page.
    /// </summary>
    private void GoHome()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Master.ClearAllCurrentDVSessionVars();
        this.Master.DisplayMessagesPage(Constants.MessagesCode.DeletedDataView, GUIShellTypes.MessagesButtonType.Home);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void Delete()
    {
        int selectedDVID = int.Parse(this.DataViewListDropDown.SelectedValue);
        if (selectedDVID > 0)
            COEDataViewBO.Delete(selectedDVID);
        else
            throw new Exception("You have not selected a dataview.");
    }
    #endregion
}
