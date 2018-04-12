using System;
//using System.Collections.Generic;
//using System.ComponentModel;
using System.Drawing;
//using System.Data;
//using System.Text;
using System.Windows.Forms;

namespace CambridgeSoft.COE.ConfigLoader.Windows.Controls
{
    /// <summary>
    /// UI for CSLA login
    /// </summary>
    partial class Login : UIBase
    {
        // data members
        private readonly System.Windows.Forms.GroupBox _ConfigurationGroupBox;
        private readonly System.Windows.Forms.RadioButton[] _ConfigurationRadioButton;
        private readonly System.Windows.Forms.Label _ConfigurationLabel;
        private readonly System.Windows.Forms.GroupBox _LoginGroupBox;
        private readonly System.Windows.Forms.Label _PasswordLabel;
        private readonly System.Windows.Forms.TextBox _PasswordTextBox;
        private readonly System.Windows.Forms.Label _UserLabel;
        private readonly System.Windows.Forms.TextBox _UserTextBox;
        private bool _bRequired = false;

        #region properties
        /// <summary>
        /// Property for the required attribute
        /// </summary>
        private bool Required
        {
            get
            {
                return _bRequired;
            }
            set
            {
                _bRequired = value;
                return;
            }
        } // Required

        /// <summary>
        /// Get property to return the user
        /// </summary>
        public string User
        {
            get
            {
                return (Required || _ConfigurationRadioButton[0].Checked) ? _UserTextBox.Text : "";
            }
        } // User
        #endregion

        #region constructors
        /// <summary>
        /// ! Constructor
        /// </summary>
        public Login(bool vbRequired)
        {
            Required = vbRequired;
            StatusText = "Login for access to CBOE";
            InitializeComponent();
            // Programmatically add control(s)
            // 
            SuspendLayout();
            // _ConfigurationGroupBox
            _ConfigurationGroupBox = UIBase.GetGroupBox();
            if (Required)
            {
                _ConfigurationLabel = UIBase.GetLabel();
                _ConfigurationLabel.Text = "Logging in as:";
                _ConfigurationGroupBox.Controls.Add(_ConfigurationLabel);
            }
            else
            {
                _ConfigurationRadioButton = new RadioButton[2];
                _ConfigurationRadioButton[0] = UIBase.GetRadioButton();
                _ConfigurationRadioButton[0].Checked = true;
                _ConfigurationRadioButton[0].CheckedChanged += new EventHandler(ConfigurationRadioButton_CheckedChanged);
                _ConfigurationRadioButton[0].Text = "Logging in as:";
                _ConfigurationGroupBox.Controls.Add(_ConfigurationRadioButton[0]);
                _ConfigurationRadioButton[1] = UIBase.GetRadioButton();
                _ConfigurationRadioButton[1].Checked = false;
                _ConfigurationRadioButton[1].Text = "Not logging in";
                _ConfigurationGroupBox.Controls.Add(_ConfigurationRadioButton[1]);
            }
            // _LoginGroupBox
            _LoginGroupBox = UIBase.GetGroupBox();
            // _UserLabel
            _UserLabel = UIBase.GetLabel();
            _UserLabel.TextAlign = ContentAlignment.MiddleLeft;
            _UserLabel.Text = "User:";
            _UserLabel.Width = _UserLabel.PreferredWidth;
            _LoginGroupBox.Controls.Add(_UserLabel);
            // _UserTextBox
            _UserTextBox = UIBase.GetTextBox();
            _LoginGroupBox.Controls.Add(_UserTextBox);
            // _PasswordLabel
            _PasswordLabel = UIBase.GetLabel();
            _PasswordLabel.TextAlign = ContentAlignment.MiddleLeft;
            _PasswordLabel.Text = "Password:";
            _PasswordLabel.Width = _PasswordLabel.PreferredWidth;
            _LoginGroupBox.Controls.Add(_PasswordLabel);
            // _PasswordTextBox
            _PasswordTextBox = UIBase.GetTextBox();
            _PasswordTextBox.PasswordChar = '*';
            _LoginGroupBox.Controls.Add(_PasswordTextBox);
            _ConfigurationGroupBox.Controls.Add(_LoginGroupBox);
            // label heights
            _UserLabel.Height = _UserTextBox.Height;
            _PasswordLabel.Height = _PasswordTextBox.Height;
            // widths
            {
                int[] xMax = new int[2] { 0, 0 }; // Per column
                xMax[0] = _UserLabel.Width;
                xMax[1] = _UserTextBox.Width;
                if (xMax[0] < _PasswordLabel.Width) xMax[0] = _PasswordLabel.Width;
                if (xMax[1] < _PasswordTextBox.Width) xMax[1] = _PasswordTextBox.Width;
                _UserLabel.Width = _PasswordLabel.Width = xMax[0];
            }
            Controls.Add(_ConfigurationGroupBox);
            // btnAccept
            AcceptButton.Enabled = false;
            Controls.Add(AcceptButton);
            // btnCancel
            CancelButton.Text = "Exit";
            Controls.Add(CancelButton);
            // events
            _UserTextBox.TextChanged += new EventHandler(UserTextBox_TextChanged);
            _PasswordTextBox.TextChanged += new EventHandler(PasswordTextBox_TextChanged);
            AcceptButton.Click += new EventHandler(AcceptButton_Click);
            CancelButton.Click += new EventHandler(CancelButton_Click);
            Layout += new LayoutEventHandler(Login_Layout);
            //
            ResumeLayout(false);
            PerformLayout();
            return;
        } // Login()

