using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.ServerControls.Login;

namespace CambridgeSoft.COE.RLSConfigurationTool
{
    /// <summary>
    /// Main class. In charge of handling user input and displaying help messages.
    /// </summary>
    class RLSCommandLineConfiguration
    {
        /// <summary>
        /// Description:
        ///     Enables/Disables RLS and updates RLS related Configuration Settings
        /// 
        /// Usage: 
        ///     rlsconfigurationtool -v=RLSValue  -i=orclInstance -u=oracleuser -p=oraclepass
        /// 
        /// Options:
        ///     -v ActiveRLS Setting value (Off|Registry Level Projects|Batch Level Projects)
        ///     -i Oracle Instance
        ///     -u Oracle user
        ///     -p Oracle password
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                // Parse Command line arguments
                Arguments argumentList = new Arguments(args);
                string rlsValue = argumentList["v"];
                string orclInstanceName = argumentList["i"];
                string orclUserName = argumentList["u"];
                string orclPassword = argumentList["p"];
                // Update RLS
                RLSConfigurationHandler.ChangeRLSValue(rlsValue, orclInstanceName, orclUserName, orclPassword);
                System.Console.WriteLine("RLS Update Executed");
            }
            catch (Exception ex)
            {
                Console.WriteLine("The following error occurred:");
                Console.Write("\t");
                Console.WriteLine(ex.Message);
                Console.WriteLine();
            }
        #if DEBUG
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        #endif
        }
    }
}
