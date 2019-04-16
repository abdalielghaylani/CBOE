using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Security.Services;
using System.Xml;
using CambridgeSoft.COE.Security.Services.Utilities;
using System.DirectoryServices;
using CambridgeSoft.COE.Security.Services.Utlities;
using System.IO;
using System.Security.Cryptography;
using CambridgeSoft.COE.Security.Services.Utilities.ELNUtils;

namespace CambridgeSoft.COE.Security.Services
{
    public class COELDAP : ICOESSO
    {
        #region ICOESSO Members


        private static string coeLDAPxml = string.Empty;
        private static XmlElement xn = null;


        DirectoryEntry mRoot = null;
        SearchResult filterResult = null;
        AuthenticationTypes authenticationType = AuthenticationTypes.Secure;

        //create an empty customizers and ENSConnection 
        //to allow code resuse from ELN
        private Customizer[] customizers = null;
        private ENSConnection connection = null;

        private ENSConnection coeConnection = null;

        public COELDAP()
        {
            //Coverity Fix :11796
            COELDAPConfiguration theConfiguration = COELDAPConfiguration.GetConfig();
            if (theConfiguration != null)
                coeLDAPxml = theConfiguration.Data;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(coeLDAPxml);

            xn = doc.DocumentElement;
            coeConnection = GetLogConnection();
        }

