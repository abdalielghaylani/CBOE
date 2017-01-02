using System;

using Csla;
using Csla.Data;

using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Services;
using CambridgeSoft.COE.Registration.Services.Common;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class ProjectNameValueList : RegistrationNameValueListBase<int, string>
    {

        private static ProjectNameValueList _list;
        private int _selectedProjectID;
        private string _selectedProjectName;

        public int SelectedProjectID
        {
            get
            {
                //CanReadProperty(true);
                return _selectedProjectID;
            }
            set
            {
                _selectedProjectID = value;
            }
        }

        public string SelectedProjectName
        {
            get
            {
                //CanReadProperty(true);
                return _selectedProjectName;
            }
            set
            {
                _selectedProjectName = value;
            }
        }

        [COEUserActionDescription("GetProjectNameValueList")]
        public static ProjectNameValueList GetProjectNameValueList()
        {
            try
            {
                if (_list == null)
                    _list = DataPortal.Fetch<ProjectNameValueList>(new Criteria(typeof(ProjectNameValueList)));
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

        private ProjectNameValueList()
        { /* require use of factory methods */}

        protected override void DataPortal_Fetch(object criteria)
        {
            RaiseListChangedEvents = false;
            IsReadOnly = false;
            using (SafeDataReader dr = this.RegDal.GetProjectNameValueList())
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
