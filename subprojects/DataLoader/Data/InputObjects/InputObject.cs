using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Common;
using CambridgeSoft.COE.DataLoader.Windows.Common;
using System.Globalization;

namespace CambridgeSoft.COE.DataLoader.Data.InputObjects
{
    /// <summary>
    /// InputObject base class
    /// </summary>
    public abstract class InputObject
    {
        static NumberStyles _style;
        static CultureInfo _localCulture = System.Threading.Thread.CurrentThread.CurrentCulture;


        #region enums and types
        /// <summary>
        /// Indicates how a delimited file is delimited
        /// </summary>
        public enum DelimiterType
        {
            /// <summary>
            /// Comma delimited
            /// </summary>
            Comma,
            /// <summary>
            /// Tab delimited
            /// </summary>
            Tab
        };
        /// <summary>
        /// Indicates whether an input table has a header
        /// </summary>
        public enum HeaderType
        {
            /// <summary>
            /// Does not have a header
            /// </summary>
            No,
            /// <summary>
            /// Has a header
            /// </summary>
            Yes
        };
        #endregion

        #region data
        private string _xmlConfiguration = string.Empty;  // OutputConfiguration (get / set later)
        private DataSet _DataSetForJob;
        private DataSet _DataSetForPreview;
        private string _strDb;
        private bool _bExternalSortRequired = true;
        private string _strFilter;
        private string _xmlInputFieldSpec = string.Empty;  // FieldSpec (get) (consulted by Mappings) Cfw::OpenTable of Cfw and possibly Mdb for conditional merge
        private string _xmlInputFieldSort = string.Empty;
        private Dictionary<string, string> _dictInputFieldSpec = new Dictionary<string, string>();
        private bool _bInputFieldSortChanged;
        private string _xmlInputFieldSpecMapped; // Mappings (set by InputOutputMapper)
        private string _strInputFilter;
        private bool _bIsValid = false;
        private int _nMaximum;  // For DataSet progress
        private int _nMinimum;  // For DataSet progress
        private int _nRecord;
        private List<string> _listOrdinal = new List<string>();
        private bool _bRecordsApproximate = false;  // note that unknown is not the same as approximate
        private int _cRecords = int.MaxValue; // implies unknown
        static private List<InputObject> _listObjectList;
        private COEProgressHelper _oPh = new COEProgressHelper();
        private string _strTable;
        private List<string> _listTableList = new List<string>();
        private int _nValue; // for DataSet progress
        #endregion

        #region properties
        /// <summary>
        /// Get/set the optional Input object configuration; note the use of <see cref="COEXmlBind"/>.
        /// </summary>
        public virtual string Configuration
        {
            get
            {
                return _xmlConfiguration = (_xmlConfiguration.Length > 0) ? COEXmlBind.LoadValues(_xmlConfiguration, "member", "value", this) : string.Empty;
            }
            set
            {
                _xmlConfiguration = (value != null) ? value : string.Empty;
                if (_xmlConfiguration.Length > 0)
                {
                    COEXmlBind.StoreValues(_xmlConfiguration, "member", "value", this);
                }
                return;
            } // set
        } // Configuration

        /// <summary>
        /// Get DataSet for Job
        /// </summary>
        public DataSet DataSetForJob
        {
            get
            {
                return _DataSetForJob;
            }
            set
            {
                _DataSetForJob = value;
                return;
            }
        }

        /// <summary>
        /// get DataSet for preview (cached)
        /// set (to null) to clear the cache for resort
        /// </summary>
        public virtual DataSet DataSetForPreview
        {
            get
            {
                if ((_DataSetForPreview == null) || (InputFieldSortChanged && (ExternalSortRequired == false)))
                {
                    _DataSetForPreview = ReadDataSetForPreview();   // null or sort changed and does own sorting
                }
                InputFieldSortChanged = false;
                return _DataSetForPreview;
            }
        } // DataSetForPreview

        /// <summary>
        /// set prior to OpenDb
        /// get to store in Job
        /// </summary>
        public virtual string Db
        {
            get
            {
                return (_strDb == null) ? string.Empty : _strDb;
            }
            set
            {
                _strDb = value;    // WJC error if already open
            }
        } // Db

