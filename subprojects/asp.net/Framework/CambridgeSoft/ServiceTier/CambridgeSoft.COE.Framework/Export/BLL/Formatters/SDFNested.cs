using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using CambridgeSoft.COE.Framework.Common;
using System.Data;
using System.Xml;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COEExportService
{
    internal class SDFNested : FormatterBase, IFormatter
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
        /// reshape a dataset as an SDF flatfile that is uncorrelated
        /// </summary>
        /// <param name="dataSet">dataset to reshape</param>
        /// <returns>a sdf flat file containing molfiles for structures</returns>
        public string FormatDataSet(System.Data.DataSet dataSet, COEDataView dataView, ResultsCriteria resultCriteria)
        {
            resultCriteria = resultCriteria.RemoveGrandChild(dataView, resultCriteria);
            Common.SqlGenerator.MetaData.DataView _dataView = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView();
            _dataView.LoadFromXML(dataView.ToString());
            Common.SqlGenerator.MetaData.ResultsCriteria _resultCriteria = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.ResultsCriteria();
            _resultCriteria.LoadFromXML(resultCriteria.ToString());

            List<int> tablesIdList = new List<int>();
            List<int> childTableIdList = new List<int>();
            List<int>[] tablesID = _resultCriteria.GetTableIds(_dataView);
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
            string strOutSDF = string.Empty;
            string fulChildTableName = string.Empty;
            StringBuilder strTemp = new StringBuilder();           
            StringBuilder strCDXTOMOL = new StringBuilder();
            StringBuilder strbuildOutSDF = new StringBuilder();
            StringBuilder rowStr = new StringBuilder();

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

            int cdxColIndex = -1;
            for (int btColumnIndex = 0; btColumnIndex < btColumnCount; btColumnIndex++)
            {
                String colname = baseDatatab.Columns[btColumnIndex].ColumnName;
                btColNameList.Add(colname);
                if (colname.Contains("CDXTOMol_"))
                    cdxColIndex = btColumnIndex;
            }

            //loop the base table row
            for (int btRowIndex = 0; btRowIndex < dataSet.Tables[baseTableName].Rows.Count; btRowIndex++, btRecordIndex++)
            {
                //save the base table first row info to a list
                /* CSBR-161868 Log file information is not correct or appropriate when we Import an Exported file from CBV     
                   Checking whether the structure is available or not, If available then strCDXTOMOL will hold it 
                   otherwise it will hold the value in the value in else */
                if (cdxColIndex != -1 && !String.IsNullOrEmpty(baseDatatab.Rows[btRowIndex][cdxColIndex].ToString())) // Fixed 161868
                {
                    strCDXTOMOL.Append(baseDatatab.Rows[btRowIndex][cdxColIndex].ToString()); // + "\r\n";//\r\n Fixed CSBR-166992, CSBR-166995
                }
                else
                {
                    strCDXTOMOL.Append("\r\n");
                    strCDXTOMOL.Append("CsStruct  NA");
                    strCDXTOMOL.Append("\r\n\r\n");
                    strCDXTOMOL.Append("  0  0  0  0  0  0  0  0  0  0999 V2000");
                    strCDXTOMOL.Append("\r\n");
                    strCDXTOMOL.Append("M  END");
                    strCDXTOMOL.Append("\r\n");
                }

                for (int colNamLstItmIndex = 0; colNamLstItmIndex < btColNameList.Count; colNamLstItmIndex++)
                {
                    if (colNamLstItmIndex != cdxColIndex)
                    {
                        strTemp.Append(">  <");
                        strTemp.Append(fulBaseTabName);
                        strTemp.Append(".");
                        strTemp.Append(baseDatatab.Columns[colNamLstItmIndex].ColumnName);
                        strTemp.Append("> (");
                        strTemp.Append(btRecordIndex);
                        strTemp.Append(")\r\n");
                        strTemp.Append(baseDatatab.Rows[btRowIndex][colNamLstItmIndex]);
                        strTemp.Append("\r\n\r\n");
                    }
                }
                strbuildOutSDF.Append(strCDXTOMOL.ToString());
                strbuildOutSDF.Append(strTemp.ToString());
                if (childTableIdList.Count > 0)
                {
                    //loop the child tables
                    for (int chdTabIdIndex = 0; chdTabIdIndex < childTableIdList.Count; chdTabIdIndex++)
                    {

                        //filt the child table
                        strChdTabName = "Table_" + childTableIdList[chdTabIdIndex];
						// CBOE-303, CBOE-1763, CBOE-1764 get the child table alias name instead of table name
                        fulChildTableName = _dataView.GetTableAliasName(childTableIdList[chdTabIdIndex]);

                        filtDataRow = dataSet.Tables[strChdTabName].Select(String.Concat("[", dataSet.Tables[strChdTabName].ParentRelations[0].ChildColumns[0].ColumnName, "]") + "= '" + baseDatatab.Rows[btRowIndex][dataSet.Tables[strChdTabName].ParentRelations[0].ParentColumns[0].ColumnName]+"'");                        

                        if (filtDataRow.Length > 0)
                        {
                            filtChildDt = dataSet.Tables[strChdTabName].Clone();
                            foreach (DataRow dr in filtDataRow)
                            {
                                filtChildDt.ImportRow(dr);
                            }

                            strbuildOutSDF.Append(">  <");
                            strbuildOutSDF.Append(fulChildTableName);
                            strbuildOutSDF.Append("> (");
                            strbuildOutSDF.Append(btRecordIndex);
                            strbuildOutSDF.Append(")\r\n");
                            // child column names
                            for (int colIndex = 0; colIndex < filtChildDt.Columns.Count; colIndex++)
                            {
                                if (colIndex > 0)
                                {                                    
                                    strbuildOutSDF.Append(" --");
                                    strbuildOutSDF.Append(filtChildDt.Columns[colIndex].ColumnName);
                                }
                            }
                            strbuildOutSDF.Append("\r\n");
                            // child rows
                            for (int filtChildDtIndex = 0; filtChildDtIndex < filtChildDt.Rows.Count; filtChildDtIndex++)
                            {
                                for (int colIndex = 0; colIndex < filtChildDt.Columns.Count; colIndex++)
                                {
                                    if (colIndex > 0)
                                    {                                        
                                        strbuildOutSDF.Append(" --");
                                        strbuildOutSDF.Append(filtChildDt.Rows[filtChildDtIndex][colIndex]);
                                    }
                                }
                                strbuildOutSDF.Append("\r\n");
                            }

                            strbuildOutSDF.Append("\r\n");
                            filtDataRow = null;

                        }

                    }
                }
                strCDXTOMOL.Clear();
                strTemp.Clear();
                strbuildOutSDF.Append("$$$$\r\n");
            }
            strOutSDF = strbuildOutSDF.ToString();
            strOutSDF = strOutSDF.TrimEnd('\r', '\n'); 
            return strOutSDF;

        }
    }
}
