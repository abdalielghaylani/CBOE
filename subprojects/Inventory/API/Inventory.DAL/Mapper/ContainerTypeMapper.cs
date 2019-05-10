using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerkinElmer.COE.Inventory.Model;

namespace PerkinElmer.COE.Inventory.DAL.Mapper
{
    public sealed class ContainerTypeMapper : MapperBase<INV_CONTAINER_TYPES, ContainerTypeData>
    {
        public override Array GetOracleParameters(ContainerTypeData element)
        {
            throw new NotImplementedException();
        }

        public override INV_CONTAINER_TYPES Map(ContainerTypeData element)
        {
            // convert from DTO to Entity
            throw new NotImplementedException();
        }

        public override ContainerTypeData Map(INV_CONTAINER_TYPES element)
        {
            if (element == null) return null;

            return new ContainerTypeData
            {
                ContainerTypeId = element.CONTAINER_TYPE_ID,
                Name = element.CONTAINER_TYPE_NAME
            };
        }
    }
}
