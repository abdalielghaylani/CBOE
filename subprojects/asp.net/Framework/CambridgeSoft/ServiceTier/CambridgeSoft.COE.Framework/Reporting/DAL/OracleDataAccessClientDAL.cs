using System;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Data.Common;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.COEReportingService
{
    public class OracleDataAccessClientDAL : DAL
    {
        #region Variables
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEReport");
        #endregion

        #region Methods
        /// <summary>
        /// return the next report id from datatbase.
        /// </summary>
        /// <param name="dataView"></param>
        /// <returns></returns>
        public override int GetNewID()
        {
            int id = -1;
            string sql = "SELECT " + _coeReportTableName + "_SEQ.NEXTVAL FROM DUAL";
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
        #endregion
    }
}
