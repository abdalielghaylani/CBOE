using System;
using System.Timers;
using CambridgeSoft.COE.Framework.Caching;
using CambridgeSoft.COE.Framework.COELoggingService;

namespace CambridgeSoft.COE.Registration.Services.AddIns
{
    /// <summary>
    /// Wrapper for IAddIn which enables the add-in to be expire if unused, especially for caching.
    /// </summary>
    public class PersistentAddInWrapper : TimeoutEnabledObject
    {
        /// <summary>
        /// Initializes a cachable IAddIn instance.
        /// Lower and upper limits can be set on the cache timer to control expiration.
        /// </summary>
        /// <param name="addIn">The IAddIn instance used to initialize the AddIn property.</param>
        /// <param name="appDomainKey">The key to assign the instance, if cached.</param>
        /// <param name="enforceConnectionTimeout">If true, enables a timer which controls the lifetime of the connection object.</param>
        /// <param name="inactiveLifespan">The length of time the connection may remain inactive before elimination from the cache.</param>
        public PersistentAddInWrapper(
            IAddIn addIn
            , string appDomainKey
            , bool enforceObjectTimeout
            , TimeSpan inactiveLifespan
        )
        {
            _addIn = addIn;
            base._key = appDomainKey;
            base._enforceTimeout = enforceObjectTimeout;
            base._elapsedEventHandler = new ElapsedEventHandler(Timer_Elapsed);
            StartTimer(base._enforceTimeout, inactiveLifespan);
        }

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("PersistentAddInWrapper");

        private IAddIn _addIn;
        /// <summary>
        /// Derived via the class's constructor.
        /// </summary>
        public IAddIn AddIn
        {
            get 
            {
                ResetTimer();
                return _addIn; 
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Instance disposal removes the System.Timers.Timer's 'Elapsed' event-handler, as well
        /// as closing the disposing of the add-in's RegistryRecord explicitly.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_timer != null)
                {
                    _timer.Elapsed -= base._elapsedEventHandler;
                    _timer.Stop();
                    _timer.Dispose();
                }
                if (_addIn != null)
                {
                    if (_addIn.RegistryRecord != null)
                    {
                        _addIn.RegistryRecord = null;
                    }
                }

                AppDomain.CurrentDomain.SetData(_key, null);
                _coeLog.Log("The IAddIn was dropped from PersistentAddInWrapper.");
            }
            // Release unmanaged resources.
            // Set large fields to null.
            // Call Dispose on your base class.
            base.Dispose(disposing);
        }

        #endregion

    }
}
