using System.Xml;
using System.Collections.Generic;

using NUnit.Framework;

using CambridgeSoft.COE.Patcher.Repository;

namespace CambridgeSoft.COE.Patcher.Test
{
    [TestFixture]
    public class MigCOEFormGroup4012Test
    {
        private const string COE_FORM_GROUP_ID = "4012";

        [Test]
        public void Test()
        {
            IConfigurationRepository repository = new FileSystemConfigurationRepository(TestUtility.TestFileFolderPath);
            XmlDocument coeFormGroup4012 = repository.GetSingleCoeFormGroupById("4012");

            TestUtility.PatchSingleCOEFormGroup<MigCOEFormGroup4012>(coeFormGroup4012);

            coeFormGroup4012.Save(TestUtility.BuildCOEFormGroupOutputFileName(COE_FORM_GROUP_ID));
        }
    }
}
