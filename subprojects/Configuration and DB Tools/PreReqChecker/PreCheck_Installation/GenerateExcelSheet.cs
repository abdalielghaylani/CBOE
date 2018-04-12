using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Reflection;
using Microsoft.Office.Interop.Excel;
using System.IO;


namespace PreCheck_Installation
{
    public class GenerateExcel
    {
        # region Constant - Static Node Vaiables

        //------ PRE CHECK NODES
        //-------------------------------------------
        string _PreCheckAttrName = "Name";
        string _PreCheckNodeName = "PreCheck";
        //-------------------------------------------

        string _FormatSep = ":";
        string _GapSep = " ";
        string _NewLine = "\n";
        string _PrecheckListNewLine = "\n\n";

        string _PrecheckNewLine = "\n--------------------------------------------------------------------------------";

        //------ PRE CHECK SUBLIST NODES
        //-------------------------------------------
        string _PCSublistNodeName = "PreCheck SubList";
        string _PCSublistAttrName = "Name";
        //-------------------------------------------

        //------ PRE CHECK SUBLIST NODES
        //-------------------------------------------
        string _ExpectedListNodeName = "Expected List";
        //-------------------------------------------


        //------ STATUS ,ACTUAL REQUIRMENT,  MESSAGE,
        //-------------------------------------------
        string _StatusNodeName = "Status";
        string _ActualReqNodeName = "Actual Result";
        string _MessageNodeName = "Message";
        //-------------------------------------------

        # endregion

        # region Vaiables
        string _PreCheck = string.Empty;
        string _Exception = string.Empty;
        List<object> _GenerateXMLList = new List<object>();
        Hashtable _hPreCheckSubList = new Hashtable();
        Hashtable _hExpectedList = new Hashtable();
        Hashtable _hActualReq = new Hashtable();
        Hashtable _hStatus = new Hashtable();
        Hashtable _hMessage = new Hashtable();
        string _PrecheckOutput = string.Empty;
        string TPrecheckConcatinate = string.Empty;
        string TPrecheckRootText = string.Empty;
        string TPreCheckText = "";
        string TPrecheckSublist = string.Empty;
        string TStatus = string.Empty;
        string TExpectedList = string.Empty;
        string TMessage = string.Empty;
        string TActualResult = string.Empty;
        string RootDocument = string.Empty;
        # endregion

        #region Public Properties


        #region String Properties
        public string PreCheckName
        {
            get
            {
                return _PreCheck;
            }
            set
            {
                _PreCheck = value;
            }
        }
        public string MakeEmpty
        {
            set
            {
                TPrecheckConcatinate = value;
                TPrecheckRootText = value;
                TPreCheckText = value;
                TPrecheckSublist = value;
                TStatus = value;
                TExpectedList = value;
                TMessage = value;
                TActualResult = value;
                _PreCheck = value;
            }
        }
        # endregion

        #region Collection Properties
        public List<object> ObjectList
        {
            get
            {
                return _GenerateXMLList;
            }
            set
            {
                _GenerateXMLList = value;
                ProcessSystemCheck();
                ProcessSysCheck_Xls();
            }
        }
        public Hashtable ExpectedValueList
        {
            get
            {
                return _hExpectedList;
            }
            set
            {
                _hExpectedList = value;
            }
        }
        public Hashtable PreCheckSubList
        {
            get
            {
                return _hPreCheckSubList;
            }
            set
            {
                _hPreCheckSubList = value;

            }
        }
        public Hashtable Status
        {
            get
            {
                return _hStatus;
            }
            set
            {
                _hStatus = value;

            }
        }
        public Hashtable ActualValue
        {
            get
            {
                return _hActualReq;
            }
            set
            {
                _hActualReq = value;
            }
        }
        public Hashtable Message
        {
            get
            {
                return _hMessage;
            }
            set
            {
                _hMessage = value;
            }
        }
        # endregion

        #region XmlDocument Properties
        public string GetPreCheckXMLDoc
        {
            get
            {
                return RootDocument;
            }

        }
        # endregion
        #endregion

