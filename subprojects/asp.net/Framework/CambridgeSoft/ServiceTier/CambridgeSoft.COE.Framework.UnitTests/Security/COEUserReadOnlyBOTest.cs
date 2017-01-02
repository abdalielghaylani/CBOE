using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Reflection;
using System.Data;

namespace CambridgeSoft.COE.Framework.UnitTests.Security
{
    /// <summary>
    /// Summary description for COEUserReadOnlyBOTest
    /// </summary>
    [TestClass]
    public class COEUserReadOnlyBOTest
    {

        #region Variables   

        static bool blnPeopleDetails = false;

        #endregion

        public COEUserReadOnlyBOTest()
        {            
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            Authentication.Logon();
            blnPeopleDetails = SecurityHelper.GetPeopleDetails();
        }
        
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup() 
        {
            Authentication.Logoff();
        }
        
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize() { }
        
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup() { }                  

        #endregion

        #region Test Methods

        [TestMethod]       
        public void COEUserReadOnlyBO_GetPersonIDProperty()
        {        
            Type type = typeof(COEUserReadOnlyBO);            

            if (blnPeopleDetails)
            {
                COEUserReadOnlyBO theCOEUserReadOnlyBO = (COEUserReadOnlyBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, SecurityHelper.argList, null, null);
                int intPersonId = theCOEUserReadOnlyBO.PersonID;
                Assert.AreEqual(SecurityHelper.personID, intPersonId);
            }
            else
                Assert.Fail("User does not exist");
        }

        [TestMethod]
        public void COEUserReadOnlyBO_GetUserCodeProperty()
        {
            Type type = typeof(COEUserReadOnlyBO);            

            if (blnPeopleDetails)
            {
                COEUserReadOnlyBO theCOEUserReadOnlyBO = (COEUserReadOnlyBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, SecurityHelper.argList, null, null);
                string strUserCode = theCOEUserReadOnlyBO.UserCode;
                Assert.AreEqual(SecurityHelper.userCode, strUserCode);
            }
            else
                Assert.Fail("User does not exist");
        }

        [TestMethod]
        public void COEUserReadOnlyBO_GetUserIdProperty()
        {
            Type type = typeof(COEUserReadOnlyBO);
         
            if (blnPeopleDetails)
            {
                COEUserReadOnlyBO theCOEUserReadOnlyBO = (COEUserReadOnlyBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, SecurityHelper.argList, null, null);
                string strUserId = theCOEUserReadOnlyBO.UserID;
                Assert.AreEqual(SecurityHelper.userID, strUserId);
            }
            else
                Assert.Fail("User does not exist");
        }

        [TestMethod]
        public void COEUserReadOnlyBO_GetSupervisorIdProperty()
        {
            Type type = typeof(COEUserReadOnlyBO);           

            if (blnPeopleDetails)
            {
                COEUserReadOnlyBO theCOEUserReadOnlyBO = (COEUserReadOnlyBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, SecurityHelper.argList, null, null);
                int intSupervisorId = theCOEUserReadOnlyBO.SupervisorID;
                Assert.AreEqual(SecurityHelper.supervisorID, intSupervisorId);
            }
            else
                Assert.Fail("User does not exist");
        }

        [TestMethod]
        public void COEUserReadOnlyBO_GetTitleProperty()
        {
            Type type = typeof(COEUserReadOnlyBO);           

            if (blnPeopleDetails)
            {
                COEUserReadOnlyBO theCOEUserReadOnlyBO = (COEUserReadOnlyBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, SecurityHelper.argList, null, null);
                string strTittle = theCOEUserReadOnlyBO.Title;
                Assert.AreEqual(SecurityHelper.title, strTittle);
            }
            else
                Assert.Fail("User does not exist");
        }

        [TestMethod]
        public void COEUserReadOnlyBO_GetFirstNameProperty()
        {
            Type type = typeof(COEUserReadOnlyBO);            

            if (blnPeopleDetails)
            {
                COEUserReadOnlyBO theCOEUserReadOnlyBO = (COEUserReadOnlyBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, SecurityHelper.argList, null, null);
                string strFirstName = theCOEUserReadOnlyBO.FirstName;
                Assert.AreEqual(SecurityHelper.firstName, strFirstName);
            }
            else
                Assert.Fail("User does not exist");
        }

