using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Configuration;
using System.Xml.Xsl;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COEExportService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using Csla.Security;


namespace COESearchServiceTest
{
    /// <summary>
    ///     Windows forms application to exercise the methods of ChemOffice Enterprise
    ///     Search Service.  The form provides a covenient way to view/edit/manage xml
    ///     files to be used as imput/output to the search service methods.
    /// 
    ///     Usage:
    ///     Click on the "Imput" tab on the top level Tab control.  Use the sub tabs to 
    ///     view/edit the various xml documents that can be used as imput for the Search
    ///     service methods.  Once you are satified with the imput data, click on the "Methods"
    ///     tab.  Click on one of the method buttons to excersise a service call.  The Results tab
    ///     presents the output objects returned by the server.
    ///     
    /// </summary>
    public partial class frmCOESearchTest : Form
    {
        #region Global variable declarations
        COELog _coeLog = COELog.GetSingleton("Client");
        /// Global proxy object to access the SearchService methods 
        COESearch _coeSearch = new COESearch();
        COEExport _coeExport = new COEExport();
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
        List<DataGridView> grids = new List<DataGridView>();
        SearchResponse searchResponse = null;
        bool _isPartialSearch;
        // Paths to xml files
        string tempPath = COEConfigurationBO.ConfigurationBaseFilePath + Application.ProductName + @"\";
        string currentPath = Application.StartupPath + @"\";


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
        const string EXPORT_FILE = @"EXPORT.txt";


        // Xslt to transform xml into html for display in IE
        const string IE_XLST_PATH = @"defaultss.xml";
        const string TMP_HTML = @"tmp.html";

        // current file path to be displayed by the controls
        string currentInputXmlPath = "";
        string currentResutlXmlPath = "";
        string currentExportPath = "";

        #endregion

        #region Security

