using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CambridgeSoft.COE.Patcher
{
   public static class ExecuteSql 
    {

        private static IDBCommunicator _oracleConfigurationRepository = null;

        public static IDBCommunicator OracleConfigurationRepository
        {
            get
            {
                return _oracleConfigurationRepository;
            }

            set
            {
                _oracleConfigurationRepository = value;
            }
        }

        public static DataSet ExecuteDataset(string sql)
        {
            return _oracleConfigurationRepository.ExecuteDataset(sql);
        }

        public static string ExecuteScaler(string sql)
        {
            return _oracleConfigurationRepository.ExecuteScaler(sql);
        }

        public static int ExecuteNonQuery(string sql)
        {
            return _oracleConfigurationRepository.ExecuteNonQuery(sql);
        }


    }
}
