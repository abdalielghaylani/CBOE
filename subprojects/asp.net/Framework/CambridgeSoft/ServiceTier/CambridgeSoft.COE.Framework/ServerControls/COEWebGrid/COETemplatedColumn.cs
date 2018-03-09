using System;
using System.Data;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.ServerControls.FormGenerator;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.Properties;
using System.Globalization;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Reflection;

namespace CambridgeSoft.COE.Framework.Controls.COEGrid
{
    #region Templated Column Properties

    public class COETemplatedColumnProperties
    {
        #region variables

        private DataSet _datasource = new DataSet();
        string _strFilterExp = string.Empty;
        bool _ChildExpanded;
        string _childTableName = string.Empty;
        bool _isFirstRow = true;
        string _imageFolder;
        string _expandImage;
        string _collapseImage;
        string _fontSizeInPoint;
        ScriptManager _sm;
        int _intChildSortColumn;
        string _strValue;
        string _strCSortOrder;
        int _intRIndex;
        int _intCIndex;
        string _controlToAddErrorMessage = string.Empty;
        bool _drawPanel;
        bool _AllowChildSort;
        XmlNodeList _xDoc;
        bool _ShowExpandCollapseImage;
        XmlNode _ChildTableNode;
        XmlNamespaceManager _xmlManager;
        private object _fullDatasource;
        #endregion

        #region Constructor
        public COETemplatedColumnProperties()
        {

        }
        #endregion

        public DataSet DataSource
        {
            get { return _datasource; }
            set { _datasource = value; }
        }
        public int intChildSortColumn
        {
            get { return _intChildSortColumn; }
            set { _intChildSortColumn = value; }
        }
        public string strValue
        {
            get { return _strValue; }
            set { _strValue = value; }
        }
        public string strCSortOrder
        {
            get { return _strCSortOrder; }
            set { _strCSortOrder = value; }
        }
        public int intRIndex
        {
            get { return _intRIndex; }
            set { _intRIndex = value; }
        }
        public int intCIndex
        {
            get { return _intCIndex; }
            set { _intCIndex = value; }
        }
        public bool drawPanel
        {
            get { return _drawPanel; }
            set { _drawPanel = value; }
        }
        public bool AllowChildSort
        {
            get { return _AllowChildSort; }
            set { _AllowChildSort = value; }
        }
        public string strFilterExp
        {
            get { return _strFilterExp; }
            set { _strFilterExp = value; }
        }
        public bool childExpanded
        {
            get { return _ChildExpanded; }
            set { _ChildExpanded = value; }
        }
        public string childTableName
        {
            get { return _childTableName; }
            set { _childTableName = value; }
        }
        public bool isFirstRow
        {
            get { return _isFirstRow; }
            set { _isFirstRow = value; }
        }
        public string imageFolder
        {
            get { return _imageFolder; }
            set { _imageFolder = value; }
        }
        public string expandImage
        {
            get { return _expandImage; }
            set { _expandImage = value; }
        }
        public string collapseImage
        {
            get { return _collapseImage; }
            set { _collapseImage = value; }
        }
        public string fontSizeInPoint
        {
            get { return _fontSizeInPoint; }
            set { _fontSizeInPoint = value; }
        }
        public XmlNodeList xDoc
        {
            get { return _xDoc; }
            set { _xDoc = value; }
        }
        public ScriptManager scManager
        {
            get { return _sm; }
            set { _sm = value; }
        }
        public bool ShowExpandCollapseImage
        {
            get { return _ShowExpandCollapseImage; }
            set { _ShowExpandCollapseImage = value; }
        }
        public XmlNode ChildTableNode
        {
            get { return _ChildTableNode; }
            set { _ChildTableNode = value; }
        }
        public XmlNamespaceManager XMLManager
        {
            get { return _xmlManager; }
            set { _xmlManager = value; }
        }

        public object FullDatasource
        {
            set { _fullDatasource = value; }
            get { return _fullDatasource; }
        }

    }

    #endregion

    #region UpdatePanelClass

    public class UpdatePanelContent : ITemplate
    {
        public Control m_Control;
        public UpdatePanelContent(Control controlToAdd)
        {
            m_Control = controlToAdd;
        }
        public void InstantiateIn(Control container)
        {
            container.Controls.Add(m_Control);
        }
    }

    #endregion

    #region HeaderTemplate Class
    public class HeaderExpandTemplate : ITemplate
    {

        #region Properties
        public string SortField
        {
            get
            {
                return this.Page.Session["SortField"] as string;
            }
            set
            {
                this.Page.Session["SortField"] = value;
            }
        }
        public string SortDirection
        {
            get
            {
                return this.Page.Session["SortDirection"] as string;
            }
            set
            {
                this.Page.Session["SortDirection"] = value;
            }
        }
        public Page Page
        {
            get
            {
                return (Page)System.Web.HttpContext.Current.CurrentHandler;
            }
        }
        #endregion

        #region Variables
        string _cssClass;
        bool _allowColumnSorting;
        Color _hdrClr, _hdrBackClr;
        FontUnit _hdrFontSize = FontUnit.Small;
        string _hdrFontFamily;
        bool _hdrFontWeight = false;
        string _strHeaderText;
        XmlNode _headerStyleNode;
        CommandEventHandler _columnClickedHandler;
        MarkAllHitsHandler _markAllHitsHandler;
        bool _showCheckBox = false;
        string _sortedDirections = string.Empty;
        string _sortedFields = string.Empty;
        List<HtmlGenericControl> _div = new List<HtmlGenericControl>();
        List<HtmlImage> _sortedImage = new List<HtmlImage>();
        List<LinkButton> _linkButton = new List<LinkButton>();
        CheckBox _checkBox = null;
        XmlNode _childTableNode;
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEFormGenerator");
        #endregion

        #region Constructors
        public HeaderExpandTemplate(bool showCheckBox, string cssClass, bool allowColumnSorting, Color hdrClr, Color hdrBackClr, FontUnit funit, string ffamily, bool fweight, string headerText, XmlNode headerStyleNode, CommandEventHandler columnClickedHandler, string sortedFields, string sortedDirections, MarkAllHitsHandler markAllHandler)
        {
            _showCheckBox = showCheckBox;
            _cssClass = cssClass;
            _allowColumnSorting = allowColumnSorting;
            _hdrBackClr = hdrBackClr;
            _hdrClr = hdrClr;
            _hdrFontSize = funit;
            _hdrFontFamily = ffamily;
            _hdrFontWeight = fweight;
            _strHeaderText = headerText;
            _headerStyleNode = headerStyleNode;
            _columnClickedHandler = columnClickedHandler;
            _sortedDirections = sortedDirections == null ? string.Empty : sortedDirections;
            _sortedFields = sortedFields == null ? string.Empty : sortedFields;
            _markAllHitsHandler = markAllHandler;
        }

