using System;

namespace Spectr.Model
{
    internal class RepairCategory
    {
        public int CategoryID { get; set; }
        public string Category { get; set; }
    }

    internal class RepairCategoryJunction
    {
        public int JunctionID { get; set; }
        public int OrderID { get; set; }
        public int CategoryID { get; set; }


        public RepairOrder Repair { get; set; }
        public RepairCategory Category { get; set; }
    }
}
