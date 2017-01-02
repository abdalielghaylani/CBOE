using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CambridgeSoft.COE.Framework.Common;
using System.Web;

namespace CambridgeSoft.COE.Framework.COESearchService {
    public class ResultFieldsToResultsCriteria {
        private ResultFieldsToResultsCriteria() { }

        public static ResultsCriteria GetResultCriteria(string[] fields, COEDataView dataView) {

            List<string> tableList = new List<string>();
            Dictionary<string, string> tableFields = new Dictionary<string, string>();
            ResultsCriteria resultCriteria = null;
           
            CriteriaUtilitiesT criteriaUtil = new CriteriaUtilitiesT();

            string value = string.Empty;

            string schemaName = string.Empty;
            string tableName = string.Empty;
            string fieldAlias = string.Empty;
            string fieldQualifier = string.Empty;

            string tableID = string.Empty;
            string fieldID = string.Empty;
            int tableIndex = 0;
            DataTable rcInfo = new DataTable("rc");
            rcInfo.Columns.Add("table", typeof(string));
            rcInfo.Columns.Add("fieldId", typeof(string));
            rcInfo.Columns.Add("fieldAlias", typeof(string));
            rcInfo.Columns.Add("resultType", typeof(string));

            StringBuilder sb = new StringBuilder();

            tableList.Clear();

            if(fields == null || fields.Length < 1)
                throw new Exception("At least one result field must be provided.");

            for(int x = 0; x < fields.Length; x++) {

                string resultType = "field";
                string[] value1 = fields[x].Split('.');

                if(value1.Length < 2)
                    throw new Exception("Wrong result field name \"" + value1 + "\", it must be in the form TABLE.FIELD");
                if (value1.Length == 2)
                {
                    //The escape character has replaced. It's used to retain the exact alias name if alias name contain dot operator
                    tableName = value1[0].Replace('\n', '.'); ;
                    fieldAlias = value1[1].Replace('\n', '.'); ;

                    tableID = criteriaUtil.GetResultTableID(dataView, tableName.ToLower().Trim());
                    tableIndex = criteriaUtil.GetResultTableIndex(dataView, tableName.ToLower().Trim());
                    fieldID = criteriaUtil.GetResultFieldID(dataView, tableIndex, fieldAlias.ToLower().Trim());                    
                }
                else if (value1.Length == 3)
                {
                    //The escape character has replaced. It's used to retain the exact alias name if alias name contain dot operator
                    schemaName = value1[0];
                    tableName = value1[1].Replace('\n', '.'); ;
                    fieldAlias = value1[2].Replace('\n', '.'); ;

                    tableID = criteriaUtil.GetResultTableID(dataView,schemaName, tableName);
                    tableIndex = criteriaUtil.GetResultTableIndex(dataView, tableName.ToLower().Trim());
                    fieldID = criteriaUtil.GetResultFieldID(dataView, tableIndex, fieldAlias.ToLower().Trim());                    
                }
                else if (value1.Length == 4)
                {
                    schemaName = value1[0];
                    tableName = value1[1].Replace('\n', '.'); ;
                    fieldAlias = value1[2].Replace('\n', '.'); ;
                    fieldQualifier = value1[3].Replace('\n', '.');

                    tableID = criteriaUtil.GetResultTableID(dataView, schemaName, tableName);
                    tableIndex = criteriaUtil.GetResultTableIndex(dataView, tableName.ToLower().Trim());
                    fieldID = criteriaUtil.GetResultFieldID(dataView, tableIndex, fieldAlias.ToLower().Trim());

                    if (fieldQualifier.ToLower() == "formula")
                        resultType = "formula";
                    else if (fieldQualifier.ToLower() == "molweight")
                        resultType = "molweight";
                    else
                        throw new Exception("Unknown Field Qualifier" + fieldQualifier);

                    fieldID = criteriaUtil.GetStructureResultFieldID(dataView, tableIndex);
                    if (string.IsNullOrEmpty(fieldID))
                    {
                        throw new Exception("Structure field was not found on dataview under Table \"" + tableName + "\".");
                    }                    

                }

                //string resultType = "field";

                if (string.IsNullOrEmpty(tableID) || tableIndex == -1)
                    throw new Exception("Table \"" + tableName + "\" was not found on dataview.");
                               

                if (!tableList.Contains(tableID))
                {
                    tableList.Add(tableID);
                }

                DataRow dr = rcInfo.NewRow();

                sb.Remove(0, sb.Length);
                sb.Append(tableID);
                dr["table"] = sb.ToString();

                sb.Remove(0, sb.Length);
                sb.Append(fieldID);
                dr["fieldId"] = sb.ToString();

                sb.Remove(0, sb.Length);
                sb.Append(fieldAlias);
                dr["fieldAlias"] = sb.ToString();

                sb.Remove(0, sb.Length);
                sb.Append(resultType);
                dr["resultType"] = sb.ToString();

                rcInfo.Rows.Add(dr);
            }

            tableFields.Clear();

            List<string> temp = new List<string>();
            string strTemp = string.Empty;

            foreach(string tbl in tableList) {
                string strExpr = "table = '" + tbl + "'";
                string rValue = string.Empty;

                DataRow[] Rows = rcInfo.Select(strExpr);
                foreach(DataRow SourceRow in Rows) {
                    strTemp = tbl + "_" + SourceRow["fieldId"] + "_" + SourceRow["fieldAlias"] + "_" + SourceRow["resultType"];
                    if(!temp.Contains(strTemp)) {
                        temp.Add(strTemp);
                       

                        if(SourceRow["resultType"].ToString() == "field")
                            // 12.3 -  Parsing the &,<,> special characters in xml 
                            rValue = rValue + "<field fieldId=\"" + SourceRow["fieldId"] + "\" alias=\"" + HttpUtility.HtmlEncode(Convert.ToString(SourceRow["fieldAlias"])) + "\"/>";
                        else if (SourceRow["resultType"].ToString() == "formula")
                            rValue = rValue + "<formula fieldId=\"" + SourceRow["fieldId"] + "\" alias=\"" + "FORMULA" + "\"/>";
                       
                        else if (SourceRow["resultType"].ToString() == "molweight")
                            rValue = rValue + "<molweight fieldId=\"" + SourceRow["fieldId"] + "\" alias=\"" + "MOLWEIGHT" + "\"/>";
                        
                    }
                }
                tableFields.Add(tbl.ToString(), rValue);
            }

            string rCriteria = string.Empty;
            rCriteria = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
            " <resultsCriteria xmlns=\"COE.ResultsCriteria\">" +
            " <tables>";

            foreach(KeyValuePair<string, string> pair in tableFields) {
                rCriteria = rCriteria + " <table id=\"" + pair.Key + "\">" + pair.Value.ToString() + " </table>";
            }

            rCriteria = rCriteria + " </tables> " +
            " </resultsCriteria>";

            //Load xml data into result criteria
            resultCriteria = new ResultsCriteria();
            resultCriteria.GetFromXML(rCriteria);

            return resultCriteria;
        }       
    }
    class CriteriaUtilitiesT {
        public CriteriaUtilitiesT() { }
        public string GetResultTableID(COEDataView dataView, string tableName) {
            for(int x = 0; x < dataView.Tables.Count; x++) {
                if(dataView.Tables[x].Alias.ToString().ToLower().Trim() == tableName)
                    return dataView.Tables[x].Id.ToString();
            }
            return null;

        }
         