        /// <summary>
        /// "set" directly by InputObjects as a side-effect of InputFieldSort
        /// </summary>
        public virtual bool ExternalSortRequired
        {
            get
            {
                return _bExternalSortRequired && (InputFieldSort != string.Empty);
            }
            protected set
            {
                _bExternalSortRequired = value;
                return;
            }
        } // ExternalSortRequired

        /// <summary>
        /// get used by InputFileChoose
        /// "set" directly by InputObjects
        /// </summary>
        public string Filter
        {
            get
            {
                return (_strFilter == null) ? string.Empty : _strFilter;
            }
            protected set
            {
                _strFilter = value;
                return;
            }
        } // Filter

        /// <summary>
        /// get/set used implicitly by InputFileSort
        /// </summary>
        public virtual string InputFieldSort
        {
            get
            {
                return _xmlInputFieldSort;
            }
            set
            {
                InputFieldSortChanged |= (_xmlInputFieldSort != value);
                _xmlInputFieldSort = value;
                return;
            }
        } // InputFieldSort

        private bool InputFieldSortChanged
        {
            get
            {
                return _bInputFieldSortChanged;
            }
            set
            {
                _bInputFieldSortChanged = value;
                return;
            }
        } // InputFieldSortChanged

        /// <summary>
        /// get Sql clause for InputFieldSort
        /// </summary>
        public string InputFieldSortSql
        {
            get
            {
                string strOrderBy = string.Empty;
                {
                    XmlDocument oXmlDocument = new XmlDocument();
                    oXmlDocument.LoadXml(InputFieldSort);
                    XmlNode oXmlNodeFieldlist = oXmlDocument.DocumentElement;
                    foreach (XmlNode oXmlNodeField in oXmlNodeFieldlist)
                    {
                        if (strOrderBy.Length > 0) strOrderBy += ",";
                        strOrderBy += "[" + oXmlNodeField.Attributes["dbname"].Value + "]";
                        if (oXmlNodeField.Attributes["orderby"].Value == "Descending")
                        {
                            strOrderBy += " DESC";
                        }
                    }
                }
                return strOrderBy;
            }
        } // InputFieldSortSql

        /// <summary>
        /// get/set used by InputFileLabelAndType
        /// </summary>
        public virtual string InputFieldSpec
        {
            get
            {
                return _xmlInputFieldSpec;
            }
            set
            {
                _xmlInputFieldSpec = value;
                _listOrdinal.Clear();
                _dictInputFieldSpec.Clear();
                if (_xmlInputFieldSpec.Length > 0)
                {
                    XmlDocument oXmlDocument = new XmlDocument();
                    oXmlDocument.LoadXml(_xmlInputFieldSpec);
                    XmlNode oXmlNodeFieldlist = oXmlDocument.SelectSingleNode("fieldlist");
                    foreach (XmlNode oXmlNode in oXmlNodeFieldlist)
                    {
                        string strColumnName = oXmlNode.Attributes["dbname"].Value.ToString();
                        _listOrdinal.Add(strColumnName);
                        string strName = (oXmlNode.Attributes["name"] != null) ? oXmlNode.Attributes["name"].Value.ToString() : strColumnName;
                        _dictInputFieldSpec.Add(strName, strColumnName);
                        if (_DataSetForPreview != null)
                            if (_DataSetForPreview.Tables[0].Columns.Contains(strName))
                                _DataSetForPreview.Tables[0].Columns[strColumnName].Caption = strName;

                        if (_DataSetForJob != null)
                            //JED: on 2nd pass of 'upload to registration', this erroneously has a value at the outset
                            if (_DataSetForJob.Tables[0].Columns.Contains(strName))
                                _DataSetForJob.Tables[0].Columns[strColumnName].Caption = strName;
                    }
                }
                else
                {
                    _DataSetForPreview = null;
                    _DataSetForJob = null;
                }
                return;
            }
        } // InputFieldSpec

