using CambridgeSoft.COE.Framework.COEReportingService.Builders;
using CambridgeSoft.COE.Framework.Common.Messaging;
using DevExpress.XtraReports.UI;

namespace CambridgeSoft.COE.Framework.ServerControls.Reporting
{
    interface IReportViewer
    {
        #region Properties
        /// <summary>
        /// Show or hide the toolbar.
        /// </summary>
        bool ShowToolbar
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies a report builder to be used for obtaining the report instance to be displayed. This property is mutually exclusive with ReportBuidlerMeta property and ReportId properties.
        /// </summary>
        ReportBuilderBase ReportBuilder
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies information about a report builder to be used for obtaining the report instance to be displayed. This property is mutually exclusive with ReportBuilder and ReportId properties.
        /// </summary>
        ReportBuilderMeta ReportBuilderMeta
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies the id of the report (definition) to be used for generating a report instance to be displayed. This property is mutually exclusive with ReportBuilder and ReportBuilderMeta properties.
        /// </summary>
        int ReportId
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies the id of a hitlist representing the data that is meant to  be bound to the report instance to be displayed. This involves interaction with search service. This property is mutually exclusive with DataSource property
        /// </summary>
        int HitlistId
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies the datasource to be bound to the report instance to be displayed. This propety is mutually exclusive with HitlistId property
        /// </summary>
        object DataSource
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Instructs the IReportViewer to show the current report instance, and adapt its presentation to readOnly or editable reports.
        /// </summary>
        void ShowReport(XtraReport report, bool readOnly);
        #endregion
    }
}
