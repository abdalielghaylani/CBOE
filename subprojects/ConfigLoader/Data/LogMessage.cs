using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.ConfigLoader.Data
{
    /// <summary>
    /// A loggable message
    /// </summary>
    public class LogMessage
    {
        #region enums and types
        /// <summary>
        /// Severity of the LogMessage
        /// </summary>
        public enum LogSeverity
        {
            /// <summary>
            /// Critical (fatal)
            /// </summary>
            Critical,
            /// <summary>
            /// Error (data not processed)
            /// </summary>
            Error,
            /// <summary>
            /// Warning (data processed)
            /// </summary>
            Warning,
            /// <summary>
            /// Informational (not an error)
            /// </summary>
            Information
        };
        /// <summary>
        /// Source of the LogMessage
        /// </summary>
        public enum LogSource
        {
            /// <summary>
            /// Unknown
            /// </summary>
            Unknown,
            /// <summary>
            /// Job
            /// </summary>
            Job,
            /// <summary>
            /// Input (InputObject)
            /// </summary>
            Input,
            /// <summary>
            /// Calculation (OutputObject base class)
            /// </summary>
            Calculation,
            /// <summary>
            /// Mapping (OutputObject base class)
            /// </summary>
            Mapping,
            /// <summary>
            /// Output (OutputObject)
            /// </summary>
            Output
        };
        #endregion

        #region data
        private LogSeverity _eLogSeverity;
        private LogSource _eLogSource;
        private int _nTransaction;
        private int _nFormat;
        private string _strParams;
        //private string _strLogMessage;
        static private List<string> _LogFormats = null;
        #endregion

        #region properties

        /// <summary>
        /// Get the FormatNumber index
        /// </summary>
        static public int FormatCount
        {
            get
            {
                return FormatList.Count;
            }
        } // FormatCount

        /// <summary>
        /// Get the FormatNumber index
        /// </summary>
        public int FormatNumber
        {
            get
            {
                return _nFormat;
            }
        } // FormatNumber

        /// <summary>
        /// Get the list of Formats
        /// </summary>
        static protected List<string> FormatList
        {
            get
            {
                return _LogFormats;
            }
        } // FormatList

        /// <summary>
        /// Get the message text
        /// </summary>
        public string Message
        {
            get
            {
                return String.Format(FormatList[FormatNumber], Params.Split('\f'));
            }
        } // Message

        /// <summary>
        /// Get the Params
        /// </summary>
        public string Params
        {
            get
            {
                return _strParams;
            }
        } // Params

        /// <summary>
        /// Get the severity
        /// </summary>
        public LogSeverity Severity
        {
            get
            {
                return _eLogSeverity;
            }
        } // Severity

        /// <summary>
        /// Get the source
        /// </summary>
        public LogSource Source
        {
            get
            {
                return _eLogSource;
            }
        } // Source

        /// <summary>
        /// Get the transaction number
        /// </summary>
        public int Transaction
        {
            get
            {
                return _nTransaction;
            }
        } // Transaction
        #endregion

        #region constructors
        /// <summary>
        /// Construct a loggable message
        /// </summary>
        /// <param name="veLogSeverity"></param>
        /// <param name="veLogSource"></param>
        /// <param name="vnTransaction"></param>
        /// <param name="vstrLogMessage"></param>
        public LogMessage(LogSeverity veLogSeverity, LogSource veLogSource, int vnTransaction, string vstrLogMessage)
        {
            _eLogSeverity = veLogSeverity;
            _eLogSource = veLogSource;
            _nTransaction = vnTransaction;
            _nFormat = AddFormat("{0:G}");
            _strParams = vstrLogMessage;
            return;
        } // LogMessage()

        /// <summary>
        /// Construct a loggable message
        /// </summary>
        /// <param name="veLogSeverity"></param>
        /// <param name="veLogSource"></param>
        /// <param name="vnTransaction"></param>
        /// <param name="vstrLogFormat"></param>
        /// <param name="vParams"></param>
        public LogMessage(LogSeverity veLogSeverity, LogSource veLogSource, int vnTransaction, string vstrLogFormat, params string[] vParams)
        {
            _eLogSeverity = veLogSeverity;
            _eLogSource = veLogSource;
            _nTransaction = vnTransaction;
            _nFormat = AddFormat(vstrLogFormat);
            if (vstrLogFormat.StartsWith("XML", StringComparison.OrdinalIgnoreCase))
            {
                //Don't store the entire xml, just the name and 'id' attribute of the root node
                if (vParams.Length == 1)
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(vParams[0]);
                        XmlNode docElement = doc.DocumentElement;
                        string id = string.Empty;
                        string name = docElement.Name;
                        XmlAttribute idAttrib = docElement.Attributes["id"];
                        if (idAttrib == null)
                            id = "{unknown id}";
                        else
                            id = idAttrib.Value;

                        _strParams = string.Format("<{0} id='{1}'/>", name, id);
                    }
                    catch (Exception ex)
                    {
                        //ignore
                    }
                }
            }
            else
                _strParams = String.Join("\f", vParams);

        } // LogMessage()
        #endregion

        private int AddFormat(string vstrLogFormat)
        {
            if (_LogFormats == null)
            {
                _LogFormats = new List<string>();
                _LogFormats.Add("{0:G}");    // always 0th entry
            }
            if (_LogFormats.Contains(vstrLogFormat) == false)
            {
                _LogFormats.Add(vstrLogFormat);
            }
            return _LogFormats.IndexOf(vstrLogFormat);
        } // AddFormat()

        /// <summary>
        /// Close
        /// </summary>
        static public void Close()
        {
            FormatList.Clear();
            return;
        } // Close()

        /// <summary>
        /// Return the requested format
        /// </summary>
        /// <param name="nFormat"></param>
        /// <returns></returns>
        static public string Format(int nFormat)
        {
            return FormatList[nFormat];
        } // Format()

    } // class LogMessage
}
