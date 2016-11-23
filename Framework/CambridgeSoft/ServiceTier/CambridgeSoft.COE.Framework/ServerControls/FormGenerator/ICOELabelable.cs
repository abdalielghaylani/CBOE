using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator {
    /// <summary>
    /// <para>
    /// This interface is intended to give the controls the ability to have a label.
    /// Along with a Property called Label another one is provided, called LabelCSSClass, to allow different css classes
    /// for the control itself and for the label.
    /// </para>
    /// </summary>
    public interface ICOELabelable {
        /// <summary>
        /// Gets or sets the Control's label.
        /// </summary>
        string Label {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Control's label CSS styles attributes.
        /// The key element of the Dictionary holds a CSS attribute name and the value is the attribute's value
        /// </summary>
        Dictionary<string, string> LabelStyles
        {
            get;
            set;
        }

     }
}
