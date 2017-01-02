using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.COEReportingService.Builders
{
    /// <summary>
    /// Base class for all Report builders. Gives support to templates (system generated reports).
    /// </summary>
    public abstract class ReportBuilderBase
    {


        #region Methods
        /// <summary>
        /// Builds (who knows how) and returns a report (definition)
        /// </summary>
        /// <returns>A report (definition)</returns>
        public abstract COEReport GetReport();
        #endregion
    }
}
