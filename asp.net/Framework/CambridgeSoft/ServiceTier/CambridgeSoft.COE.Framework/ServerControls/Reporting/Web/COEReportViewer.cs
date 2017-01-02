using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.COEReportingService.Builders;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Properties;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web;

namespace CambridgeSoft.COE.Framework.ServerControls.Reporting.Web
{
    /// <summary>
    /// to use this control is neccessary register the following httpModule manualy on web.config file on system.web/httpModules, due to            know DevExpress bug:
    /// &lt;add type="DevExpress.Web.ASPxClasses.ASPxHttpHandlerModule, DevExpress.Web.v9.3, Version=9.3.4.0, Culture=neutral,                      PublicKeyToken="b88d1754d700e49a" name="ASPxHttpHandlerModule"/&gt;
    /// </summary>
    [DefaultProperty("Text")]
    [Serializable()]
    [ToolboxData("<{0}:COEReportViewer runat=server></{0}:COEReportViewer>")]
    public class COEReportViewer : CompositeControl, IReportViewer
    {
        #region Variables
        ReportViewerController _controller = null;
        private XtraReport _blankReport = new XtraReport();
        private ReportViewer _reportViewer;
        private ReportToolbar _reporToolbar;
        private bool _showToolbar = true;
        private const int TEMPLATEBUILDERCONTROLSTATEINDEX = 1;
        private const string AUTENTICATIONTICKETPARAMETERNAME = "autenticationTicket";
        private const string REPORTTEMAPLATEIDPARAMETERNAME = "reportTemplateId";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or set the visibility of the report toolbar.
        /// </summary>
        public bool ShowToolbar
        {
            get
            {
                return _showToolbar;
            }
            set
            {
                _showToolbar = value;
                this.ChildControlsCreated = false;
            }
        }

        /// <summary>
        /// Gets or set the report template builder of the report to be show.
        /// </summary>
        public ReportBuilderBase ReportBuilder
        {
            get { return _controller.ReportBuilder; }
            set
            {
                _controller.ReportBuilder = value;
                this.ChildControlsCreated = false;
            }
        }

        /// <summary>
        /// Gets or set the template builder info of the report to be show.
        /// </summary>
        public ReportBuilderMeta ReportBuilderMeta
        {
            get { return _controller.ReportBuilderMeta; }
            set
            {
                _controller.ReportBuilderMeta = value;
                this.ChildControlsCreated = false;
            }
        }

        public int HitlistId
        {
            get
            {
                return _controller.HitListId;
            }
            set
            {
                _controller.HitListId = value;
            }
        }

        public object DataSource
        {
            get
            {
                return _controller.DataSource;
            }
            set
            {
                _controller.DataSource = value;
            }
        }

        public int ReportId
        {
            get 
            {
                return _controller.ReportId;
            }
            set 
            {
                _controller.ReportId = value;
            }
        }

        /// <summary>
        /// Gets or set the report to be show.
        /// </summary>
        public XtraReport Report
        {
            get { return this._reportViewer.Report; }
            set
            {
                this._reportViewer.Report = value;
                this.ChildControlsCreated = false;
            }
        }

        #endregion

        #region Methods

        #region Constructors

        public COEReportViewer()
        {
            _controller = new ReportViewerController(this);
        }
        #endregion

        //to put the edit and refresh buttons (custom buttons) inside the report toolbar is neccessary build this manualy.
        private void BuildReportToolbar()
        {
            this._reporToolbar = new ReportToolbar();
            this._reporToolbar.Items.Add(new ReportToolbarButton(ReportToolbarItemKind.PrintReport));
            this._reporToolbar.Items.Add(new ReportToolbarButton(ReportToolbarItemKind.PrintPage));
            this._reporToolbar.Items.Add(new ReportToolbarSeparator());
            this._reporToolbar.Items.Add(new ReportToolbarButton(ReportToolbarItemKind.FirstPage, true));
            this._reporToolbar.Items.Add(new ReportToolbarButton(ReportToolbarItemKind.PreviousPage, true));
            this._reporToolbar.Items.Add(new ReportToolbarLabel("Page"));
            this._reporToolbar.Items.Add(new ReportToolbarComboBox(ReportToolbarItemKind.PageNumber));
            this._reporToolbar.Items.Add(new ReportToolbarLabel("of"));
            this._reporToolbar.Items.Add(new ReportToolbarTextBox(ReportToolbarItemKind.PageCount, null, true));
            this._reporToolbar.Items.Add(new ReportToolbarButton(ReportToolbarItemKind.NextPage));
            this._reporToolbar.Items.Add(new ReportToolbarButton(ReportToolbarItemKind.LastPage));
            this._reporToolbar.Items.Add(new ReportToolbarSeparator());
            this._reporToolbar.Items.Add(new ReportToolbarButton(ReportToolbarItemKind.SaveToWindow, true));
            this._reporToolbar.Items.Add(new ReportToolbarButton(ReportToolbarItemKind.SaveToDisk, true));
            ReportToolbarComboBox SaveFormat = new ReportToolbarComboBox(ReportToolbarItemKind.SaveFormat);
            foreach (string format in Enum.GetNames(typeof(SavedFormats)))
            {
                SaveFormat.Elements.Add(new ListElement(format.ToLower()));
            }
            SaveFormat.Width = Unit.Pixel(70);
            this._reporToolbar.Items.Add(SaveFormat);
            ReportToolbarButton EditReportToolbarButton = new ReportToolbarButton(ReportToolbarItemKind.Custom, true);
            EditReportToolbarButton.Text = "Edit Report";
            EditReportToolbarButton.ToolTip = "To edit current report on Report End User Designer";
            EditReportToolbarButton.Name = ItemClickAction.Edit.ToString();
            this._reporToolbar.Items.Add(EditReportToolbarButton);
            this._reporToolbar.Items.Add(new ReportToolbarSeparator());
            ReportToolbarButton RefreshReportToolbarButton = new ReportToolbarButton(ReportToolbarItemKind.Custom, true);
            RefreshReportToolbarButton.Text = "Refresh";
            RefreshReportToolbarButton.ToolTip = "To reload current report from database";
            RefreshReportToolbarButton.Name = ItemClickAction.Refresh.ToString();
            this._reporToolbar.Items.Add(RefreshReportToolbarButton);
            this._reporToolbar.ClientSideEvents.ItemClick = @"function(s, e)
                                                            {
                                                                if(e.item.name == 'Refresh' || e.item.name == 'Edit')
                                                                    __doPostBack(e.item.name,''); 
                                                            }";
        }

