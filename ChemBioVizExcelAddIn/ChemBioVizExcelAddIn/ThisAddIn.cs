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
using CambridgeSoft.COE.Framework.COEExportService;
using System.Drawing;
using System.Diagnostics;
using System.Net;
using CambridgeSoft.COE.Framework.ServerControls.Login;
using Microsoft.Office.Core;

namespace ChemBioVizExcelAddIn
{
    public partial class ThisAddIn
    {
        //TODO: Add to this enum to support office versions above 2013
        enum OfficeVersions { V2003 = 11, V2007 = 12, V2010 = 14, V2013 = 15 };

        # region _ Variables _

        Excel.Application xlApp;
        Excel.AppEvents_WorkbookBeforeCloseEventHandler EventDel_BeforeBookClose;

        Excel.AppEvents_WorkbookActivateEventHandler EventDel_WorkBookActive;
        Excel.AppEvents_WorkbookDeactivateEventHandler EventDel_WorkBookDeactive;
        Excel.AppEvents_WorkbookOpenEventHandler EventDel_WorkBookOpen;
        /// List of variables used to create command bar       
        private Office.CommandBar commandBar;
        private Office.CommandBarPopup pBtnCBVSetup;
        private Office.CommandBarPopup pBtnCBVSearch;
        private Office.CommandBarPopup pBtnCBVOptions;
        private Office.CommandBarPopup pBtnCBVHelp;

        private Office.CommandBarButton NewChemBioVizWorksheet;
        private Office.CommandBarButton btnLogIn;

        private Office.CommandBarButton btnInsertCBVCategoryList;

        // 11.0.4
        private Office.CommandBarButton btnDeleteCBVSheetColumn;

        private Office.CommandBarButton btnCBVOptions;
        private Office.CommandBarButton btnToggleColumnAutosizing;
        private Office.CommandBarButton btnToggleHeaderDisplay;

        private Office.CommandBarButton btnVerifyCurrentCBVSheet;
        private Office.CommandBarButton btnCreateListfromSelection;
        private Office.CommandBarButton btnClearCurrentResults;
        private Office.CommandBarButton btnClearSearchCriteria;

        private Office.CommandBarButton btnToggleErrorLogging;
        private Office.CommandBarButton btnSearchPrefs;
        private Office.CommandBarButton btnSheetProperties;
        private Office.CommandBarButton btnShowDocument;
        private Office.CommandBarButton btnSelectData;

        private Office.CommandBarButton btnUpdateCurrentResults;
        private Office.CommandBarButton btnAppendNewResults;
        private Office.CommandBarButton btnReplaceAllResults;
        private Office.CommandBarButton btnReplaceAllResultsSub;
        private Office.CommandBarButton btnExportResults;

        // Commandbar button for help menu
        private Office.CommandBarButton btnContents;
        private Office.CommandBarButton btnAbout;

        public string _username;
        public string _password;

        private frmDataviewList dataviewListForm;
        bool gbClosing;
        int cntEventOccur = 0;

        // 11.0.4
        int cntColumns = 0;

        int headerOptionHitRow = 5; // Variable used for to indicate the header option row no
        bool optionStructureSearch = false; //Variable for calling replace function on option header change of structure field.

        Excel.AppEvents_SheetChangeEventHandler EventDel_SheetChange;
        Excel.AppEvents_SheetSelectionChangeEventHandler EventDel_SheetSelectionChange;
        Excel.AppEvents_SheetActivateEventHandler EventDel_SheetActivate;
        Excel.AppEvents_SheetDeactivateEventHandler EventDel_SheetDeactivate;
        Excel.AppEvents_SheetBeforeRightClickEventHandler EventDel_BeforeRightClick;

        /// <summary>
        /// The command bar name is dynamic because it contain the current version of Excel
        /// This is set in CreateToolbar() method.
        /// </summary>
        private string commandbarName;

        private const string LogInText = "Log In";
        private const string LogOutText = "Log Out";

        #endregion

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion

        #region _ Event Handler _

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
             // this.Application = (Excel.Application)Microsoft.Office.Tools.Excel.ExcelLocale1033Proxy.Wrap(typeof(Excel.Application), this.Application);

            CreateToolbar();
            AccessController.Instance.AfterLoginAttempt += new EventHandler<LoginAttemptEventArgs>(accessControler_AfterLoginAttempt);
            #region VSTO generated code

         

            Global._ExcelApp = this.Application;

            // Getting the Excel Version
            Global.CurrentExcelVersion = this.Application.Version;

            //Attache the current application and assocaited the event handler
            xlApp = this.Application;
            EventDel_BeforeBookClose = new Excel.AppEvents_WorkbookBeforeCloseEventHandler(BeforeBookClose);
            xlApp.WorkbookBeforeClose += EventDel_BeforeBookClose;

            EventDel_WorkBookActive = new Excel.AppEvents_WorkbookActivateEventHandler(WorkBookActive);
            xlApp.WorkbookActivate += EventDel_WorkBookActive;

            EventDel_WorkBookDeactive = new Excel.AppEvents_WorkbookDeactivateEventHandler(WorkBookDeactive);
            xlApp.WorkbookDeactivate += EventDel_WorkBookDeactive;

            EventDel_WorkBookOpen = new Microsoft.Office.Interop.Excel.AppEvents_WorkbookOpenEventHandler(WorkBookOpen);
            xlApp.WorkbookOpen += EventDel_WorkBookOpen;


            EventDel_SheetChange = new Microsoft.Office.Interop.Excel.AppEvents_SheetChangeEventHandler(WorkSheet_Change);
            xlApp.SheetChange += EventDel_SheetChange;


            EventDel_SheetDeactivate = new Microsoft.Office.Interop.Excel.AppEvents_SheetDeactivateEventHandler(WorkSheet_Deactivate);
            xlApp.SheetDeactivate += EventDel_SheetDeactivate;

            EventDel_BeforeRightClick = new Microsoft.Office.Interop.Excel.AppEvents_SheetBeforeRightClickEventHandler(WorkSheet_BeforeRightClick);
            xlApp.SheetBeforeRightClick += EventDel_BeforeRightClick;

            //11.0.3
            EventDel_SheetActivate = new Excel.AppEvents_SheetActivateEventHandler(WorkSheet_Activate);
            xlApp.SheetActivate += EventDel_SheetActivate;

         
            //Global._ExcelApp.CommandBars["Ply"].Controls["&Rename"].Enabled = true;
            #endregion

        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            if (xlApp != null)
                Marshal.ReleaseComObject(xlApp);
        }

        void WorkSheet_BeforeRightClick(object sheet, Excel.Range nTarget, ref bool cancel)
        {
        }
        void BeforeBookClose(Excel.Workbook Wb, ref bool Cancel)
        {
            try
            {
                if (this.Application.Workbooks.Count > 1)
                {
                    gbClosing = false;
                }
                else
                {
                    gbClosing = true;
                }

                //If worksheet have changes then update the last modify user in hidden sheet
                CBVExcel CBVExcel = new CBVExcel();
                if (IsWorksheetModified(CBVExcel))
                {
                    Global.WorkSheetChange = false;
                    //11.0.3
                    CBVExcel.UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.ModifiedUser, Global.CurrentWorkSheetName, Global.CurrentUser);

                }
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
        }

