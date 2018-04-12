using System;
using System.Windows.Forms;
using CambridgeSoft.COE.DataLoader.Windows.Forms;

namespace CambridgeSoft.COE.DataLoader.Windows
{
    /// <summary>
    /// The main class for the application.
    /// </summary>
    static class DataManagerMain
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Forms.DataLoader());
            return;
        }
    }
}