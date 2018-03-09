using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Collections;
using CambridgeSoft.COE.Framework.COEDataViewService;

namespace ChemBioVizExcelAddIn
{
    class CBVSheetXMLExport
    {

        public static void CreateCBVSheetXMLExport(object sheet)
        {
            CriteriaUtilities criteriaUtilities = new CriteriaUtilities();
            Excel.Worksheet nSheet = sheet as Excel.Worksheet;
            Excel.Range nRange;
            string headerColumnDatatype = string.Empty;
            string headerColumnIndextype = string.Empty;
            string headerColumnMimetype = string.Empty;
            string[] resultsHiddenCriteria = null;
            string[] criteriaHiddenFileds = null;
            int headerIncremental = 0;            
            int enumValIndex = 0;

            if (nSheet == null)
                throw new System.NullReferenceException();

            Global.IsCDXLWorkSheet = Global.IsCDExcelWorksheet(nSheet);

            if (Global.IsCDXLWorkSheet)
            {
                Global.IsCBVXLWorkSheet = Global.IsCBVExcelWorksheet(nSheet);

                if (!Global.IsCBVXLWorkSheet)
                {
                    try
                    {  
                        Global.MaxRecordInResultSet = 0; // Set the max record to zero

                        //Retrive the result fields from hashtable which is retrieved from CBV sheet export.
                        enumValIndex = (int)Global.CBVExcelExportKeys.ResultsFields;
                        if ((null != Global.CBVHiddenCriteriaCollection[enumValIndex]) || String.Empty != Global.CBVHiddenCriteriaCollection[enumValIndex].ToString())
                            resultsHiddenCriteria = Global.CBVHiddenCriteriaCollection[enumValIndex].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        else
                            throw new Exception("Invalid Result Fields");

                        //Retrive the filed criteria from hashtable which is retrieved from CBV sheet export.
                        enumValIndex = (int)Global.CBVExcelExportKeys.FieldCriteria;
                        if ((null != Global.CBVHiddenCriteriaCollection[enumValIndex]) || String.Empty != Global.CBVHiddenCriteriaCollection[enumValIndex].ToString())
                            criteriaHiddenFileds = Global.CBVHiddenCriteriaCollection[enumValIndex].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        else
                            criteriaHiddenFileds = string.Empty.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);


                        //Loop use for retrieve columns files (databases, tables and fields)
                        foreach (string resultField in resultsHiddenCriteria)
                        {

                            string[] outputEntities = resultField.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

                            //The output column should have always three 3 data (database, table and field)
                            if (outputEntities.Length != 3)
                                throw new Exception("Insufficient entry in result criteria field");
                            headerIncremental++;

                            //Set the category/database header
                            nSheet.Cells[Global.CBVHeaderRow.HeaderCategoryRow, headerIncremental] = outputEntities[0].ToString();
                            //Set the header table header row
                            nSheet.Cells[Global.CBVHeaderRow.HeaderTableRow, headerIncremental] = outputEntities[1].ToString();
                            //Set the column table header row
                            nSheet.Cells[Global.CBVHeaderRow.HeaderColumnRow, headerIncremental] = outputEntities[2].ToString();

                            //Gert the column information (datatype, indextype, mimetype)
                            headerColumnDatatype = criteriaUtilities.GetHeaderColumnInfo(1, sheet, Global.DATATYPE);
                            headerColumnIndextype = criteriaUtilities.GetHeaderColumnInfo(1, sheet, Global.INDEXTYPE);
                            headerColumnMimetype = criteriaUtilities.GetHeaderColumnInfo(1, sheet, Global.MIMITYPE);
                            Global.CBVActiveColumnIndex = headerIncremental;
                            Global.CBVNewColumnIndex = headerIncremental + 1;

                            //***************
                            Global.CellDropdownRange.Add(headerIncremental);
                            //***************

                            //Retrive the criteria row values
                            foreach (string criteriaField in criteriaHiddenFileds)
                            {
                                if (string.Empty != criteriaField)
                                {
                                    if (criteriaField.ToLower().Contains(resultField.ToLower()))
                                    {
                                        string criteria = criteriaField.Replace(resultField, "");
                                        //Set the column criteria row
                                        nSheet.Cells[Global.CBVHeaderRow.HeaderWhereRow, headerIncremental] = criteria;
                                        break;
                                    }
                                }
                            }
                        }         

                        //Retrieve the header list
                        List<string> str = DisplayOption.HeaderList(headerColumnDatatype, headerColumnIndextype, headerColumnMimetype);
                        string retdatatype = DisplayOption.OptionHeaderDataType(headerColumnDatatype, headerColumnIndextype, headerColumnMimetype);

                       
                        //Update the hidden sheet
                        CBVExcel CBVExcel = new CBVExcel();
                        //11.0.3
                        CBVExcel.UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.CBVNewColIndex, Global.CurrentWorkSheetName, Global.CBVNewColumnIndex.ToString()); // Update the hidden sheet (4 is used for column name)
                        CBVExcel.UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.CBVActiveSheetIndex, Global.CurrentWorkSheetName, Global.CBVActiveColumnIndex.ToString());

                        //Range used for freeze pane                      

                        nRange = ((Excel.Range)nSheet.Cells[Global.CBVHeaderRow.HeaderResultStartupRow, Global.CBVNewColumnIndex]);
                        //Coverity fix - CID 18736
                        if (nRange == null)
                            throw new System.NullReferenceException();
                        nRange.Columns.AutoFit();
                        nRange.Select();
                        Global._ExcelApp.ActiveWindow.FreezePanes = true;

                        Global.InsertColumn = true;
                        GlobalCBVExcel.FormatSheet(nSheet);
                        CBVExcel.SetFocus(nSheet);
                        nSheet.CustomProperties.Add(Global.CBVSHEET_PROP_DOCUMENT, "CS");
                        //Insert data into hidden sheet;
                        CBVExcel.SaveDataView(nSheet, Global.CBVSHEET_COEDATAVIEWBO);
                        //11.0.3
                        CBVExcel.UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.Display, Global.CurrentWorkSheetName, "YES"); // Update the hidden sheet (Column 9 for displaying the result in chemoffice sheet).

