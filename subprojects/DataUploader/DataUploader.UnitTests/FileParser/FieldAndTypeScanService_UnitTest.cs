using System;
using System.Collections.Generic;

using NUnit.Framework;

using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser.SD;
using CambridgeSoft.COE.DataLoader.Core.FileParser.CSV;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using CambridgeSoft.COE.DataLoader.Core.FileParser;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.FileParser
{
    public class FieldAndTypeScanService_UnitTest
    {
        IFileReader reader = null;

        /// <summary>
        /// Closes the reader and cleans up the source field definition cache.
        /// </summary>
        [TearDown()]
        public void Clean()
        {
            if (reader != null)
                reader.Close();
            if (SourceFieldTypes.TypeDefinitions != null)
                SourceFieldTypes.TypeDefinitions.Clear();
        }

        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void SDScanWorks()
        {
            SourceFileInfo sourceFileInfo = new SourceFileInfo(UnitUtils.GetDataFilePath(@"SD\sample.sdf"), SourceFileType.SDFile);
            reader = FileReaderFactory.FetchReader(sourceFileInfo);
            Assert.IsNotNull(reader, "Failed to construct SDFileReader.");

            JobParameters jobParameters = new JobParameters();
            jobParameters.ActionRanges = new IndexRange(0, 100).ToIndexList().ToIndexRanges();
            jobParameters.DataSourceInformation = sourceFileInfo;
            jobParameters.FileReader = reader;

            JobResponse response = JobServiceCaller.ListFields(jobParameters);

            Dictionary<string, Type> cols = new Dictionary<string, Type>();
            cols.Add("DATE", typeof(DateTime));

            foreach (string soughtFieldKey in cols.Keys)
            {
                Assert.True(SourceFieldTypes.TypeDefinitions.ContainsKey(soughtFieldKey));
                Type t = SourceFieldTypes.TypeDefinitions[soughtFieldKey];
                Assert.True(t == cols[soughtFieldKey]);
            }
        }

        /// <summary>
        /// Given a specified number of record to read, check if the evaluated
        /// data-type matches the expected data-type.
        /// </summary>
        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void CSVScanWorks()
        {
            SourceFileInfo sourceFileInfo = new SourceFileInfo(UnitUtils.GetDataFilePath(@"csv\AGROBASE.txt"), SourceFileType.CSV);
            sourceFileInfo.FieldDelimiters = new string[] { "\t" };
            sourceFileInfo.HasHeaderRow = true;
            reader = FileReaderFactory.FetchReader(sourceFileInfo);
            Assert.IsNotNull(reader, "Failed to construct CSVFileReader.");

            JobParameters jobParameters = new JobParameters();
            jobParameters.ActionRanges = new IndexRange(0, 100).ToIndexList().ToIndexRanges();
            jobParameters.DataSourceInformation = sourceFileInfo;
            jobParameters.FileReader = reader;

            JobResponse response = JobServiceCaller.ListFields(jobParameters);

            Dictionary<string, Type> cols = new Dictionary<string, Type>();
            cols.Add("RefNumber", typeof(int));

            foreach (string soughtFieldKey in cols.Keys)
            {
                Assert.True(SourceFieldTypes.TypeDefinitions.ContainsKey(soughtFieldKey));
                Type t = SourceFieldTypes.TypeDefinitions[soughtFieldKey];
                Assert.True(t == cols[soughtFieldKey]);
            }
        }

        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void ExcelScanWorks()
        {
            SourceFileInfo sourceFileInfo = new SourceFileInfo(UnitUtils.GetDataFilePath(@"excel\moltable.xlsx"), SourceFileType.MSExcel);
            sourceFileInfo.HasHeaderRow = true;
            sourceFileInfo.TableName = "MolTable";
            reader = FileReaderFactory.FetchReader(sourceFileInfo);
            Assert.IsNotNull(reader, "Failed to construct ExcelFileReader.");

            JobParameters jobParameters = new JobParameters();
            jobParameters.ActionRanges = new IndexRange(0, 100).ToIndexList().ToIndexRanges();
            jobParameters.DataSourceInformation = sourceFileInfo;
            jobParameters.FileReader = reader;

            JobResponse response = JobServiceCaller.ListFields(jobParameters);

            Dictionary<string, Type> cols = new Dictionary<string, Type>();
            cols.Add("Melting Point", typeof(double));

            foreach (string soughtFieldKey in cols.Keys)
            {
                Assert.True(SourceFieldTypes.TypeDefinitions.ContainsKey(soughtFieldKey));
                Type t = SourceFieldTypes.TypeDefinitions[soughtFieldKey];
                Assert.True(t == cols[soughtFieldKey]);
            }
        }
    }
}
