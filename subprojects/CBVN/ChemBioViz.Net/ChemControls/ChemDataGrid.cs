using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinDataSource;

using SharedLib;

namespace ChemControls
{
    /*
    public interface IChemDataProvider      <== now in SharedLib
    {
        bool IsFinished();                                  // returns true of all the records have been retrieved
        DataSet GetSchema();                                // gets a schema out of the data to come
        void Start();                                       // start the retrieval
        int GetRecordCount();                               // gets the record count for the base table
        bool MoveTo(int index);                             // ensure we have data for given record
        int GetPageRow(int index);                          // return row within current page corresponding to index in full list
   } */

    public class ChemDataGridToolTipItemCreationFilter : IUIElementCreationFilter
    {
        #region Variables
        private IToolTipItem toolTipItem = null;
        private bool m_bEnabled = true;
        #endregion

        #region Properties
        public bool Enabled
        {
            get { return m_bEnabled; }
            set { m_bEnabled = value; }
        }
        //-------------------------------------------------------------------------------------
        private IToolTipItem ToolTipItem
        {
            get
            {
                if (m_bEnabled)
                {
                    if (toolTipItem == null)
                        toolTipItem = new ChemDataGridToolTipItem();
                    return this.toolTipItem;
                }
                return null;
            }
        }
        #endregion

        #region Methods
        private void CheckUIElement(UIElement parent)
        {
            if (parent is ImageUIElement)
            {
                parent.ToolTipItem = this.ToolTipItem;
            }
            else if (parent is Infragistics.Win.UltraWinGrid.CellUIElement && ((Infragistics.Win.UltraWinGrid.CellUIElementBase)(parent)).Column.Key.Equals("STRUCTURE", StringComparison.CurrentCultureIgnoreCase))
            {
                parent.ToolTipItem = this.ToolTipItem;
            }

            foreach (UIElement ui in parent.ChildElements)
                CheckUIElement(ui);
        }
        //-------------------------------------------------------------------------------------
        void IUIElementCreationFilter.AfterCreateChildElements(Infragistics.Win.UIElement parent)
        {
            CheckUIElement(parent);
        }
        //-------------------------------------------------------------------------------------
        bool IUIElementCreationFilter.BeforeCreateChildElements(Infragistics.Win.UIElement parent)
        {
            return false;
        }
        #endregion
    }

    public class ChemDataGridToolTipItem : IToolTipItem
    {
        #region Methods
        ToolTipInfo Infragistics.Win.IToolTipItem.GetToolTipInfo(System.Drawing.Point mousePosition,
            UIElement uiElement, UIElement prevElement, ToolTipInfo tipInfoDefault)
        {
            //if (!ChemBioViz.NET.Properties.Settings.Default.ShowTooltips))
            //    return;

            ToolTipInfo info = new ToolTipInfo(mousePosition);

            object o = (UltraGridCell)uiElement.GetContext(typeof(UltraGridCell));
            if (o == null || !(o is UltraGridCell))
                return info;

            UltraGridCell cell = o as UltraGridCell;

            Image image = null;
            string toolTipText = string.Empty;

            if (cell.Tag != null)
            {
                ChemDrawTag chemDrawTag = cell.Tag as ChemDrawTag;
                if (chemDrawTag != null)
                {
                    image = chemDrawTag.Metafile;
                    toolTipText = chemDrawTag.Caption;
                }
            }

            //// CSBR-156959: To show maximized image when we place the cursor on Image field data in Subform Grid & table tab.
            if (cell.Column.DataType.Name.Equals("Byte[]"))
            {
                if (cell.Value != null)
                {
                    byte[] imagedata = cell.Value as byte[];
                    using (MemoryStream stream = new MemoryStream(imagedata))
                    {
                        image = System.Drawing.Image.FromStream(stream);
                    }
                }
            }
            Bitmap map = null;
            if (image != null)
            {
                map = new Bitmap(image.Width, image.Height);
                Graphics g = Graphics.FromImage(map);
                Rectangle r = new Rectangle(0, 0, image.Width, image.Height);
                g.FillRectangle(new SolidBrush(info.BackColor), r);
                g.DrawImage(image, r);
            }

            info.ToolTipImage = ToolTipImage.Custom;
            info.CustomToolTipImage = map;
            info.ToolTipText = ".";
            //coverity Bug Fix CID 11398 
            if (!string.IsNullOrEmpty(toolTipText))
                info.ToolTipText = toolTipText;

            return info;
        }
        #endregion
    }

    public class ChemDrawTag
    {
        #region Variables
        private Metafile m_metafile;
        private String m_caption;
        #endregion

        #region Properties
        public String Caption
        {
            get { return m_caption; }
            set { m_caption = value; }
        }
        public Metafile Metafile
        {
            get { return m_metafile; }
            set { m_metafile = value; }
        }
        #endregion

        #region Constructor
        public ChemDrawTag()
        {
            m_metafile = null;
        }
        #endregion
    }

    public class CustomizeButton : Control
    {
        #region Variables
        ChemDataGrid grid;
        ContextMenuStrip menu;
        ToolStripMenuItem chooseColumns;
#if ANDRAS
        ToolStripMenuItem normalView;
        ToolStripMenuItem cardView;
        ToolStripMenuItem cellsNormal;
        ToolStripMenuItem cellsForm;
        ToolStripMenuItem labelsInHeader;
        ToolStripMenuItem labelsInCells;
#endif
        bool menuShown;
        #endregion

        #region Properties
        public CustomizeButton()
        {
            grid = null;
            menu = new ContextMenuStrip();
            chooseColumns = new ToolStripMenuItem(Resources.ChooseColumns);
            chooseColumns.Click += new EventHandler(chooseColumns_Click);
            menu.Items.Add(chooseColumns);
#if ANDRAS
            normalView = new ToolStripMenuItem(Resources.NormalView);
            normalView.Click += new EventHandler(normalView_Click);

            cardView = new ToolStripMenuItem(Resources.CardView);
            cardView.Click += new EventHandler(cardView_Click);

            ToolStripMenuItem viewStyle = new ToolStripMenuItem(Resources.ViewStyle);
            viewStyle.DropDownItems.Add(normalView);
            viewStyle.DropDownItems.Add(cardView);

            menu.Items.Add(viewStyle);

            cellsNormal = new ToolStripMenuItem("Horizontal");
            cellsNormal.Click += new EventHandler(cellsNormal_Click);

            cellsForm = new ToolStripMenuItem("Optimized Form");
            cellsForm.Click += new EventHandler(cellsForm_Click);

            ToolStripMenuItem cellArrangementStyle = new ToolStripMenuItem("Cell Arrangements");
            cellArrangementStyle.DropDownItems.Add(cellsNormal);
            cellArrangementStyle.DropDownItems.Add(cellsForm);

            menu.Items.Add(cellArrangementStyle);

            labelsInHeader = new ToolStripMenuItem("in Column headers");
            labelsInHeader.Click += new EventHandler(labelsInHeader_Click);

            labelsInCells = new ToolStripMenuItem("in Cells");
            labelsInCells.Click += new EventHandler(labelsInCells_Click);

            ToolStripMenuItem labelPlacement = new ToolStripMenuItem("Labels placement");
            labelPlacement.DropDownItems.Add(labelsInHeader);
            labelPlacement.DropDownItems.Add(labelsInCells);

            menu.Items.Add(labelPlacement);
#endif
            menuShown = false;
        }
        #endregion

