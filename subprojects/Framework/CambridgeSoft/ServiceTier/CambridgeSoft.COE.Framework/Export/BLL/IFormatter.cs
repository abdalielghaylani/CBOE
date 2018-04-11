using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.COEExportService
{
    interface IFormatter
    {

        //String FormatDataSet(DataSet dataSet); 
        /// <summary>
        /// modify by david zhang for export services,because need relationship between parent table and child table
        /// in the dataset , so add two parameter 'COEDataView dataView,ResultsCriteria resultCriteria'
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="dataView"></param>
        /// <param name="resultCriteria"></param>
        /// <returns></returns>
        String FormatDataSet(DataSet dataSet, COEDataView dataView, ResultsCriteria resultCriteria);        
        
    }
    //
    interface IFormatterAdvanced
    {
        string FormatData(DataSet dataSet, COEDataView dataView, SearchCriteria searchCriteriaItem, ResultsCriteria resultCriteria, PagingInfo pageInfo, string userName, string SARSheetName);

    }
    //
}
