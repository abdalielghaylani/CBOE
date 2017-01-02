using System;
using System.Runtime.Serialization;
using System.Xml;
using FormDBLib;
using Spotfire.Dxp.Framework.Persistence;

namespace SpotfireIntegration.SpotfireAddin
{
    /// <summary>
    /// Contains the metadata about the connection with CBOE server for persisting
    /// </summary>
    [Serializable]
    [PersistenceVersion(1, 0)]
    public class CBOEConnectionPersistance : ISerializable
    {
        #region Variables
        string strConnInfo;

        [NonSerialized]
        COEConnInfo theConnInfo;
        #endregion

        #region Properties
        /// <summary>
        /// Gets connection info in string format
        /// </summary>
        public string StrConnInfo
        {
            get { return strConnInfo; }
        }

        /// <summary>
        /// Gets or sets the coe connection info object
        /// </summary>
        public COEConnInfo TheConnInfo
        {
            get
            {
                if (!string.IsNullOrEmpty(strConnInfo))
                {
                    theConnInfo.Deserialize(strConnInfo);
                }
                return theConnInfo;
            }
            set
            {
                theConnInfo = value;
                strConnInfo = theConnInfo.ToString();
            }
        }
        #endregion


        #region Constructor
        /// <summary>
        /// Initializes an instance of the CBOEConnectionPersistance object
        /// </summary>
        public CBOEConnectionPersistance()
        {
            theConnInfo = new COEConnInfo();
        }

        /// <summary>
        /// Initializes an instance of the CBOEConnectionPersistance object
        /// </summary>
        /// <param name="strConnInformation">connection information string</param>
        public CBOEConnectionPersistance(string strConnInformation)
            : this()
        {
            this.strConnInfo = strConnInformation;
        }

        /// <summary>
        /// Initializes an instance of the CBOEConnectionPersistance object
        /// </summary>
        /// <param name="info">SerializationInfo object</param>
        /// <param name="context">StreamingContext object</param>
        public CBOEConnectionPersistance(SerializationInfo info, StreamingContext context)
        {
            strConnInfo = info.GetString("cboeConnInfo");
        }

        #endregion

        /// <summary>
        /// Populates a System.Runtime.Serialization.SerializationInfo with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo to populate with data.</param>
        /// <param name="context">The destination (see System.Runtime.Serialization.StreamingContext) for this serialization.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("cboeConnInfo", strConnInfo);
        }

        /// <summary>
        /// Sets the connection information using the MRUEntry object
        /// </summary>
        /// <param name="mruEntry">MRU entry</param>
        public void SetConnInfo(MRUEntry mruEntry)
        {
            theConnInfo.SetConnInfo(mruEntry);
            strConnInfo = theConnInfo.ToString();
        }

        /// <summary>
        /// Checks for object equal condition
        /// </summary>
        /// <param name="obj">COEConnInfo object to compare</param>
        /// <returns>returns true if objects are same; otherwise false</returns>
        public override bool Equals(object obj)
        {
            return theConnInfo.Equals(obj);
        }

