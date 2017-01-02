using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using Csla.Security;
using Csla.Data;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Reflection;

namespace CambridgeSoft.COE.Framework.COEHitListService
{
    /// <summary>
    /// 
    /// </summary>
    public class DAL : DALBase
    {
        protected string _tempHitListTableName = string.Empty;
        protected string _tempHitListIDTableName = string.Empty;
        protected string _savedHitListTableName = string.Empty;
        protected string _savedHitListIDTableName = string.Empty;
        protected string _hitListTableName = string.Empty;
        protected string _hitListIDTableName = string.Empty;
        private string _serviceName = "COEHitList";
        [NonSerialized]
        static protected COELog _coeLog = COELog.GetSingleton("COEHitList");



        public override void SetServiceSpecificVariables()
        {

            try
            {
                string owner = this.DALManager.DatabaseData.OriginalOwner.ToString();
                HitListUtilities.BuildTempHitListFullTableName(owner, ref _tempHitListTableName);
                HitListUtilities.BuildTempHitListIDFullTableName(owner, ref _tempHitListIDTableName);
                HitListUtilities.BuildSavedHitListFullTableName(owner, ref _savedHitListTableName);
                HitListUtilities.BuildSavedHitListIDFullTableName(owner, ref _savedHitListIDTableName);

            }
            catch (Exception ex)
            {

                throw;
            }
        }



