////===========================================================================
//// CambridgeSoft Corp. Copyright © 2004-2007, All rights reserved.
//// 
//// MolSrvHelper.cs
////
//// Description:
////      
////
//// Created On: 8/12/2006 10:18:22 AM
//// Created By: Sunil Gupta <mailto:sgupta@camsoft.com> 
////===========================================================================
//using System;
//using System.Data;
//using System.Diagnostics;
//using System.Data.OleDb;
//using System.Data.SqlClient;
//using System.Collections;
//using System.IO;
//using CambridgeSoft.COE.Framework.Properties;
//using Cambridgesoft.COE.Framework.Types.Exceptions;

//namespace CambridgeSoft.COE.Framework.Common.MolSrvWrapper
//{
//    /// <summary>
//    /// Summary description for MolSrvHelper.
//    /// </summary>
//    public abstract class MolSrvHelper
//    {
//        /// <summary>
//        /// Executes a chemical search against MolServer.
//        /// </summary>
//        /// <param name="molConnStr">a valid connection string</param>
//        /// <param name="relTable">a relational table that chemical data is attached to</param>
//        /// <param name="relField">the relational field that provides the key for the chemical data (usually MOL_ID)</param>
//        /// <param name="mstFile">path to the MST file containing the chemical data</param>
//        /// <param name="molPassword">password, if any for the MST file</param>
//        /// <param name="molOpenMode">Open as Normal = 0, ReadOnly = 1, Exclusive = 2</param>
//        /// <param name="optionsMap">Search options</param>
//        /// <returns>A list of molIDs</returns>
//        public static IList ExecuteChemSearch(
//            string molConnStr, bool identity,
//            string relTable, string relField,
//            string mstFile, string molUser, string molPassword,
//            int molOpenMode, IList chemFields,
//            int maxHits, Hashtable searchOptionsMap
//            ) {
//            // we use a try/catch here because if the method throws an exception we want to 
//            // throw code
//            try {
//                if (!File.Exists(mstFile))
//                    throw new MolServerException(Resources.MSTFileNotFound + " " + mstFile);
//                if (identity)
//                    searchOptionsMap["IdentitySearch"] = true;
//                MolSrvWrapper molSrvW = new MolSrvWrapper();
//                molSrvW.MSTPath = mstFile;
//                molSrvW.ConnectionString = molConnStr;
//                molSrvW.UserID = molUser;
//                molSrvW.Password = molPassword;
//                IList chemHitList = molSrvW.CacheChemicalStructures(relTable, searchOptionsMap, maxHits, chemFields);

//                return chemHitList;

//            } catch {
//                throw;
//            } finally {
//            }

//        }

//        /// <summary>
//        /// Add a structure to the structure database (MST file).
//        /// </summary>
//        /// <param name="molConnStr"></param>
//        /// <param name="mstFile"></param>
//        /// <param name="molUser"></param>
//        /// <param name="molPassword"></param>
//        /// <param name="strucFile"></param>
//        /// <returns></returns>
//        public static int AddStructure(string molConnStr,
//            string mstFile, string molUser, string molPassword,
//            string strucFile) {
//            try {
//                int molID = 0;

//                if (!File.Exists(mstFile))
//                    throw new MolServerException(Resources.MSTFileNotFound + " " + mstFile);
//                MolSrvWrapper molSrvW = new MolSrvWrapper();
//                molSrvW.MSTPath = mstFile;
//                molSrvW.ConnectionString = molConnStr;
//                molSrvW.UserID = molUser;
//                molSrvW.Password = molPassword;
//                molSrvW.OpenMode = MolServer10.MSOpenModes.kMSExclusive;
//                molID = molSrvW.AddStructure(strucFile);

//                return molID;
//            } catch {
//                throw;
//            }
//        }

