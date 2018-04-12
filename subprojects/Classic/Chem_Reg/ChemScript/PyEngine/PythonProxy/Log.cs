using System;
using System.IO;
using System.Text;

namespace CambridgeSoft.ChemFlow
{
    public class Log
    {
        public enum Level
        {
            Regular,
            Warning,
            Error,
            Failed,
            Crash
        }

        int errorCount = 0;
        int warningCount = 0;
        string logfile = null;
        StreamWriter stream = null;
        string logEntry = "";

        public int ErrorCount { get { return errorCount; } }
        public int WarningCount { get { return warningCount; } }
        public string LogEntry { get { return logEntry; } }

        public void Close()
        {
            if (stream != null)
            {
                stream.Close();
                stream = null;
            }
        }

        public Log(string file, bool clean)
        {
            if (clean)
            {
                try
                {
                    if (File.Exists(file))
                        File.Delete(file);
                }
                catch
                {
                }
            }

            logfile = file;
        }

        public void Add(string msg, Level lvl)
        {
            try
            {
                AddRecord(msg, lvl);
            }
            catch
            {
            }
        }

        void AddRecord(string msg, Level lvl)
        {
            if (lvl != Level.Regular)
            {
                if (lvl == Level.Warning)
                    ++warningCount;
                else
                    ++errorCount;
            }

            if (logfile == null)
                return;

            if (stream == null)
                stream = File.AppendText(logfile);

            string s = null;
            switch (lvl)
            {
                case Level.Regular:
                    s = "REG";
                    break;
                case Level.Warning:
                    s = "WAR";
                    break;
                case Level.Error:
                    s = "ERR";
                    break;
                case Level.Failed:
                    s = "FAI";
                    break;
                case Level.Crash:
                    s = "CRA";
                    break;
                default:
                    s = "   ";
                    break;
            }

            s += ":" + DateTime.Now+ ":" + msg;
            stream.WriteLine(s);
            stream.Flush();

            logEntry = logEntry + "!!!!!" + s;
        }
    }
}
