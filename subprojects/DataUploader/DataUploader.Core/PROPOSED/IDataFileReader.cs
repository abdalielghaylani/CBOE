using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.DataLoader.Core.Contracts;

namespace CambridgeSoft.COE.DataLoader.Core.PROPOSED
{
    /// <summary>
    /// File-parsers must implement this interface to be usable by the Loader CORE API
    /// </summary>
    interface IDataFileReader
    {
        /// <summary>
        /// Represents the last record from the data-file that was actually parsed
        /// </summary>
        ISourceRecord LastParsedRecord { get; }

        /// <summary>
        /// Indicates the reader's 'position' in the file, in terms of a record index.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// The total number of records in the file.
        /// </summary>
        int FileRecordCount { get; }

        /// <summary>
        /// Allows re-positioning of the parser's cursor to a particular index.
        /// </summary>
        /// <param name="index">the zero-based target index</param>
        void MoveToIndex(int index);

        /// <summary>
        /// Retrieves the 'next' record based on the current Index value
        /// </summary>
        /// <returns>an instance of ISourceRecord</returns>
        ISourceRecord Read();

        /// <summary>
        /// Moves the reader to the desired index and retrieves that record
        /// </summary>
        /// <param name="index">the zero-based target index</param>
        /// <returns>an instance of ISourceRecord</returns>
        ISourceRecord Read(int index);

        /// <summary>
        /// Moves the reader to the desired index based on the start of the range,
        /// and retrieves the subsequent records based on the end of the range.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        List<ISourceRecord> Read(IndexRange range);

        /// <summary>
        /// Broadcaster for individual record-parsed notification
        /// </summary>
        event EventHandler<RecordParsedEventArgs> RecordParsed;

        /// <summary>
        /// Broadcaster for recordlist-parsed notification
        /// </summary>
        event EventHandler<RecordsParsedEventArgs> RecordsParsed;
    }
}
