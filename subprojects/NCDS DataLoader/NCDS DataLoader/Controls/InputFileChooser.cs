using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.FileParser.Excel;
using System.Diagnostics;
using CambridgeSoft.NCDS_DataLoader.Forms;

namespace CambridgeSoft.NCDS_DataLoader.Controls
{
    public partial class InputFileChooser : UIBase 
    {
        private string HAVEHEADER = "Y";
        private string NOHEADER = "N";
        private string strFileName;
        private List<string> _listCommand = new List<string>();
        private List<int> _listFilterIndices = new List<int>();
        private string _inputFilePath;
        private string _filter;
        private string _inputFilter;
        private bool _split = false;
        private bool _splitNumber;

        /// <summary>
        /// Get property to return the input file.
        /// </summary>
        public string InputFileName
        {
            get
            {
                return _InputFileTextBox.Text.Trim();
            }
        }


        /// <summary>
        /// Get property to return the input file Type.
        /// </summary>
        public SourceFileType GetFileType
        {
            get
            {
                string strExt = Path.GetExtension(_InputFileTextBox.Text.Trim()).ToLower();

                return FileType(strExt);
            }
        }

        /// <summary>
        /// Get property to return true when the input file cotains structure.
        /// </summary>
        public bool HaveStructure
        {
            get
            {
                return !_noStructureCheckBox.Checked;
            }
        }

        /// <summary>
        /// Get property to return true when split.
        /// </summary>
        public bool Split
        {
            get
            {
                return _split;
            }
        }

        /// <summary>
        /// Get property to return split number.
        /// </summary>
        public bool SplitNumber
        {
            get
            {
                return _splitNumber;
            }
        }


