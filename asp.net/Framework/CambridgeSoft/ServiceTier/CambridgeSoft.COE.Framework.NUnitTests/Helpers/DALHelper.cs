using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Data;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using System.Reflection;
using System.Data;
using System.Data.Common;
using Oracle.DataAccess.Client;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.NUnitTests.Helpers
{
    public static class DALHelper
    {
        #region Properties

        /// <summary>
        /// Get Database connection to execute Query
        /// </summary>
        private static Database TheDataBase
        {
            get { return new OracleDatabase(ConfigurationManager.ConnectionStrings["coedbConn"].ConnectionString); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Method to execute input Query and returning DataTable as output
        /// </summary>
        /// <param name="strQuery">SQL query to be executed</param>
        /// <returns>Returning datatable</returns>
        public static DataTable ExecuteQuery(string strQuery)
        {
            try
            {
                DataSet ds = TheDataBase.ExecuteDataSet(CommandType.Text, strQuery);
                if (ds != null && ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
                return null;
            }
            catch(Exception ex) 
            {
 
            }
            return null;
        }

        public static object ExecuteScalar(string strQuery)
        {
            try
            {
                object objResult = TheDataBase.ExecuteScalar(CommandType.Text, strQuery);
                if (objResult != null && objResult != DBNull.Value)
                {
                    return objResult;
                }
                return null;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        /// <summary>
        /// Method to execute input Query and returning DataTable as output
        /// </summary>
        /// <param name="strQuery">SQL query to be executed</param>
        /// <returns>Returning datatable</returns>
        public static DataTable ExecuteQueryForRegDB(string strQuery)
        {
            try
            {
                Database theRegDbDatabase = new OracleDatabase(ConfigurationManager.ConnectionStrings["regdbConn"].ConnectionString);
                DataSet ds = TheDataBase.ExecuteDataSet(CommandType.Text, strQuery);
                if (ds != null && ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
                return null;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        #endregion

    }
}
