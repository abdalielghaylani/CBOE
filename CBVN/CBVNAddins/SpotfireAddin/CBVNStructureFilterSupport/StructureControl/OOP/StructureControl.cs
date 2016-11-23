// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureControl.cs" company="PerkinElmer Inc.">
//   Copyright 2012 PerkinElmer Inc. 
// 100 CambridgePark Drive, Cambridge, MA 02140. 
// All rights reserved. 
// This software is the confidential and proprietary information 
// of PerkinElmer Inc. ("Confidential Information"). You shall not 
// disclose such Confidential Information and may not use it in any way, 
// absent an express written license agreement between you and PerkinElmer Inc. 
// that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CBVNStructureFilterSupport.OOP
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using log4net;
    using CBVNStructureFilterSupport.ChemDraw;
    using CBVNStructureFilterSupport.Framework;

    internal class StructureControl : StructureControlBase
    {
        #region Constants and Fields

        public const string StructureControlHostErrorPrefix = "StructureControlHostError ";

        public const string StructureControlHostOutputPrefix = "StructureControlHostOutput ";

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(StructureControl));

        private Image cachedImage;

        private Process oopProcess;

        #endregion

        #region Constructors and Destructors

        internal StructureControl(ControlType controlType, bool viewOnly)
            : base(controlType, viewOnly)
        {
            this.Resize += this.StructureControl_Resize;
        }

        #endregion

        #region Properties

        internal override string ChimeString
        {
            get
            {
                try
                {
                    string chimeString = this.CallOOPProcess("get_ChimeString");
                    return Encoding.UTF8.GetString(Convert.FromBase64String(chimeString));
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }

            set
            {
                string chimeString = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
                this.CallOOPProcess(string.Format(CultureInfo.CurrentCulture, "set_ChimeString {0}", chimeString));
                this.cachedImage = null;
            }
        }

        internal override string HydrogenDisplayMode
        {
            set
            {
                string displayModeString = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
                this.CallOOPProcess(
                    string.Format(CultureInfo.CurrentCulture, "set_HydrogenDisplayMode {0}", displayModeString));
            }
        }

        internal override string[] HydrogenDisplayModes
        {
            get
            {
                string displayModeValues = this.CallOOPProcess("get_HydrogenDisplayModeValues");
                string joinedDisplayModevalue = Encoding.UTF8.GetString(Convert.FromBase64String(displayModeValues));
                return joinedDisplayModevalue.Split(new[] { '\t' });
            }
        }

        internal override Image Image
        {
            get
            {
                string result = this.CallOOPProcess("get_Image");
                Image image = null;
                if (!string.IsNullOrEmpty(result))
                {
                    try
                    {
                        var bytes = Convert.FromBase64String(result);
                        if (bytes.Length > 0)
                        {
                            using (var stream = new MemoryStream(bytes))
                            {
                                image = new Bitmap(stream);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warn("Failed to get image.", e);
                    }
                }
                else
                {
                    Log.Debug("Received an empty string as the image.");
                }

                return image;
            }
        }

        internal bool IsInstalled
        {
            get
            {
                if (this.oopProcess == null)
                {
                    if (!this.LaunchProcess())
                    {
                        return false;
                    }
                }

                bool isInstalled = this.CallOOPProcess(string.Format(CultureInfo.CurrentCulture, "IsInstalled {0}", this.CtrlType)).
                    Equals(bool.TrueString);
                return isInstalled;
            }
        }

        internal override string MolFileString
        {
            get
            {
                try
                {
                    string result = this.CallOOPProcess("get_MolFileString");
                    return Encoding.UTF8.GetString(Convert.FromBase64String(result));
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }

            set
            {
                string molFileString = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
                this.CallOOPProcess(
                    string.Format(CultureInfo.CurrentCulture, "set_MolFileString {0}", molFileString));
                this.cachedImage = null;
            }
        }

        internal override string SmilesString
        {
            get
            {
                try
                {
                    string smilesString = this.CallOOPProcess("get_SmilesString");
                    return Encoding.UTF8.GetString(Convert.FromBase64String(smilesString));
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }

            set
            {
                string smilesString = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
                this.CallOOPProcess(string.Format(CultureInfo.CurrentCulture, "set_SmilesString {0}", smilesString));
                this.cachedImage = null;
            }
        }

        internal override byte[] CDXData
        {
            get
            {
                try
                {
                    if (CtrlType == ControlType.ChemDraw)
                    {
                        string data = this.CallOOPProcess("get_CDXData");
                        return Convert.FromBase64String(data);
                    }
                }
                catch (Exception)
                {
                }
                return base.CDXData;
            }
            set
            {
                try
                {
                    if (CtrlType == ControlType.ChemDraw)
                    {
                        string data = Convert.ToBase64String(value);
                        this.CallOOPProcess(string.Format(CultureInfo.CurrentCulture, "set_CDXData {0}", data));
                        this.cachedImage = null;
                        return;
                    }
                }
                catch (Exception)
                {
                }
                base.CDXData = value;
            }
        }

        /// <summary>
        /// Gets or Sets the render settings
        /// </summary>
        internal override RenderSettings RenderSettings
        {
            get
            {
                if (this.CtrlType == ControlType.ChemDraw)
                {
                    string result = this.CallOOPProcess("get_RenderSettings");
                    if (string.IsNullOrEmpty(result))
                    {
                        return ChemDrawRenderSettings.GetDefaultSettings();
                    }
                    return ChemDrawUtilities.Base64DeserializeChemDrawRenderSettings(result);
                }
                else
                {
                    return RenderSettings.GetDefaultSettings();
                }
            }

            set
            {
                if (this.CtrlType == ControlType.ChemDraw)
                {
                    string renderSettingsString = string.Empty;
                    if (value != null && (value is ChemDrawRenderSettings))
                    {
                        renderSettingsString = ChemDrawUtilities.Base64SerializeChemDrawRenderSettings(value as ChemDrawRenderSettings);
                    }
                    this.CallOOPProcess(string.Format(CultureInfo.CurrentCulture, "set_RenderSettings {0}", renderSettingsString));
                    this.cachedImage = null;
                }
            }
        }

        internal override string CDXFileString
        {
            get
            {
                try
                {
                    return Encoding.UTF8.GetString(Convert.FromBase64String(this.CallOOPProcess("get_CDXFileString")));
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }

            set
            {
                string cdxFileString = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
                this.CallOOPProcess(string.Format(CultureInfo.CurrentCulture, "set_CDXFileString {0}", cdxFileString));
                this.cachedImage = null;
            }
        }

        #endregion

        #region Methods

        internal override string GetClosestHydrogenDisplayMode(string hydrogenDisplayMode)
        {
            string displayModeString = Convert.ToBase64String(Encoding.UTF8.GetBytes(hydrogenDisplayMode));
            string result =
                this.CallOOPProcess(
                    string.Format(CultureInfo.CurrentCulture, "GetClosestHydrogenDisplayMode {0}", displayModeString));
            return Encoding.UTF8.GetString(Convert.FromBase64String(result));
        }

        internal override bool Init(Control parentControl)
        {
            if (this.oopProcess == null)
            {
                if (!this.LaunchProcess())
                {
                    return false;
                }
            }

            string initResult =
                this.CallOOPProcess(
                    string.Format(CultureInfo.CurrentCulture, "Init {0} {1}", this.CtrlType, this.ViewOnly));
            if (initResult.Equals(bool.TrueString))
            {
                if (parentControl != null)
                {
                    parentControl.Controls.Add(this);
                    this.Dock = DockStyle.Fill;
                }

                return true;
            }

            return false;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && this.oopProcess != null)
            {
                this.oopProcess.Dispose();
                this.oopProcess = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
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
                //Fix LD-111:duplicated chem structure in structure view. 
                // We need to Clip  image to redraw on the invalid area when only part of draw region is invalid.
                // The original version will scale down the image to fit into the invalid area (smaller). 
                // It will cause structure duplicated.
                // our fix is to only draw the clip area of the image on the invalid area.
                e.Graphics.DrawImage(this.cachedImage, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);                
                //End: clip image to fit the cliprectangle
            }
        }

        private string CallOOPProcess(string call)
        {
            if (this.oopProcess == null)
            {
                throw new Exception("Outer process not started");
            }

            if (this.oopProcess.HasExited)
            {
                throw new InvalidOperationException("Outer process has exited.");
            }

            this.oopProcess.StandardInput.WriteLine(call);

            string result = string.Empty;
            while (!result.StartsWith(StructureControlHostOutputPrefix, StringComparison.OrdinalIgnoreCase))
            {
                result = this.oopProcess.StandardOutput.ReadLine();
            }

            result = result.Remove(0, StructureControlHostOutputPrefix.Length);

            string error = string.Empty;
            while (!error.StartsWith(StructureControlHostErrorPrefix, StringComparison.OrdinalIgnoreCase))
            {
                error = this.oopProcess.StandardError.ReadLine();
            }

            error = error.Remove(0, StructureControlHostErrorPrefix.Length);

            if (!string.IsNullOrEmpty(error))
            {
                try
                {
                    error = Encoding.UTF8.GetString(Convert.FromBase64String(error));
                }
                catch (Exception e)
                {
                    Log.Warn("Failed to convert error string from base64.", e);
                }

                throw new Exception(error);
            }

            return result;
        }

        private bool LaunchProcess()
        {
            string structureControlHost = Path.Combine(GeneralClass.LDInstallationPath, "Spotfire.Dxp.LeadDiscovery.StructureControlHost.exe");
            ProcessStartInfo processStartInfo = new ProcessStartInfo(structureControlHost);
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;

            try
            {
                this.oopProcess = Process.Start(processStartInfo);
                return true;
            }
            catch (Exception e)
            {
                Log.Warn("Unable to start external process.", e);
            }

            return false;
        }

        private void StructureControl_Resize(object sender, EventArgs e)
        {
            Control control = (Control)sender;

            this.CallOOPProcess(
                string.Format(
                    CultureInfo.CurrentCulture, "set_Size {0} {1}", control.Width, control.Height));

            this.cachedImage = null;
            this.Invalidate();
        }

        /// <summary>
        /// Load render settings
        /// </summary>
        /// <param name="settingsFileName">the file full name</param>
        internal override void LoadRenderSettings(string settingsFileName)
        {
            string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(settingsFileName));
            this.CallOOPProcess(string.Format(CultureInfo.CurrentCulture, "LoadRenderSettings {0}", base64String));
            this.cachedImage = null;
        }

        internal override void ClearRenderSettings()
        {
            this.CallOOPProcess("ClearRenderSettings");
            this.cachedImage = null;
        }

        #endregion
    }
}
