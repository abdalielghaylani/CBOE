using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;

using Microsoft.VisualBasic.FileIO;

namespace CambridgeSoft.COE.DataLoader.Core.FileParser.CSV
{
    /// <summary>
    /// The core class responsibly for reading and parsing CSV (character-separated values)
    /// text-only data-files.
    /// </summary>
    public class CSVFileReader : FileReaderBase
    {
        /// <summary>
        /// Typically, a worksheet without a header has column names starting with "F1"
        /// and ending in "F##" where ## represents the number of columns found. This
        /// string is used as a prefix to replace such column names.
        /// </summary>
        private const string FIELD_AUTO_PREFIX = "COLUMN_";

        /// <summary>
        /// The underlying stream wrapper which orchestrates record reading and parsing.
        /// </summary>
        private TextFieldParser _textParser;

        /// <summary>
        /// Provided by the constructor for files with a fixed-width format.
        /// This property is mutually exclusive of <see cref="_fieldDelimiters"/>.
        /// </summary>
        private int[] _fieldWidths;

        /// <summary>
        /// Provided by the constructor for files with a character-delimited format.
        /// This property is mutually exclusive of <see cref="_fieldWidths"/>.
        /// </summary>
        private string[] _fieldDelimiters;

        #region > Properties <

        private List<string> _fieldNames = new List<string>();
        /// <summary>
        /// The field names, either discovered or artifically created.
        /// </summary>
        public List<string> FieldNames
        {
            get { return _fieldNames; }
        }

        private bool _hasHeaderRow;
        /// <summary>
        /// Indicator of presence/absence of a row with explicit field names.
        /// </summary>
        public bool HasHeaderRow
        {
            get
            {
                if (_textParser != null && _textParser.TextFieldType != FieldType.Delimited)
                    _hasHeaderRow = false;
                return _hasHeaderRow;
            }
        }

        #endregion

