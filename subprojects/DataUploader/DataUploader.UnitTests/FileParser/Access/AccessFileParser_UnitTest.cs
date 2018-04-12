using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser;
using CambridgeSoft.COE.DataLoader.Core.FileParser.Access;
using CambridgeSoft.COE.DataLoader.Core.FileParser.CSV;

using NUnit.Framework;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.FileParser.Access
{
    [TestFixture()]
    public class AccessFileParser_UnitTest
    {
        IFileReader _reader = null;
        const int ACCESS_RECORD_COUNT = 285;

        /// <summary>
        /// Loads a local test file into memory for testing
        /// </summary>
        [SetUp()]
        public void Initialize()
        {
            string fullFilePath = UnitUtils.GetDataFilePath("CS_DEMO.mdb");
            _reader = new AccessOleDbReader(fullFilePath, "MolTable", MSOfficeVersion.Unknown);
            Assert.IsNotNull(_reader, "Failed to construct AccessOleDbReader.");
        }

        /// <summary>
        /// The Reader should be able to accurately count the records in the given file/table combination
        /// </summary>
        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanCountAll()
        {
            int fileRecordCount = _reader.CountAll();
            Assert.AreEqual(ACCESS_RECORD_COUNT, fileRecordCount);
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadBackwards()
        {
            _reader.Rewind();
            _reader.Seek(10);
            _reader.Seek(3);
            ISourceRecord record = _reader.GetNext();
            Assert.AreEqual("Thiophene", record.FieldSet["Molname"].ToString());
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadFirst()
        {
            _reader.Rewind();
            _reader.Seek(0);
            ISourceRecord record = _reader.GetNext();
            Assert.AreEqual("Benzene", record.FieldSet["Molname"].ToString());
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadForwards()
        {
            _reader.Rewind();
            _reader.Seek(1);
            _reader.Seek(3);
            ISourceRecord record = _reader.GetNext();
            Assert.AreEqual("Thiophene", record.FieldSet["Molname"].ToString());
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadLast()
        {
            _reader.Rewind();
            _reader.Seek(ACCESS_RECORD_COUNT - 1);
            ISourceRecord record = _reader.GetNext();
            Assert.AreEqual("C70 (D5h) fullerene", record.FieldSet["Molname"].ToString());
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanSeekBackwards()
        {
            _reader.Rewind();
            _reader.Seek(10);
            _reader.Seek(5);
            Assert.AreEqual(5, _reader.CurrentRecordIndex);
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanSeekFirst()
        {
            _reader.Rewind();
            _reader.Seek(0);
            Assert.AreEqual(0, _reader.CurrentRecordIndex);
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanSeekForwards()
        {
            _reader.Rewind();
            _reader.Seek(5);
            _reader.Seek(10);
            Assert.AreEqual(10, _reader.CurrentRecordIndex);
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanSeekLast()
        {
            _reader.Rewind();
            _reader.Seek(ACCESS_RECORD_COUNT - 1);
            Assert.AreEqual(ACCESS_RECORD_COUNT - 1, _reader.CurrentRecordIndex);
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void SeekWhenIndexOutOfRange()
        {
            _reader.Rewind();
            try
            {
                _reader.Seek(ACCESS_RECORD_COUNT);
                Assert.Fail("This seek method is expected to throw an IndexOutOfRangeException");
            }
            catch (IndexOutOfRangeException)
            {
                Assert.AreEqual(0, _reader.CurrentRecordIndex);
                Assert.Pass();
            }
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanAddFieldWithAllNullValues()
        {
            _reader.Rewind();
            while (_reader.GetNext() != null)
            {
                _reader.ReadNext();
            }

            Assert.AreEqual(true,SourceFieldTypes.TypeDefinitions.ContainsKey("NullValue_Column"));
            Assert.AreEqual(null, SourceFieldTypes.TypeDefinitions["NullValue_Column"]);
        }

        /// <summary>
        /// Closes the reader and cleans up the source field definition cache.
        /// </summary>
        [TearDown()]
        public void Clean()
        {
            if (_reader != null)
                _reader.Close();
            if (SourceFieldTypes.TypeDefinitions != null)
                SourceFieldTypes.TypeDefinitions.Clear();
        }
    }
}
