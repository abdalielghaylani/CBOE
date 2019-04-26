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
                    if (doRollback)
                        dbContextTransaction.Rollback();

                    throw new Exception("The creation of the container failed.", ex);
                }
            }

            return container.ContainerId;
        }

        public void UpdateContainer(int containerId, List<ContainerUpdatedData> containerUpdatedData)
        {
            var dbContext = ((DbContext)db);
            using (var dbContextTransaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Oracle.ManagedDataAccess.Client.OracleConnection connection = (Oracle.ManagedDataAccess.Client.OracleConnection)dbContext.Database.Connection;
                    Oracle.ManagedDataAccess.Client.OracleCommand cmd = dbContext.Database.Connection.CreateCommand() as Oracle.ManagedDataAccess.Client.OracleCommand;
                    cmd.CommandText = "CHEMINVDB2.UpdateContainer";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Int32, 0, null, System.Data.ParameterDirection.ReturnValue));
                    cmd.Parameters.Add(new OracleParameter("PCONTAINERIDS", OracleDbType.Varchar2, 4000, containerId.ToString(), System.Data.ParameterDirection.Input));
                    var valuePairs = GetKeyValuePairsParameters(containerUpdatedData);
                    cmd.Parameters.Add(new OracleParameter("PVALUEPAIRS", OracleDbType.Varchar2, 10000, valuePairs, System.Data.ParameterDirection.Input));
                    cmd.ExecuteNonQuery();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception("The creation of the container failed.", ex);
                }
            }
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

        private string GetKeyValuePairsParameters(List<ContainerUpdatedData> containerUpdatedData)
        {
            var result = new List<string>();

            foreach (var property in containerUpdatedData)
            {
                var propertyEnumValue = EnumHelper.ParseEnum<ContainerPropertyEnum>(property.PropertyName);
                switch (propertyEnumValue)
                {
                    case ContainerPropertyEnum.LocationId:
                        result.Add(string.Format("{0}='{1}'", "LOCATION_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.CompoundId:
                        result.Add(string.Format("{0}='{1}'", "COMPOUND_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.ParentContainerId:
                        result.Add(string.Format("{0}='{1}'", "PARENT_CONTAINER_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.RegId:
                        result.Add(string.Format("{0}='{1}'", "REG_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.BatchNumber:
                        result.Add(string.Format("{0}='{1}'", "BATCH_NUMBER_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Family:
                        result.Add(string.Format("{0}='{1}'", "FAMILY", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.BatchId:
                        result.Add(string.Format("{0}='{1}'", "BATCH_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.ContainerName:
                        result.Add(string.Format("{0}='{1}'", "CONTAINER_NAME", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.ContainerDescription:
                        result.Add(string.Format("{0}='{1}'", "CONTAINER_DESCRIPTION", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.QuantityMax:
                        result.Add(string.Format("{0}='{1}'", "QTY_MAX", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.QuantityInitial:
                        result.Add(string.Format("{0}='{1}'", "QTY_INITIAL", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.QuantityRemaining:
                        result.Add(string.Format("{0}='{1}'", "QTY_REMAINING", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.QuantityMinStock:
                        result.Add(string.Format("{0}='{1}'", "QTY_MINSTOCK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.QuantityMaxStock:
                        result.Add(string.Format("{0}='{1}'", "QTY_MAXSTOCK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.WellNumber:
                        result.Add(string.Format("{0}='{1}'", "WELL_NUMBER", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.WellRow:
                        result.Add(string.Format("{0}='{1}'", "WELL_ROW", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.WellColumn:
                        result.Add(string.Format("{0}='{1}'", "WELL_COLUMN", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.DateExpires:
                        result.Add(string.Format("{0}='{1}'", "DATE_EXPIRES", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.DateCreated:
                        result.Add(string.Format("{0}='{1}'", "DATE_CREATED", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.ContainerTypeId:
                        result.Add(string.Format("{0}='{1}'", "CONTAINER_TYPE_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Purity:
                        result.Add(string.Format("{0}='{1}'", "PURITY", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.PurityId:
                        result.Add(string.Format("{0}='{1}'", "UNIT_OF_PURITY_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.SolventId:
                        result.Add(string.Format("{0}='{1}'", "SOLVENT_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Concentration:
                        result.Add(string.Format("{0}='{1}'", "CONCENTRATION", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.ConcentrationId:
                        result.Add(string.Format("{0}='{1}'", "UNIT_OF_CONC_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.QuantityMaxId:
                        result.Add(string.Format("{0}='{1}'", "UNIT_OF_MEAS_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Grade:
                        result.Add(string.Format("{0}='{1}'", "GRADE", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Weight:
                        result.Add(string.Format("{0}='{1}'", "WEIGHT", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.WeightId:
                        result.Add(string.Format("{0}='{1}'", "UNIT_OF_WGHT_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.TareWeight:
                        result.Add(string.Format("{0}='{1}'", "TARE_WEIGHT", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.OwnerId:
                        result.Add(string.Format("{0}='{1}'", "OWNER_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.ContainerComments:
                        result.Add(string.Format("{0}='{1}'", "CONTAINER_COMMENTS", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.StorageConditions:
                        result.Add(string.Format("{0}='{1}'", "STORAGE_CONDITIONS", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.HandlingProcedures:
                        result.Add(string.Format("{0}='{1}'", "HANDLING_PROCEDURES", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.OrderedById:
                        result.Add(string.Format("{0}='{1}'", "ORDERED_BY_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.DateOrdered:
                        result.Add(string.Format("{0}='{1}'", "DATE_ORDERED", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.DateReceived:
                        result.Add(string.Format("{0}='{1}'", "DATE_RECEIVED", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.DateCertified:
                        result.Add(string.Format("{0}='{1}'", "DATE_CERTIFIED", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.DateApproved:
                        result.Add(string.Format("{0}='{1}'", "DATE_APPROVED", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.LotNum:
                        result.Add(string.Format("{0}='{1}'", "LOT_NUM", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.ContainerStatusId:
                        result.Add(string.Format("{0}='{1}'", "CONTAINER_STATUS_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.ReceivedById:
                        result.Add(string.Format("{0}='{1}'", "RECEIVED_BY_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.FinelWeight:
                        result.Add(string.Format("{0}='{1}'", "FINAL_WGHT", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.NetWeight:
                        result.Add(string.Format("{0}='{1}'", "NET_WGHT", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.QuantityAvailable:
                        result.Add(string.Format("{0}='{1}'", "QTY_AVAILABLE", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.QuantityReserved:
                        result.Add(string.Format("{0}='{1}'", "QTY_RESERVED", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.PhysicalStateId:
                        result.Add(string.Format("{0}='{1}'", "PHYSICAL_STATE_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.CurrentUserId:
                        result.Add(string.Format("{0}='{1}'", "CURRENT_USER_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.SupplierId:
                        result.Add(string.Format("{0}='{1}'", "SUPPLIER_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.SupplierCatNumber:
                        result.Add(string.Format("{0}='{1}'", "SUPPLIER_CATNUM", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.DateProduced:
                        result.Add(string.Format("{0}='{1}'", "DATE_PRODUCED", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.ContainerCost:
                        result.Add(string.Format("{0}='{1}'", "CONTAINER_COST", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.ContainerCostId:
                        result.Add(string.Format("{0}='{1}'", "UNIT_OF_COST_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.DefaultLocationId:
                        result.Add(string.Format("{0}='{1}'", "DEF_LOCATION_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Barcode:
                        result.Add(string.Format("{0}='{1}'", "BARCODE", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.PONumber:
                        result.Add(string.Format("{0}='{1}'", "PO_NUMBER", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.RequestNumber:
                        result.Add(string.Format("{0}='{1}'", "REQ_NUMBER", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Density:
                        result.Add(string.Format("{0}='{1}'", "DENSITY", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.DensityId:
                        result.Add(string.Format("{0}='{1}'", "UNIT_OF_DENSITY_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.POLineNumber:
                        result.Add(string.Format("{0}='{1}'", "PO_LINE_NUMBER", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Field1:
                        result.Add(string.Format("{0}='{1}'", "FIELD_1", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Field2:
                        result.Add(string.Format("{0}='{1}'", "FIELD_2", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Field3:
                        result.Add(string.Format("{0}='{1}'", "FIELD_3", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Field4:
                        result.Add(string.Format("{0}='{1}'", "FIELD_4", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Field5:
                        result.Add(string.Format("{0}='{1}'", "FIELD_5", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Field6:
                        result.Add(string.Format("{0}='{1}'", "FIELD_6", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Field7:
                        result.Add(string.Format("{0}='{1}'", "FIELD_7", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Field8:
                        result.Add(string.Format("{0}='{1}'", "FIELD_8", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Field9:
                        result.Add(string.Format("{0}='{1}'", "FIELD_9", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Field10:
                        result.Add(string.Format("{0}='{1}'", "FIELD_10", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Date1:
                        result.Add(string.Format("{0}='{1}'", "DATE_1", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Date2:
                        result.Add(string.Format("{0}='{1}'", "DATE_2", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Date3:
                        result.Add(string.Format("{0}='{1}'", "DATE_3", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Date4:
                        result.Add(string.Format("{0}='{1}'", "DATE_4", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.Date5:
                        result.Add(string.Format("{0}='{1}'", "DATE_5", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.PrincipalId:
                        result.Add(string.Format("{0}='{1}'", "PRINCIPAL_ID_FK", property.PropertyValue));
                        break;
                    case ContainerPropertyEnum.LocationTypeId:
                        result.Add(string.Format("{0}='{1}'", "LOCATION_TYPE_ID_FK", property.PropertyValue));
                        break;
                    default:
                        throw new Exception(string.Format("{0} is not a valid property of the container", property.PropertyName));
                }
            }

            return string.Join(",", result);
        }
    }
}
