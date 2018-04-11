using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Linq;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COEHitListService;

using CBVUtilities;
using SearchPreferences;

namespace FormDBLib
{
    #region QueryCollection class
    public class QueryCollection : List<Query>
    {
        #region Variables
        private FormDbMgr m_formDbMgr;
        private int m_queryIDOnOpen;
        private int m_queryIDSearchOver;
        #endregion

        #region Properties
        public Form Form
        {
            // this is a top-level CBVN form
            get { return this.m_formDbMgr.Owner; }
        }
        //---------------------------------------------------------------------
        public int QueryOnOpen
        {
            get { return m_queryIDOnOpen; }
            set { m_queryIDOnOpen = value; }
        }
        //---------------------------------------------------------------------
        public int QuerySearchOver
        {
            get { return m_queryIDSearchOver; }
            set { m_queryIDSearchOver = value; }
        }
        //---------------------------------------------------------------------
        public Query RetrieveAllQuery
        {
            //get { Debug.Assert(this.Count > 0 && this[0] is RetrieveAllQuery); return this[0]; }
            get {
                if (this.Count == 0)    // CSBR-141809
                {
                    Query qRAQ = MakeRetrieveAllQuery();
                    Add(qRAQ);
                }
                Debug.Assert(this.Count > 0 && this[0] is RetrieveAllQuery); 
                return this[0]; }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Constructors
        public QueryCollection(FormDbMgr formDbMgr)
        {
            // collection of queries owned by a form
            this.m_formDbMgr = formDbMgr;
            m_queryIDOnOpen = 0;
            m_queryIDSearchOver = 0;

            Query qRAQ = MakeRetrieveAllQuery();
            Add(qRAQ);
        }
        #endregion

        #region Methods
        //---------------------------------------------------------------------
        private Query MakeRetrieveAllQuery()
        {
            Query q = new RetrieveAllQuery(m_formDbMgr, this);
            q.ID = 0;
            q.Name = "Retrieve All";
            // CSBR-147559: prevent crash here if dataview faulty
            try
            {
                q.NumHits = m_formDbMgr.BaseTableRecordCount;   // may not yet be counted
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERR IN MAKE RTVALL QUERY: " + ex.Message);
            }
            return q;
        }
        //---------------------------------------------------------------------
        public void ResetRAQCount()
        {
            // Debug.Assert(Count > 0 && this[0] is RetrieveAllQuery);
            // Query qRAQ = this[0];
            Query qRAQ = RetrieveAllQuery;
            if (qRAQ != null)
                qRAQ.NumHits = m_formDbMgr.BaseTableRecordCount;
        }
        //---------------------------------------------------------------------
        public void PrepareToSave()
        {
            // make permanent hitlists for queries not yet saved
            CBVStatMessage.Show("Saving hitlists");
            foreach (Query q in this)
            {
                if (q is RetrieveAllQuery)
                    continue;
                if (!q.IsFlagged(Query.QueryFlag.kfDiscard) && !q.IsSaved && q.HitListID != 0)
                    q.SaveHitlist();
            }
            CBVStatMessage.Hide();
        }
        //---------------------------------------------------------------------
        public XmlElement CreateXmlElement(XmlDocument xdoc, String eltname)
        {
            // create xml for entire collection
            // do not save queries marked for discard
            // first make sure we have saved any hitlists needed permanently
            PrepareToSave();

            XmlElement root = xdoc.CreateElement(eltname);
            foreach (Query q in this)
            {
                if (!q.IsFlagged(Query.QueryFlag.kfDiscard))
                    root.AppendChild(q.CreateXmlElement(xdoc, "query"));
            }
            // root.AppendChild();
            if (m_queryIDOnOpen > 0)
                CBVUtil.SetStrAttrib(root, "queryIDOnOpen", CBVUtil.IntToStr(this.m_queryIDOnOpen));
            return root;
        }
        //---------------------------------------------------------------------
        public Query CreateFromXmlNode(XmlNode node)
        {
            // create appropriate type of query object based on presence of hitlistID
            // no! never create a retrieve-all query; create normal or merge [<= OBSOLETE COMMENT]
            Query q = null;
            String q1NameOld = CBVUtil.GetStrAttrib(node, "q1name");
            int q1IDNew = CBVUtil.GetIntAttrib(node, "q1ID");
            bool bIsMergeQuery = !String.IsNullOrEmpty(q1NameOld) || q1IDNew > 0;
            int qID = CBVUtil.GetIntAttrib(node, "id");
            bool bIsRetrieveAllQuery = qID == 0;

            if (bIsMergeQuery)
                q = new MergeQuery(m_formDbMgr, this);
            else if (bIsRetrieveAllQuery)
                q = new RetrieveAllQuery(m_formDbMgr, this);
            else
                q = new Query(m_formDbMgr, this);
            q.LoadXmlElement(node);
            return q;
        }
        //---------------------------------------------------------------------
        public void LoadXmlElement(XmlNode node)
        {
            // create collection from xml
            String oldQueryOnOpenStr = CBVUtil.GetStrAttrib(node, "queryOnOpen");
            m_queryIDOnOpen = CBVUtil.StrToInt(CBVUtil.GetStrAttrib(node, "queryIDOnOpen"));

            this.Clear();
            foreach (XmlNode qNode in node.ChildNodes)
            {
                if (qNode.Name.Equals("query"))
                    Add(CreateFromXmlNode(qNode));
            }

            // formerly queryOnOpen was a name; now it is an ID.  When loading old style, convert here
            if (m_queryIDOnOpen == 0 && !String.IsNullOrEmpty(oldQueryOnOpenStr))
            {
                Query qOnOpen = this.Find(oldQueryOnOpenStr);
                m_queryIDOnOpen = (qOnOpen == null) ? 0 : qOnOpen.ID;
            }
            ProvideRAQIfMissing();
        }
        //---------------------------------------------------------------------
        private void ProvideRAQIfMissing()
        {
            if (Count > 0 && this[0] is RetrieveAllQuery)
            {
                m_formDbMgr.BaseTableRecordCount = this[0].NumHits;
                return;
            }
            
            foreach (Query q in this)
            {
                if (q is RetrieveAllQuery)
                {
                    // move RAQ to slot zero
                    this.Remove(q);
                    this.Insert(0, q);
                    return;
                }
            }
            // no RAQ: make one
            Query qRAQ = MakeRetrieveAllQuery();
            Insert(0, qRAQ);
        }
        //---------------------------------------------------------------------
        public void LookupMergeQueries()
        {
            // after loading query ID's for merged queries, find the associated queries
            foreach (Query q in this)
                if (q is MergeQuery)
                    (q as MergeQuery).LookupComponents(this);
        }
        //---------------------------------------------------------------------
        public int FindNextAvailableID()
        {
            int idMax = 0;
            foreach (Query q in this)
                if (q.ID > idMax) idMax = q.ID;
            return idMax + 1;
        }
        //---------------------------------------------------------------------
        public Query FindByID(int id)
        {
            Query q = (from k in this
                       where (k.ID == id)
                       select k).FirstOrDefault<Query>();

            return q;
        }
        //---------------------------------------------------------------------
        public Query Find(String name)
        {
            // locate query by name in collection
            Query q = (from k in this
                       where (CBVUtil.Eqstrs(k.Name, name))
                       select k).FirstOrDefault<Query>();
            return q;
        }
        //---------------------------------------------------------------------
        public bool ExistQuery(String name)
        {
            return (Find(name) != null ? true : false);
        }
        //---------------------------------------------------------------------
        public String GenerateUniqueQueryName()
        {
            int nextSuffix = 1;
            foreach (Query q in this)
            {
                if (q.Name.StartsWith("Q"))
                {
                    int nSuffix = CBVUtil.ParseLetterNumStr(q.Name);
                    if (nSuffix >= nextSuffix) nextSuffix = nSuffix + 1;
                }
            }
            return String.Concat("Q", nextSuffix.ToString());
        }
        //---------------------------------------------------------------------

        #endregion
    }
    #endregion

