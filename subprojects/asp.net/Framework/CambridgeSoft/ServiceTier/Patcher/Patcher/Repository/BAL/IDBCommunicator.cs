using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


namespace CambridgeSoft.COE.Patcher
{
  public   interface IDBCommunicator
    {

       DataSet ExecuteDataset(string sql);
       string ExecuteScaler(string sql);
       int ExecuteNonQuery(string sql);

    }
}
