//using System;
//using System.Collections.Generic;
//using System.Text;
//using NUnit.Framework;

//using CambridgeSoft.COE.UnitTests.DataLoader;
//using CambridgeSoft.COE.DataLoader.Core.Contracts;
//using CambridgeSoft.COE.DataLoader.Core;
//using CambridgeSoft.COE.DataLoader.Core.DataMapping;
//using CambridgeSoft.COE.UnitTests.DataLoader.Core.Utility;
//using CambridgeSoft.COE.DataLoader.Core.Workflow;
//using CambridgeSoft.COE.Framework.Services;
//using CambridgeSoft.COE.Registration.Services.Types;
//using CambridgeSoft.COE.DataLoader.Core.FileParser;

//Jack should eliminate this class after comparing with CSBR-134490.cs

//namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.BugFix
//{
//    public class BugFix_UnitTest
//    {
//        IFileReader _reader = null;
//        SourceFileInfo _sourceFileInfo = null;

//        private void Initialize(string sourceFileName)
//        {
//            string fullFilePath = UnitUtils.GetDataFilePath(sourceFileName);

//            //Leverage the factory
//            _sourceFileInfo = new SourceFileInfo(fullFilePath, SourceFileType.SDFile);
//            _reader = FileReaderFactory.FetchReader(_sourceFileInfo);

//            Assert.IsNotNull(_reader, "Failed to construct SDFileReader.");
//        }

//        private void Initialize(string sourceFileName, bool doLogin)
//        {
//            Initialize(sourceFileName);

//            if (doLogin)
//            {
//                UnitUtils.AuthenticateCoeUser("T5_85", "T5_85");
//            }
//        }

//        public void CSBR134490_FixTest()
//        {
//            Initialize("Sample_With_Picklist.sdf", true);

//            string expectedCorrespondingId = "2";

//            //create a mapping xml
//            Mappings mappings = new Mappings();
//            Mappings.Mapping mapping = MappingUtils.CreateMappingWithMemberInfo(
//                "this.BatchList[0].PropertyList['SCIENTIST_ID'].Value",
//                null,
//                Mappings.MemberTypeEnum.Property,
//                null,
//                Mappings.TypeEnum.Instance);
//            Mappings.Arg arg = MappingUtils.CreateMemberInfoArgument(
//                0, Mappings.InputEnum.Derived, null, "string", "Chemist"
//                );
//            mapping.MemberInformation.Args.Add(arg);
//            mappings.MappingCollection.Add(mapping);

//            JobParameters jobParameters = new JobParameters();
//            jobParameters.Mappings = mappings;
//            jobParameters.ActionRanges = new IndexRanges();
//            jobParameters.ActionRanges.Add(0, new IndexRange(0, 0));
//            jobParameters.FileReader = _reader;
//            JobResponse jobResponse = JobServiceCaller.ExtractRecords(jobParameters);
//            jobParameters.SourceRecords = jobResponse.ResponseContext[RecordsExtractionService.SOURCERECORDS] as List<ISourceRecord>;

//            jobResponse = JobServiceCaller.MapRecords(jobParameters);
//            List<IDestinationRecord> destinationRecordList = jobResponse.ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

//            Assert.IsNotNull(destinationRecordList);
//            Assert.AreEqual(1, destinationRecordList.Count);

//            RegistryRecord destinationRecord = destinationRecordList[0] as RegistryRecord;

//            Assert.AreEqual(expectedCorrespondingId, destinationRecord.BatchList[0].PropertyList["SCIENTIST_ID"].Value);
//        }
//    }
//}
