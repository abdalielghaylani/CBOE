using System;
using System.Collections;
using System.Data;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COEReportingService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;

namespace CambridgeSoft.COE.Framework.CustomReportDesigner
{
    public class COEServicesAdapter
    {
        #region Singleton Template
        private COEServicesAdapter()
        { 

        }

        private static COEServicesAdapter _instance;

        public static COEServicesAdapter GetInstance()
        {
            if (_instance == null)
                _instance = new COEServicesAdapter();

            return _instance;
        }
        #endregion

        #region Interface Methods

        public bool LogIn(string userName, string password)
        {            
            bool isLoged = COEPrincipal.Login(userName,password);            
            if (isLoged)
                CambridgeSoft.COE.Framework.Common.WebUtils.SetCslaPrincipal();
            return isLoged;
        }

        public bool LogIn(string autenticationTicket)
        {
            bool isLoged = false;
            try { isLoged = COEPrincipal.Login(autenticationTicket); }
            catch { isLoged = false; }
            if (isLoged)
                CambridgeSoft.COE.Framework.Common.WebUtils.SetCslaPrincipal();            
            return isLoged;
        }

        public DataSet GetAvailableDataViews()
        {
            COEDataViewBOList dataviewList = COEDataViewBOList.GetDataViewListAndNoMaster();

            return BuildDataSet(dataviewList, "COEDataViewBOList", new string [] { "ID", "Name", "Description", "BaseTable", "DatabaseName", "DateCreated" });
        }

        public COEDataView GetDataView(int dataviewId)
        {
            return COEDataViewBO.Get(dataviewId).COEDataView;
        }

        public DataSet GetAvailableReportTemplates()
        {
            COEReportBOList templateList = COEReportBOList.GetCOEReportBOList();

            return BuildDataSet(templateList, "COEReportTemplateBOList", new string[] { "ID", "Name", "Description", "DateCreated"});
            
        }

        #endregion

        #region Private Methods
        public DataSet BuildDataSet(IList sourceList, string tableName, string [] fieldNames)
        {
            DataSet resultDataSet = new DataSet();
            resultDataSet.Tables.Add(new DataTable(tableName));

            if (sourceList.Count > 0)
            {
                foreach (string currentField in fieldNames)
                {
                    COEDataBinder typeGuessingDataBinder = null;
                    if (sourceList.Count > 0 && typeGuessingDataBinder == null)
                        typeGuessingDataBinder = new COEDataBinder(sourceList[0]);

                    Type type = (typeGuessingDataBinder == null ? typeof(string) : typeGuessingDataBinder.RetrieveProperty(currentField).GetType());

                    resultDataSet.Tables[0].Columns.Add(new DataColumn(currentField, type));
                }

                foreach (Object currentObject in sourceList)
                {
                    resultDataSet.Tables[0].Rows.Add(resultDataSet.Tables[0].NewRow());

                    COEDataBinder dataBinder = new COEDataBinder(currentObject);

                    foreach (string currentField in fieldNames)
                        resultDataSet.Tables[0].Rows[resultDataSet.Tables[0].Rows.Count - 1][currentField] = dataBinder.RetrieveProperty(currentField);
                }
            }

            return resultDataSet;
        }
        #endregion
    }
}
