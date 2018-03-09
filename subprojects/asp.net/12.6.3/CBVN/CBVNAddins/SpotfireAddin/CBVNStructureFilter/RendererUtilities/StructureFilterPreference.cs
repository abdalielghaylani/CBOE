// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureFilterPreference.cs" company="PerkinElmer Inc.">
//   Copyright © 2013 PerkinElmer Inc. 
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
    using System.Collections.Generic;

    using Spotfire.Dxp.Application.Extension;
    using Spotfire.Dxp.Framework.Persistence;
    using Spotfire.Dxp.Framework.Preferences;
    using CBVNStructureFilterSupport.ChemDraw;
    using CBVNStructureFilterSupport.Framework;

    [PersistenceVersion(9, 0)]
    internal class StructureFilterPreference : CustomPreference
    {
        #region Constants and Fields

        /// <summary>The percentage visibility for the the upper section of the full panel. Between 0 and 1.
        /// </summary>
        private readonly PreferenceProperty<float> listPanelVisibilityPortion;

        /// <summary>The number of columns to show in the lower pane (structures)
        /// </summary>
        private readonly PreferenceProperty<int> numberOfStructureColumns;

        /// <summary>The renderer type that is used.
        /// </summary>
        private readonly PreferenceProperty<int> rendererType;
        /// <summary>
        /// The editor type that is used.
        /// </summary>
        private readonly PreferenceProperty<int> editorType;

        /// <summary>The common setting for the "Show Hydrogens".
        /// </summary>
        private readonly PreferenceProperty<string> showHydrogens;

        private readonly PreferenceProperty<Dictionary<ControlType, string>> showHydrogensPerRenderer;

        private PreferenceProperty<bool> rememberHideManyRowsWarning;

        /// <summary>
        /// the rendering settings used by renderer
        /// </summary>
        private readonly PreferenceProperty<Dictionary<string, string>> renderSettingsPerRenderer;

        /// <summary>
        /// the name of rendering settings style file
        /// </summary>
        private readonly PreferenceProperty<Dictionary<string, string>> renderSettingsFileNamePerRenderer;

        #endregion

        #region Constructors and Destructors

        public StructureFilterPreference()
        {
            this.rememberHideManyRowsWarning =
                this.AddPreference(
                    new PreferenceProperty<bool>(
                        "rememberHideManyRowsWarning", 
                        "1.4", 
                        PreferencePersistenceScope.Local, 
                        PreferenceUsage.SingleUser, 
                        (bool) false));

            // Must be local because another machine might not have the same renderers
            this.rendererType =
                this.AddPreference(
                    new PreferenceProperty<int>(
                        "rendererType", 
                        "1.2", 
                        PreferencePersistenceScope.Local, 
                        PreferenceUsage.SingleUser, 
                        (int)ControlType.None));

            this.editorType =
                this.AddPreference(
                    new PreferenceProperty<int>(
                        "editorType", "1.2",
                        PreferencePersistenceScope.Local,
                        PreferenceUsage.SingleUser,
                        (int) ControlType.None));

            this.showHydrogens =
                this.AddPreference(
                    new PreferenceProperty<string>(
                        "showHydrogens", "1.2", PreferencePersistenceScope.Server, PreferenceUsage.SingleUser, "Off"));

            this.showHydrogensPerRenderer =
                this.AddPreference(
                    new PreferenceProperty<Dictionary<ControlType, string>>(
                        "ShowHydrogensPerRenderer", 
                        "1.4", 
                        PreferencePersistenceScope.Server, 
                        PreferenceUsage.SingleUser, 
                        () => new Dictionary<ControlType, string>()));

            this.numberOfStructureColumns =
                this.AddPreference(
                    new PreferenceProperty<int>(
                        "NumberOfStructureColumns", 
                        "1.4", 
                        PreferencePersistenceScope.Server, 
                        PreferenceUsage.SingleUser, 
                        1));

            this.listPanelVisibilityPortion =
                this.AddPreference(
                    new PreferenceProperty<float>(
                        "ListPanelVisibilityPortion", 
                        "1.4", 
                        PreferencePersistenceScope.Server, 
                        PreferenceUsage.SingleUser, 
                        0.5f, 
                        0.0f, 
                        1.0f));

            this.renderSettingsPerRenderer =
                this.AddPreference(
                    new PreferenceProperty<Dictionary<string, string>>(
                        "RenderSettingsPerRenderer",
                        "1.0",
                        PreferencePersistenceScope.Server,
                        PreferenceUsage.SingleUser,
                        () => new Dictionary<string, string>()));

            this.renderSettingsFileNamePerRenderer =
                this.AddPreference(
                    new PreferenceProperty<Dictionary<string, string>>(
                        "RenderSettingsFileNamePerRenderer",
                        "1.0",
                        PreferencePersistenceScope.Server,
                        PreferenceUsage.SingleUser,
                        () => new Dictionary<string, string>()));
        }

        #endregion

        #region Properties

        public override string Category
        {
            get
            {
                return "StructureFilter";
            }
        }

        /// <summary>The percentage visibility for the the upper section of the full panel. Between 0 and 1.
        /// </summary>
        public float ListPanelVisibilityPortion
        {
            get
            {
                return this.listPanelVisibilityPortion.Value;
            }

            set
            {
                this.listPanelVisibilityPortion.Value = value;
            }
        }

        public PreferenceProperty<bool> RememberHideManyRowsWarning
        {
            get
            {
                return this.rememberHideManyRowsWarning;
            }

            set
            {
                this.rememberHideManyRowsWarning = value;
            }
        }

        public override string SubCategory
        {
            get
            {
                return "View";
            }
        }

        internal int NumberOfStructureColumns
        {
            get
            {
                return this.numberOfStructureColumns.Value;
            }

            set
            {
                this.numberOfStructureColumns.Value = value;
            }
        }

        internal ControlType RendererType
        {
            get
            {
                return (ControlType)this.rendererType.Value;
            }

            set
            {
                this.rendererType.Value = (int)value;
            }
        }

        internal ControlType EditorType
        {
            get 
            { 
                return (ControlType)this.editorType.Value;
            }
            set 
            { 
                this.editorType.Value = (int) value;
            }
        }
        
        private string ShowHydrogens
        {
            get
            {
                return this.showHydrogens.Value;
            }

            set
            {
                this.showHydrogens.Value = value;
            }
        }

        private Dictionary<ControlType, string> ShowHydrogensPerRenderer
        {
            get
            {
                return this.showHydrogensPerRenderer.Value;
            }
        }

        private Dictionary<string, string> RenderSettingsPerRenderer
        {
            get
            {
                return this.renderSettingsPerRenderer.Value;
            }
        }

        private Dictionary<string, string> RenderSettingsFileNamePerRenderer
        {
            get
            {
                return this.renderSettingsFileNamePerRenderer.Value;
            }
        }

        #endregion

        #region Methods

        internal string GetShowHydrogens(ControlType type)
        {
            Dictionary<ControlType, string> hydrogensPerRenderer = this.ShowHydrogensPerRenderer;
            if (hydrogensPerRenderer.ContainsKey(type))
            {
                return hydrogensPerRenderer[type];
            }

            return this.ShowHydrogens;
        }

        internal void SetShowHydrogens(ControlType type, string hydrogenDisplayMode)
        {
            Dictionary<ControlType, string> hydrogensPerRenderer = this.ShowHydrogensPerRenderer;
            hydrogensPerRenderer[type] = hydrogenDisplayMode;
            this.showHydrogensPerRenderer.Value = hydrogensPerRenderer;
            this.showHydrogensPerRenderer.OnValueChanged();
            this.ShowHydrogens = hydrogenDisplayMode;
        }

        internal string GetRenderSettings(string type)
        {
            string result = string.Empty;
            Dictionary<string, string> settings = this.RenderSettingsPerRenderer;
            if (type != null && settings.ContainsKey(type))
            {
                result = settings[type];
            }

            return string.IsNullOrEmpty(result) ? ChemDrawUtilities.DefaultRenderSettingsString : result;
        }

        internal void SetRenderSettings(string type, string renderSettingsMode)
        {
            if (type != null)
            {
                Dictionary<string, string> settings = this.RenderSettingsPerRenderer;
                settings[type] = renderSettingsMode;
                this.renderSettingsPerRenderer.Value = settings;
                this.renderSettingsPerRenderer.OnValueChanged();
            }
        }

        internal string GetRenderStyleFileName(string type)
        {
            Dictionary<string, string> settings = this.RenderSettingsFileNamePerRenderer;
            if (type != null && settings.ContainsKey(type))
            {
                return settings[type];
            }

            return string.Empty;
        }

        internal void SetRenderStyleFileName(string type, string renderSettingsFileNameMode)
        {
            if (type != null)
            {
                Dictionary<string, string> settings = this.RenderSettingsFileNamePerRenderer;
                settings[type] = renderSettingsFileNameMode;
                this.renderSettingsFileNamePerRenderer.Value = settings;
                this.renderSettingsFileNamePerRenderer.OnValueChanged();
            }
        }

        #endregion
    }
}
