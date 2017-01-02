using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This interface is intended to give the controls the ability to have a help mechanism .
    /// Two properties are to be implemented, ShowHelp and HelpText. It is up to the implementing control to render the help
    /// in whatever way it wants.
    /// </para>
    /// </summary>
    public interface ICOEHelpContainer
    {
        bool ShowHelp { get; set; }
        string HelpText { get; set; }
    }
}
