using System;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.COEReportingService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ServerControls.Login;
using CambridgeSoft.COE.Framework.CustomReportDesigner.Dialogs;
using System.Reflection;
using System.Data;
using CambridgeSoft.COE.Framework.CustomReportDesigner.Properties;
using CambridgeSoft.COE.Framework.Reporting.BLL;
using DevExpress.XtraReports.Design;

namespace CambridgeSoft.COE.Framework.CustomReportDesigner
{
    public class ReportDesignerController
    {
        #region Variables
        private ReportDesignerForm _reportDesignerForm;
        private COEReportBO _reportTemplateBO;
        private CambridgeSoft.COE.Framework.ServerControls.Login.Arguments _arguments = new CambridgeSoft.COE.Framework.ServerControls.Login.Arguments();

        #endregion

        #region Properties

        public ReportDesignerForm ReportDesignerForm
        {
            get
            {
                if (_reportDesignerForm == null)
                    _reportDesignerForm = new ReportDesignerForm(this);

                return _reportDesignerForm;
            }
        }
        #endregion

        #region Methods
        #region Constructors

        public ReportDesignerController(string username, string password)
        {
            AccessController.Instance.Login(username, password);
        }

        public ReportDesignerController(string authenticationTicket)
        {
            AccessController.Instance.Login(authenticationTicket);
        }

        public ReportDesignerController(string[] args)
        {
            _arguments.SetArguments(args);
        }

        public ReportDesignerController()
        {
        }

        #endregion

        #region public Methods
        /// <summary>
        /// Loads the provided report on designer and displays the form.
        /// </summary>
        /// <param name="report">the report to load</param>
        /// <returns></returns>
        public DialogResult OpenReport(COEReport report)
        {
            this.LoadReportTemplate(report);
            return this.ReportDesignerForm.ShowDialog();
        }

        /// <summary>
        /// Loads a report from database and presents it on report designer.
        /// </summary>
        /// <param name="reportTemplateId">The id corresponding to a report stored on database</param>
        /// <returns></returns>
        public DialogResult OpenReport(int reportTemplateId)
        {
            this.LoadReportTemplate(reportTemplateId);
            this.ReportDesignerForm.Visible = false;
            return this.ReportDesignerForm.ShowDialog();
        }

        /// <summary>
        /// Creates a new report, related to the search service, and displays the designer form. Note that, due to the report not having a dataview, it will have to be picked by the user.
        /// </summary>
        /// <returns>the Id of the last report being edited.</returns>        
        public int CreateReport()
        {
            return this.CreateReport(-1);
        }

        /// <summary>
        /// Creates a new report, related to the search service, and displays the designer form.
        /// </summary>
        /// <param name="dataviewId">The dataview the report will be related to. The report will only be able to use fields on this dataview</param>
        /// <returns>the Id of the last report being edited.</returns>
        public int CreateReport(int dataviewId)
        {
            return CreateReport(null, dataviewId, 0);
        }

        /// <summary>
        /// Creates a new report, related to the search service, and displays the designer form.
        /// </summary>
        /// <param name="dataviewId">The dataview the report will be related to. The report will only be able to use fields on this dataview</param>
        /// <param name="reportType">Type of the report (COEReportType): currently list or label</param>
        /// <returns>the Id of the last report being edited.</returns>
        public int CreateReport(int dataviewId, COEReportType reportType)
        {
            return CreateReport(null, dataviewId, reportType);
        }

        /// <summary>
        /// Creates a new report, related to the search service, and displays the designer form.
        /// </summary>
        /// <param name="dataviewId">The dataview the report will be related to. The report will only be able to use fields on this dataview</param>
        /// <param name="reportType">Type of the report (COEReportType): currently list or label</param>
        /// <param name="category">A string that assigns a client application specific "category" to the report, for filtering purposes</param>
        /// <returns>the Id of the last report being edited.</returns>
        public int CreateReport(int dataviewId, COEReportType reportType, string category)
        {
            return CreateReport(null, dataviewId, reportType, category);
        }

