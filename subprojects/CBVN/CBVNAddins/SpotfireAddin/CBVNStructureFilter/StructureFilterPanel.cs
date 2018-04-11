// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureFilterPanel.cs" company="PerkinElmer Inc.">
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
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using Spotfire.Dxp.Data;
    using Spotfire.Dxp.Application.Extension;
    using Spotfire.Dxp.Framework.DocumentModel;
    using Spotfire.Dxp.Framework.Persistence;
    //using CBVNSFCoreChemistryCLR;

    /// <summary>
    /// Implementation of a simple custom panel.
    /// </summary>
    [Serializable]
    [PersistenceVersion(1, 0)]
    internal class StructureFilterPanel : CustomPanel
    {
        #region Constants and Fields

        private readonly StructureFilterModel _structureFilterModel;

        #endregion

        #region Constructors and Destructors

        internal StructureFilterPanel()
        {
            CreateReadOnlyProperty(
                PrivatePropertyNames.StructureFilterModel, new StructureFilterModel(), out _structureFilterModel);
        }

        /// <summary>Implements ISerializable.</summary>
        protected StructureFilterPanel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            DeserializeReadOnlyProperty(
                info, context, PrivatePropertyNames.StructureFilterModel, out _structureFilterModel);
        }

        #endregion

        #region Properties

        internal StructureFilterModel StructureFilterModel
        {
            get
            {
                return _structureFilterModel;
            }
        }

        internal DataColumn DataColumnReference
        {
            get
            {
                return StructureFilterModel.DataColumnReference;
            }
        }

        internal void SetFilterSettings(StructureFilterSettings.FilterModeEnum searchType, string structure, bool doRGroup, bool nickNames)
        {
            _structureFilterModel.SetFilterSettings(searchType, structure);
            _structureFilterModel.SetRGroupDecomposition(doRGroup, nickNames);
        }

        internal void ApplyFilter(DataTable dataTable)
        {
            _structureFilterModel.ApplyFilter(dataTable);
        }

        internal void SetDataColumn()
        {
            _structureFilterModel.SetDataColumn();
        }

        #endregion

        #region Methods

        /// <summary>Implements ISerializable.</summary>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            SerializeReadOnlyProperty(
                info, context, PrivatePropertyNames.StructureFilterModel, _structureFilterModel);
        }

        #endregion

        private abstract class PrivatePropertyNames : DocumentNode.PropertyNames
        {
            #region Constants and Fields

            internal static readonly PropertyName StructureFilterModel = CreatePropertyName("StructureFilterModel");

            #endregion
        }
    }
}
