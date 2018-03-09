using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Deployment.Application;
using System.Web;
using CambridgeSoft.COE.Framework.ServerControls.Reporting;
using CambridgeSoft.COE.Framework.ServerControls.Login;

namespace ReportViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string [] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CambridgeSoft.COE.Framework.ReportViewer.ReportViewer(Arguments.GetArgsFromQueryString(args)));
        }
    }
}
