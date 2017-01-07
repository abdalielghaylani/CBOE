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
using ChemDrawControl14;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Reflection;


[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEUltraGridView", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEGrid
{
    [DefaultProperty("DisplayLayout")]
    [ToolboxData("<{0}:COEUltraGridView runat=server></{0}:COEUltraGridView>")]
    public class COEUltraGridView : CompositeControl, INamingContainer, ICOECultureable, ICOEGrid
    {
        #region Public Events
        public event CommandEventHandler OrderCommand;
        #endregion

        #region Variables
        internal DataSet _mainDS;
        protected static IDictionary trackChild;
        protected static IDictionary trackColumn;
        XmlDocument doc;
        ScriptManager _scManager;

        XmlNamespaceManager xmlManager;
        private List<int> childColumnsToHide = new List<int>();

        private UltraWebGrid uwebgrid;
        //private UpdatePanel upanel;

        private object _fullDatasource;
        public event MarkingHitHandler MarkingHit;

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEFormGenerator");
        static bool bCreateChildControlsInProgress = false;
        #endregion

        #region Constructor
        public COEUltraGridView()
        {

            if(trackChild == null)
                trackChild = new Dictionary<string, object>();
            if(trackColumn == null)
                trackColumn = new Dictionary<string, object>();
        }
        #endregion

        #region Properties

        public bool ExpandChild
        {
            get
            {
                object o = ViewState["ExpandChild"];
                return (o == null) ? false : (bool) o;
            }
            set
            {
                ViewState["ExpandChild"] = value;
            }
        }
        public string ImageFolder
        {
            get
            {
                object o = ViewState["ImageFolder"];
                return (o == null) ? string.Empty : (string) o;
            }
            set
            {
                ViewState["ImageFolder"] = value;
            }
        }
        public string ExpandImage
        {
            get
            {
                object o = ViewState["ExpandImage"];
                return (o == null) ? string.Empty : (string) o;
            }
            set { ViewState["ExpandImage"] = value; }
        }
        public string CollapseImage
        {
            get
            {
                object o = ViewState["CollapseImage"];
                return (o == null) ? string.Empty : (string) o;
            }
            set
            {
                ViewState["CollapseImage"] = value;
            }
        }
        public string FontSizeInPoint
        {
            get
            {
                object o = ViewState["FontSizeInPoint"];
                return (o == null) ? string.Empty : (string) o;
            }
            set { ViewState["FontSizeInPoint"] = value; }
        }
        [Category("Navigation")]
        public string NavigationImage
        {
            get
            {
                object o = ViewState["NavigationImage"];
                return (o == null) ? string.Empty : (string) o;
            }
            set { ViewState["NavigationImage"] = value; }
        }
        public object DataSource
        {
            get
            {
                object o = _mainDS == null ? ViewState["DataSource"] : _mainDS;
                return (o == null) ? (object) new DataSet() : (object) o;
            }
            set
            {
                ViewState["DataSource"] = _mainDS = value as DataSet;
                //CreateChildControls();
            }
        }
        public bool FixedHeaders
        {
            get
            {
                object o = ViewState["FixedHeaders"];
                return (o == null) ? true : (bool) o;
            }
            set
            {
                ViewState["FixedHeaders"] = value;
            }
        }
        public TableLayout TableLayout
        {
            get
            {
                object o = ViewState["TableLayout"];
                return (o == null) ? TableLayout.Auto : (TableLayout) o;
            }
            set
            {
                ViewState["TableLayout"] = value;
            }
        }
        public bool IsReadyOnly
        {
            get
            {
                object o = ViewState["IsReadyOnly"];
                return (o == null) ? false : (bool) o;
            }
            set
            {
                ViewState["IsReadyOnly"] = value;
            }
        }
        public bool AllowColumnMoving
        {
            get
            {
                object o = ViewState["AllowColumnMoving"];
                return (o == null) ? false : (bool) o;
            }
            set
            {
                ViewState["AllowColumnMoving"] = value;
            }
        }
        public bool AllowFormGeneratorRendering
        {
            get
            {
                object o = ViewState["AllowFormGeneratorRendering"];
                return (o == null) ? false : (bool) o;
            }
            set
            {
                ViewState["AllowFormGeneratorRendering"] = value;
            }
        }
        public bool AllowSorting
        {
            get
            {
                object o = ViewState["AllowSorting"];
                return (o == null) ? true : (bool) o;
            }
            set
            {
                ViewState["AllowSorting"] = value;
            }
        }
        public bool AllowChildSorting
        {
            get
            {
                object o = ViewState["AllowChildSorting"];
                return (o == null) ? true : (bool) o;
            }
            set
            {
                ViewState["AllowChildSorting"] = value;
            }
        }
        public bool ShowSortButton
        {
            get
            {
                object o = ViewState["ShowSortButton"];
                return (o == null) ? false : (bool) o;
            }
            set
            {
                ViewState["ShowSortButton"] = value;
            }
        }
        public bool ShowExpandCollapseImage
        {
            get
            {
                object o = ViewState["ShowExpandCollapseImage"];
                return (o == null) ? false : (bool) o;
            }
            set
            {
                ViewState["ShowExpandCollapseImage"] = value;
            }
        }
        public bool ShowChildDataOnly
        {
            get
            {
                object o = ViewState["ShowChildDataOnly"];
                return (o == null) ? false : (bool) o;
            }
            set
            {
                ViewState["ShowChildDataOnly"] = value;
            }
        }
        public object FullDatasource
        {
            set { _fullDatasource = value; }
            get { return _fullDatasource; }
        }
        public string xmlDataAsString
        {
            get
            {
                object o = ViewState["XMLdoc"];
                return (String) o;
            }
            set
            {
                ViewState["XMLdoc"] = value;
            }

        }

        public string SortFields
        {
            get
            {
                return this.Page.Session["SortFields"] as string;
            }
            set
            {
                this.Page.Session["SortFields"] = value;
            }
        }
        public string SortDirections
        {
            get
            {
                return this.Page.Session["SortDirections"] as string;
            }
            set
            {
                this.Page.Session["SortDirections"] = value;
            }
        }

        //private string Culture
        //{
        //    get
        //    {
        //        return ViewState["Culture"] as string;
        //    }
        //    set
        //    {
        //        ViewState["Culture"] = value;
        //    }
        //}
        #endregion

        #region Override Methods      
        protected override void CreateChildControls()
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            try
            {
                if (this.Page != null && !bCreateChildControlsInProgress )
                {
                    bCreateChildControlsInProgress = true;
                    DataSet ds = this.DataSource as DataSet;
                    if(ds != null && ds.Tables.Count > 0)
                    {                        
                        bool ischildSorting = this.SetSortingOptions();

                        _scManager = ScriptManager.GetCurrent(this.Page);
                        if(_scManager != null)
                            _scManager.Controls.Clear();

                        this.Controls.Clear();

                        GridProperties();
                        CreateControlHierarchy(!this.Page.IsPostBack || ischildSorting);

                    }
                    ChildControlsCreated = true;
                    bCreateChildControlsInProgress = false;
                }
            }
            catch
            {
                bCreateChildControlsInProgress = false;
                throw; 
            }
            finally
            {
                _coeLog.LogEnd(methodSignature);
            }
        }
        #endregion

        #region Events Handlers


        void uwebgrid_ItemCommand(object sender, UltraWebGridCommandEventArgs e)
        {
            ImageButton img;
            if(e.CommandSource.GetType().FullName.ToString() == "System.Web.UI.WebControls.ImageButton")
                img = (ImageButton) e.CommandSource;
            else
                img = null;
            DataView dv = null;
            if(img != null)
            {
                if(e.ParentControl.BindingContainer.GetType() == typeof(CellItem))
                {
                    UltraGridCell cell = ((CellItem) e.ParentControl.BindingContainer).Cell as UltraGridCell;

                    string cellIndex = cell.Column.Index + "," + cell.Row.Index;
                    string path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
                    ICollection<String> icoll = (ICollection<System.String>) trackChild.Keys;

                    DataSet ds = (DataSet) this.DataSource;
                    DataRelation currentRelation = null;
                    Label lbl = e.ParentControl.Parent.Controls[2].Controls[1] as Label;
                    string childtable = string.Empty;
                    if(lbl != null)
                        childtable = lbl.Text.Trim();

                    foreach(DataRelation dr in ds.Relations)
                    {
                        if(dr.ChildTable.TableName.ToString().Trim() == childtable)
                        {
                            currentRelation = dr;
                            string strSortExp = currentRelation.ChildColumns[0].ColumnName + " = '" + cell.Value + "'";
                            DataTable dt = ds.Tables[currentRelation.ChildTable.TableName];
                            dv = new DataView(dt);
                            dv.RowFilter = strSortExp;
                            break;
                        }
                        else if(dr.ParentTable.TableName.ToString().Trim() == childtable)
                        {
                            currentRelation = dr;
                            string strSortExp = currentRelation.ParentColumns[0].ColumnName + " = '" + cell.Value + "'";
                            DataTable dt = ds.Tables[currentRelation.ChildTable.TableName];
                            dv = new DataView(dt);
                            dv.RowFilter = strSortExp;
                            break;
                        }
                    }

                    string[] rowColIndes = cellIndex.Split(',');

                    cell = uwebgrid.Rows[Int32.Parse(rowColIndes[1])].Cells[Int32.Parse(rowColIndes[0])] as UltraGridCell;
                    // Coverity Fix CID - 10463 (from local server)
                    if (cell != null)
                    {
                        TemplatedColumn col = cell.Column as TemplatedColumn;
                        if (col != null)
                        {
                            HeaderItem hItem = col.HeaderItem;
                            if (hItem != null)
                            {

                                CellItem cItems = ((TemplatedColumn)hItem.Column).CellItems[Int32.Parse(rowColIndes[1])] as CellItem;
                                if (cItems != null)
                                {
                                    DataGrid grd = cItems.FindControl("GridView1") as DataGrid;

                                    ImageButton imgChild = cItems.FindControl("ImageControl1") as ImageButton;
                                    // Coverity Fix CID - 10463 (from local server)
                                    if (imgChild != null)
                                    {
                                        if (!trackChild.Contains(cellIndex))
                                        {
                                            trackChild.Add(cellIndex, true);

                                            img.ImageUrl = "/ig_common/Images/minus.gif";
                                            imgChild.ImageUrl = "/ig_common/Images/minus.gif";

                                        }
                                        else
                                        {

                                            trackChild.Remove(cellIndex);
                                            dv = null;
                                            img.ImageUrl = "/ig_common/Images/plus.gif";
                                            imgChild.ImageUrl = "/ig_common/Images/plus.gif";
                                        }
                                    }

                                    //UpdatePanel pnl = cItems.FindControl("UpdatePanel10") as UpdatePanel;
                                    // Coverity Fix CID - 10463 (from local server)
                                    if (grd != null)
                                    {
                                        grd.DataSource = dv;
                                        grd.Visible = true;
                                        grd.DataBind();
                                    }
                                    //if(pnl != null)
                                    //    pnl.Update();
                                }
                            }
                        }
                    }
                }
                //******************For Header ************************//
                else if (e.ParentControl.BindingContainer.GetType() == typeof(HeaderItem))
                {
                    HeaderItem hItem = e.ParentControl.BindingContainer as HeaderItem;
                    // Coverity Fix CID - 10463 (from local server)
                    if (hItem != null)
                    {
                        int colIndex = hItem.Column.Index;
                        string path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;

                        if (!(trackColumn.Contains(colIndex.ToString()) || img.ImageUrl.Contains("minus.gif")))
                        {
                            trackColumn.Add(colIndex.ToString(), true);
                            img.ImageUrl = "/ig_common/Images/minus.gif";
                            DataSet ds = (DataSet)this.DataSource;
                            DataRelation currentRelation = null;
                            CellItem citem = ((TemplatedColumn)hItem.Column).CellItems[0] as CellItem;
                            Label lbl = citem.FindControl("Label1") as Label;
                            string childtable = string.Empty;
                            if (lbl != null)
                                childtable = lbl.Text.Trim();

                            foreach (DataRelation dr in ds.Relations)
                            {
                                if (dr.ParentColumns[0].ColumnName == hItem.Column.BaseColumnName || dr.ChildColumns[0].ColumnName == hItem.Column.BaseColumnName)
                                {
                                    if (dr.ChildTable.TableName.ToString().Trim() == childtable)
                                    {
                                        currentRelation = dr;

                                        foreach (CellItem cItems in ((TemplatedColumn)hItem.Column).CellItems)
                                        {
                                            UltraGridCell cell = cItems.Cell;

                                            string cellIndex = cell.Column.Index + "," + cell.Row.Index;


                                            string strSortExp = currentRelation.ChildColumns[0].ColumnName + " = '" + cell.Value + "'";
                                            DataTable dt = ds.Tables[currentRelation.ChildTable.TableName];
                                            dv = new DataView(dt);
                                            dv.RowFilter = strSortExp;

                                            DataGrid grd = cItems.FindControl("GridView1") as DataGrid;
                                            if (dv.Count != 0)
                                            {
                                                if (!trackChild.Contains(cellIndex))
                                                    trackChild.Add(cellIndex, true);

                                            }
                                            ImageButton imgChild = cItems.FindControl("ImageControl1") as ImageButton;
                                            if (imgChild != null)
                                                imgChild.ImageUrl = "/ig_common/Images/minus.gif";
                                            //UpdatePanel pnl = cItems.FindControl("UpdatePanel10") as UpdatePanel;
                                            // Coverity Fix CID - 10463 (from local server)
                                            if (grd != null && dv.Count != 0)
                                            {
                                                grd.DataSource = dv;
                                                grd.Visible = true;
                                                grd.DataBind();

                                            }
                                            //if(pnl != null)
                                            //    pnl.Update();
                                        }
                                        break;
                                    }
                                    else if (dr.ParentTable.TableName.ToString().Trim() == childtable)
                                    {
                                        currentRelation = dr;

                                        foreach (CellItem cItems in ((TemplatedColumn)hItem.Column).CellItems)
                                        {
                                            UltraGridCell cell = cItems.Cell;

                                            string cellIndex = cell.Column.Index + "," + cell.Row.Index;

                                            string strSortExp = currentRelation.ParentColumns[0].ColumnName + " = '" + cell.Value + "'";
                                            DataTable dt = ds.Tables[currentRelation.ParentTable.TableName];
                                            dv = new DataView(dt);
                                            dv.RowFilter = strSortExp;

                                            DataGrid grd = cItems.FindControl("GridView1") as DataGrid;                                            
                                            if (dv.Count != 0)
                                            {
                                                if (!trackChild.Contains(cellIndex))
                                                    trackChild.Add(cellIndex, true);

                                            }
                                            ImageButton imgChild = cItems.FindControl("ImageControl1") as ImageButton;
                                            if (imgChild != null)
                                                imgChild.ImageUrl = "/ig_common/Images/minus.gif";
                                            //UpdatePanel pnl = cItems.FindControl("UpdatePanel10") as UpdatePanel;
                                            // Coverity Fix CID - 10463 (from local server)
                                            if (grd != null && dv.Count != 0)
                                            {
                                                grd.DataSource = dv;
                                                grd.Visible = true;
                                                grd.DataBind();
                                            }
                                            //if(pnl != null)
                                            //    pnl.Update();
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            trackColumn.Remove(colIndex.ToString());
                            foreach (CellItem cItems in ((TemplatedColumn)hItem.Column).CellItems)
                            {
                                UltraGridCell cell = cItems.Cell;
                                string cellIndex = cell.Column.Index + "," + cell.Row.Index;

                                DataGrid grd = cItems.FindControl("GridView1") as DataGrid;
                                if (grd != null)
                                {
                                    if (!trackChild.Contains(cellIndex))
                                        trackChild.Remove(cellIndex);
                                }
                                ImageButton imgChild = cItems.FindControl("ImageControl1") as ImageButton;
                                if (imgChild != null)
                                    imgChild.ImageUrl = "/ig_common/Images/plus.gif";

                                //UpdatePanel pnl = cItems.FindControl("UpdatePanel10") as UpdatePanel;
                                // Coverity Fix CID - 10463 (from local server)
                                if (grd != null)
                                {
                                    grd.DataSource = null;
                                    grd.Visible = true;
                                    grd.DataBind();
                                }
                                //if(pnl != null)
                                //    pnl.Update();
                            }
                            img.ImageUrl = "/ig_common/Images/plus.gif";
                        }
                    }
                }                
            }
            else
            {
                if(e.ParentControl.BindingContainer.GetType() == typeof(HeaderItem))
                {
                    HeaderItem hItem = e.ParentControl.BindingContainer as HeaderItem;
                    // Coverity Fix CID - 10463 (from local server)
                    if (hItem != null)
                    {
                        int colIndex = hItem.Column.Index;

                        trackChild = null;
                        if (ViewState["SortColumn"] != null)
                        {
                            if (ViewState["SortColumn"].ToString() == colIndex.ToString())
                            {
                                if (ViewState["SortOrder"] != null && ViewState["SortOrder"].ToString() == "Asc")
                                    ViewState["SortOrder"] = "Des";
                                else
                                    ViewState["SortOrder"] = "Asc";
                            }
                            else
                            {
                                ViewState["SortColumn"] = colIndex.ToString();
                                ViewState["SortOrder"] = "Asc";
                            }
                        }
                        else
                        {
                            ViewState.Add("SortColumn", colIndex.ToString());
                            ViewState.Add("SortOrder", "Asc");
                        }
                        DataSet ds1 = (DataSet)this.DataSource;
                        if (ViewState["SortColumn"] != null)
                        {
                            int colNo = Convert.ToInt32(ViewState["SortColumn"]);

                            if (ViewState["SortOrder"].ToString() == "Asc")
                            {
                                ds1.Tables[0].DefaultView.Sort = this.uwebgrid.Columns[colIndex].BaseColumnName + " ASC ";
                            }
                            else
                            {
                                ds1.Tables[0].DefaultView.Sort = this.uwebgrid.Columns[colIndex].BaseColumnName + " DESC ";
                            }

                            _scManager = ScriptManager.GetCurrent(this.Page);
                            if (_scManager != null)
                                _scManager.Controls.Clear();

                            this.Controls.Clear();

                            GridProperties();
                            this.DataSource = ds1;
                            CreateControlHierarchy(true);
                        }
                    }
                }
            }
        }

        void frmColumn_MarkingHit(object sender, MarkHitEventArgs eventArgs)
        {
            if(MarkingHit != null)
                MarkingHit(sender, eventArgs);
        }

        void MarkAllHits(object sender, MarkAllHitsEventArgs eventArgs)
        {
            RaiseBubbleEvent(sender, eventArgs);
        }

        void ColumnClicked(object sender, CommandEventArgs e)
        {

            string[] fieldAndDirection = e.CommandArgument.ToString().Split(',');
            string columnHeader = fieldAndDirection[0];
            XmlNode columnNode = this.doc.SelectSingleNode("//COE:Column[COE:headerText='" + columnHeader + "']", this.xmlManager);
            if (columnNode != null)
            {
                 columnHeader = columnNode.Attributes["sortField"] != null ? columnNode.Attributes["sortField"].Value : columnNode.Attributes["name"].Value;
            }
            this.SortFields = columnHeader;
            this.SortDirections = fieldAndDirection[1];
            CommandEventArgs cmdEventArgs = new CommandEventArgs("OrderCommand", this.SortFields + "," + this.SortDirections);
            if(OrderCommand != null)
            {
                OrderCommand(sender, cmdEventArgs);
            }

            RaiseBubbleEvent(sender, cmdEventArgs);
        }
        #endregion

        #region Private Methods
        public void LoadFromXml(string xmlDataAsString)
        {
            if(xmlDataAsString != null && !string.IsNullOrEmpty(xmlDataAsString))
            {
                this.xmlDataAsString = xmlDataAsString;
                doc = new XmlDocument();
                doc.LoadXml(xmlDataAsString);
            }
            else
            {
                doc = new XmlDocument();
                doc.LoadXml(this.xmlDataAsString);
            }

            xmlManager = new XmlNamespaceManager(doc.NameTable);
            xmlManager.AddNamespace("COE", doc.DocumentElement.NamespaceURI);

            XmlNode nodeConfigSettings = doc.SelectSingleNode("//COE:GridConfigSettings", xmlManager);

            XmlNode nodeSort = doc.SelectSingleNode("//COE:AllowSorting", xmlManager);
            if(nodeSort != null && nodeSort.InnerXml != null)
                this.AllowSorting = Convert.ToBoolean(nodeSort.InnerXml.ToString());


            XmlNode nodeChildSort = doc.SelectSingleNode("//COE:AllowChildSorting", xmlManager);
            if(nodeChildSort != null && nodeChildSort.InnerXml != null)
                this.AllowChildSorting = Convert.ToBoolean(nodeChildSort.InnerXml.ToString());

            XmlNode nodeIsReadOnly = doc.SelectSingleNode("//COE:IsReadOnly", xmlManager);
            if(nodeIsReadOnly != null && nodeIsReadOnly.InnerXml != null)
                this.IsReadyOnly = Convert.ToBoolean(nodeIsReadOnly.InnerXml.ToString());

            XmlNode nodeFixedHeader = doc.SelectSingleNode("//COE:FixedHeader", xmlManager);
            if(nodeFixedHeader != null && nodeFixedHeader.InnerXml != null)
            {
                this.FixedHeaders = Convert.ToBoolean(nodeFixedHeader.InnerXml.ToString());
                if(this.FixedHeaders)
                {
                    this.TableLayout = TableLayout.Fixed;
                }
            }

            XmlNode nodeShowExpandCollapseImage = doc.SelectSingleNode("//COE:ShowExpandCollapseImage", xmlManager);
            if(nodeShowExpandCollapseImage != null && nodeShowExpandCollapseImage.InnerXml != null)
                this.ShowExpandCollapseImage = Convert.ToBoolean(nodeShowExpandCollapseImage.InnerXml.ToString());

            XmlNode ShowChildDataOnly = doc.SelectSingleNode("//COE:ShowChildDataOnly", xmlManager);
            if(ShowChildDataOnly != null && ShowChildDataOnly.InnerXml != null)
                this.ShowChildDataOnly = Convert.ToBoolean(ShowChildDataOnly.InnerXml.ToString());
        }

        private bool SetSortingOptions()
        {
            string strT = this.Page.Request.Form["__EVENTTARGET"];
            string[] strP = new string[5];
            string[] strPR = new string[4];
            bool ischildSorting = false;

            // "ctl00$ContentPlaceHolder$COEGridViewTestm1$UltraWebGrid1$ci_0_4_0$GridView1$ctl01$ctl02";
            if(strT != null && strT != "" && strT.Contains("GridView1"))
            {
                ischildSorting = strT.Contains("GridView1");
                if(ischildSorting)
                    ViewState["ischildSorting"] = "true";
                if(strT.Contains("UltraWebGrid1"))
                {
                    strT = strT.Substring(strT.IndexOf("UltraWebGrid1"));
                    strP = strT.Split('$');
                    if(strP.Length >= 4)
                    {
                        int _childCNo = 0, _parentCno = 0, _parentRNo = 0;
                        _childCNo = Convert.ToInt32(strP[4].Substring(4));
                        strPR = strP[1].Split('_');
                        _parentCno = Convert.ToInt32(strPR[2].ToString());
                        _parentRNo = Convert.ToInt32(strPR[3].ToString());
                        if(ViewState["ChildSortColumn"] != null && ViewState["ParentSortColumn"] != null)
                        {
                            if(Convert.ToInt32(ViewState["ChildSortColumn"].ToString()) == _childCNo && Convert.ToInt32(ViewState["ParentSortColumn"].ToString()) == _parentCno)
                            {
                                if(ViewState["ChildSortOrder"] != null && ViewState["ChildSortOrder"].ToString() == "Asc")
                                {
                                    ViewState["ChildSortOrder"] = "Desc";
                                    ViewState["ParentSortOrder"] = "Desc";
                                }
                                else
                                {
                                    ViewState["ChildSortOrder"] = "Asc";
                                    ViewState["ParentSortOrder"] = "Asc";
                                }
                            }
                            else
                            {
                                ViewState["ChildSortColumn"] = _childCNo;
                                ViewState["ChildSortOrder"] = "Asc";
                                ViewState["ParentSortOrder"] = "Asc";
                            }
                            ViewState.Add("ParentSortColumn", _parentCno);
                            ViewState.Add("ParentSortRow", _parentRNo);
                        }
                        else
                        {
                            ViewState.Add("ParentSortColumn", _parentCno);
                            ViewState.Add("ParentSortOrder", "Asc");
                            ViewState.Add("ParentSortRow", _parentRNo);
                            ViewState.Add("ChildSortColumn", _childCNo);
                            ViewState.Add("ChildSortOrder", "Asc");
                        }
                    }
                    else
                    {
                        ViewState["ChildSortColumn"] = null;
                        ViewState["ChildSortOrder"] = "Asc";
                        ViewState["ParentSortColumn"] = null;
                        ViewState["ParentSortOrder"] = null;
                        ViewState["ParentSortRow"] = null;
                        ViewState["ischildSorting"] = "false";
                    }
                }
            }
            else
            {
                ViewState["ChildSortColumn"] = null;
                ViewState["ChildSortOrder"] = "Asc";
                ViewState["ParentSortColumn"] = null;
                ViewState["ParentSortOrder"] = null;
                ViewState["ParentSortRow"] = null;
                ViewState["ischildSorting"] = "false";
            }

            return ischildSorting;
        }

        private void GridProperties()
        {
            uwebgrid = null;
            uwebgrid = new Infragistics.WebUI.UltraWebGrid.UltraWebGrid();
            uwebgrid.ID = "UltraWebGrid1";
            uwebgrid.DisplayLayout.EnableInternalRowsManagement = false;
            uwebgrid.EnableViewState = false;
            //upanel = new UpdatePanel();
            //upanel.ID = "MainUpdatePanel";
            //upanel.UpdateMode = UpdatePanelUpdateMode.Conditional;

            _scManager = ScriptManager.GetCurrent(this.Page);
            if(_scManager != null)
            {
                _scManager.Controls.Clear();
                _scManager.RegisterAsyncPostBackControl(uwebgrid);
            }
            uwebgrid.DisplayLayout.ViewType = ViewType.Flat;

            uwebgrid.DisplayLayout.CellClickActionDefault = Infragistics.WebUI.UltraWebGrid.CellClickAction.CellSelect;
            uwebgrid.DisplayLayout.HeaderTitleModeDefault = Infragistics.WebUI.UltraWebGrid.CellTitleMode.Always;

            if(this.AllowSorting)
            {
                uwebgrid.DisplayLayout.AllowSortingDefault = Infragistics.WebUI.UltraWebGrid.AllowSorting.Yes;
                uwebgrid.DisplayLayout.HeaderClickActionDefault = HeaderClickAction.SortSingle;
            }
            else
            {
                uwebgrid.DisplayLayout.AllowSortingDefault = Infragistics.WebUI.UltraWebGrid.AllowSorting.No;
                uwebgrid.DisplayLayout.HeaderClickActionDefault = HeaderClickAction.NotSet;
            }

            uwebgrid.DisplayLayout.AutoGenerateColumns = false;
            uwebgrid.DisplayLayout.RowSizingDefault = Infragistics.WebUI.UltraWebGrid.AllowSizing.Free;
            uwebgrid.DisplayLayout.CellClickActionDefault = Infragistics.WebUI.UltraWebGrid.CellClickAction.CellSelect;
            uwebgrid.DisplayLayout.AllowUpdateDefault = Infragistics.WebUI.UltraWebGrid.AllowUpdate.No;
            uwebgrid.DisplayLayout.AllowRowNumberingDefault = RowNumbering.ByBandLevel;
            uwebgrid.DisplayLayout.AllowColSizingDefault = AllowSizing.Free;
            uwebgrid.DisplayLayout.RowSelectorsDefault = RowSelectors.No;
            uwebgrid.DisplayLayout.SelectTypeRowDefault = SelectType.Extended;
            uwebgrid.DisplayLayout.StationaryMarginsOutlookGroupBy = true;
            uwebgrid.ItemCommand += new Infragistics.WebUI.UltraWebGrid.ItemCommandEventHandler(uwebgrid_ItemCommand);

            if(this.IsReadyOnly)
                uwebgrid.DisplayLayout.ReadOnly = ReadOnly.LevelZero;
                
        }

        private void CreateControlHierarchy(bool drawPanel)
        {
            Color hdrColor = Color.Gray;
            Color hdrBackColor = Color.DimGray;
            FontUnit hdrFontSize = FontUnit.Small;
            string hdrFontFamily = "";
            bool hdrFontWeight = false;

            int intCCNo = -1;
            int intPRNo = -1;
            int intPRCNo = -1;
            string strPCValue = "";
            string strCSOrder = "";
            string childTableName = string.Empty;

            if(ViewState["ChildSortColumn"] != null)
                intCCNo = Convert.ToInt32(ViewState["ChildSortColumn"].ToString());

            if(ViewState["ParentSortRow"] != null)
                intPRNo = Convert.ToInt32(ViewState["ParentSortRow"].ToString());

            if(ViewState["ChildSortOrder"] != null)
            {
                strCSOrder = ViewState["ChildSortOrder"].ToString();
            }
            if(ViewState["ParentSortColumn"] != null)
            {
                intPRCNo = Convert.ToInt32(ViewState["ParentSortColumn"].ToString());
            }

            if(uwebgrid.Bands.Count == 0)
            {
                UltraGridBand uGB = new UltraGridBand(true);
                uwebgrid.DisplayLayout.Bands.Add(uGB);
            }
            string ChildTableName = string.Empty;

            if(!this.DesignMode)
            {
                try
                {

                    DataSet ds = (DataSet) this.DataSource;
                    if(intPRCNo > 0 && intPRCNo > 0)
                        strPCValue = ds.Tables[0].Columns[intPRCNo].ToString();
                    DataRelation dr = null;
                    int COEWebGridColIndex = 0;
                    ArrayList colIndex = new ArrayList();
                    int iRelationCounts = 0;
                    string primaryKeyID = string.Empty;
                    if(ds.Relations.Count != null)
                    {
                        iRelationCounts = ds.Relations.Count;
                    }
                    if(iRelationCounts > 0)
                    {
                        primaryKeyID = ds.Relations[0].ParentColumns[0].ColumnName;
                    }
                    else if(uwebgrid.DataKeyField != string.Empty)
                    {
                        primaryKeyID = uwebgrid.DataKeyField;
                    }

                    uwebgrid.Bands[0].Columns.Clear();
                    try
                    {
                        if(doc == null)
                        {
                            doc = new XmlDocument();
                            doc.LoadXml(this.xmlDataAsString);
                        }
                        xmlManager = new XmlNamespaceManager(doc.NameTable);
                        xmlManager.AddNamespace("COE", doc.DocumentElement.NamespaceURI);

                        List<DataRelation> IRelations = null;

                        if(ds.Relations.Count > 0)
                        {
                            IRelations = new List<DataRelation>();
                            foreach(DataRelation relation in ds.Relations)
                            {
                                IRelations.Add(relation);
                            }
                        }

                        XmlNode tableNode = doc.SelectSingleNode("//COE:table", xmlManager);

                        if(tableNode != null)
                        {
                            if(this.ShowChildDataOnly)
                            {
                                ChildTableName = tableNode.Attributes["name"].Value;
                            }

                            XmlNode xColumnsCollection = tableNode.SelectSingleNode("COE:Columns", xmlManager);
                            XmlNodeList xColumnsList = xColumnsCollection.SelectNodes("COE:Column", xmlManager);

                            XmlNode xCSS = tableNode.SelectSingleNode("//COE:CSSClass", xmlManager);
                            if(xCSS != null && xCSS.InnerXml != null)
                                this.uwebgrid.ControlStyle.CssClass = xCSS.InnerXml;

                            this.SetHeaderStyle(tableNode, ref hdrColor, ref hdrBackColor, ref hdrFontSize, ref hdrFontFamily, ref hdrFontWeight);
                            this.SetColumnStyle(tableNode, ref hdrColor, ref hdrBackColor, ref hdrFontSize, ref hdrFontFamily, ref hdrFontWeight);

                            foreach(XmlNode columnNode in xColumnsList)
                            {
                                dr = null;
                                string colName = string.Empty;
                                if(columnNode.Attributes["name"] != null)
                                    colName = columnNode.Attributes["name"].Value;
                                else if(columnNode.SelectSingleNode("COE:formElement", xmlManager).Attributes["name"] != null)
                                    colName = columnNode.SelectSingleNode("COE:formElement", xmlManager).Attributes["name"].Value;

                                //jhs 01/26/2010 - deprecate drawChildTable
                                //bool drawChildTable = true;

                                //if(columnNode.Attributes["drawChildTable"] != null)
                                //    drawChildTable = Convert.ToBoolean(columnNode.Attributes["drawChildTable"].Value);

                                XmlNode width = columnNode.SelectSingleNode("./COE:width", xmlManager);
                                XmlNode formElementNode = columnNode.SelectSingleNode("COE:formElement", xmlManager);

                                if(columnNode.SelectNodes("COE:formElement", xmlManager).Count != 0)
                                {
                                    if(columnNode.SelectSingleNode("COE:formElement", xmlManager).ChildNodes.Count > 0)
                                    {
                                        //jhs new attribute will just tell you which child Table to match
                                        if (columnNode.Attributes["childTableName"] != null)
                                        {
                                            childTableName = columnNode.Attributes["childTableName"].Value;

                                            foreach (DataRelation relation in ds.Relations)
                                            {
                                                //then if you hit the right child Table find make sure it is the correct parent column
                                                if ((relation.ChildTable.TableName == childTableName) && (relation.ParentColumns[0].ColumnName.ToUpper() == colName.ToUpper()))
                                                {
                                                    dr = relation;
                                                    break;
                                                }
                                            }
                                        }

                                        //jhs 01/26/2010 - deprecate drawChildTable
                                        //if(dr != null && drawChildTable)
                                        if (dr != null)
                                        {
                                            TemplatedColumn tempCol = new TemplatedColumn(true);
                                            tempCol.BaseColumnName = colName;
                                            tempCol.Header.Caption = colName;
                                            tempCol.Key = colName;
                                            tempCol.CellStyle.HorizontalAlign = HorizontalAlign.Left;
                                            tempCol.Header.Style.HorizontalAlign = HorizontalAlign.Left;
                                            tempCol.Header.ClickAction = HeaderClickAction.SortSingle;
                                            tempCol.AllowUpdate = AllowUpdate.Yes;
                                            if(columnNode.Attributes["hidden"] != null)//Check visibility of the column (just if the attribute exists)
                                                this.SetColumnVisibility(ref tempCol, columnNode.Attributes["hidden"]);
                                            string _Ctable = string.Empty;
                                            string strSortExp = dr.ChildColumns[0].ColumnName;

                                            _Ctable = dr.ChildTable.TableName;
                                            strSortExp = dr.ChildColumns[0].ColumnName;

                                            XmlNodeList tableNodelist = doc.SelectNodes("//COE:table", xmlManager);
                                            XmlNode childTableNode = null;


                                            if(width != null && width.InnerText.Length > 0)
                                            {
                                                tempCol.Header.Style.Width = new Unit(width.InnerText);
                                                tempCol.CellStyle.Width = new Unit(width.InnerText);
                                                tempCol.Width = new Unit(width.InnerText);
                                            }


                                            foreach(XmlNode relationalNode in tableNodelist)
                                            {
                                                if(relationalNode.Attributes["name"].Value.ToString().ToUpper() == dr.ChildTable.TableName.ToUpper().ToString())
                                                {
                                                    childTableNode = relationalNode;
                                                    break;
                                                }
                                            }

                                            XmlNode childTableColumns = childTableNode.SelectSingleNode("./COE:Columns", xmlManager);

                                            bool showParentColumn = false;
                                            List<XmlNode> toRemoveColumns = new List<XmlNode>();

                                            foreach(XmlNode childColumn in childTableColumns)
                                            {
                                                if(childColumn.Attributes["hidden"] == null)
                                                    showParentColumn = true;
                                                else if(childColumn.Attributes["hidden"].Value.ToString().ToLower() == "false")
                                                    showParentColumn = true;
                                                else
                                                {
                                                    toRemoveColumns.Add(childColumn);
                                                }
                                            }

                                            if(!showParentColumn)
                                            {
                                                XmlDocument xmlDoc = new XmlDocument();
                                                XmlAttribute hidden = xmlDoc.CreateAttribute("hidden");
                                                hidden.Value = "false";
                                                this.SetColumnVisibility(ref tempCol, hidden);
                                            }

                                            COETemplatedColumnProperties tempColParam = new COETemplatedColumnProperties();
                                            tempColParam.DataSource = ds;
                                            tempColParam.strFilterExp = strSortExp;
                                            tempColParam.childExpanded = ExpandChild;
                                            tempColParam.childTableName = _Ctable;
                                            tempColParam.imageFolder = ImageFolder;
                                            tempColParam.expandImage = this.ExpandImage;
                                            tempColParam.collapseImage = this.CollapseImage;
                                            tempColParam.fontSizeInPoint = this.FontSizeInPoint;
                                            tempColParam.xDoc = columnNode.SelectNodes("COE:formElement", xmlManager);
                                            tempColParam.drawPanel = drawPanel;
                                            tempColParam.scManager = _scManager;
                                            tempColParam.intChildSortColumn = intCCNo;
                                            tempColParam.strValue = strPCValue;
                                            tempColParam.strCSortOrder = strCSOrder;
                                            tempColParam.intRIndex = intPRNo;
                                            tempColParam.intCIndex = intPRCNo;
                                            tempColParam.AllowChildSort = AllowChildSorting;
                                            tempColParam.ShowExpandCollapseImage = this.ShowExpandCollapseImage;
                                            tempColParam.ChildTableNode = childTableNode;
                                            tempColParam.FullDatasource = this.FullDatasource;


                                            string strHeaderText = string.Empty;
                                            string childCss = string.Empty;
                                            string sortedFields = this.SortFields == null ? string.Empty : this.SortFields;

                                            if(childTableNode != null)
                                            {
                                                foreach(XmlNode colToRemove in toRemoveColumns)
                                                    childTableNode.SelectSingleNode("./COE:Columns", xmlManager).RemoveChild(colToRemove);

                                                XmlNode Columns = childTableNode.SelectSingleNode("COE:Columns", xmlManager);
                                                XmlNodeList ColumnList = Columns.SelectNodes("COE:Column", xmlManager);
                                                XmlNode cCss = childTableNode.SelectSingleNode("COE:CSSClass", xmlManager);

                                                if(cCss != null && cCss.InnerText != string.Empty)
                                                    childCss = cCss.InnerText;

                                                foreach(XmlNode cNode in ColumnList)
                                                {
                                                    XmlNode header = cNode.SelectSingleNode("./COE:headerText", xmlManager);

                                                    string headerText = string.Empty;

                                                    if (header != null && header.InnerText.Length == 0)
                                                    {
                                                        XmlNode widthNode = cNode.SelectSingleNode("./COE:width", xmlManager);
                                                        if (widthNode != null && widthNode.InnerText.Trim().Length > 0)
                                                            strHeaderText += " " + ',' + widthNode.InnerText + ',';
                                                    }
                                                    if(header != null && header.InnerText.Length > 0)
                                                    {
                                                        headerText = header.InnerText;
                                                        sortedFields = sortedFields.Replace(cNode.Attributes["name"].Value, headerText);
                                                        if(headerText.Length > 0)
                                                        {
                                                            if((cNode.Attributes["hidden"] == null || cNode.Attributes["hidden"].Value != "true") && cNode.SelectSingleNode("./COE:width", xmlManager) != null)
                                                                strHeaderText += headerText + ',' + cNode.SelectSingleNode("./COE:width", xmlManager).InnerText + ',';
                                                            else
                                                                strHeaderText += headerText + ", ,";
                                                        }
                                                    }
                                                    else if(cNode.Attributes["name"] != null)
                                                    {
                                                        if(cNode.Attributes["hidden"] != null && cNode.Attributes["hidden"].Value != "true" && cNode.SelectSingleNode("./COE:width", xmlManager) != null)
                                                            strHeaderText += cNode.Attributes["name"].Value + ',' + cNode.SelectSingleNode("./COE:width", xmlManager).InnerText + ',';
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach(DataColumn dc in dr.ChildTable.Columns)
                                                    strHeaderText += dc.ColumnName + ',';
                                            }

                                            NetGridTemplate temp = new NetGridTemplate(tempColParam, this.DisplayCulture);
                                            tempCol.Tag = dr.ChildTable.TableName;
                                            tempCol.CellTemplate = temp;

                                            // Coverity Fix CID - 18763
                                            HeaderExpandTemplate headerTemp = new HeaderExpandTemplate(this.ShowExpandCollapseImage,
                                                !string.IsNullOrEmpty(childCss) ? childCss : xCSS != null ? xCSS.InnerText : string.Empty,
                                                this.AllowSorting, uwebgrid.DisplayLayout.HeaderStyleDefault.ForeColor,
                                                uwebgrid.DisplayLayout.HeaderStyleDefault.BackColor, uwebgrid.DisplayLayout.HeaderStyleDefault.Font.Size,
                                                uwebgrid.DisplayLayout.HeaderStyleDefault.Font.Name, uwebgrid.DisplayLayout.HeaderStyleDefault.Font.Bold,
                                                strHeaderText, childTableNode.SelectSingleNode("COE:headerStyle", xmlManager),
                                                this.ColumnClicked,
                                                sortedFields,
                                                this.SortDirections,
                                                this.MarkAllHits,
                                                childTableNode);

                                            tempCol.HeaderTemplate = headerTemp;

                                            if(width != null && width.InnerText.Length > 0)
                                            {
                                                tempCol.Width = new Unit(width.InnerText);
                                                tempCol.Header.Style.Width = new Unit(width.InnerText);
                                                tempCol.CellStyle.Width = new Unit(width.InnerText);
                                            }
                                            temp.MarkingHit += new MarkingHitHandler(frmColumn_MarkingHit);

                                            XmlNode headerNode = columnNode.SelectSingleNode("./COE:headerText", xmlManager);

                                            string headertext = string.Empty;
                                            if(headerNode != null && headerNode.InnerText.Length > 0)
                                            {
                                                headertext = headerNode.InnerText;
                                            }
                                            if(headertext.Length > 0)
                                                tempCol.Header.Caption = "&nbsp" + headertext + "&nbsp";

                                            uwebgrid.Bands[0].Columns.Add(tempCol);
                                            dr = null;
                                        }
                                        else
                                        {
                                            TemplatedColumn formElementColumn = new TemplatedColumn(true);
                                            formElementColumn.Key = colName;
                                            FormElementTemplate frmColumn = new FormElementTemplate(columnNode.SelectNodes("COE:formElement", xmlManager), this.FullDatasource, this.Page.IsPostBack, this.DisplayCulture);

                                            bool drawcheckBox = false;

                                            if(formElementNode.SelectSingleNode("COE:displayInfo", xmlManager) != null && formElementNode.SelectSingleNode("COE:displayInfo", xmlManager).SelectSingleNode("COE:type", xmlManager) != null)
                                            {
                                                string type = formElementNode.SelectSingleNode("COE:displayInfo", xmlManager).SelectSingleNode("COE:type", xmlManager).InnerText;
                                                if(type.Contains("COEMarkHit"))
                                                {
                                                    drawcheckBox = true;
                                                }
                                            }
                                            XmlNode headerNode = columnNode.SelectSingleNode("./COE:headerText", xmlManager);

                                            string headertext = string.Empty;
                                            if(headerNode != null && headerNode.InnerText.Length > 0)
                                            {
                                                headertext = headerNode.InnerText;
                                            }
                                            else
                                                headertext = colName;

                                            bool isSortingColumn = this.AllowSorting && columnNode.Attributes["allowSorting"] != null && (columnNode.Attributes["allowSorting"].Value.ToLower() == "yes" || columnNode.Attributes["allowSorting"].Value.ToLower() == "true");

                                            string sortedFields = this.SortFields == null ? string.Empty : this.SortFields;
                                            if(sortedFields.Contains(columnNode.Attributes["name"].Value) && columnNode.Attributes["name"].Value != headertext)
                                                sortedFields = this.SortFields.Replace(columnNode.Attributes["name"].Value, headertext);

                                            //HeaderExpandTemplate frmHeader = new HeaderExpandTemplate(drawcheckBox, isSortingColumn, uwebgrid.DisplayLayout.HeaderStyleDefault.ForeColor, uwebgrid.DisplayLayout.HeaderStyleDefault.BackColor, uwebgrid.DisplayLayout.HeaderStyleDefault.Font.Size, uwebgrid.DisplayLayout.HeaderStyleDefault.Font.Name, uwebgrid.DisplayLayout.HeaderStyleDefault.Font.Bold, this.ColumnClicked, sortedDirection);
                                            HeaderExpandTemplate frmHeader = new HeaderExpandTemplate(drawcheckBox,
                                                xCSS != null ? xCSS.InnerText : null,
                                                isSortingColumn, uwebgrid.DisplayLayout.HeaderStyleDefault.ForeColor,
                                                uwebgrid.DisplayLayout.HeaderStyleDefault.BackColor, uwebgrid.DisplayLayout.HeaderStyleDefault.Font.Size,
                                                uwebgrid.DisplayLayout.HeaderStyleDefault.Font.Name, uwebgrid.DisplayLayout.HeaderStyleDefault.Font.Bold,
                                                headertext, tableNode.SelectSingleNode("COE:headerStyle", xmlManager),
                                                this.ColumnClicked,
                                                sortedFields,
                                                this.SortDirections,
                                                this.MarkAllHits);

                                            formElementColumn.Header.ClickAction = HeaderClickAction.SortSingle;

                                            formElementColumn.CellTemplate = frmColumn;

                                            string bindingExpression = (formElementNode.Attributes["name"] != null && !string.IsNullOrEmpty(formElementNode.Attributes["name"].Value)) ? formElementNode.Attributes["name"].Value : null;

                                            if(bindingExpression != null && !string.IsNullOrEmpty(bindingExpression))
                                                formElementColumn.BaseColumnName = bindingExpression;
                                            else
                                                formElementColumn.BaseColumnName = colName;
                                            formElementColumn.Header.Caption = colName;
                                            formElementColumn.HeaderTemplate = frmHeader;
                                            formElementColumn.AllowUpdate = AllowUpdate.Yes;

                                            if(width != null && width.InnerText.Length > 0)
                                            {
                                                formElementColumn.Header.Style.Width = new Unit(width.InnerText);
                                                formElementColumn.CellStyle.Width = new Unit(width.InnerText);
                                                formElementColumn.Width = new Unit(width.InnerText);
                                            }
                                            
                                            frmColumn.MarkingHit += new MarkingHitHandler(frmColumn_MarkingHit);

                                            XmlNode header = columnNode.SelectSingleNode("./COE:headerText", xmlManager);

                                            string headerText = string.Empty;
                                            if(header != null && header.InnerText.Length > 0)
                                            {
                                                headerText = header.InnerText;
                                            }
                                            if(headerText.Length > 0)
                                                formElementColumn.Header.Caption = headerText;

                                            if(columnNode.Attributes["hidden"] != null && columnNode.Attributes["hidden"].Value.ToString().ToLower() == "true") //Check visibility of the column (just if the attribute exists)
                                                this.SetColumnVisibility(ref formElementColumn, columnNode.Attributes["hidden"]);

                                            uwebgrid.Bands[0].Columns.Add(formElementColumn);
                                        }
                                    }
                                    else
                                    {
                                        string colText = columnNode.SelectSingleNode("COE:formElement", xmlManager).Attributes["name"].Value.ToString();
                                        UltraGridColumn col = new UltraGridColumn(colText, colText, ColumnType.NotSet, null);
                                        col.BaseColumnName = columnNode.SelectSingleNode("COE:formElement", xmlManager).Attributes["name"].Value;

                                        if(width != null && width.InnerText.Length > 0)
                                        {
                                            col.Header.Style.Width = new Unit(width.InnerText);
                                            col.CellStyle.Width = new Unit(width.InnerText);
                                            col.Width = new Unit(width.InnerText);
                                        }
                                        if(columnNode.Attributes["hidden"] != null)//Check visibility of the column (just if the attribute exists)
                                            this.SetColumnVisibility(ref col, columnNode.Attributes["hidden"]);
                                        if(this.AllowChildSorting)
                                            col.Header.ClickAction = HeaderClickAction.SortSingle;

                                        uwebgrid.Bands[0].Columns.Add(col);
                                    }
                                }
                                else
                                {
                                    UltraGridColumn col = new UltraGridColumn(columnNode.Attributes["name"].Value.ToString(), columnNode.Attributes["name"].Value.ToString(), ColumnType.NotSet, null);
                                    col.BaseColumnName = columnNode.Attributes["name"].Value;

                                    if(width != null && width.InnerText.Length > 0)
                                    {
                                        col.Header.Style.Width = new Unit(width.InnerText);
                                        col.CellStyle.Width = new Unit(width.InnerText);
                                        col.Width = new Unit(width.InnerText);
                                    }
                                    if(columnNode.Attributes["hidden"] != null)//Check visibility of the column (just if the attribute exists)
                                        this.SetColumnVisibility(ref col, columnNode.Attributes["hidden"]);
                                    uwebgrid.Bands[0].Columns.Add(col);
                                }
                            }
                        }
                    }
                    catch { throw; }
                }
                catch { throw; }
            }
            TrackViewState();

            DataSet ds1 = this.DataSource as DataSet;
            bool isEmptyDatasource = true;
            if (ds1 != null && ds1.Tables.Count > 0)  //Coverity Fix CID 18763 ASV
            {
                if (this.ShowChildDataOnly)
                {
                    if (ds1.Tables[ChildTableName] != null && ds1.Tables[ChildTableName].Rows.Count > 0)
                    {
                        uwebgrid.DataSource = ds1.Tables[ChildTableName];
                        isEmptyDatasource = false;
                    }
                }
                else
                {
                    uwebgrid.DataSource = ds1;
                    isEmptyDatasource = !(ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0);
                }
            }
            
            //Try to avoid the use of Style, Width and Height; instead of it, define all in a CSSClass.
            XmlNode style = doc.SelectSingleNode("//COE:Style", xmlManager);
            if(style != null && style.InnerText.Length > 0)
            {
                string[] styles = style.InnerText.Split(new char[1] { ';' });
                for(int i = 0; i < styles.Length; i++)
                {
                    if(styles[i].Length > 0)
                    {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        string styleId = styleDef[0].Trim();
                        string styleValue = styleDef[1].Trim();

                        this.uwebgrid.Style.Add(styleId, styleValue);
                    }
                }
            }

            this.Controls.Add(uwebgrid);
            
            if(!isEmptyDatasource)
                uwebgrid.DataBind();
        }

        private void SetColumnStyle(XmlNode tableNode, ref Color hdrColor, ref Color hdrBackColor, ref FontUnit hdrFontSize, ref string hdrFontFamily, ref bool hdrFontWeight)
        {
            XmlNode columnStyle = tableNode.SelectSingleNode("//COE:columnStyle", xmlManager);

            if(columnStyle != null && columnStyle.InnerText.Length > 0)
            {
                string[] styles = columnStyle.InnerText.Split(new char[1] { ';' });
                styles[0] = styles[0].Trim();
                styles[1] = styles[1].Trim();
                for(int i = 0; i < styles.Length; i++)
                {
                    if(styles[i].Length > 0)
                    {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        switch(styleDef[0].Trim())
                        {
                            case "background-color":
                                System.Drawing.Color color = new System.Drawing.Color();
                                if(styles[1].ToLower().Contains("rgb("))
                                {
                                    styles[1] = styles[1].Remove(0, styles[1].IndexOf("(") + 1);
                                    styles[1] = styles[1].Remove(styles[1].Length - 1);
                                    string[] rgb = styles[1].Split(',');

                                    uwebgrid.BackColor = System.Drawing.Color.FromArgb(int.Parse(rgb[0].Trim()), int.Parse(rgb[1].Trim()), int.Parse(rgb[2].Trim()));
                                    hdrBackColor = System.Drawing.Color.FromArgb(int.Parse(rgb[0].Trim()), int.Parse(rgb[1].Trim()), int.Parse(rgb[2].Trim()));
                                }
                                else
                                {
                                    color = System.Drawing.Color.FromName(styleDef[1]);
                                    uwebgrid.BackColor = color;
                                    hdrBackColor = color;
                                }
                                break;
                            case "color":
                                color = System.Drawing.Color.FromName(styleDef[1]);
                                uwebgrid.ForeColor = color;
                                hdrColor = color;
                                break;
                            case "border-color":
                                {
                                    color = new System.Drawing.Color();
                                    if(styles[1].ToLower().Contains("rgb("))
                                    {
                                        styles[1] = styles[1].Remove(0, styles[1].IndexOf("(") + 1);
                                        styles[1] = styles[1].Remove(styles[1].Length - 1);
                                        string[] rgb = styles[1].Split(',');

                                        uwebgrid.BorderColor = color;
                                        uwebgrid.DisplayLayout.RowStyleDefault.BorderColor = color;
                                    }
                                    else
                                    {
                                        color = System.Drawing.Color.FromName(styleDef[1]);
                                        uwebgrid.BorderColor = color;
                                        uwebgrid.DisplayLayout.RowStyleDefault.BorderColor = color;
                                    }
                                    break;
                                }
                            case "border-width":
                                {
                                    uwebgrid.BorderWidth = new Unit(styleDef[1]);
                                    uwebgrid.DisplayLayout.RowStyleDefault.BorderWidth = new Unit(styleDef[1]);
                                    break;
                                }
                            case "border-style":
                                {
                                    switch(styleDef[1].Trim().ToLower())
                                    {
                                        case "dashed":
                                            uwebgrid.BorderStyle = BorderStyle.Dashed;
                                            uwebgrid.DisplayLayout.RowStyleDefault.BorderStyle = BorderStyle.Dashed;
                                            break;
                                        case "dotted":
                                            uwebgrid.BorderStyle = BorderStyle.Dotted;
                                            uwebgrid.DisplayLayout.RowStyleDefault.BorderStyle = BorderStyle.Dotted;
                                            break;
                                        case "double":
                                            uwebgrid.BorderStyle = BorderStyle.Double;
                                            uwebgrid.DisplayLayout.RowStyleDefault.BorderStyle = BorderStyle.Double;
                                            break;
                                        case "groove":
                                            uwebgrid.BorderStyle = BorderStyle.Groove;
                                            uwebgrid.DisplayLayout.RowStyleDefault.BorderStyle = BorderStyle.Groove;
                                            break;
                                        case "inset":
                                            uwebgrid.BorderStyle = BorderStyle.Inset;
                                            uwebgrid.DisplayLayout.RowStyleDefault.BorderStyle = BorderStyle.Inset;
                                            break;
                                        case "solid":
                                            uwebgrid.BorderStyle = BorderStyle.Solid;
                                            uwebgrid.DisplayLayout.RowStyleDefault.BorderStyle = BorderStyle.Solid;
                                            break;
                                        case "ridge":
                                            uwebgrid.BorderStyle = BorderStyle.Ridge;
                                            uwebgrid.DisplayLayout.RowStyleDefault.BorderStyle = BorderStyle.Ridge;
                                            break;
                                        case "outset":
                                            uwebgrid.BorderStyle = BorderStyle.Outset;
                                            uwebgrid.DisplayLayout.RowStyleDefault.BorderStyle = BorderStyle.Outset;
                                            break;
                                        case "notset":
                                            uwebgrid.BorderStyle = BorderStyle.NotSet;
                                            uwebgrid.DisplayLayout.RowStyleDefault.BorderStyle = BorderStyle.NotSet;
                                            break;
                                        case "none":
                                            uwebgrid.BorderStyle = BorderStyle.None;
                                            uwebgrid.DisplayLayout.RowStyleDefault.BorderStyle = BorderStyle.None;
                                            break;
                                    }
                                    break;
                                }

                            case "border-lines":
                                switch(styleDef[1].Trim().ToLower())
                                {
                                    case "both":
                                        uwebgrid.DisplayLayout.GridLinesDefault = UltraGridLines.Both;
                                        break;
                                    case "vertical":
                                        uwebgrid.DisplayLayout.GridLinesDefault = UltraGridLines.Vertical;
                                        break;
                                    case "horizontal":
                                        uwebgrid.DisplayLayout.GridLinesDefault = UltraGridLines.Horizontal;
                                        break;
                                    case "none":
                                        uwebgrid.DisplayLayout.GridLinesDefault = UltraGridLines.None;
                                        break;
                                }
                                break;
                            case "font-weight":
                                uwebgrid.Font.Bold = styleDef[1].ToLower().Contains("bold");
                                hdrFontWeight = styleDef[1].ToLower().Contains("bold");
                                break;

                            case "font-family":
                                uwebgrid.Font.Name = styleDef[1];
                                hdrFontFamily = styleDef[1];
                                break;

                            case "font-size":
                                uwebgrid.Font.Size = new FontUnit(new Unit(styleDef[1]));
                                hdrFontSize = new FontUnit(new Unit(styleDef[1]));
                                break;

                            case "text-align":
                                switch(styleDef[1].Trim().ToLower())
                                {
                                    case "left":
                                        uwebgrid.HorizontalAlign = HorizontalAlign.Left;
                                        uwebgrid.DisplayLayout.RowStyleDefault.HorizontalAlign = HorizontalAlign.Left;
                                        break;
                                    case "right":
                                        uwebgrid.HorizontalAlign = HorizontalAlign.Right;
                                        uwebgrid.DisplayLayout.RowStyleDefault.HorizontalAlign = HorizontalAlign.Right;
                                        break;
                                    case "center":
                                        uwebgrid.HorizontalAlign = HorizontalAlign.Center;
                                        uwebgrid.DisplayLayout.RowStyleDefault.HorizontalAlign = HorizontalAlign.Center;
                                        break;
                                }
                                break;
                            case "cell-spacing":
                                uwebgrid.DisplayLayout.CellSpacingDefault = int.Parse(styleDef[1].Trim());
                                break;
                        }
                    }
                }
            }
        }

        private void SetHeaderStyle(XmlNode tableNode, ref Color hdrColor, ref Color hdrBackColor, ref FontUnit hdrFontSize, ref string hdrFontFamily, ref bool hdrFontWeight)
        {
            XmlNode headerStyle = tableNode.SelectSingleNode("//COE:headerStyle", xmlManager);

            if(headerStyle != null && headerStyle.InnerText.Length > 0)
            {
                string[] styles = headerStyle.InnerText.Split(new char[1] { ';' });
                styles[0] = styles[0].Trim();
                styles[1] = styles[1].Trim();
                for(int i = 0; i < styles.Length; i++)
                {
                    if(styles[i].Length > 0)
                    {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        switch(styleDef[0].Trim())
                        {
                            case "background-color":
                                System.Drawing.Color color = new System.Drawing.Color();
                                if(styles[1].ToLower().Contains("rgb("))
                                {
                                    styles[1] = styles[1].Remove(0, styles[1].IndexOf("(") + 1);
                                    styles[1] = styles[1].Remove(styles[1].Length - 1);
                                    string[] rgb = styles[1].Split(',');
                                    uwebgrid.DisplayLayout.HeaderStyleDefault.BackColor = System.Drawing.Color.FromArgb(int.Parse(rgb[0].Trim()), int.Parse(rgb[1].Trim()), int.Parse(rgb[2].Trim()));
                                    hdrBackColor = System.Drawing.Color.FromArgb(int.Parse(rgb[0].Trim()), int.Parse(rgb[1].Trim()), int.Parse(rgb[2].Trim()));
                                }
                                else
                                {
                                    color = System.Drawing.Color.FromName(styleDef[1]);
                                    uwebgrid.DisplayLayout.HeaderStyleDefault.BackColor = color;
                                    hdrBackColor = color;
                                }
                                break;
                            case "color":
                                color = System.Drawing.Color.FromName(styleDef[1]);
                                uwebgrid.DisplayLayout.HeaderStyleDefault.ForeColor = color;
                                hdrColor = color;
                                break;

                            case "border-color":
                                {
                                    color = new System.Drawing.Color();
                                    if(styles[1].ToLower().Contains("rgb("))
                                    {
                                        styles[1] = styles[1].Remove(0, styles[1].IndexOf("(") + 1);
                                        styles[1] = styles[1].Remove(styles[1].Length - 1);
                                        string[] rgb = styles[1].Split(',');

                                        uwebgrid.DisplayLayout.HeaderStyleDefault.BorderColor = color;
                                    }
                                    else
                                    {
                                        color = System.Drawing.Color.FromName(styleDef[1]);
                                        uwebgrid.DisplayLayout.HeaderStyleDefault.BorderColor = color;
                                    }
                                    break;
                                }
                            case "border-width":
                                {
                                    uwebgrid.DisplayLayout.HeaderStyleDefault.BorderWidth = new Unit(styleDef[1]);
                                    break;
                                }
                            case "border-style":
                                {
                                    switch(styleDef[1].Trim().ToLower())
                                    {
                                        case "dashed":
                                            uwebgrid.DisplayLayout.HeaderStyleDefault.BorderStyle = BorderStyle.Dashed;
                                            break;
                                        case "dotted":
                                            uwebgrid.DisplayLayout.HeaderStyleDefault.BorderStyle = BorderStyle.Dotted;
                                            break;
                                        case "double":
                                            uwebgrid.DisplayLayout.HeaderStyleDefault.BorderStyle = BorderStyle.Double;
                                            break;
                                        case "groove":
                                            uwebgrid.DisplayLayout.HeaderStyleDefault.BorderStyle = BorderStyle.Groove;
                                            break;
                                        case "inset":
                                            uwebgrid.DisplayLayout.HeaderStyleDefault.BorderStyle = BorderStyle.Inset;
                                            break;
                                        case "solid":
                                            uwebgrid.DisplayLayout.HeaderStyleDefault.BorderStyle = BorderStyle.Solid;
                                            break;
                                        case "ridge":
                                            uwebgrid.DisplayLayout.HeaderStyleDefault.BorderStyle = BorderStyle.Ridge;
                                            break;
                                        case "outset":
                                            uwebgrid.DisplayLayout.HeaderStyleDefault.BorderStyle = BorderStyle.Outset;
                                            break;
                                        case "notset":
                                            uwebgrid.DisplayLayout.HeaderStyleDefault.BorderStyle = BorderStyle.NotSet;
                                            break;
                                        case "none":
                                            uwebgrid.DisplayLayout.HeaderStyleDefault.BorderStyle = BorderStyle.None;
                                            break;
                                    }
                                    break;
                                }
                            case "font-weight":
                                uwebgrid.DisplayLayout.HeaderStyleDefault.Font.Bold = styleDef[1].ToLower().Contains("bold");
                                hdrFontWeight = styleDef[1].ToLower().Contains("bold");
                                break;

                            case "font-family":
                                uwebgrid.DisplayLayout.HeaderStyleDefault.Font.Name = styleDef[1];
                                hdrFontFamily = styleDef[1];
                                break;

                            case "font-size":
                                uwebgrid.DisplayLayout.HeaderStyleDefault.Font.Size = new FontUnit(new Unit(styleDef[1]));

                                hdrFontSize = new FontUnit(new Unit(styleDef[1]));
                                break;

                            case "text-align":
                                switch(styleDef[1].Trim().ToLower())
                                {
                                    case "left":
                                        uwebgrid.DisplayLayout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Left;
                                        break;
                                    case "right":
                                        uwebgrid.DisplayLayout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Right;
                                        break;
                                    case "center":
                                        uwebgrid.DisplayLayout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the column visibility based on an xml attribute (hidden)
        /// </summary>
        /// <param name="col">Column to change visibility</param>
        /// <param name="value">Value to set</param>
        private void SetColumnVisibility(ref TemplatedColumn col, XmlAttribute value)
        {
            bool hidden = false;
            if(value != null && !string.IsNullOrEmpty(value.Value) && col != null)
            {
                if(bool.TryParse(value.Value, out hidden))
                    col.Hidden = hidden;
            }
        }

        /// <summary>
        /// Sets the column visibility based on an xml attribute (hidden)
        /// </summary>
        /// <param name="col">Column to change visibility</param>
        /// <param name="value">Value to set</param>
        private void SetColumnVisibility(ref UltraGridColumn col, XmlAttribute value)
        {
            bool hidden = false;
            if(value != null && !string.IsNullOrEmpty(value.Value) && col != null)
            {
                if(bool.TryParse(value.Value, out hidden))
                    col.Hidden = hidden;
            }
        }

        #endregion

        #region ICOEGrid Members
        public void SetColumnVisibility(string key, bool visibility)
        {
            if (this.uwebgrid != null)
            {
                if (this.uwebgrid.Columns.FromKey(key) != null)
                    this.uwebgrid.Columns.FromKey(key).Hidden = !visibility;
            }
        }
        #endregion

        #region ICOECultureable Members

        public System.Globalization.CultureInfo DisplayCulture
        {
            set { ViewState["DisplayCulture"] = value.Name; }
            get { return new System.Globalization.CultureInfo(ViewState["DisplayCulture"] == null ? string.Empty : ViewState["DisplayCulture"] as string); }
        }

        #endregion
    }
}