        #region Events
        void chooseColumns_Click(object sender, EventArgs e)
        {
            grid.ShowColumnChooser();
        }

#if ANDRAS
       void labelsInCells_Click(object sender, EventArgs e)
        {
            menuShown = false;
            grid.LabelsInHeader = false;
        }

        void labelsInHeader_Click(object sender, EventArgs e)
        {
            menuShown = false;
            grid.LabelsInHeader = true;
        }

        void cellsForm_Click(object sender, EventArgs e)
        {
            menuShown = false;
            grid.CellArrangementOptimization = true;
        }

        void cellsNormal_Click(object sender, EventArgs e)
        {
            menuShown = false;
            grid.CellArrangementOptimization = false;
        }

        void cardView_Click(object sender, EventArgs e)
        {
            menuShown = false;
            grid.DisplayLayout.Bands[0].CardView = true;
        }

        void normalView_Click(object sender, EventArgs e)
        {
            menuShown = false;
            grid.DisplayLayout.Bands[0].CardView = false;
        }
#endif
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int w = Width;
            int h = Height;

            Rectangle r = new Rectangle(0, 0, w - 1, h - 1);
            e.Graphics.FillRectangle(new SolidBrush(BackColor), r);
            e.Graphics.DrawRectangle(new Pen(Color.Black), r);

            Point[] points = new Point[3];
            points[0].X = 2; points[0].Y = 2;
            points[1].X = 2; points[1].Y = 8;
            points[2].X = 7; points[2].Y = 5;
            e.Graphics.FillPolygon(new SolidBrush(Color.Black), points);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            BackColor = Color.Gray;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            BackColor = Color.White;
        }

        void SetMenu(ChemDataGrid grid)
        {
            this.grid = grid;
#if ANDRAS
            int nBands = grid.DisplayLayout.Bands.Count;
            bool isCardView = nBands > 0 ? grid.DisplayLayout.Bands[0].CardView : false;

            normalView.Enabled = nBands > 0;
            cardView.Enabled = nBands > 0;

            normalView.Checked = !isCardView;
            cardView.Checked = isCardView;
#endif
        }
        #endregion

