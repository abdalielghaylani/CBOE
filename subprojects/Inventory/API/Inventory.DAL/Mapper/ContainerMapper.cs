using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using PerkinElmer.COE.Inventory.Model;

namespace PerkinElmer.COE.Inventory.DAL.Mapper
{
    public sealed class ContainerMapper : MapperBase<INV_CONTAINERS, ContainerData>
    {
        public override Array GetOracleParameters(ContainerData element)
        {
            var parameters = new List<OracleParameter>();

            parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Int32, 0, null, System.Data.ParameterDirection.ReturnValue));

            parameters.Add(new OracleParameter("PBARCODE", OracleDbType.Varchar2, 50, element.Barcode, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PBARCODEDESCID", OracleDbType.Int32, 50, element.BarcodeDescriptionId, System.Data.ParameterDirection.Input));

            var locationId = element.Location != null ? element.Location.Id.ToString() : string.Empty;
            parameters.Add(new OracleParameter("PLOCATIONID", OracleDbType.Varchar2, 4000, locationId, System.Data.ParameterDirection.Input));

            decimal? quantityMax = null;
            int? quantityMaxId = null;
            if (element.QuantityMax != null) { quantityMax = element.QuantityMax.Value; quantityMaxId = element.QuantityMax.Id; };
            parameters.Add(new OracleParameter("PUOMID", OracleDbType.Int32, 0, quantityMaxId, System.Data.ParameterDirection.Input));

            int? containerTypeId = null;
            if (element.ContainerType != null) { containerTypeId = element.ContainerType.ContainerTypeId; };
            parameters.Add(new OracleParameter("PCONTAINERTYPEID", OracleDbType.Int32, 0, containerTypeId, System.Data.ParameterDirection.Input));

            int? containerStatusId = null;
            if (element.Status != null) { containerStatusId = element.Status.StatusId; };
            parameters.Add(new OracleParameter("PCONTAINERSTATUSID", OracleDbType.Int32, 0, containerStatusId, System.Data.ParameterDirection.Input));

            parameters.Add(new OracleParameter("PMAXQTY", OracleDbType.Double, 0, quantityMax, System.Data.ParameterDirection.Input));

            parameters.Add(new OracleParameter("PREGID", OracleDbType.Int32, 0, element.RegId, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PBATCHNUMBER", OracleDbType.Int32, 0, element.RequestNumber, System.Data.ParameterDirection.Input));

            decimal? quantityInitial = null;
            if (element.QuantityInitial != null) { quantityInitial = element.QuantityInitial.Value; };
            parameters.Add(new OracleParameter("PINITIALQTY", OracleDbType.Double, 0, quantityInitial, System.Data.ParameterDirection.Input));

            decimal? quantityRemaining = null;
            if (element.QuantityRemaining != null) { quantityRemaining = element.QuantityRemaining.Value; };
            parameters.Add(new OracleParameter("PQTYREMAINING", OracleDbType.Double, 0, quantityRemaining, System.Data.ParameterDirection.Input));

            parameters.Add(new OracleParameter("PMINSTOCKQTY", OracleDbType.Double, 0, element.MinStockQty, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PMAXSTOCKQTY", OracleDbType.Double, 0, element.MaxStockQty, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PEXPDATE", OracleDbType.TimeStamp, 0, element.ExpirationDate, System.Data.ParameterDirection.Input));

            int? CompoundId = null;
            if (element.Compound != null) { CompoundId = element.Compound.CompoundId; };
            parameters.Add(new OracleParameter("PCOMPOUNDID", OracleDbType.Int32, 0, CompoundId, System.Data.ParameterDirection.Input));

            parameters.Add(new OracleParameter("PCONTAINERNAME", OracleDbType.Varchar2, 255, element.ContainerName, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PCONTAINERDESC", OracleDbType.Varchar2, 255, element.Description, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PTAREWEIGHT", OracleDbType.Double, 0, element.TareWeight, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PNETWEIGHT", OracleDbType.Double, 0, element.NetWeight, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PFINALWEIGHT", OracleDbType.Double, 0, element.FinalWeight, System.Data.ParameterDirection.Input));

            int? weightId = null;
            if (element.Weight != null) { weightId = element.Weight.Id; };
            parameters.Add(new OracleParameter("PUOWID", OracleDbType.Int32, 0, weightId, System.Data.ParameterDirection.Input));

            decimal? purity = null;
            int? purityId = null;
            if (element.Purity != null)
            {
                purityId = element.Purity.Id;
                purity = element.Purity.Value;
            };
            parameters.Add(new OracleParameter("PPURITY", OracleDbType.Double, 0, purity, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PUOPID", OracleDbType.Int32, 0, purityId, System.Data.ParameterDirection.Input));

            decimal? concentration = null;
            int? concentrationId = null;
            if (element.Concentration != null)
            {
                concentrationId = element.Concentration.Id;
                concentration = element.Concentration.Value;
            };
            parameters.Add(new OracleParameter("PCONCENTRATION", OracleDbType.Double, 0, concentration, System.Data.ParameterDirection.Input));

            decimal? density = null;
            int? densityId = null;
            if (element.Density != null)
            {
                densityId = element.Density.Id;
                density = element.Density.Value;
            };
            parameters.Add(new OracleParameter("PDENSITY", OracleDbType.Double, 0, density, System.Data.ParameterDirection.Input));

            parameters.Add(new OracleParameter("PUOCID", OracleDbType.Int32, 0, concentrationId, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PUODID", OracleDbType.Int32, 0, densityId, System.Data.ParameterDirection.Input));

            parameters.Add(new OracleParameter("PSOLVENTIDFK", OracleDbType.Varchar2, 255, element.SolventId, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PGRADE", OracleDbType.Varchar2, 255, element.Grade, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PCOMMENTS", OracleDbType.Varchar2, 4000, element.Comments, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PSTORAGECONDITIONS", OracleDbType.Varchar2, 4000, element.StorageConditions, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PHANDLINGPROCEDURES", OracleDbType.Varchar2, 4000, element.HandlingProcedures, System.Data.ParameterDirection.Input));

            int? supplierId = null;
            string catNumber = null;
            if (element.Supplier != null)
            {
                supplierId = element.Supplier.SupplierId;
                catNumber = element.Supplier.CatNumber;
            };
            parameters.Add(new OracleParameter("PSUPPLIERID", OracleDbType.Int32, 0, supplierId, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PSUPPLIERCATNUM", OracleDbType.Varchar2, 50, catNumber, System.Data.ParameterDirection.Input));

            parameters.Add(new OracleParameter("PLOTNUM", OracleDbType.Varchar2, 50, element.LotNumber, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PDATEPRODUCED", OracleDbType.TimeStamp, 0, element.DateProduced, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PDATEORDERED", OracleDbType.TimeStamp, 0, element.DateOrdered, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PDATERECEIVED", OracleDbType.TimeStamp, 0, element.DateReceived, System.Data.ParameterDirection.Input));

            decimal? containerCost = null;
            int? containerCostId = null;
            if (element.ContainerCost != null)
            {
                containerCostId = element.ContainerCost.Id;
                containerCost = element.ContainerCost.Value;
            };
            parameters.Add(new OracleParameter("PCONTAINERCOST", OracleDbType.Int32, 0, containerCost, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PUOCOSTID", OracleDbType.Int32, 50, containerCostId, System.Data.ParameterDirection.Input));

            parameters.Add(new OracleParameter("PPONUMBER", OracleDbType.Varchar2, 50, element.PONumber, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PPOLINENUMBER", OracleDbType.Varchar2, 50, element.POLineNumber, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PREQNUMBER", OracleDbType.Varchar2, 50, element.RequestNumber, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("POWNERID", OracleDbType.Varchar2, 50, element.OwnerId, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PCURRENTUSERID", OracleDbType.Varchar2, 50, element.CurrentUser, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PNUMCOPIES", OracleDbType.Int32, 50, element.NumberOfCopies.HasValue ? element.NumberOfCopies.Value : 1, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PNEWIDS", OracleDbType.Varchar2, 4000, null, System.Data.ParameterDirection.Output));
            parameters.Add(new OracleParameter("PFIELD_1", OracleDbType.Varchar2, 2000, element.Field1, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PFIELD_2", OracleDbType.Varchar2, 2000, element.Field2, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PFIELD_3", OracleDbType.Varchar2, 2000, element.Field3, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PFIELD_4", OracleDbType.Varchar2, 2000, element.Field4, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PFIELD_5", OracleDbType.Varchar2, 2000, element.Field5, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PFIELD_6", OracleDbType.Varchar2, 2000, element.Field6, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PFIELD_7", OracleDbType.Varchar2, 2000, element.Field7, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PFIELD_8", OracleDbType.Varchar2, 2000, element.Field8, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PFIELD_9", OracleDbType.Varchar2, 2000, element.Field9, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PFIELD_10", OracleDbType.Varchar2, 2000, element.Field10, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PDATE_1", OracleDbType.TimeStamp, 0, element.Date1, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PDATE_2", OracleDbType.TimeStamp, 0, element.Date2, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PDATE_3", OracleDbType.TimeStamp, 0, element.Date3, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PDATE_4", OracleDbType.TimeStamp, 0, element.Date4, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PDATE_5", OracleDbType.TimeStamp, 0, element.Date5, System.Data.ParameterDirection.Input));
            parameters.Add(new OracleParameter("PPRINCIPALID", OracleDbType.Double, 0, element.PrincipalId, System.Data.ParameterDirection.Input));

            int? locationTypeId = null;
            if (element.Location != null && element.Location.LocationType != null) { locationTypeId = element.Location.LocationType.LocationTypeId; }
            parameters.Add(new OracleParameter("PLOCATIONTYPE", OracleDbType.Double, 0, locationTypeId, System.Data.ParameterDirection.Input));

            return parameters.ToArray();
        }

        public override INV_CONTAINERS Map(ContainerData element)
        {
            // convert from DTO to Entity
            throw new NotImplementedException();
        }

        public override ContainerData Map(INV_CONTAINERS element)
        {
            if (element == null) return null;

            var container = new ContainerData
            {
                ContainerId = element.CONTAINER_ID,
                Barcode = element.BARCODE,
                ContainerName = element.CONTAINER_NAME,
                CurrentUser = element.CURRENT_USER_ID_FK,
                DateCreated = element.DATE_CREATED,
                Compound = new CompoundMapper().Map(element.INV_COMPOUNDS),
                Location = new LocationMapper().Map(element.INV_LOCATIONS1),
                BatchNumber = element.BATCH_NUMBER_FK,
                Comments = element.CONTAINER_COMMENTS,
                Field1 = element.FIELD_1,
                Field2 = element.FIELD_2,
                Field3 = element.FIELD_3,
                Field4 = element.FIELD_4,
                Field5 = element.FIELD_5,
                Field6 = element.FIELD_6,
                Field7 = element.FIELD_7,
                Field8 = element.FIELD_8,
                Field9 = element.FIELD_9,
                Field10 = element.FIELD_10,
                Date1 = element.DATE_1,
                Date2 = element.DATE_2,
                Date3 = element.DATE_3,
                Date4 = element.DATE_4,
                Date5 = element.DATE_5,
                DateOrdered = element.DATE_ORDERED,
                DateProduced = element.DATE_PRODUCED,
                DateReceived = element.DATE_RECEIVED,
                ExpirationDate = element.DATE_EXPIRES,
                DateApproved = element.DATE_APPROVED,
                Description = element.CONTAINER_DESCRIPTION,
                FinalWeight = element.FINAL_WGHT,
                TareWeight = element.TARE_WEIGHT,
                NetWeight = element.NET_WGHT,
                Grade = element.GRADE,
                PrincipalId = element.PRINCIPAL_ID_FK,
                PONumber = element.PO_NUMBER,
                POLineNumber = element.PO_LINE_NUMBER.ToString(),
                HandlingProcedures = element.HANDLING_PROCEDURES,
                StorageConditions = element.STORAGE_CONDITIONS,
                LotNumber = element.LOT_NUM,
                MaxStockQty = element.QTY_MAXSTOCK,
                MinStockQty = element.QTY_MINSTOCK,
                RegId = element.REG_ID_FK,
                RequestNumber = element.REQ_NUMBER,
                SolventId = element.SOLVENT_ID_FK.ToString(),
                OwnerId = element.OWNER_ID_FK,
                QuantityAvailable = (element.QTY_AVAILABLE.HasValue) ? new UnitData() { Value = element.QTY_AVAILABLE } : null,
                QuantityInitial = new UnitData() { Value = element.QTY_INITIAL },
                QuantityMax = new UnitData() { Value = element.QTY_MAX },
                QuantityRemaining = new UnitData() { Value = element.QTY_REMAINING },
                Concentration = (element.CONCENTRATION.HasValue) ? new UnitData() { Value = element.CONCENTRATION } : null,
                Purity = (element.PURITY.HasValue) ? new UnitData() { Value = element.PURITY } : null,
                Weight = (element.WEIGHT.HasValue) ? new UnitData() { Value = element.WEIGHT } : null,
                Density = (element.DENSITY.HasValue) ? new UnitData() { Value = element.DENSITY } : null,

            };

            if (element.INV_CONTAINER_TYPES != null)
            {
                container.ContainerType = new ContainerTypeData() { ContainerTypeId = element.INV_CONTAINER_TYPES.CONTAINER_TYPE_ID, Name = element.INV_CONTAINER_TYPES.CONTAINER_TYPE_NAME };
            }

            if (element.INV_UNITS1 != null)
            {
                if (container.QuantityAvailable != null)
                {
                    container.QuantityAvailable.Id = element.INV_UNITS1.UNIT_ID;
                    container.QuantityAvailable.Unit = element.INV_UNITS1.UNIT_ABREVIATION;
                }

                container.QuantityInitial.Id = element.INV_UNITS1.UNIT_ID;
                container.QuantityInitial.Unit = element.INV_UNITS1.UNIT_ABREVIATION;

                container.QuantityMax.Id = element.INV_UNITS1.UNIT_ID;
                container.QuantityMax.Unit = element.INV_UNITS1.UNIT_ABREVIATION;

                container.QuantityRemaining.Id = element.INV_UNITS1.UNIT_ID;
                container.QuantityRemaining.Unit = element.INV_UNITS1.UNIT_ABREVIATION;
            }

            if (element.INV_UNITS != null && container.Concentration != null)
            {
                container.Concentration.Id = element.INV_UNITS.UNIT_ID;
                container.Concentration.Unit = element.INV_UNITS.UNIT_ABREVIATION;
            }

            if (element.INV_UNITS2 != null && container.Purity != null)
            {
                container.Purity.Id = element.INV_UNITS2.UNIT_ID;
                container.Purity.Unit = element.INV_UNITS2.UNIT_ABREVIATION;
            }

            if (element.INV_UNITS3 != null && container.Weight != null)
            {
                container.Weight.Id = element.INV_UNITS3.UNIT_ID;
                container.Weight.Unit = element.INV_UNITS3.UNIT_ABREVIATION;
            }

            if (element.INV_UNITS4 != null && container.Density != null)
            {
                container.Density.Id = element.INV_UNITS4.UNIT_ID;
                container.Density.Unit = element.INV_UNITS4.UNIT_ABREVIATION;
            }

            if (element.INV_CONTAINER_STATUS != null)
            {
                container.Status = new ContainerStatusData() { StatusId = element.INV_CONTAINER_STATUS.CONTAINER_STATUS_ID, Name = element.INV_CONTAINER_STATUS.CONTAINER_STATUS_NAME };
            }

            if (element.INV_SUPPLIERS != null)
            {
                container.Supplier = new SupplierData() { SupplierId = element.INV_SUPPLIERS.SUPPLIER_ID, Name = element.INV_SUPPLIERS.SUPPLIER_NAME };
            }

            container.ContainerCost = new UnitData();
            if (element.UNIT_OF_COST_ID_FK.HasValue)
            {
                container.ContainerCost.Id = int.Parse(element.UNIT_OF_COST_ID_FK.ToString());
            }
            if (element.CONTAINER_COST.HasValue)
            {
                container.ContainerCost.Value = element.CONTAINER_COST;
            }

            return container;
        }
    }
}
