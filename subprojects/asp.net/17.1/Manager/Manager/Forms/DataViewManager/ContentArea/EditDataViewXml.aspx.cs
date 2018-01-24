using System;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COEDataViewService;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using System.Collections.Generic;
using Infragistics.WebUI.UltraWebNavigator;
using Resources;

public partial class Forms_DataViewManager_ContentArea_EditDataViewXml : GUIShellPage
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

    /// <summary>
    /// Current selected Node.
    /// </summary>
    private string SelectedNode
    {
        get
        {
            string retVal = String.Empty;
            if (ViewState[Constants.SelectedNode] != null)
            {
                if (ViewState[Constants.SelectedNode].ToString() != _rootNode)
                    retVal = ViewState[Constants.SelectedNode].ToString();
            }
            return retVal;
        }
        set
        {
            if (ViewState[Constants.SelectedNode] != null)
            {
                if (ViewState[Constants.SelectedNode].ToString() != value)
                    ViewState[Constants.SelectedNode] = value;
            }
            else
                ViewState[Constants.SelectedNode] = value;
        }
    }

    /// <summary>
    /// Current selected NodeID.
    /// </summary>
    public int SelectedNodeID
    {
        get
        {
            int retVal = int.MinValue;
            retVal = this.SelectedNode != String.Empty ? Convert.ToInt32(Utilities.GetParamInDataKey(this.SelectedNode, Constants.DataKeysParam.ID)) : int.MinValue;
            return retVal;
        }
    }

    /// <summary>
    /// Current DataViewBOList. Use this for easier access
    /// </summary>
    private COEDataViewBOList DataViewBOList
    {
        get
        {
            return Session[Constants.COEDataViewBOList] != null ? (COEDataViewBOList)Session[Constants.COEDataViewBOList] : null;
        }
        set
        {
            if (Session[Constants.COEDataViewBOList] != null && value != null)
                Session[Constants.COEDataViewBOList] = value;
        }
    }

    #endregion

    #region Constants
    private const string _rootNode = "rootNode";
    private Constants.SortCriterias _defaultSortCriteria = Constants.SortCriterias.Database_ASC;
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
            SetEditxmlControlAttributes(true);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnInit(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.EditXmlButton.Click += new EventHandler(EditXmlButton_Click);
        base.OnInit(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.BindNodeDetails(this.SelectedNodeID);
        SetEditxmlControlAttributes(true);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnPreRender(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        base.OnPreRender(e);
        Page.ClientScript.RegisterStartupScript(typeof(Forms_DataViewManager_ContentArea_EditDataViewXml), "copytoclipboard", @"
            function copyToClipboard() {
                if(!window.clipboardData.setData('Text', document.getElementById('" + this.DataViewXmlText.ClientID + @"').value)) {
                     alert('" + Resource.ErrorTextNotCoppiedToClipboard_Alert_Text + @"');
                    return false;
                }
                return true;
            }

            function pasteFromClipboard() {
                var clipBoardData = window.clipboardData.getData('Text');
                if(!clipBoardData) {
                    alert('" + Resource.ErrorTextNotCoppiedFromClipboard_Alert_Text + @"');
                    return false;
                }
                
                document.getElementById('" + this.DataViewXmlText.ClientID + @"').value = clipBoardData;
                
                return true;
            }", true);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void EditXmlButton_Click(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        SetEditxmlControlAttributes(false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void SaveXmlButton_Click(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        int selectedDV = this.GetUCSelectedNode();
        COEDataViewBO dv = COEDataViewBO.Get(selectedDV, true);
        dv.COEDataView.GetFromXML(this.DataViewXmlText.Text);        
        dv.DataViewManager = COEDataViewManagerBO.NewManager(dv.COEDataView);
        this.Master.SetDataViewBO(dv);
        ShowExistingDataViews();
        SetEditxmlControlAttributes(true);
        DvDetails.Visible = false;
        tblTreeview.Visible = true;
        Server.Transfer("ValidationSummary.aspx", false);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void BackButton_Click(object sender, EventArgs e)
    {
        this.SetControlsAttributtes();
        this.ShowExistingDataViews();
        SetEditxmlControlAttributes(true);

        DvDetails.Visible = false;
        tblTreeview.Visible = true;
    }

    protected void DataViewsTree_NodeClicked(object sender, WebTreeNodeEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (e.Node.DataKey != null)
        {
            if (Utilities.IsActionNode(e.Node.DataKey.ToString()))
            {
                this.SelectedNode = e.Node.DataKey.ToString();
                this.BindNodeDetails(this.SelectedNodeID);
                DvDetails.Visible = true;
                tblTreeview.Visible = false;
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void SortByWebMenu_MenuItemClicked(object sender, WebMenuItemEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        //Coverity Fixes: CBOE-313 :CID-11767
        object theObject = e.Item.DataKey;
        if (theObject  != null)
            this.SortDataViewBO((Constants.SortCriterias)Enum.Parse(typeof(Constants.SortCriterias), theObject.ToString()));
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
    /// Method to get the selected Node in the UC tree.
    /// </summary>
    /// <returns>The ID of the selected node</returns>
    private int GetUCSelectedNode()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        int retVal = int.MinValue;
        retVal = this.SelectedNodeID;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
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
            this.DataBind(dataViews);
        }
        else
        {
            //No Dataview present in DB
            Constants.MessagesCode messageCode = Constants.MessagesCode.None;
            messageCode = Constants.MessagesCode.NoDataViews;
            this.Master.DisplayMessagesPage(messageCode, GUIShellTypes.MessagesButtonType.Back);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void SetEditxmlControlAttributes(bool visible)
    {
        DataViewXmlText.ReadOnly = visible;
        EditXmlButton.Visible = visible;
        BackButton.Visible = visible;
        SaveXmlButton.Visible = !visible;
        CancelButton.Visible = !visible;
        PasteFromClipBoardButton.Visible = !visible;
        CopyToClipBoardButton.Visible = !visible;

    }

    /// <summary>
    /// Method to bind with a DataViewList
    /// </summary>
    /// <param name="dataViewsBOList">A DataViewList dataSource</param>
    private void DataBind(COEDataViewBOList dataViewsBOList)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.DataViewBOList = dataViewsBOList;
        this.ExistingDataViewsUltraWebTree.Nodes.Clear();
        this.ExistingDataViewsUltraWebTree.SelectedNode = null;
        List<Node> nodeList = this.CreateDataSource(dataViewsBOList, _defaultSortCriteria);
        foreach (Node node in nodeList)
            this.ExistingDataViewsUltraWebTree.Nodes.Add(node);
        this.BindNodeDetails(int.MinValue);
        this.SetSelectedSort(_defaultSortCriteria.ToString());
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to display the node Details in the right table.
    /// </summary>
    /// <param name="selectedNodeId">Node id to display details</param>
    private void BindNodeDetails(int selectedNodeId)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (this.ExistingDataViewsUltraWebTree.Nodes.Count > 0)
        {
            if (selectedNodeId != int.MinValue)
            {
                COEDataViewBO selectedDataViewBO = this.GetSelectedDataViewBO(selectedNodeId);
                if (selectedDataViewBO != null)
                {
                    this.DisableDetailsTextBox(false);                    
                    this.DisplayNameTextBoxWithPopUp.Text = selectedDataViewBO.Name;
                    this.DescriptionTextBoxWithPopUp.Text = selectedDataViewBO.Description;
                    this.UserTextBoxWithPopUp.Text = selectedDataViewBO.UserName;
                    this.FormGroupTextBoxWithPopUp.Text = selectedDataViewBO.FormGroup.ToString();
                    this.DateCreatedTextBoxWithPopUp.Text = selectedDataViewBO.DateCreated.ToString(Resources.Resource.DateFormat);
                    this.DataViewXmlText.Text = selectedDataViewBO.COEDataView.ToString();
                    this.DataViewXmlText.Visible = true;
                }
                else
                {
                    //TODO: If we cannot find the obj that we are looking for, do something (Exception, Warning, etc)
                }
            }
            else
            {
                this.DisableDetailsTextBox(true);
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Create a datasource with a selected criteria for the Webtree
    /// </summary>
    /// <param name="dataViewsBOList">List of DataViews to display</param>
    /// <param name="criteria">Criteria to order the source of info</param>
    /// <returns>A list of nodes that will show the info in the tree</returns>
    private List<Node> CreateDataSource(COEDataViewBOList dataViewsBOList, Constants.SortCriterias criteria)
    {
        List<Node> nodes = new List<Node>();
        List<string> databasesList = new List<string>();
        Csla.SortedBindingList<COEDataViewBO> sortedList;
        string sortProperty = "DatabaseName";
        string tempDBName = String.Empty;
        switch (criteria)
        {
            case Constants.SortCriterias.Database_ASC:
                sortProperty = "DatabaseName";

                break;
            case Constants.SortCriterias.Name_ASC:
                sortProperty = "Name";
                break;
            case Constants.SortCriterias.ID_ASC:
                sortProperty = "ID";
                break;
        }

        sortedList = new Csla.SortedBindingList<COEDataViewBO>(dataViewsBOList);
        sortedList.ApplySort(sortProperty, System.ComponentModel.ListSortDirection.Ascending);

        if (criteria == Constants.SortCriterias.Database_ASC)
        {

            foreach (COEDataViewBO dataViewBO in sortedList)
            {
                if (!databasesList.Contains(dataViewBO.DatabaseName))
                {
                    //Filter by DataBase
                    List<COEDataViewBO> list = dataViewsBOList.GetByDatabaseName(dataViewBO.DatabaseName);
                    Node node = Utilities.CreateTreeStructure(DataViewNode.NewDataViewNode(list, criteria));
                    node.CheckBox = CheckBoxes.False;
                    nodes.Add(node);
                    databasesList.Add(dataViewBO.DatabaseName);
                }
            }
        }
        else
        {
            foreach (COEDataViewBO dataViewBO in sortedList)
            {
                Node node = Utilities.CreateTreeStructure(DataViewNode.NewDataViewNode(dataViewBO));
                node.CheckBox = CheckBoxes.False;
                nodes.Add(node);
            }
        }

        return nodes;
    }

    /// <summary>
    /// Method to set the selected sort criteria in a label
    /// </summary>
    /// <param name="selectedCriteria">The selected criteria</param>
    private void SetSelectedSort(string selectedCriteria)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (!string.IsNullOrEmpty(selectedCriteria))
            this.SelectedSortLabel.Text = selectedCriteria;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Finds a COEDataViewBO given an ID
    /// </summary>
    /// <param name="selectedDataViewBOID">Selected dataVi</param>
    /// <returns>Found DataViewBO or null</returns>
    /// <remarks>This can be moved to the object it self, for now I'm trying to avoid touching the objects (just the Manager and childs) for compatibility issues</remarks>
    private COEDataViewBO GetSelectedDataViewBO(int dataViewBOID)
    {
        COEDataViewBO retVal = null;
        if (this.DataViewBOList != null)
        {
            Csla.FilteredBindingList<COEDataViewBO> filteredList = new Csla.FilteredBindingList<COEDataViewBO>(this.DataViewBOList);
            filteredList.ApplyFilter("ID", dataViewBOID);
            if (filteredList.Count > 0)
                retVal = filteredList[0];
        }
        return retVal;
    }

    /// <summary>
    /// Method to change the status of the table. For instance, if the rootNode was clicked, display an empty table.
    /// </summary>
    /// <param name="disable">Status of the table</param>
    private void DisableDetailsTextBox(bool disable)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.DataViewTableDetails.Visible = !disable;
        this.DataViewTableDetails.Disabled = disable;
        this.DataViewXmlDetails.Visible = !disable;
        this.DataViewXmlDetails.Disabled = disable;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to chose a method of sorting
    /// </summary>
    /// <param name="criteria">Criteria to follow</param>
    private void SortDataViewBO(Constants.SortCriterias criteria)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.ExistingDataViewsUltraWebTree.Nodes.Clear();

        //Coverity Fixes: CBOE-313 : CID-13146
        COEDataViewBOList coeDataViewBOList = this.DataViewBOList;
        if (coeDataViewBOList != null)
        {
            List<Node> nodeList = this.CreateDataSource(coeDataViewBOList, criteria);
            foreach (Node node in nodeList)
                this.ExistingDataViewsUltraWebTree.Nodes.Add(node);
        }
        this.SetSelectedSort(criteria.ToString());
        //Rebind Schema Details Table.
        this.BindNodeDetails(this.SelectedNodeID);
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
        //this.NextImageButton.ButtonText = Resources.Resource.OK_Button_Text;       
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion
}

