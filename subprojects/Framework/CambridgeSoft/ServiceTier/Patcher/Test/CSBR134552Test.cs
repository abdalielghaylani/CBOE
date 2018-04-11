using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CambridgeSoft.COE.Patcher.Repository;
using System.Xml;

namespace CambridgeSoft.COE.Patcher.Test
{
    [TestFixture]
    public class CSBR134552Test
    {
        const string COE_FORM_GROUP_ID = "4002";
        [Test]
        public void Test()
        {
            IConfigurationRepository configRepo = new FileSystemConfigurationRepository(TestUtility.TestFileFolderPath);
            XmlDocument coeFormGroup4002 = configRepo.GetSingleCoeFormGroupById(COE_FORM_GROUP_ID);

            TestUtility.PatchSingleCOEFormGroup<CSBR134552>(coeFormGroup4002);

            coeFormGroup4002.Save(TestUtility.BuildCOEFormGroupOutputFileName(COE_FORM_GROUP_ID));
        }
    }
}
