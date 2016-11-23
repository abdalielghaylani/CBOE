using System;
using System.Web;
using System.Web.UI;
using CambridgeSoft.COE.Framework.Common.GUIShell.DataServices;
using System.Collections.Specialized;
using System.Xml.XPath;
using System.Configuration;
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using CambridgeSoft.COE.Framework.COESecurityService;

namespace CambridgeSoft.COE.Framework.GUIShell
{
    /// <summary>
    /// Class to define common Constans for all GUIShell Type Apps.
    /// </summary>
    public class GUIShellTypes
    {
        /// <summary>
        /// View State Vars
        /// </summary>
        public enum ViewStateEntries
        {
            AccordionControlID,
            BusinessObject,
        }

        /// <summary>
        /// Request Vars
        /// </summary>
        public enum RequestVars
        {
            CompoundID,
            MessageText,
            MessageCode,
            DisplayBack,
            MessagesButtonType,
        }

        /// <summary>
        /// Suppported control types.
        /// Remember that all control types implements ICOENavigationPaneControl
        /// </summary>
        public enum SupportedControlTypes
        {
            UserControl,
        }

        /// <summary>
        /// Table disply states, allowed values are:
        /// <list type="bullet">
        ///   <item>Collapsed</item>
        ///   <item>Expanded</item>
        /// </list>
        /// </summary>
        public enum TableDisplayStates
        {
            Colapsed,
            Expanded,
        }

        /// <summary>
        /// WebTree Events to Respond
        /// </summary>
        public enum WebTreeActions
        {
            NodeClicked,
        }

        /// <summary>
        /// Policies Names for Exceptions handling
        /// </summary>
        public enum PoliciesNames
        {
            UIExceptionPolicy,
            UnknownExceptionPolicy,
        }

        /// <summary>
        /// Codes for Messages Page
        /// </summary>
        public enum MessagesCode
        {
            Error = 0,
            PageNotFound = 1,
            Unknown = 2,
            InvalidParameter = 3,
            NoEnoughPrivileges = 4,
            SessionTimeOut = 5,
            InvalidHttpRequest = 6,
            PageSettingsDisable = 7,
            RecordNotFound = 8,
        }

        public enum MessagesButtonType
        {
            None,
            Back,
            Close,
            Login,
            Home,
        }

        /// <summary>
        /// Logs message Level/Category
        /// </summary>
        public enum LogsCategories
        {
            Information = 0,
            Warning = 1,
            Error = 2,
            Unknown = 3,
        }

        /// <summary>
        /// Log Message Type
        /// </summary>
        public enum LogMessageType
        {
            Unknown = 0,
            BeginMethod = 1,
            EndMethod = 2,
            InvalidPage = 3,
        }

        /// <summary>
        /// Application settings enumeration
        /// </summary>
        public enum AppSettings
        {
            LogCategoryMessages,
        }

        /// <summary>
        /// Logical Operators availables for handling to show or not the current page according privs
        /// </summary>
        public enum Operators
        {
            AND,
            OR,
        }

        /// <summary>
        /// Applications supported to Log.
        /// </summary>
        public enum Applications
        {
            REGISTRATION,
            MANAGER,
        }

        public enum ControlType
        {
            NotSet = 0,
            Page = 1,
            COEForm = 2,
        }

        #region String Constants

        public const string GroupTitleTextAlign = "Left";
        public const string MainFormID = "MainForm";
        public const string AppFilesExtension = ".aspx";
        public const string stringsURLSplitter = "_";
        public const int CommentsFieldSummaryLenght = 17;
        public const int DescriptionFieldSummaryLenght = 10;
        public const int CharactersOfIdNumbers = 3;
        public const char IdsPaddingCharacter = '0';
        public const string CloseWithDone = "closewithdone";
        public const string COEPageSettings = "COEPSet_";
        public const string AccordionControlID = "UltraWebListbarControl";
        public const string DashSeparatedFormat = "{0} - {1}";
        public const string IndexNumbersFormat = "{0:00} - ";
        public const string MainBodyID = "MainBody";
        public const string COEPageSettingsNameSeparator = "|";
        public const string HeaderRowControlId = "HeaderRow";
        public const string FooterRowControlId = "FooterRow";
        public const string Embed = "embed";
        public const string Embed_Apps_WildCard = "TRUE";
        public const string Theme = "theme";
        public const string EnablePopUpForBrowserVersions = "EnbPopUpBrowVer";
        public const string ControlsTypesSplitter = "|";
        public const string PageSettings_AppName = "PS_AppName";
        public const string PageSettings_Caller = "PS_Caller";
        public const string FileUploadTitleCtrlID = "FUTit";
        public const string FileUploadDescriptionCtrlID = "FUDesc";
        public const string FileUploadBindingExpression = "FUBindExp";

