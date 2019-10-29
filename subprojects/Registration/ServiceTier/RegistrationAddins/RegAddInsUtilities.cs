using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using ChemDrawControl19;
using CambridgeSoft.COE.Framework.Caching;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Registration.Services.AddIns;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration.Services.RegistrationAddins.localhost1;

namespace CambridgeSoft.COE.Registration.Services.RegistrationAddins
{
    public class RegAddInsUtilities
    {
        #region Variables
        //Mutex to apply wait on the chemdraw control 
        const string mutexName = "Global\\ChemDrawProcess";
        private static Mutex _chemDrawMutex;

        public static Mutex ChemDrawMutex
        {
            get
            {
                if (_chemDrawMutex == null)
                {
                    Mutex chemDrawProcessMutex = null;
                    bool doesNotExist = false;
                    bool unauthorized = false;
                    bool mutexWasCreated = false;

                    // Attempt to open the named mutex.
                    try
                    {
                        chemDrawProcessMutex = Mutex.OpenExisting(mutexName);
                    }
                    catch (WaitHandleCannotBeOpenedException)
                    {
                        doesNotExist = true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        unauthorized = true;
                    }

                    if (doesNotExist)
                    {
                        var user = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

                        MutexSecurity mSec = new MutexSecurity();

                        MutexAccessRule rule = new MutexAccessRule(user,
                            MutexRights.Synchronize | MutexRights.Modify,
                            AccessControlType.Deny);
                        mSec.AddAccessRule(rule);

                        rule = new MutexAccessRule(user,
                            MutexRights.ReadPermissions | MutexRights.ChangePermissions,
                            AccessControlType.Allow);
                        mSec.AddAccessRule(rule);

                        chemDrawProcessMutex = new Mutex(false, mutexName, out mutexWasCreated, mSec);
                    }
                    else if (unauthorized)
                    {
                        chemDrawProcessMutex = Mutex.OpenExisting(mutexName,
                                MutexRights.ReadPermissions | MutexRights.ChangePermissions);

                        MutexSecurity mSec = chemDrawProcessMutex.GetAccessControl();

                        var user = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

                        MutexAccessRule rule = new MutexAccessRule(user,
                             MutexRights.Synchronize | MutexRights.Modify,
                             AccessControlType.Deny);
                        mSec.RemoveAccessRule(rule);

                        rule = new MutexAccessRule(user,
                            MutexRights.Synchronize | MutexRights.Modify,
                            AccessControlType.Allow);
                        mSec.AddAccessRule(rule);

                        chemDrawProcessMutex.SetAccessControl(mSec);
                        chemDrawProcessMutex = Mutex.OpenExisting(mutexName);
                    }

                    _chemDrawMutex = chemDrawProcessMutex;
                }
                return _chemDrawMutex;
            }
        }

        #endregion

        /// <summary>
        /// Parses a string given a splitter char
        /// </summary>
        /// <param name="input">Long string to be splitted</param>
        /// <param name="splitter">Splitter to apply</param>
        /// <returns>Array of splitted strings</returns>
        public static string[] ParseArray(string input, char[] splitter)
        {
            string[] retVal = { "" };
            if (!string.IsNullOrEmpty(input))
                retVal = input.Split(splitter);
            return retVal;
        }

