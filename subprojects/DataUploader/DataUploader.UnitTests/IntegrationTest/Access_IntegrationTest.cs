using System.Collections.Generic;

using NUnit.Framework;
using CambridgeSoft.COE.DataLoader.Common;
using CommandLine;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using CambridgeSoft.COE.DataLoader.Core;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.IntegrationTest
{
    [TestFixture]
    public class Access_IntegrationTest
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

            InitializeTestFile("CS_DEMO.mdb");
        }

        [TearDown()]
        public void TearDown()
        {
            if (SourceFieldTypes.TypeDefinitions != null)
                SourceFieldTypes.TypeDefinitions.Clear();
        }

        /// <summary>
        /// COEDataLoader.exe /data:{0} /type:MSAccess /act:ListTables
        /// </summary>
        //[Test]
        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void Test_ListTablesCanWork()
        {
            _argumentList.Add(string.Format("/data:{0}", _dataFilePath));
            _argumentList.Add("/type:MSAccess");
            _argumentList.Add("/act:ListTables");

            if (Parser.ParseArguments(_argumentList.ToArray(), _parsedArgs, _errorReporter))
            {
                JobParameters jobParameters = JobCommandInfoConverter.ConvertToJobParameters(_parsedArgs);

                CambridgeSoft.COE.DataLoader.Core.Workflow.JobResponse response = SessionEngine.ListTables(jobParameters);

                List<string> tableNameList = response.ResponseContext[TablesDiscoveryService.RESPONSE_TABLENAMESLIST] as List<string>;

                Assert.IsNotNull(tableNameList, "Table name list is null");
                Assert.AreEqual(2, tableNameList.Count);
                Assert.AreEqual("MolTable", tableNameList[0]);
                Assert.AreEqual("Synonyms", tableNameList[1]);
            }
            else
                Assert.Fail("Failed to parse command-line arguments:" + _commandArgumentParsingError);
        }

        private void InitializeTestFile(string fileName)
        {
            _dataFilePath = UnitUtils.GetDataFilePath(fileName);
        }
    }
}
