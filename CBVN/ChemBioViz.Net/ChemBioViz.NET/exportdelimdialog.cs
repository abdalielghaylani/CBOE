using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using FormDBLib;
using CBVUtilities;
using Infragistics.Win.UltraWinGrid;
using ChemControls;
namespace ChemBioViz.NET
{
    public partial class ExportDelimDialog : Form
    {
        #region Variables
        private ChemBioVizForm m_form;        
        #endregion

        #region Properties
        public ExportOpts ExportOpts
        {
            get { return m_form.ExportOpts; }
            set { m_form.ExportOpts = value; }
        }
        #endregion

        #region Constructor
        public ExportDelimDialog(ChemBioVizForm form)
        {
            InitializeComponent();
            m_form = form;
            OptsToDialog();
            this.CenterToParent();
        }
        #endregion

        #region Methods
        private void OptsToDialog()
        {
            filenameTextBox.Text = ExportOpts.OutputPath;
            if (String.IsNullOrEmpty(filenameTextBox.Text))
                filenameTextBox.Text = CBVConstants.DEFAULT_DELIM_FILE_NAME;
            commaRadioButton.Checked = ExportOpts.TextDelimiterType == ExportOpts.DelimiterType.Comma;
            tabRadioButton.Checked = ExportOpts.TextDelimiterType == ExportOpts.DelimiterType.Tab;           
            // CSBR-133517: change extension to target type
            String sExt = commaRadioButton.Checked ? ".csv" : ".txt";
            filenameTextBox.Text = Path.ChangeExtension(filenameTextBox.Text, sExt);

            withHdrCheckBox.Checked = ExportOpts.WithHeader;
            int nRecs = m_form.Pager.ListSize;
            expRecordsLabel.Text = String.Format("Records to export: {0}", nRecs);
            bool bWithChildFields = true;
            FillListBox(fieldsListBox, ExportOpts, m_form, bWithChildFields);
        }
        //---------------------------------------------------------------------
        private void DialogToOpts()
        {
            ExportOpts.OutputPath = filenameTextBox.Text;
            if (commaRadioButton.Checked) ExportOpts.TextDelimiterType = ExportOpts.DelimiterType.Comma;
            else if (tabRadioButton.Checked) ExportOpts.TextDelimiterType = ExportOpts.DelimiterType.Tab;
            ExportOpts.WithHeader = withHdrCheckBox.Checked;
            ExportOpts.ExportFieldNames = GetCheckedNames(fieldsListBox);
            ExportOpts.DataviewID = this.m_form.TabManager.CurrentTab.FormDbMgr.SelectedDataView.DataViewID;
        }
        //---------------------------------------------------------------------
        public static List<String> GetCheckedNames(ListView fieldsListBox)
        {
            List<String> names = new List<string>();
            foreach (ListViewItem item in fieldsListBox.Items)
                if (item.Checked)
                    names.Add(item.Text);
            return names;
        }
        //---------------------------------------------------------------------
        public static void FillListBox(ListView fieldsListBox, ExportOpts opts, ChemBioVizForm form,
                                        bool bWithChildFields)
        {
            fieldsListBox.Items.Clear();
            fieldsListBox.View = View.Details;
            fieldsListBox.GridLines = true;
            fieldsListBox.Columns[0].Width = 200;

            int dataviewId = opts.DataviewID;
            List<String> checkedFieldNames = new List<string>();
            // get list of fields to be checked, or create a default one
            if (dataviewId == 0 || dataviewId == form.TabManager.CurrentTab.FormDbMgr.SelectedDataView.DataViewID)
                checkedFieldNames = opts.ExportFieldNames;

            if (checkedFieldNames.Count == 0 && form.TabManager.CurrentTab is FormViewTab)
            {
                FormViewControl fvc = form.TabManager.CurrentTab.Control as FormViewControl;
                //Coverity Bug Fix CID 12985 
                if (fvc != null)
                {     // CBOE-303
                   checkedFieldNames = fvc.GetDefaultFieldNames();
                }
            }
            if (checkedFieldNames.Count == 0)
                checkedFieldNames = opts.AllFieldNames;

            // add all fields to listview
            // new 3/11: or skip child fields if requested
            foreach (String fieldName in opts.AllFieldNames)
            {
                ListViewItem item = new ListViewItem(fieldName);
                if (fieldName.Contains('.') && !bWithChildFields) // CBOE-303, CBOE-1763, CBOE-1764
                    continue;
                item.Checked = checkedFieldNames.Contains(fieldName); //CBOE-303, CBOE-1763, CBOe-1764
                fieldsListBox.Items.Add(item);
            }
            fieldsListBox.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
        //---------------------------------------------------------------------
        private void CheckEnabling()
        {
            bool bHasPath = !String.IsNullOrEmpty(filenameTextBox.Text);
            okButton.Enabled = bHasPath;
        }
        #endregion   
 
        #region Events
        //---------------------------------------------------------------------
        private void okButton_Click(object sender, EventArgs e)
        {
            // CSBR-128061: don't do this in form_closing event
            String filename = filenameTextBox.Text;
            if (!String.IsNullOrEmpty(filename) && File.Exists(filename))
            {
                String message = String.Format("File exists: {0}\n\nOK to overwrite?", filename);
                if (MessageBox.Show(message, "File Overwrite", MessageBoxButtons.YesNo) != DialogResult.Yes) {
                    this.DialogResult = DialogResult.Cancel;
                    return;
                }
            }
            DialogToOpts();
            this.Close();
        }
        //---------------------------------------------------------------------
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //---------------------------------------------------------------------
        private void browseButton_Click(object sender, EventArgs e)
        {
            String fname = filenameTextBox.Text;

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.OverwritePrompt = false;    // check on close
            dlg.FileName = String.IsNullOrEmpty(fname) ? CBVConstants.DEFAULT_DELIM_FILE_NAME : Path.GetFileName(fname);
            dlg.InitialDirectory = String.IsNullOrEmpty(fname) ? Application.CommonAppDataPath : Path.GetDirectoryName(fname);
            dlg.Filter = CBVConstants.DELIM_FILE_FILTERS;
            dlg.FilterIndex = commaRadioButton.Checked ? 2 : 1;
            dlg.DefaultExt = commaRadioButton.Checked ? ".csv" : ".txt";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                filenameTextBox.Text = dlg.FileName;
            }
            CheckEnabling();
        }
        //---------------------------------------------------------------------
        private void filenameTextBox_TextChanged(object sender, EventArgs e)
        {
            CheckEnabling();
        }