        # region Functions
        private string CreatePrecheckNode(string Name)
        {
            if (Name.Trim().Length == 0)
            {
                return "";
            }
            return _PreCheckNodeName + _GapSep + _PreCheckAttrName + _FormatSep + _GapSep + Name + _PrecheckListNewLine;
        }
        private string CreatePCSublistNode<T>(T Name)
        {
            if (Name.ToString().Trim().Length == 0)
            {
                return "";
            }
            return _PCSublistNodeName + _GapSep + _PCSublistAttrName + _GapSep + _FormatSep + _GapSep + Name.ToString() + _NewLine;
        }
        private string CreateStatusNode<T>(T Name)
        {
            try
            {
                if (Name.ToString().Trim().Length == 0)
                {
                    return "";
                }
                return _StatusNodeName + _GapSep + _FormatSep + _GapSep + Name.ToString() + _NewLine;
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
                return null;
            }
        }
        private string CreateActualReqNode<T>(T Name)
        {
            try
            {
                if (Name.ToString().Trim().Length == 0)
                {
                    return "";
                }
                return _ActualReqNodeName + _GapSep + _FormatSep + _GapSep + Name.ToString() + _NewLine;
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
                return null;
            }
        }
        private string CreateMessageNode<T>(T Name)
        {
            try
            {
                if (Name.ToString().Trim().Length == 0)
                {
                    return "";
                }
                return _MessageNodeName + _GapSep + _FormatSep + _GapSep + Name.ToString() + _NewLine;
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
                return null;
            }
        }
        private string CreateExpectedListRootNode(Hashtable _hExpectedList, string sDupHashname_ExpectedList)
        {
            try
            {
                string ExpectedValues = string.Empty;
                string[] ArrayExpectedList = { "" };

                int nExpectedCount = 0;
                foreach (DictionaryEntry Key_ExpList in _hExpectedList)
                {
                    if (Key_ExpList.Key.ToString().StartsWith(sDupHashname_ExpectedList.ToString()))
                    {
                        Array.Resize(ref ArrayExpectedList, nExpectedCount + 1);
                        ArrayExpectedList[nExpectedCount] = Key_ExpList.Value.ToString();
                        nExpectedCount = nExpectedCount + 1;
                    }
                    // create node Expected
                }
                ExpectedValues = string.Join(",", ArrayExpectedList);
                if (ExpectedValues.ToString().Trim().Length == 0)
                {
                    return "";
                }
                return _ExpectedListNodeName + _GapSep + _FormatSep + _GapSep + ExpectedValues + _NewLine;
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
                return null;
            }
        }
        # endregion

        # region Methods
        private void SetPropertys(ref PropertyInfo[] propertyInfos, object myObject)
        {
            try
            {
                string PropertyName = string.Empty;


                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    PropertyName = propertyInfo.Name;
                    object oPropValue = propertyInfo.GetValue(myObject, null);

                    if (PropertyName.ToUpper() == "PRECHECKNAME")
                    {
                        this.PreCheckName = Convert.ToString(oPropValue);
                    }
                    else if (PropertyName.ToUpper() == "PRECHECKSUBLIST")
                    {
                        this.PreCheckSubList = ((Hashtable)oPropValue);
                    }
                    else if (PropertyName.ToUpper() == "STATUS")
                    {
                        this.Status = ((Hashtable)oPropValue);
                    }
                    else if (PropertyName.ToUpper() == "EXPECTEDVALUELIST")
                    {
                        this.ExpectedValueList = ((Hashtable)oPropValue);
                    }
                    else if (PropertyName.ToUpper() == "ACTUALVALUE")
                    {
                        this.ActualValue = ((Hashtable)oPropValue);
                    }
                    else if (PropertyName.ToUpper() == "MESSAGE")
                    {
                        this.Message = ((Hashtable)oPropValue);
                    }
                }

            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
            }
        }
        private void ProcessSystemCheck()
        {
            try
            {

                foreach (object obj in _GenerateXMLList)
                {

                    Type t = (obj).GetType();
                    PropertyInfo[] propertyInfos = t.GetProperties();
                    SetPropertys(ref propertyInfos, obj);

                    _PreCheck = this.PreCheckName;  // retrive precheck name
                    TPreCheckText = CreatePrecheckNode(_PreCheck);

                    _hPreCheckSubList = this.PreCheckSubList;
                    _hStatus = this.Status;
                    _hActualReq = this.ActualValue;
                    _hMessage = this.Message;
                    _hExpectedList = this.ExpectedValueList;


                    int n_StatusCount = 0;

                    if (!(_hPreCheckSubList.Count == 0))
                    {
                        for (int i = 0; i <= _hPreCheckSubList.Count - 1; i++)
                        {
                            object Key_PCList = _hPreCheckSubList[i];
                            object sDupHashname_Status = n_StatusCount;
                            object sDupHashname_Requirment = n_StatusCount;
                            object sDupHashname_Message = n_StatusCount;
                            object sDupHashname_ExpectedList = Convert.ToString(n_StatusCount + 1);

                            if (_hStatus.ContainsKey(sDupHashname_Status))
                            {
                                sDupHashname_Status = _hStatus[sDupHashname_Status];
                            }
                            else
                            {
                                sDupHashname_Status = string.Empty;
                            }
                            if (_hActualReq.ContainsKey(sDupHashname_Requirment))
                            {
                                sDupHashname_Requirment = _hActualReq[sDupHashname_Requirment];
                            }
                            else
                            {
                                sDupHashname_Requirment = string.Empty;
                            }
                            if (_hMessage.ContainsKey(sDupHashname_Message))
                            {
                                sDupHashname_Message = _hMessage[sDupHashname_Message];
                            }
                            else
                            {
                                sDupHashname_Message = string.Empty;
                            }

                            TPrecheckSublist = CreatePCSublistNode(Key_PCList);
                            TStatus = CreateStatusNode(sDupHashname_Status);
                            TExpectedList = CreateExpectedListRootNode(_hExpectedList, sDupHashname_ExpectedList.ToString());
                            TMessage = CreateMessageNode(sDupHashname_Message);
                            TActualResult = CreateActualReqNode(sDupHashname_Requirment);
                            n_StatusCount = n_StatusCount + 1;


                            TPrecheckConcatinate = TPrecheckConcatinate + TPrecheckSublist;
                            TPrecheckConcatinate = TPrecheckConcatinate + TActualResult;
                            TPrecheckConcatinate = TPrecheckConcatinate + TExpectedList;
                            TPrecheckConcatinate = TPrecheckConcatinate + TStatus;
                            TPrecheckConcatinate = TPrecheckConcatinate + TMessage;
                            TPrecheckConcatinate = TPrecheckConcatinate + _PrecheckListNewLine;
                        }
                    }

                    RootDocument = RootDocument + TPreCheckText + TPrecheckConcatinate.Trim() + _PrecheckNewLine;
                    // RootDocument = RootDocument.Trim();
                    MakeEmpty = "";
                }
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
            }
        }
        # endregion














