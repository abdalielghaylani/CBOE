using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Xml;

using Csla.Security;
using NUnit.Framework;

using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser;
using CambridgeSoft.COE.DataLoader.Core.FileParser.SD;
using CambridgeSoft.COE.DataLoader.Core.DataMapping;

using CambridgeSoft.COE.Framework.Services;
using CambridgeSoft.COE.Registration.Services.Types;

using CambridgeSoft.COE.DataLoader.Core.FileParser.CSV;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.DataMapping
{
    [TestFixture]
    public class DataMapping_UnitTest
    {
        IFileReader _reader = null;
        SourceFileInfo _sourceFileInfo = null;

        [SetUp]
        public void Initialize()
        {
            Initialize(@"SD\SAMPLE.sdf");
        }

        #region Map to MockRegistryRecord

        /// <summary>
        /// Test whether or not the first record in SAMPLE.sdf file can be mapped to a new
        /// mock destination record.
        /// </summary>
        [Test]
        public void Test_CanMapToMockRegistryRecord()
        {
            string expectedMolName = "Benzene";
            DateTime expectedDate = Convert.ToDateTime("1/1/91").ToUniversalTime();
            
            JobParameters jobParameters = new JobParameters();
            jobParameters.Mappings = Mappings.GetMappingsFromFile(UnitUtils.GetMappingFilePath("MockPropertyMapping.xml"));
            jobParameters.SourceRecords = _reader.ExtractToRecords(new IndexRange(0,0));

            JobResponse jobResponse = JobServiceCaller.MapRecords(jobParameters);
            List<IDestinationRecord> destinationRecordList = jobResponse.ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

            Assert.IsNotNull(destinationRecordList);
            Assert.AreEqual(1, destinationRecordList.Count);
            
            IDestinationRecord destinationRecord = destinationRecordList[0];

            Assert.AreEqual(expectedMolName, ((MockRegistryRecord)destinationRecord).MolName);
            Assert.AreEqual(expectedDate, ((MockRegistryRecord)destinationRecord).BatchList[0].DateCreated.ToUniversalTime());
        }

        [Test]
        public void Test_CanMapToMockRegistryRecordByMockPicklistId()
        {
            Initialize("Sample_With_Picklist.sdf");

            string expectedPicklistItemId = "3";

            string xmlFilePath = Path.Combine(UnitUtils.GetTestFolderPath(), "PicklistXmlRepresentation.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);

            JobParameters jobParameters = new JobParameters();
            jobParameters.Mappings = Mappings.GetMappingsFromFile(UnitUtils.GetMappingFilePath("PicklistMethodMapping.xml"));
            jobParameters.SourceRecords = _reader.ExtractToRecords(new IndexRange(0, 0));
            // TODO: Use the new way to add a picklist, instead of maintaining a Picklist property in JobParameters
            //JobUtility.AddPicklist("", Picklist.NewPicklist(doc.OuterXml));
            
            JobResponse jobResponse = JobServiceCaller.MapRecords(jobParameters);

            List<IDestinationRecord> destinationRecordList = jobResponse.ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

            Assert.IsNotNull(destinationRecordList);
            Assert.AreEqual(1, destinationRecordList.Count);

            IDestinationRecord destinationRecord = destinationRecordList[0];

            Assert.AreEqual(expectedPicklistItemId, ((MockRegistryRecord)destinationRecord).PropertyList["Units"].Value);
        }

        [Test]
        public void Test_CanMapToMockRegistryRecordByMockPicklistName()
        {
            Initialize("Sample_With_Picklist.sdf");

            //made some change to mapping xml in memory
            Mappings mappings = Mappings.GetMappingsFromFile(UnitUtils.GetMappingFilePath("PicklistMethodMapping.xml"));
            mappings.MappingCollection[0].MemberInformation.Args[1].PickListCode = "Units";

            string expectedPicklistItemId = "3";

            string xmlFilePath = Path.Combine(UnitUtils.GetTestFolderPath(), "PicklistXmlRepresentation.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);

            JobParameters jobParameters = new JobParameters();
            jobParameters.Mappings = mappings;
            jobParameters.SourceRecords = _reader.ExtractToRecords(new IndexRange(0, 0));
            // TODO: Use the new way to add a picklist, instead of maintaining a Picklist property in JobParameters
            //JobUtility.AddPicklist("", Picklist.NewPicklist(doc.OuterXml));

            JobResponse jobResponse = JobServiceCaller.MapRecords(jobParameters);

            List<IDestinationRecord> destinationRecordList = jobResponse.ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

            Assert.IsNotNull(destinationRecordList);
            Assert.AreEqual(1, destinationRecordList.Count);

            IDestinationRecord destinationRecord = destinationRecordList[0];

            Assert.AreEqual(expectedPicklistItemId, ((MockRegistryRecord)destinationRecord).PropertyList["Units"].Value);
        }

        [Test]
        public void Test_CanMapMethodInvocation()
        {
            string expectedMolRegNo = "2";

            JobParameters jobParameters = new JobParameters();
            jobParameters.Mappings = Mappings.GetMappingsFromFile(UnitUtils.GetMappingFilePath("MockMethodMapping.xml"));
            jobParameters.SourceRecords = _reader.ExtractToRecords(new IndexRange(0, 0));

            JobResponse jobResponse = JobServiceCaller.MapRecords(jobParameters);
            List<IDestinationRecord> destinationRecordList = jobResponse.ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

            Assert.IsNotNull(destinationRecordList);
            Assert.AreEqual(1, destinationRecordList.Count);

            IDestinationRecord destinationRecord = destinationRecordList[0];

            Assert.AreEqual(expectedMolRegNo, ((MockRegistryRecord)destinationRecord).PropertyList["MOLREGNO"].Value);
        }

        [Test]
        public void Test_CanMapDividedBindingPathMapping()
        {
            DateTime expectedDate = Convert.ToDateTime("1/1/91").ToUniversalTime();
            string expectedMolRegNo = "2";
            Mappings mappingList = Mappings.GetMappingsFromFile(@"..\..\TestFiles\MappingFiles\DividedBindingPathMapping.xml");

            JobParameters jobParameters = new JobParameters();
            jobParameters.Mappings = mappingList;
            jobParameters.SourceRecords = _reader.ExtractToRecords(new IndexRange(0, 0));

            JobResponse jobResponse = JobServiceCaller.MapRecords(jobParameters);
            List<IDestinationRecord> destinationRecordList = jobResponse.ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

            Assert.IsNotNull(destinationRecordList);
            Assert.AreEqual(1, destinationRecordList.Count);

            IDestinationRecord destinationRecord = destinationRecordList[0];

            Assert.AreEqual(expectedDate, ((MockRegistryRecord)destinationRecord).BatchList[0].DateCreated.ToUniversalTime());
            Assert.AreEqual(expectedMolRegNo, ((MockRegistryRecord)destinationRecord).PropertyList["MOLREGNO"].Value);
        }

        [Test]
        public void Test_CanMapStaticMethodInvocation()
        {
            string expectedMolNo = "2";

            JobParameters jobParameters = new JobParameters();
            jobParameters.Mappings = Mappings.GetMappingsFromFile(UnitUtils.GetMappingFilePath("MockStaticMethodMapping.xml"));
            jobParameters.SourceRecords = _reader.ExtractToRecords(new IndexRange(0, 0));

            JobResponse jobResponse = JobServiceCaller.MapRecords(jobParameters);
            List<IDestinationRecord> destinationRecordList = jobResponse.ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

            Assert.IsNotNull(destinationRecordList);
            Assert.AreEqual(1, destinationRecordList.Count);
            Assert.AreEqual(expectedMolNo, MockRegistryRecord.StaticProperty);
        }

        [Test]
        public void Test_CanMapIncludeDisabledMappingMapping()
        {
            string expectedMolName = "Benzene";
            DateTime expectedDate = Convert.ToDateTime("1/1/91").ToUniversalTime();

            JobParameters jobParameters = new JobParameters();
            jobParameters.Mappings = Mappings.GetMappingsFromFile(UnitUtils.GetMappingFilePath("IncludeDisabledMappingMapping.xml"));
            jobParameters.SourceRecords = _reader.ExtractToRecords(new IndexRange(0, 0));

            JobResponse jobResponse = JobServiceCaller.MapRecords(jobParameters);
            List<IDestinationRecord> destinationRecordList = jobResponse.ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

            Assert.IsNotNull(destinationRecordList);
            Assert.AreEqual(1, destinationRecordList.Count);

            IDestinationRecord destinationRecord = destinationRecordList[0];

            Assert.AreNotEqual(expectedMolName, ((MockRegistryRecord)destinationRecord).MolName);
            Assert.AreEqual(expectedDate, ((MockRegistryRecord)destinationRecord).BatchList[0].DateCreated.ToUniversalTime());
        }

        [Test]
        public void Test_CanMapPropertyByMockPicklistWithTabDelimiterResolver()
        {
            string fullFilePath = "../../TestFiles/DataFiles/ResolverTestFiles/Sample_With_NeedResolvePicklist.sdf";
            _sourceFileInfo = new SourceFileInfo(fullFilePath, SourceFileType.SDFile);
            _reader = FileReaderFactory.FetchReader(_sourceFileInfo);

            string expectedPicklistItemId = "3";

            string xmlFilePath = UnitUtils.GetTestFilePath("PicklistXmlRepresentation.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);

            JobParameters jobParameters = new JobParameters();
            jobParameters.Mappings = Mappings.GetMappingsFromFile(UnitUtils.GetMappingFilePath("IncludeTabDelimiterResolverMapping.xml"));
            jobParameters.SourceRecords = _reader.ExtractToRecords(new IndexRange(0, 0));
            // TODO: Use the new way to add a picklist, instead of maintaining a Picklist property in JobParameters
            //JobUtility.AddPicklist("", Picklist.NewPicklist(doc.OuterXml));

            JobResponse jobResponse = JobServiceCaller.MapRecords(jobParameters);

            List<IDestinationRecord> destinationRecordList = jobResponse.ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

            Assert.IsNotNull(destinationRecordList);
            Assert.AreEqual(1, destinationRecordList.Count);

            IDestinationRecord destinationRecord = destinationRecordList[0];

            Assert.AreEqual(expectedPicklistItemId, ((MockRegistryRecord)destinationRecord).PropertyList["Units"].Value);
        }

        [Test]
        public void Test_CanMapPropertyByMockPicklistWithPipelineDelimiterResolver()
        {
            string fullFilePath = "../../TestFiles/DataFiles/ResolverTestFiles/Sample_With_NeedResolvePicklist.sdf";
            _sourceFileInfo = new SourceFileInfo(fullFilePath, SourceFileType.SDFile);
            _reader = FileReaderFactory.FetchReader(_sourceFileInfo);

            string expectedPicklistItemId = "3";

            string xmlFilePath = UnitUtils.GetTestFilePath("PicklistXmlRepresentation.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);

            JobParameters jobParameters = new JobParameters();
            jobParameters.Mappings = Mappings.GetMappingsFromFile(UnitUtils.GetMappingFilePath("IncludePipelineDelimiterResolverMapping.xml"));
            jobParameters.SourceRecords = _reader.ExtractToRecords(new IndexRange(0, 0));
            // TODO: Use the new way to add a picklist, instead of maintaining a Picklist property in JobParameters
            //JobUtility.AddPicklist("", Picklist.NewPicklist(doc.OuterXml));

            JobResponse jobResponse = JobServiceCaller.MapRecords(jobParameters);

            List<IDestinationRecord> destinationRecordList = jobResponse.ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

            Assert.IsNotNull(destinationRecordList);
            Assert.AreEqual(1, destinationRecordList.Count);

            IDestinationRecord destinationRecord = destinationRecordList[0];

            Assert.AreEqual(expectedPicklistItemId, ((MockRegistryRecord)destinationRecord).PropertyList["Units"].Value);
        }

        [Test]
        public void Test_CanMapPropertyByMockPicklistWithCommaDelimiterResolver()
        {
            string fullFilePath = "../../TestFiles/DataFiles/ResolverTestFiles/Sample_With_NeedResolvePicklist.sdf";
            _sourceFileInfo = new SourceFileInfo(fullFilePath, SourceFileType.SDFile);
            _reader = FileReaderFactory.FetchReader(_sourceFileInfo);

            string expectedPicklistItemId = "3";

            string xmlFilePath = UnitUtils.GetTestFilePath("PicklistXmlRepresentation.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);

            JobParameters jobParameters = new JobParameters();
            jobParameters.Mappings = Mappings.GetMappingsFromFile(UnitUtils.GetMappingFilePath("IncludeCommaDelimiterResolverMapping.xml"));
            jobParameters.SourceRecords = _reader.ExtractToRecords(new IndexRange(0, 0));
            // TODO: Use the new way to add a picklist, instead of maintaining a Picklist property in JobParameters
            //JobUtility.AddPicklist("", Picklist.NewPicklist(doc.OuterXml));

            JobResponse jobResponse = JobServiceCaller.MapRecords(jobParameters);

            List<IDestinationRecord> destinationRecordList = jobResponse.ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

            Assert.IsNotNull(destinationRecordList);
            Assert.AreEqual(1, destinationRecordList.Count);

            IDestinationRecord destinationRecord = destinationRecordList[0];

            Assert.AreEqual(expectedPicklistItemId, ((MockRegistryRecord)destinationRecord).PropertyList["Units"].Value);
        }

        #endregion

        #region Map to RegistryRecord

        [Test]
        [Category(UnitUtils.DATABASE_DEPENDENCY)]
        public void Test_CanMapToRegistryRecord()
        {
            Initialize(UnitUtils.GetDataFilePath("fragments_example.sdf"), true);
            

            JobParameters jobParameters = new JobParameters();
            jobParameters.Mappings = Mappings.GetMappingsFromFile(UnitUtils.GetMappingFilePath(@"RegistryRecord\fragments_example.maps.xml"));
            jobParameters.ActionRanges = new IndexRanges();
            jobParameters.ActionRanges.Add(0, new IndexRange(0, 0));
            jobParameters.FileReader = _reader;
            JobResponse jobResponse = JobServiceCaller.ExtractRecords(jobParameters);
            jobParameters.SourceRecords = jobResponse.ResponseContext[RecordsExtractionService.SOURCERECORDS] as List<ISourceRecord>;

            jobResponse = JobServiceCaller.MapRecords(jobParameters);
            List<IDestinationRecord> destinationRecordList = jobResponse.ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

            Assert.IsNotNull(destinationRecordList);
            Assert.AreEqual(1, destinationRecordList.Count);

            IDestinationRecord destinationRecord = destinationRecordList[0];
            Assert.AreEqual("2441", ((RegistryRecord)destinationRecord).ComponentList[0].Compound.PropertyList["CMP_COMMENTS"].Value);
            Assert.AreEqual(@"Sodium hydroxymethylsulfonate sd example_sd chemdig rzepa
example.sd  NOtclserve10289911332 0   0.00000     0.00000NCI NS
 
 10  9  0  0  0  0  0  0  0  0  1 V2000
    3.7321    0.0000    0.0000 S   0  0  0  0  0  0  0  0  0  0  0  0
    4.5981    0.5000    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    3.2321    0.8660    0.0000 O   0  0  0  0  0  0  0  0  0  0  0  0
    4.2320   -0.8660    0.0000 O   0  0  0  0  0  0  0  0  0  0  0  0
    2.8660   -0.5000    0.0000 O   0  0  0  0  0  0  0  0  0  0  0  0
    5.4641   -0.0000    0.0000 O   0  0  0  0  0  0  0  0  0  0  0  0
    4.9966    0.9749    0.0000 H   0  0  0  0  0  0  0  0  0  0  0  0
    4.1995    0.9749    0.0000 H   0  0  0  0  0  0  0  0  0  0  0  0
    2.3291   -0.1900    0.0000 H   0  0  0  0  0  0  0  0  0  0  0  0
    6.0010    0.3100    0.0000 H   0  0  0  0  0  0  0  0  0  0  0  0
  1  2  1  0  0  0  0
  1  3  2  0  0  0  0
  1  4  2  0  0  0  0
  1  5  1  0  0  0  0
  2  6  1  0  0  0  0
  2  7  1  0  0  0  0
  2  8  1  0  0  0  0
  5  9  1  0  0  0  0
  6 10  1  0  0  0  0
M  END
", ((RegistryRecord)destinationRecord).ComponentList[0].Compound.BaseFragment.Structure.Value);
            //mapped RegistryRecord originally has a BatchComponentFragment
            Assert.IsTrue(0.05f == ((RegistryRecord)destinationRecord).BatchList[0].BatchComponentList[0].BatchComponentFragmentList[0].Equivalents);
        }

        private Mappings ConstructMapping_StructurePropertyList()
        {
            Mappings mappings = new Mappings();

            Mappings.Mapping mapping = new Mappings.Mapping();
            mapping.ObjectBindingPath =
                "this.ComponentList[0].Compound.BaseFragment.Structure.PropertyList['STRUCT_COMMENTS'].Value";
            mapping.MemberInformation.MemberType = Mappings.MemberTypeEnum.Property;
            mapping.MemberInformation.Type = Mappings.TypeEnum.Instance;

            Mappings.Arg arg = new Mappings.Arg();
            arg.Index = 0;
            arg.Input = Mappings.InputEnum.Derived;
            arg.Type = "string";
            arg.Value = "STRUCT_COMMENTS";

            mapping.MemberInformation.Args.Add(arg);
            mappings.MappingCollection.Add(mapping);

            return mappings;
        }

        [Test]
        [Category(UnitUtils.DATABASE_DEPENDENCY)]
        public void Test_CanMapToStructurePropertyList()
        {
            Initialize(@"SD\SAMPLE.sdf",true);

            JobParameters jobParameters = new JobParameters();
            jobParameters.Mappings = ConstructMapping_StructurePropertyList();
            jobParameters.SourceRecords = _reader.ExtractToRecords(new IndexRange(0, 0));

            JobResponse jobResponse = JobServiceCaller.MapRecords(jobParameters);
            List<IDestinationRecord> destinationRecordList = jobResponse.ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

            Assert.IsNotNull(destinationRecordList);
            Assert.AreEqual(1, destinationRecordList.Count);

            RegistryRecord destinationRecord = destinationRecordList[0] as RegistryRecord;

            Assert.AreEqual("sample comments", destinationRecord.ComponentList[0].Compound.BaseFragment.Structure.PropertyList["STRUCT_COMMENTS"].Value);
        }

        private Mappings ConstructMapping_StructureIndentifierList()
        {
            Mappings mappings = new Mappings();

            Mappings.Mapping mapping = new Mappings.Mapping();
            mapping.ObjectBindingPath = "this.ComponentList[0].Compound.BaseFragment.Structure";
            mapping.MemberInformation.MemberType = Mappings.MemberTypeEnum.Method;
            mapping.MemberInformation.Name = "AddIdentifier";
            mapping.MemberInformation.Type = Mappings.TypeEnum.Instance;

            Mappings.Arg arg_1 = new Mappings.Arg();
            arg_1.Index = 0;
            arg_1.Input = Mappings.InputEnum.Constant;
            arg_1.Type = "string";
            arg_1.Value = "LegacyID";

            Mappings.Arg arg_2 = new Mappings.Arg();
            arg_2.Index = 1;
            arg_2.Input = Mappings.InputEnum.Derived;
            arg_2.Type = "string";
            arg_2.Value = "LEGACY_ID";

            mapping.MemberInformation.Args.Add(arg_1);
            mapping.MemberInformation.Args.Add(arg_2);
            mappings.MappingCollection.Add(mapping);

            return mappings;
        }

        [Test]
        [Category(UnitUtils.DATABASE_DEPENDENCY)]
        public void Test_CanMapToStructureIndentifierList()
        {
            Initialize(@"SD\SAMPLE.sdf", true);

            JobParameters jobParameters = new JobParameters();
            jobParameters.Mappings = ConstructMapping_StructureIndentifierList();
            jobParameters.SourceRecords = _reader.ExtractToRecords(new IndexRange(0, 0));

            JobResponse jobResponse = JobServiceCaller.MapRecords(jobParameters);
            List<IDestinationRecord> destinationRecordList = jobResponse.ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

            Assert.IsNotNull(destinationRecordList);
            Assert.AreEqual(1, destinationRecordList.Count);

            RegistryRecord destinationRecord = destinationRecordList[0] as RegistryRecord;

            Assert.AreEqual("lid",destinationRecord.ComponentList[0].Compound.BaseFragment.Structure.IdentifierList[0].InputText);
        }

        #endregion

        #region Mappings Serialization and Deserialization Test

        [Test]
        public void Test_CanMappingsSaveToFile()
        {
            XmlDocument doc = new XmlDocument();

            Mappings mappings = new Mappings();
            Mappings.Arg arg = new Mappings.Arg();
            arg.Type = "testType";
            Mappings.MemberInformation memberInformation = new Mappings.MemberInformation();
            memberInformation.Args.Add(arg);
            Mappings.Mapping mapping = new Mappings.Mapping();
            mapping.MemberInformation = memberInformation;
            mappings.MappingCollection.Add(mapping);

            string compareStrFormMappings = mappings.MappingCollection[0].MemberInformation.Args[0].Type;

            string filePath = UnitUtils.GetTestFolderPath();
            string fileName = "Mappings_SaveToFile_Test.xml";
            string fullFilePath = Path.Combine(filePath, fileName);

            mappings.SaveToFile(fullFilePath);

            doc.Load(fullFilePath);
            string XPath = "/mappings/mapping[1]/memberInformation/args/arg[1]/@type";

            string compareStrFormDom = doc.SelectSingleNode(XPath).Value;

            Assert.AreEqual(compareStrFormDom, compareStrFormMappings);
        }

        /// <summary>
        /// Test case: Construct a Mappings object in memory, with at least 1 Mapping object,
        ///            all of whose properties should have values. Also, the Mapping object should contain
        ///            at least Argument with all properties having values. 
        /// Expected result: All properties should be serialized. Also, there should be 
        ///                  no xml declaration line in the first line and no any attributes
        ///                  for the <mappings> root node.
        /// </summary>
        [Test]
        public void Test_CanSerializeFullElementsMappings()
        {
            Mappings mappings = new Mappings();
            Mappings.Mapping mapping = new Mappings.Mapping();
            mapping.ObjectBindingPath = "testBindingPath";
            mappings.MappingCollection.Add(mapping);
            mappings.MappingCollection[0].MemberInformation.Name = "testName";
            mappings.MappingCollection[0].MemberInformation.Description = "testDescription";
            Mappings.Resolver resolver = new Mappings.Resolver();
            resolver.File = "fullFilePath";
            resolver.ExternalValueColumn = "c1";
            resolver.InternalValueColumn = "c2";
            Mappings.Arg arg = new Mappings.Arg();
            arg.Value = "testValue";
            arg.Resolver = resolver;
            arg.PickListCode = "1";
            mappings.MappingCollection[0].MemberInformation.Args.Add(arg);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(mappings.ToString());

            string xpathOne = "/mappings/destinationRecordType/text()";
            string xpathTwo = "/mappings/mapping[1]/enabled/text()";
            string xpathThree = "/mappings/mapping[1]/objectBindingPath/text()";
            string xpathFour = "/mappings/mapping[1]/memberInformation/@memberType";
            string xpathFive = "/mappings/mapping[1]/memberInformation/type/text()";
            string xpathSix = "/mappings/mapping[1]/memberInformation/name/text()";
            string xpathSeven = "/mappings/mapping[1]/memberInformation/description/text()";
            string xpathEight = "/mappings/mapping[1]/memberInformation/args/arg[1]/@index";
            string xpathNine = "/mappings/mapping[1]/memberInformation/args/arg[1]/@input";
            string xpathTen = "/mappings/mapping[1]/memberInformation/args/arg[1]/@type";
            string xpathEleven = "/mappings/mapping[1]/memberInformation/args/arg[1]/@pickListCode";
            string xpathTwelve = "/mappings/mapping[1]/memberInformation/args/arg[1]/value/text()";
            string xpathThirteen = "/mappings/mapping[1]/memberInformation/args/arg[1]/resolver/file/text()";
            string xpathFourteen = "/mappings/mapping[1]/memberInformation/args/arg[1]/resolver/delimiter/text()";
            string xpathFifteen = "/mappings/mapping[1]/memberInformation/args/arg[1]/resolver/externalValueColumn/text()";
            string xpathSixteen = "/mappings/mapping[1]/memberInformation/args/arg[1]/resolver/internalValueColumn/text()";

            string compareStrFromXPathOne = doc.SelectSingleNode(xpathOne).Value;
            string compareStrFromXPathTwo = doc.SelectSingleNode(xpathTwo).Value;
            string compareStrFromXPathThree = doc.SelectSingleNode(xpathThree).Value;
            string compareStrFromXPathFour = doc.SelectSingleNode(xpathFour).Value;
            string compareStrFromXPathFive = doc.SelectSingleNode(xpathFive).Value;
            string compareStrFromXPathSix = doc.SelectSingleNode(xpathSix).Value;
            string compareStrFromXPathSeven = doc.SelectSingleNode(xpathSeven).Value;
            string compareStrFromXPathEight = doc.SelectSingleNode(xpathEight).Value;
            string compareStrFromXPathNine = doc.SelectSingleNode(xpathNine).Value;
            string compareStrFromXPathTen = doc.SelectSingleNode(xpathTen).Value;
            string compareStrFromXPathEleven = doc.SelectSingleNode(xpathEleven).Value;
            string compareStrFromXPathTwelve = doc.SelectSingleNode(xpathTwelve).Value;
            string compareStrFromXPathThirteen = doc.SelectSingleNode(xpathThirteen).Value;
            string compareStrFromXPathFourteen = doc.SelectSingleNode(xpathFourteen).Value;
            string compareStrFromXPathFifteen = doc.SelectSingleNode(xpathFifteen).Value;
            string compareStrFromXPathSixteen = doc.SelectSingleNode(xpathSixteen).Value;

            string compareStrFromMappingsOne = mappings.DestinationRecordType.ToString();
            string compareStrFromMappingsTwo = mappings.MappingCollection[0].Enabled.ToString().ToLower();
            string compareStrFromMappingsThree = mappings.MappingCollection[0].ObjectBindingPath;
            string compareStrFromMappingsFour = mappings.MappingCollection[0].MemberInformation.MemberType.ToString().ToLower();
            string compareStrFromMappingsFive = mappings.MappingCollection[0].MemberInformation.Type.ToString().ToLower();
            string compareStrFromMappingsSix = mappings.MappingCollection[0].MemberInformation.Name;
            string compareStrFromMappingsSeven = mappings.MappingCollection[0].MemberInformation.Description;
            string compareStrFromMappingsEight = mappings.MappingCollection[0].MemberInformation.Args[0].Index.ToString();
            string compareStrFromMappingsNine = mappings.MappingCollection[0].MemberInformation.Args[0].Input.ToString().ToLower();
            string compareStrFromMappingsTen = mappings.MappingCollection[0].MemberInformation.Args[0].Type;
            string compareStrFromMappingsEleven = mappings.MappingCollection[0].MemberInformation.Args[0].PickListCode;
            string compareStrFromMappingsTwelve = mappings.MappingCollection[0].MemberInformation.Args[0].Value;
            string compareStrFromMappingsThirteen = mappings.MappingCollection[0].MemberInformation.Args[0].Resolver.File;
            string compareStrFromMappingsFourteen = mappings.MappingCollection[0].MemberInformation.Args[0].Resolver.Delimiter.ToString().ToLower();
            string compareStrFromMappingsFifteen = mappings.MappingCollection[0].MemberInformation.Args[0].Resolver.ExternalValueColumn;
            string compareStrFromMappingsSixteen = mappings.MappingCollection[0].MemberInformation.Args[0].Resolver.InternalValueColumn;

            Assert.AreEqual(compareStrFromXPathOne, compareStrFromMappingsOne);
            Assert.AreEqual(compareStrFromXPathTwo, compareStrFromMappingsTwo);
            Assert.AreEqual(compareStrFromXPathThree, compareStrFromMappingsThree);
            Assert.AreEqual(compareStrFromXPathFour, compareStrFromMappingsFour);
            Assert.AreEqual(compareStrFromXPathFive, compareStrFromMappingsFive);
            Assert.AreEqual(compareStrFromXPathSix, compareStrFromMappingsSix);
            Assert.AreEqual(compareStrFromXPathSeven, compareStrFromMappingsSeven);
            Assert.AreEqual(compareStrFromXPathEight, compareStrFromMappingsEight);
            Assert.AreEqual(compareStrFromXPathNine, compareStrFromMappingsNine);
            Assert.AreEqual(compareStrFromXPathTen, compareStrFromMappingsTen);
            Assert.AreEqual(compareStrFromXPathEleven, compareStrFromMappingsEleven);
            Assert.AreEqual(compareStrFromXPathTwelve, compareStrFromMappingsTwelve);
            Assert.AreEqual(compareStrFromXPathThirteen, compareStrFromMappingsThirteen);
            Assert.AreEqual(compareStrFromXPathFourteen, compareStrFromMappingsFourteen);
            Assert.AreEqual(compareStrFromXPathFifteen, compareStrFromMappingsFifteen);
            Assert.AreEqual(compareStrFromXPathSixteen, compareStrFromMappingsSixteen);

        }

        [Test]
        public void Test_CanDeserializeFullElementsMappings()
        {
            Mappings mappings = Mappings.GetMappingsFromFile(@"..\..\TestFiles\MappingFiles\FullElementsMapping.xml");

            List<string> compareStrFromMappingXml =
                new List<string>(new string[] {
                    "testObjectBindingPath","method","instance",
                    "testName" ,"testDescription","0",
                "constant","testType","0",
                "testValue","false","MockRegistryRecord",
                "fullFilePath","tab","c1","c2"});

            List<string> compareStrFromMappings = new List<string>();

            string mappingsStrOne = mappings.MappingCollection[0].ObjectBindingPath;
            compareStrFromMappings.Add(mappingsStrOne);
            string mappingsStrTwo = mappings.MappingCollection[0].MemberInformation.MemberType.ToString().ToLower();
            compareStrFromMappings.Add(mappingsStrTwo);
            string mappingsStrThree = mappings.MappingCollection[0].MemberInformation.Type.ToString().ToLower();
            compareStrFromMappings.Add(mappingsStrThree);
            string mappingsStrFour = mappings.MappingCollection[0].MemberInformation.Name;
            compareStrFromMappings.Add(mappingsStrFour);
            string mappingsStrFive = mappings.MappingCollection[0].MemberInformation.Description;
            compareStrFromMappings.Add(mappingsStrFive);
            string mappingsStrSix = mappings.MappingCollection[0].MemberInformation.Args[0].Index.ToString();
            compareStrFromMappings.Add(mappingsStrSix);
            string mappingsStrSeven = mappings.MappingCollection[0].MemberInformation.Args[0].Input.ToString().ToLower();
            compareStrFromMappings.Add(mappingsStrSeven);
            string mappingsStrEight = mappings.MappingCollection[0].MemberInformation.Args[0].Type;
            compareStrFromMappings.Add(mappingsStrEight);
            string mappingsStrNine = mappings.MappingCollection[0].MemberInformation.Args[0].PickListCode;
            compareStrFromMappings.Add(mappingsStrNine);
            string mappingsStrTen = mappings.MappingCollection[0].MemberInformation.Args[0].Value;
            compareStrFromMappings.Add(mappingsStrTen);
            string mappingsStrEleven = mappings.MappingCollection[0].Enabled.ToString().ToLower();
            compareStrFromMappings.Add(mappingsStrEleven);
            string mappingsStrTwelve = mappings.DestinationRecordType.ToString();
            compareStrFromMappings.Add(mappingsStrTwelve);
            string mappingsStrThirteen = mappings.MappingCollection[0].MemberInformation.Args[0].Resolver.File;
            compareStrFromMappings.Add(mappingsStrThirteen);
            string mappingsStrFourteen = mappings.MappingCollection[0].MemberInformation.Args[0].Resolver.Delimiter.ToString().ToLower();
            compareStrFromMappings.Add(mappingsStrFourteen);
            string mappingsStrFifteen = mappings.MappingCollection[0].MemberInformation.Args[0].Resolver.ExternalValueColumn;
            compareStrFromMappings.Add(mappingsStrFifteen);
            string mappingsStrSixteen = mappings.MappingCollection[0].MemberInformation.Args[0].Resolver.InternalValueColumn;
            compareStrFromMappings.Add(mappingsStrSixteen);

            for (int i = 0; i < compareStrFromMappings.Count; i++)
            {
                Assert.AreEqual(compareStrFromMappings[i], compareStrFromMappingXml[i]);
            }
        }

        /// <summary>
        /// Tese case: Manually create an xml file and store it in TestFiles folder. Deserialize it, 
        ///            make some changes to the Mappings object and then serialize it back to the xml file.
        /// Expected result: The resultant xml should reflect the changes you made by operating the objects.
        /// </summary>
        [Test]
        public void Test_CanMappingsSaveChanges()
        {
            Mappings mappings = Mappings.GetMappingsFromFile(@"..\..\TestFiles\MappingFiles\ValueToChangeMapping.xml");
            mappings.MappingCollection[0].MemberInformation.Args[0].Value = "NewTestValue";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(mappings.ToString());
            string XPath = "/mappings/mapping[1]/memberInformation/args/arg[1]/value/text()";
            string compareStrFromXPath = doc.SelectSingleNode(XPath).Value;
            Assert.AreEqual("NewTestValue", compareStrFromXPath);
        }

        #endregion

        private void Initialize(string sourceFileName)
        {
            string fullFilePath = UnitUtils.GetDataFilePath(sourceFileName);

            //Leverage the factory
            _sourceFileInfo = new SourceFileInfo(fullFilePath, SourceFileType.SDFile);
            _reader = FileReaderFactory.FetchReader(_sourceFileInfo);

            Assert.IsNotNull(_reader, "Failed to construct SDFileReader.");
        }

        private void Initialize(string sourceFileName, bool doLogin)
        {
            Initialize(sourceFileName);

            if (doLogin)
            {
                UnitUtils.AuthenticateCoeUser("T5_85", "T5_85");
            }
        }

        private void SetSourceFieldTypes(string key, Type type)
        {
            if (!SourceFieldTypes.TypeDefinitions.ContainsKey(key))
                SourceFieldTypes.TypeDefinitions.Add(key, type);
            else
                SourceFieldTypes.TypeDefinitions[key] = type;
        }
    }
}
