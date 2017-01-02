using CambridgeSoft.COE.RegistrationAdmin.Services.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CambridgeSoft.COE.Framework.Common.Messaging;
using System.Xml;

namespace RegistrationAdmin.Services.MSUnitTests
{
    /// <summary>
    ///This is a test class for RegAdminUtilsTest and is intended
    ///to contain all RegAdminUtilsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RegAdminUtilsTest
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
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for GetRegistryPrefix
        ///</summary>
        [TestMethod()]
        public void GetRegistryPrefixTest()
        {
            string expected = string.Empty;
            string actual;
            actual = RegAdminUtils.GetRegistryPrefix();
            Assert.AreEqual<string>(expected, actual, "GetRegistryPrefix() failed to return expected value");
        }

        /// <summary>
        ///A test for CanEditPrecision
        ///</summary>
        [TestMethod()]
        public void CanEditPrecision_NewPrecisionHighTest()
        {
            string oldPrecision = "1.2"; // TODO: Initialize to an appropriate value
            string newPrecision = "1.3"; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = RegAdminUtils.CanEditPrecision(oldPrecision, newPrecision);
            Assert.AreEqual<bool>(expected, actual,"CanEditPrecision(oldPrecision, newPrecision) failed for newprecision with higher value");
        }

        /// <summary>
        ///A test for CanEditPrecision
        ///</summary>
        [TestMethod()]
        public void CanEditPrecision_NewPrecisionLowTest()
        {
            string oldPrecision = "1.2"; // TODO: Initialize to an appropriate value
            string newPrecision = "1.1"; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = RegAdminUtils.CanEditPrecision(oldPrecision, newPrecision);
            Assert.AreEqual<bool>(expected, actual, "CanEditPrecision(oldPrecision, newPrecision) failed for newprecision with lower value");
        }

        /// <summary>
        ///A test for CanEditPrecision
        ///</summary>
        [TestMethod()]
        public void CanEditPrecision_OldPrecisionEmptyTest()
        {
            string oldPrecision = string.Empty; // TODO: Initialize to an appropriate value
            string newPrecision = "1.1"; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = RegAdminUtils.CanEditPrecision(oldPrecision, newPrecision);
            Assert.AreEqual<bool>(expected, actual, "CanEditPrecision(oldPrecision, newPrecision) failed for oldprecision as empty value");
        }

        /// <summary>
        ///A test for CanEditPrecision
        ///</summary>
        [TestMethod()]
        public void CanEditPrecision_NewPrecisionEmptyTest()
        {
            string oldPrecision = "1.2"; // TODO: Initialize to an appropriate value
            string newPrecision = ""; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = RegAdminUtils.CanEditPrecision(oldPrecision, newPrecision);
            Assert.AreEqual<bool>(expected, actual, "CanEditPrecision(oldPrecision, newPrecision) failed for newprecision with empty value");
        }

        /// <summary>
        ///A test for CanEditPrecision
        ///</summary>
        [TestMethod()]
        public void CanEditPrecision_NewPrecisionWithoutDelimiterAndHighTest()
        {
            string oldPrecision = "12"; // TODO: Initialize to an appropriate value
            string newPrecision = "13"; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = RegAdminUtils.CanEditPrecision(oldPrecision, newPrecision);
            Assert.AreEqual<bool>(expected, actual, "CanEditPrecision(oldPrecision, newPrecision) failed for without delimiter");
        }

        /// <summary>
        ///A test for CanEditPrecision
        ///</summary>
        [TestMethod()]
        public void CanEditPrecision_NewPrecisionWithoutDelimiterAndLowTest()
        {
            string oldPrecision = "12"; // TODO: Initialize to an appropriate value
            string newPrecision = "11"; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = RegAdminUtils.CanEditPrecision(oldPrecision, newPrecision);
            Assert.AreEqual<bool>(expected, actual, "CanEditPrecision(oldPrecision, newPrecision) failed for without delimiter and new precison with lower value");
        }

        /// <summary>
        ///A test for CanEditPrecision
        ///</summary>
        [TestMethod()]
        public void CanEditPrecision_PrecisionWithoutDelimiterAndEqualTest()
        {
            string oldPrecision = "11"; // TODO: Initialize to an appropriate value
            string newPrecision = "11"; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = RegAdminUtils.CanEditPrecision(oldPrecision, newPrecision);
            Assert.AreEqual<bool>(expected, actual, "CanEditPrecision(oldPrecision, newPrecision) failed for without delimiter and equal precision values");
        }