        /// <summary>
        /// Return InputFieldSpec only for input field participating in mapping 
        /// </summary>
        public string InputFieldSpecMapped
        {
            get
            {
                return _xmlInputFieldSpecMapped;
            }
            private set
            {
                _xmlInputFieldSpecMapped = value;
                return;
            }
        } // InputFieldSpecMapped

        /// <summary>
        /// Set and get for the Input Filter used to invoke this input object
        /// </summary>
        public string InputFilter
        {
            get
            {
                return _strInputFilter;
            }
            set
            {
                _strInputFilter = value;
                return;
            }
        }

        /// <summary>
        /// Return true if the input object is valid for use
        /// </summary>
        public bool IsValid
        {
            get
            {
                return _bIsValid;
            }
            protected set
            {
                _bIsValid = value;
                return;
            }
        } // IsValid

        /// <summary>
        /// set by InputOutputMapper
        /// Side-effect is to build _xmlInputFieldSpecMapped which are unique mapped fields from _xmlInputFieldSpec
        /// </summary>
        public virtual string Mappings
        {
            set
            {
                string _xmlMappings = value;
                if (_xmlMappings.Length > 0)
                {
                    Dictionary<string, string> dictMap = new Dictionary<string, string>();
                    {
                        XmlDocument oXmlDocumentFieldSpec = new XmlDocument();
                        oXmlDocumentFieldSpec.LoadXml(_xmlInputFieldSpec);
                        XmlNode oXmlNodeFieldSpecRoot = oXmlDocumentFieldSpec.DocumentElement;

                        XmlDocument oXmlDocumentMappings = new XmlDocument();
                        oXmlDocumentMappings.LoadXml(_xmlMappings);
                        XmlNode oXmlNodeMappingsRoot = oXmlDocumentMappings.DocumentElement;
                        XmlNodeList oXmlNodeListMappings = oXmlNodeMappingsRoot.SelectNodes("descendant::field[attribute::fields]");
                        foreach (XmlNode oXmlNodeMapping in oXmlNodeListMappings)
                        {
                            string strNameList = oXmlNodeMapping.Attributes["fields"].Value;
                            string[] strNames = strNameList.Split(';');
                            foreach (string strName in strNames)
                            {
                                if (dictMap.ContainsKey(strName) == false)
                                {
                                    XmlNode oXmlNodeFieldSpec = oXmlNodeFieldSpecRoot.SelectSingleNode("child::field[attribute::name='" + strName + "']");
                                    if (oXmlNodeFieldSpec == null)
                                    {
                                        oXmlNodeFieldSpec = oXmlNodeFieldSpecRoot.SelectSingleNode("child::field[attribute::name='" + strName.ToUpper() + "']");
                                    }
                                    string strDbName = oXmlNodeFieldSpec.Attributes["dbname"].Value;
                                    string strDbType = oXmlNodeFieldSpec.Attributes["dbtype"].Value;
                                    dictMap.Add(strName, strDbName + '\t' + strDbType);
                                }
                            }
                        } // foreach (XmlNode oXmlNodeMapping in oXmlNodeListMappings)
                    }
                    {
                        COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                        oCOEXmlTextWriter.WriteStartElement("fieldlist");
                        foreach (KeyValuePair<string, string> kvp in dictMap)
                        {
                            oCOEXmlTextWriter.WriteStartElement("field");
                            string[] strValue = kvp.Value.Split('\t');
                            oCOEXmlTextWriter.WriteAttributeString("dbname", strValue[0]);
                            oCOEXmlTextWriter.WriteAttributeString("dbtype", strValue[1]);
                            oCOEXmlTextWriter.WriteAttributeString("name", kvp.Key);
                            oCOEXmlTextWriter.WriteEndElement();
                        }
                        oCOEXmlTextWriter.WriteEndElement();
                        InputFieldSpecMapped = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                        oCOEXmlTextWriter.Close();
                    }
                }
                else
                {
                    _xmlInputFieldSpecMapped += string.Empty;   // Hmmm
                }
                return;
            } // set
        } // Mappings

