using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace ChemBioVizExcelAddIn
{
    #region _  Logging Class  _
    class Log
    {
        public Log()
        { }

        
        public static void CreateEditLogFile(string file, string logData)
        {
            try
            {
                if (File.Exists(file))
                {
                    using (StreamWriter CBVLogFile = File.AppendText(file))
                    {
                        CBVLogFile.WriteLine(DateTime.Now + " : " + logData);
                        CBVLogFile.Close();
                    }
                }
                else
                {
                    using (StreamWriter CBVLogFile = File.CreateText(file))
                    {
                        CBVLogFile.WriteLine(DateTime.Now + " : " + logData);
                        CBVLogFile.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        private static void AddText(FileStream CBVLogFile, string logData)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(logData);
            CBVLogFile.Write(info, 0, info.Length);
        }

        public static void CreateLogFile(string file)
        {
            try
            {
                if (!File.Exists(file))
                {
                    StreamWriter CBVLogFile = File.CreateText(file);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
    #endregion
    
    #region _  UserInfo Class  _

    class UserInfo : CBVExcel
    {
        public string username = String.Empty;

        public UserInfo()
        {
            username = null;
        }

        public string GetLoginUserInfo()
        {
            try
            {
                Excel::Worksheet hiddenSheet = GlobalCBVExcel.Get_CSHidden();
                Excel.Range cell = (Excel.Range)hiddenSheet.Cells[2, Global.CBVHiddenSheetHeader.LoginUser]; // the  User name has stored in row 2, column 11
                if (cell.Text.ToString() != String.Empty)
                {
                    return cell.Text.ToString(); ;
                }
                else
                {
                    return String.Empty;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       

        public bool SetUserInfo(Excel::Worksheet nSheet, string lastModifiedUser)
        {
            try
            {
                Excel::Worksheet hiddenSheet = GlobalCBVExcel.Get_CSHidden();               

                if (hiddenSheet != null)
                {
                    //UnProtectHiddenSheet(nSheet); //Un protect the hidden sheet for edit/update data.
                    for (int rows = 2; rows <= hiddenSheet.Rows.Count; rows++)
                    {
                        Excel.Range cell = (Excel.Range)hiddenSheet.Cells[rows, Global.CBVHiddenSheetHeader.Sheetname]; // the sheet name has stored in column 5
                        if (cell.Text.ToString() == nSheet.Name)
                        
                        {

                            GlobalCBVExcel.SetCell(rows, Global.CBVHiddenSheetHeader.ModifiedUser, hiddenSheet, lastModifiedUser);
                           
                        }
                    }
                    //ProtectHiddenSheet(nSheet);
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public  void UpdateUserInfo(string userName)
        {
            try
            {
                  if (!string.IsNullOrEmpty(userName))
                    {

                        Excel::Worksheet hiddenSheet = GlobalCBVExcel.Get_CSHidden();
                        if (hiddenSheet != null)
                            if (!userName.Equals(GlobalCBVExcel.GetCell(2, (int)Global.CBVHiddenSheetHeader.LoginUser, hiddenSheet), StringComparison.OrdinalIgnoreCase))
                            {
                                GlobalCBVExcel.SetCell(2, (int)Global.CBVHiddenSheetHeader.LoginUser, hiddenSheet, userName); // the column is fixed here
                            }
                        GC.Collect();
                        GC.WaitForPendingFinalizers();                      
                        //System.Runtime.InteropServices.Marshal.ReleaseComObject(hiddenSheet);
                    }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Excel::Worksheet HiddenSheet()
        {
            Excel::Worksheet hiddenSheet = GlobalCBVExcel.FindSheet(Global.COEDATAVIEW_HIDDENSHEET);
            return hiddenSheet;
        }

        public bool ISHiddenSheetExists()
        {
            Excel::Worksheet hiddenSheet = GlobalCBVExcel.FindSheet(Global.COEDATAVIEW_HIDDENSHEET);
            if (hiddenSheet != null)
                return true;
            else
                return false;
        }
    }

    #endregion
}
