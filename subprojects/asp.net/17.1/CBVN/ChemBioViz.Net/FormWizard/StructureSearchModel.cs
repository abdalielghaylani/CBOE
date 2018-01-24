using System;
using Spotfire.Dxp.Application;
using Spotfire.Dxp.Framework.ApplicationModel;
using Spotfire.Dxp.Framework.DocumentModel;
using CBVNStructureFilter;

using System.Text;


namespace FormWizard
{
    internal class StructureSearchModel : IStructureEditModel, IStructureFilterModel, IStructureSearchModel //DocumentNode,
    {
        #region Variables

        /// <summary>
        /// Variable holds an default Similarity percent value
        /// </summary>
        private const int DefaultPercentSimilarity = 90;

        /// <summary>
        /// Variable holds an FilterMode or Search type object
        /// </summary>
        private const StructureFilterSettings.FilterModeEnum DefaultSearchType = StructureFilterSettings.FilterModeEnum.SubStructure;

        /// <summary>
        /// Variable holds an SimularityMode object
        /// </summary>
        private const StructureFilterSettings.SimularityModeEnum DefaultSimilarityMode = StructureFilterSettings.SimularityModeEnum.GreaterThan;

        /// <summary>
        /// Variable hodls an default Show Hydrogens value
        /// </summary>
        private const string DefaultShowHydrogens = "Off";

        /// <summary>
        /// Variable holds an default structure string
        /// </summary>
        private const string DefaultStructureString = "";

        /// <summary>
        /// Variable holds AnalysisApplication class object
        /// </summary>
        AnalysisApplication _theAnalysisApplication = null;

        /// <summary>
        /// Variable holds an structure data in byte array format
        /// </summary>
        private byte[] _cdxStructure;

        /// <summary>
        /// Variable holds an GUID for Model ID
        /// </summary>
        private readonly Guid _modelId;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializaes an instance of the class StructureSearchModel
        /// </summary>
        /// <param name="theAnalysisApplication">Object of AnalysisApplication class</param>
        public StructureSearchModel(AnalysisApplication theAnalysisApplication)
        {
            _theAnalysisApplication = theAnalysisApplication;

            this.RemoveRGroupDecompositionColumnsFromDocument();

            _modelId = Guid.NewGuid();
            if (!DataTableInfoMgr.ContainsKey(_modelId))
            {
                DataTableInfoMgr.Add(_modelId,_theAnalysisApplication.Document, true);
            }
        }

        #endregion

        #region Supporting Methods

        /// <summary>
        /// Method for resetting structure filter settings.
        /// </summary>
        /// <returns>An object of StructureFilterSettings class</returns>
        internal StructureFilterSettings CreateDefaultFilterSettings()
        {
            return new StructureFilterSettings(
                   String.Empty,
                   DefaultSearchType,
                   false,
                   DefaultSimilarityMode,
                   DefaultPercentSimilarity,
                   false,
                   DefaultStructureString,
                   false);
        }

        #endregion

        #region IStructureEditModel Implementation

        /// <summary>
        /// Returns the current Model ID
        /// </summary>
        public Guid ModelId
        {
            get { return _modelId; }
        }

        /// <summary>
        /// Get/Set Show Hydrogen value
        /// </summary>
        public string ShowHydrogens
        {
            get { return "no"; }
            set { }
        }

        /// <summary>
        /// Get/Set structure data in byte array format
        /// </summary>
        public byte[] CdxStructure
        {
            get
            {
                return this._cdxStructure;
            }
            set
            {
                this._cdxStructure = value;
            }
        }

        /// <summary>
        /// Get/Set structure string
        /// </summary>
        public string StructureString { get; set; }

        /// <summary>
        /// Returns DocuemntNode context
        /// </summary>
        public INodeContext NodeContext
        {
            get
            {
                return _theAnalysisApplication.Document.Context;
            }
            //Context; }
        }

        /// <summary>
        /// Method for verifying whether structure string exist or not
        /// </summary>
        /// <returns></returns>
        public bool HasStructureString()
        {
            return !string.IsNullOrEmpty(StructureString);
        }

        public bool NickNames
        {
            get
            {
                return false;
            }
            set
            {
                //throw new NotImplementedException();
            }
        }

        public bool UpdateSavedSearchList
        {
            get
            {
                return false;
            }
            set
            {
                //throw new NotImplementedException();
            }
        }

        public void SetRGroupDecomposition(bool enabled, bool nickNames)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region IStructureFilterModel Implementation

