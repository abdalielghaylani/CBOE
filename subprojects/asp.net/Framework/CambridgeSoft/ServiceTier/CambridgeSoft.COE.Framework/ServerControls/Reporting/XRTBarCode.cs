using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UI;

namespace CambridgeSoft.COE.Framework.Controls.Reporting
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(XRTBarCode))]
    [DefaultBindableProperty("Text")]
    public class XRTBarCode : XRControl
    {
        #region Variables
        TECIT.TBarCode.Barcode _barcode;
        #endregion

        #region Properties
        [Bindable(true)]
        [Description("Structure goes here!")]
        [SRCategory(ReportStringId.CatData)]
        public string Text
        {
            get {
                return BarCode.Data;
            }
            set {
                BarCode.Data = value;
            }
        }

        [Bindable(false)]
        [Description("Structure goes here!")]
        [SRCategory(ReportStringId.CatData)]
        public TECIT.TBarCode.Barcode BarCode
        {
            get
            {
                if (_barcode == null)
                    _barcode = new TECIT.TBarCode.Barcode();

                return _barcode;
            }
            set { _barcode = value; }
        }
        #endregion

        #region Control Life-Cycle
        /// <summary>
        /// DevExpress XtraReports required method. Returns the brick (in this case ImageBrick) that will be used for this control.
        /// </summary>
        /// <param name="childrenBricks"></param>
        /// <returns>an ImageBrick to be used for the current XRChemDrawEmbed</returns>
        protected override DevExpress.XtraPrinting.VisualBrick CreateBrick(DevExpress.XtraPrinting.VisualBrick[] childrenBricks)
        {
                return new ImageBrick(this);
        }
        /// <summary>
        /// DevExpress XtraReports required method. This does the actual work of generating the structure image and assigning it to the ImageBrick.
        /// </summary>
        /// <param name="brick"></param>
        /// <param name="ps"></param>
        protected override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps)
        {
            ImageBrick imageBrick = (ImageBrick)brick;

            try
            {
                base.PutStateToBrick(brick, ps);

                imageBrick.Image = BarCode.DrawBitmap((int)this.WidthF, (int)this.HeightF);
                imageBrick.SizeMode =DevExpress.XtraPrinting.ImageSizeMode.CenterImage;
            }
            catch (Exception exception)
            {
                ImageBrick imagebrick = (ImageBrick)brick;
                imagebrick.Text = exception.Message;
            }
        }
        #endregion
    }
}
