using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework;
using Spotfire.Dxp.Application;
using Spotfire.Dxp.Data;
using Spotfire.Dxp.Framework.ApplicationModel;
using Spotfire.Dxp.Framework.Library;
using SpotfireIntegration.Common;
using SpotfireIntegration.SpotfireAddin.Properties;
using CambridgeSoft.COE.Framework.COEDataViewService;
using COEServiceLib;

namespace SpotfireIntegration.SpotfireAddin
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public class SpotfireCOETableLoader : ISpotfireCOETableLoader
    {
        #region Constructors and Destructors

        public SpotfireCOETableLoader()
        {
        }

        #endregion

        #region Public Methods (WCF entry points)

        public void Subscribe()
        {
            ISpotfireCallbacks callbacks = OperationContext.Current.GetCallbackChannel<ISpotfireCallbacks>();
            CBVNController.GetInstance().Subscribe(callbacks);
        }

        public void LoadTablesFromCOE(COEHitList hitList, string baseTableName, string formName,
string authenticationTicket, string cslaDataPortalUrl, string cslaDataPortalProxy, string dataSource,
bool bForceReload, int maxRows, bool filterChildHits, string serverName, string userName)
        {
            try
            {
                CBVNController.GetInstance().InteractOnApplicationThread(
                delegate(AnalysisApplication application)
                {
                    ProgressService progressService = application.GetService<ProgressService>();
                    progressService.ExecuteWithProgress(FormWizard.Properties.Resources.FORM_TITLE, "The ChemBioViz form is being loaded.", delegate
                    {
                        ProgressService.CurrentProgress.ExecuteSubtask("Authenticating...");
                        COEService service = application.GetService<COEService>();
                        service.SetServer(cslaDataPortalUrl, cslaDataPortalProxy, dataSource);
                        service.ServerName = serverName;
                        service.UserName = userName;
                        application.SetUserCustomPreferences(service);
                        service.Login(authenticationTicket);

                        if (application.Document == null || application.Document.Data.Tables.GetCOEBaseTable() == null)
                        {
                            bForceReload = true;
                        }
                        else if (!bForceReload)
                        {
                            int baseTableID = (int)application.Document.Data.Tables.GetCOEBaseTable().Properties.GetProperty(Properties.Resources.COETableID_PropertyName);
                            bForceReload = (baseTableID != service.GetDataViewBO(hitList, filterChildHits).COEDataView.Basetable);
                        }


                        ProgressService.CurrentProgress.CheckCancel();
                        if (bForceReload)
                        {
                            ProgressService.CurrentProgress.ExecuteSubtask("The data tables in Spotfire are being loaded.", delegate
                            {
                                //CSBR:151920 formName parameter is added to LoadCOETables to set Document Title.
                                application.LoadCOETables(hitList, baseTableName, formName, filterChildHits);
                            });
                        }
                        else
                        {
                            ProgressService.CurrentProgress.ExecuteSubtask("The data tables in Spotfire are being updated.", delegate
                            {
                                application.UpdateSpotfireDataTable(hitList, filterChildHits);
                            });
                        }
                    });
                }
            );

            }
            catch (TargetInvocationException e)
            {
                while (e.InnerException is TargetInvocationException)
                {
                    e = e.InnerException as TargetInvocationException;
                }
                throw new FaultException<TableLoadFault>(new TableLoadFault(e.InnerException), e.InnerException.Message);
            }
        }

        public string LoadTablesFromFile(COEHitList hitList, string baseTableName, string formName,
string filename, string authenticationTicket, string cslaDataPortalUrl, string cslaDataPortalProxy,
string dataSource, int maxRows, string serverName, string userName)
        {
            string resultMessage = null;
            try
            {
                CBVNController.GetInstance().InteractOnApplicationThread(
                delegate(AnalysisApplication application)
                {
                    COEService service = application.GetService<COEService>();
                    service.SetServer(cslaDataPortalUrl, cslaDataPortalProxy, dataSource);
                    service.ServerName = serverName;
                    service.UserName = userName;
                    application.SetUserCustomPreferences(service);
                    service.Login(authenticationTicket);

                    // input string might be disk pathname or pathname from SF Library; latter starts with /
                    // THIS DOESN'T YET WORK
                    if (filename.StartsWith("/"))
                    {
                        LibraryManager manager = application.GetService<LibraryManager>();
                        LibraryItem item = null;
                        if (manager.TryGetItem(filename, LibraryItemType.Analysis, out item))
                        {
                            DocumentOpenSettings dos = new DocumentOpenSettings();
                            application.Open(item, dos);    // FAILS with null ref exception
                        }
                    }
                    else if (!filename.Equals("NOLOAD"))    // keyword means connect without rebuild
                    {
                        application.Open(filename);
                    }
                    if (hitList != null)
                    {
                        // Verify that the hitlist makes sense to load into this document.
                        COEHitList docHitList = CBVNController.GetInstance().GetCOEHitList();
                        COEDataViewBO docDataViewBO = service.GetDataViewBO(docHitList, false);
                        COEDataViewBO cbvnDataViewBO = service.GetDataViewBO(hitList, false);
                        if (docDataViewBO.ID != cbvnDataViewBO.ID)
                        {
                            if (docDataViewBO.COEDataView.Basetable != cbvnDataViewBO.COEDataView.Basetable)
                            {
                                // Fail
                                throw new FaultException<TableLoadFault>(new TableLoadFault(),
                                "The form data cannot be loaded into this document because the base table does not match.");
                            }
                            else
                            {
                                // Continue loading the data, but warn the user that tables may be empty.
                                resultMessage = "The dataview of the form does not match the dataview associated with this document.\n" +
                                                "Some Spotfire tables may not be populated as a result.";
                            }
                        }
                        application.UpdateSpotfireDataTable(hitList.HitListID, hitList.HitListType, hitList.NumHits);
                    }
                    CBVNMarkingTool.SynchronizeMarking(application.Document);
                }
            );
            }
            catch (TargetInvocationException e)
            {
                while (e.InnerException is TargetInvocationException)
                {
                    e = e.InnerException as TargetInvocationException;
                }
                throw new FaultException<TableLoadFault>(new TableLoadFault(e.InnerException), e.InnerException.Message);
            }

            return resultMessage;
        }

        public void CBVNRecordsetChanged(int hitListID, HitListType hitListType, int numHits)
        {
            try
            {
                CBVNController.GetInstance().InteractOnApplicationThread(
                delegate(AnalysisApplication application)
                {
                    application.GetService<ProgressService>().ExecuteWithProgress(
                        "Updating data", "The data tables are being updated with a new hitlist.",
                        delegate
                        {
                            ProgressService.CurrentProgress.ExecuteSubtask("Updating data");
                            application.UpdateSpotfireDataTable(hitListID, hitListType, numHits);
                        });
                });
            }
            catch (TargetInvocationException e)
            {
                while (e.InnerException is TargetInvocationException)
                {
                    e = e.InnerException as TargetInvocationException;
                }
                throw new FaultException<TableLoadFault>(new TableLoadFault(e.InnerException), e.InnerException.Message);
            }
        }

        public void CBVNResultsCriteriaChanged(string rcNew)   // added by JD, for sorting
        {
            try
            {
                CBVNController.GetInstance().InteractOnApplicationThread(
                    delegate(AnalysisApplication application)
                    {
                        application.GetService<ProgressService>().ExecuteWithProgress(
                            "Updating data tables", "The data tables are being updated with new result criteria.",
                            delegate
                            {
                                ProgressService.CurrentProgress.ExecuteSubtask("Updating data tables");
                                application.UpdateSpotfireDataTable(rcNew);
                            });
                    });
            }
            catch (TargetInvocationException e)
            {
                while (e.InnerException is TargetInvocationException)
                {
                    e = e.InnerException as TargetInvocationException;
                }
                throw new FaultException<TableLoadFault>(new TableLoadFault(e.InnerException), e.InnerException.Message);
            }
        }

        public void CloseCOEDocument(int dataViewID, int hitListID)
        {
            CBVNController.GetInstance().InteractOnApplicationThread(
                delegate(AnalysisApplication application)
                {
                    Document document = application.Document;

                    // Get the base data table.
                    DataTable dataTable = document.Data.Tables.GetCOEBaseTable();
                    if (dataTable == null)
                    {
                        return;
                    }

                    // Make sure that CBVN and Spotfire are looking at the same data set.
                    object currentDataViewID = dataTable.Properties.GetProperty(Resources.COEDataViewID_PropertyName);
                    if (currentDataViewID == null || !(currentDataViewID is int) || (int)currentDataViewID != dataViewID)
                    {
                        return;
                    }

                    object currentHitListID = dataTable.Properties.GetProperty(Resources.COEHitListID_PropertyName);
                    if (currentHitListID == null || !(currentHitListID is int) || (int)currentHitListID != hitListID)
                    {
                        return;
                    }

                    // Remove the current WCF callback subscription and close the document.
                    CBVNController.GetInstance().Subscribe(null);
                    application.Close();
                });
        }

        public void CBVNRecordChanged(int dataViewID, int hitListID, List<int> rowIndexes)
        {
            UpdateSpotfireMarking(dataViewID, hitListID, rowIndexes);
        }

        public COEHitList GetCOEHitList()
        {
            return CBVNController.GetInstance().GetCOEHitList();
        }

        public string GetAnalysisOrigin()
        {
            string origin = null;

            CBVNController.GetInstance().InteractOnApplicationThread(
                delegate(AnalysisApplication application)
                {
                    if (application.Document != null)
                    {
                        origin = application.DocumentMetadata.LoadedFromFileName;
                        if (origin == null)
                        {
                            origin = application.DocumentMetadata.LoadedFromLibraryPath;
                        }
                        if (origin == null)
                        {
                            COEHitList hitList = GetCOEHitList();
                            if (hitList != null)
                            {
                                origin = string.Format("Hitlist {0}", hitList.HitListID);
                            }
                        }
                        if (origin == null)
                        {
                            origin = "Unknown analysis";
                        }
                    }
                }
            );

            return origin;
        }

        public void FindForm()
        {
            CBVNController.GetInstance().InteractOnApplicationThread(
                delegate(AnalysisApplication application)
                {
                    foreach (Form form in Application.OpenForms)
                    {
                        if ("Spotfire.Dxp.Forms.Application.MainForm".Equals(form.GetType().FullName))
                        {
                            if (form.WindowState == FormWindowState.Minimized)
                            {
                                form.WindowState = FormWindowState.Normal;
                            }
                            form.Activate();
                            break;
                        }
                    }
                });
        }

        public void Ping()
        {
        }

        #endregion

        #region Non-public Methods

        internal void UpdateSpotfireMarking(int dataViewID, int hitListID, List<int> rowIndexes)
        {
            CBVNController.GetInstance().InteractOnApplicationThread(
                delegate(AnalysisApplication application)
                {
                    Document document = application.Document;

                    // Get the marking property.
                    DataMarkingSelection marking = document.ActiveMarkingSelectionReference;
                    if (marking == null)
                    {
                        marking = document.Data.Markings.DefaultMarkingReference;
                    }

                    // Get the base data table.
                    DataTable dataTable = document.Data.Tables.GetCOEBaseTable();

                    if (marking == null || dataTable == null)
                    {
                        return;
                    }

                    // Make sure that CBVN and Spotfire are looking at the same data set.
                    object currentDataViewID = dataTable.Properties.GetProperty(Resources.COEDataViewID_PropertyName);
                    if (currentDataViewID == null || !(currentDataViewID is int) || (int)currentDataViewID != dataViewID)
                    {
                        return;
                    }

                    object currentHitListID = dataTable.Properties.GetProperty(Resources.COEHitListID_PropertyName);
                    if (currentHitListID == null || !(currentHitListID is int) || (int)currentHitListID != hitListID)
                    {
                        return;
                    }

                    RowSelection rowSelection = new RowSelection(dataTable.RowCount, rowIndexes);

                    if (rowSelection.Equals(marking.GetSelection(dataTable)))
                    {
                        return;
                    }

                    document.Transactions.ExecuteInvisibleTransaction(delegate
                    {
                        marking.SetSelection(rowSelection, dataTable);
                    });
                });
        }

        #endregion
    }
}
