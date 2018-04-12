using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CambridgeSoft.COE.DataLoader.Windows.Controls
{
    public partial class OutputFileChooser : UIBase
    {
        #region data
        private System.Windows.Forms.Button _BrowseButton;
        string _strFilter;
        private System.Windows.Forms.Label _OutputFileLabel;
        private System.Windows.Forms.TextBox _OutputFileTextBox;
        #endregion

        #region properties
        /// <summary>
        /// Get property to return the current filter
        /// Set property to set the filter
        /// (Filter is an accumulation of InputObject filters separated by '|' )
        /// </summary>
        public string Filter
        {
            get
            {
                if (_strFilter == null) _strFilter = string.Empty;
                return _strFilter;
            }
            set
            {
                _strFilter = value;
                return;
            }
        } // Filter

        /// <summary>
        /// Get property for the selected input file
        /// </summary>
        public string OutputFile
        {
            get
            {
                return _OutputFileTextBox.Text;
            }
            //Fix for CSBR 160709-Application hangs while uploading data after writing the files to text or XML kind 
            set
            {
                _OutputFileTextBox.Text = value;
            }

        } // OutputFile
        #endregion

        #region constructors
        /// <summary>
        /// ! Constrcutor
        /// </summary>
        public OutputFileChooser()
        {
            StatusText = "Choose output file";
            InitializeComponent();
            // Programmatically add control(s)
            // 
            SuspendLayout();
            // lblOutputType
            _OutputFileLabel = UIBase.GetLabel();
            _OutputFileLabel.AutoSize = true;
            _OutputFileLabel.Text = "Output file:";
            Controls.Add(_OutputFileLabel);
            // _OutputFileTextBox
            _OutputFileTextBox = UIBase.GetTextBox();
            Controls.Add(_OutputFileTextBox);
            // _BrowseButton
            _BrowseButton = UIBase.GetButton(ButtonType.Browse);
            Controls.Add(_BrowseButton);
            // btnAccept
            AcceptButton.Enabled = false;
            Controls.Add(AcceptButton);
            // btnCancel
            Controls.Add(CancelButton);
            // events
            _OutputFileTextBox.TextChanged += new EventHandler(OutputFileTextBox_TextChanged);
            _BrowseButton.Click += new EventHandler(BrowseButton_Click);
            AcceptButton.Click += new EventHandler(AcceptButton_Click);
            CancelButton.Click += new EventHandler(CancelButton_Click);
            Layout += new LayoutEventHandler(OutputFileChooser_Layout);
            //
            ResumeLayout(false);
            PerformLayout();
            return;
        } // OutputFileChooser()
        #endregion

        #region event handlers
        private void AcceptButton_Click(object sender, EventArgs e)
        {
            OnAccept(); // The parent will check to see if the file exists
            return;
        } // AcceptButton_Click()

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgSaveFileDialog = new OpenFileDialog();    // SaveFileDialog doesn't really cut it!
            dlgSaveFileDialog.Filter = Filter;
            dlgSaveFileDialog.CheckFileExists = true;
            {
                string strOutputFile = _OutputFileTextBox.Text;
                try
                {
                    dlgSaveFileDialog.FileName = (File.Exists(strOutputFile)) ? strOutputFile : string.Empty;
                    {
                        string strDirectoryName = strOutputFile;
                        while (strDirectoryName.Length > 0)
                        {
                            if (Directory.Exists(strDirectoryName))
                            {
                                break;
                            }
                            int nPathSeparator = strDirectoryName.LastIndexOf(Path.DirectorySeparatorChar);
                            if (nPathSeparator <= 0)
                            {
                                strDirectoryName = string.Empty;
                                break;
                            }
                            strDirectoryName = strDirectoryName.Remove(nPathSeparator);
                        }
                        dlgSaveFileDialog.InitialDirectory = strDirectoryName;
                    }
                    string strExt = "*" + Path.GetExtension(strOutputFile).ToLower();
                    string[] strFilters = Filter.Split('|');
                    dlgSaveFileDialog.FilterIndex = 1;
                    for (int nFilterIndex = 1; nFilterIndex <= strFilters.GetLength(0) / 2; nFilterIndex++)
                    {
                        if (strFilters[nFilterIndex * 2 - 1].ToLower().Contains(strExt))
                        {
                            dlgSaveFileDialog.FilterIndex = nFilterIndex;
                        }
                    }
                }
                catch (Exception)
                {
                    // Ignore
                }
            }
            dlgSaveFileDialog.CheckFileExists = false;
            dlgSaveFileDialog.RestoreDirectory = true;
            dlgSaveFileDialog.Title = Application.ProductName + " - Output source file";
            if (dlgSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _OutputFileTextBox.Text = dlgSaveFileDialog.FileName;
                AcceptButton.Enabled = true;
                AcceptButton.Focus();
            }
            return;
        } // BrowseButton_Click()

        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel();
            return;
        } // CancelButton_Click()

        private void OutputFileChooser_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds"))
            {
                // Vertical
                int y = 0;
                int ySpacing = _OutputFileTextBox.Height;
                if (ySpacing < _BrowseButton.Height) ySpacing = _BrowseButton.Height;
                _OutputFileLabel.Top = y + (ySpacing - _OutputFileLabel.Height) / 2;
                _OutputFileTextBox.Top = y + (ySpacing - _OutputFileTextBox.Height) / 2;
                _BrowseButton.Top = y + (ySpacing - _BrowseButton.Height) / 2;
                y += ySpacing + UIBase.ExtraPadding.Top;
                CancelButton.Top = y;
                AcceptButton.Top = y;
                y += AcceptButton.Height;
                Height = y;
                // Horizontal
                int x = 0;
                _OutputFileLabel.Left = x;
                x += _OutputFileLabel.Width;
                _OutputFileTextBox.Left = x;
                {
                    int nWidth = MaximumSize.Width - _OutputFileTextBox.Left - _BrowseButton.Width;
                    int nWidthMax = TextRenderer.MeasureText("X", _OutputFileTextBox.Font).Width * 64;
                    if (nWidth > nWidthMax) nWidth = nWidthMax;
                    _OutputFileTextBox.Width = nWidth;
                }
                x += _OutputFileTextBox.Width;
                _BrowseButton.Left = x;
                x += _BrowseButton.Width;
                Width = x;
                // Horizontal
                x = 0;
                CancelButton.Left = x;
                x += CancelButton.Width;
                AcceptButton.Left = x;
                x += AcceptButton.Width;
                if (Width < x) Width = x;
                x = Width;
                x -= AcceptButton.Width;
                AcceptButton.Left = x;
                x -= CancelButton.Width;
                CancelButton.Left = x;
            } // if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds"))
            return;
        } // OutputFileChooser_Layout()

        private void OutputFileTextBox_TextChanged(object sender, EventArgs e)
        { //Fix for CSBR-158007-Cut or Delete action of the output file path shows exception
            UIBase.TextBox_CleanPath(_OutputFileTextBox);
            bool bOK = false;
            try
            {
                if (_OutputFileTextBox.Text.Length > 0)
                {
                    bOK = true;
                    string strFullName = Path.GetFullPath(_OutputFileTextBox.Text);
                    string strDirectoryName = Path.GetDirectoryName(strFullName);
                    bOK &= ((File.GetAttributes(strDirectoryName) & FileAttributes.Directory) == FileAttributes.Directory);
                }

            }
            catch
            {
                bOK = false;
            }
            AcceptButton.Enabled = bOK;
            return;
        } // OutputFileTextBox_TextChanged()
        #endregion
    } // class OutputFileChooser
}
