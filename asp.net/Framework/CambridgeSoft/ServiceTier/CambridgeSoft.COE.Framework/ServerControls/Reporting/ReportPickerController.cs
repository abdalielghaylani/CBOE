using System.Collections.Generic;
using CambridgeSoft.COE.Framework.COEReportingService;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.ServerControls.Reporting
{
    internal class ReportPickerController
    {
        #region Variables
        private IReportPicker _reportPicker;
        private int _dataviewId = -1;
        private COEReportType? _reportType;
        private List<ReportBuilderMeta> _reportBuilders;
        private bool _showPrivateReports;
        private string _applicationName = null;
        private string _category = null;
        private string _userName = null;
        #endregion

        #region Properties
        /// <summary>
        /// List containing information of available report builders. This list is meant to be presented for the user to pick one and view the generated report instance.
        /// </summary>
        public List<ReportBuilderMeta> ReportBuilders
        {
            get {
                if (_reportBuilders == null)
                    _reportBuilders = RetrieveReportBuildersMeta();

                return _reportBuilders;
            }
        }

        /// <summary>
        /// Allows filtering of available report builders by dataviewId. Ignored if null.
        /// </summary>
        public int DataViewId
        {
            get
            {
                return _dataviewId;
            }
            set 
            {
                if (_dataviewId != value)
                {
                    _dataviewId = value;
                    RefreshReportBuilders();
                }
            }
        }
        
        /// <summary>
        /// Allows filtering of available report builders by type of report. Ignored if null.
        /// </summary>
        public COEReportType? ReportType
        {
            get
            {
                return _reportType;
            }
            set
            {
                if (_reportType != value)
                {
                    _reportType = value;
                    RefreshReportBuilders();
                }
            }
        }

        /// <summary>
        /// Allows filtering of available report builders by public attribute
        /// </summary>
        public bool ShowPrivateReports
        {
            get 
            {
                return _showPrivateReports;
            }
            set 
            {
                if (!_showPrivateReports.Equals(value))
                {
                    _showPrivateReports = value;
                    RefreshReportBuilders();
                }
            }
        }

        /// <summary>
        /// Allows overriding of current applicationName
        /// </summary>
        public string ApplicationName
        {
            get 
            {
                return _applicationName;
            }
            set 
            {
                if (_applicationName == null || !_applicationName.Equals(value))
                {
                    _applicationName = value;
                    RefreshReportBuilders();
                }
            }
        }

        /// <summary>
        /// Allows filtering of available report builders by Category. Ignored if null or empty.
        /// </summary>
        public string Category
        {
            get 
            {
                return _category;
            }
            set 
            {
                if (_category == null || !_category.Equals(value))
                {
                    _category = value;
                    RefreshReportBuilders();
                }
            }
        }

        public string UserName
        {
            get 
            {
                return _userName;
            }
            set 
            {
                if (_userName == null || !_userName.Equals(value))
                {
                    _userName = value;
                    RefreshReportBuilders();
                }
            }
        }
        #endregion

        #region Methods
        #region Constructors
        public ReportPickerController(IReportPicker reportPicker)
        {
            _reportPicker = reportPicker;
            _userName = COEUser.Get();
        }
        #endregion

        #region public Methods
        public ReportBuilderMeta GetReportBuilder(int id)
        {
            foreach (ReportBuilderMeta currentReportBuilderMeta in this.ReportBuilders)
                if (currentReportBuilderMeta.Id == id)
                    return currentReportBuilderMeta;

            return null;
        }
        #endregion

        #region private Methods
        /// <summary>
        /// Interacts with Reporting service for retrieving the information about available report builders, both from database and configured on xml. Also performs some embellishment for displaying onscreen.
        /// </summary>
        /// <returns></returns>
        private List<ReportBuilderMeta> RetrieveReportBuildersMeta()
        {
            List<ReportBuilderMeta> reportBuilders = new List<ReportBuilderMeta>();

            List<ReportBuilderMeta> databaseReportBuilders = COEReporting.GetDatabaseReportBuilders(this.UserName, this.ApplicationName, DataViewId, ReportType, Category);

            if (databaseReportBuilders.Count > 0)
            {
                reportBuilders.Add(new ReportBuilderMeta(-1, "Available Reports", string.Empty, string.Empty));

                foreach (ReportBuilderMeta currentTemplateBuilder in databaseReportBuilders)
                    currentTemplateBuilder.Name = "     " + currentTemplateBuilder.Name;

                reportBuilders.AddRange(databaseReportBuilders);
            }

            List<ReportBuilderMeta> configuredReportBuilders = COEReporting.GetTemplateReportBuilders(this.UserName, this.ApplicationName, DataViewId, ReportType, Category);
            if (configuredReportBuilders.Count > 0)
            {
                int currentId = reportBuilders.Count;
                reportBuilders.Add(new ReportBuilderMeta(-1, "Available Templates", string.Empty, string.Empty));

                foreach (ReportBuilderMeta currentTemplateBuilder in configuredReportBuilders)
                {
                    currentTemplateBuilder.Id = currentId++;
                    currentTemplateBuilder.Name = "     " + currentTemplateBuilder.Name;
                }

                reportBuilders.AddRange(configuredReportBuilders);
            }

            return reportBuilders;
        }
        #endregion 

        #region internal Methods
        /// <summary>
        /// Deletes (if not a read-only report) the selected report (definition)
        /// </summary>
        /// <param name="templateBuilderInfo"></param>
        internal void DeleteReport(ReportBuilderMeta reportBuilderMeta)
        {
            if (reportBuilderMeta != null && !reportBuilderMeta.ReadOnly && reportBuilderMeta.Id > 0)
                COEReporting.DeleteReport(reportBuilderMeta.Id);

            RefreshReportBuilders();
        }

        /// <summary>
        /// Refreshes the list with information of available report builders, after any filter propert(y|ies) ha(s|ve) changed.
        /// </summary>
        internal void RefreshReportBuilders()
        {
            _reportBuilders = null;
        }
        #endregion
        #endregion
    }
}
