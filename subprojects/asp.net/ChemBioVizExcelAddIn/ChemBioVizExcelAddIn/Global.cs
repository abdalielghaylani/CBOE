using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Drawing;
using Office = Microsoft.Office.Core;
using System.Collections;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COEExportService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using System.Xml.Serialization;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using CambridgeSoft.COE.Framework.ServerControls.Login;
using System.Xml;
using CambridgeSoft.COE.Framework;
namespace ChemBioVizExcelAddIn
{
    /// <summary>
    /// Summary description for Globals
    /// </summary>
    static class Global
    {
        # region enums

        [Flags]
        public enum CBVHeaderRow
        {
            HeaderCategoryRow = 1,
            HeaderTableRow = 2,
            HeaderColumnRow = 3,
            HeaderWhereRow = 4,
            HeaderOptionRow = 5,
            HeaderColumnLabelDisplayRow = 6,
            HeaderResultStartupRow = 7
        }

        [Flags]
        public enum CBVHeaderColorIndex
        {
            HeaderCategoryColorIndex = 17,
            HeaderTableColorIndex = 18,
            HeaderColumnColorIndex = 18,
            HeaderWhereColorIndex = 19,
            HeaderOptionsColorIndex = 17
        }
        [Flags]
        public enum CBVHiddenSheetHeader
        {
            ID = 1,
            Database = 2,
            Dataview = 3,
            CBVNewColIndex = 4,
            Dataviewname = 5,
            Sheetname = 6,
            MaxResultCount = 7,
            CBVActiveSheetIndex = 8,
            Display = 9,
            SerializeCBVResult = 10,
            SearchUpdateCriteria = 11,
            LoginUser = 12,
            SheetCreatedUser = 13,
            ModifiedUser = 14,
            CellDropdownColList = 15,
            SerializeSheetProperties = 16,
            AddInCellDropdownList = 17,
            UID = 18,
            Server = 19,
            Servermode = 20
        }

        [Flags]
        public enum CBVDisplayHeaderOptionNumeric
        {
            [StringValue("All (1 cell)")]
            Disp_All_1cell = 1,
            [StringValue("All (1 cell A-Z)")]
            Disp_All_1cellAZ = 2,
            [StringValue("All (1 cell Z-A)")]
            Disp_All_1cellZA = 3,
            [StringValue("Average")]
            Disp_Avg = 4,
            [StringValue("Min")]
            Disp_Min = 5,
            [StringValue("Max")]
            Disp_Max = 6,
            [StringValue("Median")]
            Disp_Med = 7,
            [StringValue("Count")]
            Disp_Cnt = 8,
            [StringValue("All (Split Cells)")]
            Disp_All_Splitcells = 9,
            [StringValue("All (Split Cells A-Z)")]
            Disp_All_SplitcellsAZ = 10,
            [StringValue("All (Split Cells Z-A)")]
            Disp_All_SplitcellsZA = 11,
            [StringValue("All (Multiple Rows)")]
            Disp_All_Multiplerows = 12,
            [StringValue("All (Multiple Rows A-Z)")]
            Disp_All_MultiplerowsAZ = 13,
            [StringValue("All (Multiple Rows Z-A)")]
            Disp_All_MultiplerowsZA = 14
        }

        [Flags]
        public enum CBVDisplayHeaderOptionText
        {
            [StringValue("All (1 cell)")]
            Disp_All_1cell = 1,
            [StringValue("All (1 cell A-Z)")]
            Disp_All_1cellAZ = 2,
            [StringValue("All (1 cell Z-A)")]
            Disp_All_1cellZA = 3,
            [StringValue("All (Split Cells)")]
            Disp_All_Splitcells = 4,
            [StringValue("All (Split Cells A-Z)")]
            Disp_All_SplitcellsAZ = 5,
            [StringValue("All (Split Cells Z-A)")]
            Disp_All_SplitcellsZA = 6,
            [StringValue("All (Multiple Rows)")]
            Disp_All_Multiplerows = 7,
            [StringValue("All (Multiple Rows A-Z)")]
            Disp_All_MultiplerowsAZ = 8,
            [StringValue("All (Multiple Rows Z-A)")]
            Disp_All_MultiplerowsZA = 9
        }

        [Flags]
        public enum CBVDisplayHeaderOptionStructure
        {
            [StringValue("IDENTITY")]
            Disp_Exact = 1,
            [StringValue("FULL")]
            Disp_Full = 2,
            [StringValue("SIMILARITY")]
            Disp_Similar = 3,
            [StringValue("SUBSTRUCTURE")]
            Disp_Substructure = 4,
            //[StringValue("FORMULA")]
            //Disp_Formula = 5,
            //[StringValue("MOLWEIGHT")]
            //Disp_MolWeight = 6
        }
        [Flags]
        public enum CBVDisplayHeaderRowStructure
        {
            [StringValue(".FORMULA")]
            Disp_Formula = 1,
            [StringValue(".MOLWEIGHT")]
            Disp_MolWeight = 2
        }

        [Flags]
        public enum CBVDisplayHeaderOptionOLE
        {
            //[StringValue("Image (1 cell)")]
            //Img_1cell=1,
            [StringValue("Image (1 cell - 100%)")]
            Img_1cell100 = 1,
            [StringValue("Image (1 cell - 75%)")]
            Img_1cell75 = 2,
            [StringValue("Image (1 cell - 50%)")]
            Img_1cell50 = 3,
            [StringValue("Image (Split Cells)")]
            Img_Splitcells = 4,
            [StringValue("Image (Multiple Cells)")]
            Img_Multiplecells = 5
        }

        [Flags]
        public enum CBVDisplayHeaderOptionDate
        {
            [StringValue("All (1 cell)")]
            Disp_All_1cell = 1,
            [StringValue("All (1 cell A-Z)")]
            Disp_All_1cellAZ = 2,
            [StringValue("All (1 cell Z-A)")]
            Disp_All_1cellZA = 3,
            [StringValue("All (Split Cells)")]
            Disp_All_Splitcells = 4,
            [StringValue("All (Split Cells A-Z)")]
            Disp_All_SplitcellsAZ = 5,
            [StringValue("All (Split Cells Z-A)")]
            Disp_All_SplitcellsZA = 6,
            [StringValue("All (Multiple Rows)")]
            Disp_All_Multiplerows = 7,
            [StringValue("All (Multiple Rows A-Z)")]
            Disp_All_MultiplerowsAZ = 8,
            [StringValue("All (Multiple Rows Z-A)")]
            Disp_All_MultiplerowsZA = 9
        }

        [Flags]
        public enum CBVHeaderOptionNumericCalculation
        {
            [StringValue("AVERAGE")]
            average = 1,
            [StringValue("MIN")]
            minimum = 2,
            [StringValue("MAX")]
            maximum = 3,
            [StringValue("MEDIAN")]
            median = 4,
            [StringValue("COUNT")]
            count = 5
        }

        [Flags]
        public enum CBVSearch
        {
            UpdateCurrentResults = 0,
            AppendNewResults = 1,
            ReplaceAllResults = 2,
            ReplaceMimeTypeResult = 3
        }

        [Flags]
        public enum CBVSplitHeaderOptions
        {
            [StringValue("All (Split Cells)")]
            Disp_All_Splitcells = 1,
            [StringValue("All (Split Cells A-Z)")]
            Disp_All_SplitcellsAZ = 2,
            [StringValue("All (Split Cells Z-A)")]
            Disp_All_SplitcellsZA = 3,
            [StringValue("Image (Split Cells)")]
            Img_Splitcells = 4
        }

        [Flags]
        public enum CBVMultipleHeaderOptions
        {
            [StringValue("All (Multiple Rows)")]
            Disp_All_Multiplerows = 1,
            [StringValue("All (Multiple Rows A-Z)")]
            Disp_All_MultiplerowsAZ = 2,
            [StringValue("All (Multiple Rows Z-A)")]
            Disp_All_MultiplerowsZA = 3,
            [StringValue("Image (Multiple Cells)")]
            Img_Multiplecells = 4
        }

        [Flags]
        public enum CBV_ALL_ASC_HeaderOptions
        {
            [StringValue("All (Split Cells A-Z)")]
            ASC_SplitrowsAZ = 1,
            [StringValue("All (Multiple Rows A-Z)")]
            ASC_MultiplerowsAZ = 2
        }

        [Flags]
        public enum CBV_All_DESC_HeaderOptions
        {
            [StringValue("All (Split Cells Z-A)")]
            DESC_SplitrowsZA = 1,
            [StringValue("All (Multiple Rows Z-A)")]
            DESC_MultiplerowsZA = 2
        }

        [Flags]
        public enum CBVHeaderOptionInstanceDisplay
        {
            [StringValue("All (1 cell)")]
            Disp_All_1cell = 1,
            [StringValue("AVERAGE")]
            average = 2,
            [StringValue("MIN")]
            minimum = 3,
            [StringValue("MAX")]
            maximum = 4,
            [StringValue("MEDIAN")]
            median = 5,
            [StringValue("COUNT")]
            count = 6
        }


        [Flags]
        public enum CBVExcelExportKeys
        {
            Username = 1,
            Password = 2,
            Servername = 3,
            tier = 4,
            SSL = 5,
            DataviewID = 6,
            FieldCriteria = 7,
            Domain = 8,
            DomainFieldName = 9,
            ReturnPartialResults = 10,
            ReturnSimilarityScores = 11,
            SearchOptions = 12,
            AvoidHitList = 13,
            ResultSetID = 14,
            PageSize = 15,
            Start = 16,
            End = 17,
            ResultsFields = 18,
            SheetName = 19
        }

        public enum ContentType
        {
            [StringValue("IMAGE_GIF")]
            IMAGE_GIF,
            [StringValue("IMAGE_PNG")]
            IMAGE_PNG,
            [StringValue("IMAGE_JPEG")]
            IMAGE_JPEG,
            [StringValue("IMAGE_JPG")]
            IMAGE_JPG,
            [StringValue("IMAGE_TIF")]
            IMAGE_TIF,
            [StringValue("IMAGE_TIFF")]
            IMAGE_TIFF,
            [StringValue("IMAGE_WMF")]
            IMAGE_WMF,
            [StringValue("IMAGE_EPS")]
            IMAGE_EPS,
            [StringValue("IMAGE_BMP")]
            IMAGE_BMP

        }
        [Flags]
        public enum ContentTypeFile
        {
            [StringValue("TEXT_XML")]
            TEXT_XML,
            [StringValue("TEXT_HTML")]
            TEXT_HTML,
            [StringValue("TEXT_PLAIN")]
            TEXT_PLAIN,
            [StringValue("APPLICATION_MS_MSWORD")]
            APPLICATION_MS_WORD,
            [StringValue("APPLICATION_MS_EXCEL")]
            APPLICATION_MS_EXCEL,
            [StringValue("APPLICATION_PDF")]
            APPLICATION_PDF
        }
        [Flags]
        public enum ContentTypeExecutable
        {
            [StringValue("WINWORD.EXE")]
            WORD,
            [StringValue("EXCEL.EXE")]
            EXCEL,
            [StringValue("NOTEPAD.EXE")]
            TEXT,
            //[StringValue("NOTEPAD.EXE")]
            [StringValue("IEXPLORE.EXE")]
            XML,
            [StringValue("IEXPLORE.EXE")]
            HTML,
            [StringValue("ACRORD32.EXE")]
            PDF
        }
        [Flags]
        public enum ContentTypeFileExtension
        {
            [StringValue(".doc")]
            WORD,
            [StringValue(".xls")]
            EXCEL,
            [StringValue(".txt")]
            TEXT,
            [StringValue(".xml")]
            XML,
            [StringValue(".htm")]
            HTML,
            [StringValue(".pdf")]
            PDF
        }
        [Flags]
        public enum ConfigurationKey
        {
            [StringValue("MAX_PROMPT_HITS")]
            MAX_PROMPT_HITS,
            [StringValue("MAX_NO_HITS")]
            MAX_NO_HITS,
            [StringValue("STRUCTURE_MAX_HITS")]
            STRUCTURE_MAX_HITS,
            [StringValue("STRUCTURE_MAX_HEIGHT")]
            STRUCTURE_MAX_HEIGHT,
            [StringValue("STRUCTURE_MAX_WIDTH")]
            STRUCTURE_MAX_WIDTH,
            [StringValue("SQL_LOGGING_FILE")]
            SQL_LOGGING_FILE,
            [StringValue("CslaDataPortalProxy")]
            CSLADATAPORTALPROXY,
            [StringValue("CslaDataPortalUrl")]
            CSLADATAPORTALURL,
            [StringValue("ignoreCertificateTrust")]
            IGNORECERTIFICATETRUST,
            [StringValue("MRULIST")]
            MRULIST,
            [StringValue("WorkBookPass")]
            WORKBOOKPASS
        }

        [Flags]
        internal enum ChemOfficeSheetType
        {
            Normal, Reaction, MasterReactant, ExptReactant, ExptProd
        }

        [Flags]
        public enum MRUListConstant
        {
            [StringValue("2-Tier")]
            MRU_2T,
            [StringValue("3-Tier")]
            MRU_3T,
            [StringValue("<Add server...>")]
            ADD_SERVER,
            [StringValue("<Remove selected item...>")]
            REMOVE_ITEM,
            [StringValue("http")]
            HTTP,
            [StringValue("https")]
            HTTPS,
            [StringValue("SSL")]
            SSL
        }
        [Flags]
        public enum ProgressBarStyle
        {
            [StringValue("Block")]
            BLOCK,
            [StringValue("Continuous")]
            CONTINUOUS,
            [StringValue("Marquee")]
            MARQUEE
        }

        // 11.0.3
        [Flags]
        public enum ContentDataType
        {
            Blob, Clob
        }

        public enum ExcelVersion
        {
            [StringValue("11.0")]
            Excel_2003,
            [StringValue("12.0")]
            Excel_2007
        }

        // Advance Export
        [Flags]
        public enum ExportSheetColumns
        {
            [StringValue("A")]
            USERNAME,
            [StringValue("B")]
            SERVERNAME,
            [StringValue("C")]
            TIER,
            [StringValue("D")]
            SSL,
            [StringValue("E")]
            DATAVIEWID,
            [StringValue("F")]
            FIELDCRITERA,
            [StringValue("G")]
            DOMAIN,
            [StringValue("H")]
            DOMAINFIELDNAME,
            [StringValue("I")]
            RETURNSIMILARITYSCORES,
            [StringValue("J")]
            SEARCHOPTIONS,
            [StringValue("K")]
            RESULTFIELDS,
            [StringValue("L")]
            RESULTSETID,
            [StringValue("M")]
            PAGESIZE,
            [StringValue("N")]
            START,
            [StringValue("O")]
            END,
            [StringValue("P")]
            SHEETNAME,
            [StringValue("Q")]
            CHEMBIOVIZEXCELADDIN,
            [StringValue("R")]
            HITLISTQUERYTYPE

        }



        # endregion

        # region Constants

        public const string WORKSHEET_PROP_DOCUMENT = "cdxldocument";
        public const string WORKSHEET_PROP_VERSION = "version";
        public const string WORKSHEET_PROP_TYPE = "sheettype";
        public const string ADDIN_PROG = "ChemDrawExcel14.xla";
        public const string ADDIN_PROG_EXEC = "ChemDrawExcel14.xla!Execute";

        public const string CBVSHEET_PROP_DOCUMENT = "CBVexceldocument";
        public const string COEDATAVIEW_HIDDENSHEET = "_COECBVHidden";
        public const string COEDATAVIEW_CBVEXPORT = "CBVSheetExport";
        public const string COEDATAVIEW_HIDDENSHEET_PASSWORD = "CBVExcelAddIn";

        public const int CBVSHEET_RESULT_STRAT_ROW = 7;
        public const string OPERATOR_RETRIEVE_ALL = "RETRIEVEALL";
        public const string DATARESULT_SUCCESS_STATUS = "SUCCESS";
        public const string DATARESULT_FAILURE_STATUS = "FAILURE";
        public const string COESTRUCTURE_INDEXTYPE = "CS_CARTRIDGE";

        public const char LIST_SELECTION_SEPARATOR = ',';
        public const string SQL_LOGGING_FILE = @"C:\CBVLog.txt";

        public const string DATATYPE = "datatype";
        public const string INDEXTYPE = "indextype";
        public const string MIMITYPE = "mimetype";
        public const string ID = "id";
        public const string NAME = "name";
        public const string ALIAS = "alias";
        public const char SPLITCHAR = '\n';

        public const string CDX_MIME_STRING = "chemical/x-cdx";
        public const string CDXML_MIME_STRING = "text/xml";
        public const string EMF_MIME_STRING = "image/emf";
        public const char CELL_DATA_DELIMITER = '|';
        public const int PAGESIZE = 200;
        public const string ImageNameStartWith = "Picture";
        public const string DataPortalProxy = "Csla.DataPortalClient.WebServicesProxy,  Csla, Version=2.1.1.0, Culture=neutral, PublicKeyToken=93be5fdc093e4c30";
        public const string CHMFilename = "ChemBioVizExcel.chm";
        public const int NoOfRecToProcessOnPBar = 5;

        public const char DOT_SEPARATOR = '.';
        public const char COMMA_SEPARATOR = ',';
        //The escape characters are used to retain the exact alias names of category, table and fields and they work upon the dot and comma.
        public const char ESCAPE_SEPERATOR_FOR_DOT = '\n';
        public const char ESCAPE_SEPERATOR_FOR_COMMA = '\r';

        // 12.3 -- Separator to use in column instead of #
        public const string COLUMN_SEPARATOR = "$-S$E$P-$";
        public const string STRUCTURE = "Structure";
        public const int MaxCellDataLength = 900;
       
        public const int MaxOffice2003Columns = 256;
        public const int MaxOffice2007Columns = 16384;
        //  Web Help URL path
        public const String HELP_URL = "/CBOEHelp/CBOEContextHelp/ChemBioViz Excel Webhelp/default.htm"; //CSBR-157234

        //public const string MACRO_SHAPE_ONACTION = VBA_CALLBACK_ADDIN_FILENAME + "!Shape_OnAction";
        # endregion

        #region AddIn Related Constants

