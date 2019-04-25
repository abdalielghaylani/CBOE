using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using PerkinElmer.COE.Inventory.Model;
using PerkinElmer.COE.Inventory.DAL.Mapper;
using Oracle.ManagedDataAccess.Client;

namespace PerkinElmer.COE.Inventory.DAL
{
    public class ContainerDAL : BaseDAL
    {
        MapperBase<INV_CONTAINERS, ContainerData> containerMapper = new ContainerMapper();
        MapperBase<INV_CUSTOM_CPD_FIELD_VALUES, CustomFieldData> customFieldMapper = new CustomFieldMapper();

        public ContainerDAL()
        {
        }

        public ContainerDAL(IInventoryDBContext context) : base(context)
        {
        }

        public ContainerData GetContainerById(int containerId)
        {
            var containerData = containerMapper.Map(db.INV_CONTAINERS
                .Include(c => c.INV_CONTAINER_TYPES)
                .Include(c => c.INV_CONTAINER_STATUS)
                .Include(c => c.INV_SUPPLIERS)
                .Include(c => c.INV_UNITS)
                .Include(c => c.INV_LOCATIONS)
                .Include(c => c.INV_LOCATIONS1)
                .Include(c => c.INV_COMPOUNDS)
                .Include(c => c.INV_LOCATION_TYPES)
                .SingleOrDefault(c => c.CONTAINER_ID == containerId));

            if (containerData != null && containerData.Compound != null)
            {
                containerData.Compound.SafetyData = customFieldMapper.Map(db.INV_CUSTOM_CPD_FIELD_VALUES
                    .Include(c => c.INV_CUSTOM_FIELDS)
                    .Include("INV_CUSTOM_FIELDS.INV_CUSTOM_FIELD_GROUPS")
                    .Where(c => c.COMPOUND_ID_FK == containerData.Compound.CompoundId)
                    .ToList());
            }

            return containerData;
        }

        public ContainerData GetContainerByBarcode(string barcode)
        {
            var containerData = containerMapper.Map(db.INV_CONTAINERS
                .Include(c => c.INV_CONTAINER_TYPES)
                .Include(c => c.INV_CONTAINER_STATUS)
                .Include(c => c.INV_SUPPLIERS)
                .Include(c => c.INV_UNITS)
                .Include(c => c.INV_LOCATIONS)
                .Include(c => c.INV_LOCATIONS1)
                .Include(c => c.INV_COMPOUNDS)
                .Include(c => c.INV_LOCATION_TYPES)
                .SingleOrDefault(c => c.BARCODE.ToUpper() == barcode.ToUpper()));

            if (containerData != null && containerData.Compound != null)
            {
                containerData.Compound.SafetyData = customFieldMapper.Map(db.INV_CUSTOM_CPD_FIELD_VALUES
                    .Include(c => c.INV_CUSTOM_FIELDS)
                    .Include("INV_CUSTOM_FIELDS.INV_CUSTOM_FIELD_GROUPS")
                    .Where(c => c.COMPOUND_ID_FK == containerData.Compound.CompoundId)
                    .ToList());
            }

            return containerData;
        }

        public int CreateContainer(ContainerData container)
        {
            ValidateContainer(container);            

            var dbContext = ((DbContext)db);
            using (var dbContextTransaction = dbContext.Database.BeginTransaction())
            {
                var doRollback = true;
                try
                {                    
                    Oracle.ManagedDataAccess.Client.OracleConnection connection = (Oracle.ManagedDataAccess.Client.OracleConnection)dbContext.Database.Connection;
                    Oracle.ManagedDataAccess.Client.OracleCommand cmd = dbContext.Database.Connection.CreateCommand() as Oracle.ManagedDataAccess.Client.OracleCommand;
                    cmd.CommandText = "CHEMINVDB2.CreateContainer";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(containerMapper.GetOracleParameters(container));
                    cmd.ExecuteNonQuery();
                    dbContextTransaction.Commit();
                    
                    var returnValue = int.Parse(cmd.Parameters["RETURN_VALUE"].Value.ToString());
                    doRollback = false;

                    if (returnValue.Equals(-123))
                    {
                        throw new Exception("The creation of the container failed with code -123.");
                    }
                    else if (returnValue.Equals(-102))
                    {
                        throw new Exception("A container with same barcode already exists.");
                    }
                    else if (returnValue.Equals(-103))
                    {
                        throw new Exception("Amount cannot exceed container size.");
                    }
                    else if (returnValue.Equals(-128))
                    {
                        throw new Exception("The container type is not allowed.");
                    }

                    container.ContainerId = returnValue;
                }
                catch (Exception ex)
                {
                    if(doRollback)
                        dbContextTransaction.Rollback();

                    throw new Exception("The creation of the container failed.", ex);
                }
            }

            return container.ContainerId;
        }

        private void ValidateContainer(ContainerData container)
        {
            if (container.Location == null)
            {
                throw new Exception("LocationID is a required parameter");
            }

            if (container.QuantityMax == null || container.QuantityMax.Value == null)
            {
                throw new Exception("QuantityMax is a required parameter");
            }

            if (container.QuantityInitial == null || container.QuantityInitial.Value == null)
            {
                throw new Exception("QuantityInitial is a required parameter");
            }

            if (container.ContainerType == null)
            {
                throw new Exception("ContainerTypeID is a required parameter");
            }

            if (container.Status == null)
            {
                throw new Exception("ContainerStatusID is a required parameter");
            }

            if (string.IsNullOrEmpty(container.CurrentUser))
            {
                throw new Exception("CurrentUser is a required parameter");
            }
        }
    }
}
