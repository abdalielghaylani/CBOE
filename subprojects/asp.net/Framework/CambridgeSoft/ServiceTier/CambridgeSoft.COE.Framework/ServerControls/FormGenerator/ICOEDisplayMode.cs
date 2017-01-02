using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This interface defines the properties that are usefull for those controls that are intended to
    /// know the display mode when they are in creation.
    /// </para>
    /// </summary>
    public interface ICOEDisplayMode
    {
        /// <summary>
        /// <para>
        /// Gets or set the container's display mode. Every container should allow one of the following modes:
        /// </para>
        /// <list type="bullet">
        ///     <item>Add</item>
        ///     <item>Edit</item>
        ///     <item>Query</item>
        ///     <item>View</item>
        /// </list>
        /// </summary>
        CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.DisplayMode DisplayMode { get; set; }
    }
}
