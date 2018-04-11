using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using Csla.Security;
using Csla.Data;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;


namespace CambridgeSoft.COE.Framework.COEDataViewService.UnitTests
{
    class DataViewDAL 
    { 

       /// <summary>
        /// Get all dataviews that are not application specific
        /// </summary>
        /// <returns>a dvcount</returns>
        public  int GetExpectedDataViewsCount()
        {
            //string sql = "select count(*) from coedb.coedataview dv1 where dv1.is_public=1 ";

            string sql = @"select count(*) from coedb.coedataview where is_public=1 and application IS NULL OR (is_public=0 AND user_id='cssadmin')";
            
            return GetDataViews(sql);
        }

        /// <summary>
        /// Get all dataviews that are specific to user 
        /// </summary>
        /// <returns>a dvcount</returns>
        public int GetExpectedDataViewsCount(string username)
        {
            string sql = "select count(*) from coedb.coedataview dv1 where dv1.user_id='" + username + "'";
           return GetDataViews(sql);
        }

        ///// <summary>
        ///// retuens the dataviews count based on the sql passed
        ///// </summary>
        ///// <param name="sql"></param>
        ///// <returns></returns>
        //private int GetDataViews(string sql)
        //{

        //    OracleConnection conn = new OracleConnection(@"User Id=COEDB;Password=ORACLE;Pooling=true;Data Source=dtp281/ORCL");
        //    int dvCount = 0;
        //    try
        //    {
        //        using (OracleCommand dbCommand = conn.CreateCommand())
        //        {

        //            dbCommand.CommandType = CommandType.Text;
        //            dbCommand.CommandText = sql;
        //            conn.Open();
        //            dvCount =Convert.ToInt32("0"+ dbCommand.ExecuteScalar());
        //            //dvCount = dbCommand.ExecuteNonQuery();
        //            conn.Close();
        //            conn.Dispose();
        //        }
        //        return dvCount;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
     
        //}


        /// <summary>
        /// retuens the dataviews count based on the sql passed
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private int GetDataViews(string sql)
        {
            int dvCount = 0;
            try
            {
                DataTable theResult = DALHelper.ExecuteQuery(sql);
                if (theResult != null && theResult.Rows.Count > 0)
                {
                    dvCount = Convert.ToInt32("0" + theResult.Rows[0][0]);
                }
                return dvCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
