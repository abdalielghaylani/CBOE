// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InteractivePublishingWorkflow.cs" company="PerkinElmer Inc.">
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
    using System.Xml.Linq;
    using System.Xml.XPath;
    using CambridgeSoft.COE.Framework.COEConfigurationService;
    using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
    using CambridgeSoft.COE.Framework.Common;

    /// <summary>
    /// The work flow for full configuration for publishing 
    /// </summary>
    internal class InteractivePublishingWorkflow : WorkflowExecutor
    {
        /// <summary>
        /// The max retry time
        /// </summary>
        private const int MAXRETRYTIMES = 3;

        private string primaryDataSourceGlobalUser = Consts.DefaultMainInstanceGlobalDb;
        private string dataSourceFromDbmsType = string.Empty;

        /// <summary>
        /// Start to run the workflow to publish the instances and data view.
        /// </summary>
        internal override void Run()
        {
            // Check if main instance exists or not.
            var mainInstance = this.GetMainInstance();
            bool instanceRepublishRequired = true;

            // Get the global user name from old dbms type section node.
            Util.AnalyseMainGlobalUserAndDataSource(ref primaryDataSourceGlobalUser, ref dataSourceFromDbmsType);

            // Publish the main instance if not exists.
            if (mainInstance == null)
            {
                instanceRepublishRequired = false;
                Logger.SingleInstance.PrintAndLogMsg("Primary data source is not found, it needs to be published...");

                var defaultGlobalUserPassword = Util.GetGlobalUserPassword(primaryDataSourceGlobalUser);

                // Run the sql scripts to enable the data source.
                EnablePrimaryDataSource();

                mainInstance = new COEInstanceBO(
                    Guid.NewGuid(),
                    Consts.DefaultInstanceName,
                    DBMSType.ORACLE,
                    primaryDataSourceGlobalUser,
                    true,
                    true,
                    defaultGlobalUserPassword,
                    string.Empty,
                    1521,
                    string.Empty,
                    DriverType.Oracle);

                this.PublishInstance(mainInstance);

                Logger.SingleInstance.LogOnly("Primary data source published.");
            }
            else
            {
                Logger.SingleInstance.PrintAndLogMsg("Primary data source is found in configuration file.", true);
            }

            // Publish existing instances.
            if (!instanceRepublishRequired && COEInstanceBOList.GetCOEInstanceBOList().Count() == 1)
            {
                Logger.SingleInstance.PrintAndLogMsg("There is no additional data sources need to be republished.");
            }
            else
            {
                if (Util.ShowYesNoConsoleConfirmDialog("Would you like to republish the existing data sources?"))
                {
                    this.PublishExistingInstances();
                }
            }

            Console.WriteLine();

            if (Util.ShowYesNoConsoleConfirmDialog("Would you like to republish the existing dataviews?"))
            {
                Util.PublishExistingDataviews();
            }
        }

        /// <summary>
        /// Enable the primary data source by execute the database script.
        /// </summary>
        private void EnablePrimaryDataSource()
        {
            var defaultGlobalUser = System.Configuration.ConfigurationManager.AppSettings["DefaultGlobalUserName"];
            var defaultGlobalUserPassword = System.Configuration.ConfigurationManager.AppSettings["DefaultGlobalUserPassword"];
            var passwordDecrypted = Utilities.IsRijndaelEncrypted(defaultGlobalUserPassword) ? Utilities.DecryptRijndael(defaultGlobalUserPassword) : defaultGlobalUserPassword;

            while (true)
            {
                var sysUser = Util.GetInput<string>("Oracle account with system privileges", "system");
                var sysPassword = Util.GetInput<string>("Oracle account password", string.Empty, true);
                int exitCode = Util.EnableDataSource(
                    dataSourceFromDbmsType, sysUser, sysPassword, defaultGlobalUser, passwordDecrypted);

                if (exitCode == 0)
                {
                    break;
                }
                else if (exitCode == 1017)
                {
                    Logger.SingleInstance.PrintAndLogError("Username or password is incorrect, please type again.");
                }
                else
                {
                    Logger.SingleInstance.PrintAndLogError(string.Format("Enable data source error with exit code '{0}'", exitCode));
                }
            }
        }

        /// <summary>
        /// Republish the existing instances.
        /// </summary>
        private void PublishExistingInstances()
        {
            var coeInstanceList = COEInstanceBOList.GetCOEInstanceBOList().OrderByDescending(i => i.IsCBOEInstance);

            Logger.SingleInstance.PrintAndLogMsg(string.Format("There are {0} data source(s) in total.", coeInstanceList.Count()));
            Logger.SingleInstance.LogOnly("Start to publish data sources...");

            foreach (var instance in coeInstanceList)
            {
                try
                {
                    Logger.SingleInstance.PrintAndLogMsg(string.Format("Start to publish data source '{0}'", instance.InstanceName));
                    Logger.SingleInstance.PrintAndLogMsg(string.Format("Data source '{0}' detailed information as below:", instance.InstanceName));
                    instance.Display();

                    var publishingChoices = new string[] { "Modify", "Publish", "Skip" };

                    var choice = Util.ShowConsoleConfirmDialog("Please choose an option to publish the data source.", publishingChoices);

                    switch (char.ToUpper(choice))
                    {
                        case 'M':
                            this.PublishInstance(instance);
                            break;
                        case 'P':
                            {
                                int maxRetry = 3;
                                int currentRetry = 0;

                                while (currentRetry < maxRetry)
                                {
                                    if (instance.PublishToSpotfire())
                                    {
                                        Logger.SingleInstance.PrintAndLogInfo(string.Format("Data source '{0}' is published to Spotfire server", instance.InstanceName));
                                        break;
                                    }

                                    if (++currentRetry >= maxRetry)
                                    {
                                        Logger.SingleInstance.PrintAndLogMsg("Reach the max retries");
                                        Console.WriteLine();
                                        break;
                                    }
                                    else
                                    {
                                        if (!Util.ShowYesNoConsoleConfirmDialog("Would you like to retry?"))
                                        {
                                            break;
                                        }
                                    }
                                }

                                break;
                            }

                        case 'S':
                            Logger.SingleInstance.PrintAndLogMsg(
                            string.Format("Skip the data source '{0}'", instance.InstanceName));
                            break;
                        default:
                            throw new NotSupportedException(string.Format("Unknown choice '{0}'", choice));
                    }
                }
                catch (Exception ex)
                {
                    Logger.SingleInstance.PrintAndLogError(
                        string.Format("Data source '{0}' publishing failure with message:{1}", instance.InstanceName, ex.Message));
                    Logger.SingleInstance.PrintAndLogMsg("Continue next data source...");
                }
            }

            Logger.SingleInstance.LogOnly("Data source republishing subflow completed.");
        }

        /// <summary>
        /// Publish the instance in SPOTFIRE and configuration file. 
        /// The instance data will be retrieved by interactive with end user in console.
        /// </summary>
        /// <param name="instanceBO">
        /// The database instance BO.
        /// </param>
        private void PublishInstance(COEInstanceBO instanceBO)
        {
            int currentRetry = 0;

            if (string.IsNullOrEmpty(instanceBO.Password))
            {
                // Retrive the password from configuration file.
                var globalUserAlias = instanceBO.IsCBOEInstance ?
                    instanceBO.DatabaseGlobalUser : instanceBO.InstanceName + "." + instanceBO.DatabaseGlobalUser;
                instanceBO.Password = Util.GetGlobalUserPassword(globalUserAlias);
            }

            while (currentRetry <= MAXRETRYTIMES)
            {
                currentRetry++;
                var newInstance = Util.BuildCOEInstanceByConsoleDialog(instanceBO);
                Console.WriteLine();
                Console.WriteLine("Data source information you entered as below:");
                newInstance.Display();
                if (!Util.ShowYesNoConsoleConfirmDialog("Publish the data source as above?"))
                {
                    Console.WriteLine("will re-config the data source...");
                    continue;
                }

                Logger.SingleInstance.LogOnly("Start to publish data source to Spotfire.");

                if (instanceBO.PublishToSpotfire())
                {
                    Logger.SingleInstance.LogOnly("Data source published to Spotfire successfully.");
                    Logger.SingleInstance.LogOnly("Start to register the data source and global database in configuration file");
                    instanceBO.RegisterToCfg(dataSourceFromDbmsType);
                    Logger.SingleInstance.LogOnly("Data source and global database user registered in configuration file.");
                    Logger.SingleInstance.PrintAndLogInfo(string.Format("Data source '{0}' is published successfully", instanceBO.InstanceName));

                    break;
                }
                else
                {
                    if (!Util.ShowYesNoConsoleConfirmDialog("Data source publishing failed. Would you like to retry it?"))
                    {
                        throw new Exception("User abort it, will exit.");
                    }

                    continue;
                }
            }

            if (currentRetry > MAXRETRYTIMES)
            {
                Logger.SingleInstance.PrintAndLogError("Limit retry exceed, will exist...");
                throw new Exception("Limit retry exceed");
            }
        }

        /// <summary>
        /// Try to get the main instance BO.
        /// </summary>
        /// <returns>
        /// Gets the COE main instance. If not found, return null.
        /// </returns>
        private COEInstanceBO GetMainInstance()
        {
            var coeConfigPath = COEConfigurationManager.GetDefaultConfigurationFilePath();
            XDocument doc = XDocument.Load(coeConfigPath);

            var instancesElement = doc.XPathSelectElements("configuration/coeConfiguration/instances").FirstOrDefault();

            if (instancesElement != null)
            {
                var instances = COEInstanceBOList.GetCOEInstanceBOList();

                if (instances != null)
                {
                    return instances.Where(i => i.IsCBOEInstance == true).FirstOrDefault();
                }
            }

            return null;
        }
    }
}
