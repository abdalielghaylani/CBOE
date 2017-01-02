using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Data;

using ChemBioViz.NET;
using FormDBLib;
using CBVUtilities;
using ChemControls;
using SharedLib;

namespace CBVNAddins
{
    public class CBVSampleAddin : ICBVAddin
    {
        protected ChemBioVizForm m_form;

        public void Init(object form)
        {
            m_form = form as ChemBioVizForm;
        }
        //---------------------------------------------------------------------
        public virtual void Execute() { }
        public void ExecuteWithString(string s)         { }
        public void Deinit()                            { }
        public virtual string GetDescription()          { return string.Empty; }
        public IAddinMenu GetMenu()                     { return null; }
        public string GetMenuImagePath()                { return string.Empty; }
        public bool HandleMenuCommand(string menuCmd)   { return false; }
        public bool IsEnabled(string menuCmd)           { return false; }
        public bool IsChecked(string menuCmd)           { return false; }
        public string GetSettings()                     { return string.Empty; }
        public void SetSettings(string s)               { }
        //---------------------------------------------------------------------
        public bool ShowSplash(String sTitle)
        {
            String sMessage = String.Concat(sTitle, " Add-in\n\n", GetDescription());
            DialogResult dResult = MessageBox.Show(sMessage, "Add-In", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            return dResult == DialogResult.OK;
        }
        //---------------------------------------------------------------------
        public static bool CurrStructToFile(ChemBioVizForm form, String sFilename)
        {
            DataSet dataSet = form.Pager.CurrDataSet;
            if (dataSet == null || dataSet.Tables.Count == 0) return false;
            int currRow = form.Pager.CurrRowInPage;
            if (currRow < 0) return false;
            DataRow dataRow = dataSet.Tables[0].Rows[currRow];
            if (dataRow == null) return false;

            String sBase64 = dataRow["STRUCTURE"].ToString();
            if (String.IsNullOrEmpty(sBase64)) return false;

            ChemDraw chemdraw = new ChemDraw();
            chemdraw.Base64 = sBase64;
            Object oCdxml = chemdraw.AxChemDrawControl.get_Data("cdxml");
            if (oCdxml == null) return false;
            String sCdxml = oCdxml.ToString();
            CBVUtil.StringToFile(sCdxml, sFilename);
            return true;
        }
    }
    //---------------------------------------------------------------------
    public class CBVExcelDemoAddin : CBVSampleAddin
    {
        public override string GetDescription()
        {
            return "Exports current page of records to Excel";
        }
        //---------------------------------------------------------------------
        public override void Execute()
        {
            if (m_form != null)
            {
                if (!ShowSplash("Excel Demo"))
                    return;

                DataSet dataSet = m_form.Pager.CurrDataSet;
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    List<String> strs = CBVUtil.DataSetToCSV(dataSet, "STRUCTURE", true, ',');
                    String sFilename = Path.Combine(Path.GetTempPath(), "xl_tmp.csv");

                    CBVUtil.StringsToFile(strs, sFilename, false);
                    CBVUtil.Launch("Excel", sFilename);
                }
            }
        }
    }
    //---------------------------------------------------------------------
    public class CBV3DDemoAddin : CBVSampleAddin
    {
        public override string GetDescription()
        {
            return "Exports current structure to Chem3D";
        }
        //---------------------------------------------------------------------
        public override void Execute()
        {
            if (m_form != null)
            {
                if (!ShowSplash("Chem3D Demo"))
                    return;

                // get path to Chem3D.exe using file association
                FileAssociation fileAssocation = new FileAssociation();
                string s3DExePath = fileAssocation.Get(".c3d");
                if (String.IsNullOrEmpty(s3DExePath))
                    return;

                // save file as cdxml, then pass to 3D exe
                String sFilename = Path.Combine(Path.GetTempPath(), "d3d_tmp.cdxml");
                if (File.Exists(sFilename))
                    File.Delete(sFilename);
                if (CBVSampleAddin.CurrStructToFile(m_form, sFilename))
                {
                    CBVUtil.Launch(s3DExePath, sFilename);
                }
            }
        }
    }
    //---------------------------------------------------------------------
    public class CBVChemDrawDemoAddin : CBVSampleAddin
    {
        public override string GetDescription()
        {
            return "Exports current structure to ChemDraw";
        }
        //---------------------------------------------------------------------
        public override void Execute()
        {
            if (m_form != null)
            {
                if (!ShowSplash("ChemDraw Demo"))
                    return;

                String sFilename = Path.Combine(Path.GetTempPath(), "dcd_tmp.cdxml");
                if (File.Exists(sFilename))
                    File.Delete(sFilename);
                if (CBVSampleAddin.CurrStructToFile(m_form, sFilename))
                {
                    CBVUtil.LaunchDoc(sFilename);
                }
            }
        }
    }
    //---------------------------------------------------------------------}
    public class CBVRecViewerAddin : CBVSampleAddin
    {
        public override string GetDescription()
        {
            return "Exports current page of records to grid window";
        }
        //---------------------------------------------------------------------
        public override void Execute()
        {
            if (m_form != null)
            {
                if (!ShowSplash("Record Viewer Demo"))
                    return;

                RecViewDlg dlg = new RecViewDlg(m_form);
                dlg.Show();
            }
        }
    }
    //---------------------------------------------------------------------}
}
