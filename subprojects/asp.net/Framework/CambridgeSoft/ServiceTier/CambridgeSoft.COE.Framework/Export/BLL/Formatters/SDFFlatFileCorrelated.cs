using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using CambridgeSoft.COE.Framework.Common;
using System.Data;//Jerry
using System.Xml;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COEExportService
{
    internal class SDFFlatFileCorrelated : FormatterBase, IFormatter
    {
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEExport");
        /// <summary>
        /// Overridden ModifyResultsCriteria to format structure fields to return molfiles
        /// </summary>
        /// <param name="resultsCriteria">originating results criteria object</param>
        /// <returns>results critiria modified to include criteria itme in desired format</returns>
        protected override void Modify(ResultsCriteria.ResultsCriteriaTable resultsCriteriaTable, int fieldID)
        {
            ResultsCriteria.CDXToMolFile newCriteria = new ResultsCriteria.CDXToMolFile(fieldID);
            newCriteria.Alias = "CDXTOMol_" + fieldID;
            bool alreadyExists = false;

            foreach (ResultsCriteria.IResultsCriteriaBase rc in resultsCriteriaTable.Criterias)
            {
                if (rc.Alias == newCriteria.Alias)
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (!alreadyExists)
                resultsCriteriaTable.Criterias.Add(newCriteria);
        }

        /// <summary>
        /// reshape a dataset as an SDF flatfile that is correlated
        /// </summary>
        /// <param name="dataSet">dataset to reshape</param>
        /// <returns>a sdf flat file containing molfiles for structures  modified on 12.5</returns>
        public string FormatDataSet(System.Data.DataSet dataSet, COEDataView dataView, ResultsCriteria resultCriteria)
        {
            resultCriteria = resultCriteria.RemoveGrandChild(dataView, resultCriteria);
            Common.SqlGenerator.MetaData.DataView _dataView = new Common.SqlGenerator.MetaData.DataView();
            _dataView.LoadFromXML(dataView.ToString());
            Common.SqlGenerator.MetaData.ResultsCriteria _resultsCriteria = new Common.SqlGenerator.MetaData.ResultsCriteria();
            _resultsCriteria.LoadFromXML(resultCriteria.ToString());

            XmlDocument _xmlDocument = new XmlDocument();
            _xmlDocument.LoadXml(resultCriteria.ToString());
            List<List<string>> tableFiledsIdList = new List<List<string>>();
            XmlNodeList tabXmlNodeList = _xmlDocument.DocumentElement.ChildNodes[0].ChildNodes;

            foreach (XmlElement tabElement in tabXmlNodeList)
            {
                if (!tabElement.Attributes["id"].Value.Trim().Equals(_dataView.GetBaseTableId().ToString()))
                {
                    XmlNodeList fieldXmlNodeList = tabElement.ChildNodes;
                    List<string> filedsIdList = new List<string>();
                    foreach (XmlElement fieldElement in fieldXmlNodeList)
                    {
                        string sFieldName = GetChildFieldName(fieldElement, _dataView);
                        if (!String.IsNullOrEmpty(sFieldName))
                            filedsIdList.Add(sFieldName.Trim());
                    }
                    tableFiledsIdList.Add(filedsIdList);
                }

            }

            List<int> tablesIdList = new List<int>();
            List<int> childTableIdList = new List<int>();
            List<int>[] tablesID = _resultsCriteria.GetTableIds(_dataView);
            for (int i = 0; i < tablesID.Length; i++)
            {
                tablesIdList.Add(int.Parse(tablesID[i][0].ToString()));
            }
            for (int itemIndex = 0; itemIndex < tablesID.Length; itemIndex++)
            {
                if (tablesID[itemIndex][0] != _dataView.GetBaseTableId())
                    childTableIdList.Add(int.Parse(tablesID[itemIndex][0].ToString()));
            }

            string baseTableName = "Table_" + _dataView.GetBaseTableId().ToString();
            DataTable baseDatatab = dataSet.Tables[baseTableName];
            string fulBaseTabName = _dataView.GetTableName(_dataView.GetBaseTableId());
            int btColumnCount = baseDatatab.Columns.Count;
            List<string> btColNameList = new List<string>();
            int btRecordIndex = 1;
            DataTable filtChildDt = new DataTable();
            DataRow[] filtDataRow;
            List<string> tempList = new List<string>();
            string strChdTabName = string.Empty;
            string strTemp = string.Empty;
            string strOutSDF = string.Empty;
            string strCDXTOMOL = string.Empty;

            //save the base table column name into a list
            int cdxColIndex = -1;
            for (int btColumnIndex = 0; btColumnIndex < btColumnCount; btColumnIndex++)
            {
                String colname = baseDatatab.Columns[btColumnIndex].ColumnName;
                btColNameList.Add(colname);
                if (colname.Contains("CDXTOMol_"))
                    cdxColIndex = btColumnIndex; 
                // Removed the else statement from here and placed it in below condition   
            }
            //loop the base table row
            for (int btRowIndex = 0; btRowIndex < dataSet.Tables[baseTableName].Rows.Count; btRowIndex++, btRecordIndex++) 
            {
                //save the base table first row info to a list
                /* CSBR-161868 Log file information is not correct or appropriate when we Import an Exported file from CBV     
                   Checking whether the structure is available or not, If available then strCDXTOMOL will hold it 
                   otherwise it will hold the value in the value in else */
                if (cdxColIndex != -1 && !String.IsNullOrEmpty(baseDatatab.Rows[btRowIndex][cdxColIndex].ToString())) // Fixed 161868
                    strCDXTOMOL = baseDatatab.Rows[btRowIndex][cdxColIndex].ToString(); // + "\r\n";//\r\n Fixed CSBR-166992, CSBR-166995
                // if the condition is not satisfied then strCDXTOMOL must be set to value in else statement  
                else
                    strCDXTOMOL = "\r\n" + "CsStruct  NA" + "\r\n\r\n" + "  0  0  0  0  0  0  0  0  0  0999 V2000" + "\r\n" + "M  END" + "\r\n"; //Fixed CSBR-158168, CSBR-166992, CSBR-166995
                /* end of CSBR-161886 */ 

                for (int colNamLstItmIndex = 0; colNamLstItmIndex < btColNameList.Count; colNamLstItmIndex++)
                {
                    if (colNamLstItmIndex != cdxColIndex)
                        strTemp = strTemp + ">  <" + fulBaseTabName + "." + baseDatatab.Columns[colNamLstItmIndex].ColumnName + "> (" + btRecordIndex + ")\r\n" + baseDatatab.Rows[btRowIndex][colNamLstItmIndex] + "\r\n\r\n";
                }
                tempList.Add(strTemp);
                List<string> tp = new List<string>();
                //loop every child table

                for (int chdTabIdIndex = 0; chdTabIdIndex < childTableIdList.Count; chdTabIdIndex++)
                {
                    //filt the child table
                    strChdTabName = "Table_" + childTableIdList[chdTabIdIndex].ToString();
                    string fullTableName = _dataView.GetTableName(childTableIdList[chdTabIdIndex]);
                    filtDataRow = dataSet.Tables[strChdTabName].Select(String.Concat("[", dataSet.Tables[strChdTabName].ParentRelations[0].ChildColumns[0].ColumnName, "]") + "= '" + baseDatatab.Rows[btRowIndex][dataSet.Tables[strChdTabName].ParentRelations[0].ParentColumns[0].ColumnName]+ "'");
                    filtChildDt = dataSet.Tables[strChdTabName].Clone();
                    if (filtDataRow.Length > 0)
                    {
                        foreach (DataRow dr in filtDataRow)
                        {
                            filtChildDt.ImportRow(dr);
                        }
                        tp = tempList;
                        tempList = AddChildTableData(tp, filtChildDt, btRecordIndex, tableFiledsIdList, _dataView, chdTabIdIndex, fullTableName);
                        tp.Clear();
                    }
                    filtDataRow = null;

                }

                string tempOutStr = string.Empty;
                for (int lstItemIndex = 0; lstItemIndex < tempList.Count; lstItemIndex++)
                {
                    tempOutStr = tempOutStr + tempList[lstItemIndex];
                }
                //save CDXTOMOL field value to the front of base table every row  
                strOutSDF = strOutSDF + strCDXTOMOL + tempOutStr + "$$$$\r\n";
                tempList.Clear();
                strTemp = string.Empty;
            }
            strOutSDF = strOutSDF.TrimEnd('\r', '\n'); //Fixed 160850
            return strOutSDF;

        }

        /// <summary>
        /// add every row info of child table into a list
        /// </summary>
        /// <param name="parTempList">the previous base table and child table info string list</param>
        /// <param name="childTable">the current child table</param>
        /// <param name="parBtRecordIndex">base table record index</param>
        /// <param name="_tableFiledsIdList">the filed list of all fields in every  table</param>
        /// <param name="_dataView">COEDataView</param>
        /// <param name="_chdTabIdIndex">child table id index in child table id list</param>
        /// <returns></returns>
        private List<string> AddChildTableData(List<string> parTempList, DataTable childTable, int parBtRecordIndex, List<List<string>> _tableFiledsIdList, Common.SqlGenerator.MetaData.DataView _dataView, int _chdTabIdIndex, string childTabName)
        {
            string strCTRowInfo = string.Empty;
            List<string> lstCTRowInfo = new List<string>();
            string strBCTRowInfo = string.Empty;
            List<string> lstBCTRowInfo = new List<string>();
            List<string> tempList = new List<string>();
            string columnName = string.Empty;
            string columnValue = string.Empty;
            int childTabId = 0;
            //save enery row information of the child table to a list
            for (int rowIndex = 0; rowIndex < childTable.Rows.Count; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < childTable.Columns.Count; columnIndex++)
                {
                    columnName = childTable.Columns[columnIndex].ColumnName;
                    columnValue = childTable.Rows[rowIndex][columnIndex].ToString();
                    strCTRowInfo = strCTRowInfo + ">  <" + childTabName + "." + columnName + "> (" + parBtRecordIndex + ")\r\n" + columnValue + "\r\n\r\n";
                }
                lstCTRowInfo.Add(strCTRowInfo);
                strCTRowInfo = string.Empty;
            }

            //loop the previous base table and child table info string list
            for (int listItemIndex = 0; listItemIndex < parTempList.Count; listItemIndex++)
            {
                // save the information of base table and child table to a  list
                for (int indexCTRowInfo = 0; indexCTRowInfo < lstCTRowInfo.Count; indexCTRowInfo++)
                {
                    strBCTRowInfo = parTempList[listItemIndex] + lstCTRowInfo[indexCTRowInfo];
                    lstBCTRowInfo.Add(strBCTRowInfo);
                }
            }
            lstCTRowInfo.Clear();
            return lstBCTRowInfo;
        }
    }
}
