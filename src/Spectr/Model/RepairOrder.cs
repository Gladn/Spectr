using System;
using System.Collections.Generic;


namespace Spectr.Model
{
    internal class RepairOrder
    {
        public int OrderID { get; set; }
        public DateTime DateStart { get; set; }
        public int CustomerID { get; set; }
        public int DeviceID { get; set; }
        public int EmployerID { get; set; }
        public DateTime PlainDateEnd { get; set; }
        public bool Status { get; set; }
        public decimal? Discount { get; set; }
        public decimal TotalCost { get; set; }
        public string Comment { get; set; }

        public Customer Customer { get; set; }
        public Device Device { get; set; }
        public Employer Employer { get; set; }

        public List<RepairCategoryJunction> Categories { get; set; }
    }
}
