using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Schema;
using System.Configuration;
using System.Collections.Generic;

using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Text.RegularExpressions;
using CambridgeSoft.COE.Registration.Services.Types;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using CambridgeSoft.COE.DataLoader.Core.Caching;
using CambridgeSoft.COE.Framework.Services;
using CambridgeSoft.COE.DataLoader.Core.DataMapping;
using CambridgeSoft.COE.DataLoader.Core.FileParser;
using CambridgeSoft.COE.DataLoader.Core.Properties;
using CambridgeSoft.COE.Framework.COEPickListPickerService;
using System.Collections;
using CambridgeSoft.COE.Registration;
using System.Threading;

namespace CambridgeSoft.COE.DataLoader.Core.Workflow
{
    /// <summary>
    /// Utility class that helps wire up the workflow
    /// </summary>
    public static class JobUtility
    {
        public const string PICKLIST_CACHE_MANAGER_NAME = "Picklist Cache Manager";
        public static readonly int CHUNK_SIZE = Convert.ToInt32(ConfigurationManager.AppSettings["ChunkSize"]);
        public static readonly int NUMBER_OF_THREADS = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfThreads"]);
        public static readonly Boolean USE_THREADING = Convert.ToBoolean(ConfigurationManager.AppSettings["UseThreading"]);

        //TODO: The 'ExportSourceRecords' machinery must be more generic.
        // It does not know the full context for the export, so the caller should provide the file name
        //   because it is likely to change given the context (which 'job' is being executed).
        public static void ExportSourceRecords(
            List<ISourceRecord> recordsToExport
            , JobParameters args
            , string exportFileName
            , bool overwrite
            )
        {
            // The caller knows the context for the export, so force it to name the file
            string filePath = Path.Combine(GetDataFolderPath(), exportFileName);
            bool writeHeader = false;

            //create folder if not exists
            int index = filePath.LastIndexOf("\\");
            if (index != -1)
            {
                string fileFolder = filePath.Remove(index);
                Directory.CreateDirectory(fileFolder);
            }

            if (recordsToExport.Count > 0)
            {
                StreamWriter sw = null;
                Mutex ExportFileMutex = new Mutex(false, "ExportFile");
                ExportFileMutex.WaitOne();
                try
                {
                    if (!File.Exists(filePath))
                    {
                        sw = File.CreateText(filePath);
                        writeHeader = true;
                    }
                    else
                        sw = new StreamWriter(filePath, !overwrite);

                    using (sw)
                    {
                        string[] columns = new string[SourceFieldTypes.TypeDefinitions.Count];
                        SourceFieldTypes.TypeDefinitions.Keys.CopyTo(columns, 0);

                        // Define a delimiter for formatting the output; default to TAB
                        string fieldDelimiter = "\t";
                        if (args.DataSourceInformation.FieldDelimiters != null)
                            if (args.DataSourceInformation.FieldDelimiters.Length == 1)
                                fieldDelimiter = args.DataSourceInformation.FieldDelimiters[0];

                        // Only write the header record if the file has just been created
                        // This methid will do nothing for SDFiles
                        if (writeHeader)
                            WriteHeaderRecord(sw, columns, fieldDelimiter, args.DataSourceInformation.FileType);

                        for (int i = 0; i < recordsToExport.Count; i++)
                        {
                            WriteOutputRecord(sw, recordsToExport[i], columns, fieldDelimiter, args.DataSourceInformation.FileType);
                        }
                        sw.Close();
                        sw.Dispose();
                    }
                }
                catch { }
                ExportFileMutex.ReleaseMutex();

            }
        }

