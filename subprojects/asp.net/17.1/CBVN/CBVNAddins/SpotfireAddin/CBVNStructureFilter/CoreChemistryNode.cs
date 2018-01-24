// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureFilterPanelControlBase.cs" company="PerkinElmer Inc.">
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
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    using Spotfire.Dxp.Framework.DocumentModel;
    using Spotfire.Dxp.Framework.Persistence;
    using Spotfire.Dxp.Application.Extension;

    [PersistenceVersion(1, 0)]
    [Serializable]
    internal class CoreChemistryNode : CustomNode
    {
        private readonly UndoableList<ImmutableByteArray> _coreChemistryCollection;

        internal Collection<ImmutableByteArray> CoreChemistryCollection
        {
            get { return _coreChemistryCollection.ToCollection(); }
            set { _coreChemistryCollection.ReplaceAll(value); }
        }

        public CoreChemistryNode()
        {
            CreateProperty(PrivatePropertyNames.CoreChemistryCollection, out _coreChemistryCollection);
        }

        /// <summary>Implements ISerializable.</summary>
        protected CoreChemistryNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            DeserializeProperty(
                info, context, PrivatePropertyNames.CoreChemistryCollection, out _coreChemistryCollection);
        }

        /// <summary>Implements ISerializable.</summary>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            SerializeProperty(info, context, _coreChemistryCollection);
        }

        internal void ClearCollection()
        {
            _coreChemistryCollection.Clear();
        }

        private abstract class PrivatePropertyNames : DocumentNode.PropertyNames
        {
            #region Constants and Fields

            /// <summary>
            /// The name of the property CoreChemistryCollection.
            /// </summary>
            public static readonly PropertyName CoreChemistryCollection = CreatePropertyName("CoreChemistryCollection");

            #endregion
        }

        /// <summary>
        /// Helper class required to satisfy Spotfire property rules.
        /// </summary>
        [Immutable, Serializable]
        [PersistenceVersion(1, 0)]
        public class ImmutableByteArray
        {
            /// <summary>
            /// 
            /// </summary>
            public readonly byte[] Data;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="d"></param>
            public ImmutableByteArray(byte[] d)
            {
                Data = d;
            }
        }
    }
}
