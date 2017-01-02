using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using CambridgeSoft.COE.RegistrationAdmin.Services;

namespace CambridgeSoft.COE.Registration.UnitTests.Web
{
    [TestFixture]
    public class Exercise_RegNumSequencer
    {
        /// <summary>
        /// Re-use database-dependent data to speed up testing.
        /// </summary>
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            //Helpers.Authentication.Logon("T5_85", "T5_85");
        }

        [Test]
        public void Can_Create_RegNumSequencer()
        {
            RegNumSequencer rns = new RegNumSequencer();
            bool valid;
            int errCnt;
            string errors;

            errCnt = rns.BrokenRulesCollection.ErrorCount;
            errors = rns.BrokenRulesCollection.ToString();
            Assert.True(errCnt == 0, "There should be no validation errors");

            //alter prefix
            rns.Prefix = "RG";
            errCnt = rns.BrokenRulesCollection.ErrorCount;
            errors = rns.BrokenRulesCollection.ToString();
            Assert.True(errCnt == 1, "We have introduced an error that was not detected");

            rns.Prefix = null;
            errCnt = rns.BrokenRulesCollection.ErrorCount;
            errors = rns.BrokenRulesCollection.ToString();
            Assert.True(errCnt == 0, "We should have removed all validation errors");
        }

        /// <summary>
        /// Eliminate database data created by this fixture.
        /// </summary>
        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            //Helpers.Authentication.Logoff();
        }
    }
}
