using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CambridgeSoft.COE.DataLoader.UserControls
{
    public partial class ImportFilePicker : UserControl
    {

        #region constructor

        public ImportFilePicker()
        {
            InitializeComponent();
            this.InitControlsSetting();
        }

        #endregion

        #region methods

        private void InitControlsSetting()
        {
            //properties
            this._sdfConfigGroupBox.Visible = false;
            this._csvConfigGroupBox.Visible = false;

            //events
            this._chooseFileButton.Click += new EventHandler(ChooseFileButton_Click);
            this._selectExtentionNameComboBox.SelectedIndexChanged +=
                new EventHandler(SelectExtentionNameComboBox_SelectedIndexChanged);
        }

        #endregion

        #region event handlers

        //TODO: Drag and drop onto the form. If you don't HAVE to do things dynamically, don't.

        private void ChooseFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.InitialDirectory = "C:\\";
            op.Filter = "csv files(*.csv)|*.csv|tsv files(*.tsv)|*.tsv|All files (*.*)|*.*";
            if (op.ShowDialog() == DialogResult.OK)
            {
                this._fileNameTextBox.Text = op.FileName;
            }
        }

        //TODO: Make an enumeration out of the file type. We cannot accurately determine the file type
        //      based on the file extension.
        // { SDFile, MSExcel, CSV }
        // All we're going to do is make an educated guess for the user:
        //   (1) for *.sd|*.sdf, assume 'FileType.SDFile'
        //   (2) for *.sls|*.xslx, assume 'FileType.MSExcel'
        //   (3) for everything else, assume 'FileType.CSV'
        // The user can override this assumed value

        private void SelectExtentionNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if ((string)cb.Items[cb.SelectedIndex] == "csv;tsv")
            {
                this._csvConfigGroupBox.Visible = true;
                this._csvConfigGroupBox.BringToFront();
            }
            else if ((string)cb.Items[cb.SelectedIndex] == "sdf")
            {
                this._sdfConfigGroupBox.Visible = true;
                this._sdfConfigGroupBox.BringToFront();
            }
            else
            {
                this._sdfConfigGroupBox.Visible = false;
                this._csvConfigGroupBox.Visible = false;
            }
        }

        #endregion

    }
}
