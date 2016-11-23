using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.COE.Registration.Services.UnitTests
{
    /// <summary>
    /// Summary description for ValidationRuleTest
    /// </summary>
    [TestClass]
    public class ValidationRuleListTest
    {
        public ValidationRuleListTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        ///A test for NewValidationRuleList ()
        ///</summary>
        [DeploymentItem("CambridgeSoft.COE.Registration.Services.dll")]
        [TestMethod()]
        public void NewValidationRuleListTest()
        {
            ValidationRuleList target = CambridgeSoft.COE.Registration.Services.Types.ValidationRuleList.NewValidationRuleList("BP");

        }
        

    }
}
