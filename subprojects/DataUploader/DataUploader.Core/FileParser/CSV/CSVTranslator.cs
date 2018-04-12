using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;

using Microsoft.VisualBasic.FileIO;

using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;

namespace CambridgeSoft.COE.DataLoader.Core.FileParser.CSV
{
    /// <summary>
    /// This class enables reading of smallish character-separated value data-files for the
    /// purposes of providing lookup tables for custom data-translations.
    /// </summary>
    public class CSVTranslator
    {
        #region > Members < 

        /// <summary>
        /// for 'caching' purposes
        /// </summary>
        private static Dictionary<string, Dictionary<string, string>> allSynonymDictionaries =
            new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// for 'caching' purposes
        /// </summary>
        private static Dictionary<string, DataTable> allLookupTables = new Dictionary<string, DataTable>();

        #endregion

        /// <summary>
        /// Generates a set of key/value pairs from a CSV file on the file-system, using the
        /// data from the specified columns as sources of 'keys' and their associated 'values'.
        /// </summary>
        /// <param name="fullFilePath">the file containing the CSV data</param>
        /// <param name="delimiter">the delimiter separating the columns of data</param>
        /// <param name="externalValueColumn">the name of the column representing the 'foreign key' data</param>
        /// <param name="internalValueColumn">the name of the column representing the desired value</param>
        /// <returns>
        /// a Dictionary which can be used as a translation tool (a look-up) for any given data-point
        /// </returns>
        public static Dictionary<string, string> DictionaryFromTranslationFile(
            string fullFilePath
            , string delimiter
            , string externalValueColumn
            , string internalValueColumn
            )
        {
            DataTable dt = TableFromTranslationFile(fullFilePath, delimiter);
            if (dt != null)
                return DictionaryFromTable(dt, externalValueColumn, internalValueColumn);
            else
                return new Dictionary<string, string>();
        }

        /// <summary>
        /// Clears cached 'translation' data from memory.
        /// </summary>
        public static void ClearCache()
        {
            allLookupTables.Clear();
            allSynonymDictionaries.Clear();
        }

        #region > Helpers <

        /// <summary>
        /// Converts a lookup table into a dictionary for faster lookups by the consumer.
        /// </summary>
        /// <remarks>
        /// For simplicity's sake, the data types of the columns are all System.String. The caller will
        /// be responsible for converting any values as necessary.
        /// </remarks>
        /// <param name="lookupTable">the System.Data.DataTable containing the original data</param>
        /// <param name="externalValueColumn">the name of the column representing the 'foreign key' data</param>
        /// <param name="internalValueColumn">the name of the column representing the desired value</param>
        /// <returns></returns>
        public static Dictionary<string, string>
            DictionaryFromTable(DataTable lookupTable, string externalValueColumn, string internalValueColumn)
        {
            string tableName = lookupTable.TableName;

            // return privately-cached dictionary if found
            if (allSynonymDictionaries.ContainsKey(tableName))
                return allSynonymDictionaries[tableName];

            Dictionary<string, string> synonyms = new Dictionary<string, string>();
            if (
                lookupTable.Columns.Contains(externalValueColumn.ToUpper())
                && lookupTable.Columns.Contains(internalValueColumn.ToUpper())
                )
            {
                foreach (DataRow dr in lookupTable.Rows)
                {
                    object oKey = dr[externalValueColumn.ToUpper()];
                    object oValue = dr[internalValueColumn.ToUpper()];

                    // bypass NULL 'key' values
                    if (!ObjectIsNullOrEmpty(oKey))
                    {
                        string key = oKey.ToString();
                        string value = null;
                        if (!ObjectIsNullOrEmpty(oValue))
                            value = oValue.ToString();

                        // a 'last-in-wins' approach if there is a duplciated 'key' value
                        if (!synonyms.ContainsKey(key))
                            synonyms.Add(key, value);
                        else
                            synonyms[key] = value;
                    }
                }
            }

            if (synonyms.Count > 0)
                allSynonymDictionaries.Add(tableName, synonyms);

            return synonyms;
        }

        /// <summary>
        /// Generates a data table from a simple, header-containing CSV file.
        /// </summary>
        /// <param name="fullFilePath">the file containing the CSV data</param>
        /// <param name="delimiter">the delimiter separating the columns of data</param>
        /// <returns>a System.Data.DataTable representative of the CSV-file contents</returns>
        public static DataTable
            TableFromTranslationFile(string fullFilePath, string delimiter)
        {

            // return privately-cached table if found
            if (allLookupTables.ContainsKey(fullFilePath))
                return allLookupTables[fullFilePath];

            FileInfo info = new FileInfo(fullFilePath);
            DataTable lookupTable = null;

            if (info.Exists)
            {
                using (TextFieldParser tfp = new TextFieldParser(fullFilePath))
                {
                    tfp.TextFieldType = FieldType.Delimited;
                    tfp.Delimiters = new string[]{ delimiter };
                    string[] headers = null;
                    string[] data = null;
                    List<string[]> records = new List<string[]>();

                    do
                    {
                        if (tfp.LineNumber == 1)
                        {
                            data = tfp.ReadFields();
                            headers = data;
                        }
                        else
                        {
                            data = tfp.ReadFields();
                            records.Add(data);
                        }

                    } while (!tfp.EndOfData);

                    tfp.Close();

                    if (headers.Length > 1 && records.Count > 0)
                        lookupTable = CSVTranslator.MakeTable(string.Format("[{0}]", info.Name), headers, records);
                }
            }

            if (lookupTable != null)
                allLookupTables.Add(fullFilePath, lookupTable);

            return lookupTable;
        }

        /// <summary>
        /// Converts a table name, an array of column names, and a list of record-value arrays
        /// into a System.Data.DataTable.
        /// </summary>
        /// <param name="tableName">the name of the new DataTable</param>
        /// <param name="headers">the string array used to define DataColumn objects</param>
        /// <param name="records">the list of string arrays used to define DataRow objects</param>
        /// <returns></returns>
        private static DataTable
            MakeTable(string tableName, string[] headers, List<string[]> records)
        {
            DataTable dt = new DataTable(tableName);
            foreach (string colName in headers)
                dt.Columns.Add(colName.ToUpper());
            foreach (string[] record in records)
            {
                DataRow newRow = dt.Rows.Add(record);
                object[] items = newRow.ItemArray;
            }
            return dt;
        }

        /// <summary>
        /// Determines if the object passed is any of three flavors of 'empty':
        /// <list type="bullet">
        /// <item>DbNull.Value</item>
        /// <item>null</item>
        /// <item>string.Empty</item>
        /// </list>
        /// </summary>
        /// <param name="subject">the object to test</param>
        /// <returns>
        /// false if the <paramref name="subject"/> is a non-empty string or any non-null value
        /// </returns>
        private static bool
            ObjectIsNullOrEmpty(object subject)
        {
            bool isVoid = false;
            if (
                subject == DBNull.Value
                || subject == null
                || string.IsNullOrEmpty(subject.ToString())
            )
                isVoid = true;
            return isVoid;
        }

        #endregion
    }
}
