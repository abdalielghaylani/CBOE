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
using System.Data.SqlClient;

namespace CambridgeSoft.COE.Framework.COEFormService
{
 
    public class SystemDataSqlClientDAL : CambridgeSoft.COE.Framework.COEFormService.DAL
    {
   

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataView"></param>
        /// <returns></returns>
        public override int GetNewID()
        {
            string sql = null;
            DbCommand dbCommand;

            sql = "SELECT MAX(ID) FROM " + _coeFormTableName + "_SEQ";
            dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            int id = Convert.ToInt32(DALManager.Database.ExecuteScalar(dbCommand));
            if (id == 0)
            {
                sql = "UPDATE " + _coeFormTableName + "_SEQ SET ID=1";
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.ExecuteNonQuery(dbCommand);
                id = 1;
            }
            else
            {
                sql = "UPDATE " + _coeFormTableName + "_SEQ SET ID= " + id + "+1 ";
                dbCommand = DALManager.Database.GetSqlStringCommand(sql);
                DALManager.ExecuteNonQuery(dbCommand);
                id = id + 1;
            }
            return id;
        }

       
    }
}
