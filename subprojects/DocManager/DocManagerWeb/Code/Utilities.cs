using System;
using System.Data;
using System.Configuration;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.GUIShell;
using System.IO;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using System.Diagnostics;

/// <summary>
/// Generic class for multiple purposes related with COEManager App.
/// </summary>
public class Utilities
{

    #region Properties

    public static string ThemesCommonImagesPath
    {
        get { return "~/App_Themes/Common/Images/"; }
    }

    #endregion

    public Utilities() { }

    /// <summary>
    /// Here we can clean the string from malicious characters as SQL Inyection, etc...
    /// </summary>
    /// <param name="stringToClean">Dirty string</param>
    /// <returns>Clean string</returns>
    public static string CleanString(string stringToClean)
    {
        return stringToClean;
    }

    public static void CleanSession(HttpSessionState Session)
    {
        Session.Abandon();
    }

    /// <summary>
    /// Method to write messages to our selected Log File. For GuiShell Pages the selected Log is Trace.axd file.
    /// </summary>
    /// <param name="messageType">Type of message</param>
    /// <param name="message">String to write to Log (For instance: a Method Name)</param>
    public static void WriteToAppLog(GUIShellTypes.LogMessageType messageType, string message)
    {
        //if (HttpContext.Current.Trace.IsEnabled)
        //{
        //    switch (messageType)
        //    {
        //        //When a method is begining to process.
        //        case GUIShellTypes.LogMessageType.BeginMethod:
        //            GUIShellUtilities.WriteToLog(GUIShellTypes.LogsCategories.Information,
        //                                            string.Format(, message),
        //                                            HttpContext.Current.Application["AppName"].ToString());
        //            break;

        //        //When a method is ending to process.
        //        case GUIShellTypes.LogMessageType.EndMethod:
        //            GUIShellUtilities.WriteToLog(GUIShellTypes.LogsCategories.Information,
        //                                            string.Format(Resources.En_Resources.EndProcessingMethod_Label_Text, message),
        //                                            HttpContext.Current.Application["AppName"].ToString());
        //            break;

        //        case GUIShellTypes.LogMessageType.InvalidPage:
        //            GUIShellUtilities.WriteToLog(GUIShellTypes.LogsCategories.Information,
        //                                            message,
        //                                            HttpContext.Current.Application["AppName"].ToString());
        //            break;
        //    }
        //}
    }

    public static string ImagesBaseRelativeFolder()
    {
        string retVal = string.Empty;
        if (HttpContext.Current.Handler != null)
        {
            if (HttpContext.Current.Application[Constants.ImagesFolder] != null)
                retVal += string.Format(HttpContext.Current.Application[Constants.ImagesFolder].ToString(), ((Page)HttpContext.Current.Handler).StyleSheetTheme);
            else
                retVal += @"~/App_Themes/" + ((Page)HttpContext.Current.Handler).StyleSheetTheme + @"/Images/";
        }
        return retVal;
    }

    public static string ImagesBaseFullFolder(string styleSheetTheme)
    {
        string retVal = HttpContext.Current.Request.Url.Scheme
                        + "://"
                        + HttpContext.Current.Request.Url.Authority
                        + HttpContext.Current.Request.ApplicationPath;
        if (HttpContext.Current.Application[Constants.ImagesFolder] != null)
            retVal += string.Format(HttpContext.Current.Application[Constants.ImagesFolder].ToString().Substring(1), styleSheetTheme);
        else
            retVal += @"/App_Themes/" + styleSheetTheme + @"/Images/";
        return retVal;
    }

