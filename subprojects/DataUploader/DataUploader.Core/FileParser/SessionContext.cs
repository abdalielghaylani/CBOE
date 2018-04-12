using System.Threading;

namespace CambridgeSoft.COEDataLoader.FileParser
{
    /// <summary>
    /// Stores context information specific to one session. The session here is not equivalent to an application, because
    /// user can continue loading another file or another part of the same file at the last step of the application.
    /// </summary>
    public class SessionContext
    {
        private static SessionContext _current = null;
        private static object syncObject = new object();

        private int _numberOfRecordsToProcess = 0;
        private int _numberOfParsedRecords = 0;

        private SessionContext() { }

        public static SessionContext Current
        {
            get
            {
                lock (syncObject)
                {
                    if (_current == null)
                    {
                        lock (syncObject)
                        {
                            if (_current == null)
                            {
                                _current = new SessionContext();
                                _current.Reset();
                            }
                        }
                    }
                }

                return _current;
            }
        }

        /// <summary>
        /// The number of records user has selected at the beginning to process during this session.
        /// </summary>
        public int NumberOfRecordsToProcess
        {
            get { return _numberOfRecordsToProcess; }
            set { Interlocked.Exchange(ref _numberOfRecordsToProcess, value); }
        }

        public int NumberOfParsedRecords
        {
            get { return _numberOfParsedRecords; }
            set { Interlocked.Exchange(ref _numberOfParsedRecords, value); }
        }

        public void Reset()
        {
            Interlocked.Exchange(ref _numberOfRecordsToProcess, 0);
            Interlocked.Exchange(ref _numberOfParsedRecords, 0);
        }
    }
}
