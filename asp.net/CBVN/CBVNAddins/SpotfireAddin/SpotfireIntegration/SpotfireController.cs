using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using ChemBioViz.NET;
using CBVUtilities;

namespace SpotfireIntegration
{
    // This is a singleton class that manages the Spotfire controller thread.
    // All calls into the Spotfire integration addin should be routed through
    // this object to prevent multiple threads from running inside the addin
    // simultaneously.

    internal class SpotfireController
    {
        private static SpotfireController instance = null;
        private static object instanceLock = new object();

        private Thread workerThread;
        private delegate void Work();

        private Work pendingWorkItem = null;
        private object pendingWorkItemLock = new object();

        private bool workComplete = false;
        private object workCompleteLock = new object();

        private ManualResetEvent waitingThreadEvent = new ManualResetEvent(true);
        private object waitingThreadEventLock = new object();

        private SpotfireIntegration spotfireIntegration;

        public static SpotfireController GetInstance()
        {
            lock (instanceLock)
            {
                if (instance == null)
                {
                    instance = new SpotfireController();
                }
            }
            return instance;
        }

        public SpotfireIntegration.SpotfireIntegrationState State
        {
            get { return (this.spotfireIntegration == null)?
                SpotfireIntegration.SpotfireIntegrationState.Disconnected : spotfireIntegration.State; }
        }

        private SpotfireController()
        {
            this.workerThread = new Thread(DoWork);
            this.workerThread.IsBackground = true;
            this.workerThread.Name = "Spotfire Integration Controller Thread";
            this.workerThread.Start();
        }

        // This is the main loop of the Spotfire controller thread.
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

        // Performs a synchronous call into the Spotfire controller thread to do some work.
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
                                catch (Exception e)
                                {
                                    CBVUtil.ReportError(e, "Spotfire addin: Error in worker thread");
                                    MessageBox.Show(e.ToString());
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

        // Performs an asynchronous call into the Spotfire controller thread to do some work.
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
                            catch (Exception e)
                            {
                                CBVUtil.ReportError(e, "Spotfire addin: Error in worker thread");
                                MessageBox.Show(e.ToString());
                            }
                        };
                    Monitor.PulseAll(this.pendingWorkItemLock);
                }
            }
        }

        internal void StartIntegration(ChemBioVizForm form)
        {
            bool success = false;

            if (this.spotfireIntegration != null &&
                this.spotfireIntegration.State == SpotfireIntegration.SpotfireIntegrationState.Starting)
            {
                //CSBR: 150132: Need to show the Glass hour instead of message.
                Cursor.Current = Cursors.WaitCursor;
                this.EndIntegration();
                Cursor.Current = Cursors.Default;
                this.StartIntegration(form);
            }
            
            // Synchronous call to prevent the CBVN application thread from
            // modifying the form instance while we pull data from it.
            this.Invoke(
                delegate
                {
                    if (this.spotfireIntegration == null)
                    {
                        this.spotfireIntegration = new SpotfireIntegration();
                    }
                    success = this.spotfireIntegration.StartIntegration(form);
                });

            if (success)
            {
                // Asynchronous call to load up Spotfire in the background,
                // since it may take a while or even fail silently.
                this.InvokeLater(
                    delegate
                    {
                        this.spotfireIntegration.LinkToSpotfire();
                    });
            }
        }

        internal void PingSpotfire()
        {
            this.Invoke(
                delegate
                {
                    if (this.spotfireIntegration != null)
                    {
                        if (!this.spotfireIntegration.PingSpotfire())
                        {
                            this.spotfireIntegration.Dispose();
                            this.spotfireIntegration = null;
                        }
                    }
                });
        }

        internal void EndIntegration()
        {
            this.InvokeLater(
               delegate
               {
                   if (this.spotfireIntegration != null)
                   {
                       //this.spotfireIntegration.EndIntegration(); .. no need .. called by Dispose
                       this.spotfireIntegration.Dispose();
                       this.spotfireIntegration = null;
                   }
               });
        }

        internal void CBVNRecordChanged(object sender, RecordChangedEventArgs e)
        {
            this.Invoke(
                delegate
                {
                    if (this.spotfireIntegration != null)
                    {
                        this.spotfireIntegration.CBVNRecordChanged(sender, e);
                    }
                });
        }

        internal void CBVNRecordsetChanged(object sender, RecordsetChangedEventArgs e)
        {
            lock (this.waitingThreadEventLock)
            {
                this.InvokeLater(
                    delegate
                    {
                        try
                        {
                            //Coverity Bug Fix CID 19033 
                            lock (this.waitingThreadEventLock)
                            {
                                this.waitingThreadEvent.Reset();
                                Monitor.PulseAll(this.waitingThreadEventLock);
                                if (this.spotfireIntegration != null)
                                {
                                    this.spotfireIntegration.CBVNRecordsetChanged(sender, e);
                                }
                            }
                        }
                        finally
                        {
                            this.waitingThreadEvent.Set();
                        }
                    });

                // Wait for the invoked code to start to ensure the waitingThreadEvent
                // is properly reset for the current invocation.
                Monitor.Wait(this.waitingThreadEventLock);
            }

            // Wait for the invoked code to release the calling thread.
            this.waitingThreadEvent.WaitOne();
        }

        internal void ReleaseWaitingThreads()
        {
            this.waitingThreadEvent.Set();
        }

        internal void CBVNGridSelectionChanged(object sender, Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs e)
        {
            this.Invoke(
                delegate
                {
                    if (this.spotfireIntegration != null)
                    {
                        this.spotfireIntegration.CBVNGridSelectionChanged(sender, e);
                    }
                });
        }
    }
}