        /// <summary>
        /// Normalizes the picklistCode to its case-insensitive equivalent.
        /// </summary>
        public static string NormalizePicklistCode(string picklistCode)
        {
            if (string.IsNullOrEmpty(picklistCode))
                throw new ArgumentNullException("picklistCode");

            string normalizedPicklistCode = string.Empty;
            PickListNameValueList allPicklistDomains = PickListNameValueList.GetAllPickListDomains();

            foreach (DictionaryEntry de in allPicklistDomains.KeyValueList)
            {
                if (string.Compare(de.Key.ToString(), picklistCode, true) == 0)
                {
                    normalizedPicklistCode = de.Key.ToString();
                    break;
                }
                if (string.Compare(de.Value.ToString(), picklistCode, true) == 0)
                {
                    normalizedPicklistCode = de.Value.ToString();
                    break;
                }
            }

            return normalizedPicklistCode;
        }

        /// <summary>
        /// Retrieves a Picklist by its code, from underlying cache.
        /// </summary>
        /// <param name="picklistCode">The code of the Picklist to retrieve</param>
        /// <returns>The retrieved Picklist</returns>
        public static Picklist GetPicklistByCode(string picklistCode)
        {
            string normalizedPicklistCode = NormalizePicklistCode(picklistCode);
            if (normalizedPicklistCode == string.Empty)
                throw new ArgumentException("Invalid picklistCode");

            CacheManager picklistCacheManager = COECacheManager.GetCacheManager(PICKLIST_CACHE_MANAGER_NAME);
            Picklist picklist = null;
            object cachedObject = picklistCacheManager.GetData(normalizedPicklistCode);

            if (cachedObject != null)
                picklist = cachedObject as Picklist;
            else
            {
                int picklistId = -1;
                if (int.TryParse(picklistCode, out picklistId))
                    picklist = Picklist.GetPicklist(PicklistDomain.GetPicklistDomain(picklistId));
                else
                    picklist = Picklist.GetPicklist(PicklistDomain.GetPicklistDomain(picklistCode));

                picklistCacheManager.Add(picklistCode, picklist);
            }

            return picklist;
        }

        public static void DisposeDestinationRecords(List<IDestinationRecord> destinationRecords)
        {
            foreach (IDestinationRecord record in destinationRecords)
            {
                record.Dispose();
            }
        }

        /// <summary>
        /// Utility method to create a unique file name based on the current time.
        /// </summary>
        /// <param name="filePrefix">
        /// Any text, as part of the file name, that should come before the timestamp.
        /// </param>
        /// <param name="fileExtension">
        /// The desired file extension. Defaults to ".txt" if null is provided.
        /// </param>
        /// <returns>a formatted file name</returns>
        public static string GetTimeStampFileName(string filePrefix, string fileExtension)
        {
            if (string.IsNullOrEmpty(filePrefix))
                filePrefix = "job-";
            if (string.IsNullOrEmpty(fileExtension))
                fileExtension = "txt";

            if (fileExtension.StartsWith("."))
                fileExtension = fileExtension.Substring(1);
            if (fileExtension.ToUpper().Equals("SDF"))
                fileExtension = "sdf";

            // Protect the file name from path-unfriendly characters
            string proposedPath =
                string.Format("{0}{1:u}.{2}", filePrefix, DateTime.Now, fileExtension);
            return proposedPath;
        }

