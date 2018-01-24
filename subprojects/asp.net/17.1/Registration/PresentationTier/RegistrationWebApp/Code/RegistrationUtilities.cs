using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Linq;
using CambridgeSoft.COE.Framework.GUIShell;
using Resources;
using System.Reflection;
using CambridgeSoft.COE.Framework.COEPageControlSettingsService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using System.Collections;
using System.Collections.Generic;
using RegistrationWebApp.Forms.ComponentDuplicates.ContentArea;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.RegistrationAdmin.Services.Common;
using RegistrationWebApp;
using RegistrationWebApp.Forms.ComponentDuplicates;
using System.IO;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.Common.Exceptions;
using System.Text;
using CambridgeSoft.COE.Registration;


/// <summary>
/// Common Utilities (methods, properties, vars for Registration App)
/// </summary>
/// <remarks>Note that some common functionalities for all GUIShell based App, are availables in GUIShellUtilities class (Framework)</remarks>
/// <see>/Common/GUIShell/GUIShellUtilities.cs (Framework.dll)</see>
public class RegUtilities
{
    #region Constructors

    private RegUtilities()
    {

    }

    #endregion

    #region Private Variables
    private static string _errorMessage = string.Empty;
    private static bool _overrideDisplayErrorMessage = false;

    #endregion

    #region Properties

    public static string ThemesCommonImagesPath
    {
        get { return "~/App_Themes/Common/Images/"; }
    }

    public static string ErrorMessage
    {
        get {return _errorMessage;}
        set {_errorMessage = value;}
    }

    public static bool OverrideDisplayErrorMessage
    {
        get
        {
            if(string.IsNullOrEmpty(ErrorMessage)) // RULE
                _overrideDisplayErrorMessage = false;
            return _overrideDisplayErrorMessage;
        }
        set
        {
            _overrideDisplayErrorMessage = value;
        }

    }

    /// <summary>
    /// Adds individual pageurls as the user navigates to different pages.
    /// On SET  Accepts string as an object.
    /// On GET  Returns List of url paths as List of type string.
    /// </summary>
    /// <returns>List of type string.</returns>
    public static object UserNavigationPaths
    {
        get
        {
            return GetUserNavigationPaths;
        }
        set
        {
            List<string> lstPaths = GetUserNavigationPaths;
            if (!(string.IsNullOrEmpty(value.ToString()) && lstPaths.Contains(value.ToString())))
            {
                lstPaths.Add(value.ToString());
            }
            GetUserNavigationPaths = lstPaths;
        }

    }

