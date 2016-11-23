using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace CambridgeSoft.COE.Framework.Common.IniParser
{
    /// <summary>
    /// The object-oriented representation of the legacy INI settings. It also contains
    /// methods to parse the INI settings as well as methods to manipulate them.
    /// </summary>
    [Serializable]
    public class IniSettingInfo
    {
        private StreamReader _reader = null;
        private IDictionary<string, IniSection> _iniSections = null;
        private IList<string> _validationErrorMessages = null;
        private ValidateStatus _validateStatus = ValidateStatus.NotValidated;
        public const string DUPLICATE_SECTIONS_MESSAGE = "Sections [{0}] has {1} duplicates found";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="iniStream">The original INI settings in the form of a stream. It 
        /// could be a file stream read from disk file, or a memory stream read from
        /// database, and so forth.</param>
        public IniSettingInfo(Stream iniStream)
        {
            _reader = new StreamReader(iniStream);
            _validationErrorMessages = new List<string>();
            _iniSections = new Dictionary<string, IniSection>();
        }

        #region Properties

        public IDictionary<string, IniSection> IniSections
        {
            get { return _iniSections; }
        }

        public IList<string> ValidationErrorMessages
        {
            get { return _validationErrorMessages; }
        }

        public bool IsValid
        {
            get
            {
                // As it's not possible to change the INI stream once it's constructed, we only need to do the validation ONCE
                // for each INI stream.
                if (_validateStatus == ValidateStatus.NotValidated)
                {
                    Validate();
                    _validateStatus = _validationErrorMessages.Count == 0 ? ValidateStatus.Valid : ValidateStatus.Invalid;
                }

                return _validateStatus == ValidateStatus.Valid;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Provides the underlying parsing functionality for instances of this class. Generates
        /// section entries in a list, and populates each section with its own dictionary of
        /// INI settings.
        /// </summary>
        /// <param name="iniStream">Stream containing the INI file's contents to parse</param>
        public void Parse()
        {
            if (!IsValid)
            {
                throw new Exception("Unable to continue parsing. The input INi is not valid");
            }

            ParseToSections();
            ParseSectionSelf();
        }

        /// <summary>
        /// The calling code doesn't call this method explicitly. Instead, it simply checks the value of IsValid property and,
        /// if it's false, reads the ValidationErrorMessages property to get the error messages.
        /// </summary>
        private void Validate()
        {
            Rewind();
            ValidateSections();
            // Only validate ini settings inside each section when all sections are valid.
            if (_validationErrorMessages.Count == 0)
            {
                ParseToSections();
                ValidateSectionSelf();
            }
        }

        /// <summary>
        /// Checks if there're duplicate section names.
        /// </summary>
        private void ValidateSections()
        {
            _validationErrorMessages.Clear();

            Dictionary<string, int> sectionNames = ExtractSectionNames();
            foreach (KeyValuePair<string, int> item in sectionNames)
            {
                if (item.Value > 1)
                {
                    _validationErrorMessages.Add(string.Format(DUPLICATE_SECTIONS_MESSAGE, item.Key, item.Value));
                }
            }
        }

        private void ParseToSections()
        {
            Rewind();
            _iniSections.Clear();

            string lineData = string.Empty;
            List<string> sectionRawLines = new List<string>();
            string currentSectionName = string.Empty;

            while ((lineData = _reader.ReadLine()) != null)
            {
                string line = lineData.Trim();

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    if (sectionRawLines.Count > 0)
                    {
                        // Conclue the previous section, by constructing an IniSection object and adding it to the
                        // global section repository.
                        ExtractSection(currentSectionName, sectionRawLines);

                        // Reset temp storage.
                        sectionRawLines = new List<string>();
                    }

                    sectionRawLines.Add(line);
                    currentSectionName = line.Substring(1, line.Length - 2).Trim();
                }
                else
                {
                    sectionRawLines.Add(line);
                }
            }

            ExtractSection(currentSectionName, sectionRawLines);
        }

        private void ValidateSectionSelf()
        {
            foreach (IniSection iniSection in IniSections.Values)
            {
                iniSection.Validate();
                if (iniSection.ValidationErrorMessage.Count != 0)
                {
                    ((List<string>)_validationErrorMessages).AddRange(iniSection.ValidationErrorMessage);
                }
            }
        }

        private void ExtractSection(string sectionName, List<string> sectionRawLines)
        {
            IniSection newSection = IniSection.CreateIniSection(sectionName, sectionRawLines);

            if (_iniSections.ContainsKey(sectionName))
            {
                // The validity check is supposed to have already been done, so it's theoretically not possible to encounter
                // duplicate section name inside this method.
                throw new Exception("Fatal error: duplicate section names encountered, which is not supposed to happen");
            }

            _iniSections.Add(sectionName, newSection);
        }

        /// <summary>
        /// Applies any section-specific parsing logic.
        /// </summary>
        private void ParseSectionSelf()
        {
            foreach (IniSection section in this._iniSections.Values)
            {
                section.Parse();
            }
        }

        private void Rewind()
        {
            _reader.DiscardBufferedData();
            _reader.BaseStream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Extracts out only section names and the counts of each name.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, int> ExtractSectionNames()
        {
            Dictionary<string, int> sectionNames = new Dictionary<string, int>();

            string lineData = null;
            string currentSectionName = string.Empty;

            while ((lineData = _reader.ReadLine()) != null)
            {
                string line = lineData.Trim();

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSectionName = line.Substring(1, line.Length - 2).Trim();
                    if (!sectionNames.ContainsKey(currentSectionName))
                    {
                        sectionNames.Add(currentSectionName, 1);
                    }
                    else
                    {
                        sectionNames[currentSectionName]++;
                    }
                }
            }

            return sectionNames;
        }

        #endregion

        private enum ValidateStatus
        {
            Invalid,
            Valid,
            NotValidated
        }
    }
}
