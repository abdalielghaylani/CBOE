using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using ChemDrawControl18;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UI;

namespace CambridgeSoft.COE.Framework.Controls.Reporting
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(XRChemDrawEmbed))]
    [DefaultBindableProperty("InlineData")]
    public class XRChemDrawEmbed : XRControl
    {
        #region Constants
        private const float PixelsPerInch = 72.0f;
        private const double MaxZoomInFactor = 1.0f;
        #endregion

        #region Variables
        private string _inlineData = string.Empty;
        private bool _shrinkToFit = true;
        private bool _enlargeToFit = false;

        private static ChemDrawCtlClass _chemDrawControl = new ChemDrawCtlClass();
        #endregion

        #region Properties
        [Bindable(true)]
        [Description("Structure goes here!")]
        [SRCategory(ReportStringId.CatData)]
        public string InlineData
        {
            get
            {
                return _inlineData;
            }
            set
            {
                _inlineData = value;
            }
        }

        [Bindable(true)]
        [Description("Structure goes here!")]
        [SRCategory(ReportStringId.CatData)]
        public override string Text
        {
            get
            {
                return _inlineData;
            }
            set
            {
                _inlineData = value;
            }
        }
        
        [Bindable(true)]
        [Description("Should the structure be zoomed out to fit the control?")]
        [SRCategory(ReportStringId.CatBehavior)]
        public bool ShrinkToFit
        {
            get
            {
                return _shrinkToFit;
            }
            set
            {
                _shrinkToFit = value;
            }
        }

        [Bindable(true)]
        [Description("Should the structure be zoomed in to fit the control size?")]
        [SRCategory(ReportStringId.CatBehavior)]
        public bool EnlargeToFit
        {
            get
            {
                return _enlargeToFit;
            }
            set
            {
                _enlargeToFit = value;
            }
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
            base.PutStateToBrick(brick, ps);

            ImageBrick imageBrick = (ImageBrick)brick;

            if (!string.IsNullOrEmpty(_inlineData))
            {
                float pixelsPerInch = PixelsPerInch;

                //ReportUnit.TenthsOfAMillimeter
                float coeficient = pixelsPerInch * 0.1f / 25.4f; 

                if (((XtraReport)this.RootReport).ReportUnit == ReportUnit.HundredthsOfAnInch)
                {
                    coeficient = pixelsPerInch * 0.01f;
                }

                imageBrick.Image = GetStructureImage(_inlineData, HeightF * coeficient, this.WidthF * coeficient, pixelsPerInch);
                imageBrick.SizeMode = DevExpress.XtraPrinting.ImageSizeMode.CenterImage;
            }
        }
        /// <summary>
        /// Obtains an image out of a base64 cdx, a height, a width and a resolution.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="_inlineData"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        private System.Drawing.Image GetStructureImage(string _inlineData, float height, float width, float resolution)
        {
            _chemDrawControl.Objects.Clear();
            _chemDrawControl.DataEncoded = true;
            _chemDrawControl.set_Data("chemical/x-cdx", _inlineData);

            Preferences preferences = _chemDrawControl.Application.Preferences;
            _chemDrawControl.DataEncoded = false;
            _chemDrawControl.ViewOnly = false;
            preferences.AntialiasedGIFs = true;

            _chemDrawControl.Objects.Select();
            double horizontalZoom = height / _chemDrawControl.Objects.Height;
            double verticalZoom = width / _chemDrawControl.Objects.Width;

            double scale = Math.Min(horizontalZoom, verticalZoom);

            if (!_shrinkToFit)
                scale = Math.Max(1.0, scale);

            if (!_enlargeToFit)
                scale = Math.Min(1.0, scale);

            _chemDrawControl.Objects.Scale(scale, true, true);

            byte[] imageByteArray = (byte[])_chemDrawControl.get_Data("bmp");
            
            //Unscale as chemdrawcontrol doesn't clear the scale
            _chemDrawControl.Objects.Scale(1.0 /scale, true, true);

            return new System.Drawing.Bitmap(new MemoryStream(imageByteArray));
        }
        #endregion
    }
}
