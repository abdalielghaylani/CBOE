using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using CambridgeSoft.COE.UnitTests.DataLoader;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.DataMapping;
using CambridgeSoft.COE.UnitTests.DataLoader.Core.Utility;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using CambridgeSoft.COE.Framework.Services;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.DataLoader.Core.FileParser;

//TODO: Provide both positive and negative test-cases to ensure the ASP.NET application use-case
//      functionality has not been broken. (The caller passes the ID values directly.)

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.BugFix
{
    /// <summary>
    /// The picklist-based conversion capabilities had NOT been coded, so when the user, for
    /// example, maps "T5_85" to the SCIENTIST_ID component property, the value will be converted
    /// internal to the Property object to (person ID =) "15", the expected value for the database.
    /// </summary>
    /// <remarks>
    /// This functionality uses case-insensitive matching ("CSSADMIN" is equivalent to "cSsAdMiN").
    /// </remarks>
    [TestFixture]
    public class CSBR_134490
    {
        IFileReader _reader = null;
        SourceFileInfo _sourceFileInfo = null;

        /// <summary>
        /// Loads a local test file into memory for testing.
        /// </summary>
        [SetUp()]
        public void Initialize()
        {
            string sourceFileName = "Sample_With_Picklist.sdf";
            string fullFilePath = UnitUtils.GetDataFilePath(sourceFileName);

            //log the user in becuase authentication is required for this test
            UnitUtils.AuthenticateCoeUser("T5_85", "T5_85");

            //Leverage the factory
            _sourceFileInfo = new SourceFileInfo(fullFilePath, SourceFileType.SDFile);
            _reader = FileReaderFactory.FetchReader(_sourceFileInfo);

            Assert.IsNotNull(_reader, "Failed to construct SDFileReader.");
        }

        /// <summary>
        /// Creates a mapping to a user name as SCIENTIST_ID, which is configured by default to
        /// use the 'Scientists' picklist. The expected result is that the Property object will
        /// first convert the text into the corresponding picklist-item's ID, then validate the
        /// ID value to ensure it is valid (ID value of -1 means not found).
        /// </summary>
        [Test]
        [Category(UnitUtils.DATABASE_DEPENDENCY)]
        public void CSBR134490_FixTest()
        {
            string expectedCorrespondingId = "2";

            //create a mapping xml
            Mappings mappings = new Mappings();
            Mappings.Mapping mapping = MappingUtils.CreateMappingWithMemberInfo(
                "this.BatchList[0].PropertyList['SCIENTIST_ID'].Value",
                null,
                Mappings.MemberTypeEnum.Property,
                null,
                Mappings.TypeEnum.Instance);
            Mappings.Arg arg = MappingUtils.CreateMemberInfoArgument(
                0, Mappings.InputEnum.Derived, null, "string", "Chemist"
                );
            mapping.MemberInformation.Args.Add(arg);
            mappings.MappingCollection.Add(mapping);

            JobParameters jobParameters = new JobParameters();
            jobParameters.Mappings = mappings;
            jobParameters.ActionRanges = new IndexRanges();
            jobParameters.ActionRanges.Add(0, new IndexRange(0, 0));
            jobParameters.FileReader = _reader;
            JobResponse jobResponse = JobServiceCaller.ExtractRecords(jobParameters);
            jobParameters.SourceRecords = jobResponse.ResponseContext[RecordsExtractionService.SOURCERECORDS] as List<ISourceRecord>;

            jobResponse = JobServiceCaller.MapRecords(jobParameters);
            List<IDestinationRecord> destinationRecordList = jobResponse.ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

            Assert.IsNotNull(destinationRecordList);
            Assert.AreEqual(1, destinationRecordList.Count);

            RegistryRecord destinationRecord = destinationRecordList[0] as RegistryRecord;

            Assert.AreEqual(expectedCorrespondingId, destinationRecord.BatchList[0].PropertyList["SCIENTIST_ID"].Value);
        }

        /// <summary>
        /// Closes the file reader and cleans up the source field definitions cache.
        /// </summary>
        [TearDown()]
        public void Clean()
        {
            _sourceFileInfo = null;

            if (_reader != null)
                _reader.Close();
            if (SourceFieldTypes.TypeDefinitions != null)
                SourceFieldTypes.TypeDefinitions.Clear();
        }

    }
}