        /// <summary>
        /// Creates a new report, related to the search service, and displays the designer form.
        /// </summary>
        /// <param name="resultsCriteria">The (initial) fields that are available for the user to pick. It will be auto-generated from the dataview if not present. This can then be edited by the user.</param>
        /// <param name="dataviewId">The dataview the report will be related to. The report will only be able to use fields on this dataview</param>
        /// <returns>the Id of the last report being edited.</returns>
        public int CreateReport(ResultsCriteria resultsCriteria, int dataviewId)
        {
            return this.CreateReport(resultsCriteria, dataviewId, 0);
        }

        /// <summary>
        /// Creates a new report, related to the search service, and displays the designer form.
        /// </summary>
        /// <param name="resultsCriteria">The (initial) fields that are available for the user to pick. It will be auto-generated from the dataview if not present. This can then be edited by the user.</param>
        /// <param name="dataviewId">The dataview the report will be related to. The report will only be able to use fields on this dataview</param>
        /// <param name="reportType">Type of the report (COEReportType): currently list or label</param>
        /// <returns>the Id of the last report being edited.</returns>
        public int CreateReport(ResultsCriteria resultsCriteria, int dataviewId, COEReportType reportType)
        {
            return this.CreateReport(resultsCriteria, dataviewId, reportType, string.Empty);
        }

        /// <summary>
        /// Creates a new report, related to the search service, and displays the designer form.
        /// </summary>
        /// <param name="resultsCriteria">The (initial) fields that are available for the user to pick. It will be auto-generated from the dataview if not present. This can then be edited by the user.</param>
        /// <param name="dataviewId">The dataview the report will be related to. The report will only be able to use fields on this dataview</param>
        /// <param name="reportType">Type of the report (COEReportType): currently list or label</param>
        /// <param name="category">A string that assigns a client application specific "category" to the report, for filtering purposes</param>
        /// <returns>the Id of the last report being edited.</returns>
        public int CreateReport(ResultsCriteria resultsCriteria, int dataviewId, COEReportType reportType, string category)
        {
            if (this.CreateReportFlow(dataviewId, resultsCriteria, reportType, category))
            {
                this.ReportDesignerForm.Visible = false;
                this.ReportDesignerForm.ShowDialog();
                return _reportTemplateBO.ID;
            }
            return -1;
        }

        /// <summary>
        /// Creates a new report, independent form search service, and displays the designer form.
        /// </summary>
        /// <param name="dataset">A rowless dataset that contains the tables and fields that are going to be available for the user to pick.</param>
        /// <returns>the Id of the last report being edited.</returns>
        public int CreateReport(DataSet dataset)
        {
            return CreateReport(dataset, 0);
        }

        /// <summary>
        /// Creates a new report, independent form search service, and displays the designer form.
        /// </summary>
        /// <param name="dataset">A rowless dataset that contains the tables and fields that are going to be available for the user to pick.</param>
        /// <param name="reportType">Type of the report (COEReportType): currently list or label</param>
        /// <returns>the Id of the last report being edited.</returns>
        public int CreateReport(DataSet dataset, COEReportType reportType)
        {
            return CreateReport(dataset, reportType, string.Empty);
        }

        /// <summary>
        /// Creates a new report, independent form search service, and displays the designer form.
        /// </summary>
        /// <param name="dataset">A rowless dataset that contains the tables and fields that are going to be available for the user to pick.</param>
        /// <param name="reportType">Type of the report (COEReportType): currently list or label</param>
        /// <param name="category">A string that assigns a client application specific "category" to the report, for filtering purposes</param>
        /// <returns>the Id of the last report being edited.</returns>
        public int CreateReport(DataSet dataset, COEReportType reportType, string category)
        {
            if (this.CreateReportFlow(dataset, reportType, category))
            {
                this.ReportDesignerForm.ShowDialog();

                return _reportTemplateBO.ID;
            }
            return -1;
        }
        #endregion

        #region Interface Methods
        internal bool CreateReportFlow(bool useSearchService)
        {
            if (useSearchService)
                return CreateReportFlow(-1);
            else
                return CreateReportFlow(null);
        }

        internal bool CreateReportFlow(int dataviewId)
        {
            return CreateReportFlow(dataviewId, null, COEReportType.List, string.Empty);
        }