        # region ExcelObject



        int _RowindexS_Mrg = 0;
        int _ColindexS_Mrg = 0;

        int _RowindexE_Mrg = 0;
        int _ColindexE_Mrg = 0;
        int _PropCounter = 0;

        public int Rowindex_D
        {
            get
            {
                return _Rowindex_D;
            }
            set
            {
                _Rowindex_D = _Rowindex_Inc + value;
            }
        }
        public int Colindex_D
        {
            get
            {
                return _Colindex_D;
            }
            set
            {
                _Colindex_D = _Colindex_Inc + value;
            }
        }



        public int RowindexSub_D
        {
            get
            {
                return _RowindexSub_D;
            }
            set
            {
                _RowindexSub_D = _Rowindex_Inc + value;
            }
        }
        public int ColindexSub_D
        {
            get
            {
                return _ColindexSub_D;
            }
            set
            {
                _ColindexSub_D = _Colindex_Inc + value;
            }
        }


        public int Rowindex_H
        {
            get
            {
                return _Rowindex_H;
            }
            set
            {
                _Rowindex_H = _Rowindex_Inc + value;
            }
        }
        public int Colindex_H
        {
            get
            {
                return _Colindex_H;
            }
            set
            {
                _Colindex_H = _Colindex_Inc + value;
            }
        }




        public int RowindexS_Mrg
        {
            get
            {
                return _RowindexS_Mrg;
            }
            set
            {
                _RowindexS_Mrg = _PropCounter + value;
            }
        }
        public int ColindexS_Mrg
        {
            get
            {
                return _ColindexS_Mrg;
            }
            set
            {
                _ColindexS_Mrg = _PropCounter + value;
            }
        }
        public int RowindexE_Mrg
        {
            get
            {
                return _RowindexE_Mrg;
            }
            set
            {
                _RowindexE_Mrg = _PropCounter + value;
            }
        }
        public int ColindexE_Mrg
        {
            get
            {
                return _ColindexE_Mrg;
            }
            set
            {
                _ColindexE_Mrg = _PropCounter + value;
            }
        }


        private void Dummy()
        {
            try
            {
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
            }
        }

        Application xlApp;
        Workbook xlWorkBook;
        Worksheet xlWorkSheet;
        Range chartRange;


        int _Rowindex_D = 0;
        int _Colindex_D = 0;

        int _RowindexSub_D = 0;
        int _ColindexSub_D = 0;

        int _Rowindex_Inc = 1;
        int _Colindex_Inc = 1;

