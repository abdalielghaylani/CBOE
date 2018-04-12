using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using CambridgeSoft.COE.ConfigLoader.Windows.Common;
using CambridgeSoft.COE.ConfigLoader.Windows.Controls;

namespace CambridgeSoft.COE.ConfigLoader.Data.OutputObjects
{
    /// <summary>
    /// OutputObject base class
    /// </summary>
    public abstract class OutputObject
    {
        /// <summary>
        /// To report exceptions
        /// </summary>
        protected const string _fmtException_2 = "Exception during {0:G} message is {1:G}";
        /// <summary>
        /// To report exception XML
        /// </summary>
        protected const string _fmtExceptionXML_1 = "XML\n{0:G}";
        private const string _fmtParserFormula_1 = "Invalid formula: {0:G}";

        #region property data
        private bool _bIsValid;
        private string _xmlConfiguration = "";  // OutputConfiguration (get / set later)
        private string _strDb;
        private string _strFilter = ""; // For those that can output to a file
        private string _xmlMappings;    // InputOutputMapper (set)
        private DataSet _oOutputDataSet = null;
        private string _xmlOutputFieldSpec;
        private string _strOutputType;    // OutputTypeChooser (get)
        private COEProgressHelper _oPh = new COEProgressHelper();
        #endregion

        #region properties
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
        } // IsValid
        /// <summary>
        /// Used for optional output object configuration
        /// </summary>
        public virtual string Configuration
        {
            get
            {
                if (_xmlConfiguration.Length > 0)
                {
                    _xmlConfiguration = UIBase.FormatXmlString(COEXmlBind.LoadValues(_xmlConfiguration, "member", "value", this));
                }
                return _xmlConfiguration;
            }
            set
            {
                _xmlConfiguration = (value != null) ? value : "";
                if (_xmlConfiguration.Length > 0)
                {
                    COEXmlBind.StoreValues(_xmlConfiguration, "member", "value", this);
                }
                return;
            } // set
        } // Configuration

        /// <summary>
        /// The output object database
        /// </summary>
        public string Db
        {
            get
            {
                return (_strDb == null) ? "" : _strDb;
            }
            set
            {
                _strDb = value;    // WJC error if already open
                return;
            }
        } // Db

        /// <summary>
        /// The output object file filter
        /// </summary>
        public virtual string Filter
        {
            get
            {
                return (_strFilter == null) ? "" : _strFilter;
            }
            protected set
            {
                _strFilter = value;
                return;
            }
        } // Filter

