using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerkinElmer.COE.Inventory.API.Controllers;
using PerkinElmer.COE.Inventory.DAL;
using PerkinElmer.COE.Inventory.DAL.Mapper;
using PerkinElmer.COE.Inventory.Model;

namespace PerkinElmer.COE.Inventory.API.Tests.Controllers
{
    [TestClass]
    public class ContainerControllerTest : BaseControllerTest
    {
        protected ContainerController containerController;

        protected ContainerData containerTest;

        public ContainerControllerTest()
        {
            var INV_LOCATIONS = new INV_LOCATIONS() { LOCATION_ID = 2, LOCATION_NAME = "Disposed", LOCATION_DESCRIPTION = "Disposed Location", LOCATION_BARCODE = "2" };
            Context.INV_LOCATIONS1.Add(INV_LOCATIONS);

            var INV_COMPOUNDS = new INV_COMPOUNDS() { COMPOUND_ID = 447, MOL_ID = 449, CAS = "462-06-6", ACX_ID = "X1003808-9", SUBSTANCE_NAME = "Fluorobenzene", BASE64_CDX = "VmpDRDAxMDAEAwIBAAAAAAAAAAAAAAAAAAAAAAMAOQAAAENEWCBkcml2ZXIgMS42\r\nIFtmb3IgQ2hlbURyYXcgNitdIC8gSmFuIDEyIDE5OjAxOjMwIDIwMDIEAhAA//87\r\nAAAAPABU/m8A2RqWAAEJCAAAAAAAAAAAAAIJCAAAAKsAAADSAAUIBAAAAB4AAAMy\r\nAAgA////////AAAAAAAA//8AAAAAAAAAAP///////wAAAAD//wAAAAD///////8A\r\nAP//AYACAAAABAIQAP//OwAAADwAVP5vANkalgADgAMAAAAEgAQAAAAAAggAMf9V\r\nAAAAPAACBAIACQA5BAMAAAAwBoAFAAAAAAcNAAEAAAADAGAAyAADAEYAAAAABIAG\r\nAAAAAAIIADH/VQDeFFoAOQQDAAAAMQAABIAHAAAAAAIIAAAAPAC6DWkAOQQDAAAA\r\nMgAABIAIAAAAAAIIAFT+bwC6DWkAOQQDAAAAMwAABIAJAAAAAAIIAAAAPAAMIocA\r\nOQQDAAAANAAABIAKAAAAAAIIAFT+bwAMIocAOQQDAAAANQAABIALAAAAAAIIADH/\r\nVQDZGpYAOQQDAAAANgAABYAMAAAABAYEAAQAAAAFBgQABgAAAAAABYANAAAABAYE\r\nAAYAAAAFBgQABwAAAAAGAgACAAAABYAOAAAABAYEAAYAAAAFBgQACAAAAAAABYAP\r\nAAAABAYEAAcAAAAFBgQACQAAAAAABYAQAAAABAYEAAgAAAAFBgQACgAAAAAGAgAC\r\nAAAABYARAAAABAYEAAkAAAAFBgQACwAAAAAGAgACAAAABYASAAAABAYEAAoAAAAF\r\nBgQACwAAAAAAAAAAAAAA\r\n", MOLECULAR_WEIGHT = null, DENSITY = 1 };
            Context.INV_COMPOUNDS.Add(INV_COMPOUNDS);

            var INV_CONTAINER_TYPES = new INV_CONTAINER_TYPES() { CONTAINER_TYPE_ID = 1, CONTAINER_TYPE_NAME = "can" };
            Context.INV_CONTAINER_TYPES.Add(INV_CONTAINER_TYPES);

            var INV_CONTAINER_STATUS = new INV_CONTAINER_STATUS() { CONTAINER_STATUS_ID = 1, CONTAINER_STATUS_NAME = "Available" };
            Context.INV_CONTAINER_STATUS.Add(INV_CONTAINER_STATUS);

            var INV_SUPPLIERS = new INV_SUPPLIERS() { SUPPLIER_ID = 1, SUPPLIER_NAME = "Supplier" };
            Context.INV_SUPPLIERS.Add(INV_SUPPLIERS);

            var INV_UNITS = new INV_UNITS() { UNIT_ID = 1, UNIT_ABREVIATION = "mmol" };
            Context.INV_UNITS.Add(INV_UNITS);

            var INV_CONTAINERS = new INV_CONTAINERS() { CONTAINER_ID = 1648, BARCODE = "1648", CONTAINER_NAME = "fluorobenzene", LOCATION_ID_FK = 2, CURRENT_USER_ID_FK = "INVADMIN", QTY_MAX = 1, QTY_REMAINING = 1, DATE_CREATED = DateTime.Now, COMPOUND_ID_FK = 447, CONTAINER_TYPE_ID_FK = 1, CONTAINER_STATUS_ID_FK = 1, SUPPLIER_ID_FK = 1, UNIT_OF_WGHT_ID_FK = 1 };
            Context.INV_CONTAINERS.Add(INV_CONTAINERS);

            var INV_CUSTOM_FIELD_GROUPS = new INV_CUSTOM_FIELD_GROUPS() { CUSTOM_FIELD_GROUP_ID = 100, CUSTOM_FIELD_GROUP_NAME = "Hazards", CUSTOM_FIELD_GROUP_TYPE_ID_FK = 1 };
            Context.INV_CUSTOM_FIELD_GROUPS.Add(INV_CUSTOM_FIELD_GROUPS);

            var INV_CUSTOM_FIELDS = new INV_CUSTOM_FIELDS() { CUSTOM_FIELD_ID = 100, CUSTOM_FIELD_GROUP_ID_FK = 100, CUSTOM_FIELD_NAME = "C" };
            Context.INV_CUSTOM_FIELDS.Add(INV_CUSTOM_FIELDS);

            var INV_CUSTOM_CPD_FIELD_VALUES = new INV_CUSTOM_CPD_FIELD_VALUES() { CUSTOM_FIELD_ID_FK = 100, COMPOUND_ID_FK = 447 };
            Context.INV_CUSTOM_CPD_FIELD_VALUES.Add(INV_CUSTOM_CPD_FIELD_VALUES);

            containerController = new ContainerController(Context);

            INV_CONTAINERS.INV_LOCATIONS1 = INV_LOCATIONS;
            INV_CONTAINERS.INV_COMPOUNDS = INV_COMPOUNDS;
            INV_CONTAINERS.INV_CONTAINER_TYPES = INV_CONTAINER_TYPES;
            INV_CONTAINERS.INV_CONTAINER_STATUS = INV_CONTAINER_STATUS;
            INV_CONTAINERS.INV_SUPPLIERS = INV_SUPPLIERS;
            INV_CONTAINERS.INV_UNITS = INV_UNITS;

            containerTest = new ContainerMapper().Map(INV_CONTAINERS);

            if (containerTest != null && containerTest.Compound != null)
            {
                INV_CUSTOM_FIELDS.INV_CUSTOM_FIELD_GROUPS = INV_CUSTOM_FIELD_GROUPS;
                INV_CUSTOM_CPD_FIELD_VALUES.INV_CUSTOM_FIELDS = INV_CUSTOM_FIELDS;

                var safetyData = new List<CustomFieldData>();
                safetyData.Add(new CustomFieldMapper().Map(INV_CUSTOM_CPD_FIELD_VALUES));
                containerTest.Compound.SafetyData = safetyData;
            }
        }

