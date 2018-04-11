using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Data.Common;
using CambridgeSoft.COE.Framework.COESearchService;
using System.IO;
using Office = Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;

namespace ChemBioVizExcelAddIn
{
    class DataSetUtilities
    {
        # region _ Variables _

        public DataSet ds;

        #endregion

        #region _ Constructor _

        public DataSetUtilities(ref DataSet DataSet)
        {
            ds = DataSet;
        }
        public DataSetUtilities()
        {
            ds = null;
        }

        #endregion

        #region _ Create Relation between DataTables _

        public void CreateRelation(string Relation, int FirstTableId, int FirstColumnId, int SecondTableId, int SecondColumnId)
        {
            try
            {
                ds.Relations.Add(Relation, ds.Tables[FirstTableId].Columns[FirstColumnId], ds.Tables[SecondTableId].Columns[SecondColumnId]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateRelation(string Relation, int FirstTableId, string FirstColumnName, int SecondTableId, string SecondColumnName)
        {
            try
            {
                ds.Relations.Add(Relation, ds.Tables[FirstTableId].Columns[FirstColumnName], ds.Tables[SecondTableId].Columns[SecondColumnName]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateRelation(string Relation, string FirstTableName, int FirstColumnId, string SecondTableName, int SecondColumnId)
        {
            try
            {
                ds.Relations.Add(Relation, ds.Tables[FirstTableName].Columns[FirstColumnId], ds.Tables[SecondTableName].Columns[SecondColumnId]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateRelation(string Relation, string FirstTableName, string FirstColumnName, string SecondTableName, string SecondColumnName)
        {
            try
            {
                ds.Relations.Add(Relation, ds.Tables[FirstTableName].Columns[FirstColumnName], ds.Tables[SecondTableName].Columns[SecondColumnName]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        # region _ CBV Field List Collection _
        private string m_FieldList;
        /// <summary>
        /// This code Validate fldRelationshipList into FieldInfo objects  and then
        /// adds them to the Global.FieldInfo private member
        /// fldRelationshipList syntax:  [relationname.]fieldname[@alias], ...
        /// </summary>
        /// <param name="fldRelationshipList"></param>
        public void ValidateFieldList(string fldRelationshipList)
        {
            if (m_FieldList == fldRelationshipList)
                return;

            Global.FieldInfo = new System.Collections.ArrayList();
            m_FieldList = fldRelationshipList;

            string[] FieldParts;
            string[] Fields = fldRelationshipList.Split(',');
            for (int i = 0; i <= Fields.Length - 1; i++)
            {
                CBVFieldInfo Field = new CBVFieldInfo();


                if (Fields[i] == null)
                {
                    return;
                }
                else if (String.Empty == Fields[i])
                {
                    Field.RelationName = string.Empty;
                    Field.FieldAlias = string.Empty;
                    Field.FieldName = string.Empty;
                }
                else
                {
                    //Set FieldName and RelationName
                    FieldParts = Fields[i].Trim(Global.charsToTrim).Split('.');
                    switch (FieldParts.Length)
                    {
                        case 1:
                            Field.FieldName = FieldParts[0].Replace(Global.ESCAPE_SEPERATOR_FOR_DOT, Global.DOT_SEPARATOR).Replace(Global.ESCAPE_SEPERATOR_FOR_COMMA, Global.COMMA_SEPARATOR);
                            break;
                        case 2:
                            Field.RelationName = FieldParts[0].Trim(Global.charsToTrim).Replace(Global.ESCAPE_SEPERATOR_FOR_DOT, Global.DOT_SEPARATOR).Replace(Global.ESCAPE_SEPERATOR_FOR_COMMA, Global.COMMA_SEPARATOR);
                            Field.FieldName = FieldParts[1].Trim(Global.charsToTrim).Replace(Global.ESCAPE_SEPERATOR_FOR_DOT, Global.DOT_SEPARATOR).Replace(Global.ESCAPE_SEPERATOR_FOR_COMMA, Global.COMMA_SEPARATOR);
                            break;
                        default:
                            throw new Exception("Invalid field definition: '" + Fields[i] + "'.");
                    }
                    if (Field.FieldAlias == null)
                        Field.FieldAlias = Field.FieldName.Replace(Global.ESCAPE_SEPERATOR_FOR_DOT, Global.DOT_SEPARATOR).Replace(Global.ESCAPE_SEPERATOR_FOR_COMMA, Global.COMMA_SEPARATOR);
                }

                Global.FieldInfo.Add(Field);
            }
        }

        #endregion

        #region _ Create CBV Table Schema and Result in CBV Table _

        /// <summary>
        /// Creates a View based on fields of another table and related parent tables
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="sourceTable"></param>
        /// <param name="fldRelationshipList">
        /// </param>
        /// <returns></returns>   

        private bool FieldExists(string fieldName, DataTable sourceTable)
        {

            foreach (DataColumn dtCol in sourceTable.Columns)
            {
                if (fieldName.Equals(dtCol.ColumnName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }



        /// <summary>
        /// Creates a View based on fields of another table and related parent tables. As well as consider the empty columns
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="sourceTable"></param>
        /// <param name="fldRelationshipList">
        /// </param>
        /// <returns></returns>     
        public DataTable CreateView(string TableName, DataTable sourceTable, string fldRelationshipList, Excel::Worksheet nSheet)
        {
            try
            {
                if (fldRelationshipList == null)
                {
                    DataTable dt = new DataTable(TableName);
                    dt = sourceTable.Copy();
                    dt.TableName = TableName;
                    //return dt;
                    if (ds != null)
                    {
                        try
                        {
                            ds.Tables.Add(dt);
                        }
                        catch
                        { }
                    }

                    return dt;
                }
                else
                {
                    DataTable dt = new DataTable(TableName);
                    string tableName = string.Empty;
                    string indexType = string.Empty;
                    string mimeType = string.Empty;
                    string optionHeader = string.Empty;
                    int cnt = 0; //use counter for maintaining the columns   



                    int colCnt = nSheet.UsedRange.Columns.Count;
                    object[,] cellRngVal = new object[1, colCnt];

                    //The array have double dimensation
                    if (colCnt==1)
                        colCnt =2;

                    Excel.Range cellRange = nSheet.get_Range("A" + (int)Global.CBVHeaderRow.HeaderOptionRow, Global.NumToString(colCnt) + (int)Global.CBVHeaderRow.HeaderOptionRow);
                    cellRngVal = (object[,])cellRange.Value2;

                    foreach (CBVFieldInfo Field in Global.FieldInfo)
                    {
                        ++cnt;

                        //The entire field has found empty then add a empty column and continue...
                        if (Field.FieldName == string.Empty && Field.FieldAlias == string.Empty && Field.RelationName == string.Empty && Field.FieldName == String.Empty)
                        {
                            tableName = sourceTable.TableName.ToString();

                            // 12.3 - Proper Separator used instead of #
                            dt.Columns.Add(tableName + Global.COLUMN_SEPARATOR + "NONE" + Global.COLUMN_SEPARATOR + "EMPTY" + Global.COLUMN_SEPARATOR + "NONE" + Global.COLUMN_SEPARATOR + "NONE" + Global.COLUMN_SEPARATOR + cnt, Type.GetType("System.String"));
                            continue;

                        }

                        optionHeader = DataSetUtilities.GetDataFromCellRange(cellRngVal, 1, cnt);

                        if (Field.RelationName == null)
                        {
                            DataColumn dc = sourceTable.Columns[Field.FieldName];
                            tableName = sourceTable.TableName.ToString();

                            using (COEDataViewData cDataVW = new COEDataViewData())
                            {
                                indexType = cDataVW.IndexType(dc.ColumnName, tableName.ToLower(), true);
                                mimeType = cDataVW.MimeType(dc.ColumnName, tableName.ToLower(), true);
                            }
                            // 12.3 - Proper Separator used instead of #
                            dt.Columns.Add(tableName + Global.COLUMN_SEPARATOR + indexType + Global.COLUMN_SEPARATOR + dc.ColumnName + Global.COLUMN_SEPARATOR + optionHeader + Global.COLUMN_SEPARATOR + mimeType + Global.COLUMN_SEPARATOR + cnt, dc.DataType, dc.Expression);
                        }
                        else if (Field.FieldName == string.Empty && Field.FieldAlias == string.Empty && Field.RelationName == string.Empty)
                        {
                            tableName = sourceTable.TableName.ToString();
                            // 12.3 - Proper Separator used instead of #
                            dt.Columns.Add(tableName + Global.COLUMN_SEPARATOR + "NONE" + Global.COLUMN_SEPARATOR + "EMPTY" + Global.COLUMN_SEPARATOR + optionHeader + Global.COLUMN_SEPARATOR + "NONE" + Global.COLUMN_SEPARATOR + cnt, Type.GetType("System.String"));

                        }
                        else
                        {
                            DataColumn dc = sourceTable.ChildRelations[Field.RelationName].ChildTable.Columns[Field.FieldName];

                            tableName = sourceTable.ChildRelations[Field.RelationName].ChildTable.TableName.ToString();

                            using (COEDataViewData cDataVW = new COEDataViewData())
                            {
                                indexType = cDataVW.IndexType(dc.ColumnName, tableName.ToLower(), true);
                                mimeType = cDataVW.MimeType(dc.ColumnName, tableName.ToLower(), true);

                            }
                            // 12.3 - Proper Separator used instead of #
                            dt.Columns.Add(tableName + Global.COLUMN_SEPARATOR + indexType + Global.COLUMN_SEPARATOR + dc.ColumnName + Global.COLUMN_SEPARATOR + optionHeader + Global.COLUMN_SEPARATOR + mimeType + Global.COLUMN_SEPARATOR + cnt, dc.DataType, dc.Expression);
                        }
                    }
                    if (ds != null)
                    {
                        ds.Tables.Add(dt);
                    }
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetCBVDataFromBaseTable(DataTable destTable, DataTable sourceTable, string fldRelationshipList, Excel::Worksheet nSheet, bool ISSplitCellsOptionExists, bool ISMultipleRowsOptionExists)
        {
            DataTable destTable1 = null;

            // Code moved below after modifying the destTable
            //Global._CBVResult = new DataTable();
            //Global._CBVResult = destTable.Clone();
            string optionHeader = string.Empty;
            string optionResult = string.Empty;
            string actualResult = string.Empty;

            ValidateFieldList(fldRelationshipList);
            DataRow[] Rows = sourceTable.Select();

            // loop to modify the destTable, to change the datatype to "string" of those columns that contains multiple values in a single row (having one to many relationship with the source table)
            foreach (DataRow SourceRow in Rows)
            {
                int cntCol = 0;

                foreach (CBVFieldInfo Field in Global.FieldInfo)
                {                   

                    if (Convert.ToString(Field.RelationName) != string.Empty && Field.FieldName != string.Empty && Field.FieldAlias != string.Empty)
                    {
                        DataRow[] ChildRows = SourceRow.GetChildRows(Field.RelationName);

                        if (ChildRows.Length > 1)
                        {
                            if (destTable.Columns[cntCol].DataType != Type.GetType("System.String"))
                            {
                                destTable.Columns[cntCol].DataType = Type.GetType("System.String");
                                destTable.AcceptChanges();
                            }
                        }
                    }

                    cntCol++;
                }
            }

            Global._CBVResult = new DataTable();
            Global._CBVResult = destTable.Clone();

            Global._ExcelApp.EnableEvents = false;
            string[] oHeaderArray = new string[Global.FieldInfo.Count];
            string[] oHeaderColumnRowArray = new string[Global.FieldInfo.Count];

            foreach (DataRow SourceRow in Rows)
            {              

                DataRow DestRow = destTable.NewRow();
                              

                DataRow CBVRow = Global._CBVResult.NewRow();
                //this blanks out the array in each loop
                //move out of looo
                //string[] oHeaderArray = new string[Global.FieldInfo.Count];

                // counter variable for each row
                int cnt = 0;
                destTable1 = destTable.Clone();

                foreach (CBVFieldInfo Field in Global.FieldInfo)
                {
                    if (oHeaderArray[cnt] != null)
                        optionHeader = oHeaderArray[cnt].ToString();
                    else
                    {   
                        object oHeaderValue = ((Excel.Range)nSheet.Cells[Global.CBVHeaderRow.HeaderOptionRow, cnt + 1]).Value2;

                        if (oHeaderValue != null)
                        {
                            optionHeader = oHeaderValue.ToString();
                            oHeaderArray[cnt] = optionHeader;
                        }
                    }


                    if (Field.RelationName == null)
                    {
                        string tableName = string.Empty;
                        tableName = SourceRow.Table.TableName.ToString();
                        optionResult = DisplayOption.OptionCalculation(SourceRow[Field.FieldName].ToString(), optionHeader);
                        DestRow[cnt] = optionResult;
                        CBVRow[cnt] = SourceRow[Field.FieldName];
                        //counter increment  for next column
                        cnt++;
                    }
                    else if (Field.RelationName == string.Empty && Field.FieldName == string.Empty && Field.FieldAlias == string.Empty)
                    {
                        DestRow[cnt] = string.Empty;
                        CBVRow[cnt] = string.Empty;
                        //counter increment  for next column
                        cnt++;
                    }
                    else
                    {
                        DataRow[] ChildRows = SourceRow.GetChildRows(Field.RelationName);
                        StringBuilder strBld = new StringBuilder();
                        string tableName = string.Empty;
                        //check where the child rows contains one or more rows.
                      
                        //This is not even being used!!!!!
                        //string colName = ((Excel::Range)nSheet.Cells[Global.CBVHeaderRow.HeaderColumnRow, cnt + 1]).Value2.ToString().ToLower();
                        
                        string strField = Field.FieldName.ToString();

                        List<string> lstVar = new List<string>();
                        if (ChildRows.Length == 0)
                        {
                            cnt++;
                        }
                        else if (ChildRows.Length == 1)
                        {
                            strBld.Append(ChildRows[0][Field.FieldName].ToString());
                            optionResult = DisplayOption.OptionCalculation(strBld.ToString(), optionHeader);
                            DestRow[cnt] = optionResult;
                            CBVRow[cnt] = strBld.ToString();
                            cnt++;
                        }
                        else if (ChildRows.Length > 1)
                        {
                            foreach (DataRow ChildRow in ChildRows)
                            {
                                tableName = ChildRow.Table.TableName.ToString();

                                if (ChildRow[Field.FieldName] != null)
                                {
                                    lstVar.Add(ChildRow[Field.FieldName].ToString());
                                    DataRow DestRow1 = destTable1.NewRow();
                                    DestRow1[cnt] = ChildRow[Field.FieldName].ToString();
                                    destTable1.Rows.Add(DestRow1);
                                }
                            }

                            if (!OrderByCriteriaExists(optionHeader))
                            {
                                optionResult = DisplayOption.OptionCalculation(lstVar, optionHeader);
                                DestRow[cnt] = optionResult;
                            }
                            actualResult = DisplayOption.ListVarIntoString(lstVar);
                            CBVRow[cnt] = actualResult;
                            cnt++;
                            lstVar.Clear();
                        }
                    }
                }

                if (destTable1 != null && destTable1.Rows.Count > 0)
                {
                    DestRow = GetSortedOrderCBVDataFromChildTable(destTable1, DestRow, nSheet);
                }
                destTable.Rows.Add(DestRow);
                Global._CBVResult.Rows.Add(CBVRow);
            }

            if (ISSplitCellsOptionExists && ISMultipleRowsOptionExists)
            {
                destTable = CBVResultSplitCellsMultipleRows(destTable).Copy();
            }
            else if (ISSplitCellsOptionExists)
            {
                destTable = CBVResultSplitCells(destTable).Copy();
            }
            else if (ISMultipleRowsOptionExists)
            {
                destTable = CBVResultMultipleRows(destTable).Copy();
            }

            Global._ExcelApp.EnableEvents = true;

            return destTable;
        }

        public DataRow GetSortedOrderCBVDataFromChildTable(DataTable destChildTable1, DataRow DestRow, Excel::Worksheet nSheet)
        {
            DataTable destChildTable2 = destChildTable1.Clone();
            CriteriaUtilities criteriaUtilities = new CriteriaUtilities();
            Type dataType = null;
            int cnt = 1;

            //Set the real column data types
            foreach (DataColumn dcDest2 in destChildTable1.Columns)
            {
                dataType = DisplayOption.OptionHeaderSystemDataType(criteriaUtilities.GetHeaderColumnInfo(cnt, nSheet, Global.DATATYPE));
                destChildTable2.Columns[dcDest2.ColumnName].DataType = dataType;
                cnt++;
            }

            //Add bank rows - no of rows in destChildTable1
            for (int r = 0; r < destChildTable1.Rows.Count; r++)
            {
                DataRow drNew = destChildTable2.NewRow();
                destChildTable2.Rows.Add(drNew);
            }
            int rw = 0;
            // DataRow drNew = null;
            for (int c = 0; c < destChildTable1.Columns.Count; c++)
            {
                rw = 0;
                for (int r = 0; r < destChildTable1.Rows.Count; r++)
                {
                    if (!string.IsNullOrEmpty(destChildTable1.Rows[r][c].ToString()))
                    {
                        destChildTable2.Rows[rw][c] = destChildTable1.Rows[r][c];
                        rw++;
                    }
                }
            }

            //Remove the empty rows from table
            bool isEmpty = true;
            for (int i = 0; i < destChildTable2.Rows.Count; i++)
            {
                isEmpty = true;
                for (int j = 0; j < destChildTable2.Columns.Count; j++)
                {
                    if (string.IsNullOrEmpty(destChildTable2.Rows[i][j].ToString().Trim()) == false)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (isEmpty == true)
                {
                    destChildTable2.Rows.RemoveAt(i);
                    i--;
                }
            }
            //Create order criteria
            StringBuilder sortString = new StringBuilder();

            if (destChildTable2 != null && destChildTable2.Rows.Count > 0)
            {
                for (int c = 0; c < destChildTable2.Columns.Count; c++)
                {
                    if (!string.IsNullOrEmpty(destChildTable2.Rows[0][c].ToString()))
                    {
                        if (OrderByCriteriaASCExists(destChildTable2.Columns[c].ColumnName))
                            sortString.Append("," + " " + destChildTable2.Columns[c].ColumnName + " " + "ASC");
                        else if (OrderByCriteriaDESCExists(destChildTable2.Columns[c].ColumnName))
                            sortString.Append("," + " " + destChildTable2.Columns[c].ColumnName + " " + "DESC");
                    }
                }
            }
            if (sortString.ToString().StartsWith(","))
                //sortString = sortString.ToString().Trim().Substring(1, sortString.Length - 1);
                sortString = sortString.Remove(0, 1);

            destChildTable2.DefaultView.Sort = sortString.ToString();

            //Create a dataview to store the default view
            DataView destDataview = new DataView();
            destDataview = destChildTable2.DefaultView;

            //Maintain the sorting and copying data data from dataview to datatable
            //Clones the structure of table 
            DataTable destChildTable3 = destDataview.Table.Clone();
            int inc = 0;
            string[] colNames = new string[destChildTable3.Columns.Count];
            foreach (DataColumn col in destChildTable3.Columns)
            {
                colNames[inc++] = col.ColumnName;
            }
            IEnumerator Ienum = destDataview.GetEnumerator();

            while (Ienum.MoveNext())
            {
                DataRowView drv = (DataRowView)Ienum.Current;
                DataRow drSorted = destChildTable3.NewRow();
                try
                {
                    foreach (string name in colNames)
                    {
                        drSorted[name] = drv[name];
                    }
                }
                catch (Exception ex)
                {
                    CBVExcel.ErrorLogging(ex.Message);
                }
                destChildTable3.Rows.Add(drSorted);
            }


            //Retrieve childs data as string value with \n
            bool chk = false;
            for (int c = 0; c < destChildTable3.Columns.Count; c++)
            {
                chk = false;
                StringBuilder sb2 = new StringBuilder();
                for (int r = 0; r < destChildTable3.Rows.Count; r++)
                {
                    if (!string.IsNullOrEmpty(destChildTable3.Rows[r][c].ToString()))
                    {
                        sb2.AppendFormat("{0}\n", destChildTable3.Rows[r][c].ToString());
                        chk = true;
                    }
                }
                if (chk)
                    DestRow[c] = sb2.ToString().Trim();
            }


            destChildTable3.Dispose();
            destChildTable2.Dispose();
            return DestRow;
        }

        #endregion

        #region _ Utilty functions _

        public void ConvertIntToStringDataColum(DataTable dtTable)
        {
            foreach (DataColumn dc in dtTable.Columns)
            {
                if (dc.DataType.ToString() == "System.Int64")
                    dtTable.Columns[dc.ColumnName].DataType = System.Type.GetType("System.String");
            }

        }

        private static bool ISBase64Data(string cdxData)
        {
            try
            {
                byte[] cdxDataB = null;
                cdxDataB = Convert.FromBase64String(cdxData);
                ReturnedData data = Global.ConvertUsingCDAX(Global.CDX_MIME_STRING, cdxDataB, Global.EMF_MIME_STRING);
                if (null == data.Data)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public static void AddMissingTables(ref DataSet resultSet, object nWSheet, CBVExcel cbvExcel)
        {
            try
            {
                DataSet dataSet = resultSet.Copy();
                Excel::Worksheet nSheet = (Excel::Worksheet)nWSheet;

                int rowTabHeader = (int)Global.CBVHeaderRow.HeaderTableRow;
                string tableName = string.Empty;

                if (Global.CBVNewColumnIndex == 1)
                    Global.CBVNewColumnIndex = 2;

                Excel.Range cellRange = nSheet.get_Range("A" + rowTabHeader, Global.NumToString(Global.CBVNewColumnIndex) + rowTabHeader);

                object[,] cellRngVal = new object[1, Global.CBVNewColumnIndex];
                cellRngVal = (object[,])cellRange.Value2;

                for (int exCol = 1; exCol <= Global.CBVNewColumnIndex; exCol++)
                {
                    if (!Global.CellDropdownRange.Contains(exCol))
                        continue;

                    tableName = GetDataFromCellRange(cellRngVal, 1, exCol);

                    if (string.IsNullOrEmpty(tableName))
                        continue;

                    foreach (DataTable dtTables in dataSet.Tables)
                    {
                        if (!Global.IsTableExists(tableName, resultSet))
                        {
                            resultSet.Tables.Add(tableName);
                            resultSet.AcceptChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void AddMissingColumns(ref DataSet resultSet, object nWSheet, CBVExcel cbvExcel)
        {
            try
            {
                DataSet dataSet = resultSet.Copy();
                Excel::Worksheet nSheet = (Excel::Worksheet)nWSheet;

                int rowCatHeader = (int)Global.CBVHeaderRow.HeaderCategoryRow;
                int rowTabHeader = (int)Global.CBVHeaderRow.HeaderTableRow;
                int rowColHeader = (int)Global.CBVHeaderRow.HeaderColumnRow;

                string tableName = string.Empty;
                string colName = string.Empty;

                if (Global.CBVNewColumnIndex == 1)
                {
                    Global.CBVNewColumnIndex = 2;
                }

                Excel.Range cellRange = nSheet.get_Range("A" + rowCatHeader, Global.NumToString(Global.CBVNewColumnIndex) + rowColHeader);

                object[,] cellRngVal = new object[3, Global.CBVNewColumnIndex];
                cellRngVal = (object[,])cellRange.Value2;

                for (int exCol = 1; exCol <= Global.CBVNewColumnIndex; exCol++)
                {
                    if (!Global.CellDropdownRange.Contains(exCol))
                    {
                        continue;
                    }

                    tableName = GetDataFromCellRange(cellRngVal, rowTabHeader, exCol);
                    colName = GetDataFromCellRange(cellRngVal, rowColHeader, exCol);

                    if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(colName) || resultSet.Tables[tableName] == null)
                    {
                        continue;
                    }

                    // The table found empty
                    if (resultSet.Tables[tableName].Columns == null || resultSet.Tables[tableName].Columns.Count <= 0)
                    {
                        resultSet.Tables[tableName].Columns.Add(colName, Type.GetType("System.String"));
                        resultSet.AcceptChanges();
                    }
                    else if ((colName.ToUpper().EndsWith(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_MolWeight))) || (colName.ToUpper().EndsWith(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_Formula))))
                    {                       
                        string[] structCol = colName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                        //CSBR - 154574 (If structure column doesn't have any records then the molweight and formula column are not created in the resultset that's why the columns are added here.
                        if (!Global.IsColumnExists(structCol[1], resultSet.Tables[tableName.Trim()]))
                        {
                            resultSet.Tables[tableName].Columns.Add(structCol[1], Type.GetType("System.String"));
                            resultSet.AcceptChanges();

                        }
                    }
                    else if (!Global.IsColumnExists(colName, resultSet.Tables[tableName.Trim()]))
                    {
                        resultSet.Tables[tableName].Columns.Add(colName, Type.GetType("System.String"));
                        resultSet.AcceptChanges();

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Remove the space start and end of the table and column names
        public static void RemoveEmptySpacesFromTableColumn(ref DataSet resultSet)
        {
            foreach (DataTable dataTab in resultSet.Tables)
            {
                dataTab.TableName = dataTab.TableName.Trim();
                foreach (DataColumn dataCol in dataTab.Columns)
                {
                    dataCol.ColumnName = dataCol.ColumnName.Trim();
                }
            }
        }

        public static string GetDataFromCellRange(object[,] cellRngVal, int rowIndex, int colIndex)
        {
            try
            {
                if (cellRngVal[rowIndex, colIndex] != null)
                    return cellRngVal[rowIndex, colIndex].ToString();
                else
                    return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion

        #region "Split Data in Single/Multiple Rows"

        private static DataTable CBVResultSplitCells(DataTable CBVResult)
        {
            CBVResult = CBVSplitTableRows(CBVResult).Copy();

            return CBVResult;
        }

        private static DataTable CBVResultMultipleRows(DataTable CBVResult)
        {
            CBVResult = CBVMultipleTableRows(CBVResult).Copy();
            CBVResult = CBVResultOrderBy(CBVResult).Copy(); // sorting is only applicable on entire result.
            return CBVResult;
        }

        private static DataTable CBVResultSplitCellsMultipleRows(DataTable CBVResult)
        {
            CBVResult = CBVSplitMultipleTableRows(CBVResult).Copy();
            CBVResult = CBVResultOrderBy(CBVResult).Copy(); // sorting is only applicable on entire result.
            return CBVResult;
        }

        private static DataTable CBVResultOrderBy(DataTable CBVResult)
        {
            string orderBYCriteria = OrderBYCriteria(CBVResult.Clone());
            if (!string.IsNullOrEmpty(orderBYCriteria))
            {
                using (DataView dvResult = CBVResult.DefaultView)
                {
                    dvResult.Sort = orderBYCriteria;
                    CBVResult = dvResult.ToTable();
                }
            }
            return CBVResult;
        }

        private static DataTable CBVSplitTableRows(DataTable destTable)
        {
            CBVExcel cbvExcel = new CBVExcel();
            DataTable resultTable = destTable.Clone();
            DataTable resultTemp = destTable.Clone();
            int splitIndex = 0;
            try
            {
                foreach (DataRow dr in destTable.Rows)
                {
                    int maxSplitDataLen = 0;
                    for (int i = 0; i < resultTable.Columns.Count; i++)
                    {
                        splitIndex = i + 1;
                        if (cbvExcel.ISCBVSplitColIndexExists(splitIndex))
                        {
                            string[] arr = dr[i].ToString().Split(Global.SPLITCHAR);

                            if (arr.Length > maxSplitDataLen)
                            {
                                maxSplitDataLen = arr.Length;
                            }
                        }
                    }

                    //Clear data from temp table;
                    resultTemp.Clear();
                    //Add the blank rows in temp table
                    if (maxSplitDataLen > 0)
                    {
                        for (int rows = 0; rows < maxSplitDataLen; rows++)
                        {
                            DataRow drtemp = resultTemp.NewRow();
                            resultTemp.Rows.Add(drtemp);
                        }
                        //Insert splitted data into blank rows
                        for (int col = 0; col < resultTable.Columns.Count; col++)
                        {
                            splitIndex = col + 1;
                            if (cbvExcel.ISCBVSplitColIndexExists(splitIndex))
                            {
                                string[] val = dr[col].ToString().Split(Global.SPLITCHAR);

                                for (int len = 0; len < val.Length; len++)
                                {
                                    resultTemp.Rows[len][col] = val[len];
                                }
                            }
                            else
                            {
                                resultTemp.Rows[0][col] = dr[col];
                            }
                        }
                    }
                    else
                    {
                        resultTemp.ImportRow(dr);
                    }
                    //Merge the temp table data into result table
                    resultTable.Merge(resultTemp);
                }
                return resultTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static DataTable CBVMultipleTableRows(DataTable destTable)
        {
            CBVExcel cbvExcel = new CBVExcel();
            DataTable resultTable = destTable.Clone();
            DataTable resultTemp = destTable.Clone();
            //int maxSplitDataLen = 0;
            int splitIndex = 0;
            try
            {
                foreach (DataRow dr in destTable.Rows)
                {
                    int maxSplitDataLen = 0;
                    for (int i = 0; i < resultTable.Columns.Count; i++)
                    {
                        splitIndex = i + 1;
                        if (cbvExcel.ISCBVMultipleColIndexExists(splitIndex))
                        {
                            string[] arr = dr[i].ToString().Split(Global.SPLITCHAR);

                            if (arr.Length > maxSplitDataLen)
                            {
                                maxSplitDataLen = arr.Length;
                            }
                        }
                    }

                    //Clear data from temp table;
                    resultTemp.Clear();

                    if (maxSplitDataLen > 0)
                    {
                        //Add the blank rows in temp table
                        for (int rows = 0; rows < maxSplitDataLen; rows++)
                        {
                            DataRow drtemp = resultTemp.NewRow();
                            resultTemp.Rows.Add(drtemp);
                        }


                        string[] val = null;
                        string lastStrVal = string.Empty;
                        bool IsStructureColumn = false;

                        for (int col = 0; col < resultTable.Columns.Count; col++)
                        {
                            splitIndex = col + 1;
                            val = null;
                            lastStrVal = string.Empty;
                            // 12.3 - Proper Separator used instead of #
                            IsStructureColumn = resultTable.Columns[col].ColumnName.ToUpper().Contains(Global.COLUMN_SEPARATOR + Global.COESTRUCTURE_INDEXTYPE + Global.COLUMN_SEPARATOR);

                            if (IsStructureColumn)
                            {
                                lastStrVal = dr[col].ToString();
                            }
                            else if (!cbvExcel.ISCBVMultipleColIndexExists(splitIndex))
                            {
                                lastStrVal = dr[col].ToString();
                            }
                            else
                            {
                                val = dr[col].ToString().Split(Global.SPLITCHAR);
                            }

                            if (val != null)
                            {
                                for (int len = 0; len < resultTemp.Rows.Count; len++)
                                {
                                    if (val.Length > len)
                                    {
                                        resultTemp.Rows[len][col] = val[len];
                                        lastStrVal = val[len];
                                    }
                                    else
                                    {
                                        resultTemp.Rows[len][col] = lastStrVal;
                                    }
                                }
                            }
                            else if (val == null)
                            {
                                for (int len = 0; len < resultTemp.Rows.Count; len++)
                                {
                                    resultTemp.Rows[len][col] = lastStrVal;
                                }
                            }
                        }
                    }
                    else
                    {
                        resultTemp.ImportRow(dr);
                    }
                    //Merge the temp table data into result table
                    resultTable.Merge(resultTemp);
                }
                return resultTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static DataTable CBVSplitMultipleTableRows(DataTable destTable)
        {
            CBVExcel cbvExcel = new CBVExcel();
            DataTable resultTable = destTable.Clone();
            DataTable resultTemp = destTable.Clone();
            int splitIndex = 0;
            try
            {
                foreach (DataRow dr in destTable.Rows)
                {
                    int maxSplitDataLen = 0;
                    for (int i = 0; i < resultTable.Columns.Count; i++)
                    {
                        splitIndex = i + 1;
                        if ((cbvExcel.ISCBVMultipleColIndexExists(splitIndex)) || (cbvExcel.ISCBVSplitColIndexExists(splitIndex)))
                        {
                            string[] arr = dr[i].ToString().Split(Global.SPLITCHAR);

                            if (arr.Length > maxSplitDataLen)
                            {
                                maxSplitDataLen = arr.Length;
                            }
                        }
                    }

                    //Clear data from temp table;
                    resultTemp.Clear();

                    if (maxSplitDataLen > 0)
                    {
                        //Add the blank rows in temp table
                        for (int rows = 0; rows < maxSplitDataLen; rows++)
                        {
                            DataRow drtemp = resultTemp.NewRow();
                            resultTemp.Rows.Add(drtemp);
                        }
                        string[] val = null;
                        string lastStrVal = string.Empty;
                        bool IsStructureColumn = false;

                        for (int col = 0; col < resultTable.Columns.Count; col++)
                        {
                            splitIndex = col + 1;
                            val = null;
                            lastStrVal = string.Empty;

                            // 12.3 - Proper Separator used instead of #
                            IsStructureColumn = resultTable.Columns[col].ColumnName.ToUpper().Contains(Global.COLUMN_SEPARATOR + Global.COESTRUCTURE_INDEXTYPE + Global.COLUMN_SEPARATOR);

                            if (IsStructureColumn)
                            {
                                lastStrVal = dr[col].ToString();

                            }
                            else if ((!cbvExcel.ISCBVMultipleColIndexExists(splitIndex)) && (!cbvExcel.ISCBVSplitColIndexExists(splitIndex)))
                            {
                                lastStrVal = dr[col].ToString();
                            }
                            else
                            {
                                val = dr[col].ToString().Split(Global.SPLITCHAR);
                            }

                            if (val != null)
                            {
                                for (int len = 0; len < resultTemp.Rows.Count; len++)
                                {
                                    if (val.Length > len)
                                    {
                                        resultTemp.Rows[len][col] = val[len];
                                        lastStrVal = val[len];
                                    }
                                    else
                                    {
                                        resultTemp.Rows[len][col] = lastStrVal;
                                    }
                                }
                            }
                            else if (val == null)
                            {
                                for (int len = 0; len < resultTemp.Rows.Count; len++)
                                {
                                    resultTemp.Rows[len][col] = lastStrVal;
                                }
                            }
                        }
                    }
                    else
                    {
                        resultTemp.ImportRow(dr);
                    }
                    //Merge the temp table data into result table
                    resultTable.Merge(resultTemp);
                }
                return resultTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string OrderBYCriteria(DataTable dtResultSchema)
        {
            CBVExcel cbvExcel = new CBVExcel();
            StringBuilder sbOrderBY = new StringBuilder();
            string orderBY = string.Empty;
            try
            {
                for (int col = 0; col < dtResultSchema.Columns.Count; col++)
                {
                    orderBY = cbvExcel.CBVColOrderBy(col + 1);
                    if (!string.IsNullOrEmpty(orderBY))
                    {

                        sbOrderBY.Append("[" + dtResultSchema.Columns[col].ColumnName + "]" + " " + orderBY + ",");
                    }
                }
                if (orderBY.Length > 1)
                {
                    return sbOrderBY.ToString().Trim().Substring(0, sbOrderBY.Length - 1);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static bool OrderByCriteriaExists(string value)
        {
            if ((value.Contains(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_1cellAZ))) || (value.Contains(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_SplitcellsAZ))) || (value.Contains(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_MultiplerowsAZ))) || (value.Contains(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_1cellZA))) || (value.Contains(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_SplitcellsZA))) || (value.Contains(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_MultiplerowsZA))))
                return true;
            else
                return false;
        }

        private static bool OrderByCriteriaASCExists(string value)
        {
            if ((value.Contains(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_1cellAZ))) || (value.Contains(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_SplitcellsAZ))) || (value.Contains(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_MultiplerowsAZ))))
                return true;
            else
                return false;
        }
        private static bool OrderByCriteriaDESCExists(string value)
        {
            if ((value.Contains(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_1cellZA))) || (value.Contains(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_SplitcellsZA))) || (value.Contains(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_MultiplerowsZA))))
                return true;
            else
                return false;
        }


        #endregion

        #region "Restore Chemoffice/CBV Result Sheet Data"

        public static void RestoreCBVResult(object sheet)
        {
            try
            {
                Excel::Worksheet nSheet = (sheet) as Excel.Worksheet;
                //Coverity fix - CID 18721
                if (nSheet == null)
                    throw new System.NullReferenceException();
                if (sheet != null)
                {
                    CBVExcel cbvExcel = new CBVExcel();
                    Global._CBVSearchResult.Clear();

                    int startUPRow = (int)Global.CBVHeaderRow.HeaderResultStartupRow;
                    int labelDispIndex = (int)Global.CBVHeaderRow.HeaderColumnLabelDisplayRow;
                    int noRows = labelDispIndex + Global.MaxRecordInResultSet;

                    Excel.Range cellRange = nSheet.get_Range("A" + startUPRow, Global.NumToString(Global._CBVSearchResult.Columns.Count) + noRows);
                    if (cellRange == null)
                        throw new System.NullReferenceException();
                    object[,] cellRngVal = new object[Global._CBVSearchResult.Rows.Count, Global._CBVSearchResult.Columns.Count];

                    cellRngVal = (object[,])cellRange.Value2;


                    List<int> structColLst = new List<int>();
                    for (int col = 0; col < Global._CBVSearchResult.Columns.Count; col++)
                    {
                        if (Global.ISStructureContains(Global._CBVSearchResult.Columns[col].ColumnName))
                            structColLst.Add(col);
                    }

                    //for (int row = startUPRow; row <= startUPRow + Global.MaxRecordInResultSet - 1; row++)
                    for (int row = 0; row < Global.MaxRecordInResultSet; row++)
                    {
                        DataRow drNew = Global._CBVSearchResult.NewRow();

                        for (int col = 0; col < Global._CBVSearchResult.Columns.Count; col++)
                        {
                            if (Global._CBVSearchResult.Columns[col].ColumnName.ToUpper().Contains("#NONE#EMPTY#NONE#"))
                                continue;

                            if (structColLst.Contains(col))
                                drNew[col] = GlobalCBVExcel.GetCellR1C1(row + 7, col + 1, sheet);
                            else
                            {
                                if (cellRngVal[row + 1, col + 1] == null)
                                    drNew[col] = string.Empty;
                                else
                                    drNew[col] = cellRngVal[row + 1, col + 1].ToString();
                            }
                        }
                        Global._CBVSearchResult.Rows.Add(drNew);
                    }
                }
                Global._CBVSearchResult.AcceptChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region "Convert DataResult to DataSet"
        public static DataSet DataResultToDataSet(DataResult dsResult)
        {
            DataSet resultDataSet = new DataSet();

            using (System.IO.StringReader stringReader = new System.IO.StringReader(dsResult.ResultSet))
            {
                resultDataSet.ReadXml(stringReader);
            }
            return resultDataSet;
            
        }
        #endregion

        #region "Convert DataResult to String"
        public static StringBuilder DataResultToStringBuilder(DataResult dsResult, string colAlias, string tabAlias)
        {
            DataSet resultDataSet = new DataSet();
            StringBuilder data=null;
            using (System.IO.StringReader stringReader = new System.IO.StringReader(dsResult.ResultSet))
            {
                resultDataSet.ReadXml(stringReader);
            }

            try
            {
                foreach (DataTable dt in resultDataSet.Tables)
                {
                    if (dt.TableName.Equals(tabAlias, StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (DataColumn dc in resultDataSet.Tables[dt.TableName].Columns)
                        {
                            if (dc.ColumnName.Equals(colAlias, StringComparison.OrdinalIgnoreCase))
                            {                               
                                data = new StringBuilder(resultDataSet.Tables[dt.TableName].Rows[0][dc].ToString());
                                return data;

                            }
                        }
                    }
                }
            }
            catch (DataException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return data;
            
        }

        public static byte[] DataResultToStringBuilder_Test(DataResult dsResult, string colAlias, string tabAlias)
        {
            DataSet resultDataSet = new DataSet();
            StringBuilder data = null;
            using (System.IO.StringReader stringReader = new System.IO.StringReader(dsResult.ResultSet))
            {
                resultDataSet.ReadXml(stringReader);
            }

            try
            {
                foreach (DataTable dt in resultDataSet.Tables)
                {
                    if (dt.TableName.Equals(tabAlias, StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (DataColumn dc in resultDataSet.Tables[dt.TableName].Columns)
                        {
                            if (dc.ColumnName.Equals(colAlias, StringComparison.OrdinalIgnoreCase))
                            {
                                //data = new StringBuilder(resultDataSet.Tables[dt.TableName].Rows[0][dc].ToString());
                                //return data;

                              //  return (byte[])resultDataSet.Tables[dt.TableName].Rows[0][dc];
                               return System.Text.Encoding.UTF8.GetBytes(resultDataSet.Tables[dt.TableName].Rows[0][dc].ToString());
                            }
                        }
                    }
                }
            }
            catch (DataException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;

        }

        #endregion "Convert DataResult to String"


        #region "_ Modifiy GlobalDataSet Schema _"

        public static void ModifyColumnMimeType(DataTable globalCBVResult, string mimeType, int columnIndex)
        {
            string newName = string.Empty;

            // 12.3 - Proper Separator used instead of #
            string[] splitColName = globalCBVResult.Columns[columnIndex].ColumnName.Split(new string[] { Global.COLUMN_SEPARATOR }, StringSplitOptions.None);

            for (int len = 0; len < splitColName.Length; len++)
            {
                //Index fourth is the mime type
                if (len == 4)
                    newName += mimeType.ToUpper();
                else
                    newName += splitColName[len].ToString();

                // 12.3 - Proper Separator used instead of #
                newName += Global.COLUMN_SEPARATOR;
            }
            globalCBVResult.Columns[columnIndex].ColumnName = newName;
            Global._CBVSearchResult = globalCBVResult.Copy();
        }
        public static void ModifyColumnIndexType(DataTable globalCBVResult, string indexType, int columnIndex)
        {
            string newName = string.Empty;

            // 12.3 - Proper Separator used instead of #
            string[] splitColName = globalCBVResult.Columns[columnIndex].ColumnName.Split(new string[] { Global.COLUMN_SEPARATOR }, StringSplitOptions.None);


            for (int len = 0; len < splitColName.Length; len++)
            {
                //Index second is the index type
                if (len == 1)
                    newName += indexType.ToUpper();
                else
                    newName += splitColName[len].ToString();

                // 12.3 - Proper Separator used instead of #
                newName += Global.COLUMN_SEPARATOR;
            }
            globalCBVResult.Columns[columnIndex].ColumnName = newName;
            Global._CBVSearchResult = globalCBVResult.Copy();

        }

        #endregion "_ Modifiy GlobalDataSet Schema _"
    }

    #region _ Get valid Field List _

    internal class CBVFieldInfo
    {
        private string relationName;
        private string fieldName;
        private string fieldAlias;

        public string RelationName
        {
            get { return relationName; }
            set { relationName = value; }

        }
        public string FieldName
        {
            get { return fieldName; }
            set { fieldName = value; }

        }
        public string FieldAlias
        {
            get { return fieldAlias; }
            set { fieldAlias = value; }
        }
    }
    #endregion
}
   
