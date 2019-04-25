using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerkinElmer.COE.Inventory.Model;

namespace PerkinElmer.COE.Inventory.DAL.Mapper
{
    public sealed class LocationMapper : MapperBase<INV_LOCATIONS, LocationData>
    {
        public override Array GetOracleParameters(LocationData element)
        {
            throw new NotImplementedException();
        }

        public override INV_LOCATIONS Map(LocationData element)
        {
            // convert from DTO to Entity
            throw new NotImplementedException();
        }

        public override LocationData Map(INV_LOCATIONS element)
        {
            if (element == null) return null;

            var location = new LocationData
            {
                Id = element.LOCATION_ID,
                ParentId = element.PARENT_ID,
                Name = element.LOCATION_NAME,
                Description = element.LOCATION_DESCRIPTION,
                Barcode = element.LOCATION_BARCODE
            };

            if (element.INV_LOCATION_TYPES != null)
            {
                location.LocationType = new LocationTypeData() { LocationTypeId = element.INV_LOCATION_TYPES.LOCATION_TYPE_ID, Name = element.INV_LOCATION_TYPES.LOCATION_TYPE_NAME };
            }

            return location;
        }
    }
}