        public HeaderExpandTemplate(bool showCheckBox, string cssClass, bool allowColumnSorting, Color hdrClr, Color hdrBackClr, FontUnit funit, string ffamily, bool fweight, string headerText, XmlNode headerStyleNode, CommandEventHandler columnClickedHandler, string sortedFields, string sortedDirections, MarkAllHitsHandler markAllHandler, XmlNode childTableNode)
            : this(showCheckBox, cssClass, allowColumnSorting, hdrClr, hdrBackClr, funit, ffamily, fweight, headerText, headerStyleNode, columnClickedHandler, sortedFields, sortedDirections, markAllHandler)
        {
            _childTableNode = childTableNode;
        }
        #endregion

        #region Instantiante IN
        public void InstantiateIn(Control control)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            _div.Clear();
            _sortedImage.Clear();
            _linkButton.Clear();

            bool showOrderingArrow = false;
            Table tblMain = new Table();
            TableRow trMain = new TableRow();
            TableCell tc2Main = new TableCell();
            HtmlGenericControl pan = new HtmlGenericControl();
            pan.TagName = "div";
            pan.Style.Add(HtmlTextWriterStyle.Overflow, "hidden");
            pan.Attributes.Add("onmouseover", "this.title=this.innerText;");            
            tc2Main.Controls.Add(pan);
            control.Controls.Add(tblMain);
            trMain.Cells.Add(tc2Main);
            this.AddJavascripts();

            control.PreRender += new EventHandler(control_PreRender);

            HeaderItem headeritem = (HeaderItem)control;
            string[] strColumnText = _strHeaderText.Split(',');
            int j = strColumnText.Length;

            if (j <= 1 && !_showCheckBox)
            {

                if (_allowColumnSorting)
                {
                    control.Controls.Add(this.CreateSortingDiv(strColumnText[0], ref showOrderingArrow));
                    pan.Controls.Add(_linkButton[0]);
                    _linkButton[0].Text = strColumnText[0];
                    if (showOrderingArrow)
                    {
                        TableCell arrowCell = new TableCell();
                        arrowCell.Controls.Add(_sortedImage[0]);
                        trMain.Cells.Add(arrowCell);
                    }
                }
                else
                    pan.InnerText = strColumnText[0];
            }
            else if (!_showCheckBox)
            {
                TableRow row1 = new TableRow();
                int k = 0;
                for (int i = 0; i < j; i = i + 2)
                {
                    if (strColumnText[i] != null && strColumnText[i] != string.Empty)
                    {
                        TableCell cell1 = new TableCell();
                        HtmlGenericControl pan1 = new HtmlGenericControl();
                        pan1.TagName = "div";
                        pan1.Style.Add(HtmlTextWriterStyle.Overflow, "hidden");
                        pan1.Attributes.Add("onmouseover", "this.title=this.innerText;");                        
                        cell1.Controls.Add(pan1);

                        cell1.ID = System.Web.HttpUtility.HtmlEncode(strColumnText[i]) + "HeaderCell";
                        XmlNamespaceManager xmlManager = new XmlNamespaceManager(_childTableNode.OwnerDocument.NameTable);
                        xmlManager.AddNamespace("COE", _childTableNode.OwnerDocument.DocumentElement.NamespaceURI);

                        if (strColumnText[i].Trim() != string.Empty)
                        {
                            XmlNode columnNode = _childTableNode.SelectSingleNode("//COE:Column[COE:headerText='" + strColumnText[i] + "']", xmlManager) != null ?
                                _childTableNode.SelectSingleNode("//COE:Column[COE:headerText='" + strColumnText[i] + "']", xmlManager) :
                                _childTableNode.SelectSingleNode("//COE:Column[@name='" + strColumnText[i] + "']", xmlManager);

                            _allowColumnSorting = _allowColumnSorting && columnNode.Attributes["allowSorting"] != null && (columnNode.Attributes["allowSorting"].Value.ToLower() == "yes" || columnNode.Attributes["allowSorting"].Value.ToLower() == "true");
                            string sortedDirection = string.Empty;
                            if (this.SortField == columnNode.Attributes["name"].Value)
                                sortedDirection = this.SortDirection;
                        }
                        else
                        {
                            _allowColumnSorting = false;
                        }
                        if (_allowColumnSorting)
                        {
                            pan1.Controls.Add(this.CreateSortingDiv(strColumnText[i], ref showOrderingArrow));
                            pan1.Controls.Add(_linkButton[k]);
                            _linkButton[k].Text = strColumnText[i];

                        }
                        else
                        {
                            pan1.InnerText = strColumnText[i];
                            if (strColumnText[i + 1].Trim() != string.Empty)
                                pan1.Style.Add(HtmlTextWriterStyle.Width, strColumnText[i + 1]);
                        }
                        row1.Cells.Add(cell1);
                        if (showOrderingArrow)
                        {
                            TableCell arrowCell = new TableCell();
                            arrowCell.Controls.Add(_sortedImage[k]);
                            row1.Cells.Add(arrowCell);
                        }
                        k++;
                    }
                }

                Table tbl1 = this.SetHeaderStyle();

                if (!string.IsNullOrEmpty(_cssClass))
                {
                    tbl1.CssClass = _cssClass;
                }

                tbl1.Rows.Add(row1);
                tc2Main.Controls.Add(tbl1);
            }
            else
            {
                _checkBox = new CheckBox();
                tc2Main.Controls.Add(_checkBox);

                _checkBox.ID = "checkboxheader";
                _checkBox.AutoPostBack = true;

                if (_markAllHitsHandler != null)
                    _checkBox.CheckedChanged += new EventHandler(CheckBox_CheckedChanged);
                _checkBox.DataBinding += new EventHandler(_checkBox_DataBinding);
                _checkBox.Text = strColumnText[0];
                _checkBox.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");

            }

            tblMain.Rows.Add(trMain);
            _coeLog.LogEnd(methodSignature);

        }
        #endregion

        #region Control's Pre render
        void control_PreRender(object sender, EventArgs e)
        {
            if (_checkBox != null)
            {
                ScriptManager manager = ScriptManager.GetCurrent(((Control)sender).Page);
                if (manager != null)
                    manager.RegisterPostBackControl(_checkBox);
            }
            for (int i = 0; i < _div.Count; i++)
            {
                if (((LinkButton)_div[i].Controls[0]).Enabled)
                {
                    _sortedImage[i].Attributes.Add("onclick", "ShowDiv('" + _div[i].ClientID + "', '" + _div[i].Controls[0].ClientID + "')");
                    _linkButton[i].OnClientClick = "ShowDiv('" + _div[i].ClientID + "', '" + _div[i].Controls[0].ClientID + "'); return false;";
                    ((LinkButton)_div[i].Controls[0]).Attributes.Add("onblur", "window.setTimeout(\"HideDiv('" + _div[i].ClientID + "')\", 300)");
                }
                else
                {
                    _sortedImage[i].Attributes.Add("onclick", "ShowDiv('" + _div[i].ClientID + "', '" + _div[i].Controls[1].ClientID + "')");
                    _linkButton[i].OnClientClick = "ShowDiv('" + _div[i].ClientID + "', '" + _div[i].Controls[1].ClientID + "'); return false;";
                    ((LinkButton)_div[i].Controls[1]).Attributes.Add("onblur", "window.setTimeout(\"HideDiv('" + _div[i].ClientID + "')\", 300)");
                }
            }
        }
        #endregion