        /// <summary>
        /// To edit the showed report on the Report Designer.
        /// </summary>
        public void EditReport()
        {
            string autenticationTicket = string.Empty;
            if (Page.Request.Cookies["COESSO"] != null && !string.IsNullOrEmpty(Page.Request.Cookies["COESSO"].Value))
                autenticationTicket = Page.Request.Cookies["COESSO"].Value;

            this.Page.Response.Redirect(Resources.ReportEUD_URL + "?" + REPORTTEMAPLATEIDPARAMETERNAME + "=" + ReportBuilderMeta.Id.ToString() + "&" + AUTENTICATIONTICKETPARAMETERNAME + "=" + autenticationTicket);

        }

        /// <summary>
        /// to show the report <paramref name="report"/>
        /// </summary>
        /// <param name="report">the report to be show.</param>
        /// <param name="readOnly">if is false the report will be read only.</param>
        public void ShowReport(XtraReport report, bool readOnly)
        {
            EnsureChildControls();
            this._reportViewer.Report = report == null ? _blankReport : report;

            if (_showToolbar)
            {
                this._reporToolbar.Enabled = (report != null);
                ((ReportToolbarButton)this._reporToolbar.Items[_reporToolbar.Items.Count - 3]).Enabled = !readOnly;
            }
        }

        private void Refresh()
        {
            _controller.DisplayReport();
        }

        #endregion

        #region Overide Methods

        protected override void CreateChildControls()
        {
            if (_showToolbar)
            {
                this._reporToolbar = new ReportToolbar();
                this.BuildReportToolbar();
            }
            this._reportViewer = new ReportViewer();
            this._reportViewer.ID = "COEReportViewer";

            if (_showToolbar)
            {
                this._reporToolbar.ReportViewer = this._reportViewer;
                this.Controls.Add(_reporToolbar);
            }

            this.Controls.Add(_reportViewer);
            this.AddASPxHttpHandlerModule();
            base.CreateChildControls();

            _controller.DisplayReport();
        }

        private void AddASPxHttpHandlerModule() 
        {
           //TODO add the neccessary http module reference on web.config.          
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Page.RegisterRequiresControlState(this);
        }

        //this method is overridden to chek the report toolbar custom items click acction.
        protected override void OnLoad(EventArgs e)
        {
            if (this.Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(this.Page.Request["__EVENTTARGET"]))
                {
                    ItemClickAction action;
                    try { action = (ItemClickAction)Enum.Parse(typeof(ItemClickAction), this.Page.Request["__EVENTTARGET"], true); }
                    catch { action = ItemClickAction.None; }

                    switch (action)
                    {
                        case ItemClickAction.Edit:
                            this.EditReport();
                            break;
                        case ItemClickAction.Refresh:
                            this.Refresh();
                            break;
                        default:
                            break;
                    }
                }
            }
            base.OnLoad(e);
        }

        protected override object SaveControlState()
        {
            object[] savedState = new object[2];
            savedState[0] = base.SaveControlState();
            savedState[TEMPLATEBUILDERCONTROLSTATEINDEX] = ReportBuilderMeta;
            return savedState;
        }

        protected override void LoadControlState(object savedState)
        {
            object[] saved = (object[])savedState;
            base.LoadControlState(saved[0]);
            ReportBuilderMeta = (ReportBuilderMeta)saved[TEMPLATEBUILDERCONTROLSTATEINDEX];
        }

        #endregion

        #region Enums

        private enum SavedFormats
        {
            Pdf,
            Xls,
            Rtf,
            Mht,
            Txt,
            Csv,
            Png
        }

        private enum ItemClickAction
        {
            None,
            Refresh,
            Edit
        }

        #endregion
    }
}
