using System;
using System.Data;
using System.Configuration;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.COEDataViewService;
using System.Reflection;
using CambridgeSoft.COE.Framework.Common;
using Infragistics.WebUI.UltraWebNavigator;
using System.Text.RegularExpressions;
using Resources;
using Infragistics.WebUI.UltraWebGrid;
using CambridgeSoft.COE.Framework.COEConfigurationService;

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
    /// <exception cref="">InvalidInputText</exception>
    public static string CleanString(string stringToClean)
    {
        string retVal = string.Empty;
        if (!string.IsNullOrEmpty(stringToClean))
        {
            retVal = stringToClean.Trim();
            //Throw exception
            Regex expression = new Regex(Constants.ValidInputCharacters);
            if (!expression.IsMatch(stringToClean))
                throw new InvalidInputText();
            //retVal = CleanFromCSSAttack(retVal);
            //retVal = CleanFromCSSAttack(retVal);
        }
        return retVal;
    }  

    /// <summary>
    /// Here we can clean the string from malicious characters as SQL Inyection, etc...
    /// </summary>
    /// <param name="stringToClean"></param>
    /// <returns></returns>
    public static string CleanNameString(string stringToClean)
    {
        string retVal = string.Empty;
        if (!string.IsNullOrEmpty(stringToClean))
        {
            retVal = stringToClean.Trim();
            //Throw exception
            Regex expression = new Regex(Constants.ValidInputCharactersForName);
            if (!expression.IsMatch(stringToClean))
                throw new InvalidInputText();
            //retVal = CleanFromCSSAttack(retVal);
            //retVal = CleanFromCSSAttack(retVal);
        }
        return retVal;
    }

    /// <summary>
    /// Method to write messages to our selected Log File. For GuiShell Pages the selected Log is Trace.axd file.
    /// </summary>
    /// <param name="messageType">Type of message</param>
    /// <param name="message">String to write to Log (For instance: a Method Name)</param>
    public static void WriteToAppLog(GUIShellTypes.LogMessageType messageType, string message)
    {
        if (HttpContext.Current.Trace.IsEnabled)
        {
            switch (messageType)
            {
                //When a method is begining to process.
                case GUIShellTypes.LogMessageType.BeginMethod:
                    GUIShellUtilities.WriteToLog(GUIShellTypes.LogsCategories.Information,
                                                    string.Format(Resource.BeginProcessingMethod_Label_Text, message),
                                                    HttpContext.Current.Application["AppName"].ToString());
                    break;

                //When a method is ending to process.
                case GUIShellTypes.LogMessageType.EndMethod:
                    GUIShellUtilities.WriteToLog(GUIShellTypes.LogsCategories.Information,
                                                    string.Format(Resource.EndProcessingMethod_Label_Text, message),
                                                    HttpContext.Current.Application["AppName"].ToString());
                    break;

                case GUIShellTypes.LogMessageType.InvalidPage:
                    GUIShellUtilities.WriteToLog(GUIShellTypes.LogsCategories.Information,
                                                    message,
                                                    HttpContext.Current.Application["AppName"].ToString());
                    break;
            }
        }
    }

    /// <summary>
    /// Method to write messages to our selected Log File.
    /// </summary>
    /// <param name="logCategory">log Category</param>
    /// <param name="message">message</param>
    public static void WriteToAppLog(GUIShellTypes.LogsCategories logCategory, string message)
    {
        if (HttpContext.Current != null && HttpContext.Current.Trace != null && HttpContext.Current.Trace.IsEnabled)
        {
            GUIShellUtilities.WriteToLog(logCategory, message, HttpContext.Current.Application["AppName"].ToString());
        }
    }

    public static string ImagesBaseRelativeFolder()
    {
        string retVal = "~";
        if (HttpContext.Current.Handler != null)
        {
            if (HttpContext.Current.Application[Constants.ImagesFolder] != null)
                retVal += string.Format(HttpContext.Current.Application[Constants.ImagesFolder].ToString(), ((Page)HttpContext.Current.Handler).StyleSheetTheme);
            else
                retVal += @"/App_Themes/" + ((Page)HttpContext.Current.Handler).StyleSheetTheme + @"/Images/";
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
        return "/COECommonResources/YUI/";
    }

    /// <summary>
    /// Method to get the relative path where the css for theme.
    /// </summary>
    /// <returns>A css base path</returns>
    public static string CssRelativeFolder(string theme)
    {
        return @"~/App_Themes/" + theme + "/";
    }

    public static string ImagesBaseFullFolder(string styleSheetTheme)
    {
        string retVal = HttpContext.Current.Request.Url.Scheme
                        + "://"
                        + HttpContext.Current.Request.Url.Authority
                        + HttpContext.Current.Request.ApplicationPath;
        if (HttpContext.Current.Application[Constants.ImagesFolder] != null)
            retVal += string.Format(HttpContext.Current.Application[Constants.ImagesFolder].ToString(), styleSheetTheme);
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

    public static string GetPageURL(Constants.Validate itemToValidate)
    {
        string retVal = Constants.DvManagerContentAreaFolder;
        switch (itemToValidate)
        {
            case Constants.Validate.BaseTable:
                retVal += "SelectBaseTable.aspx";
                break;
            case Constants.Validate.Fields:
                retVal += "EditTableAndFields.aspx";
                break;
            case Constants.Validate.NameAndDescription:
                retVal += "EnterNameDescription.aspx";
                break;
            case Constants.Validate.Relationships:
                //retVal += "DefineRelationships.aspx";
                retVal += "EditTableAndFields.aspx";
                break;
            case Constants.Validate.Security:
                retVal += "Security.aspx";
                break;
            case Constants.Validate.Tables:
                retVal += "DataviewBoard.aspx";
                break;
            case Constants.Validate.TablesAndFields:
                retVal += "EditTableAndFields.aspx";
                break;
        }
        return retVal;
    }

    public static string GetImageButtonURL(Forms_Public_UserControls_ImageButton.TypeOfButtons type)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string retVal = string.Empty;
        switch (type)
        {
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.Back:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Core_Collection) +
                        "Arrow_Left_B" + "." + Utilities.ImagesIconLibFormat().ToLower(); ;
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.Cancel:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Core_Collection) +
                        "Cross_R" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.Next:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Core_Collection) +
                        "Arrow_Right_B" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.Save:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Vista_Business_Collection) +
                        "Save" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.Submit:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Vista_Business_Collection) +
                        "Save_As" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            //SM- CSBR-142993: Adding Ok event 
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.Ok:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Vista_Business_Collection) +
                        "Ok_Dialog_Btn" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.GoBack:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Vista_Business_Collection) +
                        "Ok_Btn" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.GroupAdd:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Vista_Business_Collection) +
                        "Add" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.UpdateFields:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Core_Collection) +
                         "Edit" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.UpdateTable:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Core_Collection) +
                         "Edit" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.Clear:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Core_Collection) +
                         "Clean_Up" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.Update:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Core_Collection) +
                         "Edit" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.Edit:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Core_Collection) +
                         "Edit" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.AddUser:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Database_Collection) +
                         "User_Add" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.RemoveUser:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Database_Collection) +
                         "User_Drop" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.AddRole:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Network_Collection) +
                         "Forward_3" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.RemoveRole:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Network_Collection) +
                         "Reply_3" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.Remove:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Database_Collection) +
                         "Delete_To_Bin" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.Delete:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Vista_Collection) +
                         "Delete" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.Invalid:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Vista_Collection) +
                         "Wrong" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.Valid:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Core_Collection) +
                         "Check" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.Resolve:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Vista_Collection) +
                         "Accessibility_Warning" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.CreateAlias:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Core_Collection) +
                         "Add_Green" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.NewRelationship:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Core_Collection) +
                         "Add_Green" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.Publish:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Database_Collection) +
                         "Database_Server" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.AddSchema:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Database_Collection) +
                         "Database_Register" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.EditRoleRoles:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Core_Collection) +
                         "Edit" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.EditRoleUsers:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Core_Collection) +
                         "Edit" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.Close:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Core_Collection) +
                         "Cross_R" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;
            case Forms_Public_UserControls_ImageButton.TypeOfButtons.Done:
                retVal = Utilities.ImagesIconLibRelativeFolder(Constants.IconLibrary.Core_Collection) +
                         "Cross_R" + "." + Utilities.ImagesIconLibFormat().ToLower();
                break;


        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
    }

    /// <summary>
    /// Method to get the DataKey (ID) of a node.
    /// </summary>
    /// <param name="nodeIndex">Index of the node in the array of nodes</param>
    /// <param name="DVBOID">Id of the COEDataviewBO</param>
    /// <param name="database">Database name where this node belongs</param>
    /// <returns>A formated string with the given information</returns>
    /// <remarks>This is the way we identify a Node (very important!)</remarks>
    public static string GetNodeDataKey(int nodeIndex, int DVBOID, string database)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string dataKey = Constants.DataKeysParam.INDEX.ToString() + Constants.DataKeysItemsSeparator + nodeIndex;
        dataKey += Constants.DataKeysParamSeparator + Constants.DataKeysParam.ID.ToString() + Constants.DataKeysItemsSeparator + DVBOID;
        dataKey += Constants.DataKeysParamSeparator + Constants.DataKeysParam.DATABASE.ToString() + Constants.DataKeysItemsSeparator + database;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return dataKey;
    }

    #region Tables Nodes Settings

    public static string FormatTableText(string name, string alias)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string retVal = String.Empty;
        if (!string.IsNullOrEmpty(alias))
            retVal = string.Format(Constants.TableText, name, FormatAlias(alias));
        else
            retVal = name;  //Name is mandatory (That's why we don't check)
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        retVal = System.Web.HttpUtility.HtmlDecode(retVal);
        retVal = System.Web.HttpUtility.HtmlDecode(retVal);
        return retVal;
    }

    public static string FormatAlias(string alias)
    {
        return alias.Length > Constants.AliasMaxLength ? alias.Substring(0, Constants.AliasMaxLength) + "..." : alias;
    }

    public static string FormatTableNodeDataKey(string id, string name, string alias, string database, bool action)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string dataKey = String.Empty;
        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(id))
        {
            dataKey = Constants.DataKeysParam.ID.ToString() + Constants.DataKeysItemsSeparator + id;
            dataKey += Constants.DataKeysParamSeparator + Constants.DataKeysParam.NAME.ToString() + Constants.DataKeysItemsSeparator + name;
            dataKey += Constants.DataKeysParamSeparator + Constants.DataKeysParam.DATABASE.ToString() + Constants.DataKeysItemsSeparator + database;
            if (!string.IsNullOrEmpty(alias))
                dataKey += Constants.DataKeysParamSeparator + Constants.DataKeysParam.ALIAS.ToString() + Constants.DataKeysItemsSeparator + alias;

            if (!action)
                dataKey += Constants.DataKeysParamSeparator + Constants.DataKeysParam.ACTION + Constants.DataKeysItemsSeparator + Constants.NodeAction.DoNothing.ToString();
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return dataKey;
    }

    public static string FormatTableToolTip(string name, string alias, string ID, string database)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string retVal = String.Empty;
        if (!string.IsNullOrEmpty(alias))
            retVal = string.Format(Constants.TableText, name, alias);
        else
            retVal = name;  //Name is mandatory (That's why we don't check)
        retVal = string.Format(Constants.TableToolTip, Resources.Resource.ID_Label_Text, ID,
                                                        Resources.Resource.Name_Label_Text, retVal,
                                                        Resources.Resource.Database_Label_Text, database);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;

    }

    #endregion

    #region Fields Node Settings

    public static string FormatFieldText(string name, string alias)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string retVal = String.Empty;
        if (!string.IsNullOrEmpty(alias))
            retVal = string.Format(Constants.TableText, name, alias);
        else
            retVal = name;  //Name is mandatory (That's why we don't check)
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        retVal = System.Web.HttpUtility.HtmlDecode(retVal);
        retVal = System.Web.HttpUtility.HtmlDecode(retVal);
        return retVal;
    }

    public static string FormatFieldNodeDataKey(int id, string name, string alias, bool action)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string dataKey = String.Empty;
        if (!string.IsNullOrEmpty(name) && id > 0)
        {
            dataKey = Constants.DataKeysParam.ID.ToString() + Constants.DataKeysItemsSeparator + id.ToString();
            dataKey += Constants.DataKeysParamSeparator + Constants.DataKeysParam.NAME.ToString() + Constants.DataKeysItemsSeparator + name;
            if (!string.IsNullOrEmpty(alias))
                dataKey += Constants.DataKeysParamSeparator + Constants.DataKeysParam.ALIAS.ToString() + Constants.DataKeysItemsSeparator + alias;
            if (!action)
                dataKey += Constants.DataKeysParamSeparator + Constants.DataKeysParam.ACTION + Constants.DataKeysItemsSeparator + Constants.NodeAction.DoNothing.ToString();
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return dataKey;
    }

    public static string FormatFieldToolTip(string ID, string name, string alias)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string retVal = String.Empty;
        if (!string.IsNullOrEmpty(alias))
            retVal = string.Format(Constants.FieldText, name, alias);
        else
            retVal = name;  //Name is mandatory (That's why we don't check)
        retVal = string.Format(Constants.FieldToolTip, Resources.Resource.ID_Label_Text, ID,
                                                        Resources.Resource.Name_Label_Text, retVal);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        retVal = System.Web.HttpUtility.HtmlDecode(retVal);
        retVal = System.Web.HttpUtility.HtmlDecode(retVal);
        return retVal;
    }

    #endregion

    #region DataViewBO Node Settings

    public static string FormatDataViewBONodeText(string name, string description, string database)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string retVal = String.Empty;
        int maxLength = 50;
        retVal = name.Length > maxLength ? name.Substring(0, maxLength) + "..." : name;
        if (!string.IsNullOrEmpty(description))
            retVal += description.Length > maxLength ? " (" + description.Substring(0, maxLength) + "...)" : " (" + description + ")";
        retVal += "-" + database.ToUpper();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
    }

    public static string FormatDataViewBONodeDataKey(string ID, bool action)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        String dataKey = Constants.DataKeysParam.ID.ToString() + Constants.DataKeysItemsSeparator + ID;
        if (!action)
            dataKey += Constants.DataKeysParamSeparator + Constants.DataKeysParam.ACTION + Constants.DataKeysItemsSeparator + Constants.NodeAction.DoNothing.ToString();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return dataKey;
    }

    /// <summary>
    /// Returns a well formated Tooltip string for a DataViewBO node in a tree.
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="database"></param>
    /// <returns></returns>
    /// <remarks>We just check those fields that can have very long as defined in the DB</remarks>
    public static string FormatDataViewBONodeTooltip(string ID, string name, string description, string database)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //string tempDescription = String.Empty;
        string tempName = String.Empty;
        int maxLength = 50;
        string tempDescription = description.Length > maxLength ? description.Substring(0, maxLength) + "..." : description;
        tempName = name.Length > maxLength ? name.Substring(0, maxLength) + "..." : name;
        string toolTip = Resources.Resource.ID_Label_Text + ": " +
                        ID + " " +
                        Resources.Resource.User_Label_Text + ": " +
                        tempName + " " +
                        Resources.Resource.Description_Label_Text + ": " +
                        tempDescription + " " +
                        Resources.Resource.Database_Label_Text + ": " +
                        database;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return toolTip;
    }

    #endregion

    public static string FormatDataViewNodeDataKey(string name, int id, bool action)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string dataKey = String.Empty;
        if (!string.IsNullOrEmpty(name) && id > int.MinValue)
        {
            dataKey = Constants.DataKeysParam.NAME.ToString() + Constants.DataKeysItemsSeparator + name;
            dataKey += Constants.DataKeysParamSeparator + Constants.DataKeysParam.ID.ToString() + Constants.DataKeysItemsSeparator + id.ToString();
            if (!action)
                dataKey += Constants.DataKeysParamSeparator + Constants.DataKeysParam.ACTION + Constants.DataKeysItemsSeparator + Constants.NodeAction.DoNothing.ToString();

        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return dataKey;
    }

    public static string FormatDataViewText(string name, string description)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string text = String.Empty;
        int nameLength = 25;
        if (!string.IsNullOrEmpty(name))
        {
            if (name.Length > nameLength)
                text = name.Substring(0, nameLength) + "...";
            else
                text = name;
        }
        if (!string.IsNullOrEmpty(description))
        {
            if (description.Length > nameLength)
                text += " (" + description.Substring(0, nameLength) + "..." + ")";
            else
                text += " (" + name + ")";
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return text;
    }

    public static string FormatRootNodeText(string rootNodeText)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string text = rootNodeText;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return text;
    }

    public static string FormatRootDataKey(string name, bool action)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string dataKey = Constants.DataKeysParam.NAME.ToString() + Constants.DataKeysItemsSeparator + name;
        if (!action)
            dataKey += Constants.DataKeysParamSeparator + Constants.DataKeysParam.ACTION + Constants.DataKeysItemsSeparator + Constants.NodeAction.DoNothing.ToString();
        else
            dataKey += Constants.DataKeysParamSeparator + Constants.DataKeysParam.ACTION + Constants.DataKeysItemsSeparator + Constants.NodeAction.Grouping.ToString();
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return dataKey;
    }

    /// <summary>
    /// Method to get the tooltip of a node.
    /// </summary>
    /// <param name="id">Id of the COEDataviewBO</param>
    /// <param name="userName">UserName of the COEDataviewBO</param>
    /// <returns>A formated string with the given info</returns>
    public static string GetNodeToolTip(int id, string userName)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string toolTip = Resources.Resource.ID_Label_Text + ": " +
                        id.ToString() + " " +
                        Resources.Resource.User_Label_Text + ": " +
                        userName;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return toolTip;
    }

    public static Node CreateTreeStructure(DataViewNode dataViewNode)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        bool nodeExpanded = true;
        Node node = new Node();
        if (dataViewNode != null)
        {
            node.ToolTip = dataViewNode.ToolTip;
            if (dataViewNode.Text.Length > 45)
                node.Text = dataViewNode.Text.Substring(0, 45) + "...";
            else
                node.Text = dataViewNode.Text;
            node.DataKey = dataViewNode.DataKey;
            node.Tag = node.DataKey;
            node.Expanded = nodeExpanded;
            if (!dataViewNode.Enabled)
            {
                node.CssClass = "ParentTableNode";
                node.Checked = true;
            }
            node.Hidden = !dataViewNode.Visible;
            if (dataViewNode.CheckBox && dataViewNode.Enabled)
            {
                node.CheckBox = CheckBoxes.True;
                node.Checked = dataViewNode.Checked;
            }
            if (dataViewNode.HasChilds)
            {
                foreach (DataViewNode dataViewChildNode in dataViewNode.DataViewItems)
                {
                    Node childNode = new Node();
                    childNode.ToolTip = dataViewChildNode.ToolTip;
                    if (dataViewChildNode.Text.Length > 42)
                        childNode.Text = dataViewChildNode.Text.Substring(0, 42) + "...";
                    else
                        childNode.Text = dataViewChildNode.Text;
                    childNode.DataKey = dataViewChildNode.DataKey;
                    childNode.Tag = childNode.DataKey;
                    childNode.Expanded = nodeExpanded;
                    if (!dataViewChildNode.Enabled)
                    {
                        childNode.CssClass = "ParentTableNode";
                        childNode.Checked = true;
                    }
                    childNode.Hidden = !dataViewChildNode.Visible;
                    if (dataViewChildNode.CheckBox && dataViewChildNode.Enabled)
                    {
                        childNode.CheckBox = CheckBoxes.True;
                        childNode.Checked = dataViewChildNode.Checked;
                    }
                    if (dataViewChildNode.HasChilds)
                    {
                        foreach (DataViewNode dataViewGrandChildNode in dataViewChildNode.DataViewItems)
                        {
                            Node grandChildNode = new Node();
                            grandChildNode.ToolTip = dataViewGrandChildNode.ToolTip;
                            if (dataViewGrandChildNode.Text.Length > 39)
                                grandChildNode.Text = dataViewGrandChildNode.Text.Substring(0, 39) + "...";
                            else
                                grandChildNode.Text = dataViewGrandChildNode.Text;
                            grandChildNode.DataKey = dataViewGrandChildNode.DataKey;
                            grandChildNode.Tag = grandChildNode.DataKey;
                            grandChildNode.Expanded = false;
                            if (!dataViewGrandChildNode.Enabled)
                            {
                                grandChildNode.CssClass = "ParentTableNode";
                                grandChildNode.Checked = true;
                            }
                            grandChildNode.Hidden = !dataViewGrandChildNode.Visible;
                            if (dataViewGrandChildNode.CheckBox && dataViewGrandChildNode.Enabled)
                            {
                                grandChildNode.CheckBox = CheckBoxes.True;
                                grandChildNode.Checked = dataViewGrandChildNode.Checked;
                            }
                            childNode.Nodes.Add(grandChildNode);
                        }
                    }
                    node.Nodes.Add(childNode);
                }
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return node;
    }

    public static bool IsActionNode(string dataKey)
    {
        bool retVal = true;
        if (dataKey.Contains(Constants.NodeAction.DoNothing.ToString()))
            retVal = false;
        return retVal;
    }

    /// <summary>
    /// Method to get a Parameter given a full DataKey
    /// </summary>
    /// <param name="dataKey">The ID of a Tree Node</param>
    /// <returns>The ID part of the DataKey</returns>
    public static string GetParamInDataKey(string dataKey, Constants.DataKeysParam parameter)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string retVal = String.Empty;
        if (!string.IsNullOrEmpty(dataKey))
        {
            string[] keys = dataKey.Split(Constants.DataKeysParamSeparator.ToCharArray());
            foreach (string currentKey in keys)
            {
                if (currentKey.IndexOf(parameter.ToString(), 0) > -1)
                {
                    retVal = currentKey.Split(Constants.DataKeysItemsSeparator.ToCharArray())[1];
                    break;
                }
            }
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
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
                CambridgeSoft.COE.Framework.COEPageControlSettingsService.ControlList ctrls = CambridgeSoft.COE.Framework.COEPageControlSettingsService.COEPageControlSettings.GetControlListToDisableForCurrentUser(GetApplicationName());
                if (ctrls != null) HttpContext.Current.Session[GUIShellTypes.COEPageSettings + COEAppName.Get().ToString()] = ctrls;
            }
        }
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
    /// returns MimeType value list for dropdownlist
    /// </summary>
    /// <returns></returns>
    public static ValueList GetMimeTypeValueList()
    {
        ValueList MimeTypeValueList = new ValueList();
        MimeTypeValueList.Style.CssClass = "MimeDropDownOnGrid";
        foreach (string item in Enum.GetNames(typeof(COEDataView.MimeTypes)))
        {
            MimeTypeValueList.ValueListItems.Add(item, item);
        }
        return MimeTypeValueList;
    }

    /// <summary>
    /// returns IndexType value list for dropdownlist.
    /// </summary>
    /// <returns></returns>
    public static ValueList GetIndexTypeValueList(ValidCartridgeReadOnlyBO cartridgeBO = null)
    {
        var needFilt = (cartridgeBO != null && cartridgeBO.ExistValidCartridgeTable);

        var indexTypeValueList = new ValueList();
        indexTypeValueList.Style.CssClass = "IndexDropDownOnGrid";
        foreach (var item in Enum.GetNames(typeof(COEDataView.IndexTypes)))
        {
            var cartridgeId = GetCartridgeID(item);
            if (needFilt && cartridgeId!=0)
            {
                if (!cartridgeBO.IsCartridgeValid(cartridgeId))
                {
                    continue;
                }
            }

            indexTypeValueList.ValueListItems.Add(item, item);
        }
        return indexTypeValueList;
    }

    private static int GetCartridgeID(string cartridgeName)
    {
        var cartridgeTypes = new Dictionary<string, int>()
                                                 {
                                                     {Enum.GetName(typeof(COEDataView.IndexTypes), COEDataView.IndexTypes.CS_CARTRIDGE), Constants.CSCartridgeID},
                                                     {Enum.GetName(typeof(COEDataView.IndexTypes), COEDataView.IndexTypes.DIRECT_CARTRIDGE), Constants.DirectCartridgeID},
                                                     {Enum.GetName(typeof(COEDataView.IndexTypes), COEDataView.IndexTypes.JCHEM_CARTRIDGE), Constants.JChemCartridgeID}
                                                 };

        return cartridgeTypes.ContainsKey(cartridgeName) ? cartridgeTypes[cartridgeName] : 0;
    }

    /// <summary>
    /// Analyse the instance name.
    /// </summary>
    /// <param name="instanceSchema">The instance schema name which include the instance name.</param>
    /// <param name="instanceName">The database instance name.</param>
    /// <param name="schemaName">The database schema name.</param>
    public static void AnalyseInstanceSchema(string instanceSchema, ref string instanceName, ref string schemaName)
    {
        if (string.IsNullOrEmpty(instanceSchema))
        {
            instanceName = schemaName = string.Empty;
        }
        else if (!instanceSchema.Contains("."))
        {
            InstanceData mainInstance = ConfigurationUtilities.GetMainInstance();
            instanceName = mainInstance.Name;
            schemaName = instanceSchema;
        }
        else
        {
            instanceName = instanceSchema.Split(new char[] { '.' })[0];
            schemaName = instanceSchema.Split(new char[] { '.' })[1];
        }
    }

    /// <summary>
    /// Gets the qualify instance schema name.
    /// </summary>
    /// <param name="instanceName">The database instance name.</param>
    /// <param name="ownerName">The database owner/schema name.</param>
    /// <returns>
    /// The qualify instance schema name which will be saved in database and configuration file.
    /// </returns>
    public static string GetQualifyInstaceSchemaName(string instanceName, string ownerName)
    {
        InstanceData mainInstance = ConfigurationUtilities.GetMainInstance();

        if (string.IsNullOrEmpty(ownerName))
        {
            return ownerName;
        }

        if (ownerName.Contains("."))
        {
            var qualifyName = !ownerName.StartsWith(mainInstance.Name + ".") ?
                                    ownerName : ownerName.Remove(0, mainInstance.Name.Length + 1);
            return qualifyName;
        }

        var isMainInstance = string.IsNullOrWhiteSpace(instanceName) ||
            instanceName.Equals(mainInstance.Name, StringComparison.InvariantCultureIgnoreCase);

        var qualifyInstanceName = isMainInstance ? ownerName : instanceName + "." + ownerName;

        return qualifyInstanceName;
    }
}

