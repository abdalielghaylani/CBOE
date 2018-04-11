using System;
using System.Data;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using Csla.Security;



public partial class COESearchServiceTest : System.Web.UI.Page
{

    COESearch _coeSearch = new COESearch();
    string tempPath;
    string inputxmlPath;
    /// Note that the SearchServiceProxy object depends on configurtion
    /// values being present in the App.Config file of this test application.
    /// The proxy abstracts the access to the SearchService class which can be
    /// instatiated either on the same tier as this test application, or on a
    /// remote server via SOAP or Remoting.

    // global objects to hold the input for test methods
    COEDataView dataView = new COEDataView();
    SearchCriteria searchCriteria = new SearchCriteria();
    ResultsCriteria resultsCriteria = new ResultsCriteria();
    SecurityInfo securityInfo = new SecurityInfo();
    PagingInfo pagingInfo = new PagingInfo();
    HitListInfo hitlistInfo = new HitListInfo();
    //List<DataGridView> grids = new List<DataGridView>();
    SearchResponse searchResponse = null;

    // Paths to xml files


    // Xml file names
    const string APPNAME_FILE = @"AppName.xml";
    const string DATAVIEW_FILE = @"DataView.xml";
    const string SEARCHCRITERIA_FILE = @"SearchCriteria.xml";
    const string RESULTCRITERIA_FILE = @"ResultsCriteria.xml";
    const string SECURITYINFO_FILE = @"SecurityInfo.xml";
    const string PAGINGINFO_FILE = @"PagingInfo.xml";
    const string HITLISTINFO_FILE = @"HitlistInfo.xml";
    const string DATASET_FILE = @"DataSet.xml";
    const string PAGINGINFO_OUTPUT_FILE = @"PagingInfoOut.xml";

    protected void Page_Load(object sender, EventArgs e)
    {
          tempPath = this.Server.MapPath(this.Request.ApplicationPath) ;
          inputxmlPath = tempPath + @"\xml\ChemInv\";
          btnXML.Visible = false;
          btnGridView.Visible = true;
         
       //   string currentPath = Application.StartupPath + @"\";

            if (Csla.ApplicationContext.AuthenticationType == "Windows")
            {
                AppDomain.CurrentDomain.SetPrincipalPolicy(System.Security.Principal.PrincipalPolicy.WindowsPrincipal);
            }
            else
            {
                //in this case you are logging in with the appName will get and set all the privilges
                //the a user has. these privileges are gathtered from cs_security and is oracle only at this time
                //this inforamtion could come from a login screen
                DoLogin();
               
            }
        }

    private void DoLogin()
    {
        //ensures that the CurrentPrincipal property of the thread is set to an unauthenticated COEPrincipal object.
        COEPrincipal.Logout();
        //I am hardcoding this for now. I don't have time to figure this all out.
        System.Security.Principal.IPrincipal user = Csla.ApplicationContext.User;
        //here you are logging in based on your application. If you are not a user for this application
        //in cs_security you will be rejected.
        string appName = "SAMPLE";
        string userName = "T5_85";
        string password = "T5_85";
       bool result = COEPrincipal.Login(userName, password);

            
    }

    protected void tbInputData_TabClick(object sender, Infragistics.WebUI.UltraWebTab.WebTabEvent e)
    {
      
    }

