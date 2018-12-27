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
    public class ContainerDAL
    {
        MapperBase<INV_CONTAINERS, ContainerData> containerMapper = new ContainerMapper();

        public ContainerData GetContainerById(int containerId)
        {
            using (InventoryDB invDB = new InventoryDB())
            {
                return containerMapper.Map(invDB.INV_CONTAINERS
                    .Include(c => c.INV_CONTAINER_TYPES)
                    .Include(c => c.INV_CONTAINER_STATUS)
                    .Include(c => c.INV_SUPPLIERS)
                    .Include(c => c.INV_UNITS)
                    .Include(c => c.INV_LOCATIONS)
                    .SingleOrDefault(c => c.CONTAINER_ID == containerId));

            }
            
        }

        public ContainerData GetContainerByBarcode(string barcode)
        {
            using (InventoryDB invDB = new InventoryDB())
            {
                return containerMapper.Map(invDB.INV_CONTAINERS
                    .Include(c => c.INV_CONTAINER_TYPES)
                    .Include(c => c.INV_CONTAINER_STATUS)
                    .Include(c => c.INV_SUPPLIERS)
                    .Include(c => c.INV_UNITS)
                    .Include(c => c.INV_LOCATIONS)
                    .SingleOrDefault(c => c.BARCODE == barcode));
            }
        }
    }
}