        internal bool CreateReportFlow(int dataviewId, ResultsCriteria resultsCriteria, COEReportType reportType, string category)
        {
            try
            {
                _reportTemplateBO = COEReportBO.New(string.Empty);
                
                if (dataviewId <= 0)
                {
                    DataBaseOpenDialog dataViewOpenDialog = new DataBaseOpenDialog(Resources.PickDataView);
                    dataViewOpenDialog.DataMember = "COEDataViewBOList";
                    dataViewOpenDialog.DataSource = COEServicesAdapter.GetInstance().GetAvailableDataViews();

                    if (dataViewOpenDialog.ShowDialog() == DialogResult.OK)
                        dataviewId = dataViewOpenDialog.SelectedId;
                    else
                        return false;
                }

                this.CreateReportTemplate(dataviewId, resultsCriteria, reportType, category);

                ReportDesignerForm.CreateNewReport(Resources.NewReport, _reportTemplateBO.ReportDefinition.Dataview.Tables.getById(_reportTemplateBO.ReportDefinition.Dataview.Basetable).Alias /*"Table_" + _reportTemplateBO.ReportTemplate.Dataview.Basetable*/);
                ReportDesignerForm.DisplayAvailableFields(CreateFieldListDataSource(_reportTemplateBO.ReportDefinition.ResultsCriteria, _reportTemplateBO.ReportDefinition.Dataview));

                this.ReportDesignerForm.Visible = true;

                if (reportType.Equals(COEReportType.Label))
                    new CustomLabelWizardRunner(this.ReportDesignerForm.Report).Run();
                        
                ReportDesignerForm.Report.Category = category;

                return true;
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return false;
            }
        }

        internal bool CreateReportFlow(DataSet dataset)
        {
            return CreateReportFlow(dataset, 0, string.Empty);
        }

        internal bool CreateReportFlow(DataSet dataset, COEReportType reportType, string category)
        {
            try
            {
                _reportTemplateBO = COEReportBO.New(string.Empty);
                ReportDesignerForm.CreateNewReport(Resources.NewReport, dataset, dataset != null && dataset.Tables.Count > 0 ? dataset.Tables[0].TableName : null);

                if (dataset != null)
                    ReportDesignerForm.DisplayAvailableFields(new COEDescriptiveDataSet(Resources.AvailableReportFields, dataset));

                this.ReportDesignerForm.Visible = true;

                if (reportType.Equals(COEReportType.Label))
                    new CustomLabelWizardRunner(this.ReportDesignerForm.Report).Run();
                        

                ReportDesignerForm.Report.Category = category;

                return true;
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return false;
            }
        }


        internal bool OpenReportFlow()
        {
            try
            {
                DataBaseOpenDialog reportTemplatesOpenDialog = new DataBaseOpenDialog(Resources.OpenReportTemplate);
                reportTemplatesOpenDialog.DataMember = "COEReportTemplateBOList";
                reportTemplatesOpenDialog.DataSource = COEServicesAdapter.GetInstance().GetAvailableReportTemplates();

                if (reportTemplatesOpenDialog.ShowDialog() == DialogResult.OK)
                {
                    return this.LoadReportTemplate(reportTemplatesOpenDialog.SelectedId);
                }
                return false;
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return false;

            }
        }

