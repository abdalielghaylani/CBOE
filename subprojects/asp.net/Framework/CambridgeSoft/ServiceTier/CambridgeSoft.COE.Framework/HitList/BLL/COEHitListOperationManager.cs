using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework.Common;
using Csla;
using Csla.Data;

namespace CambridgeSoft.COE.Framework.COEHitListService
{
    //<summary>
    //Class for performing all hitlist operations
    //</summary>
    public class COEHitListOperationManager
    {

        #region Public Methods

        public static COEHitListBO IntersectHitList(HitListInfo hitList1, HitListInfo hitList2)
        {
            return IntersectHitList(hitList1, hitList2, 0);
        }

        public static COEHitListBO IntersectHitList(HitListInfo hitList1, HitListInfo hitList2, int dataviewID) {
            IntersectHitListsCommand result;

            result = DataPortal.Execute<IntersectHitListsCommand>(new IntersectHitListsCommand(hitList1, hitList2, dataviewID));
            return result.HitListBO;

        }

        public static COEHitListBO SubtractHitLists(HitListInfo hitList1, HitListInfo hitList2) {
            return SubtractHitLists(hitList1, hitList2, 0);
        }

        public static COEHitListBO SubtractHitLists(HitListInfo hitList1, HitListInfo hitList2, int dataviewID) {
            SubtractHitListsCommand result;

            result = DataPortal.Execute<SubtractHitListsCommand>(new SubtractHitListsCommand(hitList1, hitList2, dataviewID));
            return result.HitListBO;
        }

        public static COEHitListBO UnionHitLists(HitListInfo hitList1, HitListInfo hitList2) {
            return UnionHitLists(hitList1, hitList2, 0);
        }

        public static COEHitListBO UnionHitLists(HitListInfo hitList1, HitListInfo hitList2, int dataviewID) {
            UnionHitListsCommand result;

            result = DataPortal.Execute<UnionHitListsCommand>(new UnionHitListsCommand(hitList1, hitList2, dataviewID));
            return result.HitListBO;
        }


        public static COEHitListBO SubtractHitLists(HitListInfo hitListInfo, int[] idsToExclude) {
            //for now we will take the database name from the first hitList at some later time we may enhance this.
            try {

                //Create a new hitlist ids to  exclude

                COEHitListBO excludeHitsHitList = null;

                excludeHitsHitList = COEHitListBO.New(hitListInfo.Database, HitListType.TEMP);
                excludeHitsHitList.HitIds = idsToExclude;
                excludeHitsHitList.Description = "TEMP";
                excludeHitsHitList = excludeHitsHitList.Update();

                //retrieve id from object for substract hitlist step 
                COEHitListBO newTempHitList = SubtractHitLists(hitListInfo, excludeHitsHitList.HitListInfo);

                return newTempHitList;
            } catch(Exception ex) {
                throw;
            }
        }

        public static COEHitListBO SubtractHitLists(HitListInfo hitListInfo, int[] idsToExclude, int dataViewID) {
            //for now we will take the database name from the first hitList at some later time we may enhance this.
            try {

                //Create a new hitlist ids to  exclude
                COEHitListBO excludeHitsHitList = null;

                excludeHitsHitList = COEHitListBO.New(hitListInfo.Database, HitListType.TEMP);
                excludeHitsHitList.DataViewID = dataViewID;
                excludeHitsHitList.HitIds = idsToExclude;
                excludeHitsHitList.Description = "TEMP";
                excludeHitsHitList = excludeHitsHitList.Update();

                //retrieve id from object for substract hitlist step 
                COEHitListBO newTempHitList = SubtractHitLists(hitListInfo, excludeHitsHitList.HitListInfo, dataViewID);

                return newTempHitList;
            } catch(Exception ex) {
                throw;
            }
        }
 
        #endregion

        #region Commands

        [Serializable]
        private abstract class OperationHitListCommand : CommandBase
        {
            #region Variables

            protected HitListInfo _hitList1;
            protected HitListInfo _hitList2;
            protected COEHitListBO _hitListBO;
            protected int _dataViewID;
            protected int _newHitListTempID;
            [NonSerialized]
            protected DAL _coeDAL = null;
            [NonSerialized]
            protected DALFactory _dalFactory = new DALFactory();
            protected string _serviceName = "COEHitList";

            #endregion

            #region Properties

            public COEHitListBO HitListBO
            {
                get
                {
                    return _hitListBO;
                }
            }

            public int NewHitListTempID
            {
                get { return _newHitListTempID; }
                set { _newHitListTempID = value; }
            }

            public int DataViewID {
                get { return _dataViewID; }
            }
            #endregion

            #region Methods

            protected override void DataPortal_Execute()
            {
                GetDAL();

            }

            protected void GetDAL()
            {
                if (_dalFactory == null) { _dalFactory = new DALFactory(); }
                _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get(), true);
            }




            #endregion

        }

        [Serializable]
        private class IntersectHitListsCommand : OperationHitListCommand
        {


            public IntersectHitListsCommand(HitListInfo hitList1, HitListInfo hitList2, int dataviewID)
            {
                _hitList1 = hitList1;
                _hitList2 = hitList2;
                _dataViewID = dataviewID;
            }

            protected override void DataPortal_Execute()
            {
                try
                {
                    base.DataPortal_Execute();
                    //for now we will take the database name from the first hitList at some later time we may enhance this.
                    _newHitListTempID = _coeDAL.IntersectHitLists(_hitList1.HitListID, _hitList1.HitListType, _hitList2.HitListID, _hitList2.HitListType, _hitList1.Database, _dataViewID);
                    this._hitListBO = COEHitListBO.Get(HitListType.TEMP, _newHitListTempID);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [Serializable]
        private class SubtractHitListsCommand : OperationHitListCommand
        {
            public SubtractHitListsCommand(HitListInfo hitList1, HitListInfo hitList2, int dataviewID)
            {
                _hitList1 = hitList1;
                _hitList2 = hitList2;
                _dataViewID = dataviewID;
            }

            protected override void DataPortal_Execute()
            {
                try
                {
                    base.DataPortal_Execute();
                    //for now we will take the database name from the first hitList at some later time we may enhance this.
                    _newHitListTempID = _coeDAL.SubtractHitLists(_hitList1.HitListID, _hitList1.HitListType, _hitList2.HitListID, _hitList2.HitListType, _hitList1.Database, _dataViewID);
                    _hitListBO = COEHitListBO.Get(HitListType.TEMP, _newHitListTempID);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [Serializable]
        private class UnionHitListsCommand : OperationHitListCommand
        {
            public UnionHitListsCommand(HitListInfo hitList1, HitListInfo hitList2, int dataviewID)
            {
                _hitList1 = hitList1;
                _hitList2 = hitList2;
                _dataViewID = dataviewID;
            }

            protected override void DataPortal_Execute()
            {
                try
                {
                    base.DataPortal_Execute();
                    //for now we will take the database name from the first hitList at some later time we may enhance this.
                    _newHitListTempID = _coeDAL.UnionHitLists(_hitList1.HitListID, _hitList1.HitListType, _hitList2.HitListID, _hitList2.HitListType, _hitList1.Database, _dataViewID);
                    _hitListBO = COEHitListBO.Get(HitListType.TEMP, _newHitListTempID);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

    }
}
