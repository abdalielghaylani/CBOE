using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using PerkinElmer.COE.Inventory.Model;
using PerkinElmer.COE.Inventory.DAL.Mapper;
using Oracle.ManagedDataAccess.Client;
using System.Reflection;
using System.Globalization;

namespace PerkinElmer.COE.Inventory.DAL
{
    public class ContainerDAL : BaseDAL
    {
        MapperBase<INV_CONTAINERS, ContainerData> containerMapper = new ContainerMapper();
        MapperBase<INV_CUSTOM_CPD_FIELD_VALUES, CustomFieldData> customFieldMapper = new CustomFieldMapper();
        MapperBase<INV_CONTAINER_TYPES, ContainerTypeData> containerTypeMapper = new ContainerTypeMapper();
        MapperBase<INV_CONTAINER_STATUS, ContainerStatusData> containerStatusMapper = new ContainerStatusMapper();
        MapperBase<INV_UNITS, UnitData> unitMapper = new UnitMapper();

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

        public List<ContainerData> GetContainersByLocationId(int locationId)
        {
            IsValidLocation(locationId);

            var containers = db.INV_CONTAINERS
                .Include(c => c.INV_CONTAINER_TYPES)
                .Include(c => c.INV_CONTAINER_STATUS)
                .Include(c => c.INV_SUPPLIERS)
                .Include(c => c.INV_UNITS)
                .Include(c => c.INV_LOCATIONS)
                .Include(c => c.INV_LOCATIONS1)
                .Include(c => c.INV_COMPOUNDS)
                .Include(c => c.INV_LOCATION_TYPES).Where(c => c.LOCATION_ID_FK == locationId);

            var containersData = new List<ContainerData>();

            foreach (var container in containers)
            {
                var containerData = containerMapper.Map(container);

                if (containerData != null && containerData.Compound != null)
                {
                    containerData.Compound.SafetyData = customFieldMapper.Map(db.INV_CUSTOM_CPD_FIELD_VALUES
                        .Include(c => c.INV_CUSTOM_FIELDS)
                        .Include("INV_CUSTOM_FIELDS.INV_CUSTOM_FIELD_GROUPS")
                        .Where(c => c.COMPOUND_ID_FK == containerData.Compound.CompoundId)
                        .ToList());
                }

                containersData.Add(containerData);
            }

            return containersData;
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

        public ContainerData UpdateContainer(int containerId, ContainerData container)
        {
            IsValidContainer(containerId);

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
                    var valuePairs = GetKeyValuePairsParameters(container);
                    cmd.Parameters.Add(new OracleParameter("PVALUEPAIRS", OracleDbType.NVarchar2, 4000, valuePairs, System.Data.ParameterDirection.Input));
                    cmd.ExecuteNonQuery();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception("The update of the container failed.", ex);
                }
            }
            RefreshDBContext();
            return GetContainerById(containerId);
        }