        /// <summary>
        /// Create the union of two hitlist
        /// </summary>
        /// <param name="hitListID1">the first id</param>
        /// <param name="hitListID1Type">the first table type</param>
        /// <param name="hitListID2">the second id</param>
        /// <param name="hitListID2Type">the second table type</param>
        /// <returns>the new id for the newly created hitlist</returns>
        public virtual int UnionHitLists(int hitListID1, HitListType hitListID1Type, int hitListID2, HitListType hitListID2Type, string databaseName, int dataViewID)
        {
            try
            {
                return ExecuteOperation(hitListID1, hitListID1Type, hitListID2, hitListID2Type, databaseName, "UNION", dataViewID);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Create the subtraction of two hitlist
        /// </summary>
        /// <param name="hitListID1">the first id</param>
        /// <param name="hitListID1Type">the first table type</param>
        /// <param name="hitListID2">the second id</param>
        /// <param name="hitListID2Type">the second table type</param>
        /// <returns>the new id for the newly created hitlist</returns>
        public virtual int SubtractHitLists(int hitListID1, HitListType hitListID1Type, int hitListID2, HitListType hitListID2Type, string databaseName, int dataViewID)
        {
            try
            {
                return ExecuteOperation(hitListID1, hitListID1Type, hitListID2, hitListID2Type, databaseName, "MINUS", dataViewID);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Create the intersection of two hitlist
        /// </summary>
        /// <param name="hitListID1">the first id</param>
        /// <param name="hitListID1Type">the first table type</param>
        /// <param name="hitListID2">the second id</param>
        /// <param name="hitListID2Type">the second table type</param>
        /// <returns>the new id for the newly created hitlist</returns>
        public virtual int IntersectHitLists(int hitListID1, HitListType hitListID1Type, int hitListID2, HitListType hitListID2Type, string databaseName, int dataViewID)
        {
            try
            {
                return ExecuteOperation(hitListID1, hitListID1Type, hitListID2, hitListID2Type, databaseName, "INTERSECT", dataViewID);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private int ExecuteOperation(int hitListID1, HitListType hitListID1Type, int hitListID2, HitListType hitListID2Type, string databaseName, string operation, int dataViewID) {
            int newHitListID = CreateNewTempHitList("TEMP", false, "TEMP", COEUser.Name.ToString(), 0, databaseName, 0, dataViewID, HitListType.TEMP, -1, string.Empty);
            int recordsAffected;
            string sql = null;
            string operator1TableName = _tempHitListTableName;
            string operator2TableName = _tempHitListTableName;
            DbCommand dbCommand = null;

            if(hitListID1Type != HitListType.TEMP)
            {
                operator1TableName = _savedHitListTableName;
            }
            if(hitListID2Type != HitListType.TEMP) 
            {
                operator2TableName = _savedHitListTableName;
            }

            sql = "INSERT INTO " + _tempHitListTableName + @" (HITLISTID, ID, DATESTAMP, SORTORDER) 
                    SELECT " + DALManager.BuildSqlStringParameterName("pNewHitListID") + @" AS HITLISTID, Q.ID, SYSDATE, ROWNUM FROM (
                        SELECT L1.ID FROM " + operator1TableName + @" L1
                        WHERE L1.HITLISTID = " + DALManager.BuildSqlStringParameterName("pHitListId1") + @"
                         " + operation + @"
                        SELECT L2.ID FROM " + operator2TableName + @" L2 
                        WHERE L2.HITLISTID = " + DALManager.BuildSqlStringParameterName("pHitListId2") + ") Q";

            dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pNewHitListID"), DbType.Int32, newHitListID);
            DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pHitListId1"), DbType.Int32, hitListID1);
            DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pHitListId2"), DbType.Int32, hitListID2);
            recordsAffected = DALManager.Database.ExecuteNonQuery(dbCommand);

            UpdateHitListIDRow(newHitListID, HitListType.TEMP, databaseName, dataViewID);

            return newHitListID;
        }

        public virtual void UpdateHitListIDRow(int id, HitListType hitListType, string databaseName, int dataviewID)
        {
            string sql = null;
            DbCommand dbCommand = null;
            string hitlistIDTableName = _savedHitListIDTableName;
            string hitlistTableName = _savedHitListTableName;
            if(hitListType == HitListType.TEMP)
            {
                hitlistIDTableName = _tempHitListIDTableName;
                hitlistTableName = _tempHitListTableName;
            }
            
            sql = " UPDATE " + hitlistIDTableName + @" 
                            SET NUMBER_HITS = (SELECT COUNT(*) FROM " + hitlistTableName + @"
                                                WHERE HITLISTID = " + DALManager.BuildSqlStringParameterName("pHitListId") + @"),
                                DATABASE = " + DALManager.BuildSqlStringParameterName("pDatabase") + @",
                                DATAVIEW_ID = " + DALManager.BuildSqlStringParameterName("pDataViewID") + @"
                            WHERE ID=" + DALManager.BuildSqlStringParameterName("pHitListId");

            dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pHitListId"), DbType.Int32, id);
            DALManager.Database.AddParameter(dbCommand, "pDatabase", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
            DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pDataViewID"), DbType.Int32, dataviewID);
            DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pHitListId"), DbType.Int32, id);
            int recordsAffected = DALManager.Database.ExecuteNonQuery(dbCommand);

        }

        /// <summary>
        /// this method is for generating the next value of the temp hit list
        /// </summary>
        /// <param name="tempHitListCreateRequestInfo">to take all the required info</param>
        /// <returns>the new id generated</returns>
        public virtual int GetNewTempHitlistID()
        {
            //this has to be overridden so throw an error if there is not override
            return -1;
        }

        /// <summary>
        /// this function will create a new entry in temp hitlist table. it first reterieves the value of the next id to be inserted 
        /// by calling the getnewtemphitlistid.
        /// it inserts in two table the values simultaneously. first in csdohitlistid table with all the info being passed by temphitlistcreaterequestinfo
        /// and other csdohitlist where it stores the respective hitlistid with the ids of the hitlist returned. this entry will depend on the number_hits returned after a search.
        /// this functionis called by default. every serach that is made will first go in this table thereafter it is either usermarked or saved in user table.
        /// </summary>
        /// <param name="tempHitListCreateRequestInfo">it will take all the info required for csdohitlistid table like name,description, is_publice, number_hits, datecreated</param>
        /// <returns>the new id generated.</returns>
        public virtual int CreateNewTempHitList(string name, bool isPublic, string description, string userName, int numHits, string databaseName, int hitlistID, int dataViewID, HitListType originalType, int searchCriteriaId, string searchCriteriaType)
        {
            try
            {
                _coeLog.LogStart(MethodBase.GetCurrentMethod().Name.ToUpper() + " - " + MethodBase.GetCurrentMethod().DeclaringType.Name);

                //createtemphitlist inserts values in two table--csdohitlistid table(primary table) and csdohitlist table(referenced table)
                //have changed the signature 
                _coeLog.LogStart(MethodBase.GetCurrentMethod().Name.ToUpper() + " - " + MethodBase.GetCurrentMethod().DeclaringType.Name + " - Get new id");
                int id = GetNewTempHitlistID();
                if(hitlistID <= 0)
                    hitlistID = id;
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name.ToUpper() + " - " + MethodBase.GetCurrentMethod().DeclaringType.Name + " - Get new id");

                _coeLog.LogStart(MethodBase.GetCurrentMethod().Name.ToUpper() + " - " + MethodBase.GetCurrentMethod().DeclaringType.Name + " - Insert into hitlist");

                //inserting the values in csdohitlistid table
                string sql = "INSERT INTO " + _tempHitListIDTableName +
                            "(ID,HITLISTID,DATAVIEW_ID,NAME,DESCRIPTION,USER_ID,IS_PUBLIC,NUMBER_HITS,DATE_CREATED,DATABASE,TYPE, SEARCH_CRITERIA_ID, SEARCH_CRITERIA_TYPE) VALUES " +
                            "(" + DALManager.BuildSqlStringParameterName("pID") +
                            " ,  " + DALManager.BuildSqlStringParameterName("pHitListID") +
                            " ,  " + DALManager.BuildSqlStringParameterName("pDataViewID") +
                            " ,  " + DALManager.BuildSqlStringParameterName("pHitListName") +
                            " , " + DALManager.BuildSqlStringParameterName("pDescription") +
                            " , " + DALManager.BuildSqlStringParameterName("pUserId") +
                            " , " + DALManager.BuildSqlStringParameterName("pIsPublic") +
                            " , " + DALManager.BuildSqlStringParameterName("pNumberHits") +
                            " , " + DALManager.BuildSqlStringParameterName("pDate") +
                            " , " + DALManager.BuildSqlStringParameterName("pDatabase") +
                            " , " + DALManager.BuildSqlStringParameterName("pType") +
                            " , " + DALManager.BuildSqlStringParameterName("pSearchCriteriaId") +
                            " , " + DALManager.BuildSqlStringParameterName("pSearchCriteriaType") + " )";

                DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddInParameter(dbCommand, "pID", DbType.Int32, id);
                DALManager.Database.AddInParameter(dbCommand, "pHitListID", DbType.Int32, hitlistID);
                DALManager.Database.AddInParameter(dbCommand, "pDataViewID", DbType.Int32, dataViewID);
                DALManager.Database.AddParameter(dbCommand, "pHitListName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, name);
                DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, description);
                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userName.ToUpper());
                DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
                DALManager.Database.AddInParameter(dbCommand, "pNumberHits", DbType.Int32, numHits);
                DALManager.Database.AddInParameter(dbCommand, "pDate", DbType.Date, DateTime.Now);
                DALManager.Database.AddParameter(dbCommand, "pDatabase", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
                DALManager.Database.AddParameter(dbCommand, "pType", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, originalType);
                DALManager.Database.AddInParameter(dbCommand, "pSearchCriteriaId", DbType.Int32, searchCriteriaId);
                DALManager.Database.AddParameter(dbCommand, "pSearchCriteriaType", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, searchCriteriaType);

                _coeLog.Log(MethodBase.GetCurrentMethod().Name.ToUpper() + " - " + MethodBase.GetCurrentMethod().DeclaringType.Name +": "+ sql +
                                    "\nParameter: pID = " + id +
                                    "\nParameter: pHitListID = " + hitlistID +
                                    "\nParameter: pDataViewID = " + dataViewID +
                                    "\nParameter: pHitListName = " + name +
                                    "\nParameter: pDescription = " + description +
                                    "\nParameter: pUserId = " + userName.ToUpper() +
                                    "\nParameter: pIsPublic = " + isPublic +
                                    "\nParameter: pNumberHits = " + numHits +
                                    "\nParameter: pDate = " + DateTime.Now +
                                    "\nParameter: pDatabase = " + databaseName +
                                    "\nParameter: pType = " + originalType +
                                    "\nParameter: pSearchCriteriaId = " + searchCriteriaId +
                                    "\nParameter: pSearchCriteriaType = " + searchCriteriaType.ToString());

                int recordsAffected = DALManager.ExecuteNonQuery(dbCommand);

                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name.ToUpper() + " - " + MethodBase.GetCurrentMethod().DeclaringType.Name + " - Insert into hitlist");

                return id;
            }
            catch (Exception ex)
            {
                _coeLog.LogEnd(ex.Message);
                throw;
            }
            finally
            {
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name.ToUpper() + " - " + MethodBase.GetCurrentMethod().DeclaringType.Name);
            }
        }


        /// <summary>
        /// this function will create a new entry in temp hitlist table. it first reterieves the value of the next id to be inserted 
        /// by calling the getnewtemphitlistid.
        /// it inserts in two table the values simultaneously. first in csdohitlistid table with all the info being passed by temphitlistcreaterequestinfo
        /// and other csdohitlist where it stores the respective hitlistid with the ids of the hitlist returned. this entry will depend on the number_hits returned after a search.
        /// this functionis called by default. every serach that is made will first go in this table thereafter it is either usermarked or saved in user table.
        /// </summary>
        /// <param name="tempHitListCreateRequestInfo">it will take all the info required for csdohitlistid table like name,description, is_publice, number_hits, datecreated</param>
        /// <returns>the new id generated.</returns>
        public virtual int CreateNewSavedHitList(string name, bool isPublic, string description, string userName, int numHits, string databaseName, int hitlistID, int dataViewID, int searchCriteriaId, string searchCriteriaType)
        {
            try
            {
                if (name == string.Empty) { name = "saved"; }
                DateTime dateCreated = DateTime.Now;
                int id = GetSavedHitListID();
                
                if(hitlistID <= 0)
                    hitlistID = id;

                DbCommand dbCommand;

                string sql;
                //inserting the values in csdohitlistid table
                sql = "INSERT INTO " + _savedHitListIDTableName +
                            "(ID,HITLISTID,DATAVIEW_ID,NAME,DESCRIPTION,USER_ID,IS_PUBLIC,NUMBER_HITS,DATE_CREATED,DATABASE,SEARCH_CRITERIA_ID,SEARCH_CRITERIA_TYPE) VALUES " +
                            "(" + DALManager.BuildSqlStringParameterName("pID") +
                            " , " + DALManager.BuildSqlStringParameterName("pHitListID") +
                            " , " + DALManager.BuildSqlStringParameterName("pDataViewID") +
                            " , " + DALManager.BuildSqlStringParameterName("pHitListName") +
                            " , " + DALManager.BuildSqlStringParameterName("pDescription") +
                            " , " + DALManager.BuildSqlStringParameterName("pUserId") +
                            " , " + DALManager.BuildSqlStringParameterName("pIsPublic") +
                            " , " + DALManager.BuildSqlStringParameterName("pNumberHits") +
                            " , " + DALManager.BuildSqlStringParameterName("pDate") +
                            " , " + DALManager.BuildSqlStringParameterName("pDatabase") +
                            " , " + DALManager.BuildSqlStringParameterName("pSearchCriteriaId") +
                            " , " + DALManager.BuildSqlStringParameterName("pSearchCriteriaType") + " )";

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddInParameter(dbCommand, "pID", DbType.Int32, id);
                DALManager.Database.AddInParameter(dbCommand, "pHitListID", DbType.Int32, hitlistID);
                DALManager.Database.AddInParameter(dbCommand, "pDataViewID", DbType.Int32, dataViewID);
                DALManager.Database.AddParameter(dbCommand, "pHitListName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, name);
                DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, description);
                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userName.ToUpper());
                DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
                DALManager.Database.AddInParameter(dbCommand, "pNumberHits", DbType.Int32, numHits);
                DALManager.Database.AddInParameter(dbCommand, "pDate", DbType.Date, dateCreated);
                DALManager.Database.AddParameter(dbCommand, "pDatabase", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
                DALManager.Database.AddInParameter(dbCommand, "pSearchCriteriaId", DbType.Int32, searchCriteriaId);
                DALManager.Database.AddParameter(dbCommand, "pSearchCriteriaType", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, searchCriteriaType);

                int recordsAffected = -1;
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);

                return id;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// this will generate the new id from the userhitlist table to store the new entry.
        /// </summary>
        /// <param name="tempID">the id from csdohitlistd table which will be saved in userhitlist table or will be usermarked.</param>
        /// <param name="name">the name of the list tbe saved or usermarked</param>
        /// <param name="description">the description of the list</param>
        /// <returns>the new id generated</returns>
        public virtual int GetSavedHitListID()
        {
            return GetSavedHitListID();
        }

        ///// <summary>
        ///// this will generate the new id from the userhitlist table to store the new entry.
        ///// </summary>
        ///// <param name="tempID">the id from csdohitlistd table which will be saved in userhitlist table or will be usermarked.</param>
        ///// <param name="name">the name of the list tbe saved or usermarked</param>
        ///// <param name="description">the description of the list</param>
        ///// <returns>the new id generated</returns>
        //public virtual int GetMarkedHitListID(string user_ID, string formgroup)
        //{
        //    return GetMarkedHitListID(user_ID, formgroup);
        //}


        /// <summary>
        /// this will create the user hit list. the id which os selected from the temphitlist table. it wil ask for name of the list and the description of the list 
        /// and rest others values are reterieved from temp hit list table depending on the id selected.
        /// it will first call getnewuserhitlistid so as to get the next value of the id.
        /// it will store the entries in two table simultaneouly...userhitlistid and userhitlist. 
        /// in main table only the info is there ,and in the child table there are multiple entries i.e. storing the ids of the number_hits, along with the newid generated.
        /// it will be called when a user wants to save the list. 
        /// </summary>
        /// <param name="tempID">the tempid from csdohitlistid table which is being saved in user table</param>
        /// <param name="name">name of the list</param>
        /// <param name="description">description of the list</param>
        /// <returns></returns>
        public virtual int CreateNewSavedHitListFromTemp(int tempID, string name, string description, bool isPublic)
        {
            try
            {
                //inserting in userhitlistid table.
                string sql;
                DbCommand dbCommand;
                //this will return the next id to be inserted.
                int SavedHitListID = GetSavedHitListID();

                //we need to insert the values in both tables userhitlistid and userhitlist.
                //thus depending on the temp id we pick user_id, number_hits,is_public from csdohitlistid table
                //then inserts name, description, date_created along with the other info reterived.

                //insert into coesavehitlist
                sql = "INSERT INTO " + _savedHitListIDTableName +
                    "(ID,HITLISTID,DATAVIEW_ID,NAME,DESCRIPTION,DATE_CREATED,IS_PUBLIC,USER_ID,NUMBER_HITS,DATABASE)" +
                    " SELECT " + DALManager.BuildSqlStringParameterName("pSavedHitListID") + ",HITLISTID,DATAVIEW_ID,NAME,DESCRIPTION,DATE_CREATED,IS_PUBLIC,USER_ID,NUMBER_HITS,DATABASE from " + _tempHitListIDTableName +
                    " WHERE ID = " + DALManager.BuildSqlStringParameterName("pTempId");
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pSavedHitListID", DbType.Int32, SavedHitListID);
                DALManager.Database.AddInParameter(dbCommand, "pTempId", DbType.Int32, tempID);

                int recordsAffected = DALManager.ExecuteNonQuery(dbCommand);

                //insert into coesavehitlistid
                sql = "INSERT INTO " + _savedHitListTableName + "(ID,HITLISTID,SORTORDER,DATESTAMP) " +
                    " SELECT ID," + DALManager.BuildSqlStringParameterName("pSavedHitListID") + " ,SORTORDER, SYSDATE FROM " + _tempHitListTableName + " WHERE HITLISTID=" + DALManager.BuildSqlStringParameterName("pHitListID");
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pSavedHitListID", DbType.Int32, SavedHitListID);
                DALManager.Database.AddInParameter(dbCommand, "pHitListID", DbType.Int32, tempID);

                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);

                return SavedHitListID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// this will create the user hit list. the id which os selected from the temphitlist table. it wil ask for name of the list and the description of the list 
        /// and rest others values are reterieved from temp hit list table depending on the id selected.
        /// it will first call getnewuserhitlistid so as to get the next value of the id.
        /// it will store the entries in two table simultaneouly...userhitlistid and userhitlist. 
        /// in main table only the info is there ,and in the child table there are multiple entries i.e. storing the ids of the number_hits, along with the newid generated.
        /// it will be called when a user wants to save the list. 
        /// </summary>
        /// <param name="tempID">the tempid from csdohitlistid table which is being saved in user table</param>
        /// <param name="name">name of the list</param>
        /// <param name="description">description of the list</param>
        /// <returns></returns>
        public virtual int CreateNewSavedHitListFromExisting(int originalID, HitListType hlType, string name, string description, string userID, bool isPublic, int numHits, string database, int dataviewID, int searchCriteriaId, string searchCriteriaType)
        {
            try
            {
                //inserting in userhitlistid table.
                string sql;
                DbCommand dbCommand;
                //this will return the next id to be inserted.
                int SavedHitListID = GetSavedHitListID();

                //we need to insert the values in both tables userhitlistid and userhitlist.
                //thus depending on the temp id we pick user_id, number_hits,is_public from csdohitlistid table
                //then inserts name, description, date_created along with the other info reterived.

                string sourceTable = (hlType == HitListType.TEMP) ? _tempHitListTableName : _savedHitListTableName;
                sql = "INSERT INTO " + _savedHitListIDTableName +
                           "(ID,NAME,DATAVIEW_ID,DESCRIPTION,USER_ID,IS_PUBLIC,NUMBER_HITS,DATE_CREATED,DATABASE,HITLISTID,SEARCH_CRITERIA_ID,SEARCH_CRITERIA_TYPE) VALUES " +
                           "(" + DALManager.BuildSqlStringParameterName("pSavedHitListID") +
                           " ,  " + DALManager.BuildSqlStringParameterName("pName") +
                           " , " + DALManager.BuildSqlStringParameterName("pDataViewID") +
                           " , " + DALManager.BuildSqlStringParameterName("pDescription") +
                           " , " + DALManager.BuildSqlStringParameterName("pUserId") +
                           " , " + DALManager.BuildSqlStringParameterName("pIsPublic") +
                           " , " + DALManager.BuildSqlStringParameterName("pNumberHits") +
                           " , " + DALManager.BuildSqlStringParameterName("pDate") +
                           " , " + DALManager.BuildSqlStringParameterName("pDatabase") +
                           " , " + DALManager.BuildSqlStringParameterName("pHitListID") +
                           " , " + DALManager.BuildSqlStringParameterName("pSearchCriteriaId") +
                           " , " + DALManager.BuildSqlStringParameterName("pSearchCriteriaType") + " )";

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddInParameter(dbCommand, "pSavedHitListID", DbType.Int32, SavedHitListID);
                DALManager.Database.AddParameter(dbCommand, "pName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, name);
                DALManager.Database.AddInParameter(dbCommand, "pDataViewID", DbType.Int32, dataviewID);
                DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, description);
                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userID.ToUpper());
                DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
                DALManager.Database.AddInParameter(dbCommand, "pNumberHits", DbType.Int32, numHits);
                DALManager.Database.AddInParameter(dbCommand, "pDate", DbType.Date, DateTime.Now);
                DALManager.Database.AddParameter(dbCommand, "pDatabase", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, database);
                DALManager.Database.AddInParameter(dbCommand, "pHitListID", DbType.Int32, SavedHitListID);
                DALManager.Database.AddInParameter(dbCommand, "pSearchCriteriaId", DbType.Int32, searchCriteriaId);
                DALManager.Database.AddParameter(dbCommand, "pSearchCriteriaType", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, searchCriteriaType);

                int recordsAffected = DALManager.ExecuteNonQuery(dbCommand);

                //insert into coesavehitlistid
                sql = "INSERT INTO " + _savedHitListTableName + "(ID,HITLISTID,SORTORDER, DATESTAMP) " +
                    " SELECT ID," + DALManager.BuildSqlStringParameterName("pSavedHitListID") + " ,SORTORDER, DATESTAMP FROM " + sourceTable + " WHERE HITLISTID=" + DALManager.BuildSqlStringParameterName("pHitListID");
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pSavedHitListID", DbType.Int32, SavedHitListID);
                DALManager.Database.AddInParameter(dbCommand, "pHitListID", DbType.Int32, originalID);

                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);

                return SavedHitListID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// update a user hit list. it will uodate info like name, description, is_public for the id which is bieng passed
        /// it accepts a row of the dataset with all the values.
        /// </summary>
        /// <param name="hitListRow">row of the dataset to pass all the values.</param>
        public void UpdateSavedHitList(int id, string hitListName, string description, bool isPublic, int numHits, int hitlistID, int dataViewID, int parentHitListID, int searchCriteriaId, string searchCriteriaType)
        {
            try
            {
                string sql = "UPDATE " + _savedHitListIDTableName +
                           " SET " +
                           " NAME =  " + DALManager.BuildSqlStringParameterName("pHitListName") +
                           " , DESCRIPTION = " + DALManager.BuildSqlStringParameterName("pDescription") +
                           " , IS_PUBLIC =  " + DALManager.BuildSqlStringParameterName("pIsPublic") +
                           " , NUMBER_HITS = " + DALManager.BuildSqlStringParameterName("pNumHits") +
                           " , DATAVIEW_ID = " + DALManager.BuildSqlStringParameterName("pDataViewID") +
                           ((hitlistID > 0) ? " , HITLISTID = " + DALManager.BuildSqlStringParameterName("pHitListID") : " ") +
                           " , SEARCH_CRITERIA_ID = " + DALManager.BuildSqlStringParameterName("pSearchCriteriaId") +
                           " , SEARCH_CRITERIA_TYPE = " + DALManager.BuildSqlStringParameterName("pSearchCriteriaType") +
                           "  WHERE ID =" + DALManager.BuildSqlStringParameterName("pID");

                DbCommand dbCommand;
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pHitListName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, hitListName);
                DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, description);
                DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
                DALManager.Database.AddInParameter(dbCommand, "pNumHits", DbType.Int32, numHits);
                DALManager.Database.AddInParameter(dbCommand, "pDataViewID", DbType.Int32, dataViewID);
                if(hitlistID > 0) {
                    DALManager.Database.AddInParameter(dbCommand, "pHitListID", DbType.Int32, hitlistID);
                }
                DALManager.Database.AddInParameter(dbCommand, "pSearchCriteriaId", DbType.Int32, searchCriteriaId);
                DALManager.Database.AddParameter(dbCommand, "pSearchCriteriaType", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, searchCriteriaType);
                DALManager.Database.AddInParameter(dbCommand, "pID", DbType.Int32, id);
                int recordsAffected = -1;
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// update the temp hit list. it will update info like name, description, is_public for the id which is bieng passed
        /// it accepts a row of the dataset with all the values.
        /// </summary>
        /// <param name="hitListRow">row of the dataset to pass all the values.</param>
        public void UpdateTempHitList(int id, string hitListName, string description, bool isPublic, int numHits, int hitlistID, int dataViewID, int parentHitListID, int searchCriteriaId, string searchCriteriaType)
        {
            try
            {
                //writing the normal sql statements--megha (edit begins)
                string sql = "UPDATE " + _tempHitListIDTableName +
                           " SET " +
                           " NAME = " + DALManager.BuildSqlStringParameterName("pHitListName") +
                           " , DESCRIPTION =" + DALManager.BuildSqlStringParameterName("pDescription") +
                           " , IS_PUBLIC = " + DALManager.BuildSqlStringParameterName("pIsPublic") +
                           " , NUMBER_HITS = " + DALManager.BuildSqlStringParameterName("pNumberHits") +
                           " , DATAVIEW_ID = " + DALManager.BuildSqlStringParameterName("pDataViewID") +
                           ((hitlistID > 0) ? " , HITLISTID = " + DALManager.BuildSqlStringParameterName("pHitListID") : " ") +
                           " , SEARCH_CRITERIA_ID = " + DALManager.BuildSqlStringParameterName("pSearchCriteriaId") +
                           " , SEARCH_CRITERIA_TYPE = " + DALManager.BuildSqlStringParameterName("pSearchCriteriaType") +
                           " WHERE ID =" + DALManager.BuildSqlStringParameterName("pID");

                DbCommand dbCommand;
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pHitListName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, hitListName);
                DALManager.Database.AddParameter(dbCommand, "pDescription", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, description);
                DALManager.Database.AddParameter(dbCommand, "pIsPublic", DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
                DALManager.Database.AddInParameter(dbCommand, "pNumberHits", DbType.Int32, numHits);
                DALManager.Database.AddInParameter(dbCommand, "pDataViewID", DbType.Int32, dataViewID);
                if(hitlistID > 0) {
                    DALManager.Database.AddInParameter(dbCommand, "pHitListID", DbType.Int32, hitlistID);
                }
                DALManager.Database.AddInParameter(dbCommand, "pSearchCriteriaId", DbType.Int32, searchCriteriaId);
                DALManager.Database.AddParameter(dbCommand, "pSearchCriteriaType", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, searchCriteriaType);
                DALManager.Database.AddInParameter(dbCommand, "pID", DbType.Int32, id);
                int recordsAffected = -1;
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates a temporary hitlist by assigning the associated search criteria id and type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="searchCriteriaId"></param>
        /// <param name="searchCriteriaType"></param>
        internal void AssociateSearchCriteriaWithTempHitList(int id, int searchCriteriaId, string searchCriteriaType)
        {
            try
            {
                //writing the normal sql statements--megha (edit begins)
                string sql = "UPDATE " + _tempHitListIDTableName +
                           " SET " +
                           " SEARCH_CRITERIA_ID = " + DALManager.BuildSqlStringParameterName("pSearchCriteriaId") +
                           " , SEARCH_CRITERIA_TYPE = " + DALManager.BuildSqlStringParameterName("pSearchCriteriaType") +
                           " WHERE ID =" + DALManager.BuildSqlStringParameterName("pID");

                DbCommand dbCommand;
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pSearchCriteriaId", DbType.Int32, searchCriteriaId);
                DALManager.Database.AddParameter(dbCommand, "pSearchCriteriaType", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, searchCriteriaType);
                DALManager.Database.AddInParameter(dbCommand, "pID", DbType.Int32, id);
                int recordsAffected = -1;
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates a saved hitlist by assigning the associated search criteria id and type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="searchCriteriaId"></param>
        /// <param name="searchCriteriaType"></param>
        internal void AssociateSearchCriteriaWithSavedHitList(int id, int searchCriteriaId, string searchCriteriaType)
        {
            try
            {
                //writing the normal sql statements--megha (edit begins)
                string sql = "UPDATE " + _savedHitListIDTableName +
                           " SET " +
                           " SEARCH_CRITERIA_ID = " + DALManager.BuildSqlStringParameterName("pSearchCriteriaId") +
                           " , SEARCH_CRITERIA_TYPE = " + DALManager.BuildSqlStringParameterName("pSearchCriteriaType") +
                           " WHERE ID =" + DALManager.BuildSqlStringParameterName("pID");

                DbCommand dbCommand;
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pSearchCriteriaId", DbType.Int32, searchCriteriaId);
                DALManager.Database.AddParameter(dbCommand, "pSearchCriteriaType", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, searchCriteriaType);
                DALManager.Database.AddInParameter(dbCommand, "pID", DbType.Int32, id);
                int recordsAffected = -1;
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// delete a hitlist
        /// </summary>
        /// <param name="hitListType"></param>
        public void DeleteHitList(HitListType hitListType, int hitListID)
        {
            try
            {
                string sql = string.Empty;
                DbCommand dbCommand;
                int recordsAffected = -1;
                string hitListId = null;
                switch (hitListType)
                {
                    case HitListType.TEMP:
                        _hitListTableName = _tempHitListTableName;
                        _hitListIDTableName = _tempHitListIDTableName;
                        break;
                    case HitListType.SAVED:
                        _hitListTableName = _savedHitListTableName;
                        _hitListIDTableName = _savedHitListIDTableName;
                        break;
                    case HitListType.MARKED:
                        _hitListTableName = _savedHitListTableName;
                        _hitListIDTableName = _savedHitListIDTableName;
                        break;
                }


                sql = "DELETE FROM " + _hitListTableName + " WHERE HITLISTID =" + DALManager.BuildSqlStringParameterName("pHitListId");
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pHitListId"), DbType.Int32, hitListID);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);


                //then deleting the records from main table-- csdohitlistid or userhitlistid
                sql = "DELETE FROM " + _hitListIDTableName + " WHERE HITLISTID =" + DALManager.BuildSqlStringParameterName("pHitListId");
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pHitListId"), DbType.Int32, hitListID);
                DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// delete all hitlists of a certain type
        /// </summary>
        /// <param name="hitListType"></param>
        ///this should be overridden for oracle to truncate!!!  
        public void DeleteAllHitLists(string hitListType)
        {
            try
            {
                string sql = string.Empty;
                DbCommand dbCommand;
                int recordsAffected = -1;
                string hitListId = null;
                switch (hitListType)
                {
                    case "TEMP":
                        _hitListTableName = _tempHitListTableName;
                        _hitListIDTableName = _tempHitListIDTableName;
                        break;
                    case "SAVED":
                        _hitListTableName = _savedHitListTableName;
                        _hitListIDTableName = _savedHitListIDTableName;
                        break;
                }

                sql = "DELETE FROM " + _hitListTableName;
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pHitListId", DbType.Int32, hitListId);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);

                //then deleting the records from main table-- csdohitlistid or userhitlistid
                sql = "DELETE FROM " + _hitListIDTableName;
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pHitListId", DbType.Int32, hitListId);
                DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// delete all hitlists of a certain type for a single user
        /// </summary>
        /// <param name="hitListType"></param>
        public void DeleteHitLists(string hitListType, string userName)
        {
            try
            {
                string sql = string.Empty;
                DbCommand dbCommand;
                int recordsAffected = -1;
                string hitListId = null;
                switch (hitListType)
                {
                    case "TEMP":
                        _hitListTableName = _tempHitListTableName;
                        _hitListIDTableName = _tempHitListIDTableName;
                        break;
                    case "SAVED":
                        _hitListTableName = _savedHitListTableName;
                        _hitListIDTableName = _savedHitListIDTableName;
                        break;
                }

                sql = "DELETE FROM " + _hitListTableName + " WHERE HITLISTID IN(select id from " + _hitListTableName + " where user_id=" + DALManager.BuildSqlStringParameterName("puserName") + ")";
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userName.ToUpper());
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);

                sql = "DELETE FROM " + _hitListIDTableName + " WHERE ID IN(select id from " + _hitListTableName + " where user_id=" + DALManager.BuildSqlStringParameterName("puserName") + ")";
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userName.ToUpper());
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        /// <summary>
        /// this function will add all hits to the usermarked records. it will accept a tempid from the temporarry table and will pick its corresponding details from both of the temp (mailn 
        /// and child) table and will save the list in user table with the name as USERMARKED.
        /// it will save in two steps...first the entry goes in main user table and the corresponding multiple entries goes in child user table.
        /// it will first call th function to reterieve the maximum id from the user table and then wil save the values in both tables.
        /// </summary>
        /// <param name="userID">the current user that is logged in</param>
        /// <param name="tempID">the tempid from temp table which is made as usermarked.</param>
        public void AddAllMarkedHit(string userID, int tempID, string databaseName)
        {
            try
            {
                DbCommand dbCommand;
                string sql;
                string name = "USERMARKED";

                //we need to get the maximum id from USERHITLISTID table 
                int SavedHitListId = GetSavedHitListID();

                //now we will insert the values in USERHITLISTID table, where name will be usermarked,
                //number_hits, is_public will be from csdohitlistid w.r.t. to id
                sql = "INSERT INTO " + _savedHitListIDTableName +
                    " (NAME,ID,DATE_CREATED,USER_ID,NUMBER_HITS,IS_PUBLIC,DATABASE) " +
                    " SELECT " + DALManager.BuildSqlStringParameterName("pHitListName") + " , " + DALManager.BuildSqlStringParameterName("pHitListId") +
                    "," + DALManager.BuildSqlStringParameterName("pDate") +
                    " , USER_ID,NUMBER_HITS,IS_PUBLIC,DATABASE FROM " + _tempHitListIDTableName +
                    " WHERE ID= " + DALManager.BuildSqlStringParameterName("pTempId");

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pHitListName", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, name);
                DALManager.Database.AddInParameter(dbCommand, "pHitListId", DbType.Int32, SavedHitListId);
                DALManager.Database.AddInParameter(dbCommand, "pDate", DbType.Date, DateTime.Now);
                DALManager.Database.AddInParameter(dbCommand, "pTempId", DbType.Int32, tempID);
                int recordsAffected = -1;
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);

                //now inserting in the userhitlist table the corressponding multiple entries.
                sql = "SELECT NUMBER_HITS FROM " + _savedHitListIDTableName + " WHERE ID= " + DALManager.BuildSqlStringParameterName("pHitListId");
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pHitListId", DbType.Int32, SavedHitListId);
                int numberhits = Convert.ToInt32(DALManager.Database.ExecuteScalar(dbCommand));

                for (int count = 1; count <= numberhits; count++)
                {
                    sql = "INSERT INTO " + _savedHitListTableName + "(HITLISTID,ID)" +
                        " VALUES( " + DALManager.BuildSqlStringParameterName("pHitListId") + " , " + count + " )";
                    dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                    DALManager.Database.AddInParameter(dbCommand, "pHitListId", DbType.Int32, SavedHitListId);
                    recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// this function will mark one oe many hitlists in the usermarked list. it will take an id and its corresponding ids of the histlists
        /// in the form of array which are to be marked
        /// it will first call the function to reterive the maximum id from the user table and then will store the values in user tables both...child and main table.
        /// it will store the list by the name of USERMARKED.
        /// </summary>
        /// <param name="userId">the current user that is logged in</param>
        /// <param name="hitsToBeMarked">the array of the hitlists to be marked ny the user</param>
        /// <param name="tempId">the id from temp table whose hitlists are to be marked.</param>
        public virtual int AddToMarkedHitList(int markedHitListID, int[] hitsToBeMarked, out int recordsAffected)
        {
            try
            {
                string sql = string.Empty;
                recordsAffected = 0;
                DbCommand dbCommand = null;
                //Insert in temphitlist table
                for (int i = 0; i <= hitsToBeMarked.Length - 1; i++)
                {
                    sql = "INSERT INTO " + _savedHitListTableName + " (HITLISTID,ID,DATESTAMP,SORTORDER) " +
                        " VALUES (" + DALManager.BuildSqlStringParameterName("pMarkedHitListId") + ", " + 
                        DALManager.BuildSqlStringParameterName("pHitID") + ", " + 
                        DALManager.BuildSqlStringParameterName("pDate") + ", " + 
                        DALManager.BuildSqlStringParameterName("pSortOrder") + ")" +
                        " WHERE ID=" + DALManager.BuildSqlStringParameterName("pMarkedHitListId");
                    dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                    DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pMarkedHitListId"), DbType.Int32, markedHitListID);
                    DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pHitID"), DbType.Int32, hitsToBeMarked[i]);
                    DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pDate"), DbType.DateTime, DateTime.Now);
                    DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pSortOrder"), DbType.Int32, i);
                    DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pMarkedHitListId"), DbType.Int32, markedHitListID);
                    
                    recordsAffected += DALManager.ExecuteNonQuery(dbCommand);
                }

                //Update temphitlistid table
                sql = "UPDATE" + _savedHitListIDTableName +
                    " SET NUMBER_HITS = (SELECT COUNT(*) FROM " + _savedHitListTableName + " WHERE HITLISTID=" + DALManager.BuildSqlStringParameterName("pMarkedHitListID") + ")" +
                    " WHERE ID = " + DALManager.BuildSqlStringParameterName("pMarkedHitListID") + ")";

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pMarkedHitListID"), DbType.Int32, markedHitListID);
                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pMarkedHitListID"), DbType.Int32, markedHitListID);
                
                DALManager.ExecuteNonQuery(dbCommand);

                if(recordsAffected < 0)
                    recordsAffected = 0;

                return markedHitListID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
  

        public virtual int RemoveFromMarkedHitList(int markedHitListID, int[] hitsToBeUnmarked, out int recordsAffected) {
            try {
                string sql = string.Empty;
                recordsAffected = 0;
                DbCommand dbCommand = null;
                //Delete from temphitlist table
                for(int i = 0; i <= hitsToBeUnmarked.Length - 1; i++) {
                    sql =   "DELETE FROM " + _savedHitListTableName + 
                            " WHERE HITLISTID=" + DALManager.BuildSqlStringParameterName("pMarkedHitListId") + 
                            " AND ID=" + DALManager.BuildSqlStringParameterName("pHitID");

                    dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                    DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pMarkedHitListId"), DbType.Int32, markedHitListID);
                    DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pHitID"), DbType.Int32, hitsToBeUnmarked[i]);

                    recordsAffected += DALManager.ExecuteNonQuery(dbCommand);
                }

                //Update temphitlistid table
                sql = "UPDATE" + _savedHitListIDTableName +
                    " SET NUMBER_HITS = (SELECT COUNT(*) FROM " + _savedHitListTableName + 
                                        " WHERE HITLISTID=" + DALManager.BuildSqlStringParameterName("pMarkedHitListID") + ")" +
                    " WHERE ID = " + DALManager.BuildSqlStringParameterName("pMarkedHitListID") + ")";

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pMarkedHitListID"), DbType.Int32, markedHitListID);
                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pMarkedHitListID"), DbType.Int32, markedHitListID);

                DALManager.ExecuteNonQuery(dbCommand);

                if(recordsAffected < 0)
                    recordsAffected = 0;

                return markedHitListID;
            } catch(Exception ex) {
                throw;
            }
        }

        public virtual void InsertHits(int[] hitsArray1, double[] hitsArray2, int hitListID, HitListType hitListType)
        {
            throw new Exception("not implemented");
        }

        public virtual void UpdateHits(int[] hitsArray1, double[] hitsArray2, int hitListID, HitListType hitListType)
        {
            throw new Exception("not implemented");
        }

        /// <summary>
        /// to unmark all the marked records. all the USERMARKED records are unmarked.in the case the number of hits of all the USERMARKED records will turn zero from the main user table
        /// and will delete all the hitlistids which are usermarked from the user child table.
        ///  
        /// </summary>
        /// <param name="userId">the current user that is logged in.</param>
        public void UnMarkAllMarked(string userId, int dataviewId)
        {
            try
            {
                string sql;
                DbCommand dbCommand;

                //first deleting from child table
                sql = "DELETE FROM " + _savedHitListTableName + " WHERE HITLISTID IN " +
                    " (SELECT ID FROM " + _savedHitListIDTableName + " WHERE TYPE='MARKED' " +
                    " AND LTRIM(RTRIM(USER_ID))= LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserId") + "))" +
                    " AND DATAVIEW_ID =" + DALManager.BuildSqlStringParameterName("pDatviewId") + ")";
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId.ToUpper());
                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pDataviewId"), DbType.Int32, dataviewId);
                int recordsAffected = -1;
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);

                //setting the number hits in main table to 0
                sql = "UPDATE " + _savedHitListIDTableName +
                    " SET NUMBER_HITS=0 " +
                    " WHERE LTRIM(RTRIM(USER_ID))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserId") + ")) AND TYPE='MARKED'" +
                    "AND DATAVIEW_ID =" + DALManager.BuildSqlStringParameterName("pDataviewId");
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userId.ToUpper());
                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pDataviewId"), DbType.Int32, dataviewId);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual int MarkAllHits(int hitListId, int markedHitListId,HitListType hitListType, int dataviewId, int markedHitsMax) 
        {
            return -1;
        }

        /// <summary>
        /// to unmark one or many USERMARKED records from the userhitlist. it will decrease the number_hits by the number of the hits that have been unmarked.
        /// and will delete the same entries from user child table.
        /// it will accept an array of hitlistids to be unmarked.
        /// and a id for which hitlists are unmarked.
        /// </summary>
        /// <param name="userId">the current user logged in</param>
        /// <param name="hitsToBeUnmarked">array of hitlist to be unmarked</param>
        /// <param name="hitlistId">the id for which we are unmarking the records.</param>
        public void UnMarkHitList(string userId, int[] hitsToBeUnmarked, int hitlistId)
        {
            try
            {
                //to unmark one or many records, thus using and array
                string sql;
                DbCommand dbCommand;
                int recordsAffected = -1;
                //thus deleting from userhitlist table the ids which are unmarked.
                for (int i = 0; i <= hitsToBeUnmarked.Length - 1; i++)
                {
                    sql = "DELETE FROM " + _savedHitListTableName +
                        " WHERE ID=" + DALManager.BuildSqlStringParameterName("pHitsToBeUnmarked") +
                        " AND HITLISTID = " + DALManager.BuildSqlStringParameterName("pHitListId");
                    dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                    DALManager.Database.AddInParameter(dbCommand, "pHitsToBeUnmarked", DbType.Int32, hitsToBeUnmarked[i]);
                    DALManager.Database.AddInParameter(dbCommand, "pHitListId", DbType.Int32, hitlistId);
                    recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
                }
                //updating the userhitlistid table
                sql = "UPDATE " + _savedHitListIDTableName +
                    " SET NUMBER_HITS= NUMBER_HITS-" + hitsToBeUnmarked.Length +
                    " WHERE ID= " + DALManager.BuildSqlStringParameterName("pHitListId");
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pHitsToBeUnmarkedLength", DbType.Int32, hitsToBeUnmarked.Length);
                DALManager.Database.AddInParameter(dbCommand, "pHitListId", DbType.Int32, hitlistId);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// to delete one or many usermarked hitlists for the given hitlistid. it will delete those ids from the user child table and will update the number_hits 
        /// in the main user table by subtracting from the number hits , the number that has been deleted.
        /// it will accept an array of ids to be deleted.
        /// </summary>
        /// <param name="userID">the current user logged in</param>
        /// <param name="markedIDArray">the array of the ids to be deleted</param>
        /// <param name="hitlistID">the id for which it is deleted.</param>
        public void DeleteMarkedHit(string userID, int[] markedIDArray, int hitlistID)
        {
            try
            {
                string sql;
                DbCommand dbCommand;

                int recordsAffected = -1;
                for (int i = 0; i <= markedIDArray.Length - 1; i++)
                {
                    sql = "DELETE FROM " + _savedHitListTableName + " WHERE ID =" + DALManager.BuildSqlStringParameterName("pMarkedIDArray") + "  AND HITLISTID= " + DALManager.BuildSqlStringParameterName("pHitListId");
                    dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                    DALManager.Database.AddInParameter(dbCommand, "pMarkedIDArray", DbType.Int32, markedIDArray[i]);
                    DALManager.Database.AddInParameter(dbCommand, "pHitListId", DbType.Int32, hitlistID);
                    recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
                }

                // int numberMarked=Convert.ToInt32(markedIDArray.Length);

                sql = "UPDATE " + _savedHitListIDTableName +
                    " SET " +
                    " NUMBER_HITS=NUMBER_HITS-" + DALManager.BuildSqlStringParameterName("pMarkedIDArrayLength") +
                    " WHERE ID=" + DALManager.BuildSqlStringParameterName("pHitListId");
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pMarkedIDArrayLength", DbType.Int32, markedIDArray.Length);
                DALManager.Database.AddInParameter(dbCommand, "pHitListId", DbType.Int32, hitlistID);
                recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// delete all the usermarked hit list. it will delete all the records from the user child table and then will delete all the USERMARKED records
        /// from user main table.
        /// </summary>
        /// <param name="userID">the current user logged in</param>
        public void DeleteAllMarkedHits(string userID)
        {
            try
            {
                string sql;
                DbCommand dbCommand;

                int recordAffected = -1;

                //deleting from child table
                sql = "DELETE FROM " + _savedHitListTableName +
                      " WHERE HITLISTID IN " +
                      "(SELECT ID FROM " + _savedHitListIDTableName + " WHERE LTRIM(RTRIM(USER_ID))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserId") + " )) AND LTRIM(RTRIM(NAME))='USERMARKED')";
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userID.ToUpper());
                recordAffected = DALManager.ExecuteNonQuery(dbCommand);

                //deleting from main table
                sql = "DELETE FROM " + _savedHitListIDTableName +
                    " WHERE ID IN " +
                   " (SELECT ID FROM " + _savedHitListIDTableName + " WHERE LTRIM(RTRIM(USER_ID))=LTRIM(RTRIM(" + DALManager.BuildSqlStringParameterName("pUserId") + ")) AND LTRIM(RTRIM(NAME))= 'USERMARKED')";
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pUserId", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userID.ToUpper());
                recordAffected = DALManager.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public SafeDataReader GetHitLists(string userID, HitListType hitListType, bool isPublic, string databaseName)
        {
            try
            {
                string sql = null;
                DbCommand dbCommand;
                SafeDataReader returnSafeDataReader = null;
                string fullTableName = string.Empty;
                //to capture the ids coming in the array, and inserting a comma in between the values data
                string id = null;

                switch (hitListType)
                {
                    case HitListType.TEMP:
                        fullTableName = _tempHitListIDTableName;
                        break;
                    case HitListType.SAVED:
                        fullTableName = _savedHitListIDTableName;
                        break;
                    case HitListType.ALL:
                        fullTableName = _savedHitListIDTableName + ", " + _tempHitListIDTableName;
                        break;
                    case HitListType.MARKED:
                        fullTableName = _savedHitListIDTableName;
                        break;
                }

				//JHS 8/20/2008 added order by clause, because I think it is needed to display
				//A nice overload would be to allow the ordering to be specified like by id, name, date, etc
                sql = "SELECT ID,HITLISTID,NAME, DESCRIPTION, IS_PUBLIC, NUMBER_HITS, USER_ID, DATE_CREATED,DATABASE,DATAVIEW_ID,PARENT_HITLIST_ID,TYPE,SEARCH_CRITERIA_ID,SEARCH_CRITERIA_TYPE" +
                " FROM " + fullTableName +
                " WHERE USER_ID=" + DALManager.BuildSqlStringParameterName("pUserID") +
                " AND IS_PUBLIC=" + DALManager.BuildSqlStringParameterName("pIsPublic") +
                " AND DATABASE=" + DALManager.BuildSqlStringParameterName("pDatabase") +
				" ORDER BY ID DESC";

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pUserID", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userID.ToUpper());
                DALManager.Database.AddParameter(dbCommand, "pisPublic", DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, Convert.ToInt16(isPublic));
                DALManager.Database.AddParameter(dbCommand, "pDatabase", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);

                returnSafeDataReader = (SafeDataReader)DALManager.Database.ExecuteReader(dbCommand);
                return returnSafeDataReader; //watched through quick watch dataset visualizer
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public SafeDataReader GetHitLists(HitListType hitListType, string databaseName)
        {
            try
            {
                string sql = null;
                DbCommand dbCommand;

                string fullTableName = string.Empty;
                //to capture the ids coming in the array, and inserting a comma in between the values data
                string id = null;

                switch (hitListType)
                {
                    case HitListType.TEMP:
                        fullTableName = _tempHitListIDTableName;
                        break;
                    case HitListType.SAVED:
                        fullTableName = _savedHitListIDTableName;
                        break;
                    case HitListType.ALL:
                        fullTableName = _savedHitListIDTableName + ", " + _tempHitListIDTableName;
                        break;
                    case HitListType.MARKED:
                        fullTableName = _savedHitListIDTableName;
                        break;
                }

				//JHS 8/20/2008 added order by clause, because I think it is needed to display
				//A nice overload would be to allow the ordering to be specified like by id, name, date, etc
                sql = "SELECT ID,HITLISTID,NAME, DESCRIPTION, IS_PUBLIC, NUMBER_HITS, USER_ID, DATE_CREATED,DATABASE,DATAVIEW_ID,PARENT_HITLIST_ID,TYPE,SEARCH_CRITERIA_ID,SEARCH_CRITERIA_TYPE" +
                " FROM " + fullTableName + " WHERE DATABASE=" + DALManager.BuildSqlStringParameterName("pDatabase") + " ORDER BY ID DESC";

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, "pDatabase", DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader; //watched through quick watch dataset visualizer
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public SafeDataReader GetHitLists(HitListType hitListType)
        {
            try
            {
                string sql = null;
                DbCommand dbCommand;

                string fullTableName = string.Empty;
                //to capture the ids coming in the array, and inserting a comma in between the values data
                string id = null;

                switch (hitListType)
                {
                    case HitListType.TEMP:
                        fullTableName = _tempHitListIDTableName;
                        break;
                    case HitListType.SAVED:
                        fullTableName = _savedHitListIDTableName;
                        break;
                    case HitListType.ALL:
                        fullTableName = _savedHitListIDTableName + ", " + _tempHitListIDTableName;
                        break;
                    case HitListType.MARKED:
                        fullTableName = _savedHitListIDTableName;
                        break;
                }

				//JHS 8/20/2008 added order by clause, because I think it is needed to display
				//A nice overload would be to allow the ordering to be specified like by id, name, date, etc
                sql = "SELECT ID, HITLISTID, NAME, DESCRIPTION, IS_PUBLIC, NUMBER_HITS, USER_ID, DATE_CREATED, DATABASE, DATAVIEW_ID, PARENT_HITLIST_ID, TYPE,SEARCH_CRITERIA_ID,SEARCH_CRITERIA_TYPE" +
                " FROM " + fullTableName + " WHERE ID>0 ORDER BY ID DESC";

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader; //watched through quick watch dataset visualizer
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public SafeDataReader GetHitList(int id, HitListType hitListType)
        {
            try
            {
                string sql = null;
                DbCommand dbCommand;

                string fullTableName = string.Empty;
                //to capture the ids coming in the array, and inserting a comma in between the values data

                switch (hitListType)
                {
                    case HitListType.TEMP:
                        fullTableName = _tempHitListIDTableName;
                        break;
                    case HitListType.SAVED:
                        fullTableName = _savedHitListIDTableName;
                        break;
                    case HitListType.ALL:
                        fullTableName = _savedHitListIDTableName + ", " + _tempHitListIDTableName;
                        break;
                    case HitListType.MARKED:
                        fullTableName = _savedHitListIDTableName;
                        break;
                }

                sql = "SELECT ID, HITLISTID, NAME, DESCRIPTION, IS_PUBLIC, NUMBER_HITS, USER_ID, DATE_CREATED, DATABASE, DATAVIEW_ID, PARENT_HITLIST_ID, TYPE,SEARCH_CRITERIA_ID,SEARCH_CRITERIA_TYPE" +
                " FROM " + fullTableName + " WHERE ID = " + DALManager.BuildSqlStringParameterName("pHitListId");

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pHitListId", DbType.Int32, id);
                SafeDataReader safeReader = new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
                return safeReader; //watched through quick watch dataset visualizer
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual int AddToRecent(int id, int hitListID, HitListType type, string name, string description, string userID, bool isPublic, int numHits, string databaseName, int dataviewID, int parentHitListID, string originalType, int searchCriteriaId, string searchCriteriaType) {
            return -1;
        }

        public virtual SafeDataReader GetRecentHitLists(string userID, int dataviewID, string databaseName, int quantityToRetrieve) {
            return null;
        }

        internal SafeDataReader GetSavedHitLists(string userID, int dataviewID, string databaseName) {
            try {
                string sql = null;
                DbCommand dbCommand;
                SafeDataReader returnSafeDataReader = null;
                sql = "SELECT ID,HITLISTID,NAME, DESCRIPTION, IS_PUBLIC, NUMBER_HITS, USER_ID, DATE_CREATED,DATABASE,DATAVIEW_ID,PARENT_HITLIST_ID,TYPE,SEARCH_CRITERIA_ID,SEARCH_CRITERIA_TYPE" +
                " FROM " + _savedHitListIDTableName +
                " WHERE (USER_ID=" + DALManager.BuildSqlStringParameterName("pUserID") +
                " OR IS_PUBLIC=" + DALManager.BuildSqlStringParameterName("pIsPublic") + ")" +
                " AND DATABASE=" + DALManager.BuildSqlStringParameterName("pDatabase") +
                " AND DATAVIEW_ID=" + DALManager.BuildSqlStringParameterName("pDataViewID") +
                " AND (TYPE<>'MARKED' OR TYPE IS NULL)"+
                " ORDER BY ID DESC";

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pUserID"), DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userID.ToUpper());
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pisPublic"), DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, "1");
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pDatabase"), DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pDataViewID"), DbType.Int32, dataviewID);

                return new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            } catch(Exception ex) {
                throw;
            }
        }

        internal virtual SafeDataReader GetMarkedHitList(string databaseName, string userName, int dataViewID) {
            return null;
        }

        internal virtual int SynchronizeRecordCount(int hitlistID, HitListType hitlistType, string baseTable, string primaryKeyName) {
            return -1;
        }
    }
}