        /// <summary>
        /// Used for DataSet progress
        /// </summary>
        public int Maximum
        {
            get
            {
                return _nMaximum;
            }
            protected set
            {
                _nMaximum = value;
                return;
            }
        } // Maximum

        /// <summary>
        /// Used for DataSet progress
        /// </summary>
        public int Minimum
        {
            get
            {
                return _nMinimum;
            }
            protected set
            {
                _nMinimum = value;
                return;
            }
        } // Minimum

        /// <summary>
        /// Return list of valid input objects
        /// </summary>
        static public List<InputObject> ObjectList
        {
            get
            {
                if (_listObjectList != null)
                {
                    for (int nInputObject = 0; nInputObject < _listObjectList.Count; nInputObject++)
                    {
                        _listObjectList[nInputObject] = null; ;
                    }
                    _listObjectList.Clear();
                }
                else
                {
                    _listObjectList = new List<InputObject>();
                }
                {
                    InputObject oInputObject = new InputObjectCfw();
                    if (oInputObject.IsValid)
                    {
                        _listObjectList.Add(oInputObject);
                    }
                }
                {
                    InputObject oInputObject = new InputObjectMdb();
                    if (oInputObject.IsValid)
                    {
                        _listObjectList.Add(oInputObject);
                    }
                }
                {
                    InputObject oInputObject = new InputObjectMst();
                    if (oInputObject.IsValid)
                    {
                        _listObjectList.Add(oInputObject);
                    }
                }
                {
                    InputObject oInputObject = new InputObjectSdf();
                    if (oInputObject.IsValid)
                    {
                        _listObjectList.Add(oInputObject);
                    }
                }
                {
                    InputObject oInputObject = new InputObjectTxt();
                    if (oInputObject.IsValid)
                    {
                        _listObjectList.Add(oInputObject);
                    }
                }
                {
                    InputObject oInputObject = new InputObjectTxtBlock();
                    if (oInputObject.IsValid)
                    {
                        _listObjectList.Add(oInputObject);
                    }
                }
                {
                    InputObject oInputObject = new InputObjectXl();
                    if (oInputObject.IsValid)
                    {
                        _listObjectList.Add(oInputObject);
                    }
                }
                return _listObjectList;
            }
        } // ObjectList

        /// <summary>
        /// Access to the ProgressHelper
        /// </summary>
        public virtual COEProgressHelper Ph
        {
            get
            {
                return _oPh;
            }
            set
            {
                _oPh = value;
                return;
            }
        } // Ph

        /// <summary>
        /// Get/set 0-relative record number to be read next sequentially
        /// </summary>
        public int Record
        {
            get
            {
                return _nRecord;
            }
            protected set
            {
                _nRecord = value;   // Responds to positioning. Does not cause positioning.
                return;
            }
        } // Record

        /// <summary>
        /// Get/set number of input records
        /// </summary>
        public int Records
        {
            get
            {
                return _cRecords;
            }
            protected set
            {
                _cRecords = value;
                return;
            }
        } // Records

        /// <summary>
        /// True if <see cref="Records"/> is an approximate (understated) value
        /// </summary>
        public bool RecordsApproximate
        {
            get
            {
                return _bRecordsApproximate;
            }
            protected set
            {
                _bRecordsApproximate = value;
                return;
            }
        } // RecordsApproximate

        /// <summary>
        /// True if <see cref="Records"/> is an unknown
        /// </summary>
        public bool RecordsUnknown
        {
            get
            {
                return (Records == int.MaxValue);
            }
            set
            {
                Records = int.MaxValue;
                return;
            }
        } // RecordsUnknown
        
        /// <summary>
        /// Get the number of input tables
        /// </summary>
        public int Tables
        {
            get
            {
                return TableList.Count;
            }
        } // Tables
        
        /// <summary>
        /// Get/set the currently selected table
        /// </summary>
        public string Table
        {
            get
            {
                return (_strTable == null) ? string.Empty : _strTable;
            }
            set
            {
                _strTable = value;    // WJC error if already open
                return;
            }
        } // Table

        /// <summary>
        /// Return the list of available tables
        /// </summary>
        public List<string> TableList
        {
            get
            {
                return _listTableList;
            }
        } // TableList

