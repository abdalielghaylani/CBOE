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
using Csla;

public partial class SelectTables : System.Web.UI.UserControl
{
    #region Public Events
    public event CommandEventHandler NodeSelected;
    public event CommandEventHandler NodeChecked;
    public event EventHandler NodeExpanded;
    public event EventHandler NodeCollapsed;
    public event EventHandler SortApplied;
    #endregion

    #region Variables
    private Constants.SortCriterias _defaultSortCriteria = Constants.SortCriterias.Database_ASC;
    #endregion

    #region Properties
    /// <summary>
    /// Id of the base table
    /// </summary>
    private int BaseTableID
    {
        get
        {
            if (ViewState["BaseTableID"] == null)
                ViewState["BaseTableID"] = int.MinValue;

            return (int)ViewState["BaseTableID"];
        }
        set
        {
            ViewState["BaseTableID"] = value;
        }
    }

    /// <summary>
    /// List of tables
    /// </summary>
    private TableListBO Tables
    {
        get
        {
            return Session[Constants.TableListBO] != null ? (TableListBO)Session[Constants.TableListBO] : null;
        }
        set
        {
            if (Session[Constants.TableListBO] != value && value != null)
                Session[Constants.TableListBO] = value;
        }
    }

    /// <summary>
    /// Node expanded
    /// </summary>
    public string ExpandedNode
    {
        get
        {
            if (ViewState["ExpandedNode"] == null)
                ViewState["ExpandedNode"] = string.Empty;

            return (string)ViewState["ExpandedNode"];
        }
        set
        {
            ViewState["ExpandedNode"] = value;
        }
    }

    /// <summary>
    /// Node selected
    /// </summary>
    public string SelectedNode
    {
        get
        {
            if (this.TablesUltraWebTree.SelectedNode != null)
            {
                if (Utilities.GetParamInDataKey(this.TablesUltraWebTree.SelectedNode.DataKey.ToString(), Constants.DataKeysParam.ACTION) == Constants.NodeAction.DoNothing.ToString() ||
                   Utilities.GetParamInDataKey(this.TablesUltraWebTree.SelectedNode.DataKey.ToString(), Constants.DataKeysParam.ACTION) == Constants.NodeAction.Grouping.ToString())
                {
                    return Utilities.GetParamInDataKey(this.TablesUltraWebTree.SelectedNode.DataKey.ToString(), Constants.DataKeysParam.NAME)
                            + this.TablesUltraWebTree.SelectedNode.Text;
                }
                else
                {
                    return Utilities.GetParamInDataKey(this.TablesUltraWebTree.SelectedNode.DataKey.ToString(), Constants.DataKeysParam.ID);
                }
            }
            else
                return string.Empty;
        }
        set
        {
            foreach (Node schemaNode in this.TablesUltraWebTree.Nodes)
            {
                if ((Utilities.GetParamInDataKey(schemaNode.DataKey.ToString(), Constants.DataKeysParam.NAME) + schemaNode.Text) == value)
                {
                    this.TablesUltraWebTree.CollapseAll();
                    schemaNode.Expanded = true;
                    this.TablesUltraWebTree.SelectedNode = schemaNode;
                    if (NodeSelected != null)
                        NodeSelected(this, new CommandEventArgs("NodeSelected", schemaNode));
                    break;
                }
                bool quit = false;
                foreach (Node groupingNode in schemaNode.Nodes)
                {
                    if ((Utilities.GetParamInDataKey(schemaNode.DataKey.ToString(), Constants.DataKeysParam.NAME) + schemaNode.Text) == value)
                    {
                        this.TablesUltraWebTree.CollapseAll();
                        schemaNode.Expanded = true;
                        groupingNode.Expanded = true;
                        this.TablesUltraWebTree.SelectedNode = groupingNode;
                        quit = true;
                        if (NodeSelected != null)
                            NodeSelected(this, new CommandEventArgs("NodeSelected", groupingNode));
                        break;
                    }
                    foreach (Node tableNode in groupingNode.Nodes)
                    {
                        if (Utilities.GetParamInDataKey(tableNode.DataKey.ToString(), Constants.DataKeysParam.ID) == value)
                        {
                            this.TablesUltraWebTree.CollapseAll();
                            schemaNode.Expanded = true;
                            groupingNode.Expanded = true;
                            tableNode.Selected = true;

                            quit = true;
                            if (NodeSelected != null)
                                NodeSelected(this, new CommandEventArgs("NodeSelected", tableNode));
                            break;
                        }
                    }
                    if (quit)
                        break;
                }
                if (quit)
                    break;
            }
        }
    }

