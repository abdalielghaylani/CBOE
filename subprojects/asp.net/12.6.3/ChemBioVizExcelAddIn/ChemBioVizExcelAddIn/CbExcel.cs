using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;
using System.Xml;
using System.Drawing;
using System.IO;
using System.Collections;
using Office = Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COEExportService;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using System.Threading;
using System.Net;

namespace ChemBioVizExcelAddIn
{
    #region _  CBVExcel class  _

    class CBVExcel
    {
        # region _ Variables _

        Exception excepMainThread = null;

        # endregion

        # region _ Accessor Properties _

        # endregion

        #region "_ Read/Write Excel Range _"

        public static int rowCount = 0;
        public static int colCount = 0;

        //Process column wise
        public void SetCell(int startRow, int startCol, object sheet, DataTable datatable, bool isAllClear, bool isCurrResClear)
        {
            int row, col;
            row = col = 0;
            //Structure max height and width. Validates only with structure columns.
            Int64 structureMaxHeight = StructureSearchOption.StructureMaxHeight;
            Int64 structureMaxWidth = StructureSearchOption.StructureMaxWidth;

            try
            {
                Global._ExcelApp.ScreenUpdating = false;
                Excel::Worksheet nSheet = sheet as Excel.Worksheet;
                Excel::Range nRange;
                int totalRow = datatable.Rows.Count;

                if (isAllClear && Global.MaxRecordInResultSet > 0)
                {
                    ClearResults(nSheet);
                    CleanCBVResultTable();
                }
                else if (isCurrResClear && Global.MaxRecordInResultSet > 0)
                {
                    ClearResults(nSheet);
                }


                bool IsStructureColumn = false;
                bool IsImageType = false;



                // 11.0.4 - Check if total records are less than max structure hit or not
                bool showStructure = Global.IsRecordsLessThanMaxStructureHits(datatable.Rows.Count);

                // 11.0.4 - Object array for the values of the column to be displayed in struct column 
                object[,] cellRngValcolToDisplayInStruct = new object[datatable.Rows.Count, 1];

                // 11.0.4 - Getting the values of the column to be displayed in the structure cell, if structure is not to be displayed
                if (!showStructure)
                {
                    cellRngValcolToDisplayInStruct = GetColValToBeDisplayInStruct(datatable);
                }

                Global._ExcelApp.EnableEvents = false;
                for (int j = 0; j < datatable.Columns.Count; j++)
                {
                    colCount = j;

                    if (datatable.Columns[j].ColumnName.ToUpper().Contains("#NONE#EMPTY#NONE#") || Global.ISContentTypeContains(datatable.Columns[j].ColumnName.ToString()))
                    {
                        //11.0.3

                        SetCellMimeType(startRow, startCol, j, datatable, nSheet);
                        continue;
                    }

                    string excelRange = string.Format("{0}{1}{2}", Global.NumToString(j + 1) + startRow, ":", Global.NumToString(j + 1) + ((startRow - 1) + datatable.Rows.Count));

                    object[,] cellRngVal = new object[datatable.Rows.Count, 1];


                    bool IsOptionHeader = false;

                    bool IsStructImage = false;

                    bool IsImageShown = false;

                    IsStructureColumn = Global.ISStructureContains(datatable.Columns[j].ColumnName.ToString());
                    IsImageType = Global.ISImageTypeContains(datatable.Columns[j].ColumnName.ToString());


                    Global._ExcelApp.ScreenUpdating = false;
                    for (int i = 0; i < datatable.Rows.Count; i++)
                    {
                        rowCount = i;

                        if (!string.IsNullOrEmpty(datatable.Rows[i][j].ToString()))
                        {
                            row = i + startRow;
                            col = j + startCol;

                            //This is slow.  Additionally we are not really using it for regular data (only structures and images)
                            //There is an autofit call for an entire column each time through the loop.   Seems to me it only needs to be
                            //done once at the end per column.  Would be good to get rid of this, but lets limit it 
                            //to structures and images and see what happens
                            //OK it doesn't like the code below so we can move range creation to the 2 if cases
                            //if (IsStructureColumn || IsImageType)
                            //{
                            //    nRange = nSheet.Cells[row, col] as Excel::Range;
                            //}

                            CriteriaUtilities criteriaUtilities = new CriteriaUtilities();
                            //if the columns have "Structures"
                            //this implementation has it being checked for each row.....it is a result of the flip...now it can just be done once for the column
                            //if (datatable.Columns[j].ColumnName.ToUpper().Contains("#" + Global.COESTRUCTURE_INDEXTYPE + "#"))
                            if (IsStructureColumn)
                            {
                                //nRange has teh same definition as cell further down.
                                //nRange = nSheet.Cells[row, col] as Excel::Range;



                                //If structure column have molecular weight and formula.
                                //I don't think this is being used and ranges are slow. (9 seconds for 3K records)
                                //Excel.Range columnRng = ((Excel.Range)nSheet.Cells[Global.CBVHeaderRow.HeaderColumnRow, col]);

                                using (GraphicUtility graphicUtil = new GraphicUtility())
                                {
                                    Excel.Range cell = null;
                                    object[] arrStruct = new object[4];
                                    try
                                    {
                                        arrStruct[0] = nSheet.Name;
                                        arrStruct[1] = row;
                                        arrStruct[2] = col;
                                        object b64 = datatable.Rows[i][j];
                                        arrStruct[3] = b64;

                                        //Wish we didn't need this but it seems needed for height width below
                                        cell = nSheet.Cells[row, col] as Excel.Range;


                                        // 11.0.4 If total records are less than the max structure hits than show structure else add structure data as cell comment
                                        if (showStructure)
                                        {
                                            Global.AddStructrue(arrStruct);
                                        }
                                        else
                                        {
                                            // To add the structure content as a cell comment. 
                                            Global.MakeCDXLComment(cell, datatable.Rows[i][j].ToString());

                                        }

                                        //This check takes 4 seconds for 3K structures.  Further it's purpose is to 
                                        //work past a bad installation.  I would rather know we have a bad installation
                                        //We should figure out a way to check for this on startup or something and be done with it.
                                        //if (cell.Comment == null)
                                        //{
                                        //    graphicUtil.AddStructure(nSheet, datatable.Rows[i][j].ToString(), cell); //If the chemdrawexcel retrun null
                                        //}
                                        // Set the default column width as Maxcolumn width at begning. Validation call (SetStructureColumnWidth) in below existing method
                                        graphicUtil.SetStructureHeightWidth(nSheet, cell, structureMaxHeight, structureMaxWidth);
                                        if (i + 1 == totalRow)
                                            Global.MaxColumnWidth = 0;// Reset the column width ( for strucuture coloumn)
                                    }

                                    catch (Exception Ex) //If the chemdrawexcel update doesn't exists
                                    {

                                        /*graphicUtil.AddStructure(nSheet, datatable.Rows[i][j].ToString(), nRange);
                                        graphicUtil.SetStructureHeightWidth(nSheet, cell, structureMaxHeight, structureMaxWidth);
                                        if (i + 1 == totalRow)
                                            Global.MaxColumnWidth = 0;// Reset the column width (for strucuture coloumn)*/
                                        CBVExcel.ErrorLogging(Ex.Message);
                                    }
                                    finally
                                    {
                                        IsStructImage = true;
                                    }
                                }
                            }

                            else if (IsImageType)
                            {
                                nRange = nSheet.Cells[row, col] as Excel::Range;

                                //JHS 3/22/2011 - This is slow.  It also seems like it only changes once per column.  Seems like it can
                                //be set once for that columns and then reused over and over.
                                Excel.Range columnOptionVal = ((Excel.Range)nSheet.Cells[Global.CBVHeaderRow.HeaderOptionRow, col]);

                                using (GraphicUtility graphicUtil = new GraphicUtility())
                                {
                                    graphicUtil.AddImage(nSheet, datatable.Rows[i][j], nRange, columnOptionVal.Value2.ToString());
                                    // Using another variable for Image Type
                                    //IsStructImage = true;
                                    IsImageShown = true;
                                }
                            }

                            else
                            {
                                // cellRngVal[i, 0] = datatable.Rows[i][j].ToString();

                                //we are not using nRange and I think we can do this just once per column at the end
                                //if (Global.ToggleColumnAutoSize == 1)
                                //    nRange.EntireColumn.AutoFit();

                                //Bug fix: CSBR-139938
                                // Getting the comma separated columnname values
                                string optionHeader = string.Empty;
                                // 12.3 - Proper Separator used instead of #
                                string[] lstrColumnValues = datatable.Columns[j].ColumnName.Split(new string[] { Global.COLUMN_SEPARATOR }, StringSplitOptions.None);

                                // Getting the optionHeader value from the column values
                                if (lstrColumnValues.Length > 4)
                                {
                                    optionHeader = lstrColumnValues[3];
                                }

                                // If option Header is empty or All (1 cell) then show the value as it is else call the "OptionCalculation" method to get the value as per the aggragate function selected
                                // Added condition for "MOLWEIGHT" and "FORMULA"
                                //if (string.IsNullOrEmpty(optionHeader) || optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_1cell), StringComparison.OrdinalIgnoreCase))
                                if (string.IsNullOrEmpty(optionHeader) || optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_1cell), StringComparison.OrdinalIgnoreCase)) // || datatable.Columns[j].ColumnName.Contains("#MOLWEIGHT#") || datatable.Columns[j].ColumnName.Contains("#FORMULA#"))
                                {                                   
                                    string st = datatable.Rows[i][j].ToString(); // Fixed CSBR-168045
                                    if (st.Contains("<"))
                                    {                                        
                                        XmlDocument doc = new XmlDocument();
                                        doc.LoadXml(st);
                                        string s = doc.DocumentElement.InnerText;
                                        cellRngVal[i, 0] = s.ToString();
                                    }
                                    else
                                    {
                                        cellRngVal[i, 0] = datatable.Rows[i][j].ToString();
                                    }
                                }
                                else
                                {
                                    List<string> lstrValues = new List<string>();

                                    lstrValues.AddRange(datatable.Rows[i][j].ToString().Split('\n'));

                                    cellRngVal[i, 0] = DisplayOption.OptionCalculation(lstrValues, optionHeader);

                                    IsOptionHeader = true;
                                }
                            }
                        }
                        //I think this is where it should go...have to identify the column
                        //if (Global.ToggleColumnAutoSize == 1)
                        //    nRange.EntireColumn.AutoFit();

                    } //End for loop



                    // 11.0.3
                    try
                    {
                        // Setting the value only if structure and image is not shown in the cell
                        //if (!IsStructImage)
                        if (!IsStructImage && !IsImageShown)
                        {
                            nSheet.get_Range(excelRange, Type.Missing).Value2 = cellRngVal;

                            // Auto fit the entire column instead of setting it cell by cell
                            nSheet.get_Range(excelRange, Type.Missing).EntireColumn.AutoFit();
                            //Bug fix: CSBR-139938
                            if (IsOptionHeader)
                            {
                                nSheet.get_Range(excelRange, Type.Missing).NumberFormat = "@";
                            }

                        }
                        else if (IsStructImage && !showStructure)
                        {

                            // 11.0.4 - If the column values doesn't exists then set "Structure" in the cell range
                            if (!string.IsNullOrEmpty(Convert.ToString(cellRngValcolToDisplayInStruct.GetValue(0, 0))))
                            {
                                nSheet.get_Range(excelRange, Type.Missing).Value2 = cellRngValcolToDisplayInStruct;
                            }
                            else
                            {
                                nSheet.get_Range(excelRange, Type.Missing).Value2 = Global.STRUCTURE;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        CBVExcel.ErrorLogging(ex.Message);
                    }
                    Application.DoEvents();
                    Global._ExcelApp.ScreenUpdating = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Global._ExcelApp.EnableEvents = true;
                Global._ExcelApp.ScreenUpdating = true;
            }
        }


        public void SetCellMimeType(int startRow, int startCol, int dtColumn, DataTable datatable, object sheet)
        {
            Excel::Worksheet nSheet = sheet as Excel.Worksheet;
            int row, col;
            //Coverity fix - CID 18719
            if (nSheet == null)
                throw new System.NullReferenceException();
            Excel.Range cellRange = nSheet.get_Range(Global.NumToString(dtColumn + 1) + (int)Global.CBVHeaderRow.HeaderCategoryRow, Global.NumToString(dtColumn + 1) + (int)Global.CBVHeaderRow.HeaderColumnRow);
            //Coverity fix - CID 18719
            if (cellRange == null)
                throw new System.NullReferenceException();
            Excel.Range nRange;
            object[,] cellRngHdr = new object[3, 1];
            cellRngHdr = (object[,])cellRange.Value2;

            string catAlias = Convert.ToString(cellRngHdr[1, 1]);
            string tabAlias = Convert.ToString(cellRngHdr[2, 1]);
            string colAlias = Convert.ToString(cellRngHdr[3, 1]);
            if (string.IsNullOrEmpty(catAlias) || string.IsNullOrEmpty(tabAlias) || string.IsNullOrEmpty(colAlias))
                return;
            string mimeType = COEDataViewData.GetMimeType(colAlias, tabAlias);
            if (mimeType == "NONE")
            {
                return;
            }
            else if (Global.ISMimeTypeExists(mimeType))
            {
                for (int i = 0; i < datatable.Rows.Count; i++)
                {
                    rowCount = i;

                    row = i + startRow;
                    col = dtColumn + startCol;

                    nRange = nSheet.Cells[row, col] as Excel::Range;

                    Excel.Shape shape = Global.GetContentTypeFileShape(mimeType, nRange, nSheet);
                    if (shape != null)
                    {
                        // 11.0.3
                        //nRange.Hyperlinks.Add(shape, mimeType, Type.Missing, Type.Missing, Type.Missing);
                        nRange.Hyperlinks.Add(shape, string.Empty, Type.Missing, Type.Missing, Type.Missing);

                    }

                }
            }

        }

        public void SetCellOptionHeaderResult(object sheet, int startRow, int activeCol, DataTable CBVResult, string optionHeader)
        {
            int row = 0;
            Excel::Worksheet nWorksheet = sheet as Excel.Worksheet;
            Excel::Range nRange;
            string resultCol = string.Empty;

            try
            {
                //Coverity fix - CID 18720
                if (nWorksheet == null)
                    throw new System.NullReferenceException();
                foreach (DataColumn dtCol in CBVResult.Columns)
                {
                    if (dtCol.ColumnName.EndsWith(activeCol.ToString()))
                    {
                        resultCol = dtCol.ColumnName;
                        break;
                    }
                } //end for each loop

                if (string.Empty != resultCol)
                {
                    for (int i = 0; i < CBVResult.Rows.Count; i++)
                    {
                        row = i + startRow;
                        nRange = nWorksheet.Cells[row, activeCol] as Excel::Range;
                        //Coverity fix - CID 18720
                        if (nRange == null)
                            throw new System.NullReferenceException();
                        //check if the columns Structure
                        CriteriaUtilities criteriaUtilities = new CriteriaUtilities();
                        nRange.Clear();

                        List<string> lstVar = Global.StringToArrayList(CBVResult.Rows[i][resultCol].ToString().Trim());
                        try
                        {
                            nRange.Value2 = DisplayOption.OptionCalculation(lstVar, optionHeader);
                            nRange.NumberFormat = "@";
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                } //end if
            }//end if            
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region _ Create CBV Sheet _
        //11.0.3
        public void CreateCBVSheet(object sheet)
        {
            CriteriaUtilities criteriaUtilities = new CriteriaUtilities();
            Excel.Worksheet nSheet = sheet as Excel.Worksheet;
            Excel.Range nRange = null;
            string headerColumnDatatype = string.Empty;
            string headerColumnIndextype = string.Empty;
            string headerColumnMimetype = string.Empty;

            Global.IsCDXLWorkSheet = Global.IsCDExcelWorksheet(nSheet);

            if (Global.IsCDXLWorkSheet)
            {
                //Verify the current sheet in edit mode
                GlobalCBVExcel.ISSheetInEditMode();
                Global.IsCBVXLWorkSheet = Global.IsCBVExcelWorksheet(nSheet);
                if (!Global.IsCBVXLWorkSheet)
                {
                    try
                    {
                        Global.DataView = Global.CBVSHEET_COEDATAVIEWBO.COEDataView;
                        Global.Tables = TableListBO.NewTableListBO(Global.DataView.Tables);

                        TableBO baseTable = Global.Tables.GetTable(Global.DataView.Basetable);

                        if (baseTable == null)
                            throw new Exception(Properties.Resources.msgBaseTableNull);
                        int colIndex = 0;

                        try
                        {
                            foreach (TreeNode tndb in Global.TreeDataView.Nodes)
                            {
                                if (tndb.Checked)
                                {
                                    foreach (TreeNode tnTab in tndb.Nodes)
                                    {
                                        if (tnTab.Checked)
                                        {
                                            foreach (TreeNode tnCol in tnTab.Nodes)
                                            {
                                                if (tnCol.Checked)
                                                {
                                                    colIndex++;
                                                    string catName = tndb.Text;
                                                    string tableAlias = tnTab.Text;
                                                    string colAlias = tnCol.Text;
                                                    //Coverity fix - CID 19225
                                                    if (nSheet == null)
                                                        throw new System.NullReferenceException();
                                                    //category
                                                    nSheet.Cells[Global.CBVHeaderRow.HeaderCategoryRow, colIndex] = catName;
                                                    //table
                                                    nSheet.Cells[Global.CBVHeaderRow.HeaderTableRow, colIndex] = tableAlias;
                                                    //column
                                                    nSheet.Cells[Global.CBVHeaderRow.HeaderColumnRow, colIndex] = colAlias;

                                                    headerColumnDatatype = criteriaUtilities.GetHeaderColumnInfo(catName, tableAlias, colAlias, Global.DATATYPE);
                                                    headerColumnIndextype = criteriaUtilities.GetHeaderColumnInfo(catName, tableAlias, colAlias, Global.INDEXTYPE);
                                                    headerColumnMimetype = criteriaUtilities.GetHeaderColumnInfo(catName, tableAlias, colAlias, Global.MIMITYPE);


                                                    //Option header
                                                    nSheet.Cells[Global.CBVHeaderRow.HeaderOptionRow, colIndex] = OptionListFistValue(headerColumnDatatype, headerColumnIndextype, headerColumnMimetype);
                                                    //Label Header
                                                    nSheet.Cells[Global.CBVHeaderRow.HeaderColumnLabelDisplayRow, colIndex] = colAlias;

                                                    //tell the options to do something special for molweight or formula
                                                    //this is ugly but should be safe
                                                    string mimeOverrideString = GetMolWeightOrFormulaString(sheet, colIndex);
                                                    if (mimeOverrideString != null && mimeOverrideString != String.Empty)
                                                    {
                                                        headerColumnMimetype = mimeOverrideString;
                                                        nSheet.Cells[Global.CBVHeaderRow.HeaderOptionRow, colIndex] = OptionListFistValue(headerColumnDatatype, headerColumnIndextype, headerColumnMimetype);
                                                    }

                                                    UpdateCellDropdownRange(new int[] { colIndex });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            Global.CBVActiveColumnIndex = colIndex;
                            Global.CBVNewColumnIndex = colIndex + 1;
                            //Insert the column 1 and 2 in cell dropdown list so that the dropdown will populate on selection of default populated columns
                            //UpdateCellDropdownRange(new int[] { 1, 2 });
                        }
                        finally
                        {
                            Global.TreeDataView.Dispose();
                        }

                        //Range used for freeze pane
                        if (Global.CBVNewColumnIndex > 2)
                            nRange = ((Excel.Range)nSheet.Cells[Global.CBVHeaderRow.HeaderResultStartupRow, 3]);
                        else
                        {
                            if (nSheet == null)
                                throw new System.NullReferenceException();
                            if(nSheet.Cells[Global.CBVHeaderRow.HeaderResultStartupRow, 2]!=null)
                                nRange = ((Excel.Range)nSheet.Cells[Global.CBVHeaderRow.HeaderResultStartupRow, 2]);
                        }
                        if (nRange == null)
                            throw new System.NullReferenceException();
                        nRange.Columns.AutoFit();
                        nRange.Select();
                        Global._ExcelApp.ActiveWindow.FreezePanes = true;

                        Global.InsertColumn = true;
                        GlobalCBVExcel.FormatSheet(nSheet);

                        nSheet.CustomProperties.Add(Global.CBVSHEET_PROP_DOCUMENT, "CS");
                        //Insert data into hidden sheet;
                        SaveDataView(nSheet, Global.CBVSHEET_COEDATAVIEWBO);
                        //11.0.3
                        UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.Display, Global.CurrentWorkSheetName, "YES"); // Update the hidden sheet (Column 9 for displaying the result in chemoffice sheet).
                        GroupingHeaderSection(nSheet); // Grroping the header column
                        //nSheet.Cells.NumberFormat = "@";

                        //Set the worksheet change status
                        //11.0.3
                        UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.ModifiedUser, Global.CurrentWorkSheetName, string.Empty);
                        //Set the default is false for autocolumnsizing
                        // UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.ToggleColumnAutosizing, "0"); // Update the hidden sheet (Column 9 for displaying the result in chemoffice sheet).
                        //11.0.3
                        UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.Server, Global.CurrentWorkSheetName, Global.MRUServer);
                        //11.0.3
                        UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.Servermode, Global.CurrentWorkSheetName, Global.MRUServerMode);
                        //Whether the column autosize is enable or disable
                        EnableDisableToggleColumnAutoSizing();
                        Global.SetRangeValue();

                        //Set the focus range
                        SetFocusRange(nSheet, nRange);
                        Global.WorkSheetChange = false;
                    }
                    catch (Exception ex)
                    {
                        GlobalCBVExcel.RemoveWorksheet(Global._ExcelApp.ActiveWorkbook.Worksheets, nSheet.Name);
                        throw new Exception("Dataview Error!\n" + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.msgNChemSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Advance Export - Creating the data fields retrieved from export sheet
        public void CreateCBVSheetAdvanceExport(object sheet, string resultFieldValues)
        {
            CriteriaUtilities criteriaUtilities = new CriteriaUtilities();
            Excel.Worksheet nSheet = sheet as Excel.Worksheet;
            //Coverity fix - CID 18713
            if (nSheet == null)
                throw new System.NullReferenceException();
            Excel.Range nRange = null;
            string headerColumnDatatype = string.Empty;
            string headerColumnIndextype = string.Empty;
            string headerColumnMimetype = string.Empty;

            Global.IsCDXLWorkSheet = Global.IsCDExcelWorksheet(nSheet);

            if (Global.IsCDXLWorkSheet)
            {
                //Verify the current sheet in edit mode
                GlobalCBVExcel.ISSheetInEditMode();
                Global.IsCBVXLWorkSheet = Global.IsCBVExcelWorksheet(nSheet);
                if (!Global.IsCBVXLWorkSheet)
                {
                    try
                    {
                        //Fixed CSBR - 152134
                        resultFieldValues=System.Web.HttpUtility.HtmlDecode(resultFieldValues);                        
                        string[] resultFields = resultFieldValues.Split(COEFormInterchange.columnSeparator, StringSplitOptions.None);
                        
                        Global.DataView = Global.CBVSHEET_COEDATAVIEWBO.COEDataView;
                        Global.Tables = TableListBO.NewTableListBO(Global.DataView.Tables);

                        TableBO baseTable = Global.Tables.GetTable(Global.DataView.Basetable);

                        if (baseTable == null)
                            throw new Exception(Properties.Resources.msgBaseTableNull);
                        int colIndex = 0;

                        string[] fieldDelimiter = new string[] { COEFormInterchange.fieldDelimiter };

                        foreach (string fieldDetails in resultFields)
                        {
                            colIndex++;
                            string[] fieldValue = fieldDetails.Split(fieldDelimiter, StringSplitOptions.None);

                            string catName = string.Empty;
                            string tableAlias = string.Empty;
                            string colAlias = string.Empty;

                            if (fieldValue.Length >= 3)
                            {
                                catName = fieldValue[0];
                                tableAlias = fieldValue[1];
                                colAlias = fieldValue[2];

                                if (fieldValue.Length == 4)
                                {
                                    colAlias += "." + fieldValue[3];
                                }
                            }


                            //category
                            nSheet.Cells[Global.CBVHeaderRow.HeaderCategoryRow, colIndex] = catName;
                            //table
                            nSheet.Cells[Global.CBVHeaderRow.HeaderTableRow, colIndex] = tableAlias;
                            //column
                            nSheet.Cells[Global.CBVHeaderRow.HeaderColumnRow, colIndex] = colAlias;

                            headerColumnDatatype = criteriaUtilities.GetHeaderColumnInfo(catName, tableAlias, colAlias, Global.DATATYPE);
                            headerColumnIndextype = criteriaUtilities.GetHeaderColumnInfo(catName, tableAlias, colAlias, Global.INDEXTYPE);
                            headerColumnMimetype = criteriaUtilities.GetHeaderColumnInfo(catName, tableAlias, colAlias, Global.MIMITYPE);

                            //tell the options to do something special for molweight or formula
                            string mimeOverrideString = GetMolWeightOrFormulaString(sheet, colIndex);
                            if (mimeOverrideString != null && mimeOverrideString != String.Empty)
                            {
                                headerColumnMimetype = mimeOverrideString;
                            }

                            //Option header
                            nSheet.Cells[Global.CBVHeaderRow.HeaderOptionRow, colIndex] = OptionListFistValue(headerColumnDatatype, headerColumnIndextype, headerColumnMimetype);
                            //Label Header
                            nSheet.Cells[Global.CBVHeaderRow.HeaderColumnLabelDisplayRow, colIndex] = colAlias;

                            UpdateCellDropdownRange(new int[] { colIndex });
                        }

                        Global.CBVActiveColumnIndex = colIndex;
                        Global.CBVNewColumnIndex = colIndex + 1;

                        //Range used for freeze pane
                        if (Global.CBVNewColumnIndex > 2)
                            nRange = ((Excel.Range)nSheet.Cells[Global.CBVHeaderRow.HeaderResultStartupRow, 3]);
                        else
                            nRange = ((Excel.Range)nSheet.Cells[Global.CBVHeaderRow.HeaderResultStartupRow, 2]);

                        nRange.Columns.AutoFit();
                        nRange.Select();
                        Global._ExcelApp.ActiveWindow.FreezePanes = true;

                        Global.InsertColumn = true;
                        GlobalCBVExcel.FormatSheet(nSheet);

                        nSheet.CustomProperties.Add(Global.CBVSHEET_PROP_DOCUMENT, "CS");
                        //Insert data into hidden sheet;
                        SaveDataView(nSheet, Global.CBVSHEET_COEDATAVIEWBO);
                        //11.0.3
                        UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.Display, Global.CurrentWorkSheetName, "YES"); // Update the hidden sheet (Column 9 for displaying the result in chemoffice sheet).
                        GroupingHeaderSection(nSheet); // Grroping the header column
                        //nSheet.Cells.NumberFormat = "@";

                        //Set the worksheet change status
                        //11.0.3
                        UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.ModifiedUser, Global.CurrentWorkSheetName, string.Empty);
                        //Set the default is false for autocolumnsizing
                        // UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.ToggleColumnAutosizing, "0"); // Update the hidden sheet (Column 9 for displaying the result in chemoffice sheet).
                        //11.0.3
                        UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.Server, Global.CurrentWorkSheetName, Global.MRUServer);
                        //11.0.3
                        UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.Servermode, Global.CurrentWorkSheetName, Global.MRUServerMode);
                        //Whether the column autosize is enable or disable
                        EnableDisableToggleColumnAutoSizing();
                        Global.SetRangeValue();

                        //Set the focus range
                        SetFocusRange(nSheet, nRange);
                        Global.WorkSheetChange = false;
                    }
                    catch (Exception ex)
                    {
                        GlobalCBVExcel.RemoveWorksheet(Global._ExcelApp.ActiveWorkbook.Worksheets, nSheet.Name);
                        throw new Exception("Dataview Error!\n" + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.msgNChemSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void RefreshCBVSheet(Excel.Worksheet nSheet)
        {
            try
            {
                Excel.Range nRange = null;
                Global.CBVNewColumnIndex = nSheet.UsedRange.Columns.Count + 1;
                Global.CBVActiveColumnIndex = nSheet.UsedRange.Columns.Count;
                Global.MaxRecordInResultSet = nSheet.UsedRange.Rows.Count - (int)Global.CBVHeaderRow.HeaderWhereRow;
                ReSaveDataView(nSheet, Global.CBVSHEET_COEDATAVIEWBO);
                //Range used for freeze pane
                nRange = ((Excel.Range)nSheet.Cells[Global.CBVHeaderRow.HeaderResultStartupRow, Global.CBVNewColumnIndex]);
                nRange.Columns.AutoFit();
                nRange.Select();
                Global._ExcelApp.ActiveWindow.FreezePanes = true;

                Global.InsertColumn = true;
                //GlobalCBVExcel.FormatSheet(nSheet);

                //11.0.3
                UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.Display, Global.CurrentWorkSheetName, "YES"); // Update the hidden sheet (Column 9 for displaying the result in chemoffice sheet).
                GroupingHeaderSection(nSheet); // Grroping the header column

                //Set the worksheet change status
                //11.0.3
                UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.ModifiedUser, Global.CurrentWorkSheetName, string.Empty);
                UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.Server, Global.CurrentWorkSheetName, Global.MRUServer);
                UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.Servermode, Global.CurrentWorkSheetName, Global.MRUServerMode);
                //Whether the column autosize is enable or disable
                EnableDisableToggleColumnAutoSizing();

                Global.SetRangeValue();
                //Set the focus range
                SetFocusRange(nSheet, nRange);
                Global.WorkSheetChange = false;
            }
            catch (Exception ex)
            {
                throw new Exception("Dataview Error!\n" + ex.Message);
            }
        }

        #endregion

        #region "_ Generate header List _"

        public string GetDropdownList(int row, int col, object sheet)
        {
            string value = string.Empty;
            Excel::Worksheet nSheet = (sheet) as Excel.Worksheet;
            //Coverity fix - CID 18704
            if (nSheet == null)
                throw new System.NullReferenceException("Excel sheet cannot be null");
            if (row == Convert.ToInt32(Global.CBVHeaderRow.HeaderCategoryRow))
            {
                try
                {
                    UpdateheaderCategoryList();
                    foreach (string item in Global.CBVHeaderCategoryList)
                    {
                        value = value + "," + item.Replace(Global.COMMA_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_COMMA);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else if (row == Convert.ToInt32(Global.CBVHeaderRow.HeaderTableRow))
            {
                try
                {
                    UpdateheaderTableList(col, sheet);
                    foreach (string item in Global.CBVHeaderTableList)
                    {
                        value = value + "," + item.Replace(Global.COMMA_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_COMMA);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else if (row == Convert.ToInt32(Global.CBVHeaderRow.HeaderColumnRow))
            {
                UpdateheaderColumnList(col, sheet);

                foreach (string item in Global.CBVHeaderColumnList)
                {
                    value = value + "," + item.Replace(Global.COMMA_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_COMMA);
                }
            }
            else if (row == Convert.ToInt32(Global.CBVHeaderRow.HeaderOptionRow))
            {
                Excel::Range range = (Excel.Range)nSheet.Cells[row, col];
                //Coverity fix - CID 18704
                if (range == null)
                    throw new System.NullReferenceException("Range cannot be null");
                if (range.Text != null)
                {
                    try
                    {
                        //update
                        if (!UpdateheaderOptionList(col, sheet))
                        {
                            return string.Empty;
                        }

                        if (Global.CBVHeaderOptionList.Count != 0)
                        {
                            foreach (string item in Global.CBVHeaderOptionList)
                            {
                                value = value + "," + item.Replace(Global.COMMA_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_COMMA);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// Retrieve table header row and column header row on the basis of input category header
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="sheet"></param>
        public void GetCBVTableColumnHeader(int row, int col, object sheet)
        {
            string value = string.Empty;
            Excel::Worksheet nSheet = (sheet) as Excel.Worksheet;
            Excel::Range nTarget = null;
            try
            {
                if (row == Convert.ToInt32(Global.CBVHeaderRow.HeaderCategoryRow))
                {
                    UpdateheaderTableList(col, sheet);
                    foreach (string item in Global.CBVHeaderTableList)
                    {
                        value = value + "," + item;
                    }
                    //Coverity fix - CID 18717
                    if (nSheet == null)
                        throw new System.NullReferenceException();
                    nTarget = (Excel.Range)nSheet.Cells[Global.CBVHeaderRow.HeaderTableRow, col];
                    //Coverity fix - CID 18717
                    if (nTarget == null)
                        throw new System.NullReferenceException();
                    nSheet.Cells[Global.CBVHeaderRow.HeaderTableRow, col] = Global.CBVHeaderTableList[0];
                    nTarget.Validation.Delete();
                    nTarget.Validation.Add(Excel.XlDVType.xlValidateList, Excel.XlDVAlertStyle.xlValidAlertStop, Excel.XlFormatConditionOperator.xlBetween, value, Type.Missing);
                    nTarget.Validation.InCellDropdown = true;


                    UpdateheaderColumnList(col, sheet);
                    foreach (string item in Global.CBVHeaderColumnList)
                    {
                        value = value + "," + item;
                    }
                    nTarget = (Excel.Range)nSheet.Cells[Global.CBVHeaderRow.HeaderColumnRow, col];
                    nSheet.Cells[Global.CBVHeaderRow.HeaderColumnRow, col] = Global.CBVHeaderColumnList[0];
                    nTarget.Validation.Delete();
                    nTarget.Validation.Add(Excel.XlDVType.xlValidateList, Excel.XlDVAlertStyle.xlValidAlertStop, Excel.XlFormatConditionOperator.xlBetween, value, Type.Missing);
                    nTarget.Validation.InCellDropdown = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieve column header row on the basis of input table header
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="sheet"></param>
        public void GetCBVColumnHeader(int row, int col, object sheet)
        {
            string value = string.Empty;
            Excel::Worksheet nSheet = (sheet) as Excel.Worksheet;
            Excel::Range nTarget = null;
            try
            {
                if (row == Convert.ToInt32(Global.CBVHeaderRow.HeaderTableRow))
                {
                    UpdateheaderColumnList(col, sheet);

                    foreach (string item in Global.CBVHeaderColumnList)
                    {
                        value = value + "," + item;
                    }
                    //Coverity fix - CID 18715
                    if (nSheet == null)
                        throw new System.NullReferenceException("Excel sheet cannot be null");
                    nTarget = (Excel.Range)nSheet.Cells[Global.CBVHeaderRow.HeaderColumnRow, col];
                    nSheet.Cells[Global.CBVHeaderRow.HeaderColumnRow, col] = Global.CBVHeaderColumnList[0];
                    nTarget.Validation.Delete();
                    nTarget.Validation.Add(Excel.XlDVType.xlValidateList, Excel.XlDVAlertStyle.xlValidAlertStop, Excel.XlFormatConditionOperator.xlBetween, value, Type.Missing);
                    nTarget.Validation.InCellDropdown = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Fill CBV Header column row in CBV header column label row (to Uppercase)
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="sheet"></param>
        public void GetCBVColumnLabelHeader(int row, int col, object sheet)
        {
            Excel::Worksheet nSheet = (sheet) as Excel.Worksheet;
            try
            {
                //Coverity fix - CID 18716
                if (nSheet == null)
                    throw new System.NullReferenceException();
                Excel::Range nTarget = (Excel.Range)nSheet.Cells[Global.CBVHeaderRow.HeaderColumnLabelDisplayRow, col];
                //Coverity fix - CID 18716
                if (nTarget == null)
                    throw new System.NullReferenceException();

                if (Global.CBVHeaderColumnList.Count > 0)
                    nSheet.Cells[Global.CBVHeaderRow.HeaderColumnLabelDisplayRow, col] = nSheet.Cells[Global.CBVHeaderRow.HeaderColumnRow, col];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void OnColumnHeaderSelectionChange(string[] strValue, object sheet, Microsoft.Office.Interop.Excel.Range nTarget, int colSelected, int rowActive, int colActive)
        {
            Excel.Worksheet nSheet = sheet as Excel.Worksheet;
            try
            {
                if (int.Parse(strValue[2]) == (int)Global.CBVHeaderRow.HeaderOptionRow)
                {
                    AddInOptionHeaderCellDropdown(nSheet, nTarget, rowActive, colActive);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetOptionHeaderDropdownList(int row, int col, object sheet)
        {
            string value = string.Empty;
            Excel.Worksheet nSheet = sheet as Excel.Worksheet;
            //Coverity fix - CID 18706
            if (nSheet == null)
                throw new System.NullReferenceException("Excel sheet cannot be null");
            try
            {
                if (!UpdateheaderOptionList(col, sheet))
                {
                    return string.Empty;
                }
                //show the first list value as default
                Global._ExcelApp.EnableEvents = false;
                nSheet.Cells[Global.CBVHeaderRow.HeaderOptionRow, col] = Global.CBVHeaderOptionList[0];
                Global._ExcelApp.EnableEvents = true;
                if (Global.CBVHeaderOptionList.Count != 0)
                {
                    foreach (string item in Global.CBVHeaderOptionList)
                    {
                        value = value + "," + item;
                    }
                }

                return value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Global._ExcelApp.EnableEvents = true;
            }
        }

        public void UpdateheaderCategoryList()
        {
            try
            {
                Global.DataView = Global.CBVSHEET_COEDATAVIEWBO.COEDataView;

                Global.CBVHeaderCategoryList.Clear();

                for (int i = 0; i < Global.DataView.Tables.Count; i++)
                {
                    if (!Global.CBVHeaderCategoryList.Contains(Global.DataView.Tables[i].Database.ToString()))
                        Global.CBVHeaderCategoryList.Add(Global.DataView.Tables[i].Database.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateheaderTableList(int Col, object sheet)
        {
            string value;

            try
            {
                Global.DataView = Global.CBVSHEET_COEDATAVIEWBO.COEDataView;

                Global.CBVHeaderTableList.Clear();

                value = GlobalCBVExcel.GetCell(Global.CBVHeaderRow.HeaderCategoryRow, Col, sheet);
                for (int i = 0; i < Global.DataView.Tables.Count; i++)
                {
                    if (value.ToLower() == Global.DataView.Tables[i].Database.ToLower())
                    {
                        if (Global.DataView.Tables[i].Alias != null && !Global.DataView.Tables[i].Alias.Equals(string.Empty))
                        {
                            Global.CBVHeaderTableList.Add(Global.DataView.Tables[i].Alias.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Return the option list header first value 
        public string OptionListFistValue(string headerColumnDatatype, string headerColumnIndextype, string headerColumnMimetype)
        {
            try
            {
                List<string> str = DisplayOption.OptionHeaderList(headerColumnDatatype, headerColumnIndextype, headerColumnMimetype);
                if (str == null)
                    return string.Empty;
                else
                    return str[0].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateheaderOptionList(int Col, object sheet)
        {
            CriteriaUtilities criteriaUtilities = new CriteriaUtilities();
            try
            {
                //Set the category, table and column range into global range during -  On loading of OptionHeader the category, table and column label appeared.
                Global.SetRangeValue();
                string headerColumnDatatype = criteriaUtilities.GetHeaderColumnInfo(Col, sheet, Global.DATATYPE);
                string headerColumnIndextype = criteriaUtilities.GetHeaderColumnInfo(Col, sheet, Global.INDEXTYPE);
                string headerColumnMimetype = criteriaUtilities.GetHeaderColumnInfo(Col, sheet, Global.MIMITYPE);

                //tell the options to do something special for molweight or formula
                string mimeOverrideString = GetMolWeightOrFormulaString(sheet, Col);
                if (mimeOverrideString != null && mimeOverrideString != String.Empty)
                {
                    headerColumnMimetype = mimeOverrideString;
                }

                List<string> str = DisplayOption.HeaderList(headerColumnDatatype, headerColumnIndextype, headerColumnMimetype);
                if (str == null)
                    return false;
                Global.CBVHeaderOptionList.Clear(); //Clear the existing list

                //Refill the list
                for (int i = 0; i < str.Count; i++)
                {
                    Global.CBVHeaderOptionList.Add(str[i]);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateheaderColumnList(int Col, object sheet)
        {
            string valueCategory;
            string valueTable;

            try
            {
                Global.DataView = Global.CBVSHEET_COEDATAVIEWBO.COEDataView;
                Global.CBVHeaderColumnList.Clear();

                valueCategory = GlobalCBVExcel.GetCell(Global.CBVHeaderRow.HeaderCategoryRow, Col, sheet);
                valueTable = GlobalCBVExcel.GetCell(Global.CBVHeaderRow.HeaderTableRow, Col, sheet);
                for (int i = 0; i < Global.DataView.Tables.Count; i++)
                {
                    for (int j = 0; j < Global.DataView.Tables[i].Fields.Count; j++)
                    {

                        if (valueCategory.ToLower() == Global.DataView.Tables[i].Database.ToLower())
                        {
                            if (Global.DataView.Tables[i].Alias != null && Global.DataView.Tables[i].Alias.ToString() != string.Empty)
                            {
                                if (valueTable.ToLower() == Global.DataView.Tables[i].Alias.ToLower())
                                {

                                    //Bug #126152
                                    if (Global.DataView.Tables[i].Fields[j].Alias != null && Global.DataView.Tables[i].Fields[j].Alias.ToString() != string.Empty && Global.DataView.Tables[i].Fields[j].Visible != false)
                                    {

                                        Global.CBVHeaderColumnList.Add(Global.DataView.Tables[i].Fields[j].Alias.ToString());
                                    }

                                     if ( !string.IsNullOrEmpty(Global.DataView.Tables[i].Fields[j].Alias) && Global.DataView.Tables[i].Fields[j].Visible != false)
                                    {
                                        if ((Global.DataView.Tables[i].Fields[j].IndexType.ToString().ToUpper().Trim() == Global.COESTRUCTURE_INDEXTYPE) || (Global.DataView.Tables[i].Fields[j].LookupFieldId > 0 && Global.DVTables.GetField(Global.DataView.Tables[i].Fields[j].LookupDisplayFieldId).IndexType.ToString().Equals(Global.COESTRUCTURE_INDEXTYPE, StringComparison.OrdinalIgnoreCase))) //CSBR-154419
                                        {
                                            Global.CBVHeaderColumnList.Add(Global.DataView.Tables[i].Fields[j].Alias.ToString() + ".MOLWEIGHT");
                                            Global.CBVHeaderColumnList.Add(Global.DataView.Tables[i].Fields[j].Alias.ToString() + ".FORMULA");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion "_ Generate header List _"


        #region "_ On Slection Change handler _"

        public void OnSlectionChange(object sheet, Microsoft.Office.Interop.Excel.Range nTarget)
        {
            int activeRow, activeCol;
            try
            {
                string selectedRange = nTarget.get_Address(Type.Missing, Type.Missing, Excel.XlReferenceStyle.xlA1, Type.Missing, Type.Missing);

                Excel.Worksheet nSheet = sheet as Excel.Worksheet;

                if (selectedRange.IndexOf(":") < 0)
                {
                    string[] strValue = selectedRange.Split('$');
                    int colSelected = Global.StringToNum(strValue[1].Trim());
                    activeRow = int.Parse(strValue[2]);

                    activeCol = colSelected;

                    if (Global.CellDropdownRange.Contains(activeCol) && Global.InsertColumn)
                    {
                        //Check whether the category exists or not on selected cell
                        if (activeRow != 1)
                        {
                            Excel.Range cell = (Excel.Range)nSheet.Cells[1, activeCol];
                            if (!String.IsNullOrEmpty(cell.Text.ToString()))
                            {
                                AddInCellDropdown(nSheet, nTarget, activeRow, activeCol);
                            }
                        }
                        else
                        {
                            AddInCellDropdown(nSheet, nTarget, activeRow, activeCol);
                        }
                    }
                    else if (Global.CellDropdownRange.Contains(activeCol) && activeRow <= 3)
                    {
                        nTarget.Validation.InCellDropdown = true; // To retain the dropdown of headers row.
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void OptionHeaderOperation(object sheet, Excel.Range target, string optionHeader)
        {
            //If the header table data type has Structure or Image then return
            try
            {
                // if (ISStructureImageHeader(sheet, target.Column) || ISMolWeightFormulaExists(sheet, target.Column))
                // return;

                if (ISStructureImageHeader(sheet, target.Column))
                    return;

                foreach (Global.CBVHeaderOptionInstanceDisplay ArithmeticOption in Enum.GetValues(typeof(Global.CBVHeaderOptionInstanceDisplay)))
                {
                    if (optionHeader.Equals(StringEnum.GetStringValue(ArithmeticOption), StringComparison.OrdinalIgnoreCase))
                    {
                        Global._ExcelApp.ScreenUpdating = false;
                        SetCellOptionHeaderResult(sheet, (int)Global.CBVHeaderRow.HeaderResultStartupRow, target.Column, Global._CBVSearchResult, optionHeader);
                        Global._ExcelApp.ScreenUpdating = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion "_ On Slection Change handler _"

        #region "_ Add In Cell Dropdown _"

        public void AddInCellDropdown(object sheet, Microsoft.Office.Interop.Excel.Range nTarget, int row, int col)
        {
            Excel.Worksheet nSheet = sheet as Excel.Worksheet;

            try
            {
                Global._ExcelApp.EnableEvents = false;
                GlobalCBVExcel.AddInCellDropDownRange = GetDropdownList(row, col, nSheet);
                Global._ExcelApp.EnableEvents = true;

                if (!string.IsNullOrEmpty(GlobalCBVExcel.AddInCellDropDownRange))
                {
                    nTarget.Validation.Delete();
                    nTarget.Validation.Add(Excel.XlDVType.xlValidateList, Type.Missing, Type.Missing, GlobalCBVExcel.AddInCellDropDownRange, Type.Missing);

                    nTarget.Validation.IgnoreBlank = true;
                    nTarget.Validation.InCellDropdown = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Global._ExcelApp.EnableEvents = true;
            }
        }

        public void AddInOptionHeaderCellDropdown(object sheet, Microsoft.Office.Interop.Excel.Range nTarget, int row, int col)
        {
            string value = string.Empty;
            Excel.Worksheet nSheet = sheet as Excel.Worksheet;
            try
            {
                value = GetOptionHeaderDropdownList(row, col, nSheet);
                if (!string.IsNullOrEmpty(value))
                {
                    nTarget.Validation.Delete();
                    nTarget.Validation.Add(Excel.XlDVType.xlValidateList, Excel.XlDVAlertStyle.xlValidAlertStop, Excel.XlFormatConditionOperator.xlBetween, value, Type.Missing);
                    nTarget.Validation.InCellDropdown = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion "_ Add In Cell Dropdown _"

        #region "_ DataViewBO related functions inside Excel sheet _"

        /* 11.0.3
        public void ProtectHiddenSheet(Excel::Worksheet hWorkSheet)
        {
            if (hWorkSheet != null)
                hWorkSheet.Protect(Global.COEDATAVIEW_HIDDENSHEET_PASSWORD, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                 Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                 Type.Missing, Type.Missing, Type.Missing);

        }

        public void UnProtectHiddenSheet(Excel::Worksheet hWorkSheet)
        {
            if (hWorkSheet != null)
                hWorkSheet.Unprotect(Global.COEDATAVIEW_HIDDENSHEET_PASSWORD);
        }
        */

        //11.0.3
        public bool SaveDataView(Excel::Worksheet nSheet, COEDataViewBO dataViewbo)
        {
            try
            {
                Excel::Worksheet hiddenSheet = GlobalCBVExcel.Get_CSHidden();
                int idRow = 0;

                if (hiddenSheet != null)
                {
                    int r = hiddenSheet.UsedRange.Rows.Count;
                    int c = hiddenSheet.UsedRange.Columns.Count;
                    Excel.Range cellRange = hiddenSheet.get_Range("A2", Global.NumToString(c) + r);

                    object[,] cellRngVal = null;
                    cellRngVal = (object[,])cellRange.Value2;
                    for (int rows = 2; rows <= r + 1; rows++)
                    {
                        string cellData = DataSetUtilities.GetDataFromCellRange(cellRngVal, rows - 1, 1);

                        if (rows == 2) //The second row is start point and it's always empty
                            idRow = rows;
                        else
                            idRow = rows - 1; // The row have data

                        string uid = DataSetUtilities.GetDataFromCellRange(cellRngVal, idRow - 1, (int)Global.CBVHiddenSheetHeader.ID);

                        if (cellData.Equals(string.Empty))
                        {
                            SetValueAtDataViewBegin();
                            if (string.IsNullOrEmpty(uid))
                            {
                                GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.UID, hiddenSheet, "0");
                                Global.UID = 0;
                            }
                            else
                            {
                                GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.UID, hiddenSheet, Convert.ToString(Convert.ToInt32(uid.ToString()) + 1));
                                Global.UID = Convert.ToInt32(uid.ToString()) + 1;
                            }
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.ID, hiddenSheet, dataViewbo.ToString());
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.Database, hiddenSheet, dataViewbo.DatabaseName.ToString());                          
                            // CSBR #151940 - Truncating the Dataview content, if it exceeds the max. cell content range (Limit is 32767).
                            if (Convert.ToString(dataViewbo.COEDataView).Length < Global.CellMaxCharacter)
                                GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.Dataview, hiddenSheet, Convert.ToString(dataViewbo.COEDataView));
                            else
                                GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.Dataview, hiddenSheet, Convert.ToString(dataViewbo.COEDataView).Substring(0, Global.MaxCellDataLength-1));
                            
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.CBVNewColIndex, hiddenSheet, Global.CBVNewColumnIndex.ToString());
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.Dataviewname, hiddenSheet, dataViewbo.Name);
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.Sheetname, hiddenSheet, nSheet.Name.Trim());
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.CBVActiveSheetIndex, hiddenSheet, Global.CBVActiveColumnIndex.ToString());
                            //At begning the max no of record set to 0;
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.MaxResultCount, hiddenSheet, Global.MaxRecordInResultSet.ToString());
                            //Sheet Created User - Current login user
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.SheetCreatedUser, hiddenSheet, Global.CurrentUser);

                            //Sheet Modified User - Current login user
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.ModifiedUser, hiddenSheet, Global.CurrentUser);

                            //Store the celldropdownlist as seralize format
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.CellDropdownColList, hiddenSheet, SerializeDeserialize.Serialize(Global.CellDropdownRange));

                            //Store the sheetProperties . The created user is the current user and modified user is empty bec'z the information store during the sheet creation.                       
                            CBVSheetProperties sheetProp = new CBVSheetProperties();
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.SerializeSheetProperties, hiddenSheet, sheetProp.SetSeralizeSheetProperties(Global.UID, dataViewbo.Name, Global.CurrentUser, string.Empty, Global.MRUServer, Global.MRUServerMode, DateTime.Now));
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //11.0.3
        public bool ReSaveDataView(Excel::Worksheet nSheet, COEDataViewBO dataViewbo)
        {
            try
            {
                Excel::Worksheet hiddenSheet = GlobalCBVExcel.Get_CSHidden();
                int idRow = 0;

                if (hiddenSheet != null)
                {
                    int r = hiddenSheet.UsedRange.Rows.Count;
                    int c = hiddenSheet.UsedRange.Columns.Count;
                    Excel.Range cellRange = hiddenSheet.get_Range("A2", Global.NumToString(c) + r);

                    object[,] cellRngVal = null;
                    cellRngVal = (object[,])cellRange.Value2;
                    for (int rows = 2; rows <= r; rows++)
                    {
                        string cellData = DataSetUtilities.GetDataFromCellRange(cellRngVal, rows - 1, 1);

                        if (rows == 2) //The second row is start point and it's always empty
                            idRow = rows;
                        else
                            idRow = rows - 1; // The row have data

                        string uid = DataSetUtilities.GetDataFromCellRange(cellRngVal, idRow - 1, (int)Global.CBVHiddenSheetHeader.ID);

                        if (cellData.Equals(string.Empty))
                        {

                            if (string.IsNullOrEmpty(uid))
                            {

                                GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.UID, hiddenSheet, "0");
                                Global.UID = 0;
                            }
                            else
                            {
                                GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.UID, hiddenSheet, Convert.ToString(Convert.ToInt32(uid.ToString()) + 1));
                                Global.UID = Convert.ToInt32(uid.ToString()) + 1;
                            }
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.ID, hiddenSheet, dataViewbo.ToString());
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.Database, hiddenSheet, dataViewbo.DatabaseName.ToString());
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.Dataview, hiddenSheet, dataViewbo.COEDataView.ToString());
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.CBVNewColIndex, hiddenSheet, Global.CBVNewColumnIndex.ToString());
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.Dataviewname, hiddenSheet, dataViewbo.Name);
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.Sheetname, hiddenSheet, nSheet.Name.Trim());
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.CBVActiveSheetIndex, hiddenSheet, Global.CBVActiveColumnIndex.ToString());
                            //At begning the max no of record set to 0;
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.MaxResultCount, hiddenSheet, Global.MaxRecordInResultSet.ToString());


                            //Sheet Created User - Current login user
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.SheetCreatedUser, hiddenSheet, Global.CurrentUser);

                            //Sheet Modified User - Current login user
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.ModifiedUser, hiddenSheet, Global.CurrentUser);

                            //Store the celldropdownlist as seralize format
                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.CellDropdownColList, hiddenSheet, SerializeDeserialize.Serialize(Global.CellDropdownRange));

                            //Store the sheetProperties . The created user is the current user and modified user is empty bec'z the information store during the sheet creation.                       
                            CBVSheetProperties sheetProp = new CBVSheetProperties();

                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.SerializeSheetProperties, hiddenSheet, sheetProp.SetSeralizeSheetProperties(Global.UID, dataViewbo.Name, Global.CurrentUser, string.Empty, Global.MRUServer, Global.MRUServerMode, DateTime.Now));
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string LoadCOEDataView(Excel.Worksheet worksheet)
        {
            string value = string.Empty;
            Excel::Application _Excelapp = Global._ExcelApp as Excel::Application;

            try
            {
                Excel::Worksheet nHideenSheet = GlobalCBVExcel.FindSheet(Global.COEDATAVIEW_HIDDENSHEET);

                if (nHideenSheet != null)
                {
                    SetDataViewInGlobalVars(worksheet);

                    for (int rows = 2; rows <= nHideenSheet.UsedRange.Rows.Count; rows++)
                    {
                        Excel::Range cell = (Excel.Range)nHideenSheet.Rows.Cells[rows, Global.CBVHiddenSheetHeader.Sheetname];
                        if (cell.Text.ToString() == worksheet.Name.Trim())
                        {
                            value = GlobalCBVExcel.GetCell("A" + rows, nHideenSheet);
                            Global.CBVSHEET_DATABASE_NAME = GlobalCBVExcel.GetCell("B" + rows, nHideenSheet);
                            string value2 = GlobalCBVExcel.GetCell("C" + rows, nHideenSheet);
                            Global.CBVSHEET_COEDATAVIEW = GetCOEDataView(value2);

                            Global.CBVNewColumnIndex = Convert.ToInt32(GlobalCBVExcel.GetCell("D" + rows, nHideenSheet));
                            Global.CBVCBVSHEET_NAME = GlobalCBVExcel.GetCell("F" + rows, nHideenSheet);
                            string temp = GlobalCBVExcel.GetCell("G" + rows, nHideenSheet);
                            Global.MaxRecordInResultSet = temp != string.Empty ? Convert.ToInt32(temp) : 0;
                            temp = GlobalCBVExcel.GetCell("H" + rows, nHideenSheet);
                            Global.CBVActiveColumnIndex = temp != string.Empty ? Convert.ToInt32(temp) : 0;
                            break;
                        }
                    }
                }
                return value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetDataFromHiddenSheet(string sheetName, int colIndex)
        {
            try
            {
                Excel::Worksheet nHideenSheet = GlobalCBVExcel.Get_CSHidden();
                int rowCnt = nHideenSheet.UsedRange.Rows.Count;
                int colCnt = nHideenSheet.UsedRange.Columns.Count;

                object[,] cellRngVal = new object[rowCnt, colCnt];

                Excel.Range cellRange = nHideenSheet.get_Range("A2", Global.NumToString(colCnt) + rowCnt);
                cellRngVal = (object[,])cellRange.Value2;
                //cellRngVal = (List<object>[,])cellRange.Value2;

                for (int row = 1; row <= rowCnt; row++)
                {
                    if (string.IsNullOrEmpty(DataSetUtilities.GetDataFromCellRange(cellRngVal, row, 1)))
                        continue;

                    for (int col = 1; col <= colCnt; col++)
                    {
                        if (cellRngVal[row, col] == null)
                            continue;
                        if (cellRngVal[row, col].ToString().Trim().Equals(sheetName, StringComparison.OrdinalIgnoreCase))
                        {
                            return (DataSetUtilities.GetDataFromCellRange(cellRngVal, row, colIndex));
                        }
                    }
                }
                return null;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetDataFromHiddenSheet(int uid, int colIndex)
        {
            try
            {
                Excel::Worksheet nHideenSheet = GlobalCBVExcel.Get_CSHidden();
                int rowCnt = nHideenSheet.UsedRange.Rows.Count;
                int colCnt = nHideenSheet.UsedRange.Columns.Count;

                object[,] cellRngVal = new object[rowCnt, colCnt];
                Excel.Range cellRange = nHideenSheet.get_Range("A2", Global.NumToString(colCnt) + rowCnt);
                cellRngVal = (object[,])cellRange.Value2;
                for (int row = 1; row <= rowCnt; row++)
                {
                    if (string.IsNullOrEmpty(DataSetUtilities.GetDataFromCellRange(cellRngVal, row, 1)))
                        continue;

                    for (int col = 1; col <= colCnt; col++)
                    {
                        if (cellRngVal[row, col] == null)
                            continue;
                        if (cellRngVal[row, col].ToString().Trim().Equals(uid.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            return (DataSetUtilities.GetDataFromCellRange(cellRngVal, row, colIndex));
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        bool repeat = false;

        public void SetDataViewInGlobalVars(Excel.Worksheet sheet)
        {
            try
            {
                //Retrieves the headers (category, table and column) into global variable range
                Global.GlobalcellRngVal = null;
                Global.SetRangeValue();
                Global.ISServerValidated = false;
                Global._ExcelApp.EnableEvents = false;

                if (repeat == false)
                {
                    repeat = true;
                    UserInfo userInfo = new UserInfo();
                    userInfo.UpdateUserInfo(Global.CurrentUser);

                    //Before refresh and reset - check the toggle column autosize
                    if (Global.ToggleColumnAutoSize == 0)
                    {
                        DisableToggleColumnAutosizing();
                    }

                    Global.InsertColumn = true;

                    Excel::Worksheet nHideenSheet = GlobalCBVExcel.Get_CSHidden();
                    if (nHideenSheet != null)
                    {
                        int rowCnt = nHideenSheet.UsedRange.Rows.Count;
                        int colCnt = nHideenSheet.UsedRange.Columns.Count;
                        string data = string.Empty;
                        string dvID = string.Empty;
                        string dvName = string.Empty;

                        object[,] cellRngVal = new object[rowCnt, colCnt];
                        Excel.Range cellRange = nHideenSheet.get_Range("A2", Global.NumToString(colCnt) + rowCnt);
                        cellRngVal = (object[,])cellRange.Value2;

                        for (int rows = 1; rows <= rowCnt; rows++)
                        {
                            data = DataSetUtilities.GetDataFromCellRange(cellRngVal, rows, (int)Global.CBVHiddenSheetHeader.Sheetname);
                            if (string.IsNullOrEmpty(data))
                                continue;

                            COEDataViewBOList dataViewsBOList = null;
                            if (data.Equals(sheet.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                try
                                {
                                    Global.RestoreCSLAPrincipal();
                                    dataViewsBOList = COEDataViewBOList.GetDataViewListAndNoMaster();
                                }
                                finally
                                {
                                    Global.RestoreWindowsPrincipal();

                                }
                                if (dataViewsBOList == null)
                                    return;

                                dvID = DataSetUtilities.GetDataFromCellRange(cellRngVal, rows, (int)Global.CBVHiddenSheetHeader.ID).Trim();
                                dvName = DataSetUtilities.GetDataFromCellRange(cellRngVal, rows, (int)Global.CBVHiddenSheetHeader.Dataviewname).Trim();

                                foreach (COEDataViewBO dataViewsBO in dataViewsBOList)
                                {
                                    if ((dataViewsBO.ID.ToString().Trim().Equals(dvID, StringComparison.OrdinalIgnoreCase)) && (dataViewsBO.Name.ToString().Trim().Equals(dvName, StringComparison.OrdinalIgnoreCase)))
                                    {
                                        Global.CBVSHEET_COEDATAVIEWBO = dataViewsBO;
                                        Global.DataView = Global.CBVSHEET_COEDATAVIEWBO.COEDataView;
                                        Global.Tables = TableListBO.NewTableListBO(Global.DataView.Tables);
                                        Global.CBVSHEET_DATABASE_NAME = dataViewsBO.Name;
                                        Global.CBVSHEET_COEDATAVIEW = dataViewsBO.COEDataView;

                                        data = DataSetUtilities.GetDataFromCellRange(cellRngVal, rows, (int)Global.CBVHiddenSheetHeader.CBVNewColIndex);
                                        Global.CBVNewColumnIndex = Convert.ToInt32(data);

                                        data = DataSetUtilities.GetDataFromCellRange(cellRngVal, rows, (int)Global.CBVHiddenSheetHeader.CBVActiveSheetIndex);
                                        Global.CBVActiveColumnIndex = Convert.ToInt32(data);

                                        //Set the current sheet name in global variable
                                        Global.CurrentWorkSheetName = sheet.Name.Trim();
                                        data = DataSetUtilities.GetDataFromCellRange(cellRngVal, rows, (int)Global.CBVHiddenSheetHeader.UID);
                                        Global.UID = Convert.ToInt32(data);

                                        string temp = DataSetUtilities.GetDataFromCellRange(cellRngVal, rows, (int)Global.CBVHiddenSheetHeader.MaxResultCount);
                                        Global.MaxRecordInResultSet = temp != string.Empty ? Convert.ToInt32(temp) : 0;
                                        Global.COEDATAVIEW_NAME = dataViewsBO.Name;
                                        string seachUpdaateCriteriaVal = DataSetUtilities.GetDataFromCellRange(cellRngVal, rows, (int)Global.CBVHiddenSheetHeader.SearchUpdateCriteria);

                                        if (!string.IsNullOrEmpty(seachUpdaateCriteriaVal))
                                        {
                                            Global.SearchUpdateCriteria = (List<object>)SerializeDeserialize.Deserialize(seachUpdaateCriteriaVal);
                                        }
                                        else
                                        {
                                            Global.SearchUpdateCriteria = new List<object>();
                                        }

                                        string seralizeResultSchema = DataSetUtilities.GetDataFromCellRange(cellRngVal, rows, (int)Global.CBVHiddenSheetHeader.SerializeCBVResult);


                                        if (!string.IsNullOrEmpty(seralizeResultSchema))
                                        {
                                            Global._CBVSearchResult = (DataTable)SerializeDeserialize.Deserialize(seralizeResultSchema);
                                        }

                                        string seralizeCellDropdownColList = DataSetUtilities.GetDataFromCellRange(cellRngVal, rows, (int)Global.CBVHiddenSheetHeader.CellDropdownColList);

                                        if (!string.IsNullOrEmpty(seralizeCellDropdownColList))
                                        {
                                            Global.CellDropdownRange = (List<int>)SerializeDeserialize.Deserialize(seralizeCellDropdownColList);
                                        }

                                        using (ValidateDataView validateExistingDV = new ValidateDataView())
                                        {
                                            string dataview = DataSetUtilities.GetDataFromCellRange(cellRngVal, rows, (int)Global.CBVHiddenSheetHeader.Dataview).Trim();
                                            validateExistingDV.ValidateExistingDataview(dataview, sheet, dataViewsBO);
                                        }

                                        //If no changes exists ValidateExistingDataview
                                        if (Global._CBVSearchResult != null && Global._CBVSearchResult.Rows.Count == 0)
                                        {
                                            DataSetUtilities.RestoreCBVResult(sheet);
                                        }
                                        repeat = false;

                                        return;
                                    }
                                }
                                dataViewsBOList.Clear();
                            }
                        }
                    }

                    //Set the CBVSHEET_COEDATAVIEWBO is null (if there is no record exists in hidden sheet)
                    if (nHideenSheet.UsedRange.Rows.Count <= 1)
                        Global.CBVSHEET_COEDATAVIEWBO = null;
                    Global.ISServerValidated = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Global._ExcelApp.EnableEvents = true;
            }
        }

        //11.0.3
        public void UpdateHiddenSheet(int col, string compareValue, string newValue)
        {
            try
            {
                Excel::Worksheet nHideenSheet = GlobalCBVExcel.Get_CSHidden();

                if (nHideenSheet != null)
                {
                    int rows = nHideenSheet.UsedRange.Rows.Count;
                    int cols = nHideenSheet.UsedRange.Columns.Count;

                    Excel.Range cellRange = nHideenSheet.get_Range("A2", Global.NumToString(cols) + rows);
                    object[,] cellRngVal = null; //= new object[rows-1,cols];
                    cellRngVal = (object[,])cellRange.Value2;
                    for (int r = 2; r <= rows; r++)
                    {
                        string val = DataSetUtilities.GetDataFromCellRange(cellRngVal, r - 1, (int)Global.CBVHiddenSheetHeader.Sheetname);
                        if (val.Equals(string.Empty))
                            continue;

                        if (val.Equals(compareValue, StringComparison.OrdinalIgnoreCase))
                        {
                            //Fixed CSBR-155847                
                            if (newValue.Length < Global.CellMaxCharacter)
                            {
                                GlobalCBVExcel.SetCell(r, col, nHideenSheet, newValue);
                            }
                            else // Truncating the Dataview content, if it exceeds the max. cell content range (Max limit is 32767).
                            {
                                GlobalCBVExcel.SetCell(r, col, nHideenSheet, newValue.Substring(0, Global.MaxCellDataLength - 1));
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Modify the hidden sheet - delete the records those worksheet have deleted
        public void RemoveRowFromHidden(string lastSheetName)
        {
            try
            {
                if (!GlobalCBVExcel.SheetExists(lastSheetName))
                {
                    Excel::Worksheet nHideenSheet = GlobalCBVExcel.Get_CSHidden();

                    if (nHideenSheet != null)
                    {
                        int rows = nHideenSheet.UsedRange.Rows.Count;
                        int cols = nHideenSheet.UsedRange.Columns.Count;

                        Excel.Range cellRange = nHideenSheet.get_Range("A2", Global.NumToString(cols) + rows);

                        object[,] cellRngVal = null; //= new object[rows-1,cols];
                        cellRngVal = (object[,])cellRange.Value2;
                        for (int r = 2; r <= rows; r++)
                        {
                            string val = DataSetUtilities.GetDataFromCellRange(cellRngVal, r - 1, (int)Global.CBVHiddenSheetHeader.Sheetname);
                            if (val.Equals(string.Empty))
                                continue;

                            if (val.Equals(lastSheetName, StringComparison.OrdinalIgnoreCase))
                            {
                                Excel.Range range = (Excel.Range)nHideenSheet.Cells[r, Global.CBVHiddenSheetHeader.Sheetname];
                                range.EntireRow.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                                if (nHideenSheet.UsedRange.Rows.Count <= 1)
                                    Global.CBVSHEET_COEDATAVIEWBO = null;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Use for disposing the excel object
        /// </summary>
        /// <param name="o"></param>
        protected void ReleaseObject(object o)
        {
            try
            {
                if (o != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(o);
            }

            finally
            {
                o = null;
            }
        }

        //Clear the global variables
        public void ClearDataViewGlobalVars()
        {
            Global.CBVSHEET_COEDATAVIEWBO = null;
            Global.CBVSHEET_DATABASE_NAME = null;
            Global.CBVSHEET_COEDATAVIEW = null;
            Global.CBVNewColumnIndex = 0;
            Global.CBVActiveColumnIndex = 0;
            Global.COEDATAVIEW_NAME = null;
            Global.MaxRecordInResultSet = 0;
        }

        public void SetValueAtDataViewBegin()
        {
            Global.MaxRecordInResultSet = 0;
        }

        public COEDataView GetCOEDataView(string xmlDataView)
        {
            try
            {
                COEDataView coeDataView = new COEDataView();
                coeDataView.GetFromXML(xmlDataView);
                return coeDataView;
            }
            catch
            {
                MessageBox.Show(Properties.Resources.msgErrorReadCOEData, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public void ReCreateCOEDataViewBO(Excel.Worksheet sheet)
        {
            string value = null;
            value = LoadCOEDataView(sheet);

            if (value != string.Empty)
            {
                try
                {
                    COEDataViewBO dataViewBO = null;

                    try
                    {
                        Global.RestoreCSLAPrincipal();
                        dataViewBO = COEDataViewBO.Get(Int32.Parse(value));
                    }
                    finally
                    {
                        Global.RestoreWindowsPrincipal();
                    }
                    dataViewBO.DatabaseName = Global.CBVSHEET_DATABASE_NAME;
                    dataViewBO.COEDataView = Global.CBVSHEET_COEDATAVIEW;
                    Global.CBVSHEET_COEDATAVIEWBO = dataViewBO;
                }
                catch
                {
                    MessageBox.Show(Properties.Resources.msgErrorCOEBO, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Global.RestoreWindowsPrincipal();
                }
            }
            else
            {
                SaveDataView(sheet, Global.CBVSHEET_COEDATAVIEWBO);
            }
        }

        # endregion "_ DataViewBO related functions inside Excel sheet _"

        #region "_ Format CBV Sheet _"

        public bool ToggleColumnAutosizing(Excel.Worksheet worksheet)
        {
            try
            {
                Excel.Range nTarget;
                nTarget = worksheet.get_Range("A1", Global.NumToString(worksheet.Columns.Count) + Convert.ToInt32(Global.CBVHeaderRow.HeaderColumnLabelDisplayRow));

                Excel.XlLineStyle colLineStyle = (Excel.XlLineStyle)nTarget.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight].LineStyle;
                if (colLineStyle != Excel.XlLineStyle.xlDouble)
                {
                    nTarget.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlDouble;
                    nTarget.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThick;
                    nTarget.Borders[Excel.XlBordersIndex.xlEdgeLeft].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

                    nTarget.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlDouble;
                    nTarget.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThick;
                    nTarget.Borders[Excel.XlBordersIndex.xlEdgeRight].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

                    nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlDouble;
                    nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].Weight = Excel.XlBorderWeight.xlThick;
                    nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;
                    nTarget.EntireColumn.AutoFit();

                    return true;

                }
                //11.0.3
                else if (colLineStyle.Equals(Excel.XlLineStyle.xlDouble))
                {
                    nTarget.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                    nTarget.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
                    nTarget.Borders[Excel.XlBordersIndex.xlEdgeLeft].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

                    nTarget.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    nTarget.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
                    nTarget.Borders[Excel.XlBordersIndex.xlEdgeRight].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

                    nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                    nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].Weight = Excel.XlBorderWeight.xlThin;
                    nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void EnableToggleColumnAutosizing()
        {
            try
            {
                foreach (Excel.Worksheet worksheet in Global._ExcelApp.Worksheets)
                {
                    if (Global.IsCBVExcelWorksheet(worksheet))
                    {
                        Excel.Range nTarget;

                        nTarget = worksheet.get_Range("A1", Global.NumToString(worksheet.Columns.Count) + Convert.ToInt32(Global.CBVHeaderRow.HeaderColumnLabelDisplayRow));

                        Excel.XlLineStyle colLineStyle = (Excel.XlLineStyle)nTarget.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight].LineStyle;

                        if (colLineStyle != Excel.XlLineStyle.xlDouble)
                        {
                            nTarget.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlDouble;
                            nTarget.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThick;
                            nTarget.Borders[Excel.XlBordersIndex.xlEdgeLeft].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

                            nTarget.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlDouble;
                            nTarget.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThick;
                            nTarget.Borders[Excel.XlBordersIndex.xlEdgeRight].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

                            nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlDouble;
                            nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].Weight = Excel.XlBorderWeight.xlThick;
                            nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;
                            nTarget.EntireColumn.AutoFit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void DisableToggleColumnAutosizing()
        {
            try
            {
                foreach (Excel.Worksheet worksheet in Global._ExcelApp.Worksheets)
                {
                    if (Global.IsCBVExcelWorksheet(worksheet))
                    {
                        Excel.Range nTarget;

                        nTarget = worksheet.get_Range("A1", Global.NumToString(worksheet.Columns.Count) + Convert.ToInt32(Global.CBVHeaderRow.HeaderColumnLabelDisplayRow));

                        Excel.XlLineStyle colLineStyle = (Excel.XlLineStyle)nTarget.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight].LineStyle;

                        //11.0.3
                        if (colLineStyle.Equals(Excel.XlLineStyle.xlDouble))
                        {
                            nTarget.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                            nTarget.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
                            nTarget.Borders[Excel.XlBordersIndex.xlEdgeLeft].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

                            nTarget.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                            nTarget.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
                            nTarget.Borders[Excel.XlBordersIndex.xlEdgeRight].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

                            nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                            nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].Weight = Excel.XlBorderWeight.xlThin;
                            nTarget.Borders[Excel.XlBordersIndex.xlInsideVertical].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void EnableDisableToggleColumnAutoSizing()
        {
            //11.0.3
            if (Global.ToggleColumnAutoSize.Equals(1))
                EnableToggleColumnAutosizing();
            else
                DisableToggleColumnAutosizing();
        }

        public void ToggleHeaderDisplay(Excel.Worksheet worksheet)
        {
            try
            {
                //11.0.3
                if (Global.outlineLevel.Equals(1))
                {
                    worksheet.Outline.ShowLevels(1, Type.Missing);
                    Global.outlineLevel = 2;
                }
                else
                {
                    worksheet.Outline.ShowLevels(2, Type.Missing);
                    Global.outlineLevel = 1;
                    Excel::Range nTarget = ((Excel.Range)worksheet.Cells[Global.CBVHeaderRow.HeaderCategoryRow, Global.CBVHeaderRow.HeaderCategoryRow]);
                    nTarget.Select();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GroupingHeaderSection(object sheet)
        {
            Excel.Worksheet nSheet = sheet as Excel.Worksheet;
            try
            {
                
                //Coverity fix - CID 18718
                if (nSheet == null)
                    throw new System.NullReferenceException();
                // Fix bug 126150
                object rowIndex = (int)Global.CBVHeaderRow.HeaderCategoryRow + ":" + (int)Global.CBVHeaderRow.HeaderOptionRow;
                Excel.Range nRange = nSheet.Rows[rowIndex, Type.Missing] as Excel.Range;
                //Coverity fix - CID 18718
                if (nRange == null)
                    throw new System.NullReferenceException();
                nRange.OutlineLevel = Global.outlineLevel;
                nRange.Group(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion "_ Format CBV Sheet _"

        #region "_ Utility functions"

        public void SetFocus(Excel::Worksheet worksheet)
        {
            Excel::Worksheet nSheet = (worksheet) as Excel::Worksheet;
            Global.CBVNewColumnIndex = GetCBVNewCategoryIndex(worksheet);
            try
            {
                //UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.CBVNewColIndex, Global.CBVNewColumnIndex.ToString());
                //11.0.3
                UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.CBVNewColIndex, Global.CurrentWorkSheetName, Global.CBVNewColumnIndex.ToString());

                Excel::Range nTarget = ((Excel.Range)nSheet.Cells[Global.CBVHeaderRow.HeaderCategoryRow, Global.CBVNewColumnIndex]);
                nTarget.Select();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetFocusRange(Excel::Worksheet worksheet, Excel::Range target)
        {
            CellDropDownRange(worksheet, target);

            Excel::Worksheet nSheet = (worksheet) as Excel::Worksheet;

            Global.InsertColumn = true;
            try
            {
                //11.0.3
                UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.CBVNewColIndex, Global.CurrentWorkSheetName, Global.CBVNewColumnIndex.ToString());

                //11.0.3
                UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.CellDropdownColList, Global.CurrentWorkSheetName, SerializeDeserialize.Serialize(Global.CellDropdownRange));

                Excel::Range nTarget = ((Excel.Range)nSheet.Cells[Global.CBVHeaderRow.HeaderCategoryRow, Global.CBVSelectedColumnIndex]);
                Global.CBVSelectedColumnIndex = 0;

                //If the current selected cell position is same location of dropdown (the generated) then place a more position down
                //So that the dropdown will appear.

                //11.0.3
                if (nTarget.Value2 == target.Value2)
                    worksheet.Cells.Application.ActiveCell.get_Offset(1, 0).Select();
                nTarget.Select();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void CellDropDownRange(Excel::Worksheet worksheet, Excel::Range target)
        {
            bool flagInsertCol = false;
            int insertColIndex = (target.Columns.Count > 1) ? target.Column + 1 : target.Column;
            foreach (Excel.Range col in target.Columns)
            {
                string colName = col.get_Address(Type.Missing, Type.Missing, Excel.XlReferenceStyle.xlA1, Type.Missing, Type.Missing);

                if (colName.Contains(":"))
                {
                    int colNum = Global.GetColumnNumberOnColumnRange(colName);



                    //Set the new column Index
                    if (colNum > Global.CBVNewColumnIndex)
                        Global.CBVNewColumnIndex = colNum;

                    //First column no is treated as selected column index and it's reset inside the setfocus method
                    if (Global.CBVSelectedColumnIndex <= 0)
                        Global.CBVSelectedColumnIndex = colNum;

                    if (!Global.CellDropdownRange.Contains(colNum))
                    {
                        Global.CellDropdownRange.Add(colNum);
                    }

                    //If datacolumn have selected
                    Excel.Range rngVal = worksheet.Cells[col.Row, col.Column] as Excel.Range;
                    if (col.Column == insertColIndex && (!string.IsNullOrEmpty(rngVal.Text.ToString())) && flagInsertCol == false)
                    {
                        col.Insert(Excel.XlInsertShiftDirection.xlShiftToRight, Type.Missing);
                        flagInsertCol = true;

                        Global.CBVSelectedColumnIndex = col.Column - 1;
                        Global.CBVNewColumnIndex = GetCBVNewCategoryIndex(worksheet);
                        if (!Global.CellDropdownRange.Contains(Global.CBVNewColumnIndex))
                        {
                            Global.CellDropdownRange.Add(Global.CBVNewColumnIndex);
                        }
                        break;
                    }
                }
                else
                {
                    Global.CBVNewColumnIndex = GetCBVNewCategoryIndex(worksheet);
                    Global.CBVSelectedColumnIndex = Global.CBVNewColumnIndex;
                    Global.CellDropdownRange.Add(Global.CBVNewColumnIndex);
                }

            }
        }

        public void UpdateCellDropdownRange(int[] colNums)
        {
            foreach (int colNum in colNums)
            {
                if (!Global.CellDropdownRange.Contains(colNum))
                    Global.CellDropdownRange.Add(colNum);

                if (Global.CBVNewColumnIndex < colNum)
                    Global.CBVNewColumnIndex = colNum;
            }
        }

        public void UpdateIndex(Excel.Worksheet sheet)
        {
            string value = null;

            try
            {
                //11.0.3
                if (Global.CBVNewColumnIndex.Equals(0))
                {
                    ReCreateCOEDataViewBO(sheet);
                }

                value = GlobalCBVExcel.GetCell(Global.CBVHeaderRow.HeaderColumnRow, Global.CBVNewColumnIndex, sheet);

                if ((value != null) && (value != String.Empty))
                {
                    Global.CBVNewColumnIndex++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Identify the operators which is use for conditional searching.
        /// </summary>
        /// <param name="srchValue"></param>
        /// <returns></returns>
        public bool CheckOperator(string srchValue)
        {
            Regex rgxOprSym = new Regex("{|>=|<=|>|<|=|!=|<>|BETWEEN|CONTAINS|NOTCONTAINS|LIKE|EQUALS|FORMULA|IN |STARTSWITH|ENDWITH|MOLWEIGHT|NOTLIKE}"); //CSBR-167573 fixed, Included a space postfix to IN
            if (rgxOprSym.Matches(srchValue).Count >= 1)
                return true;
            else
                return false;

        }
        // 11.0.3
        /// <summary>
        /// Identify the operators which is use for conditional searching.
        /// </summary>
        /// <param name="srchValue"></param>
        /// <returns></returns>
        public bool CheckOperatorTextValue(string srchValue)
        {
            Regex rgxOprSym = new Regex("{|=|BETWEEN|CONTAINS|NOTCONTAINS|LIKE|EQUALS|FORMULA|STARTSWITH|ENDWITH|MOLWEIGHT|IN |NOTLIKE}"); //CBOE-1145 Included IN operator to make list search work in CBVExcel
            if (rgxOprSym.Matches(srchValue).Count >= 1)
                return true;
            else
                return false;

        }

        /// <summary>
        /// Identify the operators which is use for structure conditional searching.
        /// </summary>
        /// <param name="optionVal"></param>
        /// <returns></returns>       
        public bool CheckStructureOperator(string optionVal)
        {
            Regex rgxOprSym = new Regex("{|IDENTITY|FULL|SIMILARITY|SUBSTRUCTURE|}");
            if (rgxOprSym.Matches(optionVal).Count >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// //Clean the CBV Result table which holds the CBV search result
        /// </summary>
        public void CleanCBVResultTable()
        {
            if (null != Global._CBVResult)
                Global._CBVResult.Clear();
            if (null != Global._CBVSearchResult)
                Global._CBVSearchResult.Clear();
        }

        public bool ISStructureImageHeader(object sheet, int activeCol)
        {
            Excel::Worksheet nSheet = sheet as Excel::Worksheet;
            //Coverity fix - CID 18703
            if (nSheet == null)
                throw new System.NullReferenceException("Excel sheet cannot be null");
            Excel.Range cell = (Excel.Range)nSheet.Cells[(int)Global.CBVHeaderRow.HeaderWhereRow, activeCol];
            //Coverity fix - CID 18703
            if (cell == null)
                throw new System.NullReferenceException("Range cannot be null");
            string optionHeader = string.Empty;
            string headerColumnIndextype = string.Empty;
            string headerColumnMimetype = string.Empty;

            try
            {
                CriteriaUtilities criteriaUtilities = new CriteriaUtilities();
                headerColumnIndextype = criteriaUtilities.GetHeaderColumnInfo(activeCol, sheet, Global.INDEXTYPE);
                headerColumnMimetype = criteriaUtilities.GetHeaderColumnInfo(activeCol, sheet, Global.MIMITYPE);

                if (Global.ISImageTypeExists(headerColumnMimetype))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return false;
        }

        public bool ISMolWeightFormulaExists(object sheet, int activeCol)
        {
            Excel::Worksheet nSheet = sheet as Excel::Worksheet;
            //Coverity fix - CID 18702
            if (nSheet == null)
                throw new System.NullReferenceException("Excel sheet cannot be null");

            try
            {
                Excel.Range cell = (Excel.Range)nSheet.Cells[(int)Global.CBVHeaderRow.HeaderColumnRow, activeCol];
                //Coverity fix - CID 18702
                if (cell == null)
                    throw new System.NullReferenceException("Cell cannot be null");

                if ((cell.Text.ToString().ToUpper().Trim().EndsWith(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_MolWeight))) || (cell.Text.ToString().ToUpper().Trim().EndsWith(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_Formula))))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }

            return false;
        }

        public string GetMolWeightOrFormulaString(object sheet, int activeCol)
        {
            Excel::Worksheet nSheet = sheet as Excel::Worksheet;
            //Coverity fix - CID 18705
            if (nSheet == null)
                throw new System.NullReferenceException("Excel sheet cannot be null"); 

            try
            {
                Excel.Range cell = (Excel.Range)nSheet.Cells[(int)Global.CBVHeaderRow.HeaderColumnRow, activeCol];
                //Coverity fix - CID 18705
                if (cell == null)
                    throw new System.NullReferenceException("Cell cannot be null"); 

                if (cell.Text.ToString().ToUpper().Trim().EndsWith(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_MolWeight)))
                {
                    return "MOLWEIGHT";
                }
                else if (cell.Text.ToString().ToUpper().Trim().EndsWith(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_Formula)))
                {
                    return "FORMULA";
                }
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }

            return "";
        }

        public void SetCellDatatype(Excel::Range range, Excel::Worksheet sheet)
        {
            //11.0.3
            if ((int)range.Row == (int)Global.CBVHeaderRow.HeaderWhereRow)
            {
                range.NumberFormat = "@";
            }
        }




        #endregion "_ Utility functions _"

        #region "_ CBV Search functions _"


        /// <summary>
        /// Multi call search methods with Marquee ProgressBar 
        /// </summary>

        public void UpdateCurrentResultsMP(Excel::Worksheet nSheet, Global.CBVSearch CBVSearchType)
        {
            frmMarqueeProgressBar objMProgress = null;
            Thread maxHit = null;
            Thread dataThread = null;

            DataSet dataResult = null;

            string[] ouputCriteria = GetOutputCriteria(nSheet);
            string[] searchCriteria = GetInputCriteria(nSheet, false);

            string[] domainList = GetDomainListData(nSheet);

            try
            {
                //11.0.3
                //if (Global.IsKeyExistForUpdate == false)
                if (!Global.IsKeyExistForUpdate)
                {
                    MessageBox.Show(Properties.Resources.msgUKPLNotExists, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //11.0.3
                else if ((COEDataViewData.IsSearchCriteriaModified(searchCriteria, Global.SearchCriteria)) && domainList == null)
                {
                    objMProgress = new frmMarqueeProgressBar(Properties.Resources.msgPBarRetrieveResInfo, (StringEnum.GetStringValue(Global.ProgressBarStyle.MARQUEE)), true);

                    //Thread

                    maxHit = new Thread(delegate() { try { excepMainThread = null; COEDataViewData.resultPageInfo = COEDataViewData.GetResultPageInfo(nSheet, Global.CBVSHEET_COEDATAVIEWBO.ID, Global.DataView, this); } catch (Exception ex) { excepMainThread = ex; maxHit.Abort(); } });

                    maxHit.Start();
                    objMProgress.Show();

                    while (maxHit.IsAlive) { Application.DoEvents(); }

                    if (excepMainThread != null)
                        throw excepMainThread;

                    maxHit.Abort();
                    objMProgress.Close();
                }

                else if (domainList != null)
                {
                    objMProgress = new frmMarqueeProgressBar(Properties.Resources.msgPBarRetrieveResInfo, (StringEnum.GetStringValue(Global.ProgressBarStyle.MARQUEE)), true);


                    maxHit = new Thread(delegate() { try { excepMainThread = null; COEDataViewData.resultPageInfo = COEDataViewData.GetResultPageInfo(nSheet, Global.CBVSHEET_COEDATAVIEWBO.ID, Global.DataView, this, domainList); } catch (Exception ex) { excepMainThread = ex; maxHit.Abort(); } });

                    maxHit.Start();
                    objMProgress.Show();

                    while (maxHit.IsAlive) { Application.DoEvents(); }

                    if (excepMainThread != null)
                        throw excepMainThread;

                    maxHit.Abort();
                    objMProgress.Close();

                    if (COEDataViewData.resultPageInfo == null)
                        throw new Exception(Properties.Resources.msgInvalidDV);
                }
                else if (searchCriteria != null)
                {
                    objMProgress = new frmMarqueeProgressBar(Properties.Resources.msgPBarRetrieveResInfo, (StringEnum.GetStringValue(Global.ProgressBarStyle.MARQUEE)), true);

                    maxHit = new Thread(delegate() { try { excepMainThread = null; COEDataViewData.resultPageInfo = COEDataViewData.GetResultPageInfo(nSheet, Global.CBVSHEET_COEDATAVIEWBO.ID, Global.DataView, this); } catch (Exception ex) { excepMainThread = ex; maxHit.Abort(); } });

                    maxHit.Start();
                    objMProgress.Show();

                    while (maxHit.IsAlive) { Application.DoEvents(); }

                    if (excepMainThread != null)
                        throw excepMainThread;

                    maxHit.Abort();
                    objMProgress.Close();

                }
                else
                {
                    //Prompt to user - if yes then call the Replace All Result and return else return
                    if (MessageBox.Show(Properties.Resources.msgEmptyCriteriaAddNewSearch, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        //Check whether unique key or primary key exists in cbv sheet
                        if (ISPrimaryKeyORUniqueKeyExists(nSheet) == false)
                        {
                            MessageBox.Show(Properties.Resources.msgCBVSheetPKFK, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        else
                        {
                            ReplaceAllResultsMP(nSheet, Global.CBVSearch.ReplaceAllResults);
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }


                objMProgress = new frmMarqueeProgressBar(Properties.Resources.msgUpdateTitle, (StringEnum.GetStringValue(Global.ProgressBarStyle.MARQUEE)), true);

                //Thread               
                dataThread = new Thread(delegate() { try { excepMainThread = null; dataResult = COEDataViewData.Searching(nSheet, CBVSearchType, COEDataViewData.resultPageInfo, Global.CBVSHEET_COEDATAVIEWBO.ID, searchCriteria, ouputCriteria, this); } catch (Exception ex) { excepMainThread = ex; dataThread.Abort(); } });

                dataThread.Start();
                objMProgress.Show();

                while (dataThread.IsAlive) { Application.DoEvents(); }

                if (null != excepMainThread)
                    throw excepMainThread;

                dataThread.Abort();
                objMProgress.Close();

                if (dataResult.Tables.Count <= 0 || dataResult.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show(Properties.Resources.msgHitList, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    ClearResults(nSheet);

                    objMProgress = new frmMarqueeProgressBar(dataResult.Tables[0].Rows.Count, (StringEnum.GetStringValue(Global.ProgressBarStyle.MARQUEE)));
                    objMProgress.Show();
                    DispalyResult(dataResult, nSheet, CBVSearchType);
                    objMProgress.Close();
                }
            }
            catch (Exception ex)
            {
                if (excepMainThread != null)
                    throw excepMainThread;
                else
                    throw new Exception(ex.Message);
            }

            finally
            {
                if (objMProgress != null)
                    objMProgress.Dispose();
            }
        }

        // Advance Export - Overloaded to handle the existing functionality
        public void ReplaceAllResultsMP(Excel::Worksheet nSheet, Global.CBVSearch CBVSearchType)
        {
            ReplaceAllResultsMP(nSheet, CBVSearchType, null);
        }

        // Advance Export - Extra parameter, ExportSheet object, passed to handle the export functionality
        public void ReplaceAllResultsMP(Excel::Worksheet nSheet, Global.CBVSearch CBVSearchType, Excel::Worksheet nExportSheet)
        {
            frmMarqueeProgressBar objMProgress = null;
            Thread maxHit = null;
            Thread dataThread = null;

            //jhs I may reintroduce this by merging the loops
            //string[][] criterias = GetInputAndOutputCriteria(nSheet);
            //string[] inputCriteria = criterias[0];
            //string[] outputCriteria = criterias[1];

            // 11.0.4
            Global.SetRangeValue();

            string[] inputCriteria = GetInputCriteria(nSheet);
            string[] outputCriteria = GetOutputCriteria(nSheet);

            DataSet dataResult = null;
            try
            {
                objMProgress = new frmMarqueeProgressBar(Properties.Resources.msgPBarRetrieveResInfo, (StringEnum.GetStringValue(Global.ProgressBarStyle.MARQUEE)), true);
                maxHit = new Thread(delegate()
               {
                   //try { excepMainThread = null; COEDataViewData.resultPageInfo = COEDataViewData.GetResultPageInfo(nSheet, Global.CBVSHEET_COEDATAVIEWBO.ID, Global.DataView, this); }
                   // Advance Export - Extra parameter, ExportSheet object, passed to handle the export functionality
                   try { excepMainThread = null; COEDataViewData.resultPageInfo = COEDataViewData.GetResultPageInfo(nSheet, Global.CBVSHEET_COEDATAVIEWBO.ID, Global.DataView, this, null, inputCriteria, nExportSheet); }
                   catch (Exception ex) { excepMainThread = ex; maxHit.Abort(); }
               });

                maxHit.Start();
                objMProgress.Show();

                while (maxHit.IsAlive) { Application.DoEvents(); Thread.Sleep(100); }

                if (null != excepMainThread)
                    throw excepMainThread;

                maxHit.Abort();
                objMProgress.Close();

                if (COEDataViewData.resultPageInfo == null)
                    throw new Exception(Properties.Resources.msgInvalidDV);

                int hitCnt = COEDataViewData.resultPageInfo.PageSize;

                if ((hitCnt > 0) && (hitCnt > Convert.ToDouble(AppConfigSetting.ReadSetting(StringEnum.GetStringValue(Global.ConfigurationKey.MAX_PROMPT_HITS)))))
                {
                    //The amount of records before the user gets a warning about the number of records.  This would handle cases where the hitlist might be lower then the maximum but would be expensive (slow) to populate the sheet).
                    if (MessageBox.Show(String.Format(Properties.Resources.msgHitlistCnt, hitCnt), Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        objMProgress = new frmMarqueeProgressBar(Properties.Resources.msgPBarRetrieveData, (StringEnum.GetStringValue(Global.ProgressBarStyle.MARQUEE)), true);
                        //Thread
                        dataThread = new Thread(delegate()
                        {
                            //try { excepMainThread = null; dataResult = COEDataViewData.Searching(nSheet, CBVSearchType, COEDataViewData.resultPageInfo, Global.CBVSHEET_COEDATAVIEWBO.ID, GetInputCriteria(nSheet), GetOutputCriteria(nSheet), this); }
                            try { excepMainThread = null; dataResult = COEDataViewData.Searching(nSheet, CBVSearchType, COEDataViewData.resultPageInfo, Global.CBVSHEET_COEDATAVIEWBO.ID, inputCriteria, outputCriteria, this); }
                            catch (Exception ex) { excepMainThread = ex; dataThread.Abort(); }
                        });

                        dataThread.Start();
                        objMProgress.Show();

                        while (dataThread.IsAlive) { Application.DoEvents(); Thread.Sleep(100); }

                        if (null != excepMainThread)
                            throw excepMainThread;

                        dataThread.Abort();
                        objMProgress.Close();

                        if (Global.MaxRecordInResultSet > 0)
                            ClearResults(nSheet);

                        if (dataResult.Tables.Count == 0)
                            return;

                        objMProgress = new frmMarqueeProgressBar(dataResult.Tables[0].Rows.Count, (StringEnum.GetStringValue(Global.ProgressBarStyle.MARQUEE)));
                        objMProgress.Show();
                        DispalyResult(dataResult, nSheet, CBVSearchType);
                        objMProgress.Close();
                    } //End if message hit list count
                } //End if hit list 
                else if ((hitCnt > 0) && (hitCnt <= Convert.ToDouble(AppConfigSetting.ReadSetting(StringEnum.GetStringValue(Global.ConfigurationKey.MAX_PROMPT_HITS)))))
                {
                    objMProgress = new frmMarqueeProgressBar(Properties.Resources.msgPBarRetrieveData, (StringEnum.GetStringValue(Global.ProgressBarStyle.MARQUEE)), true);
                    //Thread
                    //dataThread = new Thread(delegate() { try { excepMainThread = null; dataResult = COEDataViewData.Searching(nSheet, CBVSearchType, COEDataViewData.resultPageInfo, Global.CBVSHEET_COEDATAVIEWBO.ID, GetInputCriteria(nSheet), GetOutputCriteria(nSheet), this); } catch (Exception ex) { excepMainThread = ex; dataThread.Abort(); } });
                    dataThread = new Thread(delegate() { try { excepMainThread = null; dataResult = COEDataViewData.Searching(nSheet, CBVSearchType, COEDataViewData.resultPageInfo, Global.CBVSHEET_COEDATAVIEWBO.ID, inputCriteria, outputCriteria, this); } catch (Exception ex) { excepMainThread = ex; dataThread.Abort(); } });

                    dataThread.Start();
                    objMProgress.Show();

                    while (dataThread.IsAlive)
                    {
                        Application.DoEvents();
                        Thread.Sleep(100);
                    }

                    if (null != excepMainThread)
                        throw excepMainThread;

                    dataThread.Abort();
                    objMProgress.Close();

                    if (Global.MaxRecordInResultSet > 0)
                        ClearResults(nSheet);

                    if (dataResult.Tables.Count == 0)
                        return;

                    objMProgress = new frmMarqueeProgressBar(dataResult.Tables[0].Rows.Count, (StringEnum.GetStringValue(Global.ProgressBarStyle.MARQUEE)));
                    objMProgress.Show();
                    DispalyResult(dataResult, nSheet, CBVSearchType);
                    objMProgress.Close();
                }
                else if (hitCnt == 0)
                {
                    MessageBox.Show(Properties.Resources.msgHitList, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch (Exception ex)
            {
                if (null != excepMainThread)
                    throw excepMainThread;
                else
                    throw ex;
            }

            finally
            {
                if (objMProgress != null)
                    objMProgress.Dispose();
            }

        }

        public void GetMimeTypeResult(Excel.Range nRange, Excel.Worksheet nSheet, Global.CBVSearch CBVSearchType)
        {
            frmMarqueeProgressBar objMProgress = null;
            Thread maxHit = null;
            Thread dataThread = null;

            string[] inputCriteria = GetInputCriteriaMimeType(nSheet);
            string[] outputCriteria = GetOutputCriteriaMimeType(nSheet);

            StringBuilder dataResult = null;
            try
            {
                objMProgress = new frmMarqueeProgressBar(Properties.Resources.msgPBarRetrieveResInfo, (StringEnum.GetStringValue(Global.ProgressBarStyle.MARQUEE)), true);
                maxHit = new Thread(delegate()
               {
                   try { excepMainThread = null; COEDataViewData.resultPageInfo = COEDataViewData.GetResultPageInfo(nSheet, Global.CBVSHEET_COEDATAVIEWBO.ID, Global.DataView, this, null, inputCriteria); }
                   catch (Exception ex) { excepMainThread = ex; maxHit.Abort(); }
               });

                maxHit.Start();
                objMProgress.Show();

                while (maxHit.IsAlive) { Application.DoEvents(); Thread.Sleep(100); }

                if (null != excepMainThread)
                    throw excepMainThread;

                maxHit.Abort();
                objMProgress.Close();

                if (COEDataViewData.resultPageInfo == null)
                    throw new Exception(Properties.Resources.msgInvalidDV);

                int hitCnt = COEDataViewData.resultPageInfo.PageSize;

                if (hitCnt > 0)
                {
                    objMProgress = new frmMarqueeProgressBar(Properties.Resources.msgPBarRetrieveData, (StringEnum.GetStringValue(Global.ProgressBarStyle.MARQUEE)), true);

                    dataThread = new Thread(delegate() { try { excepMainThread = null; dataResult = COEDataViewData.SearchingMimeTypeData(nRange, nSheet, CBVSearchType, COEDataViewData.resultPageInfo, Global.CBVSHEET_COEDATAVIEWBO.ID, inputCriteria, outputCriteria, this); } catch (Exception ex) { excepMainThread = ex; dataThread.Abort(); } });

                    dataThread.Start();
                    objMProgress.Show();

                    while (dataThread.IsAlive)
                    {
                        Application.DoEvents();
                        Thread.Sleep(100);
                    }

                    if (null != excepMainThread)
                        throw excepMainThread;

                    dataThread.Abort();
                    objMProgress.Close();

                    //if (Global.MaxRecordInResultSet > 0)
                    //    ClearResults(nSheet);

                    objMProgress = new frmMarqueeProgressBar(1, (StringEnum.GetStringValue(Global.ProgressBarStyle.MARQUEE)));
                    objMProgress.Show();
                    objMProgress.Close();

                    if (dataResult == null || string.Empty == dataResult.ToString())
                        throw new Exception("Data not found in database");

                    string colAlias = GlobalCBVExcel.GetCell((int)Global.CBVHeaderRow.HeaderColumnRow, nRange.Column, nSheet);
                    string tabAlias = GlobalCBVExcel.GetCell((int)Global.CBVHeaderRow.HeaderTableRow, nRange.Column, nSheet);

                    string mimeType = mimeType = COEDataViewData.GetMimeType(colAlias, tabAlias);

                    Global.OpenFileOnMimeType(mimeType, dataResult, nRange, nSheet);
                    objMProgress.Close();
                }
                else if (hitCnt == 0)
                {
                    MessageBox.Show(Properties.Resources.msgHitList, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch (Exception ex)
            {
                if (null != excepMainThread)
                    throw excepMainThread;
                else
                    throw ex;
            }

            finally
            {
                if (objMProgress != null)
                    objMProgress.Dispose();
            }

        }


        public void AppendCurrentResultsMP(Excel::Worksheet nSheet, Global.CBVSearch CBVSearchType)
        {
            string[] ouputCriteria = GetOutputCriteria(nSheet);
            string[] searchCriteria = GetInputCriteria(nSheet);

            frmMarqueeProgressBar objMProgress = null;

            Thread maxHit = null;
            Thread dataThread = null;
            DataSet dataResult = null;

            try
            {
                if ((COEDataViewData.resultPageInfo == null) || (COEDataViewData.IsSearchCriteriaModified(searchCriteria, Global.SearchCriteria)))
                {
                    objMProgress = new frmMarqueeProgressBar(Properties.Resources.msgPBarRetrieveResInfo, (StringEnum.GetStringValue(Global.ProgressBarStyle.MARQUEE)), true);

                    maxHit = new Thread(delegate() { try { excepMainThread = null; COEDataViewData.GetResultPageInfo(nSheet, Global.CBVSHEET_COEDATAVIEWBO.ID, Global.DataView, this); } catch (Exception ex) { excepMainThread = ex; maxHit.Abort(); } });

                    maxHit.Start();
                    objMProgress.Show();

                    while (maxHit.IsAlive) { Application.DoEvents(); Thread.Sleep(100); }

                    if (null != excepMainThread)
                        throw excepMainThread;

                    maxHit.Abort();
                    objMProgress.Close();

                    if (COEDataViewData.resultPageInfo == null)
                        throw new Exception(Properties.Resources.msgInvalidDV);
                }

                int hitCnt = COEDataViewData.resultPageInfo.PageSize;

                //The amount of records before the user gets a message about the number of records.  This would handle cases where the append hitlist might be lower then the maximum but would be expensive (slow) to populate the sheet).
                if (hitCnt > 0)
                {
                    if (MessageBox.Show(String.Format(Properties.Resources.msgAppendResult, hitCnt), Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        objMProgress = new frmMarqueeProgressBar(Properties.Resources.msgPBarRetrieveData, (StringEnum.GetStringValue(Global.ProgressBarStyle.MARQUEE)), true);

                        //Thread
                        dataThread = new Thread(delegate() { try { excepMainThread = null; dataResult = COEDataViewData.Searching(nSheet, CBVSearchType, COEDataViewData.resultPageInfo, Global.CBVSHEET_COEDATAVIEWBO.ID, searchCriteria, ouputCriteria, this); } catch (Exception ex) { excepMainThread = ex; dataThread.Abort(); } });

                        dataThread.Start();
                        objMProgress.Show();

                        while (dataThread.IsAlive) { Application.DoEvents(); Thread.Sleep(100); }

                        if (null != excepMainThread)
                            throw excepMainThread;

                        dataThread.Abort();
                        objMProgress.Close();

                        if (dataResult.Tables.Count == 0)
                            return;

                        objMProgress = new frmMarqueeProgressBar(dataResult.Tables[0].Rows.Count, (StringEnum.GetStringValue(Global.ProgressBarStyle.MARQUEE)));
                        objMProgress.Show();
                        DispalyResult(dataResult, nSheet, CBVSearchType);
                        objMProgress.Close();
                    } //End if message hit list count
                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgHitList, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch (Exception ex)
            {
                if (null != excepMainThread)
                    throw excepMainThread;
                else
                    throw ex;
            }
            finally
            {
                if (objMProgress != null)
                    objMProgress.Dispose();
            }
        }

        public void ExportAllResults(Excel::Worksheet nSheet)
        {
            try
            {
                if (COEDataViewData.resultPageInfo == null)
                {
                    COEDataViewData.resultPageInfo = COEDataViewData.GetResultPageInfo(nSheet, Global.CBVSHEET_COEDATAVIEWBO.ID, Global.DataView, this);
                }

                string exportedData = COEDataViewData.ExportData(nSheet, COEDataViewData.resultPageInfo, Global.CBVSHEET_COEDATAVIEWBO.ID, Global.DataView, this);

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString();
                saveFileDialog1.Filter = "SD File (*.sdf)|*.sdf|All Files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 1;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    // Console.WriteLine(saveFileDialog1.FileName);//Do what you want here 
                    System.IO.File.WriteAllText(saveFileDialog1.FileName, exportedData);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #region "_ Data Searching _"

        private DataSet Searching(Excel::Worksheet nSheet, Global.CBVSearch CBVSearchType)
        {
            DataResult dataResult = new DataResult();
            DataSet resultsDataSet = new DataSet();
            resultsDataSet.CaseSensitive = false;
            //update the CBV new column Index
            string[] ouputCriteria = null;
            string[] searchCriteria = null;

            COESearch coeSearch = new COESearch(Global.CBVSHEET_COEDATAVIEWBO.ID);

            //update the CBV new column Index
            Global.CBVNewColumnIndex = GetCBVNewCategoryIndex(nSheet);

            SearchInput searchInput = new SearchInput();
            string txtdata = string.Empty;
            string[] searchOptions = StructureSearchOption.GetStructureSearchOptionParam();

            Global.InsertColumn = false; // Set the insert column to false

            ResultPageInfo rpi = new ResultPageInfo();
            rpi.ResultSetID = 0;
            rpi.PageSize = Global.PAGESIZE; //fixed
            rpi.Start = 1;
            rpi.End = 10001;
            //Create output criteria
            if (CBVSearchType.Equals(Global.CBVSearch.ReplaceAllResults))
            {
                ouputCriteria = GetOutputCriteria(nSheet);
                //Create input criteria            
                searchCriteria = GetInputCriteria(nSheet);

                searchInput.FieldCriteria = searchCriteria;
                searchInput.ReturnPartialResults = true;

                searchInput.SearchOptions = searchOptions;
                searchInput.Domain = txtdata.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                SearchCriteria.StructureCriteria structureCriteria = new SearchCriteria.StructureCriteria();

                try
                {
                    Global.RestoreCSLAPrincipal();
                    dataResult = coeSearch.DoSearch(searchInput, ouputCriteria, rpi, "true");
                }
                finally
                {
                    Global.RestoreWindowsPrincipal();
                }

                if (!dataResult.Status.Equals(Global.DATARESULT_SUCCESS_STATUS, StringComparison.OrdinalIgnoreCase))
                    throw new Exception(dataResult.Status);

                Global.SearchUpdateCriteria.Clear(); // Clear the existing update criteria list on Replace result command              
                Global.SearchUpdateCriteria.Add(searchCriteria);  // Add the current search criteria into update criteria list                
                resultsDataSet = DataSetUtilities.DataResultToDataSet(dataResult);
            }
            else if (CBVSearchType.Equals(Global.CBVSearch.AppendNewResults))
            {
                ouputCriteria = GetOutputCriteria(nSheet);
                //Create input criteria            
                searchCriteria = GetInputCriteria(nSheet);

                Global.SearchUpdateCriteria.Add(searchCriteria);  // Add the update criteria in list   
                searchInput.FieldCriteria = searchCriteria;
                searchInput.ReturnPartialResults = true;
                searchInput.SearchOptions = searchOptions;
                searchInput.Domain = txtdata.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                Global.RestoreCSLAPrincipal();
                try
                {
                    dataResult = coeSearch.DoSearch(searchInput, ouputCriteria, rpi, "true");
                }
                finally
                {
                    Global.RestoreWindowsPrincipal();
                }

                if (!dataResult.Status.Equals(Global.DATARESULT_SUCCESS_STATUS, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception(dataResult.Status);
                }
                resultsDataSet = DataSetUtilities.DataResultToDataSet(dataResult);

            }
            else if (CBVSearchType.Equals(Global.CBVSearch.UpdateCurrentResults))
            {

                ouputCriteria = GetOutputCriteria(nSheet);

                foreach (string[] criteria in Global.SearchUpdateCriteria)
                {
                    searchCriteria = criteria;
                    searchInput.FieldCriteria = searchCriteria;

                    searchInput.ReturnPartialResults = true;

                    searchInput.SearchOptions = searchOptions;
                    searchInput.Domain = txtdata.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    try
                    {
                        try
                        {
                            Global.RestoreCSLAPrincipal();
                            dataResult = coeSearch.DoSearch(searchInput, ouputCriteria, rpi, "true");
                        }
                        finally
                        {
                            Global.RestoreWindowsPrincipal();
                        }

                        if (dataResult.Status.Equals(Global.DATARESULT_SUCCESS_STATUS, StringComparison.OrdinalIgnoreCase))
                        {
                            using (System.IO.StringReader stringReader = new System.IO.StringReader(dataResult.ResultSet))
                            {
                                resultsDataSet.ReadXml(stringReader);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CBVExcel.ErrorLogging(ex.Message);
                    }

                }
            }

            if (resultsDataSet.Tables.Count > 0)
            {
                DataSetUtilities.AddMissingTables(ref resultsDataSet, nSheet, this);
                DataSetUtilities.AddMissingColumns(ref resultsDataSet, nSheet, this);
            }
            return resultsDataSet;
        }

        // The method is used for searching and taking all the search parameter, from CBVSheetExport hidden sheet.

        private DataSet Searching(Excel::Worksheet nSheet, Global.CBVSearch CBVSearchType, bool IsSearchParamsFromCBVSheetExport)
        {
            DataSet resultsDataSet = new DataSet();
            if ((null == Global.CBVHiddenCriteriaCollection) || (Global.CBVHiddenCriteriaCollection.Count <= 0))
                resultsDataSet = Searching(nSheet, CBVSearchType);
            else
            {
                DataResult dataResult = new DataResult();
                //update the CBV new column Index
                string[] ouputCriteria = null;
                string[] searchCriteria = null;

                try
                {
                    int enumValIndex = 0;

                    COESearch coeSearch = new COESearch(Global.CBVSHEET_COEDATAVIEWBO.ID);

                    //update the CBV new column Index
                    Global.CBVNewColumnIndex = GetCBVNewCategoryIndex(nSheet);
                    string[] searchOptions = StructureSearchOption.GetStructureSearchOptionParam();
                    //update the CBV new column Index
                    Global.CBVNewColumnIndex = GetCBVNewCategoryIndex(nSheet);
                    SearchInput searchInput = new SearchInput();

                    enumValIndex = (int)Global.CBVExcelExportKeys.ReturnPartialResults;
                    //searchInput.ReturnPartialResults = (bool)Global.CBVHiddenCriteriaCollection[enumValIndex];
                    searchInput.ReturnPartialResults = Convert.ToBoolean(Global.CBVHiddenCriteriaCollection[enumValIndex]);

                    enumValIndex = (int)Global.CBVExcelExportKeys.ReturnSimilarityScores;
                    searchInput.ReturnSimilarityScores = Convert.ToBoolean(Global.CBVHiddenCriteriaCollection[enumValIndex]);

                    //searchInput.Domain = txtdata.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    enumValIndex = (int)Global.CBVExcelExportKeys.Domain;
                    if ((null != Global.CBVHiddenCriteriaCollection[enumValIndex]) && String.Empty != Global.CBVHiddenCriteriaCollection[enumValIndex].ToString())
                        searchInput.Domain = (string[])Global.CBVHiddenCriteriaCollection[enumValIndex].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    enumValIndex = (int)Global.CBVExcelExportKeys.DomainFieldName;
                    searchInput.DomainFieldName = (string)Global.CBVHiddenCriteriaCollection[enumValIndex];

                    searchInput.SearchOptions = searchOptions;

                    Global.InsertColumn = false; // Set the insert column to false

                    ResultPageInfo rpi = new ResultPageInfo();

                    enumValIndex = (int)Global.CBVExcelExportKeys.ResultSetID;
                    rpi.ResultSetID = Convert.ToInt16(Global.CBVHiddenCriteriaCollection[enumValIndex]);

                    enumValIndex = (int)Global.CBVExcelExportKeys.PageSize;
                    rpi.PageSize = Convert.ToInt16(Global.CBVHiddenCriteriaCollection[enumValIndex]);

                    enumValIndex = (int)Global.CBVExcelExportKeys.Start;
                    rpi.Start = Convert.ToInt16(Global.CBVHiddenCriteriaCollection[enumValIndex]);

                    enumValIndex = (int)Global.CBVExcelExportKeys.End;
                    rpi.End = Convert.ToInt16(Global.CBVHiddenCriteriaCollection[enumValIndex]);

                    //Create output criteria
                    if (CBVSearchType.Equals(Global.CBVSearch.ReplaceAllResults))
                    {
                        ouputCriteria = GetOutputCriteria(nSheet);

                        //Create input criteria            
                        searchCriteria = GetInputCriteria(nSheet);

                        searchInput.FieldCriteria = searchCriteria;

                        SearchCriteria.StructureCriteria structureCriteria = new SearchCriteria.StructureCriteria();
                        try
                        {
                            Global.RestoreCSLAPrincipal();
                            dataResult = coeSearch.DoSearch(searchInput, ouputCriteria, rpi, "true");
                        }
                        finally
                        {
                            Global.RestoreWindowsPrincipal();
                        }

                        if (!dataResult.Status.Equals(Global.DATARESULT_SUCCESS_STATUS, StringComparison.OrdinalIgnoreCase))
                            throw new Exception(dataResult.Status);

                        Global.SearchUpdateCriteria.Clear(); // Clear the existing update criteria list on Replace result command

                        Global.SearchUpdateCriteria.Add(searchCriteria);  // Add the current search criteria into update criteria list

                        resultsDataSet = DataSetUtilities.DataResultToDataSet(dataResult);
                    }
                    else if (CBVSearchType.Equals(Global.CBVSearch.AppendNewResults))
                    {
                        ouputCriteria = GetOutputCriteria(nSheet);

                        //Create input criteria            
                        searchCriteria = GetInputCriteria(nSheet);

                        Global.SearchUpdateCriteria.Add(searchCriteria);  // Add the update criteria in list                       
                        searchInput.FieldCriteria = searchCriteria;
                        try
                        {
                            Global.RestoreCSLAPrincipal();
                            dataResult = coeSearch.DoSearch(searchInput, ouputCriteria, rpi, "true");
                        }
                        finally
                        {
                            Global.RestoreWindowsPrincipal();
                        }

                        if (!dataResult.Status.Equals(Global.DATARESULT_SUCCESS_STATUS, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new Exception(dataResult.Status);
                        }
                        resultsDataSet = DataSetUtilities.DataResultToDataSet(dataResult);

                    }
                    else if (CBVSearchType.Equals(Global.CBVSearch.UpdateCurrentResults))
                    {
                        ouputCriteria = GetOutputCriteria(nSheet);

                        foreach (string[] criteria in Global.SearchUpdateCriteria)
                        {
                            searchCriteria = criteria;
                            searchInput.FieldCriteria = searchCriteria;

                            try
                            {
                                try
                                {
                                    Global.RestoreCSLAPrincipal();

                                    dataResult = coeSearch.DoSearch(searchInput, ouputCriteria, rpi, "true");
                                }
                                finally
                                {
                                    Global.RestoreWindowsPrincipal();
                                }

                                if (dataResult.Status.Equals(Global.DATARESULT_SUCCESS_STATUS, StringComparison.OrdinalIgnoreCase))
                                {
                                    using (System.IO.StringReader stringReader = new System.IO.StringReader(dataResult.ResultSet))
                                    {
                                        resultsDataSet.ReadXml(stringReader);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                CBVExcel.ErrorLogging(ex.Message);
                            }

                        }
                    }
                }

                catch (Exception ex)
                {
                    throw ex;
                }

                if (resultsDataSet.Tables.Count > 0)
                {
                    DataSetUtilities.AddMissingTables(ref resultsDataSet, nSheet, this);
                    DataSetUtilities.AddMissingColumns(ref resultsDataSet, nSheet, this);
                }

                return resultsDataSet;
            }
            return resultsDataSet;
        }

        public bool ValidateInputCriteria(Excel::Worksheet nSheet)
        {
            try
            {
                //The array have double dimensation
                if (Global.CBVNewColumnIndex == 1)
                    Global.CBVNewColumnIndex = 2;

                for (int x = 1; x <= Global.CBVNewColumnIndex; x++)
                {
                    if (!Global.CellDropdownRange.Contains(x))
                        continue;

                    if (!string.IsNullOrEmpty(GlobalCBVExcel.GetCell(Global.CBVHeaderRow.HeaderWhereRow, x, nSheet)))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
            return false;

        }

        public string[] GetInputCriteria(Excel::Worksheet nSheet)
        {
            return GetInputCriteria(nSheet, true);
        }

        public string[] GetInputCriteria(Excel::Worksheet nSheet, bool IsRetrieveAll)
        {
            string[] searchCriteria = new string[] { };
            //Validate the input criteria exists or not
            if (ValidateInputCriteria(nSheet))
            {
                int rowCatHeader = (int)Global.CBVHeaderRow.HeaderCategoryRow;
                int rowTabHeader = (int)Global.CBVHeaderRow.HeaderTableRow;
                int rowColHeader = (int)Global.CBVHeaderRow.HeaderColumnRow;
                //int rowWhereHeader = (int)Global.CBVHeaderRow.HeaderWhereRow;
                int rowOptionHeader = (int)Global.CBVHeaderRow.HeaderOptionRow;

                string catAlias = string.Empty;
                string tabAlias = string.Empty;
                string colAlias = string.Empty;

                if (Global.CBVNewColumnIndex == 1)
                    Global.CBVNewColumnIndex = 2;

                Excel.Range cellRange = nSheet.get_Range("A" + rowCatHeader, Global.NumToString(Global.CBVNewColumnIndex) + rowOptionHeader);

                object[,] cellRngVal = new object[5, Global.CBVNewColumnIndex];
                cellRngVal = (object[,])cellRange.Value2;

                int cnt = 0; // counter for array value

                StringBuilder sb = new StringBuilder();
                for (int x = 1; x <= Global.CBVNewColumnIndex; x++)
                {
                    // check  whether the column exists in search range or not
                    if (!Global.CellDropdownRange.Contains(x))
                        continue;

                    catAlias = DataSetUtilities.GetDataFromCellRange(cellRngVal, rowCatHeader, x).Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT);
                    tabAlias = DataSetUtilities.GetDataFromCellRange(cellRngVal, rowTabHeader, x).Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT);
                    colAlias = DataSetUtilities.GetDataFromCellRange(cellRngVal, rowColHeader, x).Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT);

                    if (string.IsNullOrEmpty(catAlias) || string.IsNullOrEmpty(tabAlias) || string.IsNullOrEmpty(colAlias))
                        continue;


                    string searchVal = GlobalCBVExcel.GetCell(Global.CBVHeaderRow.HeaderWhereRow, x, nSheet);
                    if (!string.IsNullOrEmpty(searchVal))
                    {
                        //11.0.3 - 
                        // Lookup by Value fields
                        /*if (COEDataViewData.ISLookupExists(catAlias, tabAlias, colAlias))
                        {
                            string[] dbTabCols = COEDataViewData.GetLookupColumnTabDB(catAlias, tabAlias, colAlias).Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                            catAlias = dbTabCols[0];
                            tabAlias = dbTabCols[1];
                            colAlias = dbTabCols[2];
                        }*/

                        //Retrieve the real column (If molecular weight and formula attached with structure column name) 
                        colAlias = Global.FieldContainsMolWtMolFm(colAlias);

                        // string searchVal = DataSetUtilities.GetDataFromCellRange(cellRngVal, rowWhereHeader, x);
                        //string searchVal = GlobalCBVExcel.GetCell(Global.CBVHeaderRow.HeaderWhereRow, x, nSheet);
                        string optionVal = DataSetUtilities.GetDataFromCellRange(cellRngVal, rowOptionHeader, x);

                        // 11.0.4 - If Similary is selected then checking the Structure Search Option. It should have valid value
                        if (optionVal.ToUpper().Trim() == StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionStructure.Disp_Similar))
                        {
                            bool IsValidSimilaritySearchValue = true;

                            if (StructureSearchOption.SimilarSearchThld == null || StructureSearchOption.SimilarSearchThld.Trim() == "")
                            {
                                IsValidSimilaritySearchValue = false;
                            }
                            else
                            {
                                try
                                {
                                    if (Convert.ToInt32(StructureSearchOption.SimilarSearchThld.Trim()) >= 20 && Convert.ToInt32(StructureSearchOption.SimilarSearchThld.Trim()) <= 100)
                                    {
                                        IsValidSimilaritySearchValue = true;
                                    }
                                    else
                                    {
                                        IsValidSimilaritySearchValue = false;
                                    }
                                }
                                catch
                                {
                                    IsValidSimilaritySearchValue = false;
                                }
                            }

                            if (!IsValidSimilaritySearchValue)
                                throw new Exception("Invalid Structure similarity search value. It should be within 20-100%.\nPlease verify the value under Search Preferences.");


                        }

                        string indexType = COEDataViewData.GetIndexType(colAlias, tabAlias);

                        string colText = DataSetUtilities.GetDataFromCellRange(cellRngVal, rowColHeader, x);

                        if (searchVal.Trim() != String.Empty)
                        {
                            Array.Resize(ref searchCriteria, searchCriteria.Length + 1);
                            sb.Remove(0, sb.Length); // Clear the stringbuilder  

                            // 11.0.4 - Adding the table and field alias in quotes
                            if (tabAlias.Contains(" ") || colAlias.Contains(" "))
                                sb.Append('"');

                            //The "." is replaced with escape character. It's used to retain the exact alias name after splitting with "." if alias name contain dot operator
                            //sb.Append(tabAlias);
                            sb.Append(tabAlias.Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT));
                            sb.Append(".");

                            //The "." is replaced with escape character. It's used to retain the exact alias name after splitting with "." if alias name contain dot operator
                            //sb.Append(colAlias);
                            sb.Append(colAlias.Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT));

                            // 11.0.4 - Adding the table and field alias in quotes
                            if (tabAlias.Trim().Contains(" ") || colAlias.Trim().Contains(" "))
                                sb.Append('"');

                            sb.Append(" ");

                            //Check the conditon for structure search
                            if (colText.ToUpper().EndsWith(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_MolWeight)))
                            {
                                sb.Append("MOLWEIGHT");
                                sb.Append(" ");
                                //sb.Append(searchVal.Trim());
                                sb.Append(searchVal.Trim().Replace('"', '\r'));
                            }
                            else if (colText.ToUpper().EndsWith(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_Formula)))
                            {
                                sb.Append("FORMULA");
                                sb.Append(" ");
                                //sb.Append(searchVal.Trim());
                                sb.Append(searchVal.Trim().Replace('"', '\r'));
                            }
                            else if (indexType.Trim().Equals(Global.COESTRUCTURE_INDEXTYPE, StringComparison.OrdinalIgnoreCase) || CheckStructureOperator(optionVal)) //CSBR-153861
                            {
                                sb.Append(optionVal.Trim());
                                sb.Append(" ");
                                //sb.Append(searchVal.Trim());
                                sb.Append(searchVal.Trim().Replace('"', '\r'));
                            }
                            //Add the "=" operator for '-' and '%' criteria. If the user entered
                            else if (!CheckOperator(searchVal.ToUpper().Trim()))
                            {
                                sb.Append("=");
                                //sb.Append(searchVal.Trim());
                                sb.Append(searchVal.Trim().Replace('"', '\r'));
                            }
                            else
                            {
                                // 11.0.3 - If search field is text type and there is not text operator then add EQUAL operation
                                if (IsTextField(tabAlias, colAlias))
                                {
                                    if (!CheckOperatorTextValue(searchVal.ToUpper().Trim()))
                                    {
                                        sb.Append("=");
                                    }
                                }

                                //sb.Append(searchVal.Trim());
                                sb.Append(searchVal.Trim().Replace('"', '\r'));
                            }
                            searchCriteria.SetValue(sb.ToString(), cnt);
                            cnt++;
                        }
                    } //end searchVal
                }
            }
            else
            {
                if (IsRetrieveAll)
                    searchCriteria = new string[] { Global.OPERATOR_RETRIEVE_ALL };
                else
                    return null;
            }
            return searchCriteria;
        }

        // Advance Export - Setting the Search Criterias from the export sheet
        public void SetInputCriteria(Excel::Worksheet nSheet, Excel::Worksheet nExportSheet)
        {
            string[] searchCriteria = new string[] { };

            // Getting the FieldCriteria (field details and search criterias) from the export sheet
            string fieldCriteriasDetails = Convert.ToString(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.FIELDCRITERA) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.FIELDCRITERA) + "2").Value2);
            //Fixed CSBR - 152134
            fieldCriteriasDetails = System.Web.HttpUtility.HtmlDecode(fieldCriteriasDetails);

            string[] separator = new string[] { COEFormInterchange.separator };
            string[] delimiter = new string[] { COEFormInterchange.delimiter };
            string[] fieldDelimiter = new string[] { COEFormInterchange.fieldDelimiter };

            // Splitting the FieldCriterias individually
            string[] searchCriteriasValues = fieldCriteriasDetails.Split(separator, StringSplitOptions.None);


            foreach (string strValues in searchCriteriasValues)
            {
                // Splitting the individual FieldCriteria into field details and search criteria 
                string[] searchCriterias = strValues.Split(delimiter, StringSplitOptions.None);

                string exportCategoryAlias = string.Empty;
                string exportTableAlias = string.Empty;
                string exportColAlias = string.Empty;

                bool isStructure = false;

                bool IsColumnHeaderOptionRow = false;

                string exportCriteriaFld = string.Empty;
                string exportOperator = string.Empty;
                string exportSearchCriteriaValue = string.Empty;

                if (searchCriterias.Length == 4)
                {
                    string[] strFieldDetails = searchCriterias[0].Split(fieldDelimiter, StringSplitOptions.None);

                    if (strFieldDetails.Length >= 3)
                    {
                        exportCategoryAlias = strFieldDetails[0];
                        exportTableAlias = strFieldDetails[1];
                        exportColAlias = strFieldDetails[2];
                    }

                    exportCriteriaFld = searchCriterias[1];

                    if (!string.IsNullOrEmpty(exportCriteriaFld.Trim()))
                    {
                        if (exportCriteriaFld.Equals(COEFormInterchange.criteriaFields.FORMULA.ToString(), StringComparison.OrdinalIgnoreCase))
                            exportColAlias += "." + COEFormInterchange.criteriaFields.FORMULA.ToString();
                        else if (exportCriteriaFld.Equals(COEFormInterchange.criteriaFields.MOLWEIGHT.ToString(), StringComparison.OrdinalIgnoreCase))
                            exportColAlias += "." + COEFormInterchange.criteriaFields.MOLWEIGHT.ToString();
                        else
                        {
                            IsColumnHeaderOptionRow = true;
                            isStructure = true;
                        }
                    }

                    exportOperator = searchCriterias[2];
                    exportSearchCriteriaValue = searchCriterias[3];
                }


                string catAlias = string.Empty;
                string tabAlias = string.Empty;
                string colAlias = string.Empty;
                string searchValue = string.Empty;
                string exportOperatorToAppend = string.Empty;


                StringBuilder sb = new StringBuilder();
                // Setting the Field Search Criterias on the CBV Excel sheet
                for (int x = 1; x <= nSheet.UsedRange.Columns.Count; x++)
                {
                    catAlias = Convert.ToString(nSheet.get_Range(Global.NumToString(x) + "1", Global.NumToString(x) + "1").Value2);
                    tabAlias = Convert.ToString(nSheet.get_Range(Global.NumToString(x) + "2", Global.NumToString(x) + "2").Value2);
                    colAlias = Convert.ToString(nSheet.get_Range(Global.NumToString(x) + "3", Global.NumToString(x) + "3").Value2);

                    // Retreiving and setting the Search Value
                    if (catAlias.Equals(exportCategoryAlias, StringComparison.OrdinalIgnoreCase) && tabAlias.Equals(exportTableAlias, StringComparison.OrdinalIgnoreCase) && colAlias.Equals(exportColAlias, StringComparison.OrdinalIgnoreCase))
                    {
                      if (!string.IsNullOrEmpty(exportCriteriaFld))
                        {                         
                            exportOperatorToAppend = string.Empty;
                        }
                        else if (string.IsNullOrEmpty(exportOperator))
                        {  
                            exportOperatorToAppend = string.Empty;
                        }
                        else if (exportOperator.Equals(SearchCriteria.COEOperators.LT.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            exportOperatorToAppend = "<";
                        }
                        else if (exportOperator.Equals(SearchCriteria.COEOperators.GT.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            exportOperatorToAppend = ">";
                        }
                        else if (exportOperator.Equals(SearchCriteria.COEOperators.GTE.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            exportOperatorToAppend = ">=";
                        }
                        else if (exportOperator.Equals(SearchCriteria.COEOperators.LTE.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            exportOperatorToAppend = "<=";
                        }
                        else if (exportOperator.Equals(SearchCriteria.COEOperators.NOTEQUAL.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            exportOperatorToAppend = "!=";
                        }
                        else if (exportOperator.Equals(SearchCriteria.COEOperators.EQUAL.ToString(), StringComparison.OrdinalIgnoreCase))
                        { 
                            exportOperatorToAppend = string.Empty;
                        }                        
                        else
                        { 
                            exportOperatorToAppend = exportOperator;
                        } 
                        //if operator exists in search value then don't append the starting operator in the search value to be displayed
                        if (IsTextField(tabAlias, colAlias))
                        {
                            if (CheckOperatorTextValue(System.Web.HttpUtility.HtmlDecode(exportSearchCriteriaValue)) || exportOperatorToAppend == string.Empty)
                            {
                                searchValue = exportSearchCriteriaValue;
                            }
                            else
                            {
                                searchValue = exportOperatorToAppend + " " + exportSearchCriteriaValue;
                            }
                        }
                        else
                        {
                            if (CheckOperator(System.Web.HttpUtility.HtmlDecode(exportSearchCriteriaValue)) || exportOperatorToAppend == string.Empty)
                            {
                                searchValue = exportSearchCriteriaValue;
                            }
                            else
                            {
                                searchValue = exportOperatorToAppend + " " + exportSearchCriteriaValue;
                            }
                        }

                        // If search value is a structure then convert the value into structure image
                        if (isStructure)
                        {
                            using (GraphicUtility graphicUtil = new GraphicUtility())
                            {
                                Excel.Range cell = null;

                                Int64 structureMaxHeight = StructureSearchOption.StructureMaxHeight;
                                Int64 structureMaxWidth = StructureSearchOption.StructureMaxWidth;


                                object[] arrStruct = new object[4];
                                try
                                {
                                    arrStruct[0] = nSheet.Name;
                                    arrStruct[1] = 4;
                                    arrStruct[2] = x;
                                    object b64 = searchValue;
                                    arrStruct[3] = b64;


                                    cell = nSheet.Cells[4, x] as Excel.Range;

                                    Global.AddStructrue(arrStruct);

                                    // Set the default column width as Maxcolumn width at begning. Validation call (SetStructureColumnWidth) in below existing method
                                    graphicUtil.SetStructureHeightWidth(nSheet, cell, structureMaxHeight, structureMaxWidth);

                                }

                                catch (Exception Ex)
                                {
                                    CBVExcel.ErrorLogging(Ex.Message);
                                }
                            }
                        }
                        else
                        {                            
                            Excel.Range rngInputData = nSheet.get_Range(Global.NumToString(x) + "4", Global.NumToString(x) + "4");

                            //Set the cell format from General to Text. //12.4
                            rngInputData.NumberFormat = "@";  //Fixed CSBR-153635

                            //Set value into the cell 
                            rngInputData.Value2 = System.Web.HttpUtility.HtmlDecode(searchValue);
                         
                        }

                        if (IsColumnHeaderOptionRow)
                            nSheet.Cells[Global.CBVHeaderRow.HeaderOptionRow, x] = exportCriteriaFld;

                        break;
                    }

                }
            }



        }


        public bool IsTextField(string tabAlias, string colAlias)
        {
            bool lblnIsTextField = false;

            try
            {
                COEDataView dataView = Global.CBVSHEET_COEDATAVIEWBO.COEDataView;
                int tableIndex = -1;
                string fieldType = string.Empty;
                for (int lintTableIndex = 0; lintTableIndex < dataView.Tables.Count; lintTableIndex++)
                {
                    if (dataView.Tables[lintTableIndex].Alias.ToString().Trim().Equals(tabAlias.Trim().Replace(Global.ESCAPE_SEPERATOR_FOR_DOT, Global.DOT_SEPARATOR), StringComparison.OrdinalIgnoreCase))
                    {
                        tableIndex = lintTableIndex;
                        break;
                    }
                }

                for (int lintColIndex = 0; lintColIndex < dataView.Tables[tableIndex].Fields.Count; lintColIndex++)
                {
                    if (dataView.Tables[tableIndex].Fields[lintColIndex].Alias.ToString().Equals(colAlias.Trim().Replace(Global.ESCAPE_SEPERATOR_FOR_DOT, Global.DOT_SEPARATOR), StringComparison.OrdinalIgnoreCase))
                    {
                        fieldType = dataView.Tables[tableIndex].Fields[lintColIndex].DataType.ToString();
                        break;
                    }
                }

                if (fieldType.ToUpper().Trim() == "TEXT")
                {
                    lblnIsTextField = true;
                }


            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }

            return lblnIsTextField;
        }

        public string[] GetOutputCriteria(Excel::Worksheet nSheet)
        {
            int cnt = 0; // counter for array value

            StringBuilder sb = new StringBuilder();
            string[] outputdata = new string[] { };

            int rowCatHeader = (int)Global.CBVHeaderRow.HeaderCategoryRow;
            int rowTabHeader = (int)Global.CBVHeaderRow.HeaderTableRow;
            int rowColHeader = (int)Global.CBVHeaderRow.HeaderColumnRow;
            string catAlias = string.Empty;
            string tabAlias = string.Empty;
            string colAlias = string.Empty;

            Global.LookupByValueFields = "LookupByValueFields: ";


            if (Global.CBVNewColumnIndex == 1)
                Global.CBVNewColumnIndex = 2;

            Excel.Range cellRange = nSheet.get_Range("A" + rowCatHeader, Global.NumToString(Global.CBVNewColumnIndex) + rowColHeader);

            object[,] cellRngVal = new object[3, Global.CBVNewColumnIndex];
            cellRngVal = (object[,])cellRange.Value2;
            for (int x = 1; x <= Global.CBVNewColumnIndex; x++)
            {
                // check  whether the column exists in search range or not
                if (!Global.CellDropdownRange.Contains(x))
                    continue;


                catAlias = DataSetUtilities.GetDataFromCellRange(cellRngVal, rowCatHeader, x).Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT);

                tabAlias = DataSetUtilities.GetDataFromCellRange(cellRngVal, rowTabHeader, x).Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT);

                colAlias = DataSetUtilities.GetDataFromCellRange(cellRngVal, rowColHeader, x).Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT);

                if (string.IsNullOrEmpty(catAlias) || string.IsNullOrEmpty(tabAlias) || string.IsNullOrEmpty(colAlias))
                    continue;

                //11.0.3 - 
                // Lookup by Value fields
                if (COEDataViewData.ISLookupExists(catAlias, tabAlias, colAlias))
                {
                    Global.LookupByValueFields += tabAlias + "." + colAlias + ",";

                }
                string mimeType = COEDataViewData.GetMimeType(colAlias, tabAlias);

                if (Global.ISMimeTypeExists(mimeType))
                    continue;


                //Retrieve the real column (If molecular weight and formula attached with structure column name) 
                colAlias = Global.FieldContainsMolWtMolFm(colAlias);

                string colText = DataSetUtilities.GetDataFromCellRange(cellRngVal, rowColHeader, x);

                colAlias = Global.FieldContainsMolWtMolFm(colAlias);

                if ((!string.IsNullOrEmpty(tabAlias)) && (!string.IsNullOrEmpty(colAlias)))
                {
                    Array.Resize(ref outputdata, outputdata.Length + 1);

                    sb.Remove(0, sb.Length); // Clear the stringbuilder

                    sb.Append(catAlias);
                    sb.Append(".");
                    sb.Append(tabAlias);

                    //Check the conditon for structure search
                    //if (colText.ToUpper().EndsWith(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_MolWeight)))
                    //{
                    //    sb.Append(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_MolWeight));

                    //}
                    //else if (colText.ToUpper().EndsWith(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_Formula)))
                    //{
                    //    sb.Append(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_Formula));

                    //}
                    if ((colText.ToUpper().EndsWith(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_MolWeight))) || (colText.ToUpper().EndsWith(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_Formula))))
                    {
                        sb.Append(".");
                        sb.Append(colText);
                    }
                    else
                    {
                        sb.Append(".");
                        sb.Append(colAlias);
                    }
                    outputdata.SetValue(sb.ToString(), cnt);
                    cnt++;
                }
            }
            return outputdata;
        }


        public string[] GetInputCriteriaMimeType(Excel::Worksheet nSheet)
        {
            string[] searchCriteria = new string[] { };

            int rowCatHeader = (int)Global.CBVHeaderRow.HeaderCategoryRow;
            int rowTabHeader = (int)Global.CBVHeaderRow.HeaderTableRow;
            int rowColHeader = (int)Global.CBVHeaderRow.HeaderColumnRow;
            // int rowWhereHeader = (int)Global.CBVHeaderRow.HeaderWhereRow;
            int rowOptionHeader = (int)Global.CBVHeaderRow.HeaderOptionRow;

            string catAlias = string.Empty;
            string tabAlias = string.Empty;
            string colAlias = string.Empty;

            if (Global.CBVNewColumnIndex == 1)
                Global.CBVNewColumnIndex = 2;

            Excel.Range cellRange = nSheet.get_Range("A" + rowCatHeader, Global.NumToString(Global.CBVNewColumnIndex) + rowOptionHeader);

            object[,] cellRngVal = new object[5, Global.CBVNewColumnIndex];
            cellRngVal = (object[,])cellRange.Value2;

            int cnt = 0; // counter for array value

            StringBuilder sb = new StringBuilder();
            for (int x = 1; x <= Global.CBVNewColumnIndex; x++)
            {
                // check  whether the column exists in search range or not
                if (!Global.CellDropdownRange.Contains(x))
                    continue;

                catAlias = DataSetUtilities.GetDataFromCellRange(cellRngVal, rowCatHeader, x).Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT);
                tabAlias = DataSetUtilities.GetDataFromCellRange(cellRngVal, rowTabHeader, x).Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT);
                colAlias = DataSetUtilities.GetDataFromCellRange(cellRngVal, rowColHeader, x).Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT);

                if (string.IsNullOrEmpty(catAlias) || string.IsNullOrEmpty(tabAlias) || string.IsNullOrEmpty(colAlias))
                    continue;

                //Retrieve the real column (If molecular weight and formula attached with structure column name) 
                colAlias = Global.FieldContainsMolWtMolFm(colAlias);

                bool check = false;
                TableBO baseTable = Global.Tables.GetTable(Global.DataView.Basetable);
                if (baseTable.Alias.Equals(tabAlias, StringComparison.OrdinalIgnoreCase))
                {
                    // if(baseTable.PrimaryKeyName==colAlias)
                    //   return true;
                    foreach (FieldBO field in baseTable.Fields)
                    {
                        if (field.Alias.Equals(colAlias, StringComparison.OrdinalIgnoreCase))
                        {
                            if (field.ID == baseTable.PrimaryKey)
                            {
                                check = true;
                                break;
                            }
                            //    return true;
                            else if (field.IsUniqueKey)
                            {
                                //  return true;
                                check = true;
                                break;
                            }
                        }
                    }
                }
                if (check)
                {

                    Excel::Range nTarget = Global._ExcelApp.Selection as Excel::Range;
                    string searchVal = GlobalCBVExcel.GetCell(nTarget.Row, x, nSheet);

                    if (searchVal.Trim() != String.Empty)
                    {
                        Array.Resize(ref searchCriteria, searchCriteria.Length + 1);
                        sb.Remove(0, sb.Length); // Clear the stringbuilder  
                        sb.Append(tabAlias);
                        sb.Append(".");
                        sb.Append(colAlias);
                        sb.Append(" ");

                        if (!CheckOperator(searchVal.ToUpper().Trim()))
                        {
                            sb.Append("=");
                            sb.Append(searchVal.Trim());
                        }
                        else
                        {
                            sb.Append(searchVal.Trim());
                        }
                        searchCriteria.SetValue(sb.ToString(), cnt);
                        cnt++;
                    }
                    goto res;
                } //end check
            }

        res:
            return searchCriteria;
        }

        public string[] GetOutputCriteriaMimeType(Excel::Worksheet nSheet)
        {
            StringBuilder sb = new StringBuilder();
            string[] outputdata = new string[1];
            //Coverity fix - CID 18707
            if (nSheet == null)
                throw new System.NullReferenceException("Excel sheet cannot be null");
            Excel::Range nTarget = Global._ExcelApp.Application.Selection as Excel::Range;
            //Coverity fix - CID 18707
            if (nTarget == null)
                throw new System.NullReferenceException("Range cannot be null");
            Excel.Range cellRange = nSheet.get_Range(Global.NumToString(nTarget.Column) + (int)Global.CBVHeaderRow.HeaderCategoryRow, Global.NumToString(nTarget.Column) + (int)Global.CBVHeaderRow.HeaderColumnRow);
            if (cellRange == null)
                throw new System.NullReferenceException("Range cannot be null");
            object[,] cellRngHdr = new object[3, 1];
            cellRngHdr = (object[,])cellRange.Value2;

            //catAlias = DataSetUtilities.GetDataFromCellRange(cellRngVal, rowCatHeader, x).Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT);
            string catAlias = cellRngHdr[1, 1].ToString();
            string tabAlias = cellRngHdr[2, 1].ToString();
            string colAlias = cellRngHdr[3, 1].ToString();

            if (string.IsNullOrEmpty(catAlias) || string.IsNullOrEmpty(tabAlias) || string.IsNullOrEmpty(colAlias))
                return null;

            //mimeType = COEDataViewData.GetMimeType(colAlias, tabAlias);

            sb.Append(catAlias);
            sb.Append(".");
            sb.Append(tabAlias);
            sb.Append(".");
            sb.Append(colAlias);

            //Array.Resize(ref outputdata, outputdata.Length + 1);
            outputdata[0] = sb.ToString();
            //outputdata.SetValue(sb.ToString(), cnt);
            return outputdata;
        }

        private string[] GetDomainListData(Excel::Worksheet nSheet)
        {
            //According to the priority, check whether the value exists in unique key columns of domain values

            Global.IsKeyExistForUpdate = false;  //On every domain list the IskeyExistforUpdate is  set to false;
            Global.FirstUniqueKeyInSheet = null;
            string[] domainList = GetUKDomainListData(nSheet);

            //Step1
            if (domainList != null)
            {
                Global.IsUniqueInSearch = true; //Report the current update as search on unique key      
                return domainList;
            }

            //Step2
            domainList = null;
            domainList = GetPKDomainListData(nSheet);

            if (domainList != null)
            {
                Global.IsUniqueInSearch = false; //Report the current update as search not on unique key 
                return domainList;
            }
            Global.IsUniqueInSearch = false; //Report the current update as search not on unique key 
            return null;

        }
        /// <summary>
        /// Retrieve the domain values (corresponding to unique key field)
        /// </summary>
        /// <param name="nSheet"></param>
        /// <returns></returns>
        private string[] GetUKDomainListData(Excel::Worksheet nSheet)
        {
            int cnt = 0; // counter for array value
            string catAlias = string.Empty;
            string tabAlias = string.Empty;
            string colAlias = string.Empty;
            string[] outputdata = null;

            TableBO baseTable = Global.Tables.GetTable(Global.CBVSHEET_COEDATAVIEWBO.COEDataView.Basetable);
            for (int x = 1; x <= Global.CBVNewColumnIndex; x++)
            {
                //check  whether the column exists in search range or not
                if (!Global.CellDropdownRange.Contains(x))
                    continue;

                catAlias = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderCategoryRow, x);
                tabAlias = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderTableRow, x);
                colAlias = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderColumnRow, x);

                if (string.IsNullOrEmpty(catAlias) || string.IsNullOrEmpty(tabAlias) || string.IsNullOrEmpty(colAlias))
                    continue;
                //Retrieve the real column (If molecular weight and formula attached with structure column name)

                if (Global.ISFieldContainsMolWtMolFm(colAlias))
                {
                    string[] colAliasMolwtFormula = colAlias.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    if (colAliasMolwtFormula.Length == 2)
                        colAlias = colAliasMolwtFormula[0];
                }

                //Verify the sheet database and table with Dataview database and table.
                if (baseTable.DataBase.Trim().Equals(catAlias.Trim(), StringComparison.OrdinalIgnoreCase) && baseTable.Alias.Trim().Equals(tabAlias.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    for (int fld = 0; fld < baseTable.Fields.Count; fld++)
                    {
                        if (baseTable.Fields[fld].IsUniqueKey)
                        {

                            if (colAlias.Equals(baseTable.Fields[fld].Alias, StringComparison.OrdinalIgnoreCase))
                            {
                                for (int row = Convert.ToInt32(Global.CBVHeaderRow.HeaderResultStartupRow); row <= nSheet.UsedRange.Rows.Count; row++)
                                {
                                    Excel.Range cell = (Excel.Range)nSheet.Cells[row, x];

                                    if (string.IsNullOrEmpty(cell.Text.ToString()))
                                        continue;
                                    Array.Resize(ref outputdata, cnt + 1);
                                    outputdata.SetValue(cell.Text.ToString(), cnt++);

                                }
                                Global.IsKeyExistForUpdate = true; //Report the unique key or primary key exists for update
                                Global.FirstUniqueKeyInSheet = colAlias;
                                return outputdata; //Return if values exists - priorities (left to right)
                            }
                        }
                    }
                }
            }
            return outputdata;
        }
        /// <summary>
        /// Retrieve the domain values (corresponding to primary key field)
        /// </summary>
        /// <param name="nSheet"></param>
        /// <returns></returns>

        public string[] GetPKDomainListData(Excel::Worksheet nSheet)
        {
            int cnt = 0; // counter for array value
            string catAlias = string.Empty;
            string tabAlias = string.Empty;
            string colAlias = string.Empty;
            string[] outputdata = null;

            TableBO baseTable = Global.Tables.GetTable(Global.CBVSHEET_COEDATAVIEWBO.COEDataView.Basetable);
            string pkAlias = COEDataViewData.GetPrimaryKey(Global.CBVSHEET_COEDATAVIEWBO.COEDataView);

            for (int x = 1; x <= Global.CBVNewColumnIndex; x++)
            {
                //check  whether the column exists in search range or not
                if (!Global.CellDropdownRange.Contains(x))
                    continue;

                catAlias = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderCategoryRow, x);
                tabAlias = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderTableRow, x);
                colAlias = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderColumnRow, x);

                if (string.IsNullOrEmpty(catAlias) || string.IsNullOrEmpty(tabAlias) || string.IsNullOrEmpty(colAlias))
                    continue;
                //Retrieve the real column (If molecular weight and formula attached with structure column name)
                if (Global.ISFieldContainsMolWtMolFm(colAlias))
                {
                    string[] colAliasMolwtFormula = colAlias.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    if (colAliasMolwtFormula.Length == 2)
                        colAlias = colAliasMolwtFormula[0];
                }
                //Verify the sheet database and table with Dataview database and table.
                if (baseTable.DataBase.Trim().Equals(catAlias.Trim(), StringComparison.OrdinalIgnoreCase) && baseTable.Alias.Trim().Equals(tabAlias.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    // check whether the value exists in primary key columns for domain values
                    for (int fld = 0; fld < baseTable.Fields.Count; fld++)
                    {
                        if (pkAlias.Equals(baseTable.Fields[fld].Alias, StringComparison.OrdinalIgnoreCase))
                        {
                            if (colAlias.Equals(baseTable.Fields[fld].Alias, StringComparison.OrdinalIgnoreCase))
                            {
                                for (int row = Convert.ToInt32(Global.CBVHeaderRow.HeaderResultStartupRow); row <= nSheet.UsedRange.Rows.Count; row++)
                                {
                                    Excel.Range cell = (Excel.Range)nSheet.Cells[row, x];
                                    if (string.IsNullOrEmpty(cell.Text.ToString()))
                                        continue;

                                    Array.Resize(ref outputdata, cnt + 1);
                                    outputdata.SetValue(cell.Text.ToString(), cnt++);
                                }
                                Global.IsKeyExistForUpdate = true; //Report the unique key or primary key exists for update
                                return outputdata; //Return if values exists
                            }
                        }
                    }
                }
            }
            return outputdata;
        }

        //Identify the CBV sheet contains Primary key or unique key exists

        public bool ISPrimaryKeyORUniqueKeyExists(Excel::Worksheet nSheet)
        {
            string catAlias = string.Empty;
            string tabAlias = string.Empty;
            string colAlias = string.Empty;

            TableBO baseTable = Global.Tables.GetTable(Global.CBVSHEET_COEDATAVIEWBO.COEDataView.Basetable);
            string pkAlias = COEDataViewData.GetPrimaryKey(Global.CBVSHEET_COEDATAVIEWBO.COEDataView);

            for (int x = 1; x <= Global.CBVNewColumnIndex; x++)
            {
                //check  whether the column exists in search range or not
                if (!Global.CellDropdownRange.Contains(x))
                    continue;

                catAlias = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderCategoryRow, x);
                tabAlias = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderTableRow, x);
                colAlias = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderColumnRow, x);

                if (string.IsNullOrEmpty(catAlias) || string.IsNullOrEmpty(tabAlias) || string.IsNullOrEmpty(colAlias))
                    continue;
                //Retrieve the real column (If molecular weight and formula attached with structure column name)
                if (Global.ISFieldContainsMolWtMolFm(colAlias))
                {
                    string[] colAliasMolwtFormula = colAlias.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    if (colAliasMolwtFormula.Length == 2)
                        colAlias = colAliasMolwtFormula[0];
                }
                //Verify the sheet database and table with Dataview database and table.
                if (baseTable.DataBase.Trim().Equals(catAlias.Trim(), StringComparison.OrdinalIgnoreCase) && baseTable.Alias.Trim().Equals(tabAlias.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    for (int fld = 0; fld < baseTable.Fields.Count; fld++)
                    {
                        if (baseTable.Fields[fld].IsUniqueKey)
                        {
                            if (colAlias.Equals(baseTable.Fields[fld].Alias, StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }
                        }
                        if (colAlias.Equals(pkAlias, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        // 11.0.4 - Getting the values of the column to be displayed in the structure cell
        public object[,] GetColValToBeDisplayInStruct(DataTable datatable)
        {

            string primaryKey = string.Empty;
            string uniqueKey = string.Empty;

            string catAlias = string.Empty;
            string tabAlias = string.Empty;
            string colAlias = string.Empty;

            TableBO baseTable = Global.Tables.GetTable(Global.CBVSHEET_COEDATAVIEWBO.COEDataView.Basetable);
            string pkAlias = COEDataViewData.GetPrimaryKey(Global.CBVSHEET_COEDATAVIEWBO.COEDataView);

            for (int x = 1; x <= Global.CBVNewColumnIndex; x++)
            {
                //check  whether the column exists in search range or not
                if (!Global.CellDropdownRange.Contains(x))
                    continue;

                catAlias = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderCategoryRow, x);
                tabAlias = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderTableRow, x);
                colAlias = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderColumnRow, x);

                if (string.IsNullOrEmpty(catAlias) || string.IsNullOrEmpty(tabAlias) || string.IsNullOrEmpty(colAlias))
                    continue;

                //Retrieve the real column (If molecular weight and formula attached with structure column name)
                if (Global.ISFieldContainsMolWtMolFm(colAlias))
                {
                    string[] colAliasMolwtFormula = colAlias.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    if (colAliasMolwtFormula.Length == 2)
                        colAlias = colAliasMolwtFormula[0];
                }

                //Verify the sheet database and table with Dataview database and table.
                if (baseTable.DataBase.Trim().Equals(catAlias.Trim(), StringComparison.OrdinalIgnoreCase) && baseTable.Alias.Trim().Equals(tabAlias.Trim(), StringComparison.OrdinalIgnoreCase))
                {

                    for (int fld = 0; fld < baseTable.Fields.Count; fld++)
                    {
                        // Getting the first Unique Key
                        if (baseTable.Fields[fld].IsUniqueKey)
                        {
                            if (colAlias.Equals(baseTable.Fields[fld].Alias, StringComparison.OrdinalIgnoreCase))
                            {
                                uniqueKey = colAlias;
                                break;
                            }
                        }

                        // Getting the Primary Key
                        if (colAlias.Equals(pkAlias, StringComparison.OrdinalIgnoreCase))
                        {
                            primaryKey = colAlias;
                        }

                    }

                }

                if (!string.IsNullOrEmpty(uniqueKey))
                    break;
            }

            // Getting the column to be displayed in structure cell
            string colToDisplayInStruct = string.IsNullOrEmpty(uniqueKey) ? primaryKey : uniqueKey;

            object[,] cellRngValcolToDisplayInStruct = new object[datatable.Rows.Count, 1];

            // Getting the rows of the column to be displayed in structure cell
            for (int colIndex = 0; colIndex < datatable.Columns.Count; colIndex++)
            {
                if (datatable.Columns[colIndex].ColumnName.ToUpper().Contains(Global.COLUMN_SEPARATOR + colToDisplayInStruct.ToUpper() + Global.COLUMN_SEPARATOR) && datatable.Columns[colIndex].ColumnName.ToUpper().Contains(baseTable.Alias.ToUpper() + Global.COLUMN_SEPARATOR))
                {
                    for (int rowIndex = 0; rowIndex < datatable.Rows.Count; rowIndex++)
                    {
                        // 11.0.4 - If structure content is empty (as in case of split cell) then don't add in array to show "structure" text in the structure column
                        if (!string.IsNullOrEmpty(Convert.ToString(datatable.Rows[rowIndex][colIndex])))
                        {
                            cellRngValcolToDisplayInStruct[rowIndex, 0] = Global.STRUCTURE + " " + datatable.Rows[rowIndex][colIndex].ToString();
                        }
                    }

                    break;
                }
            }


            return cellRngValcolToDisplayInStruct;

        }


        public int GetCBVNewCategoryIndex(Excel::Worksheet nSheet)
        {
            int cnt = 1; // counter for array value  
            //If CBVActiveColumnIndex set to 0;
            if (Global.CBVActiveColumnIndex == 0)
                cnt = 0;
            return (nSheet.UsedRange.Columns.Count + cnt);
        }

        #endregion "_ Data Searching _"

        #region "_ Display Result _"

        private void DispalyResult(DataSet resultsDataSet, Excel::Worksheet nSheet, Global.CBVSearch CBVSearchType)
        {
            string strTemp = string.Empty;
            string fldRelationship = string.Empty;
            string bTable = string.Empty;
            DataTable bDataTable = null;

            bTable = resultsDataSet.Tables[0].TableName.ToString();
            bDataTable = resultsDataSet.Tables[0];

            //Create field relationship on the basis of maximum rows table
            for (int x = 1; x <= Global.CBVNewColumnIndex; x++)
            {
                string getParm = GetParameter(x, nSheet, resultsDataSet, bTable, true);

                // 11.0.3
                // Commented as blank value is required to show the null value columns, otherwise they will not be displayed on excel sheet
                //if (!string.IsNullOrEmpty(getParm))
                // {
                fldRelationship = fldRelationship + "," + getParm;
                // }

            }
            //If the string have started with comma then Remove the comma.
            if (fldRelationship.StartsWith(","))
                fldRelationship = fldRelationship.Trim(Global.charsToTrim).Substring(1, fldRelationship.Length - 1);
            if (fldRelationship.EndsWith(","))
                fldRelationship = fldRelationship.Trim(Global.charsToTrim).Substring(0, fldRelationship.Length - 1);
            DataSet dataSet = resultsDataSet.Copy();
            DataSetUtilities dsUtil = new DataSetUtilities(ref dataSet);

            dsUtil.ValidateFieldList(fldRelationship);
            //Create view/schema for dispalying data.
            dsUtil.CreateView("DisplayTable", bDataTable, fldRelationship, nSheet);
            //Convert the integer data type column into string data type column
            dsUtil.ConvertIntToStringDataColum(dsUtil.ds.Tables["DisplayTable"]);

            //Create field relationship on the basis of base rows table
            fldRelationship = string.Empty;
            for (int x = 1; x <= Global.CBVNewColumnIndex; x++)
            {
                string getParm = GetParameter(x, nSheet, resultsDataSet, bTable, true);

                // 11.0.3
                // Commented as blank value is required to show the null value columns, otherwise they will not be displayed on excel sheet
                //if (!string.IsNullOrEmpty(getParm))
                //{
                fldRelationship = fldRelationship + "," + getParm;
                //}
            }

            //If the string have started with comma then Remove the comma.
            if (fldRelationship.StartsWith(","))
                fldRelationship = fldRelationship.Trim(Global.charsToTrim).Substring(1, fldRelationship.Length - 1);
            if (fldRelationship.EndsWith(","))
                fldRelationship = fldRelationship.Trim(Global.charsToTrim).Substring(0, fldRelationship.Length - 1);
            if (string.IsNullOrEmpty(fldRelationship))
                throw new ArgumentException(Properties.Resources.excepFldRelationship);
            // Remove the fields which not exists in base data table. 
            // The COESearch eliminates the null data while searching
            if (bDataTable.Rows.Count > 0)
            {
                //Add the CBV sheet column index into global variable which have split option
                AddColIndexIntoIndexList(nSheet);

                DataTable dispTable = dsUtil.GetCBVDataFromBaseTable(dsUtil.ds.Tables["DisplayTable"], resultsDataSet.Tables[0], fldRelationship, nSheet, CBVSplitCellsExists(nSheet), CBVMultipleRowsExists(nSheet));

                if (CBVSearchType.Equals(Global.CBVSearch.ReplaceAllResults))
                {
                    SetCell((int)Global.CBVHeaderRow.HeaderResultStartupRow, 1, nSheet, dispTable, false, true);
                    Global._CBVSearchResult = dispTable.Copy();
                    Global.MaxRecordInResultSet = dispTable.Rows.Count;

                    //11.0.3
                    UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.MaxResultCount, Global.CurrentWorkSheetName, Global.MaxRecordInResultSet.ToString());

                    //11.0.3
                    //Store search schema into hidden sheet in serialize form.
                    UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.SerializeCBVResult, Global.CurrentWorkSheetName, SerializeDeserialize.Serialize(dispTable.Clone()));
                    //11.0.3
                    UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.SearchUpdateCriteria, Global.CurrentWorkSheetName, SerializeDeserialize.Serialize(Global.SearchUpdateCriteria));
                }
                else if (CBVSearchType.Equals(Global.CBVSearch.AppendNewResults))
                {
                    SetCell((int)Global.CBVHeaderRow.HeaderResultStartupRow + Global.MaxRecordInResultSet, 1, nSheet, dispTable, false, false);
                    if (Global._CBVSearchResult == null)
                        Global._CBVSearchResult = new DataTable();
                    Global._CBVSearchResult.Merge(dispTable);
                    Global.MaxRecordInResultSet = Global.MaxRecordInResultSet + dispTable.Rows.Count;
                    //11.0.3
                    UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.MaxResultCount, Global.CurrentWorkSheetName, Global.MaxRecordInResultSet.ToString());

                    //11.0.3
                    //Store search schema into hidden sheet in serialize form.
                    UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.SerializeCBVResult, Global.CurrentWorkSheetName, SerializeDeserialize.Serialize(dispTable.Clone()));
                    //11.0.3
                    UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.SearchUpdateCriteria, Global.CurrentWorkSheetName, SerializeDeserialize.Serialize(Global.SearchUpdateCriteria));
                }
                else if (CBVSearchType.Equals(Global.CBVSearch.UpdateCurrentResults))
                {
                    SetCell((int)Global.CBVHeaderRow.HeaderResultStartupRow, 1, nSheet, dispTable, false, true);
                    Global._CBVSearchResult = dispTable.Copy();
                    Global.MaxRecordInResultSet = dispTable.Rows.Count;
                    //11.0.3
                    UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.MaxResultCount, Global.CurrentWorkSheetName, Global.MaxRecordInResultSet.ToString());

                    //11.0.3
                    //Store search schema into hidden sheet in serialize form.
                    UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.SerializeCBVResult, Global.CurrentWorkSheetName, SerializeDeserialize.Serialize(dispTable.Clone()));

                }
                ClearColIndexIntoIndexList();
            }
        }

        #endregion "_ Display Result _"

        public string GetParameter(int x, Excel::Worksheet nSheet, DataSet ds, string tableName, bool realTableName)
        {
            COEDataView dataView = Global.CBVSHEET_COEDATAVIEWBO.COEDataView;
            String baseTableName = string.Empty;
            String thisTableName = string.Empty;

            if (ds.Tables.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                baseTableName = ds.Tables[0].TableName;
                CriteriaUtilities criteriaUtil = new CriteriaUtilities();

                thisTableName = criteriaUtil.GetHeaderTableInfo(x, nSheet, Global.ALIAS);

                if (tableName.Trim().Equals(thisTableName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    string columnInfoAlias = criteriaUtil.GetHeaderColumnInfo(x, nSheet, Global.ALIAS);

                    if (!string.IsNullOrEmpty(columnInfoAlias))
                    {
                        return columnInfoAlias.Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT).Replace(Global.COMMA_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_COMMA);
                    }
                    else
                    {
                        string columnInfoName = criteriaUtil.GetHeaderColumnInfo(x, nSheet, Global.NAME);
                        // 11.0.4 - Replacing dot and comma with escape charaters
                        //return columnInfoName;
                        return columnInfoName.Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT).Replace(Global.COMMA_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_COMMA);
                    }
                }
                else
                {
                    string pTable = string.Empty;
                    string cTable = string.Empty;

                    string returnValue = string.Empty;

                    foreach (DataRelation dr in ds.Relations)
                    {
                        pTable = dr.ParentTable.TableName.ToString();
                        cTable = dr.ChildTable.TableName.ToString();

                        if (baseTableName == thisTableName)
                        {
                            if ((pTable == baseTableName) && (cTable == tableName))
                            {
                                string columnInfoAlias = criteriaUtil.GetHeaderColumnInfo(x, nSheet, Global.ALIAS);
                                if (!string.IsNullOrEmpty(columnInfoAlias))
                                {
                                    returnValue = columnInfoAlias.Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT).Replace(Global.COMMA_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_COMMA);
                                }
                                else
                                {

                                    returnValue = criteriaUtil.GetHeaderColumnInfo(x, nSheet, Global.NAME);
                                }
                                // 11.0.4 - Replacing dot and comma with escape charaters
                                //return dr.RelationName + "." + returnValue;
                                return dr.RelationName.Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT).Replace(Global.COMMA_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_COMMA) + "." + returnValue;
                            }
                        }
                        else
                        {
                            if ((pTable == baseTableName) && (cTable == thisTableName))
                            {
                                string columnInfoAlias = criteriaUtil.GetHeaderColumnInfo(x, nSheet, Global.ALIAS);
                                if (!string.IsNullOrEmpty(columnInfoAlias))
                                {
                                    returnValue = columnInfoAlias.Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT).Replace(Global.COMMA_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_COMMA);
                                }
                                else
                                {
                                    returnValue = criteriaUtil.GetHeaderColumnInfo(x, nSheet, Global.NAME);
                                }

                                // 11.0.4 - Replacing the "." with "\n" from RelationName (i.e table alias name)
                                //return dr.RelationName + "." + returnValue;
                                return dr.RelationName.Replace(Global.DOT_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_DOT).Replace(Global.COMMA_SEPARATOR, Global.ESCAPE_SEPERATOR_FOR_COMMA) + "." + returnValue;
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }

        public bool CBVSplitExists(object sheet, int activeCol)
        {
            Excel::Worksheet nSheet = sheet as Excel::Worksheet;
            //Coverity fix - CID 18700
            if (nSheet == null)
                throw new System.NullReferenceException("Sheet cannot be null");
            Excel.Range cell = (Excel.Range)nSheet.Cells[(int)Global.CBVHeaderRow.HeaderWhereRow, activeCol];
            //Coverity fix - CID 18700
            if (cell == null)
                throw new System.NullReferenceException("Range cannot be null");
            string optionHeader = string.Empty;
            string headerColumnIndextype = string.Empty;
            string headerColumnMimetype = string.Empty;

            try
            {
                CriteriaUtilities criteriaUtilities = new CriteriaUtilities();
                optionHeader = ((Excel::Range)nSheet.Cells[(int)Global.CBVHeaderRow.HeaderOptionRow, activeCol]).Value2.ToString();
                headerColumnIndextype = criteriaUtilities.GetHeaderColumnInfo(activeCol, sheet, Global.INDEXTYPE);
                headerColumnMimetype = criteriaUtilities.GetHeaderColumnInfo(activeCol, sheet, Global.MIMITYPE);


                if (optionHeader.ToUpper().Contains("100%") || optionHeader.ToUpper().Contains("75%") || optionHeader.ToUpper().Contains("50%") || optionHeader.ToUpper().Contains("SPLIT") || optionHeader.ToUpper().Contains("MULTIPLE") || optionHeader.ToUpper().Contains("A-Z") || optionHeader.ToUpper().Contains("Z-A") || ((headerColumnIndextype.Equals(Global.COESTRUCTURE_INDEXTYPE, StringComparison.OrdinalIgnoreCase) || Global.ISImageTypeExists(headerColumnMimetype)) && (!string.IsNullOrEmpty(GlobalCBVExcel.GetCell(Global.CBVHeaderRow.HeaderWhereRow, activeCol, nSheet)))))
                    return true;

                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CBVStructureExists(object sheet, int activeCol)
        {
            Excel::Worksheet nSheet = sheet as Excel::Worksheet;
            if (nSheet == null)
                throw new System.NullReferenceException("Sheet cannot be null");
            Excel.Range cell = (Excel.Range)nSheet.Cells[(int)Global.CBVHeaderRow.HeaderWhereRow, activeCol];
            //Coverity fix - CID 18701
            if (cell == null)
                throw new System.NullReferenceException("Range cannot be null");
            string optionHeader = string.Empty;
            string headerColumnIndextype = string.Empty;
            // string headerColumnMimetype = string.Empty;

            try
            {
                CriteriaUtilities criteriaUtilities = new CriteriaUtilities();
                optionHeader = ((Excel::Range)nSheet.Cells[(int)Global.CBVHeaderRow.HeaderOptionRow, activeCol]).Value2.ToString();
                headerColumnIndextype = criteriaUtilities.GetHeaderColumnInfo(activeCol, sheet, Global.INDEXTYPE);


                if (headerColumnIndextype.Equals(Global.COESTRUCTURE_INDEXTYPE, StringComparison.OrdinalIgnoreCase) && (!string.IsNullOrEmpty(GlobalCBVExcel.GetCell(Global.CBVHeaderRow.HeaderWhereRow, activeCol, nSheet))))

                    return true;

                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // This function checks the split option exists or not on search data. As well consider the empty columns - 

        public bool CBVSplitCellsExists(object sheet)
        {
            Excel::Worksheet nSheet = sheet as Excel::Worksheet;
            string optionHeader = string.Empty;
            //Coverity fix - CID 18698
            if (nSheet == null)
                throw new System.NullReferenceException("Sheet cannot be null");
            int colCnt = nSheet.UsedRange.Columns.Count;
            object[,] cellRngVal = new object[1, colCnt];
            //List<object>[,] cellRngVal = new List<object>[rowCnt, colCnt];

            if (colCnt == 1)
                colCnt = 2;


            Excel.Range cellRange = nSheet.get_Range("A" + (int)Global.CBVHeaderRow.HeaderOptionRow, Global.NumToString(colCnt) + (int)Global.CBVHeaderRow.HeaderOptionRow);
            cellRngVal = (object[,])cellRange.Value2;

            for (int x = 1; x <= Global.CBVNewColumnIndex; x++)
            {
                try
                {
                    optionHeader = DataSetUtilities.GetDataFromCellRange(cellRngVal, 1, x);
                    if (string.IsNullOrEmpty(optionHeader))
                        continue;

                    foreach (Global.CBVSplitHeaderOptions splitOption in Enum.GetValues(typeof(Global.CBVSplitHeaderOptions)))
                    {
                        if (optionHeader.Equals(StringEnum.GetStringValue(splitOption)))
                        {
                            Global.CBVSplitColIndexList.Add(x);
                            return true;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }


        public bool CBVMultipleRowsExists(object sheet)
        {
            Excel::Worksheet nSheet = sheet as Excel::Worksheet;
            string optionHeader = string.Empty;
            //Coverity fix - CID 18697
            if (nSheet == null)
                throw new System.NullReferenceException("Sheet cannot be null");
            int colCnt = nSheet.UsedRange.Columns.Count;
            object[,] cellRngVal = new object[1, colCnt];

            if (colCnt == 1)
                colCnt = 2;

            Excel.Range cellRange = nSheet.get_Range("A" + (int)Global.CBVHeaderRow.HeaderOptionRow, Global.NumToString(colCnt) + (int)Global.CBVHeaderRow.HeaderOptionRow);
            cellRngVal = (object[,])cellRange.Value2;

            for (int x = 1; x <= Global.CBVNewColumnIndex; x++)
            {
                try
                {
                    optionHeader = DataSetUtilities.GetDataFromCellRange(cellRngVal, 1, x);
                    if (string.IsNullOrEmpty(optionHeader))
                        continue;

                    foreach (Global.CBVMultipleHeaderOptions multipleOption in Enum.GetValues(typeof(Global.CBVMultipleHeaderOptions)))
                    {
                        if (optionHeader.Equals(StringEnum.GetStringValue(multipleOption)))
                        {
                            Global.CBVMultipleColIndexList.Add(x);
                            return true;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        // This function checks the split/multiple option exists or not on search data. As well as consider the empty columns

        public bool CBVSplitCellsMultipleRowsExists(object sheet)
        {
            Excel::Worksheet nSheet = sheet as Excel::Worksheet;
            string optionHeader = string.Empty;
            //Coverity fix - CID 18699
            if (nSheet == null)
                throw new System.NullReferenceException("Sheet cannot be null");
            int colCnt = nSheet.UsedRange.Columns.Count;
            object[,] cellRngVal = new object[1, colCnt];

            if (colCnt == 1)
                colCnt = 2;

            Excel.Range cellRange = nSheet.get_Range("A" + (int)Global.CBVHeaderRow.HeaderOptionRow, Global.NumToString(colCnt) + (int)Global.CBVHeaderRow.HeaderOptionRow);
            cellRngVal = (object[,])cellRange.Value2;


            for (int x = 1; x <= Global.CBVNewColumnIndex; x++)
            {
                try
                {
                    optionHeader = DataSetUtilities.GetDataFromCellRange(cellRngVal, 1, x);

                    if (string.IsNullOrEmpty(optionHeader))
                        continue;

                    foreach (Global.CBVSplitHeaderOptions splitOption in Enum.GetValues(typeof(Global.CBVSplitHeaderOptions)))
                    {
                        if (optionHeader.Equals(StringEnum.GetStringValue(splitOption)))
                        {
                            Global.CBVSplitColIndexList.Add(x);
                            return true;
                        }
                    }
                    foreach (Global.CBVMultipleHeaderOptions multipleOption in Enum.GetValues(typeof(Global.CBVMultipleHeaderOptions)))
                    {
                        if (optionHeader.Equals(StringEnum.GetStringValue(multipleOption)))
                        {
                            Global.CBVMultipleColIndexList.Add(x);
                            return true;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        public void AddColIndexIntoIndexList(object sheet)
        {
            Excel::Worksheet nSheet = sheet as Excel::Worksheet;
            //Coverity fix - CID 18708
            if (nSheet == null)
                throw new System.NullReferenceException("Excel sheet cannot be null");
            Global.CBVSplitColIndexList = new ArrayList();
            Global.CBVMultipleColIndexList = new ArrayList();
            Global.CBVResultColASC = new ArrayList();
            Global.CBVResultColDESC = new ArrayList();

            int colCnt = nSheet.UsedRange.Columns.Count;
            object[,] cellRngVal = new object[1, colCnt];

            if (colCnt == 1)
                colCnt = 2;

            Excel.Range cellRange = nSheet.get_Range("A" + (int)Global.CBVHeaderRow.HeaderOptionRow, Global.NumToString(colCnt) + (int)Global.CBVHeaderRow.HeaderOptionRow);
            cellRngVal = (object[,])cellRange.Value2;

            for (int x = 1; x <= Global.CBVNewColumnIndex; x++)
            {
                try
                {

                    string optionHeader = DataSetUtilities.GetDataFromCellRange(cellRngVal, 1, x);
                    if (string.IsNullOrEmpty(optionHeader))
                        continue;

                    foreach (Global.CBVSplitHeaderOptions splitOption in Enum.GetValues(typeof(Global.CBVSplitHeaderOptions)))
                    {
                        if (optionHeader.Equals(StringEnum.GetStringValue(splitOption)))
                        {
                            Global.CBVSplitColIndexList.Add(x);
                        }
                    }
                    foreach (Global.CBVMultipleHeaderOptions multipleOption in Enum.GetValues(typeof(Global.CBVMultipleHeaderOptions)))
                    {
                        if (optionHeader.Equals(StringEnum.GetStringValue(multipleOption)))
                        {
                            Global.CBVMultipleColIndexList.Add(x);
                        }
                    }
                    foreach (Global.CBV_ALL_ASC_HeaderOptions ASCOption in Enum.GetValues(typeof(Global.CBV_ALL_ASC_HeaderOptions)))
                    {
                        if (optionHeader.Equals(StringEnum.GetStringValue(ASCOption)))
                        {
                            Global.CBVResultColASC.Add(x);
                        }
                    }
                    foreach (Global.CBV_All_DESC_HeaderOptions DESCOption in Enum.GetValues(typeof(Global.CBV_All_DESC_HeaderOptions)))
                    {
                        if (optionHeader.Equals(StringEnum.GetStringValue(DESCOption)))
                        {
                            Global.CBVResultColDESC.Add(x);
                        }
                    }

                }
                catch (Exception ex)
                {
                    CBVExcel.ErrorLogging(ex.Message);
                }
            }
        }


        public void ClearColIndexIntoIndexList()
        {
            Global.CBVSplitColIndexList.Clear();
            Global.CBVMultipleColIndexList.Clear();
            Global.CBVResultColASC.Clear();
            Global.CBVResultColDESC.Clear();
        }

        public bool ISCBVSplitColIndexExists(int index)
        {
            if (Global.CBVSplitColIndexList.Contains(index))
                return true;
            else
                return false;
        }

        public bool ISCBVMultipleColIndexExists(int index)
        {
            if (Global.CBVMultipleColIndexList.Contains(index))
                return true;
            else
                return false;
        }

        public string CBVColOrderBy(int index)
        {
            if (Global.CBVResultColASC.Contains(index))
                return "ASC";
            if (Global.CBVResultColDESC.Contains(index))
                return "DESC";
            else
                return string.Empty;
        }

        #endregion "_ CBV Search functions _"


        #region _ CBV Options _

        #region "Validate Current CBV Sheet"

        public int VerifyCurrentCBVSheet(object sheet)
        {
            string category = string.Empty;
            string tabAlias = string.Empty;
            string colAlias = string.Empty;
            bool ISValidSheet = true;

            for (int x = 1; x <= Global.CBVNewColumnIndex; x++)
            {
                category = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderCategoryRow, x);
                tabAlias = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderTableRow, x);
                colAlias = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderColumnRow, x);

                //Retrieve the real column (If molecular weight and formula attached with structure column name)
                if (Global.ISFieldContainsMolWtMolFm(colAlias))
                {
                    string[] colAliasMolwtFormula = colAlias.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    if (colAliasMolwtFormula.Length == 2)
                        colAlias = colAliasMolwtFormula[0];
                }

                if ((!string.IsNullOrEmpty(category)) && (!string.IsNullOrEmpty(tabAlias)) && (!string.IsNullOrEmpty(colAlias)))
                {
                    ISValidSheet = COEDataViewData.ISValidFields(category, tabAlias, colAlias);

                    if (!ISValidSheet)
                        return x;
                }
            }
            return 0;
        }

        #endregion "Validate Current CBV Sheet"

        #region Create list from Selection

        public void CreateListfromSelection(Excel.Application nActiveApp)
        {
            StringBuilder rngData = new StringBuilder();
            try
            {
                string nAddress = nActiveApp.ActiveCell.get_Address(Type.Missing, Type.Missing, Excel.XlReferenceStyle.xlA1, Type.Missing, Type.Missing);
                object nRange = nActiveApp.InputBox("Select the Range of Cells", "Make List in " + nAddress, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, 8);

                if (nRange != null)
                {
                    object[,] nRangeData = ((Excel.Range)nRange).get_Value(Excel.XlRangeValueDataType.xlRangeValueDefault) as object[,];
                    if (nRangeData == null)
                        throw new System.NullReferenceException();

                    foreach (object data in nRangeData)
                    {
                        if (data != null)
                            rngData.Append(data.ToString() + Global.LIST_SELECTION_SEPARATOR);
                    }
                    string mergeData = rngData.ToString().Substring(0, rngData.Length - 1);
                    nActiveApp.ActiveCell.Value2 = mergeData;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region "Clear Current Results"

        /// <summary>
        /// Clear the chemoffice sheet result (by identification of image)
        /// </summary>
        /// <param name="nSheet"></param>
        public void ClearResults(object sheet)
        {
            Excel::Worksheet nSheet = sheet as Excel::Worksheet;
            //Coverity fix - CID 18710
            if (nSheet == null)
                throw new System.NullReferenceException("Excel sheet cannot be null");

            if (Global.MaxRecordInResultSet > 0)
            {
                int maxColcnt = nSheet.UsedRange.Columns.Count;
                if (maxColcnt < Global.CellDropdownRange.Count)
                    maxColcnt = Global.CellDropdownRange.Count;

                for (int col = 1; col <= maxColcnt; col++)
                {
                    if (!Global.CellDropdownRange.Contains(col))
                    {
                        continue;
                    }
                    else if (!CriteriaUtilities.ISHeaderColumnInfoExists(col, sheet, Global.NAME))
                    {
                        continue;
                    }

                    Excel.Range nRange = nSheet.get_Range(Global.NumToString(col) + (int)Global.CBVHeaderRow.HeaderResultStartupRow, Global.NumToString(col) + ((int)Global.CBVHeaderRow.HeaderColumnLabelDisplayRow + Global.MaxRecordInResultSet));

                    object nRngTop = null;
                    object shpTop = null;

                    try
                    {
                        foreach (Excel.Shape shpObject in nSheet.Shapes)
                        {
                            nRngTop = nRange.Top;
                            shpTop = shpObject.Top;
                            string picname = shpObject.Name.ToString();
                            //if (shpObject.Name.StartsWith(Global.ImageNameStartWith) && Convert.ToInt32(shpTop) >= Convert.ToInt32(nRngTop))
                            if (shpObject.Name.Contains(Global.ImageNameStartWith) && Convert.ToInt32(shpTop) >= Convert.ToInt32(nRngTop))
                                //Another alternate
                                //if ((shpObject.Name.StartsWith(Global.ImageNameStartWith) || (shpObject.Name.StartsWith(nSheet.Name+Global.ImageNameStartWith)) && Convert.ToInt32(shpTop) >= Convert.ToInt32(nRngTop)))
                                shpObject.Delete();
                        }
                        nRange.Clear();

                        if (Global.ToggleColumnAutoSize == 1)
                            nRange.EntireColumn.AutoFit();

                        nRange.EntireRow.AutoFit();
                    }
                    catch (Exception ex)
                    {
                        Global._ExcelApp.ScreenUpdating = true;
                        throw ex;
                    }
                }//End for loop

                Global.MaxRecordInResultSet = 0;
            }

        }

        public void ClearResultsOnDataViewUpdate(object sheet, List<int> columnIndex)
        {
            Excel::Worksheet nSheet = sheet as Excel::Worksheet;
            //Coverity fix - CID 18711
            if (nSheet == null)
                throw new System.NullReferenceException("Excel sheet cannot be null");
            if (Global.MaxRecordInResultSet > 0)
            {
                for (int col = 1; col <= nSheet.UsedRange.Columns.Count; col++)
                {
                    if (!Global.CellDropdownRange.Contains(col))
                        continue;
                    else if (!CriteriaUtilities.ISHeaderColumnInfoExists(col, sheet, Global.NAME)) continue;
                    //If Image type exists in column 
                    if (columnIndex.Contains(col))
                        continue;
                    Excel.Range nRange = nSheet.get_Range(Global.NumToString(col) + (int)Global.CBVHeaderRow.HeaderResultStartupRow, Global.NumToString(col) + ((int)Global.CBVHeaderRow.HeaderColumnLabelDisplayRow + Global.MaxRecordInResultSet));
                    try
                    {
                        nRange.Clear();

                        if (Global.ToggleColumnAutoSize == 1)
                            nRange.EntireColumn.AutoFit();
                    }
                    catch (Exception ex)
                    {
                        Global._ExcelApp.ScreenUpdating = true;
                        throw ex;
                    }
                }
            }
        }

        public void ClearSearchCriteria(object sheet)
        {
            Excel::Worksheet nSheet = sheet as Excel::Worksheet;
            //Coverity fix - CID 18712
            if (nSheet == null)
                throw new System.NullReferenceException("Excel sheet cannot be null");
            Excel.Range nRange = nSheet.get_Range(Global.NumToString((int)Global.CBVHeaderRow.HeaderCategoryRow) + (int)Global.CBVHeaderRow.HeaderWhereRow, Global.NumToString((int)nSheet.UsedRange.Columns.Count) + ((int)Global.CBVHeaderRow.HeaderWhereRow));
            try
            {
                nRange.ClearContents();
                foreach (Excel.Shape shpObject in nSheet.Shapes)
                {
                    try
                    {
                        //Excel.Range nTopLeftRange = shpObject.TopLeftCell;

                        int criteriaRow = (int)shpObject.TopLeftCell.Cells.Row;
                        string picname = shpObject.Name.ToString();
                        // 11.0.4 - Instead of checking the shape name that contains "Picture" text anywhere in the name instead of checking it at the start
                        // as the shape name format has been changed
                        //if ((shpObject.Name.StartsWith(Global.ImageNameStartWith)) && (criteriaRow == (int)Global.CBVHeaderRow.HeaderWhereRow)) // Identify the image name and row number
                        if ((shpObject.Name.Contains(Global.ImageNameStartWith)) && (criteriaRow == (int)Global.CBVHeaderRow.HeaderWhereRow)) // Identify the image name and row number
                        {
                            shpObject.Delete();
                            nRange.ClearComments();
                        }
                    }
                    catch (Exception ex)
                    {
                        CBVExcel.ErrorLogging(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion "Clear Current Results"

        #region "SQL Logging"
        public static void ErrorLogging(string logData)
        {
            if (Global.ISErrorLogging)
            {
                Log.CreateEditLogFile(AppConfigSetting.ReadSetting(StringEnum.GetStringValue(Global.ConfigurationKey.SQL_LOGGING_FILE)), logData);
            }
        }
        #endregion "SQL Logging"
    }
        #endregion

    #endregion _  CBVExcel class  _

    #region _  CellData class  _

    class CellData
    {
        //Changed from using Index to Object Name. When an Picture is deleted, all other
        //Indexes after it will become invalid if stored.

        public string objectName; // Picture's Name
        public string objFormula;
        public byte[] Data;
        public string cdxData;

        public CellData()
        {
            Data = null;
            objectName = null;
            objFormula = null;
            cdxData = null;
        }

        /// <summary>
        /// Returns TRUE if associated object name is not Null.
        /// </summary>
        public bool HasValidObjectName
        {
            get
            {
                return (null != objectName) && (objectName.StartsWith("Picture"));
            }
        }
    }

    #endregion

    #region _  GraphicUtility class  _

    class GraphicUtility : IDisposable
    {
        #region .ctor and Dispose methods
        private bool _Disposed;
        public GraphicUtility()
        {
        }

        public virtual void Dispose()
        {
            if (!_Disposed)
            {
                GC.SuppressFinalize(this);
                _Disposed = true;
            }
        }

        #endregion

        #region Member Varaibles


        #endregion

        # region Public Methods

        # region _ Image Method _

        public void AddImage(Excel.Worksheet nSheet, object graphics, Excel.Range nRange, string imageOptionValue)
        {
            object[] graphic = (object[])graphics.ToString().Split('\n');

            float val = 0;
            float top = 0;
            bool IsRowHeightAboveLimit = false;

            try
            {
                for (int i = 0; i < graphic.Length; i++)
                {
                    byte[] bData = Convert.FromBase64String(graphic[i].ToString());
                    Image img = Global.ByteArrayToImage(bData);

                    string tempImageFile = Path.GetTempPath() + "\\" + "tempImage.jpeg";
                    using (FileStream fsImage = new FileStream(tempImageFile, FileMode.OpenOrCreate))
                    {
                        fsImage.Write(bData, 0, bData.Length);
                        fsImage.Close();
                    }

                    Excel::Shape shape = nSheet.Shapes.AddPicture(tempImageFile, Office.MsoTriState.msoFalse, Office.MsoTriState.msoTrue,
       (float)(double)nRange.Left, (float)(double)nRange.Top + top, img.Width, img.Height);

                    float lft1 = (float)(double)nRange.Left;
                    float top1 = (float)(double)nRange.Top;
                    // delete the temporary file
                    if (System.IO.File.Exists(tempImageFile)) System.IO.File.Delete(tempImageFile);

                    shape.Line.Visible = Office.MsoTriState.msoTrue;
                    shape.Fill.Visible = Office.MsoTriState.msoTrue;

                    // make original size image
                    shape.ScaleHeight(1, Microsoft.Office.Core.MsoTriState.msoTrue, Office.MsoScaleFrom.msoScaleFromTopLeft);
                    shape.ScaleWidth(1, Microsoft.Office.Core.MsoTriState.msoTrue, Office.MsoScaleFrom.msoScaleFromTopLeft);

                    AdjustExactCellAndShape(nRange, shape, val, ref IsRowHeightAboveLimit, imageOptionValue);

                    if (IsRowHeightAboveLimit)
                        break;

                    val += shape.Height;
                    top += shape.Height - 5; // increase the image top position for recurring images
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private float ImageSize(string imageOptionValue)
        {
            if (imageOptionValue.Contains("75%"))
                return (float)0.75;
            else if (imageOptionValue.Contains("50%"))
                return (float)0.50;
            else
                return (float)1.0;

        }

        #endregion


        #region _ Structure Methods _

        public Excel.Shape AddStructure(Excel.Worksheet worksheet, string cdxData, Excel.Range cell)
        {
            try
            {
                ReturnedData data = Global.ConvertUsingCDAX(Global.CDXML_MIME_STRING, cdxData, Global.EMF_MIME_STRING);

                if (null == data.Data)
                {
                    return null;
                }

                byte[] emfData = null;
                emfData = Convert.FromBase64String(data.Data.ToString());

                //creates temp.cdx files (for each and every imported molecule) in Temp folder
                string tempFileName = Path.GetTempFileName();
                string tempCDXFile = Path.GetTempPath() + Path.GetFileNameWithoutExtension(tempFileName) + ".cdx";

                File.Move(tempFileName, tempCDXFile);

                using (FileStream fsCDX = new FileStream(tempCDXFile, FileMode.OpenOrCreate))
                {
                    fsCDX.Write(emfData, 0, emfData.Length);
                    fsCDX.Close();
                }
                // create a picture from this cdx file              
                return CreatePictureFromFile(worksheet, cell, emfData, tempCDXFile);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetStructureHeightWidth(Excel.Worksheet nSheet, Excel.Range cell, float height, float width)
        {
            Excel.Shape objShape = null;
            try
            {
                string shapename = Global.GetCellDataCDXL(cell).objectName;
                if (string.IsNullOrEmpty(shapename))
                    return;

                objShape = nSheet.Shapes.Item(shapename);

                objShape.Placement = Excel.XlPlacement.xlFreeFloating;
                if (objShape.Height > height)
                    height = (float)Math.Round((height / objShape.Height), 2);
                else if (objShape.Height > 389) // The row max height is 409. Can't have cell height greater than this, -20 space for formula
                    height = (float)Math.Round((389 / objShape.Height), 2);
                else
                    height = 1f;

                if (objShape.Width > width)
                    width = (float)Math.Round((width / objShape.Width), 2);
                else if (objShape.Width > 255) // The col max width is 255. Can't have cell height greater than this
                    width = (float)Math.Round((255 / objShape.Height), 2);
                else
                    width = 1f;

                float factor = (float)Math.Round((height + width) / 2, 2);
                //if(height >= width)
                //    factor = (float)Math.Round((height + width) / 2, 2);
                //else
                //    factor=1;

                objShape.ScaleHeight(factor, Microsoft.Office.Core.MsoTriState.msoTrue, Office.MsoScaleFrom.msoScaleFromTopLeft);
                objShape.ScaleWidth(factor, Microsoft.Office.Core.MsoTriState.msoTrue, Office.MsoScaleFrom.msoScaleFromTopLeft);
                objShape.Placement = Excel.XlPlacement.xlMoveAndSize;

                SetStructureColumnWidth(objShape, cell);

            }
            catch
            {
                objShape.Placement = Excel.XlPlacement.xlMoveAndSize;
            }
        }


        private void SetStructureColumnWidth(Excel.Shape shape, Excel.Range nRange)
        {
            try
            {
                //column width calucation from CDXL 
                Double requiredColumnWidth = (((6 + shape.Width + ((shape.Width - 48) / 12)) / 48) * 8.43);

                // Excel Height is measured in points (72 points to the inch)
                // Excel Width is measured in points (12 points to the inch)
                //Double requiredColumnWidth1 = Math.Round(shape.Width / 6, 2);

                // The height is fit  +20 is used to add extra height for structure name display
                nRange.RowHeight = Math.Round(shape.Height + 20, 2);

                if (requiredColumnWidth > 255)
                {
                    shape.Width = 255; //Excel standards: column width must be between 0 to 255
                    nRange.ColumnWidth = 255;

                }
                else if (Global.MaxColumnWidth < 15 && requiredColumnWidth < 15) // The column size can't less than 15 - hardcoded here
                {
                    nRange.ColumnWidth = 15;
                    Global.MaxColumnWidth = 15;
                }
                else if (Global.MaxColumnWidth < requiredColumnWidth)
                {
                    nRange.ColumnWidth = requiredColumnWidth; // reducing column width
                    Global.MaxColumnWidth = requiredColumnWidth;
                }
                else
                {
                    nRange.ColumnWidth = Global.MaxColumnWidth;
                }
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
        }



        /// <summary>
        /// Creates a Picture from a specified CDX file on the specified worksheet and cell. The comment for the cell
        /// is replaced by picture name and alttext.
        /// </summary>

        public Excel.Shape CreatePictureFromFile(Excel.Worksheet worksheet, Excel.Range cell, byte[] cdxData, string cdxFile)
        {
            try
            {
                // create a filled Picture
                Excel.Shape shape = worksheet.Shapes.AddPicture(cdxFile, Office.MsoTriState.msoFalse, Office.MsoTriState.msoCTrue, (float)(double)cell.Left, (float)(double)cell.Top, Convert.ToSingle(Global.StructureWidth), Convert.ToSingle(Global.StructureHeight));

                // delete the temporary file for shape
                if (File.Exists(cdxFile)) File.Delete(cdxFile);

                // the user wont see a white rect in selection
                shape.Line.Visible = Office.MsoTriState.msoFalse;
                shape.Fill.Visible = Office.MsoTriState.msoFalse;

                // make original size image
                shape.ScaleHeight(1, Microsoft.Office.Core.MsoTriState.msoTrue, Office.MsoScaleFrom.msoScaleFromTopLeft);
                shape.ScaleWidth(1, Microsoft.Office.Core.MsoTriState.msoTrue, Office.MsoScaleFrom.msoScaleFromTopLeft);

                AdjustCellAndShape(cell, shape);

                // move and size with cell, this will automatically hide the picture when a cell is hidden                
                // set cell's comment to hold picture's name and AltText           
                Global.SetCellData(cell, shape.Name, cdxData);
                shape.Placement = Microsoft.Office.Interop.Excel.XlPlacement.xlMoveAndSize;
                return shape;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #endregion

        #region Private Methods

        #region _ Adjust Cell And Shape _

        private void AdjustExactCellAndShape(Excel.Range nRange, Excel.Shape shape, float incCellHeight, ref bool rowLimit, string imageOptionValue)
        {
            try
            {
                Double requiredRowHeight = shape.Height + incCellHeight;
                requiredRowHeight = requiredRowHeight * ImageSize(imageOptionValue);

                if (requiredRowHeight > (float)409)
                {
                    nRange.RowHeight = 409; //Excel standards: row height must be between 0 to 409
                    rowLimit = true;
                    return;
                }
                else
                {
                    nRange.RowHeight = requiredRowHeight + 2; //Add +1 to adjust within the cell
                }

                Double requiredColumnWidth = (((6 + shape.Width + ((shape.Width - 48) / 12)) / 48) * 8.43);

                //set the image width size
                requiredColumnWidth = requiredColumnWidth * ImageSize(imageOptionValue);

                shape.Width = (shape.Width * ImageSize(imageOptionValue));

                if (requiredColumnWidth > 255)
                {
                    shape.Width = 255; //Excel standards: column width must be between 0 to 255
                    nRange.ColumnWidth = 255;
                }
                else
                {
                    nRange.ColumnWidth = requiredColumnWidth; // reducing column width 
                }

                // Set the shape height and width. This has been fixed for office 2007
                shape.ScaleHeight(ImageSize(imageOptionValue), Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoScaleFrom.msoScaleFromTopLeft);
                shape.ScaleWidth(ImageSize(imageOptionValue), Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoScaleFrom.msoScaleFromTopLeft);

                shape.Placement = Microsoft.Office.Interop.Excel.XlPlacement.xlMoveAndSize;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AdjustCellAndShape(Excel.Range nRange, Excel.Shape shape)
        {
            try
            {
                // set cell height
                float requiredHeight = shape.Height + 20; // space at the bottom for formula

                if (requiredHeight < Convert.ToSingle(nRange.RowHeight))
                {
                    if (requiredHeight > 50) // we can't have cell height greater than this
                        nRange.RowHeight = shape.Height = 50;
                }
                else
                {
                    if (requiredHeight > 50)
                        nRange.RowHeight = shape.Height = 50;
                    else
                        nRange.RowHeight = requiredHeight;
                }

                Double requiredColumnWidth = (((6 + shape.Width + ((shape.Width - 48) / 12)) / 48) * 8.43);

                if (requiredColumnWidth > Global.MaxColumnWidth)
                {
                    Global.MaxColumnWidth = requiredColumnWidth;
                }

                if (Global.MaxColumnWidth > Convert.ToSingle(nRange.ColumnWidth))
                {
                    if (Global.MaxColumnWidth > 50)
                    {
                        shape.Width = 50;
                        Global.MaxColumnWidth = 50;
                    }
                    nRange.ColumnWidth = Global.MaxColumnWidth;
                }
                else
                {
                    nRange.ColumnWidth = Global.MaxColumnWidth; // reducing column width
                }

                shape.Left = Convert.ToSingle(shape.Left) + 2;
                shape.Top = Convert.ToSingle(shape.Top) + 2;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region _ Remove Structure/Image _

        /// <summary>
        /// Removes the specified picture from the specified worksheet.
        /// </summary>
        /// <param name="worksheet">Worksheet on which the picture exists</param>
        /// <param name="cell">Cell in which the picture exists</param>
        public void DeletePicture(Excel.Worksheet worksheet, Excel.Range cell)
        {
            // if the cell contains a reference to an object, delete the object first
            //string ObjectName = Global.GetCellData(cell).objectName;
            string objectname = Global.RemoveCellData(cell).objectName;
            if (null != objectname)
            {
                try
                {
                    worksheet.Shapes.Item(objectname).Delete();
                    cell.ClearComments();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #endregion
    }

    #endregion

    #region _  ReturnData class  _
    class ReturnedData
    {
        private object dataField;
        private string messageField;
        private object molWeight;
        private object formula;
        /// <remarks/>
        public object Data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }

        /// <remarks/>
        public string Message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                this.messageField = value;
            }
        }

        public object MolWeight
        {
            get
            {
                return this.molWeight;
            }
            set
            {
                this.molWeight = value;
            }
        }

        public object Formula
        {
            get
            {
                return this.formula;
            }
            set
            {
                this.formula = value;
            }
        }
    }
    #endregion

    #region _  ValidateDataView class  _

    class ValidateDataView : CBVExcel, IDisposable
    {
        #region .ctor and Dispose methods

        private bool _Disposed;
        public virtual void Dispose()
        {
            if (!_Disposed)
            {

                GC.SuppressFinalize(this);
                _Disposed = true;
            }
        }
        #endregion

        #region "Public Methods"

        public void ValidateExistingDataview(string hiddenSheetDV, Excel.Worksheet sheet, COEDataViewBO dataViewsBO)
        {
            try
            {
                Excel::Worksheet nHideenSheet = GlobalCBVExcel.Get_CSHidden();
                string msg = Properties.Resources.msgChangesinCBV1 + "\n\n";
                bool IsChangesInExistingSheet = false;

                //string hiddenSheetDV = GlobalCBVExcel.GetCell(row, Global.CBVHiddenSheetHeader.Dataview, nHideenSheet).Trim();
                string currentDV = dataViewsBO.COEDataView.ToString().Trim();
                if (currentDV.Length <= Global.CellMaxCharacter) //If the current dataview length exceeds the max character length of the excel cell then skip the validation.
                {
                    if (!hiddenSheetDV.Equals(currentDV, StringComparison.OrdinalIgnoreCase))
                    {
                        COEDataView exCOEDataView = new COEDataView();
                        TableListBO existingTables = null;
                        TableListBO newTables = null;
                        try
                        {
                            Global.RestoreCSLAPrincipal();
                            exCOEDataView = GetCOEDataView(hiddenSheetDV);
                            existingTables = TableListBO.NewTableListBO(exCOEDataView.Tables);
                            newTables = TableListBO.NewTableListBO(GetCOEDataView(currentDV).Tables);
                        }
                        finally
                        {
                            Global.RestoreWindowsPrincipal();
                        }

                        List<int> mimeTypeColumnIndex = new List<int>();
                        Dictionary<int, string> cellTabRefVal = new Dictionary<int, string>();
                        Dictionary<int, string> cellColRefVal = new Dictionary<int, string>();
                        Dictionary<int, string> cellColRefValMW = new Dictionary<int, string>();
                        Dictionary<int, string> cellColRefValMF = new Dictionary<int, string>();
                        Dictionary<int, string> cellIndexTypeRefVal = new Dictionary<int, string>();
                        Dictionary<int, string> cellMimeTypeRefVal = new Dictionary<int, string>();
                        Dictionary<int, string> cellDataTypeRefVal = new Dictionary<int, string>();

                        string catAlias = null;
                        string tabAlias = null;
                        string colAlias = null;
                        string actual_colAlias = null;

                        HeadersLooping(ref IsChangesInExistingSheet, ref msg, existingTables, newTables, exCOEDataView, sheet, ref catAlias, ref tabAlias, ref colAlias, ref actual_colAlias, ref mimeTypeColumnIndex, ref cellTabRefVal, ref cellColRefVal, ref cellColRefValMW, ref cellColRefValMF, ref cellDataTypeRefVal, ref cellIndexTypeRefVal, ref cellMimeTypeRefVal);

                        if (IsChangesInExistingSheet)
                        {
                            ValidateChangesInExistingHeaders(sheet, ref msg, currentDV, mimeTypeColumnIndex, cellTabRefVal, cellColRefVal, cellColRefValMW, cellColRefValMF, cellDataTypeRefVal, cellIndexTypeRefVal, cellMimeTypeRefVal);

                        } //End if IsChangesInExistingSheet 
                        else
                        {
                            //11.0.3
                            UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.Dataview, Global.CurrentWorkSheetName, currentDV);                    // The hidden sheet dataview should be update on click of Yes  
                        }
                    } //End if existing dataview compare to new dataview
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion "Public Methods"

        #region "Private Methods"

        private void GetHeadersAlias(int x, Excel.Worksheet sheet, ref string catAlias, ref string tabAlias, ref string colAlias, ref string actual_colAlias)
        {
            catAlias = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderCategoryRow, x);
            tabAlias = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderTableRow, x);
            colAlias = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderColumnRow, x);

            actual_colAlias = colAlias.ToUpper().Replace(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_MolWeight), string.Empty).Replace(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_Formula), string.Empty).Trim();
        }

        private void GetColumnProperties(int x, Excel.Worksheet sheet, COEDataView exCOEDataView, ref string exIndexType, ref string exMimeType, ref string exDataType)
        {
            try
            {
                using (CriteriaUtilities criteriaUtil = new CriteriaUtilities())
                {
                    exIndexType = criteriaUtil.GetHeaderColumnInfo(x, sheet, Global.INDEXTYPE, exCOEDataView).Trim().ToLower();
                    exMimeType = criteriaUtil.GetHeaderColumnInfo(x, sheet, Global.MIMITYPE, exCOEDataView).Trim().ToLower();
                    exDataType = criteriaUtil.GetHeaderColumnInfo(x, sheet, Global.DATATYPE, exCOEDataView).Trim().ToLower();
                }
            }
            catch
            {
                return;
            }

        }

        private void HeadersLooping(ref bool IsChangesInExistingSheet, ref string msg, TableListBO existingTables, TableListBO newTables, COEDataView exCOEDataView, Excel.Worksheet sheet, ref string catAlias, ref string tabAlias, ref string colAlias, ref string actual_colAlias, ref List<int> mimeTypeColumnIndex, ref Dictionary<int, string> cellTabRefVal, ref Dictionary<int, string> cellColRefVal, ref Dictionary<int, string> cellColRefValMW, ref Dictionary<int, string> cellColRefValMF, ref Dictionary<int, string> cellDataTypeRefVal, ref  Dictionary<int, string> cellIndexTypeRefVal, ref Dictionary<int, string> cellMimeTypeRefVal)
        {
            for (int x = 1; x <= Global.CBVNewColumnIndex; x++)
            {
                //check  whether the column exists in search range or not
                if (!Global.CellDropdownRange.Contains(x))
                    continue;

                GetHeadersAlias(x, sheet, ref catAlias, ref tabAlias, ref colAlias, ref actual_colAlias);

                //Validate for null or empty
                if (string.IsNullOrEmpty(catAlias) || string.IsNullOrEmpty(tabAlias) || string.IsNullOrEmpty(colAlias))
                    continue;

                string exIndexType = string.Empty;
                string exMimeType = string.Empty;
                string exDataType = string.Empty;

                int existingTableID;
                int existingColID;
                string newTableName = string.Empty;
                string newColName = string.Empty;
                string newIndexType = string.Empty;
                string newMimeType = string.Empty;
                string newDataType = string.Empty;

                //Retrive the index type, datatype and mime type
                GetColumnProperties(x, sheet, exCOEDataView, ref exIndexType, ref exMimeType, ref exDataType);
                //iterate the existing dataview tables count
                for (int tab = 0; tab < existingTables.Count; tab++)
                {
                    //Verify the existing dataview have database/category exists 
                    if (existingTables[tab].DataBase.Trim().Equals(catAlias, StringComparison.OrdinalIgnoreCase))
                    {
                        //Verify the existing dataview have table exists 
                        if (existingTables[tab].Alias.Equals(tabAlias, StringComparison.OrdinalIgnoreCase))
                        {
                            //Retrieve the exsting table id.
                            existingTableID = existingTables[tab].ID;

                            //Retrive the new/updated table name from updated dataview on the basis of existing datview table id and store in a  variable.
                            for (int nTab = 0; nTab < newTables.Count; nTab++)
                            {
                                if (existingTableID == newTables[nTab].ID)
                                {
                                    newTableName = newTables[nTab].Alias.Trim();
                                    break;
                                }
                                else
                                {
                                    newTableName = string.Empty;
                                }
                            }

                            //Set the modified table name in existing tablename cell
                            if (newTableName.ToLower() != tabAlias.ToLower())
                            {
                                //newTableName = newTableName;

                                if (!cellTabRefVal.ContainsValue(newTableName)) // to avoid redudant table name
                                {
                                    msg += "Table: " + catAlias + "." + tabAlias + " Rename to " + catAlias + "." + newTableName + "\n";
                                }

                                cellTabRefVal.Add(x, newTableName);

                                IsChangesInExistingSheet = true;

                            }
                            else if (newTableName == string.Empty)
                            {
                                //tabAlias = tabAlias;
                                if (!cellTabRefVal.ContainsValue(newTableName))
                                {
                                    msg += "Table: " + catAlias + "." + tabAlias + " can't found " + "\n";
                                }
                                cellTabRefVal.Add(x, string.Empty);
                                IsChangesInExistingSheet = true;
                            }

                            //Iterate the existing dataview tables filed count;
                            for (int col = 0; col < existingTables[tab].Fields.Count; col++)
                            {
                                //Verify the column name in existing dataview
                                if ((existingTables[tab].Fields[col].Alias.Trim().Equals(colAlias, StringComparison.OrdinalIgnoreCase)) || (existingTables[tab].Fields[col].Alias.Trim().Equals(actual_colAlias, StringComparison.OrdinalIgnoreCase)))
                                {
                                    //Retrieve the column id from the existing dataview
                                    existingColID = existingTables[tab].Fields[col].ID;

                                    //Retrive the new/updated column name from updated dataview on the basis of existing datview column id and store in a  variable.
                                    for (int nCol = 0; nCol < newTables[tab].Fields.Count; nCol++)
                                    {
                                        if (existingColID == newTables[tab].Fields[nCol].ID)
                                        {
                                            newColName = newTables[tab].Fields[nCol].Alias.Trim();
                                            newIndexType = newTables[tab].Fields[nCol].IndexType.ToString().Trim().ToLower();
                                            newMimeType = newTables[tab].Fields[nCol].MimeType.ToString().Trim().ToLower();
                                            newDataType = newTables[tab].Fields[nCol].DataType.ToString().Trim().ToLower();
                                            break; //No more itereration if id found
                                        }
                                        else
                                        {
                                            newColName = string.Empty;
                                        }
                                    }

                                    if (newColName.ToLower() != colAlias.ToLower())
                                    {
                                        // The molecualar weight selected for structure column
                                        string extract_colAlias = string.Empty;
                                        if (colAlias.ToUpper().Contains(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_MolWeight)))
                                        {
                                            extract_colAlias = colAlias.ToUpper().Replace(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_MolWeight), string.Empty);

                                            if (newColName.ToLower() != extract_colAlias.ToLower())
                                            {
                                                //colAlias = colAlias;
                                                if (!cellColRefValMW.ContainsValue(newColName + StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_MolWeight)))
                                                {
                                                    msg += "Field: " + colAlias + " Rename to " + newColName + StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_MolWeight) + "\n";
                                                }
                                                IsChangesInExistingSheet = true;

                                                cellColRefValMW.Add(x, newColName + StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_MolWeight));
                                            }
                                        }

                                        // The molecualar formula selected for structure column
                                        else if (colAlias.ToUpper().Contains(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_Formula)))
                                        {
                                            extract_colAlias = colAlias.ToUpper().Replace(StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_Formula), string.Empty);

                                            if (newColName.ToLower() != extract_colAlias.ToLower())
                                            {
                                                //colAlias = colAlias;

                                                if (!cellColRefValMF.ContainsValue(newColName + StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_Formula)))
                                                {
                                                    msg += "Field: " + colAlias + " Rename to " + newColName + StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_Formula) + "\n";
                                                }
                                                IsChangesInExistingSheet = true;

                                                cellColRefValMF.Add(x, newColName + StringEnum.GetStringValue(Global.CBVDisplayHeaderRowStructure.Disp_Formula));
                                            }
                                        }
                                        else
                                        {
                                            //colAlias = colAlias;

                                            if (!cellColRefVal.ContainsValue(newColName))
                                            {
                                                msg += "Field: " + colAlias + " Rename to " + newColName + "\n";
                                            }
                                            IsChangesInExistingSheet = true;

                                            cellColRefVal.Add(x, newColName);
                                        }
                                    }//End if newcolname match witll colalias
                                    //
                                    if (newColName == string.Empty)
                                    {
                                        //colAlias = colAlias;

                                        if (!cellColRefVal.ContainsValue(newColName))
                                        {
                                            msg += "Field: " + colAlias + " can't found" + "\n";
                                        }
                                        IsChangesInExistingSheet = true;
                                        cellColRefVal.Add(x, string.Empty);
                                    }

                                    if ((exIndexType != newIndexType) && (actual_colAlias.Equals(colAlias, StringComparison.OrdinalIgnoreCase))) // Actual column name and colAlias should be same for avoid the repetition
                                    {
                                        if (!cellIndexTypeRefVal.ContainsValue(newIndexType))
                                        {
                                            msg += "Index Type: " + exIndexType.ToUpper() + " Change to " + newIndexType.ToUpper() + "\n";
                                        }
                                        IsChangesInExistingSheet = true;
                                        cellIndexTypeRefVal.Add(x - 1, newIndexType);
                                    }

                                    if ((exMimeType != newMimeType) && (actual_colAlias.Equals(colAlias, StringComparison.OrdinalIgnoreCase))) // Actual column name and colAlias should be same for avoid the repetition
                                    {
                                        if (!cellMimeTypeRefVal.ContainsValue(newMimeType))
                                        {
                                            msg += "Mime Type: " + exMimeType.ToUpper() + " Change to " + newMimeType.ToUpper() + "\n";
                                        }
                                        IsChangesInExistingSheet = true;
                                        cellMimeTypeRefVal.Add(x - 1, newMimeType);


                                        if ((exMimeType.ToUpper() != "NONE") && (newMimeType.ToUpper() != "NONE")) // Figure out later on
                                        {
                                            mimeTypeColumnIndex.Add(x);
                                        }
                                    }

                                    if (exDataType != newDataType)
                                    {
                                        if (!cellDataTypeRefVal.ContainsValue(newDataType))
                                        {
                                            msg += "Data Type: " + exDataType.ToUpper() + " Change to " + newDataType.ToUpper() + "\n";
                                        }
                                        IsChangesInExistingSheet = true;
                                        cellDataTypeRefVal.Add(x, newDataType);
                                    }
                                }//End of for loop - existing dataview table fileds count
                            }//End of for loop - existing dataview tables count
                        } //End if, the CBVsheet table alias compare with existing dataview table alias  
                    } //End if, the CBVsheet database name compare with existing dataview database name             
                } //End of for loop for existing datview tables count
            }//End of for loop for existing CBVsheet column index

        }

        private void ValidateChangesInExistingHeaders(Excel.Worksheet sheet, ref string msg, string currentDV, List<int> mimeTypeColumnIndex, Dictionary<int, string> cellTabRefVal, Dictionary<int, string> cellColRefVal, Dictionary<int, string> cellColRefValMW, Dictionary<int, string> cellColRefValMF, Dictionary<int, string> cellDataTypeRefVal, Dictionary<int, string> cellIndexTypeRefVal, Dictionary<int, string> cellMimeTypeRefVal)
        {
            msg += "\n" + Properties.Resources.msgChangesinCBV2;
            if (MessageBox.Show(msg, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                Excel.Range nRange = null;

                //Replace the tables names in CBV sheet;
                foreach (KeyValuePair<int, string> cellTabRef in cellTabRefVal)
                {
                    sheet.Cells[(int)Global.CBVHeaderRow.HeaderTableRow, cellTabRef.Key] = cellTabRef.Value;
                }

                //Replace the column names in CBV sheet;
                foreach (KeyValuePair<int, string> cellColRef in cellColRefVal)
                {
                    sheet.Cells[(int)Global.CBVHeaderRow.HeaderColumnRow, cellColRef.Key] = cellColRef.Value;
                    sheet.Cells[(int)Global.CBVHeaderRow.HeaderColumnLabelDisplayRow, cellColRef.Key] = cellColRef.Value;

                }

                //Replace the Mol Weight column names in CBV sheet;
                foreach (KeyValuePair<int, string> cellColRefMW in cellColRefValMW)
                {
                    sheet.Cells[(int)Global.CBVHeaderRow.HeaderColumnRow, cellColRefMW.Key] = cellColRefMW.Value;
                    sheet.Cells[(int)Global.CBVHeaderRow.HeaderColumnLabelDisplayRow, cellColRefMW.Key] = cellColRefMW.Value;
                }
                //Replace the Mol Formula column names in CBV sheet;
                foreach (KeyValuePair<int, string> cellColRefMF in cellColRefValMF)
                {
                    sheet.Cells[(int)Global.CBVHeaderRow.HeaderColumnRow, cellColRefMF.Key] = cellColRefMF.Value;
                    sheet.Cells[(int)Global.CBVHeaderRow.HeaderColumnLabelDisplayRow, cellColRefMF.Key] = cellColRefMF.Value;
                }

                //DataType
                foreach (KeyValuePair<int, string> cellDataTypeRef in cellDataTypeRefVal)
                {
                    nRange = (Excel.Range)sheet.Cells[(int)Global.CBVHeaderRow.HeaderColumnRow, cellDataTypeRef.Key];

                    AddInOptionHeaderCellDropdown(sheet, nRange, (int)Global.CBVHeaderRow.HeaderOptionRow, cellDataTypeRef.Key);

                    if (Global.ToggleColumnAutoSize == 1)
                        nRange.Columns.AutoFit();
                }

                bool chkIndexMimeType = false; //Check for index type and mime type that is used for searching the data

                //Index Type
                if (Global._CBVSearchResult != null && Global._CBVSearchResult.Rows.Count > 0)
                {
                    foreach (KeyValuePair<int, string> cellIndexTypeRef in cellIndexTypeRefVal)
                    {
                        DataSetUtilities.ModifyColumnIndexType(Global._CBVSearchResult, cellIndexTypeRef.Value, cellIndexTypeRef.Key);
                        chkIndexMimeType = true;

                    }
                }

                //Mime Type
                if (Global._CBVSearchResult != null && Global._CBVSearchResult.Rows.Count > 0)
                {
                    foreach (KeyValuePair<int, string> cellMimeTypeRef in cellMimeTypeRefVal)
                    {
                        //Clear the existing data except existing images - In case of one mime type to another        
                        DataSetUtilities.ModifyColumnMimeType(Global._CBVSearchResult, cellMimeTypeRef.Value, cellMimeTypeRef.Key);
                        chkIndexMimeType = true;
                    }
                }

                if (chkIndexMimeType)
                {
                    //Store search schema into hidden sheet in serialize form.
                    //11.0.3
                    UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.SerializeCBVResult, Global.CurrentWorkSheetName, SerializeDeserialize.Serialize(Global._CBVSearchResult.Clone()));

                    //Restore the CBV result into global table
                    DataSetUtilities.RestoreCBVResult(sheet);

                    //If mime type modified except NONE
                    if (mimeTypeColumnIndex != null && mimeTypeColumnIndex.Count > 0)
                    {
                        ClearResultsOnDataViewUpdate(sheet, mimeTypeColumnIndex);
                        SetCell((int)Global.CBVHeaderRow.HeaderResultStartupRow, 1, sheet, Global._CBVSearchResult, false, false);
                    }
                    else
                    {
                        SetCell((int)Global.CBVHeaderRow.HeaderResultStartupRow, 1, sheet, Global._CBVSearchResult, false, true);
                    }
                }
                //11.0.3
                UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.Dataview, Global.CurrentWorkSheetName, currentDV);//dataview will update on click of yes   
                Global._ExcelApp.ScreenUpdating = true;
            }
        }
        #endregion "Privates Methods"

        #region "Validate Server"

        public static bool ValidateDataViewIDNameHeaders(Excel.Worksheet nSheet, int newColIndex)
        {
            try
            {
                Excel::Worksheet nHideenSheet = GlobalCBVExcel.Get_CSHidden();
                if (nHideenSheet != null)
                {
                    //string dvID = string.Empty;

                    int rowCnt = nHideenSheet.UsedRange.Rows.Count;
                    int colCnt = nHideenSheet.UsedRange.Columns.Count;
                    string data = string.Empty;


                    object[,] cellRngVal = new object[rowCnt, colCnt];
                    Excel.Range cellRange = nHideenSheet.get_Range("A2", Global.NumToString(colCnt) + rowCnt);
                    cellRngVal = (object[,])cellRange.Value2;

                    for (int rows = 1; rows <= rowCnt; rows++)
                    {
                        //Excel.Range cell = (Excel.Range)nHideenSheet.Cells[rows, Global.CBVHiddenSheetHeader.Sheetname];

                        string sheetName = DataSetUtilities.GetDataFromCellRange(cellRngVal, rows, (int)Global.CBVHiddenSheetHeader.Sheetname).Trim();
                        if (string.IsNullOrEmpty(sheetName))
                            continue;

                        COEDataViewBOList dataViewsBOList = null;
                        if (sheetName.Equals(nSheet.Name.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                Global.RestoreCSLAPrincipal();
                                dataViewsBOList = COEDataViewBOList.GetDataViewListAndNoMaster();
                            }
                            finally
                            {
                                Global.RestoreWindowsPrincipal();
                            }
                            string dvID = DataSetUtilities.GetDataFromCellRange(cellRngVal, rows, (int)Global.CBVHiddenSheetHeader.ID).Trim();
                            string dvName = DataSetUtilities.GetDataFromCellRange(cellRngVal, rows, (int)Global.CBVHiddenSheetHeader.Dataviewname).Trim();

                            foreach (COEDataViewBO dataViewsBO in dataViewsBOList)
                            {
                                if (dataViewsBO.ID.ToString().Trim().Equals(dvID, StringComparison.OrdinalIgnoreCase) && dataViewsBO.Name.Trim().Equals(dvName, StringComparison.OrdinalIgnoreCase))
                                {
                                    Global.Tables = TableListBO.NewTableListBO(dataViewsBO.COEDataView.Tables);

                                    newColIndex = Convert.ToInt32(DataSetUtilities.GetDataFromCellRange(cellRngVal, rows, (int)Global.CBVHiddenSheetHeader.CBVNewColIndex));

                                    Global.SetRangeValue();

                                    if (ValidateDataViewFields(nSheet, dataViewsBO.COEDataView, newColIndex))
                                        return true;
                                    else
                                        return false;
                                }
                            } //End of for each loop
                            return false;
                        } //end if - sheet name compare

                    }//End of for loop

                } //end if - checking hidden shet null

            }
            catch
            {
                return false;
            }
            return false;
        }

        public static bool ValidateDataViewIDNameHeaders(Excel.Worksheet nSheet)
        {
            try
            {
                COEDataViewBOList dataViewsBOList = null;

                try
                {
                    Global.RestoreCSLAPrincipal();
                    dataViewsBOList = COEDataViewBOList.GetDataViewListAndNoMaster();
                }
                finally
                {
                    Global.RestoreWindowsPrincipal();
                }


                int dvID = Global.CBVSHEET_COEDATAVIEWBO.COEDataView.DataViewID;
                string dvName = Global.CBVSHEET_COEDATAVIEWBO.COEDataView.Name;

                foreach (COEDataViewBO dataViewsBO in dataViewsBOList)
                {
                    if (dataViewsBO.ID == dvID && dataViewsBO.Name.Trim().Equals(dvName, StringComparison.OrdinalIgnoreCase))
                    {

                        Global.SetRangeValue();
                        Global.Tables = TableListBO.NewTableListBO(dataViewsBO.COEDataView.Tables);

                        if (ValidateDataViewFields(nSheet, dataViewsBO.COEDataView, nSheet.UsedRange.Columns.Count + 1))
                            return true;
                        else
                            return false;
                    }
                } //End of for each loop
                return false;
            }
            catch
            {
                return false;
            }
        }


        public static bool ValidateDataViewFields(Excel::Worksheet nSheet, COEDataView coeDataView, int newColIndex)
        {

            for (int x = 1; x < newColIndex; x++)
            {
                if (!CriteriaUtilities.ISHeaderColumnInfoExists(x, Global.ALIAS, coeDataView))
                {
                    return false;
                }
                else
                {
                    //Update cell drop down range
                    if (!Global.CellDropdownRange.Contains(x))
                        Global.CellDropdownRange.Add(x);
                }

            }

            return true;
        }

        #endregion "Validate Server"

    }
    #endregion _  ReturnData class  _
}


