using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEHitListService;

using FormDBLib;
using CBVUtilities;
using ChemControls;

namespace ChemBioViz.NET
{
    #region ChildDocForm
    public class CBVChildDocForm : ChemBioVizForm
    {
        private ChemBioVizForm m_parentForm;
        private String m_childFormName;
        private int m_hitlistID;

        public CBVChildDocForm(ChemBioVizForm parentForm, String childFormName, int hitlistID, String sSearchArg)
            : base(null)
        {
            m_childFormName = childFormName;
            m_parentForm = parentForm;
            m_hitlistID = hitlistID;
            this.CommandLine = MakeCmdLine(sSearchArg);
        }
        //---------------------------------------------------------------------
        public void RefreshSearch(int hitlistID, String sSearchArg)
        {
            // carry out a search as is done via command line
            // make a startup query and run it
            m_hitlistID = hitlistID;
            this.CommandLine = MakeCmdLine(sSearchArg);
            Query qStartup = this.CommandLine.GetStartupQuery(this.FormDbMgr, this.QueryCollection);
            CurrQuery = qStartup;

            ViewRefresh(true);
            AddQueryToTree(CurrQuery, true);
        }
        //---------------------------------------------------------------------
        public override ChemBioVizForm ParentForm
        {
            get { return m_parentForm; }
        }
        //---------------------------------------------------------------------
        public override bool IsChildDocForm
        {
            get { return true; }
        }
        //---------------------------------------------------------------------
        public String ChildFormName
        {
            get { return m_childFormName; }
        }
        //---------------------------------------------------------------------
        public int HitListID
        {
            get { return m_hitlistID; }
            set { m_hitlistID = value; }
        }
        //---------------------------------------------------------------------
        private void ParseSearchArgs(String sArgs, List<String> args)
        {
            // sArgs is like: /hitlist=22 /source=cas /target=cas
            // split into list of args
            String[] sTokens = sArgs.Split('/');
            foreach (String token in sTokens)
                if (!String.IsNullOrEmpty(token))
                    args.Add(String.Concat('/', token).Trim());
        }
        //---------------------------------------------------------------------
        private CommandLine MakeCmdLine(String sProvidedSearchArg)
        {
            List<String> args = new List<String>();
            args.Add(m_childFormName);

            if (!String.IsNullOrEmpty(sProvidedSearchArg))
                ParseSearchArgs(sProvidedSearchArg, args);
            else if (m_hitlistID > 0)
                args.Add(String.Format("/hitlist={0}", m_hitlistID));

            CommandLine cmdLine = new CommandLine(args.ToArray());
            cmdLine.Parse();
            return cmdLine;
        }
        //---------------------------------------------------------------------}
        public static CBVChildDocForm FindChildDoc(String formName)
        {
            FormCollection fcoll = Application.OpenForms;
            foreach (Form form in fcoll)
            {
                if (form is CBVChildDocForm)
                {
                    String openFormName = (form as CBVChildDocForm).ChildFormName;
                    if (CBVUtil.Eqstrs(openFormName, formName))
                        return form as CBVChildDocForm;
                }
            }
            return null;
        }
    }
    //---------------------------------------------------------------------}
    #endregion

    #region ChildForm
    public class CBVChildForm : ChemBioVizForm
    {
        private ChemBioVizForm m_parentForm;
        private String m_mainFieldName, m_childFieldName;
        private ChemDataGrid m_cdgrid;

