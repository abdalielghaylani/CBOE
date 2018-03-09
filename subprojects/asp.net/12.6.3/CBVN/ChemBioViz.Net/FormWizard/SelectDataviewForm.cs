using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.COEDataViewService;
using FormWizard.Properties;

namespace FormWizard
{
    public partial class SelectDataviewForm : Form
    {
        #region Properties

        private COEDataViewBO _dataviewBO;

        /// <summary>
        /// returns selected dataview for owner form
        /// </summary>
        public COEDataViewBO _selectedDataviewBO
        {
            get
            {
                return dataViewComboBox.SelectedItem as COEDataViewBO;
            }
        }

        /// <summary>
        /// The list of available DataViews the user can select from.
        /// </summary>
        private IEnumerable<COEDataViewBO> _dataViewBOList;
        private List<COEDataViewBO> _searchDataViewBOList;

        public COEDataViewBO DataviewBO
        {
            get
            {
                return _dataviewBO;
            }
            set
            {
                _dataviewBO = value;
                this.dataViewComboBox.SelectedItem = _dataviewBO;
                this.selectDataview1.RebuildTreeView(COEDataViewManagerBO.NewManager(this._dataviewBO.COEDataView));
            }
        }

        private string helpFilePath;
        /// <summary>
        /// Gets or sets the help file path
        /// </summary>
        public string HelpFilePath
        {
            get
            {
                return helpFilePath;
            }
            set
            {
                helpFilePath = value;
                if (!string.IsNullOrEmpty(helpFilePath))
                {
                    this.helpProvider1.HelpNamespace = helpFilePath;
                    this.helpProvider1.SetHelpKeyword(this, FormWizard.Properties.Resources.HelpSelectDataviewTopicId);
                }
            }
        }

        #endregion        

        #region Private Methods

        /// <summary>
        /// Displaying the dataview summary
        /// </summary>
        /// <param name="SelectedDv"></param>
        private void DisplayDaviewDetails(COEDataViewBO SelectedDv)
        {
            dvNameLabel.Text = SelectedDv.Name;
            dvDescLabel.Text = SelectedDv.Description;
            dvUserLabel.Text = SelectedDv.UserName;
            dvDateCreatedLabel.Text = SelectedDv.DateCreated.ToShortDateString();
        }

        /// <summary>
        /// Prepare the list of table names for type ahead
        /// </summary>
        private void FillTables()
        {
            List<string> tables = new List<string>();
            COEDataViewManagerBO dvm = null;
            if (_dataViewBOList != null)
            {
                foreach (COEDataViewBO dv in _dataViewBOList)
                {
                    dvm = COEDataViewManagerBO.NewManager(dv.COEDataView);
                    foreach (TableBO tbl in dvm.Tables)
                    {
                        if (!tables.Contains(tbl.Alias))
                            tables.Add(tbl.Alias);
                    }
                }
                // Create the list to use as the custom source.  
                var source = new AutoCompleteStringCollection();
                source.AddRange(tables.ToArray());
                searchTextBox.AutoCompleteCustomSource = source;
            }
        }

        /// <summary>
        /// Preparing the list of tags from the dataview list
        /// </summary>
        private void FillTags()
        {
            List<string> tags = new List<string>();
            COEDataViewManagerBO dvm = null;
            foreach (COEDataViewBO dv in _dataViewBOList)
            {
                dvm = COEDataViewManagerBO.NewManager(dv.COEDataView);
                foreach (TableBO tbl in dvm.Tables)
                {
                    foreach (string tag in tbl.Tags)
                    {
                        if (!tags.Contains(tag))
                            tags.Add(tag);
                    }
                }
            }
            // Create the list to use as the custom source.  
            var source = new AutoCompleteStringCollection();
            source.AddRange(tags.ToArray());
            searchTextBox.AutoCompleteCustomSource = source;
        }