/// <summary>
/// Class to define strings, paths, etc that can be usefull for the entire App.
/// </summary>
public class Constants
{
    //public Constants() { }

    public const string FooterSeparator = " | ";
    public const string DataViewFrom = "DataViewFrom";
    public const string SelectedNode = "SelectedNode";
    public const string SelectedTable = "SelTab";
    public const int AliasMaxLength = 25;

    public const int CSCartridgeID = 1;
    public const int DirectCartridgeID = 2;
    public const int JChemCartridgeID = 3;

    #region Input Validation expresions
    public const string AliasValidInputCharacters = @"^[a-zA-Z0-9_\s]{0,100}$";
    public const string ValidInputCharacters = @"^[a-zA-Z0-9_'=()!?,.:* /+%-.\s]{0,255}$";
    public const string ValidInputCharactersForName = @"^[a-zA-Z0-9_'=()!?,.:* /+%-.\s]{0,50}$";
    #endregion

    public const int MasterSchemaDataViewID = 0;

    #region Characters to split DataKeys (Ex: ID=1|NAME=MYNAME )
    public const string DataKeysParamSeparator = "|";
    public const string DataKeysItemsSeparator = "=";

    #endregion

    #region Controls IDs

    public const string ErrorAreaUCID = "ErrorAreaUserControl";
    public const string ConfirmationAreaUCID = "ConfirmationAreaUserControl";
    public const string MainFormName = "MainForm";
    public const string MainBodyName = "MainBody";
    public const string ContentPlaceHolderID = "ContentPlaceHolder";

