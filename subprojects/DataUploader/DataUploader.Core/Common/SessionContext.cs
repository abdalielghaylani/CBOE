using System.Threading;

//TODO: Finalize this object!
/* What is this object's job?
 * (1) stores all the information required to instantiate a parser from the FileReaderFactory:
 *      the working file path/name (a string; can create a FileInfo object)
 *      the working file's type (an enum member; can be derived from its assigned number or name)
 *      the presence or absence of a header row (boolean; ignored if SDFile)
 *      the field delimiter (string; ignored if SDFile)
 * (2) stores the working data-range (from the user) as a List<DataRange> with a single member to start
 * (3) stores the destination object type (an enum member)
 * (4) stores the list of mappings that are applicable for the working file and destination object
 * 
 * */

namespace CambridgeSoft.COE.DataLoader.Core
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
