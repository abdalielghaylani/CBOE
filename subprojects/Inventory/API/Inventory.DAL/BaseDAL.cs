using System;
using System.Collections.Generic;
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
    }
}
