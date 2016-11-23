using System;

using Csla;
using Csla.Data;

using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Services;
using CambridgeSoft.COE.Registration.Services.Common;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class FragmentNameValueList : RegistrationNameValueListBase<int, string>
    {

        private static FragmentNameValueList _list;
        private int _selectedFragmentID;
        private string _selectedFragmentName;

        public int SelectedFragmentID
        {
            get
            {
                //CanReadProperty(true);
                return _selectedFragmentID;
            }
            set
            {
                _selectedFragmentID = value;
            }
        }

        public string SelectedFragmentName
        {
            get
            {
                //CanReadProperty(true);
                return _selectedFragmentName;
            }
            set
            {
                _selectedFragmentName = value;
            }
        }

        [COEUserActionDescription("GetFragmentNameValueList")]
        public static FragmentNameValueList GetFragmentNameValueList()
        {
            try
            {
                if (_list == null)
                    _list = DataPortal.Fetch<FragmentNameValueList>(new Criteria(typeof(FragmentNameValueList)));
                return _list;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("GetFragmentTypesList")]
        public static FragmentNameValueList GetFragmentTypesNameValueList()
        {
            try
            {
                //Update on 2009/02/06 to fix bug 105644
                //if (_list == null)
                //    _list = DataPortal.Fetch<FragmentNameValueList>(new FragmentTypesCriteria());
                //return _list;
                return DataPortal.Fetch<FragmentNameValueList>(new FragmentTypesCriteria());
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        public static void InvalidateCache()
        {
            _list = null;
        }

        private FragmentNameValueList()
        { /* require use of factory methods */}

        [Serializable()]
        private class FragmentTypesCriteria
        {
            public FragmentTypesCriteria()
            {
            }
        }

        private void DataPortal_Fetch(FragmentTypesCriteria criteria)
        {
            this.IsReadOnly = RaiseListChangedEvents = false;

            using (SafeDataReader dr = this.RegDal.GetFragmentTypesValueList())
            {
                while (dr.Read())
                {
                    Add(new NameValueListBase<int, string>.NameValuePair(dr.GetInt32(0), dr.GetString(1)));

                }
            }
            this.IsReadOnly = RaiseListChangedEvents = true;
        }

        protected override void DataPortal_Fetch(object criteria)
        {
            //to fetch SaltId and SaltName in datareader.
            RaiseListChangedEvents = false;
            IsReadOnly = false;
            using (SafeDataReader dr = this.RegDal.GetFragmentNameValueList())
            {
                while (dr.Read())
                {
                    Add(new NameValueListBase<int, string>.NameValuePair(dr.GetInt32(0), dr.GetString(1)));
                }
            }
            IsReadOnly = true;
            RaiseListChangedEvents = true;
        }

    }
}