        /// <summary>
        /// Fill the dropdown with dataview list and refresh the treeview and dataview summary.
        /// </summary>
        private void FillDataviews()
        {
            this.dataViewComboBox.Items.Clear();
            //this.dataViewComboBox.DataSource = this._dataViewBOList;    
            foreach (COEDataViewBO dataView in this._dataViewBOList)
            {
                this.dataViewComboBox.Items.Add(dataView);
            }
            if (this.dataViewComboBox.Items.Count > 0)
                this.dataViewComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// When dataview selected from dataview refresh the dataview summary and its tables and fields in the treeview.
        /// </summary>
        private void RefreshDataview()
        {
            if (this.dataViewComboBox.SelectedItem != null)
            {
                COEDataViewBO SelectedDv = this.dataViewComboBox.SelectedItem as COEDataViewBO;
                this.selectDataview1.RebuildTreeView(COEDataViewManagerBO.NewManager(SelectedDv.COEDataView));
                DisplayDaviewDetails(SelectedDv);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public SelectDataviewForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// While popup the dataview selection from we are getting the dataviews from the owner form.
        /// </summary>
        /// <param name="dataViewBOList"></param>
        public void FillDataviews(IEnumerable<COEDataViewBO> dataViewBOList)
        {
            _dataViewBOList = dataViewBOList;
            FillDataviews();
        }       
        
        #endregion

        #region Events

        private void okButton_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            if (dataViewComboBox.SelectedItem == null)
                errorProvider1.SetError(dataViewComboBox, "Select Dataview");
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void SelectDataviewForm_Load(object sender, EventArgs e)
        {
            this.Text = Resources.Form_Browse_DataView_Title + " - " + Resources.FORM_TITLE;
            FillTables();
        }

        private void dataViewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.selectDataview1.ClearSearchText();
            RefreshDataview();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            if (string.IsNullOrEmpty(searchTextBox.Text))
                errorProvider1.SetError(searchTextBox, "Invalid/Missing input");
            else
            {
                COEDataViewManagerBO dvm = null;
                _searchDataViewBOList = new List<COEDataViewBO>();
                foreach (COEDataViewBO dv in _dataViewBOList)
                {
                    dvm = COEDataViewManagerBO.NewManager(dv.COEDataView);
                    foreach (TableBO tbl in dvm.Tables)
                    {
                        if (tagNameRadioButton.Checked)
                        {
                            if (IncludeChildTablescheckBox.Checked ? tbl.Tags.Contains(searchTextBox.Text) : dvm.BaseTableId == tbl.ID && tbl.Tags.Contains(searchTextBox.Text))
                            {
                                if (!_searchDataViewBOList.Contains(dv))
                                    _searchDataViewBOList.Add(dv);
                            }
                        }
                        if (tableNameRadioButton.Checked)
                        {
                            if (tbl.Alias == searchTextBox.Text)
                            {
                                if (!_searchDataViewBOList.Contains(dv))
                                    _searchDataViewBOList.Add(dv);
                            }
                        }
                    }
                }
                if (this._searchDataViewBOList.Count > 0)
                {
                    this.dataViewComboBox.Items.Clear();
                    //this.dataViewComboBox.DataSource = this._dataViewBOList;    
                    foreach (COEDataViewBO dataView in this._searchDataViewBOList)
                    {
                        this.dataViewComboBox.Items.Add(dataView);
                    }
                    dataViewComboBox.SelectedIndex = 0;
                    RefreshDataview();
                }
                else
                {
                    MessageBox.Show(Resources.NoRecords, Resources.FORM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void retrieveAllButton_Click(object sender, EventArgs e)
        {
            FillDataviews();
        }

        private void searchOptionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            searchTextBox.Text = string.Empty;
            if (tagNameRadioButton.Checked)
            {
                IncludeChildTablescheckBox.Visible = true;
                FillTags();
            }
            else if (tableNameRadioButton.Checked)
            {
                IncludeChildTablescheckBox.Visible = false;
                FillTables();
            }
        }

        private void dataViewComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            ComboBox cmbBox = sender as ComboBox;
            if (cmbBox != null)
            {
                if (cmbBox.DroppedDown)
                    cmbBox.DroppedDown = false;
            }
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Help.ShowHelp(this, this.helpProvider1.HelpNamespace);
        }

        private void helpButton_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            System.Windows.Forms.Help.ShowHelp(this, this.helpProvider1.HelpNamespace);
        }
        #endregion
    }
}