        /// <summary>
        ///A test for ConvertPrecision
        ///</summary>
        [TestMethod()]
        public void ConvertPrecision_EmptyPrecisionStringTest()
        {
            string precision = string.Empty; // TODO: Initialize to an appropriate value
            bool toDataBase = false; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.ConvertPrecision(precision, toDataBase);
            Assert.AreEqual<string>(expected, actual, "ConvertPrecision(precision, toDataBase) failed to return expected value with empty input precision value");
        }

        // <summary>
        ///A test for ConvertPrecision
        ///</summary>
        [TestMethod()]
        public void ConvertPrecision_PrecisionStringWithoutDelimiterTest()
        {
            string precision = "123"; // TODO: Initialize to an appropriate value
            bool toDataBase = false; // TODO: Initialize to an appropriate value
            string expected = null; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.ConvertPrecision(precision, toDataBase);
            Assert.AreEqual<string>(expected, actual, "ConvertPrecision(precision, toDataBase) failed to return expected value with input precision without delimiter value");
        }

        // <summary>
        ///A test for ConvertPrecision
        ///</summary>
        [TestMethod()]
        public void ConvertPrecision_PrecisionStringWithDelimiterAndTrueTest()
        {
            string precision = "12.3"; // TODO: Initialize to an appropriate value
            bool toDataBase = true; // TODO: Initialize to an appropriate value
            string expected = "15.3"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.ConvertPrecision(precision, toDataBase);
            Assert.AreEqual<string>(expected, actual, "ConvertPrecision(precision, toDataBase) failed to return expected value");
        }

        // <summary>
        ///A test for ConvertPrecision
        ///</summary>
        [TestMethod()]
        public void ConvertPrecision_PrecisionStringWithDelimiterAndFalseTest()
        {
            string precision = "12.3"; // TODO: Initialize to an appropriate value
            bool toDataBase = false; // TODO: Initialize to an appropriate value
            string expected = "9.3"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.ConvertPrecision(precision, toDataBase);
            Assert.AreEqual<string>(expected, actual, "ConvertPrecision(precision, toDataBase) failed to return expected value");
        }

        /// <summary>
        ///A test for GetBatchCSSClass
        ///</summary>
        [TestMethod()]
        public void GetBatchCSSClassTest()
        {
            string expected = "BatchPropertyListFormElement"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetBatchCSSClass();
            Assert.AreEqual<string>(expected, actual, "GetBatchCSSClass() did not return the expected value");
        }

        /// <summary>
        ///A test for GetBatchCompCSSClass
        ///</summary>
        [TestMethod()]
        public void GetBatchCompCSSClassTest()
        {
            string expected = "BatchCompPropertyListFormElement"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetBatchCompCSSClass();
            Assert.AreEqual<string>(expected, actual, "GetBatchCompCSSClass() did not return the expected value");
        }

        /// <summary>
        ///A test for GetBatchComponentsPrefix
        ///</summary>
        [TestMethod()]
        public void GetBatchComponentsPrefixTest()
        {
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetBatchComponentsPrefix();
            Assert.AreEqual<string>(expected, actual, "GetBatchComponentsPrefix() did not return the expected value");
        }

        /// <summary>
        ///A test for GetBatchPrefix
        ///</summary>
        [TestMethod()]
        public void GetBatchPrefixTest()
        {
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetBatchPrefix();
            Assert.AreEqual<string>(expected, actual, "GetBatchPrefix() did not return the expected value");
        }

        /// <summary>
        ///A test for GetComponentCSSClass
        ///</summary>
        [TestMethod()]
        public void GetComponentCSSClassTest()
        {
            string expected = "ComponentPropertyListFormElement"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetComponentCSSClass();
            Assert.AreEqual<string>(expected, actual, "GetComponentCSSClass() did not return the expected value");
        }

        /// <summary>
        ///A test for GetComponentPrefix
        ///</summary>
        [TestMethod()]
        public void GetComponentPrefixTest()
        {
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetComponentPrefix();
            Assert.AreEqual<string>(expected, actual, "GetComponentPrefix() did not return the expected value");
        }

