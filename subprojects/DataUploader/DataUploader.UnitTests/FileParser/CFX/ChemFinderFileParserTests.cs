using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser;
using CambridgeSoft.COE.DataLoader.Core.FileParser.CFX;
using CambridgeSoft.COE.DataLoader.Core.FileParser.CSV;

using NUnit.Framework;
using CambridgeSoft.COE.DataLoader.Core;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.FileParser.CFX
{
    /// <summary>
    /// Tests the core functionality provided by the CFXFileReader class for reading
    /// ChemFinder12 files.
    /// </summary>
    [TestFixture()]
    public class ChemFinderFileParserTests
    {
        IFileReader reader = null;
        const int MOLTABLECFX_RECORD_COUNT = 285;

        /// <summary>
        /// Loads a local test file into memory for testing
        /// </summary>
        [SetUp()]
        public void Initialize()
        {
            string fullFilePath = UnitUtils.GetDataFilePath("CS_DEMO.cfx");
            reader = new CFXFileReader(fullFilePath);
            Assert.IsNotNull(reader, "Failed to construct CFX reader.");
        }

        /// <summary>
        /// Determines the record-count of the file in question.
        /// </summary>
        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanCountAll()
        {
            int fileRecordCount = reader.CountAll();
            Assert.AreEqual(fileRecordCount, MOLTABLECFX_RECORD_COUNT);
            Console.WriteLine("CountAll() found {0} records", fileRecordCount.ToString());
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadBackwards()
        {
            ISourceRecord record = null;

            reader.Rewind();

            reader.Seek(10);
            record = reader.GetNext();
            Assert.AreEqual("Pyrazole", record.FieldSet["Molname"].ToString());

            reader.Seek(5);
            record = reader.GetNext();
            Assert.AreEqual("Cyclopentane", record.FieldSet["Molname"].ToString());
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadFirst()
        {
            reader.Rewind();
            reader.Seek(0);
            ISourceRecord record = reader.GetNext();
            Assert.AreEqual("Benzene", record.FieldSet["Molname"].ToString());
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadForwards()
        {
            ISourceRecord record = null;

            reader.Rewind();

            reader.Seek(2);
            record = reader.GetNext();
            Assert.AreEqual("Furan", record.FieldSet["Molname"].ToString());

            reader.Seek(5);
            record = reader.GetNext();
            Assert.AreEqual("Cyclopentane", record.FieldSet["Molname"].ToString());
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadLast()
        {
            reader.Rewind();
            reader.Seek(MOLTABLECFX_RECORD_COUNT - 1);
            ISourceRecord record = reader.GetNext();
            Assert.AreEqual("C70 (D5h) fullerene", record.FieldSet["Molname"].ToString());
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
        public void CanSeekFirst()
        {
            reader.Rewind();
            reader.Seek(0);
            Assert.AreEqual(0, reader.CurrentRecordIndex);
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
        public void CanSeekLast()
        {
            reader.Rewind();
            reader.Seek(MOLTABLECFX_RECORD_COUNT - 1);
            Assert.AreEqual(MOLTABLECFX_RECORD_COUNT - 1, reader.CurrentRecordIndex);
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
                Assert.AreEqual(285, reader.CurrentRecordIndex);
                Assert.Pass();
            }
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

/*

        /// <summary>
        /// Positions the reader at the record given by the target index minus 1 (using
        /// a zero-based counter), making the subsequent read the sought index (1-based).
        /// </summary>
        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void Test_SeekAndFetchRecord()
        {
            int target = 12;
            reader.Seek(target);
            ISourceRecord record = reader.GetNext();

            Assert.AreEqual(record.SourceIndex, target);
            Console.WriteLine("Seek() ended at record indicated by {0}", record.SourceIndex);
        }

        /// <summary>
        /// Tries to advance through the file beyond its known end-point
        /// </summary>
        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void Test_SeekBeyondEndOfFile()
        {
            int target = 900;
            reader.Seek(target);

            //the reader should know to terminate the Seek if it is beyond the record count
            Assert.AreEqual(reader.CurrentRecordIndex, CSV_RECORD_COUNT);
            Console.WriteLine("Seek() ended at record {0}", reader.CurrentRecordIndex);
        }

        /// <summary>
        /// Given a specified number of record to read, check if the evaluated
        /// data-type matches the expected data-type.
        /// </summary>
        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void Test_FindFieldDetermineType()
        {
            Dictionary<string, Type> cols = new Dictionary<string, Type>();
            cols.Add("RefNumber", typeof(int));

            int target = 20;
            do
            {
                ISourceRecord record = reader.GetNext();
            } while (reader.CurrentRecordIndex < target);

            foreach (string soughtFieldKey in cols.Keys)
            {
                Assert.True(SourceFieldTypes.TypeDefinitions.ContainsKey(soughtFieldKey));
                Type t = SourceFieldTypes.TypeDefinitions[soughtFieldKey];
                Assert.True(t == cols[soughtFieldKey]);
                Console.WriteLine("{0} column found as type {1}", soughtFieldKey, t.FullName);
            }
        }

*/