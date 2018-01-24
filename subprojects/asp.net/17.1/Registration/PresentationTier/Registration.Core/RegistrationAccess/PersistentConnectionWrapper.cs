using System;
using System.Data.Common;
using System.Timers;
using CambridgeSoft.COE.Framework.COELoggingService;

/* The caller uses this class by creating an instance of it, then setting it as an AppDomain variable
 * which is keyed to the user.
 * ex.
 *      string pcKey = Csla.ApplicationContext.User.Identity.Name + rand.Next();
 *      TimeSpan lifetime = new TimeSpan(0, 0, 90);
 *      PersistentConnectionWrapper persistentConn = new PersistentConnectionWrapper(myConnection, pcKey, true, lifetime);
 *      AppDomain.CurrentDomain.SetData(pcKey, persistentConn);
 * 
 * */

namespace CambridgeSoft.COE.Registration.Access
{
    /// <summary>
    /// Provides Oracle session persistence between calls by means of an expirable, cacheable connection object.
    /// </summary>
    /// <remarks>
    /// The expiration time(s) should ideally be derived from a configuration setting, but the default range provided
    /// is (minimum) 5 seconds to (maximum) 90 seconds.
    /// </remarks>
    /// <example>
    /// </example>
    public class PersistentConnectionWrapper : IDisposable
    {
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEPersistentConnectionWrapper");

        private DbConnection _connection;
        private string _key;
        private Timer _timer;
        private int _minimumInactiveLifespan = 5000; // 5 seconds
        private int _maximumInactiveLifespan = 90000; // 1.5 minute
        private int _lifespan;
        private bool _enforceConnectionTimeout;
        ElapsedEventHandler _elapsedEventHandler;

        /// <summary>
        /// Initializes a cachable Oracle.DataAccess.Client.OracleConnection instance which can be used between
        /// transactions to ensure a persistent connection will be used. Promotes Oracle session re-use. Lower
        /// and upper limits can be set on the cache timer to control expiration.
        /// </summary>
        /// <remarks>
        /// Does not enable transactional support between calls. If 'enforceConnectionTimeout' is set to false,
        /// the connection can be closed explicitly by the caller, or will go out of scope when the application closes.
        /// </remarks>
        /// <param name="connection">The oracle connection used to initialize the Connection property.</param>
        /// <param name="appDomainKey">The key to assign the instance, if cached.</param>
        /// <param name="enforceConnectionTimeout">If true, enables a timer which controls the lifetime of the connection object.</param>
        /// <param name="inactiveLifespan">The length of time the connection may remain inactive before elimination from the cache.</param>
        public PersistentConnectionWrapper(
            DbConnection connection
            , string appDomainKey
            , bool enforceConnectionTimeout
            , TimeSpan inactiveLifespan
        )
        {
            _connection = connection;
            _key = appDomainKey;
            _enforceConnectionTimeout = enforceConnectionTimeout;
            _elapsedEventHandler = new ElapsedEventHandler(Timer_Elapsed);
            StartTimer(_enforceConnectionTimeout, inactiveLifespan);
        }

        /// <summary>
        /// Derived via the class's constructor.
        /// </summary>
        public DbConnection Connection
        {
            get 
            {
                ResetTimer();
                return _connection; 
            }
        }

        /// <summary>
        /// Derived via the class's constructor.
        /// </summary>
        public string Key
        {
            get { return _key; }
        }

        /// <summary>
        /// Derived via the class's constructor.
        /// </summary>
        public bool EnforceConnectionTimeOut
        {
            get { return _enforceConnectionTimeout; }
        }

