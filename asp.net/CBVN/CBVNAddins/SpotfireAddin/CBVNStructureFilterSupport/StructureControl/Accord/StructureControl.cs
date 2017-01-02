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

namespace CBVNStructureFilterSupport.Accord
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using log4net;
    using CBVNStructureFilterSupport.Framework;
    using CBVNStructureFilterSupport.Accord;

    internal partial class StructureControl : StructureControlBase
    {
        #region Constants and Fields

        private static readonly ILog Log = LogManager.GetLogger(typeof(StructureControl));

        private static bool? isInstalled;

        private AxHost accord;

        private AxChemistry accordControl;

        #endregion

        #region Constructors and Destructors

        internal StructureControl(bool viewOnly)
            : base(ControlType.Accord, viewOnly)
        {
            this.InitializeComponent();
        }

        #endregion

        #region Properties

        internal static bool IsInstalled
        {
            get
            {
                if (isInstalled == null)
                {
                    try
                    {
                        using (AxChemistry control = new AxChemistry())
                        {
                            control.CreateControl();
                            if (control.Created)
                            {
                                isInstalled = true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // Intentionally handled and ignored as part of the installation detection
                        // [dvulcan, 2012-03-12]: Only log to debug for support issues and avoid logging 
                        // this every time on systems where this component is not installed 
                        Log.Debug("Failed to create control.", e);
                    }
                }

                if (isInstalled == null)
                {
                    isInstalled = false;
                }

                return isInstalled.Value;
            }
        }

        internal override string HydrogenDisplayMode
        {
            set
            {
                if (this.accordControl != null)
                {
                    this.accordControl.ShowHydrogens = GetClosestHydrogenDisplayModeValue(value);
                }
            }
        }

        internal override string[] HydrogenDisplayModes
        {
            get
            {
                return new[] { bool.TrueString, bool.FalseString };
            }
        }

        internal override Image Image
        {
            get
            {
                try
                {
                    Image image = this.accordControl.Picture;

                    if (image.Size != this.accordControl.Size)
                    {
                        return new Bitmap(image, this.accordControl.Size);
                    }

                    return image;
                }
                catch (Exception e)
                {
                    Log.Error("Failed to get image.", e);
                }

                return null;
            }
        }

        internal override string MolFileString
        {
            get
            {
                this.accordControl.DataFormat = "MDL Molfile";
                return this.accordControl.DataText;
            }

            set
            {
                this.accordControl.DataText = value;
            }
        }

        internal override string SmilesString
        {
            get
            {
                this.accordControl.DataFormat = "SMILES";
                return this.accordControl.DataText;
            }

            set
            {
                this.accordControl.DataText = value;
            }
        }

        #endregion

        #region Methods

        internal override string GetClosestHydrogenDisplayMode(string hydrogenDisplayMode)
        {
            return GetClosestHydrogenDisplayModeValue(hydrogenDisplayMode).ToString();
        }

        internal override bool Init(Control parentControl)
        {
            this.accordControl = new AxChemistry();

            // Let us own the accordControl.
            this.accordControl.BeginInit();
            this.Controls.Add(this.accordControl);
            this.accordControl.EndInit();

            // Then if we have a parent - add us to the parent's controls.
            if (parentControl != null)
            {
                parentControl.Controls.Add(this);
            }

            this.Dock = DockStyle.Fill;

            this.accordControl.ReadOnly = this.ViewOnly;
            this.accord = this.accordControl;
            this.accord.Dock = DockStyle.Fill;

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.accordControl != null)
                {
                    this.accordControl.ContainingControl = null;
                }

                if (this.components != null)
                {
                    this.components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private static bool GetClosestHydrogenDisplayModeValue(string hydrogenDisplayMode)
        {
            if (string.Equals(hydrogenDisplayMode, bool.TrueString, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(hydrogenDisplayMode, "On", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(hydrogenDisplayMode, "All", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