        private void frmCOESearchTest_Load(object sender, EventArgs e)
        {
            try
            {
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
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void DoLogin()
        {
            System.Security.Principal.IPrincipal user = Csla.ApplicationContext.User;
            string userName = ConfigurationManager.AppSettings.Get("LogonUserName");
            string password = ConfigurationManager.AppSettings.Get("LogonPassword");

            try
            {
                bool result = COEPrincipal.Login(userName, password);
                int mySession = COEUser.SessionID;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message.ToString());
            }
        }


        #endregion

        #region Code to manage input/output xml docs used by the tested methods
        /// <summary>
        ///     Form initialization
        ///     Start with the Input tab and SecurityInfo subtab in browser view mode
        ///     Delete output files if present from previous executions.
        /// </summary>
        public frmCOESearchTest()
        {
            InitializeComponent();
            this.txtEditor.Hide();

            //determine if logging is turned on
            if (_coeLog.Enabled)
            {
                logButton.Enabled = true;
            }
            else
            {
                logButton.Enabled = false;

            }

            //populate the export types list
            //List<string> exportTypesList = _coeExport.GetFormatterTypesList();
            //ExportTypesList.Items.Add("...select a export format");
            //foreach (string exportType in exportTypesList)
            //{
            //    ExportTypesList.Items.Add(exportType);
            //}

            //// Create the application temp directory
            System.IO.Directory.CreateDirectory(tempPath);
            LoadAppNameFromFile();
            currentInputXmlPath = tempPath + SECURITYINFO_FILE;
            this.brwsrInput.Show();
            this.btnEditSave.Text = "Edit";
            displayInBrowser(currentInputXmlPath, brwsrInput);

            if (File.Exists(tempPath + DATASET_FILE))
                File.Delete(tempPath + DATASET_FILE);

            if (File.Exists(tempPath + PAGINGINFO_OUTPUT_FILE))
                File.Delete(tempPath + PAGINGINFO_OUTPUT_FILE);

            if (File.Exists(tempPath + HITLISTINFO_FILE))
                File.Delete(tempPath + HITLISTINFO_FILE);

            //_coeSearch.HitListReady += new COESearch.HitListReadyHandler(_coeSearch_HitListReady);
        }

        /*void _coeSearch_HitListReady(object sender, HitListEventArgs args) {
            hitlistInfo.RecordCount = args.RecordsAffected;
            SaveHitlistInfoToXml(hitlistInfo);
        }*/


        /// <summary>
        ///     Handles the selection of a tab in the InputObjects tab control
        ///     Sets the global variable that controls the current xml document to be displayed
        ///     in the browser control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControlInputObjects_Selected(object sender, TabControlEventArgs e)
        {
            try
            {
                string selectedTab = ((TabControl)sender).SelectedTab.Text;

                SaveXmlFile();

                switch (selectedTab)
                {
                    case "SecurityInfo":
                        currentInputXmlPath = SECURITYINFO_FILE;
                        break;
                    case "DataView":
                        currentInputXmlPath = DATAVIEW_FILE;
                        break;
                    case "SearchCriteria":
                        currentInputXmlPath = SEARCHCRITERIA_FILE;
                        break;
                    case "ResultsCriteria":
                        currentInputXmlPath = RESULTCRITERIA_FILE;
                        break;
                    case "PagingInfo":
                        currentInputXmlPath = PAGINGINFO_FILE;
                        break;
                }
                currentInputXmlPath = tempPath + currentInputXmlPath;
                displayInBrowser(currentInputXmlPath, brwsrInput);
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }


        /// <summary>
        ///     Persists the contents of the text editor to the current xml
        ///     file path.
        /// </summary>
        private void SaveXmlFile()
        {
            if ((currentInputXmlPath != "") && (this.txtEditor.Text != ""))
            {
                File.WriteAllText(currentInputXmlPath, this.txtEditor.Text);
            }
            this.btnEditSave.Text = "Edit";
        }

        /// <summary>
        ///     Displays the current xml in a browser control.
        ///     Uses xslt to transfor the xml into html using IE default template
        /// </summary>
        private void displayInBrowser(string xmlPath, WebBrowser browser)
        {
            ClearResultGrids();
            this.ExportStringTextBox.Hide();
            this.txtEditor.Hide();
            this.txtEditor.Text = "";
            browser.Show();
            // Create a blank file if it does not exist
            if (!File.Exists(xmlPath))
            {
                CreateBlankXmlFile(xmlPath);
            }
            XPathDocument x = new XPathDocument(xmlPath);
            XslCompiledTransform t = new XslCompiledTransform();
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager("frmCOESearchTest.resx", System.Reflection.Assembly.GetExecutingAssembly());

            string styleSheet = (string)COESearchServiceTest.Properties.Resources.defaultss;
            File.WriteAllText(tempPath + IE_XLST_PATH, styleSheet);
            t.Load(tempPath + IE_XLST_PATH);
            t.Transform(xmlPath, tempPath + TMP_HTML);
            browser.Navigate(tempPath + TMP_HTML);
            Environment.SpecialFolder.LocalApplicationData.ToString();
        }

        /// <summary>
        ///     Creates an empty xml file at a specified location
        /// </summary>
        /// <param name="filePath"></param>
        void CreateBlankXmlFile(string filePath)
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(filePath, System.Text.Encoding.UTF8);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            xmlWriter.WriteStartElement("Root");
            xmlWriter.Close();
        }

        /// <summary>
        ///     Shows the current xml file in editable text box control
        /// </summary>
        private void displayInEditor()
        {
            this.brwsrInput.Hide();
            this.txtEditor.Show();
            this.txtEditor.Text = File.ReadAllText(currentInputXmlPath);
        }

        /// <summary>
        ///     Saves the contents of the text editor to current xml file
        ///     and displays the saved file in the browser control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditSave_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                TextBox txtBox = this.txtEditor;
                if (btn.Text == "Edit")
                {
                    // Enter into edit mode
                    btn.Text = "Save";
                    this.brwsrInput.Hide();
                    displayInEditor();
                    txtBox.Show();
                }
                else
                {
                    // Save the contents of the editor to file
                    // and return to browser view mode
                    btn.Text = "Edit";
                    File.WriteAllText(currentInputXmlPath, this.txtEditor.Text);
                    this.txtEditor.Hide();
                    this.brwsrInput.Show();
                    displayInBrowser(currentInputXmlPath, brwsrInput);
                }
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        /// <summary>
        ///     Displays the results
        /// </summary>
        void ShowResultsTab(TabPage tabToSelect)
        {
            TabControl outputTabControl = this.tabControlOutputObjects;
            this.tabControlTop.SelectedTab = tabResults;
            if (outputTabControl.SelectedTab.Equals(tabToSelect))
            {
                outputTabControl.DeselectTab(tabToSelect);
            }
            this.btnGridXml.Text = "Grid";
            //this.btnNext.Top = this.btnPrev.Top = this.btnGridXml.Top = brwsrResults.Bottom + 10;
            outputTabControl.SelectedTab = tabToSelect;
            brwsrResults.Show();
        }

        private void tabControlOutputObjects_Selected(object sender, TabControlEventArgs e)
        {
            try
            {
                string selectedTab = ((TabControl)sender).SelectedTab.Text;

                switch (selectedTab)
                {
                    case "HitlistInfo":
                        currentResutlXmlPath = HITLISTINFO_FILE;
                        EnablePagingButtons(false);
                        break;
                    case "DataSet":
                        currentResutlXmlPath = DATASET_FILE;
                        EnablePagingButtons(true);
                        break;

                    case "PagingInfo":
                        currentResutlXmlPath = PAGINGINFO_OUTPUT_FILE;
                        EnablePagingButtons(false);
                        break;
                }

                currentResutlXmlPath = tempPath + currentResutlXmlPath;

                this.btnGridXml.Text = "Grid";
                //this.btnPrev.Top = this.btnNext.Top = this.btnGridXml.Top = brwsrResults.Bottom + 10;

                displayInBrowser(currentResutlXmlPath, brwsrResults);
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        private void EnablePagingButtons(bool p)
        {
            this.btnPrev.Enabled = p;
            this.btnNext.Enabled = p;
        }

        private void ClearResultGrids()
        {
            // Remove any grid controls from the Results tab
            foreach (Control ctl in tabResults.Controls)
            {
                if (ctl.ToString() == "System.Windows.Forms.DataGridView")
                {
                    tabResults.Controls.Remove(ctl);
                }
            }
        }

        #endregion

        #region Build Input objects from xml files

        /// <summary>
        ///     Builds a COE Dataview object from xml file
        /// </summary>
        /// <param name="dataView"></param>
        private void BuildDataViewFromXML(ref COEDataView dataView)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(tempPath + DATAVIEW_FILE);
            dataView = new COEDataView(doc);

        }

        /// <summary>
        ///     Builds a SearchCriteria object from xml file
        /// </summary>
        /// <param name="searchCriteria"></param>
        private void BuildSearchCriteriaFromXML(ref SearchCriteria searchCriteria)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(tempPath + SEARCHCRITERIA_FILE);
            try
            {
                searchCriteria = new SearchCriteria(doc);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        ///     Builds a ResultsCriteria object from xml file
        /// </summary>
        /// <param name="resultsCriteria"></param>
        private void BuildResultsCriteriaFromXML(ref ResultsCriteria resultsCriteria)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(tempPath + RESULTCRITERIA_FILE);
            resultsCriteria = new ResultsCriteria(doc);
        }

        /// <summary>
        ///     Builds a PagingInfo object from xml file
        /// </summary>
        /// <param name="pagingInfo"></param>
        private void BuildPagingInfoFromXML(ref PagingInfo pagingInfo)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(tempPath + PAGINGINFO_FILE);
            pagingInfo = new PagingInfo(doc);
        }

        #endregion

        #region Test SearchService Methods

        /// <summary>
        ///     Button click event for Testing the GetHitlist method.
        ///     Uses the persisted SecurityInfo, Dataview, and SearchCriteria xml files as imput.
        ///     Persists the returned HitlistInfo object to xml file.
        ///     Navigates to the Results tab to display the persisted results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestGetHitlist_Click(object sender, EventArgs e)
        {
            GetHitList(false);
        }

        private void btnParcialHitList_Click(object sender, EventArgs e)
        {
            GetHitList(true);
        }

        private void GetHitList(bool partial)
        {
            _isPartialSearch = partial;

            ClearResultsXml();

            // Build DataView
            dataView = new COEDataView();
            BuildDataViewFromXML(ref dataView);

            // Build SearchCriteria
            searchCriteria = new SearchCriteria();
            BuildSearchCriteriaFromXML(ref searchCriteria);

            try
            {
                this.Cursor = Cursors.WaitCursor;
                hitlistInfo = new HitListInfo();
                if (partial)
                    hitlistInfo = _coeSearch.GetPartialHitList(searchCriteria, dataView);
                else
                    hitlistInfo = _coeSearch.GetHitList(searchCriteria, dataView);
                SaveHitlistInfoToXml(hitlistInfo);

                // Save a blank dataset
                DataSet dataset = new DataSet();
                SaveDataSetToXml(dataset);

                // Save a blank PagingInfo
                pagingInfo = new PagingInfo();
                SavePagingInfoToXml(pagingInfo);

                ShowResultsTab(tabResultsHitlistInfo);
                if (partial)
                    StartUpdatingThread();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        ///     Button click event for Testing the GetData method.
        ///     Uses the persisted SecurityInfo, Dataview, ResultsCriteria, and PagingInfo xml files as imput.
        ///     Persists the returned DataSet object to xml files.
        ///     Navigates to the Results tab to display the persisted results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestGetData_Click(object sender, EventArgs e)
        {
            ClearResultsXml();

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
                // Save a blank hitlistInfo object
                hitlistInfo = new HitListInfo();
                SaveHitlistInfoToXml(hitlistInfo);
                ShowResultsTab(tabResultsDataSet);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        ///     Button click event for Testing the DoSearch method.
        ///     Uses the persisted SecurityInfo, Dataview, SearchCriteria, ResultsCriteria, and PagingInfo xml files as imput.
        ///     Persists the returned HitlistInfo and DataSet objects to xml files.
        ///     Navigates to the Results tab to display the persisted results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestDoSearch_Click(object sender, EventArgs e)
        {
            DoSearch(false);
        }

        private void btnParcialSearchTest_Click(object sender, EventArgs e)
        {
            DoSearch(true);
        }

        private void DoSearch(bool partial)
        {
            _coeLog.LogStart(string.Empty, 4);
            _isPartialSearch = partial;

            ClearResultsXml();

            // Build DataView
            dataView = new COEDataView();
            BuildDataViewFromXML(ref dataView);

            // Build SearchCriteria
            searchCriteria = new SearchCriteria();
            BuildSearchCriteriaFromXML(ref searchCriteria);

            // Build ResultsCriteria
            resultsCriteria = new ResultsCriteria();
            BuildResultsCriteriaFromXML(ref resultsCriteria);

            //PagingInfo
            pagingInfo = new PagingInfo();
            BuildPagingInfoFromXML(ref pagingInfo);

            try
            {
                this.Cursor = Cursors.WaitCursor;
                searchResponse = null;
                if (checkBox1.Checked == true)
                {
                    //this means that a browser of the basetable will be done
                    searchCriteria = null;
                }

                if (partial)
                    searchResponse = _coeSearch.DoPartialSearch(searchCriteria, resultsCriteria, pagingInfo, dataView);
                else
                    searchResponse = _coeSearch.DoSearch(searchCriteria, resultsCriteria, pagingInfo, dataView);

                this.pagingInfo = searchResponse.PagingInfo;
                SaveHitlistInfoToXml((HitListInfo)searchResponse.HitListInfo);
                SaveDataSetToXml((DataSet)searchResponse.ResultsDataSet);
                SavePagingInfoToXml((PagingInfo)searchResponse.PagingInfo);
                ShowResultsTab(tabResultsDataSet);

                if (partial)
                    StartUpdatingThread();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
            _coeLog.LogEnd(string.Empty, 4);
        }

        /// <summary>
        ///     Button click event for Testing the GetExactRecordCount method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            ClearResultsXml();

            // Build DataView
            dataView = new COEDataView();
            BuildDataViewFromXML(ref dataView);
            try
            {
                int recordCount = _coeSearch.GetExactRecordCount(dataView);

                MessageBox.Show("Table_" + dataView.Basetable + " has exactly " + recordCount + " records");
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
        private void ClearResultsXml()
        {
            CreateBlankXmlFile(tempPath + DATASET_FILE);
            CreateBlankXmlFile(tempPath + HITLISTINFO_FILE);
            CreateBlankXmlFile(tempPath + PAGINGINFO_OUTPUT_FILE);
        }

        #endregion
        #region Test ExportService Methods

        private void buttonTestDoExport_Click(object sender, EventArgs e)
        {
            if (ExportTypesList.SelectedItem.ToString() != "...select a export format")
            {
                _coeLog.LogStart(string.Empty, 4);
                ClearResultsXml();

                // Build DataView
                dataView = new COEDataView();
                BuildDataViewFromXML(ref dataView);

                // Build SearchCriteria
                searchCriteria = new SearchCriteria();
                BuildSearchCriteriaFromXML(ref searchCriteria);

                // Build ResultsCriteria
                resultsCriteria = new ResultsCriteria();
                BuildResultsCriteriaFromXML(ref resultsCriteria);

                //PagingInfo
                pagingInfo = new PagingInfo();
                BuildPagingInfoFromXML(ref pagingInfo);

                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    searchResponse = null;
                    if (checkBox1.Checked == true)
                    {
                        //this means that a browser of the basetable will be done
                        searchCriteria = null;
                    }

                    searchResponse = _coeSearch.DoSearch(searchCriteria, resultsCriteria, pagingInfo, dataView);

                    this.pagingInfo = searchResponse.PagingInfo;
                    SaveHitlistInfoToXml((HitListInfo)searchResponse.HitListInfo);
                    SaveDataSetToXml((DataSet)searchResponse.ResultsDataSet);
                    SavePagingInfoToXml((PagingInfo)searchResponse.PagingInfo);
                    string exportType = ExportTypesList.SelectedItem.ToString();
                    string ExportString = _coeExport.GetData(resultsCriteria, pagingInfo, dataView, exportType);
                    DisplayExport(ExportString);

                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
                _coeLog.LogEnd(string.Empty, 4);
            }
            else
            {
                MessageBox.Show("you must select an export format to test");
            }
        }
        #endregion

        #region Persist Output objects to xml files
        void SaveHitlistInfoToXml(HitListInfo hitlistInfo)
        {

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(hitlistInfo.ToString());
                using (XmlWriter w = XmlWriter.Create(tempPath + HITLISTINFO_FILE))
                {
                    doc.WriteTo(w);
                    w.Flush();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        void DisplayExport(String exportString)
        {
            try
            {
                currentExportPath = tempPath + EXPORT_FILE;
                using (StreamWriter sw = File.CreateText(currentExportPath))
                {
                    sw.Write(exportString);
                }
                ExportResults myExport = new ExportResults(tempPath + EXPORT_FILE);
                myExport.ShowDialog();

            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        void SavePagingInfoToXml(PagingInfo pagingInfo)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(pagingInfo.ToString());
                using (XmlWriter w = XmlWriter.Create(tempPath + PAGINGINFO_OUTPUT_FILE))
                {
                    doc.WriteTo(w);
                    w.Flush();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        void SaveDataSetToXml(DataSet dataSet)
        {
            try
            {
                dataSet.WriteXml(tempPath + DATASET_FILE);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        #endregion

        #region Error Handling Routines
        void HandleException(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        #endregion

        private void btnGridXml_Click(object sender, EventArgs e)
        {
            try
            {
                ClearResultGrids();

                if (((Button)sender).Text == "Grid")
                {
                    this.brwsrResults.Hide();
                    if (this.tabControlOutputObjects.SelectedTab.Text == "DataSet")
                    {
                        EnablePagingButtons(true);
                        this.btnPrev.Visible = true;
                        this.btnNext.Visible = true;
                    }

                    // Read the Dataset from Xml
                    DataSet ds = new DataSet();
                    FileStream fs = new FileStream(currentResutlXmlPath, FileMode.Open, FileAccess.Read);
                    ds.ReadXml(fs);
                    fs.Close();

                    // Programmatically create one grid per table in the Dataset

                    grids = new List<DataGridView>();
                    BindGrid(ds);
                    this.btnGridXml.Text = "Xml";
                    /*if (grids.Count > 0)
                        this.btnNext.Top = this.btnPrev.Top = this.btnGridXml.Top = grids[grids.Count - 1].Bottom + 10;*/
                }
                else
                {
                    this.btnPrev.Enabled = false;
                    this.btnPrev.Visible = false;
                    this.btnNext.Enabled = false;
                    this.btnNext.Visible = false;

                    this.btnGridXml.Text = "Grid";
                    //this.btnPrev.Top = this.btnNext.Top = this.btnGridXml.Top = brwsrResults.Bottom + 10;
                    ShowResultsTab(tabControlOutputObjects.SelectedTab);
                }
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        private void BindGrid(DataSet ds)
        {
            int gridTop = this.tabControlOutputObjects.Bottom;
            int gridLeft = this.tabControlOutputObjects.Left;
            int gridWidth = this.tabControlOutputObjects.Width;
            int gridHeight = 200;
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                if (grids.Count < ds.Tables.Count)
                {
                    grids.Add(null);
                }
                if (grids[i] == null)
                    grids[i] = new DataGridView();
                grids[i].AllowUserToAddRows = false;
                grids[i].AllowUserToDeleteRows = false;
                grids[i].ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                grids[i].Location = new System.Drawing.Point(gridLeft, gridTop);
                grids[i].Name = "grid";
                grids[i].ReadOnly = true;
                grids[i].Size = new System.Drawing.Size(gridWidth, gridHeight);
                gridTop = gridTop + gridHeight + 20;
                grids[i].DataSource = ds.Tables[i];
                this.tabResults.Controls.Add(grids[i]);
            }
        }



        /// <summary>
        ///      Bring up a dialog to chose a folder path in which to save the
        ///      imput objects to file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveInput_Click(object sender, EventArgs e)
        {
            try
            {
                this.folderBrowserDialog1.Description = "Select the directory where you want to save the Input Data objects.";

                // Allow the user to create new files via the FolderBrowserDialog.
                this.folderBrowserDialog1.ShowNewFolderButton = true;

                // Default to MyDocuments
                this.folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
                this.folderBrowserDialog1.SelectedPath = currentPath;

                // Show the FolderBrowserDialog.
                DialogResult result = folderBrowserDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {

                    currentPath = folderBrowserDialog1.SelectedPath;
                    SaveInputObjectsToFiles(currentPath);
                }
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }


        private void SaveInputObjectsToFiles(string targetFolderName)
        {
            // we simply copy the contents of the xml folder to the target folder
            DirectoryInfo di = new DirectoryInfo(tempPath);
            FileInfo[] fi = di.GetFiles("*.xml", SearchOption.TopDirectoryOnly);
            foreach (FileInfo f in fi)
            {
                f.CopyTo(targetFolderName + @"\" + f.Name, true);
            }
        }

        private void btnLoadImput_Click(object sender, EventArgs e)
        {
            try
            {
                this.folderBrowserDialog1.Description = "Select the directory from where you want to load the Input Data objects.";

                this.folderBrowserDialog1.ShowNewFolderButton = false;

                // Default to Application XML path
                this.folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
                this.folderBrowserDialog1.SelectedPath = currentPath;
                DialogResult result = folderBrowserDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    currentPath = folderBrowserDialog1.SelectedPath;
                    LoadInputObjectsFromFiles(currentPath);
                }
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        private void LoadInputObjectsFromFiles(string sourceFolderName)
        {
            // we simply copy the contents of the sourceFolder folder to the xml folder
            DirectoryInfo di = new DirectoryInfo(sourceFolderName);
            FileInfo[] fi = di.GetFiles("*.xml", SearchOption.TopDirectoryOnly);
            foreach (FileInfo f in fi)
            {
                bool readOnly = f.IsReadOnly;
                f.IsReadOnly = false;
                f.CopyTo(tempPath + f.Name, true);
                f.IsReadOnly = readOnly;
            }

            LoadAppNameFromFile();

            // Re-select the selected tab to reflect the loaded file.
            TabPage currentTab = tabControlInputObjects.SelectedTab;
            tabControlInputObjects.DeselectTab(currentTab);
            tabControlInputObjects.SelectedTab = currentTab;
        }

        private void LoadAppNameFromFile()
        {
            string appNameFilePath = tempPath + APPNAME_FILE;
            if (File.Exists(appNameFilePath))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(appNameFilePath);
                string appName = xml.DocumentElement.InnerText;
                txtAppName.Text = appName;
            }
            else
            {
                txtAppName.Text = "";
            }
        }

        private void btnSaveOutput_Click(object sender, EventArgs e)
        {
            try
            {
                this.folderBrowserDialog1.Description = "Select the directory where you want to save the Output Data objects.";

                // Allow the user to create new files via the FolderBrowserDialog.
                this.folderBrowserDialog1.ShowNewFolderButton = true;

                // Default to MyDocuments
                this.folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
                this.folderBrowserDialog1.SelectedPath = currentPath;



                // Show the FolderBrowserDialog.
                DialogResult result = folderBrowserDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    currentPath = folderBrowserDialog1.SelectedPath;
                    SaveOutputObjectsToFiles(currentPath);
                }
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        private void SaveOutputObjectsToFiles(string targetFolderName)
        {
            // we simply copy the contents of the resutls folder to the target folder
            DirectoryInfo di = new DirectoryInfo(tempPath);
            FileInfo[] fi = di.GetFiles("*.xml", SearchOption.TopDirectoryOnly);
            foreach (FileInfo f in fi)
            {
                f.CopyTo(targetFolderName + @"\" + f.Name, true);
            }
        }

        private void tabControlTop_Selected(object sender, TabControlEventArgs e)
        {
            TabPage selectedTab = ((TabControl)sender).SelectedTab;

            if (selectedTab.Text == "Results")
            {
                //if (tabControlOutputObjects.SelectedTab == tabResultsHitlistInfo)
                //{
                //    tabControlOutputObjects.DeselectTab(tabResultsHitlistInfo);
                //}
                //tabControlOutputObjects.SelectedTab = tabResultsHitlistInfo;

                currentResutlXmlPath = tempPath + HITLISTINFO_FILE;
                displayInBrowser(currentResutlXmlPath, brwsrResults);
            }
        }



        private void txtAppName_Leave(object sender, EventArgs e)
        {
            string appName = (string)((TextBox)sender).Text;
            XmlTextWriter xmlWriter = new XmlTextWriter(tempPath + APPNAME_FILE, System.Text.Encoding.UTF8);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            xmlWriter.WriteStartElement("appName");
            xmlWriter.WriteValue(appName);
            xmlWriter.Close();
        }

        private void btnGridNext_Click(object sender, EventArgs e)
        {
            this.pagingInfo.Start += this.pagingInfo.RecordCount;
            if (resultsCriteria.Tables.Count <= 0)
                BuildResultsCriteriaFromXML(ref resultsCriteria);

            if (searchResponse != null)
                this.pagingInfo.HitListID = searchResponse.HitListInfo.HitListID;

            try
            {
                DataSet dataset = null;
                dataset = _coeSearch.GetData(resultsCriteria, pagingInfo, dataView);

                BindGrid(dataset);
                SavePagingInfoToXml(pagingInfo);
                //this.Refresh();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void btnGridPrev_Click(object sender, EventArgs e)
        {
            this.pagingInfo.Start -= this.pagingInfo.RecordCount;
            if (resultsCriteria.Tables.Count <= 0)
                BuildResultsCriteriaFromXML(ref resultsCriteria);

            if (searchResponse != null)
                this.pagingInfo.HitListID = searchResponse.HitListInfo.HitListID;

            try
            {
                DataSet dataset = null;
                dataset = _coeSearch.GetData(resultsCriteria, pagingInfo, dataView);

                BindGrid(dataset);
                SavePagingInfoToXml(pagingInfo);
                //this.Refresh();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void logButton_Click(object sender, EventArgs e)
        {
            DataTable dt = _coeLog.GetLogFileAsDataTable();
            LogFile myLogFile = new LogFile(dt);
            myLogFile.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _coeLog.ClearLogFile();
        }


        private void StartUpdatingThread()
        {
            System.ComponentModel.BackgroundWorker threadedHitList = new System.ComponentModel.BackgroundWorker();
            threadedHitList.DoWork += new DoWorkEventHandler(CheckForHitListUpdates);
            threadedHitList.RunWorkerCompleted += new RunWorkerCompletedEventHandler(threadedHitList_RunWorkerCompleted);
            threadedHitList.RunWorkerAsync();
        }

        void threadedHitList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string xmlPath = tempPath + HITLISTINFO_FILE;
            displayInBrowser(xmlPath, this.brwsrResults);
        }

        void CheckForHitListUpdates(object sender, DoWorkEventArgs e)
        {
            try
            {
                System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
                while (_isPartialSearch)
                {
                    if (searchResponse != null)
                        hitlistInfo = _coeSearch.GetHitListProgress(searchResponse.HitListInfo, this.dataView);
                    else
                        hitlistInfo = _coeSearch.GetHitListProgress(hitlistInfo, this.dataView);

                    if (hitlistInfo.RecordCount == hitlistInfo.CurrentRecordCount)
                        _isPartialSearch = false;

                    SaveHitlistInfoToXml(hitlistInfo);
                    System.Threading.Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }
        }

        private void Configuration_Click(object sender, EventArgs e)
        {
            ClientConfiguration clientConfig = new ClientConfiguration();
            clientConfig.LoggingEnabledCallback += new LoggingEnabledDelegate(this.LoggingEnabledCallbackFn);

            clientConfig.ShowDialog();
        }


        private void LoggingEnabledCallbackFn(bool isActive)
        {
            this.logButton.Enabled = isActive;
        }
    }
    public delegate void LoggingEnabledDelegate(bool isActive);

}
