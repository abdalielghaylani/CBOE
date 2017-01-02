namespace SpotfireIntegration.SpotfireAddin
{
    using System.Windows.Forms;
    using Spotfire.Dxp.Application;
    using Spotfire.Dxp.Application.Extension;
    using Spotfire.Dxp.Data;
    using COEServiceLib;
    using System;

    /// <summary>Add-In implementation for the CustomMitchFileDataSource project.
    /// </summary>
    public sealed class SpotfireCOETableDataSourceAddIn : AddIn
    {
        internal static string LDInstallationPath = string.Empty;

        public SpotfireCOETableDataSourceAddIn()
        {
        }
        #region Methods

        protected override void RegisterUserServices(AddIn.ServiceRegistrar registrar)
        {
            base.RegisterUserServices(registrar);

            registrar.Register<COEService>(new COEService());
        }

        /// <summary>Registers the data sources in this add-in.
        /// </summary>
        /// <param name="registrar">The registrar that is used to register data sources.</param>
        protected override void RegisterDataSources(DataSourceRegistrar registrar)
        {
            base.RegisterDataSources(registrar);

            //registrar.Register<SpotfireCOETableDataSource>(SpotfireCOEDataSourceTypeIdentifiers.SpotfireCOETableDataSource);
        }

        protected override void RegisterDataTransformations(AddIn.DataTransformationRegistrar registrar)
        {
            base.RegisterDataTransformations(registrar);

            registrar.Register<COEMetadataTransformation>(SpotfireCOEDataSourceTypeIdentifiers.COEMetadataTransformation);
        }

        protected override void RegisterProperties(AddIn.PropertyRegistrar registrar)
        {
            base.RegisterProperties(registrar);

            registrar.Register(SpotfireCOEDataSourceTypeIdentifiers.COEColumnID_Property,
                DataPropertyClass.Column, null, DataType.Integer, DataPropertyAttributes.IsVisible);
            registrar.Register(SpotfireCOEDataSourceTypeIdentifiers.COEDataView_Property,
                DataPropertyClass.Table, null, DataType.String, DataPropertyAttributes.IsVisible);
            registrar.Register(SpotfireCOEDataSourceTypeIdentifiers.COEDataViewID_Property,
                DataPropertyClass.Table, null, DataType.Integer, DataPropertyAttributes.IsVisible);
            registrar.Register(SpotfireCOEDataSourceTypeIdentifiers.COEHitListID_Property,
                DataPropertyClass.Table, null, DataType.Integer, DataPropertyAttributes.IsVisible);
            registrar.Register(SpotfireCOEDataSourceTypeIdentifiers.COEHitListType_Property,
                DataPropertyClass.Table, null, DataType.Integer, DataPropertyAttributes.IsVisible);
            registrar.Register(SpotfireCOEDataSourceTypeIdentifiers.COEResultsCriteria_Property,
                DataPropertyClass.Table, null, DataType.String, DataPropertyAttributes.IsVisible);
            registrar.Register(SpotfireCOEDataSourceTypeIdentifiers.COETableID_Property,
                DataPropertyClass.Table, null, DataType.Integer, DataPropertyAttributes.IsVisible);
            registrar.Register(SpotfireCOEDataSourceTypeIdentifiers.COETablePK_Property,
                DataPropertyClass.Table, null, DataType.Integer, DataPropertyAttributes.IsVisible);
            registrar.Register(SpotfireCOEDataSourceTypeIdentifiers.COESearchCriteria_Property,
                DataPropertyClass.Table, null, DataType.String, DataPropertyAttributes.IsVisible);
            //register the search criteria field order data table property
            registrar.Register(SpotfireCOEDataSourceTypeIdentifiers.COESearchCriteriaFieldOrder_Property,
                DataPropertyClass.Table, null, DataType.String, DataPropertyAttributes.None);
            //register the search criteria field order data table property
            registrar.Register(SpotfireCOEDataSourceTypeIdentifiers.FilterChildHits_Property,
                DataPropertyClass.Table, null, DataType.Boolean, DataPropertyAttributes.None);

            //register the coe connection information as document property
            registrar.Register(SpotfireCOEDataSourceTypeIdentifiers.COEConnectionInfo_Property,
                DataPropertyClass.Document, null, DataType.String, DataPropertyAttributes.None);
        }

        protected override void RegisterTools(AddIn.ToolRegistrar registrar)
        {
            base.RegisterTools(registrar);
           
            registrar.Register(new QueryCriteriaEditorTool());
            registrar.Register(new CBVNMarkingTool());
            //registrar.Register(new ResultsCriteriaEditorTool());
#if DISABLED
            if (false) // (Properties.Settings.Default.EnableResearchAssayHistoryTool)
            {
                registrar.Register(new ResearchAssayHistoryTool());
            }
#endif
        }

        protected override void RegisterViews(AddIn.ViewRegistrar registrar)
        {
            base.RegisterViews(registrar);

            registrar.Register(typeof(Form), typeof(SpotfireCOEAuthenticationPromptModel), typeof(SpotfireCOEAuthenticationPromptView));
            registrar.Register(typeof(Form), typeof(COEHitListPromptModel), typeof(COEHitListPromptView));
            registrar.Register(typeof(Form), typeof(COEResultsCriteriaPromptModel), typeof(COEResultsCriteriaPromptView));
        }

        protected override void OnAnalysisServicesRegistered(ServiceProvider serviceProvider)
        {
            base.OnAnalysisServicesRegistered(serviceProvider);

            // Start WCF service host
            AnalysisApplication application = serviceProvider.GetService<AnalysisApplication>();
            CBVNController.GetInstance().StartService(application);
       }

        protected override void RegisterPreferences(PreferenceRegistrar registrar)
        {
            base.RegisterPreferences(registrar);
            registrar.Register<CustomPreferences>();
        }

        /// <summary>
        /// Registers the Datalytix help in Additional Help Topics menu
        /// </summary>
        /// <param name="registrar">HelpRegistrar object</param>
        protected override void RegisterHelp(HelpRegistrar registrar)
        {
            base.RegisterHelp(registrar);

            registrar.Register(SpotfireIntegration.SpotfireAddin.Properties.Resources.HelpMenuText, FormWizard.Properties.Resources.HelpResourceName, FormWizard.Properties.Resources.HelpDefaultTopicId);
        }

        #endregion


    }
}