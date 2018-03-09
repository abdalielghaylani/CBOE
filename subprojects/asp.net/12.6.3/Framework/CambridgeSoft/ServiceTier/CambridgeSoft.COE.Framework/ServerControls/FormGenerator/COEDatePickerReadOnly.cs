using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This the read only version of <see cref="COEDatePicker"/> class.
    /// </para>
    /// <para>
    /// The COEDatePickerReadOnly class accepts every WebDateChooser property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// All dates are stored internally as Universal Time amd converted based on <see cref="Constants.DatesFormat"/>.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: What is the default date?</item>
    ///         <item>bindingExpression: What is the binding Attribute, relative to the datasource, of the formgenerator.</item>
    ///         <item>label: What is its label?</item>
    ///         <item>configInfo: Additional attributes like CSSClass, Style, Width, Height, ID...</item>
    ///     </list>
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// </para>
    /// <b>With XML:</b>
    /// <code lang="XML">
    ///   &lt;formElement&gt;
    ///     &lt;label&gt;Date Created&lt;/label&gt;
    ///     &lt;bindingExpression&gt;&lt;/bindingExpression&gt;
    ///     &lt;configInfo&gt;
    ///       &lt;fieldConfig&gt;
    ///       &lt;CSSClass&gt;COEDropDownList&lt;/CSSClass&gt;
    ///       &lt;CSSLabelClass&gt;COELabel&lt;/CSSLabelClass&gt;
    ///       &lt;ID&gt;DateCreatedTextBox&lt;/ID&gt;
    ///       &lt;NullDateLabel&gt;&lt;/NullDateLabel&gt;
    ///       &lt;Width&gt;150&lt;/Width&gt;
    ///       &lt;Height&gt;15&lt;/Height&gt;
    ///       &lt;FontSize&gt;11px&lt;/FontSize&gt;
    ///       &lt;ForeColor&gt;484848&lt;/ForeColor&gt;
    ///       &lt;BackColor&gt;EAEAEA&lt;/BackColor&gt;
    ///       &lt;FontNames&gt;Verdana&lt;/FontNames&gt;
    ///       &lt;/fieldConfig&gt;
    ///     &lt;/configInfo&gt;
    ///     &lt;displayInfo&gt;
    ///       &lt;height&gt;15px&lt;/height&gt;
    ///       &lt;width&gt;212px&lt;/width&gt;
    ///       &lt;top&gt;50px&lt;/top&gt;
    ///       &lt;left&gt;349px&lt;/left&gt;
    ///       &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePickerReadOnly&lt;/type&gt;
    ///     &lt;/displayInfo&gt;
    ///   &lt;/formElement&gt;
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// In this implementation "Default Value", GetData and PutData Methods reffer to the Value property.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COEDatePickerReadOnly runat=server></{0}:COEDatePickerReadOnly>")]
    public class COEDatePickerReadOnly : COEDatePicker
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="COEDatePickerReadOnly"/> class
        /// </summary>
        public COEDatePickerReadOnly() : base()
        {
            this.ShowDropDown = false;
            this.Enabled = false;
            this.HideDropDowns = true;
            this.DropButton.Style.Width = new Unit("0px");
            this.ReadOnly = true;
        }
        #endregion

    }
}
