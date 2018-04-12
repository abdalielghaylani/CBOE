using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.DataLoader.Core
{
    /// <summary>
    /// Each instance of ISourceRecord will have an instance of this class. It holds the
    /// underlying data derived, typically, from a single record-parsing event. The scope
    /// of this information is at the level of the record.
    /// </summary>
    public class FieldValues : Dictionary<string, object>
    {
        //NOTE: There is currently no need to define any additional functionality to
        //      this Dictionary.
    }
}
