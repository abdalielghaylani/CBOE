using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.DataLoaderGUI.Common;
using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.DataLoaderGUI.Forms;
using CambridgeSoft.COE.DataLoader.Core.FileParser.SD;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using System.IO;
using CambridgeSoft.DataLoaderGUI.Properties;
using ChemControls;
using Infragistics.Win.UltraWinGrid;

namespace CambridgeSoft.DataLoaderGUI.Controls
{
    public partial class DisplayInputData : UIBase
    {
        private Dictionary<ISourceRecord, string> records;        
        private DataSet _result = new DataSet();
        private JobParameters _job;
        public string _fName;        
        private ChemDataGrid cdGrid;
        private const string _selectedColorName = "GradientActiveCaption";

        public JobParameters JOB
        {
            set { this._job = value; }
        }

        public string FileName
        {
            get
            {
                return _fName;
            }
        }

        public Dictionary<ISourceRecord, string> InputData
        {
            set { this.records = value; }
        }

        public DataSet Data
        {
            set
            {
                _result = value;                
                //create ultradatasource here
                //Infragistics.Win.UltraWinDataSource.UltraDataSource uds = new Infragistics.Win.UltraWinDataSource.UltraDataSource();
                //uds.CellDataRequested += new Infragistics.Win.UltraWinDataSource.CellDataRequestedEventHandler(uds_CellDataRequested);
                //uds.LoadFromBinary
                //uds.LoadFromXml
                cdGrid = new ChemDataGrid();
                //cdGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
                cdGrid.DisplayLayout.Override.DefaultRowHeight = 50;
                //cdGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
                cdGrid.DisplayLayout.LoadStyle = LoadStyle.LoadOnDemand;
                cdGrid.DisplayLayout.Override.RowAppearance.BackColor = Color.White;
                cdGrid.MouseUp += cdGrid_MouseUp;
                cdGrid.MouseDown += new MouseEventHandler(cdGrid_MouseDown);
                cdGrid.Dock = DockStyle.Fill;
                SortChemGrid(string.Empty);
            }
        }

        #region Grid Sorting

        protected Infragistics.Win.UltraWinGrid.ColumnHeader m_clickedGridHeader;
        private void cdGrid_MouseDown(object sender, MouseEventArgs e)
        {
            // user clicked on a grid; see if it was a right-click on a column header
            // this handler subscribes to the grid mouseDown event

            m_clickedGridHeader = null;
            if (sender is ChemDataGrid)
            {
                ChemDataGrid cdg = sender as ChemDataGrid;
                m_clickedGridHeader = GetClickedHeader(cdg, e);

                if (e.Button == MouseButtons.Right && m_clickedGridHeader != null)
                {
                    // rclick on header: show the context menu
                    Point pScreen = cdg.PointToScreen(new Point(e.X, e.Y));
                    UltraGridColumn ugCol = m_clickedGridHeader.Column;

                    // CSBR-128730: prevent any menu on header of child band within grid view
                    bool bIsChildBand = ugCol.Band.Index > 0;
                    if (true) // (!bIsChildBand)    CSBR-133388: permit rename header in child grids
                    {
                        // hide sort menu on structure col
                        bool bIsStructCol = ugCol.Tag is ChemDrawTag;
                        ShowGridHeaderContextMenu(PointToClient(pScreen), bIsStructCol || bIsChildBand);
                    }
                }
            }
        }

        private Infragistics.Win.UltraWinGrid.ColumnHeader GetClickedHeader(ChemDataGrid cdg, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            Infragistics.Win.UIElement ui = cdg.DisplayLayout.UIElement.ElementFromPoint(p);
            Infragistics.Win.UltraWinGrid.ColumnHeader result = (ui == null) ? null :
                ui.SelectableItem as Infragistics.Win.UltraWinGrid.ColumnHeader;
            return result;
        }

