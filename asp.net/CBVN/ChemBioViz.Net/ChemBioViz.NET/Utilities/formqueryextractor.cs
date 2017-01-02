using System;
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
using SearchPreferences;

/// <summary>
/// Create query component and search criteria item from single box on query form
/// </summary>
public class FieldQueryExtractor
{
    #region Variables
    public enum CriteriumType { Unknown, TextOrNumeric, Structure, Formula, Molweight };

    private ChemBioVizForm m_form;
    private Control m_control;

    public String m_fieldName;                      // from tag
    public String m_qText;                          // pass to search criteria
    public String m_rawInput;                       // data from formbox
    public String m_treeField;                      // field name to be displayed in tree
    public String m_treeData;                       // query description after field
    public String m_selectedUnits;                  // chosen from combo if any
    public String m_aggreg;                         // aggreg function name for child search

    public COEDataView.DataViewTable m_cdvt;        // table and field to be searched
    public COEDataView.Field m_cdvf;
    public CriteriumType m_ctype;                   // type of criterium needed based on field (box) type
    public COEDataView.AbstractTypes m_dataType;    // type of the target data
    public bool m_bAllowListInput;                  // box-derived query props
    public bool m_bUseCustomOps;
    public SearchCriteria.COEOperators m_customOp;
    public bool m_bSearchByLookupID;                // set false if searching lookup field
    public bool m_bIsSDQuery;                       // multi-structure query based on SD file
    #endregion

    #region Constructor
    public FieldQueryExtractor(ChemBioVizForm form, Control c)
    {
        m_form = form;
        m_control = c;
        m_cdvt = null;
        m_cdvf = null;
        m_ctype = CriteriumType.Unknown;
        m_fieldName = String.Empty;
        m_dataType = COEDataView.AbstractTypes.Text;
        m_qText = String.Empty;
        m_treeField = m_treeData = String.Empty;
        m_bAllowListInput = false;
        m_bUseCustomOps = false;
        m_customOp = SearchCriteria.COEOperators.EQUAL;
        m_bSearchByLookupID = true;
        m_selectedUnits = String.Empty;
        m_aggreg = String.Empty;
        m_rawInput = (m_control == null) ? String.Empty : m_control.Text;
    }
    #endregion

    #region Public Methods
    public bool PrepBinding()
    {
        // set field and table to be searched
        // fail if no field name in tag
        String tagString = (m_control == null || m_control.Tag == null) ? String.Empty : m_control.Tag.ToString();
        m_fieldName = String.IsNullOrEmpty(tagString) ? String.Empty : CBVUtil.AfterDelimiter(tagString, '.');
        if (String.IsNullOrEmpty(m_fieldName)) return false;

        // get table to be searched; if subform, modify fieldname
        m_cdvt = m_form.FormDbMgr.SelectedDataViewTable;
        if (Query.IsSubformField(m_fieldName))
            m_cdvt = FormQueryExtractor.GetSubformTable(m_form.FormDbMgr, ref m_fieldName);
        if (m_cdvt == null) return false;

        // determine criterium type from control
        SetCriteriumType();

        // get field to be searched
        if (IsStructureType(m_ctype) && (String.IsNullOrEmpty(m_fieldName) || IsStdStructFieldname(m_fieldName)))
            m_cdvf = FormDbMgr.FindStructureField(m_cdvt, m_form.FormDbMgr.SelectedDataView);
        else
            m_cdvf = FormDbMgr.FindDVFieldByName(m_cdvt, m_fieldName);
        if (m_cdvf == null) return false;

        return true;
    }
    //---------------------------------------------------------------------
    public SearchCriteria.SearchCriteriaItem CreateItem()
    {
        // create search criteria item to control and field
        m_qText = m_rawInput;
        PrepQuery();

        SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
        switch (m_ctype)
        {
            case CriteriumType.TextOrNumeric:
                item.Criterium = Query.MakeCriterium(m_dataType, m_qText, false, false, false, m_bAllowListInput, m_bUseCustomOps, m_customOp);
                break;

            case CriteriumType.Structure:
                {
                    if (this.m_bIsSDQuery)
                    {
                        String fileName = m_qText;
                        SearchCriteria.StructureListCriteria listCriteria = new SearchCriteria.StructureListCriteria();
                        listCriteria.StructureList = File.ReadAllText(fileName);
                        item.Criterium = listCriteria;
                        m_qText = "SDF Search";
                    }
                    else
                    {
                        // CSBR-135585-related: make a copy of the global instance to belong to this query
                        String sXml = SearchOptions.SearchOptionsInstance.StructureCriteria.GenerateXmlSnippet();
                        XmlNode xmlNode = CBVUtil.LoadXmlString(sXml);
                        SearchCriteria.StructureCriteria sCriteria = new SearchCriteria.StructureCriteria(xmlNode);

                        sCriteria.Structure = (m_control as ChemDraw).Base64.ToString();
                        sCriteria.Implementation = "cscartridge";
                        item.Criterium = sCriteria;
                        m_qText = sCriteria.Structure;
                    }
                }
                break;

            case CriteriumType.Formula:
                item.Criterium = Query.MakeCriterium(COEDataView.AbstractTypes.Text, m_qText, true, false, false);
                break;
            case CriteriumType.Molweight:
                string customOperator = string.Empty;
                bool isContainsOperator = m_qText.Contains("<") || m_qText.Contains(">");

                // CBOE-106
                if (!isContainsOperator)
                {
                    switch (m_customOp)
                    {
                        case SearchCriteria.COEOperators.GT:
                            customOperator = ">";
                            break;
                        case SearchCriteria.COEOperators.GTE:
                            customOperator = ">=";
                            break;
                        case SearchCriteria.COEOperators.LT:
                            customOperator = "<";
                            break;
                        case SearchCriteria.COEOperators.LTE:
                            customOperator = "<=";
                            break;
                        default:
                            break;
                    }
                }//

                item.Criterium = Query.MakeCriterium(COEDataView.AbstractTypes.Text, customOperator + m_qText, false, true, false);
                break;
        }
        if (item.Criterium == null)
            return null;

        SetQueryInfo(item);
        return item;
    }
    //---------------------------------------------------------------------
    #endregion

