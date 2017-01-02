using CambridgeSoft.COE.Framework.COEDataViewService;

namespace FormWizard
{
    public interface ITableFilter
    {
        bool IncludeTable(SelectDataForm form, TableBO tableBO);
        string TableDisplayName(SelectDataForm form, TableBO tableBO);
    }
}
