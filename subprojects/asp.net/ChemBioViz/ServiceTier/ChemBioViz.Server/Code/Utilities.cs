using System;
using System.Data;
using System.Configuration;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Collections;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Caching;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.COEPageControlSettingsService;

/// <summary>
/// Generic class for multiple purposes related with COEManager App.
/// </summary>
public class Utilities
{
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
                retVal += string.Format(HttpContext.Current.Application[Constants.ImagesFolder].ToString(), ((System.Web.UI.Page)HttpContext.Current.Handler).StyleSheetTheme);
            else
                retVal += @"~/App_Themes/" + ((System.Web.UI.Page)HttpContext.Current.Handler).StyleSheetTheme + @"/Images/";
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
    public static System.Web.UI.Control FindControl(string controlID)
    {
        System.Web.UI.Control retVal = null;
        if (HttpContext.Current.Handler != null)
        {
            if (((System.Web.UI.Page)HttpContext.Current.Handler).HasControls())
                retVal = ((System.Web.UI.Control)((System.Web.UI.Page)HttpContext.Current.Handler).Controls[0].FindControl(Constants.ContentPlaceHolderID)).FindControl(controlID);
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
    /// Method to get the relative path where the yui js refere are stored.
    /// </summary>
    /// <returns>A YUI base path</returns>
    public static string YUICssRelativeFolder(string theme, string app)
    {
        return "/COECommonResources/YUI/";
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
    /// Returns the PageSettings xml to be loaded as an object for further reference.
    /// </summary>
    /// <returns></returns>
    public static void SetCOEPageSettings(bool force)
    {
        if (HttpContext.Current.Session != null)
        {
            if (HttpContext.Current.Session[GUIShellTypes.COEPageSettings + COEAppName.Get().ToString()] == null || force)
            {
                CambridgeSoft.COE.Framework.COEPageControlSettingsService.ControlList ctrls = COEPageControlSettings.GetControlListToDisableForCurrentUser(GUIShellUtilities.GetApplicationName());
                if (ctrls != null)
                    HttpContext.Current.Session[GUIShellTypes.COEPageSettings + COEAppName.Get().ToString()] = ctrls;
            }
        }
    }

    public static void CleanLocalCache()
    {
        CambridgeSoft.COE.Framework.COEConfigurationService.COEConfigurationBO.RemoveLocalCache("Registration", "Registration");
    }

    /// <summary>
    /// Here we can clean the cache available in chembioviz APPDOMAIN [When any changes performed in registration]
    /// <Note>Trigger to method depends upon registration search call</Note>
    /// </summary>   
    public static void CleanCache()
    {
        try
        {
                // 4002
                ServerCache.Remove("4002", typeof(CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBO));
                ServerCache.Remove("4002", typeof(CambridgeSoft.COE.Framework.COEFormService.COEFormBO));

                // 4003
                ServerCache.Remove("4003", typeof(CambridgeSoft.COE.Framework.COEDataViewService.COEDataViewBO));
                ServerCache.Remove("4003", typeof(CambridgeSoft.COE.Framework.COEFormService.COEFormBO));

        }
        catch (Exception ex)
        {
            CambridgeSoft.COE.Framework.ExceptionHandling.COEExceptionDispatcher.HandleBLLException(ex, CambridgeSoft.COE.Framework.ExceptionHandling.COEExceptionDispatcher.Policy.LOG_ONLY);
        }
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

    #endregion

    #region Session Vars


    #endregion

    #region ViewState Vars

    #endregion

    #region URLs & Paths Strings

    private const string PublicCommonJScriptsPath = "/Forms/Public/JScripts/CommonScripts.js";
    private const string PublicContentAreaFolder = "/Forms/Public/ContentArea/";
    private const string SearchContentAreaFolder = "/Forms/Search/ContentArea";
    
    #endregion

    #region Query-string Keys
    public const string ClearCache = "ClearCache";
    #endregion
      

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
        NoEnoughPrivileges = 4,
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
}