    #region QueryComponent Class
    public class QueryComponent
    {
        #region Variables
        // holds query info from a form box, for use in restore query
        private String m_tag, m_data;
        protected String m_boxName;     // name of form box where query was input
        protected String m_rawInput;    // data actually input by user; blank if same as m_data
        protected String m_selUnits;    // units label selected from combo, or blank
        #endregion

        #region Properties
        public String Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }
        //---------------------------------------------------------------------
        public String Data
        {
            get { return m_data; }
            set { m_data = value; }
        }
        //---------------------------------------------------------------------
        public String BoxName
        {
            get { return m_boxName; }
            set { m_boxName = value; }
        }
        //---------------------------------------------------------------------
        public String RawInput
        {
            get { return m_rawInput; }
            set { m_rawInput = value; }
        }
        //---------------------------------------------------------------------
        public String SelectedUnits
        {
            get { return m_selUnits; }
            set { m_selUnits = value; }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Constructors
        public QueryComponent()
        {
            m_boxName = "";
        }
        //---------------------------------------------------------------------
        public QueryComponent(String tag, String data, String boxname)
        {
            m_tag = tag;
            m_data = data;
            m_boxName = boxname;
            m_rawInput = m_selUnits = String.Empty;
        }
        #endregion

        #region Methods
        public XmlElement CreateXmlElement(XmlDocument xdoc, String eltname)
        {
            XmlElement element = xdoc.CreateElement(eltname);
            element.SetAttribute("tag", Tag);
            element.SetAttribute("boxname", BoxName);

            if (!String.IsNullOrEmpty(RawInput) && !RawInput.Equals(Data))
                element.SetAttribute("raw_input", RawInput);
            if (!String.IsNullOrEmpty(SelectedUnits))
                element.SetAttribute("sel_units", SelectedUnits);

            element.InnerText = Data;
            return element;
        }
        //---------------------------------------------------------------------
        public void LoadXmlElement(XmlNode node)
        {
            Tag = CBVUtil.GetStrAttrib(node, "tag");
            BoxName = CBVUtil.GetStrAttrib(node, "boxname");
            RawInput = CBVUtil.GetStrAttrib(node, "raw_input");
            SelectedUnits = CBVUtil.GetStrAttrib(node, "sel_units");
            Data = node.InnerText;
        }
        //---------------------------------------------------------------------
        #endregion
    }
    #endregion

    #region Query Class
    public class Query
    {
        #region Variables & Enums
        protected FormDbMgr m_formDbMgr;
        protected int m_id;
        protected String m_name;
        protected String m_description;
        protected int m_hitListID;
        protected int m_numHits;
        protected int m_parentQueryID = 0;

        private SearchCriteria m_searchCriteria;
        protected Pager m_pager;

        protected List<QueryComponent> m_components;
        protected String m_tabName;     // name of tab where query was input; for restore query
        protected QueryCollection m_queryCollection;

        protected int m_flags;
        protected String m_sortStr;     // fields to sort, separated by semicolons; optional " D" at end; temporary
        protected String m_defaultSort; // sort to use by default if not overridden; this one is serialized
        public bool SaveFlag;

        public enum QueryFlag
        {
            kfDiscard = 0x01,           // means has an asterisk in tree; serialized value ignored
            kfIsSaved = 0x02,           // means hitlist id is a permanent saved list on the server
        };
        public enum StrucSearchType
        {
            Substructure,
            FullStructure,
            ExactStructure,
            Similarity
        };       
        #endregion

        #region Properties
        public int Flags
        {
            get { return m_flags; }
            set { m_flags = value; }
        }
        //---------------------------------------------------------------------
        public String SortString
        {
            get { return m_sortStr; }
            set { m_sortStr = value; }
        }
        //---------------------------------------------------------------------
        public String DefaultSortString
        {
            get { return m_defaultSort; }
            set { m_defaultSort = value; }
        }
        //---------------------------------------------------------------------
        public bool HasStructComponent
        {
            get { return FindStructCriteria(SearchCriteria) != null; }
        }
        //---------------------------------------------------------------------
        public StrucSearchType SSType
        {
            get { return StructSearchTypeFromSC(FindStructCriteria(SearchCriteria)); }
        }
        //---------------------------------------------------------------------
        public bool IsHilitable
        {
            get { return HasStructComponent && SSType != StrucSearchType.Similarity; }
        }
        //---------------------------------------------------------------------
        public static StrucSearchType StructSearchTypeFromSC(SearchCriteria.StructureCriteria sc)
        {
            if (sc != null) 
            {
                if (sc.FullSearch == SearchCriteria.COEBoolean.Yes)
                    return StrucSearchType.FullStructure;
                else if (sc.Similar == SearchCriteria.COEBoolean.Yes)
                    return StrucSearchType.Similarity;
                else if (sc.Identity == SearchCriteria.COEBoolean.Yes)
                    return StrucSearchType.ExactStructure;
            }
            return StrucSearchType.Substructure;
        }
        //---------------------------------------------------------------------
        public bool HasChildField
        {
            // true if any component of SC refers to table other than base
            get {
                if (SearchCriteria != null)
                {
                    int baseTableID = this.m_formDbMgr.SelectedDataViewTable.Id;
                    foreach (SearchCriteria.SearchExpression item in SearchCriteria.Items)
                    {
                        if (item is SearchCriteria.SearchCriteriaItem)
                        {
                            int childTableID = (item as SearchCriteria.SearchCriteriaItem).TableId;
                            if (childTableID > 0 && childTableID != baseTableID)
                                return true;
                        }
                    }
                }
                return false;
            }
        }
        //---------------------------------------------------------------------
        public static SearchCriteria.StructureCriteria FindStructCriteria(SearchCriteria sc)
        {
            if (sc != null)
            {
                foreach (SearchCriteria.SearchExpression se in sc.Items)
                {
                    if (se is SearchCriteria.SearchCriteriaItem)
                    {
                        SearchCriteria.SearchCriteriaItem sci = se as SearchCriteria.SearchCriteriaItem;
                        if (sci.Criterium != null && sci.Criterium is SearchCriteria.StructureCriteria)
                            return sci.Criterium as SearchCriteria.StructureCriteria;
                    }
                }
            }
            return null;
        }
        //---------------------------------------------------------------------
        public bool IsHitlistQuery
        {
            get { return HitListID > 0 && SearchCriteria == null && Components.Count == 0; }
        }
        //---------------------------------------------------------------------
        public bool IsSaved
        {
            get { return IsFlagged(QueryFlag.kfIsSaved); }
            set { bool b = value; Flag(QueryFlag.kfIsSaved, b); }
        }
        //---------------------------------------------------------------------
        public int ID
        {
            get { return m_id; }
            set { m_id = value; }
        }
        //---------------------------------------------------------------------
        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        //---------------------------------------------------------------------
        public virtual bool IsMergeQuery
        {
            get { return false; }
        }
        //---------------------------------------------------------------------
        public String TabName
        {
            get { return m_tabName; }
            set { m_tabName = value; }
        }
        //---------------------------------------------------------------------
        public int ParentQueryID
        {
            get { return m_parentQueryID; }
            set { m_parentQueryID = value; }
        }
        //---------------------------------------------------------------------
        public bool HasParentQuery
        {
            get { return m_parentQueryID != 0; }
        }
        //---------------------------------------------------------------------
        public Query ParentQuery
        {
            get { return (m_parentQueryID > 0)? this.QueryCollection.FindByID(m_parentQueryID) : null; }
        }
        //---------------------------------------------------------------------
        public DataSet DataSet
        {
            get { return (m_pager == null) ? null : m_pager.CurrDataSet; }
        }
        //---------------------------------------------------------------------
        public Pager Pager
        {
            get { return m_pager; }
        }
        //---------------------------------------------------------------------
        public String Description
        {
            get { return m_description; }
            set { m_description = value; }
        }
        //---------------------------------------------------------------------
        public int HitListID
        {
            get { return m_hitListID; }
            set { m_hitListID = value; }
        }
        //---------------------------------------------------------------------
        public int NumHits
        {
            get { return m_numHits; }
            set { m_numHits = value; }
        }
        //---------------------------------------------------------------------
        public SearchCriteria SearchCriteria
        {
            get { return m_searchCriteria; }
            set { m_searchCriteria = value; }
        }
        //---------------------------------------------------------------------
        public List<QueryComponent> Components
        {
            get { return m_components; }
        }
        //---------------------------------------------------------------------
        public QueryCollection QueryCollection
        {
            get { return m_queryCollection; }
        }
        #endregion

