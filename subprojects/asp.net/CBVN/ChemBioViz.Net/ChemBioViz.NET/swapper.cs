using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel.Design;

using CambridgeSoft.COE.Framework.Common;

using Infragistics.Win.UltraWinGrid;
using Greatis.FormDesigner;
using FormDBLib;
using ChemControls;
using CBVUtilities;
using CBVControls;

namespace ChemBioViz.NET
{
    public class ControlSwapperEx
    {
        #region Variables
        private FormViewControl m_form;
        private List<Control> m_originals, m_copies;
        #endregion

        #region Constructors
        /// <summary>
        ///  Swaps out unsafe controls during editing and serialization
        /// </summary>
        /// <param name="form"></param>
        public ControlSwapperEx(FormViewControl form)
        {
            m_form = form;
        }
        #endregion

        #region Methods
        public void PrepForWrite()
        {
            // prepare form for saving: use unbound win grids, not infragistics
            m_originals = new List<Control>();
            m_copies = new List<Control>();

            m_form.ScrollToOrigin();

            foreach (Control c in m_form.Controls)
            {
                if (c is ChemDataGrid)
                {
                    Control cCopy = MakeSafeChemGrid(c, false);
                    (cCopy as CBVDataGridView).DataSource = null;
                    (c as ChemDataGrid).SourceWinGrid = cCopy;
                    m_originals.Add(c);
                    m_copies.Add(cCopy);
                }
            }
            for (int i = 0; i < m_originals.Count; ++i)
            {
                m_form.Controls.Remove(m_originals[i]); // THIS MODIFIES THE Y COORDS!
                m_form.Controls.Add(m_copies[i]);
            }
            SortByTabIndex(m_form);
        }
        //---------------------------------------------------------------------
        private static int CmpTabIndexes(Control x, Control y)
        {
            return (x.TabIndex < y.TabIndex) ? -1 : (x.TabIndex > y.TabIndex) ? 1 : 0;
        }
        //---------------------------------------------------------------------
        private void SortByTabIndex(Control form)
        {
            Control[] array = new Control[form.Controls.Count];
            form.Controls.CopyTo(array, 0);
            Array.Sort(array, CmpTabIndexes);
            form.Controls.Clear();  // removes Accept/Cancel button defs; handled in FormViewTab.GreatisToXml

            // CSBR-137261: make sure frames come last
            foreach (Control c in array)
            {
                if (c is CBVFrame) continue;
                form.Controls.Add(c);
            }
            foreach (Control c in array)
            {
                if (!(c is CBVFrame)) continue;
                form.Controls.Add(c);
            }
        }
        //---------------------------------------------------------------------
        public void RestoreAfterWrite()
        {
            // go back to original bound grids
            for (int i = 0; i < m_originals.Count; ++i)
            {
                m_form.Controls.Remove(m_copies[i]);
                m_form.Controls.Add(m_originals[i]);
            }
        }
        //---------------------------------------------------------------------
        public void PrepForEdit()
        {
            // modify form before editing: replace unsafe controls with alternatives
            m_originals = new List<Control>();
            m_copies = new List<Control>();

            foreach (Control c in m_form.Controls)
            {
                if (c is ChemDraw || c is ChemDataGrid)
                {
                    Control cCopy = (c is ChemDraw) ? MakeSafeChemDraw(c) : MakeSafeChemGrid(c, false);
                    m_originals.Add(c);
                    m_copies.Add(cCopy);
                }
            }
            for (int i = 0; i < m_originals.Count; ++i)
            {
                m_form.Controls.Add(m_copies[i]);
                m_form.Controls.Remove(m_originals[i]);
            }
        }
        //---------------------------------------------------------------------
        public void RestoreAfterEdit()
        {
            // after editing, switch back to original controls
            // but copy modified properties from edited alternatives
            for (int i = 0; i < m_form.Controls.Count; ++i) 
            {
                Control c = m_form.Controls[i];

                // hack because we can't seem to change button text in the editor
                if (c is CBVBrowseButton && c.Text.Equals(c.Name))
                    c.Text = "...";

                int iFound = m_copies.IndexOf(c);
                if (iFound >= 0)
                {
                    Control cUpdated = m_originals[iFound];
                    ControlFactory.CopyProperties(c, cUpdated);

                    if (c is CBVDataGridView)
                    {
                        CBVDataGridView.CopyWtoIGridProps(c as CBVDataGridView, cUpdated as ChemDataGrid, 0, false, false);
                        (cUpdated as ChemDataGrid).SourceWinGrid = c;   // new 1/10
                    }
                    // first remove c and then add cUpdated on the contrary, at some point c and cUpdated have exactly the same wrong value
                    m_form.Controls.Remove(c);
                    m_form.Controls.Add(cUpdated);

                    // hack way to prevent cdax from showing outline and toolbar
                    if (cUpdated is ChemDraw)
                        m_form.SelectNextControl(cUpdated, true, true, false, true);

                    --i;
                }
            }
            // then see if we added any alternative controls during editing
            for (int i = 0; i < m_form.Controls.Count; ++i)
            {
                Control c = m_form.Controls[i];
                if (c is CBVDataGridView)
                {
                    // set the tag from the binding source
                    BindingSource bs = (c as CBVDataGridView).DataSource as BindingSource;
                    DataView syncRoot = (bs == null) ? null : bs.SyncRoot as DataView;
                    if (syncRoot != null && syncRoot.Table != null)
                    {
                        String subtableName = syncRoot.Table.TableName;
                        c.Tag = subtableName;
                    }
                    // create the new IG grid
                    ChemDataGrid cdg = new ChemDataGrid();
                    cdg.SourceWinGrid = c;
                    ControlFactory.CopyProperties(c, cdg);
                    CBVDataGridView.CopyWtoIGridProps(c as CBVDataGridView, cdg, 0, false, false);

                    ChemBioVizForm mainForm = (m_form as FormViewControl).Form;
                    m_form.Controls.Add(cdg);
                    m_form.Controls.Remove(c);
                    mainForm.InstallGridEvents(cdg);
                    --i;
                }
                else if (c is CBVSafeChemDrawBox)
                {
                    ChemDraw cd = new ChemDraw();
                    ControlFactory.CopyProperties(c, cd);
                    m_form.Controls.Add(cd);
                    m_form.Controls.Remove(c);
                    --i;
                }
            }
        }
        //---------------------------------------------------------------------
        public void RestoreAfterRead(bool bQueryForm)
        {
            // convert Win grids to Infra after reading file
            for (int i = 0; i < m_form.Controls.Count; ++i)
            {
                Control c = m_form.Controls[i];
                if ((c is CBVDataGridView || c is ChemDataGrid) && c.Tag == null)
                    c.Tag = c.Name;

                if (c is CBVDataGridView)
                {
                    ChemDataGrid cdg = new ChemDataGrid();
                    cdg.SourceWinGrid = c;
                    cdg.DisplayLayout.Override.AllowColSizing = AllowColSizing.Free;    // child cols do not resize parent
                    ControlFactory.CopyProperties(c, cdg);
                    CBVDataGridView.CopyWtoIGridProps(c as CBVDataGridView, cdg, 0, false, false);   // a little pointless, cdg has no cols yet
                    cdg.Visible = true;
                    if (!c.Visible)
                        cdg.Visible = false;    // CSBR-134486
                    ChemBioVizForm mainForm = (m_form as FormViewControl).Form;

                    m_form.Controls.Add(cdg);
                    m_form.Controls.Remove(c);
                    mainForm.InstallGridEvents(cdg);

                    cdg.Cursor = Cursors.Default;   // otherwise has wait cursor for some reason
                    --i;
                }

                // CSBR-136292: replace old-style textboxes with new
                else if (c is TextBox && !(c is CBVTextBox) && !(c is CBVQueryTextBox))
                {
                    Control cNew = null;
                    if (bQueryForm)     cNew = new CBVQueryTextBox();
                    else                cNew = new CBVTextBox();
                    ControlFactory.CopyProperties(c, cNew);

                    cNew.Visible = true;

                    m_form.Controls.Add(cNew);
                    m_form.Controls.Remove(c);
                    --i;
                }
            }
       }
        //---------------------------------------------------------------------
        static public List<String> SaveInfraGridLayouts(FormViewControl formView)
        {
            // serialize display layout for each subform grid to a list of strings
            // added 2/10: while we're at it, this is a good place to generate xml for plots
            List<String> strings = new List<string>();
            foreach (Control c in formView.Controls)
            {
                if (c is ChemDataGrid)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        (c as ChemDataGrid).DisplayLayout.SaveAsXml(stream);
                        String sXml = CBVUtil.StreamToString(stream);
                        strings.Add(sXml);
                    }
                }
            }
            return strings;
        }
        //---------------------------------------------------------------------
        static public void LoadInfraGridLayouts(FormViewControl formView, List<String> layouts)
        {
            // reload subform grid displays from string list saved above
            // also reload plots from member xml strings
            int i = 0;
            foreach (Control c in formView.Controls)
            {
                if (c is ChemDataGrid)
                {
                    String sXml = layouts[i++];
                    Stream stream = CBVUtil.StringToStream(sXml);
                    (c as ChemDataGrid).DisplayLayout.LoadFromXml(stream);
                }
            }
        }
        //---------------------------------------------------------------------
        static public void HideGridColumnsNotInForm(ChemDataGrid igrid, FormViewControl formView)
        {
            // infra grid has columns; leave visible only those with matching boxes on form
            List<String> fieldNames = formView.GetFieldNames();
            CBVDataGridView.HideIGridColsNotListed(igrid, fieldNames);

            List<String> subTableNames = formView.GetSubformTableNames();
            CBVDataGridView.HideIGridChildGridsNotListed(igrid, subTableNames);
        }
        //---------------------------------------------------------------------
        public static void AdjustGridColumnsAfterBind(ChemDataGrid cdg)
        {
            // infra grid has columns; set widths and properties from source win grids
            CBVDataGridView dgv = cdg.SourceWinGrid as CBVDataGridView;
            if (dgv != null)
            {
                int i = 0;
                CBVDataGridView.CopyWtoIGridProps(dgv, cdg, i, false, true);

                // if source has child grids, copy their info to bands
                if (dgv.ChildGrids != null)
                {
                    foreach (CBVDataGridView dgvi in dgv.ChildGrids)
                        CBVDataGridView.CopyWtoIGridProps(dgvi, cdg, ++i, dgvi.Hidden, true);
                }
            }
            //cdg.SourceWinGrid = null;    // do this later, after loading xml attached to srcWinGrid

            CBVDataGridView.ReformatMolweightCols(cdg);
        }
        //---------------------------------------------------------------------
        public void AdjustColumnsAfterBind()
        {
            foreach (Control c in m_form.Controls)
            {
                if (c is ChemDataGrid)
                    AdjustGridColumnsAfterBind(c as ChemDataGrid);
            }
        }
        //---------------------------------------------------------------------
        public static void BindingsToTags(Control root, FormViewControl fvc)
        {
            // before leaving editor: capture binding info in control tags
            // if we are editing a query form, remove any bindings after making tags
            // the formview is supplied for looking up subtables
            foreach (Control c in root.Controls)
            {
                bool bAlreadyHasTag = (c.Tag != null) && !String.IsNullOrEmpty(c.Tag.ToString());
                if (c is CBVChartControl)
                {
                    // if chart is bound to a subtable, add tag same as for subform
                    // questionable!  do not modify tag if previously determined
                    if (!bAlreadyHasTag)
                    {
                        CBVChartControl ccc = c as CBVChartControl;
                        BindingSource bs = ccc.DataSource as BindingSource;     // bs is null if ccc.DS is a dataset
                        String subtableName = String.Empty;
                        if (IsSubBindingSourceEx(bs, fvc.Form.BindingSource, ref subtableName))
                            c.Tag = subtableName;
                    }
                    continue;
                }
                if (c is CBVDataGridView)
                {
                    CBVDataGridView dgv = c as CBVDataGridView;
                    BindingSource bs = (c as CBVDataGridView).DataSource as BindingSource;
                    String subtableName = String.Empty;
                    if (IsSubBindingSourceEx(bs, fvc.Form.BindingSource, ref subtableName))
                    {
                        c.Tag = subtableName;
                        continue;
                    }
                }
                if (c.DataBindings != null)
                {
                    String sNewTag = BindingToTag(c, fvc);
                    if (!String.IsNullOrEmpty(sNewTag))     // new 4/11
                        c.Tag = sNewTag;

                    c.DataBindings.Clear();
                }
            }
        }
        //---------------------------------------------------------------------
        private static bool EndsWithAnyAggregName(Control c, String s)
        {
            if (!(c is CBVTextBox)) return false;
            if (!s.Contains("_")) return false;
            String aggName = CBVUtil.AfterDelimiter(s, '_');
            if (String.IsNullOrEmpty(aggName)) return false;
            List<String> aggNames = CBVTextBox.StdAggregConverter.GetAggregNames();
            if (aggNames.Contains(aggName))
                return true;

            return false;
        }
        //---------------------------------------------------------------------
        private static String BindingDataToTag(Control c, CBVUtilities.BindingTagData btData)
        {
            String sNewTag = CBVUtil.MakeTag(btData);
            String sAggreg = (c is CBVTextBox) ? (c as CBVTextBox).Aggregate : String.Empty;

            // CSBR-144081: if c was previously bound to "poc_max" and aggreg is now "avg", following statement gets it wrong
            // should check if EndsWith(any aggreg name)
            bool bIsPseudoField = !String.IsNullOrEmpty(sAggreg) && CBVUtil.EndsWith(sNewTag, "_" + sAggreg);
            if (!bIsPseudoField)
            {
                if (EndsWithAnyAggregName(c, sNewTag))
                    bIsPseudoField = true;
            }
            if (bIsPseudoField)
            {
                // make a new tag with original table (stored with c) + field (from newtag), agg (from box),
                // format, nullval (from bindings)
                String origTableName = (c is CBVTextBox) ? (c as CBVTextBox).ChildAggregTable : String.Empty;
                if (!String.IsNullOrEmpty(origTableName))
                {
                    String sFieldName = CBVUtil.BeforeDelimiter(btData.m_bindingMember, '_');
                    String sCombMember = String.Format("{0}:{1}", origTableName, sFieldName);
                    sNewTag = CBVUtil.MakeTag(btData.m_bindingProp, sCombMember, btData.m_formatStr, btData.m_nullValStr);
                }
            }
            return sNewTag;
        }
        //---------------------------------------------------------------------
        private static String BindingPropName(Control c, String sNameOrig)
        {
            String sName = sNameOrig;
            if (String.IsNullOrEmpty(sName))
                sName = "Text";
            if (c is CBVSafeChemDrawBox)
                sName = "base64";
            else if (c is PictureBox)
                sName = "image";
            else if (c is RichTextBox && !(c is ChemFormulaBox))  // CSBR-110766 and CSBR-110426
                sName = "rtf";
            return sName;
        }
        //---------------------------------------------------------------------
        public static String BindingNamesToTag(Control c, String sTableName, String sFieldName, CBVUtilities.BindingTagData btData)
        {
            // pass empty tablename if base table
            btData.m_bindingProp = BindingPropName(c, btData.m_bindingProp);
            btData.m_bindingMember = String.IsNullOrEmpty(sTableName) ? sFieldName : String.Format("{0}:{1}", sTableName, sFieldName);
            String sNewTag = BindingDataToTag(c, btData);
            return sNewTag;
        }
        //---------------------------------------------------------------------
        public static String BindingToTag(Control c, FormViewControl fvc)
        {
            String sNewTag = String.Empty;
            if (c == null || c.DataBindings == null || c.DataBindings.Count == 0)
                return sNewTag;

            foreach (Binding b in c.DataBindings)
            {
                String sName = b.PropertyName, sMember = b.BindingMemberInfo.BindingMember;
                String sFormat = b.FormatString, sNullVal = (b.NullValue == null) ? String.Empty : b.NullValue as String;

                sName = BindingPropName(c, sName);  // CSBR-142585
                if (String.IsNullOrEmpty(sName) || String.IsNullOrEmpty(sMember))
                    continue;

                String subTableName = String.Empty;
                bool bIsSubBinding = IsSubBindingSourceEx(b.DataSource as BindingSource, fvc.Form.BindingSource, ref subTableName);

                if (bIsSubBinding)
                {
                    String sTmp = sMember;
                    sMember = "";   // prevent creating tag if any failure
                    int subTableId = CBVUtil.StrToInt(CBVUtil.AfterDelimiter(subTableName, '_'));
                    if (fvc.Form != null && subTableId > 0)
                    {
                        // TO DO: we ought to have a higher-level routine do this
                        COEDataView dataview = fvc.Form.FormDbMgr.SelectedDataView;
                        COEDataView.DataViewTable tSub = FormDbMgr.FindDVTableByID(subTableId, dataview);
                        if (tSub != null)
                        {
                            subTableName = tSub.Name;
                            sMember = String.Concat(subTableName, ":", sTmp);
                        }
                    }
                }
                // CSBR-109285: if user enters binding via Tag prop, it comes back with
                // sName = 'Tag' and sMember = fieldname
                if (sName.Equals("Tag"))
                {
                    sName = "Text"; // ?? don't know where to get the default propty
                    c.Tag = null;   // cause remake below
                }

                if (!String.IsNullOrEmpty(sMember))
                {
                    BindingTagData btData = new BindingTagData(sName, sMember, sFormat, sNullVal);
                    sNewTag = BindingDataToTag(c, btData);
                    break;
                }
            }
            return sNewTag;
        }
        //---------------------------------------------------------------------
        private static String ParseBindingSourceEx(BindingSource bs, BindingSource mainBs)
        {
            // find child, grandchild table name from binding source
            // CSBR-143531: look for relations in main binding source, not bs
            // NO! CSBR-143860 -- main dataset does not have relations if no subforms; must use old method
            String s = String.Empty;
            if (bs != null)
            {
                // eliminate if the datamember does not name a relation
                String bsMember = bs.DataMember;
                if (CBVUtil.StartsWith(bsMember, "Table_") || !bsMember.Contains("->"))
                    return s;

                // look for the relation in the main dataset
                DataSet dataSet = (mainBs == null) ? null : mainBs.DataSource as DataSet;
                bool bIsInvalidDataSet = dataSet == null || dataSet.Relations == null || dataSet.Relations.Count == 0;
                if (!bIsInvalidDataSet)
                {
                    // search thru relations of dataset for match on rel name in bs.DataMember
                    foreach (DataRelation rel in dataSet.Relations)
                    {
                        if (CBVUtil.Eqstrs(rel.RelationName, bsMember))
                        {
                            s = rel.ChildTable.TableName;
                            break;
                        }
                    }
                }
                //else
                // CSBR-144127 etc: the above might fail even if bIsInvalidDataSet is false; if so, use the other method
                if (String.IsNullOrEmpty(s))
                {
                    // failed to find in main dataset; look through parents of bs as we used to
                    // move up bindingsource tree to top, where dataset lives
                    BindingSource b = bs;
                    //Coverity Bug Fix CID 19020 
                    if (b != null)
                    {
                        while (true)
                        {
                            if (b.DataSource == null)
                                break;
                            if (b.DataSource is DataSet)
                            {
                                // search thru relations of dataset for match on rel name in bs.DataMember
                                dataSet = (DataSet)b.DataSource;
                                if (dataSet == null || dataSet.Relations == null || dataSet.Relations.Count == 0)
                                    break;
                                foreach (DataRelation rel in dataSet.Relations)
                                {
                                    if (CBVUtil.Eqstrs(rel.RelationName, bsMember))
                                    {
                                        s = rel.ChildTable.TableName;
                                        break;
                                    }
                                }
                                break;
                            }
                            //b = b.DataSource as BindingSource;
                        }
                    }
                }
            }
            return s;
        }
        //---------------------------------------------------------------------
        public static bool IsSubBindingSourceEx(BindingSource bs, BindingSource mainBs, ref String subTableName)
        {
            // true if data member is a relation string like "Relation Table_1000->Table_2000"
            // correction 2/15/11: server now returns rel name like "INV_LOCATIONS(LOCATION_ID)->INV_CONTAINERS(LOCATION_ID_FK)"
            // use new routine to look through relations for match, then return child table name
            if (bs != null)
            {
                subTableName = ParseBindingSourceEx(bs, mainBs);
                return !String.IsNullOrEmpty(subTableName);
            }
            return false;
        }
        //---------------------------------------------------------------------
        public static Control MakeSafeChemDraw(Control c)
        {
            // text box as alternative to chemdraw
            Control cCopy = new CBVSafeChemDrawBox();
            ControlFactory.CopyProperties(c, cCopy);
            return cCopy;
        }
        //---------------------------------------------------------------------
        public static Control MakeSafeChemGrid(Control c, bool bWithChildCols)
        {
            // creates Windows dataGridView as alternative to Infragistics-based ChemDataGrid
            // if bWithChildCols, make a list of datagridviews for child bands of infra grid
            CBVDataGridView dgv = new CBVDataGridView();
            ControlFactory.CopyProperties(c, dgv);

            ChemDataGrid cdg = c as ChemDataGrid;
            CBVDataGridView.CopyItoWGridProps(cdg, dgv, 0);

            if (bWithChildCols && cdg.DisplayLayout.Bands.Count > 1)
            {
                dgv.ChildGrids = new List<CBVDataGridView>();
                for (int i = 1; i < cdg.DisplayLayout.Bands.Count; ++i)
                {
                    CBVDataGridView dgvi = new CBVDataGridView();
                    dgvi.Hidden = cdg.DisplayLayout.Bands[i].Hidden;
                    CBVDataGridView.CopyItoWGridProps(cdg, dgvi, i);
                    dgv.ChildGrids.Add(dgvi);
                }
            }
            return dgv;
        }
        #endregion

    }
}
