using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Text;
using System.Web;
using System.Linq;

using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Services.Types;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Caching;

using ChemDrawControl18;

using CambridgeSoft.COE.Framework.COEPickListPickerService;
using CambridgeSoft.COE.Framework.Common.Exceptions;
using CambridgeSoft.COE.Framework.COESecurityService;

using Csla;
using Csla.Validation;
using CambridgeSoft.COE.Registration.Access;

namespace CambridgeSoft.COE.Registration.Services.Common
{
    /// <summary>
    /// Utilities
    /// </summary>
    public static class RegSvcUtilities
    {

        /// <summary>
        /// By default DataSet.GetXML gets DateTime and Date in format of CCYY-MM-DDThh:mm:ss and CCYY-MM-DD,
        /// which can not be parse by Oracle. Need to reformat them to be recognized by Oracle.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string ReformatXMLDate(string xml)
        {
            string ret = "";
            using (StringWriter stringWriter = new StringWriter()) //Coverity fix - CID 11847
            {
                using (XmlTextReader xmlTextReader = new XmlTextReader(xml, XmlNodeType.Element, null))
                {
                    using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                    {
                        xmlTextWriter.Indentation = 4;
                        xmlTextWriter.WriteStartDocument();
                        string elementName = "";


                        while (xmlTextReader.Read())
                        {
                            switch (xmlTextReader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    xmlTextWriter.WriteStartElement(xmlTextReader.Name);
                                    elementName = xmlTextReader.Name;
                                    break;
                                case XmlNodeType.Text:
                                    if ((elementName.ToUpper() == "CREATION_DATE") || (elementName.ToUpper() == "ENTRY_DATE") || (elementName.ToUpper() == "LAST_MOD_DATE"))
                                    {
                                        //The DateTime can be changed to any format as MM-dd-yyyy or dd-MM-yyyy or .ToShortDateString() ...
                                        //DateTime dateTime = DateTime.Parse(xmlTextReader.Value.ToString());
                                        //xmlTextWriter.WriteString(dateTime.ToString("MM-dd-yyyy"));
                                        xmlTextWriter.WriteString(XmlConvert.ToDateTime(xmlTextReader.Value, XmlDateTimeSerializationMode.Local).ToString());
                                    }
                                    else
                                        xmlTextWriter.WriteString(xmlTextReader.Value);
                                    break;
                                case XmlNodeType.EndElement:
                                    xmlTextWriter.WriteEndElement();
                                    break;
                            }
                        }
                        ret = stringWriter.ToString();
                        return ret;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the assembly File version attribute.
        /// </summary>
        /// <returns>File version text</returns>
        public static string GetFileVersion()
        {
            string retVal = "Uknown";
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            if (assembly != null)
            {
                FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
                if (fileVersion != null)
                    retVal = fileVersion.FileVersion;
            }
            return retVal;
        }

        /// <summary>
        /// Converts the current structural representation into CDX format.
        /// </summary>
        /// <param name="originalStructure">
        /// a Structure business object, the Value property of which represents the underlying
        /// structural representation
        /// </param>
        internal static void ConvertStructureFormatToCdx(Structure originalStructure)
        {
            if (!originalStructure.IsTemporary)
                return;

            ChemDrawCtl ctrl = new ChemDrawCtl();
            if (!ctrl.Version.ToUpper().Contains("PRO"))
                throw new ValidationException(
                    string.Format("ChemDraw control version not supported in server side. Server version: {0} (required a registered Ultra version)", ctrl.Version)
                );

            if (!string.IsNullOrEmpty(originalStructure.Value) && !originalStructure.Value.StartsWith("VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACA"))
            {
                string currentMimeType = string.Empty;

                //TODO: Assess the MIME value dynamically via the Structure.Type
                if (!string.IsNullOrEmpty(originalStructure.Format))
                    currentMimeType = originalStructure.Format;
                else
                    currentMimeType = "chemical/x-cdx";

                ctrl.Objects.Clear();
                ctrl.DataEncoded = true;
                ctrl.Objects.set_Data(
                    currentMimeType, null, null, null, UnicodeEncoding.ASCII.GetBytes(originalStructure.Value)
                );

                string cdx = ctrl.get_Data("cdx").ToString();
                originalStructure.Value = cdx;
            }
        }

        /// <summary>
        /// Fetches the value of the associated Property by its name. If none is found,
        /// returns null.
        /// </summary>
        /// <param name="properties">the PropertyList to search</param>
        /// <param name="propertyKey">the name of the Property</param>
        /// <returns>the matched Property's value</returns>
        internal static string GetPropertyValue(PropertyList properties, string propertyKey)
        {
            string valueToCompare = null;
            int propertyIndex = properties.GetPropertyIndex(propertyKey);
            if (propertyIndex > -1)
            {
                string rawValue = properties[propertyIndex].Value;
                if (!string.IsNullOrEmpty(rawValue))
                    valueToCompare = rawValue;
            }
            return valueToCompare;
        }

        /// <summary>
        /// Fetches the value of the first matching Identifier, by its name. If none is found,
        /// returns null.
        /// </summary>
        /// <param name="identifiers">the IdentifierList to search</param>
        /// <param name="identifierKey">the name of the Identifier</param>
        /// <returns>the matched identifier's value</returns>
        internal static string GetIdentifierValue(IdentifierList identifiers, string identifierKey)
        {
            string valueToCompare = null;
            foreach (Identifier identifier in identifiers)
            {
                if (identifier.IdentifierID != 0 && string.IsNullOrEmpty(identifier.Name))
                {
                    string identifierCacheKey = "identifiers";
                    IdentifierList lookups =
                        (IdentifierList)AppDomain.CurrentDomain.GetData(identifierCacheKey);
                    if (lookups == null)
                    {
                        lookups = IdentifierList.GetIdentifierList();
                        AppDomain.CurrentDomain.SetData(identifierCacheKey, lookups);
                    }

                    foreach (DictionaryEntry item in lookups.KeyValueList)
                    {
                        if (identifier.IdentifierID.ToString() == item.Key.ToString())
                        {
                            identifier.Name = item.Value.ToString();
                            break;
                        }
                    }
                }

                if (identifier.Name.ToUpper() == identifierKey.ToUpper())
                {
                    valueToCompare = identifier.InputText;
                    break;
                }
            }
            return valueToCompare;
        }

        /// <summary>
        /// Using the current Registration System Settings, determines the number and nature of
        /// pre-registered components which match either the structure, key property, or key identifier
        /// of this registrations' components.
        /// </summary>
        /// <param name="record">teh RegistryRecord containing the components to search for</param>
        /// <returns>a list of <see cref="DuplicateCheckResponse">response objects</see>,
        /// one response object per component evaluated
        /// </returns>
        internal static List<DuplicateCheckResponse>
            FindDuplicates(RegistryRecord record)
        {
            List<DuplicateCheckResponse> responses = new List<DuplicateCheckResponse>();
            if (GetCheckDuplicationSetting())
            {


                foreach (Component component in record.ComponentList)
                {
                    KeyValuePair<PreloadDupCheckMechanism, string> dupCheckMechanism;
                    string valueToCompare = null;

                    Structure structure = component.Compound.BaseFragment.Structure;
                    // Do structure based duplicate checking according to CheckDuplication system setting
                    if (structure.DrawingType == DrawingType.Chemical)
                    {
                        
                            string paramsToApply = RegSvcUtilities.GetCartridgeStructureMatchSettings();
                            dupCheckMechanism = new KeyValuePair<PreloadDupCheckMechanism, string>(
                                PreloadDupCheckMechanism.Structure, paramsToApply
                                );
                        
                        if ((structure.IsDirty && !record.IsNew) || structure.IsNew)
                        {
                            valueToCompare = component.Compound.BaseFragment.Structure.Value;
                        }
                        else
                        {
                            valueToCompare = String.Empty;
                        }
                    }
                    else
                    {
                        dupCheckMechanism = RegSvcUtilities.GetNonStructuralDuplicateCheckSettings();
                        switch (dupCheckMechanism.Key)
                        {
                            case PreloadDupCheckMechanism.StructureProperty:
                            case PreloadDupCheckMechanism.ComponentProperty:
                                {
                                    //get the value for the associated property
                                    PropertyList properties = null;
                                    if (dupCheckMechanism.Key == PreloadDupCheckMechanism.ComponentProperty)
                                    {
                                        properties = component.Compound.PropertyList;
                                    }
                                    else
                                    {
                                        properties = component.Compound.BaseFragment.Structure.PropertyList;
                                    }
                                    //structure.IsBeingRegistered helps to check temporary records after submit later send to  registration .
                                    //#Note [Why] : When user trys to edit temporary record its structure is neither IsNew Nor IsDirty so custom duplicate addin is getting failed.
                                    if (structure.IsDirty || structure.IsNew || (structure.IsBeingRegistered && structure.IsTemporary))
                                    {
                                        valueToCompare = GetPropertyValue(properties, dupCheckMechanism.Value);
                                    }
                                    else
                                    {
                                        valueToCompare = string.Empty;
                                    }

                                    break;
                                }
                            case PreloadDupCheckMechanism.StructureIdentifier:
                            case PreloadDupCheckMechanism.ComponentIdentifier:
                                {
                                    //get the first value for the associated identifier
                                    IdentifierList identifiers = null;
                                    if (dupCheckMechanism.Key == PreloadDupCheckMechanism.ComponentIdentifier)
                                    {
                                        identifiers = component.Compound.IdentifierList;
                                    }
                                    else
                                    {
                                        identifiers = component.Compound.BaseFragment.Structure.IdentifierList;
                                    }
                                    //structure.IsBeingRegistered helps to check temporary records after submit later send to  registration .
                                    //#Note [Why] : When user trys to edit temporary record its structure is neither IsNew Nor IsDirty so custom duplicate addin is getting failed.
                                    if (structure.IsDirty || structure.IsNew || (structure.IsBeingRegistered && structure.IsTemporary))
                                    {
                                        valueToCompare = GetIdentifierValue(identifiers, dupCheckMechanism.Value);
                                    }
                                    else
                                    {
                                        valueToCompare = string.Empty;
                                    }
                                    break;
                                }
                            default:
                                {
                                    //do nothing!
                                    break;
                                }
                        }
                    }

                    // only perform the duplicate-checing if it is configured properly
                    if (dupCheckMechanism.Key != PreloadDupCheckMechanism.None)
                    {
                        if (!string.IsNullOrEmpty(dupCheckMechanism.Value))
                        {
                            if (!string.IsNullOrEmpty(valueToCompare))
                            {
                                string checkType = null;
                                switch (dupCheckMechanism.Key)
                                {
                                    case PreloadDupCheckMechanism.Structure: checkType = "S"; break;
                                    case PreloadDupCheckMechanism.ComponentProperty: checkType = "P"; break;
                                    case PreloadDupCheckMechanism.ComponentIdentifier: checkType = "I"; break;
                                    case PreloadDupCheckMechanism.StructureProperty: checkType = "SP"; break;
                                    case PreloadDupCheckMechanism.StructureIdentifier: checkType = "SI"; break;
                                }

                                RegistrationOracleDAL dal = null;
                                DalUtils.GetRegistrationDAL(ref dal, Constants.SERVICENAME);
                                string fragmentListToCompare = component.Compound.FragmentList.ToString();
                                Dictionary<string, float> dictFragments = new Dictionary<string, float>();
                                foreach (BatchComponentFragment bcfragment in record.BatchList[0].BatchComponentList[0].BatchComponentFragmentList)
                                {
                                    if (bcfragment.FragmentID != 0 && !dictFragments.ContainsKey(bcfragment.FragmentID.ToString()))
                                        dictFragments.Add(bcfragment.FragmentID.ToString(), bcfragment.Equivalents);
                                }
                                if (record.DataStrategy != RegistryRecord.DataAccessStrategy.Atomic) //Coverity fix- CID 18793 
                                    dal.UseBulkLoadStrategy(30);
                                string rawResponse = dal.CheckForDuplicates(checkType, dupCheckMechanism.Value, valueToCompare, fragmentListToCompare);

                                DuplicateCheckResponse response = DuplicateCheckResponse.GetResponse(rawResponse);
                                foreach (DuplicateCheckResponse.MatchedRegistration regRes in response.MatchedRegistrations)
                                {
                                    if (regRes.MatchedComponents[0].SameFragments == "True" && dictFragments.Count>0)
                                        regRes.MatchedComponents[0].SameEquivalents = dal.CheckForFragmentEquivalents(regRes.MatchedComponents[0].Id, dictFragments);
                                    else
                                        regRes.MatchedComponents[0].SameEquivalents = regRes.MatchedComponents[0].SameFragments;
                                }
                                responses.Add(response);
                                responses = RemoveSharedStructureRecords(responses, component);
                            }
                        }
                    }

                    // return a new/empty DuplicateCheckResponse object for by-passed components
                    if (responses.Count == 0)
                        responses.Add(new DuplicateCheckResponse());
                }



            }
            return responses;
        }

        /// <summary>
        /// This is a call from AddIn and should not contains Shared Structure components in Duplicate response,Performs delete mechanism on Duplicate response when PreloadDupCheckMechanism is on structure
        /// </summary>
        /// <param name="responses">DuplicateCheckResponse</param>
        /// <param name="component">Current record component</param>
        /// <returns>a list of <see cref="DuplicateCheckResponse">Cleaned objects</see>,
        /// one response object per component evaluated
        /// </returns>
        private static List<DuplicateCheckResponse> RemoveSharedStructureRecords(List<DuplicateCheckResponse> responses, Component component)
        {
            try
            {
                foreach (var response in responses)
                {
                    response.MatchedRegistrations.RemoveAll(qRegRecord => qRegRecord.MatchedComponents.FindAll(qComponents => qComponents.Id == component.Compound.ID).Count > 0 || qRegRecord.MatchedComponents.FindIndex(qComponents => qComponents.Id != component.Compound.ID && (qComponents.MatchedStructures != null && qComponents.MatchedStructures.Count > 0) && qComponents.MatchedStructures.FindIndex(qStructures => qStructures.Id == component.Compound.BaseFragment.Structure.ID) != -1) != -1);

                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex, COEExceptionDispatcher.Policy.LOG_ONLY);
            }
            return responses;
        }

        #region [ Logging helpers ]

        /// <summary>
        /// Provides exception logging to a pre-configured sink (typically a text file).
        /// </summary>
        /// <param name="message">an error message</param>
        /// <param name="prioroty">
        /// the logging level that, if lower than the configuration's "priority" value, allows the entry to be written
        /// </param>
        /// <param name="exception">a System.Exception instance, or null</param>
        public static void LogError(string message, int priority, Exception exception)
        {
            COELog coeLog = COELog.GetSingleton("Registration");
            string msg = message;
            if (exception != null)
                msg = message + " | " + exception.ToString().Trim();
            coeLog.Log(msg, priority, SourceLevels.Error);
        }

        /// <summary>
        /// Provides an event-logging analog using a pre-configured sink (usually a text file).
        /// </summary>
        /// <remarks>
        /// For convenience, the EventLogEntryType is converted into a SourceLevel via a separate
        /// translation method. <seealso cref="TranslateLogEntryType"/>
        /// </remarks>
        /// <param name="message">any message, verbose or otherwise</param>
        /// <param name="prioroty">
        /// the logging level that, if lower than the configuration's "priority" value, allows the entry to be written
        /// </param>
        /// <param name="entryType">a EventLogEntryType enumeration member</param>
        public static void LogOther(string message, int priority, EventLogEntryType entryType)
        {
            COELog coeLog = COELog.GetSingleton("Registration");
            string msg = message;
            SourceLevels level = TranslateLogEntryType(entryType);

            if (level == SourceLevels.Error)
                LogError(msg, priority, null);
            else
                coeLog.Log(msg, priority, level);
        }

        /// <summary>
        /// Converts event log entry types into best-matches for SourceLevels.
        /// </summary>
        /// <param name="entryType">an EventLogEntryType</param>
        /// <returns></returns>
        private static SourceLevels TranslateLogEntryType(EventLogEntryType entryType)
        {
            SourceLevels level = SourceLevels.Off;
            switch (entryType)
            {
                case EventLogEntryType.FailureAudit:
                case EventLogEntryType.Error:
                    level = SourceLevels.Error; break;
                case EventLogEntryType.Information:
                    level = SourceLevels.Information; break;
                case EventLogEntryType.SuccessAudit:
                    level = SourceLevels.Information; break;
                case EventLogEntryType.Warning:
                    level = SourceLevels.Warning; break;
            }
            return level;
        }

        #endregion

        #region [ System Settings ]

        private const string LEVEL = "Level";
        private const string TYPE = "Type";
        private const string VALUE = "Value";        

        /// <summary>
        /// Gets a collection of SettingsGroup objects.
        /// </summary>
        /// <returns></returns>
        private static NamedElementCollection<SettingsGroup>
            GetSystemSettingGroup()
        {
            AppSettingsData appSettingData =
                FrameworkUtils.GetAppConfigSettings("REGISTRATION", "Registration");

            NamedElementCollection<SettingsGroup> groupCollection = appSettingData.SettingsGroup;
            return groupCollection;
        }

        /// <summary>
        /// Gets an individual SettingsGroup based on its name
        /// </summary>
        /// <param name="sectionName">the name of the section to retrieve</param>
        /// <returns></returns>
        public static NamedElementCollection<AppSetting>
            GetSystemSettings(string sectionName)
        {
            NamedElementCollection<SettingsGroup> groupCollection = GetSystemSettingGroup();
            NamedElementCollection<AppSetting> settings = groupCollection.Get(sectionName).Settings;
            return settings;
        }

        /// <summary>
        /// Returns true if the unregistered components allowed in mixtures workflow was enabled by configuration
        /// </summary>
        /// <returns></returns>
        public static bool GetAllowUnregisteredComponents()
        {
            string item = GetSystemSetting("REGADMIN", "AllowUnregisteredComponentsInMixtures");
            return !string.IsNullOrEmpty(item) && item.ToLower() == "true";
        }

        /// <summary>
        /// Returns true if EnableBatchPrefix is enabled 
        /// </summary>
        /// <returns></returns>
        public static bool GetEnableBatchPrefix()
        {
            string item = string.Empty;
            try
            {
                item = GetSystemSetting("REGADMIN", "EnableUseBatchPrefixes");
            }
            catch (Exception ex)
            {
                item = "false";
            }
            return !string.IsNullOrEmpty(item) && item.ToLower() == "true";
        }

        /// <summary>
        /// Returns true the default batch prefix to use for non-UI loading when EnableBatchPrefix is enabled 
        /// </summary>
        /// <returns></returns>
        public static int GetDefaultBatchPrefix()
        {
            string item = GetSystemSetting("REGADMIN", "DefaultBatchPrefix");
            if (!string.IsNullOrEmpty(item) && System.Convert.ToInt16(item) > 0)
            {
                return System.Convert.ToInt16(item);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Retrieves a single System Setting's string value.
        /// </summary>
        /// <param name="sectionName">
        /// the internal name of the AppSetting section that the setting belongs to
        /// </param>
        /// <param name="settingName">
        /// the internal name of the individual setting
        /// </param>
        /// <returns>an individual setting's value string</returns>
        public static string GetSystemSetting(string sectionName, string settingName)
        {
            string result = string.Empty;
            NamedElementCollection<AppSetting> settings = GetSystemSettings(sectionName);

            for (int i = 0; i < settings.Count; i++)
            {
                AppSetting setting = settings.Get(i);
                if (setting.Name.Equals(settingName, StringComparison.OrdinalIgnoreCase))
                {
                    result = setting.Value;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Obtains a comma-delimited representation of duplicate-checking parameters used by CSCartridge.
        /// </summa
        /// <returns>
        /// a string representing the combined settings that the CSCartridge uses to determine
        /// structure similarity (for 'MolContains'-based duplicate-checking)
        /// </returns>
        public static string GetCartridgeStructureMatchSettings()
        {
            string result = string.Empty;
            NamedElementCollection<AppSetting> settings = GetSystemSettings("DUPLICATE_CHECKING");

            for (int i = 0; i < settings.Count; i++)
            {
                AppSetting setting = settings.Get(i);
                result += setting.Name + "=" + setting.Value;
                if (i != settings.Count - 1)
                    result += ",";
            }
            return result;
        }

        /// <summary>
        /// Fetches the non-structural duplicate-checking parameters
        /// </summary>
        /// <returns>
        /// a combination of the 'level' and 'type' of checks to perform (coalesced into an ENUM value)
        /// , and the name of the custom property or identifier to use to extract the comparison value
        /// </returns>
        public static KeyValuePair<PreloadDupCheckMechanism, string>
            GetNonStructuralDuplicateCheckSettings()
        {
            NamedElementCollection<AppSetting> settings = GetSystemSettings("ENHANCED_DUPLICATE_SCAN");

            string level = null;
            string type = null;
            string value = null;
            KeyValuePair<PreloadDupCheckMechanism, string> result
                = new KeyValuePair<PreloadDupCheckMechanism, string>(PreloadDupCheckMechanism.None, null);

            if (settings.Contains(LEVEL))
                level = settings.Get(LEVEL).Value;

            if (settings.Contains(TYPE))
                type = settings.Get(TYPE).Value;

            if (settings.Contains(VALUE))
                value = settings.Get(VALUE).Value;

            if (!string.IsNullOrEmpty(level) && !string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(value))
            {
                string rawMechanism = level + type;
                PreloadDupCheckMechanism mechanism =
                    (PreloadDupCheckMechanism)Enum.Parse(typeof(PreloadDupCheckMechanism), rawMechanism, true);
                result = new KeyValuePair<PreloadDupCheckMechanism, string>(mechanism, value);
            }

            return result;
        }

        /// <summary>
        /// Fetches the group and key parameters
        /// </summary>
        /// <returns>
        /// value of the key
        /// </returns>
        public static string
            GetSystemSettings(string group, string key)
        {
            NamedElementCollection<AppSetting> settings = GetSystemSettings(group);
            return settings.Get(key).Value;            
        }

        /// <summary>
        /// Obtains the CheckDuplication system setting from the REGADMIN settings group
        /// </summa
        /// <returns>
        /// a Boolean value representing whether to perform a sturctural duplicate check
        /// </returns>
        public static bool GetCheckDuplicationSetting()
        {
            bool CheckDuplication = false;
            NamedElementCollection<AppSetting> settings = GetSystemSettings("REGADMIN");

            bool.TryParse(settings.Get("CheckDuplication").Value, out CheckDuplication);
            return CheckDuplication;
        }


        #endregion

        #region [ RegistryRecord ]

        /// <summary>
        /// Provides rich data for Fragment information given the fragment list
        /// </summary>
        /// <param name="record"></param>
        internal static void UpdateFragments(
            RegistryRecord record
            )
        {

            //Back-fill each Compound.FragmentList from its corresponding BatchComponentFragmentList
            foreach (Batch b in record.BatchList)
            {
                foreach (BatchComponent bc in b.BatchComponentList)
                {
                    foreach (BatchComponentFragment bcf in bc.BatchComponentFragmentList)
                    {
                        Component c = record.ComponentList.GetComponentByIndex(bc.ComponentIndex);
                        c = (c == null ? record.ComponentList.GetComponentByRegNumber(bc.RegNum) : c);
                        CompoundFragment cf = c.Compound.CompoundFragmentList.GetById(bcf.CompoundFragmentID);

                        if (cf == null || cf.ID == 0)
                        {
                            Fragment mf = c.Compound.FragmentList.GetByID(bcf.FragmentID);
                            if (mf == null)
                            {
                                //add a fragment to the list
                                // NOTE: the 'FragmentList' needs to be replaced by the 'CompoundFragmentList' eventually
                                c.Compound.CompoundFragmentList.Add(CompoundFragment.NewCompoundFragment(bcf.FragmentID));
                                c.Compound.FragmentList.Add(
                                    Fragment.NewFragment(bcf.FragmentID, null, null, 0, null, 0, null)
                                );
                            }
                        }
                        else
                        {
                            //update one
                            if (bcf.IsDirty)
                            {
                                cf.Fragment = Fragment.NewFragment(bcf.FragmentID, null, null, 0, null, 0, null);
                                cf.Fragment.CompoundFragmentId = cf.ID;

                                foreach (Fragment f in c.Compound.FragmentList)
                                {
                                    if (f.CompoundFragmentId == cf.ID)
                                    {
                                        f.FragmentID = bcf.FragmentID;
                                        f.CompoundFragmentId = cf.ID;
                                        f.Code = null;
                                        f.Description = null;
                                        f.Formula = null;
                                        f.FragmentTypeId = 0;
                                        f.MW = null;
                                    }
                                }
                            }
                        }
                    }
                }
            }


            //the delete is a bit more of a problem...
            //...eliminate any fragments from the compound-fragments list that are no longer in use
            //...oh, and leave those alone that don't have a CompoundFragmentID of 0!
            foreach (Component c in record.ComponentList)
            {
                Dictionary<int, int> fragmentUsage = new Dictionary<int, int>();
                foreach (Fragment f in c.Compound.FragmentList)
                {
                    if (!fragmentUsage.ContainsKey(f.FragmentID))
                        fragmentUsage.Add(f.FragmentID, f.IsDirty ? 0 : 1);
                    //if (f.CompoundFragmentId != 0 && f.IsDeleted == false)
                    //    fragmentUsage[f.FragmentID] += 1;
                }

                foreach (Batch b in record.BatchList)
                {
                    foreach (BatchComponent bc in b.BatchComponentList)
                    {
                        if (bc.ComponentIndex == c.ComponentIndex)
                        {
                            foreach (BatchComponentFragment bcf in bc.BatchComponentFragmentList)
                            {
                                if (bcf.IsDeleted)
                                    fragmentUsage[bcf.FragmentID] -= 1;
                                if (fragmentUsage.ContainsKey(bcf.FragmentID))
                                    fragmentUsage[bcf.FragmentID] += 1;
                            }
                        }
                    }
                }

                foreach (KeyValuePair<int, int> kvp in fragmentUsage)
                {
                    if (kvp.Value <= 0)
                    {
                        Fragment f = c.Compound.FragmentList.GetByID(kvp.Key);
                        if (f != null)
                            c.Compound.FragmentList.Remove(f);
                    }
                }
            }

            //For each Fragment in each component's master list that has only a Fragment ID,
            //  fetch the complete Fragment details using that Fragment ID
            // JED: Some downstream processes are expecting to have the full complement of information even
            foreach (Component c in record.ComponentList)
            {
                CompoundFragmentList cfl = c.Compound.CompoundFragmentList;
                FragmentList fl = c.Compound.FragmentList;
                if (cfl.Count > 0)
                {
                    List<int> fragList = new List<int>();
                    foreach (CompoundFragment cf in cfl)
                    {
                        //JED: this detailed information isn't available under any circumstances
                        if (cf.Fragment.FragmentID != 0 && cf.Fragment.FragmentTypeId == 0)
                            fragList.Add(cf.Fragment.FragmentID);
                    }

                    if (fragList.Count > 0)
                    {
                        FragmentList newFrags = FragmentList.GetFragmentList(fragList);
                        foreach (CompoundFragment cf in cfl)
                        {
                            Fragment matchedFrag = newFrags.GetByID(cf.Fragment.FragmentID);
                            if (matchedFrag != null)
                            {
                                cf.Fragment = matchedFrag;
                                if (!fl.Contains(matchedFrag))
                                    fl.Add(matchedFrag);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds a fragment (typcially a salt or solvate) to a specific batch component
        /// (and subsequently adds a component-level fragment as well).
        /// </summary>
        /// <param name="record">the RegistryRecord containing the batch component and component to update</param>
        /// <param name="componentIndex">the index of the component to receive the fragment</param>
        /// <param name="fragmentToAdd">the fragment itself</param>
        /// <param name="equivalents">the fragment:parent fragment (the base compound) ratio</param>
        internal static void AddFragment(
            RegistryRecord record
            , int componentIndex
            , Fragment fragmentToAdd
            , float equivalents
            )
        {
            Component parentComponent = record.ComponentList.GetComponentByIndex(componentIndex);
            if (parentComponent == null)
                throw new ArgumentOutOfRangeException(
                    "componentIndex", componentIndex, "The indexed component could not be found");

            if (equivalents == 0)
                throw new ArgumentOutOfRangeException(
                    "equivalents", equivalents, "The fragment equivalents must be a value greater than zero");

            if (fragmentToAdd == null)
                throw new ArgumentNullException("fragmentToAdd", "A fragment instance is required by this operation");
            else
            {
                //add the fragment to the appropriate BatchComponentFragmentList
                foreach (Batch batch in record.BatchList)
                {
                    foreach (BatchComponent batchComp in batch.BatchComponentList)
                    {
                        if (batchComp.ComponentIndex == parentComponent.ComponentIndex)
                        {
                            BatchComponentFragmentList bcfList = batchComp.BatchComponentFragmentList;
                            BatchComponentFragment item =
                                BatchComponentFragment.NewBatchComponentFragment(fragmentToAdd, equivalents);

                            //replace any place-holder fragment
                            if (bcfList.Count == 1 && bcfList[0].FragmentID == 0)
                                bcfList.Clear();
                            bcfList.Add(item);
                            break;
                        }
                    }
                }

                //back-fill the component's master fragments list as necessary
                foreach (Batch batch in record.BatchList)
                {
                    foreach (BatchComponent batchComp in batch.BatchComponentList)
                    {
                        if (batchComp.ComponentIndex == parentComponent.ComponentIndex)
                        {
                            FragmentList frags = parentComponent.Compound.FragmentList;

                            //Copy fragment information to the Component if not already present
                            foreach (BatchComponentFragment batchCompFrag in batchComp.BatchComponentFragmentList)
                            {
                                if (frags.GetByID(batchCompFrag.FragmentID) == null)
                                {
                                    Fragment theNewFragment =
                                        Fragment.NewFragment(batchCompFrag.FragmentID, batchCompFrag.FragmentID.ToString(), null, 0, null, 0, null);
                                    frags.Add(theNewFragment);
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Provides a 'default' structure value for a specific component.
        /// </summary>
        /// <remarks>This will not override an existing structure.</remarks>
        /// <param name="record">the RegistryRecord containing the component to update</param>
        /// <param name="componentIndex">the index of the component to receive the fragment</param>
        /// <param name="structureId">the structure's ID value</param>
        internal static void AssignDefaultStructureById(
            RegistryRecord record
            , int componentIndex
            , int structureId
            )
        {
            Component parentComponent = record.ComponentList.GetComponentByIndex(componentIndex);
            if (parentComponent == null)
                throw new ArgumentOutOfRangeException(
                    "componentIndex", componentIndex, "The indexed component could not be found");

            if (string.IsNullOrEmpty(parentComponent.Compound.BaseFragment.Structure.Value))
            {
                if (structureId < 0)
                {
                    //for 'no-structure' placeholders
                    parentComponent.Compound.BaseFragment.Structure.ID = structureId;
                }
                else
                {
                    Structure defaultStructure = Structure.GetStructure(structureId, true);
                    if (defaultStructure == null)
                        throw new ArgumentNullException("structureId", "A valid structure instance is required by this operation");

                    parentComponent.Compound.BaseFragment.Structure.ID = defaultStructure.ID;
                    parentComponent.Compound.BaseFragment.Structure.Value = defaultStructure.Value;
                }
            }
        }

        /// <summary>
        /// Generates an Identifier instance in the context of the containing IdentifierList
        /// </summary>
        /// <param name="container">the identifier list to receive the new instance</param>
        /// <param name="identifier">the Identifier object to add</param>
        /// <param name="scope">the scope across which this Identifier is expected to exist (allows for validation)</param>
        internal static void CreateNewIdentifier(
            IdentifierList container
            , Identifier identifier
            , IdentifierTypeEnum scope
            )
        {
            IdentifierList idList = IdentifierList.GetIdentifierListByType(scope, true, true);
            if (idList.KeyValueList.Contains(identifier.IdentifierID) && identifier.IsValid)
                container.Add(identifier);
        }

        /// <summary>
        /// Generates an Identifier instance in the context of the containing IdentifierList
        /// </summary>
        /// <param name="identifierId"></param>
        /// <param name="identifierValue"></param>
        /// <param name="container"></param>
        /// <param name="scope"></param>
        internal static void CreateNewIdentifier(
            int identifierId
            , string identifierValue
            , IdentifierList container
            , IdentifierTypeEnum scope
            )
        {
            IdentifierList idList = IdentifierList.GetIdentifierListByType(scope, true, true);
            foreach (Identifier item in idList)
            {
                if (identifierId == item.IdentifierID)
                {
                    if (item.IdentifierID.Equals(identifierId))
                    {
                        Identifier idf =
                            Identifier.NewIdentifier(0, item.IdentifierID, item.Name, item.Description, true);
                        idf.InputText = identifierValue;
                        container.Add(idf);
                        break;
                    }
                }
            }
        }

        internal static void CreateNewIdentifier(
            string identifierName
            , string identifierValue
            , IdentifierList container
            , IdentifierTypeEnum scope
            )
        {
            IdentifierList idList = IdentifierList.GetIdentifierListByType(scope, true, true);
            foreach (Identifier item in idList)
            {
                if (item.Name.Equals(identifierName, StringComparison.OrdinalIgnoreCase))
                {
                    Identifier idf =
                        Identifier.NewIdentifier(0, item.IdentifierID, item.Name, item.Description, true);
                    idf.InputText = identifierValue;
                    container.Add(idf);
                    break;
                }
            }
        }

        internal static void AddProject(ProjectList container, string projectName, ProjectList.ProjectTypeEnum projectType)
        {
            ProjectList projectList = ProjectList.GetActiveProjectListByPersonIDAndType(COEUser.ID, projectType, true);
            bool valid = false;

            foreach (Project project in projectList)
            {
                if (project.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase))
                {
                    Project newProject = Project.NewProject(0, project.ProjectID, projectName, true, project.Description, projectType.ToString());

                    container.Add(newProject);

                    valid = true;

                    break;
                }
            }

            if (!valid)
                throw new Exception(string.Format("Provided project '{0}' cannot be found under level {1}", projectName, projectType.ToString()));
        }

        internal static void AddProject(ProjectList container, int projectId, ProjectList.ProjectTypeEnum projectType)
        {
            ProjectList projectList = ProjectList.GetActiveProjectListByPersonIDAndType(COEUser.ID, projectType, true);
            bool valid = false;

            foreach (Project project in projectList)
            {
                if (project.ProjectID.Equals(projectId))
                {
                    Project newProject = Project.NewProject(0, project.ProjectID, project.Name, true, project.Description, projectType.ToString());

                    container.Add(newProject);

                    valid = true;

                    break;
                }
            }

            if (!valid)
                throw new Exception(string.Format("Provided project '{0}' cannot be found under level {1}", projectId, projectType.ToString()));
        }

        private const string REG_LIST = "REGISTRYLIST";
        private const string REG_NUM = "REGNUMBER";
        private const string PERCENTAGE = "PERCENTAGE";

        /// <summary>
        /// Generates a mixture from existing component registry numbers and their relative percentages
        /// in the mixture.
        /// </summary>
        /// <param name="container">a registryRecord instance</param>
        /// <param name="compoundRegNumList">a delimited list of compound registry numbers</param>
        /// <param name="compoundPercentagesList">a delimited list of item percentage values</param>
        /// <param name="delimiterCharacter">
        /// the delimiter for the <paramref name="compoundRegNumList">compound</paramref>
        /// and <paramref name="compoundPercentagesList">percentage</paramref> lists
        /// </param>
        internal static void CreateMixtureFromRegisteredComponents(
            RegistryRecord container
            , string compoundRegNumList
            , string compoundPercentagesList
            , string delimiterCharacter
            )
        {

            string[] percentages = null;
            string[] ids = compoundRegNumList.Split(
                new string[] { delimiterCharacter }, StringSplitOptions.RemoveEmptyEntries);

            if (!string.IsNullOrEmpty(compoundPercentagesList))
            {
                percentages = compoundPercentagesList.Split(
                    new string[] { delimiterCharacter }, StringSplitOptions.RemoveEmptyEntries);
            }

            //generate a 'query' xml for the DAL to parse
            XmlDocument criteria = new XmlDocument();
            XmlElement list = criteria.CreateElement(REG_LIST);
            foreach (string id in ids)
            {
                XmlElement rn = criteria.CreateElement(REG_NUM);
                rn.InnerXml = id;
                list.AppendChild(rn);
            }
            criteria.AppendChild(list);

            CompoundList compounds = CompoundList.GetCompoundList(criteria.OuterXml);
            if (compounds != null && compounds.Count > 0)
            {
                container.ComponentList.Clear();
                container.BatchList[0].BatchComponentList.Clear();

                for (int compoundIndex = 0; compoundIndex < compounds.Count; compoundIndex++)
                {
                    Component c = Component.NewComponent(true);
                    c.Compound = compounds[compoundIndex];
                    container.AddComponent(c);

                    //provide component percentages for each item in a batch, as applicable
                    if (percentages != null && percentages.Length == percentages.Length)
                    {
                        PropertyList batchComponentProperties = container.BatchList[0].BatchComponentList[compoundIndex].PropertyList;
                        if (batchComponentProperties.CheckExistingNames(PERCENTAGE, false))
                        {
                            double compoundPercentage;
                            if (Double.TryParse(percentages[compoundIndex], out compoundPercentage))
                                batchComponentProperties[PERCENTAGE].Value = compoundPercentage.ToString();
                        }
                    }
                }
            }
        }

        private const string EDITSCOPEALL = "EDIT_SCOPE_ALL";
        private const string EDITCOMPOUNDTEMP = "EDIT_COMPOUND_TEMP";
        private const string EDITCOMPOUNDREG = "EDIT_COMPOUND_REG";
        private const string EDITSCOPESUPERVISOR = "EDIT_SCOPE_SUPERVISOR";
        private const string EDITSHAREDSTRUCTURES = "EDIT_SHARED_STRUCTURES";
        private const string EDITSHAREDCOMPONENTS = "EDIT_SHARED_COMPONENTS";
        private const string CREATEDUPCOMPONENT = "CREATE_DUP_COMPONENT";
        private const string CREATEDUPREGISTRY = "CREATE_DUP_REGISTRY";
     
        
        /// <summary>
        /// Checks the compound edition scope of the current user
        /// </summary>
        /// <param name="personCreatedId">the compound owner id</param>
        /// <param name="isTemporal">determines if the container record is temporal or not</param>
        /// <returns></returns>
        public static bool CanUserEditRegistration(int personCreatedId, bool isTemporal)
        {
            bool checkSupervisor = COEUserBO.GetUserByID(personCreatedId).SupervisorID == COEUser.ID ? true : false;

            RoleList roleList = RoleList.GetListByUser(COEUser.Name);
            foreach (NameValueListBase<string, string>.NameValuePair role in roleList)
            {
                PrivilegeList privilegeList = PrivilegeList.GetList(role.Key);
                if (privilegeList.Value(EDITSCOPEALL))
                    return true;

                if (personCreatedId == COEUser.ID)
                {
                    if (isTemporal && privilegeList.Value(EDITCOMPOUNDTEMP))
                        return true;
                    if (!isTemporal && privilegeList.Value(EDITCOMPOUNDREG))
                        return true;
                }
                else if (checkSupervisor)
                {
                    if (privilegeList.Value(EDITSCOPESUPERVISOR))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether current user can propogate structure and component edits
        /// </summary>      
        public static bool CanUserPropogateStructureEdits()
        {
            
            RoleList roleList = RoleList.GetListByUser(COEUser.Name);
            foreach (NameValueListBase<string, string>.NameValuePair role in roleList)
            {
                PrivilegeList privilegeList = PrivilegeList.GetList(role.Key);
                if (privilegeList.Value(EDITSHAREDSTRUCTURES))
                    return true;
                
            }

            return false;
        }

        /// <summary>
        /// Checks whether current user can propogate structure and component edits
        /// </summary>       
        public static bool CanUserPropogateComponentEdits()
        {

            RoleList roleList = RoleList.GetListByUser(COEUser.Name);
            foreach (NameValueListBase<string, string>.NameValuePair role in roleList)
            {
                PrivilegeList privilegeList = PrivilegeList.GetList(role.Key);
                if (privilegeList.Value(EDITSHAREDCOMPONENTS))
                    return true;

            }

            return false;
        }

        

        /// <summary>
        /// Checks whether current user can create duplicate components
        /// </summary>       
        public static bool CanUserCreateDuplicateComponent()
        {

            RoleList roleList = RoleList.GetListByUser(COEUser.Name);
            foreach (NameValueListBase<string, string>.NameValuePair role in roleList)
            {
                PrivilegeList privilegeList = PrivilegeList.GetList(role.Key);
                if (privilegeList.Value(CREATEDUPCOMPONENT))
                    return true;

            }

            return false;
        }

        /// <summary>
        /// Checks whether current user can create duplicate registry
        /// </summary>       
        public static bool CanUserCreateDuplicateRegistry()
        {

            RoleList roleList = RoleList.GetListByUser(COEUser.Name);
            foreach (NameValueListBase<string, string>.NameValuePair role in roleList)
            {
                PrivilegeList privilegeList = PrivilegeList.GetList(role.Key);
                if (privilegeList.Value(CREATEDUPREGISTRY))
                    return true;

            }

            return false;
        }

        

             /// <summary>
        /// Checks whether Components are selected automatically for mixture duplicate checking
        /// </summary>       
        public static bool CanAutoSelectComponentForDupChk(Sequence sequence)
        {

            if (Sequence.GetAutoSelCompDupChk(sequence.ID))
            {
                return true;
            }
           

            return false;
        }

        /// <summary>
        /// Method to get the  no of components so that we can differentiate b/w a mixture and single component 
        /// </summary>
        /// <returns></returns>
        /// 
        public static int GetComponentCount()
        {
            RegistryRecord _regRecord;
            int iComponentCount = 0;
            try
            {

                if (HttpContext.Current.Session != null && HttpContext.Current.Session["MultiCompoundBusinessObject"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["MultiCompoundBusinessObject"].ToString()))
                {
                   // _regRecord = ((RegistryRecord)(HttpContext.Current.Session["MultiCompoundBusinessObject"])).Clone();
                    _regRecord = ((RegistryRecord)(HttpContext.Current.Session["MultiCompoundBusinessObject"]));//removed .clone() fix for CBOE-499 
                    iComponentCount = _regRecord.ComponentList.Count;
                }
            }
            catch (System.Exception ex)
            {
                iComponentCount = 1;
            }
            return iComponentCount;
        }


        #endregion

    }
}
