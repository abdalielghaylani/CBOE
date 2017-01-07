// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="PerkinElmer Inc.">
//   Copyright (c) 2013 PerkinElmer Inc.,
//   940 Winter Street, Waltham, MA 02451.
//   All rights reserved.
//   This software is the confidential and proprietary information
//   of PerkinElmer Inc. ("Confidential Information"). You shall not
//   disclose such Confidential Information and may not use it in any way,
//   absent an express written license agreement between you and PerkinElmer Inc.
//   that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace COEConfigurationPublishTool
{
    using System;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The program
    /// </summary>
    internal class Startup
    {
        /// <summary>
        /// main entry
        /// </summary>
        /// <param name="args">the parameters</param>
        internal static void Main(string[] args)
        {
            // Set the title.
            Console.Title = "Datalytix Data Source Publishing Tool";

            // Print the help information to show the usage of the tool.
            if (args.Length > 0 && args[0].Trim() == "/?")
            {
                PrintHelp();
                Console.ReadLine();
                return;
            }

            // Init the logger.
            Init(args);

            WorkflowExecutor workflow;

            if (args.Length > 1)
            {
                Logger.SingleInstance.PrintAndLogMsg("Received command:" + string.Join(" ", args));
                Console.WriteLine();

                workflow = new SilentPublishingWorkflow();
            }
            else
            {
                workflow = new InteractivePublishingWorkflow();
            }

            try
            {
                workflow.Initialize(args);
                workflow.Run();
            }
            catch (Exception ex)
            {
                Logger.SingleInstance.PrintAndLogMsg(ex.Message);
            }
            finally
            {
                Console.WriteLine("Press any key to exit");
                Console.Out.Flush();
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Before publish, prepare the lists
        /// </summary>
        /// <param name="args">The parameters</param>
        private static void Init(string[] args)
        {
            Console.WriteLine("The tool helps to publish the data source(s) and dataview(s).");
            Console.WriteLine();

            var logFileSwitch = args.Where(s => s.StartsWith("/log:", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            var logFileName = logFileSwitch == null ? Consts.DefaultLogFileName : logFileSwitch.Substring(5);

            if (System.IO.File.Exists(logFileName))
            {
                System.IO.File.Delete(logFileName);
            }

            // Initialize the logger.
            Logger.Init(logFileName);

            Csla.ApplicationContext.GlobalContext.Add("USER_PERSONID", "0");
        }

        /// <summary>
        /// Print the help usage.
        /// </summary>
        private static void PrintHelp()
        {
            var helpStr = new StringBuilder();

            System.Console.WriteLine();
            helpStr.AppendLine("Switches:");
            helpStr.AppendLine("    /usage: [publish/encrypt/decrypt/republish] (publish)");
            helpStr.AppendLine("    /name:  The name of the data source.");
            helpStr.AppendLine("    /dbmsType: [ORACLE/SQLServer/MSACCESS] (ORACLE)");
            helpStr.AppendLine("    /driverType: [0 - Oracle / 1 - OracleDirect] (0 - Oracle)");
            helpStr.AppendLine("    /databaseSysUser: required on publishing.");
            helpStr.AppendLine("    /databaseSysUserPassword: required on publishing.");
            helpStr.AppendLine("    /databaseGlobalUser: required on publishing.");
            helpStr.AppendLine("    /databaseGlobalUserPassword: required on publishing.");
            helpStr.AppendLine("    /useProxy: [true/false] (true)");
            helpStr.AppendLine("    /hostName: required on publishing");
            helpStr.AppendLine("    /port: (1521)");
            helpStr.AppendLine("    /sid: required on publishing");
            helpStr.AppendLine("    /networkAlias: oracle network alias");
            helpStr.AppendLine("    /isCBOEInstance: [true/false] (false)");
            helpStr.AppendLine("    /log: The log file name. Default is 'COEConfigurationPublishTool.log'");
            helpStr.AppendLine("    /password: The password value which will be encrypt or decrypt");
            helpStr.AppendLine("    /outputpath: The password encrypt or decrypt value output path.");
            helpStr.AppendLine("    /republishAllDataSources: [true/false] (false)");
            helpStr.AppendLine("    /republishAllDataviews: [true/false] (false)");

            Console.WriteLine(helpStr);

            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine(" - Publishing mode");
            Console.WriteLine("         /name:RemoteTest /dbmsType:ORACLE /databaseSysUser:system /databaseSysUserPassword:manager2 /databaseGlobalUser:coeuser2 /databaseGlobalUserPassword:userPwd  /hostName:myMachine /sid:orcl /networkAlias:remoteORCL /isCBOEInstance:True /usage:publish");
            Console.WriteLine();
            Console.WriteLine(" - Republishing mode:");
            Console.WriteLine("         /usage: republish /republishalldatasources:true /republishalldatasources:true");
        }
    }
}