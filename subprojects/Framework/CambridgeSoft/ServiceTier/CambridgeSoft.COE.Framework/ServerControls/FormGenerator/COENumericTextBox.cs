using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Globalization;
using CambridgeSoft.COE.Framework.Common;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// Specific text box for numbers. Specially designed to be aware of culture info.
    /// See <see cref="COETextBox"/> for further details.
    /// </summary>
    [ToolboxData("<{0}:COENumericTextBox runat=server></{0}:COENumericTextBox>")]
    public class COENumericTextBox : COETextBox, ICOECultureable
    {
        /// <summary>
        /// Gets the number in invariant culture.
        /// </summary>
        /// <returns></returns>
        public override object GetData()
        {
            return GlobalizationHelper.FormatData(this.Text, COEDataView.AbstractTypes.Real, this.DisplayCulture.NumberFormat, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <summary>
        /// Sets the data and to be displayed in a specific culture.
        /// </summary>
        /// <param name="data"></param>
        public override void PutData(object data)
        {
            this.Text = GlobalizationHelper.FormatData(data.ToString(), COEDataView.AbstractTypes.Real, CultureInfo.InvariantCulture.NumberFormat, this.DisplayCulture.NumberFormat);
        }

        #region ICOECultureable Members
        /// <summary>
        /// Gets or sets the Display Culture.
        /// </summary>
        public CultureInfo DisplayCulture
        {
            set { ViewState["DisplayCulture"] = value; }
            get { return ViewState["DisplayCulture"] != null ? ViewState["DisplayCulture"] as CultureInfo : CultureInfo.GetCultureInfoByIetfLanguageTag(Page.Session["DisplayCulture"].ToString()); }
        }
        #endregion
    }
}
