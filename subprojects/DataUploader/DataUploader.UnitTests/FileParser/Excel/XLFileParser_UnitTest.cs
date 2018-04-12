using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser;
using CambridgeSoft.COE.DataLoader.Core.FileParser.Excel;
using CambridgeSoft.COE.DataLoader.Core.FileParser.CSV;

using NUnit.Framework;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.FileParser.Excel
{
    [TestFixture()]
    public class XLFileParser_UnitTest
    {
        IFileReader reader = null;
        const int XL_RECORD_COUNT = 300;
        bool worksheetHasHeaders;

        /// <summary>
        /// Loads a local test file into memory for testing
        /// </summary>
        [SetUp()]
        public void Initialize()
        {
            string filePath = UnitUtils.GetTestFolderPath();

            worksheetHasHeaders = true;
            string fileName = @"excel\MolTable.xlsx";

            //worksheetHasHeaders = false;            
            //string fileName = @"Excel\MolTable_NoHeader.xlsx";

            string fullFilePath = UnitUtils.GetDataFilePath(fileName);
            reader = new ExcelOleDbReader(fullFilePath, "MolTable", MSOfficeVersion.Unknown, worksheetHasHeaders);
            Assert.IsNotNull(reader, "Failed to construct ExcelOleDbReader.");
        }

        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanCountAll()
        {
            int fileRecordCount = reader.CountAll();
            Assert.AreEqual(XL_RECORD_COUNT, fileRecordCount);
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
            reader.Seek(XL_RECORD_COUNT - 1);
            Assert.AreEqual(XL_RECORD_COUNT - 1, reader.CurrentRecordIndex);
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
            Assert.AreEqual("3508", record.FieldSet["MOL_ID"].ToString());
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadLast()
        {
            reader.Rewind();
            reader.Seek(XL_RECORD_COUNT - 1);
            ISourceRecord record = reader.GetNext();
            Assert.AreEqual("300", record.FieldSet["Index"].ToString());
        }

        /// <summary>
        /// Call GetNext() first time to read a chunk and store records in cache, then read a record that not in cache
        /// </summary>
        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadRecord_NotInCache()
        {
            reader.Seek(10);
            Assert.AreEqual("11",reader.GetNext().FieldSet["Index"].ToString());
            reader.Seek(280);
            Assert.AreEqual("281",reader.GetNext().FieldSet["Index"].ToString());
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadForwards()
        {
            ISourceRecord record = null;

            reader.Rewind();

            reader.Seek(2);
            record = reader.GetNext();
            Assert.AreEqual("71286", record.FieldSet["MOL_ID"].ToString());

            reader.Seek(5);
            record = reader.GetNext();
            Assert.AreEqual("4482", record.FieldSet["MOL_ID"].ToString());
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CanReadBackwards()
        {
            ISourceRecord record = null;

            reader.Rewind();
            
            reader.Seek(10);
            record = reader.GetNext();
            Assert.AreEqual("61231", record.FieldSet["MOL_ID"].ToString());

            reader.Seek(5);
            record = reader.GetNext();
            Assert.AreEqual("4482", record.FieldSet["MOL_ID"].ToString());
        }

        //[Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void Test_FindFieldDetermineType()
        {
            Dictionary<string, Type> cols = new Dictionary<string, Type>();
            if (worksheetHasHeaders)
            {
                cols.Add("Boiling Point", typeof(double));
            }
            else
            {
                cols.Add("COLUMN_005", typeof(double));
            }

            int target = 12000;
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

        /// <summary>
        /// Closes the file reader and cleans up the source field definitions cache.
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
