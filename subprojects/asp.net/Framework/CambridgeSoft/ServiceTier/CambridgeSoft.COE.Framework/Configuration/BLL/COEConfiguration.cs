using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Csla;
using Csla.Data;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COEConfigurationService
{
    //this object is not  a business object since
    //we are not directly working with CRUD operations
    //Instead this object will be more for containing command objects which will create 
    //calls to the Configuration Manager to execute it's functionality
    public class COEConfiguration
    {

        public COEConfiguration()
        {
        }

        #region GetAllAppNamesInConfig
        /// <summary>
        /// Command object for Retrieving all the AppNames reading from the configuration file that is associated with the framework located in  C:\Documents and Settings\All Users\Application Data\CambridgeSoft\ChemOfficeEnterprisexx.x.x.x COEFrameworkConfig.xml
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllAppNamesInConfig()
        {
            try
            {
                //this doesn't make a call to any database so we don't need the appname
                //Csla.ApplicationContext.GlobalContext["AppName"] = appName;
                GetAllAppNamesInConfigCommand result;
                //this is returned the DoSearchCommand object so you can call methods within it
                result = DataPortal.Execute<GetAllAppNamesInConfigCommand>(new GetAllAppNamesInConfigCommand());
                return result.MyAppNameList;
            }
            catch (Exception)
            {

                throw;
            }
        }



        //create nested command object that calls the doSearch command. Two additional objects need to be added
        //GetHitList and GetData
        [Serializable]
        private class GetAllAppNamesInConfigCommand : CommandBase
        {

            //what is returned from the command
            private List<string> _myAppNameList;


            public GetAllAppNamesInConfigCommand()
            {

                //constructor to set properties that match input paramters

            }

            public List<string> MyAppNameList
            {
                get { return _myAppNameList; }
                set { _myAppNameList = value; }
            }


            protected override void DataPortal_Execute()
            {
                try
                {
                    //the heart of the command. In this case it is simply calling a command in ConfigurationUlities, but it could also call a Manager method, or be
                    //a completely new method that calls nothing elase
                    _myAppNameList = new List<string>();
                    _myAppNameList = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetAllAppNamesInConfig();


                }
                catch (Exception)
                {

                    throw;
                }
            }



        }

        #endregion
        #region GetAllDatabaseNamesInConfig
        /// <summary>
        /// Command object for Retrieving all the AppNames reading from the configuration file that is associated with the framework located in  C:\Documents and Settings\All Users\Application Data\CambridgeSoft\ChemOfficeEnterprisex.x.x.x COEFrameworkConfig.xml
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllDatabaseNamesInConfig()
        {
            try
            {
                //this doesn't make a call to any database so we don't need the appname
                //Csla.ApplicationContext.GlobalContext["AppName"] = appName;
                GetAllDatabaseNamesInConfigCommand result;
                //this is returned the DoSearchCommand object so you can call methods within it
                result = DataPortal.Execute<GetAllDatabaseNamesInConfigCommand>(new GetAllDatabaseNamesInConfigCommand());
                return result.MyDatabaseNameList;
            }
            catch (Exception)
            {

                throw;
            }
        }



        //create nested command object that calls the doSearch command. Two additional objects need to be added
        //GetHitList and GetData
        [Serializable]
        private class GetAllDatabaseNamesInConfigCommand : CommandBase
        {

            //what is returned from the command
            private List<string> _myDatabaseNameList;


            public GetAllDatabaseNamesInConfigCommand()
            {

                //constructor to set properties that match input paramters

            }

            public List<string> MyDatabaseNameList
            {
                get { return _myDatabaseNameList; }
                set { _myDatabaseNameList = value; }
            }


            protected override void DataPortal_Execute()
            {
                try
                {
                    //the heart of the command. In this case it is simply calling a command in ConfigurationUlities, but it could also call a Manager method, or be
                    //a completely new method that calls nothing elase
                    _myDatabaseNameList = new List<string>();
                    _myDatabaseNameList = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetAllDatabaseNamesInConfig();


                }
                catch (Exception)
                {

                    throw;
                }
            }



        }

        #endregion
        #region GetAppByDatabase
        /// <summary>
        /// Command object for Retrieving all the AppNames reading from the configuration file that is associated with the framework located in  C:\Documents and Settings\All Users\Application Data\CambridgeSoft\ChemOfficeEnterprisex.x.x.x COEFrameworkConfig.xml
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAppByDatabase(DBMSType dbmsType)
        {
            try
            {
                //this doesn't make a call to any database so we don't need the appname
                //Csla.ApplicationContext.GlobalContext["AppName"] = appName;
                GetAppByDatabaseCommand result;
                //this is returned the DoSearchCommand object so you can call methods within it
                result = DataPortal.Execute<GetAppByDatabaseCommand>(new GetAppByDatabaseCommand(dbmsType));
                return result.MyAppNameList;
            }
            catch (Exception)
            {

                throw;
            }
        }



        //create nested command object that calls the doSearch command. Two additional objects need to be added
        //GetHitList and GetData
        [Serializable]
        private class GetAppByDatabaseCommand : CommandBase
        {

            //what is returned from the command
            private List<string> _myAppNameList;
            private DBMSType _dbmsType;

            public GetAppByDatabaseCommand(DBMSType dbType)
            {

                _dbmsType = dbType;

            }

            public List<string> MyAppNameList
            {
                get { return _myAppNameList; }
                set { _myAppNameList = value; }
            }


            protected override void DataPortal_Execute()
            {
                try
                {
                    //the heart of the command
                    _myAppNameList = new List<string>();
                    _myAppNameList = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetAppByDatabase(_dbmsType);
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }

        #endregion
        #region GetDatabaseByDatabaseType
        /// <summary>
        /// Command object for Retrieving all the Database Names reading from the configuration file that is associated with the framework located in  
        /// C:\Documents and Settings\All Users\Application Data\CambridgeSoft\ChemOfficeEnterprisex.x.x.x COEFrameworkConfig.xml
        /// and having DBMSType same as passed as parameter.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDatabaseByDatabaseType(DBMSType dbmsType)
        {
            try
            {
                GetDatabaseByDatabaseTypeCommand result;
                result = DataPortal.Execute<GetDatabaseByDatabaseTypeCommand>(new GetDatabaseByDatabaseTypeCommand(dbmsType));
                return result.MyDatabaseNameList;
            }
            catch (Exception)
            {
                throw;
            }
        }



        //create nested command object that calls the doSearch command. Two additional objects need to be added
        //GetHitList and GetData
        [Serializable]
        private class GetDatabaseByDatabaseTypeCommand : CommandBase
        {

            //what is returned from the command
            private List<string> _myDatabaseNameList;
            private DBMSType _dbmsType;

            public GetDatabaseByDatabaseTypeCommand(DBMSType dbType)
            {

                _dbmsType = dbType;

            }

            public List<string> MyDatabaseNameList
            {
                get { return _myDatabaseNameList; }
                set { _myDatabaseNameList = value; }
            }


            protected override void DataPortal_Execute()
            {
                try
                {
                    //the heart of the command
                    _myDatabaseNameList = new List<string>();
                    _myDatabaseNameList = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetDatabaseNameByType(_dbmsType);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        #endregion
        #region GetDatabaseNameFromAppName
        /// <summary>
        /// Get the database associated with an application name
        /// </summary>
        /// <returns></returns>
        public static string GetDatabaseNameFromAppName(string appName)
        {
            try
            {
                //this doesn't make a call to any database so we don't need the appname
                //Csla.ApplicationContext.GlobalContext["AppName"] = appName;
                GetDatabaseNameFromAppNameCommand result;
                //this is returned the DoSearchCommand object so you can call methods within it
                result = DataPortal.Execute<GetDatabaseNameFromAppNameCommand>(new GetDatabaseNameFromAppNameCommand(appName));
                return result.DatabaseName;
            }
            catch (Exception)
            {

                throw;
            }
        }



        //create nested command object that calls the doSearch command. Two additional objects need to be added
        //GetHitList and GetData
        [Serializable]
        private class GetDatabaseNameFromAppNameCommand : CommandBase
        {

            //what is returned from the command
            private string _databaseName;
            private string _appName;

            public GetDatabaseNameFromAppNameCommand(string appName)
            {

                //constructor to set properties that match input paramters
                _appName = appName;
            }

            public string DatabaseName
            {
                get { return _databaseName; }
                set { _databaseName = value; }
            }


            protected override void DataPortal_Execute()
            {
                try
                {
                    //the heart of the command. In this case it is simply calling a command in ConfigurationUlities, but it could also call a Manager method, or be
                    //a completely new method that calls nothing elase
                    _databaseName = string.Empty;
                    _databaseName = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetDatabaseNameFromAppName(_appName);


                }
                catch (Exception)
                {

                    throw;
                }
            }



        }

        #endregion
        #region GetAllDatabaseNames
        /// <summary>
        /// Get the databases available in the coeframeworkconfig. associated with an application name
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllDatabaseNames(bool filteredByUserPrivs)
        {
            try
            {

                GetAllDatabaseNamesCommand result;
                //this is returned the DoSearchCommand object so you can call methods within it
                result = DataPortal.Execute<GetAllDatabaseNamesCommand>(new GetAllDatabaseNamesCommand(filteredByUserPrivs));
                return result.DatabaseNames;
            }
            catch (Exception)
            {

                throw;
            }
        }



        //create nested command object that calls the doSearch command. Two additional objects need to be added
        //GetHitList and GetData
        [Serializable]
        private class GetAllDatabaseNamesCommand : CommandBase
        {

            //what is returned from the command
            private List<string> _databaseNames;
            private bool _filterByUserPrivs;

            public GetAllDatabaseNamesCommand(bool filterByUserPrivs)
            {

                //constructor to set properties that match input paramters
                _filterByUserPrivs = filterByUserPrivs;
            }

            public List<string> DatabaseNames
            {
                get { return _databaseNames; }
                set { _databaseNames = value; }
            }


            protected override void DataPortal_Execute()
            {
                try
                {
                    _databaseNames = null;
                    _databaseNames = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetAllDatabaseNamesInConfig(_filterByUserPrivs);


                }
                catch (Exception)
                {

                    throw;
                }
            }



        }

        #endregion
        
    }
}
