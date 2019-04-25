using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerkinElmer.COE.Inventory.API.Tests.Models;
using PerkinElmer.COE.Inventory.DAL;

namespace PerkinElmer.COE.Inventory.API.Tests
{
    public class TestInventoryDBContext : IInventoryDBContext
    {
        public TestInventoryDBContext()
        {
            this.INV_LOCATIONS = new TestDbSet<INV_LOCATIONS>();
            this.INV_LOCATIONS1 = new TestDbSet<INV_LOCATIONS>();
            this.INV_CONTAINERS = new TestDbSet<INV_CONTAINERS>();
            this.INV_CONTAINER_STATUS = new TestDbSet<INV_CONTAINER_STATUS>();
            this.INV_CONTAINER_TYPES = new TestDbSet<INV_CONTAINER_TYPES>();
            this.INV_SUPPLIERS = new TestDbSet<INV_SUPPLIERS>();
            this.INV_UNITS = new TestDbSet<INV_UNITS>();
            this.INV_COMPOUNDS = new TestDbSet<INV_COMPOUNDS>();
            this.INV_LOCATION_TYPES = new TestDbSet<INV_LOCATION_TYPES>();
            this.INV_CUSTOM_CPD_FIELD_VALUES = new TestDbSet<INV_CUSTOM_CPD_FIELD_VALUES>();
            this.INV_CUSTOM_FIELD_GROUPS = new TestDbSet<INV_CUSTOM_FIELD_GROUPS>();
            this.INV_CUSTOM_FIELDS = new TestDbSet<INV_CUSTOM_FIELDS>();
        }

        public DbSet<INV_COMPOUNDS> INV_COMPOUNDS { get; set; }
        public DbSet<INV_LOCATIONS> INV_LOCATIONS { get; set; }
        public DbSet<INV_LOCATIONS> INV_LOCATIONS1 { get; set; }
        public DbSet<INV_CONTAINERS> INV_CONTAINERS { get; set; }
        public DbSet<INV_CONTAINER_STATUS> INV_CONTAINER_STATUS { get; set; }
        public DbSet<INV_CONTAINER_TYPES> INV_CONTAINER_TYPES { get; set; }
        public DbSet<INV_SUPPLIERS> INV_SUPPLIERS { get; set; }
        public DbSet<INV_UNITS> INV_UNITS { get; set; }
        public DbSet<INV_LOCATION_TYPES> INV_LOCATION_TYPES { get; set; }
        public DbSet<INV_CUSTOM_CPD_FIELD_VALUES> INV_CUSTOM_CPD_FIELD_VALUES { get; set; }
        public DbSet<INV_CUSTOM_FIELD_GROUPS> INV_CUSTOM_FIELD_GROUPS { get; set; }
        public DbSet<INV_CUSTOM_FIELDS> INV_CUSTOM_FIELDS { get; set; }

        public void Dispose()
        {
        }

        public int SaveChanges()
        {
            return 0;
        }
    }
}
