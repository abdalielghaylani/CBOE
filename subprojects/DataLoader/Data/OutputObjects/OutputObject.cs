using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Xml;

using CambridgeSoft.COE.DataLoader.Calculation.Parser;
using CambridgeSoft.COE.DataLoader.Common;
using CambridgeSoft.COE.DataLoader.Windows.Common;

namespace CambridgeSoft.COE.DataLoader.Data.OutputObjects
{
    /// <summary>
    /// OutputObject base class
    /// </summary>
    public abstract class OutputObject
    {
        private string _serverDateFormat = "yyyy-MM-dd hh:mm:ss"; //"yyyy-MM-dd hh:mm:ss tt" does NOT work
        private CultureInfo _serverCultureInfo = new CultureInfo("en-US", false);
        
        /// <summary>
        /// To report exceptions
        /// </summary>
        protected const string _fmtException_2 = "Exception during {0:G}: \nMessage: {1:G} \nRecord xml is: {2:G}";
        /// <summary>
        /// To report exception XML
        /// </summary>
        protected const string _fmtExceptionXML_1 = "XML\n{0:G}";
        private const string _fmtParserFormula_1 = "Invalid formula: {0:G}";
        private const string _fmtParserError_2 = "Parsing error {0:G} at column {1:G}";
        private const string _fmtPicklistError_1 = "{0:G} is not a valid value for the {1:G} picklist";
        protected const string _fmtGenericError = "{0:G}";
        protected const string _fmtNoSuchBatchId = "No such batch ID as {0:G}";

        public const string MIXTURE = "mixturecsladatasource";
        public const string COMPONENT = "componentlistcsladatasource";
        public const string BATCH = "batchlistcsladatasource";
        public const string FRAGMENT = "fragmentscsladatasource";

        private Dictionary<string, string> _dictConstant = null;
        private Dictionary<string, string> _dictMapped = null;
        private Dictionary<string, Dictionary<string, Int32>> _dictPicklist = null;
        private CalculationParser _oParser = null;
        private LogMessageList _LogMessageList = new LogMessageList();

        #region > Properties <

        private bool _bIsValid;
        /// <summary>
        /// Get / set IsValid
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
        }

        private string _xmlConfiguration = string.Empty;  // OutputConfiguration (get / set later)
        /// <summary>
        /// Used for optional output object configuration
        /// </summary>
        public virtual string Configuration
        {
            get
            {
                if (_xmlConfiguration.Length > 0)
                {
                    _xmlConfiguration = COEXmlTextWriter.Pretty(COEXmlBind.LoadValues(_xmlConfiguration, "member", "value", this));
                }
                return _xmlConfiguration;
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
        }

        private string _strDb;
        /// <summary>
        /// The output object database
        /// </summary>
        public string Db
        {
            get
            {
                return (_strDb == null) ? string.Empty : _strDb;
            }
            set
            {
                _strDb = value;    // WJC error if already open
                return;
            }
        }

        private string _strFilter = string.Empty; // For those that can output to a file
        /// <summary>
        /// The output object file filter
        /// </summary>
        public virtual string Filter
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
        }

        /// <summary>
        /// Build the output field specification based on an input field specification
        /// </summary>
        public virtual string InputFieldSpec
        {
            set
            {
                return;
            }
        }

        private string _xmlMappings;    // InputOutputMapper (set)
        /// <summary>
        /// Output field mappings
        /// </summary>
        public string Mappings
        {
            private get
            {
                return _xmlMappings;
            }
            set
            {
                _xmlMappings = value;
                return;
            }
        }

