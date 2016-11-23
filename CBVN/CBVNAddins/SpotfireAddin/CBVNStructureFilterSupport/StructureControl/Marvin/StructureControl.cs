// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureControl.cs" company="PerkinElmer Inc.">
//   Copyright © 2012 PerkinElmer Inc. 
// 100 CambridgePark Drive, Cambridge, MA 02140. 
// All rights reserved. 
// This software is the confidential and proprietary information 
// of PerkinElmer Inc. ("Confidential Information"). You shall not 
// disclose such Confidential Information and may not use it in any way, 
// absent an express written license agreement between you and PerkinElmer Inc. 
// that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CBVNStructureFilterSupport.Marvin
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using CBVNStructureFilterSupport.Framework;

    internal class StructureControl : StructureControlBase
    {
        #region Constants and Fields

        private static bool? isInstalled;

        private Image cachedImage;

        private MarvinHydrogenDisplayMode hydrogenDisplayMode = MarvinHydrogenDisplayMode.HeteroOrTerminal;

        private JavaHostInstance javaHostInstance = new JavaHostInstance();

        private string stringData;

        #endregion

        #region Constructors and Destructors

        internal StructureControl(bool viewOnly)
            : base(ControlType.Marvin, viewOnly)
        {
            this.Resize += this.StructureControl_Resize;
        }

        #endregion

        #region Enums

        private enum MarvinHydrogenDisplayMode
        {
            Off, 
            Hetero, 
            HeteroOrTerminal, 
            All
        }

        #endregion

        #region Properties

        internal static bool IsInstalled
        {
            get
            {
                if (isInstalled == null)
                {
                    isInstalled = JavaHostInstance.IsInstalled;
                }

                return isInstalled.Value;
            }
        }

        internal override string HydrogenDisplayMode
        {
            set
            {
                this.hydrogenDisplayMode = StructureControl.GetClosestHydrogenDisplayModeValue(value);
                this.RedrawStructure();
            }
        }

        internal override string[] HydrogenDisplayModes
        {
            get
            {
                return Enum.GetNames(typeof(MarvinHydrogenDisplayMode));
            }
        }

        internal override Image Image
        {
            get
            {
                if (this.stringData != null)
                {
                    return this.javaHostInstance.StructureToImage(
                        this.stringData, this.Width, this.Height, this.hydrogenDisplayMode.ToString());
                }

                return null;
            }
        }

        internal override string MolFileString
        {
            get
            {
                if (this.stringData != null)
                {
                    return this.javaHostInstance.StructureToMolfile(this.stringData);
                }

                return null;
            }

            set
            {
                this.stringData = value;
                this.RedrawStructure();
            }
        }

        internal override string SmilesString
        {
            get
            {
                return null;
            }

            set
            {
                this.stringData = value;
                this.RedrawStructure();
            }
        }

        #endregion

        #region Methods

        internal override string GetClosestHydrogenDisplayMode(string hydrogenDisplayMode)
        {
            return StructureControl.GetClosestHydrogenDisplayModeValue(hydrogenDisplayMode).ToString();
        }

        internal override bool Init(Control parentControl)
        {
            if (this.javaHostInstance.Start())
            {
                if (parentControl != null)
                {
                    parentControl.Controls.Add(this);
                    this.Dock = DockStyle.Fill;
                }

                return true;
            }

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.javaHostInstance = null;
            }

            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.cachedImage == null)
            {
                this.cachedImage = this.Image;
            }

            if (this.cachedImage != null)
            {
                //Begin: clip image to fit the cliprectangle
                //Fix LD-111:Marvin has the same issue. 
                // We need to Clip  image to redraw on the invalid area when only part of draw region is invalid.
                // The original version will scale down the image to fit into the invalid area (smaller). 
                // It will cause structure duplicated.
                // our fix is to only draw the clip area of the image on the invalid area
                e.Graphics.DrawImage(this.cachedImage, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
                //End: clip image to fit the cliprectangle
            }
        }

        private static MarvinHydrogenDisplayMode GetClosestHydrogenDisplayModeValue(string hydrogenDisplayMode)
        {
            if (string.Equals(hydrogenDisplayMode, bool.TrueString, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(hydrogenDisplayMode, "On", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    hydrogenDisplayMode, 
                    MarvinHydrogenDisplayMode.All.ToString(),
                    StringComparison.OrdinalIgnoreCase))
            {
                return MarvinHydrogenDisplayMode.All;
            }

            foreach (MarvinHydrogenDisplayMode mode in Enum.GetValues(typeof(MarvinHydrogenDisplayMode)))
            {
                if (mode.ToString() == hydrogenDisplayMode)
                {
                    return mode;
                }
            }

            return MarvinHydrogenDisplayMode.Off;
        }

        private void StructureControl_Resize(object sender, EventArgs e)
        {
            this.RedrawStructure();
        }

        private void RedrawStructure()
        {
            this.cachedImage = null;
            this.Invalidate();
        }

        #endregion
    }
}
