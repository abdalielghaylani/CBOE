using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data;
using System.Xml;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEExportService;
using CambridgeSoft.COE.Framework.COEHitListService;

using FormDBLib;
using CBVUtilities;
using Utilities;
using System.ComponentModel;
using System.IO;

namespace ChemBioViz.NET
{
    public class ExportOpts
    {
        #region Variables
        public enum ExportType { SDMain, SDUncorrelated, SDCorrelated, SDNested };  // SD export only
        public enum DelimiterType { Comma, Tab };  // delimited export only

        private String m_outputPath;
        private ExportType m_exportType;
        private List<String> m_allFieldNames;
        private List<String> m_checkedFieldNames;
        private int dataviewID;
        private DelimiterType m_delimiterType;
        private bool m_bWithHeader;
        private bool m_bForExcel;   // exporting sd to send to cd/excel
        #endregion

        #region Constructor
        public ExportOpts()
        {
            m_outputPath = "";
            m_exportType = ExportType.SDMain;
            m_allFieldNames = new List<String>();
            m_checkedFieldNames = new List<String>();
            m_delimiterType = DelimiterType.Comma;
            m_bWithHeader = true;
            m_bForExcel = false;
        }
        #endregion

        #region Properties
        public ExportType ExportOptsType
        {
            get { return m_exportType; }
            set { m_exportType = value; }
        }
        public String OutputPath
        {
            get { return m_outputPath; }
            set { m_outputPath = value; }
        }
        public DelimiterType TextDelimiterType
        {
            get { return m_delimiterType; }
            set { m_delimiterType = value; }
        }
        public List<String> AllFieldNames
        {
            get { return m_allFieldNames; }
            set { m_allFieldNames = value; }
        }
        public List<String> ExportFieldNames
        {
            get { return m_checkedFieldNames; }
            set { m_checkedFieldNames = value; }
        }
        public int DataviewID
        {
            get { return dataviewID; }
            set { dataviewID = value; }
        }
        public bool WithHeader
        {
            get { return m_bWithHeader; }
            set { m_bWithHeader = value; }
        }
        public bool ForExcel
        {
            get { return m_bForExcel; }
            set { m_bForExcel = value; }
        }
        #endregion

        #region Methods
        public XmlElement CreateXmlElement(XmlDocument xdoc, String eltname)
        {
            XmlElement elt = xdoc.CreateElement(eltname);

            elt.SetAttribute("output_path", m_outputPath);
            elt.SetAttribute("export_type", m_exportType.ToString());
            elt.SetAttribute("delim_type", m_delimiterType.ToString());
            elt.SetAttribute("with_header", m_bWithHeader ? "1" : "0");
            elt.SetAttribute("fields", CBVUtil.CreateDelimitedStringList(m_checkedFieldNames));
            elt.SetAttribute("for_excel", m_bForExcel ? "1" : "0");
            return elt;
        }
        //---------------------------------------------------------------------
        public void LoadXmlElement(XmlNode node)
        {
            m_outputPath = CBVUtil.GetStrAttrib(node, "output_path");

            String sExpType = CBVUtil.GetStrAttrib(node, "export_type");
            if (!String.IsNullOrEmpty(sExpType) && Enum.IsDefined(typeof(ExportType), sExpType))
                m_exportType = (ExportType)Enum.Parse(typeof(ExportType), sExpType);

            String sDelim = CBVUtil.GetStrAttrib(node, "delim_type");
            if (!String.IsNullOrEmpty(sDelim) && Enum.IsDefined(typeof(DelimiterType), sDelim))
                m_delimiterType = (DelimiterType)Enum.Parse(typeof(DelimiterType), sDelim);

            m_bWithHeader = CBVUtil.GetIntAttrib(node, "with_header") == 1;
            m_bForExcel = CBVUtil.GetIntAttrib(node, "for_excel") == 1;
            String sFields = CBVUtil.GetStrAttrib(node, "fields");
            if (!String.IsNullOrEmpty(sFields))
                m_checkedFieldNames = CBVUtil.ParseDelimitedStringList(sFields);
        }
        #endregion
    }
    //---------------------------------------------------------------------
    /// <summary>
    ///   Base class for data exporters
    /// </summary>
    public class Exporter
    {
        #region Variables
        protected ChemBioVizForm m_form;
        #endregion

        #region Properties
        public ExportOpts ExportOpts 
        {
            get { return m_form.ExportOpts; }
            set { m_form.ExportOpts = value; } 
        }
        #endregion