        #region > Constructors <

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pathToFile">the path to a character-separated values file</param>
        /// <param name="fieldDelimiters">one or more strings acting as the field-delimiter</param>
        /// <param name="hasHeaderRow">
        /// the caller will provide true if the field names are the first row in the file
        /// </param>
        public CSVFileReader(string pathToFile, string[] fieldDelimiters, bool hasHeaderRow)
            : base(pathToFile)
        {
            _fieldDelimiters = fieldDelimiters;
            _hasHeaderRow = hasHeaderRow;
            Initialize();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataStream">a stream representing a character-separated values file</param>
        /// <param name="fieldDelimiters">one or more strings acting as the field-delimiter</param>
        /// <param name="hasHeaderRow">
        /// the caller will provide true if the field names are the first row in the file
        /// </param>
        public CSVFileReader(Stream dataStream, string[] fieldDelimiters, bool hasHeaderRow)
            : base(dataStream)
        {
            _fieldDelimiters = fieldDelimiters;
            _hasHeaderRow = hasHeaderRow;
            Initialize();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pathToFile">the path to a fixed-width format file</param>
        /// <param name="fieldWidths">the lengths of the fixed-width fields, from left to right</param>
        public CSVFileReader(string pathToFile, int[] fieldWidths)
            : base(pathToFile)
        {
            _fieldWidths = fieldWidths;
            Initialize();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataStream">a stream representing a fixed-width format file</param>
        /// <param name="fieldWidths">the lengths of the fixed-width fields, from left to right</param>
        public CSVFileReader(Stream dataStream, int[] fieldWidths)
            : base(dataStream)
        {
            _fieldWidths = fieldWidths;
            Initialize();
        }

        private void Initialize()
        {
            _textParser = new TextFieldParser(_stream, Encoding.UTF8, true, true);
            _textParser.TrimWhiteSpace = true;

            if (_fieldDelimiters != null)
                _textParser.SetDelimiters(_fieldDelimiters);
            if (_fieldWidths != null)
                _textParser.SetFieldWidths(_fieldWidths);
        }

        #endregion

        #region > IFileReader<ISourceRecord> Members <

        public override void ReadNext()
        {
            //be sure we have already read or interpreted the header row
            if (_textParser.LineNumber == 1)
                ReadHeader();            

            string[] values = null;
            if (!_textParser.EndOfData)
            {
                this.OnRecordParsing(new RecordParsingEventArgs(_currentRecordIndex));
                values = _textParser.ReadFields();

                _currentRecordIndex++;

                ExtractRecord(values);

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
            return Current; ;
        }

        public override void Seek(int targetRecordIndex)
        {
            //bypass fast-forwarding if the index being sought is the current index
            if (targetRecordIndex == this._currentRecordIndex)
                return;

            //if we require a record we have already read past, rewind before continuing
            if (targetRecordIndex < this._currentRecordIndex)
                Rewind();

            if (_textParser.LineNumber == 1 && _hasHeaderRow)
                ReadHeader();

            while (_currentRecordIndex < targetRecordIndex && !_textParser.EndOfData)
            {
                string[] buf = this._textParser.ReadFields();
                _currentRecordIndex++;
            }

            if (_textParser.EndOfData)
            {
                // Set the record index back to the beginning, otherwise it would be in an invalid state,
                // i.e. at the end of the file.
                Rewind();

                throw new IndexOutOfRangeException();
            }
        }

        public override void Rewind()
        {
            this._stream.Position = 0;
            this._currentRecordIndex = 0;
            this._parsedRecordCount = 0;
            this._current = null;
            // Unfortunately, we have to re-initialize every time, otherwise MS' TextFieldParse will just fail to read fields.
            this.Initialize();
        }

        public override int CountAll()
        {
            this.Rewind();
            this._totalRecordCount = CountAllInternal();
            this.Rewind();
            return this._totalRecordCount;
        }

        public override void Close()
        {
            this._stream.Position = 0;
            this._currentRecordIndex = 0;
            this._parsedRecordCount = 0;
            this._textParser.Close();
            this._stream.Close();
        }

        #endregion

        private void ExtractRecord(string[] rowValues)
        {
            _current = new CSVSourceRecord(this.CurrentRecordIndex, rowValues);

            if (rowValues != null && rowValues.Length > 0)
            {
                for (int fieldIndex = 0; fieldIndex < rowValues.Length; fieldIndex++)
                {
                    string key = _fieldNames[fieldIndex];
                    SourceFieldTypes.SetValue(key, rowValues[fieldIndex], _current);
                }
            }
        }

        private void ReadHeader()
        {
            string[] columns = _textParser.ReadFields();
         
            if (_hasHeaderRow)
                _fieldNames = new List<string>(columns);
            else
            {
                //discover the number of fields available               
                _fieldNames = new List<string>();
                if (SourceFieldTypes.TypeDefinitions.Count > 0)
                {
                    foreach (KeyValuePair<string, Type> kvp in SourceFieldTypes.TypeDefinitions)
                    {
                        _fieldNames.Add(kvp.Key);
                    }
                }
                else
                {
                    int numColumns = columns.Length;
                    char padChar = "0".ToCharArray()[0];
                    for (int fieldIndex = 0; fieldIndex < numColumns; fieldIndex++)
                    {
                        _fieldNames.Add(FIELD_AUTO_PREFIX + (fieldIndex + 1).ToString().PadLeft(3, padChar));
                    }
                }
                //reset the parser so we can read the same line as data;
                Rewind();
            }
            //verify the filed names to validate the delimiter
            if (_hasHeaderRow || SourceFieldTypes.TypeDefinitions.Count > 0)
            {
                foreach (string key in _fieldNames)
                {
                    if (key.Contains("\t") || key.Contains("\n") || key.Contains(","))
                        throw new Exception("invalid delimiter");
                }
            }
        }

        private int CountAllInternal()
        {
            int count = 0;
            string[] line = null;
            while ((line = _textParser.ReadFields()) != null)
            {
                count++;
            }

            // Exclude the header from record counting.
            if (HasHeaderRow)
                count--;
            return count;
        }

    }
}
