// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureColumnRendererSettings.cs" company="PerkinElmer Inc.">
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

namespace CBVNStructureFilter
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;

    using Spotfire.Dxp.Framework.ApplicationModel;    
    using System.IO;    
    using Microsoft.Win32;
    using CBVNStructureFilter.Properties;
    using CBVNStructureFilterSupport.ChemDraw;
    using CBVNStructureFilterSupport.Framework;
    using CBVNStructureFilterSupport;

    internal partial class StructureColumnRendererSettings : Form
    {
        #region Constants and Fields

        private readonly StructureColumnRendererModel model;

        private readonly ControlType rendererType;

        private string showHydrogens;

        private StructureStringType stringType;

        /// <summary>
        /// The control which is used to load template and get settings
        /// </summary>
        private StructureControlBase structureControl;

        /// <summary>
        /// Store the renderSettings
        /// </summary>
        private RenderSettings renderSettings;

        /// <summary>
        /// Store the name of the style file
        /// </summary>
        private string styleFileName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureColumnRendererSettings"/> class.
        /// </summary>
        public StructureColumnRendererSettings()
        {
            this.InitializeComponent();
            this.ShowInTaskbar = false;
        }

        private void StructureColumnRendererSettings_Disposed(object sender, EventArgs e)
        {
            if (this.structureControl != null)
            {
                this.structureControl.Dispose();
                this.structureControl = null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureColumnRendererSettings"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public StructureColumnRendererSettings(StructureColumnRendererModel model) : this()
        {
            this.model = model;
            this.stringType = model.StringType;
            this.rendererType = RendererIdentifierConverter.ToControlType(model.TypeId);

            this.InitializeHydrogensComboBox(model.ShowHydrogens);
            this.InitializeStringTypeControls();

            this.UpdateTitle(this.rendererType);
            this.InitHelp(model.Context.GetService<ModulesService>());

            this.AdjustUI(this.rendererType, model.ChemDrawStyleFileName);

            this.Disposed += new EventHandler(StructureColumnRendererSettings_Disposed);
        }

        internal StructureColumnRendererSettings(
            ControlType rendererType, 
            StructureStringType stringType, 
            string hydrogensMode, 
            ModulesService modulesService,
            string renderStyleFileName)
        {
            this.InitializeComponent();
            this.ShowInTaskbar = false;

            this.rendererType = rendererType;
            this.stringType = stringType;
            this.showHydrogens = hydrogensMode;

            this.InitializeHydrogensComboBox(hydrogensMode);
            this.InitializeStringTypeControls();

            this.UpdateTitle(rendererType);
            this.InitHelp(modulesService);

            this.AdjustUI(rendererType, renderStyleFileName);

            this.Disposed += new EventHandler(StructureColumnRendererSettings_Disposed);
        }

        #endregion

        #region Properties

        internal string ShowHydrogens
        {
            get
            {
                return this.showHydrogens;
            }
        }

        internal StructureStringType StringType
        {
            get
            {
                return this.stringType;
            }
        }

        /// <summary>
        /// The control which is used to load template and get settings
        /// </summary>
        internal StructureControlBase StructureControl
        {
            get
            {
                if (this.structureControl == null)
                {
                    this.structureControl = StructureControlFactory.Create(this.rendererType, false);
                    //CID:20267
                    if (this.structureControl != null)
                    {
                        this.structureControl.Init(null);
                    }
                }
                return this.structureControl;
            }
        }

        /// <summary>
        /// The name of the render settings' style file
        /// </summary>
        internal string StyleFileName
        {
            get
            {
                return styleFileName;
            }
        }

        /// <summary>
        /// Render Settings
        /// </summary>
        internal RenderSettings RenderSettings
        {
            get
            {
                return renderSettings;
            }
        }

        #endregion

        #region Methods

        private void CHIMECheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CHIMECheckBox.Checked)
            {
                this.stringType = StructureStringType.CHIME;
            }
        }

        private void HelpBut_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(
                this, this.helpProvider.HelpNamespace);
        }

        private void InitHelp(ModulesService modulesServices)
        {
            // Use ModulesService to map the resourceName to an absolute path to the help file.
            string absolutePathToHelpFile = null;

            if (modulesServices != null)
            {
                absolutePathToHelpFile = modulesServices.GetResourcePath(Resources.HelpRendererSettings);
            }

            if (!string.IsNullOrEmpty(absolutePathToHelpFile))
            {
                this.helpProvider.HelpNamespace = absolutePathToHelpFile;
                this.helpProvider.SetHelpKeyword(this, InvariantResources.HelpRendererSettingsTopicId);
            }
            else
            {
                this.HelpBut.Visible = false;
            }
        }

        private void InitializeHydrogensComboBox(string hydrogensMode)
        {
            this.ShowHydrogensComboBox.Items.Clear();

            //use the property "StructureControl" to replace the temporary variable "control"
            /*
            using (StructureControlBase control = StructureControlFactory.Create(this.rendererType, false))
            {
                control.Init(null);

                string[] values = control.HydrogenDisplayModes;
                string closestHydrogenDisplayMode = control.GetClosestHydrogenDisplayMode(hydrogensMode);
                if (values != null)
                {
                    foreach (string value in values)
                    {
                        this.ShowHydrogensComboBox.Items.Add(value);
                        if (string.Equals(value, closestHydrogenDisplayMode))
                        {
                            this.ShowHydrogensComboBox.SelectedItem = value;
                        }
                    }
                }

                this.CHIMERadioButton.Enabled =
                    StructureControlFactory.ControlTypeUnderstandsContentType(this.rendererType, "chime");
            }
            */
            string[] values = this.StructureControl.HydrogenDisplayModes;
            string closestHydrogenDisplayMode = this.StructureControl.GetClosestHydrogenDisplayMode(hydrogensMode);
            if (values != null)
            {
                foreach (string value in values)
                {
                    this.ShowHydrogensComboBox.Items.Add(value);
                    if (string.Equals(value, closestHydrogenDisplayMode))
                    {
                        this.ShowHydrogensComboBox.SelectedItem = value;
                    }
                }
            }

            this.CHIMECheckBox.Enabled =
                StructureControlFactory.ControlTypeUnderstandsContentType(this.rendererType, "chime");
        }

        private void InitializeStringTypeControls()
        {
            this.CHIMECheckBox.Checked = this.stringType == StructureStringType.CHIME;          
        }


        private void OKButton_Click(object sender, EventArgs e)
        {
            if (this.model != null)
            {
                // in case of Column Renderers processing
                this.model.StringType = this.stringType;

                if (this.ShowHydrogensComboBox.SelectedItem != null)
                {
                    this.model.ShowHydrogens = this.ShowHydrogensComboBox.SelectedItem.ToString();
                }

                if (this.renderSettings != null && this.rendererType == ControlType.ChemDraw)
                {
                    this.model.ChemDrawStyleFileName = this.styleFileName;
                    this.model.ChemDrawRenderSettings = ChemDrawUtilities.Base64SerializeChemDrawRenderSettings((ChemDrawRenderSettings)this.renderSettings);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void ShowHydrogensComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.showHydrogens = this.ShowHydrogensComboBox.SelectedItem.ToString();
        }

        private void UpdateTitle(ControlType rendererType)
        {
            if (rendererType == ControlType.Accord)
            {
                this.Text = string.Format(CultureInfo.InvariantCulture, Resources.SettingsTitle, InvariantResources.Accord);
            }
            else if (rendererType == ControlType.ChemDraw)
            {
                this.Text = string.Format(CultureInfo.InvariantCulture, Resources.SettingsTitle, InvariantResources.ChemDraw);
            }
            else if (rendererType == ControlType.Marvin)
            {
                this.Text = string.Format(CultureInfo.InvariantCulture, Resources.SettingsTitle, InvariantResources.Marvin);
            }
            else if (rendererType == ControlType.MdlDraw)
            {
                this.Text = string.Format(CultureInfo.InvariantCulture, Resources.SettingsTitle, InvariantResources.SymyxDraw);
            }
            else if (rendererType == ControlType.ChemIQ)
            {
                this.Text = string.Format(CultureInfo.InvariantCulture, Resources.SettingsTitle, InvariantResources.ChemIQ);
            }
        }

        private static string GetRegistryStringValue(RegistryKey root, string keyName, string propertyName)
        {
            string result = null;
            if (root != null && keyName != null && propertyName != null)
            {
                RegistryKey key = null;
                try
                {
                    key = root.OpenSubKey(keyName);
                    if (key != null)
                    {
                        result = key.GetValue(propertyName) as string;
                    }
                }
                finally
                {
                    if (key != null)
                    {
                        key.Close();
                    }
                }
            }
            return result;
        }

        private static string FindCorrectPath()
        {
            string defaultPath = string.Empty;
            Guid result = Guid.Empty;
            bool isInstalled = ChemDrawUtilities.TryGetClassIdFromProgId("ChemDrawControlConst11.ChemDrawCtl", out result);
            if (isInstalled && !result.Equals(Guid.Empty))
            {
                RegistryKey classesRoot = Registry.ClassesRoot;
                string keyName = (Environment.Is64BitOperatingSystem ? "Wow6432Node\\" : "") + "CLSID\\{" + result + "}\\InprocServer32";
                string value = GetRegistryStringValue(classesRoot, keyName, string.Empty);
                if (value != null)
                {
                    string parentDirectoryString = string.Empty;
                    try
                    {
                        DirectoryInfo parentDirectory = Directory.GetParent(Path.GetDirectoryName(value));
                        if (parentDirectory != null)
                        {
                            parentDirectoryString = parentDirectory.FullName;
                        }
                    }
                    catch { }
                    if (!string.IsNullOrEmpty(parentDirectoryString))
                    {
                        string defaultPathTry = parentDirectoryString + "\\ChemDrawConst\\ChemDraw Items";
                        if (Directory.Exists(defaultPathTry))
                        {
                            defaultPath = defaultPathTry;
                        }
                        else
                        {
                            value = GetRegistryStringValue(classesRoot, "ChemDraw.Application\\CurVer\\", string.Empty);
                            if (value != null)
                            {
                                if (value.StartsWith("ChemDraw.Application."))
                                {
                                    string ver = value.Substring("ChemDraw.Application.".Length);
                                    keyName = "SOFTWARE" + (Environment.Is64BitOperatingSystem ? "\\Wow6432Node" : "") + "\\CambridgeSoft\\ChemDraw\\" + ver + "\\General\\";
                                    value = GetRegistryStringValue(Registry.LocalMachine, keyName, "ChemDraw Items Default Path");
                                    if (value != null)
                                    {
                                        defaultPath = value;
                                    }
                                }
                            }

                        }
                    }
                }
            }
            return defaultPath;
        }

        private void LoadSettingsButton_Click(object sender, EventArgs e)
        {
            this.LoadSettingsContextMenuStrip.Show(this.LoadSettingsButton.Parent, new System.Drawing.Point(this.LoadSettingsButton.Right, this.LoadSettingsButton.Top));
        }

        private void AdjustUI(ControlType rendererType, string styleFileName)
        {
            if (rendererType == ControlType.ChemDraw)
            {
                this.OtherPanel.Visible = false;
                this.Height -= this.OtherPanel.Height;
                //init label for the file name
                if (!string.IsNullOrEmpty(styleFileName))
                {
                    this.TemplateSettingsLabel.Text = "'" + styleFileName + "'"+ " was loaded.";
                }
                //init context menu
                ToolStripItem item = this.LoadSettingsContextMenuStrip.Items.Add("Default settings");
                item.Click += this.DefaultToolStripMenuItem_Click;
                item = this.LoadSettingsContextMenuStrip.Items.Add("Other...");
                item.Click += this.ToolStripMenuItem_Click;
                this.LoadSettingsContextMenuStrip.Items.Add(new ToolStripSeparator());
                string[] fileFullNames = null;
                try
                {
                    string path = FindCorrectPath();
                    if (!string.IsNullOrEmpty(path))
                    {
                        fileFullNames = Directory.GetFiles(path, "*.cds");
                    }
                }
                catch
                {
                }
                if (fileFullNames != null)
                {
                    foreach (string fileFullName in fileFullNames)
                    {
                        string noExtensionName = Path.GetFileNameWithoutExtension(fileFullName);
                        item = this.LoadSettingsContextMenuStrip.Items.Add(noExtensionName);
                        item.Tag = fileFullName;
                        item.Click += this.ToolStripMenuItem_Click;
                    }
                }
            }
            else
            {
                this.ChemDrawPanel.Visible = false;
                this.Height -= this.ChemDrawPanel.Height;
                //disable ui
                this.LoadSettingsButton.Visible = false;
                this.LoadSettingsButton.Enabled = false;
                this.LoadSettingsButton.TabStop = false;
                this.TemplateSettingsLabel.Visible = false;
                this.TemplateSettingsLabel.Enabled = false;
            }
        }

        private void DefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.renderSettings = ChemDrawRenderSettings.GetDefaultSettings();
            this.styleFileName = string.Empty;
            this.TemplateSettingsLabel.Text = new System.ComponentModel.ComponentResourceManager(typeof(StructureColumnRendererSettings)).GetString("TemplateSettingsLabel.Text");
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (((ToolStripItem)sender).Tag == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Loading Settings";
                openFileDialog.Filter = "CS ChemDraw Style Sheet|*.cds";

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.LoadRenderSettingsProcess(openFileDialog.FileName);
                }
            }
            else
            {
                this.LoadRenderSettingsProcess(((ToolStripItem)sender).Tag.ToString());
            }
        }

        private void LoadRenderSettingsProcess(string fileFullName)
        {
            this.StructureControl.LoadRenderSettings(fileFullName);
            this.renderSettings = this.StructureControl.RenderSettings;
            string fileName = Path.GetFileNameWithoutExtension(fileFullName);
            this.styleFileName = fileName;
            this.TemplateSettingsLabel.Text = "'"+fileName +"'"+ " was loaded.";
        }

        #endregion
    }
}
