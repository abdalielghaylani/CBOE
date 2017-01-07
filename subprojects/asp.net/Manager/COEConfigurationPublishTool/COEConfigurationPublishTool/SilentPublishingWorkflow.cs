// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SilentPublishingWorkflow.cs" company="PerkinElmer Inc.">
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
    using System.Data.Common;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;

    using CambridgeSoft.COE.Framework.COEConfigurationService;
    using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
    using CambridgeSoft.COE.Framework.Common;
    using Manager.Code;

    /// <summary>
    /// The workflow for silent publishing work
    /// </summary>
    internal class SilentPublishingWorkflow : WorkflowExecutor
    {
        /// <summary>
        /// The switch bag with the default values.
        /// </summary>
        private static Dictionary<string, PublishingSwitch> switchBag = new Dictionary<string, PublishingSwitch>(StringComparer.InvariantCultureIgnoreCase)
        {
           // Option switches with default value.
           { Consts.SwitchBagKeyDBMSType, new PublishingSwitch { SwitchName = Consts.SwitchBagKeyDBMSType, SwitchValue = "ORACLE", ValueType = SwitchValueType.Options, AllowOptions = new List<string> { "ORACLE" } } },
           { Consts.SwitchBagKeyDriverType, new PublishingSwitch { SwitchName = Consts.SwitchBagKeyDriverType, SwitchValue = ((int)DriverType.Oracle).ToString(), ValueType = SwitchValueType.Int } },
           { Consts.SwitchBagKeyUseProxy, new PublishingSwitch { SwitchName = Consts.SwitchBagKeyUseProxy, SwitchValue = "True", ValueType = SwitchValueType.Bool } },           
           { Consts.SwitchBagKeyIsCBOEInstance, new PublishingSwitch { SwitchName = Consts.SwitchBagKeyIsCBOEInstance, SwitchValue = "False", ValueType = SwitchValueType.Bool } },
           { Consts.SwitchBagKeyRepublishAllDataviews, new PublishingSwitch { SwitchName = Consts.SwitchBagKeyRepublishAllDataviews, SwitchValue = "False", ValueType = SwitchValueType.Bool } },
           { Consts.SwitchBagKeyRepublishAllDatasources, new PublishingSwitch { SwitchName = Consts.SwitchBagKeyRepublishAllDatasources, SwitchValue = "False", ValueType = SwitchValueType.Bool } },
           { Consts.SwitchBagKeyLogFile, new PublishingSwitch { SwitchName = Consts.SwitchBagKeyLogFile, SwitchValue = Consts.DefaultLogFileName, ValueType = SwitchValueType.Text } },
           { Consts.SwitchBagKeyPort, new PublishingSwitch { SwitchName = Consts.SwitchBagKeyPort, SwitchValue = "1521", ValueType = SwitchValueType.Int } },
           { Consts.SwitchBagKeyNetworkAlias, new PublishingSwitch { SwitchName = Consts.SwitchBagKeyNetworkAlias, SwitchValue = string.Empty, ValueType = SwitchValueType.Text } },
                      
           // Required switches need user enter.
           { Consts.SwitchBagKeyInstanceName, new PublishingSwitch { SwitchName = Consts.SwitchBagKeyInstanceName, IsRequired = true, ValueType = SwitchValueType.Text } },
           { Consts.SwitchBagKeyDatabaseGlobalUser, new PublishingSwitch { SwitchName = Consts.SwitchBagKeyDatabaseGlobalUser, IsRequired = true, ValueType = SwitchValueType.Text } },
           { Consts.SwitchBagKeyDatabaseGlobalUserPassword, new PublishingSwitch { SwitchName = Consts.SwitchBagKeyDatabaseGlobalUserPassword, IsRequired = true, ValueType = SwitchValueType.Text } },
           { Consts.SwitchBagKeyDatabaseSysUser, new PublishingSwitch { SwitchName = Consts.SwitchBagKeyDatabaseSysUser, IsRequired = true, ValueType = SwitchValueType.Text } },
           { Consts.SwitchBagKeyDatabaseSysUserPassword, new PublishingSwitch { SwitchName = Consts.SwitchBagKeyDatabaseSysUserPassword, IsRequired = true, ValueType = SwitchValueType.Text } },
           { Consts.SwitchBagKeyHostName, new PublishingSwitch { SwitchName = Consts.SwitchBagKeyHostName, IsRequired = true, ValueType = SwitchValueType.Text } },
           { Consts.SwitchBagKeySID, new PublishingSwitch { SwitchName = Consts.SwitchBagKeySID, IsRequired = true, ValueType = SwitchValueType.Text } },
          
           // switches for encrypt and decrypt
           { Consts.SwitchBagKeyUsage, new PublishingSwitch { SwitchName = Consts.SwitchBagKeyUsage, SwitchValue = Consts.SwitchBagKeyUsageDefaultValue, IsRequired = false, ValueType = SwitchValueType.Text } },
           { Consts.SwitchBagKeyPassword, new PublishingSwitch { SwitchName = Consts.SwitchBagKeyPassword, SwitchValue = string.Empty, IsRequired = false, ValueType = SwitchValueType.Text } },
           { Consts.SwitchBagKeyOutputPath, new PublishingSwitch { SwitchName = Consts.SwitchBagKeyOutputPath, SwitchValue = string.Empty, IsRequired = false, ValueType = SwitchValueType.Text } },
        };

        /// <summary>
        /// The instances which publishing failure.
        /// </summary>
        private static List<string> instancesPublishFailure = new List<string>();

        /// <summary>
        /// The data view publish failure.
        /// </summary>
        private static List<string> dataviewsPublishFailure = new List<string>();

        /// <summary>
        /// Start to publish the instance.
        /// </summary>
        internal override void Run()
        {
            Logger.SingleInstance.LogOnly("Command:" + string.Join(" ", this.Parameters));

            // Analyse the command line.
            AnalyseSwitches(this.Parameters);

            // Check if exists special usage
            switch (switchBag[Consts.SwitchBagKeyUsage].SwitchValue.ToUpper())
            {
                case "ENCRYPT":
                case "DECRYPT":
                    PasswordCryptOrDecryptSubflow();
                    break;

                case "TESTCONNECTION":
                    TestCOEDBAccessibilitySubflow();
                    break;

                // Default is to publish the instance.
                case "PUBLISH":
                    PublishInstanceSubflow();
                    break;
                case "REPUBLISH":
                    RepublishSubflow();
                    break;
                default:
                    throw new Exception(
                        string.Format("Unknown usage value '{0}'", switchBag[Consts.SwitchBagKeyUsage].SwitchValue));
            }
        }

        /// <summary>
        /// Build the instanceBO, instance and database data base on the switches.
        /// </summary>
        /// <param name="dataSource">The legacy data source name.</param>
        /// <returns>The instance</returns>
        private static COEInstanceBO BuildInstanceBO(ref string dataSource)
        {
            Logger.SingleInstance.LogOnly("Start to build the instance and database BO");

            var instanceId = Guid.NewGuid();
            var instanceName = switchBag[Consts.SwitchBagKeyInstanceName].SwitchValue.ToUpper();

            // Check if the instance already published, if published, use the old instanceId.
            var instanceList = COEInstanceBOList.GetCOEInstanceBOList();
            if (instanceList != null)
            {
                var existingInstance = instanceList.Where(i => i.InstanceName.Equals(instanceName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (existingInstance != null)
                {
                    instanceId = existingInstance.Id;
                }
            }

            var instanceBO = new COEInstanceBO(
                    instanceId,
                    instanceName.ToUpper(),
                    (DBMSType)Enum.Parse(typeof(DBMSType), switchBag[Consts.SwitchBagKeyDBMSType].SwitchValue, true),
                    switchBag[Consts.SwitchBagKeyDatabaseGlobalUser].SwitchValue.ToUpper(),
                    bool.Parse(switchBag[Consts.SwitchBagKeyIsCBOEInstance].SwitchValue),
                    bool.Parse(switchBag[Consts.SwitchBagKeyUseProxy].SwitchValue),
                    switchBag[Consts.SwitchBagKeyDatabaseGlobalUserPassword].SwitchValue,
                    switchBag[Consts.SwitchBagKeyHostName].SwitchValue,
                    int.Parse(switchBag[Consts.SwitchBagKeyPort].SwitchValue),
                    switchBag[Consts.SwitchBagKeySID].SwitchValue,
                    (DriverType)Enum.Parse(typeof(DriverType), switchBag[Consts.SwitchBagKeyDriverType].SwitchValue, true));

            dataSource = Util.GetLegacyDataSource();

            Logger.SingleInstance.LogOnly("Instance and database BO builded");

            return instanceBO;
        }

        /// <summary>
        /// Publish instance
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>Publish succeed or not</returns>
        private static bool PublishInstance(COEInstanceBO instance)
        {
            Logger.SingleInstance.PrintAndLogMsg(string.Format("Publishing data source: {0},", instance.InstanceName));
            string result = SpotfireServiceClient.PublishDBInstance(instance);

            if (!string.IsNullOrEmpty(result))
            {
                instancesPublishFailure.Add(instance.InstanceName);
                Logger.SingleInstance.PrintAndLogError(string.Format("Data source '{0}' publish failed", instance.InstanceName), true);
                Logger.SingleInstance.PrintAndLogError(string.Format("  error message:{0}", result));

                return false;
            }
            else
            {
                Logger.SingleInstance.PrintAndLogInfo(string.Format("Data source '{0}' publish succeed!", instance.InstanceName));

                return true;
            }
        }

        /// <summary>
        /// Get password for instance
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The password</returns>
        private static string GetPasswordForInstance(COEInstanceBO instance)
        {
            var coeConfigPath = COEConfigurationManager.GetDefaultConfigurationFilePath();
            XDocument doc = XDocument.Load(coeConfigPath);

            string globalDbInstanceSchemaName = instance.DatabaseGlobalUser;
            if (instance.IsCBOEInstance)
            {
                globalDbInstanceSchemaName = instance.InstanceName + "." + instance.DatabaseGlobalUser;
            }

            var globalDbElement = doc.XPathSelectElements("configuration/coeConfiguration/databases/add")
                    .Where(e => e.Attribute("name").Value.Equals(
                        globalDbInstanceSchemaName,
                        StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();

            if (globalDbElement != null)
            {
                return globalDbElement.Attribute("password").Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// Analysis the switches entered by user.
        /// </summary>
        /// <param name="args">The switches input string.</param>
        private static void AnalyseSwitches(string[] args)
        {
            var unknownSwitches = new List<string>();

            foreach (var arg in args)
            {
                var switchKeyValue = arg.Trim();
                if (!switchKeyValue.StartsWith(Consts.Slash))
                {
                    unknownSwitches.Add(switchKeyValue);
                    continue;
                }

                switchKeyValue = switchKeyValue.Substring(Consts.Slash.Length);
                var splitArray = switchKeyValue.Split(new char[] { Consts.SwitchKeyValueSeperatorChar });

                string switchKey = splitArray[0];
                string switchValue = splitArray.Length >= 2 ? switchKeyValue.Substring(switchKey.Length + 1) : "true";

                if (switchBag.ContainsKey(switchKey))
                {
                    switchBag[switchKey].SwitchValue = switchValue;
                }
                else
                {
                    unknownSwitches.Add(switchKey);
                }
            }

            if (unknownSwitches.Count > 0)
            {
                Console.WriteLine(string.Format("Warning!! Unkown switches:", string.Join(" , ", unknownSwitches)));
            }
        }

        /// <summary>
        /// Validate the switches.
        /// </summary>
        private static void ValidateSwitches()
        {
            Logger.SingleInstance.PrintAndLogMsg("Start to validate the switch and values.");

            var switchesWithoutValue = switchBag.Values.Where(t => t.IsRequired == true && string.IsNullOrEmpty(t.SwitchValue))
                                                       .Select(t => t.SwitchName).ToList();

            // Validation the required switches.
            if (switchesWithoutValue.Count() > 0)
            {
                throw new Exception(string.Format("No value for the required switches:{0}", string.Join(",", switchesWithoutValue)));
            }

            var swithesValidationFailure = switchBag.Values.Where(t => !t.Validate()).Select(t => t.SwitchName + ":" + t.SwitchValue).ToList();

            // Validate the switch values.
            if (swithesValidationFailure.Count() > 0)
            {
                throw new Exception(string.Format("switch values validation failure: {0}", string.Join(",", swithesValidationFailure)));
            }

            Logger.SingleInstance.PrintAndLogMsg("switches and values validated.");
        }

        /// <summary>
        /// Republish all current instance in configure to database
        /// </summary>
        private static void RepublishInstance()
        {
            Logger.SingleInstance.PrintAndLogMsg("Start to retrieve the published data sources");
            var instanceList = COEInstanceBOList.GetCOEInstanceBOList();

            Logger.SingleInstance.PrintAndLogMsg(string.Format("{0} data sources(s) in total.\n", instanceList.Count));

            foreach (var instance in instanceList)
            {
                if (string.IsNullOrEmpty(instance.Password))
                {
                    instance.Password = GetPasswordForInstance(instance);
                }

                PublishInstance(instance);
            }

            Logger.SingleInstance.LogOnly("Data source republishing subflow completed.");
            Logger.SingleInstance.PrintAndLogMsg("Data Source republishing result:");
            Logger.SingleInstance.PrintAndLogInfo(string.Format(" Success - {0}", instanceList.Count - instancesPublishFailure.Count));
            Logger.SingleInstance.PrintAndLogError(string.Format(" Failure - {0}", instancesPublishFailure.Count), true);
        }

        /// <summary>
        /// Writes the content to the output file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="content">The content.</param>
        private static void WriteOutput(string filePath, string content)
        {
            if (File.Exists(filePath))
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }

            using (var fileWriter = new StreamWriter(filePath))
            {
                fileWriter.Write(content);
            }
        }

        /// <summary>
        /// Crypt or decrypt the password.
        /// </summary>
        private void PasswordCryptOrDecryptSubflow()
        {
            var usage = switchBag[Consts.SwitchBagKeyUsage].SwitchValue;
            var password = switchBag[Consts.SwitchBagKeyPassword].SwitchValue;
            var outputPath = switchBag[Consts.SwitchBagKeyOutputPath].SwitchValue;
            bool isConsole = string.IsNullOrEmpty(outputPath);

            string result = string.Empty;

            if (usage.ToUpper() == "ENCRYPT")
            {
                result = Utilities.IsRijndaelEncrypted(password) ? password : Utilities.EncryptRijndael(password);
            }
            else
            {
                result = Utilities.IsRijndaelEncrypted(password) ? Utilities.DecryptRijndael(password) : password;
            }

            if (isConsole)
            {
                Console.WriteLine(result);
            }
            else
            {
                WriteOutput(outputPath, result);

                // Exit the application so unblock the process.
                System.Environment.Exit(0);
            }
        }

        /// <summary>
        /// Tests the accessibility of COEDB.
        /// </summary>
        private void TestCOEDBAccessibilitySubflow()
        {
            const string ConnectionStrPattern = "user id={0}; password={1}; Data source={2}";
            string serviceName = string.Empty;
            string globalUser = string.Empty;

            Util.AnalyseMainGlobalUserAndDataSource(ref globalUser, ref serviceName);
            string password = Util.GetGlobalUserPassword(globalUser);
            password = Utilities.IsRijndaelEncrypted(password) ? Utilities.DecryptRijndael(password) : password;

            var connectionStr = string.Format(ConnectionStrPattern, globalUser, password, serviceName);

            var database = new OracleDatabase(connectionStr);
            DbConnection connection = null;

            var outputPath = switchBag[Consts.SwitchBagKeyOutputPath].SwitchValue;
            bool isConsole = string.IsNullOrEmpty(outputPath);
            string testResult = "Failed";

            try
            {
                // Try to open the database.
                connection = database.CreateConnection();
                connection.Open();
                testResult = "PASS";
            }
            catch (Exception ex)
            {
                if (isConsole)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            if (isConsole)
            {
                Console.Write("Database connection test result:" + testResult);
            }
            else
            {
                WriteOutput(outputPath, testResult);

                // Exit the application so unblock the process.
                System.Environment.Exit(0);
            }
        }

        /// <summary>
        /// Publish the instance to spotfire.
        /// </summary>
        private void PublishInstanceSubflow()
        {
            // Validate the command switches and values.
            ValidateSwitches();

            // Create global schema on data source and grant permission.
            EnableDataSource();

            string dataSource = string.Empty;
            var instanceBO = BuildInstanceBO(ref dataSource);

            // Display the instanceBO will be publish.
            instanceBO.Display();

            // Publish the instance to spotfire.
            if (!instanceBO.PublishToSpotfire())
            {
                throw new Exception("Publish data source failure");
            }

            instanceBO.RegisterToCfg(dataSource);

            Logger.SingleInstance.PrintAndLogInfo("Data source is published successfully");
        }

        /// <summary>
        /// Republish the data source and data views.
        /// </summary>
        private void RepublishSubflow()
        {
            if (switchBag[Consts.SwitchBagKeyRepublishAllDatasources].SwitchValue.Equals(
                "True", StringComparison.InvariantCultureIgnoreCase))
            {
                RepublishInstance();
            }

            if (switchBag[Consts.SwitchBagKeyRepublishAllDataviews].SwitchValue.Equals(
                "True", StringComparison.InvariantCultureIgnoreCase))
            {
                Util.PublishExistingDataviews();
            }
        }

        /// <summary>
        /// Create the global user and grant permissions.
        /// </summary>
        private void EnableDataSource()
        {
            var sysPasswordFromCommandline = switchBag[Consts.SwitchBagKeyDatabaseSysUserPassword].SwitchValue;
            var globalUserPasswordFromCommandLine = switchBag[Consts.SwitchBagKeyDatabaseGlobalUserPassword].SwitchValue;

            var sysPasswordDecrypted = Utilities.IsRijndaelEncrypted(sysPasswordFromCommandline) ?
                Utilities.DecryptRijndael(sysPasswordFromCommandline) : sysPasswordFromCommandline;

            var globalUserPasswordDecrypted = Utilities.IsRijndaelEncrypted(globalUserPasswordFromCommandLine) ?
                Utilities.DecryptRijndael(globalUserPasswordFromCommandLine) : globalUserPasswordFromCommandLine;

            // Use the network alias as the service name when enable the global schema.
            // If the network alias is empty, use the sid by default.
            var networkAlias = switchBag[Consts.SwitchBagKeyNetworkAlias].SwitchValue;

            if (string.IsNullOrEmpty(networkAlias))
            {
                networkAlias = switchBag[Consts.SwitchBagKeySID].SwitchValue;
            }

            int exitCode = Util.EnableDataSource(
                networkAlias,
                switchBag[Consts.SwitchBagKeyDatabaseSysUser].SwitchValue,
                sysPasswordDecrypted,
                switchBag[Consts.SwitchBagKeyDatabaseGlobalUser].SwitchValue,
                globalUserPasswordDecrypted);

            var errorMsg = string.Empty;

            if (exitCode == 0)
            {
                Logger.SingleInstance.PrintAndLogMsg("Global user created and permission granted");
                return;
            }
            else if (exitCode == 1017)
            {
                var sysUser = switchBag[Consts.SwitchBagKeyDatabaseSysUser].SwitchValue;
                errorMsg = string.Format("Logon failure, database user {0} or password is incorrect", sysUser);

                Logger.SingleInstance.PrintAndLogError(errorMsg);
            }
            else
            {
                errorMsg = string.Format("Enable global schema on datat source error with exit code '{0}'", exitCode);
                Logger.SingleInstance.PrintAndLogError(errorMsg);
            }

            throw new Exception(errorMsg);
        }
    }
}