    #endregion

    #region Session Vars

    public const string COEDataView = "COEDataView";
    public const string COESchemasOnPublishing = "COESchemasOnPublishing";
    public const string COESchemasOnRemoving = "COESchemasOnRemoving";
    public const string COEDataManager = "COEDVMgr";
    public const string COEDataViewBO = "COEDataViewBO";
    public const string COEDataViewBOList = "COEDataViewBOList";
    public const string DataViewTableHelper = "DataViewTableHelper";
    public const string DataViewHelper = "DvHelp";
    public const string AllUsers = "AllUsrs";
    public const string SelectedUsers = "SelUsrs";
    public const string AllRoles = "AllRls";
    public const string SelectedRoles = "SelRls";
    public const string ValidCartridgeBO = "ValidCartridgeBO";

    #endregion

    #region ViewState Vars

    public const string ID = "ID";
    public const string User_ID = "User_ID";
    public const string Is_Public = "Is_Public";
    public const string FormGroup = "Form_Group";
    public const string DateCreated = "DateCreated";
    public const string DataBase = "DB";
    public const string BaseTable = "BaseTbl";
    public const string ButtonModeVW = "ButtonModeVW";
    public const string ButtonType = "ButtonType";
    public const string Parent = "Parent";
    public const string ParentKey = "ParentKey";
    public const string PreviousParent = "PreviousParent";
    public const string PreviousParentKey = "PreviousParentKey";
    public const string Child = "Child";
    public const string ChildKey = "ChildKey";
    public const string JoinType = "JoinType";
    public const string PreviousJoinType = "PreviousJoinType";
    public const string CurrentEditingRel = "CurrEditRel";
    public const string PageState = "PageState";
    public const string TableListBO = "TblListBO";
    public const string SourceFieldId = "SrcFieldId";
    public const string Instances = "Instances";
    public const string Schemas = "Schemas";
    public const string InstanceSchemas = "InstanceSchemas";
    public const string PublishedDbs = "PubDbs";
    public const string TypeOfButton = "TypeBtn";
    public const string ImageName = "ImgName";
    public const string ImagesFolder = "ImgFolder";

