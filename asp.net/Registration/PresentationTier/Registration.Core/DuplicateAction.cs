using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Registration
{
    public enum DuplicateAction
    {
        Duplicate = 1,  // "D" Create duplicate
        Batch,          // "B" Add batch to existing
        Temporary,      // "T" Put into temporary
        None,           // "N" Do not store, duplicate is ignored
        Compound,       // "C" Unique Compound created (used only for display)
    }
}