        #region Constructors
        public Query(FormDbMgr formDbMgr, QueryCollection qColl)
        {
            m_formDbMgr = formDbMgr;
            m_pager = new Pager(formDbMgr);
            m_components = new List<QueryComponent>();
            m_queryCollection = qColl;
            m_flags = 0;
            m_id = qColl.FindNextAvailableID();

            m_name = "";
            m_description = "";
            m_searchCriteria = null;
            m_tabName = "";
            m_hitListID = 0;
            m_numHits = 0;
            m_sortStr = String.Empty;
            m_defaultSort = String.Empty;
        }
        //---------------------------------------------------------------------
        public Query(Query qSource, bool bDetach)
        {
            // clone given query; detach from hitlist if requested
            m_formDbMgr = qSource.m_formDbMgr;
            m_pager = new Pager(m_formDbMgr);

            m_components = new List<QueryComponent>();
            foreach (QueryComponent qc in qSource.m_components)
            {
                QueryComponent qcCopy = new QueryComponent(qc.Tag, qc.Data, qc.BoxName);
                m_components.Add(qcCopy);
            }

            m_queryCollection = qSource.m_queryCollection;
            m_flags = qSource.Flags;
            m_id = m_queryCollection.FindNextAvailableID();

            m_name = qSource.m_name;
            m_description = qSource.m_description;
            m_parentQueryID = qSource.m_parentQueryID;  // new 9/10

            String sourceCriteriaStr = qSource.SearchCriteria.ToString();
            m_searchCriteria = new SearchCriteria();
            m_searchCriteria.GetFromXML(sourceCriteriaStr);

            m_tabName = qSource.m_tabName;
            m_sortStr = qSource.m_sortStr;
            m_defaultSort = qSource.m_defaultSort;
            if (bDetach)
            {
                m_hitListID = 0;
                m_numHits = 0;
                this.IsSaved = false;
            }
            else
            {
                m_hitListID = qSource.m_hitListID;
                m_numHits = qSource.m_numHits;
                this.IsSaved = qSource.IsSaved;
            }
            this.Flag(QueryFlag.kfDiscard, true);
        }
        //---------------------------------------------------------------------
        #endregion