        #region Methods
        public void Customize(ChemDataGrid grid)
        {
            if (menuShown)
                menuShown = false;
            else
            {
                SetMenu(grid);
                menu.Show(grid, new Point(Width, 0));
                menuShown = true;
            }
        }
        #endregion
    }

    [ToolboxBitmap(typeof(ChemDataGrid))]
    public partial class ChemDataGrid : UltraGrid, IUIElementDrawFilter
    {
        #region Variables
        private ChemDrawForm m_chemDrawForm;
        private UltraGridCell m_cellEdited;
        private bool m_cellArrangementOptimization;
        private bool m_labelsInHeader;
        private UltraGridBand m_preferredChildTable;
        private IChemDataProvider m_chemDataProvider;
        private Control m_sourceWinGrid;        // used for editing and serialization
        private UltraDataSource m_ultraDataSource;
        private TimerCallback m_timerDelegate;
        private System.Threading.Timer m_checkDataProvider;
        private CustomizeButton m_customizeButton;
        private bool m_displayCustomizeButton;  // true to show caret to popup menu
        private bool m_hasChemDrawColumn;
        private UltraGridOverride m_overrideSave;
        private String m_childFormName;
        private CardViewLayoutType m_cardViewLayout;
        private CardStyle m_cardViewStyle;
        public DataSet ds = null;
        #endregion

        #region Properties
        public CardViewLayoutType CardViewLayout
        {
            get { return m_cardViewLayout; }
            set { m_cardViewLayout = value; }
        }
        //-------------------------------------------------------------------------------------          
        public CardStyle CardViewStyle
        {
            get { return m_cardViewStyle; }
            set { m_cardViewStyle = value; }
        }
        //-------------------------------------------------------------------------------------          
        public bool IsSubformGrid
        {
            get
            {
                BindingSource bs = this.DataSource as BindingSource;
                return bs != null && bs.DataMember != null && bs.DataMember.Contains("->"); //  StartsWith("Relation");
            }
        }
        //-------------------------------------------------------------------------------------          
        public String ChildFormName
        {
            get { return m_childFormName; }
            set { m_childFormName = value; }
        }
        //-------------------------------------------------------------------------------------          
        public ChemDrawForm ChemDrawForm
        {
            get { return m_chemDrawForm; }
            set { m_chemDrawForm = value; }
        }
        //-------------------------------------------------------------------------------------          
        public bool DisplayCustomizeButton
        {
            get { return m_displayCustomizeButton; }
            set
            {
                if (value)
                {
                    m_displayCustomizeButton = true;
                    CreateCustomizeButton();
                }
                else
                {
                    m_displayCustomizeButton = false;
                    DestroyCustomizeButton();
                }
            }
        }
        //-------------------------------------------------------------------------------------
        public void InitCardView()
        {
            // if there is a chemdraw column, create a rowlayout to make it larger
            // fails to find cd col until after dataset is retrieved
            CheckForChemDraw();
            if (HasChemDrawColumn)
            {
                OptimizeBandArrangementEx(DisplayLayout.Bands[0]);
            }
            DisplayLayout.Bands[0].CardSettings.ShowCaption = false;
        }
        //-------------------------------------------------------------------------------------
        public bool CardViewMode
        {
            get { return DisplayLayout.Bands[0].CardView; }
            set
            {
                if (value == true)
                    InitCardView();
                else
                    DisplayLayout.Bands[0].UseRowLayout = false;
                DisplayLayout.Bands[0].CardView = value;
            }
        }
        //-------------------------------------------------------------------------------------          
        /// <summary>
        /// override to prevent serializing
        /// 3/03/09: not used: we don't do infragistics serialization any more
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new object DataSource
        {
            get { return base.DataSource; }
            set { base.DataSource = value; }
        }
        //-------------------------------------------------------------------------------------  
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control SourceWinGrid
        {
            get { return m_sourceWinGrid; }
            set { m_sourceWinGrid = value; }
        }
        //-------------------------------------------------------------------------------------  
        public bool HasChemDrawColumn
        {
            get { return m_hasChemDrawColumn; }
            set { m_hasChemDrawColumn = value; }
        }
        //-------------------------------------------------------------------------------------  
        public bool HasMarkedColumn
        {
            get { return DisplayLayout.Bands.Count > 0 && DisplayLayout.Bands[0].Columns.Exists("Marked"); }
        }
        //-------------------------------------------------------------------------------------  
        public UltraGridColumn MarkedColumn
        {
            get { return HasMarkedColumn ? DisplayLayout.Bands[0].Columns["Marked"] : null; }
        }
        //-------------------------------------------------------------------------------------  
        public IChemDataProvider ChemDataProvider
        {
            get { return m_chemDataProvider; }
            set { m_chemDataProvider = value; ChemDataProviderChanged(); }
        }
        //-------------------------------------------------------------------------------------  
        public string PreferredChildTable
        {
            get { return m_preferredChildTable == null ? null : m_preferredChildTable.Header.Caption; }
        }
        //-------------------------------------------------------------------------------------  
        public bool CellArrangementOptimization
        {
            get { return m_cellArrangementOptimization; }
            set
            {
                m_cellArrangementOptimization = value;
                UpdateCellArrangements();
            }
        }
        //-------------------------------------------------------------------------------------  
        public bool LabelsInHeader
        {
            get { return m_labelsInHeader; }
            set
            {
                m_labelsInHeader = value;
                UpdateLabelsLayout();
            }
        }
        //-------------------------------------------------------------------------------------  
        public int RowCount
        {
            // used by GetSelectedRowForChildTable (formviewcontrol.cs) and Measure (printform.cs)
            // NOTE: will probably fail for grandchild table
            get
            {
                BindingSource bs = this.DataSource as BindingSource;
                DataView syncRoot = (bs == null) ? null : bs.SyncRoot as DataView;
                int nRows = (syncRoot == null) ? 0 : syncRoot.Count;
                return nRows;
            }
        }
        //-------------------------------------------------------------------------------------  
        #endregion

        #region Constructors
        public ChemDataGrid()
        {
            m_cardViewLayout = CardViewLayoutType.strTop;
            m_cardViewStyle = CardStyle.MergedLabels;

            this.InitializeChemDataGrid();
            this.BeforeRowExpanded += new CancelableRowEventHandler(ChemDataGrid_BeforeRowExpanded);
            this.InitializeLayout += new InitializeLayoutEventHandler(ChemDataGrid_InitializeLayout);
        }
        //-------------------------------------------------------------------------------------  
        void ChemDataGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // this is where we would set up RTF columns
        }
        //-------------------------------------------------------------------------------------  
        #endregion

        #region Methods
        /// <summary>
        ///  Initializes the ChemDataGrid
        /// </summary>
        private void InitializeChemDataGrid()
        {
            m_chemDrawForm = null;
            DrawFilter = this;
            m_cellEdited = null;
            m_cellArrangementOptimization = false;
            m_labelsInHeader = true;
            m_preferredChildTable = null;

            m_chemDataProvider = null;
            m_ultraDataSource = null;
            m_sourceWinGrid = null;

            m_customizeButton = null;

            System.Globalization.CultureInfo cultureInfo = System.Globalization.CultureInfo.CurrentCulture;
            m_lastRowRetrieved = -1;
        }
        //-------------------------------------------------------------------------------------
        public void PreferChildTable(string childTableName)
        {
            if (childTableName == null)
            {
                RestoreChildTables();
                return;
            }
            int i, n = DisplayLayout.Bands.Count;
            for (i = 1; i < n; i++)
                if (DisplayLayout.Bands[i].Header.Caption == childTableName)
                    break;

            if (i == n)
                return;

            UltraGridBand preferredBand = DisplayLayout.Bands[i];

            for (i = 1; i < n; i++)
                DisplayLayout.Bands[i].Hidden = DisplayLayout.Bands[i] != preferredBand;

            foreach (UltraGridRow row in Rows)
            {
                if (row.Band != DisplayLayout.Bands[0])
                    continue;
                foreach (UltraGridChildBand childband in row.ChildBands)
                {
                    if (childband.Band != preferredBand)
                        continue;
                    row.Hidden = childband.Rows.Count == 0;
                    row.Expanded = childband.Rows.Count != 0;
                }
            }
        }
        //-------------------------------------------------------------------------------------  
        public List<string> GetAllBandKeys(BandsCollection bands)
        {
            List<string> names = null;
            if (ds != null)
            {
                names = new List<string>();
                names.Add(bands[0].Key); // adding base table key
                foreach (DataRelation rel in this.ds.Relations)       // adding child tables keys
                {
                    names.Add(rel.ChildTable.TableName);
                }
            }
            return names;
        }
        //-------------------------------------------------------------------------------------  
        public List<string> GetChildTableNames()
        {
            List<string> childBandNames = new List<string>();
            int i, n = DisplayLayout.Bands.Count;
            for (i = 1; i < n; i++)
                childBandNames.Add(DisplayLayout.Bands[i].Header.Caption);
            return childBandNames;
        }
        //-------------------------------------------------------------------------------------  
        private bool HasRelWithParent(DataTable table, DataSet schema)
        {
            // return first relation having given table as parent; null if none
            foreach (DataRelation r in schema.Relations)
                if (r.ParentTable == table) return true;
            return false;
        }
        //-------------------------------------------------------------------------------------
        private void AddChildBands(UltraDataBand parentBand, DataSet schema, DataTable table)
        {
            // recursive: add any bands immediately below given parent, then do children if any
            foreach (DataRelation r in schema.Relations)
            {
                bool bIsChildOfBand = (r.ParentTable == table);
                if (bIsChildOfBand)
                {
                    UltraDataBand band = parentBand.ChildBands.Add(r.ChildTable.TableName);
                    //band.Tag = r;   // tag band with relation to parent                   

                    foreach (DataColumn c in r.ChildTable.Columns)
                        band.Columns.Add(c.ColumnName, c.DataType);

                    bool bHasChildren = HasRelWithParent(r.ChildTable, schema);
                    if (bHasChildren)
                        AddChildBands(band, schema, r.ChildTable);
                }
            }
        }
        //-------------------------------------------------------------------------------------
        private void ChemDataProviderChanged()
        {
            if (m_chemDataProvider == null)
                return;

            DataSet schema = m_chemDataProvider.GetSchema();
            if (schema == null)
                return;
            this.ds = schema;
            m_ultraDataSource = new UltraDataSource();
            m_ultraDataSource.CellDataRequested += new CellDataRequestedEventHandler(ultraDataSource_CellDataRequested);

            m_ultraDataSource.Band.Key = schema.Tables[0].TableName;

            foreach (DataColumn c in schema.Tables[0].Columns)                  // base table
                m_ultraDataSource.Band.Columns.Add(c.ColumnName, c.DataType);

            AddChildBands(m_ultraDataSource.Band, schema, schema.Tables[0]);    // child tables

            DisplayLayout.LoadStyle = LoadStyle.LoadOnDemand;
            m_chemDataProvider.Start();
            DataSource = m_ultraDataSource;
            DisplayLayout.Bands[0].Override.ExpansionIndicator = schema.Tables.Count > 1 ? ShowExpansionIndicator.Always : ShowExpansionIndicator.Default;

            m_timerDelegate = new TimerCallback(CheckDataProvider);
            m_checkDataProvider = new System.Threading.Timer(m_timerDelegate, null, 1000, 1000);
            m_lastRowRetrieved = -1;
        }
        //-------------------------------------------------------------------------------------
        /// <summary>
        ///  CheckDataProviderMethod can not be called directly because CheckDataProvider is called by the timer
        ///  it can be null while disposing and runs on a different thread
        /// </summary>
        /// <param name="stateInfo"></param>
        private void CheckDataProvider(Object stateInfo)
        {
            if (this.TopLevelControl == null)   // a way to see if this control has been created yet (not a very good way)
                return;                         // if not, the call below will crash

            if (m_chemDataProvider != null)
                BeginInvoke(new CheckDataProviderInvokeDelegate(CheckDataProviderMethod));
        }
        //-------------------------------------------------------------------------------------
        private void CheckDataProviderMethod()
        {
            // periodic check
            int n = m_chemDataProvider.GetRecordCount();
            m_ultraDataSource.Rows.SetCount(m_chemDataProvider.GetRecordCount());
            // forget timer if finished
            if (m_chemDataProvider.IsFinished())
            {
                m_checkDataProvider.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }
        //-------------------------------------------------------------------------------------
        internal static void SetOverride(UltraGridOverride d, UltraGridOverride s)
        {
            d.BorderStyleRow = s.BorderStyleRow;
            d.BorderStyleHeader = s.BorderStyleHeader;
            d.BorderStyleCell = s.BorderStyleHeader;

            d.RowSpacingBefore = s.RowSpacingBefore;
            d.RowSpacingAfter = s.RowSpacingAfter;

            d.HeaderAppearance.BorderColor = s.HeaderAppearance.BorderColor;
            d.HeaderAppearance.BorderColor2 = s.HeaderAppearance.BorderColor2;
            d.HeaderAppearance.BorderColor3DBase = s.HeaderAppearance.BorderColor3DBase;

            d.HeaderAppearance.BackColor = s.HeaderAppearance.BackColor;
            d.HeaderAppearance.BackColor2 = s.HeaderAppearance.BackColor2;

            d.HeaderStyle = s.HeaderStyle;
        }
        //-------------------------------------------------------------------------------------
        private void UpdateLabelsLayout()
        {
            UltraGridBand band = DisplayLayout.Bands[0];

            if (m_labelsInHeader)
            {
                if (band.RowLayoutLabelStyle == RowLayoutLabelStyle.Separate)
                    return;
                SetOverride(band.Override, m_overrideSave);
                band.RowLayoutLabelStyle = RowLayoutLabelStyle.Separate;
            }
            else
            {
                if (band.RowLayoutLabelStyle == RowLayoutLabelStyle.WithCellData)
                    return;
                m_overrideSave = new UltraGridOverride();
                SetOverride(m_overrideSave, DisplayLayout.Bands[0].Override);

                band.RowLayoutLabelStyle = RowLayoutLabelStyle.WithCellData;

                band.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
                band.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.Solid;
                band.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
                band.Override.HeaderStyle = Infragistics.Win.HeaderStyle.XPThemed;  // to have a flat appearance for the labels
            }
        }
        //-------------------------------------------------------------------------------------
        private void GetUIElements(UIElement parent)
        {
            foreach (UIElement ui in parent.ChildElements)
                GetUIElements(ui);
        }
        //-------------------------------------------------------------------------------------
        private static int CmpIgridCols(UltraGridColumn x, UltraGridColumn y)
        {
            // for sorting in column order
            return (x.Header.VisiblePosition < y.Header.VisiblePosition) ? -1 :
                    (x.Header.VisiblePosition > y.Header.VisiblePosition) ? 1 : 0;
        }
        //---------------------------------------------------------------------
        public static UltraGridColumn[] SortInVisibleOrder(ColumnsCollection cdgCols)
        {
            // make an array of the igrid cols and sort by display index
            // due to CSBR-133393: sort cols in preparing cardview
            UltraGridColumn[] colOrder = new UltraGridColumn[cdgCols.Count];
            int i = 0;
            foreach (UltraGridColumn col in cdgCols)
                colOrder[i++] = col;
            Array.Sort(colOrder, CmpIgridCols);
            return colOrder;
        }
        //-------------------------------------------------------------------------------------          
        public enum CardViewLayoutType
        {
            strTop, strBottom, strLeft, strRight, strNone
        }
        //---------------------------------------------------------------------
        private bool SetColumnInfo(CardViewLayoutType type, UltraGridColumn[] cols)
        {
            // set up card view layout to have structure at top/bottom/left/right

            // find the struct col and count others
            int nTextCols = 0;
            UltraGridColumn chemDrawCol = null;
            foreach (UltraGridColumn col in cols)
            {
                if (IsColumnChemDraw(col))
                    chemDrawCol = col;
                else
                    ++nTextCols;
            }
            if (chemDrawCol == null)
                return false;

            // set up cell sizes and positions
            int xStruct = 0, yStruct = 0, xRow = 0, yRow = 0, xSpan = 2, ySpan = 1, cdHgt = 4;
            switch (type)
            {
                case CardViewLayoutType.strTop:
                    yRow = cdHgt;
                    break;
                case CardViewLayoutType.strBottom:
                    yStruct = nTextCols;
                    break;
                case CardViewLayoutType.strLeft:
                    xRow = xSpan;
                    break;
                case CardViewLayoutType.strRight:
                    xStruct = xSpan;
                    break;
            }
            // apply to row layout
            foreach (UltraGridColumn col in cols)
            {
                col.RowLayoutColumnInfo.SpanX = xSpan;
                col.RowLayoutColumnInfo.SpanY = ySpan;

                if (col == chemDrawCol)
                {
                    col.RowLayoutColumnInfo.OriginX = xStruct;
                    col.RowLayoutColumnInfo.OriginY = yStruct;
                    col.RowLayoutColumnInfo.SpanY = cdHgt;
                    if (type == CardViewLayoutType.strLeft || type == CardViewLayoutType.strRight)
                        col.RowLayoutColumnInfo.SpanY = RowLayoutColumnInfo.Remainder;

                    // CSBR-134057: do this only when creating the first time, not after settings have been saved
                    if (col.RowLayoutColumnInfo.ActualCellSize == new Size(0, 0))
                        col.RowLayoutColumnInfo.ActualCellSize = new Size(0, 80);

                    col.RowLayoutColumnInfo.LabelPosition = LabelPosition.None;
                }
                else
                {
                    col.RowLayoutColumnInfo.OriginX = xRow;
                    col.RowLayoutColumnInfo.OriginY = yRow++;
                }
            }
            return true;
        }
        //---------------------------------------------------------------------
        private void OptimizeBandArrangementEx(UltraGridBand band)
        {
            UltraGridColumn[] cols = SortInVisibleOrder(band.Columns);
            bool bHasStructCol = SetColumnInfo(m_cardViewLayout, cols);
            band.UseRowLayout = bHasStructCol;
        }
        //---------------------------------------------------------------------
        private void OptimizeBandArrangement_ANDRAS(UltraGridBand band)
        {
            UltraGridColumn chemDrawCol = null;
            foreach (UltraGridColumn col in band.Columns)
            {
                if (IsColumnChemDraw(col))
                    chemDrawCol = col;
            }
            if (chemDrawCol == null)
                return;

            int originX = 1;
            int originY = 0;
            foreach (UltraGridColumn col in band.Columns)
            {
                if (col == chemDrawCol)
                    continue;

                col.RowLayoutColumnInfo.OriginX = originX;
                col.RowLayoutColumnInfo.OriginY = originY;
                originY += 2;
                if (originY > 8)
                {
                    originX += 2;
                    originY = 0;
                }
                col.RowLayoutColumnInfo.SpanX = 2;
                col.RowLayoutColumnInfo.SpanY = 2;
                col.RowLayoutColumnInfo.LabelPosition = LabelPosition.Top;
                col.RowLayoutColumnInfo.LabelSpan = 1;
            }

            chemDrawCol.RowLayoutColumnInfo.OriginX = 0;
            chemDrawCol.RowLayoutColumnInfo.OriginY = 0;
            chemDrawCol.RowLayoutColumnInfo.SpanX = 1;
            chemDrawCol.RowLayoutColumnInfo.SpanY = RowLayoutColumnInfo.Remainder;
            chemDrawCol.RowLayoutColumnInfo.SpanY = 20;
            chemDrawCol.RowLayoutColumnInfo.PreferredCellSize = new Size(100, 100);

            band.UseRowLayout = true;
        }
        //-------------------------------------------------------------------------------------
        private void DefaultBandArrangement(UltraGridBand band)
        {
            band.UseRowLayout = false;
        }
        //-------------------------------------------------------------------------------------
        private void UpdateBandArrangements(UltraGridBand band)
        {
            if (m_cellArrangementOptimization)
                Debug.Assert(false); // OptimizeBandArrangement_ANDRAS(band);
            else
                DefaultBandArrangement(band);
        }
        //-------------------------------------------------------------------------------------
        private void UpdateCellArrangements()
        {
            foreach (UltraGridBand band in DisplayLayout.Bands)
                UpdateBandArrangements(band);
        }
        //-------------------------------------------------------------------------------------
        private void RestoreChildTables()
        {
            foreach (UltraGridBand band in DisplayLayout.Bands)
                band.Hidden = false;
            foreach (UltraGridRow row in Rows)
            {
                row.Hidden = false;
                row.Expanded = false;
            }
        }
        //-------------------------------------------------------------------------------------
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            try
            {
                m_chemDrawForm = new ChemDrawForm();
                m_chemDrawForm.Visible = false;
                m_chemDrawForm.BackColor = Color.White; // an attempt; doesn't work
                Controls.Add(m_chemDrawForm);
            }
            catch (Exception ex)
            {
                // CSBR-132967: if can't create cdax, do without it
                Debug.WriteLine(ex.Message);
                m_chemDrawForm = null;
            }

            this.CreationFilter = new ChemDataGridToolTipItemCreationFilter();
            this.PropertyChanged += new Infragistics.Win.PropertyChangedEventHandler(ChemDataGrid_PropertyChanged);

            DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            DisplayLayout.Override.AllowColSizing = AllowColSizing.Free;
            DisplayLayout.Override.CellAppearance.BackColor = Color.White;  // this doesn't work either

            // CSBR-110810: make it read-only
            DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
            DisplayLayout.Override.AllowAddNew = AllowAddNew.No;
            DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;

            // except for Marked col if any
            if (this.HasMarkedColumn)
                this.MarkedColumn.Layout.Override.AllowUpdate = DefaultableBoolean.True;

            DisplayLayout.GroupByBox.Hidden = true;
            DisplayLayout.Override.AllowMultiCellOperations = AllowMultiCellOperation.All;
            DisplayLayout.Override.SelectTypeCell = SelectType.Extended;

            DisplayLayout.Override.TipStyleScroll = TipStyle.Hide;  // CSBR-139347

            // prevent sort by click
            // CSBR-151667: Make this so in all cases, not just grid view
            DisplayLayout.Override.HeaderClickAction = HeaderClickAction.Select;
            GetUIElements(this.ControlUIElement);

            if (m_displayCustomizeButton)
                CreateCustomizeButton();
        }
        //-------------------------------------------------------------------------------------
        private void CreateCustomizeButton()
        {
            m_customizeButton = new CustomizeButton();

            m_customizeButton.Left = Left + 3;
            m_customizeButton.Top = Top + 3;
            m_customizeButton.Width = 10;
            m_customizeButton.Height = 11;

            Parent.Controls.Add(m_customizeButton);
            Parent.Controls.SetChildIndex(m_customizeButton, 0);

            m_customizeButton.Click += new EventHandler(customizeButton_Click);
        }
        //-------------------------------------------------------------------------------------
        private void DestroyCustomizeButton()
        {
            if (Parent != null && Parent.Controls.Contains(m_customizeButton))
            {
                Parent.Controls.Remove(m_customizeButton);
                m_customizeButton = null;
            }
        }
        //-------------------------------------------------------------------------------------
        public UltraGridColumn FindUltraGridColumn(Control c)
        {
            // search for column with match on control data bindings (better than using c.Tag)
            String sBindingMem = (c.DataBindings == null || c.DataBindings.Count < 1) ? String.Empty :
                                c.DataBindings[0].BindingMemberInfo.BindingField;
            return String.IsNullOrEmpty(sBindingMem) ? null : FindUltraGridColumn(sBindingMem);
        }
        //-------------------------------------------------------------------------------------
        public UltraGridColumn FindUltraGridColumn(string columnname)
        {
            // search for column with given name or caption
            foreach (UltraGridBand band in DisplayLayout.Bands)
                foreach (UltraGridColumn c in band.Columns)
                {
                    if (c.Key.Equals(columnname))
                        return c;
                    if (c.Header.Caption != columnname)
                        continue;
                    return c;
                }
            return null;
        }
        //-------------------------------------------------------------------------------------
        public static DataSet DataSourceToDataSet(object dataSource)
        {
            // given datasource at top/child/grandchild level, return top-level dataset if any
            DataSet dataset = null;
            if (dataSource == null)
                return dataset;

            else if (dataSource is DataSet)
                dataset = dataSource as DataSet;

            else if (dataSource is BindingSource)
            {

                BindingSource b = dataSource as BindingSource;
                if (b.DataSource is DataSet)
                    dataset = b.DataSource as DataSet;
                else if (b.DataSource != null && (b.DataSource as BindingSource).DataSource is DataSet)
                    dataset = (b.DataSource as BindingSource).DataSource as DataSet;    // for subforms

                // handle grandchildren too
                else if (b.DataSource != null && (b.DataSource as BindingSource).DataSource != null)
                {
                    BindingSource bs1 = b.DataSource as BindingSource;
                    BindingSource bs2 = bs1.DataSource as BindingSource;
                    if (bs2 != null && bs2.DataSource is DataSet)
                        dataset = bs2.DataSource as DataSet;
                }
            }
            return dataset;
        }
        //-------------------------------------------------------------------------------------
        public void CheckForChemDraw()
        {
            m_hasChemDrawColumn = false;

            DataSet dataset = null;
            if (m_chemDataProvider != null)
                dataset = m_chemDataProvider.GetSchema();
            else
                dataset = DataSourceToDataSet(this.DataSource);
            if (dataset == null)
                return;

            foreach (DataTable t in dataset.Tables)
            {
                foreach (DataColumn c in t.Columns)
                {
                    // assume byte arrays are picture fields
                    if (c.DataType.Name.Equals("Byte[]"))
                    {
                        UltraGridColumn ucp = FindUltraGridColumn(c.ColumnName);
                        if (ucp != null)
                            ucp.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                        continue;
                    }
                    // structure col is any having nonblank extended property "mimetype"
                    object o = c.ExtendedProperties["mimetype"];
                    if (o == null)
                        continue;
                    if (System.Convert.ToString(o) == "")
                        continue;
                    UltraGridColumn uc = FindUltraGridColumn(c.ColumnName);
                    if (uc == null)
                        continue;
                    SetColumnToChemDraw(uc);

                    if (!uc.Hidden)
                        m_hasChemDrawColumn = true;
                }
            }
        }
        //-------------------------------------------------------------------------------------
        private void HandleChemDrawEditorLostFocus()
        {
#if ANDRAS
            if (m_cellEdited != null)
            {
                m_cellEdited.Value = m_chemDrawForm.Value;
                m_cellEdited.Tag = new ChemDrawTag();
                m_cellEdited = null;
            }
#endif
            m_chemDrawForm.Hide();
        }
        //-------------------------------------------------------------------------------------
        internal static UltraGridColumn FindColumn(ColumnsCollection columns, string caption)
        {
            foreach (UltraGridColumn c in columns)
                if (c.Header.Caption == caption)
                    return c;
            return null;
        }
        //-------------------------------------------------------------------------------------
        private UltraGridColumn ChildColumnToSort(MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            UIElement ui = this.DisplayLayout.UIElement.ElementFromPoint(p);

            if (ui == null)
                return null;

            if (!(ui is TextUIElement))
                return null;

            if (ui.SelectableItem == null)
                return null;

            if (!(ui.SelectableItem is Infragistics.Win.UltraWinGrid.ColumnHeader))
                return null;

            Infragistics.Win.UltraWinGrid.ColumnHeader h = ui.SelectableItem as Infragistics.Win.UltraWinGrid.ColumnHeader;

            if (h.Column.Band == DisplayLayout.Bands[0]) // not child column
                return null;

            return h.Column;    // that is what we were looking for
        }
        //-------------------------------------------------------------------------------------
