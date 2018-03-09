using CambridgeSoft.COE.Framework.Common;
using Csla.Data;
using System;
using System.Data.Common;
using System.Data;
using System.Text;


namespace CambridgeSoft.COE.Framework.COEReportingService
{
    public class DAL : DALBase
    {
        #region Variables
        [NonSerialized]
        public static string _coeReportTableName = string.Empty;
        //public static string _coeReportTypeTableName = string.Empty;
        private static string _star = "ID, NAME, DESCRIPTION, USER_ID, IS_PUBLIC, DATE_CREATED, REPORT_TEMPLATE, DATABASE, APPLICATION, DATAVIEW_ID, TYPE, CATEGORY";
        #endregion

        #region Methods
        /// <summary>
        ///  return the next report id from datatbase.
        /// </summary>
        /// <returns>the new id generated</returns>
        public virtual int GetNewID()
        {
            //this has to be overridden so returns an -1 if there is not override
            return -1;
        }

        /// <summary>
        /// get all reports
        /// </summary>
        /// <returns>a safedatareader containing the reports</returns>
        internal virtual SafeDataReader GetAll()
        {
            string sql = string.Format("SELECT {0} FROM {1} WHERE IS_PUBLIC ='1'", _star, _coeReportTableName);

            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch(Exception ex) 
            {
                throw ex;
            }
        }

        /// <summary>
        /// get a report
        /// </summary>
        /// <param name="id">the id for the report</param>
        /// <returns>a safedatareader containing the report</returns>
        internal virtual SafeDataReader Get(int coeReportId)
        {

            string sql = string.Format("SELECT {0} FROM {1} WHERE ID = {2}",  _star, _coeReportTableName, DALManager.BuildSqlStringParameterName("pReportIdInParam"));

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddInParameter(dbCommand, "pReportIdInParam", DbType.Int32, coeReportId);
          
            SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            return safeReader;
        }

        /// <summary>
        /// To get all Reports by diferents criteria
        /// </summary>
        /// <param name="userId">the user id of the reports</param>
        /// <param name="databaseName">the database name of the reports</param>
        /// <param name="application">the application name of the reports</param>
        /// <param name="getPublicReports"></param>
        /// <param name="dataViewId">the dataview id of the reports</param>
        /// <returns></returns>
        internal SafeDataReader GetReports(string userId, string name, string description, string databaseName, string application, bool getPublicReports, int dataViewId, COEReportType? type, string category)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(string.Format("SELECT {0} FROM {1} WHERE ", _star, _coeReportTableName));
            
            if (!string.IsNullOrEmpty(userId))
                sqlBuilder.Append("USER_ID = " + DALManager.BuildSqlStringParameterName("pUserIdParam") + " AND ");
            
            if (!string.IsNullOrEmpty(name))
                sqlBuilder.Append("NAME = " + DALManager.BuildSqlStringParameterName("pNameParam") + " AND ");
            
            if (!string.IsNullOrEmpty(description))
                sqlBuilder.Append("DESCRIPTION = " + DALManager.BuildSqlStringParameterName("pDescriptionParam") + " AND ");

            if (!string.IsNullOrEmpty(databaseName))
                sqlBuilder.Append("DATABASE = " + DALManager.BuildSqlStringParameterName("pDataBaseParam") + " AND ");
            
            if (!string.IsNullOrEmpty(application))
                sqlBuilder.Append("APPLICATION = " + DALManager.BuildSqlStringParameterName("pApplicationParam") + " AND ");
            
            if (getPublicReports == false)
                sqlBuilder.Append("IS_PUBLIC = " + DALManager.BuildSqlStringParameterName("pIsPublicParam") + " AND ");
            
            if (dataViewId >= 0)
                sqlBuilder.Append("DATAVIEW_ID = " + DALManager.BuildSqlStringParameterName("pDataViewIdParam") + " AND ");

            if (type.HasValue)
                sqlBuilder.Append("TYPE = " + DALManager.BuildSqlStringParameterName("pType") + " AND ");

            if (!string.IsNullOrEmpty(category))
                sqlBuilder.Append("CATEGORY = " + DALManager.BuildSqlStringParameterName("pCategory") + " AND ");

