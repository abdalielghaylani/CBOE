using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using Csla;
using Csla.Data;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COESecurityService
{
    /// <summary>
    /// Wrapper class for COEConnection to expose readonly information
    /// </summary>
    [Serializable]
    public class COEConnectionInfo
    {
        #region Variables
        private COEConnection _coeConnection;
        private COEIdentity _coeIdentity;
        private string _databaseName = Resources.CentralizedStorageDB;
        private string _serviceName = "COESecurity";
        #endregion

        #region Static variables
        private static COEConnectionInfo _coeConnectionInfo;
        private static object syncLockObject = new object(); 
        #endregion

        #region Properties
        /// <summary>
        /// Gets the database name
        /// </summary>
        public string DatabaseName
        {
            get { return _databaseName; }
        }

        /// <summary>
        /// Gets the service name
        /// </summary>
        public string ServiceName
        {
            get { return _serviceName; }
        }

        /// <summary>
        /// Gets the server framework version
        /// </summary>
        public Version ServerFrameworkVersion
        {
            get { return _coeConnection.VersionInfo.ServerFrameworkVersion; }
        }

        /// <summary>
        /// Gets the minimum required client framework version
        /// </summary>
        public Version MinRequiredClientFrameworkVersion
        {
            get { return _coeConnection.VersionInfo.MinRequiredClientFrameworkVersion; }
        }

        /// <summary>
        /// Gets the minimum oracle schema version
        /// </summary>
        public Version MinOracleSchemaVersion
        {
            get { return _coeConnection.VersionInfo.MinOracleSchemaVersion; }
        }

        /// <summary>
        /// Gets the server oracle schema version
        /// </summary>
        public Version ServerOracleSchemaVersion
        {
            get { return _coeConnection.VersionInfo.ServerOracleSchemaVersion; }
        }

        /// <summary>
        /// Gets the client framework version
        /// </summary>
        public Version ClientFrameworkVersion
        {
            get { return _coeConnection.VersionInfo.ClientFrameworkVersion; }
        }

        /// <summary>
        /// Gets boolean true if framework version is greater than or equal to the minimum server framework version required else false
        /// </summary>
        public bool FrameworkVersionsAreCompatible
        {
            get
            {
                return _coeConnection.FrameworkVersionsAreCompatible;
            }
        }

        /// <summary>
        /// Gets boolean true if oracle schema version is greater than or equal to the minimum oracle schema version required else false
        /// </summary>
        public bool OracleSchemasAreCompatible
        {
            get
            {
                return _coeConnection.OracleSchemasAreCompatible;
            }
        }

        /// <summary>
        /// Gets the session ID
        /// </summary>
        public int SessionID
        {
            get
            {
                return _coeConnection.SessionID;
            }
        }
        #endregion

        /// <summary>
        /// Initializes an instance of COEConnectionInfo object
        /// </summary>
        private COEConnectionInfo(COEIdentity coeIdentity)
        {
            this._coeIdentity = coeIdentity;
            this._coeConnection = _coeIdentity.COEConnection;
        }

        /// <summary>
        /// Instance method for implementing singleton pattern
        /// </summary>
        /// <param name="coeIdentity">identity object</param>
        /// <returns>returns initialized instance of the COEConnectionInfo</returns>
        internal static COEConnectionInfo Instance(COEIdentity coeIdentity)
        {
            lock(syncLockObject)
            {
                if(_coeConnectionInfo == null)
                {
                    _coeConnectionInfo = new COEConnectionInfo(coeIdentity);
                }
                return _coeConnectionInfo;
            }
        }
    }


    [Serializable]
    public class COEConnection
    {

       
        
        private string _databaseName = Resources.CentralizedStorageDB;
        private string _serviceName = "COESecurity";
        private Version _clientVersion = null;
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COESecurity");
        private VersionInfo _versionInfo = null;
        private int _sessionID = -1;
        public COEConnection()
        {
        }

         public VersionInfo VersionInfo
        {
            get { return _versionInfo; }
            set { _versionInfo = value; }
        }

        public int SessionID
        {
            get { return _sessionID; }
            set { _sessionID = value; }
        }

        public Version ClientFrameworkVersion
        {
            get { return _clientVersion; }
            set { _clientVersion = value; }
        }


         public bool OracleSchemasAreCompatible
         {
             get { 
                 
                 bool compatible= false;
                 compatible = _versionInfo.ServerOracleSchemaVersion >= _versionInfo.MinOracleSchemaVersion;
                 return compatible; }
        }

        public bool FrameworkVersionsAreCompatible
        {
            get
            {

                bool compatible = false;
                compatible = _versionInfo.ClientFrameworkVersion >= _versionInfo.MinRequiredClientFrameworkVersion;
                return compatible;
            }

        }

        public void StartSession(int userID)
        {
            StartSessionCommand result;
            try
            {
                result = DataPortal.Execute<StartSessionCommand>(new StartSessionCommand(userID));
                Csla.ApplicationContext.GlobalContext["COESESSIONID"] = result.SessionID;

            }
            catch(System.Exception ex)
            {

            }
            _sessionID = COEUser.SessionID;
        }

        public void EndSession(int sessionID)
        {
            EndSessionCommand result;
            result = DataPortal.Execute<EndSessionCommand>(new EndSessionCommand(sessionID));
        }


        internal Version GetClientFrameworkVersion()
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyFileVersionAttribute clientFrameworkVersion = (System.Reflection.AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(assembly, typeof(System.Reflection.AssemblyFileVersionAttribute));
            Attribute[] attributes = (Attribute[])this.GetType().Assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyFileVersionAttribute), true);
            if (attributes.Length > 0) { clientFrameworkVersion = attributes[0] as System.Reflection.AssemblyFileVersionAttribute; };
            return new Version(clientFrameworkVersion.Version);
        }

        public void CheckFrameworkCompatibility()
        {
            try
            {
                GetVersionInfoCommand result;
                try
                {
                    result = DataPortal.Execute<GetVersionInfoCommand>(new GetVersionInfoCommand());
                }
                
                catch (InvalidConnection)
                {
                    throw;
                }
                this.VersionInfo = new VersionInfo(result.VersionInfo.ServerFrameworkVersion, result.VersionInfo.MinRequiredClientFrameworkVersion, result.VersionInfo.MinOracleSchemaVersion, result.VersionInfo.ServerOracleSchemaVersion, this.ClientFrameworkVersion);
               
            
            }
            catch (Exception ex)
            {

                throw;
            }
        }
      


       
      


        [Serializable]
       
        private class GetVersionInfoCommand : CommandBase
        {
            private VersionInfo _versionInfo = null;
            

            public GetVersionInfoCommand()
            {
            }

            public VersionInfo VersionInfo
            {
                get { return _versionInfo; }
                set { _versionInfo = value; }
            }


            protected override void DataPortal_Execute()
            {
                
                    //get _serverFrameworkVersion from assembly

                    Assembly assembly = Assembly.GetExecutingAssembly();

                    //get server framework version from server side assembly
                    System.Reflection.AssemblyFileVersionAttribute serverFrameworkVersion = (System.Reflection.AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(assembly, typeof(System.Reflection.AssemblyFileVersionAttribute));
                    Attribute[] attributes =(Attribute[])this.GetType().Assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyFileVersionAttribute), true);
                    if (attributes.Length > 0) {  
                        serverFrameworkVersion = attributes[0] as System.Reflection.AssemblyFileVersionAttribute; 
                    }

                    //get minclientversion from assembly from server side assembly
                    AssemblyMinClientVersion minClientVersion = (AssemblyMinClientVersion)Attribute.GetCustomAttribute(assembly, typeof(AssemblyMinClientVersion));
                    Attribute[] attributes2 =(Attribute[])this.GetType().Assembly.GetCustomAttributes(typeof(AssemblyMinClientVersion), true);
                    if (attributes2.Length > 0) {
                        minClientVersion = attributes2[0] as AssemblyMinClientVersion; 
                    }
                

                    //get minoracleversion from assembly from server side assembly
                    AssemblyMinSchemaVersion minSchemaVersion = (AssemblyMinSchemaVersion)Attribute.GetCustomAttribute(assembly, typeof(AssemblyMinSchemaVersion));
                    Attribute[] attributes3 =(Attribute[])this.GetType().Assembly.GetCustomAttributes(typeof(AssemblyMinSchemaVersion), true);
                    if (attributes3.Length > 0) {  
                        minSchemaVersion = attributes3[0] as AssemblyMinSchemaVersion; 
                     }
                     Version frameworkSchemaVersion = null;

                     //Load securityDal and get schema version from server side database
                     DAL _coeDAL = null;
                     DALFactory _dalFactory = new DALFactory();
                     try
                          {
                       

                        if (_coeDAL == null)
                        {
                            _dalFactory.GetDAL<CambridgeSoft.COE.Framework.COESecurityService.DAL>(ref _coeDAL, "COESecurity", Resources.CentralizedStorageDB, true);

                        }
                        frameworkSchemaVersion = _coeDAL.GetFrameworkSchemaVersion();
                    }
                    catch (System.Exception ex)
                    {
                        throw;
                    }

                    _versionInfo = new VersionInfo(new Version(serverFrameworkVersion.Version), minClientVersion.Version, minSchemaVersion.Version, frameworkSchemaVersion);


                  

                    }


        }
        [Serializable]
        private class StartSessionCommand : CommandBase
        {
            private int _sessionID = -1;
            private int _userID = -1;
           

            public StartSessionCommand(int userID)
            {
                _userID = userID;
            }


            public int SessionID
            {
                get { return _sessionID; }
                set { _sessionID = value; }
            }


            protected override void DataPortal_Execute()
            {
                try
                {
                    //Load securityDal and get schema version from server side database
                    DAL _coeDAL = null;
                    DALFactory _dalFactory = new DALFactory();

                    if (_coeDAL == null)
                    {
                        _dalFactory.GetDAL<CambridgeSoft.COE.Framework.COESecurityService.DAL>(ref _coeDAL, "COESecurity", Resources.CentralizedStorageDB, true);

                    }
                    _sessionID = _coeDAL.StartSession(_userID);
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            private void LoadDAL()
            {

            }

        }
        [Serializable]
        private class EndSessionCommand : CommandBase
        {
            private int _sessionID = -1;
            private bool _sessionEnded = false;
           

            public EndSessionCommand(int sessionID)
            {
                _sessionID = sessionID;
            }


            public bool sessionEnded
            {
                get { return _sessionEnded; }
                set { _sessionEnded = value; }
            }


            protected override void DataPortal_Execute()
            {
                try
                {
                    //Load securityDal and get schema version from server side database
                    DAL _coeDAL = null;
                    DALFactory _dalFactory = new DALFactory();

                    if (_coeDAL == null)
                    {
                        _dalFactory.GetDAL<CambridgeSoft.COE.Framework.COESecurityService.DAL>(ref _coeDAL, "COESecurity", Resources.CentralizedStorageDB, true);

                    }
                    _coeDAL.EndSession(_sessionID);
                    _sessionEnded = true;
                }
                catch (Exception ex)
                {
                    _sessionEnded = false;
                    throw;
                }
            }

           

        }

       

   
    }
}