        public const string GUIShellProvider = "GUISHELL";
        public const string FileUploadTitleBindingExpression = "FUTitBindExp";
        public const string FileUploadDescBindingExpression = "FUDescBindExp";
        public const string FileUploadIndexBindingExpression = "FUIndBindExp";
        public const string FileUploadMaxSize = "FUSize";
        public const string FileUploadAllowedFormats = "FUFormt";

        #endregion

        #region Path References

        public const string UCImagesPath = "~/Forms/Public/UserControls/Images/";
        public const string UCPath = "~/Forms/Public/UserControls/";
        public const string ContentPagesFolder = "~/Forms/ContentArea/";
        public const string ContentPagesImagesFolder = "~/Forms/ContentArea/Images/";
        //public const string ContentAreaCommonJscriptsPath = "../ContentArea/Jscripts/CommonScripts.js";
        public const string MasterPageJscriptsPath = "../JScripts/RegistrationUtils.js";
        public const string ContentPlaceHolderID = "ContentPlaceHolder";

        #endregion

        #region Pages Names

        public const string SavedCompoundPage = "ASP.FORMS_SUBMITRECORD_CONTENTAREA_SAVEMIXTUREFORM_ASPX";
        public const string LoadCompoundPage = "ASP.FORMS_SUBMITRECORD_CONTENTAREA_LOADMIXTUREFORM_ASPX";

        #endregion

        #region WildCards

        public const string Asterisk_WildCard = "*";

        #endregion

        #region Configuration Settings
        
        public const string Themes = "themes";

        public const string UseDefaultHeader = "UseDefaultHeader";
        public const string CustomHeaderName = "CustomHeaderName";
        public const string CustomHeaderType = "CustomHeaderType";
        public const string CustomHeadersPerPage = "CustomHeadersPerPage";

        public const string UseDefaultFooter = "UseDefaultFooter";
        public const string CustomFooterName = "CustomFooterName";
        public const string CustomFooterType = "CustomFooterType";
        public const string CustomFootersPerPage = "CustomFootersPerPage";

        public enum DefaultHeader
        {
            Yes = 1,
            No = 0,
        }

        public enum DefaultFooter
        {
            Yes = 1,
            No = 0,
        }

        public enum HeaderControlSupportedTypes
        {
            NotSet = 0,
            Html = 1,
            Composite = 2,
            //Inc = 2,
            //Asp = 3,
        }

        public enum FooterControlSupportedTypes
        {
            NotSet = 0,
            Html = 1,
            Composite = 2,
            //Inc = 3,
            //UserControl = 4,
            //Asp = 3,
        }

        #endregion

        #region Header/Footer dynamic loading settings
        
        //This must be moved to a config file, so we can easily get these values and no need to recompile.
        public const string HeaderContainerID = "HeaderContainer";
        public const string HeaderUserControlID = "HeaderUserControl";
        public const string HeaderMainContentTableID = "CommonHeaderTable";
        public const string CustomHeadersFolder = @"~/Forms/Master/UserControls/Custom/";
        
        public const string FooterContainerID = "FooterContainer";
        public const string FooterUserControlID = "FooterUserControl";
        public const string FooterMainContentTableID = "CommonFooterTable";
        public const string CustomFooterFolder = @"~/Forms/Master/UserControls/Custom/";

        #endregion
    }

    /// <summary>
    /// This is a Custom EventArgs to handle our custom information 
    /// about the control wich implement COEEventArgs
    /// </summary>
    [Serializable]
    public class COEEventArgs : EventArgs
    {
        #region Variables

        private string _controlID = String.Empty;
        private string _eventType = String.Empty;

        #endregion

        #region Methods

        public COEEventArgs()
        {

        }

        public COEEventArgs(string controlID, string eventType)
        {
            this._controlID = controlID;
            this._eventType = eventType;
        }

        #endregion

        #region Properties

        public string ControlId
        {
            get { return this._controlID; }
        }

        public string EventType
        {
            get { return this._eventType; }
        }

        #endregion
    }

    /// <summary>
    /// Summary description for ICOEFooterUC
    /// </summary>
    public interface ICOEFooterUC
    {
        #region Methods

        void DataBind(COEMenu coeMenuObj);

        #endregion

    }

    /// <summary>
    /// Summary description for ICOEHeaderUC
    /// </summary>
    public interface ICOEHeaderUC
    {
        #region Methods

        void DataBind(COELogo coeLogoObj, COEMenu coeMenuObj, COEMenu coeToolBarObj);

        #endregion

    }

    /// <summary>
    /// Summary description for ICOEHeaderUC
    /// </summary>
    public interface ICOELeftPanelUC
    {
        #region Methods

        #endregion

        #region Properties

        string ID
        { get; set;}

        #endregion

        #region Events

        event EventHandler<COEEventArgs> CommandRaised;

        #endregion

    }

    public interface ICOEPagewithLeftPanelUC
    {
        #region Properties

        string ID
        { get; set;}

        #endregion

        #region Events

        event EventHandler<COEEventArgs> PageCommandRaised;

        #endregion


    }
}