    private void ClearResultsXml()
    {
        CreateBlankXmlFile(tempPath + @"\TempXMLFiles\" + @"HitListInfo.xml");
        CreateBlankXmlFile(tempPath + @"\TempXMLFiles\" + @"PagingInfo.xml");
        CreateBlankXmlFile(tempPath + @"\TempXMLFiles\" + @"DataSet.xml");
    }

    void CreateBlankXmlFile(string filePath)
    {
        XmlTextWriter xmlWriter = new XmlTextWriter(filePath, System.Text.Encoding.UTF8);
        xmlWriter.Formatting = Formatting.Indented;
        xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
        xmlWriter.WriteStartElement("Root");
        xmlWriter.Close();
    }
    protected void tbCOESearchService_TabClick(object sender, Infragistics.WebUI.UltraWebTab.WebTabEvent e)
    {

    }
    protected void tbResults_TabClick(object sender, Infragistics.WebUI.UltraWebTab.WebTabEvent e)
    {

    }

 
    protected void ShowResults()
    {
        tbResults.Tabs.GetTab(0).ContentPane.TargetUrl = @"TempXMLFiles/" + @"HitListInfo.xml";
        tbResults.Tabs.GetTab(1).ContentPane.TargetUrl = @"TempXMLFiles/" + @"PagingInfo.xml";
        tbResults.Tabs.GetTab(2).ContentPane.TargetUrl = @"TempXMLFiles/" + @"DataSet.xml";
        tbCOESearchService.SelectedTab = 2;

    
    }
    protected void btnTestGetData_Click(object sender, EventArgs e)
    {
        //ClearResultsXml();

        // Build SecurityInfo
        securityInfo = new SecurityInfo();
        BuildSecurityInfoFromXML(ref securityInfo);

        // Build DataView
        dataView = new COEDataView();
        BuildDataViewFromXML(ref dataView);

        // Build ResultsCriteria
        ResultsCriteria resultsCriteria = new ResultsCriteria();
        BuildResultsCriteriaFromXML(ref resultsCriteria);

        //PagingInfo
        pagingInfo = new PagingInfo();
        BuildPagingInfoFromXML(ref pagingInfo);

        try
        {
            DataSet dataset = null;
            dataset = _coeSearch.GetData(resultsCriteria, pagingInfo, dataView);
            SaveDataSetToXml(dataset);
            SavePagingInfoToXml(pagingInfo);
            //// Save a blank hitlistInfo object
            hitlistInfo = new HitListInfo();
            SaveHitlistInfoToXml(hitlistInfo);
            ShowResults();
        }
        catch (Exception ex)
        {
        
        }  
    }
    protected void btnTestDoSearch_Click(object sender, EventArgs e)
    {
       //ClearResultsXml();

        // Build SecurityInfo
        securityInfo = new SecurityInfo();
        BuildSecurityInfoFromXML(ref securityInfo);

        // Build DataView
        dataView = new COEDataView();
        BuildDataViewFromXML(ref dataView);

        // Build SearchCriteria
        searchCriteria = new SearchCriteria();
        if ((bool)chkSendSearchCritera.Checked)
        {
            searchCriteria = null;
        }
        else
        {
            BuildSearchCriteriaFromXML(ref searchCriteria);
        }
        // Build ResultsCriteria
        resultsCriteria = new ResultsCriteria();
        BuildResultsCriteriaFromXML(ref resultsCriteria);

        //PagingInfo
        pagingInfo = new PagingInfo();
        BuildPagingInfoFromXML(ref pagingInfo);

        try
        {
            searchResponse = null;

            searchResponse = _coeSearch.DoSearch(searchCriteria, resultsCriteria, pagingInfo, dataView);

            this.pagingInfo = searchResponse.PagingInfo;
            SaveHitlistInfoToXml((HitListInfo)searchResponse.HitListInfo);
            SaveDataSetToXml((DataSet)searchResponse.ResultsDataSet);
            SavePagingInfoToXml((PagingInfo)searchResponse.PagingInfo);
            ShowResults();
        }
        catch (Exception ex)
        {
       
        }
       
    }

    #region Build Input objects from xml files

    /// <summary>
    ///     Builds a COE Dataview object from xml file
    /// </summary>
    /// <param name="dataView"></param>
    private void BuildDataViewFromXML(ref COEDataView dataView)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(inputxmlPath + DATAVIEW_FILE);
        dataView = new COEDataView(doc);
        dataView.Database = dataView.Tables[0].Database;
    }

    /// <summary>
    ///     Builds a SearchCriteria object from xml file
    /// </summary>
    /// <param name="searchCriteria"></param>
    private void BuildSearchCriteriaFromXML(ref SearchCriteria searchCriteria)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(inputxmlPath + SEARCHCRITERIA_FILE);
        try
        {
            searchCriteria = new SearchCriteria(doc);
        }
        catch (Exception ex)
        {
           
        }
    }

    /// <summary>
    ///     Builds a ResultsCriteria object from xml file
    /// </summary>
    /// <param name="resultsCriteria"></param>
    private void BuildResultsCriteriaFromXML(ref ResultsCriteria resultsCriteria)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(inputxmlPath + RESULTCRITERIA_FILE);
        resultsCriteria = new ResultsCriteria(doc);
    }

    /// <summary>
    ///     Builds a PagingInfo object from xml file
    /// </summary>
    /// <param name="pagingInfo"></param>
    
