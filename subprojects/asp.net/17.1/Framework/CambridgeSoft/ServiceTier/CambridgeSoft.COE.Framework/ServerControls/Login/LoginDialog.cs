using System;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using System.Diagnostics;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Drawing;

namespace CambridgeSoft.COE.Framework.ServerControls.Login
{
    public partial class LoginDialog : Form
    {
        #region Variables
        private bool _isLogged = false;
        private string _lastUsedServer = string.Empty;
        private int _maxRetries;
        private int _retries;
        //11.0.3
        private bool? _serverDialogInstance = null;
        private int _serverIndex = 0;
        #endregion

        #region Events
        public event EventHandler<LoginEventArgs> ValidateUser;

        public void OnValidateUser()
        {
            try
            {
                if (ValidateUser != null)
                {
                    LoginEventArgs eventArgs = new LoginEventArgs(this.UserName, this.Password, this.Server, this.SSL);
                    ValidateUser.Invoke(this, eventArgs);

                    _isLogged = eventArgs.Succeeded;
                }
            }
            catch (Exception exception)
            {
                this.StatusToolTip.SetToolTip(this.TextBoxUserName, exception.GetBaseException() == null ? exception.Message : exception.GetBaseException().Message);
            }
        }
        #endregion

        #region Properties
        public Image Logo
        {
            get
            {
                return LogoPictureBox.Image;
            }
            set
            {
                LogoPictureBox.Image = value;
            }
        }
        public int MaxRetries
        {
            get
            {
                return _maxRetries;
            }
            set
            {
                _maxRetries = value;
            }
        }
        public string UserName
        {
            get
            {
                return TextBoxUserName.Text;
            }
            set
            {
                TextBoxUserName.Text = value;
            }
        }
        public string Password
        {
            get
            {
                return TextBoxPassword.Text;
            }
            set
            {
                TextBoxPassword.Text = value;
            }
        }

        public string Server
        {
            get
            {
                // 11.0.3 - Getting only the servername as now the dropdown shows username along with servername
                //return ServerComboBox.Text.Trim();
                return ServerNameUtils.BeforeDelimiter(ServerComboBox.Text, '[').Trim();
            }
            set
            {
                ServerComboBox.SelectedIndex = ServerComboBox.FindStringExact(value);
            }
        }

        public bool SavePasswd
        {
            get
            {
                return SavePasswordCheckBox.Checked;
            }
            set
            {
                SavePasswordCheckBox.Checked = value;
            }
        }
        public bool? ServerInstanceDialog
        {
            get
            {
                if (_serverDialogInstance.HasValue)
                    return _serverDialogInstance;
                else
                    return null;
            }
            set
            {
                _serverDialogInstance = value;
            }
        }

        public int ServerIndex
        {
            get
            {
                return _serverIndex;
            }
            set
            {
                _serverIndex = ServerComboBox.SelectedIndex;
            }
        }

        public bool SSL
        {
            get
            {
                if (ServerNameUtils.BeforeDelimiter(ServerComboBox.Text, '/').Trim().Equals("SSL", StringComparison.OrdinalIgnoreCase))
                    return true;
                else return false;
            }
        }

        public MRUEntry CurrentServerMRU
        {
            get
            {
                // create an MRUEntry from data currently in dialog, whether on drop-down or not
                string serverName = ServerNameUtils.BeforeDelimiter(Server, '[').Trim();
                return new MRUEntry(serverName, UserName, Password, SavePasswd, SSL);
            }
        }

        public string MRUListDefaultConfigPath
        {
            get
            {
                return CambridgeSoft.COE.Framework.COEConfigurationService.COEConfigurationBO.ConfigurationBaseFilePath + COEAppName.Get(); ;
            }
        }

        public string MRUListSerializeFileName
        {
            get
            {
                return "MRUList.xml";
            }
        }
        #endregion

        #region Constructors
        public LoginDialog()
        {
            InitializeComponent();
            PerformInitialization();
        }

        public LoginDialog(string lastUsedServer)
            : this()
        {
            if (!string.IsNullOrEmpty(lastUsedServer))
            {
                // 11.0.3 - Getting only the servername as the lastUsedServer contains username along with servername
                //_lastUsedServer = lastUsedServer;
                _lastUsedServer = ServerNameUtils.BeforeDelimiter(lastUsedServer, '[').Trim();

                Server = lastUsedServer;
            }
        }

