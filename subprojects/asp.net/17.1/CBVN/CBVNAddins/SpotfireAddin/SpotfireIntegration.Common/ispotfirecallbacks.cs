using System.Collections.Generic;
using System.ServiceModel;

namespace SpotfireIntegration.Common
{
    [ServiceContract]
    public interface ISpotfireCallbacks
    {
        [OperationContract(IsOneWay = true)]
        void SelectRows(int dataViewID, int hitListID, List<int> rowIndexes);

        [OperationContract(IsOneWay = true)]
        void ResultsCriteriaChanged(string newResultsCriteriaXml);

        [OperationContract(IsOneWay = true)]
        void SpotfireDocumentChanged(COEHitList newHitList);

        [OperationContract(IsOneWay = true)]
        void SpotfireExiting();
    }
}
