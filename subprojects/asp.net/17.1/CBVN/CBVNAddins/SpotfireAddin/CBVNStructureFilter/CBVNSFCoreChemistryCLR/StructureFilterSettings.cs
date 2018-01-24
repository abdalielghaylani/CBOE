// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureFilterModel.cs" company="PerkinElmer Inc.">
// Copyright © 2013 PerkinElmer Inc. 
// 940 Winter Street, Waltham, MA 02451.// All rights reserved. 
// This software is the confidential and proprietary information 
// of PerkinElmer Inc. ("Confidential Information"). You shall not 
// disclose such Confidential Information and may not use it in any way, 
// absent an express written license agreement between you and PerkinElmer Inc. 
// that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Linq;
using Spotfire.Dxp.Framework.Persistence;

[assembly: CLSCompliantAttribute(true)]
namespace CBVNStructureFilter
{
    using Spotfire.Dxp.Framework.DocumentModel;

    /// <summary>
    /// Spotfire requires all Runtime properties to be immutable
    /// </summary>
    [Immutable, Serializable]
    [PersistenceVersion(9, 0)]
    public class ImmutableStringArray
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly string[] Data;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        public ImmutableStringArray(string[] d)
        {
            Data = d;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [PersistenceVersion(9, 1)]
    public class StructureFilterSettings
    {
        #region Enumerations

        /// <summary>
        /// Filter Modes
        /// </summary>
        [PersistenceVersion(9, 0)]
        [System.ComponentModel.DefaultValue(StructureFilterSettings.FilterModeEnum.SubStructure)]
        public enum FilterModeEnum
        {
            /// <summary>
            /// 
            /// </summary>
            SubStructure, 
            /// <summary>
            /// 
            /// </summary>
            FullStructure,
            /// <summary>
            /// 
            /// </summary>
            Simularity
        }
        /// <summary>
        /// SimularityMode
        /// </summary>
        [PersistenceVersion(9, 0)]
        public enum SimularityModeEnum
        {
            /// <summary>
            /// 
            /// </summary>
            GreaterThan,
            /// <summary>
            /// 
            /// </summary>
            LessThan
        }

        #endregion

        #region Constants and Fields

        private readonly string _chemicalDocumentField;
        private readonly FilterModeEnum _filterMode;
        private readonly bool _includeHits;
        private readonly SimularityModeEnum _simularityMode;
        private readonly int _simularityPercent;
        private readonly bool _rGroupDecomposition;
        private readonly string _filterStructure;
        private readonly bool _rGroupNicknames;

        #endregion

        #region Properties

        /// <summary>Gets/sets the chemical Document Field.
        /// </summary>
        /// <value>The chemical Document Field mode.</value>
        public string ChemicalDocumentField
        {
            get
            {
                return _chemicalDocumentField;
            }
        }

        /// <summary>Gets/sets the filter mode. The allowable filter modes are stored in the FilterModeEnum.
        /// </summary>
        /// <value>The filter mode.</value>
        public FilterModeEnum FilterMode
        {
            get
            {
                return _filterMode;
            }
        }
        /// <summary>Gets or sets the boolean indicating of we are marking hits or misses.
        /// </summary>
        /// <value>true to mark structures that match, false to mark structures that do not match.</value>
        public bool IncludeHits
        {
            get
            {
                return _includeHits;
            }
        }
        /// <summary>Gets or sets the mode for a simularity filter, stored in the SimularityModeEnum.
        /// </summary>
        /// <value>The Simularity Mode.</value>
        public SimularityModeEnum SimularityMode
        {
            get
            {
                return _simularityMode;
            }
        }
        /// <summary>Gets or sets the percentage of a strucure that must match to pass a simularity test.
        /// </summary>
        /// <value>The percentage.</value>
        public int SimularityPercent
        {
            get
            {
                return _simularityPercent;
            }
        }
        /// <summary>A boolean value indicating if we are doing an RGroupDecomposition. 
        /// NOTE: This feature is not implemented as of yet so setting this value does nothing.
        /// </summary>
        /// <value>true to perform an RGroupDecomposition, false for a match.</value>
        public bool RGroupDecomposition
        {
            get
            {
                return _rGroupDecomposition;
            }
        }
        /// <summary>A boolean value indicating if we are doing an RGroupDecomposition. 
        /// NOTE: This feature is not implemented as of yet so setting this value does nothing.
        /// </summary>
        /// <value>true to perform an RGroupDecomposition, false for a match.</value>
        public string FilterStructure
        {
            get
            {
                return _filterStructure;
            }
        }

        /// <summary>
        /// A boolean value indicating if nick names are enabled for R-Group decomposition
        /// </summary>
        /// <value>true if nick names are enabled, false if disabled</value>
        public bool RGroupNicknames
        {
            get { return _rGroupNicknames; }
        }

        /// <summary>
        /// 
        /// </summary>
        public StructureFilterSettings()
        {
            _chemicalDocumentField = string.Empty;
            _filterMode = FilterModeEnum.SubStructure;
            _includeHits = false;
            _simularityMode = SimularityModeEnum.GreaterThan;
            _simularityPercent = 90;
            _rGroupDecomposition = false;
            _filterStructure = string.Empty;
            _rGroupNicknames = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chemicalDocument"></param>
        /// <param name="filMode"></param>
        /// <param name="incHits"></param>
        /// <param name="simMode"></param>
        /// <param name="simPercent"></param>
        /// <param name="rGroupDecomp"></param>
        /// <param name="filterStruct"></param>
        /// <param name="rGroupNicknames"></param>
        public StructureFilterSettings(string chemicalDocument, FilterModeEnum filMode,
            bool incHits, SimularityModeEnum simMode, int simPercent, bool rGroupDecomp,
            string filterStruct, bool rGroupNicknames)
        {
            _chemicalDocumentField = chemicalDocument;
            _filterMode = filMode;
            _includeHits = incHits;
            _simularityMode = simMode;
            _simularityPercent = simPercent;
            _rGroupDecomposition = rGroupDecomp;
            _filterStructure = filterStruct;
            _rGroupNicknames = rGroupNicknames;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="copy"></param>
        public StructureFilterSettings(StructureFilterSettings copy)
        {
            _chemicalDocumentField = copy._chemicalDocumentField;
            _filterMode = copy._filterMode;
            _includeHits = copy._includeHits;
            _simularityMode = copy._simularityMode;
            _simularityPercent = copy._simularityPercent;
            _rGroupDecomposition = copy._rGroupDecomposition;
            _filterStructure = copy._filterStructure;
            _rGroupNicknames = copy._rGroupNicknames;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public bool IsChanged(StructureFilterSettings compare)
        {
            if ((ChemicalDocumentField != compare.ChemicalDocumentField) ||
                (FilterMode != compare.FilterMode) ||
                (FilterStructure != compare.FilterStructure) ||
                (IncludeHits != compare.IncludeHits) ||
                (RGroupDecomposition != compare.RGroupDecomposition) ||
                (SimularityMode != compare.SimularityMode) ||
                (SimularityPercent != compare.SimularityPercent) ||
                (RGroupNicknames != compare.RGroupNicknames))
            {
                return true;
            }
            return false;
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [PersistenceVersion(1, 0)]
    public class SavedSearchSettings : StructureFilterSettings, IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] CDXStructureData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RGDStructure { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] RGDCDXStructureData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SavedSearchSettings()
        {
            Name = string.Empty;
            CDXStructureData = null;
            RGDStructure = string.Empty;
            RGDCDXStructureData = null;
            StructureColumn = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        public string StructureColumn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filMode"></param>
        /// <param name="simPercent"></param>
        /// <param name="rGroupDecomp"></param>
        /// <param name="filterStruct"></param>
        /// <param name="rGroupNicknames"></param>
        public SavedSearchSettings(FilterModeEnum filMode, int simPercent, bool rGroupDecomp, string filterStruct, bool rGroupNicknames)
            : base(string.Empty, filMode, false, SimularityModeEnum.GreaterThan, simPercent, rGroupDecomp, filterStruct, rGroupNicknames)
        {
            Name = string.Empty;
            CDXStructureData = null;
            RGDStructure = string.Empty;
            RGDCDXStructureData = null;
            StructureColumn = string.Empty;
        }

        public int CompareTo(object obj)
        {
            var other = (SavedSearchSettings)obj;

            if (IsChanged((StructureFilterSettings)obj))
            {
                return -1;
            }

            if (RGDStructure != null && other.RGDStructure != null)
            {
                if (RGDStructure != other.RGDStructure)
                {
                    return -1;
                }
            }
            if (StructureColumn != other.StructureColumn)
            {
                return -1;
            }
            if (!ByteArraysAreEqual(CDXStructureData, other.CDXStructureData))
            {
                return -1;
            }
            if (!ByteArraysAreEqual(RGDCDXStructureData, other.RGDCDXStructureData))
            {
                return -1;
            }
            return 0;
        }

        private static bool ByteArraysAreEqual(byte[] array1, byte[] array2)
        {
            if ((array1 == null && array2 != null) || (array1 != null && array2 == null))
            {
                return false;
            }

            if (array1 == null)
            {
                return true;
            }

            if (array1.Length != array2.Length)
            {
                return false;
            }

            return !array1.Where((t, i) => t != array2[i]).Any();
        }
    }
}
