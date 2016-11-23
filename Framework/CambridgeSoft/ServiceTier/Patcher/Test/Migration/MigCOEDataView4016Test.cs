using System;
using System.Collections.Generic;
using System.Xml;

using NUnit.Framework;

using CambridgeSoft.COE.Patcher.Repository;

namespace CambridgeSoft.COE.Patcher.Test
{
    [TestFixture]
    public class MigCOEDataView4016Test
    {
        [Test]
        public void Test()
        {
            IConfigurationRepository configRepo = new FileSystemConfigurationRepository(TestUtility.TestFileFolderPath);
            XmlDocument coeDataView4016 = configRepo.GetSingleCOEDataViewById("4016");

            TestUtility.PatchSingleCOEDataView<MigCOEDataView4016>(coeDataView4016);

            coeDataView4016.Save(TestUtility.BuildCOEDataViewOutputFileName("4016"));
        }
    }
}
