using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COESearchService;

using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;

namespace ChemBioVizExcelAddIn
{
    //11.0.3
    public partial class frmDataviewSchema : Form
    {
        private int dvID;
        public frmDataviewSchema()
        {
            InitializeComponent();
            InitializeEvents();
        }
        public frmDataviewSchema(int dvID)
        {
            InitializeComponent();
            InitializeEvents();
            this.dvID = dvID;
        }

        private void InitializeEvents()
        {
            this.Load+=new EventHandler(frmDataviewSchema_Load);
            tvDataview.AfterCheck+=new TreeViewEventHandler(tvDataview_AfterCheck);
            this.btnOK.Click+=new EventHandler(btnOK_Click);
            this.FormClosed+=new FormClosedEventHandler(frmDataviewSchema_FormClosed);         
        }

        #region "Events"

        private void frmDataviewSchema_Load(object sender, EventArgs e)
        {
            try
            {
                //Remove non-relational tabes from dataview. If any tables doesn't have reationship with base table then it would not be the part of the dataview.
                RemoveNonRelationalTables(Global.CBVSHEET_COEDATAVIEWBO.COEDataView);

                TableListBO tableListBO = TableListBO.NewTableListBO(Global.CBVSHEET_COEDATAVIEWBO.COEDataView.Tables);

                TableBO baseTable = tableListBO.GetTable(Global.CBVSHEET_COEDATAVIEWBO.COEDataView.Basetable);

                //*** The base table would be the first table in tree
                TreeNode tnDatabaseB = new TreeNode(baseTable.DataBase);
                tnDatabaseB.Name = baseTable.DataBase;
                tvDataview.Nodes.Add(tnDatabaseB);
                TreeNode tnTableB = new TreeNode(baseTable.Alias);
                tnTableB.Name = baseTable.Name;
                tvDataview.Nodes[baseTable.DataBase].Nodes.Add(tnTableB);
                //ArrangeFieldsRescurive(baseTable);                
                foreach (string fieldAlias in ArrangeFields(baseTable))
                {
                    TreeNode tnField1 = new TreeNode(fieldAlias);
                    tvDataview.Nodes[baseTable.DataBase].Nodes[baseTable.Name].Nodes.Add(tnField1);
                }
                //***

                //Remove base table from tablelistBO
                tableListBO.Remove(baseTable.ID);

                foreach (TableBO tableBO in tableListBO)
                {
                    TreeNode tnDatabase = new TreeNode(tableBO.DataBase);
                    tnDatabase.Name = tableBO.DataBase;
                    string tnDBKey = tableBO.DataBase;
                    if (!tvDataview.Nodes.ContainsKey(tnDBKey))
                    {
                        tvDataview.Nodes.Add(tnDatabase);
                    }
                    TreeNode tnTable = new TreeNode(tableBO.Alias);
                    tnTable.Name = tableBO.Name;
                    tvDataview.Nodes[tableBO.DataBase].Nodes.Add(tnTable);

                    foreach (FieldBO fieldBO in tableBO.Fields)
                    {
                        //Check whether the field visible property is true or not
                        if (fieldBO.Visible)
                        {
                            TreeNode tnField = new TreeNode(fieldBO.Alias);
                            tvDataview.Nodes[tableBO.DataBase].Nodes[tableBO.Name].Nodes.Add(tnField);
                            if ((fieldBO.IndexType.ToString().Equals(Global.COESTRUCTURE_INDEXTYPE, StringComparison.OrdinalIgnoreCase)) || (fieldBO.LookupFieldId > 0 && Global.DVTables.GetField(fieldBO.LookupDisplayFieldId).IndexType.ToString().Equals(Global.COESTRUCTURE_INDEXTYPE, StringComparison.OrdinalIgnoreCase))) //CSBR-154419
                            {  
                                tvDataview.Nodes[tableBO.DataBase].Nodes[tableBO.Name].Nodes.Add(fieldBO.Alias + ".MOLWEIGHT");
                                tvDataview.Nodes[tableBO.DataBase].Nodes[tableBO.Name].Nodes.Add(fieldBO.Alias + ".FORMULA");
                              
                            }                            
                        }                      
                    }
                }
               

                CheckedAndExpandBaseTable(tvDataview);

                lblDvname.Text = Global.CBVSHEET_COEDATAVIEWBO.COEDataView.Name;
                Global.RestoreWindowsPrincipal();

            }
            catch
            {
                Global.RestoreWindowsPrincipal();
            }
        }

