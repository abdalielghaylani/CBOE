using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CambridgeSoft.COE.Framework.NUnitTests.Helpers;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using System.Xml;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.NUnitTests.Security
{
    /// <summary>
    /// Summary description for COEAccessRightsBOTest
    /// </summary>
    [TestFixture]
    public class COEAccessRightsBOTest
    {

        private COEAccessRightsBO.ObjectTypes objectType = COEAccessRightsBO.ObjectTypes.COEDATAVIEW;
        
        public COEAccessRightsBOTest()
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
        [TestFixtureSetUp]
        public static void MyClassInitialize() 
        {
            Authentication.Logon();
        }
        
        // Use ClassCleanup to run code after all tests in a class have run
        [TestFixtureTearDown]
        public static void MyClassCleanup() 
        {
            Authentication.Logoff(); 
        }
        
        // Use TestInitialize to run code before running each test 
        [SetUp]
        public void MyTestInitialize() { }
        
        // Use TestCleanup to run code after each test has run
        [TearDown]
        public void MyTestCleanup() { }
        
        #endregion

        #region Test Methods

        [Test]
        public void Get()
        {           
           if (SecurityHelper.GetObjectId(objectType.ToString()))
            {
                COEAccessRightsBO theCOEAccessRightsBO = COEAccessRightsBO.Get(objectType, SecurityHelper.intObjectTypeId);
                Assert.IsNotNull(theCOEAccessRightsBO);
            }
            else
                Assert.Fail(objectType.ToString() + " does not exist");
         }

        [Test]
        public void New()
        {
            COEAccessRightsBO theCOEAccessRightsBO = COEAccessRightsBO.New();
            Assert.IsNotNull(theCOEAccessRightsBO);
        }

        [Test]
        public void COEAccessRightsBO_GetObjectIdProperty()
        {
            if (SecurityHelper.GetObjectId(objectType.ToString()))
            {
                COEAccessRightsBO theCOEAccessRightsBO = new COEAccessRightsBO(objectType, SecurityHelper.intObjectTypeId);
                Assert.AreEqual(SecurityHelper.intObjectTypeId, theCOEAccessRightsBO.ObjectID);
            }
            else
                Assert.Fail(objectType.ToString() + " does not exist");
        }

        [Test]
        public void COEAccessRightsBO_GetObjectTypeProperty()
        {
            if (SecurityHelper.GetObjectId(objectType.ToString()))
            {
                COEAccessRightsBO theCOEAccessRightsBO = new COEAccessRightsBO(objectType, SecurityHelper.intObjectTypeId);
                Assert.AreEqual(objectType, theCOEAccessRightsBO.ObjectType);
            }
            else
                Assert.Fail(objectType.ToString() + " does not exist");
        }

        [Test]
        public void COEAccessRightsBO_GetUsersListProperty()
        {
            if (SecurityHelper.GetObjectId(objectType.ToString()))
            {
                COEUserReadOnlyBOList theCOEUserReadOnlyBOList = COEUserReadOnlyBOList.GetList();
                COERoleReadOnlyBOList theCOERoleReadOnlyBOList = COERoleReadOnlyBOList.GetList();

                COEAccessRightsBO theCOEAccessRightsBO = new COEAccessRightsBO(theCOEUserReadOnlyBOList, theCOERoleReadOnlyBOList);
                theCOEAccessRightsBO.ObjectType = objectType;
                theCOEAccessRightsBO.ObjectID = SecurityHelper.intObjectTypeId;
                Assert.AreEqual(theCOEUserReadOnlyBOList, theCOEAccessRightsBO.Users);
            }
            else
                Assert.Fail(objectType.ToString() + " does not exist");
        }

        [Test]
        public void COEAccessRightsBO_GetRolesListProperty()
        {
            COEUserReadOnlyBOList theCOEUserReadOnlyBOList = COEUserReadOnlyBOList.GetList();
            COERoleReadOnlyBOList theCOERoleReadOnlyBOList = COERoleReadOnlyBOList.GetList();

            COEAccessRightsBO theCOEAccessRightsBO = new COEAccessRightsBO(theCOEUserReadOnlyBOList, theCOERoleReadOnlyBOList);
            Assert.AreEqual(theCOERoleReadOnlyBOList, theCOEAccessRightsBO.Roles);        
        }

        [Test]
        public void GetIdValue()
        {
            if (SecurityHelper.GetObjectId(objectType.ToString()))
            {
                COEAccessRightsBO theCOEAccessRightsBO = new COEAccessRightsBO(objectType, SecurityHelper.intObjectTypeId);

                Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject thePrivateObject =new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(theCOEAccessRightsBO);
                object theId = thePrivateObject.Invoke("GetIdValue");

                Assert.AreEqual(theCOEAccessRightsBO.ObjectID, Convert.ToInt32(theId));
            }
            else
                Assert.Fail(objectType.ToString() + " does not exist");
        }

        [Test]
        public void Save()
        {
            if (SecurityHelper.GetObjectId(objectType.ToString()))
            {
                COEAccessRightsBO theCOEAccessRightsBO = new COEAccessRightsBO(objectType, SecurityHelper.intObjectTypeId);
                theCOEAccessRightsBO.Roles = COERoleReadOnlyBOList.GetList();
                theCOEAccessRightsBO.Users = COEUserReadOnlyBOList.GetList();
                COEAccessRightsBO theNewCOEAccessRightsBO = theCOEAccessRightsBO.Save();
                Assert.IsNotNull(theCOEAccessRightsBO);
            }
            else
                Assert.Fail(objectType.ToString() + " does not exist");
        }

        [Test]
        public void Save_ForceUpdate()
        {
            if (SecurityHelper.GetObjectId(objectType.ToString()))
            {
                COEAccessRightsBO theCOEAccessRightsBO = new COEAccessRightsBO(objectType, SecurityHelper.intObjectTypeId);
                theCOEAccessRightsBO.Roles = COERoleReadOnlyBOList.GetList();
                theCOEAccessRightsBO.Users = COEUserReadOnlyBOList.GetList();
                COEAccessRightsBO theNewCOEAccessRightsBO = theCOEAccessRightsBO.Save(true);                
                Assert.IsNotNull(theCOEAccessRightsBO);
            }
            else
                Assert.Fail(objectType.ToString() + " does not exist");
        }



        [Test]
        public void Delete()
        {
            if (SecurityHelper.GetObjectId(objectType.ToString()))
            {          
                COEAccessRightsBO.Delete(objectType, SecurityHelper.intObjectTypeId);               
            }
            else
                Assert.Fail(objectType.ToString() + " does not exist");
        }

        #endregion Test Methods


    }
}
