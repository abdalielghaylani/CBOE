using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.DataLoader.Core.PROPOSED
{
    public class RecordsParsedEventArgs : EventArgs
    {
        private List<Contracts.ISourceRecord> _sourceRecords = null;
        public List<Contracts.ISourceRecord> SourceRecords
        {
            get { return _sourceRecords; }
        }

        public RecordsParsedEventArgs(List<Contracts.ISourceRecord> records)
        {
            _sourceRecords = records;
        }
    }
}
