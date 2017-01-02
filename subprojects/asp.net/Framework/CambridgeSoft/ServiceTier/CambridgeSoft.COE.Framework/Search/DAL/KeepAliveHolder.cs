using System;
using System.Collections.Generic;
using System.Text;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Timers;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COESearchService{
    /// <summary>
    /// This class is a holder for a ref cursor and connection to an oracle database.
    /// </summary>
    public class KeepAliveHolder : IDisposable {
        #region Variables
        private OracleConnection connection;
        private OracleRefCursor refCursor;
        private string key;
        private Timer timer;
        private bool dispose;
        private long lastRecord;
        private bool timerPolicy;

        [NonSerialized]
         static COELog _coeLog = COELog.GetSingleton("COESearch");

        #endregion

        #region Properties
        /// <summary>
        /// Its oracle database connection.
        /// </summary>
        public OracleConnection Connection {
            get { this.dispose = false;  return connection; }
            set { connection = value; }
        }

        /// <summary>
        /// Its ref cursor.
        /// </summary>
        public OracleRefCursor RefCursor {
            get { this.dispose = false; return refCursor; }
            set { refCursor = value; }
        }

        /// <summary>
        /// Its unique identifier.
        /// </summary>
        public string Key {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// The last accessed record.
        /// </summary>
        public long LastRecord {
            get { return lastRecord; }
            set { lastRecord = value; }
        }

        /// <summary>
        /// Determines whether a timer policy will be applied to auto release its resources or not.
        /// </summary>
        public bool TimerPolicy {
            get { return timer != null; }
            set {
                if(value) {
                    if(timer == null) {
                        this.timer = new Timer(30000);
                        this.timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                        this.timer.Start();
                    }
                } else {
                    if(timer != null) {
                        timer.Stop();
                        timer = null;
                    }
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of the holder with the desired parameters.
        /// </summary>
        /// <param name="connection">The oracle connection.</param>
        /// <param name="refCursor">Its Ref Cursor.</param>
        /// <param name="key">Its unique identifier</param>
        /// <param name="timerPolicy">Determines whether to use a timer to release its resources or not.</param>
        public KeepAliveHolder(OracleConnection connection, OracleRefCursor refCursor, string key, bool timerPolicy) {
            this.connection = connection;
            this.refCursor = refCursor;
            this.key = key;
            this.dispose = true;
            if(timerPolicy) {
                this.TimerPolicy = true;
            } else { this.TimerPolicy = false; }
            this.lastRecord = -1;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e) {
            if(dispose) {
                timer.Stop();
                this.Dispose();
            }
            dispose = true;
        }
        #endregion

        #region Business Methods
        public void Close() {
            if(refCursor != null)
                this.refCursor.Dispose();
            
            //if(connection != null) {
            //    if(this.connection.State == System.Data.ConnectionState.Open)
            //        this.connection.Close();
            //}
            
            if(timer != null)
                timer.Dispose();

            AppDomain.CurrentDomain.SetData(key, null);
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Releases its resources.
        /// </summary>
        public void Dispose() {
            Close();
        }

        #endregion
    }
}
