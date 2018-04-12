using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace CambridgeSoft.COE.DataLoader.Data
{
    /// <summary>
    /// Holds a list of messages to be logged or displayed depending on the context
    /// </summary>
    public class LogMessageList
    {
        #region data
        private List<LogMessage> _listLogMessageList = new List<LogMessage>();
        #endregion

        #region enumerator
        /// <summary>
        /// So foreach will work on this class
        /// </summary>
        /// <returns></returns>
        public IEnumerator<LogMessage> GetEnumerator()
        {
            foreach (LogMessage lm in _listLogMessageList)
            {
                yield return lm;
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// Get the number of LogMessages in the list
        /// </summary>
        public int Count
        {
            get
            {
                return _listLogMessageList.Count;
            }
        } // Count

        /// <summary>
        /// Get count of unique transactions of severity Error or above
        /// </summary>
        public int ErrorTransactions
        {
            get
            {
                Dictionary<int, int> dictErrorTransactions = new Dictionary<int, int>();
                foreach (LogMessage logMessage in _listLogMessageList)
                {
                    if (
                        (logMessage.Severity == LogMessage.LogSeverity.Warning)
                        || (logMessage.Severity == LogMessage.LogSeverity.Error)
                        || (logMessage.Severity == LogMessage.LogSeverity.Critical)
                    ) 
                    {
                        int nTransaction = logMessage.Transaction;
                        if (dictErrorTransactions.ContainsKey(nTransaction) == false)
                        {
                            dictErrorTransactions.Add(nTransaction, 0);
                        }
                        dictErrorTransactions[nTransaction]++;
                    }
                }
                return dictErrorTransactions.Count;
            }
        } // ErrorTransactions

        #endregion

        /// <summary>
        /// Add a LogMessage to the list
        /// </summary>
        /// <param name="veLogSeverity"></param>
        /// <param name="veLogSource"></param>
        /// <param name="vnTransaction"></param>
        /// <param name="vstrLogMessage"></param>
        public void Add(
            LogMessage.LogSeverity veLogSeverity
            , LogMessage.LogSource veLogSource
            , int vnTransaction
            , string vstrLogMessage
            )
        {
            _listLogMessageList.Add(new LogMessage(veLogSeverity, veLogSource, vnTransaction, vstrLogMessage));
        }

        /// <summary>
        /// Add a LogMessage to the list
        /// </summary>
        /// <param name="veLogSeverity"></param>
        /// <param name="veLogSource"></param>
        /// <param name="vnTransaction"></param>
        /// <param name="vstrLogFormat"></param>
        /// <param name="vParams"></param>
        public void Add(
            LogMessage.LogSeverity veLogSeverity
            , LogMessage.LogSource veLogSource
            , int vnTransaction
            , string vstrLogFormat
            , params string[] vParams
            )
        {
            _listLogMessageList.Add(new LogMessage(veLogSeverity, veLogSource, vnTransaction, vstrLogFormat, vParams));
        }

        /// <summary>
        /// Add a list of LogMessages to the list
        /// </summary>
        /// <param name="vlistLogMessageList"></param>
        public void Add(LogMessageList vlistLogMessageList)
        {
            foreach (LogMessage logMessage in vlistLogMessageList)
            {
                _listLogMessageList.Add(logMessage);
            }
            return;
        } // Add()

        /// <summary>
        /// Clear the list of LogMessages
        /// </summary>
        public void Clear()
        {
            _listLogMessageList.Clear();
            return;
        } // Clear()

        /// <summary>
        /// Close LogMessage
        /// </summary>
        public void Close()
        {
            _listLogMessageList.Clear();
            LogMessage.Close();
            nSavedFormats = 0;
            return;
        } // Close()

        private string Encode(LogMessage vlogMessage)
        {
            string strRet = string.Empty;
            switch (vlogMessage.Severity)
            {
                case LogMessage.LogSeverity.Critical: { strRet += "C"; break; }
                case LogMessage.LogSeverity.Error: { strRet += "E"; break; }
                case LogMessage.LogSeverity.Information: { strRet += "I"; break; }
                case LogMessage.LogSeverity.Warning: { strRet += "W"; break; }
                default: { strRet += "?"; break; }
            } // switch (Severity)
            switch (vlogMessage.Source)
            {
                case LogMessage.LogSource.Calculation: { strRet += "C"; break; }
                case LogMessage.LogSource.Input: { strRet += "I"; break; }
                case LogMessage.LogSource.Job: { strRet += "J"; break; }
                case LogMessage.LogSource.Mapping: { strRet += "M"; break; }
                case LogMessage.LogSource.Output: { strRet += "O"; break; }
                case LogMessage.LogSource.Unknown: { strRet += "U"; break; }
                default: { strRet += "?"; break; }
            } // switch (Source)
            strRet += vlogMessage.Transaction.ToString();
            strRet += "\t";
            string strFullMessage = vlogMessage.FormatNumber.ToString() + "#:--#" + vlogMessage.Params;
            strRet += strFullMessage.Length.ToString();
            strRet += "\r\n";
            strRet += strFullMessage;
            return strRet;
        } // Encode()

        private string Encode(string vstrFormat)
        {
            string strRet = string.Empty;
            strRet += "x"; // Severity
            strRet += "x"; // Source
            strRet += "-2"; // Transaction
            strRet += "\t";
            strRet += vstrFormat.Length.ToString();
            strRet += "\r\n";
            strRet += vstrFormat;
            return strRet;
        } // Encode()

#if UNUSED        
        /// <summary>
        /// Fill list of Log messages from a file
        /// </summary>
        /// <param name="voFileStream"></param>
        public void Load(FileStream voFileStream)
        {
            voFileStream.Seek(0, SeekOrigin.Begin);
            string[] strLine;
            while (voFileStream.Position < voFileStream.Length)
            {
                {
                    byte[] byBuffer = new byte[16];
                    voFileStream.Read(byBuffer, 0, 16);
                    byBuffer = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), byBuffer, 0, 16 - 0);
                    strLine = Encoding.UTF8.GetString(byBuffer).Split(new string[] {"\r\n"}, StringSplitOptions.None);
                }
                LogMessage.LogSeverity eLogSeverity = LogMessage.LogSeverity.Critical;
                switch (strLine[0][0])
                {
                    case 'C': { eLogSeverity = LogMessage.LogSeverity.Critical; break; }
                    case 'E': { eLogSeverity = LogMessage.LogSeverity.Error; break; }
                    case 'I': { eLogSeverity = LogMessage.LogSeverity.Information; break; }
                    case 'W': { eLogSeverity = LogMessage.LogSeverity.Warning; break; }
                }
                LogMessage.LogSource eLogSource = LogMessage.LogSource.Unknown;
                switch (strLine[0][1])
                {
                    case 'C': { eLogSource = LogMessage.LogSource.Calculation; break; }
                    case 'I': { eLogSource = LogMessage.LogSource.Input; break; }
                    case 'J': { eLogSource = LogMessage.LogSource.Job; break; }
                    case 'M': { eLogSource = LogMessage.LogSource.Mapping; break; }
                    case 'O': { eLogSource = LogMessage.LogSource.Output; break; }
                    case 'U': { eLogSource = LogMessage.LogSource.Unknown; break; }
                }
                string[] n = strLine[0].Substring(2).Split('\t');
                int nTransaction = Int32.Parse(n[0]);
                int nLength = Int32.Parse(n[1]);
                int nRht = nLength - strLine[1].Length;
                if (strLine[1][0] == 'E')
                {
                    string strLogMessage;
                    if (nRht > 0)
                    {
                        byte[] byBuffer = new byte[nRht];
                        voFileStream.Read(byBuffer, 0, nRht);
                        voFileStream.Seek(2, SeekOrigin.Current);
                        byBuffer = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), byBuffer, 0, nRht - 0);
                        strLogMessage = strLine[1] + Encoding.UTF8.GetString(byBuffer);
                    }
                    else
                    {
                        voFileStream.Seek(-nRht, SeekOrigin.Current);
                        strLogMessage = strLine[1].Substring(0, strLine[1].Length + nRht);
                    }
                    Add(eLogSeverity, eLogSource, nTransaction, strLogMessage);
                }
                else
                {
                    voFileStream.Seek(nRht + 2, SeekOrigin.Current);
                }
            } // while (voStreamReader.EndOfStream == false)
            return;
        } // Load()