        /// <summary>
        /// Get/Set RGroupDecomposition value
        /// </summary>
        public bool RGroupDecomposition
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set RGroupStructure value
        /// </summary>
        public string RGroupStructure { get; set; }

        /// <summary>
        /// Returns StructureStringType value
        /// </summary>
        public StructureStringType RGroupStructureType
        {
            get { return StructureStringType.Unknown; }
        }

        /// <summary>
        /// Method for resetting current structure filter
        /// </summary>
        public void ClearCurrentFilter()
        {
            this._cdxStructure = null;
            this.StructureString = string.Empty;
            // _filterSettings.Value = CreateDefaultFilterSettings();
        }

        /// <summary>
        /// Method for removing RGroupDecomposition columns from Document
        /// </summary>
        public void RemoveRGroupDecompositionColumnsFromDocument()
        {
            //Implementation not required. 
        }

        public void ClearStructureAlignment()
        {
            //throw new NotImplementedException();
        }

        public void SetStructureAlignment(bool enabled)
        {
            //throw new NotImplementedException();
        }

        public void ToggleRGroupNicknames(bool enabled)
        {

        }

        public bool StructureAlignment
        {
            get { return true; }
        }

        public void ClearRGroupDecomposition(bool nickNames)
        {
            
        }

        #endregion

        #region IStructureSearchModel Implementation

        /// <summary>
        /// Get/Set Similarity percentage
        /// </summary>
        public int PercentSimilarity { get; set; }

        /// <summary>
        /// Get/Set FilterMode or Search type 
        /// </summary>
        public StructureFilterSettings.FilterModeEnum SearchType { get; set; }

        /// <summary>
        /// Method for validating Spotfire DataTable
        /// </summary>
        /// <returns>True/False</returns>
        public bool ValidDataTable()
        {
            return true; //Implementation not required. Hence, always return 'True'
        }

        /// <summary>
        /// Method for displaying Structure source configuration dialog
        /// </summary>
        public void ShowStructureSourceConfigDialog()
        {
            //Implementation not required.
        }

        /// <summary>
        /// Method for executing transaction on Spotfire
        /// </summary>
        /// <param name="executor">AN object of Executor class</param>
        public void ExecuteTransaction(Executor executor)
        {
            //Implementation not required. 
        }

        /// <summary>
        /// Method for getting an object of class ModulesService
        /// </summary>
        /// <returns>An object of class ModulesService</returns>
        public ModulesService GetModulesService()
        {
            return null; //Implementation not required. Hence, always return 'Null'
        }

        /// <summary>
        /// Method for getting structure data received from LD Structure Filter control.
        /// </summary>
        /// <param name="searchType">FilterMode/Search type</param>
        /// <param name="structure">Structure data in String format</param>
        /// <param name="cdxStructure">Structure data in Byte array format</param>
        public void SetFilterSettings(StructureFilterSettings.FilterModeEnum searchType, string structure,
                                      byte[] cdxStructure)
        {
            this.StructureString = structure;
            this._cdxStructure = cdxStructure;
            //if (cdxStructure != null)
            //{
            //    this._cdxStructure = cdxStructure;
            //}
            //else
            //{
            //    this._cdxStructure = Encoding.UTF8.GetBytes(structure);
            //}
            DataTableInfoMgr.SetSearchState(_modelId, DataTableInfoMgr.DataTableInfo.SearchStateEnum.Enabled);
            //DataTableInfoMgr._dataTableInfo[_modelId].SearchState = DataTableInfoMgr.DataTableInfo.SearchStateEnum.Enabled;
        }

        /// <summary>
        /// Method for enabling or disabling RGroupDecomposition checkbox
        /// </summary>
        /// <param name="enabled">RGroupDecomposition checkbox state</param>
        public void SetRGroupDecomposition(bool enabled)
        {
            //Implementation not required. 
        }

        public void SetFilterSettings(SavedSearchSettings settings)
        {
            //throw new NotImplementedException();
        }

        public string GetDataColumnReference()
        {
            return string.Empty;
        }

        public bool HasActiveVisualReference()
        {
            return true;
        }

        public void SetDataColumnReference(Spotfire.Dxp.Data.DataColumn structureColumn)
        {
            //Implementation not required. 
        }

        public void SetDataColumnReference(string structureColumnName)
        {
            //Implementation not required. 
        }

        public System.Collections.Generic.List<string> GetStructureColumnNames()
        {
            System.Collections.Generic.List<string> emptyList = new System.Collections.Generic.List<string>();
            emptyList.Add("");
            return emptyList;
        }

        #endregion

    }
}
