using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEDataViewService;

namespace SpotfireIntegration.SpotfireAddin
{
    public class COEResultsCriteriaPromptModel
    {
        internal COEDataViewBOList DataViews { get; set; }
        internal COEDataViewBO DataViewBO { get; set; }
        internal ResultsCriteria ResultsCriteria { get; set; }
        internal SearchCriteria SearchCriteria { get; set; }
    }
}
