using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.DataLoader.Core.FileParser;
using CambridgeSoft.COE.DataLoader.Core.FileParser.CSV;

using NUnit.Framework;
using CambridgeSoft.COE.DataLoader.Core;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.FileParser.CSV
{
    [TestFixture()]
    public class CSVTranslatorTest
    {
        /// <summary>
        /// the number of records available from the sample CSV look-up file.
        /// </summary>
        const int CSV_RECORD_COUNT = 25;

        /// <summary>
        /// Generates a Dictionary of look-up values from a CSV file.
        /// </summary>
        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void Test_CreateDictionaryFromCsv()
        {
            string fullFilePath = UnitUtils.GetDataFilePath(@"csv\AGROBASE.txt");
            Dictionary<string, string> synonyms =
                CSVTranslator.DictionaryFromTranslationFile(fullFilePath, "\u0009", "RefNumber", "CodeNos");

            int synonymCount = synonyms.Keys.Count;
            Assert.AreEqual(synonymCount, CSV_RECORD_COUNT, "Created the the wrong number of KeyValuePairs");

            foreach (KeyValuePair<string, string> pair in synonyms)
                Console.WriteLine("Key '{0}' = '{1}'", pair.Key, pair.Value);
        }

        /// <summary>
        /// Generates a look-up DataTable from a CSV file.
        /// </summary>
        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void Test_CreateTableFromCsv()
        {
            string fullFilePath = UnitUtils.GetDataFilePath(@"csv\AGROBASE.txt");
            System.Data.DataTable dt = CSVTranslator.TableFromTranslationFile(fullFilePath, "\u0009");

            int fileRecordCount = dt.Rows.Count;
            Assert.AreEqual(fileRecordCount, CSV_RECORD_COUNT, "Parsed the wrong number of records");
        }

        /// <summary>
        /// Chains the individual members of the CSVTranslator class, more as a sample of how to do
        /// thsi when necessary.
        /// </summary>
        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void Test_CreateDictionaryIndirectly()
        {
            string fullFilePath = UnitUtils.GetDataFilePath(@"csv\AGROBASE.txt");
            Dictionary<string, string> synonyms = CSVTranslator.DictionaryFromTable(
                CSVTranslator.TableFromTranslationFile(fullFilePath, "\u0009"), "RefNumber", "CodeNos"
            );

            int synonymCount = synonyms.Keys.Count;
            Assert.AreEqual(synonymCount, CSV_RECORD_COUNT, "Created the the wrong number of KeyValuePairs");

            foreach (KeyValuePair<string, string> pair in synonyms)
                Console.WriteLine("Key '{0}' = '{1}'", pair.Key, pair.Value);
        }

        /// <summary>
        /// Clears the static cache held by all CSVTranslator dictionaries.
        /// </summary>
        [TearDown()]
        public void Clean()
        {
            CSVTranslator.ClearCache();
        }

    }
}
