using System;
using System.IO;
using System.Xml.Schema;
using System.Collections.Generic;
using NUnit.Framework;
using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.Workflow;
using CambridgeSoft.COE.DataLoader.Core.FileParser;
using CambridgeSoft.COE.DataLoader.Core.FileParser.SD;
using CambridgeSoft.COE.DataLoader.Core.FileParser.CSV;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core.Workflow
{
    [TestFixture]
    public class JobUtility_UnitTest
    {
        /// <summary>
        /// Test the functionality of exporting a list of SDSourceRecord to a sd file.
        /// </summary>
        [Test]
        public void ExportSourceRecords_SDSourceRecord()
        {
            string inputFilePath = UnitUtils.GetDataFilePath(@"SD\SAMPLE.sdf");
            string outputFilePath = UnitUtils.GetDataFilePath("Exported.sdf");

            //Export the first two records
            SourceFileInfo sourceFileInfo = new SourceFileInfo(inputFilePath, SourceFileType.SDFile);
            SDFileReader reader = (SDFileReader)FileReaderFactory.FetchReader(sourceFileInfo);
            List<ISourceRecord> recordsToExport = new List<ISourceRecord>();
            recordsToExport.Add(reader.GetNext());
            recordsToExport.Add(reader.GetNext());
            JobParameters parameters=new JobParameters(sourceFileInfo,null,null);
            JobUtility.ExportSourceRecords(recordsToExport, parameters, outputFilePath, true);

            //clear type definitions
            SourceFieldTypes.TypeDefinitions.Clear();

            //Reload the exported records
            sourceFileInfo = new SourceFileInfo(outputFilePath, SourceFileType.SDFile);
            reader = (SDFileReader)FileReaderFactory.FetchReader(sourceFileInfo);
            List<ISourceRecord> reloadedRecords = new List<ISourceRecord>();
            reloadedRecords.Add(reader.GetNext());
            reloadedRecords.Add(reader.GetNext());

            //Compare the original records with the exported ones
            SDSourceRecord recordToExport;
            SDSourceRecord reloadedRecord;
            for (int i = 0; i < 2; i++)
            {
                recordToExport = (SDSourceRecord)recordsToExport[i];
                reloadedRecord = (SDSourceRecord)reloadedRecords[i];
                foreach (string key in recordToExport.FieldSet.Keys)
                {
                    bool equal=recordToExport.FieldSet[key].ToString()==reloadedRecord.FieldSet[key].ToString();
                    Assert.IsTrue(equal, "The exported SD records are not the same as the original ones");
                }
            }
        }

        /// <summary>
        /// Test the functionality of exporting a list of CSVSourceRecord to a sd file.
        /// </summary>
        [Test]
        public void ExportSourceRecords_CSVSourceRecord()
        {
            string inputFilePath = UnitUtils.GetDataFilePath(@"csv\AGROBASE.txt");
            string outputFilePath = UnitUtils.GetDataFilePath("CSVExported.txt");

            //Export the first two records
            SourceFileInfo sourceFileInfo = new SourceFileInfo(inputFilePath, SourceFileType.CSV);
            sourceFileInfo.HasHeaderRow = true;
            sourceFileInfo.FieldDelimiters=new string[]{"\t"};
            CSVFileReader reader = (CSVFileReader)FileReaderFactory.FetchReader(sourceFileInfo);
            List<ISourceRecord> recordsToExport = new List<ISourceRecord>();
            recordsToExport.Add(reader.GetNext());
            recordsToExport.Add(reader.GetNext());
            JobParameters parameters = new JobParameters(sourceFileInfo, null, null);
            JobUtility.ExportSourceRecords(recordsToExport, parameters, outputFilePath, true);

            //clear type definitions
            SourceFieldTypes.TypeDefinitions.Clear();

            //Reload the exported records
            sourceFileInfo = new SourceFileInfo(inputFilePath, SourceFileType.CSV);
            sourceFileInfo.HasHeaderRow = true;
            sourceFileInfo.FieldDelimiters = new string[] { "\t" };
            reader = (CSVFileReader)FileReaderFactory.FetchReader(sourceFileInfo);
            List<ISourceRecord> reloadedRecords = new List<ISourceRecord>();
            reloadedRecords.Add(reader.GetNext());
            reloadedRecords.Add(reader.GetNext());

            //Compare the original records with the exported ones
            CSVSourceRecord recordToExport;
            CSVSourceRecord reloadedRecord;
            for (int i = 0; i < 2; i++)
            {
                recordToExport = (CSVSourceRecord)recordsToExport[i];
                reloadedRecord = (CSVSourceRecord)reloadedRecords[i];
                Assert.AreEqual(recordToExport.ComposeRecord(), reloadedRecord.ComposeRecord(), "The exported CSV records are not the same as the original ones");
            }
        }

        /// <summary>
        /// Test the mapping file validation against a valid mapping file.
        /// </summary>
        [Test]
        public void MappingFileValidation_Valid()
        {
            string validMappingFilePath = Path.Combine(UnitUtils.GetMappingFolderPath(), "ValidationWithSchema/Valid.xml");
            Assert.IsTrue(string.IsNullOrEmpty(JobUtility.ValidateMappingFile(validMappingFilePath)));
        }

        /// <summary>
        /// Test the mapping file validation against a mapping file with wrong boolean value.
        /// </summary>
        [Test]
        public void MappingFileValidation_InvalidBooleanValue()
        {
            string invalidMappingFilePath = Path.Combine(UnitUtils.GetMappingFolderPath(), "ValidationWithSchema/Invalid_BooleanValue.xml");
            Assert.IsFalse(string.IsNullOrEmpty(JobUtility.ValidateMappingFile(invalidMappingFilePath)));
        }

        /// <summary>
        /// Test the mapping file validation against a mapping file with invalid enumeration value
        /// </summary>
        [Test]
        public void MappingFileValidation_InvalidEnumerationValue()
        {
            string invalidMappingFilePath = Path.Combine(UnitUtils.GetMappingFolderPath(), "ValidationWithSchema/Invalid_EnumerationValue.xml");
            Assert.IsFalse(string.IsNullOrEmpty(JobUtility.ValidateMappingFile(invalidMappingFilePath)));
        }

        /// <summary>
        /// Test the mapping file validation against a mapping file with invalid integer value
        /// </summary>
        [Test]
        public void MappingFileValidation_InvalidIntegerValue()
        {
            string invalidMappingFilePath = Path.Combine(UnitUtils.GetMappingFolderPath(), "ValidationWithSchema/Invalid_IntegerValue.xml");
            Assert.IsFalse(string.IsNullOrEmpty(JobUtility.ValidateMappingFile(invalidMappingFilePath)));
        }

        /// <summary>
        /// Test the mapping file validation against a mapping file which misses a required attribute
        /// </summary>
        [Test]
        public void MappingFileValidation_MissRequiredAttribute()
        {
            string invalidMappingFilePath = Path.Combine(UnitUtils.GetMappingFolderPath(), "ValidationWithSchema/Invalid_MissRequiredAttribute.xml");
            Assert.IsFalse(string.IsNullOrEmpty(JobUtility.ValidateMappingFile(invalidMappingFilePath)));
        }
    }
}
