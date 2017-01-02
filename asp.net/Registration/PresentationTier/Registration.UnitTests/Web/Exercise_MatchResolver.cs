using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using RegistrationWebApp.Code;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.COE.Registration.UnitTests.Web
{
    /// <summary>
    /// The MatchResolver class will be used interactively by a user submitting registrations where
    /// matches are detected by the cartridge.
    /// </summary>
    /// <remarks>
    /// If the compound used to query the database has been registered, it must always be matched.
    /// </remarks>
    [TestFixture]
    public class Exercise_MatchResolver
    {
        private const string REGNUM = "XX-12345";
        private const string BENZENE_SMILES = "C1=CC=CC=C1";
        private const string MXYLENE_SMILES = "CC1=CC(C)=CC=C1";
        private RegistryRecord record;

        /// <summary>
        /// Re-use database-dependent data to speed up testing.
        /// </summary>
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            Helpers.Authentication.Logon("T5_85", "T5_85");
            record = Helpers.Registration.SubmitSampleCompound(MXYLENE_SMILES, REGNUM, false);
        }

        [Test]
        public void Can_Construct_Resolver_From_Matches()
        {
            IMatchResponse response = RegistrationMatcher.GetMatches(record);
            RegistrationMatchResolver mr = new RegistrationMatchResolver(record, response);
            mr.LoadMatch(0);

            Assert.AreEqual(record.RegNum, mr.CurrentMatch.RegNum, "The registered record must match ITSELF");

            bool sameFragments = mr.CanAddBatch;
        }

        //[Test]
        //public void Can_Read_MatchMechanism()
        //{
        //    RegistrationMatchResolver mr = new RegistrationMatchResolver(record);
        //    MatchMechanism mechanism = mr.MatchResponse.MechanismUsed;

        //    Assert.AreEqual(MatchMechanism.Structure, mechanism, "The expected matching mechanism was not used");
        //}

        //[Test]
        //public void Can_Read_FirstMatch()
        //{
        //    RegistrationMatchResolver mr = new RegistrationMatchResolver(record);
        //    MatchMechanism mechanism = mr.MatchResponse.MechanismUsed;

        //    Assert.IsNull(mr.CurrentMatch, "A match has already been loaded");
        //    mr.LoadMatch(0);
        //    RegistryRecord firstMatch = mr.CurrentMatch;
        //    Assert.IsNotNull(mr.CurrentMatch, "The first match could not be loaded");
        //}

        /// <summary>
        /// Eliminate database data created by this fixture.
        /// </summary>
        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            RegistryRecord.DeleteRegistryRecord(record.RegNum);
            Helpers.Authentication.Logoff();
        }
    }
}
