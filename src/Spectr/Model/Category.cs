using System;

namespace Spectr.Model
{
    public class RepairCategory
    {
        public int CategoryID { get; set; }
        public string Category { get; set; }
    }

    /// <summary>
    /// Заказы/Категории на случай многие ко многим (Категорий ремонта может быть неск)
    /// </summary>
    public class RepairCategoryJunction
    {
        public int JunctionID { get; set; }
        public int OrderID { get; set; }
        public int CategoryID { get; set; }


        public RepairOrder Repair { get; set; }
        public RepairCategory Category { get; set; }
    }
}