    #endregion

    #region Request Vars

    public const string NewDVFromSchema = "NewDVFSch";
    public const string SelectedDataViewID = "SelDVId";
    public const string SelectedDataViewDataBase = "SelDVDB";
    public const string Action = "Action";
    public const string Caller = "Caller";
    public const string ParamCaller = "ParamCaller";
    public const string EditingDV = "EditingDV";
    public const string IsMasterDataView = "IsMDV";
    public const string FieldId = "FieldId";
    public const string RecreateDV = "RecreateDV";
    public const string InactivityURLParam = "Inactivity";
    public const string ReturnURL = "ReturnURL";
    public const string CloneDV = "Clone_DV";

    #endregion

    #region URLs & Paths Strings

    public const string PublicCommonJScriptsPath = "~/Forms/Public/JScripts/CommonScripts.js";
    public const string PublicContentAreaFolder = "~/Forms/Public/ContentArea/";
    public const string DvManagerContentAreaFolder = "~/Forms/DataViewManager/ContentArea/";
    public const string SecurityManagerContentAreaFolder = "~/Forms/SecurityManager/ContentArea/";

    #endregion

    #region Variables

    public const string ButtonsImagesFolder = "ButtonsImgFol";
    public const string AppName = "AppName";
    public const string AppPagesTitle = "AppPagesTitle";
    public const string PageControlsSettingsPath = "PageControlsSettingsPath";
    public const string ImagesLibName = "ImgLibName";
    public const string ImagesCategory = "ImgsCat";
    public const string ImagesSubType = "ImgsSubType";
    public const string ImagesFormat = "ImgsFormat";
    public const string ImagesSize = "ImgsSize";
    public const string ImagesCollection = "ImgsColl";

