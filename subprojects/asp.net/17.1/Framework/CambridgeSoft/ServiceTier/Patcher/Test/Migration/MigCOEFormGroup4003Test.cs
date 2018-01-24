using System.IO;
using System.Xml;
using System.Collections.Generic;

using NUnit.Framework;

using CambridgeSoft.COE.Patcher.Repository;

namespace CambridgeSoft.COE.Patcher.Test
{
    [TestFixture]
    public class MigCOEFormGroup4003Test
    {
        private const string COE_FORM_GROUP_ID = "4003";

        [Test]
        public void Test()
        {
            IConfigurationRepository configRepo = new FileSystemConfigurationRepository(TestUtility.TestFileFolderPath);
            XmlDocument coeFormGroup4003 = configRepo.GetSingleCoeFormGroupById("4003");

            TestUtility.PatchSingleCOEFormGroup<MigCOEFormGroup4003>(coeFormGroup4003);

            coeFormGroup4003.Save(TestUtility.BuildCOEFormGroupOutputFileName(COE_FORM_GROUP_ID));
        }
    }
}
