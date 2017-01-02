using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Configuration;

using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CBVUtilities;

namespace FormDBLib
{
    #region Login class
    public partial class Login : Form
    {
        #region Variables
        private MRUList m_MRUList;
        #endregion

        #region Properties
        public string UserName { get { return userName.Text.ToUpper(); } set { userName.Text = value; } }
        public string Password { get { return password.Text; } set { password.Text = value; } }

        //public string Server { get { return mruCombo.Text; } }
        public string Server { get { return CBVUtil.BeforeDelimiter(ServerDisplay, '[').Trim(); } }
        public string ServerDisplay { get { return mruCombo.Text; } }

        public bool SavePasswd { get { return savePasswd.Checked; } set { savePasswd.Checked = value; } }
        public bool Is2Tier { get { return CBVUtil.StartsWith(Server, CBVConstants.MODE_2T); } }
        public bool IsSSL { get { return Server.Contains("(SSL)"); } }
        public MRUList MRUList { get { return m_MRUList; } set { m_MRUList = value; } }
        public MRUEntry CurrentMRU
        {
            get
            {
                // create an MRUEntry from data currently in dialog, whether on drop-down or not
                return new MRUEntry(Server, UserName, Password, SavePasswd);
            }
        }
        #endregion

        #region Constructors
        public Login(MRUList mruList)
        {
            InitializeComponent();

            // mru list comes from settings
            m_MRUList = mruList;
            if (mruList == null)
                m_MRUList = new MRUList();

            MruListToCombo(mruCombo);
            mruCombo.SelectedIndexChanged += new EventHandler(mruCombo_SelectedIndexChanged);
            mruCombo.SelectionChangeCommitted += new EventHandler(mruCombo_SelectionChangeCommitted);
            mruCombo.SelectedIndex = 0;
            mruCombo_SelectedIndexChanged(this, null);    // force call; above stmt doesn't actually change the value

            this.AcceptButton = this.OKUltraButton;     // CSBR-109998
        }
        #endregion

        #region Methods
        public void AddOrSelectMRU(MRUEntry mru)
        {
            // insert mru if new; otherwise move to top of list
            int index = m_MRUList.FindMRU(mru);
            if (index != -1)
                m_MRUList.RemoveAt(index);
            m_MRUList.Insert(0, mru);
        }

        /// <summary>
        /// Selects the mru entry from current list and display as selected in drop down box
        /// </summary>
        /// <param name="mru">mru entry to be selected in drop down box</param>
        public void SelectMRUFromCombo(MRUEntry mru)
        {
            AddOrSelectMRU(mru);
            MruListToCombo(mruCombo);
            int mruIndex = mruCombo.Items.IndexOf(mru.DisplayName);
            if (mruIndex >= 0)
            {
                mruCombo.SelectedIndex = mruIndex;
            }
        }

