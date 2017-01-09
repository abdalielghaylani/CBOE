using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.RegistrationAdmin.Services;
using System.Xml;
using System.IO;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Reflection;
using Resources;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COETableEditorService;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.RegistrationAdmin;

namespace PerkinElmer.COE.Registration.Server.Forms.RegistrationAdmin.ContentArea
{
    public partial class ImportExportCustom : GUIShellPage
    {
        #region Variables

        RegistrationMaster _masterPage = null;

        private const string COEFORMSFOLDERNAME = "COEForms";
        private const string COEDATAVIEWSFOLDERNAME = "COEDataViews";
        private const string COETABLESFORLDERNAME = "COETables";
        private const string COEOBJECTCONFIGFILENAME = "COEObjectConfig.xml";
        private const string CONFIGSETTINGSFILENAME = "ConfigurationSettings.xml";
        private const string IMPORTFILESPATH = "\\Config\\default";
        private const string EXPORTFILESPATH = "\\COERegistrationExportFiles\\";
        private const string IMPORTINIFILEPATH = "\\Config\\COE10Migration";
        private const string EXCEPTIONCODE = "LocalImportError ";
        private const string FIXEDINSTALLPATH = "Registration";
        private const string FILE_SEARCH_PATTERN = "*.xml";

        private string _currentExportDir;

        #endregion

        #region Page Load
        /// <summary>
        /// Page load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    SetControlsAttributtes();
                    FillTables();
                    SetActionFromURL();
                }
                _masterPage.SetDefaultAction(this.LinkButtonGoToMain.UniqueID);
                _masterPage.MakeCtrlShowProgressModal(this.ButtonLocalImport.ClientID, "Importing configuration...", string.Empty, true);
                _masterPage.MakeCtrlShowProgressModal(this.ButtonExport.ClientID, "Exporting configuration...", string.Empty, true);
                //_masterPage.MakeCtrlShowProgressModal(this.ButtonIniFile.ClientID, "Importing configuration...", string.Empty, true);
                this.ButtonLocalImport.OnClientClick = string.Format("if(document.getElementById('{0}').checked) return confirm('{1}');", this.ForceImportCheckBox.ClientID, Resources.Resource.ConfirmForceImport_Alert_Text);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        /// <summary>
        /// Overriden. Adds client side scritps
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
          
