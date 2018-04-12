using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CambridgeSoft.COE.ConfigLoader.Windows.Controls
{
    /// <summary>
    /// UI for choosing the output type (task)
    /// </summary>
    public partial class OutputTypeChooser : UIBase
    {
        #region data
        private System.Windows.Forms.GroupBox _OutputTypeGroupBox;
        private System.Windows.Forms.RadioButton[] _OutputTypeRadioButton;
        private System.Windows.Forms.RadioButton[] _TaskRadioButton;
        #endregion

        #region properties
        /// <summary>
        /// Get property that returns the tag (number) of the chosen output type
        /// </summary>
        public int OutputType
        {
            get
            {
                int nRet = Convert.ToInt32(_OutputTypeGroupBox.Tag);
                return (nRet >= 0) ? nRet : - 1;
            }
        } // OutputType

        /// <summary>
        /// Get property that returns the tag (number) of the chosen task
        /// </summary>
        public int Task
        {
            get
            {
                int nRet = Convert.ToInt32(_OutputTypeGroupBox.Tag);
                return (nRet < 0) ? (-1 - nRet) : -1;
            }
        } // Task
        #endregion

        #region constructors
        /// <summary>
        /// ! Constructor
        /// </summary>
        public OutputTypeChooser()
        {
            StatusText = "Choose a data management task";
            InitializeComponent();
            // Programmatically add control(s)
            // 
            SuspendLayout();
            // _OutputTypeGroupBox
            _OutputTypeGroupBox = UIBase.GetGroupBox();
            _OutputTypeGroupBox.Tag = -1;
            _OutputTypeGroupBox.Text = "Task";
            _OutputTypeGroupBox.Visible = false;
            _OutputTypeGroupBox.Width = 0;
            // _OutputTypeRadioButton (added later when OutputTypes is set)
            // _TaskRadioButton (added later when Tasks is set)
            Controls.Add(_OutputTypeGroupBox);
            // btnAccept
            Controls.Add(AcceptButton);
            // btnCancel
            Controls.Add(CancelButton);
            // events
            AcceptButton.Click += new EventHandler(AcceptButton_Click);
            CancelButton.Click += new EventHandler(CancelButton_Click);
            Layout += new LayoutEventHandler(OutputTypeChooser_Layout);
            //
            ResumeLayout(false);
            PerformLayout();
            return;
        } // OutputTypeChooser()
        #endregion

        #region event handlers
        private void AcceptButton_Click(object sender, EventArgs e)
        {
            OnAccept();
            return;
        } // AcceptButton_Click()

        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel();
            return;
        } // CancelButton_Click()

        private void OutputTypeChooser_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds"))
            {
                // Vertical
                int y = 0;
                _OutputTypeGroupBox.Top = y;
                y += _OutputTypeGroupBox.Height;
                y += UIBase.ExtraPadding.Top;
                CancelButton.Top = y;
                AcceptButton.Top = y;
                y += AcceptButton.Height;
                Height = y;
                // Horizontal
                int x = 0;
                _OutputTypeGroupBox.Left = x;
                x += _OutputTypeGroupBox.Width;
                Width = x;
                // Horizontal
                x = 0;
                CancelButton.Left = x;
                x += CancelButton.Width;
                AcceptButton.Left = x;
                x += AcceptButton.Width;
                if (Width < x) Width = x;
                //
                if (_OutputTypeGroupBox.Width < Width) _OutputTypeGroupBox.Width = Width;
                x = Width;
                x -= AcceptButton.Width;
                AcceptButton.Left = x;
                x -= CancelButton.Width;
                CancelButton.Left = x;
            }
            return;
        } // OutputTypeChooser_Layout()

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton oRadioButtonThis = sender as RadioButton;
            if (oRadioButtonThis.Checked)
            {
                GroupBox oGroupBoxThis = (GroupBox)oRadioButtonThis.Parent;
                oGroupBoxThis.Tag = oRadioButtonThis.Tag;
            }
            return;
        } // RadioButton_CheckedChanged()

        #endregion

        #region methods
        /// <summary>
        /// Select the requested output type
        /// </summary>
        /// <param name="vstrOutputType"></param>
        /// <returns></returns>
        public bool Select(string vstrOutputType)
        {
            bool bRet = false;
            foreach (RadioButton rb in _OutputTypeGroupBox.Controls)
            {
                bRet |= (rb.Checked = (rb.Text == vstrOutputType));
            }
            return bRet;
        } // Select()

        /// <summary>
        /// Set the available output types and tasks
        /// (Re)builds _OutputTypeGroupBox as a result
        /// </summary>
        /// <param name="voutputDictionary"></param>
        /// <param name="vlistTasks"></param>
        public void Setup(Dictionary<string, string> voutputDictionary, List<string> vlistTasks)
        {
            _OutputTypeGroupBox.Controls.Clear();
            {
                _OutputTypeRadioButton = new System.Windows.Forms.RadioButton[voutputDictionary.Count];
                int nOutputType = 0;
                int xMax = 0;
                int y = _OutputTypeGroupBox.Padding.Top;
                y += UIBase.ExtraPadding.Top;
                foreach (KeyValuePair<string, string> kvp in voutputDictionary)
                {
                    string strOutputType = kvp.Value;
                    _OutputTypeRadioButton[nOutputType] = UIBase.GetRadioButton();
                    _OutputTypeRadioButton[nOutputType].AutoSize = true;
                    _OutputTypeRadioButton[nOutputType].Tag = nOutputType;
                    _OutputTypeRadioButton[nOutputType].Text = strOutputType;
                    y += _OutputTypeRadioButton[nOutputType].PreferredSize.Height / 2;
                    _OutputTypeRadioButton[nOutputType].Top = y;
                    _OutputTypeRadioButton[nOutputType].Left = _OutputTypeGroupBox.Padding.Left + UIBase.ExtraPadding.Left;
                    y += _OutputTypeRadioButton[nOutputType].PreferredSize.Height / 2;
                    _OutputTypeRadioButton[nOutputType].CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
                    _OutputTypeGroupBox.Controls.Add(_OutputTypeRadioButton[nOutputType]);
                    if (xMax < _OutputTypeRadioButton[nOutputType].Right) xMax = _OutputTypeRadioButton[nOutputType].Right + UIBase.ExtraPadding.Right;
                    nOutputType++;
                } // foreach (string strOutputType in listOutputTypes)
                y += _OutputTypeRadioButton[0].Height / 2;
                y += UIBase.ExtraPadding.Bottom;
                _OutputTypeGroupBox.Width = xMax + _OutputTypeGroupBox.Padding.Right;
                _OutputTypeGroupBox.Height = y + _OutputTypeGroupBox.Padding.Bottom;
                _OutputTypeGroupBox.Visible = (_OutputTypeRadioButton.Length > 0);
                if (_OutputTypeGroupBox.Visible) _OutputTypeRadioButton[0].Checked = true;
            }
            {
                List<string> listTasks = vlistTasks;
                _TaskRadioButton = new System.Windows.Forms.RadioButton[listTasks.Count];
                int nTask = 0;
                int xMax = 0;
                int y;
                {
                    RadioButton rb = _OutputTypeRadioButton[_OutputTypeRadioButton.Length - 1];
                    y = rb.Bottom - rb.PreferredSize.Height / 2;
                }
                foreach (string strTask in listTasks)
                {
                    _TaskRadioButton[nTask] = UIBase.GetRadioButton();
                    _TaskRadioButton[nTask].AutoSize = true;
                    _TaskRadioButton[nTask].Tag = (-1 - nTask);
                    _TaskRadioButton[nTask].Text = strTask;
                    y += _TaskRadioButton[nTask].PreferredSize.Height / 2;
                    _TaskRadioButton[nTask].Top = y;
                    _TaskRadioButton[nTask].Left = _OutputTypeGroupBox.Padding.Left + UIBase.ExtraPadding.Left;
                    y += _TaskRadioButton[nTask].PreferredSize.Height / 2;
                    _TaskRadioButton[nTask].CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
                    _OutputTypeGroupBox.Controls.Add(_TaskRadioButton[nTask]);
                    if (xMax < _TaskRadioButton[nTask].Right) xMax = _TaskRadioButton[nTask].Right + UIBase.ExtraPadding.Right;
                    nTask++;
                } // foreach (string strOutputType in listOutputTypes)
                if (listTasks.Count > 0)
                {
                    y += _TaskRadioButton[0].Height / 2;
                    y += UIBase.ExtraPadding.Bottom;
                    if (_OutputTypeGroupBox.Width < (xMax + _OutputTypeGroupBox.Padding.Right)) _OutputTypeGroupBox.Width = (xMax + _OutputTypeGroupBox.Padding.Right);
                    _OutputTypeGroupBox.Height = y + _OutputTypeGroupBox.Padding.Bottom;
                }
            }
            return;
        } // Setup()
        #endregion
    } // class OutputTypeChooser
}
