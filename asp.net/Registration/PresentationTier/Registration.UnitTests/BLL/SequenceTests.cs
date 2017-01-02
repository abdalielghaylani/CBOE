using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.Registration.Services.Types;

using NUnit.Framework;

namespace CambridgeSoft.COE.Registration.UnitTests.BLL
{
    /// <summary>
    /// At time of creation, this class tests the DAL aspects of the Sequence object.
    /// </summary>
    /// <remarks>
    /// This domain object is not used for managing database data; Table Editor is used for that.
    /// </remarks>
    [TestFixture]
    public class SequenceTests
    {
        private const int SEQUENCE_ID = 1;

        [SetUp]
        public void SetUp()
        {
            Helpers.Authentication.Logon("T5_85", "T5_85");
        }

        /// <summary>
        /// Ensures that Sequence domain objects can be retrieved by their IDs.
        /// </summary>
        /// <remarks>
        /// ID '1' will exist in new databases and using the default Registration.Net configuration.
        /// </remarks>
        [Test]
        public void CanFetchSequence()
        {
            Sequence sequenceInfo = Sequence.GetSequence(SEQUENCE_ID);
            int padding = Convert.ToInt32(sequenceInfo.BatchNumberLength);
            Assert.IsAssignableFrom(typeof(Int32), padding);
        }

        [TearDown]
        public void TearDown()
        {
            Helpers.Authentication.Logoff();
        }
    }
}
