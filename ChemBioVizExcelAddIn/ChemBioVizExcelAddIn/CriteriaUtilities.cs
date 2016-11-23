using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Office = Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEDataViewService;

namespace ChemBioVizExcelAddIn
{
    class CriteriaUtilities : IDisposable
    {
        # region _ Variables _

        private Excel::Worksheet nSheet;
        private bool _Disposed;
        #endregion

        #region _ Constructor _

        public CriteriaUtilities(ref Excel::Worksheet worksheet)
        {
            nSheet = worksheet;
        }
        public CriteriaUtilities()
        {
            nSheet = null;
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

        # region _ Private Methods _

        private static string GetCell(string nAddress, Excel::Worksheet worksheet)
        {
            return GlobalCBVExcel.GetCell(nAddress, worksheet);
        }

        private static string GetCell(object Row, object Col, object Sheet)
        {
            CBVExcel CBVExcel = new CBVExcel();
            return GlobalCBVExcel.GetCell(Row, Col, Sheet);
        }

        private string GetResultTable(COEDataView dataView, string Parameter, int i)
        {
            if (Parameter.Trim().Equals(Global.ID, StringComparison.OrdinalIgnoreCase))
            {
                return dataView.Tables[i].Id.ToString();
            }
            else if (Parameter.Trim().Equals(Global.ALIAS, StringComparison.OrdinalIgnoreCase))
            {
                return dataView.Tables[i].Alias.ToString();
            }
            else if (Parameter.Trim().Equals(Global.NAME, StringComparison.OrdinalIgnoreCase))
            {
                return dataView.Tables[i].Name.ToString();
            }
            return string.Empty;
        }

        private string GetResultField(COEDataView dataView, string Parameter, int i, int j)
        {
            if (Parameter.Trim().Equals(Global.ID, StringComparison.OrdinalIgnoreCase))
            {
                return dataView.Tables[i].Fields[j].Id.ToString();
            }
            else if (Parameter.Trim().Equals(Global.NAME, StringComparison.OrdinalIgnoreCase))
            {
                return dataView.Tables[i].Fields[j].Name.ToString();
            }
            else if (Parameter.Trim().Equals(Global.ALIAS, StringComparison.OrdinalIgnoreCase))
            {
                return dataView.Tables[i].Fields[j].Alias.ToString();
            }
            else if (Parameter.Trim().Equals(Global.DATATYPE, StringComparison.OrdinalIgnoreCase))
            {
                if (dataView.Tables[i].Fields[j].LookupFieldId > 0)
                    return (COEDataViewData.GetLookupType(dataView.Tables[i].Fields[j].LookupDisplayFieldId));
                else
                return (dataView.Tables[i].Fields[j].DataType.ToString());
                    
            }
            else if (Parameter.Trim().Equals(Global.INDEXTYPE, StringComparison.OrdinalIgnoreCase))
            {
                // 11.0.3
                if (dataView.Tables[i].Fields[j].LookupFieldId > 0)
                    return (COEDataViewData.GetLookupIndex(dataView.Tables[i].Fields[j].LookupDisplayFieldId));
                else
                return dataView.Tables[i].Fields[j].IndexType.ToString();
            }
            else if (Parameter.Trim().Equals(Global.MIMITYPE, StringComparison.OrdinalIgnoreCase))
            {
                // 11.0.3
                if (dataView.Tables[i].Fields[j].LookupFieldId > 0)
                    return (COEDataViewData.GetLookupMime(dataView.Tables[i].Fields[j].LookupDisplayFieldId));
                else
                return dataView.Tables[i].Fields[j].MimeType.ToString();
            }
            return string.Empty;
        }


        private string GetResultField(COEDataView dataView, string Parameter, string baseTablename, int colIndex)
        {
            if (Parameter.Trim().Equals(Global.ID, StringComparison.OrdinalIgnoreCase))
            {
                return dataView.Tables[baseTablename].Fields[colIndex].Id.ToString();
            }
            else if (Parameter.Trim().Equals(Global.NAME, StringComparison.OrdinalIgnoreCase))
            {
                return dataView.Tables[baseTablename].Fields[colIndex].Name.ToString();
            }
            else if (Parameter.Trim().Equals(Global.ALIAS, StringComparison.OrdinalIgnoreCase))
            {
                return dataView.Tables[baseTablename].Fields[colIndex].Alias.ToString();
            }
            else if (Parameter.Trim().Equals(Global.DATATYPE, StringComparison.OrdinalIgnoreCase))
            {
                //return dataView.Tables[baseTablename].Fields[colIndex].DataType.ToString();
                if (dataView.Tables[baseTablename].Fields[colIndex].LookupFieldId > 0)
                    return (COEDataViewData.GetLookupType(dataView.Tables[baseTablename].Fields[colIndex].LookupDisplayFieldId));
                else
                    return (dataView.Tables[baseTablename].Fields[colIndex].DataType.ToString());
            }
            else if (Parameter.Trim().Equals(Global.INDEXTYPE, StringComparison.OrdinalIgnoreCase))
            {
                // 11.0.3
                if (dataView.Tables[baseTablename].Fields[colIndex].LookupFieldId > 0)
                    return (COEDataViewData.GetLookupIndex(dataView.Tables[baseTablename].Fields[colIndex].LookupDisplayFieldId));
                return dataView.Tables[baseTablename].Fields[colIndex].IndexType.ToString();
            }
            else if (Parameter.Trim().Equals(Global.MIMITYPE, StringComparison.OrdinalIgnoreCase))
            {
                // 11.0.3
                if (dataView.Tables[baseTablename].Fields[colIndex].LookupFieldId > 0)
                    return (COEDataViewData.GetLookupMime(dataView.Tables[baseTablename].Fields[colIndex].LookupDisplayFieldId));
                return dataView.Tables[baseTablename].Fields[colIndex].MimeType.ToString();
            }
            return string.Empty;
        }

        #endregion

        # region _ Public Methods _

        public string GetHeaderTableInfo(int Col, object Sheet, string Parameter)
        {
            string valueCategory;
            string valueTable;
            string returnValue = String.Empty;

            COEDataView dataView = Global.CBVSHEET_COEDATAVIEWBO.COEDataView;

            valueCategory = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderCategoryRow, Col);
            valueTable = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderTableRow, Col);

