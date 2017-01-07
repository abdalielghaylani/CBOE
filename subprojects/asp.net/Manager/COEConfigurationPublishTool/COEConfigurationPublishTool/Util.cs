// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Util.cs" company="PerkinElmer Inc.">
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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using CambridgeSoft.COE.Framework.COEConfigurationService;
    using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
    using CambridgeSoft.COE.Framework.COEDataViewService;
    using CambridgeSoft.COE.Framework.Common;

    /// <summary>
    /// The utility for configuration publish tool
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Build the instanceBO by interactive with end user. 
        /// The instance will be overwrite or new create.
        /// </summary>
        /// <param name="coeInstance">The instance BO. It may empty or existing BO</param>
        /// <returns>The instanceBO with new value user input.</returns>
        public static COEInstanceBO BuildCOEInstanceByConsoleDialog(COEInstanceBO coeInstance)
        {
            string[] forbiddenWordsForHostName = { "localhost", "127.0.0.1" };

            coeInstance.HostName = GetInput<string>("Host name", coeInstance.HostName, false, forbiddenWordsForHostName);
            coeInstance.SID = GetInput<string>("SID", coeInstance.SID);
            coeInstance.Port = int.Parse(GetInput<int>("Data source port", coeInstance.Port));

            var driverTypes = new Dictionary<int, string>
            {
                { (int)DriverType.Oracle, "Oracle" },
                { (int)DriverType.OracleDataDirect, "Oracle (DataDirect)" }
            };

            coeInstance.DriverType = (DriverType)GetOptionInput("Spotfire driver type", (int)coeInstance.DriverType, driverTypes);

            var instanceBO = new COEInstanceBO(
                coeInstance.Id,
                coeInstance.InstanceName,
                coeInstance.DbmsType,
                coeInstance.DatabaseGlobalUser,
                coeInstance.IsCBOEInstance,
                true,
                coeInstance.Password,
                coeInstance.HostName,
                coeInstance.Port,
                coeInstance.SID,
                coeInstance.DriverType);

            return instanceBO;
        }

        /// <summary>
        /// Enable the primary data source by execute the database script.
        /// </summary>
        /// <param name="serviceName">The oracle service name.</param>
        /// <param name="sysUser">The oracle account which has sys privilege.</param>
        /// <param name="sysUserPassword">The oracle account password.</param>
        /// <param name="globalUser">The global user.</param>
        /// <param name="globalUserPassword">The global user password.</param>
        /// <returns>The exit code when run database script.</returns>
        public static int EnableDataSource(string serviceName, string sysUser, string sysUserPassword, string globalUser, string globalUserPassword)
        {
            const string EnableDataSourceCmdFormat = "sqlplus /nolog @Instance_AutoCreation.sql {0} {1} {2} {3} {4}";

            var currentLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var currentDir = new System.IO.DirectoryInfo(currentLocation);
            var datalytixServerDir = currentDir.Parent.Parent.Parent;
            var sqlScriptDir = System.IO.Path.Combine(datalytixServerDir.FullName, @"Framework\DBInstallScripts\Datalytix\sql");

            var commandLine = string.Format(EnableDataSourceCmdFormat, serviceName, sysUser, sysUserPassword, globalUser, globalUserPassword);
            int exitCod = Util.RunCmd(commandLine, sqlScriptDir);

            return exitCod;
        }

        /// <summary>
        /// Publish existing data views to SPOTFIRE.
        /// </summary>
        public static void PublishExistingDataviews()
        {
            Logger.SingleInstance.PrintAndLogMsg("Start to retrieve all published dataviews");
            var dataviewList = COEDataViewBOList.GetAllDataViewDataList();

            Logger.SingleInstance.PrintAndLogMsg(string.Format("{0} dataview(s) in total.", dataviewList.Count), true);

            int successCount = 0;
            int failedCount = 0;

            foreach (var dataview in dataviewList)
            {
                try
                {
                    dataview.PublishToSpotfire();
                    successCount++;
                }
                catch (Exception ex)
                {
                    failedCount++;
                    Logger.SingleInstance.PrintAndLogError(ex.Message);
                }
            }

            Logger.SingleInstance.LogOnly("Dataview republishing subflow completed.");
            Logger.SingleInstance.PrintAndLogMsg("Dataview republishing result:");
            Logger.SingleInstance.PrintAndLogInfo(string.Format(" Success - {0}", successCount));
            Logger.SingleInstance.PrintAndLogError(string.Format(" Failure - {0}", failedCount), true);
        }

        /// <summary>
        /// Gets the data source value from old element.
        /// </summary>
        /// <returns>The legacy data source value will be returned.</returns>
        public static string GetLegacyDataSource()
        {
            var coeConfigPath = COEConfigurationManager.GetDefaultConfigurationFilePath();
            XDocument doc = XDocument.Load(coeConfigPath);

            var databaseTypesElements = doc.XPathSelectElements("configuration/coeConfiguration/dbmsTypes/add");
            var oracleTypeElement = databaseTypesElements.Where(x => x.Attribute("name").Value.Equals(
                Consts.OracleType,
                StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (oracleTypeElement == null)
            {
                return string.Empty;
            }

            var dataSource = oracleTypeElement.Attribute("dataSource").Value;
         
            return dataSource;
        }

        /// <summary>
        /// Show the message to user and asking the input value.
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="label">The display message to end user.</param>
        /// <param name="defaultValue">The default value of the value</param>
        /// <param name="isSecurity">The value is security or not</param>
        /// <param name="forbiddenWords">The forbidden words</param>
        /// <returns>The string user input.</returns>
        public static string GetInput<T>(string label, T defaultValue, bool isSecurity = false, string[] forbiddenWords = null)
        {
            while (true)
            {
                var defaultValueStr = defaultValue == null ? null : defaultValue.ToString();
                var displayMsgWithDefaultHint = GetDisplayMsgWithDefaultValueHint(label, defaultValueStr, isSecurity);

                Console.Write(displayMsgWithDefaultHint);

                var input = isSecurity ? GetSecurityInput() : Console.ReadLine();

                // If input value is empty, check the default value.
                if (string.IsNullOrEmpty(input))
                {
                    // The input value and default value are null/empty.
                    if (defaultValue == null || string.IsNullOrEmpty(defaultValue.ToString()))
                    {
                        Logger.SingleInstance.PrintAndLogError(string.Format("{0} cannot be null or empty", label));
                        continue;
                    }
                    else
                    {
                        // use the default value.
                        return defaultValue.ToString();
                    }
                }
                else
                {
                    // Check if the input is forbidden word.
                    if (forbiddenWords != null &&
                        forbiddenWords.Where(w => w.Equals(
                            input.Trim(),
                            StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                    {
                        Logger.SingleInstance.PrintAndLogError(string.Format("'{0}' is forbidden for {1}", input.Trim(), label));
                        continue;
                    }
                }

                // Validate the input type format.
                if (!ValidateInput(typeof(T), input))
                {
                    Logger.SingleInstance.PrintAndLogError(
                        string.Format("{0} Incorrect format of {1}, please input the correct string.", label, typeof(T)));

                    continue;
                }
                else
                {
                    return input;
                }
            }
        }

        /// <summary>
        /// Shows the options to end user and waiting for user's choice.
        /// </summary>
        /// <param name="label">The display message to end user.</param>
        /// <param name="defaultOption">The default option value.</param>
        /// <param name="optionList">The option list user can choose.</param>
        /// <returns>The option index will be returned.</returns>
        public static int GetOptionInput(string label, int defaultOption, Dictionary<int, string> optionList)
        {
            var displayMsg = new StringBuilder();
            displayMsg.AppendLine(label + ": ");
            displayMsg.AppendLine();

            foreach (var option in optionList)
            {
                displayMsg.Append("     ");
                displayMsg.AppendLine(option.Key + ". " + option.Value);
            }

            displayMsg.AppendLine();
            displayMsg.Append("Please input the option number. (" + defaultOption + ") : ");

            while (true)
            {
                Console.Write(displayMsg.ToString());

                var input = Console.ReadLine();

                // If input value is empty, check the default value.
                if (string.IsNullOrEmpty(input))
                {
                    // The input value and default value are null/empty.
                    if (!optionList.ContainsKey(defaultOption))
                    {
                        Logger.SingleInstance.PrintAndLogError(string.Format("{0} cannot be null or empty", label));
                        continue;
                    }
                    else
                    {
                        // use the default value.
                        return defaultOption;
                    }
                }

                // Validate the input type format.
                int optionNumber = 0;
                if (!int.TryParse(input, out optionNumber))
                {
                    Logger.SingleInstance.PrintAndLogError("Incorrect format of option number, please input the correct option number.");
                    continue;
                }
                else
                {
                    if (!optionList.ContainsKey(optionNumber))
                    {
                        Logger.SingleInstance.PrintAndLogError("Option number input is out of range, please input the correct option number.");
                        continue;
                    }

                    return optionNumber;
                }
            }
        }

        /// <summary>
        /// Run the command line and return the exit code.
        /// </summary>
        /// <param name="commandText">The command line content.</param>
        /// <param name="workDir"> The command execution directory</param>
        /// <returns>The exit code process returned.</returns>
        public static int RunCmd(string commandText, string workDir)
        {
            var process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c " + commandText;
            process.StartInfo.WorkingDirectory = workDir;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            process.WaitForExit();

            int exitCode = process.ExitCode;
            process.Close();

            return exitCode;
        }

        /// <summary>
        /// Confirm the choice by console dialog.
        /// </summary>
        /// <param name="description">The display message to end user.</param>
        /// <returns>
        /// The choice user chosen.
        /// </returns>
        public static bool ShowYesNoConsoleConfirmDialog(string description)
        {
            var optionChar = ShowConsoleConfirmDialog(description, new string[] { "Yes", "No" });

            switch (char.ToUpper(optionChar))
            {
                case 'Y':
                    return true;
                case 'N':
                    return false;
                default:
                    throw new NotSupportedException(string.Format("Unknown option '{0}'", optionChar));
            }
        }

        /// <summary>
        /// Show the confirm dialog on console and with options.
        /// </summary>
        /// <param name="description">The display message.</param>
        /// <param name="options">The options user can be choose.</param>
        /// <returns>The choice first char.</returns>
        public static char ShowConsoleConfirmDialog(string description, string[] options)
        {
            while (true)
            {
                Console.Write(string.Format("{0} ({1}): ", description, string.Join("/", options)));
                Console.Out.Flush();

                var stringEntered = Console.ReadLine().Trim();

                if (string.IsNullOrEmpty(stringEntered) ||
                    options.Where(c => c.StartsWith(
                        stringEntered.Substring(0, 1),
                        StringComparison.InvariantCultureIgnoreCase)).Count() == 0)
                {
                    continue;
                }

                return stringEntered[0];
            }
        }

        /// <summary>
        /// Analyses the global user and data source from the old database type elements.
        /// </summary>
        /// <param name="globalUser">The global user.</param>
        /// <param name="dataSource">The data source string.</param>
        public static void AnalyseMainGlobalUserAndDataSource(ref string globalUser, ref string dataSource)
        {
            dataSource = Util.GetLegacyDataSource();

            // Global user name is COEUSER by default
            globalUser = System.Configuration.ConfigurationManager.AppSettings["DefaultGlobalUserName"];
        }

        /// <summary>
        /// Gets the password for global user.
        /// </summary>
        /// <param name="globalUser">The global user name.</param>
        /// <returns>The password of the global user.</returns>
        public static string GetGlobalUserPassword(string globalUser)
        {
            var coeConfigPath = COEConfigurationManager.GetDefaultConfigurationFilePath();
            XDocument doc = XDocument.Load(coeConfigPath);

            var databaseElements = doc.XPathSelectElements("configuration/coeConfiguration/databases/add");
            var globalUserElement = databaseElements.Where(x => x.Attribute("name").Value.Equals(
                globalUser,
                StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (globalUserElement == null)
            {
                var defaultPassword = System.Configuration.ConfigurationManager.AppSettings["DefaultGlobalUserPassword"];
                return defaultPassword;
            }

            return globalUserElement.Attribute("password").Value;
        }

        /// <summary>
        /// Gets the display message with default value hint.
        /// </summary>
        /// <param name="label">The input label.</param>
        /// <param name="defaultValueStr">The default value string.</param>
        /// <param name="isSecurity">If the input is security or not.</param>
        /// <returns>
        /// The display message with the default value hint.
        /// </returns>
        private static string GetDisplayMsgWithDefaultValueHint(string label, string defaultValueStr, bool isSecurity)
        {
            string displayMsg = label;
            var defaultValueHint = defaultValueStr;

            if (!string.IsNullOrEmpty(defaultValueStr))
            {
                if (isSecurity)
                {
                    if (Utilities.IsRijndaelEncrypted(defaultValueStr))
                    {
                        defaultValueStr = Utilities.DecryptRijndael(defaultValueStr);
                    }

                    defaultValueHint = string.Empty;
                    for (int i = 0; i < defaultValueStr.Length; i++)
                    {
                        defaultValueHint += "*";
                    }
                }
            }

            if (!string.IsNullOrEmpty(defaultValueHint))
            {
                displayMsg = string.Format("{0} ({1}): ", displayMsg, defaultValueHint);
            }
            else
            {
                displayMsg = string.Format("{0}: ", displayMsg);
            }

            return displayMsg;
        }

        /// <summary>
        /// Hidden the security input and display * instead.
        /// </summary>
        /// <returns>
        /// Return the real input.
        /// </returns>
        private static string GetSecurityInput()
        {
            var securityStr = string.Empty;

            while (true)
            {
                var tempInput = Console.ReadKey(true);

                if (tempInput.KeyChar == (int)ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (tempInput.KeyChar == (int)ConsoleKey.Backspace)
                {
                    if (securityStr.Length > 0)
                    {
                        securityStr = securityStr.Substring(0, securityStr.Length - 1);
                    }

                    continue;
                }

                securityStr += tempInput.KeyChar;
            }

            return securityStr;
        }

        /// <summary>
        /// Validate the input value with the required the type.
        /// </summary>
        /// <param name="inputType">The input type required.</param>
        /// <param name="inputValue">The input value.</param>
        /// <returns>
        /// The validation result.
        /// </returns>
        private static bool ValidateInput(Type inputType, string inputValue)
        {
            bool validationResult = !string.IsNullOrEmpty(inputValue);
            switch (inputType.Name.ToLower())
            {
                case "int32":
                    {
                        int parseResult;
                        validationResult = int.TryParse(inputValue, out parseResult);
                        break;
                    }

                case "string":
                    validationResult = true;
                    break;
                default:
                    throw new Exception(string.Format("Validation method does not support to validate the type of ''", inputType.Name));
            }

            return validationResult;
        }
    }
}
