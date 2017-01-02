// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureColumnRendererModel.cs" company="PerkinElmer Inc.">
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
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Windows.Forms;

    using Spotfire.Dxp.Application;
    using Spotfire.Dxp.Application.Extension;
    using Spotfire.Dxp.Framework.ApplicationModel;
    using Spotfire.Dxp.Framework.DocumentModel;
    using Spotfire.Dxp.Framework.Persistence;
    using Spotfire.Dxp.Framework.Preferences;
    using CBVNStructureFilterSupport.ChemDraw;
    using CBVNStructureFilterSupport.Framework;
    using CBVNStructureFilterSupport;        

    [Serializable]
    [PersistenceVersion(9, 0)]
    internal sealed class StructureColumnRendererModel : CustomValueRendererSettings
    {
        #region Constants and Fields

        private readonly UndoableProperty<string> showHydrogens;

        private readonly UndoableProperty<StructureStringType> stringType;

        /// <summary>
        /// the field which is used to store the ChemDrawRenderSettings
        /// </summary>
        private readonly UndoableProperty<string> chemDrawRenderSettings;

        /// <summary>
        /// the name for ChemDrawRenderSettings
        /// </summary>
        public static string ChemDrawRenderSettingsName = "ChemDrawRenderSettings";

        /// <summary>
        /// the field which is used to store the name of ChemDraw style file
        /// </summary>
        private readonly UndoableProperty<string> chemDrawStyleFileName;

        /// <summary>
        /// the name for ChemDrawStyleFileName
        /// </summary>
        public static string ChemDrawStyleFileNameName = "ChemDrawStyleFileName";

        /// <summary>
        /// stattic member for auto change renderer preference
        /// </summary>
        static AutoChangeRendererPreference preference = null;

        /// <summary>
        /// static member for applciation thread
        /// </summary>
        static ApplicationThread applicationThread = null;

        /// <summary>
        /// static member for current applciation instance
        /// </summary>
        static AnalysisApplication application = null;

        #endregion

        #region Constructors and Destructors

        public StructureColumnRendererModel()
        {
            this.CreateProperty(PropertyNames.StringType, out this.stringType, StructureStringType.Molfile);
            this.CreateProperty(PropertyNames.ShowHydrogens, out this.showHydrogens, "Off");
            // Create the ChemDrawRenderSettings Property
            this.CreateProperty(PropertyNames.ChemDrawRenderSettings, out this.chemDrawRenderSettings, ChemDrawUtilities.DefaultRenderSettingsString);
            this.CreateProperty(PropertyNames.ChemDrawStyleFileName, out this.chemDrawStyleFileName, string.Empty);
        }

        /// <summary>Implements ISerializable.</summary>
        internal StructureColumnRendererModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.DeserializeProperty(info, context, PropertyNames.StringType, out this.stringType);
            this.DeserializeProperty(info, context, PropertyNames.ShowHydrogens, out this.showHydrogens);
            // if ChemDrawRenderSettings doesn't exist in the SerializationInfo, a default settings will be added.
            if (!this.CheckPropertyExistInSerializationInfo(info, ChemDrawRenderSettingsName))
            {
                info.AddValue(ChemDrawRenderSettingsName, ChemDrawUtilities.DefaultRenderSettingsString);
                info.AddValue(ChemDrawStyleFileNameName, string.Empty);
            }
            this.DeserializeProperty(info, context, PropertyNames.ChemDrawRenderSettings, out this.chemDrawRenderSettings);
            this.DeserializeProperty(info, context, PropertyNames.ChemDrawStyleFileName, out this.chemDrawStyleFileName);

            // [2013-1-17, JIRA LD-10]
            // validate structure renderer
            ValidateStructureRenderer();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Return a single key usable for comparing two instances of this class
        /// </summary>
        public string ModelKey
        {
            get
            {
                //var result = string.Format(CultureInfo.InvariantCulture, "{0},{1}", this.StringType, this.ShowHydrogens);
                string result = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", this.StringType, this.ShowHydrogens, this.ChemDrawRenderSettings);
                return result;
            }
        }

        public StructureStringType StringType
        {
            get
            {
                return this.stringType.Value;
            }

            set
            {
                this.stringType.Value = value;
            }
        }

        internal string ShowHydrogens
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

        /// <summary>
        /// ChemDrawRenderSettings
        /// </summary>
        public string ChemDrawRenderSettings
        {
            get
            {
                string result = this.chemDrawRenderSettings.Value;
                return string.IsNullOrEmpty(result) ? ChemDrawUtilities.DefaultRenderSettingsString : result;
            }

            set
            {
                this.chemDrawRenderSettings.Value = value;
            }
        }

        /// <summary>
        /// ChemDrawStyleFileName
        /// </summary>
        public string ChemDrawStyleFileName
        {
            get
            {
                return this.chemDrawStyleFileName.Value;
            }

            set
            {
                this.chemDrawStyleFileName.Value = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>Implements ISerializable.</summary>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            this.SerializeProperty(info, context, this.stringType);
            this.SerializeProperty(info, context, this.showHydrogens);
            // serialize the ChemDrawRenderSettings
            this.SerializeProperty(info, context, this.chemDrawRenderSettings);
            this.SerializeProperty(info, context, this.chemDrawStyleFileName);
        }

        protected override Trigger GetRenderTriggerCore()
        {
            return Trigger.CreateCompositeTrigger(
                Trigger.CreatePropertyTrigger(this, PropertyNames.ShowHydrogens),
                //Trigger.CreatePropertyTrigger(this, PropertyNames.StringType));
                Trigger.CreatePropertyTrigger(this, PropertyNames.StringType),
                // trigger the re-rendering of the renderer
                Trigger.CreatePropertyTrigger(this, PropertyNames.ChemDrawRenderSettings));
        }

        protected override void OnConfigured()
        {
            base.OnConfigured();
        }


        /// <summary>
        /// Validate the structure renderer in analysis file
        /// if it is not installed in computer
        /// LD can choose one default renderer to render the structure
        /// </summary>
        private void ValidateStructureRenderer()
        {
            try
            {
                // [bbao, 2013-1-10] JIRA LD-10
                // Automatically change the uninstalled renderer to installed one
                ControlType controlType = RendererIdentifierConverter.ToControlType(this.TypeId);
                if (!StructureControlFactory.IsControlTypeInstalled(controlType) &&
                    DefaultStructureRenderer.HasDefaultStructureRenderer)
                {
                    // LD-994, fix prompt dialog in Spotfire login dialog when we set one content-type to one non-installed renderer
                    if (application == null)
                        application = this.GetService(typeof(AnalysisApplication)) as AnalysisApplication;

                    // LD-1041, use ImportContext property to check wether Spotfire is opened.
                    // After Spotfire app is opened, this property has value
                    if (application == null || application.ImportContext == null)
                    {
                        return;
                    }

                    if (preference == null)
                    {
                        PreferenceManager preferenceManager = this.GetService(typeof(PreferenceManager)) as PreferenceManager;
                        if (preferenceManager != null)
                            preference = preferenceManager.GetPreference<AutoChangeRendererPreference>();
                    }

                    if (applicationThread == null)
                        applicationThread =
                            (this.GetService(typeof(ApplicationThread)) as ApplicationThread);

                    // get the stored preference values
                    bool skipPrompt = preference.SkipPrompt;
                    AutoChangeRendererPreference.AutoChangeRendererDialogResult lastPromptResult =
                        preference.LastPromptResult;

                    if (!skipPrompt &&
                        lastPromptResult == AutoChangeRendererPreference.AutoChangeRendererDialogResult.None &&
                        Environment.UserInteractive == true /* for WebPlayer, do not prompt dialog*/)
                    {

                        // show the prompt with one checkbox in application thread
                        applicationThread.Invoke(delegate
                        {
                            if (CheckableMessageBox.Show(string.Format(Properties.Resources.AutoChangeRendererText, DefaultStructureRenderer.DefaultRendererTypeIdentifier.DisplayName),
                                                         Properties.Resources.Warning,
                                                         Properties.Resources.AutoChangeRendererSkipText,
                                                         CheckableMessageBox.MessageBoxButtons.YesNo,
                                                         CheckableMessageBox.MessageBoxIcons.Information,
                                                         ref skipPrompt) == DialogResult.Yes)
                            {
                                lastPromptResult = AutoChangeRendererPreference.AutoChangeRendererDialogResult.Yes;
                            }
                            else
                            {
                                lastPromptResult = AutoChangeRendererPreference.AutoChangeRendererDialogResult.No;
                            }
                        });

                        // get the referernce value and result from prompt dialog 
                        // then save back to preference
                        preference.LastPromptResult = lastPromptResult;
                        preference.SkipPrompt = skipPrompt;
                        preference.Save();
                    }

                    if (lastPromptResult != AutoChangeRendererPreference.AutoChangeRendererDialogResult.No)
                    {
                        // use reflection to get the internal set() method of property "TypeId"
                        MethodInfo setTypeId = this.GetType().GetMethod("set_TypeId", BindingFlags.NonPublic | BindingFlags.Instance);

                        if (setTypeId != null)
                        {
                            setTypeId.Invoke(this, new object[] { DefaultStructureRenderer.DefaultRendererTypeIdentifier });
                        }
                    }
                }
            }
            catch { } // just skip the exception when DocumentNode status is Snapshot
        }

        /// <summary>
        /// Check whether the property exists in SerializationInfo or not
        /// </summary>
        /// <param name="info">the SerializationInfo</param>
        /// <param name="propertyName">the property name</param>
        /// <returns>true if exists, false otherwise</returns>
        private bool CheckPropertyExistInSerializationInfo(SerializationInfo info, string propertyName)
        {
            SerializationInfoEnumerator enumerator = info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Name.Equals(propertyName))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        public new abstract class PropertyNames : CustomValueRendererSettings.PropertyNames
        {
            #region Constants and Fields

            internal static readonly PropertyName ShowHydrogens = CreatePropertyName("ShowHydrogens");

            internal static readonly PropertyName StringType = CreatePropertyName("StringType");

            /// <summary>
            /// the PropertyName which is used in the model
            /// </summary>
            internal static readonly PropertyName ChemDrawRenderSettings = CreatePropertyName(ChemDrawRenderSettingsName);

            /// <summary>
            /// the PropertyName which is used in the model
            /// </summary>
            internal static readonly PropertyName ChemDrawStyleFileName = CreatePropertyName(ChemDrawStyleFileNameName);

            #endregion
        }


        /// <summary>
        /// [2013-1-10] JIRA LD-10
        /// Nested class for default structure renderer type identifier
        /// </summary>
        internal class DefaultStructureRenderer
        {

            /// <summary>
            /// Get current LD default structure renderer type identifier
            /// </summary>
            public static readonly TypeIdentifier DefaultRendererTypeIdentifier = null;


            /// <summary>
            /// Get value whether current LD installed at least one structure renderer
            /// </summary>
            public static readonly bool HasDefaultStructureRenderer = false;

            static DefaultStructureRenderer()
            {
                if (StructureControlFactory.IsControlTypeInstalled(ControlType.ChemDraw))
                {
                    DefaultRendererTypeIdentifier = ChemDrawRendererIdentifiers.ChemDrawRenderer;
                    HasDefaultStructureRenderer = true;
                }
                else if (StructureControlFactory.IsAnyControlTypeInstalled)
                {
                    // pick the first installed one as default renderer
                    foreach (ControlType controlType in Enum.GetValues(typeof(ControlType)))
                    {
                        if (StructureControlFactory.IsControlTypeInstalled(controlType))
                        {
                            DefaultRendererTypeIdentifier = RendererIdentifierConverter.ToCustomTypeIdentifier(controlType);
                            HasDefaultStructureRenderer = true;
                            break;
                        }
                    }
                }
            }

        }

    }
}
