using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Runtime.Serialization.Formatters.Binary;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Messaging;

using Infragistics.Win.UltraWinExplorerBar;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.EmbeddableEditors;

using Greatis.FormDesigner;
using FormDBLib;
using ChemControls;
using CBVUtilities;
using CBVControls;
using Utilities;

namespace ChemBioViz.NET
{
    public class ViewControl : Form
    {
        #region Variables
        // base class for ChemBioVizForm-connected views (form view, grid view)
        private ChemBioVizForm m_form;
        #endregion

        #region Properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChemBioVizForm Form
        {
            get { return m_form; }
            set { m_form = value; }
        }
        //---------------------------------------------------------------------
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected FormDbMgr FormDbMgr
        {
            get { return Form.FormDbMgr; }
        }
        //---------------------------------------------------------------------
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected BindingSource BindingSource
        {
            get { return Form.BindingSource; }
        }
        //---------------------------------------------------------------------
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new IButtonControl AcceptButton
        {
            // CSBR-135168-related: do not serialize these buttons
            get { return base.AcceptButton; }
            set { base.AcceptButton = value; }
        }
        //---------------------------------------------------------------------
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new IButtonControl CancelButton
        {
            get { return base.CancelButton; }
            set { base.CancelButton = value; }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Constructors
        public ViewControl(ChemBioVizForm form)
        {
            m_form = form;
            TopLevel = false;
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;
            AutoScroll = true;
        }
        #endregion
    }
    //---------------------------------------------------------------------
    public class GridViewControl : ViewControl
    {
        #region Variables
        // Form-based wrapper around grid control, for use by tab manager
        private ChemDataGrid m_cdGrid;
        #endregion

        #region Properties
        public ChemDataGrid ChemDataGrid
        {
            get { return m_cdGrid; }
            set { m_cdGrid = value; }
        }
        #endregion

        #region Constructors
        public GridViewControl(ChemBioVizForm form)
            : base(form)
        {
        }
        #endregion

    }
    //---------------------------------------------------------------------
    public partial class FormViewControl : ViewControl
    {
        #region Constants
        private const int structureBoxHeight = 100;
        private const int fmlaMolwtBoxHeight = 30;
        private const int standardBoxHeight = 20;

        #endregion

        #region Variables
        // a set of boxes bound to a data source
        private Point m_controlPosition = new Point(20, 50);	// position of top left label for auto-generate
        private ControlSwapperEx m_swapper;			            // for temporary use during editing
        private int m_boxno = 1;

        private Designer m_designer;
        private Treasury m_treasury;
        private ToolboxControlEx m_toolbox;
        private PropertyGrid m_propertyGrid;

        private static DateTime m_dummyTime = new DateTime(5000, 1, 1);

        #endregion

        #region Constructors
        public FormViewControl(ChemBioVizForm form)
            : base(form)
        {
            InitializeComponent();
        }
        //---------------------------------------------------------------------
        public FormViewControl()
            : base(null)
        {
            // default constructor is for control factory
            InitializeComponent();
        }
        #endregion

        #region Methods
        public bool HasContent()
        {
            return this.Controls.Count > 0;
        }
        //---------------------------------------------------------------------
        public void ScrollToOrigin()
        {
            this.SetDisplayRectLocation(0, 0);
            this.AdjustFormScrollbars(true);
        }
        //---------------------------------------------------------------------
        public FormViewControl Clone()
        {
            // used for creating duplicate tab; no longer used in serializing or editing
            // TO DO: should implement ICloneable instead of doing this in a homemade way
            // CHANGE: now used for saving a copy before editing, restored on cancel

            IButtonControl cAccept = this.AcceptButton, cCancel = this.CancelButton;

            FormViewControl newForm = ControlFactory.CloneCtrl(this, true) as FormViewControl;

            newForm.Form = this.Form;
            foreach (Control c in this.Controls)
            {
                Control cClone = ControlFactory.CloneCtrl(c, true);
                if (cClone == null)
                    continue;
                //cClone.Visible = true;    ?? this damages invisible buttons; not sure why it was put here
                newForm.Controls.Add(cClone);
            }

            newForm.AcceptButton = cAccept == null ? null : newForm.Controls[(cAccept as Button).Name] as IButtonControl;
            newForm.CancelButton = cCancel == null ? null : newForm.Controls[(cCancel as Button).Name] as IButtonControl;
            return newForm;

        }

