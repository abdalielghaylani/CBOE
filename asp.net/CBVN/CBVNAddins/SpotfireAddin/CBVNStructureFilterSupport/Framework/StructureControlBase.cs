// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureControlBase.cs" company="PerkinElmer Inc.">
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

namespace CBVNStructureFilterSupport.Framework
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// This class servers as both an interface and factory for structure control implementations.
    /// When adding new structure control implementations, inherit this class and update the methods
    /// inside the "Implementation installation detection" and "Creation and instantiation" regions.
    /// </summary>
    public abstract partial class StructureControlBase : UserControl
    {
        #region Constants and Fields

        /// <summary>
        /// The type of control.
        /// </summary>
        private readonly ControlType controlType;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureControlBase"/> class.
        /// </summary>
        /// <param name="controlType">Type of the control.</param>
        /// <param name="viewOnly">If set to <c>true</c> viewing only (no editing).</param>
        protected StructureControlBase(ControlType controlType, bool viewOnly)
        {
            this.controlType = controlType;
            this.ViewOnly = viewOnly;

            this.InitializeComponent();
        }

        #endregion

        #region Events

        /// <summary>
        /// A slightly special construct required since one cannot fire the event of a base class.
        /// </summary>
        protected internal event EventHandler StructureChangedEvent;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the structure data as a chime string.
        /// </summary>
        /// <value>The chime string.</value>
        internal virtual string ChimeString
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        /// <summary>Gets or sets the FormulaName.
        /// </summary>
        /// <value>The FormulaName.</value>
        internal virtual string FormulaName
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the control type.
        /// </summary>
        /// <value>The control type.</value>
        internal ControlType CtrlType
        {
            get
            {
                return this.controlType;
            }
        }

        /// <summary>Sets the hydrogen display mode.
        /// </summary>
        /// <value>The hydrogen display mode.</value>
        internal virtual string HydrogenDisplayMode
        {
            set
            {
            }
        }

        /// <summary>Gets the available hydrogen display modes.
        /// </summary>
        /// <value>The hydrogen display modes.</value>
        internal virtual string[] HydrogenDisplayModes
        {
            get
            {
                return null;
            }
        }

        /// <summary>Gets the image.
        /// </summary>
        /// <value>The image.</value>
        internal virtual Image Image
        {
            get
            {
                return null;
            }
        }

        /// <summary>Gets or sets the mol file string.
        /// </summary>
        /// <value>The mol file string.</value>
        internal virtual string MolFileString
        {
            get
            {
                return null;
            }

            set
            {
            }
        }


        /// <summary>Gets or sets the CDX file string.
        /// </summary>
        /// <value>The mol file string.</value>
        internal virtual string CDXFileString
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        /// <summary>Gets or sets the smiles string.
        /// </summary>
        /// <value>The smiles string.</value>
        internal virtual string SmilesString
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        /// <summary>Gets or sets the CDXML string.
        /// </summary>
        /// <value>The smiles string.</value>
        internal virtual string CDXMLString
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        /// <summary>get set the CDXData
        /// </summary>
        /// <value>the CDXData</value>
        internal virtual byte[] CDXData
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        /// <summary>
        /// Set structure data
        /// </summary>
        /// <value>The structure data, should be MolString, SMILES, CDXML, Chime format</value>
        internal virtual object StructureData
        {
            set { }
        }

        /// <summary>
        /// Gets a value indicating whether the control is view only or also editing.
        /// </summary>
        /// <value>The value <c>true</c> if view only; otherwise, <c>false</c>.
        /// </value>
        internal bool ViewOnly { get; private set; }

        /// <summary>
        /// Gets or sets the RenderSettings.
        /// </summary> 
        /// <value>The RenderSettings.</value>          
        internal virtual RenderSettings RenderSettings
        {
            get
            {
                return RenderSettings.GetDefaultSettings();
            }

            set
            {
            }
        }

        #endregion

        #region Methods

        /// <summary>Calls the editor.
        /// </summary>
        internal virtual void CallEditor()
        {
        }

        /// <summary>Copies to clipboard.
        /// </summary>
        internal virtual void CopyToClipboard()
        {
            // TODO [fblom, 2011-11-04]: We need to copy different formats to the clipboard.
            // [fblom, 2011-11-04]: After looking at it I don't really think that it is necessary.
            // (At least not until we implement true support for structures...)
            // All the renderers that we have are able to supply a MolFileString, and
            // all the editors that I've tried (SymyxDraw, ChemDraw, Marvin) are able to handle
            // the CT Format that we put on the clipboard...
            CopyMolFileStringToClipboard(this.MolFileString);
        }
        /// <summary>Copies to clipboard.
        /// </summary>
        internal virtual void CopyToClipboard(string StructureString)
        {
        }

        /// <summary>Gets the closest hydrogen display mode.
        /// </summary>
        /// <param name="hydrogenDisplayMode">The hydrogen display mode.</param>
        /// <returns>The closest hydrogen display mode.</returns>
        internal virtual string GetClosestHydrogenDisplayMode(string hydrogenDisplayMode)
        {
            return null;
        }

        /// <summary>Initializes the control.
        /// </summary>
        /// <param name="parentControl">The parent control.</param>
        /// <returns><c>True</c> if the operation went well; otherwise <c>false</c>.</returns>
        internal abstract bool Init(Control parentControl);

        /// <summary>
        /// Fires the structure changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "FBM: CodeCleanup for legacy code.")]
        protected void FireStructureChangedEvent(object sender, EventArgs e)
        {
            if (this.StructureChangedEvent != null)
            {
                this.StructureChangedEvent(sender, e);
            }
        }

        /// <summary>
        /// Copies the mol file string to clipboard.
        /// </summary>
        /// <param name="molFileString">The mol file string.</param>
        private static void CopyMolFileStringToClipboard(string molFileString)
        {
            using (ClipboardHelper clipboardHelper = new ClipboardHelper())
            {
                clipboardHelper.Clear();
                clipboardHelper.AddChemData(ChemDataFormats.GetFormat(ChemDataFormats.MDLCT), molFileString);
            }
        }

        /// <summary>
        /// LoadRenderSettings based on the settings path
        /// </summary>
        /// <param name="settingsFileName">the file full path</param>
        internal virtual void LoadRenderSettings(string settingsFileName)
        {
        }

        /// <summary>
        /// Clear the render Settings
        /// </summary>
        internal virtual void ClearRenderSettings()
        {
        }

        #endregion
    }
}
