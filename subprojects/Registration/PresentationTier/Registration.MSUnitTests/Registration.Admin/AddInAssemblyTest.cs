using CambridgeSoft.COE.RegistrationAdmin.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace RegistrationAdmin.Services.MSUnitTests
{
    /// <summary>
    ///This is a test class for AddInAssemblyTest and is intended
    ///to contain all AddInAssemblyTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AddInAssemblyTest
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


        /// <summary>
        ///A test for AddInAssembly Constructor
        ///</summary>
        [TestMethod()]
        public void AddInAssemblyConstructor_DefaultConstructorTest()
        {
            AddInAssembly target = new AddInAssembly();
            Assert.IsNotNull(target, string.Format("AddInAssembly() Failed to create an object"));
        }

        /// <summary>
        ///A test for AddInAssembly Constructor
        ///</summary>
        [TestMethod()]
        public void AddInAssemblyConstructorTest1()
        {
            Assembly assembly = Assembly.Load(new AssemblyName("CambridgeSoft.COE.Registration.RegistrationAddins")); // TODO: Initialize to an appropriate value
            AddInAssembly target = new AddInAssembly(assembly);
            Assert.IsNotNull(target, string.Format("AddInAssembly() Failed to create an object"));
        }

        /// <summary>
        ///A test for GetClassByName
        ///</summary>
        [TestMethod()]
        public void GetClassByNameTest()
        {
            Assembly assembly = Assembly.Load(new AssemblyName("CambridgeSoft.COE.Registration.RegistrationAddins")); // TODO: Initialize to an appropriate value
            AddInAssembly target = new AddInAssembly(assembly); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("AddInAssembly() Failed to create an object"));
            string namespaceName = "CambridgeSoft.COE.Registration.Services.RegistrationAddins";
            string className = "NormalizedStructureAddIn"; // TODO: Initialize to an appropriate value
            string fullClassName = namespaceName + "." + className;
            AddInClass actual;
            actual = target.GetClassByName(fullClassName);
            Assert.IsNotNull(target, string.Format("GetClassByName(className) Failed to return expected value"));
            Assert.AreEqual<string>(namespaceName,  actual.NameSpace, string.Format("GetClassByName(className) did not return expected namespace"));
            Assert.AreEqual<string>(className, actual.Name, string.Format("GetClassByName(className) did not return expected namespace"));
        }

        /// <summary>
        ///A test for ClassList
        ///</summary>
        [TestMethod()]
        public void ClassListTest()
        {
            Assembly assembly = Assembly.Load(new AssemblyName("CambridgeSoft.COE.Registration.RegistrationAddins")); // TODO: Initialize to an appropriate value
            AddInAssembly target = new AddInAssembly(assembly); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("AddInAssembly() Failed to create an object"));
            List<AddInClass> actual;
            actual = target.ClassList;
            Assert.IsTrue(actual.Count > 0, string.Format("ClassList Failed to return expected value"));
        }

        /// <summary>
        ///A test for FullName
        ///</summary>
        [TestMethod()]
        public void FullNameTest()
        {
            Assembly assembly = Assembly.Load(new AssemblyName("CambridgeSoft.COE.Registration.RegistrationAddins")); // TODO: Initialize to an appropriate value
            AddInAssembly target = new AddInAssembly(assembly); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("AddInAssembly() Failed to create an object"));
            string expected = "CambridgeSoft.COE.Registration.RegistrationAddins, Version=12.1.0.0, Culture=neutral, PublicKeyToken=f435ba95da9797dc";
            string actual;
            actual = target.FullName;
            Assert.AreEqual<string>(expected, actual, string.Format("FullName did not return expected full name"));
        }

        /// <summary>
        ///A test for Name
        ///</summary>
        [TestMethod()]
        public void NameTest()
        {
            Assembly assembly = Assembly.Load(new AssemblyName("CambridgeSoft.COE.Registration.RegistrationAddins")); // TODO: Initialize to an appropriate value
            AddInAssembly target = new AddInAssembly(assembly); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("AddInAssembly() Failed to create an object"));
            string expected = "CambridgeSoft.COE.Registration.RegistrationAddins";
            string actual;
            actual = target.Name;
            Assert.AreEqual<string>(expected, actual, string.Format("FullName did not return expected name"));
        }

        /// <summary>
        ///A test for Path
        ///</summary>
        ///<remarks>
        ///Test always fails because the Path property is not set in the constructor
        ///</remarks>
        [TestMethod()]
        public void PathTest()
        {
            Assembly assembly = Assembly.Load(new AssemblyName("CambridgeSoft.COE.Registration.RegistrationAddins")); // TODO: Initialize to an appropriate value
            AddInAssembly target = new AddInAssembly(assembly); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("AddInAssembly() Failed to create an object"));
            string expected = assembly.CodeBase;
            string actual;
            actual = target.Path;
            Assert.AreEqual<string>(expected, actual, string.Format("Path did not return expected path"));
        }

        /// <summary>
        ///A test for PublicToken
        ///</summary>
        [TestMethod()]
        public void PublicTokenTest()
        {
            Assembly assembly = Assembly.Load(new AssemblyName("CambridgeSoft.COE.Registration.RegistrationAddins")); // TODO: Initialize to an appropriate value
            AddInAssembly target = new AddInAssembly(assembly); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target, string.Format("AddInAssembly() Failed to create an object"));
            byte[] bArray = assembly.GetName().GetPublicKey();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bArray)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            string expected = sb.ToString();
            string actual;
            actual = target.PublicToken;
            Assert.AreEqual<string>(expected, actual, string.Format("PublicToken did not return expected public key token"));
        }
    }
}
