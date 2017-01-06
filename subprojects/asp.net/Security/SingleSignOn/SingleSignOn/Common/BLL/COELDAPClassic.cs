using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.DirectoryServices;
//using System.DirectoryServices.ActiveDirectory;
using System.Xml;
using CambridgeSoft.COE.Security.Services.Utlities;

namespace CambridgeSoft.COE.Security.Services
{
    public class COELDAPClassic : ICOESSO
    {
        const long ADS_OPTION_PASSWORD_PORTNUMBER = 6;
        const long ADS_OPTION_PASSWORD_METHOD = 7;

        const int ADS_PASSWORD_ENCODE_REQUIRE_SSL = 0;
        const int ADS_PASSWORD_ENCODE_CLEAR = 1;

        static readonly string LDAP_PROTOCOL = ConfigString("LDAP_PROTOCOL");
        static readonly string LDAP_SERVER_NAME = ConfigString("LDAP_SERVER_NAME");
        static readonly string LDAP_PORT = ConfigString("LDAP_PORT");
        static readonly string ACTIVE_DIRECTORY_DOMAIN = ConfigString("ACTIVE_DIRECTORY_DOMAIN");
        static readonly string LDAP_FILTER = ConfigString("LDAP_FILTER");
        static readonly string LDAP_DN = ConfigString("LDAP_DN");
        static readonly string LDAP_USER_DN = ConfigString("LDAP_USER_DN");
        static readonly string LDAP_ENCRYPT_SERVICE_PWD = ConfigString("LDAP_ENCRYPT_SERVICE_PWD");
        static readonly string LDAP_SERVICE_ACCOUNT_PWD = ConfigString("LDAP_SERVICE_ACCOUNT_PWD");
        static readonly string LDAP_SERVICE_ACCOUNT_NAME = ConfigString("LDAP_SERVICE_ACCOUNT_NAME");
        static readonly SearchScope LDAP_SEARCH_SCOPE = GetSearchScope(ConfigString("LDAP_SEARCH_SCOPE"));
        static readonly AuthenticationTypes LDAP_AUTHENTICATION_TYPE = GetAuthType(ConfigString("LDAP_AUTHENTICATION_TYPE"));
        static readonly string LDAP_DN_PROPERTY = (ConfigString("LDAP_DN_PROPERTY") != null && ConfigString("LDAP_DN_PROPERTY") != "") ? ConfigString("LDAP_DN_PROPERTY") : "dn";

        #region ICOESSO Members

        public bool checkUserExists(string userName)
        {
            return IsLDAPUser(userName);
            //throw new Exception("The method or operation is not implemented.");
        }

        public bool ValidateUser(string userName, string password)
        {

            return IsAuthenticated(userName, password);
            //throw new Exception("The method or operation is not implemented.");
        }

