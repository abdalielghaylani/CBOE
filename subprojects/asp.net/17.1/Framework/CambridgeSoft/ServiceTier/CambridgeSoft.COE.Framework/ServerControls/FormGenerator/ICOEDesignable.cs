using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator {
    /// <summary>
    /// <para>
    /// This interface should be implemented for those controls that allows to be configured at design time, whether in a FormDesigner/Wizard orf
    /// by a separate service.
    /// </para>
    /// </summary>
    public interface ICOEDesignable {
        /// <summary>
        /// <para>
        /// This method should return the configuration info for a specific object. This string would usually be in the form:
        /// &lt;fieldConfig&gt;<b>custom information in xml style.</b>&lt;fieldConfig&gt;
        /// </para>
        /// </summary>
        /// <returns>The string representation of the control's config info.</returns>
        XmlNode GetConfigInfo();
    }
}
