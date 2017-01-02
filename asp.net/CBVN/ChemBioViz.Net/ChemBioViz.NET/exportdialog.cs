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


namespace ChemBioViz.NET
{
    public partial class ExportDialog : Form
    {
        #region Variables
        private ChemBioVizForm m_form;
        private ExportOpts m_opts;
        #endregion

        #region Properties
        public ExportOpts ExportOpts
        {
            get { return m_opts; }
            set { m_opts = value; }
        }
        #endregion

        #region Constructors
        public ExportDialog(ChemBioVizForm form, ExportOpts opts)
        {
            InitializeComponent();
            m_form = form;
            m_opts = opts;
            OptsToDialog();
            if (ExportOpts.ForExcel)        // CSBR-151777: Hide filename, etc.
            {
                Text = "Excel Export";
                groupBox2.Visible = false;
                filenameTextBox.Visible = false;
                browseButton.Visible = false;
            }
            this.CenterToParent();
        }
        #endregion

        #region Methods
        private void OptsToDialog()
        {
            filenameTextBox.Text = m_opts.OutputPath;

            // CSBR-133517: change extension to target type
            filenameTextBox.Text = Path.ChangeExtension(filenameTextBox.Text, ".sdf");
                        
            corrRadioButton.Checked = m_opts.ExportOptsType == ExportOpts.ExportType.SDCorrelated;
            nestedRadioButton.Checked = m_opts.ExportOptsType == ExportOpts.ExportType.SDNested;
            noneRadioButton.Checked = m_opts.ExportOptsType == ExportOpts.ExportType.SDMain;
            expRecordsLabel.Text = String.Format("Records to export: {0}", m_form.Pager.ListSize);
            bool bWithChildFields = noneRadioButton.Checked ? true : (nestedRadioButton.Checked ? true : corrRadioButton.Checked);           
            ExportDelimDialog.FillListBox(fieldsListBox, ExportOpts, m_form, bWithChildFields);
        }
        //---------------------------------------------------------------------
        private void DialogToOpts()
        {
            m_opts.OutputPath = filenameTextBox.Text;
            if (corrRadioButton.Checked) m_opts.ExportOptsType = ExportOpts.ExportType.SDCorrelated;
            else if (nestedRadioButton.Checked) m_opts.ExportOptsType = ExportOpts.ExportType.SDNested;
            else if (noneRadioButton.Checked) m_opts.ExportOptsType = ExportOpts.ExportType.SDMain;
            m_opts.DataviewID = this.m_form.TabManager.CurrentTab.FormDbMgr.SelectedDataView.DataViewID;
            m_opts.ExportFieldNames = ExportDelimDialog.GetCheckedNames(fieldsListBox);
        }
        //---------------------------------------------------------------------
        private void CheckEnabling()
        {
            bool bHasPath = !String.IsNullOrEmpty(filenameTextBox.Text);
            okButton.Enabled = bHasPath;

            String msg = string.Empty;
            if (corrRadioButton.Checked) msg = CBVConstants.EXPORTSD_CORR;
            else if (nestedRadioButton.Checked) msg = CBVConstants.EXPORTSD_NESTED;
            else if (noneRadioButton.Checked) msg = CBVConstants.EXPORTSD_MAIN;
            detailsTextBox.Text = msg;
        }
        #endregion

        #region Events
        private void okButton_Click(object sender, EventArgs e)
        {
            // CSBR-128061: don't do this in form_closing event
            if (!String.IsNullOrEmpty(filenameTextBox.Text) && File.Exists(filenameTextBox.Text))
            {
                String message = String.Format("File exists: {0}\n\nOK to overwrite?", filenameTextBox.Text);
                if (MessageBox.Show(message, "File Overwrite", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    this.DialogResult = DialogResult.Cancel;    // NO! this takes down dialog; what we want is to reprompt for save file
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
        private void noneRadioButton_Click(object sender, EventArgs e)
        {
            ExportDelimDialog.FillListBox(fieldsListBox, ExportOpts, m_form, false);
            CheckEnabling();
        }
        //---------------------------------------------------------------------
        private void corrRadioButton_Click(object sender, EventArgs e)
        {
            ExportDelimDialog.FillListBox(fieldsListBox, ExportOpts, m_form, true);
            CheckEnabling();
        }
        //---------------------------------------------------------------------
        private void nestedRadioButton_Click(object sender, EventArgs e)
        {
            ExportDelimDialog.FillListBox(fieldsListBox, ExportOpts, m_form, true);
            CheckEnabling();
        }
        //---------------------------------------------------------------------
        private void browseButton_Click(object sender, EventArgs e)
        {
            String fname = filenameTextBox.Text;

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.OverwritePrompt = false;    // check on close
            dlg.FileName = String.IsNullOrEmpty(fname) ? CBVConstants.DEFAULT_SDF_FILE_NAME : Path.GetFileName(fname);
            dlg.InitialDirectory = String.IsNullOrEmpty(fname) ? Application.CommonAppDataPath : Path.GetDirectoryName(fname);
            dlg.Filter = CBVConstants.SDF_FILE_FILTERS;
            dlg.DefaultExt = ".sdf";

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
        /*  End of CSBR-151570 */
        //---------------------------------------------------------------------
        #endregion
    }
}