        #region Constructors
        public Exporter(ChemBioVizForm form)
        {
            m_form = form;
        }
        #endregion
    }
    //---------------------------------------------------------------------
    public class SDExporter : Exporter
    {
        public SDExporter(ChemBioVizForm form)
            : base(form)
        {
        }
        //---------------------------------------------------------------------
        //Fixed CSBR-161147
        public bool Export(ref int? incompRecCnt, BackgroundWorker bgwOwner = null)
        {
            // export the current hitlist to local file
            COEExport exporter = new COEExport();            

            // for debug: break here, you see SDFFlatFileUncorrelated, SDFFlatFileCorrelated, SDFNested
            // no new list as expected for 12.3
            List<String> exportTypes = exporter.GetFormatterTypesList();

            // get overall result criteria
            // if exporting no child tables, remove them from rc
            bool bUseStructHilites = false;
            ResultsCriteria rc = FormUtil.FieldListToRC(m_form.FormDbMgr, ExportOpts.ExportFieldNames, bUseStructHilites), rc0 = rc;
            if (rc == null)
                return false;
            if (ExportOpts.ExportOptsType == ExportOpts.ExportType.SDMain)
                rc = FormUtil.RemoveChildTables(rc0);

            // set up parameters for operation
            COEDataView dataView = m_form.FormDbMgr.SelectedDataView;
            Query q = m_form.CurrQuery;
            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.RecordCount = (q == null) ? m_form.FormDbMgr.BaseTableRecordCount : q.NumHits;
            pagingInfo.HitListID = (q == null) ? 0 : q.HitListID;
            pagingInfo.HitListType = (q != null && q.IsSaved) ? HitListType.SAVED : HitListType.TEMP;

            // set export type name
            String exportType = "SDFFlatFileUncorrelated";
            //JHS 6/15/2012 - The Correlated export is not delineating records and creating a bogus file.
            //The following line is corret but should be commented out until it can be properly fixed.  At least Uncorrelated is defensible.
            //if (ExportOpts.ExportOptsType == ExportOpts.ExportType.SDCorrelated) exportType = "SDFFlatFileCorrelated";
            if (ExportOpts.ExportOptsType == ExportOpts.ExportType.SDCorrelated) exportType = "SDFFlatFileUncorrelated";
            else if (ExportOpts.ExportOptsType == ExportOpts.ExportType.SDNested) exportType = "SDFNested";

            StreamWriter sw = null;
            try
            {
                // do the export, retrieve results into string, then save to file
                // TO DO: do this in pages
                /* CSBR-136190: Feature Request: It would be helpful if the Export status is shown while exporting to SDFile
                 * Changes Done by Jogi 
                 * Calculating the Page size and setting the start and end propertires of the pagingInfo class
                 * Iterating the process for loop times based on record count and witing the fetched data for 
                 * each iteration in to file, This code change is also to fix the Connection timeout error message */

                #region Iteration to fetch the Data Page by Page
                int recordCount = pagingInfo.RecordCount;
                pagingInfo.RecordCount = 0;
                int recordsPerPage;
                int increment = 0;
                string result = null;
                if (recordCount < 1000)
                    recordsPerPage = 100;
                else
                    recordsPerPage = 1000;
                int loop = recordCount / recordsPerPage;
                int reminder = recordCount % recordsPerPage;
                if (File.Exists(ExportOpts.OutputPath))
                    File.Delete(ExportOpts.OutputPath);
                incompRecCnt = 0;
                sw = new StreamWriter(ExportOpts.OutputPath, true);

                if (bgwOwner != null && bgwOwner.IsBusy && !bgwOwner.CancellationPending)
                {
                    if (recordCount <= recordsPerPage)
                    {
                        pagingInfo.Start = 1;
                        if (reminder != 0)
                            pagingInfo.End = reminder;
                        else
                            pagingInfo.End = recordsPerPage;
                        for (int i = 1; i <= 1; i++)
                        {
                            result = exporter.GetData(rc, pagingInfo, dataView, exportType);
                            System.Threading.Thread.Sleep(1000);
                            bgwOwner.ReportProgress(100, "Fetched " + i * recordsPerPage + " Records of " + recordCount + " Records ");                                   
                            if (bgwOwner.CancellationPending) break;                          
                            sw.WriteLine(result);
                            if (i == 1)                           
                                incompRecCnt = null; // Set null to variable on full iteration 
                            else                            
                                incompRecCnt = i * recordsPerPage; //Get the total retrieved value in variable                           
                        }
                    }
                    else if (recordCount > recordsPerPage && reminder == 0)
                    {
                        pagingInfo.Start = 1;
                        pagingInfo.End = recordsPerPage;
                        for (int i = 1; i <= loop; i++)
                        {
                            result = null;
                            result = exporter.GetData(rc, pagingInfo, dataView, exportType);
                            increment = increment + (100 / loop);
                            System.Threading.Thread.Sleep(1000);
                            bgwOwner.ReportProgress(increment, "Fetched " + i * recordsPerPage + " Records of " + recordCount + " Records ");
                            if (bgwOwner.CancellationPending) break;
                            sw.WriteLine(result);
                            if (i == loop)                           
                                incompRecCnt = null; // Set null to variable on full iteration
                            else
                                incompRecCnt = i * recordsPerPage; //Get the total retrieved value in variable                           
                            pagingInfo.Start = i * recordsPerPage + 1;
                            pagingInfo.End = (i + 1) * recordsPerPage;
                        }
                    }
                    else if (recordCount > recordsPerPage && reminder != 0)
                    {
                        loop = loop + 1;
                        int start = pagingInfo.Start;
                        pagingInfo.Start = 1;
                        pagingInfo.End = recordsPerPage;
                        int progress = 100 % loop;
                        for (int i = 1; i <= loop; i++)
                        {
                            result = null;
                            result = exporter.GetData(rc, pagingInfo, dataView, exportType);
                            if (progress >= i)
                                increment = increment + (100 / loop) + 1;
                            else increment = increment + (100 / loop);
                            System.Threading.Thread.Sleep(1000);
                            bgwOwner.ReportProgress(increment, "Fetched " + i * recordsPerPage + " Records of " + recordCount + " Records ");
                            if (bgwOwner.CancellationPending) break;
                            sw.WriteLine(result);
                            if (i == loop)                           
                                incompRecCnt = null; // Set null to variable on full iteration
                            else
                                incompRecCnt = i * recordsPerPage; //Get the total retrieved value in variable                           
                            pagingInfo.Start = i * recordsPerPage + 1;
                            if ((i + 1) != loop)
                                pagingInfo.End = (i + 1) * recordsPerPage;
                            else
                                pagingInfo.End = i * recordsPerPage + reminder;
                        }
                    }                    
                }

                if (!File.Exists(ExportOpts.OutputPath) && ExportOpts.OutputPath.Length == 0)
                    throw new Exception("No data retrieved -- no file will be saved");
           
            /* End of CSBR-136190 */

                # endregion
            }
            catch (Exception e)
            {
                CBVUtil.ReportError(e);
                ExportOpts.ForExcel = false;    //MN:19-SEP-2013:: CBOE-1877 
                return false;
            }
            finally
            {
                /* CSBR-160178 - Records data are not exported into the sdf file.
                     * As the StreamWriter oject was not flushed and closed the stream appears to be incomplete
                     * which is the cause of the problem */
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                }
            }
            return true;
        }
    }
    //---------------------------------------------------------------------
    public class DelimExporter : Exporter
    {
        private ResultsCriteria m_rcExport;
        private Pager m_pager;
        private int m_currRow;
        private DataSet m_dataSet;
        private int m_hitlistID;
        private bool m_isHitlistSaved;

        public DelimExporter(ChemBioVizForm form)
            : base(form)
        {
            m_rcExport = null;
            m_pager = new Pager(form.FormDbMgr);
            m_dataSet = null;
            m_currRow = -1;
            m_hitlistID = 0;
            m_isHitlistSaved = false;
        }
        //---------------------------------------------------------------------
        private bool CreateResultsCriteria()
        {
            // adapted from GenerateRC() in CBVChart.cs
            // NOTE: this expects ExportFieldNames to contain only fields from the base table!
            FormDbMgr formDbMgr = m_form.FormDbMgr;
            COEDataView.DataViewTable t = formDbMgr.SelectedDataViewTable;
            if (t == null) return false;

            ResultsCriteria.ResultsCriteriaTable rcTable = new ResultsCriteria.ResultsCriteriaTable();
            rcTable.Id = t.Id;

            m_rcExport = new ResultsCriteria();
            m_rcExport.Tables.Add(rcTable);

            bool bUseStructHilites = false;
            foreach (String fieldName in ExportOpts.ExportFieldNames)
            {
                // just to be safe:
                if (fieldName.Contains(":")) 
                    continue;
                FormUtil.FieldToRC(fieldName, formDbMgr.SelectedDataView, t, rcTable, bUseStructHilites);
            }

            return true;
        }
        //-----------------------------------------------------------------------
        // Create Result criteria for tables with child
        private ResultsCriteria CreateResultCriteriaForDelimWithChild()
        {            
            FormDbMgr formDbMgr = m_form.FormDbMgr;
            COEDataView.DataViewTable t = formDbMgr.SelectedDataViewTable;            
            ResultsCriteria.ResultsCriteriaTable rcTable = new ResultsCriteria.ResultsCriteriaTable();
            
            m_rcExport = new ResultsCriteria();
            bool bUseStructHilites = false;
            m_rcExport = FormUtil.FieldListToRC(m_form.FormDbMgr, ExportOpts.ExportFieldNames, bUseStructHilites);          

            return m_rcExport;
        }
        
        //---------------------------------------------------------------------
        public void InitPager()
        {
            m_pager.RowsPerPage = 50;
            m_pager.Clear();
            m_currRow = 1;

            Query q = m_form.CurrQuery;
            if (q != null) {
                m_hitlistID = q.HitListID;
                m_isHitlistSaved = q.IsSaved;
            }
        }
        //---------------------------------------------------------------------
        public bool RetrieveDataPage()
        {
            m_dataSet = Pager.GetNRecordsEx(m_rcExport, m_form.FormDbMgr.SelectedDataView,
                                            m_pager.RowsPerPage, m_hitlistID, m_isHitlistSaved, m_currRow);
            int nRows = (m_dataSet == null || m_dataSet.Tables.Count == 0)? 0 : m_dataSet.Tables[0].Rows.Count;
            m_currRow += nRows;
            return nRows > 0;
        }
        //---------------------------------------------------------------------
        public int ExportDataPage(int pageNo)
        {
            // return 1 if there is more to be exported after this page, 0 if not, -1 if err
            bool bWithHeader = (pageNo == 0 && ExportOpts.WithHeader);
            char delim = (ExportOpts.TextDelimiterType == ExportOpts.DelimiterType.Comma) ? ',' : '\t';
            List<String> strs = CBVUtil.DataSetToCSV(m_dataSet, "STRUCTURE", bWithHeader, delim);
            int nHasMore = 0;
            if (strs.Count > 0)
            {
                bool bAppend = pageNo > 0;
                if (!CBVUtil.StringsToFile(strs, ExportOpts.OutputPath, bAppend))
                    nHasMore = -1;
                else if (strs.Count >= this.m_pager.RowsPerPage)
                    nHasMore = 1;
            }
            return nHasMore;
        }
        //------------------------------------------------------------------
		// Convert to CSV with the Delimiter
        public int ExportDelimDataPage(ResultsCriteria rc, COEDataView dataView, int pageNo)
        {
            // return 1 if there is more to be exported after this page, 0 if not, -1 if err
            bool bWithHeader = (pageNo == 0 && ExportOpts.WithHeader);
            char delim = (ExportOpts.TextDelimiterType == ExportOpts.DelimiterType.Comma) ? ',' : '\t';
            // Delim Export for Parent and Child table
            List<String> strs = CBVUtil.DataSetToCSvWithChild(rc, dataView, m_dataSet, "STRUCTURE", bWithHeader, delim); 
            int nHasMore = 0;
            if (strs.Count > 0)
            {
                bool bAppend = pageNo > 0;
                if (!CBVUtil.StringsToFile(strs, ExportOpts.OutputPath, bAppend))
                    nHasMore = -1;
                else if (strs.Count >= this.m_pager.RowsPerPage)
                    nHasMore = 1;
            }
            return nHasMore;
        }
		
        //---------------------------------------------------------------------
        private void ReportProgress(int pageNo)
        {
            int rowNum = pageNo * this.m_pager.RowsPerPage;
            String msg = String.Format("Exporting record {0}", rowNum);
            //CBVStatMessage.Show(msg); .. can't call across threads
            Debug.WriteLine(msg);
        }
        //---------------------------------------------------------------------
        public bool Export()
        {
            // export the current hitlist to local file; selected fields only
            // steps: create RC; retrieve page of rows; output text rows to output file; repeat until no more pages
            // return false if any error encountered
            COEDataView dataView = m_form.FormDbMgr.SelectedDataView; 
            if (ExportOpts.ExportFieldNames == null)
                return false;

            if (m_rcExport == null)
                // Uncomment Only for exporting DV with Base Table and comment the below line 
                //CreateResultsCriteria(); 
                // Delim export for DV with Child tables : Returns the result criteria of the selected DV 
                m_rcExport = CreateResultCriteriaForDelimWithChild(); 

            InitPager();
            int pageNo = 0, code = 0;
            while (RetrieveDataPage())
            {
                // Uncomment only for exporting DV with Base Table and comment the below line 
                //code = ExportDataPage(pageNo++);// Uncomment only for exporting DV with Base Table and comment the below line 
                // Delim export for DV with Child tables : Convert Dataset to CSV and write the data to file  
                code = ExportDelimDataPage(m_rcExport, dataView, pageNo++);

                if (code != 1)
                    break;
                ReportProgress(pageNo);
            }
            return code != -1;  // CSBR-134754: return false if error
        }
        //---------------------------------------------------------------------
    }

    // CBV Excel Export
    //---------------------------------------------------------------------
    public class CBVExcelExporter : Exporter
    {
        public CBVExcelExporter(ChemBioVizForm form)
            : base(form)
        {
        }

        //---------------------------------------------------------------------
        public string Export()
        {
            ResultsCriteria rc = FormUtil.FormToResultsCriteria(m_form);
            COEDataView dataView = m_form.FormDbMgr.SelectedDataView;
            SearchCriteria sc = m_form.CurrQuery.SearchCriteria;           

            COEFormInterchange coeFormInterchange = new COEFormInterchange();
            string serverName = Login.ParseServerMachineName(m_form.FormDbMgr.Login.ServerDisplay);
            ServerInfo serverInfo = new ServerInfo(serverName, !m_form.FormDbMgr.Login.Is2Tier, m_form.FormDbMgr.Login.IsSSL);

            Query q = m_form.CurrQuery;
            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.RecordCount = (q == null) ? m_form.FormDbMgr.BaseTableRecordCount : q.NumHits;
            pagingInfo.HitListID = (q == null) ? 0 : q.HitListID;
            pagingInfo.HitListType = (q != null && q.IsSaved) ? HitListType.SAVED : HitListType.TEMP;
           
            if (q != null)
            {
                if (q.IsMergeQuery)
                {                   
                    pagingInfo.HitListQueryType = HitListQueryType.MERGED; //CSBR-153844 
                }
                else if (q.HasParentQuery)
                {
                    sc = SearchOverCriteria(m_form.CurrQuery, sc);
                    pagingInfo.HitListQueryType = HitListQueryType.SEARCHOVER; //CSBR-153838 and CSBR-155235
                }
                else
                {                    
                    pagingInfo.HitListQueryType = HitListQueryType.OTHER;
                }
            }
            else
            {              
                pagingInfo.HitListQueryType = HitListQueryType.OTHER;
            }

            if (sc == null)
            {
                sc = new SearchCriteria();
            }
           
            try
            {
                string exportedExcelContent = coeFormInterchange.GetForm(dataView, sc, rc, pagingInfo, serverInfo, m_form.FormDbMgr.Login.UserName, "CBVExport", COEFormInterchange.formatterType.CBVExcel.ToString());

                return exportedExcelContent;
            }
            catch (Exception e)
            {
                CBVUtil.ReportError(e);
                return string.Empty;
            }            
        }
        //----------------------------------------------------------------------------
        List<string> m_ComponentsTagList = new List<string>();
        public SearchCriteria SearchOverCriteria(Query currQuery, SearchCriteria sc)
        {
            //Coverity Bug Fix :CID:13024 
            if (currQuery != null)
            {
                if (currQuery.Components.Count > 0)
                {
                    if (!m_ComponentsTagList.Contains(currQuery.Components[0].Tag))
                    {
                        //To get the least child criteria (if same field used more than one time in search over criteria) 
                        m_ComponentsTagList.Add(currQuery.Components[0].Tag); //CSBR-155235
                    }
                }

                if (currQuery.ParentQuery != null) //If current Query has parent query (multilevel tree)
                {
                    if (currQuery.ParentQuery.SearchCriteria != null)
                    {
                        if (!m_ComponentsTagList.Contains(currQuery.ParentQuery.Components[0].Tag))
                        {
                            foreach (SearchCriteria.SearchCriteriaItem item in currQuery.ParentQuery.SearchCriteria.Items)
                            {
                                if ((!sc.Items.Contains(item)) && (item.ID > 0))
                                {
                                    sc.Items.Add(item);
                                }
                            }
                        }
                    }
                    SearchOverCriteria(currQuery.ParentQuery, sc); //Recursive call
                }
            }
            return sc;
        }
       
    }
}
