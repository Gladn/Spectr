using System;


namespace Spectr.Model
{
    public class EmployerPosition
    {
        public int PositionID { get; set; }
        public string PositionName { get; set; }
    }

    public class Employer
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