        /// <summary>
        /// Used internally to initialize the configuration options
        /// </summary>
        protected string UnboundConfiguration
        {
            set
            {
                _xmlConfiguration = value;
                return;
            }
        } // UnboundConfiguration

        /// <summary>
        /// Used for DataSet progress
        /// </summary>
        public int Value
        {
            get
            {
                return _nValue;
            }
            protected set
            {
                _nValue = value;
                return;
            }
        } // Value

        #endregion

        #region methods
        /// <summary>
        /// Append DataColumns to a DataTable based on the InputFieldSpec
        /// </summary>
        /// <param name="voDataTable"></param>
        /// <param name="vxmlInputFieldSpec"></param>
        protected void DataTableAddColumns(DataTable voDataTable, string vxmlInputFieldSpec)
        {
            if (!string.IsNullOrEmpty(vxmlInputFieldSpec))
            {
                XmlDocument oXmlDocument = new XmlDocument();
                oXmlDocument.LoadXml(vxmlInputFieldSpec);
                XmlNode oXmlNodeFieldlist = oXmlDocument.SelectSingleNode("fieldlist");
                foreach (XmlNode oXmlNode in oXmlNodeFieldlist)
                {
                    string strColumnName = oXmlNode.Attributes["dbname"].Value.ToString();
                    string strColumnType = oXmlNode.Attributes["dbtype"].Value.ToString();
                    string strCaption = (oXmlNode.Attributes["name"] != null) ? oXmlNode.Attributes["name"].Value.ToString() : strColumnName;
                    if ((strColumnType.IndexOf('[') > 0) && (strColumnType.Substring(strColumnType.IndexOf('['), 2) != "[]")) strColumnType = "System.String";    // All arrays are passed as strings
                    if (strColumnType == "Base64Cdx") strColumnType = "System.String";
                    if (strColumnType == "Time") strColumnType = "System.DateTime";
                    if (strColumnType == "null") strColumnType = "System.String";
                    {
                        DataColumn oDataColumn = new DataColumn();
                        oDataColumn.DataType = Type.GetType(strColumnType);
                        oDataColumn.Caption = strCaption;
                        oDataColumn.ColumnName = strColumnName;
                        oDataColumn.ExtendedProperties.Add("Ordinal", _listOrdinal.IndexOf(strColumnName));
                        voDataTable.Columns.Add(oDataColumn);
                    }
                }
            }
        }

        /// <summary>
        /// Will return the string representation of the type indicated by the method name.
        /// </summary>
        /// <param name="proposedType">the presumed data-type, formatted as a string</param>
        /// <param name="dataValue">the value to ascertain the type of</param>
        /// <returns>a string representation of the data-type this value most likely represents</returns>
        [Description("reusable")]
        protected static string DetermineType(string proposedType, string dataValue)
        {
            //set a default
            string determinedType = proposedType;

            //bypass type determination on empty strings or nulls
            if (string.IsNullOrEmpty(dataValue))
                return determinedType;

            //utilize "Type".TryParse(...) to determine the type
            switch (proposedType)
            {
                case "System.Int32":
                    {
                        determinedType = VerifyInt32(dataValue);
                        if (determinedType != null)
                            return determinedType;

                        determinedType = VerifyDouble(dataValue);
                        if (determinedType != null)
                            return determinedType;

                        break;
                    }
                case "System.Double":
                    {
                        determinedType = VerifyDouble(dataValue);
                        if (determinedType != null)
                            return determinedType;

                        break;
                    }
                case "System.DateTime":
                case "Time":
                    {
                        determinedType = VerifyDateAndOrTime(dataValue);
                        if (determinedType != null)
                            return determinedType;

                        break;
                    }
                case "System.Boolean":
                    {
                        determinedType = VerifyBoolean(dataValue);
                        if (determinedType != null)
                            return determinedType;

                        break;
                    }
                case "Base64Cdx":
                    {
                        determinedType = VerifyBase64Cdx(dataValue);
                        if (determinedType != null)
                            return determinedType;

                        break;
                    }
                case "null":
                    {
                        determinedType = VerifyInt32(dataValue);
                        if (determinedType != null)
                            return determinedType;

                        determinedType = VerifyDouble(dataValue);
                        if (determinedType != null)
                            return determinedType;

                        determinedType = VerifyBoolean(dataValue);
                        if (determinedType != null)
                            return determinedType;

                        determinedType = VerifyBase64Cdx(dataValue);
                        if (determinedType != null)
                            return determinedType;

                        determinedType = VerifyDateAndOrTime(dataValue);
                        if (determinedType != null)
                            return determinedType;

                        break;
                    }
            }

            if (determinedType == null)
                determinedType = "System.String";

            return determinedType;
        }