        public LoginDialog(string lastUsedServer, string caption)
            : this(lastUsedServer)
        {
            this.Text = caption;
        }

        // Advance Export - Overwrite the LoginDialog method to auto fill the user and server details
        public LoginDialog(string serverName, string userName, bool ssl)
            : this()
        {

            if (!string.IsNullOrEmpty(serverName))
            {
                this._lastUsedServer = ServerNameUtils.BeforeDelimiter(serverName, '[').Trim();

                ServerComboBox.Items.Clear();
                ServerComboBox.Items.Add(serverName);
                /*MRUEntry mru = new MRUEntry(this._lastUsedServer, userName, "", true, ssl);
                if (!this.IsMRUEntryExists(mru))
                {
                    MRUList.Inst.Insert(0, mru);
                    this.MRUListToCombo(this.ServerComboBox);
                    this.MRUListToDisk(this.ServerComboBox);
                }*/
                this.Server = serverName;
                this.UserName = userName;
            }
        }


        #endregion

        #region Private Methods

        private bool IsMRUEntryExists(MRUEntry mru)
        {
            return MRUList.Inst.Exists(delegate(MRUEntry pmru)
            {
                return pmru.DisplayName.Equals(mru.DisplayName, StringComparison.OrdinalIgnoreCase) && pmru.Server.Equals(mru.Server, StringComparison.OrdinalIgnoreCase);
            });
        }


        private void PerformInitialization()
        {
            MRUList mruList = MRUList.Inst;

            if (mruList.Count == 0)
            {
                try
                {
                    MRUList.XMLDeserialize(MRUListDefaultConfigPath + "\\" + MRUListSerializeFileName);
                }
                catch (Exception exception)
                {
                    DisplayError(exception.Message);
                }
            }

            MRUListToCombo(ServerComboBox);
            ServerComboBox.SelectedIndex = 0;

            // 11.0.3
            // if(AppSettingsManager.Read("LastServer") != null)
            // this.Server = AppSettingsManager.Read("LastServer");
        }
        private void MRUListToCombo(ComboBox combo)
        {
            MRUList mruList = MRUList.Inst;

            combo.Items.Clear();
            if (mruList != null)
            {
                foreach (MRUEntry mru in mruList)

                    combo.Items.Add(mru.DisplayName);
                //11.0.3
                //combo.Items.Add(mru.Server);

            }
            // add items for add and delete
            combo.Items.Add(Resources.LoginDialog_AddServer);
            if (mruList != null && mruList.Count > 0)
                combo.Items.Add(Resources.LoginDialog_RemoveServer);

            // 11.0.3 - Now getting the last user along with last server
            //if (AppSettingsManager.Read("LastServer") != null)
            //    this.Server = AppSettingsManager.Read("LastServer");
            if (AppSettingsManager.Read("LastServer") != null && AppSettingsManager.Read("LastUser") != null)
            {
                this.Server = string.Format("{0} [{1}]", AppSettingsManager.Read("LastServer"), AppSettingsManager.Read("LastUser"));
            }

        }

