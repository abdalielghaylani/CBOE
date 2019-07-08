using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerkinElmer.COE.Inventory.DAL
{
    public class BaseDAL
    {
        protected readonly IInventoryDBContext db = new InventoryDB();

        public BaseDAL()
        {
        }

        public BaseDAL(IInventoryDBContext context)
        {
            db = context;
        }

        public void RefreshDBContext()
        {
            var context = ((IObjectContextAdapter)db).ObjectContext;
            var refreshableObjects = (from entry in context.ObjectStateManager.GetObjectStateEntries(
                                                       EntityState.Added
                                                       | EntityState.Deleted
                                                       | EntityState.Modified
                                                       | EntityState.Unchanged)
                                      where entry.EntityKey != null
                                      select entry.Entity).ToList();

            context.Refresh(RefreshMode.StoreWins, refreshableObjects);
        }
    }
}
