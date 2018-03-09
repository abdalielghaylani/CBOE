using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Office = Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections;

namespace ChemBioVizExcelAddIn
{
    class DisplayOptionHeader : IComparable
    {
        private string option;


        public DisplayOptionHeader(string option)
        {
            this.option = option;
        }

        public string Option
        {
            get { return option; }
            set { option = value; }
        }
        public int CompareTo(object obj)
        {
            DisplayOptionHeader displayOptionHeader = (DisplayOptionHeader)obj;
            return this.Option.CompareTo(displayOptionHeader);
        }
    }

    class DisplayOption
    {
        private string option;

        public string Option
        {
            get { return option; }
            set { option = value; }
        }

        public static List<string> OptionHeaderList(string colType, string indexType, string mimeType)
        {
            if (string.Empty == colType || null == colType)
                return null;
            //CBOE-305 
            List<string> displayOptionList = new List<string>();
            string DefaultHeaderOption = AppConfigSetting.ReadSetting("DefaultDisplayHeaderOption");
            //if (colType.ToUpper() == "NUMBER" || colType.ToUpper() == "FLOAT" || colType.ToUpper() == "INTEGER" || colType.ToUpper() == "REAL" || colType.ToUpper() == "DECIMAL" || colType.ToUpper() == "NUMERIC" || colType.ToUpper() == "SMALLINT" || colType.ToUpper() == "DOUBLE" || colType.ToUpper() == "LONG" || colType.ToUpper() == "DOUBLE PRECISION" || colType.ToUpper() == "DEC")
            if ((colType.ToUpper() == "NUMBER" || colType.ToUpper() == "FLOAT" || colType.ToUpper() == "INTEGER" || colType.ToUpper() == "REAL" || colType.ToUpper() == "DECIMAL" || colType.ToUpper() == "NUMERIC" || colType.ToUpper() == "SMALLINT" || colType.ToUpper() == "DOUBLE" || colType.ToUpper() == "LONG" || colType.ToUpper() == "DOUBLE PRECISION" || colType.ToUpper() == "DEC") || (colType.ToUpper() == "TEXT" && mimeType.ToUpper() == "MOLWEIGHT") || (colType.ToUpper() == "CLOB" && mimeType.ToUpper() == "MOLWEIGHT"))
            {
                if (DefaultHeaderOption != null)
                {
                    displayOptionList.Add(DefaultHeaderOption);
                }
                else
                {
                    foreach (Global.CBVDisplayHeaderOptionNumeric dispOption in Enum.GetValues(typeof(Global.CBVDisplayHeaderOptionNumeric)))
                        displayOptionList.Add(StringEnum.GetStringValue(dispOption));
                }

                return displayOptionList;
            }
            else if ((colType.ToUpper() == "TEXT" || colType.ToUpper() == "CLOB") && mimeType.ToUpper() == "FORMULA")
            {
                if (DefaultHeaderOption != null)
                {
                    displayOptionList.Add(DefaultHeaderOption);
                }
                else
                {
                    foreach (Global.CBVDisplayHeaderOptionText dispOption in Enum.GetValues(typeof(Global.CBVDisplayHeaderOptionText)))
                        displayOptionList.Add(StringEnum.GetStringValue(dispOption));
                }
                return displayOptionList;
            }
            else if (colType.ToUpper() == "CLOB" || indexType.ToUpper() == Global.COESTRUCTURE_INDEXTYPE)
            {
                foreach (Global.CBVDisplayHeaderOptionStructure dispOption in Enum.GetValues(typeof(Global.CBVDisplayHeaderOptionStructure)))
                    displayOptionList.Add(StringEnum.GetStringValue(dispOption));
                return displayOptionList;

            }
            else if (colType.ToUpper() == "BLOB" || colType.ToUpper() == "BYTE" || Global.ISImageTypeExists(mimeType))
            {
                foreach (Global.CBVDisplayHeaderOptionOLE dispOption in Enum.GetValues(typeof(Global.CBVDisplayHeaderOptionOLE)))
                    displayOptionList.Add(StringEnum.GetStringValue(dispOption));
                return displayOptionList;

            }
            else if (colType.ToUpper() == "DATE")
            {
                if (DefaultHeaderOption != null)
                {
                    displayOptionList.Add(DefaultHeaderOption);
                }
                else
                {
                    foreach (Global.CBVDisplayHeaderOptionDate dispOption in Enum.GetValues(typeof(Global.CBVDisplayHeaderOptionDate)))
                        displayOptionList.Add(StringEnum.GetStringValue(dispOption));
                }
                return displayOptionList;
            }

            else if (colType.ToUpper() == "TEXT" || colType.ToUpper() == "VARCHAR" || colType.ToUpper() == "VARCHAR2" || colType.ToUpper() == "NCHAR" || colType.ToUpper() == "NVARCHAR2" || colType.ToUpper() == "BOOLEAN" || colType.ToUpper() == "BOOL") //CSBR-163077 (Boolean field)
            {
                if (DefaultHeaderOption != null)
                {
                    displayOptionList.Add(DefaultHeaderOption);
                }
                else
                {
                    foreach (Global.CBVDisplayHeaderOptionText dispOption in Enum.GetValues(typeof(Global.CBVDisplayHeaderOptionText)))
                        displayOptionList.Add(StringEnum.GetStringValue(dispOption));
                }
                return displayOptionList;
            }
            else
            {
                displayOptionList.Add("Undefined DataType");
                return displayOptionList;
            }
        }
        public static List<string> HeaderList(string colType, string indexType, string mimeType)
        {
            if (string.Empty == colType || null == colType)
                return null;
            List<string> displayOptionList = new List<string>();
            //if (colType.ToUpper() == "NUMBER" || colType.ToUpper() == "FLOAT" || colType.ToUpper() == "INTEGER" || colType.ToUpper() == "REAL" || colType.ToUpper() == "DECIMAL" || colType.ToUpper() == "NUMERIC" || colType.ToUpper() == "SMALLINT" || colType.ToUpper() == "DOUBLE" || colType.ToUpper() == "LONG" || colType.ToUpper() == "DOUBLE PRECISION" || colType.ToUpper() == "DEC")
            if ((colType.ToUpper() == "NUMBER" || colType.ToUpper() == "FLOAT" || colType.ToUpper() == "INTEGER" || colType.ToUpper() == "REAL" || colType.ToUpper() == "DECIMAL" || colType.ToUpper() == "NUMERIC" || colType.ToUpper() == "SMALLINT" || colType.ToUpper() == "DOUBLE" || colType.ToUpper() == "LONG" || colType.ToUpper() == "DOUBLE PRECISION" || colType.ToUpper() == "DEC") || (colType.ToUpper() == "TEXT" && mimeType.ToUpper() == "MOLWEIGHT") || (colType.ToUpper() == "CLOB" && mimeType.ToUpper() == "MOLWEIGHT"))
            {
                
                    foreach (Global.CBVDisplayHeaderOptionNumeric dispOption in Enum.GetValues(typeof(Global.CBVDisplayHeaderOptionNumeric)))
                        displayOptionList.Add(StringEnum.GetStringValue(dispOption));
                    return displayOptionList;
            }
            else if ((colType.ToUpper() == "TEXT" || colType.ToUpper() == "CLOB") && mimeType.ToUpper() == "FORMULA")
            {
                    foreach (Global.CBVDisplayHeaderOptionText dispOption in Enum.GetValues(typeof(Global.CBVDisplayHeaderOptionText)))
                        displayOptionList.Add(StringEnum.GetStringValue(dispOption));
                    return displayOptionList;
            }
            else if (colType.ToUpper() == "CLOB" || indexType.ToUpper() == Global.COESTRUCTURE_INDEXTYPE)
            {
                foreach (Global.CBVDisplayHeaderOptionStructure dispOption in Enum.GetValues(typeof(Global.CBVDisplayHeaderOptionStructure)))
                    displayOptionList.Add(StringEnum.GetStringValue(dispOption));
                return displayOptionList;

            }
            else if (colType.ToUpper() == "BLOB" || colType.ToUpper() == "BYTE" || Global.ISImageTypeExists(mimeType))
            {
                foreach (Global.CBVDisplayHeaderOptionOLE dispOption in Enum.GetValues(typeof(Global.CBVDisplayHeaderOptionOLE)))
                    displayOptionList.Add(StringEnum.GetStringValue(dispOption));
                return displayOptionList;

            }
            else if (colType.ToUpper() == "DATE")
            {
                
                    foreach (Global.CBVDisplayHeaderOptionDate dispOption in Enum.GetValues(typeof(Global.CBVDisplayHeaderOptionDate)))
                        displayOptionList.Add(StringEnum.GetStringValue(dispOption));
                    return displayOptionList;
            }

            else if (colType.ToUpper() == "TEXT" || colType.ToUpper() == "VARCHAR" || colType.ToUpper() == "VARCHAR2" || colType.ToUpper() == "NCHAR" || colType.ToUpper() == "NVARCHAR2" || colType.ToUpper() == "BOOLEAN" || colType.ToUpper() == "BOOL") //CSBR-163077 (Boolean field)
            {
                    foreach (Global.CBVDisplayHeaderOptionText dispOption in Enum.GetValues(typeof(Global.CBVDisplayHeaderOptionText)))
                        displayOptionList.Add(StringEnum.GetStringValue(dispOption));
                    return displayOptionList;
            }
            else
            {
                displayOptionList.Add("Undefined DataType");
                return displayOptionList;
            }
        }