        /// <summary>
        /// Get new timestamp file name for log files,
        /// the file name is similar to 10-10-27T19-03-49.Log.txt which is created at 2010/10/27 29:03:49
        /// </summary>
        /// <returns>new timestamp file name</returns>
        public static string GetLogFilePath()
        {
            string applicationName = System.Configuration.ConfigurationManager.AppSettings["ApplicationName"];
            string appFolderLocation = COEConfigurationBO.ConfigurationBaseFilePath;
            string logOutPutPath = Path.Combine(appFolderLocation, @"LogOutput\" + applicationName);
            Directory.CreateDirectory(logOutPutPath);//create directory if not exists
            string fileName = string.Format("{0:yy-MM-ddTHH-mm-ss}", DateTime.Now);
            string fullFilePath = GetPurposedFilePath("Log", new FileInfo(Path.Combine(logOutPutPath, fileName)));
            return fullFilePath;
        }

        /// <summary>
        /// Intended for generating the names for exported files, such as for 'invalid' records.
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetPurposedFilePath(string purpose, System.IO.FileInfo info)
        {
            string baseFile = info.FullName;
            string folder = System.IO.Path.GetDirectoryName(baseFile);
            string name = System.IO.Path.GetFileNameWithoutExtension(baseFile);
            string ext = System.IO.Path.GetExtension(baseFile);
            //Coverity fix - CID 19215
            if (ext != null)
            {
                if (ext.ToUpper() != ".SDF")
                    ext = ".txt";
            }
            if (name.LastIndexOf('.') > 0)
            {
                switch (name.Substring(name.LastIndexOf('.') + 1))
                {
                    case "unique":
                    case "selected":
                    case "structuredup":
                    case "invalid":
                        name = name.Substring(0, name.LastIndexOf('.'));
                        break;
                    
                }
            }
            string newFileName = string.Format("{0}.{1}{2}", name, purpose, ext);
            string proposedPath = System.IO.Path.Combine(folder, newFileName);
            return proposedPath;
        }

        public static string getFileName(string checkType, System.IO.FileInfo info)
        {
            int index = 0;
            string fullPath = info.FullName;
            int indexDot = fullPath.LastIndexOf('.');

            string fName = GetPurposedFilePath(checkType + index.ToString(), info);
            while (File.Exists(fName))
            {
                fName = GetPurposedFilePath(checkType + index.ToString(), info);
                index++;
            }
            return fName;
        }

        public static string getMappingFileName(string fileName)
        {
            string newFile = fileName;
            string mappingFile;
            int index = 0;

            while (File.Exists(mappingFile = (newFile + ".xml")))
            {
                index++;
                newFile = fileName + index.ToString();
            }
            return mappingFile;
        }

        private static string PathSafeString(string proposedPath, string safeReplacement)
        {
            if (safeReplacement == null)
                safeReplacement = "_";

            char safeChar = safeReplacement.ToCharArray()[0];

            char[] badNameChars = System.IO.Path.GetInvalidFileNameChars();
            if (proposedPath.IndexOfAny(badNameChars) > -1)
                foreach (char badChar in badNameChars)
                    proposedPath = proposedPath.Replace(badChar, safeChar);
            return proposedPath;
        }

        /// <summary>
        /// Validate the input mapping file against a schema file
        /// </summary>
        /// <param name="mappingFilePath">The mapping file to be validated</param>
        /// <returns>
        /// The error message if mapping file are invalid, otherwise returns string.Empty
        /// </returns>
        public static string ValidateMappingFile(string mappingFilePath)
        {
            StringBuilder validationError = new StringBuilder();
            string schemaResourceName=System.Reflection.Assembly.GetExecutingAssembly().GetName().Name+".DataMapping.MappingSchema.xsd";
            Stream schemaStream=System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(schemaResourceName);
            XmlSchema schema = XmlSchema.Read(schemaStream,null);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add(schema);
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += delegate(object sender, ValidationEventArgs e)//collect schema validaiton error message
            {
                XmlSchemaException exception = e.Exception;
                string message = string.Format( Resources.MappingFileValidationError, exception.LineNumber, exception.LinePosition, exception.Message);
                validationError.AppendLine(message);
            };
            using (FileStream mappingFile = new FileStream(mappingFilePath,FileMode.Open,FileAccess.Read))
            {
                using (XmlReader reader = XmlReader.Create(mappingFile, settings))
                {
                    try
                    {
                        while (reader.Read());
                    }
                    catch (Exception e)//other kinds of error
                    {
                        validationError.AppendLine(e.Message);
                    }

                    return validationError.ToString();
                }
            }
        }

        /// <summary>
        /// Handles the invalid records by excluding them from 3 places:
        /// ActionRanges,
        /// SourceRecordList,
        /// DestinationRecordList.
        /// The invalid records could come out in 2 job services: MappingService and ValidateRecordService.
        /// </summary>
        /// <param name="invalidSourceRecords">The list of invalid source records</param>
        /// <param name="invalidDestinationRecords">The list of invalid destination records</param>
        /// <param name="jobParameters">The job parameters in action</param>
        public static void HandleInvalidRecords(List<ISourceRecord> invalidSourceRecords,
            List<IDestinationRecord> invalidDestinationRecords,
            JobParameters jobParameters)
        {
            if (invalidSourceRecords.Count != invalidDestinationRecords.Count)
                throw new ArgumentException("Invalid source records and destination records don't have the same amount");

            IndexList indexList = jobParameters.ActionRanges.ToIndexList();

            for (int i = invalidSourceRecords.Count - 1; i >= 0; i--)
            {
                // Exclude from ActionRanges
                indexList.Remove(invalidSourceRecords[i].SourceIndex - 1);

                // Exclude from DestinationRecordList
                // When the 'invalid' records come from failed mapping action, the DestinationRecords
                // is still null now.
                if (jobParameters.DestinationRecords != null)
                    jobParameters.DestinationRecords.RemoveAt(
                        jobParameters.SourceRecords.FindIndex(delegate(ISourceRecord sourceRecord)
                        {
                            return sourceRecord.SourceIndex == invalidSourceRecords[i].SourceIndex;
                        }));

                // Exclude from SourceRecordList
                jobParameters.SourceRecords.Remove(invalidSourceRecords[i]);
            }

            invalidDestinationRecords.ForEach(delegate(IDestinationRecord record)
            {
                
            });

            jobParameters.ActionRanges = indexList.ToIndexRanges();
        }

        #region Private methods

        ///// <summary>
        ///// Retrieves a Picklist by its Id, from underlying cache.
        ///// </summary>
        ///// <param name="picklistId">The Id of the Picklist to retrieve</param>
        ///// <returns>The retrieved Picklist</returns>
        //private static Picklist GetPicklistById(int picklistId)
        //{
        //    CacheManager picklistCacheManager = COECacheManager.GetCacheManager(PICKLIST_CACHE_MANAGER_NAME);
        //    Picklist picklist = null;
        //    object cachedObject = picklistCacheManager.GetData(picklistId.ToString());

        //    if (cachedObject != null)
        //        picklist = cachedObject as Picklist;
        //    else
        //    {
        //        picklist = Picklist.GetPicklist(PicklistDomain.GetPicklistDomain(picklistId));
        //        picklistCacheManager.Add(picklistId.ToString(), picklist);
        //    }

        //    return picklist;
        //}

        ///// <summary>
        ///// Retrieves a Picklist by its description, from underlying cache.
        ///// </summary>
        ///// <param name="picklistName">The description of the Picklist to retrieve</param>
        ///// <returns>The retrieved Picklist</returns>
        //private static Picklist GetPicklistByName(string picklistName)
        //{
        //    CacheManager picklistCacheManager = COECacheManager.GetCacheManager(PICKLIST_CACHE_MANAGER_NAME);
        //    Picklist picklist = null;
        //    object cachedObject = picklistCacheManager.GetData(picklistName);
        //    if (cachedObject != null)
        //        picklist = cachedObject as Picklist;
        //    else
        //    {
        //        picklist = Picklist.GetPicklist(PicklistDomain.GetPicklistDomain(picklistName));
        //        picklistCacheManager.Add(picklistName, picklist);
        //    }

        //    return picklist;
        //}

        /// <summary>
        /// Creates a delimited header row for a CSV file using the preferred delimiter.
        /// </summary>
        /// <param name="writer">the StreamWriter to invoke to create output</param>
        /// <param name="columnNames">the field titles/names/keys</param>
        /// <param name="fieldDelimiter"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        private static void WriteHeaderRecord(
            StreamWriter writer
            , string[] columnNames
            , string fieldDelimiter
            , SourceFileType fileType
            )
        {
            if (fileType != SourceFileType.SDFile)
            {
                string buf = ConcatenateValues(columnNames, fieldDelimiter, true);
                if (!string.IsNullOrEmpty(buf))
                    writer.WriteLine(buf);
            }
        }