    #endregion

    #region Enums

    public enum DataViewFromTypes
    {
        ExistingDataView,
        Schema,
    }

    public enum MessagesCode
    {
        None,
        InvalidDataView,
        SubmittedDataView,
        DeletedDataView,
        MasterDataViewExists,
        NoDataViews,
        NoMasterDataView,
        Unknown,
        SessionTimeOut,
        PageSettingsDisable,
    }

    public enum SortDirection
    {
        ASC = 1,
        DESC = -1,
    }

    public enum SortCriterias
    {
        Name_ASC,
        Name_DESC,
        ID_ASC,
        ID_DESC,
        Database_ASC,
        Database_DESC,
        DatabaseAndBaseTable_ASC,
        DatabaseAndBaseTable_DESC,
    }

    public enum DataKeysParam
    {
        NAME,
        ID,
        INDEX,
        ALIAS,
        DATABASE,
        DATAVIEWID,
        ACTION,
        PARENT,
        PARENTKEY,
        CHILD,
        CHILDKEY,
        JOINTYPE,
    }

    public enum NodeAction
    {
        DoNothing,
        Grouping
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

    public enum Validate
    {
        NameAndDescription,
        BaseTable,
        TablesAndFields,
        Tables,
        Fields,
        Relationships,
        Security,
    }

    public enum PageStates
    {
        Create_DV = 0,
        Clone_DV = 1,
        Edit_DV = 2,
        Undefined = 3,
    }
  
    #endregion

    #region Formaters

    public const string TableText = "{0} ({1})";
    public const string TableToolTip = "{0}={1} | {2}={3} | {4}={5}";
    public const string FieldText = "{0} ({1})";
    public const string FieldToolTip = "{0}={1} | {2}={3}";
    public const string RelationshipsText = "{0}.{1}.{2} <- {6} -> {3}.{4}.{5}";
    public const string FullRelationshipsText = "{0}.{1}.{2}";

    #endregion
}

/// <summary>
/// Class that defines all the information to be shown for a Node in a Webtree.
/// </summary>
/// <remarks>This kind of objects can be used to represent nodes in a webtree. Any kind of nodes (Tables, DataViews, etc)</remarks>
public class DataViewNode
{
    #region Variables

    //private int _id = int.MinValue;
    private string _dataKey = String.Empty;
    private string _text = String.Empty;
    private string _toolTip = String.Empty;
    //private int _level = int.MinValue;
    //private int _parentLevel = int.MinValue;
    private List<DataViewNode> _dataViewNode = new List<DataViewNode>();
    private List<string> _addedItemsID = new List<string>();
    //private bool _hasChilds = false;
    private bool _enabled = true;
    private bool _expanded = true;
    private bool _checked = false;
    private bool _checkBox = false;
    private bool _visible = true;
    private bool _isView = false;

    private const string _rootNodeDataKey = "rootNode";

    #endregion

    #region Properties

    /// <summary>
    /// Returns a boolean indicating if it has child nodes
    /// </summary>
    public bool HasChilds
    {
        get
        {
            if (_dataViewNode.Count > 0)
                return true;
            else
                return false;
        }
    }

    /// <summary>
    /// The identifier of the node
    /// </summary>
    public string DataKey
    {
        get { return _dataKey; }
        set { _dataKey = value; }
    }

    /// <summary>
    /// ToolTip text to display
    /// </summary>
    public string ToolTip
    {
        get { return _toolTip; }
        set { _toolTip = value; }
    }

    /// <summary>
    /// Text node to display
    /// </summary>
    public string Text
    {
        get { return _text; }
        set { _text = value; }
    }

    /// <summary>
    /// List of Child nodes
    /// </summary>
    public List<DataViewNode> DataViewItems
    {
        get { return _dataViewNode; }
        set { _dataViewNode = value; }
    }

    /// <summary>
    /// List of the DataKeys of the child nodes
    /// </summary>
    /// <remarks>This is to avoid looping all the childs in case we just want to know if a child is present</remarks>
    public List<string> AddedItemsDataKeys
    {
        get { return _addedItemsID; }
    }

    /// <summary>
    /// Get set a node as enable
    /// </summary>
    public bool Enabled
    {
        get { return _enabled; }
        set { _enabled = value; }
    }

    /// <summary>
    /// Get Set a node to be shown expanded
    /// </summary>
    public bool Expanded
    {
        get { return _expanded; }
        set { _expanded = value; }
    }

    /// <summary>
    /// Status of the checkbox
    /// </summary>
    public bool Checked
    {
        get { return _checked; }
        set { _checked = value; }
    }

    /// <summary>
    /// Enables to display a checkbox next to the text item.
    /// </summary>
    public bool CheckBox
    {
        get { return _checkBox; }
        set { _checkBox = value; }
    }

    public bool Visible
    {
        get { return _visible; }
        set { _visible = value; }
    }

    public bool IsView
    {
        get { return _isView; }
        set { _isView = value; }
    }
    #endregion

    #region Methods

    /// <summary>
    /// Add a child node to the current Node.
    /// </summary>
    /// <param name="nodeToAdd">Node to Add to the collection</param>
    public void AddNode(DataViewNode nodeToAdd)
    {
        if (nodeToAdd != null)
        {
            this._dataViewNode.Add(nodeToAdd);
            this._addedItemsID.Add(nodeToAdd.DataKey);
        }
    }

