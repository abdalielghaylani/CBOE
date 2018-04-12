using System;
using System.Collections.Generic;

using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.DataLoader.Core.FileParser;

namespace CambridgeSoft.COE.DataLoader.Core.Workflow
{
    public class SplitFileService : JobService
    {
        /// <summary>
        /// The response 'key' for the Jobresponse.SessionContext distionary
        /// </summary>
        public const string SPLIT_FILE_RESPONSE = "File_Split_Response";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jobParameters"></param>
        public SplitFileService(JobParameters jobParameters)
            : base(jobParameters)
        {
        }

        /// <summary>
        /// Accepts a data-file, and uses the 'End' value of the data-range provided to define a
        /// maximum record number with which divide the original file into multiple smaller files.
        /// </summary>
        /// <returns>a JobResponse object containing output derived from this service call.</returns>
        protected override JobResponse DoJobInternal()
        {
            JobResponse response = new JobResponse(JobParameters);
            IFileReader reader = JobParameters.FileReader;
            List<string> filesExported = new List<string>();

            try
            {
                int fileCounter = 1;
                // for this feature we ignore the beginning of the range and use only the end value
                int chunkSize = (this.JobParameters.ActionRanges[0].RangeEnd + 1);
                int rangeCounter = 0;

                if (reader == null)
                    reader = FileReaderFactory.FetchReader(this.JobParameters.DataSourceInformation);
                else
                    reader.Rewind();
                //Coverity fix - CID 19194
                if (reader != null)
                {
                    // Convert this 'super-range' into a list of ranges
                    IndexRange actionRange = new IndexRange(1, reader.RecordCount);
                    IndexList actionIndices = actionRange.ToIndexList();
                    IndexRanges actionRanges = actionIndices.ToIndexRanges(chunkSize);

                    // Iterate the record ranges, fetching data and spawning file-subsets of the original file
                    foreach (IndexRange range in actionRanges.Values)
                    {
                        rangeCounter++;
                        IndexList indices = range.ToIndexList();
                        List<Contracts.ISourceRecord> recordList = new List<Contracts.ISourceRecord>();

                        string newFilepath =
                            GetNewFilePath(rangeCounter, this.JobParameters.DataSourceInformation.DerivedFileInfo);

                        Console.WriteLine("Exporting records {0} to {1} to '{2}'", range.RangeBegin, range.RangeEnd, newFilepath);

                        bool exitThisRange = false;
                        foreach (int i in indices)
                        {
                            // Ensure we're not past the end of the file
                            if (i > reader.RecordCount)
                            {
                                exitThisRange = true;
                                break;
                            }

                            // Fetch the next record
                            Contracts.ISourceRecord record = reader.GetNext();
                            if (record != null)
                                recordList.Add(record);
                            else
                            {
                                exitThisRange = true;
                                break;
                            }

                        }

                        if (exitThisRange)
                            break;
                        else
                        {
                            JobUtility.ExportSourceRecords(recordList, this.JobParameters, newFilepath, true);
                            filesExported.Add(newFilepath);
                        }

                    }

                }
                //Return the names of the files created as a result of splitting the master file
                response.ResponseContext[SPLIT_FILE_RESPONSE] = filesExported;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return response;
        }

        protected override bool AreJobParametersValid()
        {
            return true;
            throw new Exception("The method or operation is not implemented.");
        }

        private string GetNewFilePath(int counter, System.IO.FileInfo info)
        {
            string baseFile = info.FullName;
            string folder = System.IO.Path.GetDirectoryName(baseFile);
            string name = System.IO.Path.GetFileNameWithoutExtension(baseFile);
            string ext = System.IO.Path.GetExtension(baseFile);
            //Coverity fix - CID 19216
            if (ext != null)
            {
                if (ext.ToUpper() != ".SDF")
                    ext = ".txt";
            }

            string number = counter.ToString().PadLeft(4, "0".ToCharArray()[0]);
            
            string newFileName = string.Format("{0}_{1}{2}", name, number, ext);
            string newFilePath = System.IO.Path.Combine(folder, newFileName);

            return newFilePath;
        }
    }
}
