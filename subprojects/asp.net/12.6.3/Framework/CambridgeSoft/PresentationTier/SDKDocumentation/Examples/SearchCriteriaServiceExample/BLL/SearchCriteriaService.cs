using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COESearchCriteriaService;
using CambridgeSoft.COE.Framework;


namespace SearchCriteriaServiceExample.BLL
{
    public class SearchCriteriaService
    {

        #region Members

        private COESearchCriteriaBOList _recentSearchCriteriaBOList;
        private COESearchCriteriaBOList _savedSearchCriteriaBOList;

        private string _database;
        private string _userId;
        private int _dataViewId;
        private int _formGroup;

        #endregion

        #region Properties

        public COESearchCriteriaBOList RecentSearchCriteriaList
        {
            get 
            {
                return _recentSearchCriteriaBOList;
            }
        }

        public COESearchCriteriaBOList SavedSearchCriteriaList
        {
            get 
            {
                return _savedSearchCriteriaBOList;
            }
        }

        #endregion

        #region Constructors

        public SearchCriteriaService(string userId, string database, int dataViewId, int formGroup) 
        {
            this._userId = userId;
            this._database = database;
            this._dataViewId = dataViewId;
            this._formGroup = formGroup;

            GetRecentSearchCriteriaList();
            GetSavedSearchCriteriaList();
        }

        #endregion

        #region Methods

        private void GetRecentSearchCriteriaList() 
        {
            _recentSearchCriteriaBOList = COESearchCriteriaBOList.GetRecentSearchCriteria();
        }

        private void GetSavedSearchCriteriaList() 
        {
            _savedSearchCriteriaBOList = COESearchCriteriaBOList.GetSavedSearchCriteria(this._userId, this._dataViewId, this._database);
        }

        public void GetSomeRecentSearchCriteria(int quantity, bool byFormGroup)
        {
            if (byFormGroup)
            {
                this._recentSearchCriteriaBOList = COESearchCriteriaBOList.GetRecentSearchCriteria(_userId, _dataViewId, _formGroup, _database, quantity);
            }
            else 
            {
                this._recentSearchCriteriaBOList = COESearchCriteriaBOList.GetRecentSearchCriteria(_userId, _dataViewId,_database, quantity);
            }
        }

        public void GetSavedSearchCriteriaByFormGroup() 
        {
            _savedSearchCriteriaBOList = COESearchCriteriaBOList.GetSavedSearchCriteria(_userId, _dataViewId, _formGroup, _database);
            
        }

        public COESearchCriteriaBO GetSpecificSearhCriteria(SearchCriteriaType searchCriteriaType, int id) 
        {
            return COESearchCriteriaBO.Get(searchCriteriaType,id);
        }  
   


        #endregion


    }
}
