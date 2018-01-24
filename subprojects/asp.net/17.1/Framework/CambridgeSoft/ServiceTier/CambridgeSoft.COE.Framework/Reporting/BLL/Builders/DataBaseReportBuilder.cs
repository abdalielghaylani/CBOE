using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEDataViewService;

namespace CambridgeSoft.COE.Framework.COEReportingService.Builders
{
    /// <summary>
    /// Retrieves reports from database
    /// </summary>
    public class DataBaseReportBuilder : ReportBuilderBase
    {
        #region variables
        private int _reportId;
        #endregion

        #region Properties
        public int ReportId
        {
            get {
                return _reportId;
            }
            set {
                _reportId = value;
            }
        }
        #endregion

        #region Methods
        #region Constructors
        internal DataBaseReportBuilder(int reportId)
        {
            _reportId = reportId;
        }
        #endregion

        #region internal Methods
        public override COEReport GetReport()
        {
            COEReport result = COEReporting.NormalizeReport(COEReportBO.Get(this.ReportId).ReportDefinition);

            return result;
        }
        #endregion
        #endregion
    }
}