    public static string ImagesIconLibRelativeFolder(Constants.IconLibrary category)
    {
        string imageSubType = HttpContext.Current.Application[Constants.ImagesSubType] != null ? HttpContext.Current.Application[Constants.ImagesSubType].ToString() : "Aqua";
        string imagesFormat = ImagesIconLibFormat();
        string imagesSize = HttpContext.Current.Application[Constants.ImagesSize] != null ? HttpContext.Current.Application[Constants.ImagesSize].ToString() : "24";
        string imagesIconLibName = HttpContext.Current.Application[Constants.ImagesLibName] != null ? HttpContext.Current.Application[Constants.ImagesLibName].ToString() : "IconLib";
        string retVal = ImagesBaseRelativeFolder() + imagesIconLibName + "/";
        switch (category)
        {
            case Constants.IconLibrary.Core_Collection:
                retVal += category.ToString()
                        + @"/Core_" + imageSubType
                        + @"/" + imagesFormat
                        + @"/" + imagesSize + @"/";
                break;
            case Constants.IconLibrary.Database_Collection:
                retVal += category.ToString()
                        + @"/Database_" + imageSubType
                        + @"/" + imagesFormat
                        + @"/" + imagesSize + @"/";
                break;
            case Constants.IconLibrary.Network_Collection:
                retVal += category.ToString()
                        + @"/Network_" + imageSubType
                        + @"/" + imagesFormat
                        + @"/" + imagesSize + @"/";
                break;
            case Constants.IconLibrary.Vista_Business_Collection:
                retVal += category.ToString()
                        + @"/" + imagesFormat
                        + @"/" + imagesSize + @"/";
                break;
            case Constants.IconLibrary.Vista_Collection:
                retVal += category.ToString()
                        + @"/Vista_" + imagesFormat
                        + @"/" + imagesSize + @"/";
                break;
        }
        return retVal;
    }

    public static string ImagesIconLibFullFolder(Constants.IconLibrary category, string currentTheme)
    {
        string imageSubType = HttpContext.Current.Application[Constants.ImagesSubType] != null ? HttpContext.Current.Application[Constants.ImagesSubType].ToString() : "Aqua";
        string imagesFormat = ImagesIconLibFormat();
        string imagesSize = HttpContext.Current.Application[Constants.ImagesSize] != null ? HttpContext.Current.Application[Constants.ImagesSize].ToString() : "24";
        string imagesIconLibName = HttpContext.Current.Application[Constants.ImagesLibName] != null ? HttpContext.Current.Application[Constants.ImagesLibName].ToString() : "IconLib";
        string retVal = ImagesBaseFullFolder(currentTheme) + imagesIconLibName + "/";
        switch (category)
        {
            case Constants.IconLibrary.Core_Collection:
                retVal += category.ToString()
                        + @"/Core_" + imageSubType
                        + @"/" + imagesFormat
                        + @"/" + imagesSize + @"/";
                break;
            case Constants.IconLibrary.Database_Collection:
                retVal += category.ToString()
                        + @"/Database_" + imageSubType
                        + @"/" + imagesFormat
                        + @"/" + imagesSize + @"/";
                break;
            case Constants.IconLibrary.Network_Collection:
                retVal += category.ToString()
                        + @"/Network_" + imageSubType
                        + @"/" + imagesFormat
                        + @"/" + imagesSize + @"/";
                break;
            case Constants.IconLibrary.Vista_Business_Collection:
                retVal += category.ToString()
                        + @"/" + imagesFormat
                        + @"/" + imagesSize + @"/";
                break;
            case Constants.IconLibrary.Vista_Collection:
                retVal += category.ToString()
                        + @"/Vista_" + imagesFormat
                        + @"/" + imagesSize + @"/";
                break;
        }
        return retVal;
    }

    public static string ImagesIconLibFormat()
    {
        return HttpContext.Current.Application[Constants.ImagesFormat] != null ? HttpContext.Current.Application[Constants.ImagesFormat].ToString() : "PNG";
    }

    /// <summary>
    /// Method to find a control inside a ContentPlaceHolder given a controlID
    /// </summary>
    /// <param name="controlID">ControlId of the control to search</param>
    /// <returns>The found control or null in case of error</returns>
    public static Control FindControl(string controlID)
    {
        Control retVal = null;
        if (HttpContext.Current.Handler != null)
        {
            if (((Page)HttpContext.Current.Handler).HasControls())
                retVal = ((Control)((Page)HttpContext.Current.Handler).Controls[0].FindControl(Constants.ContentPlaceHolderID)).FindControl(controlID);
        }

        return retVal;
    }

    /// <summary>
    /// Method to get the relative path where the yui js refere are stored.
    /// </summary>
    /// <returns>A YUI base path</returns>
    public static string YUIJsRelativeFolder()
    {
        return HttpContext.Current.Request.ApplicationPath + @"/Forms/Public/JScripts/YUI/";
    }

