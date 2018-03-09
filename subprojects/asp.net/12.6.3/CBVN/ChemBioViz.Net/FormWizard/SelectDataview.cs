using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Infragistics.Win.UltraWinTree;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;
using Infragistics.Win;

namespace FormWizard
{
    public partial class SelectDataview : UserControl
    {

        #region Private Fields

        private COEDataViewManagerBO _dataViewManagerBO;

        private int baseTableId = 0;

        Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager();

        //public COEDataViewManagerBO DataViewManagerBO
        //{
        //    get
        //    {
        //        return _dataViewManagerBO;
        //    }
        //    set
        //    {
        //        _dataViewManagerBO = value;
        //if (this.ParentForm == null)
        //    _dataViewManagerBO = null;
        //else if (this.ParentForm is SelectDataForm)
        //    _dataViewManagerBO = (this.ParentForm as SelectDataForm)._dataViewManagerBO;
        //else if(this.ParentForm != null && this.ParentForm is SelectDataviewForm)
        //    _dataViewManagerBO = (this.ParentForm as SelectDataviewForm).DataviewMangerBO;
        //    }
        //}

        private ITableFilter _tableFilter;
        public ITableFilter tableFilter
        {
            get
            {
                return this._tableFilter;
            }
            set
            {
                this._tableFilter = value;
                RebuildTreeView(this._dataViewManagerBO);
            }
        }

        private IDictionary<object, bool> _expansionState = new Dictionary<object, bool>();

        #endregion

        public SelectDataview()
        {
            InitializeComponent();
            SetWatermark(searchTextBox, "Search Tables and Fields");
        }

        #region SetWatermark

        private const uint ECM_FIRST = 0x1500;
        private const uint EM_SETCUEBANNER = ECM_FIRST + 1;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        private static void SetWatermark(TextBox textBox, string watermarkText)
        {
            SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, watermarkText);
        }

        #endregion

        #region Private Methods



        private void SaveExpansionState()
        {
            UpdateExpansionState(this.availableFieldsTreeView.Nodes);

        }

        /// <summary>
        /// Builds a set containing the ids of all the tables in the current dataview
        /// that are related to the base table.
        /// </summary>
        /// <returns>A set of table ids</returns>
        private HashSet<int> GetRelatedTableIds()
        {
            HashSet<int> ids = new HashSet<int>();
            ids.Add(this._dataViewManagerBO.BaseTableId);
            bool changed = true;
            while (changed)
            {
                changed = false;
                foreach (RelationshipBO rel in this._dataViewManagerBO.Relationships)
                {
                    if (ids.Contains(rel.Parent) && !ids.Contains(rel.Child))
                    {
                        ids.Add(rel.Child);
                        changed = true;
                    }
                }
            }
            return ids;
        }

        /// <summary>
        /// Builds a heirarchical set containing the ids of all the tables in the current dataview
        /// that are related to the base table in parent-child relationship order
        /// </summary>
        /// <returns>A table relationship heirarchy collection</returns>
        private TableRelationships GetRelatedTableIdRelations()
        {
            TableRelationships theTableRelationships = new TableRelationships();
            theTableRelationships.TableId = baseTableId;

            //bool changed = true;
            //while (changed)
            //{
            //    changed = false;
            //foreach (RelationshipBO rel in this._dataViewManagerBO.Relationships)
            //{
            //    if (!theTableRelationships.Contains(rel.Parent) || !theTableRelationships.Contains(rel.Child))
            //    {
            //        theTableRelationships.AddChild(rel.Parent, rel.Child);
            //        changed = true;
            //    }
            //}
            //}

            //Collecting Child Tables
            foreach (RelationshipBO rel in this._dataViewManagerBO.Relationships)
            {
                if ((rel.Parent == baseTableId) && !theTableRelationships.Contains(rel.Child))
                {
                    theTableRelationships.AddChild(rel.Parent, rel.Child);
                }
            }
            //Collecting Grand Child Tables
            foreach (RelationshipBO rel in this._dataViewManagerBO.Relationships)
            {
                if (!theTableRelationships.Contains(rel.Parent) || !theTableRelationships.Contains(rel.Child))
                {
                    foreach (TableRelationships childTables in theTableRelationships.ChildTableRelationships)
                    {
                        if (childTables.TableId == rel.Parent)
                        {
                            childTables.AddChild(rel.Parent, rel.Child);
                            break;
                        }
                    }
                }
            }

            return theTableRelationships;
        }