        public void UpdateContainerRemainingQuantity(int containerId, decimal remainingQuantity)
        {
            IsValidContainer(containerId);

            var dbContext = ((DbContext)db);
            using (var dbContextTransaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Oracle.ManagedDataAccess.Client.OracleConnection connection = (Oracle.ManagedDataAccess.Client.OracleConnection)dbContext.Database.Connection;
                    Oracle.ManagedDataAccess.Client.OracleCommand cmd = dbContext.Database.Connection.CreateCommand() as Oracle.ManagedDataAccess.Client.OracleCommand;
                    cmd.CommandText = "CHEMINVDB2.UpdateContainerQtyRemaining";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Int32, 0, null, System.Data.ParameterDirection.ReturnValue));
                    cmd.Parameters.Add(new OracleParameter("PCONTAINERIDs", OracleDbType.Varchar2, 4000, containerId.ToString(), System.Data.ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("PQTYREMAINING", OracleDbType.Double, 0, remainingQuantity, System.Data.ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("PQTYCHANGED", OracleDbType.Double, 0, null, System.Data.ParameterDirection.Input));
                    cmd.ExecuteNonQuery();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception("The update of the remaining quantty of the container failed.", ex);
                }
            }
        }

        public string MoveContainer(int containerId, int locationId)
        {
            IsValidContainer(containerId);
            IsValidLocation(locationId);

            var dbContext = ((DbContext)db);
            using (var dbContextTransaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Oracle.ManagedDataAccess.Client.OracleConnection connection = (Oracle.ManagedDataAccess.Client.OracleConnection)dbContext.Database.Connection;
                    Oracle.ManagedDataAccess.Client.OracleCommand cmd = dbContext.Database.Connection.CreateCommand() as Oracle.ManagedDataAccess.Client.OracleCommand;
                    cmd.CommandText = "CHEMINVDB2.MoveContainer";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Varchar2, 8000, null, System.Data.ParameterDirection.ReturnValue));
                    cmd.Parameters.Add(new OracleParameter("PLOCATIONID", OracleDbType.Varchar2, 8000, locationId.ToString(), System.Data.ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("PCONTAINERID", OracleDbType.Varchar2, 8000, containerId.ToString(), System.Data.ParameterDirection.Input));
                    cmd.ExecuteNonQuery();
                    dbContextTransaction.Commit();
                    var returnValue = cmd.Parameters["RETURN_VALUE"].Value.ToString();
                    return returnValue;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception("The move container functionality failed.", ex);
                }
            }
        }

        public void UpdateContainerStatus(int containerId, int containerStatusId)
        {
            IsValidContainer(containerId);
            IsValidContainerStatus(containerStatusId);

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
                    var valuePairs = string.Format("{0}='{1}'", "CONTAINER_STATUS_ID_FK", containerStatusId);
                    cmd.Parameters.Add(new OracleParameter("PVALUEPAIRS", OracleDbType.Varchar2, 10000, valuePairs, System.Data.ParameterDirection.Input));
                    cmd.ExecuteNonQuery();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception("The update of the container failed.", ex);
                }
            }
        }

        public List<ContainerData> GetContainers(SearchContainerData searchContainerData)
        {
            var includeCompoundId = searchContainerData.CompoundId.HasValue;
            var includeACXNumber = !string.IsNullOrEmpty(searchContainerData.ACXNumber);
            var includeCASRegistryNumber = !string.IsNullOrEmpty(searchContainerData.CASRegistryNumber);
            var includeCotainerStatusId = searchContainerData.ContainerStatusId.HasValue;
            var includeCurrentUser = !string.IsNullOrEmpty(searchContainerData.CurrentUser);
            var includeLocationBarcode = !string.IsNullOrEmpty(searchContainerData.LocationBarcode);
            var includeLocationId = searchContainerData.LocationId.HasValue;
            var includeLotNumber = !string.IsNullOrEmpty(searchContainerData.LotNumber);
            var includeSubstanceName = !string.IsNullOrEmpty(searchContainerData.SubstanceName);
            var includeRemainingQuantity = searchContainerData.RemainingQuantity.HasValue;
            var includeUnitOfMeasureId = searchContainerData.UnitOfMeasureId.HasValue;

            var containers = db.INV_CONTAINERS
                .Include(c => c.INV_CONTAINER_TYPES)
                .Include(c => c.INV_CONTAINER_STATUS)
                .Include(c => c.INV_SUPPLIERS)
                .Include(c => c.INV_UNITS)
                .Include(c => c.INV_LOCATIONS)
                .Include(c => c.INV_LOCATIONS1)
                .Include(c => c.INV_COMPOUNDS)
                .Include(c => c.INV_LOCATION_TYPES).Where(c => (!includeSubstanceName || c.INV_COMPOUNDS.SUBSTANCE_NAME == searchContainerData.SubstanceName)
                    && (!includeCompoundId || c.INV_COMPOUNDS.COMPOUND_ID == searchContainerData.CompoundId.Value)
                    && (!includeACXNumber || c.INV_COMPOUNDS.ACX_ID == searchContainerData.ACXNumber)
                    && (!includeCASRegistryNumber || c.INV_COMPOUNDS.CAS == searchContainerData.CASRegistryNumber)
                    && (!includeCotainerStatusId || c.INV_COMPOUNDS.COMPOUND_ID == searchContainerData.ContainerStatusId.Value)
                    && (!includeCurrentUser || c.CURRENT_USER_ID_FK == searchContainerData.CurrentUser)
                    && (!includeLocationBarcode || c.INV_LOCATIONS1.LOCATION_BARCODE == searchContainerData.LocationBarcode)
                    && (!includeLocationId || c.INV_LOCATIONS1.LOCATION_ID == searchContainerData.LocationId.Value)
                    && (!includeLotNumber || c.LOT_NUM == searchContainerData.LotNumber)
                    && (!includeRemainingQuantity || c.QTY_REMAINING == searchContainerData.RemainingQuantity.Value)
                    && (!includeUnitOfMeasureId || c.UNIT_OF_MEAS_ID_FK == searchContainerData.UnitOfMeasureId.Value)
                );

            var containersData = new List<ContainerData>();

            foreach (var container in containers)
            {
                var containerData = containerMapper.Map(container);

                if (containerData != null && containerData.Compound != null)
                {
                    containerData.Compound.SafetyData = customFieldMapper.Map(db.INV_CUSTOM_CPD_FIELD_VALUES
                        .Include(c => c.INV_CUSTOM_FIELDS)
                        .Include("INV_CUSTOM_FIELDS.INV_CUSTOM_FIELD_GROUPS")
                        .Where(c => c.COMPOUND_ID_FK == containerData.Compound.CompoundId)
                        .ToList());
                }

                containersData.Add(containerData);
            }

            return containersData;
        }

        public List<ContainerTypeData> GetContainerTypes()
        {
            var result = new List<ContainerTypeData>();

            foreach (var containerType in db.INV_CONTAINER_TYPES)
            {
                result.Add(containerTypeMapper.Map(containerType));
            }

            return result;
        }

        public List<ContainerStatusData> GetContainerStatus()
        {
            var result = new List<ContainerStatusData>();

            foreach (var containerStatus in db.INV_CONTAINER_STATUS)
            {
                result.Add(containerStatusMapper.Map(containerStatus));
            }

            return result;
        }

        public List<UnitData> GetContainerUnits()
        {
            var result = new List<UnitData>();

            foreach (var unit in db.INV_UNITS)
            {
                result.Add(unitMapper.Map(unit));
            }

            return result;
        }

        public void DeleteContainer(int containerId)
        {
            IsValidContainer(containerId);

            var dbContext = ((DbContext)db);
            using (var dbContextTransaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Oracle.ManagedDataAccess.Client.OracleConnection connection = (Oracle.ManagedDataAccess.Client.OracleConnection)dbContext.Database.Connection;
                    Oracle.ManagedDataAccess.Client.OracleCommand cmd = dbContext.Database.Connection.CreateCommand() as Oracle.ManagedDataAccess.Client.OracleCommand;
                    cmd.CommandText = "CHEMINVDB2.DeleteContainer";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Int32, 0, null, System.Data.ParameterDirection.ReturnValue));
                    cmd.Parameters.Add(new OracleParameter("PCONTAINERID", OracleDbType.Varchar2, 2000, containerId.ToString(), System.Data.ParameterDirection.Input));
                    cmd.ExecuteNonQuery();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception("The delete of the container failed.", ex);
                }
            }
        }

        private void IsValidContainer(int containerId)
        {
            var container = db.INV_CONTAINERS.FirstOrDefault(s => s.CONTAINER_ID == containerId);
            if (container == null)
            {
                throw new IndexOutOfRangeException(string.Format("Cannot find the container, {0}", containerId));
            }
        }

        private void IsValidContainerStatus(int containerStatusId)
        {
            var status = db.INV_CONTAINER_STATUS.FirstOrDefault(s => s.CONTAINER_STATUS_ID == containerStatusId);
            if (status == null)
            {
                throw new IndexOutOfRangeException(string.Format("Cannot find the container status, {0}", containerStatusId));
            }
        }

        private void IsValidLocation(int locationId)
        {
            var location = db.INV_LOCATIONS.FirstOrDefault(s => s.LOCATION_ID == locationId);
            if (location == null)
            {
                throw new IndexOutOfRangeException(string.Format("Cannot find the location, {0}", locationId));
            }
        }

        private void ValidateContainer(ContainerData container)
        {
            if (container.Location == null)
            {
                throw new Exception("LocationID is a required parameter");
            }

            if (string.IsNullOrEmpty(container.ContainerName))
            {
                throw new Exception("ContainerName is a required parameter");
            }

            if (container.QuantityMax == null || container.QuantityMax.Value == null || container.QuantityMax.Id <= 0)
            {
                throw new Exception("QuantityMax is a required parameter, the value and id of the unit must be entered");
            }

            if (container.QuantityInitial == null || container.QuantityInitial.Value == null || container.QuantityInitial.Id <= 0)
            {
                throw new Exception("QuantityInitial is a required parameter, the value and id of the unit must be entered");
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

        private string GetKeyValuePairsParameters(ContainerData container)
        {
            var result = new List<string>();

            foreach (PropertyInfo propertyInfo in container.GetType().GetProperties())
            {
                var propertyValue = container.GetType().GetProperty(propertyInfo.Name).GetValue(container, null);
                if (propertyValue != null)
                {
                    switch (propertyInfo.Name)
                    {
                        case "Location":
                            if (container.Location.Id > -1)
                            {
                                result.Add(string.Format("{0}={1}", "LOCATION_ID_FK", container.Location.Id));
                            }
                            if (container.Location.LocationType != null && container.Location.LocationType.LocationTypeId > 0)
                            {
                                result.Add(string.Format("{0}={1}", "LOCATION_TYPE_ID_FK", container.Location.LocationType.LocationTypeId));
                            }
                            break;
                        case "Compound":
                            if (container.Compound.CompoundId > 0)
                            {
                                result.Add(string.Format("{0}={1}", "COMPOUND_ID_FK", container.Compound.CompoundId));
                            }
                            break;
                        case "ParentContainerId":
                            if (container.ParentContainerId.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "PARENT_CONTAINER_ID_FK", container.ParentContainerId.Value));
                            }
                            break;
                        case "RegId":
                            if (container.RegId.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "REG_ID_FK", container.RegId.Value));
                            }
                            break;
                        case "BatchNumber":
                            if (container.BatchNumber.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "BATCH_NUMBER_FK", container.BatchNumber.Value));
                            }
                            break;
                        case "Family":
                            if (container.Family.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "FAMILY", container.Family.Value));
                            }
                            break;
                        case "BatchId":
                            if (container.BatchId.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "BATCH_ID_FK", container.BatchId));
                            }
                            break;
                        case "ContainerName":
                            result.Add(string.Format("{0}='{1}'", "CONTAINER_NAME", container.ContainerName));
                            break;
                        case "ContainerDescription":
                            result.Add(string.Format("{0}='{1}'", "CONTAINER_DESCRIPTION", container.Description));
                            break;
                        case "WellNumber":
                            if (container.WellNumber.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "WELL_NUMBER", container.WellNumber.Value));
                            }
                            break;
                        case "WellRow":
                            if (container.WellRow != null)
                            {
                                result.Add(string.Format("{0}='{1}'", "WELL_ROW", container.WellRow));
                            }
                            break;
                        case "WellColumn":
                            if (container.WellColumn != null)
                            {
                                result.Add(string.Format("{0}='{1}'", "WELL_COLUMN", container.WellColumn));
                            }
                            break;
                        case "DateExpires":
                            if (container.ExpirationDate.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "DATE_EXPIRES",
                                    string.Format("TO_DATE('{0}', 'MM/DD/YYYY')", container.ExpirationDate.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))));
                            }
                            break;
                        case "ContainerType":
                            if (container.ContainerType.ContainerTypeId > 0)
                            {
                                result.Add(string.Format("{0}={1}", "CONTAINER_TYPE_ID_FK", container.ContainerType.ContainerTypeId));
                            }
                            break;
                        case "Purity":
                            if (container.Purity.Value.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "PURITY", container.Purity.Value.Value));
                            }
                            if (container.Purity.Id > 0)
                            {
                                result.Add(string.Format("{0}={1}", "UNIT_OF_PURITY_ID_FK", container.Purity.Id));
                            }
                            break;
                        case "SolventId":
                            if (container.SolventId != null)
                            {
                                result.Add(string.Format("{0}='{1}'", "SOLVENT_ID_FK", container.SolventId));
                            }
                            break;
                        case "Concentration":
                            if (container.Concentration.Value.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "CONCENTRATION", container.Concentration.Value.Value));
                            }
                            if (container.Concentration.Id > 0)
                            {
                                result.Add(string.Format("{0}={1}", "UNIT_OF_CONC_ID_FK", container.Concentration.Id));
                            }
                            break;
                        case "QuantityMax":
                            if (container.QuantityMax.Value.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "QTY_MAX", container.QuantityMax.Value));
                            }
                            if (container.QuantityMax.Id > 0)
                            {
                                result.Add(string.Format("{0}={1}", "UNIT_OF_MEAS_ID_FK", container.QuantityMax.Id));
                            }
                            break;
                        case "Grade":
                            if (container.Grade != null)
                            {
                                result.Add(string.Format("{0}='{1}'", "GRADE", container.Grade));
                            }
                            break;
                        case "Weight":
                            if (container.Weight.Value.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "WEIGHT", container.Weight.Value.Value));
                            }
                            if (container.Weight.Id > 0)
                            {
                                result.Add(string.Format("{0}={1}", "UNIT_OF_WGHT_ID_FK", container.Weight.Id));
                            }
                            break;
                        case "TareWeight":
                            if (container.TareWeight.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "TARE_WEIGHT", container.TareWeight.Value));
                            }
                            break;
                        case "OwnerId":
                            result.Add(string.Format("{0}='{1}'", "OWNER_ID_FK", container.OwnerId));
                            break;
                        case "ContainerComments":
                            result.Add(string.Format("{0}='{1}'", "CONTAINER_COMMENTS", container.Comments));
                            break;
                        case "StorageConditions":
                            result.Add(string.Format("{0}='{1}'", "STORAGE_CONDITIONS", container.StorageConditions));
                            break;
                        case "HandlingProcedures":
                            result.Add(string.Format("{0}='{1}'", "HANDLING_PROCEDURES", container.HandlingProcedures));
                            break;
                        case "OrderedById":
                            result.Add(string.Format("{0}='{1}'", "ORDERED_BY_ID_FK", container.OrderedById));
                            break;
                        case "DateOrdered":
                            if (container.DateOrdered.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "DATE_ORDERED",
                                    string.Format("TO_DATE('{0}', 'MM/DD/YYYY')", container.DateOrdered.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))));
                            }
                            break;
                        case "DateReceived":
                            if (container.DateReceived.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "DATE_RECEIVED",
                                    string.Format("TO_DATE('{0}', 'MM/DD/YYYY')", container.DateReceived.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))));
                            }
                            break;
                        case "DateCertified":
                            if (container.DateCertified.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "DATE_CERTIFIED",
                                    string.Format("TO_DATE('{0}', 'MM/DD/YYYY')", container.DateCertified.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))));
                            }
                            break;
                        case "DateApproved":
                            if (container.DateApproved.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "DATE_APPROVED",
                                    string.Format("TO_DATE('{0}', 'MM/DD/YYYY')", container.DateApproved.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))));
                            }
                            break;
                        case "LotNum":
                            result.Add(string.Format("{0}='{1}'", "LOT_NUM", container.LotNumber));
                            break;
                        case "Status":
                            if (container.Status.StatusId > 0)
                            {
                                result.Add(string.Format("{0}={1}", "CONTAINER_STATUS_ID_FK", container.Status.StatusId));
                            }
                            break;
                        case "ReceivedById":
                            result.Add(string.Format("{0}='{1}'", "RECEIVED_BY_ID_FK", container.ReceivedById));
                            break;
                        case "FinalWeight":
                            if (container.FinalWeight.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "FINAL_WGHT", container.FinalWeight.Value));
                            }
                            break;
                        case "NetWeight":
                            if (container.NetWeight.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "NET_WGHT", container.NetWeight.Value));
                            }
                            break;
                        case "QuantityInitial":
                            if (container.QuantityInitial.Value.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "QTY_INITIAL", container.QuantityInitial.Value));
                            }
                            break;
                        case "QuantityRemaining":
                            if (container.QuantityRemaining.Value.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "QTY_REMAINING", container.QuantityRemaining.Value));
                            }
                            break;
                        case "QuantityMinStock":
                            if (container.QuantityMinStock.Value.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "QTY_MINSTOCK", container.QuantityMinStock.Value));
                            }
                            break;
                        case "QuantityMaxStock":
                            if (container.QuantityMaxStock.Value.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "QTY_MAXSTOCK", container.QuantityMaxStock.Value));
                            }
                            break;
                        case "QuantityAvailable":
                            if (container.QuantityAvailable != null & container.QuantityAvailable.Value.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "QTY_AVAILABLE", container.QuantityAvailable.Value.Value));
                            }
                            break;
                        case "QuantityReserved":
                            if (container.QuantityReserved != null & container.QuantityReserved.Value.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "QTY_RESERVED", container.QuantityReserved.Value.Value));
                            }
                            break;
                        case "PhysicalStateId":
                            if (container.PhysicalStateId.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "PHYSICAL_STATE_ID_FK", container.PhysicalStateId.Value));
                            }
                            break;
                        case "CurrentUser":
                            result.Add(string.Format("{0}='{1}'", "CURRENT_USER_ID_FK", container.CurrentUser));
                            break;
                        case "Supplier":
                            if (container.Supplier.SupplierId > 0)
                            {
                                result.Add(string.Format("{0}={1}", "SUPPLIER_ID_FK", container.Supplier.SupplierId));
                            }
                            if (container.Supplier.CatNumber != null)
                            {
                                result.Add(string.Format("{0}='{1}'", "SUPPLIER_CATNUM", container.Supplier.CatNumber));
                            }
                            break;
                        case "DateProduced":
                            if (container.DateProduced.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "DATE_PRODUCED",
                                        string.Format("TO_DATE('{0}', 'MM/DD/YYYY')", container.DateProduced.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))));
                            }
                            break;
                        case "ContainerCost":
                            if (container.ContainerCost.Value.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "CONTAINER_COST", container.ContainerCost.Value.Value));
                            }
                            if (container.ContainerCost.Id > 0)
                            {
                                result.Add(string.Format("{0}={1}", "UNIT_OF_COST_ID_FK", container.ContainerCost.Id));
                            }
                            break;
                        case "Barcode":
                            result.Add(string.Format("{0}='{1}'", "BARCODE", container.Barcode));
                            break;
                        case "PONumber":
                            result.Add(string.Format("{0}='{1}'", "PO_NUMBER", container.PONumber));
                            break;
                        case "RequestNumber":
                            result.Add(string.Format("{0}='{1}'", "REQ_NUMBER", container.RequestNumber));
                            break;
                        case "Density":
                            if (container.Density.Value.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "DENSITY", container.Density.Value.Value));
                            }
                            if (container.Density.Id > 0)
                            {
                                result.Add(string.Format("{0}={1}", "UNIT_OF_DENSITY_ID_FK", container.Density.Id));
                            }
                            break;
                        case "POLineNumber":
                            result.Add(string.Format("{0}='{1}'", "PO_LINE_NUMBER", container.POLineNumber));
                            break;
                        case "Field1":
                            result.Add(string.Format("{0}='{1}'", "FIELD_1", container.Field1));
                            break;
                        case "Field2":
                            result.Add(string.Format("{0}='{1}'", "FIELD_2", container.Field2));
                            break;
                        case "Field3":
                            result.Add(string.Format("{0}='{1}'", "FIELD_3", container.Field3));
                            break;
                        case "Field4":
                            result.Add(string.Format("{0}='{1}'", "FIELD_4", container.Field4));
                            break;
                        case "Field5":
                            result.Add(string.Format("{0}='{1}'", "FIELD_5", container.Field5));
                            break;
                        case "Field6":
                            result.Add(string.Format("{0}='{1}'", "FIELD_6", container.Field6));
                            break;
                        case "Field7":
                            result.Add(string.Format("{0}='{1}'", "FIELD_7", container.Field7));
                            break;
                        case "Field8":
                            result.Add(string.Format("{0}='{1}'", "FIELD_8", container.Field8));
                            break;
                        case "Field9":
                            result.Add(string.Format("{0}='{1}'", "FIELD_9", container.Field9));
                            break;
                        case "Field10":
                            result.Add(string.Format("{0}='{1}'", "FIELD_10", container.Field10));
                            break;
                        case "Date1":
                            if (container.Date1.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "DATE_1",
                                            string.Format("TO_DATE('{0}', 'MM/DD/YYYY')", container.Date1.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))));
                            }
                            break;
                        case "Date2":
                            if (container.Date2.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "DATE_2",
                                        string.Format("TO_DATE('{0}', 'MM/DD/YYYY')", container.Date2.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))));
                            }
                            break;
                        case "Date3":
                            if (container.Date3.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "DATE_3",
                                        string.Format("TO_DATE('{0}', 'MM/DD/YYYY')", container.Date3.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))));
                            }
                            break;
                        case "Date4":
                            if (container.Date4.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "DATE_4",
                                        string.Format("TO_DATE('{0}', 'MM/DD/YYYY')", container.Date4.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))));
                            }
                            break;
                        case "Date5":
                            if (container.Date5.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "DATE_5",
                                        string.Format("TO_DATE('{0}', 'MM/DD/YYYY')", container.Date5.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))));
                            }
                            break;
                        case "PrincipalId":
                            if (container.PrincipalId.HasValue)
                            {
                                result.Add(string.Format("{0}={1}", "PRINCIPAL_ID_FK", container.PrincipalId.Value));
                            }
                            break;
                    }
                }
            }

            return string.Join(",", result);
        }
    }
}
