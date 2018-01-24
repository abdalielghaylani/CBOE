using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;
using Csla;
using CambridgeSoft.COE.Framework.COESearchCriteriaService;
using CambridgeSoft.COE.Framework;

namespace ChemBioViz.Services.BLL
{
    [Serializable]
    public class SearchCriteriaBuilderBO : BusinessBase<SearchCriteriaBuilderBO>
    {
        #region Attributes
        private COESearchCriteriaBO coeSearchCriteriaBO;
        #endregion

        #region Properties
        public List<SearchCriteria.SearchExpression> Items
        {
            get {
                return this.coeSearchCriteriaBO.SearchCriteria.Items;
            }
        }
        #endregion

        #region BusinessMethods
        protected override object GetIdValue()
        {
            return coeSearchCriteriaBO.ID;
        }

        public SearchCriteria BuildSearchCriteria()
        {
            return this.coeSearchCriteriaBO.SearchCriteria;
        }

        public static bool CanGetObject()
        {
            // return Csla.ApplicationContext.User.IsInRole("CanSearch");
            return true;
        }
        #endregion

        #region Factory Methods
        /*public static COESearchCriteriaBO New(string databaseName)
        {
            SetDatabaseName(databaseName);
            if (!CanAddObject())
                throw new System.Security.SecurityException(Resources.UserNotAuthorizedForAddObject + " COESearchCriteriaBO");
            return DataPortal.Create<COESearchCriteriaBO>(new CreateNewCriteria());
        }*/

        public static SearchCriteriaBuilderBO Get(string databaseName, int id)
        {
            if (!CanGetObject())
                throw new System.Security.SecurityException("User not authorized to view a SearchCriteriaBuilderBO");

            return DataPortal.Fetch<SearchCriteriaBuilderBO>(new Criteria(id, databaseName));
        }
        #endregion

        #region DataAccess
        private void DataPortal_Fetch(Criteria criteria)
        {
            try
            {
                this.coeSearchCriteriaBO = COESearchCriteriaBO.Get(SearchCriteriaType.TEMP, criteria._id);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Criteria
        [Serializable()]
        private class Criteria
        {

            internal int _id;
            internal string _databaseName;

            //constructors
            public Criteria(int id)
            {
                _id = id;
            }

            public Criteria(int id, string databaseName) : this(id)
            {
                _databaseName = databaseName;
            }

        }
        #endregion //Criteria
    }
}