        /// <summary>
        /// Coverts from DataResult to Dataset (easier binding)
        /// </summary>
        /// <param name="result"></param>
        /// <param name="ds"></param>
        public static void ConvertToDataTable(DataResult result, ref DataSet ds)
        {
            using (StringReader stringReader = new StringReader(result.ResultSet))
                ds.ReadXml(stringReader);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataViewID"></param>
        /// <param name="searchCriteria"></param>
        /// <param name="resultFields"></param>
        /// <returns></returns>
        public static DataResult DoSearch(int dataViewID, string searchCriteria, string[] resultFields)
        {
            DataResult retVal;
            COESearch search = new COESearch(dataViewID);
            SearchInput searchInput = new SearchInput();
            ResultPageInfo resultPageInfo = new ResultPageInfo(0, 100000, 1, 100001);
            searchInput.ReturnPartialResults = false;
            string[] searchFields = new string[] { searchCriteria };
            searchInput.FieldCriteria = searchFields;
            retVal = search.DoSearch(searchInput, resultFields, resultPageInfo);
            return retVal;
        }

        /// <summary>
        /// Format to a correct search expression.
        /// </summary>
        /// <param name="rawCriteria">Full raw criteria</param>
        /// <param name="key">Key to find</param>
        /// <param name="properyName">property name to find in object</param>
        /// <param name="component">datasource</param>
        /// <returns></returns>
        public static string FormatSearchCriteria(string rawCriteria, string key, string properyName, Component component)
        {
            string retVal = string.Empty;
            if (!string.IsNullOrEmpty(rawCriteria) && !string.IsNullOrEmpty(key))
                if (component.Compound.PropertyList[properyName] != null)
                    if (!string.IsNullOrEmpty(component.Compound.PropertyList[properyName].Value))
                        retVal = Regex.Replace(rawCriteria, key, component.Compound.PropertyList[properyName].Value, RegexOptions.IgnoreCase); //TODO: CHANGE this to not hardcode the 0
            return retVal;
        }

        /// <summary>
        /// Format the xml response for duplites handling
        /// </summary>
        /// <param name="dt">DataTable that stores temp results</param>
        /// <param name="registry">DataSource</param>
        /// <returns>Formated clean text</returns>
        public static string FormatResponse(DataTable dt, IRegistryRecord registry)
        {
            string responseTemplate = "<Response message=\"$$MESSAGE$$\"><Error>$$ERROR$$</Error><Result>$$RESULT$$</Result></Response>";
            string message = "Compound Validation OK";
            string result = string.Empty;
            string error = string.Empty;
            int duplicatesCounter = 0;

            if (dt.Rows.Count > 0)
            {
                foreach (Component component in registry.ComponentList)
                {
                    error += "<COMPOUNDLIST>";
                    DataRow[] results = dt.Select("TEMPCOMPOUNDID = '" + component.Compound.ID.ToString() + "'");

                    if (results.Length > 0)
                    {
                        error += "<COMPOUND><TEMPCOMPOUNDID>" + component.Compound.ID.ToString() + "</TEMPCOMPOUNDID><REGISTRYLIST>";
                        foreach (DataRow row in results)
                        {
                            error += "<REGNUMBER count=\"0\" CompoundID=\"" + row["COMPOUNDID"] + "\" SAMEFRAGMENT=\"True\" SAMEEQUIVALENT=\"True\">" + row["REGNUMBER"] + "</REGNUMBER>";
                        }
                        error += "</REGISTRYLIST></COMPOUND>";
                        duplicatesCounter++;
                    }
                }
                error += "</COMPOUNDLIST>";
                message = string.Format("{0} duplicated component(s)", duplicatesCounter.ToString());
            }

            string retVal = responseTemplate.Replace("$$MESSAGE$$", message);
            retVal = retVal.Replace("$$ERROR$$", error);
            retVal = retVal.Replace("$$RESULT$$", result);

            return retVal;
        }

        /// <summary>
        /// Method to verify the chemdraw version in the server.
        /// </summary>
        /// <param name="ctrl">Control to verify</param>
        /// <returns>Boolean indicating whether is supported or not</returns>
        public static bool IsASupportedChemDrawVersion(ref ChemDrawCtl ctrl)
        {
            return ctrl.Version.ToUpper().Contains("PRO");
        }

        public static Service GetPythonWebService(string url)
        {
            Service pythonWebService = null;

            string key = "PythonWebService" + url;

            if (AppDomain.CurrentDomain.GetData(key) == null) //No object found, so let's create it.
            {
                pythonWebService = new Service();
                pythonWebService.Url = url;

                AppDomain.CurrentDomain.SetData(key, pythonWebService); //Save the obj into the App Domain
            }
            return (Service)AppDomain.CurrentDomain.GetData(key);
        }

        /// <summary>
        /// Clears and set the given value to the ChemDraw control
        /// </summary>
        /// <param name="ctrl">Control</param>
        /// <param name="base64">base64 value</param>
        public static void SetDataStructure(ref ChemDrawCtl ctrl, string base64)
        {
            ctrl.Objects.Clear();
            ctrl.DataEncoded = true;
            ctrl.Objects.set_Data("chemical/x-cdx", null, null, null, UnicodeEncoding.ASCII.GetBytes(base64));
        }


        /// <summary>
        /// Determines if the add in should be disable for the current record based on a value in a propertylist
        /// </summary>
        /// <param name="registryRecord">the registration record</param>
        /// <param name="propertyListType">level of registry record can be  Component or Structure or Batch</param>
        /// <param name="propertyName">propery name in property list</param>
        /// <param name="disableValueList">string array of values indicating to disable</param>
        /// <returns></returns>
        public static bool DisableAddIn(IRegistryRecord registryRecord, PropertyListType propertyListType, string propertyName, string[] disableValueList)
        {

            bool disableAddIn = false;
            switch (propertyListType)
            {
                case PropertyListType.Batch:

                    string batchPropertyValue = registryRecord.BatchList[0].PropertyList[propertyName].Value.ToString().Trim().ToLower();
                    for (int i = 0; i < disableValueList.Length; i++)
                    {
                        if (batchPropertyValue == disableValueList[i].ToString().Trim().ToLower())
                        {
                            disableAddIn = true;
                        }
                    }

                    break;

                case PropertyListType.Component:

                    string componentPropertyValue = registryRecord.ComponentList[0].Compound.PropertyList[propertyName].Value.ToString();
                    for (int i = 0; i < disableValueList.Length; i++)
                    {

                        if (componentPropertyValue == disableValueList[i].ToString().Trim().ToLower())
                        {
                            disableAddIn = true;
                        }

                    }
                    break;
                case PropertyListType.Structure:

                    string structurePropertyValue = registryRecord.ComponentList[0].Compound.BaseFragment.Structure.PropertyList[propertyName].Value.ToString();
                    for (int i = 0; i < disableValueList.Length; i++)
                    {

                        if (structurePropertyValue == disableValueList[i].ToString().Trim().ToLower())
                        {
                            disableAddIn = true;
                        }

                    }
                    break;
                case PropertyListType.NotSet:
                    //by default, addIn will be run
                    disableAddIn = false;
                    break;

            }

            return disableAddIn;
        }


        /// <summary>
        /// level of registry record for propertylist
        /// </summary>
        public enum PropertyListType
        {
            Batch,
            Component,
            Structure,
            NotSet,
        }


        /// <summary>
        /// Method to apply wait for chemdraw to complete the process.
        /// </summary>
        public static void ChemDrawWaitOnce()
        {
            ChemDrawMutex.WaitOne();
        }

        /// <summary>
        /// Method to release the mutex associated with chemdrw process.
        /// </summary>
        public static void ChemDrawReleaseMutex()
        {
            ChemDrawMutex.ReleaseMutex();
        }

    }

}
