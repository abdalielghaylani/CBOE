using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using System.Xml;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using CambridgeSoft.COE.Framework.Common;
using System.Diagnostics;
using CambridgeSoft.COE.Framework.COEConfigurationService;

namespace CambridgeSoft.COE.Framework.Common.UnitTests
{
	/// <summary>
	/// Summary description for XMLExamples
	/// </summary>
	[TestFixture]
	public class XMLExamples
	{
        private string typesBasePath = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.IndexOf("CambridgeSoft.COE.Framework")) + @"CambridgeSoft.COE.Framework\Common";
            
          //  Utilities.GetProjectBasePath("CambridgeSoft.COE.Framework") + "\\Common";

		public XMLExamples() {
			//
			// TODO: Add constructor logic here
			//
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [TestFixtureSetUp]
		// public static void MyClassInitialize() { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [TestFixtureTearDown]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [SetUp]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TearDown]
		// public void MyTestCleanup() { }
		//
		#endregion

		#region Methods
		/// <summary>


		/// </summary>
		[Test]
        [Explicit("Fix it later")]
		public void XMLExamplesTest() {
			try {
                COEConfigurationBO.DefaultConfigurationFilePath = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.IndexOf("ServiceTier")) + @"ChemOfficeEnterprise11\COEFrameworkConfigNUnit.xml";
                NunitUtil.AddInstanceTag(COEConfigurationBO.DefaultConfigurationFilePath);
				string exampleName = "misc";
				XmlDocument resultCriteriaXMLDocument = GetResultCriteria(exampleName);
				XmlDocument searchCriteriaXMLDocument = GetSearchCriteria(exampleName);
				XmlDocument dataViewXMLDocument = GetDataView(exampleName);

				QueryBuilder target = new QueryBuilder(dataViewXMLDocument, searchCriteriaXMLDocument, resultCriteriaXMLDocument);

				Query[] resultingQueries = target.BuildQuery(DBMSType.ORACLE);

				Debug.WriteLine("SQLGenerated queries:");
				Debug.WriteLine("--------------------------------------------------------------");

				for (int index = 0; index < resultingQueries.Length; index++) {
					Debug.WriteLine("Query:");
					Debug.WriteLine(resultingQueries[index].ToString().Trim());
					Debug.WriteLine("Parameters:");

					for (int parameterIndex = 0; parameterIndex < resultingQueries[index].ParamValues.Count; parameterIndex++)
						Debug.WriteLine(parameterIndex.ToString() + " - Value: '" +
												resultingQueries[index].ParamValues[parameterIndex].Val + "'\t Type: " +
												resultingQueries[index].ParamValues[parameterIndex].Type);

					Debug.WriteLine("--------------------------------------------------------------");
				}
			} catch (Exception exception) {
				Assert.Fail(exception.Message);
			}
		}

		private bool CompareElements(List<Value> values_expected, List<Value> values) {
			if (values_expected.Count != values.Count)
				return false;
			for (int i = 0; i < values.Count; i++) {

				if (values_expected[i] != values[i])
					return false;
			}
			return true;
		}
		private XmlDocument GetDataView(string exampleName) {
			XmlDocument document = new XmlDocument();
            document.Load(typesBasePath + @"\Messaging\xsd\XML Examples\Get Data (multiple queries)\DataView.xml");

			return document;
		}

		private XmlDocument GetSearchCriteria(string exampleName) {
			XmlDocument document = new XmlDocument();

            switch(exampleName.Trim().ToLower()) {
                case "createhitlist":
                    document.Load(typesBasePath + @"\Messaging\xsd\XML Examples\Hit List Insertion\SearchCriteria.xml");
                    break;
                case "getdata":
                    document.Load(typesBasePath + @"\Messaging\xsd\XML Examples\Get Data (multiple queries)\SearchCriteria.xml");
                    break;
                case "misc":
                    document.Load(typesBasePath + @"\Messaging\xsd\XML Examples\Misc\SearchCriteria.xml");
                    break;
            }
			return document;
		}


		private XmlDocument GetResultCriteria(string exampleName) {
			string xml = string.Empty;
			XmlDocument document = new XmlDocument();

			switch (exampleName.Trim().ToLower()) {
				case "createhitlist":
                    document.Load(typesBasePath + @"\Messaging\xsd\XML Examples\Hit List Insertion\ResultsCriteria.xml");
					break;
				case "getdata":
                    document.Load(typesBasePath + @"\Messaging\xsd\XML Examples\Get Data (multiple queries)\ResultsCriteria.xml");
					break;
				case "misc":
                    document.Load(typesBasePath + @"\Messaging\xsd\XML Examples\Misc\ResultsCriteria.xml");
					break;
			}

			return document;
		}
		#endregion
	}
}
