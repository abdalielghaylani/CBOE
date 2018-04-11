using System;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.COEReportingService;

namespace CambridgeSoft.COE.Framework.ServerControls.Reporting
{
    interface IReportPicker
    {
        #region Properties
        /// <summary>
        /// Returns information describing the report builder currently selected by the user.
        /// </summary>
        ReportBuilderMeta SelectedReportBuilderMeta
        {
            get;
        }

        /// <summary>
        /// Allows filtering of available report builders by dataviewId. Ignored if null.
        /// </summary>
        int DataViewId
        {
            get;
            set;
        }

        /// <summary>
        /// Allows filtering of available report builders by type of report. Ignored if null.
        /// </summary>
        COEReportType? ReportType
        {
            get;
            set;
        }

        /// <summary>
        /// Allows filtering of available report builders by Category. Ignored if null or empty.
        /// </summary>
        string Category
        {
            get;
            set;
        }

        // <summary>
        /// Allows filtering of available report builders by public attribute
        /// </summary>
        bool ShowPrivateReports
        {
            get;
            set;
        }

        /// <summary>
        /// allows overriding default application name.
        /// </summary>
        string ApplicationName
        {
            get;
            set;
        }

        /// <summary>
        /// allows overriding current Logged user.
        /// </summary>
        string UserName
        {
            get;
            set;
        }
        #endregion

        #region Events
        /// <summary>
        /// Fired upon selection of a report builder by the user.
        /// </summary>
        event EventHandler SelectedReportBuilderMetaChanged;
        #endregion
    }
}