        /// <summary>
        /// List of all potentially available output objects
        /// </summary>
        static public List<OutputObject> ObjectList
        {
            get
            {
                List<OutputObject> listObjectList = new List<OutputObject>();
                OutputObject oOutputObject;
                {
                    oOutputObject = new OutputObjectReg();
                    if (oOutputObject.IsValid) listObjectList.Add(oOutputObject);
                }

                {
                    oOutputObject = new OutputObjectRegBatch();
                    if (oOutputObject.IsValid) listObjectList.Add(oOutputObject);
                }

#if NOT_IMPLEMENTED_YET
                {
                    oOutputObject = new OutputObjectInvPlate();
                    if (oOutputObject.IsValid) listObjectList.Add(oOutputObject);
                }

                {
                    oOutputObject = new OutputObjectInvContainer();
                    if (oOutputObject.IsValid) listObjectList.Add(oOutputObject);
                }

                {
                    oOutputObject = new OutputObjectInvAdd();
                    if (oOutputObject.IsValid) listObjectList.Add(oOutputObject);
                }
#endif

                {
                    oOutputObject = new OutputObjectTxt();
                    if (oOutputObject.IsValid) listObjectList.Add(oOutputObject);
                }
                {
                    oOutputObject = new OutputObjectXml();
                    if (oOutputObject.IsValid) listObjectList.Add(oOutputObject);
                }
                return listObjectList;
            }
        }

        private DataSet _oOutputDataSet = null;
        /// <summary>
        /// The output DataSet
        /// </summary>
        protected DataSet OutputDataSet
        {
            get
            {
                return _oOutputDataSet;
            }
            private set
            {
                _oOutputDataSet = value;
                return;
            }
        } // OutputDataSet

        private string _xmlOutputFieldSpec;
        /// <summary>
        /// The output field specification
        /// </summary>
        public string OutputFieldSpec
        {
            get
            {
                return _xmlOutputFieldSpec;
            }
            protected set
            {
                _xmlOutputFieldSpec = value;
                return;
            }
        } // OutputFieldSpec

        private string _strOutputType;    // OutputTypeChooser (get)
        /// <summary>
        /// Output type as displayed in the UI
        /// </summary>
        public string OutputType    // WJC should be OutputTask or OutputTasks List<string>
        {
            get
            {
                return _strOutputType;
            }
            protected set
            {
                _strOutputType = value;
                return;
            }
        } // OutputType

        private COEProgressHelper _oPh = new COEProgressHelper();
        /// <summary>
        /// The ProgressHelper; note that the default ProgressHelper does nothing.
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
        /// Used internally to set the initial configuration
        /// </summary>
        protected string UnboundConfiguration
        {
            set
            {
                _xmlConfiguration = value;
                return;
            }
        } // UnboundConfiguration

        #endregion

        #region > Mmethods <

        /// <summary>
        /// Returns the constant value for an output fields with a constant value
        /// </summary>
        /// <param name="vstrOutputField"></param>
        /// <returns></returns>
        protected string ConstantValue(string vstrOutputField)
        {
            return _dictConstant[vstrOutputField];
        } // ConstantValue()