        /// <summary>
        /// Creates a delimited row of data from the known field names and the preferred delimiter.
        /// </summary>
        /// <param name="writer">the StreamWriter to invoke to create output</param>
        /// <param name="record">the individual data-points to concatenate</param>
        /// <param name="columnNames">the field titles/names/keys</param>
        /// <param name="fieldDelimiter">the string with which to split individual column values</param>
        /// <param name="fileType">the format of the original data-input file</param>
        /// <returns></returns>
        private static void WriteOutputRecord(
            StreamWriter writer
            , ISourceRecord record
            , string[] columnNames
            , string fieldDelimiter
            , SourceFileType fileType
            )
        {
            string buf = string.Empty;

            if (fileType == SourceFileType.SDFile)
            {
                FileParser.SD.SDSourceRecord sdRecord = (FileParser.SD.SDSourceRecord)record;
                buf = sdRecord.ComposeRecord();
            }
            else
            {
                string[] values = new string[columnNames.Length];
                foreach (KeyValuePair<string, object> pair in record.FieldSet)
                {
                    int index = Array.IndexOf(columnNames, pair.Key);
                    object value = pair.Value;
                    if (value != null&& index!=-1)
                        values[index] = value.ToString();
                }
                buf = ConcatenateValues(values, fieldDelimiter, false);
            }

            if (!string.IsNullOrEmpty(buf))
                writer.WriteLine(buf);
        }

