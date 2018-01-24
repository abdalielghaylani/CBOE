using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.COE.Registration.UnitTests.BLL
{
    [TestFixture]
    public class ProjectListTests
    {
        #region XML


        #endregion

        private string originalXml = string.Empty;

        [SetUp]
        public void SetUp()
        {
            Helpers.Authentication.Logon("T5_85", "T5_85");
        }

        [Test]
        public void UpdateFromXml_Should_Update_ProjectListLevel()
        {
            //haven't implemented.
        }


        [TearDown]
        public void TearDown()
        {
            Helpers.Authentication.Logoff();
        }
    }
}
