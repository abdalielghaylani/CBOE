using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    /// <summary>
    /// The representation of one INI setting item migration result
    /// </summary>
    public class ImportIniResult
    {
        private const string TRACE_FILE_NAME = "ImportIniLog.txt";
        private const string TRACE_LISTENER_NAME = "iniMigrationListener";

        private static string _importStartMessageTemplate = string.Format("{0}\tINI migration starts", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
        /// <summary>
        /// The message template for indicating an attempt of import has just started
        /// </summary>
        public static string ImportStartMessageTemplate
        {
            get { return _importStartMessageTemplate; }
            set { _importStartMessageTemplate = value; }
        }

        private static string _importEndMessageTemplate = string.Format("{0}\tINI migration ends", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
        /// <summary>
        /// The message template for indicating an attempt of import has just ended
        /// </summary>
        public static string ImportEndMessageTemplate
        {
            get { return _importEndMessageTemplate; }
            set { _importEndMessageTemplate = value; }
        }

        private static string _importErrorMessageTemplate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\tERROR\tINI migration terminated\t{0}";
        /// <summary>
        /// The message template for indicating an general error has occured during the import
        /// </summary>
        public static string ImportErrorMessageTemplate
        {
            get { return _importErrorMessageTemplate; }
            set { _importErrorMessageTemplate = value; }
        }

        private string _successMessageTemplate = string.Empty;
        /// <summary>
        /// The message template for successful import item
        /// </summary>
        public string SuccessMessageTemplate
        {
            get { return _successMessageTemplate; }
            set { _successMessageTemplate = value; }
        }

        private string _failureMessageTemplate = string.Empty;
        /// <summary>
        /// The message  template for failed import item
        /// </summary>
        public string FailureMessageTemplate
        {
            get { return _failureMessageTemplate; }
            set { _failureMessageTemplate = value; }
        }

        private string _skipMessageTemplate = string.Empty;
        /// <summary>
        /// The message template for item that has already been imported
        /// </summary>
        public string SkipMessageTemplate
        {
            get { return _skipMessageTemplate; }
            set { _skipMessageTemplate = value; }
        }

        /// <summary>
        /// Writes the log message to disk file, with the template to format and the message components
        /// </summary>
        /// <param name="template">The template used to format the message</param>
        /// <param name="messages">The message components</param>
        [COEUserActionDescription("LogIniImport")]
        public void LogResult(string template, params object[] messages)
        {
            try
            {
                TextWriterTraceListener iniListener = InitializeTrace();
                iniListener.WriteLine(string.Format(template, messages));
                iniListener.Flush();
                iniListener.Close();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        /// <summary>
        /// Remove the listener for this import process to prevent other code from logging
        /// irrelevant content into our log file
        /// </summary>
        /// 
        [COEUserActionDescription("RemoveListenerFromTrace")]
        public static void RemoveListenerFromTrace()
        {
            try
            {
                TextWriterTraceListener iniListener = (TextWriterTraceListener)Trace.Listeners[TRACE_LISTENER_NAME];

                if (iniListener != null)
                {
                    Trace.Listeners.Remove(iniListener);
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
        }

        /// <summary>
        /// Initialize the static Trace object, clearing all listerners and then adding a new
        /// default one
        /// </summary>
        private TextWriterTraceListener InitializeTrace()
        {
            TextWriterTraceListener iniListener = (TextWriterTraceListener)Trace.Listeners[TRACE_LISTENER_NAME];
            if (iniListener == null)
            {
                string folder = System.IO.Path.Combine(COEConfigurationBO.ConfigurationBaseFilePath, "LogOutput");
                string file = TRACE_FILE_NAME;
                string path = System.IO.Path.Combine(folder, file);
                iniListener = new TextWriterTraceListener(path, TRACE_LISTENER_NAME);
                Trace.Listeners.Add(iniListener);
            }
            return iniListener;
        }
    }
}
