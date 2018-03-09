using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COEDataViewService;
using System.Reflection;
using Resources;
using Manager.Code;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Linq;
using Infragistics.WebUI.UltraWebGrid;

public partial class DeleteDataView : GUIShellPage
{
    #region Enums
    /// <summary>
    /// List of shown columns in the FieldGrid
    /// </summary>
    private enum ColumnKeys
    {
        ID,
        Name
    }

    #endregion

    #region Page Life Cycle Events
    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        this.InstanceDataviewsWebGrid.InitializeLayout += new InitializeLayoutEventHandler(TableFieldsUltraWebGrid_InitializeLayout);
        this.InstanceDataviewsWebGrid.JavaScriptFileName = "/COECommonResources/infragistics/20111CLR20/Scripts/ig_WebGrid.js";

        base.OnInit(e);
        this.CancelImageButton.ButtonClicked += new EventHandler<EventArgs>(CancelImageButton_ButtonClicked);
        this.OkImageButton.ButtonClicked += new EventHandler<EventArgs>(DeleteImageButton_ButtonClicked);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if(!Page.IsPostBack)
        {
            this.SetControlsAttributtes();
            BindDropDownList();
            this.Master.SetPageTitle(Resources.Resource.DeleteADataview_Page_Title);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }
    #endregion

    #region Event Handlers

    /// <summary>
    /// Ocurrs when we want to render the grid (just once in the control life cicle)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void TableFieldsUltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        this.InstanceDataviewsWebGrid.DisplayLayout.ViewType = Infragistics.WebUI.UltraWebGrid.ViewType.Flat;
        this.InstanceDataviewsWebGrid.DisplayLayout.AllowUpdateDefault = AllowUpdate.RowTemplateOnly;
        this.InstanceDataviewsWebGrid.DisplayLayout.AutoGenerateColumns = false;

        e.Layout.Bands[0].SortedColumns.Add(ColumnKeys.Name.ToString());
        e.Layout.AllowSortingDefault = AllowSorting.OnClient;
        e.Layout.SelectTypeRowDefault = SelectType.Single;

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    void DeleteImageButton_ButtonClicked(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        try
        {
            // Gets the dataview ids string from the hidden field.
            if (!string.IsNullOrEmpty(this.dataviewIdsHiddenField.Value))
            {
                // Split the dataview id from the string by seperator ','.
                var dataviewIds = this.dataviewIdsHiddenField.Value.Split(',');

                foreach (var id in dataviewIds)
                {
                    int dataviewId;

                    // Delete the dataview if it is valid integer.
                    if (int.TryParse(id, out dataviewId))
                    {
                        this.Delete(dataviewId);
                    }
                }
            }

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

    protected void InstanceDataviewsWebGrid_InitializeDataSource(object sender, Infragistics.WebUI.UltraWebGrid.UltraGridEventArgs e)
    {
        BindDataView(this.InstanceDropDownList.SelectedValue);
    }

    #endregion

    #region Private Methods
    public void BindDropDownList()
    {
        this.InstanceDropDownList.DataSource = GetInstanceToAddToDataView();
        this.InstanceDropDownList.DataBind();
    }

    protected void InstanceDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        InstanceDataviewsWebGrid_InitializeDataSource(null, null);
    }

    private void BindDataView(string dataSourceName)
    {
        var instanceData = COEConfiguration.GetInstanceData(dataSourceName);

        Csla.SortedBindingList<COEDataViewBO> sortedList = null;
        if (instanceData != null && instanceData.IsCBOEInstance)
        {
            sortedList = new Csla.SortedBindingList<COEDataViewBO>(COEDataViewBOList.GetDataViewListOfPrimaryDataSource());
        }
        else
        {
            sortedList = new Csla.SortedBindingList<COEDataViewBO>(COEDataViewBOList.GetDataViewListByInstance(dataSourceName));
        }

        this.InstanceDataviewsWebGrid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.Name.ToString()).SortIndicator = SortIndicator.Ascending;
        this.InstanceDataviewsWebGrid.DisplayLayout.Bands[0].SortedColumns.Add(ColumnKeys.Name.ToString());
        this.InstanceDataviewsWebGrid.DataSource = sortedList;
        this.InstanceDataviewsWebGrid.DataBind();

        if (sortedList.Count > 0)
        {
            this.spGrid.Attributes.Add("style", "display:block");
            this.spNoRecords.Attributes.Add("style", "display:none");
        }
        else
        {
            this.spGrid.Attributes.Add("style", "display:none");
            this.spNoRecords.Attributes.Add("style", "display:block");
        }
    }

    private IList<string> GetInstanceToAddToDataView()
    {
        var instancesList = COEConfiguration.GetAllInstancesInConfig();

        return instancesList.Select(i => i.Name).ToList();
    }

    protected override void SetControlsAttributtes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.InstanceDropDownList.Items.Add(new ListItem("Select data source", "-1"));
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

    private void Delete(int dataviewId)
    {
        if (dataviewId > 0)
        {
            var dataview = COEDataViewBO.Get(dataviewId);
         
            COEDataViewBO.Delete(dataviewId);
            SpotfireServiceClient.DeleteDataView(dataview);
        }
        else
        {
            throw new Exception("You have not selected a dataview.");
        }
    }

    #endregion
}