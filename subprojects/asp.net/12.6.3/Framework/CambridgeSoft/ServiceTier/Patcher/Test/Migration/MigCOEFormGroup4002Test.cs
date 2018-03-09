using System.IO;
using System.Xml;
using System.Collections.Generic;

using NUnit.Framework;

using CambridgeSoft.COE.Patcher.Repository;

namespace CambridgeSoft.COE.Patcher.Test
{
    [TestFixture]
    public class MigCOEFormGroup4002Test
    {
        private const string COE_FORM_GROUP_ID = "4002";

        [Test]
        public void Test()
        {
            IConfigurationRepository configRepo = new FileSystemConfigurationRepository(TestUtility.TestFileFolderPath);
            XmlDocument coeFormGroup4002 = configRepo.GetSingleCoeFormGroupById(COE_FORM_GROUP_ID);

            TestUtility.PatchSingleCOEFormGroup<MigCOEFormGroup4002>(coeFormGroup4002);

            coeFormGroup4002.Save(TestUtility.BuildCOEFormGroupOutputFileName(COE_FORM_GROUP_ID));
        }
    }
}
