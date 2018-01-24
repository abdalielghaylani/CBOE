using System;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Web.UI;
using System.Diagnostics;
using System.Web;
using System.Configuration;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.COEPickListPickerService;

namespace CambridgeSoft.COE.Framework.GUIShell
{
    /// <summary>
    /// Class that will provide all common functionalities to all the GUIShell Type Applications.
    /// </summary>
    public class GUIShellUtilities
    {
        #region Variables

        public delegate Object CreateNewListElement();

        #endregion

        #region RegEx Const Strings

        private const string numbersRexEx = "[0-9]";

        #endregion

        #region Public Methods

        /// <summary>
        /// Method to encode a string. 
        /// </summary>
        /// <param name="str">String to encode</param>
        /// <returns>Encoded String</returns>
        /// <remarks>This can be user when you don't what to display a string to the client side in plain text</remarks>
        public static string Encode(string str)
        {
            byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(encbuff);
        }

        /// <summary>
        /// Method to decode a string to see it in plain text
        /// </summary>
        /// <param name="str">String to decode</param>
        /// <returns>Plain string</returns>
        /// <remarks>Note that the string must be already encoded to get a plain string</remarks>
        public static string Decode(string str)
        {
            byte[] decbuff = Convert.FromBase64String(str);
            return System.Text.Encoding.UTF8.GetString(decbuff);
        }

        /// <summary>
        /// Method to write messages into the log file (trace.axd)
        /// </summary>
        /// <param name="logCategory">Category of the message</param>
        /// <param name="message">Text to write</param>
        /// <param name="appName">The name of the app trying to write the Log file</param>
        /// <remarks>For different GUIShell based apps, we can write to different log files</remarks>
        public static void WriteToLog(GUIShellTypes.LogsCategories logCategory, string message, string appName)
        {
            if (appName.ToUpper() == GUIShellTypes.Applications.REGISTRATION.ToString()) //Behavior for Reg App
            {
                int adminLogCategory = -1;
                if (ConfigurationUtilities.GetApplicationData(appName.ToUpper()) != null)
                    adminLogCategory = GetAdminLogCategory(appName, "MISC", "LogCategoryMessages"); 
                if (HttpContext.Current.Trace.IsEnabled) 
                    if (((int)logCategory) >= adminLogCategory)
                        HttpContext.Current.Trace.Warn(logCategory.ToString(), message);
            }
            else if (appName.ToUpper() == GUIShellTypes.Applications.MANAGER.ToString()) //Behavior for
            {
                int adminLogCategory = -1;
                if (ConfigurationUtilities.GetApplicationData(appName.ToUpper()) != null)
                    adminLogCategory = GetAdminLogCategory(appName, "MISC", "LogCategoryMessages");
                if (HttpContext.Current.Trace.IsEnabled) 
                    if (((int)logCategory) >= adminLogCategory)
                        HttpContext.Current.Trace.Warn(logCategory.ToString(), message);
            }
        }

        /// <summary>
        /// Gets the log category indicator from configuration
        /// </summary>
        /// <param name="app">App name</param>
        /// <param name="group">Group name</param>
        /// <param name="key">Key name</param>
        /// <returns>Found setting</returns>
        public static int GetAdminLogCategory(string app, string group, string key)
        {
            return !string.IsNullOrEmpty(FrameworkUtils.GetAppConfigSetting(app, group, key)) ? int.Parse(FrameworkUtils.GetAppConfigSetting(app, group, key)) : -1;
        }

        /// <summary>
        /// Here we can clean the string from malicous characters as SQL Inyection, etc...
        /// </summary>
        /// <param name="stringToClean">Dirty string</param>
        /// <returns>Clean string</returns>
        public static string CleanString(string stringToClean)
        {
            return stringToClean; //For now, just return the same string as the original one.
        }

        /// <summary>
        /// Clean all session vars and then call to clean the CSLA identity object.
        /// </summary>
        public static void DoLogout()
        {
            PickListNameValueList.InvalidateCache();
            HttpContext.Current.Response.Redirect("/coemanager/logoff.aspx");
        }

        /// <summary>
        /// Returns the server name
        /// </summary>
        /// <returns>Server name</returns>
        public static string GetServerName()
        {
                return HttpContext.Current.Request.Url.Scheme
                        + "://"
                        + HttpContext.Current.Request.Url.Authority;
        }

        /// <summary>
        /// Get Site name
        /// </summary>
        /// <returns></returns>
        public static string GetSiteName()
        {
            return GetServerName() + HttpContext.Current.Request.ApplicationPath;
        }

        /// <summary>
        /// Gets server URL
        /// </summary>
        /// <returns></returns>
        public static string GetServerURL()
        {
            return GetServerName() + HttpContext.Current.Request.Url.Segments[0] + HttpContext.Current.Request.Url.Segments[1];
        }

        /// <summary>
        /// Gets a list of the themes available for the app.
        /// </summary>
        /// <returns>Foud themes list</returns>
        public static string GetThemes()
        {
            return FrameworkUtils.GetAppConfigSetting(GetApplicationName(), "MISC", GUIShellTypes.Themes);
        }

        /// <summary>
        /// Gets the available browser settings (open a clean window based on the client IE version)
        /// </summary>
        /// <returns>Found string</returns>
        public static string GetPopUpBrowserVersions()
        {
            return FrameworkUtils.GetAppConfigSetting(GetApplicationName(), "MISC", "EnablePopUpForBrowserVersions");
        }

        /// <summary>
        /// Retutns the application name.
        /// </summary>
        /// <returns>App Name</returns>
        public static string GetApplicationName()
        {
            return ConfigurationManager.AppSettings["AppName"] != null ?
                    ConfigurationManager.AppSettings["AppName"].ToUpper() : string.Empty;
        }

