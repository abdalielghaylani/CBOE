using System;
using System.IO;
using System.Collections.Generic;

using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;

namespace CambridgeSoft.COE.DataLoader.Core.FileParser.SD
{
    /// <summary>
    /// Performs a stream-based reader for compounds in SDFIle format. The 
    /// </summary>
    public class SDFileReader : FileReaderBase
    {
        private const int MAX_RECORD_BYTE_SIZE = 1000000;
        private const string SD_END_OF_RECORD_SYMBOL = "$$$$";

        /// <summary>
        /// Used to traverse the source data stream.
        /// </summary>
        private StreamReader _reader;

        #region > Constructors <

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filePath">the path to a SDFile</param>
        public SDFileReader(string filePath)
            : base(filePath)
        {
            _reader = new StreamReader(this._stream);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataStream">a stream representing an SDFile</param>
        public SDFileReader(Stream dataStream)
            : base(dataStream)
        {
            _reader = new StreamReader(this._stream);
        }

        #endregion

        #region > IFileReader<ISourceRecord> Members <

        public override void ReadNext()
        {
            this.OnRecordParsing(new RecordParsingEventArgs(_currentRecordIndex));

            List<string> lines = GetRawRecordLines();

            //ensure the true 'end of record' was actually encountered
            if (lines.Count != 0 && lines[lines.Count - 1] == SD_END_OF_RECORD_SYMBOL)
            {
                //increment the index so it can be used for the Extracted record
                _currentRecordIndex++;

                //extract the record
                ExtractRecord(lines);

                //maintain counters
                _parsedRecordCount++;

                this.OnRecordParsed(new RecordParsedEventArgs(_current));
            }
            else
            {
                _current = null;
            }
        }

        public override ISourceRecord GetNext()
        {
            ReadNext();
            return Current;
        }

        public override void Seek(int targetRecordIndex)
        {
            //bypass fast-forwarding if the index being sought is the current index
            if (targetRecordIndex == this._currentRecordIndex)
                return;

            //if we require a record we have already read past, rewind before continuing
            if (targetRecordIndex < this._currentRecordIndex)
                Rewind();

            //continually retrieve the next record's raw data until we either:
            // (1) find the record by its index in the file, or
            // (2) reach the end of the file
            while (this._currentRecordIndex < targetRecordIndex && !_reader.EndOfStream)
            {
                List<string> lines = GetRawRecordLines();
                _currentRecordIndex++;
            }

            if (_reader.EndOfStream)
            {
                // Set the record index back to the beginning, otherwise it would be in an invalid state,
                // i.e. at the end of the file.
                Rewind();
                //Commented the below line because When there was one record in the sdf file then it is throwing error.
                //throw new IndexOutOfRangeException();
            }
            //do
            //{
            //    List<string> lines = GetRawRecordLines();
            //    _currentRecordIndex++;
            //} while (
            //    this._currentRecordIndex < targetRecordIndex
            //    && !_reader.EndOfStream
            //);
        }

        public override void Rewind()
        {
            this._reader.DiscardBufferedData();
            this._stream.Position = 0;
            this._currentRecordIndex = 0;
            this._parsedRecordCount = 0;
            this._current = null;
        }

        public override int CountAll()
        {
            this.Rewind();
            //this.Seek(int.MaxValue);
            //this._totalRecordCount = this._currentRecordIndex;
            this._totalRecordCount = CountAllInternal();
            this.Rewind();

            return this._totalRecordCount;
        }

        public override void Close()
        {
            this._stream.Position = 0;
            this._currentRecordIndex = 0;
            this._parsedRecordCount = 0;
            this._reader.Close();
        }

        #endregion

        /// <summary>
        /// Traverses the source stream and accumulates lines until reaching an end-of-record marker.
        /// </summary>
        /// <returns>a list of lines of data representing one SDFile record</returns>
        private List<string> GetRawRecordLines()
        {
            List<string> lines = new List<string>();
            int bytesRead = 0;
            bool reachedBreakpoint = false;

            while (!reachedBreakpoint && bytesRead < MAX_RECORD_BYTE_SIZE)
            {
                string lineOfData = _reader.ReadLine();
                reachedBreakpoint = (lineOfData == null);
                if (!reachedBreakpoint)
                {
                    lines.Add(lineOfData);
                    bytesRead += (lineOfData.Length);
                    reachedBreakpoint = (lineOfData == SD_END_OF_RECORD_SYMBOL);
                }
            }
            return lines;
        }

        /// <summary>
        /// Creates an SDSourceRecord instance, initializing it with raw data and allowing
        /// it to extract the molecule and all custom properties.
        /// </summary>
        /// <param name="lines">the raw data representing one logical SDFile record</param>
        private void ExtractRecord(List<string> lines)
        {
            _current = new SDSourceRecord(this.CurrentRecordIndex, lines);
        }

        private int CountAllInternal()
        {
            int count = 0;
            string line = null;
            while ((line = _reader.ReadLine()) != null)
            {
                if (line == SD_END_OF_RECORD_SYMBOL)
                    count++;
            }

            return count;
        }

    }
}
