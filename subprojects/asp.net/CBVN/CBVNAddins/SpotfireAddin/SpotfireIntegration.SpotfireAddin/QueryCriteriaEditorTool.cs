using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spotfire.Dxp.Application.Extension;
using Spotfire.Dxp.Application;
using System.Windows.Forms;
using SpotfireIntegration.Common;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEDataViewService;
using FormWizard;
using Spotfire.Dxp.Framework.ApplicationModel;
using Spotfire.Dxp.Data;
using System.Xml;
using System.IO;
using System.Reflection;
using COEServiceLib;
using Spotfire.Dxp.Framework.Preferences;
using SpotfireIntegration.SpotfireAddin.Properties;
using System.Threading;
using Spotfire.Dxp.Framework.License;

namespace SpotfireIntegration.SpotfireAddin
{
    public class QueryCriteriaEditorTool : CustomTool<AnalysisApplication>
    {

        ResultsCriteriaEditorTool _resultCriteriaEditorTool;
        COEHitList coeHitList = null;
        SearchCriteria _searchCriteria;
        ResultsCriteria _resultsCriteria;
        SelectDataForm form;
        private static object monitorLockObject = new object();
        SpotfireCOEAuthenticationPromptModel loginModel;

        /// <summary>
        /// Initializes an instance of the Datalytix custom tool menu with the custom licensing object information
        /// </summary>
        public QueryCriteriaEditorTool()
            : base(Resources.MENU_TITLE, DatalytixCustomLicense.Functions.QueryCriteriaToolLicense)
        {
            _resultCriteriaEditorTool = new ResultsCriteriaEditorTool();
        }

        protected override void ExecuteCore(AnalysisApplication context)
        {
            if (!Login(context))
                return;
            LaunchForm(context);
        }

