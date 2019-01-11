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

        public ContainerDAL()
        {
        }

        public ContainerDAL(IInventoryDBContext context) : base(context)
        {
        }

        public ContainerData GetContainerById(int containerId)
        {
            return containerMapper.Map(db.INV_CONTAINERS
                .Include(c => c.INV_CONTAINER_TYPES)
                .Include(c => c.INV_CONTAINER_STATUS)
                .Include(c => c.INV_SUPPLIERS)
                .Include(c => c.INV_UNITS)
                .Include(c => c.INV_LOCATIONS)
                .Include(c => c.INV_COMPOUNDS)
                .SingleOrDefault(c => c.CONTAINER_ID == containerId));
        }

        public ContainerData GetContainerByBarcode(string barcode)
        {
            return containerMapper.Map(db.INV_CONTAINERS
                .Include(c => c.INV_CONTAINER_TYPES)
                .Include(c => c.INV_CONTAINER_STATUS)
                .Include(c => c.INV_SUPPLIERS)
                .Include(c => c.INV_UNITS)
                .Include(c => c.INV_LOCATIONS)
                .Include(c => c.INV_COMPOUNDS)
                .SingleOrDefault(c => c.BARCODE == barcode));
        }
    }
}
