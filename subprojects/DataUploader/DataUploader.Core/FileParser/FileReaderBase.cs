using System;
using System.IO;
using System.Collections.Generic;

using CambridgeSoft.COE.DataLoader.Core.Contracts;

namespace CambridgeSoft.COE.DataLoader.Core
{
    public abstract class FileReaderBase : IFileReader
    {
        /// <summary>
        /// The underlying System.IO.Stream which encapsulates the data-source.
        /// </summary>
        protected Stream _stream;

        protected int _totalRecordCount = -1;
        protected int _currentRecordIndex = 0;
        protected int _parsedRecordCount = 0;
        protected ISourceRecord _current = null;

        public List<ISourceRecord> ExtractToRecords(IndexRange range)
        {
            List<ISourceRecord> extractedRecords = new List<ISourceRecord>();

            Rewind();
            Seek(range.RangeBegin);

            IndexList indices = range.ToIndexList();

            // Loop over the indexes...
            // ...they're contiguous by definition becuase they came from a single 'range'
            foreach (int i in indices)
            {
                ISourceRecord record = GetNext();
                if (record != null)
                {
                    extractedRecords.Add(record);
                }
                else
                {
                    // TODO: Add reactions to source file parsing failure.
                    break;
                }
            }

            return extractedRecords;
        }

        #region > Constructors <

        public FileReaderBase() { }

        public FileReaderBase(string filePath)
        {
            _stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }

        public FileReaderBase(Stream dataStream)
        {
            _stream = dataStream;
        }

        private void Initialize(Stream stream)
        {
            _stream = stream;
        }

        #endregion

        #region > IFileReader<SourceRecordBase> Members <

        public int RecordCount
        {
            get
            {
                if (_totalRecordCount == -1)
                    CountAll();

                return _totalRecordCount;
            }
        }

        public int ParsedRecordCount
        {
            get { return _parsedRecordCount; }
            set { _parsedRecordCount = value; }
        }

        public int CurrentRecordIndex
        {
            get { return _currentRecordIndex; }
            set { _currentRecordIndex = value; }
        }

        public ISourceRecord Current
        {
            get { return _current; }
        }

        public abstract int CountAll();

        public abstract void ReadNext();

        public abstract ISourceRecord GetNext();

        public abstract void Seek(int recordIndex);

        public abstract void Rewind();

        public abstract void Close();

        public event EventHandler<RecordParsingEventArgs> RecordParsing;

        public event EventHandler<RecordParsedEventArgs> RecordParsed;

        #endregion

        public virtual void OnRecordParsed(RecordParsedEventArgs e)
        {
            if (this.RecordParsed != null)
                RecordParsed(this, e);
        }

        public virtual void OnRecordParsing(RecordParsingEventArgs e)
        {
            if (this.RecordParsing != null)
                RecordParsing(this, e);
        }

    }
}
