using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using CambridgeSoft.COE.Framework.COELoggingService;

namespace COELogViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Startup();
        }

        private static void Startup()
        {
            try
            {
                string[] args = Environment.GetCommandLineArgs();

                COELogViewerForm logViewer = new COELogViewerForm();
                if (args.Length > 1)
                {
                    logViewer.Show();
                    logViewer.OpenLog(args[1]);
                }
                else
                {
                    logViewer.Show();
                    logViewer.ShowOpenLogDialog();
                }

                Application.Run(logViewer);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        //static void logFile_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    if (reInit)
        //    {
        //        logFile.Hide();
        //        Startup();
        //    }
        //}

    }
}