        int _Rowindex_H = 3;
        int _Colindex_H = 2;








        private void CreateHeaders(string HeaderName)
        {
            try
            {
                xlWorkSheet.Cells[_Rowindex_H, _Colindex_H] = HeaderName;
                chartRange = xlWorkSheet.get_Range(xlWorkSheet.Cells[_Rowindex_H, _Colindex_H], xlWorkSheet.Cells[_Rowindex_H, _Colindex_H]);
                chartRange.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium, XlColorIndex.xlColorIndexAutomatic, XlColorIndex.xlColorIndexAutomatic);
                //Rowindex_H = _Rowindex_H;
                Colindex_H = _Colindex_H;
                //workSheet_range = worksheet.get_Range(cell1, cell2);
                //workSheet_range.Merge(mergeColumns);
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
            }
        }
        private void CreatePrecheckNodexls(string Name)
        {
            try
            {
                xlWorkSheet.Cells[_Rowindex_D, _Colindex_D] = Name;
                chartRange = xlWorkSheet.get_Range(xlWorkSheet.Cells[_Rowindex_D, _Colindex_D], xlWorkSheet.Cells[_Rowindex_H, _Colindex_H]);
                chartRange.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium, XlColorIndex.xlColorIndexAutomatic, XlColorIndex.xlColorIndexAutomatic);
                // Rowindex_D = _Rowindex_D;
                // Colindex_D = _Colindex_D;
                //workSheet_range = worksheet.get_Range(cell1, cell2);
                //workSheet_range.Merge(mergeColumns);
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
            }
        }
        private void CreatePCSublistNodeXls<T>(T Name)
        {
            try
            {
                xlWorkSheet.Cells[_RowindexSub_D, _ColindexSub_D] = Name.ToString().Trim();
                chartRange = xlWorkSheet.get_Range(xlWorkSheet.Cells[_RowindexSub_D, _ColindexSub_D], xlWorkSheet.Cells[_RowindexSub_D, _ColindexSub_D]);
                chartRange.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, XlColorIndex.xlColorIndexAutomatic);

            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
            }
        }
        private void CreateActualReqNodeXls<T>(T Name)
        {
            try
            {
                xlWorkSheet.Cells[_RowindexSub_D, _ColindexSub_D] = Name.ToString().Trim();
                chartRange = xlWorkSheet.get_Range(xlWorkSheet.Cells[_RowindexSub_D, _ColindexSub_D], xlWorkSheet.Cells[_RowindexSub_D, _ColindexSub_D]);
                chartRange.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, XlColorIndex.xlColorIndexAutomatic);
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
            }
        }
        private void CreateExpectedListRootNodeXls(Hashtable _hExpectedList, string sDupHashname_ExpectedList)
        {
            try
            {
                string ExpectedValues = string.Empty;
                string[] ArrayExpectedList = { "" };

                int nExpectedCount = 0;
                foreach (DictionaryEntry Key_ExpList in _hExpectedList)
                {
                    if (Key_ExpList.Key.ToString().StartsWith(sDupHashname_ExpectedList.ToString()))
                    {
                        Array.Resize(ref ArrayExpectedList, nExpectedCount + 1);
                        ArrayExpectedList[nExpectedCount] = Key_ExpList.Value.ToString();
                        nExpectedCount = nExpectedCount + 1;
                    }
                    // create node Expected
                }
                ExpectedValues = string.Join(",", ArrayExpectedList);
                xlWorkSheet.Cells[_RowindexSub_D, _ColindexSub_D] = ExpectedValues.Trim();
                chartRange = xlWorkSheet.get_Range(xlWorkSheet.Cells[_RowindexSub_D, _ColindexSub_D], xlWorkSheet.Cells[_RowindexSub_D, _ColindexSub_D]);
                chartRange.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, XlColorIndex.xlColorIndexAutomatic);
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
            }
        }
        private void CreateStatusNodeXls<T>(T Name)
        {
            try
            {
                xlWorkSheet.Cells[_RowindexSub_D, _ColindexSub_D] = Name.ToString().Trim();
                chartRange = xlWorkSheet.get_Range(xlWorkSheet.Cells[_RowindexSub_D, _ColindexSub_D], xlWorkSheet.Cells[_RowindexSub_D, _ColindexSub_D]);
                chartRange.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, XlColorIndex.xlColorIndexAutomatic);

            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
            }
        }
        private void CreateMessageNodeXls<T>(T Name)
        {
            try
            {
                xlWorkSheet.Cells[_RowindexSub_D, _ColindexSub_D] = Name.ToString().Trim();
                chartRange = xlWorkSheet.get_Range(xlWorkSheet.Cells[_RowindexSub_D, _ColindexSub_D], xlWorkSheet.Cells[_RowindexSub_D, _ColindexSub_D]);
                chartRange.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, XlColorIndex.xlColorIndexAutomatic);

            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
            }
        }


        private void ProcessSysCheck_Xls()
        {
            try
            {
                object misValue = System.Reflection.Missing.Value;


                xlApp = new ApplicationClass();
                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);

                //------HEADERS
                //----------------------------------------------------------------
                CreateHeaders(_PreCheckNodeName);
                CreateHeaders(_PCSublistNodeName);
                CreateHeaders(_ActualReqNodeName);
                CreateHeaders(_ExpectedListNodeName);
                CreateHeaders(_StatusNodeName);
                CreateHeaders(_MessageNodeName);
                _Rowindex_Inc = 1;
                _Colindex_Inc = 1;
                //----------------------------------------------------------------

                _Rowindex_D = Rowindex_H + 1;
                Colindex_D = 1;



                //------DATA
                //----------------------------------------------------------------
                foreach (object obj in _GenerateXMLList)
                {

                    Type t = (obj).GetType();
                    PropertyInfo[] propertyInfos = t.GetProperties();
                    SetPropertys(ref propertyInfos, obj);

                    _PreCheck = this.PreCheckName;  // retrive precheck name
                    CreatePrecheckNodexls(_PreCheck);

                    _RowindexSub_D = _Rowindex_D;
                    _hPreCheckSubList = this.PreCheckSubList;
                    _hStatus = this.Status;
                    _hActualReq = this.ActualValue;
                    _hMessage = this.Message;
                    _hExpectedList = this.ExpectedValueList;


                    int n_StatusCount = 0;

                    if (!(_hPreCheckSubList.Count == 0))
                    {
                        for (int i = 0; i <= _hPreCheckSubList.Count - 1; i++)
                        {
                            object Key_PCList = _hPreCheckSubList[i];
                            object sDupHashname_Status = n_StatusCount;
                            object sDupHashname_Requirment = n_StatusCount;
                            object sDupHashname_Message = n_StatusCount;
                            object sDupHashname_ExpectedList = Convert.ToString(n_StatusCount + 1);

                            if (_hStatus.ContainsKey(sDupHashname_Status))
                            {
                                sDupHashname_Status = _hStatus[sDupHashname_Status];
                            }
                            else
                            {
                                sDupHashname_Status = string.Empty;
                            }
                            if (_hActualReq.ContainsKey(sDupHashname_Requirment))
                            {
                                sDupHashname_Requirment = _hActualReq[sDupHashname_Requirment];
                            }
                            else
                            {
                                sDupHashname_Requirment = string.Empty;
                            }
                            if (_hMessage.ContainsKey(sDupHashname_Message))
                            {
                                sDupHashname_Message = _hMessage[sDupHashname_Message];
                            }
                            else
                            {
                                sDupHashname_Message = string.Empty;
                            }

                            _ColindexSub_D = Colindex_D + 1;
                            CreatePCSublistNodeXls(Key_PCList);


                            _ColindexSub_D = _ColindexSub_D + 1;
                            CreateActualReqNodeXls(sDupHashname_Requirment);

                            _ColindexSub_D = _ColindexSub_D + 1;
                            CreateExpectedListRootNodeXls(_hExpectedList, sDupHashname_ExpectedList.ToString());

                            _ColindexSub_D = _ColindexSub_D + 1;
                            CreateStatusNodeXls(sDupHashname_Status);

                            _ColindexSub_D = _ColindexSub_D + 1;
                            CreateMessageNodeXls(sDupHashname_Message);

                            RowindexSub_D = _RowindexSub_D;
                            n_StatusCount = n_StatusCount + 1;
                        }
                        _Rowindex_D = _RowindexSub_D + 1;
                    }
                }
                //----------------------------------------------------------------



                xlWorkBook.SaveAs(System.Windows.Forms.Application.StartupPath + "\\PreCheker.xls", XlFileFormat.xlWorkbookNormal, misValue, misValue, 0, misValue, XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                releaseObject(xlApp);
                releaseObject(xlWorkBook);
                releaseObject(xlWorkSheet);
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
            }
        }


        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception Ex)
            {
                _Exception = Ex.Message + "||" + Ex.StackTrace;
            }
            finally
            {
                GC.Collect();
            }
        }


        #endregion
    }

}