        private bool Login(AnalysisApplication context)
        {
            // Get the COEService and make sure we are logged in.
            COEService service = context.GetService<COEService>();
            if (!service.IsAuthenticated)
            {
                bool result = service.LoginModel(out loginModel);

                if (!result)
                {
                    return false;
                }
                else
                {
                    result = false;
                    lock (monitorLockObject)
                    {
                        //Loading Spotfire Preferences values
                        context.SetUserCustomPreferences(service);
                        
                        //display progress window while authenticating on ChemOffice Enterprise
                        context.GetService<ProgressService>().ExecuteWithProgress(FormWizard.Properties.Resources.FORM_TITLE, "Authenticating...",
                            delegate()
                            {
                                try
                                {
                                    if (!service.Login(loginModel))
                                    {                                        
                                        result = false;
                                    }
                                    else
                                    {
                                        result = true;
                                    }
                                }
                                catch
                                {
                                    result = false;
                                }
                                finally
                                {
                                }
                            });
                        if (!result)
                        {
                            MessageBox.Show(Resources.Authentication_Error,
                                FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        Monitor.PulseAll(monitorLockObject);
                        return result;
                    }
                }
            }
            else
            {
                //set login model if already authenticated on server
                loginModel = service.GetLoginModel();
                //get the connection info from persistance storage
                CBOEConnectionPersistance theCBOEConnectionPersistance = context.GetConnInfo();
                if (theCBOEConnectionPersistance != null)
                {
                    loginModel.CurrentMRUEntry = theCBOEConnectionPersistance.TheConnInfo.CurrentMRUEntry;
                }
            }
            return true;
        }

        internal void LaunchForm(AnalysisApplication context)
        {
            _resultsCriteria = null;
            // Get the COEService and make sure we are logged in.
            COEService service = context.GetService<COEService>();

            // Load the DataView.
            COEDataViewBOList dataViewBOList = service.GetDataViews();
            if (dataViewBOList == null || dataViewBOList.Count == 0)
            {
                MessageBox.Show(FormWizard.Properties.Resources.No_Dataviews, FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            COEDataViewBO selectedDataViewBO = null;

            //initialize Search criteria fields order object
            SearchCriteriaFieldOrder searchFieldsOrder = new SearchCriteriaFieldOrder();
            bool filterChildHits = false;

            //variable to determine if the document is not opened from Datalytix
            bool isDocumentFresh = false;
            int oldDataviewId;

            //EditQueryCriteria(context);
            if (_resultCriteriaEditorTool != null && context.Document != null)
            {
                // Find the base table to edit.
                // TODO: Handle the case where a single Document contains multiple ResultsCriteria.
                DataTable baseDataTable = context.Document.Data.Tables.GetCOEBaseTable();
                if (baseDataTable == null)
                {
                    //if base table is missing in the document then the Datalytix is missing the data table properties, 
                    //hence set the boolean value to true instead of warning message and closing the tool
                    isDocumentFresh = true;
                    //close the existing open analysis if it dirty then prompt for save; otherwise close
                    bool isDummyTable = context.Document.Data.Tables.IsDummyAsBaseTable();
                    if (!isDummyTable)
                    {
                        if (context.DocumentMetadata.IsDirty)
                        {
                            string fileName = string.Empty;
                            FileInfo fi = new FileInfo(context.DocumentMetadata.FileNameUsedBySave);
                            fileName = fi.Name;
                            if (MessageBox.Show(string.Format("Do you want to save the changes to '{0}'?", fileName), FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                context.Save();
                            }
                        }
                        //context.Close();
                    }
                }

                //if the document is opened with criteria from Datalytix then access properties of document and assign appropriatly
                if (!isDocumentFresh)
                {
                    try
                    {
                        coeHitList = baseDataTable.GetCOEHitList();
                        //get search criteria fields order from datatable metadata
                        searchFieldsOrder = baseDataTable.GetSearchCriteriaFieldsOrder();
                        filterChildHits = baseDataTable.IsFilterChildHits();
                        //get the connection info from persistance storage
                        CBOEConnectionPersistance theCBOEConnectionPersistance = context.GetConnInfo();
                    }
                    catch (XmlException xEx)
                    {
                        ErrorMessage.ShowDialog(FormWizard.Properties.Resources.FORM_TITLE, Resources.ResultCriteria_Fail, xEx.ToString()); 
                        return;
                    }

                    _resultsCriteria = coeHitList.ResultsCriteria;
                    _searchCriteria = coeHitList.SearchCriteria;

                    selectedDataViewBO = dataViewBOList.FirstOrDefault(p => p.ID == coeHitList.DataViewID);

                    if (selectedDataViewBO == null)
                    {
                        MessageBox.Show(FormWizard.Properties.Resources.Dataview_NotFound, FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }
            }

            if (selectedDataViewBO == null && dataViewBOList.Count > 0)
            {
                selectedDataViewBO = dataViewBOList[0];
            }

            // Workaround:  Since StructureFiler depends on document node, we must have a document present before instanting it
            // in the QueryCriteriaEditor Tool.  Here we create a dummy analysis document with an empty table that will later be
            // removed after adding the real data.
            if (context.Document == null)
            {
                CreateDummyAnalysisDocument(context);
            }

            // Now launch the editor.
            form = new SelectDataForm(context);
            //set framework server version
            form.ServerVersion = service.ServerVersion;
            form.availableDataViews = dataViewBOList.ToArray();
            form.dataViewBO = selectedDataViewBO;
            oldDataviewId = selectedDataViewBO.ID;
            //assign search criteria fields collection to form property
            form.FieldsOrder = searchFieldsOrder.SearchFieldsCollection;
            form.FilterChildHits = filterChildHits;
            form.AllowChangingDataView = true;
            if (_resultsCriteria != null)
                form.resultsCriteria = _resultsCriteria;
            if (_searchCriteria != null)
                form.SearchCriteria = _searchCriteria;
            if (coeHitList != null)
                form.coeHitList = coeHitList;
            else
                form.CBVNFormName = string.Empty;

            // Get the LeadDiscovery installation path
            form.LDInstallationPath = GetLeadDiscoveryFolderPath();

            context.SetUserCustomPreferences(service);
            form.MaxRows = service.TheCOEPreferenceSettings.MaxRows; //Displayed Max Rows settings from Spotfire Preferences on Select Data form.
            form.IsLogOffLinkVisible = !service.TheCOEPreferenceSettings.UseRemoting;

            //form.PassAnalysisApplicationObjectToLDStructureFilter(context);
            DialogResult dialogResult = form.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                return;
            }
            selectedDataViewBO = form.dataViewBO;
            _resultsCriteria = form.resultsCriteria;
            _searchCriteria = form.SearchCriteria;

            bool isDataPopulated = false;
            bool canEndCBVNIntegration = false;
            //JIRA ID:CBVS-313: Checking the validations and alert the user before opening the process window            
            if (context.Document != null && context.Document.Data.Tables.DefaultTableReference.Name.ToLower() != "dummy" && !form.IsRunLastQuery)
            {
                COEHitList oldHitList = null;
                DataTable dataTable = context.Document.Data.Tables.GetCOEBaseTable();
                if (dataTable != null)
                {
                    oldHitList = dataTable.GetCOEHitList();
                    if (oldHitList.DataViewID != selectedDataViewBO.COEDataView.DataViewID || oldHitList.IsSearchCriteriaUpdated(_searchCriteria))
                    {
                        if (System.Windows.Forms.MessageBox.Show(Resources.QueryCriteriaAlertMessage, FormWizard.Properties.Resources.FORM_TITLE, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) != DialogResult.Yes)//Datatable will be modified
                            return;
                        else
                        {
                            canEndCBVNIntegration = true;
                        }
                    }
                }
                else
                {
                    PopulateData(context, selectedDataViewBO, oldDataviewId);
                    context.Document.Transactions.ExecuteTransaction(delegate
                    {
                        
                        string baseTableName = selectedDataViewBO.COEDataView.BaseTableName;
                        TableBO baseTableBo = selectedDataViewBO.DataViewManager.Tables[baseTableName];
                        if (baseTableBo != null)
                        {
                            baseTableName = string.IsNullOrEmpty(baseTableBo.Alias) ? baseTableBo.Name : baseTableBo.Alias;

                            DataTable theDataTable = context.Document.Data.Tables[baseTableName];
                            // Auto-configure the new document and remove the automatically generated cover page.
                            Page newPage = context.Document.Pages.AddNew(selectedDataViewBO.Name);
                            newPage.AutoConfigure(null);
                            Visual visual = newPage.Visuals.AddNew(Spotfire.Dxp.Application.Visuals.VisualTypeIdentifiers.Table);
                            visual.As<Spotfire.Dxp.Application.Visuals.Visualization>().Data.DataTableReference = theDataTable;
                            visual.AutoConfigure();
                            //Setting Row Height to 10 when Structure type or Image type field presents in Result Criteria
                            SetRowHeightOfTableVisualization(context, selectedDataViewBO);
                        }
                    });
                    isDataPopulated = true;
                    canEndCBVNIntegration = true;
                }
            }
            if (!isDataPopulated)
            {
                PopulateData(context, selectedDataViewBO, oldDataviewId);
            }
            if (canEndCBVNIntegration)
            {
                CBVNIntegationHelper.EndIntegrationWithCBVN();
            }
        }

        private void PopulateData(AnalysisApplication context, COEDataViewBO selectedDataViewBO, int oldDataviewId)
        {
            try
            {
                context.GetService<ProgressService>().ExecuteWithProgress(
                    FormWizard.Properties.Resources.FORM_TITLE, "The data tables in Spotfire are being loaded.",
                    delegate
                    {
                        if (form.coeHitList != null)
                        {
                            if (context.UpdateSpotfireDataTable(form.coeHitList, form.HitListInfoId, oldDataviewId, form.FilterChildHits))
                            {
                                //set search criteria fields order and Filter Child Hits to datatable property                                
                                context.UpdateDataTableProperties(form.FieldsOrder, form.FilterChildHits);

                                if (loginModel != null && !string.IsNullOrEmpty(loginModel.CurrentMRUEntry.Server))
                                {
                                    context.SetConnInfo(loginModel.CurrentMRUEntry); //sets the connection information in the document property
                                }
                            }
                        }
                    });


                //Setting Row Height to 10 when Structure type or Image type field presents in Result Criteria
                SetRowHeightOfTableVisualization(context, selectedDataViewBO);
            }
            catch (System.Security.SecurityException sEx)
            {
                ErrorMessage.ShowDialog(FormWizard.Properties.Resources.FORM_TITLE, sEx.Message, sEx.ToString());
            }
            catch (Spotfire.Dxp.Framework.ApplicationModel.ProgressCanceledException)
            {
                //Do nothing if user cancel the progress dialog.
            }
            catch (Spotfire.Dxp.Framework.ApplicationModel.PromptCanceledException)
            {
                //Do nothing if user cancel the match column window.
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("ORA-00942: table or view does not exist") || ex.InnerException.Message.Contains("ORA-00942: table or view does not exist"))
                    ErrorMessage.ShowDialog(FormWizard.Properties.Resources.FORM_TITLE, FormWizard.Properties.Resources.Invalid_Daview, ex.ToString());
                else
                    ErrorMessage.ShowDialog(FormWizard.Properties.Resources.FORM_TITLE, ex.Message, ex.ToString());                
            }
        }

        private void SetRowHeightOfTableVisualization(AnalysisApplication context, COEDataViewBO selectedDataViewBO)
        {
            foreach (Page page in context.Document.Pages)
            {
                foreach (Visual visual in page.Visuals)
                {
                    Spotfire.Dxp.Application.Visuals.TablePlot tablePlot = visual.As<Spotfire.Dxp.Application.Visuals.TablePlot>();
                    if (tablePlot != null)
                    {
                        if (tablePlot.Data.DataTableReference.Properties.HasPropertyValue(Properties.Resources.COETableID_PropertyName))
                        {
                            int tableID = (int)tablePlot.Data.DataTableReference.Properties.GetProperty(Properties.Resources.COETableID_PropertyName);
                            if (IsStructureOrImageField(selectedDataViewBO.COEDataView, form.coeHitList, tableID))
                            {
                                tablePlot.RowHeight = 10;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates an new analysis document with an empty table named dummy
        /// </summary>
        /// <param name="context"></param>
        private static void CreateDummyAnalysisDocument(AnalysisApplication context)
        {
            context.Open(new DummyTableDataSource());
            context.Document.Data.Tables.DefaultTableReference.Name = "dummy";
        }


        /// <summary>
        /// Finds the installation path for LeadDiscovery Addin by checking if the
        /// assembly that implements the Addin has been loaded and obtaining the path
        /// where it was loaded from.
        /// </summary>
        /// <returns></returns>
        private static string GetLeadDiscoveryFolderPath()
        {
            // Check to see if we have already stored the path and return it directly
            if (!String.IsNullOrEmpty(SpotfireCOETableDataSourceAddIn.LDInstallationPath))
                return SpotfireCOETableDataSourceAddIn.LDInstallationPath;  // ===> EXIT POINT

            try
            {
                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (asm.FullName.Contains("Spotfire.Dxp.StructureViewer"))
                    {
                        SpotfireCOETableDataSourceAddIn.LDInstallationPath = Path.GetDirectoryName(asm.Location); //EscapedCodeBase
                        return SpotfireCOETableDataSourceAddIn.LDInstallationPath;
                    }
                }
            }
            catch
            {
                //Do nothing
            }
            return string.Empty;
        }

        /// <summary>
        /// Checks whether to enable the tool menu based on the license available to user/group
        /// </summary>
        /// <param name="context">AnalysisApplication object</param>
        /// <returns>returns true if license is available; otherwise false</returns>
        protected override bool IsEnabledCore(AnalysisApplication context)
        {
            return DatalytixCustomLicense.IsLicensed(context.GetService<LicenseManager>());
        }

        /// <summary>
        /// Method for detecting whether Structure type or Image type field exist in Result Criteria or not.
        /// </summary>
        /// <param name="_coeDataView">DataView information</param>
        /// <param name="_coeHitList">HitList information</param>
        /// <param name="tableID">Table associated with Visualization</param>
        /// <returns>True/False</returns>
        private bool IsStructureOrImageField(COEDataView _coeDataView, SpotfireIntegration.Common.COEHitList _coeHitList, int tableID)
        {
            bool blnStructureFieldExistInResultCriteria = false;
            try
            {
                foreach (ResultsCriteria.ResultsCriteriaTable _resultsCriteriaTable in _coeHitList.ResultsCriteria.Tables)
                {
                    if (_resultsCriteriaTable.Id.Equals(tableID))
                    {
                        COEDataView.DataViewTable dataViewTable = _coeDataView.Tables.getById(_resultsCriteriaTable.Id);

                        foreach (ResultsCriteria.IResultsCriteriaBase _resultsCriteriaBase in _resultsCriteriaTable.Criterias)
                        {
                            if (_resultsCriteriaBase is ResultsCriteria.Field)
                            {
                                int fieldId = ((ResultsCriteria.Field)_resultsCriteriaBase).Id;
                                COEDataView.Field _fieldInfo = dataViewTable.Fields.getById(fieldId);
                                if (_fieldInfo.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE ||
                                    (_fieldInfo.MimeType == COEDataView.MimeTypes.IMAGE_GIF || _fieldInfo.MimeType == COEDataView.MimeTypes.IMAGE_JPEG ||
                                    _fieldInfo.MimeType == COEDataView.MimeTypes.IMAGE_PNG || _fieldInfo.MimeType == COEDataView.MimeTypes.IMAGE_X_WMF))
                                {
                                    blnStructureFieldExistInResultCriteria = true;
                                    break;
                                }
                            }
                        }

                        if (blnStructureFieldExistInResultCriteria)
                        {
                            break;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return blnStructureFieldExistInResultCriteria;
        }

    }

    /// <summary>
    /// Class for custom license
    /// </summary>
    public class DatalytixCustomLicense : CustomLicense
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DatalytixCustomLicense()
        {
        }

        /// <summary>
        /// Method to check if license is available
        /// </summary>
        /// <param name="licenseManager">licenseManager instance</param>
        /// <returns>returns true if license is available; otherwise false</returns>
        public static bool IsLicensed(LicenseManager licenseManager)
        {
            return licenseManager.IsEnabled(Functions.QueryCriteriaToolLicense);
        }

        /// <summary>
        /// Nested class to define the license function
        /// </summary>
        public new class Functions : CustomLicense.Functions
        {
            /// <summary>
            /// Datalytix license function
            /// </summary>
            public static readonly LicensedFunction QueryCriteriaToolLicense =
                CreateLicensedFunction("Datalytix", "Datalytix", FormWizard.Properties.Resources.FORM_TITLE);
        }

    }

    internal class CBVNIntegationHelper
    {
        internal static void EndIntegrationWithCBVN()
        {
            CBVNController.GetInstance().EndIntegrationWithSpotfire();
        }
    }
}