        /// <summary>
        /// Get property to return true when the input file cotains header.
        /// </summary>
        public bool IsHeader
        {
            get
            {
                if (_HeaderComboBox.Text.ToString() == HAVEHEADER)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Get property for the selected input file
        /// </summary>
        public string InputFile
        {
            get { return _inputFilePath; }
            set { _inputFilePath = value; }
        }

        /// <summary>
        /// Get property to return the user name.
        /// </summary>
        public string UserName
        {
            get
            {
                return _UserNameTextBox.Text.Trim();
            }
        }

        /// <summary>
        /// Get property to return the current filter
        /// Set property to set the filter
        /// (Filter is an accumulation of InputObject filters separated by '|' )
        /// </summary>
        public string Filter
        {
            get
            {
                if (_filter == null)
                    _filter = string.Empty;
                return _filter;
            }
            set { _filter = value; }
        }

        /// <summary>
        /// Get property to return the password.
        /// </summary>
        public string Password
        {
            get
            {
                return _PasswordTextBox.Text;
            }
        }

        /// <summary>
        /// Set property for the selected input file
        /// </summary>
        public string InputFilter
        {
            get { return _inputFilter; }
            set { _inputFilter = value; }
        }

        /// <summary>
        /// Get property for filters matching the selected input file
        /// </summary>
        public List<string> MatchingFilters
        {
            get
            {
                List<string> listMatchingFilters = new List<string>();
                foreach (int nFilterIndex in _listFilterIndices)
                {
                    listMatchingFilters.Add(LookupFilter(nFilterIndex));
                }
                return listMatchingFilters;
            }
        }

        /// <summary>
        /// Get property to return the worksheet.
        /// </summary>
        public string WorkSheet
        {
            get
            {
                return _WorkSheetComboBox.Text.Trim();
            }
        }

        /// <summary>
        /// Get property to return the header.
        /// </summary>
        public string Header
        {
            get
            {
                return _HeaderComboBox.Text.Trim();
            }
        }


        /// <summary>
        /// Get property to return the delimiter.
        /// </summary>
        public string Delimiter
        {
            get
            {
                return _DelimiterTextBox.Text;
            }
        }


        public InputFileChooser()
        {
            InitializeComponent();

            // accept/cancel
            AcceptButton.Enabled = false;
            _SplitFileButton.Enabled = false;
            Controls.Add(AcceptButton);
            CancelButton.Text = "Exit";
            Controls.Add(CancelButton);
            AcceptButton.TabIndex = 7;
            CancelButton.TabIndex = 8;
            // events
            _InputFileTextBox.TextChanged += new EventHandler(InputFileTextBox_TextChanged);
            _BrowseButton.Click += new EventHandler(btnBrowse_Click);
            AcceptButton.Click += new EventHandler(AcceptButton_Click);
            CancelButton.Click += new EventHandler(CancelButton_Click);

            _HeaderComboBox.Items.Add(HAVEHEADER);
            _HeaderComboBox.Items.Add(NOHEADER);
        }

        /// <summary>
        /// Allows the user to choose a file to process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                this.Filter = "SD files (*.sdf)|*.sdf|Text files (*.txt)|*.txt|Microsoft Excel Files (*.xls;*.xlsx)|*.xls;*.xlsx";
                OpenFileDialog fileDialog = new OpenFileDialog();

                if (!string.IsNullOrEmpty(_InputFileTextBox.Text.Trim()))
                {
                    if (File.Exists(_InputFileTextBox.Text.Trim()))
                    {
                        fileDialog.FileName = _InputFileTextBox.Text;
                    }
                }
                
                if(_InputFileTextBox.Text.ToLower().EndsWith(".sdf"))
                {
                    fileDialog.FilterIndex = 1;
                }
                else if(_InputFileTextBox.Text.ToLower().EndsWith(".txt"))
                {
                    fileDialog.FilterIndex = 2;
                }
                else if (_InputFileTextBox.Text.ToLower().EndsWith(".xls") ||
                    _InputFileTextBox.Text.ToLower().EndsWith(".xlsx"))
                {
                    fileDialog.FilterIndex = 3;
                }
                
                fileDialog.Filter = Filter;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    strFileName = fileDialog.FileName;
                    this.InputFile = fileDialog.FileName;
                    _InputFileTextBox.Text = fileDialog.FileName;

                    string extension = "*" + Path.GetExtension(this.InputFile).ToLower();
                    string filter = LookupFilter(fileDialog.FilterIndex);

                    if (filter.Contains(extension) == false)
                        fileDialog.FilterIndex = 0;

                    _InputFileTextBox.Tag = this.InputFile + "|" + fileDialog.FilterIndex;

                    AcceptButton.Enabled = true;
                    _SplitFileButton.Enabled = true;
                    AcceptButton.Focus();
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message + "\n" + ex.StackTrace;
                Trace.WriteLine(DateTime.Now, "Time ");
                Trace.WriteLine(message, "InputFileChooser_btnBrowse");
                Trace.Flush();
                MessageBox.Show(ex.Message, "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel();
        }

        /// <summary>
        /// Forces the control's Text property binding to update the property.
        /// Determines the availability of the 'Accept' button.
        /// </summary>
        /// <param name="sender">the textbox being edited</param>
        /// <param name="e">System.EventArgs</param>
        private void InputFileTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _WorkSheetComboBox.Items.Clear();
                _HeaderComboBox.SelectedIndex = -1;
                _DelimiterTextBox.Text = string.Empty;

                bool acceptEnabled = false;
                if (!string.IsNullOrEmpty(_InputFileTextBox.Text.Trim()) &&
                    File.Exists(_InputFileTextBox.Text.Trim()))
                {
                    acceptEnabled = File.Exists(_InputFileTextBox.Text.Trim());
                    AcceptButton.Enabled = acceptEnabled;
                    _SplitFileButton.Enabled = acceptEnabled;
                    _LoginGroupBox.Enabled = acceptEnabled;

                    if (this.GetFileType == SourceFileType.MSExcel)
                    {
                        ExcelOleDbReader reader = null;
                        try
                        {
                            reader = new ExcelOleDbReader(
                                                                _InputFileTextBox.Text.Trim()
                                                                , null
                                                                , MSOfficeVersion.Unknown
                                                                , false
                                                           );
                            List<string> tablelist = reader.GetWorksheetsList();
                            for (int i = 0; i < tablelist.Count; i++)
                            {
                                string worksheet = tablelist[i].ToString();
                                _WorkSheetComboBox.Items.Add(worksheet);
                            }
                        }
                        finally
                        {
                            if (reader != null)
                                reader.Close();
                        }
                        _WorkSheetComboBox.Enabled = true;
                        _HeaderComboBox.Enabled = true;
                        _DelimiterTextBox.Enabled = false;
                        _noStructureCheckBox.Checked = true;
                    }
                    else if (this.GetFileType == SourceFileType.CSV)
                    {
                        _WorkSheetComboBox.Enabled = false;
                        _HeaderComboBox.Enabled = true;
                        _DelimiterTextBox.Enabled = true;
                        _noStructureCheckBox.Checked = true;
                    }
                    else
                    {
                        _WorkSheetComboBox.Enabled = false;
                        _HeaderComboBox.Enabled = false;
                        _DelimiterTextBox.Enabled = false;
                        _noStructureCheckBox.Checked = false;
                    }
                }
                else
                {
                    AcceptButton.Enabled = false;
                    _SplitFileButton.Enabled = false;
                    _WorkSheetComboBox.Enabled = false;
                    _HeaderComboBox.Enabled = false;
                    _DelimiterTextBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message + "\n" + ex.StackTrace;
                Trace.WriteLine(DateTime.Now, "Time ");
                Trace.WriteLine(message, "InputFileChooser_InputFileTextBox_TextChanged");
                Trace.Flush();
                MessageBox.Show(ex.Message, "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            try
            {
                _split = false;

                _listFilterIndices.Clear();
                string[] strTag = (string.Empty + _InputFileTextBox.Tag).Split('|');
                if (strTag.Length == 2)
                {
                    if ((strTag[0] == InputFile) && (strTag[1] != "0")) _listFilterIndices.Add(Convert.ToInt32(strTag[1]));
                }
                if (_listFilterIndices.Count == 0)
                {
                    _listFilterIndices = LookupFilterIndices("*" + Path.GetExtension(InputFile).ToLower());
                }
                OnAccept(); // The parent will check to make certain the file exists
            }
            catch (Exception ex)
            {
                string message = ex.Message + "\n" + ex.StackTrace;
                Trace.WriteLine(DateTime.Now, "Time ");
                Trace.WriteLine(message, "InputFileChooser_Accept_Exception");
                Trace.Flush();
                MessageBox.Show(ex.Message, "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void _SplitFileButton_Click(object sender, EventArgs e)
        {
            _split = true;
            try
            {
                _listFilterIndices.Clear();
                string[] strTag = (string.Empty + _InputFileTextBox.Tag).Split('|');
                if (strTag.Length == 2)
                {
                    if ((strTag[0] == InputFile) && (strTag[1] != "0")) _listFilterIndices.Add(Convert.ToInt32(strTag[1]));
                }
                if (_listFilterIndices.Count == 0)
                {
                    _listFilterIndices = LookupFilterIndices("*" + Path.GetExtension(InputFile).ToLower());
                }
                OnAccept(); // The parent will check to make certain the file exists
            }
            catch (Exception ex)
            {
                string message = ex.Message + "\n" + ex.StackTrace;
                Trace.WriteLine(DateTime.Now, "Time ");
                Trace.WriteLine(message, "InputFileChooser_Accept_Exception");
                Trace.Flush();
                MessageBox.Show(ex.Message, "NCDS DataLoader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public SourceFileType FileType(string strExtension)
        {
            SourceFileType sourceType;
            switch (strExtension)
            {
                case ".sdf":
                    sourceType = SourceFileType.SDFile;
                    break;
                case ".xls":
                case ".xlsx":
                    sourceType = SourceFileType.MSExcel;
                    break;
                case ".txt":
                    sourceType = SourceFileType.CSV;
                    break;
                default:
                    sourceType = SourceFileType.Unknown;
                    break;
            }
            return sourceType;
        }


        private string LookupFilter(int vnFilterIndex)
        {
            string strRet = string.Empty;
            string[] strFilters = Filter.Split('|');
            if ((vnFilterIndex > 0) && (vnFilterIndex <= strFilters.Length / 2))
            {
                strRet = strFilters[vnFilterIndex * 2 - 2] + "|" + strFilters[vnFilterIndex * 2 - 1];
            }
            return strRet;
        }

        private int LookupFilter(string vstrFilter)
        {
            int nRet = 0;
            string[] strFilters = Filter.Split('|');
            for (int nFilterIndex = 1; nFilterIndex <= strFilters.GetLength(0) / 2; nFilterIndex++)
            {
                if (vstrFilter == (strFilters[nFilterIndex * 2 - 2] + "|" + strFilters[nFilterIndex * 2 - 1]))
                {
                    nRet = nFilterIndex;
                    break;
                }
            }
            return nRet;
        }
        private List<int> LookupFilterIndices(string fileExtension)
        {
            List<int> filterIndices = new List<int>();
            string[] filters = Filter.Split('|');
            if (fileExtension != "*")
            {
                for (int filterIndex = 1; filterIndex <= filters.GetLength(0) / 2; filterIndex++)
                {
                    if (filters[filterIndex * 2 - 1].ToLower().Contains(fileExtension))
                        filterIndices.Add(filterIndex);
                }
            }
            return filterIndices;
        }
    }
}
