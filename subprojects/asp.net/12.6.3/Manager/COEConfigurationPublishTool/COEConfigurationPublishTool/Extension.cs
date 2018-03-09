// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extension.cs" company="PerkinElmer Inc.">
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
    using System.IO;
    using System.Xml;

    using CambridgeSoft.COE.Framework.COEConfigurationService;
    using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
    using CambridgeSoft.COE.Framework.COEDataViewService;
    using CambridgeSoft.COE.Framework.Common;
    using Manager.Code;

    /// <summary>
    /// The class for all extension method
    /// </summary>
    public static class Extension
    {
        #region InstanceBO Extension Methods

        /// <summary>
        /// Publish instance
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>publish succeed or not</returns>
        public static bool PublishToSpotfire(this COEInstanceBO instance)
        {
            Logger.SingleInstance.PrintAndLogMsg(string.Format("Start to publish data source: {0}", instance.InstanceName));

            string result = SpotfireServiceClient.PublishDBInstance(instance);

            if (!string.IsNullOrEmpty(result))
            {
                Logger.SingleInstance.PrintAndLogError(string.Format("Fail!! \n{0}", result));
                return false;
            }
            else
            {
                Logger.SingleInstance.LogOnly("Publish to spotfire succeed!");
                return true;
            }
        }

        /// <summary>
        /// Register the instance data to configuration file.
        /// </summary>
        /// <param name="instanceBO">The instance data.</param>
        /// <param name="dataSource">The database data.</param>
        public static void RegisterToCfg(this COEInstanceBO instanceBO, string dataSource = "")
        {
            Logger.SingleInstance.LogOnly("Start to update the configuration file");

            InstanceData instanceData;
            DatabaseData databaseData;

            instanceBO.BuildInstanceData(out instanceData, out databaseData, dataSource);
            var instanceNodeXml = string.Format(
                Consts.InstanceNodeXmlFormat,
                instanceBO.Id,
                instanceData.InstanceName,
                instanceData.DBMSType,
                instanceData.DatabaseGlobalUser,
                instanceData.UseProxy,
                instanceData.HostName,
                instanceData.Port,
                instanceData.SID,
                instanceData.IsCBOEInstance,
                instanceData.DataSource,
                instanceData.DriverType);

            var databaseNodeXml = string.Format(
                Consts.DatabaseNodeXmlFormat,
                databaseData.Name,
                databaseData.InstanceId,
                databaseData.DBMSType,
                string.Empty,
                databaseData.Owner,
                databaseData.Password,
                databaseData.OracleTracing,
                databaseData.Tracing);

            var coeConfigPath = COEConfigurationManager.GetDefaultConfigurationFilePath();
            var xmldoc = new System.Xml.XmlDocument();
            xmldoc.Load(coeConfigPath);

            var coeConfigurationRoot = (XmlElement)xmldoc.DocumentElement.SelectSingleNode("coeConfiguration");
            var instancesNode = (XmlElement)xmldoc.DocumentElement.SelectSingleNode("coeConfiguration/instances");
            var databaseNode = (XmlElement)xmldoc.DocumentElement.SelectSingleNode("coeConfiguration/databases");

            var temp = xmldoc.CreateElement("temp");

            if (instancesNode == null)
            {
                instancesNode = xmldoc.CreateElement("instances");
                coeConfigurationRoot.PrependChild(instancesNode);
            }

            XmlNode instanceNodeExists = null;

            for (int i = 0; i < instancesNode.ChildNodes.Count; i++)
            {
                // Find if the instance already registered or not.
                if (instancesNode.ChildNodes[i].Attributes["name"].Value.Equals(
                    instanceData.InstanceName,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    instanceNodeExists = instancesNode.ChildNodes[i];
                }

                // Update the existing CBOE instance to non CBOE instance.
                if (instanceData.IsCBOEInstance &&
                    instancesNode.ChildNodes[i].Attributes["isCBOEInstance"].Value.Equals(
                    "true",
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    instancesNode.ChildNodes[i].Attributes["isCBOEInstance"].Value = "false";
                }
            }

            // Remove the existing ones.
            if (instanceNodeExists != null)
            {
                instancesNode.RemoveChild(instanceNodeExists);
            }

            temp.InnerXml = instanceNodeXml;

            if (!instanceData.IsCBOEInstance)
            {
                ((XmlElement)temp.FirstChild).RemoveAttribute("dataSource");
            }

            instancesNode.AppendChild(temp.FirstChild);

            XmlNode databaseNodeExists = null;

            // Find if the database already registered or not.
            for (int i = 0; i < databaseNode.ChildNodes.Count; i++)
            {
                if (databaseNode.ChildNodes[i].Attributes["name"].Value.Equals(
                    databaseData.Name,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    databaseNodeExists = databaseNode.ChildNodes[i];
                    break;
                }
            }

            // Remove the existing ones.
            if (databaseNodeExists != null)
            {
                databaseNode.RemoveChild(databaseNodeExists);
            }

            temp.InnerXml = databaseNodeXml;
            databaseNode.AppendChild(temp.FirstChild);

            File.SetAttributes(coeConfigPath, FileAttributes.Normal);
            xmldoc.Save(coeConfigPath);

            Logger.SingleInstance.LogOnly("Configuration file update successfully");
        }

        /// <summary>
        /// Display the instance BO data in console.
        /// </summary>
        /// <param name="instance">The instance data.</param>
        public static void Display(this COEInstanceBO instance)
        {
            Console.WriteLine();
            Console.WriteLine("       Data source Id: {0}", instance.Id);
            Console.WriteLine("     Data source name: {0}", instance.InstanceName);
            Console.WriteLine(" Spotfire driver type: {0}", instance.DriverType == DriverType.OracleDataDirect ? "Oracle (DataDirect)" : instance.DriverType.ToString());
            Console.WriteLine("            Host name: {0}", instance.HostName);
            Console.WriteLine("                  SID: {0}", instance.SID);
            Console.WriteLine("     Data source port: {0}", instance.Port);
            Console.WriteLine("          Global user: {0}", instance.DatabaseGlobalUser);
            Console.WriteLine();
        }

        /// <summary>
        /// Clone the new coe instance BO.
        /// </summary>
        /// <param name="instance">The instanceBO instance.</param>
        /// <returns>New instance of the InstanceBO</returns>
        public static COEInstanceBO Copy(this COEInstanceBO instance)
        {
            var newInstance = new COEInstanceBO(
                instance.Id,
                instance.InstanceName,
                instance.DbmsType,
                instance.DatabaseGlobalUser,
                instance.IsCBOEInstance,
                instance.UseProxy,
                instance.Password,
                instance.HostName,
                instance.Port,
                instance.SID,
                instance.DriverType);

            return newInstance;
        }

        /// <summary>
        /// Build the instanceBO, instance and database data base on the switches.
        /// </summary>
        /// <param name="instanceBO">The instanceBO instance build output.</param>
        /// <param name="instanceData">The instance build output.</param>
        /// <param name="databaseData">The database build output.</param>        
        /// <param name="dataSource">The data source</param>
        public static void BuildInstanceData(this COEInstanceBO instanceBO, out InstanceData instanceData, out DatabaseData databaseData, string dataSource = "")
        {
            Logger.SingleInstance.LogOnly("Start to build the data source and database BO");

            instanceData = new InstanceData
            {
                Id = instanceBO.Id,
                InstanceName = instanceBO.InstanceName.ToUpper(),
                DBMSType = instanceBO.DbmsType,
                DatabaseGlobalUser = instanceBO.DatabaseGlobalUser,
                UseProxy = instanceBO.UseProxy,
                HostName = instanceBO.HostName,
                Port = instanceBO.Port,
                SID = instanceBO.SID,
                DataSource = dataSource,
                IsCBOEInstance = instanceBO.IsCBOEInstance,
                DriverType = instanceBO.DriverType
            };

            databaseData = new DatabaseData
            {
                // The databaseName is depends on if the instance is MAIN or not.
                Name = instanceBO.IsCBOEInstance ? instanceBO.DatabaseGlobalUser : instanceBO.InstanceName + "." + instanceBO.DatabaseGlobalUser,
                InstanceId = instanceBO.Id,
                Owner = instanceBO.DatabaseGlobalUser,
                DBMSType = instanceBO.DbmsType,
                Password = instanceBO.Password,
                ProviderName = "Oracle.DataAccess.Client",
                OracleTracing = false,
                Tracing = false,                
            };

            Logger.SingleInstance.LogOnly("Data source and database BO builded");
        }

        #endregion

        #region Dataview Extension Methods.

        /// <summary>
        /// Publish data view
        /// </summary>
        /// <param name="dataview">The data view</param>
        public static void PublishToSpotfire(this COEDataViewBO dataview)
        {
            Logger.SingleInstance.PrintAndLogMsg(string.Format("Publishing dataview: {0} - {1}", dataview.ID, dataview.Name));

            var result = SpotfireServiceClient.PublishDataView(dataview);
            if (!string.IsNullOrEmpty(result))
            {
                throw new Exception(string.Format("Dataview '{0} - {1}' publishing failed with message:{2}", dataview.ID, dataview.Name, result));
            }

            Logger.SingleInstance.PrintAndLogInfo(
                string.Format("Dataview '{0} - {1}' published.", dataview.ID, dataview.Name));
        }
        
        #endregion
    }
}
