using System;
using System.Timers;
using ChemDrawControl19;
using CambridgeSoft.COE.Framework.COELoggingService;

/* JED: DO NOT USE THIS OBJECT - I will retire it soon due to inconsistent, occasional COM failures
 * during serialization and deserialization on some servers.*/

namespace CambridgeSoft.COE.Framework.Caching
{
    /// <summary>
    /// Provides a wrapper for caching a ChemDrawControl12.ChemDrawCtl instance using a singleton pattern.
    /// The caller provides the caching key to be used for a ChemDrawCtl instance via the static
    /// method "GetCachedChemdrawControl", which performs on-demand object caching, using the key provided.
    /// NOTE: Replace this using the Microsoft.Practices.EnterpriseLibrary.Caching namespace. Also, be aware
    /// that if the object you are trying to cache isn't Serializable, the object cached will be null.
    /// </summary>
    /// <example>
    /// void GetControl(string key)
    /// {
    ///     //if no ChemDrawControl12.ChemDrawCtl is presently cached against this key, one will be created,
    ///     //  added to the AppDomain (cache) and returned to the caller
    ///     //if a cached item with this key exists, that object will be fetched from the AppDomain,
    ///     //  cast (as ChemDrawControl12.ChemDrawCtl) and returned
    ///     CacheableChemdrawControl wrapper = CacheableChemdrawControl.GetCachedChemdrawControl(key);
    ///     ChemDrawControl12.ChemDrawCtl ctl = wrapper.Control;
    /// }
    /// </example>
    /// <remarks>
    /// The intention is to offset the cost of creating the ChemDrawCtl for highly-repetitive tasks such
    /// as bulk data-loading. The cache is first queried blindly, ensuring that when a null is fetched
    /// (no item found), a new control is created and cached (instead of failing a cast to a ChemDrawCtl).
    /// </remarks>
    public class CacheableChemdrawControl : TimeoutEnabledObject
    {
        private ChemDrawCtl _chemDrawControl = null;
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("CacheableChemdrawControl");

        /// <summary>
        /// Constructor (private).
        /// </summary>
        /// <param name="control">An instance of ChemDrawControl12.ChemDrawCtl</param>
        /// <param name="cacheKey">The session-specific caching key</param>
        /// <param name="lifespanInSeconds">How long the ChemDrawCtl instance will remain cached if unused</param>
        /// <param name="enforceTimeout">If False, keeps the ChemDrawCtl instance cached until the applciation expires.</param>
        private CacheableChemdrawControl(ChemDrawCtl control, string cacheKey, int lifespanInSeconds, bool enforceTimeout)
        {
            this._key = cacheKey;
            this._enforceTimeout = enforceTimeout;
            this._chemDrawControl = control;

            //Associates the timeout method with the Timer event
            this._elapsedEventHandler = new ElapsedEventHandler(Timer_Elapsed);

            //Note: this._lifespan is set by the following method
            this.StartTimer(_enforceTimeout, new TimeSpan(0, 0, lifespanInSeconds));
        }

        /// <summary>
        /// Gets a unique chemdraw control to be used by an addIn for calculation.
        /// </summary>
        /// <param name="key">The key to find the object in the appdomain</param>
        /// <returns></returns>
        public static CacheableChemdrawControl GetCachedChemdrawControl(string key)
        {
            key = "CDv12_" + key;
            CacheableChemdrawControl controlWrapper = null;
            object cachedObject = AppDomain.CurrentDomain.GetData(key);
            if (cachedObject == null) //No object found, so let's create it.
            {
                ChemDrawCtl ctrl = new ChemDrawCtl();

                if (ctrl.Version.ToUpper().Contains("PRO")) //Check the version, but just once.
                    AppDomain.CurrentDomain.SetData(key, controlWrapper); //Save the obj into the App Domain
                else
                    throw new Exception(
                        string.Format("ChemDraw control version not supported in server side. <br>Server version: {0} (required a registered Ultra version)", ctrl.Version)
                    );

                controlWrapper = new CacheableChemdrawControl(ctrl, key, 30, true);
                AppDomain.CurrentDomain.SetData(key, controlWrapper);
            }
            else
            {
                controlWrapper = (CacheableChemdrawControl)cachedObject;
            }
            //ctrl.Objects.Clear(); //Just to make sure we don't retrieve a dirty object.
            return controlWrapper;
        }

        /// <summary>
        /// Derived via the class's constructor.
        /// </summary>
        public ChemDrawCtl Control
        {
            get
            {
                ResetTimer();
                return _chemDrawControl;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Instance disposal removes the System.Timers.Timer's 'Elapsed' event-handler, as well
        /// as nullifying the ChemDrawCtl instance explicitly.
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

                AppDomain.CurrentDomain.SetData(_key, null);
                _coeLog.Log(string.Format("The ChemDrawControl (key={0}) object has been removed from the cache.", _key));
            }

            // Release unmanaged resources.
            if (this._chemDrawControl != null)
                this._chemDrawControl = null;

            // Set large fields to null.
            // Call Dispose on your base class.
            base.Dispose(disposing);
        }

        #endregion
    }
}
