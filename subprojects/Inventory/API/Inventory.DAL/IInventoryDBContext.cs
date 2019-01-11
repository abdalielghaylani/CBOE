using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerkinElmer.COE.Inventory.DAL
{
    public interface IInventoryDBContext : IDisposable
    {
        DbSet<INV_LOCATIONS> INV_LOCATIONS { get; set; }
        DbSet<INV_COMPOUNDS> INV_COMPOUNDS { get; set; }
        DbSet<INV_CONTAINERS> INV_CONTAINERS { get; set; }
        DbSet<INV_CONTAINER_STATUS> INV_CONTAINER_STATUS { get; set; }
        DbSet<INV_CONTAINER_TYPES> INV_CONTAINER_TYPES { get; set; }
        DbSet<INV_SUPPLIERS> INV_SUPPLIERS { get; set; }
        DbSet<INV_UNITS> INV_UNITS { get; set; }
    }
}
