using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Infragistics.Win.UltraWinGrid;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.COEConfigurationService;

using ChemBioViz.NET;
using ChemControls;
using FormDBLib;
using CBVUtilities;
using CBVControls;

namespace Utilities
{
    public class FormUtil
    {
        #region Methods
        #region ResultsCriteria Helpers
        public static ResultsCriteria FormToResultsCriteria(ChemBioVizForm form)
        {
            // convert boxes and subforms of all tabs of form into an RC for retrieving data
            // get base table from app/table names owned by form
            // CSBR-111677: add columns of grid and boxes of query tab too

            // new 2/10: assume we already have a selected dview and table
            COEDataView.DataViewTable t = form.FormDbMgr.SelectedDataViewTable;

            // create rc and main table
            ResultsCriteria rc = new ResultsCriteria();
            ResultsCriteria.ResultsCriteriaTable rcTable = new ResultsCriteria.ResultsCriteriaTable();
            if (t != null)
            {
                rcTable.Id = t.Id;
                rc.Tables.Add(rcTable);

                // loop tabs of form
                for (int i = 0; i < form.TabManager.Count; ++i)
                {
                    FormTab tab = form.TabManager.GetTab(i);
                    if (tab.IsGridView())
                    {
                        ChemDataGrid gridview = tab.Control as ChemDataGrid;
                        //Coverity Bug Fix CID 13021 
                        if (gridview != null)
                            GridViewToRC(gridview, rc, rcTable, form.FormDbMgr.SelectedDataView, t);
                    }
                    else if (tab.IsFormView() || tab.IsQueryView())
                    {
                        FormViewControl formview = tab.Control as FormViewControl;
                        //Coverity Bug Fix CID 13021 
                        if (formview != null)
                            FormViewToRC(formview, rc, rcTable, form.FormDbMgr.SelectedDataView, t);
                    }
                }
            }
            else
            {
                throw new FormDBLib.Exceptions.ObjectBankException(string.Concat("Source dataview",
                    " is broken or does not exist on the data base. \n", form.FormName, " form could not be opened"));
            }
            return rc;
        }
        //---------------------------------------------------------------------
        public static ResultsCriteria FieldListToRC(FormDbMgr formDbMgr, List<String> fieldNames, bool bUseStructHilites)
        {
            // for export: convert list of names (including child flds table:field) to rc for retrieval of data
            COEDataView dataview = formDbMgr.SelectedDataView;
            COEDataView.DataViewTable dvTable = formDbMgr.SelectedDataViewTable;
            ResultsCriteria rcInput = formDbMgr.ResultsCriteria;
            if (rcInput.Tables.Count == 0)
                return null;

            // create new RC with same table as existing
            ResultsCriteria rcOut = new ResultsCriteria();
            // CSBR-144005/153260: Make a new table object rather than
            //  mucking with the existing one
            ResultsCriteria.ResultsCriteriaTable rcMainTable = new ResultsCriteria.ResultsCriteriaTable((rcInput.Tables[0].Id));
            rcOut.Add(rcMainTable);

            List<String> tableNames = new List<String>();
            foreach (String fieldName in fieldNames)
            {
				// CBOE-303, CBOE-1763, CBOE-1764 removed the ":" and placed "."
                if (fieldName.Contains("."))
                    SingleSubFieldToRC(formDbMgr, rcOut, fieldName, dvTable, rcMainTable, String.Empty);
                else
                    FieldToRC(fieldName, dataview, dvTable, rcMainTable, bUseStructHilites);
            }
            return rcOut;
        }
        //---------------------------------------------------------------------
        public static ResultsCriteria MakeSingleFieldRC(String fieldName, COEDataView dataview, COEDataView.DataViewTable dvTable)
        {
            ResultsCriteria rcOut = new ResultsCriteria();
            ResultsCriteria.ResultsCriteriaTable rcTable = new ResultsCriteria.ResultsCriteriaTable();
            rcTable.Id = dvTable.Id;
            rcOut.Add(rcTable);
            FieldToRC(fieldName, dataview, dvTable, rcTable);
            return rcOut;
        }
        //---------------------------------------------------------------------
        public static ResultsCriteria RemoveChildTables(ResultsCriteria rcInput)
        {
            ResultsCriteria rcOut = new ResultsCriteria();
            if (rcInput.Tables.Count > 0)
            {
                ResultsCriteria.ResultsCriteriaTable rcMainTable = rcInput.Tables[0];
                rcOut.Add(rcMainTable);
            }
            return rcOut;
        }
        //---------------------------------------------------------------------
        public static bool IsGrandchildSubform(DataSet dataSet, ChemDataGrid cdg)
        {
            String childTable = (cdg.Tag == null) ? String.Empty : cdg.Tag.ToString();
            return String.IsNullOrEmpty(childTable) ? false : CBVUtil.IsGrandchildTable(dataSet, childTable);
        }
        //---------------------------------------------------------------------
        public static String ParseAggreg(ref String sField)
        {
            // if tag ends with /<aggreg>, pluck it off and return the aggreg
            // NOT USED
            Debug.Assert(false);

            String s = String.Empty;
            if (sField.Contains("/"))
            {
                int iPos = sField.IndexOf("/");
                Debug.Assert(iPos > 0);
                s = sField.Substring(iPos + 1);
                sField = sField.Substring(0, iPos);
            }
            return s;
        }
        //---------------------------------------------------------------------
        public static void FormViewToRC(FormViewControl formview, ResultsCriteria rc,
                                       ResultsCriteria.ResultsCriteriaTable rcTable, COEDataView d, COEDataView.DataViewTable t)
        {
            // convert boxes of formview to fields in resultscriteria
            // code adapted from FormViewControl.BindToDataSource
            // do two loops to get the aggregates at the end (they don't work otherwise)
            for (int loop = 0; loop <= 1; ++loop)
            {
                bool bDoAggregates = loop == 1;

                foreach (Control c in formview.Controls)
                {
                    bool bHasAggreg = (c is CBVTextBox && !String.IsNullOrEmpty((c as CBVTextBox).Aggregate));
                    if (bHasAggreg != bDoAggregates)
                        continue;

                    if (c is Label || c is BindingNavigator || c is CBVDataGridView)
                        continue;

                    // subform: create a new table and add to rc
                    if (c is ChemDataGrid)
                    {
                        // CSBR-135838: rc might have this subtable already, but not all its fields
                        ResultsCriteria.ResultsCriteriaTable rcSubTable = RCGetGridSubtable(c as ChemDataGrid, rc);
                        GridToRC(c as ChemDataGrid, t, formview, rc, rcSubTable);   // adds table only if rcSubTable is null
                    }
                    else if (c is CBVChartControl)
                    {
                        if ((c as CBVChartControl).IsSubformPlot)
                        {
                            if (RCHasSubplot(c as CBVChartControl, rc))
                                continue;
                            SubplotToRC(c as CBVChartControl, t, formview, rc);
                        }
                    }
                    // normal data box: tag contains binding info
                    else if (c.Tag != null && c.Tag.ToString().Length > 0)
                    {
                        String tag = c.Tag.ToString();
                        String sBindingField = CBVUtil.AfterDelimiter(tag, '.');

                        // CSBR-135003: sBindingField might be table:field (as in auto-link box)
                        if (Query.IsSubformField(sBindingField))
                        {
                            String sAggreg = (c is CBVTextBox) ? (c as CBVTextBox).Aggregate : String.Empty;
                            SingleSubFieldToRC(formview.Form.FormDbMgr, rc, sBindingField, t, rcTable, sAggreg);
                        }
                        else
                        {
                            // prevents adding same field more than once
                            // TO DO: remove this! so we can have more than one grid col on same field
                            FieldToRC(sBindingField, d, t, rcTable);   // does nothing if duplicate
                        }
                    }
                    if (c is CBVButton)
                    {
                        // CSBR-128735: bind any fields mentioned in button args
                        CBVButton cbvButton = c as CBVButton;
                        List<String> fieldNames = cbvButton.GetArgFieldNames();

                        // except: for LaunchEmbedded buttons, do not bind blob field
                        if (cbvButton.Action == CBVButton.ActionType.LaunchEmbedded && fieldNames.Count > 0)
                            fieldNames.RemoveAt(0);

                        foreach (String sField in fieldNames)
                        {
                            // CSBR-139779: handle subform fields
                            if (Query.IsSubformField(sField))
                            {
                                String sAggreg = (c is CBVTextBox) ? (c as CBVTextBox).Aggregate : String.Empty;
                                SingleSubFieldToRC(formview.Form.FormDbMgr, rc, sField, t, rcTable, sAggreg);
                            }
                            else
                            {
                                FieldToRC(sField, d, t, rcTable);
                            }
                        }
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        private static ResultsCriteria.ResultsCriteriaTable RCGetTag(object tag, ResultsCriteria rc)
        {
            // if RC has a subtable matching this tag, return it
            if (tag != null)
            {
                String subTableName = tag.ToString(); // like "Table_22"
                int subTableId = CBVUtil.StrToInt(CBVUtil.AfterDelimiter(subTableName, '_'));
                if (subTableId > 0)
                {
                    foreach (ResultsCriteria.ResultsCriteriaTable subTable in rc.Tables)
                    {
                        if (subTable.Id == subTableId)
                            return subTable;
                    }
                }
            }
            return null;
        }
        //---------------------------------------------------------------------
        private static bool RCHasTag(object tag, ResultsCriteria rc)
        {
            // true if RC has a subtable for this grid
            ResultsCriteria.ResultsCriteriaTable subTable = RCGetTag(tag, rc);
            return subTable != null;
        }
        //---------------------------------------------------------------------
        private static bool RCHasSubplot(CBVChartControl iplot, ResultsCriteria rc)
        {
            return RCHasTag(iplot.Tag, rc);
        }
        //---------------------------------------------------------------------
        private static bool RCHasGrid(ChemDataGrid igrid, ResultsCriteria rc)
        {
            return RCHasTag(igrid.Tag, rc);
        }
        //---------------------------------------------------------------------
        private static ResultsCriteria.ResultsCriteriaTable RCGetGridSubtable(ChemDataGrid igrid, ResultsCriteria rc)
        {
            return RCGetTag(igrid.Tag, rc);
        }
        //---------------------------------------------------------------------
        private static ResultsCriteria.ResultsCriteriaTable RCFindTable(ResultsCriteria rc, int tableID)
        {
            // return table with the given id
            foreach (ResultsCriteria.ResultsCriteriaTable table in rc.Tables)
                if (table.Id == tableID) return table;
            return null;
        }
        //---------------------------------------------------------------------
        private static bool AreDifferentAggregateFxns(ResultsCriteria.IResultsCriteriaBase rcbOld,
            ResultsCriteria.IResultsCriteriaBase rcbNew)
        {
            if (rcbOld is ResultsCriteria.AggregateFunction && rcbNew is ResultsCriteria.AggregateFunction)
            {
                String fxnOld = (rcbOld as ResultsCriteria.AggregateFunction).FunctionName;
                String fxnNew = (rcbNew as ResultsCriteria.AggregateFunction).FunctionName;
                return !fxnOld.Equals(fxnNew);
            }
            return false;
        }
        //---------------------------------------------------------------------
        private static int GetAggregFieldID(ResultsCriteria.AggregateFunction aggFxn)
        {
            int fldId = 0;
            if (aggFxn.Parameters.Count > 0)
            {
                ResultsCriteria.IResultsCriteriaBase rcbase = aggFxn.Parameters[0];
                if (rcbase is ResultsCriteria.Field)
                    fldId = (rcbase as ResultsCriteria.Field).Id;
            }
            return fldId;
        }
        //---------------------------------------------------------------------
        private static bool AliasMismatch(String alias1, String alias2)
        {
            if (String.IsNullOrEmpty(alias1) != String.IsNullOrEmpty(alias2)) return true;
            if (CBVUtil.Eqstrs(alias1, alias2)) return false;
            if (CBVUtil.Eqstrs(alias1, "Formula")) return false;
            if (CBVUtil.Eqstrs(alias1, "Molweight")) return false;
            return true;
        }
        //---------------------------------------------------------------------
        private static bool RCTableHasCriterium(ResultsCriteria.ResultsCriteriaTable rcTable,
                                                ResultsCriteria.IResultsCriteriaBase rcbQuery)
        {
            // true if table has a field matching the id of the given criterium
            int rcbid = (rcbQuery is ResultsCriteria.Formula) ? (rcbQuery as ResultsCriteria.Formula).Id :
                        (rcbQuery is ResultsCriteria.MolWeight) ? (rcbQuery as ResultsCriteria.MolWeight).Id :
                        (rcbQuery is ResultsCriteria.Field) ? (rcbQuery as ResultsCriteria.Field).Id :
                        (rcbQuery is ResultsCriteria.AggregateFunction) ? GetAggregFieldID(rcbQuery as ResultsCriteria.AggregateFunction) : 0;

            foreach (ResultsCriteria.IResultsCriteriaBase rcbase in rcTable.Criterias)
            {
                int rcid = (rcbase is ResultsCriteria.Formula) ? (rcbase as ResultsCriteria.Formula).Id :
                            (rcbase is ResultsCriteria.MolWeight) ? (rcbase as ResultsCriteria.MolWeight).Id :
                            (rcbase is ResultsCriteria.Field) ? (rcbase as ResultsCriteria.Field).Id :
                            (rcbase is ResultsCriteria.AggregateFunction) ? GetAggregFieldID(rcbase as ResultsCriteria.AggregateFunction) : 0;
                if (rcid == rcbid)
                {
                    if (AliasMismatch(rcbase.Alias, rcbQuery.Alias))
                    {
                        Debug.WriteLine(String.Format("ids match but aliases not: old={0} new={1}", rcbase.Alias, rcbQuery.Alias));
                        continue;
                    }
                    if (rcbase is ResultsCriteria.AggregateFunction && AreDifferentAggregateFxns(rcbase, rcbQuery))
                    {
                        Debug.WriteLine(String.Format("ids match but aggreg fxns not: old={0} new={1}", rcbid, rcid));
                        continue;
                    }
                    return true;
                }
            }
            Debug.WriteLine(String.Format("old table does NOT have id {0}", rcbid));
            return false;
        }
        //---------------------------------------------------------------------
        public static void GridViewToRC(ChemDataGrid cdgrid, ResultsCriteria rc,
                                      ResultsCriteria.ResultsCriteriaTable rcTable, COEDataView d, COEDataView.DataViewTable t)
        {
            // loop columns of grid, converting to fields in resultscriteria
            // this is for main grid, not subform

            // if we have a source wingrid, get cols from that
            CBVDataGridView wgrid = cdgrid.SourceWinGrid as CBVDataGridView;
            if (wgrid != null)
            {
                // stored form has win datagridview; create field for each column
                // TO DO: drop those marked hidden?
                DataGridViewColumnCollection dgvCols = wgrid.Columns;
                foreach (DataGridViewColumn dgCol in dgvCols)
                {
                    FieldToRC(dgCol.DataPropertyName, d, t, rcTable);
                }
            }
            // TO DO: handle case where no wgrid is available
            // TO DO: child tables
        }
        //---------------------------------------------------------------------
        private static COEDataView.DataViewTable TagToDVTable(object tag, FormViewControl formview)
        {
            if (tag == null)
                return null;

            // find subtable in dataview; id is encoded in the control tag
            String subTableName = tag.ToString(); // like "Table_22"
            int subTableId = CBVUtil.StrToInt(CBVUtil.AfterDelimiter(subTableName, '_'));
            COEDataView dataview = formview.Form.FormDbMgr.SelectedDataView;
            COEDataView.DataViewTable tSub = null;
            if (dataview != null)
                tSub = FormDbMgr.FindDVTableByID(subTableId, dataview);
            else
            {
                throw new FormDBLib.Exceptions.ObjectBankException("Source dataview is missing. \n");
            }
            return tSub;
        }
        //---------------------------------------------------------------------
        private static void SubplotToRC(CBVChartControl iplot, COEDataView.DataViewTable t,
                                   FormViewControl formview, ResultsCriteria rc)
        {
            String subtableName = String.IsNullOrEmpty(iplot.XTable) ? iplot.YTable : iplot.XTable;
            COEDataView.DataViewTable tSub = formview.Form.FormDbMgr.FindDVTableByName(subtableName);

            if (tSub == null)
                return;

            // create new table in rc
            ResultsCriteria.ResultsCriteriaTable subTable = new ResultsCriteria.ResultsCriteriaTable();
            subTable.Id = tSub.Id;
            rc.Tables.Add(subTable);

            // TEMPORARY: add all fields of subtable; later just do the ones we are plotting
            foreach (COEDataView.Field f in tSub.Fields)
            {
                if (f.Visible == false) continue;   // CSBR-131778
                ResultsCriteria.Field resultf = new ResultsCriteria.Field();
                resultf.Id = f.Id;
                subTable.Criterias.Add(resultf);

                bool bIsStructure = f.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE || FormDbMgr.IsStructureLookupField(f, formview.Form.FormDbMgr.SelectedDataView);
                if (bIsStructure)
                    FormDbMgr.AddFmlaMolwtFields(f.Id, subTable);
            }
        }
        //---------------------------------------------------------------------
        private static void GridToRC(ChemDataGrid igrid, COEDataView.DataViewTable t,
                                    FormViewControl formview, ResultsCriteria rc,
                                    ResultsCriteria.ResultsCriteriaTable existingSubTable)
        {
            COEDataView.DataViewTable tSub = TagToDVTable(igrid.Tag, formview);
            if (tSub == null)
                return;

            // create new table in rc
            ResultsCriteria.ResultsCriteriaTable subTable = existingSubTable;
            if (subTable == null)
            {
                subTable = new ResultsCriteria.ResultsCriteriaTable();
                subTable.Id = tSub.Id;
                rc.Tables.Add(subTable);
            }

            CBVDataGridView wgrid = igrid.SourceWinGrid as CBVDataGridView;
            if (wgrid != null)
            {
                // stored form has win datagridview; create field for each column
                // TO DO: drop those marked hidden?
                DataGridViewColumnCollection dgvCols = wgrid.Columns;
                foreach (DataGridViewColumn dgCol in dgvCols)
                {
                    String sFieldname = dgCol.DataPropertyName;
                    FieldToRC(sFieldname, formview.Form.FormDbMgr.SelectedDataView, tSub, subTable);
                }
            }
            else
            {
                // stored form is older, has no win dgv; add the full subform, all columns
                // code adapted from FormDbMgr.AddTableToRC
                foreach (COEDataView.Field f in tSub.Fields)
                {
                    if ((f.Visible == false) ||     // CSBR-131778
                        ((f.IsDefault == false) &&  // CSBR-153313
                        ChemBioViz.NET.Properties.Settings.Default.UseDefaultFieldsOnly))
                        continue;
                    ResultsCriteria.Field resultf = new ResultsCriteria.Field();
                    resultf.Id = f.Id;
                    subTable.Criterias.Add(resultf);

                    bool bIsStructure = f.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE || FormDbMgr.IsStructureLookupField(f, formview.Form.FormDbMgr.SelectedDataView);
                    if (bIsStructure)
                        FormDbMgr.AddFmlaMolwtFields(f.Id, subTable);
                }
            }
        }
        //---------------------------------------------------------------------
        private static bool NameAliasMismatch(String fieldName, ResultsCriteria.Field field)
        {
            // is mismatch if field has alias which doesn't match given fieldname
            String fieldAlias = field.Alias;
            return !String.IsNullOrEmpty(fieldAlias) && !CBVUtil.Eqstrs(fieldAlias, fieldName);
        }
        //---------------------------------------------------------------------
        public static bool RCHasField(String fieldName, COEDataView.DataViewTable t,
                                            ResultsCriteria.ResultsCriteriaTable rcTable)
        {
            return RCGetField(fieldName, t, rcTable) != null;
        }
        //---------------------------------------------------------------------
        public static bool RCHasAggregField(ResultsCriteria.AggregateFunction aggFxn, COEDataView.DataViewTable t,
                                            ResultsCriteria.ResultsCriteriaTable rcTable)
        {
            String fieldName = aggFxn.Alias;
            return RCGetField(fieldName, t, rcTable) != null;
        }
        //---------------------------------------------------------------------
        public static ResultsCriteria.IResultsCriteriaBase RCGetField(String fieldName, COEDataView.DataViewTable t,
                                            ResultsCriteria.ResultsCriteriaTable rcTable)
        {
            // true if rctable has a field by this name
            foreach (ResultsCriteria.IResultsCriteriaBase rcbase in rcTable.Criterias)
            {
                if ((rcbase is ResultsCriteria.Formula || rcbase is ResultsCriteria.MolWeight || rcbase is ResultsCriteria.AggregateFunction) &&
                                    CBVUtil.Eqstrs(rcbase.Alias, fieldName))
                {
                    return rcbase;
                }
                else if (rcbase is ResultsCriteria.Field)
                {
                    COEDataView.Field f = FormDbMgr.FindDVFieldByName(t, fieldName);
                    if (f != null && f.Id == (rcbase as ResultsCriteria.Field).Id)
                    {
                        // don't count it as a match if rc field is using alias different from fieldname
                        if (!NameAliasMismatch(fieldName, (rcbase as ResultsCriteria.Field)))
                            return rcbase;
                    }
                }
                else if (rcbase is ResultsCriteria.HighlightedStructure)
                {
                    COEDataView.Field f = FormDbMgr.FindDVFieldByName(t, fieldName);
                    if (f != null && f.Id == (rcbase as ResultsCriteria.HighlightedStructure).Id)
                    {
                        // don't count it as a match if rc field is using alias different from fieldname
                        //if (!NameAliasMismatch(fieldName, (rcbase as ResultsCriteria.HighlightedStructure)))
                        String fieldAlias = f.Alias;
                        if (!(!String.IsNullOrEmpty(fieldAlias) && !CBVUtil.Eqstrs(fieldAlias, fieldName)))
                            return rcbase;
                    }
                }
            }
            return null;
        }
        //---------------------------------------------------------------------
        public static void SingleSubFieldToRC(FormDbMgr formDbMgr, ResultsCriteria rc,
                String subFieldName, COEDataView.DataViewTable t, ResultsCriteria.ResultsCriteriaTable rcTable,
                String aggreg)
        {
            // CSBR-135003: for auto-link box. subFieldName is table:field
            // add table to rc if not already present, then add field
			// CBOE-303, CBOE-1763, CBOE-1764 removed the ":" and placed "."
            //CBOE-2394 check with the delimiters '.' and ':' because the new fields added in the form edit will use ':' delimiter
            String subtableName = string.Empty;
            String fieldName = string.Empty;
            if (subFieldName.Contains("."))
            {
                subtableName = CBVUtil.BeforeDelimiter(subFieldName, '.');
                fieldName = CBVUtil.AfterDelimiter(subFieldName, '.');
            }
            else if (subFieldName.Contains(":"))
            {
                subtableName = CBVUtil.BeforeDelimiter(subFieldName, ':');
                fieldName = CBVUtil.AfterDelimiter(subFieldName, ':');
            }
            if (String.IsNullOrEmpty(subtableName) || String.IsNullOrEmpty(fieldName))
                return;     // complain?

            COEDataView dataView = formDbMgr.SelectedDataView;
            COEDataView.DataViewTable tSub = FormDbMgr.FindDVTableByName(subtableName, dataView);
            COEDataView.Field f = (tSub == null) ? null : FormDbMgr.FindDVFieldByName(tSub, fieldName);
            bool fieldCheck = (fieldName == "Formula" ? true : (fieldName == "Molweight" ? true : false));
            if (f == null && !(fieldCheck))
            {
                return; // complain? return false?
            }
            if (tSub != null)
            {
                ResultsCriteria.ResultsCriteriaTable subTable = RCFindTable(rc, tSub.Id);
                if (subTable == null && String.IsNullOrEmpty(aggreg))
                {
                    // CSBR-139239: if adding an aggreg field, don't add subtable item -- it gets no contents
                    subTable = new ResultsCriteria.ResultsCriteriaTable();
                    subTable.Id = tSub.Id;
                    rc.Tables.Add(subTable);
                }
                if (!String.IsNullOrEmpty(aggreg))
                    AggregToRC(fieldName, tSub, rcTable, aggreg);   // goes in main table, not sub
                else
                    FieldToRC(fieldName, formDbMgr.SelectedDataView, tSub, subTable);
            }
        }
        //---------------------------------------------------------------------
        public static void SubFieldToRC(ResultsCriteria rc, COEDataView dataView,
                                        String subtableName, String fieldName,
                                        COEDataView.DataViewTable t,
                                        ResultsCriteria.ResultsCriteriaTable rcTable)
        {
            // LIMITATION: does not prevent adding same table more than once
            // thus not fully suitable for generating auto-link boxes

            COEDataView.DataViewTable tSub = FormDbMgr.FindDVTableByName(subtableName, dataView);
            COEDataView.Field f = (tSub == null) ? null : FormDbMgr.FindDVFieldByName(tSub, fieldName);
            if (f == null) return;

            ResultsCriteria.ResultsCriteriaTable subTable = new ResultsCriteria.ResultsCriteriaTable();
            subTable.Id = tSub.Id;
            rc.Tables.Add(subTable);

            FieldToRC(fieldName, dataView, tSub, subTable);
        }
        //---------------------------------------------------------------------
        public static void FieldIDToRC(int fieldID, ResultsCriteria.ResultsCriteriaTable rcTable,
                                        String alias)
        {
            ResultsCriteria.Field resultf = new ResultsCriteria.Field();
            resultf.Id = fieldID;
            resultf.Alias = alias;
            rcTable.Criterias.Add(resultf);
        }
        //---------------------------------------------------------------------
        public static ResultsCriteria.AggregateFunction AggregToRC(String fieldName, COEDataView.DataViewTable t,
                                        ResultsCriteria.ResultsCriteriaTable rcTable, String aggreg)
        {
            // 2/11: use new AggregateFunction criterium
            COEDataView.Field f = FormDbMgr.FindDVFieldByName(t, fieldName);
            if (f == null)
                return null;
            Debug.Assert(!String.IsNullOrEmpty(aggreg));

            // create alias with fieldname and aggreg name (e.g. POC_AVG)
            ResultsCriteria.AggregateFunction aggFxn = new ResultsCriteria.AggregateFunction();
            aggFxn.Alias = String.Concat(fieldName, "_", aggreg);
            aggFxn.FunctionName = aggreg;
            if (aggFxn.Parameters == null)
                aggFxn.Parameters = new List<ResultsCriteria.IResultsCriteriaBase>();

            if (RCHasAggregField(aggFxn, t, rcTable))
                return null;

            ResultsCriteria.Field resultf = new ResultsCriteria.Field();
            resultf.Id = f.Id;
            aggFxn.Parameters.Add(resultf);

            rcTable.Criterias.Add(aggFxn);
            return aggFxn;
        }
        //---------------------------------------------------------------------
        public static COEDataView.Field /*void*/ FieldToRC(String fieldName, COEDataView d, COEDataView.DataViewTable t,
                                        ResultsCriteria.ResultsCriteriaTable rcTable)
        {
            bool bUseStructHilites = FormDbMgr.ShowSSSHilites;
            return FieldToRC(fieldName, d, t, rcTable, bUseStructHilites);
        }
        //---------------------------------------------------------------------
        public static COEDataView.Field FieldToRC(String fieldName, COEDataView d, COEDataView.DataViewTable t,
                                        ResultsCriteria.ResultsCriteriaTable rcTable, bool bUseStructHilites)
        {
            // add single field to given table in rc
            // special handling required for fmla/molwt (fields not in dataview)
            // used in creating rc and in setting up plot (CBVChartControl.GenerateRC)
            COEDataView.Field f = null;
            if (RCHasField(fieldName, t, rcTable))
                return f;

            bool bIsFormula = CBVUtil.Eqstrs(fieldName, "Formula");
            bool bIsMolweight = CBVUtil.Eqstrs(fieldName, "Molweight");
            if (bIsFormula || bIsMolweight)
            {
                COEDataView.Field structFld = FormDbMgr.FindStructureField(t, d);
                int structID = (structFld == null) ? 0 : structFld.Id;
                if (bIsFormula && structID != 0)
                {
                    ResultsCriteria.Formula resultf = new ResultsCriteria.Formula();
                    resultf.Alias = "Formula";
                    resultf.Id = structID;
                    rcTable.Criterias.Add(resultf);
                }
                else if (bIsMolweight && structID != 0)
                {
                    ResultsCriteria.MolWeight resultm = new ResultsCriteria.MolWeight();
                    resultm.Alias = "Molweight";
                    resultm.Id = structID;
                    rcTable.Criterias.Add(resultm);
                }
            }
            else
            {
                f = FormDbMgr.FindDVFieldByName(t, fieldName);
                if (f != null)
                {
                    // we can't turn these on if hitting against a 12.1 server; requires 12.3
                    // gets "input string not in correct format" due to Convert.ToInt32 in HighlightProcessor.cs
                    bool bIsStructure = f.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE || FormDbMgr.IsStructureLookupField(f, d);
                    if (bIsStructure && bUseStructHilites)
                    {
                        ResultsCriteria.HighlightedStructure resultf = new ResultsCriteria.HighlightedStructure();
                        resultf.Id = f.Id;
                        resultf.Alias = fieldName;  // otherwise fw puts its own alias "HighlightedBase64"
                        resultf.Highlight = true;
                        rcTable.Criterias.Add(resultf);
                    }
                    else
                    {
                        ResultsCriteria.Field resultf = new ResultsCriteria.Field();
                        resultf.Id = f.Id;
                        rcTable.Criterias.Add(resultf);
                    }
                }
            }
            return f;
        }
        //---------------------------------------------------------------------
        public static int GetRCBFldID(ResultsCriteria.IResultsCriteriaBase rcb)
        {
            if (rcb is ResultsCriteria.Formula) return (rcb as ResultsCriteria.Formula).Id;
            else if (rcb is ResultsCriteria.MolWeight) return (rcb as ResultsCriteria.MolWeight).Id;
            else if (rcb is ResultsCriteria.Field) return (rcb as ResultsCriteria.Field).Id;
            return 0;
        }
        //---------------------------------------------------------------------
        public static void SetRCBFldID(ResultsCriteria.IResultsCriteriaBase rcb, int id)
        {
            if (rcb is ResultsCriteria.Formula) (rcb as ResultsCriteria.Formula).Id = id;
            else if (rcb is ResultsCriteria.MolWeight) (rcb as ResultsCriteria.MolWeight).Id = id;
            else if (rcb is ResultsCriteria.Field) (rcb as ResultsCriteria.Field).Id = id;
        }
        //---------------------------------------------------------------------
        public static ResultsCriteria.IResultsCriteriaBase CreateLikeRCB(ResultsCriteria.IResultsCriteriaBase rcb)
        {
            ResultsCriteria.IResultsCriteriaBase rcbNew = null;
            if (rcb is ResultsCriteria.Formula) rcbNew = new ResultsCriteria.Formula();
            else if (rcb is ResultsCriteria.MolWeight) rcbNew = new ResultsCriteria.MolWeight();
            else if (rcb is ResultsCriteria.Field) rcbNew = new ResultsCriteria.Field();

            if (rcbNew != null)
            {
                SetRCBFldID(rcbNew, GetRCBFldID(rcb));
                rcbNew.Alias = rcb.Alias;
            }
            return rcbNew;
        }
        //---------------------------------------------------------------------
        public static bool EqRCBs(ResultsCriteria.IResultsCriteriaBase rcb1, ResultsCriteria.IResultsCriteriaBase rcb2)
        {
            // true if rcb's are of same type with same field id
            int id1 = GetRCBFldID(rcb1), id2 = GetRCBFldID(rcb2);
            if (id1 != id2)
                return false;
            if (rcb1.GetType() != rcb2.GetType())
                return false;
            return true;
        }
        //---------------------------------------------------------------------
        public static ResultsCriteria.IResultsCriteriaBase FindOrAddRCB(
             ResultsCriteria.IResultsCriteriaBase rcbQuery, ResultsCriteria rcTarget)
        {
            foreach (ResultsCriteria.ResultsCriteriaTable rctableT in rcTarget.Tables)
            {
                foreach (ResultsCriteria.IResultsCriteriaBase rcbaseT in rctableT.Criterias)
                {
                    if (EqRCBs(rcbQuery, rcbaseT))
                        return rcbaseT;
                }
            }
            ResultsCriteria.IResultsCriteriaBase rcbNew = CreateLikeRCB(rcbQuery);
            if (rcbNew != null)
            {
                rcbNew.Visible = false;
                rcTarget.Tables[0].Criterias.Add(rcbNew);
            }
            return rcbNew;
        }
        //---------------------------------------------------------------------
        public static void CopySortInfo(ResultsCriteria rcMain, ResultsCriteria rcTarget)
        {
            foreach (ResultsCriteria.ResultsCriteriaTable rctable in rcMain.Tables)
            {
                foreach (ResultsCriteria.IResultsCriteriaBase rcbase in rctable.Criterias)
                {
                    if (rcbase.OrderById == 0)
                        continue;
                    ResultsCriteria.IResultsCriteriaBase rcbaseTarget = FindOrAddRCB(rcbase, rcTarget);
                    if (rcbaseTarget != null)
                    {
                        rcbaseTarget.OrderById = rcbase.OrderById;
                        rcbaseTarget.Direction = rcbase.Direction;
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        public static bool HasAddedFields(ResultsCriteria rcNew, ResultsCriteria rcOld)
        {
            // true if rcNew contains fields or tables not found in rcOld
            // means the dataset retrieved by rcOld is inadequate
            foreach (ResultsCriteria.ResultsCriteriaTable rcTableNew in rcNew.Tables)
            {
                ResultsCriteria.ResultsCriteriaTable rcTableOld = RCFindTable(rcOld, rcTableNew.Id);
                if (rcTableOld == null)
                    return true;

                foreach (ResultsCriteria.IResultsCriteriaBase rcBase in rcTableNew.Criterias)
                {
                    if (!RCTableHasCriterium(rcTableOld, rcBase))
                        return true;
                }
            }
            return false;
        }
        //---------------------------------------------------------------------
        #endregion

        #region Dataviews To Grid
        private static bool IsSuitableProp(PropertyDescriptor prop, Object obj)
        {
            // true if obj property can be added to dataset
            if (!prop.PropertyType.IsSerializable) return false;

            if (prop.Name.Equals("IsValid")) return false;  // these props require server trips
            if (prop.Name.Equals("IsSavable")) return false;

            if (prop.GetValue(obj) == null) return false;
            if (String.IsNullOrEmpty(prop.Name)) return false;
            if (prop.Name.StartsWith("BrokenRules")) return false;

            String s = prop.GetValue(obj).ToString();
            if (String.IsNullOrEmpty(s)) return false;
            if (s.StartsWith("System.")) return false;
            if (s.StartsWith("<?xml ")) return false;

            return true;
        }
        //---------------------------------------------------------------------
        private static DataTable MakeTable(String name, Object obj, bool bWithParentID, bool bWithUniqueID)
        {
            // create table in dataset, fill with columns of obj properties
            DataTable dataTable = new DataTable(name);
            if (bWithParentID) dataTable.Columns.Add(new DataColumn("PID"));
            if (bWithUniqueID) dataTable.Columns.Add(new DataColumn("UID"));

            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(obj))
            {
                if (IsSuitableProp(prop, obj))
                    dataTable.Columns.Add(new DataColumn(prop.Name));
            }
            return dataTable;
        }
        //---------------------------------------------------------------------
        private static void AddRow(DataTable table, Object obj, int parentID, int uniqueID)
        {
            // add row to table, filling with obj prop values
            if (table == null)
                return;

            DataRow dataRow = table.NewRow();
            if (parentID > 0) dataRow["PID"] = parentID;
            if (uniqueID > 0) dataRow["UID"] = uniqueID;

            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(obj))
            {
                try
                {
                    if (IsSuitableProp(prop, obj))
                        dataRow[prop.Name] = prop.GetValue(obj);
                }
                catch (Exception)
                { }
            }
            table.Rows.Add(dataRow);
        }
        //---------------------------------------------------------------------
        private static int FindDviewWithThreeComps(ChemBioVizForm form)
        {
            for (int i = 0; i < form.FormDbMgr.DataViews.Count; ++i)
            {
                COEDataViewBO dvView = form.FormDbMgr.DataViews[i];
                COEDataView.DataViewTable dvTable = null;
                COEDataView.Field dvField = null;
                COEDataView.Relationship dvRel = null;

                if (dvView.COEDataView.Tables.Count > 0)
                    dvTable = dvView.COEDataView.Tables[0];
                if (dvTable != null && dvTable.Fields.Count > 0)
                    dvField = dvTable.Fields[0];
                if (dvView.COEDataView.Relationships.Count > 0)
                    dvRel = dvView.COEDataView.Relationships[0];

                if (dvTable != null && dvField != null && dvRel != null)
                    return i;
            }
            return 0;
        }
        //---------------------------------------------------------------------
        public static void DataviewsToGrid(ChemBioVizForm form, UltraGrid uGrid)
        {
            // create a dataset containing info about dataviews, bind it to a grid
            CBVTimer.StartTimer(true, "Convert dataviews to grid", true);
            CBVStatMessage.Show("Converting dataviews");

            // assume dataview list has at least one entry of each type; use the first to define the schema
            if (form.FormDbMgr.DataViews == null || form.FormDbMgr.DataViews.Count == 0)
                return;

            int index = FindDviewWithThreeComps(form);

            COEDataViewBO dvView = form.FormDbMgr.DataViews[index];
            COEDataView.DataViewTable dvTable = null;
            COEDataView.Field dvField = null;
            COEDataView.Relationship dvRel = null;

            if (dvView != null)
            {
                if (dvView.COEDataView.Tables.Count > 0)
                    dvTable = dvView.COEDataView.Tables[0];
                if (dvTable != null && dvTable.Fields.Count > 0)
                    dvField = dvTable.Fields[0];
                if (dvView.COEDataView.Relationships.Count > 0)
                    dvRel = dvView.COEDataView.Relationships[0];
            }

            DataSet dataSet = new DataSet("DataviewsDS");
            DataTable t_dviews = (dvView == null) ? null : MakeTable("DViews", dvView, false, false);
            DataTable t_dvtables = (dvTable == null) ? null : MakeTable("DVTables", dvTable, true, true);
            DataTable t_dvfields = (dvField == null) ? null : MakeTable("DVFields", dvField, true, false);
            DataTable t_dvrels = (dvRel == null) ? null : MakeTable("DVRelations", dvRel, true, false);
            if (t_dviews != null) dataSet.Tables.Add(t_dviews);
            if (t_dvtables != null) dataSet.Tables.Add(t_dvtables);
            if (t_dvfields != null) dataSet.Tables.Add(t_dvfields);
            if (t_dvrels != null) dataSet.Tables.Add(t_dvrels);

            // add rows of data
            int nMainRecs = 0;
            foreach (COEDataViewBO dview in form.FormDbMgr.DataViews)
            {
                if (dview != null)
                {
                    AddRow(t_dviews, dview, 0, 0);  // does nothing if first arg null
                    ++nMainRecs;
                    foreach (COEDataView.DataViewTable dvt in dview.COEDataView.Tables)
                    {
                        if (dvt != null)
                        {
                            // each table must have a globally unique ID for relating; create it from dview + table ids
                            int uniqueTableID = 100000 * dview.ID + dvt.Id;

                            AddRow(t_dvtables, dvt, dview.ID, uniqueTableID);
                            foreach (COEDataView.Field dvf in dvt.Fields)
                            {
                                if (dvf != null)
                                    AddRow(t_dvfields, dvf, uniqueTableID, 0);
                            }
                        }
                    }
                    foreach (COEDataView.Relationship dvr in dview.COEDataView.Relationships)
                    {
                        if (dvr != null)
                            AddRow(t_dvrels, dvr, dview.ID, 0);
                    }
                }
            }

            // create relations
            if (t_dviews != null && t_dvtables != null && t_dvrels != null && t_dvfields != null)
            {
                if (t_dviews.Columns.Count > 0 && t_dvtables.Columns.Count > 0 && t_dvrels.Columns.Count > 0 && t_dvfields.Columns.Count > 0)
                {
                    if (t_dviews.Columns.Contains("ID") && t_dvtables.Columns.Contains("PID") && t_dvrels.Columns.Contains("PID")
                        && t_dvtables.Columns.Contains("UID") && t_dvfields.Columns.Contains("PID"))
                    {
                        DataRelation r_vt = new DataRelation("RelVT", t_dviews.Columns["ID"], t_dvtables.Columns["PID"]);
                        dataSet.Relations.Add(r_vt);

                        DataRelation r_vr = new DataRelation("RelVR", t_dviews.Columns["ID"], t_dvrels.Columns["PID"]);
                        dataSet.Relations.Add(r_vr);

                        DataRelation r_tf = new DataRelation("RelTF", t_dvtables.Columns["UID"], t_dvfields.Columns["PID"]);
                        dataSet.Relations.Add(r_tf);
                    }
                }
            }

#if DEBUG
            // save to file for debugging and inspection
            String fname = "C:\\dsetDV_out.xml";
            dataSet.WriteXml(fname, XmlWriteMode.WriteSchema);
#endif
            // bind to grid -- fills grid with data
            uGrid.DataSource = dataSet;

            CBVTimer.EndTimer();
            CBVStatMessage.ShowReadyMsg();
        }
        #endregion

        #region Connections Dialog
        private static void AddVersionData(ConnectionsDialog dlg)
        {
            COEPrincipal principal = (COEPrincipal)Csla.ApplicationContext.User;
            VersionInfo version = ((COEIdentity)principal.Identity).COEConnection.VersionInfo;

            dlg.AddItem("Framework version", version.ServerFrameworkVersion.ToString());
            dlg.AddItem("Client version", version.ClientFrameworkVersion.ToString());
            dlg.AddItem("Oracle schema version", version.ServerOracleSchemaVersion.ToString());
            dlg.AddItem("Client code base", ChemBioVizForm.BuildDate);
        }
        //---------------------------------------------------------------------
        private static void AddUserData(ConnectionsDialog dlg, ChemBioVizForm form)
        {
            COEPrincipal principal = (COEPrincipal)Csla.ApplicationContext.User;
            COEIdentity ident = (COEIdentity)principal.Identity;

            dlg.AddItem("Identity", ident.Name);
            dlg.AddItem("Authentication type", ident.AuthenticationType);
            dlg.AddItem("Is authenticated", ident.IsAuthenticated.ToString());

            // privileges for available roles
            PrivilegeChecker privChecker = form.FormDbMgr.PrivilegeChecker;

            // CBV-specific privileges
            dlg.AddItem("Can search", privChecker.CanSearch.ToString());
            dlg.AddItem("Can edit forms", privChecker.CanEditForms.ToString());
            dlg.AddItem("Can publish forms", privChecker.CanSavePublic.ToString());
            dlg.AddItem("Can save default settings", privChecker.CanSaveSettings.ToString());
        }
        //---------------------------------------------------------------------
        public static void DoConnectionDialog(ChemBioVizForm form)
        {
            // show connection info in list box
            CBVTimer.StartTimer(true, "Retrieve connection data", true);
            CBVStatMessage.Show("Retrieving connection data");

            FormDbMgr formdbMgr = form.FormDbMgr;
            bool bHasDV = formdbMgr.SelectedDataView != null;
            bool bHasDVT = bHasDV && formdbMgr.SelectedDataViewTable != null;

            String portalURL = ConfigurationManager.AppSettings["CslaDataPortalUrl"];
            String proxy = CBVUtil.BeforeDelimiter(ConfigurationManager.AppSettings["CslaDataPortalProxy"], ',');
            String sServer = formdbMgr.Login.Server;
            bool bIs2Tier = CBVUtil.StartsWith(sServer, "2-tier");
            String sUser = formdbMgr.Login.UserName;  // or get value from app settings?
            String sDataview = formdbMgr.SelectedDataViewBOName;
            int dataViewID = bHasDV ? formdbMgr.SelectedDataView.DataViewID : 0;
            String sTable = formdbMgr.TableName;
            int tableID = bHasDVT ? formdbMgr.SelectedDataViewTable.Id : 0;
            int sessionID = formdbMgr.SessionID;
            int recCount = formdbMgr.BaseTableRecordCount;
            String defDataSource = bIs2Tier ? ConfigurationUtilities.GetDefaultDataSource() : "";
            String ssu = String.Empty;
            if (bIs2Tier)
                ssu = ConfigurationUtilities.GetSingleSignOnURL();  // CSBR-118406: crashes in 3T

            String sBaseTableName = bHasDV ? formdbMgr.SelectedDataView.BaseTableName : String.Empty;
            String sBasePrimaryKeyID = bHasDV ? formdbMgr.SelectedDataView.BaseTablePrimaryKey : String.Empty;
            COEDataView.Field dvFieldBPK = bHasDVT ? FormDbMgr.FindDVFieldByID(formdbMgr.SelectedDataViewTable, CBVUtil.StrToInt(sBasePrimaryKeyID)) : null;
            String sBasePrimaryKey = (dvFieldBPK == null) ? "none" : dvFieldBPK.Name;

            String sTablePrimaryKeyID = bHasDVT ? formdbMgr.SelectedDataViewTable.PrimaryKey : String.Empty;
            COEDataView.Field dvFieldPK = bHasDVT ? FormDbMgr.FindDVFieldByID(formdbMgr.SelectedDataViewTable, CBVUtil.StrToInt(sTablePrimaryKeyID)) : null;
            String sTablePrimaryKey = (dvFieldPK == null) ? "none" : dvFieldPK.Name;

            ConnectionsDialog dlg = new ConnectionsDialog(form);

            dlg.AddItem("Server", sServer);
            if (bIs2Tier)
                dlg.AddItem("Oracle server", defDataSource);
            dlg.AddItem("User", sUser);

            dlg.AddItem("Dataview", sDataview);
            dlg.AddItem("Dataview ID", dataViewID.ToString());
            dlg.AddItem("Base table", sBaseTableName);
            dlg.AddItem("Base primary key", String.Format("{0} [{1}]", sBasePrimaryKeyID, sBasePrimaryKey));

            dlg.AddItem("Table", sTable);
            dlg.AddItem("Table ID", tableID.ToString());
            dlg.AddItem("Primary key", String.Format("{0} [{1}]", sTablePrimaryKeyID, sTablePrimaryKey));

            dlg.AddItem("Session ID", sessionID.ToString());
            dlg.AddItem("Records", recCount.ToString());
            dlg.AddItem("URL", portalURL);
            dlg.AddItem("Proxy", proxy);
            if (bIs2Tier)
                dlg.AddItem("Sign-on URL", ssu);
            dlg.AddItem("Form ID", form.FormID.ToString());

            AddVersionData(dlg);
            AddUserData(dlg, form);

            dlg.ShowDialog();
            CBVTimer.EndTimer();
            CBVStatMessage.ShowReadyMsg();
        }
        //---------------------------------------------------------------------
        public static List<String> GetInternalVariableNames()
        {
            List<String> names = new List<string>();
            names.Add(CBVConstants.TOKEN_AUTHTICKET);
            names.Add(CBVConstants.TOKEN_USERNAME);
            names.Add(CBVConstants.TOKEN_DATABASENAME);
            names.Add(CBVConstants.TOKEN_TABLENAME);
            names.Add(CBVConstants.TOKEN_FORMNAME);
            names.Add(CBVConstants.TOKEN_TODAYSDATE);
            names.Add(CBVConstants.TOKEN_APPDATADIR);
            names.Add(CBVConstants.TOKEN_EXEDIR);
            names.Add(CBVConstants.TOKEN_HITLISTID);
            names.Add(CBVConstants.TOKEN_CURRECNO);
            return names;
        }
        //---------------------------------------------------------------------
        public static bool IsInternalVariable(String name, ChemBioVizForm form)
        {
            if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_AUTHTICKET)) return true;
            if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_USERNAME)) return true;
            if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_DATABASENAME)) return true;
            if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_TABLENAME)) return true;
            if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_FORMNAME)) return true;
            if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_TODAYSDATE)) return true;
            if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_APPDATADIR)) return true;
            if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_EXEDIR)) return true;
            if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_HITLISTID)) return true;
            if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_CURRECNO)) return true;
            return false;
        }
        //---------------------------------------------------------------------
        public static String GetInternalValue(String name, ChemBioVizForm form)
        {
            String s = "";

            COEPrincipal principal = (COEPrincipal)Csla.ApplicationContext.User;
            COEIdentity ident = (COEIdentity)principal.Identity;

            if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_AUTHTICKET))
            {
                s = form.FormDbMgr.GetUpdatedTicket(ident.IdentityToken);
            }
            else if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_USERNAME))
            {
                s = ident.Name;
            }
            else if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_DATABASENAME))
            {
                s = form.FormDbMgr.AppName;
            }
            else if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_TABLENAME))
            {
                s = form.FormDbMgr.TableName;
            }
            else if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_FORMNAME))
            {
                s = form.FormName;
            }
            else if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_TODAYSDATE))
            {
                s = DateTime.Today.ToLongDateString();
            }
            else if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_APPDATADIR))
            {
                s = Application.CommonAppDataPath;
            }
            else if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_EXEDIR))
            {
                s = Application.StartupPath;
            }
            else if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_HITLISTID))
            {
                int hitlistID = form.GetSavedHitlistID();
                if (hitlistID > 0)
                    s = CBVUtil.IntToStr(hitlistID);
            }
            else if (CBVUtil.Eqstrs(name, CBVConstants.TOKEN_CURRECNO))
            {
                s = CBVUtil.IntToStr(form.Pager.CurrRow);
            }
            return s;
        }
        #endregion

        #region RowStack helpers
        public static RowStack GetRowStack(UltraGridCell cClicked)
        {
            // given cell in grid, return stack of parent rows, with main row on top of stack
            RowStack rstack = new RowStack();
            UltraGridRow row = cClicked.Row;
            while (row != null)
            {
                rstack.Push(row.Index);
                row = row.ParentRow;
            }
            // for subform, put main recno on top of stack
            ChemDataGrid cd_grid = cClicked.Band.Layout.Grid as ChemDataGrid;
            if (cd_grid != null && cd_grid.IsSubformGrid && cd_grid.Parent is FormViewControl)
            {
                //Coverity Bug Fix CID 13022 
                ChemBioVizForm form = ((FormViewControl)cd_grid.Parent).Form;
                if (form != null)
                    rstack.Push(form.Pager.CurrRowInPage);
            }
            else
            // for main grid: change topmost row from abs to rel
            {
                int mainRowAbs = (int)rstack.Pop();
                int mainRowRel = mainRowAbs % CBVUtil.PageSize;
                rstack.Push(mainRowRel);
            }
            return rstack;
        }
        //---------------------------------------------------------------------
        public static RowStack GetRowStack(CBVButton button)
        {
            // this version is for click on form button; all we need is the current main row
            RowStack rstack = new RowStack();
            int val = (button != null && button.Form.Pager != null) ? button.Form.Pager.CurrRowInPage : 0;
            rstack.Push(val);
            return rstack;
        }
        //---------------------------------------------------------------------
        #endregion
        #endregion
    }
    public class RowStack : Stack<int>
    {
        public RowStack()
        {
        }
        //---------------------------------------------------------------------
        public RowStack(int[] array)
            : base(array)
        {
        }
        //---------------------------------------------------------------------
        public void Debug()
        {
            RowStack tmp = new RowStack(this.ToArray());
            String s = String.Empty;
            while (tmp.Count > 0)
            {
                int i = (int)tmp.Pop();
                s += i.ToString() + " ";
            }
            Console.WriteLine(s);
        }
        //---------------------------------------------------------------------
    }
}