    /// <summary>
    /// Method to get the relative path where the yui js refere are stored.
    /// </summary>
    /// <returns>A YUI base path</returns>
    public static string YUICssRelativeFolder(string theme)
    {
        return @"~/App_Themes/" + theme + "/YUI/";
    }

    /// <summary>
    /// Method to get the relative path where the yui js refere are stored.
    /// </summary>
    /// <returns>A YUI base path</returns>
    public static string YUICssRelativeFolder(string theme, string app)
    {
        return "/" + app + "/App_Themes/" + theme + "/YUI/";
    }

    /// <summary>
    /// Gets the formatted url where the save to menu should point to.
    /// </summary>
    /// <returns></returns>
    public static string GetSaveToURL()
    {
        return ConfigurationManager.AppSettings[Constants.SaveToURL] != null ? ConfigurationManager.AppSettings[Constants.SaveToURL].ToString() : string.Empty;
    }

    public static string GetDeleteURL()
    {
        return ConfigurationManager.AppSettings[Constants.DeleteURL] != null ? ConfigurationManager.AppSettings[Constants.DeleteURL].ToString() : string.Empty;
    }

    /// <summary>
    /// Returns a COEFormId given the name
    /// </summary>
    /// <param name="formId">Name of the form</param>
    /// <returns>ID of the COEForm</returns>
    public static int GetCOEFormID(string formId)
    {
        int retVal = -1;
        if (!string.IsNullOrEmpty(GetConfigSetting("Document", formId)))
            int.TryParse(GetConfigSetting("Document", formId), out retVal);
        return retVal;
    }

    /// <summary>
    /// Gets a DocManager setting
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
    /// Gets the display left panel visibility from configuration.
    /// </summary>
    /// <returns>A boolean indicating the visibility of the left panel section</returns>
    /// <remarks>It reads the configuration settings for DocManager</remarks>
    public static bool GetDisplayLeftPanelVisibility()
    {
        bool retVal = true;
        string configSet = GetConfigSetting("MISC", "ShowLeftPanel");
        if (!string.IsNullOrEmpty(configSet))
            bool.TryParse(configSet, out retVal);
        return retVal;
    }

    /// <summary>
    /// Converts an error message into a friendlier one.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ConvertToFriendly(string text)
    {
        string retVal = string.Empty;
        int wordSize = 45; //TODO: read from config
        if (!string.IsNullOrEmpty(text))
            retVal = text.Length > wordSize ? text.Substring(0, wordSize) + "..." : text;
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
    /// Returns the Application name setting.
    /// </summary>
    /// <returns>App name</returns>
    public static string GetApplicationName()
    {
        return HttpContext.Current.Application[Constants.AppName] != null ?
                HttpContext.Current.Application[Constants.AppName].ToString().ToUpper() : string.Empty;
    }

    /// <summary>
    /// Returns the URL to the permanent search registries form including formID and other required params.
    /// </summary>
    /// <returns>A URL</returns>
    public static string GetSearchPermURL()
    {
        string url = GUIShellUtilities.GetSearchEngineURL();
        url += string.Format("?FormGroupId={0}&embed=ChemBioViz&AppName={1}&KeepRecordCountSynchronized={2}&AllowFullScan={4}&MarkedHitsMax={5}",
                            GetConfigSetting("Document", "SearchDocFormGroupId"),
                            HttpContext.Current.Application[Constants.AppName].ToString(),
                            bool.TrueString,
                            bool.TrueString,
                            bool.TrueString,
                            GetConfigSetting("DocMgr", "MarkedHitsMax")
                            );

        if (HttpContext.Current.Session[Constants.PermSearchedInfo] != null)
        {
            if (HttpContext.Current.Session[Constants.PermSearchedInfo] is SearchedInfo)
            {
                url += "&" + "HitListID=" + ((SearchedInfo)HttpContext.Current.Session[Constants.PermSearchedInfo]).HitListID.ToString();
                url += "&" + "HitListType=" + ((SearchedInfo)HttpContext.Current.Session[Constants.PermSearchedInfo]).HitListType.ToString();
            }
        }
        return url;
    }
    
}

/// <summary>
/// Class to define strings, paths, etc that can be usefull for the entire App.
/// </summary>
public class Constants
{
    //public Constants() { }

    public const string FooterSeparator = " | ";

