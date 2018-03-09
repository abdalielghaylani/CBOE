using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ChemBioVizExcelAddIn
{
    #region MRUEntry struct
    [Serializable()]
    public struct MRUEntry
    {
        #region Variables
        private String m_server, m_username;
        private String m_encrPasswd;
        private bool m_savePasswd;
        private bool m_ssl;
        #endregion

        #region Constructors
       
        public MRUEntry(String server, String username, String password, bool bSavePasswd, bool ssl)
        {
            m_server = server;
            m_username = username;
            m_savePasswd = bSavePasswd;
            m_encrPasswd = password;
            m_ssl = ssl;
            // supplied arg is plain text; encrypt into member
            // Cryptography crypt = new Cryptography();
            // m_encrPasswd = crypt.Encrypt(password, CBVConstants.CRYPTOGRAPHY_KEY);
        }
       
        #endregion

        #region Properties    
       
        public bool SSL
        {
            get { return m_ssl; }
            set { m_ssl = value; }
        }
        public String EncrPasswd
        {
            get { return m_encrPasswd; }
            set { m_encrPasswd = value; }
        }      
        public String UserName
        {
            get { return m_username; }
            set { m_username = value; }
        }
      
        public String Server
        {
            get { return m_server; }
            set { m_server = value; }
        }
      
        public bool SavePasswd
        {
            get { return m_savePasswd; }
            set { m_savePasswd = value; }
        }
      
        public String DisplayName
        {
            get { return String.IsNullOrEmpty(m_username) ? m_server : String.Format("{0} [{1}]", m_server, m_username); }
        }
       
        public static String MakeURL(String server)
        {
            return String.Format("http://{0}/COEWebServiceHost/WebServicePortal.asmx", server);
        }
        public static String MakeSURL(String server)
        {
            return String.Format("https://{0}/COEWebServiceHost/WebServicePortal.asmx", CBVUtil.AfterDelimiter(server, '/').Trim());
        }
       
        public static String MakeSURL(String serverName, int parms)
        {
            try
            {
                return String.Format("{0}://{1}/COEWebServiceHost/WebServicePortal.asmx", CBVUtil.BeforeDelimiter(serverName, '/').Trim(), CBVUtil.AfterDelimiter(serverName, '/').Trim());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      
        public String URL
        {
            get { return MakeURL(m_server); }
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
                if (String.IsNullOrEmpty(this[i].UserName))
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
                mruList = (MRUList)xmlDeSeralizer.Deserialize(s);
                s.Close();
                return mruList;
            }
            return mruList;
        }
        public static void XMLSerialize(string file, string dirPath, MRUList mruList)
        { 
            if (!Directory.Exists(@dirPath))
                Directory.CreateDirectory(@dirPath);

            XmlSerializer xmlSeralizer = new XmlSerializer(typeof(MRUList));
            Stream s = File.Open(@dirPath + file, FileMode.Create);
            xmlSeralizer.Serialize(s, mruList);
            s.Close();
        }
       
        #endregion "Static Methods"

        #endregion
    }
    #endregion

    public class CBVUtil
    {
      
        public static bool StartsWith(String s1, String s2)
        {
            return s1.StartsWith(s2, true, System.Globalization.CultureInfo.CurrentCulture);
        }
      
        public static bool EndsWith(String s1, String s2)
        {
            return s1.EndsWith(s2, true, System.Globalization.CultureInfo.CurrentCulture);
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
       
        public static string BeforeFirstDelimiter(string stringToSplit, char delimiter)
        {
            if (stringToSplit == null) return "";
            int delimiterPos = stringToSplit.IndexOf(delimiter);
            if (delimiterPos == -1)
                return stringToSplit;
            return stringToSplit.Substring(0, delimiterPos);
        }
     
        public static string AfterFirstDelimiter(string stringToSplit, char delimiter)
        {
            if (stringToSplit == null) return "";
            int delimiterPos = stringToSplit.IndexOf(delimiter);
            if (delimiterPos == -1)
                return "";      // there is no delimiter, so can't be anything after it
            return stringToSplit.Substring(delimiterPos + 1);
        }  

        public static bool StringToBoolean(string value)
        {
            try
            {
                Convert.ToBoolean(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
      
    }
}
