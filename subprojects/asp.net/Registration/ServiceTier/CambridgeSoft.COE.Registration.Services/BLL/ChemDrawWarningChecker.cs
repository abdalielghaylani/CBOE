using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CambridgeSoft.COE.Registration.Services.AddIns;
using ChemDrawControl14;
using CambridgeSoft.COE.Registration.Services.Types;
using System.Xml;
using CambridgeSoft.COE.Registration.Services.Properties;


namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// Class is used to check chemical warnings
    /// </summary>
    [Serializable()]
    public class ChemDrawWarningChecker : RegistrationBusinessBase<ChemDrawWarningChecker>
    {
        #region Variables

        bool _submitButtonCheck = false;
        bool _registerButtonCheck = false;

        //Warnigs
        bool _valenceChargeErrors = false;
        bool _unbalancedParentheses = false;
        bool _invalidIsotopes = false;
        bool _isolatedBonds = false;
        bool _atomsNearOtherBonds = false;
        bool _unspecifiedStereochemistry = false;
        bool _ambiguousStereochemistry = false;
        bool _stereobondChiralAtoms = false;
        bool _linearAtoms = false;
        bool _miscWarning = false;

        // Configuration settings for modules
        bool _dataLoaderChemDrawWarningChecker = false;
        bool _dataLoader2_ChemDrawWarningChecker = false;
        bool _invLoaderChemDrawWarningChecker = false;
        bool _eNoteBook13_2ChemDrawWarningChecker = false;

        Dictionary<enmChemDrawWarnings, int> dicWarning = new Dictionary<enmChemDrawWarnings, int>();
        string _chemicalWarningMessage = string.Empty;

        // For ENoteBook
        bool _ChemDrawAddInEnabled = false;

        private enum enmChemDrawWarnings
        {
            VALENCE_CHARGE_ERRORS,
            UNBALANCED_PARENTHESES,
            INVALID_ISOTOPES,
            ISOLATED_BONDS,
            ATOMS_NEAR_OTHER_BONDS,
            UNSPECIFIED_STEREOCHEMISTRY,
            AMBIGUOUS_STEREOCHEMISTRY,
            STEREOBOND_WARNING,
            LINEAR_ATOMS,
            MISCELLANEOUS_WARNING
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets/sets Red box warning check on Submit button click
        /// </summary>
        public bool SubmitButtonCheck
        {
            get { return _submitButtonCheck; }
            set { _submitButtonCheck = value; }
        }

        /// <summary>
        /// Gets/sets Red box warning check on Register button click
        /// </summary>
        public bool RegisterButtonCheck
        {
            get { return _registerButtonCheck; }
            set { _registerButtonCheck = value; }
        }

        /// <summary>
        /// Valence and Charge Errors preference on server side
        /// </summary>
        public bool ValenceChargeErrors
        {
            get { return _valenceChargeErrors; }
            set { _valenceChargeErrors = value; }
        }

        /// <summary>
        /// Unbalanced Parantheses preference on server side
        /// </summary>
        public bool UnbalancedParentheses
        {
            get { return _unbalancedParentheses; }
            set { _unbalancedParentheses = value; }
        }

        /// <summary>
        /// Invalid Isotopes preference on server side
        /// </summary>
        public bool InvalidIsotopes
        {
            get { return _invalidIsotopes; }
            set { _invalidIsotopes = value; }
        }

        /// <summary>
        /// Isolated bonds preference on server side
        /// </summary>
        public bool IsolatedBonds
        {
            get { return _isolatedBonds; }
            set { _isolatedBonds = value; }
        }

        /// <summary>
        /// Atoms Near Other Bonds preference on server side
        /// </summary>
        public bool AtomsNearOtherBonds
        {
            get { return _atomsNearOtherBonds; }
            set { _atomsNearOtherBonds = value; }
        }

        /// <summary>
        /// Unspecified Stereochemistry preference on server side
        /// </summary>
        public bool UnspecifiedStereochemistry
        {
            get { return _unspecifiedStereochemistry; }
            set { _unspecifiedStereochemistry = value; }
        }

        /// <summary>
        /// Ambiguous Stereochemistry preference on server side
        /// </summary>
        public bool AmbiguousStereochemistry
        {
            get { return _ambiguousStereochemistry; }
            set { _ambiguousStereochemistry = value; }
        }

        /// <summary>
        /// Stereobond between Chiral Atoms preference on server side
        /// </summary>
        public bool StereobondChiralAtoms
        {
            get { return _stereobondChiralAtoms; }
            set { _stereobondChiralAtoms = value; }
        }

        /// <summary>
        /// Linear atoms preference on server side
        /// </summary>
        public bool LinearAtoms
        {
            get { return _linearAtoms; }
            set { _linearAtoms = value; }
        }

        /// <summary>
        /// Miscellaneous preference on server side
        /// </summary>
        public bool MiscWarning
        {
            get { return _miscWarning; }
            set { _miscWarning = value; }
        }

        /// <summary>
        /// Chemical warinings description
        /// </summary>
        public string ChemicalWarningMessage
        {
            get { return _chemicalWarningMessage; }
            set { _chemicalWarningMessage = value; }
        }

        /// <summary>
        /// Gets or sets whether ChemDraw warnings addin is enabled or not
        /// </summary>
        public bool ChemDrawAddInEnabled
        {
            get { return _ChemDrawAddInEnabled; }
            set { _ChemDrawAddInEnabled = value; }
        }

        /// <summary>
        /// Gets or sets whether chemdraw warnings should check for Dataloader
        /// </summary>
        public bool DataLoaderChemDrawWarningChecker
        {
            get { return _dataLoaderChemDrawWarningChecker; }
            set { _dataLoaderChemDrawWarningChecker = value; }
        }

        /// <summary>
        /// Gets or sets whether chemdraw warnings should check for Dataloader2
        /// </summary>
        public bool DataLoader2_ChemDrawWarningChecker
        {
            get { return _dataLoader2_ChemDrawWarningChecker; }
            set { _dataLoader2_ChemDrawWarningChecker = value; }
        }

        /// <summary>
        /// Gets or sets whether chemdraw warnings should check for Invloader
        /// </summary>
        public bool InvLoaderChemDrawWarningChecker
        {
            get { return _invLoaderChemDrawWarningChecker; }
            set { _invLoaderChemDrawWarningChecker = value; }
        }

        /// <summary>
        /// Gets or sets whether chemdraw warnings should check for ENoteBook13.2
        /// </summary>
        public bool ENoteBook13_2ChemDrawWarningChecker
        {
            get { return _eNoteBook13_2ChemDrawWarningChecker; }
            set { _eNoteBook13_2ChemDrawWarningChecker = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Utilizing a configuration xml string, sets local variables and loads the python script
        /// from the file system.
        /// </summary>
        /// <param name="xmlConfiguration">An xml string describing the IAddIn's constrcution.</param>
        public void Initialize(string xmlConfiguration)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlConfiguration);
            bool blnValue = false;


            XmlNodeList xmlAddInNode = document.SelectNodes("MultiCompoundRegistryRecord/AddIns/AddIn");

            if (xmlAddInNode != null)
            {
                foreach (XmlNode addInNode in xmlAddInNode)
                {
                    if (addInNode.Attributes["class"] != null && addInNode.Attributes["class"].Value.Equals("CambridgeSoft.COE.Registration.Services.RegistrationAddins.ChemDrawChemicalWarningCheckerAddIn"))
                    {
                        if (addInNode.Attributes["enabled"] != null && addInNode.Attributes["enabled"].Value.Trim().ToUpper() == "YES")
                        {
                            ChemDrawAddInEnabled = true;
                        }

                        XmlNode xmlNode = addInNode.SelectSingleNode("AddInConfiguration/SubmitButton");
                        if (xmlNode != null)
                        {
                            if (bool.TryParse(xmlNode.InnerText, out blnValue))
                                _submitButtonCheck = blnValue;
                        }

                        xmlNode = addInNode.SelectSingleNode("AddInConfiguration/RegisterButton");
                        if (xmlNode != null)
                        {
                            if (bool.TryParse(xmlNode.InnerText, out blnValue))
                                _registerButtonCheck = blnValue;
                        }

                        xmlNode = addInNode.SelectSingleNode("AddInConfiguration/ValenceChargeErrors");
                        if (xmlNode != null)
                        {
                            if (bool.TryParse(xmlNode.InnerText, out blnValue))
                                _valenceChargeErrors = blnValue;
                        }

                        xmlNode = addInNode.SelectSingleNode("AddInConfiguration/UnbalancedParantheses");
                        if (xmlNode != null)
                        {
                            if (bool.TryParse(xmlNode.InnerText, out blnValue))
                                _unbalancedParentheses = blnValue;
                        }

                        xmlNode = addInNode.SelectSingleNode("AddInConfiguration/InvalidIsotopes");
                        if (xmlNode != null)
                        {
                            if (bool.TryParse(xmlNode.InnerText, out blnValue))
                                _invalidIsotopes = blnValue;
                        }

                        xmlNode = addInNode.SelectSingleNode("AddInConfiguration/IsolatedBonds");
                        if (xmlNode != null)
                        {
                            if (bool.TryParse(xmlNode.InnerText, out blnValue))
                                _isolatedBonds = blnValue;
                        }

                        xmlNode = addInNode.SelectSingleNode("AddInConfiguration/AtomsNearOtherBonds");
                        if (xmlNode != null)
                        {
                            if (bool.TryParse(xmlNode.InnerText, out blnValue))
                                _atomsNearOtherBonds = blnValue;
                        }

                        xmlNode = addInNode.SelectSingleNode("AddInConfiguration/UnspecifiedStereochemistry");
                        if (xmlNode != null)
                        {
                            if (bool.TryParse(xmlNode.InnerText, out blnValue))
                                _unspecifiedStereochemistry = blnValue;
                        }

                        xmlNode = addInNode.SelectSingleNode("AddInConfiguration/AmbiguousStereochemistry");
                        if (xmlNode != null)
                        {
                            if (bool.TryParse(xmlNode.InnerText, out blnValue))
                                _ambiguousStereochemistry = blnValue;
                        }

                        xmlNode = addInNode.SelectSingleNode("AddInConfiguration/StereobondChiralAtoms");
                        if (xmlNode != null)
                        {
                            if (bool.TryParse(xmlNode.InnerText, out blnValue))
                                _stereobondChiralAtoms = blnValue;
                        }

                        xmlNode = addInNode.SelectSingleNode("AddInConfiguration/LinearAtoms");
                        if (xmlNode != null)
                        {
                            if (bool.TryParse(xmlNode.InnerText, out blnValue))
                                _linearAtoms = blnValue;
                        }

                        xmlNode = addInNode.SelectSingleNode("AddInConfiguration/MiscWarning");
                        if (xmlNode != null)
                        {
                            if (bool.TryParse(xmlNode.InnerText, out blnValue))
                                _miscWarning = blnValue;
                        }

                        xmlNode = addInNode.SelectSingleNode("AddInConfiguration/DataLoader_ChemDrawWarningChecker");
                        if (xmlNode != null)
                        {
                            if (bool.TryParse(xmlNode.InnerText, out blnValue))
                                _dataLoaderChemDrawWarningChecker = blnValue;
                        }

                        xmlNode = addInNode.SelectSingleNode("AddInConfiguration/DataLoader2_ChemDrawWarningChecker");
                        if (xmlNode != null)
                        {
                            if (bool.TryParse(xmlNode.InnerText, out blnValue))
                                _dataLoader2_ChemDrawWarningChecker = blnValue;
                        }

                        xmlNode = addInNode.SelectSingleNode("AddInConfiguration/InvLoader_ChemDrawWarningChecker");
                        if (xmlNode != null)
                        {
                            if (bool.TryParse(xmlNode.InnerText, out blnValue))
                                _invLoaderChemDrawWarningChecker = blnValue;
                        }

                        xmlNode = addInNode.SelectSingleNode("AddInConfiguration/ENoteBook_ChemDrawWarningChecker");
                        if (xmlNode != null)
                        {
                            if (bool.TryParse(xmlNode.InnerText, out blnValue))
                                _eNoteBook13_2ChemDrawWarningChecker = blnValue;
                        }
                        break;
                    }
                }
            }
        }


        private void SetWarningDictionary()
        {
            dicWarning.Clear();
            dicWarning.Add(enmChemDrawWarnings.VALENCE_CHARGE_ERRORS, 0);
            dicWarning.Add(enmChemDrawWarnings.UNBALANCED_PARENTHESES, 0);
            dicWarning.Add(enmChemDrawWarnings.INVALID_ISOTOPES, 0);
            dicWarning.Add(enmChemDrawWarnings.ISOLATED_BONDS, 0);
            dicWarning.Add(enmChemDrawWarnings.ATOMS_NEAR_OTHER_BONDS, 0);
            dicWarning.Add(enmChemDrawWarnings.UNSPECIFIED_STEREOCHEMISTRY, 0);
            dicWarning.Add(enmChemDrawWarnings.AMBIGUOUS_STEREOCHEMISTRY, 0);
            dicWarning.Add(enmChemDrawWarnings.STEREOBOND_WARNING, 0);
            dicWarning.Add(enmChemDrawWarnings.LINEAR_ATOMS, 0);
            dicWarning.Add(enmChemDrawWarnings.MISCELLANEOUS_WARNING, 0);
        }

        /// <summary>
        /// Returns true if gets chemical warnings in the structure
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public bool CheckRedBoxWarnings(IRegistryRecord record)
        {
            bool blnWarningsFound = false;
            bool blnModuleSetting = false;     // For Registration

            _chemicalWarningMessage = string.Empty;
            int intNumOfChemiclaWarnings = 0;

            record.SubmitCheckRedBoxWarning = false;
            record.RegisterCheckRedBoxWarning = false;
            record.RedBoxWarning = string.Empty;

            Initialize(record.XmlWithAddInsWithoutUpdate);

            SetWarningDictionary();

            //CBOE-1159 customizing warning messages as per module : ASV 10JUL13
            ModuleName _ModuleName = record.ModuleName;

            // Checking of chemdraw warnings depending on configuration setting for module
            if (_ModuleName == ModuleName.REGISTRATION)
                blnModuleSetting = true;
            else if (_ModuleName == ModuleName.DATALOADER)
                blnModuleSetting = DataLoaderChemDrawWarningChecker;
            else if (_ModuleName == ModuleName.DATALOADER2)
                blnModuleSetting = DataLoader2_ChemDrawWarningChecker;
            else if (_ModuleName == ModuleName.INVLOADER)
                blnModuleSetting = InvLoaderChemDrawWarningChecker;
            else if (_ModuleName == ModuleName.ELN)
                blnModuleSetting = ENoteBook13_2ChemDrawWarningChecker;

            if (_ChemDrawAddInEnabled && blnModuleSetting)
            {
                if ((record.IsTemporal && _submitButtonCheck) || ((record.IsDirectReg || !record.IsTemporal) && _registerButtonCheck))
                {
                    foreach (Component component in record.ComponentList)
                    {
                        if (!string.IsNullOrEmpty(component.Compound.BaseFragment.Structure.Value)
                            && component.Compound.BaseFragment.Structure.DrawingType == DrawingType.Chemical
                        )
                        {
                            ChemDrawCtl ctrl = new ChemDrawCtl();
                            string originalStructureValue = component.Compound.BaseFragment.Structure.Value;

                            ctrl.Objects.Clear();
                            ctrl.DataEncoded = true;
                            ctrl.Settings.ShowAtomStereo = AmbiguousStereochemistry;
                            ctrl.set_Data("chemical/x-cdx", originalStructureValue);
                           
                            string cdxXml = ctrl.get_Data("cdxml").ToString();

                            // Logic for reading chemical warning description
                            for (int i = 1; i <= ctrl.Objects.Count; i++)
                            {
                                // CBOE-1447, WarningsIgnored condition added for some structures like nano tubes
                                if (!string.IsNullOrEmpty(ctrl.Objects.Item(i).ChemicalWarning) && !ctrl.Objects.Item(i).WarningsIgnored)
                                {
                                    string strWarning = ctrl.Objects.Item(i).ChemicalWarning.Trim();
                                    if (AddChemicalWarning(strWarning))
                                    {
                                        if (!_chemicalWarningMessage.Contains(strWarning))
                                            _chemicalWarningMessage += "<br />" + (++intNumOfChemiclaWarnings).ToString() + ". " + strWarning;
                                    }
                                }
                            }

                            if (intNumOfChemiclaWarnings > 0)
                            {
                                record.RedBoxWarning = ChemicalWarningMessage;
                                if (_submitButtonCheck && !record.SubmitCheckRedBoxWarning)
                                    record.SubmitCheckRedBoxWarning = true;

                                if (_registerButtonCheck && !record.RegisterCheckRedBoxWarning)
                                    record.RegisterCheckRedBoxWarning = true;

                                blnWarningsFound = true;
                            }
                        }
                    }

                    if (blnWarningsFound)
                    {
                        if (_ModuleName != ModuleName.REGISTRATION)
                            ChemicalWarningMessage = ChemicalWarningMessage.Replace("<br />", Environment.NewLine);

                        record.RedBoxWarning = string.Format(GenerateRedBoxWarningMessages(_ModuleName, ((RegistryRecord)record).ActionDuplicates.ToString()) + " " + Resources.RedBoxWarningMessage, record.IsTemporal ? "submit" : "register", ChemicalWarningMessage);
                        record.CheckValidationRedBoxWarningRule();
                    }
                }
            }

            // if no warnings found in any components in the registry record
            if (intNumOfChemiclaWarnings == 0)
            {
                record.RedBoxWarning = string.Empty;
                record.SubmitCheckRedBoxWarning = false;
                record.RegisterCheckRedBoxWarning = false;
            }

            return blnWarningsFound;
        }


        //CBOE-1159 added method for customizing warning messages as per module : ASV 10JUL13
        //CBOE: 1640 added new parameter duplicateAction
        /// <summary>
        /// Function to generate chemical warnings as per module
        /// </summary>
        ///<param name="modulename">Module name</param>
        /// <param name="duplicateAction">Duplicate action</param>
        /// <returns>Customized message for modules</returns>
        private string GenerateRedBoxWarningMessages(ModuleName modulename, string duplicateAction)
        {
            switch (modulename)
            {
                case ModuleName.DATALOADER:
                case ModuleName.DATALOADER2:
                    if (duplicateAction.ToUpper() == "NONE")
                        return string.Empty;
                    return Resources.RedBoxWarningMessage_NotFromRegistration;
                case ModuleName.ELN:
                    return Resources.RedBoxWarningMessage_NotFromRegistration;
                case ModuleName.INVLOADER:
                case ModuleName.REGISTRATION:
                default:
                    return Resources.RedBoxWarningMessage_FromRegistration;
            }
        }

        private bool AddChemicalWarning(string strWarning)
        {

            if (ValenceChargeErrors && (strWarning == Resources.kParseCarbonTooManyBondsOrRadicals || strWarning == Resources.kParseChargeErr || strWarning == Resources.kParseValenceErr || strWarning == Resources.kParseOddSystem || strWarning == Resources.kParseLonePairs))
            {
                dicWarning[enmChemDrawWarnings.VALENCE_CHARGE_ERRORS] = Convert.ToInt32(dicWarning[enmChemDrawWarnings.VALENCE_CHARGE_ERRORS]) + 1;
                return true;
            }
            else if (UnbalancedParentheses && (strWarning == Resources.kParseParenErr || strWarning == Resources.kParseMonomerCrossingBonds || strWarning == Resources.kParseComponentsOutsideMixture))
            {
                dicWarning[enmChemDrawWarnings.UNBALANCED_PARENTHESES] = Convert.ToInt32(dicWarning[enmChemDrawWarnings.UNBALANCED_PARENTHESES]) + 1;
                return true;
            }
            else if (InvalidIsotopes && (strWarning == Resources.kParseInvalidIsotope))
            {
                dicWarning[enmChemDrawWarnings.INVALID_ISOTOPES] = Convert.ToInt32(dicWarning[enmChemDrawWarnings.INVALID_ISOTOPES]) + 1;
                return true;
            }
            else if (AtomsNearOtherBonds && (strWarning == Resources.kParseOverlappingBond || strWarning == Resources.kParseStrayAtom || strWarning == Resources.kParseIsolatedAtom))
            {
                dicWarning[enmChemDrawWarnings.ATOMS_NEAR_OTHER_BONDS] = Convert.ToInt32(dicWarning[enmChemDrawWarnings.ATOMS_NEAR_OTHER_BONDS]) + 1;
                return true;
            }
            else if (IsolatedBonds && (strWarning == Resources.kParseStrayBond))
            {
                dicWarning[enmChemDrawWarnings.ISOLATED_BONDS] = Convert.ToInt32(dicWarning[enmChemDrawWarnings.ISOLATED_BONDS]) + 1;
                return true;
            }
            else if (UnspecifiedStereochemistry && (strWarning == Resources.kParseStereochemistry || strWarning == Resources.kParseRepeatPatternBondTypeMismatch))
            {
                dicWarning[enmChemDrawWarnings.UNSPECIFIED_STEREOCHEMISTRY] = Convert.ToInt32(dicWarning[enmChemDrawWarnings.UNSPECIFIED_STEREOCHEMISTRY]) + 1;
                return true;
            }
            else if (AmbiguousStereochemistry && (strWarning == Resources.kParseStereoAmbig || strWarning == Resources.kParseHDotTooFewBonds || strWarning == Resources.kParseRepeatPatternTooManyCrossingBonds))
            {
                dicWarning[enmChemDrawWarnings.AMBIGUOUS_STEREOCHEMISTRY] = Convert.ToInt32(dicWarning[enmChemDrawWarnings.AMBIGUOUS_STEREOCHEMISTRY]) + 1;
                return true;
            }
            else if (StereobondChiralAtoms && (strWarning == Resources.kParseStereoBondBtwAtoms || strWarning == Resources.kParseChiralContradictsEnhanced))
            {
                dicWarning[enmChemDrawWarnings.STEREOBOND_WARNING] = Convert.ToInt32(dicWarning[enmChemDrawWarnings.STEREOBOND_WARNING]) + 1;
                return true;
            }
            else if (LinearAtoms && (strWarning == Resources.kParseLinearAtom))
            {
                dicWarning[enmChemDrawWarnings.LINEAR_ATOMS] = Convert.ToInt32(dicWarning[enmChemDrawWarnings.LINEAR_ATOMS]) + 1;
                return true;
            }
            else if (MiscWarning && (strWarning == Resources.kParseNonsenseErr || strWarning == Resources.kParseInternalErr || strWarning == Resources.kParseDoublyBondedAtomPair
                || strWarning == Resources.kParseMulticenterIntendedVariable || strWarning == Resources.kParseOverlappingSymbol || strWarning == Resources.kParseIsolatedSymbol
                || strWarning == Resources.kParseMixturesTooFewComponents || strWarning == Resources.kParseQueriesNotPermitted || strWarning == Resources.kParseUnorderedMixturesUnorderedComponents
                || strWarning == Resources.kParseOrderedMixtureNotOrderedComponents || strWarning == Resources.kParseCopolymerTooFewUnits))
            {
                dicWarning[enmChemDrawWarnings.MISCELLANEOUS_WARNING] = Convert.ToInt32(dicWarning[enmChemDrawWarnings.MISCELLANEOUS_WARNING]) + 1;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Enum for modulenames
        /// </summary>
        public enum ModuleName	//CBOE-1159 added enum for modules : ASV 10JUL13
        {
            NOTSET = 0,
            ELN = 1,
            INVLOADER = 2,
            DATALOADER = 3,
            DATALOADER2 = 4,
            REGISTRATION = 5
        }      

        #endregion

    }
}
