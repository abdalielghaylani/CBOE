using System;
using System.Collections.Generic;
using System.Text;
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
using CambridgeSoft.COE.UnitTests.DataLoader.Core.Utility;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.DataMapping
{
    [TestFixture]
    class NormalizeJobParameters_UnitTest
    {
        const int SD_RECORD_COUNT = 36;

        [SetUp]
        public void Initialize()
        {
            //there are no setup requirements at this time
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void Test_NormalizeDerivedArgValues()
        {
            JobParameters jobParameters = new JobParameters();
            string expectedValue = "InvalidValue";
            string pathToInputFile = System.IO.Path.Combine(UnitUtils.GetDataFolderPath(), @"SD\SAMPLE.sdf");

            Mappings mappings = new Mappings();
            Mappings.Mapping mapping = MappingUtils.CreateMappingWithMemberInfo("this", null, Mappings.MemberTypeEnum.Property, null, Mappings.TypeEnum.Instance);
            Mappings.Arg arg = MappingUtils.CreateMemberInfoArgument(0, Mappings.InputEnum.Derived, null, "string", expectedValue);
            mapping.MemberInformation.Args.Add(arg);
            mappings.MappingCollection.Add(mapping);

            jobParameters.Mappings = mappings;
            jobParameters.DataSourceInformation = new SourceFileInfo(pathToInputFile, SourceFileType.SDFile);
            jobParameters.ActionRanges = new IndexList(new int[] { 1, 2, 3 }).ToIndexRanges();
            jobParameters.TargetActionType = TargetActionType.ValidateMapping;

            JobResponse response = JobServiceCaller.NormalizeJobParameters(jobParameters);
            Dictionary<string, object> allResultsDic = response.ResponseContext[NormalizeJobParametersService.DIC_ALL_RESULTS] as Dictionary<string, object>;

            List<string> invalidDerivedArgList = allResultsDic[NormalizeJobParametersService.INVALID_DERIVED_ARG_VALUE] as List<string>;

            Assert.AreEqual(expectedValue, invalidDerivedArgList[0]);
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void Test_ValidateRangeBeginValue()
        {
            JobParameters jobParameters = new JobParameters();
            string pathToInputFile = System.IO.Path.Combine(UnitUtils.GetDataFolderPath(), @"SD\SAMPLE.sdf");

            jobParameters.DataSourceInformation = new SourceFileInfo(pathToInputFile, SourceFileType.SDFile);
            jobParameters.ActionRanges = new IndexList(new int[] { 36 }).ToIndexRanges();

            IFileReader reader = FileReaderFactory.FetchReader(jobParameters.DataSourceInformation);
            Assert.AreEqual(SD_RECORD_COUNT, reader.CountAll());

            JobResponse response = JobServiceCaller.NormalizeJobParameters(jobParameters);
            Dictionary<string, object> allResultsDic = response.ResponseContext[NormalizeJobParametersService.DIC_ALL_RESULTS] as Dictionary<string, object>;

            int rangeBeginValidateResult = (int)allResultsDic[NormalizeJobParametersService.INVALID_RANGE_BEGIN];

            Assert.AreEqual(SD_RECORD_COUNT, rangeBeginValidateResult);
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void Test_NormalizePickListCodes()
        {
            JobParameters jobParameters = new JobParameters();
            string expectedPicklistCode = "4";
            string pathToInputFile = System.IO.Path.Combine(UnitUtils.GetDataFolderPath(), @"SD\SAMPLE.sdf");

            Mappings mappings = new Mappings();
            mappings.MappingCollection.Add(MappingUtils.NewRegistryPropertyMapping("REG_COMMENTS", "reg_comment_field", "4"));
            mappings.MappingCollection.Add(MappingUtils.NewComponentPropertyMapping(0, "CMP_COMMENTS", "comp_comment_field", "Scientists"));

            jobParameters.Mappings = mappings;
            jobParameters.DataSourceInformation = new SourceFileInfo(pathToInputFile, SourceFileType.SDFile);
            jobParameters.ActionRanges = new IndexList(new int[] { 1, 2, 3 }).ToIndexRanges();
            jobParameters.TargetActionType = TargetActionType.ValidateMapping;

            JobResponse response = JobServiceCaller.NormalizeJobParameters(jobParameters);
            Dictionary<string, object> allResultsDic = response.ResponseContext[NormalizeJobParametersService.DIC_ALL_RESULTS] as Dictionary<string, object>;

            List<string> invalidPicklistCodeList = allResultsDic[NormalizeJobParametersService.INVALID_PICKLIST_CODE] as List<string>;

            Assert.AreEqual(1, invalidPicklistCodeList.Count);
            Assert.AreEqual(expectedPicklistCode, invalidPicklistCodeList[0]);
        }

        /// <summary>
        /// This test ensures that, when an MSExcel or MSAccess reader is initialized with an
        /// invalid worksheet or table name, the value of that table or worksheet name will be
        /// returned in the 'invalid table name' response parameter.
        /// </summary>
        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void Test_NormalizeTableName()
        {
            JobParameters jobParameters = new JobParameters();
            string expectedTableName = "BadWorksheetName";
            string pathToInputFile = UnitUtils.GetDataFilePath(@"excel\moltable.xlsx");

            SourceFileInfo sourceInfo = new SourceFileInfo(pathToInputFile, SourceFileType.MSExcel);
            sourceInfo.TableName = expectedTableName;
            sourceInfo.FileType = SourceFileType.MSExcel;
            sourceInfo.HasHeaderRow = true;

            jobParameters.DataSourceInformation = sourceInfo;

            JobResponse response = JobServiceCaller.NormalizeJobParameters(jobParameters);
            Dictionary<string, object> allResultsDic = response.ResponseContext[NormalizeJobParametersService.DIC_ALL_RESULTS] as Dictionary<string, object>;
            string invalidTableName = allResultsDic[NormalizeJobParametersService.INVALID_TABLE_NAME] as string;

            Assert.AreEqual(expectedTableName, invalidTableName);
        }

        /// <summary>
        /// Ensure shared data is cleared out between these Test executions.
        /// </summary>
        [TearDown()]
        public void Clean()
        {
            if (SourceFieldTypes.TypeDefinitions != null)
                SourceFieldTypes.TypeDefinitions.Clear();
        }
    }
}