        /*
        /// <summary>
        /// The file name of the VBA addin used as a callback addin.
        /// The handler for shapes on the worksheet are written in this VBA which are
        /// routed back to C# code because we cannot handler the event directly in C#.
        /// </summary>
        internal const string VBA_CALLBACK_ADDIN_FILENAME = "ChemDrawExcel12.xla";

        /// <summary>
        /// The name of the callback addin in COMAddIns collection.
        /// </summary>
        internal const string VBA_CALLBACK_ADDIN_NAME = "ChemDrawExcel12.xla";

        /// <summary>
        /// The program ID of the automation addin (containing user defined functions
        /// for CDXL) in COMAddIns collection.
        /// Used to install the addin
        /// </summary>
        internal const string CDXL_FUNCTION_ADDIN_PROGID = "ChemDrawExcelAddIn12.Functions";

#if DEBUG
        /// <summary>
        /// IN DEBUG MODE
        /// The name of the automation addin (containing user defined functions
        /// for CDXL) in COMAddIns collection.
        /// </summary>
        internal const string CDXL_FUNCTION_ADDIN_NAME = "ChemDrawExcelAddIn12.Functions";
#else
        /// <summary>
        /// The name of the automation addin (containing user defined functions
        /// for CDXL) in COMAddIns collection.
        /// </summary>
        internal const string CDXL_FUNCTION_ADDIN_NAME = "ChemDrawExcelShim.dll";
#endif
        */
        #endregion

        # region Static Variables

        //public static Dictionary<string, COEDataViewBO> DataViewList;

        private static bool _isLogin = false;
        private static bool _isCDXLWorkSheet;
        private static bool _isCBVXLWorkSheet;
        private static bool _insertColumn;
        public static int CBVActiveColumnIndex;
        public static int outlineLevel = 1;
        public static int CBVNewColumnIndex;
        public static int CBVSelectedColumnIndex;
        public static string lastWorkSheetName = String.Empty;
        public static string CurrentWorkSheetName = String.Empty;
        public static ArrayList CBVSplitColIndexList = null; // Use for identify the CBV excel split options column index
        public static ArrayList CBVMultipleColIndexList = null;
        public static ArrayList CBVResultColASC = null;
        public static ArrayList CBVResultColDESC = null;

        public static COEDataViewBO CBVSHEET_COEDATAVIEWBO = null;
        public static string CBVSHEET_DATABASE_NAME = null;
        public static COEDataView CBVSHEET_COEDATAVIEW = null;
        public static string COEDATAVIEW_INDEX = null;
        public static string COEDATAVIEW_NAME = null;
        public static int CBVHIDDENSHEET_INDEX = 0;
        public static string CBVCBVSHEET_NAME = null;
        public static string COEDATAVIEWBOID = null;
        //public static bool ISCBVRESULT_SPLIT_CELLS = false;
        public static bool ISCBVRESULT_MULTIPLE_CELLS = false;

        public static Excel::_Application _ExcelApp = null;

        public static List<string> CBVHeaderCategoryList = new List<string>();
        public static List<string> CBVHeaderTableList = new List<string>();
        public static List<string> CBVHeaderColumnList = new List<string>();
        public static List<string> CBVHeaderOptionList = new List<string>();

        public static List<string> rcTableList = new List<string>();
        public static Dictionary<string, string> rcFields = new Dictionary<string, string>();

        private static double _MaxColumnWidth = 0;
        private static int _MaxRecordInResultSet = 0;

        private static object _structureHeight = 0;
        private static object _structureWidth = 0;
        // public static bool _IsSqlLogging = false;
        public static bool _IsErrorLogging = false;

        public static TableListBO Tables;
        public static TableListBO DVTables;       
        public static COEDataView DataView;
        public static DataTable _CBVResult = null; //Use to cache the searh result while             running the applicaiton        
        public static DataTable _CBVSearchResult = null;
        public static DataResult _dataResult = null;
        public static List<object> SearchUpdateCriteria;// = new List<object>();
        public static List<object> OutputUpdateCriteria = new List<object>();
        public static List<string> SearchCriteria;// = new List<object>();

        public static ArrayList FieldInfo = null;
        // public static Hashtable CBVHiddenCriteriaCollection = null;
        public static Dictionary<int, string> CBVHiddenCriteriaCollection = null;
        public static Dictionary<int, string> CBVExportSheetCollection = null;
        //Default preferences for structure search options

        public static string[] StructureSearchParams = { "HITANYCHARGECARBON=YES", "HITANYCHARGEHETERO=YES", "FRAGMENTSOVERLAP=NO", "IGNOREIMPLICITH=NO", "LOOSEDELOCALIZATION=NO", "PERMITEXTRANEOUSFRAGMENTS=NO", "PERMITEXTRANEOUSFRAGMENTSIFRXN=NO", "REACTIONCENTER=YES", "RELATIVETETSTEREO=NO", "SIMTHRESHOLD=90", "TAUTOMER=NO", "TETRAHEDRALSTEREO=YES", "DOUBLEBONDSTEREO=YES" };

        public static ArrayList StructureSearchOptions = null;

        //11.0.3
        public static string LookupByValueFields = null;
        //

        public static string CurrentUser = null;
        public static string SheetCreatedUser = null;

        public static int HitlistID = 0;
        //public static System.Security.Principal.IPrincipal user=null;

        public static List<int> CellDropdownRange = new List<int>();

        public static bool WorkSheetChange = false;
        public static bool IsUniqueInSearch = false;
        public static bool IsKeyExistForUpdate = false;
        public static string FirstUniqueKeyInSheet = null;
        public static int ToggleColumnAutoSize = 0;
        public static int UID = 0;

        public static string _mruServer = string.Empty;
        public static string _mruServerMode = string.Empty;
        public static string MRUListSeralizeFileName = "CBVEMRUList.xml";
        //public static bool RighClickEnable = false;
        public static object[,] GlobalcellRngVal;
        public static bool ISServerValidated = false;

        // 11.0.4 - Flag for whether the result needs to be updated from framework or not
        public static bool IsUpdatingResultRequired = false;

        public static TreeView TreeDataView;

        public static string CurrentExcelVersion = "11.0";
        public static int RangeCounter = 1;
        public static int CellMaxCharacter = 32767;
        public static char[] charsToTrim = { ' ' };

        #endregion

        #region Public Static Methods
        /// <summary>
        /// Checks whether the given worksheet is ChemDrawExcel 12 worksheet by checking 
        /// the existance of a custom property in it.
        /// </summary>
        /// <param name="worksheet">Worksheet to check</param>
        /// <returns>True if the given worksheet is a ChemDrawExcel 12 worksheet, False otherwise</returns>
        public static bool IsCDExcelWorksheet(Excel::Worksheet worksheet)
        {
            if (null == worksheet) return false;

            int index = 0;
            int count = worksheet.CustomProperties.Count;

            for (; index < count; index++)
            {
                string valueABc = worksheet.CustomProperties[index + 1].Name;

                if (worksheet.CustomProperties[index + 1].Name.CompareTo(Global.WORKSHEET_PROP_DOCUMENT) == 0)
                    break;
            }
            return (index < count);
        }

