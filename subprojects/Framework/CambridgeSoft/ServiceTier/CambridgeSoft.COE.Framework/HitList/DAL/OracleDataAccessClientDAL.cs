
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Data;
using System.Data.Common;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Reflection;
using Csla.Data;
using System.Text.RegularExpressions;
using CambridgeSoft.COE.Framework.COEDataViewService;

namespace CambridgeSoft.COE.Framework.COEHitListService
{
    /// <summary>
    /// 
    /// </summary>
    public class OracleDataAccessClientDAL : CambridgeSoft.COE.Framework.COEHitListService.DAL
    {
        /// <summary>
        /// 
        /// </summary>
        public override int AddToMarkedHitList(int markedHitListID, int[] hitsToBeMarked, out int recordsAffected)
        {
            try
            {

                //Insert in savedhitlistid

                string anonymous_block = "DECLARE " + "\n" +
                                         "TYPE hitsArray IS TABLE OF NUMBER INDEX BY BINARY_INTEGER;\n" +
                                         "biggestSortOrderParam NUMBER(9) := 0;\n" +
                                         "recordsAffected NUMBER(9) := 0;\n" +
                                         "numHitsParam NUMBER(9) := " + DALManager.BuildSqlStringParameterName("pHitsNum") + ";\n" +
                                         "markedHitListIDParam NUMBER(9) := " + DALManager.BuildSqlStringParameterName("pMarkedHitListID") + ";\n" +
                                         "hitsToBeMarkedParam hitsArray := " + DALManager.BuildSqlStringParameterName("pHitsToBeMarked") + ";\n" +
                                         "BEGIN " +
                                            "SELECT COUNT(*) INTO biggestSortOrderParam FROM " + _savedHitListTableName + " WHERE HITLISTID=markedHitListIDParam;\n" +
                                            "FOR I IN 1 .. numHitsParam LOOP \n" +
                    //insert into coesavedhitlist (select 1,28 ,sysdate,null from dual where not exists (select 1 from coesavedhitlist where hitlistid=1 and id=28) );
                                                 "INSERT INTO " + _savedHitListTableName + " (HITLISTID,ID,DATESTAMP,SORTORDER) \n" +
                                                   "(SELECT markedHitListIDParam, hitsToBeMarkedParam(I), SYSDATE, (biggestSortOrderParam+I) \n" +
                                                    "FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM " + _savedHitListTableName + " WHERE HITLISTID=markedHitListIDParam AND ID=hitsToBeMarkedParam(I)));\n" +
                                                 "recordsAffected := recordsAffected + SQL%ROWCOUNT;\n" +
                                            " END LOOP;\n" +
                                            DALManager.BuildSqlStringParameterName("pRecordsAffected") + ":=recordsAffected;\n" +
                                         "END;";

                OracleCommand dbCommand = (OracleCommand)DALManager.Database.GetSqlStringCommand(anonymous_block);

                dbCommand.Parameters.Add(new OracleParameter("pHitsNum", OracleDbType.Int32, hitsToBeMarked.Length, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pMarkedHitListID", OracleDbType.Int32, markedHitListID, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pHitsToBeMarked", OracleDbType.Int32, hitsToBeMarked, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pRecordsAffected", OracleDbType.Int32, 0, ParameterDirection.Output));
                dbCommand.Parameters["pHitsToBeMarked"].CollectionType = OracleCollectionType.PLSQLAssociativeArray;

                DALManager.ExecuteNonQuery(dbCommand);
                recordsAffected = int.Parse(dbCommand.Parameters["pRecordsAffected"].Value.ToString());

                //Update savedhitlistid table
                string sql = "UPDATE " + _savedHitListIDTableName +
                    " SET NUMBER_HITS = (SELECT COUNT(*) FROM " + _savedHitListTableName + " WHERE HITLISTID=" + DALManager.BuildSqlStringParameterName("pMarkedHitListID") + ")" +
                    " WHERE ID = " + DALManager.BuildSqlStringParameterName("pMarkedHitListID");

                dbCommand = (OracleCommand)DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pMarkedHitListID"), DbType.Int32, markedHitListID);
                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pMarkedHitListID"), DbType.Int32, markedHitListID);

                DALManager.ExecuteNonQuery(dbCommand);

                if (recordsAffected < 0)
                    recordsAffected = 0;

                return markedHitListID;

            }
            catch (Exception e)
            {
                throw;
            }
        }

        public override int MarkAllHits(int hitListId, int markedHitListId, HitListType hitListType, int dataviewId, int markedHitsMax)
        {
            string tableName = string.Empty;
            string insertString = string.Empty;
            string pkFieldName = string.Empty;
            string maxMarkedSizeStr = (markedHitsMax != int.MaxValue) ? " WHERE ROWNUM < " + (++markedHitsMax).ToString() : string.Empty;

            switch (hitListType)
            {
                case HitListType.SAVED:
                    tableName = _savedHitListTableName;
                    break;
                case HitListType.TEMP:
                    tableName = _tempHitListTableName;
                    break;
            }

            if(hitListId > 0)
                insertString = "INSERT INTO " + _savedHitListTableName + " (HITLISTID, ID, DATESTAMP, SORTORDER) " +
                                         "(SELECT * FROM (SELECT " + DALManager.BuildSqlStringParameterName("pMarkedHitListId") + ",ID, actualDate,maxSortOrder+ROWNUM  FROM " + tableName + " WHERE HITLISTID = " + DALManager.BuildSqlStringParameterName("pHitListId") +
                                         " AND " + DALManager.BuildSqlStringParameterName("pMarkedHitListId") + " || ID NOT IN (SELECT " + DALManager.BuildSqlStringParameterName("pMarkedHitListId") + " || ID FROM " + _savedHitListTableName + " WHERE HITLISTID = " + DALManager.BuildSqlStringParameterName("pMarkedHitListId") + "))" + maxMarkedSizeStr + ");" + "\n";
            else
            {
                COEDataViewBO dv = COEDataViewBO.Get(dataviewId);
                // Coverity Fix CID - 10819 (from local server)
                COEDataView.DataViewTable theDataViewTable = dv.COEDataView.Tables[dv.BaseTable];
                if (theDataViewTable != null)
                {
                    foreach (COEDataView.Field field in theDataViewTable.Fields)
                    {
                        if (field.Id.ToString() == theDataViewTable.PrimaryKey)
                        {
                            pkFieldName = field.Name.ToUpper();
                            break;
                        }
                    }
                }
                insertString = "INSERT INTO " + _savedHitListTableName + " (HITLISTID, ID, DATESTAMP, SORTORDER) " +
                                         "(SELECT * FROM (SELECT " + DALManager.BuildSqlStringParameterName("pMarkedHitListId") + ",bt.\"" + pkFieldName + "\", actualDate,maxSortOrder+ROWNUM  FROM \"" + dv.COEDataView.Database.ToUpper() + "\".\"" + dv.BaseTable.ToUpper() + "\" bt" +
                                         " WHERE  " + DALManager.BuildSqlStringParameterName("pMarkedHitListId") + " || bt.\"" + pkFieldName + "\" NOT IN (SELECT " + DALManager.BuildSqlStringParameterName("pMarkedHitListId") + " || ID FROM " + _savedHitListTableName + " WHERE HITLISTID = " + DALManager.BuildSqlStringParameterName("pMarkedHitListId") + "))" + maxMarkedSizeStr + ");" + "\n";
            }

            try
            {
                string anonymous_block = "DECLARE " + "\n" +

                                         "actualDate date;" + "\n" +
                                         "maxSortOrder number;" + "\n" +

                                         "BEGIN" + "\n" +

                                         "actualDate := SYSDATE;" + "\n" +
                                         "SELECT MAX(SORTORDER) INTO maxSortOrder FROM " + _savedHitListTableName + " WHERE HITLISTID = " + DALManager.BuildSqlStringParameterName("pMarkedHitListId") + ";" + "\n" +

                                         "IF maxSortOrder = NULL THEN" + "\n" +
                                         "maxSortOrder := 0;" + "\n" +    
                                         "END IF;" + "\n" +
                                         insertString +
                                         DALManager.BuildSqlStringParameterName("pRecordsAffected") + " :=SQL%ROWCOUNT;" + "\n" +

                                         "UPDATE " + _savedHitListIDTableName +
                                         " SET NUMBER_HITS = (SELECT COUNT(*) FROM " + _savedHitListTableName + " WHERE HITLISTID = " + DALManager.BuildSqlStringParameterName("pMarkedHitListID") + " AND DATAVIEW_ID = " + DALManager.BuildSqlStringParameterName("pDataviewId") + ") " +
                                         "WHERE HITLISTID = " + DALManager.BuildSqlStringParameterName("pMarkedHitListId") +
                                         " AND DATAVIEW_ID = " + DALManager.BuildSqlStringParameterName("pDataviewId") + "; " + "\n" +

                                         "END;";

                OracleCommand dbCommand = (OracleCommand)DALManager.Database.GetSqlStringCommand(anonymous_block);

                dbCommand.Parameters.Add(new OracleParameter("pMarkedHitListId", OracleDbType.Int32, markedHitListId, ParameterDirection.Input));
                if(hitListId > 0)
                    dbCommand.Parameters.Add(new OracleParameter("pHitListId", OracleDbType.Int32, hitListId, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pRecordsAffected", OracleDbType.Int32, 0, ParameterDirection.Output));
                dbCommand.Parameters.Add(new OracleParameter("pDataviewId", OracleDbType.Int32, dataviewId, ParameterDirection.Input));
                

                DALManager.ExecuteNonQuery(dbCommand);

                return int.Parse(dbCommand.Parameters["pRecordsAffected"].Value.ToString());

            }
            catch (Exception e)
            {
                throw;
            }
        }

        public override int RemoveFromMarkedHitList(int markedHitListID, int[] hitsToBeUnmarked, out int recordsAffected)
        {
            try
            {

                //Delete from savedhitlistid
                string anonymous_block = "DECLARE " + "\n" +
                                         "TYPE hitsArray IS TABLE OF NUMBER INDEX BY BINARY_INTEGER;" + "\n" +
                                         "recordsAffected NUMBER(9) := 0;\n" +
                                         "numHitsParam NUMBER(9) := " + DALManager.BuildSqlStringParameterName("pHitsNum") + ";" + "\n" +
                                         "markedHitListIDParam NUMBER(9) := " + DALManager.BuildSqlStringParameterName("pMarkedHitListID") + ";" + "\n" +
                                         "hitsToBeUnMarkedParam hitsArray := " + DALManager.BuildSqlStringParameterName("pHitsToBeUnMarked") + ";" + "\n" +
                                         "BEGIN " + "\n" +
                                            "FOR I IN 1 .. numHitsParam LOOP " + "\n" +
                                                 "DELETE FROM " + _savedHitListTableName + " WHERE HITLISTID=markedHitListIDParam\n" +
                                                 " AND ID=hitsToBeUnMarkedParam(I);" + "\n" +
                                                 "recordsAffected := recordsAffected + SQL%ROWCOUNT;\n" +
                                            " END LOOP; " + "\n" +
                                            DALManager.BuildSqlStringParameterName("pRecordsAffected") + ":=recordsAffected;\n" +
                                         "END;";

                OracleCommand dbCommand = (OracleCommand)DALManager.Database.GetSqlStringCommand(anonymous_block);

                dbCommand.Parameters.Add(new OracleParameter("pHitsNum", OracleDbType.Int32, hitsToBeUnmarked.Length, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pMarkedHitListID", OracleDbType.Int32, markedHitListID, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pHitsToBeUnMarked", OracleDbType.Int32, hitsToBeUnmarked, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pRecordsAffected", OracleDbType.Int32, 0, ParameterDirection.Output));
                dbCommand.Parameters["pHitsToBeUnMarked"].CollectionType = OracleCollectionType.PLSQLAssociativeArray;

                DALManager.ExecuteNonQuery(dbCommand);
                recordsAffected = int.Parse(dbCommand.Parameters["pRecordsAffected"].Value.ToString());

                //Update savedhitlistid table
                string sql = "UPDATE " + _savedHitListIDTableName +
                    " SET NUMBER_HITS = (SELECT COUNT(*) FROM " + _savedHitListTableName + " WHERE HITLISTID=" + DALManager.BuildSqlStringParameterName("pMarkedHitListID") + ")" +
                    " WHERE ID = " + DALManager.BuildSqlStringParameterName("pMarkedHitListID");

                dbCommand = (OracleCommand)DALManager.Database.GetSqlStringCommand(sql);

                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pMarkedHitListID"), DbType.Int32, markedHitListID);
                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pMarkedHitListID"), DbType.Int32, markedHitListID);

                DALManager.ExecuteNonQuery(dbCommand);

                if (recordsAffected < 0)
                    recordsAffected = 0;

                return markedHitListID;

            }
            catch (Exception e)
            {
                throw;
            }
        }

        public override int GetNewTempHitlistID()
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + "  ";

            string sql = "SELECT " + Resources.CentralizedStorageDB + "." + Resources.COEHitListIDSequence + ".NEXTVAL as HITLISTID FROM DUAL";

            _coeLog.LogStart(methodSignature + sql);

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            int hitListID = Convert.ToInt32(DALManager.Database.ExecuteScalar(dbCommand));
            _coeLog.LogEnd(methodSignature);
            return hitListID;
        }

        public override int GetSavedHitListID()
        {
            string sql = "SELECT " + Resources.CentralizedStorageDB + "." + Resources.COEHitListIDSequence + ".NEXTVAL as HITLISTID FROM DUAL";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            int hitListID = Convert.ToInt32(DALManager.Database.ExecuteScalar(dbCommand));
            return hitListID;
        }

        public void UpdateCorrelatedHits(int[] hitsArray1, double[] hitsArray2, int hitListID, HitListType hitListType)
        {
            try
            {
                DbCommand dbCommand = DALManager.Database.GetStoredProcCommand(Resources.CentralizedStorageDB + ".COEDBLIBRARY.UPDATEARRAY");
                OracleParameter Param1 = new OracleParameter("AHitListID", OracleDbType.Int32);
                OracleParameter Param2 = new OracleParameter("AIdArray", OracleDbType.Int32);
                OracleParameter Param3 = new OracleParameter("ASortArray", OracleDbType.Double);

                Param1.Value = hitListID;

                Param2.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                Param2.Value = hitsArray1;
                Param2.Size = hitsArray1.Length;

                Param3.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                Param3.Value = hitsArray2;
                Param3.Size = hitsArray2.Length;

                dbCommand.Parameters.Add(Param1);
                dbCommand.Parameters.Add(Param2);
                dbCommand.Parameters.Add(Param3);
                dbCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public override void UpdateHits(int[] hitsArray1, double[] hitsArray2, int hitListID, HitListType hitListType)
        {
            string hitListTableName = string.Empty;
            DbCommand dbCommand;

            _coeLog.LogStart(MethodBase.GetCurrentMethod().Name.ToUpper() + " - OracleDataAccessClientDAL");

            try
            {
                if ((hitsArray2.Length > 0) && (hitsArray2[0] != -1))
                {
                    UpdateCorrelatedHits(hitsArray1, hitsArray2, hitListID, hitListType);
                }
                else
                {
                    if (hitListType == HitListType.TEMP)
                    {
                        hitListTableName = _tempHitListTableName;
                    }
                    else
                    {
                        hitListTableName = _savedHitListTableName;
                    }

                    //update  hitListTableName with hits without addin duplicates

                    for (int i = 0; i <= hitsArray1.Length - 1; i++)
                    {
                        //first see if the hit already exits:
                        string sql = "SELECT COUNT(*) FROM " + hitListTableName + " WHERE HITLISTID = " + DALManager.BuildSqlStringParameterName("pHitListId") + " AND ID = " + DALManager.BuildSqlStringParameterName("pHitsArray1");
                        dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                        DALManager.Database.AddInParameter(dbCommand, "pHitListId", DbType.Int32, hitListID);
                        DALManager.Database.AddInParameter(dbCommand, "pHitsArray1", DbType.Int32, hitsArray1[i]);
                        int count = System.Convert.ToInt32(DALManager.ExecuteScalar(dbCommand));


                        if (count == 0)
                        {
                            sql = "INSERT INTO " + hitListTableName + "(HITLISTID, ID) " +
                            " VALUES ( " + DALManager.BuildSqlStringParameterName("pHitListId") + " , " + DALManager.BuildSqlStringParameterName("pHitsArray1") + " )";
                            dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                            DALManager.Database.AddInParameter(dbCommand, "pHitListId", DbType.Int32, hitListID);
                            DALManager.Database.AddInParameter(dbCommand, "pHitsArray1", DbType.Int32, hitsArray1[i]);
                            int recordsAffected = DALManager.ExecuteNonQuery(dbCommand);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                _coeLog.Log(ex.Message, 1, System.Diagnostics.SourceLevels.Error);
                throw;
            }
            finally
            {
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name.ToUpper() + " - OracleDataAccessClientDAL");
            }

        }

        public override void InsertHits(int[] hitsArray1, double[] hitsArray2, int hitListID, HitListType hitListType)
        {

            string hitListTableName;
            if (hitListType == HitListType.TEMP)
                hitListTableName = _tempHitListTableName;
            else
                hitListTableName = _savedHitListTableName;

            try
            {
                _coeLog.LogStart(MethodBase.GetCurrentMethod().Name.ToUpper() + " - OracleDataAccessClientDAL");

                //Insert in child table  

                string sql = "BEGIN " +
                                            "FOR I IN 1 .. " + DALManager.BuildSqlStringParameterName("pHitsNum") + " LOOP " +
                                                 "INSERT INTO " + hitListTableName + " (HITLISTID,ID,DATESTAMP,SORTORDER) VALUES " +
                                                   "(" + DALManager.BuildSqlStringParameterName("pHitListID") + "," + DALManager.BuildSqlStringParameterName("pIDs") + "(I),SYSDATE,I);" +
                                            " END LOOP; " +
                                         "END;";

                OracleCommand dbCommand = (OracleCommand)DALManager.Database.GetSqlStringCommand(sql);

                dbCommand.Parameters.Add(new OracleParameter("pHitsNum", OracleDbType.Int32, hitsArray1.Length, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pHitListID", OracleDbType.Int32, hitListID, ParameterDirection.Input));
                dbCommand.Parameters.Add(new OracleParameter("pIDs", OracleDbType.Int32, hitsArray1, ParameterDirection.Input));
                dbCommand.Parameters["pIDs"].CollectionType = OracleCollectionType.PLSQLAssociativeArray;

                int recordsAffected = DALManager.ExecuteNonQuery(dbCommand);

                if ((hitsArray2.Length > 0) && (hitsArray2[0] != -1))
                {
                    UpdateCorrelatedHits(hitsArray1, hitsArray2, hitListID, hitListType);
                }
            }
            catch (Exception ex)
            {
                _coeLog.Log(ex.Message, 1, System.Diagnostics.SourceLevels.Error);
                throw;
            }
            finally
            {
                _coeLog.LogEnd(MethodBase.GetCurrentMethod().Name.ToUpper() + " - OracleDataAccessClientDAL");
            }
        }

        public override int AddToRecent(int id, int hitListID, HitListType type, string name, string description, string userID, bool isPublic, int numHits, string databaseName, int dataviewID, int parentHitListID, string originalType, int searchCriteriaId, string searchCriteriaType)
        {
            id = GetNewTempHitlistID();
            string sql = "declare" + "\n" +
                    "lastRecent number(9);" + "\n" +
                    "hitListIDParam number(9) := " + DALManager.BuildSqlStringParameterName("pHitListID") + ";" + "\n" +
                    "userIDParam varchar2(30) := " + DALManager.BuildSqlStringParameterName("pUserID") + ";" + "\n" +
                    "dataviewIDParam number(9) := " + DALManager.BuildSqlStringParameterName("pDataViewID") + ";" + "\n" +
                    "iDParam number(9) := " + DALManager.BuildSqlStringParameterName("pID") + ";" + "\n" +
                    "hitlistNameParam varchar2(255) := " + DALManager.BuildSqlStringParameterName("pHitListName") + ";" + "\n" +
                    "descriptionParam varchar2(255) := " + DALManager.BuildSqlStringParameterName("pDescription") + ";" + "\n" +
                    "numberHitsParam number(9) := " + DALManager.BuildSqlStringParameterName("pNumberHits") + ";" + "\n" +
                    "isPublicParam varchar2(1) := " + DALManager.BuildSqlStringParameterName("pIsPublic") + ";" + "\n" +
                    "databaseParam varchar2(30) := " + DALManager.BuildSqlStringParameterName("pDatabase") + ";" + "\n" +
                    "parentHitListIDParam number(9) := " + DALManager.BuildSqlStringParameterName("pParentHitListID") + ";" + "\n" +
                    "typeParam varchar2(10) := " + DALManager.BuildSqlStringParameterName("pType") + ";" + "\n" +
                    "searchCriteriaIdParam number(9) := " + DALManager.BuildSqlStringParameterName("pSearchCriteriaId") + ";" + "\n" +
                    "searchCriteriaTypeParam varchar2(10) := " + DALManager.BuildSqlStringParameterName("pSearchCriteriaType") + ";" + "\n" +
                "begin" + "\n" +
                    "select nvl(max(hitlistid),-1) into lastRecent from COEDB.COETEMPHITLISTID where user_id = userIDParam and dataview_id = dataviewIDParam;" + "\n" +
                    "if lastRecent <> hitListIDParam then" + "\n" +
                        "INSERT INTO " + _tempHitListIDTableName + "\n" +
     "(ID,HITLISTID,TYPE,NAME,DESCRIPTION,USER_ID,NUMBER_HITS,IS_PUBLIC,DATE_CREATED,DATABASE,DATAVIEW_ID,PARENT_HITLIST_ID,SEARCH_CRITERIA_ID,SEARCH_CRITERIA_TYPE) VALUES" + "\n" +
     "(iDParam, hitListIDParam, typeParam, hitlistNameParam, descriptionParam, userIDParam, numberHitsParam, isPublicParam, SYSDATE, databaseParam, dataviewIDParam, parentHitListIDParam, searchCriteriaIdParam, searchCriteriaTypeParam);" + "\n" +
                    "end if;" + "\n" +
                "end;";
            

            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);

            dbCommand.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("pHitListID"), OracleDbType.Int32, hitListID, ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("pUserID"), OracleDbType.Varchar2, userID.ToUpper(), ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("pDataViewID"), OracleDbType.Int32, dataviewID, ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("pID"), OracleDbType.Int32, id, ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("pHitListName"), OracleDbType.Varchar2, name, ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("pDescription"), OracleDbType.Varchar2, description, ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("pNumberHits"), OracleDbType.Int32, numHits, ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("pIsPublic"), OracleDbType.Varchar2, isPublic ? "1" : "0", ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("pDatabase"), OracleDbType.Varchar2, databaseName, ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("pParentHitListID"), OracleDbType.Int32, parentHitListID, ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("pType"), OracleDbType.Varchar2, originalType, ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("pSearchCriteriaId"), OracleDbType.Int32, searchCriteriaId, ParameterDirection.Input));
            dbCommand.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("pSearchCriteriaType"), OracleDbType.Varchar2, searchCriteriaType, ParameterDirection.Input));

            #region Debugging Information (Not consuming resources in RELEASE MODE compilations)
            System.Diagnostics.Debugger.Log(2, "Information", "\nAdd to recent SQL:\n\t");
            System.Diagnostics.Debugger.Log(2, "Information", sql + "\n");
            System.Diagnostics.Debugger.Log(2, "Information", "\n\t" + DALManager.BuildSqlStringParameterName("pHitListID") + ": " + hitListID);
            System.Diagnostics.Debugger.Log(2, "Information", "\n\t" + DALManager.BuildSqlStringParameterName("pUserID") + ": " + userID.ToUpper());
            System.Diagnostics.Debugger.Log(2, "Information", "\n\t" + DALManager.BuildSqlStringParameterName("pDataViewID") + ": " + dataviewID);
            System.Diagnostics.Debugger.Log(2, "Information", "\n\t" + DALManager.BuildSqlStringParameterName("pID") + ": " + id);
            System.Diagnostics.Debugger.Log(2, "Information", "\n\t" + DALManager.BuildSqlStringParameterName("pHitListName") + ": " + name);
            System.Diagnostics.Debugger.Log(2, "Information", "\n\t" + DALManager.BuildSqlStringParameterName("pDescription") + ": " + description);
            System.Diagnostics.Debugger.Log(2, "Information", "\n\t" + DALManager.BuildSqlStringParameterName("pNumberHits") + ": " + numHits);
            System.Diagnostics.Debugger.Log(2, "Information", "\n\t" + DALManager.BuildSqlStringParameterName("pIsPublic") + ": " + isPublic);
            System.Diagnostics.Debugger.Log(2, "Information", "\n\t" + DALManager.BuildSqlStringParameterName("pDatabase") + ": " + databaseName);
            System.Diagnostics.Debugger.Log(2, "Information", "\n\t" + DALManager.BuildSqlStringParameterName("pParentHitListID") + ": " + parentHitListID);
            System.Diagnostics.Debugger.Log(2, "Information", "\n\t" + DALManager.BuildSqlStringParameterName("pType") + ": " + originalType);
            System.Diagnostics.Debugger.Log(2, "Information", "\n\t" + DALManager.BuildSqlStringParameterName("pSearchCriteriaId") + ": " + searchCriteriaId);
            System.Diagnostics.Debugger.Log(2, "Information", "\n\t" + DALManager.BuildSqlStringParameterName("pSearchCriteriaType") + ": " + searchCriteriaType + "\n");
            #endregion

            int recordsAffected = DALManager.Database.ExecuteNonQuery(dbCommand);

            return id;
        }

        public override SafeDataReader GetRecentHitLists(string userID, int dataviewID, string databaseName, int quantityToRetrieve)
        {
            try
            {
                string sql = null;
                DbCommand dbCommand;
                SafeDataReader returnSafeDataReader = null;
                sql = "SELECT * FROM (SELECT ID,HITLISTID,NAME, DESCRIPTION, IS_PUBLIC, NUMBER_HITS, USER_ID, DATE_CREATED,DATABASE,DATAVIEW_ID,PARENT_HITLIST_ID,TYPE,SEARCH_CRITERIA_ID,SEARCH_CRITERIA_TYPE" +
                " FROM " + _tempHitListIDTableName +
                " WHERE (USER_ID=" + DALManager.BuildSqlStringParameterName("pUserID") +
                " OR IS_PUBLIC=" + DALManager.BuildSqlStringParameterName("pIsPublic") + ")" +
                " AND DATABASE=" + DALManager.BuildSqlStringParameterName("pDatabase") +
                " AND DATAVIEW_ID=" + DALManager.BuildSqlStringParameterName("pDataViewID") +
                " ORDER BY ID DESC) WHERE ROWNUM<" + DALManager.BuildSqlStringParameterName("pQuantityToRetrieve");
                System.Diagnostics.Debugger.Log(2, "Information", sql);
                System.Diagnostics.Debugger.Log(2, "Information", "UserID: " + userID + ", IsPublic: " + 1 + ", Database: " + databaseName + ", DataviewID: " + dataviewID + ", QuantityToRetrieve: " + quantityToRetrieve + "\n");
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pUserID"), DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userID.ToUpper());
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pIsPublic"), DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, "1");
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pDatabase"), DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pDataViewID"), DbType.Int32, dataviewID);
                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pQuantityToRetrieve"), DbType.Int32, ++quantityToRetrieve);

                return new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override SafeDataReader GetRecentHitListsConfig(string userID, int dataviewID, string databaseName, int quantityToRetrieve, DateTime dt)
        {
            try
            {
                string sql = null;
                DbCommand dbCommand;
                SafeDataReader returnSafeDataReader = null;
                sql = "SELECT * FROM (SELECT ID,HITLISTID,NAME, DESCRIPTION, IS_PUBLIC, NUMBER_HITS, USER_ID, DATE_CREATED,DATABASE,DATAVIEW_ID,PARENT_HITLIST_ID,TYPE,SEARCH_CRITERIA_ID,SEARCH_CRITERIA_TYPE" +
                " FROM " + _tempHitListIDTableName +
                " WHERE (USER_ID=" + DALManager.BuildSqlStringParameterName("pUserID") +
                " OR IS_PUBLIC=" + DALManager.BuildSqlStringParameterName("pIsPublic") + ")" +
                " AND DATABASE=" + DALManager.BuildSqlStringParameterName("pDatabase") +
                " AND DATAVIEW_ID=" + DALManager.BuildSqlStringParameterName("pDataViewID") +
                " AND DATE_CREATED>=" + DALManager.BuildSqlStringParameterName("pDt") +
                " ORDER BY ID DESC) WHERE ROWNUM<" + DALManager.BuildSqlStringParameterName("pQuantityToRetrieve");
                System.Diagnostics.Debugger.Log(2, "Information", sql);
                System.Diagnostics.Debugger.Log(2, "Information", "UserID: " + userID + ", IsPublic: " + 1 + ", Database: " + databaseName + ", DataviewID: " + dataviewID + ", QuantityToRetrieve: " + quantityToRetrieve + "\n");
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pUserID"), DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userID.ToUpper());
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pIsPublic"), DbType.AnsiString, 1, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, "1");
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pDatabase"), DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pDataViewID"), DbType.Int32, dataviewID);
                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pDt"), DbType.DateTime, dt);
                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pQuantityToRetrieve"), DbType.Int32, ++quantityToRetrieve);

                return new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal override SafeDataReader GetMarkedHitList(string databaseName, string userName, int dataViewID)
        {
            try
            {
                DbCommand dbCommand;
                SafeDataReader returnSafeDataReader = null;

                string sql = "DECLARE" + "\n" +
                    "IDPARAM " + Resources.SecurityDatabaseName + ".COESAVEDHITLISTID.ID%TYPE;" + "\n" +
                    "DATABASEPARAM " + Resources.SecurityDatabaseName + ".COESAVEDHITLISTID.DATABASE%TYPE := " + DALManager.BuildSqlStringParameterName("pDatabase") + ";" + "\n" +
                    "USERNAMEPARAM " + Resources.SecurityDatabaseName + ".COESAVEDHITLISTID.USER_ID%TYPE := " + DALManager.BuildSqlStringParameterName("pUserID") + ";" + "\n" +
                    "DATAVIEWIDPARAM " + Resources.SecurityDatabaseName + ".COESAVEDHITLISTID.DATAVIEW_ID%TYPE := " + DALManager.BuildSqlStringParameterName("pDataViewID") + ";" + "\n" +
                    "TYPEPARAM " + Resources.SecurityDatabaseName + ".COESAVEDHITLISTID.TYPE%TYPE := 'MARKED';" + "\n" +
                "BEGIN" + "\n" +
                    "SELECT COUNT(*) INTO IDPARAM FROM " + Resources.SecurityDatabaseName + ".COESAVEDHITLISTID WHERE DATABASE=DATABASEPARAM AND DATAVIEW_ID=DATAVIEWIDPARAM AND USER_ID=USERNAMEPARAM AND TYPE='MARKED';" + "\n" +
                    "IF IDPARAM = 0 THEN" + "\n" +
                        "SELECT " + Resources.CentralizedStorageDB + "." + Resources.COEHitListIDSequence + ".NEXTVAL INTO IDPARAM FROM DUAL;" + "\n" +
                        "INSERT INTO " + _savedHitListIDTableName + "\n" +
                        " (ID,NAME,DESCRIPTION,USER_ID,NUMBER_HITS,IS_PUBLIC,DATE_CREATED,DATABASE,DATAVIEW_ID,TYPE,HITLISTID) VALUES " + "\n" +
                        "(IDPARAM" + "\n" +
                        ",'USERMARKED'" + "\n" +
                        ",'USERMARKED'" + "\n" +
                        ",USERNAMEPARAM" + "\n" +
                        ",0" + "\n" +
                        ",'0'" + "\n" +
                        ",SYSDATE" + "\n" +
                        ",DATABASEPARAM" + "\n" +
                        ",DATAVIEWIDPARAM" + "\n" +
                        ",TYPEPARAM" + "\n" +
                        ",IDPARAM);" + "\n" +
                    "ELSE" + "\n" +
                        "SELECT ID INTO IDPARAM FROM " + Resources.SecurityDatabaseName + ".COESAVEDHITLISTID WHERE DATABASE=DATABASEPARAM AND DATAVIEW_ID=DATAVIEWIDPARAM AND USER_ID=USERNAMEPARAM AND TYPE='MARKED';" + "\n" +
                    "END IF;" + "\n" +
                    DALManager.BuildSqlStringParameterName("pID") + ":=IDPARAM;" + "\n" +
                "END;";

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pDatabase"), DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, databaseName);
                DALManager.Database.AddParameter(dbCommand, DALManager.BuildSqlStringParameterName("pUserID"), DbType.AnsiString, 1000, ParameterDirection.Input, true, 0, 0, string.Empty, DataRowVersion.Current, userName.ToUpper());
                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pDataViewID"), DbType.Int32, dataViewID);
                DALManager.Database.AddOutParameter(dbCommand, DALManager.BuildSqlStringParameterName("pID"), DbType.Int32, 9);

                DALManager.Database.ExecuteNonQuery(dbCommand);

                int markedHitListID = Convert.ToInt32(DALManager.Database.GetParameterValue(dbCommand, DALManager.BuildSqlStringParameterName("pID")));

                sql = "SELECT ID,HITLISTID,NAME, DESCRIPTION, IS_PUBLIC, NUMBER_HITS, USER_ID, DATE_CREATED,DATABASE,DATAVIEW_ID,PARENT_HITLIST_ID,TYPE,SEARCH_CRITERIA_ID,SEARCH_CRITERIA_TYPE" +
                " FROM " + _savedHitListIDTableName +
                " WHERE ID=" + DALManager.BuildSqlStringParameterName("pID");

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pID"), DbType.Int32, markedHitListID);

                return new SafeDataReader(DALManager.Database.ExecuteReader(dbCommand));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal override int SynchronizeRecordCount(int hitlistID, HitListType hitlistType, string baseTable, string primaryKeyName)
        {
            try
            {
                string sql = null;
                string hitlistTableName = hitlistType == HitListType.TEMP ? _tempHitListTableName : _savedHitListTableName;
                string hitlistIDTableName = hitlistType == HitListType.TEMP ? _tempHitListIDTableName : _savedHitListIDTableName;
                DbCommand dbCommand;
                sql = "DECLARE\n" +
                      "  recordCount number(9);\n" +
                      "  paramHitlistID number(9);\n" +
                      "BEGIN\n" +
                      "  paramHitlistID := " + DALManager.BuildSqlStringParameterName("pHitListID") + ";\n" +
                      "  SELECT count(*) INTO recordCount FROM " +
                      hitlistTableName + " list, " + baseTable + " bt where bt." + primaryKeyName + "=list.id AND list.hitlistid=paramHitlistID;\n" +
                      "  UPDATE " + hitlistIDTableName + " SET NUMBER_HITS = recordCount WHERE HITLISTID = paramHitlistID;\n" +
                      "  " + DALManager.BuildSqlStringParameterName("pRecordCount") + " := recordCount;\n" +
                      "END;";

                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, DALManager.BuildSqlStringParameterName("pHitListID"), DbType.Int32, hitlistID);
                DALManager.Database.AddOutParameter(dbCommand, DALManager.BuildSqlStringParameterName("pRecordCount"), DbType.Int32, 9);
                DALManager.Database.ExecuteNonQuery(dbCommand);
                return (int)dbCommand.Parameters[1].Value;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }

}
