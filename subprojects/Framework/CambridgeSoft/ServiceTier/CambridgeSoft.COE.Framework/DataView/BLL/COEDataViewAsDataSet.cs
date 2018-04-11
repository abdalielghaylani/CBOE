using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using System.Data;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.COEDataViewService
{
    [Serializable]
    public class COEDataViewAsDataSet : CommandBase
    {
        private string _serviceName = "COEDataView";
        private string _database = Resources.CentralizedStorageDB;
        private string _action = string.Empty;

        [NonSerialized]
        private DAL _coeDAL;
        [NonSerialized]
        private DALFactory _dalFactory;

        internal DataSet ResultDataSet;

        private COEDataViewAsDataSet(string action) { _action = action.ToUpper(); }
        private COEDataViewAsDataSet() { } //Required for serialization

        protected override void DataPortal_Execute()
        {
            if (_coeDAL == null)
                LoadDAL();

            switch(_action)
            {
                case "GETMASTERTABLES":
                    this.ResultDataSet = _coeDAL.GetMasterDataViewTables();
                    break;
                case "GETPUBLISHEDTABLES":
                    this.ResultDataSet = _coeDAL.GetPublishedDataViewTables();
                    break;
                default:
                    this.ResultDataSet = new DataSet();
                    break;
            }
        }

        private void LoadDAL()
        {

            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, _database, true);
        }

        public static DataSet GetMasterTables()
        {
            return DataPortal.Execute<COEDataViewAsDataSet>(new COEDataViewAsDataSet("GETMASTERTABLES")).ResultDataSet;
        }

        public static DataSet GetPublishedTables()
        {
            return DataPortal.Execute<COEDataViewAsDataSet>(new COEDataViewAsDataSet("GETPUBLISHEDTABLES")).ResultDataSet;
        }
    }
}