        /// <summary>
        /// The local folder path in which logs and output files will be placed.
        /// </summary>
        /// <returns>a string representing a file (which generally must be created by the caller)</returns>
        private static string GetDataFolderPath()
        {
            string applicationFolder = null;
            string applicationName = null;

            if (ConfigurationManager.AppSettings["ApplicationName"] != null)
            {
                applicationName = ConfigurationManager.AppSettings["ApplicationName"];
                applicationFolder =
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            }
            else
            {
                applicationName = COEAppName.Get();
                applicationFolder = COEConfigurationBO.ConfigurationBaseFilePath;

            }
            string appFolderLocation = Path.Combine(applicationFolder, applicationName);

            return appFolderLocation;
        }

        /// <summary>
        /// Concatenates an array of strings, paying special attention to the possibility that
        /// a specific delimiter, or other formatting-unfriendly characters, might have to be
        /// quote-enclosed.
        /// </summary>
        /// <param name="values">the raw values to concatenate</param>
        /// <param name="delimiter">the string used to separate the individual values</param>
        /// <param name="protectSpaces">
        /// In cases where spaces should not be allowed, quote-enclosure is provided
        /// </param>
        /// <returns></returns>
        private static string ConcatenateValues(string[] values, string delimiter, bool protectSpaces)
        {
            string buf = null;
            for (int j = 0; j < values.Length; j++)
            {

                bool requiresQuoteEnclosure = false;

                if (values[j] != null)
                {
                    string val = values[j];

                    if (
                        (val.Contains(delimiter) || val.Contains(Environment.NewLine) || val.Contains("\r\n"))
                        || (protectSpaces == true && Regex.IsMatch(val, "^ +$"))
                    )
                        requiresQuoteEnclosure = true;

                    if (requiresQuoteEnclosure)
                        buf += ("\"" + values[j] + "\"");
                    else
                        buf += values[j];
                }

                if ((j + 1) < values.Length)
                    buf += delimiter;
            }
            return buf;
        }

        #endregion
    }
}