        private void MRUListToDisk(ComboBox combo)
        {
            if (combo.Items.Count <= 0)
                return;

            try
            {
                MRUList.XMLSerialize(MRUListSerializeFileName, MRUListDefaultConfigPath, MRUList.Inst);

                /*
                AppConfigSetting.WriteSetting(GetstringValue(Global.ConfigurationKey.MRULIST), string.Empty);
                AppConfigSetting.WriteSetting(GetstringValue(Global.ConfigurationKey.MRULIST), SerializeDeserialize.Serialize(M_MRUList));
                */

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool ChangeServerMode(MRUEntry mru, string existsServer, ref string changeTier)
        {
            if (string.IsNullOrEmpty(mru.Server) || string.IsNullOrEmpty(existsServer))
                return false;

            bool bIs2Tier1 = ServerNameUtils.StartsWith(mru.Server, StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_2T));
            bool bIs2Tier2 = ServerNameUtils.StartsWith(existsServer, StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_2T));

            if (bIs2Tier2)
                changeTier = StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_2T) + " to " + StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_3T);
            else
                changeTier = StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_3T) + " to " + StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_2T);

            return bIs2Tier1 != bIs2Tier2;
        }

        private bool ChangeSingleMode(MRUEntry mru, string existsServer, string tier)
        {
            if ((ServerNameUtils.StartsWith(existsServer, tier)) && (ServerNameUtils.StartsWith(mru.Server, tier)) && (!mru.Server.Equals(existsServer, StringComparison.OrdinalIgnoreCase)))
                return true;

            else
                return false;

        }
        private bool ChangingMode(MRUEntry mru1, MRUEntry mru2)
        {
            bool bIs2Tier1 = ServerNameUtils.StartsWith(mru1.Server, StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_2T));
            bool bIs2Tier2 = ServerNameUtils.StartsWith(mru2.Server, StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_2T));
            return bIs2Tier1 != bIs2Tier2;
        }

        internal void DisplayError(string error)
        {
            MessageBox.Show(error, Resources.LoginDialog_Error_Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        #region EventHandlers
        private void ButtonOK_Click(object sender, EventArgs e)
        {
            try
            {
                this.Focus();
                Cursor.Current = Cursors.WaitCursor;

                //11.0.3
                //Submit the information
                MRUEntry mru = this.CurrentServerMRU;

                // 11.0.3
                /*if ((mru != null) || (mru.Server.Equals(this.Server.Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    MRUList.Inst.RemoveAt(ServerComboBox.SelectedIndex);

                }*/
                if (mru != null)
                {
                    MRUEntry mruLocal = null;
                    foreach (MRUEntry item in MRUList.Inst)
                    {
                        if (item.Server.Equals(this.Server.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            if (item.UserName.Equals(this.UserName.Trim(), StringComparison.OrdinalIgnoreCase) || item.UserName.Trim() == "")
                            {
                                mruLocal = item;
                                break;
                            }
                        }
                    }

                    if (mruLocal != null)
                        MRUList.Inst.Remove(mruLocal);
                }


                mru = new MRUEntry(this.Server, this.UserName, this.Password, this.SavePasswd, this.SSL);
                MRUList.Inst.Insert(0, mru);
                MRUListToDisk(ServerComboBox);

                string changeTier = string.Empty;

                if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
                {
                    MessageBox.Show(Properties.Resources.msgUserPass, Resources.LoginDialog_Error_Caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                //TODO: Enable these validations.
                else if (ChangeSingleMode(this.CurrentServerMRU, this._lastUsedServer, StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_2T)))
                {
                    MessageBox.Show(Properties.Resources.msgChange2tierTo2tier, Resources.LoginDialog_Error_Caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                //11.0.3
                else if (ChangeServerMode(this.CurrentServerMRU, this._lastUsedServer, ref changeTier))
                {
                    MessageBox.Show(string.Format(Properties.Resources.msgChangeServerMode, changeTier), Resources.LoginDialog_Error_Caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                this.OnValidateUser();

                if (this._isLogged)
                {
                    AppSettingsManager.Write("LastServer", this.Server);
                    // 11.0.3 - Saving the last logged in Username
                    AppSettingsManager.Write("LastUser", this.UserName);
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    if (_maxRetries > 0 && ++_retries >= _maxRetries)
                    {
                        MessageBox.Show(Resources.LoginDialog_Error + ": Maximun number of retries exceeded", Resources.LoginDialog_Error_Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        DialogResult = DialogResult.Cancel;
                    }

                    /*//TODO: move this to access controller.
                    MessageBox.Show(Resources.LoginDialog_Error, Resources.LoginDialog_Error_Caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);*/
                }

            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void ServerComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // if user chooses <add...> or <delete...>, take appropriate action
            string sCurItem = ServerComboBox.Text;
            //Coverity Bug Fix :- CID : 11658  Jira Id :CBOE-194
            string sNewItem = (ServerComboBox.SelectedItem != null && ServerComboBox.SelectedItem is string) ? ServerComboBox.SelectedItem as string : string.Empty;

            if (sNewItem.Equals(Resources.LoginDialog_AddServer))
            {
                ServerDialog addServerDialog = new ServerDialog();
                if (addServerDialog.ShowDialog() == DialogResult.OK)
                {
                    //11.0.3
                    ServerInstanceDialog = true;

                    string input = addServerDialog.ServerName;

                    if (!string.IsNullOrEmpty(input))
                    {
                        string sNewVal = input;
                        string sMode = string.Empty;
                        if (addServerDialog.Tier2 == true)
                        {
                            sNewVal = string.Format(StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_2T) + " / {0}", input);
                            sMode = StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_2T);
                        }
                        else if (addServerDialog.Tier3 == true && addServerDialog.SSL == true)
                        {
                            sNewVal = string.Format(StringValueAttribute.GetstringValue(AccessControllerConstants.SSL) + " / {0}", input);
                            sMode = StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_3T);
                        }
                        else
                        {
                            sMode = StringValueAttribute.GetstringValue(AccessControllerConstants.MRU_3T);
                        }

                        try
                        {
                            MRUEntry mru = new MRUEntry(sNewVal, this.UserName, this.Password, this.SavePasswd, addServerDialog.SSL);
                            MRUList.Inst.Insert(0, mru);
                            MRUListToCombo(ServerComboBox);
                            MRUListToDisk(ServerComboBox);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                else if (addServerDialog.DialogResult == DialogResult.Cancel)
                {
                    //11.0.3
                    ServerInstanceDialog = false;
                    ServerComboBox.SelectedIndex = ServerIndex;
                    return;
                }

            }
            else if (sNewItem.Equals(Resources.LoginDialog_RemoveServer))
            {
                ServerInstanceDialog = null;
                string msg = string.Format(Resources.LoginDialog_DeleteMRU_Msg, sCurItem);
                if (MessageBox.Show(msg, Resources.LoginDialog_DeleteMRU_Caption, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        int comboIndex = ServerComboBox.FindStringExact(sCurItem);
                        int mruIndex = comboIndex;
                        Debug.Assert(mruIndex >= 0 && mruIndex < MRUList.Inst.Count);
                        MRUList.Inst.RemoveAt(mruIndex);
                        MRUListToCombo(ServerComboBox);
                        MRUListToDisk(ServerComboBox);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                //11.0.3
                ServerInstanceDialog = null;
                return;
            }
            ServerComboBox.SelectedIndex = 0;// do not leave <add> or <delete> chosen
        }
        #endregion

        private void ServerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ServerInstanceDialog == false) // if the serverdialog form open and cancel
                return;


            if (0 <= ServerComboBox.SelectedIndex && ServerComboBox.SelectedIndex < MRUList.Inst.Count)
            {
                MRUEntry mru = MRUList.Inst[ServerComboBox.SelectedIndex];
                UserName = mru.UserName;
                SavePasswd = mru.SavePasswd;
                //11.0.3
                if (ServerInstanceDialog == null) // ServerInstanceDialog=true, Password has persist in password textbox if user added server using serverdialong form. ServerInstanceDialog=null, is the case, to show the password in password text if checkbox was checked.
                    Password = SavePasswd ? mru.Password : "";

                ServerIndex = ServerComboBox.SelectedIndex;
            }
            else
            {
                UserName = string.Empty;
                Password = string.Empty;
            }
        }


    }

    public class LoginEventArgs : EventArgs
    {
        #region Variables
        string _userName;
        string _password;
        string _server;
        bool _ssl;
        bool _succeeded;
        #endregion

        #region Properties
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        public string Server
        {
            get
            {
                return _server;
            }
            set
            {
                _server = value;
            }
        }

        public bool SSL
        {
            get
            {
                return _ssl;
            }
            set
            {
                _ssl = value;
            }
        }

        public bool Succeeded
        {
            get
            {
                return _succeeded;
            }
            set
            {
                _succeeded = value;
            }
        }
        #endregion

        #region Constructors
        public LoginEventArgs(string username, string password)
        {
            _userName = username;
            _password = password;
        }
        public LoginEventArgs(string username, string password, string server, bool ssl)
        {
            _userName = username;
            _password = password;
            _server = server;
            _ssl = ssl;
        }
        #endregion
    }
}
