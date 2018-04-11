using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Data.OleDb;

namespace CambridgeSoft.COE.Framework.COESearchCriteriaService
{
    public class SystemDataOledbClientDAL : CambridgeSoft.COE.Framework.COESearchCriteriaService.DAL
    {
       

        
        /// <summary>
        /// will get the new id by selecting max id from teh table DB_DATAVIEW_SEQ
        /// if the max id is 0 then will return the new id as 1 else will update the max id by 1 and wil
        /// return that id and also will update the DB_DATAVIEW_SEQ by the same value.
        /// </summary>
        /// <returns>the new id retrived</returns>
        public override int GetNewID()
        {
            string sql = null;
            DbCommand dbCommand;

            sql = "SELECT MAX(ID) FROM " + _coeSearchCriteriaTableName + "_SEQ";
            dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            int id = Convert.ToInt32(DALManager.Database.ExecuteScalar(dbCommand));
            if (id == 0)
            {
                sql = "UPDATE " + _coeSearchCriteriaTableName + "_SEQ SET ID=1";
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.ExecuteNonQuery(dbCommand);
                id = 1;
            }
            else
            {
                sql = "UPDATE " + _coeSearchCriteriaTableName + "_SEQ SET ID=" + id + "+1";
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.ExecuteNonQuery(dbCommand);
                id = id + 1;
            }
            return id;
        }

       
        
    }
}