        /// <summary>
        /// Build the output field specification based on an input field specification
        /// </summary>
        public virtual string InputFieldSpec
        {
            set
            {
                return;
            }
        } // InputFieldSpec

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
        } // Mappings

        /// <summary>
        /// List of all potentially available output objects
        /// </summary>
        static public List<OutputObject> ObjectList
        {
            get
            {
                List<OutputObject> listObjectList = new List<OutputObject>();
                OutputObject oOutputObject;
#if UNUSED
                {
                    oOutputObject = new OutputObjectReg();
                    if (oOutputObject.IsValid) listObjectList.Add(oOutputObject);
                }

                {
                    oOutputObject = new OutputObjectRegBatch();
                    if (oOutputObject.IsValid) listObjectList.Add(oOutputObject);
                }
#endif

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
#if UNUSED
                {
                    oOutputObject = new OutputObjectTxt();
                    if (oOutputObject.IsValid) listObjectList.Add(oOutputObject);
                }
#endif
                {
                    oOutputObject = new OutputObjectConfigImp();
                    if (oOutputObject.IsValid) listObjectList.Add(oOutputObject);
                }
                {
                    oOutputObject = new OutputObjectConfigExp();
                    if (oOutputObject.IsValid) listObjectList.Add(oOutputObject);
                }
                return listObjectList;
            }
        } // ObjectList

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

        #region data

        private Dictionary<string, string> _dictConstant = null;
        private Dictionary<string, string> _dictMapped = null;

        #endregion

        #region methods
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
            DataTable oOutputDataTable = OutputDataSet.Tables[0];
            DataTable oInputDataTable = voInputDataSet.Tables[0];

            int cInputRows = oInputDataTable.Rows.Count;
            int nTransaction = vnRecord;
            foreach (DataRow oInputDataRow in oInputDataTable.Rows)
            {
                nTransaction++;
                DataRow oOutputDataRow = oOutputDataTable.NewRow();
                int cErrors = 0;
                foreach (KeyValuePair<string, string> kvp in _dictMapped)
                {
                    // Source of the value
                    Object oValue;
                    oValue = oInputDataRow[kvp.Value];

                    if (oValue == null) oValue = "";    // WJC TODO actually if DbNull is true then we should let it through
                    // Store
                    if (oValue != null)
                    {
                        if ((oOutputDataTable.Columns[kvp.Key].DataType.FullName == "System.Byte[]") && (oValue.GetType().FullName != "System.Byte[]"))
                        {
                            try
                            {
                                oOutputDataRow[kvp.Key] = Convert.FromBase64String(oValue.ToString());
                            }
                            catch (Exception ex)
                            {
                                AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Mapping, nTransaction, _fmtException_2, "Convert.FromBase64String", ex.Message);
                                cErrors++;
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
                                {
                                    strValue = strValue.Substring(indexOf).Trim();
                                }
                                try
                                {
                                    oOutputDataRow[kvp.Key] = strValue;
                                }
                                catch (Exception ex)
                                {
                                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Mapping, nTransaction, _fmtException_2, "assignment to output", ex.Message);
                                    cErrors++;
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
                                    AddMessage(LogMessage.LogSeverity.Error, LogMessage.LogSource.Mapping, nTransaction, _fmtException_2, "assignment to output", ex.Message);
                                    cErrors++;
                                }
                            }
                        }
                    }
                }
                if (cErrors > 0)
                {
                    oOutputDataRow.RowError = cErrors.ToString();
                }
                oOutputDataTable.Rows.Add(oOutputDataRow);
            }
            oInputDataTable.Rows.Clear();
            DataSetWrite(vnRecord);
            oOutputDataTable.Rows.Clear();
            oOutputDataTable.AcceptChanges();

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
            return _dictMapped.ContainsKey(vstrOutputField);
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
            do
            {
                { // Build output dataset
                    // Convert mappings to dictionaries for quick access
                    XmlDocument oXmlDocument = new XmlDocument();
                    oXmlDocument.LoadXml(OutputFieldSpec);
                    XmlNode oXmlNodeRoot = oXmlDocument.DocumentElement;
                    OutputDataSet = new DataSet();
                    DataTable oDataTable = new DataTable("OutputTable");   // Property ?
                    OutputDataSet.Tables.Add(oDataTable);
                    XmlNodeList oXmlNodeList = oXmlNodeRoot.SelectNodes("descendant::field");
                    foreach (XmlNode oXmlNode in oXmlNodeList)
                    {
                        string strName = oXmlNode.Attributes["name"].Value;
                        string strFormat = oXmlNode.Attributes["format"].Value;
                        DataColumn oDataColumn = new DataColumn();
                        oDataColumn.ColumnName = strName;
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
                        }
                        oDataColumn.DataType = T;
                        oDataColumn.ExtendedProperties.Add("Format", strFormat);
                        oDataTable.Columns.Add(oDataColumn);
                    }
                }
                { // Handle fields with a value attribute
                    // Convert mappings to dictionaries for quick access
                    XmlDocument oXmlDocument = new XmlDocument();
                    oXmlDocument.LoadXml(Mappings);
                    XmlNode oXmlNodeRoot = oXmlDocument.DocumentElement;
                    {
                        XmlNodeList oXmlNodeList = oXmlNodeRoot.SelectNodes("child::field[attribute::source='none']");
                        foreach (XmlNode oXmlNode in oXmlNodeList)
                        {
                            string strName = oXmlNode.Attributes["name"].Value;
                            OutputDataSet.Tables[0].Columns.Remove(strName);
                        } // foreach (XmlNode oXmlNode in oXmlNodeList)
                    }
                    {
                        XmlNodeList oXmlNodeList = oXmlNodeRoot.SelectNodes("child::field[attribute::value]");
                        foreach (XmlNode oXmlNode in oXmlNodeList)
                        {
                            string strName = oXmlNode.Attributes["name"].Value;
                            if (OutputDataSet.Tables[0].Columns.Contains(strName) == false)
                            {
                                AddMessage(LogMessage.LogSeverity.Critical, LogMessage.LogSource.Output, 0, "No such output column as {0:G}", strName);
                                continue;
                            }
                            string strSource = oXmlNode.Attributes["source"].Value;
                            string strValue = oXmlNode.Attributes["value"].Value;

                            if (strSource == "map")
                                _dictMapped.Add(strName, strValue);
                            else
                            {
                                _dictConstant.Add(strName, strValue);
                                OutputDataSet.Tables[0].Columns[strName].DefaultValue = strValue;
                            }
                        }
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

        #endregion

        #region LogMessageList
        private LogMessageList _LogMessageList = new LogMessageList();
        /// <summary>
        /// Append a message to the message list
        /// </summary>
        /// <param name="veLogSeverity"></param>
        /// <param name="veLogSource"></param>
        /// <param name="vnTransaction"></param>
        /// <param name="vstrLogFormat"></param>
        /// <param name="vParams"></param>
        protected void AddMessage(LogMessage.LogSeverity veLogSeverity, LogMessage.LogSource veLogSource, int vnTransaction, string vstrLogFormat, params string[] vParams)
        {
            _LogMessageList.Add(veLogSeverity, veLogSource, vnTransaction, vstrLogFormat, vParams);
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

        #region abstract declarations
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

    } // class OutputObject
}