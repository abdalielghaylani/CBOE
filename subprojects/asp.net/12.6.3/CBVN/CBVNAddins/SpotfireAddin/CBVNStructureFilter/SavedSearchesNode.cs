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

using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Spotfire.Dxp.Application.Extension;
using Spotfire.Dxp.Framework.DocumentModel;
using Spotfire.Dxp.Framework.Persistence;

namespace CBVNStructureFilter
{
    [PersistenceVersion(1, 0)]
    [Serializable]
    class SavedSearchesNode: CustomNode
    {
        private readonly UndoableList<ImmutableByteArray> _savedSearchesCollection;

        internal Collection<ImmutableByteArray> SavedSearchesCollection
        {
            get { return _savedSearchesCollection.ToCollection(); }
            set { _savedSearchesCollection.ReplaceAll(value); }
        }

        public SavedSearchesNode()
        {
            CreateProperty(PrivatePropertyNames.SavedSearchesCollection, out _savedSearchesCollection);
        }

        /// <summary>Implements ISerializable.</summary>
        protected SavedSearchesNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            DeserializeProperty(
                info, context, PrivatePropertyNames.SavedSearchesCollection, out _savedSearchesCollection);
        }

        /// <summary>Implements ISerializable.</summary>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            SerializeProperty(info, context, _savedSearchesCollection);
        }

        private abstract class PrivatePropertyNames : DocumentNode.PropertyNames
        {
            #region Constants and Fields

            /// <summary>
            /// The name of the property SavedSearchesCollection.
            /// </summary>
            public static readonly PropertyName SavedSearchesCollection = CreatePropertyName("SavedSearchesCollection");

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
