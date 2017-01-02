////===========================================================================
//// CambridgeSoft Corp. Copyright © 2004-2007, All rights reserved.
//// 
//// MolSrvWrapper.cs
////
//// Description:
////      
////
//// Created On: 8/12/2006 10:19:56 AM
//// Created By: Sunil Gupta <mailto:sgupta@camsoft.com> 
////===========================================================================
//using System;
//using System.Collections;
//using System.Diagnostics;

//namespace CambridgeSoft.COE.Framework.Common.MolSrvWrapper
//{
//    /// <summary>
//    /// A class used to manage storage and searching for chemical structures into MolServer.
//    /// </summary>
//    public class MolSrvWrapper
//    {
//        #region Variables
//        private MolServer10.Document mMolDocument = null;
//        private string mMSTPath = null;
//        private string mConnectionString = null;
//        private string mUserID = null;
//        private string mPassword = null;
//        private MolServer10.MSOpenModes openMode = MolServer10.MSOpenModes.kMSNormal;
//        #endregion

//        #region Constructors
//        public MolSrvWrapper() {
//            //
//            // TODO: Add constructor logic here
//            //
//        }
//        #endregion

//        #region Properties
//        public string MSTPath {
//            get {
//                return mMSTPath;
//            }
//            set {
//                mMSTPath = value;
//                //Clear molDocument so that it is re-gotten the next time it is needed.
//                if (mMolDocument != null) {
//                    mMolDocument.Close();
//                    mMolDocument = null;
//                }
//            }
//        }

//        public string ConnectionString {
//            set {
//                mConnectionString = value;
//            }
//        }

//        public string UserID {
//            set {
//                mUserID = value;
//            }
//        }

//        public string Password {
//            set {
//                mPassword = value;
//            }
//        }

//        public MolServer10.MSOpenModes OpenMode {
//            set {
//                openMode = value;
//            }
//        }

//        private MolServer10.Document MolDocument {
//            get {
//                Debug.WriteLine("Entering SearchEngine::GetMolDocument");
//                if (mMolDocument == null) {
//                    if (MSTPath != null) {
//                        Debug.WriteLine("Instantiating Document object");
//                        mMolDocument = new MolServer10.Document();
//                        mMolDocument.Open(MSTPath, Convert.ToInt32(openMode), mPassword);
//                    }
//                }
//                return mMolDocument;
//            }
//        }
//        #endregion

//        #region Methods
//        public IList CacheChemicalStructures(string tableName,
//            Hashtable molServerOptionsMap, int maxHits, IList chemFields) {
//            ArrayList molIDs = new ArrayList();
//            MolServer10.Molecule mol = new MolServer10.MoleculeClass();
//            MolServer10.Search search;
//            MolServer10.searchInfo searchInfo = new MolServer10.searchInfo();
//            searchInfo.maxhits = maxHits;

//            foreach (CSField csField in chemFields) {
//                switch (csField.Type) {
//                    case CSFieldType.CSFDMolWt:
//                        searchInfo.MolwtQuery = csField.Value;
//                        break;
//                    case CSFieldType.CSFDFormula:
//                        searchInfo.FmlaQuery = csField.Value;
//                        break;
//                    case CSFieldType.CSFDStructFile:
//                        mol.Read(csField.Value);
//                        searchInfo.MolQuery = mol;
//                        switch (csField.StrucSearchType) {
//                            case CSStrucSearchType.Substructure:
//                                searchInfo.FullStructure = false;
//                                break;
//                            case CSStrucSearchType.Exact:
//                                searchInfo.FullStructure = true;
//                                break;
//                            case CSStrucSearchType.Similarity:
//                                if ((bool)molServerOptionsMap["SimSearchFullStructure"])
//                                    searchInfo.FullStructure = true;
//                                else
//                                    searchInfo.FullStructure = false;
//                                searchInfo.Similarity = true;
//                                searchInfo.SimThreshold = Convert.ToInt16(csField.Value);
//                                break;
//                        }
//                        break;
//                }
//            }

