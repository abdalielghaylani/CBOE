using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace FormDBLib
{
    /// <summary>
    ///   This class contains definitions of global constants and enumerations.
    /// </summary>
    public abstract class CBVConstants
    {
        #region Enums
        public enum FormOptions
        {
            Public = 0,
            Private = 1
        }

        public enum NodeType
        {
            Folder = 0,
            Form = 1,
            Query = 2,
            MergedQuery = 3
        }
        #endregion

        #region Constants

        //  Application name
        public const String APP_NAME = "ChemBioViz";

        // fixed version info
        public const String FILE_VERSION = "12.1.0.0";    // used by AssemblyInfo.cs

        //  Help file name
        public const String HELP_FILE = "ChemBioVizHelp.chm";

		//  Web Help URL path
        public const String HELP_URL = "/CBOEHelp/CBOEContextHelp/ChemBioViz Webhelp/Default.htm";
		
        //  Key States
        //  See http://msdn.microsoft.com/es-es/library/system.windows.forms.drageventargs.keystate.aspx
        public const int KEYSTATE_CTRL = 8;   // Ctrl Key
        public const int KEYSTATE_ALT = 32;   // Alt Key

        // Key Values
        public const int KEYVALUE_ENTER = 13;  // Enter Key
        public const int KEYVALUE_BACK = 8;    // Backspace Key

        // Key Chars
        public const char KEYCHAR_ENTER = '\r'; // Enter Key
        public const char KEYCHAR_BACK = '\b';  // Backspace Key

        //  Encryption/Decryption key
        public const string CRYPTOGRAPHY_KEY = "cbvn_mru";

        //  File names
        public const String ERROR_LOG_FILE = "CBVNLog.txt";
        public const String DEFAULT_FORM_FILE_NAME = "form_out.xml";
        public const String DEFAULT_SDF_FILE_NAME = "exported.sdf";
        public const String DEFAULT_DELIM_FILE_NAME = "exported.csv";        
        public const String DEFAULT_CBVExcel_FILE_NAME = "CBVExcel.xls";

        //  Privileges
        public const String CAN_SEARCH = "CBV_CAN_SEARCH";
        public const String CAN_EDIT_FORMS = "CBV_CAN_EDIT_FORMS";
        public const String CAN_SAVE_PUBLIC_FORM = "CBV_CAN_SAVE_PUBLIC_FORM";
        public const String CAN_SAVE_DEFAULT_SETTINGS = "CBV_CAN_SAVE_DEFAULT_SETTINGS";
        public const String CAN_RENAME = "CBV_CAN_RENAME";
        
        //  Links
        public const String INSTITUTIONAL_LINK = "http://www.cambridgesoft.com/";
        public const String INSTITUTIONAL_SUPPORT_LINK = "http://www.cambridgesoft.com/support/";
        public const String HELP_LINK = "http://csnet/CSPartnerNet/Desktop/ChemFinder/CBVNet/UserGuide/user_guide.htm";
        public const String GOOGLE_LINK = "http://www.google.com/search?hl=en&q=%%&btnG=Google+Search&aq=f&oq=";

        //  COE.Framework constants
        public const String SEARCHCRITERIA_COEBOOLEAN_YES = "Yes";
        public const String SEARCHCRITERIA_COEBOOLEAN_NO = "No";

        //  COE Generic object descriptions
        public const String COE_GENERIC_OBJECT_FORM = "CBVN Form";
        public const String COE_GENERIC_OBJECT_SETTINGS = "CBVN Settings";
        public const String COE_GENERIC_OBJECT_STYLES = "CBVN Style";

        //  Toolbars names
        public const String TOOLBAR_MAIN = "MainMenuUltraToolbar";
        public const String TOOLBAR_NAVIGATOR = "BindingNavigatorUltraToolbar";

        //  Plot info
        public const String RECNO_NAME = "[Record]";


        //  Menu items names
        //  File Menu
        public const String MENU_FILE = "FilePopupMenu";
        //  File Menu items
        public const String MENU_ITEM_OPEN = "OpenPopupMenu";
        public const String MENU_ITEM_CLOSE_FORM = "Close";
        public const String MENU_ITEM_SAVE = "SavePopupMenu";
        public const String MENU_ITEM_SAVE_AS = "SaveAsPopupMenu";
        public const String MENU_ITEM_SAVE_LOCAL = "SaveLocalPopupMenu";
        public const String MENU_ITEM_PREFERENCES = "PreferencesPopupMenu";
        public const String MENU_ITEM_PAGE_SETUP = "PageSetupPopupMenu";
        public const String MENU_ITEM_PRINT = "PrintPopupMenu";
        public const String MENU_ITEM_PRINT_PREVIEW = "PrintPreviewPopupMenu";
        public const String MENU_ITEM_EXIT = "ExitPopupMenu";
        public const String MENU_ITEM_EXPORT = "Export";
        public const String MENU_ITEM_EXPORT_SD = "ExportSDFile";
        public const String MENU_ITEM_REVERT_EDITS = "RevertEditsPopupMenu";
        public const String MENU_ITEM_EXPORT_DELIM = "ExportDelimTextMenuItem";
        public const String MENU_ITEM_EXPORT_EXCEL = "ExportExcelMenuItem";        
        public const String MENU_ITEM_EXPORT_CBVEXCEL = "ExportCBVExcelMenuItem";

        //  Edit Menu
        public const String MENU_EDIT = "EditPopupMenu";
        //  Edit Menu items
        public const String MENU_ITEM_EDIT_FORM = "EditFormMenu";
        public const String MENU_ITEM_UNDO = "UndoMenuItem";
        public const String MENU_ITEM_REDO = "RedoMenuItem";
        public const String MENU_ITEM_CUT = "CutMenuItem";
        public const String MENU_ITEM_COPY = "CopyMenuItem";
        public const String MENU_ITEM_PASTE = "PasteMenuItem";
        public const String MENU_ITEM_SELECT_ALL = "SelectAllMenuItem";
        public const String MENU_ITEM_SORT = "SortMenuItem";

        //  Help Menu
        public const String MENU_HELP = "HelpPopupMenu";
        //  Help Menu items
        public const String MENU_ITEM_USER_GUIDE = "UserGuideMenuItem";
        public const String MENU_ITEM_ABOUT = "AboutMenuItem";
        public const String MENU_ITEM_DEBUG = "DebugMenuItem";
        public const String MENU_ITEM_CONNECTION = "ConnectMenuItem";

        //  View Menu
        public const String MENU_VIEW = "ViewPopupMenu";
        //  View Menu items
        public const String MENU_ITEM_EXPLORER = "ExplorerMenuItem";
        public const String MENU_ITEM_TOOLBOX = "ToolboxMenuItem";
        public const String MENU_ITEM_PROPERTIES = "PropertiesMenuItem";
        public const String MENU_ITEM_REFRESH = "RefreshMenuItem";
        public const String MENU_ITEM_VIEWTOOLTIPS = "ViewTooltipsMenuItem";

        //  Group names
        public const String PUBLIC_GROUPNAME = "Public Forms";
        public const String PRIVATE_GROUPNAME = "My Forms";
        public const String OLD_USERFORMS_GROUPNAME = "User Forms";
        public const String LOCAL_FORMS = "Local Forms";
        public const String QUERIES_GROUPNAME = "Queries";
        public const String DATAVIEWS_GROUPNAME = "Dataviews";
        public const String FORMEDIT_GROUPNAME = "Edit Form";
        public const String EDITING_GROUPNAME =    "OK - keep changes";
        public const String CANCELEDIT_GROUPNAME = "Cancel - discard changes";
        public const String FORM_UNSAVED = "(unsaved)";
        public const String PLOT_GROUPNAME = "Plot Control";

        public const String FORMTABEDIT = "Edit Form Tab";
        public const String QUERYTABEDIT = "Edit Query Tab";
        public const String GRIDTABEDIT = "Edit Grid Tab";
        public const String EDIT_PREFIX = "Edit ";
        public const String GRIDEDITDONE = "Edit Grid Done";

        //  Tree Keys
        public const String PUBLICFORMS_TREE = "PublicFormsTree";
        public const String USERFORMS_TREE = "UserFormsTree";
        public const String QUERIES_TREE = "QueriesTree";
        public const String DATAVIEWS_TREE = "DataviewsTree";

        // Tree reserved words
        public const String TREE_FORMS_ROOT = "Forms";
        public const String TREE_QUERIES_ROOT = "Queries";
        public const String DEFAULT_FOLDER_NAME = "New Folder";
        public const String RETRIEVE_ALL = "Retrieve All";
        public const String LOOKUP_NAME = "Lookup";

        public const String TREE_PATH_SEPARATOR = "\\";
        public const String TREE_QUERY_MERGED_SUFFIX = "|MergedQuery";

        //  Context Menus
        //  -------------------------------------------------------------------------------------
        //  Grid Header
        //  Add Form - Subitems indexes
        public const int BLANK_FORM = 0;
        public const int DUPLICATE_FORM = 1;
        public const int GRID_FROM_FORM = 2;
        public const int FULL_GRID = 3;
        public const int QUERY_FORM = 4;
        public const int NEW_CARDVIEW = 5;
        public const String TAB_CARDVIEW = "cardViewTabContextMenuItem";
        public const String TAB_CARDVIEW_SEP = "cardViewToolStripSeparator";
        public const String TAB_CARDVIEW_STYLE = "cardViewStyleToolStripMenuItem";
        public const String TAB_SELTOLIST = "selectionToListToolStripMenuItem1";
        public const String DELETE_TAB = "DeleteTabContextMenuItem";
       
        //  Trees 
        //  General items
        public const String CMENU_NEWFOLDER = "newFolderToolStripMenuItem";
        public const String CMENU_RENAME = "RenameFormContextMenuItem";
        public const String CMENU_REMOVE = "DeleteFormContextMenuItem";
        public const String CMENU_PROPERTIES = "PropertiesContextMenuItem";
        // Queries tree items
        public const String CMENU_Q_RESTORE_HITLIST = "restoreHitlistToolStripMenuItem";
        public const String CMENU_Q_RERUN = "RerunQueryMenuItem";
        public const String CMENU_Q_RESTORE = "RestoreQueryMenuItem";
        public const String CMENU_Q_RUN_ON_OPENING = "runOnOpenToolStripMenuItem";
        public const String CMENU_Q_SELECT_FOR_MERGE = "queryTree_selectForMergeToolStripMenuItem";
        public const String CMENU_Q_MERGE_WITH = "queryTree_mergeWithToolStripMenuItem";
        public const String CMENU_Q_KEEP = "queryTree_keepToolStripMenuItem";
        public const String CMENU_Q_KEEP_ALL = "queryTree_keepAllToolStripMenuItem";
        public const String CMENU_Q_RETRIEVE_ALL = "retrieveAllToolStripMenuItem";
        public const String CMENU_Q_DELETE = "DeleteQueryMenuItem";
        public const String CMENU_Q_RENAME = "RenameQueryMenuItem";
        public const String CMENU_Q_NEWFOLDER = "newFolderToolStripMenuItem1";
        public const String CMENU_Q_PROPERTIES = "queryTree_propertiesToolStripMenuItem";
        public const String CMENU_Q_SRCHOVERTHIS = "searchOverThisToolStripMenuItem";
        // Separators
        public const String CMENU_Q_SEPARATOR1 = "toolStripSeparator1";
        public const String CMENU_Q_SEPARATOR2 = "toolStripSeparator2";
        public const String CMENU_Q_SEPARATOR3 = "toolStripSeparator3";
        public const String CMENU_Q_SEPARATOR4 = "toolStripSeparator4";
        public const String CMENU_Q_SEPARATOR8 = "toolStripSeparator8";

        //  File filters
        public const String XML_FILE_FILTERS = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
        public const String SDF_FILE_FILTERS = "SD files (*.sdf)|*.sdf|All files (*.*)|*.*";
        public const String TXT_FILE_FILTERS = "TXT files (*.txt)|*.txt|All files (*.*)|*.*";
        public const String DELIM_FILE_FILTERS = "TXT files (*.txt)|*.txt|CSV files (*.csv)|*.csv|All files (*.*)|*.*";
        public const String STRUCT_FILE_FILTERS = "CDX files (*.cdx)|*.cdx|CDXML files (*.cdxml)|*.cdxml|MOL files (*.mol)|*.mol|All files (*.*)|*.*";
        public const String DXP_FILE_FILTERS = "DXP files (*.dxp)|*.dxp|All files (*.*)|*.*";
        public const String ALL_FILE_FILTERS = "All files (*.*)|*.*";        
        public const String CBVExcel_FILE_FILTERS = "Excel files (*.xls)|*.xls|All files (*.*)|*.*";
        //  Dockable Panes
        public const String PROPERTIES_PANE_NAME = "Properties";
        public const String PROPERTIES_DOCKABLE_CONTROL_PANE = "PropertiesDockableControlPane";
        public const String PROPERTIES_DOCKAREA_PANE = "PropertiesDockArea";
        public const String TOOLBOX_PANE_NAME = "Toolbox";
        public const String EXPLORER_PANE_NAME = "Explorer";

        //  Preferences Tab names
        public const String PREFERENCES_MAIN = "Main";
        public const String PREFERENCES_STYLES = "Styles";
        public const String PREFERENCES_SEARCH = "Search";
        public const String PREFERENCES_LOG = "Log";
        public const String PREFERENCES_ADVANCED = "Advanced";

        //  Form Visibility Sufixes
        public const String PUBLIC_FORM = " [public]";
        public const String PRIVATE_FORM = " [private]";
        public const String LOCAL_FORM = " [local]";
        public const String PUBLIC = "Public";
        public const String PRIVATE = "Private";
        public const String LOCAL = "Local";

        //  Styles folder
        public const String STYLES_FOLDER_NAME = "Styles"; // was "\\Styles\\";
        public const String STYLE_NOT_SET = "No Style";

        //  Settings. Open Mode constants
        public const String OPEN_LAST_FORM = "Last form viewed";
        public const String OPEN_DEFAULT_FORM = "Default form chosen below";
        public const String OPEN_BLANK_FORM = "Blank form";

        //  General. Settings constants
        public const String PORTABLE_SETTINGS_PROVIDER = "PortableSettingsProvider";
        public const String SETTINGS_XML_ROOT = "cbvnsettings";
        public const String SETTINGS_XML_NODE_GENERAL = "general";
        public const String SETTINGS_XML_NODE_SEARCH = "search";    
        public const String SETTINGS_BASE_NAME_GENERAL = "ChemBioViz.NET.Properties.Settings";
        public const String SETTINGS_BASE_NAME_SEARCH = "ChemBioViz.NET.SearchOptionsSettings";
        public const String SETTINGS_FILE_EXTENSION = ".settings";
        public const String SETTINGS_FILE_STRUCTURE_ERROR = "Configuration File has a wrong internal structure. Error occured at: ";
        public const String SETTINGS_FILE_SAVING_ERROR = "Settings file could not be saved";
        public const String SETTINGS_FILE_LOADING_ERROR = "Settings file could not be loaded";
        public const String SETTINGS_FILE_CREATION_ERROR = "Settings file could not be created";
        public const String SETTINGS_FILE_PREFIX_LOCAL = "Local";
        public const String DEFAULT_MESSAGE_FORM_SELECTION = "Select a form";
        public const String INFRAGISTICS = "Infragistics";
        public const String TREE_STRUCTURE = "TreeStructure";

        //  Queries constants
        public const String QUERIES_ROOT_NODE = "Queries";

        // SD export descriptions
        public const String EXPORTSD_MAIN = "No child tables will be exported.";
        public const String EXPORTSD_CORR = "Each row contains all child data, with multiple child items concatenated into multi-line data items.";
        public const String EXPORTSD_UNCORR = "Each child row occupies a separate record, with data from main rows duplicated as needed.";
        public const String EXPORTSD_NESTED = "Each main row contains all child data, with each child table condensed within a single column.";

        //  Messages
        public const String QUERY_NORESTORE = "This query cannot be restored to the form.";
        public const String LOGIN_NAME_REQD = "User name and password are required";

        // Login MRU
        public const String MRU_NEWSERVER = "<add server...>";
        public const String MRU_DELETE = "<delete selected entry...>";
        public const String MRU_3TPROMPT = "Enter middle-tier server address:";
        public const String MRU_2TPROMPT = "Enter Oracle service name:";
        public const String MODE_2T = "2-Tier";
        public const String MODE_3T = "3-Tier";

        // Variable names for action button arguments
        public const String TOKEN_AUTHTICKET = "AuthenticationTicket";
        public const String TOKEN_USERNAME = "UserName";
        public const String TOKEN_DATABASENAME = "DatabaseName";
        public const String TOKEN_TABLENAME = "TableName";
        public const String TOKEN_FORMNAME = "FormName";
        public const String TOKEN_TODAYSDATE = "TodaysDate";
        public const String TOKEN_APPDATADIR = "AppDataPath";
        public const String TOKEN_EXEDIR = "StartupPath";
        public const String TOKEN_HITLISTID = "HitlistID";
        public const String TOKEN_CURRECNO = "CurrentRecord";
        #endregion
    }
}
