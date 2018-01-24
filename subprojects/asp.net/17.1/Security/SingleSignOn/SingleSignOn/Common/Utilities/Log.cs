using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Threading;

namespace CambridgeSoft.COE.Security.Services.Utilities.ELNUtils
{
    public class SSOLog
    {

        private string currentAction;
        private string _logPath;
        private static Mutex fileMutex = new Mutex();

        /// <summary>
        /// The last time a message was been written to the log stream.
        /// </summary>
        private DateTime startTime;

        private int mLineCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="SSOLog"/> class.
        /// </summary>
        public SSOLog()
        {
            this.LogPath = GetLogPath();
            startTime = System.DateTime.Now;
            mLineCount = 1;
        }


        private string GetLogPath()
        {
            //string logDirPath = Path.Combine(Path.GetTempPath(), "SSOLog");
            //Environment.SpecialFolder.CommonApplicationData) + "\\" + ConfigurationManager.AppSettings["ConfigPath"].ToString() 
            string logDirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + ConfigurationManager.AppSettings["ConfigPath"].ToString() + "\\", "SSOLog");
            if (!Directory.Exists(logDirPath))
                Directory.CreateDirectory(logDirPath);

            logDirPath = Path.Combine(logDirPath, "SSO.log");

            if (!File.Exists(logDirPath))
            {
                FileStream fs = File.Create(logDirPath);
                fs.Close();
            }   
            
            return logDirPath;
        }


        /// <summary>
        /// Gets or sets a description of the current operation being performed.
        /// </summary>
        public string CurrentAction
        {
            get
            {
                return currentAction;
            }
            set
            {
                currentAction = value;
            }
        }

        /// <summary>
        /// Gets or sets a description of the current operation being performed.
        /// </summary>
        public string LogPath
        {
            get
            {
                return _logPath;
            }
            set
            {
                _logPath = value;
            }
        }

        /// <summary>
        /// Write the specified string to any currently open log stream.
        /// </summary>
        /// <param name="value">The string to write to the currently open log stream.</param>
        /// <remarks>If the log stream has not been opened, then no operation occurs, except that the line number increments.</remarks>
        public void Write(string value)
        {
                System.DateTime ctrl = System.DateTime.Now;
                TimeSpan timeSpan = ctrl.Subtract(startTime);
                fileMutex.WaitOne();

                try
                {
                    FileStream fsOut = new FileStream(_logPath, FileMode.Append, FileAccess.Write, FileShare.Write);
                    StreamWriter sw = new StreamWriter(fsOut);
                    string log = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString("00000000") + " " + mLineCount.ToString("00000") +
                                         " " + timeSpan.TotalSeconds.ToString("00000.0000") + " " + ": " + value;
                    System.Diagnostics.Debug.WriteLineIf(!log.EndsWith("\n"), log);
                    sw.WriteLine(log);
                    sw.Close();
                }
                catch (Exception)
                {
                    // Ignore errors.
                }
                finally
                {
                    fileMutex.ReleaseMutex();
                }
  
        }



        private static string Join(Exception e, string messageSuffix)
        {
            string errDesc = e.Message;
            if (!errDesc.EndsWith("."))
                errDesc += ".";

            if (messageSuffix.Length > 0 && !messageSuffix.StartsWith("\n\n"))
                errDesc += "\n\n";

            return errDesc + messageSuffix;
        }


        /// <summary>
        /// Writes information about the specified exception to the log file.
        /// </summary>
        /// <param name="ex">The exception to pass to the log file</param>
        /// <param name="messageSuffix">Additional messages to appear after the message in the exception.</param>
        /// <param name="title">The title of the operation that threw the exception.</param>
        public void Write(Exception ex, string messageSuffix, string title)
        {
            string errDesc = Join(ex, messageSuffix);

            Write("");
            Write("The Following Error Message Appeared:");
            Write("Title: " + title);
            Write("Message: " + "Sorry, " + errDesc);

            Console.WriteLine();
            Console.WriteLine("The Following Error Message Appeared:");
            Console.WriteLine("Title: " + title);
            Console.WriteLine("Message: " + "Sorry, " + errDesc);

            WriteException(ex);
        }

        /// <summary>
        /// Writes the specified message to the log file, setting the current action to the assembly.module.routine name.
        /// </summary>
        /// <param name="type">The type of object.</param>
        /// <param name="routine">The name of the calling routine.</param>
        /// <param name="message">The message to be written to the log.</param>
        public void Write(System.Type type, string routine, string message)
        {
            System.Reflection.Assembly assembly = type.Assembly;
            string module = type.Name;
            Write(assembly.GetName().Name + "." + module + "." + routine, message);
        }

        /// <summary>
        /// Writes the specified message to the log file.
        /// </summary>
        /// <param name="currentAction">The current action to be recorded.</param>
        /// <param name="message">The message to be written to the log.</param>
        public void Write(string currentAction, string message)
        {
            this.currentAction = currentAction;
            Write("==>> " + currentAction + ": " + message);
        }


        /// <summary>
        /// Recursively writes the inner exception of this exception (if there is one) and then writes this exception.
        /// </summary>
        /// <param name="ex">The exception to write.</param>
        private void WriteException(Exception ex)
        {
            if (ex.InnerException != null)
                WriteException(ex.InnerException);

            Write("Source:  " + ex.StackTrace);
            Write("Type:    " + ex.GetType().FullName);
            Write("Message: " + ex.Message);
            Console.WriteLine("Source:  " + ex.StackTrace);
            Console.WriteLine("Type:    " + ex.GetType().FullName);
            Console.WriteLine("Message: " + ex.Message);
        }


    }
}
