using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Configuration;

namespace COELogViewer
{
    /* The grid's DataSource should be a DataView which is configurable based on the user's
     * selection of a combination of:
     * (1) the column to filter by
     * (2) a filter value to complete the filter clause
     * */

    public partial class COELogViewerForm : Form
    {
        #region Variables

        private string _currentPath;
        Configuration _configuration;
        DataTable _dataTable;
        private int _currentColumnIndex = -1;
        private string _currentColumnName = string.Empty;
        private string _currentColumnFilter = string.Empty;

        #endregion

        #region Properties

        public string LastOpenedFile
        {
            get
            {
                if(_configuration == null)
                    _configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                return _configuration.AppSettings.Settings["lastOpenedFile"].Value;
            }
            set
            {
                if(_configuration == null)
                    _configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                if(_configuration.AppSettings.Settings["lastOpenedFile"] == null)
                    _configuration.AppSettings.Settings.Add("lastOpenedFile", value);
                else
                    _configuration.AppSettings.Settings["lastOpenedFile"].Value = value;

                _configuration.Save(ConfigurationSaveMode.Full);

                //ConfigurationManager.RefreshSection("appSettings");
            }
        }

        #endregion

        #region Constructors

        public COELogViewerForm()
        {
            InitializeComponent();

            this._currentPath = COEConfigurationBO.ConfigurationBaseFilePath + "LogOutput";

            if(!string.IsNullOrEmpty(this.LastOpenedFile))
                this._currentPath = this.LastOpenedFile;
        }

        #endregion

        #region Events

        private void OpenButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.ShowOpenLogDialog();
            }
            catch(Exception exception)
            {
                HandleError(exception);
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            try
            {
                if(_currentPath != string.Empty)
                {
                    this.OpenLog(_currentPath);
                }
            }
            catch(Exception exception)
            {
                HandleError(exception);
            }
        }

