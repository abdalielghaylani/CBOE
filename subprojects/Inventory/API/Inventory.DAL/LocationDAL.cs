using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerkinElmer.COE.Inventory.Model;
using PerkinElmer.COE.Inventory.DAL.Mapper;

namespace PerkinElmer.COE.Inventory.DAL
{
    public class LocationDAL : BaseDAL
    {

        MapperBase<INV_LOCATIONS, LocationData> locationMapper = new LocationMapper();

        public LocationDAL()
        {
        }

        public LocationDAL(IInventoryDBContext context) : base(context)
        {
        }

        public List<LocationData> GetLocations()
        {
            return locationMapper.Map(db.INV_LOCATIONS.ToList());
        }

        public LocationData GetLocation(int locationId)
        {
            return locationMapper.Map(db.INV_LOCATIONS.SingleOrDefault(l => l.LOCATION_ID == locationId));
        }
    }
}
