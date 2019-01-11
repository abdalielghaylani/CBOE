using PerkinElmer.COE.Inventory.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerkinElmer.COE.Inventory.DAL.Mapper
{
    public sealed class CompoundMapper : MapperBase<INV_COMPOUNDS, CompoundData>
    {
        public override INV_COMPOUNDS Map(CompoundData element)
        {
            // convert from DTO to Entity
            throw new NotImplementedException();
        }

        public override CompoundData Map(INV_COMPOUNDS element)
        {
            if (element == null) return null;

            return new CompoundData
            {
                CompoundId = element.COMPOUND_ID,
                MolId = element.MOL_ID,
                Cas = element.CAS,
                AcxId = element.ACX_ID,
                SubstanceName = element.SUBSTANCE_NAME,
                Base64Cdx = element.BASE64_CDX,
                MolecularWeight = element.MOLECULAR_WEIGHT,
                Density = element.DENSITY
            };
        }
    }
}
