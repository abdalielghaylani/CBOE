using System;
using System.Globalization;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This interface should be implemented for those controls that may be configured for using different culture info, like a Date picker.
    /// It only provides a DisplayCulture property, which accepts a CultureInfo Property.
    /// </para>
    /// </summary>
    public interface ICOECultureable
    {
        /// <summary>
        /// Sets the culture info for the underlying control.
        /// <see cref="CultureInfo"/>
        /// </summary>
        CultureInfo DisplayCulture
        {
            set;
        }
    }
}