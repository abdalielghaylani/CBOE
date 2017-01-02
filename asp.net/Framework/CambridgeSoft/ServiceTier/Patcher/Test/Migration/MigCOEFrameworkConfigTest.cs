using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CambridgeSoft.COE.Patcher.Repository;
using System.Xml;

namespace CambridgeSoft.COE.Patcher.Test
{
    [TestFixture]
    public class MigCOEFrameworkConfigTest
    {
        [Test]
        public void Test()
        {
            IConfigurationRepository configRepo = new FileSystemConfigurationRepository(TestUtility.TestFileFolderPath);
            XmlDocument coeFrameworkConfig = configRepo.GetFrameworkConfig();

            TestUtility.RunSinglePatcher<MigCOEFrameworkConfig>(
                null,
                null,
                null,
                null,
                coeFrameworkConfig);

            coeFrameworkConfig.Save(TestUtility.BuildCOEFrameworkConfigFileName());
        }
    }
}
