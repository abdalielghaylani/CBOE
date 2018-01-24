// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureFilterModelInterface.cs" company="PerkinElmer Inc.">
//   Copyright © 2013 PerkinElmer Inc. 

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
    using Spotfire.Dxp.Framework.DocumentModel;
    using Spotfire.Dxp.Framework.ApplicationModel;

    /// <summary>
    /// Properties and methods needed just for editting structures
    /// </summary>
    public interface IStructureEditModel
    {
        /// <summary>
        /// 
        /// </summary>
        Guid ModelId { get; }
        /// <summary>
        /// 
        /// </summary>
        string ShowHydrogens { get; set; }
        /// <summary>
        /// 
        /// </summary>
        byte[] CdxStructure { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string StructureString { get; set; }
        /// <summary>
        /// 
        /// </summary>
        INodeContext NodeContext { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool HasStructureString();
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IStructureSearchModel
    {
        /// <summary>
        /// 
        /// </summary>
        int PercentSimilarity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        StructureFilterSettings.FilterModeEnum SearchType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool ValidDataTable();
        /// <summary>
        /// 
        /// </summary>
        void ShowStructureSourceConfigDialog();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetDataColumnReference();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="executor"></param>
        void ExecuteTransaction(Executor executor);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ModulesService GetModulesService();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchType"></param>
        /// <param name="structure"></param>
        /// <param name="cdxStructure"></param>
        void SetFilterSettings(StructureFilterSettings.FilterModeEnum searchType, string structure, byte[] cdxStructure);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        void SetFilterSettings(SavedSearchSettings settings);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="nickNames"></param>
        void SetRGroupDecomposition(bool enabled, bool nickNames);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IStructureFilterModel
    {
        /// <summary>
        /// 
        /// </summary>
        bool RGroupDecomposition { get; }
        /// <summary>
        /// 
        /// </summary>
        string RGroupStructure { get; set; }
        /// <summary>
        /// 
        /// </summary>
        StructureStringType RGroupStructureType { get; }
        /// <summary>
        /// 
        /// </summary>
        bool NickNames { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool UpdateSavedSearchList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        void ClearCurrentFilter();
        /// <summary>
        /// 
        /// </summary>
        void RemoveRGroupDecompositionColumnsFromDocument();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="nickNames"></param>
        void SetRGroupDecomposition(bool enabled, bool nickNames);
        /// <summary>
        /// <param name="enabled"></param>
        /// </summary>
        void SetStructureAlignment(bool enabled);
    }
}
