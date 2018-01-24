using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COESearchService;

namespace CambridgeSoft.COE.Framework.COEExportService
{
    interface IFormInterchangeFormatter
    {
        string GetForm(COEDataView dataView, SearchCriteria searchCriteria, ResultsCriteria resultCriteria, PagingInfo pageInfo, ServerInfo serverInfo, string userName, string formName);               
    }
    
}
