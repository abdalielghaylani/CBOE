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

namespace CBVNStructureFilterSupport.ChemDraw
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Forms;
    using AxChemDrawControlConst11;
    using log4net;
    using Graphics = System.Drawing.Graphics;
    using System.Reflection;
    using ChemDrawControlConst11;
    using System.Text;
    using CBVNStructureFilterSupport.Framework;    
    /// <summary>
    /// The Structure control implementation for ChemDraw
    /// </summary>
    internal class StructureControl : StructureControlBase
    {
        #region Constants and Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(StructureControl));

        /// <summary>
        /// The AxHost-wrapping control instance
        /// </summary>
        private AxChemDrawCtl chemDraw;

        /// <summary>
        /// The native control instance
        /// </summary>
        private AxChemDrawCtl chemDrawControl;

        #endregion

        #region Constructors and Destructors

        internal StructureControl(bool viewOnly)
            : base(ControlType.ChemDraw, viewOnly)
        {
        }

        #endregion

        #region Properties

        // Specific check for ChemDraw ActiveX Enterprise Const
        // If not present it will get installed in ChemistryAddin
        internal static bool IsInstalled
        {
            get
            {
                Guid result;
                return ChemDrawUtilities.TryGetClassIdFromProgId("ChemDrawControlConst11.ChemDrawCtl", out result);
            }
        }

        internal override byte[] CDXData
        {
            get
            {
                try
                {
                    object obj = this.GetControlData("chemical/x-cdx");
                    if (obj != null)
                    {
                        return obj as byte[];
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to get CDX data.", e);
                }

                return null;
            }

            set
            {
                try
                {
                    if (value == null)
                    {
                        this.Clear();
                    }
                    else
                    {
                        this.SetControlData("chemical/x-cdx", value);
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to set CDX data.", e);
                }
            }
        }

        internal byte[] EMFData
        {
            get
            {
                try
                {
                    object obj = this.GetControlData(Identifiers.EMFContentType);
                    if (obj != null)
                    {
                        return obj as byte[];
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to get EMF data.", e);
                }

                return null;
            }

            set
            {
                try
                {
                    if (value == null)
                    {
                        this.Clear();
                    }
                    else
                    {
                        this.SetControlData(Identifiers.EMFContentType, value);
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to set EMF data.", e);
                }
            }
        }
        internal byte[] SKCData
        {
            get
            {
                try
                {
                    object obj = this.GetControlData(Identifiers.SKCContentType);
                    if (obj != null)
                    {
                        return obj as byte[];
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to get SKC data.", e);
                }

                return null;
            }

            set
            {
                try
                {
                    if (value == null)
                    {
                        this.Clear();
                    }
                    else
                    {
                        this.SetControlData(Identifiers.SKCContentType, value);
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to set SKC data.", e);
                }
            }
        }

        internal override string HydrogenDisplayMode
        {
            set
            {
                if (this.chemDrawControl != null)
                {
                    bool hideHydrogens = !StructureControl.GetClosestHydrogenDisplayModeValue(value);

                    // Some versions of CDAX throw a casting exception on this call so we don't honor
                    //  HydrogenDisplay mode if that happens.
                    try
                    {
                        Settings settings = this.chemDrawControl.Objects.Settings;
                        if (ChemDrawUtilities.GetDllVersion() > 12)
                        {
                            settings = this.chemDrawControl.Settings;
                        }
                        settings.HideImplicitHydrogens = hideHydrogens;
                    }
                    catch (Exception e)
                    {
                        Log.Warn("Failed to set HydrogenDisplayMode.", e);
                    }
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
                    // When retrieving control data as "wmf" or "bmp" it can basically be of any size, so we need to do some scaling
                    // and that is best done with a metafile, so that's why we get the wmf rather than the bmp
                    byte[] data = this.GetControlData("emf") as byte[];
                    if (data != null && data.Length > 0)
                    {
                        Image metafileImage = Image.FromStream(new MemoryStream(data));

                        int maxMetafileDimension = Math.Max(metafileImage.Width, metafileImage.Height);
                        int minFitDimension = Math.Min(this.Width, this.Height);
                        if (maxMetafileDimension != 0)
                        {
                            float scale = minFitDimension > maxMetafileDimension ? 1f : (float)minFitDimension / maxMetafileDimension;
                            Size scaledSize = new Size(
                                (int)((metafileImage.Width * scale) + 0.5), (int)((metafileImage.Height * scale) + 0.5));

                            Bitmap scaledCanvas = new Bitmap(this.Width, this.Height);
                            using (Graphics scaledCanvasGraphics = Graphics.FromImage(scaledCanvas))
                            {
                                // Workarround for structures not rendering in the web player tool tips
                                // ChemDraw is returning transparent emf image which causes
                                // black bonds to not be visible in black background tool tips
                                // Force white background while creating the scaled bitmap
                                scaledCanvasGraphics.Clear(Color.White);
                                scaledCanvasGraphics.DrawImage(
                                    metafileImage,
                                    (this.Width - scaledSize.Width) / 2,
                                    (this.Height - scaledSize.Height) / 2,
                                    scaledSize.Width,
                                    scaledSize.Height);
                            }

                            return scaledCanvas;
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to get image.", e);
                }

                return null;
            }
        }

        internal override string MolFileString
        {
            get
            {
                try
                {
                    object obj = this.GetControlData("mol");
                    if (obj != null)
                    {
                        return obj.ToString();
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to get Molfile string.", e);
                }

                return null;
            }

            set
            {
                try
                {
                    if (value == null)
                    {
                        this.Clear();
                    }
                    else
                    {
                        this.SetControlData("mol", value);
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to set Molfile string.", e);
                }
            }
        }

        internal override string CDXFileString
        {
            get
            {
                try
                {
                    //use const string to avoid typo
                    object obj = this.GetControlData(Identifiers.CDXContentType);
                    if (obj != null && obj is byte[])
                    {
                        return Convert.ToBase64String((byte[])obj);
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to get CDX string.", e);
                }

                return null;
            }

            set
            {
                try
                {
                    if (value == null)
                    {
                        this.Clear();
                    }
                    else
                    {
                        //use const string to avoid typo
                        if (value.TrimStart().StartsWith(@"<?xml"))
                        {
                            this.SetControlData(Identifiers.CDXMLContentType, Encoding.ASCII.GetString(Encoding.UTF8.GetBytes(value)));
                        }
                        else if (value.TrimStart().StartsWith(@"VmpDR"))
                        {
                            this.SetControlData(Identifiers.CDXContentType, Convert.FromBase64String(value));
                        }
                        else // LD-950 to render mol/similes data when content-type is CDX
                        {
                            this.SetControlData("mol", value);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to set CDX string.", e);
                }
            }
        }

        internal override string SmilesString
        {
            get
            {
                try
                {
                    //use const string to avoid typo
                    object obj = this.GetControlData(Identifiers.XSmilesContentType);
                    if (obj != null)
                    {
                        return obj.ToString();
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to get Smiles string.", e);
                }

                return null;
            }

            set
            {
                try
                {
                    if (value == null)
                    {
                        this.Clear();
                    }
                    else
                    {
                        //use const string to avoid typo
                        this.SetControlData(Identifiers.XSmilesContentType, value);
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to set Smiles string.", e);
                }
            }
        }

        internal override string CDXMLString
        {
            get
            {
                try
                {
                    object obj = this.GetControlData(Identifiers.CDXMLContentType);
                    if (obj != null)
                    {
                        return obj.ToString();
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to get CDXML string.", e);
                }

                return null;
            }

            set
            {
                try
                {
                    if (value == null)
                    {
                        this.Clear();
                    }
                    else
                    {
                        this.SetControlData(Identifiers.CDXMLContentType, Encoding.ASCII.GetString(Encoding.UTF8.GetBytes(value)));
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to set CDXML string.", e);
                }
            }
        }

        internal override string FormulaName
        {
            get
            {
                try
                {
                    object obj = this.chemDrawControl.Objects.Formula;
                    if (obj != null)
                    {
                        return obj.ToString();
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to get Formula name.", e);
                }

                return null;
            }
        }

        internal override object StructureData
        {
            set
            {
                try
                {
                    if (value == null)
                    {
                        this.Clear();
                    }
                    else
                    {
                        if (value is string && ((string)value).TrimStart().StartsWith(@"<?xml"))
                        {
                            this.SetControlData(Identifiers.CDXMLContentType, Encoding.ASCII.GetString(Encoding.UTF8.GetBytes((string)value)));
                        }
                        else
                        {
                            // set structure data by MolString format
                            // ChemDraw control can identify it always if we transfer any CDXML, SMILES, Chime or MolString data with mol type
                            this.SetControlData("mol", value);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to set structure data.", e);
                }
            }
        }

        /// <summary>
        /// Gets or Sets the render settings
        /// </summary>
        internal override RenderSettings RenderSettings
        {
            get
            {
                return this.GetRenderSettings();
            }
            set
            {
                SetRenderSettings(value);
            }

        }

        #endregion

        #region Methods

        /// <summary>Copies to clipboard.
        /// </summary>
        internal override void CopyToClipboard()
        {
        }
        /// <summary>Copies to clipboard.
        /// </summary>
        internal override void CopyToClipboard(string StructureString)
        {
            SetControlData(Identifiers.CDXContentType, Convert.FromBase64String(StructureString));

            Metafile metafile = new Metafile(new MemoryStream(this.EMFData));
            using (ClipboardHelper clipboard = new ClipboardHelper())
            {
                clipboard.Clear();
                try
                {
                    //CDX 
                    clipboard.AddChemData(DataFormats.GetFormat(ChemDataFormats.CDX), this.CDXData);
                    //Molfile 
                    clipboard.AddChemData(DataFormats.GetFormat(ChemDataFormats.MDLCT), this.MolFileString);
                    //Smiles 
                    clipboard.AddChemData(DataFormats.GetFormat(ChemDataFormats.Smiles), this.SmilesString);
                    //Enhanced Metafile 
                    clipboard.AddChemData(DataFormats.GetFormat(ChemDataFormats.EnhancedMetafile), this.EMFData);
                    //SKC 
                    clipboard.AddChemData(DataFormats.GetFormat(ChemDataFormats.MDLSK), this.SKCData);
                }
                catch (ArgumentNullException e)
                {
                    //Unsupported Data Type, so LD has to log this warning
                    //This could happen whey copying Chim Data Type
                    //In original implementation, LD won't copy chime data into clipboard.
                    //Here we just keep the same behavior.
                    Log.Warn("Failed to add data to clipboard", e);
                }
            }
        }
        internal override void CallEditor()
        {
            using (StructureEditor structureEditor = new StructureEditor())
            {
                structureEditor.StructureControl.CDXData = this.CDXData;

                if (structureEditor.ShowDialog(this) == DialogResult.OK)
                {
                    this.CDXData = structureEditor.ResultCDXData;
                    this.FireStructureChangedEvent(this, null);
                }
            }
        }

        internal override string GetClosestHydrogenDisplayMode(string hydrogenDisplayMode)
        {
            return StructureControl.GetClosestHydrogenDisplayModeValue(hydrogenDisplayMode).ToString();
        }

        internal override bool Init(Control parentControl)
        {

            this.chemDraw = new AxChemDrawCtl();
            this.chemDraw.CreateControl();
            this.chemDrawControl = this.chemDraw;
            if (this.chemDraw != null && this.chemDrawControl != null)
            {
                // Let us own the chemDraw control.
                this.chemDraw.BeginInit();
                this.Controls.Add(this.chemDraw);
                this.chemDraw.EndInit();

                // Then if we have a parent - add us to the parent's controls.
                if (parentControl != null)
                {
                    parentControl.Controls.Add(this);
                }

                this.Dock = DockStyle.Fill;
                this.chemDraw.Dock = DockStyle.Fill;

                this.chemDrawControl.ViewOnly = this.ViewOnly;
                this.chemDrawControl.EnlargeToFit = true;

                return true;
            }

            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.chemDraw != null)
                {
                    this.chemDraw.ContainingControl = null;
                    this.chemDraw.Dispose();

                    this.chemDrawControl = null;
                    this.chemDraw = null;
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

        private void Clear()
        {
            if (this.chemDrawControl != null)
            {
                this.chemDrawControl.Objects.Clear();
            }
        }

        /// <summary>
        /// Bottleneck for get_Data
        /// </summary>
        /// <param name="dataType">mol, wmf etc</param>
        /// <returns></returns>
        private object GetControlData(string dataType)
        {
            if (this.chemDrawControl != null)
            {
                return this.chemDrawControl.get_Data(dataType);
            }

            return null;
        }

        private void SetControlData(string dataType, object value)
        {
            if (this.chemDrawControl != null)
            {
                this.chemDrawControl.set_Data(dataType, value);
            }
        }

        /// <summary>
        /// Get the render settings
        /// </summary>
        /// <returns>the ChemDrawRenderSettings</returns>
        private RenderSettings GetRenderSettings()
        {
            ChemDrawRenderSettings chemDrawSettings = ChemDrawRenderSettings.GetDefaultSettings();
            try
            {
                if (this.chemDrawControl != null)
                {
                    PropertyInfo[] properties = typeof(ChemDrawRenderSettings).GetProperties();
                    Settings settings = this.chemDrawControl.Objects.Settings;
                    foreach (PropertyInfo property in properties)
                    {
                        object value = settings.GetType().GetProperty(property.Name).GetValue(settings, null);
                        property.SetValue(chemDrawSettings, value, null);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warn("Failed to get RenderSettings.", e);
            }
            return chemDrawSettings;
        }

        /// <summary>
        /// Set the render settings
        /// </summary>
        /// <param name="renderSettings">the ChemDrawRenderSettings</param>
        private void SetRenderSettings(RenderSettings renderSettings)
        {
            try
            {
                ChemDrawRenderSettings chemDrawRenderSettings;
                if (renderSettings == null || !(renderSettings is ChemDrawRenderSettings))
                {
                    chemDrawRenderSettings = ChemDrawRenderSettings.GetDefaultSettings();
                }
                else
                {
                    chemDrawRenderSettings = renderSettings as ChemDrawRenderSettings;
                }

                if (this.chemDrawControl != null)
                {
                    PropertyInfo[] properties = typeof(ChemDrawRenderSettings).GetProperties();
                    Settings settings = this.chemDrawControl.Objects.Settings;
                    if (ChemDrawUtilities.GetDllVersion() > 12)
                    {
                        settings = this.chemDrawControl.Settings;
                    }
                    foreach (PropertyInfo property in properties)
                    {
                        object value = property.GetValue(chemDrawRenderSettings, null);
                        settings.GetType().GetProperty(property.Name).SetValue(settings, value, null);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warn("Failed to set RenderSettings.", e);
            }
        }

        /// <summary>
        /// Load the render settings
        /// </summary>
        /// <param name="settingsFileName">the file full name</param>
        internal override void LoadRenderSettings(string settingsFileName)
        {
            if (this.chemDrawControl != null)
            {
                try
                {
                    System.IO.FileStream fs = System.IO.File.OpenRead(settingsFileName);
                    int length = (int)fs.Length;
                    Byte[] buffer = new Byte[length];
                    fs.Read(buffer, 0, length);
                    fs.Close();

                    this.chemDrawControl.Objects.set_Data("cdx", Type.Missing, Type.Missing, Type.Missing, buffer);
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to load cds file", e);
                }
            }
        }

        /// <summary>
        /// Clear the render settings
        /// </summary>
        internal override void ClearRenderSettings()
        {
            if (this.chemDrawControl != null)
            {
                this.chemDrawControl.Objects.Clear();
            }
        }

        #endregion
    }
}
