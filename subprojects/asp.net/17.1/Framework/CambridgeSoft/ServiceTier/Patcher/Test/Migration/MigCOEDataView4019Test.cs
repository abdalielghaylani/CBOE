using System;
using System.Collections.Generic;
using System.Xml;

using NUnit.Framework;

using CambridgeSoft.COE.Patcher.Repository;

namespace CambridgeSoft.COE.Patcher.Test
{
    [TestFixture]
    public class MigCOEDataView4019Test
    {
        [Test]
        public void Test()
        {
            IConfigurationRepository configRepo = new FileSystemConfigurationRepository(TestUtility.TestFileFolderPath);
            XmlDocument coeDataView4019 = configRepo.GetSingleCOEDataViewById("4019");

            TestUtility.PatchSingleCOEDataView<MigCOEDataView4019>(coeDataView4019);

            coeDataView4019.Save(TestUtility.BuildCOEDataViewOutputFileName("4019"));
        }
    }
}
