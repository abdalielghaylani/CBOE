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

namespace CBVNStructureFilterSupport.MDLDraw
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;

    using log4net;

    using MDL.Draw.Renderer;
    using MDL.Draw.Renderer.Preferences;
    using MDL.Draw.Renditor;
    using MDL.Draw.StructureConversion;
    using CBVNStructureFilterSupport.Framework;


    internal class StructureControl : StructureControlBase
    {
        #region Constants and Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(StructureControl));

        private static bool? isInstalled;

        private bool modifyingStructure;

        private Renderer renderer;

        private Renditor renditor;

        #endregion

        #region Constructors and Destructors

        internal StructureControl(bool viewOnly)
            : base(ControlType.MdlDraw, viewOnly)
        {
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
                        Assembly editorAssembly = typeof(Renditor).Assembly;
                        string fullName = editorAssembly.FullName;

                        // Try to load a known dependent assembly that is 32-bit only and not one of the assemblies
                        // we put in the GAC for build purposes only
                        string textServicesFullName = fullName.Replace("MDL.Draw.Editor", "MDL.Draw.TextServicesWrapper");

                        Assembly otherAssembly = Assembly.Load(textServicesFullName);
                        isInstalled = otherAssembly != null;
                    }
                    catch (FileNotFoundException e)
                    {
                        // Intentionally ignored as part of the installation detection
                        Log.Debug("Failed to load assembly.", e);
                    }
                    catch (FileLoadException e)
                    {
                        // Intentionally ignored as part of the installation detection
                        Log.Debug("Failed to load assembly.", e);
                    }
                    catch (BadImageFormatException e)
                    {
                        // Intentionally ignored as part of the installation detection
                        Log.Debug("Failed to load assembly.", e);
                    }
                }

                if (isInstalled == null)
                {
                    isInstalled = false;
                }

                return isInstalled.Value;
            }
        }

        internal override string ChimeString
        {
            get
            {
                return this.renderer.ChimeString;
            }

            set
            {
                this.modifyingStructure = true;
                if (IsValidChimeString(value))
                {
                    this.renderer.ChimeString = value;
                }
                else
                {
                    this.renderer.MolfileString = string.Empty;
                }

                this.modifyingStructure = false;
            }
        }

        internal override string HydrogenDisplayMode
        {
            set
            {
                if (this.renderer != null)
                {
                    this.renderer.Preferences.HydrogenDisplayMode = GetClosestHydrogenDisplayModeValue(value);
                }
            }
        }

        internal override string[] HydrogenDisplayModes
        {
            get
            {
                return Enum.GetNames(typeof(HydrogenDisplayMode));
            }
        }

        internal override Image Image
        {
            get
            {
                return this.renderer.Image;
            }
        }

        internal override string MolFileString
        {
            get
            {
                return this.renderer.MolfileString;
            }

            set
            {
                this.modifyingStructure = true;
                if (IsValidMolFileString(value))
                {
                    this.renderer.MolfileString = value;
                }
                else
                {
                    this.renderer.MolfileString = string.Empty;
                }

                this.modifyingStructure = false;
            }
        }

        internal override string SmilesString
        {
            get
            {
                return this.renderer.SmilesString;
            }

            set
            {
                this.modifyingStructure = true;
                if (IsValidSmilesString(value))
                {
                    this.renderer.SmilesString = value;
                }
                else
                {
                    this.renderer.MolfileString = string.Empty;
                }

                this.modifyingStructure = false;
            }
        }

        #endregion

        #region Methods

        internal override void CallEditor()
        {
            if (this.renditor != null)
            {
                this.renditor.FireEditor();
            }
        }

        internal override string GetClosestHydrogenDisplayMode(string hydrogenDisplayMode)
        {
            return GetClosestHydrogenDisplayModeValue(hydrogenDisplayMode).ToString();
        }

        internal override bool Init(Control parentControl)
        {
            if (parentControl == null)
            {
                parentControl = this;
            }

            if (this.ViewOnly)
            {
                this.renderer = new Renderer();
            }
            else
            {
                this.renditor = new Renditor();
                this.renditor.StructureChanged += this.Renditor_StructureChanged;

                this.renderer = this.renditor;
            }

            parentControl.Controls.Add(this.renderer);
            this.renderer.Dock = DockStyle.Fill;

            this.renderer.Preferences.StructureScalingMode = StructureScalingMode.ScaleToFitBox;

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.renderer != null)
                {
                    this.renderer.Dispose();
                    this.renderer = null;
                }

                if (this.renditor != null)
                {
                    // The renditor object is the same as the renderer object!
                    this.renditor = null;
                }
            }

            base.Dispose(disposing);
        }

        private static HydrogenDisplayMode GetClosestHydrogenDisplayModeValue(string hydrogenDisplayMode)
        {
            if (string.Equals(hydrogenDisplayMode, bool.TrueString, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(hydrogenDisplayMode, "On", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    hydrogenDisplayMode,
                    MDL.Draw.Renderer.Preferences.HydrogenDisplayMode.All.ToString(),
                    StringComparison.OrdinalIgnoreCase))
            {
                return MDL.Draw.Renderer.Preferences.HydrogenDisplayMode.All;
            }

            foreach (
                HydrogenDisplayMode mode in Enum.GetValues(typeof(HydrogenDisplayMode)))
            {
                if (mode.ToString() == hydrogenDisplayMode)
                {
                    return mode;
                }
            }

            return MDL.Draw.Renderer.Preferences.HydrogenDisplayMode.Off;
        }

        private static bool IsValidChimeString(string chimeString)
        {
            try
            {
                if (!string.IsNullOrEmpty(StructureConverter.ChimeStringToSmilesString(chimeString)))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.Warn("Unable to determine validity of chime string.", e);
            }

            return false;
        }

        private static bool IsValidMolFileString(string molFileString)
        {
            try
            {
                StructureConverter structureConverter = new StructureConverter();
                structureConverter.MolfileString = molFileString;
                return true;
            }
            catch (Exception e)
            {
                Log.Warn("Unable to determine validity of molfile string.", e);
            }

            return false;
        }

        private static bool IsValidSmilesString(string smilesString)
        {
            try
            {
                StructureConverter structureConverter = new StructureConverter();
                structureConverter.Smiles = smilesString;
                return true;
            }
            catch (Exception e)
            {
                Log.Warn("Unable to determine validity of smiles string.", e);
            }

            return false;
        }

        private void Renditor_StructureChanged(object sender, EventArgs e)
        {
            if (!this.modifyingStructure)
            {
                this.FireStructureChangedEvent(sender, e);
            }
        }

        #endregion
    }
}
