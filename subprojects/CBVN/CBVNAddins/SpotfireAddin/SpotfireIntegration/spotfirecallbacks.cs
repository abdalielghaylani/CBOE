using System.Collections.Generic;
using System.Xml;
using CambridgeSoft.COE.Framework.Common;
using SpotfireIntegration.Common;

namespace SpotfireIntegration
{
    class SpotfireCallbacks : ISpotfireCallbacks
    {
        private SpotfireIntegration spotfireIntegration;

        internal SpotfireCallbacks(SpotfireIntegration spotfireIntegration)
        {
            this.spotfireIntegration = spotfireIntegration;
        }

        #region ISpotfireCallbacks Members

        public void SelectRows(int dataViewID, int hitListID, List<int> rowIndexes)
        {
            this.spotfireIntegration.SelectRows(dataViewID, hitListID, rowIndexes);
        }

        public void ResultsCriteriaChanged(string newResultsCriteriaXml)
        {
            XmlDocument resultsCriteriaDoc = new XmlDocument();
            resultsCriteriaDoc.LoadXml(newResultsCriteriaXml);
            ResultsCriteria newResultsCriteria = new ResultsCriteria(resultsCriteriaDoc);
            this.spotfireIntegration.ResultsCriteriaChanged(newResultsCriteria);
        }

        public void SpotfireDocumentChanged(COEHitList newHitList)
        {
            this.spotfireIntegration.SpotfireDocumentChanged(newHitList);
        }

        public void SpotfireExiting()
        {
            this.spotfireIntegration.SpotfireExiting();
        }

        #endregion
    }
}
