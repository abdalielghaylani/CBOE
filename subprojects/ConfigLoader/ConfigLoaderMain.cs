using System;
using System.Windows.Forms;
using CambridgeSoft.COE.ConfigLoader.Windows.Forms;

namespace CambridgeSoft.COE.ConfigLoader.Windows
{
    /// <summary>
    /// The main class for the application.
    /// </summary>
    static class ConfigLoaderMain
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Forms.ConfigLoader());
            return;
        }
    }
}