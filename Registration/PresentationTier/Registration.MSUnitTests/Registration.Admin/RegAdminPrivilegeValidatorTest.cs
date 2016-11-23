using CambridgeSoft.COE.RegistrationAdmin.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RegistrationAdmin.Services.MSUnitTests
{
    /// <summary>
    ///This is a test class for RegAdminPrivilegeValidatorTest and is intended
    ///to contain all RegAdminPrivilegeValidatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RegAdminPrivilegeValidatorTest
    {
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
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod]
        public void ConstructorTest()
        {
            RegAdminPrivilegeValidator actual = new RegAdminPrivilegeValidator();
            Assert.IsNotNull(actual, "failed to create object");
        }

        /// <summary>
        ///A test for HasPrivilege
        ///</summary>
        [TestMethod()]
        public void HasPrivilege_CUSTOMIZE_FORMSTest()
        {
            Helpers.Helpers.Logon();
            RegAdminPrivilegeValidator.RegAdminTasks privilegeName = RegAdminPrivilegeValidator.RegAdminTasks.CUSTOMIZE_FORMS; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = RegAdminPrivilegeValidator.HasPrivilege(privilegeName);
            Assert.AreEqual(expected, actual, "CUSTOMIZE_FORMS privilege not available to user");
            Helpers.Helpers.Logoff();
        }

        /// <summary>
        ///A test for HasPrivilege
        ///</summary>
        [TestMethod()]
        public void HasPrivilege_EDIT_FORM_XMLTest()
        {
            Helpers.Helpers.Logon();
            RegAdminPrivilegeValidator.RegAdminTasks privilegeName = RegAdminPrivilegeValidator.RegAdminTasks.EDIT_FORM_XML; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = RegAdminPrivilegeValidator.HasPrivilege(privilegeName);
            Assert.AreEqual(expected, actual, "EDIT_FORM_XML privilege not available to user");
            Helpers.Helpers.Logoff();
        }

        /// <summary>
        ///A test for HasPrivilege
        ///</summary>
        [TestMethod()]
        public void HasPrivilege_IMPORT_CONFIGTest()
        {
            Helpers.Helpers.Logon();
            RegAdminPrivilegeValidator.RegAdminTasks privilegeName = RegAdminPrivilegeValidator.RegAdminTasks.IMPORT_CONFIG; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = RegAdminPrivilegeValidator.HasPrivilege(privilegeName);
            Assert.AreEqual(expected, actual, "IMPORT_CONFIG privilege not available to user");
            Helpers.Helpers.Logoff();
        }

        /// <summary>
        ///A test for HasPrivilege
        ///</summary>
        [TestMethod()]
        public void HasPrivilege_MANAGE_ADDINSTest()
        {
            Helpers.Helpers.Logon();
            RegAdminPrivilegeValidator.RegAdminTasks privilegeName = RegAdminPrivilegeValidator.RegAdminTasks.MANAGE_ADDINS; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = RegAdminPrivilegeValidator.HasPrivilege(privilegeName);
            Assert.AreEqual(expected, actual, "MANAGE_ADDINS privilege not available to user");
            Helpers.Helpers.Logoff();
        }

        /// <summary>
        ///A test for HasPrivilege
        ///</summary>
        [TestMethod()]
        public void HasPrivilege_MANAGE_PROPERTIESTest()
        {
            Helpers.Helpers.Logon();
            RegAdminPrivilegeValidator.RegAdminTasks privilegeName = RegAdminPrivilegeValidator.RegAdminTasks.MANAGE_PROPERTIES; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = RegAdminPrivilegeValidator.HasPrivilege(privilegeName);
            Assert.AreEqual(expected, actual, "MANAGE_PROPERTIES privilege not available to user");
            Helpers.Helpers.Logoff();
        }

        /// <summary>
        ///A test for HasPrivilege
        ///</summary>
        [TestMethod()]
        public void HasPrivilege_MANAGE_SYSTEM_SETTINGSTest()
        {
            Helpers.Helpers.Logon();
            RegAdminPrivilegeValidator.RegAdminTasks privilegeName = RegAdminPrivilegeValidator.RegAdminTasks.MANAGE_SYSTEM_SETTINGS; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = RegAdminPrivilegeValidator.HasPrivilege(privilegeName);
            Assert.AreEqual(expected, actual, "MANAGE_SYSTEM_SETTINGS privilege not available to user");
            Helpers.Helpers.Logoff();
        }

        /// <summary>
        ///A test for HasPrivilege
        ///</summary>
        [TestMethod()]
        public void HasPrivilege_MANAGE_TABLESTest()
        {
            Helpers.Helpers.Logon();
            RegAdminPrivilegeValidator.RegAdminTasks privilegeName = RegAdminPrivilegeValidator.RegAdminTasks.MANAGE_TABLES; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = RegAdminPrivilegeValidator.HasPrivilege(privilegeName);
            Assert.AreEqual(expected, actual, "MANAGE_TABLES privilege not available to user");
            Helpers.Helpers.Logoff();
        }
    }
}
