using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CambridgeSoft.COE.Framework.UnitTests.Helpers;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using System.Xml;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.UnitTests.Security
{
    /// <summary>
    /// Summary description for COEAccessRightsBOTest
    /// </summary>
    [TestClass]
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext) 
        {
            Authentication.Logon();
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

        [TestMethod]
        public void New()
        {
            COEAccessRightsBO theCOEAccessRightsBO = COEAccessRightsBO.New();
            Assert.IsNotNull(theCOEAccessRightsBO);
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void COEAccessRightsBO_GetRolesListProperty()
        {
            COEUserReadOnlyBOList theCOEUserReadOnlyBOList = COEUserReadOnlyBOList.GetList();
            COERoleReadOnlyBOList theCOERoleReadOnlyBOList = COERoleReadOnlyBOList.GetList();

            COEAccessRightsBO theCOEAccessRightsBO = new COEAccessRightsBO(theCOEUserReadOnlyBOList, theCOERoleReadOnlyBOList);
            Assert.AreEqual(theCOERoleReadOnlyBOList, theCOEAccessRightsBO.Roles);        
        }

        [TestMethod]
        public void GetIdValue()
        {
            if (SecurityHelper.GetObjectId(objectType.ToString()))
            {
                COEAccessRightsBO theCOEAccessRightsBO = new COEAccessRightsBO(objectType, SecurityHelper.intObjectTypeId);

                PrivateObject thePrivateObject = new PrivateObject(theCOEAccessRightsBO);
                object theId = thePrivateObject.Invoke("GetIdValue");

                Assert.AreEqual(theCOEAccessRightsBO.ObjectID, Convert.ToInt32(theId));
            }
            else
                Assert.Fail(objectType.ToString() + " does not exist");
        }

        [TestMethod]
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

        [TestMethod]
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



        [TestMethod]
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