//        /// <summary>
//        /// Remove the specified mol_ID from the structure database.
//        /// </summary>
//        /// <param name="molConnStr"></param>
//        /// <param name="mstFile"></param>
//        /// <param name="molUser"></param>
//        /// <param name="molPassword"></param>
//        /// <param name="molID"></param>
//        public static void DeleteStructure(string molConnStr,
//            string mstFile, string molUser, string molPassword,
//            int molID) {
//            try {
//                if (!File.Exists(mstFile))
//                    throw new MolServerException(Resources.MSTFileNotFound + " " + mstFile);
//                MolSrvWrapper molSrvW = new MolSrvWrapper();
//                molSrvW.MSTPath = mstFile;
//                molSrvW.ConnectionString = molConnStr;
//                molSrvW.UserID = molUser;
//                molSrvW.Password = molPassword;
//                molSrvW.OpenMode = MolServer10.MSOpenModes.kMSExclusive;
//                molSrvW.DeleteStructure(molID);
//            } catch {
//                throw;
//            }
//        }

//        /// <summary>
//        /// Save structure to the structure database (MST file).
//        /// i.e. update the structure info for a particular molID
//        /// </summary>
//        /// <param name="molConnStr"></param>
//        /// <param name="mstFile"></param>
//        /// <param name="molUser"></param>
//        /// <param name="molPassword"></param>
//        /// <param name="strucFile"></param>
//        /// <param name="molID"></param>
//        public static void SaveStructureToRecord(string molConnStr,
//            string mstFile, string molUser, string molPassword,
//            string strucFile, int molID) {
//            try {
//                if (!File.Exists(mstFile))
//                    throw new MolServerException(Resources.MSTFileNotFound + " " + mstFile);
//                MolSrvWrapper molSrvW = new MolSrvWrapper();
//                molSrvW.MSTPath = mstFile;
//                molSrvW.ConnectionString = molConnStr;
//                molSrvW.UserID = molUser;
//                molSrvW.Password = molPassword;
//                molSrvW.OpenMode = MolServer10.MSOpenModes.kMSExclusive;
//                molSrvW.SaveStructureToRecord(strucFile, molID);
//            } catch {
//                throw;
//            }
//        }

//        /// <summary>
//        /// Write structure to file.
//        /// </summary>
//        /// <param name="molConnStr"></param>
//        /// <param name="mstFile"></param>
//        /// <param name="molUser"></param>
//        /// <param name="molPassword"></param>
//        /// <param name="molID"></param>
//        /// <param name="path"></param>
//        public static void WriteMoleculeToFile(string molConnStr,
//            string mstFile, string molUser, string molPassword,
//            int molID, string path) {
//            try {
//                if (!File.Exists(mstFile))
//                    throw new MolServerException(Resources.MSTFileNotFound + " " + mstFile);
//                MolSrvWrapper molSrvW = new MolSrvWrapper();
//                molSrvW.MSTPath = mstFile;
//                molSrvW.ConnectionString = molConnStr;
//                molSrvW.UserID = molUser;
//                molSrvW.Password = molPassword;
//                molSrvW.OpenMode = MolServer10.MSOpenModes.kMSReadOnly;
//                molSrvW.WriteMoleculeToFile(molID, path);
//            } catch {
//                throw;
//            }
//        }

//        /// <summary>
//        /// Get the formula and molecular weight for a specific molID.
//        /// </summary>
//        /// <param name="molConnStr"></param>
//        /// <param name="mstFile"></param>
//        /// <param name="molUser"></param>
//        /// <param name="molPassword"></param>
//        /// <param name="molID"></param>
//        /// <param name="formula"></param>
//        /// <param name="molWt"></param>
//        public static void GetMoleculeInfo(string molConnStr,
//            string mstFile, string molUser, string molPassword,
//            int molID, ref string formula, ref double molWt) {
//            try {
//                if (!File.Exists(mstFile))
//                    throw new MolServerException(Resources.MSTFileNotFound + " " + mstFile);
//                MolSrvWrapper molSrvW = new MolSrvWrapper();
//                molSrvW.MSTPath = mstFile;
//                molSrvW.ConnectionString = molConnStr;
//                molSrvW.UserID = molUser;
//                molSrvW.Password = molPassword;
//                molSrvW.OpenMode = MolServer10.MSOpenModes.kMSReadOnly;
//                molSrvW.GetMoleculeInfo(molID, ref formula, ref molWt);
//            } catch {
//                throw;
//            }
//        }

//    }
//}
