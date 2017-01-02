using System;
using System.Collections.Generic;
using System.Xml;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using Csla.Server;
using System.Text.RegularExpressions;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using System.Web.UI;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// COEFramework utilitarian class.
    /// </summary>
    public class FrameworkUtils
    {
        /// <summary>
        /// Builds a soap exception based on the notifications.
        /// </summary>
        /// <param name="notifications">The <see cref="Notification"/> parameter</param>
        /// <exception cref="SoapException">Throws a soap exception wrapping the notivications.</exception>
        public static void BuildAndThrowSoapException(Notification notifications)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            System.Xml.XmlNode node = doc.CreateNode(XmlNodeType.Element, System.Web.Services.Protocols.SoapException.DetailElementName.Name, SoapException.DetailElementName.Namespace);
            NotificationUtility.FormatAsSoapException(notifications, ref node, ref doc);
            SoapException se = new SoapException("Notifications", SoapException.ClientFaultCode, null, node);
            throw se;
        }

        /// <summary>
        /// Gets an App config setting
        /// </summary>
        /// <param name="group">Groups which it belongs</param>
        /// <param name="key">Settings Name</param>
        /// <returns>Found settting or empty</returns>
        public static string GetAppConfigSetting(string app, string group, string key)
        {
            return GetAppConfigSetting(app, group, key, false);
        }

        public static string GetAppConfigSetting(string app, string group, string key, bool ignoreCache)
        {
            string retVal = string.Empty;
            COEConfigurationBO bo;
            if (!string.IsNullOrEmpty(app) && !string.IsNullOrEmpty(group) && !string.IsNullOrEmpty(key))
            {
                AppSettingsData appSet = GetAppConfigSettings(app, ignoreCache);
                if (appSet != null)
                {
                    if (appSet.SettingsGroup.Get(group) != null)
                    {
                        if (appSet.SettingsGroup.Get(group).Settings.Get(key) != null)
                        {
                            if (!string.IsNullOrEmpty(appSet.SettingsGroup.Get(group).Settings.Get(key).Value))
                            {
                                retVal = appSet.SettingsGroup.Get(group).Settings.Get(key).Value;
                            }
                        }
                    }
                }
            }
            return retVal;
        }


        /// <summary>
        /// Gets an App config setting
        /// </summary>
        /// <param name="app">App name</param>
        /// <returns>All the app settings</returns>
        public static AppSettingsData GetAppConfigSettings(string app)
        {
            return GetAppConfigSettings(app, false);
        }

        /// <summary>
        /// Gets an App config setting
        /// </summary>
        /// <param name="app">App name</param>
        /// <returns>All the app settings</returns>
        public static bool IsAValidToken(string token)
        {
            bool retVal = false;
            switch (token.ToUpper())
            {
                case "&&LOGGEDINUSER":
                    retVal = true;
                    break;
                case "&&LOGGEDINUSERNAME":
                    retVal = true;
                    break;
                case "&&CURRENTDATE":
                    retVal = true;
                    break;
            }
            return retVal;
        }

        /// <summary>
        /// Gets an App config setting
        /// </summary>
        /// <param name="app">App name</param>
        /// <returns>All the app settings</returns>
        public static string ReplaceSpecialTokens(string token, bool isSqlQuery)
        {
            string decodedToken = string.Empty;
            string quote = string.Empty;

            decodedToken = WebUtils.Decode(token);
            quote = isSqlQuery ? Convert.ToChar(39).ToString() : "";

            if (decodedToken.ToUpper().Contains("&&LOGGEDINUSER"))
            {
                decodedToken = decodedToken.ToUpper().Replace("&&LOGGEDINUSER", COEUser.ID.ToString());
            }

            if (decodedToken.ToUpper().Contains("&&LOGGEDINUSERNAME"))
            {
                decodedToken = decodedToken.ToUpper().Replace("&&LOGGEDINUSERNAME", quote + COEUser.Name.ToUpper() + quote);
            }

            if (decodedToken.ToUpper().Contains("&&CURRENTDATE"))
            {
                decodedToken = decodedToken.ToUpper().Replace("&&CURRENTDATE", quote + DateTime.Today.ToShortDateString() + quote);
            }
            return decodedToken;
        }

        /// <summary>
        /// Gets an App config setting using or not the cache
        /// </summary>
        /// <param name="app">App name</param>
        /// <param name="notFromCache">boolean to specify if cache should be used</param>
        /// <returns>All the app settings</returns>
        public static AppSettingsData GetAppConfigSettings(string app, bool ignoreCache)
        {
            return GetAppConfigSettings(app, ConfigurationManager.AppSettings["ConfigurationName"], ignoreCache);
        }

        /// <summary>
        /// Gets an App config setting
        /// </summary>
        /// <param name="app">App name</param>
        /// <returns>All the app settings</returns>
        public static AppSettingsData GetAppConfigSettings(string app, string description)
        {
            return GetAppConfigSettings(app, description, false);
        }

        /// <summary>
        /// Gets an App config setting
        /// </summary>
        /// <param name="app">App name</param>
        /// <param name="description">Description to search</param>
        /// <param name="notFromCache">boolean to specify if cache should be used</param>
        /// <returns>All the app settings</returns>
        public static AppSettingsData GetAppConfigSettings(string app, string description, bool ignoreCache)
        {
            AppSettingsData retVal = null;
            if(!string.IsNullOrEmpty(app))
            {
                COEConfigurationBO bo;
                if ((bo = COEConfigurationBO.Get(app, description, ignoreCache)) != null && bo.ConfigurationSection != null)
                {
                    retVal = ((ApplicationDataConfigurationSection)bo.ConfigurationSection).AppSettings;
                }
                else
                {
                    ApplicationDataConfigurationSection configurationSection = new ApplicationDataConfigurationSection();
                    retVal = configurationSection.AppSettings;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets an App config setting xml
        /// </summary>
        /// <param name="app">App name</param>
        /// <returns>All the app settings</returns>
        public static string GetAppConfigSettingsXml(string app)
        {
            string retVal = null;
            if (!string.IsNullOrEmpty(app))
            {
                COEConfigurationBO bo;
                if ((bo = COEConfigurationBO.Get(app, ConfigurationManager.AppSettings["ConfigurationName"])) != null)
                {
                    retVal = bo.GetConfigurationSettingsXml();
                }
                else
                {
                    retVal = string.Empty;
                }
            }
            return retVal;
        }

        public static void SaveAppConfigSettings(string app, AppSettingsData data)
        {
            COEConfigurationBO bo = COEConfigurationBO.Get(app, ConfigurationManager.AppSettings["ConfigurationName"]);

            if (bo == null)
                bo = COEConfigurationBO.New(app, ConfigurationManager.AppSettings["ConfigurationName"]);

            ApplicationDataConfigurationSection configSection = new ApplicationDataConfigurationSection();
            configSection.AppSettings = data;

            bo.ConfigurationSection = configSection;

            bo.Save();
        }

        public static void SaveConfigurationSettingsFromXml(string appName, string xml)
        {
            if (!string.IsNullOrEmpty(appName))
            {
                COEConfigurationBO bo;
                if ((bo = COEConfigurationBO.Get(appName, ConfigurationManager.AppSettings["ConfigurationName"])) != null)
                {
                    bo.SetConfigurationSettingsFromXml(xml);
                }
            }

        }

        /// <summary>
        /// Registers the yui js script into the page.
        /// </summary>
        /// <param name="page">The page to register the js lib.</param>
        /// <param name="js">The js name.</param>
        public static void RegisterYUIScript(System.Web.UI.Page page, FrameworkConstants.YUI_JS js)
        {
            if (!page.ClientScript.IsClientScriptIncludeRegistered(js.ToString()))
            {
                switch (js)
                {
                    case FrameworkConstants.YUI_JS.CONTAINERMIN:
                        page.ClientScript.RegisterClientScriptInclude(js.ToString(), FrameworkConstants.YUIJSFOLDER + "container-min.js");
                        break;
                    case FrameworkConstants.YUI_JS.DRAGDROPMIN:
                        page.ClientScript.RegisterClientScriptInclude(js.ToString(), FrameworkConstants.YUIJSFOLDER + "dragdrop-min.js");
                        break;
                    case FrameworkConstants.YUI_JS.YAHOODOMEVENTS:
                        page.ClientScript.RegisterClientScriptInclude(js.ToString(), FrameworkConstants.YUIJSFOLDER + "yahoo-dom-event.js");
                        break;
                    case FrameworkConstants.YUI_JS.ELEMENTMIN:
                        page.ClientScript.RegisterClientScriptInclude(js.ToString(), FrameworkConstants.YUIJSFOLDER + "element-min.js");
                        break;
                    case FrameworkConstants.YUI_JS.UPLOADERMIN:
                        page.ClientScript.RegisterClientScriptInclude(js.ToString(), FrameworkConstants.YUIJSFOLDER + "uploader-min.js");
                        break;
                    case FrameworkConstants.YUI_JS.POSITIONING:
                        page.ClientScript.RegisterClientScriptInclude(js.ToString(), FrameworkConstants.YUIJSFOLDER + "positioning.js");
                        break;
                }
            }
        }

        /// <summary>
        /// Adds the reference to the YUI css
        /// </summary>
        /// <param name="page">The page to add the ref.</param>
        /// <param name="css">The CSS to add.</param>
        public static void AddYUICSSReference(System.Web.UI.Page page, FrameworkConstants.YUI_CSS css)
        {
            System.Web.UI.HtmlControls.HtmlLink yuiContainer = new System.Web.UI.HtmlControls.HtmlLink();
            yuiContainer.Attributes.Add("rel", "stylesheet");
            switch(css)
            {
                case FrameworkConstants.YUI_CSS.CONTAINER:
                    yuiContainer.Href = FrameworkConstants.YUICSSFOLDER + "container.css"; break;
                case FrameworkConstants.YUI_CSS.FONTSMIN:
                    yuiContainer.Href = FrameworkConstants.YUICSSFOLDER + "fonts-min.css"; break;
                case FrameworkConstants.YUI_CSS.AUTOCOMPLETE:
                    yuiContainer.Href = FrameworkConstants.YUICSSFOLDER + "autocomplete.css"; break;
                case FrameworkConstants.YUI_CSS.PAGINATOR:
                    yuiContainer.Href = FrameworkConstants.YUICSSFOLDER + "paginator.css"; break;
                case FrameworkConstants.YUI_CSS.DATATABLE:
                    yuiContainer.Href = FrameworkConstants.YUICSSFOLDER + "datatable.css"; break;
            }
            yuiContainer.Attributes.Add("type", "text/css");
            page.Header.Controls.Add(yuiContainer);
        }

        /// <summary>
        /// Gets the hide chem draws JS code.
        /// </summary>
        /// <returns></returns>
        public static string GetHideChemDrawsJSCode()
        {
            return @"function HideChemDraws() {
                            if (typeof(cd_objectArray)!='undefined' && typeof(cd_objectArray)!='unknown' && cd_objectArray) {
                                for(i = 0; i < cd_objectArray.length; i++) {
                                    cd_getSpecificObject(cd_objectArray[i]).style.visibility = 'hidden';
                                }
                            }
                        }";
        }

        /// <summary>
        /// Gets the show chem draws JS code.
        /// </summary>
        /// <returns></returns>
        public static string GetShowChemDrawsJSCode()
        {
            return @"function ShowChemDraws() {
                            if (typeof(cd_objectArray)!='undefined' && typeof(cd_objectArray)!='unknown' && cd_objectArray) {
                                for(i = 0; i < cd_objectArray.length; i++) {
                                    cd_getSpecificObject(cd_objectArray[i]).style.visibility = 'visible';
                                }
                            }
                        }";
        }

        /// <summary>
        /// Registers a given formelemtn to control client events.
        /// </summary>
        /// <param name="controlToAdd">FormElment to check for its client side events</param>
        /// <param name="list">Events list</param>
        public static void RegisterToControlClientEvents(ICOEGenerableControl controlToAdd, List<FormGroup.COEEventInfo> list, System.Web.HttpContext currentContext)
        {
            foreach (FormGroup.COEEventInfo currentEvent in list)
            {
                string eventHandlerScript = !string.IsNullOrEmpty(currentEvent.EventHandlerScript) ? currentEvent.EventHandlerScript : currentEvent.Value;

                if (!string.IsNullOrEmpty(currentEvent.EventHandlerName))
                {
                    ((System.Web.UI.WebControls.WebControl)controlToAdd).Attributes.Add(currentEvent.EventName,
                                                                                string.Format("return {0}(this, event)", currentEvent.EventHandlerName));
                    //maybe we should allow the user to specify the method handler code here too, and register it with the page.
                }
                else if (!string.IsNullOrEmpty(eventHandlerScript))
                {
                    string scriptCode = ReplaceControlNames(eventHandlerScript, currentContext);

                    ((System.Web.UI.WebControls.WebControl)controlToAdd).Attributes.Add(currentEvent.EventName,
                                                                               scriptCode);
                }
            }
        }

        /// <summary>
        /// Replaces the control names removing/translating known characters
        /// </summary>
        /// <param name="script">String to inspect</param>
        /// <returns></returns>
        public static string ReplaceControlNames(string script, System.Web.HttpContext currentContext)
        {
            return ReplaceReservedWords(ReplaceReservedWords(script, "@", null, currentContext), "<%=", "%>", currentContext);
        }

        /// <summary>
        /// Replaces the reserved words.
        /// </summary>
        /// <param name="script">String to inspect/manipulate</param>
        /// <param name="openToken">The open token.</param>
        /// <param name="closingToken">The closing token.</param>
        /// <returns></returns>
        public static string ReplaceReservedWords(string script, string openToken, string closingToken, System.Web.HttpContext currentContext)
        {
            string scriptCode = script;
            int startIndex = 0;
            while (startIndex < scriptCode.Length && scriptCode.IndexOf(openToken, startIndex) > 0)
            {
                startIndex = scriptCode.IndexOf(openToken) + openToken.Length;
                int endIndex = closingToken == null ? 
                                            scriptCode.IndexOfAny(FrameworkConstants.TokenSeparator, startIndex + openToken.Length) : 
                                            scriptCode.IndexOf(closingToken, startIndex + openToken.Length);
                if (endIndex < 0)
                    endIndex = scriptCode.Length - openToken.Length;
                int length = endIndex - startIndex;
                if (length > 0)
                {
                    string reservedWord = scriptCode.Substring(startIndex, endIndex - startIndex);
                    string newValue = string.Empty;
                    if (reservedWord.Contains("Context"))
                    {
                        COEDataBinder dataBinder = new COEDataBinder(currentContext);
                        if (dataBinder.RetrieveProperty(reservedWord.Replace("Context", "")) != null)
                            newValue = dataBinder.RetrieveProperty(reservedWord.Replace("Context", "")).ToString();
                        else
                            newValue = string.Format("COEFormGenerator Client script error: {0} NOT FOUND!", reservedWord);
                    }
                    else
                    {
                        if (currentContext.CurrentHandler is Page)
                        {
                            Control referencedControl = ((Page)currentContext.CurrentHandler).FindControl(reservedWord);
                            if (referencedControl != null)
                                newValue = referencedControl.ClientID;
                            else
                                newValue = string.Format("COEFormGenerator Client script error: {0} NOT FOUND!", reservedWord);
                        }
                    }
                    scriptCode = scriptCode.Substring(0, startIndex - openToken.Length) + newValue + scriptCode.Substring(endIndex + (closingToken == null ? 0 : closingToken.Length));
                }
            }
            return scriptCode;
        }

        /// <summary>
        /// Cast the attribute associated to ENUM.
        /// </summary>
        /// <param name="value">Enum to be processed</param>
        /// <returns>The attribute value</returns>
        public static string CastEnumAttrubute(Enum value)
        {
            System.Reflection.FieldInfo fi = value.GetType().GetField(value.ToString());

            System.ComponentModel.DescriptionAttribute[] attributes =
                (System.ComponentModel.DescriptionAttribute[])fi.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        } 
    }

    /// <summary>
    /// Class to hold common strings, enums, etc to be used across thew fw.
    /// </summary>
    public class FrameworkConstants
    {
        public const string YUIJSFOLDER = "/COECommonResources/YUI/"; //TODO: read from config.
        public const string YUICSSFOLDER = "/COECommonResources/YUI/"; //TODO: read from config.
        public const string YUI_YAHOODOMEVENTS = "yahoo-dom-event";
        public const string YUI_DRAGDROPMIN = "dragdrop-min";
        public const string YUI_CONTAINERMIN = "container-min";
        public const string YUI_POSITIONING = "positioning";
        public const string YUI_ELEMENTMIN = "element-min";
        public const string YUI_UPLOADERMIN = "uploader-min";
        public const string YUI_CONTAINERCSS = "container.css";
        public const string YUI_FONTSMIN = "fonts-min.css";

        public enum YUI_JS
        {
            YAHOODOMEVENTS,
            DRAGDROPMIN,
            CONTAINERMIN,
            POSITIONING,
            ELEMENTMIN,
            UPLOADERMIN,
        }

        public enum YUI_CSS
        {
            CONTAINER,
            FONTSMIN,
            AUTOCOMPLETE,
            PAGINATOR,
            DATATABLE
        }

        public static char[] TokenSeparator = { '\"', '\'', '@', ' ', ',', /*'.',*/ '|', '!', '\\', '%', '&', '/', '(', ')', '=', '?', '*', '-', '+', ';', ':', '<', '>', ';' };

    }

    public class MultiFieldSearchObject
    {
        
        #region Variables

        protected string _searchInput = String.Empty;
        protected string _searchType = String.Empty;
        protected string _searchValue = String.Empty;
        protected bool _isRegistryID = false;
        protected bool _isFormula = false;
        protected bool _isMW = false;
        protected bool _isChemicalName = false;
        protected string _searchFormula = String.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// The input string of the contructor is returned
        /// </summary>
        /// <value>The search input.</value>
        public string SearchInput
        {
            get { return _searchInput; }

        }
        /// <summary>
        /// Gets the type of the search. ex. CHEMICALNAME, MW, MF, REGISTRYID
        /// </summary>
        /// <value>The type of the search.</value>
        public string SearchType
        {
            get { return _searchType; }
        }

        /// <summary>
        /// True if searchInput is a Registry ID
        /// False if not a Registry ID
        /// </summary>
        /// <value>Is the SearchInput a registry ID. Matches the value of search type</value>
        public bool IsRegistryID
        {
            get { return _isRegistryID; }
        }

        /// <summary>
        /// True if searchInput is a Molecular Weight
        /// False if not a Molecular Weight
        /// </summary>
        /// <value>Is the SearchInput a MW. Matches the value of search type</value>
        public bool IsMW
        {
            get { return _isMW; }

        }

        /// <summary>
        /// True if searchInput is a Chemical Name
        /// False if not a Chemical Name
        /// </summary>
        /// <value>Is the SearchInput a MW. Matches the value of search type</value>
        public bool IsChemicalName
        {
            get { return _isChemicalName; }

        }

        /// <summary>
        /// True if searchInput is a Formula
        /// False if not a Formula
        /// </summary>
        /// <value>Is the SearchInput a Formula. Matches the value of search type</value>
        public bool IsFormula
        {
            get { return _isFormula; }

        }


        /// <summary>
        /// Gets the normalized version of search text
        /// </summary>
        /// <value>The search text.</value>
        public string SearchValue
        {
            get { return _searchValue; }

        }
        #endregion

        #region Constructors

        public MultiFieldSearchObject(string SearchInput)
        {
            _searchInput = SearchInput;
            SetSearchTypeAndValue();
            //_searchType = GetSearchType();
            //_searchValue = GetSearchText();
        }

        #endregion

        #region Methods
        private void SetSearchTypeAndValue()
        {

            string strSearch = String.Empty;

            //Check for Registry Id
            if (IsValidRegistry(_searchInput))
            {
                _searchValue = _searchInput.Trim();
                _searchType = "REGISTRYID";
                _isRegistryID = true;

                #region old code
                //code from current gateway
                //Session["TextValue"] = "1";
                //SearchIndex = 1;
                #endregion
            }
            //Check for Molecular Weight
            else if (IsMolecularWeight(_searchInput))
            {

                _searchType = "MW";
                _isMW = true;

                strSearch = _searchInput;
                string firstValue = string.Empty;
                firstValue = _searchInput.Substring(0, 1);
                if (firstValue == "=")
                {
                    strSearch = _searchInput.Remove(0, 1);
                }

                // Facundo: There is no need to parse the criterias, this is already done in the framework.
                //_searchValue = MWCheck(strSearch);
                _searchValue = _searchInput;

                #region old code
                //string firstValue = string.Empty;
                //firstValue = searchInput.Substring(0, 1);
                //if (firstValue == "=")
                //{
                //    searchInput = searchInput.Remove(0, 1);
                //}
                //string result = string.Empty;
                //result = objGF.MWCheck(searchInput);
                //searchFilterValue = result;
                //if (result != null)
                //{
                //    SearchIndex = 2;
                //}
                #endregion

            }
            //Check for Chemical Formula Search
            else if (IsValidFormula(_searchInput))
            {

                _searchValue = _searchFormula;
                _searchType = "FORMULA";
                _isFormula = true;

                #region old code
                //code from current gateway
                //Session["TempFormulaValue"] = objGF.strFor;
                //SearchIndex = 3;
                #endregion
            }

            //must be text
            else
            {
                _searchValue = _searchInput.Trim();
                _searchType = "CHEMICALNAME";
                _isChemicalName = true;
            }
        }

        #endregion


        #region private methods

        private bool IsValidRegistry(String strToCheck)
        {
            //Regular Expressions for CASRN and ACX.
            /*
             * A CAS registry number is separated by hyphens into three parts, 
             * the first consisting of up to 7 digits, 
             * the second consisting of two digits, 
             * and the third consisting of a single digit serving as a check digit. 
             * */
            string objCAS = "^[0-9\\?_]{1,7}[-][0-9\\?_]{1,2}[-][[0-9\\?_]{1}$|^[0-9\\?_-]+[\\*%]$|^[\\*%][0-9\\?_-]+$";
            string objACX = "^[\\*%]?[Xx][0-9\\?_]{1,7}[\\*%]?$|^[\\*%]?[Xx][0-9\\?_]{1,7}[\\*%]?$|^[\\*%]?[Xx][0-9\\?_]{1,7}-[0-9\\?_]{1}[\\*%]?$";

            //Regex object for CASRN and ACX.
            Regex regCheck = new Regex("(" + objCAS + ")");
            Regex regACX = new Regex("(" + objACX + ")");

            if (regCheck.IsMatch(strToCheck.Trim()))
            {
                return regCheck.IsMatch(strToCheck);
            }
            else if (regACX.IsMatch(strToCheck.Trim()))
            {
                return regACX.IsMatch(strToCheck);
            }
            return false;
        }

        //function to check for Molecular Formula
        /// <summary>
        /// check input string is valid for formula or not
        /// </summary>
        /// <param name="StrFormula"></param>
        /// <returns>boolean value</returns>
        private bool IsValidFormula(string strFormula)
        {
            if (strFormula.IndexOf("*") == -1)
            {
                if (strFormula.IndexOf("[") == 0 && strFormula.IndexOf("]") == strFormula.Length - 1)
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
            else
            {

                return false;
            }
        }


        private bool IsNumeric(string numString)
        {
            Match m = Regex.Match(numString, @"(\d*\.)?\d+");
            return (m.ToString() == numString);
        }

        /// <summary>
        /// check valid molecular wt by regular expression
        /// </summary>
        /// <param name="strNumber"></param>
        /// <returns>boolean value</returns>
        private bool IsMolecularWeight(string strNumber)
        {
            string firstValue = string.Empty;
            firstValue = strNumber.Substring(0, 1);
            if (firstValue == "=")
            {
                strNumber = strNumber.Remove(0, 1);
            }
            string strNumberWithoutBlankSpaces = strNumber.Replace(" ", "");
            string dOperPattern = "^[<>]{1}[=][0-9]+$|^[<>]{1}[=][0-9]{1,5}[.][0-9]+$|^[<>]{1}[=][.][0-9]+$";
            string sOperPattern = "^[<>]{1}[0-9]+$|^[<>]{1}[0-9]{1,5}[.][0-9]+$|^[<>]{1}[.][0-9]+$";
            string singlePattern = "^[0-9]+$|^[.][0-9]+$|^[0-9]*[.][0-9]+$";
            string rangePattern = "^[0-9]{1,5}[-][0-9]+$|^[0-9]{1,5}[-][0-9]{1,5}[.][0-9]+$|^[0-9]{1,5}[.][0-9]*[-][0-9]+$|^[0-9]{1,5}[.][0-9]*[-][0-9]{1,5}[.][0-9]+$|^[.][0-9]*[-][0-9]*[.][0-9]+$|^[0-9]{1,5}[.][0-9]*[-][.][0-9]+$|^[.][0-9]*[-][0-9]+$|^[0-9]{1,5}[-][.][0-9]+$";

            Regex regExp = new Regex("(" + dOperPattern + ")|(" + sOperPattern + ")|(" + singlePattern + ")|(" + rangePattern + ")");
            return regExp.IsMatch(strNumberWithoutBlankSpaces);
        }

        /// <summary>
        /// take mol wt string and make comma seprated string
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns>return mol wt as comma seprated value</returns>
        public string MWCheck(string inputString)
        {
            string minWt = string.Empty;
            string maxWt = string.Empty;
            string molecularWeight = string.Empty;

            //if the comparison operator is >= or >
            if ((inputString.Contains(">=")) || (inputString.Contains(">")))
            {
                int locationIndex;
                if (inputString.Contains(">="))
                {
                    locationIndex = inputString.IndexOf(">=");
                    minWt = inputString.Substring(locationIndex + 2);
                    if (!CheckRange(minWt))
                    {
                        return null;
                    }
                }
                else
                {
                    locationIndex = inputString.IndexOf(">");
                    minWt = inputString.Substring(locationIndex + 1);
                    if (!CheckRange(minWt))
                    {
                        return null;
                    }
                }
                double ValueString = Convert.ToDouble(minWt);
                //we want to use a dash and not a comma for ChemBioViz
                //the 12000 is kind of random as why not just make it >x
                //but for now I will leave it
                //molecularWeight = minWt + ",12000";
                molecularWeight = minWt + "-12000";

            }
            //if the comparison operator is <= or <
            else if ((inputString.Contains("<=")) || (inputString.Contains("<")))
            {
                int locationIndex = 0;
                if (inputString.Contains("<="))
                {
                    locationIndex = inputString.IndexOf("<=");
                    maxWt = inputString.Substring(locationIndex + 2);
                    if (!CheckRange(maxWt))
                    {
                        return null;
                    }
                }
                else
                {
                    locationIndex = inputString.IndexOf("<");
                    maxWt = inputString.Substring(locationIndex + 1);
                    if (!CheckRange(maxWt))
                    {
                        return null;
                    }
                }

                double ValueString = Convert.ToDouble(maxWt);

                //we want to keep the dash for ChemBioViz
                //molecularWeight = "0," + maxWt;
                molecularWeight = "0-" + maxWt;
            }
            //if the comparison operator is -
            else if (inputString.Contains("-"))
            {
                int locationIndex = inputString.IndexOf("-");
                if (locationIndex != 0)
                {
                    minWt = inputString.Substring(0, locationIndex);
                    maxWt = inputString.Substring(locationIndex + 1);
                    double ValueString1 = Convert.ToDouble(minWt);
                    double ValueString2 = Convert.ToDouble(maxWt);
                    if (!CheckRange(ValueString1.ToString()))
                    {
                        return null;
                    }
                    if (!CheckRange(ValueString2.ToString()))
                    {
                        return null;
                    }
                    //we want to keep the dash for ChemBioViz
                    //molecularWeight = ValueString1 + "," + ValueString2;
                    molecularWeight = ValueString1 + "-" + ValueString2;
                }
            }
            else
            {
                if (inputString.Contains("."))
                {
                    if (!CheckRange(inputString))
                    {
                        return null;
                    }
                    string Input = inputString + "99";
                    //we want to keep the dash for ChemBioViz
                    //molecularWeight = inputString + "," + Input;
                    molecularWeight = inputString + "-" + Input;
                }
                else
                {
                    if (!CheckRange(inputString))
                    {
                        return null;
                    }
                    minWt = inputString;
                    maxWt = inputString + ".99";

                    //we want to keep the dash for ChemBioViz
                    //molecularWeight = minWt + "," + maxWt;
                    molecularWeight = minWt + "-" + maxWt;

                }
            }
            return molecularWeight;
        }

        //function to check the valid maximum limit of Molecular Weight.
        /// <summary>
        /// checked input string is greater or less frm 12000
        /// </summary>
        /// <param name="inputStrValue"></param>
        /// <returns></returns>
        private bool CheckRange(string inputStrValue)
        {
            decimal value = Convert.ToDecimal(inputStrValue);
            if (value > 12000)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion
    }



}
