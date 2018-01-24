using CambridgeSoft.COE.RegistrationAdmin.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Text;

namespace RegistrationAdmin.Services.MSUnitTests
{
    /// <summary>
    ///This is a test class for RegNumSequencerTest and is intended
    ///to contain all RegNumSequencerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RegNumSequencerTest
    {
        static int sequenceId;
        CambridgeSoft.COE.RegistrationAdmin.Access.RegAdminOracleDAL regDAl = null;
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void RegNumSequencerTestInitialize(TestContext testContext)
        {
            //insert a new sequence record in the database for unit testing purpose
            StringBuilder strSql = new StringBuilder();
            strSql.Append("INSERT INTO REGDB.SEQUENCE (PREFIX, NEXT_IN_SEQUENCE, ACTIVE, BATCHDELIMETER, PREFIX_DELIMITER, REGNUMBER_LENGTH,");
            strSql.Append("DUP_CHECK_LOCAL, SUFFIX, SUFFIXDELIMITER, SALTSUFFIXTYPE, OBJECTTYPE, EXAMPLE, SITEID, TYPE, BATCHNUMBER_LENGTH, SORTORDER, AUTOSELCOMPDUPCHK) VALUES ");
            strSql.Append("('TEST', '1', 'T', '/', '-', '6', '0', null, '.',  'code', null, 'TEST123456', null, 'C','6', '1', 'F')");
            CambridgeSoft.COE.RegistrationAdmin.Access.RegAdminOracleDAL regDAl = null;
            Helpers.Helpers.GetRegDal(ref regDAl, "");
            regDAl.DALManager.Database.ExecuteNonQuery(CommandType.Text, strSql.ToString());
            sequenceId = Helpers.Helpers.GetMaxSequenceNumberFromSequences();
        }

        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void RegNumSequencerTestCleanup()
        {
            //delete the sequence record from database that was inserted for unit testing purpose.
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("DELETE FROM REGDB.SEQUENCE WHERE SEQUENCE_ID='{0}'", sequenceId);
            CambridgeSoft.COE.RegistrationAdmin.Access.RegAdminOracleDAL regDAl = null;
            Helpers.Helpers.GetRegDal(ref regDAl, "");
            regDAl.DALManager.Database.ExecuteNonQuery(CommandType.Text, strSql.ToString());
        }

        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Helpers.Helpers.GetRegDal(ref regDAl, "");
        }
        
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        public void MyTestCleanup()
        {
            regDAl = null;
        }
        
        #endregion

