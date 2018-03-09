using System;
using System.Runtime.Serialization;
using Spotfire.Dxp.Application;
using Spotfire.Dxp.Application.Extension;
using Spotfire.Dxp.Data;
using Spotfire.Dxp.Framework.DocumentModel;
using Spotfire.Dxp.Framework.Persistence;
using System.Collections.Generic;
using SpotfireIntegration.SpotfireAddin.Properties;

namespace SpotfireIntegration.SpotfireAddin
{
    [PersistenceVersion(1, 0)]
    [Serializable]
    class CBVNMarkingNode : CustomNode
    {
        private readonly UndoableCrossReferenceProperty<DataMarkingSelection> marking;
        private readonly UndoableCrossReferenceProperty<DataTable> table;

        #region Classes for property names

        /// <summary>
        /// Contains property name constants for the public properties of <see cref="CBVNMarkingNode"/>.
        /// </summary>
        public new abstract class PropertyNames : CustomNode.PropertyNames
        {
            /// <summary>
            /// The name of the property Marking.
            /// </summary>
            public static readonly PropertyName Marking = CreatePropertyName("Marking");

            /// <summary>
            /// The name of the property Table.
            /// </summary>
            public static readonly PropertyName Table = CreatePropertyName("Table");

        }

        #endregion // Classes for property names

        #region Public properties

        /// <summary>
        /// Gets or sets Marking.
        /// </summary>
        public DataMarkingSelection Marking
        {
            get { return this.marking.Value; }
            set { this.marking.Value = value; }
        }

        /// <summary>
        /// Gets or sets Table.
        /// </summary>
        public DataTable Table
        {
            get { return this.table.Value; }
            set { this.table.Value = value; }
        }


        #endregion // Public properties

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CBVNMarkingNode"/> class./// </summary>
        public CBVNMarkingNode()
        {
            CreateProperty(PropertyNames.Marking, out this.marking, default(DataMarkingSelection));
            CreateProperty(PropertyNames.Table, out this.table, default(DataTable));
        }

        #endregion // Construction

        #region ISerializable Members

        /// <summary>Implements ISerializable.</summary>
        protected CBVNMarkingNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            DeserializeProperty<DataMarkingSelection>(info, context, PropertyNames.Marking, out this.marking);
            DeserializeProperty<DataTable>(info, context, PropertyNames.Table, out this.table);
        }

        /// <summary>Implements ISerializable.</summary>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            SerializeProperty<DataMarkingSelection>(info, context, this.marking);
            SerializeProperty<DataTable>(info, context, this.table);
        }

        #endregion // ISerializable Members

        protected override void OnConfigure()
        {
            base.OnConfigure();
            Reconfigure();
        }

        protected override void OnReconfigure()
        {
            base.OnReconfigure();
            Reconfigure();
        }

        internal void Reconfigure()
        {
            // Get the document that this node is part of.
            Document document = this.Context.GetAncestor<Document>();

            // Get the marking used in the currently active plot, may be null.
            DataMarkingSelection marking = document.ActiveMarkingSelectionReference;

            if (marking == null)
            {
                // Use the default marking if no plot was selected.
                marking = document.Data.Markings.DefaultMarkingReference;
            }

            // Select the chosen marking.
            this.Marking = marking;

            // Now we need to choose a table to use and we require that certain columns
            // exist in the table.
            foreach (DataTable dataTable in document.Data.Tables)
            {
                if (dataTable.Properties.HasPropertyValue(Resources.COEDataViewID_PropertyName) &&
                    dataTable.Properties.HasPropertyValue(Resources.COEHitListID_PropertyName))
                {
                    this.Table = dataTable;
                    break;
                }
            }
        }

        protected override void DeclareInternalEventHandlers(InternalEventManager eventManager)
        {
            base.DeclareInternalEventHandlers(eventManager);

            eventManager.AddEventHandler(
                MarkingChanged,
                Trigger.CreateMutablePropertyTrigger<DataMarkingSelection>(
                    this,
                    CBVNMarkingNode.PropertyNames.Marking,
                    DataMarkingSelection.PropertyNames.Selection));
        }

        private void MarkingChanged(DocumentNode node, PropertyName propertyName)
        {
            if (this.Marking == null || this.Table == null)
            {
                return;
            }

            // Get the currently marked rows.
            IndexSet markedRows = this.Marking.GetSelection(this.Table).AsIndexSet();

            if (markedRows.Count == 0)
            {
                // No rows marked, return.
                return;
            }

            // Get the data view ID and hitlist ID.
            object dataViewID = this.Table.Properties.GetProperty(Resources.COEDataViewID_PropertyName);
            // PROBLEM: these properties are not updated or erased by a close event, so they get stale
            if (dataViewID == null || !(dataViewID is int))
            {
                return;
            }

            object hitListID = this.Table.Properties.GetProperty(Resources.COEHitListID_PropertyName);
            if (hitListID == null || !(hitListID is int))
            {
                return;
            }

            // Select the row in CBVN.
            List<int> rows = new List<int>(markedRows);
            CBVNController.GetInstance().SelectRows((int) dataViewID, (int) hitListID, rows);
        }
    }
}
