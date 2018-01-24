using System;

using Csla;
using Csla.Data;

using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Services;
using CambridgeSoft.COE.Registration.Services.Common;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class IdentifierNameValueList : RegistrationNameValueListBase<int, string>
    {
        private static IdentifierNameValueList _list;
        private int _selectedIdentifierID;
        private string _selectedIdentifierName;

        public int SelectedIdentifierID
        {
            get
            {
                //CanReadProperty(true);
                return _selectedIdentifierID;
            }
            set
            {
                _selectedIdentifierID = value;
            }
        }

        public string SelectedIdentifierName
        {
            get
            {
                //CanReadProperty(true);
                return _selectedIdentifierName;
            }
            set
            {
                _selectedIdentifierName = value;
            }
        }

        [COEUserActionDescription("GetIdentifierNameValueList")]
        public static IdentifierNameValueList GetIdentifierNameValueList()
        {
            try
            {
                if (_list == null)
                    _list = DataPortal.Fetch<IdentifierNameValueList>(new Criteria(typeof(IdentifierNameValueList)));
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

        private IdentifierNameValueList()
        { /* require use of factory methods */}

        protected override void DataPortal_Fetch(object criteria)
        {
            //to fetch IdentifierId and IdentifierName in datareader.
            RaiseListChangedEvents = false;
            IsReadOnly = false;

            using (SafeDataReader dr = this.RegDal.GetIdentifierNameValueList())
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