        /// <summary>
        ///A test for TranslateBatchPaddingSetting
        ///</summary>
        [TestMethod()]
        public void TranslateBatchPaddingSetting_TwoDigitsTest()
        {
            RegNumSequencer target = new RegNumSequencer(); // TODO: Initialize to an appropriate value
            RegNumSequencer.BatchNumberPadding padding = RegNumSequencer.BatchNumberPadding.TwoDigits; // TODO: Initialize to an appropriate value
            int expected = 2; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.TranslateBatchPaddingSetting(padding);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TranslateBatchPaddingSetting
        ///</summary>
        [TestMethod()]
        public void TranslateBatchPaddingSetting_ThreeDigitsTest()
        {
            RegNumSequencer target = new RegNumSequencer(); // TODO: Initialize to an appropriate value
            RegNumSequencer.BatchNumberPadding padding = RegNumSequencer.BatchNumberPadding.ThreeDigits; // TODO: Initialize to an appropriate value
            int expected = 3; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.TranslateBatchPaddingSetting(padding);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TranslateBatchPaddingSetting
        ///</summary>
        [TestMethod()]
        public void TranslateBatchPaddingSetting_FourDigitsTest()
        {
            RegNumSequencer target = new RegNumSequencer(); // TODO: Initialize to an appropriate value
            RegNumSequencer.BatchNumberPadding padding = RegNumSequencer.BatchNumberPadding.FourDigits; // TODO: Initialize to an appropriate value
            int expected = 4; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.TranslateBatchPaddingSetting(padding);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TranslateBatchPaddingSetting
        ///</summary>
        [TestMethod()]
        public void TranslateBatchPaddingSetting_FiveDigitsTest()
        {
            RegNumSequencer target = new RegNumSequencer(); // TODO: Initialize to an appropriate value
            RegNumSequencer.BatchNumberPadding padding = RegNumSequencer.BatchNumberPadding.FiveDigits; // TODO: Initialize to an appropriate value
            int expected = 5; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.TranslateBatchPaddingSetting(padding);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TranslateBatchPaddingSetting
        ///</summary>
        [TestMethod()]
        public void TranslateBatchPaddingSetting_SixDigitsTest()
        {
            RegNumSequencer target = new RegNumSequencer(); // TODO: Initialize to an appropriate value
            RegNumSequencer.BatchNumberPadding padding = RegNumSequencer.BatchNumberPadding.SixDigits; // TODO: Initialize to an appropriate value
            int expected = 6; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.TranslateBatchPaddingSetting(padding);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TranslateBatchPaddingSetting
        ///</summary>
        [TestMethod()]
        public void TranslateBatchPaddingSetting_noneTest()
        {
            RegNumSequencer target = new RegNumSequencer(); // TODO: Initialize to an appropriate value
            RegNumSequencer.BatchNumberPadding padding = RegNumSequencer.BatchNumberPadding.None; // TODO: Initialize to an appropriate value
            int expected = -1; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.TranslateBatchPaddingSetting(padding);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetRegNumSequencer
        ///</summary>
        [TestMethod()]
        public void GetRegNumSequencer_CreateNewSequenceTest()
        {
            CambridgeSoft.COE.RegistrationAdmin.Access.RegAdminOracleDAL regDAl = null;
            Helpers.Helpers.GetRegDal(ref regDAl, "");
            int sequenceId = 0; // TODO: Initialize to an appropriate value
            RegNumSequencer actual;
            actual = RegNumSequencer.GetRegNumSequencer(sequenceId);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for GetRegNumSequencer - get existing seuence details
        ///</summary>
        [TestMethod()]
        public void GetRegNumSequencer_FetchExistingSequenceTest()
        {
            RegNumSequencer actual;
            actual = RegNumSequencer.GetRegNumSequencer(sequenceId);
            Assert.IsNotNull(actual);
            DataTable dtSequence = Helpers.Helpers.GetSequenceDetails(sequenceId);
            Assert.AreEqual<int>(actual.ID, Convert.ToInt32(dtSequence.Rows[0][0]));
        }

        /// <summary>
        ///A test for TranslateSuffixSetting
        ///</summary>
        [TestMethod()]
        public void TranslateSuffixSetting_CodeTest()
        {
            RegNumSequencer target = new RegNumSequencer(); // TODO: Initialize to an appropriate value
            RegNumSequencer.FragmentSuffixGenerator generator = RegNumSequencer.FragmentSuffixGenerator.Code; // TODO: Initialize to an appropriate value
            string expected = "code"; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.TranslateSuffixSetting(generator);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TranslateSuffixSetting
        ///</summary>
        [TestMethod()]
        public void TranslateSuffixSetting_noneTest()
        {
            RegNumSequencer target = new RegNumSequencer(); // TODO: Initialize to an appropriate value
            RegNumSequencer.FragmentSuffixGenerator generator = RegNumSequencer.FragmentSuffixGenerator.None; // TODO: Initialize to an appropriate value
            string expected = ""; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.TranslateSuffixSetting(generator);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TranslateSuffixSetting
        ///</summary>
        [TestMethod()]
        public void TranslateSuffixSetting_descTest()
        {
            RegNumSequencer target = new RegNumSequencer(); // TODO: Initialize to an appropriate value
            RegNumSequencer.FragmentSuffixGenerator generator = RegNumSequencer.FragmentSuffixGenerator.Description; // TODO: Initialize to an appropriate value
            string expected = "desc"; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.TranslateSuffixSetting(generator);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TranslateSuffixSetting
        ///</summary>
        [TestMethod()]
        public void TranslateSuffixSetting_formulaTest()
        {
            RegNumSequencer target = new RegNumSequencer(); // TODO: Initialize to an appropriate value
            RegNumSequencer.FragmentSuffixGenerator generator = RegNumSequencer.FragmentSuffixGenerator.Formula; // TODO: Initialize to an appropriate value
            string expected = "formula"; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.TranslateSuffixSetting(generator);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TranslateSuffixSetting
        ///</summary>
        [TestMethod()]
        public void TranslateSuffixSetting_IdTest()
        {
            RegNumSequencer target = new RegNumSequencer(); // TODO: Initialize to an appropriate value
            RegNumSequencer.FragmentSuffixGenerator generator = RegNumSequencer.FragmentSuffixGenerator.InternalId; // TODO: Initialize to an appropriate value
            string expected = "id"; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.TranslateSuffixSetting(generator);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BatchPadding
        ///</summary>
        [TestMethod()]
        public void BatchPaddingTest()
        {
            RegNumSequencer target = new RegNumSequencer(); // TODO: Initialize to an appropriate value
            RegNumSequencer.BatchNumberPadding expected = RegNumSequencer.BatchNumberPadding.TwoDigits; // TODO: Initialize to an appropriate value
            RegNumSequencer.BatchNumberPadding actual;
            target.BatchPadding = expected;
            actual = target.BatchPadding;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Example
        ///</summary>
        [TestMethod()]
        public void ExampleTest()
        {
            RegNumSequencer target = RegNumSequencer.GetRegNumSequencer(sequenceId); // TODO: Initialize to an appropriate value
            string expected = "TEST";
            string actual;
            actual = target.Example;
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        ///A test for IsActive
        ///</summary>
        [TestMethod()]
        public void IsActiveTest()
        {
            RegNumSequencer target = RegNumSequencer.GetRegNumSequencer(sequenceId); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsActive;
            Assert.AreEqual<bool>(expected, actual);
        }

        /// <summary>
        ///A test for Name
        ///</summary>
        [TestMethod()]
        public void NumberSequencerNameTest()
        {
            RegNumSequencer target = RegNumSequencer.GetRegNumSequencer(sequenceId); // TODO: Initialize to an appropriate value
            string expected = "TEST"; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.NumberSequencer.Name;
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        ///A test for Name
        ///</summary>
        [TestMethod()]
        public void NameTest()
        {
            RegNumSequencer target = RegNumSequencer.GetRegNumSequencer(sequenceId); // TODO: Initialize to an appropriate value
            string expected = null; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.NumberSequencer.Name;
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        ///A test for Name
        ///</summary>
        [TestMethod()]
        public void RegPaddingTest()
        {
            RegNumSequencer target = RegNumSequencer.GetRegNumSequencer(sequenceId); // TODO: Initialize to an appropriate value
            NumericSequence.RegNumberPadding expected =  NumericSequence.RegNumberPadding.SixDigits ; // TODO: Initialize to an appropriate value
            NumericSequence.RegNumberPadding  actual;
            actual = target.NumberSequencer.RegPadding;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for NumberSequencer
        ///</summary>
        [TestMethod()]
        public void NumberSequencerTest()
        {
            RegNumSequencer target = RegNumSequencer.GetRegNumSequencer(sequenceId); // TODO: Initialize to an appropriate value
            NumericSequence actual;
            actual = target.NumberSequencer;
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for Prefix
        ///</summary>
        [TestMethod()]
        public void PrefixTest()
        {
            RegNumSequencer target = RegNumSequencer.GetRegNumSequencer(sequenceId); // TODO: Initialize to an appropriate value
            string expected = "TEST"; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Prefix;
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        ///A test for PrefixSeparator
        ///</summary>
        [TestMethod()]
        public void PrefixSeparatorTest()
        {
            RegNumSequencer target = RegNumSequencer.GetRegNumSequencer(sequenceId); // TODO: Initialize to an appropriate value
            string expected = null; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.PrefixSeparator;
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        ///A test for SuffixGenerator
        ///</summary>
        [TestMethod()]
        public void SuffixGeneratorTest()
        {
            RegNumSequencer target = RegNumSequencer.GetRegNumSequencer(sequenceId); // TODO: Initialize to an appropriate value
            string expected = null; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.SuffixGenerator;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SuffixSeparator
        ///</summary>
        [TestMethod()]
        public void SuffixSeparatorTest()
        {
            RegNumSequencer target = RegNumSequencer.GetRegNumSequencer(sequenceId); // TODO: Initialize to an appropriate value
            string expected = null; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.SuffixSeparator;
            Assert.AreEqual(expected, actual);
        }
    }
}
