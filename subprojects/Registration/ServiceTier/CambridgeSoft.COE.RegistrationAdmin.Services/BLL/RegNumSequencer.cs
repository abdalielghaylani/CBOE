using System;
using System.Collections.Generic;
using System.Text;

using Csla;
using Csla.Data;
using Csla.Validation;

using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;

using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Validation;

/*
SEQUENCE_ID: PK
PREFIX: VARCHAR2(150), but should be maxlength 5
ACTIVE: NCHAR(1)
BATCHDELIMETER: VARCHAR2(1)
PREFIX_DELIMITER: VARCHAR2(1)
  DUP_CHECK_LOCAL: unused as this time
  SUFFIX: unused at this time
SUFFIXDELIMITER: VARCHAR2(1)
SALTSUFFIXTYPE: VARCHAR2(30)
  OBJECTTYPE: unused at this time
  EXAMPLE: should NOT be stored information
SITEID: NUMBER(8), but limits ths sequence to one site...is that a good idea?
TYPE: NCHAR(1)
BATCHNUMBER_LENGTH: NUMBER(1), with -1 being the 'no-padding' indicator
*/

/*
SEQUENCE_ID (ID)
PREFIX
NEXT_IN_SEQUENCE
ACTIVE
BATCHDELIMETER
PREFIX_DELIMITER
REGNUMBER_LENGTH
  DUP_CHECK_LOCAL
  SUFFIX
SUFFIXDELIMITER
SALTSUFFIXTYPE
OBJECTTYPE
EXAMPLE
SITEID
TYPE
BATCHNUMBER_LENGTH
*/

namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    /// <summary>
    /// Business class for managing the metadata responsible for creating a Registration Number.
    /// </summary>
    public class RegNumSequencer : RegAdminBusinessBase<RegNumSequencer>
    {
        #region [Properties]

        private string _prefix;
        public string Prefix
        {
            get { return _prefix; }
            set { _prefix = value; ValidationRules.CheckRules(); PropertyHasChanged(); }
        }

        //TODO: Add Name field to the Sequence table.
        //TODO: The default Name is the Prefix value.
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _prefixSeparator;
        public string PrefixSeparator
        {
            get { return _prefixSeparator; }
            set { _prefixSeparator = value; ValidationRules.CheckRules(); PropertyHasChanged(); }
        }

        private string _suffixSeparator;
        public string SuffixSeparator
        {
            get { return _suffixSeparator; }
            set { _suffixSeparator = value; ValidationRules.CheckRules(); PropertyHasChanged(); }
        }

        private NumericSequence _numberSequencer;
        public NumericSequence NumberSequencer
        {
            get { return _numberSequencer; }
            set { _numberSequencer = value; }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        private string _suffixGenerator;
        public string SuffixGenerator
        {
            get { return _suffixGenerator; }
            set { _suffixGenerator = value; }
        }

        private BatchNumberPadding _batchPadding;
        public BatchNumberPadding BatchPadding
        {
            get { return _batchPadding; }
            set { _batchPadding = value; }
        }

        public string Example
        {
            get { return BuildExample(); } 
        }

        #endregion

        #region [Validation Rules]

        /// <summary>
        /// Generate all the business rules required for an individual Sequence.
        /// </summary>
        protected override void AddInstanceBusinessRules()
        {
            RuleHandler rulehandler = new RuleHandler(ValidationRulesFactory.CoRequiredValues);
            RuleArgs ruleArgs = new CoRequiredRuleArgs("Prefix", "Prefix", "PrefixSeparator");
            this.ValidationRules.AddRule(rulehandler, ruleArgs);

            this.ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Prefix", 5));
            this.ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("PrefixSeparator", 5));
        }

        #endregion

        //This is temporary - we're going to convert this to using a metadata lookup table.
        public enum FragmentSuffixGenerator
        {
            None = -1,
            Code = 1,
            Formula = 2,
            InternalId = 3,
            Description = 4
        }
        public string TranslateSuffixSetting(FragmentSuffixGenerator generator)
        {
            string translatedValue = string.Empty;
            switch (generator)
            {
                case FragmentSuffixGenerator.Code:
                    {
                        translatedValue = "code";
                        break;
                    }
                case FragmentSuffixGenerator.Description:
                    {
                        translatedValue = "desc";
                        break;
                    }
                case FragmentSuffixGenerator.Formula:
                    {
                        translatedValue = "formula";
                        break;
                    }
                case FragmentSuffixGenerator.InternalId:
                    {
                        translatedValue = "id";
                        break;
                    }
                case FragmentSuffixGenerator.None:
                default:
                    {
                        break;
                    }
            }

            return translatedValue;
        }

        //This is temporary - we're going to convert this to using a metadata lookup table.
        public enum BatchNumberPadding
        {
            None = -1,
            TwoDigits = 2,
            ThreeDigits = 3,
            FourDigits = 4,
            FiveDigits = 5,
            SixDigits = 6
        }
        public int TranslateBatchPaddingSetting(BatchNumberPadding padding)
        {
            int translatedValue = -1;
            switch (padding)
            {
                case BatchNumberPadding.TwoDigits:
                    {
                        translatedValue = 2;
                        break;
                    }
                case BatchNumberPadding.ThreeDigits:
                    {
                        translatedValue = 3;
                        break;
                    }
                case BatchNumberPadding.FourDigits:
                    {
                        translatedValue = 4;
                        break;
                    }
                case BatchNumberPadding.FiveDigits:
                    {
                        translatedValue = 5;
                        break;
                    }
                case BatchNumberPadding.SixDigits:
                    {
                        translatedValue = 6;
                        break;
                    }
                case BatchNumberPadding.None:
                    {
                        translatedValue = -1;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return translatedValue;
        }

        #region [Fetch - ID]

        [Serializable()]
        protected class IDCriteria
        {
            private int _id;
            public int ID
            {
                get { return _id; }
            }

            public IDCriteria(int sequenceId) { _id = sequenceId; }
        }

        public static RegNumSequencer GetRegNumSequencer(int sequenceId)
        {
            try
            {
                return DataPortal.Fetch<RegNumSequencer>(new IDCriteria(sequenceId));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private void DataPortal_Fetch(IDCriteria criteria)
        {
            using (SafeDataReader sdr = this.RegDal.GetRegNumberSequencer(criteria.ID))
            {
                this.Fill(sdr);
            }
        }

        #endregion

        #region [Fetch - Prefix]

        //[Serializable()]
        //protected class PrefixCriteria
        //{
        //    private string _prefix;
        //    public int Prefix
        //    {
        //        get { return _prefix; }
        //    }

        //    public PrefixCriteria(string prefix) { _prefix = prefix; }
        //}

        //public static RegNumSequencer GetRegNumSequencer(string prefix)
        //{
        //    RegNumSequencer rns = new RegNumSequencer();
        //}
        
        #endregion

        #region [Fetch - Name]

        //[Serializable()]
        //protected class NameCriteria
        //{
        //    private string _name;
        //    public int Name
        //    {
        //        get { return _name; }
        //    }

        //    public NameCriteria(string name) { _name = name; }
        //}

        //public static RegNumSequencer GetRegNumSequencer(string name)
        //{
        //    RegNumSequencer rns = new RegNumSequencer();
        //}

        #endregion

        private void Fill(SafeDataReader reader)
        {
            while (reader.Read())
            {
                RegNumSequencer rns = new RegNumSequencer();
                this.ID = reader.GetInt32("ID");

                if (!reader.IsDBNull(reader.GetOrdinal("PREFIX")))
                    this._prefix = reader.GetString("PREFIX");

                //// The RegNumberSequencer NAME column doesn't exist yet
                //if (!reader.IsDBNull(reader.GetOrdinal("NAME")))
                //    this._prefix = reader.GetString("NAME");

                if (_numberSequencer == null)
                {
                    _numberSequencer = new NumericSequence();
                    //// The NumericSequence NAME column doesn't exist yet
                    //if (!reader.IsDBNull(reader.GetOrdinal("NAME")))
                    //    this._prefix = reader.GetString("NAME");

                    if (!reader.IsDBNull(reader.GetOrdinal("PREFIX")))
                        this._numberSequencer.Name = reader.GetString("PREFIX");

                    if (!reader.IsDBNull(reader.GetOrdinal("REGNUMBER_LENGTH")))
                    {
                        int padding = reader.GetInt32("REGNUMBER_LENGTH");
                        this._numberSequencer.RegPadding = (NumericSequence.RegNumberPadding)padding;
                    }
                }

                if (!reader.IsDBNull(reader.GetOrdinal("BATCHNUMBER_LENGTH")))
                {
                    int padding = reader.GetInt32("BATCHNUMBER_LENGTH");
                    this.BatchPadding = (BatchNumberPadding)padding;
                }
            }
        }

        private string BuildExample()
        {
            string sample = string.Empty;

            if (!string.IsNullOrEmpty(this.Prefix))
                sample += this.Prefix;
            if (!string.IsNullOrEmpty(this.PrefixSeparator))
                sample += this.PrefixSeparator;

            return sample;
        }

    }

}