        internal bool SaveReportFlow(string reportLayout, bool doSaveAs)
        {
            try
            {
                if (_reportTemplateBO.IsNew || doSaveAs)
                {
                    DataBaseSaveDialog reportTemplatesSaveDialog = new DataBaseSaveDialog(Resources.SaveReportTemplateAs);
                    reportTemplatesSaveDialog.DataMember = "COEReportTemplateBOList";
                    reportTemplatesSaveDialog.DataSource = COEServicesAdapter.GetInstance().GetAvailableReportTemplates();
                    reportTemplatesSaveDialog.NewRecordName = _reportTemplateBO.Name;
                    reportTemplatesSaveDialog.NewRecordDescription = _reportTemplateBO.Description;

                    if (reportTemplatesSaveDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (reportTemplatesSaveDialog.SelectedId > 0)
                        {
                            if (reportTemplatesSaveDialog.SelectedId != this._reportTemplateBO.ID)
                                this._reportTemplateBO = COEReportBO.Get(reportTemplatesSaveDialog.SelectedId);
                        }
                        else
                        {
                            this._reportTemplateBO = COEReportBO.New(reportTemplatesSaveDialog.NewRecordName,
                                                                                reportTemplatesSaveDialog.NewRecordDescription,
                                                                                _reportTemplateBO.ReportDefinition);
                        }
                    }
                    else
                        return false;
                }

                this._reportTemplateBO.ReportDefinition.ReportLayout = reportLayout;
                this._reportTemplateBO.ReportType = this.ReportDesignerForm.Report.ReportType;
                this._reportTemplateBO.Category = this.ReportDesignerForm.Report.Category;
                this._reportTemplateBO = this._reportTemplateBO.Save();

                ReportDesignerForm.ShowReport(_reportTemplateBO.Name, _reportTemplateBO.ReportDefinition.ReportLayout);

                return true;
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return false;
            }
        }

