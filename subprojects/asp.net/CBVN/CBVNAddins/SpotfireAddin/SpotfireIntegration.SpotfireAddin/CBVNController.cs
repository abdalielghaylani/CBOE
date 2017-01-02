using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using CambridgeSoft.COE.Framework;
using Spotfire.Dxp.Application;
using Spotfire.Dxp.Data;
using Spotfire.Dxp.Framework.ApplicationModel;
using SpotfireIntegration.Common;

namespace SpotfireIntegration.SpotfireAddin
{
    public sealed class CBVNController : IDisposable
    {
        private static CBVNController instance;
        private static object instanceLock = new object();
        public static CBVNController GetInstance()
        {
            lock (instanceLock)
            {
                if (instance == null)
                {
                    instance = new CBVNController();
                }
                return instance;
            }

        }

        private ISpotfireCallbacks spotfireCallbacks;

        private readonly Thread workerThread;
        private delegate void Work();

        private readonly object pendingWorkItemLock = new object();
        private Work pendingWorkItem = null;

        private readonly object workCompleteLock = new object();
        private bool workComplete = false;

        private bool updatingMarking = false;

        private ServiceHost serviceHost;
        private AnalysisApplication analysisApplication;
        internal delegate void ApplicationInteractor(AnalysisApplication application);

        private CBVNController()
        {
            // Create and start the worker thread.
            this.workerThread = new Thread(DoWork);
            this.workerThread.IsBackground = true;
            this.workerThread.Name = "ChemBioViz.NET controller thread";
            this.workerThread.Start();
        }