            if (string.IsNullOrEmpty(valueCategory) || string.IsNullOrEmpty(valueTable))
            {
                return string.Empty;
            }

            for (int i = 0; i < dataView.Tables.Count; i++)
            {
                if (valueCategory.ToLower() == dataView.Tables[i].Database.ToLower())
                {
                    if (dataView.Tables[i].Alias != null && !dataView.Tables[i].Alias.Equals(string.Empty))
                    {
                        if (dataView.Tables[i].Alias.ToLower().Trim() == valueTable.ToLower().Trim())
                        {
                            return returnValue = GetResultTable(dataView, Parameter, i);
                        }
                    }
                    else
                    {
                        if (dataView.Tables[i].Name.ToLower().Trim() == valueTable.ToLower().Trim())
                        {
                            return returnValue = GetResultTable(dataView, Parameter, i);
                        }
                    }
                }
            }
            return string.Empty;
        }
        
        public string GetHeaderColumnInfo(int Col, object Sheet, string Parameter)
        {
            string valueCategory;
            string valueTable;
            string valueField;

            string returnValue = String.Empty;

            COEDataView dataView = Global.CBVSHEET_COEDATAVIEWBO.COEDataView;

            valueCategory = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderCategoryRow, Col);
            valueTable = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderTableRow, Col);
            valueField = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderColumnRow, Col);


            if (string.IsNullOrEmpty(valueCategory) || string.IsNullOrEmpty(valueTable) || string.IsNullOrEmpty(valueField))
            {
                return string.Empty;
            }

            //Retrieve the real column (If molecular weight and formula attached with structure column name)
            bool isMolWTFor = false;
            string[] colFieldMolwtFormula = null;
            if (Global.ISFieldContainsMolWtMolFm(valueField))
            {
                colFieldMolwtFormula = valueField.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

                if (colFieldMolwtFormula.Length == 2)
                {
                    valueField = colFieldMolwtFormula[0];
                    isMolWTFor = true;
                }
            }

            for (int i = 0; i < dataView.Tables.Count; i++)
            {
                for (int j = 0; j < dataView.Tables[i].Fields.Count; j++)
                {

                    if (valueCategory.ToLower().Trim() == dataView.Tables[i].Database.ToLower().Trim())
                    {
                        if (dataView.Tables[i].Alias != null && dataView.Tables[i].Alias != string.Empty)
                        {
                            if (valueTable.ToLower().Trim() == dataView.Tables[i].Alias.ToLower().Trim())
                            {
                                if (dataView.Tables[i].Fields[j].Alias.ToLower().Trim() == valueField.ToLower().Trim())
                                {
                                    returnValue = GetResultField(dataView, Parameter, i, j);
                                    if ((isMolWTFor == true) && (Parameter.ToLower().Trim().Equals(Global.ALIAS) || Parameter.ToLower().Trim().Equals(Global.NAME)))
                                        returnValue = colFieldMolwtFormula[1];
                                    return returnValue;
                                }
                            }
                        }
                        else
                        {
                            if (valueTable.ToLower().Trim() == dataView.Tables[i].Name.ToLower().Trim())
                            {
                                if (dataView.Tables[i].Fields[j].Alias.ToLower().Trim() == valueField.ToLower().Trim())
                                {
                                    returnValue = GetResultField(dataView, Parameter, i, j);
                                    if ((isMolWTFor == true) && (Parameter.ToLower().Trim().Equals(Global.ALIAS) || Parameter.ToLower().Trim().Equals(Global.NAME)))
                                        returnValue = colFieldMolwtFormula[1];
                                    return returnValue;
                                }
                            }
                        }

                    }
                }
            }
            return string.Empty;
        }

        public string GetHeaderColumnInfo(int Col, object Sheet, string Parameter, COEDataView dataView)
        {
            string valueCategory;
            string valueTable;
            string valueField;

            string returnValue = String.Empty;

            valueCategory = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderCategoryRow, Col);
            valueTable = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderTableRow, Col);
            valueField = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderColumnRow, Col);


            if (string.IsNullOrEmpty(valueCategory) || string.IsNullOrEmpty(valueTable) || string.IsNullOrEmpty(valueField))
            {
                return string.Empty;
            }

            //Retrieve the real column (If molecular weight and formula attached with structure column name)
            bool isMolWTFor = false;
            string[] colFieldMolwtFormula = null;
            if (Global.ISFieldContainsMolWtMolFm(valueField))
            {
                colFieldMolwtFormula = valueField.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (colFieldMolwtFormula.Length == 2)
                {
                    valueField = colFieldMolwtFormula[0];
                    isMolWTFor = true;
                }
            }

            for (int i = 0; i < dataView.Tables.Count; i++)
            {
                for (int j = 0; j < dataView.Tables[i].Fields.Count; j++)
                {
                    if (valueCategory.ToLower().Trim() == dataView.Tables[i].Database.ToLower().Trim())
                    {
                        if (dataView.Tables[i].Alias != null && dataView.Tables[i].Alias != string.Empty)
                        {
                            if (valueTable.ToLower().Trim() == dataView.Tables[i].Alias.ToLower().Trim())
                            {
                                if (dataView.Tables[i].Fields[j].Alias.ToLower().Trim() == valueField.ToLower().Trim())
                                {
                                    returnValue = GetResultField(dataView, Parameter, i, j);
                                    if ((isMolWTFor == true) && (Parameter.ToLower().Trim().Equals(Global.ALIAS) || Parameter.ToLower().Trim().Equals(Global.NAME)))
                                        returnValue = colFieldMolwtFormula[1];
                                    return returnValue;
                                }
                            }
                        }
                        else
                        {
                            if (valueTable.ToLower().Trim() == dataView.Tables[i].Name.ToLower().Trim())
                            {
                                if (dataView.Tables[i].Fields[j].Alias.ToLower().Trim() == valueField.ToLower().Trim())
                                {
                                    returnValue = GetResultField(dataView, Parameter, i, j);
                                    if ((isMolWTFor == true) && (Parameter.ToLower().Trim().Equals(Global.ALIAS) || Parameter.ToLower().Trim().Equals(Global.NAME)))
                                        returnValue = colFieldMolwtFormula[1];
                                    return returnValue;
                                }
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }

        public string GetHeaderColumnInfo(string valueCategory, string valueTable, string valueField, string Parameter)
        {
            string returnValue = String.Empty;

            COEDataView dataView = Global.CBVSHEET_COEDATAVIEWBO.COEDataView;

            if (string.IsNullOrEmpty(valueCategory) || string.IsNullOrEmpty(valueTable) || string.IsNullOrEmpty(valueField))
            {
                return string.Empty;
            }


            //Retrieve the real column (If molecular weight and formula attached with structure column name)
            bool isMolWTFor = false;
            string[] colFieldMolwtFormula = null;
            if (Global.ISFieldContainsMolWtMolFm(valueField))
            {
                colFieldMolwtFormula = valueField.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (colFieldMolwtFormula.Length == 2)
                {
                    valueField = colFieldMolwtFormula[0];
                    isMolWTFor = true;
                }
            }

            for (int i = 0; i < dataView.Tables.Count; i++)
            {
                for (int j = 0; j < dataView.Tables[i].Fields.Count; j++)
                {
                    if (valueCategory.ToLower().Trim() == dataView.Tables[i].Database.ToLower().Trim())
                    {
                        if (dataView.Tables[i].Alias != null && dataView.Tables[i].Alias != string.Empty)
                        {
                            if (valueTable.ToLower().Trim() == dataView.Tables[i].Alias.ToLower().Trim())
                            {
                                if (dataView.Tables[i].Fields[j].Alias.ToLower().Trim() == valueField.ToLower().Trim())
                                {
                                    returnValue = GetResultField(dataView, Parameter, i, j);
                                    if ((isMolWTFor == true) && (Parameter.ToLower().Trim().Equals(Global.ALIAS) || Parameter.ToLower().Trim().Equals(Global.NAME)))
                                        returnValue = colFieldMolwtFormula[1];
                                    return returnValue;
                                }
                            }
                        }
                        else
                        {
                            if (valueTable.ToLower().Trim() == dataView.Tables[i].Name.ToLower().Trim())
                            {
                                if (dataView.Tables[i].Fields[j].Alias.ToLower().Trim() == valueField.ToLower().Trim())
                                {
                                    returnValue = GetResultField(dataView, Parameter, i, j);
                                    if ((isMolWTFor == true) && (Parameter.ToLower().Trim().Equals(Global.ALIAS) || Parameter.ToLower().Trim().Equals(Global.NAME)))
                                        returnValue = colFieldMolwtFormula[1];
                                    return returnValue;
                                }
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }

        public static bool ISHeaderColumnInfoExists(int Col, string Parameter, COEDataView dataView)
        {
            string valueCategory;
            string valueTable;
            string valueField;

            string returnValue = String.Empty;

            try
            {
                valueCategory = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderCategoryRow, Col);
                valueTable = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderTableRow, Col);
                valueField = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderColumnRow, Col);

                if (string.IsNullOrEmpty(valueCategory) && string.IsNullOrEmpty(valueTable) && string.IsNullOrEmpty(valueField))
                {
                    return true;
                }

                //Retrieve the real column (If molecular weight and formula attached with structure column name)
                if (Global.ISFieldContainsMolWtMolFm(valueField))
                {
                    string[] colFieldMolwtFormula = valueField.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    if (colFieldMolwtFormula.Length == 2)
                        valueField = colFieldMolwtFormula[0];
                }                

                for (int i = 0; i < dataView.Tables.Count; i++)
                {
                    for (int j = 0; j < dataView.Tables[i].Fields.Count; j++)
                    {
                        if (valueCategory.ToLower().Trim() == dataView.Tables[i].Database.ToLower().Trim())
                        {
                            if (dataView.Tables[i].Alias != null && dataView.Tables[i].Alias != string.Empty)
                            {
                                if (valueTable.ToLower().Trim() == dataView.Tables[i].Alias.ToLower().Trim())
                                {
                                    if (dataView.Tables[i].Fields[j].Alias.ToLower().Trim() == valueField.ToLower().Trim())
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                if (valueTable.ToLower().Trim() == dataView.Tables[i].Name.ToLower().Trim())
                                {
                                    if (dataView.Tables[i].Fields[j].Alias.ToLower().Trim() == valueField.ToLower().Trim())
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        } 

        public static bool ISHeaderColumnInfoExists(int Col, object Sheet, string Parameter)
        {
            string valueCategory;
            string valueTable;
            string valueField;

            string returnValue = String.Empty;

            COEDataView dataView = Global.CBVSHEET_COEDATAVIEWBO.COEDataView;

            valueCategory = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderCategoryRow, Col);
            valueTable = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderTableRow, Col);
            valueField = DataSetUtilities.GetDataFromCellRange(Global.GlobalcellRngVal, (int)Global.CBVHeaderRow.HeaderColumnRow, Col);

            if (string.IsNullOrEmpty(valueCategory) || string.IsNullOrEmpty(valueTable) || string.IsNullOrEmpty(valueField))
            {
                return false;
            }

            //Retrieve the real column (If molecular weight and formula attached with structure column name)
            if (Global.ISFieldContainsMolWtMolFm(valueField))
            {
                string[] colFieldMolwtFormula = valueField.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (colFieldMolwtFormula.Length == 2)
                    valueField = colFieldMolwtFormula[0];
            }          

            for (int i = 0; i < dataView.Tables.Count; i++)
            {
                for (int j = 0; j < dataView.Tables[i].Fields.Count; j++)
                {

                    if (valueCategory.ToLower().Trim() == dataView.Tables[i].Database.ToLower().Trim())
                    {
                        if (dataView.Tables[i].Alias != null && dataView.Tables[i].Alias != string.Empty)
                        {
                            if (valueTable.ToLower().Trim() == dataView.Tables[i].Alias.ToLower().Trim())
                            {
                                if (dataView.Tables[i].Fields[j].Alias.ToLower().Trim() == valueField.ToLower().Trim())
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            if (valueTable.ToLower().Trim() == dataView.Tables[i].Name.ToLower().Trim())
                            {
                                if (dataView.Tables[i].Fields[j].Alias.ToLower().Trim() == valueField.ToLower().Trim())
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public string GetSearchColumnInfo(int Col, object Sheet)
        {
            return GetCell(Global.CBVHeaderRow.HeaderWhereRow, Col, Sheet);
        }

        public string GetSearchOptionInfo(int Col, object Sheet)
        {
            return GetCell(Global.CBVHeaderRow.HeaderOptionRow, Col, Sheet);
        }

        #endregion
    }
}