    public static DataViewNode NewDataViewNode(List<TableBO> tables, Constants.SortCriterias criteria, int checkedFieldId)
    {
        return new DataViewNode(tables, criteria, checkedFieldId);
    }

    public static DataViewNode NewDataViewNode(List<TableBO> tables, Constants.SortCriterias criteria, int checkedFieldId, bool excludeLookupFields)
    {
        return new DataViewNode(tables, criteria, checkedFieldId, excludeLookupFields);
    }

    public static DataViewNode NewDataViewNode(List<COEDataViewBO> dataViewBO, Constants.SortCriterias criteria)
    {
        return new DataViewNode(dataViewBO, criteria);
    }

    public static DataViewNode NewDataViewNode(List<TableBO> tables, Constants.SortCriterias criteria)
    {
        return new DataViewNode(tables, criteria);
    }

    public static DataViewNode NewDataViewNode(COEDataViewBO dataViewBO)
    {
        return new DataViewNode(Utilities.FormatDataViewBONodeDataKey(dataViewBO.ID.ToString(), true),
                                Utilities.FormatDataViewBONodeText(dataViewBO.Name, dataViewBO.Description, dataViewBO.DatabaseName),
                                Utilities.FormatDataViewBONodeTooltip(dataViewBO.ID.ToString(), dataViewBO.UserName, dataViewBO.Description, dataViewBO.DatabaseName));
    }

    public static DataViewNode NewDataViewNode(List<TableBO> tables, Constants.SortCriterias criteria, List<int> checkedIds, int baseTableId)
    {
        return new DataViewNode(tables, criteria, checkedIds, baseTableId);
    }

    public static DataViewNode NewDataViewNode(Csla.SortedBindingList<TableBO> tables, Constants.SortCriterias criteria, List<int> checkedIds, int baseTableId)
    {
        return new DataViewNode(tables, criteria, checkedIds, baseTableId);
    }

    public static DataViewNode NewDataViewNode(TableBO table, bool checkBox, bool checkBoxChecked)
    {
        return new DataViewNode(Utilities.FormatTableNodeDataKey(table.ID.ToString(), table.Name, table.Alias, table.DataBase, true),
                                Utilities.FormatTableText(table.Name, table.Alias),
                                Utilities.FormatTableToolTip(table.Name, table.Alias, table.ID.ToString(), table.DataBase), checkBox, checkBoxChecked, table.IsView);
    }

    public override string ToString()
    {
        return "DataKey:" + this._dataKey + " Items=" + this._addedItemsID.Count.ToString();
    }

    #endregion

    #region Constructors

    public DataViewNode()
    {
    }

    internal DataViewNode(string dataKey, string text, string tooltip)
        : this()
    {
        _dataKey = dataKey;
        _text = text;
        _toolTip = tooltip;
    }


    internal DataViewNode(string dataKey, string text, string tooltip, bool checkbox, bool checkboxchecked)
        : this(dataKey, text, tooltip)
    {
        _checkBox = checkbox;
        _checked = checkboxchecked;
    }

    internal DataViewNode(string dataKey, string text, string tooltip, bool checkbox, bool checkboxchecked, bool isview)
        : this(dataKey, text, tooltip, checkbox, checkboxchecked)
    {
        _isView = isview;
    }

    internal DataViewNode(List<TableBO> tables, Constants.SortCriterias criteria)
    {
        if (tables != null && tables.Count > 0)
        {
            //First set the root node based on the criteria param: Database, name, alias, and some combinations.
            _dataKey = Utilities.FormatRootDataKey(tables[0].DataBase, false);
            _text = Utilities.FormatRootNodeText(tables[0].DataBase);
            _checkBox = true;

            DataViewNode tablesNode = new DataViewNode(Utilities.FormatRootDataKey(tables[0].DataBase, true), "Tables", "Tables", false, false);
            DataViewNode viewsNode = new DataViewNode(Utilities.FormatRootDataKey(tables[0].DataBase, true), "Views", "Views", false, false);

            //Now create child nodes.
            foreach (TableBO table in tables)
            {
                string text = Utilities.FormatTableText(table.Name, table.Alias);
                string dataKey = Utilities.FormatTableNodeDataKey(table.ID.ToString(), table.Name, table.Alias, table.DataBase, true);
                string tooltip = Utilities.FormatTableToolTip(table.Name, table.Alias, table.ID.ToString(), table.DataBase);
                DataViewNode node = new DataViewNode(dataKey, text, tooltip, false, true, table.IsView);

                if (table.IsView)
                    viewsNode.AddNode(node);
                else
                    tablesNode.AddNode(node);
            }

            if (tablesNode.HasChilds)
                this.AddNode(tablesNode);
            if (viewsNode.HasChilds)
                this.AddNode(viewsNode);
        }
    }

    internal DataViewNode(List<TableBO> tables, Constants.SortCriterias criteria, int checkedFieldId)
    {
        bool allTablesChecked = true;
        if (tables != null && tables.Count > 0)
        {
            //First set the root node based on the criteria param: Database, name, alias, and some combinations.
            _dataKey = Utilities.FormatRootDataKey(tables[0].DataBase, false);
            _text = Utilities.FormatRootNodeText(tables[0].DataBase);
            _checkBox = false;

            //Now create child nodes.
            foreach (TableBO table in tables)
            {
                string text = Utilities.FormatTableText(table.Name, table.Alias);
                string dataKey = Utilities.FormatTableNodeDataKey(table.ID.ToString(), table.Name, table.Alias, table.DataBase, true);
                string tooltip = Utilities.FormatTableToolTip(table.Name, table.Alias, table.ID.ToString(), table.DataBase);
                DataViewNode node = new DataViewNode(dataKey, text, tooltip, false, false, table.IsView);
                foreach (FieldBO field in table.Fields)
                {
                    string nodeText = Utilities.FormatFieldText(field.Name, field.Alias);
                    string nodeDataKey = Utilities.FormatFieldNodeDataKey(field.ID, field.Name, field.Alias, true);
                    string nodeToolTip = Utilities.FormatFieldToolTip(field.ID.ToString(), field.Name, field.Alias);
                    DataViewNode fieldNode = new DataViewNode(nodeDataKey, nodeText, nodeToolTip);
                    fieldNode.CheckBox = true;
                    if (field.ID == checkedFieldId)
                        fieldNode.Checked = true;
                    fieldNode.Visible = !field.FromMasterSchema;
                    node.AddNode(fieldNode);
                }
                this.AddNode(node);
            }
        }
        else
            allTablesChecked = false;

        _checked = allTablesChecked;
    }

