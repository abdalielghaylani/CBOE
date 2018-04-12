using System;
using System.Collections.Generic;

namespace CambridgeSoft.COE.DataLoader.Core.Contracts
{
    /// <summary>
    /// This contract describes the reading and parsing of chemical data files into individual
    /// records (typicall single compounds).
    /// </summary>
    /// <typeparam name="T">the type which will be populated by the parsing</typeparam>
    public interface IFileReader
    {
        /// <summary>
        /// The count of records that have been parsed.
        /// </summary>
        int ParsedRecordCount { get; }

        /// <summary>
        /// The count of records that have been encountered.
        /// </summary>
        int CurrentRecordIndex { get; }

        /// <summary>
        /// The last record parsed.
        /// </summary>
        ISourceRecord Current { get; }

        /// <summary>
        /// Extracts the subsequent record data from the stream into an object instance of type <see cref="ISourceRecord"/>.
        /// Optionally broadcasts a RecordParsing event, before the real parsing happens.
        /// After parsing out a record, broadcasts this as a RecordParsed event, with the object instance included in the event argument.
        /// Increments two internal counters to keep track of the number of records that were both (1) encountered and (2) actually parsed.
        /// </summary>
        void ReadNext();

        /// <summary>
        /// Extracts the subsequent record data from the stream into an object instance of type <see cref="ISourceRecord"/>.
        /// Optionally broadcasts a RecordParsing event, before the real parsing happens.
        /// After parsing out a record, broadcasts this as a RecordParsed event, with the object instance included in the event argument.
        /// Increments two internal counters to keep track of the number of records that were both (1) encountered and (2) actually parsed.
        /// </summary>
        /// <returns>The parsed out object instance of type <see cref="ISourceRecord"/></returns>
        ISourceRecord GetNext();

        /// <summary>
        /// Extracts a range of source record data into a list.
        /// </summary>
        /// <param name="range">The range object representing the records to extract</param>
        /// <returns>A list of extracted source records</returns>
        List<ISourceRecord> ExtractToRecords(IndexRange range);

        /// <summary>
        /// Moves the stream cursor forward to a specific record index in the file.
        /// Increments only the counter for records encountered (since it does not parse any of them).
        /// </summary>
        /// <param name="recordIndex">Index of the record to move the cursor to.</param>
        void Seek(int recordIndex);

        /// <summary>
        /// Moves the stream cursor back to the beginning of the stream.
        /// Resets all internal counters.
        /// </summary>
        void Rewind();

        /// <summary>
        /// Releases all the resources used by the reader.
        /// Resets all internal counters.
        /// </summary>
        void Close();

        /// <summary>
        /// Traverses the entire file stream.
        /// Increments only the counter for records encountered (since it does not parse any of them) – internally, this may do a Rewind() and then a FastForward(int.MaxValue)
        /// </summary>
        /// <returns></returns>
        int CountAll();

        /// <summary>
        /// The product of <see cref="CountAll"/> should be retrievable from this proeprty.
        /// </summary>
        int RecordCount { get; }

        /// <summary>
        /// Raised before a single record is to be parsed.
        /// </summary>
        event EventHandler<RecordParsingEventArgs> RecordParsing;

        /// <summary>
        /// Raised after a single record has been parsed.
        /// </summary>
        event EventHandler<RecordParsedEventArgs> RecordParsed;
    }
}
