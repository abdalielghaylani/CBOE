using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using System.Data;

namespace CambridgeSoft.NCDS_DataLoader.Common
{
    public class Utils
    {
        /// <summary>
        /// Generates a System.DataTable from a list of ISourceRecord instances.
        /// </summary>
        /// <param name="records">list of ISourceRecord instances</param>
        /// <param name="typeDefinitions">dictionary of data-types for column creation</param>
        /// <returns>a DataTable representative of both the data and data-types from the record-list</returns>
        public DataTable GenerateTable(
            List<ISourceRecord> records
            , Dictionary<string, Type> typeDefinitions
            , bool isDisplay
            )
        {
            DataTable dt = new DataTable("Records");

            string indexColumnName = "No";
            DataColumn indexColumn = new DataColumn(indexColumnName, typeof(int));
            dt.Columns.Add(indexColumn);

            if (isDisplay == true)
            {
                string checkColumnName = "Check";
                string regNumColumnName = "RegNum";

                //generate columns, eliminating those with no data in them
                DataColumn checkColumn = new DataColumn(checkColumnName, typeof(bool));
                DataColumn regNumColumn = new DataColumn(regNumColumnName, typeof(string));
                dt.Columns.Add(checkColumn);
                dt.Columns.Add(regNumColumn);
            }
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

                if(!IsBlankLine(rec))
                {
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
          }
          return dt;
        }

        public DataTable GenerateDisplayTable(
            Dictionary<ISourceRecord, string> records
            , Dictionary<string, Type> typeDefinitions
            )
        {
            DataTable dt = new DataTable("Records");

            string indexColumnName = "index";
            string noColumnName = "No";
            DataColumn indexColumn = new DataColumn(indexColumnName, typeof(int));
            DataColumn noColumn = new DataColumn(noColumnName, typeof(int));
            dt.Columns.Add(noColumn);

            string checkColumnName = "Check";
            string regNumColumnName = "RegNum";

            //generate columns, eliminating those with no data in them
            DataColumn checkColumn = new DataColumn(checkColumnName, typeof(bool));
            DataColumn regNumColumn = new DataColumn(regNumColumnName, typeof(string));
            dt.Columns.Add(checkColumn);
            dt.Columns.Add(regNumColumn);

            DataColumn dc;
            foreach (KeyValuePair<string, Type> kvp in typeDefinitions)
            {
                if (kvp.Value != null)
                {
                    if (kvp.Value == typeof(int))                   
                   {
                       dc = new DataColumn(kvp.Key, typeof(double));
                    }
                    else{
                    dc = new DataColumn(kvp.Key, kvp.Value);}
                    dt.Columns.Add(dc);
                }
            }
            dt.Columns.Add(indexColumn);
            //generate rows
            int number = 1;
            int MaxValue = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxDisplayValue"]);
            DataRow dr;
            ISourceRecord rec;
            foreach (KeyValuePair<ISourceRecord, string> record in records)
            {
                if (number > MaxValue)
                {
                    break;
                }

                dr = dt.NewRow();

                rec = record.Key;
                if (!IsBlankLine(rec))
                {
                    dr[regNumColumnName] = record.Value;
                    if (!string.IsNullOrEmpty(record.Value.Trim()))
                    {
                        dr[checkColumnName] = true;
                    }
                    else
                    {
                        dr[checkColumnName] = false;
                    }
                    //dr[indexColumnName] = rec.SourceIndex;
                    dr[indexColumnName] = number;
                    dr[noColumnName] = number;
                    foreach (KeyValuePair<string, object> kvp in rec.FieldSet)
                    {
                        if (dt.Columns.Contains(kvp.Key))
                            if (kvp.Value == null ||
                                string.IsNullOrEmpty(kvp.Value.ToString()))
                                dr[kvp.Key] = DBNull.Value;
                            else
                            {
                                if (typeDefinitions[kvp.Key] == typeof(int))
                                    dr[kvp.Key] = Convert.ToDouble(kvp.Value);

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
                number++;
            }
            return dt;
        }

        private static bool IsBlankLine(ISourceRecord rec)
        {
            bool isBlnakLine = true;
            foreach (KeyValuePair<string, object> kvp in rec.FieldSet)
            {
                if (kvp.Value != null &&
                    !string.IsNullOrEmpty(kvp.Value.ToString().Trim()))
                {
                    isBlnakLine = false;
                }
            }
            return isBlnakLine;
        }

    }
}