        /// <summary>
        /// Persists the contents of the DataSet
        /// </summary>
        /// <param name="voInputDataSet"></param>
        /// <param name="vnRecord"></param>
        /// <returns></returns>
        public bool DataSetWrite(DataSet voInputDataSet, int vnRecord)
        {
            ClearMessages();
            do
            {
                DataTable oOutputDataTable = OutputDataSet.Tables[0];
                DataTable oInputDataTable = voInputDataSet.Tables[0];
                int cInputRows = oInputDataTable.Rows.Count;
                int nTransaction = vnRecord;
                foreach (DataRow oInputDataRow in oInputDataTable.Rows)
                {
                    nTransaction++;
                    DataRow oOutputDataRow = oOutputDataTable.NewRow();
                    if (_oParser != null)
                    {
                        _oParser.DataRowBind(oInputDataRow, oInputDataTable);
                    }

                    foreach (KeyValuePair<string, string> kvp in _dictMapped)
                    {
                        // Source of the value
                        Object oValue;
                        if (kvp.Value[0] == '=')
                            oValue = _oParser.CalculationExecute(kvp.Value.Substring(1));
                        else
                            oValue = oInputDataRow[kvp.Value];

                        //Evaluate picklist item if value isn't null
                        if (!IsNullValue(oValue))
                        {
                            if (_dictPicklist.ContainsKey(kvp.Key))
                            {
                                if (_dictPicklist[kvp.Key].ContainsKey(oValue.ToString()) == false)
                                {
                                    if (oValue.ToString() != "-1")
                                    {
                                        oOutputDataRow.SetColumnError(kvp.Key, string.Format(_fmtPicklistError_1, oValue.ToString(), kvp.Key));
                                        oValue = -1;
                                        //oValue = System.DBNull.Value;
                                    }
                                }
                                else
                                    oValue = _dictPicklist[kvp.Key][oValue.ToString()].ToString();
                            }
                        }

                        // Store value
                        if (!IsNullValue(oValue))
                        {
                            if ((oOutputDataTable.Columns[kvp.Key].DataType.FullName == "System.Byte[]") && (oValue.GetType().FullName != "System.Byte[]"))
                            {
                                try
                                {
                                    oOutputDataRow[kvp.Key] = Convert.FromBase64String(oValue.ToString());
                                }
                                catch (Exception ex)
                                {
                                    oOutputDataRow.SetColumnError(kvp.Key, string.Format(_fmtException_2, "Convert.FromBase64String", ex.Message));
                                }
                            }
                            else
                            {
                                string strFormat = oOutputDataTable.Columns[kvp.Key].ExtendedProperties["Format"].ToString();
                                if (strFormat == "Time")
                                {
                                    string strValue = oValue.ToString();
                                    int indexOf = strValue.IndexOf(' ');
                                    if (indexOf > 0)
                                        strValue = strValue.Substring(indexOf).Trim();

                                    try
                                    {
                                        oOutputDataRow[kvp.Key] = strValue;
                                    }
                                    catch (Exception ex)
                                    {
                                        oOutputDataRow.SetColumnError(kvp.Key, string.Format(_fmtException_2, "assignment to output", ex.Message));
                                    }
                                } 
                                else if (strFormat == "System.DateTime" || strFormat == "Date")
                                {
                                    string strValue = oValue.ToString();
                                    if (!string.IsNullOrEmpty(strValue))
                                    {
                                        string convertedString = Convert.ToDateTime(strValue).ToString(_serverDateFormat, _serverCultureInfo);
                                        try
                                        {
                                            oOutputDataRow[kvp.Key] = convertedString.ToUpper();
                                        }
                                        catch (Exception ex)
                                        {
                                            oOutputDataRow.SetColumnError(kvp.Key, string.Format(_fmtException_2, "assignment to output", ex.Message));
                                        }
                                    }
                                }
                                else if (strFormat == "System.Boolean" || strFormat == "Boolean") // to fix CSBR-155935
                                {
                                    string strValue = "false";
                                    switch (oValue.ToString().ToLower())
                                    {
                                        case "y":
                                        case "yes":
                                        case "t":
                                        case "true":
                                        case "1":
                                        case "-1":       // CSBR 145474 - condition added to support -1 used for true (consistent with chemfinder)
                                            {
                                                strValue = "true";
                                                break;
                                            }
                                        case "n":
                                        case "no":
                                        case "f":
                                        case "false":
                                        case "0":
                                            {
                                                strValue = "false";
                                                break;
                                            }
                                        default: strValue = "false"; break;
                                    }
                                    bool result;
                                    if (bool.TryParse(strValue.ToString(), out result))
                                        strValue = result.ToString();
                                    try
                                    {
                                        oOutputDataRow[kvp.Key] = strValue;
                                    }
                                    catch (Exception ex)
                                    {
                                        oOutputDataRow.SetColumnError(kvp.Key, string.Format(_fmtException_2, "assignment to output", ex.Message));
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        oOutputDataRow[kvp.Key] = oValue;
                                    }
                                    catch (Exception ex)
                                    {
                                        oOutputDataRow.SetColumnError(kvp.Key, string.Format(_fmtException_2, "assignment to output", ex.Message));
                                    }
                                }
                            }
                        }
                    }
                    
                    oOutputDataTable.Rows.Add(oOutputDataRow);
                }

                oInputDataTable.Rows.Clear();   // WJC TODO we will have to clear these later
                DataSetWrite(vnRecord); // WJC TODO rather than vnRecord we need an array of vnRecord
                oOutputDataTable.Rows.Clear();
                oOutputDataTable.AcceptChanges();
            } while (false);
            return HasMessages;
        }

