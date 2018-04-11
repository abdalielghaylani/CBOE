using System;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.CustomReportDesigner;
using CambridgeSoft.COE.Framework.ServerControls.Reporting;
using CambridgeSoft.COE.Framework.ServerControls.Login;

namespace CustomReportDesigner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string []args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ReportDesignerController(Arguments.GetArgsFromQueryString(args)/*"T5_85", "T5_85"*/).ReportDesignerForm);           
        }
    }
}
