using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
 /// <summary>
 /// Added new interface to set Editable property for all controls
 /// </summary>
    public interface ICOEReadOnly
    {
        /// <summary>
        /// Property to set/get the edit behaviour
        /// </summary>
        COEEditControl COEReadOnly
        {
            get;
            set;
        }

    }
/// <summary>
/// Enum to set the different editing behaviour to all framework controls
/// </summary>
    public enum COEEditControl
    {
        NotSet,
        ReadOnly,
        Edit
    }
}
