using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.COEDisplayDataBrokerService;

using Csla;
using Csla.Data;
using Csla.Validation;

using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// Class retrieves
    /// </summary>
    [Serializable()]
    public class FragmentList :
        RegistrationBusinessListBase<FragmentList, Fragment>, IKeyValueListHolder
    {
        #region >Private members<

        private const string FRAGMENTLIST_CACHEKEY = "AllFragments";
        private const string FRAGMENTLIST_MANAGERNAME = "Default Cache Manager";
        private const CacheItemPriority CACHE_PRIORITY = CacheItemPriority.Normal;
        private static SlidingTime CacheExpiry = new SlidingTime(TimeSpan.FromSeconds(60));

        /// <summary>
        /// Data-binding helper.
        /// </summary>
        [NonSerialized]
        private FilteredBindingList<Fragment> _filteredList;

        #endregion

        /// <summary>
        /// Determines if any of the list members are 'dirty', indicating an edit.
        /// </summary>
        private bool HasDirtyChildren
        {
            get
            {
                bool retVal = false;
                foreach (Fragment child in this)
                {
                    if (child.IsDirty)
                    {
                        retVal = true;
                        break;
                    }
                }
                return retVal;
            }
        }

        public void ClearDeletedList()
        {
            this.DeletedList.Clear();
        }


        /// <summary>
        /// Get the deleted list count of fragments.
        /// </summary>
        public List<Fragment> GetDeletedList()
        {
            return this.DeletedList;
        }

        /// <summary>
        /// Core Factory instantiator.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="isNew"></param>
        /// <param name="isClean"></param>
        /// <returns></returns>
        /// 
        [COEUserActionDescription("CreateFragmentList")]
        public static FragmentList NewFragmentList(string xml, bool isNew, bool isClean)
        {
            try
            {
                FragmentList fragmentList = new FragmentList(xml, isNew, isClean);
                return fragmentList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// Private constructor for the core Facory method.
        /// </summary>
        /// <see cref="NewFragmentList"/>
        /// <param name="xml"></param>
        /// <param name="isNew"></param>
        /// <param name="isClean"></param>
        private FragmentList(string xml, bool isNew, bool isClean)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("FragmentList/Fragment");
            if (xIterator.MoveNext())
            {
                do
                {
                    this.Add(Fragment.NewFragment(xIterator.Current.OuterXml, isNew, isClean));
                } while (xIterator.Current.MoveToNext());
            }
        }

        /// <summary>
        /// For use by other members of this assembly, specifically to create an empty
        /// list for a component.
        /// </summary>
        /// <returns></returns>
        internal static FragmentList NewFragmentList()
        {
            return new FragmentList();
        }

        /// <summary>
        /// Private constructor.
        /// </summary>
        private FragmentList()
        {
            //MarkAsChild();
        }

        #region [ Fetch - No criteria ]

        /// <summary>
        /// Unfiltered retrieval of the entire fragments list from the repository.
        /// </summary>
        /// <returns></returns>
        [COEUserActionDescription("GetFragmentList")]
        public static FragmentList GetFragmentList()
        {
            try
            {
                return DataPortal.Fetch<FragmentList>(new Criteria());
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// Unfiltered retrieval of the repository's fragment list, optionally using Enterprise Library
        /// caching when available.
        /// </summary>
        /// <param name="useCache">use false to explicitly force a refresh from the repository</param>
        /// <returns>a FragmentList instance with all repository fragments</returns>
        [COEUserActionDescription("GetFragmentList")]
        public static FragmentList GetFragmentList(bool useCache)
        {
            FragmentList fragments = null;

            try
            {
                if (useCache)
                {
                    CacheManager fragmentManager = CacheFactory.GetCacheManager(FRAGMENTLIST_MANAGERNAME);
                    fragments = (FragmentList)fragmentManager.GetData(FRAGMENTLIST_CACHEKEY);
                    if (fragments == null)
                    {
                        fragments = FragmentList.GetFragmentList();
                        fragmentManager.Add(FRAGMENTLIST_CACHEKEY, fragments, CACHE_PRIORITY, null, CacheExpiry);
                    }
                }
                else
                    fragments = FragmentList.GetFragmentList();
            }
            catch (System.Configuration.ConfigurationErrorsException)
            {
                // caching is not configured by the hosting application
                fragments = FragmentList.GetFragmentList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
            return fragments;
        }

        /// <summary>
        /// DataPortal retrieval instructions.
        /// </summary>
        [Serializable()]
        protected class Criteria
        {
            /// <summary>
            /// The null-criterion instruction to retrieve all records.
            /// </summary>
            public Criteria() { }
        }

        /// <summary>
        /// The core data-retrieval function, called internally by CSLA.
        /// </summary>
        /// <param name="criteria"></param>
        protected void DataPortal_Fetch(Criteria criteria)
        {
            using (SafeDataReader sdr = this.RegDal.GetFragmentsList())
            {
                this.Fill(sdr);
            }
        }

        #endregion

        #region [ Fetch - Structure-match ]

        /// <summary>
        /// Filtered retrieval of matching fragments from the repository.
        /// </summary>
        /// <param name="structureToMatch">the fragment structure used to filter the result-set</param>
        /// <returns>a FragmentList collection</returns>
        [COEUserActionDescription("GetFragmentList")]
        public static FragmentList GetFragmentList(string structureToMatch)
        {
            try
            {
                return DataPortal.Fetch<FragmentList>(new StructureMatchCriteria(structureToMatch));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// DataPortal retrieval instructions.
        /// </summary>
        [Serializable()]
        protected class StructureMatchCriteria
        {
            private string _matchableStructure = string.Empty;
            /// <summary>
            /// Assign this via the constructor to propose a filtered match based on structure.
            /// </summary>
            public string MatchableStructure
            {
                get { return _matchableStructure; }
            }

            /// <summary>
            /// The structure-matching criterion to retrieve only records whose structure matches.
            /// </summary>
            public StructureMatchCriteria(string structureToMatch)
            {
                this._matchableStructure = structureToMatch;
            }
        }

        /// <summary>
        /// The core data-retrieval function, called internally by CSLA.
        /// </summary>
        /// <param name="criteria"></param>
        protected void DataPortal_Fetch(StructureMatchCriteria criteria)
        {
            using (SafeDataReader sdr =
                this.RegDal.GetMatchedFragmentsList(criteria.MatchableStructure))
            {
                this.Fill(sdr);
            }
        }

        #endregion

        #region [ Fetch - Fragment IDs ]

        /// <summary>
        /// Filtered retrieval of matching fragments from the repository.
        /// </summary>
        /// <param name="fragmentIds">the fragment IDs used to populate the result-set</param>
        /// <returns>a FragmentList collection</returns>
        [COEUserActionDescription("GetFragmentList")]
        public static FragmentList GetFragmentList(List<int> fragmentIds)
        {
            try
            {
                return DataPortal.Fetch<FragmentList>(new FragmentIdsCriteria(fragmentIds));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// DataPortal retrieval instructions. Use a list of FragmentID values to populate
        /// the list.
        /// </summary>
        [Serializable()]
        protected class FragmentIdsCriteria
        {
            private List<int> _fragmentIds = new List<int>();
            /// <summary>
            /// Assign this via the constructor to propose a filtered match based on fragment IDs.
            /// </summary>
            public List<int> FragmentIds
            {
                get { return _fragmentIds; }
            }

            /// <summary>
            /// The id-matching criterion to retrieve only records whose fragment IDs are in the list.
            /// </summary>
            public FragmentIdsCriteria(List<int> fragmentIds)
            {
                this._fragmentIds = fragmentIds;
            }
        }

        /// <summary>
        /// The core data-retrieval function, called internally by CSLA.
        /// </summary>
        /// <param name="criteria"></param>
        protected void DataPortal_Fetch(FragmentIdsCriteria criteria)
        {
            using (SafeDataReader sdr = this.RegDal.GetFragmentsList(criteria.FragmentIds))
            {
                this.Fill(sdr);
            }
        }

        #endregion

        /// <summary>
        /// Populates the internal fragments list via a DataReader returned from the DAL.
        /// </summary>
        /// <param name="reader"></param>
        private void Fill(SafeDataReader reader)
        {
            while (reader.Read())
            {
                Fragment f = Fragment.NewFragment(
                    reader.GetInt32("FRAGMENTID"),
                    reader.GetString("CODE"),
                    reader.GetString("DESCRIPTION"),
                    reader.GetInt32("FRAGMENTTYPEID"),
                    reader.GetString("STRUCTURE"),
                    Convert.ToDouble(reader.GetDecimal("MOLWEIGHT")),
                    reader.GetString("FORMULA")
                );
                this.Add(f);
            }
        }

        /// <summary>
        /// Generates a customized xml representation of this object.
        /// </summary>
        /// <remarks>
        /// Note that the entire list is regenerated, but only attributed for 'update' (with new
        /// children marked for 'insert') if there have been edits.
        /// </remarks>
        /// <param name="addCRUDattributes"></param>
        /// <returns></returns>
        internal string UpdateSelf(bool addCRUDattributes)
        {
            bool showUpdateTag = false;

            // If any fragment is modified, or there's any new fragment,
            // we should set update tag in <FragmentList> node.
            foreach (Fragment currentFragment in this)
            {
                if (currentFragment.IsDirty || currentFragment.IsNew)
                {
                    showUpdateTag = true;
                    break;
                }
            }

           

            // If there's any fragment deleted, we should also set update tag in <FragmentList> node.
            
            if (!showUpdateTag && this.DeletedList.Count > 0)
                showUpdateTag = true;

            StringBuilder builder = new StringBuilder("");
            builder.Append(string.Format("<FragmentList{0}>", (addCRUDattributes && showUpdateTag ? " update=\"yes\"" : string.Empty)));

            foreach (Fragment currentFragment in this)
                builder.Append(currentFragment.UpdateSelf(addCRUDattributes));

            foreach (Fragment currentDeletedFragment in this.DeletedList)
            {
                if (!currentDeletedFragment.IsNew)
                {
                builder.Append(currentDeletedFragment.UpdateSelf(true));
                }
            }

            builder.Append("</FragmentList>");


            return builder.ToString();
        }

        #region >Filtering Methods<

        [COEUserActionDescription("GetFragmentFromList")]
        public Fragment GetByID(int id)
        {
            try
            {
                Fragment retVal = null;
                if (_filteredList == null) this.InstantiateFilters();
                _filteredList.RemoveFilter();//Just in case a previous filter has been applied
                _filteredList.FilterProvider = new Csla.FilterProvider(FragmentFilters.GetByID);
                _filteredList.ApplyFilter(String.Empty, id);
                if (_filteredList.Count > 0)
                    retVal = _filteredList[0];
                return retVal;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("GetFragmentFromList")]
        public Fragment GetByCode(string code)
        {
            try
            {
                Fragment retVal = null;
                if (_filteredList == null) this.InstantiateFilters();
                _filteredList.RemoveFilter();//Just in case a previous filter has been applied
                _filteredList.FilterProvider = new Csla.FilterProvider(FragmentFilters.GetByCode);
                _filteredList.ApplyFilter(String.Empty, code);
                if (_filteredList.Count > 0)
                    retVal = _filteredList[0];
                return retVal;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("GetFragmentFromList")]
        public Fragment GetByDesc(string desc)
        {
            try
            {
                Fragment retVal = null;
                if (_filteredList == null) this.InstantiateFilters();
                _filteredList.RemoveFilter();//Just in case a previous filter has been applied
                _filteredList.FilterProvider = new Csla.FilterProvider(FragmentFilters.GetByDesc);
                _filteredList.ApplyFilter(String.Empty, desc);
                if (_filteredList.Count > 0)
                    retVal = _filteredList[0];
                return retVal;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private void InstantiateFilters()
        {
            _filteredList = new FilteredBindingList<Fragment>(this);
        }

        private class FragmentFilters
        {
            public static bool GetByID(object item, object filter)
            {
                int id = 0;
                Fragment field = null;
                //Filter is a int
                if (filter is int)
                    id = Convert.ToInt32((string)filter.ToString());
                //assume item is the object it self. (See string.empty parameter)
                if (item is Fragment)
                {
                    field = ((Fragment)item);
                    if (field != null && field.FragmentID == id) // Coverity fix - CID 11699
                        return true;
                }
                return false;
            }

            public static bool GetByCode(object item, object filter)
            {
                string code = string.Empty;
                Fragment field = null;
                //Filter is a int
                if (filter is string)
                    code = filter.ToString();
                //assume item is the object it self. (See string.empty parameter)
                if (item is Fragment)
                {
                    field = ((Fragment)item);
                    if (field != null && field.Code == code) // Coverity fix - CID 11698 
                        return true;
                }
                return false;
            }

            public static bool GetByDesc(object item, object filter)
            {
                string desc = string.Empty;
                Fragment field = null;
                //Filter is a int
                if (filter is string)
                    desc = filter.ToString();
                //assume item is the object it self. (See string.empty parameter)
                if (item is Fragment) // Coverity fix - CID 11699
                {
                    field = ((Fragment)item);
                    if (field != null && field.Description == desc)
                        return true;
                }
                return false;
            }
        }

        #endregion

        #region IKeyValueListHolder Members

        public IDictionary KeyValueList
        {
            get
            {
                System.Collections.Specialized.HybridDictionary dictionary = new System.Collections.Specialized.HybridDictionary(this.Count);
                foreach (Fragment fragment in this)
                    dictionary.Add(fragment.FragmentID, fragment.Code);
                return dictionary;
            }
        }
        /// Override the ToString implementation of the Fragmentlist to return a comma
        /// separated list of fragment ids.  This list is ordered so that it can be used
        /// for comparisons with other fragment lists
        public override string ToString()
        {
            List<int> myFragmentsList = new List<int>();
            foreach (Fragment fragment in this)
                myFragmentsList.Add(fragment.FragmentID);
            myFragmentsList.Sort();

            StringBuilder sb = new StringBuilder();
            foreach (int fragmentid in myFragmentsList)
            {
                if (fragmentid != 0)
                {
                sb.Append(fragmentid.ToString()).Append(",");
                }
            }
            if (sb.Length <= 0)
            {
                return sb.ToString();
            }
            else
            {
            return sb.Remove(sb.Length - 1, 1).ToString();
        }

        }
        #endregion

    }

}