#endif

        /// <summary>
        /// Load Saved file into a DataSet
        /// </summary>
        /// <param name="voFileStream"></param>
        /// <param name="vbJob"></param>
        /// <param name="vbSeverity"></param>
        /// <param name="rcSeverity"></param>
        /// <returns></returns>
        public static DataSet MakeDataSet(FileStream voFileStream, bool vbJob, bool[] vbSeverity, ref int[] rcSeverity)
        {
            int nTransaction = 0;
            char chSeverity = '\0';
            char chSource = '\0';
            string strMessage = string.Empty;

            DataSet oDataSet = new DataSet("LogMessageDataSet");
            DataTable oDataTable = new DataTable("LogMessageTable");

            DataColumn colTransaction = new DataColumn("Transaction", nTransaction.GetType());
            oDataTable.Columns.Add(colTransaction);

            DataColumn colOutcome = new DataColumn("Outcome", typeof(char));
            rcSeverity = new int[3];
            oDataTable.Columns.Add(colOutcome);

            DataColumn colSource = new DataColumn("Source", chSource.GetType());
            oDataTable.Columns.Add(colSource);

            DataColumn colMessage = new DataColumn("Message", strMessage.GetType());
            oDataTable.Columns.Add(colMessage);

            DataColumn colXml = new DataColumn("Xml", strMessage.GetType());
            oDataTable.Columns.Add(colXml);

            oDataSet.Tables.Add(oDataTable);
            List<string> FormatList = new List<string>();
            voFileStream.Seek(0, SeekOrigin.Begin);
            {
                byte[] version = new byte[6];
                voFileStream.Read(version, 0, 6);
                if (new string(ASCIIEncoding.ASCII.GetChars(version)) != "v100\r\n")
                {
                    DataRow oDataRow = oDataTable.NewRow();
                    oDataRow[colOutcome] = 'C';
                    oDataRow[colSource] = 'U';
                    oDataRow[colTransaction] = -1;
                    oDataRow[colMessage] = "The log file format is not recognized";
                    oDataRow[colXml] = string.Empty;
                    oDataTable.Rows.Add(oDataRow);
                    return oDataSet;
                }
            }
            string[] strLine;
            while (voFileStream.Position < voFileStream.Length)
            {
                {
                    byte[] byBuffer = new byte[16];
                    int cBytes = voFileStream.Read(byBuffer, 0, 16);
                    if (cBytes <= 2) continue;
                    byBuffer = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), byBuffer, 0, 16 - 0);
                    string lineText = Encoding.UTF8.GetString(byBuffer);

                    //when line begins with a CRLF (sporadic), it must be removed or the array reads will fail
                    if (lineText.StartsWith("\r\n"))
                    {
                        lineText = lineText.Substring(2);
                    }
                    strLine = lineText.Split(new string[] { "\r\n" }, 2, StringSplitOptions.None);
                }
                chSeverity = strLine[0][0];
                chSource = strLine[0][1];
                string[] n = strLine[0].Substring(2).Split('\t');
                nTransaction = Int32.Parse(n[0]);
                int nLength = Int32.Parse(n[1]);
                int nRht = nLength - strLine[1].Length;
                bool bOK = vbJob;
                {
                    if (nTransaction >= 0)
                    {
                        int nSeverity = 0;
                        switch (chSeverity)
                        {
                            case 'C':   // Treated like an Error
                            case 'E': { nSeverity = 0; break; }
                            case 'W': { nSeverity = 1; break; }
                            case 'I': { nSeverity = 2; break; }
                        }
                        rcSeverity[nSeverity]++;
                        bOK = vbSeverity[nSeverity];
                    }
                }
                if (bOK || (nTransaction < -1))
                {
                    {
                        string strFullMessage;
                        if (nRht > 0)
                        {
                            byte[] byBuffer = new byte[nRht];
                            voFileStream.Read(byBuffer, 0, nRht);
                            voFileStream.Seek(2, SeekOrigin.Current);
                            byBuffer = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), byBuffer, 0, nRht - 0);
                            strFullMessage = strLine[1] + Encoding.UTF8.GetString(byBuffer);
                        }
                        else
                        {
                            voFileStream.Seek(2 + nRht, SeekOrigin.Current);    // 2 for \r\n
                            strFullMessage = strLine[1].Substring(0, strLine[1].Length + nRht);
                        }
                        if (nTransaction >= -1)
                        {
                            string[] strSplitNumber = strFullMessage.Split(new string[] { "#:--#" },StringSplitOptions.RemoveEmptyEntries);
                            int nFormat = Convert.ToInt32(strSplitNumber[0]);
                            string[] strParams = strSplitNumber[1].Split('\f');
                            strMessage = String.Format(FormatList[nFormat], strParams);
                        }
                        else
                        {
                            strMessage = strFullMessage;    // Format record
                        }
                    }

                    string xmlThatFailed = string.Empty;
                    strMessage = ScrubExceptionDetails(strMessage, out xmlThatFailed);

                    if (nTransaction >= -1)
                    {
                        DataRow oDataRow = oDataTable.NewRow();
                        oDataRow[colOutcome] = chSeverity;
                        oDataRow[colSource] = chSource;
                        oDataRow[colTransaction] = nTransaction;
                        oDataRow[colMessage] = strMessage.TrimEnd();
                        oDataRow[colXml] = xmlThatFailed;
                        oDataTable.Rows.Add(oDataRow);
                    }
                    else
                    {
                        FormatList.Add(strMessage); // Store format
                    }
                }
                else
                {
                    voFileStream.Seek(nRht + 2, SeekOrigin.Current);
                }
            }
            return oDataSet;
        }

        static int nSavedFormats = 0;
        /// <summary>
        /// Saves (appends) the current list of Log messages and clears the list
        /// </summary>
        /// <param name="voStreamWriter"></param>
        public void Save(StreamWriter voStreamWriter)
        {
            if (voStreamWriter.BaseStream.Position == 0)
            {
                voStreamWriter.WriteLine("v100");
            }
            for (int nFormat = nSavedFormats; nFormat < LogMessage.FormatCount; nFormat++)
            {
                voStreamWriter.WriteLine(Encode(LogMessage.Format(nFormat)));
            }
            nSavedFormats = LogMessage.FormatCount;
            foreach (LogMessage logMessage in _listLogMessageList)
            {
                voStreamWriter.WriteLine(Encode(logMessage));
            }
            voStreamWriter.Flush();
            Clear();
            return;
        } // Save()

        /// <summary>
        /// Construct an array of the Message text
        /// </summary>
        /// <returns></returns>
        public string[] ToMessageArray()
        {
            string[] retArray = new string[_listLogMessageList.Count];
            int n = 0;
            foreach (LogMessage logMessage in _listLogMessageList)
            {
                retArray[n++] = logMessage.Message;
            }
            return retArray;
        } // ToArray()

        /// <summary>
        /// If the message is a stack-trace, extract the 'message' part of the exception and return it.
        /// </summary>
        /// <remarks>JED: This is a temporary fix to keep the display lean.</remarks>
        /// <param name="originalMessage">Contains xml which is failed to register</param>
        /// <returns></returns>
        private static string ScrubExceptionDetails(string originalMessage, out string xmlThatFailed)
        {
            string newMessage = originalMessage;
            xmlThatFailed = string.Empty;

            if (originalMessage.Contains("Exception"))
            {
                string[] messageSeparators = new string[] { "Record xml is: ", " at " };
                string[] exceptionMessages = originalMessage.Split(messageSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (exceptionMessages.Length > 1)
                {
                    newMessage = exceptionMessages[0];
                    string msgWithXml = exceptionMessages[exceptionMessages.Length - 1];
                    int startXml = msgWithXml.IndexOf("<MultiCompoundRegistryRecord");
                    if (startXml >= 0)
                    {
                        xmlThatFailed = msgWithXml.Substring(startXml);
                    }
                    else
                    {
                        newMessage = originalMessage;//  Exception during {0:G}: \nMessage: {1:G} \nRecord xml is: {2:G} To  Reset Its Original Format We Have Else Condition ,Else New Message will be like Exception during {0:G}: \nMessage: {1:G} 
                    }
                }
            }

            return newMessage;
        }

    }
}