#if ANDRAS
        protected override void OnMouseClick(MouseEventArgs e)
        {
            UltraGridColumn column = ChildColumnToSort(e);
            bool ctrlDown = (Control.ModifierKeys & Keys.Control) == Keys.Control;

            if (column == null || !ctrlDown)
            {
                base.OnMouseClick(e);
                return;
            }

            UltraGridBand band = column.Band;

            if (column.SortIndicator == SortIndicator.None) // if it is sorted and clicked, then it will come resorted the opposite
            {
                band.SortedColumns.Clear();
                band.SortedColumns.Add(column, true);
            }

            UltraGridBand band0 = DisplayLayout.Bands[0];   // get the base table, this needs to be sorted as well.

            UltraGridColumn sortCol = FindColumn(band0.Columns, "Hidden Sort");
            if (sortCol == null)
            {
                sortCol = band0.Columns.Add("Hidden Sort");
                //                sortCol.Hidden = true;
            }

            foreach (UltraGridRow row in Rows)
            {
                if (row.Band != band0)                              // process only the root band
                    continue;

                UltraGridChildBand cband = null;                    // find the selected child band
                foreach (UltraGridChildBand cb in row.ChildBands)
                {
                    if (cb.Band == band)
                        cband = cb;
                }

                if (cband == null)                                  // this is just extra caution. It should not happen
                    continue;

                if (cband.Rows.Count == 0)
                    row.Cells["Hidden Sort"].Value = null;
                else
                {
                    int i = column.SortIndicator == SortIndicator.Ascending ? cband.Rows.Count - 1 : cband.Rows.Count - 1;
                    object value = cband.Rows[i].Cells[column.Header.Caption].Value; // largest or smallest value
                    row.Cells["Hidden Sort"].Value = value;
                }
            }

            band0.SortedColumns.Clear();
            band0.SortedColumns.Add("Hidden Sort", column.SortIndicator == SortIndicator.Ascending);

            UpdateData();

            /*
                        if (ui == null)
                        {
                            CustomizeGridDialog d = new CustomizeGridDialog();
                            d.ChemDataGrid = this;
                            d.ShowDialog();
                        }
                        else if (ui is RowColRegionIntersectionUIElement || ui is CardAreaScrollRegionUIElement)
                        {
                            CustomizeGridDialog d = new CustomizeGridDialog();
                            d.ChemDataGrid = this;
                            d.ShowDialog();
                        }
            */
        }
