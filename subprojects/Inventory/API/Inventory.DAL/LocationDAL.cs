using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerkinElmer.COE.Inventory.Model;
using PerkinElmer.COE.Inventory.DAL.Mapper;

namespace PerkinElmer.COE.Inventory.DAL
{
    public class LocationDAL
    {
        MapperBase<INV_LOCATIONS, LocationData> locationMapper = new LocationMapper();

        public List<LocationData> GetLocations()
        {
            using (InventoryDB invDB = new InventoryDB())
            {
                return locationMapper.Map(invDB.INV_LOCATIONS.ToList());
            }
        }

        public LocationData GetLocation(int locationId)
        {
            using (InventoryDB invDB = new InventoryDB())
            {
                return locationMapper.Map(invDB.INV_LOCATIONS.SingleOrDefault(l => l.LOCATION_ID == locationId));
            }
        }
    }
}
