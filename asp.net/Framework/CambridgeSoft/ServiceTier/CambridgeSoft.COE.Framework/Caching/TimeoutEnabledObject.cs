using System;
using System.Timers;
using CambridgeSoft.COE.Framework.COELoggingService;

namespace CambridgeSoft.COE.Framework.Caching
{
    /// <summary>
    /// Base class for wrapping time-sensitive objects, such as for caching purposes.
    /// The inheriting class should provide a constructor that applies values to the protected
    /// variables: _key, _lifespan, _enforceTimeout
    /// </summary>
    /// <remarks>
    /// Refer to http://msdn.microsoft.com/en-us/library/b1yfkh5e%28VS.71%29.aspx for
    /// a more complete explanation of implementing IDisposable in an abstract base class.
    /// </remarks>
    [Serializable]
    public abstract class TimeoutEnabledObject : IDisposable
    {
        protected string _key;
        protected Timer _timer;
        private int _minimumInactiveLifespan = 5000; // 5 seconds
        private int _maximumInactiveLifespan = 90000; // 1.5 minute
        private int _lifespan;
        protected bool _enforceTimeout;
        protected ElapsedEventHandler _elapsedEventHandler;

        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("TimeoutEnabledObject");

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
        public bool EnforceTimeOut
        {
            get { return _enforceTimeout; }
        }

        /// <summary>
        /// If enabled, begins a System.Timers.Timer which governs cached time-out.
        /// </summary>
        /// <param name="allowConnectionTimeOut">Boolean determining whether a timer should be enabled.</param>
        /// <param name="inactiveLifespan">System.TimeSpan used to set the timer</param>
        protected void StartTimer(bool allowConnectionTimeOut, TimeSpan inactiveLifespan)
        {
            _enforceTimeout = allowConnectionTimeOut;

            TimeSpan localSpan = inactiveLifespan;

            if (_enforceTimeout)
            {
                if (_timer == null)
                {
                    //if settings exist, override hard-coded defaults
                    try
                    {
                        string minLife = System.Configuration.ConfigurationManager.AppSettings["MinimumInactiveLifespan"];
                        if (minLife != null) { _maximumInactiveLifespan = Convert.ToInt32(minLife); }
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
        /// Called by the wrapped object's property accessor to re-initialize the timer, if enabled.
        /// </summary>
        protected void ResetTimer()
        {
            if (this._enforceTimeout && _timer != null)
            {
                _timer.Interval = _lifespan;
            }
        }

        /// <summary>
        /// handles the timer's Elapsed event.
        /// </summary>
        /// <param name="sender">The System.Timers.Timer which has elapsed.</param>
        /// <param name="e">A System.Timers.ElapsedEventArgs object</param>
        protected void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this._enforceTimeout)
            {
                _coeLog.Log(string.Format(
                    "The cachable object has elapsed after {0} seconds."
                    , new TimeSpan(0, 0, 0, 0, _lifespan).TotalSeconds)
                );
                this.Dispose();
            }
        }

        #region IDisposable Members

           //Implement IDisposable.
           public void Dispose() 
           {
             Dispose(true);
              GC.SuppressFinalize(this); 
           }

           protected virtual void Dispose(bool disposing) 
           {
              if (disposing) 
              {
                 // Free other state (managed objects).
              }
              // Free your own state (unmanaged objects).
              // Set large fields to null.
           }

            // Use C# destructor syntax for finalization code.
           ~TimeoutEnabledObject()
           {
              // Simply call Dispose(false).
              Dispose (false);
           }

        #endregion
    }
}
