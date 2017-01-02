// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureFilterPresenter.cs" company="PerkinElmer Inc.">
// Copyright © 2013 PerkinElmer Inc. 
// 940 Winter Street, Waltham, MA 02451.
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
    using Spotfire.Dxp.Framework.ApplicationModel;
    using Spotfire.Dxp.Framework.DocumentModel;
    
    /// <summary>
    /// 
    /// </summary>
    public class StructureFilterPresenter
    {
        private readonly IStructureFilterModel _filterModel;
        private readonly IStructureSearchModel _searchModel;
        private readonly IStructureEditModel _editModel;
        private readonly IStructureFilterView _filterView;

        internal IStructureFilterModel FilterModel
        {
            get { return _filterModel; }
        }
        internal IStructureSearchModel SearchModel
        {
            get { return _searchModel; }
        }

        internal string HydrogenDisplayMode
        {
            get
            {
                // Default to no hydrogen display if filterModel is not yet available
                return _editModel == null ? "Off" : _editModel.ShowHydrogens ?? string.Empty;
            }
            set { _editModel.ShowHydrogens = value; }
        }

        internal INodeContext NodeContext
        {
            get { return _editModel == null ? null : _editModel.NodeContext; }
        }

        internal StructureFilterSettings.FilterModeEnum SearchType
        {
            get
            {
                return _searchModel == null
                           ? StructureFilterSettings.FilterModeEnum.SubStructure
                           : _searchModel.SearchType;
            }
            set { _searchModel.SearchType = value; }
        }

        internal byte[] CdxStructure
        {
            get { return _editModel.CdxStructure; }
        }

        internal string StructureString
        {
            get { return _editModel.StructureString; }
        }

        internal Guid ModelId
        {
            get { return _editModel == null ? Guid.Empty : _editModel.ModelId; }
        }

        internal int PercentSimilarity
        {
            set { _searchModel.PercentSimilarity = value; }
        }

        internal bool NickNames
        {
            set { _filterModel.NickNames = value; }
        }

        internal string RGroupStructure
        {
            get { return _filterModel == null ? string.Empty : _filterModel.RGroupStructure; }
            set { _filterModel.RGroupStructure = value; }
        }

        internal StructureStringType RGroupStructureType
        {
            get { return _filterModel.RGroupStructureType; }
        }

        internal StructureFilterPresenter(IStructureEditModel editModel, IStructureSearchModel searchModel,
                                          IStructureFilterModel filterModel, IStructureFilterView filterView)
        {
            if (editModel == null)
            {
                throw new ArgumentNullException("editModel");
            }
            if (filterView == null)
            {
                throw new ArgumentNullException("filterView");
            }
            _filterModel = filterModel;
            _searchModel = searchModel;
            _editModel = editModel;
            _filterView = filterView;
        }

        internal void ClearFilter()
        {
            _filterModel.ClearCurrentFilter();

            _filterModel.RemoveRGroupDecompositionColumnsFromDocument();
            //var coreChemistryClr = DataTableInfoMgr.GetCoreChemistry(_editModel.ModelId);
            //if (coreChemistryClr != null)
            //{
            //    coreChemistryClr.ClearFilterSettings();
            //}

            _filterView.RGroupDecomposition = false;
            _filterView.PercentLabel = _filterView.SimPercent;

            _filterView.SetImage();
        }

        internal bool HasStructureString()
        {
            return _editModel.HasStructureString();
        }

        internal void ShowStructureSourceConfigDialog()
        {
            _searchModel.ShowStructureSourceConfigDialog();
        }

        internal string GetDataColumnReference()
        {
            return _searchModel.GetDataColumnReference();
        }

        internal void ExecuteTransaction(Executor executor)
        {
            _searchModel.ExecuteTransaction(executor);
        }

        internal ModulesService GetModulesService()
        {
            return _searchModel.GetModulesService();
        }

        internal void SetFilterSettings(StructureFilterSettings.FilterModeEnum searchType, string structure, byte[] cdxStructure)
        {
            if (_searchModel != null)
            {
                _searchModel.SetFilterSettings(searchType, structure, cdxStructure);
            }
            else
            {
                _editModel.CdxStructure = cdxStructure;
                _editModel.StructureString = structure;
            }
        }

        internal void SetFilterSettings(SavedSearchSettings settings)
        {
            if (_searchModel != null)
            {
                _searchModel.SetFilterSettings(settings);
            }
        }

        internal string ConvertCdxToMol(string cdx)
        {
            return FilterUtilities.ConvertCdxToMol(cdx);
        }

        internal void SetRGroupDecomposition(bool enabled, bool nicknames)
        {
            _filterModel.SetRGroupDecomposition(enabled, nicknames);
        }

        internal void RemoveRGroupDecompositionColumnsFromDocument()
        {
            _filterModel.RemoveRGroupDecompositionColumnsFromDocument();
        }

        internal void SetStructureAlignment(bool enabled)
        {
            _filterModel.SetStructureAlignment(enabled);
        }        
    }
}
