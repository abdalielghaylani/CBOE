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
    /// <summary>
    /// UI to choose an input database; often a file.
    /// </summary>
    public partial class InputFileChooser : UIBase
    {
        //Constants
        private const string TEXT_PROPERTY = "Text";

        //Controls
        private Button _BrowseButton;
        private Label _InputFileLabel;
        private TextBox _InputFileTextBox;
        private OpenFileDialog fileDialog = new OpenFileDialog();

        //Data-container
        List<int> _listFilterIndices = new List<int>();

        /// <summary>
        /// ! Constructor
        /// </summary>
        public InputFileChooser()
        {
            StatusText = "Choose an input file";
            InitializeComponent();

            //
            // Programmatically add control(s)
            // 

            SuspendLayout();

            // output type
            _InputFileLabel = UIBase.GetLabel();
            _InputFileLabel.TextAlign = ContentAlignment.MiddleLeft;
            _InputFileLabel.Text = "Input file:";
            _InputFileLabel.Width = _InputFileLabel.PreferredWidth;
            Controls.Add(_InputFileLabel);

            // file full-name
            _InputFileTextBox = UIBase.GetTextBox();
            Controls.Add(_InputFileTextBox);

            // file-browse button
            _BrowseButton = UIBase.GetButton(ButtonType.Browse);
            Controls.Add(_BrowseButton);

            // accept/cancel
            AcceptButton.Enabled = false;
            Controls.Add(AcceptButton);
            Controls.Add(CancelButton);

            // events
            _InputFileTextBox.TextChanged += new EventHandler(InputFileTextBox_TextChanged);
            _BrowseButton.Click += new EventHandler(btnBrowse_Click);
            AcceptButton.Click += new EventHandler(AcceptButton_Click);
            CancelButton.Click += new EventHandler(CancelButton_Click);
            Layout += new LayoutEventHandler(InputFileChooser_Layout);

            ResumeLayout(false);
            PerformLayout();

            this._InputFileTextBox.DataBindings.Add(TEXT_PROPERTY, this, "InputFile");

            fileDialog.CheckFileExists = true;
            fileDialog.Multiselect = false;
            fileDialog.ReadOnlyChecked = true;
            fileDialog.RestoreDirectory = true;
            fileDialog.ShowReadOnly = false;
            fileDialog.Title = Application.ProductName + " - Input source file";
        }

        #region > Properties <

        string _filter;
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

        private string _inputFilePath;
        /// <summary>
        /// Get property for the selected input file
        /// </summary>
        public string InputFile
        {
            get { return _inputFilePath; }
            set { _inputFilePath = value; }
        }

        string _inputFilter;
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

        #endregion

        #region > Event handlers <

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

        /// <summary>
        /// Allows the user to choose a file to process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            //reset the file filters?
            fileDialog.Filter = Filter;

            //if there is a file listed, use it to glean the initial directory AND the file extension filter
            if (!string.IsNullOrEmpty(this.InputFile))
            {
                //unfortunately, this is the best way to check if the value provided is a valid path
                try
                {
                    FileInfo fi = new FileInfo(this.InputFile);
                    if (fi.Exists)
                        fileDialog.InitialDirectory = fi.DirectoryName;
                }
                catch { }

                if (!string.IsNullOrEmpty(this.InputFilter))
                    fileDialog.FilterIndex = LookupFilter(InputFilter);
            }

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                this.InputFile = fileDialog.FileName.Trim();
                //Coverity fix- CID 13123
                Binding binding = _InputFileTextBox.DataBindings[TEXT_PROPERTY];
                if (binding != null)
                    binding.ReadValue();

                string extension = "*" + Path.GetExtension(this.InputFile).ToLower();
                string filter = LookupFilter(fileDialog.FilterIndex);

                if (filter.Contains(extension) == false)
                    fileDialog.FilterIndex = 0;

                _InputFileTextBox.Tag = this.InputFile + "|" + fileDialog.FilterIndex;

                AcceptButton.Enabled = true;
                AcceptButton.Focus();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel();
        }
        
        private void AcceptButton_Click(object sender, EventArgs e)
        {
            _listFilterIndices.Clear();
            string[] strTag = (string.Empty+_InputFileTextBox.Tag).Split('|');
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
        
        /// <summary>
        /// Forces the control's Text property binding to update the property.
        /// Determines the availability of the 'Accept' button.
        /// </summary>
        /// <param name="sender">the textbox being edited</param>
        /// <param name="e">System.EventArgs</param>
        private void InputFileTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (t != null)
            {
                //Coverity fix - CID 13122
                Binding binding = t.DataBindings[TEXT_PROPERTY];
                if (binding != null)
                   t.DataBindings[TEXT_PROPERTY].WriteValue();
                bool acceptEnabled = false;
                acceptEnabled = File.Exists(this.InputFile);
                AcceptButton.Enabled = acceptEnabled;
                
            }
        }
        
        private void InputFileChooser_Layout(object sender, LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds"))
            {
                // Vertical
                int y = 0;
                int ySpacing = _InputFileTextBox.Height;
                if (ySpacing < _BrowseButton.Height) ySpacing = _BrowseButton.Height;
                _InputFileLabel.Top = y + (ySpacing - _InputFileLabel.Height) / 2;
                _InputFileTextBox.Top = y + (ySpacing - _InputFileTextBox.Height) / 2;
                _BrowseButton.Top = y + (ySpacing - _BrowseButton.Height) / 2;
                y += ySpacing + UIBase.ExtraPadding.Top;
                CancelButton.Top = y;
                AcceptButton.Top = y;
                y += AcceptButton.Height;
                Height = y;
                // Horizontal
                int x = 0;
                _InputFileLabel.Left = x;
                x += _InputFileLabel.Width;
                _InputFileTextBox.Left = x;
                {
                    int nWidth = MaximumSize.Width - _InputFileTextBox.Left - _BrowseButton.Width;
                    int nWidthMax = TextRenderer.MeasureText("X",_InputFileTextBox.Font).Width * 64;
                    if (nWidth > nWidthMax) nWidth = nWidthMax;
                    _InputFileTextBox.Width = nWidth;
                }
                x += _InputFileTextBox.Width;
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
        }

        #endregion
    }
}