//            searchInfo.ExtraFragsOK = (bool)molServerOptionsMap["ExtraFragsOK"];
//            searchInfo.ExtraFragsOKIfRxn = (bool)molServerOptionsMap["ExtraFragsOKIfRxn"];
//            searchInfo.FindChargedCarbon = (bool)molServerOptionsMap["FindChargedCarbon"];
//            searchInfo.FindChargedHetero = (bool)molServerOptionsMap["FindChargedHetero"];
//            searchInfo.FragsCanOverlap = (bool)molServerOptionsMap["FragsCanOverlap"];
//            searchInfo.StereoDB = (bool)molServerOptionsMap["StereoDB"];
//            searchInfo.StereoTetr = (bool)molServerOptionsMap["StereoTetr"];
//            searchInfo.UseRxnCenters = (bool)molServerOptionsMap["UseRxnCenters"];
//            searchInfo.AbsHitsRel = (bool)molServerOptionsMap["AbsHitsRel"];
//            searchInfo.RelativeTetStereo = (bool)molServerOptionsMap["RelativeStereo"];
//            searchInfo.IdentitySearch = (bool)molServerOptionsMap["IdentitySearch"];
//            /*
//            if (strucFile != null)
//            {
//                mol.Read(strucFile);
//                searchInfo.MolQuery = mol;
//            }
//            if (molWtQuery != null) 
//                searchInfo.MolwtQuery = molWtComparator + molWtComparator;
//            if (formulaQuery != null)
//                searchInfo.FmlaQuery = formulaQuery;
//*/
//            search = MolDocument.CreateSearchObject(searchInfo);
//            search.Start();
//            do {
//                search.WaitForCompletion(1000);
//            } while (search.Status == 1);

//            Debug.WriteLine("Search Count = " + search.Hitlist.Count.ToString());

//            //Build a list of molIDs
//            for (int i = 0; i <= search.Hitlist.Count - 1; i++) {
//                molIDs.Add((int)(search.Hitlist.get_At(i)));
//            }

//            return molIDs;

//        }

//        /// <summary>
//        /// Add a structure to the structure database (MST file).
//        /// </summary>
//        /// <param name="path"></param>
//        /// <returns></returns>
//        public int AddStructure(string path) {
//            MolServer10.Molecule mol = new MolServer10.MoleculeClass();
//            mol.Read(path);
//            MolDocument.PutMol(mol, 0);
//            return MolDocument.Count;
//        }

//        /// <summary>
//        /// Remove the specified mol_ID from the structure database.
//        /// </summary>
//        /// <param name="molID"></param>
//        public void DeleteStructure(int molID) {
//            MolDocument.DeleteMol(molID);
//        }


//        /// <summary>
//        /// Save structure to the structure database (MST file).
//        /// i.e. update the structure info for a particular molID
//        /// </summary>
//        /// <param name="path"></param>
//        /// <param name="molID"></param>
//        public void SaveStructureToRecord(string path, int molID) {
//            MolServer10.Molecule mol = new MolServer10.MoleculeClass();
//            mol.Read(path);
//            MolDocument.PutMol(mol, molID);
//        }

//        /// <summary>
//        /// Get the formula and molecular weight for a specific molID.
//        /// </summary>
//        /// <param name="molID"></param>
//        /// <param name="formula"></param>
//        /// <param name="molWt"></param>
//        public void GetMoleculeInfo(int molID,
//            ref string formula, ref double molWt) {
//            MolServer10.Molecule mol = MolDocument.GetMol(molID);
//            formula = mol.Formula;
//            molWt = mol.MolWeight;
//        }

//        /// <summary>
//        /// Write structure to file.
//        /// </summary>
//        /// <param name="molID"></param>
//        /// <param name="path"></param>
//        public void WriteMoleculeToFile(int molID, string path) {
//            MolServer10.Molecule mol = MolDocument.GetMol(molID);
//            mol.Write(path, null, null);
//        }
//        #endregion
//    }
//}