        [TestMethod]
        public void COEUserReadOnlyBO_GetMiddleNameProperty()
        {
            Type type = typeof(COEUserReadOnlyBO);           

            if (blnPeopleDetails)
            {
                COEUserReadOnlyBO theCOEUserReadOnlyBO = (COEUserReadOnlyBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, SecurityHelper.argList, null, null);
                string strMiddleName = theCOEUserReadOnlyBO.MiddleName;
                Assert.AreEqual(SecurityHelper.middleName, strMiddleName);
            }
            else
                Assert.Fail("User does not exist");
        }

        [TestMethod]
        public void COEUserReadOnlyBO_GetLastNameProperty()
        {
            Type type = typeof(COEUserReadOnlyBO);            

            if (blnPeopleDetails)
            {
                COEUserReadOnlyBO theCOEUserReadOnlyBO = (COEUserReadOnlyBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, SecurityHelper.argList, null, null);
                string strLastName = theCOEUserReadOnlyBO.LastName;
                Assert.AreEqual(SecurityHelper.lastName, strLastName);
            }
            else
                Assert.Fail("User does not exist");
        }

        [TestMethod]
        public void COEUserReadOnlyBO_GetDepartmentProperty()
        {
            Type type = typeof(COEUserReadOnlyBO);           

            if (blnPeopleDetails)
            {
                COEUserReadOnlyBO theCOEUserReadOnlyBO = (COEUserReadOnlyBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, SecurityHelper.argList, null, null);
                string strDept = theCOEUserReadOnlyBO.Department;
                Assert.AreEqual(SecurityHelper.department, strDept);
            }
            else
                Assert.Fail("User does not exist");
        }

        [TestMethod]
        public void COEUserReadOnlyBO_GetTelephoneProperty()
        {
            Type type = typeof(COEUserReadOnlyBO);            

            if (blnPeopleDetails)
            {
                COEUserReadOnlyBO theCOEUserReadOnlyBO = (COEUserReadOnlyBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, SecurityHelper.argList, null, null);
                string strTelephone = theCOEUserReadOnlyBO.Telephone;
                Assert.AreEqual(SecurityHelper.telephone, strTelephone);
            }
            else
                Assert.Fail("User does not exist");
        }

        [TestMethod]
        public void COEUserReadOnlyBO_GetEmailProperty()
        {
           Type type = typeof(COEUserReadOnlyBO);          

           if (blnPeopleDetails)
           {
               COEUserReadOnlyBO theCOEUserReadOnlyBO = (COEUserReadOnlyBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, SecurityHelper.argList, null, null);
               string strEmail = theCOEUserReadOnlyBO.Email;
               Assert.AreEqual(SecurityHelper.email, strEmail);
           }
           else
               Assert.Fail("User does not exist");
        }

        [TestMethod]
        public void COEUserReadOnlyBO_GetActiveProperty()
        {
            Type type = typeof(COEUserReadOnlyBO);         

            if (blnPeopleDetails)
            {
                COEUserReadOnlyBO theCOEUserReadOnlyBO = (COEUserReadOnlyBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, SecurityHelper.argList, null, null);
                bool blnActive = theCOEUserReadOnlyBO.Active;
                Assert.AreEqual(SecurityHelper.active, blnActive);
            }
            else
                Assert.Fail("User does not exist");
        }       

        [TestMethod]
        public void GetIdValue_PersonId()
        {
            Type type = typeof(COEUserReadOnlyBO);         

            if (blnPeopleDetails)
            {
                COEUserReadOnlyBO theCOEUserReadOnlyBO = (COEUserReadOnlyBO)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, SecurityHelper.argList, null, null);
                PrivateObject thePrivateObject = new PrivateObject(theCOEUserReadOnlyBO);
                object theResult = thePrivateObject.Invoke("GetIdValue", null);
                Assert.AreEqual(SecurityHelper.personID, Convert.ToInt32(theResult));
            }
            else
                Assert.Fail("User does not exist");
        }

        [TestMethod]
        public void Get_RoleBusinessObject()
        {
            if (blnPeopleDetails)
            {
                COEUserReadOnlyBO theCOEUserReadOnlyBO = COEUserReadOnlyBO.Get(SecurityHelper.userID);
                // If any field related to T5_85 is null in PERSON table then it will throw exception in FetchObject method.
                Assert.Fail();
            }
            else
                Assert.Fail("User does not exist");
        }

        #endregion Test Methods
    }
}
