using System.Collections.Generic;
using System.ServiceModel.Security;
using Manager.Code;
using Manager.SpotfireElementManagerService;
using NUnit.Framework;
using System;
using System.Linq;
using System.ServiceModel;

namespace Manager.NUnitTests
{
    [Ignore]
    class SpotfireServiceTest
    {
        private const string SpotfireUrl = "http://shadev437:650";
        private const string UserName = "admin";
        private const string Password = "admin";

        private const string DbInstance = "Oracle_Test";
        private const string DbUrl = "shadev431";
        private const int DbPort = 1521;
        private const string DbSid = "orcl";
        private const string DbUserName = "system";
        private const string DbPassword = "manager2";

        private const string DbSchema = "SCOTT";
        private const string DbTable = "EMP";
        private readonly string[] dbColumnIds = { "EMPNO", "ENAME", "JOB", "MGR", "HIREDATE", "SAL", "COMM", "DEPTNO" };
        private readonly string[] dbColumnTypes = { "Integer", "String", "String", "Integer", "Date", "Integer", "Integer", "Integer" };
        private readonly string[] dbColumnAliases = { "SCOTT_EMPNO", "SCOTT_ENAME", "SCOTT_JOB", "SCOTT_MGR", "SCOTT_HIREDATE", "SCOTT_SAL", "SCOTT_COMM", "SCOTT_DEPTNO" };

        private SpotfireServiceClient client;

        [SetUp]
        public void Setup()
        {
            // Test failed creation spotfire service client
            Assert.Throws<UriFormatException>(() => new SpotfireServiceClient("wrong_url", UserName, Password));
            Assert.Throws<MessageSecurityException>(() =>
            {
                using (var failedClient = new SpotfireServiceClient(SpotfireUrl, UserName, "wrong_password"))
                {
                    failedClient.SpotfireLibraryServiceClient.loadTypes(new[] { "spotfire.datasource", "spotfire.folder", "spotfire.column" });
                }
            });

            client = new SpotfireServiceClient(SpotfireUrl, UserName, Password);
        }

        [TearDown]
        public void Finish()
        {
            if (client != null)
            {
                client.Dispose();
            }
        }

        [Test]
        public void TestSpotfireService_Client()
        {
            var types = client.SpotfireLibraryServiceClient.loadTypes(new[] { "spotfire.datasource", "spotfire.folder", "spotfire.column" });
            Assert.IsTrue(types.Count() == 3);
        }

        [Test]
        public void TestSpotfireService_FolderAPI()
        {
            var folderId = client.CreateFolder("Test Folder");
            Assert.IsNotNullOrEmpty(folderId);

            var duplicatedFolderId = client.CreateFolder("Test Folder");
            Assert.AreEqual(folderId, duplicatedFolderId);

            client.SpotfireLibraryServiceClient.deleteItems(new[] { folderId });
        }

        [Test]
        public void TestSpotfireService_DataSourceAPI()
        {
            var folderId = client.CreateFolder("Test DataSourceFolder");
            Assert.IsNotNullOrEmpty(folderId);

            var datasourceId = client.CreateDataSource(folderId, DbInstance, DbUrl, DbPort, DbSid, DbUserName, DbPassword);
            Assert.IsNotNullOrEmpty(datasourceId);

            var duplicatedDataSourceId = client.CreateDataSource(folderId, DbInstance, DbUrl, DbPort, DbSid, DbUserName, DbPassword);
            Assert.AreEqual(datasourceId, duplicatedDataSourceId);
            VerifyDatasourceItem(datasourceId, folderId, DbInstance, DbUrl, DbPort, DbSid, DbUserName);

            client.SpotfireLibraryServiceClient.deleteItems(new[] { datasourceId });

            datasourceId = client.CreateDataSource(folderId, DbInstance, DbUrl, DbPort, DbSid, DbUserName, DbPassword);
            Assert.IsNotNullOrEmpty(datasourceId);

            var updatedDataSourceId = client.CreateDataSource(folderId, DbInstance, DbUrl, DbPort, DbSid, DbUserName, DbPassword, true);
            Assert.AreNotEqual(datasourceId, updatedDataSourceId);
            VerifyDatasourceItem(updatedDataSourceId, folderId, DbInstance, DbUrl, DbPort, DbSid, DbUserName);

            client.SpotfireLibraryServiceClient.deleteItems(new[] { updatedDataSourceId, folderId });

            Assert.Throws(typeof(FaultException<IMFaultInfo>), () => client.CreateDataSource("test", DbInstance, DbUrl, DbPort, DbSid, DbUserName, DbPassword));
            Assert.Throws(typeof(FaultException<IMFaultInfo>), () => client.CreateDataSource(Guid.NewGuid().ToString(), DbInstance, DbUrl, DbPort, DbSid, DbUserName, DbPassword));
            Assert.Throws(typeof(FaultException<IMFaultInfo>), () => client.CreateDataSource(folderId, DbInstance, "test_url", DbPort, DbSid, DbUserName, DbPassword));
            Assert.Throws(typeof(FaultException<IMFaultInfo>), () => client.CreateDataSource(folderId, DbInstance, DbUrl, 3210, DbSid, DbUserName, DbPassword));
            Assert.Throws(typeof(FaultException<IMFaultInfo>), () => client.CreateDataSource(folderId, DbInstance, DbUrl, DbPort, "test_sid", DbUserName, DbPassword));
            Assert.Throws(typeof(FaultException<IMFaultInfo>), () => client.CreateDataSource(folderId, DbInstance, DbUrl, DbPort, DbSid, "test_userName", DbPassword));
            Assert.Throws(typeof(FaultException<IMFaultInfo>), () => client.CreateDataSource(folderId, DbInstance, DbUrl, DbPort, DbSid, DbUserName, "wrong_password"));
        }