                        //***************
                        //Update the server info
                        //11.0.3
                        CBVExcel.UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.Server, Global.CurrentWorkSheetName, Global.MRUServer); //
                        CBVExcel.UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.Servermode, Global.CurrentWorkSheetName, Global.MRUServerMode); //
                        //***************
                        CBVExcel.GroupingHeaderSection(nSheet); // Grroping the header column
                    }
                    //}
                    catch (Exception ex)
                    {
                        //throw new Exception(Properties.Resources.msgCBVError);
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        GC.Collect();                        
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

        internal static void SetCBVSheetExportData()
        {         
            Global.RestoreCSLAPrincipal();            
            CBVExcel objCBVExcel = new CBVExcel();
            Dictionary<string, COEDataViewBO> DataViewList = new Dictionary<string, COEDataViewBO>();
            
            if (null == Global.CBVHiddenCriteriaCollection || Global.CBVHiddenCriteriaCollection.Count <= 0)
                throw new Exception("The " + Global.COEDATAVIEW_CBVEXPORT + " sheet not found!");
                //return false;
            int enumValIndex = 0;

            enumValIndex = (int)Global.CBVExcelExportKeys.DataviewID;
            string stringkey = Global.CBVHiddenCriteriaCollection[enumValIndex].ToString();
            COEDataViewBOList dataViews = COEDataViewBOList.GetDataViewListAndNoMaster();
            foreach (COEDataViewBO dv in dataViews)
            {
                DataViewList.Add(dv.ID.ToString(), dv);
            }
            Global.CBVSHEET_COEDATAVIEWBO = DataViewList[stringkey]; 
        }


        //public static Hashtable GetCBVSheetExportData()
        public static Dictionary<int,string> GetCBVSheetExportData()
        {
            Excel::Application _Excelapp = Global._ExcelApp as Excel::Application;
            Global.CBVHiddenCriteriaCollection = new Dictionary<int, string>();
            try
            {
                Excel::Worksheet CBVExportSheet = GlobalCBVExcel.FindSheet(Global.COEDATAVIEW_CBVEXPORT);
                //Coverity fix - CID 18735
                if (CBVExportSheet == null)
                    throw new System.NullReferenceException();
                Excel.Range cell = null;

                if (CBVExportSheet != null)
                {
                    for (int col = 1; col <= CBVExportSheet.UsedRange.Columns.Count; col++)
                    {
                        cell = CBVExportSheet.Cells[2, col] as Excel.Range;
                        if (cell == null)
                            throw new System.NullReferenceException("cell value cannot be null");
                        Global.CBVHiddenCriteriaCollection.Add(col, cell.Value2.ToString());
                    }

                }
                return Global.CBVHiddenCriteriaCollection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }       
    }
}
