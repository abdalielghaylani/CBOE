using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;

using Csla;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.Framework.COEDatabasePublishingService
{
    public class COEInstanceBO : Csla.BusinessBase<COEInstanceBO>
    {
        private Guid _id = Guid.Empty;
        private string _instanceName;
        private DBMSType _dbmsType;
        private string _databaseGlobalUser;
        private string _password;
        private bool _isCBOEInstance;
        private bool _useProxy;
        private string _hostName;
        private int _port;
        private string _sid;
        private string _oldDefaultDatabase;
        private DriverType _driverType;

        //variables data access
        [NonSerialized]
        internal DAL _coeDAL = null;

        [NonSerialized]
        internal DAL _instanceDAL = null;

        [NonSerialized]
        internal DALFactory _dalFactory = new DALFactory();
        internal string _serviceName = "COEDatabasePublishing";

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEDatabasePublishing");

        internal COEInstanceBO()
        {
        }

        public COEInstanceBO(Guid instanceId, string instanceName, DBMSType dbmsType, string databaseGlobalUser, bool isCBOEInstance,
            bool useProxy, string password, string hostName, int port, string sid, DriverType driverType)
        {
            Id = instanceId;
            InstanceName = instanceName;
            DbmsType = dbmsType;
            DatabaseGlobalUser = databaseGlobalUser;
            IsCBOEInstance = isCBOEInstance;
            UseProxy = useProxy;
            Password = password;
            HostName = hostName;
            Port = port;
            SID = sid;
            DriverType = driverType;
        }

        #region Properties
        protected override object GetIdValue()
        {
            return _id;
        }

        public Guid Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public string InstanceName
        {
            get { return _instanceName; }
            set { _instanceName = value; }
        }

        public DBMSType DbmsType
        {
            get { return _dbmsType; }
            set { _dbmsType = value; }
        }

        public string DatabaseGlobalUser
        {
            get { return _databaseGlobalUser; }
            set { _databaseGlobalUser = value; }
        }

        public string Password
        {
            get { return _password; }
            set 
            {
                if (Common.Utilities.IsRijndaelEncrypted(value))
                {
                    _password = value;
                }
                else
                {
                    _password = Common.Utilities.EncryptRijndael(value);
                }
            }
        }

        public bool IsCBOEInstance
        {
            get { return _isCBOEInstance; }
            set { _isCBOEInstance = value; }
        }

        public bool UseProxy
        {
            get { return _useProxy; }
            set { _useProxy = value; }
        }

        public string HostName
        {
            get { return _hostName; }
            set { _hostName = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public string SID
        {
            get { return _sid; }
            set { _sid = value; }
        }

        public string OldDefaultDatabase
        {
            get { return _oldDefaultDatabase; }
            set { _oldDefaultDatabase = value; }
        }

        public DriverType DriverType
        {
            get { return _driverType; }
            set { _driverType = value; }
        }
        #endregion

        #region Methods

        internal static void SetDatabaseName()
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));
        }

        internal static void SetDatabaseName(string databaseName)
        {
            COEDatabaseName.Set(DALUtils.GetDefaultQualifyDbName(Resources.CentralizedStorageDB));
        }

        public COEInstanceBO Publish()
        {
            try
            {
                SetDatabaseName();
                ValidateSQLConnectString();

                COEInstanceBO returnBO = DataPortal.Create<COEInstanceBO>(new CreateInstanceCriteria(this.Id, this.InstanceName, this.DbmsType,
                    this.DatabaseGlobalUser, this.IsCBOEInstance, this.UseProxy, this.Password, this.HostName, this.Port, this.SID, this.DriverType));

                return returnBO;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public COEInstanceBO Update()
        {
            try
            {
                SetDatabaseName();
                ValidateSQLConnectString();

                MarkOld();
                return DataPortal.Update<COEInstanceBO>(this);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public COEInstanceBO Delete()
        {
            if (_isCBOEInstance)
            {
                throw new Exception("Can't delete CBOE primary data source!");
            }

            SetDatabaseName();
            DataPortal.Delete(new DeleteInstanceCriteria(_instanceName,_databaseGlobalUser));
            return this;
        }
        
        #endregion

        #region Data Access

        private void DataPortal_Create(CreateInstanceCriteria criteria)
        {
            AddToConfig(criteria);
        }

        private void CreateIntanceMetas(CreateInstanceCriteria criteria)
        {
            string decryptedPassword = string.Empty;

            if (Common.Utilities.IsRijndaelEncrypted(criteria._password))
            {
                decryptedPassword = Common.Utilities.DecryptRijndael(criteria._password);
            }
            else
            {
                decryptedPassword = criteria._password;
            }

            ((OracleDataAccessClientDAL)_instanceDAL).CreateInstanceMetas(criteria._databaseGlobalUser, decryptedPassword, criteria._hostName, criteria._port, criteria._sid);
        }

        protected override void DataPortal_Insert()
        {
            UpdateConfig();
        }

        protected override void DataPortal_Update()
        {
            UpdateConfig();
        }

        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new DeleteInstanceCriteria(_instanceName, _databaseGlobalUser));
        }

        private void DataPortal_Delete(DeleteInstanceCriteria criteria)
        {
            // Step1 check wether the datasource was used by existing dataviews/master dataview or not
            var dataViewBOList = COEDataViewService.COEDataViewBOList.GetAllDataViewListIncludeMasterPrivate();

            foreach (COEDataViewService.COEDataViewBO dataViewBO in dataViewBOList)
            {
                var tableCount = dataViewBO.COEDataView.Tables
                                    .Where(t => t.Database.StartsWith(criteria._instanceName + ".", StringComparison.InvariantCultureIgnoreCase))
                                    .ToList();

                if (tableCount.Count > 0)
                {
                    throw new Exception("Current data source is used by existing dataviews or Master dataview, please delete related dataviews or schema first!");
                }
            }

            try
            {
                LoadDAL();
                _coeDAL.DALManager.BeginTransaction();

                // Step2 delete intance schema from COEDB.COEDATABASE
                _coeDAL.DeleteInstanceSchemas(criteria._instanceName);

                _coeDAL.DALManager.CommitTransaction();

                // Step3 delete corresponding config
                DeleteConfig(criteria);
            }
            catch (Oracle.DataAccess.Client.OracleException oracleEx)
            {
                _coeDAL.DALManager.RollbackTransaction();
                throw new Exception("Error happened during updating database, Please contact to administrator for more detail!");
            }
            catch (Exception ex)
            {
                throw new Exception("Error happened during deleting data source, Please contact to administrator for more detail!");
            }
        }

        private void LoadDAL()
        {
            if (_dalFactory == null) 
            { 
                _dalFactory = new DALFactory(); 
            }

            _dalFactory.GetDAL<CambridgeSoft.COE.Framework.COEDatabasePublishingService.DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
        }

        private void LoadInstanceDAL(string intanceDatabaseName)
        {
            _instanceDAL = new OracleDataAccessClientDAL();
        }

        #endregion

        #region Criteria

        [Serializable()]
        protected class CreateInstanceCriteria
        {
            internal Guid _id;
            internal string _instanceName;
            internal DBMSType _dbmsType;
            internal string _databaseGlobalUser;
            internal string _password;
            internal bool _isCBOEInstance;
            internal bool _useProxy;
            internal string _hostName;
            internal int _port;
            internal string _sid;
            internal DriverType _driverType;

            public CreateInstanceCriteria(Guid instanceId, string instanceName, DBMSType dbmsType, string databaseGlobalUser,
                bool isCBOEInstance, bool useProxy, string password, string hostName, int port, string sid, DriverType driverType)
            {
                _id = instanceId;
                _instanceName = instanceName;
                _dbmsType = dbmsType;
                _databaseGlobalUser = databaseGlobalUser;
                _password = password;
                _isCBOEInstance = isCBOEInstance;
                _useProxy = useProxy;
                _hostName = hostName;
                _port = port;
                _sid = sid;
                _driverType = driverType;
            }
        }

        protected class Criteria
        {
            public Criteria()
            {
            }
        }

        protected class DeleteInstanceCriteria
        {
            internal string _instanceName;
            internal string _databaseGlobalUser;

            public DeleteInstanceCriteria(string instanceName, string databaseGlobalUser)
            {
                _instanceName = instanceName;
                _databaseGlobalUser = databaseGlobalUser;
            }
        }

        #endregion //Criteria

        #region Publishing methods

        private void AddToConfig(CreateInstanceCriteria criteria)
        {
            List<string> allInstancesList = ConfigurationUtilities.GetAllInstancesNameInConfig();

            if (allInstancesList.Contains(criteria._instanceName.ToUpper()))
            {
                throw new Exception(Resources.DataSourceExistsError);
            }

            InstanceData instanceData = new InstanceData();
            DatabaseData databasesData = new DatabaseData();
            COEConfigurationSettings connSettings = new COEConfigurationSettings();

            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);
            var xmlWriter = new XmlTextWriter(stringWriter);

            //Allocation of values to instanceData
            instanceData.Id = criteria._id;
            instanceData.Name = criteria._instanceName.ToUpper();
            instanceData.DBMSType = criteria._dbmsType;
            instanceData.DatabaseGlobalUser = criteria._databaseGlobalUser.ToUpper();
            instanceData.IsCBOEInstance = criteria._isCBOEInstance;
            instanceData.UseProxy = criteria._useProxy;
            instanceData.HostName = criteria._hostName;
            instanceData.Port = criteria._port;
            instanceData.SID = criteria._sid;
            instanceData.DriverType = criteria._driverType;

            COENamedElementCollection<SQLGeneratorData> sqlGeneratorData = new COENamedElementCollection<SQLGeneratorData>();
            sqlGeneratorData.Add(new SQLGeneratorData
            {
                Name = Properties.Resources.GeneratorData_CSORACLECARTRIDGE_NAME,
                Schema = Properties.Resources.GeneratorData_CSORACLECARTRIDGE_CSCARTRIDGE,
                TempQueries = Properties.Resources.GeneratorData_CSORACLECARTRIDGE_TempQueries,
                MolFileFormat = Properties.Resources.GeneratorData_CSORACLECARTRIDGE_MolFileFormat
            });

            instanceData.SQLGeneratorData = sqlGeneratorData;

            databasesData.Name = (criteria._instanceName + "." + criteria._databaseGlobalUser).ToUpper();
            databasesData.Owner = (criteria._databaseGlobalUser).ToUpper();
            databasesData.DBMSType = criteria._dbmsType;
            databasesData.InstanceId = instanceData.Id;
            databasesData.Password = criteria._password;
            databasesData.ProviderName = "Oracle.DataAccess.Client";
            databasesData.OracleTracing = false;
            databasesData.Tracing = false;

            string COEConfigPath = COEConfigurationManager.GetDefaultConfigurationFilePath();

            try
            {
                var xmldoc = new XmlDocument();
                xmldoc.Load(COEConfigPath);
                UpdateOrInsertInstanceElement(xmldoc, instanceData);
                UpdateOrInsertDatabaseElement(xmldoc, databasesData);

                File.SetAttributes(COEConfigPath, FileAttributes.Normal);
                xmldoc.Save(COEConfigPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateOrInsertInstanceElement(XmlDocument xmlDoc, InstanceData instanceData)
        {
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);
            var xmlWriter = new XmlTextWriter(stringWriter);
            var connSettings = new COEConfigurationSettings();

            const string instanceElementPath = "configuration/coeConfiguration/instances";

            connSettings.Instances.Add(instanceData);

            connSettings.WriteXml(xmlWriter);
            stringBuilder.Replace("<clear />", "");

            var tempXmlDoc = new XmlDocument();
            tempXmlDoc.LoadXml(stringBuilder.ToString());

            var instanceXml = tempXmlDoc.SelectSingleNode("SerializableConfigurationSection/instances").InnerXml;

            var tempEelment = xmlDoc.CreateElement("temp");
            tempEelment.InnerXml = instanceXml;

            var existingInstancesNode = xmlDoc.SelectSingleNode(instanceElementPath);

            XmlNode existingNode = null;
            for (int i = 0; i < existingInstancesNode.ChildNodes.Count; i++)
            {
                if (existingInstancesNode.ChildNodes[i].Attributes["name"].Value.Equals(
                    instanceData.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    existingNode = existingInstancesNode.ChildNodes[i];
                    break;
                }
            }

            if (existingNode != null)
            {
                existingInstancesNode.RemoveChild(existingNode);
            }

            xmlDoc.SelectSingleNode(instanceElementPath).AppendChild(tempEelment.FirstChild);
        }

        private void UpdateOrInsertDatabaseElement(XmlDocument xmlDoc, DatabaseData databaseData)
        {
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);
            var xmlWriter = new XmlTextWriter(stringWriter);
            var connSettings = new COEConfigurationSettings();

            const string databaseEelmentPath = "configuration/coeConfiguration/databases";

            connSettings.Databases.Add(databaseData);

            connSettings.WriteXml(xmlWriter);
            stringBuilder.Replace("<clear />", "");

            var tempXmlDoc = new XmlDocument();
            tempXmlDoc.LoadXml(stringBuilder.ToString());

            var instanceXml = tempXmlDoc.SelectSingleNode("SerializableConfigurationSection/databases").InnerXml;

            var tempEelment = xmlDoc.CreateElement("temp");
            tempEelment.InnerXml = instanceXml;

            var existingDatabaseNode = xmlDoc.SelectSingleNode(databaseEelmentPath);

            XmlNode existingNode = null;
            for (int i = 0; i < existingDatabaseNode.ChildNodes.Count; i++)
            {
                if (existingDatabaseNode.ChildNodes[i].Attributes["name"].Value.Equals(
                    databaseData.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    existingNode = existingDatabaseNode.ChildNodes[i];
                    break;
                }
            }

            if (existingNode != null)
            {
                existingDatabaseNode.RemoveChild(existingNode);
            }

            xmlDoc.SelectSingleNode(databaseEelmentPath).AppendChild(tempEelment.FirstChild);
        }

        private void UpdateConfig()
        {
            string COEConfigPath = COEConfigurationManager.GetDefaultConfigurationFilePath();

            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.Load(COEConfigPath);

            var instanceData = new InstanceData
            {
                Id = _id,
                InstanceName = _instanceName.ToUpper(),
                HostName = _hostName,
                Port = _port,
                SID = _sid,
                DBMSType = _dbmsType,
                IsCBOEInstance = _isCBOEInstance,
                UseProxy = _useProxy,
                DatabaseGlobalUser = _databaseGlobalUser.ToUpper(),
                DriverType = _driverType,
            };

            var globalDatabase = new DatabaseData
            {
                Name = (_instanceName + "." + _databaseGlobalUser).ToUpper(),
                InstanceId = _id,
                Owner = _databaseGlobalUser.ToUpper(),
                Password = _password,
                ProviderName = "Oracle.DataAccess.Client",
                OracleTracing = false,
                Tracing = false,
            };

            UpdateOrInsertInstanceElement(xmlDoc, instanceData);
            UpdateOrInsertDatabaseElement(xmlDoc, globalDatabase);

            File.SetAttributes(COEConfigPath, FileAttributes.Normal);
            xmlDoc.Save(COEConfigPath);
        }

        private void DeleteConfig(DeleteInstanceCriteria criteria)
        {
            string COEConfigPath = COEConfigurationManager.GetDefaultConfigurationFilePath();

            System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
            xmldoc.Load(COEConfigPath);

            XmlElement instancesNode = (XmlElement)xmldoc.DocumentElement.SelectSingleNode("coeConfiguration/instances");
            XmlElement databasesNode = (XmlElement)xmldoc.DocumentElement.SelectSingleNode("coeConfiguration/databases");

            var instanceIdWillBeDeleted = string.Empty;

            foreach (XmlElement xe in instancesNode.ChildNodes)
            {
                string name = xe.GetAttribute("name");
                if (name.Equals(criteria._instanceName, StringComparison.InvariantCultureIgnoreCase))
                {
                    instanceIdWillBeDeleted = xe.GetAttribute("id");

                    instancesNode.RemoveChild(xe);
                    break;
                }
            }

            if (!string.IsNullOrEmpty(instanceIdWillBeDeleted))
            {
                var instanceSchemaElements = new List<XmlElement>();
                // Remove all the databases under instance will be removed.
                foreach (XmlElement xe in databasesNode.ChildNodes)
                {
                    string instanceId = xe.GetAttribute("instanceId");
                    if (!string.IsNullOrEmpty(instanceId) && instanceId.Equals(instanceIdWillBeDeleted))
                    {
                        instanceSchemaElements.Add(xe);
                    }
                }

                foreach (var element in instanceSchemaElements)
                {
                    databasesNode.RemoveChild(element);
                }
            }

            File.SetAttributes(COEConfigPath, FileAttributes.Normal);
            xmldoc.Save(COEConfigPath);
        }

        private void ValidateSQLConnectString()
        {
            if (_coeDAL == null) { LoadDAL(); }

            if (_coeDAL != null)
            {
                if (!_coeDAL.AuthenticateUser(_databaseGlobalUser, Common.Utilities.DecryptRijndael(_password), _hostName, _port, _sid))
                {
                    throw new Exception("Can't connect to database!");
                }
            }
        }

        #endregion
    }
}
