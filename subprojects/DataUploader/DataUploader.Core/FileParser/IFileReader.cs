using System;

namespace CambridgeSoft.COEDataLoader.FileParser
{
    public interface IFileReader<T>
        where T : ISourceRecord
    {
        /// <summary>
        /// The count of records that have been parsed.
        /// </summary>
        int ParsedRecordCount { get;set;}

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
        T ReadNextTo();

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
        /// Traverses the entire file stream.
        /// Increments only the counter for records encountered (since it does not parse any of them) – internally, this may do a Rewind() and then a FastForward(int.MaxValue)
        /// </summary>
        /// <returns></returns>
        int CountAll();

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
