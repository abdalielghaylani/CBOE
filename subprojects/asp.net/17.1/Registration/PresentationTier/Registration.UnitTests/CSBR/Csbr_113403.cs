using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.Registration.Services.Types;

using NUnit.Framework;

namespace CambridgeSoft.COE.Registration.UnitTests
{
    [TestFixture]
    public class Csbr_113403
    {
        [SetUp]
        public void SetUp()
        {
            Helpers.Authentication.Logon("T5_85", "T5_85");
        }
        
        [Test]
        public void Can_Store_BatchStatusId()
        {
            RegistryRecord rr = RegistryRecord.GetRegistryRecord("AB-000001");
            rr.BatchList[0].Status = RegistryStatus.Approved;
            RegistryRecord rr2 = rr.Save();
            Assert.AreEqual(rr2.BatchList[0].Status, RegistryStatus.Approved);
        }

        [TearDown]
        public void TearDown()
        {
            Helpers.Authentication.Logoff();
        }
    }
}
