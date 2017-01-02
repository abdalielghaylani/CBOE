using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.Exceptions
{
    public interface ICOEException
    {
        string ToShortErrorString();
        string ToErrorString(bool includeInnerMessage);
    }
}
