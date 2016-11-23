using System;
using FormWizard;

namespace SpotfireIntegration.SpotfireAddin
{
    public class COEResultsCriteriaPromptView : SelectDataForm
    {
        private COEResultsCriteriaPromptModel promptModel;

        public COEResultsCriteriaPromptView(COEResultsCriteriaPromptModel promptModel)
            : base()
        {
            this.availableDataViews = promptModel.DataViews;
            this.promptModel = promptModel;
            this.Finish += new FinishHandler(COEResultsCriteriaPromptView_Finish);
        }

        void COEResultsCriteriaPromptView_Finish(object sender, EventArgs e)
        {
            this.promptModel.DataViewBO = this.dataViewBO;
            this.promptModel.ResultsCriteria = this.resultsCriteria;
            this.promptModel.SearchCriteria = this.SearchCriteria;
        }
    }
}
