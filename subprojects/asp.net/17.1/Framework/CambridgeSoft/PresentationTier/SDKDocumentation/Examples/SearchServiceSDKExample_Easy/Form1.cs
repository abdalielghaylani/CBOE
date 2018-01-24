using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using Csla.Security;
using CambridgeSoft.COE.Framework.COEDataViewService;

namespace SearchServiceSDKExample_Easy
{
    public partial class Form1 : Form
    {
        #region Constants
        // Xml file names
        private const string SEARCHCRITERIA_XML = @"../../SearchCriteria.xml";
        private const string RESULTCRITERIA_XML = @"../../ResultsCriteria.xml";
        private const string PAGINGINFOFILE_XML = @"../../PagingInfo.xml";
        #endregion

        #region Variables

        private ResultsCriteria _resultsCriteria = null;
        private SearchCriteria _searchCriteria = null;
        private PagingInfo _pagingInfo = null;
        private HitListInfo _hitlistInfo = null;
        private COESearch _coeSearch = new COESearch();
        private COEDataView _dataView = null;
        private enum Steps
        {
            Login,
            RetrieveDataViews,
            GetHitList,
            GetData,
        }

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        #region Events Handlers

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DoLogin();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message.ToString());
            }
        }

        private void RetrieveDVsButton_Click(object sender, EventArgs e)
        {
            //Get the available dataviews in the given datasource. Check your COEFrameworkConfig for more details.
            COEDataViewBOList dataViews = COEDataViewBOList.GetDataViewListAndNoMaster();
            if (dataViews.Count > 0)
            {
                //At least, I have one DataView to display.
                //Display a status text.
                this.DataViewsStepStatusLabel.Text += "Found: " + dataViews.Count.ToString() + " DataViews!";
                //Select by default the first one in the list.
                _dataView = dataViews[0].COEDataView;
            }
            else
            {
                this.DataViewsStepStatusLabel.Text += "No DataViews have been found!";
            }
        }

        private void HitlistStepButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Build SearchCriteria
                _searchCriteria = new SearchCriteria();
                this.BuildSearchCriteriaFromXML(ref _searchCriteria);

                _hitlistInfo = new HitListInfo();
                //Call GetHitList method.
                _hitlistInfo = _coeSearch.GetHitList(_searchCriteria, _dataView);
            }
            catch (Exception ex)
            {
                this.ShowErrorMessageBox(ex.GetBaseException().Message);
            }
            if (_hitlistInfo != null)
                this.HitListStepStatusLabel.Text += " Returned HitlistID:" + _hitlistInfo.HitListID.ToString() +
                                                    " Record Count:" + _hitlistInfo.RecordCount.ToString();
            else
                this.HitListStepStatusLabel.Text += "Null Hitlist!";
        }

        private void DataStepButton_Click(object sender, EventArgs e)
        {
            // Build ResultsCriteria
            _resultsCriteria = new ResultsCriteria();
            BuildResultsCriteriaFromXML(ref _resultsCriteria);

            //PagingInfo
            _pagingInfo = new PagingInfo();
            BuildPagingInfoFromXML(ref _pagingInfo);

            DataSet dataset = null;
            try
            {
                //Call GetDataMethod
                dataset = _coeSearch.GetData(_resultsCriteria, _pagingInfo, _dataView);
            }
            catch (Exception ex)
            {
                this.ShowErrorMessageBox(ex.GetBaseException().Message);
            }
            if (dataset != null)
            {
                if (dataset.Tables.Count > 0)
                    this.DataStepStatusLabel.Text += " Number of found records according Search/Results/PagingInfo criterias: " + dataset.Tables[0].Rows.Count.ToString();
            }
            else
            {
                this.DataStepStatusLabel.Text += " Invalid dataset";
            }
        }

        #endregion

        #region Methods

        private void DoLogin()
        {
            System.Security.Principal.IPrincipal user = Csla.ApplicationContext.User;
            string userName = "T5_85";
            string password = "T5_85";
            bool result = false;
            try
            {
                result = COEPrincipal.Login(userName, password);
                int mySession = COEUser.SessionID;
            }
            catch (System.Exception ex)
            {
                this.ShowErrorMessageBox(ex.GetBaseException().Message.ToString());
            }
            if (result)
            {
                this.LoginStepStatusLabel.Text += "Logged On!";
            }
            else
            {
                this.LoginStepStatusLabel.Text += "Invalid user/password!";
            }
        }

        /// <summary>
        /// Builds a SearchCriteria object from xml file
        /// </summary>
        /// <param name="searchCriteria"></param>
        private void BuildSearchCriteriaFromXML(ref SearchCriteria searchCriteria)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(SEARCHCRITERIA_XML);
            try
            {
                searchCriteria = new SearchCriteria(doc);
            }
            catch (Exception ex)
            {
                this.ShowErrorMessageBox(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        ///     Builds a ResultsCriteria object from xml file
        /// </summary>
        /// <param name="resultsCriteria"></param>
        private void BuildResultsCriteriaFromXML(ref ResultsCriteria resultsCriteria)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(RESULTCRITERIA_XML);
            resultsCriteria = new ResultsCriteria(doc);
        }

        /// <summary>
        ///     Builds a PagingInfo object from xml file
        /// </summary>
        /// <param name="pagingInfo"></param>
        private void BuildPagingInfoFromXML(ref PagingInfo pagingInfo)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PAGINGINFOFILE_XML);
            pagingInfo = new PagingInfo(doc);
        }

        private void ShowErrorMessageBox(string message)
        {
            MessageBox.Show(message);
        }

        #endregion

        
    }
}