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

namespace CBVNStructureFilterSupport.ChemIQ
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using Interop.ChemIQ;
    using log4net;
    using CBVNStructureFilterSupport.Framework;

    internal partial class StructureControl : StructureControlBase
    {
        #region Constants and Fields

        /// <summary>
        /// The structure control logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(StructureControl));

        /// <summary>The different hydrogen display modes available for the control.
        /// </summary>
        private static readonly Dictionary<string, ictHydrogenDisplayTypes> HydrogenDisplayNameToTypeMap =
            new Dictionary<string, ictHydrogenDisplayTypes>();

        private static readonly Dictionary<ictHydrogenDisplayTypes, string> HydrogenTypeToDisplayNameMap =
            new Dictionary<ictHydrogenDisplayTypes, string>();

        /// <summary>A flag to indicate if the control is installed or not.
        /// </summary>
        private static bool? isInstalled;

        /// <summary>The actual control.
        /// </summary>
        private AxChemDisplay chemDisplay;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="StructureControl"/> class. 
        /// </summary>
        static StructureControl()
        {
            AddValidHydrogenDisplayType("None", ictHydrogenDisplayTypes.ictHydrogenDisplayNone);
            AddValidHydrogenDisplayType("Hetero", ictHydrogenDisplayTypes.ictHydrogenDisplayHetero);
            AddValidHydrogenDisplayType("Terminal", ictHydrogenDisplayTypes.ictHydrogenDisplayTerminal);
            AddValidHydrogenDisplayType(
                "Hetero and terminal", ictHydrogenDisplayTypes.ictHydrogenDisplayHeteroAndTerminal);
            AddValidHydrogenDisplayType("All", ictHydrogenDisplayTypes.ictHydrogenDisplayAll);
        }

        /// <summary>Initializes a new instance of the <see cref="StructureControl"/> class.
        /// </summary>
        public StructureControl()
            : this(true)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="StructureControl"/> class.
        /// </summary>
        /// <param name="viewOnly">if set to <c>true</c> the control will enter view only mode.</param>
        public StructureControl(bool viewOnly)
            : base(ControlType.ChemIQ, viewOnly)
        {
            this.InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>Gets a value indicating whether this instance is installed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is installed; otherwise, <c>false</c>.
        /// </value>
        internal static bool IsInstalled
        {
            get
            {
                if (isInstalled == null)
                {
                    try
                    {
                        using (AxChemDisplay control = new AxChemDisplay())
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
                        // Intentionally ignored as part of the installation detection
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

        /// <summary>Sets the hydrogen display mode.
        /// </summary>
        /// <value>The hydrogen display mode.</value>
        internal override string HydrogenDisplayMode
        {
            set
            {
                if (this.chemDisplay != null)
                {
                    this.chemDisplay.DisplayImplicitHydrogens = GetClosestHydrogenDisplayModeValue(value);
                }
                else
                {
                    base.HydrogenDisplayMode = value;
                }
            }
        }

        /// <summary>Gets the available hydrogen display modes.
        /// </summary>
        /// <value>The hydrogen display modes.</value>
        internal override string[] HydrogenDisplayModes
        {
            get
            {
                Dictionary<string, ictHydrogenDisplayTypes>.KeyCollection keys = HydrogenDisplayNameToTypeMap.Keys;
                string[] modes = new string[keys.Count];

                int index = 0;
                foreach (string mode in keys)
                {
                    modes[index++] = mode;
                }

                return modes;
            }
        }

        /// <summary>Gets the image.
        /// </summary>
        /// <value>The image.</value>
        internal override Image Image
        {
            get
            {
                Image image = this.chemDisplay.Picture;
                return image;
            }
        }

        /// <summary>Gets or sets the mol file string.
        /// </summary>
        /// <value>The mol file string.</value>
        internal override string MolFileString
        {
            get
            {
                string structureString =
                    this.chemDisplay.GetStructureString(ictStructureStringTypes.ictStructureStringTypeMDLCTFile);
                return structureString;
            }

            set
            {
                this.chemDisplay.SetStructureString(value);
            }
        }

        /// <summary>Gets or sets the smiles string.
        /// </summary>
        /// <value>The smiles string.</value>
        internal override string SmilesString
        {
            get
            {
                string structureString =
                    this.chemDisplay.GetStructureString(ictStructureStringTypes.ictStructureStringTypeSMILES);
                return structureString;
            }

            set
            {
                this.chemDisplay.SetStructureString(value);
            }
        }

        #endregion

        #region Methods

        /// <summary>Calls the editor.
        /// </summary>
        internal override void CallEditor()
        {
            if (this.chemDisplay != null)
            {
                this.chemDisplay.Edit();
            }
            else
            {
                base.CallEditor();
            }
        }

        /// <summary>Gets the closest hydrogen display mode.
        /// </summary>
        /// <param name="hydrogenDisplayMode">The hydrogen display mode.</param>
        /// <returns></returns>
        internal override string GetClosestHydrogenDisplayMode(string hydrogenDisplayMode)
        {
            // Get the human readable string matching the given string.
            ictHydrogenDisplayTypes displayMode = GetClosestHydrogenDisplayModeValue(hydrogenDisplayMode);
            return HydrogenTypeToDisplayNameMap[displayMode];
        }

        /// <summary>Initializes the control.
        /// </summary>
        /// <param name="parentControl">The parent control.</param>
        /// <returns>
        /// <c>True</c> if the operation went well; otherwise <c>false</c>
        /// </returns>
        internal override bool Init(Control parentControl)
        {
            this.chemDisplay = new AxChemDisplay();

            // Let us own the chemdisplay control.
            this.chemDisplay.BeginInit();
            this.Controls.Add(this.chemDisplay);
            this.chemDisplay.EndInit();

            // Then if we have a parent - add us to the parent's controls.
            if (parentControl != null)
            {
                parentControl.Controls.Add(this);
            }

            this.chemDisplay.AllowFileDrops = false;
            this.chemDisplay.Locked = this.ViewOnly;
            this.chemDisplay.AllowMenu = !this.ViewOnly;
            this.chemDisplay.ScaleToFit = true;
            
            this.chemDisplay.Change += this.ChemDisplay_Change;

            // [fblom, 2009-11-11]: When retreiveing the image of the structure the size needs to be correct.
            // When we use DockStyle.Fill on the component, the resulting image was 1 pixel too large in both directions.
            // We use anchoring to accomplish an image that is of the correct size instead. Since the difference was only
            // 1 pixel there should be no noticable difference to the user. The other way to do this would be to 
            // change the size of the control when retreiving the image, and then restoring it afterwards
            // which could lead to flickering.
            this.Dock = DockStyle.Fill;

            this.chemDisplay.Left = 0;
            this.chemDisplay.Top = 0;
            this.chemDisplay.Width = this.Size.Width - 1;
            this.chemDisplay.Height = this.Size.Height - 1;
            this.chemDisplay.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            return true;
        }

        /// <summary>Disposes the control.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.chemDisplay != null)
                {
                    this.chemDisplay.ContainingControl = null;
                }
            }

            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }

        private static void AddValidHydrogenDisplayType(string displayName, ictHydrogenDisplayTypes type)
        {
            HydrogenDisplayNameToTypeMap.Add(displayName, type);
            HydrogenTypeToDisplayNameMap.Add(type, displayName);
        }

        /// <summary>Gets the closest hydrogen display mode value.
        /// </summary>
        /// <param name="hydrogenDisplayMode">The hydrogen display mode.</param>
        /// <returns>The closest hydrogen display mode.</returns>
        private static ictHydrogenDisplayTypes GetClosestHydrogenDisplayModeValue(string hydrogenDisplayMode)
        {
            if (HydrogenDisplayNameToTypeMap.ContainsKey(hydrogenDisplayMode))
            {
                return HydrogenDisplayNameToTypeMap[hydrogenDisplayMode];
            }
            else if (string.Equals(hydrogenDisplayMode, bool.TrueString, StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(hydrogenDisplayMode, "On", StringComparison.OrdinalIgnoreCase))
            {
                return ictHydrogenDisplayTypes.ictHydrogenDisplayAll;
            }
            else
            {
                return ictHydrogenDisplayTypes.ictHydrogenDisplayNone;
            }
        }

        /// <summary>Handles the Change event of the chemDisplay control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ChemDisplay_Change(object sender, EventArgs e)
        {
            this.FireStructureChangedEvent(sender, e);
        }

        #endregion
    }
}
