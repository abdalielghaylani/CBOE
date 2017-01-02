using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.Registration.Services.Types;

using NUnit.Framework;

namespace CambridgeSoft.COE.Registration.UnitTests.BLL
{
    /// <summary>
    /// This fixture tests the new compound-matching and mixture-matching algorithms and business objects.
    /// These mechanisms assume that all mixtures are comprised only of pre-registered compounds.
    /// </summary>
    [TestFixture]
    public class MatcherTests
    {
        private const string REGNUM = "GD-12345";
        private const string ALT_REGNUM = "BX/54321";
        private const string MIX_REGNUM = "MIX-001";
        private const string NAPROXEN_SMILES = "O=C([C@H](C1=CC2=CC=C(OC)C=C2C=C1)C)[O-]";
        private const string NAPROXEN_SUBSTRUCTURE_SMILES = "C1=CC2=CC=C(OC)C=C2C=C1";
        private const string ALT_SMILES = "Cl[H].C1=CN=NC=C1";

        [TestFixtureSetUp]
        public void SetUp()
        {
            Helpers.Authentication.Logon("T5_85", "T5_85");

            //Prepare some database records for testing.

            //'SubmitSampleCompound(...)' will lazy-load them so we have their details handy, regardless.
            RegistryRecord naproxen =
                Helpers.Registration.SubmitSampleCompound(NAPROXEN_SMILES, REGNUM, false);
            naproxen.Dispose();

            RegistryRecord naproxen2 =
                Helpers.Registration.SubmitSampleCompound(NAPROXEN_SMILES, ALT_REGNUM, false);
            naproxen2.Dispose();

        }

        [Test]
        public void Can_Report_Matches_Via_Interface()
        {
            //Create an in-memory 'sample' Compound to search for matches
            Compound sampleCompound = Component.NewComponent(true).Compound;
            Structure structure = sampleCompound.BaseFragment.Structure;
            structure.DrawingType = DrawingType.Chemical;
            structure.Value = NAPROXEN_SMILES;

            //Perform the search
            IMatchResponse imr = RegistrationMatcher.GetMatches(sampleCompound);

            //Make some test assertions.
            Assert.AreEqual(MatchMechanism.Structure, imr.MechanismUsed
                , "The wrong mechanism was used to match this compound");
            Assert.AreEqual(2, imr.MatchedItems.Count
                , "The wrong number of matches were found");
        }

        [Test]
        public void Can_Report_Matched_Compounds()
        {
            //Create an in-memory 'sample' Compound to search for matches
            Compound sampleCompound = Component.NewComponent(true).Compound;
            Structure structure = sampleCompound.BaseFragment.Structure;
            structure.DrawingType = DrawingType.Chemical;
            structure.Value = NAPROXEN_SMILES;

            //Perform the search
            CompoundMatchResponse cmr = 
                (CompoundMatchResponse)RegistrationMatcher.GetMatches(sampleCompound);

            //Make some test assertions.
            // NOTE: It's up to the caller to subtract 'self' from the matches, IF 'self' is a registered entity!
            Assert.AreEqual(cmr.MechanismUsed, MatchMechanism.Structure);
            Assert.AreEqual(2, cmr.MatchedItems.Count, "The wrong number of compounds were matched");
        }

        [Test]
        public void Can_Report_No_Compound_Matches()
        {
            //Create an in-memory compound unlikely to already exist
            Compound compound = Component.NewComponent(true).Compound;
            compound.BaseFragment.Structure.Value = ALT_SMILES;
            compound.BaseFragment.Structure.Format = "chemical/x-smiles";

            CompoundMatchResponse cmr =
                (CompoundMatchResponse)RegistrationMatcher.GetMatches(compound);

            Assert.AreEqual(cmr.MechanismUsed, MatchMechanism.Structure);
            Assert.AreEqual(cmr.MatchedItems.Count, 0, "The wrong number of compounds were matched");
        }

        [Test]
        public void Can_Report_Matched_Mixture()
        {
            RegistryRecord naproxen =
                Helpers.Registration.SubmitSampleCompound(NAPROXEN_SMILES, REGNUM, false);

            RegistryRecord naproxen2 =
                Helpers.Registration.SubmitSampleCompound(NAPROXEN_SMILES, ALT_REGNUM, false);

            RegistryRecord mixture = null;
            try { mixture = RegistryRecord.GetRegistryRecord(MIX_REGNUM); }
            catch(Exception ex) { string msg = ex.ToString(); }
            finally
            {
                if (mixture == null)
                {
                    //create a mixture
                    RegistryRecord result = RegistryRecord.NewRegistryRecord();
                    string compounds = naproxen.ComponentList[0].Compound.RegNumber.RegNum
                        + "|" + naproxen2.ComponentList[0].Compound.RegNumber.RegNum;
                    string percentages = "23|16";
                    result.CreateMixture(compounds, percentages, "|");
                    result.RegNumber.RegNum = MIX_REGNUM;
                    mixture = result.Register(DuplicateCheck.None);
                }
            }

            List<int> ids = new List<int>(
                new int[2] { naproxen.ComponentList[0].Compound.ID, naproxen2.ComponentList[0].Compound.ID }
            );

            MixtureMatchResponse mmr =
                (MixtureMatchResponse)RegistrationMatcher.GetMatches(mixture);

            //Make some test assertions.
            // NOTE: It's up to the caller to subtract 'self' from the matches, IF 'self' is a registered entity!
            Assert.AreEqual(mmr.MechanismUsed, MatchMechanism.Mixture);
            Assert.AreEqual(mmr.MatchedItems.Count, 1, "The wrong number of mixtures were matched");
        }

        [Test]
        public void Can_Report_No_Mixture_Matches()
        {
            RegistryRecord naproxen =
                Helpers.Registration.SubmitSampleCompound(NAPROXEN_SMILES, REGNUM, false);

            RegistryRecord other =
                Helpers.Registration.SubmitSampleCompound(NAPROXEN_SUBSTRUCTURE_SMILES, "OTHER:01", false);

            RegistryRecord mixture = RegistryRecord.NewRegistryRecord();
            string compounds = naproxen.ComponentList[0].Compound.RegNumber.RegNum
                + "|" + other.ComponentList[0].Compound.RegNumber.RegNum;
            string percentages = "31|47";
            mixture.CreateMixture(compounds, percentages, "|");

            MixtureMatchResponse mmr =
                (MixtureMatchResponse)RegistrationMatcher.GetMatches(mixture);

            Assert.AreEqual(mmr.MechanismUsed, MatchMechanism.Mixture);
            Assert.AreEqual(mmr.MatchedItems.Count, 0, "The wrong number of mixtures were matched");
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Helpers.Authentication.Logoff();
        }
    }
}
