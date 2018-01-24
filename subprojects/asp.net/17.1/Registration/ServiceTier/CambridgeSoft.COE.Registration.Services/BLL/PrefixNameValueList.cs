using System;

using Csla;
using Csla.Data;

using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Services;
using CambridgeSoft.COE.Registration.Services.Common;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class PrefixNameValueList : RegistrationNameValueListBase<int, string>
    {
        private static PrefixNameValueList _list;
        private int _selectedPrefixID;
        private string _selectedPrefixName;

        public int SelectedPrefixID
        {
            get
            {
                //CanReadProperty(true);
                return _selectedPrefixID;
            }
            set
            {
                _selectedPrefixID = value;
            }
        }

        public string SelectedPrefixName
        {
            get
            {
                //CanReadProperty(true);
                return _selectedPrefixName;
            }
            set
            {
                _selectedPrefixName = value;
            }
        }

        [COEUserActionDescription("GetPrefixList")]
        public static PrefixNameValueList GetPrefixNameValueList()
        {
            try
            {
                if (_list == null)
                    _list = DataPortal.Fetch<PrefixNameValueList>(new Criteria(typeof(PrefixNameValueList)));
                return _list;
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

        private PrefixNameValueList()
        { /* require use of factory methods */}

        protected override void DataPortal_Fetch(object criteria)
        {
            //to fetch PrefixId and PrefixName in datareader.
            RaiseListChangedEvents = false;
            IsReadOnly = false;
            using (SafeDataReader dr = this.RegDal.GetPrefixNameValueList())
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
