using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.CustomReportDesigner;
using CambridgeSoft.COE.Framework.CustomReportDesigner.Dialogs;
using CambridgeSoft.COE.Framework.ServerControls.Reporting;
using Csla;
using CambridgeSoft.COE.Framework.COEReportingService.Builders;
using CambridgeSoft.COE.Framework.ServerControls.Login;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COEReportingService;
using System.Threading;
using CambridgeSoft.COE.Framework.ServerControls.Reporting.Win;
using CambridgeSoft.COE.Framework.COEPickListPickerService;
using CambridgeSoft.COE.Framework.Reporting.BLL;
using System.Collections;

namespace CambridgeSoft.COE.Framework.ReportViewer
{
    public partial class ReportViewer : Form
    {
        #region Variables
        Arguments _arguments = new Arguments();
        #endregion

        #region Methods

        #region Constructors
        public ReportViewer()
        {
            InitializeComponent();
        }

        public ReportViewer(string[] args)
            : this()
        {
            _arguments.SetArguments(args);
        }

        #endregion

        #region Common Private Methods
        internal void PerformStartup()
        {
            AccessController.Instance.LoginMaxRetries = 3;
            AccessController.Instance.LoginDialogCaption = "ReportViewer Sample Application";
            AccessController.Instance.LoginDialogLogo = System.Drawing.Bitmap.FromFile(this.GetExecutingPath() + "Layout.png");

            if (!string.IsNullOrEmpty(_arguments["username"]) && !string.IsNullOrEmpty(_arguments["password"]))
                AccessController.Instance.Login(_arguments["username"], _arguments["password"]);

            //this will show the login dialog if no user is logged (invalid credentials or no credentials at all)
            if (!AccessController.Instance.Login())
                this.ExitApplication();

            if (!string.IsNullOrEmpty(_arguments["reportTemplateId"]))
                this.reportPreviewer1.ReportId = int.Parse(_arguments["reportTemplateId"]);

            if (!string.IsNullOrEmpty(_arguments["hitlistId"]))
                this.reportPreviewer1.HitlistId = int.Parse(_arguments["hitlistId"]);

            this.LoadConfiguration();

            ArrayList items = new ArrayList();
            items.Add("Not Set");
            items.AddRange(Enum.GetValues(typeof(COEReportType)));
            ReportTypesComboBox.DataSource = items;
            ReportTypesComboBox.SelectedIndex = items.Count - 1;

            UserNameTextBox.Text = ReportSelectionUserControl.UserName = COEUser.Get();
        }

        private void LoadConfiguration()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            KeyValueConfigurationCollection settings = config.AppSettings.Settings;

            this.SelectDataView(settings["lastDataViewId"] == null ? 0 : int.Parse(settings["lastDataViewId"].Value));
            this.ResultsCriteriaTextBox.Text = settings["lastResultsCriteria"] == null ? "Pick a Results Criteria..." : settings["lastResultsCriteria"].Value;

            this.SearchCriteriaTextBox.Text = settings["lastSearchCriteria"] == null ? "Pick a Search Criteria..." : settings["lastSearchCriteria"].Value;

            this.PagingInfoTextBox.Text = settings["lastPagingInfo"] == null ? "Pick a Paging Info..." : settings["lastPagingInfo"].Value;
        }

        private void SaveConfiguration()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            KeyValueConfigurationCollection settings = config.AppSettings.Settings;

            if (settings["lastDataViewId"] == null)
                settings.Add("lastDataViewId", getDataViewId().ToString());
            else
                settings["lastDataViewId"].Value = getDataViewId().ToString();

            if (settings["lastResultsCriteria"] == null)
                settings.Add("lastResultsCriteria", ResultsCriteriaTextBox.Text);
            else
                settings["lastResultsCriteria"].Value = ResultsCriteriaTextBox.Text;

            if (settings["lastSearchCriteria"] == null)
                settings.Add("lastSearchCriteria", SearchCriteriaTextBox.Text);
            else
                settings["lastSearchCriteria"].Value = SearchCriteriaTextBox.Text;

