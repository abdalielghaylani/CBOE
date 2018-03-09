using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace CambridgeSoft.COE.Security.Services.Utilities.ELNUtils
{
    //This class has been added to keep code compiling from ELN
    public class Customizer
    {
        internal void BeforeBinding(ref string domainName,ref string userName,ref bool cancel)
        {
 	        throw new Exception("Customizers are not currently supported for Single Sign On. Please remove any customizer configuration from the configuration xml.");
        }

        internal static Customizer[] GetCustomizers(System.Xml.XmlElement xmlElement, ENSConnection connection)
        {
            return null;
            //throw new Exception("The method or operation is not implemented.");
        }

        internal void Initialize(System.Xml.XmlElement configElement)
        {
            throw new Exception("Customizers are not currently supported for Single Sign On. Please remove any customizer configuration from the configuration xml.");
        }

        internal void BeforeQuery(ref string filterText, ref string vUserName, ref bool cancel)
        {
            throw new Exception("Customizers are not currently supported for Single Sign On. Please remove any customizer configuration from the configuration xml.");
        }
    }


    public class ENSConnection
    {
        private SSOLog Log;

        /// <summary>
        /// Instantiates a new instance of the ENSConnection class.
        /// This is not the same as the one in ELN as it has been trimmed down
        /// </summary>
        public ENSConnection()
        {
        //    mGlobals = new Globals(this);
        //    logMode = 0;
            Log = new SSOLog();

        }

        /// <summary>
        /// Write the specified string to a log file.
        /// </summary>
        /// <param name="log">The string to be written to the log file.</param>
        /// <remarks>The location of the log file is in the same folder as the dll that contains this class.</remarks>
        public void WriteLogStream(string log)
        {
            Log.Write(log);
        }

        /// <summary>
        /// Writes information about the specified exception to the log file.
        /// </summary>
        /// <param name="ex">The exception to pass to the log file</param>
        /// <param name="messageSuffix">Additional messages to appear after the message in the exception.</param>
        /// <param name="title">The title of the operation that threw the exception.</param>
        public void WriteLogStream(Exception ex, string messageSuffix, string title)
        {
            if (Log != null)
                Log.Write(ex, messageSuffix, title);
        }


        //internal void WriteLogStream(string message)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}
    }

    public class EVWriter
    {
        public static void Write(string textToWrite)
        {
        }
    }

    public class Utils
    {
          /// <summary>
        /// Write the specified message to the E-Notebook log file.
        /// </summary>
        /// <param name="connection">Connection is just left here for easy reusability of calls from ELN.</param>
        /// <param name="module">The name of the calling module.</param>
        /// <param name="routine">The name of the calling routine.</param>
        /// <param name="message">The message to be written to the log.</param>
        /// Eln version
        //public static void WriteLog(IConnection connection, string module, string routine, string message)
        public static void WriteLog(ENSConnection connection, string module, string routine, string message)
        {
            try
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                message = "==>> " + ((System.Reflection.AssemblyTitleAttribute)System.Reflection.AssemblyTitleAttribute.GetCustomAttribute(assembly, typeof(System.Reflection.AssemblyTitleAttribute))).Title + ": " + module + "." + routine + ": " + message;
                System.Diagnostics.Debug.Print(DateTime.Now.ToString("HH:mm:ss") + " " + message);
                if (connection != null)
                    connection.WriteLogStream(message);
            }
            catch (Exception ex)
            {
                connection.WriteLogStream("WriteLog Method failed: " + ex.Message);
            }
        }
    }
      

}
