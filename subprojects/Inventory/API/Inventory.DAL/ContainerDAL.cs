using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using PerkinElmer.COE.Inventory.Model;
using PerkinElmer.COE.Inventory.DAL.Mapper;

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
                .SingleOrDefault(c => c.BARCODE == barcode));

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
    }
}