        /// <summary>
        /// If enabled, begins a System.Timers.Timer which governs cached time-out.
        /// </summary>
        /// <param name="allowConnectionTimeOut">Boolean determining whether a timer should be enabled.</param>
        /// <param name="inactiveLifespan">System.TimeSpan used to set the timer</param>
        private void StartTimer(bool allowConnectionTimeOut, TimeSpan inactiveLifespan)
        {
            _enforceConnectionTimeout = allowConnectionTimeOut;

            TimeSpan localSpan = inactiveLifespan;

            if (_enforceConnectionTimeout)
            {
                if (_timer == null)
                {
                    //if settings exist, override hard-coded defaults
                    try
                    {
                        string minLife = System.Configuration.ConfigurationManager.AppSettings["MinimumInactiveLifespan"];
                        if (minLife != null) { _minimumInactiveLifespan = Convert.ToInt32(minLife); }
                    }
                    finally { }

                    try
                    {
                        string maxLife = System.Configuration.ConfigurationManager.AppSettings["MaximumInactiveLifespan"];
                        if (maxLife != null) { _maximumInactiveLifespan = Convert.ToInt32(maxLife); }
                    }
                    finally { }

                    //account for null TimeSpan
                    if (localSpan == null)
                    {
                        localSpan = new TimeSpan(0, 0, _maximumInactiveLifespan);
                    }

                    _lifespan = Convert.ToInt32(localSpan.TotalMilliseconds);

                    //account for or out-of-range cases
                    if (_lifespan > _maximumInactiveLifespan || _lifespan < _minimumInactiveLifespan)
                    {
                        //JED: Could throw an exception, but probably more usable if silently overridden
                        //string notification = "The timer lifespan provided was outside the bounds ({0} to {1} milliseconds) allowed.";
                        //throw new ArgumentOutOfRangeException(
                        //    "allowConnectionTimeOut"
                        //    , allowConnectionTimeOut
                        //    , String.Format(notification, _minimumInactiveLifespan.ToString(), _maximumInactiveLifespan.ToString())
                        //);
                        _coeLog.Log(String.Format("The timer lifespan was reset to {0} milliseconds", _maximumInactiveLifespan.ToString()));
                        _lifespan = _maximumInactiveLifespan;
                    }

                    _timer = new Timer(_lifespan);
                    _timer.Enabled = true;
                    //subscribe to event
                    _timer.Elapsed += _elapsedEventHandler;
                    _timer.Start();
                }
            }
            else
            {
                if (_timer != null)
                {
                    //unsubscribe from event
                    _timer.Elapsed -= _elapsedEventHandler;
                    _timer.Stop();
                    _timer = null;
                }
            }

        }

        /// <summary>
        /// Called by the Connection property accessor to re-initialize the timer, if enabled.
        /// </summary>
        private void ResetTimer()
        {
            if (this._enforceConnectionTimeout && _timer != null)
            {
                _timer.Interval = _lifespan;
            }
        }

        /// <summary>
        /// handles the timer's Elapsed event.
        /// </summary>
        /// <param name="sender">The System.Timers.Timer which has elapsed.</param>
        /// <param name="e">A System.Timers.ElapsedEventArgs object</param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this._enforceConnectionTimeout)
            {
                _coeLog.Log(string.Format(
                    "The Oracle connection has elapsed after {0} seconds."
                    , new TimeSpan(0, 0, 0, 0, _lifespan).TotalSeconds)
                );
                this.Dispose();
            }
        }

        /// <summary>
        /// Determines whether the specified connection object is equal to the connection object in the class.
        /// </summary>
        /// <param name="obj">The connection object to compare for equality with the local connection object.</param>
        /// <returns>true if the passed connection object equals to local connection object; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is DbConnection)
            {
                return this._connection.Equals((DbConnection)obj);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Instance disposal removes the System.Timers.Timer's 'Elapsed' event-handler, as well
        /// as closing the disposing of the connection explicitly.
        /// </summary>
        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Elapsed -= _elapsedEventHandler;
                _timer.Stop();
                _timer.Dispose();
            }
            if (_connection != null && _connection.State != System.Data.ConnectionState.Closed)
            {
                _connection.Close();
                _connection.Dispose();
            }

            AppDomain.CurrentDomain.SetData(_key, null);
            _coeLog.Log("The Oracle connection was closed from PersistentConnectionWrapper.");

        }

        #endregion
    }
}