        public CBVChildForm(ChemBioVizForm parentForm, String childFormName, String relationName, ChemDataGrid cdgrid)
            : base(null)
        {
            m_parentForm = parentForm;
            m_cdgrid = cdgrid;

            m_parentForm.RecordChanged += new RecordChangedEventHandler(ParentForm_RecordChanged);
            m_parentForm.SearchCompleted += new SearchCompletedEventHandler(m_parentForm_SearchCompleted);
            m_parentForm.SubRecordChanged += new SubRecordChangedEventHandler(m_parentForm_SubRecordChanged);

            m_mainFieldName = m_childFieldName = String.Empty;
            DataSet mainDataSet = m_parentForm.Pager.CurrDataSet;
            if (mainDataSet.Relations != null && mainDataSet.Relations.Count > 0)
            {
                DataRelation rel = FindRelHavingName(mainDataSet, relationName);
                if (rel != null && rel.ParentColumns != null && rel.ParentColumns.Length > 0 &&
                        rel.ChildColumns != null && rel.ChildColumns.Length > 0)
                {
                    m_mainFieldName = rel.ParentColumns[0].ColumnName;
                    m_childFieldName = rel.ChildColumns[0].ColumnName;
                }
            }
            this.CommandLine = MakeCmdLine(childFormName);
        }
        //---------------------------------------------------------------------
        public override void OnLoadForm()
        {
            base.OnLoadForm();
            this.MainToolbarManager.Visible = false;
        }
        //---------------------------------------------------------------------
        private DataRelation FindRelHavingName(DataSet dataSet, String relName)
        {
            foreach (DataRelation rel in dataSet.Relations)
                if (CBVUtil.Eqstrs(rel.RelationName, relName))
                    return rel;
            return null;
        }
        //---------------------------------------------------------------------
        public override bool IsChildForm
        {
            get { return true; }
        }
        //---------------------------------------------------------------------
        public override ChemBioVizForm ParentForm
        {
            get { return m_parentForm; }
        }
        //---------------------------------------------------------------------
        public ChemDataGrid ParentSubformGrid
        {
            get { return m_cdgrid; }
        }
        //---------------------------------------------------------------------
        private CommandLine MakeCmdLine(String childFormName)
        {
            String curPK = GetCurPK(this.ParentForm, m_mainFieldName);  // result might be blank
            String searchArg = MakeSearchArg(curPK, m_childFieldName);  // ditto
            CommandLine cmdLine = null;
            if (String.IsNullOrEmpty(searchArg))
            {
                String[] args = { childFormName };
                cmdLine = new CommandLine(args);
            }
            else
            {
                String[] args = { childFormName, searchArg };
                cmdLine = new CommandLine(args);
            }
            if (cmdLine != null)
                cmdLine.Parse();
            return cmdLine;
        }
        //---------------------------------------------------------------------
        void m_parentForm_SearchCompleted(object sender, EventArgs e)
        {
            if (!this.Visible) return;
            Fill();
            DoMove(Pager.MoveType.kmGotoPageRow, 0);
            ViewRefresh();
            Application.DoEvents();
        }
        //---------------------------------------------------------------------
        void m_parentForm_SubRecordChanged(object sender, SubRecordChangedEventArgs e)
        {
            // change record of parent subform: move to same record in our child form
            // CSBR-134655: do this via Find within records, do not use record number
            if (!this.Visible) return;
            if (e.grid == null) return;

            // must have a pk field
            String pkField = this.FormDbMgr.PKFieldName();
            if (String.IsNullOrEmpty(pkField))
                return;

            // find the value in that field from the active row of the sending subform
            UltraGridRow ugRow = e.grid.ActiveRow;
            if (ugRow == null) return;
            if (!ugRow.Band.Columns.Contains(pkField)) return;
            UltraGridColumn ugCol = ugRow.Band.Columns[pkField];
            if (ugCol == null) return;
            String sDataVal = ugRow.GetCellText(ugCol);

            // search for matching val in our dataset, using BindingSource.Find
            // this assumes we have the whole set of records in memory, not paged
            BindingSource bs = this.BindingSource;
            int recno = bs.Find(pkField, sDataVal);

            // if found, move in child form to that record
            if (recno >= 0 && recno < this.Pager.ListSize)
                DoMove(Pager.MoveType.kmGoto, recno);
        }
        //---------------------------------------------------------------------
        void ParentForm_RecordChanged(object sender, RecordChangedEventArgs e)
        {
            // change main record in parent form: refill this child form
            if (!this.Visible) return;
            Fill();
            DoMove(Pager.MoveType.kmGotoPageRow, 0);
            ViewRefresh();
            Application.DoEvents();
        }
        //---------------------------------------------------------------------
        public void Fill()
        {
            if (!String.IsNullOrEmpty(m_mainFieldName) && !String.IsNullOrEmpty(m_childFieldName))
                Fill(m_mainFieldName, m_childFieldName);
        }
        //---------------------------------------------------------------------
        public static String GetCurPK(ChemBioVizForm parentForm, String parentLinkField)
        {
            String sDataVal = String.Empty;
            DataSet dataSet = parentForm.Pager.CurrDataSet;
            DataTable table = dataSet.Tables[0];
            DataColumn dataCol = table.Columns[parentLinkField];

            if (dataCol != null)
            {
                int curRow = parentForm.Pager.CurrRow;
                int colIndex = table.Columns.IndexOf(dataCol);
                if (colIndex >= 0)
                    sDataVal = table.Rows[curRow].ItemArray.GetValue(colIndex).ToString();
            }
            return sDataVal;
        }
        //---------------------------------------------------------------------
        public static String MakeSearchArg(String curPK, String childLinkField)
        {
            String searchArg = String.Empty;
            if (!String.IsNullOrEmpty(curPK) && !String.IsNullOrEmpty(childLinkField))
                searchArg = String.Format("/search={0}:{1}", childLinkField, curPK);
            return searchArg;
        }
        //---------------------------------------------------------------------
        public void Fill(String parentLinkField, String childLinkField)
        {
            if (!IsChildForm)
                return;

            String curPK = GetCurPK(this.ParentForm, parentLinkField);
            if (String.IsNullOrEmpty(curPK))
                return;

            Query qNew = Query.CreateFromStrings(childLinkField, curPK,
                                this.FormDbMgr, this.QueryCollection, false, false);
            try
            {
                qNew.RestoreHitlist(); // uses saved hitlist if possible, otherwise runs query
                TabManager.Bind(qNew, BindingSource);
            }
            catch (Exception e)
            {
                Console.WriteLine("FILL error: " + e.Message);  // might just mean no hits
                qNew = null;
            }
            CurrQuery = qNew;
        }
    }
    //---------------------------------------------------------------------
    #endregion
}