        private void LogDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if(e.ColumnIndex > 0)
                {
                    int startCount = 1;
                    for(int currentRow = e.RowIndex; currentRow < LogDataGridView.Rows.Count; currentRow++)
                    {
                        string currentMessage = this.LogDataGridView.Rows[currentRow].Cells["Message"].Value.ToString();

                        this.LogDataGridView.Rows[currentRow].Selected = true;

                        if(currentMessage.Contains("END") && (--startCount) <= 0)
                        {
                            //endTime = float.Parse(LogDataGridView.Rows[currentRow].Cells["Time"])
                            break;
                        }
                        else if(currentMessage.Contains("START") && currentRow != e.RowIndex)
                            startCount++;
                    }

                    int endCount = 1;
                    for(int currentRow = e.RowIndex; currentRow >= 0; currentRow--)
                    {
                        string currentMessage = this.LogDataGridView.Rows[currentRow].Cells["Message"].Value.ToString();

                        this.LogDataGridView.Rows[currentRow].Selected = true;

                        if(currentMessage.Contains("START") && (--endCount) <= 0)
                            break;
                        else if(currentMessage.Contains("END") && currentRow != e.RowIndex)
                            endCount++;
                    }
                }
            }
            catch
            { }
        }

        private void LogDataGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if(this.LogDataGridView.Columns.Contains("TotalTime") && this.LogDataGridView.SelectedRows.Count > 1)
                {
                    if(e.RowIndex > 0)
                    {
                        double starting = double.Parse(this.LogDataGridView.SelectedRows[0].Cells["TotalTime"].Value.ToString());
                        double ending = double.Parse(this.LogDataGridView.SelectedRows[this.LogDataGridView.SelectedRows.Count - 1].Cells["TotalTime"].Value.ToString());

                        double elapsed = Math.Abs(ending - starting);

                        this.LogDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex < 0 ? 0 : e.ColumnIndex].ToolTipText = elapsed.ToString();
                    }
                }
            }
            catch
            {
                //this is just cosmetic so don't stop if something fails here.
            }
        }

        private void LogDataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                //MessageBox.Show(this, this.LogDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString(), this.LogDataGridView.Columns[e.ColumnIndex].Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
                TextViewer viewer = new TextViewer(this.LogDataGridView.Columns[e.ColumnIndex].Name, this.LogDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString());
                viewer.ShowDialog(this);
            }
            catch(Exception)
            { }

        }

        private void COELogViewerForm_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void COELogViewerForm_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                Array a = (Array) e.Data.GetData(DataFormats.FileDrop);

                if(a != null)
                {
                    string s = a.GetValue(0).ToString();
                    this.OpenLog(s);
                }
            }
            catch
            {
            }
        }

        private void ColumnNamesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripComboBox selector = (ToolStripComboBox) sender;
            _currentColumnIndex = selector.SelectedIndex;
            if(selector.SelectedIndex != -1 && this._dataTable != null)
            {
                _currentColumnName = this._dataTable.Columns[_currentColumnIndex].ColumnName;
            }
            else
            {
                _currentColumnName = string.Empty;
            }

            //re-populate the options available in the combo
            ToolStripComboBox filterSelector = (ToolStripComboBox) FilterClauseTextCombo;
            filterSelector.Items.Clear();
            foreach(DataRow dr in this._dataTable.Rows)
            {
                if(!filterSelector.Items.Contains(dr[_currentColumnIndex]))
                {
                    filterSelector.Items.Add(dr[_currentColumnIndex]);
                }
            }

            RefreshGrid();
        }

        private void FilterClauseTextBox_TextChanged(object sender, EventArgs e)
        {
            ToolStripComboBox filterBox = (ToolStripComboBox) sender;
            this._currentColumnFilter = filterBox.Text;
            RefreshGrid();
        }

        #endregion

        #region Methods

        public void ShowOpenLogDialog()
        {
            string selectedPath = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Log Files (*.log)| *.log";
            openFileDialog.InitialDirectory = _currentPath;
            openFileDialog.Multiselect = false;

            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.OpenLog(openFileDialog.FileName);
            }

            /*FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.ShowNewFolderButton = false;
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.CommonApplicationData;
            folderBrowserDialog1.SelectedPath = currentPath;
            
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                currentPath= folderBrowserDialog1.SelectedPath ;

                this.DisplayLog(currentPath);
            }*/
        }

        public void OpenLog(string path)
        {
            _dataTable = COELog.GetLogFileAsDataTable(path);

            this.ColumnNamesComboBox.Items.Clear();
            foreach(DataColumn dc in _dataTable.Columns)
            {
                this.ColumnNamesComboBox.Items.Add(dc.ColumnName);
            }

            this.RefreshGrid();

            int index = Math.Min(Math.Max(path.LastIndexOf("\\") + 1, 0), path.Length - 1);
            this.Text = path.Substring(index);

            _currentPath = LastOpenedFile = path;
        }

        private void RefreshGrid()
        {
            if(_dataTable != null)
            {

                //Apply a DataView based on the selected-column and filter options
                DataView newView = null;
                if(string.IsNullOrEmpty(_currentColumnFilter) || _currentColumnIndex == -1)
                {
                    //clear any filters
                    this._dataTable.DefaultView.RowFilter = string.Empty;
                    newView = _dataTable.DefaultView;
                }
                else
                {
                    //apply the column filter
                    //TODO: Enforce more robust type-checking for filtering based on the DataColumn, as necessary
                    string filterTemplate = string.Empty;
                    if(this._dataTable.Columns[this._currentColumnIndex].DataType == typeof(System.String))
                    {
                        filterTemplate = "[{0}] like '%{1}%'";
                    }
                    else
                    {
                        filterTemplate = "[{0}] = {1}";
                    }
                    string filterClause = string.Format(filterTemplate, _currentColumnName, _currentColumnFilter);
                    this._dataTable.DefaultView.RowFilter = filterClause;
                    newView = _dataTable.DefaultView;
                }
                LogDataGridView.DataSource = newView;

                //cuistomize the appearance and interaction
                foreach(DataGridViewRow currentRow in this.LogDataGridView.Rows)
                {
                    if(this.LogDataGridView.Columns.Contains("Severity"))
                    {
                        if(currentRow.Cells["Severity"].Value.ToString().ToLower().Trim() == "error")
                            currentRow.DefaultCellStyle.ForeColor = Color.Red;
                    }
                }

                this.LogEntriesStatusLabel.Text = newView.Count.ToString() + " matching log entries";
            }
            else
            {
                this.LogEntriesStatusLabel.Text = "No file currently loaded";
            }
        }

        private void HandleError(Exception exception)
        {
            MessageBox.Show(exception.Message);
        }

        #endregion

    }
}
