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
        public override INV_LOCATIONS Map(LocationData element)
        {
            // convert from DTO to Entity
            throw new NotImplementedException();
        }

        public override LocationData Map(INV_LOCATIONS element)
        {
            if (element == null) return null;

            return new LocationData
            {
                Id = element.LOCATION_ID,
                Name = element.LOCATION_NAME
            };
        }
    }
}
