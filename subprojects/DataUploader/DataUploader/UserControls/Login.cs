using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.DataLoader.UserControls
{
    public partial class Login : UserControl
    {

        #region fields

        private string _statusText = "Unknown!";

        #endregion

        #region properties

        public string StatusText
        {
            get
            {
                return this._statusText;
            }
            set
            {
                this._statusText = value;
            }
        }

        public string User
        {
            get
            {
                return (this._ConfigurationRadioButtonOne.Checked ) ?
                    this._UserTextBox.Text : string.Empty;
            }
        }

        #endregion

        #region events

        public event EventHandler Authenticated;

        public event EventHandler Cancel;

        #endregion

        #region Constructor

        public Login()
        {
            InitializeComponent();
            this.InitControlsSetting();
        }

        #endregion

        #region methods

        private void InitControlsSetting()
        {
            //control property setting
            this._buttonLogIn.Enabled = false;
            this._ConfigurationRadioButtonOne.Checked = true;
            this._PasswordTextBox.PasswordChar = '*';
            
            //events
            this._ConfigurationRadioButtonOne.CheckedChanged+=new EventHandler(ConfigurationRadioButtonOne_CheckedChanged);
            this._UserTextBox.TextChanged += new EventHandler(UserTextBox_TextChanged);
            this._PasswordTextBox.TextChanged += new EventHandler(PasswordTextBox_TextChanged);
            this._buttonLogIn.Click += new EventHandler(ButtonLogIn_Click);
            this._buttonCancel.Click += new EventHandler(ButtonCancel_Click);
        }

        private void OnAuthenticated(EventArgs args)
        {
            if (this.Authenticated != null)
            {
                this.Authenticated(this, args);
            }
        }

        public void OnAuthenticated()
        {
            this.OnAuthenticated(new EventArgs());
            return;
        }

        private void OnCancel(EventArgs args)
        {
            if (this.Cancel != null)
            {
                this.Cancel(this, args);
            }
        }

        public void OnCancel()
        {
            this.OnCancel(new EventArgs());
            return;
        }

        #endregion

        #region event handlers

        private void ConfigurationRadioButtonOne_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            _LoginGroupBox.Enabled = rb.Checked;
            if (rb.Checked)
            {
                this._buttonLogIn.Enabled = (_UserTextBox.Text.Length > 0) && (_PasswordTextBox.Text.Length > 0);
            }
            else
            {
                this._buttonLogIn.Enabled = true;
            }
            return;
        }

        private void UserTextBox_TextChanged(object sender, EventArgs e)
        {
            this._UserTextBox.Text = this._UserTextBox.Text.Trim();
            this._buttonLogIn.Enabled = (this._UserTextBox.Text.Length > 0) && (this._PasswordTextBox.Text.Length > 0);
            return;
        }

        private void PasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            this._PasswordTextBox.Text = this._PasswordTextBox.Text.Trim();
            this._buttonLogIn.Enabled = (this._UserTextBox.Text.Length > 0) && (this._PasswordTextBox.Text.Length > 0);
            return;
        }

        private void ButtonLogIn_Click(object sender, EventArgs e)
        {
            if (this._ConfigurationRadioButtonOne.Checked)
            {
                bool bIsAuthenticated = false;
                string strMessage = "Unable to log in.";
                {
                    try
                    {
                        this.StatusText = "Logging you in. Please wait...";
                        string login = _UserTextBox.Text;
                        string pwd = _PasswordTextBox.Text;

                        bIsAuthenticated = CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.Login(login, pwd);
                    }
                    catch (Csla.DataPortalException cex)
                    {
                        bIsAuthenticated = false;
                        strMessage = cex.BusinessException.GetBaseException().Message;
                    }
                    catch (Exception ex)
                    {
                        bIsAuthenticated = false;
                        strMessage = ex.GetBaseException().Message;
                    }
                }
                if (bIsAuthenticated)
                {
                    this.OnAuthenticated();
                }
                else
                {
                    MessageBox.Show(strMessage, "Login", MessageBoxButtons.OK);
                    this._buttonLogIn.Enabled = false;
                    _PasswordTextBox.SelectAll();
                    _PasswordTextBox.Focus();
                }
            }
            else
            {
                CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.Logout();
                this.OnAuthenticated();
            }
            return;
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.OnCancel();
        }

        #endregion

    }
}