        [TestMethod]
        public async Task GetContainerById_ShouldReturnContainerWithSameID()
        {
            SetupControllerForTests(containerController, "containers", HttpMethod.Get);
            var result = await containerController.GetContainerById(containerTest.ContainerId) as ResponseMessageResult;

            Assert.IsNotNull(result.Response);
            Assert.AreEqual(HttpStatusCode.OK, result.Response.StatusCode);
            Assert.AreEqual(containerTest.ContainerId, result.Response.Content.ReadAsAsync<ContainerData>().Result.ContainerId);
        }

        [TestMethod]
        public async Task GetContainerById_ShouldReturnContainerWithAllData()
        {
            SetupControllerForTests(containerController, "containers", HttpMethod.Get);
            var result = await containerController.GetContainerById(containerTest.ContainerId) as ResponseMessageResult;
            ContainerData content = null;
            if (result.Response != null)
            {
                content = result.Response.Content.ReadAsAsync<ContainerData>().Result;
            }

            Assert.IsNotNull(result.Response);
            Assert.AreEqual(HttpStatusCode.OK, result.Response.StatusCode);
            Assert.AreEqual(containerTest.ContainerId, content.ContainerId);
            Assert.AreEqual(containerTest.Barcode, content.Barcode);
            Assert.AreEqual(containerTest.Name, content.Name);
            Assert.AreEqual(containerTest.Type, content.Type);
            Assert.AreEqual(containerTest.ContainerSize, content.ContainerSize);
            Assert.AreEqual(containerTest.QuantityAvailable, content.QuantityAvailable);
            Assert.AreEqual(containerTest.Supplier, content.Supplier);
            Assert.AreEqual(containerTest.CurrentUser, content.CurrentUser);
            Assert.AreEqual(containerTest.UnitOfMeasure, content.UnitOfMeasure);
            Assert.AreEqual(containerTest.DateCreated, content.DateCreated);
            Assert.AreEqual(containerTest.Status, content.Status);
            Assert.AreEqual(containerTest.Compound.CompoundId, content.Compound.CompoundId);
            Assert.AreEqual(containerTest.Compound.MolId, content.Compound.MolId);
            Assert.AreEqual(containerTest.Compound.Cas, content.Compound.Cas);
            Assert.AreEqual(containerTest.Compound.AcxId, content.Compound.AcxId);
            Assert.AreEqual(containerTest.Compound.SubstanceName, content.Compound.SubstanceName);
            Assert.AreEqual(containerTest.Compound.Base64Cdx, content.Compound.Base64Cdx);
            Assert.AreEqual(containerTest.Compound.SafetyData.Count, content.Compound.SafetyData.Count);
            Assert.AreEqual(containerTest.Compound.SafetyData[0].CustomFieldId, content.Compound.SafetyData[0].CustomFieldId);
            Assert.AreEqual(containerTest.Compound.SafetyData[0].CustomFielName, content.Compound.SafetyData[0].CustomFielName);
            Assert.AreEqual(containerTest.Compound.SafetyData[0].CustomFielGroupName, content.Compound.SafetyData[0].CustomFielGroupName);
            Assert.AreEqual(containerTest.Location.Id, content.Location.Id);
            Assert.AreEqual(containerTest.Location.ParentId, content.Location.ParentId);
            Assert.AreEqual(containerTest.Location.Name, content.Location.Name);
            Assert.AreEqual(containerTest.Location.Description, content.Location.Description);
            Assert.AreEqual(containerTest.Location.Barcode, content.Location.Barcode);
        }

