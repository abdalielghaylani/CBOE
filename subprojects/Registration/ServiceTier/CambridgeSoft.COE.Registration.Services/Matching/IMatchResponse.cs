using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Registration
{
    /// <summary>
    /// Implementations must contain the salient points of information required by
    /// match-resolution mechanisms for both Compounds and Mixtures.
    /// </summary>
    public interface IMatchResponse
    {
        List<string> MatchedItems { get; }

        MatchMechanism MechanismUsed { get; }
        string DuplicateXML { get; set; }
    }
}
