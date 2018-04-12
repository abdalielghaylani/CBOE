using System;

using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser;
using CambridgeSoft.COE.DataLoader.Core.FileParser.SD;

using NUnit.Framework;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.FileParser.SD
{
    [TestFixture()]
    public class SDFileParser_UnitTest
    {
        IFileReader reader = null;
        IFileReader reader2 = null;
        const int SD_RECORD_COUNT = 36;

        /// <summary>
        /// Loads a local test file into memory for testing
        /// </summary>
        [SetUp()]
        public void Initialize()
        {
            string fullFilePath = UnitUtils.GetDataFilePath(@"SD\sample.sdf");
            reader = new SDFileReader(fullFilePath);
            Assert.IsNotNull(reader, "Failed to construct SDFileReader.");

            fullFilePath = UnitUtils.GetDataFilePath(@"SD\sample2.sdf");
            reader2 = new SDFileReader(fullFilePath);
            Assert.IsNotNull(reader, "Failed to construct 2nd SDFileReader.");
        }

        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanCountAll()
        {
            int fileRecordCount = reader.CountAll();
            Assert.AreEqual(SD_RECORD_COUNT, fileRecordCount);
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
            reader.Seek(SD_RECORD_COUNT - 1);
            Assert.AreEqual(SD_RECORD_COUNT - 1, reader.CurrentRecordIndex);
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
            Assert.AreEqual("Benzene", record.FieldSet["MOLNAME"].ToString());
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadLast()
        {
            reader.Rewind();
            reader.Seek(SD_RECORD_COUNT - 1);
            ISourceRecord record = reader.GetNext();
            Assert.AreEqual("Methyl-2-furoate", record.FieldSet["MOLNAME"].ToString());
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadForwards()
        {
            ISourceRecord record = null;

            reader.Rewind();
            
            reader.Seek(2);
            record = reader.GetNext();
            Assert.AreEqual("Chlorobenzene", record.FieldSet["MOLNAME"].ToString());

            reader.Seek(5);
            record = reader.GetNext();
            Assert.AreEqual("2,5-Dimethyl furan", record.FieldSet["MOLNAME"].ToString());
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadBackwards()
        {
            ISourceRecord record = null;

            reader.Rewind();
            
            reader.Seek(10);
            record = reader.GetNext();
            Assert.AreEqual("(L) Leucine", record.FieldSet["MOLNAME"].ToString());

            reader.Seek(5);
            record = reader.GetNext();
            Assert.AreEqual("2,5-Dimethyl furan", record.FieldSet["MOLNAME"].ToString());
        }

        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadDataheaders()
        {
            if (SourceFieldTypes.TypeDefinitions != null)
                SourceFieldTypes.TypeDefinitions.Clear();

            int count = reader2.CountAll();
            reader2.Rewind();
            reader2.Seek(0);
            ISourceRecord record = reader2.GetNext();
            Assert.IsTrue(record.FieldSet.ContainsKey("DT04"), "DT04 is not a valid field name");
        }

        /// <summary>
        /// Closes the reader and cleans up the source field definition cache.
        /// </summary>
        [TearDown()]
        public void Clean()
        {
            if (reader != null)
                reader.Close();
            if (reader2 != null)
                reader2.Close();
            if (SourceFieldTypes.TypeDefinitions != null)
                SourceFieldTypes.TypeDefinitions.Clear();
        }
    }
}