        public XmlDocument GetUserInfo(string userName)
        {
            DirectoryEntry entry = GetDirectorySearcherEntry();
            XmlDocument xmldoc = new XmlDocument();

            XmlElement nodeElem = xmldoc.CreateElement("results");
            xmldoc.AppendChild(nodeElem);

            XmlElement root = xmldoc.DocumentElement;
            try
            {

                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = LDAP_FILTER.Replace("%username%", userName);

                foreach (SSOConfigurationReturnInfo ri in SSOConfigurationLDAP.GetConfig().GetUserReturnInfo)
                {
                    search.PropertiesToLoad.Add(ri.ldapCode);
                }

                search.SearchScope = LDAP_SEARCH_SCOPE;
                search.ReferralChasing = ReferralChasingOption.All;
                SearchResult result = search.FindOne();

                if (null == result)
                {
                    throw new Exception("error");
                    //return "Error";
                }


                foreach (SSOConfigurationReturnInfo ri in SSOConfigurationLDAP.GetConfig().GetUserReturnInfo)
                {
                    string nodeName = "";
                    if (!(ri.nodeName == ""))
                    {
                        nodeName = ri.nodeName;
                    }
                    else
                    {
                        nodeName = ri.ldapCode;
                    }
                    XmlElement elem = xmldoc.CreateElement(nodeName);

                    elem.SetAttribute("mapTo", ri.mapTo);
                    elem.SetAttribute("displayName", ri.displayName);
                    try
                    {
                        elem.InnerText = result.Properties[ri.ldapCode][0].ToString();
                    }
                    catch
                    {
                        elem.InnerText = "";
                    }
                    root.AppendChild(elem);

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not find user. " + ex.Message);
            }

            return xmldoc;


        }

        public string GetCSSecurityPassword(string userName, string password)
        {
            return password;
        }

        #endregion

        private static string ConfigString(string key)
        {
            try
            {

                string configValue = String.Empty;

                if (key == "LDAP_SERVICE_ACCOUNT_PWD" && LDAP_ENCRYPT_SERVICE_PWD == "YES")
                {
                    Configuration config = SSOConfig.OpenConfig();
                    SSOConfigurationLDAP ssoldConfig = (SSOConfigurationLDAP)config.GetSection("SSOConfiguration/LDAPConfiguration");
                    //this is where encryption shoudl go
                    string encryptedPass = ssoldConfig.GetLDAPSettings[key].Value.ToString();
                    if (!(Encryptor.IsEncrypted(encryptedPass)))
                    {
                        ssoldConfig.GetLDAPSettings[key].Value = Encryptor.Encrypt(encryptedPass);
                        config.Save();
                    }
                    configValue = Encryptor.Decrypt(SSOConfigurationLDAP.GetConfig().GetLDAPSettings[key].Value.ToString());
                }
                else
                {
                    configValue = SSOConfigurationLDAP.GetConfig().GetLDAPSettings[key].Value.ToString();
                }


                return configValue;
            }
            catch { }
            return "";
        }

        private static int ConfigValue(string key)
        {
            try { return Int32.Parse(SSOConfigurationLDAP.GetConfig().GetLDAPSettings[key].Value.ToString()); }
            catch { }
            return 0;
        }

        private bool IsAuthenticated(string userName, string password)
        {
            DirectoryEntry entry = GetDirectorySearcherEntry();
            string userDn = LDAP_USER_DN;
            try
            {
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = LDAP_FILTER.Replace("%username%", userName);
                search.PropertiesToLoad.Add(LDAP_DN_PROPERTY);
                search.SearchScope = LDAP_SEARCH_SCOPE;
                search.ReferralChasing = ReferralChasingOption.All;

                SearchResult result = search.FindOne();
                if (null == result) return false;
                ResultPropertyValueCollection value = result.Properties["dn"];
                userDn = userDn.Replace("%username%", userName);
                //S. Homer: Binding always succeeds -- allowing user authentication even with a bad password -- if the userDn is an empty string.
                //So if the LDAP server does not include dn as a user attribute, get the userDn from the Path instead. 
                //if (userDn == "" && value.Count > 0) userDn = value[0].ToString();
                if (userDn == "")
                {
                    userDn = ((value.Count > 0) ? value[0].ToString() : result.Path.Substring(result.Path.LastIndexOf('/') + 1));
                }
                //End change by S. Homer

                //what if you don't want to use just plain vanilla username
                //if (userName.IndexOf("%") >= 0 && filterResult != null)
                //{
                //    foreach (string propName in filterResult.Properties.PropertyNames)
                //    {
                //        if (userName.IndexOf("%" + propName.ToLower() + "%") >= 0)
                //        {
                //            string toReplace = "";
                //            if (filterResult.Properties[propName][0] is byte[])
                //                toReplace = System.Text.Encoding.ASCII.GetString((byte[])filterResult.Properties[propName][0]);
                //            else
                //                toReplace = filterResult.Properties[propName][0].ToString();

                //            userName = userName.Replace("%" + propName.ToLower() + "%", toReplace);
                //            break;
                //        }
                //    }
                //}



                //JHS has hardcoded as fast bind this should probably have a secondary ldap auth type in the config
                //object obj = new DirectoryEntry(entry.Path, userDn, password, LDAP_AUTHENTICATION_TYPE).NativeObject;
                object obj = new DirectoryEntry(entry.Path, userDn, password, AuthenticationTypes.FastBind).NativeObject;

            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error authenticating user.\nPath: {0}\nUser DN: {1}\n", entry.Path, userDn) + ex.ToString());
            }

            return true;
        }

        private bool IsLDAPUser(string userName)
        {
            DirectoryEntry entry = GetDirectorySearcherEntry();
            try
            {
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = LDAP_FILTER.Replace("%username%", userName);
                search.PropertiesToLoad.Add(LDAP_DN_PROPERTY);
                search.SearchScope = LDAP_SEARCH_SCOPE;
                search.ReferralChasing = ReferralChasingOption.All;
                Int32 secs = 5;
                TimeSpan timeOut = new TimeSpan(0, 0, 0, secs);
                search.ClientTimeout = timeOut;
                SearchResult result = search.FindOne();
                if (null == result) return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not find user. " + ex.Message);
            }
            return true;
        }

        private DirectoryEntry GetDirectorySearcherEntry()
        {
            string path = LDAP_SERVER_NAME;
            if (LDAP_PROTOCOL != null && LDAP_PROTOCOL != string.Empty)
            {
                path = LDAP_PROTOCOL + "://" + path;
            }
            else
            {
                path = "LDAP" + "://" + path;
            }

            if (LDAP_PORT != "") path = string.Concat(path, ":", LDAP_PORT);
            if (LDAP_DN != "") path = string.Concat(path, "/", LDAP_DN);
            DirectoryEntry entry = new DirectoryEntry(path);
            entry.AuthenticationType = LDAP_AUTHENTICATION_TYPE;
            //use service account to bind......
            if (LDAP_SERVICE_ACCOUNT_NAME != "")
                entry.Username = ((ACTIVE_DIRECTORY_DOMAIN != "") ? (ACTIVE_DIRECTORY_DOMAIN + @"\") : "") + LDAP_SERVICE_ACCOUNT_NAME;
            if (LDAP_SERVICE_ACCOUNT_PWD != "")
                entry.Password = LDAP_SERVICE_ACCOUNT_PWD;
            object obj = entry.NativeObject;
            return entry;
        }

        private static SearchScope GetSearchScope(string scope)
        {

            SearchScope sc = SearchScope.Subtree;
            switch (scope.ToUpper())
            {
                case "BASE":
                    sc = SearchScope.Base;
                    break;
                case "ONELEVEL":
                    sc = SearchScope.OneLevel;
                    break;
                default:
                    sc = SearchScope.Subtree;
                    break;
            }
            return sc;
        }


        private static AuthenticationTypes GetAuthType(string authType)
        {
            AuthenticationTypes authenticationType = AuthenticationTypes.Secure;
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
                case "":
                    authenticationType = AuthenticationTypes.None;
                    break;
                default:
                    authenticationType = AuthenticationTypes.Secure;
                    break;
            }
            return authenticationType;
        }
    }

}