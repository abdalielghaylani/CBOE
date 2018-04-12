using System;
using System.Collections.Generic;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.COE.DataLoader.Core
{
    public class RecordParsingEventArgs : EventArgs
    {
        public RecordParsingEventArgs(int sourceRecordIndex)
        {
            _sourceRecordIndex = sourceRecordIndex;
        }

        private int _sourceRecordIndex = -1;
        public int SourceRecordIndex
        {
            get { return _sourceRecordIndex; }
        }
    }

    public class RecordParsedEventArgs : EventArgs
    {
        public RecordParsedEventArgs(Contracts.ISourceRecord record)
        {
            _sourceRecord = record;
        }

        private Contracts.ISourceRecord _sourceRecord = null;
        public Contracts.ISourceRecord SourceRecord
        {
            get { return _sourceRecord; }
        }
    }
    /*
    /// <summary>
    /// Used when a list of records has been processed
    /// </summary>
    public class RecordsProcessedEventArgs : EventArgs
    {
        private int _count;

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        public RecordsProcessedEventArgs(int count)
        {
            _count = count;
        }
    }
    */

    public class RecordsProcessedEventArgs : EventArgs
    {
        private Dictionary<ISourceRecord,string> _sourceRecords;
        private int _count;

        public Dictionary<ISourceRecord,string> SourceRecords
        {
            get { return _sourceRecords; }
            set { _sourceRecords = value; }
        }

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        public RecordsProcessedEventArgs(Dictionary<ISourceRecord, string> sourceRecords,int count)
        {
            _sourceRecords = sourceRecords;
            _count = count;
        }
    }

    /// <summary>
    /// Used before a record is to be processed
    /// </summary>
    public class RecordProcessingEventArgs : EventArgs
    {
        private Contracts.ISourceRecord _sourceRecord;

        public Contracts.ISourceRecord SourceRecord
        {
            get { return _sourceRecord; }
            set { _sourceRecord = value; }
        }

        public RecordProcessingEventArgs(Contracts.ISourceRecord sourceRecord)
        {
            _sourceRecord = sourceRecord;
        }
    }

    /// <summary>
    /// Information about the result of trying to import records to database
    /// </summary>
    public class RecordsImportedEventArgs : EventArgs
    {
        Dictionary<ISourceRecord,RegRecordSummaryInfo> _recordsSummaryInfo;

        /// <summary>
        /// Result of trying to import records to database
        /// </summary>
        public Dictionary<ISourceRecord, RegRecordSummaryInfo> RecordsSummaryInfo
        {
            get { return _recordsSummaryInfo; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="recordsSummaryInfo"></param>
        public RecordsImportedEventArgs(Dictionary<ISourceRecord, RegRecordSummaryInfo> recordsSummaryInfo)
        {
            _recordsSummaryInfo = recordsSummaryInfo;
        }
    }
}
