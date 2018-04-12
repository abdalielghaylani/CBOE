using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser;
using CambridgeSoft.COE.DataLoader.Core.FileParser.CSV;

using NUnit.Framework;
using CambridgeSoft.COE.DataLoader.Core;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.FileParser.CSV
{
    /// <summary>
    /// Tests the core functionality provided by the CSVSourceReader class for reading
    /// character-delimited files.
    /// </summary>
    [TestFixture()]
    public class CSVFileParser_UnitTest
    {
        IFileReader reader = null;
        const int CSV_RECORD_COUNT = 25;
        int recordIndex = -1;

        /// <summary>
        /// Loads a local test file into memory for testing
        /// </summary>
        [SetUp()]
        public void Initialize()
        {
            string fullFilePath = UnitUtils.GetDataFilePath(@"csv\AGROBASE.txt");
            reader = new CSVFileReader(fullFilePath, new string[] { "\t" }, true);
            Assert.IsNotNull(reader, "Failed to construct CSVFileReader.");

            if (reader != null)
            {
                reader.RecordParsing += new EventHandler<RecordParsingEventArgs>(reader_RecordParsing);
                reader.RecordParsed += new EventHandler<RecordParsedEventArgs>(reader_RecordParsed);
            }

        }

        /// <summary>
        /// Part of internal test for event-handlers.
        /// </summary>
        /// <param name="sender">the IFileReader instance broadcasting the event</param>
        /// <param name="e">the index of the previously-read source file record</param>
        private void reader_RecordParsing(object sender, RecordParsingEventArgs e)
        {
            recordIndex = e.SourceRecordIndex;
        }

        /// <summary>
        /// Part of internal test for event-handlers.
        /// </summary>
        /// <param name="sender">the IFileReader instance broadcasting the event</param>
        /// <param name="e">the most recently-read ISourceRecord instance, complete with data-values</param>
        private void reader_RecordParsed(object sender, RecordParsedEventArgs e)
        {
            Assert.AreEqual(
                recordIndex + 1, e.SourceRecord.SourceIndex
                , "Index mismatch between 'RecordParsingEventArgs' (before) and 'RecordParsedEventArgs' (after)");
        }

        /// <summary>
        /// Determines the record-count of the file in question.
        /// </summary>
        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanCountAll()
        {
            int fileRecordCount = reader.CountAll();
            Assert.AreEqual(fileRecordCount,CSV_RECORD_COUNT);
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanSeekFirst()
        {
            reader.Rewind();
            reader.Seek(0);
            Assert.AreEqual(0, reader.CurrentRecordIndex);
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanSeekLast()
        {
            reader.Rewind();
            reader.Seek(CSV_RECORD_COUNT - 1);
            Assert.AreEqual(CSV_RECORD_COUNT - 1, reader.CurrentRecordIndex);
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanSeekForwards()
        {
            reader.Rewind();
            reader.Seek(5);
            reader.Seek(10);
            Assert.AreEqual(10, reader.CurrentRecordIndex);
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanSeekBackwards()
        {
            reader.Rewind();
            reader.Seek(10);
            reader.Seek(5);
            Assert.AreEqual(5, reader.CurrentRecordIndex);
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void SeekWhenIndexOutOfRange()
        {
            reader.Rewind();
            try
            {
                reader.Seek(900);
                Assert.Fail("This seek method is expected to throw an IndexOutOfRangeException");
            }
            catch (IndexOutOfRangeException)
            {
                Assert.AreEqual(0, reader.CurrentRecordIndex);
                Assert.Pass();
            }
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadFirst()
        {
            reader.Rewind();
            reader.Seek(0);
            ISourceRecord record = reader.GetNext();
            Assert.AreEqual("C9H17N5S", record.FieldSet["Formula"].ToString());
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadLast()
        {
            reader.Rewind();
            reader.Seek(CSV_RECORD_COUNT - 1);
            ISourceRecord record = reader.GetNext();
            Assert.AreEqual("C10H19N5S", record.FieldSet["Formula"].ToString());
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadForwards()
        {
            ISourceRecord record = null;

            reader.Rewind();

            reader.Seek(2);
            record = reader.GetNext();
            Assert.AreEqual("C17H24NNaO5", record.FieldSet["Formula"].ToString());

            reader.Seek(5);
            record = reader.GetNext();
            Assert.AreEqual("C12H16ClNOS", record.FieldSet["Formula"].ToString());
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadBackwards()
        {
            ISourceRecord record = null;

            reader.Rewind();

            reader.Seek(10);
            record = reader.GetNext();
            Assert.AreEqual("C13H24N4O3S", record.FieldSet["Formula"].ToString());

            reader.Seek(5);
            record = reader.GetNext();
            Assert.AreEqual("C12H16ClNOS", record.FieldSet["Formula"].ToString());
        }

        /// <summary>
        /// Closes the testing file, resources, and clears the static cache held by
        /// the FieldDictionary's GlobalEntries dictionary.
        /// </summary>
        [TearDown()]
        public void Clean()
        {
            if (reader != null)
                reader.Close();
            if (SourceFieldTypes.TypeDefinitions != null)
                SourceFieldTypes.TypeDefinitions.Clear();
        }
    }
}
