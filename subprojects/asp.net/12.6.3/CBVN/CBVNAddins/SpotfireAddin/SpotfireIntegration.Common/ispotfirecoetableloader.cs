using System.Collections.Generic;
using System.ServiceModel;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;

namespace SpotfireIntegration.Common
{
    [ServiceContract(SessionMode=SessionMode.Required, CallbackContract=typeof(ISpotfireCallbacks))]
    public interface ISpotfireCOETableLoader
    {
        #region Public Methods

        [OperationContract(IsOneWay = true)]
        void Subscribe();

        [OperationContract]
        [FaultContract(typeof(TableLoadFault))]
        void LoadTablesFromCOE(COEHitList hitList, string baseTableName, string formName, 
string authenticationTicket, string cslaDataPortalUrl, string cslaDataPortalProxy, string dataSource,
bool bForceReload, int maxRows, bool filterChildHits, string serverName, string userName);

        [OperationContract]
        [FaultContract(typeof(TableLoadFault))]
        //CSBR:151920 formName parameter is added to LoadTablesFromFile to set Document Title.
        string LoadTablesFromFile(COEHitList hitList, string baseTableName, string formName,
string filename, string authenticationTicket, string cslaDataPortalUrl, string cslaDataPortalProxy,
string dataSource, int maxRows, string serverName, string userName);

        [OperationContract(IsOneWay = true)]
        void CloseCOEDocument(int dataViewID, int hitListID);

        [OperationContract(IsOneWay = true)]
        void CBVNRecordChanged(int dataViewID, int hitListID, List<int> rowIndexes);

        [OperationContract]
        [FaultContract(typeof(TableLoadFault))]
        void CBVNRecordsetChanged(int hitListID, HitListType hitListType, int numHits);

        [OperationContract]
        [FaultContract(typeof(TableLoadFault))]
        void CBVNResultsCriteriaChanged(string rcNew);

        [OperationContract]
        COEHitList GetCOEHitList();

        /// <summary>
        /// Retrieves a string describing the origin of the currently open analysis.
        /// </summary>
        /// <returns>A string describing the origin of the currently open analysis, or null if no analysis is open</returns>
        [OperationContract]
        string GetAnalysisOrigin();

        [OperationContract]
        void FindForm();

        [OperationContract]
        void Ping();

        #endregion
    }
}