        [TestMethod]
        public async Task GetContainerById_ShouldReturnContainerNotFound()
        {
            SetupControllerForTests(containerController, "containers", HttpMethod.Get);
            var result = await containerController.GetContainerById(999) as ResponseMessageResult;

            Assert.IsNotNull(result.Response);
            Assert.AreEqual(HttpStatusCode.NotFound, result.Response.StatusCode);
        }

        [TestMethod]
        public async Task GetContainerByBarcode_ShouldReturnContainerWithSameBarcode()
        {
            SetupControllerForTests(containerController, "containers", HttpMethod.Get);
            var result = await containerController.GetContainerByBarcode(containerTest.Barcode) as ResponseMessageResult;

            Assert.IsNotNull(result.Response);
            Assert.AreEqual(HttpStatusCode.OK, result.Response.StatusCode);
            Assert.AreEqual(containerTest.Barcode, result.Response.Content.ReadAsAsync<ContainerData>().Result.Barcode);
        }

        [TestMethod]
        public async Task GetContainerByBarcode_ShouldReturnContainerWithAllData()
        {
            SetupControllerForTests(containerController, "containers", HttpMethod.Get);
            var result = await containerController.GetContainerByBarcode(containerTest.Barcode) as ResponseMessageResult;
            ContainerData content = null;
            if (result.Response != null)
            {
                content = result.Response.Content.ReadAsAsync<ContainerData>().Result;
            }

            Assert.IsNotNull(result.Response);
            Assert.AreEqual(HttpStatusCode.OK, result.Response.StatusCode);
            Assert.AreEqual(containerTest.ContainerId, content.ContainerId);
            Assert.AreEqual(containerTest.Barcode, content.Barcode);
            Assert.AreEqual(containerTest.Name, content.Name);
            Assert.AreEqual(containerTest.Type, content.Type);
            Assert.AreEqual(containerTest.ContainerSize, content.ContainerSize);
            Assert.AreEqual(containerTest.QuantityAvailable, content.QuantityAvailable);
            Assert.AreEqual(containerTest.Supplier, content.Supplier);
            Assert.AreEqual(containerTest.CurrentUser, content.CurrentUser);
            Assert.AreEqual(containerTest.UnitOfMeasure, content.UnitOfMeasure);
            Assert.AreEqual(containerTest.DateCreated, content.DateCreated);
            Assert.AreEqual(containerTest.Status, content.Status);
            Assert.AreEqual(containerTest.Compound.CompoundId, content.Compound.CompoundId);
            Assert.AreEqual(containerTest.Compound.MolId, content.Compound.MolId);
            Assert.AreEqual(containerTest.Compound.Cas, content.Compound.Cas);
            Assert.AreEqual(containerTest.Compound.AcxId, content.Compound.AcxId);
            Assert.AreEqual(containerTest.Compound.SubstanceName, content.Compound.SubstanceName);
            Assert.AreEqual(containerTest.Compound.Base64Cdx, content.Compound.Base64Cdx);
            Assert.AreEqual(containerTest.Compound.SafetyData.Count, content.Compound.SafetyData.Count);
            Assert.AreEqual(containerTest.Compound.SafetyData[0].CustomFieldId, content.Compound.SafetyData[0].CustomFieldId);
            Assert.AreEqual(containerTest.Compound.SafetyData[0].CustomFielName, content.Compound.SafetyData[0].CustomFielName);
            Assert.AreEqual(containerTest.Compound.SafetyData[0].CustomFielGroupName, content.Compound.SafetyData[0].CustomFielGroupName);
            Assert.AreEqual(containerTest.Location.Id, content.Location.Id);
            Assert.AreEqual(containerTest.Location.ParentId, content.Location.ParentId);
            Assert.AreEqual(containerTest.Location.Name, content.Location.Name);
            Assert.AreEqual(containerTest.Location.Description, content.Location.Description);
            Assert.AreEqual(containerTest.Location.Barcode, content.Location.Barcode);
        }

        [TestMethod]
        public async Task GetContainerByBarcode_ShouldReturnContainerNotFound()
        {
            SetupControllerForTests(containerController, "containers", HttpMethod.Get);
            var result = await containerController.GetContainerByBarcode("999") as ResponseMessageResult;

            Assert.IsNotNull(result.Response);
            Assert.AreEqual(HttpStatusCode.NotFound, result.Response.StatusCode);
        }
    }
}