        void WorkBookDeactive(Excel.Workbook workbook)
        {
            try
            {
                if (gbClosing)
                {
                    int x = this.Application.Workbooks.Count;
                    if (this.Application.Workbooks.Count == 1)
                    {
                        LogOut();
                        //Global.IsLogin = false;
                        this.ChangeCaptionInOut();
                    }
                    if (!Global.IsLogin)
                    {
                        LogOut();
                        //Global.IsLogin = false;
                    }
                }
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
        }

        void WorkBookActive(Excel.Workbook workbook)
        {
            try
            {
                //11.0.3
                Excel::Worksheet nSheet = (Excel::Worksheet)workbook.ActiveSheet;
                Global.ProtectCDXLWorkSheet(nSheet);

                if (Global.IsLogin)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    CBVExcel CBVExcel = new CBVExcel();
                    //Excel::Worksheet nSheet = (Excel::Worksheet)workbook.ActiveSheet;
                    if (!Global.IsCDExcelWorksheet(nSheet))
                        return;
                    Global.ISServerValidated = false;
                    CBVExcel.SetDataViewInGlobalVars(nSheet);
                    //Create the CBV Sheet from CBVEXportShet
                    //CreateCBVSheetFromCBVExportSheet();
                }

            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        void WorkBookOpen(Excel.Workbook workbook)
        {

            // Advance Export - If the opened excel file contains the exported schema details then show the login window
            if (IsCOECBVExcelExportSheet())
            {
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    //Retrieve the CBV Sheet Export data into collection
                    //Hidden Sheet funcitonality for xlst
                    Global.CBVHiddenCriteriaCollection = CBVSheetXMLExport.GetCBVSheetExportData();
                    if (Global.CBVHiddenCriteriaCollection == null || Global.CBVHiddenCriteriaCollection.Count <= 0)
                        return;

                    string username = Global.CBVHiddenCriteriaCollection[(int)Global.CBVExcelExportKeys.Username].ToString().Trim();
                    string password = Global.CBVHiddenCriteriaCollection[(int)Global.CBVExcelExportKeys.Password].ToString().Trim();



                    //This will enable once the CBVEExport functionality will work finally.
                    /*
                    //Global.MRUListConstant.MRU_2T

                    string tier = Global.CBVHiddenCriteriaCollection[(int)Global.CBVExcelExportKeys.tier].ToString().Trim();
                    Global.MRUServerMode = tier;
                    string servername = string.Empty;
                    if (tier.Equals(StringEnum.GetStringValue(Global.MRUListConstant.MRU_2T), StringComparison.OrdinalIgnoreCase))
                    {
                        servername = String.Format(StringEnum.GetStringValue(Global.MRUListConstant.MRU_2T) + " / {0}", Global.CBVHiddenCriteriaCollection[(int)Global.CBVExcelExportKeys.Servername].ToString().Trim());
                    }
                    else
                    {
                        servername = Global.CBVHiddenCriteriaCollection[(int)Global.CBVExcelExportKeys.Servername].ToString().Trim();
                    }

                    Global.MRUServer = servername;
                
                    bool ssl = bool.Parse(Global.CBVHiddenCriteriaCollection[(int)Global.CBVExcelExportKeys.SSL]);

                    //if (!Global.IsLogin)
                    //{
                    //    using (frmLogIn logInForm = new frmLogIn(this))
                    //    {
                    //        logInForm.ShowDialog();
                    //    }
                    //}
                    frmLogIn frmlogin = new frmLogIn();
                    if (frmlogin.Login(username, password, servername, ssl))
                    {
                        //EnableDataViewBOSelect();
                        Global.IsLogin = true;
                        AttachSelectionChangeEvent();
                        //this.parent.RefreshCommandBar();
                        UserInfo objUserInfo = new UserInfo();
                        objUserInfo.UpdateUserInfo(username);

                        CreateCBVSheetFromCBVExportSheet();
                  
                    }
                    */


                    CBVExcel CBVExcel = new CBVExcel();
                    bool _bHidden = GlobalCBVExcel.Is_CBVEHidden();
                    if (_bHidden)
                    {
                        SetDataViewPosition();
                    }
                }
                catch (Exception ex)
                {
                    CBVExcel.ErrorLogging(ex.Message);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        //11.0.3
        void Worksheet_Calculate(object sheet)
        {

        }
      
        void Worksheet_SheetSelectionChange(object sheet, Microsoft.Office.Interop.Excel.Range nTarget)
        {
            Excel.Worksheet nSheet = this.Application.ActiveSheet as Excel.Worksheet;
            //Coverity fix - CID 18744
            if (nSheet == null)
                throw new System.NullReferenceException();
            //12.4
            // Keep the original count columns before editing sheet.      //Fixed CSBR-153644    
            if (nTarget.Columns.Count > 0)
                cntColumns = nSheet.UsedRange.Columns.Count;
            
            // 11.0.3
            //As enabling/Disabling the commandbarbutton control uisng the object doenot work with excel 2013, had to make this change 
            // as it works with all the excel versions. 
            //CBOE-2428 Fix: To make sure the show document is not enabled when the mimetype is wrong 
            CommandBar contextMenu = Globals.ThisAddIn.Application.CommandBars[commandbarName.ToString()];
            foreach (CommandBarControl c in contextMenu.Controls)
            {
                if (c.Caption == "Show Document")
                {
                    var contextMenuItem = (CommandBarButton)c;
                    contextMenuItem.Enabled = false;
                }
            }             

            //To avoid the dropdown 
            if (Global.IsLogin == false || Global.IsCBVExcelWorksheet(nSheet) == false)
            {
                return;
            }
                      
            

            VerifyCurrentSheetReName(nSheet, Global.CurrentWorkSheetName);
            //Global.CurrentWorkSheetName = nSheet.Name;

            // 11.0.3
            CheckMimeType(nSheet, nTarget);

            CBVExcel CBVExcel = new CBVExcel();

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (ISAllHeadersExists(nSheet, nTarget))
                {
                    //Added the range column index in column index global variables
                    CBVExcel.UpdateCellDropdownRange(new int[] { (int)nTarget.Column });
                    //Set the data type of cell in criteria row
                    //Global.SetRangeValue();
                    CBVExcel.SetCellDatatype(nTarget, nSheet);
                }
                else
                {
                    Global.SetRangeValue();
                    if (Global.IsCellRangeNull(Global.GlobalcellRngVal))
                        return;
                }
                CBVExcel.OnSlectionChange(nSheet, nTarget);
                Cursor.Current = Cursors.Default;

                
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

           
        }

      static bool IsWSActivated = false;
        void WorkSheet_Activate(object nativeSheet)
        {
            //11.0.3
            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            Global.ProtectCDXLWorkSheet(nSheet);
            //Coverity fix - CID 18742
            if (nSheet == null)
                throw new System.NullReferenceException();

            if (IsWSActivated)
            {
                IsWSActivated = false;
                return;
            }
            CBVExcel CBVExcel = new CBVExcel();
            //Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;

            /*  if ((!Global.IsCDExcelWorksheet(nSheet)) || (!Global.IsLogin)) 
             */
            if (!Global.IsLogin)
                return;
           

            Global.CurrentWorkSheetName = nSheet.Name;
            Cursor.Current = Cursors.WaitCursor;

            Global.WorkSheetChange = false;

            try
            {
                if (GlobalCBVExcel.Is_CBVEHidden())
                {
                    //11.0.3
                    //CBVExcel.UpdateHiddenSheet(nativeSheet, Global.lastWorkSheetName);
                    CBVExcel.RemoveRowFromHidden(Global.lastWorkSheetName);

                    if (ValidateServer(true))
                    {
                        Global._ExcelApp.EnableEvents = false;
                        CBVExcel.SetDataViewInGlobalVars(nSheet);
                        Global._ExcelApp.EnableEvents = true;
                    }
                    //cntEventOccur++;

                    //Reset the Hithist information
                    COEDataViewData.resultPageInfo = null;
                    //Reset the search criteria
                    if (Global.SearchCriteria != null)
                        Global.SearchCriteria.Clear();
                    IsWSActivated = true;
                }
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        void WorkSheet_Deactivate(object sheet)
        {
            Excel::Worksheet nSheet = (sheet) as Excel::Worksheet;

            //Coverity fix - CID 18743
            if (nSheet == null)
                throw new System.NullReferenceException();
            IsWSActivated = false;
            Global.ISServerValidated = false;
            VerifyCurrentSheetReName(nSheet, Global.CurrentWorkSheetName);

            Global.lastWorkSheetName = nSheet.Name.Trim();
            CBVExcel CBVExcel = new CBVExcel();

            if (IsWorksheetModified(CBVExcel))
            {
                Global.WorkSheetChange = false;
                //11.0.3
                CBVExcel.UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.ModifiedUser, Global.CurrentWorkSheetName, Global.CurrentUser);

            }
        }

       
        void WorkSheet_Change(object sheet, Excel.Range nTarget)
        {
            Global.WorkSheetChange = true;
            Global.InsertColumn = true; //Mark as true when any changes occur in cell, specially for dropdown cell values.

            //User counter to control the recursive call of the dropdown selection event(for field and option header).
            Excel::Worksheet nSheet = (sheet) as Excel::Worksheet;

            //if current sheet is not chembioviz sheet
            if ((!Global.IsCDExcelWorksheet(nSheet)) || (!Global.IsLogin))
                return;

            if (!Global.ISServerValidated)
            {
                ValidateServer(true);
                if (!Global.ISServerValidated)
                {
                Global._ExcelApp.EnableEvents = false;
                Global._ExcelApp.Undo();
                Global._ExcelApp.EnableEvents = true;
                return;
            }
            }

            if (InsertDeleteRowInCBVHeaderRow(nSheet, nTarget))
                return;

            if (cntEventOccur > 0 || (!Global.IsLogin)) // check the counter and login information. 
            {
                cntEventOccur = 0;
                return;
            }

            if (DeleteCriteriaRange(nTarget))
                return;

            CBVExcel CBVExcel = new CBVExcel();

            //if sheet name match with hidden sheet name then return
            if (nSheet.Name.Equals(Global.COEDATAVIEW_HIDDENSHEET, StringComparison.OrdinalIgnoreCase))
                return;

            // 11.0.4 - in case column is deleted
            if (cntColumns > nSheet.UsedRange.Columns.Count)
            {
                cntColumns = nSheet.UsedRange.Columns.Count;
                return;
            }

            //Set the display option cell as nTarget range 
            Excel.Range optionHeader = (Excel.Range)nSheet.Cells[(int)Global.CBVHeaderRow.HeaderOptionRow, (int)nTarget.Column];


            try
            {
                if ((int)nTarget.Row == (int)Global.CBVHeaderRow.HeaderColumnRow)
                {
                    ++cntEventOccur;
                    CBVExcel.AddInOptionHeaderCellDropdown(sheet, optionHeader, (int)nTarget.Row, (int)nTarget.Column);
                    //Get column value in header label row            
                    CBVExcel.GetCBVColumnLabelHeader((int)nTarget.Row, (int)nTarget.Column, sheet);

                    //Resize the coumns width on columns name                    
                    Global._ExcelApp.ScreenUpdating = false;

                    if (Global.ToggleColumnAutoSize == 1)
                        GlobalCBVExcel.AutoFitColumn(nTarget, Global._CBVResult);

                    Global._ExcelApp.ScreenUpdating = true;
                }

                else if ((int)nTarget.Row == (int)Global.CBVHeaderRow.HeaderOptionRow)
                {
                    Excel.Range val = (Excel.Range)nSheet.Cells[(int)Global.CBVHeaderRow.HeaderOptionRow, (int)nTarget.Column];

                    if (val.Text.ToString() != string.Empty && Global.MaxRecordInResultSet > 0 && (!string.IsNullOrEmpty(CBVExcel.GetDataFromHiddenSheet(Global.UID, (int)Global.CBVHiddenSheetHeader.MaxResultCount))))
                    {
                        //only executes when the click of header options
                        if (headerOptionHitRow == (int)Global.CBVHeaderRow.HeaderOptionRow)
                        {
                            if (!CBVExcel.CBVSplitExists(nSheet, (int)nTarget.Column))
                            {
                                // 11.0.4 - If CBVSpilt not exists and IsUpdatingResultRequired = true then update result from framework else update result from client side
                                if (Global.IsUpdatingResultRequired)
                                {
                                    if (MessageBox.Show(Properties.Resources.msgSplitSearch, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                                    {
                                        btnUpdateCurrentResults.Execute();
                                    }
                                }
                                else
                                {

                                    ++cntEventOccur;
                                    Global._ExcelApp.ScreenUpdating = false;
                                    CBVExcel.OptionHeaderOperation(sheet, nTarget, optionHeader.Text.ToString());

                                    //Resize the coumns width on columns name
                                    if (Global.ToggleColumnAutoSize == 1)
                                        GlobalCBVExcel.AutoFitColumn(nTarget, Global._CBVResult);

                                    Global._ExcelApp.ScreenUpdating = true;
                                }

                                // 11.0.4 - Set to false, if CBVSplit not exists
                                Global.IsUpdatingResultRequired = false;
                            }
                            //11.0.3
                            else if (CBVExcel.CBVStructureExists(nSheet, (int)nTarget.Column))
                            {
                                if (MessageBox.Show(Properties.Resources.msgStructureSearch, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                                {
                                    optionStructureSearch = true;
                                    btnReplaceAllResults.Execute();
                                    // 11.0.4 - commented as to execute the "Global.IsUpdatingResultRequired = true/false" code
                                    //return;
                                }
                            }
                            else
                            {
                                if (MessageBox.Show(Properties.Resources.msgSplitSearch, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                                {
                                    btnUpdateCurrentResults.Execute();
                                    // 11.0.4 - commented as to execute the "Global.IsUpdatingResultRequired = true/false" code
                                    //return;
                                }
                            }


                        }
                        else
                        {
                            Global._ExcelApp.ScreenUpdating = false;

                            if (Global.ToggleColumnAutoSize == 1)
                                GlobalCBVExcel.AutoFitColumn(nTarget, Global._CBVResult);

                            Global._ExcelApp.ScreenUpdating = true;
                        }
                    }
                    
                    
                    // 11.0.4 - Set to true, if optionHeader contains either "Split" or "Mulitple"
                    if (val.Text.ToString().ToUpper().Contains("SPLIT") || val.Text.ToString().ToUpper().Contains("MULTIPLE"))
                    {
                        Global.IsUpdatingResultRequired = true;
                    }
                    else
                    {
                        Global.IsUpdatingResultRequired = false;
                    }
                    
                    

                }
                //The below conditon is used to fill the table header row on the basis of selected category header
                else if ((int)nTarget.Row == (int)Global.CBVHeaderRow.HeaderCategoryRow)
                {
                    ++cntEventOccur;
                    CBVExcel.GetCBVTableColumnHeader((int)nTarget.Row, (int)nTarget.Column, sheet);
                    //Get column value in header label row            
                    CBVExcel.GetCBVColumnLabelHeader((int)nTarget.Row, (int)nTarget.Column, sheet);

                    //Resize the coumns width on columns name
                    Global._ExcelApp.ScreenUpdating = false;

                    if (Global.ToggleColumnAutoSize == 1)
                        GlobalCBVExcel.AutoFitColumn(nTarget, Global._CBVResult);

                    Global._ExcelApp.ScreenUpdating = true;
                }
                //The below conditon is used to fill the column header row on the basis of selected table header
                else if ((int)nTarget.Row == (int)Global.CBVHeaderRow.HeaderTableRow)
                {
                    ++cntEventOccur;
                    Global._ExcelApp.ScreenUpdating = false;
                    CBVExcel.GetCBVColumnHeader((int)nTarget.Row, (int)nTarget.Column, sheet);
                    CBVExcel.AddInOptionHeaderCellDropdown(sheet, optionHeader, (int)nTarget.Row, (int)nTarget.Column);
                    //Get column value in header label row            
                    CBVExcel.GetCBVColumnLabelHeader((int)nTarget.Row, (int)nTarget.Column, sheet);

                    //Resize the coumns width on columns name
                    if (Global.ToggleColumnAutoSize == 1)
                        GlobalCBVExcel.AutoFitColumn(nTarget, Global._CBVResult);
                    Global._ExcelApp.ScreenUpdating = true;
                }
                //Reset
                cntEventOccur = 0;
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }

        }
        #endregion

        #region "CommandBar Actions"
        void NewChemBioVizWorksheet_Click(Microsoft.Office.Core.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            LoginDVPopupResult(true);

            ////11.0.3
            //Excel.Worksheet nSheet = Global._ExcelApp.ActiveSheet as Excel.Worksheet;
            //Global.ProtectCDXLWorkSheet(nSheet);
        }

        void LogIn_Click(Microsoft.Office.Core.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            try
            {
                if (!Global.IsLogin)
                {
                    this.LogIn();
                }
                else
                {
                    if (MessageBox.Show(Properties.Resources.msgLogOut, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        this.LogOut();
                    }
                }

                //11.0.3
                Excel.Worksheet nSheet = Global._ExcelApp.ActiveSheet as Excel.Worksheet;
                if(nSheet !=null)
                Global.ProtectCDXLWorkSheet(nSheet);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void LogOut()
        {
            try
            {
                Global.RestoreCSLAPrincipal();
                AccessController.Instance.LogOut();

                Global.IsLogin = false;
                this.ChangeCaptionInOut();
                SetErrorLoggingCaption(false);
            }
            finally
            {
                Global.RestoreWindowsPrincipal();
            }
        }

        private bool LogIn()
        {
            bool result = true;

            if (!AccessController.Instance.IsLogged)
            {
                if (AppDomain.CurrentDomain.GetData("ORIGINAL_PRINCIPAL") == null)
                    AppDomain.CurrentDomain.SetData("ORIGINAL_PRINCIPAL", Csla.ApplicationContext.User);

                AccessController.Instance.LoginDialogCaption = Properties.Resources.msgCaption;

                if ((result = AccessController.Instance.Login()))
                {
                    Global.IsLogin = true;
                    this.ChangeCaptionInOut();
                    AttachSelectionChangeEvent();
                }
            }

            return result;
        }

        // Advance Export - Opening the login window, autofilling the user and server details
        private bool LoginAdvanceExport(string userName, string serverName, string tier, bool ssl)
        {
            bool result = true;

            if (!AccessController.Instance.IsLogged)
            {
                if (AppDomain.CurrentDomain.GetData("ORIGINAL_PRINCIPAL") == null)
                    AppDomain.CurrentDomain.SetData("ORIGINAL_PRINCIPAL", Csla.ApplicationContext.User);

                AccessController.Instance.LoginDialogCaption = Properties.Resources.msgCaption;              

                if ((result = AccessController.Instance.LoginAdvanceExport(userName, serverName, tier, ssl)))                
                {
                    Global.IsLogin = true;
                    this.ChangeCaptionInOut();
                    AttachSelectionChangeEvent();
                }
            }

            return result;
        }

        void accessControler_AfterLoginAttempt(object sender, LoginAttemptEventArgs e)
        {
            Global.CurrentUser = AccessController.Instance.UserName;
            Global.SetPrincipal();
            Global.RestoreWindowsPrincipal();

            if (!e.Succeeded && !string.IsNullOrEmpty(e.ErrorMessage))
                CBVExcel.ErrorLogging(e.ErrorMessage);

            Global.WorkSheetChange = false;

            CBVExcel CBVExcelInstance = new CBVExcel();
            bool _bHidden = GlobalCBVExcel.Is_CBVEHidden();
            if (_bHidden)
            {
                //this.parent.SetDataViewPosition();
                Excel::Worksheet _nWorkSheet = (Excel.Worksheet)Global._ExcelApp.ActiveWorkbook.ActiveSheet;
                try
                {
                    CBVExcelInstance.SetDataViewInGlobalVars(_nWorkSheet);
                }
                catch (System.Exception ex) 
                {
                    CBVExcel.ErrorLogging(ex.Message);
                }
            }
        }

        void SelectDV_Click(Microsoft.Office.Core.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            dataviewListForm = new frmDataviewList(this);
            dataviewListForm.ShowDialog();
        }

        #region "CBV Setup Command Actions"

        void btnInsertCBVCategoryList_Click(Office::CommandBarButton Ctrl, ref bool CancelDefault)
        {
            if (!ValidateServer(true))
                return;

            if (!LoginDVPopupResult(false))
                return;

            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            //Coverity fix - CID 18746
            if (nSheet == null)
                throw new System.NullReferenceException();
            Excel::Range nTarget = nSheet.Cells.Application.Selection as Excel::Range;
            //Coverity fix - CID 18746
            if (nTarget == null)
                throw new System.NullReferenceException();
            try
            {

                if (Global.IsCBVExcelWorksheet(nSheet))
                {
                    try
                    {
                        ChemBioVizExcelAddIn.CBVExcel CBVExcel = new ChemBioVizExcelAddIn.CBVExcel();
                        CBVExcel.SetFocusRange(nSheet, nTarget);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Properties.Resources.msgError + "\n" + ex.Message, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgslectCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
        }

        // 11.0.4
        void btnDeleteCBVSheetColumn_Click(Office::CommandBarButton Ctrl, ref bool CancelDefault)
        {
            if (!ValidateServer(true))
                return;

            if (!LoginDVPopupResult(false))
                return;

            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            //Coverity fix - CID 18745
            if (nSheet == null)
                throw new System.NullReferenceException();
            Excel::Range nTarget = nSheet.Cells.Application.Selection as Excel::Range;

            //Coverity fix - CID 18745
            if (nTarget == null)
                throw new System.NullReferenceException();
            try
            {


                if (Global.IsCBVExcelWorksheet(nSheet))
                {
                    try
                    {
                        //verify the edit mode of CBV Sheet
                        GlobalCBVExcel.ISSheetInEditMode();

                        if (nTarget.Columns.Count == 0)
                        {
                            MessageBox.Show(Properties.Resources.msgSelectColumn, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else if (nTarget.Columns.Count > 1)
                        {
                            MessageBox.Show(Properties.Resources.msgSelectOneColumn, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            if (nTarget.Column > nSheet.UsedRange.Columns.Count)
                            {
                                MessageBox.Show(Properties.Resources.msgValidDataColumn, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            DialogResult objResult = MessageBox.Show(Properties.Resources.msgDeleteColumn, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (objResult == DialogResult.Yes)
                            {
                                cntColumns = nSheet.UsedRange.Columns.Count;
                                nTarget.EntireColumn.Delete(Type.Missing);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Properties.Resources.msgError + "\n" + ex.Message, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgslectCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
        }


        void btnToggleColumnAutosizing_Click(Office::CommandBarButton Ctrl, ref bool CancelDefault)
        {
            if (!ValidateServer(true))
                return;
            if (!LoginDVPopupResult(false))
                return;
            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            try
            {
                if (Global.IsCBVExcelWorksheet(nSheet))
                {
                    //verify the edit mode of CBV Sheet
                    GlobalCBVExcel.ISSheetInEditMode();

                    ChemBioVizExcelAddIn.CBVExcel CBVExcel = new ChemBioVizExcelAddIn.CBVExcel();

                    if (Global.ToggleColumnAutoSize == 0)
                    {
                        btnToggleColumnAutosizing.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
                        Global.ToggleColumnAutoSize = 1;
                        CBVExcel.EnableToggleColumnAutosizing();
                    }
                    else
                    {
                        btnToggleColumnAutosizing.Style = Office.MsoButtonStyle.msoButtonCaption;
                        Global.ToggleColumnAutoSize = 0;
                        CBVExcel.DisableToggleColumnAutosizing();
                    }

                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgslectCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
                MessageBox.Show(ex.Message, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        void btnToggleHeaderDisplay_Click(Office::CommandBarButton Ctrl, ref bool CancelDefault)
        {
            if (!ValidateServer(true))
                return;
            if (!LoginDVPopupResult(false))
                return;

            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            try
            {
                if (Global.IsCBVExcelWorksheet(nSheet))
                {
                    //verify the edit mode of CBV Sheet
                    GlobalCBVExcel.ISSheetInEditMode();
                    ChemBioVizExcelAddIn.CBVExcel CBVExcel = new ChemBioVizExcelAddIn.CBVExcel();

                    CBVExcel.ToggleHeaderDisplay(nSheet);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgslectCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
                MessageBox.Show(ex.Message, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        void btnCBVOptions_Click(Office::CommandBarButton Ctrl, ref bool CancelDefault)
        {
            if (!LoginDVPopupResult(false))
                return;

            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            try
            {
                if (Global.IsCBVExcelWorksheet(nSheet))
                {
                    try
                    {
                        using (frmMaxHit maxHit = new frmMaxHit())
                        {
                            maxHit.ShowDialog();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Properties.Resources.msgError + "\n" + ex.Message, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                else
                {
                    MessageBox.Show(Properties.Resources.msgslectCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
        }

        #endregion

        #region "CBV Options Command Actions"
        void btnVerifyCurrentCBVSheet_Click(Office::CommandBarButton Ctrl, ref bool CancelDefault)
        {
            if (!ValidateServer(true))
                return;
            if (!LoginDVPopupResult(false))
                return;
            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                ChemBioVizExcelAddIn.CBVExcel CBVExcel = new ChemBioVizExcelAddIn.CBVExcel();
                if (Global.IsCBVExcelWorksheet(nSheet))
                {

                    int errorCol = CBVExcel.VerifyCurrentCBVSheet(nSheet);
                    if (errorCol == 0)
                    {
                        MessageBox.Show(Properties.Resources.msgValidCurrentCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(String.Format(Properties.Resources.msgInValidCurrentCBVSheet, errorCol), Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgslectCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        void btnCreateListfromSelection_Click(Office::CommandBarButton Ctrl, ref bool CancelDefault)
        {
            if (!ValidateServer(true))
                return;

            if (!LoginDVPopupResult(false))
                return;
            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                if (Global.IsCBVExcelWorksheet(nSheet))
                {
                    ChemBioVizExcelAddIn.CBVExcel CBVExcel = new ChemBioVizExcelAddIn.CBVExcel();
                    CBVExcel.CreateListfromSelection(xlApp);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgslectCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        void btnClearCurrentResults_Click(Office::CommandBarButton Ctrl, ref bool CancelDefault)
        {
            if (!ValidateServer(true))
                return;
            if (!LoginDVPopupResult(false))
                return;
            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (Global.IsCBVExcelWorksheet(nSheet))
                {
                    //verify the edit mode of CBV Sheet
                    GlobalCBVExcel.ISSheetInEditMode();

                    ChemBioVizExcelAddIn.CBVExcel CBVExcel = new ChemBioVizExcelAddIn.CBVExcel();

                    Global._ExcelApp.ScreenUpdating = false;
                    CBVExcel.ClearResults(nSheet);
                    Global._ExcelApp.ScreenUpdating = true;

                    //Set the max record count is zero and update the hidden sheet
                    Global.MaxRecordInResultSet = 0;
                    //11.0.3
                    CBVExcel.UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.MaxResultCount, Global.CurrentWorkSheetName, Global.MaxRecordInResultSet.ToString());
                    //Set empty data into seralize data cell
                    //11.0.3
                    CBVExcel.UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.SerializeCBVResult, Global.CurrentWorkSheetName, string.Empty);

                    CBVExcel.CleanCBVResultTable();
                    Global.SearchUpdateCriteria.Clear(); // Clear the search update criteria
                    //11.0.3
                    CBVExcel.UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.SearchUpdateCriteria, Global.CurrentWorkSheetName, string.Empty);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgslectCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
                MessageBox.Show(ex.Message, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        void btnClearSearchCriteria_Click(Office::CommandBarButton Ctrl, ref bool CancelDefault)
        {
            if (!ValidateServer(true))
                return;

            if (!LoginDVPopupResult(false))
                return;
            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (Global.IsCBVExcelWorksheet(nSheet))
                {
                    //verify the edit mode of CBV Sheet
                    GlobalCBVExcel.ISSheetInEditMode();

                    ChemBioVizExcelAddIn.CBVExcel CBVExcel = new ChemBioVizExcelAddIn.CBVExcel();
                    CBVExcel.ClearSearchCriteria(nSheet);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgslectCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
                MessageBox.Show(ex.Message, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        void btnToggleErrorLogging_Click(Office::CommandBarButton Ctrl, ref bool CancelDefault)
        {
            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            try
            {
                if (Global.IsCBVExcelWorksheet(nSheet) && Global.IsLogin == true)
                {
                    if (!Global.ISErrorLogging)
                    {
                        ChemBioVizExcelAddIn.CBVExcel CBVExcel = new ChemBioVizExcelAddIn.CBVExcel();
                        Log.CreateLogFile(AppConfigSetting.ReadSetting(StringEnum.GetStringValue(Global.ConfigurationKey.SQL_LOGGING_FILE)));

                        MessageBox.Show(Properties.Resources.msgLogStarted + AppConfigSetting.ReadSetting(StringEnum.GetStringValue(Global.ConfigurationKey.SQL_LOGGING_FILE)), Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.None);
                        SetErrorLoggingCaption(true);
                    }
                    else if (Global.ISErrorLogging)
                    {
                        SetErrorLoggingCaption(false);
                    }
                }
                else if (LoginDVPopupResult(Global.IsCBVExcelWorksheet(nSheet)))
                {
                    Log.CreateLogFile(AppConfigSetting.ReadSetting(StringEnum.GetStringValue(Global.ConfigurationKey.SQL_LOGGING_FILE)));
                    MessageBox.Show(Properties.Resources.msgLogStarted + AppConfigSetting.ReadSetting(StringEnum.GetStringValue(Global.ConfigurationKey.SQL_LOGGING_FILE)), Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.None);
                    SetErrorLoggingCaption(true);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgslectCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
        }

        void btnSearchPrefs_Click(Office::CommandBarButton Ctrl, ref bool CancelDefault)
        {

            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            try
            {
                if (Global.IsCBVExcelWorksheet(nSheet))
                {
                    frmSearchOption searchOption = new frmSearchOption();
                    searchOption.ShowDialog();
                }
                else if (!LoginDVPopupResult(false))
                {
                    return;
                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgslectCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
        }
        //11.0.3
        void btnShowDocument_Click(Office::CommandBarButton Ctrl, ref bool CancelDefault)
        {
            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            Excel::Range nTarget = this.Application.Selection as Excel::Range;
            
            // 11.0.3 - Commented as already checked in SheetSelectionChange event
            //if (rowPos >= 0 && colPos >= 0 && (Global._CBVSearchResult != null) && Global._CBVSearchResult.Rows.Count > 0 && nTarget.Column <= Global.CBVNewColumnIndex)
            //{
                try
                {
                    // 11.0.3 - Commented as already checked in SheetSelectionChange event
                    //if (!IsValidMimeType(nSheet, nTarget))
                    //    return;

                    if (!ValidateServer(true))
                        return;
                    if (!LoginDVPopupResult(false))
                        return;

                    Cursor.Current = Cursors.WaitCursor;

                    if (Global.IsCBVExcelWorksheet(nSheet))
                    {
                        //verify the edit mode of CBV Sheet
                        GlobalCBVExcel.ISSheetInEditMode();

                        ChemBioVizExcelAddIn.CBVExcel CBVExcel = new ChemBioVizExcelAddIn.CBVExcel();
                        //Check whether unique key or primary key exists in cbv sheet
                        if (!CBVExcel.ISPrimaryKeyORUniqueKeyExists(nSheet))
                        {
                            MessageBox.Show(Properties.Resources.msgCBVSheetPKFK, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        else
                        {
                            CBVExcel.GetMimeTypeResult(nTarget, nSheet, Global.CBVSearch.ReplaceMimeTypeResult);
                        }
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.msgslectCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                catch (Exception Ex)
                {
                    CBVExcel.ErrorLogging(Ex.Message);
                    MessageBox.Show(Ex.Message, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Csla.ApplicationContext.User = null;
                    Cursor.Current = Cursors.Default;
                    Global._ExcelApp.EnableEvents = true;
                }
            //}
        }

        // 11.0.4 - To select the data of the selected column
        void btnSelectData_Click(Office::CommandBarButton Ctrl, ref bool CancelDefault)
        {
            if (!ValidateServer(true))
                return;

            if (!LoginDVPopupResult(false))
                return;
            //Coverity fix - CID 18747
            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            if (nSheet == null)
                throw new System.NullReferenceException();

            Excel::Range nTarget = nSheet.Cells.Application.Selection as Excel::Range;
            
            if (nTarget == null)
                throw new System.NullReferenceException();
            try
            {
                if (Global.IsCBVExcelWorksheet(nSheet))
                {
                    try
                    {
                        //verify the edit mode of CBV Sheet
                        GlobalCBVExcel.ISSheetInEditMode();

                        if (nTarget.Columns.Count == 0)
                        {
                            MessageBox.Show(Properties.Resources.msgSelectColumnToSelectData, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            if (nTarget.Column > nSheet.UsedRange.Columns.Count)
                            {
                                MessageBox.Show(Properties.Resources.msgValidDataColumn, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                            if (Global._CBVSearchResult != null)
                            {
                                string excelRange = string.Format("{0}{1}{2}", Global.NumToString(nTarget.Column) + Global.CBVSHEET_RESULT_STRAT_ROW, ":", Global.NumToString(nTarget.Column + nTarget.Columns.Count - 1) + ((Global.CBVSHEET_RESULT_STRAT_ROW - 1) + Global._CBVSearchResult.Rows.Count));
                                nSheet.get_Range(excelRange, Type.Missing).Select();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Properties.Resources.msgError + "\n" + ex.Message, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgslectCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
        }

        void btnSearchPrefsSub_Click(Office::CommandBarButton Ctrl, ref bool CancelDefault)
        {
            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            try
            {
                if (Global.IsCBVExcelWorksheet(nSheet))
                {
                    frmSearchOption searchOption = new frmSearchOption();
                    searchOption.ShowDialog();
                }
                //
                else if (!LoginDVPopupResult(false))
                {
                    return;
                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgslectCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
        }

        void btnSheetProperties_Click(Office::CommandBarButton Ctrl, ref bool CancelDefault)
        {
           // Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            try
            {
                Excel::Worksheet nActiveSheet = this.Application.ActiveSheet as Excel::Worksheet;
                //Coverity fix - CID 18748
                if (nActiveSheet == null)
                    throw new System.NullReferenceException();
                // Getting a dummy range
                Excel.Range nTarget = nActiveSheet.get_Range("A1", "A1");
                //Coverity fix - CID 18748
                if (nTarget == null)
                    throw new System.NullReferenceException();
                Worksheet_SheetSelectionChange(nActiveSheet, nTarget);
                if (!LoginDVPopupResult(false))
                {
                    return;
                }
                else
                {
                    Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
                    if (nSheet == null)
                        throw new System.NullReferenceException();
                    if (!GlobalCBVExcel.IsSheetExistsInHidden(nSheet.Name))
                    {
                        
                        if (Global.CBVSHEET_COEDATAVIEWBO == null)
                        {
                            //Instead of message the dataview popup will display
                            if (MessageBox.Show(Properties.Resources.msgNewChemBioVizSheet, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            {
                                using (frmDataviewList frmDataview = new frmDataviewList(this))
                                {
                                    if (frmDataview.ShowDialog() == DialogResult.OK)
                                    {
                                        frmSheetProperties frmSheetProp = new frmSheetProperties();
                                        frmSheetProp.ShowDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            //if (MessageBox.Show(Properties.Resources.msgSelectCBVSheetProperties, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            //{
                            //   ValidateSheet(nSheet);
                            //}
                            MessageBox.Show(Properties.Resources.msgSelectDVSheetProperties, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                }
                else if (Global.IsCBVExcelWorksheet(nSheet) && Global.IsLogin)
                {
                    frmSheetProperties frmSheetProp = new frmSheetProperties();
                    frmSheetProp.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
        }

        #endregion

        #region "CBV Search Command Actions"

        void btnUpdateCurrentResults_Click(Microsoft.Office.Core.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            if (!ValidateServer(true))
                return;

            if (!LoginDVPopupResult(false))
                return;

            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                if (Global.IsCBVExcelWorksheet(nSheet))
                {
                    //verify the edit mode of CBV Sheet
                    GlobalCBVExcel.ISSheetInEditMode();

                    ChemBioVizExcelAddIn.CBVExcel CBVExcel = new ChemBioVizExcelAddIn.CBVExcel();

                    CBVExcel.UpdateCurrentResultsMP(nSheet, Global.CBVSearch.UpdateCurrentResults);

                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgslectCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
                MessageBox.Show(ex.Message, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                Global._ExcelApp.EnableEvents = true;
            }
        }

        void btnAppendNewResults_Click(Microsoft.Office.Core.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            if (!ValidateServer(true))
                return;
            if (!LoginDVPopupResult(false))
                return;
            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                if (Global.IsCBVExcelWorksheet(nSheet))
                {
                    //verify the edit mode of CBV Sheet
                    GlobalCBVExcel.ISSheetInEditMode();

                    ChemBioVizExcelAddIn.CBVExcel CBVExcel = new ChemBioVizExcelAddIn.CBVExcel();
                    if (Global.MaxRecordInResultSet <= 0)
                    {
                        if (MessageBox.Show(Properties.Resources.msgAddNewSearch, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            CBVExcel.ReplaceAllResultsMP(nSheet, Global.CBVSearch.ReplaceAllResults);
                            return;
                        }
                    }
                    else
                    {
                        CBVExcel.AppendCurrentResultsMP(nSheet, Global.CBVSearch.AppendNewResults);
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgslectCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
                MessageBox.Show(ex.Message, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                Global._ExcelApp.EnableEvents = true;
            }
        }
        void btnReplaceAllResults_Click(Microsoft.Office.Core.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            if (!ValidateServer(true))
                return;
            if (!LoginDVPopupResult(false))
                return;
            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                if (Global.IsCBVExcelWorksheet(nSheet))
                {
                    //verify the edit mode of CBV Sheet
                    GlobalCBVExcel.ISSheetInEditMode();

                    ChemBioVizExcelAddIn.CBVExcel CBVExcel = new ChemBioVizExcelAddIn.CBVExcel();
                    //Check whether unique key or primary key exists in cbv sheet
                    if (!CBVExcel.ISPrimaryKeyORUniqueKeyExists(nSheet))
                    {
                        MessageBox.Show(Properties.Resources.msgCBVSheetPKFK, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (optionStructureSearch == false)
                    {
                        if (MessageBox.Show(Properties.Resources.msgSearching, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            CBVExcel.ReplaceAllResultsMP(nSheet, Global.CBVSearch.ReplaceAllResults);
                        }
                    }

                    else if (optionStructureSearch == true)
                    {
                        CBVExcel.ReplaceAllResultsMP(nSheet, Global.CBVSearch.ReplaceAllResults);     
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgslectCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            catch (Exception Ex)
            {
                CBVExcel.ErrorLogging(Ex.Message);
                MessageBox.Show(Ex.Message, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                optionStructureSearch = false; // once the replace call, the optionStructureSearch should be false;
                Csla.ApplicationContext.User = null;
                Cursor.Current = Cursors.Default;
                Global._ExcelApp.EnableEvents = true;
            }
        }

        // Advance Export - Checking whether the opened excel file contains the exported data schema details to be displayed in CBV Excel Sheet       
        private bool IsCOECBVExcelExportSheet()
        {           
            try
            {
                Excel::Worksheet nExportSheet = (Excel::Worksheet)this.Application.ActiveSheet;
              
                // Getting the values of Q column's 1st and 2nd rows
                string key = Convert.ToString(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.CHEMBIOVIZEXCELADDIN) + "1", StringEnum.GetStringValue(Global.ExportSheetColumns.CHEMBIOVIZEXCELADDIN) + "1").Value2);
                string value = Convert.ToString(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.CHEMBIOVIZEXCELADDIN) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.CHEMBIOVIZEXCELADDIN) + "2").Value2);

                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)) //If the key and value have null or empty  //Fixed  CSBR-154626 (Object Reference issue)
                {
                    return false;
                }
                // If they contains ChemBioVizExcelAddIn and COECBVExcel values then it means it contains exported schema details
                if (key.Equals(COEFormInterchange.key, StringComparison.OrdinalIgnoreCase) && value.Equals(COEFormInterchange.value, StringComparison.OrdinalIgnoreCase))
                {
                    string userName = string.Empty;
                    string serverName = string.Empty;
                    string tier = string.Empty;
                    string resultFields = string.Empty;
                    bool ssl = false;
                    int dataViewID = -1;

                    try
                    {
                        // Getting the User and Server Detials from the sheet
                        userName = Convert.ToString(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.USERNAME) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.USERNAME) + "2").Value2);
                        serverName = Convert.ToString(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.SERVERNAME) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.SERVERNAME) + "2").Value2);
                        tier = Convert.ToString(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.TIER) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.TIER) + "2").Value2);
                        ssl = Convert.ToBoolean(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.SSL) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.SSL) + "2").Value2);
                        dataViewID = Convert.ToInt32(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.DATAVIEWID) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.DATAVIEWID) + "2").Value2);

                        resultFields = Convert.ToString(nExportSheet.get_Range(StringEnum.GetStringValue(Global.ExportSheetColumns.RESULTFIELDS) + "2", StringEnum.GetStringValue(Global.ExportSheetColumns.RESULTFIELDS) + "2").Value2);
                    }
                    catch
                    {
                        throw new Exception(Properties.Resources.msgExportSheetError);
                    }

                    // Calling the login function, passing the user and server details to be shown in login window
                    if (LoginAdvanceExport(userName, serverName, tier, ssl))
                    {

                        Global.RestoreCSLAPrincipal();
                        // Setting the DataView and Tables from exported sheet to global class
                        Global.CBVSHEET_COEDATAVIEWBO = COEDataViewBO.Get(dataViewID);
                        //Global.COEDATAVIEW_INDEX = cmbCOEDataViewBOList.SelectedIndex.ToString();                       
                        Global.DVTables = TableListBO.NewTableListBO(Global.CBVSHEET_COEDATAVIEWBO.COEDataView.Tables);

                        Global.RestoreWindowsPrincipal();

                        UserInfo userInfo = new UserInfo();

                        userInfo.UpdateUserInfo(Global.CurrentUser);

                        //Create a new chemoffice sheet and convert to sar sheet
                        Global.NewChemOfficeSheet();

                        ChemBioVizExcelAddIn.CBVExcel CBVExcel = new ChemBioVizExcelAddIn.CBVExcel();
                        Excel::Worksheet nNewSheet = Global._ExcelApp.ActiveSheet as Excel.Worksheet;
                        //Coverity fix - CID 18741
                        if (nNewSheet == null)
                            throw new System.NullReferenceException();
                        //Set the sheet name
                        Global.CurrentWorkSheetName = nNewSheet.Name;

                        //Clear the cell drop down list. It clear before the create CBV sheet because the defualt column nos are inserted during CBV sheet creation.
                        Global.CellDropdownRange.Clear();

                        //Disable the worksheet event and create the CBV sheet
                        Global._ExcelApp.ScreenUpdating = false;
                        DeAttachWorkSheetChangeEvent();
                        //Create the CBV Sheet

                        //Fixed CSBR - 152733
                        string[] resultFieldsColumnCount = resultFields.Split(COEFormInterchange.columnSeparator, StringSplitOptions.None);

                        if ((resultFieldsColumnCount.Length > nNewSheet.Columns.Count) && (nNewSheet.Columns.Count == Global.MaxOffice2003Columns))
                        {
                            MessageBox.Show(Properties.Resources.msgOffice2003MaxColumnReachedError, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return false;
                        }
                        if ((resultFieldsColumnCount.Length > nNewSheet.Columns.Count) && (nNewSheet.Columns.Count == Global.MaxOffice2007Columns))
                        {
                            MessageBox.Show(Properties.Resources.msgOffice2007MaxColumnReachedError, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return false;
                        }

                        CBVExcel.CreateCBVSheetAdvanceExport(nNewSheet, resultFields);
                        AttachWorkSheetChangeEvent();
                        Global._ExcelApp.ScreenUpdating = true;

                        //Initalize the searchUpdateCriteria
                        Global.SearchUpdateCriteria = new List<object>();

                        // Setting the Input Criterias in the Search Criteria cells
                        CBVExcel.SetInputCriteria(nNewSheet, nExportSheet);

                        if (MessageBox.Show(Properties.Resources.msgDoSearch, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            if (CBVExcel.ISPrimaryKeyORUniqueKeyExists(nNewSheet))
                            {
                                // Executing the search functionality based upon the search and result criterias retrieved from the export sheet
                                CBVExcel.ReplaceAllResultsMP(nNewSheet, Global.CBVSearch.ReplaceAllResults, nExportSheet);

                            }
                            else
                            {
                                MessageBox.Show(Properties.Resources.msgCBVSheetPKFK, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                        }

                        // Removing the Export sheet after searching.
                        Application.DisplayAlerts = false;
                        nExportSheet.Delete();
                        Application.DisplayAlerts = true;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
                MessageBox.Show(ex.Message, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                Global.RestoreWindowsPrincipal();
            }            
        }

        #endregion "CBV Search Command Actions"

        #region "CBV Help Command Actions"
        // Help buttons

        void btnContents_Click(Microsoft.Office.Core.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            if (!Global.IsLogin)
                return;

            try
            {
                Excel.Worksheet nSheet = this.Application.ActiveSheet as Excel.Worksheet;
                CBVExcel CBVExcel = new CBVExcel();
                string currServer = Global.MRUServer;

                if (!string.IsNullOrEmpty(currServer))
                    OpenHelp(currServer);
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
        }

        void btnAbout_Click(Microsoft.Office.Core.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            frmAbout objAbout = new frmAbout();
            objAbout.ShowDialog();
        }
        #endregion "CBV Help Command Actions"

        #region "CBV Export Command Actions"
        void btnExportResults_Click(Microsoft.Office.Core.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            if (!ValidateServer(true))
                return;

            if (!LoginDVPopupResult(false))
                return;
            Excel::Worksheet nSheet = this.Application.ActiveSheet as Excel::Worksheet;
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                if (Global.IsCBVExcelWorksheet(nSheet))
                {
                    //verify the edit mode of CBV Sheet
                    GlobalCBVExcel.ISSheetInEditMode();

                    ChemBioVizExcelAddIn.CBVExcel CBVExcel = new ChemBioVizExcelAddIn.CBVExcel();
                    //Check whether unique key or primary key exists in cbv sheet
                    if (!CBVExcel.ISPrimaryKeyORUniqueKeyExists(nSheet))
                    {
                        MessageBox.Show(Properties.Resources.msgCBVSheetPKFK, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (MessageBox.Show(Properties.Resources.msgExporting, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        CBVExcel.ExportAllResults(nSheet);
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.msgslectCBVSheet, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            catch (Exception Ex)
            {
                CBVExcel.ErrorLogging(Ex.Message);
                MessageBox.Show(Ex.Message, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Csla.ApplicationContext.User = null;
                Cursor.Current = Cursors.Default;
            }
        }

        #endregion "CBV Export Command Actions"

        #endregion

        #region _ Customize commandbar _

        /// <summary>
        /// function to create a customized commandbar
        /// </summary>
        private void CreateToolbar()
        {
            // display the current Office year in tooltip
            OfficeVersions ofcVer = (OfficeVersions)((int)Convert.ToSingle(Application.Version));
            string officeYear = ofcVer.ToString().Substring(1); // substring will remove the "V" from year string
            commandbarName = "ChemDraw CBV Excel " + officeYear;

            commandBar = Application.CommandBars.Add(commandbarName, Office.MsoBarPosition.msoBarTop, missing, true);
            commandBar.Visible = true;

            NewChemBioVizWorksheet = (Office.CommandBarButton)
                commandBar.Controls.Add(Office.MsoControlType.msoControlButton, System.Type.Missing, System.Type.Missing, 1, true);
            NewChemBioVizWorksheet.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;

            NewChemBioVizWorksheet.FaceId = 65;
            NewChemBioVizWorksheet.Tag = "NCBVZS";
            NewChemBioVizWorksheet.TooltipText = "New ChemBioViz Worksheet";

            SetIcon(NewChemBioVizWorksheet, ChemBioVizExcelAddIn.Properties.Resources.cbv);

            NewChemBioVizWorksheet.Click +=
               new Office._CommandBarButtonEvents_ClickEventHandler(NewChemBioVizWorksheet_Click);


            btnLogIn = (Office.CommandBarButton)commandBar.Controls.Add(1, missing, missing, missing, true);
            btnLogIn.Style = Office.MsoButtonStyle.msoButtonCaption;
            btnLogIn.Caption = LogInText;
            btnLogIn.Tag = LogInText;
            //Attach the event with login
            btnLogIn.BeginGroup = true;
            btnLogIn.Click +=
               new Office._CommandBarButtonEvents_ClickEventHandler(LogIn_Click);


            btnInsertCBVCategoryList = (Office.CommandBarButton)
                commandBar.Controls.Add(1, missing, missing, missing, true);
            btnInsertCBVCategoryList.Style = Office.MsoButtonStyle.msoButtonIcon;
            btnInsertCBVCategoryList.Tag = "Insert Data Column";
            btnInsertCBVCategoryList.TooltipText = "Insert Data Column";
            SetIcon(btnInsertCBVCategoryList, ChemBioVizExcelAddIn.Properties.Resources.Add_Column);
            btnInsertCBVCategoryList.BeginGroup = true;
            btnInsertCBVCategoryList.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnInsertCBVCategoryList_Click);

            btnReplaceAllResults = (Office.CommandBarButton)commandBar.Controls.Add(1, missing, missing, missing, true);
            btnReplaceAllResults.Style = Office.MsoButtonStyle.msoButtonIcon;

            SetIcon(btnReplaceAllResults, ChemBioVizExcelAddIn.Properties.Resources.Search);

            btnReplaceAllResults.TooltipText = "Replaces All Results";
            btnReplaceAllResults.Tag = "Replaces All Results";

            btnReplaceAllResults.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnReplaceAllResults_Click);


            btnUpdateCurrentResults = (Office.CommandBarButton)commandBar.Controls.Add(1, missing, missing, missing, true);
            btnUpdateCurrentResults.Style = Office.MsoButtonStyle.msoButtonIcon;
            btnUpdateCurrentResults.Caption = "Update Current Results";
            btnUpdateCurrentResults.TooltipText = "Update Current Results";
            btnUpdateCurrentResults.Tag = "Update Current Results";
            SetIcon(btnUpdateCurrentResults, ChemBioVizExcelAddIn.Properties.Resources.Refresh_Y);

            btnUpdateCurrentResults.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnUpdateCurrentResults_Click);

            btnClearCurrentResults = (Office.CommandBarButton)commandBar.Controls.Add(1, missing, missing, missing, true);
            btnClearCurrentResults.Style = Office.MsoButtonStyle.msoButtonIcon;
            btnClearCurrentResults.Caption = "Clear Current Results";
            btnClearCurrentResults.Tag = "Clear Current Results";
            btnClearCurrentResults.TooltipText = "Clear Current Results";

            SetIcon(btnClearCurrentResults, ChemBioVizExcelAddIn.Properties.Resources.Cross_Y);
            btnClearCurrentResults.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnClearCurrentResults_Click);

            btnSearchPrefs = (Office.CommandBarButton)commandBar.Controls.Add(1, missing, missing, missing, true);
            btnSearchPrefs.Style = Office.MsoButtonStyle.msoButtonIcon;
            btnSearchPrefs.Caption = "Search Preferences";
            btnSearchPrefs.Tag = "Search Preferences";

            SetIcon(btnSearchPrefs, ChemBioVizExcelAddIn.Properties.Resources.Settings);
            btnSearchPrefs.Click +=
      new Office._CommandBarButtonEvents_ClickEventHandler(btnSearchPrefs_Click);
            //end tool bar buttons

            
            // Add a pop-up menu to the command bar.
            pBtnCBVSetup = (Office.CommandBarPopup)commandBar.Controls.Add(
                    Office.MsoControlType.msoControlPopup, missing, missing, missing, missing);
            pBtnCBVSetup.Caption = "Setup";
            /*pBtnCBVSetup.Enabled = false;*/
            pBtnCBVSetup.BeginGroup = true;



            pBtnCBVSearch = (Office.CommandBarPopup)commandBar.Controls.Add(
        Office.MsoControlType.msoControlPopup, missing, missing, missing, missing);
            pBtnCBVSearch.Caption = "Search";



            pBtnCBVOptions = (Office.CommandBarPopup)commandBar.Controls.Add(
        Office.MsoControlType.msoControlPopup, missing, missing, missing, missing);
            pBtnCBVOptions.Caption = "Sheet Options";

            // 11.0.4 - Button to select the data of the selected column
            btnSelectData = (Office.CommandBarButton)commandBar.Controls.Add(1, missing, missing, missing, true);
            btnSelectData.Style = Office.MsoButtonStyle.msoButtonCaption;
            btnSelectData.Caption = "Select Data";
            btnSelectData.Tag = "Select Data";
            btnSelectData.Click +=
      new Office._CommandBarButtonEvents_ClickEventHandler(btnSelectData_Click);

            //11.0.3
            btnShowDocument = (Office.CommandBarButton)commandBar.Controls.Add(1, missing, missing, missing, true);
            btnShowDocument.Style = Office.MsoButtonStyle.msoButtonCaption;
            btnShowDocument.Caption = "Show Document";
            btnShowDocument.Enabled = false;
            btnShowDocument.Tag = "Show Document";
            btnShowDocument.Click +=
      new Office._CommandBarButtonEvents_ClickEventHandler(btnShowDocument_Click);
            //

            pBtnCBVHelp = (Office.CommandBarPopup)commandBar.Controls.Add(Office.MsoControlType.msoControlPopup, missing, missing, missing, missing);
            pBtnCBVHelp.Caption = "Help";

            btnToggleColumnAutosizing = (Office.CommandBarButton)
                pBtnCBVSetup.CommandBar.Controls.Add(1, missing, missing, missing, true);
            btnToggleColumnAutosizing.Style = Office.MsoButtonStyle.msoButtonCaption;

            //The face id: http://www.kebabshopblues.co.uk/2007/01/04/visual-studio-2005-tools-for-office-commandbarbutton-faceid-property/
            btnToggleColumnAutosizing.FaceId = 0990;
            btnToggleColumnAutosizing.Caption = "Toggle Column Autosizing";
            btnToggleColumnAutosizing.Tag = "Toggle Column Autosizing";
            /*btnToggleColumnAutosizing.Enabled = false;*/
            btnToggleColumnAutosizing.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnToggleColumnAutosizing_Click);

            btnToggleHeaderDisplay = (Office.CommandBarButton)
                pBtnCBVSetup.CommandBar.Controls.Add(1, missing, missing, missing, true);
            btnToggleHeaderDisplay.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
            btnToggleHeaderDisplay.Caption = "Toggle Header Display";
            btnToggleHeaderDisplay.Tag = "Toggle Header Display";
            btnToggleHeaderDisplay.TooltipText = "Hide/Show header columns";

            SetIcon(btnToggleHeaderDisplay, ChemBioVizExcelAddIn.Properties.Resources.Merge_Cells_Below);

            btnToggleHeaderDisplay.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnToggleHeaderDisplay_Click);

            btnInsertCBVCategoryList = (Office.CommandBarButton)pBtnCBVSetup.CommandBar.Controls.Add(1, missing, missing, missing, true);
            btnInsertCBVCategoryList.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
            btnInsertCBVCategoryList.Caption = "Insert Data Column";
            btnInsertCBVCategoryList.Tag = "Insert Data Column";
            btnInsertCBVCategoryList.TooltipText = "Insert Data Column";
            SetIcon(btnInsertCBVCategoryList, ChemBioVizExcelAddIn.Properties.Resources.Add_Column);
            btnInsertCBVCategoryList.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnInsertCBVCategoryList_Click);


            // 11.0.4
            btnDeleteCBVSheetColumn = (Office.CommandBarButton)pBtnCBVSetup.CommandBar.Controls.Add(1, missing, missing, missing, true);
            btnDeleteCBVSheetColumn.Style = Office.MsoButtonStyle.msoButtonCaption;
            btnDeleteCBVSheetColumn.Caption = "Delete Data Column";
            btnDeleteCBVSheetColumn.Tag = "Delete Data Column";
            btnDeleteCBVSheetColumn.TooltipText = "Delete Data Column";
            //SetIcon(btnDeleteCBVSheetColumn, ChemBioVizExcelAddIn.Properties.Resources.Add_Column);
            btnDeleteCBVSheetColumn.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnDeleteCBVSheetColumn_Click);


            btnCBVOptions = (Office.CommandBarButton)
              pBtnCBVSetup.CommandBar.Controls.Add(1, missing, missing, missing, true);
            btnCBVOptions.Style = Office.MsoButtonStyle.msoButtonCaption;
            btnCBVOptions.Caption = "ChemBioViz Options";
            btnCBVOptions.Tag = "ChemBioViz Options";
            btnCBVOptions.TooltipText = "ChemBioViz Options";

            btnCBVOptions.Click +=
               new Office._CommandBarButtonEvents_ClickEventHandler(btnCBVOptions_Click);

            //sub command button implementation - Verify Current CBV Sheet
            btnVerifyCurrentCBVSheet = (Office.CommandBarButton)
                pBtnCBVOptions.CommandBar.Controls.Add(1, missing, missing, missing, true);
            btnVerifyCurrentCBVSheet.Style = Office.MsoButtonStyle.msoButtonCaption;
            btnVerifyCurrentCBVSheet.Caption = "Verify Current CBV Sheet";
            btnVerifyCurrentCBVSheet.Tag = "Verify Current CBV Sheet";

            btnVerifyCurrentCBVSheet.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnVerifyCurrentCBVSheet_Click);

            //sub command button implementation - Create List from Selection
            btnCreateListfromSelection = (Office.CommandBarButton)
                            pBtnCBVOptions.CommandBar.Controls.Add(1, missing, missing, missing, true);
            btnCreateListfromSelection.Style = Office.MsoButtonStyle.msoButtonCaption;
            btnCreateListfromSelection.Caption = "Create List from Selection";
            btnCreateListfromSelection.Tag = "Create List from Selection";

            btnCreateListfromSelection.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnCreateListfromSelection_Click);



            //sub command button implementation - clear current results
            btnClearCurrentResults = (Office.CommandBarButton)
                pBtnCBVOptions.CommandBar.Controls.Add(1, missing, missing, missing, true);
            btnClearCurrentResults.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
            btnClearCurrentResults.Caption = "Clear Current Results";
            btnClearCurrentResults.Tag = "Clear Current Results";

            SetIcon(btnClearCurrentResults, ChemBioVizExcelAddIn.Properties.Resources.Cross_Y);

            btnClearCurrentResults.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnClearCurrentResults_Click);

            //sub command button implementation - clear search criteria            
            btnClearSearchCriteria = (Office.CommandBarButton)
                pBtnCBVOptions.CommandBar.Controls.Add(1, missing, missing, missing, true);
            btnClearSearchCriteria.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
            btnClearSearchCriteria.Caption = "Clear Search Criteria";
            btnClearSearchCriteria.Tag = "Clear Search Criteria";

            btnClearSearchCriteria.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnClearSearchCriteria_Click);

            //sub command button implementation - Toggle Error logging
            btnToggleErrorLogging = (Office.CommandBarButton)
                pBtnCBVOptions.CommandBar.Controls.Add(1, missing, missing, missing, true);
            btnToggleErrorLogging.Style = Office.MsoButtonStyle.msoButtonCaption;
            btnToggleErrorLogging.Caption = "Toggle Error Logging";
            btnToggleErrorLogging.Tag = "Toggle Error Logging";

            btnToggleErrorLogging.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnToggleErrorLogging_Click);

            //sub command button implementation - Search Preferences
            btnSearchPrefs = (Office.CommandBarButton)
                pBtnCBVOptions.CommandBar.Controls.Add(1, missing, missing, missing, true);
            btnSearchPrefs.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
            btnSearchPrefs.Caption = "Search Preferences";
            btnSearchPrefs.Tag = "Search Preferences";
            SetIcon(btnSearchPrefs, ChemBioVizExcelAddIn.Properties.Resources.Settings);

            btnSearchPrefs.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnSearchPrefs_Click);

            //sub command button implementation - Sheet Properties
            btnSheetProperties = (Office.CommandBarButton)pBtnCBVOptions.CommandBar.Controls.Add(1, missing, missing, missing, true);
            btnSheetProperties.Style = Office.MsoButtonStyle.msoButtonIcon;
            btnSheetProperties.Caption = "Sheet Properties";
            btnSheetProperties.Tag = "Sheet properties";

            btnSheetProperties.Click +=
      new Office._CommandBarButtonEvents_ClickEventHandler(btnSheetProperties_Click);

            btnUpdateCurrentResults = (Office.CommandBarButton)
                pBtnCBVSearch.CommandBar.Controls.Add(1, missing, missing, missing, true);
            btnUpdateCurrentResults.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
            btnUpdateCurrentResults.Caption = "Update Current Results";
            btnUpdateCurrentResults.Tag = "Update Current Results";

            SetIcon(btnUpdateCurrentResults, ChemBioVizExcelAddIn.Properties.Resources.Refresh_Y);

            btnUpdateCurrentResults.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnUpdateCurrentResults_Click);

            btnAppendNewResults = (Office.CommandBarButton)
               pBtnCBVSearch.CommandBar.Controls.Add(1, missing, missing, missing, true);
            btnAppendNewResults.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
            btnAppendNewResults.Caption = "Append Current Results";
            btnAppendNewResults.Tag = "Append Current Results";

            SetIcon(btnAppendNewResults, ChemBioVizExcelAddIn.Properties.Resources.Insert_2_G);

            btnAppendNewResults.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnAppendNewResults_Click);


            btnReplaceAllResultsSub = (Office.CommandBarButton)
              pBtnCBVSearch.CommandBar.Controls.Add(1, missing, missing, missing, true);
            btnReplaceAllResultsSub.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
            btnReplaceAllResultsSub.Caption = "Replace All Results";
            btnReplaceAllResultsSub.Tag = "Replace All Results";

            SetIcon(btnReplaceAllResultsSub, ChemBioVizExcelAddIn.Properties.Resources.Search);
            btnReplaceAllResultsSub.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnReplaceAllResults_Click);


            btnExportResults = (Office.CommandBarButton)commandBar.Controls.Add(1, missing, missing, missing, true);
            btnExportResults.Style = Office.MsoButtonStyle.msoButtonIcon;
            btnExportResults.Caption = "Export Results";
            btnExportResults.TooltipText = "Export Results";
            btnExportResults.Tag = "Export Results";
            SetIcon(btnExportResults, ChemBioVizExcelAddIn.Properties.Resources.Export);
            btnExportResults.Visible = true;
            btnExportResults.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnExportResults_Click);

            //Time being the export button is invisible.
            btnExportResults.Visible = false;
            //Help commandpopup buttons            

            btnContents = (Office.CommandBarButton)
                pBtnCBVHelp.CommandBar.Controls.Add(1, missing, missing, missing, true);
            btnContents.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;

            btnContents.Caption = "Contents";
            btnContents.Tag = "Contents";
            SetIcon(btnContents, ChemBioVizExcelAddIn.Properties.Resources.content_chm);
            btnContents.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnContents_Click);

            btnAbout = (Office.CommandBarButton)pBtnCBVHelp.CommandBar.Controls.Add(1, missing, missing, missing, true);
            btnAbout.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
            btnAbout.Caption = "About";
            btnAbout.Tag = "About";
            btnAbout.Click +=
                new Office._CommandBarButtonEvents_ClickEventHandler(btnAbout_Click);
        }

        #endregion

        #region _ Utility functions _

        public void AttachSelectionChangeEvent()
        {
            Excel.Worksheet worksheet = this.Application.ActiveSheet as Excel.Worksheet;
            EventDel_SheetSelectionChange = new Excel.AppEvents_SheetSelectionChangeEventHandler(Worksheet_SheetSelectionChange);
            this.Application.SheetSelectionChange += EventDel_SheetSelectionChange;

            //11.0.3 - commented
            //EventDel_SheetActivate = new Excel.AppEvents_SheetActivateEventHandler(WorkSheet_Activate);
            //this.Application.SheetActivate += EventDel_SheetActivate;
        }
        public void DeAttachSelectionChangeEvent()
        {
            Excel.Worksheet worksheet = this.Application.ActiveSheet as Excel.Worksheet;
            EventDel_SheetSelectionChange = new Excel.AppEvents_SheetSelectionChangeEventHandler(Worksheet_SheetSelectionChange);
            this.Application.SheetSelectionChange -= EventDel_SheetSelectionChange;

            EventDel_SheetActivate = new Excel.AppEvents_SheetActivateEventHandler(WorkSheet_Activate);
            this.Application.SheetActivate -= EventDel_SheetActivate;
        }

        public void AttachWorkSheetChangeEvent()
        {
            EventDel_SheetChange = new Microsoft.Office.Interop.Excel.AppEvents_SheetChangeEventHandler(WorkSheet_Change);
            this.Application.SheetChange += new Microsoft.Office.Interop.Excel.AppEvents_SheetChangeEventHandler(WorkSheet_Change);

        }
        public void DeAttachWorkSheetChangeEvent()
        {
            EventDel_SheetChange = new Microsoft.Office.Interop.Excel.AppEvents_SheetChangeEventHandler(WorkSheet_Change);
            this.Application.SheetChange -= new Microsoft.Office.Interop.Excel.AppEvents_SheetChangeEventHandler(WorkSheet_Change);
        }

        private bool IsWorksheetModified(CBVExcel CBVExcel)
        {
            if ((Global.WorkSheetChange == true) && (Global.IsLogin == true) && (GlobalCBVExcel.Is_CBVEHidden() == true))
            {
                return true;
            }
            else
                return false;
        }

        private void VerifyCurrentSheetReName(Excel.Worksheet nSheet, string existingSheetName)
        {
            if ((!Global.IsCDExcelWorksheet(nSheet)) || (!Global.IsLogin))
                return;

            if (!string.IsNullOrEmpty(existingSheetName))
            {
                if (nSheet.Name != existingSheetName)
                {
                    CBVExcel CBVExcel = new CBVExcel();

                    CBVExcel.UpdateHiddenSheet((int)Global.CBVHiddenSheetHeader.Sheetname, existingSheetName, nSheet.Name);
                    Global.CurrentWorkSheetName = nSheet.Name;
                }
            }
        }

        private void SetErrorLoggingCaption(bool isErrorLog)
        {
            if (isErrorLog)
            {
                btnToggleErrorLogging.Caption = "Disable Error Logging";
                btnToggleErrorLogging.Tag = "Disable Error Logging";
                Global.ISErrorLogging = isErrorLog;
            }
            else if (!isErrorLog)
            {
                btnToggleErrorLogging.Caption = "Toggle Error Logging";
                btnToggleErrorLogging.Tag = "Toggle Error Logging";
                Global.ISErrorLogging = isErrorLog;
            }
        }

        private bool ISAllHeadersExists(Excel.Worksheet nSheet, Excel.Range nTarget)
        {
            object[,] cellRngVal = new object[3, 1];
            Excel.Range cellRange = nSheet.get_Range(Global.NumToString(nTarget.Column) + 1, Global.NumToString(nTarget.Column) + 3);
            cellRngVal = (object[,])cellRange.Value2;
           
            string categoryHeader = DataSetUtilities.GetDataFromCellRange(cellRngVal, (int)Global.CBVHeaderRow.HeaderCategoryRow, 1);
           
            string tableHeader = DataSetUtilities.GetDataFromCellRange(cellRngVal, (int)Global.CBVHeaderRow.HeaderTableRow, 1);
           
            string columnHeader = DataSetUtilities.GetDataFromCellRange(cellRngVal, (int)Global.CBVHeaderRow.HeaderColumnRow, 1);

            if ((!string.IsNullOrEmpty(categoryHeader)) && (!string.IsNullOrEmpty(tableHeader)) && (!string.IsNullOrEmpty(columnHeader)))
                return true;
            else
                return false;
        }

        private bool IsValidMimeType(Excel.Worksheet nSheet, Excel.Range nTarget)
        {
            if (nTarget.Rows.Count > 1 || nTarget.Columns.Count > 1)
                throw new Exception("The range can't retrived data for mime type");


                Excel.Range cellRange = nSheet.get_Range(Global.NumToString(nTarget.Column) + (int)Global.CBVHeaderRow.HeaderCategoryRow, Global.NumToString(nTarget.Column) + (int)Global.CBVHeaderRow.HeaderColumnRow);
    

                object[,] cellRngHdr = new object[3, 1];

                if (cellRngHdr == null)
                    return false;

                cellRngHdr = (object[,])cellRange.Value2;

                string catAlias = Convert.ToString(cellRngHdr[1, 1]);
                string tabAlias = Convert.ToString(cellRngHdr[2, 1]);
                string colAlias = Convert.ToString(cellRngHdr[3, 1]);
                if (string.IsNullOrEmpty(catAlias) || string.IsNullOrEmpty(tabAlias) || string.IsNullOrEmpty(colAlias))
                    //throw new Exception("Invalid cell selection");
                    return false;

                string mimeType = COEDataViewData.GetMimeType(colAlias, tabAlias);
                if (Global.ISMimeTypeExists(mimeType))
                    return true;
                else
                    throw new Exception("The selected cell haven't set the desired mime type");

        }

        /// <summary>
        ///  Open ChemBioVizExcelAddin help file
        /// </summary>
        private void OpenHelp(string sServer)  //CSBR-157234
        {
            try
            {
                bool Is2Tier = sServer.StartsWith(StringEnum.GetStringValue(Global.MRUListConstant.MRU_2T));

                if (Is2Tier)
                {  
                    string fileName = Global.GetAssemblyLocation()+ "\\Help\\" + Global.CHMFilename; //CSBR-161180
                    if (System.IO.File.Exists(fileName))
                        Help.ShowHelp(null, fileName);
                    else
                        MessageBox.Show("There is no help file. Contact ChemBioVizExcelAddIn Administrator", "No help file found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    String sProtocol = "http";
                    if (CBVUtil.StartsWith(sServer, StringEnum.GetStringValue(Global.MRUListConstant.SSL)))
                        sProtocol += "s";

                    string fileName = sProtocol + "://" + sServer + Global.HELP_URL;
                    Help.ShowHelp(null, fileName);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
       
        #endregion _ Utility functions _

        #region "Methods"
        private void LoadDataViewList()
        {
            if (dataviewListForm == null)
            {
                dataviewListForm = new frmDataviewList(this);
            }
            dataviewListForm.ShowDialog();
        }

        private void OpenCloseWorkbook()
        {
            Global._ExcelApp.Workbooks.Close();
            Global._ExcelApp.Workbooks.Add(missing);
        }


        private bool ISWorkBookOpen(Excel.Workbook workbook)
        {
            if (this.Application.Workbooks.Count > 0)
                return true;
            else
                return false;
        }

        private void CreateCBVSheetFromCBVExportSheet()
        {
            try
            {
                if (GlobalCBVExcel.IsSheetExists(Global.COEDATAVIEW_CBVEXPORT))
                {
                    CBVSheetXMLExport.SetCBVSheetExportData();
                    string sheetName = Global.CBVHiddenCriteriaCollection[(int)Global.CBVExcelExportKeys.SheetName].ToString();
                    Excel::Worksheet worksheet = (Excel::Worksheet)Global.CreateChemOfficeSheet(sheetName);
                    CBVSheetXMLExport.CreateCBVSheetXMLExport(worksheet);

                    GlobalCBVExcel.RemoveWorksheet(Global._ExcelApp.ActiveWorkbook.Worksheets, Global.COEDATAVIEW_CBVEXPORT);

                    Global.SearchUpdateCriteria = new List<object>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetDataViewPosition()
        {
            Excel::Worksheet _nWorkSheet = (Excel.Worksheet)Global._ExcelApp.ActiveWorkbook.ActiveSheet;

            if (Global.IsCBVExcelWorksheet(_nWorkSheet))
            {
                Global.InsertColumn = true;
                CBVExcel CBVExcel = new CBVExcel();

                CBVExcel.UpdateIndex(_nWorkSheet);
                CBVExcel.SetFocus(_nWorkSheet);
                //Retrieve the values in global var
                CBVExcel.SetDataViewInGlobalVars(_nWorkSheet);
            }
        }

        public void ChangeCaptionInOut()
        {
            if (Global.IsLogin)
            {
                // simply setting caption through btnLogIn.Caption does not work in Excel 2013
                // but the following statement will work in all versions
                this.Application.CommandBars[commandbarName].Controls[LogInText].Caption = LogOutText;
            }
            else
            {
                this.Application.CommandBars[commandbarName].Controls[LogOutText].Caption = LogInText;
                Global.CurrentUser = null;
                RemoveDropdownAppearance();
            }
        }

        private void RemoveDropdownAppearance()
        {
            try
            {
                Excel.Worksheet nSheet = (Excel.Worksheet)Global._ExcelApp.ActiveWorkbook.ActiveSheet;
                Excel.Range nTarget = nSheet.get_Range("A1", Global.NumToString((int)nSheet.UsedRange.Columns.Count) + ((int)Global.CBVHeaderRow.HeaderColumnLabelDisplayRow));

                nTarget.Validation.Delete();
            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }

        }

        private bool InsertDeleteRowInCBVHeaderRow(Excel.Worksheet nWorkSheet, Excel.Range nTarget)
        {
            try
            {
                if ((nTarget.Row <= (int)Global.CBVHeaderRow.HeaderColumnLabelDisplayRow) && (nTarget.Row > 0))
                {
                    Excel.Range activeCell = Global._ExcelApp.ActiveCell as Excel.Range;
                    Excel.Range cellsSelection = Global._ExcelApp.Selection as Excel.Range;
                    if (nTarget.get_Address(nTarget.Row, nTarget.Column, Excel.XlReferenceStyle.xlA1, true, nWorkSheet.Name) == nTarget.EntireRow.get_Address(nTarget.Row, nTarget.Column, Excel.XlReferenceStyle.xlA1, true, nWorkSheet.Name))
                    {
                        Global._ExcelApp.EnableEvents = false;

                        //If the application has changes 
                        try
                        {
                            Global._ExcelApp.Undo();
                        }
                        catch (Exception ex)
                        {
                            CBVExcel.ErrorLogging(ex.Message);
                        }
                        MessageBox.Show(Properties.Resources.msgInsertDelRow, Properties.Resources.msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Global._ExcelApp.EnableEvents = true;
                        return true;
                    }

                    else
                    {
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                CBVExcel.ErrorLogging(ex.Message);
            }
            return false;
        }

        private bool DeleteCriteriaRange(Excel.Range nTarget)
        {
            Office.CommandBarControl control = commandBar.FindControl(Type.Missing, 478, Type.Missing, Type.Missing, Type.Missing);

            if (null != control)
            {
                control.OnAction = "OnDelete";

                foreach (Excel.Range cell in nTarget.Cells)
                {
                    if ((!string.IsNullOrEmpty(cell.Value2.ToString())) && cell.Row < 4)
                    {
                        MessageBox.Show("Can't delete the headers data");
                        return true;
                    }
                }
            }

            return false;
        }

        //Validate the existing log server with sheet reference server
        public bool ValidateServer(bool dispMsg)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Excel.Worksheet nSheet = Global._ExcelApp.ActiveSheet as Excel.Worksheet;
                CBVExcel CBVExcel = new CBVExcel();

                if (Global.ISServerValidated == true)
                    return true;

                    if (GlobalCBVExcel.Is_CBVEHidden() && (Global.IsCDExcelWorksheet(nSheet)) && Global.IsLogin)
                {

                   int newColIndex = 1;
                    string mruServer = CBVExcel.GetDataFromHiddenSheet(nSheet.Name.Trim(), (int)Global.CBVHiddenSheetHeader.Server);
                    string sheetName = CBVExcel.GetDataFromHiddenSheet(nSheet.Name.Trim(), (int)Global.CBVHiddenSheetHeader.Sheetname);
                    string currentServer = Global.MRUServer.Trim();

                        //When null values from hideen sheet
                    if ( string.IsNullOrEmpty(mruServer) && string.IsNullOrEmpty(sheetName))
                    {
                        if (dispMsg)
                        {
                            if (MessageBox.Show(Properties.Resources.msgSelectCBVSheetServer, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            {
                                return (Global.ISServerValidated = ValidateSheet(nSheet));
                            }
                            else
                            {
                                Global.ISServerValidated = false;
                                return false;
                            }
                        }
                    }
                    else if (String.IsNullOrEmpty(mruServer) || String.IsNullOrEmpty(currentServer))
                    {
                        Global.ISServerValidated = false;
                        return false;
                    }
                    else if (mruServer.Equals(currentServer))
                    {
                        Global.ISServerValidated = true;
                        return true;
                    }
                    else
                    {  
                        if (mruServer.StartsWith(StringEnum.GetStringValue(Global.MRUListConstant.MRU_2T)))
                            mruServer = "localhost";
                        if (currentServer.StartsWith(StringEnum.GetStringValue(Global.MRUListConstant.MRU_2T)))
                            currentServer = "localhost";

                        IPHostEntry ipEntry = null;
                        IPAddress[] ipAddr1 = null;
                        IPAddress[] ipAddr2 = null;

                        if (!Global.IsValidIP(mruServer.Replace("SSL / ", "").Trim()))
                        {
                            ipEntry = Dns.GetHostEntry(mruServer);
                            ipAddr1 = ipEntry.AddressList;
                        }
                        else
                        {
                            ipAddr1 = new IPAddress[] { IPAddress.Parse(mruServer.Replace("SSL / ", "").Trim()) };
                        }
                        if (!Global.IsValidIP(currentServer))
                    {
                            ipEntry = Dns.GetHostEntry(currentServer.Replace("SSL / ", "").Trim());
                            ipAddr2 = ipEntry.AddressList;

                        }
                        else
                        {
                            ipAddr2 = new IPAddress[] { IPAddress.Parse(Global.MRUServer.Replace("SSL / ", "").Trim()) };
                        }
                        bool IsEqual = false;
                        if (ipAddr1.Length >= ipAddr2.Length)
                            IsEqual = Global.CompareIPAddress(ipAddr1, ipAddr2);
                        else
                            IsEqual = Global.CompareIPAddress(ipAddr2, ipAddr1);

                        if (!IsEqual)
                        {
                            //After server name, validate with dataview id and name
                            if (!ValidateDataView.ValidateDataViewIDNameHeaders(nSheet, newColIndex))
                            {
                            if (dispMsg)
                            {
                                    if (MessageBox.Show(Properties.Resources.msgValidateServerPrompt, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                                    {
                                        //frmDataviewList frmDataview = new frmDataviewList();
                                        //frmDataview.ShowDialog();
                                        LoginDVPopupResult(true);
                            }
                                }

                                Global.ISServerValidated = false;
                            return false;
                            } //End if IsEqual
                            else
                            {
                                Global.ISServerValidated = true;
                                return true;
                        }

                        }
                        else
                        {
                            
                            Global.ISServerValidated = true;
                            return true;
                    }
                    } //End if else
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

        }

        public bool ValidateSheet(Excel.Worksheet nSheet)
        {
            Cursor.Current = Cursors.WaitCursor;
            //Excel.Worksheet nSheet = this.Application.ActiveSheet as Excel.Worksheet;
            CBVExcel CBVExcel = new CBVExcel();

            try
            {               
                frmDataviewList frmDataview = new frmDataviewList(true);
                if (frmDataview.ShowDialog() == DialogResult.OK)
                {
                    if (ValidateDataView.ValidateDataViewIDNameHeaders(nSheet))
                    {
                        CBVExcel.RefreshCBVSheet(nSheet);
                        return true;
                    }
                    else if (MessageBox.Show(Properties.Resources.msgValidateServerPrompt, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        if (LoginDVPopupResult(true) == true)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private bool LoginDVPopupResult(bool IsBioVizSheet)
        {
            try
            {
                if (!Global.IsLogin)
                {
                    if (this.LogIn())
                    {
                        //11.0.4
                        Excel.Worksheet nSheet = Global._ExcelApp.ActiveSheet as Excel.Worksheet;
                        if (nSheet != null)
                            Global.ProtectCDXLWorkSheet(nSheet);

                        if (Global.CBVSHEET_COEDATAVIEWBO == null && IsBioVizSheet == false)
                        {
                            //Instead of message the dataview popup will display

                            if (MessageBox.Show(Properties.Resources.msgNewChemBioVizSheet, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            {
                                using (frmDataviewList frmDataview = new frmDataviewList(this))
                                {
                                    if (frmDataview.ShowDialog() == DialogResult.OK)
                                        return true;
                                    else
                                        return false;
                                }
                            }

                            else
                            {
                                return false;
                            }
                        }

                        //Only execute on a new chembioviz worksheet creation
                        else if (Global.CBVSHEET_COEDATAVIEWBO == null && IsBioVizSheet == true)
                        {
                            using (frmDataviewList frmDataview = new frmDataviewList(this))
                            {
                                if (frmDataview.ShowDialog() == DialogResult.OK)
                                    return true;
                                else
                                    return false;
                            }
                        }
                        else if (Global.CBVSHEET_COEDATAVIEWBO != null && IsBioVizSheet == true)
                        {
                            using (frmDataviewList frmDataview = new frmDataviewList(this))
                            {
                                if (frmDataview.ShowDialog() == DialogResult.OK)
                                    return true;
                                else
                                    return false;
                            }
                        }

                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
               
                /*else if (Global.CBVSHEET_COEDATAVIEWBO == null && IsBioVizSheet == false)
                {
                    //Instead of message the dataview popup will display

                    if (MessageBox.Show(Properties.Resources.msgNewChemBioVizSheet, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        using (frmDataviewList frmDataview = new frmDataviewList(this))
                        {
                            if (frmDataview.ShowDialog() == DialogResult.OK)
                                return true;
                            else
                                return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }*/

                //Only for dataview selection
                else if (Global.IsLogin == true && IsBioVizSheet == true)
                {
                    using (frmDataviewList frmDataview = new frmDataviewList(this))
                    {
                        if (frmDataview.ShowDialog() == DialogResult.OK)
                            return true;
                        else
                            return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool LoginDVPopupResult(bool IsBioVizSheet, bool IsLogin)
        {
            try
            {
                if (!Global.IsLogin)
                {
                    if (LogIn())
                    {

                        if (IsLogin == true)
                        {
                            return true;
                        }

                        else if (Global.CBVSHEET_COEDATAVIEWBO == null && IsBioVizSheet == false)
                        {
                            //Instead of message the dataview popup will display
                            if (MessageBox.Show(Properties.Resources.msgNewChemBioVizSheet, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            {
                                using (frmDataviewList frmDataview = new frmDataviewList(this))
                                {
                                    if (frmDataview.ShowDialog() == DialogResult.OK)
                                        return true;
                                    else
                                        return false;
                                }
                            }

                            else
                            {
                                return false;
                            }
                        }

                        //Only execute on a new chembioviz worksheet creation
                        else if (Global.CBVSHEET_COEDATAVIEWBO == null && IsBioVizSheet == true)
                        {
                            using (frmDataviewList frmDataview = new frmDataviewList(this))
                            {
                                if (frmDataview.ShowDialog() == DialogResult.OK)
                                    return true;
                                else
                                    return false;
                            }
                        }
                        else if (Global.CBVSHEET_COEDATAVIEWBO != null && IsBioVizSheet == true)
                        {
                            using (frmDataviewList frmDataview = new frmDataviewList(this))
                            {
                                if (frmDataview.ShowDialog() == DialogResult.OK)
                                    return true;
                                else
                                    return false;
                            }
                        }

                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (Global.CBVSHEET_COEDATAVIEWBO == null && IsBioVizSheet == false)
                {
                    //Instead of message the dataview popup will display
                    if (MessageBox.Show(Properties.Resources.msgNewChemBioVizSheet, Properties.Resources.msgCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        using (frmDataviewList frmDataview = new frmDataviewList(this))
                        {
                            if (frmDataview.ShowDialog() == DialogResult.OK)
                                return true;
                            else
                                return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                //Only for dataview selection
                else if (Global.IsLogin == true && IsBioVizSheet == true)
                {
                    using (frmDataviewList frmDataview = new frmDataviewList(this))
                    {
                        if (frmDataview.ShowDialog() == DialogResult.OK)
                            return true;
                        else
                            return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private stdole.IPictureDisp GetIcon(System.Drawing.Icon newIcon)
        {
            stdole.IPictureDisp tempImage = null;
            try
            {
                //Coverity fix - CID 18779. Used 'using' keyword to dispose the tempimage
                using (ImageList newImageList = new ImageList())
                {
                    newImageList.Images.Add(newIcon);
                    tempImage = ConvertImage.Convert(newImageList.Images[0]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tempImage;
        }

        private void SetIcon(Office.CommandBarButton button, Icon newIcon)
        {
            try
            {
                ImageList newImageList = new ImageList();
                newImageList.ColorDepth = ColorDepth.Depth8Bit;
                newImageList.ImageSize = new Size(16, 16);
                newImageList.Images.Add(newIcon);
                button.Picture = ConvertImage.Convert(newImageList.Images[0]); Bitmap mask = (Bitmap)newImageList.Images[0].Clone();
                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        mask.SetPixel(x, y, (mask.GetPixel(x, y) == Color.Transparent || mask.GetPixel(x, y).Name == "0") ? Color.White : Color.Black);
                    }
                }
                button.Mask = ConvertImage.Convert(mask);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void OpenCHMFile(string filename)
        {
            try
            {

                Global.CHMProcess process = Global.CHMProcess.Inst;

                if (Global.CHMProcess.isExists || process.HasExited)
                {
                    process.StartInfo.FileName = filename;
                    process.Start();
                }
                else
                {
                    Global.CHMProcess.ShowWindow(process.MainWindowHandle, Global.CHMProcess.SHOW_WINDOW.SW_NORMAL);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void OpenCHMFile(string filename, string processname)
        {
            try
            {
                Process process = new Process();
                Process[] pname = Process.GetProcessesByName(processname);
                if (pname.Length > 0)
                {
                    foreach (Process processes in pname)
                    {
                        if (processes.MainWindowTitle.Equals(Application.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            Global.CHMProcess.ShowWindow(processes.MainWindowHandle, Global.CHMProcess.SHOW_WINDOW.SW_MAXIMIZE);
                            return;
                        }
                    }
                }

                process.StartInfo.FileName = filename;
                process.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // 11.0.3
        private void CheckMimeType(Excel.Worksheet nSheet, Excel.Range nTarget)
        {            
            int rowPos = nTarget.Row - 7;
            int colPos = nTarget.Column - 1;

            if (rowPos >= 0 && colPos >= 0 && (Global._CBVSearchResult != null) && Global._CBVSearchResult.Rows.Count > 0 && nTarget.Column <= Global.CBVNewColumnIndex && rowPos < Global._CBVSearchResult.Rows.Count)
            {
                try
                {
                    if (IsValidMimeType(nSheet, nTarget))
                    {
                        //As enabling/Disabling the commandbarbutton control uisng the object doenot work with excel 2013, Had to make this change 
                        // as it works with all the excel versions. 
                        // CBOE-2428 Fix:In order to enable excel2013 custom toolbar menuitem
                        CommandBar contextMenu = Globals.ThisAddIn.Application.CommandBars[commandbarName.ToString()];
                        foreach (CommandBarControl c in contextMenu.Controls)
                        {
                            if (c.Caption == "Show Document")
                            {
                                var contextMenuItem = (CommandBarButton)c;
                                contextMenuItem.Enabled = true;
                            }
                        }                       
                    }                    
                }
                catch(Exception ex)
                {
                    CBVExcel.ErrorLogging(ex.Message);
                }
            }
        }


        #endregion "Methods"
        /*This class uses System.Forms.Axhost to convert an Image file to an image type                that can be applied to the menu item.*/
        sealed public class ConvertImage : System.Windows.Forms.AxHost
        {
            private ConvertImage()
                : base(null)
            {
            }
            public static stdole.IPictureDisp Convert
                (System.Drawing.Image image)
            {
                return (stdole.IPictureDisp)System.
                    Windows.Forms.AxHost
                    .GetIPictureDispFromPicture(image);
            }
        }
    }
}
