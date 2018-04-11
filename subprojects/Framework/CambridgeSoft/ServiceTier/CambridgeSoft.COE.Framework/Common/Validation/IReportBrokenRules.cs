using System;
using System.Collections.Generic;

namespace CambridgeSoft.COE.Framework.Common.Validation
{
    /// <summary>
    /// Support custom broken rules maintenance for business objects
    /// </summary>
    public interface IReportBrokenRules
    {

        /// <summary>
        /// Form a list of broken rules from the current object and each child object
        /// </summary>
        /// <param name="brokenRules">A BrokenRuleDescription List to be appended</param>
        void GetBrokenRulesDescription(List<BrokenRuleDescription> brokenRules);
    }
}
