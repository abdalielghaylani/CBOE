using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.ServerControls.Login
{
    #region MRUEntry struct
    [Serializable()]
    public class MRUEntry
    {
        #region Variables
        private const string key = "farielloletemealaincertidumbre";
        private string m_server, m_username;
        private string m_encrPasswd;
        private bool m_savePasswd;
        private bool m_ssl;
        #endregion

        #region Constructors

        public MRUEntry(string server, string username, string password, bool bSavePasswd, bool ssl)
        {
            m_server = server;
            m_username = username;
            m_savePasswd = bSavePasswd;
            Password = password;
            m_ssl = ssl;
            // supplied arg is plain text; encrypt into member
            // Cryptography crypt = new Cryptography();
            // m_encrPasswd = crypt.Encrypt(password, CBVConstants.CRYPTOGRAPHY_KEY);
        }

        public MRUEntry()
        { 

        }
        #endregion

        #region Properties

        public bool SSL
        {
            get { return m_ssl; }
            set { m_ssl = value; }
        }
        [XmlIgnore]
        public string Password
        {
            get 
            {

                return CambridgeSoft.COE.Framework.Common.Utilities.DecryptRijndael(m_encrPasswd, key); 
            }
            set 
            {
                m_encrPasswd = CambridgeSoft.COE.Framework.Common.Utilities.EncryptRijndael(value, key); 
            }
        }

        public string EncrPasswd
        {
            get
            {
                return m_encrPasswd;
            }
            set
            {
                m_encrPasswd = value;
            }
        }

        public string UserName
        {
            get { return m_username; }
            set { m_username = value; }
        }

        public string Server
        {
            get { return m_server; }
            set { m_server = value; }
        }

        public bool SavePasswd
        {
            get { return m_savePasswd; }
            set { m_savePasswd = value; }
        }

        public string DisplayName
        {
            get { return string.IsNullOrEmpty(m_username) ? m_server : string.Format("{0} [{1}]", m_server, m_username); }
        }

        public static string MakeURL(string server)
        {
            return MakeURL(server, false);
        }
        public static string MakeSURL(string server)
        {
            return MakeURL(server, true);
        }

        public static string MakeURL(String server, Boolean isSSL)
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

            String protocol = "http";
            if ((isSSL) || server.StartsWith("SSL /"))
            {
                protocol = "https";
                server = ServerNameUtils.AfterDelimiter(server, '/').Trim();
            }

            if (server.EndsWith(".asmx") || server.EndsWith(".rem")) // server string has the whole url
                return server;
            else if (server.StartsWith("http"))    // as in "https://servername"
                return String.Format("{0}/COEWebServiceHost/{1}", server, file);

            
            return String.Format("{0}://{1}/COEWebServiceHost/{2}", protocol, server, file);
        }


        public static string MakeSURL(string serverName, int parms)
        {
            try
            {
                return MakeURL(serverName, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string URL
        {
            get { return MakeURL(m_server, false); }
        }

        #endregion

    }
    #endregion

    #region MRUList class
    [Serializable()]
    public class MRUList : List<MRUEntry>
    {

        private static MRUList mruList = null;
        private static MRUList inst = null;
        static readonly object padlock = new object();

        public static MRUList Inst
        {
            get
            {
                lock (padlock)
                {
                    if (inst == null)
                    {
                        inst = new MRUList();
                    }
                    return inst;
                }
            }
        }


        #region Constructors
        public MRUList()
        {
        }
        #endregion



        #region Methods

        public void Trim()
        {
            // remove any artifact entries not having username
            for (int i = 0; i < this.Count; ++i)
            {
                if (string.IsNullOrEmpty(this[i].UserName))
                {
                    this.RemoveAt(i);
                    --i;
                }
            }
        }

        #region "Static Methods"

        public static MRUList XMLDeserialize(string file)
        {
            mruList = new MRUList();
            if (File.Exists(file))
            {
                Stream s = File.Open(file, FileMode.Open);
                XmlSerializer xmlDeSeralizer = new XmlSerializer(typeof(MRUList));
                inst = (MRUList)xmlDeSeralizer.Deserialize(s);
                s.Close();
                return mruList;
            }
            return mruList;
        }
        public static void XMLSerialize(string file, string dirPath, MRUList mruList)
        {
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            if (!dirPath.Trim().EndsWith("\\"))
                dirPath = dirPath.Trim() + "\\";

            XmlSerializer xmlSeralizer = new XmlSerializer(typeof(MRUList));
            Stream s = File.Open(dirPath + file, FileMode.Create);
            xmlSeralizer.Serialize(s, mruList);
            s.Close();
        }

        #endregion "Static Methods"

        #endregion
    }
    #endregion

    public class ServerNameUtils
    {
        public static bool StartsWith(string s1, string s2)
        {
            return s1.StartsWith(s2, true, System.Globalization.CultureInfo.CurrentCulture);
        }

        public static string BeforeDelimiter(string stringToSplit, char delimiter)
        {
            if (stringToSplit == null) return "";
            int delimiterPos = stringToSplit.LastIndexOf(delimiter);
            if (delimiterPos == -1)
                return stringToSplit;
            return stringToSplit.Substring(0, delimiterPos);
        }

        public static string AfterDelimiter(string stringToSplit, char delimiter)
        {
            if (stringToSplit == null) return "";
            int delimiterPos = stringToSplit.LastIndexOf(delimiter);
            if (delimiterPos == -1)
                return "";      // there is no delimiter, so can't be anything after it
            return stringToSplit.Substring(delimiterPos + 1);
        }
    }
}
