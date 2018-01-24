using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CambridgeSoft.COE.Patcher.Repository;
using System.Xml;

namespace CambridgeSoft.COE.Patcher.Test
{
    [TestFixture]
    public class MigCOEObjectConfigTest
    {
        [Test]
        public void Test()
        {
            IConfigurationRepository configRepo = new FileSystemConfigurationRepository(TestUtility.TestFileFolderPath);
            XmlDocument coeObjectConfig = configRepo.GetCoeObjectConfig();

            TestUtility.RunSinglePatcher<MigCOEObjectConfig>(
                null,
                null,
                null,
                coeObjectConfig,
                null);

            coeObjectConfig.Save(TestUtility.BuildCOEObjectConfigFileName());
        }
    }
}
