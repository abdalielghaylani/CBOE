using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerkinElmer.COE.Inventory.API;
using PerkinElmer.COE.Inventory.API.Controllers;

namespace PerkinElmer.COE.Inventory.API.Tests.Controllers
{
    [TestClass]
    public class LocationControllerTest
    {
        [TestMethod]
        public async void Get()
        {
            // Arrange
            LocationController controller = new LocationController();

            // Act
            var result = await controller.GetLocations();

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