        public bool checkUserExists(string userName)
        {

            bool test = false;
            try
            {
                test = Authenticate(xn, userName, null, false, "addUser", coeConnection);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().Name == "InvalidOperationException"))
                {
                    throw new Exception(ex.Message + ": " + ex.StackTrace);
                }
                else
                {
                    test = false;
                }
            }
            return test;

        }

        public bool ValidateUser(string userName, string password)
        {
            bool test = false;
            try
            {
                //test = Authenticate(xn, userName, password, false, "authenticate", null);
                test = Authenticate(xn, userName, password, false, "authenticate", coeConnection);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType().Name == "InvalidOperationException"))
                {
                    throw new Exception(ex.Message + ": " + ex.StackTrace);
                }
                else
                {
                    test = false;
                }
            }
            return test;
            //throw new Exception("The method or operation is not implemented.");
        }


        public System.Xml.XmlDocument GetUserInfo(string userName)
        {
            bool test = false;
            XmlDocument xmldoc = new XmlDocument();

            XmlElement nodeElem = xmldoc.CreateElement("results");
            xmldoc.AppendChild(nodeElem);

            XmlElement root = xmldoc.DocumentElement;

            test = Authenticate(xn, userName, null, false, "getUserInfo", coeConnection);

            if (null == filterResult)
            {
                throw new Exception("Error: User could not be found");
                //return "Error";
            }


            XmlNode getuserInfoNode = xn.SelectSingleNode("GetUserReturnInfo");


            foreach (XmlNode ri in getuserInfoNode.ChildNodes)
            {
                string nodeName = "";
                //if (!(ri.nodeName == ""))
                if ((ri.Attributes["nodeName"] != null) && (!(ri.Attributes["nodeName"].Value == "")))
                {
                    nodeName = ri.Attributes["nodeName"].Value;
                    //nodeName = ri.nodeName;
                }
                else
                {
                    nodeName = ri.Attributes["ldapCode"].Value;
                    //nodeName = ri.ldapCode;
                }
                XmlElement elem = xmldoc.CreateElement(nodeName);

                //elem.SetAttribute("mapTo", ri.mapTo);
                //elem.SetAttribute("displayName", ri.displayName);
                elem.SetAttribute("mapTo", ri.Attributes["mapTo"].Value);
                elem.SetAttribute("displayName", ri.Attributes["displayName"].Value);

                try
                {
                    elem.InnerText = filterResult.Properties[ri.Attributes["ldapCode"].Value][0].ToString();
                }
                catch
                {
                    elem.InnerText = "";
                }
                root.AppendChild(elem);

            }



            // throw new Exception("The method or operation is not implemented.");
            return xmldoc;
        }

        public string GetCSSecurityPassword(string userName, string password)
        {
            return ReverseName(userName);
        }


        private ENSConnection GetLogConnection()
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["LogSSO"].ToString()))
            {
                ENSConnection en = new ENSConnection();
                return en;
            }
            else
            {
                return null;
            }
        }


        private string ReverseName(string userName)
        {
            return "7" + ReverseString(userName).ToUpper() + "11C";
        }
        private string ReverseString(string x)
        {
            char[] charArray = new char[x.Length];
            int len = x.Length - 1;
            for (int i = 0; i <= len; i++)
                charArray[i] = x[len - i];
            return new string(charArray);
        }

        #endregion

        /// <summary>
        /// Authenticates a user against an LDAP-based directory
        /// </summary>
        /// <param name="configElement">The contents of the configuration file associated with the business tier of this E-Notebook system.</param>
        /// <param name="userName">Username of user to authenticate</param>
        /// <param name="password">Password of user to authenticate</param>
        /// <param name="admin">Indicates if the user is an administrator or not</param>
        /// <param name="operation">Indicates the operation being performed: authentication (authenticate) or creation of a new user (addUser)</param>
        /// <param name="conn">The object used to communicate between the client tier, the business tier and the database tier.</param>
        /// <returns>True if it authenticated successfully, false otherwise.</returns>
        public bool Authenticate(System.Xml.XmlElement configElement, string userName, string password, bool admin, string operation, ENSConnection conn)
        {
            bool bound = false;
            int count = 0;
            string error = "";
            const string ROUTINE_NAME = "Authenticate";

            //Set the connection
            connection = conn;
            Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "Username = " + userName);

            if (password == null || password.Length == 0)
            {
                Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "No password provided");
            }
            else
            {
                Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "Password was provided");
            }

            //Do the authentication.
            //Get the validate process definition section of the config file.
            #region Multiple LDAP Servers
            XmlNode operationNode = configElement.SelectSingleNode(operation);
            foreach (XmlNode processNode in configElement.SelectNodes(operation + "/process"))
            {
                try
                {
                    if (processNode != null && count < 1)
                    {
                        bound = true;
                        #region processNode !=  null
                        //Get all associated customizers
                        customizers = Customizer.GetCustomizers((XmlElement)configElement.SelectSingleNode(operation), connection);

                        //If a query result is positive, stop looking for the user and check the user's group membership. 
                        bool passwordRequired = false;
                        foreach (XmlNode node in processNode.ChildNodes)
                        {
                            if (node.Name.ToLower() == "bind" && bound)
                            {
                                #region Node Bind
                                //Try to bind.
                                if (node.Attributes["pwdRequired"] != null)
                                {
                                    if (!node.Attributes["pwdRequired"].Value.ToLower().Equals("true")
                                        && !node.Attributes["pwdRequired"].Value.ToLower().Equals("false"))
                                    {
                                        error = "Invalid value in the attribute \"pwdRequired\" of the <bind> node, allowed values are \"true\" and \"false\"";
                                        Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, error);
                                        throw new InvalidOperationException(error);
                                    }
                                    passwordRequired = false;
                                    if (node.Attributes["pwdRequired"].Value.ToLower().Equals("true"))
                                    {
                                        passwordRequired = true;
                                        if (password == null && node.SelectSingleNode("dn") == null)
                                        {
                                            // If the user node is missing, a service account will be used.
                                            // Set the password to anything but null.
                                            // Refer to Bind()
                                            password = "";
                                        }
                                        else if (password == null || password.Length == 0 && !(operation.Equals("addUser")))
                                        {
                                            error = "No password provided. A password is REQUIRED for this operation.";
                                            Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, error);
                                            throw new InvalidOperationException(error);
                                        }
                                    }
                                    else
                                    {
                                        passwordRequired = false;
                                    }
                                }
                                else
                                {
                                    error = "The attribute pwdRequired was not found! Please enter this attribute in the bind node, as it is a mandatory attribute.";
                                    Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, error);
                                    throw new InvalidOperationException(error);
                                }

                                if (node.Attributes["authenticationType"] != null)
                                {
                                    string authType = node.Attributes["authenticationType"].Value;
                                    switch (authType.ToLower())
                                    {
                                        case "secure":
                                            authenticationType = AuthenticationTypes.Secure;
                                            break;
                                        case "fastbind":
                                            authenticationType = AuthenticationTypes.FastBind;
                                            break;
                                        case "anonymous":
                                            authenticationType = AuthenticationTypes.Anonymous;
                                            break;
                                        case "serverbind":
                                            authenticationType = AuthenticationTypes.ServerBind;
                                            break;
                                        case "none":
                                            authenticationType = AuthenticationTypes.None;
                                            break;
                                        case "ssl":
                                            authenticationType = AuthenticationTypes.SecureSocketsLayer;
                                            break;
                                        case "delegation":
                                            authenticationType = AuthenticationTypes.Delegation;
                                            break;
                                        case "encryption":
                                            authenticationType = AuthenticationTypes.Encryption;
                                            break;
                                        case "readonlyserver":
                                            authenticationType = AuthenticationTypes.ReadonlyServer;
                                            break;
                                        case "sealing":
                                            authenticationType = AuthenticationTypes.Sealing;
                                            break;
                                        case "signing":
                                            authenticationType = AuthenticationTypes.Signing;
                                            break;
                                        default:
                                            error = "Invalid Authentication Type in the attribute \"authenticationtype\" of the <bind> node";
                                            Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, error);
                                            throw new InvalidOperationException(error);
                                    }
                                }

                                //Initialize all the existent customizers
                                if (customizers != null)
                                {
                                    for (int j = 0; j < customizers.Length; j++)
                                    {
                                        customizers[j].Initialize(configElement);
                                    }
                                }

                                if (passwordRequired)
                                {
                                    Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "Calling Bind with username and password");
                                    bound = Bind(node, userName, password, operation);
                                    if (bound) count = 1;
                                }
                                else
                                {
                                    Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "Calling Bind without username and password");
                                    bound = Bind(node, userName, null, operation);
                                    if (bound) count = 1;
                                }
                                #endregion
                            }
                            else if (node.Name.ToLower() == "query" && bound)
                            {
                                bound = Query(node, userName, password, operation);
                            }
                        }
                        #endregion
                    }
                }
                catch (InvalidOperationException ex)
                {
                    if (processNode.NextSibling == null) throw;
                    count = 0;
                    Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, ex.Message);
                }
            }
            #endregion
            return bound;
        }

        /// <summary>
        /// Binds to specified LDAP directory spesifying the password
        /// </summary>
        /// <param name="bindNode">The contents of the configuration file associated with the business tier of this E-Notebook system.</param>
        /// <param name="vUserName">The username of the user to be authenticated</param>
        /// <param name="vPassword">The password of the user to be authenticated</param>
        /// <param name="operation">Indicates the operation being performed: authentication (authenticate) or creation of a new user (addUser)</param>
        /// <returns>True if bound, false if not</returns>
        /// <remarks>Forcing a bind with NativeObject method. If the object is null then we haven't managed to bind.
        /// A DN from the query method is needed </remarks>
        private bool Bind(System.Xml.XmlNode bindNode, string vUserName, string vPassword, string operation)
        {
            bool status = false;
            XmlNode errorNode = null;
            const string ROUTINE_NAME = "Bind";
            string error = "";

            if (!status)
            {
                Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "Reading Bind node" + bindNode.InnerText);
                Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "Attempting to bind as user with password");
                XmlNode domainNode = bindNode.SelectSingleNode("basedn");
                XmlNode userNode = bindNode.SelectSingleNode("dn");
                errorNode = bindNode.SelectSingleNode("error");

                if (vPassword != null && userNode == null)
                {
                    userNode = bindNode.SelectSingleNode("//ldapUserDN");
                    vPassword = GetAccountPass(bindNode.SelectSingleNode("//ldapPass"));
                }

                if (domainNode != null)
                {
                    string domainName = domainNode.InnerText;
                    string userName = userNode == null ? "" : userNode.InnerText.ToLower();
                    userName = userName.Replace("%username%", vUserName);
                    int dnIndex = userName.IndexOf("%dn%", StringComparison.InvariantCultureIgnoreCase);
                    if (dnIndex >= 0)
                    {
                        userName = (dnIndex == 0 ? "" : userName.Substring(0, dnIndex)) + GetAttributeValue("dn", connection) + (userName.Length <= 4 ? "" : userName.Substring(dnIndex + 4));
                    }
                    if (userName.IndexOf("%") >= 0 && filterResult != null)
                    {
                        foreach (string propName in filterResult.Properties.PropertyNames)
                        {
                            if (userName.IndexOf("%" + propName.ToLower() + "%") >= 0)
                            {
                                string toReplace = "";
                                if (filterResult.Properties[propName][0] is byte[])
                                    toReplace = System.Text.Encoding.ASCII.GetString((byte[])filterResult.Properties[propName][0]);
                                else
                                    toReplace = filterResult.Properties[propName][0].ToString();

                                userName = userName.Replace("%" + propName.ToLower() + "%", toReplace);
                                break;
                            }
                        }
                    }

                    //Allow customizers to do something before attempting to bind
                    if (customizers != null)
                    {
                        bool cancel = false;
                        for (int j = 0; j < customizers.Length; j++)
                        {
                            customizers[j].BeforeBinding(ref domainName, ref userName, ref cancel);
                            if (cancel) break;
                        }
                        if (cancel)
                        {
                            error = "Operation cancelled by Administrator";
                            Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "Cancel set to " + cancel + " error " + error);
                            EVWriter.Write(error);
                            throw new InvalidOperationException(error);
                        }
                    }

                    Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "vUserName: " + vUserName);

                    mRoot = new DirectoryEntry();
                    mRoot.AuthenticationType = authenticationType;
                    mRoot.Username = userName == "" ? null : userName;
                    if (vPassword != null)
                    {
                        if (mRoot.Username == null || mRoot.Username == "")
                        {
                            EVWriter.Write("For non-anonymous binding, the user DN is required.");
                            throw new InvalidOperationException("For non-anonymous binding, the user DN is required.");
                        }
                        mRoot.Password = vPassword;
                    }
                    mRoot.Path = "LDAP://" + domainName;
                    Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "Bind user = " + mRoot.Username);
                    Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "Bind string = " + mRoot.Path);

                    try
                    {
                        object nativeObject = mRoot.NativeObject;
                        if (nativeObject != null)
                        {
                            Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "Successfully bound");
                            status = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "Error while trying to bind: " + ex.Message);
                        if (!ex.Message.Contains("The server is not operational."))
                        {
                            status = false;
                            EVWriter.Write("Error while trying to bind: " + ex.Message);
                            throw new InvalidOperationException(errorNode.InnerText);
                        }
                    }
                }
                else
                {
                    error = "Configuration file is not valid";
                    Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, error);
                    EVWriter.Write(error);
                    throw new InvalidOperationException(error);
                }
            }
            return status;
        }

        /// <summary>
        /// Get an attribute from the LDAP search
        /// </summary>
        /// <param name="attributeName">Attribute to obtain form the LDAP search</param>
        /// <param name="conn">The object used to communicate between the client tier, the business tier and the database tier.</param>
        /// <returns>An XML string detailing the attribute name and the attribute value.</returns>
        public string GetAttributeValue(string attributeName, ENSConnection conn)
        {
            string result = "";
            const string ROUTINE_NAME = "GetAttributeValue";
            //Set the connection
            connection = conn;

            if (filterResult == null)
            {
                Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "Reading Bind node");
                EVWriter.Write("Please authentiate user before requesting attribute values");
                throw new InvalidOperationException("Please authentiate user before requesting attribute values");
            }
            else
            {
                try
                {
                    result = filterResult.Properties[attributeName][0].ToString();
                }
                catch (Exception e)
                {
                    // Get the dn if it is requested but not found in search results' attributes.
                    try
                    {
                        if (attributeName.Equals("dn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return filterResult.Path.Substring(filterResult.Path.LastIndexOf('/') + 1);
                        }
                    }
                    catch { }

                    EVWriter.Write("Error during authentication: " + e.Message);
                    throw new InvalidOperationException("Unknown Error during authentication.", e);
                }
            }
            return result;
        }


        /// <summary>
        /// Get the plain text password of the ldap service account for log in non-anonymous way
        /// when the LDAP server does not support it
        /// </summary>
        /// <param name="ldapPasswordNode">The contents of the configuration file associated with the business tier of this E-Notebook system.</param>
        /// <returns>ldap service account password decrypted</returns>
        private string GetAccountPass(System.Xml.XmlNode ldapPasswordNode)
        {
            if (ldapPasswordNode == null) return null;
            byte[] encrypted = HexConverter.ToByteArray(ldapPasswordNode.InnerText);
            byte[] iv = { 218, 123, 211, 4, 145, 147, 132, 228, 121, 39, 222, 60, 158, 10, 229, 62 };
            byte[] key = UnicodeConverter.ToByteArray("LdapPass");

            var fipsEnabled = SSOConfigurationProvider.GetConfig().GetSettings["FIPS_ENABLED"] != null && SSOConfigurationProvider.GetConfig().GetSettings["FIPS_ENABLED"].Value.ToUpper() == "TRUE";

            System.Security.Cryptography.ICryptoTransform decryptor = null;
            if (fipsEnabled)
            {
                AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider();
                decryptor = aesCryptoProvider.CreateDecryptor(key, iv);
            } 
            else
            {
                var myRijndael = new System.Security.Cryptography.RijndaelManaged();
                decryptor = myRijndael.CreateDecryptor(key, iv);
            }

            //Now decrypt the previously encrypted message using the decryptor
            // obtained in the above step.
            using (MemoryStream msDecrypt = new MemoryStream(encrypted))
            {
                using (System.Security.Cryptography.CryptoStream csDecrypt = new System.Security.Cryptography.CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    byte[] fromEncrypt = new byte[encrypted.Length];
                    int numRead = csDecrypt.Read(fromEncrypt, 0, (int)encrypted.Length);
                    byte[] targetDecrypt = new byte[numRead];
                    Array.Copy(fromEncrypt, 0, targetDecrypt, 0, numRead);
                    return UnicodeConverter.ToUnicodeString(targetDecrypt);
                }
            }
        }


        /// <summary>
        /// Queries the LDAP directory domain for a partiular user
        /// </summary>
        /// <param name="queryNode">The contents of the configuration file associated with the business tier of this E-Notebook system.</param>
        /// <param name="vUserName">The username of the user to be authenticated</param>
        /// <param name="vPassword">The password of the user to be authenticated</param>
        /// <param name="operation">Indicates the operation being performed: authentication (authenticate) or creation of a new user (adUser)</param>
        /// <returns>True if the entity was found in the domain. False otherwise</returns>
        private bool Query(System.Xml.XmlNode queryNode, string vUserName, string vPassword, string operation)
        {
            bool status = false;
            XmlNode errorNode = null;
            string error = "";
            const string ROUTINE_NAME = "Query";

            Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "Attempting query execution");
            Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "username = " + vUserName);

            using (DirectorySearcher search = new DirectorySearcher(mRoot))
            {
                search.ReferralChasing = ReferralChasingOption.All;
                errorNode = queryNode.SelectSingleNode("error");

                if (queryNode.Attributes["scope"] != null)
                {
                    switch (queryNode.Attributes["scope"].Value.ToLower())
                    {
                        case "subtree":
                            search.SearchScope = System.DirectoryServices.SearchScope.Subtree;
                            break;
                        case "base":
                            search.SearchScope = System.DirectoryServices.SearchScope.Base;
                            break;
                        case "onelevel":
                            search.SearchScope = System.DirectoryServices.SearchScope.OneLevel;
                            break;
                        default:
                            error = "Invalid scope value. Allowed values are: subtree, base and onelevel";
                            Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, error);
                            throw new InvalidOperationException(error);
                    }
                }

                if (queryNode.Attributes["timeOut"] != null)
                {
                    try
                    {
                        int secs = Int32.Parse(queryNode.Attributes["timeOut"].Value);
                        TimeSpan timeOut = new TimeSpan(0, 0, 0, secs);
                        search.ClientTimeout = timeOut;
                    }
                    catch (Exception e)
                    {
                        error = "Invalid timeout value. Only integers are allowed " + e.Message;
                        Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, error);
                        EVWriter.Write(error);
                        throw new InvalidOperationException(error);
                    }
                }

                //Get the filter string.
                string filter = "";
                XmlNode filterNode = queryNode.SelectSingleNode("filter");
                if (filterNode != null)
                {
                    string filterText = filterNode.InnerText;
                    Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "Before customizer action: filterText = " + filterText);
                    Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "Attempting to call method BeforeQuery of customizer with username " + vUserName);

                    //Allow customizers to do something before attempting to query
                    if (customizers != null)
                    {
                        bool cancel = false;
                        for (int j = 0; j < customizers.Length; j++)
                        {
                            customizers[j].BeforeQuery(ref filterText, ref vUserName, ref cancel);
                            if (cancel) break;
                        }
                        if (cancel)
                        {
                            error = "Operation cancelled by Administrator";
                            Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "Cancel set to " + cancel + " error " + error);
                            EVWriter.Write(error);
                            throw new InvalidOperationException(error);
                        }
                    }

                    Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "After customizer action: filterText = " + filterText);
                    filter = filterText.Replace("%username%", vUserName);
                    Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "After %username% replacement: Filter = " + filter.ToString());
                }
                search.Filter = filter;

                //Get the attribute to return.
                foreach (XmlNode attrNode in queryNode.SelectNodes("attr"))
                {
                    search.PropertiesToLoad.Add(attrNode.InnerText);
                }

                SearchResult searchResult = search.FindOne();
                if (searchResult != null)
                {
                    filterResult = searchResult;
                    Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "Filter succeeded in finding the user " + vUserName);
                    status = true;
                }
                else
                {
                    Utils.WriteLog(connection, this.GetType().Name, ROUTINE_NAME, "Filter failed to find the user " + vUserName);
                    status = false;
                    EVWriter.Write("Filter failed to find the user " + vUserName);
                    throw new InvalidOperationException(errorNode.InnerText.Replace("%username%", vUserName));
                }
            }
            return status;
        }

        public int GetCSExpiryDate(string userName, string password)
        {            
            int daysForPasswordExpiration = Convert.ToInt16(ConfigurationManager.AppSettings.Get("DaysToExpire"));
            return daysForPasswordExpiration;
        }

    }
}