        public static bool IsCBVExcelWorksheet(Excel::Worksheet worksheet)
        {
            if (null == worksheet) return false;

            int index = 0;
            int count = worksheet.CustomProperties.Count;

            for (; index < count; index++)
            {
                string lvalue = worksheet.CustomProperties[index + 1].Name;
                if (worksheet.CustomProperties[index + 1].Name.CompareTo(Global.CBVSHEET_PROP_DOCUMENT) == 0)
                    break;
            }
            return (index < count);
        }


        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        public static CellData GetCellData(Excel.Range cell)
        {
            CellData celldata = new CellData();

            try
            {
                if ((null != cell) && (null != cell.Comment))
                {
                    // get commented text
                    string cellText = cell.Comment.Text(Type.Missing, Type.Missing, Type.Missing);

                    // break the string into parts
                    cellText = System.Text.Encoding.Default.GetString(Convert.FromBase64String(cellText));
                    string[] values = cellText.Split(CELL_DATA_DELIMITER);
                    if (2 == values.Length)
                    {
                        // celldata.Text = values[0];
                        celldata.objectName = values[0];
                        celldata.Data = (values[1].Length > 0) ? Convert.FromBase64String(values[1]) : null;
                    }
                    if (3 == values.Length)
                    {
                        celldata.cdxData = (values[1].Length > 0) ? Base64Encoder.ToBase64(Convert.FromBase64String(values[2])) : null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return celldata;
        }

        public static CellData GetCellDataCDXL(Excel.Range cell)
        {
            CellData celldata = new CellData();

            try
            {
                if ((null != cell) && (null != cell.Comment))
                {
                    // get commented text
                    string cellText = cell.Comment.Text(Type.Missing, Type.Missing, Type.Missing);

                    // break the string into parts
                    cellText = System.Text.Encoding.Default.GetString(Convert.FromBase64String(cellText));
                    string[] values = cellText.Split(CELL_DATA_DELIMITER);
                    if (2 == values.Length)
                    {
                        // celldata.Text = values[0];
                        celldata.objectName = values[0];
                        celldata.Data = (values[1].Length > 0) ? Convert.FromBase64String(values[1]) : null;
                    }
                    if (3 == values.Length)
                    {
                        celldata.objFormula = values[0];
                        celldata.objectName = values[1];
                        celldata.cdxData = (values[1].Length > 0) ? Base64Encoder.ToBase64(Convert.FromBase64String(values[2])) : null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return celldata;
        }

        public static CellData RemoveCellData(Excel.Range cell)
        {
            CellData celldata = new CellData();

            try
            {
                if ((null != cell) && (null != cell.Comment))
                {
                    // get comment text
                    string cellText = cell.Comment.Text(Type.Missing, Type.Missing, Type.Missing);

                    // break the string into parts
                    cellText = System.Text.Encoding.Default.GetString(Convert.FromBase64String(cellText));
                    string[] values = cellText.Split(CELL_DATA_DELIMITER);
                    if (values.Length > 0)
                    {
                        // celldata.Text = values[0];
                        celldata.objectName = values[1];
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return celldata;
        }


        public static void SetCellData(Excel.Range cell, string objectName, byte[] data)
        {
            try
            {
                cell.ClearComments();
                string commentString = string.Format("{0}{1}{2}{3}{4}",
                    String.Empty,
                    CELL_DATA_DELIMITER,
                    objectName,
                    CELL_DATA_DELIMITER,
                    (null == data) ? string.Empty : Convert.ToBase64String(data));

                commentString = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(commentString));
                cell.AddComment(commentString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Creates a comment string in CDXL comment format
        /// </summary>
        /// <param name="cdxData">Base64 encoded cdx data</param>
        /// <returns>A string in CDXL comment format</returns>
        public static void MakeCDXLComment(Excel.Range cell, string cdxData)
        {
            cell.ClearComments();

            // create comment string in cdxl format
            string commentString = string.Format("{0}{1}{2}{3}{4}", string.Empty,
                "|", string.Empty, "|", cdxData);

            // Base64 encode the whole string
            commentString = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(commentString));

            cell.AddComment(commentString);
            //cell.Value2 = "Structure";
        }

        /// <summary>
        /// Checks if No. of records are less than max structure hits
        /// </summary>
        /// <returns>true or false</returns>
        public static bool IsRecordsLessThanMaxStructureHits(int noOfRecords)
        {
            string key = StringEnum.GetStringValue(ConfigurationKey.STRUCTURE_MAX_HITS);

            int maxStructureHits = Convert.ToInt32(AppConfigSetting.ReadSetting(key));

            if (noOfRecords < maxStructureHits)
                return true;
            else
                return false;
        }



        // <summary>
        /// Converts data from one format to another.
        /// </summary>
        /// <param name="sourceMIME"></param>
        /// <param name="sourceData"></param>
        /// <param name="targetMIME"></param>
        /// <returns></returns>
        public static ReturnedData ConvertUsingCDAX(string sourceMIME, object sourceData, string targetMIME)
        {
            ReturnedData oRet = new ReturnedData();
            ChemDrawControl14.ChemDrawCtl cdaxControl = new ChemDrawControl14.ChemDrawCtl();

            try
            {
                if (null == sourceData)
                {
                    oRet.Data = null;
                    oRet.Message = "Invalid data";
                    return oRet;
                }

                cdaxControl.ViewOnly = false;

                if (null == (sourceData as byte[]))
                {
                    cdaxControl.DataEncoded = true;
                    cdaxControl.set_Data(sourceMIME, sourceData);

                    // The picture is outside the view, bring it back by moving all the objects
                    cdaxControl.Modified = true;
                    cdaxControl.Objects.Select();
                    cdaxControl.RecenterWhenFitting = true;
                    cdaxControl.ShrinkToFit = true;

                    //Set the hide and width of the structure
                    object obk = cdaxControl.Objects.Height;
                    StructureHeight = cdaxControl.Objects.Height;
                    StructureWidth = cdaxControl.Objects.Width;

                }
                else
                {
                    cdaxControl.set_Data(sourceMIME, sourceData);
                }
                oRet.Data = cdaxControl.get_Data(targetMIME);
                oRet.MolWeight = cdaxControl.Objects.MolecularWeight;
                oRet.Formula = cdaxControl.Objects.Formula;
                // if there are no object in the CDAX control, the CDX is invalid
                if (0 == cdaxControl.Objects.Count)
                {
                    oRet.Data = null;
                    oRet.Message = "Invalid data";
                }
            }
            catch (Exception ex)
            {
                oRet.Message = ex.Message;
            }
            finally
            {
                cdaxControl.Objects.Clear();
            }

            return oRet;
        }

        public static ReturnedData ConvertUsingCDAXMOLWTFOR(string sourceMIME, object sourceData)
        {
            ReturnedData oRet = new ReturnedData();
            ChemDrawControl14.ChemDrawCtl cdaxControl = new ChemDrawControl14.ChemDrawCtl();

            try
            {
                if (null == sourceData)
                {
                    oRet.Data = null;
                    oRet.Message = "Invalid data";
                    oRet.MolWeight = 0;
                    oRet.Formula = "Empty";
                    return oRet;
                }

                if (null == (sourceData as byte[]))
                {
                    cdaxControl.DataEncoded = true;
                    cdaxControl.set_Data(sourceMIME, sourceData);
                }
                else
                {
                    cdaxControl.set_Data(sourceMIME, sourceData);
                }

                // if there are no object in the CDAX control, the CDX is invalid
                if (0 == cdaxControl.Objects.Count)
                {
                    oRet.MolWeight = 0;
                    oRet.Formula = "Empty";
                }
                else
                {
                    oRet.MolWeight = cdaxControl.Objects.MolecularWeight;
                    oRet.Formula = cdaxControl.Objects.Formula;
                }
            }
            catch (Exception ex)
            {
                oRet.Message = ex.Message;
            }
            finally
            {
                cdaxControl.Objects.Clear();
            }

            return oRet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] StrToByteArray(object str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str.ToString());
        }

        public static bool IsColumnExists(string colName, DataTable dtTable)
        {
            foreach (DataColumn dtCol in dtTable.Columns)
            {
                if (dtCol.ColumnName.Trim().Equals(colName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsTableExists(string tableName, DataSet dataSet)
        {
            foreach (DataTable dtTables in dataSet.Tables)
            {
                if (dtTables.TableName.Trim().Equals(tableName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        // Checking whether the Data to be entered in Cell exceeds the limit or not
        public static bool IsCellDataLengthExceeds(DataTable dtTab, int colNum, int dataLen)
        {
            try
            {
                foreach (DataRow drTemp in dtTab.Rows)
                {
                    if (drTemp[colNum].ToString().Length > dataLen)
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        # endregion

        # region Public Set and Get Accessor

        /// <summary>
        /// The maximum column width set while displaying image/picture.
        /// </summary>
        public static double MaxColumnWidth
        {
            get { return _MaxColumnWidth; }
            set { _MaxColumnWidth = value; }
        }

        public static int MaxRecordInResultSet
        {
            get { return _MaxRecordInResultSet; }
            set { _MaxRecordInResultSet = value; }
        }

        public static bool IsLogin
        {
            get { return AccessController.Instance.IsLogged; }
            set { _isLogin = value; }
        }

        public static string MRUServer
        {
            get { return AccessController.Instance.Server; }
            // set { _mruServer = value; }
        }
        public static string MRUServerMode
        {
            get
            {
                return AccessController.Instance.ServerMode;
            }
        }

        public static bool InsertColumn
        {
            get { return _insertColumn; }
            set { _insertColumn = value; }
        }

        public static bool IsCDXLWorkSheet
        {
            get { return _isCDXLWorkSheet; }
            set { _isCDXLWorkSheet = value; }
        }
        public static bool IsCBVXLWorkSheet
        {
            get { return _isCBVXLWorkSheet; }
            set { _isCBVXLWorkSheet = value; }
        }

        public static object StructureHeight
        {
            get { return _structureHeight; }
            set { _structureHeight = value; }
        }
        public static object StructureWidth
        {
            get { return _structureWidth; }
            set { _structureWidth = value; }
        }

        public static bool ISErrorLogging
        {
            get { return _IsErrorLogging; }
            set { _IsErrorLogging = value; }
        }


        #endregion

        #region _ Utility functions _

        public static string DefaultConfigurationPath()
        {
            try
            {
                string path = CambridgeSoft.COE.Framework.COEConfigurationService.COEConfigurationBO.ConfigurationBaseFilePath + @"ChemBioVizExcel\";
                return path;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetActualStructureHeaderOption(string optionVal)
        {
            if (optionVal.Trim().Equals("EXACT", StringComparison.OrdinalIgnoreCase))
                //return "IDENTITY=YES"; //IDENTITY
                return "SIMILARITY";
            else if (optionVal.Trim().Equals("FULL", StringComparison.OrdinalIgnoreCase))
                return "FULL";
            else if (optionVal.Trim().Equals("SUBSTRUCTURE", StringComparison.OrdinalIgnoreCase))
                return "SUBSTRUCTURE";
            //return "IDENTITY=NO FullSearch=NO";
            else if (optionVal.Trim().Equals("FORMULA", StringComparison.OrdinalIgnoreCase))
                return "FORMULA";
            else if (optionVal.Trim().Equals("MOLWEIGHT", StringComparison.OrdinalIgnoreCase))
                return "MOLWEIGHT";
            else
                return null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ProperCase(string value)
        {
            TextInfo TI = new CultureInfo("en-US", false).TextInfo;
            return TI.ToTitleCase(value.ToLower());
        }
        public static string ToUpperCase(string value)
        {
            TextInfo TI = new CultureInfo("en-US", false).TextInfo;
            return TI.ToTitleCase(value.ToUpper());
        }

        public static string GetAsciiChar(int value)
        {
            char asciiValue = (char)value;
            return asciiValue.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>

        internal static string NumToString(int colNum)
        {
            if (colNum > 16384) //Maximum col index - office 2007
            {
                throw new Exception("Index exceeds maximum columns allowed.");
            }
            string strColumn;
            char nullChr = '\0';
            char sLetter1;
            char sLetter2;
            char sFirstLetter;
            int iInitialLetter;
            int iFirstLetter;
            int iSecondLetter;
            try
            {
                if (colNum > 702)
                {
                    iInitialLetter = Convert.ToInt32((colNum - 703) / 676);
                    iInitialLetter = iInitialLetter + 65;
                    sFirstLetter = Convert.ToChar(iInitialLetter);
                }
                else
                {
                    sFirstLetter = nullChr;
                }

                iFirstLetter = Convert.ToInt32(((colNum - 1 - 26) % 676) / 26);
                iFirstLetter = iFirstLetter + 65;

                if (iFirstLetter > 64 && colNum > 26)
                {
                    sLetter1 = Convert.ToChar(iFirstLetter);
                }

                else
                {
                    sLetter1 = nullChr;
                }
                //This letter always exists!
                iSecondLetter = Convert.ToChar(colNum % 26);

                if (iSecondLetter == 0)
                    iSecondLetter = 26;

                iSecondLetter = iSecondLetter + 64;
                sLetter2 = Convert.ToChar(iSecondLetter);

                //Puts togehter

                strColumn = Convert.ToString((sFirstLetter != nullChr ? sFirstLetter : nullChr)) + Convert.ToString((sLetter1 != nullChr ? sLetter1 : nullChr)) + Convert.ToString((sLetter2 != nullChr ? sLetter2 : nullChr));
                strColumn = strColumn.Replace(nullChr.ToString(), string.Empty);
                return strColumn.Trim();
            }
            catch
            {
                throw new Exception("Column index exceeds maximum columns");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /*
        public static int stringToNum2(String value)
        {
            int x = 0;
            value = value.ToLower();
            char[] chr = value.ToCharArray();
            for (int i = 0; i < value.Length; ++i)
            {
                x = (x * 26) + (chr[i] - 'a' + 1);
            }
            return (x);
        }*/

        public static int StringToNum(string colName)
        {
            int number = 0;
            int pow = 1;
            for (int i = colName.Length - 1; i >= 0; i--)
            {
                number += (colName[i] - 'A' + 1) * pow;
                pow *= 26;
            }
            return number;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Array1"></param>
        /// <param name="Array2"></param>
        /// <returns></returns>
        public static bool CompareStringArray(string[] Array1, string[] Array2)
        {
            try
            {
                if (Array1.Length.Equals(Array2.Length))
                {
                    for (int x = 0; x < Array1.Length; x++)
                    {
                        if (Array1[x] != Array2[x])
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static List<string> StringToArrayList(string listString)
        {
            if (string.IsNullOrEmpty(listString))
                return null;
            listString = listString.Replace("=MAX", "").Replace("=MIN", "").Replace("=AVERAGE", "").Replace("=COUNT", "").Replace("=MEDIAN", "").Replace("(", "").Replace(")", "").Replace(",", "\n");

            List<string> lstVar = new List<string>();
            foreach (string var in listString.Split(SPLITCHAR))
            {
                if (string.Empty != var)
                    lstVar.Add(var.Trim());
            }
            return lstVar;
        }

        public static void RestoreCSLAPrincipal()
        {
            System.Threading.Thread.CurrentPrincipal = Csla.ApplicationContext.User = (System.Security.Principal.IPrincipal)AppDomain.CurrentDomain.GetData("CURRENT_PRINCIPAL");
        }

        public static void RestoreWindowsPrincipal()
        {
            System.Threading.Thread.CurrentPrincipal = Csla.ApplicationContext.User = (System.Security.Principal.IPrincipal)AppDomain.CurrentDomain.GetData("ORIGINAL_PRINCIPAL");
        }

        public static void SetPrincipal()
        {
            AppDomain.CurrentDomain.SetData("CURRENT_PRINCIPAL", Csla.ApplicationContext.User);
        }

        //Create a new chemoffice sheet
        internal static Excel.Worksheet CreateChemOfficeSheet(string sheetname)
        {
            Excel::Application _Excelapp = _ExcelApp as Excel::Application;
            //Coverity fix - CID 18722
            if (_Excelapp == null)
                throw new System.NullReferenceException();
            Excel::Worksheet worksheet = (Excel::Worksheet)_Excelapp.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            //Coverity fix - CID 18722
            if (worksheet == null)
                throw new System.NullReferenceException();
            // add custom properties to the worksheet
            worksheet.CustomProperties.Add(Global.WORKSHEET_PROP_DOCUMENT, System.Windows.Forms.Application.CompanyName);
            worksheet.CustomProperties.Add(Global.WORKSHEET_PROP_VERSION, System.Windows.Forms.Application.ProductVersion);

            //string worksheetName = worksheet.Name;
            if (GlobalCBVExcel.FindSheet(sheetname) != null)
                throw new Exception(Properties.Resources.msgSheetExists);

            worksheet.Name = sheetname;
            ChemOfficeSheetType worksheetType = ChemOfficeSheetType.Normal;


            // add type property to the worksheet
            worksheet.CustomProperties.Add(Global.WORKSHEET_PROP_TYPE, worksheetType.ToString());
            return worksheet;
        }

        internal static void NewChemOfficeSheet()
        {
            try
            {
                Excel::Application Excelapp = _ExcelApp as Excel::Application;
                //Coverity fix - CID 18729
                if (Excelapp == null)
                    throw new System.NullReferenceException("Excel Application cannot be null");
                Excelapp.Run(ADDIN_PROG_EXEC, 1, new object[] { null }, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception ex)
            {
                throw new Exception(Properties.Resources.msgCBVError + "\n" + ex.Message);
            }

        }

        internal static object AddStructrue(object[] structItems)
        {
            Excel::Application Excelapp = _ExcelApp as Excel::Application;
            //Coverity fix - CID 18726
            if (Excelapp == null)
                throw new System.NullReferenceException();
            return Excelapp.Run(ADDIN_PROG_EXEC, 5, structItems, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

        }


        public static bool ISImageTypeExists(string imageType)
        {
            try
            {
                foreach (ContentType imgType in Enum.GetValues(typeof(ContentType)))
                {
                    if (imageType.Trim().Equals(StringEnum.GetStringValue(imgType), StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool ISImageTypeContains(string dataString)
        {
            try
            {
                foreach (ContentType imgType in Enum.GetValues(typeof(ContentType)))
                {
                    if (dataString.ToUpper().Contains(Global.COLUMN_SEPARATOR + StringEnum.GetStringValue(imgType) + Global.COLUMN_SEPARATOR))
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool ISStructureContains(string dataString)
        {
            try
            {
                if (dataString.ToUpper().Contains(Global.COLUMN_SEPARATOR + Global.COESTRUCTURE_INDEXTYPE + Global.COLUMN_SEPARATOR))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        internal static int GetColumnNumberOnCellRange(string colName)
        {
            if (colName.Contains("$"))
            {
                colName = colName.Substring(colName.IndexOf('$') + 1, colName.LastIndexOf('$') - 1);
            }

            int number = 0; int pow = 1;
            for (int i = colName.Length - 1; i >= 0; i--)
            {
                number += (colName[i] - 'A' + 1) * pow; pow *= 26;
            }
            return number;
        }
        internal static int GetColumnNumberOnColumnRange(string colName)
        {
            if (colName.Contains(":"))
            {
                colName = colName.Substring(colName.IndexOf('$') + 1, colName.LastIndexOf(':') - 1);
            }

            int number = 0; int pow = 1;
            for (int i = colName.Length - 1; i >= 0; i--)
            {
                number += (colName[i] - 'A' + 1) * pow; pow *= 26;
            }
            return number;
        }

        public static bool ISFieldContainsMolWtMolFm(string field)
        {
            field = field.Replace(Global.ESCAPE_SEPERATOR_FOR_DOT, Global.DOT_SEPARATOR);
            if ((field.ToUpper().Contains(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_MolWeight))) || (field.ToUpper().Contains(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_Formula))))
                return true;
            else
                return false;
        }

        public static string FieldContainsMolWtMolFm(string fld)
        {
            string field = fld.Replace(Global.ESCAPE_SEPERATOR_FOR_DOT, Global.DOT_SEPARATOR);
            if (field.ToUpper().Contains(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_MolWeight)))
            {
                fld = field.ToUpper().Replace(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_MolWeight), string.Empty);
                return fld;
            }
            else if (field.ToUpper().Contains(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_Formula)))
            {
                fld = field.ToUpper().Replace(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_Formula), string.Empty);
                return fld;
            }
            else
                return fld;
        }


        public static int StartUpRowPosition(Excel::Worksheet nSheet)
        {
            int value = 0;

            int colCnt = nSheet.UsedRange.Columns.Count;
            if (colCnt < Global.CellDropdownRange.Count)
                colCnt = Global.CellDropdownRange.Count;

            if (colCnt == 1)
                colCnt = 2;

            object[,] cellRngVal = new object[1, colCnt];
            Excel.Range cellRange = nSheet.get_Range("A" + (int)Global.CBVHeaderRow.HeaderColumnRow, Global.NumToString(colCnt) + colCnt);
            cellRngVal = (object[,])cellRange.Value2;


            for (int x = 1; x < colCnt; x++)
            {
                string colAlias = DataSetUtilities.GetDataFromCellRange(cellRngVal, 1, x);

                if (!string.IsNullOrEmpty(colAlias))
                {
                    for (int y = 1; y <= nSheet.UsedRange.Columns.Count; y++)
                    {
                        if (!Global.CellDropdownRange.Contains(value + y))
                            Global.CellDropdownRange.Add(value + y);
                    }
                    return value;


                }
                else
                {
                    value++;
                    //if (value != 0)
                    if (!Global.CellDropdownRange.Contains(value))
                        Global.CellDropdownRange.Add(value);
                }
            }
            return value;
        }

        public static string GetTempFolder()
        {
            string path = string.Empty;
            //Coverity fix - CID 18766
            string temppath = System.Environment.GetEnvironmentVariable("TEMP");
            if (temppath != null)
            {
                path = temppath;
                if (!path.EndsWith("\\"))
                    path += "\\";
            }
            return path;
        }

        public static void SetRangeValue()
        {
            try
            {
                Excel::Worksheet nSheet = Global._ExcelApp.ActiveSheet as Excel.Worksheet;
                //Coverity fix - CID 18730
                if (nSheet == null)
                    throw new System.NullReferenceException("Excel sheet value cannot be null");
                int rowCnt = (int)Global.CBVHeaderRow.HeaderColumnRow;
                int colCnt = nSheet.UsedRange.Columns.Count;

                if (colCnt == 1)
                    colCnt = 2;

                Global.GlobalcellRngVal = new object[rowCnt, colCnt];
                //List<object>[,] cellRngVal = new List<object>[rowCnt, colCnt];

                Excel.Range cellRange = nSheet.get_Range("A1", Global.NumToString(colCnt) + rowCnt);
                //Coverity fix - CID 18730
                if (cellRange == null)
                    throw new System.NullReferenceException("Range value value cannot be enull");
                Global.GlobalcellRngVal = (object[,])cellRange.Value2;

            }
            catch
            {
            }
        }
        public static bool IsCellRangeNull(object[,] cellRangeVal)
        {
            if (cellRangeVal == null)
                return true;
            else
            {
                foreach (string rngVal in cellRangeVal)
                {
                    if (!string.IsNullOrEmpty(rngVal))
                        return false;
                }
            }
            return true;
        }

        public static bool IsValidIP(string strIPAddressField)
        {
            try
            {
                Regex regIP = new Regex(@"(?<First>2[0-4]\d|25[0-5]|[01]?\d\d?)\.(?<Second>2[0-4]\d|25" + @"[0-5]|[01]?\d\d?)\.(?<Third>2[0-4]\d|25[0-5]|[01]?\d\d?)\.(?" + @"<Fourth>2[0-4]\d|25[0-5]|[01]?\d\d?)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
                if (regIP.IsMatch(strIPAddressField))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool CompareIPAddress(IPAddress[] ipAdd1, IPAddress[] ipAdd2)
        {
            try
            {
                for (int i = 0; i < ipAdd1.Length; i++)
                {
                    for (int j = 0; j < ipAdd2.Length; j++)
                    {
                        if (ipAdd1[i].Equals(ipAdd2[j]))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        //11.0.3
        public static void ProtectCDXLWorkSheet(Excel.Worksheet nSheet)
        {
            if (Global.IsCBVExcelWorksheet(nSheet) == true && Global.IsLogin == false)
            {
                Global._ExcelApp.ActiveWorkbook.Protect(AppConfigSetting.ReadSetting(StringEnum.GetStringValue(Global.ConfigurationKey.WORKBOOKPASS)), true, false);
                //nSheet.Protect(AppConfigSetting.ReadSetting(StringEnum.GetStringValue(Global.ConfigurationKey.WORKBOOKPASS)), false, true, true, false, false, true, true, false, false, false, false, false, false, false, false);
            }
            else
            {
                Global._ExcelApp.ActiveWorkbook.Protect(AppConfigSetting.ReadSetting(StringEnum.GetStringValue(Global.ConfigurationKey.WORKBOOKPASS)), false, false); //Fixed CSBR - 155618
            }
        }


        public static bool ISMimeTypeExists(string dataString)
        {
            try
            {
                foreach (ContentTypeFile contType in Enum.GetValues(typeof(ContentTypeFile)))
                {
                    if (dataString.Equals(StringEnum.GetStringValue(contType), StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        //11.0.3
        public static bool ISContentTypeContains(string dataString)
        {
            try
            {
                foreach (ContentTypeFile contType in Enum.GetValues(typeof(ContentTypeFile)))
                {
                    // 12.3 - Proper Separator used instead of #
                    if (dataString.ToUpper().Contains(Global.COLUMN_SEPARATOR + StringEnum.GetStringValue(contType) + Global.COLUMN_SEPARATOR))
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        //11.0.3
        public static bool ISContentTypeFileExists(string contentType)
        {
            try
            {
                foreach (ContentTypeFile contType in Enum.GetValues(typeof(ContentTypeFile)))
                {
                    if (contentType.Trim().Equals(StringEnum.GetStringValue(contType), StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        //11.0.3
        public static string GetContentType(string dataString)
        {
            try
            {
                foreach (ContentTypeFile contType in Enum.GetValues(typeof(ContentTypeFile)))
                {
                    // 12.3 - Proper Separator used instead of #
                    if (dataString.ToUpper().Contains(Global.COLUMN_SEPARATOR + StringEnum.GetStringValue(contType) + Global.COLUMN_SEPARATOR))
                        return StringEnum.GetStringValue(contType);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        //11.0.3
        public static Excel.Shape GetContentTypeFileShape(string contentType, Excel.Range nRange, Excel.Worksheet nSheet)
        {
            string path = string.Empty;
            Excel.Shape shape = null;
            switch (contentType)
            {
                case "APPLICATION_MS_MSWORD":
                    path = Global.GetAssemblyLocation() + @"\Resources\Word_.png";
                    break;
                case "APPLICATION_MS_EXCEL":
                    path = Global.GetAssemblyLocation() + @"\Resources\Excel_.png";
                    break;
                case "APPLICATION_PDF":
                    path = Global.GetAssemblyLocation() + @"\Resources\PDF_.png";
                    break;
                case "TEXT_HTML":
                    path = Global.GetAssemblyLocation() + @"\Resources\HTML_.png";
                    break;
                case "TEXT_XML":
                    path = Global.GetAssemblyLocation() + @"\Resources\XML_.png";
                    break;
                case "TEXT_PLAIN":
                    path = Global.GetAssemblyLocation() + @"\Resources\TEXT_.png";
                    break;
                default:
                    path = Global.GetAssemblyLocation() + @"\Resources\TEXT_.png";
                    break;
            }
            try
            {
                shape = nSheet.Shapes.AddPicture(path, Office.MsoTriState.msoFalse, Office.MsoTriState.msoTrue, (float)(double)nRange.Left, (float)(double)nRange.Top, 15, 10);
                shape.Placement = Microsoft.Office.Interop.Excel.XlPlacement.xlMoveAndSize;
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
            return shape;
        }



        //11.0.3
        public static void OpenFileOnMimeType(string mimeType, StringBuilder data, Excel.Range nTarget, Excel.Worksheet nSheet)
        {
            string fileName = Global.NumToString(nTarget.Column).ToString() + nTarget.Row.ToString();
            try
            {
                switch (mimeType)
                {
                    case "APPLICATION_MS_MSWORD":
                        WriteContentInFile(ContentDataType.Blob.ToString(), data, fileName, StringEnum.GetStringValue(ContentTypeFileExtension.WORD), StringEnum.GetStringValue(ContentTypeExecutable.WORD));
                        break;
                    case "APPLICATION_MS_EXCEL":
                        WriteContentInFile(ContentDataType.Blob.ToString(), data, fileName, StringEnum.GetStringValue(ContentTypeFileExtension.EXCEL), StringEnum.GetStringValue(ContentTypeExecutable.EXCEL));
                        break;
                    case "APPLICATION_PDF":
                        WriteContentInFile(ContentDataType.Blob.ToString(), data, fileName, StringEnum.GetStringValue(ContentTypeFileExtension.PDF), StringEnum.GetStringValue(ContentTypeExecutable.PDF));
                        break;
                    case "TEXT_HTML":
                        WriteContentInFile(ContentDataType.Clob.ToString(), data, fileName, StringEnum.GetStringValue(ContentTypeFileExtension.HTML), StringEnum.GetStringValue(ContentTypeExecutable.TEXT));
                        break;
                    case "TEXT_XML":
                        WriteContentInFile(ContentDataType.Clob.ToString(), data, fileName, StringEnum.GetStringValue(ContentTypeFileExtension.XML), StringEnum.GetStringValue(ContentTypeExecutable.TEXT));
                        break;
                    case "TEXT_PLAIN":
                        WriteContentInFile(ContentDataType.Clob.ToString(), data, fileName, StringEnum.GetStringValue(ContentTypeFileExtension.TEXT), StringEnum.GetStringValue(ContentTypeExecutable.TEXT));
                        break;
                    default:
                        break;
                }
            }
            catch (FileLoadException ex)
            {
                throw new Exception("The file " + ex.FileName + " already open");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //11.0.3
        internal static void WriteContentInFile(string contentDataType, StringBuilder data, string fileName, string fileExtension, string executable)
        {
            try
            {
                byte[] blob = null;

                // 11.0.3
                //when datatype is BLOB or CLOB the data is saved in DB with Base64 encoding
                if (contentDataType.Equals(ContentDataType.Blob.ToString()) || contentDataType.Equals(ContentDataType.Clob.ToString()))
                {
                    blob = Convert.FromBase64String(data.ToString());
                }
                else
                {
                    blob = System.Text.Encoding.Unicode.GetBytes(data.ToString());
                }

                string path = Path.GetTempPath();
                string fullFilePath = path + fileName + fileExtension;
                using (FileStream writer = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
                {
                    writer.Write(blob, 0, blob.Length);
                    writer.Close();
                }
                string quotedPath = "\"" + fullFilePath + "\"";

                //Just use the default application and pass the filepath
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = quotedPath;

                startInfo.ErrorDialog = true;

                //special case Excel.  For some reason when you specify just the file path and no exe
                //Excel doesn't like it.  Perhaps some kind of context issue.  For this use the path to excel
                //and exe name.  Then specify the file path as an argument
                if (fileExtension.ToLower() == ".xls" || fileExtension.ToLower() == ".xlsx")
                {
                    string p = string.Concat(_ExcelApp.Path, @"\", "Excel.exe");
                    startInfo.FileName = p;
                    startInfo.Arguments = quotedPath;
                }

                Process proc = new Process();
                proc.StartInfo = startInfo;
                proc.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // 11.0.4 - Getting the SearchCriteria
        // 11.0.4 - uncommented as full api search is not required
        /*public static void GetSearchCriteria(ref SearchCriteria searchCriteria, string[] searchCriteriaFields, string[] searchOptions, string[] domain, string domainFieldName, int dataViewID, bool avoidHitList)
        {
            COEDataViewBO dataViewBO = null;
            try
            {
                Global.RestoreCSLAPrincipal();
                //First thing needed will be a dataview from the id
                dataViewBO = COEDataViewBO.Get(dataViewID);
            }
            catch (Csla.DataPortalException ex)
            {
                throw new Exception("FAILURE: DataView with id " + dataViewID + " was not found.\n" + ex.BusinessException.Message);
            }
            finally
            {
                Global.RestoreWindowsPrincipal();
            }

            if ((searchCriteriaFields == null || searchCriteriaFields.Length < 1) && (domain == null || domain.Length < 1))
            {
                throw new Exception("FAILURE: No criteria and no domain entered. Please check your input and privide at least one of them.");
            }

            try
            {
            
                searchCriteria = InputFieldsToSearchCriteria.GetSearchCriteria(searchCriteriaFields, searchOptions, domain, domainFieldName, dataViewBO.COEDataView);

                if (avoidHitList)
                    searchCriteria = null;

            }
            catch (Exception ex)
            {
                throw new Exception("FAILURE: Could not build the search criteria with the provided input.\n" + ex.Message);
            }
        }

        // 11.0.4 - Getting the ResultCriteria for hist list (based upon primary field only)
        public static void GetResultCriteriaForHitList(ref ResultsCriteria resultsCriteria, ref ResultsCriteria resultsCriteriaWithoutSimilarity, string pkField, string[] searchCriteriaFields, int dataViewID, bool returnSimilarityScores)
        {
            //these will need to get looped and converted to results criteria
            //resultFields            

            COEDataViewBO dataViewBO = null;
            try
            {
                Global.RestoreCSLAPrincipal();

                //First thing needed will be a dataview from the id

                dataViewBO = COEDataViewBO.Get(dataViewID);
            }
            catch (Csla.DataPortalException ex)
            {
                throw new Exception("FAILURE: DataView with id " + dataViewID + " was not found.\n" + ex.BusinessException.Message);
            }
            finally
            {
                Global.RestoreWindowsPrincipal();
            }

            try
            {
                string[] resultFields = new string[] { pkField };

                resultsCriteria = ResultFieldsToResultsCriteria.GetResultCriteria(resultFields, dataViewBO.COEDataView);
                resultsCriteriaWithoutSimilarity = ResultFieldsToResultsCriteria.GetResultCriteria(resultFields, dataViewBO.COEDataView);

                if (returnSimilarityScores)
                    InputFieldsToSearchCriteria.AddSimilarityIfNecessary(searchCriteriaFields, ref resultsCriteria, dataViewBO.COEDataView);

            }
            catch (Exception ex)
            {
                throw new Exception("FAILURE: Could not build the result criteria with the provided result fields.\n" + ex.Message);
            }

        }

        // 11.0.4 - Getting the ResultCriteria for all the fields of sheet
        public static void GetResultCriteria(ref ResultsCriteria resultsCriteria, string[] resultCriteriaFields, int dataViewID)
        {
            //these will need to get looped and converted to results criteria
            //resultFields            

            COEDataViewBO dataViewBO = null;
            try
            {
                Global.RestoreCSLAPrincipal();

                //First thing needed will be a dataview from the id

                dataViewBO = COEDataViewBO.Get(dataViewID);
            }
            catch (Csla.DataPortalException ex)
            {
                throw new Exception("FAILURE: DataView with id " + dataViewID + " was not found.\n" + ex.BusinessException.Message);
            }
            finally
            {
                Global.RestoreWindowsPrincipal();
            }

            try
            {
                resultsCriteria = ResultFieldsToResultsCriteria.GetResultCriteria(resultCriteriaFields, dataViewBO.COEDataView);               
            }
            catch (Exception ex)
            {
                throw new Exception("FAILURE: Could not build the result criteria with the provided result fields.\n" + ex.Message);
            }

        }

        // 11.0.4 - Getting the SearchOptions
        public static string[] GetSearchOptions()
        {
            string[] searchOptions = StructureSearchOption.GetStructureSearchOptionParam();

            //11.0.3
            //Global.LookupByValueFields
            int optionLen = searchOptions.Length;
            Array.Resize(ref searchOptions, optionLen + 1);
            searchOptions.SetValue(Global.LookupByValueFields.Remove(Global.LookupByValueFields.Length - 1, 1), optionLen);

            return searchOptions;

        }

        // 11.0.4 - Converting the Dataset to StringBuilder
        public static StringBuilder DataSetToStringBuilder(DataSet resultsDataSet, string tabAlias, string colAlias)
        {
            StringBuilder data = null;
            DataSet resultDataSet = new DataSet();

            try
            {
                string ResultSet = resultsDataSet.GetXml();

                using (System.IO.StringReader stringReader = new System.IO.StringReader(ResultSet))
                {
                    resultDataSet.ReadXml(stringReader);
                }


                foreach (DataTable dt in resultDataSet.Tables)
                {
                    if (dt.TableName.Equals(tabAlias, StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (DataColumn dc in resultDataSet.Tables[dt.TableName].Columns)
                        {
                            if (dc.ColumnName.Equals(colAlias, StringComparison.OrdinalIgnoreCase))
                            {
                                data = new StringBuilder(resultDataSet.Tables[dt.TableName].Rows[0][dc].ToString());
                                return data;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (resultDataSet != null)
                    resultDataSet.Dispose();

            }
            return data;
        }*/

        public static string GetAssemblyLocation()
        {
            try
            {
                //Get the assembly information
                System.Reflection.Assembly assemblyInfo = System.Reflection.Assembly.GetExecutingAssembly();

                //Location is where the assembly is run from 
                string assemblyLocation = assemblyInfo.Location;

                //CodeBase is the location of the ClickOnce deployment files
                Uri uriCodeBase = new Uri(assemblyInfo.CodeBase);
                string AssemblyLocation = System.IO.Path.GetDirectoryName(uriCodeBase.LocalPath.ToString());
                return AssemblyLocation;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion " _ Utility functions _"
        #region "ProgressBar Methods"
        /*
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        public static System.Windows.Forms.ProgressBar downloadProgressBar = null;

        // marque progress bar on status bar
        /// <summary> 
        /// Shows a marquee progress bar on the status bar of current excel process. 
        /// </summary> 
        public static void ShowProgressBar(string title)
        {
            
            IntPtr xlMain = IntPtr.Zero;
            IntPtr hwndStatusbar = IntPtr.Zero;
            IntPtr hDcStatusBar = IntPtr.Zero;
            string statusbarName = title;
            try
            {
                downloadProgressBar = new System.Windows.Forms.ProgressBar();
                downloadProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
                downloadProgressBar.Top = 2;
                downloadProgressBar.Left = 500;
                downloadProgressBar.Height = 15;
                downloadProgressBar.Width = 100;
                _ExcelApp.DisplayStatusBar = true;
                _ExcelApp.StatusBar = title;
                
                switch (_ExcelApp.Version)
                {
                    case "10.0": // Office XP 
                        statusbarName = "Excel4";
                        break;
                    case "11.0": // Office 2003 
                        statusbarName = "Excel4";
                        break;
                    case "12.0": // Office 2007 
                        statusbarName = "Excel2";
                        downloadProgressBar.Top = 3;
                        
                        break;
                }
                xlMain = Process.GetCurrentProcess().MainWindowHandle;
               // hwndStatusbar = FindWindowEx(xlMain, IntPtr.Zero, statusbarName, "Result Processing");
                hwndStatusbar = FindWindowEx(xlMain, IntPtr.Zero, statusbarName, null);
                if (hwndStatusbar != IntPtr.Zero)
                    SetParent(downloadProgressBar.Handle, hwndStatusbar);
                //else if (_log.IsWarnEnabled) _log.Warn("Status bar not found.");

            }
            catch { }
        }

        public static void UpdateProgress()
        {
            if (downloadProgressBar == null)
                return;
            if (downloadProgressBar.InvokeRequired)
            {
                MethodInvoker updateProgress = UpdateProgress;
                downloadProgressBar.Invoke(updateProgress);
            }
            else
            {
               // downloadProgressBar.Increment(1);
                ShowProgressBar("Processing");

            }
        }

        public  static void  IncrementMe()
        {
            for (int i = 0; i < 100; i++)
            {
                UpdateProgress();
                Thread.Sleep(100);
               
            }


            if (downloadProgressBar.InvokeRequired)
            {
                downloadProgressBar.Invoke(new MethodInvoker(downloadProgressBar.Dispose));
            }
            else
            {
                downloadProgressBar.Dispose();
            }


        }
        /// <summary> 
        /// Disposes the progressbar on current excel process. 
        /// </summary> 
        public static void HideProgressBar()
        {
            _ExcelApp.StatusBar = null;
            if (null != downloadProgressBar)
            {
                if (downloadProgressBar.InvokeRequired)
                {
                    downloadProgressBar.Invoke(new MethodInvoker(HideProgressBar));

                    return;
                }
                else
                {
                    downloadProgressBar.Dispose();
                }
            }

        }
        

        public static void ShowProgressStatus(int rowNum, int totalRow)
        { 
            _ExcelApp.StatusBar = String.Format("Processing row: {0} of {1}", rowNum, totalRow);

        }
        */
        #endregion "ProgressBar Methods"

        #region "Nested/Inner class"
        //Class to derieved from process class as well as it's own functionality
        public class CHMProcess : Process
        {
            private static CHMProcess inst = null;
            public static bool isExists = false;
            static readonly object padlock = new object();

            // Instance for singleton
            public static CHMProcess Inst
            {
                get
                {
                    lock (padlock)
                    {
                        if (inst == null)
                        {
                            inst = new CHMProcess();
                        }
                        else
                        {
                            CHMProcess.isExists = false;
                        }
                        return inst;
                    }
                }
            }
            //constructor
            public CHMProcess()
            {
                CHMProcess.isExists = true;
            }

            #region "Methods"
            //Call win API - to display the different forms of window
            [DllImport("user32.dll")]
            internal static extern bool ShowWindow(IntPtr hWnd, SHOW_WINDOW nCmdShow);
            internal enum SHOW_WINDOW
            {
                SW_HIDE = 0,
                SW_SHOWNORMAL = 1,
                SW_NORMAL = 1,
                SW_SHOWMINIMIZED = 2,
                SW_SHOWMAXIMIZED = 3,
                SW_MAXIMIZE = 3,
                SW_SHOWNOACTIVATE = 4,
                SW_SHOW = 5,
                SW_MINIMIZE = 6,
                SW_SHOWMINNOACTIVE = 7,
                SW_SHOWNA = 8,
                SW_RESTORE = 9,
                SW_SHOWDEFAULT = 10,
                SW_FORCEMINIMIZE = 11,
                SW_MAX = 11
            }
            #endregion "Methods"
        }
        #endregion "Nested/Inner class";
    }

    class Base64Encoder
    {
        static public string ToBase64(byte[] data)
        {
            int length = data == null ? 0 : data.Length;
            if (length == 0)
                return String.Empty;

            int padding = length % 3;
            if (padding > 0)
                padding = 3 - padding;
            int blocks = (length - 1) / 3 + 1;

            char[] s = new char[blocks * 4];

            for (int i = 0; i < blocks; i++)
            {
                bool finalBlock = i == blocks - 1;
                bool pad2 = false;
                bool pad1 = false;
                if (finalBlock)
                {
                    pad2 = padding == 2;
                    pad1 = padding > 0;
                }

                int index = i * 3;
                byte b1 = data[index];
                byte b2 = pad2 ? (byte)0 : data[index + 1];
                byte b3 = pad1 ? (byte)0 : data[index + 2];

                byte temp1 = (byte)((b1 & 0xFC) >> 2);

                byte temp = (byte)((b1 & 0x03) << 4);
                byte temp2 = (byte)((b2 & 0xF0) >> 4);
                temp2 += temp;

                temp = (byte)((b2 & 0x0F) << 2);
                byte temp3 = (byte)((b3 & 0xC0) >> 6);
                temp3 += temp;

                byte temp4 = (byte)(b3 & 0x3F);

                index = i * 4;
                s[index] = SixBitToChar(temp1);
                s[index + 1] = SixBitToChar(temp2);
                s[index + 2] = pad2 ? '=' : SixBitToChar(temp3);
                s[index + 3] = pad1 ? '=' : SixBitToChar(temp4);
            }

            return new string(s);
        }

        static private char SixBitToChar(byte b)
        {
            char c;
            if (b < 26)
            {
                c = (char)((int)b + (int)'A');
            }
            else if (b < 52)
            {
                c = (char)((int)b - 26 + (int)'a');
            }
            else if (b < 62)
            {
                c = (char)((int)b - 52 + (int)'0');
            }
            else if (b == 62)
            {
                c = s_CharPlusSign;
            }
            else
            {
                c = s_CharSlash;
            }
            return c;
        }

        static public byte[] FromBase64(string s)
        {
            int length = s == null ? 0 : s.Length;
            if (length == 0)
                return new byte[0];

            int padding = 0;
            if (length > 2 && s[length - 2] == '=')
                padding = 2;
            else if (length > 1 && s[length - 1] == '=')
                padding = 1;

            int blocks = (length - 1) / 4 + 1;
            int bytes = blocks * 3;

            byte[] data = new byte[bytes - padding];

            for (int i = 0; i < blocks; i++)
            {
                bool finalBlock = i == blocks - 1;
                bool pad2 = false;
                bool pad1 = false;
                if (finalBlock)
                {
                    pad2 = padding == 2;
                    pad1 = padding > 0;
                }

                int index = i * 4;
                byte temp1 = CharToSixBit(s[index]);
                byte temp2 = CharToSixBit(s[index + 1]);
                byte temp3 = CharToSixBit(s[index + 2]);
                byte temp4 = CharToSixBit(s[index + 3]);

                byte b = (byte)(temp1 << 2);
                byte b1 = (byte)((temp2 & 0x30) >> 4);
                b1 += b;

                b = (byte)((temp2 & 0x0F) << 4);
                byte b2 = (byte)((temp3 & 0x3C) >> 2);
                b2 += b;

                b = (byte)((temp3 & 0x03) << 6);
                byte b3 = temp4;
                b3 += b;

                index = i * 3;
                data[index] = b1;
                if (!pad2)
                    data[index + 1] = b2;
                if (!pad1)
                    data[index + 2] = b3;
            }

            return data;
        }

        static private byte CharToSixBit(char c)
        {
            byte b;
            if (c >= 'A' && c <= 'Z')
            {
                b = (byte)((int)c - (int)'A');
            }
            else if (c >= 'a' && c <= 'z')
            {
                b = (byte)((int)c - (int)'a' + 26);
            }
            else if (c >= '0' && c <= '9')
            {
                b = (byte)((int)c - (int)'0' + 52);
            }
            else if (c == s_CharPlusSign)
            {
                b = (byte)62;
            }
            else
            {
                b = (byte)63;
            }
            return b;
        }

        static private char s_CharPlusSign = '+';
        /// <summary>
        /// Gets or sets the plus sign character.
        /// Default is '+'.
        /// </summary>
        static public char CharPlusSign
        {
            get
            {
                return s_CharPlusSign;
            }
            set
            {
                s_CharPlusSign = value;
            }
        }

        static private char s_CharSlash = '/';
        /// <summary>
        /// Gets or sets the slash character.
        /// Default is '/'.
        /// </summary>
        static public char CharSlash
        {
            get
            {
                return s_CharSlash;
            }
            set
            {
                s_CharSlash = value;
            }
        }
    }

    [Serializable]
    public class SerializeDeserialize
    {
        //Serialize the data
        public static string Serialize(object value)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                StringWriter sw = new StringWriter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, value);
                //convert the stream to a string
                Byte[] bytes = ms.ToArray();
                string serializeData = Convert.ToBase64String(bytes);
                return serializeData;
            }
            catch
            {
                return null;
            }
        }

        //Deserialize the data
        public static object Deserialize(string value)
        {
            try
            {
                MemoryStream ms = new MemoryStream(Convert.FromBase64String(value));
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(ms);
            }
            catch  
            {
                return null; //Fixed CSBR-152910
            }
        }

        //Compress the string
        public static string Compress(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            MemoryStream ms = new MemoryStream();
            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }

            ms.Position = 0;
            MemoryStream outStream = new MemoryStream();
            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            byte[] gzBuffer = new byte[compressed.Length + 4];
            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(gzBuffer);
        }

        //Deompress the string
        public static string Decompress(string compressedText)
        {
            byte[] gzBuffer = Convert.FromBase64String(compressedText);
            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                byte[] buffer = new byte[msgLength];

                ms.Position = 0;
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }
                return Encoding.UTF8.GetString(buffer);
            }
        }
    }

    #region _  Structure Preferences  _
    public class StructureSearchOption
    {
        public StructureSearchOption() { }


        #region Variables

        //private static string _implementation = "CSCARTRIDGE";
        //private string cartridgeSchema;
        //private static bool _absoluteHitsRel = false;
        private static bool _doubleBondStereo = true;
        private static bool _fragmentsOverlap = false;
        //private static bool _full = false;
        //private static string _fullSearch = "SUBSTRUCTURE";
        //private static bool _identity = false;
        //private static bool _similar = false;
        private static bool _hitAnyChargeCarbon = true;
        private static bool _hitAnyChargeHetero = true;
        private static bool _permitExtraneousFragments = false;
        private static bool _permitExtraneousFragmentsIfRXN = false;
        private static bool _reactionCenter = true;

        private static bool _tautomeric = false;

        private static bool _matchTetrahedralStereo = true;
        private static bool _tetrahedralStereo = true; //[YES|NO|SAME|EITHER|ANY] 
        private static bool _tetrahedralStereoHitsSame = true;
        private static bool _tetrahedralStereoHitsEither = false;
        private static bool _tetrahedralStereoHitsAny = false;
        private static bool _relativeTetStereo = false;

        //private string _ignoreImplicitHydrogens="NO";
        //private static bool _ignoreImplicith = false;
        //private static bool _looseDeLocalization = false;

        private static bool _doubleBondHitsSame = false;
        private static bool _doubleBondHitsAny = true;

        private static string _similarSearchThld = "90";

        private static Int64 _structureMaxHeight, _structureMaxWidth;
        //private static int _massMax = 0;
        //private static int _massMin = 0;
        //private static int _complete = 0;
        //private static int _simThreshold = 90;

        #endregion

        #region "Gets or sets the search options"


        public static bool FragmentsOverlap
        {
            get { return _fragmentsOverlap; }
            set
            {
                if (value)
                    Global.StructureSearchOptions.Add("FRAGMENTSOVERLAP=YES");
                else
                    Global.StructureSearchOptions.Add("FRAGMENTSOVERLAP=NO");
                _fragmentsOverlap = value;
            }
        }

        public static bool HitAnyChargeCarbon
        {
            get { return _hitAnyChargeCarbon; }
            set
            {
                if (value)
                    Global.StructureSearchOptions.Add("HITANYCHARGECARBON=YES");
                else
                    Global.StructureSearchOptions.Add("HITANYCHARGECARBON=NO");
                _hitAnyChargeCarbon = value;
            }
        }

        public static bool HitAnyChargeHetero
        {
            get { return _hitAnyChargeHetero; }
            set
            {
                if (value)
                    Global.StructureSearchOptions.Add("HITANYCHARGEHETERO=YES");
                else
                    Global.StructureSearchOptions.Add("HITANYCHARGEHETERO=NO");
                _hitAnyChargeHetero = value;
            }
        }

        public static bool PermitExtraneousFragments
        {
            get { return _permitExtraneousFragments; }
            set
            {
                if (value)
                    Global.StructureSearchOptions.Add("PERMITEXTRANEOUSFRAGMENTS=YES");
                else
                    Global.StructureSearchOptions.Add("PERMITEXTRANEOUSFRAGMENTS=NO");
                _permitExtraneousFragments = value;
            }
        }

        public static bool PermitExtraneousFragmentsIfRXN
        {
            get { return _permitExtraneousFragmentsIfRXN; }
            set
            {
                if (value)
                    Global.StructureSearchOptions.Add("PERMITEXTRANEOUSFRAGMENTSIFRXN=YES");
                else
                    Global.StructureSearchOptions.Add("PERMITEXTRANEOUSFRAGMENTSIFRXN=NO");
                _permitExtraneousFragmentsIfRXN = value;
            }
        }

        public static bool ReactionCenter
        {
            get { return _reactionCenter; }
            set
            {
                if (value)
                    Global.StructureSearchOptions.Add("REACTIONCENTER=YES");
                else
                    Global.StructureSearchOptions.Add("REACTIONCENTER=NO");
                _reactionCenter = value;
            }
        }

        public static bool RelativeTetStereo
        {
            get { return _relativeTetStereo; }
            set
            {
                if (value)
                    Global.StructureSearchOptions.Add("RELATIVETETSTEREO=YES");
                else
                    Global.StructureSearchOptions.Add("RELATIVETETSTEREO=NO");
                _relativeTetStereo = value;
            }
        }

        public static bool Tautomeric
        {
            get { return _tautomeric; }
            set
            {
                if (value)
                    Global.StructureSearchOptions.Add("TAUTOMER=YES");
                else
                    Global.StructureSearchOptions.Add("TAUTOMER=NO");
                _tautomeric = value;
            }
        }
        public static bool MatchTetrahedralStereo
        {
            get { return _matchTetrahedralStereo; }
            set { _matchTetrahedralStereo = value; }
        }

        public static bool TetrahedralStereo
        {
            get { return _tetrahedralStereo; }
            set
            {
                if (value)
                    Global.StructureSearchOptions.Add("TETRAHEDRALSTEREO=YES");
                else
                    Global.StructureSearchOptions.Add("TETRAHEDRALSTEREO=NO");
                _tetrahedralStereo = value;
            }
        }

        public static bool TetrahedralStereoHitsSame
        {
            get { return _tetrahedralStereoHitsSame; }
            set
            {
                if (value)
                    Global.StructureSearchOptions.Add("TETRAHEDRALSTEREO=SAME");

                _tetrahedralStereoHitsSame = value;
            }
        }

        public static bool TetrahedralStereoHitsAny
        {
            get { return _tetrahedralStereoHitsAny; }
            set
            {
                if (value)
                    Global.StructureSearchOptions.Add("TETRAHEDRALSTEREO=ANY");
                _tetrahedralStereoHitsAny = value;
            }
        }

        public static bool TetrahedralStereoHitsEither
        {
            get { return _tetrahedralStereoHitsEither; }
            set
            {
                if (value)
                    Global.StructureSearchOptions.Add("TETRAHEDRALSTEREO=EITHER");
                _tetrahedralStereoHitsEither = value;
            }
        }

        public static bool DoubleBondStereo
        {
            get { return _doubleBondStereo; }
            set
            {
                if (value)
                    Global.StructureSearchOptions.Add("DOUBLEBONDSTEREO=YES");
                else
                    Global.StructureSearchOptions.Add("DOUBLEBONDSTEREO=NO");
                _doubleBondStereo = value;
            }
        }

        public static bool DoubleBondHitsAny
        {
            get { return _doubleBondHitsAny; }
            set
            {
                if (value)
                    Global.StructureSearchOptions.Add("DOUBLEBONDSTEREO=ANY");
                _doubleBondHitsAny = value;
            }
        }

        public static bool DoubleBondHitsSame
        {
            get { return _doubleBondHitsSame; }
            set
            {
                if (value)
                    Global.StructureSearchOptions.Add("DOUBLEBONDSTEREO=SAME");
                _doubleBondHitsSame = value;
            }
        }

        public static string SimilarSearchThld
        {
            get { return _similarSearchThld; }
            set
            {
                _similarSearchThld = value;
                Global.StructureSearchOptions.Add("SIMTHRESHOLD=" + _similarSearchThld.Trim());
            }
        }

        //The default height and widht of structure is 50 and 100 respectively. It's only set if not specified in config or Setup CBV options
        public static Int64 StructureMaxHeight
        {

            get
            {
                try
                {
                    _structureMaxHeight = Convert.ToInt64(AppConfigSetting.ReadSetting(StringEnum.GetStringValue(Global.ConfigurationKey.STRUCTURE_MAX_HEIGHT)));
                    if (_structureMaxHeight == 0)
                        _structureMaxHeight = 50;
                }
                catch { _structureMaxHeight = 50; }
                return _structureMaxHeight;
            }
        }
        public static Int64 StructureMaxWidth
        {
            get
            {
                try
                {
                    _structureMaxWidth = Convert.ToInt64(AppConfigSetting.ReadSetting(StringEnum.GetStringValue(Global.ConfigurationKey.STRUCTURE_MAX_WIDTH)));
                    if (_structureMaxWidth == 0)
                        _structureMaxWidth = 250;
                }
                catch { _structureMaxWidth = 250; }
                return _structureMaxWidth;
            }
        }

        #endregion

        public static string[] GetStructureSearchOptionParam()
        {
            try
            {
                if (Global.StructureSearchOptions == null)
                {
                    return Global.StructureSearchParams;
                }
                else
                {
                    if (Global.StructureSearchOptions.Count > 0)
                    {
                        Global.StructureSearchParams = null;
                        Global.StructureSearchParams = (string[])Global.StructureSearchOptions.ToArray(typeof(string));
                        return Global.StructureSearchParams;
                    }

                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    #endregion

    #region _  Generic Enumeration class  _
    public class GenericEnums
    {
        public static T GetEnum<T>(object o)
        {
            T one = (T)Enum.Parse(typeof(T), o.ToString());
            return one;
        }
    }
    #endregion _  Generic Enumeration class  _

    #region _  COEDataViewData class  _

    partial class COEDataViewData
    {
        public static ResultPageInfo resultPageInfo = null;

        #region "Static Methods"

        internal static void SetHitListID(int COEDataviewID)
        {
            SearchCriteria searchCriteria = new SearchCriteria();

            COESearch coeSearch = new COESearch(COEDataviewID);
            HitListInfo hitListInfo = new HitListInfo();
            try
            {
                Global.RestoreCSLAPrincipal();
                hitListInfo = coeSearch.GetHitList(searchCriteria, COEDataviewID);
                Global.HitlistID = hitListInfo.HitListID; //Hitlist id in global variable       
            }
            catch
            {
                throw new Exception(Properties.Resources.msgInvalidDV);
            }
            finally
            {
                Global.RestoreWindowsPrincipal();
            }
        }

        // 11.0.4 - commented as not in use
        // 11.0.4 - uncommented as full api search is not required
        internal static ResultPageInfo GetResultPageInfo(Excel::Worksheet nSheet, int COEDataviewID, COEDataView coeDataview, CBVExcel cbvExcel)
        {

            string[] searchCriteria = cbvExcel.GetInputCriteria(nSheet);
            return GetResultPageInfo(nSheet, COEDataviewID, coeDataview, cbvExcel, null, searchCriteria);
        }

        // 11.0.4 - commented as full api search is not required
        /*internal static ResultPageInfo GetResultPageInfo(Excel::Worksheet nSheet, CBVExcel cbvExcel, int COEDataviewID, SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, ResultsCriteria resultsCriteriaWithoutSimilarity, PagingInfo pagingInfo, bool returnPartialResults, string useRealTableNames)
        {
            try
            {
                COESearch coeSearch = new COESearch(COEDataviewID);               
               
                SearchResponse searchResponse = null;
                try
                {
                    Global.RestoreCSLAPrincipal();

                    // Calling the DoSearch function directly by passing the searchcriteria, resultscriteria and paginginfo
                    if (returnPartialResults)
                    {
                        searchResponse = coeSearch.DoPartialSearch(searchCriteria, resultsCriteria, pagingInfo, COEDataviewID, useRealTableNames);
                    }
                    else
                    {
                        searchResponse = coeSearch.DoSearch(searchCriteria, resultsCriteria, pagingInfo, COEDataviewID, useRealTableNames);
                    }
                }
                catch (Csla.DataPortalException ex)
                {
                    throw new Exception("FAILURE: The search failed.\n" + ex.BusinessException.Message);                   
                }                
                
                // Getting the resultPageInfo from the search response
                resultPageInfo = new ResultPageInfo(searchResponse.PagingInfo.HitListID, searchResponse.PagingInfo.RecordCount, searchResponse.PagingInfo.Start, searchResponse.PagingInfo.End);

                return resultPageInfo;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(Properties.Resources.excepRangeInvalid))
                    throw new Exception(Properties.Resources.msgSearchFailed + Properties.Resources.msgRangeInvalid);
                else if (ex.Message.Contains(Properties.Resources.excepMissDVRelation))
                    throw new Exception(Properties.Resources.msgSearchFailed + Properties.Resources.msgMissingDVRelation);
                else if (ex.Message.Contains(Properties.Resources.excepTableViewNotExists))
                    throw new Exception(Properties.Resources.msgSearchFailed + Properties.Resources.msgTableViewNotExists);
                else 
                    throw new Exception(ex.Message);

            }
            finally
            {
                Global.RestoreWindowsPrincipal();
            }
        }               

        // 11.0.4 - Getting the DomainFieldName        
        /*public static string GetDomainFieldName(Excel::Worksheet nSheet, COEDataView coeDataview, CBVExcel cbvExcel)
        {
            string domainFieldName = string.Empty;
            string pkField = GetPrimaryKeyRelOnAlias(coeDataview);

            string[] searchCriteriaFields = cbvExcel.GetInputCriteria(nSheet, false);

            //Whether unique key use for searching or primary key,
            if (Global.IsUniqueInSearch == true)

                //Case of multiple unique key so retrive and take the first unique key from CBV sheet
                if (!string.IsNullOrEmpty(Global.FirstUniqueKeyInSheet))
                    domainFieldName = GetUniqueKeyRelOnAlias(coeDataview, Global.FirstUniqueKeyInSheet.Trim());
                else
                    domainFieldName = GetUniqueKeyRelOnAlias(coeDataview);
            else
                domainFieldName = pkField;


            return domainFieldName;

        }

        // 11.0.4 - Getting the Paging Info
        public static void GetPagingInfo(ref PagingInfo pagingInfo, int pageSize, int start, int end, int hitListID)
        {
            pagingInfo.RecordCount = pageSize;
            pagingInfo.Start = start;
            pagingInfo.End = end;
            pagingInfo.HitListID = hitListID;
        }*/


        // 11.0.4 - commented as not in use
        // 11.0.4 - uncommented as full api search is not required
        internal static ResultPageInfo GetResultPageInfo(Excel::Worksheet nSheet, int COEDataviewID, COEDataView coeDataview, CBVExcel cbvExcel, string[] domainList)
        {
            try
            {
                SearchInput searchInput = new SearchInput();
                resultPageInfo = new ResultPageInfo();
                COESearch coeSearch = new COESearch(COEDataviewID);
                string[] searchOptions = StructureSearchOption.GetStructureSearchOptionParam();

                //11.0.3
                //Global.LookupByValueFields
                int optionLen = searchOptions.Length;
                Array.Resize(ref searchOptions, optionLen + 1);
                searchOptions.SetValue(Global.LookupByValueFields.Remove(Global.LookupByValueFields.Length - 1, 1), optionLen);



                string pkField = GetPrimaryKeyRelOnAlias(coeDataview);
                string[] searchCriteria = cbvExcel.GetInputCriteria(nSheet, false);
                searchInput.FieldCriteria = searchCriteria;
                searchInput.ReturnPartialResults = false;
                searchInput.SearchOptions = searchOptions;
                searchInput.Domain = domainList;

                //Whether unique key use for searching or primary key,
                if (Global.IsUniqueInSearch == true)

                    //Case of multiple unique key so retrive and take the first unique key from CBV sheet
                    if (!string.IsNullOrEmpty(Global.FirstUniqueKeyInSheet))
                        searchInput.DomainFieldName = GetUniqueKeyRelOnAlias(coeDataview, Global.FirstUniqueKeyInSheet.Trim());
                    else
                        searchInput.DomainFieldName = GetUniqueKeyRelOnAlias(coeDataview);
                else
                    searchInput.DomainFieldName = pkField;

                resultPageInfo.ResultSetID = 0;
                resultPageInfo.PageSize = Convert.ToInt32(AppConfigSetting.ReadSetting(StringEnum.GetStringValue(Global.ConfigurationKey.MAX_NO_HITS)));
                resultPageInfo.Start = 1;
                resultPageInfo.End = 100001;
                Global.RestoreCSLAPrincipal();

                DataListResult dataListResult = coeSearch.DoSearch(searchInput, pkField, resultPageInfo, "true");

                if (dataListResult.Status.Contains(Properties.Resources.excepRangeInvalid))
                    throw new Exception(Properties.Resources.msgSearchFailed + Properties.Resources.msgRangeInvalid);
                else if (dataListResult.Status.Contains(Properties.Resources.excepMissDVRelation))
                    throw new Exception(Properties.Resources.msgSearchFailed + Properties.Resources.msgMissingDVRelation);
                else if (dataListResult.Status.Contains(Properties.Resources.excepTableViewNotExists))
                    throw new Exception(Properties.Resources.msgSearchFailed + Properties.Resources.msgTableViewNotExists);

                else if (dataListResult.Status != Global.DATARESULT_SUCCESS_STATUS)
                    throw new Exception(dataListResult.Status);

                resultPageInfo = dataListResult.resultPageInfo;
                return resultPageInfo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                Global.RestoreWindowsPrincipal();
            }
        }

        // Advance Export - Overloaded to handle the existing functionality
        internal static ResultPageInfo GetResultPageInfo(Excel::Worksheet nSheet, int COEDataviewID, COEDataView coeDataview, CBVExcel cbvExcel, string[] domainList, string[] searchCriteria)
        {
            return GetResultPageInfo(nSheet, COEDataviewID, coeDataview, cbvExcel, domainList, searchCriteria, null);
        }

        // Advance Export - Extra parameter, ExportSheet object, passed to handle the export functionality
        internal static ResultPageInfo GetResultPageInfo(Excel::Worksheet nSheet, int COEDataviewID, COEDataView coeDataview, CBVExcel cbvExcel, string[] domainList, string[] searchCriteria, Excel::Worksheet nExportSheet)
        {
            try
            {
                //SearchCriteria searchCriteria = new SearchCriteria();
                SearchInput searchInput = new SearchInput();
                resultPageInfo = new ResultPageInfo();
                COESearch coeSearch = new COESearch(COEDataviewID);

                string exportDomain = string.Empty;
                string exportDomainFieldName = string.Empty;
                bool exportReturnSimilarityScores = false;
                string exportSearchOptions = string.Empty;
                int exportResultSetID = 0;
                int exportPageSize = 0;
                int exportStart = 0;
                int exportEnd = 0;
                string hitListQueryType = string.Empty;
                string[] searchOptions;

                string pkField = GetPrimaryKeyRelOnAlias(coeDataview);
                int excelPageSize = Convert.ToInt32(AppConfigSetting.ReadSetting(StringEnum.GetStringValue(Global.ConfigurationKey.MAX_NO_HITS)));
                // Advance Export - If ExportSheet object is not null then retrieving the values from it and setting the appropriate fields required for searching
                if (nExportSheet != null)
                {
                    try
                    {
                        exportDomain = Convert.ToString(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.DOMAIN) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.DOMAIN) + "2").Value2);
                        exportDomainFieldName = Convert.ToString(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.DOMAINFIELDNAME) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.DOMAINFIELDNAME) + "2").Value2);

                        if (string.IsNullOrEmpty(Convert.ToString(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.RETURNSIMILARITYSCORES) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.RETURNSIMILARITYSCORES) + "2").Value2)))
                        {
                            exportReturnSimilarityScores = Convert.ToBoolean(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.RETURNSIMILARITYSCORES) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.RETURNSIMILARITYSCORES) + "2").Value2);
                        }

                        exportSearchOptions = Convert.ToString(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.SEARCHOPTIONS) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.SEARCHOPTIONS) + "2").Value2);


                        if (!string.IsNullOrEmpty(Convert.ToString(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.HITLISTQUERYTYPE) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.HITLISTQUERYTYPE) + "2").Value2)))
                        {
                            hitListQueryType = Convert.ToString(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.HITLISTQUERYTYPE) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.HITLISTQUERYTYPE) + "2").Value2);
                        }

                        searchInput.ReturnPartialResults = exportReturnSimilarityScores;
                        searchInput.DomainFieldName = exportDomainFieldName;
                        searchInput.SearchOptions = exportSearchOptions.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        if (searchInput.SearchOptions.Length <= 0)  //CSBR-162482 (If exported sheet doesn't have the search options for structure searching then the existing options are set)
                        {
                            searchOptions = StructureSearchOption.GetStructureSearchOptionParam();
                            int optionLen = searchOptions.Length;
                            Array.Resize(ref searchOptions, optionLen + 1);
                            searchOptions.SetValue(Global.LookupByValueFields.Remove(Global.LookupByValueFields.Length - 1, 1), optionLen);
                            searchInput.SearchOptions = searchOptions;
                        }
                        searchInput.Domain = exportDomain.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        searchInput.FieldCriteria = searchCriteria;
                                              
                        if (hitListQueryType.Equals(HitListQueryType.MERGED.ToString(), StringComparison.OrdinalIgnoreCase) || hitListQueryType.Equals(HitListQueryType.SEARCHOVER.ToString(), StringComparison.OrdinalIgnoreCase)) //Fixed CSBR-153844 and CSBR-155235
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.RESULTSETID) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.RESULTSETID) + "2").Value2)))
                            {
                                exportResultSetID = Convert.ToInt32(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.RESULTSETID) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.RESULTSETID) + "2").Value2);
                            }

                            if (!string.IsNullOrEmpty(Convert.ToString(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.PAGESIZE) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.PAGESIZE) + "2").Value2)))
                            {
                                exportPageSize = Convert.ToInt32(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.PAGESIZE) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.PAGESIZE) + "2").Value2);
                            }

                            if (!string.IsNullOrEmpty(Convert.ToString(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.START) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.START) + "2").Value2)))
                            {
                                exportStart = Convert.ToInt32(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.START) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.START) + "2").Value2);
                            }

                            if (!string.IsNullOrEmpty(Convert.ToString(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.END) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.END) + "2").Value2)))
                            {
                                exportEnd = Convert.ToInt32(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.END) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.END) + "2").Value2);
                            }
                            resultPageInfo.ResultSetID = exportResultSetID;
                           
                           //resultPageInfo.PageSize = exportPageSize;
                            resultPageInfo.PageSize = exportPageSize > excelPageSize ? excelPageSize:exportPageSize;
                            resultPageInfo.Start = exportStart;
                            resultPageInfo.End = exportEnd;
                            return resultPageInfo;
                        }
                        else
                        {
                            resultPageInfo.ResultSetID = 0;
                            resultPageInfo.PageSize = excelPageSize;
                            resultPageInfo.Start = 1;
                            resultPageInfo.End = 100001;
                        }
                    }
                    catch
                    {
                        throw new Exception(Properties.Resources.msgExportSheetError);
                    }
                }
                else
                {
                    searchOptions = StructureSearchOption.GetStructureSearchOptionParam();
                    //11.0.3
                    //Global.LookupByValueFields
                    int optionLen = searchOptions.Length;
                    Array.Resize(ref searchOptions, optionLen + 1);
                    searchOptions.SetValue(Global.LookupByValueFields.Remove(Global.LookupByValueFields.Length - 1, 1), optionLen);

                    string txtdata = string.Empty;
                    searchInput.Domain = txtdata.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    resultPageInfo.ResultSetID = 0;
                    resultPageInfo.PageSize = excelPageSize;
                    resultPageInfo.Start = 1;
                    resultPageInfo.End = 100001;

                    searchInput.ReturnPartialResults = false;
                    searchInput.SearchOptions = searchOptions;
                    searchInput.FieldCriteria = searchCriteria;
                }

                Global.RestoreCSLAPrincipal();

                DataListResult dataListResult = coeSearch.DoSearch(searchInput, pkField, resultPageInfo, "true");

                if (dataListResult.Status.Contains(Properties.Resources.excepRangeInvalid))
                {
                    throw new Exception(Properties.Resources.msgSearchFailed + Properties.Resources.msgRangeInvalid);
                }
                else if (dataListResult.Status.Contains(Properties.Resources.excepMissDVRelation))
                {
                    throw new Exception(Properties.Resources.msgSearchFailed + Properties.Resources.msgMissingDVRelation);
                }
                else if (dataListResult.Status.Contains(Properties.Resources.excepTableViewNotExists))
                {
                    throw new Exception(Properties.Resources.msgSearchFailed + Properties.Resources.msgTableViewNotExists);
                }
                else if (dataListResult.Status != Global.DATARESULT_SUCCESS_STATUS)
                {
                    throw new Exception(dataListResult.Status);
                }

                resultPageInfo = dataListResult.resultPageInfo;
                return resultPageInfo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                Global.RestoreWindowsPrincipal();
            }
        }




        // 11.0.4 - modified below as full search API is used
        // 11.0.4 - uncommented as full search API is not required
        internal static string ExportData(Excel::Worksheet nSheet, ResultPageInfo resultPageInfo, int COEDataviewID, COEDataView coeDataview, CBVExcel cbvExcel)
        {
            try
            {
                Global.RestoreCSLAPrincipal();

                string[] ouputCriteria = cbvExcel.GetOutputCriteria(nSheet);
                ResultsCriteria rc = new ResultsCriteria();
                rc = CambridgeSoft.COE.Framework.COESearchService.ResultFieldsToResultsCriteria.GetResultCriteria(ouputCriteria, coeDataview);

                PagingInfo pi = new PagingInfo();
                pi.HitListID = resultPageInfo.ResultSetID;
                pi.Start = 1;
                pi.End = resultPageInfo.End;
                pi.RecordCount = resultPageInfo.PageSize;

                COEExport expData = new COEExport();
                string exportType = "SDFNested";
                string exportData = expData.GetData(rc, pi, COEDataviewID, exportType);
                return exportData;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                Global.RestoreWindowsPrincipal();
            }
        }

        // 11.0.4 - Commented as full search API is not required
        /*internal static string ExportData(ResultsCriteria resultsCriteria, PagingInfo pagingInfo, int COEDataviewID)
        {
            try
            {
                Global.RestoreCSLAPrincipal();


                COEExport coeExport = new COEExport();
                string exportType = "SDFNested";
                string exportData = coeExport.GetData(resultsCriteria, pagingInfo, COEDataviewID, exportType);
                return exportData;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                Global.RestoreWindowsPrincipal();
            }
        }*/

        //Main Search       
        // 11.0.4 - Modified below to previous one as full search API is not required
        /*public static DataSet Searching(Excel::Worksheet nSheet, CBVExcel cbvExcel, int COEDataviewBOID, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, string[] searchCriteriaFields, string useRealTableNames, Global.CBVSearch CBVSearchType)
        {
            DataSet resultsDataSet = new DataSet();
            resultsDataSet.CaseSensitive = false;

            COESearch coeSearch = new COESearch(COEDataviewBOID);

            //update the CBV new column Index
            Global.CBVNewColumnIndex = cbvExcel.GetCBVNewCategoryIndex(nSheet) + Global.StartUpRowPosition(nSheet);

            Global.InsertColumn = false; // Set the insert column to false

            //Create output criteria
            if (CBVSearchType.Equals(Global.CBVSearch.ReplaceAllResults))
            {
                //On every replace the new search criteria stored in global variable and it's us in update result.
                Global.SearchCriteria = new List<string>();
                if (searchCriteriaFields != null)
                    Global.SearchCriteria.AddRange(searchCriteriaFields);
                                
                Global.SearchUpdateCriteria.Clear(); // Clear the existing update criteria list on Replace result command

                Global.SearchUpdateCriteria.Add(searchCriteriaFields);  // Add the current search criteria into update criteria list
                                
            }
            else if (CBVSearchType.Equals(Global.CBVSearch.AppendNewResults))
            {
                if (searchCriteriaFields != null)
                    Global.SearchUpdateCriteria.Add(searchCriteriaFields);  // Add the update criteria in list      

                Global.SearchCriteria = new List<string>();
                if (searchCriteriaFields != null)
                    Global.SearchCriteria.AddRange(searchCriteriaFields);
                
            }
            else if (CBVSearchType.Equals(Global.CBVSearch.UpdateCurrentResults))
            {
                Global.SearchCriteria = new List<string>();
                if (searchCriteriaFields != null)
                    Global.SearchCriteria.AddRange(searchCriteriaFields);               
            }

            try
            {
                Global.RestoreCSLAPrincipal();

                // Calling the GetData function directly (based upon resultscriteria and paginginfo)
                resultsDataSet = coeSearch.GetData(resultsCriteria, pagingInfo, COEDataviewBOID, useRealTableNames);
            }
            catch (Exception ex)
            {
                
                if (ex.Message.Contains(Properties.Resources.excepInsufficientPriv))
                        throw new Exception(Properties.Resources.msgInsufficientPriv);                
                else
                    throw;
            }
            finally
            {
                Global.RestoreWindowsPrincipal();
            }
            
            if (resultsDataSet.Tables.Count > 0)
            {
                DataSetUtilities.AddMissingTables(ref resultsDataSet, nSheet, cbvExcel);
                DataSetUtilities.AddMissingColumns(ref resultsDataSet, nSheet, cbvExcel);
                DataSetUtilities.RemoveEmptySpacesFromTableColumn(ref resultsDataSet);
            }
            return resultsDataSet;
        }*/

        public static DataSet Searching(Excel::Worksheet nSheet, Global.CBVSearch CBVSearchType, ResultPageInfo resultPageInfo, int COEDataviewBOID, string[] searchCriteria, string[] ouputCriteria, CBVExcel cbvExcel)
        {
            DataResult dataResult = new DataResult();
            DataSet resultsDataSet = new DataSet();
            resultsDataSet.CaseSensitive = false;

            COESearch coeSearch = new COESearch(COEDataviewBOID);

            //update the CBV new column Index
            Global.CBVNewColumnIndex = cbvExcel.GetCBVNewCategoryIndex(nSheet) + Global.StartUpRowPosition(nSheet);

            Global.InsertColumn = false; // Set the insert column to false

            //Create output criteria
            if (CBVSearchType.Equals(Global.CBVSearch.ReplaceAllResults))
            {
                //On every replace the new search criteria stored in global variable and it's us in update result.
                Global.SearchCriteria = new List<string>();
                if (searchCriteria != null)
                    Global.SearchCriteria.AddRange(searchCriteria);

                try
                {
                    Global.RestoreCSLAPrincipal();

                    dataResult = coeSearch.GetDataPage(resultPageInfo, ouputCriteria, "true");
                }
                catch (Exception ex)
                {
                    if (string.IsNullOrEmpty(dataResult.Status) && string.IsNullOrEmpty(ex.Message))
                    {
                        if (dataResult.Status.Contains(Properties.Resources.excepInsufficientPriv))
                            throw new Exception(Properties.Resources.msgInsufficientPriv);
                    }
                    else
                        throw;

                }
                finally
                {
                    Global.RestoreWindowsPrincipal();
                }

                if (!dataResult.Status.Equals(Global.DATARESULT_SUCCESS_STATUS, StringComparison.OrdinalIgnoreCase))
                    throw new Exception(dataResult.Status);

                Global.SearchUpdateCriteria.Clear(); // Clear the existing update criteria list on Replace result command

                Global.SearchUpdateCriteria.Add(searchCriteria);  // Add the current search criteria into update criteria list

                resultsDataSet = DataSetUtilities.DataResultToDataSet(dataResult);
            }
            else if (CBVSearchType.Equals(Global.CBVSearch.AppendNewResults))
            {
                if (searchCriteria != null)
                    Global.SearchUpdateCriteria.Add(searchCriteria);  // Add the update criteria in list      

                Global.SearchCriteria = new List<string>();
                if (searchCriteria != null)
                    Global.SearchCriteria.AddRange(searchCriteria);
                try
                {
                    Global.RestoreCSLAPrincipal();
                    dataResult = coeSearch.GetDataPage(resultPageInfo, ouputCriteria, "true");
                }
                catch (Exception ex)
                {
                    if (string.IsNullOrEmpty(dataResult.Status) && string.IsNullOrEmpty(ex.Message))
                    {
                        if (dataResult.Status.Contains(Properties.Resources.excepInsufficientPriv))
                            throw new Exception(Properties.Resources.msgInsufficientPriv);
                    }
                    else
                        throw;
                }
                finally
                {
                    Global.RestoreWindowsPrincipal();
                }

                if (!dataResult.Status.Equals(Global.DATARESULT_SUCCESS_STATUS, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception(dataResult.Status);
                }
                resultsDataSet = DataSetUtilities.DataResultToDataSet(dataResult);

            }
            else if (CBVSearchType.Equals(Global.CBVSearch.UpdateCurrentResults))
            {
                Global.SearchCriteria = new List<string>();
                if (searchCriteria != null)
                    Global.SearchCriteria.AddRange(searchCriteria);

                try
                {
                    Global.RestoreCSLAPrincipal();
                    dataResult = coeSearch.GetDataPage(resultPageInfo, ouputCriteria, "true");
                }
                catch (Exception ex)
                {
                    if (string.IsNullOrEmpty(dataResult.Status) && string.IsNullOrEmpty(ex.Message))
                    {
                        if (dataResult.Status.Contains(Properties.Resources.excepInsufficientPriv))
                            throw new Exception(Properties.Resources.msgInsufficientPriv);
                    }
                    else
                        throw;
                }
                finally
                {
                    Global.RestoreWindowsPrincipal();
                }

                if (dataResult.Status.Equals(Global.DATARESULT_SUCCESS_STATUS, StringComparison.OrdinalIgnoreCase))
                {
                    using (System.IO.StringReader stringReader = new System.IO.StringReader(dataResult.ResultSet))
                    {
                        resultsDataSet.ReadXml(stringReader);
                    }
                }
            }

            if (resultsDataSet.Tables.Count > 0)
            {
                DataSetUtilities.AddMissingTables(ref resultsDataSet, nSheet, cbvExcel);
                DataSetUtilities.AddMissingColumns(ref resultsDataSet, nSheet, cbvExcel);
                DataSetUtilities.RemoveEmptySpacesFromTableColumn(ref resultsDataSet);
            }
            return resultsDataSet;
        }

        // 11.0.4 - Modified below to previous one as full search API is not required
        /*public static StringBuilder SearchingMimeTypeData(Excel.Worksheet nSheet, CBVExcel cbvExcel, int COEDataviewBOID, ResultsCriteria resultsCriteria, PagingInfo pagingInfo, string[] searchCriteriaFields, string useRealTableNames, Global.CBVSearch CBVSearchType, string tabAlias, string colAlias)
        {            
            StringBuilder resultsDataSet = new StringBuilder();
            DataSet dataSet = new DataSet();

            COESearch coeSearch = new COESearch(COEDataviewBOID);

            //update the CBV new column Index
            Global.CBVNewColumnIndex = cbvExcel.GetCBVNewCategoryIndex(nSheet) + Global.StartUpRowPosition(nSheet);

            Global.InsertColumn = false; // Set the insert column to false


            if (CBVSearchType.Equals(Global.CBVSearch.ReplaceMimeTypeResult))
            {
                //On every replace the new search criteria stored in global variable and it's us in update result.
                Global.SearchCriteria = new List<string>();
                if (searchCriteriaFields != null)
                    Global.SearchCriteria.AddRange(searchCriteriaFields);

                try
                {
                    Global.RestoreCSLAPrincipal();
                    // Calling the GetData function directly (based upon resultscriteria and paginginfo)
                    dataSet = coeSearch.GetData(resultsCriteria, pagingInfo, COEDataviewBOID, useRealTableNames);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains(Properties.Resources.excepInsufficientPriv))
                        throw new Exception(Properties.Resources.msgInsufficientPriv);
                    else
                        throw;

                }
                finally
                {
                    Global.RestoreWindowsPrincipal();
                }
                              
              
                resultsDataSet = Global.DataSetToStringBuilder(dataSet, tabAlias, colAlias);

            }

            return resultsDataSet;
        }*/

        public static StringBuilder SearchingMimeTypeData(Excel.Range nRange, Excel.Worksheet nSheet, Global.CBVSearch CBVSearchType, ResultPageInfo resultPageInfo, int COEDataviewBOID, string[] searchCriteria, string[] ouputCriteria, CBVExcel cbvExcel)
        {
            DataResult dataResult = new DataResult();
            StringBuilder resultsDataSet = new StringBuilder();


            COESearch coeSearch = new COESearch(COEDataviewBOID);

            //update the CBV new column Index
            Global.CBVNewColumnIndex = cbvExcel.GetCBVNewCategoryIndex(nSheet) + Global.StartUpRowPosition(nSheet);

            Global.InsertColumn = false; // Set the insert column to false


            if (CBVSearchType.Equals(Global.CBVSearch.ReplaceMimeTypeResult))
            {
                //On every replace the new search criteria stored in global variable and it's us in update result.
                Global.SearchCriteria = new List<string>();
                if (searchCriteria != null)
                    Global.SearchCriteria.AddRange(searchCriteria);

                try
                {
                    Global.RestoreCSLAPrincipal();
                    dataResult = coeSearch.GetDataPage(resultPageInfo, ouputCriteria, "true");
                }
                catch (Exception ex)
                {
                    if (string.IsNullOrEmpty(dataResult.Status) && string.IsNullOrEmpty(ex.Message))
                    {
                        if (dataResult.Status.Contains(Properties.Resources.excepInsufficientPriv))
                            throw new Exception(Properties.Resources.msgInsufficientPriv);
                    }
                    else
                        throw;

                }
                finally
                {
                    Global.RestoreWindowsPrincipal();
                }

                if (!dataResult.Status.Equals(Global.DATARESULT_SUCCESS_STATUS, StringComparison.OrdinalIgnoreCase))
                    throw new Exception(dataResult.Status);


                string colAlias = GlobalCBVExcel.GetCell((int)Global.CBVHeaderRow.HeaderColumnRow, nRange.Column, nSheet);
                string tabAlias = GlobalCBVExcel.GetCell((int)Global.CBVHeaderRow.HeaderTableRow, nRange.Column, nSheet);

                resultsDataSet = DataSetUtilities.DataResultToStringBuilder(dataResult, colAlias, tabAlias);

            }

            return resultsDataSet;
        }


        //Check whether the search criteria is modifed or not as well as set into the global variable
        public static bool IsSearchCriteriaModified(string[] newSearchCriteria, List<string> lastSearchCriteria)
        {
            try
            {
                if (newSearchCriteria == null || lastSearchCriteria == null)
                    return false;

                else if (newSearchCriteria.Length > lastSearchCriteria.Count)
                {
                    Global.SearchCriteria.Clear(); //Clear the previous criteria
                    Global.SearchCriteria.AddRange(newSearchCriteria); //Added the new criteria
                    return true;
                }
                else
                {
                    foreach (string criteria in newSearchCriteria)
                    {
                        if (!lastSearchCriteria.Exists(delegate(string s)
                       { return criteria == s; }))
                        {
                            Global.SearchCriteria.Clear(); //Clear the previous criteria
                            Global.SearchCriteria.AddRange(newSearchCriteria); //Added the new criteria
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static string GetPrimaryKey(COEDataView dataView)
        {
            try
            {
                TableBO baseTable = Global.Tables.GetTable(dataView.Basetable);
                string PKAlias = string.Empty;
                int pkAliasID = baseTable.PrimaryKey;
                string pkAliasname = string.Empty;
                for (int col = 0; col < baseTable.Fields.Count; col++)
                {
                    if (baseTable.Fields[col].ID == baseTable.PrimaryKey)
                    {
                        PKAlias = baseTable.Fields[col].Alias.Trim().Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT);
                        return PKAlias;
                    }
                }
                return string.Empty;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static string GetPrimaryKeyRel(COEDataView dataView)
        {
            try
            {
                TableBO baseTable = Global.Tables.GetTable(dataView.Basetable);
                baseTable.PrimaryKeyName.IndexOf('(');
                string PKRel = baseTable.Name + "." + baseTable.PrimaryKeyName.Substring(0, baseTable.PrimaryKeyName.IndexOf('('));
                return PKRel.Trim();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetPrimaryKeyRelOnAlias(COEDataView dataView)
        {
            try
            {
                TableBO baseTable = Global.Tables.GetTable(dataView.Basetable);
                string PKAlias = string.Empty;
                int pkAliasID = baseTable.PrimaryKey;
                string pkAliasname = string.Empty;
                for (int col = 0; col < baseTable.Fields.Count; col++)
                {
                    if (baseTable.Fields[col].ID == baseTable.PrimaryKey)
                    {
                        PKAlias = baseTable.Fields[col].Alias.Trim();
                        break;
                    }
                }

                string PKRel = baseTable.Alias.Trim().Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT) + "." + PKAlias.Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT);
                return PKRel.Trim();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetPrimaryKeyRelOnTabNameColAlias(COEDataView dataView)
        {
            try
            {
                TableBO baseTable = Global.Tables.GetTable(dataView.Basetable);
                string PKAlias = string.Empty;
                int pkAliasID = baseTable.PrimaryKey;
                string pkAliasname = string.Empty;
                for (int col = 0; col < baseTable.Fields.Count; col++)
                {
                    if (baseTable.Fields[col].ID == baseTable.PrimaryKey)
                    {
                        PKAlias = baseTable.Fields[col].Alias.Trim();
                        break;
                    }
                }

                string PKRel = baseTable.Name.Trim() + "." + PKAlias.Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT);
                return PKRel.Trim();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetUniqueKeyRel(COEDataView dataView)
        {
            try
            {
                TableBO baseTable = Global.Tables.GetTable(dataView.Basetable);
                for (int col = 0; col < baseTable.Fields.Count; col++)
                {
                    if (baseTable.Fields[col].IsUniqueKey)
                    {
                        string UKRel = baseTable.Name + "." + baseTable.Fields[col].Name;
                        return UKRel.Trim();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetUniqueKeyRelOnTabNameColAlias(COEDataView dataView)
        {
            try
            {
                TableBO baseTable = Global.Tables.GetTable(dataView.Basetable);
                for (int col = 0; col < baseTable.Fields.Count; col++)
                {
                    if (baseTable.Fields[col].IsUniqueKey)
                    {
                        string UKRel = baseTable.Name + "." + baseTable.Fields[col].Alias.Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT);
                        return UKRel.Trim();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetUniqueKeyRelOnTabNameColAlias(COEDataView dataView, string uniqueKeyAliasInSheet)
        {
            try
            {
                TableBO baseTable = Global.Tables.GetTable(dataView.Basetable);
                for (int col = 0; col < baseTable.Fields.Count; col++)
                {
                    if ((baseTable.Fields[col].IsUniqueKey) && (baseTable.Fields[col].Alias.Trim().Equals(uniqueKeyAliasInSheet, StringComparison.OrdinalIgnoreCase)))
                    {
                        string UKRel = baseTable.Name + "." + baseTable.Fields[col].Alias.Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT);
                        return UKRel.Trim();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetUniqueKeyRelOnAlias(COEDataView dataView)
        {
            try
            {
                TableBO baseTable = Global.Tables.GetTable(dataView.Basetable);
                for (int col = 0; col < baseTable.Fields.Count; col++)
                {
                    if (baseTable.Fields[col].IsUniqueKey)
                    {
                        string UKRel = baseTable.Alias.Trim().Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT) + "." + baseTable.Fields[col].Alias.Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT);
                        return UKRel.Trim();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetUniqueKeyRelOnAlias(COEDataView dataView, string uniqueKeyAliasInSheet)
        {
            try
            {
                TableBO baseTable = Global.Tables.GetTable(dataView.Basetable);
                for (int col = 0; col < baseTable.Fields.Count; col++)
                {
                    if ((baseTable.Fields[col].IsUniqueKey) && (baseTable.Fields[col].Alias.Trim().Equals(uniqueKeyAliasInSheet, StringComparison.OrdinalIgnoreCase)))
                    {
                        string UKRel = baseTable.Alias.Trim().Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT) + "." + baseTable.Fields[col].Alias.Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT);
                        return UKRel.Trim();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool ISValidFields(string category, string tablename, string colname)
        {
            bool ISValid = false;
            foreach (TableBO tableBO in Global.Tables)
            {
                if (tableBO.DataBase.Equals(category, StringComparison.OrdinalIgnoreCase) && tableBO.Alias.Equals(tablename, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (FieldBO fieldBO in tableBO.Fields)
                    {
                        if (fieldBO.Alias.Equals(colname, StringComparison.OrdinalIgnoreCase))
                        {
                            ISValid = true;
                            break;
                        }
                        else
                        {
                            ISValid = false;
                        }
                    }
                }
            }
            return ISValid;
        }


        public static string GetIndexType(string colAlias, string tabAlias)
        {
            try
            {
                Global.DataView = Global.CBVSHEET_COEDATAVIEWBO.COEDataView;
                Global.Tables = TableListBO.NewTableListBO(Global.DataView.Tables);
                //TableBO baseTable = Global.Tables.GetTable(tableID);
                for (int i = 0; i < Global.Tables.Count; i++)
                {
                    for (int j = 0; j < Global.Tables[i].Fields.Count; j++)
                    {
                        if (Global.Tables[i].Fields[j].Alias.Trim().Equals(colAlias, StringComparison.OrdinalIgnoreCase))
                            //11.0.3

                            if (Global.Tables[i].Fields[j].LookupFieldId > 0)
                                return (COEDataViewData.GetLookupIndex(Global.Tables[i].Fields[j].LookupDisplayFieldId));
                            else //
                                return Global.Tables[i].Fields[j].IndexType.ToString();
                    }
                }
                return "NONE";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //11.0.3

        public static string GetMimeType(string colAlias, string tabAlias)
        {
            try
            {
                Global.DataView = Global.CBVSHEET_COEDATAVIEWBO.COEDataView;
                Global.Tables = TableListBO.NewTableListBO(Global.DataView.Tables);
                //TableBO baseTable = Global.Tables.GetTable(tableID);
                for (int i = 0; i < Global.Tables.Count; i++)
                {
                    for (int j = 0; j < Global.Tables[i].Fields.Count; j++)
                    {
                        if (Global.Tables[i].Fields[j].Alias.Trim().Equals(colAlias, StringComparison.OrdinalIgnoreCase))
                        {
                            //11.0.3
                            if (Global.Tables[i].Fields[j].LookupFieldId > 0)
                                return (COEDataViewData.GetLookupMime(Global.Tables[i].Fields[j].LookupDisplayFieldId));
                            else //
                                return Global.Tables[i].Fields[j].MimeType.ToString();
                        }
                    }
                }
                return "NONE";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //11.0.3
        public static bool ISLookupExists(string catAlias, string tabAlias, string colAlias)
        {
            try
            {
                foreach (TableBO tableBO in Global.Tables)
                {
                    if (tableBO.DataBase.Equals(catAlias, StringComparison.OrdinalIgnoreCase) && tableBO.Alias.Equals(tabAlias, StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (FieldBO fieldBO in tableBO.Fields)
                        {
                            if (fieldBO.Alias.Equals(colAlias, StringComparison.OrdinalIgnoreCase))
                            {
                                if (fieldBO.LookupFieldId >= 0)
                                    return true;
                                else
                                    return false;
                            }
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }

        }

        public static string GetLookupColumnTabDB(string catAlias, string tabAlias, string colAlias)
        {
            try
            {
                //foreach (COEDataView.DataViewTable tableBO in Global.CBVSHEET_COEDATAVIEWBO.COEDataView.Tables)
                foreach (TableBO tableBO in Global.DVTables)
                {
                    if (tableBO.DataBase.Equals(catAlias, StringComparison.OrdinalIgnoreCase) && tableBO.Alias.Equals(tabAlias, StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (FieldBO fieldBO in tableBO.Fields)
                        {
                            if (fieldBO.Alias.Equals(colAlias, StringComparison.OrdinalIgnoreCase))
                            {
                                if (fieldBO.LookupFieldId >= 0)
                                {
                                    foreach (TableBO tableBOIn in Global.DVTables)
                                    {
                                        foreach (FieldBO fieldBOIn in tableBOIn.Fields)
                                        {
                                            if (fieldBOIn.ID == fieldBO.LookupDisplayFieldId)
                                            {
                                                return (tableBOIn.DataBase + "." + tableBOIn.Alias + "." + fieldBOIn.Alias);
                                            }
                                        }
                                    }
                                }
                                else
                                    //throw new Exception("The lookup field doesn't exists"); ;
                                    return (catAlias + "." + tabAlias + "." + colAlias);
                            }
                        }
                    }
                }
                throw new Exception("The lookup table doesn't exist in dataview");
            }
            catch
            {
                throw new Exception("The lookup table doesn't exist in dataview");
            }
        }

        public static string GetLookupType(int lookupDispfieldID)
        {
            // CSBR #151953/151940 - Global.DVTables used for this fix instead of Global.Tables
            foreach (TableBO tableBO in Global.DVTables)
            {
                foreach (FieldBO fieldBO in tableBO.Fields)
                {
                    if (fieldBO.ID.Equals(lookupDispfieldID))
                    {
                        return (fieldBO.DataType.ToString().Trim());
                    }
                }
            }
            return "TEXT"; //If doesn't found any dataType then Text will return
        }

        //11.0.3
        public static string GetLookupIndex(int lookupDispfieldID)
        {
            // CSBR #151953/151940 - Global.DVTables used for this fix instead of Global.Tables
            foreach (TableBO tableBO in Global.DVTables)
            {
                foreach (FieldBO fieldBO in tableBO.Fields)
                {
                    if (fieldBO.ID.Equals(lookupDispfieldID))
                    {
                        return (fieldBO.IndexType.ToString().Trim());
                    }
                }
            }
            return "NONE"; //If doesn't found any dataType then Text will return
        }

        //11.0.3
        public static string GetLookupMime(int lookupDispfieldID)
        {
            // CSBR #151953/151940 - Global.DVTables used for this fix instead of Global.Tables
            foreach (TableBO tableBO in Global.DVTables)
            {
                foreach (FieldBO fieldBO in tableBO.Fields)
                {
                    if (fieldBO.ID.Equals(lookupDispfieldID))
                    {
                        return (fieldBO.MimeType.ToString().Trim());
                    }
                }
            }
            return "NONE"; //If doesn't found any dataType then Text will return
        }

        #endregion "Static Methods"


    }

    #endregion _  COEDataViewData class  _

    #region _ GlobalCBVExcel Class _

    public class GlobalCBVExcel
    {
        public GlobalCBVExcel()
        {
        }

        #region "Variables"
        private static string addInCellDropDownRange;
        #endregion "Varaibles"

        #region "Public Properties"
        public static string AddInCellDropDownRange
        {
            get
            {
                return addInCellDropDownRange;
            }
            set
            {
                try
                {
                    //Excel have limitation : Doesn't take more than 255 character as data for dropdown cell. So, split the data and put it into hidden sheet and use as range from hidden sheet.                   
                    //if (value.Length > 254)
                    if ((value != null) && (value.Length > 0))
                    {
                        CBVExcel CBVexcel = new CBVExcel();

                        Excel::Worksheet hiddenSheet = GlobalCBVExcel.Get_CSHidden();
                        string[] dataList = value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        //Retrieve the column number and name
                        int dropDownColID = (int)Global.CBVHiddenSheetHeader.AddInCellDropdownList;
                        string dropDownColName = Global.NumToString(dropDownColID);

                        //Clear the dropdwon list column  
                        //Excel.Range delRange = hiddenSheet.get_Range("=$" + dropDownColName + "$2:$" + dropDownColName + "$65535", Type.Missing); //The range have taken from 2 row to end of row.
                        double rwcnt = Global._ExcelApp.ActiveWorkbook.Worksheets.Application.Rows.Count;
                        Excel.Range delRange = hiddenSheet.get_Range("=$" + dropDownColName + "$2:$" + dropDownColName + "$" + rwcnt.ToString(), Type.Missing); //The range have taken from 2 row to end of row.

                        delRange.ClearContents();

                        //Fill the splitted data into dropdownlist column
                        int cnt = 0;
                        for (int row = 2; row <= dataList.Length + 1; row++)
                        {
                            //The 17th column of hidden Sheet is used for dropdown list

                            Excel.Range cell = hiddenSheet.Cells[row, dropDownColID] as Excel.Range;
                            //Coverity fix - CID 18734
                            if (cell == null)
                                throw new System.NullReferenceException("cell value cannot be null");
                            cell.Value2 = dataList[cnt++].Replace(Global.ESCAPE_SEPERATOR_FOR_COMMA, Global.COMMA_SEPARATOR);
                        }

                        //Create range of dropdown list column
                        //string newDropdownRange = "=INDIRECT(\"'" + hiddenSheet.Name + "'!$" + dropDownColName + "$2:$" + dropDownColName + "$" + (dataList.Length + 1) + "\")";
                        // Instead of using the Indirect function to get the range from hidden sheet (to be used in Validation method),
                        // the range in hidden sheet is given a unique name and that name is used in Validation method
                        Excel.Range ddRange = hiddenSheet.get_Range("=$" + dropDownColName + "$2:$" + dropDownColName + "$" + (dataList.Length + 1).ToString(), Type.Missing);

                        if (ddRange != null)
                        {
                            string RangeName = "ddListRange" + Global.RangeCounter.ToString();
                            ddRange.Name = RangeName;
                            string newDropdownRange = "=" + RangeName;
                            Global.RangeCounter++;
                            addInCellDropDownRange = newDropdownRange;
                        }
                        else
                        {
                            addInCellDropDownRange = string.Empty;
                        }                        
                    }
                    else
                    {
                        addInCellDropDownRange = value;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion "Public Properties"


        #region "Public Methods"

        public static Excel::Worksheet Get_CSHidden()
        {
            Excel::Application _Excelapp = Global._ExcelApp as Excel::Application;
            try
            {
                Excel::Worksheet newWorksheet = GlobalCBVExcel.FindSheet(Global.COEDATAVIEW_HIDDENSHEET);

                if (newWorksheet == null)
                {
                    //Global._ExcelApp.ScreenUpdating = false;
                    Global._ExcelApp.EnableEvents = false;
                    newWorksheet = (Excel::Worksheet)_Excelapp.Worksheets.Add(
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    newWorksheet.Name = Global.COEDATAVIEW_HIDDENSHEET;
                    Global._ExcelApp.EnableEvents = true;

                    //the first rows treat as headers rows in hidden sheet
                    SetCell(1, Global.CBVHiddenSheetHeader.ID, newWorksheet, "ID");
                    SetCell(1, Global.CBVHiddenSheetHeader.Database, newWorksheet, "Database");
                    SetCell(1, Global.CBVHiddenSheetHeader.Dataview, newWorksheet, "DataView");
                    SetCell(1, Global.CBVHiddenSheetHeader.CBVNewColIndex, newWorksheet, "CBVNewColIndex");
                    SetCell(1, Global.CBVHiddenSheetHeader.Dataviewname, newWorksheet, "DataViewName");
                    SetCell(1, Global.CBVHiddenSheetHeader.Sheetname, newWorksheet, "SheetName");
                    SetCell(1, Global.CBVHiddenSheetHeader.MaxResultCount, newWorksheet, "MaxNoofResultFound");
                    SetCell(1, Global.CBVHiddenSheetHeader.CBVActiveSheetIndex, newWorksheet, "CBVActiveSheetIndex");
                    SetCell(1, Global.CBVHiddenSheetHeader.Display, newWorksheet, "Display");
                    SetCell(1, Global.CBVHiddenSheetHeader.SerializeCBVResult, newWorksheet, "SerializeCBVResultSchema");
                    SetCell(1, Global.CBVHiddenSheetHeader.SearchUpdateCriteria, newWorksheet, "SerializeSearchUpdateCriteria");

                    SetCell(1, Global.CBVHiddenSheetHeader.LoginUser, newWorksheet, "LoginUserName");
                    SetCell(1, Global.CBVHiddenSheetHeader.SheetCreatedUser, newWorksheet, "SheetCreatedUserName");
                    SetCell(1, Global.CBVHiddenSheetHeader.ModifiedUser, newWorksheet, "ModifiedUserName");

                    SetCell(1, Global.CBVHiddenSheetHeader.SerializeSheetProperties, newWorksheet, "SerializeSheetProperties");

                    SetCell(1, Global.CBVHiddenSheetHeader.CellDropdownColList, newWorksheet, "SeralizeCellDropdownColList");

                    SetCell(1, Global.CBVHiddenSheetHeader.AddInCellDropdownList, newWorksheet, "AddInCellDropdownList");

                    SetCell(1, Global.CBVHiddenSheetHeader.UID, newWorksheet, "UID");

                    SetCell(1, Global.CBVHiddenSheetHeader.Server, newWorksheet, "Server");
                    SetCell(1, Global.CBVHiddenSheetHeader.Servermode, newWorksheet, "Servermode");

                    newWorksheet.Visible = Excel::XlSheetVisibility.xlSheetHidden;

                }
                return newWorksheet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Is_CBVEHidden()
        {
            try
            {
                Excel::Worksheet _WorkSheet = GlobalCBVExcel.FindSheet(Global.COEDATAVIEW_HIDDENSHEET);
                if (_WorkSheet == null)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static void RemoveWorksheet(Excel.Sheets worksheets, string sheetname)
        {
            //DisplayAlerts = true : will display confirmation popup for the deletion.                     
            //to do a silent deletion, we'll set it to false (but save the old values to                     restore it) 
            bool displayAlerts = worksheets.Application.DisplayAlerts;
            try
            {
                worksheets.Application.DisplayAlerts = false;
                for (int i = worksheets.Count; i > 0; i--)
                {
                    Excel::Worksheet nSheet = worksheets[i] as Excel.Worksheet;
                    //Coverity fix - CID 18732
                    if (nSheet == null)
                        throw new System.NullReferenceException("Worksheet cannot be null");
                    if (nSheet.Name.Equals(sheetname, StringComparison.OrdinalIgnoreCase))
                    {
                        nSheet.Delete();
                        return;
                    }
                }
            }
            finally
            {
                worksheets.Application.DisplayAlerts = displayAlerts;
            }
        }
        /// <summary>
        /// Auto fit the columns those doesn't have any structure/image data.
        /// </summary>
        /// <param name="nRange"></param>
        /// <param name="resultDataSet"></param>

        public static void AutoFitColumn(Excel::Range nRange, DataTable resultDataSet)
        {
            if (Global.MaxRecordInResultSet <= 0 || resultDataSet == null || resultDataSet.Rows.Count <= 0)
            {
                nRange.EntireColumn.AutoFit();
                return;
            }
            else if ((int)nRange.Column > resultDataSet.Columns.Count)
            {
                nRange.EntireColumn.AutoFit();
                return;
            }
            else
            {
                for (int col = 0; col < resultDataSet.Columns.Count; col++)
                {
                    if (((int)nRange.Column - 1) == col)
                    {
                        if (resultDataSet.Columns[col].ColumnName.Contains(Global.COESTRUCTURE_INDEXTYPE))
                        {
                            return;
                        }
                        else if (Global.ISImageTypeContains(Global._CBVResult.Columns[col].ColumnName))
                        {
                            return;
                        }
                        else
                        {
                            nRange.EntireColumn.AutoFit();
                            return;
                        }
                    }
                }
            }
        }

        public static Excel::Worksheet FindSheet(string sheetName)
        {

            Excel::Worksheet _WorkSheet = null;
            Excel::Application _ExcelApp = Global._ExcelApp as Excel::Application;
            //Coverity fix - CID 18723
            if (_ExcelApp == null)
                throw new System.NullReferenceException();
            string value = string.Empty;
            int intSheetCount = 0;

            try
            {
                if (Global._ExcelApp.Workbooks.Count <= 0)
                {
                    Global._ExcelApp.Workbooks.Add(Type.Missing);
                }


                intSheetCount = _ExcelApp.ActiveWorkbook.Sheets.Count;
            }
            catch
            {
                MessageBox.Show(Properties.Resources.msgErrorSheetCount, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception(Properties.Resources.msgErrorSheetCount);
            }

            if (intSheetCount > 0)
            {
                for (int i = 1; i <= intSheetCount; i++)
                {
                    try
                    {
                        _WorkSheet = (Excel.Worksheet)_ExcelApp.ActiveWorkbook.Sheets[i];
                        //if (_WorkSheet.Name == Global.COEDATAVIEW_HIDDENSHEET)
                        if (_WorkSheet.Name == sheetName.Trim())
                        {
                            return _WorkSheet;
                            //break;
                        }
                    }
                    catch
                    {
                        throw new Exception(Properties.Resources.msgErrorSheetCount);
                    }
                }
            }
            return null;
        }

        public static bool IsSheetExistsInHidden(string sheetName)
        {
            Excel::Worksheet nSheet = GlobalCBVExcel.Get_CSHidden();

            if (nSheet != null)
            {
                // UnProtectHiddenSheet(nSheet); //Un protect the hidden sheet for edit/update data.
                for (int rows = 2; rows <= nSheet.UsedRange.Rows.Count; rows++)
                {
                    Excel.Range cell = (Excel.Range)nSheet.Cells[rows, (int)Global.CBVHiddenSheetHeader.Sheetname];
                    if (cell.Text.ToString().Trim().Equals(sheetName.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public static bool IsSheetExists(string sheetName)
        {
            Excel::Worksheet _WorkSheet = null;
            Excel::Application _ExcelApp = Global._ExcelApp as Excel::Application;
            //Coverity fix - CID 18724
            if (_ExcelApp == null)
                throw new System.NullReferenceException();
            string value = string.Empty;
            int intSheetCount = 0;

            try
            {
                intSheetCount = _ExcelApp.ActiveWorkbook.Sheets.Count;
            }
            catch
            {
                MessageBox.Show(Properties.Resources.msgErrorSheetCount, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception(Properties.Resources.msgErrorSheetCount);
            }

            if (intSheetCount > 0)
            {
                for (int i = 1; i <= intSheetCount; i++)
                {
                    try
                    {
                        _WorkSheet = (Excel.Worksheet)_ExcelApp.ActiveWorkbook.Sheets[i];
                        //if (_WorkSheet.Name == Global.COEDATAVIEW_HIDDENSHEET)
                        if (_WorkSheet.Name == sheetName.Trim())
                        {
                            return true;
                            //break;
                        }
                    }
                    catch
                    {
                        throw new Exception();
                    }
                }
            }
            return false;
        }


        public static bool SheetExists(string lastSheetName)
        {
            Excel::Application nApplication = Global._ExcelApp as Excel.Application;
            //Coverity fix - CID 18725
            if (nApplication == null)
                throw new System.NullReferenceException();
            foreach (Excel::Worksheet nSheet in nApplication.Worksheets)
            {
                if (nSheet.Name.Trim().Equals(lastSheetName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// It's check whether the current sheet in edit mode or not
        /// </summary>
        /// <returns></returns>

        public static bool ISSheetInEditMode()
        {
            object m = Type.Missing;
            const int MENU_ITEM_TYPE = 1;
            const int NEW_MENU = 18;

            Office.CommandBarControl oNewMenu = Global._ExcelApp.Application.CommandBars["Worksheet Menu Bar"].FindControl(MENU_ITEM_TYPE, NEW_MENU, m, m, true);

            if (oNewMenu != null)
            {
                // Check if "New" menu item is enabled or not.     
                if (!oNewMenu.Enabled)
                {
                    //return true;
                    throw new Exception(Properties.Resources.excepEditMode);
                }
            }
            return false;
        }


        public static void FormatSheet(Excel.Worksheet worksheet)
        {
            try
            {
                Excel.Range nTarget;
                //AutoFit columns A:D.
                nTarget = worksheet.get_Range("A1", "D1");
                if (nTarget == null)
                    throw new System.NullReferenceException();
                nTarget.EntireColumn.AutoFit();

                nTarget = worksheet.Rows[Global.CBVHeaderRow.HeaderCategoryRow, Type.Missing] as Excel.Range;
                //Coverity fix - CID 18731
                if (nTarget == null)
                    throw new System.NullReferenceException();
                nTarget.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkGray);
                nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].Weight = Excel.XlBorderWeight.xlThin;
                nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

                nTarget = worksheet.Rows[Global.CBVHeaderRow.HeaderTableRow, Type.Missing] as Excel.Range;
                //Coverity fix - CID 18731
                if (nTarget == null)
                    throw new System.NullReferenceException();
                nTarget.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].Weight = Excel.XlBorderWeight.xlThin;
                nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

                nTarget = worksheet.Rows[Global.CBVHeaderRow.HeaderColumnRow, Type.Missing] as Excel.Range;
                //Coverity fix - CID 18731
                if (nTarget == null)
                    throw new System.NullReferenceException();
                nTarget.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].Weight = Excel.XlBorderWeight.xlThin;
                nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

                nTarget = worksheet.Rows[Global.CBVHeaderRow.HeaderWhereRow, Type.Missing] as Excel.Range;
                //Coverity fix - CID 18731
                if (nTarget == null)
                    throw new System.NullReferenceException();
                nTarget.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(223, 225, 225));
                //nTarget.NumberFormat = "@";
                nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].Weight = Excel.XlBorderWeight.xlThin;
                nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

                nTarget = worksheet.Rows[Global.CBVHeaderRow.HeaderOptionRow, Type.Missing] as Excel.Range;
                //Coverity fix - CID 18731
                if (nTarget == null)
                    throw new System.NullReferenceException();
                nTarget.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkGray);
                nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].Weight = Excel.XlBorderWeight.xlThin;
                nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

                nTarget = worksheet.Rows[Global.CBVHeaderRow.HeaderColumnLabelDisplayRow, Type.Missing] as Excel.Range;
                //Coverity fix - CID 18731
                if (nTarget == null)
                    throw new System.NullReferenceException();
                nTarget.RowHeight = 25;
                nTarget.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                nTarget.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                nTarget.Font.Bold = true;
                nTarget.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                nTarget.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;
                nTarget.Borders[Excel.XlBordersIndex.xlEdgeBottom].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion "Public Methods"

        #region "Read/Write Excel Range"

        public static string GetCell(string nAddress, Excel::Worksheet worksheet)
        {
            string value = string.Empty;
            try
            {
                Global._ExcelApp.EnableEvents = false;
                Excel::Range cell = worksheet.get_Range(nAddress, Type.Missing);
                value = cell.Value2.ToString();
                Global._ExcelApp.EnableEvents = true;
                return cell.Value2.ToString();
            }
            catch
            {
                return value;
            }
        }

        public static string GetCell(object Row, object Col, object sheet)
        {
            try
            {
                Global._ExcelApp.EnableEvents = false;
                Excel.Worksheet worksheet = sheet as Excel.Worksheet;
                //Coverity fix - CID 18727
                if (worksheet == null)
                    throw new System.NullReferenceException("Worksheet cannot be null");
                Excel.Range cell = worksheet.Cells[Row, Col] as Excel.Range;
                //Coverity fix - CID 18727
                if (cell == null)
                    throw new System.NullReferenceException("Cell value cannot be null");
                Global._ExcelApp.EnableEvents = true;
                CellData cellData = Global.GetCellData(cell);
                if (null != cellData.cdxData)
                    return cellData.cdxData;
                else
                    //return cell.Text.ToString();
                    if ((cell != null) && (cell.Value2 != null))
                    {
                        return cell.Value2.ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                //return cell.Text.ToStrinIg();
            }
            catch
            {
                return String.Empty;
            }
        }

        public static string GetCellR1C1(object Row, object Col, object sheet)
        {
            try
            {
                Global._ExcelApp.EnableEvents = false;
                Excel.Worksheet worksheet = sheet as Excel.Worksheet;
                //Coverity fix - CID 18728
                if (worksheet == null)
                    throw new System.NullReferenceException();
                Excel.Range cell = worksheet.Cells[Row, Col] as Excel.Range;
                //Coverity fix - CID 18728
                if (cell == null)
                    throw new System.NullReferenceException();
                Global._ExcelApp.EnableEvents = true;
                CellData cellData = Global.GetCellData(cell);
                //Coverity fix - CID 18728
                if (cell == null)
                    throw new System.NullReferenceException();
                if (null != cellData.cdxData)
                    return cellData.cdxData;
                else if ((cell != null) && ((bool)cell.HasFormula == true) && (cell.FormulaR1C1 != null))
                    return cell.FormulaR1C1.ToString();
                else
                    if ((cell != null) && (cell.Value2 != null))
                    {
                        return cell.Value2.ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
            }
            catch
            {
                return String.Empty;
            }
        }

        public static void SetCell(object Row, object Col, object sheet, string StrValue)
        {
            try
            {
                Excel.Worksheet worksheet = sheet as Excel.Worksheet;
                //Coverity fix - CID 18733
                if (worksheet == null)
                    throw new System.NullReferenceException("Worksheet cannot be null");
                Excel.Range cell = worksheet.Cells[Row, Col] as Excel.Range;
                //Coverity fix - CID 18733
                if (cell == null)
                    throw new System.NullReferenceException("Cell value cannot be null");
                cell.Value2 = StrValue;

            }
            catch
            {
                throw new Exception(String.Format(Properties.Resources.msgErrorExlCell, Row, Col));
            }
        }

        #endregion "Read/Write Excel Range"
    }

    #endregion _ GlobalCBVExcelClass _

    #region _  COEDataViewData class  _

    partial class COEDataViewData : IDisposable
    {
        #region .ctor and Dispose methods

        private bool _Disposed;
        public COEDataViewData()
        {
        }
        public virtual void Dispose()
        {
            if (!_Disposed)
            {

                GC.SuppressFinalize(this);
                _Disposed = true;
            }
        }
        #endregion

        public string IndexType(string colAlias, string tableName)
        {
            try
            {
                int tableID = Convert.ToInt32(tableName.Replace("table_", ""));
                Global.DataView = Global.CBVSHEET_COEDATAVIEWBO.COEDataView;
                Global.Tables = TableListBO.NewTableListBO(Global.DataView.Tables);
                TableBO baseTable = Global.Tables.GetTable(tableID);

                for (int j = 0; j < baseTable.Fields.Count; j++)
                {
                    if (baseTable.Fields[j].Alias != null)
                        if (baseTable.Fields[j].Alias.Equals(colAlias, StringComparison.OrdinalIgnoreCase))
                        {
                            //11.0.3.
                            if (baseTable.Fields[j].LookupFieldId > 0)
                                return (COEDataViewData.GetLookupIndex(baseTable.Fields[j].LookupDisplayFieldId));
                            else
                                return baseTable.Fields[j].IndexType.ToString();
                        }
                }
                return "NONE";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string IndexType(string colName, string tableName, bool IsColTableAlias)
        {
            try
            {
                if (IsColTableAlias)
                {
                    for (int i = 0; i < Global.Tables.Count; i++)
                    {
                        for (int j = 0; j < Global.Tables[i].Fields.Count; j++)
                        {
                            if (Global.Tables[i].Alias.Trim().Equals(tableName, StringComparison.OrdinalIgnoreCase))
                            {
                                if (Global.Tables[i].Fields[j].Alias.Trim().Equals(colName, StringComparison.OrdinalIgnoreCase))

                                    //11.0.3
                                    if (Global.Tables[i].Fields[j].LookupFieldId > 0)
                                        return (COEDataViewData.GetLookupIndex(Global.Tables[i].Fields[j].LookupDisplayFieldId));
                                    else
                                        return Global.Tables[i].Fields[j].IndexType.ToString();
                            }
                        }
                    }
                    return "NONE";
                }
                else
                {
                    for (int i = 0; i < Global.Tables.Count; i++)
                    {
                        for (int j = 0; j < Global.Tables[i].Fields.Count; j++)
                        {
                            if (Global.Tables[i].Name.Trim().Equals(tableName, StringComparison.OrdinalIgnoreCase))
                            {
                                if (Global.Tables[i].Fields[j].Name.Trim().Equals(colName, StringComparison.OrdinalIgnoreCase))
                                    //11.0.3
                                    if (Global.Tables[i].Fields[j].LookupFieldId > 0)
                                        return (COEDataViewData.GetLookupIndex(Global.Tables[i].Fields[j].LookupDisplayFieldId));
                                    else
                                        return Global.Tables[i].Fields[j].IndexType.ToString();
                            }
                        }
                    }
                    return "NONE";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string MimeType(string colAlias, string tableName)
        {
            try
            {
                int tableID = Convert.ToInt32(tableName.Replace("table_", ""));
                Global.DataView = Global.CBVSHEET_COEDATAVIEWBO.COEDataView;
                Global.Tables = TableListBO.NewTableListBO(Global.DataView.Tables);
                TableBO baseTable = Global.Tables.GetTable(tableID);

                for (int j = 0; j < baseTable.Fields.Count; j++)
                {
                    if (baseTable.Fields[j].Alias != null)
                        if (baseTable.Fields[j].Alias.Equals(colAlias, StringComparison.OrdinalIgnoreCase))
                        {
                            //11.0.3
                            if (baseTable.Fields[j].LookupFieldId > 0)
                                return (COEDataViewData.GetLookupMime(baseTable.Fields[j].LookupDisplayFieldId));
                            else //
                                return baseTable.Fields[j].MimeType.ToString();
                        }
                }
                return "NONE";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string MimeType(string colName, string tableName, bool IsColTableAlias)
        {
            try
            {
                if (IsColTableAlias)
                {
                    for (int i = 0; i < Global.Tables.Count; i++)
                    {
                        for (int j = 0; j < Global.Tables[i].Fields.Count; j++)
                        {
                            if (Global.Tables[i].Alias.Trim().Equals(tableName, StringComparison.OrdinalIgnoreCase))
                            {
                                if (Global.Tables[i].Fields[j].Alias.Trim().Equals(colName, StringComparison.OrdinalIgnoreCase))
                                {
                                    //11.0.3
                                    if (Global.Tables[i].Fields[j].LookupFieldId > 0)
                                        return (COEDataViewData.GetLookupMime(Global.Tables[i].Fields[j].LookupDisplayFieldId));
                                    else //
                                        return Global.Tables[i].Fields[j].MimeType.ToString();
                                }
                            }
                        }
                    }
                    return "NONE";
                }
                else
                {
                    for (int i = 0; i < Global.Tables.Count; i++)
                    {
                        for (int j = 0; j < Global.Tables[i].Fields.Count; j++)
                        {
                            if (Global.Tables[i].Name.Trim().Equals(tableName, StringComparison.OrdinalIgnoreCase))
                            {
                                if (Global.Tables[i].Fields[j].Name.Trim().Equals(colName, StringComparison.OrdinalIgnoreCase))
                                {
                                    //11.0.3
                                    if (Global.Tables[i].Fields[j].LookupFieldId > 0)
                                        return (COEDataViewData.GetLookupMime(Global.Tables[i].Fields[j].LookupDisplayFieldId));
                                    else //
                                        return Global.Tables[i].Fields[j].MimeType.ToString();
                                }
                            }
                        }
                    }
                    return "NONE";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public int IsStructure(TableBO baseTable)
        {
            int x = -1;
            for (int j = 0; j < baseTable.Fields.Count; j++)
            {
                //bug #126152               
                if ((baseTable.Fields[j].IndexType.ToString().ToUpper() == Global.COESTRUCTURE_INDEXTYPE) && (baseTable.Fields[j].Visible == true))
                    return j;
            }
            return x;
        }

        /// <summary>
        /// Return the primary key index from base table
        /// </summary>
        /// <param name="baseTable"></param>
        /// <returns></returns>
        public int GetPkeyIndex(TableBO baseTable)
        {
            int x = -1;
            for (int j = 0; j < baseTable.Fields.Count; j++)
            {
                //if (baseTable.PrimaryKey == baseTable.Fields[j].ID)
                // Bug #126152
                if ((baseTable.PrimaryKey == baseTable.Fields[j].ID) && (baseTable.Fields[j].Visible == true))
                    return j;
            }
            return x;
        }

        /// <summary>
        /// Return the unique key index from base table
        /// </summary>
        /// <param name="baseTable"></param>
        /// <returns></returns>
        public int GetUkeyIndex(TableBO baseTable)
        {
            int x = -1;
            for (int j = 0; j < baseTable.Fields.Count; j++)
            {
                //if (baseTable.Fields[j].IsUniqueKey)
                //Bugs #126152
                if (baseTable.Fields[j].IsUniqueKey && (baseTable.Fields[j].Visible == true))
                    return j;

            }
            return x;
        }

        /// <summary>
        /// Return base table
        /// </summary>
        /// <param name="baseTable"></param>
        /// <returns></returns>
        public string GetName(TableBO baseTable)
        {
            string value;

            if ((baseTable.Alias != null) && (baseTable.Alias != String.Empty))
            {
                value = baseTable.Alias.ToString();
            }
            else
            {
                value = baseTable.Name.ToString();
            }
            return value;
        }
    }
    #endregion _  COEDataViewData class  _
}