            string script = @"function selectTables()
                       {
                            var cbl = document.getElementById('" + this.CheckBoxListTables.ClientID + @"');
                            var rbAll = document.getElementById('" + this.RadioButtonListTables.ClientID + @"_0');                                                        
                            if(rbAll.checked)
                            {
                                for(i=0; i<" + this.CheckBoxListTables.Items.Count + @"; i++)
                                {
                                    document.getElementById(cbl.id + '_' + i.toString()).checked = true;
                                }
                            }
                            else
                            {
                                for(i=0; i<" + this.CheckBoxListTables.Items.Count + @"; i++)
                                {
                                    document.getElementById(cbl.id + '_' + i.toString()).checked = false;
                                }
                            }                                                                                                        
                       }";
            if (!this.Page.ClientScript.IsStartupScriptRegistered(typeof(ImportExportCustom), "SelectTables"))
                this.Page.ClientScript.RegisterStartupScript(typeof(ImportExportCustom), "SelectTables", script, true);
        }

        /// <summary>
        /// Overriden. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            #region Page Settings

            // To make easier to read the source code.
            if (this.Master is RegistrationMaster)
            {
                _masterPage = (RegistrationMaster)this.Master;
                _masterPage.ShowLeftPanel = false;
            }

            #endregion
            base.OnInit(e);
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }
        #endregion

        #region Properties

        private ConfigurationRegistryRecord ConfigurationRegistryRecord
        {
            get
            {
                if (Session["ConfigurationRegistryRecord"] == null)
                {
                    Session["ConfigurationRegistryRecord"] = ConfigurationRegistryRecord.NewConfigurationRegistryRecord();
                    return (ConfigurationRegistryRecord)Session["ConfigurationRegistryRecord"];
                }
                else
                    return (ConfigurationRegistryRecord)Session["ConfigurationRegistryRecord"];
            }
            set
            {
                Session["ConfigurationRegistryRecord"] = value;
            }
        }

        private string AppDrive
        {
            get
            {
                return HttpContext.Current.Server.MapPath(string.Empty).Remove(2);
            }
        }

        private string AppRootInstallPath
        {
            get { return Server.MapPath(string.Empty).Remove(Server.MapPath(string.Empty).IndexOf(FIXEDINSTALLPATH) + FIXEDINSTALLPATH.Length); }
        }

        private string CurrentDate
        {
            get { return DateTime.Now.ToString("yy-MM-dd HH_mm_ss"); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method to display confirmation messages in the top of the page (MessagesAreaUC)
        /// </summary>
        /// <param name="messageToDisplay">The text to display</param>
        private void ShowConfirmationMessage(string messageToDisplay)
        {
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
            this.MessagesAreaUserControl.AreaText = messageToDisplay;
            this.MessagesAreaUserControl.Visible = true;
            RegUtilities.WriteToRegLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Sets various attributes to the server controls
        /// </summary>
        protected override void SetControlsAttributtes()
        {
            this.Page.Title = Resource.Brand + " - " + Resource.RegAdminHome_Page_Title + " - " + Resource.ImportExport_Page_Title;
            this.ButtonExport.Text = Resources.Resource.Export_Button_Text;
            //this.LabelInfo.Text = Resources.Resource.ImportInfo_Label_Text;
            this.LabelInfo2.Text = Resources.Resource.ExportInfo_Label_Text;
            //this.LabelLegen.Text = Resources.Resource.ImportCustomization_Label_Text;
            this.LabelLegen2.Text = Resources.Resource.ExportCustomization_Label_Text;
            //this.ButtonIniFile.Text = Resources.Resource.Import_Button_Text;
            this.LabelLegen3.Text = Resources.Resource.TablesExportation_Legen_Text;
            this.LabelImporExportInfo.Text = Resource.ImportExport_Page_Title;
            //this.LabelImportIni.Text = Resource.ImportIni_Label_Text;
            this.RadioButtonListTables.Items.Add("All");
            this.RadioButtonListTables.Items.Add("None");
            this.RadioButtonListTables.Items[0].Text = "Select All";
            this.RadioButtonListTables.Items[1].Text = "Select None";
            this.RadioButtonListTables.Items[0].Attributes.Add("onclick", "selectTables()");
            this.RadioButtonListTables.Items[1].Attributes.Add("onclick", "selectTables()");
            this.LabelLocalImportInfo.Text = Resource.LocalImportLableInfo_Label_Text;
            this.LabelLocalImportLegend.Text = Resource.LocalImportLegen_Label_Text;
            this.ButtonLocalImport.Text = Resource.Import_Button_Text;
            this.TextBoxLocalImport.Text = AppRootInstallPath + IMPORTFILESPATH;
            this.LabelLocalImportUrl.Text = Resource.LocalImportExportUrl_Label_Text;
            this.LabelLocalExportUrl.Text = Resource.LocalImportExportUrl_Label_Text;
            this.TextBoxLocalExport.Text = AppDrive + EXPORTFILESPATH + CurrentDate;
            //this.LabelIniServerPath.Text = Resource.LocalImportExportUrl_Label_Text;
            //this.TextBoxIniServerPath.Text = AppRootInstallPath + IMPORTINIFILEPATH;
            this.ForceImportCheckBox.Text = Resource.ForceImport_Label_Text;
        }

        private void SetActionFromURL()
        {
            string mustImportConfig = Request[Constants.MustImportCustomization_URLParameter];

            if (mustImportConfig == "YES")
            {
                this.ExportGroup.Visible = false;
                //this.INIGroup.Visible = false;
                this.ShowConfirmationMessage(Resource.RegAdmin_MustImportCustomizationInfo_Text);
            }
        }

        private void FillTables()
        {
            string[] tableList = ConfigurationRegistryRecord.GetTableNames;

            foreach (string str in tableList)
            {
                this.CheckBoxListTables.Items.Add(str);
            }
            for (int i = 0; i < CheckBoxListTables.Items.Count; i++)
            {
                this.CheckBoxListTables.Items[i].Selected = true;
            }
        }

        private List<string> GetSelectedTableNames()
        {
            List<string> tableNameList = new List<string>();
            string tableName = string.Empty;
            for (int i = 0; i < CheckBoxListTables.Items.Count; i++)
            {
                tableName = CheckBoxListTables.Items[i].Text;
                if (CheckBoxListTables.Items[i].Selected == true)
                {
                    tableNameList.Add(tableName);
                }
            }

            return tableNameList;
        }

        private void ImportCustomization()
        {
            MessagesAreaUserControl.Visible = false;
            ConfigurationRegistryRecord.ImportCustomization(AppRootInstallPath, this.TextBoxLocalImport.Text, this.ForceImportCheckBox.Checked);

            ShowConfirmationMessage(Resource.ImportConfigurationSuccess_Message);
        }

        private void ExportConfigurations()
        {
            try
            {
                _currentExportDir = this.TextBoxLocalExport.Text;

                Directory.CreateDirectory(_currentExportDir);

                this.ExportConfigurationSettings();

                this.ExportCustomProperties();

                this.ExportForms();

                this.ExportDataViews();

                this.ExportTables();

                ShowConfirmationMessage(Resource.ExportSuccess_Message);

            }
            catch (Exception e)
            {
                _masterPage.DisplayErrorMessage(e.Message, false);
            }
        }

        private void SendFileToClient(string xmlContent, string fileSuffix)
        {
            try
            {
                string filepath = Path.GetTempPath();
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(xmlContent);

                xml.Save(filepath + "\\" + User.Identity + fileSuffix);
                filepath = filepath + "\\" + User.Identity + fileSuffix;
                FileInfo file = new FileInfo(filepath);

                if (file.Exists)
                {
                    Response.ClearContent();
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                    Response.AddHeader("Content-Length", file.Length.ToString());
                    Response.ContentType = "text/xml";
                    Response.TransmitFile(file.FullName);
                }
                Response.End();
            }
            catch (Exception e)
            {
                _masterPage.DisplayErrorMessage(e.Message, false);
            }
        }

        protected void ImportIniFile()
        {
            //string iniFilePath = !string.IsNullOrEmpty(this.TextBoxIniServerPath.Text) ? this.TextBoxIniServerPath.Text : AppRootInstallPath + IMPORTINIFILEPATH;

            //string[] files = null;
            //Stream streamCfserver = null;
            //Stream streamReg = null;

            //try
            //{
            //    files = Directory.GetFiles(iniFilePath, "*.ini");
            //    bool iniProcessed = false;

            //    if (files.Length > 0)
            //    {
            //        foreach (string file in files)
            //        {
            //            if (file.EndsWith("cfserver.ini", true, null))
            //            {
            //                streamCfserver = File.OpenRead(file);
            //            }
            //            if (file.EndsWith("reg.ini", true, null))
            //            {
            //                streamReg = File.OpenRead(file);
            //            }
            //        }

            //        if (streamCfserver == null || streamReg == null)
            //            _masterPage.DisplayErrorMessage(Resource.EmptyIniFileFolder_Message, false);
            //        else
            //        {
            //            try
            //            {
            //                this.ConfigurationRegistryRecord.ImportIniFile(streamCfserver, streamReg);
            //                iniProcessed = true;
            //            }
            //            catch (Exception e)
            //            {
            //                _masterPage.DisplayErrorMessage(e.Message, true);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        _masterPage.DisplayErrorMessage(Resource.EmptyIniFileFolder_Message, false);
            //    }

            //    if (iniProcessed)
            //        ShowConfirmationMessage(Resource.IniFileImportationSuccess_Message);
            //}
            //catch
            //{
            //    _masterPage.DisplayErrorMessage(Resource.IniFilePath_Error_Message, false);
            //}

        }

        private void ExportConfigurationSettings()
        {
            XmlDocument confSettingsXml = new XmlDocument();
            confSettingsXml.AppendChild(confSettingsXml.CreateElement("configurationSettings"));
            confSettingsXml.FirstChild.InnerXml = this.ConfigurationRegistryRecord.GetConfigurationSettingsXml();
            this.WriteFile(_currentExportDir, CONFIGSETTINGSFILENAME, true, confSettingsXml.OuterXml);
        }

        private void ExportCustomProperties()
        {
            WriteFile(_currentExportDir, COEOBJECTCONFIGFILENAME, true, this.ConfigurationRegistryRecord.ExportCustomizedProperties());
        }

        private void ExportForms()
        {
            string formsDir = _currentExportDir + "\\" + COEFORMSFOLDERNAME;
            Directory.CreateDirectory(formsDir);

            foreach (COEFormBO coeFormBO in COEFormBOList.GetCOEFormBOList(null, null, COEAppName.Get(),null, true))
            {
                COEFormBO toExport = COEFormBO.Get(coeFormBO.ID);
                WriteFile(formsDir, coeFormBO.ID.ToString(), true, toExport.COEFormGroup.ToString());
            }
        }

        private void ExportDataViews()
        {
            string dataViewDir = _currentExportDir + "\\" + COEDATAVIEWSFOLDERNAME;
            Directory.CreateDirectory(dataViewDir);

            foreach (COEDataViewBO coeDV in COEDataViewBOList.GetDataviewListForApplication(COEAppName.Get()))
            {
                WriteFile(dataViewDir, coeDV.ID.ToString(), true, coeDV.COEDataView.ToString());
            }
        }

        private void ExportTables()
        {
            string tablesDir = _currentExportDir + "\\" + COETABLESFORLDERNAME;
            Directory.CreateDirectory(tablesDir);
            int tableNamePrefix = 1;
            foreach (string tableName in this.GetSelectedTableNames())
            {
                WriteFile(tablesDir, string.Format("{0:000}", tableNamePrefix++) + " - " + tableName, true, ConfigurationRegistryRecord.GetTable(tableName));
            }
        }

        private void WriteFile(string dir, string fileName, bool outputFormatted, string content)
        {
            XmlDocument document = new XmlDocument();
            if (fileName.Contains(".xml"))
            {
                fileName = fileName.Replace(".xml", "");
            }
            using (XmlTextWriter tw = new XmlTextWriter(dir + "\\" + fileName + ".xml", Encoding.UTF8))
            {
                tw.Formatting = outputFormatted ? Formatting.Indented : Formatting.None;

                document.LoadXml(content);
                document.Save(tw);
            }
        }
        #endregion

        #region Event Handlers

        protected void LinkButtonGoToMain_Click(object sender, EventArgs e)
        {
            try
            {
                Server.Transfer(Resource.RegAdmin_URL);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void ButtonExport_Click(object sender, EventArgs e)
        {
            try
            {
                ExportConfigurations();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected void ButtonIniFile_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    this.ImportIniFile();
            //}
            //catch (Exception exception)
            //{
            //    COEExceptionDispatcher.HandleUIException(exception);
            //    _masterPage.DisplayErrorMessage(exception, false);
            //}
        }

        protected void ButtonLocalImport_Click(object sender, EventArgs e)
        {
            try
            {
                this.ImportCustomization();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        #endregion

    }
}
