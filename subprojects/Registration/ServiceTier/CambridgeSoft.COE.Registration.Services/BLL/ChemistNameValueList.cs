using System;

using Csla;
using Csla.Data;

using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration.Services;
using CambridgeSoft.COE.Registration.Services.Common;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class ChemistNameValueList : RegistrationNameValueListBase<string, string>
    {

        private static ChemistNameValueList _list;
        private static ChemistNameValueList _activeList;
        private int _selectedChemistID;
        private string _selectedChemistName;

        public int SelectedChemistId
        {
            get
            {
                return _selectedChemistID;
            }
            set
            {
                _selectedChemistID = value;
            }
        }

        public string SelectedChemistName
        {
            get
            {
                return _selectedChemistName;
            }
            set
            {
                _selectedChemistName = value;
            }
        }

        [COEUserActionDescription("GetChemistList")]
        public static ChemistNameValueList GetChemistNameValueList()
        {
            try
            {
                if (_list == null)
                    _list = DataPortal.Fetch<ChemistNameValueList>(new ActiveOnlyCriteria(false));
                return _list;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("GetActiveChemistList")]
        public static ChemistNameValueList GetActiveChemistNameValueList() {
            try
            {
                if(_activeList == null)
                    _activeList = DataPortal.Fetch<ChemistNameValueList>(new ActiveOnlyCriteria(true));
                return _activeList;
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
            _activeList = null;
        }

        private ChemistNameValueList()
        {
        }

        private void DataPortal_Fetch(ActiveOnlyCriteria criteria)
        {
            //to fetch ChemistId and ChemistName in datareader.
            RaiseListChangedEvents = false;
            IsReadOnly = false;
            Add(new NameValueListBase<string, string>.NameValuePair("", "Choose a chemist"));

            SafeDataReader dr = null;

            try
            {
                if (criteria._activeOnly)
                    dr = this.RegDal.GetActiveChemistNameValueList();
                else
                    dr = this.RegDal.GetChemistNameValueList();

                while (dr.Read())
                {
                    Add(new NameValueListBase<string, string>.NameValuePair(dr.GetInt32(0).ToString(), dr.GetString(1)));
                }
            }
            finally
            {
                dr.Close();
            }
            IsReadOnly = true;
            RaiseListChangedEvents = true;
        }

        [Serializable()]
        private class ActiveOnlyCriteria {
            public bool _activeOnly;
            public ActiveOnlyCriteria(bool activeOnly) {
                _activeOnly = activeOnly;
            }
        }

    }
}
