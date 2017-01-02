using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Registration.Core;

namespace CambridgeSoft.COE.Registration
{
    /// <summary>
    /// Holds the results of a search for mixtures with the same components.
    /// </summary>
    public class MixtureMatchResponse : IMatchResponse
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

        /// <summary>
        /// The mechanism that was used by the matching algorithm.
        /// </summary>
        public MatchMechanism MechanismUsed
        {
            get { return _mechanismUsed; }
        }

        public string DuplicateXML
        {
            get { return _duplicateXml; }
            set { _duplicateXml = value; }
        }

        #endregion

        /// <summary>
        /// Public constructor, accepting a list of Registration numbers (RegNum).
        /// </summary>
        /// <param name="matchedItems">the top-level RegNums of registrations</param>
        public MixtureMatchResponse(List<string> matchedItems)
        {
            _matchedItems = matchedItems;
            if (_matchedItems.Count > 1)
                _mechanismUsed = MatchMechanism.Mixture;
        }

        /// <summary>
        /// Public constructor, accepting a data-transfer object from the DAL
        /// </summary>
        /// <param name="matches">a value-only object from the DAL</param>
        public MixtureMatchResponse(MixtureMatchesDTO matches)
        {
            this._mechanismUsed = (MatchMechanism)matches.Mechanism;
            
            StringBuilder sb = new StringBuilder();
            sb.Append("<REGISTRYLIST>");
            foreach (MixtureMatchesDTO.MixtureMatchDTO dto in matches.Mixtures)
            {
                sb.Append(String.Format("<REGNUMBER count=\"1\"  SAMEFRAGMENT=\"True\" SAMEEQUIVALENT=\"True\">{0}</REGNUMBER>", dto.MixtureRegNumber));
                this._matchedItems.Add(dto.MixtureRegNumber);
            }
            sb.Append("</REGISTRYLIST>");
            _duplicateXml = sb.ToString();
            if (_matchedItems.Count > 1)
                _mechanismUsed = MatchMechanism.Mixture;
        }
    }
}
