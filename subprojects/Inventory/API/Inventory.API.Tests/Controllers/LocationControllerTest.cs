using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerkinElmer.COE.Inventory.API.Controllers;
using PerkinElmer.COE.Inventory.DAL;
using PerkinElmer.COE.Inventory.Model;

namespace PerkinElmer.COE.Inventory.API.Tests.Controllers
{
    [TestClass]
    public class LocationControllerTest : BaseControllerTest
    {
        protected LocationController locationController;

        public LocationControllerTest()
        {
            Context.INV_LOCATIONS.Add(new INV_LOCATIONS() { LOCATION_ID = 1, LOCATION_NAME = "Test name 1", LOCATION_DESCRIPTION = "Test description 1" });
            Context.INV_LOCATIONS.Add(new INV_LOCATIONS() { LOCATION_ID = 2, LOCATION_NAME = "Test name 2", LOCATION_DESCRIPTION = "Test description 2" });
            Context.INV_LOCATIONS.Add(new INV_LOCATIONS() { LOCATION_ID = 3, LOCATION_NAME = "Test name 3", LOCATION_DESCRIPTION = "Test description 3" });

            locationController = new LocationController(Context);
        }

        [TestMethod]
        public async Task GetLocationById_ShouldReturnLocationWithSameID()
        {
            SetupControllerForTests(locationController, "locations", HttpMethod.Get);
            var result = await locationController.GetLocationById(1) as ResponseMessageResult;

            Assert.IsNotNull(result.Response);
            Assert.AreEqual(HttpStatusCode.OK, result.Response.StatusCode);
            Assert.AreEqual(1, result.Response.Content.ReadAsAsync<LocationData>().Result.Id);
        }

        [TestMethod]
        public async Task GetLocationById_ShouldReturnLocationWithAllData()
        {
            SetupControllerForTests(locationController, "locations", HttpMethod.Get);
            var result = await locationController.GetLocationById(1) as ResponseMessageResult;
            LocationData content = null;
            if (result.Response != null)
            {
                content = result.Response.Content.ReadAsAsync<LocationData>().Result;
            }

            Assert.IsNotNull(result.Response);
            Assert.AreEqual(HttpStatusCode.OK, result.Response.StatusCode);
            Assert.AreEqual(1, content.Id);
            Assert.AreEqual("Test name 1", content.Name);
            Assert.AreEqual("Test description 1", content.Description);
        }

        [TestMethod]
        public async Task GetLocationById_ShouldReturnLocationNotFound()
        {
            SetupControllerForTests(locationController, "locations", HttpMethod.Get);
            var result = await locationController.GetLocationById(10) as ResponseMessageResult;

            Assert.IsNotNull(result.Response);
            Assert.AreEqual(HttpStatusCode.NotFound, result.Response.StatusCode);
        }

        [TestMethod]
        public async Task GetLocations_ShouldReturnAllLocations()
        {
            SetupControllerForTests(locationController, "locations", HttpMethod.Get);
            var result = await locationController.GetLocations() as ResponseMessageResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.Response.StatusCode);
            Assert.AreEqual(3, result.Response.Content.ReadAsAsync<List<LocationData>>().Result.Count);
        }
    }
}