        private IEnumerable<string> GetCategories()
        {
            HashSet<string> categories = new HashSet<string>();
            foreach (TableBO tableBO in this._dataViewManagerBO.Tables)
            {
                if (tableBO.Tags != null)
                {
                    foreach (string tag in tableBO.Tags)
                    {
                        categories.Add(tag);
                    }
                }
            }
            return categories;
        }

        /// <summary>
        /// Gets the tags associated with tableBO object
        /// </summary>
        /// <param name="tableBO">tableBO object to get the list of tags</param>
        /// <returns>retunrs collection of tags</returns>
        private IEnumerable<string> GetCategoriesForTable(TableBO tableBO)
        {
            HashSet<string> categories = new HashSet<string>();
            if (tableBO.Tags != null)
            {
                foreach (string tag in tableBO.Tags)
                {
                    categories.Add(tag);
                }
            }
            return categories;
        }

        /// <summary>
        /// Creates the tool tip display text to show the tags associated with table object
        /// </summary>
        /// <param name="tableBO">table object to get all tags and create tool tip text</param>
        /// <returns>returns tool tip text</returns>
        private string BuildToolTipForTreeNode(TableBO tableBO)
        {
            StringBuilder toolTipString = new StringBuilder();
            IEnumerable<string> tagsCollection = GetCategoriesForTable(tableBO);
            foreach (string str in tagsCollection)
            {
                toolTipString.Append(str + ",");
            }
            if (toolTipString.Length > 0)
            {
                toolTipString = toolTipString.Remove(toolTipString.Length - 1, 1);
            }
            return toolTipString.ToString();
        }

