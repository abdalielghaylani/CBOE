using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.DataLoader.Core.FileParser.CSV
{
    public class CSVSourceRecord : SourceRecordBase
    {
        #region > Properties, private members <

        /// <summary>
        /// Internal container for the raw data, in lines, from a CSV record.
        /// </summary>
        private List<string> _rawValues;

        private string[] _delimiters;
        /// <summary>
        /// The list of field- and field value-delimiters used by the parent file.
        /// </summary>
        public string[] Delimiters
        {
            get { return _delimiters; }
            set { _delimiters = value; }
        }

        #endregion

        public CSVSourceRecord(int recordIndex, List<string> rawRecordValues)
            : base(recordIndex)
        {
            if (rawRecordValues.Count > 0)
                _rawValues = rawRecordValues;
        }

        public CSVSourceRecord(int recordIndex, string[] rawRecordValues)
            :base(recordIndex)
        {
            _rawValues = new List<string>(rawRecordValues);
        }

        /// <summary>
        /// Creates an SDFile record from the MolFile and Fields contents, producing a value
        /// for the RawRecord property.
        /// </summary>
        public string ComposeRecord()
        {
            string buf = string.Empty;

            //initialize using the first line of data
            StringBuilder sdBuilder = new StringBuilder();

            //add each property field sequentially
            foreach (string key in _fieldSet.Keys)
            {
                //provide placeholders for missing field data
                object rawValue = null;
                if (this._fieldSet.TryGetValue(key, out rawValue) != false)
                    if (rawValue != null && rawValue != DBNull.Value)
                    {
                        string rawString = rawValue.ToString();
                        if (!string.IsNullOrEmpty(rawString))
                            sdBuilder.AppendLine(rawString);
                    }
                else
                {

                }
            }
            //add the end-of-record marker and a CrLf
            buf = sdBuilder.ToString();

            return buf;
        }

    }
}
