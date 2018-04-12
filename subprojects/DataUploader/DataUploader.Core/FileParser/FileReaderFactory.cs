using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.DataLoader.Core.Contracts;

namespace CambridgeSoft.COE.DataLoader.Core.FileParser
{
    /// <summary>
    /// Provides an abstraction for the creation of <see cref="IFileReader"/> instances, as
    /// well as utility methods for <see cref="SourceFileType"/> data-sources which may house
    /// more than one logical data-set.
    /// </summary>
    public static class FileReaderFactory
    {
        /// <summary>
        /// Simplified factory method. Creates a new instance of <see cref="IFileReader"/>
        /// based on the specified file information (including <see cref="SourceFileType"/>).
        /// </summary>
        /// <param name="fileInformation">the gathered file information</param>
        /// <returns></returns>
        public static IFileReader FetchReader(SourceFileInfo fileInformation)
        {
            if (!fileInformation.DerivedFileInfo.Exists)
                return null;

            IFileReader reader = null;
            switch (fileInformation.FileType)
            {
                case SourceFileType.SDFile:
                    {
                        reader = new SD.SDFileReader(fileInformation.FullFilePath);
                        break;
                    }
                case SourceFileType.CSV:
                    {
                        if (fileInformation.FieldDelimiters != null)
                            reader = new CSV.CSVFileReader(
                                fileInformation.FullFilePath
                                , fileInformation.FieldDelimiters
                                , fileInformation.HasHeaderRow
                            );
                        else if (fileInformation.FieldWidths != null)
                            reader = new CSV.CSVFileReader(
                                fileInformation.FullFilePath
                                , fileInformation.FieldWidths
                            );
                        break;
                    }
                case SourceFileType.MSAccess:
                    {
                        string[] allTables = Access.AccessOleDbReader.FetchTableNames(fileInformation).ToArray();
                        if (Array.IndexOf(allTables, fileInformation.TableName) > -1)
                        {
                            reader = new Access.AccessOleDbReader(
                                fileInformation.FullFilePath
                                , fileInformation.TableName
                                , MSOfficeVersion.Unknown
                            );
                        }
                        break;
                    }
                case SourceFileType.MSExcel:
                    {
                        string[] allTables = Excel.ExcelOleDbReader.FetchTableNames(fileInformation).ToArray();
                        if (allTables != null && allTables.Length> 0 &&  Array.IndexOf(allTables, fileInformation.TableName) > -1)    // Coverity Fix: CBOE-1946
                        {
                            reader = new Excel.ExcelOleDbReader(
                                fileInformation.FullFilePath
                                , fileInformation.TableName
                                , MSOfficeVersion.Unknown
                                , fileInformation.HasHeaderRow
                            );
                        }
                        break;
                    }
                case SourceFileType.ChemFinder:
                    {
                        reader = new CFX.CFXFileReader(fileInformation.FullFilePath);
                        break;
                    }
                case SourceFileType.Unknown:
                    {
                        break;
                    }
                default: break;
            }

            return reader;
        }
    }
}