        #endregion

        #region event handlers
        private void AcceptButton_Click(object sender, EventArgs e)
        {
            if (Required || _ConfigurationRadioButton[0].Checked)
            {
                bool bIsAuthenticated = false;
                string strMessage = "Not a valid login. No such user or invalid password.";
                {
                    Ph.Maximum = -1;
                    Ph.SupportsCancellation = false;
                    Ph.ProgressSection(delegate() /* Login::AcceptButton_Click (unknown, no Cancel) */
                    {
                        try
                        {
                            Ph.StatusText = "Logging you in. Please wait...";
                            bIsAuthenticated = CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.Login(_UserTextBox.Text, _PasswordTextBox.Text);
                            Csla.ApplicationContext.GlobalContext["SimulationMode"] = false;
                            Csla.ApplicationContext.GlobalContext["AppName"] = "REGISTRATION";  // WJC need to set in OutputObjectXxx
                        }
                        catch (Csla.DataPortalException cex)
                        {
                            strMessage = cex.BusinessException.Message;
                        }
                        catch (Exception ex)
                        {
                            //CSBR-103803
                            strMessage = ex.Message;
                        }
                    });
                }
                if (bIsAuthenticated)
                {
                    OnAccept();
                }
                else
                {
                    MessageBox.Show(strMessage, "Login", MessageBoxButtons.OK);
                    AcceptButton.Enabled = false;
                    _PasswordTextBox.SelectAll();
                    _PasswordTextBox.Focus();
                }
            } else {
                CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.Logout();
                OnAccept();
            } // if (_ConfigurationRadioButton[0].Checked)
            return;
        } // AcceptButton_Click()

        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel();
            return;
        } // CancelButton_Click()