        /// <summary>
        /// Returns the default page title
        /// </summary>
        /// <returns>default page title</returns>
        public static string GetDefaultPagesTitle()
        {
            return FrameworkUtils.GetAppConfigSetting(GetApplicationName(), "MISC", "AppPageTitle");
        }

        /// <summary>
        /// Returns a boolean Enables the same pages title.
        /// </summary>
        /// <returns></returns>
        public static bool EnableSamePagesTitle()
        {
            bool retVal = false;
            if(IsConfigurationAvailable())
                bool.TryParse(FrameworkUtils.GetAppConfigSetting(GetApplicationName(), "MISC", "EnableSamePagesTitle"), out retVal);
            return retVal;
        }

        /// <summary>
        /// Determines whether [is configuration available].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is configuration available]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsConfigurationAvailable()
        {
            return ConfigurationManager.AppSettings["ConfigurationName"] != null;
        }

        /// <summary>
        /// Return the url of the search engine, for instace ChemBioViz URL.
        /// </summary>
        /// <returns>A URL</returns>
        public static string GetSearchEngineURL()
        {
            string serverName = HttpContext.Current.Request.Url.Authority;
            string serverNameKey = "{serverName}";
            string protocol = HttpContext.Current.Request.Url.Scheme;
            string protocolKey = "{protocol}";
            string retVal = FrameworkUtils.GetAppConfigSetting(GetApplicationName(), "CBV", "SearchEngineURL");
            if (!string.IsNullOrEmpty(retVal))
            {
                if (retVal.Contains(serverNameKey))
                    retVal = retVal.Replace(serverNameKey, serverName);
                if (retVal.Contains(protocolKey))
                    retVal = retVal.Replace(protocolKey, protocol);
            }
            return retVal;
        }

        /// <summary>
        /// Binds to an object given a binding expression.
        /// </summary>
        /// <param name="bindingExp">The binding expression.</param>
        /// <param name="datasource">The datasource.</param>
        /// <param name="inputvalue">The inputvalue.</param>
        public static void BindToObject(string bindingExp, ref object datasource, object inputvalue)
        {
            //Coverity Bug Fix :- CID : 11462  Jira Id :CBOE-194
            if (!string.IsNullOrEmpty(bindingExp) && datasource != null && inputvalue!=null)
            {
                COEDataBinder dataBinder = new COEDataBinder(datasource);
                dataBinder.SetProperty(bindingExp, inputvalue);
            }
        }

        /// <summary>
        /// Returns the index of the desired element, if found. Otherwise returns -1.
        /// We use indexes instead of (more desirable)Objects because somewhy List.GetIndex(IBusinessObject Element) always returns zero. 
        /// So List.Remove(element) inexorably removes the first element.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idPrimaryKey"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private static int GetListElementIndex(int id, string idPrimaryKey, IList list)
        {
            System.Collections.IEnumerator e = list.GetEnumerator();
            COEDataBinder databinder = new COEDataBinder(null);
            int index = 0;
            while (e.MoveNext())
            {
                databinder.RootObject = e.Current;
                int key = (int)databinder.RetrieveProperty(idPrimaryKey);
                if (key == id)
                    return index;
                index++;
            }
            return -1;
        }

        /// <summary>
        /// Sincronizes a data table with a given object.
        /// Mostly use when client side "objects" are created. E.g Adding a new identifier, a new SaleHistory, etc
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <param name="idPrimaryKey">The primary key (ID) of the datasource</param>
        /// <param name="list">The collection of items ("objects")</param>
        /// <param name="newElementDelegate">Method name to create new objects</param>
        public static void SincronizeListDataTable(DataTable dataTable, string idPrimaryKey, IList list, CreateNewListElement newElementDelegate)
        {
            List<System.Data.DataRow> rowsToDelete = new List<System.Data.DataRow>();
            if (dataTable != null)
            {
                foreach (DataRow currentDataRow in dataTable.Rows)
                {
                    switch (currentDataRow.RowState)
                    {
                        case System.Data.DataRowState.Deleted:
                            int index = -1;
                            if ((index = GetListElementIndex(int.Parse(currentDataRow[idPrimaryKey, DataRowVersion.Original].ToString()), idPrimaryKey, list)) >= 0)
                                list.RemoveAt(index);

                            rowsToDelete.Add(currentDataRow);
                            break;
                        case System.Data.DataRowState.Added:
                            list.Add(newElementDelegate());
                            break;
                        default:
                            if (GetListElementIndex(int.Parse(currentDataRow[idPrimaryKey, DataRowVersion.Original].ToString()), idPrimaryKey, list) < 0)
                                list.Add(newElementDelegate());
                            break;
                    }
                }
                foreach (System.Data.DataRow currentRowToDelete in rowsToDelete)
                    dataTable.Rows.Remove(currentRowToDelete);
            }
        }

        /// <summary>
        /// Find the real key inside of the collection
        /// </summary>
        /// <param name="key">Part of the key</param>
        /// <param name="coll">Collection of keys</param>
        /// <returns>Real key (if found)</returns>
        public static string FindKey(string key, System.Collections.ICollection coll)
        {
            string retVal = key;
            if (!string.IsNullOrEmpty(key) && coll.Count > 0)
            {
                foreach (string currentKey in coll)
                {
                    if (currentKey.Contains(key))
                    {
                        retVal = currentKey;
                        break;
                    }
                }
            }
            return retVal;
        }

        public static string GetCulture()
        {
            string displayCulture = COEConfigurationService.ConfigurationUtilities.GetApplicationData(GetApplicationName()).DisplayCulture;
            if(string.IsNullOrEmpty(displayCulture))
                return string.Empty;
            else
                return displayCulture;
        }

        #endregion

    }
}

