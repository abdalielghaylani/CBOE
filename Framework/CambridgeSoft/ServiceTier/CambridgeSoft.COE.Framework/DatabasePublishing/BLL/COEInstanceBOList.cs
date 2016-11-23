using System;
using System.Data;
using Csla;
using Csla.Data;
using Csla.Validation;
using System.Collections.Generic;
using System.Configuration;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Properties;
using System.ComponentModel;
using CambridgeSoft.COE.Framework.COEConfigurationService;

namespace CambridgeSoft.COE.Framework.COEDatabasePublishingService
{
    [Serializable()]
    public class COEInstanceBOList : Csla.BusinessListBase<COEInstanceBOList, COEInstanceBO>
    {
        //variables data access
        [NonSerialized]
        private DAL _coeDAL = null;
        [NonSerialized]
        private DAL _instanceDAL = null;

        [NonSerialized]
        private DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COEDatabasePublishing";
        private string _databaseName;
        static COELog _coeLog = COELog.GetSingleton("COEDatabasePublishing");
        #region Factory Methods

        //this method must be called prior to any other method inorder to set the database that the dal will use
        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));
        }


        internal static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));           
        }

        private COEInstanceBOList()
        {
        }

        public static COEInstanceBOList NewList()
        {
            SetDatabaseName();
            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COEInstanceBOList");
            return new COEInstanceBOList();
        }

        public COEInstanceBO GetInstance(string name)
        {
            foreach (COEInstanceBO instance in GetCOEInstanceBOList())
                if (instance.InstanceName == name)
                    return instance;

            return null;
        }

        public static COEInstanceBOList GetCOEInstanceBOList()
        {
            string COEConfigPath = COEConfigurationManager.GetDefaultConfigurationFilePath();
            var exeConfigurationFileMap = new ExeConfigurationFileMap();
            COEInstanceBOList instanceBOList = new COEInstanceBOList();

            exeConfigurationFileMap.ExeConfigFilename = COEConfigPath;

            var configuration = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);
            var configSection = (COEConfigurationSettings)configuration.GetSection("coeConfiguration");

            foreach (InstanceData instanceData in configSection.Instances)
            {
                try
                {
                    COEInstanceBO instanceBO = new COEInstanceBO(instanceData.Id, instanceData.InstanceName, instanceData.DBMSType,
                        instanceData.DatabaseGlobalUser, instanceData.IsCBOEInstance, instanceData.UseProxy,
                        string.Empty, instanceData.HostName, instanceData.Port, instanceData.SID, instanceData.DriverType);

                    instanceBOList.Add(instanceBO);
                }
                catch (Exception e)
                {
                    throw;
                }
            }

            return instanceBOList;
        }

        public static bool CanAddObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }

        public static bool CanGetObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }
        public static bool CanEditObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }
        public static bool CanDeleteObject()
        {
            //TO DO:  need to add authorization access code
            return true;
        }

        #endregion //Factory Methods

        public COEInstanceBOList InstanceBOList { get; private set; }
    }
}