        void ConfigurationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            _LoginGroupBox.Enabled = rb.Checked;
            if (rb.Checked)
            {
                AcceptButton.Enabled = (_UserTextBox.Text.Length > 0) && (_PasswordTextBox.Text.Length > 0);
            }
            else
            {
                AcceptButton.Enabled = true;
            }
            return;
        } // ConfigurationRadioButton_CheckedChanged()

        private void Login_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds"))
            {
                // Vertical
                {
                    int yOuter = 0;
                    _ConfigurationGroupBox.Top = yOuter;
                    yOuter += _ConfigurationGroupBox.Padding.Top;
                    yOuter += 9;    // WJC need property
                    if (Required)
                    {
                        _ConfigurationLabel.Top = yOuter;
                        yOuter += _ConfigurationLabel.Height;
                        _LoginGroupBox.Top = _ConfigurationLabel.Top + _ConfigurationLabel.Height;
                    }
                    else
                    {
                        _ConfigurationRadioButton[0].Top = yOuter;
                        yOuter += _ConfigurationRadioButton[0].Height;
                        _LoginGroupBox.Top = _ConfigurationRadioButton[0].Top + _ConfigurationRadioButton[0].Height;
                    }
                    {
                        int y = 0;
                        y += (_ConfigurationGroupBox.Margin.Top + _LoginGroupBox.Padding.Top);
                        y += _UserLabel.Height / 2;
                        int ySpacing = _UserTextBox.Height;                      // We know the text box is taller than the label
                        _UserLabel.Top = y;
                        _UserTextBox.Top = y;
                        y += ySpacing + UIBase.ExtraPadding.Top;
                        _PasswordLabel.Top = y;
                        _PasswordTextBox.Top = y;
                        y += ySpacing;
                        y += _UserLabel.Height / 2;
                        y += (_LoginGroupBox.Padding.Bottom + _ConfigurationGroupBox.Margin.Bottom);
                        _LoginGroupBox.Height = y;
                    }
                    yOuter += _LoginGroupBox.Height;
                    yOuter += _LoginGroupBox.Padding.Bottom;
                    if (Required == false)
                    {
                        _ConfigurationRadioButton[1].Top = yOuter;
                        yOuter += _ConfigurationRadioButton[1].Height;
                        yOuter += _ConfigurationGroupBox.Padding.Bottom;
                    }
                    _ConfigurationGroupBox.Height = yOuter;
                    yOuter += AcceptButton.Margin.Top;
                    AcceptButton.Top = CancelButton.Top = yOuter;
                    yOuter += AcceptButton.Height;
                    Height = yOuter;
                }
                // Horizontal
                {
                    int xOuter = 0;
                    _ConfigurationGroupBox.Left = xOuter;
                    xOuter += (_ConfigurationGroupBox.Margin.Left + _LoginGroupBox.Padding.Left);
                    if (Required)
                    {
                        _ConfigurationLabel.Left = xOuter;
                    }
                    else
                    {
                        _ConfigurationRadioButton[0].Left = xOuter;
                        _ConfigurationRadioButton[1].Left = xOuter;
                    }
                    _LoginGroupBox.Left = xOuter;
                    {
                        int x = 0;
                        x += (_ConfigurationGroupBox.Margin.Left + _LoginGroupBox.Padding.Left);
                        _UserLabel.Left = _PasswordLabel.Left = x;
                        x = (_ConfigurationGroupBox.Margin.Left + _LoginGroupBox.Padding.Left) + _UserLabel.Width;
                        _UserTextBox.Left = _PasswordTextBox.Left = x;
                        x += _UserTextBox.Width;
                        x += (_ConfigurationGroupBox.Margin.Right + _LoginGroupBox.Padding.Right);
                        _LoginGroupBox.Width = x;
                    }
                    _ConfigurationGroupBox.Width = Width = (_ConfigurationGroupBox.Margin.Left + _LoginGroupBox.Padding.Left) + _LoginGroupBox.Width + (_LoginGroupBox.Padding.Right + _ConfigurationGroupBox.Margin.Right);
                    {
                        int x = 0;
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
                    }
                }
            }
            return;
        } // Login_Layout()

        private void PasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            _PasswordTextBox.Text = _PasswordTextBox.Text.Trim();
            AcceptButton.Enabled = (_UserTextBox.Text.Length > 0) && (_PasswordTextBox.Text.Length > 0);
            return;
        } // PasswordTextBox_TextChanged()

        private void UserTextBox_TextChanged(object sender, EventArgs e)
        {
            _UserTextBox.Text = _UserTextBox.Text.Trim();
            AcceptButton.Enabled = (_UserTextBox.Text.Length > 0) && (_PasswordTextBox.Text.Length > 0);
            return;
        } // UserTextBox_TextChanged()
        #endregion
    } // class Login
}
