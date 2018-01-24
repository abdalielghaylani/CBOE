using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using CambridgeSoft.COE.Framework.COEDatabasePublishingService;
using CambridgeSoft.COE.Framework.Controls;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.Common.Messaging;


namespace NUnitTest
{
    [TestFixture]
    public class Paging_Test : LoginBase
    {
        public void Test1()
        {
        }

        /// <summary>
        ///Page Search based on Record Number Regular Value
        ///</summary>
        [Test]
        public void PageBasedOnRecNo()
        {
            int currentoutput = 0;// TODO: Initialize to an appropriate value
            int expectedOutput = 2;
            string RecNo = "15";// TODO: Initialize to an appropriate Record Number to Search
            int GridPageSize = 10;// TODO: Initialize to an appropriate value
            FormGroup.CurrentFormEnum CurrentMode = FormGroup.CurrentFormEnum.ListForm;// TODO: Initialize to an appropriate Mode

            currentoutput = RecordSearch(RecNo, GridPageSize, CurrentMode);
            Assert.AreEqual(expectedOutput, currentoutput, "Did not Return the Expected Page");
        }

        /// <summary>
        ///Page Search based on Record Number with Negative Value
        ///</summary> 
        [Test]
        public void PageBasedOnRecNo2()
        {
            int currentoutput = 0;// TODO: Initialize to an appropriate value
            int expectedOutput = 1;
            string RecNo = "-25";// TODO: Initialize to an appropriate Record Number to Search
            int GridPageSize = 10;// TODO: Initialize to an appropriate value
            FormGroup.CurrentFormEnum CurrentMode = FormGroup.CurrentFormEnum.ListForm;// TODO: Initialize to an appropriate Mode

            currentoutput = RecordSearch(RecNo, GridPageSize, CurrentMode);
            Assert.AreEqual(expectedOutput, currentoutput, "Did not Return the Expected Page");
        }

        /// <summary>
        ///Page Search based on Record Number with Decimal Value
        ///</summary> 
        [Test]
        public void PageBasedOnRecNo3()
        {
            int currentoutput = 0;// TODO: Initialize to an appropriate value
            int expectedOutput = 2;
            string RecNo = "12.55";// TODO: Initialize to an appropriate Record Number to Search
            int GridPageSize = 10;// TODO: Initialize to an appropriate value
            FormGroup.CurrentFormEnum CurrentMode = FormGroup.CurrentFormEnum.ListForm;// TODO: Initialize to an appropriate Mode

            currentoutput = RecordSearch(RecNo, GridPageSize, CurrentMode);
            Assert.AreEqual(expectedOutput, currentoutput, "Did not Return the Expected Page");
        }



        /// <summary>
        ///Page Search based on Record Number with Decimal Value
        ///</summary> 
        [Test]
        public void PageBasedOnRecNo4()
        {
            int currentoutput = 0;// TODO: Initialize to an appropriate value
            int expectedOutput = 0;
            string RecNo = "0";// TODO: Initialize to an appropriate Record Number to Search
            int GridPageSize = 10;// TODO: Initialize to an appropriate value
            FormGroup.CurrentFormEnum CurrentMode = FormGroup.CurrentFormEnum.ListForm;// TODO: Initialize to an appropriate Mode

            currentoutput = RecordSearch(RecNo, GridPageSize, CurrentMode);
            Assert.AreEqual(expectedOutput, currentoutput, "Did not Return the Expected Page");

        }


        #region Private Class

        private int RecordSearch(string RecNo, int GridPageSize, FormGroup.CurrentFormEnum CurrentMode)
        {
            int currentpage = 0;
            if (CurrentMode == FormGroup.CurrentFormEnum.ListForm)
            {
                currentpage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(RecNo) / GridPageSize));
            }
            else if (CurrentMode == FormGroup.CurrentFormEnum.DetailForm)
            {
                currentpage = Convert.ToInt32(RecNo);
            }
            else
            {
                currentpage = 1;
            }
            return currentpage;
        }

        #endregion

    }

}
