using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CambridgeSoft.COE.Patcher.Repository;
using System.Xml;

namespace CambridgeSoft.COE.Patcher.Test
{
    [TestFixture]
    public class MigCOEDataView4003Test
    {
        [Test]
        public void Test()
        {
            IConfigurationRepository configRepo = new FileSystemConfigurationRepository(TestUtility.TestFileFolderPath);
            XmlDocument coeDataView4003 = configRepo.GetSingleCOEDataViewById("4003");

            TestUtility.PatchSingleCOEDataView<MigCOEDataView4003>(coeDataView4003);

            coeDataView4003.Save(TestUtility.BuildCOEDataViewOutputFileName("4003"));
        }
    }
}
