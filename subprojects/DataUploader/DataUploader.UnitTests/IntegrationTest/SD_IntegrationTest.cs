using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using CambridgeSoft.COE.DataLoader.Common;
using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser;
using CambridgeSoft.COE.DataLoader.Core.Workflow;

using CommandLine;
using NUnit.Framework;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.IntegrationTest
{
    [TestFixture]
    public class SD_IntegrationTest
    {
        IndividualArgumentsCommandInfo _parsedArgs = null;
        ErrorReporter _errorReporter = null;
        private string _executableFilePath = string.Empty;
        private string _dataFilePath = string.Empty;
        private List<string> _argumentList = null;
        private string _commandArgumentParsingError = string.Empty;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _executableFilePath = UnitUtils.GetExecutableFilePath();

            _errorReporter = new ErrorReporter(delegate(string errorMsg)
            {
                _commandArgumentParsingError = errorMsg;
            });
        }

        [SetUp]
        public void SetUp()
        {
            _parsedArgs = new IndividualArgumentsCommandInfo();
            _argumentList = new List<string>();

            InitializeTestFile("SAMPLE.sdf");
        }

        [TearDown()]
        public void TearDown()
        {
            if (SourceFieldTypes.TypeDefinitions != null)
                SourceFieldTypes.TypeDefinitions.Clear();
        }

        /// <summary>
        /// COEDataLoader.exe /data:{0} /type:SDFile /act:CountRecords
        /// </summary>
        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void Test_CountRecordsCanWork()
        {
            _dataFilePath = System.IO.Path.Combine(System.IO.Path.Combine(UnitUtils.GetDataFolderPath(), "SD"), "SAMPLE.sdf");
            _argumentList.Add(string.Format("/data:{0}", _dataFilePath));
            _argumentList.Add("/type:SDFile");
            _argumentList.Add("/act:CountRecords");
            
            if (Parser.ParseArguments(_argumentList.ToArray(), _parsedArgs, _errorReporter))
            {
                JobParameters jobParameters = JobCommandInfoConverter.ConvertToJobParameters(_parsedArgs);
                IFileReader reader = FileReaderFactory.FetchReader(jobParameters.DataSourceInformation);
                jobParameters.FileReader = reader;

                CambridgeSoft.COE.DataLoader.Core.Workflow.JobResponse response = SessionEngine.CountRecords(jobParameters);

                Assert.AreEqual(36, (int)response.ResponseContext[CountRecordsService.RECORD_COUNT]);
            }
            else
                Assert.Fail("Failed to parse command-line arguments:" + _commandArgumentParsingError);
        }

        /// <summary>
        /// COEDataLoader.exe /data:{0} /type:SDFile /act:ListFields
        /// </summary>
        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void Test_ListFieldsCanWork()
        {
            _dataFilePath = System.IO.Path.Combine(System.IO.Path.Combine(UnitUtils.GetDataFolderPath(), "SD"), "SAMPLE.sdf");
            _argumentList.Add(string.Format("/data:{0}", _dataFilePath));
            _argumentList.Add("/type:SDFile");
            _argumentList.Add("/act:ListFields");

            if (Parser.ParseArguments(_argumentList.ToArray(), _parsedArgs, _errorReporter))
            {
                JobParameters jobParameters = JobCommandInfoConverter.ConvertToJobParameters(_parsedArgs);

                CambridgeSoft.COE.DataLoader.Core.Workflow.JobResponse response = SessionEngine.ListFields(jobParameters);

                Dictionary<string, Type> typeDefinitions = response.ResponseContext[FieldAndTypeScanService.TYPE_DEFINITIONS] as Dictionary<string, Type>;

                Assert.AreEqual(8, typeDefinitions.Count);
                Assert.AreEqual(typeof(string), typeDefinitions["sd_molecule"]);
                Assert.AreEqual(typeof(int), typeDefinitions["MOLREGNO"]);
                Assert.AreEqual(typeof(string), typeDefinitions["MOLNAME"]);
                Assert.AreEqual(typeof(string), typeDefinitions["CORP_ID"]);
                Assert.AreEqual(typeof(DateTime), typeDefinitions["DATE"]);
                Assert.AreEqual(typeof(string), typeDefinitions["ACTIVITY"]);
            }
            else
                Assert.Fail("Failed to parse command-line arguments:" + _commandArgumentParsingError);
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void Test_CountRecordsCanWorkWithSpacedName()
        {
            string folderPath = Path.Combine(UnitUtils.GetDataFolderPath(), "Folder with spaces");
            InitializeTestFile(Path.Combine(folderPath, "File with spaces.sdf"));

            Test_CountRecordsCanWork();
        }

        private void InitializeTestFile(string fileName)
        {
            _dataFilePath = UnitUtils.GetDataFilePath(fileName);
        }
    }
}
