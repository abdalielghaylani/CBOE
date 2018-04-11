using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;
using System.Data;

namespace CambridgeSoft.COE.ChemBioViz.Services.COEChemBioVizService
{
    public class DAL : DALBase {
        internal virtual DataSet GetDataSet(string databaseName, COEDataView dataView, SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo) {
            //this has to be overridden so throw an error if there is not override
            return null;
        }

        internal virtual void InsertRecord(DataSet dataset) {
            //this has to be overridden so throw an error if there is not override
            return;
        }

        internal virtual void UpdateRecord(DataSet dataset) {
            //this has to be overridden so throw an error if there is not override
            return;
        }
    }
}