        /// <summary>
        /// Will return the string representation of the type indicated by the method name.
        /// </summary>
        /// <param name="dataValue">the value to ascertain the type of</param>
        /// <returns></returns>
        [Description("reusable")]
        private static string VerifyInt32(string dataValue)
        {
            int i;
            if (int.TryParse(dataValue, out i))
                return "System.Int32";
            else
                return null;
        }

        /// <summary>
        /// Will return the string representation of the type indicated by the method name.
        /// </summary>
        /// <param name="dataValue">the value to ascertain the type of</param>
        /// <returns></returns>
        [Description("reusable")]
        private static string VerifyDouble(string dataValue)
        {
            double d;
            _style = NumberStyles.AllowDecimalPoint;
            if (Double.TryParse(dataValue, _style, _localCulture, out d))
                return "System.Double";
            else
                return null;
        }

        /// <summary>
        /// Will return the string representation of the type indicated by the method name.
        /// </summary>
        /// <param name="dataValue">the value to ascertain the type of</param>
        /// <returns></returns>
        [Description("reusable")]
        private static string VerifyBoolean(string dataValue)
        {
            bool b;
            if (bool.TryParse(dataValue, out b))
                return "System.Boolean";
            else
                return null;
        }

        /// <summary>
        /// Will return the string representation of the type indicated by the method name.
        /// </summary>
        /// <param name="dataValue">the value to ascertain the type of</param>
        /// <returns></returns>
        [Description("reusable")]
        private static string VerifyBase64Cdx(string dataValue)
        {
            if (dataValue.StartsWith("VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACA"))
                return "Base64Cdx";
            else
                return null;
        }

