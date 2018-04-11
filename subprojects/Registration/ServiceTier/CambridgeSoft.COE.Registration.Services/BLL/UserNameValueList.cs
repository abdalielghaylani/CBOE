using System;

using Csla;
using Csla.Data;

using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Services;
using CambridgeSoft.COE.Registration.Services.Common;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class UserNameValueList : RegistrationNameValueListBase<int, string>
    {

        private static UserNameValueList _list;
        private int _selectedUserID;
        private string _selectedUserName;

        public int SelectedUserID
        {
            get
            {
                //CanReadProperty(true);
                return _selectedUserID;
            }
            set
            {
                _selectedUserID = value;
            }
        }

        public string SelectedUserName
        {
            get
            {
                //CanReadProperty(true);
                return _selectedUserName;
            }
            set
            {
                _selectedUserName = value;
            }
        }

        [COEUserActionDescription("GetUserList")]
        public static UserNameValueList GetUserNameValueList()
        {
            try
            {
                if (_list == null)
                    _list = DataPortal.Fetch<UserNameValueList>(new Criteria(typeof(UserNameValueList)));
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

        private UserNameValueList()
        { /* require use of factory methods */}

        protected override void DataPortal_Fetch(object criteria)
        {
            RaiseListChangedEvents = false;
            IsReadOnly = false;

            using (SafeDataReader dr = this.RegDal.GetUserNameValueList())
            {
                while (dr.Read())
                {
                    Add(new NameValueListBase<int, string>.NameValuePair(dr.GetInt32(0), dr.GetString(1)));
                }
            }
            RaiseListChangedEvents = true;
            IsReadOnly = true;
        }
    }
}
