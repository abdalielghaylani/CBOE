using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Linq;
using System.Reflection;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.COEConfigurationService;

using FormDBLib;

namespace CBVUtilities
{
    public class CBVUtil
    {
        #region Variables
        private static bool m_log = false;
        private static string m_logPath = string.Empty;
        private static int m_pageSize = 0;
        #endregion

        #region Properties
        public static bool Log
        {
            get { return CBVUtil.m_log; }
            set { CBVUtil.m_log = value; }
        }
        public static string LogPath
        {
            get { return CBVUtil.m_logPath; }
            set { CBVUtil.m_logPath = value; }
        }
        public static int PageSize
        {
            get { return CBVUtil.m_pageSize; }
            set { CBVUtil.m_pageSize = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        ///  Sort a list of strings. 
        ///  If the list has numbers inside, it will be sorted in their natural numerical order 
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static List<String> SortStringList(List<String> strings)
        {
            if (strings != null)
            {
                NumericComparer ncomparer = new NumericComparer();
                strings.Sort(ncomparer);
            }
            return strings;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Sort a Dictionary with an int key and a string value
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static Dictionary<int, string> SortDictionary(Dictionary<int, string> sDictionary)
        {
            // using LINQ statements
            var temp = (from entry in sDictionary orderby entry.Value ascending select entry);
            Dictionary<int, string> sortedDictionary = new Dictionary<int, string>();
            foreach (KeyValuePair<int, string> item in temp)
            {
                sortedDictionary.Add(item.Key, item.Value);
            }
            return sortedDictionary;
        }
        //---------------------------------------------------------------------
        public static int StrToInt(String s)
        {
            int i;
            return (Int32.TryParse(s, out i)) ? i : 0;
        }
        //---------------------------------------------------------------------
        public static String IntToStr(int i)
        {
            return i.ToString();
        }
        //---------------------------------------------------------------------
        public static double StrToDbl(String s)
        {
            double d;
            return (double.TryParse(s, out d)) ? d : 0.0;
        }
        //---------------------------------------------------------------------
        public static String DblToStr(double d)
        {
            return d.ToString();
        }
        //---------------------------------------------------------------------
        public static bool Eqstrs(String s1, String s2)
        {
            // true if strings match without regard to case
            // also true if both are empty
            if (String.IsNullOrEmpty(s1))
                return String.IsNullOrEmpty(s2);
            return s1.Equals(s2, StringComparison.CurrentCultureIgnoreCase);
        }
        //---------------------------------------------------------------------
        public static bool StartsWith(String s1, String s2)
        {
            return s1.StartsWith(s2, true, System.Globalization.CultureInfo.CurrentCulture);
        }
        //---------------------------------------------------------------------
        public static bool EndsWith(String s1, String s2)
        {
            return s1.EndsWith(s2, true, System.Globalization.CultureInfo.CurrentCulture);
        }
        //---------------------------------------------------------------------
        public static bool StrEmpty(String s)
        {
            return s == null || s.Length == 0;
        }
        //---------------------------------------------------------------------
        public static String Capitalize(String s)
        {
            String sCap = s;
            if (s.Length > 0)
                sCap = s.Substring(0, 1).ToUpper() + s.Substring(1);
            return sCap;
        }
        //---------------------------------------------------------------------
        public static String ReplaceCRs(String s)
        {
            String sRet = s;
            sRet = sRet.Replace("\\r\\n", "\r\n");
            sRet = sRet.Replace("\\r", "\r\n");
            sRet = sRet.Replace("\\n", "\r\n");
            return sRet;
        }
        //---------------------------------------------------------------------
        public static String ReplacePhrase(String s, String start, String end, String replWith)
        {
            // if s contains phrase "start ... end" then replace the entire thing with replWith
            // do this for every such phrase

            int startIndex = 0;
            while (true)
            {
                if (startIndex >= s.Length) break;
                int iFirst = s.IndexOf(start, startIndex, StringComparison.CurrentCultureIgnoreCase);
                if (iFirst < 0) break;
                int iLast = s.IndexOf(end, iFirst, StringComparison.CurrentCultureIgnoreCase);
                if (iLast < 0) break;
                int iEnd = iLast + end.Length;
                String foundPhrase = s.Substring(iFirst, iEnd - iFirst);
                s = s.Replace(foundPhrase, replWith);

                startIndex = iEnd;
            }
            return s;
        }
        //---------------------------------------------------------------------
        public static int FindFinalTokenPos(String s, Char delim)
        {
            if (s.Length > 1)
            {
                for (int i = s.Length - 2; i >= 0; --i)
                {
                    if (s[i] == delim)
                        return i + 1;
                }
            }
            return -1;
        }
        //---------------------------------------------------------------------
        public static void SetStrAttrib(XmlElement element, String name, String value)
        {
            element.SetAttribute(name, value);
        }
        //---------------------------------------------------------------------
        public static String GetStrAttrib(XmlNode node, String name)
        {
            if (node == null || node.Attributes == null || node.Attributes.Count == 0)
                return "";
            return (node.Attributes[name] == null) ? "" : node.Attributes[name].Value;
        }
        //---------------------------------------------------------------------
        public static int GetIntAttrib(XmlNode node, String name)
        {
            if (node == null || node.Attributes == null || node.Attributes.Count == 0)
                return 0;
            return (node.Attributes[name] == null) ? 0 : StrToInt(node.Attributes[name].Value);
        }
        //---------------------------------------------------------------------
        public static void WriteExceptionInfo(Exception ex)
        {
            Console.WriteLine("--------- Exception Data ---------");
            Console.WriteLine("Message: {0}", ex.Message);
            Console.WriteLine("Exception Type: {0}", ex.GetType().FullName);
            Console.WriteLine("Source: {0}", ex.Source);
            Console.WriteLine("StackTrace: {0}", ex.StackTrace);
            Console.WriteLine("TargetSite: {0}", ex.TargetSite);
        }
        //---------------------------------------------------------------------
        public static void LaunchDoc(String docPath)
        {
            // new 9/11: quote if contains spaces
            String sQuoted = docPath.Contains(' ') ? String.Format("\"{0}\"", docPath) : docPath;
            System.Diagnostics.Process.Start(sQuoted);
        }
        //---------------------------------------------------------------------
        public static void Launch(String progName, String args)
        {
            // new 9/11: quote if contains spaces
            String sQuoted = args.Contains(' ') ? String.Format("\"{0}\"", args) : args;
            System.Diagnostics.Process.Start(progName, sQuoted);
        }
        //---------------------------------------------------------------------
        [DllImport("shell32.dll", EntryPoint = "FindExecutable")]
        public static extern long FindExecutableA(string lpFile, string lpDirectory, StringBuilder lpResult);

        public static String FindExe(String docPath)
        {
            StringBuilder objResultBuffer = new StringBuilder(1024);
            long lngResult = FindExecutableA(docPath, string.Empty, objResultBuffer);

            if (lngResult >= 32)
                return objResultBuffer.ToString();

            return String.Empty;
        }
        //---------------------------------------------------------------------
        public static int ParseLetterNumStr(string s)
        {
            // if string starts with a letter followed by an integer, return the integer
            // return -1 if fails to parse
            int suffix = 0;
            if (s.Length > 1 && Char.IsLetter(s[0]) && Char.IsDigit(s[1]))
                suffix = StrToInt(s.Substring(1));
            return (suffix > 0) ? suffix : -1;
        }
        //---------------------------------------------------------------------
        public static String ParseQualifier(String s, String sQual, ref String sStripped)
        {
            // if string contains qualifier like "/qual=val", return val
            // sQual contains slash or other prefix (ex, sQual = "/Len")
            // if qual is found, sStripped is input string with qual phrase removed
            sStripped = s;
            String sVal = String.Empty, sQuery = sQual;
            int iFound = s.IndexOf(sQuery, StringComparison.CurrentCultureIgnoreCase);
            if (iFound >= 0)
            {
                int iEqPos = s.IndexOf('=', iFound);
                if (iEqPos >= 0)
                {
                    int iEndPos = s.IndexOf('/', iEqPos);
                    if (iEndPos == -1)
                        iEndPos = s.Length;
                    sVal = s.Substring(iEqPos + 1, iEndPos - iEqPos - 1);
                    sVal = sVal.Trim();
                    sStripped = String.Concat(s.Substring(0, iFound), s.Substring(iEndPos)).Trim();
                }
            }
            return sVal;
        }
        //---------------------------------------------------------------------
        public static string BeforeDelimiter(string stringToSplit, char delimiter)
        {
            if (stringToSplit == null) return "";
            int delimiterPos = stringToSplit.LastIndexOf(delimiter);
            if (delimiterPos == -1)
                return stringToSplit;
            return stringToSplit.Substring(0, delimiterPos);
        }
        //---------------------------------------------------------------------
        public static string AfterDelimiter(string stringToSplit, char delimiter)
        {
            if (stringToSplit == null) return "";
            int delimiterPos = stringToSplit.LastIndexOf(delimiter);
            if (delimiterPos == -1)
                return "";      // there is no delimiter, so can't be anything after it
            return stringToSplit.Substring(delimiterPos + 1);
        }
        //---------------------------------------------------------------------
        public static string BeforeFirstDelimiter(string stringToSplit, char delimiter)
        {
            if (stringToSplit == null) return "";
            int delimiterPos = stringToSplit.IndexOf(delimiter);
            if (delimiterPos == -1)
                return stringToSplit;
            return stringToSplit.Substring(0, delimiterPos);
        }
        //---------------------------------------------------------------------
        public static string AfterFirstDelimiter(string stringToSplit, char delimiter)
        {
            if (stringToSplit == null) return "";
            int delimiterPos = stringToSplit.IndexOf(delimiter);
            if (delimiterPos == -1)
                return "";      // there is no delimiter, so can't be anything after it
            return stringToSplit.Substring(delimiterPos + 1);
        }
        //---------------------------------------------------------------------
        public static string ExtractBracketed(string input, ref string bracketed)
        {
            // if string contains [text], remove brackets and contents, return bracketed part in arg
            // example: "abc[def]ghi" returns "abcghi" and sets bracketed to "def"
            string output = input;
            bracketed = string.Empty;
            int iBrackL = input.IndexOf('['), iBrackR = input.IndexOf(']');
            if (iBrackL >= 0 && iBrackR > iBrackL)
            {
                bracketed = input.Substring(iBrackL + 1, iBrackR - iBrackL - 1);
                output = input.Substring(0, iBrackL);
                if (iBrackR < input.Length - 1)
                    output += input.Substring(iBrackR + 1);
            }
            return output;
        }
        //---------------------------------------------------------------------
        public static void ParseTag(string tag, ref BindingTagData btData)
        {
            ParseTag(tag, ref btData.m_bindingProp, ref btData.m_bindingMember, ref btData.m_formatStr, ref btData.m_nullValStr);
        }
        //---------------------------------------------------------------------
        public static void ParseTag(string tag, ref string prop, ref string field)
        {
            string format = "", nullval = "";
            ParseTag(tag, ref prop, ref field, ref format, ref nullval);
        }
        //---------------------------------------------------------------------
        public static void ParseTag(string tag, ref string prop, ref string field, ref string format, ref string nullval)
        {
            // tag format is prop[format/nullval].field, where format and nullval are optional
            // example: Text.BP or Text[N3].BP or Text[N3/0.0].BP
            format = nullval = string.Empty;
            field = AfterDelimiter(tag, '.');
            prop = BeforeDelimiter(tag, '.');
            if (prop.Contains('['))
            {
                prop = ExtractBracketed(prop, ref format);
                int slashPos = format.IndexOf('/');
                if (slashPos > 0)
                {
                    nullval = format.Substring(slashPos + 1);
                    format = format.Substring(0, slashPos);
                }
            }
        }
        //---------------------------------------------------------------------
        public static string MakeTag(BindingTagData btData)
        {
            return MakeTag(btData.m_bindingProp, btData.m_bindingMember, btData.m_formatStr, btData.m_nullValStr);
        }
        //---------------------------------------------------------------------
        public static string MakeTag(string prop, string field)
        {
            return MakeTag(prop, field, "", "");
        }
        //---------------------------------------------------------------------
        public static string MakeTag(string prop, string field, string format, string nullval)
        {
            if (String.IsNullOrEmpty(field))
                return String.Empty;

            string prefix = prop;
            if (!string.IsNullOrEmpty(format) || !string.IsNullOrEmpty(nullval))
            {
                prefix += "[" + format;
                if (!string.IsNullOrEmpty(nullval))
                    prefix += "/" + nullval;
                prefix += "]";
            }
            string s = string.Format("{0}.{1}", prefix, field);
            return s;
        }
        //---------------------------------------------------------------------
        public static string MimeTypeToFileExt(COEDataView.MimeTypes mimeType)
        {
            switch (mimeType)
            {
                case COEDataView.MimeTypes.NONE: break;
                case COEDataView.MimeTypes.UNKNOWN: break;
                case COEDataView.MimeTypes.IMAGE_JPEG: return ".jpg";
                case COEDataView.MimeTypes.IMAGE_GIF: return ".gif";
                case COEDataView.MimeTypes.IMAGE_PNG: return ".png";
                case COEDataView.MimeTypes.IMAGE_X_WMF: return ".wmf";
                case COEDataView.MimeTypes.CHEMICAL_X_MDLMOLFILE: return ".mol";
                case COEDataView.MimeTypes.CHEMICAL_X_CDX: return ".cdx";
                case COEDataView.MimeTypes.CHEMICAL_X_SMILES: return ".txt";
                case COEDataView.MimeTypes.TEXT_XML: return ".xml";
                case COEDataView.MimeTypes.TEXT_HTML: return ".html";
                case COEDataView.MimeTypes.TEXT_PLAIN: return ".txt";
                case COEDataView.MimeTypes.TEXT_RAW: return ".txt";
                case COEDataView.MimeTypes.APPLICATION_MS_EXCEL: return ".xls";
                case COEDataView.MimeTypes.APPLICATION_MS_MSWORD: return ".doc";
                case COEDataView.MimeTypes.APPLICATION_PDF: return ".pdf";
            }
            return string.Empty;
        }
        //---------------------------------------------------------------------
        public static DataTable BindingSourceToDataTable(BindingSource bs)
        {
            // Given a BindingSource, return the table it represents.
            //TODO: This has some things in common with ControlSwapperEx.ParseBindingSourceEx;
            //  perhaps refactor that to call this?
            DataSet dataSet = null;
            DataTable dataTable = null;
            //Coverity Bug Fix CID 12940 
            if (bs != null)
            {
                String bsMember = bs.DataMember;

                if (CBVUtil.StartsWith(bsMember, "Table_") || !bsMember.Contains("->"))
                {
                    // Main table
                    dataSet = bs.DataSource as DataSet;
                    if (dataSet != null)
                        dataTable = dataSet.Tables[bsMember];
                }
                else
                {
                    // Child table
                    dataSet = (bs.DataSource as BindingSource).DataSource as DataSet;

                    if (dataSet == null)
                    {
                        // Grandchild table
                        dataSet = ((bs.DataSource as BindingSource).DataSource as BindingSource).DataSource as DataSet;
                    }

                    if (dataSet != null)
                    {
                        foreach (DataRelation rel in dataSet.Relations)
                        {
                            if (CBVUtil.Eqstrs(rel.RelationName, bsMember))
                            {
                                dataTable = rel.ChildTable;
                                break;
                            }
                        }
                    }
                }
            }

            return dataTable;
        }
        //---------------------------------------------------------------------
        public static DataSet ExtractChildDataset(DataSet fullDataset, int childIndex)
        {
            // dataset contains more than one table: extract schema data of table[index] into new dataset
            // first child table is index=1
            // this routine is not used at the moment
            DataSet childDataset = new DataSet();
            DataTable childTable = fullDataset.Tables[childIndex];
            DataTable subTable = childTable.Clone();
            childDataset.Tables.Add(subTable);
            return childDataset;
        }
        //---------------------------------------------------------------------
        private static DataTable AddRelToList(DataTable dtChild, List<DataRelation> list, DataSet dataSet)
        {
            foreach (DataRelation rel in dataSet.Relations)
            {
                if (rel.ChildTable.TableName == dtChild.TableName)
                {
                    list.Add(rel);
                    return rel.ParentTable;
                }
            }
            return null;
        }
        //---------------------------------------------------------------------
        public static String GetValByPath(List<DataRelation> path, int mainRow, String fieldName, DataSet dataSet)
        {
            // assert trips if prior call to GetSubDataTable returned null table => path list empty
            //Debug.Assert(dataSet.Tables.Count > 1);
            //Debug.Assert(mainRow >= 0 && mainRow < dataSet.Tables[0].Rows.Count);
            //Debug.Assert(path.Count > 0);
            if (!(dataSet.Tables.Count > 1) || !(mainRow >= 0 && mainRow < dataSet.Tables[0].Rows.Count) || !(path.Count > 0))
                return "";
            try
            {
                // start with main row of main table
                int rowNum = mainRow;
                DataTable table = dataSet.Tables[0];
                DataRow row = table.Rows[rowNum];

                // loop backwards through rel list; it goes from bottom to top
                for (int i = path.Count - 1; i >= 0; --i)
                {
                    DataRelation rel = path[i];
                    DataRow[] subRows = row.GetChildRows(rel);
                    rowNum = (int)rel.ExtendedProperties["selrow"];
                    if (subRows.Length == 0)
                        break;

                    if (rowNum < 0 || rowNum >= subRows.Length)
                        rowNum = 0; // auto-link case: we have no sel row, just use first one

                    row = subRows[rowNum];

                    // on last item, grab value
                    if (i == 0)
                        return row[fieldName].ToString();
                    table = rel.ChildTable;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return "";
        }
        //---------------------------------------------------------------------
        public static List<DataRelation> FindRelPathByTables(DataTable dtMain, DataTable dtChild, DataSet dataSet)
        {
            // child may be directly beneath main, or under another child; return full pathway
            List<DataRelation> list = new List<DataRelation>();
            DataTable dtCurr = dtChild;
            while (dtCurr != null && dtCurr != dtMain)  // CSBR-143846 -- prevent infinite loop
            {
                dtCurr = AddRelToList(dtCurr, list, dataSet);
            }
            return list;
        }
        //---------------------------------------------------------------------
        public static DataRelation FindRelByTables(DataTable dtParent, DataTable dtChild, DataSet dataSet)
        {
            // look through relations of dataset for one having given parent and child
            foreach (DataRelation rel in dataSet.Relations)
            {
                if (rel.ParentTable == dtParent && rel.ChildTable == dtChild)
                    return rel;
            }
            return null;
        }
        //---------------------------------------------------------------------
        public static DataRelation FindRelByTableNames(String sParent, String sChild, DataSet dataSet)
        {
            // look through relations of dataset for one having given parent and child
            foreach (DataRelation rel in dataSet.Relations)
            {
                if (rel.ParentTable.TableName.Equals(sParent) && rel.ChildTable.TableName.Equals(sChild))
                    return rel;
            }
            return null;
        }
        //---------------------------------------------------------------------
        private static object GetChildAggregVal(DataRow row, DataRelation relation, String childColName, String aggregName)
        {
            // loop sub-rows, find avg/min/max
            double fmin = double.MaxValue, fmax = -double.MaxValue, fsum = 0.0;
            int nVals = 0;
            //Coverity Bug Fix : CID 19025 
            if (!string.IsNullOrEmpty(aggregName))
            {
                DataRow[] childRows = row.GetChildRows(relation);
                foreach (DataRow childRow in childRows)
                {
                    object dataValObj = childRow[childColName];
                    if (dataValObj == null)
                        continue;
                    ++nVals;

                    String sVal = dataValObj.ToString();
                    double dVal = CBVUtil.StrToDbl(sVal);
                    switch (aggregName)
                    {
                        case "MIN": if (dVal < fmin) fmin = dVal; break;
                        case "MAX": if (dVal > fmax) fmax = dVal; break;
                        default:
                        case "AVG": fsum += dVal; break;
                    }
                }
                // CSBR-129488: prevent overflow when adding rec to dataset
                if (fmin == double.MaxValue)
                    fmin = 0.0;
                if (fmax == -double.MaxValue)
                    fmax = 0.0;

                if (aggregName.Equals("MIN")) return fmin;
                else if (aggregName.Equals("MAX")) return fmax;
                else if (nVals > 0) return fsum / nVals;
            }
            return null;
        }
        //---------------------------------------------------------------------
        private static DataRelation FindRelHavingChildTable(DataSet dataSet, String childTable)
        {
            Debug.Assert(dataSet.Relations.Count > 0);
            foreach (DataRelation rel in dataSet.Relations)
            {
                if (CBVUtil.Eqstrs(rel.ChildTable.TableName, childTable))
                    return rel;
            }
            return null;
        }
        //---------------------------------------------------------------------
        public static bool IsGrandchildTable(DataSet dataSet, String childTable)
        {
            DataRelation rel = FindRelHavingChildTable(dataSet, childTable);
            if (rel != null)
            {
                String parentTable = rel.ParentTable.TableName;
                if (!CBVUtil.Eqstrs(parentTable, dataSet.Tables[0].TableName))
                    return true;
            }
            return false;
        }
        //---------------------------------------------------------------------
        private static void OutputRows(DataRow[] rows, DataTable dtOut, Object[] newVals, int iStart)
        {
            // recursive
            foreach (DataRow row in rows)
            {
                DataTable dtParent = row.Table;
                int nCols = row.ItemArray.Count();
                for (int i = iStart; i < iStart + nCols; ++i)
                {
                    DataColumn outCol = dtOut.Columns[i];
                    String origColName = CBVUtil.AfterDelimiter(outCol.ColumnName, '/');
                    newVals[i] = row[origColName];
                }
                bool bHasChildRows = dtParent.ChildRelations != null && dtParent.ChildRelations.Count > 0;
                if (bHasChildRows)
                {
                    foreach (DataRelation rel in dtParent.ChildRelations)
                    {
                        DataRow[] childRows = row.GetChildRows(rel);
                        OutputRows(childRows, dtOut, newVals, iStart + nCols);
                    }
                }
                else
                {
                    dtOut.Rows.Add(newVals);
                }
            }
        }
        //---------------------------------------------------------------------
        public static DataSet FlattenDatasetEx(DataSet dataSet)
        {
            Debug.Assert(dataSet.Tables.Count > 0);

            DataSet dsOut = new DataSet();
            dsOut.DataSetName = dataSet.DataSetName;
            DataTable dtOut = new DataTable(dataSet.Tables[0].TableName + "_flat");
            int nColsMain = dataSet.Tables[0].Columns.Count;

            // add cols to output table: all main fields and child fields
            for (int i = 0; i < dataSet.Tables.Count; ++i)
            {
                DataTable table = dataSet.Tables[i];
                foreach (DataColumn col in table.Columns)
                {
                    String outColName = String.Concat(table.TableName, "/", col.ColumnName);
                    DataColumn outCol = new DataColumn(outColName, col.DataType);
                    dtOut.Columns.Add(outCol);
                }
            }
            dsOut.Tables.Add(dtOut);

            int nCols = dtOut.Columns.Count;
            int nRows = dataSet.Tables[0].Rows.Count;
            Object[] newVals = new Object[nCols];
            DataRow[] mainRows = new DataRow[nRows];

            dataSet.Tables[0].Rows.CopyTo(mainRows, 0);
            OutputRows(mainRows, dtOut, newVals, 0);

            return dsOut;
        }
        //---------------------------------------------------------------------
        public static DataSet FlattenDataset(DataSet dataSet)
        {
            Debug.Assert(dataSet.Tables.Count > 0);

            DataSet dsOut = new DataSet();
            DataTable dtOut = new DataTable("ResultTable");

            // add cols to output table: all main fields, linked child fields
            for (int i = 0; i < dataSet.Tables.Count; ++i)
            {
                DataTable table = dataSet.Tables[i];
                foreach (DataColumn col in table.Columns)
                {
                    if (i > 0 && col.ExtendedProperties["aggreg"] == null)
                        continue;

                    DataColumn outCol = new DataColumn(col.ColumnName, col.DataType);
                    if (i > 0)
                    {
                        outCol.ExtendedProperties["aggreg"] = col.ExtendedProperties["aggreg"];
                        outCol.ExtendedProperties["table"] = table.TableName;
                    }
                    dtOut.Columns.Add(outCol);
                }
            }
            dsOut.Tables.Add(dtOut);

            // loop rows of dataset
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                int nCols = dtOut.Columns.Count;
                Object[] newVals = new Object[nCols];
                for (int i = 0; i < nCols; ++i)
                {
                    newVals[i] = null;
                    DataColumn outCol = dtOut.Columns[i];
                    String aggreg = outCol.ExtendedProperties["aggreg"] as String;
                    bool bIsChildCol = !String.IsNullOrEmpty(aggreg);
                    if (bIsChildCol)
                    {
                        String childTableName = outCol.ExtendedProperties["table"] as String;
                        if (!String.IsNullOrEmpty(childTableName))
                        {
                            DataRelation rel = FindRelHavingChildTable(dataSet, childTableName);
                            newVals[i] = GetChildAggregVal(row, rel, outCol.ColumnName, aggreg);
                        }
                    }
                    else
                    {
                        newVals[i] = row[outCol.ColumnName];
                    }
                }
                dtOut.Rows.Add(newVals);
            }
            return dsOut;
        }
        //---------------------------------------------------------------------
        [DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);

        public static void BeginUpdate(Control c)
        {
            c.Cursor = Cursors.WaitCursor;
            LockWindowUpdate(c.Handle);
        }
        //---------------------------------------------------------------------
        public static void EndUpdate(Control c)
        {
            c.Cursor = Cursors.Default;
            c.Refresh();
            LockWindowUpdate(IntPtr.Zero);
        }
        //---------------------------------------------------------------------
        public static String RemoveDocHeader8(String s)
        {
            String sHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            if (s.StartsWith(sHeader))
                s = s.Substring(sHeader.Length);
            return s;
        }
        //---------------------------------------------------------------------
        public static String RemoveDocHeader16(String s)
        {
            String sHeader = "<?xml version=\"1.0\" encoding=\"utf-16\"?>";
            if (s.StartsWith(sHeader))
                s = s.Substring(sHeader.Length);
            return s;
        }
        //---------------------------------------------------------------------
        public static bool IsDocHeader16(String xmlDocument)
        {
            bool isType16 = false;
            if (xmlDocument.StartsWith("<?xml version=\"1.0\" encoding=\"utf-16\"?>", StringComparison.OrdinalIgnoreCase))
            {
                isType16 = true;
            }
            return isType16;
        }
        //---------------------------------------------------------------------
        public static string ReplaceDocHeader16(String xmlDocument)
        {
            string xmlDeclaration = "<?xml version=\"1.0\" encoding=\"utf-16\"?>";
            string substring = string.Empty;
            substring = xmlDocument.Remove(0, xmlDeclaration.Length);
            xmlDeclaration = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            substring = substring.Insert(0, xmlDeclaration);
            return substring;
        }
        //---------------------------------------------------------------------
        public static String StreamToString(Stream stream)
        {
            String s = "";
            stream.Seek(0, SeekOrigin.Begin);
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                s = reader.ReadToEnd();
            return s;
        }
        //---------------------------------------------------------------------
        public static String StreamToStringUnicode(Stream stream)
        {
            String s = "";
            stream.Seek(0, SeekOrigin.Begin);
            using (StreamReader reader = new StreamReader(stream, Encoding.Unicode))
                s = reader.ReadToEnd();
            return s;
        }
        //---------------------------------------------------------------------
        public static Stream StringToStream(String s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            MemoryStream stream = new MemoryStream(bytes);
            return stream;
        }
        //---------------------------------------------------------------------
        public static Stream StringToStreamUnicode(String s)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(s);
            MemoryStream stream = new MemoryStream(bytes);
            return stream;
        }
        //---------------------------------------------------------------------
        public static String PromptForString(String dialogLabel, String initialValue)
        {
            String insertedString = String.Empty;
            PromptForStringDialog getStringDialog = new PromptForStringDialog(dialogLabel, initialValue);
            if (getStringDialog.ShowDialog() == DialogResult.OK)
            {
                insertedString = getStringDialog.InsertedString;
            }
            return insertedString;
        }
        //---------------------------------------------------------------------
        public static String PromptForStringAndVal(String dialogLabel, String initialValue,
                                                String sRadio1, String sRadio2, ref int radioVal,
                                                String dialogLabel2, String initialValue2,
                                                String checkboxLabel, ref bool bCheckVal)
        {
            // prompt dialog with string and pair of radio buttons
            String insertedString = String.Empty;
            PromptWithRadio dialog = new PromptWithRadio(dialogLabel, initialValue,
                    sRadio1, sRadio2, radioVal, dialogLabel2, initialValue2, checkboxLabel, bCheckVal);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                insertedString = dialog.InsertedString;
                radioVal = dialog.RadioChoice;
                bCheckVal = dialog.Checked;
            }
            return insertedString;
        }
        //---------------------------------------------------------------------
        public static String GetSSOServer()
        {
            // NOT USED
            // parse servername from single signon URL
            String serverName = String.Empty;
            String ssu = ConfigurationUtilities.GetSingleSignOnURL();   // like "http://localhost/coesinglesignon/singlesignon.asmx"
            if (!String.IsNullOrEmpty(ssu))
            {
                string[] urlTokens = ssu.Split('/');
                serverName = urlTokens[2];
            }
            return serverName;
        }
        //---------------------------------------------------------------------
        public static bool StringsToFile(List<String> sDataLines, String sFilename, bool bAppend)
        {
            try
            {
                using (StreamWriter swDS = new StreamWriter(sFilename, bAppend, Encoding.UTF8))
                {
                    foreach (String s in sDataLines)
                        swDS.WriteLine(s);
                }
                return true;
            }
            catch (Exception ex)
            {
                CBVUtil.ReportError(ex);
                return false;
            }
        }
        //---------------------------------------------------------------------
        public static void StringToFile(String sData, String sFilename)
        {
            StringToFile(sData, sFilename, Encoding.UTF8);
        }
        //---------------------------------------------------------------------
        public static String FileToString(String sFilename)
        {
            return FileToString(sFilename, Encoding.UTF8);
        }
        //---------------------------------------------------------------------
        public static void StringToFile(String sData, String sFilename, Encoding encoding)
        {
            try     // CSBR-134439 -- crash if can't create file
            // TO DO: return false, let caller know operation failed
            {
                using (StreamWriter swDS = new StreamWriter(sFilename, false, encoding)) // Encoding.Unicode))
                    swDS.Write(sData);
            }
            catch (Exception ex)
            {
                CBVUtil.ReportError(ex);
            }
        }
        //---------------------------------------------------------------------
        public static bool IsValidOutputPath(String sFilename)
        {
            // true if file exists and is writable, or can be created
            FileStream fs = null;
            if (!String.IsNullOrEmpty(sFilename))
            {
                try
                {
                    if (File.Exists(sFilename))
                    {
                        fs = File.OpenWrite(sFilename);
                        if (fs != null) fs.Close();
                    }
                    else
                    {
                        fs = File.Create(sFilename);
                        if (fs != null) fs.Close();
                        File.Delete(sFilename);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    fs = null;
                }
            }
            return fs != null;
        }
        //---------------------------------------------------------------------
        public static String FileToString(String sFilename, Encoding encoding)
        {
            using (StreamReader reader = new StreamReader(sFilename, encoding))
            {
                String s = reader.ReadToEnd();
                return s;
            }
        }
        //---------------------------------------------------------------------
        public static XmlNode LoadXmlFile(String filename)
        {
            String sXml = CBVUtil.FileToString(filename);
            return LoadXmlString(sXml);
        }
        //---------------------------------------------------------------------
        public static String XmlDocToString(XmlDocument xdoc)
        {
            String sNewXml = string.Empty;
            //Coverity Bug Fix CID 18777 
            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    xdoc.Save(xmlTextWriter);
                    sNewXml = stringWriter.ToString();
                }
            }
            return sNewXml;
        }
        //---------------------------------------------------------------------
        public static XmlNode LoadXmlString(String sXml)
        {
            XmlDocument xdoc = new XmlDocument();
            using(StringReader stringReader = new StringReader(sXml))
            {
                using (XmlTextReader xmlReader = new XmlTextReader(stringReader))
                {
                    xmlReader.MoveToContent();
                    XmlNode root = xdoc.ReadNode(xmlReader);
                    return root;
                }
            }
        }
        //---------------------------------------------------------------------
        static String GetDisplayMessage(Exception ex)
        {
            /* CSBR-154349: CBV crashes instead of showing the error message while working from a Client 
               Checking the source of exception and displaying the appropriate message    */
            String s;
            // return short exception message for release, long for debug
            if (ex.Source == "Oracle.DataAccess" || ex.Source == "mscorlib")
                s = ex.Message;
            else
                s = ex.GetBaseException().Message;
#if DEBUG
            s = String.Concat("\nDEBUG MODE ERROR REPORT:\n\n", ex.ToString());
#endif
            /* End of CSBR-154349 */
            return s;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///   Reports the thown exception and writes a log file if Write log setting is on
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="prefix"></param>
        public static void ReportError(Exception ex, String prefix, bool bUseErrorIcon)
        {
            if (m_log) WriteLog(ex, prefix);
            Debug.WriteLine(ex.ToString());

            if (String.IsNullOrEmpty(ex.Message))
            {
                // new 2/11: to get here, throw new Exception("")
                Debug.WriteLine("MESSAGE EMPTY ... not reporting to user");
                return;
            }

            if (ex is FormDBLib.Exceptions.NoHitsException)
            {
                MessageBox.Show("  No hits        ", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                String displayMessage = String.IsNullOrEmpty(prefix) ? GetDisplayMessage(ex) :
                                                        String.Concat(prefix, ": ", GetDisplayMessage(ex));
                if (bUseErrorIcon)
                    MessageBox.Show(displayMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(displayMessage, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //---------------------------------------------------------------------
        public static void ReportError(Exception ex, String prefix)
        {
            ReportError(ex, prefix, true);
        }
        //---------------------------------------------------------------------
        public static void ReportError(Exception ex)
        {
            ReportError(ex, "", true);     // CSBR-114365, do not repeat same message twice
        }
        //---------------------------------------------------------------------
        public static void ReportWarning(Exception ex, String prefix)
        {
            ReportError(ex, prefix, false);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Writes exception details to a log file without displaying an error dialog.
        /// </summary>
        /// <param name="ex">Exception to log</param>
        /// <param name="prefix">A prefix string to identify the exception</param>
        public static void WriteException(Exception ex, String prefix)
        {
            if (m_log) WriteLog(ex, prefix);
        }
        /// <summary>
        ///  Writes a log file with the complete information about the error that ocurred
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="prefix"></param>
        private static void WriteLog(Exception ex, String prefix)
        {
            string eMessage = ex.ToString();
            if (ex.InnerException != null && ex.InnerException.InnerException != null)
                eMessage = ex.InnerException.InnerException.ToString();
            // Exception message
            string message = (String.IsNullOrEmpty(prefix)) ? eMessage : String.Concat(prefix, "\n", eMessage);

            //  Write to error m_log file
            StringBuilder mBuilder = new StringBuilder(DateTime.Now.ToString());
            mBuilder.Append(" - EXCEPTION occurred on ");
            mBuilder.Append(ex.Source);
            if (ex.TargetSite != null)
            {
                mBuilder.Append(" - Method: ");
                mBuilder.Append(ex.TargetSite.Name);
            }
            mBuilder.Append(" - ");

            mBuilder.AppendLine(message);
            mBuilder.AppendLine();
            mBuilder.AppendLine();

            // CSBR-132998; CSBR-115292: create file if path valid but doesn't exist; if path invalid, turn off logging
            if (String.IsNullOrEmpty(m_logPath))
                m_logPath = Path.Combine(Application.CommonAppDataPath, CBVConstants.ERROR_LOG_FILE);

            if (!IsValidOutputPath(m_logPath))
            {
                CBVUtil.Log = false;    // turn off logging
            }
            else
            {
                try
                {
                    using (StreamWriter logWriter = new StreamWriter(m_logPath, true))
                        logWriter.Write(mBuilder.ToString());
                    FormDBLib.SecurityUtils.GrantAccess(m_logPath);
                }
                catch (Exception ex2)
                {
                    Debug.WriteLine(ex2.Message);
                    CBVUtil.Log = false;
                }
            }
        }
        //---------------------------------------------------------------------
        public static void FindOrCreateDir(String dirPath)
        {
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
        }
        //---------------------------------------------------------------------
        public static String FindItemInXml(String sXml, String itemPath)
        {
            // return the value of given attribute or node
            // itemPath is like "//cbvnform/connection/@comments"
            String sItem = "";
            try
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(CBVUtil.StringToStreamUnicode(sXml));
                XmlNode node = xdoc.SelectSingleNode(itemPath);
                if (node != null)
                    sItem = node.InnerText;
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERR IN XML FIND " + e.Message);
            }
            return sItem;
        }
        //---------------------------------------------------------------------
        public static String ReplaceItemInXml(String sXml, String itemPath, String attribName, String newValue)
        {
            // replace given attribute with new value
            // itemPath is like "//cbvnform/connection" ... attribName is like "comments"
            String sNewXml = "";
            try
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(CBVUtil.StringToStreamUnicode(sXml));
                XmlNode node = xdoc.SelectSingleNode(itemPath);
                if (node != null)
                {
                    // remove existing attribute if any
                    XmlAttribute attrib = node.Attributes[attribName];
                    if (attrib != null)
                        node.Attributes.Remove(attrib);

                    // add new attribute
                    //Coverity Bug Fix CID 13027 
                    XmlElement xmlEl = node as XmlElement;
                    if (xmlEl != null)
                        (node as XmlElement).SetAttribute(attribName, newValue);

                    // save to string
                    StringWriter stringWriter = new StringWriter();
                    xdoc.Save(new XmlTextWriter(stringWriter));
                    sNewXml = stringWriter.ToString();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERR IN XML REPLACE " + e.Message);
            }
            return sNewXml;
        }
        //---------------------------------------------------------------------
        public static String CreateDelimitedList(List<int> vals)
        {
            String s = String.Empty;
            for (int i = 0; i < vals.Count; ++i)
            {
                if (!String.IsNullOrEmpty(s)) s += ",";
                s += CBVUtil.IntToStr(vals[i]);
            }
            return s;
        }
        //---------------------------------------------------------------------
        public static String CreateDelimitedStringList(List<String> vals)
        {
            // return comma-delimited string of vals
            // omit empty values; double-quote vals containing commas
            String s = String.Empty;
            for (int i = 0; i < vals.Count; ++i)
            {
                String sVal = vals[i];
                if (!String.IsNullOrEmpty(sVal))
                {
                    if (!String.IsNullOrEmpty(s)) s += ",";
                    if (sVal.Contains(','))
                        sVal = String.Format("\"{0}\"", vals[i]);
                    s += sVal;
                }
            }
            return s;
        }
        //---------------------------------------------------------------------
        public static List<String> ParseDelimitedStringList(String s)
        {
            if (String.IsNullOrEmpty(s))
                return new List<String>();
            String[] toks = s.Split(',');
            List<String> vals = new List<String>(toks);
            return vals;
        }
        //---------------------------------------------------------------------
        public static Rectangle ScaleRect(Rectangle rInner, Rectangle rOuter)
        {
            // return scaled copy of rInner fitted to fill rOuter without distortion
            Rectangle r = rInner;
            int curWid = r.Width, curHgt = r.Height;
            int targWid = rOuter.Width, targHgt = rOuter.Height;
            int top = r.Top, bottom = r.Bottom, left = r.Left, right = r.Right;

            if (curWid == 0 || targWid == 0)
            {
                top = rOuter.Top;
                bottom = rOuter.Bottom;

            }
            else if (curHgt == 0 || targHgt == 0)
            {
                left = rOuter.Left;
                right = rOuter.Right;

            }
            else if (((double)curHgt / (double)curWid) > ((double)targHgt / (double)targWid))
            {
                top = rOuter.Top;
                bottom = rOuter.Bottom;
                int desiredWid = curWid * targHgt / curHgt;
                left = (rOuter.Left + rOuter.Right - desiredWid) / 2;
                right = left + desiredWid;

            }
            else
            {
                left = rOuter.Left;
                right = rOuter.Right;
                int desiredHgt = curHgt * targWid / curWid;
                top = (rOuter.Top + rOuter.Bottom - desiredHgt) / 2;
                bottom = top + desiredHgt;
            }
            return new Rectangle(left, top, right - left, bottom - top);
        }
        //---------------------------------------------------------------------
        public static Rectangle MapRect(Rectangle r, Rectangle rSrc, Rectangle rDest)
        {
            // transform r from one rect space to another
            long srcWid = rSrc.Width, srcHgt = rSrc.Height;
            long destWid = rDest.Width, destHgt = rDest.Height;

            int left = (int)Math.Round((double)rDest.Left + (double)(r.Left - rSrc.Left) * (double)destWid / (double)srcWid);
            int right = (int)Math.Round((double)rDest.Right + (double)(r.Right - rSrc.Right) * (double)destWid / (double)srcWid);
            int top = (int)Math.Round((double)rDest.Top + (double)(r.Top - rSrc.Top) * (double)destHgt / (double)srcHgt);
            int bottom = (int)Math.Round((double)rDest.Bottom + (double)(r.Bottom - rSrc.Bottom) * (double)destHgt / (double)srcHgt);

            return new Rectangle(left, top, right - left, bottom - top);
        }
        //---------------------------------------------------------------------
        public static String FontToString(Font font, Color color)
        {
            // string is <family> <size_pts> <style> <color>
            // where <style> = [B|I|R]
            // and <color> = known color name
            String sStyle = String.Empty;
            if (font.Bold) sStyle = "b";
            if (font.Italic) sStyle += "i";
            if (String.IsNullOrEmpty(sStyle)) sStyle = "r";

            String sColorName = color.ToKnownColor().ToString();
            String s = String.Format("{0},{1},{2},{3}",
                font.FontFamily.Name, font.SizeInPoints, sStyle, sColorName);

            return s;
        }
        //---------------------------------------------------------------------
        public static Font StringToFont(String s, ref Color color)
        {
            String[] toks = s.Split(',');
            FontStyle fs = FontStyle.Regular;
            if (toks[2].Contains('b')) fs |= FontStyle.Bold;
            if (toks[2].Contains('i')) fs |= FontStyle.Italic;

            Font f = new Font(toks[0], (float)StrToDbl(toks[1]), fs);
            KnownColor knownColor = (KnownColor)Enum.Parse(typeof(KnownColor), toks[3]);
            color = Color.FromKnownColor(knownColor);
            return f;
        }
        //---------------------------------------------------------------------
        public static String NumExpressionToString(String expression, bool bIntegers)
        {
            // expression is clause;clause;...
            // where clause is num,num,num-num,... or start:incr:end
            // example: "1-7;3:.25:10;1,5,9;4-7"

            String s = String.Empty, format = bIntegers ? "F0" : "N";
            var results = CBVUtil.ParseExpression(expression);
            foreach (List<float> list in results)
            {
                foreach (float f in list)
                {
                    if (!String.IsNullOrEmpty(s))
                        s += ",";
                    s += f.ToString(format);
                }
            }
            return s;
        }
        //---------------------------------------------------------------------
        public static List<List<float>> ParseExpression(string expression)
        {
            // from http://stackoverflow.com/questions/707508/c-string-convention-parsing
            // "x-y" is the same as "x:1:y" so simplify the expression... 
            expression = expression.Replace("-", ":1:");

            var results = new List<List<float>>();
            foreach (var part in expression.Split(';'))
                results.Add(ParseSubExpression(part));

            return results;
        }
        //---------------------------------------------------------------------
        private static List<float> ParseSubExpression(string part)
        {
            var results = new List<float>();

            // If this is a set of numbers... 
            if (part.IndexOf(',') != -1)
                // Then add each member of the set... 
                foreach (string a in part.Split(','))
                    results.AddRange(ParseSubExpression(a));
            // If this is a range that needs to be computed... 
            else if (part.IndexOf(":") != -1)
            {
                // Parse out the range parameters... 
                var parts = part.Split(':');
                var start = float.Parse(parts[0]);
                var increment = float.Parse(parts[1]);
                var end = float.Parse(parts[2]);

                // Evaluate the range... 
                for (var i = start; i <= end; i += increment)
                    results.Add(i);
            }
            else
                results.Add(float.Parse(part));

            return results;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Splits any string using seperators string.  This is different from the
        /// string.Split method as we ignore delimiters inside double quotes and
        /// will *ignore multiple delimiters in a row (i.e. "One,,,,two" will split
        /// to two fields if comma is a delimiter).
        /// Example:
        /// Delims: " \t," (space, tab, comma)
        /// Input: "one two" three four,five
        /// Returns (4 strings):
        /// one two
        /// three
        /// four
        /// five
        /// </summary>
        /// <param name="text">The string to split.</param>
        /// <param name="delimiters">The characters to split on.</param>
        /// <returns></returns>
        public static string[] SplitQuoted(string text, string delimiters)
        {
            // Default delimiters are a space and tab (e.g. " \t").
            // All delimiters not inside quote pair are ignored.  
            // Default quotes pair is two double quotes ( e.g. '""' ).
            if (text == null)
                throw new ArgumentNullException("text", "text is null.");
            if (delimiters == null || delimiters.Length < 1)
                delimiters = " \t"; // Default is a space and tab.

            // ArrayList res = new ArrayList();
            List<string> res = new List<string>();

            // Build the pattern that searches for both quoted and unquoted elements
            // notice that the quoted element is defined by group #2 (g1)
            // and the unquoted element is defined by group #3 (g2).

            string pattern =
             @"""([^""\\]*[\\.[^""\\]*]*)""" +
             "|" +
             @"([^" + delimiters + @"]+)";

            // Search the string.
            foreach (System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(text, pattern))
            {
                //string g0 = m.Groups[0].Value;
                string g1 = m.Groups[1].Value;
                string g2 = m.Groups[2].Value;
                if (g2 != null && g2.Length > 0)
                {
                    res.Add(g2);
                }
                else
                {
                    // get the quoted string, but without the quotes in g1;
                    res.Add(g1);
                }
            }
            //return (string[])res.ToArray(typeof(string));
            return (string[])res.ToArray();
        }
        //---------------------------------------------------------------------
        public static bool IsQuoted(String s)
        {
            if (s.StartsWith("'") && s.EndsWith("'"))
                return true;
            if (s.StartsWith("\"") && s.EndsWith("\""))
                return true;
            return false;
        }
        //---------------------------------------------------------------------
        public static DataRow FindRowInDataset(DataSet dataSet, String query, String colname)
        {
            if (dataSet.Tables.Count > 0)
            {
                foreach (DataRow dRow in dataSet.Tables[0].Rows)
                    if (CBVUtil.Eqstrs(query, dRow[colname].ToString()))
                        return dRow;
            }
            return null;
        }
        //---------------------------------------------------------------------
        private static void AddStr(ref String strs, String str, char delimiter, int index)
        {
            if (str.Contains(delimiter))
                str = String.Concat('"', str, '"');
            if (index > 0)
                strs += delimiter;
            strs += str;
        }
        //---------------------------------------------------------------------
        public static List<String> DataSetToCSV(DataSet dataSet, String excludeField, bool bWithHeader, char delimiter)
        {
            List<String> strs = new List<String>();
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                DataTable dataTable = dataSet.Tables[0];

                // first line is fieldnames if requested
                if (bWithHeader)
                {
                    int index = 0;
                    String fldNames = String.Empty;
                    for (int col = 0; col < dataTable.Columns.Count; ++col)
                    {
                        String colName = dataTable.Columns[col].ColumnName;
                        if (!CBVUtil.Eqstrs(excludeField, colName))
                            AddStr(ref fldNames, colName, delimiter, index++);
                    }
                    strs.Add(fldNames);
                }

                // rest are for data rows
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    int index = 0;
                    String dataLine = String.Empty;
                    for (int col = 0; col < dataTable.Columns.Count; ++col)
                    {
                        if (!CBVUtil.Eqstrs(excludeField, dataTable.Columns[col].ColumnName))
                            AddStr(ref dataLine, dataRow[col].ToString(), delimiter, index++);
                    }
                    strs.Add(dataLine);
                }
            }
            return strs;
        }
        //---------------------------------------------------------------------  
        // Retrieve the Basetable and child table data 
		public static List<String> DataSetToCSvWithChild(ResultsCriteria resultCriteria,COEDataView dataView, DataSet dataSet, String excludeField, bool bWithHeader, char delimiter) 
        {

            resultCriteria = resultCriteria.RemoveGrandChild(dataView, resultCriteria);
            CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView _dataView = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView();
            _dataView.LoadFromXML(dataView.ToString());
            CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.ResultsCriteria _resultCriteria = new CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.ResultsCriteria();
            _resultCriteria.LoadFromXML(resultCriteria.ToString());

            List<int> tablesIdList = new List<int>();
            List<int> childTableIdList = new List<int>();
            List<String> strs = new List<String>();
            List<int>[] tablesID = _resultCriteria.GetTableIds(_dataView);

            for (int itemIndex = 0; itemIndex < tablesID.Length; itemIndex++)
            {
                if (tablesID[itemIndex][0] != _dataView.GetBaseTableId())
                    childTableIdList.Add(int.Parse(tablesID[itemIndex][0].ToString()));
            }

            string baseTableName = "Table_" + _dataView.GetBaseTableId().ToString();
            DataTable baseDatatab = dataSet.Tables[baseTableName];

            int btColumnCount = baseDatatab.Columns.Count;
            List<string> btColNameList = new List<string>();
            int btRecordIndex = 1;

            DataTable filtChildDt = new DataTable();
            DataRow[] filtDataRow;
            string strChdTabName = string.Empty;
            string strTemp = string.Empty;
            string strOutDELIM = string.Empty;
            string strCDXTOMOL = string.Empty;
            int cdxColIndex = -1;

            for (int btColumnIndex = 0; btColumnIndex < btColumnCount; btColumnIndex++)
            {
                String colname = baseDatatab.Columns[btColumnIndex].ColumnName;
                btColNameList.Add(colname);
                if (colname.Contains("BASE64_CDX"))
                    cdxColIndex = btColumnIndex;
            }
            // To Display the Header
            if (bWithHeader)
            {
                int index = 0;
                String fldNames = String.Empty;

                foreach (DataTable dt in dataSet.Tables)
                {
                    if (dt.TableName == baseTableName)
                    {
                        for (int col = 0; col < dt.Columns.Count; ++col)
                        {
                            String colName = dt.Columns[col].ColumnName;
                            if (!CBVUtil.Eqstrs(excludeField, colName))
                                AddStr(ref fldNames, colName, delimiter, index++);
                        }
                    }
                    else
                    {
                        if (childTableIdList.Count > 0)
                        {
                            //foreach (COEDataView.DataViewTable t in dataView.Tables)
                            //{
                            for (int i = 0; i < childTableIdList.Count; i++)
                            {
                                string ChdTabName = "Table_" + childTableIdList[i];
                                int ID = childTableIdList[i];
                                if (dt.TableName == ChdTabName)
                                {
                                    for (int j = 0; j < dataView.Tables.Count; j++)
                                    {
                                        if (ID == dataView.Tables[j].Id)
                                        {
                                            for (int col = 0; col < dt.Columns.Count; ++col)
                                            {
												// CBOE-303, CBOE-1763, CBOE-1764 removed the ":" and placed "."
                                                String colName = dataView.Tables[j].Alias + "." + dt.Columns[col].ColumnName;
                                                if (!CBVUtil.Eqstrs(excludeField, colName))
                                                    AddStr(ref fldNames, colName, delimiter, index++);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }

                }
                strs.Add(fldNames);
            }

            //loop the base table row --- CBOE-303 
            for (int btRowIndex = 0; btRowIndex < dataSet.Tables[baseTableName].Rows.Count; btRowIndex++, btRecordIndex++)
            {
                String dataLine = String.Empty;
                int index = 0;
                int rowCount = 0;
                // Display the base table 
                if (cdxColIndex != -1 && !String.IsNullOrEmpty(baseDatatab.Rows[btRowIndex][cdxColIndex].ToString()))
                {
                    AddStr(ref dataLine, baseDatatab.Rows[btRowIndex][cdxColIndex].ToString(), delimiter, index++);
                    strCDXTOMOL = dataLine;
                }
                for (int colNamLstItmIndex = 0; colNamLstItmIndex < btColNameList.Count; colNamLstItmIndex++)
                {
                    if (colNamLstItmIndex != cdxColIndex)
                        AddStr(ref dataLine, baseDatatab.Rows[btRowIndex][colNamLstItmIndex].ToString(), delimiter, index++);

                }
                strTemp = strTemp + dataLine; dataLine = string.Empty;

                if (childTableIdList.Count > 0)
                {
                    //loop the child tables to get the max rowcount
                    int chdTabIdIndex;
                    for (chdTabIdIndex = 0; chdTabIdIndex < childTableIdList.Count; chdTabIdIndex++)
                    {

                        //filt the child table
                        strChdTabName = "Table_" + childTableIdList[chdTabIdIndex];
                        // string fulChildTableName = _dataView.GetTableName(childTableIdList[chdTabIdIndex]);

                        filtDataRow = dataSet.Tables[strChdTabName].Select(String.Concat("[", dataSet.Tables[strChdTabName].ParentRelations[0].ChildColumns[0].ColumnName, "]") + "= '" + baseDatatab.Rows[btRowIndex][dataSet.Tables[strChdTabName].ParentRelations[0].ParentColumns[0].ColumnName] + "'");
                        filtChildDt = dataSet.Tables[strChdTabName].Clone();
                        if (filtDataRow.Length > 0)
                        {
                            foreach (DataRow dr in filtDataRow)
                            {
                                filtChildDt.ImportRow(dr);
                            }
                            if (rowCount < filtChildDt.Rows.Count)
                                rowCount = filtChildDt.Rows.Count;
                            filtDataRow = null;
                        }
                        filtChildDt = null;
                    }

                    //loop the child tables

                    for (int i = 0; i < rowCount; i++)
                    {

                        for (chdTabIdIndex = 0; chdTabIdIndex < childTableIdList.Count; chdTabIdIndex++)
                        {

                            //filt the child table
                            strChdTabName = "Table_" + childTableIdList[chdTabIdIndex];
                            filtDataRow = dataSet.Tables[strChdTabName].Select(String.Concat("[", dataSet.Tables[strChdTabName].ParentRelations[0].ChildColumns[0].ColumnName, "]") + "= '" + baseDatatab.Rows[btRowIndex][dataSet.Tables[strChdTabName].ParentRelations[0].ParentColumns[0].ColumnName] + "'");
                            filtChildDt = dataSet.Tables[strChdTabName].Clone();
                            if (filtDataRow.Length > 0)
                            {
                                foreach (DataRow dr in filtDataRow)
                                {
                                    filtChildDt.ImportRow(dr);
                                }

                            }
                            for (int colIndex = 0; colIndex < filtChildDt.Columns.Count; colIndex++)
                            {
                                if (i < filtChildDt.Rows.Count)
                                    AddStr(ref dataLine, filtChildDt.Rows[i][colIndex].ToString(), delimiter, index++);
                                else
                                    AddStr(ref dataLine, " ", delimiter, index++);

                            }
                            filtDataRow = null;
                            filtChildDt = null;
                        }
                        strOutDELIM = strTemp + dataLine; dataLine = string.Empty;
                        strs.Add(strOutDELIM);
                    }

                }
                else
                    strs.Add(strTemp);
                if (rowCount == 0 && childTableIdList.Count > 0)
                    strs.Add(strTemp + dataLine);

                strTemp = string.Empty; dataLine = string.Empty;
            }

            return strs;
         
        }        
        //---------------------------------------------------------------------
        public static List<String> EnumerateFiles(String sDir, String sPattern)
        {
            String[] fnames = Directory.GetFiles(sDir, sPattern);
            List<String> filenames = fnames.ToList<String>();
            return filenames;
        }
        //---------------------------------------------------------------------
        //  NUMERIC PARSING for query units processing
        //---------------------------------------------------------------------
        private struct NumSect
        {
            public int iFirst, iLastp1;
            public String sVal;
            public double dVal;
        }
        //---------------------------------------------------------------------
        private static int FindNumStart(String s, int pStart)
        {
            for (int i = pStart; i < s.Length; ++i)
                if (Char.IsDigit(s[i]) ||
                    ((s[i] == '.' || s[i] == '-') &&
                        i < (s.Length - 1) && Char.IsDigit(s[i + 1])))
                    return i;
            return -1;
        }
        //---------------------------------------------------------------------
        private static int FindNumEnd(String s, int pStart)
        {
            int i = 0;
            for (i = pStart; i < s.Length; ++i)
                if (!Char.IsDigit(s[i]) && s[i] != '.')
                    break;
            return i;
        }
        //---------------------------------------------------------------------
        private static List<NumSect> ParseNumSections(String s)
        {
            // find numeric sections of string; return with start/end/val for each
            List<NumSect> sects = new List<NumSect>();
            int pos = 0;
            while ((pos = FindNumStart(s, pos)) != -1)
            {
                NumSect ns = new NumSect();
                ns.iFirst = pos;
                ns.iLastp1 = FindNumEnd(s, pos + 1);
                ns.sVal = s.Substring(ns.iFirst, ns.iLastp1 - ns.iFirst);
                ns.dVal = CBVUtil.StrToDbl(ns.sVal);
                sects.Add(ns);
                pos = ns.iLastp1 + 1;
            }
            return sects;
        }
        //---------------------------------------------------------------------
        public static String ScaleNumerics(String s, double dFactor, double dOffset)
        {
            // parse each numeric val, mult by factor, substitute back into string
            // ex: "abc 123 - 44.4 def" (factor 10.0) -> "abc 1230 - 444 def"
            String sOut = String.Empty;
            int curPos = 0;
            List<NumSect> sects = ParseNumSections(s);
            foreach (NumSect sect in sects)
            {
                sOut += s.Substring(curPos, sect.iFirst - curPos);
                double dVal = (sect.dVal * dFactor) + dOffset;
                String sVal = dVal.ToString("G7");  // uses format G, so some values come out with exponents (like "22E-04")
                sOut += sVal;
                curPos = sect.iLastp1;
            }
            if (curPos < s.Length - 1)
                sOut += s.Substring(curPos, s.Length - curPos);

            return sOut;
        }
        //---------------------------------------------------------------------
        public static bool HasProperty(PropertyInfo[] propsArr, String propName)
        {
            foreach (PropertyInfo pi in propsArr)
                if (pi.Name.Equals(propName))
                    return true;
            return false;
        }
        //---------------------------------------------------------------------
        // key-value pair helpers
        //---------------------------------------------------------------------
        public static Dictionary<string, string> StringToDict(string sData)
        {
            // string format: key=value key=value ...
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] tokens = sData.Split(' ');
            foreach (string token in tokens)
            {
                string[] parts = token.Split('=');
                if (parts != null && parts.GetLength(0) == 2)
                    dict.Add(parts[0], parts[1]);
            }
            return dict;
        }
        //---------------------------------------------------------------------
        public static string DictToString(Dictionary<string, string> dict)
        {
            string s = string.Empty;
            foreach (KeyValuePair<string, string> kvp in dict)
            {
                if (s.Length > 0) s += ' ';
                s += string.Format("{0}={1}", kvp.Key, kvp.Value);
            }
            return s;
        }
        //---------------------------------------------------------------------
        #endregion

        #region Events
        public class StringEventArgs : EventArgs
        {
            private String m_string;
            public StringEventArgs(String s)
            {
                m_string = s;
            }
            public String String
            {
                get { return m_string; }
            }
        }
        //---------------------------------------------------------------------
        #endregion
    }
    //---------------------------------------------------------------------
    public class BindingTagData
    {
        public BindingTagData()
        {
            m_bindingProp = m_bindingMember = m_formatStr = m_nullValStr = String.Empty;
        }
        //---------------------------------------------------------------------
        public BindingTagData(String sProp, String sMem, String sFormat, String sNull)
        {
            m_bindingProp = sProp;
            m_bindingMember = sMem;
            m_formatStr = sFormat;
            m_nullValStr = sNull;
        }
        public String m_bindingProp, m_bindingMember, m_formatStr, m_nullValStr;
    }
    //---------------------------------------------------------------------
    public class FileAssociation
    {
        public FileAssociation()
        { }

        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint AssocQueryString(AssocF flags, AssocStr str, string pszAssoc, string pszExtra,
                                        [Out] StringBuilder pszOut, [In][Out] ref uint pcchOut);

        public string Get(string doctype)
        {
            uint pcchOut = 0;
            AssocQueryString(AssocF.Verify, AssocStr.Executable, doctype, null, null, ref pcchOut);

            StringBuilder pszOut = new StringBuilder((int)pcchOut);
            AssocQueryString(AssocF.Verify, AssocStr.Executable, doctype, null, pszOut, ref pcchOut);
            string doc = pszOut.ToString();
            return doc;
        }
    }
    [Flags]
    public enum AssocF
    {
        Init_NoRemapCLSID = 0x1, Init_ByExeName = 0x2, Open_ByExeName = 0x2, Init_DefaultToStar = 0x4,
        Init_DefaultToFolder = 0x8, NoUserSettings = 0x10, NoTruncate = 0x20, Verify = 0x40,
        RemapRunDll = 0x80, NoFixUps = 0x100, IgnoreBaseClass = 0x200
    }
    public enum AssocStr
    {
        Command = 1, Executable, FriendlyDocName, FriendlyAppName, NoOpen,
        ShellNewValue, DDECommand, DDEIfExec, DDEApplication, DDETopic
    }
}
//---------------------------------------------------------------------
