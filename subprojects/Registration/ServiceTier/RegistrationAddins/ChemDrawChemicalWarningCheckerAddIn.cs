using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CambridgeSoft.COE.Registration.Services.AddIns;
using CambridgeSoft.COE.Registration.Services.Types;
using ChemDrawControl17;
using System.Xml.XPath;
using System.Xml;
using Csla.Validation;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.COE.Registration.Services.RegistrationAddins
{
    /// <summary>
    /// This class will check whether red box warnings should be checked on Submit/Register as per admin configuration
    /// </summary>
    [Serializable]
    public class ChemDrawChemicalWarningCheckerAddIn : IAddIn
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

        Dictionary<enmChemDrawWarnings, int> dicWarning = new Dictionary<enmChemDrawWarnings, int>();
        string _chemicalWarningMessage = string.Empty;     

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

        #endregion

        #region IAddIn Members

        IRegistryRecord _registryRecord;

        /// <summary>
        /// Current Registry record
        /// </summary>
        public IRegistryRecord RegistryRecord
        {
            get
            {
                return _registryRecord;
            }
            set
            {
                _registryRecord = value;
            }
        }

        /// <summary>
        /// Utilizing a configuration xml string, sets local variables and loads the python script
        /// from the file system.
        /// </summary>
        /// <param name="xmlConfiguration">An xml string describing the IAddIn's constrcution.</param>
        public void Initialize(string xmlConfiguration)
        {            
        }

        #endregion  

        #region Events

        /// <summary>
        /// handler for determining whether red box warnings present or not
        /// this should only be done for submit/Register button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnSubmitHandler(object sender, EventArgs args)
        {
            IRegistryRecord record = (IRegistryRecord)sender;
            //CheckRedBoxWarnings(record);
            ChemDrawWarningChecker theChemDrawChecker = new ChemDrawWarningChecker();
            theChemDrawChecker.CheckRedBoxWarnings(record);
        }

       
        #endregion
    }
}