        public static string OptionHeaderDataType(string colType, string indexType, string mimeType)
        {
            if (colType.ToUpper() == "NUMBER" || colType.ToUpper() == "FLOAT" || colType.ToUpper() == "INTEGER" || colType.ToUpper() == "REAL" || colType.ToUpper() == "DECIMAL" || colType.ToUpper() == "NUMERIC" || colType.ToUpper() == "SMALLINT" || colType.ToUpper() == "DOUBLE" || colType.ToUpper() == "LONG" || colType.ToUpper() == "DOUBLE PRECISION" || colType.ToUpper() == "DEC")
            {
                return "NUMBER";
            }
            else if (colType.ToUpper() == "CLOB" || indexType.ToUpper() == "CS_CARTRIDGE")
            {
                return "STRUCTURE";
            }
            else if (colType.ToUpper() == "BLOB" || (indexType.ToUpper() == "NONE" && colType.ToUpper() != "TEXT"))
            {
                return "IMAGE";
            }
            else if (colType.ToUpper() == "DATE")
            {
                return "DATE";
            }
            else
            {
                return "STRING";
            }
        }

        public static Type OptionHeaderSystemDataType(string type)
        {
            if (type.ToLower()=="text")
            {
                return Type.GetType("System.String");
            }
            else if (type.ToLower() == "integer")
            {
              //  return Type.GetType("System.Int32");
                return Type.GetType(" System.Double");
            }
            else if (type.ToLower() == "double")
            {
                return Type.GetType("System.Double");
            }
            else 
            {
                return Type.GetType("System.String");
            }
        }

