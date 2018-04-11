using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.ComponentModel;
using CambridgeSoft.COE.Framework.Common;

using Greatis.FormDesigner;
using ChemControls;
using FormDBLib;
using CBVUtilities;
using CBVControls;
using Utilities;

using Infragistics.Win;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinGrid;

namespace ChemBioViz.NET
{
    /// <summary>
    ///   TabManager belongs to the doc (ChemBioVizForm)
    ///   manages a list of form/grid/query views; operates the form's tab control
    /// </summary>
    public class TabManager
    {
        #region Enums
        public enum TabType { ktUnknown = -1, ktForm, ktGrid, ktQuery };
        #endregion

        #region Variables
        private ChemBioVizForm m_form;
        private List<FormTab> m_tabs;
        private int m_selectedIndex;
        private int m_lastSelectedRow;
        private int m_lastSelectedTabIndex;
        private bool m_bClearQueryOnNextOpen;
        private string m_bindingError;
        #endregion

        #region Properties
        public bool ClearQueryOnNextOpen
        {
            get { return m_bClearQueryOnNextOpen; }
            set { m_bClearQueryOnNextOpen = value; }
        }
        //---------------------------------------------------------------------
        public String BindingError
        {
            get { return m_bindingError; }
            set { m_bindingError = value; }
        }
        //---------------------------------------------------------------------
        public List<FormTab> Tabs
        {
            get { return m_tabs; }
            set { m_tabs = value; }
        }
        //---------------------------------------------------------------------
        public ChemBioVizForm Form
        {
            get { return m_form; }
        }
        //---------------------------------------------------------------------
        public int LastSelectedRow
        {
            // row which was current at last tab change
            get { return m_lastSelectedRow; }
            set { m_lastSelectedRow = value; }
        }
        //---------------------------------------------------------------------
        public int SelectedIndex
        {
            get { return m_selectedIndex; }
            set { SelectTab(value); }
        }
        //---------------------------------------------------------------------
        public int Count
        {
            get { return m_tabs.Count; }
        }
        //---------------------------------------------------------------------
        public FormTab CurrentTab
        {
            get { return (m_selectedIndex >= 0 && m_selectedIndex < m_tabs.Count) ? GetTab(m_selectedIndex) : null; }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Constructors
        public TabManager(ChemBioVizForm form)
        {
            m_form = form;
            m_tabs = new List<FormTab>();
            m_selectedIndex = -1;
            m_lastSelectedRow = 0;
            m_lastSelectedTabIndex = -1;
        }
        #endregion

        #region Methods
        //---------------------------------------------------------------------
        // accessors
        //---------------------------------------------------------------------
        public static String TypeName(TabType type, bool bCapitalize)
        {
            switch (type)
            {
                case TabType.ktForm: return bCapitalize ? "Form" : "form";
                case TabType.ktGrid: return bCapitalize ? "Grid" : "grid";
                case TabType.ktQuery: return bCapitalize ? "Query" : "query";
            }
            return "";
        }
        //---------------------------------------------------------------------
        public static TabType Type(String typeName)
        {
            switch (typeName)
            {
                case "form": return TabType.ktForm;
                case "grid": return TabType.ktGrid;
                case "query": return TabType.ktQuery;
            }
            return TabType.ktUnknown;
        }
        //---------------------------------------------------------------------
        public FormTab GetTab(int i)
        {
            return (i >= 0 && i < Count) ? m_tabs[i] : null;
        }
        //---------------------------------------------------------------------
        public void AddTab(FormTab tab)
        {
            m_tabs.Add(tab);
        }
        //---------------------------------------------------------------------
        public int AddTab(TabType type, String name)
        {
            FormTab tab = CreateTab(type);
            //Coverity Bug Fix CID :13096 
            if (tab != null)
            {
                tab.Name = name;
                m_tabs.Add(tab);
            }
            return m_tabs.Count - 1;
        }
        //---------------------------------------------------------------------
        public void DeleteTab(int index)
        {
            Debug.Assert(index >= 0 && index < Count);
            FormTab tabToDelete = m_tabs[index];

            // CSBR-113332: move to different tab before deleting
            if (m_selectedIndex >= index)
            {
                int moveToTab = m_selectedIndex > 0 ? m_selectedIndex - 1 : 0;
                SelectTab(moveToTab);
            }

            // remove deleted control from form and tabmgr
            RemoveFromForm(tabToDelete);
            m_tabs.RemoveAt(index);
        }
        //---------------------------------------------------------------------
        public int FindTab(String name)
        {
            for (int i = 0; i < m_tabs.Count; ++i)
                if (CBVUtil.Eqstrs(m_tabs[i].Name, name))
                    return i;
            return -1;
        }
        //---------------------------------------------------------------------
        public FormTab FindTabByName(String name)
        {
            foreach (FormTab tab in m_tabs)
                if (CBVUtil.Eqstrs(name, tab.Name))
                    return tab;
            return null;
        }
        //---------------------------------------------------------------------
        public String GetUniqueTabName(String baseName)
        {
            String s = baseName;
            while (s.Length > 0 && char.IsDigit(s[s.Length - 1]))
                s = s.Substring(0, s.Length - 1);

            for (int i = 1; i < 99; ++i)
            {
                String sNew = String.Concat(s, i.ToString());
                if (FindTab(sNew) == -1)
                    return sNew;
            }
            return s;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// For any button on any form view, connect it with a column in the grid if fieldnames match
        /// </summary>
        /// <param name="cdGrid"></param>
        public void SetupButtonAndRichtextCols(ChemDataGrid cdGrid)
        {
            //this is not a good way to do this -- what if two buttons are bound to the same field
            foreach (FormTab tab in m_tabs)
            {
                if (tab.IsFormView() && !tab.IsQueryView())
                {
                    FormViewControl fvc = tab.Control as FormViewControl;
                    //Coverity Bug Fix CID :
                    if (fvc != null)
                    {
                        fvc.ConnectButtonsToGrid(cdGrid, false);
                        fvc.CopyFormDataToGridCols(cdGrid);     // copy format info from form boxes to grid cols
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        public CBVButton FindButtonByName(String name)
        {
            List<CBVButton> buttons = GetButtonList(false);
            foreach (CBVButton button in buttons)
                if (CBVUtil.Eqstrs(button.Name, name))
                    return button;
            return null;
        }
        //---------------------------------------------------------------------
        public List<CBVButton> GetButtonList(bool bForMenu)
        {
            List<CBVButton> buttons = new List<CBVButton>();
            foreach (FormTab tab in m_tabs)
            {
                if (tab.IsFormView())
                {
                    FormViewControl fvc = tab.Control as FormViewControl;
                    //COverity Bug Fix CID 13004 
                    if (fvc != null)
                    {
                        foreach (Control c in fvc.Controls)
                            if (c is CBVButton && (!bForMenu || (c as CBVButton).ShowOnMenu))
                                buttons.Add(c as CBVButton);
                    }
                }
            }
            return buttons;
        }
        //---------------------------------------------------------------------
        public List<String> GetMenuNames()
        {
            List<String> menuNames = new List<String>();

            if (Form.FeatEnabler.CanCreateActionMenus())
            {
                List<CBVButton> buttons = GetButtonList(true);
                foreach (CBVButton button in buttons)
                {
                    if (!menuNames.Contains(button.MenuName))
                        menuNames.Add(button.MenuName);
                }
            }
            return menuNames;
        }
        //---------------------------------------------------------------------
        public void Clear()
        {
            RemoveFromForm();
            m_tabs.Clear();
            m_selectedIndex = -1;
        }
        //---------------------------------------------------------------------
        public void AddToForm(FormTab tab)
        {
            Form.FillPanel.Controls.Add(tab.Control);
        }
        //---------------------------------------------------------------------
        public void AddToForm()
        {
            foreach (FormTab tab in m_tabs)
                Form.FillPanel.Controls.Add(tab.Control);
        }
        //---------------------------------------------------------------------
        public void RemoveFromForm(FormTab tab)
        {
            Form.FillPanel.Controls.Remove(tab.Control);
        }
        //---------------------------------------------------------------------
        public void RemoveFromForm()
        {
            foreach (FormTab tab in m_tabs)
                Form.FillPanel.Controls.Remove(tab.Control);
        }
        //---------------------------------------------------------------------
        public String NewTabName(TabType type)
        {
            // return next available name
            String root = TypeName(type, true), candName = "";
            if (CBVUtil.Eqstrs(root, "Grid"))
                root = "Table";

            for (int i = 1; i < 999; ++i)
            {
                candName = String.Concat(root, i.ToString());
                if (FindTabByName(candName) == null)
                    break;
            }
            return candName;
        }


        #region high-level calls, managing app interaction with tabbed form
        public void CreateBlank(TabType type, String name)
        {
            // create blank tab
            // used on Add Blank from context menu
            int newTabNo = AddTab(type, name);
            AddToForm(GetTab(newTabNo));
            BuildTabControl();
            SelectTab(newTabNo);
        }
        //---------------------------------------------------------------------
        public void CreateDuplicate(FormTab sourceTab)
        {
            // create new tab as a copy of given tab
            // used on Add Duplicate from context menu
            FormViewControl sourceForm = sourceTab.Control as FormViewControl;
            if (sourceForm == null) return;
            if (sourceTab != this.CurrentTab)
                sourceForm.Visible = true;  // CSBR-128312: otherwise clone has all boxes invisible

            String newTabName = NewTabName(sourceTab.TabType);
            int newTabNo = AddTab(sourceTab.TabType, newTabName);
            FormTab newTab = GetTab(newTabNo);

            sourceForm.ScrollToOrigin();    // CSBR-152358: otherwise clone cut off
            newTab.Control = sourceForm.Clone();

            AddToForm(newTab);
            BuildTabControl();
            SelectTab(newTabNo);
        }
        //---------------------------------------------------------------------
        public void CreateQueryTabFromForm(FormTab sourceTab)
        {
            if (!(sourceTab is FormViewTab)) return;
            FormViewControl sourceForm = sourceTab.Control as FormViewControl;
            if (sourceForm == null) return;

            String newTabName = NewTabName(TabType.ktQuery);
            int newTabNo = AddTab(TabType.ktQuery, newTabName);
            QueryViewTab newTab = GetTab(newTabNo) as QueryViewTab;
            //Coverity Bug Fix CID 
            if (newTab != null)
            {
                newTab.CreateFromFormView(sourceForm);
                AddToForm(newTab);
            }
            BuildTabControl();
            SelectTab(newTabNo);
        }
        //---------------------------------------------------------------------
        public void CreateDuplicateGrid(FormTab sourceTab)
        {
            // creates full grid from bindingsource, not duplicate
            GridViewTab newTab = new GridViewTab(sourceTab.TabManager);
            newTab.Name = NewTabName(TabType.ktGrid);

            AddTab(newTab);
            AddToForm(newTab);

            newTab.Bind(Form.CurrQuery, Form.BindingSource);
            //newTab.m_bRowHeightSet = false;   // attempt to force adjust ... doesn't work

            BuildTabControl();
            SelectTab(m_tabs.Count - 1);
        }
        //---------------------------------------------------------------------
        public void CreateGridFromForm(FormTab sourceTab)
        {
            // make a new grid tab with cols to match given form view
            if (!(sourceTab is FormViewTab)) return;
            FormViewControl sourceForm = sourceTab.Control as FormViewControl;
            if (sourceForm == null) return;

            GridViewTab newTab = new GridViewTab(sourceTab.TabManager);
            newTab.Name = NewTabName(TabType.ktGrid);
            AddTab(newTab);
            AddToForm(newTab);

            // bind to create all cols
            newTab.Bind(Form.CurrQuery, Form.BindingSource);

            // then hide those not on form
            ControlSwapperEx.HideGridColumnsNotInForm(newTab.CDGrid, sourceForm);

            BuildTabControl();
            SelectTab(m_tabs.Count - 1);
        }
        //---------------------------------------------------------------------
        public void Create3TBlank()
        {
            // create standard blank form with form/table/query tabs
            // used on File New or File Close
            Clear();
            AddTab(TabType.ktForm, "Form");
            AddTab(TabType.ktGrid, "Table");
            AddTab(TabType.ktQuery, "Query");
            AddToForm();
            BuildTabControl();
            SelectDefaultTab();
        }
        //---------------------------------------------------------------------
        public void Create3TFromDataSource(DataSet dataSet, String tableName, bool withSubforms)
        {
            // use dataset to create standard form/table/query tabs
            // this is the auto generator used when user chooses from dataview tree
            Create3TBlank();

            FormTab frmTab = null;

            frmTab = GetTab(0);
            if (frmTab != null)
            {
                FormViewControl fvc = frmTab.Control as FormViewControl;
                if (fvc != null)
                {
                    fvc.CreateFromDataSourceEx(dataSet, tableName, withSubforms);

                    QueryViewTab qvt = GetTab(2) as QueryViewTab;
                    //Coverity Bug Fix CID : 13009
                    if (qvt != null)
                        qvt.CreateFromFormView(fvc);
                }
            }
            // grid doesn't require any setup; this may change when grid info is serialized
            //ChemDataGrid cdg = GetTab(1).Control as ChemDataGrid;
            SelectDefaultTab();
        }
        //---------------------------------------------------------------------
        public void ReportBindingError()
        {
            String sErr = BindingError;
            if (!String.IsNullOrEmpty(sErr))
                MessageBox.Show(sErr, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        //---------------------------------------------------------------------
        public void Bind(Query q, BindingSource bs)
        {
            // attach all tabs to dataset
            // used after loading or auto-generating
            // form view binds to bindingsource; gridview to q->pager
            BindingError = String.Empty;
            if (bs == null && q != null && q.DataSet != null && q.DataSet.Tables.Count > 0)
            {
                bs = new BindingSource(q.DataSet, q.DataSet.Tables[0].TableName);
                Form.BindingSource = bs;
                Form.BindingSource.PositionChanged += new EventHandler(BindingSource_PositionChanged);
            }

            if (bs != null) // CSBR-113591
            {
                foreach (FormTab tab in m_tabs)
                    tab.Bind(q, bs);
                Form.BindingNavigator.BindingSource = bs;
                Form.DoMove(Pager.MoveType.kmFirst);

                Form.FireRecordsetChanged();
            }
        }
        //---------------------------------------------------------------------
        void BindingSource_PositionChanged(object sender, EventArgs e)
        {
            // hack! if bindingnav is changing the position behind the scenes, change it to the correct value
            // this helps fix CSBR-136866
            if (Form.BindingSource.Position != 0 && Form.BindingSource.Position != Form.Pager.CurrRowInPage)
                Form.BindingSource.Position = Form.Pager.CurrRowInPage;
        }
        //---------------------------------------------------------------------
        public bool IsEditingForm()
        {
            return CurrentTab != null && CurrentTab.IsEditingForm();
        }
        //---------------------------------------------------------------------
        public void BeginEdit(ToolboxControlEx toolbox, PropertyGrid propertyGrid)
        {
            // edit current tab: let tab determine how to edit
            Debug.Assert(CurrentTab != null);
            CurrentTab.BeginEdit(toolbox, propertyGrid);

            // if "editing" grid view, go back to normal view immediately
            if (!CurrentTab.IsFormView())
                Form.RestoreExplorerBar();
        }
        //---------------------------------------------------------------------
        public bool EndEdit(bool bCancel)
        {
            // terminate editing
            Debug.Assert(CurrentTab != null);
            return CurrentTab.EndEdit(bCancel);
        }
        //---------------------------------------------------------------------
        public void BeginQueryMode()
        {
            Debug.Assert(CurrentTab.IsQueryView());
            //Coverity Bug Fix CID: 13007
            if (CurrentTab is QueryViewTab)
                ((QueryViewTab)CurrentTab).ClearOnFirstView();    // CSBR-133385
        }
        //---------------------------------------------------------------------
        public void ClearQueryTab()
        {
            //Coverity Bug Fix CID 13008
            if (CurrentTab.IsQueryView() && CurrentTab is QueryViewTab)
                ((QueryViewTab)CurrentTab).ClearQueryForm();
        }
        //---------------------------------------------------------------------
        public void EndQueryMode()
        {
        }
        #endregion

        #region Management of tab set
        public FormTab CreateTab(TabType type)
        {
            switch (type)
            {
                case TabType.ktForm: return new FormViewTab(this);
                case TabType.ktGrid: return new GridViewTab(this);
                case TabType.ktQuery: return new QueryViewTab(this);
            }
            return null;
        }
        //---------------------------------------------------------------------
        public void BuildTabControl()
        {
            UltraTabControl tc = Form.TabControl;
            tc.Tabs.Clear();
            tc.MinimumSize = new Size(tc.Size.Width, tc.Size.Height);

            // CSBR-134875: do not assign default tab order here, it clobbers saved values

            foreach (FormTab tab in m_tabs)
            {
                UltraTab tpage = new UltraTab();
                tpage.Text = tab.Name;
                tc.Tabs.Add(tpage);
            }

            // CSBR-132766 (11/10): set visible order from values saved with tabmgr tabs
            foreach (UltraTab utab in tc.Tabs)
                if (m_tabs[utab.Index].VisibleIndex != -1)
                {
                    int newVisIndex = m_tabs[utab.Index].VisibleIndex;
                    if (newVisIndex < m_tabs.Count)
                        utab.VisibleIndex = newVisIndex;
                }

            if (m_selectedIndex == -1 && tc.Tabs.Count > 0)
                m_selectedIndex = 0;

            if (m_selectedIndex >= 0 && m_selectedIndex < tc.Tabs.Count)
                tc.Tabs.TabControl.SelectedTab = tc.Tabs[m_selectedIndex];
        }
        //---------------------------------------------------------------------
        public void SelectNamedTab(String name)
        {
            int index = FindTab(name);
            if (index != -1)
                SelectTab(index);
        }
        //---------------------------------------------------------------------
        public void SelectDefaultTab()
        {
            // go to default tab for browse mode: leftmost formview tab
            // better (CSBR-119752): last tab viewed before going into query mode
            if (m_lastSelectedTabIndex != -1)
            {
                SelectTab(m_lastSelectedTabIndex);
                return;
            }
            for (int i = 0; i < m_tabs.Count; ++i)
            {
                if (m_tabs[i].IsFormView() && !m_tabs[i].IsQueryView())
                {
                    SelectTab(i);
                    return;
                }
            }
        }
        //---------------------------------------------------------------------
        public void SaveTabOrdering(UltraTabControl tc)
        {
            for (int i = 0; i < this.Tabs.Count; ++i)
                Tabs[i].VisibleIndex = tc.Tabs[i].VisibleIndex;
        }
        //---------------------------------------------------------------------
        public void SelectTab(int index)
        {
            if (index < 0 || index >= m_tabs.Count)
                return;     // CSBR-133490 .. prevent crash

            CBVUtil.BeginUpdate(Form);

            // if we are going into query mode, remember where came from
            m_lastSelectedTabIndex = -1;
            if (CurrentTab != null && !CurrentTab.IsQueryView() && m_tabs[index].IsQueryView())
                m_lastSelectedTabIndex = m_selectedIndex;

            // deactivate current tab
            bool bChangingTabs = (index != m_selectedIndex);
            if (CurrentTab != null)
                CurrentTab.Unselect();

            // set visibility true for new choice only
            for (int i = 0; i < m_tabs.Count; ++i)
            {
                FormTab tab = m_tabs[i];
                tab.Show(i == index);
            }
            m_selectedIndex = index;

            // update tab control strip
            UltraTabControl tc = Form.TabControl;
            if (m_selectedIndex >= 0 && m_selectedIndex < tc.Tabs.Count)
                tc.Tabs.TabControl.SelectedTab = tc.Tabs[m_selectedIndex];

            // activate new current tab
            if (CurrentTab != null)
            {
                CurrentTab.Select();
                //coverity Bug Fix CID 13100 
                Control control = CurrentTab.Control;
                if(control != null)
                {
                    control.Focus();
                    // new for checkmarks: rebind to refresh data
                    bool bRebindTab = false;
                    ChemDataGrid chemDatagrid = control as ChemDataGrid;
                    if (bChangingTabs && CurrentTab.IsGridView() && chemDatagrid != null && chemDatagrid.HasMarkedColumn)
                        // TO DO: should do this only if a check has been modified
                        bRebindTab = true;
                    if (bRebindTab)
                        CurrentTab.Bind(Form.CurrQuery, Form.BindingSource);
                }
            }
            CBVUtil.EndUpdate(Form);

        }
        #endregion

        #region Serialization
        public XmlElement CreateXmlElement(XmlDocument xdoc, String eltname)
        {
            XmlElement xmlTabs = xdoc.CreateElement(eltname);
            xmlTabs.SetAttribute("selected", this.SelectedIndex.ToString());

            // CSBR-135582: freeze display, then mark each tab as visible before saving
            FormTab frontTab = CurrentTab;
            CBVUtil.LockWindowUpdate(this.Form.Handle);
            foreach (FormTab tab in m_tabs)
            {
                //Coverity Bug Fix CID 13101 
                Control control = tab.Control;
                if (control != null)
                {
                    control.Visible = true;
                }
                XmlElement xmlTab = tab.CreateXmlElement(xdoc);
                xmlTabs.AppendChild(xmlTab);
                if (control != null && tab != frontTab)
                    control.Visible = false;
            }
            CBVUtil.LockWindowUpdate(IntPtr.Zero);

            return xmlTabs;
        }
        //---------------------------------------------------------------------
        public void LoadXmlElement(XmlNode node)
        {
            // load all tabs
            m_tabs = new List<FormTab>();

            m_selectedIndex = CBVUtil.GetIntAttrib(node, "selected");
            foreach (XmlNode tabNode in node.ChildNodes)
            {
                if (tabNode.Name.Equals("tab"))
                {
                    String typeName = CBVUtil.GetStrAttrib(tabNode, "type");
                    TabType type = TabManager.Type(typeName);
                    Debug.Assert(type != TabType.ktUnknown);

                    FormTab t = CreateTab(type);
                    t.LoadXmlElement(tabNode);
                    AddTab(t);
                }
            }
        }
        #endregion
        #endregion
    }

    /// <summary>
    ///   BASE FOR TAB CLASSES
    /// </summary>
    public class FormTab
    {
        #region Variables
        protected TabManager m_tabMgr;
        protected TabManager.TabType m_type;
        protected String m_name;
        protected int m_visibleIndex;
        #endregion

        #region Properties
        public ChemBioVizForm Form
        {
            get { return m_tabMgr.Form; }
        }
        //---------------------------------------------------------------------
        public virtual Control Control
        {
            get { return null; }
            set { }
        }
        //---------------------------------------------------------------------
        public String BindingError
        {
            get { return TabManager.BindingError; }
            set { TabManager.BindingError = value; }
        }
        //---------------------------------------------------------------------
        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        //---------------------------------------------------------------------
        public int VisibleIndex
        {
            get { return m_visibleIndex; }
            set { m_visibleIndex = value; }
        }
        //---------------------------------------------------------------------
        public TabManager TabManager
        {
            get { return m_tabMgr; }
        }
        //---------------------------------------------------------------------
        public TabManager.TabType TabType
        {
            get { return m_type; }
        }
        //---------------------------------------------------------------------
        public FormDbMgr FormDbMgr
        {
            get { return m_tabMgr.Form.FormDbMgr; }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Constructors
        public FormTab(TabManager tabMgr)
        {
            m_tabMgr = tabMgr;
            m_visibleIndex = -1;
        }
        #endregion

        #region Methods
        //---------------------------------------------------------------------
        public bool IsFormView()
        {
            return m_type == TabManager.TabType.ktForm || m_type == TabManager.TabType.ktQuery;
        }
        //---------------------------------------------------------------------
        public bool IsQueryView()
        {
            return m_type == TabManager.TabType.ktQuery;
        }
        //---------------------------------------------------------------------
        public bool IsGridView()
        {
            return m_type == TabManager.TabType.ktGrid;
        }
        //---------------------------------------------------------------------
        public virtual bool IsEditingForm()
        {
            return false;
        }
        //---------------------------------------------------------------------
        public virtual void BeginEdit(ToolboxControlEx toolbox, PropertyGrid propertyGrid)
        {
        }
        //---------------------------------------------------------------------
        public virtual bool EndEdit(bool bCancel)
        {
            return true;
        }
        //---------------------------------------------------------------------
        public virtual void Select()
        {
        }
        //---------------------------------------------------------------------
        public virtual void Unselect()
        {
        }
        //---------------------------------------------------------------------
        public virtual void Bind(Query q, BindingSource bs)
        {
        }
        //---------------------------------------------------------------------
        public virtual void Show(bool bShow)
        {
            if (Control != null)
                Control.Visible = bShow;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Set the tab name
        /// </summary>
        /// <param name="node"></param>
        public virtual void LoadXmlElement(XmlNode node)
        {
            Name = CBVUtil.GetStrAttrib(node, "name");
            String sVis = CBVUtil.GetStrAttrib(node, "vis_index");
            if (!String.IsNullOrEmpty(sVis))
                VisibleIndex = CBVUtil.StrToInt(sVis);
        }
        //---------------------------------------------------------------------
        public virtual XmlElement CreateXmlElement(XmlDocument xdoc)
        {
            XmlElement xmlTab = xdoc.CreateElement("tab");
            xmlTab.SetAttribute("type", TabManager.TypeName(TabType, false));
            xmlTab.SetAttribute("name", Name);
            if (VisibleIndex != -1)
                xmlTab.SetAttribute("vis_index", CBVUtil.IntToStr(VisibleIndex));
            return xmlTab;
        }
        //---------------------------------------------------------------------
        #endregion
    }

    /// <summary>
    ///  FORM VIEW TAB
    /// </summary>
    public class FormViewTab : FormTab
    {
        #region Variables
        protected FormViewControl m_formView;
        protected FormViewControl m_formViewCopyDuringEdit;
        #endregion

        #region Properties
        public override Control Control
        {
            get { return m_formView; }
            set { m_formView = value as FormViewControl; }
        }
        #endregion

        #region Constructors
        public FormViewTab(TabManager tabMgr)
            : base(tabMgr)
        {
            m_type = TabManager.TabType.ktForm;

            m_formView = new FormViewControl(tabMgr.Form);
            m_formView.Dock = System.Windows.Forms.DockStyle.Fill;
            m_formView.Visible = true;
            m_formViewCopyDuringEdit = null;
        }
        #endregion

        #region Methods
        public override bool IsEditingForm()
        {
            return m_formView.IsEditingForm();
        }
        //---------------------------------------------------------------------
        public bool CanRevertEdits()
        {
            return m_formViewCopyDuringEdit != null;
        }
        //---------------------------------------------------------------------
        public void RevertEdits()
        {
            Form.Modified = true;   // CSBR-134541
            EndEdit(true);  // just like Cancel
        }
        //---------------------------------------------------------------------
        private List<String> m_savedInfraGridLayouts;

        public override void BeginEdit(ToolboxControlEx toolbox, PropertyGrid propertyGrid)
        {
            // save layout info for each subform grid, in case of cancel
            //Coverity Bug Fix CID 12987  
            if (m_formView != null)
            {
                m_savedInfraGridLayouts = ControlSwapperEx.SaveInfraGridLayouts(m_formView);

                m_formView.ScrollToOrigin();

                m_formViewCopyDuringEdit = m_formView.Clone();  // loses infra column info
                m_formView.BeginFormEdit(toolbox, propertyGrid, this.IsQueryView());
            }
        }
        //---------------------------------------------------------------------
        public override bool EndEdit(bool bCancel)
        {
            if (bCancel)
            {
                // swap out edited form for copy made above
                m_formView.CancelFormEdit();
                Control cParent = m_formView.Parent;
                if (cParent != null)
                {
                    try
                    {
                        cParent.Controls.Remove(m_formView);    // fails if binding problem
                        m_formView = m_formViewCopyDuringEdit;

                        // restore layouts to subform grids
                        ControlSwapperEx.LoadInfraGridLayouts(m_formView, m_savedInfraGridLayouts);
                        cParent.Controls.Add(m_formView);

                        m_formView.UpdatePlots(false);

                        m_formViewCopyDuringEdit = null;    // prevent reverting to this state again
                        m_formView.RefreshButtonLabels();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
            else
            {
                if (m_formView.EndFormEdit(false))
                    Form.Modified = true;
                else
                    return false;
            }
            RestoreAcceptCancelButtons();   // CSBR-144127
            return true;
        }
        //---------------------------------------------------------------------
        public override void Select()
        {
            // form tab now in front; fill from selected row
            Form.ShowSearchButtons(false);
            Form.DoMove(Pager.MoveType.kmGoto, this.TabManager.LastSelectedRow);
        }
        //---------------------------------------------------------------------
        public override void Unselect()
        {
            // on unselecting bound form tab, make a note of the current row
            if (Form.CurrQuery != null)
                this.TabManager.LastSelectedRow = Form.CurrQuery.Pager.CurrRow;
        }
        //---------------------------------------------------------------------
        public override void Bind(Query q, BindingSource bs)
        {
            Exception ex = null;
            m_formView.BindToDataSource(bs, ref ex);
            if (ex != null)
                BindingError = ex.Message;

            ControlSwapperEx swapper = new ControlSwapperEx(m_formView);
            swapper.AdjustColumnsAfterBind();

            // for each subform, check for buttons on formview bound to subform field
            m_formView.SetupSubformButtonCols();

            m_formView.CheckForChemDraw();  // CSBR-118054 .. for structures in subforms

            /* note found on forum:
             *    A really good place to set column widths and positions is the InitializeLayout event of the grid. 
             *    This event will fire any time the grid gets a reset notification or is bound to a data source, 
             *    so it ensures that the layout is applied after the data source is established.  
             *    Also, why are you saving the colmun widths yourself. There are Save and Load methods on the 
             *    grid's DisplayLayout that will do this for you. 
            */
        }
        //---------------------------------------------------------------------
        public static String GreatisToXml(FormViewControl formView, ChemBioVizForm form, bool bDoUnbind, bool bIsQueryTab)
        {
            // let Greatis convert form to xml
            // use Text property to store app.table with formview
            // bDoBindAndUnbind true => remove bindings before convert, restore after;
            // is false for query forms and one-grid forms during serialize
            if (formView.Controls.Count == 0)
                return "";  // CSBR-109885

            formView.Text = String.Concat(form.FormDbMgr.AppName, ".", form.FormDbMgr.TableName);

            // CSBR-135168: PrepForWrite->SortByTabIndex->Clear removes Accept/Cancel button defs
            IButtonControl cAccept = formView.AcceptButton, cCancel = formView.CancelButton;
            if (cAccept != null && cAccept is CBVButton)
                (cAccept as CBVButton).IsAcceptButton = true;
            if (cCancel != null && cCancel is CBVButton)
                (cCancel as CBVButton).IsCancelButton = true;

            ControlSwapperEx swapper = new ControlSwapperEx(formView);
            swapper.PrepForWrite();

            // remove data bindings during save
            if (bDoUnbind)
                formView.Unbind();

            // create a temporary designer
            Designer designerT = new Designer();
            Treasury treasuryT = new Treasury();
            designerT.FormTreasury = treasuryT;
            designerT.DesignedForm = formView;

            // let the designer generate xml
            String s = designerT.LayoutXML;

            swapper.RestoreAfterWrite();

            formView.AcceptButton = cAccept;
            formView.CancelButton = cCancel;

            // CSBR-134702: restore bindings if removed (but only for form tabs)
            if (bDoUnbind && !bIsQueryTab)
                formView.Rebind();

            return s;
        }
        //---------------------------------------------------------------------
        public override XmlElement CreateXmlElement(XmlDocument xdoc)
        {
            XmlElement xmlTab = base.CreateXmlElement(xdoc);
            XmlElement xmlGreatis = xdoc.CreateElement("greatis");
            bool bIsQueryTab = this.IsQueryView();
            xmlGreatis.InnerXml = GreatisToXml(m_formView, Form, true, bIsQueryTab); // t=> unbind
            xmlTab.AppendChild(xmlGreatis);
            return xmlTab;
        }
        //---------------------------------------------------------------------
        private void AdjustBoxLocations()
        {
            // CSBR-110420: if any boxes have negative Y, it means form was saved in scrolled position
            // offset boxes so all have positive Y
            int minY = 99999;
            foreach (Control c in m_formView.Controls)
            {
                if (c.Location.Y < minY)
                    minY = c.Location.Y;
            }
            if (minY >= 0)
                return;

            int offset = -minY;
            foreach (Control c in m_formView.Controls)
            {
                c.Location = new Point(c.Location.X, c.Location.Y + offset);
            }
        }
        //---------------------------------------------------------------------
        private void RestoreAcceptCancelButtons()
        {
            foreach (Control c in m_formView.Controls)
            {
                if (c is CBVButton)
                {
                    if ((c as CBVButton).IsAcceptButton)
                        m_formView.AcceptButton = c as IButtonControl;
                    else if ((c as CBVButton).IsCancelButton)
                        m_formView.CancelButton = c as IButtonControl;
                }
            }
        }
        //---------------------------------------------------------------------
        private String UpdateChemControlsItems(String sOrig)
        {
            // replace phrases      assembly="ChemControls.ChemDraw ... ">
            // with                 assembly="ChemControls.ChemDraw, ChemControls">
            String s = sOrig;

            String tfD = "assembly=\"ChemControls.ChemDraw";
            String tfF = "assembly=\"ChemControls.ChemFormulaBox";
            String trD = String.Concat(tfD, ", ChemControls\">");
            String trF = String.Concat(tfF, ", ChemControls\">");

            if (s.Contains(tfD)) s = CBVUtil.ReplacePhrase(s, tfD, "\">", trD);
            if (s.Contains(tfF)) s = CBVUtil.ReplacePhrase(s, tfF, "\">", trF);

            return s;
        }
        //---------------------------------------------------------------------
        public override void LoadXmlElement(XmlNode node)
        {
            base.LoadXmlElement(node);
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name.Equals("greatis"))
                {
                    String sGreatisXml = childNode.InnerXml;
                    if (String.IsNullOrEmpty(sGreatisXml))
                        break;

                    Designer designerT = new Designer();
                    Treasury treasuryT = new Treasury();
                    designerT.FormTreasury = treasuryT;
                    designerT.FormTreasury.LoadMode = Greatis.FormDesigner.LoadModes.EraseForm;

                    Form.FillPanel.Tag = designerT;
                    designerT.DesignedForm = m_formView;

                    // HACK to read old forms using new signed ChemControls
                    // if form contains "ChemControls.X, ChemControls, Version=Y, Culture=neutral, PublicKeyToken=null"
                    // change to correct version and publickeytoken=cb0b5bb9e67d878a
                    // to be phased out when better scheme available
                    String sGreatisModified = UpdateChemControlsItems(sGreatisXml);

                    designerT.LayoutXML = sGreatisModified;

                    ControlSwapperEx swapper = new ControlSwapperEx(m_formView);
                    swapper.RestoreAfterRead(this.IsQueryView());

                    RestoreAcceptCancelButtons();
                    AdjustBoxLocations();
                    break;
                }
            }
        }
        //---------------------------------------------------------------------
        #endregion
    }

    /// <summary>
    ///   GRID VIEW TAB
    /// </summary>
    public class GridViewTab : FormTab
    {
        #region Variables
        private GridViewControl m_gridView;
        private bool m_bRowHeightSet;
        #endregion

        #region Properties
        public ChemDataGrid CDGrid
        {
            get { return m_gridView.ChemDataGrid; }
            set { m_gridView.ChemDataGrid = value as ChemDataGrid; }
        }
        //---------------------------------------------------------------------
        public override Control Control
        {
            get { return CDGrid; }
            set { CDGrid = value as ChemDataGrid; }
        }
        //---------------------------------------------------------------------
        public int CurrSelectedRowIndex
        {
            get { return (CDGrid.ActiveRow == null) ? 0 : CDGrid.ActiveRow.Index; }
        }
        //---------------------------------------------------------------------
        public bool BRowHeightSet
        {
            get { return m_bRowHeightSet; }
            set { m_bRowHeightSet = value; }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Constructors
        public GridViewTab(TabManager tabMgr)
            : base(tabMgr)
        {
            m_type = TabManager.TabType.ktGrid;

            ChemDataGrid cdGrid = new ChemDataGrid();
            cdGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            cdGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            cdGrid.DisplayLayout.Scrollbars = Infragistics.Win.UltraWinGrid.Scrollbars.Both;
            cdGrid.Visible = false;

            // CSBR-128662: do not add double-click or activate handlers here, is done in InstallGridEvents
            cdGrid.DisplayLayout.Override.AllowColSizing = AllowColSizing.Free;
            cdGrid.InitializeLayout += new InitializeLayoutEventHandler(cdGrid_InitializeLayout);

            m_gridView = new GridViewControl(tabMgr.Form);
            m_gridView.ChemDataGrid = cdGrid;
            m_bRowHeightSet = false;
            tabMgr.Form.InstallGridEvents(cdGrid);
        }

        public void cdGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            ChemDataGrid cdg = e.Layout.Grid as ChemDataGrid;
            if (cdg == null)
                return;
            Debug.Assert(cdg != null);
            {
                // select row on click cell
                e.Layout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;

                // except for checkbox column
                if (cdg.HasMarkedColumn)
                {
                    //Coverity fix - CID 13098
                    UltraGridColumn markedColumn = cdg.MarkedColumn;
                    if(markedColumn != null)
                        markedColumn.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.Edit;
                }
                e.Layout.AllowCardPrinting = AllowCardPrinting.RootBandOnly;

                List<string> bandKeys = cdg.GetAllBandKeys(e.Layout.Grid.DisplayLayout.Bands);
                //Coverity Bug Fix CID :13098 
                if (bandKeys != null)
                    AssignBandCaptions(bandKeys);
            }
        }
        #endregion

        #region Methods
        //---------------------------------------------------------------------
        public static void cdGrid_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            UltraGridCell c = e.Cell;
            if (c.Column.Style == Infragistics.Win.UltraWinGrid.ColumnStyle.URL && c.Column.Tag != null)
            {
                CBVButton button = c.Column.Tag as CBVButton;
                if (button != null)
                {
                    // new 9/11: pass clicked row and its parent rows to DoClickAction
                    RowStack rstack = FormUtil.GetRowStack(c);
                    rstack.Debug();
                    button.DoClickAction(e, rstack);
                }
            }
        }
        //---------------------------------------------------------------------
        private void cdGrid_AfterRowActivate(object sender, EventArgs e)
        {
            // if activating main row (not child row), move form and navigator
            ChemDataGrid cd_grid = sender as ChemDataGrid;
            //Coverity Bug Fix CID
            if (cd_grid != null && cd_grid.ActiveRow.Band == cd_grid.DisplayLayout.Bands[0]) // CSBR-114158
            {
                UltraGridRow activeRow = cd_grid.ActiveRow;
                int rowIndex = activeRow.Index;
                if (rowIndex != Form.Pager.CurrRow)
                    Form.DoMove(Pager.MoveType.kmGoto, rowIndex);
            }
        }
        //---------------------------------------------------------------------
        public override bool IsEditingForm()
        {
            return false;
        }
        //---------------------------------------------------------------------
        private ColumnChooserDialog columnChooserDialog = null;

        public override void BeginEdit(ToolboxControlEx toolbox, PropertyGrid propertyGrid)
        {
            CDGrid.BeforeColumnChooserDisplayed += new BeforeColumnChooserDisplayedEventHandler(CDGrid_BeforeColumnChooserDisplayed);
            //CSBR-154708. ColumnChooser band header caption is updated with table alias names.
            int i = 0;
            foreach (UltraGridBand band in CDGrid.DisplayLayout.Bands)
            {
                COEDataView.DataViewTable dvTable = (i++ == 0) ? this.FormDbMgr.SelectedDataViewTable :
                                                    this.Form.FormDbMgr.DSTableNameToDVTable(band.Key);
                if (dvTable != null)
                {
                    band.Header.Caption = String.IsNullOrEmpty(dvTable.Alias) ? dvTable.Name : dvTable.Alias;
                }
            }
            CDGrid.ShowColumnChooser();
            if (propertyGrid != null)
            {
                propertyGrid.SelectedObject = CDGrid;
                propertyGrid.Update();
            }
        }
        //---------------------------------------------------------------------
        void CDGrid_BeforeColumnChooserDisplayed(object sender, BeforeColumnChooserDisplayedEventArgs e)
        {
            e.Dialog.FormClosing += new FormClosingEventHandler(Dialog_FormClosing);
            columnChooserDialog = e.Dialog;
        }
        //---------------------------------------------------------------------
        void Dialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (columnChooserDialog != null)
                Form.EndFormEdit();
        }
        //---------------------------------------------------------------------
        public override bool EndEdit(bool bCancel)
        {
            if (columnChooserDialog != null && columnChooserDialog.Visible)
            {
                columnChooserDialog.FormClosing -= Dialog_FormClosing;
                columnChooserDialog.Close();
            }
            columnChooserDialog = null;
            return true;
        }
        //---------------------------------------------------------------------
        public override void Select()
        {
            Form.DoMove(Pager.MoveType.kmGoto, this.TabManager.LastSelectedRow);
        }
        //---------------------------------------------------------------------
        public override void Unselect()
        {
            // selected row is whatever user chose in grid
            this.TabManager.LastSelectedRow = this.CurrSelectedRowIndex;
        }
        //---------------------------------------------------------------------
        public void AdjustRowHeightsForStructs(ChemDataGrid cdGrid)
        {
            // must not do this until grid has rows!
            if (CDGrid.HasChemDrawColumn && CDGrid.Rows != null && CDGrid.Rows.Count > 0)
                CDGrid.Rows[0].Height = 60;
        }
        //---------------------------------------------------------------------
        public override void Show(bool bShow)
        {
            base.Show(bShow);
            if (bShow && CDGrid != null && !m_bRowHeightSet)
            {
                AdjustRowHeightsForStructs(CDGrid);
                m_bRowHeightSet = true;
            }
        }
        //---------------------------------------------------------------------
        private void AssignBandCaptions(List<string> bandKeys)
        {
            int i = 0;
            foreach (UltraGridBand band in CDGrid.DisplayLayout.Bands)
            {
                COEDataView.DataViewTable dvTable = (i == 0) ? this.FormDbMgr.SelectedDataViewTable :
                                                                this.Form.FormDbMgr.DSTableNameToDVTable(band.Key); // CSBR-161033: Incorrect Table names present in Table View with Grandchild tables.
                if (dvTable != null)
                {
                    band.HeaderVisible = true;
                    string headerName = dvTable.Alias;
                    if (string.IsNullOrEmpty(headerName))
                        headerName = dvTable.Name;
                    band.Header.Caption = headerName.ToUpper();
                }
                i++;
            }
        }
        //---------------------------------------------------------------------
        public override void Bind(Query q, BindingSource bs)
        {
            if (q == null)
                return;
            CDGrid.ChemDataProvider = q.Pager;
            ControlSwapperEx.AdjustGridColumnsAfterBind(CDGrid);

            // CSBR-132594: load infra display layout xml if saved with wgrid
            // CSBR-132719: don't load xml until cols have been created
            //Coverity Bug Fix CID 13005 
            CBVDataGridView dgv = (CDGrid.SourceWinGrid is CBVDataGridView) ? CDGrid.SourceWinGrid as CBVDataGridView : null;
            if (dgv != null)
            {

                String sInfraXml = dgv.InfraLayoutXml;
                if (!String.IsNullOrEmpty(sInfraXml))
                {
                    Stream xmlStream = CBVUtil.StringToStream(sInfraXml);
                    CDGrid.DisplayLayout.LoadFromXml(xmlStream);
                }
                // CSBR-154097: Get rid of big xml strings after use
                if (dgv.ChildGrids != null)
                {
                    // CSBR-154743: Child's InfraLayoutXml is not written out
                    //  anymore, but zero it out here in case this is an older form.
                    foreach (CBVDataGridView dgvi in dgv.ChildGrids)
                        dgvi.InfraLayoutXml = "";
                }
                dgv.InfraLayoutXml = "";
                CDGrid.SourceWinGrid = null;    // use first time only; previously we did this in AdjustGridColumnsAfterBind
                GC.Collect();
            }

            CDGrid.CheckForChemDraw();
            TabManager.SetupButtonAndRichtextCols(CDGrid);
            if (CDGrid.CardViewMode)
                CDGrid.InitCardView();
        }
        //---------------------------------------------------------------------
        private void LoadWinGridXml(XmlNode wingridNode)
        {
            // read greatis xml with one or more wingrids; convert to infragistics
            foreach (XmlNode formviewNode in wingridNode.ChildNodes)
            {
                if (!formviewNode.Name.Equals("object"))
                    continue;

                FormViewControl formView = new FormViewControl(TabManager.Form);

                // create designer and load; copied from FormViewTab:LoadXml
                String sGreatisXml = formviewNode.OuterXml;

                Designer designerT = new Designer();
                Treasury treasuryT = new Treasury();
                designerT.FormTreasury = treasuryT;
                designerT.FormTreasury.LoadMode = Greatis.FormDesigner.LoadModes.EraseForm;
                designerT.DesignedForm = formView;
                designerT.LayoutXML = sGreatisXml;

                // first control is main grid; others are children
                ChemDataGrid cdg = CDGrid;
                CBVDataGridView cWinGrid = null;
                int i = 0;
                foreach (Control c in formView.Controls)
                {
                    if (!(c is CBVDataGridView)) continue;
                    if (i == 0)
                    {
                        cWinGrid = c as CBVDataGridView;
                        cWinGrid.ChildGrids = new List<CBVDataGridView>();
                        cdg.SourceWinGrid = cWinGrid;
                        ControlFactory.CopyProperties(cWinGrid, cdg);
                        CBVDataGridView.CopyWtoIGridProps(cWinGrid, cdg, i, false, true);
                    }
                    else
                    {
                        cWinGrid.ChildGrids.Add(c as CBVDataGridView);
                    }
                    ++i;
                }
                break;
            }
        }
        //---------------------------------------------------------------------
        private String GetWinGridXml()
        {
            // return xml for the infragistics grid layout
            // serialize via greatis using normal win grid control(s) .. one per band
            String s = "";
            if (CDGrid != null)
            {
                // create a temporary form view with one or more win grid controls
                FormViewControl formView = new FormViewControl(TabManager.Form);
                Control cTempGrid = ControlSwapperEx.MakeSafeChemGrid(CDGrid, true);
                CBVDataGridView cTempDGV = cTempGrid as CBVDataGridView;

                formView.Parent = Form;

                cTempGrid.Visible = true;   // otherwise comes back invisible
                formView.Controls.Add(cTempGrid);

                if (cTempDGV.ChildGrids != null)
                {
                    foreach (CBVDataGridView dgvi in cTempDGV.ChildGrids)
                    {
                        formView.Controls.Add(dgvi);
                    }
                }
                if (formView.Parent == null)
                    Debug.Assert(true, "form has no parent; next call will fail");
                else
                    s = FormViewTab.GreatisToXml(formView, TabManager.Form, false, false);     // false => do not unbind

                // CSBR-154097: Release refs to temp grids
                if (cTempDGV.ChildGrids != null)
                {
                    foreach (CBVDataGridView dgvi in cTempDGV.ChildGrids)
                        formView.Controls.Remove(dgvi);
                }
                formView.Controls.Remove(cTempGrid);
                GC.Collect();
            }
            return s;
        }
        //---------------------------------------------------------------------
        public override XmlElement CreateXmlElement(XmlDocument xdoc)
        {
            XmlElement xmlTab = base.CreateXmlElement(xdoc);
            //Coverity Bug Fix CID 13012 
            string isCardView = "0"; //default value of cardview attribute
            CardStyle cardViewStyle = default(CardStyle);
            ChemControls.ChemDataGrid.CardViewLayoutType cardViewLayout = default(ChemControls.ChemDataGrid.CardViewLayoutType);
            ChemDataGrid chemDataGrid = Control as ChemDataGrid;

            if (chemDataGrid != null)
            {
                isCardView = chemDataGrid.CardViewMode ? "1" : isCardView;
                cardViewStyle = chemDataGrid.CardViewStyle;
                cardViewLayout = chemDataGrid.CardViewLayout;
            }
            //now assign the values to xml attributes
            xmlTab.SetAttribute("cardview", isCardView);
            xmlTab.SetAttribute("cvstyle", cardViewStyle.ToString());
            xmlTab.SetAttribute("cvlayout", cardViewLayout.ToString());

            XmlElement xmlInfra = xdoc.CreateElement("wingrid");
            xmlInfra.InnerXml = GetWinGridXml();
            xmlTab.AppendChild(xmlInfra);
            return xmlTab;
        }
        //---------------------------------------------------------------------
        public override void LoadXmlElement(XmlNode node)
        {
            // load grid view from xml, either old-style (infragistics) or new (win grid)
            base.LoadXmlElement(node);
            bool bIsCardView = CBVUtil.GetIntAttrib(node, "cardview") == 1;
            String sCvstyle = CBVUtil.GetStrAttrib(node, "cvstyle");
            String sCvlayout = CBVUtil.GetStrAttrib(node, "cvlayout");

            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (!childNode.Name.Equals("wingrid"))
                    continue;

                String sLayoutXml = childNode.InnerXml;
                sLayoutXml.Trim();
                if (CDGrid != null && !String.IsNullOrEmpty(sLayoutXml))
                {
                    Debug.Assert(!sLayoutXml.StartsWith("<SOAP-ENV:Envelope")); // infragistics xml

                    LoadWinGridXml(childNode);
                    ChemDataGrid cdg = Control as ChemDataGrid; // why Control? how come not CDGrid?  
                    cdg.CardViewMode = bIsCardView;
                    if (!String.IsNullOrEmpty(sCvstyle))
                        cdg.CardViewStyle = (CardStyle)Enum.Parse(typeof(CardStyle), sCvstyle);
                    if (!String.IsNullOrEmpty(sCvlayout))
                        cdg.CardViewLayout = (ChemDataGrid.CardViewLayoutType)Enum.Parse(typeof(ChemDataGrid.CardViewLayoutType), sCvlayout);
                }
                break;
            }
        }
        //---------------------------------------------------------------------
        #endregion
    }

    /// <summary>
    ///   QUERY VIEW TAB
    /// </summary>
    public class QueryViewTab : FormViewTab
    {
        private bool m_bFirstView;    // true until shown once

        #region Constructors
        public QueryViewTab(TabManager tabMgr)
            : base(tabMgr)
        {
            m_type = TabManager.TabType.ktQuery;
            m_bFirstView = true;
        }
        #endregion

        #region Methods
        public override void Bind(Query q, BindingSource bs)
        {
            // no-op: query form is not bound
        }
        //---------------------------------------------------------------------
        public override void BeginEdit(ToolboxControlEx toolbox, PropertyGrid propertyGrid)
        {
            Form.ShowSearchButtons(false);
            base.BeginEdit(toolbox, propertyGrid);  // CSBR-112908
        }
        //---------------------------------------------------------------------
        public override bool EndEdit(bool bCancel)
        {
            if (bCancel)
                base.EndEdit(bCancel);      // CSBR-112908
            else if (m_formView.EndFormEdit(true))
                Form.Modified = true;
            else
                return false;
            Form.ShowSearchButtons(true);
            RefreshUnitsCombos();
            return true;
        }
        //---------------------------------------------------------------------
        public override void Select()
        {
            Form.ShowSearchButtons(true);
            RefreshSSSCombo();
            RefreshUnitsCombos();
            Form.BeginQueryMode();
        }
        //---------------------------------------------------------------------
        public override void Unselect()
        {
            Form.ShowSearchButtons(false);
            Form.EndQueryMode();

            // set curr rec to whatever is on display in the nav bar. in case user changed
            int recno = 0;
            if (Form.BindingNavigator != null)
                CBVUtil.StrToInt(Form.BindingNavigator.PositionItem.Text);
            if (recno > 0)
                this.TabManager.LastSelectedRow = recno - 1;
        }
        //---------------------------------------------------------------------
        public void ClearOnFirstView()
        {
            if (m_bFirstView || this.TabManager.ClearQueryOnNextOpen)
            {
                ClearQueryForm();
                this.TabManager.ClearQueryOnNextOpen = false;
                m_bFirstView = false;
            }
        }
        //---------------------------------------------------------------------
        public void ClearQueryForm()
        {
            foreach (Control c in m_formView.Controls)
            {
                if (c is ChemDataGrid || c is Button || c is Label || c is CBVFrame /*CSBR-134344*/)
                    continue;
                else if (c is DateTimePicker)
                    (c as DateTimePicker).Checked = false;
                else if (c is MonthCalendar)
                    (c as MonthCalendar).SetSelectionRange(DateTime.Today, DateTime.Today);
                else if (c is CheckBox)
                {
                    CheckBox cbox = c as CheckBox;
                    if (cbox.ThreeState) cbox.CheckState = CheckState.Indeterminate;
                    else cbox.Checked = false;
                    continue;   // do not erase text from checkbox
                }

                if (c is CBVSSSOptionsCombo)
                    (c as CBVSSSOptionsCombo).Reset();

                else if (c is CBVUnitsCombo)
                    (c as CBVUnitsCombo).Reset();
                else
                    c.Text = "";

                if (c is ChemDraw)
                    (c as ChemDraw).Base64 = null;
            }
        }
        //---------------------------------------------------------------------
        public void RefreshFromQuery(Query q)
        {
            // CSBR-135585: reset combo to match query
            foreach (Control c in m_formView.Controls)
            {
                if (c is CBVSSSOptionsCombo)
                {
                    int setting = CBVSSSOptionsCombo.GetComboSetting(q);
                    if (setting != -1)
                        (c as CBVSSSOptionsCombo).Set(setting);
                }
            }
        }
        //---------------------------------------------------------------------
        public void RefreshSSSCombo()
        {
            foreach (Control c in m_formView.Controls)
                if (c is CBVSSSOptionsCombo)
                    (c as CBVSSSOptionsCombo).Reset();
        }
        //---------------------------------------------------------------------
        public void RefreshUnitsCombos()
        {
            foreach (Control c in m_formView.Controls)
                if (c is CBVUnitsCombo)
                    (c as CBVUnitsCombo).Reset();
        }
        //---------------------------------------------------------------------
        private bool IsSubformLabel(Control c, Control.ControlCollection controls)
        {
            if (c is Label)
            {
                int index = controls.IndexOf(c);
                if ((index + 1) < controls.Count && (controls[index + 1] is ChemDataGrid))
                    return true;
            }
            return false;
        }
        //---------------------------------------------------------------------
        public void CreateFromFormView(FormViewControl formViewControl)
        {
            // clone and empty the boxes for query use
            int yOffset = 0;
            foreach (Control c in formViewControl.Controls)
            {
                if (c is ChemDataGrid || IsSubformLabel(c, formViewControl.Controls))
                {
                    // subform is skipped, so controls below here must move up
                    yOffset += c.Height;
                    continue;
                }
                Control cClone = ControlFactory.CloneCtrl(c, false);
                //coverity Bug Fix CID 13099 
                if (cClone != null)
                {
                    cClone.Visible = true;
                    if (!(cClone is Label))
                        cClone.Text = "";
                    if (cClone is ChemDraw)
                    {
                        (cClone as ChemDraw).Base64 = null;
                        (cClone as ChemDraw).ReadOnly = false;  // CSBR-111885
                    }

                    // replace fmla box with standard text box for query input
                    if (cClone is ChemFormulaBox)
                    {
                        Control cTmp = new TextBox();
                        ControlFactory.UseClonedCtrl(cClone, false, cTmp);
                        cClone = cTmp;
                    }
                    if (cClone is TextBox)
                    {
                        (cClone as TextBox).ReadOnly = false;
                        bool bCanCreateQTB = formViewControl.Form.FeatEnabler.CanCreateQueryTextBox();
                        if (bCanCreateQTB)
                        {
                            Control cTmp = new CBVQueryTextBox();
                            ControlFactory.UseClonedCtrl(cClone, false, cTmp);

                            // TO DO: use tag (like "Text.BP") to determine if field is numeric
                            // then assiqn query type accordingly

                            cClone = cTmp;
                        }
                    }
                    if (yOffset > 0)
                        cClone.Top -= yOffset;

                    m_formView.Controls.Add(cClone);
                }
            }

            if (!this.Form.FeatEnabler.CanCreateSubformQueryBoxes())
                return;

            Rectangle rForm = m_formView.GetFormSpace();
            int gap = 10;
            Point controlPosition = new Point(rForm.Left, rForm.Bottom + gap);
            int yPos = controlPosition.Y;
            foreach (Control ctrl in formViewControl.Controls)
            {
                if (ctrl is ChemDataGrid)
                    m_formView.CreateQueryBoxesForSubtable(ctrl as ChemDataGrid, ref yPos);
            }
        }
        //---------------------------------------------------------------------
        #endregion
    }
}
