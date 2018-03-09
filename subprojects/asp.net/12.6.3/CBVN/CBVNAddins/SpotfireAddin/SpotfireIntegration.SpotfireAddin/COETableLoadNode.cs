using System;
using System.Runtime.Serialization;
using Spotfire.Dxp.Application;
using Spotfire.Dxp.Application.Extension;
using Spotfire.Dxp.Data;
using Spotfire.Dxp.Framework.DocumentModel;
using Spotfire.Dxp.Framework.Persistence;

namespace SpotfireIntegration.SpotfireAddin
{
    [PersistenceVersion(1, 0)]
    [Serializable]
    class COETableLoadNode : CustomNode
    {
        //private readonly UndoableCrossReferenceProperty<DataTable> coeBaseTable;

        #region Classes for property names

        public new abstract class PropertyNames : CustomNode.PropertyNames
        {
            //public static readonly PropertyName CoeBaseTable = CreatePropertyName("CoeBaseTable");
        }

        #endregion

        #region Public properties

        /*
        public DataTable CoeBaseTable
        {
            get { return this.coeBaseTable.Value; }
            set { this.coeBaseTable.Value = value; }
        }
        */

        #endregion

        #region Construction

        public COETableLoadNode()
        {
            //CreateProperty<DataTable>(PropertyNames.CoeBaseTable, out this.coeBaseTable, default(DataTable));
        }

        #endregion

        #region ISerializable Members

        /// <summary>Implements ISerializable.</summary>
        protected COETableLoadNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //DeserializeProperty<DataTable>(info, context, PropertyNames.CoeBaseTable, out this.coeBaseTable);
        }

        /// <summary>Implements ISerializable.</summary>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            //SerializeProperty<DataTable>(info, context, this.coeBasetable);
        }

        #endregion // ISerializable Members

        protected override void OnConfigure()
        {
            base.OnConfigure();
        }

        protected override void DeclareInternalEventHandlers(InternalEventManager eventManager)
        {
            base.DeclareInternalEventHandlers(eventManager);

            DataManager dataManager = this.Context.GetAncestor<Document>().Data;
            eventManager.AddEventHandler(TableLoaded, Trigger.CreatePropertyTrigger(dataManager.Tables, DataTableCollection.PropertyNames.Items));
        }

        private void TableLoaded(DocumentNode node, PropertyName propertyName)
        {
            AnalysisApplication application = node.Context.GetService<AnalysisApplication>();
            DataTable baseDataTable = application.Document.Data.Tables.GetCOEBaseTable();
            if (baseDataTable != null)
            {
                node.Transactions.ExecuteTransaction(delegate
                {
                    application.MergeCOEChildTables(baseDataTable);
                });
            }
        }
    }
}
