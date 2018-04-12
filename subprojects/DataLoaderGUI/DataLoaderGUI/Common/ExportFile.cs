using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.DataLoader.Core;
using System.Data;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser.CSV;
using CambridgeSoft.COE.DataLoader.Core.Workflow;

namespace CambridgeSoft.DataLoaderGUI.Common
{
    /// <summary>
    /// Export to file.
    /// </summary>
    public class ExportFile
    {
        /// <summary>
        /// Export data from datarow to file.
        /// </summary>
        /// <param name="jobParameters">
        /// Includes information from input file.
        /// </param>
        /// <param name="drs">
        /// Includes records which will be exported
        /// </param>
        /// <param name="exportedFilePath">
        /// The export file path and file name.
        /// </param>
        public static void doExportFile(
            JobParameters jobParameters, 
            DataRow[] drs, 
            string exportedFilePath, 
            Dictionary<ISourceRecord, string> records)
        {
            List<ISourceRecord> listRecords = new List<ISourceRecord>();
            List<ISourceRecord> uniqueRecords = new List<ISourceRecord>();

            Dictionary<ISourceRecord, string>.KeyCollection keyCol = records.Keys;
            int indexer = 0;
            foreach (ISourceRecord iSR in keyCol)
            {
                indexer++;
                listRecords.Add(iSR);
            }

            foreach (DataRow dr in drs)
            {
                int index = Convert.ToInt32(dr["index"]) -1;
                uniqueRecords.Add(listRecords[index]);
            }
            JobUtility.ExportSourceRecords(uniqueRecords, jobParameters, exportedFilePath, false);
        }

        /// <summary>
        /// Export data from Dictionary to file.
        /// </summary>
        /// <param name="jobParameters">
        /// Includes information from input file.
        /// </param>
        /// <param name="drs">
        /// Includes records which will be exported
        /// </param>
        /// <param name="exportedFilePath">
        /// The export file path and file name.
        /// </param>
        public static void doExportFile(JobParameters jobParameters, Dictionary<ISourceRecord, string> records, string exportedFilePath)
        {
            List<ISourceRecord> record = new List<ISourceRecord>();

            foreach (KeyValuePair<ISourceRecord, string> kvp in records)
            {
                record.Add(kvp.Key);
            }

            JobUtility.ExportSourceRecords(record, jobParameters, exportedFilePath, true);
        }
    }
}