    /// <summary>
    /// First node
    /// </summary>
    public string FirstNode
    {
        get
        {
            if (this.TablesUltraWebTree.Nodes.Count > 0)
            {
                if (Utilities.GetParamInDataKey(this.TablesUltraWebTree.Nodes[0].DataKey.ToString(), Constants.DataKeysParam.ACTION) == Constants.NodeAction.DoNothing.ToString())
                {
                    return Utilities.GetParamInDataKey(this.TablesUltraWebTree.Nodes[0].DataKey.ToString(), Constants.DataKeysParam.NAME)
                           + this.TablesUltraWebTree.Nodes[0].Text;
                }
                else
                {
                    return Utilities.GetParamInDataKey(this.TablesUltraWebTree.Nodes[0].DataKey.ToString(), Constants.DataKeysParam.ID);
                }
            }
            else
                return string.Empty;
        }
    }
    #endregion

    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        ScriptManager sm = ScriptManager.GetCurrent(this.Page);
        this.TablesUltraWebTree.EnableViewState = true;
        if (sm != null)
        {
            sm.RegisterAsyncPostBackControl(this.TablesUltraWebTree);
            sm.RegisterAsyncPostBackControl(this.SortByWebMenu);
        }
        if (!Page.IsPostBack)
        {
            this.SetControlsAttributes();
        }
        this.ShowConfirmationMessage(null);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
    }

    /// <summary>
    /// Ocurrs when a Node in the tree is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void TablesUltraWebTree_NodeClicked(object sender, WebTreeNodeEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (this.NodeSelected != null)
        {
            NodeSelected(this, new CommandEventArgs("NodeSelected", e.Node));
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Ocurrs when a Node in the tree is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void TablesUltraWebTree_NodeChecked(object sender, WebTreeNodeEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        int tableRemovedHadRelationships = 0;
        int tableRemovedHadLookups = 0;

        if (Utilities.GetParamInDataKey(e.Node.DataKey.ToString(), Constants.DataKeysParam.ACTION) == Constants.NodeAction.DoNothing.ToString())
        {
            foreach (Node groupingNode in e.Node.Nodes)
            {
                groupingNode.Checked = e.Node.Checked;
                foreach (Node tableNode in groupingNode.Nodes)
                {
                    int tableId = int.Parse(Utilities.GetParamInDataKey(tableNode.DataKey.ToString(), Constants.DataKeysParam.ID));
                    if (tableNode.CheckBox == CheckBoxes.True)
                    {
                        tableNode.Checked = e.Node.Checked;
                        if (!tableNode.Checked)
                        {
                            tableRemovedHadRelationships += this.IsTableInRelationships(tableId) ? 1 : 0;
                            tableRemovedHadLookups += this.IsTableInLookup(tableId) ? 1 : 0;
                        }
                    }
                }
            }
        }
        else if (Utilities.GetParamInDataKey(e.Node.DataKey.ToString(), Constants.DataKeysParam.ACTION) == Constants.NodeAction.Grouping.ToString())
        {
            foreach (Node tableNode in e.Node.Nodes)
            {
                int tableId = int.Parse(Utilities.GetParamInDataKey(tableNode.DataKey.ToString(), Constants.DataKeysParam.ID));
                if (tableNode.CheckBox == CheckBoxes.True)
                {
                    tableNode.Checked = e.Node.Checked;
                    if (!tableNode.Checked)
                    {
                        tableRemovedHadRelationships += this.IsTableInRelationships(tableId) ? 1 : 0;
                        tableRemovedHadLookups += this.IsTableInLookup(tableId) ? 1 : 0;
                    }
                }
            }
            bool allchecked = true;
            foreach (Node groupingNode in e.Node.Parent.Nodes)
            {
                allchecked &= groupingNode.Checked;
                if (!allchecked)
                    break;
            }
            e.Node.Parent.Checked = allchecked;
        }
        else
        {
            if (!e.Node.Checked)
            {
                int tableId = int.Parse(Utilities.GetParamInDataKey(e.Node.DataKey.ToString(), Constants.DataKeysParam.ID));
                tableRemovedHadRelationships += this.IsTableInRelationships(tableId) ? 1 : 0;
                tableRemovedHadLookups += this.IsTableInLookup(tableId) ? 1 : 0;
            }
            bool allchecked = true;
            foreach (Node tableNode in e.Node.Parent.Nodes)
            {
                allchecked &= tableNode.Checked;
                if (!allchecked)
                    break;
            }
            e.Node.Parent.Checked = allchecked;
            allchecked = true;
            foreach (Node groupingNode in e.Node.Parent.Parent.Nodes)
            {
                allchecked &= groupingNode.Checked;
                if (!allchecked)
                    break;
            }
            e.Node.Parent.Parent.Checked = allchecked;
        }

        this.ShowConfirmationMessage(this.BuildConfirmationMessage(tableRemovedHadRelationships, tableRemovedHadLookups));

        if (NodeChecked != null)
            NodeChecked(this, new CommandEventArgs("NodeChecked", e.Node));

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Occurs when a node is expanded
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void TablesUltraWebTree_NodeExpanded(object sender, WebTreeNodeEventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["__EVENTTARGET"]))
        {
            this.ExpandedNode = Utilities.GetParamInDataKey(e.Node.DataKey.ToString(), Constants.DataKeysParam.NAME) + e.Node.Text;
            if (NodeExpanded != null)
                NodeExpanded(this, e);
            this.TablesUltraWebTree_NodeClicked(sender, e);
        }
    }

    /// <summary>
    /// Occurs when a node is collapsed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void TablesUltraWebTree_NodeCollapsed(object sender, WebTreeNodeEventArgs e)
    {
        if (Utilities.GetParamInDataKey(e.Node.DataKey.ToString(), Constants.DataKeysParam.ACTION) == Constants.NodeAction.Grouping.ToString())
            this.ExpandedNode = Utilities.GetParamInDataKey(e.Node.Parent.DataKey.ToString(), Constants.DataKeysParam.NAME) + e.Node.Text;
        else
            this.ExpandedNode = string.Empty;

        if (NodeCollapsed != null)
            NodeCollapsed(this, e);
    }

    /// <summary>
    /// Ocurres when a SortBy item is clicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SortByWebMenu_MenuItemClicked(object sender, WebMenuItemEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (e.Item.DataKey != null)
        {
            this.SortTables((Constants.SortCriterias)Enum.Parse(typeof(Constants.SortCriterias), e.Item.DataKey as string));
            if (this.SortApplied != null)
                SortApplied(this, new EventArgs());
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion

    #region Methods

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
    /// Method to sort the datasource (a list of tables) given a criteria.
    /// </summary>
    /// <param name="criteria">Criteria to follow. This comes from the datakey of the webmenu of SortBy</param>
    private void SortTables(Constants.SortCriterias criteria)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (this.Tables != null)
        {

            List<int> checkedTableIDs = this.GetCheckedTables();

            this.TablesUltraWebTree.Nodes.Clear();
            List<Node> nodeList = this.CreateDataSource(this.Tables, criteria, checkedTableIDs, this.BaseTableID);
            foreach (Node node in nodeList)
            {
                node.Expanded = (Utilities.GetParamInDataKey(node.DataKey.ToString(), Constants.DataKeysParam.NAME) + node.Text) == this.ExpandedNode;
                this.TablesUltraWebTree.Nodes.Add(node);
            }
            this.SetSelectedSort(criteria.ToString());
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Gets the tables that are selected in the tree
    /// </summary>
    /// <returns></returns>
    private List<int> GetCheckedTables()
    {
        List<int> checkedTables = new List<int>();
        foreach (Node schemaNode in this.TablesUltraWebTree.Nodes)
        {
            if (Utilities.GetParamInDataKey(schemaNode.DataKey.ToString(), Constants.DataKeysParam.ACTION) == Constants.NodeAction.DoNothing.ToString())
            {
                foreach (Node groupingNode in schemaNode.Nodes)
                {
                    foreach (Node tableNode in groupingNode.Nodes)
                    {
                        if (tableNode.Checked)
                            checkedTables.Add(int.Parse(Utilities.GetParamInDataKey(tableNode.DataKey.ToString(), Constants.DataKeysParam.ID)));
                    }
                }
            }
            else
            {
                if (schemaNode.Checked)
                    checkedTables.Add(int.Parse(Utilities.GetParamInDataKey(schemaNode.DataKey.ToString(), Constants.DataKeysParam.ID)));
            }
        }
        return checkedTables;
    }

    /// <summary>
    /// Method to add the items to show in the sortby webmenu
    /// </summary>
    private void AddSortByItems()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        Item rootNode = new Item();
        rootNode.Text = Resources.Resource.SortBy_Label_Text;

        Item Database_Asc = new Item();
        Database_Asc.Text = "DB Asc";
        Database_Asc.DataKey = Constants.SortCriterias.Database_ASC.ToString();
        rootNode.Items.Add(Database_Asc);

        Item Name_Asc = new Item();
        Name_Asc.Text = "Name Asc";
        Name_Asc.DataKey = Constants.SortCriterias.Name_ASC.ToString();
        rootNode.Items.Add(Name_Asc);

        this.SortByWebMenu.Items.Add(rootNode);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Method to set all the controls attributtes as Text, tooltip, etc...
    /// </summary>
    private void SetControlsAttributes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.AddSortByItems();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Binds a datasource to the control
    /// </summary>
    /// <param name="dataViewsNodes">List of tables to display</param>
    /// <remarks>Here we have to convert the datasource to a List<DataViewNode></remarks>
    public void DataBind(TableListBO tables, List<int> selectedTablesIds, int basetableId)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.Tables = tables;
        this.BaseTableID = basetableId;
        this.TablesUltraWebTree.Nodes.Clear();
        List<Node> nodeList = this.CreateDataSource(tables, _defaultSortCriteria, selectedTablesIds, basetableId);
        foreach (Node node in nodeList)
        {
            this.TablesUltraWebTree.Nodes.Add(node);
            if (string.IsNullOrEmpty(this.ExpandedNode))
                this.ExpandedNode = this.SelectedNode = (Utilities.GetParamInDataKey(node.DataKey.ToString(), Constants.DataKeysParam.NAME) + node.Text);
            //CSBR-128248
            foreach (Node subnode in node.Nodes)
            {
                foreach (Node tablenode in subnode.Nodes)
                {
                    if (tablenode.CheckBox == CheckBoxes.True)
                    {
                        if (tablenode.Checked == true)
                        {
                            node.Style.Font.Bold = true;
                            subnode.Style.Font.Bold = true;
                            break;
                        }
                    }
                    else
                    {
                        node.Style.Font.Bold = true;
                        subnode.Style.Font.Bold = true;
                        this.ExpandedNode = this.SelectedNode = node.ToString() + node.ToString();
                        break;
                    }                    
                }
            }
            node.Expanded = (Utilities.GetParamInDataKey(node.DataKey.ToString(), Constants.DataKeysParam.NAME) + node.Text) == this.ExpandedNode;
        }
        this.SetSelectedSort(_defaultSortCriteria.ToString());
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Created the necesary Nodes for the presentation of the info in the current webtree
    /// </summary>
    /// <param name="tables">Tables to display</param>
    /// <param name="criteria">How it will be ordered</param>
    /// <param name="selectedTablesIds">Table to display as checked(selected)</param>
    /// <returns></returns>
    private List<Node> CreateDataSource(TableListBO tables, Constants.SortCriterias criteria, List<int> selectedTablesIds, int basetableId)
    {
        List<Node> nodes = new List<Node>();
        if (tables != null)
        {
            List<string> databasesList = new List<string>();
            Csla.SortedBindingList<TableBO> sortedTables;
            string tempDBName = String.Empty;
            switch (criteria)
            {
                case Constants.SortCriterias.Database_ASC:
                    sortedTables = tables.ApplySort("DataBase", System.ComponentModel.ListSortDirection.Ascending);
                    foreach (TableBO table in sortedTables)
                    {
                        if (!databasesList.Contains(table.DataBase))
                        {
                            //Filter by DataBase
                            List<TableBO> filteredList = tables.GetTablesByDB(table.DataBase);
                            Node node = Utilities.CreateTreeStructure(DataViewNode.NewDataViewNode(filteredList, criteria, selectedTablesIds, basetableId));
                            nodes.Add(node);
                            databasesList.Add(table.DataBase);
                        }
                    }
                    break;
                case Constants.SortCriterias.Name_ASC:
                    sortedTables = new Csla.SortedBindingList<TableBO>(tables);
                    sortedTables = tables.ApplySort("Name", System.ComponentModel.ListSortDirection.Ascending);
                    foreach (TableBO table in sortedTables)
                    {
                        if (selectedTablesIds != null)
                        {
                            Node node = Utilities.CreateTreeStructure(DataViewNode.NewDataViewNode(table, true, selectedTablesIds.Contains(table.ID)));
                            if (table.ID == basetableId)
                            {
                                node.CssClass = "ParentTableNode";
                                node.Checked = true;
                                node.CheckBox = CheckBoxes.False;
                            }
                            nodes.Add(node);
                        }
                        else
                        {
                            Node node = Utilities.CreateTreeStructure(DataViewNode.NewDataViewNode(table, true, false));
                            if (table.ID == basetableId)
                            {
                                node.CssClass = "ParentTableNode";
                                node.Checked = true;
                                node.CheckBox = CheckBoxes.False;
                            }
                            nodes.Add(node);
                        }
                    }
                    break;
                case Constants.SortCriterias.Name_DESC:
                    sortedTables = new Csla.SortedBindingList<TableBO>(tables);
                    sortedTables = tables.ApplySort("Name", System.ComponentModel.ListSortDirection.Descending);
                    nodes.Add(Utilities.CreateTreeStructure(DataViewNode.NewDataViewNode(sortedTables, criteria, selectedTablesIds, basetableId)));

                    break;
                default:
                    foreach (TableBO table in tables)
                    {
                        if (!databasesList.Contains(table.DataBase))
                        {
                            //Filter by DataBase
                            List<TableBO> filteredList = tables.GetTablesByDB(table.DataBase);
                            Node node = Utilities.CreateTreeStructure(DataViewNode.NewDataViewNode(filteredList, _defaultSortCriteria, selectedTablesIds, basetableId));
                            nodes.Add(node);
                            databasesList.Add(table.DataBase);
                        }
                    }
                    break;
            }
        }
        return nodes;
    }

    /// <summary>
    /// Returns a List
    /// </summary>
    /// <returns></returns>
    public List<int> UnBind()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //ArrayList checkedNodes = new ArrayList();
        List<int> selectedTablesIds = new List<int>();
        this.CreateListofTablesIds(selectedTablesIds, this.TablesUltraWebTree.CheckedNodes);
        //Now we have all the selected tables.
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return selectedTablesIds;
    }

    /// <summary>
    /// Adds a table
    /// </summary>
    /// <param name="tableBO"></param>
    public void AddTable(TableBO tableBO)
    {
        foreach (Node parentNode in this.TablesUltraWebTree.Nodes)
        {
            if (Utilities.GetParamInDataKey(parentNode.DataKey.ToString(), Constants.DataKeysParam.NAME) == tableBO.DataBase)
            {
                DataViewNode dataViewNode = DataViewNode.NewDataViewNode(tableBO, true, true);
                Node node = new Node();

                node.ToolTip = dataViewNode.ToolTip;
                node.Text = dataViewNode.Text;
                node.DataKey = dataViewNode.DataKey;
                node.Tag = node.DataKey;
                if (!dataViewNode.Enabled)
                {
                    node.CssClass = "ParentTableNode";
                    node.Checked = true;
                }
                node.Hidden = !dataViewNode.Visible;
                if (dataViewNode.CheckBox && dataViewNode.Enabled)
                {
                    node.CheckBox = CheckBoxes.True;
                    node.Checked = dataViewNode.Checked;
                }
                if (dataViewNode.IsView)
                {
                    if (parentNode.Nodes.Count > 1)
                        parentNode.Nodes[1].Nodes.Add(node);
                    else
                        parentNode.Nodes[0].Nodes.Add(node);
                }
                else
                    parentNode.Nodes[0].Nodes.Add(node);

                //parentNode.Nodes.Add(node);
                break;
            }
        }
    }

    /// <summary>
    /// Deletes a table
    /// </summary>
    /// <param name="tableId"></param>
    /// <param name="removeItem"></param>
    public void DeleteTable(int tableId, bool removeItem)
    {
        if (Utilities.GetParamInDataKey(this.TablesUltraWebTree.Nodes[0].DataKey.ToString(), Constants.DataKeysParam.ACTION) == Constants.NodeAction.DoNothing.ToString())
        {
            for (int i = 0; i < this.TablesUltraWebTree.Nodes.Count; i++)
            {
                this.DeleteTable(tableId, removeItem, i);
            }
        }
        else
            this.DeleteTable(tableId, removeItem, 0);
    }

    /// <summary>
    /// Deletes a table
    /// </summary>
    /// <param name="tableId"></param>
    /// <param name="removeItem"></param>
    /// <param name="parentNodeIndex"></param>
    public void DeleteTable(int tableId, bool removeItem, int parentNodeIndex)
    {
        int index = 0;
        int groupingIndex = 0;
        bool itemFound = false;
        int tableHadRelationship = 0;
        int tableHadLookup = 0;

        foreach (Node groupingNode in this.TablesUltraWebTree.Nodes[parentNodeIndex].Nodes)
        {
            groupingIndex = 0;
            foreach (Node node in groupingNode.Nodes)
            {
                if (Utilities.GetParamInDataKey(node.DataKey.ToString(), Constants.DataKeysParam.ID) == tableId.ToString())
                {
                    if (!removeItem)
                    {
                        node.Checked = false;
                    }
                    itemFound = true;
                    break;
                }
                groupingIndex++;
            }
            if (itemFound)
                break;
            index++;
        }
        if (removeItem && itemFound)
        {
            this.TablesUltraWebTree.Nodes[parentNodeIndex].Nodes[groupingIndex].Nodes.RemoveAt(index);
        }

        tableHadRelationship += IsTableInRelationships(tableId) ? 1 : 0;
        tableHadLookup += IsTableInLookup(tableId) ? 1 : 0;
        this.ShowConfirmationMessage(this.BuildConfirmationMessage(tableHadRelationship, tableHadLookup));
    }

    /// <summary>
    /// Checks if a table belongs to a relationship
    /// </summary>
    /// <param name="tableId"></param>
    /// <returns></returns>
    private bool IsTableInRelationships(int tableId)
    {
        COEDataViewBO dvBO = ((Manager.Forms.Master.DataViewManager)this.Page.Master).GetDataViewBO();
        return dvBO.DataViewManager.Relationships.GetByParentOrChildId(tableId).Count > 0;
    }

    /// <summary>
    /// Checks if a table belongs to a lookup
    /// </summary>
    /// <param name="tableId"></param>
    /// <returns></returns>
    private bool IsTableInLookup(int tableId)
    {
        COEDataViewBO dvBO = ((Manager.Forms.Master.DataViewManager)this.Page.Master).GetDataViewBO();
        foreach (FieldBO field in dvBO.DataViewManager.Tables.GetTable(tableId).Fields)
        {
            if (field.LookupDisplayFieldId > 0)
                return true;

            List<TableBO> tables = dvBO.DataViewManager.Tables.GetTablesByLookup(field.ID);
            if (tables != null && tables.Count > 0)
                return true;

        }
        return false;
    }

    /// <summary>
    /// Builds a confirmation message
    /// </summary>
    /// <param name="tablesWithRelationships"></param>
    /// <param name="tablesWithLookups"></param>
    /// <returns></returns>
    private string BuildConfirmationMessage(int tablesWithRelationships, int tablesWithLookups)
    {
        string confirmationMessage = string.Empty;
        if (tablesWithLookups > 1)
        {
            if (tablesWithRelationships > 0)
                confirmationMessage = Resources.Resource.TablesRemovedHaveRelationshipsAndLookups_Label_Text;
            else
                confirmationMessage = Resources.Resource.TablesRemovedHaveLookups_Label_Text;
        }
        else if (tablesWithLookups > 0)
        {
            if (tablesWithRelationships > 1)
                confirmationMessage = Resources.Resource.TablesRemovedHaveRelationshipsAndLookups_Label_Text;
            else if (tablesWithRelationships > 0)
                confirmationMessage = Resources.Resource.TableRemovedHasRelationshipsAndLookups_Label_Text;
            else
                confirmationMessage = Resources.Resource.TableRemovedHasLookups_Label_Text;
        }
        else
        {
            if (tablesWithRelationships > 1)
                confirmationMessage = Resources.Resource.TablesRemovedHaveRelationships_Label_Text;
            else if (tablesWithRelationships > 0)
                confirmationMessage = Resources.Resource.TableRemovedHasRelationships_Label_Text;
        }
        return confirmationMessage;
    }

    /// <summary>
    /// Shows a confirmation message
    /// </summary>
    /// <param name="message"></param>
    private void ShowConfirmationMessage(string message)
    {
        Forms_Public_UserControls_ConfirmationArea confirmationArea = null;
        UpdatePanel updPanel = (UpdatePanel)((Manager.Forms.Master.DataViewManager)this.Page.Master).FindControlInPage("ConfirmationAreaUpdatePanel");

        if (updPanel != null)
        {
            confirmationArea = (Forms_Public_UserControls_ConfirmationArea)updPanel.FindControl("ConfirmationAreaUserControl");
            if (confirmationArea != null)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    confirmationArea.Visible = true;
                    confirmationArea.Text = message;
                }
                else
                    confirmationArea.Visible = false;
            }
        }
    }

    /// <summary>
    /// Creates a list of selected Ids from the webtree info.
    /// </summary>
    /// <param name="IdList">List of selected ids</param>
    /// <param name="checkedNodes">DataKeys of the selected tables in the webtree</param>
    private void CreateListofTablesIds(List<int> IdList, ArrayList checkedNodes)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        int id = -1;
        foreach (Node currentTable in checkedNodes)
        {
            string tempId = Utilities.GetParamInDataKey(currentTable.DataKey as string, Constants.DataKeysParam.ID);
            if (int.TryParse(tempId, out id))
                IdList.Add(id);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    #endregion
}