        public string GetResultTableID(COEDataView dataView, string schemaName, string tableName){
            for(int x = 0; x < dataView.Tables.Count; x++) {
                if ((dataView.Tables[x].Alias.ToString().Trim().Equals(tableName,StringComparison.OrdinalIgnoreCase)) && dataView.Tables[x].Database.Equals(schemaName, StringComparison.OrdinalIgnoreCase))

                    return dataView.Tables[x].Id.ToString();
            }
            return null;

        }
        

        public int GetResultTableIndex(COEDataView dataView, string tableName) {

            for(int x = 0; x < dataView.Tables.Count; x++) {
                if(dataView.Tables[x].Alias.ToString().ToLower().Trim() == tableName)
                    return x;
            }
            return -1;
        }
        public int GetResultTableIndex(COEDataView dataView,  string tableName,string databaseName)
        {

            for (int x = 0; x < dataView.Tables.Count; x++)
            {
                if ((dataView.Tables[x].Alias.ToString().ToLower().Trim() == tableName) && (dataView.Tables[x].Database.ToString().ToLower().Trim() == databaseName)) 
                    return x;
            }
            return -1;
        }

        public string GetResultFieldID(COEDataView dataView, int tableIndex, string aliasName) {
            for(int x = 0; x < dataView.Tables[tableIndex].Fields.Count; x++) {
                if(dataView.Tables[tableIndex].Fields[x].Alias.ToString().ToLower().Trim() == aliasName)
                    return dataView.Tables[tableIndex].Fields[x].Id.ToString();
            }
            return null;
        }

        public string GetResultAliasName(COEDataView dataView, int tableIndex, string aliasName) {
            for(int x = 0; x < dataView.Tables[tableIndex].Fields.Count; x++) {
                if(dataView.Tables[tableIndex].Fields[0].Alias.ToString().ToLower().Trim() == aliasName)
                    return dataView.Tables[tableIndex].Fields[x].Alias.ToString();
            }
            return null;
        }


        public string GetStructureResultFieldID(COEDataView dataView, int tableIndex)
        {
            for(int x = 0; x < dataView.Tables[tableIndex].Fields.Count; x++)
            {
                if((dataView.Tables[tableIndex].Fields[x].IndexType == COEDataView.IndexTypes.CS_CARTRIDGE) || (dataView.Tables[tableIndex].Fields[x].LookupFieldId >0))  //Fixed CSBR-153856
                    return dataView.Tables[tableIndex].Fields[x].Id.ToString();
            }
            return null;
        }
    }
}
