using CambridgeSoft.COE.RegistrationAdmin.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using CambridgeSoft.COE.Registration.Services.AddIns;
using System.Diagnostics;

namespace RegistrationAdmin.Services.MSUnitTests
{
    /// <summary>
    ///This is a test class for AddInClassTest and is intended
    ///to contain all AddInClassTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AddInClassTest
    {
        Type addInClassType;

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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            //Assert.Fail(Assembly.GetExecutingAssembly().CodeBase);
            Assembly assembly = Assembly.Load(new AssemblyName("CambridgeSoft.COE.Registration.RegistrationAddins")); // TODO: Initialize to an appropriate value
            AddInAssembly addinAssembly = new AddInAssembly(assembly);
            Assert.IsNotNull(addinAssembly, string.Format("AddInAssembly() Failed to create an object"));

            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetInterface(typeof(IAddIn).FullName) != null)
                {
                    addInClassType = type.GetInterface(typeof(IAddIn).FullName);
                    Assert.IsNotNull(addInClassType, string.Format("addInClassType failed to return expected rsult"));
                    break;
                }
            }
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {

        }
        
        #endregion

        /// <summary>
        ///A test for AddInClass Constructor
        ///</summary>
        [TestMethod()]
        public void AddInClassConstructor_NameAndEventHandlersTest()
        {
            string name = addInClassType.Name; // TODO: Initialize to an appropriate value
            List<string> eventHandlers = new List<string>(); // TODO: Initialize to an appropriate value
            MethodInfo[] methods = addInClassType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            foreach (MethodInfo method in methods)
            {
                eventHandlers.Add(method.Name);
            }
            AddInClass target = new AddInClass(name, eventHandlers);
            Assert.IsNotNull(target, string.Format("AddInClass(name, eventHandlers) failed to return the object"));
        }

        /// <summary>
        ///A test for AddInClass Constructor
        ///</summary>
        [TestMethod()]
        public void AddInClassConstructor_TypeConstructorTest()
        {
            Type type = addInClassType; // TODO: Initialize to an appropriate value
            AddInClass target = new AddInClass(type);
            Assert.IsNotNull(target, string.Format("AddInClass(type) failed to return the object"));
        }

        /// <summary>
        ///A test for AddInClass Constructor
        ///</summary>
        [TestMethod()]
        public void AddInClassConstructor_DefaultCOnstructorTest()
        {
            AddInClass target = new AddInClass();
            Assert.IsNotNull(target, string.Format("AddInClass() failed to return the object"));
        }

        /// <summary>
        ///A test for EventHandlerList
        ///</summary>
        [TestMethod()]
        public void EventHandlerListTest()
        {
            Type type = addInClassType; // TODO: Initialize to an appropriate value
            AddInClass target = new AddInClass(type);
            Assert.IsNotNull(target, string.Format("AddInClass(type) failed to return the object"));
            List<string> actual;
            actual = target.EventHandlerList;

            MethodInfo[] methods = addInClassType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            
            Assert.AreEqual<int>(methods.Length, actual.Count, string.Format("EventHandlerList failed to return expected value"));
        }

        /// <summary>
        ///A test for Name
        ///</summary>
        [TestMethod()]
        public void NameTest()
        {
            Type type = addInClassType; // TODO: Initialize to an appropriate value
            AddInClass target = new AddInClass(type);
            Assert.IsNotNull(target, string.Format("AddInClass(type) failed to return the object"));
            string expected = "IAddIn";
            string actual;
            actual = target.Name;
            Assert.AreEqual<string>(expected, actual, string.Format("Name failed to return expected value"));
        }

        /// <summary>
        ///A test for NameSpace
        ///</summary>
        [TestMethod()]
        public void NameSpaceTest()
        {
            Type type = addInClassType; // TODO: Initialize to an appropriate value
            AddInClass target = new AddInClass(type);
            Assert.IsNotNull(target, string.Format("AddInClass(type) failed to return the object"));
            string expected = "CambridgeSoft.COE.Registration.Services.AddIns";
            string actual;
            actual = target.NameSpace;
            Assert.AreEqual<string>(expected, actual, string.Format("NameSpace failed to return expected value"));
        }
    }
}
