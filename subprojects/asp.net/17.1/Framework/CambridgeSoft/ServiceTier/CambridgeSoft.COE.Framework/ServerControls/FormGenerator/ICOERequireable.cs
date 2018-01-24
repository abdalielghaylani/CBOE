using System;
using System.Web.UI;


namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator {
    /// <summary>
    /// <para>
    /// This interface is intended to give the controls the ability to be marked as required.
    /// Each control may use this interface to be rendered in some different way if the control is required.
    /// </para>
    /// </summary>
    public interface ICOERequireable {
        /// <summary>
        /// Gets or sets if the control is required to have a value.
        /// </summary>
        bool Required {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the CSS styles for the ICOERequireable control
        /// The style string accepts a list of css attibutes that allow the customization of 
        /// the ICOERequireable control's appearence
        /// </summary>
        string RequiredStyle {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the CSS style for the ICOERequireable control's label
        /// The style string accepts a list of css attibutes that allow the customization of 
        /// the ICOERequireable control's label appearence
        /// </summary>
        string RequiredLabelStyle {
            get;
            set;
        }

    }
}
