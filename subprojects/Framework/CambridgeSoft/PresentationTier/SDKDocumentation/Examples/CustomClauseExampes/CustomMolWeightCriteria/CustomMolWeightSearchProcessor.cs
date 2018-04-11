using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COESearchService.Processors;

namespace CustomMolWeightCriteria {
    public class CustomMolWeightSearchProcessor : BaseCustomProcessor {
        public override void PreProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL) {
            // This method gives access to the db prior to executing the search.
            System.Diagnostics.Debug.WriteLine("Executed CustomMolWeightSearchProcessor PreProcess");
        }
        public override void Process(CambridgeSoft.COE.Framework.Common.SearchCriteria.SearchCriteriaItem item) {
            // This method adds a chance to modify the search criteria item just before the execution.
            System.Diagnostics.Debug.WriteLine("Executed CustomMolWeightSearchProcessor Process");
        }
        public override void PostProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL) {
            // This method gives access to the db after to executing the search.
            System.Diagnostics.Debug.WriteLine("Executed CustomMolWeightSearchProcessor PostProcess");
        }
    }
}
