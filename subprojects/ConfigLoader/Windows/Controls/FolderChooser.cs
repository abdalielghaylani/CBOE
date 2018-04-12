using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CambridgeSoft.COE.ConfigLoader.Windows.Controls
{
    /// <summary>
    /// UI to choose an input database; often a file.
    /// </summary>
    public partial class FolderChooser : UIBase
    {
        #region enums and types
        /// <summary>
        /// Indicates save versus load
        /// </summary>
        public enum DirectionType
        {
            /// <summary>
            /// Export
            /// </summary>
            Export,
            /// <summary>
            /// Import
            /// </summary>
            Import
        };
        #endregion

        #region data
        private DirectionType _DirectionType = DirectionType.Import;
        private System.Windows.Forms.Button _BrowseButton;
        private System.Windows.Forms.Label _FolderLabel;
        private System.Windows.Forms.TextBox _FolderTextBox;
        #endregion

        #region properties
        /// <summary>
        /// Get / set Direction
        /// </summary>
        public DirectionType Direction
        {
            get
            {
                return _DirectionType;
            }
            set
            {
                _DirectionType = value;
                _FolderLabel.Text = ((Direction == DirectionType.Import) ? "Input" : "Output") + " folder" + ":";
                StatusText = "Choose an " + ((Direction == DirectionType.Import) ? "input" : "output") + " folder";
                return;
            }
        } // Direction

        /// <summary>
        /// Get property for the selected input file
        /// </summary>
        public string Folder
        {
            get
            {
                return _FolderTextBox.Text;
            }
        } // Folder

        #endregion

        #region constructors
        /// <summary>
        /// ! Constructor
        /// </summary>
        public FolderChooser()
        {
            InitializeComponent();
            // Programmatically add control(s)
            // 
            SuspendLayout();
            // lblOutputType
            _FolderLabel = UIBase.GetLabel();
            _FolderLabel.AutoSize = true;
            _FolderLabel.TextAlign = ContentAlignment.MiddleLeft;
            _FolderLabel.Text = "Input file:";
            _FolderLabel.Width = _FolderLabel.PreferredWidth;
            Controls.Add(_FolderLabel);
            // _FolderTextBox
            _FolderTextBox = UIBase.GetTextBox();
            //_FolderTextBox.Enabled = false;  // Force user to Browse
            string strSelectedPath;
            Controls.Add(_FolderTextBox);
            // _BrowseButton
            _BrowseButton = UIBase.GetButton(ButtonType.Browse);
            Controls.Add(_BrowseButton);
            // btnAccept
            AcceptButton.Enabled = false;
            Controls.Add(AcceptButton);
            // btnCancel
            Controls.Add(CancelButton);
            // events
            _FolderTextBox.TextChanged += new EventHandler(txtInputFile_TextChanged);
            _BrowseButton.Click += new EventHandler(btnBrowse_Click);
            AcceptButton.Click += new EventHandler(AcceptButton_Click);
            CancelButton.Click += new EventHandler(CancelButton_Click);
            Layout += new LayoutEventHandler(FolderChooser_Layout);
            {
                System.Reflection.Assembly oAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                strSelectedPath = oAssembly.Location;
                int nAt = strSelectedPath.IndexOf(@"\ConfigLoader");
                strSelectedPath = strSelectedPath.Substring(0, nAt) + @"\Configuration";
            }
            _FolderTextBox.Text = strSelectedPath;
            //
            ResumeLayout(false);
            PerformLayout();
            Direction = DirectionType.Import;
            return;
        } // FolderChooser()
        #endregion

        #region event handlers
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlgFolderBrowserDialog = new FolderBrowserDialog();
            string strSelectedPath = "";
            if ((_FolderTextBox.Text.Length > 0) && Directory.Exists(_FolderTextBox.Text))
            {
                strSelectedPath = Folder;
            }
            else
            {
                System.Reflection.Assembly oAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                strSelectedPath = oAssembly.Location;
                int nAt = strSelectedPath.IndexOf(@"\ConfigLoader");
                strSelectedPath = strSelectedPath.Substring(0, nAt) + @"\Configuration";
            }
            dlgFolderBrowserDialog.SelectedPath = strSelectedPath;
            dlgFolderBrowserDialog.ShowNewFolderButton = (Direction == DirectionType.Export);
            if (dlgFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                _FolderTextBox.Text = dlgFolderBrowserDialog.SelectedPath.Trim();
                AcceptButton.Enabled = true;
                AcceptButton.Focus();
            }
            return;
        } // btnBrowse_Click()
        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel();
            return;
        } // CancelButton_Click()
        private void AcceptButton_Click(object sender, EventArgs e)
        {
            OnAccept(); // The parent will check to make certain the folder exists
            return;
        } // AcceptButton_Click()
        private void txtInputFile_TextChanged(object sender, EventArgs e)
        {
            UIBase.TextBox_CleanPath(_FolderTextBox);
            try
            {
                AcceptButton.Enabled = (_FolderTextBox.Text.Length > 0) && Directory.Exists(_FolderTextBox.Text);
            }
            catch
            {
                AcceptButton.Enabled = false;
            }
            return;
        } // txtInputFile_TextChanged()
        private void FolderChooser_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds"))
            {
                // Vertical
                int y = 0;
                int ySpacing = _FolderTextBox.Height;
                if (ySpacing < _BrowseButton.Height) ySpacing = _BrowseButton.Height;
                _FolderLabel.Top = y + (ySpacing - _FolderLabel.Height) / 2;
                _FolderTextBox.Top = y + (ySpacing - _FolderTextBox.Height) / 2;
                _BrowseButton.Top = y + (ySpacing - _BrowseButton.Height) / 2;
                y += ySpacing + UIBase.ExtraPadding.Top;
                CancelButton.Top = y;
                AcceptButton.Top = y;
                y += AcceptButton.Height;
                Height = y;
                // Horizontal
                int x = 0;
                _FolderLabel.Left = x;
                x += _FolderLabel.Width;
                _FolderTextBox.Left = x;
                {
                    int nWidth = MaximumSize.Width - _FolderTextBox.Left - _BrowseButton.Width;
                    int nWidthMax = TextRenderer.MeasureText("X",_FolderTextBox.Font).Width * 64;
                    if (nWidth > nWidthMax) nWidth = nWidthMax;
                    _FolderTextBox.Width = nWidth;
                }
                x += _FolderTextBox.Width;
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
        } // FolderChooser_Layout()
        #endregion
    } // class FolderChooser
}