            string sql = sqlBuilder.ToString();
            sql = sql.Remove(sql.Length - 5);

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            if (!string.IsNullOrEmpty(userId))
                DALManager.Database.AddInParameter(dbCommand, "pUserIdParam", DbType.AnsiString, userId);
            if (!string.IsNullOrEmpty(name))
                DALManager.Database.AddInParameter(dbCommand, "pNameParam", DbType.AnsiString, name);
            if (!string.IsNullOrEmpty(description))
                DALManager.Database.AddInParameter(dbCommand, "pDescriptionParam", DbType.AnsiString, description);
            if (!string.IsNullOrEmpty(databaseName))
                DALManager.Database.AddInParameter(dbCommand, "pDataBaseParam", DbType.AnsiString, databaseName);
            if (!string.IsNullOrEmpty(application))
                DALManager.Database.AddInParameter(dbCommand, "pApplicationParam", DbType.AnsiString, application);
            if (getPublicReports == false)
                DALManager.Database.AddInParameter(dbCommand, "pIsPublicParam", DbType.Boolean, true);
            if (dataViewId >= 0)
                DALManager.Database.AddInParameter(dbCommand, "pDataViewIdParam", DbType.Int32, dataViewId);
            if (type.HasValue)
                DALManager.Database.AddInParameter(dbCommand, "pType", DbType.AnsiString, type.ToString());
            if (!string.IsNullOrEmpty(category))
                DALManager.Database.AddInParameter(dbCommand, "pCategory", DbType.AnsiString, category);


            try
            {
                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }

        /// <summary>
        /// creates a new record in the coereport table
        /// </summary>
        /// <param name="coeReportId">the id of the report</param>
        /// <param name="name">the name of the report</param>
        /// <param name="isPublic">is report available to users other then the one that created it</param>
        /// <param name="description">the description of the report</param>
        /// <param name="userId">the user that is making the request</param>
        /// <param name="serializedCOEReportTemplate">the serialized/xml report of the coereport template object</param>
        /// <param name="databaseName">the database name</param>
        /// <param name="application">the application name</param>
        /// <param name="dataViewId">the related dataview id</param>
        /// <returns>the report id</returns>
        internal virtual int Insert(string name, bool isPublic, string description, string userId, COEReport reportTemplate, string databaseName, string application, int dataViewId, COEReportType type, string category)
        {
            int coeReportId = reportTemplate.ID = GetNewID();
            string serializedCOEReportTemplate = reportTemplate.ToString();
            //inserting the data in the database.
            string sql = string.Format("INSERT INTO {0}({1}) VALUES", _coeReportTableName, _star) + 
                 "( " + DALManager.BuildSqlStringParameterName("pReportId") +
                 " ," + DALManager.BuildSqlStringParameterName("pName") +
                 " , " + DALManager.BuildSqlStringParameterName("pDescription") +
                 " , " + DALManager.BuildSqlStringParameterName("pUserId") +
                 " , " + DALManager.BuildSqlStringParameterName("pIsPublic") +
                 " , " + DALManager.BuildSqlStringParameterName("pDateCreated") +
                 " , " + DALManager.BuildSqlStringParameterName("pReportTemplate") +
                 " , " + DALManager.BuildSqlStringParameterName("pDatabaseName") +
                 " , " + DALManager.BuildSqlStringParameterName("pApplication") +
                 " , " + DALManager.BuildSqlStringParameterName("pDataViewId") +
                 " , " + DALManager.BuildSqlStringParameterName("pType") + 
                 " , " + DALManager.BuildSqlStringParameterName("pCategory") + 
                 " )";

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            DALManager.Database.AddInParameter(dbCommand, "pReportId", DbType.Int32, coeReportId);
            DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, name);
            DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, description);
            DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId);
            DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
            DALManager.Database.AddInParameter(dbCommand, "pDateCreated", DbType.Date, DateTime.Now);
            DALManager.Database.AddParameter(dbCommand, "pReportTemplate", DbType.AnsiString, Int32.MaxValue, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, serializedCOEReportTemplate);
            DALManager.Database.AddParameter(dbCommand, "pDatabaseName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
            DALManager.Database.AddParameter(dbCommand, "pApplication", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, application);
            DALManager.Database.AddInParameter(dbCommand, "pDataViewId", DbType.Int32, dataViewId);
            DALManager.Database.AddInParameter(dbCommand, "pType", DbType.AnsiString, type.ToString());
            DALManager.Database.AddInParameter(dbCommand, "pCategory", DbType.AnsiString, category);

            int recordsAffected = -1;
            try
            {
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return coeReportId;
        }

        /// <summary>
        /// update the report with id = <paramref name="coeReportId"/>
        /// </summary>
        /// <param name="coeReportId">the id of the report</param>
        /// <param name="name">the name of the report</param>
        /// <param name="isPublic">is report available to users other then the one that created it</param>
        /// <param name="description">the description of the report</param>
        /// <param name="userId">the user that is making the request</param>
        /// <param name="serializedCOEReportTemplate">the serialized/xml report of the coereport template object</param>
        /// <param name="databaseName">the database name</param>
        /// <param name="application">the application name</param>
        /// <param name="dataViewId">the related dataview id</param>
        /// <returns>the report id</returns>
        internal virtual int Update(int coeReportId, string name, bool isPublic, string description, string userId, string serializedCOEReportTemplate, string databaseName, string application, int dataViewId, COEReportType type, string category)
        {            
            string sql = "UPDATE " + _coeReportTableName +
                 " SET NAME = " + DALManager.BuildSqlStringParameterName("pName") +
                 ", DESCRIPTION = " + DALManager.BuildSqlStringParameterName("pDescription") +
                 ", USER_ID = " + DALManager.BuildSqlStringParameterName("pUserId") +
                 ", IS_PUBLIC = " + DALManager.BuildSqlStringParameterName("pIsPublic") +            
                 ", REPORT_TEMPLATE = " + DALManager.BuildSqlStringParameterName("pReportTemplate") +
                 ", DATABASE = " + DALManager.BuildSqlStringParameterName("pDatabaseName") +
                 ", APPLICATION = " + DALManager.BuildSqlStringParameterName("pApplication") +
                 ", DATAVIEW_ID = " + DALManager.BuildSqlStringParameterName("pDataViewId") +
                 ", TYPE = " + DALManager.BuildSqlStringParameterName("pType") +
                 ", CATEGORY = " + DALManager.BuildSqlStringParameterName("pCategory") +
                 " WHERE ID = " + DALManager.BuildSqlStringParameterName("pReportId");

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            
            DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, name);
            DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, description);
            DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId);
            DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));            
            DALManager.Database.AddParameter(dbCommand, "pReportTemplate", DbType.AnsiString, Int32.MaxValue, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, serializedCOEReportTemplate);
            DALManager.Database.AddParameter(dbCommand, "pDatabaseName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
            DALManager.Database.AddParameter(dbCommand, "pApplication", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, application);
            DALManager.Database.AddInParameter(dbCommand, "pDataViewId", DbType.Int32, dataViewId);
            DALManager.Database.AddInParameter(dbCommand, "pType", DbType.AnsiString, type.ToString());
            DALManager.Database.AddInParameter(dbCommand, "pCategory", DbType.AnsiString, category);
            DALManager.Database.AddInParameter(dbCommand, "pReportId", DbType.Int32, coeReportId);

            int recordsAffected = -1;
            try
            {
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return coeReportId;
        }

        /// <summary>
        /// delete report with id =<paramref name="coeReportId"/> from database
        /// </summary>
        /// <param name="coeReportId">the id of the report to delete</param>
        public virtual void Delete(int coeReportId)
        {
            string sql = null;
            DbCommand dbCommand = null;
            sql = "DELETE FROM " + _coeReportTableName +
            " WHERE ID =" + DALManager.BuildSqlStringParameterName("pReportId");

            dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            DALManager.Database.AddInParameter(dbCommand, "pReportId", DbType.Int32, coeReportId);
            int recordsAffected = -1;
            try
            {
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// to get the name of the table
        /// </summary>
        public override void SetServiceSpecificVariables()
        {
            string owner = this.DALManager.DatabaseData.OriginalOwner.ToString();
            _coeReportTableName = COEReportUtilities.BuildCOEReportTableName(owner);
            //_coeReportTypeTableName = COEReportUtilities.BuildCOEReportTypeTableName(owner);
        }

        /*internal SafeDataReader GetReportTypes()
        {
            string sql = "SELECT ID, TYPE FROM " + _coeReportTypeTableName + " ORDER BY ID ASC";

            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }*/

        #endregion
    }
}
