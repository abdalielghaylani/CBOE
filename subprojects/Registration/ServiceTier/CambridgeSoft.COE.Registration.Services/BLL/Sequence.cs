using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Csla;
using Csla.Data;
using Csla.Validation;
using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using System.Data;
using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Registration.Access;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// This isn't a full-fledged business object: 'Sequence' data is handled via the
    /// Table Editor functionality. This object could simply be a property of the
    /// RegNumber object without the intention of using it for maintenance of system metadata.
    /// </summary>
    [Serializable()]
    public class Sequence : RegistrationBusinessBase<Sequence>
    {
        #region [ Factory Methods ]

        [Serializable()]
        private class Criteria
        {
            private int _id;
            public int Id
            {
                get { return _id; }
            }

            public Criteria(int id)
            { _id = id; }
        }


        [Serializable()]
        private class Criteria2
        {
            private int _id;
            public int Id
            {
                get { return _id; }
            }

            public Criteria2(int id)
            { _id = id; }
        }

        public static Sequence GetSequence(int id)
        {
            Sequence result = null;
            try
            {
                result = DataPortal.Fetch<Sequence>(new Criteria(id));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
            return result;
        }

        public static bool GetAutoSelCompDupChk(int id)
        {
            Sequence result = null;
            try
            {
                result = DataPortal.Fetch<Sequence>(new Criteria2(id));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
            return result._autoSelCompDupChk;
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
            using (SafeDataReader sdr = this.RegDal.GetSequence(criteria.Id))
            {
                this.Fill(sdr);
            }
        }

        private void Fill(SafeDataReader reader)
        {
            while (reader.Read())
            {
                Sequence s = new Sequence(
                    reader.GetInt32("ID"),
                    reader.GetString("PREFIX"),
                    reader.GetString("PREFIXDELIMITER"),
                    reader.GetInt32("REGNUMBERLENGTH"),
                    reader.GetString("SUFFIXDELIMITER"),
                    reader.GetString("SALTSUFFIXTYPE"),
                    reader.GetInt32("BATCHNUMLENGTH"),
                    reader.GetInt32("NEXTINSEQUENCE"),
                    reader.GetString("EXAMPLE"),
                    reader.GetString("ACTIVE"),
                    reader.GetString("TYPE"),
                    reader.GetString("AUTOSELCOMPDUPCHK")
                );
                s.MarkClean();
            }
        }

        private void DataPortal_Fetch(Criteria2 criteria)
        {
            using (SafeDataReader sdr = this.RegDal.GetSequence(criteria.Id))
            {
                this.Fill2(sdr);
            }
        }

        private void Fill2(SafeDataReader reader)
        {
            while (reader.Read())
            {
                COEDALBoolean trueFalse = (COEDALBoolean)Enum.Parse(typeof(COEDALBoolean), reader.GetString("AUTOSELCOMPDUPCHK"), true);
                this._autoSelCompDupChk = (trueFalse.Equals(COEDALBoolean.T)); 
             
               
               
            }
        }

        private Sequence(
            int id, string prefix, string prefixDelimiter, int regNumberLength
            , string suffixDelimiter, string suffixType, int batchNumberlength, int nextInSequence
            , string example, string isActive, string sequenceType, string autoSelCompDupChk
            )
        {
            this.ID = id;
            this._prefix = prefix;
            this._prefixDelimeter = prefixDelimiter;
            this._regNumberLength = regNumberLength;
            this._suffixDelimeter = suffixDelimiter;
            this._suffix = suffixType;
            this._batchNumberLength = batchNumberlength;
            this._nextInSequence = nextInSequence;
            this._example = example;
            COEDALBoolean trueFalse = (COEDALBoolean)Enum.Parse(typeof(COEDALBoolean), autoSelCompDupChk, true);
            this._autoSelCompDupChk = (trueFalse.Equals(COEDALBoolean.T)); 
            COEDALBoolean trueFalse2 = (COEDALBoolean)Enum.Parse(typeof(COEDALBoolean), isActive, true);
            this._active = (trueFalse2.Equals(COEDALBoolean.T));
            this._sequenceType = (ProjectList.ProjectTypeEnum)Enum.Parse(typeof(ProjectList.ProjectTypeEnum), sequenceType, true);
        }

        [COEUserActionDescription("CreateSequence")]
        public static Sequence NewSequence(string xml, bool isClean)
        {
            try
            {
                return new Sequence(xml, isClean);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateSequence")]
        public static Sequence NewSequence(int sequenceID)
        {
            try
            {
                return new Sequence(sequenceID, true);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateSequence")]
        public static Sequence NewSequence(int sequenceID, int rootNumber, string prefix)
        {
            try
            {
                return new Sequence(sequenceID, rootNumber, prefix);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private Sequence()
        {
            return;
        }

        private Sequence(bool isClean)
        {
            if (isClean)
                MarkClean();
        }

        private Sequence(int sequenceID, bool isClean)
            : this(isClean)
        {
            this.ID = sequenceID;
        }

        private Sequence(int sequenceID, int regNumberLength, string prefix)
        {
            this.ID = sequenceID;
            _regNumberLength = regNumberLength;
            _prefix = prefix;
        }

        private Sequence(string xml, bool isClean)
            : this(isClean)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml.ToString()));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("Sequence/Prefix");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _prefix = xIterator.Current.Value;

            xIterator = xNavigator.Select("Sequence/Suffix");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _suffix = xIterator.Current.Value;

            xIterator = xNavigator.Select("Sequence/PrefixDelimiter");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _prefixDelimeter = xIterator.Current.Value;

            xIterator = xNavigator.Select("Sequence/SuffixDelimiter");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _suffixDelimeter = xIterator.Current.Value;

            xIterator = xNavigator.Select("Sequence/RootNumberLength");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _regNumberLength = xIterator.Current.ValueAsInt;

            xIterator = xNavigator.Select("Sequence/Active");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _active = xIterator.Current.Value.ToUpper() == "T" ? true : false;

            xIterator = xNavigator.Select("Sequence/AutoSelCompDupChk");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _autoSelCompDupChk = xIterator.Current.Value.ToUpper() == "T" ? true : false;

            xIterator = xNavigator.Select("Sequence/ID");
            if (xIterator.MoveNext() && !string.IsNullOrEmpty(xIterator.Current.InnerXml))
                this.ID = xIterator.Current.ValueAsInt;
        }

        #endregion

        #region [ Authorization and Validation Rules ]

        protected override void AddAuthorizationRules()
        {
            AuthorizationRules.AllowWrite(
              "Sequence", "ADD_IDENTIFIER");
        }

        public static bool CanAddObject()
        {
            //return Csla.ApplicationContext.User.IsInRole("sequence");
            return true;
        }

        public static bool CanGetObject()
        {
            return true;
        }

        public static bool CanDeleteObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("sequence");
        }

        public static bool CanEditObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("sequence");
        }

        protected override void AddBusinessRules()
        {
            ValidationRules.AddRule(CommonRules.StringRequired, "Description");
            ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs("Description", 100));
        }

        #endregion

        #region [ Properties and members ]

        private bool _active = true;
        public bool Active
        {
            get { return _active; }
        }

        private bool _autoSelCompDupChk = false;
        public bool AutoSelCompDupChk
        {
            get { return _autoSelCompDupChk; }
        }

        private ProjectList.ProjectTypeEnum _sequenceType;
        public ProjectList.ProjectTypeEnum SequenceType
        {
            get { return _sequenceType; }
        }

        private int _regNumberLength;
        /// <summary>
        /// Root number length of the given secuence
        /// </summary>
        public int RegNumberLength
        {
            get { return _regNumberLength; }
        }

        private int _nextInSequence = 1;
        public int NextInSequence
        {
            get { return _nextInSequence; }
        }

        private string _prefix = string.Empty;
        /// <summary>
        /// Prefix of the given sequence
        /// </summary>
        public string Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        private string _prefixDelimeter = string.Empty;
        /// <summary>
        /// Prefix delimiter of the given sequence
        /// </summary>
        public string PrefixDelimiter
        {
            get { return _prefixDelimeter; }
            set { _prefixDelimeter = value; }
        }

        private string _suffix = string.Empty;
        /// <summary>
        /// Indicates the Fragment attribute are used in Batch registration numbers.
        /// </summary>
        public string Suffix
        {
            get { return _suffix; }
            set { _suffix = value; }
        }

        private string _suffixDelimeter = string.Empty;
        /// <summary>
        /// The spacer character between the padded 'Next In Sequence' value and the suffix.
        /// </summary>
        public string SuffixDelimiter
        {
            get { return _suffixDelimeter; }
            set { _suffixDelimeter = value; }
        }

        private int _batchNumberLength = -1;
        /// <summary>
        /// The total length of the Batch number during Batch Registration Number generation.
        /// </summary>
        /// <remarks>A value of -1 indicates no left-padding of zeroes should be used.</remarks>
        public int BatchNumberLength
        {
            get { return _batchNumberLength; }
            set { _batchNumberLength = value; }
        }

        private string _example = string.Empty;
        /// <summary>
        /// A user-provided sample registration number; a memo field.
        /// </summary>
        public string Example
        {
            get { return _example; }
        }



        public override bool IsValid
        {
            //get { return base.IsValid && _resources.IsValid; }
            get { return base.IsValid; }
        }

        public override bool IsDirty
        {
            get { return base.IsDirty; }
            //get { return base.IsDirty || _resources.IsDirty; }
        }

        #endregion

        internal string UpdateSelf(bool addCRUDattributes)
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append("<SequenceID>" + this.ID.ToString() + "</SequenceID>");
            return builder.ToString();
        }

    }
}