        public static string ListVarIntoString(List<string> lstVar)
        {
            StringBuilder strBld = new StringBuilder();
            foreach (string strVar in lstVar)
            {
                strBld.Append(strVar);
                strBld.Append("\n");
            }
            strBld.Remove(strBld.Length - 1, 1);
            return strBld.ToString();
        }

        public static string OptionCalculation(List<string> lstVar, string optionHeader)
        {
            if (lstVar == null)
                return string.Empty;
            string tableName = string.Empty;
            string result = string.Empty;
            StringBuilder strBld = new StringBuilder();
            try
            {
                //Coverity fix - CID 18792
                //if  - Not match
                if (string.IsNullOrEmpty(optionHeader))
                {
                    foreach (string strVar in lstVar)
                    {
                        strBld.Append(strVar);
                        strBld.Append("\n");
                    }

                    strBld.Remove(strBld.Length - 1, 1);
                    result = strBld.ToString();

                }
                //int cnt = 0;
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_Avg), StringComparison.OrdinalIgnoreCase))
                {
                    result = CreateFormula(lstVar, StringEnum.GetStringValue(Global.CBVHeaderOptionNumericCalculation.average));
                }
                //    else if (optionHeader == "Displays :Min")
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_Min), StringComparison.OrdinalIgnoreCase))
                {
                    result = CreateFormula(lstVar, StringEnum.GetStringValue(Global.CBVHeaderOptionNumericCalculation.minimum));
                }
                //else if (optionHeader == "Displays :Max")
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_Max), StringComparison.OrdinalIgnoreCase))
                {
                    result = CreateFormula(lstVar, StringEnum.GetStringValue(Global.CBVHeaderOptionNumericCalculation.maximum));

                }
                //else if (optionHeader == "Displays :Median")
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_Med), StringComparison.OrdinalIgnoreCase))
                {
                    result = CreateFormula(lstVar, StringEnum.GetStringValue(Global.CBVHeaderOptionNumericCalculation.median));
                }
                //else if (optionHeader == "Displays :Count")
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_Cnt), StringComparison.OrdinalIgnoreCase))
                {
                    result = CreateFormula(lstVar, StringEnum.GetStringValue(Global.CBVHeaderOptionNumericCalculation.count));
                }
                //else if (optionHeader == "Displays :All (1 cell)")
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_1cell), StringComparison.OrdinalIgnoreCase))
                {
                    foreach (string strVar in lstVar)
                    {
                        strBld.Append(strVar);
                        strBld.Append("\n");
                    }
                    strBld.Remove(strBld.Length - 1, 1);
                    result = strBld.ToString();
                }
                //else if (optionHeader == "Displays :All (1 cell A-Z)")
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_1cellAZ), StringComparison.OrdinalIgnoreCase))
                {
                    List<string> lstVarNew = new List<string>();
                    lstVarNew = lstVar;
                    lstVarNew.Sort();
                    foreach (string strVar in lstVarNew)
                    {
                        strBld.Append(strVar);
                        strBld.Append("\n");
                    }
                    strBld.Remove(strBld.Length - 1, 1);
                    result = strBld.ToString();
                }
                //else if (optionHeader == "Displays :All (1 cell Z-A)")
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_1cellZA), StringComparison.OrdinalIgnoreCase))
                {
                    List<string> lstVarNew = new List<string>();
                    lstVarNew = lstVar;
                    lstVarNew.Sort();
                    lstVarNew.Reverse();
                    foreach (string strVar in lstVarNew)
                    {
                        strBld.Append(strVar);
                        strBld.Append("\n");
                    }
                    strBld.Remove(strBld.Length - 1, 1);
                    result = strBld.ToString();
                }


                //else if (optionHeader == "Displays :All (Split Cells)")
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_Splitcells), StringComparison.OrdinalIgnoreCase))
                {
                    foreach (string strVar in lstVar)
                    {
                        strBld.Append(strVar);
                        strBld.Append(Global.SPLITCHAR);
                    }
                    strBld.Remove(strBld.Length - 1, 1);
                    result = strBld.ToString();
                }
                // else if (optionHeader == "Displays :All (Split Cells A-Z)")
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_SplitcellsAZ), StringComparison.OrdinalIgnoreCase))
                {
                    List<string> lstVarNew = new List<string>();
                    lstVarNew = lstVar;
                    lstVarNew.Sort();
                    foreach (string strVar in lstVarNew)
                    {
                        strBld.Append(strVar);
                        strBld.Append("\n");
                    }
                    strBld.Remove(strBld.Length - 1, 1);
                    result = strBld.ToString();
                }
                //else if (optionHeader == "Displays :All (Split Cells Z-A)")
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_SplitcellsZA), StringComparison.OrdinalIgnoreCase))
                {
                    List<string> lstVarNew = new List<string>();
                    lstVarNew = lstVar;
                    lstVarNew.Sort();
                    lstVarNew.Reverse();
                    foreach (string strVar in lstVarNew)
                    {
                        strBld.Append(strVar);
                        strBld.Append("\n");
                    }
                    strBld.Remove(strBld.Length - 1, 1);
                    result = strBld.ToString();
                }

                 //else if (optionHeader == "Displays :All (Multiple Rows)")
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_Multiplerows), StringComparison.OrdinalIgnoreCase))
                {
                    foreach (string strVar in lstVar)
                    {
                        strBld.Append(strVar);
                        strBld.Append(Global.SPLITCHAR);
                    }
                    strBld.Remove(strBld.Length - 1, 1);
                    result = strBld.ToString();
                }
                // else if (optionHeader == "Displays :All (Multiple Rows A-Z)")
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_MultiplerowsAZ), StringComparison.OrdinalIgnoreCase))
                {
                    List<string> lstVarNew = new List<string>();
                    lstVarNew = lstVar;
                    lstVarNew.Sort();
                    foreach (string strVar in lstVarNew)
                    {
                        strBld.Append(strVar);
                        strBld.Append("\n");
                    }
                    strBld.Remove(strBld.Length - 1, 1);
                    result = strBld.ToString();
                }
                //else if (optionHeader == "Displays :All (Multiple Rows Z-A)")
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_MultiplerowsZA), StringComparison.OrdinalIgnoreCase))
                {
                    List<string> lstVarNew = new List<string>();
                    lstVarNew = lstVar;
                    lstVarNew.Sort();
                    lstVarNew.Reverse();
                    foreach (string strVar in lstVarNew)
                    {
                        strBld.Append(strVar);
                        strBld.Append("\n");
                    }
                    strBld.Remove(strBld.Length - 1, 1);
                    result = strBld.ToString();
                }
                   
                else if ((optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionOLE.Img_1cell100), StringComparison.OrdinalIgnoreCase)) || (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionOLE.Img_1cell50), StringComparison.OrdinalIgnoreCase)) || (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionOLE.Img_1cell75), StringComparison.OrdinalIgnoreCase)) || (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionOLE.Img_Splitcells), StringComparison.OrdinalIgnoreCase)) || (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionOLE.Img_Multiplecells), StringComparison.OrdinalIgnoreCase)))
                {
                    foreach (string strVar in lstVar)
                    {
                        strBld.Append(strVar);
                        strBld.Append("\n");
                    }
                    strBld.Remove(strBld.Length - 1, 1);
                    result = strBld.ToString();
                }
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
            return result;
        }

        public static string OptionCalculation(string resVal, string optionHeader)
        {
            //If optionHeader have an structure search.
            foreach (Global.CBVDisplayHeaderOptionStructure option in Enum.GetValues(typeof(Global.CBVDisplayHeaderOptionStructure)))
            {
                if (optionHeader.Equals(StringEnum.GetStringValue(option), StringComparison.OrdinalIgnoreCase))
                    return resVal;
            }

            string tableName = string.Empty;
            string result = string.Empty;
            try
            {
                List<string> lstVar = new List<string>();
                foreach (string var in resVal.Split('\n'))
                {
                    if (string.Empty != var)
                        lstVar.Add(var);
                }

                StringBuilder strBld = new StringBuilder();

                int cnt = 0;
                if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_Avg), StringComparison.OrdinalIgnoreCase))
                {
                    decimal Avgvalue = 0;
                    foreach (string strVar in lstVar)
                    {
                        cnt++;
                        Avgvalue = Avgvalue + Convert.ToDecimal(strVar);
                    }
                    Avgvalue = Avgvalue / cnt;
                    result = Avgvalue.ToString();
                }
                //   else if (optionHeader == "Displays :Min")
                if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_Min), StringComparison.OrdinalIgnoreCase))
                {
                    List<Int32> lstMin = new List<int>();
                    lstMin.Clear();
                    foreach (string strVar in lstVar)
                    {
                        lstMin.Add(Convert.ToInt32(strVar));
                    }
                    //int min = lstMin[0];
                    int min = 0;
                    if (lstMin.Count > 0)
                        min = lstMin[0];

                    for (int i = 0; i < lstMin.Count; i++)
                    {
                        if (lstMin[i] < min)
                        {
                            min = lstMin[i];
                        }
                    }
                    result = min.ToString();
                }
                if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_Max), StringComparison.OrdinalIgnoreCase))
                {
                    List<Int32> lstMax = new List<int>();
                    lstMax.Clear();
                    foreach (string strVar in lstVar)
                    {
                        lstMax.Add(Convert.ToInt32(strVar));
                    }
                    //int max = lstMax[0];
                    int max = 0;
                    for (int i = 0; i < lstMax.Count; i++)
                    {
                        if (lstMax[i] > max)
                        {
                            max = lstMax[i];
                        }
                    }
                    result = max.ToString();
                }
                if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_Med), StringComparison.OrdinalIgnoreCase))
                {
                    List<Int32> lstMedian = new List<int>();
                    lstMedian.Clear();
                    foreach (string strVar in lstVar)
                    {
                        lstMedian.Add(Convert.ToInt32(strVar));
                    }
                    int size = lstMedian.Count;
                    int mid = size / 2;
                    double median = (size % 2 != 0) ? (double)lstMedian[mid] : ((double)lstMedian[mid] + (double)lstMedian[mid - 1]) / 2;
                    result = median.ToString();
                }
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_Cnt), StringComparison.OrdinalIgnoreCase))
                {
                    cnt = lstVar.Count;
                    result = cnt.ToString();
                }
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_1cell), StringComparison.OrdinalIgnoreCase))
                {
                    foreach (string strVar in lstVar)
                    {
                        strBld.Append(strVar);
                        strBld.Append("\n");
                    }
                    if (strBld.Length > 0)
                    {
                        strBld.Remove(strBld.Length - 1, 1);
                    }
                    result = strBld.ToString();
                }
                //else if (optionHeader == "Displays :All (Split Cells)")
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_Splitcells), StringComparison.OrdinalIgnoreCase))
                {
                    foreach (string strVar in lstVar)
                    {
                        strBld.Append(strVar);
                        strBld.Append(Global.SPLITCHAR);
                    }
                    if (strBld.Length > 0)
                    {
                        strBld.Remove(strBld.Length - 1, 1);
                    }
                    result = strBld.ToString();
                }
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_SplitcellsAZ), StringComparison.OrdinalIgnoreCase))
                {
                    List<string> lstVarNew = new List<string>();
                    lstVarNew = lstVar;
                    lstVarNew.Sort();
                    foreach (string strVar in lstVarNew)
                    {
                        strBld.Append(strVar);
                        strBld.Append("\n");
                    }
                    if (strBld.Length > 0)
                    {
                        strBld.Remove(strBld.Length - 1, 1);
                    }
                    result = strBld.ToString();
                }
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_SplitcellsZA), StringComparison.OrdinalIgnoreCase))
                {
                    List<string> lstVarNew = new List<string>();
                    lstVarNew = lstVar;
                    lstVarNew.Sort();
                    lstVarNew.Reverse();
                    foreach (string strVar in lstVarNew)
                    {
                        strBld.Append(strVar);
                        strBld.Append("\n");
                    }
                    if (strBld.Length > 0)
                    {
                        strBld.Remove(strBld.Length - 1, 1);
                    }
                    result = strBld.ToString();
                }
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_Multiplerows), StringComparison.OrdinalIgnoreCase))
                {
                    foreach (string strVar in lstVar)
                    {
                        strBld.Append(strVar);
                        strBld.Append(Global.SPLITCHAR);
                    }
                    if (strBld.Length > 0)
                    {
                        strBld.Remove(strBld.Length - 1, 1);
                    }
                    result = strBld.ToString();
                }
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_MultiplerowsAZ), StringComparison.OrdinalIgnoreCase))
                {
                    List<string> lstVarNew = new List<string>();
                    lstVarNew = lstVar;
                    lstVarNew.Sort();
                    foreach (string strVar in lstVarNew)
                    {
                        strBld.Append(strVar);
                        strBld.Append("\n");
                    }
                    if (strBld.Length > 0)
                    {
                        strBld.Remove(strBld.Length - 1, 1);
                    }
                    result = strBld.ToString();
                }
                else if (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionNumeric.Disp_All_MultiplerowsZA), StringComparison.OrdinalIgnoreCase))
                {
                    List<string> lstVarNew = new List<string>();
                    lstVarNew = lstVar;
                    lstVarNew.Sort();
                    lstVarNew.Reverse();
                    foreach (string strVar in lstVarNew)
                    {
                        strBld.Append(strVar);
                        strBld.Append("\n");
                    }
                    if (strBld.Length > 0)
                    {
                        strBld.Remove(strBld.Length - 1, 1);
                    }
                    result = strBld.ToString();
                }

                   
                else if ((optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionOLE.Img_1cell100), StringComparison.OrdinalIgnoreCase)) || (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionOLE.Img_1cell50), StringComparison.OrdinalIgnoreCase)) || (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionOLE.Img_1cell75), StringComparison.OrdinalIgnoreCase)) || (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionOLE.Img_Splitcells), StringComparison.OrdinalIgnoreCase)) || (optionHeader.Equals(StringEnum.GetStringValue(Global.CBVDisplayHeaderOptionOLE.Img_Multiplecells), StringComparison.OrdinalIgnoreCase)))
                {
                    foreach (string strVar in lstVar)
                    {
                        strBld.Append(strVar);
                        strBld.Append("\n");
                    }
                    if (strBld.Length > 0)
                    {
                        strBld.Remove(strBld.Length - 1, 1);
                    }
                    result = strBld.ToString();
                }
                //
                else if (optionHeader == null || optionHeader == String.Empty)
                {
                    foreach (string strVar in lstVar)
                    {
                        strBld.Append(strVar);
                        strBld.Append("\n");
                    }
                    if (strBld.Length > 0)
                    {
                        strBld.Remove(strBld.Length - 1, 1);
                    }
                    result = strBld.ToString();
                }
            }

            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }

            finally
            {
                //if anyhow the calculation fails, return the incoming value
                if (string.Empty == result)
                    result = resVal;
            }

            return result;
        }

        public static string CreateFormula(List<string> lstVar, string formulaName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string var in lstVar)
            {
                sb.AppendFormat("{0},", var);
            }
            string result = "=" + formulaName + "(" + sb.ToString().TrimEnd(Convert.ToChar(",")) + ")";
            return result;
        }
    }
}

