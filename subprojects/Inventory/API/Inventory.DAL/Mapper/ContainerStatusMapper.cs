using PerkinElmer.COE.Inventory.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerkinElmer.COE.Inventory.DAL.Mapper
{
    public sealed class ContainerStatusMapper : MapperBase<INV_CONTAINER_STATUS, ContainerStatusData>
    {
        public override Array GetOracleParameters(ContainerStatusData element)
        {
            throw new NotImplementedException();
        }

        public override INV_CONTAINER_STATUS Map(ContainerStatusData element)
        {
            // convert from DTO to Entity
            throw new NotImplementedException();
        }

        public override ContainerStatusData Map(INV_CONTAINER_STATUS element)
        {
            if (element == null) return null;

            return new ContainerStatusData
            {
                StatusId = element.CONTAINER_STATUS_ID,
                Name = element.CONTAINER_STATUS_NAME
            };
        }
    }
}
