using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Diagnostics;
using System.ComponentModel.Design;
using System.Web.UI.Design;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using Infragistics.Win.UltraWinGrid;

using ChemBioViz.NET;
using ChemControls;
using FormDBLib;
using CBVUtilities;
using Utilities;
using SearchPreferences;

namespace CBVControls
{
    public class CBVSafeChemDrawBox : TextBox
    {
        #region Constructors
        public CBVSafeChemDrawBox()
        {
            // CD replacement to use during editing
        }
        #endregion

        #region Properties
        public override Size MinimumSize
        {
            get { return base.MinimumSize; }
            set { base.MinimumSize = value; }
        }
        #endregion

        #region Methods
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            // TO DO: find a way to center the text vertically
            this.Text = "\r\n\r\n\r\nChemDraw Box";

            this.TextAlign = HorizontalAlignment.Center;
            this.Multiline = true;
            this.BackColor = Color.LightGray;
            this.ForeColor = Color.DarkGray;
        }
        #endregion
    }
    //---------------------------------------------------------------------
    public class CBVButton : Button
    {
        #region Variables and Enums
        public enum ActionType
        {
            None, Launch, LaunchDoc, LaunchURL, OpenForm, Google, MsgBox, ChgTab, Addin, JumpTo, LaunchEmbedded
        }

        private ActionType m_actionType;
        private String m_actionArgs;
        private String m_displayString;
        private String m_tooltipText;
        private CBVButtonProps m_buttonProps;
        private bool m_bShowOnMenu;
        private String m_menuName;
        private bool m_bIsAccept, m_bIsCancel;
        private ToolTip m_toolTip;
        private Object m_dataObj;   // for embedded docs
        #endregion

        #region Constructors
        public CBVButton()
        {
            // button with wildcard strings for executing various actions
            m_displayString = m_tooltipText = String.Empty;
            m_actionType = ActionType.MsgBox;
            this.Click += new EventHandler(CBVButton_Click);
            m_buttonProps = new CBVButtonProps(this);
            m_bShowOnMenu = true;
            m_menuName = "Action";
            m_bIsAccept = m_bIsCancel = false;

            m_toolTip = new ToolTip();
            m_dataObj = null;
        }
        #endregion

        #region Properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [BrowsableAttribute(false)]
        public ChemBioVizForm Form
        {
            get { return this.TopLevelControl as ChemBioVizForm; }  // WRONG
        }
        //---------------------------------------------------------------------
        public bool IsAcceptButton
        {
            get { return m_bIsAccept; }
            set { m_bIsAccept = value; }
        }
        //---------------------------------------------------------------------
        public bool IsCancelButton
        {
            get { return m_bIsCancel; }
            set { m_bIsCancel = value; }
        }
        //---------------------------------------------------------------------
        public new String Text
        {
            get { return base.Text; }
            set
            {
                if (String.IsNullOrEmpty(m_displayString))
                    base.Text = value;
                else
                {
                    RowStack rstack = FormUtil.GetRowStack(this);
                    base.Text = MakeSubstitutions(m_displayString, false, rstack);
                }
            }
        }
        //---------------------------------------------------------------------
        public ActionType Action
        {
            get { return m_actionType; }
            set { m_actionType = value; }
        }
        //---------------------------------------------------------------------
        public String Arguments
        {
            get { return m_actionArgs; }
            set { m_actionArgs = value; }
        }
        //---------------------------------------------------------------------
        public String TooltipText
        {
            get { return m_tooltipText; }
            set { m_tooltipText = value; }
        }
        //---------------------------------------------------------------------
        public String DisplayString
        {
            get { return m_displayString; }
            set { m_displayString = value; }
        }
        //---------------------------------------------------------------------
        public bool ShowOnMenu
        {
            get { return m_bShowOnMenu; }
            set { m_bShowOnMenu = value; }
        }
        //---------------------------------------------------------------------
        public String MenuName
        {
            get { return m_menuName; }
            set { m_menuName = value; }
        }
        //---------------------------------------------------------------------
        public String BoundFieldName
        {
            get
            {
                String s = "(unbound)";
                if (Tag != null && !String.IsNullOrEmpty(Tag.ToString()))
                {
                    s = CBVUtil.AfterDelimiter(Tag.ToString(), '.');
                }
                else if (DataBindings != null && DataBindings.Count > 0)
                {
                    s = DataBindings[0].BindingMemberInfo.BindingMember;
                    if (Form.TabManager.CurrentTab.IsFormView())
                    {
                        //Coverity Bug Fix CID 12935 
                        FormViewControl Fvc = Form.TabManager.CurrentTab.Control as FormViewControl;
                        if (Fvc != null)
                        {
                            s = ControlSwapperEx.BindingToTag(this, Fvc);
                            s = CBVUtil.AfterDelimiter(s, '.');
                        }
                    }
                }
                return s;
            }
        }
        //---------------------------------------------------------------------
        [BrowsableAttribute(true)]
        [DisplayNameAttribute("(Advanced)")]
        [EditorAttribute(typeof(CBVButtonPropsEditor), typeof(UITypeEditor))]
        public CBVButtonProps ButtonProps
        {
            get { return new CBVButtonProps(this); }
            set
            {
                CBVButtonProps props = value as CBVButtonProps;
                this.Action = props.ActionType;
                this.Arguments = props.ActionArgs;
                this.DisplayString = props.DisplayLabel;
                this.TooltipText = props.TooltipText;
            }
        }
        #endregion

        #region Methods
        public List<String> GetArgFieldNames()
        {
            List<String> v = new List<String>();
            if (!String.IsNullOrEmpty(this.Arguments))
            {
                String s = this.Arguments;
                int i1 = 0, i2 = 0;

                // process each token within percents: might be fieldname, internal var, or blank (=> curr field)
                while (((i1 = s.IndexOf('%')) >= 0) && ((i2 = s.IndexOf('%', i1 + 1)) > i1))
                {
                    String token = s.Substring(i1 + 1, i2 - i1 - 1);
                    if (String.IsNullOrEmpty(token))
                        token = BoundFieldName;
                    if (!String.IsNullOrEmpty(token))
                        v.Add(token);
                    String sTmp = s.Substring(0, i1) + "XXX" + s.Substring(i2 + 1);
                    s = sTmp;
                }
            }
            return v;
        }
        //---------------------------------------------------------------------
        private static String ParseAndFill(String sTemplate, DataSet dataSet, String curFieldName, RowStack rstack, ChemBioVizForm form,
                                            bool bIsBase64, ref Object dataObj)
        {
            // given user-input string with tokens in %%, substitute data values for tokens
            dataObj = null;
            if (String.IsNullOrEmpty(sTemplate))
                return "";

            String s = sTemplate;
            int i1 = 0, i2 = 0;
            if (dataSet == null || dataSet.Tables.Count == 0)
                return s;

            // process each token within percents: might be fieldname, internal var, or blank (=> curr field)
            while (((i1 = s.IndexOf('%')) >= 0) && ((i2 = s.IndexOf('%', i1 + 1)) > i1))
            {
                String token = s.Substring(i1 + 1, i2 - i1 - 1);
                if (String.IsNullOrEmpty(token))
                    token = curFieldName;

                // new 7/09: allow special tokens to be substituted
                String dataVal = "";
                if (FormUtil.IsInternalVariable(token, form))
                {
                    dataVal = FormUtil.GetInternalValue(token, form);
                }
                else
                {
                    DataTable table = dataSet.Tables[0];
                    String fieldName = token;
                    // if fieldname contains colon, retrieve subform data item from child rows
                    // new 9/10: new scheme to retrieve from grandchild; only works in form view with subforms present     
                    bool bIsSubField = fieldName.Contains(":");
                    if (bIsSubField)
                    {
                        DataTable subtable = FormViewControl.GetSubDataTable(token, form.BindingSource, form.FormDbMgr);
                        if (subtable == null)
                            continue;
                        fieldName = CBVUtil.AfterDelimiter(token, ':');
                        int mainRow = (rstack.Count == 0) ? 0 : rstack.Pop();
                        // find chain of relations linking main table to sub
                        List<DataRelation> dataRelList = CBVUtil.FindRelPathByTables(table, subtable, dataSet);
                        if (form.TabManager.CurrentTab.IsFormView())
                        {
                            FormViewControl fvc = form.TabManager.CurrentTab.Control as FormViewControl;
                            //coverity Bug Fix CID 12934 
                            if (fvc != null)
                            {
                                fvc.GetSelectedRowsByRels(dataRelList); // attaches rownums as extended props of each rel
                                dataVal = CBVUtil.GetValByPath(dataRelList, mainRow, fieldName, dataSet);
                            }
                        }
                        else if (dataRelList.Count > 0)
                        {
                            // for hyperlinks in grids: use the old pre-9/10 
                            int curRow = mainRow;
                            DataRelation dataRel = dataRelList[0];
                            if (dataRel != null)
                            {
                                mainRow = curRow;
                                DataRow[] subRows = table.Rows[mainRow].GetChildRows(dataRel);
                                DataColumn dataCol = subtable.Columns[fieldName];
                                if (dataCol != null)
                                {
                                    int colIndex = subtable.Columns.IndexOf(dataCol);
                                    int subRow = (rstack.Count == 0) ? 0 : rstack.Pop();
                                    // CSBR-147470: Hyperlinks in Grid or Table tab are not showing data.To handle situations when child table has no matching data.
                                    if (subRows.Length > 0)
                                    {
                                        if (colIndex >= 0)
                                            dataVal = subRows[subRow].ItemArray.GetValue(colIndex).ToString();
                                    }
                                    else
                                    {
                                        String msg = "There is no matching child table data";
                                        return msg;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // otherwise retrieve item from main rows
                        int curRow = (rstack.Count == 0) ? 0 : rstack.Peek();
                        DataColumn dataCol = (table == null) ? null : table.Columns[fieldName];
                        if (dataCol == null)
                        {
                            dataVal = fieldName;    // change %NAME% to just NAME and keep going
                            bIsBase64 = false;      // HACK ... in case first field is name of blob but doesn't exist in dset
                        }
                        else
                        {
                            int colIndex = table.Columns.IndexOf(dataCol);
                            if (bIsBase64)
                            {
                                dataObj = table.Rows[curRow].ItemArray.GetValue(colIndex);
                                dataVal = fieldName;
                                bIsBase64 = false;  // only the first token is b64 data
                            }
                            else
                            {
                                dataVal = table.Rows[curRow].ItemArray.GetValue(colIndex).ToString();
                            }
                        }
                    }
                }
                String sTmp = s.Substring(0, i1) + dataVal + s.Substring(i2 + 1);
                s = sTmp;
            }
            return s;
        }
        //---------------------------------------------------------------------
        internal static String ActionTypeName(ActionType type)
        {
            switch (type)
            {
                case ActionType.None: return "None";
                case ActionType.Launch: return "Launch Program";
                case ActionType.LaunchURL: return "Launch URL";
                case ActionType.LaunchDoc: return "Launch Document";
                case ActionType.OpenForm: return "Open Form";
                case ActionType.Google: return "Google";
                case ActionType.MsgBox: return "Message";
                case ActionType.ChgTab: return "Change Tab";
                case ActionType.Addin: return "Add-in";
                case ActionType.JumpTo: return "Jump to";
                case ActionType.LaunchEmbedded: return "Launch Embedded Document";
            }
            return "";
        }
        //---------------------------------------------------------------------
        internal static String ActionDescription(ActionType type)
        {
            switch (type)
            {
                case ActionType.None: return "No action on click.";
                case ActionType.Launch: return "Launch program. Argument is a program name or path, followed optionally by a parameter list.";
                case ActionType.LaunchURL: return "Launch URL in web browser. Argument is a full URL.";
                case ActionType.LaunchDoc: return "Launch document file in its application. Argument is a pathname to a file.";
                case ActionType.OpenForm: return "Open Form. Argument is the name of public or private form, or pathname to a local form file.";
                case ActionType.Google: return "Search Google. Argument is a search query. May not work on all browsers.";
                case ActionType.MsgBox: return "Display a message in an alert box. Argument is the message to be displayed.";
                case ActionType.ChgTab: return "Bring specified tab to the front.  Argument is text of the target tab.";
                case ActionType.Addin: return "Invoke default method of named add-in.  For list, choose Add-in from combo.";
                case ActionType.JumpTo: return "Jump to alternate form.  Argument is form name, optional /source=<field> /target=<field> for link search.";
                case ActionType.LaunchEmbedded: return "Launch document stored in database.  Argument is blob field name, optional /file=<field> for accompanying filename.";
            }
            return "";
        }
        //---------------------------------------------------------------------
        #endregion

        #region Events
        protected override void OnMouseHover(EventArgs e)
        {
            String msg = String.Empty;
            if (ChemBioViz.NET.Properties.Settings.Default.ShowTooltips)
            {
                String sRaw = TooltipText;
                if (!String.IsNullOrEmpty(sRaw))
                {
                    RowStack rstack = FormUtil.GetRowStack(this);
                    msg = MakeSubstitutions(sRaw, false, rstack);
                }
            }
            m_toolTip.SetToolTip(this, msg);
            base.OnMouseHover(e);
        }
        //---------------------------------------------------------------------
        private void CBVButton_Click(object sender, EventArgs e)
        {
            // get the current row(s), then call the action routine below
            RowStack rstack = FormUtil.GetRowStack(sender as CBVButton);
            DoClickAction(e, rstack);
        }
        //---------------------------------------------------------------------
        private String MakeSubstitutions(String raw, bool bIsBase64, RowStack rstack)
        {
            ChemBioVizForm form1 = this.Form;
            DataSet dataSet = null;
            String curFieldName = String.Empty;
            Query q = (form1 == null) ? null : form1.CurrQuery;
            if (q != null)
            {
                dataSet = q.DataSet;
                if (this.Tag != null)
                    curFieldName = CBVUtil.AfterDelimiter(this.Tag.ToString(), '.');
            }
            String s = ParseAndFill(raw, dataSet, curFieldName, rstack, form1, bIsBase64, ref m_dataObj);
            return s;
        }
        //---------------------------------------------------------------------
        private void DoLaunchEmbeddedDocEx(String sArgs)
        {
            // this version retrieves the blob, then launches
            // args are <blobfldname> [/file=<filenamefield>]
            m_dataObj = null;

            // create a query to retrieve the current row
            String pkColName = this.Form.FormDbMgr.PKFieldName();
            String curPK = CBVChildForm.GetCurPK(this.Form, pkColName);

            String sTmp = sArgs, sFilename = CBVUtil.ParseQualifier(sTmp, "/FILE", ref sTmp);
            String blobFieldName = sTmp;
            Query qBlob = Query.CreateFromStrings(pkColName, curPK, this.Form.FormDbMgr, this.Form.QueryCollection, false, false);

            // create a resultsCriteria to get just the blob field
            ResultsCriteria rcBlob = FormUtil.MakeSingleFieldRC(blobFieldName, this.Form.FormDbMgr.SelectedDataView, this.Form.FormDbMgr.SelectedDataViewTable);

            // run the query
            ResultsCriteria rcCurr = Form.FormDbMgr.ResultsCriteria;
            Form.FormDbMgr.ResultsCriteria = rcBlob;    // swap in this one for a minute
            qBlob.Run();
            Form.FormDbMgr.ResultsCriteria = rcCurr;

            // extract m_dataObj from the resulting dataset
            DataSet dsBlob = qBlob.Pager.CurrDataSet;
            if (dsBlob != null && dsBlob.Tables.Count > 0)
            {
                DataTable table = dsBlob.Tables[0];
                m_dataObj = table.Rows[0].ItemArray.GetValue(0);
            }

            // pass to the launch routine below
            DoLaunchEmbeddedDoc(sArgs);
        }
        //---------------------------------------------------------------------
        private void DoLaunchEmbeddedDoc(String sArgs)
        {
            // args = <blobfield> [/file=<filenamefield>]
            // blob was retrieved to m_dataObj during ParseAndFill
            if (String.IsNullOrEmpty(sArgs)) return;
            String sFilename = CBVUtil.ParseQualifier(sArgs, "/FILE", ref sArgs);
            String sFullPath = sFilename;
            if (String.IsNullOrEmpty(sFilename))
            {

                // if no filename provided, use mimetype on column
                String fieldName = sArgs;
                COEDataView.DataViewTable t = Form.FormDbMgr.SelectedDataViewTable;
                COEDataView.Field dvField = FormDbMgr.FindDVFieldByName(Form.FormDbMgr.SelectedDataViewTable, fieldName);
                if (dvField != null)
                {
                    String sExt = CBVUtil.MimeTypeToFileExt(dvField.MimeType);
                    if (!String.IsNullOrEmpty(sExt))
                        sFilename = String.Format("tmpdoc{0}", sExt);
                }
            }
            else
            {
                if (CBVUtil.EndsWith(sFullPath, "\\"))  // hack for docmgr location field
                    sFullPath = sFullPath.Substring(0, sFullPath.Length - 1);
                sFilename = Path.GetFileName(sFullPath);
            }
            // write blob to temporary file
            if (m_dataObj != null && !String.IsNullOrEmpty(sFilename))
            {
                String sTempDir = Path.GetTempPath();
                String sPath = Path.Combine(sTempDir, sFilename);
                File.WriteAllBytes(sPath, m_dataObj as System.Byte[]);
                System.Diagnostics.Process.Start(sPath);
            }
        }
        //---------------------------------------------------------------------
        internal void DoClickAction(EventArgs e, RowStack rstack)
        {
            // click: parse and process the attached string, then launch according to type
            ChemBioVizForm form1 = this.Form;
            m_dataObj = null;
            bool bIsDataBase64 = m_actionType == ActionType.LaunchEmbedded;
            String sArgs = MakeSubstitutions(m_actionArgs, bIsDataBase64, rstack);

            try
            {
                switch (m_actionType)
                {
                    case ActionType.Google:
                        String sRawURL = CBVConstants.GOOGLE_LINK;
                        String sURL = sRawURL.Replace("%%", sArgs);
                        System.Diagnostics.Process.Start(sURL);
                        break;

                    case ActionType.LaunchURL:
                    case ActionType.LaunchDoc:
                        System.Diagnostics.Process.Start(sArgs);
                        break;

                    case ActionType.LaunchEmbedded:
                        if (m_dataObj != null)
                            DoLaunchEmbeddedDoc(sArgs);
                        else
                            DoLaunchEmbeddedDocEx(sArgs);
                        break;

                    case ActionType.Addin:
                        form1.ExecuteAddin(sArgs);
                        break;

                    case ActionType.Launch:
                        {
                            // split into <exe> <args> separated by space
                            String[] sa = CommandLine.SplitCommandLine(sArgs);
                            if (sa.Length == 2)
                                System.Diagnostics.Process.Start(sa[0], sa[1]);
                            else
                                System.Diagnostics.Process.Start(sArgs);
                        }
                        break;

                    case ActionType.OpenForm:
                        //form1.UnloadForm();     // fixes CSBR-127628 but causes CSBR-126120 and also 128632

                        form1.FireCBVFormClosed();  // CSBR-156727

                        if (!form1.CheckForSaveOnClose())   // CSBR-128632
                            return;

                        form1.RemoveActionMenus();  // CSBR-134011

                        if (CBVUtil.EndsWith(sArgs, ".xml"))
                            form1.LoadLocalForm(sArgs);
                        else
                            form1.LoadFormByPath(sArgs);
                        break;

                    case ActionType.MsgBox:
                        MessageBox.Show(sArgs);
                        break;

                    case ActionType.ChgTab:
                        form1.TabManager.SelectNamedTab(sArgs);
                        break;

                    case ActionType.JumpTo:
                        form1.DoJumpTo(sArgs);
                        break;
                }
                form1.FireButtonEvent(this.Text, sArgs, this.MenuName);

            }
            catch (Exception ex)
            {
                CBVUtil.ReportError(ex, "Action button error");
            }
        }
        #endregion
    }
    //---------------------------------------------------------------------   
    public class CBVDataGridView : DataGridView
    {
        #region Variables
        private List<CBVDataGridView> m_childGrids;
        private bool m_bHidden;
        private String m_childFormName;
        private String m_infraLayoutXml;
        #endregion

        #region Properties
        [BrowsableAttribute(false)]
        public List<CBVDataGridView> ChildGrids
        {
            get { return m_childGrids; }
            set { m_childGrids = value; }
        }
        //---------------------------------------------------------------------
        [BrowsableAttribute(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new object DataSource
        {
            get { return base.DataSource; }
            set { base.DataSource = value; }
        }
        //---------------------------------------------------------------------
        [BrowsableAttribute(false)]
        public bool Hidden
        {
            get { return m_bHidden; }
            set { m_bHidden = value; }
        }
        //-------------------------------------------------------------------------------------          
        public String ChildFormName
        {
            get { return m_childFormName; }
            set { m_childFormName = value; }
        }
        //-------------------------------------------------------------------------------------          
        [BrowsableAttribute(false)]
        public String InfraLayoutXml
        {
            get { return this.m_infraLayoutXml; }
            set { this.m_infraLayoutXml = value; }
        }
        //-------------------------------------------------------------------------------------          
        #endregion

        #region Constructors
        public CBVDataGridView()
        {
            m_bHidden = false;
            DataSourceChanged += new EventHandler(CBVDataGridView_DataSourceChanged);
        }
        #endregion

        void CBVDataGridView_DataSourceChanged(object sender, EventArgs e)
        {
            if (DesignMode)
            {
                // CSBR-155133: This doesn't seem to happen by default,
                //  leading to lost columns. ???
                Columns.Clear();
                // CSBR-156177: Restrict columns here too
                DataTable dataTable = CBVUtil.BindingSourceToDataTable(DataSource as BindingSource);
                if (dataTable != null)
                    ChemBioVizForm.RestrictColumns(dataTable);
            }
        }

        #region Methods
        /// <summary>
        /// Code for converting between two types of grid:
        /// CBVDataGridView (abbrev DGV or W) based on Windows DataGridView; for use in editing and serializing
        /// ChemDataGrid (CDG or I) based on Infragistics UltraGrid; for use in main display
        /// </summary>
        /// <param name="dgCol"></param>
        /// <param name="cdgCol"></param>
        private static void CopyWtoIColProps(DataGridViewColumn dgCol, UltraGridColumn cdgCol)
        {
            // copy properties of a single column from Win to Infragistics grid
            // this is not yet finished; is just doing a couple of key props right now

            ControlFactory.CopyObjectProperties(dgCol, cdgCol);
            cdgCol.Hidden = !dgCol.Visible;
            cdgCol.Header.VisiblePosition = dgCol.DisplayIndex;
            cdgCol.Header.Caption = dgCol.HeaderText; // CSBR-109804
            // can't set key, is read-only
            cdgCol.Format = dgCol.DefaultCellStyle.Format;
            cdgCol.CellAppearance.BackColor = dgCol.DefaultCellStyle.BackColor;

            if (cdgCol.DataType.Name.Equals("Byte[]"))
                cdgCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image; ;
        }
        //---------------------------------------------------------------------
        private static void CopyItoWColProps(UltraGridColumn cdgCol, DataGridViewColumn dgCol)
        {
            try
            {
                ControlFactory.CopyObjectProperties(cdgCol, dgCol);
                dgCol.Visible = !cdgCol.Hidden;
                dgCol.DisplayIndex = cdgCol.Header.VisiblePosition; // can throw if out of range
                dgCol.DataPropertyName = cdgCol.Key;
                dgCol.HeaderText = cdgCol.Header.Caption;   // CSBR-109807
                dgCol.DefaultCellStyle.Format = cdgCol.Format;

                // semi-hack: set molwt format to 2 decimal places when saving
                if (CBVUtil.Eqstrs(dgCol.DataPropertyName, "molweight") && String.IsNullOrEmpty(cdgCol.Format))
                    dgCol.DefaultCellStyle.Format = "F2";
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        //---------------------------------------------------------------------
        private static DataGridViewColumn FindMatchingCol(UltraGridColumn igCol, DataGridViewColumnCollection dgvCols)
        {
            foreach (DataGridViewColumn dgCol in dgvCols)
            {
                if (CBVUtil.Eqstrs(dgCol.DataPropertyName, igCol.Key))
                    return dgCol;
            }
            return null;
        }
        //---------------------------------------------------------------------
        private static bool IsColOnList(UltraGridColumn igCol, List<String> fieldNames)
        {
            foreach (String s in fieldNames)
                if (CBVUtil.Eqstrs(s, igCol.Key))
                    return true;
            return false;
        }
        //---------------------------------------------------------------------
        public static void ReformatMolweightCols(ChemDataGrid igrid)
        {
            // semi-hack: set molwt format to 2 decimal places; used when creating new grid
            ColumnsCollection cdgCols = igrid.DisplayLayout.Bands[0].Columns;
            foreach (UltraGridColumn igCol in cdgCols)
            {
                if (CBVUtil.Eqstrs(igCol.Key, "molweight") && String.IsNullOrEmpty(igCol.Format))
                    igCol.Format = "F2";
            }
        }
        //---------------------------------------------------------------------
        public static void HideIGridColsNotListed(ChemDataGrid igrid, List<String> fieldNames)
        {
            // loop over columns of grid, hide any not found on given list
            Debug.Assert(igrid.DisplayLayout.Bands.Count > 0);
            ColumnsCollection cdgCols = igrid.DisplayLayout.Bands[0].Columns;
            foreach (UltraGridColumn igCol in cdgCols)
            {
                bool bIsChildTableCol = igCol.IsChaptered;
                if (!bIsChildTableCol)
                {
                    igCol.Hidden = false;
                    if (!IsColOnList(igCol, fieldNames))
                        igCol.Hidden = true;
                }
            }
        }
        //---------------------------------------------------------------------
        private static void HideBandWithKey(ChemDataGrid igrid, String key, bool bHide)
        {
            foreach (UltraGridBand band in igrid.DisplayLayout.Bands)
            {
                if (CBVUtil.Eqstrs(band.Key, key))
                    band.Hidden = bHide;
            }
        }
        //---------------------------------------------------------------------
        public static void HideIGridChildGridsNotListed(ChemDataGrid igrid, List<String> tableNames)
        {
            // loop over child cols of grid; hide any band not in given list
            ColumnsCollection cdgCols = igrid.DisplayLayout.Bands[0].Columns;
            foreach (UltraGridColumn igCol in cdgCols)
            {
                bool bIsChildTableCol = igCol.IsChaptered;
                if (bIsChildTableCol)
                {
                    bool bIsListed = IsColOnList(igCol, tableNames);
                    HideBandWithKey(igrid, igCol.Key, !bIsListed);
                }
            }
        }
        //---------------------------------------------------------------------
        private static int GetWRowHeight(CBVDataGridView wgrid)
        {
            int ht = wgrid.RowTemplate.Height;
            return ht;
        }
        //---------------------------------------------------------------------
        private static int GetIRowHeight(ChemDataGrid igrid, int bandNo)
        {
            UltraGridBand band = igrid.DisplayLayout.Bands[bandNo];
            int defht = band.Override.DefaultRowHeight;
            int row1ht = 0;

            // get height from top row if any
            if (band.Layout.Rows.Count > 0)
            {
                UltraGridRow topRow = band.Layout.Rows[0];
                row1ht = topRow.Height;

                // if child, look at top row of given child band
                if (bandNo > 0 && topRow.ChildBands.Count >= bandNo)
                {
                    UltraGridChildBand childBand = topRow.ChildBands[bandNo - 1];
                    if (childBand != null && childBand.Rows.Count > 0)
                        row1ht = childBand.Rows[0].Height;
                }
            }
            return (row1ht > 0) ? row1ht : (defht > 0) ? defht : 10;
        }
        //---------------------------------------------------------------------
        public static void CopyWtoIGridProps(CBVDataGridView wgrid, ChemDataGrid igrid, int bandNo, bool bHidden,
                                        bool bHeaderVisible)
        {
            // copy properties of column collection from Win to Infra
            if (bandNo >= igrid.DisplayLayout.Bands.Count)
                return;

            UltraGridBand band = igrid.DisplayLayout.Bands[bandNo];
            if (band == null) return;

            // copy row height
            if (bandNo == 0)
                band.Override.DefaultRowHeight = GetWRowHeight(wgrid);

            ColumnsCollection cdgCols = band.Columns;                   // full set of bound cols
            DataGridViewColumnCollection dgvCols = wgrid.Columns;       // may be a subset          

            band.Hidden = bHidden;

            if (String.IsNullOrEmpty(band.Header.Caption))
                band.Header.Caption = wgrid.Name;

            // set header visible only in main grid, not subforms
            band.HeaderVisible = bHeaderVisible;  // CSBR-110605
            //CSBR-165203: Hiding Column Header for Sub-form grids 
            band.ColHeadersVisible = wgrid.ColumnHeadersVisible;
            if (bHeaderVisible && igrid.IsSubformGrid)
                band.HeaderVisible = false;

            // loop through the full set in the Infra grid
            foreach (UltraGridColumn igCol in cdgCols)
            {
                // if there is a matching col in the W grid, set the IG props from it
                // if not, then hide the IG one
                DataGridViewColumn dgMatchingCol = FindMatchingCol(igCol, dgvCols);
                if (dgMatchingCol != null)
                    CopyWtoIColProps(dgMatchingCol, igCol);
                else
                    igCol.Hidden = true;
            }
        }
        //---------------------------------------------------------------------
        public static void CopyItoWGridProps(ChemDataGrid igrid, CBVDataGridView wgrid, int bandNo)
        {
            UltraGridBand band = igrid.DisplayLayout.Bands[bandNo];
            ColumnsCollection cdgCols = band.Columns;
            DataGridViewColumnCollection dgvCols = wgrid.Columns;
            if (cdgCols.Count == 0)
                return;

            // CSBR-132594: attach display layout xml to wgrid
            // CSBR-154743: ... but only for main table in grid view,
            //  as that's the only place it's used; see GridViewTab.Bind
            if (!igrid.IsSubformGrid && (bandNo == 0))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    igrid.DisplayLayout.SaveAsXml(stream);
                    wgrid.InfraLayoutXml = CBVUtil.StreamToString(stream);
                }
            }

            // CSBR-118089: get default row height from band override; (11/09) or from row 1
            wgrid.RowTemplate.Height = GetIRowHeight(igrid, bandNo);
            //CSBR-165203: Hiding Column Header for Sub-form grids
            wgrid.ColumnHeadersVisible = band.ColHeadersVisible;

            // CSBR-133393: factor out routine to sort cols, for use in preparing cardview
            UltraGridColumn[] colOrder = ChemDataGrid.SortInVisibleOrder(cdgCols);

            // first loop adds all cols (including hidden)
            foreach (UltraGridColumn col in colOrder)
            {
                dgvCols.Add(col.Key, col.Key);
            }
            // second loop sets props
            int i = 0;
            foreach (UltraGridColumn col in colOrder)
            {
                DataGridViewColumn dgvCol = dgvCols[col.Key];
                //Coverity Bug Fix  local Analysis
                if (dgvCol != null)
                {
                    CopyItoWColProps(col, dgvCol);
                    dgvCol.DataPropertyName = col.Key;
                    dgvCol.DisplayIndex = i++;
                }
            }
        }
        #endregion
    }
    //---------------------------------------------------------------------
    public class CBVLookupCombo : ComboBox
    {
        #region Variables
        private DataSet m_dataSet;              // retrieved table of id/name pairs
        private ResultsCriteria m_rcLookup;     // rc for retrieval
        private int m_lookupBaseTableID;
        #endregion

        #region Constructors
        public CBVLookupCombo()
        {
            m_dataSet = null;
            m_rcLookup = null;
            m_lookupBaseTableID = 0;
            this.DropDown += new EventHandler(CBVLookupCombo_DropDown);
        }
        #endregion

        #region Methods
        //---------------------------------------------------------------------
        public COEDataView.Field BoundLookupField()
        {
            // return null unless bound field is a genuine lookup field
            COEDataView.DataViewTable t = Form.FormDbMgr.SelectedDataViewTable;
            if (t != null)
            {
                String fieldName = BoundFieldName;
                // might be subfield, like "FCV_COLLECTIONS:STATUS_ID"
                bool bIsSubField = fieldName.Contains(":");
                if (bIsSubField)
                {
                    String tname = CBVUtil.BeforeDelimiter(fieldName, ':');
                    String fname = CBVUtil.AfterDelimiter(fieldName, ':');
                    if (!String.IsNullOrEmpty(tname) && !String.IsNullOrEmpty(fname))
                    {
                        t = Form.FormDbMgr.FindDVTableByName(tname);    // this child table contains the ID, not the data
                        fieldName = fname;
                    }
                }
                COEDataView.Field fkField = FormDbMgr.FindDVFieldByName(t, fieldName);
                if (fkField != null && fkField.LookupFieldId != -1 && fkField.LookupDisplayFieldId != -1)
                    return fkField;
            }
            return null;
        }
        //---------------------------------------------------------------------
        private void GenerateRC()
        {
            // look at bound field -- generate rc only if it is a lookup field
            m_rcLookup = null;
            COEDataView.Field fkField = BoundLookupField();
            if (fkField == null)
                return;

            // find the lookup table -- search for any table containing the lookup field
            COEDataView.DataViewTable tLookup =
                FormDbMgr.FindDVTableWithField(fkField.LookupFieldId, this.Form.FormDbMgr.SelectedDataView);
            if (tLookup == null)
                return;

            // create results criteria on lookup table
            ResultsCriteria.ResultsCriteriaTable rcTable = new ResultsCriteria.ResultsCriteriaTable();
            rcTable.Id = tLookup.Id;
            m_lookupBaseTableID = rcTable.Id;

            m_rcLookup = new ResultsCriteria();
            m_rcLookup.Tables.Add(rcTable);

            // add ID and TEXT fields to retrieve the lookup fields
            ResultsCriteria.Field idfield = new ResultsCriteria.Field();
            idfield.Id = fkField.LookupFieldId;
            idfield.Alias = "ID";
            rcTable.Criterias.Add(idfield);

            ResultsCriteria.Field textfield = new ResultsCriteria.Field();
            textfield.Id = fkField.LookupDisplayFieldId;
            textfield.Alias = "TEXT";
            rcTable.Criterias.Add(textfield);
        }
        //---------------------------------------------------------------------
        private void RetrieveDataSet()
        {
            // like CBVChart.cs -- RetrieveDS()
            FormDbMgr formDbMgr = this.Form.FormDbMgr;
            COEDataView dv = formDbMgr.SelectedDataView;

            int maxRecs = 200;  // later make this a setting
            int oldBaseTableID = dv.Basetable;
            if (m_lookupBaseTableID != 0)
                dv.Basetable = m_lookupBaseTableID;

            m_dataSet = Pager.GetNRecords(m_rcLookup, dv, maxRecs, 0, false);

            dv.Basetable = oldBaseTableID;
        }
        //---------------------------------------------------------------------
        private void BindDSToCombo()
        {
            DataSource = m_dataSet.Tables[0];
            DisplayMember = "TEXT";
        }
        //---------------------------------------------------------------------
        private void FillCombo()
        {
            if (BoundLookupField() != null)
            {
                if (m_rcLookup == null)
                    GenerateRC();
                if (m_dataSet == null)
                    RetrieveDataSet();
                if (m_dataSet != null && m_dataSet.Tables.Count > 0)
                    BindDSToCombo();
            }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Events
        void CBVLookupCombo_DropDown(object sender, EventArgs e)
        {
            FillCombo();
        }
        #endregion

        #region Properties
        //---------------------------------------------------------------------
        private ChemBioVizForm Form
        {
            get { return this.TopLevelControl as ChemBioVizForm; }  // WRONG
        }
        //---------------------------------------------------------------------
        private String BoundFieldName
        {
            get { return (this.Tag == null) ? "" : CBVUtil.AfterDelimiter(this.Tag.ToString(), '.'); }
        }
        //---------------------------------------------------------------------
        public String SelectedID
        {
            get
            {
                DataRow dataRow = (m_dataSet == null) ? null : CBVUtil.FindRowInDataset(m_dataSet, this.Text, "TEXT");
                return (dataRow == null) ? "" : dataRow["ID"].ToString();
            }
        }
        //---------------------------------------------------------------------
        [BrowsableAttribute(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new object DataSource
        {
            get { return base.DataSource; }
            set { base.DataSource = value; }
        }
        //---------------------------------------------------------------------
        #endregion
    }
    //---------------------------------------------------------------------
    public class CBVSSSOptionsCombo : ComboBox
    {
        #region Variables
        enum SSSComboOptions
        {
            Substructure,
            FullStructure,
            ExactStructure,
            TanimotoSimilarity,
            SearchOptions
        };
        #endregion

        #region Constructors
        public CBVSSSOptionsCombo()
        {
            foreach (SSSComboOptions opt in Enum.GetValues(typeof(SSSComboOptions)))
                this.Items.Add(GetComboString(opt));

            this.SelectedIndexChanged += new EventHandler(CBVSSSOptionsCombo_SelectedIndexChanged);
            this.VisibleChanged += new EventHandler(CBVSSSOptionsCombo_VisibleChanged);
        }
        #endregion

        #region Methods
        public void Reset()
        {
            this.SelectedIndex = (int)GlobalSetting;
        }
        //---------------------------------------------------------------------
        public void Set(int index)
        {
            this.SelectedIndex = index;
        }
        //---------------------------------------------------------------------
        static SSSComboOptions SCToComboSetting(SearchCriteria.StructureCriteria sCrit)
        {
            if (sCrit.FullSearch == SearchCriteria.COEBoolean.Yes)
                return SSSComboOptions.FullStructure;
            else if (sCrit.Similar == SearchCriteria.COEBoolean.Yes)
                return SSSComboOptions.TanimotoSimilarity;
            else if (sCrit.Identity == SearchCriteria.COEBoolean.Yes)
                return SSSComboOptions.ExactStructure;
            else
                return SSSComboOptions.Substructure;
        }
        //---------------------------------------------------------------------
        public static int GetComboSetting(Query q)
        {
            SearchCriteria sc = q.SearchCriteria;
            foreach (SearchCriteria.SearchCriteriaItem scItem in sc.Items)
                //Coverity Bug Fix CID 12933
                if (scItem.Criterium != null && scItem.Criterium is SearchCriteria.StructureCriteria)
                    return (int)SCToComboSetting((SearchCriteria.StructureCriteria)scItem.Criterium);
            return -1;
        }
        //---------------------------------------------------------------------
        String GetComboString(SSSComboOptions opt)
        {
            switch (opt)
            {
                case SSSComboOptions.Substructure: return "Substructure";
                case SSSComboOptions.FullStructure: return "Full Structure";
                case SSSComboOptions.ExactStructure: return "Exact Structure";
                case SSSComboOptions.TanimotoSimilarity: return "Tanimoto Similarity";
                case SSSComboOptions.SearchOptions: return "Search Options...";
            }
            return "";
        }
        #endregion

        #region Events
        void CBVSSSOptionsCombo_VisibleChanged(object sender, EventArgs e)
        {
            if ((sender as CBVSSSOptionsCombo).Visible)
                Reset();
        }
        //---------------------------------------------------------------------
        private void ShowSearchPrefs()
        {
            ChemBioVizForm form = this.TopLevelControl as ChemBioVizForm;
            //Coverity Bug Fix CID 12943 
            if (form != null)
                form.ShowSearchPrefsDialog();
        }
        //---------------------------------------------------------------------
        void CBVSSSOptionsCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            int newIndex = (sender as CBVSSSOptionsCombo).SelectedIndex;
            if (newIndex == (int)SSSComboOptions.SearchOptions)
                ShowSearchPrefs();
            else
                GlobalSetting = (SSSComboOptions)newIndex;  // UGH .. saves all prefs every time combo is changed
        }
        #endregion

        #region Properties
        SSSComboOptions GlobalSetting
        {
            get
            {
                if (SearchOptionsSettings.Default.FullSearch)
                    return SSSComboOptions.FullStructure;

                if (SearchOptionsSettings.Default.Similar)
                    return SSSComboOptions.TanimotoSimilarity;

                if (SearchOptionsSettings.Default.Substructure)
                    return SSSComboOptions.Substructure;

                if (SearchOptionsSettings.Default.Exact)
                    return SSSComboOptions.ExactStructure;

                Debug.Assert(false);
                return SSSComboOptions.Substructure;
            }
            set
            {
                bool bSub = false, bFull = false, bExact = false, bSim = false;
                switch (value)
                {
                    case SSSComboOptions.Substructure: bSub = true; break;
                    case SSSComboOptions.FullStructure: bFull = true; break;
                    case SSSComboOptions.ExactStructure: bExact = true; break;
                    case SSSComboOptions.TanimotoSimilarity: bSim = true; break;
                    case SSSComboOptions.SearchOptions:
                        Debug.Assert(false);
                        break;
                }
                SearchOptionsSettings.Default.Substructure = bSub;
                SearchOptionsSettings.Default.FullSearch = bFull;
                SearchOptionsSettings.Default.Similar = bSim;
                SearchOptionsSettings.Default.Exact = bExact;
                PreferencesHelper.PreferencesHelperInstance.ApplyAllSettings();
            }
        }
        #endregion
    }
    //---------------------------------------------------------------------
    public class CBVUnitsCombo : ComboBox
    {
        // developed for CSBR-132401
        #region Variables
        private String m_boxname;   // associated data box
        #endregion

        #region Constructors
        public CBVUnitsCombo()
        {
            m_boxname = String.Empty;
            this.SelectedIndexChanged += new EventHandler(CBVUnitsCombo_SelectedIndexChanged);
            this.ParentChanged += new EventHandler(CBVUnitsCombo_ParentChanged);
        }
        #endregion

        #region Properties
        public String TargetBox
        {
            get { return m_boxname; }
            set
            {
                // fill combo and select value specified on target box
                m_boxname = value;
                this.Items.Clear();
                this.Enabled = true;
                if (!String.IsNullOrEmpty(m_boxname) && this.Parent != null)
                {
                    Control cBox = this.Parent.Controls[m_boxname];
                    if (cBox != null && cBox is CBVQueryTextBox)
                    {
                        CBVQueryTextBox qBox = cBox as CBVQueryTextBox;
                        if (!String.IsNullOrEmpty(qBox.Units))
                        {
                            String sXml = ChemBioVizForm.GetResourceXmlString();
                            CBVUnitsManager unitsMgr = new CBVUnitsManager(sXml);
                            List<String> valuesList = unitsMgr.GetComboList(qBox.Units);
                            foreach (String s in valuesList)
                            {
                                this.Items.Add(s);
                                if (CBVUtil.Eqstrs(s, qBox.Units))
                                    this.SelectedItem = s;
                            }
                        }
                        else
                        {
                            String s = "unknown units";
                            this.Items.Add(s);
                            this.SelectedItem = s;
                            this.Enabled = false;
                        }
                    }
                }
            }
        }
        #endregion

        #region Methods
        public void Reset()
        {
            TargetBox = m_boxname;
        }
        #endregion

        #region Events
        void CBVUnitsCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        //---------------------------------------------------------------------
        void CBVUnitsCombo_ParentChanged(object sender, EventArgs e)
        {
            CBVUnitsCombo combo = sender as CBVUnitsCombo;
            if (String.IsNullOrEmpty(m_boxname) && combo.Parent.Controls.Count > 1)
            {
                foreach (Control c in combo.Parent.Controls)
                {
                    if (c.TabIndex == (this.TabIndex - 1))
                    {
                        TargetBox = c.Name;
                        break;
                    }
                }
            }
        }
        #endregion
    }
    //---------------------------------------------------------------------
    public class TransparentTrackbar : TrackBar
    {
        #region Constructors
        public TransparentTrackbar()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
        }

        #endregion
    }
    //---------------------------------------------------------------------
    public class CBVBrowseButton : Button
    {
        #region Variables
        public enum FileType { Text, DelimitedText, Structure, Any, SDFile };

        private String m_boxname;
        private bool m_bInsertContents;
        private FileType m_fileType;
        private String m_fileName;
        #endregion

        #region Constructors
        public CBVBrowseButton()
        {
            this.Click += new EventHandler(CBVBrowseButton_Click);
            this.ParentChanged += new EventHandler(CBVBrowseButton_ParentChanged);
            m_fileName = String.Empty;
            m_boxname = String.Empty;
            this.Size = new Size(42, 20);
        }
        #endregion

        #region Properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [BrowsableAttribute(false)]
        public String Filename
        {
            get { return m_fileName; }
            set { m_fileName = value; }
        }
        //---------------------------------------------------------------------
        public String TargetBox
        {
            get { return m_boxname; }
            set { m_boxname = value; }
        }
        //---------------------------------------------------------------------
        public bool InsertContents
        {
            get { return m_bInsertContents; }
            set { m_bInsertContents = value; }
        }
        //---------------------------------------------------------------------
        public FileType FilesOfType
        {
            get { return m_fileType; }
            set { m_fileType = value; }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Events
        void CBVBrowseButton_ParentChanged(object sender, EventArgs e)
        {
            CBVBrowseButton button = sender as CBVBrowseButton;
            if (String.IsNullOrEmpty(m_boxname) && button.Parent.Controls.Count > 1)
            {
                foreach (Control c in button.Parent.Controls)
                {
                    if (c.TabIndex == (this.TabIndex - 1))
                    {
                        m_boxname = c.Name;
                        break;
                    }
                }
            }
            button.Text = "...";
        }
        //---------------------------------------------------------------------
        void CBVBrowseButton_Click(object sender, EventArgs e)
        {
            CBVBrowseButton button = sender as CBVBrowseButton;
            String fname = m_fileName;

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = String.IsNullOrEmpty(fname) ? "" : Path.GetFileName(fname);
            dlg.InitialDirectory = String.IsNullOrEmpty(fname) ? Application.CommonAppDataPath : Path.GetDirectoryName(fname);

            String filter = CBVConstants.ALL_FILE_FILTERS, defExt = ".*";
            switch (this.m_fileType)
            {
                case FileType.DelimitedText: filter = CBVConstants.DELIM_FILE_FILTERS; defExt = ".csv"; break;
                case FileType.Structure: filter = CBVConstants.STRUCT_FILE_FILTERS; defExt = ".cdx"; break;
                case FileType.Text: filter = CBVConstants.TXT_FILE_FILTERS; defExt = ".txt"; break;
                case FileType.SDFile: filter = CBVConstants.SDF_FILE_FILTERS; defExt = ".sdf"; break;
            }
            dlg.Filter = filter;
            dlg.DefaultExt = defExt;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                m_fileName = dlg.FileName;
                if (!String.IsNullOrEmpty(m_boxname))
                {
                    Control cBox = button.Parent.Controls[m_boxname];
                    if (cBox != null)
                    {
                        if (InsertContents)
                            cBox.Text = CBVUtil.FileToString(m_fileName);
                        else
                            cBox.Text = m_fileName;
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        #endregion
    }
    //---------------------------------------------------------------------
    public class CBVFrame : Control
    {
        #region Constructors
        public CBVFrame()
        {
            this.Text = this.Name;
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.Opaque, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.BackColor = Color.Transparent;
        }
        #endregion

        #region Overrides
        //---------------------------------------------------------------------
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rc = this.ClientRectangle;
            rc.Inflate(-2, -2);

            Size textSize = g.MeasureString(this.Text, this.Font).ToSize();
            Rectangle r = new Rectangle(rc.X, rc.Y + textSize.Height / 2,
                                        rc.X + rc.Width - 2, rc.Y + rc.Height - textSize.Height / 2 - 2);
            Point ptText = new Point(r.Location.X + 8, r.Location.Y - textSize.Height / 2);
            Rectangle rText = new Rectangle(ptText, textSize), rOrig = r;
            Rectangle rTextClip = new Rectangle(rText.Location, new Size(rc.Width, rText.Height));

            // erase area behind text
            g.SetClip(rTextClip);
            OnPaintBackground(e);

            // draw text, then exclude its area from rect draw
            g.DrawString(Text, this.Font, new SolidBrush(this.ForeColor), ptText);
            g.ResetClip();
            g.SetClip(rText, System.Drawing.Drawing2D.CombineMode.Exclude);

            // draw box, gray and white
            r.Offset(0, 1);
            r.Inflate(-1, 0);
            g.DrawRectangle(new Pen(Color.White), r);
            g.DrawRectangle(new Pen(Color.LightGray), rOrig);
        }
        //---------------------------------------------------------------------
        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_TRANSPARENT = 0x20;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TRANSPARENT;
                return cp;
            }
        }
        //---------------------------------------------------------------------
        #endregion
    }
    //---------------------------------------------------------------------
    public class CBVBindingNavigator : BindingNavigator
    {
        public CBVBindingNavigator(bool b)
            : base(b)
        {
        }
        protected override void OnRefreshItems()
        {
            // no-op: we refresh these ourselves
            // this is the main fix for CSBR-136866
        }
    }
    //---------------------------------------------------------------------
    [Designer(typeof(BindableControlDesigner))]
    // 4/26/11: disable temporarily until ready for release; re-enabled 7/5/11
    public class CBVTextBox : TextBox
    {
        #region Variables
        private String m_aggreg;
        private ToolTip m_toolTip;
        private String m_tooltipText;
        private String m_childAggregTable;
        #endregion

        #region Constructors
        public CBVTextBox()
        {
            m_aggreg = String.Empty;
            m_childAggregTable = String.Empty;
            m_toolTip = new ToolTip();
        }
        #endregion

        #region Properties
        [TypeConverterAttribute(typeof(StdAggregConverter))]
        public String Aggregate
        {
            get { return m_aggreg; }
            set { m_aggreg = value; }
        }
        //---------------------------------------------------------------------
        public String TooltipText
        {
            get { return m_tooltipText; }
            set { m_tooltipText = value; }
        }
        //---------------------------------------------------------------------
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [BrowsableAttribute(false)]
        public String ChildAggregTable
        {
            get { return m_childAggregTable; }
            set { m_childAggregTable = value; }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Events
        protected override void OnMouseHover(EventArgs e)
        {
            String msg = String.Empty;
            if (ChemBioViz.NET.Properties.Settings.Default.ShowTooltips && !String.IsNullOrEmpty(TooltipText))
            {
                msg = TooltipText;
                msg = CBVUtil.ReplaceCRs(msg);
            }
            m_toolTip.SetToolTip(this, msg);
            base.OnMouseHover(e);
        }
        #endregion

        /// <summary>
        /// String converter to provide list of aggregate functions
        /// </summary>
        public class StdAggregConverter : StringConverter
        {
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                List<String> valuesList = GetAggregNames();
                valuesList.Insert(0, "");
                StandardValuesCollection vals = new StandardValuesCollection(valuesList);
                return vals;
            }
            //---------------------------------------------------------------------
            public static List<String> GetAggregNames()
            {
                String[] strs = { 
                    "avg",
                    "count",
                    "max",
                    "min",
                    "stddev",
                    "stddev_pop",
                    "stddev_samp",
                    "sum",
                    "var_pop",
                    "var_samp",
                    "variance"
                    //"corr", "covar_pop", "covar_samp", "cume_dist", "dense_rank",
                    //"first", "group_id", "grouping", "grouping_id", "last",
                    //"percentile_cont", "percentile_disc", "percent_rank", "rank", "regr_",
                };
                List<String> valuesList = new List<String>(strs);
                return valuesList;
            }
            //---------------------------------------------------------------------
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;    // could require box be bound to child table
            }
            //---------------------------------------------------------------------
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;    // means no values are allowed except those we offer
            }
        }
    }
    //---------------------------------------------------------------------
    /// <summary>
    /// Smart-tag designer for textbox binding to table/field/aggregate
    /// </summary>
    public class BindableControlDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        private DesignerActionListCollection actionLists;
        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (actionLists == null)
                {
                    actionLists = new DesignerActionListCollection();
                    actionLists.Add(new BindableControlActionList(this.Component));
                }
                return actionLists;
            }
        }
    }
    //---------------------------------------------------------------------
    /// <summary>
    /// Smart-tag action list for BindableControlDesigner
    /// </summary>
    public class BindableControlActionList : DesignerActionList
    {
        #region Variables
        private DesignerActionUIService designerActionUISvc = null;
        private FormViewControl m_formViewControl = null;
        private int m_tableID = 0;
        private String m_tableName = String.Empty, m_fieldName = String.Empty, m_aggregName = String.Empty;
        private bool m_bIsChildTable = false;
        private String m_sTag = String.Empty;
        private BindingTagData m_bindingTagData = null;
        #endregion

        #region Constructor
        public BindableControlActionList(IComponent component)
            : base(component)
        {
            this.designerActionUISvc = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
            m_bindingTagData = new BindingTagData();
            Init();
        }
        #endregion

        #region Properties
        public FormViewControl FormViewControl
        {
            get { return m_formViewControl; }
        }
        //---------------------------------------------------------------------
        public int TableID
        {
            get { return m_tableID; }
        }
        //---------------------------------------------------------------------
        [TypeConverterAttribute(typeof(DataSourceConverter))]
        public String TableName
        {
            get { return m_tableName; }
            set
            {
                if (!m_tableName.Equals(value))
                {
                    m_tableName = value;
                    m_fieldName = String.Empty;     // field is invalid when table changes
                    AggregateName = String.Empty;   // aggregate too
                    m_tableID = 0;
                    m_bIsChildTable = false;
                    if (!String.IsNullOrEmpty(m_tableName))
                    {
                        FormDbMgr formDbMgr = FormViewControl.Form.FormDbMgr;
                        m_tableID = formDbMgr.FindDVTableByName(m_tableName).Id;
                        m_bIsChildTable = !CBVUtil.Eqstrs(m_tableName, formDbMgr.TableName);
                    }
                    RecreateTag();
                    designerActionUISvc.Refresh(this.Component);
                }
            }
        }
        //---------------------------------------------------------------------
        [TypeConverterAttribute(typeof(FieldStringConverterEx))]
        public String FieldName
        {
            get { return m_fieldName; }
            set
            {
                if (!m_fieldName.Equals(value))
                {
                    m_fieldName = value;
                    AggregateName = String.Empty;
                    RecreateTag();
                    designerActionUISvc.Refresh(this.Component);
                }
            }
        }
        //---------------------------------------------------------------------
        [TypeConverterAttribute(typeof(CBVTextBox.StdAggregConverter))]
        public String AggregateName
        {
            get
            {
                Control c = this.Component as Control;
                if (c is CBVTextBox)
                    return (c as CBVTextBox).Aggregate;
                else if (c is CBVQueryTextBox)
                    return (c as CBVQueryTextBox).Aggregate;
                return String.Empty;
            }
            set
            {
                Control c = this.Component as Control;
                if (c is CBVTextBox)
                    (c as CBVTextBox).Aggregate = value;
                else if (c is CBVQueryTextBox)
                    (c as CBVQueryTextBox).Aggregate = value;
                RecreateTag();
                designerActionUISvc.Refresh(this.Component);
            }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Methods
        private void Init()
        {
            // if we have a tag, parse to get table/field names
            // otherwise set table name to base table, field name blank
            //Coverity Bug Fix CID 12936 
            Control c = this.Component as Control;
            if (c != null)
            {
                m_formViewControl = c.FindForm() as FormViewControl;
                Debug.Assert(m_formViewControl != null);
                //Coverity fix - CID 12936
                if (m_formViewControl == null)
                    return;
                Object tag = c.Tag;
                m_tableName = m_formViewControl.Form.FormDbMgr.TableName;
                m_bindingTagData.m_bindingProp = "Text";
                if (tag != null)
                {
                    m_sTag = tag.ToString();
                    CBVUtil.ParseTag(m_sTag, ref m_bindingTagData);
                    m_fieldName = m_bindingTagData.m_bindingMember;
                    if (m_fieldName.Contains(":"))
                    {
                        m_bIsChildTable = true;
                        m_tableName = CBVUtil.BeforeDelimiter(m_fieldName, ':');
                        m_fieldName = CBVUtil.AfterDelimiter(m_fieldName, ':');
                    }
                }
                if (!String.IsNullOrEmpty(m_tableName))
                    m_tableID = FormViewControl.Form.FormDbMgr.FindDVTableByName(m_tableName).Id;
            }
        }
        //---------------------------------------------------------------------
        private void RecreateTag()
        {
            String sTable = m_bIsChildTable ? m_tableName : String.Empty;
            Control c = this.Component as Control;
            //Coverity Bug Fix CID 12937 
            if (c != null)
            {
                m_bindingTagData.m_formatStr = m_bindingTagData.m_nullValStr = String.Empty;    // erase format/nullval in new tag
                String sNewTag = ControlSwapperEx.BindingNamesToTag(c, sTable, m_fieldName, m_bindingTagData);

                if (!String.IsNullOrEmpty(sNewTag))
                {
                    c.Tag = sNewTag;
                    this.FormViewControl.Designer.SetDirty();
                    c.DataBindings.Clear();
                }
            }
        }
        //---------------------------------------------------------------------
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Data"));
            //items.Add(new DesignerActionHeaderItem("Style"));

            items.Add(new DesignerActionPropertyItem("TableName", "Data Table", "Data", "Main or child table"));
            items.Add(new DesignerActionPropertyItem("FieldName", "Field", "Data", "Field"));
            items.Add(new DesignerActionPropertyItem("AggregateName", "Aggregate", "Data", "Aggregate function if child table"));

            //items.Add(new DesignerActionMethodItem(this, "ShowProperties", "Properties...", "Style", "Advanced features", true));
            return items;
        }
        //---------------------------------------------------------------------
        public void ShowProperties()
        {
            MessageBox.Show("Coming soon");
        }
        #endregion
    }
    //---------------------------------------------------------------------
    #region FieldConverter
    public class FieldStringConverterEx : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        //---------------------------------------------------------------------
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
        //---------------------------------------------------------------------
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            BindableControlActionList bcal = context.Instance as BindableControlActionList;
            List<String> valuesList = new List<String>();
            if (bcal != null)
                valuesList = bcal.FormViewControl.Form.FormDbMgr.GetFieldList(true, false, false, bcal.TableID, true);    // no blank
            StandardValuesCollection vals = new StandardValuesCollection(valuesList);
            return vals;
        }
        //---------------------------------------------------------------------
    }
    #endregion
}
