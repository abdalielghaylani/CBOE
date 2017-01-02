using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework;

namespace HitListServiceExamples
{
    class HitListService
    {
        #region Variables

        private COEHitListBOList _savedHitListBOList;
        private COEHitListBOList _tempHitListBOList;
        private const string DATABASENAME = "COEDB";

        #endregion

        #region Properties

        public COEHitListBOList SavedHitListList 
        {
            get 
            {
                return _savedHitListBOList;
            }
        }

        public COEHitListBOList TempHitListList 
        {
            get 
            {
                return _tempHitListBOList;
            }
        }

        #endregion

        #region Constructors

        public HitListService() 
        {
            Console.WriteLine("Geting HitListList...\n");
            //Geting SAVED HitLists
            _savedHitListBOList = COEHitListBOList.GetSavedHitListList(DATABASENAME);
            //Geting TEMP Hitlists
            _tempHitListBOList = COEHitListBOList.GetTempHitListList(DATABASENAME);
        }

        #endregion       

        #region Methods

        public COEHitListBO IntersecHitLists(HitListType hitListType, int hitListIndex1, int hitListIndex2) 
        {
            COEHitListBO result;
            
            switch(hitListType)
            {
                case HitListType.TEMP:
                    result = COEHitListOperationManager.IntersectHitList(_tempHitListBOList[hitListIndex1].HitListInfo,_tempHitListBOList[hitListIndex2].HitListInfo);
                    break;
                case HitListType.SAVED:
                    result = COEHitListOperationManager.IntersectHitList(_savedHitListBOList[hitListIndex1].HitListInfo,_savedHitListBOList[hitListIndex2].HitListInfo);
                    break;
                default:
                    throw new Exception("HitList Type is not correct");
            }
            return result;
        }

        public COEHitListBO SubtractHitLists(HitListType hitListType, int hitListIndex1, int hitListIndex2)
        {
            COEHitListBO result;

            switch (hitListType)
            {
                case HitListType.TEMP:
                    result = COEHitListOperationManager.SubtractHitLists(_tempHitListBOList[hitListIndex1].HitListInfo, _tempHitListBOList[hitListIndex2].HitListInfo);
                    break;
                case HitListType.SAVED:
                    result = COEHitListOperationManager.SubtractHitLists(_savedHitListBOList[hitListIndex1].HitListInfo, _savedHitListBOList[hitListIndex2].HitListInfo);
                    break;
                default:
                    throw new Exception("HitList Type is not correct");
            }
            return result;
        }

        public COEHitListBO SubtractHitLists(HitListType hitListType, int hitListIndex1, int[] idListToExclude) {
            COEHitListBO result;

            switch(hitListType) {
                case HitListType.TEMP:
                    result = COEHitListOperationManager.SubtractHitLists(_tempHitListBOList[hitListIndex1].HitListInfo, idListToExclude);
                    break;
                case HitListType.SAVED:
                    result = COEHitListOperationManager.SubtractHitLists(_savedHitListBOList[hitListIndex1].HitListInfo, idListToExclude);
                    break;
                default:
                    throw new Exception("HitList Type is not correct");
            }
            return result;
        }

        public COEHitListBO UniteHitLists(HitListType hitListType, int hitListIndex1, int hitListIndex2)
        {
            COEHitListBO result;

            switch (hitListType)
            {
                case HitListType.TEMP:
                    result = COEHitListOperationManager.UnionHitLists(_tempHitListBOList[hitListIndex1].HitListInfo, _tempHitListBOList[hitListIndex2].HitListInfo);
                    break;
                case HitListType.SAVED:
                    result = COEHitListOperationManager.UnionHitLists(_savedHitListBOList[hitListIndex1].HitListInfo, _savedHitListBOList[hitListIndex2].HitListInfo);
                    break;
                default:
                    throw new Exception("HitList Type is not correct");
                    
            }
            return result;
        }

        public COEHitListBO MarkHits(HitListType hitListType, int hitListIndex, int[] hitToBeMarked) 
        {
            COEHitListBO markedHitList;
            switch (hitListType) 
            {
                case HitListType.TEMP:
                    _tempHitListBOList[hitListIndex].AddMarkedHits(hitToBeMarked);
                    markedHitList = _tempHitListBOList[hitListIndex].MarkHitList();                    
                    break;
                case HitListType.SAVED:
                    _savedHitListBOList[hitListIndex].AddMarkedHits(hitToBeMarked);
                    markedHitList = _savedHitListBOList[hitListIndex].MarkHitList();                    
                    break;
                default:
                    throw new Exception("HitList Type is not correct");
            }
            return markedHitList;
        }

        #endregion
    }
}
