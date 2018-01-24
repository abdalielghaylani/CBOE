using CambridgeSoft.COE.Framework.COEDataViewService;
using System.Data;

namespace COEServiceLib
{
    internal struct COEHitListCache
    {
        internal delegate COEDataViewBO DataViewLoader();
        internal delegate DataSet DataSetLoader();

        private readonly DataViewLoader LoadDataView;
        private readonly DataSetLoader LoadDataSet;
        private COEDataViewBO dataViewBO;
        private DataSet dataSet;

        internal COEHitListCache(DataViewLoader dataViewLoader, DataSetLoader dataSetLoader)
        {
            try
            {
                this.LoadDataView = dataViewLoader;
                this.LoadDataSet = dataSetLoader;
                this.dataViewBO = dataViewLoader();
                this.dataSet = dataSetLoader();
            }
            catch
            {
                throw;
            }
        }

        internal COEDataViewBO DataViewBO
        {
            get
            {
                if (this.dataViewBO == null)
                {
                    this.dataViewBO = this.LoadDataView();
                }
                return this.dataViewBO;
            }
        }

        internal DataSet DataSet
        {
            get
            {
                if (this.dataSet == null && LoadDataSet != null)
                {
                    this.dataSet = this.LoadDataSet();
                }
                return this.dataSet;
            }
        }
    }
}