        private void frmDataviewSchema_FormClosed(object sender, EventArgs e)
        {               
            this.Close();
        }

        Boolean bChildTrigger = true;
        Boolean bParentTrigger = true;
        private void tvDataview_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (bChildTrigger)
            {
                CheckAllChildren(e.Node, e.Node.Checked);
            }
            if (bParentTrigger)
            {
                CheckParent(e.Node, e.Node.Checked);
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            //Fixed CSBR - 152733 : Handled the maximum columns case in excel.
            Excel::Worksheet worksheet = Global._ExcelApp.ActiveSheet as Excel.Worksheet;
            int chkNodeCnt = GetCheckedNodesCount(tvDataview.Nodes);
            //Coverity fix - CID 18751
            if (worksheet == null)
                throw new System.NullReferenceException();
            if ((chkNodeCnt > worksheet.Columns.Count) && (worksheet.Columns.Count == Global.MaxOffice2003Columns))
            {
                MessageBox.Show(Properties.Resources.msgOffice2003MaxColumnReachedError, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            if ((chkNodeCnt > worksheet.Columns.Count) && (worksheet.Columns.Count == Global.MaxOffice2007Columns))
            {
                MessageBox.Show(Properties.Resources.msgOffice2007MaxColumnReachedError, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            if (ValidatePrimaryUniqueKey())
            {
                if (chkNodeCnt > 0)
                {
                    Global.TreeDataView = tvDataview;
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    this.DialogResult = DialogResult.No;
                }
                this.Close();
            }
            else
            {
                MessageBox.Show(Properties.Resources.msgCBVShemaPKUK, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion "Events"

        #region "Private Methods"

        private List<string> ArrangeFields(TableBO baseTable) //// for-each loop for conditions instead of recursive loop (Priority goes to Structure-> Primary key -> Unique key).

        {
            List<string> fieldList = new List<string>();

            try
            {
                foreach (FieldBO fieldBO in baseTable.Fields) //
                {
                    if ((fieldBO.IndexType.ToString().Equals(Global.COESTRUCTURE_INDEXTYPE, StringComparison.OrdinalIgnoreCase)) && (fieldBO.Visible))
                    {
                        fieldList.Add(fieldBO.Alias);
                    }
                }

                foreach (FieldBO fieldBO in baseTable.Fields)
                {
                    if (fieldBO.ID.Equals(baseTable.PrimaryKey) && (fieldBO.Visible))
                    {

                        fieldList.Add(fieldBO.Alias);
                    }
                }
                //add molweight and formula to checklist
                foreach (FieldBO fieldBO in baseTable.Fields) //
                {
                  if (((fieldBO.IndexType.ToString().Equals(Global.COESTRUCTURE_INDEXTYPE, StringComparison.OrdinalIgnoreCase)) || (fieldBO.LookupFieldId > 0) && (Global.DVTables.GetField(fieldBO.LookupDisplayFieldId).IndexType.ToString().Equals(Global.COESTRUCTURE_INDEXTYPE, StringComparison.OrdinalIgnoreCase))) && (fieldBO.Visible)) //Fixed CSBR-157171
                    {
                        fieldList.Add(fieldBO.Alias + ".MOLWEIGHT");
                        fieldList.Add(fieldBO.Alias + ".FORMULA");
                    }
                }

                foreach (FieldBO fieldBO in baseTable.Fields)
                {
                    if ((fieldBO.IsUniqueKey) && (fieldBO.Visible))
                    {
                        fieldList.Add(fieldBO.Alias);
                    }
                }

                foreach (FieldBO fieldBO in baseTable.Fields)
                {
                    if ((!fieldList.Contains(fieldBO.Alias)) && (fieldBO.Visible))
                    {
                        fieldList.Add(fieldBO.Alias);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return fieldList;
        }


        private void CheckParent(TreeNode tn, Boolean bCheck)
        {
            if (tn == null) return;
            if (tn.Parent == null) return;

            bChildTrigger = false;
            bParentTrigger = false;
            if (!bCheck)
                tn.Parent.Checked = IsChildCheck(tn.Parent);
            else
                tn.Parent.Checked = bCheck;
            CheckParent(tn.Parent, bCheck);
            bParentTrigger = true;
            bChildTrigger = true;
        }

        private void CheckAllChildren(TreeNode tn, Boolean bCheck)
        {
            bParentTrigger = false;
            foreach (TreeNode ctn in tn.Nodes)
            {
                bChildTrigger = false;
                ctn.Checked = bCheck;
                bChildTrigger = true;

                CheckAllChildren(ctn, bCheck);
            }
            bParentTrigger = true;
        }

        private bool IsChildCheck(TreeNode tnP)
        {
            foreach (TreeNode tnC in tnP.Nodes)
            {
                if (tnC.Checked)
                    return true;
            }
            return false;

        }
        /*
        private void LoadChecked(TreeNodeCollection tNodeList,  ArrayList checkedItems)
        {
            TreeView tv = new TreeView();
            foreach (TreeNode tndb in tvDataview.Nodes)
            {   
                if (tndb.Checked)
                {
                    foreach (TreeNode tnTab in tndb.Nodes)
                    {
                        if (tnTab.Checked)
                        {
                            foreach (TreeNode tnCol in tnTab.Nodes)
                            {
                                if (tnCol.Checked)
                                {
                                    string database = tndb.Text;
                                    string tabble = tnTab.Text;
                                    string colName = tnCol.Text;
                                }
                            }
                        }
                    }
                }
            }
        }*/
        private void CheckedAndExpandBaseTable(TreeView tvDV)
        {
            tvDV.GetNodeAt(0, 0);
            foreach (TreeNode tndb in tvDV.Nodes)
            {
                foreach (TreeNode tnTab in tndb.Nodes)
                    {
                       tnTab.ExpandAll();
                       tnTab.EnsureVisible();                        
                            foreach (TreeNode tnCol in tnTab.Nodes)
                            {
                                tnCol.Checked = true;
                                if (tnCol.Index == 1) // Index start with 0. As per the requirement the structure and pk/uk are selected by default ( First table is basetable, and first field would be structure then pk then uk).
                                    break; 
                            }
                            break; //because first table is basetable.
                    }
                    break; ////because first database contain the base.
            }
        }

        //Return only base table and relational child tables. If any tables doesn't have reationship with base table then it would not be the part of the dataview.
        /*
        private TableListBO GetRelationalTables(COEDataView coeDataView, ref TableBO baseTable)
        {
            COEDataView.Relationship[] relationshipArr = coeDataView.Relationships.ToArray();


            TableListBO tableListBO = TableListBO.NewTableListBO(coeDataView.Tables);
            baseTable = tableListBO.GetTable(coeDataView.Basetable);

            TableListBO TableListBORel = TableListBO.NewTableListBO();
            TableListBORel.Add(baseTable);

            foreach (COEDataView.Relationship relation in relationshipArr)
            {
                if (relation.Parent == baseTable.ID)
                    TableListBORel.Add(tableListBO.GetTable(relation.Child));
            }
            return TableListBORel;
        }

        private COEDataView GetRelationalTablesDataview(COEDataView coeDataView)
        {
            COEDataView dataView = new COEDataView();
            COEDataView.Relationship[] relationshipArr = coeDataView.Relationships.ToArray();
            TableListBO tableListBO = TableListBO.NewTableListBO(coeDataView.Tables);


            dataView.Tables.Add(coeDataView.Tables[coeDataView.BaseTableName]);

            foreach (COEDataView.Relationship relationship in relationshipArr)
            {
                TableBO tableBO = tableListBO.GetTable(relationship.Child);
                dataView.Tables.Add(coeDataView.Tables[tableBO.Name]);
            }
            return dataView;
        }*/


        private List<int> GetNonRelationalTablesIDs(COEDataView coeDataView)
        {
            List<int> idNotExists = new List<int>();
            foreach (COEDataView.DataViewTable table in coeDataView.Tables)
            {
                idNotExists.Add(table.Id);
            }
            idNotExists.Remove(coeDataView.Basetable);
            COEDataView.Relationship[] relationshipArr = coeDataView.Relationships.ToArray();
            IEnumerator relationshipEnum = coeDataView.Relationships.GetEnumerator();
            while (relationshipEnum.MoveNext())
            {
                COEDataView.Relationship relation = (COEDataView.Relationship)relationshipEnum.Current;
               // if (relation.Parent == coeDataView.Basetable)
                idNotExists.Remove(relation.Child);
            }
            return idNotExists;
        }


        #endregion "Private Methods"

        #region "Public Methods"
        public static int GetCheckedNodesCount(TreeNodeCollection nodes)
        {
            int checkedNodes = 0;

            for (int i = 0; i < nodes.Count; i++)
            {
                TreeNode node = nodes[i];
                if (node.Checked)
                    checkedNodes++;

                if (node.Nodes.Count > 0)
                    checkedNodes += GetCheckedNodesCount(node.Nodes);
            }
            return checkedNodes;
        }

        public bool ValidatePrimaryUniqueKey()
        {
            TableListBO Tables = TableListBO.NewTableListBO(Global.CBVSHEET_COEDATAVIEWBO.COEDataView.Tables); 
            TableBO baseTable = Tables.GetTable(Global.CBVSHEET_COEDATAVIEWBO.COEDataView.Basetable);                    

            try
            {
                foreach (TreeNode tnDatabase in tvDataview.Nodes)
                {
                    if ((tnDatabase.Checked) && (tnDatabase.Text.Equals(baseTable.DataBase, StringComparison.OrdinalIgnoreCase)))
                    {
                        foreach (TreeNode tnTable in tnDatabase.Nodes)
                        {
                            if ((tnTable.Checked) && (tnTable.Text.Equals(baseTable.Alias, StringComparison.OrdinalIgnoreCase)))
                            {
                                foreach (TreeNode tnField in tnTable.Nodes)
                                {
                                    if (tnField.Checked)
                                    {
                                        foreach (FieldBO fieldBO in baseTable.Fields)
                                        {
                                            if (tnField.Text.Equals(fieldBO.Alias, StringComparison.OrdinalIgnoreCase))
                                            {
                                                if (fieldBO.IsUniqueKey)
                                                    return true;
                                                // else if (fieldBO.Alias.Equals(pkAlias))
                                                else if (fieldBO.ID.Equals(baseTable.PrimaryKey))
                                                    return true;
                                                else
                                                    continue;
                                            }
                                        }
                                    } // end if field check
                                }
                            }//end if table checked
                        }
                    } // end if database checked
                }
            }
                
            catch 
            {
                return false;
            }
                
            finally
            {
                if (Tables != null)
                {
                    Tables = null;
                    if (baseTable != null)
                        baseTable = null;
                }
                
            }
            return false;
        }



        private void RemoveNonRelationalTables(COEDataView coeDataView)
        {
            foreach (int nonRelTabID in GetNonRelationalTablesIDs(coeDataView))
            {
                COEDataView.DataViewTable dvTable = coeDataView.Tables.getById(nonRelTabID);
                coeDataView.Tables.Remove(dvTable);
            }

        }
        #endregion "Public Methods"
    }
}