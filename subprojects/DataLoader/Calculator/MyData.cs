using System;
using System.Data;

namespace Calculator
{
    class MyData
    {
        private DataSet oDataSet;
        private DataTable oDataTable;
        public DataViewManager MyDataViewManager()
        {
            return oDataSet.DefaultViewManager;
        } // MyDataViewManager()
        public MyData()
        {
            oDataSet = new DataSet("DataSet");
            oDataTable = new DataTable("TableName");
            oDataSet.Tables.Add(oDataTable);
            oDataTable.Columns.Add("Compound Name", Type.GetType("System.String"));
            oDataTable.Columns.Add("Bonds", Type.GetType("System.Int32"));
            oDataTable.Columns.Add("Weight", Type.GetType("System.Double"));
            oDataTable.Columns.Add("Trouble", Type.GetType("System.Double"));
            DataRow oDataRow = oDataTable.NewRow();
            oDataRow["Compound Name"] = "Benzene";
            oDataRow["Bonds"] = 6;
            oDataRow["Weight"] = 123.456;
            oDataTable.Rows.Add(oDataRow);
            return;
        }
    } // class MyData
}
