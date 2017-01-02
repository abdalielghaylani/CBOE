using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using CambridgeSoft.COE.Framework.COEDataViewService;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;

namespace ChemBioVizExcelAddIn
{
    partial class frmDataviewList :Form
    { 
        private Dictionary<string, COEDataViewBO> DataViewList;
        ThisAddIn parent;
        bool IsExisting = false;

        public frmDataviewList(ThisAddIn parent)
        {
            InitializeComponent();
            InitializeEvents();
            this.parent = parent;
            DataViewList = null;          
        }
        public frmDataviewList()
        {
            InitializeComponent();
            InitializeEvents();          
            DataViewList = null;           
        }
        public frmDataviewList( bool isExisting)
        {
            InitializeComponent();
            InitializeEvents();
            DataViewList = null;
            IsExisting = isExisting;
        }

        private void InitializeEvents()
        {
            this.btnOk.Click+=new EventHandler(btnOk_Click);            
            this.cmbCOEDataViewBOList.SelectedIndexChanged+=new EventHandler(cmbCOEDataViewBOList_SelectedIndexChanged);
            this.cmbCOEDataViewBOList.KeyPress+=new KeyPressEventHandler(cmbCOEDataViewBOList_KeyPress);
            this.Load += new EventHandler(frmDataviewList_Load);
            this.FormClosed+=new FormClosedEventHandler(frmDataviewList_FormClosed);
        }

        #region "Events"
     
        //11.0.3
        private void btnOk_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            UserInfo userInfo = new UserInfo();
            try
            {
                if (IsExisting == true)
                {
                    if (cmbCOEDataViewBOList.SelectedIndex > 0)
                    {
                        Global.CBVSHEET_COEDATAVIEWBO = DataViewList[cmbCOEDataViewBOList.SelectedIndex.ToString()];
                        Global.COEDATAVIEW_INDEX = cmbCOEDataViewBOList.SelectedIndex.ToString();
                        //11.0.3 Retrieve all the dataview tables
                        Global.DVTables = TableListBO.NewTableListBO(Global.CBVSHEET_COEDATAVIEWBO.COEDataView.Tables);

                        Global.RestoreWindowsPrincipal();
                        //Hide the current form
                        this.Hide();

                        //For hour glass settings, the user information is updated instead of inside login form                    
                        userInfo.UpdateUserInfo(Global.CurrentUser);
                        Excel::Worksheet nSheet = Global._ExcelApp.ActiveSheet as Excel.Worksheet;
                        //Coverity fix - CID 18750
                        if (nSheet == null)
                            throw new System.NullReferenceException();
                        Global.CurrentWorkSheetName = nSheet.Name;
                        Global.CellDropdownRange.Clear();
                        Global.SearchUpdateCriteria = new List<object>();
                        //Set the dialog result
                        this.DialogResult = DialogResult.OK;
                    }
                }
                else
                {
                    if (cmbCOEDataViewBOList.SelectedIndex > 0)
                    {
                        Global.CBVSHEET_COEDATAVIEWBO = DataViewList[cmbCOEDataViewBOList.SelectedIndex.ToString()];
                        Global.COEDATAVIEW_INDEX = cmbCOEDataViewBOList.SelectedIndex.ToString();

                       //11.0.3 Retrieve all the dataview tables
                        Global.DVTables = TableListBO.NewTableListBO(Global.CBVSHEET_COEDATAVIEWBO.COEDataView.Tables);

                        Global.RestoreWindowsPrincipal();
                        //this.Visible = false;
                       
                        frmDataviewSchema objFrmdvschema = new frmDataviewSchema();
                        if (objFrmdvschema.ShowDialog() == DialogResult.OK)
                        {
                            //For hour glass settings, the user information is updated instead of inside login form                    
                            userInfo.UpdateUserInfo(Global.CurrentUser);

                            //Create a new chemoffice sheet and convert to sar sheet
                            Global.NewChemOfficeSheet();

                            ChemBioVizExcelAddIn.CBVExcel CBVExcel = new ChemBioVizExcelAddIn.CBVExcel();
                            Excel::Worksheet nSheet = Global._ExcelApp.ActiveSheet as Excel.Worksheet;
                            //Set the sheet name
                            Global.CurrentWorkSheetName = nSheet.Name;

                            //Clear the cell drop down list. It clear before the create CBV sheet because the defualt column nos are inserted during CBV sheet creation.
                            Global.CellDropdownRange.Clear();

                            if (parent != null)
                            {
                                //Disable the worksheet event and create the CBV sheet
                                Global._ExcelApp.ScreenUpdating = false;
                                parent.DeAttachWorkSheetChangeEvent();
                                //Create the CBV Sheet
                                CBVExcel.CreateCBVSheet(nSheet);
                                parent.AttachWorkSheetChangeEvent();
                                Global._ExcelApp.ScreenUpdating = true;
                                //((Microsoft.Office.Interop.Excel._Worksheet)nSheet).Activate();
                            }
                            else
                            {
                                CBVExcel.CreateCBVSheet(nSheet);
                            }
                            //Initalize the searchUpdateCriteria
                            Global.SearchUpdateCriteria = new List<object>();

                            //Set the dialog result
                            
                            this.DialogResult = DialogResult.OK;
                        }
                        else
                        {
                            Global.CBVSHEET_COEDATAVIEWBO = null;
                            this.DialogResult = DialogResult.None;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Global._ExcelApp.ScreenUpdating = true;
                MessageBox.Show(ex.Message, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                Global._ExcelApp.ScreenUpdating = true;
                this.Close();
            }
        }


        private void frmDataviewList_Load(object sender, EventArgs e)
        {
            //Set CSLA principal
            Global.RestoreCSLAPrincipal();
            int i = 0;
            Cursor.Current = Cursors.WaitCursor;
            COEDataViewBOList dataViews=null;
            try
            {
                cmbCOEDataViewBOList.Items.Clear();
                DataViewList = new Dictionary<string, COEDataViewBO>();
                dataViews = COEDataViewBOList.GetDataViewListAndNoMaster(); 
                cmbCOEDataViewBOList.Items.Add(Properties.Resources.txtSelectDV);
                if (dataViews.Count > 0)
                {
                    foreach (COEDataViewBO dv in dataViews)
                    {
                        cmbCOEDataViewBOList.Items.Add(dv.Name);
                        ++i;
                        DataViewList.Add(i.ToString(), dv);
                    }
                }               
            }
            catch(Exception ex) 
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
            finally
            {              
                if (dataViews != null)
                    dataViews.Clear();
                Cursor.Current = Cursors.Default;
            }
        }

        private void cmbCOEDataViewBOList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void frmDataviewList_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Restore to window principal
            Global.RestoreWindowsPrincipal();
        }
        private void cmbCOEDataViewBOList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                this.btnOk_Click(sender, e);
        }
        #endregion "Events"
    }
}
