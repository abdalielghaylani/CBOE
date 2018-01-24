using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using CambridgeSoft.COE.Framework.Export;
using Csla.Data;

namespace CambridgeSoft.COE.Framework.COEExportService
{
    public class DAL : DALBase
    {
        public string _coeExportTemplateTableName = string.Empty;
        [NonSerialized]
        private string _serviceName = "COEExportTemplate";

        /// <summary>
        /// to get the name of the table
        /// </summary>
        public override void SetServiceSpecificVariables()
        {
            try
            {
                string owner = this.DALManager.DatabaseData.OriginalOwner.ToString();
                COEExportTemplateUtilities.BuildExportTemplateTableName(owner, ref _coeExportTemplateTableName);
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region Insert and Update Methods

        public virtual int Insert(string name, string description, string userId, bool isPublic, int dataViewId, int webFormId, int winFormId, ResultsCriteria coeResultCriteria)
        {
            try
            {
                if (name == string.Empty)
                    name = "EXPORTTEMPLATE";
                if (description == string.Empty)
                    description = "Export Template";
                string serializedCOEResultCriteria = COEExportTemplateUtilities.SerializeCOEResultCriteria(coeResultCriteria);

                string sql = "INSERT INTO " + _coeExportTemplateTableName +
                            "(NAME,DESCRIPTION,COERESULTCRITERIA,IS_PUBLIC,DATE_CREATED,DATAVIEW_ID,WEBFORM_ID,WINFORM_ID,USER_ID) VALUES " +
                            "(" + DALManager.BuildSqlStringParameterName("pName") +
                            "," + DALManager.BuildSqlStringParameterName("pDescription") +
                            "," + DALManager.BuildSqlStringParameterName("pCOEResultCriteria") +
                            "," + DALManager.BuildSqlStringParameterName("pIsPublic") +
                            "," + DALManager.BuildSqlStringParameterName("pDateCreated") +
                            "," + DALManager.BuildSqlStringParameterName("pDataViewId") +
                            "," + DALManager.BuildSqlStringParameterName("pWebFormId") +
                            "," + DALManager.BuildSqlStringParameterName("pWinFormId") +
                            "," + DALManager.BuildSqlStringParameterName("pUserId") + ")";

                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, name);
                DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, description);
                DALManager.Database.AddParameter(dbCommand, "pCOEResultCriteria", DbType.AnsiString, Int32.MaxValue, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, serializedCOEResultCriteria);
                DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
                DALManager.Database.AddInParameter(dbCommand, "pDateCreated", DbType.Date, DateTime.Now);
                DALManager.Database.AddInParameter(dbCommand, "pDataViewId", DbType.Int32, dataViewId);
                DALManager.Database.AddInParameter(dbCommand, "pWebFormId", DbType.Int32, webFormId);
                DALManager.Database.AddInParameter(dbCommand, "pWinFormId", DbType.Int32, winFormId);
                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId.ToUpper());

                int recordsAffected = DALManager.ExecuteNonQuery(dbCommand);

                // returning last id inserted
                int id = 0;
                SafeDataReader dr = null;
                if (recordsAffected == 1)
                {
                    id = this.GetId(name);
                }
                return id;

            }
            catch
            {
                throw;
            }
        }

