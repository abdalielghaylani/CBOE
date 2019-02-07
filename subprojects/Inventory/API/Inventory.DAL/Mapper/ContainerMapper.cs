using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerkinElmer.COE.Inventory.Model;

namespace PerkinElmer.COE.Inventory.DAL.Mapper
{
    public sealed class ContainerMapper : MapperBase<INV_CONTAINERS, ContainerData>
    {
        public override INV_CONTAINERS Map(ContainerData element)
        {
            // convert from DTO to Entity
            throw new NotImplementedException();
        }

        public override ContainerData Map(INV_CONTAINERS element)
        {
            if (element == null) return null;

            return new ContainerData
            {
                ContainerId = element.CONTAINER_ID,
                Barcode = element.BARCODE,
                Name = element.CONTAINER_NAME,
                Type = (element.INV_CONTAINER_TYPES != null) ? element.INV_CONTAINER_TYPES.CONTAINER_TYPE_NAME : string.Empty,
                ContainerSize = new UnitData() { Value = element.QTY_MAX, Unit = (element.INV_UNITS1 != null) ? element.INV_UNITS1.UNIT_ABREVIATION : string.Empty },
                QuantityAvailable = element.QTY_AVAILABLE,
                Concentration = new UnitData() { Value = element.CONCENTRATION, Unit = (element.INV_UNITS != null) ? element.INV_UNITS.UNIT_ABREVIATION : string.Empty },
                Supplier = (element.INV_SUPPLIERS != null) ? element.INV_SUPPLIERS.SUPPLIER_NAME : string.Empty,
                CurrentUser = element.CURRENT_USER_ID_FK,
                Purity = new UnitData() { Value = element.PURITY, Unit = (element.INV_UNITS2 != null) ? element.INV_UNITS2.UNIT_ABREVIATION : string.Empty },
                Weight = new UnitData() { Value = element.FINAL_WGHT, Unit = (element.INV_UNITS3 != null) ? element.INV_UNITS3.UNIT_ABREVIATION : string.Empty },
                Density = new UnitData() { Value = element.DENSITY, Unit = (element.INV_UNITS4 != null) ? element.INV_UNITS4.UNIT_ABREVIATION : string.Empty },
                DateCreated = element.DATE_CREATED,
                Status = (element.INV_CONTAINER_STATUS != null) ? element.INV_CONTAINER_STATUS.CONTAINER_STATUS_NAME : string.Empty,
                Compound = new CompoundMapper().Map(element.INV_COMPOUNDS),
                Location = new LocationMapper().Map(element.INV_LOCATIONS1)
            };
        }
    }
}
