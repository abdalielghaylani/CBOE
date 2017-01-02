using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections;
using CBVUtilities;

namespace FormDBLib
{
    public class CBVExcelMgr
    {
        #region Variables
        private Microsoft.Office.Interop.Excel.Application m_xlapp;
        private Microsoft.Office.Interop.Excel.AddIn m_xlAddin;
        #endregion

        #region Constructors
        public CBVExcelMgr()
        {
            m_xlapp = null;
            m_xlAddin = null;
        }
        #endregion

        #region Methods
        //---------------------------------------------------------------------
        public void Init()
        {
            if (m_xlapp == null)
            {
                try
                {
                    m_xlapp = new Microsoft.Office.Interop.Excel.Application();
                    foreach (Microsoft.Office.Interop.Excel.AddIn addin in m_xlapp.AddIns)
                    {
                        if (CBVUtil.StartsWith(addin.Name, "ChemDrawExcel") &&
                            CBVUtil.EndsWith(addin.Name, ".xla"))
                        {
                            // activate using same trick as ChemFinder
                            if (addin.Installed)
                                addin.Installed = false;
                            addin.Installed = true;
                            m_xlAddin = addin;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to start Excel application");
                }
            }
            if (m_xlapp == null || m_xlAddin == null)
                throw new Exception("Failed to start ChemDraw/Excel application");
        }
        //---------------------------------------------------------------------
        public void Deinit()
        {
            if (m_xlapp != null)
            {
                m_xlapp.Quit(); // doesn't really work
                m_xlapp = null;
            }
       }
        //---------------------------------------------------------------------
        public void LoadSDFile(String sdFileName)
        {
            try
            {
                Init();
                m_xlapp.Visible = true;
                DoExcelLoad(m_xlapp, m_xlAddin.Name, sdFileName);
            }
            catch (Exception e)
            {
                CBVUtil.ReportError(e);
            }
            finally
            {
                // Cleanup the temporary sdf.
                File.Delete(sdFileName);
            }
           
        }
        //---------------------------------------------------------------------
        private void DoExcelLoad(Microsoft.Office.Interop.Excel.Application xlapp,
            String sXLAname, String sdFilename)
        {
            // method numbers changed between CO12 and 13
            bool bIsChemDraw12 = CBVUtil.Eqstrs(m_xlAddin.Name, "ChemDrawExcel12.xla");
            long NewWorksheet = bIsChemDraw12 ? 1 : 3001;
            long ImportFile = bIsChemDraw12 ? 2 : 3003;

            String strExecute = String.Concat(sXLAname, "!Execute");
            Object vtCommand = NewWorksheet;
            Object vtArgs = 0, vDum = Type.Missing, oFalse = false, oTrue = true;

            // create new workbook
            Microsoft.Office.Interop.Excel.Workbook workBook = xlapp.Workbooks.Add(vDum);

            // create new CD worksheet
            xlapp.Run(strExecute, vtCommand, vtArgs,
                vDum, vDum, vDum, vDum, vDum, vDum, vDum, vDum, vDum, vDum,
                vDum, vDum, vDum, vDum, vDum, vDum, vDum, vDum, vDum, vDum,
                vDum, vDum, vDum, vDum, vDum, vDum, vDum, vDum);

            Microsoft.Office.Interop.Excel.Worksheet activeSheet = xlapp.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;
            if (activeSheet == null)
                return;

            // load SD into worksheet
            vtCommand = ImportFile;
            Object[] vtArgsA = new Object[3];
            vtArgsA[0] = sdFilename;
            vtArgsA[1] = "$A1";
            vtArgsA[2] = activeSheet.Index;

            xlapp.Run(strExecute, vtCommand, vtArgsA,
               vDum, vDum, vDum, vDum, vDum, vDum, vDum, vDum, vDum, vDum,
               vDum, vDum, vDum, vDum, vDum, vDum, vDum, vDum, vDum, vDum,
               vDum, vDum, vDum, vDum, vDum, vDum, vDum, vDum);
        }
        //---------------------------------------------------------------------
        #endregion
    }
}