    internal DataViewNode(List<TableBO> tables, Constants.SortCriterias criteria, int checkedFieldId, bool excludeLookupFields)
    {
        bool allTablesChecked = true;
        if (tables != null && tables.Count > 0)
        {
            //First set the root node based on the criteria param: Database, name, alias, and some combinations.
            switch (criteria)
            {
                case Constants.SortCriterias.Database_ASC:
                    _dataKey = Utilities.FormatRootDataKey(tables[0].DataBase, false);
                    _text = Utilities.FormatRootNodeText(tables[0].DataBase);
                    _checkBox = false;
                    break;
                default:
                    _dataKey = Utilities.FormatRootDataKey(tables[0].DataBase, false);
                    _text = Utilities.FormatRootNodeText(tables[0].DataBase);
                    _checkBox = false;
                    break;
            }
            //Now create child nodes.
            foreach (TableBO table in tables)
            {
                string text = Utilities.FormatTableText(table.Name, table.Alias);
                string dataKey = Utilities.FormatTableNodeDataKey(table.ID.ToString(), table.Name, table.Alias, table.DataBase, true);
                string tooltip = Utilities.FormatTableToolTip(table.Name, table.Alias, table.ID.ToString(), table.DataBase);
                DataViewNode node = new DataViewNode(dataKey, text, tooltip, false, false, table.IsView);
                foreach (FieldBO field in table.Fields)
                {
                    if (!(excludeLookupFields && field.LookupFieldId > 0))
                    {
                        string nodeText = Utilities.FormatFieldText(field.Name, field.Alias);
                        string nodeDataKey = Utilities.FormatFieldNodeDataKey(field.ID, field.Name, field.Alias, true);
                        string nodeToolTip = Utilities.FormatFieldToolTip(field.ID.ToString(), field.Name, field.Alias);
                        DataViewNode fieldNode = new DataViewNode(nodeDataKey, nodeText, nodeToolTip);
                        fieldNode.CheckBox = true;
                        if (field.ID == checkedFieldId)
                            fieldNode.Checked = true;
                        fieldNode.Visible = !field.FromMasterSchema;
                        node.AddNode(fieldNode);
                    }
                }
                if (node.AddedItemsDataKeys.Count > 0)
                    this.AddNode(node);
            }
        }
        else
            allTablesChecked = false;

        _checked = allTablesChecked;
    }

    private DataViewNode(List<COEDataViewBO> dataViewBOList, Constants.SortCriterias criteria)
    {
        if (dataViewBOList != null && dataViewBOList.Count > 0)
        {
            switch (criteria)
            {
                case Constants.SortCriterias.Database_ASC:
                    _dataKey = Utilities.FormatRootDataKey(dataViewBOList[0].DatabaseName, false);
                    _text = Utilities.FormatRootNodeText(dataViewBOList[0].DatabaseName);
                    _toolTip = _text;
                    _checkBox = true;
                    foreach (COEDataViewBO dataViewBO in dataViewBOList)
                    {
                        string text = Utilities.FormatDataViewBONodeText(dataViewBO.Name, dataViewBO.Description, dataViewBO.DatabaseName);
                        string dataKey = Utilities.FormatDataViewBONodeDataKey(dataViewBO.ID.ToString(), true);
                        string tooltip = Utilities.FormatDataViewBONodeTooltip(dataViewBO.ID.ToString(), dataViewBO.UserName, dataViewBO.Description, dataViewBO.DatabaseName);
                        this.AddNode(new DataViewNode(dataKey, text, tooltip));
                    }
                    break;
                case Constants.SortCriterias.Database_DESC:
                    _dataKey = Utilities.FormatRootDataKey(dataViewBOList[0].DatabaseName, false);
                    _text = Utilities.FormatRootNodeText(dataViewBOList[0].DatabaseName);
                    _toolTip = _text;
                    _checkBox = true;
                    foreach (COEDataViewBO dataViewBO in dataViewBOList)
                    {
                        string text = Utilities.FormatDataViewBONodeText(dataViewBO.Name, dataViewBO.Description, dataViewBO.DatabaseName);
                        string dataKey = Utilities.FormatDataViewBONodeDataKey(dataViewBO.ID.ToString(), true);
                        string tooltip = Utilities.FormatDataViewBONodeTooltip(dataViewBO.ID.ToString(), dataViewBO.UserName, dataViewBO.Description, dataViewBO.DatabaseName);
                        this.AddNode(new DataViewNode(dataKey, text, tooltip));
                    }
                    break;
            }
            _checked = false;
        }
    }

    internal DataViewNode(List<TableBO> tables, Constants.SortCriterias criteria, List<int> checkedTablesIds, int baseTableId)
    {
        bool allTablesChecked = true;
        bool allViewsChecked = true;
        if (tables != null && tables.Count > 0)
        {
            //First set the root node based on the criteria param: Database, name, alias, and some combinations.
            switch (criteria)
            {
                case Constants.SortCriterias.Database_ASC:
                    _dataKey = Utilities.FormatRootDataKey(tables[0].DataBase, false);
                    _text = Utilities.FormatRootNodeText(tables[0].DataBase);
                    _checkBox = true;
                    break;
                default:
                    _dataKey = Utilities.FormatRootDataKey(tables[0].DataBase, false);
                    _text = Utilities.FormatRootNodeText(tables[0].DataBase);
                    _checkBox = true;
                    break;
            }
            DataViewNode tablesNode = new DataViewNode(Utilities.FormatRootDataKey(tables[0].DataBase, true), "Tables", "Tables", true, true);
            DataViewNode viewsNode = new DataViewNode(Utilities.FormatRootDataKey(tables[0].DataBase, true), "Views", "Views", true, false);

            //Now create child nodes.
            foreach (TableBO table in tables)
            {
                string text = Utilities.FormatTableText(table.Name, table.Alias);
                string dataKey = Utilities.FormatTableNodeDataKey(table.ID.ToString(), table.Name, table.Alias, table.DataBase, true);
                string tooltip = Utilities.FormatTableToolTip(table.Name, table.Alias, table.ID.ToString(), table.DataBase);
                DataViewNode node = new DataViewNode(dataKey, text, tooltip, true, false, table.IsView);
                node.CheckBox = true;
                if (checkedTablesIds != null)
                {
                    if (checkedTablesIds.Contains(table.ID))
                        node.Checked = true;
                    else
                    {
                        if (table.IsView)
                            allViewsChecked = false;
                        else
                            allTablesChecked = false;
                    }
                }
                else
                    allTablesChecked = false;
                if (table.ID == baseTableId)
                    node.Enabled = false;

                if (table.IsView)
                    viewsNode.AddNode(node);
                else
                    tablesNode.AddNode(node);
            }

            tablesNode.Checked = allTablesChecked;
            viewsNode.Checked = allViewsChecked;

            if (tablesNode.HasChilds)
                this.AddNode(tablesNode);
            if (viewsNode.HasChilds)
                this.AddNode(viewsNode);
        }
        else
            allTablesChecked = false;

        _checked = allTablesChecked && allViewsChecked;
    }

