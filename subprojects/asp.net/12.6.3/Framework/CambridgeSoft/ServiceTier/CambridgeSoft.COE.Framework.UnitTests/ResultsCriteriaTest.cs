﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.Common;
using System.Xml.Serialization;
using System.IO;
namespace CambridgeSoft.COE.Framework.Common.UnitTests
{
    /// <summary>
    ///This is a test class for CambridgeSoft.COE.Framework.Common.ResultsCriteria.Switch and is intended
    ///to contain all CambridgeSoft.COE.Framework.Common.ResultsCriteria.Switch Unit Tests
    ///</summary>
    [TestClass()]
    public class SwitchTest
    {


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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion 


        /// <summary>
        ///A test for GenerateXmlSnippet ()
        ///</summary>
        [TestMethod()]
        public void Switch_GenerateXmlSnippetTest()
        {
            ResultsCriteria.Condition condition = new ResultsCriteria.Condition("José", new ResultsCriteria.Field(1));

            ResultsCriteria.Concatenation concatenation = new ResultsCriteria.Concatenation();
            concatenation.Operands.Add(new ResultsCriteria.Field(2));
            concatenation.Operands.Add(new ResultsCriteria.Literal(" Honguito "));

            ResultsCriteria.Condition defaultCondition = new ResultsCriteria.Condition();
            defaultCondition.Conditional = concatenation;
            defaultCondition.IsDefault = true;

            ResultsCriteria.Switch target = new ResultsCriteria.Switch();
            target.Clause = new ResultsCriteria.Field(3);
            target.Conditions.Add(condition);
            target.Conditions.Add(defaultCondition);
            target.Alias = "Greatests switch-case";

            string expected = "<?xml version=\"1.0\" encoding=\"utf-16\"?><switch xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" visible=\"true\" alias=\"Greatests switch-case\" orderById=\"0\" direction=\"asc\" inputType=\"text\">  <field visible=\"true\" orderById=\"0\" direction=\"asc\" fieldId=\"3\" />  <conditions>    <condition visible=\"true\" orderById=\"0\" direction=\"asc\" value=\"José\" default=\"false\">      <field visible=\"true\" orderById=\"0\" direction=\"asc\" fieldId=\"1\" />    </condition>    <condition visible=\"true\" orderById=\"0\" direction=\"asc\" value=\"\" default=\"true\">      <concatenation visible=\"true\" orderById=\"0\" direction=\"asc\">        <field visible=\"true\" orderById=\"0\" direction=\"asc\" fieldId=\"2\" />        <literal visible=\"true\" orderById=\"0\" direction=\"asc\"> Honguito </literal>      </concatenation>    </condition>  </conditions></switch>";
            string actual = GenerateXmlSnippet(target, typeof(ResultsCriteria.Switch)).Replace("\r\n", "");

            Assert.AreEqual(expected, actual, "CambridgeSoft.COE.Framework.Common.ResultsCriteria.Switch.GenerateXmlSnippet did " +
                    "not return the expected value.");
        }

        string GenerateXmlSnippet(Object objectToSerialize, Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);

            StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, objectToSerialize);
            stringWriter.Flush();

            return stringWriter.ToString();
        }
    }


}