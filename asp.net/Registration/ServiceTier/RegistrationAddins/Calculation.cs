using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Registration.Services.RegistrationAddins
{
    /// <summary>
    /// A configuration container for Add-In calculators. Each Calculation has its own parameters.
    /// </summary>
    [Serializable]
    public class Calculation
    {
        private Behavior _behavior = Calculation.Behavior.NotSet;
        /// <summary>
        /// Behavior of the addin (object to apply transformation)
        /// </summary>
        public enum Behavior
        {
            BatchProperty,
            CompoundProperty,
            StructureProperty,
            NotSet
        }

        private Transformation _transformation = Transformation.NotSet;
        /// <summary>
        /// List of transformation available
        /// </summary>
        public enum Transformation
        {
            /// <summary>
            /// Finds a structure name given the structure value
            /// </summary>
            FindNameByStructure,
            /// <summary>
            /// Finds the MW of the components (including salts + solvates)
            /// </summary>
            FormulaWeight,
            /// <summary>
            /// Finds the Mol Formulat (including salts + solvates)
            /// </summary>
            MolecularFormula,
            /// <summary>
            /// Finds the percentage of active sol
            /// </summary>
            PercentActive,
            /// <summary>
            /// Uses ChemScript to normalize all structures
            /// </summary>
            NormalizeStructure,
            /// <summary>
            /// Finds the logP (hydrophilicity)
            /// </summary>
            CLogP,
            /// <summary>
            /// Default value
            /// </summary>
            NotSet
        }

        /// <summary>
        /// Gets or sets the target, which is really whether 
        /// this is an action to be taken on a batch or a compound or none
        /// </summary>
        /// <value>The target.</value>
        public Behavior Target
        {
            get { return _behavior; }
            set { _behavior = value; }
        }

        /// <summary>
        /// Gets or sets the Action, which is the transformation you would like performed
        /// </summary>
        /// <value>The target.</value>
        public Transformation Action
        {
            get { return _transformation; }
            set { _transformation = value; }
        }

        private string _defaultText = string.Empty;
        /// <summary>
        /// The default value for the property
        /// </summary>
        public string DefaultText
        {
            get
            {
                return _defaultText;
            }
            set
            {
                _defaultText = value;
            }
        }

        private string _propertyName = string.Empty;
        /// <summary>
        /// The Name of the property that where the calculation will be written to.
        /// </summary>
        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
            set
            {
                _propertyName = value;
            }
        }

        private string _enabledString = Boolean.FalseString;
        /// <summary>
        /// Allows any given calculation to be bypassed
        /// </summary>
        public bool Enabled
        {
            get { return Convert.ToBoolean(_enabledString); }
            set { _enabledString = Convert.ToString(value); }
        }
    }
}