        #region EventHandlers
        void _checkBox_DataBinding(object sender, EventArgs e)
        {
            DataTable baseTable = ((DataSet)((Infragistics.WebUI.UltraWebGrid.UltraWebGrid)((Control)sender).NamingContainer.NamingContainer).DataSource).Tables[0];

            string basecolumnName = ((Infragistics.WebUI.UltraWebGrid.HeaderItem)(_checkBox.Parent.NamingContainer)).Column.BaseColumnName;
            bool allMarked = true;
            foreach (DataRow row in baseTable.Rows)
            {
                if (row[basecolumnName] != null)
                {
                    if (row[basecolumnName].ToString().ToLower() == "0" || row[basecolumnName].ToString().ToLower() == "false" || row[basecolumnName] is System.DBNull)
                    {
                        allMarked = false;
                        break;
                    }
                }
            }
            _checkBox.Checked = allMarked;
            if (this.Page != null && this.Page.Master is GUIShell.GUIShellMaster)
            {
                string message = string.Empty;
                if (_checkBox.Checked)
                    message = Resources.UnmarkingAllHits;
                else
                    message = Resources.MarkingAllHits;
                ((GUIShell.GUIShellMaster)this.Page.Master).MakeCtrlShowProgressModal(_checkBox.ClientID, message, string.Empty, false);
            }
        }