        /// <summary>
        /// Set enable state of the server name drop down box based on the specified state
        /// </summary>
        /// <param name="state">state value to determine combobox enable state</param>
        public void SetServerComboState(bool state)
        {
            mruCombo.Enabled = state;
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///  Adds the <delete selected entry ...> tags to the combo-->
        /// </summary> 
        /// <param name="combo"></param>
        private void MruListToCombo(ComboBox combo)
        {
            combo.Items.Clear();
            if (m_MRUList != null)
            {
                foreach (MRUEntry mru in m_MRUList)
                    combo.Items.Add(mru.DisplayName);
            }
            // add items for add and delete
            combo.Items.Add(CBVConstants.MRU_NEWSERVER);
            if (m_MRUList != null && m_MRUList.Count > 0)
                combo.Items.Add(CBVConstants.MRU_DELETE);
        }
        //---------------------------------------------------------------------
        public static String ParseServerMachineName(String server)
        {
            // return name from string like "2-Tier / name [user]"
            String s = server, q1 = " (SSL)", q2 = "2-Tier / ";
            if (CBVUtil.EndsWith(s, q1))
                s = server.Substring(0, s.IndexOf(q1));
            if (CBVUtil.StartsWith(s, q2))
                s = s.Substring(q2.Length);
            if (s.Contains("["))
                s = s.Substring(0, s.IndexOf("["));
            return s.Trim();
        }
        //---------------------------------------------------------------------
        #endregion

        #region Events
        private void mruCombo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // if user chooses <add...> or <delete...>, take appropriate action
            String sCurItem = mruCombo.Text;
            String sNewItem = mruCombo.SelectedItem as String;
            //Coverity Bug Fix CID 13032 
            if (!string.IsNullOrEmpty(sNewItem) && sNewItem.Equals(CBVConstants.MRU_NEWSERVER))
            {
                String initialValue1 = "<middle-tier server address>";
                String initialValue2 = "<Oracle service name>";

                int radioVal = 0;
                bool bUseSSL = false;
                String sInput = CBVUtil.PromptForStringAndVal(CBVConstants.MRU_3TPROMPT, initialValue1,
                                        "3-Tier", "2-Tier", ref radioVal, CBVConstants.MRU_2TPROMPT, initialValue2,
                                        "Use SSL", ref bUseSSL);
                if (!String.IsNullOrEmpty(sInput))
                {
                    String sNewVal = sInput;
                    if (bUseSSL && !sNewVal.EndsWith("(SSL)"))
                        sNewVal = String.Format("{0} (SSL)", sInput);
                    if (radioVal == 1)
                        sNewVal = String.Format("2-Tier / {0}", sNewVal);
                    MRUEntry mru = new MRUEntry(sNewVal, "", "", false);
                    m_MRUList.Insert(0, mru);
                    MruListToCombo(mruCombo);
                }
            }
            else if (!string.IsNullOrEmpty(sNewItem) && sNewItem.Equals(CBVConstants.MRU_DELETE))
            {
                String msg = String.Concat("Delete MRU entry '", sCurItem, "'?"); // CSBR-111990
                if (MessageBox.Show(msg, "Delete MRU Entry", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int comboIndex = mruCombo.FindStringExact(sCurItem);
                    int mruIndex = comboIndex;
                    Debug.Assert(mruIndex >= 0 && mruIndex < m_MRUList.Count);

                    m_MRUList.RemoveAt(mruIndex);
                    MruListToCombo(mruCombo);
                }
            }
            else
            {
                return;
            }
            mruCombo.SelectedIndex = 0;   // do not leave <add> or <delete> chosen
        }
        //---------------------------------------------------------------------
        private void mruCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if user selects mru entry, display its data
            if (mruCombo.SelectedIndex >= 0 && m_MRUList != null && mruCombo.SelectedIndex < m_MRUList.Count)
            {
                MRUEntry mru = m_MRUList[mruCombo.SelectedIndex];
                UserName = mru.UserName;
                SavePasswd = mru.SavePasswd;
                Password = SavePasswd ? mru.DecrPasswd : "";
            }
        }
        //---------------------------------------------------------------------
        private void cancelUltraButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        //---------------------------------------------------------------------
        private void OKUltraButton_Click(object sender, EventArgs e)
        {
            // can't proceed without username and password

            if (String.IsNullOrEmpty(UserName) || String.IsNullOrEmpty(Password))
            {
                MessageBox.Show(CBVConstants.LOGIN_NAME_REQD, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
        }
        //---------------------------------------------------------------------
        #endregion
    }
    #endregion

    #region MRUEntry struct
    public struct MRUEntry
    {
        #region Variables
        private String m_server, m_username;
        private String m_encrPasswd;
        private bool m_savePasswd;
        #endregion

        #region Constructors
        //---------------------------------------------------------------------
        public MRUEntry(String server, String username, String password, bool bSavePasswd)
        {
            m_server = server;
            m_username = username;
            m_savePasswd = bSavePasswd;

            // supplied arg is plain text; encrypt into member
            Cryptography crypt = new Cryptography();
            m_encrPasswd = crypt.Encrypt(password, CBVConstants.CRYPTOGRAPHY_KEY);
        }
        //---------------------------------------------------------------------
        #endregion

        #region Properties
        [XmlIgnore]
        public String DecrPasswd
        {
            // return plain-text password from encrypted member
            get
            {
                Cryptography crypt = new Cryptography();
                String decrPasswd = crypt.Decrypt(m_encrPasswd, CBVConstants.CRYPTOGRAPHY_KEY);
                return decrPasswd;
            }
            set
            {
                Cryptography crypt = new Cryptography();
                m_encrPasswd = crypt.Encrypt(value, CBVConstants.CRYPTOGRAPHY_KEY);
            }
        }
        //---------------------------------------------------------------------
        public String EncrPasswd
        {
            get { return m_encrPasswd; }
            set { m_encrPasswd = value; }
        }
        //---------------------------------------------------------------------
        public String UserName
        {
            get { return m_username; }
            set { m_username = value; }
        }
        //---------------------------------------------------------------------
        public String Server
        {
            get { return m_server; }
            set { m_server = value; }
        }
        //---------------------------------------------------------------------
        public bool SavePasswd
        {
            get { return m_savePasswd; }
            set { m_savePasswd = value; }
        }
        //---------------------------------------------------------------------
        public String DisplayName
        {
            get { return String.IsNullOrEmpty(m_username) ? m_server : String.Format("{0} [{1}]", m_server, m_username); }
        }
        //---------------------------------------------------------------------
        public String URL
        {
            get { return MakeURL(m_server); }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Methods
        public static String MakeURL(String server)
        {
            // Pick between SOAP or Remoting portal based on app config setting
            string file = @"WebServicePortal.asmx";
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseRemoting"]))
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["CompressRemotingData"]))
                {
                    file = @"RemotingPortalCompressed.rem";
                }
                else
                {
                    file = @"RemotingPortal.rem";
                }
            }

            if (CBVUtil.EndsWith(server, ".asmx") || CBVUtil.EndsWith(server, ".rem"))          // server string has the whole url
                return server;
            else if (CBVUtil.StartsWith(server, "http"))    // as in "https://servername"
                return String.Format("{0}/COEWebServiceHost/{1}", server, file);
            else if (server.Contains("(SSL)"))
                return String.Format("https://{0}/COEWebServiceHost/{1}", CBVUtil.BeforeDelimiter(server, '(').Trim(), file);
            else
                return String.Format("http://{0}/COEWebServiceHost/{1}", server, file);
        }

        //---------------------------------------------------------------------
        public bool Equals(MRUEntry entry, bool bIgnoreCase)
        {
            if (bIgnoreCase)
                return (CBVUtil.Eqstrs(m_server, entry.m_server) &&
                        CBVUtil.Eqstrs(m_username, entry.m_username));
            else
                return m_server.Equals(entry.m_server) &&
                        m_username.Equals(entry.m_username);
        }
        //---------------------------------------------------------------------

        #endregion
    }
    #endregion

    #region MRUList class
    public class MRUList : List<MRUEntry>
    {
        #region Constructors
        public MRUList()
        {
        }
        #endregion

        #region Methods
        public int FindMRU(MRUEntry entry)
        {
            bool bIgnoreCase = false;
            for (int i = 0; i < this.Count; ++i)
                if (this[i].Equals(entry, bIgnoreCase))
                    return i;
            return -1;
        }
        //---------------------------------------------------------------------
        public void Trim()
        {
            // remove any artifact entries not having username
            for (int i = 0; i < this.Count; ++i)
            {
                if (String.IsNullOrEmpty(this[i].UserName))
                {
                    this.RemoveAt(i);
                    --i;
                }
            }
        }
        #endregion
    }
    #endregion
}