using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectr.Model
{
    internal class EmployerPosition
    {
        public int PositionID { get; set; }
        public string PositionName { get; set; }
    } 

    internal class Employer
    {
        public int EmployerID { get; set; }
        public string EmFirstName { get; set; }
        public string EmSecondName { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Salary { get; set; }
        public int PositionID { get; set; }

        public EmployerPosition Position { get; set; }
    }
}
