using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COEGenericObjectStorageService
{
    /// <summary>
    /// Class for supporting Oracle based COEGenericObjectStorage Service.
    /// </summary>
    public class OracleDataAccessClientDAL : CambridgeSoft.COE.Framework.COEGenericObjectStorageService.DAL
    {

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEGenericObjectStorage");

        /// <summary>
        /// Gets the new available id to be used as id for a record in COEGenericObjectStorage.
        /// </summary>
        /// <returns>The new id generated</returns>
        public override int GetNewID()
        {
            int id = -1;
            string sql = "SELECT " + _coeGenericObjectStorageTableName + "_SEQ.NEXTVAL FROM DUAL";
            try
            {
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                id = Convert.ToInt32(DALManager.ExecuteScalar(dbCommand));
                return id;
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Creates a new record in the COEGenericObjectStorage table.
        /// </summary>
        /// <param name="name">The name of the form.</param>
        /// <param name="description">The description of the form.</param>
        /// <param name="userId">The user that is making the request.</param>
        /// <param name="coeGenericObjectStorage">The string version of the object to be stored.</param>
        /// <param name="formgroup">The form group.</param>
        /// <param name="isPublic">Is the object public?.</param>
        /// <returns>The form's id.</returns>
        public override int Insert(int formgroup, string name, bool isPublic, string description, string userId, string coeGenericObjectStorage, string databaseName, int id, string associatedDataviewID = "")
        {
            if(NameAlreadyExist(name, isPublic, userId))
                throw new Exception(Resources.NameAlreadyExists);

            //inserting the data in the database.
            string sql = "INSERT INTO " + _coeGenericObjectStorageTableName +
                 "(ID,NAME,DESCRIPTION,IS_PUBLIC, USER_ID," +
                 "FORMGROUP, COEGENERICOBJECT,DATE_CREATED,ASSOCIATEDDATAVIEWID,DATABASE)VALUES " +
                 "( " + DALManager.BuildSqlStringParameterName("pId") +
                 " ," + DALManager.BuildSqlStringParameterName("pName") +
                 " , " + DALManager.BuildSqlStringParameterName("pDescription") +
                 " , " + DALManager.BuildSqlStringParameterName("pIsPublic") +
                 " , " + DALManager.BuildSqlStringParameterName("pUserId") +
                 " , " + DALManager.BuildSqlStringParameterName("pFormGroup") +
                 " , " + DALManager.BuildSqlStringParameterName("pCOEGenericObject") +
                 " , " + DALManager.BuildSqlStringParameterName("pDateCreated") +
                 " , " + DALManager.BuildSqlStringParameterName("pAssociatedDataviewID") +
                " , " + DALManager.BuildSqlStringParameterName("pDatabaseName") + " )";

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
            DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, name);
            DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, description);
            DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
            DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId.ToUpper());
            DALManager.Database.AddParameter(dbCommand, "pFormGroup", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, formgroup);
            //The Clob paramter must be done so it can use the Oracle Clob type
            dbCommand.Parameters.Add(new OracleParameter("pCOEGenericObject", OracleDbType.Clob, coeGenericObjectStorage, ParameterDirection.Input));
            DALManager.Database.AddInParameter(dbCommand, "pDateCreated", DbType.Date, DateTime.Now);
            DALManager.Database.AddParameter(dbCommand, "pAssociatedDataviewID", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, associatedDataviewID);
            DALManager.Database.AddParameter(dbCommand, "pDatabaseName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);

            int recordsAffected = -1;
            try
            {
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return id;
        }

        private bool NameAlreadyExist(string name, bool isPublic, string userId)
        {
            int count = 0;
            string sql = "SELECT COUNT(*) FROM " + _coeGenericObjectStorageTableName +
                             " WHERE Name=" + DALManager.BuildSqlStringParameterName("pName");
                sql += " AND USER_ID=" + DALManager.BuildSqlStringParameterName("pUserId");
            
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, name);
            DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId);

            count = int.Parse(DALManager.ExecuteScalar(dbCommand).ToString());
            return (count > 0);
        }

        /// <summary>
        /// Updates a generic object.
        /// </summary>
        /// <param name="id">ID of the generic object.</param>
        /// <param name="coeGenericObject">The updated xml form of the form.</param>
        /// <param name="name">The updated name of the form.</param>
        /// <param name="description">The updated description of the form.</param>
        /// <param name="isPublic">The is public updated property.</param>
        public override void Update(int id, string coeGenericObjectStorage, string name, string description, bool isPublic, string databaseName)
        {
            try
            {

                string sql = "UPDATE " + _coeGenericObjectStorageTableName +
                    " SET NAME= " + DALManager.BuildSqlStringParameterName("pName") + "," +
                    " DESCRIPTION=" + DALManager.BuildSqlStringParameterName("pDescription") + "," +
                    " IS_PUBLIC= " + DALManager.BuildSqlStringParameterName("pIsPublic") + "," +
                    " COEGENERICOBJECT=" + DALManager.BuildSqlStringParameterName("pCOEGenericObject") +
                    " WHERE ID=" + DALManager.BuildSqlStringParameterName("pId");

                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                //now setting the values for the parameters.
                DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, name);
                DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, description);
                DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
                //The Clob paramter must be done so it can use the Oracle Clob type
                dbCommand.Parameters.Add(new OracleParameter("pCOEGenericObject", OracleDbType.Clob, coeGenericObjectStorage, ParameterDirection.Input));
                DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
                DALManager.ExecuteNonQuery(dbCommand);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