    #region Private Methods
    private bool IsSDQuery(Control c)
    {
        return c is CBVQueryTextBox && !String.IsNullOrEmpty(c.Text) &&
                        CBVUtil.EndsWith(c.Text, ".sdf");   // kinda hack; could also look at field binding
    }
    //---------------------------------------------------------------------
    private void SetCriteriumType()
    {
        if (m_control is ChemDraw)
            m_ctype = CriteriumType.Structure;

        else if (IsSDQuery(m_control))
            m_ctype = CriteriumType.Structure;

        else if (m_control is ChemFormulaBox || (m_control is TextBox && CBVUtil.Eqstrs(m_fieldName, "Formula")))	// hack
            m_ctype = CriteriumType.Formula;

        else if (m_control is TextBox && CBVUtil.Eqstrs(m_fieldName, "Molweight"))	// hack
            m_ctype = CriteriumType.Molweight;

        else
            m_ctype = CriteriumType.TextOrNumeric;
    }
    //---------------------------------------------------------------------
    private bool IsFrameworkPre1103()
    {
        String fVersion = FormDbMgr.GetFrameworkVersion();
        bool bServerIsOld = fVersion.StartsWith("11.0.1") || fVersion.StartsWith("11.0.2");
        if (m_form.FormDbMgr.Login.Is2Tier && fVersion.Equals("11.0.1.0"))
            bServerIsOld = false;
        return bServerIsOld;
    }
    //---------------------------------------------------------------------
    static bool IsStructureType(CriteriumType ctype)
    {
        return ctype == CriteriumType.Structure || ctype == CriteriumType.Formula || ctype == CriteriumType.Molweight;
    }
    //---------------------------------------------------------------------
    private bool IsLookupField()
    {
        return m_cdvf != null && m_cdvf.LookupDisplayFieldId > 0 && m_cdvf.LookupFieldId > 0;
    }
    //---------------------------------------------------------------------
    static bool IsStdStructFieldname(String fieldName)
    {
        return CBVUtil.Eqstrs(fieldName, "Molweight") || CBVUtil.Eqstrs(fieldName, "Formula")
                || CBVUtil.Eqstrs(fieldName, "Structure");
    }
    //---------------------------------------------------------------------
    private COEDataView.Field GetLookupField()
    {
        int fieldID = (m_cdvf == null) ? 0 : m_cdvf.LookupDisplayFieldId;
        COEDataView.DataViewTable tLookup = FormDbMgr.FindDVTableWithField(fieldID, m_form.FormDbMgr.SelectedDataView);
        COEDataView.Field fLookup = (tLookup == null) ? null : FormDbMgr.FindDVFieldByID(tLookup, fieldID);
        return fLookup;
    }
    //---------------------------------------------------------------------
    private void SetQueryInfo(SearchCriteria.SearchCriteriaItem item)
    {
        // called after item is prepared, to set info to attach to Query object
        m_treeField = m_fieldName;
        if (this.IsLookupField())
        {
            COEDataView.Field lField = GetLookupField();
            if (lField != null)
                m_treeField = lField.Alias;
        }

        if (!IsStructureType(m_ctype))
        {
            if (!m_fieldName.Equals(m_cdvf.Alias))
                m_treeField = m_cdvf.Alias;
        }
        if (m_ctype == CriteriumType.Structure)
        {
            if (item.Criterium is SearchCriteria.StructureCriteria)
            {
                SearchCriteria.StructureCriteria sCriteria = item.Criterium as SearchCriteria.StructureCriteria;
                m_treeField = "Substructure";
                //Coverity Bug Fix CID 13020 
                if (sCriteria != null)
                {
                    if (sCriteria.FullSearch == SearchCriteria.COEBoolean.Yes) m_treeField = "Full structure";
                    if (sCriteria.Similar == SearchCriteria.COEBoolean.Yes) m_treeField = "Similarity";
                    if (sCriteria.Identity == SearchCriteria.COEBoolean.Yes) m_treeField = "Exact structure";
                    m_treeData = String.Empty;
                }
            }
            else if (item.Criterium is SearchCriteria.StructureListCriteria)
            {
                m_treeData = "SD Search";
            }
        }
    }
    //---------------------------------------------------------------------
    private String GetCheckboxQuery(CheckBox cbox)
    {
        // CSBR-135144: returns as per check state value
        String ans = String.Empty;

        switch (cbox.CheckState.ToString())
        {
            case "Checked": ans = "<> 0";
                break;
            case "Unchecked": ans = "= 0";
                break;
            case "Indeterminate": ans = "null";
                break;
            default: ans = "= 0";
                break;
        }
        return ans;
    }
    //---------------------------------------------------------------------
    private String GetUnitsFromCombo(CBVQueryTextBox qBox)
    {
        // see if there is a units combo associated with given box
        FormViewControl fvc = m_form.TabManager.CurrentTab.Control as FormViewControl;
        //coverity Bug Fix CID 13019 
        if (fvc != null)
        {
            foreach (Control c in fvc.Controls)
            {
                if (c is CBVUnitsCombo && CBVUtil.Eqstrs(qBox.Name, (c as CBVUnitsCombo).TargetBox))
                    return c.Text;
            }
        }
        return String.Empty;
    }
    //---------------------------------------------------------------------
    private void PrepQuery()
    {
        // called after field and table are established, to set parameters for creating criterium (query text, datatype, tree text)
        m_treeData = m_qText;

        // decode units if any
        m_selectedUnits = String.Empty;
        bool bCheckForUnits = (m_control is CBVQueryTextBox) && FormDbMgr.IsNumericField(m_cdvf) &&
                               !String.IsNullOrEmpty((m_control as CBVQueryTextBox).Units);
        if (bCheckForUnits)
        {
            String sResourceXml = ChemBioVizForm.GetResourceXmlString();
            CBVUnitsManager unitsMgr = new CBVUnitsManager(sResourceXml);
            m_selectedUnits = GetUnitsFromCombo(m_control as CBVQueryTextBox);
            m_qText = unitsMgr.TranslateString(m_rawInput, (m_control as CBVQueryTextBox).Units, m_selectedUnits, ref m_treeData);
        }

        // translate values from date pickers
        if (m_control is DateTimePicker)
        {
            m_treeData = m_qText = (m_control as DateTimePicker).Value.ToShortDateString();
        }
        else if (m_control is MonthCalendar)
        {
            MonthCalendar mcal = m_control as MonthCalendar;
            m_treeData = m_qText = String.Concat(mcal.SelectionStart.ToShortDateString(), " - ", mcal.SelectionEnd.ToShortDateString());
        }

        // CSBR-135144: handle checkbox
        else if (m_control is CheckBox)
        {
            m_treeData = m_qText = GetCheckboxQuery(m_control as CheckBox);
        }


        m_dataType = m_cdvf.DataType;
        m_bSearchByLookupID = false;

        // if the control is a LookupCombo and has a selection
        if (m_control is CBVLookupCombo && (m_control as CBVLookupCombo).SelectedID.Length > 0)
        {
            m_qText = (m_control as CBVLookupCombo).SelectedID;
            m_bSearchByLookupID = true;
        }
        else
        {
            COEDataView.Field fLookup = GetLookupField();
            if (fLookup != null)
            {
                m_dataType = fLookup.DataType;
            }
        }

        // set options attached to query text box
        m_bAllowListInput = m_bUseCustomOps = m_bIsSDQuery = false;
        m_customOp = SearchCriteria.COEOperators.EQUAL;

        if (m_control is CBVQueryTextBox)
        {
            CBVQueryTextBox cqtb = m_control as CBVQueryTextBox;
            m_bAllowListInput = cqtb.AllowListInput;
            // CSBR-153863: Remove this property (that is, always use
            //  the custom operator for text boxes), but leave this member
            //  due to trickiness of logic used in Query.MakeCriterium,
            //  to which it is a parameter for more than just query
            //  text boxes (see CreateItem).
            m_bUseCustomOps = true;
            m_customOp = cqtb.Operator;
            m_bIsSDQuery = IsSDQuery(m_control);
            m_aggreg = cqtb.Aggregate;
        }
    }
    //---------------------------------------------------------------------
    #endregion
}
//---------------------------------------------------------------------
/// <summary>
/// Create query and search criteria from boxes on query form
/// </summary>
public class FormQueryExtractor
{
    #region Variables
    private ChemBioVizForm m_form;
    #endregion