        /// <summary>
        /// Will return the string representation of the type indicated by the method name.
        /// </summary>
        /// <param name="dataValue">the value to ascertain the type of</param>
        /// <returns></returns>
        [Description("reusable")]
        private static string VerifyDateAndOrTime(string dataValue)
        {
            DateTime dt;
            if (DateTime.TryParse(dataValue
                , System.Threading.Thread.CurrentThread.CurrentCulture
                , System.Globalization.DateTimeStyles.NoCurrentDateDefault
                , out dt)
                )
            {
                //CBOE-902 Check if the input is having time
                var chkDateTime = System.DateTime.Parse(dataValue);
                if ((dt.Year == 1) && (dt.Month == 1) && (dt.Day == 1))
                    return "Time";                    
                else if (chkDateTime.TimeOfDay.TotalSeconds != 0)
                    return "System.DateTime";
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// Return DbName for the given Name
        /// </summary>
        /// <param name="vstrName"></param>
        /// <returns></returns>
        public string NameGetDbName(string vstrName)
        {
            string strRet;
            strRet = _dictInputFieldSpec.ContainsKey(vstrName) ? _dictInputFieldSpec[vstrName] : string.Empty;
            return strRet;
        } // NameGetDbName()

        #region LogMessageList
        private LogMessageList _LogMessageList = new LogMessageList();
        /// <summary>
        /// Append a message to the message list
        /// </summary>
        /// <param name="veLogSeverity"></param>
        /// <param name="veLogSource"></param>
        /// <param name="vnTransaction"></param>
        /// <param name="vstrLogMessage"></param>
        protected void AddMessage(LogMessage.LogSeverity veLogSeverity, LogMessage.LogSource veLogSource, int vnTransaction, string vstrLogMessage)
        {
            _LogMessageList.Add(veLogSeverity, veLogSource, vnTransaction, vstrLogMessage);
            return;
        } // AddMessage()

        /// <summary>
        /// Append a list of errors to the message list
        /// </summary>
        /// <param name="vlistLogMessageList"></param>
        protected void AddMessages(LogMessageList vlistLogMessageList)
        {
            _LogMessageList.Add(vlistLogMessageList);
            return;
        } // AddMessage()

        /// <summary>
        /// Clear the message list
        /// </summary>
        protected void ClearMessages()
        {
            _LogMessageList.Clear();
            return;
        } // ClearMessages()

        /// <summary>
        /// Return true if there are any errors reported in the message list
        /// </summary>
        public bool HasMessages
        {
            get
            {
                return (_LogMessageList.Count > 0);
            }
        } // HasMessages

        /// <summary>
        /// The message list (if any)
        /// </summary>
        public LogMessageList MessageList
        {
            get
            {
                return _LogMessageList;
            }
        } // MessageList

        #endregion

        #region TableList
        /// <summary>
        /// Add Table to the TableList
        /// </summary>
        /// <param name="vstrTable"></param>
        protected void AddTableToTableList(string vstrTable)
        {
            _listTableList.Add(vstrTable);
            return;
        } // AddTableToTableList()

        /// <summary>
        /// Clear the TableList
        /// </summary>
        protected void ClearTableList()
        {
            _listTableList.Clear();
            return;
        } // ClearTableList()
        #endregion

#endregion

        #region abstract declarations
        /// <summary>
        /// Close the database
        /// </summary>
        public abstract void CloseDb();

        /// <summary>
        /// Open the database
        /// </summary>
        /// <returns></returns>
        public abstract bool OpenDb();    // true iif messages

        /// <summary>
        /// Close a table within the database
        /// </summary>
        /// <returns></returns>
        public virtual bool CloseTable()
        {
            ClearMessages();
            do
            {
                // WJC Perhaps this could be elsewhere such as in the Table property iif Table changes (CloseDb should set Table = string.Empty)
                _DataSetForPreview = null;
                _DataSetForJob = null;
                InputFieldSpec = string.Empty;
                InputFieldSort = string.Empty;
                InputFieldSpecMapped = string.Empty;
                Mappings = string.Empty;
                Record = 0;
                Records = int.MaxValue; // Unknown
                RecordsApproximate = false;
                Table = string.Empty;
            } while (false);
            return HasMessages;
        } // CloseTable()

        /// <summary>
        /// Open a table within the database
        /// </summary>
        /// <returns></returns>
        public abstract bool OpenTable();    // true iif messages

        /// <summary>
        /// Close the DataSet for processing
        /// </summary>
        /// <returns></returns>
        public virtual bool CloseDataSet()  // true iif messages
        {
            return HasMessages;
        } // CloseDataSet()

        /// <summary>
        /// Open of the DataSet for processing
        /// </summary>
        /// <returns></returns>
        protected  void OpenDataSet()
        {
            // DataSet
            DataSetForJob = new DataSet(Table + "List");
            // DataTable
            DataSetForJob.Tables.Add(Table);
            DataTable oDataTable = DataSetForJob.Tables[0];
            DataTableAddColumns(oDataTable, InputFieldSpecMapped);
            Value = 0;
            return;
        } // OpenDataSet()

        /// <summary>
        /// Open DataSet for processing
        /// </summary>
        /// <returns></returns>
        public abstract bool OpenDataSet(int vnStart, int vcLimit);  // nvStart and vcLimit are number of records   // true iif messages

        /// <summary>
        /// Get DataSet for processing
        /// </summary>
        /// <returns></returns>
        public abstract bool ReadDataSet(int vcLimit, ref System.Data.DataSet riDataSet);  // true iif error

        /// <summary>
        /// Get the preview DataSet
        /// </summary>
        /// <returns></returns>
        protected abstract System.Data.DataSet ReadDataSetForPreview();    // null iif error

        #endregion

    }
}