        internal void ShowDataView()
        {
            try
            {
                if (_reportTemplateBO != null && this._reportTemplateBO.ReportDefinition != null)
                {
                    if (this._reportTemplateBO.ReportDefinition.Dataview != null)
                    {
                        XmlViewerDialog xmlViewer = new XmlViewerDialog();
                        xmlViewer.ShowDialog(this._reportTemplateBO.ReportDefinition.Dataview.ToString());
                    }
                    else
                        this.ShowErrorMessage(Resources.NoDataViewToDisplay);
                }
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }

        internal void EditResultsCriteria()
        {
            try
            {
                if (_reportTemplateBO != null && _reportTemplateBO.ReportDefinition != null)
                {
                    if (this._reportTemplateBO.ReportDefinition.ResultsCriteria != null)
                    {
                        ResultsCriteriaEditor resultsCriteriaEditor = new ResultsCriteriaEditor(this._reportTemplateBO.ReportDefinition.ResultsCriteria, this._reportTemplateBO.ReportDefinition.Dataview);

                        if (resultsCriteriaEditor.ShowDialog() == DialogResult.OK)
                        {
                            this._reportTemplateBO.ReportDefinition.ResultsCriteria = resultsCriteriaEditor.ResultsCriteria;
                            ReportDesignerForm.DisplayAvailableFields(CreateFieldListDataSource(_reportTemplateBO.ReportDefinition.ResultsCriteria, _reportTemplateBO.ReportDefinition.Dataview));
                        }
                    }
                    else
                        this.ShowErrorMessage(Resources.NoResultsCriteriaToDisplay);
                }
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }

        internal void Initialize()
        {
            if (!string.IsNullOrEmpty(_arguments["username"]) && !string.IsNullOrEmpty(_arguments["password"]))
                AccessController.Instance.Login(_arguments["username"], _arguments["password"]);

            if (!string.IsNullOrEmpty(_arguments["autenticationTicket"]))
                AccessController.Instance.Login(_arguments["autenticationTicket"]);

            //this will show the login dialog if no user is logged (invalid credentials or no credentials at all)
            AccessController.Instance.LoginMaxRetries = 3;
            AccessController.Instance.LoginDialogCaption = "COEReportDesigner";
            AccessController.Instance.LoginDialogLogo = System.Drawing.Bitmap.FromFile(Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf("\\")) + "\\Logo.png");
            if (!AccessController.Instance.Login())
                Application.Exit();

            if (!string.IsNullOrEmpty(_arguments["reportTemplateId"]))
                this.OpenReport(int.Parse(_arguments["reportTemplateId"]));
            else if (_arguments["createNewReport"] != null)
                this.CreateReportFlow(true);
            else if (_arguments["dataviewId"] != null)
                this.CreateReportFlow(int.Parse(_arguments["dataviewId"]));
        }
        #endregion

        #region Private Methods
        private bool CreateReportTemplate(int dataviewId)
        {
            return this.CreateReportTemplate(dataviewId, null);
        }

        private bool CreateReportTemplate(int dataviewId, ResultsCriteria resultsCriteria)
        {
            return CreateReportTemplate(dataviewId, resultsCriteria, 0);
        }
        
        private bool CreateReportTemplate(int dataviewId, ResultsCriteria resultsCriteria, COEReportType reportType)
        {
            return CreateReportTemplate(dataviewId, resultsCriteria, reportType, string.Empty);
        }
        private bool CreateReportTemplate(int dataviewId, ResultsCriteria resultsCriteria, COEReportType reportType, string category)
        {
            _reportTemplateBO.ReportDefinition.DataViewId = dataviewId;
            _reportTemplateBO.ReportDefinition = COEReporting.NormalizeReport(_reportTemplateBO.ReportDefinition);
            _reportTemplateBO.ReportDefinition.ResultsCriteria = resultsCriteria == null ? ExtractResultsCriteria(_reportTemplateBO.ReportDefinition.Dataview) : resultsCriteria;
            _reportTemplateBO.ReportType = reportType;
            _reportTemplateBO.Category = category;

            return true;
        }

        private bool LoadReportTemplate(int reportTemplateId)
        {
            _reportTemplateBO = COEReportBO.Get(reportTemplateId);
            _reportTemplateBO.ReportDefinition = COEReporting.NormalizeReport(_reportTemplateBO.ReportDefinition);

            ReportDesignerForm.ShowReport(_reportTemplateBO.Name, _reportTemplateBO.ReportDefinition.ReportLayout);
            ReportDesignerForm.Report.Category = _reportTemplateBO.Category;

            if (_reportTemplateBO.ReportDefinition.usesSearchManager)
                ReportDesignerForm.DisplayAvailableFields(CreateFieldListDataSource(_reportTemplateBO.ReportDefinition.ResultsCriteria, _reportTemplateBO.ReportDefinition.Dataview));

            

            return true;
        }

        private bool LoadReportTemplate(COEReport reportTemplate)
        {
            _reportTemplateBO = COEReportBO.New(Resources.NewReport, Resources.NewReport, reportTemplate);

            _reportTemplateBO.ReportDefinition = COEReporting.NormalizeReport(_reportTemplateBO.ReportDefinition);

            ReportDesignerForm.ShowReport(_reportTemplateBO.Name, _reportTemplateBO.ReportDefinition.ReportLayout);
            ReportDesignerForm.Report.Category = _reportTemplateBO.Category;

            if (_reportTemplateBO.ReportDefinition.usesSearchManager)
                ReportDesignerForm.DisplayAvailableFields(CreateFieldListDataSource(_reportTemplateBO.ReportDefinition.ResultsCriteria, _reportTemplateBO.ReportDefinition.Dataview));

            return true;
        }

        //TODO: Centralize Tables and relations "name in dataset" generation.
        private COEDescriptiveDataSet CreateFieldListDataSource(ResultsCriteria resultsCriteria, COEDataView dataview)
        {
            COEDescriptiveDataSet dataSource = new COEDescriptiveDataSet(Resources.AvailableReportFields);


            foreach (ResultsCriteria.ResultsCriteriaTable currentTable in resultsCriteria.Tables)
            {
                string tableName = dataview.Tables.getById(currentTable.Id).Alias;  //"Table_" + currentTable.Id;
                string displayName = dataview.Tables.getById(currentTable.Id).Alias;

                if (currentTable.Id == dataview.Basetable)
                    displayName += string.Format(" ({0})", Resources.BaseTable);

                if (!dataSource.Tables.Contains(tableName))
                {
                    dataSource.Tables.Add();
                    dataSource.Tables[dataSource.Tables.Count - 1].TableName = tableName;
                    dataSource.SetObjectDisplayName(tableName, displayName);
                }

                foreach (ResultsCriteria.IResultsCriteriaBase currentCriterium in currentTable.Criterias)
                {
                    if (!dataSource.Tables[dataSource.Tables.Count - 1].Columns.Contains(currentCriterium.Alias))
                        dataSource.Tables[dataSource.Tables.Count - 1].Columns.Add(currentCriterium.Alias);
                }
            }

            foreach (COEDataView.Relationship currentRelationship in dataview.Relationships)
            {
                string parentTableName = dataview.Tables.getById(currentRelationship.Parent).Alias; //"Table_" + currentRelationship.Parent;
                string childTableName = dataview.Tables.getById(currentRelationship.Child).Alias;  //"Table_" + currentRelationship.Child;

                if (dataSource.Tables.Contains(parentTableName) && dataSource.Tables.Contains(childTableName))
                {
                    System.Data.DataColumn parentColumn = dataSource.Tables[parentTableName].Columns[dataview.Tables.getById(currentRelationship.Parent).Fields.getById(currentRelationship.ParentKey).Alias];
                    System.Data.DataColumn childColumn = dataSource.Tables[childTableName].Columns[dataview.Tables.getById(currentRelationship.Child).Fields.getById(currentRelationship.ChildKey).Alias];

                    //Display names can not have dots nor spaces.
                    string relationFriendlyName = string.Format("{1}({2})->{3}({4})",
                                                                dataSource.Relations.Count,
                                                                dataview.Tables.getById(currentRelationship.Parent).Alias,
                                                                parentColumn.ColumnName,
                                                                dataview.Tables.getById(currentRelationship.Child).Alias,
                                                                childColumn.ColumnName);

                    //TODO: Find a way for troubleshooting DetailReportBands not having the table name on it's datamember (this way it gets cleared when binding the datasource to the report)
                    //string relationName = string.Format("Table_{0}->Table_{1}", currentRelationship.Parent, currentRelationship.Child);

                    System.Data.DataRelation currentDataRelation = new System.Data.DataRelation(relationFriendlyName,
                                                                                                parentColumn,
                                                                                                childColumn);

                    dataSource.Relations.Add(currentDataRelation);
                    dataSource.SetObjectDisplayName(relationFriendlyName, relationFriendlyName);
                }
            }

            return dataSource;
        }

        private ResultsCriteria ExtractResultsCriteria(COEDataView dataview)
        {
            CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView metaDataDataView = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView(dataview.ToString());

            ResultsCriteria resultsCriteria = new ResultsCriteria();

            foreach (COEDataView.DataViewTable currentTable in dataview.Tables)
            {
                if (currentTable.Id == dataview.Basetable || metaDataDataView.GetRelations(dataview.Basetable, currentTable.Id).Count == 1)
                {
                    ResultsCriteria.ResultsCriteriaTable resultsCriteriaTable = new ResultsCriteria.ResultsCriteriaTable(currentTable.Id);

                    foreach (COEDataView.Field currentField in currentTable.Fields)
                    {
                        ResultsCriteria.Field resultsCriteriaField = new ResultsCriteria.Field(currentField.Id);
                        resultsCriteriaField.Alias = currentField.Alias;

                        resultsCriteriaTable.Criterias.Add(resultsCriteriaField);

                        if (currentField.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE)
                        {
                            ResultsCriteria.Formula resultsCriteriaFormula = new ResultsCriteria.Formula();
                            resultsCriteriaFormula.Id = currentField.Id;
                            resultsCriteriaFormula.Alias = currentField.Alias + "_FORMULA";
                            resultsCriteriaTable.Criterias.Add(resultsCriteriaFormula);

                            ResultsCriteria.MolWeight resultsCriteriaMolweight = new ResultsCriteria.MolWeight();
                            resultsCriteriaMolweight.Id = currentField.Id;
                            resultsCriteriaMolweight.Alias = currentField.Alias + "_MOLWEIGHT";
                            resultsCriteriaTable.Criterias.Add(resultsCriteriaMolweight);
                        }
                    }
                    resultsCriteria.Tables.Add(resultsCriteriaTable);
                }
            }
            return resultsCriteria;
        }

        private void HandleException(Exception exception)
        {
            this.ShowErrorMessage(exception.Message);
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message);
        }
        #endregion
        #endregion 
    }

    public class CustomLabelWizardRunner : XRWizardRunnerBase
    {
        public CustomLabelWizardRunner(COEXtraReport report) : base(report)
        {
            this.Wizard = new DevExpress.XtraReports.Design.XtraReportLabelBuilderWizard(report);
        }

        public DialogResult Run()
        {
            return RunForm(
                new DevExpress.Utils.WizardPage []
                {
                    new WizPageLabelType(this),
                    new WizPageLabelOptions(this),
                }
                );
        }
    }
}