    #region Constructor
    public FormQueryExtractor(ChemBioVizForm form)
    {
        m_form = form;
    }
    #endregion

    #region Public Methods
    public Query CreateEx()
    {
        Query query = new Query(m_form.FormDbMgr, m_form.QueryCollection);
        String sDescr = String.Empty;
        int nextID = 1;
        SearchCriteria sc = new SearchCriteria();

        FormViewControl fvc = m_form.TabManager.CurrentTab.Control as FormViewControl;
        //Coverity Bug Fix CID 13018 
        if (fvc != null)
        {
            foreach (Control c in fvc.Controls)
            {
                // process only controls having content
                if (HasNoContent(c))
                    continue;

                // use field extractor for each control: find binding field/table, skip if none 
                FieldQueryExtractor fqe = new FieldQueryExtractor(m_form, c);
                if (!fqe.PrepBinding())
                    continue;

                // create search criteria item
                SearchCriteria.SearchCriteriaItem item = fqe.CreateItem();
                if (item == null || item.Criterium == null)
                    continue;

                // add query info to tree string, add query to collection
                if (String.IsNullOrEmpty(fqe.m_fieldName))
                {
                    Debug.Assert(false);
                }
                else
                {
                    AddStr(ref sDescr, fqe.m_treeField, fqe.m_treeData, fqe.m_aggreg);
                    AddCompEx(query, fqe, c.Name);
                }

                // new 3/11: add aggregate if any attached to box
                item.AggregateFunctionName = fqe.m_aggreg;

                // add item to search criteria
                item.TableId = fqe.m_cdvt.Id;
                item.FieldId = fqe.m_cdvf.Id;
                item.SearchLookupByID = fqe.m_bSearchByLookupID;
                item.ID = nextID++;
                sc.Items.Add(item);
            }
        }
        if (query.Components.Count == 0)    // form has no query data or is blank
            return null;

        query.SearchCriteria = sc;
        query.Description = sDescr;
        return query;
    }
    //---------------------------------------------------------------------
    public static COEDataView.DataViewTable GetSubformTable(FormDbMgr formDbMgr, ref String fieldName)
    {
        String tableName = CBVUtil.BeforeDelimiter(fieldName, ':');
        fieldName = CBVUtil.AfterDelimiter(fieldName, ':');
        COEDataView.DataViewTable t = formDbMgr.FindDVTableByName(tableName);
        return t;
    }
    //---------------------------------------------------------------------
    #endregion