        /// <summary>
        /// Method to get the hash code of the COEConnInfo object
        /// </summary>
        /// <returns>returns hashcode</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode(); // theConnInfo.GetHashCode();
        }

        /// <summary>
        /// Generates the xml string representation of the search field collection
        /// </summary>
        /// <returns>returns the xml string of the search fields collection</returns>
        public override string ToString()
        {
            return strConnInfo;
        }
    }

    /// <summary>
    /// Class to maintain MRUEntry details for persisting as document property
    /// </summary>
    public class COEConnInfo
    {
        #region Variables
        string serverName;
        string serverDisplayName;
        bool isThreeTier = true;
        MRUEntry _currentMRUEntry;
        bool savePassword = false;
        const String MODE_2T = "2-Tier";
        string userName; 
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the current MRUEntry
        /// </summary>
        public MRUEntry CurrentMRUEntry
        {
            get { return _currentMRUEntry; }
            set
            {
                _currentMRUEntry = value;
                this.serverName = _currentMRUEntry.Server;
                this.serverDisplayName = _currentMRUEntry.DisplayName;
                this.isThreeTier = this.serverName.StartsWith(MODE_2T, true, System.Globalization.CultureInfo.CurrentCulture) ? false : true;
                this.savePassword = _currentMRUEntry.SavePasswd;
                this.userName = _currentMRUEntry.UserName;
            }
        }

        /// <summary>
        /// Gets the user name
        /// </summary>
        public string UserName
        {
            get { return userName; }
        }

        /// <summary>
        /// Gets the server display name
        /// </summary>
        public string ServerDisplayName
        {
            get { return serverDisplayName; }
        }

        /// <summary>
        /// Gets whether to save the passowrd
        /// </summary>
        public bool SavePassword
        {
            get { return savePassword; }
        }

        /// <summary>
        /// Gets or sets the serve name
        /// </summary>
        public string ServerName
        {
            get { return serverName; }
            private set
            {
                serverName = value;
                this.isThreeTier = this.serverName.StartsWith(MODE_2T, true, System.Globalization.CultureInfo.CurrentCulture) ? false : true;
            }
        }

        /// <summary>
        /// Gets whether connection is three tier
        /// </summary>
        public bool IsThreeTier
        {
            get { return isThreeTier; }
        } 
        #endregion

        /// <summary>
        /// Initializes an instance of the COEConnInfo class
        /// </summary>
        public COEConnInfo()
        {
            this.serverName = string.Empty;
            this.isThreeTier = true;
        }

        /// <summary>
        /// Sets the connection information using the MRUEntry
        /// </summary>
        /// <param name="currentMRUEntry">MRUEntry object</param>
        public void SetConnInfo(MRUEntry currentMRUEntry)
        {
            CurrentMRUEntry = currentMRUEntry;
        }

        /// <summary>
        /// Serialization method for converting connection information in xml format
        /// </summary>
        /// <returns>returns connection information object in xml string format</returns>
        public override string ToString()
        {
            XmlDocument doc = new XmlDocument();
            XmlNode parentNode = doc.CreateNode(XmlNodeType.Element, "CBOEConnInfo", "");
            XmlAttribute serverNameAttr = doc.CreateAttribute("serverName");
            serverNameAttr.Value = serverName;
            XmlAttribute isThreeTierAttr = doc.CreateAttribute("isThreeTier");
            isThreeTierAttr.Value = isThreeTier.ToString();
            XmlAttribute serverDisplayNameAttr = doc.CreateAttribute("serverDisplayName");
            serverDisplayNameAttr.Value = serverDisplayName;
            XmlAttribute savePasswordAttr = doc.CreateAttribute("savePassword");
            savePasswordAttr.Value = savePassword.ToString();
            XmlAttribute userNameAttr = doc.CreateAttribute("userName");
            userNameAttr.Value = userName;

            parentNode.Attributes.Append(serverNameAttr);
            parentNode.Attributes.Append(isThreeTierAttr);
            parentNode.Attributes.Append(serverDisplayNameAttr);
            parentNode.Attributes.Append(savePasswordAttr);
            parentNode.Attributes.Append(userNameAttr);
            doc.AppendChild(parentNode);
            return doc.InnerXml;
        }

        /// <summary>
        /// Method to de-serialize the connection info xml string in object 
        /// </summary>
        /// <param name="strConnInfo">input xml string</param>
        public void Deserialize(string strConnInfo)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(strConnInfo);
            if (doc.HasChildNodes)
            {
                XmlNode parentNode = doc.SelectSingleNode("CBOEConnInfo");
                if (parentNode != null)
                {
                    string serverName = parentNode.Attributes["serverName"].Value;
                    string userName = parentNode.Attributes["userName"].Value;
                    string serverDisplayName = parentNode.Attributes["serverDisplayName"].Value;
                    bool savePwd = Convert.ToBoolean(parentNode.Attributes["savePassword"].Value);
                    MRUEntry mruEntry = new MRUEntry(serverName, userName, string.Empty, savePwd);
                    this.CurrentMRUEntry = mruEntry;
                }
            }
        }

        /// <summary>
        /// Checks for equal of the two objects
        /// </summary>
        /// <param name="obj">object to compare</param>
        /// <returns>returns true if object is equal; otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            COEConnInfo otherConnInfo = obj as COEConnInfo;
            if (otherConnInfo == null)
            {
                return false;
            }

            if (otherConnInfo.serverName.Equals(this.serverName, StringComparison.OrdinalIgnoreCase) && otherConnInfo.isThreeTier == this.isThreeTier)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Generate and returns the hashcode
        /// </summary>
        /// <returns>returns the hashcode</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode(); // this.serverName.GetHashCode();
        }
    }
}
