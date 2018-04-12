using System;
using System.IO;
using System.Collections.Generic;
using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Workflow;

using NUnit.Framework;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.FileParser
{
    /// <summary>
    /// Test the functionality of a job service which lists the names of all the tables in an Access file, 
    /// or worksheets in an Excel file.
    /// </summary>
    [TestFixture]
    public class TablesDiscoveryJob_UnitTest
    {
        /// <summary>
        /// Test this job service againt an Access file named CS_DEMO.mdb in the TestFiles folder.
        /// This test succeed if the job service finds that there are exactly two tables in the Access file,
        /// that is, it does not return the automatically created tables.
        /// </summary>
        [Test]
        public void Test_AccessTableNames()
        {
            string accessSourceFilePath = UnitUtils.GetDataFilePath("CS_DEMO.mdb");
            SourceFileInfo dataSourceInfomation = new SourceFileInfo(accessSourceFilePath, SourceFileType.MSAccess);
            JobParameters jobParameters = new JobParameters(dataSourceInfomation, null, null);
            TablesDiscoveryService tablesDiscoveryJob = new TablesDiscoveryService(jobParameters);
            CambridgeSoft.COE.DataLoader.Core.Workflow.JobResponse response = tablesDiscoveryJob.DoJob();
            List<string> tableNamesList = (List<string>)response.ResponseContext[TablesDiscoveryService.RESPONSE_TABLENAMESLIST];
            Assert.AreEqual(2,tableNamesList.Count);
        }

        /// <summary>
        /// Test this job service againt an Excel file named WorksheetsName.xlsx in the TestFiles folder.
        /// </summary>
        /// <remarks>
        /// This test succeed if the tested job service return a worksheet list that contains exactly three worksheets:General,General$,EndWith$
        /// </remarks>
        [Test]
        public void Test_ExcelWorkSheetNames()
        {
            string excelSourceFilePath = UnitUtils.GetDataFilePath("WorksheetsName.xlsx"); 
            SourceFileInfo dataSourceInfomation = new SourceFileInfo(excelSourceFilePath, SourceFileType.MSExcel);
            JobParameters jobParameters = new JobParameters(dataSourceInfomation, null, null);
            TablesDiscoveryService tablesDiscoveryJob = new TablesDiscoveryService(jobParameters);
            CambridgeSoft.COE.DataLoader.Core.Workflow.JobResponse response = tablesDiscoveryJob.DoJob();
            List<string> tableNamesList = (List<string>)response.ResponseContext[TablesDiscoveryService.RESPONSE_TABLENAMESLIST];
            Assert.AreEqual(3, tableNamesList.Count);
            Assert.True(tableNamesList.Contains("EndWith$"));
            Assert.True(tableNamesList.Contains("General"));
            Assert.True(tableNamesList.Contains("General$"));
        }
    }
}