#endif
        //-------------------------------------------------------------------------------------
        public static bool bgAllowGridTooltips = true;  // CSBR-132730 -- View Tooltips command affects grids

        protected override void OnMouseHover(EventArgs e)
        {
            bool bUsingTooltips = this.DisplayLayout.Override.TipStyleCell == TipStyle.Default;
            if (bUsingTooltips != bgAllowGridTooltips)
            {
                this.DisplayLayout.Override.TipStyleCell = bgAllowGridTooltips ? TipStyle.Default : TipStyle.Hide;
                ChemDataGridToolTipItemCreationFilter filter = this.CreationFilter as ChemDataGridToolTipItemCreationFilter;
                //Coverity Bug Fix CID : 11400 
                if (filter != null)
                    filter.Enabled = bgAllowGridTooltips;
            }
            base.OnMouseHover(e);
        }
        //-------------------------------------------------------------------------------------
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (m_cellEdited != null)
                HandleChemDrawEditorLostFocus();
            else
                base.OnMouseDown(e);
        }
        //-------------------------------------------------------------------------------------
        public bool IsColumnChemDraw(UltraGridColumn col)
        {
            if (col == null)
                return false;
            if (col.Tag == null)
                return col.Key.Equals("STRUCTURE", StringComparison.CurrentCultureIgnoreCase);
            return (col.Tag is ChemDrawTag);
        }
        //-------------------------------------------------------------------------------------
        public bool IsCellChemDraw(UltraGridCell cell)
        {
            if (cell == null)
                return false;
            if (cell.Tag == null)
                return IsColumnChemDraw(cell.Column);
            if (!(cell.Tag is ChemDrawTag))
                return false;
            return true;
        }
        //-------------------------------------------------------------------------------------
        public bool IsChemDraw(object o)
        {
            if (o == null)
                return false;
            if (o.GetType() == typeof(UltraGridCell))
                return IsCellChemDraw(o as UltraGridCell);
            if (o.GetType() == typeof(UltraGridColumn))
                return IsColumnChemDraw(o as UltraGridColumn);
            return false;
        }
        //-------------------------------------------------------------------------------------
        public void SetColumnToChemDraw(UltraGridColumn structureCol)
        {
            structureCol.Tag = new ChemDrawTag();
            structureCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
            structureCol.CellAppearance.BackColor = Color.White;
            structureCol.CellAppearance.BackColor2 = Color.DarkBlue;
        }
        //-------------------------------------------------------------------------------------
        public void SetCellToChemDraw(UltraGridCell structureCell)
        {
            structureCell.Tag = new ChemDrawTag();
            structureCell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
            structureCell.Appearance.BackColor = Color.White;
            structureCell.Appearance.BackColor2 = Color.DarkBlue;
        }
        //-------------------------------------------------------------------------------------
        public static Rectangle GetDisplayRectangle(Rectangle boundsRect, Rectangle imageRect)
        {
            //make sure that display is proportional in the target bounds
            Rectangle r = new Rectangle();

            if (imageRect.Width == 0 || imageRect.Height == 0)
            {
                r = boundsRect;
                return r;
            }

            int xc = boundsRect.X + boundsRect.Width / 2;
            int yc = boundsRect.Y + boundsRect.Height / 2;
            double fx = 1.0 * boundsRect.Width / imageRect.Width;
            double fy = 1.0 * boundsRect.Height / imageRect.Height;
            double f = System.Math.Min(fx, fy);
            f = System.Math.Min(f, 1.0f);   // CSBR-132740: don't allow expanding image

            r.Width = (int)(imageRect.Width * f);
            r.Height = (int)(imageRect.Height * f);
            r.X = xc - r.Width / 2;
            r.Y = yc - r.Height / 2;

            return r;
        }
        //-------------------------------------------------------------------------------------
        bool IUIElementDrawFilter.DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
        {
            CellUIElement e = drawParams.Element as CellUIElement;
            UltraGridCell c = e.Cell;

            if (!IsCellChemDraw(c))
                return false;

            if (c.Value == null)
                return true;


            if (c.Tag != null)
                if (!(c.Tag is ChemDrawTag))
                    c.Tag = null;

            if (c.Tag == null)
                c.Tag = new ChemDrawTag();

            ChemDrawTag chemDrawTag = c.Tag as ChemDrawTag;

            if (chemDrawTag.Metafile == null && m_chemDrawForm != null)
            {
                chemDrawTag.Metafile = m_chemDrawForm.GetMetafile(c.Value); // cache the metafile
                chemDrawTag.Caption = m_chemDrawForm.GetCaption(c.Value);
            }

            Metafile m = chemDrawTag.Metafile;

            if (m == null)
                return true;

            Rectangle bounds = m_chemDrawForm.GetBounds(c.Value);
            Rectangle insideBorders = drawParams.Element.RectInsideBorders;
            Rectangle imageRect = GetDisplayRectangle(insideBorders, bounds);

            drawParams.AppearanceData.BackColor = Color.White;
            drawParams.DrawBackColor(insideBorders);

            drawParams.Graphics.DrawImage(m, imageRect);

            if (c.Band.RowLayoutLabelStyle == RowLayoutLabelStyle.WithCellData)
            {
                Infragistics.Win.UIElementBorderStyle borderStyle = c.Band.Override.BorderStyleCell;
                Rectangle border = drawParams.Element.Rect;
                drawParams.Graphics.DrawRectangle(new Pen(Color.LightGray), border);
            }
            return true;
        }
        //-------------------------------------------------------------------------------------
        DrawPhase IUIElementDrawFilter.GetPhasesToFilter(ref UIElementDrawParams drawParams)
        {
            if (drawParams.Element.GetType() != typeof(CellUIElement))
                return DrawPhase.None;
            else
                return DrawPhase.BeforeDrawElement;
        }
        #endregion

        #region Events
        private delegate void CheckDataProviderInvokeDelegate();
        //-------------------------------------------------------------------------------------
        public void ChemDataGrid_BeforeRowExpanded(object sender, CancelableRowEventArgs e)
        {
            UltraDataRow row = e.Row.ListObject as UltraDataRow;    // when connected to ChemDataProvider
            if (row == null)
                return;

            foreach (UltraDataBand childBand in row.Band.ChildBands)
            {
                int nChildRows = GetChildRowCountForBand(childBand, row);
                row.GetChildRows(childBand).SetCount(nChildRows);
            }
        }
        //-------------------------------------------------------------------------------------
        public void customizeButton_Click(object sender, EventArgs e)
        {
            m_customizeButton.Customize(this);
        }
        //-------------------------------------------------------------------------------------
        public void ChemDataGrid_PropertyChanged(object sender, Infragistics.Win.PropertyChangedEventArgs e)
        {
            // check for change of datasource property
            if (e.ChangeInfo.PropId.CompareTo(Infragistics.Win.UltraWinGrid.PropertyIds.DataSource) != 0)
                return;
            CheckForChemDraw();
        }
        //-------------------------------------------------------------------------------------
        public void ultraDataSource_CellDataRequested(object sender, CellDataRequestedEventArgs e)
        {
            DataRow dataRow = URowToDRow(e.Row);
            Debug.Assert(dataRow != null);
            if (e.Column.Index < dataRow.ItemArray.Length) //CBOE-779
                e.Data = dataRow[e.Column.Index];
        }
        //-------------------------------------------------------------------------------------
        public void ChemDrawEditor_LostFocus(object sender, EventArgs e)
        {
            HandleChemDrawEditorLostFocus();
        }
        //-------------------------------------------------------------------------------------
        private DataRelation FindRelByTableNames(String sParent, String sChild, DataSet dataSet)
        {
            // look through relations of dataset for one having given parent and child
            // duplicates method in cbvutilities
            foreach (DataRelation rel in dataSet.Relations)
                if (rel.ParentTable.TableName.Equals(sParent) && rel.ChildTable.TableName.Equals(sChild))
                    return rel;
            return null;
        }
        //---------------------------------------------------------------------
        public DataRelation GetRelationForBand(UltraDataBand band)
        {
            DataRelation rel = (band.ParentBand == null) ? null :
                            FindRelByTableNames(band.ParentBand.Key, band.Key, m_chemDataProvider.GetSchema());
            return rel;
        }
        //-------------------------------------------------------------------------------------
        public int GetChildRowCountForBand(UltraDataBand band, UltraDataRow mainRow)
        {
            // count child rows for given band under given row
            DataRelation rel = GetRelationForBand(band);
            DataRow dataRow = URowToDRow(mainRow);
            if (rel != null && dataRow != null)
            {
                try
                {
                    // sometimes gets System.ArgumentException: The row doesn't belong to the same DataSet as this relation.
                    // it may be that child rows are continued on another page

                    // see http://thedatafarm.com/blog/dotnet/error-the-row-doesn-t-belong-to-the-same-dataset-as-this-relation/
                    // says we need ds.Relations.Add(rel)
                    DataRow[] childRows = dataRow.GetChildRows(rel);
                    return childRows.Length;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            return 0;
        }
        //-------------------------------------------------------------------------------------
        private Stack<UltraDataRow> MakeURowStack(UltraDataRow uRow)
        {
            // march up to topmost parent row, making stack
            Stack<UltraDataRow> stack = new Stack<UltraDataRow>();
            UltraDataRow row = uRow;
            while (row != null)
            {
                stack.Push(row);
                row = row.ParentRow;
            }
            return stack;
        }
        //-------------------------------------------------------------------------------------
        private int m_lastRowRetrieved = -1;    // to prevent re-retrieving same row

        private void EnsureRowAvailable(int rowIndex)
        {
            // retrieve new page of data if necessary to get specified row
            DataSet dataSet = m_chemDataProvider.GetSchema();
            if (dataSet == null || dataSet.Tables.Count < 1)
                return;
            if (rowIndex < 0 || rowIndex > m_chemDataProvider.GetRecordCount() - 1)
                return;

            if (rowIndex != m_lastRowRetrieved)
            {
                if (m_chemDataProvider.MoveTo(rowIndex))
                    m_lastRowRetrieved = rowIndex;
                else
                {
                    // CSBR-153391: Don't just stay stuck here
                    //TODO: Get back to the actual record the user was on
                    ActiveRow = Rows[m_lastRowRetrieved];
                }
            }
        }
        //-------------------------------------------------------------------------------------
        public DataRow URowToDRow(UltraDataRow uRowTarget)
        {
            // given row in grid, find row in dataset
            // do this by starting at topmost row, using GetChildRows for each level below
            Stack<UltraDataRow> stack = MakeURowStack(uRowTarget);
            UltraDataRow uMainRow = stack.Pop();

            EnsureRowAvailable(uMainRow.Index);
            int pageIndex = m_chemDataProvider.GetPageRow(uMainRow.Index);
            DataSet dataSet = m_chemDataProvider.GetSchema();
            if (dataSet == null)
                throw new Exception("No dataset available for grid");
            DataRow dMainRow = dataSet.Tables[0].Rows[pageIndex];

            DataRow dRow = dMainRow;
            while (stack.Count > 0)
            {
                UltraDataRow uSubRow = stack.Pop();
                DataRelation rel = GetRelationForBand(uSubRow.Band);
                Debug.Assert(rel != null);

                DataRow[] childRows = dRow.GetChildRows(rel);
                Debug.Assert(uSubRow.Index >= 0 && uSubRow.Index < childRows.Length);
                dRow = childRows[uSubRow.Index];
            }
            return dRow;
        }
        //-------------------------------------------------------------------------------------
        #endregion
    }
}