        private void ShowGridHeaderContextMenu(Point p, bool bHideSort)
        {
            // create and display context menu for grid header
            ContextMenuStrip cMenu = new ContextMenuStrip();
            ToolStripMenuItem sortAsc = new ToolStripMenuItem(ChemControlsConstants.GRID_CMENU_SORT_ASCENDING);
            ToolStripMenuItem sortDesc = new ToolStripMenuItem(ChemControlsConstants.GRID_CMENU_SORT_DESCENDING);
            sortAsc.Click += new EventHandler(sortAsc_Click);
            sortDesc.Click += new EventHandler(sortDesc_Click);

            cMenu.Items.AddRange(new ToolStripItem[] { sortAsc, sortDesc });
            if (bHideSort)
            {
                sortAsc.Visible = false;
                sortDesc.Visible = false;
            }
            cMenu.Show(this, p);
        }

        private void sortDesc_Click(object sender, EventArgs e)
        {
            DoSortOnGridClick(sender, false);
        }
        //-------------------------------------------------------------------------------------
        private void sortAsc_Click(object sender, EventArgs e)
        {
            DoSortOnGridClick(sender, true);
        }

        private void DoSortOnGridClick(object sender, bool bAscending)
        {            
            UltraGridColumn ugCol = m_clickedGridHeader.Column;
            ugCol.Band.SortedColumns.Clear();
            ugCol.SortIndicator = bAscending ? SortIndicator.Ascending : SortIndicator.Descending;
            SortChemGrid(ugCol.Key);
        }

        DataSet dsResult = new DataSet();
        private void SortChemGrid(string colKey)
        {
            cdGrid.DataSource = _result;
            _result.Tables[0].DefaultView.Sort = colKey;            
            if (_result.Tables[0].Rows.Count > 0)
            {
                cdGrid.CheckForChemDraw();
            }
            SetColorToChemGrid();
        }

        private void SetColorToChemGrid()
        {
            for (int j = 0; j < cdGrid.Rows.Count; j++)
            {
                if (SelectedDic.ContainsKey(cdGrid.Rows[j].Cells["No"].Value.ToString()))
                    cdGrid.Rows[j].Appearance.BackColor = Color.FromName(_selectedColorName);
                else
                    cdGrid.Rows[j].Appearance.BackColor = Color.White;
            }

        }

        #endregion

        public DisplayInputData()
        {
            InitializeComponent();

            // accept/cancel
            Controls.Add(AcceptButton);
            Controls.Add(CancelButton);
            AcceptButton.Top = _BatchMarkbutton.Top;
            CancelButton.Top = _BatchMarkbutton.Top;
            CancelButton.Click += new EventHandler(CancelButton_Click);
            AcceptButton.Click += new EventHandler(AcceptButton_Click);
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            string select = "Check = true";
            DataRow[] drs = _result.Tables[0].Select(select);

            if (drs.Length == 0)
            {
                MessageBox.Show("No record have been checked!", CambridgeSoft.DataLoaderGUI.Properties.Resources.Message_Title, MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            _fName = string.Empty;
            _fName = JobUtility.getFileName("selected", _job.DataSourceInformation.DerivedFileInfo);
            ExportFile.doExportFile(_job, drs, _fName, records);            
            OnAccept();
        }


        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel();
        }

        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            SelectedDic.Clear();
            foreach (DataRow dr in _result.Tables[0].Rows)
            {
                dr["Check"] = true;
                SelectedDic.Add(dr["No"].ToString(), true);
                cdGrid.Rows[Convert.ToInt32(dr["No"]) - 1].Appearance.BackColor = Color.FromName(_selectedColorName);;
            }
        }

        private void _Inversebutton_Click(object sender, EventArgs e)
        {
            SelectedDic.Clear();
            foreach (DataRow dr in _result.Tables[0].Rows)
            {
                if ((bool)dr["Check"] == false)
                {
                    dr["Check"] = true;
                    SelectedDic.Add(dr["No"].ToString(), true);                    
                }
                else
                {
                    dr["Check"] = false;                    
                }
            }
            SetColorToChemGrid();
        }

