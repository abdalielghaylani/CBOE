using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.Registration.Core;

namespace CambridgeSoft.COE.Registration
{
    /// <summary>
    /// Holds the results of a search for compounds having either the same structure or
    /// extensible property value. The mechanism that was used to perform the matching
    /// is returned.
    /// </summary>
    public class CompoundMatchResponse : IMatchResponse
    {
        private List<string> _matchedItems = new List<string>();
        private MatchMechanism _mechanismUsed = MatchMechanism.None;
        private string _duplicateXml = string.Empty;

        #region [IMatchResponse Members]

        /// <summary>
        /// The list of IDs constituting matches.
        /// </summary>
        public List<string> MatchedItems
        {
            get { return _matchedItems; }
        }
        public string DuplicateXML
        {
            get { return _duplicateXml; }
            set { _duplicateXml = value; }
        }

        /// <summary>
        /// The mechanism that was used by the matching algorithm.
        /// </summary>
        public MatchMechanism MechanismUsed
        {
            get { return _mechanismUsed; }
        }

        #endregion

        /// <summary>
        /// Alternative constructor
        /// </summary>
        /// <param name="matchedItems">
        /// A list of Registration Numbers, either at the registry-level or component-level
        /// </param>
        /// <param name="mechanism">
        /// the mechanism that was used to determine if registered matches exist
        /// </param>
        public CompoundMatchResponse(List<string> matchedItems, MatchMechanism mechanism)
        {
            _matchedItems = matchedItems;
            _mechanismUsed = mechanism;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="matches">
        /// A data-transfer object intended solely to shuttle data from the data-acces slayer
        /// </param>
        public CompoundMatchResponse(CompoundMatchesDTO matches)
        {
            this._mechanismUsed = (MatchMechanism)matches.Mechanism;
            StringBuilder sb = new StringBuilder();
            sb.Append("<COMPOUNDLIST><COMPOUND><TEMPCOMPOUNDID>{0}</TEMPCOMPOUNDID><REGISTRYLIST>");
            foreach (CompoundMatchesDTO.CompoundMatchDTO dto in matches.Compounds)
            {
                sb.Append(String.Format("<REGNUMBER count=\"1\" CompoundID=\"{0}\" SAMEFRAGMENT=\"True\" SAMEEQUIVALENT=\"True\">{1}</REGNUMBER>", dto.CompoundRegistrationId, dto.MixtureRegNumber));
                this._matchedItems.Add(dto.MixtureRegNumber);
            }
            sb.Append("</REGISTRYLIST></COMPOUND></COMPOUNDLIST>");
            _duplicateXml = sb.ToString();
        }

    }
}