        /* CSBR-151570 Feature Request: Ability to select/unselect All fields (that fall under a table) at a time while exporting hitlist in ChemBioViz Client 
         * Feature request implemented by Jogi  
           Implementation is to select or unselect all the fields in fields Listbox  */

        private void selectallBtn_Click(object sender, EventArgs e)
        {
            if (selectallBtn.Text == "Select All")
            {
                foreach (ListViewItem item in fieldsListBox.Items)
                {
                    item.Checked = true;
                }
                selectallBtn.Text = "Unselect All";
            }
            else if (selectallBtn.Text == "Unselect All")
            {
                foreach (ListViewItem item in fieldsListBox.Items)
                {
                    item.Checked = false;
                }
                selectallBtn.Text = "Select All";
            }
        }       
        /* End of CSBR-151570 */
        //-------------------------------------------------------------------
        /* CSBR-150816 File extension is not getting updated when we switch between Comma & Tab options in delimited export
         * Change done by Jogi
         when the tab radio button is selected changing the extension of file name to .txt otherwise it will be .csv*/

        private void tabRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            String sExt = tabRadioButton.Checked ? ".txt" : ".csv";
            filenameTextBox.Text = Path.ChangeExtension(filenameTextBox.Text, sExt);
        }
        /*  End of CSBR-150816 */
        //---------------------------------------------------------------------
        #endregion
    }
}
