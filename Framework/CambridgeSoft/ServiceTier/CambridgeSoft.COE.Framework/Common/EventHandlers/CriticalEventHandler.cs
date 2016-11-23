using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Permissions;
using System.Threading;
using System.Web;
using System.Configuration;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.EnterpriseLibrary.Common.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Logging.Properties;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.IO;

namespace CambridgeSoft.COE.Framework.Types.EventHandlers
{
    /// <summary>
    /// Our custom Exception Handler Error Reporting.
    /// This class writes to the Trace.axd file in the root folder of a GUIShell Application.
    /// </summary>
    [ConfigurationElementType(typeof(CustomHandlerData))]
    public class COEEventsHandler : IExceptionHandler
    {
        #region Variables

        private const string LogCategoryMessages = "LogCategoryMessages";
        private string _specialFolder = COEConfigurationBO.ConfigurationBaseFilePath;
        private int adminLogCategory = int.MinValue;
        private uint categoryException = uint.MinValue;
        private const string messageCategory = "Error";

        #endregion

        #region Constructor

        public COEEventsHandler(NameValueCollection ignore)
        {

        }

        #endregion

        #region IExceptionHandler Members

        /// <summary>
        /// Method that is raised when a COEEventsHandler (Custom Handler) is defined for a particular exception
        /// </summary>
        /// <param name="exception">A particular exception raised by the App</param>
        /// <param name="handlingInstanceId"></param>
        /// <returns>The same exception that the one received by parameter</returns>
        public Exception HandleException(Exception exception, Guid handlingInstanceId)
        {
            ReadExceptionProperties(_specialFolder + @"COEExceptionHandling.xml", exception.GetType().Name);
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Trace.IsEnabled)
                {
                    adminLogCategory = CambridgeSoft.COE.Framework.GUIShell.GUIShellUtilities.GetAdminLogCategory(HttpContext.Current.Application["AppName"].ToString(), "MISC", "LogCategoryMessages");
                    /* Only write to log if the message's category is greater than the one defined by the App Admin at the 
                     * LogCategoryMessages key in the Web.Config */
                    if (categoryException >= adminLogCategory)
                        WriteToLog(exception.Message.ToString());
                }
            }
            return exception;
        }

        #endregion

        #region Methods

        /// <summary>
        /// This method read the XML excepetion definition. This XML doc says the Category of each exception
        /// (Customized or System exceptions)
        /// </summary>
        /// <param name="xmlFilePath">Where to find the XML</param>
        /// <param name="exceptionName">The Name of the exception (exception.GetType().Name)</param>
        private void ReadExceptionProperties(string xmlFilePath, string exceptionName)
        {
            try
            {
                if(File.Exists(xmlFilePath))
                {
                    XmlTextReader xReader = new XmlTextReader(xmlFilePath);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(xReader);
                    string _xml = xmlDoc.InnerXml;
                    XPathDocument xDocument = new XPathDocument(xmlFilePath);
                    XPathNavigator xNavigator = xDocument.CreateNavigator();
                    XPathNodeIterator xIterator = xNavigator.Select("ExceptionsHandling/Exceptions/" + exceptionName);
                    if(xIterator.MoveNext())
                    {
                        if(!string.IsNullOrEmpty(xIterator.Current.Value))
                            categoryException = uint.Parse(xIterator.Current.Value);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("Error in Custom Handler. ({0})", exception.Message.ToString()) );
            }
        }

        /// <summary>
        /// This method writes to the trace.axd file a warn message.
        /// </summary>
        /// <param name="exceptionMessage">The message exception to log </param>
        private void WriteToLog(string exceptionMessage)
        {
            HttpContext.Current.Trace.Warn(messageCategory, exceptionMessage);
        }

        #endregion
    }
}
