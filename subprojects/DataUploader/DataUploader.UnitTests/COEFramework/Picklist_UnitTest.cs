using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Access;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.COEFramework
{
    [TestFixture]
    public class Picklist_UnitTest
    {
        /// <summary>
        /// Test mechanism of creating a new Picklist instance from a xml snippet(from the PicklistXmlRepresentation.xml file).
        /// </summary>
        [Test]
        public void CreatePicklistFromXml()
        {
            string xmlFilePath = Path.Combine(UnitUtils.GetTestFolderPath(), "PicklistXmlRepresentation.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);
            Picklist picklist = Picklist.NewPicklist(doc.OuterXml);
            Assert.True(picklist.PickList.ContainsKey(3));
            Assert.True(picklist.PickList.ContainsValue("mg"));
        }

        /// <summary>
        /// Test mechanism of trying to create a new Picklist instance from a invalid xml snippet(from the PicklistXmlRepresentation_Invalid.xml file) which,
        /// if run correctly, will throw an COEBusinessLayerException.
        /// </summary>
        [Test]
        [ExpectedException(typeof(CambridgeSoft.COE.Framework.ExceptionHandling.COEBusinessLayerException))]
        public void CreatePicklistFromInvalidXml()
        {
            string xmlFilePath = Path.Combine(UnitUtils.GetTestFolderPath(), "PicklistXmlRepresentation_Invalid.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);
            Picklist picklist = Picklist.NewPicklist(doc.OuterXml);
        }

        /// <summary>
        /// Given the picklist created from a xml fragment and the value of one picklist item,get the id of that item.
        /// </summary>
        [Test]
        public void GetListItemIdByValue()
        {
            string xmlFilePath = Path.Combine(UnitUtils.GetTestFolderPath(), "PicklistXmlRepresentation.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);
            Picklist picklist = Picklist.NewPicklist(doc.OuterXml);
            int id = picklist.GetListItemIdByValue("mg");
            Assert.AreEqual(3, id);
        }

        [Test]
        public void CanGetPicklistByValidNumber()
        {
            Picklist picklist = DalUtils.GetPicklistByCode("2");
            Assert.IsNotNull(picklist, "Unable to retrieve PickList 2");
        }

        [Test]
        [ExpectedException(typeof(System.ArgumentException))]
        public void GetPicklistByInvalidNumber()
        {
            Picklist picklist = DalUtils.GetPicklistByCode("6");
        }

        [Test]
        public void CanGetPicklistByDescriptionWithRightCase()
        {

            Picklist picklist = DalUtils.GetPicklistByCode("Units");
            Assert.IsNotNull(picklist, "Unable to retrieve PickList Units");
        }

        [Test]
        public void CanGetPicklistByDescriptionWithWrongCase()
        {
            Picklist picklist = DalUtils.GetPicklistByCode("unitS");
            Assert.IsNotNull(picklist, "Unable to retrieve PickList unitS");
        }

        [Test]
        [ExpectedException(typeof(System.ArgumentException))]
        public void GetPicklistByInvalidDescription()
        {
            Picklist picklist = DalUtils.GetPicklistByCode("Invalid_Description");
        }
    }
}
