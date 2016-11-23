using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Office = Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;

namespace ChemBioVizExcelAddIn
{
    class ResultCriteria
    {
        # region _ Variables _

        private Excel::Worksheet nSheet;

        private List<string> tableList = new List<string>();

        private Dictionary<string, string> tableFields = new Dictionary<string, string>();

        #endregion

        #region _ Constructor _

        public ResultCriteria(ref Excel::Worksheet worksheet)
        {
            nSheet = worksheet;
        }
        public ResultCriteria()
        {
            nSheet = null;
        }

        #endregion

        # region _ Private Methods _

        private string GetHeaderTableInfo(int Col, object Sheet, string Parameter)
        {
            CriteriaUtilities criteriaUtil = new CriteriaUtilities();

            return criteriaUtil.GetHeaderTableInfo(Col, Sheet, Parameter);
        }

        private string GetHeaderColumnInfo(int Col, object Sheet, string Parameter)
        {
            CriteriaUtilities criteriaUtil = new CriteriaUtilities();

            return criteriaUtil.GetHeaderColumnInfo(Col, Sheet, Parameter);
        }

        # endregion

        # region _ Public Methods _

        public string GetResultCriteria(ref Excel::Worksheet worksheet)
        {
            this.nSheet = worksheet;

            return GetResultCriteria();
        }

        public string GetResultCriteria()
        {
            Excel::Worksheet nSheet = this.nSheet as Excel::Worksheet;

            string value = string.Empty;

            string tableId = string.Empty;
            string fieldId = string.Empty;
            string fieldAlias = string.Empty;

            DataTable rcInfo = new DataTable("rc");
            rcInfo.Columns.Add("table", typeof(string));
            rcInfo.Columns.Add("fieldId", typeof(string));
            rcInfo.Columns.Add("fieldAlias", typeof(string));

            StringBuilder sb = new StringBuilder();

            tableList.Clear();
            int maxCnt = Global.CBVNewColumnIndex + Global.StartUpRowPosition(nSheet);
            for (int x = 1; x < maxCnt + Global.StartUpRowPosition(nSheet); x++)
            {
                tableId = GetHeaderTableInfo(x, nSheet,Global.ID);
                fieldId = GetHeaderColumnInfo(x, nSheet, Global.ID);
                fieldAlias = GetHeaderColumnInfo(x, nSheet,Global.ALIAS);

                if (!tableList.Contains(tableId))
                {
                    tableList.Add(tableId);
                }

                DataRow dr = rcInfo.NewRow();

                sb.Remove(0, sb.Length);
                sb.Append(tableId);
                dr["table"] = sb.ToString();

                sb.Remove(0, sb.Length);
                sb.Append(fieldId);
                dr["fieldId"] = sb.ToString();

                sb.Remove(0, sb.Length);
                sb.Append(fieldAlias);
                dr["fieldAlias"] = sb.ToString();

                rcInfo.Rows.Add(dr);
            }


            tableFields.Clear();

            List<string> temp = new List<string>();
            string strTemp = string.Empty;

            foreach (string tbl in tableList)
            {
                string strExpr = "table = '" + tbl + "'";
                string rValue = string.Empty;

                DataRow[] Rows = rcInfo.Select(strExpr);
                foreach (DataRow SourceRow in Rows)
                {
                    strTemp = tbl + "_" + SourceRow["fieldId"] + "_" + SourceRow["fieldAlias"];
                    if (!temp.Contains(strTemp))
                    {
                        temp.Add(strTemp);
                        rValue = rValue + "<field fieldId=\"" + SourceRow["fieldId"] + "\" alias=\"" + SourceRow["fieldAlias"] + "\"/>";
                    }
                }
                tableFields.Add(tbl.ToString(), rValue);
            }

            string rCriteria = string.Empty;

            rCriteria = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
            " <resultsCriteria xmlns=\"COE.ResultsCriteria\">" +
            " <tables>";

            foreach (KeyValuePair<string, string> pair in tableFields)
            {
                rCriteria = rCriteria + " <table id=\"" + pair.Key + "\">" + pair.Value.ToString() + " </table>";
            }

            rCriteria = rCriteria + " </tables> " +
            " </resultsCriteria>";

            return rCriteria;
        }

        public string GetResultCriteriaJS(string[] fields)
        {
            Excel::Worksheet nSheet = this.nSheet as Excel::Worksheet;
            CriteriaUtilities criteriaUtil = new CriteriaUtilities();
            string value = string.Empty;

            string tableId = string.Empty;
            string fieldId = string.Empty;
            string fieldAlias = string.Empty;

            DataTable rcInfo = new DataTable("rc");
            rcInfo.Columns.Add("table", typeof(string));
            rcInfo.Columns.Add("fieldId", typeof(string));
            rcInfo.Columns.Add("fieldAlias", typeof(string));

            StringBuilder sb = new StringBuilder();

            tableList.Clear();

            for (int x = 1; x < fields.Length; x++)
            {
                string[] value1 = fields[x].ToString().Split('.');

                //tableId = value1[0].ToString();
                //fieldId = value1[1].ToString();
                //fieldAlias = value1[1].ToString();

                tableId = GetHeaderTableInfo(x, nSheet, value1[0].ToString());
                fieldId = GetHeaderColumnInfo(x, nSheet, "");
                fieldAlias = GetHeaderColumnInfo(x, nSheet, Global.ALIAS);
                
                
                if (!tableList.Contains(tableId))
                {
                    tableList.Add(tableId);
                }

                DataRow dr = rcInfo.NewRow();

                sb.Remove(0, sb.Length);
                sb.Append(tableId);
                dr["table"] = sb.ToString();

                sb.Remove(0, sb.Length);
                sb.Append(fieldId);
                dr["fieldId"] = sb.ToString();

                sb.Remove(0, sb.Length);
                sb.Append(fieldAlias);
                dr["fieldAlias"] = sb.ToString();

                rcInfo.Rows.Add(dr);
            }


            tableFields.Clear();

            List<string> temp = new List<string>();
            string strTemp = string.Empty;

            foreach (string tbl in tableList)
            {
                string strExpr = "table = '" + tbl + "'";
                string rValue = string.Empty;

                DataRow[] Rows = rcInfo.Select(strExpr);
                foreach (DataRow SourceRow in Rows)
                {
                    strTemp = tbl + "_" + SourceRow["fieldId"] + "_" + SourceRow["fieldAlias"];
                    if (!temp.Contains(strTemp))
                    {
                        temp.Add(strTemp);
                        rValue = rValue + "<field fieldId=\"" + SourceRow["fieldId"] + "\" alias=\"" + SourceRow["fieldAlias"] + "\"/>";
                    }
                }
                tableFields.Add(tbl.ToString(), rValue);
            }

            string rCriteria = string.Empty;

            rCriteria = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
            " <resultsCriteria xmlns=\"COE.ResultsCriteria\">" +
            " <tables>";

            foreach (KeyValuePair<string, string> pair in tableFields)
            {
               // rCriteria = rCriteria + " <table id=\"" + pair.Key + "\">" + pair.Value.ToString() + " </table>";
                rCriteria = rCriteria + " <table name=\"" + pair.Key + "\">" + pair.Value.ToString() + " </table>";
            }

            rCriteria = rCriteria + " </tables> " +
            " </resultsCriteria>";

            return rCriteria;
        }
        # endregion

    }
}