        /// <summary>
        /// Returns true if the output field is a constant
        /// </summary>
        /// <param name="vstrOutputField"></param>
        /// <returns></returns>
        protected bool IsConstant(string vstrOutputField)
        {
            return _dictConstant.ContainsKey(vstrOutputField);
        } // IsConstant()

        /// <summary>
        /// Returned true if the output field is mapped to an input field
        /// </summary>
        /// <param name="vstrOutputField"></param>
        /// <returns></returns>
        protected bool IsMapped(string vstrOutputField)
        {
            bool isMapped = _dictMapped.ContainsKey(vstrOutputField);
            return isMapped;
        } // IsMapped()

        /// <summary>
        /// Call prior to <see cref="DataSetWrite(DataSet, int)"/> to allow for initialization and optimization
        /// </summary>
        /// <param name="voDataSet"></param>
        /// <returns></returns>
        public bool StartWrite(DataSet voDataSet)
        {
            ClearMessages();
            _dictMapped = new Dictionary<string, string>();
            _dictConstant = new Dictionary<string, string>();
            _dictPicklist = new Dictionary<string, Dictionary<string, int>>();
            do
            {
                { // Build output dataset
                    // Convert mappings to dictionaries for quick access
                    XmlDocument oXmlDocument = new XmlDocument();
                    oXmlDocument.LoadXml(OutputFieldSpec);
                    XmlNode oXmlNodeRoot = oXmlDocument.DocumentElement;
                    XmlNodeList oXmlNodeList = oXmlNodeRoot.SelectNodes("descendant::field");

                    //TODO: Put something intelligent as the DataSet and DataTable names
                    string x = this.Configuration;

                    OutputDataSet = new DataSet();
                    DataTable oDataTable = new DataTable("OutputTable");   // Property ?
                    OutputDataSet.Tables.Add(oDataTable);

                    foreach (XmlNode oXmlNode in oXmlNodeList)
                    {
                        string strFormName = oXmlNode.ParentNode.Attributes["name"].Value;
                        string fldCaption = oXmlNode.Attributes["name"].Value;
                        if (!string.IsNullOrEmpty(strFormName))
                        {
                            fldCaption = strFormName + ":" + fldCaption;
                        }

                        //string fldIdentifier = oXmlNode.Attributes["key"].Value;
                        string strFormat = oXmlNode.Attributes["format"].Value;

                        DataColumn oDataColumn = new DataColumn();
                        oDataColumn.ColumnName = fldCaption;
                        oDataColumn.Caption = fldCaption;
                        Type T;
                        switch (strFormat)
                        {
                            case "Binary": { T = Type.GetType("System.Byte[]"); break; }
                            case "Boolean": { T = Type.GetType("System.Boolean"); break; }
                            case "Date": { T = Type.GetType("System.DateTime"); break; }
                            case "Decimal": { T = Type.GetType("System.Double"); break; }
                            case "Integer": { T = Type.GetType("System.Int32"); break; }
                            case "String": { T = Type.GetType("System.String"); break; }
                            case "Structure": { T = Type.GetType("System.String"); break; }
                            case "Time": { T = Type.GetType("System.String"); break; }
                            default: { T = Type.GetType("System.String"); break; }
                        } // switch (strFormat)
                        oDataColumn.DataType = T;
                        oDataColumn.ExtendedProperties.Add("Format", strFormat);
                        oDataTable.Columns.Add(oDataColumn);
                        // Build picklist dictionaries
                        {
                            XmlNode oXmlNodePicklist = oXmlNode.SelectSingleNode("descendant::picklist");
                            if (oXmlNodePicklist != null)
                            {
                                XmlNodeList oXmlNodeListItem = oXmlNodePicklist.SelectNodes("descendant::item");
                                if (oXmlNodeListItem != null)
                                {
                                    Dictionary<string, Int32> dictPicklist = new Dictionary<string, int>();
                                    _dictPicklist.Add(fldCaption, dictPicklist);
                                    foreach (XmlNode oXmlNodeItem in oXmlNodeListItem)
                                    {
                                        if (!dictPicklist.ContainsKey(oXmlNodeItem.InnerText.Trim()))
                                        {
                                            dictPicklist.Add(oXmlNodeItem.InnerText.Trim(), Convert.ToInt32(oXmlNodeItem.Attributes["value"].Value.ToString()));
                                        }
                                        else
                                        {
                                            string strPickField = fldCaption.Replace(strFormName + ":", "");
                                            string strErrMsg = "Value " + oXmlNodeItem.InnerText.Trim() + " is duplicated in " + strPickField +" list. Please remove the duplicate value to proceed." ;
                                            AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Mapping, 0, strErrMsg, oXmlNodeItem.InnerText.Trim(), oXmlNodeItem.Attributes["value"].Value.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                { // Handle fields with a value attribute
                    // Convert mappings to dictionaries for quick access
                    XmlDocument oXmlDocument = new XmlDocument();
                    oXmlDocument.LoadXml(Mappings);
                    XmlNode oXmlNodeRoot = oXmlDocument.DocumentElement;
                    {
                        XmlNodeList oXmlNodeList = oXmlNodeRoot.SelectNodes("descendant::field[attribute::source='none']");
                        foreach (XmlNode oXmlNode in oXmlNodeList)
                        {
                            string strName = oXmlNode.Attributes["name"].Value;
                            {
                                string strFormName = oXmlNode.ParentNode.Attributes["name"].Value;
                                if (!string.IsNullOrEmpty(strFormName))
                                {
                                    strName = strFormName + ":" + strName;
                                }
                            }
                            OutputDataSet.Tables[0].Columns.Remove(strName);
                        }
                    }
                    {
                        XmlNodeList oXmlNodeList = oXmlNodeRoot.SelectNodes("descendant::field[attribute::source='picklist']");
                        foreach (XmlNode oXmlNode in oXmlNodeList)
                        {
                            string strName = oXmlNode.Attributes["name"].Value;
                            {
                                string strFormName = oXmlNode.ParentNode.Attributes["name"].Value;
                                if (!string.IsNullOrEmpty(strFormName))
                                {
                                    strName = strFormName + ":" + strName;
                                }
                            }
                            _dictPicklist.Remove(strName);  // Not needed, already chosen from picklist in GUI
                        }
                    }
                    {
                        XmlNodeList oXmlNodeList = oXmlNodeRoot.SelectNodes("descendant::field[attribute::value]");
                        foreach (XmlNode oXmlNode in oXmlNodeList)
                        {
                            string strName = oXmlNode.Attributes["name"].Value;
                            {
                                string strFormName = oXmlNode.ParentNode.Attributes["name"].Value;
                                if (!string.IsNullOrEmpty(strFormName))
                                {
                                    strName = strFormName + ":" + strName;
                                }
                            }
                            if (OutputDataSet.Tables[0].Columns.Contains(strName) == false)
                            {
                                AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Output, 0, "No such output column as {0:G}", strName);
                                continue;
                            }
                            string strSource = oXmlNode.Attributes["source"].Value;
                            string strValue = oXmlNode.Attributes["value"].Value;
                            if (strSource == "calculation")
                            {
                                if (_oParser == null)
                                {
                                    _oParser = new CalculationParser();
                                    _oParser.TypeAddFunctions(Type.GetType("System.Boolean"));
                                    _oParser.TypeAddFunctions(Type.GetType("System.Char"));
                                    _oParser.TypeAddFunctions(Type.GetType("System.Convert"));
                                    _oParser.TypeAddFunctions(Type.GetType("System.DateTime"));
                                    _oParser.TypeAddFunctions(Type.GetType("System.Double"));
                                    _oParser.TypeAddFunctions(Type.GetType("System.Int32"));
                                    _oParser.TypeAddFunctions(Type.GetType("System.Math"));
                                    _oParser.TypeAddFunctions(Type.GetType("System.String"));
                                    //_oParser.ObjectAddFunctions(new MyFunctions());    // Add functions exposed by OutputObject
                                    DataViewManager dvm = new DataViewManager(voDataSet);
                                    _oParser.DataViewManagerAddFields(dvm);
                                }
                                bool bFailed = _oParser.CalculationAdd(strName, string.Empty, strValue); // Will eventually need a string anyway
                                if (bFailed)
                                {
                                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Calculation, 0, _fmtParserFormula_1, strValue);
                                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Calculation, 0, _fmtParserError_2, _oParser.Error, _oParser.ErrorColumn.ToString());
                                    break;
                                }
                                _dictMapped.Add(strName, "=" + strName);    // '=' tells us it is a calculation as opposed to a direct mapping
                            }
                            else if (strSource == "map")
                            {
                                //int nOrdinal = _dictMapped.Count;
                                //_dictMapped.Add(fldCaption, nOrdinal);
                                _dictMapped.Add(strName, strValue);
                            }
                            else
                            {
                                _dictConstant.Add(strName, strValue);
                                if (_dictPicklist.ContainsKey(strName))
                                {
                                    if (_dictPicklist[strName].ContainsKey(strValue) == false)
                                    {
                                        AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Mapping, 0, _fmtPicklistError_1, strValue, strName);
                                        continue;
                                    }
                                    strValue = _dictPicklist[strName][strValue].ToString();
                                    _dictPicklist.Remove(strName);  // No longer needed, single use for constant
                                }
                                OutputDataSet.Tables[0].Columns[strName].DefaultValue = strValue;
                            }
                        } // foreach (XmlNode oXmlNode in oXmlNodeList)
                    }
                }
                if (HasMessages)
                {
                    break;
                }
                StartWrite();
            } while (false);
            return HasMessages;
        } // StartWrite()

        /// <summary>
        /// Tests an object to see if it's either NULL or System.DBNull.Value
        /// </summary>
        /// <param name="value">the object to evaluate</param>
        /// <returns>true if the object is either null or System.DBNull.Value</returns>
        public bool IsNullValue(object value)
        {
            bool isNull = false;
            isNull = (value == null || value == System.DBNull.Value);
            return isNull;
        }

        #endregion

        #region > Log Messages <
        
        /// <summary>
        /// Append a message to the message list
        /// </summary>
        /// <param name="veLogSeverity"></param>
        /// <param name="veLogSource"></param>
        /// <param name="vnTransaction"></param>
        /// <param name="vstrLogFormat"></param>
        /// <param name="vParams"></param>
        protected void AddMessage(
            LogMessage.LogSeverity veLogSeverity
            , LogMessage.LogSource veLogSource
            , int vnTransaction
            , string vstrLogFormat
            , params string[] vParams
        )
        {
            _LogMessageList.Add(veLogSeverity, veLogSource, vnTransaction, vstrLogFormat, vParams);
            return;
        }

        /// <summary>
        /// Clear the message list
        /// </summary>
        protected void ClearMessages()
        {
            _LogMessageList.Clear();
            return;
        }

        /// <summary>
        /// Return true if there are any errors reported in the message list
        /// </summary>
        public bool HasMessages
        {
            get { return (_LogMessageList.Count > 0); }
        }

        /// <summary>
        /// The message list (if any)
        /// </summary>
        public LogMessageList MessageList
        {
            get { return _LogMessageList; }
        }

        #endregion

        #region > abstract declarations <

        /// <summary>
        /// Called to persist data
        /// </summary>
        /// <returns></returns>
        protected abstract bool DataSetWrite(int vnRecord);

        /// <summary>
        /// Called after <see cref="DataSetWrite(int)"/> has persisted data
        /// </summary>
        /// <returns></returns>
        public abstract bool EndWrite();

        /// <summary>
        /// Called before <see cref="DataSetWrite(int)"/> to prepare to persist data
        /// </summary>
        /// <returns></returns>
        protected abstract bool StartWrite();

        #endregion

    }
}