        #region Methods
        public void Flag(QueryFlag f, bool bOn)
        {
            int fi = (int)f;
            if (bOn) m_flags |= fi;
            else m_flags &= ~fi;
        }       
        //---------------------------------------------------------------------
        public bool IsFlagged(QueryFlag f)
        {
            return (m_flags & (int)f) != 0;
        }
        //--------------------------------------------------------------------- 
        public virtual XmlElement CreateXmlElement(XmlDocument xdoc, String eltname)
        {
            XmlElement element = xdoc.CreateElement(eltname);
            element.SetAttribute("id", CBVUtil.IntToStr(ID));
            element.SetAttribute("name", Name);
            element.SetAttribute("description", Description);
            element.SetAttribute("numhits", CBVUtil.IntToStr(NumHits));
            element.SetAttribute("hitlistid", CBVUtil.IntToStr((int)HitListID));
            element.SetAttribute("tabname", TabName);
            if (!String.IsNullOrEmpty(DefaultSortString))
                element.SetAttribute("sort", DefaultSortString);

            if (Flags != 0)
                element.SetAttribute("flags", CBVUtil.IntToStr(Flags));
            if (this.ParentQueryID != 0)
                element.SetAttribute("parent_id", CBVUtil.IntToStr(ParentQueryID));

            // NOTE: for a structure query, we are saving two copies of the base64 blob
            // one with query, one with SC

            if (Components.Count > 0)
            {
                XmlElement compselt = xdoc.CreateElement("components");
                foreach (QueryComponent comp in Components)
                    compselt.AppendChild(comp.CreateXmlElement(xdoc, "comp"));
                element.AppendChild(compselt);
            }
            if (m_searchCriteria != null)
            {
                XmlElement scelt = xdoc.CreateElement("criteria");
                String sXmlWithHeader = m_searchCriteria.ToString();
                scelt.InnerXml = CBVUtil.RemoveDocHeader8(sXmlWithHeader);
                element.AppendChild(scelt);
            }
            return element;
        }
        //---------------------------------------------------------------------
        public virtual void LoadXmlElement(XmlNode node)
        {
            ID = CBVUtil.GetIntAttrib(node, "id");
            Name = CBVUtil.GetStrAttrib(node, "name");
            Description = CBVUtil.GetStrAttrib(node, "description");
            NumHits = CBVUtil.GetIntAttrib(node, "numhits");
            HitListID = CBVUtil.GetIntAttrib(node, "hitlistid");
            TabName = CBVUtil.GetStrAttrib(node, "tabname");
            Flags = CBVUtil.GetIntAttrib(node, "flags");
            ParentQueryID = CBVUtil.GetIntAttrib(node, "parent_id");
            DefaultSortString = CBVUtil.GetStrAttrib(node, "sort");

            if (!this.IsSaved)
                HitListID = 0;  // disregard old value being read in; must rerun when first used

            Debug.Assert(!(HitListID == 0 && IsSaved));

            Components.Clear();
            foreach (XmlNode cNode in node.ChildNodes)
            {
                if (cNode.Name.Equals("components"))
                {
                    foreach (XmlNode qNode in cNode.ChildNodes)
                    {
                        if (qNode.Name.Equals("comp"))
                        {
                            QueryComponent qcomp = new QueryComponent();
                            qcomp.LoadXmlElement(qNode);
                            Components.Add(qcomp);
                        }
                    }
                }
                else if (cNode.Name.Equals("criteria"))
                {
                    m_searchCriteria = new SearchCriteria();
                    String sCriteria = cNode.InnerXml;
                    if (!CBVUtil.StrEmpty(sCriteria))
                        m_searchCriteria.GetFromXML(sCriteria);

                    // CSBR-135585: sss options in loaded sc need to be copied to global settings
                    // here or preferably at Run time
                }
            }
        }
        //---------------------------------------------------------------------
        private static void AddComp(Query q, String tag, String data, String boxname)
        {
            QueryComponent qcomp = new QueryComponent(tag, data, boxname);
            q.Components.Add(qcomp);
        }
        //---------------------------------------------------------------------
        private static void AddStr(ref String s, String sField, String sQuery)
        {
            // for use below in creating description
            if (s.Length > 0)
                s += ";";
            s += sField;
            if (sQuery.Length > 0)
                s += String.Concat(": ", sQuery);
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Get the formatted text for queries on tree. 
        /// </summary>
        /// <returns></returns>
        public string GetQueryText()
        {
            // name must be followed by colon
            String qname = this.m_name;
            if (this.IsFlagged(QueryFlag.kfDiscard))
                qname += "*";

            int qSearchOver = this.QueryCollection.QuerySearchOver;
            bool bSearchOverThis = qSearchOver != 0 && qSearchOver == this.ID;
            if (bSearchOverThis)
                qname += " [DOMAIN]";

            return string.Format("{0}: {1} [{2:D}]", qname, this.m_description, this.m_numHits);
        }
        /// <summary>
        ///  Get just the Query description and number of hits
        /// </summary>
        /// <returns></returns>
        public string GetQueryDesc()
        {
            return string.Format("{0} [{1:D}]", this.m_description, this.m_numHits);
        }
        //---------------------------------------------------------------------
        public static Control FindControlByTag(Control parent, String tag)
        {
            if (parent != null) {
                foreach (Control c in parent.Controls)
                    if (c.Tag != null && c.Tag.ToString().EndsWith(tag))    // tag is like "Text.fieldname"
                        return c;
            }
            return null;
        }
        //---------------------------------------------------------------------
        public static Control FindControlByBoxname(Control parent, String boxname)
        {
            if (parent != null) {
                foreach (Control c in parent.Controls)
                    if (CBVUtil.Eqstrs(c.Name, boxname))
                        return c;
            }
            return null;
        }
        //---------------------------------------------------------------------
        protected void ApplySortIfAny()
        {
            bool bHasSortStr = !String.IsNullOrEmpty(this.SortString);
            bool bHasDefSort = !String.IsNullOrEmpty(this.DefaultSortString);

            // if we have no designated sort but have a default, use that
            if (!bHasSortStr && bHasDefSort)
                this.SortString = this.DefaultSortString;

            // apply or unapply to RC
            if (!String.IsNullOrEmpty(this.SortString))
                m_formDbMgr.ApplySortStringToRC(SortString);
            else
                m_formDbMgr.UnapplySortStringToRC();    // CSBR-143048; otherwise sort in rc doesn't match SortString
        }
        //---------------------------------------------------------------------
        protected void CheckStructureHighlighting()
        {
            // if query has a structure component, and search mode is exact/full/similar,
            // and rc has a HighlightedStructure field: change rc field to normal, unhighlighted
            // do this the quick and dirty way by editing the xml
            if (!HasStructComponent)
                return;

            String sXml = m_formDbMgr.ResultsCriteria.ToString();
            bool bWantHiliting = SSType != StrucSearchType.Similarity; // eventually server might handle this case, but not yet
            if (!FormDbMgr.ShowSSSHilites)
                    bWantHiliting = false;
            bool bHaveHiliting = sXml.Contains("HighlightedStructure");
            if (!bWantHiliting && bHaveHiliting)
            {
                // don't want highlighting: replace with normal structure tag, rip out 'highlight="true"'
                // a tad hack
                String sXmlMod = sXml.Replace("HighlightedStructure", "field");
                sXmlMod = sXmlMod.Replace(" highlight=\"true\"", "");
                m_formDbMgr.ResultsCriteria.GetFromXML(sXmlMod);
            }
            else if (bWantHiliting && !bHaveHiliting)
            {
                // want highlighting but don't have it: must handle this ahead of time (see restoreHitlistToolStripMenuItem_Click)
                Debug.Assert(false);
            }
        }
        //---------------------------------------------------------------------
        public virtual bool CanRerun(Control form)
        {
            // true if safe to call Run
            return !IsMergeQuery && SearchCriteria != null && SearchCriteria.Items.Count > 0;
        }
        //---------------------------------------------------------------------
        public virtual bool CanRestoreHitlist
        {
            // true whether hitlist is permanent or was created during current session
            get { return HitListID != 0; }
        }
        //---------------------------------------------------------------------
        public virtual bool CanRestoreToForm(Control form)
        {
            // true if given formviewctl has at least one control tag matching query comps
            if (IsMergeQuery)
                return false;

            if (form == null)
                return false;

            foreach (QueryComponent qcomp in Components)
            {
                if (qcomp.Tag != null)
                {
                    // new 1/3/11: use same criterion as in RestoreToForm below
                    Control c = String.IsNullOrEmpty(qcomp.BoxName) ? FindControlByTag(form, qcomp.Tag.ToString()) :
                                    FindControlByBoxname(form, qcomp.BoxName);
                    if (c != null)
                        return true;    // as long as we have one valid box, we can restore
                }
            }
            return false;
        }
        //---------------------------------------------------------------------
        private static SearchCriteria MakeSBHLCriteria(int hitlistID, HitListType hlType, int srcFieldID, int tableID, int fieldID)
        {
            SearchCriteria sc = new SearchCriteria();
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            SearchCriteria.HitlistCriteria criterium = new SearchCriteria.HitlistCriteria();

            item.FieldId = fieldID;
            item.TableId = tableID;
            item.ID = 1;

            criterium.SourceFieldId = srcFieldID;
            criterium.HitlistType = hlType;
            criterium.Value = hitlistID.ToString();
            item.Criterium = criterium;

            sc.Items.Add(item);
            return sc;
        }
        //---------------------------------------------------------------------
        public bool ChangeHitlistID(int newHitlistID, HitListType hlType)
        {
            SearchCriteria scOld = this.SearchCriteria;
            if (scOld != null && scOld.Items.Count > 0)
            {
                SearchCriteria.SearchCriteriaItem item = scOld.Items[0].GetSearchCriteriaItem(1);
                //Coverity Bug Fix CID:19047 
                if (item != null)
                {
                    SearchCriteria.HitlistCriteria hlc = item.Criterium as SearchCriteria.HitlistCriteria;
                    //Coverity Bug Fix CID 13035 
                    if (hlc != null)
                    {
                        int sourceFieldID = hlc.SourceFieldId, tableID = item.TableId, fieldID = item.FieldId;
                        SearchCriteria scNew = MakeSBHLCriteria(newHitlistID, hlType, sourceFieldID, tableID, fieldID);

                        this.SearchCriteria = scNew;
                        this.Description = String.Format("Search By Hitlist {0}", newHitlistID);    // add source/tgt info?
                        return true;
                    }
                }
            }
            return false;
       }
        //---------------------------------------------------------------------
        public static Query CreateFromHitlistInfo(int hitlistID, HitListType hlType, FormDbMgr formDbMgr, QueryCollection qColl, 
            int sourceFieldID, String targetFieldName)
        {
            Debug.Assert(sourceFieldID > 0);    // if no source fld, should be calling a different method
            Debug.Assert(!String.IsNullOrEmpty(targetFieldName));

            COEDataView.DataViewTable table = formDbMgr.SelectedDataViewTable;
            int tableID = (table == null)? 0 : formDbMgr.SelectedDataViewTable.Id;
            COEDataView.Field field = (table == null)? null : FormDbMgr.FindDVFieldByName(table, targetFieldName);
            int fieldID = (field == null) ? 0 : field.Id;
            if (tableID == 0 || fieldID == 0)
                return null;

            Query query = new Query(formDbMgr, qColl);
            query.SearchCriteria = MakeSBHLCriteria(hitlistID, hlType, sourceFieldID, tableID, fieldID);
            query.IsSaved = (hlType == HitListType.SAVED);
            query.Description = String.Format("Search By Hitlist {0}", hitlistID);    // add source/tgt info?
            query.Flag(Query.QueryFlag.kfDiscard, true);
            query.Name = qColl.GenerateUniqueQueryName();

            return query;
        }
        //---------------------------------------------------------------------
        public static Query CreateFromHitlistID(int hitlistID, HitListType hlType, FormDbMgr formDbMgr, QueryCollection qColl)//, int srcFieldID)
        {
            // given id had better be a valid saved hitlist id
            Query query = new Query(formDbMgr, qColl);
            query.Description = String.Format("Hitlist {0}", hitlistID);
            query.SearchCriteria = null;
            query.HitListID = hitlistID;
            query.IsSaved = (hlType == HitListType.SAVED);

            COEHitListBO hlbo = COEHitListBO.Get(HitListType.SAVED, hitlistID);
            query.NumHits = (hlbo == null) ? 0 : hlbo.NumHits;
            query.Flag(Query.QueryFlag.kfDiscard, true);
            query.Name = qColl.GenerateUniqueQueryName();

            return query;
        }
        //---------------------------------------------------------------------
        public static Query CreateFromStrings(String fieldName, String queryStr, FormDbMgr formDbMgr, QueryCollection qColl,
                                                bool isDelimitedListOfIDs, bool idsAreText)
        {
            // create query from command-line input tokens (like "search=molname:benz")
            COEDataView.DataViewTable t = formDbMgr.SelectedDataViewTable;
            COEDataView.Field dvField = FormDbMgr.FindDVFieldByName(t, fieldName);

            Query query = new Query(formDbMgr, qColl);
            String sDescr = "";
            SearchCriteria sc = new SearchCriteria();
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            SearchCriteria.COEOperators nullOp = SearchCriteria.COEOperators.EQUAL;

            if (CBVUtil.Eqstrs(fieldName, "Formula"))
            {
                item.Criterium = MakeCriterium(COEDataView.AbstractTypes.Text, queryStr, true, false, false, false, false, nullOp);
                dvField = FormDbMgr.FindStructureField(t, formDbMgr.SelectedDataView);
            }
            else if (CBVUtil.Eqstrs(fieldName, "Molweight"))
            {
                item.Criterium = MakeCriterium(COEDataView.AbstractTypes.Text, queryStr, false, true, false, false, false, nullOp);
                dvField = FormDbMgr.FindStructureField(t, formDbMgr.SelectedDataView);
            }
            else if (CBVUtil.Eqstrs(fieldName, "Structure"))
            {
                item.Criterium = MakeCriterium(COEDataView.AbstractTypes.Text, queryStr, false, false, true, false, false, nullOp);
                dvField = FormDbMgr.FindStructureField(t, formDbMgr.SelectedDataView);
            }
            else if (dvField != null)
            {
                if (isDelimitedListOfIDs)
                    item.Criterium = idsAreText? MakeTextInClauseCriterium(queryStr) : MakeInClauseCriterium(queryStr);
                else
                    item.Criterium = MakeCriterium(dvField.DataType, queryStr, false, false, false, false, false, nullOp);
            }
            else
            {
                return null;    // field not found
            }

            if (item.Criterium == null)
                return null;

            String sDisplayName = CBVUtil.Capitalize(fieldName);
            AddStr(ref sDescr, sDisplayName, queryStr);
            AddComp(query, "Text." + fieldName, queryStr, "");  // boxname doesn't apply here (nor Restore Query)

            item.FieldId = dvField.Id;
            item.TableId = t.Id;
            item.ID = 1;
            sc.Items.Add(item);

            query.SearchCriteria = sc;
            query.Description = sDescr;

            query.Flag(Query.QueryFlag.kfDiscard, true);
            query.Name = qColl.GenerateUniqueQueryName();
            
            return query;
        }
        //---------------------------------------------------------------------
        public static SearchCriteria.ISearchCriteriaBase MakeInClauseCriterium(String delimitedList)
        {
            SearchCriteria.NumericalCriteria criterium = new SearchCriteria.NumericalCriteria();
            criterium.InnerText = delimitedList;
            criterium.Operator = SearchCriteria.COEOperators.IN;
            return criterium;
        }
        //---------------------------------------------------------------------
        public static SearchCriteria.ISearchCriteriaBase MakeTextInClauseCriterium(String delimitedList)
        {
            SearchCriteria.TextCriteria criterium = new SearchCriteria.TextCriteria();
            criterium.InnerText = delimitedList;
            criterium.Operator = SearchCriteria.COEOperators.IN;
            return criterium;
        }
        //---------------------------------------------------------------------
        private static SearchCriteria.ISearchCriteriaBase MakeCriteriumFromFile(bool bIsNumeric, bool bIsInteger, 
            String pfxdname)
        {
            String dataRaw = CBVUtil.FileToString(pfxdname.Substring(1));
            if (!String.IsNullOrEmpty(dataRaw))
            {
                String data = dataRaw.Replace("\r\n", ",");
                while (data.EndsWith(","))
                    data = data.Substring(0, data.Length - 1);
                String delimList = data;
                if (bIsNumeric)
                    delimList = CBVUtil.NumExpressionToString(data, bIsInteger);
                return MakeCriteriumFromListOfValues(bIsNumeric, bIsInteger, delimList);
            }
            return null;
        }
        //---------------------------------------------------------------------
        private static String PrepText(String textIn)
        {
            String text = textIn;
            if (text.Contains("\r\n"))
            {
                text = text.Replace("\r\n", ",");
                if (text.EndsWith(","))
                    text = text.Substring(0, text.Length - 1);
                text = text.Replace(",,", ",");
            }
            return text;
        }
        //---------------------------------------------------------------------
        private static bool IsListOfValues(bool bNumeric, String textIn)
        {
            if (CBVUtil.IsQuoted(textIn))   // CSBR-135329
                return false;

            String text = PrepText(textIn);
            if (bNumeric)
            {
                if (text.Contains(",")) return true;
                if (text.Contains(":")) return true;
            }
            else
            {
                String[] toks = text.Split(',');
                int ntoks = toks.Length;

                if (text.Contains(",")) return true;    // TO DO: not true if comma is within quoted string
                if (text.Contains("\r")) return true;
            }
            return false;
        }
        //---------------------------------------------------------------------
        private static SearchCriteria.ISearchCriteriaBase MakeCriteriumFromListOfValues(bool bIsNumeric, bool bIsInteger,
            String textIn)
        {
            String text = PrepText(textIn);
            SearchCriteria.ISearchCriteriaBase criterium = null;
            if (bIsNumeric)
            {
                String delimList = CBVUtil.NumExpressionToString(text, bIsInteger);
                criterium = new SearchCriteria.NumericalCriteria();
                ((SearchCriteria.NumericalCriteria)criterium).InnerText = delimList;
                ((SearchCriteria.NumericalCriteria)criterium).Operator = SearchCriteria.COEOperators.IN;
            } else 
            {
                criterium = new SearchCriteria.TextCriteria();
                String qtext = text.Replace(", ", ",");
                ((SearchCriteria.TextCriteria)criterium).InnerText = qtext;
                ((SearchCriteria.TextCriteria)criterium).CaseSensitive = SearchCriteria.COEBoolean.No;
                ((SearchCriteria.TextCriteria)criterium).DefaultWildCardPosition = SearchCriteria.Positions.Both;
                ((SearchCriteria.TextCriteria)criterium).Operator = SearchCriteria.COEOperators.IN;
            }
            return criterium;
        }
        //---------------------------------------------------------------------
        public static SearchCriteria.ISearchCriteriaBase MakeCriterium(COEDataView.AbstractTypes type, String text,
                                                       bool bFormula, bool bMolweight, bool base64)
        {
            return MakeCriterium(type, text, bFormula, bMolweight, base64, false, false, SearchCriteria.COEOperators.EQUAL);
        }
        //---------------------------------------------------------------------
        public static SearchCriteria.ISearchCriteriaBase MakeCriterium(COEDataView.AbstractTypes type, String text,
                                                        bool bFormula, bool bMolweight, bool base64, 
                                                        bool bListInputOK, bool bCustomOperatorOK,
                                                        SearchCriteria.COEOperators op)
        {
            // create search criteria item from field type or fmla/molwt flags, plus text of query
            SearchCriteria.ISearchCriteriaBase criterium = null;
            bool bIsInteger = type == COEDataView.AbstractTypes.Integer;
            bool bIsNumeric = bIsInteger || type == COEDataView.AbstractTypes.Real ;

            try
            {
                if (bFormula)
                {
                    criterium = new SearchCriteria.CSFormulaCriteria();
                    ((SearchCriteria.CSFormulaCriteria)criterium).Formula = text.Trim();
                    ((SearchCriteria.CSFormulaCriteria)criterium).Implementation = "cscartridge";
                }
                else if (bMolweight)
                {
                    criterium = new SearchCriteria.CSMolWeightCriteria();
                    ((SearchCriteria.CSMolWeightCriteria)criterium).Value = text;
                    ((SearchCriteria.CSMolWeightCriteria)criterium).Implementation = "cscartridge";
                }
                else if (base64)
                {
                    criterium = new SearchCriteria.StructureCriteria();
                    ((SearchCriteria.StructureCriteria)criterium).Structure = text;
                    ((SearchCriteria.StructureCriteria)criterium).Implementation = "cscartridge";
                }
                else if (bListInputOK && text.StartsWith("@"))
                {
                    // list of values supplied in text file
                    criterium = MakeCriteriumFromFile(bIsNumeric, bIsInteger, text);
                }
                else if (bListInputOK && IsListOfValues(bIsNumeric, text))
                {
                    // comma-delimited list of numeric or text values; numerics allow ranges
                    criterium = MakeCriteriumFromListOfValues(bIsNumeric, bIsInteger, text);
                }
                else
                {
                    switch (type)
                    {
                        case COEDataView.AbstractTypes.Date:
                            criterium = new SearchCriteria.DateCriteria();
                            ((SearchCriteria.DateCriteria)criterium).InnerText = text;
                            ((SearchCriteria.DateCriteria)criterium).Operator = bCustomOperatorOK? op : SearchCriteria.COEOperators.EQUAL;
                            ((SearchCriteria.DateCriteria)criterium).Culture = CultureInfo.CurrentCulture.Name;
                            break;
                        case COEDataView.AbstractTypes.Integer:
                        case COEDataView.AbstractTypes.Real:
                            criterium = new SearchCriteria.NumericalCriteria();
                            ((SearchCriteria.NumericalCriteria)criterium).InnerText = text;
                            ((SearchCriteria.NumericalCriteria)criterium).Operator = bCustomOperatorOK ? op : SearchCriteria.COEOperators.EQUAL;
                            break;
                        case COEDataView.AbstractTypes.Boolean:
                        case COEDataView.AbstractTypes.Text:
                            criterium = new SearchCriteria.TextCriteria();
                            ((SearchCriteria.TextCriteria)criterium).InnerText = text;
                            ((SearchCriteria.TextCriteria)criterium).CaseSensitive = SearchCriteria.COEBoolean.No;
                            ((SearchCriteria.TextCriteria)criterium).DefaultWildCardPosition = SearchCriteria.Positions.Both;
                            ((SearchCriteria.TextCriteria)criterium).Operator = bCustomOperatorOK ? op : SearchCriteria.COEOperators.LIKE;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exceptions.SearchException(String.Concat("Invalid expression on search criterium. '", text, "' not recognized."), ex);
            }
            return criterium;
        }
        //---------------------------------------------------------------------
        public static bool IsSubformField(String fieldName)
        {
            return fieldName.Contains(":");
        }
        //---------------------------------------------------------------------
        public static COEDataView.DataViewTable GetSubformTable(FormDbMgr formDbMgr, ref String fieldName)
        {
            String tableName = CBVUtil.BeforeDelimiter(fieldName, ':');
            fieldName = CBVUtil.AfterDelimiter(fieldName, ':');
            COEDataView.DataViewTable t = formDbMgr.FindDVTableByName(tableName);
            return t;
        }
        
        #region Search
        public virtual void Run()
        {
            // execute search using gathered criteria
            // result is searchResponse with dataset for caller to use

            // modify rc so it does not ask for highlighting if query is exact/full/similar
            CheckStructureHighlighting();

            // if query has sort attached, execute it by modifying RC
            ApplySortIfAny();

            try
            {
                // new 1/11: hitlist query, just restore given list, already saved
                bool bFilterChildHits = m_formDbMgr.FilterChildHits && this.HasChildField;
                if (this.IsHitlistQuery)
                {
                    CBVStatMessage.Show("Restoring hitlist " + m_hitListID.ToString());
                    bool bIsSaved = this.IsSaved;
                    m_pager.DoRetrieveHitlist(m_hitListID, this.NumHits, bIsSaved, bFilterChildHits);
                    CBVStatMessage.Hide();  // CSBR-135536
                    return;
                }

                // if we have a parent, run over the hitlist of that
                if (this.HasParentQuery) {
                    Query qParent = this.QueryCollection.FindByID(this.ParentQueryID);
                    if (qParent != null) {
                        RunOverCurrentList(qParent);
                        return;
                    }
                }
                CBVStatMessage.Show("Running query " + this.Description);
                m_pager.DoSearch(SearchCriteria, null, bFilterChildHits);
                this.m_numHits = m_pager.ListSize;
                this.m_hitListID = m_pager.HitListID;
                this.IsSaved = m_pager._HitListType == HitListType.SAVED;
                CBVStatMessage.Hide();
            }
            catch (FormDBLib.Exceptions.SearchException ex)
            {
                throw ex;
            }
        }
        //---------------------------------------------------------------------
        public virtual void RunOverCurrentList(Query qCurr)
        {
            // execute search using gathered criteria; restrict hits to given list
            try
            {
                CBVStatMessage.Show("Running query " + this.Description + " over domain " + qCurr.Name);

                this.ParentQueryID = qCurr.ID;

                HitListInfo domainHLInfo = new HitListInfo();
                domainHLInfo.HitListID = qCurr.HitListID;
                domainHLInfo.HitListType = qCurr.IsSaved? HitListType.SAVED : HitListType.TEMP;
                domainHLInfo.RecordCount = qCurr.NumHits;

                bool bFilterChildHits = m_formDbMgr.FilterChildHits && this.HasChildField;
                m_pager.DoSearch(SearchCriteria, domainHLInfo, bFilterChildHits);
                this.m_numHits = m_pager.ListSize;
                this.m_hitListID = m_pager.HitListID;
                this.IsSaved = m_pager._HitListType == HitListType.SAVED;
                CBVStatMessage.Hide();
            }
            catch (FormDBLib.Exceptions.SearchException ex)
            {
                throw ex;
            }
        }
        //---------------------------------------------------------------------
        public void RestoreHitlist()
        {
            // retrieve hitlist from hitlist id
            // if we have none, or retrieve fails (as on stale saved list), then do a rerun (CSBR-128228)

            bool bMustRerun = (HitListID == 0);
            if (!bMustRerun)
            {
                try
                {
                    // as in Run, apply sort if any, turn highlighting on or off depending on sc
                    CheckStructureHighlighting();
                    ApplySortIfAny();

                    bool bFilterChildHits = m_formDbMgr.FilterChildHits && this.HasChildField;
                    m_pager.DoRetrieveHitlist(this.m_hitListID, this.m_numHits, this.IsSaved, bFilterChildHits);
                    // CSBR-115608: leave existing numHits, do not set it to retrieved page size
                    this.m_hitListID = m_pager.HitListID;
                    if (this.IsSaved)
                        this.SaveFlag = true;
                    else
                        this.SaveFlag = false;
                }
                catch (FormDBLib.Exceptions.NoHitsException)
                {
                    bMustRerun = true;
                }
            }
            if (bMustRerun)
            {
                Run();
            }
        }
        //---------------------------------------------------------------------
        public void SaveHitlist()
        {
            // change hitlist to permanent saved list in db; set IsSavedHitlist flag after saving
            // do nothing if no hitlist or already saved
            if (this.m_hitListID == 0 || this.IsSaved)
                return;

            int oldHitListID = this.m_hitListID;
            COEHitListBO hlbo = COEHitListBO.Get(HitListType.TEMP, oldHitListID);

            // new 9/11: save name and comments with hitlist
            hlbo.Name = this.Name;
            hlbo.Description = this.Description;

            COEHitListBO hlboSaved = hlbo.Save();
            HitListInfo hlinfoSaved = (hlboSaved == null) ? null : hlboSaved.HitListInfo;
            int newHitListID = (hlinfoSaved == null) ? 0 : hlinfoSaved.HitListID;

            if (newHitListID == 0)
                return; // throw?

            this.HitListID = newHitListID;
            this.Flag(QueryFlag.kfIsSaved, true);

            Debug.WriteLine(String.Format("Saved hitlist, tempID={0} permID={1}", oldHitListID, newHitListID));

            // add notation to description, like "[saved]" ?

#if DEBUG
            HitListInfo hlinfo = hlbo.HitListInfo;
            COEHitListBO hlboRetrieved = COEHitListBO.Get(HitListType.SAVED, newHitListID);
            HitListInfo hlinfoRetrieved = hlboRetrieved.HitListInfo;
#endif
        }
        //---------------------------------------------------------------------
        public void DeleteSavedHitlist()
        {
            try
            {
                if (this.HitListID == 0 || !this.IsSaved)
                    return;
                COEHitListBO hlbo = COEHitListBO.Get(HitListType.SAVED, this.HitListID);
                if (hlbo != null)
                    hlbo.Delete();
                this.HitListID = 0;
                this.IsSaved = false;   // clears flag
            }
            catch (Exception ex)
            {
                CBVUtil.ReportError(ex);
            }
        }
        //---------------------------------------------------------------------
        #endregion
        #endregion
    }
    #endregion

    #region RetrieveAllQuery class
    public class RetrieveAllQuery : Query
    {
        #region Constructor
        public RetrieveAllQuery(FormDbMgr formDbMgr, QueryCollection qColl)
			: base(formDbMgr, qColl)
		{
			m_description = formDbMgr.TableName;
        }
        #endregion

        #region Methods
        public override bool CanRerun(Control form)
        {
            return true;
        }
        //---------------------------------------------------------------------
        public override bool CanRestoreHitlist
        {
            get { return true; }
        }
        //---------------------------------------------------------------------
        public override bool CanRestoreToForm(Control form)
        {
            return false;
        }
        //---------------------------------------------------------------------
        public override void Run()
		{
			// simulate a search: prepare a search response
            CheckStructureHighlighting();
            ApplySortIfAny();
            m_pager.DoRetrieveAll();
			this.m_numHits = m_formDbMgr.BaseTableRecordCount;
            this.m_hitListID = 0;
        }
        #endregion
    }
    #endregion

    #region MergeQuery class
    public class MergeQuery : Query
    {
        #region Variables & Enums
        public enum LogicChoice { kmUnknown = 0, kmIntersect, kmSubtract /*q1-q2*/, kmUnion, kmSubtractFrom /*q2-q1*/ };

        private Query m_q1, m_q2;
        private int m_q1ID, m_q2ID;
        private LogicChoice m_logic;
        #endregion

        #region Constructors
        public MergeQuery(FormDbMgr formDbMgr, QueryCollection qColl)
            : base(formDbMgr, qColl)
        {
            // this version for serialization
            m_q1 = null;
            m_q2 = null;
            m_logic = LogicChoice.kmUnknown;
        }
        //---------------------------------------------------------------------
        public MergeQuery(FormDbMgr formDbMgr, Query q1, Query q2, LogicChoice logic, QueryCollection qColl)
            : base(formDbMgr, qColl)
        {
            m_q1 = q1;
            m_q2 = q2;
            m_logic = logic;

            m_q1ID = q1.ID;
            m_q2ID = q2.ID;
            m_description = CreateDescriptor();
        }
        #endregion

        #region Properties
        public override bool IsMergeQuery
        {
            get { return true; }
        }
        //---------------------------------------------------------------------
        public Query Query1
        {
            get { return m_q1; }
        }
        //---------------------------------------------------------------------
        public Query Query2
        {
            get { return m_q2; }
        }
        #endregion

        #region Methods
        private String CreateDescriptor()
        {
            // call after we have two valid query objects
            LookupComponents(this.QueryCollection);
            if (m_q1 == null || m_q2 == null)
                return "[Invalid merge]";

            String s = "", s1Name = m_q1.Name, s2Name = m_q2.Name;
            switch (m_logic)
            {
                case LogicChoice.kmIntersect: s = String.Concat(s1Name, " AND ", s2Name); break;
                case LogicChoice.kmSubtract: s = String.Concat(s1Name, " MINUS ", s2Name); break;
                case LogicChoice.kmUnion: s = String.Concat(s1Name, " OR ", s2Name); break;
                case LogicChoice.kmSubtractFrom: s = String.Concat(s2Name, " MINUS ", s1Name); break;
            }
            return s;
        }
        //---------------------------------------------------------------------
        public void LookupComponents(QueryCollection qcoll)
        {
            // given query IDs, find queries themselves
            m_q1 = qcoll.FindByID(m_q1ID);
            m_q2 = qcoll.FindByID(m_q2ID);
        }
        //---------------------------------------------------------------------
        public static String LogicNameFromChoice(LogicChoice logic)
        {
            switch (logic)
            {
                case LogicChoice.kmIntersect: return "and";
                case LogicChoice.kmSubtract: return "minus";
                case LogicChoice.kmUnion: return "or";
                case LogicChoice.kmSubtractFrom: return "from";
            }
            return "";
        }
        //---------------------------------------------------------------------
        public static LogicChoice LogicChoiceFromName(String name)
        {
            if (name.Equals("and")) return LogicChoice.kmIntersect;
            if (name.Equals("minus")) return LogicChoice.kmSubtract;
            if (name.Equals("or")) return LogicChoice.kmUnion;
            if (name.Equals("from")) return LogicChoice.kmSubtractFrom;
            return LogicChoice.kmUnknown;
        }
        //---------------------------------------------------------------------
        public override void Run()
        {
            // first make sure both components exist and have hitlists
            Debug.Assert(m_formDbMgr.SelectedDataView != null);
            Debug.Assert(m_formDbMgr.ResultsCriteria != null);

            if (m_q1 == null || m_q2 == null)
                LookupComponents(m_queryCollection);

            if (m_q1 == null || m_q2 == null)
                throw new FormDBLib.Exceptions.NoHitsException();
            try
            {
                // run queries if no hitlist
                if (m_q1.HitListID == 0 && !(m_q1 is RetrieveAllQuery))
                    m_q1.Run();
                if (m_q2.HitListID == 0 && !(m_q2 is RetrieveAllQuery))
                    m_q2.Run();

                if ((m_q1.HitListID == 0 && !(m_q1 is RetrieveAllQuery)) ||
                    (m_q2.HitListID == 0 && !(m_q2 is RetrieveAllQuery)))
                    throw new FormDBLib.Exceptions.NoHitsException();
            }
            catch (FormDBLib.Exceptions.SearchException ex)
            {
                throw ex;
            }

            // create hitlist objects needed for merge
            HitListInfo hl1 = new HitListInfo();
            hl1.HitListID = m_q1.HitListID;
            hl1.Database = m_formDbMgr.SelectedDataView.Database;
            hl1.RecordCount = m_q1.NumHits;
            hl1.HitListType = m_q1.IsSaved ? HitListType.SAVED : HitListType.TEMP;

            HitListInfo hl2 = new HitListInfo();
            hl2.HitListID = m_q2.HitListID;
            hl2.Database = m_formDbMgr.SelectedDataView.Database;
            hl2.RecordCount = m_q2.NumHits;
            hl2.HitListType = m_q2.IsSaved ? HitListType.SAVED : HitListType.TEMP;

            // pager does the actual merge
            int mergedHitListID = m_pager.DoMergeHitlists(hl1, hl2, m_logic);
            this.m_numHits = m_pager.ListSize;
            if (this.m_numHits == 0) //CSBR-111886
                throw new FormDBLib.Exceptions.NoHitsException();

            this.m_hitListID = mergedHitListID;
        }
        //---------------------------------------------------------------------
        public override XmlElement CreateXmlElement(XmlDocument xdoc, String eltname)
        {
            XmlElement element = base.CreateXmlElement(xdoc, eltname);
            element.SetAttribute("q1ID", CBVUtil.IntToStr(m_q1ID));
            element.SetAttribute("q2ID", CBVUtil.IntToStr(m_q2ID));
            element.SetAttribute("logic", LogicNameFromChoice(m_logic));
            return element;
        }
        //---------------------------------------------------------------------
        private int ConvertOldStrToNewID(String nameOld, int valNew)
        {
            // for back-compability: if old form has query name, convert to ID
            Query q = null;
            if (!String.IsNullOrEmpty(nameOld) && valNew == 0)
                q = this.QueryCollection.Find(nameOld);
            else if (valNew > 0)
                q = this.QueryCollection.FindByID(valNew);
            return (q == null) ? 0 : q.ID;
        }
        //---------------------------------------------------------------------
        public override void LoadXmlElement(XmlNode node)
        {
            base.LoadXmlElement(node);

            String q1NameOLD = CBVUtil.GetStrAttrib(node, "q1name");
            String q2NameOLD = CBVUtil.GetStrAttrib(node, "q2name");

            m_q1ID = ConvertOldStrToNewID(q1NameOLD, CBVUtil.GetIntAttrib(node, "q1ID"));
            m_q2ID = ConvertOldStrToNewID(q2NameOLD, CBVUtil.GetIntAttrib(node, "q2ID"));

            m_logic = LogicChoiceFromName(CBVUtil.GetStrAttrib(node, "logic"));
            m_description = CreateDescriptor();
        }
        //---------------------------------------------------------------------
        #endregion
    }
    #endregion

    #region SortData
    public class SortField
    {
        public String m_fieldName;
        public bool m_bAscending;

        public SortField(String fieldName, bool bAscending)
        {
            m_fieldName = fieldName;
            m_bAscending = bAscending;
        }
        //---------------------------------------------------------------------
    }
    public class SortData : List<SortField>
    {
        public static String SortDataToString(SortData sData)
        {
            String s = String.Empty;
            foreach (SortField sf in sData)
            {
                String sfs = sf.m_fieldName;
                if (!sf.m_bAscending)
                    sfs += " D";
                if (!String.IsNullOrEmpty(s))
                    s += ";";
                s += sfs;
            }
            return s;
        }
        //---------------------------------------------------------------------
        public static SortData StringToSortData(String sortStr)
        {
            SortData sData = new SortData();
            String[] toks = sortStr.Split(';');
            foreach (String tok in toks)
            {
                String fldName = tok;
                bool bAscending = true;
                if (CBVUtil.EndsWith(tok, " D"))
                {
                    fldName = tok.Substring(0, tok.Length - 2);
                    bAscending = false;
                }
                SortField sf = new SortField(fldName, bAscending);
                sData.Add(sf);
            }
            return sData;
        }
        //---------------------------------------------------------------------
    }
    #endregion


}
