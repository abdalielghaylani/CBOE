using NUnit.Framework;

using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.FileParser;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using CambridgeSoft.COE.Framework.Services;
using CambridgeSoft.COE.DataLoader.Core.DataMapping;
using System.Collections.Generic;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.UnitTests.DataLoader.Core.Utility;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.DataMapping
{
    [TestFixture]
    public class RegistryRecordMapping_UnitTest
    {
        IFileReader _reader = null;
        SourceFileInfo _sourceFileInfo = null;

        [Test]
        [Category(UnitUtils.DATABASE_DEPENDENCY)]
        public void AddProjectMethodWorks()
        {
            Initialize(UnitUtils.GetDataFilePath(@"SD\addproject.sdf"), true);

            JobParameters jobParameters = new JobParameters();
            jobParameters.DataSourceInformation = _sourceFileInfo;
            jobParameters.Mappings = ConstructMapping_AddProject();
            jobParameters.SourceRecords = _reader.ExtractToRecords(new IndexRange(0, 0));

            JobResponse jobResponse = JobServiceCaller.MapRecords(jobParameters);
            List<IDestinationRecord> destinationRecordList = jobResponse.ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

            Assert.IsNotNull(destinationRecordList);
            Assert.AreEqual(1, destinationRecordList.Count);

            RegistryRecord registryRecord = destinationRecordList[0] as RegistryRecord;

            Assert.IsNotNull(registryRecord);
            Assert.AreEqual(1, registryRecord.ProjectList.Count);
            Assert.AreEqual("PR100", registryRecord.ProjectList[0].Name);
        }

        /// <summary>
        /// Verify the ability to add a fragment to a registry record component when provided
        /// with a valid fragment code.
        /// </summary>
        [Test]
        [Category(UnitUtils.DATABASE_DEPENDENCY)]
        public void AddFragmentMethod_WorksWithValidFragmentCode()
        {
            Initialize(UnitUtils.GetDataFilePath(@"SD\addproject.sdf"), true);

            string code = "26";
            string eq = "2.5";

            JobParameters jobParameters = new JobParameters();
            jobParameters.DataSourceInformation = _sourceFileInfo;
            jobParameters.Mappings = ConstructMapping_AddFragment(code, eq);
            jobParameters.SourceRecords = _reader.ExtractToRecords(new IndexRange(0, 0));

            JobResponse jobResponse = JobServiceCaller.MapRecords(jobParameters);
            List<IDestinationRecord> destinationRecordList = jobResponse.ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

            Assert.IsNotNull(destinationRecordList);
            Assert.AreEqual(1, destinationRecordList.Count);

            RegistryRecord registryRecord = destinationRecordList[0] as RegistryRecord;

            Assert.IsNotNull(registryRecord);

            //the BatchComponent must be filled correctly
            BatchComponentFragmentList bcf = registryRecord.BatchList[0].BatchComponentList[0].BatchComponentFragmentList;
            Assert.AreEqual(1, bcf.Count);
            Assert.AreEqual(code, bcf[0].FragmentID.ToString());
            Assert.AreEqual(eq, bcf[0].Equivalents.ToString());
            System.Console.WriteLine(
                string.Format("Fragment code '{0}' has formula {1}", code, bcf[0].Formula));

            //the 'master' fragment list for the component must also be back-filled corrrectly
            FragmentList f = registryRecord.ComponentList[0].Compound.FragmentList;
            Assert.AreEqual(1, f.Count);
            Assert.AreEqual(code, f[0].FragmentID.ToString());
            System.Console.WriteLine(
                string.Format("and description {0}", f[0].Description));
        }

        [Test]
        [Category(UnitUtils.DATABASE_DEPENDENCY)]
        public void AddFragmentMethod_FailsWithInvalidFragmentCode()
        {
            Initialize(UnitUtils.GetDataFilePath(@"SD\addproject.sdf"), true);

            string code = "BOGUS_FRAGMENT_CODE";
            string eq = "2.5";

            JobParameters jobParameters = new JobParameters();
            jobParameters.DataSourceInformation = _sourceFileInfo;
            jobParameters.Mappings = ConstructMapping_AddFragment(code, eq);
            jobParameters.SourceRecords = _reader.ExtractToRecords(new IndexRange(0, 0));

            JobResponse jobResponse = JobServiceCaller.MapRecords(jobParameters);
            List<IDestinationRecord> destinationRecordList = jobResponse.ResponseContext[MappingService.DESTINATION_RECORDS] as List<IDestinationRecord>;

            Assert.IsNotNull(destinationRecordList);
            Assert.AreEqual(0, destinationRecordList.Count);
        }

        private Mappings ConstructMapping_AddProject()
        {
            Mappings mappings = new Mappings();

            Mappings.Mapping mapping = new Mappings.Mapping();
            mapping.ObjectBindingPath = "this";
            mapping.MemberInformation.MemberType = Mappings.MemberTypeEnum.Method;
            mapping.MemberInformation.Name = "AddProject";
            mapping.MemberInformation.Type = Mappings.TypeEnum.Instance;

            Mappings.Arg arg = new Mappings.Arg();
            arg.Index = 0;
            arg.Input = Mappings.InputEnum.Derived;
            arg.Type = "string";
            arg.Value = "PROJECT";

            mapping.MemberInformation.Args.Add(arg);
            mappings.MappingCollection.Add(mapping);

            return mappings;
        }

        private Mappings ConstructMapping_AddFragment(string code, string equivalents)
        {
            Mappings mappings = new Mappings();
            {
                Mappings.Mapping mapping = new Mappings.Mapping();
                mapping.ObjectBindingPath = "this";
                mapping.MemberInformation.MemberType = Mappings.MemberTypeEnum.Method;
                mapping.MemberInformation.Name = "AddFragment";
                mapping.MemberInformation.Type = Mappings.TypeEnum.Instance;

                mapping.MemberInformation.Args.Add(
                    MappingUtils.CreateMemberInfoArgument(
                        0, Mappings.InputEnum.Constant, null, "int", "0"
                    )
                );

                mapping.MemberInformation.Args.Add(
                    MappingUtils.CreateMemberInfoArgument(
                        1, Mappings.InputEnum.Constant, null, "string", code
                    )
                );

                mapping.MemberInformation.Args.Add(
                    MappingUtils.CreateMemberInfoArgument(
                        2, Mappings.InputEnum.Constant, null, "float", equivalents
                    )
                );

                mappings.MappingCollection.Add(mapping);
            }
            return mappings;
        }

        private void Initialize(string sourceFileName, bool doLogin)
        {
            string fullFilePath = UnitUtils.GetDataFilePath(sourceFileName);
            _sourceFileInfo = new SourceFileInfo(fullFilePath, SourceFileType.SDFile);

            //Leverage the factory
            _reader = FileReaderFactory.FetchReader(_sourceFileInfo);

            Assert.IsNotNull(_reader, "Failed to construct SDFileReader.");

            if (doLogin)
            {
                UnitUtils.AuthenticateCoeUser("T5_85", "T5_85");
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (_reader != null)
                _reader.Close();

            if (SourceFieldTypes.TypeDefinitions != null)
                SourceFieldTypes.TypeDefinitions.Clear();
        }

    }
}