    #region Private Methods
    private static bool HasNoContent(Control c)
    {
        if (c is Label)
            return true;

        if (c is DateTimePicker)
            return (c as DateTimePicker).Checked == false;

        if (c is MonthCalendar)
            return (c as MonthCalendar).SelectionStart == DateTime.Today &&
                                    (c as MonthCalendar).SelectionEnd == DateTime.Today;
        if (c is ChemDraw)
            return (c as ChemDraw).IsEmpty();

        if (c is CheckBox)
        {
            // checkbox always exists in one state (Checked/UnChecked/Indeterminate) so always false
            return false;
        }

        if (c.Text.Length == 0)
            return true;

        return false;
    }
    //---------------------------------------------------------------------
    private static void AddComp(Query q, String tag, String data, String boxname)
    {
        QueryComponent qcomp = new QueryComponent(tag, data, boxname);
        q.Components.Add(qcomp);
    }
    //---------------------------------------------------------------------
    private static void AddCompEx(Query q, FieldQueryExtractor fqe, String boxname)
    {
        QueryComponent qcomp = new QueryComponent(fqe.m_fieldName, fqe.m_qText, boxname);
        qcomp.RawInput = fqe.m_rawInput;
        qcomp.SelectedUnits = fqe.m_selectedUnits;
        q.Components.Add(qcomp);
    }
    //---------------------------------------------------------------------
    private static void AddStr(ref String s, String sField, String sQuery, String sAggreg)
    {
        // for use below in creating description
        if (s.Length > 0)
            s += ";";
        s += sField;
        if (sAggreg.Length > 0)
            s += String.Concat("(", sAggreg, ")");
        if (sQuery.Length > 0)
            s += String.Concat(": ", sQuery);
    }
    //---------------------------------------------------------------------
    #endregion
}