    private void BuildPagingInfoFromXML(ref PagingInfo pagingInfo)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(inputxmlPath + PAGINGINFO_FILE);
        pagingInfo = new PagingInfo(doc);
    }

    /// <summary>
    ///     Builds a SecurityInfo object from xml file
    /// </summary>
    /// <param name="securityInfo"></param>

    private void BuildSecurityInfoFromXML(ref SecurityInfo securityInfo)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(inputxmlPath + SECURITYINFO_FILE);
        securityInfo = new SecurityInfo(doc);
    }
    #endregion

    #region Persist Output objects to xml files
    void SaveHitlistInfoToXml(HitListInfo hitlistInfo)
    {

        try
        {
            XmlDocument xmldoc = new XmlDocument();

            string path = tempPath + @"\TempXMLFiles\" + @"HitListInfo.xml";
            File.SetAttributes(path, FileAttributes.Normal);

            xmldoc.LoadXml(hitlistInfo.ToString());
            xmldoc.Save(path);

        }
        catch (Exception ex)
        {
          
        }
    }

    void SavePagingInfoToXml(PagingInfo pagingInfo)
    {

        try
        {
            XmlDocument xmldoc = new XmlDocument();

            string path = tempPath + @"\TempXMLFiles\" + @"PagingInfo.xml";
            File.SetAttributes(path, FileAttributes.Normal);
            xmldoc.LoadXml(pagingInfo.ToString());
            xmldoc.Save(path);

       }
        catch (Exception ex)
        {

        }
    }

    void SaveDataSetToXml(DataSet dataSet)
    {
        try
        {
            string path = tempPath + @"\TempXMLFiles\" + @"DataSet.xml";
            File.SetAttributes(path, FileAttributes.Normal);
            dataSet.WriteXml(path);
        }
        catch (Exception ex)
        {
          
        }
    }

    #endregion


    protected void btnGridView_Click(object sender, EventArgs e)
    {
        tbResults.Tabs.GetTab(0).ContentPane.TargetUrl = null;
        DataSet dsHitListInfo = new DataSet();
        dsHitListInfo.ReadXml(tempPath + @"\TempXMLFiles\" + @"HitListInfo.xml");
        gvHitListInfo.DataSource = dsHitListInfo;
        gvHitListInfo.DataBind();

        tbResults.Tabs.GetTab(1).ContentPane.TargetUrl = null;
        DataSet dsPagingInfo = new DataSet();
        dsPagingInfo.ReadXml(tempPath + @"\TempXMLFiles\" + @"PagingInfo.xml");
        gvPagingInfo.DataSource = dsPagingInfo;
        gvPagingInfo.DataBind();

        tbResults.Tabs.GetTab(2).ContentPane.TargetUrl = null;
        DataSet dsDataSet = new DataSet();
        dsDataSet.ReadXml(tempPath + @"\TempXMLFiles\" + @"DataSet.xml");
        if (dsDataSet.Tables.Count > 0)
        {
            gvDataSet.DataSource = dsDataSet;
            gvDataSet.DataBind();
        }

        gvHitListInfo.Visible = true;
        gvPagingInfo.Visible = true;
        gvDataSet.Visible = true;

        btnGridView.Visible = false;
        btnXML.Visible = true;
    }

    protected void btnXML_Click(object sender, EventArgs e)
    {
        ShowResults();
        btnGridView.Visible = true;
        btnXML.Visible = false;
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (uploadSecurityInfo.HasFile)
        {
            File.SetAttributes(inputxmlPath + "SecurityInfo.xml", FileAttributes.Normal);

            uploadSecurityInfo.SaveAs(inputxmlPath + "SecurityInfo.xml");
            tbInputData.Tabs.GetTab(0).ContentPane.TargetUrl = @"xml/ChemInv/SecurityInfo.xml";

        }
                
        if (uploadDataView.HasFile)
        {
            File.SetAttributes(inputxmlPath + "DataView.xml", FileAttributes.Normal);

            uploadDataView.SaveAs(inputxmlPath + "DataView.xml");
            tbInputData.Tabs.GetTab(1).ContentPane.TargetUrl = @"xml/ChemInv/DataView.xml";

        }

        
        if (uploadSearchCriteria.HasFile)
        {
            File.SetAttributes(inputxmlPath + "SearchCriteria.xml", FileAttributes.Normal);

            uploadSearchCriteria.SaveAs(inputxmlPath + ".xml");
            tbInputData.Tabs.GetTab(2).ContentPane.TargetUrl = @"xml/ChemInv/SearchCriteria.xml";

        }

        if (uploadResultsCriteria.HasFile)
        {
            File.SetAttributes(inputxmlPath + "ResultsCriteria.xml", FileAttributes.Normal);

            uploadResultsCriteria.SaveAs(inputxmlPath + "ResultsCriteria.xml");
            tbInputData.Tabs.GetTab(3).ContentPane.TargetUrl = @"xml/ChemInv/ResultsCriteria.xml";

        }

        if (uploadPagingInfo.HasFile)
        {
            File.SetAttributes(inputxmlPath + "PagingInfo.xml", FileAttributes.Normal);

            uploadPagingInfo.SaveAs(inputxmlPath + "PagingInfo.xml");
            tbInputData.Tabs.GetTab(4).ContentPane.TargetUrl = @"xml/ChemInv/PagingInfo.xml";

        }

    }



    protected void btnGetHitList_Click(object sender, EventArgs e)
    {
        // ClearResultsXml();
        // Build SecurityInfo
        securityInfo = new SecurityInfo();
        BuildSecurityInfoFromXML(ref securityInfo);

        // Build DataView
        dataView = new COEDataView();
        BuildDataViewFromXML(ref dataView);

        // Build SearchCriteria
        searchCriteria = new SearchCriteria();
        BuildSearchCriteriaFromXML(ref searchCriteria);

        try
        {
            hitlistInfo = new HitListInfo();
            hitlistInfo = _coeSearch.GetHitList(searchCriteria, dataView);
            SaveHitlistInfoToXml(hitlistInfo);

            //            tbResults.Tabs.GetTab(0).ContentPane.TargetUrl = @"TempXMLFiles/" + @"HitListInfo.xml";
            //// Save a blank dataset
            DataSet dataset = new DataSet();
            SaveDataSetToXml(dataset);

            // Save a blank PagingInfo
            pagingInfo = new PagingInfo();
            SavePagingInfoToXml(pagingInfo);
            ShowResults();
            tbCOESearchService.SelectedTab = 2;

        }
        catch (Exception ex)
        {

        }
 
    }
}