        void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _markAllHitsHandler(this, new MarkAllHitsEventArgs(_checkBox.Checked));
        }
        #endregion
        #region Private Methods
        private void AddJavascripts()
        {
            string showDivJS = @"
            function ShowDiv(divID, elementToFocusID){
                var panel = document.getElementById(divID);
                var elementToFocus = document.getElementById(elementToFocusID);
                if(panel.style.display == 'none')
                {
                    panel.style.display = 'block';
                    elementToFocus.focus();
                }
                else
                    panel.style.display = 'none';
            }

            function HideDiv(panelID)
            {
                var panel = document.getElementById(panelID);
                panel.style.display = 'none';
            }
            ";
            if (!Page.ClientScript.IsClientScriptBlockRegistered(typeof(HeaderExpandTemplate), "ShowDivJS"))
                Page.ClientScript.RegisterClientScriptBlock(typeof(HeaderExpandTemplate), "ShowDivJS", showDivJS, true);
        }

        private HtmlGenericControl CreateSortingDiv(string headerText, ref bool showOrderingArrow)
        {
            _linkButton.Add(new LinkButton());
            _linkButton[_linkButton.Count - 1].ID = headerText + "OrderDefaultLinkButton";
            _linkButton[_linkButton.Count - 1].CssClass = "WebGridHeaderLinkButton";

            _sortedImage.Add(new HtmlImage());
            _sortedImage[_sortedImage.Count - 1].ID = headerText + "SortedImage";
            _sortedImage[_sortedImage.Count - 1].Style.Add(HtmlTextWriterStyle.Position, "relative");
            _sortedImage[_sortedImage.Count - 1].Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
            string[] sortedFields = _sortedFields.Split(',');
            string[] sortedDirections = _sortedDirections.Split(',');
            int sortFieldIndex = -1;
            for (int i = 0; i < sortedFields.Length; i++)
            {
                if (sortedFields[i] == headerText)
                {
                    sortFieldIndex = i;
                    break;
                }
            }

            if (sortFieldIndex > -1)
            {

                if (sortedDirections[sortFieldIndex] == "ASC")
                {
                    _sortedImage[_sortedImage.Count - 1].Src = Page.ClientScript.GetWebResourceUrl(typeof(HeaderExpandTemplate), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.up.png");
                }
                else if (sortedDirections[sortFieldIndex] == "DESC")
                {
                    _sortedImage[_sortedImage.Count - 1].Src = Page.ClientScript.GetWebResourceUrl(typeof(HeaderExpandTemplate), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.down.png");
                }
                showOrderingArrow = true;
            }
            else
                showOrderingArrow = false;

            LinkButton ascendingLinkButton = new LinkButton();
            LinkButton descendingLinkButton = new LinkButton();
            _div.Add(new HtmlGenericControl("div"));
            _div[_div.Count - 1].Attributes.Add("class", "MenuItemContainer");
            _div[_div.Count - 1].ID = headerText + "DirectionContainer";
            ascendingLinkButton.ID = headerText + "OrderAscendingLinkButton";
            ascendingLinkButton.Text = "ASC";
            ascendingLinkButton.CssClass = "SortingMenuItem";
            ascendingLinkButton.Style.Add(HtmlTextWriterStyle.Display, "block");
            descendingLinkButton.ID = headerText + "OrderDescendingLinkButton";
            descendingLinkButton.Text = "DESC";
            descendingLinkButton.Style.Add(HtmlTextWriterStyle.Display, "block");
            descendingLinkButton.CssClass = "SortingMenuItem";
            _div[_div.Count - 1].Style.Add(HtmlTextWriterStyle.Display, "none");
            if (_columnClickedHandler != null)
            {
                showOrderingArrow = showOrderingArrow && true;
                string orderDirection = string.Empty;
                if (sortFieldIndex > -1)
                {
                    if (sortedDirections[sortFieldIndex] == "ASC")
                    {
                        orderDirection = "DESC";
                        descendingLinkButton.CommandName = "OrderCommand";
                        descendingLinkButton.CommandArgument = headerText + ",DESC";
                        descendingLinkButton.Command += new CommandEventHandler(_columnClickedHandler);
                        ascendingLinkButton.Enabled = false;

                    }
                    else if (sortedDirections[sortFieldIndex] == "DESC")
                    {
                        orderDirection = "ASC";
                        ascendingLinkButton.CommandName = "OrderCommand";
                        ascendingLinkButton.CommandArgument = headerText + ",ASC";
                        ascendingLinkButton.Command += new CommandEventHandler(_columnClickedHandler);
                        descendingLinkButton.Enabled = false;
                    }
                }
                else
                {
                    orderDirection = "ASC";
                    ascendingLinkButton.CommandName = "OrderCommand";
                    ascendingLinkButton.CommandArgument = headerText + ",ASC";
                    ascendingLinkButton.Command += new CommandEventHandler(_columnClickedHandler);
                    descendingLinkButton.CommandName = "OrderCommand";
                    descendingLinkButton.CommandArgument = headerText + ",DESC";
                    descendingLinkButton.Command += new CommandEventHandler(_columnClickedHandler);
                }
            }
            _div[_div.Count - 1].Controls.Add(ascendingLinkButton);
            _div[_div.Count - 1].Controls.Add(descendingLinkButton);

            ScriptManager scm = ScriptManager.GetCurrent(this.Page);
            if (scm != null)
            {
                //scm.RegisterPostBackControl(_linkButton);
                scm.RegisterPostBackControl(ascendingLinkButton);
                scm.RegisterPostBackControl(descendingLinkButton);
            }

            return _div[_div.Count - 1];
        }

        private Table SetHeaderStyle()
        {
            Table tbl1 = new Table();
            if (_headerStyleNode != null && _headerStyleNode.InnerText.Length > 0)
            {
                string[] styles = _headerStyleNode.InnerText.Split(new char[1] { ';' });
                styles[0] = styles[0].Trim();
                styles[1] = styles[1].Trim();
                for (int i = 0; i < styles.Length; i++)
                {
                    if (styles[i].Length > 0)
                    {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        switch (styleDef[0].Trim())
                        {
                            case "background-color":
                                System.Drawing.Color color = new System.Drawing.Color();
                                if (styles[1].ToLower().Contains("rgb("))
                                {
                                    styles[1] = styles[1].Remove(0, styles[1].IndexOf("(") + 1);
                                    styles[1] = styles[1].Remove(styles[1].Length - 1);
                                    string[] rgb = styles[1].Split(',');
                                    tbl1.BackColor = System.Drawing.Color.FromArgb(int.Parse(rgb[0].Trim()), int.Parse(rgb[1].Trim()), int.Parse(rgb[2].Trim()));
                                }
                                else
                                {
                                    color = System.Drawing.Color.FromName(styleDef[1]);
                                    tbl1.BackColor = color;
                                }
                                break;
                            case "color":
                                color = System.Drawing.Color.FromName(styleDef[1]);
                                tbl1.ForeColor = color;
                                break;

                            case "border-color":
                                {
                                    color = new System.Drawing.Color();
                                    if (styles[1].ToLower().Contains("rgb("))
                                    {
                                        styles[1] = styles[1].Remove(0, styles[1].IndexOf("(") + 1);
                                        styles[1] = styles[1].Remove(styles[1].Length - 1);
                                        string[] rgb = styles[1].Split(',');

                                        tbl1.BorderColor = color;
                                    }
                                    else
                                    {
                                        color = System.Drawing.Color.FromName(styleDef[1]);
                                        tbl1.BorderColor = color;
                                    }
                                    break;
                                }
                            case "border-width":
                                {
                                    tbl1.BorderWidth = new Unit(styleDef[1]);
                                    break;
                                }
                            case "border-style":
                                {
                                    switch (styleDef[1].Trim().ToLower())
                                    {
                                        case "dashed":
                                            tbl1.BorderStyle = BorderStyle.Dashed;
                                            break;
                                        case "dotted":
                                            tbl1.BorderStyle = BorderStyle.Dotted;
                                            break;
                                        case "double":
                                            tbl1.BorderStyle = BorderStyle.Double;
                                            break;
                                        case "groove":
                                            tbl1.BorderStyle = BorderStyle.Groove;
                                            break;
                                        case "inset":
                                            tbl1.BorderStyle = BorderStyle.Inset;
                                            break;
                                        case "solid":
                                            tbl1.BorderStyle = BorderStyle.Solid;
                                            break;
                                        case "ridge":
                                            tbl1.BorderStyle = BorderStyle.Ridge;
                                            break;
                                        case "outset":
                                            tbl1.BorderStyle = BorderStyle.Outset;
                                            break;
                                        case "notset":
                                            tbl1.BorderStyle = BorderStyle.NotSet;
                                            break;
                                        case "none":
                                            tbl1.BorderStyle = BorderStyle.None;
                                            break;
                                    }
                                    break;
                                }
                            case "font-weight":
                                tbl1.Font.Bold = styleDef[1].ToLower().Contains("bold");
                                break;

                            case "font-family":
                                tbl1.Font.Name = styleDef[1];
                                break;

                            case "font-size":
                                tbl1.Font.Size = new FontUnit(new Unit(styleDef[1]));
                                break;

                            case "text-align":
                                switch (styleDef[1].Trim().ToLower())
                                {
                                    case "left":
                                        tbl1.HorizontalAlign = HorizontalAlign.Left;
                                        break;
                                    case "right":
                                        tbl1.HorizontalAlign = HorizontalAlign.Right;
                                        break;
                                    case "tbl1":
                                        tbl1.HorizontalAlign = HorizontalAlign.Center;
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
            return tbl1;
        }
        #endregion
    }
    #endregion

    #region NestedGrid Column Class

    public class NetGridTemplate : ITemplate
    {
        private DataSet datasource_ = new DataSet();
        string strFilterExp_ = string.Empty;
        bool cExpanded_;
        string childTableName_ = string.Empty;
        bool isFirstRow_ = true;
        string imageFolder_;
        string expandImage_;
        string collapseImage_;
        string fontSizeInPoint_;
        DataView dv = null;
        DataGrid grd;
        XmlNode tableNode;
        XmlNodeList xDoc;
        ScriptManager sm_;
        int intChildSortColumn;
        string strValue;
        string strCSortOrder;
        int intRIndex;
        int intCIndex;
        string controlToAddErrorMessage = string.Empty;
        bool drawPanel;
        bool AllowChildSort;
        bool ShowExpandCollapseImage;
        XmlNode childTableNode;
        XmlNamespaceManager XmlManager;
        //private UpdatePanel _uPanel;
        private object _fullDatasource;
        private CultureInfo _displayCulture;
        public event MarkingHitHandler MarkingHit;

        protected static IDictionary trackChild;

        public NetGridTemplate()
        {
            datasource_ = null;
            strFilterExp_ = null;
            childTableName_ = null;
            imageFolder_ = null;
            expandImage_ = null;
            collapseImage_ = null;
            fontSizeInPoint_ = null;

            intChildSortColumn = 0;
            strValue = "";
            strCSortOrder = "";
            intRIndex = 0;
            intCIndex = 0;

            tableNode = new XmlDocument().CreateNode(XmlNodeType.Element, "tableNode", "COE.FormGroup");
            if (trackChild == null)
                trackChild = new Dictionary<string, object>();
            xDoc = null;
            drawPanel = true;
        }

        public NetGridTemplate(COETemplatedColumnProperties props, CultureInfo displayCulture)
        {
            datasource_ = props.DataSource;
            strFilterExp_ = props.strFilterExp;
            cExpanded_ = props.childExpanded;
            childTableName_ = props.childTableName;
            imageFolder_ = props.imageFolder;
            expandImage_ = props.expandImage;
            collapseImage_ = props.collapseImage;
            fontSizeInPoint_ = props.fontSizeInPoint;
            tableNode = new XmlDocument().CreateNode(XmlNodeType.Element, "tableNode", "COE.FormGroup");
            xDoc = props.xDoc;
            drawPanel = props.drawPanel;
            sm_ = props.scManager;
            intChildSortColumn = props.intChildSortColumn;
            strValue = props.strValue;
            strCSortOrder = props.strCSortOrder;
            intRIndex = props.intRIndex;
            intCIndex = props.intCIndex;
            AllowChildSort = props.AllowChildSort;
            ShowExpandCollapseImage = props.ShowExpandCollapseImage;
            childTableNode = props.ChildTableNode;
            XmlManager = props.XMLManager;
            _fullDatasource = props.FullDatasource;
            _displayCulture = displayCulture;
        }

        public NetGridTemplate(DataSet mainDS, string sortExp, bool expandChild, string childTable,
            string imageFolder, string expandImage, string collapseImage,
            string fontSizeInPoint, XmlNode tableNodeParam, bool Panel, ScriptManager _sm,
            int intCNo, string strPValue, string strCSOrder, int _Rindex, int _Cindex, bool AllowCSort)
        {
            datasource_ = mainDS;
            strFilterExp_ = sortExp;
            cExpanded_ = expandChild;
            childTableName_ = childTable;
            imageFolder_ = imageFolder;
            expandImage_ = expandImage;
            collapseImage_ = collapseImage;
            fontSizeInPoint_ = fontSizeInPoint;
            tableNode = tableNodeParam;
            xDoc = null;
            drawPanel = Panel;
            intChildSortColumn = intCNo;
            strValue = strPValue;
            strCSortOrder = strCSOrder;
            intRIndex = _Rindex;
            intCIndex = _Cindex;
            AllowChildSort = AllowCSort;
        }

        public NetGridTemplate(DataSet mainDS, string sortExp, bool expandChild, string childTable,
            string imageFolder, string expandImage, string collapseImage,
            string fontSizeInPoint, int i, bool panel)
        {
            datasource_ = mainDS;
            strFilterExp_ = sortExp;
            cExpanded_ = expandChild;
            childTableName_ = childTable;
            imageFolder_ = imageFolder;
            expandImage_ = expandImage;
            collapseImage_ = collapseImage;
            fontSizeInPoint_ = fontSizeInPoint;
            tableNode = new XmlDocument().CreateNode(XmlNodeType.Element, "tableNode", "COE.FormGroup");
            xDoc = null;
            drawPanel = panel;
        }

        public NetGridTemplate(DataSet mainDS, string sortExp, bool expandChild, string childTable,
            string imageFolder, string expandImage, string collapseImage,
            string fontSizeInPoint, XmlNodeList doc, bool Panel, ScriptManager _sm, int intCNo,
            string strPValue, string strCSOrder, int _Rindex, int _Cindex, bool AllowCSort)
        {
            datasource_ = mainDS;
            strFilterExp_ = sortExp;
            cExpanded_ = expandChild;
            childTableName_ = childTable;
            imageFolder_ = imageFolder;
            expandImage_ = expandImage;
            collapseImage_ = collapseImage;
            fontSizeInPoint_ = fontSizeInPoint;
            tableNode = new XmlDocument().CreateNode(XmlNodeType.Element, "tableNode", "COE.FormGroup");
            xDoc = doc;
            drawPanel = Panel;
            sm_ = _sm;
            intChildSortColumn = intCNo;
            strValue = strPValue;
            strCSortOrder = strCSOrder;
            intRIndex = _Rindex;
            intCIndex = _Cindex;
            AllowChildSort = AllowCSort;
        }

        //Required by any class implementing ITemplate.  
        public void InstantiateIn(Control control)
        {
            //Create a MS Grid and label controls, and add them to the Group‘s Controls collection.

            control.Init += new EventHandler(control_Init);
        }

        void control_Init(object sender, EventArgs e)
        {
            Table tbl = new Table();
            TableRow tr = new TableRow();
            TableRow tr1 = new TableRow();
            TableCell tc = new TableCell();
            TableCell tc1 = new TableCell();
            TableCell tc2 = new TableCell();
            TableCell tc3 = new TableCell();
            Label lbl = new Label();
            CellItem cellitem = (CellItem)sender;

            // Coverity Fix CID : 13156            
            try
            {
                //Coverity Bug Fix (local Analysis)
                if (cellitem == null) return;

                int intCellRindex = -1;
                int intCellCindex = -1;

                grd = new DataGrid();
                grd.ID = "GridView1";
                grd.EnableViewState = true;


                lbl.ID = "Label1";


                intCellRindex = cellitem.Cell.Row.Index;
                intCellCindex = cellitem.Cell.Column.Index;

                //string strCellRID = cellitem.Cell.RenderID;
                //strCell = cellitem.Cell.RenderID.;
                if (fontSizeInPoint_ != string.Empty)
                {
                    try
                    {
                        grd.Font.Size = FontUnit.Point(int.Parse(fontSizeInPoint_));
                    }
                    catch (Exception ex) { }
                }

                lbl.Text = childTableName_ + "  ";

                DataTable dt = datasource_.Tables[childTableName_];
                dv = new DataView(dt);

                // Coverity Fix CID - 13081
                if (cellitem.Cell.Value != null)
                    dv.RowFilter = strFilterExp_ + " = '" + cellitem.Cell.Value.ToString() + "'";

                if (AllowChildSort)
                    grd.AllowSorting = true;
                else
                    grd.AllowSorting = false;

                grd.ItemDataBound += new DataGridItemEventHandler(grd_ItemDataBound);

                sm_.RegisterAsyncPostBackControl(grd);

                System.Web.UI.WebControls.ImageButton img = new System.Web.UI.WebControls.ImageButton();

                if (dv.Count != 0)
                {
                    try
                    {
                        if (childTableNode != null)
                        {
                            grd.AutoGenerateColumns = false;
                            XmlNamespaceManager xmlManager = new XmlNamespaceManager(childTableNode.OwnerDocument.NameTable);
                            xmlManager.AddNamespace("COE", childTableNode.OwnerDocument.DocumentElement.NamespaceURI);


                            XmlNode xCSS = childTableNode.SelectSingleNode("COE:CSSClass", xmlManager);
                            if (xCSS != null && xCSS.InnerXml != null)
                                grd.CssClass = xCSS.InnerXml;

                            this.SetColumnStyle();

                            XmlNode Columns = childTableNode.SelectSingleNode("COE:Columns", xmlManager);
                            //for Coverity bug fix - CID 13133
                            if (Columns != null)
                            {
                                XmlNodeList ColumnList = Columns.SelectNodes("COE:Column", xmlManager);

                                foreach (XmlNode cNode in ColumnList)
                                {
                                    XmlNode formElementNode = cNode.SelectSingleNode("./COE:formElement", xmlManager);

                                    string colName = string.Empty;
                                    string headerText = string.Empty;
                                    if (cNode.SelectSingleNode("./COE:headerText", xmlManager) != null)
                                        headerText = cNode.SelectSingleNode("./COE:headerText", xmlManager).InnerText;
                                    if (cNode.Attributes["name"] != null)
                                        colName = cNode.Attributes["name"].Value;
                                    if (string.IsNullOrEmpty(headerText))
                                        headerText = colName;

                                    XmlNode width = cNode.SelectSingleNode("./COE:width", xmlManager);

                                    if (formElementNode != null && formElementNode.SelectSingleNode("COE:displayInfo", xmlManager) != null && formElementNode.SelectSingleNode("COE:displayInfo", xmlManager).SelectSingleNode("COE:type", xmlManager) != null)
                                    {
                                        TemplateColumn formElementColumn = new TemplateColumn();
                                        BoundColumn col = new BoundColumn();

                                        ChildGridTemplate frmColumn = new ChildGridTemplate(cNode.SelectNodes("COE:formElement", xmlManager), colName, dv, _fullDatasource, _displayCulture);
                                        formElementColumn.ItemTemplate = frmColumn;

                                        if (width != null && width.InnerText.Length > 0)
                                        {
                                            col.ItemStyle.Width = formElementColumn.ItemStyle.Width = new Unit(width.InnerText);
                                            col.HeaderStyle.Width = formElementColumn.HeaderStyle.Width = new Unit(width.InnerText);
                                        }
                                        frmColumn.MarkingHit += new MarkingHitHandler(frmColumn_MarkingHit);
                                        grd.Columns.Add(formElementColumn);


                                        col.HeaderText = headerText;
                                        col.DataField = colName;
                                        col.Visible = false;
                                        grd.Columns.Add(col);

                                    }
                                    else
                                    {
                                        BoundColumn col = new BoundColumn();
                                        col.HeaderText = headerText;
                                        col.DataField = colName;
                                        col.ReadOnly = true;
                                        col.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                                        if (width != null)
                                            col.HeaderStyle.Width = col.ItemStyle.Width = new Unit(width.InnerText);
                                        grd.Columns.Add(col);
                                    }
                                }
                            }
                        }

                        if (ShowExpandCollapseImage)
                        {
                            img.ID = "ImageControl1";
                            img.EnableViewState = false;

                            string path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;

                            if (!cExpanded_)
                            {
                                img.ImageUrl = "/COECommonResources/infragistics/20111CLR20/Images/plus.gif";
                            }
                            else
                            {
                                img.ImageUrl = "/COECommonResources/infragistics/20111CLR20/Images/minus.gif";
                            }
                            sm_.RegisterAsyncPostBackControl(img);

                            grd.DataSource = null;

                            if (intChildSortColumn >= 0 && intRIndex == intCellRindex && intCIndex == intCellCindex)
                            {
                                dv.Sort = dv.Table.Columns[intChildSortColumn].ColumnName + "  " + strCSortOrder.ToUpper(); //+ "DESC";
                                grd.DataSource = dv;
                                img.ImageUrl = "/COECommonResources/infragistics/20111CLR20/Images/minus.gif";
                            }
                            grd.DataBind();
                            img.Click += new ImageClickEventHandler(img_Click);
                        }
                        else
                        {
                            grd.DataSource = dv;
                            grd.DataBind();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                //grd.Style.Add("table-layout", "fixed");

                lbl.Visible = false;
                tc1.VerticalAlign = VerticalAlign.Top;
                if (ShowExpandCollapseImage)
                    tc1.Controls.Add(img);
                //tc.Text = cellitem.Cell.Text;
                //tc.Width = Unit.Pixel(25);
                tc2.Controls.Add(grd);
                tc2.Controls.Add(lbl);
                tr.Cells.Add(tc1);
                tr.Cells.Add(tc);
                tr.Cells.Add(tc2);
                tbl.Rows.Add(tr);

                //_uPanel = new UpdatePanel();
                //_uPanel.ID = "UpdatePanel10";
                //_uPanel.Load += new EventHandler(UpdatePanel1_Load);
                //_uPanel.Unload += new EventHandler(UpdatePanel1_Unload);
                //_uPanel.UpdateMode = UpdatePanelUpdateMode.Conditional;
                //UpdatePanelContent panelContent = new UpdatePanelContent(tbl);
                //_uPanel.ContentTemplate = panelContent;
                //if(drawPanel)
                //{
                //    cellitem.Controls.Remove(_uPanel);
                //}
                cellitem.Controls.Add(tbl);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cellitem.Dispose();
                lbl.Dispose();
                tc3.Dispose();
                tc2.Dispose();
                tc1.Dispose();
                tc.Dispose();
                tr1.Dispose();
                tr.Dispose();
                tbl.Dispose();
            }
        }

        private void SetColumnStyle()
        {
            XmlNamespaceManager xmlManager = new XmlNamespaceManager(childTableNode.OwnerDocument.NameTable);
            xmlManager.AddNamespace("COE", childTableNode.OwnerDocument.DocumentElement.NamespaceURI);

            XmlNode columnStyle = childTableNode.SelectSingleNode("COE:columnStyle", xmlManager);

            if (columnStyle != null && columnStyle.InnerText.Length > 0)
            {
                string[] styles = columnStyle.InnerText.Split(new char[1] { ';' });
                styles[0] = styles[0].Trim();
                styles[1] = styles[1].Trim();
                for (int i = 0; i < styles.Length; i++)
                {
                    if (styles[i].Length > 0)
                    {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        switch (styleDef[0].Trim())
                        {
                            case "background-color":
                                System.Drawing.Color color = new System.Drawing.Color();
                                if (styles[1].ToLower().Contains("rgb("))
                                {
                                    styles[1] = styles[1].Remove(0, styles[1].IndexOf("(") + 1);
                                    styles[1] = styles[1].Remove(styles[1].Length - 1);
                                    string[] rgb = styles[1].Split(',');

                                    grd.BackColor = System.Drawing.Color.FromArgb(int.Parse(rgb[0].Trim()), int.Parse(rgb[1].Trim()), int.Parse(rgb[2].Trim()));
                                }
                                else
                                {
                                    color = System.Drawing.Color.FromName(styleDef[1]);
                                    grd.BackColor = color;
                                }
                                break;
                            case "color":
                                color = System.Drawing.Color.FromName(styleDef[1]);
                                grd.ForeColor = color;
                                break;

                            case "border-color":
                                {
                                    color = new System.Drawing.Color();
                                    if (styles[1].ToLower().Contains("rgb("))
                                    {
                                        styles[1] = styles[1].Remove(0, styles[1].IndexOf("(") + 1);
                                        styles[1] = styles[1].Remove(styles[1].Length - 1);
                                        string[] rgb = styles[1].Split(',');

                                        grd.BorderColor = color;
                                    }
                                    else
                                    {
                                        color = System.Drawing.Color.FromName(styleDef[1]);
                                        grd.BorderColor = color;
                                    }
                                    break;
                                }
                            case "border-width":
                                {
                                    grd.BorderWidth = new Unit(styleDef[1]);
                                    break;
                                }
                            case "border-style":
                                {
                                    switch (styleDef[1].Trim().ToLower())
                                    {
                                        case "dashed":
                                            grd.BorderStyle = BorderStyle.Dashed;
                                            break;
                                        case "dotted":
                                            grd.BorderStyle = BorderStyle.Dotted;
                                            break;
                                        case "double":
                                            grd.BorderStyle = BorderStyle.Double;
                                            break;
                                        case "groove":
                                            grd.BorderStyle = BorderStyle.Groove;
                                            break;
                                        case "inset":
                                            grd.BorderStyle = BorderStyle.Inset;
                                            break;
                                        case "solid":
                                            grd.BorderStyle = BorderStyle.Solid;
                                            break;
                                        case "ridge":
                                            grd.BorderStyle = BorderStyle.Ridge;
                                            break;
                                        case "outset":
                                            grd.BorderStyle = BorderStyle.Outset;
                                            break;
                                        case "notset":
                                            grd.BorderStyle = BorderStyle.NotSet;
                                            break;
                                        case "none":
                                            grd.BorderStyle = BorderStyle.None;
                                            break;
                                    }
                                    break;
                                }

                            case "border-lines":
                                switch (styleDef[1].Trim().ToLower())
                                {
                                    case "both":
                                        grd.GridLines = GridLines.Both;
                                        break;
                                    case "vertical":
                                        grd.GridLines = GridLines.Vertical;
                                        break;
                                    case "horizontal":
                                        grd.GridLines = GridLines.Horizontal;
                                        break;
                                    case "none":
                                        grd.GridLines = GridLines.None;
                                        break;
                                }
                                break;

                            case "font-weight":
                                grd.Font.Bold = styleDef[1].ToLower().Contains("bold");
                                break;

                            case "font-family":
                                grd.Font.Name = styleDef[1];
                                break;

                            case "font-size":
                                grd.Font.Size = new FontUnit(new Unit(styleDef[1]));
                                break;

                            case "text-align":
                                switch (styleDef[1].Trim().ToLower())
                                {
                                    case "left":
                                        grd.HorizontalAlign = HorizontalAlign.Left;
                                        break;
                                    case "right":
                                        grd.HorizontalAlign = HorizontalAlign.Right;
                                        break;
                                    case "center":
                                        grd.HorizontalAlign = HorizontalAlign.Center;
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
        }

        //private void RegisterUpdatePanel(ScriptManager manager, UpdatePanel panel)
        //{
        //    Page page = (Page) System.Web.HttpContext.Current.CurrentHandler;
        //    System.Reflection.MethodInfo method = typeof(ScriptManager).GetMethod("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        //    method.Invoke(manager, new object[] { panel });

        //    for(Control parent = panel.Parent; parent != null; parent = parent.Parent)
        //    {
        //        UpdatePanel curPanel = parent as UpdatePanel;
        //        if(curPanel != null)
        //        {
        //            method.Invoke(manager, new object[] { curPanel });
        //            break;
        //        }
        //    }
        //}

        //protected void UpdatePanel1_Unload(object sender, EventArgs e)
        //{
        //    RegisterUpdatePanel(this.sm_, _uPanel);
        //}

        /*protected void UpdatePanel1_Load(object sender, EventArgs e)
        {
            RegisterUpdatePanel(this.sm_, _uPanel);
        }*/

        void frmColumn_MarkingHit(object sender, MarkHitEventArgs eventArgs)
        {
            if (MarkingHit != null)
                MarkingHit(sender, eventArgs);
        }

        void img_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton btn = (ImageButton)sender;
            string path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;

            if (btn.ImageUrl.Contains(collapseImage_))
            {
                grd.Visible = true;
                btn.ImageUrl = path + imageFolder_ + "\\" + expandImage_;
            }
            else
            {
                grd.Visible = false;
                btn.ImageUrl = path + imageFolder_ + "\\" + collapseImage_;
            }
        }

        void grd_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
                e.Item.Visible = false;
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                for (int i = 0; i < e.Item.Cells.Count; i++)
                {
                    TableCell cel = e.Item.Cells[i];

                    if (cel.Controls.Count > 0)
                    {
                        ICOEGenerableControl controlToAdd = (ICOEGenerableControl)cel.Controls[0];
                        if (e.Item.Cells[i + 1] != null && controlToAdd != null)
                        {
                            if (e.Item.Cells[i + 1].Text == "&nbsp;")
                                controlToAdd.PutData(string.Empty);
                            else
                                controlToAdd.PutData(e.Item.Cells[i + 1].Text);
                        }
                    }
                    else
                    {
                        HtmlGenericControl pan = new HtmlGenericControl();
                        pan.TagName = "div";
                        pan.Style.Add(HtmlTextWriterStyle.Overflow, "hidden");
                        pan.Attributes.Add("onmouseover", "this.title=this.innerText;");
                        pan.Attributes.Add("style", "word-break:break-all; word-wrap:break-word");
                        pan.InnerText = cel.Text;

                        //JHS 1/28/2010 - This line was passing a row index to a column
                        //causing an index out of range error
                        //pan.Style.Add(HtmlTextWriterStyle.Width, ((DataGrid) sender).Columns[e.Item.ItemIndex].ItemStyle.Width.ToString());
                        //i seems to be the equivalent of the column index.
                        pan.Style.Add(HtmlTextWriterStyle.Width, ((DataGrid)sender).Columns[i].ItemStyle.Width.ToString());


                        if (pan.InnerText == "&nbsp;")
                            pan.InnerText = string.Empty;
                        cel.Controls.Add(pan);
                    }
                }
            }
        }
    }

    #endregion

    #region NestedFormElement Column Class

    public class FormElementTemplate : ITemplate
    {
        XmlNodeList xDoc;
        string controlToAddErrorMessage = string.Empty;
        private object _fullDataSource;
        private bool _IsPostback;
        private CultureInfo _displayCulture;
        public event MarkingHitHandler MarkingHit;
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEFormGenerator");

        public FormElementTemplate(XmlNodeList doc, object fullDataSource, bool isPostback, CultureInfo displayCulture)
        {
            xDoc = doc;
            _fullDataSource = fullDataSource;
            _IsPostback = isPostback;
            _displayCulture = displayCulture;
        }

        public void InstantiateIn(Control control)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            foreach (XmlNode node in xDoc)
            {
                XmlNamespaceManager xmlManager = new XmlNamespaceManager(node.OwnerDocument.NameTable);
                xmlManager.AddNamespace("COE", node.OwnerDocument.DocumentElement.NamespaceURI);
                //Coverity fix
                XmlNode labelNode = node.SelectSingleNode("COE:label", xmlManager);
                if (labelNode != null)
                {
                    XmlNode childNode = labelNode.ChildNodes[0];
                    if (childNode != null)
                    {
                        childNode.Value = string.Empty;
                    }
                }
               
                ICOEGenerableControl controlToAdd = COEFormGenerator.COEFormGenerator.GetCOEGenerableControl(node.OuterXml.ToString(), out controlToAddErrorMessage);
                string bindingExpression = (node.Attributes["name"] != null && !string.IsNullOrEmpty(node.Attributes["name"].Value)) ?
                                            node.Attributes["name"].Value : null;

                if (controlToAdd != null)
                {
                    if (controlToAdd is ICOEFullDatasource && _fullDataSource != null)
                        ((ICOEFullDatasource)controlToAdd).FullDatasource = _fullDataSource;

                    if (controlToAdd is ICOEHitMarker)
                    {
                        ((ICOEHitMarker)controlToAdd).MarkingHit += new MarkingHitHandler(FormElementTemplate_MarkingHit);
                    }
                    if (((CellItem)control).Value != null)
                    {
                        //controlToAdd.PutData(((CellItem)control).Value);
                    }
                        //CBOE-2007 ADD Label and bind the value when the control is "COEDropDownList"
                        ((WebControl)controlToAdd).Attributes.Add("bindingExpression", bindingExpression);
                        if (controlToAdd.GetType().Name.Equals("COEDropDownList"))
                        {
                            foreach (ListItem item in ((COEDropDownList)controlToAdd).Items)
                            {
                                if (((CellItem)control).Value != null)
                                {
                                    string key = ((CellItem)control).Value.ToString();
                                    string currentKey = item.Value.ToString();
                                    if (key == currentKey)
                                    {
                                        Label lbl1 = new Label();                                        
                                        lbl1.Text = item.Text;
                                        control.Controls.Add(lbl1);
                                        break;
                                    }
                                }
                                else
                                {
                                    Label lbl2 = new Label();                                   
                                    CellItem cItem = (CellItem)control;
                                    lbl2.Text = cItem.Text;
                                    control.Controls.Add(lbl2);
									break;
                                }
                            }
                        }
                        else
                        {
                            control.Controls.Add((Control)controlToAdd);
                            ((Control)controlToAdd).DataBinding += new EventHandler(FormElementTemplate_DataBinding);
                            ((Control)controlToAdd).PreRender += new EventHandler(FormElementTemplate_PreRender);
                        }
                    
                }
                else
                {
                    Label lbl = new Label();
                    lbl.ID = "newLabel";
                    CellItem cItem = (CellItem)control;
                    lbl.Text = cItem.Text;
                    control.Controls.Add(lbl);
                }
            }
            _coeLog.LogEnd(methodSignature);
        }

        void FormElementTemplate_PreRender(object sender, EventArgs e)
        {
            if (sender is ICOECultureable)
            {
                ((ICOECultureable)sender).DisplayCulture = _displayCulture;
            }
        }

        void FormElementTemplate_DataBinding(object sender, EventArgs e)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            object container = null;
            if (sender is ICOEGenerableControl)
            {
                ICOEGenerableControl formElement = sender as ICOEGenerableControl;
                object dataItem = DataBinder.GetDataItem(((Control)formElement).NamingContainer);
                object dataItemValue = "Error retrieving " + ((CellItem)((Control)sender).Parent).Cell.Column.BaseColumnName + " value";
                object columnIDValue = null;
                if (formElement is ICOEDisplayModeChanger)
                {
                    ((ICOEDisplayModeChanger)formElement).CurrentIndex = ((CellItem)((Control)sender).Parent).Cell.Row.Index;
                }
                COEDataBinder dataBinder = new COEDataBinder(dataItem);
                string bindingExpression = ((WebControl)formElement).Attributes["bindingExpression"] != null ? ((WebControl)formElement).Attributes["bindingExpression"] : ((CellItem)((Control)sender).Parent).Cell.Column.BaseColumnName;
                dataItemValue = dataBinder.RetrieveProperty("this['" + bindingExpression + "']");

                if (formElement is ICOEHitMarker)
                {
                    ((ICOEHitMarker)formElement).ColumnIDValue = dataBinder.RetrieveProperty(((ICOEHitMarker)formElement).ColumnIDBindingExpression).ToString();
                }
                //Pass full datasource to inner controls
                if (formElement is ICOEFullDatasource && _fullDataSource != null)
                    ((ICOEFullDatasource)formElement).FullDatasource = _fullDataSource;

                if (formElement.GetType() == typeof(COECheckBox))
                {
                    if ((dataItemValue.ToString() == "1"))
                        dataItemValue = (object)true;
                    else
                        dataItemValue = (object)false;
                }
                if (formElement is ICOECultureable)
                {
                    ((ICOECultureable)formElement).DisplayCulture = _displayCulture;
                }

                formElement.PutData(dataItemValue);
            }
            _coeLog.LogEnd(methodSignature);
        }

        void FormElementTemplate_MarkingHit(object sender, MarkHitEventArgs eventArgs)
        {
            if (MarkingHit != null)
            {
                MarkingHit(sender, eventArgs);
            }
        }
    }
    #endregion

    #region ChildGridTemplate
    public class ChildGridTemplate : ITemplate
    {
        String _colName;
        XmlNodeList xDoc;
        DataTable _dt;
        string controlToAddErrorMessage = string.Empty;
        private object _fullDatasource;
        private CultureInfo _displayCulture;
        public event MarkingHitHandler MarkingHit;
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEFormGenerator");

        public ChildGridTemplate(XmlNodeList doc, string colName, DataView dv, object fullDatasource, CultureInfo displayCulture)
        {
            xDoc = doc;
            _colName = colName;
            _dt = dv.ToTable();
            _fullDatasource = fullDatasource;
            _displayCulture = displayCulture;
        }

        void ITemplate.InstantiateIn(System.Web.UI.Control container)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            foreach (XmlNode node in xDoc)
            {
                XmlNamespaceManager xmlManager = new XmlNamespaceManager(node.OwnerDocument.NameTable);
                xmlManager.AddNamespace("COE", node.OwnerDocument.DocumentElement.NamespaceURI);
                //Coverity fix
                XmlNode labelNode = node.SelectSingleNode("COE:label", xmlManager);
                if (labelNode != null)
                {
                    XmlNode childNode = labelNode.ChildNodes[0];
                    if(childNode!=null)
                        childNode.Value = string.Empty;
                }

                ICOEGenerableControl controlToAdd = COEFormGenerator.COEFormGenerator.GetCOEGenerableControl(node.OuterXml.ToString(), out controlToAddErrorMessage);

                //DataBinder binder = new DataBinder();
                //controlToAdd.PutData(DataBinder.GetIndexedPropertyValue(container,_colName));

                if (controlToAdd != null)
                {
                    TableCell tc = (TableCell)container;
                    if (controlToAdd is ICOEFullDatasource && _fullDatasource != null)
                        ((ICOEFullDatasource)controlToAdd).FullDatasource = _fullDatasource;
                    if (controlToAdd is ICOEHitMarker)
                        ((ICOEHitMarker)controlToAdd).MarkingHit += new MarkingHitHandler(ChildGridTemplate_MarkingHit);
                    if (controlToAdd is ICOECultureable)
                        ((ICOECultureable)controlToAdd).DisplayCulture = _displayCulture;
                    container.Controls.Add((Control)controlToAdd);
                }
                else
                {
                    Label lbl = new Label();
                    lbl.ID = "newLabel";
                    container.Controls.Add(lbl);
                }
            }
            _coeLog.LogEnd(methodSignature);
        }

        void ChildGridTemplate_MarkingHit(object sender, MarkHitEventArgs eventArgs)
        {
            if (MarkingHit != null)
                MarkingHit(sender, eventArgs);
        }
    }

    #endregion
}
