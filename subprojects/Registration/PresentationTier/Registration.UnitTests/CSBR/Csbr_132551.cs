using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.Registration.Services.Types;

using NUnit.Framework;

namespace CambridgeSoft.COE.Registration.UnitTests
{
    [TestFixture]
    public class Csbr_132551
    {
        private string sodiumIonSalt = "2";
        private string expectedErrorMessage = "Don't want to save changes!";

        [SetUp]
        public void SetUp()
        {
            Helpers.Authentication.Logon("T5_85", "T5_85");
        }

        [Test]
        [ExpectedException(typeof(COE.Framework.ExceptionHandling.COEBusinessLayerException))]
        public void Can_Fetch_Fragment_Details()
        {
            RegistryRecord rr = RegistryRecord.GetRegistryRecord("AB-000001");
            rr.AddFragment(-1, sodiumIonSalt, 1);

            //Add the event-handler
            rr.Updating += new EventHandler(rr_Updating);

            try
            {
                RegistryRecord rr2 = rr.Save();
            }
            catch (Exception ex)
            {
                Exception innermostError = ex.GetBaseException();
                if (innermostError.Message == expectedErrorMessage)
                {
                    //ensure we throw the COEBusinessLayerException or the test will fail
                    throw;
                }
            }
            finally
            {
                //disposal eliminates event-listeners which allows the GC of the object
                rr.Updating -= new EventHandler(rr_Updating);
                rr.Dispose();
            }
        }

        /// <summary>
        /// This event-handler code will ensure that the fragment has been added and the
        /// subsequent details retireved.
        /// </summary>
        /// <param name="sender">the RegistryRecord object notifying this listener</param>
        /// <param name="e">null event arguments</param>
        private void rr_Updating(object sender, EventArgs e)
        {
            RegistryRecord rr = (RegistryRecord)sender;
            Assert.AreEqual(rr.ComponentList[0].Compound.FragmentList[0].Formula, "Na+");
            //prevent saving to repository by throwing an exception
            throw new NotSupportedException(expectedErrorMessage);
        }

        [TearDown]
        public void TearDown()
        {
            Helpers.Authentication.Logoff();
        }
    }
}
