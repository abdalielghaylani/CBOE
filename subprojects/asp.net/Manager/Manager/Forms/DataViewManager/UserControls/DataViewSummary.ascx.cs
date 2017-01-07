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
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEDataViewService;
using Infragistics.WebUI.UltraWebNavigator;
using Infragistics.WebUI.UltraWebToolbar;
using System.Reflection;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Collections.Generic;
using System.Text;

public partial class DataViewSummary : System.Web.UI.UserControl
{
    #region Properties

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
                    retVal =  ViewState[Constants.SelectedNode].ToString();
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
            this.SetControlsAttributes();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
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
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void SortByWebMenu_MenuItemClicked(object sender, WebMenuItemEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);

        //Coverity Fixes: CBOE-313 : CID-11767,11772
        object theObject = e.Item.DataKey;
        if (theObject != null)
            this.SortDataViewBO((Constants.SortCriterias)Enum.Parse(typeof(Constants.SortCriterias), theObject.ToString()));
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Method to chose a method of sorting
    /// </summary>
    /// <param name="criteria">Criteria to follow</param>
    private void SortDataViewBO(Constants.SortCriterias criteria)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.ExistingDataViewsUltraWebTree.Nodes.Clear();
        //Coverity Fixes: CBOE-313 :CID-13147 
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
    /// This method sets all the controls attributtes as Text, etc...
    /// </summary>
    private void SetControlsAttributes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.DescriptionLabel.Text  = Resources.Resource.Description_Label_Text;
        this.DisplayNameLabel.Text  = Resources.Resource.Display_Label_Text;
        this.UserLabel.Text = Resources.Resource.User_Label_Text;
        this.FormGroupLabel.Text = Resources.Resource.FormGroup_Label_Text;
        this.DateCreatedLabel.Text = Resources.Resource.DateCreated_Label_Text;
        this.AddSortByItems();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Add sortby items to the webmenu.
    /// </summary>
    private void AddSortByItems()
    {
        Item rootNode = new Item();
        rootNode.Text = Resources.Resource.SortBy_Label_Text;

        Item ID_Asc = new Item();
        ID_Asc.Text = "ID Asc";
        ID_Asc.DataKey = "ID_ASC";
        rootNode.Items.Add(ID_Asc);

        Item Database_Asc = new Item();
        Database_Asc.Text = "DB Asc";
        Database_Asc.DataKey = Constants.SortCriterias.Database_ASC.ToString();
        rootNode.Items.Add(Database_Asc);

        Item Name_Asc = new Item();
        Name_Asc.Text = "Name Asc";
        Name_Asc.DataKey = Constants.SortCriterias.Name_ASC.ToString();
        rootNode.Items.Add(Name_Asc);

        this.SortByWebMenu.Items.Add(rootNode);
    }

    /// <summary>
    /// Method to bind the usercontrol with a DataViewList
    /// </summary>
    /// <param name="dataViewsBOList">A DataViewList dataSource</param>
    public void DataBind(COEDataViewBOList dataViewsBOList)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.DataViewBOList = dataViewsBOList;
        this.ExistingDataViewsUltraWebTree.Nodes.Clear();
        
        List<Node> nodeList = this.CreateDataSource(dataViewsBOList, _defaultSortCriteria);
        foreach (Node node in nodeList)
            this.ExistingDataViewsUltraWebTree.Nodes.Add(node);
       
        //CBOE-708 : Object reference error is displayed ... when session timeout occurred. ASV 260413.
		//Added condition to check if querystring contains the action as Edit_DV and SelectedDataViewID attribute.
		//then bind the details of selected detaview id.
        if (this.Page.ClientQueryString.Contains(Constants.PageStates.Edit_DV.ToString()) 
			&& this.Page.ClientQueryString.Contains(Constants.SelectedDataViewID))
        {
			if (Utilities.IsActionNode(Page.Request.QueryString.Get(Constants.SelectedDataViewID)))
            {
                this.SelectedNode = "ID="+Page.Request.QueryString.Get(Constants.SelectedDataViewID);
                this.BindNodeDetails(this.SelectedNodeID);
            }
        }
        else
        this.BindNodeDetails(int.MinValue);
        this.SetSelectedSort(_defaultSortCriteria.ToString());
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
                    this.DataBaseLabel.Text = Resources.Resource.Database_Label_Text + ": " + selectedDataViewBO.COEDataView.Database;
                    this.DisplayNameTextBoxWithPopUp.Text = selectedDataViewBO.Name;
                    this.DescriptionTextBoxWithPopUp.Text = selectedDataViewBO.Description;
                    this.UserTextBoxWithPopUp.Text = selectedDataViewBO.UserName;
                    this.FormGroupTextBoxWithPopUp.Text = selectedDataViewBO.FormGroup.ToString();
                    this.DateCreatedTextBoxWithPopUp.Text = selectedDataViewBO.DateCreated.ToString(Resources.Resource.DateFormat);
                     
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
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion
}
