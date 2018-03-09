using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.COEDisplayDataBrokerService
{
    public interface IKeyValueListHolder
    {
        System.Collections.IDictionary KeyValueList
        {
            get;
        }
    }
}
