using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.RegistrationAdmin.Access;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Registration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Configuration;
using System.Data;

namespace RegistrationAdmin.Services.MSUnitTests.Helpers
{
   class Helpers
    {
        const string COEApplicationName = "REGISTRATION";
        const string COEUserID = "96";
        const string coeFormServiceName = "COEForm";
        const string coeDBName = "COEDB";

        private static DALFactory _dalFactory;

        static void InitializeHelpers()
        {
            Csla.ApplicationContext.GlobalContext["USER_PERSONID"] = COEUserID;
            Csla.ApplicationContext.GlobalContext["AppName"] = COEApplicationName;
        }

        public static string GetFullTypeNameMessage<T>(T obj)
        {
            StringBuilder strMessage = new StringBuilder();
            strMessage.AppendFormat("{0}", obj.GetType().FullName);
            return strMessage.ToString();
        }

        /// <summary>
        /// Retrieves an instance of the core RegAdmin DAL class
        /// </summary>
        /// <param name="dalBase">the 'out' parameter that will be populated</param>
        /// <param name="serviceName">the calling assembly's key (name)</param>
        public static void GetRegDal(
            ref RegAdminOracleDAL dalBase, string serviceName
            )
        {
            InitializeHelpers();
            if (string.IsNullOrEmpty(serviceName)) serviceName = Constants.ADMINSERVICENAME;
            string databaseName = ConfigurationUtilities.GetDatabaseNameFromAppName(COEApplicationName);
            if (_dalFactory == null)
                _dalFactory = new DALFactory();

            string typeName = typeof(RegAdminOracleDAL).FullName;
            _dalFactory.GetDAL<RegAdminOracleDAL>(
                ref dalBase, serviceName, databaseName, true, typeName);
        }

        /// <summary>
        /// Retrieves an instance of the core COEDB database class
        /// </summary>
        /// <param name="coeDb">the 'out' parameter that will be populated</param>
        public static void GetDatabase(ref Database coeDb, DbType dbType)
        {
            //connect to COEDB direct, and delete the newly added form 4020
            coeDb = new OracleDatabase(GetConnStringFromConfig(dbType));
        }

        static string GetConnStringFromConfig(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.COEDB:
                    return ConfigurationManager.ConnectionStrings["coedbConn"].ConnectionString;
                case DbType.REGDB:
                    return ConfigurationManager.ConnectionStrings["regdbConn"].ConnectionString;
                default :
                    return string.Empty;
            }
        }

        public static List<string> ColumList(string tableName)
        {
            List<string> columnList = new List<string>();
            Database ds = null;
            GetDatabase(ref ds, DbType.REGDB);
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("SELECT UPPER(COLUMN_NAME) AS COLUMNNAME FROM SYS.ALL_TAB_COLUMNS WHERE TABLE_NAME = '{0}'", tableName.ToUpper());
            using (IDataReader rdr = ds.ExecuteReader(System.Data.CommandType.Text, strSql.ToString()))
            {
                if (rdr != null)
                {
                    while (rdr.Read())
                    {
                        columnList.Add(rdr["COLUMNNAME"].ToString());
                    }
                }
            }
            return columnList;
        }

        public enum DbType
        {
            COEDB,
            REGDB
        }

        /// <summary>
        /// Provides authentication for database-enabled unit tests.
        /// </summary>
        /// <returns></returns>
        public static bool Logon()
        {
            return CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.Login(ConfigurationManager.AppSettings["TestingUserName"], ConfigurationManager.AppSettings["TestingPassword"]);
        }

        /// <summary>
        /// Clearing current login session.
        /// </summary>
        public static void Logoff()
        {
            CambridgeSoft.COE.Framework.COESecurityService.COEPrincipal.Logout();
        }

        public static int GetMaxSequenceNumberFromSequences()
        {
            RegAdminOracleDAL regDal = null;
            GetRegDal(ref regDal, "");
            if (regDal != null)
            {
                object seqNumberObj = regDal.DALManager.Database.ExecuteScalar(CommandType.Text, "SELECT MAX(SEQUENCE_ID) FROM SEQUENCE");
                int seqNumber;
                if (int.TryParse(seqNumberObj.ToString(), out seqNumber))
                {
                    return seqNumber;
                }
            }
            return -1;
        }

        public static DataTable GetSequenceDetails(int sequenceNumber)
        {
            RegAdminOracleDAL regDal = null;
            DataTable dt = null;
            GetRegDal(ref regDal, "");
            if (regDal != null)
            {
                DataSet seqDetails = regDal.DALManager.Database.ExecuteDataSet(CommandType.Text, string.Format("SELECT * FROM SEQUENCE WHERE SEQUENCE_ID ='{0}'", sequenceNumber));
                dt = seqDetails.Tables[0];
            }
            return dt;
        }
    }

}
