using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerkinElmer.COE.Inventory.Model;

namespace PerkinElmer.COE.Inventory.DAL.Mapper
{
    public sealed class UnitMapper : MapperBase<INV_UNITS, UnitData>
    {
        public override Array GetOracleParameters(UnitData element)
        {
            throw new NotImplementedException();
        }

        public override INV_UNITS Map(UnitData element)
        {
            // convert from DTO to Entity
            throw new NotImplementedException();
        }

        public override UnitData Map(INV_UNITS element)
        {
            if (element == null) return null;

            var unitData = new UnitData
            {
                Id = element.UNIT_ID,
                Unit = element.UNIT_ABREVIATION,
                Description = element.UNIT_NAME,
            };

            if (element.INV_UNIT_TYPES != null)
            {
                unitData.UnitType = new UnitTypeData() { UnitTypeId = element.INV_UNIT_TYPES.UNIT_TYPE_ID, Name = element.INV_UNIT_TYPES.UNIT_TYPE_NAME };
            }

            return unitData;
        }
    }
}
