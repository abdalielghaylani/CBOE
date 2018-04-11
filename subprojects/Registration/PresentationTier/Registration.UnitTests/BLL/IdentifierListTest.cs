using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.COE.Registration.UnitTests.BLL
{
    [TestFixture]
    public class IdentifierListTest
    {
         private string updatedIdentifierListXml= @"<MultiCompoundRegistryRecord> 
 <IdentifierList>
    <Identifier>
      <ID>1</ID>
      <IdentifierID Description=""Alias"" Name=""Alias"" Active=""T"">3</IdentifierID>
      <InputText>a</InputText>
    </Identifier>
    <Identifier>
      <ID>3</ID>
      <IdentifierID Description=""Alias"" Name=""Alias"" Active=""T"">3</IdentifierID>
      <InputText>fsdfsf</InputText>
    </Identifier>
    <Identifier>
      <ID>2</ID>
      <IdentifierID Description=""Alias"" Name=""Alias"" Active=""T"">3</IdentifierID>
      <InputText>bb</InputText>
    </Identifier>
    <Identifier>
      <ID>2</ID>
      <IdentifierID Description=""Alias"" Name=""Alias"" Active=""T"">3</IdentifierID>
      <InputText>cc</InputText>
    </Identifier>
  </IdentifierList>
</MultiCompoundRegistryRecord>";

        private string originalXml = string.Empty;

        [SetUp]
        public void SetUp()
        {
            Helpers.Authentication.Logon("T5_85", "T5_85");
        }

        [Test]
        public void UpdateFromXml_Should_Update_IdentifierListLevel()
        {
            RegistryRecord record = RegistryRecord.GetRegistryRecord("AB-000002");

            if (string.IsNullOrEmpty(originalXml))
                originalXml = record.Xml;

            record.UpdateFromXml(updatedIdentifierListXml);

            record.Save();

            RegistryRecord updatedRecord = RegistryRecord.GetRegistryRecord("AB-000002");

            Assert.AreEqual(4, record.IdentifierList.Count);
        }


        [TearDown]
        public void TearDown()
        {
            if (!string.IsNullOrEmpty(originalXml))
            {
                RegistryRecord record = RegistryRecord.GetRegistryRecord("AB-000002");
                record.Xml = originalXml;
                record.Save();
            }

            Helpers.Authentication.Logoff();
        }
    }
}
