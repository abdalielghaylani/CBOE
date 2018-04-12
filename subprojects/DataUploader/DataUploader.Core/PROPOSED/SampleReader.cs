using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser.CSV;

namespace CambridgeSoft.COE.DataLoader.Core.PROPOSED
{
    public class SampleReader : IDataFileReader
    {
        private FileStream _fileStream;
        private StreamReader _reader;

        public SampleReader(string filePath)
        {
            _filePath = filePath;
            _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            _reader = new StreamReader(_fileStream);
        }

        public SampleReader(FileStream fileStream)
        {
            _fileStream = fileStream;
            _reader = new StreamReader(_fileStream);
        }

        private void Rewind()
        {
            _reader.DiscardBufferedData();
            _fileStream.Position = 0;
            _index = 0;
            _lastParsedRecord = null;
        }

        #region > Candidates for a base class <

        private string _filePath;
        private FileInfo _dataFileInfo;
        /// <summary>
        /// If the data comes from a local file-system file, file metadata is available via this property.
        /// </summary>
        public FileInfo DataFileInfo
        {
            get
            {
                if (_dataFileInfo == null)
                    if (!string.IsNullOrEmpty(_filePath))
                        _dataFileInfo = new FileInfo(_filePath);
                return _dataFileInfo;
            }
        }

        public virtual void OnRecordParsed(RecordParsedEventArgs e)
        {
            if (this.RecordParsed != null)
                RecordParsed(this, e);
        }

        public virtual void OnRecordsParsed(RecordsParsedEventArgs e)
        {
            if (this.RecordsParsed != null)
                RecordsParsed(this, e);
        }

        #endregion

        #region > IDataFileReader Members <

        public ISourceRecord _lastParsedRecord = null;
        public ISourceRecord LastParsedRecord
        {
            get { return _lastParsedRecord; }
        }

        private int _index;
        public int Index
        {
            get { return _index; }
        }

        private int _fileRecordCount;
        public int FileRecordCount
        {
            get { return _fileRecordCount; }
        }

        public void MoveToIndex(int index)
        {
            if (index > _fileRecordCount)
                throw new Exception("Invalid index requested: beyond end of file");
            if (index < 0)
                throw new Exception("Invalid index requested: cannot be negative");
            if (index == _index)
                return;
            if (index < _index)
                Rewind();

            while (_index < index && !_reader.EndOfStream)
            {
                _reader.ReadLine();
                _index++;
            }

            if (_reader.EndOfStream)
                Rewind();
        }

        public ISourceRecord Read()
        {
            string lineOfData;
            if ((lineOfData = _reader.ReadLine()) != null)
            {
                List<string> values = new List<string>();
                values.Add(lineOfData);
                _lastParsedRecord = new CSVSourceRecord(_index, values);
                _lastParsedRecord.FieldSet.Add("line_of_data", lineOfData);
                _index++;
                OnRecordParsed(new RecordParsedEventArgs(_lastParsedRecord));
            }
            return _lastParsedRecord;
        }

        public ISourceRecord Read(int index)
        {
            MoveToIndex(index);
            return Read();
        }

        public List<ISourceRecord> Read(IndexRange range)
        {
            MoveToIndex(range.RangeBegin);
            List<ISourceRecord> list = new List<ISourceRecord>();
            while(_index < range.RangeEnd)
            {
                ISourceRecord item = Read();
                list.Add(item);
            }
            OnRecordsParsed(new RecordsParsedEventArgs(list));
            return list;
        }

        public event EventHandler<RecordParsedEventArgs> RecordParsed;

        public event EventHandler<RecordsParsedEventArgs> RecordsParsed;

        #endregion
    }
}
/* So what happens when a user asks for records 11-15?
 * The Range object is created and is from 10 to 14;
 *
 * We move to index 10.
 * 
 * We get 10, and then increment the counter for the next iteration.
 * We get 11, "".
 * We get 12, "".
 * We get 13, "".
 * We get 14, "".
 *  
 * Wait...do we get 14? Yes, we are ON 13 in the loop when we get Record 14 then increment the index to 14.
*/