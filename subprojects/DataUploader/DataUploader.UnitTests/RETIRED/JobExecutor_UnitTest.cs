//using System;
//using System.Collections.Generic;
//using System.Text;

//using CambridgeSoft.COE.DataLoader.Core;
//using CambridgeSoft.COE.DataLoader.Core.Contracts;
//using CambridgeSoft.COE.DataLoader.Core.DataMapping;

//using NUnit.Framework;

//namespace CambridgeSoft.COE.UnitTests.DataLoader.Core
//{
//    /// <summary>
//    /// Performs tests against the JobExecutor service mthods. Will grow to become quite
//    /// large and encompassing.
//    /// </summary>
//    [TestFixture()]
//    public class JobExecutor_UnitTest
//    {
//        JobParameters parameters = null;

//        /// <summary>
//        /// Test initialization.
//        /// </summary>
//        [SetUp()]
//        public void Initialize()
//        {
//            string fullFilePath = UnitUtils.GetDataFilePath("AGROBASE.txt");
//            parameters = CreateJobParameters(fullFilePath);
//        }

//        /// <summary>
//        /// Tests the execution of a sample job which exports a subset of records from a CSV file.
//        /// The reponse object points the caller to the 'execution log' entry, which itself points
//        /// to a 'details log' file.
//        /// <para>
//        /// In this case, the 'details log' is actually the exported file itself, but that's typically
//        /// NOT the intention, it's just an early-stage sample of record export functionality.
//        /// </para>
//        /// </summary>
//        [Test()]
//        [Category("NoDependencies")]
//        public void Test_SampleJobExecution()
//        {
//            JobResponse response = JobExecutor.ExportDataSubset(parameters, "NUTest");
//        }

//        /// <summary>
//        /// Unit test tear-down
//        /// </summary>
//        [TearDown()]
//        public void Clean()
//        {
//            if (parameters != null)
//                parameters = null;
//        }

//        #region > Helpers <

//        /// <summary>
//        /// Helper method to construct basic JobParameters used by the JobExecutor
//        /// </summary>
//        /// <param name="sourceFilePath"></param>
//        /// <returns></returns>
//        private JobParameters CreateJobParameters(string sourceFilePath)
//        {
//            //the source file information
//            SourceFileInfo sourceFileInformation = new SourceFileInfo(sourceFilePath, SourceFileType.CSV);
//            sourceFileInformation.FileType = SourceFileType.CSV;
//            sourceFileInformation.FieldDelimiters = new string[] { "\t" };
//            sourceFileInformation.HasHeaderRow = true;

//            //data-mappings
//            Mappings mappings = new Mappings();
//            Mappings.Mapping myMap = new Mappings.Mapping();
//            myMap.MemberInformation = new Mappings.MemberInformation();
//            myMap.ObjectBindingPath = null;
//            mappings.MappingCollection.Add(myMap);

//            //file index-range(s) to read
//            IndexRanges ranges = new IndexList(new IndexRange(11, 20)).ToIndexRanges();

//            //construct the JobParameters
//            JobParameters args = new JobParameters(sourceFileInformation, mappings, ranges);
            
//            return args;
//        }

//        #endregion

//    }
//}
