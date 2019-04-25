using PerkinElmer.COE.Inventory.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerkinElmer.COE.Inventory.DAL.Mapper
{
    public sealed class CustomFieldMapper : MapperBase<INV_CUSTOM_CPD_FIELD_VALUES, CustomFieldData>
    {
        public override Array GetOracleParameters(CustomFieldData element)
        {
            throw new NotImplementedException();
        }

        public override INV_CUSTOM_CPD_FIELD_VALUES Map(CustomFieldData element)
        {
            // convert from DTO to Entity
            throw new NotImplementedException();
        }

        public override CustomFieldData Map(INV_CUSTOM_CPD_FIELD_VALUES element)
        {
            if (element == null || element.INV_CUSTOM_FIELDS == null || element.INV_CUSTOM_FIELDS.INV_CUSTOM_FIELD_GROUPS == null) return null;

            return new CustomFieldData
            {
                CustomFieldId = element.INV_CUSTOM_FIELDS.CUSTOM_FIELD_ID,
                CustomFielName = element.INV_CUSTOM_FIELDS.CUSTOM_FIELD_NAME,
                CustomFielGroupName = element.INV_CUSTOM_FIELDS.INV_CUSTOM_FIELD_GROUPS.CUSTOM_FIELD_GROUP_NAME
            };
        }
    }
}
