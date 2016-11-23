using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CambridgeSoft.COE.Framework.COEFormService;


namespace CambridgeSoft.COE.Framework.COEFormService.UnitTests
{
    /// <summary>
    /// Summary description for COEFormUtilitiesTest
    /// </summary>
    [TestFixture]
    public class COEFormUtilitiesTest
    {
        private TestContext testContextInstance;

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

        /// <summary>
        /// For testing COEFormUtilities.BuildCOEFormTableName method by passing empty owner name
        /// This method internally called by DAL with proper owner name, 
        /// hence only tested with empty owner name here
        /// </summary>
        [Test]
        public void BuildCOEFormTableName_NullOwnerTest()
        {
            string strTableName = COEFormGroupUtilities.BuildCOEFormTableName(string.Empty);
            Assert.IsTrue(!string.IsNullOrEmpty(strTableName), "COEFormGroupUtilities.BuildCOEFormTableName did not return the expected value.");
        }

        /// <summary>
        /// For testing COEFormUtilities.BuildCOEFormTypeTableName method by passing empty owner name
        /// This method internally called by DAL with proper owner name, 
        /// hence only tested with empty owner name here
        /// </summary>
        [Test]
        public void BuildCOEFormTypeTableName_NullOwnerTest()
        {
            string strTableName = COEFormGroupUtilities.BuildCOEFormTypeTableName(string.Empty);
            Assert.IsTrue(!string.IsNullOrEmpty(strTableName), "COEFormGroupUtilities.BuildCOEFormTypeTableName did not return the expected value.");
        }
    }
}