    #region Controls IDs

    public const string ErrorAreaUCID = "ErrorAreaUserControl";
    public const string ConfirmationAreaUCID = "ConfirmationAreaUserControl";
    public const string MainFormName = "MainForm";
    public const string MainBodyName = "MainBody";
    public const string ContentPlaceHolderID = "ContentPlaceHolder";
    public const string MessagesAreaUCID = "MessagesAreaUserControl";
    public const string ErrorYUIPanelID = "ErrorControlYUI";

    #endregion

    #region Session Vars


    #endregion

    #region Session Keys

    // Session Keys
    public const string DocumentObject_Session = "DocumentObject";
    public const string NewDocumentObject_Session = "NewDocumentObject";
    public const string PermSearchedInfo = "PermSearchedInfo";

    #endregion    

    #region ViewState Vars

    // ViewState Names
    public const string SelectedNode_ViewState = "SelectedNode";
    public const string CurrentPageState_ViewState = "CurrentPageState";
    public const string LastNodeEditted_ViewState = "LastNodeEditted";

    #endregion

    #region URLs & Paths Strings

    public const string PublicCommonJScriptsPath = "/Forms/Public/JScripts/CommonScripts.js";
    public const string PublicContentAreaFolder = "/Forms/Public/ContentArea/";
    public const string SearchContentAreaFolder = "/Forms/Search/ContentArea";

    #endregion

    #region Public Methods

    public static string GetPublicCommonJSsPath(string appName)
    {
        return !string.IsNullOrEmpty(appName) ? "/" + appName + PublicCommonJScriptsPath : string.Empty;
    }

    public static string GetPublicCommonJSsPath()
    {
        return "~" + PublicCommonJScriptsPath;
    }

    public static string GetPublicContentAreaFolder(string appName)
    {
        return !string.IsNullOrEmpty(appName) ? "/" + appName + PublicContentAreaFolder : string.Empty;
    }

    public static string GetPublicContentAreaFolder()
    {
        return "~" + PublicContentAreaFolder;
    }

    public static string GetSearchContentAreaFolder(string appName)
    {
        return !string.IsNullOrEmpty(appName) ? "/" + appName + SearchContentAreaFolder : string.Empty;
    }

    public static string GetSearchContentAreaFolder()
    {
        return "~" + SearchContentAreaFolder;
    }    

    #endregion

    #region Variables

    public const string ButtonsImagesFolder = "ButtonsImgFol";
    public const string AppName = "AppName=";
    public const string AppPagesTitle = "AppPagesTitle";
    public const string PageControlsSettingsPath = "PageControlsSettingsPath";
    public const string ImagesLibName = "ImgLibName";
    public const string ImagesCategory = "ImgsCat";
    public const string ImagesSubType = "ImgsSubType";
    public const string ImagesFormat = "ImgsFormat";
    public const string ImagesSize = "ImgsSize";
    public const string ImagesCollection = "ImgsColl";
    public const string ImagesFolder = "ImgFolder";
    public const string ImageName = "ImgName";
    public const string TypeOfButton = "TypeBtn";
    public const string CssClass = "CssClass";
    public const string StructuresCount = "StrucCnt";
    public const string SaveToURL = "SaveToURL";
    public const string DeleteURL = "DeleteURL";

    #endregion

    #region Enums

    public enum MessagesCode
    {
        None = 0,
        Unknown = 1,
        InvalidEnteredInput = 2,
        InvalidHttpRequest = 6,
    }

    public enum ButtonSize
    {
        Small,
        Big,
    }

    public enum IconLibrary
    {
        Core_Collection,
        Database_Collection,
        Network_Collection,
        Vista_Business_Collection,
        Vista_Collection,
        None,
    }

    #endregion
    
    // Exception Policies
    public const string DOC_GUI_POLICY = "DocUIExceptionPolicy";

    // Query-string Keys
    public const string Ticket_UrlParameter = "ticket";
    public const string SavedObjectId_UrlParameter = "SavedObjectId";
    public const string FormGroup_UrlParameter = "FormGroup";
    public const string CurrentPageState_UrlParameter = "CurrentPageState";
    public const string DocId_UrlParameter = "DocId";
    public const string BinaryWriter_UrlParameter = "disposition";

    
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