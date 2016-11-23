using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.COE.Registration.UnitTests.BLL
{
    [TestFixture]
    public class PropertyListTests
    {
        private string updatedPropertyListXml_PropertyChanged = @"<MultiCompoundRegistryRecord SameBatchesIdentity=""True"" ActiveRLS=""Off"" IsEditable=""True"">
            <PropertyList><Property name=""REG_COMMENTS"" type=""TEXT"">rccccccccccccc</Property></PropertyList>
</MultiCompoundRegistryRecord>";

        private string originalXml = string.Empty;

        [SetUp]
        public void SetUp()
        {
            Helpers.Authentication.Logon("T5_85", "T5_85");
        }


        [Test]
        public void UpdateFromXml_Should_Update_PropertyListLevel()
        {
            RegistryRecord record = RegistryRecord.GetRegistryRecord("AB-000002");

            if (string.IsNullOrEmpty(originalXml))
                originalXml = record.Xml;

            Assert.IsNotNull(record);

            record.UpdateFromXml(updatedPropertyListXml_PropertyChanged);

            record.Save();

            RegistryRecord updatedRecord = RegistryRecord.GetRegistryRecord("AB-000002");

            Assert.AreEqual("rccccccccccccc", updatedRecord.PropertyList[0].Value);

            if (!string.IsNullOrEmpty(originalXml))
            {
                updatedRecord.Xml = originalXml;
                updatedRecord.Save();
            }
        }


        [TearDown]
        public void TearDown()
        {
            Helpers.Authentication.Logoff();
        }
    }
}