        [Test]
        public void TestSpotfireService_ColumnAPI()
        {
            var folderId = client.CreateFolder("Test DataSourceFolder");
            Assert.IsNotNullOrEmpty(folderId);

            var datasourceId = client.CreateDataSource(folderId, DbInstance, DbUrl, DbPort, DbSid, DbUserName, DbPassword);
            Assert.IsNotNullOrEmpty(datasourceId);

            var columnIds = new List<string>();

            for (int i = 0; i < dbColumnIds.Length; i++)
            {
                var columnId = client.CreateColumn(datasourceId, folderId, DbSchema, DbTable, dbColumnIds[i], dbColumnTypes[i], dbColumnAliases[i]);
                Assert.IsNotNullOrEmpty(columnId);

                var duplicatedColumnId = client.CreateColumn(datasourceId, folderId, DbSchema, DbTable, dbColumnIds[i], dbColumnTypes[i], dbColumnAliases[i]);
                Assert.AreEqual(columnId, duplicatedColumnId);
                VerifyColumnItem(columnId, datasourceId, folderId, DbSchema, DbTable, dbColumnIds[i], dbColumnTypes[i], dbColumnAliases[i]);

                var updatedColumnId = client.CreateColumn(datasourceId, folderId, DbSchema, DbTable, dbColumnIds[i], dbColumnTypes[i], dbColumnAliases[i], updateExisting: true);
                Assert.AreNotEqual(columnId, updatedColumnId);
                VerifyColumnItem(columnId, datasourceId, folderId, DbSchema, DbTable, dbColumnIds[i], dbColumnTypes[i], dbColumnAliases[i]);

                columnIds.Add(updatedColumnId);
            }

            Assert.IsTrue(columnIds.Count == dbColumnIds.Length);

            // Delete all test created columns
            foreach (var id in columnIds)
            {
                client.SpotfireLibraryServiceClient.deleteItems(new[] { id });
            }

            client.SpotfireLibraryServiceClient.deleteItems(new[] { datasourceId, folderId });
        }

        [Test]
        public void TestSpotfireService_DeleteFolder()
        {
            var folderId = client.CreateFolder("Test DataSourceFolder");
            Assert.IsNotNullOrEmpty(folderId);
            client.DeleteFolder(folderId);

            folderId = client.CreateFolder("Test DataSourceFolder");
            Assert.IsNotNullOrEmpty(folderId);
            var subFolderId = client.CreateFolder("Test Sub DataSourceFolder", folderId);
            Assert.IsNotNullOrEmpty(subFolderId);
            client.DeleteFolder(folderId);
        }

        private void VerifyColumnItem(string columnId, string datasourceId, string folderId, string dbSchema, string dbTable, string dbColumnId, string dbColumnType, string dbColumnAlias)
        {
            var column = client.SpotfireElementManagerServiceClient.loadElement(columnId) as Column;
            Assert.IsNotNull(column);
            Assert.IsNotNull(column.sourceColumns);
            Assert.IsTrue(column.sourceColumns.Length == 1);

            var dataSourceColumnElementPath = column.sourceColumns[0];

            Assert.AreEqual(column.parentId, folderId);
            Assert.AreEqual(dataSourceColumnElementPath.schema, dbSchema);
            Assert.AreEqual(dataSourceColumnElementPath.dataSourceId, datasourceId);
            Assert.AreEqual(dataSourceColumnElementPath.table, dbTable);
            Assert.AreEqual(dataSourceColumnElementPath.column, dbColumnId);
            Assert.AreEqual(column.dataType.ToLower(), dbColumnType.ToLower());
            Assert.AreEqual(column.name, dbColumnAlias);
        }

        private void VerifyDatasourceItem(string datasourceId, string folderId, string dbInstance, string dbUrl, int dbPort, string dbSid, string dbUserName)
        {
            var datasource = client.SpotfireElementManagerServiceClient.loadElement(datasourceId) as DataSource;
            Assert.IsNotNull(datasource);

            Assert.AreEqual(datasource.parentId, folderId);
            Assert.AreEqual(datasource.name, dbInstance);
            Assert.AreEqual(datasource.connectionUrl, "jdbc:tibcosoftwareinc:oracle://" + dbUrl + ":" + dbPort + ";SID=" + dbSid);
            Assert.AreEqual(datasource.userName, dbUserName);
        }

    }
}
