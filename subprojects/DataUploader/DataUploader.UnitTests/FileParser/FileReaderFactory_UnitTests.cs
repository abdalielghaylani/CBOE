using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser;
using CambridgeSoft.COE.DataLoader.Core.FileParser.Access;
using CambridgeSoft.COE.DataLoader.Core.FileParser.CFX;
using CambridgeSoft.COE.DataLoader.Core.FileParser.Excel;

using NUnit.Framework;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.FileParser
{
    /// <summary>
    /// Exercises the <see cref="FileReaderFactory"/>
    /// </summary>
    [TestFixture()]
    public class FileReaderFactoryTests
    {
        //TODO: Continue creating unit tests for the FileReaderFactory.
        /*
         * Further tests should include:
         * --> empty Excel worksheet - should be invalid, OR give count 0
         * --> empty CSV file - should give count 0
         * --> empty SDFile - should give count 0
         * */

        string filePath = null;
        IFileReader reader = null;
        const int ACCESS_RECORD_COUNT = 285;
        const int ACCESS_TABLE_COUNT = 2;

        /// <summary>
        /// Define the base path for the test data-files.
        /// </summary>
        [SetUp]
        public void Initialize()
        {
            filePath = UnitUtils.GetDataFolderPath();
        }

        #region > Positive controls <

        /// <summary>
        /// The factory must return a valid reader of the appropriate type.
        /// </summary>
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        [Test()]
        public void MSAccess_Reader_PositiveControl()
        {
            string fileName = "CS_DEMO.mdb";
            string fullFilePath = System.IO.Path.Combine(filePath, fileName);
            SourceFileInfo sfi = new SourceFileInfo(fullFilePath, SourceFileType.MSAccess, "MolTable", false);
            reader = FileReaderFactory.FetchReader(sfi);

            Assert.NotNull(reader, "Error creating reader");
            Assert.IsInstanceOf<AccessOleDbReader>(reader, "Not an instance of MSAccess OleDb file reader");
        }

        /// <summary>
        /// The factory should find the appropriate number of tables in the given database file.
        /// </summary>
        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void MSAccess_TableList_PositiveControl()
        {
            string fileName = "CS_DEMO.mdb";
            string fullFilePath = System.IO.Path.Combine(filePath, fileName);
            SourceFileInfo sfi = new SourceFileInfo(fullFilePath, SourceFileType.MSAccess, null, false);
            string[] tableNames = AccessOleDbReader.FetchTableNames(sfi).ToArray();
            string allTables = string.Join(",", tableNames);

            Assert.AreEqual(tableNames.Length, ACCESS_TABLE_COUNT, "Incorrect number of tables {0}", allTables);
            Console.WriteLine("Tables found: {0}", allTables);
        }

        /// <summary>
        /// The factory must return a valid reader of the appropriate type.
        /// </summary>
        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void MSExcel_Reader_PositiveControl()
        {
            string fileName = "Moltable_NoHeader.xlsx";
            string fullFilePath = System.IO.Path.Combine(filePath, fileName);
            SourceFileInfo sfi = new SourceFileInfo(fullFilePath, SourceFileType.MSExcel, "MolTable", false);
            reader = FileReaderFactory.FetchReader(sfi);

            Assert.NotNull(reader, "Error creating reader");
            Assert.IsInstanceOf<ExcelOleDbReader>(reader, "Not an instance of MSExcel OleDb file reader");
        }

        /// <summary>
        /// The factory must return a valid reader of the appropriate type.
        /// </summary>
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        [Test()]
        public void ChemFinderDotNet_Reader_PositiveControl()
        {
            string fileName = "CS_DEMO.cfx";
            string fullFilePath = System.IO.Path.Combine(filePath, fileName);
            SourceFileInfo sfi = new SourceFileInfo(fullFilePath, SourceFileType.ChemFinder);
            reader = FileReaderFactory.FetchReader(sfi);

            Assert.NotNull(reader, "Error creating reader");
            Assert.IsInstanceOf<CFXFileReader>(reader, "Not an instance of MSAccess/Molserver CFX file reader");
        }

        #endregion

        #region > Invalid file names <

        /// <summary>
        /// The factory should return NULL if no such file exists at the expected path.
        /// </summary>
        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void MSAccess_Reader_Invalid_File_Name()
        {
            string fileName = "CS_DEMO_X.mdb";
            string fullFilePath = System.IO.Path.Combine(filePath, fileName);
            SourceFileInfo sfi = new SourceFileInfo(fullFilePath, SourceFileType.MSAccess, "MolTable", false);
            reader = FileReaderFactory.FetchReader(sfi);

            Assert.Null(reader, "Erroneous MSAccess OleDb file reader created");
        }

        /// <summary>
        /// The factory should return NULL if the table or view requested does not exist.
        /// </summary>
        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void MSAccess_Reader_Invalid_Table_Name()
        {
            string fileName = "CS_DEMO.mdb";
            string fullFilePath = System.IO.Path.Combine(filePath, fileName);
            SourceFileInfo sfi = new SourceFileInfo(fullFilePath, SourceFileType.MSAccess, "NoSuchTableName", false);
            reader = FileReaderFactory.FetchReader(sfi);

            Assert.Null(reader, "Erroneous MSAccess OleDb file reader created");
        }

        #endregion

        /// <summary>
        /// Close any open readers and re-initialize any static variables re: source field types.
        /// </summary>
        [TearDown()]
        public void Clean()
        {
            if (reader != null)
                reader.Close();
            SourceFieldTypes.TypeDefinitions.Clear();
            reader = null;
        }
    }
}