        private void _BatchMarkbutton_Click(object sender, EventArgs e)
        {
            using(BatchMarkForm form = new BatchMarkForm())
            {
                form.MaxValue = cdGrid.Rows.Count;
                form.ShowDialog();
                if(form.OK)
                {
                    int[] number = form.GetNumber;
                    SelectedDic.Clear();
                    foreach (DataRow dr in _result.Tables[0].Rows)
                    {
                        if (Convert.ToInt32(dr["No"]) < number[0] ||
                            Convert.ToInt32(dr["No"]) > number[1])
                        {
                            dr["Check"] = false;
                            cdGrid.Rows[Convert.ToInt32(dr["No"])-1].Appearance.BackColor = Color.White;
                        }
                        else
                        {
                            dr["Check"] = true;
                            SelectedDic.Add(dr["No"].ToString(), true);
                            cdGrid.Rows[Convert.ToInt32(dr["No"])-1].Appearance.BackColor = Color.FromName(_selectedColorName);;
                        }
                    }
                }
            }
        }

        private void _ExportButton_Click(object sender, EventArgs e)
        {
            string filter = string.Empty;
            switch (_job.DataSourceInformation.FileType)
            {
                case SourceFileType.MSExcel:
                case SourceFileType.CSV:
                    filter = "Text files (*.txt)|*.txt";
                    break;
                case SourceFileType.SDFile:
                    filter = "SD files (*.sdf)|*.sdf";
                    break;
                default:
                    break;
            }

            string select = "Check = true";
            DataRow[] drs = _result.Tables[0].Select(select);

            if (drs.Length == 0)
            {
                MessageBox.Show("No record have been checked!", CambridgeSoft.DataLoaderGUI.Properties.Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = filter;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fName = saveFileDialog.FileName;

                try
                {
                    if (File.Exists(fName))
                    {
                        File.Delete(fName);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, CambridgeSoft.DataLoaderGUI.Properties.Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ExportFile.doExportFile(_job, drs, fName, records);

                MessageBox.Show("Export completed!", CambridgeSoft.DataLoaderGUI.Properties.Resources.Message_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }        

        Dictionary<string, bool> SelectedDic = new Dictionary<string, bool>();
        private void cdGrid_MouseUp(object sender, MouseEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGrid grid = (UltraGrid)sender;
            Infragistics.Win.UIElement element =
            grid.DisplayLayout.UIElement.LastElementEntered;
            if (element is Infragistics.Win.CheckIndicatorUIElement)
            {
                UltraGridCell cell =
                element.GetContext(typeof(UltraGridCell)) as UltraGridCell;
                if (cell != null)
                {
                    cell.Value = !((bool)cell.Value);   
                    if(Convert.ToBoolean(cell.Value))
                    {
                        cell.Row.Appearance.BackColor = Color.FromName(_selectedColorName);;
                        if (!SelectedDic.ContainsKey(cell.Row.Cells["No"].Value.ToString()))
                            SelectedDic.Add(cell.Row.Cells["No"].Value.ToString(), true);
                    }
                    else
                    {
                        cell.Row.Appearance.BackColor = Color.White;
                        if (SelectedDic.ContainsKey(cell.Row.Cells["No"].Value.ToString()))
                            SelectedDic.Remove(cell.Row.Cells["No"].Value.ToString());
                    }
                }
            }
            AcceptButton.Focus();
        }

        private void DisplayInputData_Load(object sender, EventArgs e)
        {
            groupBox1.Dock = DockStyle.Fill;
            timer1.Enabled = true;        
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            groupBox1.Controls.Add(cdGrid);
            groupBox1.Dock = DockStyle.Top;
            lblWait.Visible = false;
            timer1.Enabled = false;
        }
    }
}