    /// <summary>
    /// Create session for storing url paths .
    /// </summary>
    /// <returns>List of type string.</returns>
    private static List<string> GetUserNavigationPaths
    {
        get
        {
            if (HttpContext.Current.Session["UserNavigationPaths"] == null)
            {
                HttpContext.Current.Session["UserNavigationPaths"] = new List<string>();
            }
            return ((List<string>)HttpContext.Current.Session["UserNavigationPaths"]);
        }
        set
        {
            HttpContext.Current.Session["UserNavigationPaths"] = value;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns the identifier list with adding inactive identifiers which are used in current record
    ///<param name="sourceIdentifiers" > identifiers fetched from DB , contains all active identifiers </param>
    ///<param name="registryRecord">Current registry record object to fetch identifiers based on identifier type supplied</param>
    ///<param name="identifierType">Type of identifiers to add</param>
    /// </summary>
    /// <returns>Returns identifier list</returns>
    public static IdentifierList AddInActiveIdentifiers(IdentifierList sourceIdentifiers, RegistryRecord registryRecord, IdentifierTypeEnum identifierType)
    {
        try 
        {
            var sourceIdentifiersList = sourceIdentifiers.ToList();
            switch (identifierType)
            {
                case IdentifierTypeEnum.C:
                    foreach (Component component in registryRecord.ComponentList)
                    {
                        IdentifierList componentIdenList = component.Compound.IdentifierList;
                        foreach (Identifier identifier in componentIdenList)
                        {
                            if (identifier.Active)
                                continue;
                            var i = sourceIdentifiersList.FindIndex(x => x.IdentifierID == identifier.IdentifierID);
                            if (i == -1)
                                sourceIdentifiers.Add(identifier);
                        }
                    }
                    break;
                case IdentifierTypeEnum.R:
                    IdentifierList regIdenList = registryRecord.IdentifierList;
                    foreach (Identifier identifier in regIdenList)
                    {
                        if (identifier.Active)
                            continue;
                        var i = sourceIdentifiersList.FindIndex(x => x.IdentifierID == identifier.IdentifierID);
                        if (i == -1)
                            sourceIdentifiers.Add(identifier);
                    }
                    break;
                case IdentifierTypeEnum.B:
                    foreach (Batch batch in registryRecord.BatchList)
                    {
                        IdentifierList bacthIdenList = batch.IdentifierList;
                        foreach (Identifier identifier in bacthIdenList)
                        {
                            if (identifier.Active)
                                continue;
                            var i = sourceIdentifiersList.FindIndex(x => x.IdentifierID == identifier.IdentifierID);
                            if (i == -1)
                                sourceIdentifiers.Add(identifier);
                        }
                    }
                    break;
                case IdentifierTypeEnum.S:
                    foreach (Component component in registryRecord.ComponentList)
                    {
                        IdentifierList componentIdenList = component.Compound.BaseFragment.Structure.IdentifierList;
                        foreach (Identifier identifier in componentIdenList)
                        {
                            if (identifier.Active)
                                continue;
                            var i = sourceIdentifiersList.FindIndex(x => x.IdentifierID == identifier.IdentifierID);
                            if (i == -1)
                                sourceIdentifiers.Add(identifier);
                        }
                    }
                    break;
            }
            return sourceIdentifiers;

        }
        catch (Exception ex)
        {
            COEExceptionDispatcher.HandleBLLException(ex, COEExceptionDispatcher.Policy.LOG_ONLY);
            return IdentifierList.GetIdentifierListByType(identifierType);
        }
    }

    public delegate Object CreateNewListElement();

    /// <summary>
    /// Returns the index of the desired element, if found. Otherwise returns -1.
    /// We use indexes instead of (more desirable)Objects because somewhy List.GetIndex(IBusinessObject Element) always returns zero. So List.Remove(element) inexorably removes the first element.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="idPrimaryKey"></param>
    /// <param name="list"></param>
    /// <returns></returns>

    private static int GetListElementIndex(int id, string idPrimaryKey, IList list)
    {
        IEnumerator e = list.GetEnumerator();

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
    public static void SincronizeListDataTable(DataTable dataTable, string idPrimaryKey, IList list, CreateNewListElement newElementDelegate)
    {
        List<DataRow> rowsToDelete = new List<DataRow>();

        if (dataTable != null)
        {
            foreach (DataRow currentDataRow in dataTable.Rows)
            {
                switch (currentDataRow.RowState)
                {

                    case DataRowState.Deleted:
                        int index = -1;
                        if ((index = GetListElementIndex(int.Parse(currentDataRow[idPrimaryKey, DataRowVersion.Original].ToString()), idPrimaryKey, list)) >= 0)
                            list.RemoveAt(index);

                        rowsToDelete.Add(currentDataRow);
                        break;
                    case DataRowState.Added:
                        list.Add(newElementDelegate());
                        break;
                    default:
                        if (GetListElementIndex(int.Parse(currentDataRow[idPrimaryKey, DataRowVersion.Original].ToString()), idPrimaryKey, list) < 0)
                            list.Add(newElementDelegate());
                        break;
                }
            }

            foreach (DataRow currentRowToDelete in rowsToDelete)
                dataTable.Rows.Remove(currentRowToDelete);
        }
    }

    public enum DuplicateType
    {
        Compound,
        Mixture,
        DuplicatedOrCopied,
        None,
    }

    public static DuplicateType HandleDuplicates(RegistryRecord registryRecord, string duplicatesXml, bool isPreReg)
    { //here we need to determine if autoselcompdupchk is true for the prefix
        if (!string.IsNullOrEmpty(duplicatesXml))
        {
            if (duplicatesXml.Contains("<COMPOUNDLIST>") && duplicatesXml.Contains("</COMPOUNDLIST>"))
            {
                int startIndex = duplicatesXml.IndexOf("<COMPOUNDLIST>");
                int endIndex = duplicatesXml.LastIndexOf("</COMPOUNDLIST>") + "</COMPOUNDLIST>".Length;
                duplicatesXml = duplicatesXml.Substring(startIndex, endIndex - startIndex);

                HttpContext.Current.Session[RegistrationWebApp.Constants.DuplicateCompoundIdsList_Session] = duplicatesXml;

                DuplicatesResolver duplicatesResolver = new DuplicatesResolver(registryRecord, duplicatesXml, isPreReg);

                if (!duplicatesResolver.HasUnsolvedComponents && string.IsNullOrEmpty(registryRecord.FoundDuplicates)){
                    return DuplicateType.None;
                }
                else if (duplicatesResolver.Duplicates == null){
                    return DuplicateType.DuplicatedOrCopied;
                }
                else{

                    if (!registryRecord.IsSingleCompound && registryRecord.CanAutoSelectComponentForDupChk())
                    {   //this looks hrough all the components in the mixture that are duplicated and chooins the first mactching
                        //component that is stored in the duplicate resolve.  after each interaction one 
                        //of the components is removed so you need to pad the index +1 or you miss the last component.
                        for (int i = 0; i<duplicatesResolver.CompoundsToResolve.Count +1; i++)
                        {
                            registryRecord = duplicatesResolver.AutoCreateCompoundForm();
                        }
                        //now check to see if a duplicate mixture was found after tha autoresolution
                        if (registryRecord.DalResponseMessage.Contains("mixture"))
                        {
                            return DuplicateType.Mixture;
                        }
                        else
                        {
                            return DuplicateType.None;
                        }
                    }
                    return DuplicateType.Compound;
                }
            }

            if (duplicatesXml.Contains("<REGISTRYLIST>") && duplicatesXml.Contains("</REGISTRYLIST>"))
            {
                int startIndex = duplicatesXml.IndexOf("<REGISTRYLIST>");
                int endIndex = duplicatesXml.LastIndexOf("</REGISTRYLIST>") + "</REGISTRYLIST>".Length;
                duplicatesXml = duplicatesXml.Substring(startIndex, endIndex - startIndex);

                HttpContext.Current.Session[RegistrationWebApp.Constants.DuplicateIdsList_Session] = duplicatesXml;

                DuplicatesList duplicatesList = new DuplicatesList(duplicatesXml, true, registryRecord.ComponentList.Count > 1);

                if (duplicatesList.Count > 0)
                    return DuplicateType.Mixture;
                else
                {
                    if (registryRecord.IsTemporal)
                        registryRecord.Register(DuplicateCheck.None);
                    else
                        registryRecord.Save(DuplicateCheck.None);

                    return DuplicateType.None;
                }
            }
        }

        return DuplicateType.None;
    }

    /// <summary>
    /// Method to check if it is a valid Temp Id.
    /// </summary>
    /// <param name="tempId">The regId to test</param>
    /// <returns>Booolean indicating the validation</returns>
    /// <remarks>We can also check here with a regular expresion, SQL Inyection, etc.</remarks>
    public static bool IsAValidTempId(string tempId)
    {
        bool retVal = false;
        int temporalTempID = int.MinValue;
        //Clean String
        tempId = GUIShellUtilities.CleanString(tempId);
        //For now We just check if it is a int type.
        if (int.TryParse(tempId, out temporalTempID))
            retVal = true;

        return retVal;
    }

    public static bool CheckIfMWAndFormulaMatch(string Base64_1, string Base64_2)
    {

        ChemDrawControl17.ChemDrawCtl ctl = new ChemDrawControl17.ChemDrawCtl();
        ctl.Objects.Clear();
        ctl.DataEncoded = true;
        ctl.Objects.set_Data("chemical/x-cdx", null, null, null, UnicodeEncoding.ASCII.GetBytes(Base64_1));
        double objectMW1 = ctl.Objects.MolecularWeight;
        string objectFormula1 = ctl.Objects.Formula;
        ctl.Objects.Clear();
        ctl.Objects.set_Data("chemical/x-cdx", null, null, null, UnicodeEncoding.ASCII.GetBytes(Base64_2));
        double objectMW2 = ctl.Objects.MolecularWeight;
        string objectFormula2 = ctl.Objects.Formula;
        if (objectMW1 == objectMW2 && objectFormula1 == objectFormula2)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    /// <summary>
    /// Method to check if it is a valid Registry Id.
    /// </summary>
    /// <param name="regId">The regId to test</param>
    /// <returns>Booolean indicating the validation</returns>
    /// <remarks>We can also check here with a regular expresion, SQL Inyection, etc.</remarks>
    public static bool IsAValidRegId(string regId)
    {
        bool retVal = true;
        //Clean String
        regId = GUIShellUtilities.CleanString(regId);
        return retVal;
    }

    /// <summary>
    /// Method to write messages to our selected Log File. For GuiShell Pages the selected Log is Trace.axd file.
    /// </summary>
    /// <param name="messageType">Type of message</param>
    /// <param name="message">String to write to Log (For instance: a Method Name)</param>
    public static void WriteToRegLog(GUIShellTypes.LogMessageType messageType, string message)
    {
        try
        {
            CambridgeSoft.COE.Framework.COELoggingService.COELog _coeLog = CambridgeSoft.COE.Framework.COELoggingService.COELog.GetSingleton("COERegistration");

            switch (messageType)
            {
                //When a method is begining to process.
                case GUIShellTypes.LogMessageType.BeginMethod:
                    _coeLog.LogStart(string.Format(Resource.BeginProcessingMethod_Label_Text, message), 1);
                    break;
                //When a method is ending to process.
                case GUIShellTypes.LogMessageType.EndMethod:
                    _coeLog.LogEnd(string.Format(Resource.EndProcessingMethod_Label_Text, message), 1);
                    break;
            }
        }
        catch
        {
            //Do nothing as we don't want to break the workflow in case there is a logging problem.
        }
    }

    /// <summary>
    /// Returns the URL to the temp search registries form including formID and other required params.
    /// </summary>
    /// <returns>A URL</returns>
    public static string GetSearchTempURL(bool restoreHitListId)
    {
        string url = GUIShellUtilities.GetSearchEngineURL();
        url += string.Format("?FormGroupId={0}&embed=ChemBioViz&AppName={1}&AllowFullScan={2}&KeepRecordCountSynchronized={3}&MarkedHitsMax={4}&ExpireHitlistHistoryDays={5}&ExpireQueryHistoryDays={6}",
                            GetConfigSetting(Groups.CBV, "ReviewRegisterSearchFormGroupId"),
                            GUIShellUtilities.GetApplicationName(),
                            bool.TrueString,
                            bool.TrueString,
                            GetConfigSetting(Groups.CBV, "MarkedHitsMax"),
                            GetConfigSetting(Groups.CBV, "ExpireHitlistHistoryDays"),
                            GetConfigSetting(Groups.CBV, "ExpireQueryHistoryDays")
                            );

        if (restoreHitListId && HttpContext.Current.Session[RegistrationWebApp.Constants.TempSearchedInfo] != null)
        {
            if (HttpContext.Current.Session[RegistrationWebApp.Constants.TempSearchedInfo] is SearchedInfo)
            {
                url += "&" + "HitListID=" + ((SearchedInfo)HttpContext.Current.Session[RegistrationWebApp.Constants.TempSearchedInfo]).HitListID.ToString();
                url += "&" + "HitListType=" + ((SearchedInfo)HttpContext.Current.Session[RegistrationWebApp.Constants.TempSearchedInfo]).HitListType.ToString();
            }
        }
        return url;
    }

    /// <summary>
    /// Returns the URL to search a component from the submit component section
    /// </summary>
    /// <returns>A URL</returns>
    public static string GetSearchComponentURL()
    {
        string url = GUIShellUtilities.GetSearchEngineURL();
        url += string.Format("?FormGroupId={0}&embed=ChemBioViz&AppName={1}&AllowFullScan={2}&KeepRecordCountSynchronized={3}",
                            GetConfigSetting(Groups.CBV, "SearchComponentsToAddFormGroupId"),
                            GUIShellUtilities.GetApplicationName(),
                            bool.TrueString,
                            bool.FalseString);
        return url;
    }

    /// <summary>
    /// Returns the URL to search duplicates components 
    /// </summary>
    /// <returns>A URL</returns>
    public static string GetSearchDuplicatesComponentsURL()
    {
        string url = GUIShellUtilities.GetSearchEngineURL();
        url += string.Format("?FormGroupId={0}&embed=ChemBioViz&AppName={1}&AllowFullScan={2}&KeepRecordCountSynchronized={3}",
                            GetConfigSetting(Groups.CBV, "SearchDuplicatesComponentsFormGroupId"),
                            GUIShellUtilities.GetApplicationName(),
                            bool.TrueString,
                            bool.FalseString);
        return url;
    }

    /// <summary>
    /// Returns the URL to search a component from review register section
    /// </summary>
    /// <returns>A URL</returns>
    public static string GetSearchComponentRRURL()
    {
        string url = GUIShellUtilities.GetSearchEngineURL();
        url += string.Format("?FormGroupId={0}&embed=ChemBioViz&AppName={1}&AllowFullScan={2}&KeepRecordCountSynchronized={3}",
                            GetConfigSetting(Groups.CBV, "SearchComponentsToAddRRFormGroupId"),
                            GUIShellUtilities.GetApplicationName(),
                            bool.TrueString,
                            bool.FalseString);
        return url;
    }

    /// <summary>
    /// Returns the URL to search a component from view mixture section 
    /// </summary>
    /// <returns>A URL</returns>
    public static string GetSearchComponentVMURL()
    {
        string url = GUIShellUtilities.GetSearchEngineURL();
        url += string.Format("?FormGroupId={0}&embed=ChemBioViz&AppName={1}&AllowFullScan={2}&KeepRecordCountSynchronized={3}",
                            GetConfigSetting(Groups.CBV, "SearchComponentsToAddVMFormGroupId"),
                            GUIShellUtilities.GetApplicationName(),
                            bool.TrueString,
                            bool.FalseString);
        return url;
    }


    /// <summary>
    /// Returns the URL to the delete marked form including formID and other required params.
    /// </summary>
    /// <returns>A URL</returns>
    public static string GetDeleteMarkedURL()
    {
        string url = GUIShellUtilities.GetSearchEngineURL();
        url += string.Format("?FormGroupId={0}&embed=ChemBioViz&AppName={1}",
                            GetConfigSetting(Groups.CBV, "DeleteLogFormGroupId"),
                            GUIShellUtilities.GetApplicationName());
        return url;
    }

    /// <summary>
    /// Gets the field name to be used for highliting batches (if any)
    /// </summary>
    /// <returns></returns>
    public static string GetHighlightingField()
    {
        string result = string.Empty;
        try
        {
            result = GetConfigSetting(Groups.RegAdmin, "BatchHighlightingField");
        }
        catch { }
        return result;
    }

    /// <summary>
    /// Gets the value required to highlight a batch (if any)
    /// </summary>
    /// <returns></returns>
    public static string GetHighlightingValue()
    {
        string result = string.Empty;
        try
        {
            result = GetConfigSetting(Groups.RegAdmin, "BatchHighlightingValue");
        }
        catch { }
        return result;
    }

    /// <summary>
    /// Gets the tooltip to display if a batch is highlighted
    /// </summary>
    /// <returns></returns>
    public static string GetHighlightingTooltip()
    {
        string result = string.Empty;
        try
        {
            result = GetConfigSetting(Groups.RegAdmin, "BatchHighlightingTooltip");
        }
        catch { }
        return result;
    }

    /// <summary>
    /// Returns a COEFormId given the name
    /// </summary>
    /// <param name="formId">Name of the form</param>
    /// <returns>ID of the COEForm</returns>
    public static int GetCOEFormID(string formId)
    {
        int retVal = -1;
        if (!string.IsNullOrEmpty(GetConfigSetting(Groups.CBV, formId)))
            int.TryParse(GetConfigSetting(Groups.CBV, formId), out retVal);
        return retVal;
    }
    /*CSBR-154565
     Modified By DIVYA
     Added to enable or disable SendToInventory link depending upon configuration settings.
   */
    public static bool IsSendToInventoryEnabled()
    {
        string SendToInventorySet = GetConfigSetting(Groups.Inventory, "SendtoInventory");
        return string.IsNullOrEmpty(SendToInventorySet) ? false : SendToInventorySet.ToLower() == "true";
    }
    //End of CSBR-154565
    /// <summary>
    /// Gets a Registration setting
    /// </summary>
    /// <param name="group">Groups which it belongs</param>
    /// <param name="key">Settings Name</param>
    /// <returns>Found settting or empty</returns>
    public static string GetConfigSetting(string group, string key)
    {
        return GetConfigSetting(group, key, false);
    }

    public static string GetConfigSetting(string group, string key, bool ignoreCache)
    {
        return FrameworkUtils.GetAppConfigSetting(GUIShellUtilities.GetApplicationName(), group, key, ignoreCache);
    }

    /// <summary>
    /// Gets a Registration setting
    /// </summary>
    /// <param name="group">Groups which it belongs</param>
    /// <param name="key">Settings Name</param>
    /// <returns>Found settting or empty</returns>
    public static string GetConfigSetting(Groups group, string key)
    {
        return GetConfigSetting(group, key, false);
    }

    public static string GetConfigSetting(Groups group, string key, bool ignoreCase)
    {
        return GetConfigSetting(group.ToString().ToUpper(), key, ignoreCase);
    }

    /// <summary>
    /// Gets the view mixture URL params to be added when redirecting from submit page.
    /// </summary>
    /// <returns>The extra params to add to the Server.Transfer call</returns>
    /// <remarks>It reads the configuration settings for Registration</remarks>
    public static string GetViewMixURLParams()
    {
        return !string.IsNullOrEmpty(GetConfigSetting(Groups.Misc, "ViewMixtureExtraParamURL")) ?
            GetConfigSetting(Groups.Misc, "ViewMixtureExtraParamURL") : string.Empty;
    }

    /// <summary>
    /// Gets the display left panel visibility from configuration.
    /// </summary>
    /// <returns>A boolean indicating the visibility of the left panel section</returns>
    /// <remarks>It reads the configuration settings for Registration</remarks>
    public static bool GetDisplayLeftPanelVisibility()
    {
        bool retVal = true;
        string configSet = GetConfigSetting(Groups.Misc, "ShowLeftPanel");
        if (!string.IsNullOrEmpty(configSet))
            bool.TryParse(configSet, out retVal);
        return retVal;
    }

    /// <summary>
    /// Returns the URL to the permanent search registries form including formID and other required params.
    /// </summary>
    /// <returns>A URL</returns>
    public static string GetSearchPermURL()
    {
        string url = GUIShellUtilities.GetSearchEngineURL();
        url += string.Format("?FormGroupId={0}&embed=ChemBioViz&AppName={1}&KeepRecordCountSynchronized={2}&AllowFullScan={4}&MarkedHitsMax={5}&ExpireHitlistHistoryDays={6}&ExpireQueryHistoryDays={7}",
                            GetConfigSetting(Groups.CBV, "ViewRegistrySearchFormGroupId"),
                            HttpContext.Current.Application[RegistrationWebApp.Constants.AppName].ToString(),
                            bool.TrueString,
                            bool.TrueString,
                            bool.TrueString,
                            GetConfigSetting(Groups.CBV, "MarkedHitsMax"), 
                            GetConfigSetting(Groups.CBV, "ExpireHitlistHistoryDays"),
                            GetConfigSetting(Groups.CBV, "ExpireQueryHistoryDays")
                            );

        if (HttpContext.Current.Session[RegistrationWebApp.Constants.PermSearchedInfo] != null)
        {
            if (HttpContext.Current.Session[RegistrationWebApp.Constants.PermSearchedInfo] is SearchedInfo)
            {
                url += "&" + "HitListID=" + ((SearchedInfo)HttpContext.Current.Session[RegistrationWebApp.Constants.PermSearchedInfo]).HitListID.ToString();
                url += "&" + "HitListType=" + ((SearchedInfo)HttpContext.Current.Session[RegistrationWebApp.Constants.PermSearchedInfo]).HitListType.ToString();
            }
        }
        return url;
    }

    /// <summary>
    /// Returns true if reassigning batches between registry records is allowed by configuration.
    /// </summary>
    /// <returns>True if batches can be moved, false otherwise.</returns>
    public static bool GetAllowMoveBatch()
    {
        string item = RegUtilities.GetConfigSetting(Groups.RegAdmin, "EnableMoveBatch");
        return !string.IsNullOrEmpty(item) && item.ToLower() == "true";
    }

    /// <summary>
    /// Returns true if the approvals workflow was enabled by configuration
    /// </summary>
    /// <returns></returns>
    public static bool GetApprovalsEnabled()
    {
        string item = RegUtilities.GetConfigSetting(Groups.RegAdmin, "ApprovalsEnabled");
        return !string.IsNullOrEmpty(item) && item.ToLower() == "true";
    }
    /// <summary>
    /// Returns true is Locking work flow enabled
    /// </summary>
    /// <returns></returns>
    public static bool GetLockingEnabled()
    {
        string item = RegUtilities.GetConfigSetting(Groups.RegAdmin, "LockingEnabled");
        return !string.IsNullOrEmpty(item) && item.ToLower() == "true";
    }
    /// <summary>
    /// Returns true if the batchprefix workflow was enabled by configuration
    /// </summary>
    /// <returns></returns>
    public static bool GetBatchPrefixesEnabled()
    {
        string item = RegUtilities.GetConfigSetting(Groups.RegAdmin, "EnableUseBatchPrefixes");
        return !string.IsNullOrEmpty(item) && item.ToLower() == "true";
    }

    /// <summary>
    /// Returns true if the unregistered components allowed in mixtures workflow was enabled by configuration
    /// </summary>
    /// <returns></returns>
    public static bool GetAllowUnregisteredComponents()
    {
        string item = RegUtilities.GetConfigSetting(Groups.RegAdmin, "AllowUnregisteredComponentsInMixtures");
        return !string.IsNullOrEmpty(item) && item.ToLower() == "true";
    }

    /// <summary>
    /// Returns true if the unregistered components allowed in mixtures workflow was enabled by configuration
    /// </summary>
    /// <returns></returns>
    public static bool GetMixturesEnabled()
    {
        string item = RegUtilities.GetConfigSetting(Groups.RegAdmin, "EnableMixtures");
        return !string.IsNullOrEmpty(item) && item.ToLower() == "true";
    }

    /// <summary>
    /// Returns true if the use fragments is enabled
    /// </summary>
    /// <returns></returns>
    public static bool GetFragmentsEnabled()
    {
        string item = RegUtilities.GetConfigSetting(Groups.RegAdmin, "EnableFragments");
        return !string.IsNullOrEmpty(item) && item.ToLower() == "true";
    }

    /// <summary>
    /// Returns true if the unregistered components allowed in mixtures workflow was enabled by configuration
    /// </summary>
    /// <returns></returns>
    public static bool GetEnableSubmissionDuplicateCheck()
    {
        string item = RegUtilities.GetConfigSetting(Groups.RegAdmin, "EnableSubmissionDuplicateChecking");
        return !string.IsNullOrEmpty(item) && item.ToLower() == "true";
    }
    /// <summary>
    /// Returns the PageSettings xml to be loaded as an object for further reference.
    /// </summary>
    /// <returns></returns>
    public static string GetPageSettingsXML()
    {
        return HttpContext.Current.Server.MapPath(@"~/Forms/Settings/COEControlPagesSettings.xml");
    }

    /// <summary>
    /// Returns the PageSettings xml to be loaded as an object for further reference.
    /// </summary>
    /// <returns></returns>
    public static void SetCOEPageSettings(bool force)
    {
        if (HttpContext.Current.Session != null)
        {
            if (HttpContext.Current.Session[GUIShellTypes.COEPageSettings + COEAppName.Get().ToString()] == null || force)
            {
                CambridgeSoft.COE.Framework.COEPageControlSettingsService.ControlList ctrls = COEPageControlSettings.GetControlListToDisableForCurrentUser(GetApplicationName());
                if (ctrls != null)
                    HttpContext.Current.Session[GUIShellTypes.COEPageSettings + COEAppName.Get().ToString()] = ctrls;
            }
        }
    }

    /// <summary>
    /// Cleans all known session variables in the App
    /// </summary>
    /// <param name="Session">Session obj to remove states</param>
    public static void CleanSession(System.Web.SessionState.HttpSessionState Session)
    {
        Session.Remove(RegistrationWebApp.Constants.CompoundsToResolve_Session);
        Session.Remove(RegistrationWebApp.Constants.DuplicateCompoundIdsList_Session);
        Session.Remove(RegistrationWebApp.Constants.DuplicateCompoundObjects_Session);
        Session.Remove(RegistrationWebApp.Constants.DuplicateIdsList_Session);
        Session.Remove(RegistrationWebApp.Constants.DuplicateMultiCompounds_Session);

        if (Session[RegistrationWebApp.Constants.MultiCompoundObject_Session] != null)
            ((IDisposable)Session[RegistrationWebApp.Constants.MultiCompoundObject_Session]).Dispose();

        Session.Remove(RegistrationWebApp.Constants.MultiCompoundObject_Session);
    }

    /// <summary>
    /// Returns the assembly File version attribute.
    /// </summary>
    /// <returns>File version text</returns>
    public static string GetRegistrationFileVersion()
    {
        string retVal = "Uknown";
        object[] fileversionAttributte;
        Assembly assembly = Assembly.GetExecutingAssembly();
        if (assembly != null)
        {
            fileversionAttributte = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            if (fileversionAttributte != null && fileversionAttributte.Length > 0)
                retVal = ((AssemblyFileVersionAttribute)fileversionAttributte[0]).Version;

        }
        return retVal;
    }

    /// <summary>
    /// Returns the Application name setting.
    /// </summary>
    /// <returns>App name</returns>
    public static string GetApplicationName()
    {
        return HttpContext.Current.Application[RegistrationWebApp.Constants.AppName] != null ?
                HttpContext.Current.Application[RegistrationWebApp.Constants.AppName].ToString().ToUpper() : string.Empty;
    }

    /// <summary>
    /// Gets the array of DocMgr fields to use in the search.
    /// </summary>
    /// <returns>Array of string with the list of field from the web.config entry</returns>
    public static string[] GetDocMgrFields()
    {
        string splitter = "|";
        string[] retVal = { "" };
        return ParseArray(RegUtilities.GetConfigSetting("DOCMGR", "DocMgrFieldsToSearch"), splitter.ToCharArray());
    }

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
    /// Gets the DataView ID for DocMgr Search
    /// </summary>
    /// <returns></returns>
    public static int GetDocMgrDataViewID()
    {
        return !string.IsNullOrEmpty(RegUtilities.GetConfigSetting("DOCMGR", "DocMgrDataViewID")) ? int.Parse(RegUtilities.GetConfigSetting("DOCMGR", "DocMgrDataViewID")) : 0;
    }

    /// <summary>
    /// Gets the Search Field for DocMgr
    /// </summary>
    /// <returns></returns>
    public static string GetDocMgrSearchField()
    {
        return RegUtilities.GetConfigSetting("DOCMGR", "DocMgrSearchCriteriaField");
    }

    /// <summary>
    /// Returns boolean value indicating the DocManager Enabled | Disabled.
    /// </summary>
    /// <returns></returns>
    public static bool GetDocMgrEnabled()
    {
        string docMgrSet = RegUtilities.GetConfigSetting("DOCMGR", "DocMgrEnabled");
        return string.IsNullOrEmpty(docMgrSet) ? false : docMgrSet.ToLower() == "true";
    }

    /// <summary>
    /// Gets InventoryIntegration setting
    /// </summary>
    /// <returns></returns>
    public static bool GetInventoryIntegration()
    {
        return !string.IsNullOrEmpty(RegUtilities.GetConfigSetting(Groups.Inventory, "InventoryIntegration")) ? RegUtilities.GetConfigSetting(Groups.Inventory, "InventoryIntegration").ToLower() == "enabled" : false;
    }


    public static bool GetShowRequestFromContainer()
    {
        return !string.IsNullOrEmpty(RegUtilities.GetConfigSetting(Groups.Inventory, "ShowRequestFromContainer")) ? RegUtilities.GetConfigSetting(Groups.Inventory, "ShowRequestFromContainer").ToLower() == "true" : false;

    }

    /// <summary>
    /// Gets the dataview to be used to read inventory grouping field configuration
    /// </summary>
    /// <returns></returns>
    public static int GetInvGroupingFieldsDataViewID()
    {
        return 3002;
    }

    /// <summary>
    /// Gets the DataView ID for Inventory containers Search
    /// </summary>
    /// <returns></returns>
    public static int GetInvContainersDataViewID()
    {
        return !string.IsNullOrEmpty(RegUtilities.GetConfigSetting(Groups.Inventory, "InvContainersDataviewID")) ? int.Parse(RegUtilities.GetConfigSetting(Groups.Inventory, "InvContainersDataviewID")) : 3001;
    }

    /// <summary>
    /// Gets the inventory URL used to request material
    /// </summary>
    /// <returns>The inventory url</returns>
    public static string GetContainersRequestMaterialURL()
    {
        return !string.IsNullOrEmpty(RegUtilities.GetConfigSetting(Groups.Inventory, "RequestMaterialURL")) ? RegUtilities.GetConfigSetting(Groups.Inventory, "RequestMaterialURL") : "/ChemInv/GUI/RequestSample.asp?action=create";
    }

    /// <summary>
    /// Gets the inventory URL used to create a container
    /// </summary>
    /// <returns></returns>
    public static string GetNewInventoryContainerURL()
    {
        return !string.IsNullOrEmpty(GetConfigSetting(Groups.Inventory, "NewContainerURL")) ? GetConfigSetting(Groups.Inventory, "NewContainerURL") : "/Cheminv/gui/CreateOrEditContainer.asp?GetData=new";
    }

    /// <summary>
    /// To Avoid recursive calls to create a new object for RegistryRecord prototype.
    /// </summary>
    /// <returns>The RegistryRecord object as a prototype which is stored in session.</returns>
    public static RegistryRecord GetNewRegistryRecord()
    {
        try
        {
            if (HttpContext.Current.Session["NewRegistryRecord"] == null)
            {
                HttpContext.Current.Session["NewRegistryRecord"] = RegistryRecord.NewRegistryRecord();
            }
            SetForceImportConfiguration = false; // This ensures that configuration is good .
            return ((RegistryRecord)HttpContext.Current.Session["NewRegistryRecord"]).Clone(); // Clone to return the ByVal,if clone is removed it will be ByRef,Changes made to referred object will effect the NewRegistryRecord session object.
        }
        catch (Exception exception)
        {
            if (exception.InnerException is Csla.DataPortalException && ((Csla.DataPortalException)(exception.InnerException)).BusinessException is BusinessObjectNotFoundException)
            {
                Exception BusinessException = new Exception();
                BusinessException = ((Csla.DataPortalException)(exception.InnerException)).BusinessException.InnerException;

                if (BusinessException.Message.Contains(((int)RegAdminUtils.ConfigurationErrorCodes.ORA30625).ToString()) || BusinessException.Message.Contains(((int)RegAdminUtils.ConfigurationErrorCodes.ORA01403).ToString()))
                {
                    // For more accurate we can check for object which caused exception
                    // ((Csla.DataPortalException)(exception.InnerException)).BusinessObject is RegistryRecord
                    SetForceImportConfiguration = true;// This ensures that configuration is defective and user must import the configuration.
                }
                else
                {
                    _errorMessage = BusinessException.Message;
                    OverrideDisplayErrorMessage = true;
                }
            }
            else
            {
                COEExceptionDispatcher.HandleUIException(exception); // This may not occur just handle it and return null so that it may be some other exception and user is not forced to import configuration which is not defective // Will be a bug fix.
            }
            return null;
        }
    }

    /// <summary>
    /// Set context which will guide the user to import configuration when config is dirty, invalid, null,  empty
    /// 'True' This ensures that configuration is defective and user must import the configuration.
    /// 'False' This ensures that configuration is good .
    /// </summary>
    public static bool SetForceImportConfiguration
    {
        set
        {
            HttpContext.Current.Session[RegistrationWebApp.Constants.AvailableRecord] = (value == true) ? "NO" : "YES"; // This ensures that configuration is defective and user must import the configuration.
        }
    }


    /// <summary>
    /// Call this to clear the navigation paths when user starts from [Search Engines,Creating the components].
    /// </summary>
    public static void ClearNavigationPaths()
    {
        HttpContext.Current.Session["UserNavigationPaths"] = null;
    }

    /// <summary>
    /// Gets the inventory URL used to view a container
    /// </summary>
    /// <returns>The inventory url</returns>
    public static string GetViewContainerURL()
    {
        return !string.IsNullOrEmpty(RegUtilities.GetConfigSetting(Groups.Inventory, "ViewContainerURL")) ? RegUtilities.GetConfigSetting(Groups.Inventory, "ViewContainerURL") : "/ChemInv/GUI/ViewContainer.asp";
    }

    /// <summary>
    /// Gets the send to inventory URL
    /// </summary>
    /// <returns>The inventory url</returns>
    public static string GetSendToInventoryURL()
    {
        return !string.IsNullOrEmpty(RegUtilities.GetConfigSetting(Groups.Inventory, "SendToInventoryURL")) ? RegUtilities.GetConfigSetting(Groups.Inventory, "SendToInventoryURL") : "/cheminv/gui/ImportFromChemReg.asp";
    }

    /// <summary>
    /// Gets the inventory schema name from configuration
    /// </summary>
    /// <returns>The inventory schema name</returns>
    public static string GetInventorySchemaName()
    {
        return !string.IsNullOrEmpty(RegUtilities.GetConfigSetting(Groups.Inventory, "ChemInvSchemaName")) ? RegUtilities.GetConfigSetting(Groups.Inventory, "ChemInvSchemaName") : "CHEMINVDB2";
    }

    /// <summary>
    /// Gets the UseFullContainerForm configuration.
    /// </summary>
    /// <returns>True for using full container form</returns>
    public static bool GetUseFullContainerForm()
    {
        return !string.IsNullOrEmpty(RegUtilities.GetConfigSetting(Groups.Inventory, "UseFullContainerForm")) ? RegUtilities.GetConfigSetting(Groups.Inventory, "UseFullContainerForm").ToLower() == "true" : true;
    }

    /// <summary>
    /// Builds the search criteria for inv_containers that are available.
    /// </summary>
    /// <param name="regid">RegID to filter</param>
    /// <param name="batchid">BatchID to filter</param>
    /// <returns>The SearcgCriteria</returns>
    public static SearchCriteria GetInvContainersSC(int regid, int batchid)
    {
        SearchCriteria sc = new SearchCriteria();
        sc.SearchCriteriaID = 1;
        if (regid > 0)
        {
            SearchCriteria.NumericalCriteria regidCriteria = new SearchCriteria.NumericalCriteria();
            regidCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
            regidCriteria.Value = regid.ToString();
            SearchCriteria.SearchCriteriaItem regidItem = new SearchCriteria.SearchCriteriaItem();
            regidItem.Criterium = regidCriteria;
            regidItem.TableId = 1;
            regidItem.FieldId = 9;
            regidItem.ID = sc.Items.Count + 1;
            sc.Items.Add(regidItem);
        }

        if (batchid > 0)
        {
            SearchCriteria.NumericalCriteria batchidCriteria = new SearchCriteria.NumericalCriteria();
            batchidCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
            batchidCriteria.Value = batchid.ToString();
            SearchCriteria.SearchCriteriaItem batchidItem = new SearchCriteria.SearchCriteriaItem();
            batchidItem.Criterium = batchidCriteria;
            batchidItem.TableId = 1;
            batchidItem.FieldId = 10;
            batchidItem.ID = sc.Items.Count + 1;
            sc.Items.Add(batchidItem);
        }

        // STATUSID = 1 (Available)
        SearchCriteria.NumericalCriteria statusIdCriteria = new SearchCriteria.NumericalCriteria();
        statusIdCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
        statusIdCriteria.Value = "1";
        SearchCriteria.SearchCriteriaItem statusIdItem = new SearchCriteria.SearchCriteriaItem();
        statusIdItem.Criterium = statusIdCriteria;
        statusIdItem.TableId = 1;
        statusIdItem.FieldId = 14;
        statusIdItem.ID = sc.Items.Count + 1;
        sc.Items.Add(statusIdItem);
        return sc;
    }

    /// <summary>
    /// Builds the ResultsCriteria for getting the containers associated to a record
    /// </summary>
    /// <param name="regid">RegID</param>
    /// <param name="batchid">BatchID</param>
    /// <returns>The ResultsCriteria</returns>
    public static ResultsCriteria GetInvContainersRC(int regid, int batchid)
    {
        string chemInvSchemaName = GetInventorySchemaName();

        ResultsCriteria rc = new ResultsCriteria();
        rc.Tables.Add(new ResultsCriteria.ResultsCriteriaTable(1));                             //CONTAINERS
        rc.Tables[0].Criterias.Add(new ResultsCriteria.Field(2));                               //CONTAINERID
        rc.Tables[0].Criterias.Add(new ResultsCriteria.Field(3));                               //REGNUMBER
        ResultsCriteria.Concatenation qtyAvailable = new ResultsCriteria.Concatenation();
        qtyAvailable.Alias = "QTYAVAILABLE";
        qtyAvailable.Operands.Add(new ResultsCriteria.Field(4));                              //QTYAVAILABLE
        ResultsCriteria.Concatenation qtyAvailableStr = new ResultsCriteria.Concatenation();
        qtyAvailableStr.Alias = "QTYAVAILABLE";
        qtyAvailableStr.Operands.Add(qtyAvailable);
        qtyAvailableStr.Operands.Add(new ResultsCriteria.Literal("' '"));
        qtyAvailableStr.Operands.Add(new ResultsCriteria.Field(12));
        rc.Tables[0].Criterias.Add(qtyAvailableStr);                                            //QTYAVAILABLE
        rc.Tables[0].Criterias.Add(new ResultsCriteria.Field(5));                               //CONTAINERTYPE
        ResultsCriteria.Concatenation containerSizeStr = new ResultsCriteria.Concatenation();
        containerSizeStr.Alias = "CONTAINERSIZE";
        containerSizeStr.Operands.Add(new ResultsCriteria.Field(6));
        containerSizeStr.Operands.Add(new ResultsCriteria.Literal("' '"));
        containerSizeStr.Operands.Add(new ResultsCriteria.Field(12));
        rc.Tables[0].Criterias.Add(containerSizeStr);                                           //CONTAINERSIZE
        ResultsCriteria.SQLFunction locationPath = new ResultsCriteria.SQLFunction();
        locationPath.Alias = "LOCATIONPATH";
        locationPath.FunctionName = string.Format("{0}.GUIUTILS.GETLOCATIONPATH", chemInvSchemaName);
        locationPath.Parameters.Add(new ResultsCriteria.Field(8));
        ResultsCriteria.Concatenation locationText = new ResultsCriteria.Concatenation();
        locationText.Alias = "LOCATION";
        locationText.Operands.Add(new ResultsCriteria.Literal("q'[<a title=\"]'"));
        locationText.Operands.Add(locationPath);
        locationText.Operands.Add(new ResultsCriteria.Literal("q'[\">]'"));
        locationText.Operands.Add(new ResultsCriteria.Field(7));
        locationText.Operands.Add(new ResultsCriteria.Literal("q'[</a>]'"));
        rc.Tables[0].Criterias.Add(locationText);                                                   //LOCATION
        rc.Tables[0].Criterias.Add(new ResultsCriteria.Field(8));                               //LOCATIONID
        rc.Tables[0].Criterias.Add(new ResultsCriteria.Field(9));                               //REGID
        rc.Tables[0].Criterias.Add(new ResultsCriteria.Field(10));                              //BATCHID
        rc.Tables[0].Criterias.Add(new ResultsCriteria.Field(17));                              //BATCHID
        rc.Tables[0].Criterias.Add(new ResultsCriteria.Field(18));                              //BATCHID 
        rc.Tables[0].Criterias.Add(new ResultsCriteria.Field(11));                              //REGBATCHID
        // /Cheminv/GUI/RequestSample.asp?action=create&ContainerID={0}&LocationID={1}&UOMAbv={3}&QtyRequired={4}&BatchID={5}&ContainerBarcode={6}&allowContainerRequest=1&allowSampleRequest=1
        ResultsCriteria.Concatenation requestSample = new ResultsCriteria.Concatenation();
        requestSample.Alias = "REQUEST_URL";
        requestSample.Operands.Add(new ResultsCriteria.Literal(string.Format("'{0}{1}'", GetContainersRequestMaterialURL(), RegUtilities.GetContainersRequestMaterialURL().Contains("?") ? "&" : "?")));
        requestSample.Operands.Add(new ResultsCriteria.Literal("'ContainerID='"));
        requestSample.Operands.Add(new ResultsCriteria.Field(2));
        requestSample.Operands.Add(new ResultsCriteria.Literal("'&UOMAbv='"));
        requestSample.Operands.Add(new ResultsCriteria.Field(12));
        requestSample.Operands.Add(new ResultsCriteria.Literal("'&QtyRequired='"));
        requestSample.Operands.Add(qtyAvailable);
        requestSample.Operands.Add(new ResultsCriteria.Literal("'&BatchID1='"));
        requestSample.Operands.Add(new ResultsCriteria.Field(13));
        requestSample.Operands.Add(new ResultsCriteria.Literal("'&BatchID2='"));
        requestSample.Operands.Add(new ResultsCriteria.Field(17));
        requestSample.Operands.Add(new ResultsCriteria.Literal("'&BatchID3='"));
        requestSample.Operands.Add(new ResultsCriteria.Field(18));
        requestSample.Operands.Add(new ResultsCriteria.Literal("'&ContainerBarcode='"));
        if (GetShowRequestFromContainer())
        {
            requestSample.Operands.Add(new ResultsCriteria.Field(16));
            requestSample.Operands.Add(new ResultsCriteria.Literal("'&allowContainerRequest=1&allowSampleRequest=1'"));
        }
        else
        {
            requestSample.Operands.Add(new ResultsCriteria.Field(16));
            requestSample.Operands.Add(new ResultsCriteria.Literal("'&allowSampleRequest=1'"));
        }
        rc.Tables[0].Criterias.Add(requestSample);                                              //Request_Sample
        rc.Tables[0].Criterias.Add(new ResultsCriteria.Field(15));                              //COMPOUNDID
        ResultsCriteria.SQLFunction getContainersAmountStr = new ResultsCriteria.SQLFunction();
        getContainersAmountStr.FunctionName = string.Format("{0}.GUIUTILS.GETCONTAINERSAMOUNTSTRING", chemInvSchemaName);
        getContainersAmountStr.Alias = "TotalQtyAvailable";
        getContainersAmountStr.Parameters.Add(new ResultsCriteria.Literal(regid.ToString()));
        if (batchid > 0)
            getContainersAmountStr.Parameters.Add(new ResultsCriteria.Literal(batchid.ToString()));
        rc.Tables[0].Criterias.Add(getContainersAmountStr);                                     //TotalQtyAvailable
        ResultsCriteria.Concatenation barcodeLink = new ResultsCriteria.Concatenation();
        barcodeLink.Alias = "BARCODE";
        barcodeLink.Operands.Add(new ResultsCriteria.Literal("'<a href=\"'"));
        barcodeLink.Operands.Add(new ResultsCriteria.Literal(string.Format("'{0}{1}'", GetViewContainerURL(), RegUtilities.GetViewContainerURL().Contains("?") ? "&" : "?")));
        barcodeLink.Operands.Add(new ResultsCriteria.Literal("'GetData=db&hideMenu=true&ContainerID='"));
        barcodeLink.Operands.Add(new ResultsCriteria.Field(2));
        barcodeLink.Operands.Add(new ResultsCriteria.Literal("'\" class=''LinkButton'' titleText=''Container Information'' onclick=\"ShowModalFrame(this.href, this.titleText, true);return false;\">'"));
        barcodeLink.Operands.Add(new ResultsCriteria.Field(16));
        barcodeLink.Operands.Add(new ResultsCriteria.Literal("'</a>'"));
        rc.Tables[0].Criterias.Add(barcodeLink);                                                //BARCODE - Link
        return rc;
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

    /// <summary>
    /// Method to get the relative path where the yui js refere are stored.
    /// </summary>
    /// <returns>A YUI base path</returns>
    public static string YUIJsRelativeFolder()
    {
        return "/COECommonResources/YUI/";
    }

    /// <summary>
    /// Method to get the relative path where the yui js refere are stored.
    /// </summary>
    /// <returns>A YUI base path</returns>
    public static string YUICssRelativeFolder(string theme)
    {
        return @"/COECommonResources/YUI/";
    }

    /// <summary>
    /// Converts an error message into a friendlier one.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ConvertToFriendly(string text)
    {
        string retVal = string.Empty;
        int wordSize = 500; //TODO: read from config
        if (!string.IsNullOrEmpty(text))
            retVal = text.Length > wordSize ? text.Substring(0, wordSize) + "..." : text;
        return retVal;
    }

    public enum PageState
    {
        None,
        ViewRecord,
        EditRecord,
        ViewComponent,
        EditComponent,
        EndComponent,
        ViewBatch,
        EditBatch,
        End,
        ViewSingleComponent,
        AddComponent,
        AddBatch,
        AddSingleCompound,
        EditSingleCompound,
        DisplayUserPreference
    }

    public enum FragmentForm
    {
        Edit,
        ReadOnly,
        Hidden,
    }

    /// <summary>
    /// Methods that indicates what Fragments coeforms to display
    /// </summary>
    /// <param name="sameIdentityBatch"></param>
    /// <param name="state"></param>
    /// <param name="isSingle"></param>
    /// <returns></returns>
    public static FragmentForm ShowFragmentsInRO(bool sameIdentityBatch, PageState state, bool isSingle, string CurRegType)
    {
        FragmentForm retVal = FragmentForm.Hidden;
        //Singles with SameIdentityBatch = true.


        switch (CurRegType.ToUpper())
        {
            case "BOTH":
                if (isSingle)
                {
                    if (sameIdentityBatch)
                    {
                        if (state == PageState.AddBatch)
                            retVal = FragmentForm.Hidden;
                        else if (state == PageState.EditComponent || state == PageState.AddComponent)
                            retVal = FragmentForm.Edit;
                        else if (state == PageState.EditBatch)
                            retVal = FragmentForm.Hidden;
                        else if (state == PageState.ViewBatch)
                            retVal = FragmentForm.Hidden;
                        else if (state == PageState.ViewComponent)
                            retVal = FragmentForm.ReadOnly;
                        else if (state == PageState.EndComponent)
                            retVal = FragmentForm.ReadOnly;
                        else if (state == PageState.AddSingleCompound || state == PageState.EditSingleCompound)
                            retVal = FragmentForm.Edit;
                    }
                    else
                    {
                        //Singles with SameIdentityBatch = false.
                        if (state == PageState.AddBatch)
                            retVal = FragmentForm.Edit;
                        else if (state == PageState.EditComponent || state == PageState.AddComponent)
                            retVal = FragmentForm.Hidden;
                        else if (state == PageState.EditBatch)
                            retVal = FragmentForm.Edit;
                        else if (state == PageState.ViewBatch)
                            retVal = FragmentForm.ReadOnly;
                        else if (state == PageState.ViewComponent)
                            retVal = FragmentForm.Hidden;
                        else if (state == PageState.AddSingleCompound || state == PageState.EditSingleCompound)
                            retVal = FragmentForm.Edit;
                    }
                }
                else
                {
                    if (state == PageState.AddBatch)
                        retVal = FragmentForm.Hidden;
                    else if (state == PageState.EditComponent || state == PageState.AddComponent)
                        retVal = FragmentForm.Edit;
                    else if (state == PageState.EditBatch)
                        retVal = FragmentForm.Hidden;
                    else if (state == PageState.ViewBatch)
                        retVal = FragmentForm.Hidden;
                    else if (state == PageState.ViewComponent)
                        retVal = FragmentForm.ReadOnly;
                    else if (state == PageState.EndComponent)
                        retVal = FragmentForm.ReadOnly;
                    else if (state == PageState.AddSingleCompound || state == PageState.EditSingleCompound)
                        retVal = FragmentForm.ReadOnly;
                }
                break;
            case "MIXTURE":
                if (sameIdentityBatch)
                {
                    if (state == PageState.AddBatch)
                        retVal = FragmentForm.Hidden;
                    else if (state == PageState.EditComponent || state == PageState.AddComponent)
                        retVal = FragmentForm.Edit;
                    else if (state == PageState.EditBatch)
                        retVal = FragmentForm.Hidden;
                    else if (state == PageState.ViewBatch)
                        retVal = FragmentForm.Hidden;
                    else if (state == PageState.ViewComponent)
                        retVal = FragmentForm.ReadOnly;
                    else if (state == PageState.EndComponent)
                        retVal = FragmentForm.ReadOnly;
                    else if (state == PageState.AddSingleCompound || state == PageState.EditSingleCompound)
                        retVal = FragmentForm.Hidden;
                }
                else
                {
                    //Singles with SameIdentityBatch = false.
                    if (state == PageState.AddBatch)
                        retVal = FragmentForm.Edit;
                    else if (state == PageState.EditComponent || state == PageState.AddComponent)
                        retVal = FragmentForm.Hidden;
                    else if (state == PageState.EditBatch)
                        retVal = FragmentForm.Edit;
                    else if (state == PageState.ViewBatch)
                        retVal = FragmentForm.ReadOnly;
                    else if (state == PageState.ViewComponent)
                        retVal = FragmentForm.Hidden;
                    else if (state == PageState.AddSingleCompound || state == PageState.EditSingleCompound)
                        retVal = FragmentForm.Hidden;

                }
                break;
            case "COMPONENT":
                if (sameIdentityBatch)
                {
                    if (state == PageState.AddBatch)
                        retVal = FragmentForm.Hidden;
                    else if (state == PageState.EditComponent || state == PageState.AddComponent)
                        retVal = FragmentForm.Edit;
                    else if (state == PageState.EditBatch)
                        retVal = FragmentForm.Hidden;
                    else if (state == PageState.ViewBatch)
                        retVal = FragmentForm.Hidden;
                    else if (state == PageState.ViewComponent)
                        retVal = FragmentForm.ReadOnly;
                    else if (state == PageState.EndComponent)
                        retVal = FragmentForm.ReadOnly;
                    else if (state == PageState.AddSingleCompound || state == PageState.EditSingleCompound)
                        retVal = FragmentForm.Edit;
                }
                else
                {
                    //Singles with SameIdentityBatch = false.
                    if (state == PageState.AddBatch)
                        retVal = FragmentForm.Edit;
                    else if (state == PageState.EditComponent || state == PageState.AddComponent)
                        retVal = FragmentForm.Hidden;
                    else if (state == PageState.EditBatch)
                        retVal = FragmentForm.Edit;
                    else if (state == PageState.ViewBatch)
                        retVal = FragmentForm.ReadOnly;
                    else if (state == PageState.ViewComponent)
                        retVal = FragmentForm.Hidden;
                    else if (state == PageState.AddSingleCompound || state == PageState.EditSingleCompound)
                        retVal = FragmentForm.Edit;
                }
                if (state == PageState.DisplayUserPreference)
                    retVal = FragmentForm.Hidden;
                break;
        }
        return retVal;
    }

    //0806modified
    public static string GetClipboardMessageByException(Exception ex)
    {
        StringWriter sw = new StringWriter();
        COETextExceptionFormatter formatter = new COETextExceptionFormatter(sw, ex);
        formatter.Format();
        return sw.GetStringBuilder().ToString();
    }
    //0806modified


    /// <summary>
    /// Returns the projects grids styles according RLS Status.
    /// </summary>
    /// <param name="rlsStatus">The rls status</param>
    /// <returns>A literal that contain the cssclasses acording rls status</returns>
    public static Literal GetRLSCSSClass(RLSStatus rlsStatus)
    {
        Literal RLSLabels = new Literal();

        StringBuilder rlsLabelsBuilder = new StringBuilder();

        rlsLabelsBuilder.Append("<style>");

        switch (rlsStatus)
        {
            case RLSStatus.Off:
                rlsLabelsBuilder.Append(".RLSRegLabel { } .RLSBatchLabel { }</style>");
                break;
            case RLSStatus.RegistryLevelProjects:
                rlsLabelsBuilder.Append(".RLSRegLabel {color:Red; } .RLSBatchLabel { }</style>");
                break;
            case RLSStatus.BatchLevelProjects:
                rlsLabelsBuilder.Append(".RLSRegLabel { } .RLSBatchLabel {color:Red; }</style>");
                break;
        }

        RLSLabels.Text = rlsLabelsBuilder.ToString();

        return RLSLabels;
    }

    #endregion

    #region Enums

    public enum Groups
    {
        CBV,
        DocMgr,
        Misc,
        Inventory,
        RegAdmin,
    }

    #endregion
}

/// <summary>
/// Class that stores some of the required information to restore a search.
/// </summary>
public class SearchedInfo
{
    #region Variables

    private int _hitListID = -1;
    private int _currentPage = -1;
    private CambridgeSoft.COE.Framework.HitListType _hitListType = CambridgeSoft.COE.Framework.HitListType.TEMP;

    #endregion

    #region Variables

    /// <summary>
    /// Id of the HitList
    /// </summary>
    public int HitListID
    {
        get { return _hitListID; }
    }

    /// <summary>
    /// Shown page number 
    /// </summary>
    public int CurrentPage
    {
        get { return _currentPage; }
    }

    /// <summary>
    /// Type of HitList (Temp, etc)
    /// </summary>
    public CambridgeSoft.COE.Framework.HitListType HitListType
    {
        get { return _hitListType; }
    }

    #endregion

    #region Contructors

    private SearchedInfo(int hitListID, int currentPage, CambridgeSoft.COE.Framework.HitListType hitListType)
    {
        _hitListID = hitListID;
        _currentPage = currentPage;
        _hitListType = hitListType;
    }

    #endregion

    #region Public Methods

    public static SearchedInfo Create(int hitlistID, int currentPage, CambridgeSoft.COE.Framework.HitListType hitListType)
    {
        return new SearchedInfo(hitlistID, currentPage, hitListType);
    }

    #endregion
}