        private void availableFieldsTreeView_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            if (e.Element is Infragistics.Win.UltraWinTree.NodeTextUIElement)
            {
                // Get the location of the cursor
                Point pt = new Point(e.Element.Rect.X, e.Element.Rect.Y);
                Infragistics.Win.UltraWinTree.UltraTreeNode node = this.availableFieldsTreeView.GetNodeFromPoint(pt);

                // If the cursor is on a Tree node
                if (node != null && node.Tag is TableBO)
                {
                    TableBO tableBO = (TableBO)node.Tag;
                    string toolTip = BuildToolTipForTreeNode(tableBO);
                    if (!string.IsNullOrEmpty(toolTip))
                    {
                        Infragistics.Win.UltraWinToolTip.UltraToolTipInfo tipInfo = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo(string.Format("Tags:{0}", toolTip), Infragistics.Win.ToolTipImage.Default, "", Infragistics.Win.DefaultableBoolean.True);
                        this.ultraToolTipManager1.SetUltraToolTip(this.availableFieldsTreeView, tipInfo);
                        this.ultraToolTipManager1.ShowToolTip(this.availableFieldsTreeView);
                    }
                    else
                        this.ultraToolTipManager1.SetUltraToolTip(this.availableFieldsTreeView, null);
                }
            }
            else
            {
                this.ultraToolTipManager1.HideToolTip();
            }
        }

        /// <summary>
        /// Determines whether search text matches tag value in table bo object
        /// </summary>
        /// <param name="searchText">test to serach</param>
        /// <param name="tableBO">tablBo object to search the tags</param>
        /// <returns>returns true if search text found in tag; otherwise false</returns>
        private bool IsTagContainsSearchText(string searchText, TableBO tableBO)
        {
            bool result = false;
            result = GetCategoriesForTable(tableBO).Contains(searchText, new TagComparer());
            return result;
        }

        /// <summary>
        /// Populates the treenode collection using the base table node and table relationships collection
        /// </summary>
        /// <param name="baseTableNode">tree node corresponding to the base tableBO object</param>
        /// <param name="tables">collection of all tableBO objects from current dataview</param>
        /// <param name="tableRelationships">table relation hierarchy object to decide the order of tree nodes</param>
        private void PopulateCategoryNode(UltraTreeNode baseTableNode, IEnumerable<TableBO> tables, TableRelationships tableRelationships)
        {
            string searchText = this.searchTextBox.Text.ToUpper();
            if (tableRelationships.ChildTableRelationships != null)
            {
                foreach (TableRelationships tableRelationShipCollection in tableRelationships.ChildTableRelationships)
                {
                    int tableIdToAdd = tableRelationShipCollection.TableId; // tableRelationships.ChildTableRelationships.TableId;
                    TableBO tableBO = tables.FirstOrDefault(t => t.ID == tableIdToAdd);

                    string tableAlias = tableBO.Alias;
                    UltraTreeNode tableNode = new UltraTreeNode(null, tableAlias);
                    tableNode.Tag = tableBO;

                    //added condition to check if table bo contains the search text as table tag
                    bool addTableNode = (string.IsNullOrEmpty(searchText) || tableBO.Alias.ToUpper().Contains(searchText) || IsTagContainsSearchText(searchText, tableBO));
                    bool addAllFieldNodes = addTableNode;

                    foreach (FieldBO fieldBO in tableBO.Fields)
                    {
                        if (!fieldBO.Visible)
                        {
                            continue;
                        }

                        if (addAllFieldNodes || fieldBO.Alias.ToUpper().Contains(searchText))
                        {
                            addTableNode = true;
                            UltraTreeNode fieldNode = tableNode.Nodes.Add(null, fieldBO.Alias);
                            FieldBO lookupField = null;
                            if (fieldBO.LookupDisplayFieldId > 0)
                                lookupField = _dataViewManagerBO.Tables.GetField(fieldBO.LookupDisplayFieldId);
                            fieldNode.Tag = new SelectDataForm.TreeNodeFieldContext(tableBO, fieldBO, _dataViewManagerBO.DataViewId, lookupField);

                            int imageIndex = GetImageIndex(fieldBO, lookupField);
                            if (imageIndex > 0)
                            {
                                fieldNode.LeftImages.Add(this.availableFieldsImageList.Images[imageIndex]);
                            }
                            if ((fieldBO.Alias == "Structure" || fieldBO.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE) || ((fieldBO.LookupDisplayFieldId != -1 && lookupField != null) && (lookupField.Alias == "Structure" || lookupField.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE)))
                            {
                                string lkpStruct = string.Empty;
                                if (lookupField != null)
                                    lkpStruct = fieldBO.Alias + "-";
                                UltraTreeNode molWeightNode = (UltraTreeNode)fieldNode.Clone();
                                molWeightNode.Tag = new SelectDataForm.TreeNodeFieldContext(tableBO, fieldBO, _dataViewManagerBO.DataViewId, lookupField, typeof(ResultsCriteria.MolWeight));
                                molWeightNode.Text = lkpStruct + "Mol Weight";
                                tableNode.Nodes.Add(molWeightNode);

                                UltraTreeNode formulaNode = (UltraTreeNode)fieldNode.Clone();

                                formulaNode.Tag = new SelectDataForm.TreeNodeFieldContext(tableBO, fieldBO, _dataViewManagerBO.DataViewId, lookupField, typeof(ResultsCriteria.Formula));

                                formulaNode.Text = lkpStruct + "Formula";
                                tableNode.Nodes.Add(formulaNode);
                            }
                        }
                    }

                    if (addTableNode)
                    {
                        baseTableNode.Nodes.Add(tableNode);
                        PopulateCategoryNode(tableNode, tables, tableRelationShipCollection);
                    }
                    else
                    {
                        if (tableRelationships.ChildTableRelationships != null)
                        {
                            PopulateCategoryNode(tableNode, tables, tableRelationShipCollection);
                            if (addTableNode || tableNode.HasNodes)
                            {
                                baseTableNode.Nodes.Add(tableNode);
                            }
                        }
                    }
                }
            }
        }

        private void PopulateCategoryNode(UltraTreeNode baseTableNode, IEnumerable<TableBO> tables, HashSet<int> relatedTableIds, HashSet<TableBO> unusedTables)
        {
            string searchText = this.searchTextBox.Text.ToUpper();

            foreach (TableBO tableBO in tables)
            {
                // Skip any table that isn't ultimately related to the base table.
                if (!relatedTableIds.Contains(tableBO.ID))
                {
                    continue;
                }

                //do not add base table node again
                if (tableBO.ID == this.baseTableId)
                {
                    continue;
                }

                string tableAlias = tableBO.Alias;
                if (this._tableFilter != null && this.ParentForm != null && this.ParentForm is SelectDataForm)
                {
                    if (!this._tableFilter.IncludeTable(this.ParentForm as SelectDataForm, tableBO))
                    {
                        continue;
                    }
                    tableAlias = this._tableFilter.TableDisplayName(this.ParentForm as SelectDataForm, tableBO);
                }

                UltraTreeNode tableNode = new UltraTreeNode(null, tableAlias);
                tableNode.Tag = tableBO;

                //added condition to check if table bo contains the search text as table tag
                bool addTableNode = (string.IsNullOrEmpty(searchText) || tableBO.Alias.ToUpper().Contains(searchText) || IsTagContainsSearchText(searchText, tableBO));
                bool addAllFieldNodes = addTableNode;

                foreach (FieldBO fieldBO in tableBO.Fields)
                {
                    if (!fieldBO.Visible)
                    {
                        continue;
                    }

                    if (addAllFieldNodes || fieldBO.Alias.ToUpper().Contains(searchText))
                    {
                        addTableNode = true;
                        UltraTreeNode fieldNode = tableNode.Nodes.Add(null, fieldBO.Alias);
                        FieldBO lookupField = null;
                        if (fieldBO.LookupDisplayFieldId > 0)
                            lookupField = _dataViewManagerBO.Tables.GetField(fieldBO.LookupDisplayFieldId);
                        fieldNode.Tag = new SelectDataForm.TreeNodeFieldContext(tableBO, fieldBO, _dataViewManagerBO.DataViewId, lookupField);

                        //remove bold display of default fields in treeview
                        //if (fieldBO.IsDefault)
                        //{
                        //    fieldNode.Override.NodeAppearance.FontData.Bold = DefaultableBoolean.True;
                        //}
                        int imageIndex = GetImageIndex(fieldBO, lookupField);
                        if (imageIndex > 0)
                        {
                            fieldNode.LeftImages.Add(this.availableFieldsImageList.Images[imageIndex]);
                        }
                        if ((fieldBO.Alias == "Structure" || fieldBO.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE) || ((fieldBO.LookupDisplayFieldId != -1 && lookupField != null) && (lookupField.Alias == "Structure" || lookupField.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE)))
                        {
                            string lkpStruct = string.Empty;
                            if (lookupField != null)
                                lkpStruct = fieldBO.Alias + "-";
                            UltraTreeNode molWeightNode = (UltraTreeNode)fieldNode.Clone();
                            molWeightNode.Tag = new SelectDataForm.TreeNodeFieldContext(tableBO, fieldBO, _dataViewManagerBO.DataViewId, lookupField, typeof(ResultsCriteria.MolWeight));
                            molWeightNode.Text = lkpStruct + "Mol Weight";
                            tableNode.Nodes.Add(molWeightNode);

                            UltraTreeNode formulaNode = (UltraTreeNode)fieldNode.Clone();
                            formulaNode.Tag = new SelectDataForm.TreeNodeFieldContext(tableBO, fieldBO, _dataViewManagerBO.DataViewId, lookupField, typeof(ResultsCriteria.Formula));
                            formulaNode.Text = lkpStruct + "Formula";
                            tableNode.Nodes.Add(formulaNode);
                        }
                    }
                }

                if (addTableNode)
                {
                    //categoryNode.Nodes.Add(tableNode);
                    baseTableNode.Nodes.Add(tableNode);
                    if (unusedTables != null)
                    {
                        unusedTables.Remove(tableBO);
                    }
                }
            }
        }

        /// <summary>
        /// Method to add base table node from dataview as parent node in treeview
        /// </summary>
        /// <param name="baseTableBO">table bo object of base table</param>
        /// <returns>returns the parent tree node corresponding to base table</returns>
        private UltraTreeNode AddBaseTableNode(TableBO baseTableBO)
        {
            string searchText = this.searchTextBox.Text.ToUpper();
            UltraTreeNode tableNode = new UltraTreeNode(null, baseTableBO.Alias);
            tableNode.Tag = baseTableBO;
            bool addTableNode = (string.IsNullOrEmpty(searchText) || baseTableBO.Alias.ToUpper().Contains(searchText));
            bool addAllFieldNodes = addTableNode;

            foreach (FieldBO fieldBO in baseTableBO.Fields)
            {
                if (!fieldBO.Visible)
                {
                    continue;
                }

                if (addAllFieldNodes || fieldBO.Alias.ToUpper().Contains(searchText))
                {
                    addTableNode = true;
                    UltraTreeNode fieldNode = tableNode.Nodes.Add(null, fieldBO.Alias);
                    FieldBO lookupField = null;
                    if (fieldBO.LookupDisplayFieldId > 0)
                        lookupField = _dataViewManagerBO.Tables.GetField(fieldBO.LookupDisplayFieldId);
                    fieldNode.Tag = new SelectDataForm.TreeNodeFieldContext(baseTableBO, fieldBO, _dataViewManagerBO.DataViewId, lookupField);

                    int imageIndex = GetImageIndex(fieldBO, lookupField);
                    if (imageIndex > 0)
                    {
                        fieldNode.LeftImages.Add(this.availableFieldsImageList.Images[imageIndex]);
                    }
                    if ((fieldBO.Alias == "Structure" || fieldBO.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE) || ((fieldBO.LookupDisplayFieldId != -1 && lookupField != null) && (lookupField.Alias == "Structure" || lookupField.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE)))
                    {
                        string lkpStruct = string.Empty;
                        if (lookupField != null)
                            lkpStruct = fieldBO.Alias + "-";
                        UltraTreeNode molWeightNode = (UltraTreeNode)fieldNode.Clone();
                        molWeightNode.Tag = new SelectDataForm.TreeNodeFieldContext(baseTableBO, fieldBO, _dataViewManagerBO.DataViewId, lookupField, typeof(ResultsCriteria.MolWeight));
                        molWeightNode.Text = lkpStruct + "Mol Weight";
                        tableNode.Nodes.Add(molWeightNode);

                        UltraTreeNode formulaNode = (UltraTreeNode)fieldNode.Clone();
                        formulaNode.Tag = new SelectDataForm.TreeNodeFieldContext(baseTableBO, fieldBO, _dataViewManagerBO.DataViewId, lookupField, typeof(ResultsCriteria.Formula));
                        formulaNode.Text = lkpStruct + "Formula";
                        tableNode.Nodes.Add(formulaNode);
                    }
                }
            }
            return tableNode;
        }

        private void RestoreExpansionState()
        {
            RestoreExpansionState(this.availableFieldsTreeView.Nodes);
        }

        private void RestoreExpansionState(TreeNodesCollection nodes)
        {
            foreach (UltraTreeNode node in nodes)
            {
                if (node.Nodes.Count > 0)
                {
                    if (node.Tag != null)
                    {
                        bool expanded;
                        if (this._expansionState.TryGetValue(node.Tag, out expanded))
                        {
                            node.Expanded = expanded;
                        }
                    }
                    RestoreExpansionState(node.Nodes);
                }
            }
        }

        private void UpdateExpansionState(TreeNodesCollection nodes)
        {
            foreach (UltraTreeNode node in nodes)
            {
                if (node.Nodes.Count > 0)
                {
                    if (node.Tag != null)
                    {
                        this._expansionState[node.Tag] = node.Expanded;
                    }
                    UpdateExpansionState(node.Nodes);
                }
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Rebuilds the tree of tables and fields available to select from.
        /// </summary>
        internal void RebuildTreeView(COEDataViewManagerBO dvm)
        {
            _dataViewManagerBO = dvm;
            SaveExpansionState();

            this.Cursor = Cursors.WaitCursor;
            this.availableFieldsTreeView.BeginUpdate();
            try
            {
                this.availableFieldsTreeView.Nodes.Clear();
                if (this._dataViewManagerBO == null)
                {
                    return;
                }

                baseTableId = this._dataViewManagerBO.BaseTableId;

                //HashSet<int> relatedTableIds = GetRelatedTableIds();
                TableRelationships tableRelationships = GetRelatedTableIdRelations();
                //use comparison based on table id instead of table name
                HashSet<TableBO> unusedTables = new HashSet<TableBO>(this._dataViewManagerBO.Tables, new TableBoComparer());

                if (unusedTables.Count > 0)
                {
                    //detect the base table from tables collection and add as first node in treeview
                    TableBO baseTableBO = unusedTables.FirstOrDefault(t => t.ID == this.baseTableId);
                    UltraTreeNode baseTableNode = AddBaseTableNode(baseTableBO);
                    PopulateCategoryNode(baseTableNode, unusedTables, tableRelationships);

                    if (baseTableNode.Nodes.Count > 0)
                    {
                        this.availableFieldsTreeView.Nodes.Add(baseTableNode);
                    }
                }
                RestoreExpansionState();
            }
            finally
            {
                this.availableFieldsTreeView.EndUpdate();
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Looks up the appropriate index into the field type ImageList for a given field.
        /// </summary>
        /// <param name="fieldBO">The field to look up</param>
        /// <param name="lookupField">lookup field object</param>
        /// <returns>The appropriate index into the field type ImageList for the field</returns>
        internal int GetImageIndex(FieldBO fieldBO, FieldBO lookupField)
        {
            if (fieldBO.LookupDisplayFieldId != -1 && lookupField != null)
            {
                return GetImageIndex(lookupField, null);
            }
            else if (fieldBO.Alias == "Structure" || fieldBO.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE)
            {
                return 1;
            }
            else
            {
                switch (fieldBO.DataType)
                {
                    case CambridgeSoft.COE.Framework.Common.COEDataView.AbstractTypes.Date:
                        return 2;
                    case CambridgeSoft.COE.Framework.Common.COEDataView.AbstractTypes.Boolean:
                        return 3;
                    case CambridgeSoft.COE.Framework.Common.COEDataView.AbstractTypes.Integer:
                        return 4;
                    case CambridgeSoft.COE.Framework.Common.COEDataView.AbstractTypes.Real:
                        return 5;
                    case CambridgeSoft.COE.Framework.Common.COEDataView.AbstractTypes.Text:
                        return 6;
                }
            }
            return 0;
        }

        #endregion

        #region Trreview Events

        void availableFieldsTreeView_DoubleClick(object sender, EventArgs e)
        {
            UltraTreeNode node = this.availableFieldsTreeView.UIElement.LastElementEntered.GetContext(typeof(UltraTreeNode)) as UltraTreeNode;

            if (node != null)
            {
                if (this.ParentForm != null && this.ParentForm is SelectDataForm)
                {
                    if (node.Tag is SelectDataForm.FieldContext)
                    {
                        (this.ParentForm as SelectDataForm).AppendField((SelectDataForm.FieldContext)node.Tag);
                    }
                }
            }
        }

        void availableFieldsTreeView_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && this.ParentForm != null && this.ParentForm is SelectDataForm)
            {
                UltraTreeNode nodeUnder = this.availableFieldsTreeView.GetNodeFromPoint(e.Location);
                if (nodeUnder != null)
                {
                    this.availableFieldsTreeView.ActiveNode = nodeUnder;
                    if (nodeUnder.Tag is TableBO)
                    {
                        this.tableContextMenuStrip.Show(this.availableFieldsTreeView.PointToScreen(e.Location));
                    }
                    else if (nodeUnder.Tag is SelectDataForm.FieldContext)
                    {
                        this.fieldContextMenuStrip.Show(this.availableFieldsTreeView.PointToScreen(e.Location));
                    }
                }
            }
        }

        private void availableFieldsTreeView_SelectionDragStart(object sender, System.EventArgs e)
        {
            IList<TableBO> tableData = null;
            IList<SelectDataForm.FieldContext> fieldData = null;
            foreach (UltraTreeNode node in this.availableFieldsTreeView.SelectedNodes)
            {
                if (node.Tag is TableBO)
                {
                    if (tableData == null)
                    {
                        tableData = new List<TableBO>();
                    }
                    tableData.Add((TableBO)node.Tag);
                }
                else if (node.Tag is SelectDataForm.FieldContext)
                {
                    if (fieldData == null)
                    {
                        fieldData = new List<SelectDataForm.FieldContext>();
                    }
                    fieldData.Add((SelectDataForm.FieldContext)node.Tag);
                }
            }

            if (tableData != null)
            {
                this.availableFieldsTreeView.DoDragDrop(tableData, DragDropEffects.Copy);
            }
            else if (fieldData != null)
            {
                this.availableFieldsTreeView.DoDragDrop(fieldData, DragDropEffects.Copy);
            }
        }

        private void availableFieldsTreeView_DragDrop(object sender, DragEventArgs e)
        {
            if (this.ParentForm != null && this.ParentForm is SelectDataForm)
            {
                if (e.Data.GetDataPresent(typeof(QueryFieldPanel)) || e.Data.GetDataPresent(typeof(ResultsFieldPanel)))
                {
                    FieldPanel fieldPanel = (FieldPanel)e.Data.GetData(typeof(QueryFieldPanel));
                    if (fieldPanel == null)
                    {
                        fieldPanel = (FieldPanel)e.Data.GetData(typeof(ResultsFieldPanel));
                    }
                    if (fieldPanel != null)
                    {
                        (this.ParentForm as SelectDataForm).RemoveField(fieldPanel);
                    }
                }
                else if (e.Data.GetDataPresent(typeof(TablePanel)))
                {
                    TablePanel tablePanel = (TablePanel)e.Data.GetData(typeof(TablePanel));
                    (this.ParentForm as SelectDataForm).RemoveTable(tablePanel);
                }
            }
        }

        private void availableFieldsTreeView_DragOver(object sender, DragEventArgs e)
        {
            //added separate conditions for QueryFieldPanel and ResultsFieldPanel, as the GetDataPresent method does not understand the base object FieldPanel
            if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                if (e.Data.GetDataPresent(typeof(QueryFieldPanel)) || e.Data.GetDataPresent(typeof(ResultsFieldPanel)))
                {
                    e.Effect = DragDropEffects.Move;
                }
                else if (e.Data.GetDataPresent(typeof(TablePanel)))
                {
                    TablePanel tablePanel = (TablePanel)e.Data.GetData(typeof(TablePanel));
                    e.Effect = (tablePanel != null) ? (((this.ParentForm as SelectDataForm)._dataViewManagerBO.BaseTableId != tablePanel.tableBO.ID) ? DragDropEffects.Move : DragDropEffects.None) : DragDropEffects.None;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        #endregion

        #region Menu Events

        /// <summary>
        /// Selects a table along with its default fields from a context menu click.
        /// </summary>
        private void addDefaultFieldsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AppendTableFields(SelectDataForm.TableSelectionMethod.DefaultFields);
        }

        /// <summary>
        /// Selects a table along with all its fields from a context menu click.
        /// </summary>
        private void addAllFieldsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AppendTableFields(SelectDataForm.TableSelectionMethod.AllFields);
        }

        /// <summary>
        /// Selects a table along with its unique key fields and structure from a context menu click.
        /// </summary>
        private void addUniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AppendTableFields(SelectDataForm.TableSelectionMethod.UniqueKeyAndStructure);
        }

        /// <summary>
        /// Selects a single field from a context menu click.
        /// </summary>
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ParentForm != null && this.ParentForm is SelectDataForm)
            {
                UltraTreeNode node = availableFieldsTreeView.ActiveNode;
                if (node != null)
                {
                    //call AppendField method from parent form i.e. SelectDataForm
                    (this.ParentForm as SelectDataForm).AppendField((SelectDataForm.FieldContext)node.Tag);
                }
            }
        }

        /// <summary>
        /// Common method for table context menu
        /// </summary>
        /// <param name="tsm"></param>
        private void AppendTableFields(SelectDataForm.TableSelectionMethod tsm)
        {
            if (this.ParentForm != null && this.ParentForm is SelectDataForm)
            {
                UltraTreeNode node = availableFieldsTreeView.ActiveNode;
                if (node != null)
                {
                    TableBO tableBO = node.Tag as TableBO;
                    if (tableBO != null)
                    {
                        (this.ParentForm as SelectDataForm).AppendTableFields(tableBO, tsm);
                    }
                }
            }
        }

        public void ClearSearchText()
        {
            this.searchTextBox.Text = string.Empty;
        }

        #endregion

        #region Events

        /// <summary>
        /// Rebuilds the tree of tables and fields when the search text is changed.
        /// </summary>
        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            this.RebuildTreeView(this._dataViewManagerBO);
            //this.clearButton.Enabled = (this.searchTextBox.Text.Length > 0);
        }

        #endregion

        /// <summary>
        /// Class to compare table object using id instaed of default name comparison
        /// </summary>
        class TableBoComparer : IEqualityComparer<TableBO>
        {
            /// <summary>
            /// Determines whether specified table object has same value as the table id
            /// </summary>
            /// <param name="x">table object to compare</param>
            /// <param name="y">table bo to compare with</param>
            /// <returns>returns true if the id value of x is the same as the id value of y; otherwise, false.</returns>
            public bool Equals(TableBO x, TableBO y)
            {
                if (x == null || y == null)
                    return false;
                if (x.ID == y.ID)
                {
                    return true;
                }
                return false;
            }

            public int GetHashCode(TableBO obj)
            {
                if (obj == null)
                    return 0;
                return obj.GetHashCode();
            }
        }


        /// <summary>
        /// class to compare the tags in table with search text
        /// </summary>
        class TagComparer : IEqualityComparer<string>
        {
            /// <summary>
            /// Determines whether specified search text has same value as the table tag
            /// </summary>
            /// <param name="x">search text to compare</param>
            /// <param name="y">table tag to compare with</param>
            /// <returns>returns true if the value of x is the same as the value of y; otherwise, false.</returns>
            public bool Equals(string x, string y)
            {
                return x.Equals(y, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string obj)
            {
                if (obj == null)
                {
                    return 0;
                }
                return obj.GetHashCode();
            }
        }

        /// <summary>
        /// Class to hold the table id relationships
        /// </summary>
        class TableRelationships
        {
            #region Variables
            bool isContains;
            TableRelationships returnTableRelationships = null;
            List<TableRelationships> childTableRelationships;
            int tableId = 0;
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets the child table collections
            /// </summary>
            public List<TableRelationships> ChildTableRelationships
            {
                get { return childTableRelationships; }
                set { childTableRelationships = value; }
            }

            /// <summary>
            /// Gets or sets the current table id
            /// </summary>
            public int TableId
            {
                get { return tableId; }
                set { tableId = value; }
            }
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes an instance of the TableRelationships object
            /// </summary>
            public TableRelationships()
            {
                childTableRelationships = new List<TableRelationships>();
            }
            #endregion

            /// <summary>
            /// Adds child table id to its parent table id in the hierarchy
            /// </summary>
            /// <param name="tableId">current table id which will be parent</param>
            /// <param name="childTableId">child table id to be added in hierarchy</param>
            public void AddChild(int tableId, int childTableId)
            {
                TableRelationships tableRel = this[tableId];
                if (tableRel != null)
                {
                    TableRelationships tblRel = new TableRelationships();
                    tblRel.TableId = childTableId;
                    tableRel.childTableRelationships.Add(tblRel);
                }
                else
                {
                    TableRelationships tblRel = new TableRelationships();
                    if (this.tableId > 0)
                    {
                        tblRel.tableId = tableId;
                        tblRel.ChildTableRelationships.Add(new TableRelationships() { TableId = childTableId });
                    }
                    else
                    {
                        this.tableId = tableId;
                        tblRel.TableId = childTableId;
                    }
                    this.childTableRelationships.Add(tblRel);
                }
            }

            /// <summary>
            /// Checks if the table id is present in the hierarchy of the table id collection
            /// </summary>
            /// <param name="tableId">table id to search for in collection</param>
            /// <returns>returns true if the table id is present; otherwise false</returns>
            public bool Contains(int tableId)
            {
                isContains = false;
                if (this.tableId == tableId)
                {
                    isContains = true;
                }
                isContains = Contains(tableId, this.ChildTableRelationships);
                return isContains;
            }

            /// <summary>
            /// Gets the TableRelatioships object from the hierarchy corresponding to the specified table id
            /// </summary>
            /// <param name="tableId">table id to search for</param>
            /// <returns>returns TableRelationships object from collection for table id; otherwise null</returns>
            public TableRelationships this[int tableId]
            {
                get
                {
                    returnTableRelationships = null;
                    if (this.TableId == tableId)
                    {
                        return this;
                    }
                    returnTableRelationships = GetTable(tableId, this);
                    return returnTableRelationships;
                }
            }

            bool Contains(int tableId, List<TableRelationships> childTableRelationships)
            {
                if (childTableRelationships == null)
                {
                    return false;
                }

                foreach (TableRelationships tableRel in childTableRelationships)
                {
                    if (tableRel.TableId == tableId)
                    {
                        isContains = true;
                    }
                    if (!isContains)
                    {
                        Contains(tableId, tableRel.ChildTableRelationships);
                    }
                }
                if (isContains)
                    return true;
                else
                    return false;
            }

            TableRelationships GetTable(int tableId, TableRelationships table)
            {
                if (table == null)
                {
                    return null;
                }
                foreach (TableRelationships tableRel in table.ChildTableRelationships)
                {
                    if (tableRel.TableId == tableId)
                    {
                        returnTableRelationships = tableRel;
                        break;
                    }
                    GetTable(tableId, tableRel);
                }
                return returnTableRelationships;
            }
        }
    }
}