        #region Browsing and paging
        private void RefreshData()
        {
            if (Form.BindingSource != null)
            {
                Exception ex = null;
                BindToDataSource(Form.BindingSource, ref ex);
                if (ex != null)
                    CBVUtil.ReportError(ex);    // new 3/11
            }
        }
        //---------------------------------------------------------------------
        public void Rebind()
        {
            // CSBR-134702: restore binding after unbind
            RefreshData();
        }
        //---------------------------------------------------------------------
        public void Unbind()
        {
            // remove bindings from data controls and subforms; do before saving to file
            // TO DO: it would be more elegant if controls did this themselves via some IBindableControl interface
            foreach (Control c in Controls)
            {
                if (c.DataBindings != null)
                {
                    foreach (Binding b in c.DataBindings)
                    {
                        String sName = b.PropertyName, sMember = b.BindingMemberInfo.BindingMember;
                        String sFormat = b.FormatString, sNullVal = (b.NullValue == null) ? String.Empty : b.NullValue as String;
                        if (!String.IsNullOrEmpty(sName) && !String.IsNullOrEmpty(sMember))
                        {
                            // assign new tag only if we don't already have one
                            if (c.Tag == null || String.IsNullOrEmpty(c.Tag.ToString()))
                            {
                                c.Tag = CBVUtil.MakeTag(sName, sMember, sFormat, sNullVal);
                                break;
                            }
                        }
                    }
                    c.DataBindings.Clear();
                }
            }
        }
        //---------------------------------------------------------------------
        public List<String> GetFieldNames()
        {
            // return binding names attached to boxes; extracted from BindToDataSource
            List<String> names = new List<String>();
            foreach (Control c in Controls)
            {
                if (c is Label || c is BindingNavigator || c is ChemDataGrid || c is CBVDataGridView)
                    continue;
                if (c.Tag != null && c.Tag.ToString().Length > 0)
                {
                    String sBindingFieldFROMTAG = CBVUtil.AfterDelimiter(c.Tag.ToString(), '.');
                    String sBindingFieldFROMBINDG = (c.DataBindings.Count == 0) ? String.Empty :
                                                    c.DataBindings[0].BindingMemberInfo.BindingField;
                    if (!CBVUtil.Eqstrs(sBindingFieldFROMTAG, sBindingFieldFROMBINDG))
                        Debug.WriteLine(String.Format("MISMATCH tag {0} vs binding {1}", sBindingFieldFROMTAG, sBindingFieldFROMBINDG));
                    names.Add(sBindingFieldFROMBINDG);
                }
            }
            return names;
        }
        //---------------------------------------------------------------------
        public List<String> GetDefaultFieldNames()
        {
            // return binding names attached to boxes; extracted from BindToDataSource
            List<String> names = new List<String>();
            foreach (Control c in Controls)
            {
                if (c is Label || c is BindingNavigator)
                    continue;

                else if (c is ChemDataGrid)
                {
                    ChemDataGrid cdg = c as ChemDataGrid;
                    string headerName = cdg.Name;
                    string childTableName = string.Empty;
                    COEDataView.DataViewTable dvTable = this.Form.FormDbMgr.DSTableNameToDVTable(headerName);
                    ColumnsCollection cdgCols = cdg.DisplayLayout.Bands[0].Columns;

                    foreach (UltraGridColumn igCol in cdgCols)
                    {
						// CBOE-303, CBOE-1763, CBOE-1764 changed the name to alias and removed the ":" and placed "."
                        childTableName = dvTable.Alias + '.' + igCol.Header.Caption; 
                        names.Add(childTableName);
                    }
                }
                else if (c.Tag != null && c.Tag.ToString().Length > 0 && !(c is ChemDataGrid))
                {
                    String sBindingFieldFROMTAG = CBVUtil.AfterDelimiter(c.Tag.ToString(), '.');
                    // String dsTableName = String.Concat("Table_", CBVUtil.IntToStr(dvTable.Id));
                    String sBindingFieldFROMBINDG = (c.DataBindings.Count == 0) ? String.Empty :
                                                    c.DataBindings[0].BindingMemberInfo.BindingField;
                    if (!CBVUtil.Eqstrs(sBindingFieldFROMTAG, sBindingFieldFROMBINDG))
                        Debug.WriteLine(String.Format("MISMATCH tag {0} vs binding {1}", sBindingFieldFROMTAG, sBindingFieldFROMBINDG));
                    names.Add(sBindingFieldFROMBINDG);
                }
            }
            return names;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------
        public void CopyFormDataToGridCols(ChemDataGrid cdGrid)
        {
            // CSBR-137043: transfer formbox format info to grid columns
            foreach (Control c in Controls)
            {
                if (c.Tag == null || !c.Tag.ToString().Contains("[")) continue;
                String pdum, field, format, nullval;
                pdum = field = format = nullval = String.Empty;
                CBVUtil.ParseTag(c.Tag.ToString(), ref pdum, ref field, ref format, ref nullval);

                UltraGridColumn col = cdGrid.FindUltraGridColumn(c);   // new 3/11: find by data binding member
                if (col == null)
                    continue;
                if (!String.IsNullOrEmpty(format))
                    col.Format = format;
                if (!String.IsNullOrEmpty(nullval))
                    col.NullText = nullval;
            }
        }
        //---------------------------------------------------------------------
        public void ConnectButtonsToGrid(ChemDataGrid cdGrid, bool bIsSubgrid)
        {
            // go through buttons on form; if any is bound to a column of cdgrid, turn col into buttons (links)
            // this handles rich text fields too
            foreach (Control c in Controls)
            {
                bool bIsRichText = c is RichTextBox && !(c is ChemFormulaBox);
                if (!(c is CBVButton) && !bIsRichText) continue;
                Control button = c;

                if (button.Tag != null && button.Tag.ToString().Contains("."))
                {
                    UltraGridColumn col = cdGrid.FindUltraGridColumn(button);   // new 3/11: find by data binding member
                    if (col != null)
                    {
                        if (c is RichTextBox)
                        {
                            col.Editor = new RichTextEditor();
                            col.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                        }
                        else
                        {
                            col.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.URL;
                            col.Tag = button;
                        }
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        private List<CBVButton> GetButtons()
        {
            List<CBVButton> blist = new List<CBVButton>();
            foreach (Control c in Controls)
            {
                if (c is CBVButton)
                    blist.Add(c as CBVButton);
            }
            return blist;
        }
        //---------------------------------------------------------------------
        public List<String> GetSubformTableNames()
        {
            // return table names attached to subform boxes
            List<String> names = new List<String>();
            foreach (Control c in Controls)
            {
                if ((c is ChemDataGrid || c is CBVDataGridView) && (c.Tag != null && c.Tag.ToString().Length > 0))
                    names.Add(c.Tag.ToString());
            }
            return names;
        }
        //---------------------------------------------------------------------
        public static DataTable GetSubDataTable(String subFieldName, BindingSource bs, FormDbMgr formDbMgr)
        {
            DataTable dataTable = null;
            DataSet dataSet = bs.DataSource as DataSet;
            //Coverity Bug Fix 19018 
            if (dataSet != null)
            {
                String tablename = CBVUtil.BeforeDelimiter(subFieldName, ':');
                String fieldname = CBVUtil.AfterDelimiter(subFieldName, ':');
                if (String.IsNullOrEmpty(tablename) || String.IsNullOrEmpty(fieldname))
                    return null;

                // get the relation from the dataview
                // TO DO: subFieldName should name the dvtable, like "Table_2" not "cs_demo_synonyms"
                COEDataView.DataViewTable dvTable = formDbMgr.FindDVTableByName(tablename);
                if (dvTable == null)
                    return null;
                String dsTableName = String.Concat("Table_", CBVUtil.IntToStr(dvTable.Id));
                dataTable = dataSet.Tables[dsTableName];
            }
            return dataTable;
        }
        //---------------------------------------------------------------------
        private void BindToSubField(String sBindingProp, String subFieldName, BindingSource bs, Control c)
        {
            // like an "auto-link" box: bind a box to a subtable column, with name like "cs_demo_synonyms:synonym"
            // TO DO: this mimics a blob of code from binding to subtables, below; refactor

            DataSet dataSet = bs.DataSource as DataSet;
            DataTable dataTable = GetSubDataTable(subFieldName, bs, FormDbMgr);
            String fieldname = CBVUtil.AfterDelimiter(subFieldName, ':');

            if (dataTable == null)
                return;
            //Coverity Bug Fix CID : 19019 
            if (dataSet == null)
                return;
            DataRelation relation = CBVUtil.FindRelByTables(dataSet.Tables[0], dataTable, dataSet);
            if (relation == null)
                return;

            try
            {
                // create a new binding source for the auto-link box
                BindingSource subBindingSource = new BindingSource();
                subBindingSource.DataSource = bs;
                subBindingSource.DataMember = relation.ToString();
                Binding binding = new Binding(sBindingProp, subBindingSource, fieldname, true);
                c.DataBindings.Add(binding);
            }
            catch (Exception e)
            {
                Debug.WriteLine("BINDING EXCEPTION: " + e.Message); // should show to user
            }
        }
        //---------------------------------------------------------------------
        private int GetSelectedRowForChildTable(String childTableName)
        {
            // find subform connected to given tablename; return active row in grid
            int ans = -1;
            foreach (Control c in Controls)
            {
                if (c is ChemDataGrid)
                {
                    ChemDataGrid cdg = c as ChemDataGrid;
                    if (c.Tag != null && CBVUtil.Eqstrs(c.Tag.ToString(), childTableName))
                    {
                        if (cdg.ActiveRow != null)
                            ans = cdg.ActiveRow.Index;
                        else if (cdg.RowCount > 0)
                            ans = 0;    // return first row if there are rows but no selection
                        break;
                    }
                }
            }
            return ans;
        }
        //---------------------------------------------------------------------
        public void GetSelectedRowsByRels(List<DataRelation> rels)
        {
            // find subform for each relation; attach current selected rownum as extended prop
            foreach (DataRelation rel in rels)
            {
                String childTableName = rel.ChildTable.TableName;
                int selRow = GetSelectedRowForChildTable(childTableName);
                rel.ExtendedProperties["selrow"] = selRow;
            }
        }
        //---------------------------------------------------------------------
        public void SetupSubformButtonCols()
        {
            // for each subform:
            // look for buttons on this formview bound to subform field
            foreach (Control c in Controls)
            {
                if (c is ChemDataGrid)
                    ConnectButtonsToGrid(c as ChemDataGrid, true);
            }
        }
        //---------------------------------------------------------------------
        public void RefreshButtonLabels()
        {
            // refresh text on all buttons in case edited
            foreach (Control c in Controls)
            {
                if (c is CBVButton)
                    (c as CBVButton).Text = c.Text;
                else if (c is CBVFrame)
                {
                    c.Invalidate();
                    c.Update();
                }
            }
        }
        //---------------------------------------------------------------------
        public bool HasPlots()
        {
            foreach (Control c in Controls)
                if (c is CBVChartControl)
                    return true;
            return false;
        }
        //---------------------------------------------------------------------
        public CBVChartControl FindPlot()
        {
            foreach (Control c in Controls)
                if (c is CBVChartControl)
                    return c as CBVChartControl;
            return null;
        }
        //---------------------------------------------------------------------
        public void ReplacePlot(CBVChartControl ctlOld, CBVChartControl ctlNew)
        {
            if (this.Controls.Contains(ctlOld))
                Controls.Remove(ctlOld);
            Controls.Add(ctlNew);
            ctlNew.Rebind(false, false);
        }
        //---------------------------------------------------------------------
        public void UpdatePlots(bool bHiliteOnly)
        {
            // update main plot to show new highlight; update subplots to new data
            foreach (Control c in Controls)
            {
                if (c is CBVChartControl)
                {
                    CBVChartControl chartCtl = c as CBVChartControl;
                    chartCtl.Rebind(bHiliteOnly, false);
                    if (!chartCtl.IsSubformPlot && chartCtl.Selected == true)   // CSBR-128901
                        chartCtl.UpdatePlotStatLine(this.Form.Pager.CurrRow);
                }
            }
        }
        //---------------------------------------------------------------------
        public Rectangle GetFormSpace()
        {
            Rectangle r = Rectangle.Empty;
            foreach (Control c in this.Controls)
            {
                if (r.IsEmpty)
                    r = new Rectangle(c.Location, c.Size);
                else
                    r = Rectangle.Union(r, new Rectangle(c.Location, c.Size));
            }
            return r;
        }
        //---------------------------------------------------------------------
        private static DataRelation FindRelForChild(DataSet dataSet, String childTablename)
        {
            foreach (DataRelation rel in dataSet.Relations)
                if (CBVUtil.Eqstrs(rel.ChildTable.TableName, childTablename))
                    return rel;
            return null;
        }
        //---------------------------------------------------------------------
        public static BindingSource GetSubBindingSourceEx(BindingSource mainBs, String tablename, BindingSource parentBs)
        {
            DataSet dataSet = mainBs.DataSource as DataSet;
            //Coverity Bug Fix CID 12992 
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                DataTable mainTable = dataSet.Tables[0];
                DataRelation relation = FindRelForChild(dataSet, tablename);
                if (relation != null)
                {
                    BindingSource subBindingSource = new BindingSource();
                    subBindingSource.DataSource = (parentBs == null) ? mainBs : parentBs;
                    /*CSBR-163080: Error appears & Entire columns disappear after saving the form tab with only sub-form grids 
                      The issue is raised when the parentBs is not null and datasource is null, Inorder to fix this issue when grand child is present 
                      this condition is required */
                    if (parentBs == null || (parentBs != null && parentBs.DataSource != null)) //fixed 163080
                    {
                        subBindingSource.DataMember = relation.ToString();
                        return subBindingSource;
                    }
                }
            }
            return null;
        }
        //---------------------------------------------------------------------
        public ChemDataGrid FindParentSubform(ChemDataGrid childSubform, DataSet dataSet)
        {
            // find another subform which has the given subform as child, if any
            foreach (Control c in Controls)
            {
                if (c is ChemDataGrid && c != childSubform && c.Tag != null && childSubform.Tag != null)
                {
                    DataRelation relation = CBVUtil.FindRelByTableNames(c.Tag.ToString(), childSubform.Tag.ToString(), dataSet);
                    if (relation != null)
                        return c as ChemDataGrid;
                }
            }
            return null;
        }
        //---------------------------------------------------------------------
        private void AssignSubformBindingSource(BindingSource bsMain, ChemDataGrid c, DataSet dataSet, ref Exception ex)
        {
            // create binding source for subform or grandchild subform

            // must have grid with tag giving table name
            String tablename = (c.Tag == null) ? c.Name : c.Tag.ToString();
            if (String.IsNullOrEmpty(tablename))
            {
                ex = new Exception(String.Format("Subform {0} has no table specified", c.Name));
                return;
            }

            // if subform is grandchild, must have parent subform available
            ChemDataGrid cdgParent = FindParentSubform(c, dataSet);
            bool bIsGC = FormUtil.IsGrandchildSubform(dataSet, c);
            if (cdgParent == null && bIsGC)
            {
                ex = new Exception(String.Format("Parent subform not found for grandchild {0}", c.Name));
                return;
            }

            // get binding source from parent, or create if not yet available
            BindingSource parentBS = (cdgParent == null) ? null : cdgParent.DataSource as BindingSource;
            if (cdgParent != null && parentBS == null)
            {
                AssignSubformBindingSource(bsMain, cdgParent, dataSet, ref ex);
                parentBS = cdgParent.DataSource as BindingSource;
            }

            // create binding source based on appropriate relation
            BindingSource subBS = null;
            try
            {
                subBS = GetSubBindingSourceEx(bsMain, tablename, parentBS);  // might throw or return null 
            }
            catch (Exception e)
            {
                ex = e;
            }
            c.DataSource = subBS;     // this creates the columns to match the data
        }
        //---------------------------------------------------------------------
        private void HandleIncomingNull(object sender, ConvertEventArgs cevent)
        {
            // CSBR-143100: Show a recongnizable dummy time rather than a stale one
            if (cevent.Value is System.DBNull)
            {
                Binding bnd = sender as Binding;
                if (bnd != null)
                {
                    Control control = bnd.Control;
                    Debug.Assert((control is DateTimePicker) || (control is MonthCalendar));
                    if (control is DateTimePicker)
                        cevent.Value = m_dummyTime;
                    else if (control is MonthCalendar)
                    {
                        SelectionRange range = new SelectionRange(m_dummyTime, m_dummyTime);
                        cevent.Value = range;
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        private void HandleOutgoingNull(object sender, ConvertEventArgs cevent)
        {
            // CSBR-143100: Replace dummy time with null value
            // This doesn't seem to be fired for DateTimePicker...
            Debug.Assert((sender as Binding).Control is MonthCalendar);
            SelectionRange range = cevent.Value as SelectionRange;
            //Coverity Bug Fix CID 12991 
            if (range != null && (range.Start == m_dummyTime) && (range.End == m_dummyTime))
                cevent.Value = System.DBNull.Value;
        }
        //---------------------------------------------------------------------
        public void BindToDataSource(BindingSource bs, ref Exception ex)
        {
            if (bs == null) // CSBR-113591
                return;

            // add bindings to data controls and subforms
            DataSet dataSet = bs.DataSource as DataSet;
            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Columns.Count == 0) //Added validation check for DataTable and Columns in the DataTable are not zero.
                return;
            foreach (Control c in Controls)
            {
                try  // CSBR-110426: protect against crash if binding is faulty
                {
                    c.DataBindings.Clear();
                    if (c is Label || c is BindingNavigator)
                    {
                        continue;
                    }
                    else if (c is CBVDataGridView)
                    {
                        Debug.Assert(false);
                    }
                    else if (c is ChemDataGrid)
                    {
                        AssignSubformBindingSource(bs, c as ChemDataGrid, dataSet, ref ex);
                    }
                    else if (c is CBVChartControl)
                    {
                        (c as CBVChartControl).Rebind(false, true);
                    }

                    // tag contains binding info (written by Unbind)
                    // CSBR-137043: tag includes format and null val
                    else if (c.Tag != null && c.Tag.ToString().Length > 0)
                    {
                        String tag = c.Tag.ToString();
                        String sBindingProp = String.Empty, sBindingField = String.Empty;
                        String sFormatStr = String.Empty, sNullValStr = String.Empty;
                        CBVUtil.ParseTag(tag, ref sBindingProp, ref sBindingField, ref sFormatStr, ref sNullValStr);
                        Object oNullVal = String.IsNullOrEmpty(sNullValStr) ? null : sNullValStr;

                        if (!String.IsNullOrEmpty(sBindingField))
                        {

                            String sAggreg = (c is CBVTextBox) ? (c as CBVTextBox).Aggregate : String.Empty;
                            bool bHasAggreg = !String.IsNullOrEmpty(sAggreg);

                            if (!bHasAggreg && Query.IsSubformField(sBindingField))
                            {
                                // bind to subfield (like "Synonyms:synonym")
                                BindToSubField(sBindingProp, sBindingField, bs, c);
                            }
                            else
                            {
                                if (bHasAggreg)
                                {
                                    // bind to a main field we will call <fldname>_<agg>, e.g., POC_AVG
                                    // squirrel away the child table name as a propty of the box
                                    String childTableName = CBVUtil.BeforeDelimiter(sBindingField, ':');
                                    String simpleFieldName = CBVUtil.AfterDelimiter(sBindingField, ':');
                                    if (!String.IsNullOrEmpty(simpleFieldName))
                                    {
                                        sBindingField = String.Concat(simpleFieldName, "_", sAggreg);
                                        if (c is CBVTextBox)
                                            (c as CBVTextBox).ChildAggregTable = childTableName;
                                    }
                                }
                                Binding binding = new Binding(sBindingProp, bs, sBindingField, true,
                                                        DataSourceUpdateMode.OnValidation, oNullVal, sFormatStr);
                                //CSBR-143100: Better handle missing values
                                //TODO: This might be handled more simply:
                                //http://msdn.microsoft.com/en-us/library/y0h25we8(VS.90).aspx
                                if (c is MonthCalendar || c is DateTimePicker)
                                {
                                    binding.Format += new ConvertEventHandler(HandleIncomingNull);
                                    if (c is MonthCalendar)
                                        binding.Parse += new ConvertEventHandler(HandleOutgoingNull);
                                }
                               
                                //Checked binding to fields are present in datasource. If yes then only bind the value.
                                if (IsDatasetContainsColumn(dataSet, sBindingField))
                                {
                                    c.DataBindings.Add(binding);
                                }

                                if (CBVUtil.Eqstrs(sBindingField, "Molweight"))	// hack
                                    binding.FormatString = "N3";
                            }
                            if (c is TextBox)
                                (c as TextBox).ReadOnly = true;     // CSBR-105758
                            else if (c is CBVTextBox)   // TEMPORARY
                                Debug.Assert(false);
                            else if (c is ChemFormulaBox)
                                (c as ChemFormulaBox).ReadOnly = true;
                            else if (c is ChemDraw)
                                (c as ChemDraw).ReadOnly = true;    // CSBR-110287
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("BINDING EXCEPTION: " + e.Message);
                    ex = e;
                }
            }
        }
        /// <summary>
        /// Returns true if column is present in datasource else false.
        /// </summary>
        /// <param name="ds">datasource in which to find the column present or not</param>
        /// <param name="columnName">Column Name which is getting bind to specific control</param>
        /// <returns></returns>
        private bool IsDatasetContainsColumn(DataSet ds, string columnName)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    if (ds.Tables[i].Columns.Contains(columnName))
                    {
                        result = true;
                        break;
                    }
                }
            }
            catch
            {
                throw;
            }
            return result;
        }


        //---------------------------------------------------------------------
        public void CheckForChemDraw()
        {
            foreach (Control c in Controls)
                if (c is ChemDataGrid)
                    (c as ChemDataGrid).CheckForChemDraw();
        }
        //---------------------------------------------------------------------
        #endregion

        #region Auto-generation of form from from dataset
        public void CreateFromDataSourceEx(DataSet dataSet, String tableName, bool withSubforms)
        {
            // for each col in main dataset table: create controls
            // if there are multiple tables, create subforms
            // does no binding

            DataTable dtBase = null;
            foreach (DataTable dataTable in dataSet.Tables)
            {
                if (dtBase == null)
                {
                    dtBase = dataTable;
                    // first table is main table; create boxes for all cols
                    foreach (DataColumn dataCol in dataTable.Columns)
                    {
                        String colName = dataCol.ToString(), friendlyName = String.Empty;
                        bool bIsStructCol = false, bIsFmlaCol = false, bIsMolwtCol = false;
                        PropertyCollection exprops = dataCol.ExtendedProperties;
                        if (exprops.Count > 0)
                        {
                            bIsStructCol = exprops["mimetype"] != null && exprops["mimetype"].ToString() != "";
                            bIsFmlaCol = exprops["Alias"] != null && exprops["Alias"].ToString().Equals("Formula");
                            bIsMolwtCol = exprops["Alias"] != null && exprops["Alias"].ToString().Equals("Molweight");
                            friendlyName = (exprops["Alias"] == null) ? String.Empty : exprops["Alias"].ToString();
                        }
                        Control cNew = null;
                        if (bIsStructCol)
                        {
                            cNew = CreateLabelledBox(colName, 1, structureBoxHeight, 0, friendlyName);
                            m_controlPosition.Y += 105;
                        }
                        else if (bIsFmlaCol)
                        {
                            cNew = CreateLabelledBox(colName, 2, fmlaMolwtBoxHeight, 0, friendlyName);
                            m_controlPosition.Y += 35;
                        }
                        else if (bIsMolwtCol)
                        {
                            cNew = CreateLabelledBox(colName, 0, fmlaMolwtBoxHeight, 0, friendlyName);
                            m_controlPosition.Y += 25;
                        }
                        else
                        {
                            cNew = CreateLabelledBox(colName, 0, standardBoxHeight, 0, friendlyName);
                            m_controlPosition.Y += 25;
                        }
                    }
                }
                else if (dataSet.Relations.Count > 0 && withSubforms)
                {
                    CreateSubformBoxEx(dataTable, ref m_controlPosition);
                }
            }
        }
        //---------------------------------------------------------------------
        private void CreateSubformLabel(String tableName, ref Point controlPosition)
        {
            Label layoutLabel = new Label();
            layoutLabel.Text = tableName;
            layoutLabel.Name = String.Concat("Label", (m_boxno++).ToString());	// must be unique
            layoutLabel.Location = new Point(controlPosition.X, controlPosition.Y);
            layoutLabel.Size = new Size(200, 20);
            this.Controls.Add(layoutLabel);
            controlPosition.Y += layoutLabel.Height + 2;
        }
        //---------------------------------------------------------------------
        public ChemDataGrid CreateSubformBoxEx(DataTable dataTable, ref Point controlPosition)
        {
            // create grid box, do not bind
            // modify supplied position to move below new box
            String tableName = dataTable.TableName;
            String friendlyName = (dataTable.ExtendedProperties.Contains("alias")) ?
                dataTable.ExtendedProperties["alias"].ToString() : tableName;

            bool bWithLabel = true;
            if (bWithLabel)
                CreateSubformLabel(friendlyName, ref controlPosition);

            Size sfboxSize = new Size(350, 100);

            ChemDataGrid cdGrid = new ChemDataGrid();
            cdGrid.Parent = this;
            cdGrid.Visible = true;
            cdGrid.Size = sfboxSize;
            cdGrid.Location = new Point(controlPosition.X, controlPosition.Y);
            cdGrid.Name = tableName;
            cdGrid.Tag = tableName;

            this.Controls.Add(cdGrid);
            Form.InstallGridEvents(cdGrid);
            controlPosition.Y += sfboxSize.Height + 10;
            return cdGrid;
        }
        //---------------------------------------------------------------------
        private int FindNextAvailBoxno()
        {
            int maxBoxNo = 0;
            foreach (Control c in this.Controls)
            {
                String name = c.Name, suffix = String.Empty;
                if (name.StartsWith("Label")) suffix = name.Substring(5);
                else if (name.StartsWith("Box")) suffix = name.Substring(3);
                else continue;
                int val = CBVUtil.StrToInt(suffix);
                if (val > maxBoxNo)
                    maxBoxNo = val;
            }
            return maxBoxNo + 1;
        }
        //---------------------------------------------------------------------
        private Control CreateLabelledBox(string datafield, int type, int height, int indent, string alias)
        {
            // create fixed-size label on left, data box on right
            // type 0=text, 1=struct, 2=fmla, 3=qry
            Label layoutLabel = new Label();
            layoutLabel.Text = String.IsNullOrEmpty(alias) ? datafield : alias;
            layoutLabel.Name = String.Concat("Label", (m_boxno++).ToString());	// must be unique  TO DO: ensure unique among all tabs
            layoutLabel.Location = new Point(m_controlPosition.X, m_controlPosition.Y);
            layoutLabel.Size = new Size(90 + indent, 20);
            this.Controls.Add(layoutLabel);

            Control layoutBox = null;
            if (type == 3 && !this.Form.FeatEnabler.CanCreateQueryTextBox())
                type = 0;
            switch (type)
            {
                case 1: layoutBox = new ChemDraw(); layoutBox.Tag = String.Concat("Base64.", datafield); break;
                case 2: layoutBox = new ChemFormulaBox(); layoutBox.Tag = String.Concat("Text.", datafield); break;
                case 3: layoutBox = new CBVQueryTextBox(); layoutBox.Tag = String.Concat("Text.", datafield); break;
                default: layoutBox = new CBVTextBox(); layoutBox.Tag = String.Concat("Text.", datafield); break;
            }
            layoutBox.Text = string.Empty;
            //layoutBox.Name = datafield;

            layoutBox.Name = String.Concat("Box", (m_boxno++).ToString());  // TO DO: ensure unique among all tabs

            layoutBox.Location = new Point(m_controlPosition.X + 100 + indent, m_controlPosition.Y);
            layoutBox.Size = new Size(250, height);
            layoutBox.BackColor = Color.White;
            this.Controls.Add(layoutBox);
            return layoutBox;
        }
        //---------------------------------------------------------------------
        private bool IsHiddenCol(ChemDataGrid cdg, String colName)
        {
            if (cdg != null && cdg.DisplayLayout.Bands.Count > 0)
            {
                foreach (UltraGridColumn cdgCol in cdg.DisplayLayout.Bands[0].Columns)
                    if (CBVUtil.Eqstrs(cdgCol.Key, colName) && cdgCol.Hidden)
                        return true;
            }
            return false;
        }
        //---------------------------------------------------------------------
        public void CreateQueryBoxesForSubtable(ChemDataGrid cdg, ref int yPosition)
        {
            // given subtable name from cdg tag (like "Table_2379"):
            // create a labelled box for each searchable, non-hidden column
            // tags to be <dvtablename>:<colname> (e.g. "CS_DEMO_SYNS:synonym")
            // note that cdg may be unbound, thus not have columns yet

            if (cdg == null || cdg.Tag == null)
                return;
            String dsTableName = cdg.Tag as String;

            COEDataView dv = this.FormDbMgr.SelectedDataView;
            COEDataView.DataViewTable dvt = this.FormDbMgr.DSTableNameToDVTable(dsTableName);
            if (dvt == null)
                return;

            m_boxno = FindNextAvailBoxno();
            m_controlPosition.Y = yPosition;
            String dvTableName = dvt.Name;
            int indent = 80;
            foreach (COEDataView.Field dvf in dvt.Fields)
            {
                if (IsHiddenCol(cdg, dvf.Name))
                    continue;
                if ((dvf.Visible == false) ||       // CSBR-131778
                    ((dvf.IsDefault == false) &&    // CSBR-153313
                    ChemBioViz.NET.Properties.Settings.Default.UseDefaultFieldsOnly) &&
                    !FormDbMgr.IsLinkingField(dvf) ||               // CSBR-154584
                    (dvf.MimeType != COEDataView.MimeTypes.NONE))   // CSBR-156589
                    continue;
                String fieldTag = String.Format("{0}:{1}", dvTableName, dvf.Name);
                // CSBR-156589: Make structure boxes when called for
                bool bIsStructure = dvf.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE ||
                    FormDbMgr.IsStructureLookupField(dvf, dv);
                int type = bIsStructure ? 1 : 3;
                int height = bIsStructure ? structureBoxHeight : standardBoxHeight;
                String friendlyLabel = (dvt.Alias != null && dvf.Alias != null) ?
                    String.Format("{0}:{1}", dvt.Alias, dvf.Alias) : String.Empty;

                Control cBox = CreateLabelledBox(fieldTag, type, height, indent, friendlyLabel);
                m_controlPosition.Y += cBox.Height + 5;
            }
            yPosition = m_controlPosition.Y;
        }
        //---------------------------------------------------------------------
        #endregion

        #region  FORM EDITING in Greatis FormDesigner
        public Designer Designer
        {
            get { return m_designer; }
        }
        //---------------------------------------------------------------------
        public bool IsEditingForm()
        {
            return m_designer != null && m_designer.Active == true;
        }
        //---------------------------------------------------------------------
        private void AddChildBindingSourcesEx(BindingSource parentSource, ref IComponent[] iComps)
        {
            List<BindingSource> blist = FormDbMgr.GetSubBindingSources(parentSource);

            int nComps = iComps.Length;
            Array.Resize<IComponent>(ref iComps, nComps + blist.Count);
            foreach (BindingSource bs in blist)
            {
                iComps[nComps++] = bs as Component;
            }
        }
        //---------------------------------------------------------------------
        public void BeginFormEdit(ToolboxControlEx toolbox, PropertyGrid propertyGrid, bool bIsQueryMode)
        {
            m_toolbox = toolbox;
            m_propertyGrid = propertyGrid;

            // CSBR-141475: if app is maximized, set form view to match
            if (this.Form.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Maximized;
            Application.DoEvents();

            // note Accept/Cancel buttons -- must restore below after beginning edit
            IButtonControl cAccept = this.AcceptButton, cCancel = this.CancelButton;

            // replace controls unsafe to edit (chemdraw, infra grid)
            m_swapper = new ControlSwapperEx(this);
            m_swapper.PrepForEdit();

            // create Greatis designer
            m_designer = new Designer();
            m_treasury = new Treasury();
            m_designer.FormTreasury = m_treasury;
            m_designer.DesignedForm = this;
            m_designer.ShowErrorMessage = true;
            m_designer.ObjectBoundSmartTagAutoShow = false;
            Application.DoEvents();

            // add binding source as component
            // use full binding source of entire dataview
            // first make sure it matches current dataview
            BindingSource BSource = (Form == null) ? null : Form.FullBindingSource;
            if (BSource != null)
            {
                Component iComp = BSource as Component;
                if (iComp != null)
                {
                    IComponent[] iComps = new IComponent[1];
                    iComps[0] = iComp;
                    AddChildBindingSourcesEx(BSource, ref iComps);
                    m_designer.DesignedComponents = iComps;
                }
            }

            // further edit prep
            propertyGrid.SelectedGridItemChanged += new SelectedGridItemChangedEventHandler(propertyGrid_SelectedGridItemChanged);
            propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid_PropertyValueChanged);
            toolbox.Designer = m_designer;
            propertyGrid.Site = new PropertyGridSite(m_designer.DesignerHost as IServiceProvider);
            ISelectionService ss = (ISelectionService)m_designer.DesignerHost.GetService(typeof(ISelectionService));
            ss.SelectionChanged += new EventHandler(SelectionChanged);
            m_designer.KeyDown += new KeyEventHandler(DesignedFormEdit_KeyDown);

            IDesignEvents de = (IDesignEvents)m_designer.DesignerHost.GetService(typeof(IDesignEvents));
            if (de != null)
                de.FilterProperties += new FilterEventHandler(de_FilterProperties);

            // hand form to Greatis
            bSkipFiltering = false;
            m_designer.Active = true;

            // change component site names to friendly table names, assigned in PrepDataSet
            int tableNo = 0;
            string aliasString = string.Empty;
            if (BSource != null)    // prevents crash but also prevents binding to anything within form editor
            {
                DataSet dataSet = BSource.DataSource as DataSet;
                foreach (IComponent icomp in m_designer.DesignedComponents)
                {
                    if (icomp is BindingSource)
                    {
                        try
                        {
                            DataTable dataTable = dataSet.Tables[tableNo++];
                            if (dataTable.ExtendedProperties.Contains("alias"))
                            {
                                aliasString = dataTable.ExtendedProperties["alias"].ToString();
                                icomp.Site.Name = aliasString;

                            }
                            else
                                icomp.Site.Name = dataTable.TableName;
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("ERROR ASSIGNING COMP SITE NAME : " + e.Message);
                        }
                    }
                }
            }
            // new 11/09: expand prop grid sections based on settings
            m_propertyGrid.CollapseAllGridItems();
            SaveRestoreExpansionStates(false);

            (m_designer.DesignedForm as FormViewControl).AcceptButton = cAccept;
            (m_designer.DesignedForm as FormViewControl).CancelButton = cCancel;

            m_designer.ClearDirty();
            ComponentTray tray = m_designer.DesignerHost.GetService(typeof(ComponentTray)) as ComponentTray;
            if (tray != null)
                tray.Hide();

            // select full form at start
            m_propertyGrid.SelectedObject = m_designer.DesignedForm;
            Application.DoEvents();
        }
        //---------------------------------------------------------------------
        private static bool bSkipFiltering = false;

        private List<String> GetCategoryExclusionList()
        {
            List<String> list = new List<string>();
            list.Add("Accessibility");
            list.Add("Focus");
            return list;
        }
        //---------------------------------------------------------------------
        private List<String> GetItemExclusionList(String category)
        {
            List<String> list = new List<string>();
            if (category.Equals("Data"))
            {
                ; //  list.Add("Tag");
            }
            else if (category.Equals("Behavior"))
            {
                list.Add("AllowDrop");
                list.Add("ContextMenuStrip");
                list.Add("ImeMode");
                list.Add("PasswordChar");
                list.Add("UseSystemPasswordChar");
                list.Add("AutoValidate");
                list.Add("DoubleBuffered");
            }
            else if (category.Equals("Appearance"))
            {
                list.Add("Cursor");
                list.Add("Lines");
                list.Add("UseWaitCursor");
            }
            else if (category.Equals("Misc"))
            {
                list.Add("AutoCompleteCustomSource");
                list.Add("AutoCompleteMode");
                list.Add("AutoCompleteSource");

                if (!Form.FeatEnabler.CanUseNumericUnits())
                    list.Add("Units");
            }
            return list;
        }
        //---------------------------------------------------------------------
        void de_FilterProperties(IComponent component, ref FilterEventArgs args)
        {
            if (bSkipFiltering)
                return;

            Array elements = Array.CreateInstance(typeof(DictionaryEntry), args.data.Count);
            args.data.CopyTo(elements, 0);
            args.caching = false;

            List<String> excludeCats = GetCategoryExclusionList();
            foreach (DictionaryEntry entry in elements)
            {
                PropertyDescriptor desc = entry.Value as PropertyDescriptor;
                if (desc == null) continue;

                if (excludeCats.Contains(desc.Category))
                    args.data.Remove(entry.Key);
                else
                {
                    // TO DO: we should only do this once per category
                    List<String> excludeItems = GetItemExclusionList(desc.Category);
                    if (excludeItems.Contains(entry.Key as String))
                        args.data.Remove(entry.Key);
                }
            }
        }
        //---------------------------------------------------------------------
        public static void DebugScrollInfo(String caption, ScrollableControl scrControl)
        {
            Debug.WriteLine(caption);
            Point aScrollPos = scrControl.AutoScrollPosition;
            Debug.WriteLine(String.Concat("ScrollPos  ", aScrollPos.ToString()));
            Debug.WriteLine(String.Concat("AutoScrollMinSize  ", scrControl.AutoScrollMinSize.ToString()));
            Debug.WriteLine(String.Concat("this size  ", scrControl.Size.ToString()));
            Debug.WriteLine(String.Concat("DisplayRectangle  ", scrControl.DisplayRectangle.ToString()));
            Debug.WriteLine(String.Concat("Hscroll val  ", scrControl.HorizontalScroll.Value.ToString(),
                                        "  Vscroll val  ", scrControl.VerticalScroll.Value.ToString()));
            foreach (Control c in scrControl.Controls)
                Debug.WriteLine(String.Concat(c.Name, " loc ", c.Location.ToString()));

            Debug.WriteLine("----------------------------");
        }
        //---------------------------------------------------------------------
        public void CancelFormEdit()
        {
            bSkipFiltering = true;
            if (m_designer != null)
                m_designer.Active = false;
        }
        //---------------------------------------------------------------------
        private void SaveRestoreExpansionStates(bool bSave)
        {
            // a settings string saves which props grid categories are expanded (like "Behavior,Appearance")
            // this routine creates the string or reads it and does the expansions
            String sStates = bSave ? String.Empty : Properties.Settings.Default.GridsExpanded;

            GridItem groot = m_propertyGrid.SelectedGridItem;
            if (groot == null)
                return;
            while (groot.Parent != null)
                groot = groot.Parent;
            if (groot != null)
            {
                foreach (GridItem g in groot.GridItems)
                {
                    if (bSave && g.Expanded)
                    {
                        if (!String.IsNullOrEmpty(sStates)) sStates += ",";
                        sStates += g.Label;
                    }
                    else if (!bSave)
                    {
                        if (sStates.Contains(g.Label))
                            g.Expanded = true;
                    }
                }
                if (bSave)
                    Properties.Settings.Default.GridsExpanded = sStates;
            }
        }
        //---------------------------------------------------------------------
        private bool ValidateData(Control root)
        {
            foreach (Control c in root.Controls)
            {
                if (c is CBVChartControl &&
                        !CBVChartPanel.ValidateChartControlData(c as CBVChartControl))
                    return false;
            }
            return true;
        }
        //---------------------------------------------------------------------
        public bool EndFormEdit(bool bIsQueryMode)
        {
            // before deactivating, grab binding info; scroll to origin
            // return value always true, should be disregarded

            if (m_designer == null)
                return true;     // happens if attempting to end editing without having started it

            bool bWasModified = m_designer.IsDirty;
            Control root = m_designer.DesignerHost.RootComponent as Control;

            if (bWasModified && root != null && !ValidateData(root))
                return false;
            //Coverity Bug Fix CID 12988 
            ScrollableControl obj = root as ScrollableControl;
            if (obj != null)
                (obj).AutoScrollPosition = new Point(0, 0);

            if (bWasModified)   // CSBR-115617 -- don't mess with bindings if form unchanged
                ControlSwapperEx.BindingsToTags(root, this);

            SaveRestoreExpansionStates(true);

            bSkipFiltering = true;
            m_designer.Active = false;
            m_swapper.RestoreAfterEdit();
            this.WindowState = FormWindowState.Normal;

            // CSBR-144053: clear/cancel/search buttons must come first in parent control list
            // CAUTION: the 3 here will be wrong if the number of buttons changes!
            this.Parent.Controls.SetChildIndex(this, 3);

            // if we added or deleted boxes, we need a new RC and bindingSource and dataset before binding
            if (bWasModified)   // hasty fix for CSBR-110294
            {
                bool bOnlyIfFieldsAdded = true;    // or if aggregate fxn changed
                try
                {
                    int currRow = (this.Form.Pager == null) ? 0 : this.Form.Pager.CurrRow;
                    this.Form.RecreateBindingSource(bOnlyIfFieldsAdded, currRow);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error recreating binding source: " + ex.Message);
                }
            }
            SetupSubformButtonCols();
            RefreshButtonLabels();

            if (!bIsQueryMode)
                RefreshData();

            return true;
        }
        //---------------------------------------------------------------------
        private void propertyGrid_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
        }
        //---------------------------------------------------------------------
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (s is PropertyGrid)
            {
                Control controlToSelect = this.Form.TabManager.CurrentTab.Control;
                if ((s as PropertyGrid).SelectedObject == null)
                    (s as PropertyGrid).SelectedObject = controlToSelect; //  this;  // CSBR-105934: prevent grid from going blank
            }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Property grid
        internal class PropertyGridSite : System.ComponentModel.ISite
        {
            private IServiceProvider sp;
            public PropertyGridSite(IServiceProvider sp)
            {
                this.sp = sp;
            }
            #region Implementation of ISite

            public System.ComponentModel.IComponent Component
            {
                get { return null; }
            }
            public System.ComponentModel.IContainer Container
            {
                get { return null; }
            }
            public bool DesignMode
            {
                get { return false; }
            }
            public string Name
            {
                get { return null; }
                set { }
            }
            #endregion

            #region Implementation of IServiceProvider
            public object GetService(Type serviceType)
            {
                if (sp != null)
                    return sp.GetService(serviceType);
                return null;
            }
            #endregion
        }
        //---------------------------------------------------------------------
        private void SelectionChanged(object sender, EventArgs e)
        {
            int selectionCount = ((ISelectionService)sender).SelectionCount;
            if (selectionCount == 0)
            {
                // select entire form on click outside a control
                // CSBR-142224: do NOT select form; fails under test conditions (window maximized, XP)
                // CSBR-142224 revert: fix for CSBR-141475 eliminates the need to avoid this
                m_propertyGrid.SelectedObject = m_designer.DesignedForm;
            }
            else if (selectionCount > 0)
            {
                object[] selected = new object[selectionCount];
                ((ISelectionService)sender).GetSelectedComponents().CopyTo(selected, 0);
                m_propertyGrid.SelectedObjects = selected;
            }
        }
        //---------------------------------------------------------------------
        private void DesignedFormEdit_KeyDown(object sender, KeyEventArgs e)
        {
            // this should possibly belong to FormEditView
            if (!IsEditingForm())
                return;

            try
            {
                if (e.KeyCode == Keys.Delete)
                    m_designer.DeleteSelected();

                if (e.Control == true)
                {
                    // these calls are also made in ChemBioVizForm: DoEditCommand
                    switch (e.KeyCode)
                    {
                        case Keys.C: m_designer.CopyControlsToClipboard(); break;
                        case Keys.X: m_designer.CopyControlsToClipboard(); m_designer.DeleteSelected(); break;
                        case Keys.V: m_designer.PasteControlsFromClipboard(); break;
                        case Keys.Z: m_designer.Undo(); break;
                        case Keys.Y: m_designer.Redo(); break;
                    }
                }
            }
            catch (Exception ex)
            {
                CBVUtil.ReportError(ex);
            }
        }
        #endregion

        #endregion
    }
    //---------------------------------------------------------------------
    #region Type converter for showing list of queries in prop grid
    public class QueryListConverter : TypeConverter
    {
        public override bool
        GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool
        GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true; // means no values are allowed except those we offer
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            FormViewControl formview = context.Instance as FormViewControl;
            String[] strings = new String[0];
            //Coverity Bug Fix CID 12989 
            if (formview != null && formview.Form != null)
            {
                QueryCollection queries = formview.Form.QueryCollection;
                if (queries != null)
                {
                    strings = new String[queries.Count];

                    int i = 0;
                    foreach (Query q in queries)
                        strings[i++] = q.Name;
                }
            }
            return new StandardValuesCollection(strings);
        }
    }
    #endregion
}
