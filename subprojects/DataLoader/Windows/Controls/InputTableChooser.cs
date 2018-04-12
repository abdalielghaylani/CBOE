using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CambridgeSoft.COE.DataLoader.Windows.Controls
{
    /// <summary>
    /// UI to choose table within database when there are multiple tables
    /// </summary>
    public partial class InputTableChooser : UIBase
    {
        #region data
        // data members
        private System.Windows.Forms.ContainerControl _ContainerControl;
        private System.Windows.Forms.GroupBox _GroupBox;
        private System.Windows.Forms.RadioButton[] _RadioButton;
        #endregion

        #region properties
        /// <summary>
        /// Get property returning the test of the currently selected input table radio button
        /// </summary>
        public string InputTable
        {
            get
            {
                string strRet = string.Empty;
                foreach (RadioButton radn in _RadioButton)
                {
                    if (radn.Checked)
                    {
                        strRet = radn.Text;
                        break;
                    }
                }
                return strRet;
            }
        } // InputTable

        /// <summary>
        /// Set property to provide the list of tables from which to choose
        /// Has the effect of (re)building _GroupBox
        /// </summary>
        public List<string> TableList
        {
            set
            {
                List<string> listTableList = value;
                _RadioButton = new System.Windows.Forms.RadioButton[listTableList.Count];
                _ContainerControl.Controls.Clear();
                int nInputTable = 0;
                int xMax = 0;
                int y = 0;
                int nChecked = 0;
                foreach (string strTable in listTableList)
                {
                    _RadioButton[nInputTable] = UIBase.GetRadioButton();
                    _RadioButton[nInputTable].AutoSize = true;
                    _RadioButton[nInputTable].Tag = nInputTable;
                    _RadioButton[nInputTable].Text = strTable;
                    if (strTable == "MolTable") nChecked = nInputTable;
                    _RadioButton[nInputTable].Top = y;
                    _RadioButton[nInputTable].Left = _ContainerControl.Padding.Left;
                    y += _RadioButton[nInputTable].PreferredSize.Height;
                    _ContainerControl.Controls.Add(_RadioButton[nInputTable]);
                    if (xMax < _RadioButton[nInputTable].Right) xMax = _RadioButton[nInputTable].Right;
                    nInputTable++;
                }
                _ContainerControl.Width = xMax + 24 + _ContainerControl.Padding.Right;    // Leave space for scroll button
                _ContainerControl.Left = _ContainerControl.Margin.Left;
                _ContainerControl.Top = 0;
                {
                    Size size = TextRenderer.MeasureText(_GroupBox.Text, _GroupBox.Font);
                    _ContainerControl.Top += size.Height;
                }
                _ContainerControl.MaximumSize = new Size(_ContainerControl.Width, y);
                _GroupBox.Width = _ContainerControl.Margin.Left + _ContainerControl.Width + _ContainerControl.Margin.Right;
                _GroupBox.Height = _ContainerControl.Top + _ContainerControl.Height + _ContainerControl.Margin.Bottom;
                if (nChecked < _RadioButton.Length) _RadioButton[nChecked].Checked = true;  // Either MolTable or 0th entry
                return;
            }
        } // TableList
        #endregion

        #region constructors
        /// <summary>
        /// ! Constructor
        /// </summary>
        public InputTableChooser()
        {
            StatusText = "Choose an input table";
            InitializeComponent();
            // Programmatically add control(s)
            // 
            SuspendLayout();
            // _ContainerControl
            _ContainerControl = UIBase.GetContainerControlClass();
            _ContainerControl.Scroll += new ScrollEventHandler(ContainerControl_Scroll);
            // _RadioButton (added to _ContainerControl later when InputTables is set)
            // _GroupBox
            _GroupBox = UIBase.GetGroupBox();
            _GroupBox.Text = "Table";
            _GroupBox.Controls.Add(_ContainerControl);
            Controls.Add(_GroupBox);
            // btnAccept
            Controls.Add(AcceptButton);
            // btnCancel
            Controls.Add(CancelButton);
            // events
            AcceptButton.Click += new EventHandler(AcceptButton_Click);
            CancelButton.Click += new EventHandler(CancelButton_Click);
            Layout += new LayoutEventHandler(InputTableChooser_Layout);
            //
            ResumeLayout(false);
            PerformLayout();
        } // InputTableChooser()

        void ContainerControl_Scroll(object sender, ScrollEventArgs e)
        {
            ContainerControl oContainerControl = sender as ContainerControl;
            if (oContainerControl != null)
            {
                if (e.Type != ScrollEventType.ThumbTrack)
                {
                    e.NewValue = (int)((double)e.NewValue / oContainerControl.VerticalScroll.SmallChange) * oContainerControl.VerticalScroll.SmallChange;
                    oContainerControl.VerticalScroll.Value = e.NewValue;
                }
            }
            return;
        }
        #endregion

        #region events
        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel();
            return;
        }
        private void AcceptButton_Click(object sender, EventArgs e)
        {
            OnAccept();
            return;
        }
        private void InputTableChooser_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds"))
            {
                // Vertical
                int y = 0;
                _GroupBox.Top = y;
                {
                    int yMax = MaximumSize.Height;  // with respect to _ContainerControl
                    yMax -= UIBase.ExtraPadding.Top;
                    yMax -= AcceptButton.Height;
                    int nDelta = _GroupBox.Height - _ContainerControl.Height;
                    yMax -= nDelta;
                    if (yMax < 0) yMax = 0;
                    if (yMax > _ContainerControl.MaximumSize.Height) yMax = _ContainerControl.MaximumSize.Height;
                    if (_ContainerControl.Height != yMax)
                    {
                        _ContainerControl.Height = (yMax / _RadioButton[0].Height) * _RadioButton[0].Height;
                        _GroupBox.Height = _ContainerControl.Height + nDelta;
                        if (_ContainerControl.Height < _ContainerControl.MaximumSize.Height)
                        {
                            _ContainerControl.AutoScroll = true;
                            _ContainerControl.VerticalScroll.LargeChange = (_ContainerControl.Height / _RadioButton[0].Height) * _RadioButton[0].Height;
                            _ContainerControl.VerticalScroll.SmallChange = _RadioButton[0].Height;
                        }
                        else
                        {
                            _ContainerControl.AutoScroll = false;
                        }
                    }
                }
                y += _GroupBox.Height + UIBase.ExtraPadding.Top;
                CancelButton.Top = y;
                AcceptButton.Top = y;
                y += AcceptButton.Height;
                Height = y;
                // Horizontal
                int x = 0;
                _GroupBox.Left = x;
                x += _GroupBox.Width;
                Width = x;
                // Horizontal
                x = 0;
                CancelButton.Left = x;
                x += CancelButton.Width;
                AcceptButton.Left = x;
                x += AcceptButton.Width;
                if (Width < x) Width = x;
                //
                if (_GroupBox.Width < Width) _GroupBox.Width = Width;
                x = Width;
                x -= AcceptButton.Width;
                AcceptButton.Left = x;
                x -= CancelButton.Width;
                CancelButton.Left = x;
            } // if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds"))
            return;
        } // InputTableChooser_Layout()
        #endregion
    } // class InputTableChooser
}