        /// <summary>
        ///A test for GetCustomPropertyStyles
        ///</summary>
        [TestMethod()]
        public void GetCustomPropertyStylesTest()
        {
            string[] expected = null; // TODO: Initialize to an appropriate value
            string[] actual;
            XmlDocument configSettings = Helpers.XMLDataLoader.LoadConfigurationSettings();
            string xPath = @"Registration/applicationSettings/groups/add[@name='MISC']/settings/add[@name='CustomPropertyStyles']";
            XmlNode customPropertyStylesNode = configSettings.SelectSingleNode(xPath);
            Assert.IsNotNull(customPropertyStylesNode, "configuration settings does not contain custom property styles node");
            string customPropertyStyle = customPropertyStylesNode.Attributes["value"].Value;
            Assert.IsTrue(!string.IsNullOrEmpty(customPropertyStyle), "custom property style doen not have value configured");
            expected = customPropertyStyle.Split('|');
            actual = RegAdminUtils.GetCustomPropertyStyles();
            Assert.AreEqual(expected.Length, actual.Length, "GetCustomPropertyStyles() did not return the expected value");
        }

        /// <summary>
        ///A test for GetDefaultControlStyle
        ///</summary>
        [TestMethod()]
        public void GetDefaultControlStyle_COETextAreaAndViewTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextArea"; // TODO: Initialize to an appropriate value
            FormGroup.DisplayMode mode = FormGroup.DisplayMode.View; // TODO: Initialize to an appropriate value
            string expected ="FETextAreaViewMode"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetDefaultControlStyle(controlType, mode);
            Assert.AreEqual<string>(expected, actual, "GetDefaultControlStyle(controlType, mode) did not return expected value");
        }

        /// <summary>
        ///A test for GetDefaultControlStyle
        ///</summary>
        [TestMethod()]
        public void GetDefaultControlStyle_COETextAreaTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextArea"; // TODO: Initialize to an appropriate value
            FormGroup.DisplayMode mode = FormGroup.DisplayMode.All; // TODO: Initialize to an appropriate value
            string expected = "FETextArea"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetDefaultControlStyle(controlType, mode);
            Assert.AreEqual<string>(expected, actual, "GetDefaultControlStyle(controlType, mode) did not return expected value");
        }

        /// <summary>
        ///A test for GetDefaultControlStyle
        ///</summary>
        [TestMethod()]
        public void GetDefaultControlStyle_COETextAreaReadOnlyTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextAreaReadOnly"; // TODO: Initialize to an appropriate value
            FormGroup.DisplayMode mode = FormGroup.DisplayMode.All; // TODO: Initialize to an appropriate value
            string expected = "FETextAreaViewMode"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetDefaultControlStyle(controlType, mode);
            Assert.AreEqual<string>(expected, actual, "GetDefaultControlStyle(controlType, mode) did not return expected value");
        }

        /// <summary>
        ///A test for GetDefaultControlStyle
        ///</summary>
        [TestMethod()]
        public void GetDefaultControlStyle_COENumericTextBoxAndViewTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COENumericTextBox"; // TODO: Initialize to an appropriate value
            FormGroup.DisplayMode mode = FormGroup.DisplayMode.View; // TODO: Initialize to an appropriate value
            string expected = "FETextBoxViewMode"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetDefaultControlStyle(controlType, mode);
            Assert.AreEqual<string>(expected, actual, "GetDefaultControlStyle(controlType, mode) did not return expected value");
        }

        /// <summary>
        ///A test for GetDefaultControlStyle
        ///</summary>
        [TestMethod()]
        public void GetDefaultControlStyle_COENumericTextBoxTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COENumericTextBox"; // TODO: Initialize to an appropriate value
            FormGroup.DisplayMode mode = FormGroup.DisplayMode.All; // TODO: Initialize to an appropriate value
            string expected = "FETextBox"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetDefaultControlStyle(controlType, mode);
            Assert.AreEqual<string>(expected, actual, "GetDefaultControlStyle(controlType, mode) did not return expected value");
        }

        /// <summary>
        ///A test for GetDefaultControlStyle
        ///</summary>
        [TestMethod()]
        public void GetDefaultControlStyle_COETextBoxAndViewTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBox"; // TODO: Initialize to an appropriate value
            FormGroup.DisplayMode mode = FormGroup.DisplayMode.View; // TODO: Initialize to an appropriate value
            string expected = "FETextBoxViewMode"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetDefaultControlStyle(controlType, mode);
            Assert.AreEqual<string>(expected, actual, "GetDefaultControlStyle(controlType, mode) did not return expected value");
        }

        /// <summary>
        ///A test for GetDefaultControlStyle
        ///</summary>
        [TestMethod()]
        public void GetDefaultControlStyle_COETextBoxTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBox"; // TODO: Initialize to an appropriate value
            FormGroup.DisplayMode mode = FormGroup.DisplayMode.All; // TODO: Initialize to an appropriate value
            string expected = "FETextBox"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetDefaultControlStyle(controlType, mode);
            Assert.AreEqual<string>(expected, actual, "GetDefaultControlStyle(controlType, mode) did not return expected value");
        }

        /// <summary>
        ///A test for GetDefaultControlStyle
        ///</summary>
        [TestMethod()]
        public void GetDefaultControlStyle_COETextBoxReadOnlyTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly"; // TODO: Initialize to an appropriate value
            FormGroup.DisplayMode mode = FormGroup.DisplayMode.All; // TODO: Initialize to an appropriate value
            string expected = "FETextBoxViewMode"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetDefaultControlStyle(controlType, mode);
            Assert.AreEqual<string>(expected, actual, "GetDefaultControlStyle(controlType, mode) did not return expected value");
        }

        /// <summary>
        ///A test for GetDefaultControlStyle
        ///</summary>
        [TestMethod()]
        public void GetDefaultControlStyle_COEDropDownListAndViewTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList"; // TODO: Initialize to an appropriate value
            FormGroup.DisplayMode mode = FormGroup.DisplayMode.View; // TODO: Initialize to an appropriate value
            string expected = "FEDropDownListViewMode"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetDefaultControlStyle(controlType, mode);
            Assert.AreEqual<string>(expected, actual, "GetDefaultControlStyle(controlType, mode) did not return expected value");
        }

        /// <summary>
        ///A test for GetDefaultControlStyle
        ///</summary>
        [TestMethod()]
        public void GetDefaultControlStyle_COEDropDownListTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList"; // TODO: Initialize to an appropriate value
            FormGroup.DisplayMode mode = FormGroup.DisplayMode.All; // TODO: Initialize to an appropriate value
            string expected = "FEDropDownList"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetDefaultControlStyle(controlType, mode);
            Assert.AreEqual<string>(expected, actual, "GetDefaultControlStyle(controlType, mode) did not return expected value");
        }

        /// <summary>
        ///A test for GetDefaultControlStyle
        ///</summary>
        [TestMethod()]
        public void GetDefaultControlStyle_COEDatePickerAndViewTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePicker"; // TODO: Initialize to an appropriate value
            FormGroup.DisplayMode mode = FormGroup.DisplayMode.View; // TODO: Initialize to an appropriate value
            string expected = "FEDropDownListViewMode"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetDefaultControlStyle(controlType, mode);
            Assert.AreEqual<string>(expected, actual, "GetDefaultControlStyle(controlType, mode) did not return expected value");
        }

        /// <summary>
        ///A test for GetDefaultControlStyle
        ///</summary>
        [TestMethod()]
        public void GetDefaultControlStyle_COEDatePickerTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePicker"; // TODO: Initialize to an appropriate value
            FormGroup.DisplayMode mode = FormGroup.DisplayMode.All; // TODO: Initialize to an appropriate value
            string expected = "FEDropDownList"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetDefaultControlStyle(controlType, mode);
            Assert.AreEqual<string>(expected, actual, "GetDefaultControlStyle(controlType, mode) did not return expected value");
        }

        /// <summary>
        ///A test for GetDefaultControlStyle
        ///</summary>
        [TestMethod()]
        public void GetDefaultControlStyle_COEDatePickerReadOnlyTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePickerReadOnly"; // TODO: Initialize to an appropriate value
            FormGroup.DisplayMode mode = FormGroup.DisplayMode.All; // TODO: Initialize to an appropriate value
            string expected = "FEDropDownListViewMode"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetDefaultControlStyle(controlType, mode);
            Assert.AreEqual<string>(expected, actual, "GetDefaultControlStyle(controlType, mode) did not return expected value");
        }

        /// <summary>
        ///A test for GetDefaultControlStyle
        ///</summary>
        [TestMethod()]
        public void GetDefaultControlStyle_EmptyControlTypeTest()
        {
            string controlType = ""; // TODO: Initialize to an appropriate value
            FormGroup.DisplayMode mode = FormGroup.DisplayMode.All; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetDefaultControlStyle(controlType, mode);
            Assert.AreEqual<string>(expected, actual, "GetDefaultControlStyle(controlType, mode) did not return expected value");
        }


        /// <summary>
        ///A test for GetFormElementCSSClass
        ///</summary>
        [TestMethod()]
        public void GetFormElementCSSClass_COETextAreaTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextArea"; // TODO: Initialize to an appropriate value
            string expected = "Std100x80"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetFormElementCSSClass(controlType);
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        ///A test for GetFormElementCSSClass
        ///</summary>
        [TestMethod()]
        public void GetFormElementCSSClass_COETextAreaReadOnlyTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextAreaReadOnly"; // TODO: Initialize to an appropriate value
            string expected = "Std100x80"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetFormElementCSSClass(controlType);
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        ///A test for GetFormElementCSSClass
        ///</summary>
        [TestMethod()]
        public void GetFormElementCSSClass_COETextBoxTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBox"; // TODO: Initialize to an appropriate value
            string expected = "Std20x40"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetFormElementCSSClass(controlType);
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        ///A test for GetFormElementCSSClass
        ///</summary>
        [TestMethod()]
        public void GetFormElementCSSClass_COENumericTextBoxTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COENumericTextBox"; // TODO: Initialize to an appropriate value
            string expected = "Std20x40"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetFormElementCSSClass(controlType);
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        ///A test for GetFormElementCSSClass
        ///</summary>
        [TestMethod()]
        public void GetFormElementCSSClass_COETextBoxReadOnlyTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly"; // TODO: Initialize to an appropriate value
            string expected = "Std20x40"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetFormElementCSSClass(controlType);
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        ///A test for GetFormElementCSSClass
        ///</summary>
        [TestMethod()]
        public void GetFormElementCSSClass_COEDatePickerTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePicker"; // TODO: Initialize to an appropriate value
            string expected = "CalenderClass"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetFormElementCSSClass(controlType);
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        ///A test for GetFormElementCSSClass
        ///</summary>
        [TestMethod()]
        public void GetFormElementCSSClass_COEDropDownListTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList"; // TODO: Initialize to an appropriate value
            string expected = "Std20x40"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetFormElementCSSClass(controlType);
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        ///A test for GetFormElementCSSClass
        ///</summary>
        [TestMethod()]
        public void GetFormElementCSSClass_COEDatePickerReadOnlyTest()
        {
            string controlType = "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePickerReadOnly"; // TODO: Initialize to an appropriate value
            string expected = "Std20x40"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetFormElementCSSClass(controlType);
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        ///A test for GetFormElementCSSClass
        ///</summary>
        [TestMethod()]
        public void GetFormElementCSSClass_EmptyControlTypeTest()
        {
            string controlType = ""; // TODO: Initialize to an appropriate value
            string expected = "Std25x40"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetFormElementCSSClass(controlType);
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        ///A test for GetRegCustomFormGroupsIds
        ///</summary>
        [TestMethod()]
        public void GetRegCustomFormGroupsIdsTest()
        {
            string[] expected = null; // TODO: Initialize to an appropriate value
            string[] actual;
            XmlDocument configSettings = Helpers.XMLDataLoader.LoadConfigurationSettings();
            string xPath = @"Registration/applicationSettings/groups/add[@name='REGADMIN']/settings/add[@name='CustomRegFormGroupsIds']";
            XmlNode customPropertyStylesNode = configSettings.SelectSingleNode(xPath);
            Assert.IsNotNull(customPropertyStylesNode, "configuration settings does not contain custom property styles node");
            string customPropertyStyle = customPropertyStylesNode.Attributes["value"].Value;
            Assert.IsTrue(!string.IsNullOrEmpty(customPropertyStyle), "custom property style doen not have value configured");
            expected = customPropertyStyle.Split('|');
            actual = RegAdminUtils.GetRegCustomFormGroupsIds();
            Assert.AreEqual<int>(expected.Length, actual.Length);
        }

        /// <summary>
        ///A test for GetRegistryCSSClass
        ///</summary>
        [TestMethod()]
        public void GetRegistryCSSClassTest()
        {
            string expected = "RegPropertyListFormElement"; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetRegistryCSSClass();
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        ///A test for GetStructurePrefix
        ///</summary>
        [TestMethod()]
        public void GetStructurePrefixTest()
        {
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = RegAdminUtils.GetStructurePrefix();
            Assert.AreEqual<string>(expected, actual);
        }

        /// <summary>
        ///A test for GetLinkTarget
        ///</summary>
        [TestMethod()]
        public void GetLinkTargetTest()
        {
            string expected = "_blank";
            string actual;
            actual = RegAdminUtils.GetLinkTarget;
            Assert.AreEqual<string>(expected, actual);
        }
    }
}
