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
using CambridgeSoft.COE.Framework.Caching;


namespace CambridgeSoft.COE.Framework.COEFormService
{
  
    public class OracleDataAccessClientDAL : CambridgeSoft.COE.Framework.COEFormService.DAL
    {
        [NonSerialized]
         static COELog _coeLog = COELog.GetSingleton("COEForm");
        /// <summary>
        /// to return the next id
        /// </summary>
        /// <param name="dataView"></param>
        /// <returns></returns>
        public override int GetNewID()
        {
            int id = -1;
            string sql = "SELECT " + _coeFormTableName + "_SEQ.NEXTVAL FROM DUAL";
            DbCommand dbCommand = DALManager.Database.GetSqlStringCommand(sql);
            try
            {
                 id = Convert.ToInt32(DALManager.ExecuteScalar(dbCommand));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return id;
        }

        /// <summary>
        /// Builds an oracle dependecy to be used with the caching mechanism. The dependency is built against a single record in database.
        /// </summary>
        /// <param name="id">Id used to filter the record in database</param>
        /// <returns>An OracleCacheDependency</returns>
        internal override COECacheDependency GetCacheDependency(int id)
        {
            OracleConnection conn = DALManager.Database.CreateConnection() as OracleConnection;
            OracleCommand cmd = new OracleCommand("select rowid from coedb.coeform forms where forms.id=" + DALManager.BuildSqlStringParameterName("id"), conn);
            cmd.Parameters.Add(new OracleParameter(DALManager.BuildSqlStringParameterName("id"), OracleDbType.Int32, id, ParameterDirection.Input));
            OracleCacheDependency dependency = new OracleCacheDependency(cmd);
            return dependency;
        }
    }
}
