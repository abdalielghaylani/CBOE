using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;

namespace DataPreviewer
{
    public class Utils
    {
        /// <summary>
        /// Generates a System.DataTable from a list of ISourceRecord instances.
        /// </summary>
        /// <param name="records">list of ISourceRecord instances</param>
        /// <param name="typeDefinitions">dictionary of data-types for column creation</param>
        /// <returns>a DataTable representative of both the data and data-types from the record-list</returns>
        public static DataTable GenerateTable(
            List<ISourceRecord> records
            , Dictionary<string, Type> typeDefinitions
            )
        {
            string indexColumnName = "__Index";
            DataTable dt = new DataTable("Records");

            //generate columns, eliminating those with no data in them
            DataColumn indexColumn = new DataColumn(indexColumnName, typeof(int));
            dt.Columns.Add(indexColumn);

            foreach (KeyValuePair<string, Type> kvp in typeDefinitions)
            {
                if (kvp.Value != null)
                {
                    DataColumn dc = new DataColumn(kvp.Key, kvp.Value);
                    dt.Columns.Add(dc);
                }
            }

            //generate rows
            foreach (ISourceRecord rec in records)
            {
                DataRow dr = dt.NewRow();
                dr[indexColumnName] = rec.SourceIndex;
                foreach (KeyValuePair<string, object> kvp in rec.FieldSet)
                {
                    if (dt.Columns.Contains(kvp.Key))
                        if (kvp.Value == null)
                            dr[kvp.Key] = DBNull.Value;
                        else
                        {
                            if (typeDefinitions[kvp.Key] == typeof(int))
                                dr[kvp.Key] = Convert.ToInt32(kvp.Value);

                            else if (typeDefinitions[kvp.Key] == typeof(double))
                                dr[kvp.Key] = Convert.ToDouble(kvp.Value);

                            else if (typeDefinitions[kvp.Key] == typeof(DateTime))
                                dr[kvp.Key] = Convert.ToDateTime(kvp.Value);

                            else
                                dr[kvp.Key] = kvp.Value;

                        }
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

    }
}