    internal DataViewNode(Csla.SortedBindingList<TableBO> tables, Constants.SortCriterias criteria, List<int> checkedTablesIds, int baseTableId)
    {
        bool allTablesChecked = true;
        if (tables != null && tables.Count > 0)
        {
            //First set the root node based on the criteria param: Database, name, alias, and some combinations.
            switch (criteria)
            {
                case Constants.SortCriterias.Database_ASC:
                    _dataKey = Utilities.FormatRootDataKey(tables[0].DataBase, false);
                    _text = Utilities.FormatRootNodeText(tables[0].DataBase);
                    _checkBox = true;
                    //Now create child nodes.
                    foreach (TableBO table in tables)
                    {
                        string text = Utilities.FormatTableText(table.Name, table.Alias);
                        string dataKey = Utilities.FormatTableNodeDataKey(table.ID.ToString(), table.Name, table.Alias, table.DataBase, true);
                        string tooltip = Utilities.FormatTableToolTip(table.Name, table.Alias, table.ID.ToString(), table.DataBase);
                        DataViewNode node = new DataViewNode(dataKey, text, tooltip, true, false, table.IsView);
                        node.CheckBox = true;
                        if (checkedTablesIds != null)
                        {
                            if (checkedTablesIds.Contains(table.ID))
                                node.Checked = true;
                            else
                                allTablesChecked = false;
                        }
                        else
                            allTablesChecked = false;
                        if (table.ID == baseTableId) node.Enabled = false;
                        this.AddNode(node);
                    }
                    break;
                case Constants.SortCriterias.Database_DESC:
                    _dataKey = Utilities.FormatRootDataKey(tables[0].DataBase, false);
                    _text = Utilities.FormatRootNodeText(tables[0].DataBase);
                    _checkBox = true;
                    //Now create child nodes.
                    foreach (TableBO table in tables)
                    {
                        string text = Utilities.FormatTableText(table.Name, table.Alias);
                        string dataKey = Utilities.FormatTableNodeDataKey(table.ID.ToString(), table.Name, table.Alias, table.DataBase, true);
                        string tooltip = Utilities.FormatTableToolTip(table.Name, table.Alias, table.ID.ToString(), table.DataBase);
                        DataViewNode node = new DataViewNode(dataKey, text, tooltip, true, false, table.IsView);
                        node.CheckBox = true;
                        if (checkedTablesIds != null)
                        {
                            if (checkedTablesIds.Contains(table.ID))
                                node.Checked = true;
                            else
                                allTablesChecked = false;
                        }
                        else
                            allTablesChecked = false;
                        if (table.ID == baseTableId) node.Enabled = false;
                        this.AddNode(node);
                    }
                    break;
                default:
                    _dataKey = Utilities.FormatRootDataKey(tables[0].DataBase, false);
                    _text = Utilities.FormatRootNodeText(tables[0].DataBase);
                    _checkBox = true;
                    allTablesChecked = false;
                    break;
            }
            _checked = allTablesChecked;
        }
    }

    #endregion
}


/// <summary>
/// Exception raised when invalid text was entered
/// </summary>
[Serializable]
public class InvalidInputText : Exception
{
    public InvalidInputText() : base("Invalid entered text") { }
    public InvalidInputText(string message) : base(message.ToString()) { }
}


#region Controls used in the webgrid as ITemplates

public class DataTypeDropDrown : ITemplate
{
    DropDownList _dropDownList = null;

    public string SelectedValue
    {
        get { return _dropDownList.SelectedValue; }
    }

    #region ITemplate Members

    public void InstantiateIn(Control container)
    {
        _dropDownList = new DropDownList();
        _dropDownList.ID = "DataTypeDropDown";
        _dropDownList.CssClass = "DataDropDownOnGrid";
        _dropDownList.Enabled = false;
        Infragistics.WebUI.UltraWebGrid.CellItem controlItem = (Infragistics.WebUI.UltraWebGrid.CellItem)container;
        foreach (string item in Enum.GetNames(typeof(COEDataView.AbstractTypes)))
        {
            ListItem currentItem = new ListItem(item, item);
            _dropDownList.Items.Add(currentItem);
        }
        _dropDownList.EnableViewState = true;
        controlItem.Controls.Add(_dropDownList);
    }

    #endregion
}

public class MimeTypeDropDrown : ITemplate
{
    DropDownList _dropDownList = null;

    public string SelectedValue
    {
        get { return _dropDownList.SelectedValue; }
    }

    #region ITemplate Members

    public void InstantiateIn(Control container)
    {
        _dropDownList = new DropDownList();
        _dropDownList.ID = "MimeTypeDropDown";
        _dropDownList.CssClass = "MimeDropDownOnGrid";
        Infragistics.WebUI.UltraWebGrid.CellItem controlItem = (Infragistics.WebUI.UltraWebGrid.CellItem)container;
        foreach (string item in Enum.GetNames(typeof(COEDataView.MimeTypes)))
        {
            ListItem currentItem = new ListItem(item, item);
            _dropDownList.Items.Add(currentItem);
        }
        _dropDownList.EnableViewState = true;
        controlItem.Controls.Add(_dropDownList);
    }

    #endregion
}

public class IndexTypeDropDrown : ITemplate
{
    DropDownList _dropDownList = null;

    public string SelectedValue
    {
        get { return _dropDownList.SelectedValue; }
    }

    public override string ToString()
    {
        return _dropDownList.SelectedValue;
    }

    #region ITemplate Members

    public void InstantiateIn(Control container)
    {
        Infragistics.WebUI.UltraWebGrid.CellItem controlItem = (Infragistics.WebUI.UltraWebGrid.CellItem)container;
        _dropDownList = new DropDownList();
        _dropDownList.ID = "IndexTypeDropDown";
        _dropDownList.CssClass = "IndexDropDownOnGrid";
        foreach (string item in Enum.GetNames(typeof(COEDataView.IndexTypes)))
        {
            ListItem currentItem = new ListItem(item, item);
            _dropDownList.Items.Add(currentItem);
        }
        _dropDownList.EnableViewState = true;
        controlItem.Controls.Add(_dropDownList);
    }

    #endregion
}

public class SortOrderDropDown : ITemplate
{
    DropDownList _dropDownList = null;

    public string SelectedValue
    {
        get { return _dropDownList.SelectedValue; }
    }

    public override string ToString()
    {
        return _dropDownList.SelectedValue;
    }

    #region ITemplate Members

    public void InstantiateIn(Control container)
    {
        Infragistics.WebUI.UltraWebGrid.CellItem controlItem = (Infragistics.WebUI.UltraWebGrid.CellItem)container;
        _dropDownList = new DropDownList();
        _dropDownList.ID = "SortOrderDropDown";
        _dropDownList.CssClass = "SortOrderDropDownOnGrid";
        foreach (string item in Enum.GetNames(typeof(COEDataView.SortDirection)))
        {
            ListItem currentItem = new ListItem(item, item);
            _dropDownList.Items.Add(currentItem);
        }
        _dropDownList.EnableViewState = true;
        controlItem.Controls.Add(_dropDownList);
    }

    #endregion
}

public class ParentColumnButton : ITemplate
{
    #region Variables
    Button _button = null;
    #endregion

    #region ITemplate Members

    public void InstantiateIn(Control container)
    {
        Infragistics.WebUI.UltraWebGrid.CellItem controlItem = (Infragistics.WebUI.UltraWebGrid.CellItem)container;
        _button = new Button();
        _button.ID = "ParentColumnButton";
        _button.SkinID = "ButtonLikeLink";
        _button.CssClass = "LogOffButton";
        _button.EnableViewState = true;
        controlItem.Controls.Add(_button);
    }

    #endregion
}

public class LookupButton : ITemplate
{
    #region Variables
    Button _button = null;
    #endregion

    #region ITemplate Members

    public void InstantiateIn(Control container)
    {
        Infragistics.WebUI.UltraWebGrid.CellItem controlItem = (Infragistics.WebUI.UltraWebGrid.CellItem)container;
        _button = new Button();
        _button.ID = "LookupButton";
        _button.SkinID = "ButtonLikeLink";
        _button.CssClass = "LogOffButton";
        _button.EnableViewState = true;
        controlItem.Controls.Add(_button);
    }

    #endregion
}

public class ActionButton : ITemplate
{
    #region Variables
    Button _button = null;
    #endregion

    #region ITemplate Members

    public void InstantiateIn(Control container)
    {
        Infragistics.WebUI.UltraWebGrid.CellItem controlItem = (Infragistics.WebUI.UltraWebGrid.CellItem)container;
        _button = new Button();
        _button.ID = "ActionButton";
        _button.SkinID = "ButtonLikeLink";
        _button.CssClass = "LogOffButton";
        _button.EnableViewState = true;
        controlItem.Controls.Add(_button);
    }

    #endregion
}

#endregion