        private void DoWork()
        {
            try
            {
                while (true)
                {
                    lock (this.pendingWorkItemLock)
                    {
                        while (this.pendingWorkItem == null)
                        {
                            Monitor.Wait(this.pendingWorkItemLock);
                        }

                        Work todo = this.pendingWorkItem;
                        this.pendingWorkItem = null;

                        todo();
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
        }

        private void Invoke(Work work)
        {
            lock (this)
            {
                lock (this.workCompleteLock)
                {
                    lock (this.pendingWorkItemLock)
                    {
                        this.pendingWorkItem =
                            delegate()
                            {
                                try
                                {
                                    work();
                                }
                                catch (Exception)
                                {
                                    // TODO: Log and handle exception.
                                }
                                finally
                                {
                                    // Signal that the work has been completed.
                                    lock (this.workCompleteLock)
                                    {
                                        this.workComplete = true;
                                        Monitor.PulseAll(this.workCompleteLock);
                                    }
                                }
                            };
                        Monitor.PulseAll(this.pendingWorkItemLock);
                    }

                    while (!this.workComplete)
                    {
                        Monitor.Wait(this.workCompleteLock);
                    }

                    this.workComplete = false;
                }
            }
        }

        private void InvokeLater(Work work)
        {
            lock (this)
            {
                lock (this.pendingWorkItemLock)
                {
                    this.pendingWorkItem =
                        delegate()
                        {
                            try
                            {
                                work();
                            }
                            catch (Exception)
                            {
                                // TODO: Log and handle exception.
                            }
                        };
                    Monitor.PulseAll(this.pendingWorkItemLock);
                }
            }
        }

        internal void InteractOnApplicationThread(ApplicationInteractor interactor)
        {
            this.analysisApplication.GetService<ApplicationThread>().Invoke(delegate
            {
                interactor(this.analysisApplication);
            });
        }

        #region Methods called from application thread

        internal void StartService(AnalysisApplication analysisApplication)
        {
            // Don't start the WCF service on the WinForms application thread.
            this.InvokeLater(delegate
            {
                this.analysisApplication = analysisApplication;

                this.serviceHost = new ServiceHost(
                    typeof(SpotfireCOETableLoader),
                    new Uri[] {
                    Uris.SpotfireServiceBaseAddress,
                });

                NetNamedPipeBinding binding = new NetNamedPipeBinding();
                binding.ReaderQuotas.MaxStringContentLength = 65536;

                this.serviceHost.AddServiceEndpoint(
                    typeof(ISpotfireCOETableLoader),
                    binding,
                    SpotfireIntegration.Common.Uris.SpotfireServiceName);

                try
                {
                    this.serviceHost.Open();
                    analysisApplication.DocumentChanged += application_DocumentChanged;
                    analysisApplication.Exiting += application_Exiting;
                }
                catch (Exception)
                {
                }
            });
        }

        internal void SelectRows(int dataViewID, int hitListID, List<int> rowIndexes)
        {
            if (this.updatingMarking)
            {
                return;
            }

            this.InvokeLater(
                delegate
                {
                    if (this.spotfireCallbacks == null)
                    {
                        return;
                    }

                    try
                    {
                        this.spotfireCallbacks.SelectRows(dataViewID, hitListID, rowIndexes);
                    }
                    catch (CommunicationException)
                    {
                        this.spotfireCallbacks = null;
                    }
                });
        }

        internal void ResultsCriteriaChanged()
        {
            COEHitList hitList = GetCOEHitList();
            this.InvokeLater(
                delegate
                {
                    if (this.spotfireCallbacks != null)
                    {
                        try
                        {
                            this.spotfireCallbacks.ResultsCriteriaChanged(hitList.ResultsCriteriaXML);
                        }
                        catch (CommunicationException)
                        {
                            this.spotfireCallbacks = null;
                        }
                    }
                });
        }

        internal void EndIntegrationWithSpotfire()
        {
            COEHitList hitList = null;
            EndIntegrationWithSpotfire(hitList);
        }

        private void EndIntegrationWithSpotfire(COEHitList hitList)
        {
            this.InvokeLater(
                delegate
                {
                    if (this.spotfireCallbacks != null)
                    {
                        try
                        {
                            this.spotfireCallbacks.SpotfireDocumentChanged(hitList);
                        }
                        catch (CommunicationException)
                        {
                            this.spotfireCallbacks = null;
                        }
                    }
                });
        }

        private void application_DocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            //clear the notifications from helper class when document is changed
            InteractOnApplicationThread(
                delegate(AnalysisApplication application)
                {
                    NotificationServiceHelper.ClearNotifications();
                });
            COEHitList hitList = GetCOEHitList();
            EndIntegrationWithSpotfire(hitList);
        }

        private void application_Exiting(object sender, AnalysisApplicationExitingEventArgs e)
        {
            this.Invoke(
                delegate
                {
                    if (this.spotfireCallbacks != null)
                    {
                        try
                        {
                            this.spotfireCallbacks.SpotfireExiting();
                        }
                        catch (CommunicationException)
                        {
                            this.spotfireCallbacks = null;
                        }
                    }

                    this.serviceHost.Close();
                });
        }

        #endregion

        #region Methods called from WCF worker thread

        internal void Subscribe(ISpotfireCallbacks callbacks)
        {
            this.Invoke(
                delegate
                {
                    this.spotfireCallbacks = callbacks;
                });
        }

        internal COEHitList GetCOEHitList()
        {
            COEHitList hitList = null;

            InteractOnApplicationThread(
                delegate(AnalysisApplication application)
                {
                    if (application.Document != null)
                    {
                        DataTable baseDataTable = application.Document.Data.Tables.GetCOEBaseTable();
                        if (baseDataTable != null)
                        {
                            int hitListID = (int)baseDataTable.Properties.GetProperty(Properties.Resources.COEHitListID_PropertyName);
                            HitListType hitListType = (HitListType)baseDataTable.Properties.GetProperty(Properties.Resources.COEHitListType_PropertyName);
                            int numHits = baseDataTable.RowCount;
                            int dataViewID = (int)baseDataTable.Properties.GetProperty(Properties.Resources.COEDataViewID_PropertyName);
                            string resultsCriteriaXml = (string)baseDataTable.Properties.GetProperty(Properties.Resources.COEResultsCriteria_PropertyName);
                            string searchCriteriaXml = (string)baseDataTable.Properties.GetProperty(Properties.Resources.COESearchCriteria_PropertyName);
                            hitList = new COEHitList(hitListID, hitListType, numHits, dataViewID, resultsCriteriaXml, searchCriteriaXml);
                        }
                    }
                });

            return hitList;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
