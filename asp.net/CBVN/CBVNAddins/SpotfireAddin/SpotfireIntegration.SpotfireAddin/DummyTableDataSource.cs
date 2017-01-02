using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotfireIntegration.SpotfireAddin
{
    using System;
    using System.Data;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Spotfire.Dxp.Application;
    using Spotfire.Dxp.Application.Extension;
    using Spotfire.Dxp.Data;
    using Spotfire.Dxp.Data.Exceptions;
    using Spotfire.Dxp.Framework.ApplicationModel;
    using Spotfire.Dxp.Framework.Persistence;
    using SpotfireIntegration.Common;

    [Serializable]
    [PersistenceVersion(1, 0)]
    class DummyTableDataSource: CustomDataSource
    {
        #region Fields

        
        #endregion

        #region Constructors

        public DummyTableDataSource()
            : base()
        {
           
        }

        /// <summary>Initializes a new instance of the <see cref="DummyTableDataSource"/> class.
        /// Deserialization constructor, this is called when a stored data source is loaded and
        /// also when a data source is cloned.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The serialization context.</param>
        private DummyTableDataSource(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Empty
        }


        #endregion

        #region Properties

        public override bool IsLinkable
        {
            get { return false; }
        }

        public override string Name
        {
            get { return "dummy"; }
        }

        

        #endregion

        #region Public Methods

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion

        #region Methods

        protected override DataSourceConnection ConnectCore(IServiceProvider serviceProvider, DataSourcePromptMode promptMode)
        {
            // We do not need to create a specific connection or prompt so we can just use the utility method
            // on the DataSourceConnection class. The CreateReader method that we send as an argument
            // will create the actual reader.
            return DataSourceConnection.CreateConnection2(this, this.CreateReader, serviceProvider);
        }

        /// <summary>Create a reader for the dummy table.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>A reader for the dummy table</returns>
        private DataRowReader CreateReader(IServiceProvider serviceProvider)
        {
            // Create a ADO.NET data set.
            System.Data.DataTable tbl = new System.Data.DataTable("dummy");
            System.Data.DataColumn col = new System.Data.DataColumn();
            tbl.Columns.Add(col);
            tbl.NewRow();
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(tbl);
            
            // Here we can use a utility method on DataRowReader which automatically creates
            // a DataRowReader from an ADO.NET data reader.
            return DataRowReader.CreateReader(dataSet.CreateDataReader());
        }

        #endregion
    }
}
