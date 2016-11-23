using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CambridgeSoft.COE.Framework.Common.IniParser
{
    /// <summary>
    /// This class represents regular INI sections that have only normal [key]=[value] items
    /// that don't need any further interpretations.
    /// </summary>
    public class IniRegularSection : IniSection
    {
        public IniRegularSection(string sectionName, IList<string> rawLines)
            : base(sectionName, rawLines)
        { }
    }
}
