using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data.OleDb;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using System.Data;

namespace CambridgeSoft.COE.Framework.COEHitListService
{
    /// <summary>
    /// 
    /// </summary>
    public class SystemDataOledbClientDAL : CambridgeSoft.COE.Framework.COEHitListService.DAL
    {



        /// <summary>
        /// 
        /// </summary>
        /// <param name="tempHitListCreateRequestInfo"></param>
        /// <returns></returns>
        public override int GetNewTempHitlistID()
        {
            string sql;
            DbCommand dbCommand;
            sql = "SELECT ID FROM " + _tempHitListIDTableName + "_SEQ ";
            int hitlistId = -1;
            dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            hitlistId = Convert.ToInt32(DALManager.Database.ExecuteScalar(dbCommand));
            //please see will that be okay to do?
            //created a table csdohitlistid_seq, and from there took the next value of the id and entered as the next id.
            if (hitlistId == 0)
            {
                sql = "UPDATE " + _tempHitListIDTableName + "_SEQ SET ID=1";
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.ExecuteNonQuery(dbCommand);
                hitlistId = 1;
            }
            else
            {
                sql = "UPDATE " + _tempHitListIDTableName + "_SEQ SET ID=" + DALManager.BuildSqlStringParameterName("pHitlistId");
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.Database.AddInParameter(dbCommand, "pHitListId", DbType.Int32, ++hitlistId);
                DALManager.ExecuteNonQuery(dbCommand);
            }
            return hitlistId;
        }
    }
    
}