        public virtual int Update(int id, string name, string description, string userId, bool isPublic, int dataViewId, int webFormId, int winFormId, ResultsCriteria coeResultCriteria)
        {
            try
            {
                if (name == string.Empty)
                    name = "EXPORTTEMPLATE";
                if (description == string.Empty)
                    description = "Export Template";
                string serializedCOEResultCriteria = COEExportTemplateUtilities.SerializeCOEResultCriteria(coeResultCriteria);

                string sql = "UPDATE " + _coeExportTemplateTableName +
                    " SET NAME= " + DALManager.BuildSqlStringParameterName("pName") + "," +
                    " DESCRIPTION=" + DALManager.BuildSqlStringParameterName("pDescription") + "," +
                    " COERESULTCRITERIA=" + DALManager.BuildSqlStringParameterName("pCOEResultCriteria") + "," +
                    " IS_PUBLIC= " + DALManager.BuildSqlStringParameterName("pIsPublic") + "," +
                    " DATAVIEW_ID=" + DALManager.BuildSqlStringParameterName("pDataViewId") + "," +
                    " WEBFORM_ID=" + DALManager.BuildSqlStringParameterName("pWebFormId") + "," +
                    " WINFORM_ID=" + DALManager.BuildSqlStringParameterName("pWinFormId") + "," +
                    " USER_ID=" + DALManager.BuildSqlStringParameterName("pUserId") +
                    " WHERE ID=" + DALManager.BuildSqlStringParameterName("pId");

                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, name);
                DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, description);
                DALManager.Database.AddParameter(dbCommand, "pCOEResultCriteria", DbType.AnsiString, Int32.MaxValue, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, serializedCOEResultCriteria);
                DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));                
                DALManager.Database.AddInParameter(dbCommand, "pDataViewId", DbType.Int32, dataViewId);
                DALManager.Database.AddInParameter(dbCommand, "pWebFormId", DbType.Int32, webFormId);
                DALManager.Database.AddInParameter(dbCommand, "pWinFormId", DbType.Int32, winFormId);
                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId.ToUpper());
                DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);

                int recordsAffected = DALManager.ExecuteNonQuery(dbCommand);

                // returning last id updated

                return id;

            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region Delete methods

        /// <summary>
        /// deletes a saved template record containing id
        /// </summary>
        /// <param name="id">saved template id</param>
        public void Delete(int id)
        {
            try
            {

                string sql = null;
                DbCommand dbCommand = null;

                sql = "DELETE FROM " + _coeExportTemplateTableName +
                " WHERE ID =" + DALManager.BuildSqlStringParameterName("pId");

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);
                DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void DeleteAll(string userId)
        {
            try
            {

                string sql = null;
                DbCommand dbCommand = null;

                sql = "DELETE FROM " + _coeExportTemplateTableName +
                " WHERE USER_ID =" + DALManager.BuildSqlStringParameterName("pUserId");

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddInParameter(dbCommand, "pUserId", DbType.Int32, userId.ToUpper());
                DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// deletes a saved template record containing Template name
        /// </summary>
        /// <param name="name">saved template name</param>
        public void Delete(string name)
        {
            try
            {

                string sql = null;
                DbCommand dbCommand = null;

                sql = "DELETE FROM " + _coeExportTemplateTableName +
                " WHERE NAME =" + DALManager.BuildSqlStringParameterName("pName");

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddInParameter(dbCommand, "pName", DbType.AnsiString, name);
                DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception)
            {

                throw;
            }
        }



        #endregion

        #region Get methods

        /// <summary>
        /// gets all templates data for a user
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <returns>a safedatareader containing all templates for a user </returns>
        public SafeDataReader GetUserTemplates(string userId)
        {
            try
            {

                string sql = null;
                DbCommand dbCommand = null;
                sql = "SELECT * FROM " + _coeExportTemplateTableName +
                    " WHERE USER_ID=" + DALManager.BuildSqlStringParameterName("pUserId") + " order by UPPER(NAME)";
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId.ToUpper());

                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// returns template filtered by template name
        /// </summary>
        /// <param name="templateName"> the template name </param>
        /// <returns> safedatareader contains the template </returns>
        public SafeDataReader GetTemplate(int id)
        {
            try
            {

                string sql = null;
                DbCommand dbCommand = null;
                sql = "SELECT * FROM " + _coeExportTemplateTableName +
                    " WHERE ID=" + DALManager.BuildSqlStringParameterName("pId");
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                //DALManager.Database.AddParameter(dbCommand, "pTemplateID", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, name.ToUpper());
                DALManager.Database.AddInParameter(dbCommand, "pId", DbType.Int32, id);

                SafeDataReader safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// returns id filtered by template name
        /// </summary>
        /// <param name="name">the template name</param>
        /// <returns> id of the template</returns>
        public int GetId(string name)
        {
            try
            {
                int id = 0;
                string sql = null;
                DbCommand dbCommand = null;
                sql = "SELECT ID FROM " + _coeExportTemplateTableName +
                    " WHERE NAME=" + DALManager.BuildSqlStringParameterName("pTemplateName");
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pTemplateName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, name);

                // Coverity Fix CID - 11818
                using (SafeDataReader safeReader = new SafeDataReader(DALManager.ExecuteReader(dbCommand)))
                {
                    if (safeReader.Read())
                        id = safeReader.GetInt32("ID");
                }

                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// returns templates filtered by data view id
        /// </summary>
        /// <param name="dataViewId"> the dataview id </param>
        /// <returns> safedatareader contains the template list </returns>
        public SafeDataReader GetTemplatesByDataViewId(int dataViewId)
        {
            try
            {

                string sql = null;
                DbCommand dbCommand = null;
                sql = "SELECT * FROM " + _coeExportTemplateTableName +
                    " WHERE DATAVIEW_ID=" + DALManager.BuildSqlStringParameterName("pDataViewId") + " order by UPPER(NAME)";
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pDataViewId", DbType.Int32, dataViewId);

                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// returns templates filtered by web form id
        /// </summary>
        /// <param name="webFormId"> the webform id </param>
        /// <returns> safedatareader contains the template list </returns>
        public SafeDataReader GetTemplatesByWebFormId(int webFormId)
        {
            try
            {

                string sql = null;
                DbCommand dbCommand = null;
                sql = "SELECT * FROM " + _coeExportTemplateTableName +
                    " WHERE WEBFORM_ID=" + DALManager.BuildSqlStringParameterName("pWebFormId") + " order by UPPER(NAME)";
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pWebFormId", DbType.Int32, webFormId);

                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// returns templates filtered by winform id
        /// </summary>
        /// <param name="winFormId"> the winform id </param>
        /// <returns> safedatareader contains the template list </returns>
        public SafeDataReader GetTemplatesByWinFormId(int winFormId)
        {
            try
            {

                string sql = null;
                DbCommand dbCommand = null;
                sql = "SELECT * FROM " + _coeExportTemplateTableName +
                    " WHERE WINFORM_ID=" + DALManager.BuildSqlStringParameterName("pWinFormId") + " order by UPPER(NAME)";
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pWinFormId", DbType.Int32, winFormId);

                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Gets templates for a dataview and a user. If getPublicTemplates is true, it also brings other users' public templates.
        /// </summary>
        /// <param name="dataViewId"></param>
        /// <param name="userId"></param>
        /// <param name="getPublicTemplates"></param>
        /// <returns></returns>
        public SafeDataReader GetUserTemplatesByDataViewId(int dataViewId, string userId, bool getPublicTemplates)
        {
            try
            {
                string sql = null;
                DbCommand dbCommand = null;
                sql = "SELECT * FROM " + _coeExportTemplateTableName +
                          " WHERE DATAVIEW_ID=" + DALManager.BuildSqlStringParameterName("pDataViewId") + " AND " +
                          " (USER_ID=" + DALManager.BuildSqlStringParameterName("pUserId") +
                          (getPublicTemplates ? " OR IS_PUBLIC='1'": "") +
                          ") order by UPPER(NAME)";

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pDataViewId", DbType.Int32, dataViewId);
                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId.ToUpper());
                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch (Exception)
            {

                throw;
            }

        }



        /// <summary>
        /// returns templates filtered by isPublic flag
        /// </summary>
        /// <param name="isPublic">whether the template is public or private</param>
        /// <returns>a safedatareader containing the templates</returns>
        public SafeDataReader GetAll(bool isPublic)
        {
            try
            {

                string sql = "SELECT * FROM " + _coeExportTemplateTableName +
                    " WHERE IS_PUBLIC=" + DALManager.BuildSqlStringParameterName("pIsPublic") + " order by UPPER(NAME)";
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));

                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// get all templates
        /// </summary>
        /// <returns>a safedatareader containing the templates</returns>
        public SafeDataReader GetAll()
        {
            try
            {

                string sql = "SELECT * FROM " + _coeExportTemplateTableName + " order by UPPER(NAME)";
                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion



    }
}
