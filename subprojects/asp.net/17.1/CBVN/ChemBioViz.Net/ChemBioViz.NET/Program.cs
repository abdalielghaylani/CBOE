using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Deployment.Application;
using System.Web;
using Utilities;

namespace ChemBioViz.NET
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>

        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        [STAThread]
        static void Main(String[] args0)
		{
            string[] args = GetArgs(args0);

            // CBOE-2198: Object reference error while click OK for formatting and advanced binding dialog without selecting type
            // Set up a thread exception handler to catch any unhandled exceptions in WinForms threads
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            // TO DO: concat leading args ("cs demo props /login=xxx" gets cs,demo,props,/login; want cs demo props,/login)
            CommandLine cmdLine = new CommandLine(args);

            if (args.Length > 0)
            {
                // if there are command line args, redirect output to console window
                AttachConsole(ATTACH_PARENT_PROCESS);
                if (!cmdLine.Parse())
                {
                    cmdLine.ShowHelp();
                    return;
                }
            } 
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ChemBioVizForm(cmdLine));
		}

        /// <summary>
        /// Handles any unhandled exception coming from Windows Forms
        /// </summary>
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            // log this unhandled exception. ReportError/ReportWarning will display a 
            // message box that we want to avoid.
            CBVUtilities.CBVUtil.WriteException(e.Exception, "Application_ThreadException");
        }

        static string[] GetArgs(string[] args0)
        {
            // get command-line args from http deployment (for click-once)
            // from http://weblogs.asp.net/marianor/archive/2008/03/08/simulating-command-line-parameters-in-click-once-applications.aspx
            if (ApplicationDeployment.IsNetworkDeployed  )
            {
                Uri theUri = ApplicationDeployment.CurrentDeployment.ActivationUri;
                if (theUri != null)
                {
                    string query = HttpUtility.UrlDecode(theUri.Query);
                    if (!string.IsNullOrEmpty(query) && query.StartsWith("?"))
                    {
                        string[] arguments = query.Substring(1).Split(' ');
                        string[] commandLineArgs = new string[arguments.Length + 1];
                        commandLineArgs[0] = Environment.GetCommandLineArgs()[0];
                        arguments.CopyTo(commandLineArgs, 1);
                        return commandLineArgs;
                    }
                }
            }
            return args0;
        }
	}
}