            if (settings["lastPagingInfo"] == null)
                settings.Add("lastPagingInfo", PagingInfoTextBox.Text);
            else
                settings["lastPagingInfo"].Value = PagingInfoTextBox.Text;

            //save the file
            config.Save(ConfigurationSaveMode.Full);
            //relaod the section you modified
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }

        private string GetExecutingPath()
        {
            return Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf("\\")) + "\\";
        }

        private void ExitApplication()
        {
            if (MessageBox.Show("Are you sure you want to exit?", "lalala", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Application.Exit();
        }
        #endregion

        #region Tab1
        #region Private Methods

        private void SelectDataView(int dataViewId)
        {
            this.DataViewTextBox.Text = dataViewId.ToString();
            this.ReportSelectionUserControl.DataViewId = dataViewId;
            //this.ReportSelectionUserControl.Refresh();
        }

        private void EditReport(COEReportViewer reportViewer)
        {
            if (reportViewer.ContainsReportToShow())
            {
                CustomReportDesigner.ReportDesignerController reportDesignerController = new CambridgeSoft.COE.Framework.CustomReportDesigner.ReportDesignerController(COEPrincipal.Token);

                if (!reportViewer.IsReportReadOnly())
                    reportDesignerController.OpenReport(reportViewer.ReportBuilderMeta.Id);
                else
                    reportDesignerController.OpenReport(reportViewer.GetReportTemplate());

                ReportSelectionUserControl.Refresh();
                reportViewer.RefreshReport();
            }
        }

        private void CreateReport()
        {
            CustomReportDesigner.ReportDesignerController reportController = new CambridgeSoft.COE.Framework.CustomReportDesigner.ReportDesignerController(COEPrincipal.Token);

            int reportTemplateId = -1;

            if (GetResultsCriteria() != null)
                if (ReportSelectionUserControl.ReportType != null)
                    reportTemplateId = reportController.CreateReport(GetResultsCriteria(), this.ReportSelectionUserControl.DataViewId, this.ReportSelectionUserControl.ReportType.Value, this.ReportSelectionUserControl.Category);
                else
                    reportTemplateId = reportController.CreateReport(GetResultsCriteria(), this.ReportSelectionUserControl.DataViewId, COEReportType.List, this.ReportSelectionUserControl.Category);
            else
                if (ReportSelectionUserControl.ReportType != null)
                    reportTemplateId = reportController.CreateReport(this.ReportSelectionUserControl.DataViewId, ReportSelectionUserControl.ReportType.Value, this.ReportSelectionUserControl.Category);
                else
                    reportTemplateId = reportController.CreateReport(this.ReportSelectionUserControl.DataViewId, COEReportType.List, this.ReportSelectionUserControl.Category);

            if (reportTemplateId > 0)
            {
                reportPreviewer1.Clear();
                ReportSelectionUserControl.Refresh();
                ReportSelectionUserControl.SelectReport(reportTemplateId);
            }
        }

        private void DisplaySelectedReport()
        {
            if (ReportSelectionUserControl.SelectedReportBuilderMeta.Id >= 0)
            {
                this.reportPreviewer1.ReportBuilderMeta = ReportSelectionUserControl.SelectedReportBuilderMeta;

                if (!ReportSelectionUserControl.SelectedReportBuilderMeta.Class.Equals(typeof(DataBaseReportBuilder)) && this.getDataViewId() > 0 && GetResultsCriteria() != null)
                {
                    string xmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

                    if (GetResultsCriteria().ToString().StartsWith("<?xml"))
                        xmlHeader = GetResultsCriteria().ToString().Substring(0, GetResultsCriteria().ToString().IndexOf(">") + 1);

                    string resultsCriteria = GetResultsCriteria().ToString().Substring(GetResultsCriteria().ToString().IndexOf(">") + 1).Trim();

                    this.reportPreviewer1.ReportBuilderMeta.Config = string.Format("{0}<configInfo><dataViewId>{1}</dataViewId><resultsCriteria>{2}</resultsCriteria><baseReportTemplateId>{3}</baseReportTemplateId></configInfo>", xmlHeader, getDataViewId(), resultsCriteria, 61);
                }
            }
            else
                this.reportPreviewer1.Clear();
        }

        private DialogResult ShowOpenXmlFileDialog(string previousFileName, out string filename)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "xml files|*.xml|All files|*.*";
            openFileDialog.InitialDirectory = Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf("\\")) + DataViewTextBox.Text;

            DialogResult result = openFileDialog.ShowDialog();
            filename = openFileDialog.FileName;

            return result;
        }

        private void ShowMessage(string message)
        {
            this.StatusTextBox.Text += message + "\r\n";
        }

        private void ShowMessage(Control control, string message)
        {
            this.ShowMessage(message);
            FormErrorProvider.SetError(control, message);
        }

        private int getDataViewId()
        {
            return ReportSelectionUserControl.DataViewId;
        }

        private SearchCriteria GetSearchCriteria()
        {
            try
            {
                if (string.IsNullOrEmpty(SearchCriteriaTextBox.Text) || !File.Exists(SearchCriteriaTextBox.Text))
                    return null;

                XmlDocument document = new XmlDocument();
                document.LoadXml(File.ReadAllText(SearchCriteriaTextBox.Text));

                return new SearchCriteria(document);
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        private ResultsCriteria GetResultsCriteria()
        {
            try
            {
                if (string.IsNullOrEmpty(ResultsCriteriaTextBox.Text) || !File.Exists(ResultsCriteriaTextBox.Text))
                    return null;

                return ResultsCriteria.GetResultsCriteria(File.ReadAllText(ResultsCriteriaTextBox.Text));
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        private PagingInfo GetPagingInfo()
        {
            try
            {
                if (string.IsNullOrEmpty(PagingInfoTextBox.Text) || !File.Exists(PagingInfoTextBox.Text))
                {
                    PagingInfo pagingInfo = new PagingInfo();
                    pagingInfo.Start = 0;
                    pagingInfo.End = pagingInfo.RecordCount = int.MaxValue;

                    return pagingInfo;
                }

                XmlDocument document = new XmlDocument();
                document.LoadXml(File.ReadAllText(PagingInfoTextBox.Text));

                return new PagingInfo(document);
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        private void GenerateReport()
        {
            this.FormErrorProvider.Clear();
            this.StatusTextBox.Clear();

            if (string.IsNullOrEmpty(this.DataViewTextBox.Text))
                this.ShowMessage(this.DataViewButton, "Pick a dataview!");

            if (string.IsNullOrEmpty(this.ResultsCriteriaTextBox.Text) || GetResultsCriteria() == null)
                this.ShowMessage(this.ResultsCriteriaButton, "Pick a valid ResultsCriteria!");

            if (string.IsNullOrEmpty(StatusTextBox.Text))
            {
                Cursor currentCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;

                ResultsCriteriaReportBuilder templateBuilder = new ResultsCriteriaReportBuilder(getDataViewId(), GetResultsCriteria(), 61);
                reportPreviewer1.ReportBuilder = templateBuilder;
                Cursor.Current = currentCursor;
            }
        }

        private void DoSearch()
        {
            this.FormErrorProvider.Clear();
            this.StatusTextBox.Clear();

            int dataviewId;
            if (string.IsNullOrEmpty(this.DataViewTextBox.Text) || !int.TryParse(this.DataViewTextBox.Text, out dataviewId))
                this.ShowMessage(this.DataViewButton, "Pick a dataview!");

            if (string.IsNullOrEmpty(this.ResultsCriteriaTextBox.Text) || GetResultsCriteria() == null)
                this.ShowMessage(this.ResultsCriteriaButton, "Pick a valid ResultsCriteria!");

            if (string.IsNullOrEmpty(this.PagingInfoTextBox.Text) || GetPagingInfo() == null)
                this.ShowMessage(this.PagingInfoButton, "Pick a PagingInfo!");

            if (string.IsNullOrEmpty(StatusTextBox.Text))
            {
                Cursor currentCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;

                COESearch searchService = new COESearch();

                if (GetSearchCriteria() != null)
                {
                    SearchResponse searchResponse = searchService.DoSearch(GetSearchCriteria(), GetResultsCriteria(), GetPagingInfo(), getDataViewId(), bool.TrueString);

                    if (this.UseHitlistIdRadioButton.Checked)
                        this.reportPreviewer1.HitlistId = searchResponse.HitListInfo.HitListID;
                    else
                        this.reportPreviewer1.DataSource = searchResponse.ResultsDataSet;

                    this.ShowMessage(string.Format("{0} rows affected.", searchResponse.PagingInfo.RecordCount));
                }
                else
                {
                    DataSet dataset = searchService.GetData(GetResultsCriteria(), GetPagingInfo(), getDataViewId(), bool.TrueString);

                    if (this.UseHitlistIdRadioButton.Checked)
                        this.reportPreviewer1.HitlistId = 0;
                    else
                        this.reportPreviewer1.DataSource = dataset;

                    this.ShowMessage(string.Format("{0} rows affected.", dataset.Tables[0].Rows.Count));
                }

                Cursor.Current = currentCursor;
            }
        }

        private void HandleException(Exception exception)
        {
            MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        #region Events
        void reportSelectionUserControl1_SelectedReportBuilderInfoChanged(object sender, System.EventArgs e)
        {
            try
            {
                this.DisplaySelectedReport();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }

        private void ReportViewer_Load(object sender, EventArgs e)
        {
            try
            {
                this.PerformStartup();
                this.reportPreviewer1.RefreshReport();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }

        private void reportPreviewer1_EditReportRequested(object sender, EventArgs e)
        {
            try
            {
                EditReport((COEReportViewer)sender);
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }
        private void ReportSelectionUserControl_CreateNewReport(object sender, EventArgs e)
        {
            try
            {
                CreateReport();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }

        private void ReportSelectionUserControl_DeleteSelectedReport(object sender, EventArgs e)
        {
            try
            {
                this.DisplaySelectedReport();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }

        private void ResultsCriteriaButton_Click(object sender, EventArgs e)
        {
            string filename;
            if (ShowOpenXmlFileDialog(ResultsCriteriaTextBox.Text, out filename) == DialogResult.OK)
                this.ResultsCriteriaTextBox.Text = filename;
        }

        private void SearchCriteriaButton_Click(object sender, EventArgs e)
        {
            string filename;
            if (ShowOpenXmlFileDialog(SearchCriteriaTextBox.Text, out filename) == DialogResult.OK)
                this.SearchCriteriaTextBox.Text = filename;
        }

        private void DataViewButton_Click(object sender, EventArgs e)
        {
            DataBaseOpenDialog dataViewOpenDialog = new DataBaseOpenDialog("Pick a DataView");
            dataViewOpenDialog.DataMember = "COEDataViewBOList";

            COEServicesAdapter servicesAdapter = COEServicesAdapter.GetInstance();
            dataViewOpenDialog.DataSource = servicesAdapter.GetAvailableDataViews();

            if (dataViewOpenDialog.ShowDialog() == DialogResult.OK)
                this.SelectDataView(dataViewOpenDialog.SelectedId);
        }

        private void PagingInfoButton_Click(object sender, EventArgs e)
        {
            string filename;
            if (ShowOpenXmlFileDialog(PagingInfoTextBox.Text, out filename) == DialogResult.OK)
                this.PagingInfoTextBox.Text = filename;
        }


        private void DataViewTextBox_TextChanged(object sender, EventArgs e)
        {
            int id;
            if (int.TryParse(((TextBox)sender).Text, out id))
                this.SelectDataView(id);
            else
                this.SelectDataView(0);
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            try
            {
                DoSearch();
            }
            catch (DataPortalException dataPortalException)
            {
                this.ShowMessage(dataPortalException.BusinessException == null ? dataPortalException.Message : dataPortalException.BusinessException.Message);
            }
            catch (Exception exception)
            {
                this.ShowMessage(exception.Message);
            }
        }

        private void ReportViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.SaveConfiguration();
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (GetResultsCriteria() != null)
                System.Diagnostics.Process.Start(ResultsCriteriaTextBox.Text);
        }

        private void SampleDataSetPassingReportViewer_ExitRequested(object sender, EventArgs e)
        {
            this.ExitApplication();
        }

        private void GenerateReportButton_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void SampleDataSetPassingReportViewer_EditReportRequested(object sender, EventArgs e)
        {
            ReportDesignerController controller = new ReportDesignerController();
            controller.OpenReport(this.SampleDataSetPassingReportViewer.ReportId);
        }
        #endregion

        #endregion

        #region Tab2
        #region Variables
        private int _sampleReportId = -1;
        DataSet _dataSet;
        #endregion

        #region Constants
        private const string _sampleReportName = "COEDataViewDataSetPassing";
        private const string _sampleReportDescription = "Label - COEDataView Table for dataset passing sample";
        #endregion

        #region Methods
        private void EnsureDataSetPassingReport()
        {
            if (_sampleReportId < 0)
            {
                COEReportBOList coeReportTemplateBOList = COEReportBOList.GetCOEReportBOList(_sampleReportName, _sampleReportDescription);
                if (coeReportTemplateBOList.Count > 0)
                {
                    _sampleReportId = coeReportTemplateBOList[0].ID;
                    return;
                }

                COEReportBO reportTemplateBO = COEReportBO.New(_sampleReportName, _sampleReportDescription, GetDefaultSampleReportTemplate());
                reportTemplateBO = reportTemplateBO.Save();

                _sampleReportId = reportTemplateBO.ID;
            }
        }

        private COEReport GetDefaultSampleReportTemplate()
        {
            string sampleTemplatePath = GetExecutingPath() + "SampleReport.xml";
            XmlDocument document = new XmlDocument();
            document.Load(sampleTemplatePath);

            return (COEReport)Framework.Common.Utilities.XmlDeserialize(document.OuterXml, typeof(COEReport));
        }

        #endregion

        #region Event Handlers
        private void RecordPickingButton_Click(object sender, EventArgs e)
        {
            DataBaseOpenDialog dataViewOpenDialog = new DataBaseOpenDialog("Pick a Record");
            dataViewOpenDialog.DataMember = "COEDataViewBOList";

            COEServicesAdapter servicesAdapter = COEServicesAdapter.GetInstance();
            dataViewOpenDialog.DataSource = servicesAdapter.GetAvailableDataViews();

            if (dataViewOpenDialog.ShowDialog() == DialogResult.OK)
            {
                List<COEDataViewBO> dataviewList = new List<COEDataViewBO>();

                dataviewList.Add(COEDataViewBO.Get(dataViewOpenDialog.SelectedId));

                _dataSet = servicesAdapter.BuildDataSet(dataviewList, "COEDATAVIEW", new string[] { "ID", "Name", "Description", "DatabaseName", "UserName", "BaseTable" });

                SampleDataSetPassingReportViewer.DataSource = _dataSet;
                SampleDataSetPassingReportViewer.ReportId = _sampleReportId;

                this.CreateReportLinkLabel.Enabled = true;
            }
        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            EnsureDataSetPassingReport();
        }

        private void DesignReportLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int reportId;

            ReportDesignerController controller = new ReportDesignerController();
            if ((reportId = controller.CreateReport(_dataSet)) > 0)
                this.SampleDataSetPassingReportViewer.ReportId = reportId;
        }

        private void ReportTypesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedItem is COEReportType)
                this.ReportSelectionUserControl.ReportType = (COEReportType)((ComboBox)sender).SelectedItem;
            else
                this.ReportSelectionUserControl.ReportType = null;
        }

        private void CategoriesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ReportSelectionUserControl.Category = ((ComboBox)sender).SelectedItem.ToString();
        }

        private void UserNameTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                this.ReportSelectionUserControl.UserName = ((TextBox)sender).Text;
        }

        private void reportPreviewer1_ExitRequested(object sender, EventArgs e)
        {
            this.ExitApplication();
        }
        #endregion
        #endregion
        #endregion
    }
}
